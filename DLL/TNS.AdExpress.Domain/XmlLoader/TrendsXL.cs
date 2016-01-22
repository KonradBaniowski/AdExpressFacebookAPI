using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.ModulesDescritpion;
using TNS.FrameWork.DB.Common;
using System.Xml;
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Exceptions;

namespace TNS.AdExpress.Domain.XmlLoader
{
    public class TrendsXL
    {
         /// Load Trends options
        /// </summary>
        /// <param name="dataSource">Data Source</param>
        public static void LoadTrends(IDataSource dataSource, Dictionary<Vehicles.names, TrendsDateOpeningOption> trendsDateOpeningOptions)
        {
            XmlTextReader reader = null;
            XmlReader subtree = null;
            Vehicles.names id;
            Tendencies.DateOpeningOption defaultOptionId;
            List<Tendencies.DateOpeningOption> optionsIds = new List<Tendencies.DateOpeningOption>();
            TrendsDateOpeningOption oTrend = null;
             try {
				 reader = (XmlTextReader)dataSource.GetSource();
                 while (reader.Read())
                 {
                     if (reader.NodeType == XmlNodeType.Element)
                     {
                         switch (reader.LocalName)
                         {
                             case "dateOpeningOption":
                                 if (reader.GetAttribute("vehicleId") == null || reader.GetAttribute("vehicleId").Length == 0) throw (new InvalidXmlValueException("Invalid vehicleId parameter"));
                                 id = (Vehicles.names)Enum.Parse(typeof(Vehicles.names), reader.GetAttribute("vehicleId"), true);
                                 if (reader.GetAttribute("defaultOption") == null || reader.GetAttribute("defaultOption").Length == 0) throw (new InvalidXmlValueException("Invalid defaultOption parameter"));
                                 defaultOptionId = (Tendencies.DateOpeningOption)Enum.Parse(typeof(Tendencies.DateOpeningOption), reader.GetAttribute("defaultOption"), true);
                                 optionsIds = new List<Tendencies.DateOpeningOption>();
                                 subtree = (XmlReader)reader.ReadSubtree();
                                 while (subtree.Read())
                                 {
                                     if (subtree.NodeType == XmlNodeType.Element)
                                     {
                                         switch (subtree.LocalName)
                                         {
                                             case "option":
                                                 string subVal = subtree.ReadString();
                                                 if (subVal == null || subVal.Length == 0) throw (new InvalidXmlValueException("Invalid option parameter"));
                                                 optionsIds.Add((Tendencies.DateOpeningOption)Enum.Parse(typeof(Tendencies.DateOpeningOption), subVal, true));
                                                 break;
                                         }
                                     }
                                 }
                                 oTrend = new TrendsDateOpeningOption(id, defaultOptionId, optionsIds);
                                 trendsDateOpeningOptions.Add(id, oTrend);
                                 break;
                           
                         }
                     }
                 }
                 #region Close the file
                 if (reader != null) reader.Close();
                 #endregion
             }

             #region Errors management
             catch (System.Exception e)
             {
                 #region Close the file
                 if (reader != null) reader.Close();
                 #endregion
                 throw (new TrendsXLException("Impossible to load Trends XML file", e));
             }
             #endregion
        }
    }
}
