using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LinkSystem.LinkKernel.CoreMonitor;

namespace KM.AdExpress.PPTX.Domain.XmlLoaders
{
    public class MailerConfigLoader
    {
        public static MailerConfig Load(string filePath)
        {
            var xDoc = XDocument.Load(filePath);

            var query = from c in xDoc.Descendants("server")
                        select new MailerConfig
                        {
                            PrimarySmtpPort = (int)c.Attribute("port"),
                            PrimarySmtpServer = (string)c.Attribute("adress"),
                        };

            var mailerConfig = query.FirstOrDefault();
            if (mailerConfig != null)
            {
                var queryFrom = from c in xDoc.Descendants("message")
                                select (string)c.Attribute("from");

                mailerConfig.SenderEmail = queryFrom.FirstOrDefault();

                var queryTo = from c in xDoc.Descendants("to")
                              select c.Value;

                mailerConfig.DestEmails.AddRange(queryTo.ToList());
            }
            return mailerConfig;
        }
    }
}
