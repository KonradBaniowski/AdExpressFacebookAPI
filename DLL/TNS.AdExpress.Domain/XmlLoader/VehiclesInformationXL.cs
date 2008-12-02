#region Information
//  Author : Y. R'kaina
//  Creation  date: 06/08/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Domain.XmlLoader {
    /// <summary>
    /// Load vehicles descriptions from XML files
    /// </summary>
    public class VehiclesInformationXL {
        
        /// <summary>
        /// Load vehicle description list
        /// </summary>
        /// <param name="source">source</param>
        /// <returns>vehicle information list</returns>
        public static List<VehicleInformation> Load(IDataSource source) {

            #region Variables
            List<VehicleInformation> list = new List<VehicleInformation>();
            List<CustomerSessions.Unit> allowedUnitsList = new List<CustomerSessions.Unit>();
            List<DetailLevelItemInformation.Levels> allowedMediaLevelItemsList = new List<DetailLevelItemInformation.Levels>();
            List<DetailLevelItemInformation.Levels> mediaSelectionParentsList = new List<DetailLevelItemInformation.Levels>();
			List<DetailLevelItemInformation.Levels> allowedRecapMediaLevelItemsList = new List<DetailLevelItemInformation.Levels>();
            List<DetailLevelItemInformation>  selectionAllowedMediaLevelItemsList = new List<DetailLevelItemInformation>();
            List<TNS.AdExpress.Domain.Level.GenericDetailLevel> selectionDefaultMediaDetailLevels = new List<TNS.AdExpress.Domain.Level.GenericDetailLevel>();
            XmlTextReader reader = null;
            VehicleInformation vhInfo = null;
            string id= string.Empty;
            string defaultMediaSelectionParent = string.Empty;
            Int64 baseId=0;
            Int64 detailColumnId = 0;
            bool showInsertions=false;
            bool showCreations = false;
            bool showActiveMedia = false;
            string readString = string.Empty;
            #endregion

            try {
                source.Open();
                reader = (XmlTextReader)source.GetSource();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "vehicle":
                                if (id.Length > 0) {
                                    vhInfo = new VehicleInformation(id, baseId, showInsertions, showCreations, showActiveMedia, allowedUnitsList, allowedMediaLevelItemsList, defaultMediaSelectionParent, mediaSelectionParentsList, detailColumnId, allowedRecapMediaLevelItemsList);
									//vhInfo.SelectionAllowedMediaLevelItemsList = selectionAllowedMediaLevelItemsList;
									//vhInfo.SelectionDefaultMediaDetailLevels = selectionDefaultMediaDetailLevels;
                                    list.Add(vhInfo);
                                    id = string.Empty;
                                    showActiveMedia = false;
                                    defaultMediaSelectionParent = string.Empty;
                                    allowedUnitsList = new List<CustomerSessions.Unit>();
                                    allowedMediaLevelItemsList = new List<DetailLevelItemInformation.Levels>();
                                    mediaSelectionParentsList = new List<DetailLevelItemInformation.Levels>();
									allowedRecapMediaLevelItemsList = new List<DetailLevelItemInformation.Levels>();
                                    selectionAllowedMediaLevelItemsList = new List<DetailLevelItemInformation>();
                                    selectionDefaultMediaDetailLevels = new List<TNS.AdExpress.Domain.Level.GenericDetailLevel>();
                                }
                                if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid id parameter"));
                                id = reader.GetAttribute("id");
                                if (reader.GetAttribute("databaseId") == null || reader.GetAttribute("databaseId").Length == 0) throw (new InvalidXmlValueException("Invalid baseId parameter"));
                                baseId = Int64.Parse(reader.GetAttribute("databaseId"));
                                if (reader.GetAttribute("showInsertions") == null || reader.GetAttribute("showInsertions").Length == 0) throw (new InvalidXmlValueException("Invalid showInsertions parameter"));
                                showInsertions = bool.Parse(reader.GetAttribute("showInsertions"));
                                if (reader.GetAttribute("showCreations") == null || reader.GetAttribute("showCreations").Length == 0) throw (new InvalidXmlValueException("Invalid showCreations parameter"));
                                showCreations = bool.Parse(reader.GetAttribute("showCreations"));
                                if (reader.GetAttribute("showActiveMedia") != null && reader.GetAttribute("showActiveMedia").Length > 0)
                                    showActiveMedia = bool.Parse(reader.GetAttribute("showActiveMedia"));
                                break;
                            case "allowedUnit":
                                readString = reader.ReadString();
                                if (readString == null || readString.Length == 0) throw (new InvalidXmlValueException("Invalid allowedUnit parameter"));
                                allowedUnitsList.Add((CustomerSessions.Unit)Enum.Parse(typeof(CustomerSessions.Unit), readString, true));
                                break;
                            case "allowedMediaLevelItem":
                                readString = reader.ReadString();
                                if (readString == null || readString.Length == 0) throw (new InvalidXmlValueException("Invalid allowedMediaLevelItem parameter"));
                                allowedMediaLevelItemsList.Add((DetailLevelItemInformation.Levels)Enum.Parse(typeof(DetailLevelItemInformation.Levels), readString, true));
                                break;
                            case "MediaSelectionParents": 
                                if (reader.GetAttribute("DefaultMediaSelectionParent") == null || reader.GetAttribute("DefaultMediaSelectionParent").Length == 0) throw (new InvalidXmlValueException("Invalid DefaultMediaSelectionParent parameter"));
                                defaultMediaSelectionParent = reader.GetAttribute("DefaultMediaSelectionParent");
                                break;
                            case "MediaSelectionParent":
                                readString = reader.ReadString();
                                if (readString == null || readString.Length == 0) throw (new InvalidXmlValueException("Invalid MediaSelectionParent parameter"));
                                mediaSelectionParentsList.Add((DetailLevelItemInformation.Levels)Enum.Parse(typeof(DetailLevelItemInformation.Levels), readString, true));
                                break;
                            case "detailColumn":
                                if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid detailColumn parameter"));
                                detailColumnId = Int64.Parse(reader.GetAttribute("id"));
                                break;
							case "allowedRecapMediaLevelItem":
								readString = reader.ReadString();
								if (readString == null || readString.Length == 0) throw (new InvalidXmlValueException("Invalid allowedMediaLevelItem parameter"));
								allowedRecapMediaLevelItemsList.Add((DetailLevelItemInformation.Levels)Enum.Parse(typeof(DetailLevelItemInformation.Levels), readString, true));
								break;
							//case "defaultMediaDetailLevel":
							//    if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid defaultMediaDetailLevel parameter"));
							//    selectionDefaultMediaDetailLevels.Add(DetailLevelsInformation.Get(int.Parse(reader.GetAttribute("id"))));
							//    break;
							//case "selectionAllowedMediaLevelItem":
							//    if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid selectionAllowedMediaLevelItem parameter"));
							//    selectionAllowedMediaLevelItemsList.Add(DetailLevelItemsInformation.Get(int.Parse(reader.GetAttribute("id"))));
							//    break;
                        }
                    }
                }
                if (id.Length > 0) {
                    vhInfo = new VehicleInformation(id, baseId, showInsertions, showCreations, showActiveMedia, allowedUnitsList, allowedMediaLevelItemsList, defaultMediaSelectionParent, mediaSelectionParentsList, detailColumnId, allowedRecapMediaLevelItemsList);
					//vhInfo.SelectionAllowedMediaLevelItemsList = selectionAllowedMediaLevelItemsList;
					//vhInfo.SelectionDefaultMediaDetailLevels = selectionDefaultMediaDetailLevels;
                    list.Add(vhInfo);
                }
            }
            catch (System.Exception err) {

                #region Close the file
                if (source.GetSource() != null) source.Close();
                #endregion

                throw (new Exception(" Error : ", err));
            }

            source.Close();
            return (list);
        
        }

    }
}
