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
using TNS.AdExpressI.Insertions.Cells;
using TNS.FrameWork.WebResultUI;

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
        GenericColumnItemInformation _genericColumnItemInformation = null;
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

        public InsertionCreativeResponse GetCreativeGridResult(string idWebSession, string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle, bool isVehicleChanged)
        {
            InsertionCreativeResponse creativeResponse = new InsertionCreativeResponse();
            ArrayList levels = new ArrayList();
            try
            {
                _customerWebSession = (WebSession)WebSession.Load(idWebSession);

                IInsertionsResult creativeResult = InitCreativeCall(_customerWebSession, moduleId);

                creativeResponse.Vehicles = creativeResult.GetPresentVehicles(ids, idUnivers, false);
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
                //data = result.GetCreatives(vehicle, _fromDate, _toDate, ids, idUnivers, zoomDate);
                creativeResponse.GridResult = creativeResult.GetCreativesGridResult(vehicle, _fromDate, _toDate, ids, idUnivers, zoomDate);
                if (saveLevels != null)
                {
                    _customerWebSession.DetailLevel = saveLevels;
                }
                if (creativeResponse.GridResult != null && creativeResponse.GridResult.HasData)
                {
                    //VOIR YOUGIL POUR LES FILTRES
                   //BuildCustomFilter(ref data, columnFilters);
                }
                                          

            }
            catch (Exception ex)
            {
                creativeResponse.Message = GestionWeb.GetWebWord(959, _customerWebSession.SiteLanguage);
            }

            return creativeResponse;

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

        public List<List<string>> GetPresentVehicles(string idWebSession, string ids, int idUnivers, long moduleId)
        {
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);
            IInsertionsResult creativeResult = InitCreativeCall(_customerWebSession, moduleId);

            List<VehicleInformation> Vehicles = creativeResult.GetPresentVehicles(ids, idUnivers, false);

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

        #region Build Custom Filters
        protected virtual void BuildCustomFilter(ref ResultTable data, List<GenericColumnItemInformation> columnFilters)
        {

            if (columnFilters == null || columnFilters.Count <= 0)
            {
                return;
            }

            #region Filter data and build available filters
            string value = string.Empty;
            string[] values = null;
            var sep = new char[1] { ',' };
            bool match = true;

            //Set available filters
            for (int i = 0; i < data.LinesNumber; i++)
            {
                foreach (GenericColumnItemInformation g in columnFilters)
                {

                    value = ((CellCreativesInformation)data[i, 1]).GetValue(g);
                    values = value.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < values.Length; j++)
                    {
                        if (!_availableFilterValues[g.Id].Contains(values[j]))
                        {
                            _availableFilterValues[g.Id].Add(values[j]);
                        }
                    }


                }
            }
            foreach (GenericColumnItemInformation g in columnFilters)
            {
                if (_customFilterValues.ContainsKey(g.Id) && _customFilterValues[g.Id].Count > 0)
                {
                    foreach (string s in _customFilterValues[g.Id])
                    {
                        if (!_availableFilterValues[g.Id].Contains(s))
                        {
                            _availableFilterValues[g.Id].Add(s);
                        }

                    }
                }
            }

            //Check custom filters
            int doNotMatch = 0;
            for (int i = 0; i < data.LinesNumber; i++)
            {
                match = true;
                foreach (GenericColumnItemInformation g in columnFilters)
                {

                    value = ((CellCreativesInformation)data[i, 1]).GetValue(g);
                    values = value.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < values.Length; j++)
                    {
                        if (_availableFilterValues[g.Id].Contains(values[j]) && _customFilterValues.ContainsKey(g.Id) && _customFilterValues[g.Id].Count > 0 && !_customFilterValues[g.Id].Contains(values[j]))
                        {
                            match = false;
                        }
                    }


                }
                if (!match)
                {
                    data.SetLineStart(new LineHide(data.GetLineStart(i).LineType), i);
                    doNotMatch++;
                }

            }
            if (doNotMatch == data.LinesNumber)
            {
                data = new ResultTable(1, 1);
                data.AddNewLine(LineType.total);
                var c = new CellLabel(GestionWeb.GetWebWord(2543, _customerWebSession.SiteLanguage));
                c.CssClass = "error";
                //this.CssLTotal = "error";//COMMENTE PAR DD
                data[0, 1] = c;
            }
            #endregion

            #region Build Filters Html Code
            string themeName = WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name;
            var str = new StringBuilder();
            string ID = "tt";
            str.AppendFormat("<tr id=\"filters_{0}\" style=\"display:none;\" ><td colSpan=\"2\"><table>", ID);
            bool checke = false;
            foreach (GenericColumnItemInformation c in columnFilters)
            {
                if (_availableFilterValues.ContainsKey(c.Id) && _availableFilterValues[c.Id].Count > 0)
                {
                    str.AppendFormat("<tr><th colSpan=\"4\">{0}</th></tr>", GestionWeb.GetWebWord(c.WebTextId, _customerWebSession.SiteLanguage));
                    _availableFilterValues[c.Id].Sort();
                    for (int i = 0; i < _availableFilterValues[c.Id].Count; i++)
                    {
                        if ((i % 4) == 0)
                        {
                            if (i > 0)
                            {
                                str.Append("</tr>");
                            }
                            str.Append("<tr>");
                        }
                        checke = checke || (_customFilterValues.ContainsKey(c.Id) && _customFilterValues[c.Id].Contains(_availableFilterValues[c.Id][i]));
                        str.AppendFormat("<td><input {4} id=\"{2}_{0}_{3}\" type=\"checkbox\" onclick=\"if(this.checked){{AddFilter({0},'{1}');}}else {{RemoveFilter({0},'{1}');}};\" ><label for=\"{2}_{0}_{3}\">{1}</label></td>"
                            , c.Id.GetHashCode(), _availableFilterValues[c.Id][i], ID, i
                            , (_customFilterValues.ContainsKey(c.Id) && _customFilterValues[c.Id].Contains(_availableFilterValues[c.Id][i])) ? "checked" : string.Empty);

                    }
                }
            }
            str.AppendFormat("<tr><td colSpan=\"4\">&nbsp;</td></tr><tr><td colSpan=\"4\" align=\"right\"><a href=\"#\" onclick=\"ApplyFilters();\" onmouseover=\"filterButton_{0}.src='/App_Themes/{1}/Images/Common/Button/appliquer_down.gif';\" onmouseout=\"filterButton_{0}.src = '/App_Themes/{1}/Images/Common/Button/appliquer_up.gif';\"><img src=\"/App_Themes/{1}/Images/Common/Button/appliquer_up.gif\" border=0 name=filterButton_{0}></a>", ID, themeName);
            str.AppendFormat("&nbsp;&nbsp;&nbsp;<a href=\"#\" onclick=\"{{InitFilters();ApplyFilters();}}\" onmouseover=\"filterInitButton_{0}.src='/App_Themes/{1}/Images/Common/Button/initialize_all_down.gif';\" onmouseout=\"filterInitButton_{0}.src = '/App_Themes/{1}/Images/Common/Button/initialize_all_up.gif';\"><img src=\"/App_Themes/{1}/Images/Common/Button/initialize_all_up.gif\" border=0 name=filterInitButton_{0}></a></td></tr>", ID, themeName);
            str.Append("</table></td></tr></table><br>");

            str.Insert(0, string.Format("<table class=\"creativeFilterBox\" border=\"0\" cellSpacing=\"0\" cellPadding=\"0\"><tr style=\"cursor:hand;\" onclick=\"if(filters_{1}.style.display == 'none'){{filters_{1}.style.display = '';}}else {{filters_{1}.style.display = 'none';}};\"><td class=\"{2}\">{0}</td><td align=\"right\" class=\"arrowBackGround\"></td></tr>", GestionWeb.GetWebWord(518, _customerWebSession.SiteLanguage), ID, checke ? "pinkTextColor" : string.Empty));


            _optionHtml = str.ToString();
            #endregion
        }
        #endregion

    }
}
