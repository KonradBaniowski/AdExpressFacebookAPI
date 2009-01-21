#region Information
// Author: D. Mussuma
// Creation date: 08/08/2008
// Modification date:
#endregion

using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.Core.Sessions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Constantes.FrameWork.Results;

using TNS.AdExpress.Domain;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;

using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpressI.Portofolio.Engines {
	/// <summary>
	/// Compute portofolio synthesis' results
	/// </summary>
	public class SynthesisEngine : Engine{

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicle">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public SynthesisEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd) {
		}

		#endregion
		
		#region Public methods

		#region Abstract methods implementation
		/// <summary>
		/// Get Result Table for portofolio synthesis
		/// </summary>
		/// <returns>ResultTable</returns>
		protected override ResultTable ComputeResultTable() {

			#region Constantes
			/// <summary>
			/// Header column index
			/// </summary>
			const int HEADER_COLUMN_INDEX = 0;
			/// <summary>
			/// First column index
			/// </summary>
			const int FIRST_COLUMN_INDEX = 1;
			/// <summary>
			/// Second column index
			/// </summary>
			const int SECOND_COLUMN_INDEX = 2;

			#endregion

			#region Variables
			string investment = "";
			string firstDate = "";
			string lastDate = "";
			string media = "";
			string periodicity = "";
			string category = "";
			string regie = "";
			string interestCenter = "";
			string pageNumber = "", adNumber = "";
            string adNumberIncludingInsets = "", adNumberExcludingInsets="";
			string nbrSpot = "";
			string nbrEcran = "";
			decimal averageDurationEcran = 0;
			decimal nbrSpotByEcran = 0;
			string totalDuration = "";
			string numberBoard = "";
			string volume = "";
            string numberProduct = "", numberNewProductInTracking = "", numberNewProductInVehicle = "", numberAdvertiser = "", numberBanner = "";
			ResultTable resultTable = null;
			LineType lineType = LineType.level1;
			string typeReseauStr = string.Empty;
			string unitFormat = "{0:max0}";
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo;
			#endregion
			
			#region Accès aux tables
			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[5];
			parameters[0] = _webSession;
			parameters[1] = _vehicleInformation;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

            DataSet ds;
            DataTable dt;
            DataTable dtTypeSale = null;

            #region AlertModule
            bool isAlertModule = _webSession.CustomerPeriodSelected.IsSliding4M;
            if (isAlertModule == false) {
                DateTime DateBegin = WebFunctions.Dates.getPeriodBeginningDate(_periodBeginning, _webSession.PeriodType);
                if(DateBegin > DateTime.Now)
                    isAlertModule = true;
            }
            #endregion

            #region Media
            ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.media);
				dt = ds.Tables[0];
				if (dt.Rows.Count > 0)
					media = dt.Rows[0]["media"].ToString();
            #endregion

            #region Category
				if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category)) {
					ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.category);
					dt = ds.Tables[0];
					if (dt.Rows.Count > 0)
						category = dt.Rows[0]["category"].ToString();
				}
            #endregion

            #region Media seller
            if(_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack
				&& _vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.mediaSeller)) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.mediaSeller);
                dt = ds.Tables[0];
                if(dt.Rows.Count > 0)
                    regie = dt.Rows[0]["media_seller"].ToString();
            }
            #endregion

            #region Interest center
			if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.interestCenter)) {
				ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.interestCenter);
				dt = ds.Tables[0];
				if (dt.Rows.Count > 0)
					interestCenter = dt.Rows[0]["interest_center"].ToString();
			}
            #endregion

            #region Periodicity
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press ||
                _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.periodicity);
                dt = ds.Tables[0];
                periodicity = dt.Rows[0]["periodicity"].ToString();
            }
            #endregion

            #region investment
            ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.investment);
            dt = ds.Tables[0];

			if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.euro) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.euro].Id.ToString()) && dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.euro].Id.ToString()].ToString().Length > 0)
                investment = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.euro].Id.ToString()].ToString();
            else
                investment = "0";

			if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.insertion) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString())
                && dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString().Length > 0)
                nbrSpot = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString();
            else if(_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.spot) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.spot].Id.ToString())) 
                nbrSpot = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.spot].Id.ToString()].ToString();
            else if(_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.occurence) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.occurence].Id.ToString())) 
                nbrSpot = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.occurence].Id.ToString()].ToString();

			if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.pages) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.pages].Id.ToString())) adNumber = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.pages].Id.ToString()].ToString();
            if(_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.numberBoard) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.numberBoard].Id.ToString())) numberBoard = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.numberBoard].Id.ToString()].ToString();
            if(_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.duration) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].Id.ToString())) totalDuration = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].Id.ToString()].ToString();
            if(_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.volume) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.volume].Id.ToString())) {
                if(dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.volume].Id.ToString()].ToString().Length > 0) {
                    volume = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.volume].Id.ToString()].ToString();
                }
                else volume = "0";
            }
            #endregion

            #region Period selected
            if (isAlertModule 
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.periodSelected);
                dt = ds.Tables[0];
                if (dt.Columns.Contains("first_date")) firstDate = dt.Rows[0]["first_date"].ToString();
                if (dt.Columns.Contains("last_date")) lastDate = dt.Rows[0]["last_date"].ToString();
            }
            #endregion

            #region ad Number Including Insets
            if (WebApplicationParameters.AllowInsetOption && _vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.pages) && _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.adNumberIncludingInsets);
                dt = ds.Tables[0];
                adNumberIncludingInsets = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.pages].Id.ToString()].ToString();
            }
            #endregion

            #region ad Number Excluding Insets
			if (WebApplicationParameters.AllowInsetOption && _vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.pages) && _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.adNumberExcludingInsets);
                dt = ds.Tables[0];
                adNumberExcludingInsets = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.pages].Id.ToString()].ToString();
            }
            #endregion

            #region Page number
			if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.pages) && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.pageNumber);
                dt = ds.Tables[0];
                pageNumber = dt.Rows[0]["page"].ToString();
                if (pageNumber.Length == 0)
                    pageNumber = "0";
            }
            #endregion

            #region Encart Data
            if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others)
                && isAlertModule) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberAdBreaks);
                dt = ds.Tables[0];
                nbrEcran = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString();
                if (nbrEcran.Length > 0) {
					if (dt.Rows[0]["ecran_duration"] != System.DBNull.Value) averageDurationEcran = decimal.Parse(dt.Rows[0]["ecran_duration"].ToString()) / decimal.Parse(dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString());
                    if(dt.Rows[0]["nbre_spot"] != System.DBNull.Value) nbrSpotByEcran = decimal.Parse(dt.Rows[0]["nbre_spot"].ToString()) / decimal.Parse(dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString());
                }
            }
            #endregion

            #region Number of product
            ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberProduct);
            dt = ds.Tables[0];
            numberProduct = dt.Rows[0]["nbLines"].ToString(); 
            #endregion

            #region Number of product in tracking
            if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.outdoor
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberNewProductInTracking);
                dt = ds.Tables[0];
                numberNewProductInTracking = dt.Rows[0]["nbLines"].ToString();
            }
            #endregion

            #region Number of product in vehicle
            if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.outdoor
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberNewProductInVehicle);
                dt = ds.Tables[0];
                numberNewProductInVehicle = dt.Rows[0]["nbLines"].ToString();
            }
            #endregion

            #region Number of advertiser
            ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberAdvertiser);
            dt = ds.Tables[0];
            numberAdvertiser = dt.Rows[0]["nbLines"].ToString();
            #endregion

            #region Number of banners (Evaliant)
            if(_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberBanners);
                dt = ds.Tables[0];
                CellIdsNumber cell = new CellIdsNumber();
                foreach (DataRow row in dt.Rows)
                {
                    cell.Add(row["hashcode"].ToString().Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                }
                numberBanner = cell.Value.ToString();
            }
            #endregion

            #region Type sale
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.typeSale);
                dtTypeSale = ds.Tables[0];
            }
            #endregion

            #endregion

            #region No data
            if ((investment == null || investment.Length < 1 || investment == "0")
                && (numberProduct == null || numberProduct.Length < 1 || numberProduct == "0")
                && (numberAdvertiser == null || numberAdvertiser.Length < 1 || numberAdvertiser == "0")) {
                return resultTable;
            }
            #endregion

            #region Period
            DateTime dtFirstDate = DateTime.Today;
            DateTime dtLastDate = DateTime.Today;
            if (isAlertModule && (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet)) {
                if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor) {
                    if (firstDate.Length > 0) {
                        dtFirstDate = Convert.ToDateTime(firstDate);
                        dtFirstDate = dtFirstDate.Date;
                    }
                    if (lastDate.Length > 0) {
                        dtLastDate = Convert.ToDateTime(lastDate);
                        dtLastDate = dtLastDate.Date;
                    }
                }
                else {
                    if (firstDate.Length > 0) {
                        dtFirstDate = new DateTime(int.Parse(firstDate.Substring(0, 4)), int.Parse(firstDate.Substring(4, 2)), int.Parse(firstDate.Substring(6, 2)));
                    }

                    if (lastDate.Length > 0) {
                        dtLastDate = new DateTime(int.Parse(lastDate.Substring(0, 4)), int.Parse(lastDate.Substring(4, 2)), int.Parse(lastDate.Substring(6, 2)));
                    }
                }
            }
            else {
                dtFirstDate = WebFunctions.Dates.getPeriodBeginningDate(_periodBeginning, _webSession.PeriodType);
                dtLastDate = WebFunctions.Dates.getPeriodEndDate(_periodEnd, _webSession.PeriodType);
            }
			#endregion

			#region nbLines init
            long nbLines = 0;
            long lineIndex = 0;
            switch (_vehicleInformation.Id) {
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress:
                    nbLines = 9;
                    if (adNumber != null && adNumber.Length > 0) nbLines = nbLines + 2;
					if (WebApplicationParameters.AllowInsetOption && isAlertModule && adNumber != null && adNumber.Length > 0 && adNumberExcludingInsets != null && adNumberExcludingInsets.Length > 0) nbLines++;
					if (WebApplicationParameters.AllowInsetOption && isAlertModule && adNumber != null && adNumber.Length > 0 && adNumberIncludingInsets!=null && adNumberIncludingInsets.Length > 0) nbLines++;
                    if (investment != null && investment.Length > 0) nbLines++;
                    if (isAlertModule) nbLines = nbLines + 2; // nbLines = 16;
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.others:
                    nbLines = 10;
                    if (isAlertModule) nbLines = nbLines + 5;
                    if (investment != null && investment.Length > 0) nbLines++; // nbLines = 16;
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.outdoor:
                    nbLines = 9;
                    if (dtTypeSale != null && dtTypeSale.Rows.Count > 0) {
                        nbLines++;//number board
                        if (isAlertModule) nbLines++;
                    }
                    if (investment != null && investment.Length > 0) nbLines++;//nbLines = 12;
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing:
                    nbLines = 7;
                    if (_webSession.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_VOLUME_MARKETING_DIRECT) && !isAlertModule) nbLines++;
                    if (investment != null && investment.Length > 0) nbLines++;
                    if (isAlertModule) nbLines = nbLines + 2; //nbLines = 11;
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internet:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.adnettrack:
                    nbLines = 8;
                    if (investment != null && investment.Length > 0) nbLines++;
                    break;
                default:
                    throw (new PortofolioException("Vehicle unknown"));
            }
			#endregion

			#region headers
            Headers headers = new Headers();
            TNS.FrameWork.WebResultUI.Header header = new TNS.FrameWork.WebResultUI.Header(media.ToString(), HEADER_COLUMN_INDEX, "SynthesisH1");
            header.Add(new TNS.FrameWork.WebResultUI.Header("", FIRST_COLUMN_INDEX, "SynthesisH2"));
            header.Add(new TNS.FrameWork.WebResultUI.Header("", SECOND_COLUMN_INDEX, "SynthesisH2"));
            headers.Root.Add(header);
            resultTable = new ResultTable(nbLines, headers);
			#endregion

			#region Building resultTable
            if (!isAlertModule
                || (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.directMarketing && _webSession.CustomerPeriodSelected.Is4M)
                || (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internet && _webSession.CustomerPeriodSelected.Is4M)
                || (firstDate.Length > 0 && lastDate.Length > 0 && isAlertModule)) {
                // Date begin and date end for outdooor
                if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor && isAlertModule) {
                    lineIndex = resultTable.AddNewLine(lineType);
                    resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1607, _webSession.SiteLanguage));
                    resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(Dates.DateToString(dtFirstDate, _webSession.SiteLanguage));

                    ChangeLineType(ref lineType);

                    lineIndex = resultTable.AddNewLine(lineType);
                    resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1608, _webSession.SiteLanguage));
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(Dates.DateToString(dtLastDate, _webSession.SiteLanguage));
                }
                // Period selected
                else {
                    //if (firstDate.Length > 0 || !isAlertModule) {
                    lineIndex = resultTable.AddNewLine(lineType);
                    if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) && isAlertModule)
                        resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1381, _webSession.SiteLanguage));
                    else resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1541, _webSession.SiteLanguage));
                    if ((firstDate != null && firstDate.Length > 0 && lastDate != null && lastDate.Length > 0 && firstDate.Equals(lastDate) && isAlertModule)
                        || (dtLastDate.CompareTo(dtFirstDate) == 0 && !isAlertModule)) {
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(Dates.DateToString(dtFirstDate, _webSession.SiteLanguage));
                    }
                    else {
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(896, _webSession.SiteLanguage) + " " + Dates.DateToString(dtFirstDate, _webSession.SiteLanguage) + " " + GestionWeb.GetWebWord(1730, _webSession.SiteLanguage) + " " + Dates.DateToString(dtLastDate, _webSession.SiteLanguage));
                    }
                    //}
                }

                ChangeLineType(ref lineType);
            }

            // Periodicity
            if (periodicity.Length>0) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1450, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(periodicity);
                ChangeLineType(ref lineType);
            }
            // Category
            if (category.Length > 0) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1416, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(category);
                ChangeLineType(ref lineType);
            }

            // Media seller
            if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing && regie.Length>0) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1417, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(regie);
                ChangeLineType(ref lineType);
            }

            //  Volume for Marketing Direct
			if (volume!=null && volume.Length>0 && _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.directMarketing &&
                _webSession.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_VOLUME_MARKETING_DIRECT)) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(2216, _webSession.SiteLanguage));
				CellVolume cV = new CellVolume(double.Parse(volume));
                cV.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.volume).StringFormat;
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = cV;
                ChangeLineType(ref lineType);
            }

            // Interest center
            if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing && interestCenter.Length>0) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1411, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(interestCenter);
                ChangeLineType(ref lineType);
            }

            // Number of banners
            if(_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack && numberBanner.Length > 0) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(2479, _webSession.SiteLanguage));
				CellNumber cN = new CellNumber(double.Parse(numberBanner.ToString()));
				cN.StringFormat = unitFormat;
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN;
                ChangeLineType(ref lineType);
            }

            //number board and newtwork type 
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor) {
                //number board
				if (numberBoard != null && numberBoard.Length > 0) {
					lineIndex = resultTable.AddNewLine(lineType);
					resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1604, _webSession.SiteLanguage));
					CellNumber cN1 = new CellNumber(double.Parse(numberBoard.ToString()));
					cN1.StringFormat = unitFormat;
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN1;
					ChangeLineType(ref lineType);
				}

                if(dtTypeSale != null && dtTypeSale.Rows.Count > 0 && isAlertModule){
                    //Type sale
                    int count = 0;
                    lineIndex = resultTable.AddNewLine(lineType);
                    resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1609, _webSession.SiteLanguage));
                    if (dtTypeSale.Rows.Count == 0) typeReseauStr = "&nbsp;";
                    else {
                        foreach (DataRow row in dtTypeSale.Rows) {
                            if (count > 0) {
                                typeReseauStr += "<BR>";
                            }
                            typeReseauStr += SQLGenerator.SaleTypeOutdoor(row["type_sale"].ToString(), _webSession.SiteLanguage);
                            count++;
                        }
                    }
                    resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(typeReseauStr);
                    ChangeLineType(ref lineType);
                }
                
            }
            // Case vehicle press
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {
				if (pageNumber != null && pageNumber.Length > 0) {
					// Nombre de page
					lineIndex = resultTable.AddNewLine(lineType);
					resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1385, _webSession.SiteLanguage));
					CellNumber cN2 = new CellNumber(double.Parse(pageNumber));
					cN2.StringFormat = unitFormat;
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN2;
					ChangeLineType(ref lineType);
				}

                if (adNumber != null && adNumber.Length > 0) {
                    // Nb de page pub		
                    lineIndex = resultTable.AddNewLine(lineType);
                    resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1386, _webSession.SiteLanguage));
					CellPage cP = new CellPage(double.Parse(adNumber));
					cP.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.pages).StringFormat;
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = cP;
                    ChangeLineType(ref lineType);

                    // Ratio
                    if (pageNumber != null && pageNumber.Length > 0) {
                        lineIndex = resultTable.AddNewLine(lineType);
                        resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1387, _webSession.SiteLanguage));
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellPercent(((double.Parse(adNumber) / double.Parse(pageNumber) * 100) / (double)1000));
                    }
                    else {
                        lineIndex = resultTable.AddNewLine(lineType);
                        resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1387, _webSession.SiteLanguage));
                        resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel("&nbsp;&nbsp;&nbsp;&nbsp;");
                    }
                    ChangeLineType(ref lineType);
                }
                if (isAlertModule && WebApplicationParameters.AllowInsetOption == true) {
					if (adNumberExcludingInsets != null && adNumberExcludingInsets.Length > 0) {
                        // Nombre de page de pub hors encarts
                        if (adNumberExcludingInsets.Length == 0)
                            adNumberExcludingInsets = "0";
                        lineIndex = resultTable.AddNewLine(lineType);
                        resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1388, _webSession.SiteLanguage));
						CellPage cP1 = new CellPage(double.Parse(adNumberExcludingInsets));
                        cP1.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.pages).StringFormat;
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = cP1;
                        ChangeLineType(ref lineType);
                    }
					if (adNumberIncludingInsets != null && adNumberIncludingInsets.Length > 0) {
                        // Nombre de page de pub encarts
                        if (adNumberIncludingInsets.Length == 0) {
                            adNumberIncludingInsets = "0";
                        }
                        lineIndex = resultTable.AddNewLine(lineType);
                        resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1389, _webSession.SiteLanguage));
						CellPage cP2 = new CellPage(double.Parse(adNumberIncludingInsets));
                        cP2.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.pages).StringFormat;
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = cP2;
                        ChangeLineType(ref lineType);
                    }
                }
            }

            // Cas tv, radio
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others) {

                //Nombre de spot
                if (nbrSpot.Length == 0) {
                    nbrSpot = "0";
                }
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1404, _webSession.SiteLanguage));
				CellNumber cN3 = new CellNumber(double.Parse(nbrSpot));
				cN3.StringFormat = unitFormat;
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN3;
                ChangeLineType(ref lineType);

                if (isAlertModule) {
                    // Nombre d'ecran
                    if (nbrEcran.Length == 0) {
                        nbrEcran = "0";
                    }
                    lineIndex = resultTable.AddNewLine(lineType);
                    resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1412, _webSession.SiteLanguage));
					CellNumber cN4 = new CellNumber(double.Parse(nbrEcran));
					cN4.StringFormat = unitFormat;
                    resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN4;
                    ChangeLineType(ref lineType);
                }
                if (totalDuration.Length == 0) {
                    totalDuration = "0";
                }
                // total durée
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1413, _webSession.SiteLanguage));
				CellDuration cD = new CellDuration(double.Parse(totalDuration));
                cD.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.duration).StringFormat;
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = cD;
                ChangeLineType(ref lineType);
            }
            else if(_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack) {
                // Number of insertion (occurrences) Evaliant
                if(nbrSpot.Length == 0) {
                    nbrSpot = "0";
                }
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1398, _webSession.SiteLanguage));
				CellNumber cN5 = new CellNumber(double.Parse(nbrSpot));
				cN5.StringFormat = unitFormat;
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN5;
                ChangeLineType(ref lineType);
            }

            // Total investissements
            if(investment != null && investment.Length > 0 && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1390, _webSession.SiteLanguage));
				CellEuro cE = new CellEuro(double.Parse(investment));
                cE.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.euro).StringFormat;
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = cE;
                ChangeLineType(ref lineType);
            }

            //Nombre de produits
            lineIndex = resultTable.AddNewLine(lineType);
            resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1393, _webSession.SiteLanguage));
			CellNumber cN6 = new CellNumber(double.Parse(numberProduct));
			cN6.StringFormat = unitFormat;
			resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN6;
            ChangeLineType(ref lineType);

            if ((_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.outdoor 
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing 
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.internet
                && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.adnettrack) && isAlertModule) {
                //Nombre de nouveaux produits dans la pige
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1394, _webSession.SiteLanguage));
				CellNumber cN7 = new CellNumber(double.Parse(numberNewProductInTracking));
				cN7.StringFormat = unitFormat;
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN7;
                ChangeLineType(ref lineType);

                //Nombre de nouveaux produits dans le support
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1395, _webSession.SiteLanguage));
				CellNumber cN8 = new CellNumber(double.Parse(numberNewProductInVehicle));
				cN8.StringFormat = unitFormat;
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN8;
                ChangeLineType(ref lineType);
            }
            //Nombre d'annonceurs
            lineIndex = resultTable.AddNewLine(lineType);
            resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1396, _webSession.SiteLanguage));
			CellNumber cN9 = new CellNumber(double.Parse(numberAdvertiser));
			cN9.StringFormat = unitFormat;
            resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN9;
            ChangeLineType(ref lineType);

            // Cas tv, radio, others
            if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others)
                && isAlertModule) {

                // Durée moyenne d'un écran
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1414, _webSession.SiteLanguage));
				CellDuration cD1 = new CellDuration(Convert.ToDouble(((long)averageDurationEcran).ToString()));
                cD1.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.duration).StringFormat;
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = cD1;
                ChangeLineType(ref lineType);

                // Nombre moyen de spots par écran
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1415, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(nbrSpotByEcran.ToString("0.00"));

            }
			#endregion

            return resultTable;

		}
		/// <summary>
		/// Build Html result
		/// </summary>
		/// <returns></returns>
		protected override string BuildHtmlResult() {
			throw new PortofolioException("The method or operation is not implemented.");
		}
		#endregion		

		#endregion

		#region Protected methods

		#region Change line type
		/// <summary>
		/// Change line type
		/// </summary>
		/// <param name="lineType">Line Type</param>
		/// <returns>Line type</returns>
		protected virtual void ChangeLineType(ref LineType lineType) {

			if (lineType == LineType.level1)
				lineType = LineType.level2;
			else
				lineType = LineType.level1;

		}
		#endregion		

		#endregion
	}
}
