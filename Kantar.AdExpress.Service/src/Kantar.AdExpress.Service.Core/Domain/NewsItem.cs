using System;
using System.Xml.Serialization;

namespace Kantar.AdExpress.Service.Core.Domain
{
    [Serializable()]
    public class NewsItem
    {
        [XmlElement("Title")]
        public string Title { get; set; }

        [XmlElement("Date")]
        public DateTime Date { get; set; }

        [XmlElement("Author")]
        public string Author { get; set; }

        [XmlElement("Content")]
        public string Content { get; set; }

        public string Day => Date.ToString("dd");

        public string Month { get; set; }
    }
}
