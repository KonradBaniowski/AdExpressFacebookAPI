#region Info
/*
 * Author :     Y R'kaina
 * Created on : 28/07/2010
 * History:
 * 
 * 
 * */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using TNS.AdExpressI.ProductClassIndicators.Exceptions;
using System.Data;

using FctUtilities = TNS.AdExpress.Web.Functions;
using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using FctIndicators = TNS.AdExpressI.ProductClassIndicators.Russia.Functions;
using TNS.AdExpress.Domain.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using CstUnit = TNS.AdExpress.Constantes.Web.CustomerSessions.Unit;


namespace TNS.AdExpressI.ProductClassIndicators.Russia.Engines{
    /// <summary>
    /// Engine to build a Summary report as a table
    /// This class contains specific rules for the russian version
    /// </summary>
    public class EngineSummary : TNS.AdExpressI.ProductClassIndicators.Engines.EngineSummary
    {

        #region Constantes
        /// <summary>
        /// Reference period index
        /// </summary>
        public const int REFERENCE_PERIOD_INDEX = 1;
        /// <summary>
        /// Competitor period index
        /// </summary>
        public const int COMPETITOR_PERIOD_INDEX = 2;
        /// <summary>
        /// Change index
        /// </summary>
        public const int CHANGE_INDEX = 3;
        /// <summary>
        /// Difference index
        /// </summary>
        public const int DiFFERENCE_INDEX = 4;
        #endregion 

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public EngineSummary(WebSession session, IProductClassIndicatorsDAL dalLayer) : base(session, dalLayer) { }
        #endregion

