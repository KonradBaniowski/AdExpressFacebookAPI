using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LinkSystem.LinkKernel.Core;

namespace KM.AdExpress.PPTX.Domain.XmlLoaders
{
    public class ModuleDescriptionLoader
    {
        public static List<ModuleDescription> Load(string filePath)
        {
            var xDoc = XDocument.Load(filePath);

            var query = from c in xDoc.Descendants("moduleDescription")
                        select new ModuleDescription((string)c.Attribute("description"), (int)c.Attribute("moduleId"), (bool)c.Attribute("isSelectable"));

            return query.ToList();
        }
    }
}
