using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace KM.AdExpress.AudioVideoConverter.Domain.XmlLoaders
{
    public class ServerLinkConfigurationLoader
    {
        public static ServerLink Load(string filePath)
        {
            var xDoc = XDocument.Load(filePath);

            var query = from c in xDoc.Descendants("LinkServer")
                        select new ServerLink
                        {
                            Host = (string)c.Attribute("host"),
                            Port = (int)c.Attribute("port"),
                        };


            return query.FirstOrDefault();
        }
    }
}
