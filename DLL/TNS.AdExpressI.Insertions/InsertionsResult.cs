using System;
using System.Collections.Generic;
using System.Text;

using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CsCustomer = TNS.AdExpress.Constantes.Customer;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstFlags = TNS.AdExpress.Constantes.DB.Flags;
using CstVMCFormat = TNS.AdExpress.Constantes.DB.Format;

using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using System.Windows.Forms;
using TNS.AdExpressI.Insertions.Exceptions;
using TNS.AdExpressI.Insertions.DAL;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.FrameWork.WebResultUI;
using System.Data;
using System.Collections;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.Insertions.Cells;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Layers;


namespace TNS.AdExpressI.Insertions
{
    public abstract class InsertionsResult : IInsertionsResult
    {

        #region Attributes
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Current Module Id
        /// </summary>
        protected TNS.AdExpress.Domain.Web.Navigation.Module _module;
        /// <summary>
        /// Data Access Layer
        /// </summary>
        protected IInsertionsDAL _dalLayer;
        /// <summary>
        /// Get Creatives or insertions?
        /// </summary>
        protected bool _getCreatives = false;
        /// <summary>
        /// Get Creatives for media schedule and PDF export
        /// </summary>
        protected bool _getMSCreatives = false;
        /// <summary>
        /// Zoom indicator
        /// </summary>
        protected string _zoomDate = string.Empty;
        /// <summary>
        /// Univers Id parameter
        /// </summary>
        protected Int64 _universId = -1;
        /// <summary>
        /// Mutex
        /// </summary>
        protected object _mutex = new object();
        /// <summary>
        /// List of media to test for creative acces (press specific)
        /// </summary>
        protected string[] _mediaList = null;
        /// <summary>
        /// List of category to test for top diffusion rule
        /// </summary>
        protected string[] _topDiffCategory = TNS.AdExpress.Domain.Lists.GetIdList(CstWeb.GroupList.ID.category, CstWeb.GroupList.Type.digitalTv).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        #endregion

        #region RenderType
        /// <summary>
        /// Define Render Type
        /// </summary>
        protected TNS.FrameWork.WebResultUI.RenderType _renderType = RenderType.html;
        /// <summary>
        /// Get render type Session
        /// </summary>
        public TNS.FrameWork.WebResultUI.RenderType RenderType
        {
            get { return _renderType; }
            set { _renderType = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// DEfault constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="moduleId">Module ID</param>
        public InsertionsResult(WebSession session, Int64 moduleId)
        {
            _session = session;
            _module = ModulesList.GetModule(moduleId);
            object[] param = new object[2];

            param[0] = session;
            param[1] = moduleId;
            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.insertionsDAL];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the insertions DAL"));
            _dalLayer = (IInsertionsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null, null);
        }
        #endregion

        #region GetVehicles
        /// <summary>
        /// Get vehicles matching filters and which has data
        /// </summary>
        /// <param name="filters">Filters to apply (id1,id2,id3,id4</param>
        /// <returns>List of vehicles with data</returns>
        public virtual List<VehicleInformation> GetPresentVehicles(string filters, int universId, bool sloganNotNull)
        {
            List<VehicleInformation> vehicles = new List<VehicleInformation>();

            DateTime dateBegin = FctUtilities.Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            DateTime dateEnd = FctUtilities.Dates.getPeriodEndDate(_session.PeriodEndDate, _session.PeriodType);
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
                    string[] ids = filters.Split(',');
                    vehicles = GetVehicles(Convert.ToInt64(ids[0]), Convert.ToInt64(ids[1]), Convert.ToInt64(ids[2]), Convert.ToInt64(ids[3]));
                    string[] list = _session.GetSelection(_session.SelectionUniversMedia, CsCustomer.Right.type.vehicleAccess).Split(',');
                    for (int i = vehicles.Count-1; i >= 0; i--)
                    {
                        if (Array.IndexOf(list, vehicles[i].DatabaseId.ToString()) < 0){
                            vehicles.Remove(vehicles[i]);
                        }
                    }
                    break;
                case CstWeb.Module.Name.ANALYSE_DES_DISPOSITIFS:
                case CstWeb.Module.Name.ANALYSE_DES_PROGRAMMES:
                    vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.tv));
                    break;
            }

