using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using Kantar.AdExpress.Service.Core.Domain;
using TNS.AdExpressI.Insertions;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Translation;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Level;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Web;
using System.Collections;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class CreativeService : ICreativeService
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

        /// <summary>
        /// Available Filters values
        /// </summary>
        protected Dictionary<GenericColumnItemInformation.Columns, List<string>> _availableFilterValues = new Dictionary<GenericColumnItemInformation.Columns, List<string>>();

        /// <summary>
        /// Customer Filters values
        /// </summary>
        protected Dictionary<GenericColumnItemInformation.Columns, List<string>> _customFilterValues = new Dictionary<GenericColumnItemInformation.Columns, List<string>>();

        /// <summary>
        /// Option Html Code
        /// </summary>
        protected string _optionHtml = string.Empty;

        public InsertionCreativeResponse GetCreativeGridResult(string idWebSession, string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle, bool isVehicleChanged, List<EvaliantFilter> evaliantFilter)
        {
            InsertionCreativeResponse creativeResponse = new InsertionCreativeResponse();
            ArrayList levels = new ArrayList();
            try
            {
                _customerWebSession = (WebSession)WebSession.Load(idWebSession);

                IInsertionsResult creativeResult = InitCreativeCall(_customerWebSession, moduleId);

                creativeResponse.Vehicles = creativeResult.GetPresentVehicles(ids, idUnivers, true);
                if (creativeResponse.Vehicles.Count <= 0)
                {
                    return creativeResponse;
                }

                if (idVehicle.HasValue)
                {
                    creativeResponse.IdVehicle = idVehicle.Value;
                }
                _idVehicle = creativeResponse.IdVehicle;
                VehicleInformation vehicle = VehiclesInformation.Get(creativeResponse.IdVehicle);

                string message = string.Empty;
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

                _columnSetId = WebApplicationParameters.CreativesDetail.GetDetailColumnsId(creativeResponse.IdVehicle, _customerWebSession.CurrentModule);
                _defaultDetailItemList = WebApplicationParameters.CreativesDetail.GetDefaultMediaDetailLevels(creativeResponse.IdVehicle);
                _allowedDetailItemList = WebApplicationParameters.CreativesDetail.GetAllowedMediaDetailLevelItems(creativeResponse.IdVehicle);

                List<GenericColumnItemInformation> tmp = WebApplicationParameters.CreativesDetail.GetDetailColumns(creativeResponse.IdVehicle, _customerWebSession.CurrentModule);
                _columnItemList = new List<GenericColumnItemInformation>();

                if (isVehicleChanged)
                {
                    if(evaliantFilter != null)
                        LoadCustomFilters(evaliantFilter);
                }

                foreach (GenericColumnItemInformation column in tmp)
                {
                    if (WebApplicationParameters.GenericColumnsInformation.IsVisible(_columnSetId, column.Id))
                    {
                        _columnItemList.Add(column);
                    }
                }

                var columns = WebApplicationParameters.CreativesDetail.GetDetailColumns(vehicle.DatabaseId, moduleId);
                var columnsId = new List<long>();
                var columnFilters = new List<GenericColumnItemInformation>();
                Int64 idColumnsSet = WebApplicationParameters.CreativesDetail.GetDetailColumnsId(vehicle.DatabaseId, moduleId);
                foreach (GenericColumnItemInformation g in columns)
                {
                    columnsId.Add(g.Id.GetHashCode());
                    if (WebApplicationParameters.GenericColumnsInformation.IsFilter(idColumnsSet, g.Id))
                    {
                        columnFilters.Add(g);
                        _availableFilterValues.Add(g.Id, new List<string>());
                    }
                }
                _customerWebSession.GenericCreativesColumns = new GenericColumns(columnsId);
                GenericDetailLevel saveLevels = null;
                try
                {
                    saveLevels = _customerWebSession.DetailLevel;
                }
                catch
                {
                    saveLevels = null;
                }
                _customerWebSession.DetailLevel = new GenericDetailLevel(new ArrayList());
                creativeResponse.GridResult = creativeResult.GetCreativesGridResult(vehicle, _fromDate, _toDate, ids, idUnivers, zoomDate, columnFilters, _availableFilterValues, _customFilterValues);
                if (saveLevels != null)
                {
                    _customerWebSession.DetailLevel = saveLevels;
                }

            }
            catch (Exception ex)
            {
                creativeResponse.Message = GestionWeb.GetWebWord(959, _customerWebSession.SiteLanguage);
            }

            return creativeResponse;

        }

        public void LoadCustomFilters(List<EvaliantFilter> evaliantFilter)
        {
            _customFilterValues = new Dictionary<GenericColumnItemInformation.Columns, List<string>>();
            for (int i = 0; i < evaliantFilter.Count; i++)
            {
                _customFilterValues.Add((GenericColumnItemInformation.Columns)evaliantFilter[i].IdFilter, evaliantFilter[i].ValuesFilter);
            }
        }


        public SpotResponse GetCreativePath(string idWebSession, string idVersion, long idVehicle)
        {
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);

            SpotResponse spotResponse = new SpotResponse
            {
                SiteLanguage = _customerWebSession.SiteLanguage
            };

            //L'utilisateur a accès au créations en lecture ?
            var vehicleName = VehiclesInformation.Get(idVehicle).Id;
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
            IInsertionsResult creativeResult = InitCreativeCall(_customerWebSession, moduleId);

            List<VehicleInformation> Vehicles = creativeResult.GetPresentVehicles(ids, idUnivers, slogan);

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


        public IInsertionsResult InitCreativeCall(WebSession custSession, long moduleId)
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
    }
}
