using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace KM.AdExpress.AudioVideoConverter.DAL
{
  public  class Provider
    {
      public static string GetConnectionStringByProvider(string providerName)
      {
          // Return null on failure.
          string returnValue = null;

          // Get the collection of connection strings.
          ConnectionStringSettingsCollection settings = ConfigurationManager.ConnectionStrings;

          // Walk through the collection and return the first 
          // connection string matching the providerName.
          if (settings != null)
          {
              foreach (ConnectionStringSettings cs in settings)
              {
                  if (cs.ProviderName == providerName)
                  {
                      returnValue = cs.ConnectionString;
                      break;
                  }
              }
          }
          return returnValue;
      }
    }
}
