#region Information
// Author: Y. Rkaian & D. Mussuma
// Creation date: 17/03/2007
// Modification date:
#endregion

using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Reflection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using FrameWorkConstantes = TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using CustomerConstantes=TNS.AdExpress.Constantes.Customer;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Web.Navigation;
using WebCst=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Result;
using WebFunctions = TNS.AdExpress.Web.Functions;

using TNS.AdExpress.Domain.Level;
using DBCst=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;

using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpressI.Portofolio {
    /// <summary>
    /// Portofolio Results
    /// </summary>
    public abstract class PortofolioResults:IPortofolioResults {

        #region Constantes
        protected const long TOTAL_LINE_INDEX=0;
        protected const long DETAILED_PORTOFOLIO_EURO_COLUMN_INDEX=2;
        protected const long DETAILED_PORTOFOLIO_INSERTION_COLUMN_INDEX=3;
        protected const long DETAILED_PORTOFOLIO_DURATION_COLUMN_INDEX=4;
        protected const long DETAILED_PORTOFOLIO_MMC_COLUMN_INDEX=4;
        protected const long DETAILED_PORTOFOLIO_PAGE_COLUMN_INDEX=5;
		//protected const int PROD_COL = 1164;
		//protected const int PM_COL = 751;
		//protected const int INSERTIONS_LIST_COL = 2245;
		//protected const int CREATIVES_COL = 1994;
		
		//protected const int EUROS_COL = 1423;
		//protected const int MM_COL = 1424;
		//protected const int SPOTS_COL = 939;
		//protected const int INSERTIONS_COL = 940;
		//protected const int PAGE_COL =943;
		//protected const int PAN_COL = 1604;
		//protected const int DURATION_COL = 1435;
		//protected const int VOLUME = 2216;
		//protected const int TOTAL_COL = 1401;
		//protected const int POURCENTAGE_COL = 1236;
        #endregion

        #region Variables
        /// <summary>
        /// Customer session
        /// </summary>
        protected WebSession _webSession;
        /// <summary>
        /// Vehicle
        /// </summary>
		protected VehicleInformation _vehicleInformation;
        /// <summary>
        /// Media Id
        /// </summary>
        protected Int64 _idMedia;
        /// <summary>
        /// Date begin
        /// </summary>
        protected string _periodBeginning;
        /// <summary>
        /// Date end
        /// </summary>
        protected string _periodEnd;
        /// <summary>
        /// Current Module
        /// </summary>
        protected TNS.AdExpress.Domain.Web.Navigation.Module _module;
        /// <summary>
        /// Show creations in the result
        /// </summary>
        protected bool _showCreatives;
        /// <summary>
        /// Show insertions in the result
        /// </summary>
        protected bool _showInsertions;		
		/// <summary>
		/// Screen code
		/// </summary>
		protected string _adBreak;
		/// <summary>
		/// Day of Week
		/// </summary>
		protected string _dayOfWeek;
		///// <summary>
		///// Date parution
		///// </summary>
		//protected string _dateParution;
		///// <summary>
		///// Date cover
		///// </summary>
		//protected string _dateCover;
		///// <summary>
		///// Name media
		///// </summary>
		//protected string _nameMedia;
		///// <summary>
		///// Nb pages
		///// </summary>
		//protected string _nbrePages;
		///// <summary>
		///// Page anchor
		///// </summary>
		//protected string _pageAnchor;

		///// <summary>
		///// File name 
		///// </summary>
		//protected string _fileName1;
		///// <summary>
		/////  File name 
		///// </summary>
		//protected string _fileName2;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
		public PortofolioResults(WebSession webSession) {
            if(webSession==null) throw (new ArgumentNullException("Customer session is null"));
            _webSession=webSession;
            try {
                // Set Vehicle
				_vehicleInformation = GetVehicleInformation();
                //Set Media Id
                _idMedia = GetMediaId();
                // Period
                _periodBeginning = GetDateBegin();
                _periodEnd = GetDateEnd();
                // Module
                _module=ModulesList.GetModule(webSession.CurrentModule);
                _showCreatives=ShowCreatives();
                _showInsertions=ShowInsertions();
            }
            catch(System.Exception err) {
                throw (new PortofolioException("Impossible to set parameters",err));
            }
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="adBreak">Screen code</param>
		/// <param name="dayOfWeek">Day of week</param>
		public PortofolioResults(WebSession webSession, string adBreak, string dayOfWeek)
			: this(webSession) {
			_adBreak = adBreak;
			_dayOfWeek = dayOfWeek;
		}
		///// <summary>
		///// Constructor
		///// </summary>
		///// <param name="webSession">Customer Session</param>
		///// <param name="dateParution">date Parution</param>
		///// <param name="dateCover">Date cover</param>
		/////<param name="nameMedia">Name media</param>
		/////<param name="nbrePages">nb pages</param>
		/////<param name="pageAnchor">Page anchor</param>
		//public PortofolioResults(WebSession webSession, string dateParution, string dateCover, string nameMedia, string nbrePages, string pageAnchor)
		//    : this(webSession) {
		//    _dateParution = dateParution;
		//    _dateCover = dateCover;
		//    _nameMedia = nameMedia;
		//    _nbrePages = nbrePages;
		//    _pageAnchor = pageAnchor;
		//}
		///// <summary>
		///// Constructor
		///// </summary>
		///// <param name="webSession">Customer Session</param>
		///// <param name="dateCover">Date cover</param>
		/////<param name="fileName1">file Name </param>
		/////<param name="fileName2">nb pages</param>
		/////<param name="pageAnchor">file Name</param>
		//public PortofolioResults(long idMedia, string dateCover, string fileName1, string fileName2)
		// {
		//    _idMedia = idMedia;
		//    _dateCover = dateCover;
		//    _fileName1 = fileName1;
		//    _fileName2 = fileName2;
		//}
        #endregion

		#region Implementation of abstract methods
		/// <summary>
		/// Get ResultTable for some portofolio result
		///  - DETAIL_PORTOFOLIO
		///  - CALENDAR
		///  - SYNTHESIS (only result table)
		/// </summary>
		/// <param name="webSession">Customer session</param>
		/// <returns>Result Table</returns>
		public virtual ResultTable GetResultTable() {
			Engines.Engine result = null;
			try {
				switch (_webSession.CurrentTab) {
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
						result = new Engines.PortofolioDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, _showInsertions, _showCreatives);
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
						result = new Engines.CalendarEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
						result = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
						break;
					default:
						throw (new PortofolioException("Impossible to identified current tab "));
				}
			}
			catch (System.Exception err) {
				throw (new PortofolioException("Impossible to compute portofolio results", err));
			}

			return result.GetResultTable();
		}

		/// <summary>
		/// Get view of the vehicle (HTML)
		/// </summary>
		/// <param name="excel">True for excel result</param>
		/// <returns>HTML code</returns>
		public virtual string GetVehicleViewHtml(bool excel) {
			Engines.SynthesisEngine result = null;
			result = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
			return result.GetVehicleViewHtml(excel);
		}
		

		//#region Chemin de fer popUp
		///// <summary>
		/////  Show all pages of a magazine
		///// </summary>		
		///// <returns>Code Html</returns>
		//public virtual string GetPortofolioCreationMedia() {

		//    #region Variables
		//    string pathWeb = WebCst.CreationServerPathes.IMAGES + "/" + _idMedia + "/" + _dateCover + "/imagette/";
		//    string path = WebCst.CreationServerPathes.LOCAL_PATH_IMAGE + _idMedia + @"\" + _dateCover + @"\imagette";

		//    // Pour test en localhost :
		//    //string path="\\\\localhost\\ImagesPresse\\"+idMedia+"\\"+date+"\\imagette";

		//    string[] files = Directory.GetFiles(path, "*.jpg");
		//    string[] endFile;
		//    StringBuilder t = new StringBuilder(5000);
		//    int i = 1;
		//    int compteur = 0;
		//    string endBalise = "";
		//    string day;
		//    string[] filesName = new string[2];
		//    #endregion
		//    CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Localization);
		//    DateTime dayDT = new DateTime(int.Parse(_dateParution.Substring(0, 4)), int.Parse(_dateParution.Substring(4, 2)), int.Parse(_dateParution.ToString().Substring(6, 2)));
		//    day = DayString.GetCharacters(dayDT, cultureInfo) + " " + dayDT.ToString("dd/MM/yyyy");

		//    t.Append("<table border=1 class=\"violetBorder paleVioletBackGroundV2\" cellpadding=0 cellspacing=0 width=100% ><tr>");
		//    t.Append("<td class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-BOTTOM-STYLE: none\">" + day + "</td>");
		//    t.Append("<td align=center class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">" + _nameMedia + "</td>");
		//    t.Append("<td align=right class=\"portofolio1\" style=\"BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">" + GestionWeb.GetWebWord(1385, _webSession.SiteLanguage) + " : " + _nbrePages + "</td>");
		//    t.Append("</tr></table>");

		//    t.Append("<table border=0 cellpadding=0 cellspacing=0 width=100% class=\"paleVioletBackGroundV2\">");
		//    foreach (string name in files) {

		//        endFile = name.Split('\\');
		//        // Couverture - Dos
		//        if (i == 1 || i == files.Length) {
		//            t.Append("<tr><td colspan=4 align=center>");
		//            t.Append("<table border=1 class=\"violetBorder\" cellpadding=0 cellspacing=0 width=100%><tr><td align=center>");
		//            if (i == 1) t.Append("<a name=\"C1\"></a><a name=\"C2\"></a>");
		//            if (i == files.Length) t.Append("<a name=\"C3\"></a><a name=\"C4\"></a>");
		//            t.Append("<a href=\"javascript:portofolioOneCreation('" + _idMedia + "','" + _dateCover + "','" + endFile[endFile.Length - 1] + "','');\"><img src='" + pathWeb + endFile[endFile.Length - 1] + "' border=\"0\"></a>");
		//            t.Append("</td></tr></table>");
		//            t.Append("</td></tr>");
		//        }
		//        else {
		//            if (compteur == 0) {
		//                t.Append("<tr>");
		//                endBalise = "";
		//            }
		//            else if (compteur == 3) {
		//                compteur = -1;
		//                endBalise = "</tr>";

		//            }
		//            else {
		//                endBalise = "";
		//            }

		//            t.Append("<td align=center style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">");
		//            // Tableau niveau 2
		//            if (compteur == 0 || compteur == 2) {
		//                t.Append("<table border=1 class=\"violetBorder\" cellpadding=0 cellspacing=0 width=100%><tr><td style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">");
		//                filesName[0] = endFile[endFile.Length - 1];
		//                filesName[1] = files[i].Split('\\')[endFile.Length - 1];
		//            }
		//            // Tableau niveau 1
		//            t.Append("<table border=0 cellpadding=0 cellspacing=0 width=100%><tr><td>");
		//            t.Append("<a name=\"#" + i.ToString() + "\" href=\"javascript:portofolioOneCreation('" + _idMedia + "','" + _dateCover + "','" + filesName[0] + "','" + filesName[1] + "');\"><img src='" + pathWeb + endFile[endFile.Length - 1] + "' border=\"0\"></a>");
		//            t.Append("</td></tr>");
		//            t.Append("</table>");

		//            if (compteur == 1 || compteur == -1) {
		//                t.Append("<tr ><td colspan=2 align=center class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">Pages : " + ((int)(i - 1)).ToString() + "/" + i.ToString() + "</td></tr>");
		//                t.Append("</td></tr></table>");
		//            }

		//            t.Append("</td>");
		//            t.Append(endBalise);
		//            compteur++;
		//        }
		//        i++;
		//    }
		//    t.Append("</table>");

		//    // Script location
		//    t.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
		//    t.Append("\ndocument.location='#" + _pageAnchor + "';");
		//    t.Append("\n</script>");

		//    return t.ToString();
		//}
		//#endregion

		///// <summary>
		///// Show one page of a magazine
		///// </summary>
		///// <param name="dateCover">date cover</param>
		///// <param name="idMedia">id média</param>
		/////<param name="fileName1">file name</param>
		/////<param name="fileName2">file name</param>
		///// <returns>page d'un support</returns>
		//public virtual string GetPortofolioOneCreationMedia() {
		//    string pathWeb1 = WebCst.CreationServerPathes.IMAGES + "/" + _idMedia + "/" + _dateCover + "/" + _fileName1 + "";
		//    string pathWeb2 = WebCst.CreationServerPathes.IMAGES + "/" + _idMedia + "/" + _dateCover + "/" + _fileName2 + "";
		//    StringBuilder t = new StringBuilder(3000);

		//    t.Append("<table><tr><td>");
		//    t.Append("<img src='" + pathWeb1 + "' border=\"0\" width=470 height=627>");
		//    if (_fileName2.Length > 0)
		//        t.Append("<img src='" + pathWeb2 + "' border=\"0\" width=470 height=627>");

		//    t.Append("</td></tr></table>");

		//    return t.ToString();
		//}

		#endregion

		#region Structure
		/// <summary>
		/// Get Structure html result
		/// </summary>
		/// <param name="excel">True if export excel</param>
		/// <returns>html code</returns>
		public virtual string GetStructureHtml(bool excel) {
			Engines.StructureEngine result = null;
			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					List<FrameWorkResultConstantes.PortofolioStructure.Ventilation> ventilationTypeList = GetVentilationTypeList();					
					result = new TNS.AdExpressI.Portofolio.Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, ventilationTypeList,excel);
					break;
				case DBClassificationConstantes.Vehicles.names.radio:
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					Dictionary<string, double> hourBeginningList = new Dictionary<string,double>();
					Dictionary<string, double> hourEndList = new Dictionary<string, double>();
					GetHourIntevalList(hourBeginningList, hourEndList);
					result = new TNS.AdExpressI.Portofolio.Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, hourBeginningList, hourEndList, excel);									
					break;
				default:
					throw new PortofolioException("Vehicle unknown.");
			}
			return result.GetHtmlResult();
		}


		/// <summary>
		/// Get structure chart data
		/// </summary>
		/// <returns></returns>
		public virtual DataTable GetStructureChartData() {
			Engines.StructureEngine result = null;

			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					List<FrameWorkResultConstantes.PortofolioStructure.Ventilation> ventilationTypeList = GetVentilationTypeList();
					result = new TNS.AdExpressI.Portofolio.Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, ventilationTypeList, false);
					break;
				case DBClassificationConstantes.Vehicles.names.radio:
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					Dictionary<string, double> hourBeginningList = new Dictionary<string, double>();
					Dictionary<string, double> hourEndList = new Dictionary<string, double>();
					GetHourIntevalList(hourBeginningList, hourEndList);
					result = new TNS.AdExpressI.Portofolio.Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, hourBeginningList, hourEndList, false);
					break;
				default:
					throw new PortofolioException("Vehicle unknown.");
			}
			return result.GetChartData();
		}
		#endregion

		#region HTML detail media
		/// <summary>
		/// Get media detail html
		/// </summary>
		/// <param name="excel">true for excel result</param>
		/// <returns>HTML Code</returns>
		public virtual string GetDetailMediaHtml(bool excel) {
			Engines.MediaDetailEngine result = null;
			StringBuilder t = new StringBuilder(5000);

			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					result = new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
					result.GetAllPeriodInsertions(t, GestionWeb.GetWebWord(1837, _webSession.SiteLanguage));
					t.Append(result.GetVehicleViewHtml(excel));
					return t.ToString();
				case DBClassificationConstantes.Vehicles.names.radio:
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					result = new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, excel);
					return result.GetHtmlResult();
				default:
					throw new PortofolioException("Vehicle unknown.");
			}
		}
		#endregion

		#region Insetion detail
		/// <summary>
		/// Get media insertion detail
		/// </summary>
		/// <returns></returns>
		public virtual ResultTable GetInsertionDetailResultTable() {
			Engines.InsertionDetailEngine result = new Engines.InsertionDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, _adBreak, _dayOfWeek);
			return result.GetResultTable();
		}
		#endregion

		#region IResult Membres

		#region HTML for:SYNTHESIS, NOVELTY, DETAIL MEDIA, PERFORMANCES
		/// <summary>
        /// Get HTML code for some portofolio result
        ///  - SYNTHESIS
        ///  - NOVELTY
        ///  - DETAIL_MEDIA
        ///  - PERFORMANCES
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="webSession">Customer session</param>
        /// <returns>HTML Code</returns>
        public string GetHtml(Page page) {
            throw new Exception("The method or operation is not implemented.");
        }
		
        #endregion
      
        #endregion

        #region Methods
  
        #region Dates
        /// <summary>
        /// Get begin date for the 2 module types
        /// - Portofolio Alert
        /// - Portofolio analysis
        /// </summary>
        /// <returns>Begin date</returns>
        protected string GetDateBegin() {
            switch(_webSession.CurrentModule) {
                case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PORTEFEUILLE:
                    return (Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate,_webSession.PeriodType).ToString("yyyyMMdd"));
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:
                    return (_webSession.PeriodBeginningDate);
            }
            return (null);
        }

        /// <summary>
        /// Get ending date for the 2 module types
        /// </summary>
        /// - Portofolio Alert
        /// - Portofolio analysis
        /// <returns>Ending date</returns>
        protected string GetDateEnd() {
            switch(_webSession.CurrentModule) {
                case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PORTEFEUILLE:
                    return (Dates.getPeriodEndDate(_webSession.PeriodEndDate,_webSession.PeriodType).ToString("yyyyMMdd"));
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:
                    return (_webSession.PeriodEndDate);
            }
            return (null);
        }
        #endregion

        #region Vehicle Selection
        /// <summary>
        /// Get Vehicle Selection
        /// </summary>
        /// <returns>Vehicle label</returns>
        protected string GetVehicle() {
            string vehicleSelection=_webSession.GetSelection(_webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
            if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw (new PortofolioException("The media selection is invalid"));
            return (vehicleSelection);
        }
        /// <summary>
        /// Get vehicle selection
        /// </summary>
        /// <returns>Vehicle</returns>
        protected VehicleInformation GetVehicleInformation() {
            try {
                return (VehiclesInformation.Get(Int64.Parse(GetVehicle())));
            }
            catch(System.Exception err) {
                throw (new PortofolioException("Impossible to retreive vehicle selection",err));
            }
        }
        #endregion

        #region Media Selection
        /// <summary>
        /// Get Media Id
        /// </summary>
        /// <returns>Media Id</returns>
        protected Int64 GetMediaId() {
            try {
                return (((LevelInformation)_webSession.ReferenceUniversMedia.FirstNode.Tag).ID);
            }
            catch (System.Exception err) {
                throw (new PortofolioException("Impossible to retrieve media id"));
            }
        }
        #endregion

        #region Insertion and Creations
        /// <summary>
        /// Determine if the result shows the insertion column
        /// </summary>
        /// <returns>True if the Insertion column is shown</returns>
        protected virtual bool ShowInsertions() {
			if (!_webSession.CustomerPeriodSelected.Is4M || !_vehicleInformation.ShowInsertions) return (false);
            foreach(DetailLevelItemInformation item in _webSession.GenericProductDetailLevel.Levels) {
                if(item.Id.Equals(DetailLevelItemInformation.Levels.advertiser)
					|| item.Id.Equals(DetailLevelItemInformation.Levels.product)) {
                    return (true);
                }
            }
            return (false);
        }
        /// <summary>
        /// Determine if the result shows the creation column
        /// </summary>
        /// <returns>True if the creation column is shown</returns>
        protected virtual bool ShowCreatives() {
			if (!_webSession.CustomerPeriodSelected.Is4M ||
				!_webSession.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_SLOGAN_ACCESS_FLAG) ||
				!_vehicleInformation.ShowCreations ||
                !_webSession.CustomerLogin.ShowCreatives(_vehicleInformation.Id)) return (false);

            foreach(DetailLevelItemInformation item in _webSession.GenericProductDetailLevel.Levels) {
				if (item.Id.Equals(DetailLevelItemInformation.Levels.advertiser)
					|| item.Id.Equals(DetailLevelItemInformation.Levels.product)) {
                    return (true);
                }
            }
            return (false);
        }
        #endregion              		


		/// <summary>
		/// Vérifie qu'une datarow est vide
		/// </summary>
		/// <param name="ds">dataset</param>
		/// <returns>vrai si non vide</returns>
		protected virtual bool IsRowNull(DataSet ds) {
			if (ds != null && ds.Tables[0].Rows.Count > 0) {
				foreach (DataRow dr in ds.Tables[0].Rows) {
					return (dr["euros"] != System.DBNull.Value && dr["spot"] != System.DBNull.Value && dr["duration"] != System.DBNull.Value);
				}
			}
			return false;
		}

		/// <summary>
		/// Get ventilation type list
		/// </summary>
		/// <returns>ventilation type list</returns>
		protected virtual List<FrameWorkResultConstantes.PortofolioStructure.Ventilation> GetVentilationTypeList() {
			List<FrameWorkResultConstantes.PortofolioStructure.Ventilation> ventilationTypeList = new List<TNS.AdExpress.Constantes.FrameWork.Results.PortofolioStructure.Ventilation>();
			ventilationTypeList.Add(FrameWorkResultConstantes.PortofolioStructure.Ventilation.format);
			ventilationTypeList.Add(FrameWorkResultConstantes.PortofolioStructure.Ventilation.color);
			ventilationTypeList.Add(FrameWorkResultConstantes.PortofolioStructure.Ventilation.location);
			ventilationTypeList.Add(FrameWorkResultConstantes.PortofolioStructure.Ventilation.insert);
			return ventilationTypeList;
		}
		/// <summary>
		/// Get hour interval list
		/// </summary>
		/// <param name="hourBeginningList">hour begininng list</param>
		/// <param name="hourEndList">hour end list</param>
		protected virtual void GetHourIntevalList(Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList) {
			if (hourBeginningList == null) hourBeginningList = new Dictionary<string, double>();
			if (hourEndList == null) hourEndList = new Dictionary<string, double>();

			switch (_vehicleInformation.Id) {				
				case DBClassificationConstantes.Vehicles.names.radio:					
					hourBeginningList.Add("0507", 50000); hourEndList.Add("0507", 70000);
					hourBeginningList.Add("0709", 70000); hourEndList.Add("0709", 90000);
					hourBeginningList.Add("0913", 90000); hourEndList.Add("0913", 130000);
					hourBeginningList.Add("1319", 130000); hourEndList.Add("1319", 190000);
					hourBeginningList.Add("1924", 190000); hourEndList.Add("1924", 240000);
					break;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					hourBeginningList.Add("0007",0); hourEndList.Add("0007", 70000);
					hourBeginningList.Add("0712", 70000); hourEndList.Add("0712", 120000);
					hourBeginningList.Add("1214", 120000); hourEndList.Add("1214", 140000);
					hourBeginningList.Add("1417", 140000); hourEndList.Add("1417", 170000);
					hourBeginningList.Add("1719", 170000); hourEndList.Add("1719", 190000);
					hourBeginningList.Add("1922", 190000); hourEndList.Add("1922", 220000);
					hourBeginningList.Add("2224", 220000); hourEndList.Add("2224", 240000);
					break;
				default:
					throw new PortofolioException("GetHourIntevalList(): Vehicle unknown.");
			}
		}
		#endregion

		
	}
}
