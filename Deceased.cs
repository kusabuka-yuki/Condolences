using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Condolences
{
    internal class Deceased
    {
        private string publishedAtStr;
        private Regex regex = new Regex(@"^(\d{2})月(\d{2})日$");

        public string Name { get; private set; }
        public string Area { get; private set; }
        public int Age { get; private set; }
        public string Reason { get; private set; }
        public string Description { get; private set; }
        public string PublishedAt 
        {
            get 
            {
                DateTime dtNow = DateTime.Now;

                // 年 (Year) を取得する
                int iYear = dtNow.Year;
                var infoItems = regex.Split(publishedAtStr).ToList();
                infoItems.RemoveAll(s => string.IsNullOrEmpty(s));
                var month = infoItems[0];
                var date = infoItems[1];

                var publishedAt = $"{iYear}/{month}/{date}";
                return publishedAt;
            }
            private set  => publishedAtStr = value; 
        }
        public DateTime Anniversary { get; private set; }
        public Deceased() { }
        public Deceased(string area, string name, int age, string reason, string description,
            string publishedAt, DateTime anniversary) 
        {
            this.Area = area;
            this.Name = name;
            this.Age = age;
            this.Reason = reason;
            this.Description = description;
            this.PublishedAt = publishedAt;
            this.Anniversary= anniversary;
        }
        public Deceased(string area, string name, int age, string publishedAt)
        { 
            this.Area = area;
            this.Name = name;
            this.Age = age;
            this.PublishedAt = publishedAt;
        }
    }
}
