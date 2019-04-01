using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Kantar.AdExpress.Service.Core.Domain
{
    [Serializable()]
    [System.Xml.Serialization.XmlRoot("News")]
    public class News
    {
        [XmlArray("NewsList")]
        [XmlArrayItem("NewsItem", typeof(NewsItem))]
        public List<NewsItem> NewsList { get; set; }
    }
}
