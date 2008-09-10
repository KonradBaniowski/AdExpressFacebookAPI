#region Information
// Author: Y. R'kaina
// Creation date: 29/08/2008
// Modification date:
#endregion

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

namespace TNS.AdExpressI.Portofolio.Finland.Engines {

    public class SynthesisEngine : AbstractResult.SynthesisEngine{

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
            string adNumberIncludingInsets = "", adNumberExcludingInsets = "";
            string nbrSpot = "";
            string totalDuration = "";
            string numberBoard = "";
            string numberProduct = "", numberAdvertiser = "";
            ResultTable resultTable = null;
            LineType lineType = LineType.level1;
            string typeReseauStr = string.Empty;
            bool isAlertModule = _webSession.CustomerPeriodSelected.Is4M; //(_webSession.CurrentModule == WebCst.Module.Name.ALERTE_PORTEFEUILLE);			
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
			if( _vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.mediaSeller)){
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

            #region investment
            ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.investment);
            dt = ds.Tables[0];

            if (dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.euro].Id.ToString()].ToString().Length > 0)
                investment = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.euro].Id.ToString()].ToString();
            else
                investment = "0";

            if (dt.Columns.Contains(UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].Id.ToString())) totalDuration = dt.Rows[0][UnitsInformation.List[WebCst.CustomerSessions.Unit.duration].Id.ToString()].ToString();
            #endregion

            #region Period selected
            if (isAlertModule) {
                ds = portofolioDAL.GetSynthisData(PortofolioSynthesis.dataType.periodSelected);
                dt = ds.Tables[0];
                if (dt.Columns.Contains("first_date")) firstDate = dt.Rows[0]["first_date"].ToString();
                if (dt.Columns.Contains("last_date")) lastDate = dt.Rows[0]["last_date"].ToString();
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
            long nbLines = 0;
            long lineIndex = 0;
            switch (_vehicleInformation.Id) {
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
                    nbLines = 10;
                    if (isAlertModule) nbLines = nbLines + 5;
                    if (investment != null && investment.Length > 0) nbLines++; // nbLines = 16;
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.outdoor:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.cinema:
                    nbLines = 9;
                    if (investment != null && investment.Length > 0) nbLines++;//nbLines = 12;
                    break;
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press:
					nbLines = 3;
					if (investment != null && investment.Length > 0) nbLines++;
					if (category != null && category.Length > 0 ) nbLines++;
					if (regie != null && regie.Length > 0 ) nbLines++;
					if (interestCenter != null && interestCenter.Length > 0 ) nbLines++;
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
            // Date begin and date end for outdooor
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor && _webSession.CustomerPeriodSelected.Is4M) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1607, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(DateString.dateTimeToDD_MM_YYYY(dtFirstDate, _webSession.SiteLanguage));

                ChangeLineType(ref lineType);

                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1608, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(DateString.dateTimeToDD_MM_YYYY(dtLastDate, _webSession.SiteLanguage));
            }
            // Period selected
            else {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1541, _webSession.SiteLanguage));
                if ((firstDate != null && firstDate.Length > 0 && lastDate != null && lastDate.Length > 0 && firstDate.Equals(lastDate) && isAlertModule)
                    || (dtLastDate.CompareTo(dtFirstDate) == 0 && !isAlertModule)) {
                    resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(DateString.dateTimeToDD_MM_YYYY(dtFirstDate, _webSession.SiteLanguage));
                }
                else {
                    resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(896, _webSession.SiteLanguage) + " " + DateString.dateTimeToDD_MM_YYYY(dtFirstDate, _webSession.SiteLanguage) + " " + GestionWeb.GetWebWord(1730, _webSession.SiteLanguage) + " " + DateString.dateTimeToDD_MM_YYYY(dtLastDate, _webSession.SiteLanguage));
                }
                //}
            }

            ChangeLineType(ref lineType);

            // Category
            if (category.Length > 0) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1416, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(category);
                ChangeLineType(ref lineType);
            }

            // Media seller
            if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing && regie.Length > 0) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1417, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(regie);
                ChangeLineType(ref lineType);
            }

            // Interest center
            if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing && interestCenter.Length > 0) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1411, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(interestCenter);
                ChangeLineType(ref lineType);
            }

            // Cas tv, radio
            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others) {

                if (totalDuration.Length == 0) {
                    totalDuration = "0";
                }
                // total durée
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1413, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellDuration(double.Parse(totalDuration));
                ChangeLineType(ref lineType);
            }

            // Total investissements
            if (investment != null && investment.Length > 0) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1390, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellEuro(double.Parse(investment));
                ChangeLineType(ref lineType);
            }

            //Nombre de produits
            lineIndex = resultTable.AddNewLine(lineType);
            resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1393, _webSession.SiteLanguage));
            resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(numberProduct.ToString()));
            ChangeLineType(ref lineType);

            //Nombre d'annonceurs
            lineIndex = resultTable.AddNewLine(lineType);
            resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1396, _webSession.SiteLanguage));
            resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(numberAdvertiser.ToString()));
            ChangeLineType(ref lineType);
            #endregion

            return resultTable;

        }
        #endregion		

    }
}
