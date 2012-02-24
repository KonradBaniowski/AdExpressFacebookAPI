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
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Exceptions;

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
			List<DetailLevelItemInformation> allowedMediaSelectionLevelItemsList = new List<DetailLevelItemInformation>();
            List<DetailLevelItemInformation> allowedColumnLevelItems = new List<DetailLevelItemInformation>();
			List<TNS.AdExpress.Domain.Level.GenericDetailLevel> defaultMediaSelectionDetailLevels = new List<TNS.AdExpress.Domain.Level.GenericDetailLevel>();
            TNS.AdExpress.Domain.Level.GenericDetailLevel defaultMediaSelectionDetailLevel = null, trendsDefaultMediaSelectionDetailLevel = null, tempL = null;                    
			List<long> allowedUniverseLevels = new List<long>();
            XmlTextReader reader = null;
            XmlReader subtree = null;
            VehicleInformation vhInfo = null;
            string id= string.Empty;
            string defaultMediaSelectionParent = string.Empty;
            Int64 baseId=0;
            Int64 detailColumnId = 0;
            bool showInsertions=false;
            bool showCreations = false;
            bool showActiveMedia = false;
            bool needLastAvailableDate = false;
			bool autopromo = false;
            string readString = string.Empty;
            CustomerSessions.Unit defaultAllowUnit;
            AllowUnits allowUnits = new AllowUnits(); 
            Int64 mediaAgencyFlag = -1;
            #endregion

            try {
                source.Open();
                reader = (XmlTextReader)source.GetSource();
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.LocalName) {
                            case "vehicle":
                                if (id.Length > 0) {
                                    vhInfo = new VehicleInformation(id, baseId, showInsertions, showCreations, showActiveMedia, needLastAvailableDate, allowUnits, allowedMediaLevelItemsList, defaultMediaSelectionParent, mediaSelectionParentsList, detailColumnId, allowedRecapMediaLevelItemsList);
                                    allowUnits = new AllowUnits();
									vhInfo.AllowedMediaSelectionLevelItemsList = allowedMediaSelectionLevelItemsList;
                                    vhInfo.AllowedColumnDetailLevelItems = allowedColumnLevelItems;
									vhInfo.DefaultMediaSelectionDetailLevels = defaultMediaSelectionDetailLevels;
									vhInfo.DefaultMediaSelectionDetailLevel = defaultMediaSelectionDetailLevel;
                                    vhInfo.TrendsDefaultMediaSelectionDetailLevel = trendsDefaultMediaSelectionDetailLevel;
									vhInfo.Autopromo = autopromo;
									vhInfo.AllowedUniverseLevels = allowedUniverseLevels;
                                    vhInfo.MediaAgencyFlag = mediaAgencyFlag;
                                    list.Add(vhInfo);
                                    id = string.Empty;
                                    showActiveMedia = false;
                                    needLastAvailableDate = false;
                                    defaultMediaSelectionParent = string.Empty;
                                    allowedUnitsList = new List<CustomerSessions.Unit>();
                                    allowedMediaLevelItemsList = new List<DetailLevelItemInformation.Levels>();
                                    mediaSelectionParentsList = new List<DetailLevelItemInformation.Levels>();
									allowedRecapMediaLevelItemsList = new List<DetailLevelItemInformation.Levels>();
									allowedMediaSelectionLevelItemsList = new List<DetailLevelItemInformation>();
                                    allowedColumnLevelItems = new List<DetailLevelItemInformation>();
									defaultMediaSelectionDetailLevels = new List<TNS.AdExpress.Domain.Level.GenericDetailLevel>();
									defaultMediaSelectionDetailLevel = null;
                                    trendsDefaultMediaSelectionDetailLevel = null;                                  
									autopromo = false;
									allowedUniverseLevels = new List<long>();
                                    mediaAgencyFlag = -1;
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
                                if (reader.GetAttribute("needLastAvailableDate") != null && reader.GetAttribute("needLastAvailableDate").Length > 0)
                                    needLastAvailableDate = bool.Parse(reader.GetAttribute("needLastAvailableDate"));
								if (reader.GetAttribute("autopromo") != null && reader.GetAttribute("autopromo").Length > 0)
									autopromo = bool.Parse(reader.GetAttribute("autopromo"));
                                if (reader.GetAttribute("mediaAgencyFlag") != null && reader.GetAttribute("mediaAgencyFlag").Length > 0)
                                    mediaAgencyFlag = Int64.Parse(reader.GetAttribute("mediaAgencyFlag"));
                                break;
                            case "allowedUnits":
                                defaultAllowUnit = (CustomerSessions.Unit)Enum.Parse(typeof(CustomerSessions.Unit), reader.GetAttribute("defaultUnit"));
                                allowedUnitsList = new List<CustomerSessions.Unit>();
                                subtree = (XmlReader)reader.ReadSubtree();

                                while (subtree.Read()) {
                                    if (subtree.NodeType == XmlNodeType.Element) {
                                        switch (subtree.LocalName) {
                                            case "allowedUnit":
                                                readString = subtree.ReadString();
                                                if (readString == null || readString.Length == 0) throw (new InvalidXmlValueException("Invalid allowedUnit parameter"));
                                                try {
                                                    allowedUnitsList.Add((CustomerSessions.Unit)Enum.Parse(typeof(CustomerSessions.Unit), readString, true));
                                                }
                                                catch (Exception e) {
                                                    throw new Exception("Impossible to Add unit '" + readString + "' to allowedUnitsList", e);
                                                }
                                                break;
                                        }
                                    }
                                }
                                allowUnits.DefaultAllowUnit = defaultAllowUnit;
                                allowUnits.AllowUnitList = allowedUnitsList;
                                allowUnits.Verify();
                                break;
                            case "allowedMediaLevelItem":
                                readString = reader.ReadString();
                                if (readString == null || readString.Length == 0) throw (new InvalidXmlValueException("Invalid allowedMediaLevelItem parameter"));
                                allowedMediaLevelItemsList.Add((DetailLevelItemInformation.Levels)Enum.Parse(typeof(DetailLevelItemInformation.Levels), readString, true));
                                break;                               
                            case "allowedColumnDetailLevelItem":
                                if (reader.GetAttribute("id") != null)
                                {
                                    allowedColumnLevelItems.Add(DetailLevelItemsInformation.Get(int.Parse(reader.GetAttribute("id"))));
                                }
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
							case "defaultMediaSelectionDetailLevels":
                                if (reader.GetAttribute("trendsDefaultMediaSelectionDetailLevel") != null && reader.GetAttribute("trendsDefaultMediaSelectionDetailLevel").Length > 0)
                                {
                                    trendsDefaultMediaSelectionDetailLevel = DetailLevelsInformation.Get(int.Parse(reader.GetAttribute("trendsDefaultMediaSelectionDetailLevel")));
                                    trendsDefaultMediaSelectionDetailLevel.FromControlItem = TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels;
                                }
                                if (reader.GetAttribute("baseDefaultMediaSelectionDetailLevel") == null || reader.GetAttribute("baseDefaultMediaSelectionDetailLevel").Length == 0) throw (new InvalidXmlValueException("Invalid baseDefaultMediaSelectionDetailLevel attribute parameter"));
                                defaultMediaSelectionDetailLevel = DetailLevelsInformation.Get(int.Parse(reader.GetAttribute("baseDefaultMediaSelectionDetailLevel")));
                                defaultMediaSelectionDetailLevel.FromControlItem = TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels;
								break;
							case "defaultMediaSelectionDetailLevel":
								if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid defaultMediaDetailLevel parameter"));
								defaultMediaSelectionDetailLevels.Add(DetailLevelsInformation.Get(int.Parse(reader.GetAttribute("id"))));
								break;
							case "allowedMediaSelectionLevelItem":
								if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid selectionAllowedMediaLevelItem parameter"));
								allowedMediaSelectionLevelItemsList.Add(DetailLevelItemsInformation.Get(int.Parse(reader.GetAttribute("id"))));
								break;
							case "allowedUniverseLevel":
								if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid allowedUniverseLevel parameter"));
								allowedUniverseLevels.Add(int.Parse(reader.GetAttribute("id")));
								break;
                            case "AsyncExportDetailLevel":
                                if (!string.IsNullOrEmpty(reader.GetAttribute("id")))
                                    vhInfo.AsyncExportDetailLevel =
                                        Convert.ToInt32(reader.GetAttribute("id"));
                                break;

                                                        
                        }
                    }
                }
                if (id.Length > 0) {
                    vhInfo = new VehicleInformation(id, baseId, showInsertions, showCreations, showActiveMedia, needLastAvailableDate, allowUnits, allowedMediaLevelItemsList, defaultMediaSelectionParent, mediaSelectionParentsList, detailColumnId, allowedRecapMediaLevelItemsList);
					vhInfo.AllowedMediaSelectionLevelItemsList = allowedMediaSelectionLevelItemsList;
                    vhInfo.AllowedColumnDetailLevelItems = allowedColumnLevelItems;
					vhInfo.DefaultMediaSelectionDetailLevels = defaultMediaSelectionDetailLevels;
					vhInfo.DefaultMediaSelectionDetailLevel = defaultMediaSelectionDetailLevel;
                    vhInfo.TrendsDefaultMediaSelectionDetailLevel = trendsDefaultMediaSelectionDetailLevel;
					vhInfo.Autopromo = autopromo;
					vhInfo.AllowedUniverseLevels = allowedUniverseLevels;
                    vhInfo.MediaAgencyFlag = mediaAgencyFlag;
                    list.Add(vhInfo);
                }
            }
            catch (System.Exception err) {

                #region Close the file
                if (source.GetSource() != null) source.Close();
                #endregion

                throw (new VehiclesInformationXLException(" Error : ", err));
            }

            source.Close();
            return (list);
        
        }

    }
}
