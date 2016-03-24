using System;
using System.Collections.Generic;
using System.Linq;
using TNS.AdExpress.Domain;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CsCustomer = TNS.AdExpress.Constantes.Customer;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstFlags = TNS.AdExpress.Constantes.DB.Flags;
using CstVMCFormat = TNS.AdExpress.Constantes.DB.Format;

using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Insertions.Exceptions;
using TNS.AdExpressI.Insertions.DAL;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.FrameWork.WebResultUI;
using System.Data;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.Insertions.Cells;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Results;
using System.Text;

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
        ///  media items without top diffusion
        /// </summary>
        protected List<long> _mediasWithoutTopDif = null;

        /// <summary>
        /// Get media items without top diffusion
        /// </summary>
        public List<long> MediasWithoutTopDif
        {
            get
            {
                if (_mediasWithoutTopDif == null)
                {
                    MediaItemsList items = null;
                    if (Media.Contains(CstWeb.AdExpressUniverse.TV_VEHICLE_WITHOUT_TOP_DIFFUSION))
                        items = Media.GetItemsList(CstWeb.AdExpressUniverse.TV_VEHICLE_WITHOUT_TOP_DIFFUSION);

                    if (items != null && !string.IsNullOrEmpty(items.MediaList))
                    {
                        _mediasWithoutTopDif = new List<string>(items.MediaList.Split(',')).ConvertAll(Convert.ToInt64);
                    }
                }
                return _mediasWithoutTopDif;
            }
        }

        /// <summary>
        ///  Categories items without top diffusion
        /// </summary>
        protected List<long> _categoriesWithoutTopDif = null;

        /// <summary>
        /// Get Categories items without top diffusion
        /// </summary>
        public List<long> CategoriesWithoutTopDif
        {
            get
            {
                if (_categoriesWithoutTopDif == null)
                {
                    MediaItemsList items = null;
                    if (Media.Contains(CstWeb.AdExpressUniverse.TV_VEHICLE_WITHOUT_TOP_DIFFUSION))
                        items = Media.GetItemsList(CstWeb.AdExpressUniverse.TV_VEHICLE_WITHOUT_TOP_DIFFUSION);

                    if (items != null && !string.IsNullOrEmpty(items.CategoryList))
                    {
                        _categoriesWithoutTopDif = new List<string>(items.CategoryList.Split(',')).ConvertAll(Convert.ToInt64);
                    }
                }
                return _categoriesWithoutTopDif;
            }
        }


        #endregion

        #region RenderType
        /// <summary>
        /// Define Render Type
        /// </summary>
        protected RenderType _renderType = RenderType.html;
        /// <summary>
        /// Get render type Session
        /// </summary>
        public RenderType RenderType
        {
            get { return _renderType; }
            set { _renderType = value; }
        }



        #endregion

        #region UseBlurImageForPress

        protected bool _useBlurImageForPress = false;

        /// <summary>
        ///Use Blur Image For Press
        /// </summary>
        public bool UseBlurImageForPress
        {
            get { return _useBlurImageForPress; }
            set { _useBlurImageForPress = value; }
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
            var param = new object[2];

            param[0] = session;
            param[1] = moduleId;
            CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.insertionsDAL];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the insertions DAL"));
            _dalLayer = (IInsertionsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.
                CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false,
                System.Reflection.BindingFlags.CreateInstance |
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null);
        }
        #endregion

        #region GetVehicles

        /// <summary>
        /// Get vehicles matching filters and which has data
        /// </summary>
        /// <param name="filters">Filters to apply (id1,id2,id3,id4</param>
        /// <param name="universId">univers Id</param>
        /// <param name="sloganNotNull">true if slogan Not Null</param>
        /// <returns>List of vehicles with data</returns>
        public virtual List<VehicleInformation> GetPresentVehicles(string filters, int universId, bool sloganNotNull)
        {
            var vehicles = new List<VehicleInformation>();

            DateTime dateBegin = Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            DateTime dateEnd = Dates.getPeriodEndDate(_session.PeriodEndDate, _session.PeriodType);
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
                    for (int i = vehicles.Count - 1; i >= 0; i--)
                    {
                        if (Array.IndexOf(list, vehicles[i].DatabaseId.ToString()) < 0)
                        {
                            vehicles.Remove(vehicles[i]);
                        }
                    }
                    break;
                case CstWeb.Module.Name.ANALYSE_DES_DISPOSITIFS:
                case CstWeb.Module.Name.ANALYSE_DES_PROGRAMMES:
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.tv));
                    break;
            }

            AddVehiclesInformation(vehicles);

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

        /// <summary>
        /// Add Vehicles Information
        /// </summary>
        /// <param name="vehicles">media type liste</param>
        protected virtual void AddVehiclesInformation(List<VehicleInformation> vehicles)
        {
            if (vehicles.Count <= 0)
            {
                if (VehiclesInformation.Contains(Vehicles.names.others))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.others));
                if (VehiclesInformation.Contains(Vehicles.names.directMarketing))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.directMarketing));
                if (VehiclesInformation.Contains(Vehicles.names.mailValo))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.mailValo));
                if (VehiclesInformation.Contains(Vehicles.names.internet))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.internet));
                if (VehiclesInformation.Contains(Vehicles.names.czinternet))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.czinternet));
                if (VehiclesInformation.Contains(Vehicles.names.adnettrack))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.adnettrack));
                if (VehiclesInformation.Contains(Vehicles.names.evaliantMobile))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.evaliantMobile));
                if (VehiclesInformation.Contains(Vehicles.names.press))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.press));
                if (VehiclesInformation.Contains(Vehicles.names.newspaper))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.newspaper));
                if (VehiclesInformation.Contains(Vehicles.names.magazine))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.magazine));
                if (VehiclesInformation.Contains(Vehicles.names.outdoor))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.outdoor));
                if (VehiclesInformation.Contains(Vehicles.names.indoor))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.indoor));
                if (VehiclesInformation.Contains(Vehicles.names.instore))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.instore));
                if (VehiclesInformation.Contains(Vehicles.names.radio))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.radio));
                if (VehiclesInformation.Contains(Vehicles.names.radioGeneral))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.radioGeneral));
                if (VehiclesInformation.Contains(Vehicles.names.radioSponsorship))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.radioSponsorship));
                if (VehiclesInformation.Contains(Vehicles.names.radioMusic))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.radioMusic));
                if (VehiclesInformation.Contains(Vehicles.names.tv)) vehicles.Add(VehiclesInformation.Get(Vehicles.names.tv));
                if (VehiclesInformation.Contains(Vehicles.names.tvGeneral))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.tvGeneral));
                if (VehiclesInformation.Contains(Vehicles.names.tvSponsorship))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.tvSponsorship));
                if (VehiclesInformation.Contains(Vehicles.names.tvNonTerrestrials))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.tvNonTerrestrials));
                if (VehiclesInformation.Contains(Vehicles.names.tvAnnounces))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.tvAnnounces));
                if (VehiclesInformation.Contains(Vehicles.names.cinema))
                    vehicles.Add(VehiclesInformation.Get(Vehicles.names.cinema));
            }
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
            var vehicles = new List<VehicleInformation>();
            Int64[] vIds = null;

            try
            {
                //Get media classification filters
                var filters = MediaDetailLevel.GetFilters(_session, idLevel1, idLevel2, idLevel3, idLevel4);
                vIds = _dalLayer.GetVehiclesIds(filters);
                vehicles.AddRange(vIds.Where(VehiclesInformation.Contains).Select(VehiclesInformation.Get));
            }
            catch (Exception ex)
            {
                throw (new InsertionsException("Unable to get media classifications level matching the filters.", ex));
            }
            return vehicles;
        }
        #endregion

        #region GetInsertions
        public virtual ResultTable GetInsertions(VehicleInformation vehicle, int fromDate, int toDate, string filters,
            int universId, string zoomDate)
        {
            _getCreatives = false;
            _zoomDate = zoomDate;
            _universId = universId;
            return GetData(vehicle, fromDate, toDate, filters, universId);
        }
        #endregion

        #region GetCreatives
        public virtual ResultTable GetCreatives(VehicleInformation vehicle, int fromDate, int toDate, string filters,
            int universId, string zoomDate)
        {
            _getCreatives = true;
            _zoomDate = zoomDate;
            _universId = universId;
            return GetData(vehicle, fromDate, toDate, filters, universId);
        }
        #endregion

        #region GetMSCreatives
        public virtual ResultTable GetMSCreatives(VehicleInformation vehicle, int fromDate, int toDate, string filters,
            int universId, string zoomDate)
        {
            _getMSCreatives = true;
            _zoomDate = zoomDate;
            _universId = universId;
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
            string vignettes = string.Empty;
            const string imagesList = "";
            const string dateField = "date_media_num";
            string themeName = WebApplicationParameters.Themes[_session.SiteLanguage].Name;

            if (_session.CustomerLogin.ShowCreatives(VehiclesInformation.DatabaseIdToEnum(idVehicle)))
            {//droit créations
                if (currentRow["associated_file"] != DBNull.Value && !string.IsNullOrEmpty(currentRow["associated_file"].ToString()))
                {
                    string[] fileList;
                    switch (VehiclesInformation.DatabaseIdToEnum(idVehicle))
                    {
                        case Vehicles.names.press:
                        case Vehicles.names.newspaper:
                        case Vehicles.names.magazine:
                        case Vehicles.names.internationalPress:
                            vignettes = GetPressVignettes(currentRow, dateField, vignettes, imagesList);
                            break;
                        case Vehicles.names.radio:
                        case Vehicles.names.radioGeneral:
                        case Vehicles.names.radioSponsorship:
                        case Vehicles.names.radioMusic:
                            vignettes = GetVignettes(idVehicle, currentRow, vignettes, themeName, "Picto_Radio.gif");
                            break;
                        case Vehicles.names.tv:
                        case Vehicles.names.tvGeneral:
                        case Vehicles.names.tvSponsorship:
                        case Vehicles.names.tvAnnounces:
                        case Vehicles.names.tvNonTerrestrials:
                        case Vehicles.names.others:
                            vignettes = GetVignettes(idVehicle, currentRow, vignettes, themeName, "Picto_pellicule.gif");
                            break;
                        case Vehicles.names.mailValo:
                        case Vehicles.names.directMarketing:
                        case Vehicles.names.outdoor:
                        case Vehicles.names.indoor:
                        case Vehicles.names.instore:
                            vignettes = GetOutDoorVignettes(idVehicle, currentRow, vignettes, imagesList);
                            break;
                        case Vehicles.names.adnettrack:
                            vignettes = GetEvaliantVignettes(currentRow, vignettes, themeName, CreationServerPathes.CREA_ADNETTRACK);
                            break;
                        case Vehicles.names.evaliantMobile:
                            vignettes = GetEvaliantVignettes(currentRow, vignettes, themeName, CreationServerPathes.CREA_EVALIANT_MOBILE);
                            break;
                    }
                }
            }

            string sloganDetail = "\n<table border=\"0\" width=\"50\" height=\"64\" class=\"txtViolet10\">";

            if (vignettes.Length > 0)
            {
                sloganDetail += "\n<tr><td   nowrap align=\"center\">";
                sloganDetail += vignettes;
                sloganDetail += "\n</td></tr>";
            }

            sloganDetail += "\n<tr><td  nowrap align=\"center\">";
            sloganDetail += currentRow["id_slogan"].ToString();
            if (currentRow["advertDimension"] != DBNull.Value && currentRow["advertDimension"] != DBNull.Value)
            {
                var vehicleEnum = VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString()));
                if ((vehicleEnum != Vehicles.names.directMarketing && vehicleEnum != Vehicles.names.mailValo)
                    || (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_POIDS_MARKETING_DIRECT)))
                    sloganDetail += string.Format(" - {0}", currentRow["advertDimension"].ToString());
            }
            sloganDetail += "\n</td></tr>";
            sloganDetail += "\n</table>";

            return sloganDetail;

        }


        protected virtual string GetEvaliantVignettes(DataRow currentRow, string vignettes, string themeName, string creativePath)
        {
            if (currentRow["associated_file"] != DBNull.Value && !string.IsNullOrEmpty(currentRow["associated_file"].ToString()))
                vignettes =
                    string.Format(
                        "<a href=\"javascript:openEvaliantCreative('{1}/{0}', '{3}');\"><img border=\"0\" src=\"/App_Themes/{2}/Images/Common/Button/adnettrack.gif\"></a>",
                        currentRow["associated_file"].ToString().Replace(@"\", "/"),
                        creativePath, themeName, currentRow["advertDimension"]);
            return vignettes;
        }

        protected virtual string GetOutDoorVignettes(long idVehicle, DataRow currentRow, string vignettes,
                                                     string imagesList)
        {
            bool first = true;
            string pathWeb;
            string[] fileList = currentRow["associated_file"].ToString().Split(',');
            string idAssociatedFile = currentRow["associated_file"].ToString();

            var vehicleEnum = VehiclesInformation.DatabaseIdToEnum(long.Parse(idVehicle.ToString()));
            if (vehicleEnum == Vehicles.names.directMarketing ||
                vehicleEnum == Vehicles.names.mailValo)
                pathWeb = CreationServerPathes.IMAGES_MD;
            else pathWeb = CreationServerPathes.IMAGES_OUTDOOR;
            string dir1 = idAssociatedFile.Substring(idAssociatedFile.Length - 8, 1);
            pathWeb = string.Format(@"{0}/{1}", pathWeb, dir1);
            string dir2 = idAssociatedFile.Substring(idAssociatedFile.Length - 9, 1);
            pathWeb = string.Format(@"{0}/{1}", pathWeb, dir2);
            string dir3 = idAssociatedFile.Substring(idAssociatedFile.Length - 10, 1);
            pathWeb = string.Format(@"{0}/{1}/imagette/", pathWeb, dir3);

            if (fileList != null && fileList.Length > 0)
            {
                for (int j = 0; j < fileList.Length; j++)
                {
                    vignettes += string.Format("<img src='{0}{1}' border=\"0\" width=\"50\" height=\"64\" >", pathWeb, fileList[j]);
                    if (first) imagesList = string.Format("{0}{1}", pathWeb, fileList[j]);
                    else
                    {
                        imagesList += string.Format(",{0}{1}", pathWeb, fileList[j]);
                    }
                    first = false;
                }

                if (vignettes.Length > 0)
                {
                    vignettes = string.Format("<a href=\"javascript:openPressCreation('{0}');\">{1}</a>"
                        , imagesList.Replace("/Imagette", ""), vignettes);
                    vignettes += "\n<br>";
                }
            }
            else vignettes = GestionWeb.GetWebWord(843, _session.SiteLanguage) + "<br>";
            return vignettes;
        }

        protected virtual string GetPressVignettes(DataRow currentRow, string dateField, string vignettes,
                                                   string imagesList)
        {
            bool first = true;
            var fileList = currentRow["associated_file"].ToString().Split(',');

            if (_session.CurrentModule == CstWeb.Module.Name.BILAN_CAMPAGNE) dateField = "date_cover_num";

            string blurDirectory = UseBlurImageForPress ? "blur/" : string.Empty;

            string pathWebImagette = string.Format("{0}/{1}/{2}/imagette/{3}", CreationServerPathes.IMAGES, currentRow["id_media"].ToString()
                , currentRow[dateField].ToString(), blurDirectory);
            string pathWeb = string.Format("{0}/{1}/{2}/{3}", CreationServerPathes.IMAGES, currentRow["id_media"].ToString()
                , currentRow[dateField].ToString(), blurDirectory);

            foreach (string file in fileList)
            {
                vignettes += string.Format("<img src='{0}{1}' border=\"0\" width=\"50\" height=\"64\" >", pathWebImagette, file);
                if (first) imagesList = string.Format("{0}{1}", pathWeb, file);
                else
                {
                    imagesList += string.Format(",{0}{1}", pathWeb, file);
                }
                first = false;
            }

            if (vignettes.Length > 0)
            {
                vignettes = string.Format("<a href=\"javascript:openPressCreation('{0}');\">{1}</a>", imagesList.Replace("/Imagette", ""), vignettes);
                vignettes += "\n<br>";
            }
            return vignettes;
        }

        protected virtual string GetVignettes(long idVehicle, DataRow currentRow, string vignettes, string themeName, string picto)
        {
            if (currentRow["associated_file"] != DBNull.Value && !string.IsNullOrEmpty(currentRow["associated_file"].ToString()))
                vignettes =
                    string.Format(
                        "<a href=\"javascript:openDownload('{0}','{1}','{2}');\"><img border=\"0\" src=\"/App_Themes/{3}/Images/Common/{4}\"></a>"
                        , currentRow["associated_file"].ToString(), _session.IdSession, idVehicle, themeName, picto);
            return vignettes;
        }

        #endregion




        #region GetData
        /// <summary>
        /// Get Data
        /// </summary>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="fromDate">Data beginning</param>
        /// <param name="toDate">Date end</param>
        /// <param name="filters">filters</param>
        /// <param name="universId">univers Ids</param>
        /// <returns></returns>
        protected virtual ResultTable GetData(VehicleInformation vehicle, int fromDate, int toDate, string filters,
            int universId)
        {
            ResultTable data;

            #region Data Access
            if (vehicle == null)
                return null;

            DataSet ds;

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

            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                return null;
            #endregion

            DataTable dt = ds.Tables[0];

            #region Init ResultTable
            var levels = new List<DetailLevelItemInformation>();
            if (!_getMSCreatives)
            {
                levels.AddRange(_session.DetailLevel.Levels.Cast<DetailLevelItemInformation>());
            }
            List<GenericColumnItemInformation> columns;

            if (_getMSCreatives)
                columns = WebApplicationParameters.MsCreativesDetail.GetDetailColumns(vehicle.DatabaseId);
            else if (_getCreatives)
            {
                columns = _session.GenericCreativesColumns.Columns;
            }
            else
                columns = _session.GenericInsertionColumns.Columns;

            bool hasVisualRight = false;
            switch (vehicle.Id)
            {
                case Vehicles.names.adnettrack:
                case Vehicles.names.internet:
                case Vehicles.names.czinternet:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DETAIL_INTERNET_ACCESS_FLAG);
                    break;
                case Vehicles.names.evaliantMobile:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DETAIL_EVALIANT_MOBILE_ACCESS_FLAG);
                    break;
                case Vehicles.names.mailValo:
                case Vehicles.names.directMarketing:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DIRECT_MARKETING_CREATION_ACCESS_FLAG);
                    break;
                case Vehicles.names.internationalPress:
                case Vehicles.names.newspaper:
                case Vehicles.names.magazine:
                case Vehicles.names.press:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_PRESS_CREATION_ACCESS_FLAG);
                    break;
                case Vehicles.names.others:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_OTHERS_CREATION_ACCESS_FLAG);
                    break;
                case Vehicles.names.indoor:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_INDOOR_CREATION_ACCESS_FLAG);
                    break;
                case Vehicles.names.outdoor:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_OUTDOOR_CREATION_ACCESS_FLAG);
                    break;
                case Vehicles.names.instore:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_INSTORE_CREATION_ACCESS_FLAG);
                    break;
                case Vehicles.names.radio:
                case Vehicles.names.radioGeneral:
                case Vehicles.names.radioSponsorship:
                case Vehicles.names.radioMusic:
                    hasVisualRight = _session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_RADIO_CREATION_ACCESS_FLAG);
                    break;
                case Vehicles.names.tv:
                case Vehicles.names.tvGeneral:
                case Vehicles.names.tvSponsorship:
                case Vehicles.names.tvNonTerrestrials:
                case Vehicles.names.tvAnnounces:
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
                    hasVisuals = hasVisualRight;
                }
                if (c.Id == GenericColumnItemInformation.Columns.content)
                {
                    //Data Base ID
                    if (!string.IsNullOrEmpty(c.DataBaseAliasIdField))
                    {
                        divideCol = c.DataBaseAliasIdField.ToUpper();
                    }
                    else if (!string.IsNullOrEmpty(c.DataBaseIdField))
                    {
                        divideCol = c.DataBaseIdField.ToUpper();
                    }
                    //Database Label
                    if (!string.IsNullOrEmpty(c.DataBaseAliasField))
                    {
                        divideCol = c.DataBaseAliasField.ToUpper();
                    }
                    else if (!string.IsNullOrEmpty(c.DataBaseField))
                    {
                        divideCol = c.DataBaseField.ToUpper();
                    }
                    break;
                }
            }

            Int64 idColumnsSet = -1;
            if (_getMSCreatives)
                idColumnsSet = WebApplicationParameters.MsCreativesDetail.GetDetailColumnsId(vehicle.DatabaseId);
            else if (_getCreatives)
            {
                idColumnsSet = WebApplicationParameters.CreativesDetail.GetDetailColumnsId(vehicle.DatabaseId, _module.Id);
            }
            else
            {
                idColumnsSet = WebApplicationParameters.InsertionsDetail.GetDetailColumnsId(vehicle.DatabaseId, _module.Id);
            }

            //Data Keys
            var keys = WebApplicationParameters.GenericColumnsInformation.GetKeys(idColumnsSet);
            var keyIdName = new List<string>();
            var keyLabelName = new List<string>();
            GetKeysColumnNames(dt, keys, keyIdName, keyLabelName);
            //Line Number
            int nbLine = GetLineNumber(dt, levels, keyIdName);
            //Data Columns
            var cells = new List<Cell>();
            var columnsName = GetColumnsName(dt, columns, cells);
            //Result Table init
            var root = GetHeaders(vehicle, columns, hasVisuals);
            data = root != null ? new ResultTable(nbLine, root) : new ResultTable(nbLine, 1);


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
                if (!hasVisuals)
                {
                    setLine = SetRawLine;
                }
                else
                {
                    switch (vehicle.Id)
                    {
                        case Vehicles.names.directMarketing:
                        case Vehicles.names.mailValo:
                        case Vehicles.names.internationalPress:
                        case Vehicles.names.press:
                        case Vehicles.names.newspaper:
                        case Vehicles.names.magazine:
                        case Vehicles.names.outdoor:
                        case Vehicles.names.instore:
                        case Vehicles.names.indoor:
                            setLine = SetAggregLine;
                            break;
                        default:
                            setLine = SetRawLine;
                            break;
                    }
                }
            }
            #endregion

            #region Table fill
            var lineTypes = new[] { LineType.level1, LineType.level2, LineType.level3, LineType.level4 };

            var oldIds = new Int64[levels.Count];
            for (int i = 0; i < oldIds.Length; i++) { oldIds[i] = -1; }
            var cIds = new Int64[levels.Count];

            var oldKeyIds = new Int64[keys.Count];
            for (int i = 0; i < oldKeyIds.Length; i++) { oldKeyIds[i] = -1; }
            var cKeyIds = new Int64[keys.Count];

            int cLine = 0;

            foreach (DataRow row in dt.Rows)
            {

                bool isNewInsertion = false;
                //Detail levels
                for (int i = 0; i < oldIds.Length; i++)
                {
                    cIds[i] = Convert.ToInt64(row[levels[i].DataBaseIdField]);
                    if (cIds[i] != oldIds[i])
                    {
                        oldIds[i] = cIds[i];
                        if (i < oldIds.Length - 1)
                        {
                            oldIds[i + 1] = -1;
                            for (int j = 0; j < oldKeyIds.Length; j++) { oldKeyIds[j] = -1; }
                        }

                        //Set current level
                        cLine = data.AddNewLine(lineTypes[i]);
                        switch (levels[i].Id)
                        {
                            case DetailLevelItemInformation.Levels.date:
                                data[cLine, 1] = new CellDate(Dates.getPeriodBeginningDate(row[levels[i].DataBaseField].ToString(),
                                    CustomerSessions.Period.Type.dateToDate), "{0:shortdatepattern}");
                                break;
                            case DetailLevelItemInformation.Levels.duration:
                                data[cLine, 1] = new CellDuration(Convert.ToDouble(row[levels[i].DataBaseField]));
                                ((CellUnit)data[cLine, 1]).StringFormat = string.Format("{{0:{0}}}",
                                    WebApplicationParameters.GenericColumnItemsInformation.
                                    Get(GenericColumnItemInformation.Columns.duration.GetHashCode()).StringFormat);
                                break;
                            default:
                                data[cLine, 1] = new CellLabel(row[levels[i].DataBaseField].ToString());
                                break;

                        }

                        for (int j = 2; j <= data.DataColumnsNumber; j++)
                        {
                            data[cLine, j] = new CellEmpty();
                        }

                    }
                }

                //Insertion Keys
                for (int i = 0; i < oldKeyIds.Length; i++)
                {
                    cKeyIds[i] = Convert.ToInt64(row[keyIdName[i]]);
                    if (cKeyIds[i] != oldKeyIds[i])
                    {
                        oldKeyIds[i] = cKeyIds[i];
                        if (i < oldKeyIds.Length - 1)
                        {
                            oldKeyIds[i + 1] = -1;
                        }
                        isNewInsertion = true;
                    }
                }

                if (isNewInsertion)
                {
                    cLine = data.AddNewLine(lineTypes[3]);
                }

                if (_getMSCreatives)
                    setSpecificLine(vehicle, data, row, cLine, columns, columnsName, cells, idColumnsSet);
                else
                    setLine(vehicle, data, row, cLine, columns, columnsName, cells, divideCol);

            }

            #endregion

            return data;
        }


        protected virtual void SetRawLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine,
            List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, string divideCol)
        {
            int i = -1;
            int j = 0;
            MediaItemsList tntMediaItems = null;

            foreach (GenericColumnItemInformation g in columns)
            {

                i++;
                j++;
                if (cells[i] is CellUnit)
                {
                    int div = 1;
                    if (g.IsSum)
                    {
                        div = Math.Max(div, row[divideCol].ToString().Split(',').Length);
                    }
                    Double val = 0;
                    if (row[columnsName[i]] != DBNull.Value)
                    {
                        val = Convert.ToDouble(row[columnsName[i]]) / div;
                    }
                    switch (columns[i].Id)
                    {
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
                            if (IsTvVehicle(vehicle))
                            {
                                long idCategory =
                                    Convert.ToInt64(row[WebApplicationParameters.GenericColumnItemsInformation.
                                                                                 Get(
                                                                                     (long)
                                                                                     GenericColumnItemInformation
                                                                                         .Columns.idCategory)
                                                                                .DataBaseField].ToString());
                                long idMedia =
                                   Convert.ToInt64(row[WebApplicationParameters.GenericColumnItemsInformation.
                                                                                Get(
                                                                                    (long)
                                                                                    GenericColumnItemInformation
                                                                                        .Columns.media)
                                                                               .DataBaseIdField].ToString());

                                if ((MediasWithoutTopDif != null && MediasWithoutTopDif.Contains(idMedia))
                                    || (CategoriesWithoutTopDif != null && CategoriesWithoutTopDif.Contains(idCategory)))
                                {
                                    val = 0;
                                }
                            }
                            break;
                        default:
                            break;
                    }
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
                    string s = string.Empty;
                    switch (columns[i].Id)
                    {
                        case GenericColumnItemInformation.Columns.associatedFile:
                            switch (vehicle.Id)
                            {
                                case Vehicles.names.others:
                                case Vehicles.names.tv:
                                case Vehicles.names.tvGeneral:
                                case Vehicles.names.tvSponsorship:
                                case Vehicles.names.tvAnnounces:
                                case Vehicles.names.tvNonTerrestrials:
                                    s = row[columnsName[i]].ToString();
                                    if (s.Length > 0)
                                        tab[cLine, j] = new CellTvCreativeLink(s, _session, vehicle.DatabaseId);
                                    else
                                        tab[cLine, j] = new CellTvCreativeLink(string.Empty, _session, vehicle.DatabaseId);
                                    break;
                                case Vehicles.names.radio:
                                    tab[cLine, j] = new CellRadioCreativeLink(row[columnsName[i]].ToString(),
                                        _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radio));

                                    break;
                                case Vehicles.names.radioGeneral:
                                    tab[cLine, j] = new CellRadioCreativeLink(row[columnsName[i]].ToString(),
                                        _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioGeneral));
                                    break;
                                case Vehicles.names.radioSponsorship:
                                    tab[cLine, j] = new CellRadioCreativeLink(row[columnsName[i]].ToString(),
                                        _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioSponsorship));
                                    break;
                                case Vehicles.names.radioMusic:
                                    tab[cLine, j] = new CellRadioCreativeLink(row[columnsName[i]].ToString(),
                                        _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radioMusic));
                                    break;
                            }
                            break;
                        case GenericColumnItemInformation.Columns.dayOfWeek:
                            Int32 n = Convert.ToInt32(row[columnsName[i]]);
                            int y = n / 10000;
                            int m = (n - (10000 * y)) / 100;
                            int d = n - (10000 * y + 100 * m);
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


        protected virtual bool IsTvVehicle(VehicleInformation vehicle)
        {
            return (vehicle.Id == Vehicles.names.tv
                    || vehicle.Id == Vehicles.names.tvGeneral
                    || vehicle.Id == Vehicles.names.tvAnnounces
                    || vehicle.Id == Vehicles.names.tvSponsorship
                    || vehicle.Id == Vehicles.names.tvNonTerrestrials);
        }
        protected virtual void SetAggregLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine,
            List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, string divideCol)
        {

            CellInsertionInformation c;
            var visuals = new List<string>();
            if (tab[cLine, 1] == null)
            {
                switch (vehicle.Id)
                {
                    case Vehicles.names.mailValo:
                    case Vehicles.names.directMarketing:
                        tab[cLine, 1] = c = new CellInsertionVMCInformation(_session, columns, columnsName, cells);
                        break;
                    case Vehicles.names.press:
                        c = new CellInsertionInformation(_session, columns, columnsName, cells);
                        tab[cLine, 1] = c;
                        break;
                    default:
                        tab[cLine, 1] = c = new CellInsertionInformation(_session, columns, columnsName, cells);
                        break;
                }
            }
            else
            {
                c = (CellInsertionInformation)tab[cLine, 1];
            }
            foreach (GenericColumnItemInformation g in columns)
            {
                if (g.Id == GenericColumnItemInformation.Columns.visual || g.Id == GenericColumnItemInformation.Columns.associatedFile
                    || g.Id == GenericColumnItemInformation.Columns.poster)
                {
                    visuals = GetPath(vehicle, row, columns, columnsName);
                }
            }
            c.Add(row, visuals);

        }
        protected virtual void SetCreativeLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine,
            List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, string divideCol)
        {

            CellCreativesInformation c;
            List<string> visuals = new List<string>();
            if (tab[cLine, 1] == null)
            {
                switch (vehicle.Id)
                {
                    case CstDBClassif.Vehicles.names.mailValo:
                    case CstDBClassif.Vehicles.names.directMarketing:
                        tab[cLine, 1] = c = new CellCreativesVMCInformation(_session, vehicle, columns, columnsName, cells, _module);
                        break;
                    case CstDBClassif.Vehicles.names.radio:
                    case CstDBClassif.Vehicles.names.radioSponsorship:
                    case CstDBClassif.Vehicles.names.radioGeneral:
                    case CstDBClassif.Vehicles.names.radioMusic:
                        tab[cLine, 1] = c = GetCellCreativesRadioInformation(vehicle, columns, columnsName, cells);
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
                if (g.Id == GenericColumnItemInformation.Columns.visual || g.Id == GenericColumnItemInformation.Columns.associatedFile
                    || g.Id == GenericColumnItemInformation.Columns.poster || g.Id == GenericColumnItemInformation.Columns.associatedFileMax)
                {
                    visuals = GetPath(vehicle, row, columns, columnsName);
                }
            }
            c.Add(row, visuals);

        }

        protected virtual CellCreativesRadioInformation GetCellCreativesRadioInformation(VehicleInformation vehicle, List<GenericColumnItemInformation> columns
            , List<string> columnsName, List<Cell> cells)
        {
            return new CellCreativesRadioInformation(_session, vehicle, columns, columnsName, cells, _module);
        }


        protected virtual void SetMSCreativeLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine,
            List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, Int64 idColumnsSet)
        {

            CellCreativesInformation c;
            var visuals = new List<string>();
            if (tab[cLine, 1] == null)
            {
                switch (vehicle.Id)
                {
                    case Vehicles.names.mailValo:
                    case Vehicles.names.directMarketing:
                        tab[cLine, 1] = c = new CellCreativesVMCInformation(_session, vehicle, columns, columnsName, cells, _module, idColumnsSet);
                        break;
                    case Vehicles.names.radio:
                    case Vehicles.names.radioSponsorship:
                    case Vehicles.names.radioGeneral:
                    case Vehicles.names.radioMusic:
                        tab[cLine, 1] = c = GetCellCreativesRadioInformation(vehicle, columns, columnsName, cells, idColumnsSet);
                        break;
                    case Vehicles.names.tv:
                    case Vehicles.names.tvGeneral:
                    case Vehicles.names.tvSponsorship:
                    case Vehicles.names.tvAnnounces:
                    case Vehicles.names.tvNonTerrestrials:
                    case Vehicles.names.others:
                        tab[cLine, 1] = c = new CellCreativesTvInformation(_session, vehicle, columns,
                            columnsName, cells, _module, idColumnsSet);
                        break;
                    case Vehicles.names.adnettrack:
                    case Vehicles.names.internet:
                        tab[cLine, 1] = c = new CellCreativesEvaliantInformation(_session, vehicle, columns,
                            columnsName, cells, _module, _zoomDate, _universId, idColumnsSet);
                        break;
                    case Vehicles.names.evaliantMobile:
                        tab[cLine, 1] = c = new CellCreativesEvaliantMobileInformation(_session, vehicle,
                            columns, columnsName, cells, _module, _zoomDate, _universId, idColumnsSet);
                        break;
                    default:
                        tab[cLine, 1] = c = new CellCreativesInformation(_session, vehicle, columns, columnsName, cells, _module, idColumnsSet);
                        break;
                }
            }
            else
            {
                c = (CellCreativesInformation)tab[cLine, 1];
            }
            foreach (GenericColumnItemInformation g in columns)
            {
                if (g.Id == GenericColumnItemInformation.Columns.visual || g.Id == GenericColumnItemInformation.Columns.associatedFile
                    || g.Id == GenericColumnItemInformation.Columns.poster || g.Id == GenericColumnItemInformation.Columns.associatedFileMax)
                {
                    visuals = GetPath(vehicle, row, columns, columnsName);
                }
            }
            c.Add(row, visuals);

        }

        protected virtual CellCreativesRadioInformation GetCellCreativesRadioInformation(VehicleInformation vehicle, List<GenericColumnItemInformation> columns,
            List<string> columnsName, List<Cell> cells, long idColumnsSet)
        {
            return
                new CellCreativesRadioInformation(_session, vehicle, columns, columnsName, cells, _module, idColumnsSet);


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

            var oldIds = new Int64[levels.Count];
            for (int i = 0; i < oldIds.Length; i++) { oldIds[i] = -1; }
            var cIds = new Int64[levels.Count];

            var oldKeyIds = new Int64[keys.Count];
            for (int i = 0; i < oldKeyIds.Length; i++) { oldKeyIds[i] = -1; }
            var cKeyIds = new Int64[keys.Count];
            foreach (DataRow row in dt.Rows)
            {
                bool isNewInsertion = false;
                //Detail levels
                for (int i = 0; i < oldIds.Length; i++)
                {
                    cIds[i] = Convert.ToInt64(row[levels[i].DataBaseIdField]);
                    if (cIds[i] != oldIds[i])
                    {
                        oldIds[i] = cIds[i];
                        nbLine++;
                        if (i < oldIds.Length - 1)
                        {
                            oldIds[i + 1] = -1;
                            for (int j = 0; j < oldKeyIds.Length; j++) { oldKeyIds[j] = -1; }
                        }
                    }
                }

                //Insertion Keys
                for (int i = 0; i < oldKeyIds.Length; i++)
                {
                    cKeyIds[i] = Convert.ToInt64(row[keys[i]]);
                    if (cKeyIds[i] != oldKeyIds[i])
                    {
                        oldKeyIds[i] = cKeyIds[i];
                        isNewInsertion = true;
                        if (i < oldKeyIds.Length - 1)
                        {
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
        /// <param name="dt">data table</param>
        /// <param name="columns">List of columns</param>
        /// <param name="cells">cells</param>
        /// <returns>List of data column names</returns>
        protected virtual List<string> GetColumnsName(DataTable dt, List<GenericColumnItemInformation> columns, List<Cell> cells)
        {

            var names = new List<string>();
            string name = string.Empty;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(@"TNS.FrameWork.WebResultUI");

            foreach (GenericColumnItemInformation g in columns)
            {

                if (!string.IsNullOrEmpty(g.DataBaseAliasField))
                {
                    name = g.DataBaseAliasField.ToUpper();
                }
                else if (!string.IsNullOrEmpty(g.DataBaseField))
                {
                    name = g.DataBaseField.ToUpper();
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

                        Type type = assembly.GetType(g.CellType);
                        Cell cellUnit = (Cell)type.InvokeMember("GetInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.InvokeMethod, null, null, null);
                        cellUnit.StringFormat = string.Format("{{0:{0}}}", g.StringFormat);
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
        protected virtual void GetKeysColumnNames(DataTable dt, List<GenericColumnItemInformation> columns, List<string> idsColumn, List<string> labelsColumn)
        {
            foreach (GenericColumnItemInformation g in columns)
            {
                //Init stirngs
                var idName = string.Empty;
                var labelName = string.Empty;

                //Data Base ID
                if (!string.IsNullOrEmpty(g.DataBaseAliasIdField))
                {
                    labelName = idName = g.DataBaseAliasIdField.ToUpper();
                }
                else if (!string.IsNullOrEmpty(g.DataBaseIdField))
                {
                    labelName = idName = g.DataBaseIdField.ToUpper();
                }
                //Database Label
                if (!string.IsNullOrEmpty(g.DataBaseAliasField))
                {
                    labelName = g.DataBaseAliasField.ToUpper();
                }
                else if (!string.IsNullOrEmpty(g.DataBaseField))
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
        protected virtual Headers GetHeaders(VehicleInformation vehicle, List<GenericColumnItemInformation> columns, bool hasVisual)
        {

            var root = new Headers();
            if (_getCreatives || _getMSCreatives)
            {
                return null;
            }
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
                    case Vehicles.names.mailValo:
                    case Vehicles.names.directMarketing:
                    case Vehicles.names.adnettrack:
                    case Vehicles.names.evaliantMobile:
                    case Vehicles.names.internet:
                    case Vehicles.names.czinternet:
                    case Vehicles.names.internationalPress:
                    case Vehicles.names.outdoor:
                    case Vehicles.names.instore:
                    case Vehicles.names.indoor:
                    case Vehicles.names.press:
                    case Vehicles.names.newspaper:
                    case Vehicles.names.magazine:
                        return null;
                    case Vehicles.names.others:
                    case Vehicles.names.radio:
                    case Vehicles.names.radioGeneral:
                    case Vehicles.names.radioSponsorship:
                    case Vehicles.names.radioMusic:
                    case Vehicles.names.tv:
                    case Vehicles.names.tvGeneral:
                    case Vehicles.names.tvSponsorship:
                    case Vehicles.names.tvNonTerrestrials:
                    case Vehicles.names.tvAnnounces:
                        for (int i = 0; i < columns.Count; i++)
                        {
                            root.Root.Add(new Header(GestionWeb.GetWebWord(columns[i].WebTextId, _session.SiteLanguage), columns[i].Id.GetHashCode()));
                        }
                        break;
                }
            }

            return root;

        }
        #endregion

        #region Init cells
        protected List<Cell> GetCells(List<GenericColumnItemInformation> columns)
        {

            var cells = new List<Cell>();
            int i = -1;

            foreach (GenericColumnItemInformation g in columns)
            {

                i++;
                try
                {
                    var assembly = System.Reflection.Assembly.Load(@"TNS.FrameWork.WebResultUI");
                    Type type = assembly.GetType(g.CellType);
                    var cell = (Cell)type.InvokeMember("GetInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
                                                                       | System.Reflection.BindingFlags.InvokeMethod, null, null, null);
                    cell.StringFormat = g.StringFormat;
                }
                catch (Exception)
                {
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
            return vehicle != null && vehicle.Id != Vehicles.names.internet;
        }

        #endregion

        #region Creatives Rules

        #region GetPath
        /// <summary>
        /// Get Path
        /// </summary>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="row">data row</param>
        /// <param name="columns">columns</param>
        /// <param name="columnNames">columnNames</param>
        /// <returns></returns>
        protected virtual List<string> GetPath(VehicleInformation vehicle, DataRow row, List<GenericColumnItemInformation> columns, List<string> columnNames)
        {
            var visuals = new List<string>();

            switch (vehicle.Id)
            {
                case Vehicles.names.press:
                case Vehicles.names.newspaper:
                case Vehicles.names.magazine:
                case Vehicles.names.internationalPress:

                    if (CheckPressFlagAccess(vehicle)) break;

                    Int64 idMedia;
                    Int64 dateCoverNum;
                    Int64 dateMediaNum;

                    if (IsVisualAvailable(row, out idMedia, out dateCoverNum, out dateMediaNum))
                    {
                        //visuel(s) disponible(s)
                        string[] files = row["visual"].ToString().Split(',');
                        for (int fileIndex = 0; fileIndex < files.Length; fileIndex++)
                        {
                            if (files[fileIndex].Length > 0)
                            {
                                visuals.Add(GetCreativePathPress(files[fileIndex], idMedia, dateCoverNum, false, dateMediaNum));
                            }
                        }
                    }
                    break;
                case Vehicles.names.indoor:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_INDOOR_CREATION_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathOutDoor);
                    break;
                case Vehicles.names.outdoor:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_OUTDOOR_CREATION_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathOutDoor);
                    break;
                case Vehicles.names.instore:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_INSTORE_CREATION_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathInStore);
                    break;
                case Vehicles.names.directMarketing:
                case Vehicles.names.mailValo:
                    if (
                        !_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DIRECT_MARKETING_CREATION_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathVMC);
                    break;
                case Vehicles.names.radio:
                case Vehicles.names.radioGeneral:
                case Vehicles.names.radioSponsorship:
                case Vehicles.names.radioMusic:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_RADIO_CREATION_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathRadio);
                    break;
                case Vehicles.names.tv:
                case Vehicles.names.others:
                case Vehicles.names.tvGeneral:
                case Vehicles.names.tvSponsorship:
                case Vehicles.names.tvNonTerrestrials:
                case Vehicles.names.tvAnnounces:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_TV_CREATION_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathTv);
                    break;
                case Vehicles.names.adnettrack:
                case Vehicles.names.czinternet:
                case Vehicles.names.internet:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_INTERNET_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathAdNetTrack);
                    break;
                case Vehicles.names.evaliantMobile:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_EVALIANT_MOBILE_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathEvaliantMobile);
                    break;
            }

            return visuals;

        }

        /// <summary>
        /// Add Visuals
        /// </summary>
        /// <param name="row">Data row</param>
        /// <param name="visuals">Visuals list</param>
        /// <param name="getCreativePath">extact creative path</param>
        protected virtual void AddVisuals(DataRow row, List<string> visuals, Func<string, bool, string> getCreativePath)
        {
            if (row["associated_file"] != DBNull.Value)
            {
                var files = row["associated_file"].ToString().Split(',');
                visuals.AddRange(files.Select(s => getCreativePath(s, false)));
            }
        }
        /// <summary>
        /// Add Visuals
        /// </summary>
        /// <param name="row">Data row</param>
        /// <param name="visuals">Visuals list</param>
        /// <param name="getCreativePath">extact creative path</param>
        protected virtual void AddVisuals(DataRow row, List<string> visuals, Func<string, string> getCreativePath)
        {
            if (row["associated_file"] != DBNull.Value)
            {
                var files = row["associated_file"].ToString().Split(',');
                visuals.AddRange(files.Select(getCreativePath));
            }
        }

        /// <summary>
        /// Check Press FlagAccess
        /// </summary>
        /// <param name="vehicle">Media Type</param>
        /// <returns>True if has no press flag access</returns>
        protected virtual bool CheckPressFlagAccess(VehicleInformation vehicle)
        {
            if ((vehicle.Id == Vehicles.names.internationalPress &&
                 !_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_INTERNATIONAL_PRESS_CREATION_ACCESS_FLAG))
                ||
                (vehicle.Id == Vehicles.names.press &&
                 !_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRESS_CREATION_ACCESS_FLAG))
                ||
                (vehicle.Id == Vehicles.names.newspaper &&
                 !_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRESS_CREATION_ACCESS_FLAG))
                ||
                (vehicle.Id == Vehicles.names.magazine &&
                 !_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRESS_CREATION_ACCESS_FLAG))
                )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if visual is available
        /// </summary>
        /// <param name="row">Data  row</param>
        /// <param name="idMedia">Id vehicle</param>
        /// <param name="dateCoverNum">cover date</param>
        /// <param name="dateMediaNum">date</param>
        /// <returns>True if viual is available</returns>
        protected virtual bool IsVisualAvailable(DataRow row, out long idMedia, out long dateCoverNum, out long dateMediaNum)
        {
            Int64 disponibility = -1;
            Int64 activation = -1;
            dateCoverNum = -1;
            dateMediaNum = -1;
            idMedia = -1;
            if (row.Table.Columns.Contains("disponibility_visual") && row["disponibility_visual"] != DBNull.Value)
            {
                disponibility = Convert.ToInt64(row["disponibility_visual"]);
            }
            if (row.Table.Columns.Contains("activation") && row["activation"] != DBNull.Value)
            {
                activation = Convert.ToInt64(row["activation"]);
            }
            if (row.Table.Columns.Contains("id_media") && row["id_media"] != DBNull.Value)
            {
                idMedia = Convert.ToInt64(row["id_media"]);
            }
            if (row.Table.Columns.Contains("date_cover_num") && row["date_cover_num"] != DBNull.Value)
            {
                dateCoverNum = Convert.ToInt64(row["date_cover_num"]);
            }
            if (row.Table.Columns.Contains("date_media_num") && row["date_media_num"] != DBNull.Value)
            {
                dateMediaNum = Convert.ToInt64(row["date_media_num"]);
            }
            if (row.Table.Columns.Contains("dateKiosque") && row["dateKiosque"] != DBNull.Value)
            {
                dateMediaNum = Convert.ToInt64(row["dateKiosque"]);
            }
            return (disponibility <= 10 && activation <= 100 && idMedia > 0 && dateCoverNum > 0);
        }

        //protected virtual bool HasPressCopyright(DataRow row)
        //{

        //    if (row.Table.Columns.Contains("id_media") && row["id_media"] != DBNull.Value)
        //    {
        //       long idMedia = Convert.ToInt64(row["id_media"]);
        //       string ids = Lists.GetIdList(GroupList.ID.media, GroupList.Type.mediaExcludedForCopyright);
        //       var notAllowedMediaIds = ids.Split(',').Select( p => Convert.ToInt64(p)).ToList();
        //        return !notAllowedMediaIds.Contains(idMedia);
        //    }
        //    return true;
        //}

        protected virtual bool HasPressCopyright(long idMedia)
        {
            string ids = Lists.GetIdList(GroupList.ID.media, GroupList.Type.mediaExcludedForCopyright);
            if (!string.IsNullOrEmpty(ids))
            {
                var notAllowedMediaIds = ids.Split(',').Select(p => Convert.ToInt64(p)).ToList();
                return !notAllowedMediaIds.Contains(idMedia);
            }
            return true;
        }

        /// <summary>
        /// Check if parution date is before the year 2015
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        protected virtual bool ParutionDateBefore2015(string date)
        {

            string year = date.Substring(0, 4);

            if (int.Parse(year) < 2015)
                return true;
            else
                return false;

        }
        #endregion

        #region GetCreativePathPress
        /// <summary>
        /// Get Creative Path Press
        /// </summary>
        /// <param name="file">file</param>
        /// <param name="idMedia">id Media</param>
        /// <param name="dateCoverNum">date Cover 
        /// </param>
        /// <param name="bigSize">true if big size else false</param>
        /// <param name="dateMediaNum">date Media </param>
        /// <returns>creative path</returns>
        protected virtual string GetCreativePathPress(string file, Int64 idMedia, Int64 dateCoverNum, bool bigSize, Int64 dateMediaNum)
        {
            string imagette = (bigSize) ? string.Empty : "/Imagette";
            string blurDirectory = string.Empty;
            long date = dateCoverNum;

            lock (_mutex)
            {
                if (_mediaList == null)
                {
                    try
                    {
                        _mediaList = Media.GetItemsList(AdExpressUniverse.CREATIVES_KIOSQUE_LIST_ID).MediaList.Split(',');
                    }
                    catch
                    { }
                }
            }
            if (_mediaList != null && Array.IndexOf(_mediaList, idMedia.ToString()) > -1)
                date = dateMediaNum;

            if (!HasPressCopyright(idMedia) && !ParutionDateBefore2015(date.ToString()))
                blurDirectory = "/blur";
            return string.Format("{0}/{1}/{2}{3}{4}/{5}", CreationServerPathes.IMAGES, idMedia, date, imagette, blurDirectory, file);
        }
        #endregion

        #region GetCreativePathOutDoor
        protected virtual string GetCreativePathOutDoor(string file, bool bigSize)
        {
            string imagette = (bigSize) ? string.Empty : "/Imagette";

            return string.Format("{0}/{1}/{2}/{3}{4}/{5}"
                , CreationServerPathes.IMAGES_OUTDOOR
                , file.Substring(file.Length - 8, 1)
                , file.Substring(file.Length - 9, 1)
                , file.Substring(file.Length - 10, 1)
                , imagette
                , file);

        }
        protected virtual string GetCreativePathInStore(string file, bool bigSize)
        {
            string imagette = (bigSize) ? string.Empty : "/Imagette";

            return string.Format("{0}/{1}/{2}/{3}{4}/{5}"
                , CreationServerPathes.IMAGES_INSTORE
                , file.Substring(file.Length - 8, 1)
                , file.Substring(file.Length - 9, 1)
                , file.Substring(file.Length - 10, 1)
                , imagette
                , file);

        }
        #endregion

        #region GetCreativePathVMC
        protected virtual string GetCreativePathVMC(string file, bool bigSize)
        {
            string imagette = (bigSize) ? string.Empty : "/Imagette";

            return string.Format("{0}/{1}/{2}/{3}{4}/{5}"
                , CreationServerPathes.IMAGES_MD
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
        virtual protected string GetCreativePathAdNetTrack(string file)
        {
            return string.Format("{0}/{1}", CreationServerPathes.CREA_ADNETTRACK, file);
        }
        #endregion

        #region GetCreativePathEvaliantMobile
        virtual protected string GetCreativePathEvaliantMobile(string file)
        {
            return string.Format("{0}/{1}", CreationServerPathes.CREA_EVALIANT_MOBILE, file);
        }
        #endregion

        #region GetCreativePathTv
        virtual protected string GetCreativePathTv(string file)
        {
            return file;
        }

        public GridResult GetInsertionsGridResult(VehicleInformation vehicle, int fromDate, int toDate, string filters, int universId, string zoomDate)
        {
            GridResult gridResult = new GridResult();

            ResultTable _data = GetInsertions(vehicle, fromDate, toDate, filters, universId, zoomDate);

            if (_data != null)
            {
                _data.Sort(ResultTable.SortOrder.NONE, 1);

                int k;
                object[,] gridData = new object[_data.LinesNumber, _data.ColumnsNumber + 2]; //+2 car ID et PID en plus  -  //_data.LinesNumber
                List<object> columns = new List<object>();
                List<object> schemaFields = new List<object>();
                List<object> columnsFixed = new List<object>();

                _data.CultureInfo = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
                //int creativeIndexInResultTable = -1;

                if (_data.NewHeaders != null)
                {
                    columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
                    schemaFields.Add(new { name = "ID" });
                    columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
                    schemaFields.Add(new { name = "PID" });

                    for (int j = 0; j < _data.NewHeaders.Root.Count; j++)
                    {

                        //if (_data[j, 1] is CellImageLink)
                        //{
                        //    creativeIndexInResultTable = _data.NewHeaders.Root[j].IndexInResultTable;

                        //    columns.Add(new { headerText = _data.NewHeaders.Root[j].Label, key = _data.NewHeaders.Root[j].Label, dataType = "string", width = "200px" });
                        //    schemaFields.Add(new { name = _data.NewHeaders.Root[j].Label });
                        //}
                        //else {
                            columns.Add(new { headerText = _data.NewHeaders.Root[j].Label, key = _data.NewHeaders.Root[j].Label, dataType = "string", width = "200px" });
                            schemaFields.Add(new { name = _data.NewHeaders.Root[j].Label });
                        //}
                    }
                }

                try
                {
                    for (int i = 0; i < _data.LinesNumber; i++) //_data.LinesNumber
                    {
                        gridData[i, 0] = i; // Pour column ID
                        gridData[i, 1] = _data.GetSortedParentIndex(i); // Pour column PID

                        for (k = 1; k < _data.ColumnsNumber - 1; k++)
                        {
                            gridData[i, k + 1] = _data[i, k].RenderString();
                        }
                    }
                }
                catch (Exception err)
                {
                    throw (new Exception(err.Message));
                }

                gridResult.HasData = true;
                gridResult.Columns = columns;
                gridResult.Schema = schemaFields;
                gridResult.ColumnsFixed = columnsFixed;
                gridResult.Data = gridData;

            }
            else
            {
                gridResult.HasData = false;
                return (gridResult);
            }



            return gridResult;

        }
        #endregion

        #endregion

    }
}
