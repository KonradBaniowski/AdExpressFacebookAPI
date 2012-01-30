#region Info
/*
 * Author :     G Ragneau
 * Created on : 29/07/2008
 * History:
 *      Date - Author - Description
 *      29/07/2008 - G Ragneau - Moved from TNS.AdExpress.Web
 * 
 * 
 * */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using CstComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CstUnit = TNS.AdExpress.Constantes.Web.CustomerSessions.Unit;
using FctUtilities = TNS.AdExpress.Web.Functions;
using DBConstantes = TNS.AdExpress.Constantes.DB;


using TNS.AdExpress.Classification;
using TNS.Classification.Universe;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using TNS.AdExpressI.ProductClassIndicators.Exceptions;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpressI.ProductClassIndicators.Engines
{
    /// <summary>
    /// Engine to build a Summary report as a table
    /// </summary>
    public class EngineSummary:Engine
    {

        #region Constantes
        /// <summary>
        /// Label column index
        /// </summary>
        public const int LABEL_COLUMN_INDEX = 0;
        /// <summary>
        /// Selected Univers Investments Line index
        /// </summary>
        public const int TOTAL_UNIV_INVEST_LINE_INDEX = 1;
        /// <summary>
        /// Sector Univers Investments Line index
        /// </summary>
        public const int TOTAL_SECTOR_INVEST_LINE_INDEX = 2;
        /// <summary>
        /// PDV Selected Univers / Sector line index
        /// </summary>
        public const int PDV_UNIV_TOTAL_SECTOR_LINE_INDEX = 3;
        /// <summary>
        /// Market Univers Investments Line index
        /// </summary>
        public const int TOTAL_MARKET_INVEST_LINE_INDEX = 4;
        /// <summary>
        /// PDV Selected Univers / Market line index
        /// </summary>
        public const int PDV_UNIV_TOTAL_MARKET_LINE_INDEX = 5;
        /// <summary>
        /// Advertiser number line index
        /// </summary>
        public const int NB_ADVERTISER_LINE_INDEX = 6;
        /// <summary>
        /// Average investments advertisers/univers
        /// </summary>
        public const int AVERAGE_INVEST_BY_ADVERTISER_LINE_INDEX = 7;
        /// <summary>
        /// Products number line index
        /// </summary>
        public const int NB_PRODUCT_LINE_INDEX = 8;
        /// <summary>
        /// Average investments products/univers
        /// </summary>
        public const int AVERAGE_INVEST_BY_PRODUCT_LINE_INDEX = 9;
        /// <summary>
        /// Year N column index
        /// </summary>
        public const int TOTAL_N_COLUMN_INDEX = 1;
        /// <summary>
        /// Year N-1 column index
        /// </summary>
        public const int TOTAL_N1_COLUMN_INDEX = 2;
        /// <summary>
        /// Evoluiotn column index
        /// </summary>
        public const int EVOLUTION_COLUMN_INDEX = 3;
        /// <summary>
        /// Difference Column index
        /// </summary>
        public const int ECART_COLUMN_INDEX = 4;
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public EngineSummary(WebSession session, IProductClassIndicatorsDAL dalLayer) : base(session, dalLayer) { }
        #endregion

        #region GetResult
        /// <summary>
        /// Get Summary indicator as a table
        /// </summary>
        /// <returns>StringBuilder with HTML code</returns>
        public override StringBuilder GetResult()
        {
            return BuildReport();
        }
        #endregion

        #region GetData
        /// <summary>
        /// Get data ready for display
        /// </summary>
        /// <returns></returns>
        virtual public object[,] GetData()
        {

            object[,] tab = null;
            UnitInformation selectedCurrency = GetUnit();

            try
            {

                #region Load data
                //Total selection
                DataTable dtUniversTotal = _dalLayer.GetSummaryInvestments(CstComparaisonCriterion.universTotal).Tables[0];

                //Sector total
                DataTable dtSectorTotal = _dalLayer.GetSummaryInvestments(CstComparaisonCriterion.sectorTotal).Tables[0];

                //Market total
                DataTable dtMarketTotal = _dalLayer.GetSummaryInvestments(CstComparaisonCriterion.marketTotal).Tables[0];

                //Number of active product
				DataTable dtNbRef = (_session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) ? _dalLayer.GetSummaryVolumes(CstComparaisonCriterion.universTotal, CstResult.MotherRecap.ElementType.product).Tables[0] : null;

                //Number of active advertiser
                DataTable dtNbAdvert = _dalLayer.GetSummaryVolumes(CstComparaisonCriterion.universTotal, CstResult.MotherRecap.ElementType.advertiser).Tables[0];
                #endregion

                #region Build table
                if (hasData(dtUniversTotal, dtSectorTotal, dtMarketTotal, dtNbRef, dtNbAdvert))
                {
                    tab = new object[10, 5];

                    #region Labels
                    tab[0, TOTAL_N_COLUMN_INDEX] = FctUtilities.Dates.getPeriodLabel(_session, CstPeriod.Type.currentYear);

                    if (_session.ComparativeStudy)
                    {
                        tab[0, TOTAL_N1_COLUMN_INDEX] = FctUtilities.Dates.getPeriodLabel(_session, CstPeriod.Type.previousYear);
                        tab[0, EVOLUTION_COLUMN_INDEX] = GestionWeb.GetWebWord(1207, _session.SiteLanguage);
                        tab[0, ECART_COLUMN_INDEX] = GestionWeb.GetWebWord(1213, _session.SiteLanguage);

                    }
                    tab[1, 0] = GestionWeb.GetWebWord(1712, _session.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_session.SiteLanguage) + ") (" + GestionWeb.GetWebWord(2790, _session.SiteLanguage) + ")";
                    tab[2, 0] = GestionWeb.GetWebWord(1712, _session.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_session.SiteLanguage) + ") (" + GestionWeb.GetWebWord(2791, _session.SiteLanguage) + ")";
                    tab[3, 0] = GestionWeb.GetWebWord(1903, _session.SiteLanguage);
                    tab[4, 0] = GestionWeb.GetWebWord(1712, _session.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_session.SiteLanguage) + ") (" + GestionWeb.GetWebWord(2792, _session.SiteLanguage) + ")";
                    tab[5, 0] = GestionWeb.GetWebWord(1904, _session.SiteLanguage);
                    tab[6, 0] = GestionWeb.GetWebWord(1905, _session.SiteLanguage);
                    tab[7, 0] = GestionWeb.GetWebWord(1153, _session.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_session.SiteLanguage) + ") " + GestionWeb.GetWebWord(2793, _session.SiteLanguage);
                    tab[8, 0] = GestionWeb.GetWebWord(1906, _session.SiteLanguage);
                    tab[9, 0] = GestionWeb.GetWebWord(1153, _session.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_session.SiteLanguage) + ") " + GestionWeb.GetWebWord(2794, _session.SiteLanguage);
                    #endregion

                    #region Data
                    //Keuros Univers Total
                    ComputeBudget(tab, dtUniversTotal, TOTAL_UNIV_INVEST_LINE_INDEX, _session.ComparativeStudy);

                    //Keuros Sector Total
                    ComputeBudget(tab, dtSectorTotal, TOTAL_SECTOR_INVEST_LINE_INDEX, _session.ComparativeStudy);

                    //PDV univers vs sector total
                    ComputePDV(tab, TOTAL_UNIV_INVEST_LINE_INDEX, TOTAL_SECTOR_INVEST_LINE_INDEX, PDV_UNIV_TOTAL_SECTOR_LINE_INDEX, TOTAL_N_COLUMN_INDEX);//Année N

                    if (_session.ComparativeStudy)
                        ComputePDV(tab, TOTAL_UNIV_INVEST_LINE_INDEX, TOTAL_SECTOR_INVEST_LINE_INDEX, PDV_UNIV_TOTAL_SECTOR_LINE_INDEX, TOTAL_N1_COLUMN_INDEX);//Année N-1

                    //Keuros Market Total
                    ComputeBudget(tab, dtMarketTotal, TOTAL_MARKET_INVEST_LINE_INDEX, _session.ComparativeStudy);

                    //PDV univers vs market Total
                    ComputePDV(tab, TOTAL_UNIV_INVEST_LINE_INDEX, TOTAL_MARKET_INVEST_LINE_INDEX, PDV_UNIV_TOTAL_MARKET_LINE_INDEX, TOTAL_N_COLUMN_INDEX);//Année N

                    if (_session.ComparativeStudy)
                        ComputePDV(tab, TOTAL_UNIV_INVEST_LINE_INDEX, TOTAL_MARKET_INVEST_LINE_INDEX, PDV_UNIV_TOTAL_MARKET_LINE_INDEX, TOTAL_N1_COLUMN_INDEX);//Année N-1

                    //Nb of active advertisers
                    if (!dtNbAdvert.Equals(System.DBNull.Value) && dtNbAdvert.Rows.Count > 0)
                    {
                        tab[NB_ADVERTISER_LINE_INDEX, TOTAL_N_COLUMN_INDEX] = dtNbAdvert.Rows[0]["nbElt"];//Année N
                        if (_session.ComparativeStudy)
                        {
                            tab[NB_ADVERTISER_LINE_INDEX, TOTAL_N1_COLUMN_INDEX] = dtNbAdvert.Rows[1]["nbElt"];//Année N-1
                            //Evolution and difference of N / N-1
                            ComputeEvolAndEcart(tab, NB_ADVERTISER_LINE_INDEX, TOTAL_N_COLUMN_INDEX, TOTAL_N1_COLUMN_INDEX, EVOLUTION_COLUMN_INDEX, ECART_COLUMN_INDEX);
                        }
                    }

                    //Investments average by advertiser
                    ComputeAverageBudget(tab, TOTAL_UNIV_INVEST_LINE_INDEX, NB_ADVERTISER_LINE_INDEX, AVERAGE_INVEST_BY_ADVERTISER_LINE_INDEX, TOTAL_N_COLUMN_INDEX);//Année N

                    if (_session.ComparativeStudy)
                    {
                        ComputeAverageBudget(tab, TOTAL_UNIV_INVEST_LINE_INDEX, NB_ADVERTISER_LINE_INDEX, AVERAGE_INVEST_BY_ADVERTISER_LINE_INDEX, TOTAL_N1_COLUMN_INDEX);//Année N-1
                        //Evolution and difference of N / N-1
                        ComputeEvolAndEcart(tab, AVERAGE_INVEST_BY_ADVERTISER_LINE_INDEX, TOTAL_N_COLUMN_INDEX, TOTAL_N1_COLUMN_INDEX, EVOLUTION_COLUMN_INDEX, ECART_COLUMN_INDEX);
                    }

					if (_session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) {
						//Nb of active products
						if (!dtNbRef.Equals(System.DBNull.Value) && dtNbRef.Rows.Count > 0) {
							tab[NB_PRODUCT_LINE_INDEX, TOTAL_N_COLUMN_INDEX] = dtNbRef.Rows[0]["nbElt"];//Année N
							if (_session.ComparativeStudy) {
								tab[NB_PRODUCT_LINE_INDEX, TOTAL_N1_COLUMN_INDEX] = dtNbRef.Rows[1]["nbElt"];//Année N-1
								//Evolution and difference of N / N-1
								ComputeEvolAndEcart(tab, NB_PRODUCT_LINE_INDEX, TOTAL_N_COLUMN_INDEX, TOTAL_N1_COLUMN_INDEX, EVOLUTION_COLUMN_INDEX, ECART_COLUMN_INDEX);
							}
						}

						//Investments average by product
						ComputeAverageBudget(tab, TOTAL_UNIV_INVEST_LINE_INDEX, NB_PRODUCT_LINE_INDEX, AVERAGE_INVEST_BY_PRODUCT_LINE_INDEX, TOTAL_N_COLUMN_INDEX);//Année N

						if (_session.ComparativeStudy) {
							ComputeAverageBudget(tab, TOTAL_UNIV_INVEST_LINE_INDEX, NB_PRODUCT_LINE_INDEX, AVERAGE_INVEST_BY_PRODUCT_LINE_INDEX, TOTAL_N1_COLUMN_INDEX);//Année N-1
							//Evolution and difference of N / N-1
							ComputeEvolAndEcart(tab, AVERAGE_INVEST_BY_PRODUCT_LINE_INDEX, TOTAL_N_COLUMN_INDEX, TOTAL_N1_COLUMN_INDEX, EVOLUTION_COLUMN_INDEX, ECART_COLUMN_INDEX);
						}
					}


                    #endregion
                }
                #endregion

            }
            catch (Exception ex)
            {
                throw new ProductClassIndicatorsException("Unable to compute data for summary report.", ex);
            }

            return tab;
        }
        #endregion

        #region Build report

        #region Table
        /// <summary>
        /// Build Sumamry report
        /// </summary>
        /// <returns>Novelty Table</returns>
        protected virtual StringBuilder BuildReport()
        {

            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);

            object[,] tab = this.GetData();

            #region No data
            if (tab == null)
            {
                t.AppendFormat("<div align=\"center\" class=\"txtViolet11Bold\">{0}</div>", GestionWeb.GetWebWord(177, _session.SiteLanguage));
                return t;
            }
            #endregion

            #region Styles
            string cssHeader = "p2";
            string cssLabel = (_excel) ? "acl11" : "acl1";
            string cssNb = (_excel) ? "acl21" : "acl2Bg";
            #endregion

            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

            t.Append("<table class=\"backGroundWhite\" border=0 cellpadding=0 cellspacing=0 align=center>");

            #region Headers
            t.Append("\r\n\t<tr height=\"20px\">");
            //N
            t.Append("<td  class=\"backGroundWhite\" nowrap  valign=\"middle\">&nbsp;</td>");
            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssHeader, tab[0, TOTAL_N_COLUMN_INDEX]);
            if (_session.ComparativeStudy)
            {
                //N-1
                t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssHeader, tab[0, TOTAL_N1_COLUMN_INDEX]);
                //Evolution
                t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssHeader, tab[0, EVOLUTION_COLUMN_INDEX]);
                //Ecart
                t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssHeader, tab[0, ECART_COLUMN_INDEX]);

            }
            t.Append("</tr>");
            #endregion

            #region Line
            for (int i = 1; i < tab.GetLength(0); i++)
            {
				//Rights level products (For Finland)
				if ((NB_PRODUCT_LINE_INDEX == i && !_session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
					|| (AVERAGE_INVEST_BY_PRODUCT_LINE_INDEX == i && !_session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))) continue;

                t.Append("\r\n\t<tr align=\"right\"  class=\"violetBackGroundV3\" height=\"20px\" >");
                //Label
                t.AppendFormat("\r\n\t\t<td align=\"left\" class=\"{0}\" nowrap>{1}</td>", cssLabel, tab[i, 0]);
                switch (i)
                {
                    case TOTAL_MARKET_INVEST_LINE_INDEX:
                    case TOTAL_SECTOR_INVEST_LINE_INDEX:
                    case TOTAL_UNIV_INVEST_LINE_INDEX:
                    case AVERAGE_INVEST_BY_ADVERTISER_LINE_INDEX:
                    case AVERAGE_INVEST_BY_PRODUCT_LINE_INDEX:
                        //N
                        t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(tab[i, TOTAL_N_COLUMN_INDEX], _session.Unit, fp));
                        //N-1
                        if (_session.ComparativeStudy)
                        {
                            t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(tab[i, TOTAL_N1_COLUMN_INDEX], _session.Unit, fp));
                            AppendEvol(t, tab, cssNb, i, fp);
                            //Difference
                            t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(tab[i, ECART_COLUMN_INDEX], _session.Unit, fp));
                        }
                        break;
                    case NB_ADVERTISER_LINE_INDEX:
                    case NB_PRODUCT_LINE_INDEX:
						
                        //N
                        t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(tab[i, TOTAL_N_COLUMN_INDEX], UnitsInformation.DefaultCurrency, fp));
                        //N-1
                        if (_session.ComparativeStudy)
                        {
                            t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(tab[i, TOTAL_N1_COLUMN_INDEX], UnitsInformation.DefaultCurrency, fp));
                            AppendEvol(t, tab, cssNb, i, fp);
                            //Difference
                            t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(tab[i, ECART_COLUMN_INDEX], UnitsInformation.DefaultCurrency, fp));
                        }
                        break;
                    case PDV_UNIV_TOTAL_MARKET_LINE_INDEX:
                    case PDV_UNIV_TOTAL_SECTOR_LINE_INDEX:
                        //N
                        t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1} %</td>", cssNb, FctUtilities.Units.ConvertUnitValueAndPdmToString(tab[i, TOTAL_N_COLUMN_INDEX], _session.Unit, true, fp));
                        //N-1
                        if (_session.ComparativeStudy)
                        {
                            t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1} %</td>", cssNb, FctUtilities.Units.ConvertUnitValueAndPdmToString(tab[i, TOTAL_N1_COLUMN_INDEX], _session.Unit, true, fp));
                            t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>&nbsp;</td>", cssNb);
                            t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>&nbsp;</td>", cssNb);
                        }
                        break;
                }
                t.Append("</tr>");

                if (!_excel && (i == PDV_UNIV_TOTAL_MARKET_LINE_INDEX || i == AVERAGE_INVEST_BY_ADVERTISER_LINE_INDEX)){
                    t.AppendFormat("<tr><td class=\"backGroundWhite whiteBottomBorder\" style=\"HEIGHT: 5px; BORDER-TOP: white 0px solid;\" colspan={0}></td></tr>"
                        ,(_session.ComparativeStudy) ? 5 : 3);
                }

            }
            #endregion

            t.Append("</table>");

            return t;

        }

        #region AppendEvol
        /// <summary>
        /// Append Evolution cell
        /// </summary>
        /// <param name="t">HTML container</param>
        /// <param name="tab">Data table</param>
        /// <param name="cssNb">CSS Style to apply</param>
        /// <param name="line">Current line</param>
        protected virtual void AppendEvol(System.Text.StringBuilder t, object[,] tab, string cssNb, int line, IFormatProvider fp)
        {
            //Evol
            string img = string.Empty;
            double value = Convert.ToDouble(tab[line, EVOLUTION_COLUMN_INDEX]);
            if (!_excel)
            {
                if (value > 0){
                    if(_pdf)
                        img = " <img src=\"" + AppDomain.CurrentDomain.BaseDirectory + "/Images/g.jpg\">";
                    else
                        img = " <img src=/I/g.gif>";
                }
                else if (value < 0){
                    if (_pdf)
                        img = " <img src=\"" + AppDomain.CurrentDomain.BaseDirectory + "/Images/r.jpg\">";
                    else
                        img = " <img src=/I/r.gif>";
                }
                else{
                    if (_pdf)
                        img = " <img src=\"" + AppDomain.CurrentDomain.BaseDirectory + "/Images/o.jpg\">";
                    else
                        img = " <img src=/I/o.gif>";
                }
            }
            if (!Double.IsNaN(value))
            {
                t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}{2}</td>"
                    , cssNb
                    , (Double.IsInfinity(value)) ? string.Empty : string.Format("{0} %", FctUtilities.Units.ConvertUnitValueAndPdmToString(tab[line, EVOLUTION_COLUMN_INDEX], _session.Unit, true, fp))
                    , img);
            }
            else
            {
                t.AppendFormat("\r\n\t<td class=\"{0}{1}\" nowrap></td>", (_excel ? "" : "violetBackGroundV2 "), cssNb);
            }
        }
        #endregion

        #endregion

        #endregion

        #region Engine methods
        /// <summary>
        /// Compute investments depending no the type of univers
        /// </summary>
        /// <param name="tab">Result table</param>
        /// <param name="dt">Source data</param>
        /// <param name="lineIndex">Index of the current line</param>
        /// <param name="comparativeStudy">Comparative study?</param>
        protected void ComputeBudget(object[,] tab, DataTable dt, int lineIndex, bool comparativeStudy)
        {
            double tempEvol = 0;
            if (!dt.Equals(System.DBNull.Value) && dt.Rows.Count > 0)
            {
                tab[lineIndex, TOTAL_N_COLUMN_INDEX] = dt.Rows[0]["total_N"];//Année N
                if (comparativeStudy)
                {
                    tab[lineIndex, TOTAL_N1_COLUMN_INDEX] = dt.Rows[0]["total_N1"];//Année N-1
                    if (dt.Rows[0]["evol"].Equals(System.DBNull.Value) && tab[lineIndex, TOTAL_N1_COLUMN_INDEX] != DBNull.Value)
                    {
                        tempEvol = ((Convert.ToDouble(tab[lineIndex, TOTAL_N_COLUMN_INDEX]) - Convert.ToDouble(tab[lineIndex, TOTAL_N1_COLUMN_INDEX])) * (double)100.0) / Convert.ToDouble(tab[lineIndex, TOTAL_N1_COLUMN_INDEX]);
                        tab[lineIndex, EVOLUTION_COLUMN_INDEX] = tempEvol;
                    }
                    else tab[lineIndex, EVOLUTION_COLUMN_INDEX] = Convert.ToDouble(dt.Rows[0]["evol"]);//evolution
                    tab[lineIndex, ECART_COLUMN_INDEX] = Convert.ToDouble(dt.Rows[0]["ecart"]);//ecart
                }
            }
        }

        /// <summary>
        /// Compute investments average depending on type of univers by product or advertiser
        /// </summary>
        /// <param name="tab">Result table</param>
        /// <param name="totalInvestLineIndex">Index of total investments line</param>
        /// <param name="nbAdvertLineIndex">Index of elemtn number line</param>
        /// <param name="averageInvestLineIndex">Investments average line</param>
        /// <param name="columnLineIndex">Current column index</param>
        protected void ComputeAverageBudget(object[,] tab, int totalInvestLineIndex, int nbAdvertLineIndex, int averageInvestLineIndex, int columnLineIndex)
        {
            if (tab[totalInvestLineIndex, columnLineIndex] != null
                && tab[nbAdvertLineIndex, columnLineIndex] != null
                && Convert.ToDouble(tab[nbAdvertLineIndex, columnLineIndex]) > 0)
            {
                tab[averageInvestLineIndex, columnLineIndex] = Convert.ToDouble(tab[totalInvestLineIndex, columnLineIndex]) / Convert.ToDouble(tab[nbAdvertLineIndex, columnLineIndex]);
            }
        }
        /// <summary>
        /// Compute PDV
        /// </summary>
        /// <param name="tab">Result table</param>
        /// <param name="childLineIndex">Sub total line index</param>
        /// <param name="parentLineIndex">total line index</param>
        /// <param name="pdvLineIndex">PDV line index</param>
        /// <param name="columnIndex">Column index</param>
        protected void ComputePDV(object[,] tab, int childLineIndex, int parentLineIndex, int pdvLineIndex, int columnIndex)
        {
            if (tab[childLineIndex, columnIndex] != null
                && tab[parentLineIndex, columnIndex] != null
                && Convert.ToDouble(tab[parentLineIndex, columnIndex]) > 0)
            {
                tab[pdvLineIndex, columnIndex] = (Convert.ToDouble(tab[childLineIndex, columnIndex]) * (double)100.0) / Convert.ToDouble(tab[parentLineIndex, columnIndex]);
            }
        }

        /// <summary>
        /// Compute evolution and difference between N and N-1
        /// </summary>
        /// <param name="tab">result table</param>
        /// <param name="lineIndex">Current line index</param>
        /// <param name="total_N_Column">Year N total column index</param>
        /// <param name="total_N1_Column">Year -1N total column index</param>
        /// <param name="evolColumn">Evolution column index</param>
        /// <param name="ecartColumn">Difference column index</param>
        protected void ComputeEvolAndEcart(object[,] tab, int lineIndex, int total_N_Column, int total_N1_Column, int evolColumn, int ecartColumn)
        {
            if (tab[lineIndex, total_N_Column] != null && tab[lineIndex, total_N1_Column] != null)
            {
                double totalN = Convert.ToDouble(tab[lineIndex, total_N_Column]);
                double totalN1 = Convert.ToDouble(tab[lineIndex, total_N1_Column]);

                //Evolution anneé N par rapport N-1 = ((N-(N-1))*100)/N-1 
                tab[lineIndex, evolColumn] = ((totalN - totalN1) * (double)100.0) / totalN1;//evolution
                //Ecart anneé N - rapport N-1				
                tab[lineIndex, ecartColumn] = totalN - totalN1;//ecart
            }
        }

        /// <summary>
        /// Check there is data to treat
        /// </summary>
        /// <param name="dtUniversTotal">Univers total data</param>
        /// <param name="dtSectorTotal">Sector total data</param>
        /// <param name="dtMarketTotal">Market total data</param>
        /// <param name="dtNbRef">NUmner of references data</param>
        /// <param name="dtNbAdvert">Number of advertsiers data</param>
        /// <returns>True if there are data.</returns>
        protected bool hasData(DataTable dtUniversTotal, DataTable dtSectorTotal, DataTable dtMarketTotal, DataTable dtNbRef, DataTable dtNbAdvert)
        {
            return (!dtUniversTotal.Equals(System.DBNull.Value) && dtUniversTotal.Rows.Count > 0)
                || (!dtSectorTotal.Equals(System.DBNull.Value) && dtSectorTotal.Rows.Count > 0)
                || (!dtMarketTotal.Equals(System.DBNull.Value) && dtMarketTotal.Rows.Count > 0)
                || (!dtNbRef.Equals(System.DBNull.Value) && dtNbRef.Rows.Count > 0 && Convert.ToDouble(dtNbRef.Compute("sum(nbelt)", string.Empty)) > 0)
                || (!dtNbAdvert.Equals(System.DBNull.Value) && dtNbAdvert.Rows.Count > 0 && Convert.ToDouble(dtNbAdvert.Compute("sum(nbelt)", string.Empty)) > 0);
        }
        #endregion
    }
}
