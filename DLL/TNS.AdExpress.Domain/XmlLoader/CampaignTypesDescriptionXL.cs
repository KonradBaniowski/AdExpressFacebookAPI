#region Information
//  Author : G. Facon
//  Creation  date: 05/08/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Domain.CampaignTypes;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Domain.XmlLoader {
    /// <summary>
    /// Load CampaignTypes descriptions from XML files
    /// </summary>
    public class CampaignTypesDescriptionXL
    {

        /// <summary>
        /// Load CampaignType description list
        /// </summary>
        /// <param name="source">source</param>
        /// <returns>flag list</returns>
        public static List<CampaignTypeInformation> Load(IDataSource source) {

            #region Variables
            List<CampaignTypeInformation> list = new List<CampaignTypeInformation>();
            XmlTextReader reader = null;
            string id;
            int webTextId;                   
            CampaignTypeInformation campaignType;
            #endregion

            try {
                source.Open();
                reader = (XmlTextReader)source.GetSource();
                while(reader.Read()) {
                    if(reader.NodeType == XmlNodeType.Element) {
                        switch(reader.LocalName) {
                            case "campaignType":
                                if(reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid id parameter"));
                                id = reader.GetAttribute("id");                                
                                if(reader.GetAttribute("webTextId") == null || reader.GetAttribute("webTextId").Length == 0) throw (new InvalidXmlValueException("Invalid webTextId parameter"));
                                webTextId = int.Parse(reader.GetAttribute("webTextId"));                               
                                campaignType = new CampaignTypeInformation(id,webTextId);
                                list.Add(campaignType);
                                break;
                        }
                    }
                }
            }
            catch(System.Exception err) {

                #region Close the file
                if(source.GetSource() != null) source.Close();
                #endregion

                throw (new Exception(" Error : ",err));
            }
            source.Close();
            return (list);

        }
    }
}
