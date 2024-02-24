using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Condolences
{
    internal class AngleSharpLoader
    {
        public string Address { get; set; }
        private IConfiguration config;
        public IBrowsingContext Context 
        {
            get
            {
                return BrowsingContext.New(config);
            }
        }
        public AngleSharpLoader(string address) 
        {
            config = Configuration.Default.WithDefaultLoader();
            Address = address;
        }
    }
}
