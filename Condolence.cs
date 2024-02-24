using AngleSharp;
using AngleSharp.Dom;
using MySqlConnector;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Condolences
{
    internal class Condolence
    {
        private string address = @"https://memorix-shinmai.com/condolences_list";
        private Regex regex = new Regex(@"(\S+)\((\S+)\)さん(\d.+)歳【(\S+)掲載】");
        private string connectionString = ConfigurationManager.ConnectionStrings["mariadbconnect"].ConnectionString;
        private List<Deceased> Deceaseds= new List<Deceased>();

        private bool success = false;
        private int addedCount = 0;
        private enum ECondolencesList
        {
            Name_Kanji,
            Name_Kana,
            Age,
            PublishedAt,
        }
        public Condolence()
        {
            return;
        }
        public async void Begin()
        {
            // htmlからデータを取得
            await AnalysisHtml();
            // DBに登録する。
            Save();
            // 終了
            if (success)
            {
                Console.WriteLine($"正常終了しました。対象件数:{addedCount}");
            }
            else
            {
                Console.WriteLine("異常終了しました。");
            }
            return;
        }
        /// <summary>
        /// // htmlからデータを取得
        /// </summary>
        /// <returns></returns>
        private async Task AnalysisHtml()
        {
            try
            {
                // htmlの取得
                var angleSharp = new AngleSharpLoader(address);

                // 全体の情報
                var document = await angleSharp.Context.OpenAsync(address);
                // 地域ごとの固まりを取得
                var parentFrame = document.GetElementById("parent_frame");
                if(parentFrame == null) { return; }
                var childFrame = parentFrame.GetElementsByClassName("card");

                foreach(var (childElement, idx) in childFrame.Select((childElement, idx)=>(childElement, idx+1)))
                {
                    // 都市ごとの固まりを取得
                    var areaIdElement = document.GetElementById($"area_{idx}");

                    if (areaIdElement == null)
                    {
                        return;
                    }
                    var cardElements = areaIdElement.GetElementsByClassName("card-body");

                    if (cardElements.Count() <= 0) { return; }
                    var cardElement = cardElements[0];

                    var condolenceRows = cardElement.GetElementsByClassName("card-body condolence-list");

                    foreach ((IElement condolence, int cIdx) in condolenceRows.Select((condolence,cIdx) => (condolence, cIdx+1)))
                    {
                        if(condolence.ParentElement == null) { continue; }
                        // 都市名を取得
                        var areaElement = condolence.ParentElement.GetElementsByTagName("h5").First();

                        var infoElementsParentSelector = condolence.GetElementsByClassName("row condolence-row");

                        foreach (IElement infoElementParent in infoElementsParentSelector)
                        {
                            // リンクがあるかの判定に使用
                            var infoElements = infoElementParent.GetElementsByClassName("condolence-check cursor_pointer");
                            var infoSelector = string.Empty;
                            IElement? info = null;
                            if (infoElements.Count() > 0)
                            {
                                // リンクがあれば
                                infoSelector = @"//a[@name='condolences']";
                                info = infoElementParent.GetElementsByTagName("a").First();
                            }
                            else
                            {
                                // なければ
                                infoSelector = @"//div[@class='row condolence-row']//span[@class='condolence-check']/following-sibling::span[1]";
                                info = infoElementParent.GetElementsByClassName("condolence-check").First().NextElementSibling;
                            }
                            // 故人
                            if (info == null) { continue; }
                            var infoText = info.InnerHtml.Trim().Replace("\r", "").Replace("\n", "").Replace("　", "").Replace(" ", "");

                            // 分割して取得
                            var infoItems = regex.Split(infoText).ToList();
                            infoItems.Remove("");
                            string name_kanji = infoItems[(int)ECondolencesList.Name_Kanji];
                            if (!int.TryParse(infoItems[(int)ECondolencesList.Age], out int age))
                            {
                                age = -1;
                            }
                            string publishedAt = infoItems[(int)ECondolencesList.PublishedAt];
                            string area = areaElement.InnerHtml;
                            Console.WriteLine($"area: {area} / name_kanji: {name_kanji}");
                            var deceased = new Deceased(area, name_kanji, age, publishedAt);
                            Deceaseds.Add(deceased);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// DBに登録
        /// </summary>
        private void Save()
        {
            if (Deceaseds.Count <= 0) { return; }

            using (var connect = new MySqlConnection(connectionString))
            {
                connect.Open();
                try
                {
                    foreach (var deceased in Deceaseds)
                    {

                        // すでに登録されているか確認
                        var selectSql = $"SELECT COUNT(*) FROM deceaseds " +
                            $"WHERE area = '{deceased.Area}' AND name = '{deceased.Name}' AND " +
                            $"age = {deceased.Age} AND published_at = '{deceased.PublishedAt}';";

                        using (var myCommand = new MySqlCommand(selectSql, connect))
                        {
                            var result = myCommand.ExecuteScalar();

                            if (result == null || Convert.ToInt32(result) > 0)
                            {
                                // すでに登録されている場合は次へ
                                continue;
                            }
                        }

                        // 登録
                        var insertSql = $"INSERT INTO deceaseds (area, name, age, published_at) " +
                            $"VALUES ('{deceased.Area}','{deceased.Name}',{deceased.Age},'{deceased.PublishedAt}')";

                        using (var myCommand = new MySqlCommand(insertSql, connect))
                        {
                            myCommand.ExecuteNonQuery();
                        }
                        addedCount++;
                    }
                    success = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    success = false;
                }
            }
        }
    }
}
