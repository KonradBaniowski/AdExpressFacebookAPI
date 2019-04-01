using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Kantar.AdExpress.Service.Core.Domain;

namespace Kantar.AdExpress.Service.DataAccess.Xml
{
    public class NewsXL
    {
        public static News LoadNews(string xmlPath)
        {
            var reader = new XmlSerializer(typeof(News));
            News news = new News();
            news.NewsList = new List<NewsItem>();

            try
            {
                if (File.Exists(xmlPath))
                {
                    using (var sr = new StreamReader(xmlPath))
                    {
                        news = (News) reader.Deserialize(sr);
                    }
                }
            }
            catch (System.Exception e)
            {
                news = new News();
                news.NewsList = new List<NewsItem>();
            }

            return news;
        }
    }
}
