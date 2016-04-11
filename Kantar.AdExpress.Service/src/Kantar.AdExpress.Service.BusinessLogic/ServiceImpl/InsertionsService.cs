using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kantar.AdExpress.Service.Core.Domain;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpressI.Insertions;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Translation;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstFlags = TNS.AdExpress.Constantes.DB.Flags;
using TNS.AdExpress.Domain.Level;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Web;
using System.Collections;
using WebCst = TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class InsertionsService : IInsertionsService
    {
        private WebSession _customerWebSession = null;
        private int _fromDate;
        private int _toDate;
        private long? _idVehicle = long.MinValue;   	
        private long _columnSetId;
        /// <summary>
		/// Liste des éléments de détail par défaut
		/// </summary>
		private List<GenericDetailLevel> _defaultDetailItemList = null;
        /// <summary>
        /// Liste des éléments de détail par personnalisées
        /// </summary>
        private List<DetailLevelItemInformation> _allowedDetailItemList = null;
        /// <summary>
		/// Liste des niveaux de détaille personnalisés
		/// </summary>
		private ArrayList _DetailLevelItemList = new ArrayList();
        /// <summary>
		/// Nombre de niveaux de détaille personnalisés
		/// </summary>
		protected int _nbDetailLevelItemList = 3;
        /// <summary>
		/// Liste des colonnes personnalisées
		/// </summary>
		private List<GenericColumnItemInformation> _columnItemList = null;

        /// <summary>
        /// Indique si l'utilisateur à le droit de lire les créations
        /// </summary>
        private bool _hasCreationReadRights = false;

        /// <summary>
        /// Indique si l'utilisateur à le droit de télécharger les créations
        /// </summary>
        private bool _hasCreationDownloadRights = false;


        public InsertionCreativeResponse GetInsertionsGridResult(string idWebSession, string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle, bool isVehicleChanged = false)
        {
            InsertionCreativeResponse insertionResponse = new InsertionCreativeResponse();
            ArrayList levels = new ArrayList();
            try
            {
                _customerWebSession = (WebSession)WebSession.Load(idWebSession);
              
                IInsertionsResult insertionResult = InitInsertionCall(_customerWebSession, moduleId);

                insertionResponse.Vehicles = insertionResult.GetPresentVehicles(ids, idUnivers, false);
                if (insertionResponse.Vehicles.Count <= 0)
                {
                    return insertionResponse;
                }

                if (idVehicle.HasValue)
                {
                    insertionResponse.IdVehicle = idVehicle.Value;
                }
                _idVehicle = insertionResponse.IdVehicle;
                VehicleInformation vehicle = VehiclesInformation.Get(insertionResponse.IdVehicle);

                

                string message = string.Empty;
                if (vehicle.Id == CstDBClassif.Vehicles.names.outdoor &&
                       !_customerWebSession.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DETAIL_OUTDOOR_ACCESS_FLAG))
                {
                    insertionResponse.Message = GestionWeb.GetWebWord(1882, _customerWebSession.SiteLanguage);
                }
                else if (vehicle.Id == CstDBClassif.Vehicles.names.instore &&
                    !_customerWebSession.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DETAIL_INSTORE_ACCESS_FLAG))
                {
                    insertionResponse.Message = GestionWeb.GetWebWord(2668, _customerWebSession.SiteLanguage);
                }
                else if (vehicle.Id == CstDBClassif.Vehicles.names.indoor &&
                    !_customerWebSession.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DETAIL_INDOOR_ACCESS_FLAG))
                {
                    insertionResponse.Message = GestionWeb.GetWebWord(2992, _customerWebSession.SiteLanguage);
                }

                if (!string.IsNullOrEmpty(insertionResponse.Message)) return insertionResponse;

                if ((vehicle.Id == CstDBClassif.Vehicles.names.internet
                   || vehicle.Id == CstDBClassif.Vehicles.names.czinternet) && !insertionResult.CanShowInsertion(vehicle))
                {
                    insertionResponse.Message = GestionWeb.GetWebWord(2244, _customerWebSession.SiteLanguage);
                    return insertionResponse;
                }

                //date
                TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType = _customerWebSession.PeriodType;
                string periodBegin = _customerWebSession.PeriodBeginningDate;
                string periodEnd = _customerWebSession.PeriodEndDate;

                if (!string.IsNullOrEmpty(zoomDate))
                {
                    periodType = _customerWebSession.DetailPeriod == TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.weekly
                        ? TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDateWeek : TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDateMonth;
                    _fromDate = Convert.ToInt32(
                        Dates.Max(Dates.getZoomBeginningDate(zoomDate, periodType),
                            Dates.getPeriodBeginningDate(periodBegin, _customerWebSession.PeriodType)).ToString("yyyyMMdd")
                        );
                    _toDate = Convert.ToInt32(
                        Dates.Min(Dates.getZoomEndDate(zoomDate, periodType),
                            Dates.getPeriodEndDate(periodEnd, _customerWebSession.PeriodType)).ToString("yyyyMMdd")
                        );
                }
                else
                {
                    _fromDate = Convert.ToInt32(Dates.getZoomBeginningDate(periodBegin, periodType).ToString("yyyyMMdd"));
                    _toDate = Convert.ToInt32(Dates.getZoomEndDate(periodEnd, periodType).ToString("yyyyMMdd"));
                }

                _columnSetId = WebApplicationParameters.InsertionsDetail.GetDetailColumnsId(insertionResponse.IdVehicle, _customerWebSession.CurrentModule);
                _defaultDetailItemList = WebApplicationParameters.InsertionsDetail.GetDefaultMediaDetailLevels(insertionResponse.IdVehicle);
                _allowedDetailItemList = WebApplicationParameters.InsertionsDetail.GetAllowedMediaDetailLevelItems(insertionResponse.IdVehicle);

                List<GenericColumnItemInformation> tmp = WebApplicationParameters.InsertionsDetail.GetDetailColumns(insertionResponse.IdVehicle, _customerWebSession.CurrentModule);
                _columnItemList = new List<GenericColumnItemInformation>();
                foreach (GenericColumnItemInformation column in tmp)
                {
                    if (WebApplicationParameters.GenericColumnsInformation.IsVisible(_columnSetId, column.Id))
                    {
                        _columnItemList.Add(column);
                    }
                }

                _columnItemList = ColumnRight(_columnItemList);

                if (isVehicleChanged)
                {
                    foreach (GenericDetailLevel detailItem in _defaultDetailItemList)
                        levels = detailItem.LevelIds;

                    _customerWebSession.DetailLevel = new GenericDetailLevel(levels, WebCst.GenericDetailLevel.SelectedFrom.defaultLevels);
                }
                List<Int64> genericColumnList = new List<Int64>();
                foreach (GenericColumnItemInformation Column in _columnItemList)
                {                   
                        genericColumnList.Add((int)Column.Id);
                       // _columnItemSelectedList.Add(Column);//TODO: A checker                   
                }
                _customerWebSession.GenericInsertionColumns = new GenericColumns(genericColumnList);

                _customerWebSession.Save();

                if (_customerWebSession.GenericInsertionColumns.Columns.Count < 1)
                {
                    return insertionResponse;
                }

                //TODO: A GERER pour les exports Excel
                //if (this._renderType != RenderType.html)
                //{
                //    var columns = _customerWebSession.GenericInsertionColumns.Columns;
                //    var columnIds = (columns.Where(
                //        g =>
                //        g.Id != GenericColumnItemInformation.Columns.associatedFile &&
                //        g.Id != GenericColumnItemInformation.Columns.associatedFileMax &&
                //        g.Id != GenericColumnItemInformation.Columns.poster &&
                //        g.Id != GenericColumnItemInformation.Columns.visual).Select(g => g.Id.GetHashCode()))
                //        .Select(dummy => (long)dummy).ToList();
                //    _customerWebSession.GenericInsertionColumns = new GenericColumns(columnIds);

                //    result.RenderType = _renderType;
                //}


                insertionResponse.GridResult = insertionResult.GetInsertionsGridResult(vehicle, _fromDate, _toDate, ids, idUnivers, zoomDate);
               
            }
            catch (Exception ex)
            {
                insertionResponse.Message = GestionWeb.GetWebWord(959,_customerWebSession.SiteLanguage);
            }

            return insertionResponse;

        }

        public ResultTable GetInsertionsResult(string idWebSession, string ids, string zoomDate, int idUnivers, long moduleId, long idVehicle)
        {
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);
            IInsertionsResult insertionResult = InitInsertionCall(_customerWebSession, moduleId);
            VehicleInformation vehicle = VehiclesInformation.Get(idVehicle);


            //date
            TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType = _customerWebSession.PeriodType;
            string periodBegin = _customerWebSession.PeriodBeginningDate;
            string periodEnd = _customerWebSession.PeriodEndDate;

            if (!string.IsNullOrEmpty(zoomDate))
            {
                periodType = _customerWebSession.DetailPeriod == TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.weekly
                    ? TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDateWeek : TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDateMonth;
                _fromDate = Convert.ToInt32(
                    Dates.Max(Dates.getZoomBeginningDate(zoomDate, periodType),
                        Dates.getPeriodBeginningDate(periodBegin, _customerWebSession.PeriodType)).ToString("yyyyMMdd")
                    );
                _toDate = Convert.ToInt32(
                    Dates.Min(Dates.getZoomEndDate(zoomDate, periodType),
                        Dates.getPeriodEndDate(periodEnd, _customerWebSession.PeriodType)).ToString("yyyyMMdd")
                    );
            }
            else
            {
                _fromDate = Convert.ToInt32(Dates.getZoomBeginningDate(periodBegin, periodType).ToString("yyyyMMdd"));
                _toDate = Convert.ToInt32(Dates.getZoomEndDate(periodEnd, periodType).ToString("yyyyMMdd"));
            }


            return insertionResult.GetInsertions(vehicle, _fromDate, _toDate, ids, idUnivers, zoomDate);
           
        }

        public SpotResponse GetCreativePath(string idWebSession,string idVersion,long idVehicle)
        {
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);

            SpotResponse spotResponse = new SpotResponse
            {
                SiteLanguage = _customerWebSession.SiteLanguage
            };

            //L'utilisateur a accès au créations en lecture ?
            var vehicleName =  VehiclesInformation.Get(idVehicle).Id;   
            _hasCreationReadRights = _customerWebSession.CustomerLogin.ShowCreatives(vehicleName);

            if (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_DOWNLOAD_ACCESS_FLAG))
            {
                //L'utilisateur a accès aux créations en téléchargement
                _hasCreationDownloadRights = true;
            }


            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.creativePopUp];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the creative pop up"));
            var param = new object[6];
           
            param[0] = vehicleName;
            param[1] = idVersion;
            param[2] = string.Empty;
            param[3] = _customerWebSession;          
            param[4] = _hasCreationReadRights;
            param[5] = _hasCreationDownloadRights;
            var result = (TNS.AdExpressI.Insertions.CreativeResult.ICreativePopUp)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                , AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName), cl.Class, false, System.Reflection.BindingFlags.CreateInstance
                | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null);

            result.SetCreativePaths();

            spotResponse.PathDownloadingFile = result.PathDownloadingFile;
            spotResponse.PathReadingFile = result.PathReadingFile;
            return spotResponse;

        }

        public List<List<string>> GetPresentVehicles(string idWebSession, string ids, int idUnivers, long moduleId, bool slogan = false)
        {
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);
            IInsertionsResult insertionResult = InitInsertionCall(_customerWebSession, moduleId);

            List<VehicleInformation> Vehicles = insertionResult.GetPresentVehicles(ids, idUnivers, slogan);

            List<List<string>> ListRetour = new List<List<string>>();
            string vehicle = string.Empty;
            for (int i = 0; i < Vehicles.Count; i++)
            {
                switch (Vehicles[i].Id)
                {
                    case CstDBClassif.Vehicles.names.newspaper:
                        vehicle = GestionWeb.GetWebWord(2620, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.magazine:
                        vehicle = GestionWeb.GetWebWord(2621, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.press:
                        vehicle = GestionWeb.GetWebWord(1298, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.pressClipping:
                        vehicle = GestionWeb.GetWebWord(2955, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.internationalPress:
                        vehicle = GestionWeb.GetWebWord(646, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.radio:
                        vehicle = GestionWeb.GetWebWord(644, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.radioGeneral:
                        vehicle = GestionWeb.GetWebWord(2630, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.radioSponsorship:
                        vehicle = GestionWeb.GetWebWord(2632, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.radioMusic:
                        vehicle = GestionWeb.GetWebWord(2631, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.tv:
                        vehicle = GestionWeb.GetWebWord(1300, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.tvGeneral:
                        vehicle = GestionWeb.GetWebWord(2633, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.tvClipping:
                        vehicle = GestionWeb.GetWebWord(2956, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.tvSponsorship:
                        vehicle = GestionWeb.GetWebWord(2634, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.tvAnnounces:
                        vehicle = GestionWeb.GetWebWord(2635, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                        vehicle = GestionWeb.GetWebWord(2636, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.others:
                        vehicle = GestionWeb.GetWebWord(647, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.outdoor:
                        vehicle = GestionWeb.GetWebWord(1302, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.instore:
                        vehicle = GestionWeb.GetWebWord(2665, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.indoor:
                        vehicle = GestionWeb.GetWebWord(2644, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.adnettrack:
                        vehicle = GestionWeb.GetWebWord(648, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.directMarketing:
                    case CstDBClassif.Vehicles.names.mailValo:
                        vehicle = GestionWeb.GetWebWord(2989, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.internet:
                    case CstDBClassif.Vehicles.names.czinternet:
                        vehicle = GestionWeb.GetWebWord(1301, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.evaliantMobile:
                        vehicle = GestionWeb.GetWebWord(2577, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.cinema:
                        vehicle = GestionWeb.GetWebWord(2726, _customerWebSession.SiteLanguage);
                        break;
                    case CstDBClassif.Vehicles.names.editorial:
                        vehicle = GestionWeb.GetWebWord(2801, _customerWebSession.SiteLanguage);
                        break;
                }

                ListRetour.Add(new List<string> { Vehicles[i].DatabaseId.ToString(), vehicle });
            }

            return ListRetour;
        }


        public IInsertionsResult InitInsertionCall(WebSession custSession, long moduleId)
        {
            //**TODO : IdVehicules not null

            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.insertions];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the insertions rules"));
            var param = new object[2];
            param[0] = custSession;
            param[1] = moduleId;
            var result = (IInsertionsResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\"
                + cl.AssemblyName, cl.Class, false, System.Reflection.BindingFlags.CreateInstance
                | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null);


            return result;

        }

        /// <summary>
		/// Verifie les droits pour une liste de colonnes
		/// </summary>
		/// <param name="columnItemList">Liste des colonnes par média</param>
		/// <returns></returns>
        private List<GenericColumnItemInformation> ColumnRight(List<GenericColumnItemInformation> columnItemList)
        {
            List<GenericColumnItemInformation> columnList = new List<GenericColumnItemInformation>();

            foreach (GenericColumnItemInformation column in columnItemList)
                if (CanAddColumnItem(column))
                    columnList.Add(column);

            return (columnList);
        }

        /// <summary>
		/// Verifie les droits pour une colonne
		/// </summary>
		/// <param name="currentColumn">Colonne courante</param>
		/// <returns></returns>
		private bool CanAddColumnItem(GenericColumnItemInformation currentColumn)
        {
            switch (currentColumn.Id)
            {
                case GenericColumnItemInformation.Columns.slogan:
                    return _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG);
                case GenericColumnItemInformation.Columns.interestCenter:
                case GenericColumnItemInformation.Columns.mediaSeller:
                    return (!_customerWebSession.isCompetitorAdvertiserSelected());
                case GenericColumnItemInformation.Columns.visual:
                case GenericColumnItemInformation.Columns.associatedFile:
                    return _customerWebSession.CustomerLogin.ShowCreatives(VehiclesInformation.DatabaseIdToEnum(_idVehicle.Value));
                case GenericColumnItemInformation.Columns.product:
                    return _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
                default:
                    return (true);
            }
        }

        /// <summary>
        /// Get Allowed Detail Item List
        /// </summary>
        /// <returns>ID of Allowed Detail Item List</returns>
        protected List<int> GetAllowedDetailItemList()
        {
            List<int> list = new List<int>();
            foreach (DetailLevelItemInformation currentDetailLevelItem in _allowedDetailItemList)
            {
                if (CanAddDetailLevelItem(currentDetailLevelItem))
                {
                    list.Add(currentDetailLevelItem.Id.GetHashCode());
                }
            }
            return list;
        }

        ///<summary>
		/// Test si un niveau de détail peut être montré
		/// </summary>
		///  <param name="currentDetailLevel">
		///  </param>
		///  <returns>
		///  </returns>
		///  <url>element://model:project::TNS.AdExpress.Web.Controls/design:view:::a6hx33ynklrfe4g_v</url>
		private bool CanAddDetailLevelItem(DetailLevelItemInformation currentDetailLevel)
        {

            switch (currentDetailLevel.Id)
            {
                case DetailLevelItemInformation.Levels.slogan:
                    return _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG);
                case DetailLevelItemInformation.Levels.interestCenter:
                case DetailLevelItemInformation.Levels.mediaSeller:
                    return (!_customerWebSession.isCompetitorAdvertiserSelected());
                case DetailLevelItemInformation.Levels.brand:
                    return ((CheckProductDetailLevelAccess()) && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE));
                case DetailLevelItemInformation.Levels.product:
                    return ((CheckProductDetailLevelAccess()) && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG));
                case DetailLevelItemInformation.Levels.advertiser:
                    return (CheckProductDetailLevelAccess());
                case DetailLevelItemInformation.Levels.sector:
                case DetailLevelItemInformation.Levels.subSector:
                case DetailLevelItemInformation.Levels.group:
                    return (CheckProductDetailLevelAccess() && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_GROUP_LEVEL_ACCESS_FLAG));
                case DetailLevelItemInformation.Levels.segment:
                    return (CheckProductDetailLevelAccess() && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG));
                case DetailLevelItemInformation.Levels.holdingCompany:
                    return (CheckProductDetailLevelAccess());
                case DetailLevelItemInformation.Levels.groupMediaAgency:
                case DetailLevelItemInformation.Levels.agency:
                    List<Int64> vehicleList = GetVehicles();
                    return (_customerWebSession.CustomerLogin.CustomerMediaAgencyFlagAccess(vehicleList));
                case DetailLevelItemInformation.Levels.mediaGroup:
                    return _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MEDIA_GROUP);
                default:
                    return (true);
            }
        }


        /// <summary>
        /// Vérifie si le client à le droit de voir un détail produit dans les plan media
        /// </summary>
        /// <returns>True si oui false sinon</returns>
        private bool CheckProductDetailLevelAccess()
        {
            return (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG));

        }

        private List<Int64> GetVehicles()
        {
            List<Int64> vehicleList = new List<Int64>();
            string listStr = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
            if (listStr != null && listStr.Length > 0)
            {
                string[] list = listStr.Split(',');
                for (int i = 0; i < list.Length; i++)
                    vehicleList.Add(Convert.ToInt64(list[i]));
            }
            else
            {
                //When a vehicle is not checked but one or more category, this get the vehicle correspondly
                string Vehicle = ((LevelInformation)_customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
                vehicleList.Add(Convert.ToInt64(Vehicle));
            }
            return vehicleList;
        }


    }
}
