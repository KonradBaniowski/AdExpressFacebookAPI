using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace KM.AdExpress.AlertPreRoll.Domain.XmlLoaders
{
  public  class PreRollInformationLoader
    {
       public static List<PreRollInformation> Load(string filePath)
          {
              var xDoc = XDocument.Load(filePath);

              var query = from c in xDoc.Descendants("media")
                          select new PreRollInformation
                          {
                             
                          };

              return query.ToList();
          }
      
    }
}
