using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using KM.AdExpress.BlurPressCreative.Domain.Types;

namespace KM.AdExpress.BlurPressCreative.Domain.XmlLoaders
{
    public class MediaInformationLoader
    {
        public static List<MediaInformation> Load(string filePath)
        {
            var xDoc = XDocument.Load(filePath);

            var query = from c in xDoc.Descendants("media")
                        select new MediaInformation
                            {
                                Id = Convert.ToInt64(c.Attribute("id").Value),
                                Label = Convert.ToString(c.Attribute("label").Value),
                                Periodicity = c.Attribute("periodicity").Value,
                                LatestBlurDate = c.Attribute("latestBlurDate").Value                                    
                            };
                                                       
            return query.ToList();
        }
    }
}
