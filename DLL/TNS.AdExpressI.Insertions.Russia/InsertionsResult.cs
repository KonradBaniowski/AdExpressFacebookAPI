

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using CstWeb = TNS.AdExpress.Constantes.Web;
using CsCustomer = TNS.AdExpress.Constantes.Customer;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstFlags = TNS.AdExpress.Constantes.DB.Flags;

using TNS.AdExpressI.Insertions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Classification;
using System.Data;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.FrameWork.Date;
using TNS.AdExpressI.Insertions.Russia.Cells;
using WebCore = TNS.AdExpress.Web.Core;

namespace TNS.AdExpressI.Insertions.Russia
{
    public class InsertionsResult : TNS.AdExpressI.Insertions.InsertionsResult
    {

        #region Constantes
        protected const char SEPARATOR = '°';
        protected const string brClass = "class=\"mso\"";
        protected const string CARRIAGE_RETURN = "<br>";
        protected const string EXCEL_CARRIAGE_RETURN = " ";
        protected const int TV_TIME_START_OFFSET = 5;
        protected const int RADIO_TIME_START_OFFSET = 6;
        protected const string SCAN_LOW = "scan_low";
        #endregion

        #region Constructor
        /// <summary>
        /// DEfault constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="moduleId">Module ID</param>
        public InsertionsResult(WebSession session, Int64 moduleId)
            : base(session, moduleId)
        {
        }
        #endregion

        #region GetInsertions
        public override ResultTable GetInsertions(VehicleInformation vehicle, int fromDate, int toDate, string filters, int universId, string zoomDate)
        {
            this._getCreatives = false;
            this._zoomDate = zoomDate;
            this._universId = universId;
            if (!CanShowInsertion(vehicle)) return null;
            return GetData(vehicle, fromDate, toDate, filters, universId);
        }
        #endregion

        #region GetCreatives
        public override ResultTable GetCreatives(VehicleInformation vehicle, int fromDate, int toDate, string filters, int universId, string zoomDate)
        {
            this._getCreatives = true;
            this._zoomDate = zoomDate;
            this._universId = universId;
            return GetData(vehicle, fromDate, toDate, filters, universId);
        }
        #endregion

        #region GetMSCreatives
        public override ResultTable GetMSCreatives(VehicleInformation vehicle, int fromDate, int toDate, string filters, int universId, string zoomDate)
        {
            this._getMSCreatives = true;
            this._zoomDate = zoomDate;
            this._universId = universId;
            return GetData(vehicle, fromDate, toDate, filters, universId);
        }
        #endregion

        #region GetCreativeLinks
        /// <summary>
        /// Get creative Links
        /// </summary>
        /// <param name="idVehicle">Identifier Vehicle</param>
        /// <param name="currentRow">Current row</param>
        /// <returns>Creative Links string</returns>
        public override string GetCreativeLinks(long idVehicle, DataRow currentRow)
        {
            string vignettes = "";
            string sloganDetail = "";
            string imagesList = "";
            string[] fileList = null;
            string dateField = "";
            string pathWeb = "";
            bool first = true;
            string themeName = WebApplicationParameters.Themes[_session.SiteLanguage].Name;
            long idVersion = long.MinValue;
            TNS.AdExpress.Constantes.Classification.DB.Vehicles.names vehicleName = VehiclesInformation.DatabaseIdToEnum(idVehicle);
            string encryptedParams = "";

            if (_session.CustomerLogin.ShowCreatives(VehiclesInformation.DatabaseIdToEnum(idVehicle)))
            {//droit créations
                if (currentRow["associated_file"] != null && currentRow["associated_file"].ToString().Length > 0)
                {
                    switch (vehicleName)
                    {

                        case Vehicles.names.press:
                        case Vehicles.names.pressClipping:
                        case Vehicles.names.editorial:
                        case Vehicles.names.outdoor:
                            #region Construction de la liste des images presse

                            if (currentRow["associated_file"] != DBNull.Value && currentRow["slogan"] != DBNull.Value && !string.IsNullOrEmpty(currentRow["slogan"].ToString()))
                            {
                                fileList = currentRow["associated_file"].ToString().Split(',');
                                idVersion = Convert.ToInt64(currentRow["slogan"].ToString());
                                for (int j = 0; j < fileList.Length; j++)
                                {
                                    string encryptedParams2 = WebCore.Utilities.QueryStringEncryption.EncryptQueryString(GetCreativePathVisual(GetImagesPath(vehicleName), fileList[j], idVersion, false, GetLowDirPath(vehicleName)));

                                    vignettes += "<img src='" + TNS.AdExpress.Constantes.Web.Links.CREATIVE_VIEW_PAGE + "?path=" + encryptedParams2 + "&id_vehicle=" + idVehicle.ToString() + "&idSession=" + _session.IdSession + "&is_blur=false&crypt=1' border=\"0\" width=\"50\" height=\"64\" >";
                                    if (first) imagesList = GetCreativePathVisual(GetImagesPath(vehicleName), fileList[j], idVersion, true, "");
                                    else { imagesList += "," + GetCreativePathVisual(GetImagesPath(vehicleName), fileList[j], idVersion, true, ""); }
                                    first = false;
                                }
                                encryptedParams = "";
                                if (vignettes.Length > 0)
                                {
                                    if (!string.IsNullOrEmpty(imagesList)) encryptedParams = WebCore.Utilities.QueryStringEncryption.EncryptQueryString(imagesList);
                                    vignettes = "<a href=\"javascript:OpenWindow('" + TNS.AdExpress.Constantes.Web.Links.CREATIVE_VIEW_PAGE + "?path=" + encryptedParams + "&id_vehicle=" + idVehicle.ToString() + "&idSession=" + _session.IdSession + "&is_blur=false&crypt=1');\">" + vignettes + "</a>";
                                    vignettes += "\n<br>";
                                }

                            }
                            else vignettes = GestionWeb.GetWebWord(843, _session.SiteLanguage) + "<br>";
                            #endregion
                            break;

                        case Vehicles.names.radio:
                        case Vehicles.names.radioGeneral:
                        case Vehicles.names.radioSponsorship:
                        case Vehicles.names.radioMusic:
                            vignettes = "";
                            if (currentRow["associated_file"] != System.DBNull.Value)
                            {
                                encryptedParams = WebCore.Utilities.QueryStringEncryption.EncryptQueryString(currentRow["associated_file"].ToString());
                                CellRadioCreativeLink cellRadioCreativeLink = new CellRadioCreativeLink(encryptedParams, _session, idVehicle);
                                vignettes = "<a href=\"" + cellRadioCreativeLink.GetLink() + "\"><img border=\"0\" src=\"/App_Themes/" + themeName + "/Images/Common/Picto_Radio.gif\"></a>";
                            }
                            break;

                        case Vehicles.names.tv:
                        case Vehicles.names.tvGeneral:
                        case Vehicles.names.tvSponsorship:
                        case Vehicles.names.tvAnnounces:
                        case Vehicles.names.tvNonTerrestrials:
                            vignettes = "";
                            if (currentRow["associated_file"] != System.DBNull.Value)
                            {
                                encryptedParams = WebCore.Utilities.QueryStringEncryption.EncryptQueryString(currentRow["associated_file"].ToString());
                                CellTvCreativeLink cellTvCreativeLink = new CellTvCreativeLink(encryptedParams, _session, idVehicle);
                                vignettes = "<a href=\"" + cellTvCreativeLink.GetLink() + "\"><img border=\"0\" src=\"/App_Themes/" + themeName + "/Images/Common/Picto_pellicule.gif\"></a>";
                            }
                            break;
                        case Vehicles.names.internet:
                            vignettes = "";
                            if (currentRow["associated_file"] != System.DBNull.Value && currentRow["slogan"] != System.DBNull.Value && !string.IsNullOrEmpty(currentRow["slogan"].ToString()))
                            {
                                string internetPath = GetCreativePath(currentRow["associated_file"].ToString(), Convert.ToInt64(currentRow["slogan"].ToString()));
                                encryptedParams = WebCore.Utilities.QueryStringEncryption.EncryptQueryString(internetPath);
                                string creatives = "path=" + encryptedParams + "&id_vehicle=" + idVehicle.ToString() + "&idSession=" + _session.IdSession + "&is_blur=false&crypt=1";

                                CellInternetCreativeLink cellInternetCreativeLink = new CellInternetCreativeLink(creatives, _session);
                                vignettes = string.Format("<a href=\"" + cellInternetCreativeLink.GetLink() + "\"><img border=\"0\" src=\"/App_Themes/{1}/Images/Common/Button/adnettrack.gif\"></a>", creatives, themeName);
                            }
                            break;
                    }
                }
            }

            sloganDetail = "\n<table border=\"0\" width=\"50\" height=\"64\" class=\"txtViolet10\">";
            if (vignettes.Length > 0)
            {
                sloganDetail += "\n<tr><td   nowrap align=\"center\">";
                sloganDetail += vignettes;
                sloganDetail += "\n</td></tr>";
            }
            sloganDetail += "\n<tr><td  nowrap align=\"center\">";
            sloganDetail += currentRow["slogan"].ToString();
            if (currentRow["advertDimension"] != null && currentRow["advertDimension"] != System.DBNull.Value)
            {
                sloganDetail += " - " + currentRow["advertDimension"].ToString();
            }
            sloganDetail += "\n</td></tr>";
            sloganDetail += "\n</table>";

            return sloganDetail;

        }
        #endregion


