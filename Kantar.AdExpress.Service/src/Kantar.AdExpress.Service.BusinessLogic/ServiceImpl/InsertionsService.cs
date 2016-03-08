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
        GenericColumnItemInformation _genericColumnItemInformation = null;
       

        public InsertionResponse GetInsertionsGridResult(string idWebSession, string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle)
        {
            InsertionResponse insertionResponse = new InsertionResponse();
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

                //TODO : TROUVER OU IL EST CHARGE
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

                #region Initialisation de la liste des niveaux de détail
                _allowedDetailItemList = WebApplicationParameters.InsertionsDetail.GetAllowedMediaDetailLevelItems(insertionResponse.IdVehicle);
                _defaultDetailItemList = WebApplicationParameters.InsertionsDetail.GetDefaultMediaDetailLevels(insertionResponse.IdVehicle);
                #endregion

                foreach (GenericDetailLevel detailItem in _defaultDetailItemList)
                    levels = detailItem.LevelIds;
                List<Int64> genericColumnList = new List<Int64>();
                foreach (GenericColumnItemInformation Column in _columnItemList)
                {                   
                        genericColumnList.Add((int)Column.Id);
                       // _columnItemSelectedList.Add(Column);//TODO: A checker                   
                }
                _customerWebSession.GenericInsertionColumns = new GenericColumns(genericColumnList);
                _customerWebSession.DetailLevel = new GenericDetailLevel(levels, WebCst.GenericDetailLevel.SelectedFrom.defaultLevels);

                // _customerWebSession.Save();

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
            catch (Exception)
            {
                insertionResponse.Message = GestionWeb.GetWebWord(959,_customerWebSession.SiteLanguage);
            }

            return insertionResponse;

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
