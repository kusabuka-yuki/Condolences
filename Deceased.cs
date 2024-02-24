using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Condolences
{
    internal class Deceased
    {
        public string Name { get; private set; }
        public string Area { get; private set; }
        public int Age { get; private set; }
        public string Reason { get; private set; }
        public string Description { get; private set; }
        public string PublishedAt { get; private set; }
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