            if (vehicles.Count <= 0)
            {
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.others)) vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.others));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.directMarketing)) vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.directMarketing));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.internet)) vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.internet));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.adnettrack)) vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.evaliantMobile)) vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.evaliantMobile));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.press)) vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.press));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.newspaper)) vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.newspaper));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.magazine)) vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.magazine));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.outdoor)) vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.outdoor));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.indoor))vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.indoor));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.instore)) vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.instore));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.radio)) vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.radio));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.radioGeneral))vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.radioGeneral));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.radioSponsorship))vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.radioSponsorship));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.radioMusic))vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.radioMusic));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.tv)) vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.tv));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.tvGeneral))vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.tvGeneral));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.tvSponsorship))vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.tvSponsorship));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.tvNonTerrestrials))vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.tvNonTerrestrials));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.tvAnnounces))vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.tvAnnounces));
if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.cinema)) vehicles.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.cinema));


            }
            for (int i = vehicles.Count-1; i >= 0; i-- )
            {
                if (_module.AllowedMediaUniverse.GetVehicles() != null && !_module.AllowedMediaUniverse.GetVehicles().Contains(vehicles[i].DatabaseId))
                {
                    vehicles.Remove(vehicles[i]);
                }
            }
            for (int i = vehicles.Count-1; i >= 0; i--)
            {
                if ((_getCreatives && !vehicles[i].ShowCreations) || (!_getCreatives && !vehicles[i].ShowInsertions)){
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

        #region Liste of vehicles matching filters
        /// <summary>
        /// Get List of vehicles matching filters
        /// </summary>
        /// <param name="idLevel1">Level 1 filter</param>
        /// <param name="idLevel2">Level 2 filter</param>
        /// <param name="idLevel3">Level 3 filter</param>
        /// <param name="idLevel4">Level 4 filter</param>
        /// <returns>List of vehicles matching filters</returns>
        protected List<VehicleInformation> GetVehicles(Int64 idLevel1, Int64 idLevel2, Int64 idLevel3, Int64 idLevel4)
        {

            Dictionary<DetailLevelItemInformation, Int64> filters;
            List<VehicleInformation> vehicles = new List<VehicleInformation>();
            Int64[] vIds = null;

            try
            {
                //Get media classification filters
                filters = FctUtilities.MediaDetailLevel.GetFilters(_session, idLevel1, idLevel2, idLevel3, idLevel4);
                vIds = _dalLayer.GetVehiclesIds(filters);
                foreach (Int64 id in vIds)
                {
                    if (VehiclesInformation.Contains(id))
                    {
                        vehicles.Add(VehiclesInformation.Get(id));
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw (new InsertionsException("Unable to get media classifications level matching the filters.", ex));
            }
            return vehicles;
        }
        #endregion

        #region GetInsertions
        public virtual ResultTable GetInsertions(VehicleInformation vehicle, int fromDate, int toDate, string filters, int universId, string zoomDate)
        {
            this._getCreatives = false;
            this._zoomDate = zoomDate;
            this._universId = universId;
            return GetData(vehicle, fromDate, toDate, filters, universId);
        }
        #endregion

        #region GetCreatives
        public virtual ResultTable GetCreatives(VehicleInformation vehicle, int fromDate, int toDate, string filters, int universId, string zoomDate)
        {
            this._getCreatives = true;
            this._zoomDate = zoomDate;
            this._universId = universId;
            return GetData(vehicle, fromDate, toDate, filters, universId);
        }
        #endregion

        #region GetMSCreatives
        public virtual ResultTable GetMSCreatives(VehicleInformation vehicle, int fromDate, int toDate, string filters, int universId, string zoomDate) {
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
        public virtual string GetCreativeLinks(long idVehicle, DataRow currentRow)
        {
            string vignettes = "";
            string sloganDetail = "";
            string imagesList = "";
            string[] fileList = null;
            string dateField = "date_media_num";
            string pathWeb = "";
            bool first = true;
            string themeName = WebApplicationParameters.Themes[_session.SiteLanguage].Name;
            if (_session.CustomerLogin.ShowCreatives(VehiclesInformation.DatabaseIdToEnum(idVehicle))) {//droit créations
                if (currentRow["associated_file"] != DBNull.Value && currentRow["associated_file"].ToString().Length > 0)
                {

                    switch (VehiclesInformation.DatabaseIdToEnum(idVehicle))
                    {

                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press:
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.newspaper:
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.magazine:
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress:

                            #region Construction de la liste des images presse
                            fileList = currentRow["associated_file"].ToString().Split(',');

                            if (_session.CurrentModule == CstWeb.Module.Name.BILAN_CAMPAGNE) dateField = "date_cover_num";

                            string pathWebImagette = CstWeb.CreationServerPathes.IMAGES + "/" + currentRow["id_media"].ToString() + "/" + currentRow[dateField].ToString() + "/imagette/";
                            pathWeb = CstWeb.CreationServerPathes.IMAGES + "/" + currentRow["id_media"].ToString() + "/" + currentRow[dateField].ToString() + "/";
                            string pathImagette = CstWeb.CreationServerPathes.LOCAL_PATH_IMAGE + currentRow["id_media"].ToString() + @"\" + currentRow[dateField].ToString() + @"\imagette\";

                            if (fileList != null)
                            {
                                for (int j = 0; j < fileList.Length; j++)
                                {

                                    vignettes += "<img src='" + pathWebImagette + fileList[j] + "' border=\"0\" width=\"50\" height=\"64\" >";
                                    if (first) imagesList = pathWeb + fileList[j];
                                    else { imagesList += "," + pathWeb + fileList[j]; }
                                    first = false;
                                }

                                if (vignettes.Length > 0)
                                {
                                    vignettes = "<a href=\"javascript:openPressCreation('" + imagesList.Replace("/Imagette", "") + "');\">" + vignettes + "</a>";
                                    vignettes += "\n<br>";
                                }

                            }
                            else vignettes = GestionWeb.GetWebWord(843, _session.SiteLanguage) + "<br>";
                            #endregion

                            break;

                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioGeneral:
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioSponsorship:
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioMusic:
                            if (currentRow["associated_file"] != null && currentRow["associated_file"].ToString().CompareTo("") != 0)
                                vignettes = "<a href=\"javascript:openDownload('" + currentRow["associated_file"].ToString() + "','" + _session.IdSession + "','" + idVehicle + "');\"><img border=\"0\" src=\"/App_Themes/" + themeName + "/Images/Common/Picto_Radio.gif\"></a>";
                            break;

                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvGeneral:
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvSponsorship:
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvAnnounces:
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvNonTerrestrials:
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.others:
                            if (currentRow["associated_file"] != null && currentRow["associated_file"].ToString().CompareTo("") != 0)
                                vignettes = "<a href=\"javascript:openDownload('" + currentRow["associated_file"].ToString() + "','" + _session.IdSession + "','" + idVehicle + "');\"><img border=\"0\" src=\"/App_Themes/" + themeName + "/Images/Common/Picto_pellicule.gif\"></a>";
                            break;

                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing:
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.outdoor:
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.indoor:
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.instore:

                            #region Construction de la liste des images du marketing direct ou de la publicité extérieure
                            fileList = currentRow["associated_file"].ToString().Split(',');
                            string idAssociatedFile = currentRow["associated_file"].ToString();

                            if (VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())) == CstDBClassif.Vehicles.names.directMarketing)
                                pathWeb = CstWeb.CreationServerPathes.IMAGES_MD;
                            else pathWeb = CstWeb.CreationServerPathes.IMAGES_OUTDOOR;
                            string dir1 = idAssociatedFile.Substring(idAssociatedFile.Length - 8, 1);
                            pathWeb = string.Format(@"{0}/{1}", pathWeb, dir1);
                            string dir2 = idAssociatedFile.Substring(idAssociatedFile.Length - 9, 1);
                            pathWeb = string.Format(@"{0}/{1}", pathWeb, dir2);
                            string dir3 = idAssociatedFile.Substring(idAssociatedFile.Length - 10, 1);
                            pathWeb = string.Format(@"{0}/{1}/imagette/", pathWeb, dir3);

                            if (fileList != null)
                            {
                                for (int j = 0; j < fileList.Length; j++)
                                {
                                    vignettes += "<img src='" + pathWeb + fileList[j] + "' border=\"0\" width=\"50\" height=\"64\" >";
                                    if (first) imagesList = pathWeb + fileList[j];
                                    else { imagesList += "," + pathWeb + fileList[j]; }
                                    first = false;
                                }

                                if (vignettes.Length > 0)
                                {
                                    vignettes = "<a href=\"javascript:openPressCreation('" + imagesList.Replace("/Imagette", "") + "');\">" + vignettes + "</a>";
                                    vignettes += "\n<br>";
                                }

                            }
                            else vignettes = GestionWeb.GetWebWord(843, _session.SiteLanguage) + "<br>";
                            #endregion

                            break;
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.adnettrack:
                            if (currentRow["associated_file"] != null && currentRow["associated_file"].ToString().CompareTo("") != 0)
                                vignettes = string.Format("<a href=\"javascript:openEvaliantCreative('{1}/{0}', '{3}');\"><img border=\"0\" src=\"/App_Themes/{2}/Images/Common/Button/adnettrack.gif\"></a>", currentRow["associated_file"].ToString().Replace(@"\", "/"), CstWeb.CreationServerPathes.CREA_ADNETTRACK, themeName, currentRow["advertDimension"]);
                            break;
                        case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile:
                            if (currentRow["associated_file"] != null && currentRow["associated_file"].ToString().CompareTo("") != 0)
                                vignettes = string.Format("<a href=\"javascript:openEvaliantCreative('{1}/{0}', '{3}');\"><img border=\"0\" src=\"/App_Themes/{2}/Images/Common/Button/adnettrack.gif\"></a>", currentRow["associated_file"].ToString().Replace(@"\", "/"), CstWeb.CreationServerPathes.CREA_EVALIANT_MOBILE, themeName, currentRow["advertDimension"]);
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
            sloganDetail += currentRow["id_slogan"].ToString();
            if (currentRow["advertDimension"] != null && currentRow["advertDimension"] != System.DBNull.Value)
            {
                if (VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())) != Vehicles.names.directMarketing
                    || (VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString())) == Vehicles.names.directMarketing && _session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_POIDS_MARKETING_DIRECT)))
                    sloganDetail += " - " + currentRow["advertDimension"].ToString();
            }
            sloganDetail += "\n</td></tr>";
            sloganDetail += "\n</table>";

            return sloganDetail;

        }
        #endregion

        #region GetData
        protected virtual ResultTable GetData(VehicleInformation vehicle, int fromDate, int toDate, string filters, int universId)
        {
            ResultTable data = null;

            #region Data Access
            if (vehicle == null)
                return null;

            DataSet ds = null;

            if (_getMSCreatives) {
                ds = _dalLayer.GetMSCreativesData(vehicle, fromDate, toDate, universId, filters);
            }
            else if (_getCreatives){
                ds = _dalLayer.GetCreativesData(vehicle, fromDate, toDate, universId, filters);
            }
            else{
                ds = _dalLayer.GetInsertionsData(vehicle, fromDate, toDate, universId, filters);
            }

            if (ds == null || ds.Equals(DBNull.Value) || ds.Tables[0] == null || ds.Tables[0].Rows.Count ==0)
                return null;
            #endregion

            DataTable dt = ds.Tables[0];

            #region Init ResultTable
            List<DetailLevelItemInformation> levels = new List<DetailLevelItemInformation>();
            if (!_getMSCreatives) {
                foreach (DetailLevelItemInformation d in _session.DetailLevel.Levels) {
                    levels.Add((DetailLevelItemInformation)d);
                }
            }
            List<GenericColumnItemInformation> columns ;

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
                case CstDBClassif.Vehicles.names.adnettrack:
                case CstDBClassif.Vehicles.names.internet:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DETAIL_INTERNET_ACCESS_FLAG);
                    break;
                case CstDBClassif.Vehicles.names.evaliantMobile:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DETAIL_EVALIANT_MOBILE_ACCESS_FLAG);
                    break;
                case CstDBClassif.Vehicles.names.directMarketing:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DIRECT_MARKETING_CREATION_ACCESS_FLAG);
                    break;
                case CstDBClassif.Vehicles.names.internationalPress:
                case CstDBClassif.Vehicles.names.newspaper:
                case CstDBClassif.Vehicles.names.magazine:
                case CstDBClassif.Vehicles.names.press:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_PRESS_CREATION_ACCESS_FLAG);
                    break;
                case CstDBClassif.Vehicles.names.others:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_OTHERS_CREATION_ACCESS_FLAG);
                    break;
                case CstDBClassif.Vehicles.names.indoor:
                case CstDBClassif.Vehicles.names.outdoor:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_OUTDOOR_CREATION_ACCESS_FLAG);
                    break;
                case CstDBClassif.Vehicles.names.instore:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_INSTORE_CREATION_ACCESS_FLAG);
                    break;
                case CstDBClassif.Vehicles.names.radio:
                case CstDBClassif.Vehicles.names.radioGeneral:
                case CstDBClassif.Vehicles.names.radioSponsorship:
                case CstDBClassif.Vehicles.names.radioMusic:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_RADIO_CREATION_ACCESS_FLAG);
                    break;
                case CstDBClassif.Vehicles.names.tv:
                case CstDBClassif.Vehicles.names.tvGeneral:
                case CstDBClassif.Vehicles.names.tvSponsorship:
                case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                case CstDBClassif.Vehicles.names.tvAnnounces:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_TV_CREATION_ACCESS_FLAG);
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
                if (c.Id == GenericColumnItemInformation.Columns.content)
                {
                        //Data Base ID
                        if (c.DataBaseAliasIdField != null && c.DataBaseAliasIdField.Length > 0)
                        {
                            divideCol = c.DataBaseAliasIdField.ToUpper();
                        }
                        else if (c.DataBaseIdField != null && c.DataBaseIdField.Length > 0)
                        {
                            divideCol = c.DataBaseIdField.ToUpper();
                        }
                        //Database Label
                        if (c.DataBaseAliasField != null && c.DataBaseAliasField.Length > 0)
                        {
                            divideCol = c.DataBaseAliasField.ToUpper();
                        }
                        else if (c.DataBaseField != null && c.DataBaseField.Length > 0)
                        {
                            divideCol = c.DataBaseField.ToUpper();
                        }
                        break;
                }
            }

            Int64 idColumnsSet = -1;
            if (this._getMSCreatives) 
                idColumnsSet = WebApplicationParameters.MsCreativesDetail.GetDetailColumnsId(vehicle.DatabaseId);
            else if (this._getCreatives){
                idColumnsSet = WebApplicationParameters.CreativesDetail.GetDetailColumnsId(vehicle.DatabaseId, _module.Id);
            }
            else{
                idColumnsSet = WebApplicationParameters.InsertionsDetail.GetDetailColumnsId(vehicle.DatabaseId, _module.Id);
            }
            
            //Data Keys
            List<GenericColumnItemInformation> keys = WebApplicationParameters.GenericColumnsInformation.GetKeys(idColumnsSet);
            List<string> keyIdName = new List<string>();
            List<string> keyLabelName = new List<string>();
            GetKeysColumnNames(dt, keys, keyIdName, keyLabelName);
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

            SetLine setLine = null;
            SetSpecificLine setSpecificLine = null;
            if (_getMSCreatives) {
                setSpecificLine = new SetSpecificLine(SetMSCreativeLine);
            }
            if (_getCreatives){
                setLine = new SetLine(SetCreativeLine);
            }
            else{
                if (!hasVisuals)
                {
                    setLine = new SetLine(SetRawLine);
                }
                else
                {
                    switch (vehicle.Id)
                    {
                        case CstDBClassif.Vehicles.names.directMarketing:
                        case CstDBClassif.Vehicles.names.internationalPress:
                        case CstDBClassif.Vehicles.names.press:
                        case CstDBClassif.Vehicles.names.newspaper:
                        case CstDBClassif.Vehicles.names.magazine:
                        case CstDBClassif.Vehicles.names.outdoor:
                        case CstDBClassif.Vehicles.names.instore:
                        case CstDBClassif.Vehicles.names.indoor:
                            setLine = new SetLine(SetAggregLine);
                            break;
                        default:
                            setLine = new SetLine(SetRawLine);
                            break;
                    }
                }
            }
            #endregion

            #region Table fill
            LineType[] lineTypes = new LineType[4]{LineType.level1, LineType.level2, LineType.level3, LineType.level4};
            Dictionary<string, Int64> levelKeyValues = null;

            Int64[] oldIds = new Int64[levels.Count];
            for (int i = 0; i < oldIds.Length; i++) { oldIds[i] = -1; }
            Int64[] cIds = new Int64[levels.Count];

            Int64[] oldKeyIds = new Int64[keys.Count];
            for (int i = 0; i < oldKeyIds.Length; i++) { oldKeyIds[i] = -1; }
            Int64[] cKeyIds = new Int64[keys.Count];

            string label = string.Empty;
            int cLine = 0;
            bool isNewInsertion = false;
            string key = string.Empty;

            foreach (DataRow row in dt.Rows) {

                isNewInsertion = false;
                //Detail levels
                for (int i = 0; i < oldIds.Length; i++) {
                    cIds[i] = Convert.ToInt64(row[levels[i].DataBaseIdField]);
                    if (cIds[i] != oldIds[i]) {
                        oldIds[i] = cIds[i];                        
                        if (i < oldIds.Length - 1) {
                            oldIds[i + 1] = -1;
                            for (int j = 0; j < oldKeyIds.Length; j++) { oldKeyIds[j] = -1; }
                        }

                        //Set current level
                        cLine = data.AddNewLine(lineTypes[i]);
                        switch (levels[i].Id)
                        {
                            case DetailLevelItemInformation.Levels.date:
                                data[cLine, 1] = new CellDate(Dates.getPeriodBeginningDate(row[levels[i].DataBaseField].ToString(), CstWeb.CustomerSessions.Period.Type.dateToDate), "{0:shortdatepattern}");
                                break;
                            case DetailLevelItemInformation.Levels.duration:
                                data[cLine, 1] = new CellDuration(Convert.ToDouble(row[levels[i].DataBaseField]));
                                ((CellUnit)data[cLine, 1]).StringFormat = string.Format("{{0:{0}}}", WebApplicationParameters.GenericColumnItemsInformation.Get(GenericColumnItemInformation.Columns.duration.GetHashCode()).StringFormat);
                                break;
                            default:
                                data[cLine, 1] = new CellLabel(row[levels[i].DataBaseField].ToString());
                                break;

                        }

                        for (int j = 2; j <= data.DataColumnsNumber; j++) {
                            data[cLine, j] = new CellEmpty();
                        }

                    }
                }

                //Insertion Keys
                key = string.Empty;
                for (int i = 0; i < oldKeyIds.Length; i++) {
                    cKeyIds[i] = Convert.ToInt64(row[keyIdName[i]]);                    
                    if (cKeyIds[i] != oldKeyIds[i]) {
                        oldKeyIds[i] = cKeyIds[i];
                        if (i < oldKeyIds.Length - 1) {
                            oldKeyIds[i + 1] = -1;
                        }
                        isNewInsertion = true;
                    }
                }

                if (isNewInsertion)
                {
                    cLine = data.AddNewLine(lineTypes[3]);
                }

                if(_getMSCreatives)
                    setSpecificLine(vehicle, data, row, cLine, columns, columnsName, cells, idColumnsSet);
                else
                    setLine(vehicle, data, row, cLine, columns, columnsName, cells, divideCol);

            }

            #endregion

            return data;
        }

        protected delegate void SetLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine, List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, string divideCol);
        protected virtual void SetRawLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine, List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, string divideCol)
        {
            int i = -1;
            int j = 0;
            foreach (GenericColumnItemInformation g in columns) {

                i++;
                j++;
                if (cells[i] is CellUnit) {
                    int div = 1;
                    if (g.IsSum)
                    {
                        div = Math.Max(div, row[divideCol].ToString().Split(',').Length);
                    }
                    Double val = 0;
                    if (row[columnsName[i]] != System.DBNull.Value)
                    {
                        val = Convert.ToDouble(row[columnsName[i]]) / div;
                    }
                    switch(columns[i].Id){
                        case GenericColumnItemInformation.Columns.weight:
                            if (!_session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_POIDS_MARKETING_DIRECT))
                            {
                                val = 0;
                            }
                            break;
                        case GenericColumnItemInformation.Columns.volume:
                            if (!_session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_VOLUME_MARKETING_DIRECT))
                            {
                                val = 0;
                            }
                            break;
                        case GenericColumnItemInformation.Columns.topDiffusion:
                            if(vehicle.Id == CstDBClassif.Vehicles.names.tv
                                || vehicle.Id == CstDBClassif.Vehicles.names.tvGeneral
                                || vehicle.Id == CstDBClassif.Vehicles.names.tvAnnounces
                                || vehicle.Id == CstDBClassif.Vehicles.names.tvSponsorship
                                || vehicle.Id == CstDBClassif.Vehicles.names.tvNonTerrestrials)
                            {
								string idCat = "";
								try {
									idCat = row[WebApplicationParameters.GenericColumnItemsInformation.Get((long)GenericColumnItemInformation.Columns.idCategory).DataBaseField].ToString();
								}
								catch (Exception e) {
									idCat = "";
								}
                                if (Array.IndexOf(_topDiffCategory, idCat) >= 0){
                                    val = 0;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    if (tab[cLine, j] == null){
                        tab[cLine, j] = ((CellUnit)cells[i]).Clone(val);
                    }
                    else{
                        ((CellUnit)tab[cLine, j]).Add(val);
                    }
                }
                else {
                    string s = string.Empty;
                    switch (columns[i].Id)
                    {
                        case GenericColumnItemInformation.Columns.associatedFile:
                            switch (vehicle.Id){
                                case CstDBClassif.Vehicles.names.others:
                                case CstDBClassif.Vehicles.names.tv:
                                case CstDBClassif.Vehicles.names.tvGeneral:
                                case CstDBClassif.Vehicles.names.tvSponsorship:
                                case CstDBClassif.Vehicles.names.tvAnnounces:
                                case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                                    s = row[columnsName[i]].ToString();
                                    if(s.Length > 0)
                                        tab[cLine, j] = new CellTvCreativeLink(s, _session,vehicle.DatabaseId);
                                    else
                                        tab[cLine, j] = new CellTvCreativeLink(string.Empty, _session, vehicle.DatabaseId);
                                    break;
                                case CstDBClassif.Vehicles.names.radio:
                                    tab[cLine, j] = new CellRadioCreativeLink(row[columnsName[i]].ToString(), _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radio));
                                    break;
                                case CstDBClassif.Vehicles.names.radioGeneral:
                                    tab[cLine, j] = new CellRadioCreativeLink(row[columnsName[i]].ToString(), _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioGeneral));
                                    break;
                                case CstDBClassif.Vehicles.names.radioSponsorship:
                                    tab[cLine, j] = new CellRadioCreativeLink(row[columnsName[i]].ToString(), _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioSponsorship));
                                    break;
                                case CstDBClassif.Vehicles.names.radioMusic:
                                    tab[cLine, j] = new CellRadioCreativeLink(row[columnsName[i]].ToString(), _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioMusic));
                                    break;
                            }
                            break;
                        case GenericColumnItemInformation.Columns.dayOfWeek:
                            Int32 n = Convert.ToInt32(row[columnsName[i]]);
                            int y = n/10000;
                            int m = (n-(10000*y))/100;
                            int d = n-(10000*y + 100*m);                            
                            tab[cLine, j] = new CellDate(new DateTime(y, m, d), string.Format("{{0:{0}}}", g.StringFormat));
                            break;
                        case GenericColumnItemInformation.Columns.mailFormat:
                            string cValue = row[columnsName[i]].ToString();
                            if (cValue != CstVMCFormat.FORMAT_ORIGINAL)
                            {
                                cValue = GestionWeb.GetWebWord(2240, _session.SiteLanguage);
                            }
                            else
                            {
                                cValue = GestionWeb.GetWebWord(2241, _session.SiteLanguage);
                            }
                            if (tab[cLine, j] == null)
                            {
                                tab[cLine, j] = new CellLabel(cValue);
                            }
                            else
                            {
                                ((CellLabel)tab[cLine, j]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, j]).Label, cValue);
                            }
                            break;
                        case GenericColumnItemInformation.Columns.product:
                            s = row[columnsName[i]].ToString();
                            if (!_session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                            {
                                s = string.Empty;
                            }
                            if (tab[cLine, j] == null || ((CellLabel)tab[cLine, j]).Label.Length <= 0)
                            {
                                tab[cLine, j] = new CellLabel(s);
                            }
                            else
                            {
                                ((CellLabel)tab[cLine, j]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, j]).Label, s);
                            }
                            break;
                        case GenericColumnItemInformation.Columns.slogan:
                            s = row[columnsName[i]].ToString();
                            if (!_session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_SLOGAN_ACCESS_FLAG))
                            {
                                s = string.Empty;
                            }
                            if (tab[cLine, j] == null || ((CellLabel)tab[cLine, j]).Label.Length <= 0)
                            {
                                tab[cLine, j] = new CellLabel(s);
                            }
                            else
                            {
                                ((CellLabel)tab[cLine, j]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, j]).Label, s);
                            }
                            break;
                        default:
                            if (tab[cLine, j] == null)
                            {
                                tab[cLine, j] = new CellLabel(row[columnsName[i]].ToString());
                            }
                            else
                            {
                                ((CellLabel)tab[cLine, j]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, j]).Label, row[columnsName[i]].ToString());
                            }
                            break;
                    }
                }

            }
        }
        protected virtual void SetAggregLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine, List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, string divideCol)
        {

            CellInsertionInformation c;
            List<string> visuals = new List<string>();
            if (tab[cLine, 1] == null) {
                switch (vehicle.Id)
                {
                    case CstDBClassif.Vehicles.names.directMarketing:
                        tab[cLine, 1] = c = new CellInsertionVMCInformation(_session, columns, columnsName, cells);
                        break;
                    default:
                        tab[cLine, 1] = c = new CellInsertionInformation(_session, columns, columnsName, cells);
                        break;
                }
            }
            else {
                c = (CellInsertionInformation)tab[cLine, 1];
            }
            foreach (GenericColumnItemInformation g in columns)
            {
                if (g.Id == GenericColumnItemInformation.Columns.visual || g.Id == GenericColumnItemInformation.Columns.associatedFile || g.Id == GenericColumnItemInformation.Columns.poster)
                {
                    visuals = GetPath(vehicle, row, columns, columnsName);
                }
            }
            c.Add(row, visuals);

        }
        protected virtual void SetCreativeLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine, List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, string divideCol)
        {

            CellCreativesInformation c;
            List<string> visuals = new List<string>();
            if (tab[cLine, 1] == null)
            {
                switch (vehicle.Id)
                {
                    case CstDBClassif.Vehicles.names.directMarketing:
                        tab[cLine, 1] = c = new CellCreativesVMCInformation(_session, vehicle, columns, columnsName, cells, _module);
                        break;
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
                    case CstDBClassif.Vehicles.names.adnettrack:
                    case CstDBClassif.Vehicles.names.internet:
                        tab[cLine, 1] = c = new CellCreativesEvaliantInformation(_session, vehicle, columns, columnsName, cells, _module, _zoomDate, _universId);
                        break;
                    case CstDBClassif.Vehicles.names.evaliantMobile:
                        tab[cLine, 1] = c = new CellCreativesEvaliantMobileInformation(_session, vehicle, columns, columnsName, cells, _module, _zoomDate, _universId);
                        break;
                    default:
                        tab[cLine, 1] = c = new CellCreativesInformation(_session, vehicle, columns, columnsName, cells, _module);
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
                    visuals = GetPath(vehicle, row, columns, columnsName);
                }
            }
            c.Add(row, visuals);

        }

        protected delegate void SetSpecificLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine, List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, Int64 idColumnsSet);
        protected virtual void SetMSCreativeLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine, List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, Int64 idColumnsSet)
        {

            CellCreativesInformation c;
            List<string> visuals = new List<string>();
            if (tab[cLine, 1] == null) {
                switch (vehicle.Id) {
                    case CstDBClassif.Vehicles.names.directMarketing:
                        tab[cLine, 1] = c = new CellCreativesVMCInformation(_session, vehicle, columns, columnsName, cells, _module, idColumnsSet);
                        break;
                    case CstDBClassif.Vehicles.names.radio:
                    case CstDBClassif.Vehicles.names.radioSponsorship:
                    case CstDBClassif.Vehicles.names.radioGeneral:
                    case CstDBClassif.Vehicles.names.radioMusic:
                        tab[cLine, 1] = c = new CellCreativesRadioInformation(_session, vehicle, columns, columnsName, cells, _module, idColumnsSet);
                        break;
                    case CstDBClassif.Vehicles.names.tv:
                    case CstDBClassif.Vehicles.names.tvGeneral:
                    case CstDBClassif.Vehicles.names.tvSponsorship:
                    case CstDBClassif.Vehicles.names.tvAnnounces:
                    case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                    case CstDBClassif.Vehicles.names.others:
                        tab[cLine, 1] = c = new CellCreativesTvInformation(_session, vehicle, columns, columnsName, cells, _module, idColumnsSet);
                        break;
                    case CstDBClassif.Vehicles.names.adnettrack:
                    case CstDBClassif.Vehicles.names.internet:
                        tab[cLine, 1] = c = new CellCreativesEvaliantInformation(_session, vehicle, columns, columnsName, cells, _module, _zoomDate, _universId, idColumnsSet);
                        break;
                    case CstDBClassif.Vehicles.names.evaliantMobile:
                        tab[cLine, 1] = c = new CellCreativesEvaliantMobileInformation(_session, vehicle, columns, columnsName, cells, _module, _zoomDate, _universId, idColumnsSet);
                        break;
                    default:
                        tab[cLine, 1] = c = new CellCreativesInformation(_session, vehicle, columns, columnsName, cells, _module, idColumnsSet);
                        break;
                }
            }
            else {
                c = (CellCreativesInformation)tab[cLine, 1];
            }
            foreach (GenericColumnItemInformation g in columns) {
                if (g.Id == GenericColumnItemInformation.Columns.visual || g.Id == GenericColumnItemInformation.Columns.associatedFile || g.Id == GenericColumnItemInformation.Columns.poster || g.Id == GenericColumnItemInformation.Columns.associatedFileMax) {
                    visuals = GetPath(vehicle, row, columns, columnsName);
                }
            }
            c.Add(row, visuals);

        }

        #region GetLineNumber
        /// <summary>
        /// Get table line numbers
        /// </summary>
        /// <param name="dt">Data Table</param>
        /// <param name="levels">Detail levels</param>
        /// <param name="keys">Data Key</param>
        /// <returns>Number of line in final table</returns>
        protected virtual int GetLineNumber(DataTable dt, List<DetailLevelItemInformation> levels, List<string> keys)
        {

            int nbLine = 0;

            Int64[] oldIds = new Int64[levels.Count];
            for (int i = 0; i < oldIds.Length; i++) { oldIds[i] = -1; }
            Int64[] cIds = new Int64[levels.Count];

            Int64[] oldKeyIds = new Int64[keys.Count];
            for (int i = 0; i < oldKeyIds.Length; i++) { oldKeyIds[i] = -1; }
            Int64[] cKeyIds = new Int64[keys.Count];
            bool isNewInsertion = false;
            foreach (DataRow row in dt.Rows) {
                isNewInsertion = false;
                //Detail levels
                for (int i = 0; i < oldIds.Length; i++) {
                    cIds[i] = Convert.ToInt64(row[levels[i].DataBaseIdField]);
                    if (cIds[i] != oldIds[i]) {
                        oldIds[i] = cIds[i];
                        nbLine++;
                        if (i < oldIds.Length - 1) {
                            oldIds[i + 1] = -1;
                            for (int j = 0; j < oldKeyIds.Length; j++) { oldKeyIds[j] = -1; }
                        }
                    }
                }

                //Insertion Keys
                for (int i = 0; i < oldKeyIds.Length; i++) {
                    cKeyIds[i] = Convert.ToInt64(row[keys[i]]);
                    if (cKeyIds[i] != oldKeyIds[i]) {
                        oldKeyIds[i] = cKeyIds[i];
                        isNewInsertion = true;
                        if (i < oldKeyIds.Length - 1) {
                            oldKeyIds[i + 1] = -1;
                        }
                    }
                }

                if (isNewInsertion)
                {
                    nbLine++;
                }


            }

            return nbLine;

        }
        #endregion

        #region GetColumnsName
        /// <summary>
        /// Get Data Column Names for data to display
        /// </summary>
        /// <param name="columns">List of columns</param>
        /// <returns>List of data column names</returns>
        protected virtual List<string> GetColumnsName(DataTable dt, List<GenericColumnItemInformation> columns, List<Cell> cells) {

            List<string> names = new List<string>();
            string name = string.Empty;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(@"TNS.FrameWork.WebResultUI");

            foreach (GenericColumnItemInformation g in columns) {

                if (g.DataBaseAliasField != null && g.DataBaseAliasField.Length > 0) {
                    name = g.DataBaseAliasField.ToUpper();
                }
                else if (g.DataBaseField != null && g.DataBaseField.Length > 0) {
                    name = g.DataBaseField.ToUpper();
                }

                if (dt.Columns.Contains(name) && !names.Contains(name))
                    names.Add(name);

                switch (g.CellType) {
                    case "":
                    case "TNS.FrameWork.WebResultUI.CellLabel":
                        cells.Add(new CellLabel(string.Empty));
                        break;
                    default:
                        
                        Type type = assembly.GetType(g.CellType);
                        Cell cellUnit = (Cell)type.InvokeMember("GetInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, null, null);
                        cellUnit.StringFormat = string.Format("{{0:{0}}}",g.StringFormat);
                        cells.Add(cellUnit);
                        break;
                }
            }

            return names;
        }
        #endregion

        #region GetKeysName
        /// <summary>
        /// Get Data Column Names for data keys
        /// </summary>
        /// <param name="columns">List of key columns</param>
        /// <returns>List of key column names</returns>
        protected virtual void GetKeysColumnNames(DataTable dt, List<GenericColumnItemInformation> columns, List<string> idsColumn, List<string> labelsColumn) {

            string idName = string.Empty;
            string labelName = string.Empty;

            foreach (GenericColumnItemInformation g in columns) {
                //Init stirngs
                idName = string.Empty;
                labelName = string.Empty;

                //Data Base ID
                if (g.DataBaseAliasIdField != null && g.DataBaseAliasIdField.Length > 0) {
                    labelName = idName = g.DataBaseAliasIdField.ToUpper();
                }
                else if (g.DataBaseIdField != null && g.DataBaseIdField.Length > 0) {
                    labelName = idName = g.DataBaseIdField.ToUpper();
                }
                //Database Label
                if (g.DataBaseAliasField != null && g.DataBaseAliasField.Length > 0 ) {
                    labelName = g.DataBaseAliasField.ToUpper();
                }
                else if (g.DataBaseField != null && g.DataBaseField.Length > 0) {
                    labelName = g.DataBaseField.ToUpper();
                }

                if (idName.Length < 1) {
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
        protected virtual Headers GetHeaders(VehicleInformation vehicle, List<GenericColumnItemInformation> columns, bool hasVisual) {

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
                        case CstDBClassif.Vehicles.names.directMarketing:
                        case CstDBClassif.Vehicles.names.adnettrack:
                        case CstDBClassif.Vehicles.names.evaliantMobile:
                        case CstDBClassif.Vehicles.names.internet:
                        case CstDBClassif.Vehicles.names.internationalPress:
                        case CstDBClassif.Vehicles.names.outdoor:
                        case CstDBClassif.Vehicles.names.instore:
                        case CstDBClassif.Vehicles.names.indoor :
                        case CstDBClassif.Vehicles.names.press:
                        case CstDBClassif.Vehicles.names.newspaper:
                        case CstDBClassif.Vehicles.names.magazine:
                            return null;
                            break;
                        case CstDBClassif.Vehicles.names.others:
                        case CstDBClassif.Vehicles.names.radio:
                        case CstDBClassif.Vehicles.names.radioGeneral:
                        case CstDBClassif.Vehicles.names.radioSponsorship:
                        case CstDBClassif.Vehicles.names.radioMusic:
                        case CstDBClassif.Vehicles.names.tv:
                        case CstDBClassif.Vehicles.names.tvGeneral:
                        case CstDBClassif.Vehicles.names.tvSponsorship:
                        case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                        case CstDBClassif.Vehicles.names.tvAnnounces:
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

        #region Init cells
        protected List<Cell> GetCells(List<GenericColumnItemInformation> columns) {

            List<Cell> cells = new List<Cell>();
            Cell cell = null;
            int i = -1;

            foreach (GenericColumnItemInformation g in columns) {

                i++;
                try {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(@"TNS.FrameWork.WebResultUI");
                    Type type = assembly.GetType(g.CellType);
                    cell = (Cell)type.InvokeMember("GetInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, null, null);
                    cell.StringFormat = g.StringFormat;
                }
                catch (Exception e) {
                    cells[i] = new CellLabel(string.Empty);
                }

            }

            return cells;
        }
        #endregion

        #endregion

        #region CanShowInsertion
        /// <summary>
        /// True if can show insertion 
        /// </summary>
        /// <param name="vehicle">vehicle</param>
        /// <returns>True if can show insertion </returns>
        public virtual bool CanShowInsertion(VehicleInformation vehicle)
        {            
            if (vehicle == null || vehicle.Id == CstDBClassif.Vehicles.names.internet)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Creatives Rules

        #region GetPath
        virtual protected List<string> GetPath(VehicleInformation vehicle, DataRow row, List<GenericColumnItemInformation> columns, List<string> columnNames)
        {
            string path = string.Empty;
            List<string> visuals = new List<string>();

            switch (vehicle.Id)
            {
                case CstDBClassif.Vehicles.names.press:
                case CstDBClassif.Vehicles.names.newspaper:
                case CstDBClassif.Vehicles.names.magazine:
                case CstDBClassif.Vehicles.names.internationalPress:

                    if ((vehicle.Id==CstDBClassif.Vehicles.names.internationalPress && !_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_INTERNATIONAL_PRESS_CREATION_ACCESS_FLAG))
                        || (vehicle.Id==CstDBClassif.Vehicles.names.press && !_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRESS_CREATION_ACCESS_FLAG))
                        || (vehicle.Id == CstDBClassif.Vehicles.names.newspaper && !_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRESS_CREATION_ACCESS_FLAG))
                        || (vehicle.Id == CstDBClassif.Vehicles.names.magazine && !_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRESS_CREATION_ACCESS_FLAG))
                        )
                    {
                        break;
                    }

                    Int64 disponibility = -1;
                    Int64 activation = -1;
                    Int64 idMedia = -1;
                    Int64 dateCoverNum = -1;
                    Int64 dateMediaNum = -1;

                    if (row.Table.Columns.Contains("disponibility_visual") && row["disponibility_visual"] != System.DBNull.Value){
                        disponibility = Convert.ToInt64(row["disponibility_visual"]);
                    }
                    if (row.Table.Columns.Contains("activation") && row["activation"] != System.DBNull.Value){
                        activation = Convert.ToInt64(row["activation"]);
                    }
                    if (row.Table.Columns.Contains("id_media") && row["id_media"] != System.DBNull.Value){
                        idMedia = Convert.ToInt64(row["id_media"]);
                    }
                    if (row.Table.Columns.Contains("date_cover_num") && row["date_cover_num"] != System.DBNull.Value){
                        dateCoverNum = Convert.ToInt64(row["date_cover_num"]);
                    }
                    if (row.Table.Columns.Contains("date_media_num") && row["date_media_num"] != System.DBNull.Value){
                        dateMediaNum = Convert.ToInt64(row["date_media_num"]);
                    }
                    if (row.Table.Columns.Contains("dateKiosque") && row["dateKiosque"] != System.DBNull.Value)
                    {
                        dateMediaNum = Convert.ToInt64(row["dateKiosque"]);
                    }
                    if (disponibility <= 10 && activation <= 100 && idMedia > 0 && dateCoverNum > 0)
                    {
                        //visuel(s) disponible(s)
                        string[] files = row["visual"].ToString().Split(',');
                        for (int fileIndex = 0; fileIndex < files.Length; fileIndex++)
                        {
                            if (files[fileIndex].Length > 0)
                            {
                                visuals.Add(this.GetCreativePathPress(files[fileIndex], idMedia, dateCoverNum, false, dateMediaNum));
                            }
                        }
                    }
                    break;
                case CstDBClassif.Vehicles.names.indoor:
                case CstDBClassif.Vehicles.names.outdoor:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_OUTDOOR_CREATION_ACCESS_FLAG))
                    {
                        break;
                    }

                    if (row["associated_file"] != System.DBNull.Value)
                    {
                        string[] files = row["associated_file"].ToString().Split(',');
                        foreach (string s in files)
                        {
                            visuals.Add(this.GetCreativePathOutDoor(s, false));
                        }

                    }
                    break;
                case CstDBClassif.Vehicles.names.instore:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_INSTORE_CREATION_ACCESS_FLAG)) {
                        break;
                    }

                    if (row["associated_file"] != System.DBNull.Value) {
                        string[] files = row["associated_file"].ToString().Split(',');
                        foreach (string s in files) {
                            visuals.Add(this.GetCreativePathInStore(s, false));
                        }

                    }
                    break;
                case CstDBClassif.Vehicles.names.directMarketing:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DIRECT_MARKETING_CREATION_ACCESS_FLAG))
                    {
                        break;
                    }

                    if (row["associated_file"] != System.DBNull.Value)
                    {
                        string[] files = row["associated_file"].ToString().Split(',');
                        foreach (string s in files)
                        {
                            visuals.Add(this.GetCreativePathVMC(s, false));
                        }

                    }
                    break;
                case CstDBClassif.Vehicles.names.radio:
                case CstDBClassif.Vehicles.names.radioGeneral:
                case CstDBClassif.Vehicles.names.radioSponsorship:
                case CstDBClassif.Vehicles.names.radioMusic:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_RADIO_CREATION_ACCESS_FLAG))
                    {
                        break;
                    }

                    if (row["associated_file"] != System.DBNull.Value)
                    {
                        string[] files = row["associated_file"].ToString().Split(',');
                        foreach (string s in files)
                        {
                            visuals.Add(this.GetCreativePathRadio(s));
                        }

                    }
                    break;
                case CstDBClassif.Vehicles.names.tv:
                case CstDBClassif.Vehicles.names.others:
                case CstDBClassif.Vehicles.names.tvGeneral:
                case CstDBClassif.Vehicles.names.tvSponsorship:
                case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                case CstDBClassif.Vehicles.names.tvAnnounces:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_TV_CREATION_ACCESS_FLAG))
                    {
                        break;
                    }

                    if (row["associated_file"] != System.DBNull.Value)
                    {
                        string[] files = row["associated_file"].ToString().Split(',');
                        foreach (string s in files)
                        {
                            visuals.Add(this.GetCreativePathTv(s));
                        }

                    }
                    break;
                case CstDBClassif.Vehicles.names.adnettrack:
                case CstDBClassif.Vehicles.names.internet:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_INTERNET_ACCESS_FLAG))
                    {
                        break;
                    }

                    if (row["associated_file"] != System.DBNull.Value)
                    {
                        string[] files = row["associated_file"].ToString().Split(',');
                        foreach (string s in files)
                        {
                            visuals.Add(this.GetCreativePathAdNetTrack(s));
                        }

                    }
                    break;
                case CstDBClassif.Vehicles.names.evaliantMobile:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_EVALIANT_MOBILE_ACCESS_FLAG)) {
                        break;
                    }
                    if (row["associated_file"] != System.DBNull.Value) {
                        string[] files = row["associated_file"].ToString().Split(',');
                        foreach (string s in files) {
                            visuals.Add(this.GetCreativePathEvaliantMobile(s));
                        }

                    }
                    break;
                default:
                    break;
            }

            return visuals;

        }
        #endregion

        #region GetCreativePathPress
        protected string GetCreativePathPress(string file, Int64 idMedia, Int64 dateCoverNum, bool bigSize, Int64 dateMediaNum)
        {
            string imagette = (bigSize)?string.Empty:"/Imagette";

            lock (_mutex)
            {
                if (_mediaList == null)
                {
                    try
                    {
                        _mediaList = Media.GetItemsList(AdExpressUniverse.CREATIVES_KIOSQUE_LIST_ID).MediaList.Split(',');
                    }
                    catch { }
                }
            }
            if (Array.IndexOf(_mediaList, idMedia.ToString()) > -1)
            {
                return string.Format("{0}/{1}/{2}{3}/{4}", CstWeb.CreationServerPathes.IMAGES, idMedia, dateMediaNum, imagette, file);
            }
            else
            {
                return string.Format("{0}/{1}/{2}{3}/{4}", CstWeb.CreationServerPathes.IMAGES, idMedia, dateCoverNum, imagette, file);
            }
        }
        #endregion

        #region GetCreativePathOutDoor
        virtual protected string GetCreativePathOutDoor(string file, bool bigSize)
        {
            string imagette = (bigSize)?string.Empty:"/Imagette";

            return string.Format("{0}/{1}/{2}/{3}{4}/{5}"
                , CstWeb.CreationServerPathes.IMAGES_OUTDOOR
                , file.Substring(file.Length - 8, 1)
                , file.Substring(file.Length - 9, 1)
                , file.Substring(file.Length - 10, 1)
                , imagette
                , file);

        }
        protected string GetCreativePathInStore(string file, bool bigSize) {
            string imagette = (bigSize) ? string.Empty : "/Imagette";

            return string.Format("{0}/{1}/{2}/{3}{4}/{5}"
                , CstWeb.CreationServerPathes.IMAGES_INSTORE
                , file.Substring(file.Length - 8, 1)
                , file.Substring(file.Length - 9, 1)
                , file.Substring(file.Length - 10, 1)
                , imagette
                , file);

        }
        #endregion

        #region GetCreativePathVMC
        virtual protected string GetCreativePathVMC(string file, bool bigSize)
        {
            string imagette = (bigSize)?string.Empty:"/Imagette";

            return string.Format("{0}/{1}/{2}/{3}{4}/{5}"
                , CstWeb.CreationServerPathes.IMAGES_MD
                , file.Substring(file.Length - 8, 1)
                , file.Substring(file.Length - 9, 1)
                , file.Substring(file.Length - 10, 1)
                , imagette
                , file);
        }
        #endregion

        #region GetCreativePathRadio
        virtual protected string GetCreativePathRadio(string file)
        {
            return file;
        }
        #endregion

        #region GetCreativePathAdNetTrack
        virtual protected string GetCreativePathAdNetTrack(string file){
            return string.Format("{0}/{1}", CstWeb.CreationServerPathes.CREA_ADNETTRACK, file);
        }
        #endregion

        #region GetCreativePathEvaliantMobile
        virtual protected string GetCreativePathEvaliantMobile(string file){
            return string.Format("{0}/{1}", CstWeb.CreationServerPathes.CREA_EVALIANT_MOBILE, file);
        }
        #endregion

        #region GetCreativePathTv
        virtual protected string GetCreativePathTv(string file){
            return file;
        }
        #endregion

        #endregion
    }
}
