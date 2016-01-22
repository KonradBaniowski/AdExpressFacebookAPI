using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using KM.AdExpress.BlurPressCreative.Domain.Types;
using LinkSystem.LinkKernel.Core;

namespace KM.AdExpress.BlurPressCreative.Domain.XmlLoaders
{
    public class ProductInformationLoader
    {
        public static ProductInformations Load(string filePath)
        {

            var xDoc = XDocument.Load(filePath);

            var query = from c in xDoc.Descendants("lsClientConfiguration")
                        select new ProductInformations
                            {
                                ProductInformation = new ProductInformation
                                    {
                                        FamilyID = (int) c.Attribute("familyId"),
                                        FamilyName = (string) c.Attribute("familyName"),
                                        ProductName = (string) c.Attribute("productName"),

                                    },
                                MonitorPort = (int)c.Attribute("monitorPort")

                            };
            var prodInfo = query.FirstOrDefault();
            if (prodInfo != null)
            {
                if (Assembly.GetEntryAssembly() != null)
                    prodInfo.ProductInformation.ProductVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
                 return prodInfo;
            }
            return null;
            
        }

    }
}