        #region GetData
        /// <summary>
        /// Get data ready for display
        /// </summary>
        /// <returns></returns>
        override public object[,] GetData(){

            object[,] tab = null;
            UnitInformation selectedCurrency = _session.GetSelectedUnit();

            try{

                //Load data
                DataSet dsUniversTotal = _dalLayer.GetSummary();
                DataTable dtUniversTotal ;
                IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo;

                // Check if there is no data
                if (dsUniversTotal == null || dsUniversTotal.Tables.Count == 0){
                    return null;
                }
                dtUniversTotal = dsUniversTotal.Tables[0];

                if (dtUniversTotal.Rows.Count == 0) return null;

                if (!dtUniversTotal.Equals(System.DBNull.Value) && dtUniversTotal.Rows.Count > 0) {

                    tab = new object[10, 5];

                    #region Labels
                    tab[0, TOTAL_N_COLUMN_INDEX] = FctUtilities.Dates.getPeriodLabel(_session, CstPeriod.Type.currentYear);

                    if (_session.ComparativeStudy){
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
                    int index = 0;
                    foreach (DataRow row in dtUniversTotal.Rows) {

                        index = Convert.ToInt32(row["total_type"].ToString());
                        if (!FctIndicators.IsNull(row["Unit_N"]))
                        {
                            if (index == NB_ADVERTISER_LINE_INDEX || index == NB_PRODUCT_LINE_INDEX)//Number
                                tab[index, REFERENCE_PERIOD_INDEX] = FctIndicators.ConvertUnitValueToString(row["Unit_N"], CstUnit.rubles, fp);
                            else if (index == PDV_UNIV_TOTAL_SECTOR_LINE_INDEX || index == PDV_UNIV_TOTAL_SECTOR_LINE_INDEX)//PDV
                                tab[index, REFERENCE_PERIOD_INDEX] = FctIndicators.ConvertUnitValueAndPdmToString(row["Unit_N"], _session.Unit, true, fp);
                            else tab[index, REFERENCE_PERIOD_INDEX] = FctIndicators.ConvertUnitValueToString(row["Unit_N"], _session.Unit, fp);
                        }

                        if (_session.ComparativeStudy){

                            if (!FctIndicators.IsNull(row["Unit_N1"]))
                            {
                                if (index == NB_ADVERTISER_LINE_INDEX || index == NB_PRODUCT_LINE_INDEX)//Number
                                    tab[index, COMPETITOR_PERIOD_INDEX] = FctIndicators.ConvertUnitValueToString(row["Unit_N1"], CstUnit.rubles, fp);
                                else if (index == PDV_UNIV_TOTAL_SECTOR_LINE_INDEX || index == PDV_UNIV_TOTAL_SECTOR_LINE_INDEX)//PDV
                                    tab[index, COMPETITOR_PERIOD_INDEX] = FctIndicators.ConvertUnitValueAndPdmToString(row["Unit_N1"], _session.Unit, true, fp);
                                else tab[index, COMPETITOR_PERIOD_INDEX] = FctIndicators.ConvertUnitValueToString(row["Unit_N1"], _session.Unit, fp);
                            }

                            if (!FctIndicators.IsNull(row["change"]))
                                tab[index, CHANGE_INDEX] = FctIndicators.ConvertUnitValueAndPdmToString(row["change"], _session.Unit, true, fp);

                            if (!FctIndicators.IsNull(row["difference"]))
                            {
                                if (index == NB_ADVERTISER_LINE_INDEX || index == NB_PRODUCT_LINE_INDEX)//Number
                                    tab[index, DiFFERENCE_INDEX] = FctIndicators.ConvertUnitValueToString(row["difference"], CstUnit.rubles, fp);
                                else tab[index, DiFFERENCE_INDEX] = FctIndicators.ConvertUnitValueToString(row["difference"], _session.Unit, fp);
                            }
                        }
                    }
                    #endregion

                }

            }
            catch (Exception ex){
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
        protected override StringBuilder BuildReport()
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

            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo;

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
                if(!FctIndicators.IsNull(tab[i, 0]))
                t.AppendFormat("\r\n\t\t<td align=\"left\" class=\"{0}\" nowrap>{1}</td>", cssLabel, tab[i, 0]);
                else t.AppendFormat("\r\n\t\t<td align=\"left\" class=\"{0}\" nowrap>{1}</td>", cssLabel, "&nbsp;");

                switch (i)
                {
                    case TOTAL_MARKET_INVEST_LINE_INDEX:
                    case TOTAL_SECTOR_INVEST_LINE_INDEX:
                    case TOTAL_UNIV_INVEST_LINE_INDEX:
                    case AVERAGE_INVEST_BY_ADVERTISER_LINE_INDEX:
                    case AVERAGE_INVEST_BY_PRODUCT_LINE_INDEX:
                        //N
                        if(!FctIndicators.IsNull(tab[i, TOTAL_N_COLUMN_INDEX]))
                        t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb,tab[i, TOTAL_N_COLUMN_INDEX]);
                        else t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, "&nbsp;");
                        //N-1
                        if (_session.ComparativeStudy)
                        {
                            if (!FctIndicators.IsNull(tab[i, TOTAL_N1_COLUMN_INDEX]))
                                t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, tab[i, TOTAL_N1_COLUMN_INDEX]);
                             else t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, "&nbsp;");
                            AppendEvol(t, tab, cssNb, i, fp);
                            //Difference
                            if (!FctIndicators.IsNull(tab[i, ECART_COLUMN_INDEX]))
                            t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, tab[i, ECART_COLUMN_INDEX]);
                            else t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, "&nbsp;");
                        }
                        break;
                    case NB_ADVERTISER_LINE_INDEX:
                    case NB_PRODUCT_LINE_INDEX:

                        //N
                        if (!FctIndicators.IsNull(tab[i, TOTAL_N_COLUMN_INDEX]))
                        t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, tab[i, TOTAL_N_COLUMN_INDEX]);
                        else  t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, "&nbsp;");
                        //N-1
                        if (_session.ComparativeStudy)
                        {
                            if (!FctIndicators.IsNull(tab[i, TOTAL_N1_COLUMN_INDEX]))
                            t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, tab[i, TOTAL_N1_COLUMN_INDEX]);
                            else t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, "&nbsp;");
                            AppendEvol(t, tab, cssNb, i, fp);
                            //Difference
                            if (!FctIndicators.IsNull(tab[i, ECART_COLUMN_INDEX]))
                            t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, tab[i, ECART_COLUMN_INDEX]);
                            else t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, "&nbsp;");
                        }
                        break;
                    case PDV_UNIV_TOTAL_MARKET_LINE_INDEX:
                    case PDV_UNIV_TOTAL_SECTOR_LINE_INDEX:
                        //N
                        if (!FctIndicators.IsNull(tab[i, TOTAL_N_COLUMN_INDEX]))
                        t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1} %</td>", cssNb, tab[i, TOTAL_N_COLUMN_INDEX]);
                        else t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, "&nbsp;");
                        //N-1
                        if (_session.ComparativeStudy)
                        {
                            if (!FctIndicators.IsNull(tab[i, TOTAL_N1_COLUMN_INDEX]))
                            t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1} %</td>", cssNb, tab[i, TOTAL_N1_COLUMN_INDEX]);
                            else t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>{1}</td>", cssNb, "&nbsp;");
                            t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>&nbsp;</td>", cssNb);
                            t.AppendFormat("\r\n\t<td class=\"{0}\" nowrap>&nbsp;</td>", cssNb);
                        }
                        break;
                }
                t.Append("</tr>");

                if (!_excel && (i == PDV_UNIV_TOTAL_MARKET_LINE_INDEX || i == AVERAGE_INVEST_BY_ADVERTISER_LINE_INDEX))
                {
                    t.AppendFormat("<tr><td class=\"backGroundWhite whiteBottomBorder\" style=\"HEIGHT: 5px; BORDER-TOP: white 0px solid;\" colspan={0}></td></tr>"
                        , (_session.ComparativeStudy) ? 5 : 3);
                }

            }
            #endregion

            t.Append("</table>");

            return t;

        }

        

        #endregion

        #region AppendEvol
        /// <summary>
        /// Append Evolution cell
        /// </summary>
        /// <param name="t">HTML container</param>
        /// <param name="tab">Data table</param>
        /// <param name="cssNb">CSS Style to apply</param>
        /// <param name="line">Current line</param>
        protected override void AppendEvol(System.Text.StringBuilder t, object[,] tab, string cssNb, int line, IFormatProvider fp)
        {
            //Evol
            string img = string.Empty;
            double value = Convert.ToDouble(tab[line, EVOLUTION_COLUMN_INDEX],fp);
            if (!_excel)
            {
                if (value > 0)
                {
                    if (_pdf)
                        img = " <img src=\"" + AppDomain.CurrentDomain.BaseDirectory + "/Images/g.jpg\">";
                    else
                        img = " <img src=/I/g.gif>";
                }
                else if (value < 0)
                {
                    if (_pdf)
                        img = " <img src=\"" + AppDomain.CurrentDomain.BaseDirectory + "/Images/r.jpg\">";
                    else
                        img = " <img src=/I/r.gif>";
                }
                else
                {
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
                    , (Double.IsInfinity(value)) ? string.Empty : string.Format("{0} %", tab[line, EVOLUTION_COLUMN_INDEX])
                    , img);
            }
            else
            {
                t.AppendFormat("\r\n\t<td class=\"{0}{1}\" nowrap></td>", (_excel ? "" : "violetBackGroundV2 "), cssNb);
            }
        }
        #endregion

        #endregion
    }
}
