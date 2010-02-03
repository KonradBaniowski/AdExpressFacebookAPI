#region Information
// Author: Y. R'kaina
// Creation date: 25/11/2008
// Modification date:
#endregion

#region Using
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using AbstractResult = TNS.AdExpressI.Portofolio.Engines;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.FrameWork.Results;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Translation;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
#endregion

namespace TNS.AdExpressI.Portofolio.Poland.Engines {
    /// <summary>
    /// Poland synthesis engine
    /// </summary>
    public class SynthesisEngine : AbstractResult.SynthesisEngine {

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
            string adNumber = "";
            string nbrSpot = "";
            string nbrEcran = "";
            decimal averageDurationEcran = 0;
            string totalDuration = "";
            string numberProduct = "", numberAdvertiser = "";
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

            #region AlertModule
            bool isAlertModule = _webSession.CustomerPeriodSelected.Is4M;
            if (isAlertModule == false) {
                DateTime DateBegin = WebFunctions.Dates.getPeriodBeginningDate(_periodBeginning, _webSession.PeriodType);
                if (DateBegin > DateTime.Now)
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
            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.mediaSeller)) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.mediaSeller);
                dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
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
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                  || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine)
            {
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
            else if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.spot) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.spot].Id.ToString()))
                nbrSpot = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.spot].Id.ToString()].ToString();
            else if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.occurence) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.occurence].Id.ToString()))
                nbrSpot = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.occurence].Id.ToString()].ToString();

            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.pages) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.pages].Id.ToString())) adNumber = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.pages].Id.ToString()].ToString();
            if (_vehicleInformation.AllowedUnitEnumList.Contains(WebCst.CustomerSessions.Unit.duration) && dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].Id.ToString())) totalDuration = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].Id.ToString()].ToString();
            #endregion

            #region Period selected
            if (isAlertModule) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.periodSelected);
                dt = ds.Tables[0];
                if (dt.Columns.Contains("first_date")) firstDate = dt.Rows[0]["first_date"].ToString();
                if (dt.Columns.Contains("last_date")) lastDate = dt.Rows[0]["last_date"].ToString();
            }
            #endregion

            #region Encart Data
            if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv)
                && isAlertModule) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberAdBreaks);
                dt = ds.Tables[0];
                nbrEcran = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString();
                if (nbrEcran.Length > 0) {
                    if (dt.Rows[0]["ecran_duration"] != System.DBNull.Value) averageDurationEcran = decimal.Parse(dt.Rows[0]["ecran_duration"].ToString()) / decimal.Parse(dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.insertion].Id.ToString()].ToString());
                }
            }
            #endregion

            #region Number of product
            ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberProduct);
            dt = ds.Tables[0];
            numberProduct = dt.Rows[0]["nbLines"].ToString();
            #endregion

            #region Number of advertiser
            ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.numberAdvertiser);
            dt = ds.Tables[0];
            numberAdvertiser = dt.Rows[0]["nbLines"].ToString();
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
            if (isAlertModule) {
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
            Int32 nbLines = 0;
            Int32 lineIndex = 0;
            switch (_vehicleInformation.Id) {
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.newspaper:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.magazine:
                    nbLines = 9;
                    if (adNumber != null && adNumber.Length > 0) nbLines = nbLines + 2;
                    if (investment != null && investment.Length > 0) nbLines++;
                    if (isAlertModule) nbLines = nbLines + 2; // nbLines = 16;
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
                    nbLines = 10;
                    if (isAlertModule) nbLines = nbLines + 5;
                    if (investment != null && investment.Length > 0) nbLines++; // nbLines = 16;
                    break;

				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.outdoor:
					nbLines = 9;					
					if (investment != null && investment.Length > 0) nbLines++;
					break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internet:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.adnettrack:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile:
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
            if (!isAlertModule || (firstDate.Length > 0 && lastDate.Length > 0 && isAlertModule)) {
                // Period selected
                lineIndex = resultTable.AddNewLine(lineType);
                if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                      || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine)
                    && isAlertModule)
                    resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1381, _webSession.SiteLanguage));
                else resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1541, _webSession.SiteLanguage));
                if ((firstDate != null && firstDate.Length > 0 && lastDate != null && lastDate.Length > 0 && firstDate.Equals(lastDate) && isAlertModule)
                    || (dtLastDate.CompareTo(dtFirstDate) == 0 && !isAlertModule)) {
                    resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(Dates.DateToString(dtFirstDate, _webSession.SiteLanguage));
                }
                else {
                    resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(896, _webSession.SiteLanguage) + " " + Dates.DateToString(dtFirstDate, _webSession.SiteLanguage) + " " + GestionWeb.GetWebWord(1730, _webSession.SiteLanguage) + " " + Dates.DateToString(dtLastDate, _webSession.SiteLanguage));
                }

                ChangeLineType(ref lineType);
            }

            // Periodicity
            if (periodicity.Length > 0) {
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
            if (regie.Length > 0) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1417, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(regie);
                ChangeLineType(ref lineType);
            }

            // Interest center
            if (interestCenter.Length > 0) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1411, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(interestCenter);
                ChangeLineType(ref lineType);
            }

            // Case vehicle press
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                  || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine)
            {
                
                if (adNumber != null && adNumber.Length > 0) {
                    // Nb de page pub		
                    lineIndex = resultTable.AddNewLine(lineType);
                    resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1386, _webSession.SiteLanguage));
                    CellPage cP = new CellPage(double.Parse(adNumber));
                    cP.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.pages).StringFormat;
                    resultTable[lineIndex, SECOND_COLUMN_INDEX] = cP;
                    ChangeLineType(ref lineType);
                }
             }

            // Cas tv, radio
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv) {

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

            // Total investissements
            if (investment != null && investment.Length > 0) {
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

            //Nombre d'annonceurs
            lineIndex = resultTable.AddNewLine(lineType);
            resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1396, _webSession.SiteLanguage));
            CellNumber cN9 = new CellNumber(double.Parse(numberAdvertiser));
            cN9.StringFormat = unitFormat;
            resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN9;
            ChangeLineType(ref lineType);

            // Cas tv, radio
            if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv)
                && isAlertModule) {

                // Durée moyenne d'un écran
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1414, _webSession.SiteLanguage));
                CellDuration cD1 = new CellDuration(Convert.ToDouble(((long)averageDurationEcran).ToString()));
                cD1.StringFormat = UnitsInformation.Get(WebCst.CustomerSessions.Unit.euro).StringFormat;
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = cD1;
                ChangeLineType(ref lineType);

            }
            #endregion

            return resultTable;

        }
        #endregion

    }
}