        #region GetData
        /// <summary>
        /// Get Data
        /// </summary>
        /// <param name="vehicle">vehicle</param>
        /// <param name="fromDate">beginning date</param>
        /// <param name="toDate">end date</param>
        /// <param name="filters">filters</param>
        /// <param name="universId">universe Id</param>
        /// <returns>ResultTable</returns>
        protected override ResultTable GetData(VehicleInformation vehicle, int fromDate, int toDate, string filters, int universId)
        {
            ResultTable data = null;

            #region Data Access
            if (vehicle == null)
                return null;

            DataSet ds = null;

            if (_getMSCreatives)
            {
                ds = _dalLayer.GetMSCreativesData(vehicle, fromDate, toDate, universId, filters);
            }
            else if (_getCreatives)
            {
                ds = _dalLayer.GetCreativesData(vehicle, fromDate, toDate, universId, filters);
            }
            else
            {
                ds = _dalLayer.GetInsertionsData(vehicle, fromDate, toDate, universId, filters);
            }

            if (ds == null || ds.Equals(DBNull.Value) || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                return null;
            #endregion

            DataTable dt = ds.Tables[0];


            #region Init ResultTable
            List<DetailLevelItemInformation> levels = new List<DetailLevelItemInformation>();
            if (!_getMSCreatives)
            {
                foreach (DetailLevelItemInformation d in _session.DetailLevel.Levels)
                {
                    levels.Add((DetailLevelItemInformation)d);
                }
            }
            List<GenericColumnItemInformation> columns;

            if (this._getMSCreatives)
                columns = WebApplicationParameters.MsCreativesDetail.GetDetailColumns(vehicle.DatabaseId);
            else if (this._getCreatives)
            {
                columns = _session.GenericCreativesColumns.Columns;
            }
            else
                columns = _session.GenericInsertionColumns.Columns;

            bool hasVisualRight = false;
            switch (vehicle.Id)
            {
                case CstDBClassif.Vehicles.names.internet:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DETAIL_INTERNET_ACCESS_FLAG);
                    break;
                case CstDBClassif.Vehicles.names.press:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_PRESS_CREATION_ACCESS_FLAG);
                    break;
                case CstDBClassif.Vehicles.names.pressClipping:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_PRESS_CLIPPING_CREATION_ACCESS_FLAG);
                    break;
                case CstDBClassif.Vehicles.names.indoor:
                case CstDBClassif.Vehicles.names.outdoor:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_OUTDOOR_CREATION_ACCESS_FLAG);
                    break;
                case CstDBClassif.Vehicles.names.radio:
                case CstDBClassif.Vehicles.names.radioGeneral:
                case CstDBClassif.Vehicles.names.radioSponsorship:
                case CstDBClassif.Vehicles.names.radioMusic:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_RADIO_CREATION_ACCESS_FLAG);
                    break;
                case CstDBClassif.Vehicles.names.tv:
                case CstDBClassif.Vehicles.names.tvGeneral:
                case CstDBClassif.Vehicles.names.tvClipping:
                case CstDBClassif.Vehicles.names.tvSponsorship:
                case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                case CstDBClassif.Vehicles.names.tvAnnounces:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_TV_CREATION_ACCESS_FLAG);
                    break;
                case CstDBClassif.Vehicles.names.cinema:
                case CstDBClassif.Vehicles.names.editorial:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_EDITORIAL_CREATION_ACCESS_FLAG);
                    break;
            }

            bool hasVisuals = false;
            string divideCol = string.Empty;
            foreach (GenericColumnItemInformation c in columns)
            {
                if (c.Id == GenericColumnItemInformation.Columns.associatedFile
                    || c.Id == GenericColumnItemInformation.Columns.associatedFileMax
                    || c.Id == GenericColumnItemInformation.Columns.poster
                    || c.Id == GenericColumnItemInformation.Columns.visual
                    )
                {
                    hasVisuals = true && hasVisualRight;
                }
            }

            Int64 idColumnsSet = -1;
            if (this._getMSCreatives)
                idColumnsSet = WebApplicationParameters.MsCreativesDetail.GetDetailColumnsId(vehicle.DatabaseId);
            else if (this._getCreatives)
            {
                idColumnsSet = WebApplicationParameters.CreativesDetail.GetDetailColumnsId(vehicle.DatabaseId, _module.Id);
            }
            else
            {
                idColumnsSet = WebApplicationParameters.InsertionsDetail.GetDetailColumnsId(vehicle.DatabaseId, _module.Id);
            }

            //Data Keys
            List<GenericColumnItemInformation> keys = WebApplicationParameters.GenericColumnsInformation.GetKeys(idColumnsSet);
            List<string> keyIdName = new List<string>();
            List<string> keyLabelName = new List<string>();

            //Line Number
            int nbLine = GetLineNumber(dt, levels, keyIdName);

            //Data Columns
            List<Cell> cells = new List<Cell>();
            List<string> columnsName = GetColumnsName(dt, columns, cells);

            //Result Table init
            Headers root = GetHeaders(vehicle, columns, hasVisuals);

            if (root != null)
            {
                data = new ResultTable(nbLine, root);
            }
            else
            {
                data = new ResultTable(nbLine, 1);
            }

            Action<VehicleInformation, ResultTable, DataRow, int,
             List<GenericColumnItemInformation>, List<string>, List<Cell>, string> setLine;

            Action<VehicleInformation, ResultTable, DataRow, int,
             List<GenericColumnItemInformation>, List<string>, List<Cell>, Int64> setSpecificLine = null;

            if (_getMSCreatives)
            {
                setSpecificLine = SetMSCreativeLine;
            }
            if (_getCreatives)
            {
                setLine = SetCreativeLine;
            }
            else
            {
                setLine = SetRawLine;
            }
            #endregion

            #region Table fill
            LineType[] lineTypes = new LineType[4] { LineType.level1, LineType.level2, LineType.level3, LineType.level4 };
            Dictionary<string, Int64> levelKeyValues = null;

            string[] oldIds = new string[levels.Count];
            string[] fieldsName = new string[levels.Count];
            string[] fieldsNameInReport = new string[levels.Count];
            for (int i = 0; i < oldIds.Length; i++) { oldIds[i] = string.Empty; }
            string[] cIds = new string[levels.Count];

            Int64[] oldKeyIds = new Int64[keys.Count];
            for (int i = 0; i < oldKeyIds.Length; i++) { oldKeyIds[i] = -1; }
            Int64[] cKeyIds = new Int64[keys.Count];

            string label = string.Empty;
            int cLine = 0;
            bool isNewInsertion = false;
            string key = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                isNewInsertion = false;

                //Detail levels
                for (int k = 0; k < levels.Count; k++)
                {
                    //Detail levels fields to use for comparison
                    if (levels[k].DataBaseAliasIdField != null && dt.Columns.Contains(levels[k].DataBaseAliasIdField))
                    {
                        fieldsName[k] = levels[k].DataBaseAliasIdField;
                    }
                    else if (levels[k].DataBaseIdField != null && dt.Columns.Contains(levels[k].DataBaseIdField))
                    {
                        fieldsName[k] = levels[k].DataBaseIdField;
                    }
                    else if (levels[k].DataBaseAliasField != null && dt.Columns.Contains(levels[k].DataBaseAliasField))
                    {
                        fieldsName[k] = levels[k].DataBaseAliasField;
                    }
                    else if (levels[k].DataBaseField != null && dt.Columns.Contains(levels[k].DataBaseField))
                    {
                        fieldsName[k] = levels[k].DataBaseField;
                    }

                    fieldsNameInReport[k] = GetColumnFieldName(levels[k], dt);
                }



                for (int i = 0; i < oldIds.Length; i++)
                {
                    if (row[fieldsName[i]] != null && row[fieldsName[i]] != System.DBNull.Value)
                    {
                        cIds[i] = Convert.ToString(row[fieldsName[i]]);
                    }
                    else cIds[i] = string.Empty;

                    if (cIds[i] != oldIds[i])
                    {
                        oldIds[i] = cIds[i];
                        if (i < oldIds.Length - 1)
                        {
                            oldIds[i + 1] = string.Empty;
                        }
                        //Set current level
                        cLine = data.AddNewLine(lineTypes[i]);
                        switch (levels[i].Id)
                        {
                            case DetailLevelItemInformation.Levels.date:
                                data[cLine, 1] = new CellDate(DateString.YYYYMMDDToDateTime(row[fieldsName[i]].ToString()), string.Format("{{0:{0}}}", WebApplicationParameters.GenericColumnItemsInformation.Get(GenericColumnItemInformation.Columns.dateMediaNum.GetHashCode()).StringFormat));
                                break;
                            case DetailLevelItemInformation.Levels.duration:
                                data[cLine, 1] = new CellDuration(Convert.ToDouble(row[fieldsNameInReport[i]]));
                                ((CellUnit)data[cLine, 1]).StringFormat = string.Format("{{0:{0}}}", WebApplicationParameters.GenericColumnItemsInformation.Get(GenericColumnItemInformation.Columns.duration.GetHashCode()).StringFormat);
                                break;
                            default:
                                data[cLine, 1] = new CellLabel(SplitStringValue(row[fieldsNameInReport[i]].ToString()));
                                break;
                        }

                        for (int j = 2; j <= data.DataColumnsNumber; j++)
                        {
                            data[cLine, j] = new CellEmpty();
                        }
                    }
                }

                cLine = data.AddNewLine(lineTypes[3]);

                if (_getMSCreatives)
                    setSpecificLine(vehicle, data, row, cLine, columns, columnsName, cells, idColumnsSet);
                else
                    setLine(vehicle, data, row, cLine, columns, columnsName, cells, divideCol);
            }
            #endregion

            return data;
        }
        #endregion

        #region GetVehicles
        /// <summary>
        /// Get vehicles matching filters and which has data
        /// </summary>
        /// <param name="filters">Filters to apply (id1,id2,id3,id4</param>
        /// <returns>List of vehicles with data</returns>
        public override List<VehicleInformation> GetPresentVehicles(string filters, int universId, bool sloganNotNull)
        {
            List<VehicleInformation> vehicles = new List<VehicleInformation>();

            DateTime dateBegin = WebCore.Utilities.Dates.GetPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            DateTime dateEnd = WebCore.Utilities.Dates.GetPeriodEndDate(_session.PeriodEndDate, _session.PeriodType);
            int iDateBegin = Convert.ToInt32(dateBegin.ToString("yyyyMMdd"));
            int iDateEnd = Convert.ToInt32(dateEnd.ToString("yyyyMMdd"));
            _getCreatives = sloganNotNull;
            Int64 id = -1;

            switch (_module.Id)
            {
                case CstWeb.Module.Name.ANALYSE_CONCURENTIELLE:
                case CstWeb.Module.Name.ANALYSE_DYNAMIQUE:
                case CstWeb.Module.Name.ANALYSE_PORTEFEUILLE:
                case CstWeb.Module.Name.ANALYSE_POTENTIELS:
                case CstWeb.Module.Name.NEW_CREATIVES:
                    id = ((LevelInformation)_session.SelectionUniversMedia.Nodes[0].Tag).ID;
                    vehicles.Add(VehiclesInformation.Get(id));
                    break;
                case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA:
                case CstWeb.Module.Name.CELEBRITIES:
                    string[] ids = filters.Split(',');
                    vehicles = GetVehicles(Convert.ToInt64(ids[0]), Convert.ToInt64(ids[1]), Convert.ToInt64(ids[2]), Convert.ToInt64(ids[3]));
                    string[] list = _session.GetSelection(_session.SelectionUniversMedia, CsCustomer.Right.type.vehicleAccess).Split(',');
                    for (int i = vehicles.Count - 1; i >= 0; i--)
                    {
                        if (Array.IndexOf(list, vehicles[i].DatabaseId.ToString()) < 0)
                        {
                            vehicles.Remove(vehicles[i]);
                        }
                    }
                    break;
            }

            if (vehicles.Count <= 0)
            {
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.others));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.directMarketing));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.internet));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.adnettrack));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.evaliantMobile));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.press));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.pressClipping));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.newspaper));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.magazine));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.outdoor));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.indoor));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.radio));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.radioGeneral));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.radioSponsorship));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.radioMusic));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.tv));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.tvGeneral));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.tvSponsorship));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.tvNonTerrestrials));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.tvAnnounces));
                vehicles.Add(VehiclesInformation.Get(Vehicles.names.editorial));

            }
            for (int i = vehicles.Count - 1; i >= 0; i--)
            {
                if (_module.AllowedMediaUniverse.GetVehicles() != null && !_module.AllowedMediaUniverse.GetVehicles().Contains(vehicles[i].DatabaseId))
                {
                    vehicles.Remove(vehicles[i]);
                }
            }
            for (int i = vehicles.Count - 1; i >= 0; i--)
            {
                if ((_getCreatives && !vehicles[i].ShowCreations) || (!_getCreatives && !vehicles[i].ShowInsertions))
                {
                    vehicles.Remove(vehicles[i]);
                }
            }
            if (vehicles.Count <= 0)
            {
                return vehicles;
            }

            return _dalLayer.GetPresentVehicles(vehicles, filters, iDateBegin, iDateEnd, universId, _module, _getCreatives);

        }
        #endregion

        #region SetRawLine
        protected override void SetRawLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine, 
            List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, string divideCol)
        {
            TNS.AdExpress.Domain.Web.TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
            int i = -1;
            int j = 0;
            //int offSet = 0;
            foreach (GenericColumnItemInformation g in columns)
            {
                i++;
                j++;
                if (cells[i] is CellUnit)
                {
                    Double val = 0;
                    if (row[columnsName[i]] != System.DBNull.Value)
                    {
                        val = Convert.ToDouble(row[columnsName[i]]);
                        if (tab[cLine, j] == null)
                        {
                            tab[cLine, j] = ((CellUnit)cells[i]).Clone(val);
                        }
                        else
                        {
                            ((CellUnit)tab[cLine, j]).Add(val);
                        }
                    }
                    else
                    {
                        if (tab[cLine, j] == null)
                        {
                            tab[cLine, j] = ((CellUnit)cells[i]).Clone(null);
                        }
                    }
                }
                else
                {
                    string s = string.Empty;
                    switch (columns[i].Id)
                    {
                        case GenericColumnItemInformation.Columns.associatedFile:
                        case GenericColumnItemInformation.Columns.visual:
                            switch (vehicle.Id)
                            {
                                case CstDBClassif.Vehicles.names.tv:
                                case CstDBClassif.Vehicles.names.tvGeneral:
                                case CstDBClassif.Vehicles.names.tvClipping:
                                case CstDBClassif.Vehicles.names.tvSponsorship:
                                case CstDBClassif.Vehicles.names.tvAnnounces:
                                case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                                case CstDBClassif.Vehicles.names.tvNicheChannels:
                                case CstDBClassif.Vehicles.names.indoor:

                                    if (row[columnsName[i]] != System.DBNull.Value)
                                    {
                                        string encryptedParams = WebCore.Utilities.QueryStringEncryption.EncryptQueryString(row[columnsName[i]].ToString());
                                        tab[cLine, j] = new CellTvCreativeLink(encryptedParams, _session, vehicle.DatabaseId);
                                    }
                                    else
                                    {
                                        tab[cLine, j] = new CellEmpty();
                                    }
                                    break;
                                case CstDBClassif.Vehicles.names.radio:
                                    if (row[columnsName[i]] != System.DBNull.Value)
                                    {
                                        string encryptedParams = WebCore.Utilities.QueryStringEncryption.EncryptQueryString(row[columnsName[i]].ToString());
                                        tab[cLine, j] = new CellRadioCreativeLink(encryptedParams, _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radio));
                                    }
                                    else
                                    {
                                        tab[cLine, j] = new CellEmpty();
                                    }
                                    break;
                                case CstDBClassif.Vehicles.names.radioGeneral:
                                    if (row[columnsName[i]] != System.DBNull.Value)
                                    {
                                        string encryptedParams = WebCore.Utilities.QueryStringEncryption.EncryptQueryString(row[columnsName[i]].ToString());
                                        tab[cLine, j] = new CellRadioCreativeLink(encryptedParams, _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioGeneral));
                                    }
                                    else
                                    {
                                        tab[cLine, j] = new CellEmpty();
                                    }
                                    break;
                                case CstDBClassif.Vehicles.names.radioSponsorship:
                                     if (row[columnsName[i]] != System.DBNull.Value)
                                    {
                                        string encryptedParams = WebCore.Utilities.QueryStringEncryption.EncryptQueryString(row[columnsName[i]].ToString());
                                        tab[cLine, j] = new CellRadioCreativeLink(encryptedParams, _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioSponsorship));
                                    }
                                     else
                                     {
                                         tab[cLine, j] = new CellEmpty();
                                     }
                                    break;
                                case CstDBClassif.Vehicles.names.radioMusic:
                                      if (row[columnsName[i]] != System.DBNull.Value)
                                    {
                                        string encryptedParams = WebCore.Utilities.QueryStringEncryption.EncryptQueryString(row[columnsName[i]].ToString());
                                        tab[cLine, j] = new CellRadioCreativeLink(encryptedParams, _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioMusic));
                                    }
                                      else
                                      {
                                          tab[cLine, j] = new CellEmpty();
                                      }
                                    break;
                                case CstDBClassif.Vehicles.names.internet:
                                    if (row.Table.Columns.Contains("id_slogan") && row["id_slogan"] != System.DBNull.Value
                                        && row[columnsName[i]] != System.DBNull.Value
                                       && row.Table.Columns.Contains("associated_file") && row["associated_file"] != System.DBNull.Value)
                                    {

                                        string internetPath = GetCreativePath(row[columnsName[i]].ToString(), Convert.ToInt64(row["id_slogan"].ToString()));
                                        string encryptedParams = WebCore.Utilities.QueryStringEncryption.EncryptQueryString(internetPath);
                                        string creatives = "path=" + encryptedParams + "&id_vehicle=" + vehicle.DatabaseId.ToString() + "&idSession=" + _session.IdSession + "&is_blur=false&crypt=1";

                                        tab[cLine, j] = new CellInternetCreativeLink(creatives, _session);
                                    }
                                    else
                                    {
                                        tab[cLine, j] = new CellEmpty();
                                    }
                                    break;
                                case CstDBClassif.Vehicles.names.outdoor:
                                case CstDBClassif.Vehicles.names.editorial:
                                case CstDBClassif.Vehicles.names.press:
                                case CstDBClassif.Vehicles.names.pressClipping:
                                    if (row.Table.Columns.Contains("id_slogan") && row["id_slogan"] != System.DBNull.Value
                                        && row[columnsName[i]] != System.DBNull.Value
                                        && row.Table.Columns.Contains("associated_file") && row["associated_file"] != System.DBNull.Value)
                                    {
                                        Int64 advertisementId = Convert.ToInt64(row["id_slogan"]);
                                        string creatives = "";
                                        string[] files = row["associated_file"].ToString().Split(',');
                                        foreach (string file in files)
                                        {
                                            creatives += GetCreativePathVisual(GetImagesPath(vehicle.Id), file, advertisementId, true, "") + ",";
                                        }
                                        creatives = creatives.Substring(0, creatives.Length - 1);
                                        string encryptedParams = WebCore.Utilities.QueryStringEncryption.EncryptQueryString(creatives);
                                        creatives = TNS.AdExpress.Constantes.Web.Links.CREATIVE_VIEW_PAGE + "?path=" + encryptedParams + "&id_vehicle=" + vehicle.DatabaseId.ToString() + "&idSession=" + _session.IdSession + "&is_blur=false&crypt=1";
                                        if (vehicle.Id == CstDBClassif.Vehicles.names.outdoor)
                                            tab[cLine, j] = new CellRussiaCreativeLink(creatives, _session, "Picto_outdoor.gif");
                                        else
                                            tab[cLine, j] = new CellRussiaCreativeLink(creatives, _session, "Picto_press.gif");
                                    }
                                    else
                                    {
                                        tab[cLine, j] = new CellEmpty();
                                    }
                                    break;
                            }
                            break;

                        case GenericColumnItemInformation.Columns.product:
                            s = row[columnsName[i]].ToString();
                            if (!_session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                            {
                                s = string.Empty;
                            }

                            if (columns[i].IsContainsSeparator)
                            {
                                s = SplitStringValue(s);
                            }
                            else if (this.RenderType == RenderType.html)
                                s = TNS.FrameWork.WebResultUI.Functions.TextWrap.WrapHtml(s, textWrap.NbCharDescription, textWrap.Offset);
                            tab[cLine, j] = new CellLabel(s);
                            break;
                        case GenericColumnItemInformation.Columns.slogan:
                            s = row[columnsName[i]].ToString();
                            if (!_session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_SLOGAN_ACCESS_FLAG))
                            {
                                s = string.Empty;
                            }
                            if (columns[i].IsContainsSeparator)
                            {
                                s = SplitStringValue(s);
                            }
                            else if (this.RenderType == RenderType.html)
                                s = TNS.FrameWork.WebResultUI.Functions.TextWrap.WrapHtml(s, textWrap.NbCharDescription, textWrap.Offset);
                            tab[cLine, j] = new CellLabel(s);
                            break;
                        case GenericColumnItemInformation.Columns.dateMediaNum:
                        case GenericColumnItemInformation.Columns.firstIssueDate:
                            tab[cLine, j] = new CellDate(DateString.YYYYMMDDToDateTime(row[columnsName[i]].ToString()), string.Format("{{0:{0}}}", WebApplicationParameters.GenericColumnItemsInformation.Get(GenericColumnItemInformation.Columns.dateMediaNum.GetHashCode()).StringFormat));
                            break;
                        case GenericColumnItemInformation.Columns.duration:
                            tab[cLine, j] = new CellDuration(Convert.ToDouble(row[columnsName[i]]));
                            ((CellUnit)tab[cLine, j]).StringFormat = string.Format("{{0:{0}}}", WebApplicationParameters.GenericColumnItemsInformation.Get(GenericColumnItemInformation.Columns.duration.GetHashCode()).StringFormat);
                            break;
                        case GenericColumnItemInformation.Columns.breakFlightStart:
                        case GenericColumnItemInformation.Columns.programmeFlightStart:
                        case GenericColumnItemInformation.Columns.timestart:
                            string strTime = row[columnsName[i]].ToString();
                            string seconde = (strTime.Length == 6) ? strTime.Substring(4, 2) : strTime.Substring(3, 2);
                            string minute = (strTime.Length == 6) ? strTime.Substring(2, 2) : strTime.Substring(1, 2);
                            string hour = (strTime.Length == 6) ? strTime.Substring(0, 2) : "0" + strTime.Substring(0, 1);
                            strTime = hour + ":" + minute + ":" + seconde;
                            tab[cLine, j] = new CellLabel(strTime);
                            break;
                        default:
                            s = row[columnsName[i]].ToString();
                            if (columns[i].IsContainsSeparator)
                            {
                                s = SplitStringValue(s);
                            }
                            else if (this.RenderType == RenderType.html)
                                s = TNS.FrameWork.WebResultUI.Functions.TextWrap.WrapHtml(s, textWrap.NbCharDescription, textWrap.Offset);
                            tab[cLine, j] = new CellLabel(s);
                            break;
                    }
                }

            }
        }
        #endregion

        #region SetAggregLine
        protected override void SetAggregLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine,
            List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, string divideCol)
        {

            CellInsertionInformation c;
            List<string> visuals = new List<string>();
            if (tab[cLine, 1] == null)
            {
                switch (vehicle.Id)
                {
                    case Vehicles.names.outdoor:
                        tab[cLine, 1] = c = new CellInsertionOutdoorInformation(_session, columns, columnsName, cells, vehicle);
                        break;
                    case Vehicles.names.editorial:
                        tab[cLine, 1] = c = new CellInsertionEditorialInformation(_session, columns, columnsName, cells, vehicle);
                        break;
                    case Vehicles.names.internet:
                        tab[cLine, 1] = c = new CellInsertionInternetInformation(_session, columns, columnsName, cells, vehicle);
                        break;
                    default:
                        c = new CellInsertionInformation(_session, columns, columnsName, cells, vehicle);
                        if (vehicle.Id == Vehicles.names.pressClipping) c.ThumbnailsDirectory = "/" + SCAN_LOW;
                        tab[cLine, 1] = c;
                        break;
                }
            }
            else
            {
                c = (CellInsertionInformation)tab[cLine, 1];
            }
            foreach (GenericColumnItemInformation g in columns)
            {
                if (g.Id == GenericColumnItemInformation.Columns.visual || g.Id == GenericColumnItemInformation.Columns.associatedFile || g.Id == GenericColumnItemInformation.Columns.poster)
                {
                    visuals = new List<string>();
                    visuals = GetPath(vehicle, row, columns, columnsName);
                }
            }
            c.Add(row, visuals);

        }
        #endregion

        #region SetCreativeLine
        protected override void SetCreativeLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine,
            List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, string divideCol)
        {

            CellCreativesInformation c;
            List<string> visuals = new List<string>();
            if (tab[cLine, 1] == null)
            {
                switch (vehicle.Id)
                {
                    case CstDBClassif.Vehicles.names.radio:
                    case CstDBClassif.Vehicles.names.radioSponsorship:
                    case CstDBClassif.Vehicles.names.radioGeneral:
                    case CstDBClassif.Vehicles.names.radioMusic:
                        tab[cLine, 1] = c = new CellCreativesRadioInformation(_session, vehicle, columns, columnsName, cells, _module);
                        break;
                    case CstDBClassif.Vehicles.names.tv:
                    case CstDBClassif.Vehicles.names.tvGeneral:
                    case CstDBClassif.Vehicles.names.tvSponsorship:
                    case CstDBClassif.Vehicles.names.tvAnnounces:
                    case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                    case CstDBClassif.Vehicles.names.others:
                        tab[cLine, 1] = c = new CellCreativesTvInformation(_session, vehicle, columns, columnsName, cells, _module);
                        break;
                    case CstDBClassif.Vehicles.names.internet:
                        tab[cLine, 1] = c = new CellCreativesInternetInformation(_session, vehicle, columns, columnsName, cells, _module, _zoomDate, _universId);
                        break;
                    case CstDBClassif.Vehicles.names.outdoor:
                        tab[cLine, 1] = c = new CellCreativesOutdoorInformation(_session, vehicle, columns, columnsName, cells, _module);
                        break;
                    case CstDBClassif.Vehicles.names.editorial:
                        tab[cLine, 1] = c = new CellCreativesEditorialInformation(_session, vehicle, columns, columnsName, cells, _module);
                        break;

                    default:
                        c = new CellCreativesInformation(_session, vehicle, columns, columnsName, cells, _module);
                        if (vehicle.Id == Vehicles.names.pressClipping) c.ThumbnailsDirectory = "/"+SCAN_LOW;
                        tab[cLine, 1] = c;
                        break;
                }
            }
            else
            {
                c = (CellCreativesInformation)tab[cLine, 1];
            }
            foreach (GenericColumnItemInformation g in columns)
            {
                if (g.Id == GenericColumnItemInformation.Columns.visual || g.Id == GenericColumnItemInformation.Columns.associatedFile || g.Id == GenericColumnItemInformation.Columns.poster || g.Id == GenericColumnItemInformation.Columns.associatedFileMax)
                {
                    visuals = new List<string>();
                    visuals = GetPath(vehicle, row, columns, columnsName);
                }
            }
            c.Add(row, visuals);

        }
        #endregion

        #region CanShowInsertion
        /// <summary>
        /// True if can show insertion in Russia
        /// </summary>
        /// <param name="vehicle">vehicle</param>
        /// <returns>True if can show insertion in Russia</returns>
        public override bool CanShowInsertion(VehicleInformation vehicle)
        {
            if (vehicle == null)
                return false;
            switch (vehicle.Id)
            {
                case CstDBClassif.Vehicles.names.cinema:
                case CstDBClassif.Vehicles.names.indoor:
                    return false;
                default: return true;
            }
        }
        #endregion

        #region GetKeysName
        /// <summary>
        /// Get Data Column Names for data keys
        /// </summary>
        /// <param name="columns">List of key columns</param>
        /// <returns>List of key column names</returns>
        protected override void GetKeysColumnNames(DataTable dt, List<GenericColumnItemInformation> columns, List<string> idsColumn, List<string> labelsColumn)
        {

            string idName = string.Empty;
            string labelName = string.Empty;

            foreach (GenericColumnItemInformation g in columns)
            {
                //Init stirngs
                idName = string.Empty;
                labelName = string.Empty;

                //Data Base ID
                if (g.DataBaseAliasIdField != null && g.DataBaseAliasIdField.Length > 0)
                {
                    labelName = idName = g.DataBaseAliasIdField.ToUpper();
                }
                else if (g.DataBaseIdField != null && g.DataBaseIdField.Length > 0)
                {
                    labelName = idName = g.DataBaseIdField.ToUpper();
                }
                //Database Label
                if (g.DataBaseAliasField != null && g.DataBaseAliasField.Length > 0)
                {
                    labelName = g.DataBaseAliasField.ToUpper();
                }
                else if (g.DataBaseField != null && g.DataBaseField.Length > 0)
                {
                    labelName = g.DataBaseField.ToUpper();
                }

                if (idName.Length < 1)
                {
                    idName = labelName;
                }

                if (dt.Columns.Contains(idName) && !idsColumn.Contains(idName))
                    idsColumn.Add(idName);
                if (dt.Columns.Contains(labelName) && !labelsColumn.Contains(labelName))
                    labelsColumn.Add(labelName);
            }

        }
        #endregion

        #region GetHeaders
        /// <summary>
        /// Get Table headers
        /// </summary>
        /// <param name="vehicle">Current vehicle</param>
        /// <param name="columns">Data columns to display</param>
        /// <returns>Table headers</returns>
        protected override Headers GetHeaders(VehicleInformation vehicle, List<GenericColumnItemInformation> columns, bool hasVisual)
        {

            Headers root = new Headers();
            if (_getCreatives || _getMSCreatives)
            {
                return null;
            }
            else
            {
                if (!hasVisual)
                {
                    for (int i = 0; i < columns.Count; i++)
                    {
                        root.Root.Add(new Header(GestionWeb.GetWebWord(columns[i].WebTextId, _session.SiteLanguage), columns[i].Id.GetHashCode()));
                    }
                }
                else
                {
                    switch (vehicle.Id)
                    {

                        //case CstDBClassif.Vehicles.names.internet:                       
                        case CstDBClassif.Vehicles.names.indoor:
                            return null;
                        case Vehicles.names.radio:
                        case Vehicles.names.radioGeneral:
                        case Vehicles.names.radioSponsorship:
                        case Vehicles.names.radioMusic:
                        case Vehicles.names.tv:
                        case Vehicles.names.tvClipping:
                        case Vehicles.names.tvGeneral:
                        case Vehicles.names.tvSponsorship:
                        case Vehicles.names.tvNonTerrestrials:
                        case Vehicles.names.tvAnnounces:
                        case Vehicles.names.cinema:
                        case Vehicles.names.internet:
                        case Vehicles.names.outdoor:
                        case Vehicles.names.editorial:
                        case Vehicles.names.press:
                        case Vehicles.names.pressClipping:
                            for (int i = 0; i < columns.Count; i++)
                            {
                                root.Root.Add(new Header(GestionWeb.GetWebWord(columns[i].WebTextId, _session.SiteLanguage), columns[i].Id.GetHashCode()));
                            }
                            break;
                    }
                }
            }

            return root;

        }
        #endregion

        #region GetLineNumber
        /// <summary>
        /// Get table line numbers
        /// </summary>
        /// <param name="dt">Data Table</param>
        /// <param name="levels">Detail levels</param>
        /// <param name="keys">Data Key</param>
        /// <returns>Number of line in final table</returns>
        protected override int GetLineNumber(DataTable dt, List<DetailLevelItemInformation> levels, List<string> keys)
        {

            int nbLine = 0;
            int nbInsertionsLine = (dt != null && dt.Rows != null) ? dt.Rows.Count : 0;
            string[] oldIds = new string[levels.Count];
            string[] fieldsName = new string[levels.Count];
            for (int i = 0; i < oldIds.Length; i++) { oldIds[i] = string.Empty; }
            string[] cIds = new string[levels.Count];
            //bool isNewInsertion = false;

            for (int k = 0; k < levels.Count; k++)
            {
                //Detail levels fields to use for comparison
                if (levels[k].DataBaseAliasIdField != null && dt.Columns.Contains(levels[k].DataBaseAliasIdField))
                {
                    fieldsName[k] = levels[k].DataBaseAliasIdField;
                }
                else if (levels[k].DataBaseIdField != null && dt.Columns.Contains(levels[k].DataBaseIdField))
                {
                    fieldsName[k] = levels[k].DataBaseIdField;
                }
                else if (levels[k].DataBaseAliasField != null && dt.Columns.Contains(levels[k].DataBaseAliasField))
                {
                    fieldsName[k] = levels[k].DataBaseAliasField;
                }
                else if (levels[k].DataBaseField != null && dt.Columns.Contains(levels[k].DataBaseField))
                {
                    fieldsName[k] = levels[k].DataBaseField;
                }
            }

            foreach (DataRow row in dt.Rows)
            {

                //isNewInsertion = false;

                //Detail levels
                for (int i = 0; i < oldIds.Length; i++)
                {

                    if (row[fieldsName[i]] != null && row[fieldsName[i]] != System.DBNull.Value)
                    {
                        cIds[i] = Convert.ToString(row[fieldsName[i]]);
                    }
                    else cIds[i] = string.Empty;

                    if (cIds[i] != oldIds[i])
                    {
                        oldIds[i] = cIds[i];
                        nbLine++;
                        if (i < oldIds.Length - 1)
                        {
                            oldIds[i + 1] = string.Empty;
                        }
                    }
                }
            }

            nbLine = nbLine + nbInsertionsLine;

            return nbLine;

        }
        #endregion

        #region GetColumnsName
        /// <summary>
        /// Get Data Column Names for data to display
        /// </summary>
        /// <param name="columns">List of columns</param>
        /// <returns>List of data column names</returns>
        protected override List<string> GetColumnsName(DataTable dt, List<GenericColumnItemInformation> columns, List<Cell> cells)
        {

            List<string> names = new List<string>();
            string name = string.Empty;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(@"TNS.FrameWork.WebResultUI");
            System.Reflection.Assembly coreAssembly = null;

            foreach (GenericColumnItemInformation g in columns)
            {

                if (g.DataBaseAliasField != null && g.DataBaseAliasField.Length > 0)
                {
                    name = g.DataBaseAliasField.ToUpper();
                }
                else if (g.DataBaseField != null && g.DataBaseField.Length > 0)
                {
                    name = g.DataBaseField.ToUpper();
                }
                else if (g.DataBaseAliasIdField != null && g.DataBaseAliasIdField.Length > 0)
                {
                    name = g.DataBaseAliasIdField.ToUpper();
                }
                else if (g.DataBaseIdField != null && g.DataBaseIdField.Length > 0)
                {
                    name = g.DataBaseIdField.ToUpper();
                }

                if (dt.Columns.Contains(name) && !names.Contains(name))
                    names.Add(name);

                switch (g.CellType)
                {
                    case "":
                    case "TNS.FrameWork.WebResultUI.CellLabel":
                        cells.Add(new CellLabel(string.Empty));
                        break;
                    default:
                        Type type = null;
                        if (coreAssembly == null && g.CellType.IndexOf("TNS.AdExpress.Web.Core") >= 0)
                        {
                            coreAssembly = System.Reflection.Assembly.Load(@"TNS.AdExpress.Web.Core");
                            type = coreAssembly.GetType(g.CellType);
                        }
                        else type = assembly.GetType(g.CellType);
                        Cell cellUnit = (Cell)type.InvokeMember("GetInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, null, null);
                        cellUnit.StringFormat = string.Format("{{0:{0}}}", g.StringFormat);
                        cells.Add(cellUnit);
                        break;
                }
            }
            coreAssembly = assembly = null;


            return names;
        }
        #endregion

        #region SplitStringValue
        /// <summary>
        /// Split string value
        /// </summary>
        /// <param name="s">string</param>
        /// <returns></returns>
        protected virtual string SplitStringValue(string s)
        {
            string[] sArr = s.Split(SEPARATOR);
            string stringValue = string.Empty;
            for (int z = 0; z < sArr.Length; z++)
            {
                stringValue += sArr[z].ToString();
                if (sArr.Length > 1 && z < sArr.Length - 1) stringValue += (this.RenderType != RenderType.html) ? EXCEL_CARRIAGE_RETURN : CARRIAGE_RETURN;
            }
            s = stringValue;
            return s;
        }
        #endregion

        #region Set Fields Name
        protected virtual string GetColumnFieldName(DetailLevelItemInformation g, DataTable dt)
        {
            string name = string.Empty;

            if (g.DataBaseAliasField != null && g.DataBaseAliasField.Length > 0 && dt.Columns.Contains(g.DataBaseAliasField))
            {
                name = g.DataBaseAliasField.ToUpper();
            }
            else if (g.DataBaseField != null && g.DataBaseField.Length > 0 && dt.Columns.Contains(g.DataBaseField))
            {
                name = g.DataBaseField.ToUpper();
            }
            else if (g.DataBaseAliasIdField != null && g.DataBaseAliasIdField.Length > 0 && dt.Columns.Contains(g.DataBaseAliasIdField))
            {
                name = g.DataBaseAliasIdField.ToUpper();
            }
            else if (g.DataBaseIdField != null && g.DataBaseIdField.Length > 0 && dt.Columns.Contains(g.DataBaseIdField))
            {
                name = g.DataBaseIdField.ToUpper();
            }
            return name;
        }

        #endregion

        #region Creatives Rules

        #region GetPath
        override protected List<string> GetPath(VehicleInformation vehicle, DataRow row, List<GenericColumnItemInformation> columns, List<string> columnNames)
        {
            string path = string.Empty;
            string[] files;
            List<string> visuals = new List<string>();
            Int64 advertisementId = -1;

            if (row.Table.Columns.Contains("id_slogan") && row["id_slogan"] != System.DBNull.Value)
            {
                advertisementId = Convert.ToInt64(row["id_slogan"]);
            }
            else
                return new List<string>();

            switch (vehicle.Id)
            {
                case CstDBClassif.Vehicles.names.press:

                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRESS_CREATION_ACCESS_FLAG)) break;
                    //visuel(s) disponible(s)
                    files = row["associated_file"].ToString().Split(',');
                    for (int fileIndex = 0; fileIndex < files.Length; fileIndex++)
                    {
                        if (files[fileIndex].Length > 0)
                        {
                            visuals.Add(this.GetCreativePathVisual(CstWeb.CreationServerPathes.IMAGES, files[fileIndex], advertisementId, false, "press_low/"));
                        }
                    }
                    break;
                case CstDBClassif.Vehicles.names.pressClipping:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRESS_CLIPPING_CREATION_ACCESS_FLAG)) break;                   
                    //visuel(s) disponible(s)
                    files = row["associated_file"].ToString().Split(',');
                    for (int fileIndex = 0; fileIndex < files.Length; fileIndex++)
                    {
                        if (files[fileIndex].Length > 0)
                        {
                            visuals.Add(this.GetCreativePathVisual(CstWeb.CreationServerPathes.IMAGES_PRESS_CLIPPING, files[fileIndex], advertisementId, false, SCAN_LOW+"/"));
                        }
                    }
                    break;
                case CstDBClassif.Vehicles.names.indoor:
                case CstDBClassif.Vehicles.names.outdoor:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_OUTDOOR_CREATION_ACCESS_FLAG)) break;                   
                    if (row["associated_file"] != System.DBNull.Value)
                    {
                        files = row["associated_file"].ToString().Split(',');
                        foreach (string s in files)
                        {
                            visuals.Add(this.GetCreativePathVisual(CstWeb.CreationServerPathes.IMAGES_OUTDOOR, s, advertisementId, false, "outdoor_id_low/"));
                        }
                    }
                    break;
                case CstDBClassif.Vehicles.names.editorial:

                    if (row["associated_file"] != System.DBNull.Value)
                    {
                        files = row["associated_file"].ToString().Split(',');
                        foreach (string s in files)
                        {
                            visuals.Add(this.GetCreativePathVisual(CstWeb.CreationServerPathes.IMAGES_EDITORIAL, s, advertisementId, false, SCAN_LOW+"/"));
                        }
                    }
                    break;
                case CstDBClassif.Vehicles.names.radioGeneral:
                case CstDBClassif.Vehicles.names.radioSponsorship:
                case CstDBClassif.Vehicles.names.radioMusic:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_RADIO_CREATION_ACCESS_FLAG)) break;                  
                    if (row["associated_file"] != System.DBNull.Value)
                    {
                        files = row["associated_file"].ToString().Split(',');
                        foreach (string s in files)
                        {
                            visuals.Add(s);
                        }
                    }
                    break;
                case CstDBClassif.Vehicles.names.tvGeneral:
                case CstDBClassif.Vehicles.names.tvSponsorship:
                case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                case CstDBClassif.Vehicles.names.tvAnnounces:
                case CstDBClassif.Vehicles.names.tvClipping:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_TV_CREATION_ACCESS_FLAG)) break;                   
                    if (row["associated_file"] != DBNull.Value)
                    {
                        files = row["associated_file"].ToString().Split(',');
                        foreach (string s in files)
                        {
                            visuals.Add(s);
                        }
                    }
                    break;
                case CstDBClassif.Vehicles.names.internet:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_INTERNET_ACCESS_FLAG)) break;                    

                    if (row["associated_file"] != DBNull.Value)
                    {
                        files = row["associated_file"].ToString().Split(',');
                        foreach (string s in files)
                        {
                            visuals.Add(this.GetCreativePath(s, advertisementId));
                        }
                    }
                    break;
                default:
                    break;
            }

            return visuals;

        }
        #endregion

        #region GetCreativePathVisual
        protected string GetCreativePathVisual(string creativePath, string file, Int64 advertisementId, bool bigSize, string mediaDirectory)
        {
            string imagette = (bigSize) ? string.Empty : mediaDirectory;
            int directoryName = (int)(advertisementId / 10000) * 10000;

            return string.Format("{0}/{1}{2}/{3}", creativePath, imagette, directoryName, file);
        }
        #endregion

        #region GetCreativePath
        protected string GetCreativePath(string file, Int64 advertisementId)
        {
            int directoryName = (int)(advertisementId / 10000) * 10000;





            return string.Format("{0}/{1}/{2}", CstWeb.CreationServerPathes.CREA_ADNETTRACK, directoryName, file);
        }
        #endregion

        string GetImagesPath(CstDBClassif.Vehicles.names vehicleId)
        {
            switch (vehicleId)
            {
                case CstDBClassif.Vehicles.names.press:
                    return CstWeb.CreationServerPathes.IMAGES;
                case CstDBClassif.Vehicles.names.pressClipping:
                    return CstWeb.CreationServerPathes.IMAGES_PRESS_CLIPPING;
                case CstDBClassif.Vehicles.names.outdoor:
                    return CstWeb.CreationServerPathes.IMAGES_OUTDOOR;
                case CstDBClassif.Vehicles.names.internet:
                    return CstWeb.CreationServerPathes.CREA_ADNETTRACK;
                case CstDBClassif.Vehicles.names.editorial:
                    return CstWeb.CreationServerPathes.IMAGES_EDITORIAL;
                default:
                    throw (new Exceptions.InsertionsException("Unable to determine vehicle ID"));
            }
        }

        string GetLowDirPath(CstDBClassif.Vehicles.names vehicleId)
        {
            switch (vehicleId)
            {
                case Vehicles.names.press:
                    return "press_low/";
                case Vehicles.names.editorial:
                case Vehicles.names.pressClipping:
                    return SCAN_LOW+"/";
                case Vehicles.names.outdoor:
                    return "outdoor_id_low/";               
               default:
                    throw (new Exceptions.InsertionsException("Unable to determine vehicle ID"));
            }
        }

        #endregion
    }
}
