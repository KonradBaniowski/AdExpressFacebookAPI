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
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;
using System.Data;
using System.Text;

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using CstComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using DBConstantes = TNS.AdExpress.Constantes.DB;

using TNS.AdExpress.Classification;
using TNS.Classification.Universe;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using TNS.FrameWork.Date;

namespace TNS.AdExpressI.ProductClassIndicators.Engines
{
    /// <summary>
    /// Engine to build a Top report or to provide computed data for top report or indicator
    /// </summary>
    public class EngineTop:Engine
    {

        #region Constantes
        /// <summary>
        /// Number of element contained in the tops : 10 best advertisers...
        /// </summary>
        public const int TOPS_NUMBER = 10;
        /// <summary>
		/// Index of product id column
		/// </summary>
		public const int ID_PRODUCT_INDEX=0;
        /// <summary>
        /// Index of product column
        /// </summary>
        public const int PRODUCT = 1;
        /// <summary>
		/// Index of total column
		/// </summary>
		public const int TOTAL_N = 2;
        /// <summary>
        /// Index of SOV column
        /// </summary>
        public const int SOV = 3;
        /// <summary>
        /// Index of cumuled sov column
        /// </summary>
        public const int CUMUL_SOV = 4;
        /// <summary>
        /// Index of rank column
        /// </summary>
        public const int RANK = 5;
        /// <summary>
        /// Index of progress rank column
        /// </summary>
        public const int PROGRESS_RANK = 6;
        /// <summary>
        /// Index of column total N - 1
        /// </summary>
        public const int TOTAL_N1 = 7;
        /// <summary>
		/// Index of column to know if element is a competitor
		/// </summary>
		public const int COMPETITOR=8;
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public EngineTop(WebSession session, IProductClassIndicatorsDAL dalLayer) : base(session, dalLayer) { }
        #endregion

        #region GetResult
        /// <summary>
        /// Get Top indicator as a table
        /// </summary>
        /// <returns>StringBuilder with HTML code</returns>
        public override StringBuilder GetResult()
        {
            return BuildTops();
        }
        #endregion

        #region GetData
        /// <summary>
        /// Get data ready for display
        /// </summary>
        /// <param name="typeYear">Type of study (current or previous year)</param>
        /// <param name="classifLevel">Classification detail (product or advertiser)</param>
        /// <returns></returns>
        public object[,] GetData(CstResult.PalmaresRecap.typeYearSelected typeYear, CstResult.MotherRecap.ElementType classifLevel)
        {


            #region Data loading
            DataSet ds = _dalLayer.GetTops(typeYear, classifLevel); 
            DataTable dt = ds.Tables[0];
            #endregion

            #region Init Result Table
            int nbLine = Math.Min( dt.Rows.Count+1, TOPS_NUMBER+1 );
            object[,] tabResult = new object[ nbLine, COMPETITOR + 1];
            Dictionary<long, double> topN1Values = new Dictionary<long,double>();
            Dictionary<int, long> topIds = new Dictionary<int,long>();
            Dictionary<long, double> topN1ranks = new Dictionary<long,double>();
            string strTotalColName = ( typeYear == CstResult.PalmaresRecap.typeYearSelected.currentYear ) ? "total_N" : "total_N1";
            string strLabColName = ( classifLevel == CstResult.MotherRecap.ElementType.advertiser ) ? "advertiser" : "product";
            long idLev = -1;
            long idAdv = -1;
            double dN = 0;
            double dN1 = 0;
            DataRow row = null;

    
            for (int i = 1; i < nbLine; i++){
                
                row = dt.Rows[i-1];

                idAdv = Convert.ToInt64(row["id_advertiser"]);
                idLev = (classifLevel == CstResult.MotherRecap.ElementType.advertiser) ? idAdv : Convert.ToInt64(row["id_product"]);
                dN = Convert.ToDouble(row[strTotalColName]);
                tabResult[i, ID_PRODUCT_INDEX] = idLev;
                tabResult[i, PRODUCT] = row[strLabColName].ToString();

                if (_competitorIDS.Contains(idAdv)){
                    tabResult[i, COMPETITOR] = CstWeb.AdvertiserPersonalisation.Type.competitor;
                }
                else if (_referenceIDS.Contains(idAdv)){
                    tabResult[i, COMPETITOR] = CstWeb.AdvertiserPersonalisation.Type.reference;
                }
                else{
                    tabResult[i, COMPETITOR] = CstWeb.AdvertiserPersonalisation.Type.none;
                }
                tabResult[i, TOTAL_N] = dN;
                if (_session.ComparativeStudy && typeYear == CstResult.PalmaresRecap.typeYearSelected.currentYear)
                {
                    tabResult[1, TOTAL_N1] = dN1 = Convert.ToDouble(row["total_N1"]);
                    topN1Values.Add(idLev, dN1);
                    topN1ranks.Add(idLev, 1);
                    topIds.Add(i, idLev);
                }
                tabResult[i, RANK] = i;

            }
            #endregion

            #region Compute rank progression and total univers
            double totalUniverse = 0;
            if ((_session.ComparativeStudy && typeYear == CstResult.PalmaresRecap.typeYearSelected.currentYear) || _session.ComparaisonCriterion == CstWeb.CustomerSessions.ComparisonCriterion.universTotal)
            {
                int c = 0;
                foreach (DataRow cRow in dt.Rows)
                {

                    c++;
                    totalUniverse += Convert.ToDouble(cRow[strTotalColName]);

                    if (_session.ComparativeStudy && typeYear == CstResult.PalmaresRecap.typeYearSelected.currentYear)
                    {

                        dN1 = Convert.ToDouble(cRow["total_N1"]);

                        for (int i = 1; i < nbLine; i++)
                        {
                            //check rank related to current record
                            if (topN1Values[topIds[i]] < dN1)
                            {
                                topN1ranks[topIds[i]] = topN1ranks[topIds[i]] + 1;
                            }
                        }

                    }

                }
            }
            if (_session.ComparaisonCriterion != CstWeb.CustomerSessions.ComparisonCriterion.universTotal)
            {
                totalUniverse = _dalLayer.GetTotal(typeYear);
            }
            #endregion

            #region Finalise table (rank progress, SOVs, total line)
            //total line
            tabResult[0, TOTAL_N] = totalUniverse;
            tabResult[0, SOV] = (double)100.00;
            tabResult[0, CUMUL_SOV] = (double)100.00;
            //rank progress and SOV
            double cumulTops = 0;
            bool computeProgress = (_session.ComparativeStudy && typeYear == CstResult.PalmaresRecap.typeYearSelected.currentYear);
            for (int i = 1; i < nbLine; i++)
            {
                dN = Convert.ToDouble(tabResult[i, TOTAL_N]);
                cumulTops += dN;
                tabResult[i, SOV] = 100.00 * dN / totalUniverse;
                tabResult[i, CUMUL_SOV] = 100.00 * cumulTops / totalUniverse;
                if (computeProgress && topN1Values[topIds[i]] > 0)
                {
                    tabResult[i, PROGRESS_RANK] = topN1ranks[topIds[i]] - Convert.ToInt32(tabResult[i, RANK]);
                }

            }
            #endregion

            return tabResult;        
        
        }
        #endregion

        #region Build report
        /// <summary>
        /// Build one report
        /// </summary>
        /// <param name="classifLevel">Detail level (advertiser or product)</param>
        /// <param name="typeYear">Year N or N-1?</param>
        /// <returns>Code HTML</returns>
        protected StringBuilder BuildTop(CstResult.PalmaresRecap.typeYearSelected typeYear, CstResult.MotherRecap.ElementType classifLevel)
        {

            #region Variables
            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
            bool firstLine = true;
            string styleClassTitle = "";
            string styleClassNumber = "";
            string total = "";
            string periodDate = "";
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
            #endregion

            #region Constantes
            const string P2 = "p2";
            //Labels
            const string L1 = "p7";
            const string competitorStyle = "p14";
            const string competitorExcelStyle = "p142";
            const string referenceStyle = "p15";
            //Numbers
            const string L3 = "p9";
            const string competitorStyleNB = "p141";
            const string competitorExcelStyleNB = "p143";

            const string referenceStyleNB = "p151";
            // Total
            const string totalTitle = "pmtotal";
            const string totalNb = "pmtotalnb";
            #endregion

            object[,] tab = this.GetData(typeYear, classifLevel);

			CultureInfo cInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
			DateTimeFormatInfo myDTFI = cInfo.DateTimeFormat;

            if (typeYear == CstResult.PalmaresRecap.typeYearSelected.currentYear) {				
				periodDate = string.Format("{0}-{1}", FctUtilities.Dates.DateToString(_periodBegin.Date,_session.SiteLanguage), FctUtilities.Dates.DateToString(_periodEnd.Date,_session.SiteLanguage));
            }
            else
            {
				periodDate = string.Format("{0}-{1}", FctUtilities.Dates.DateToString(_periodBegin.Date.AddYears(-1),_session.SiteLanguage),FctUtilities.Dates.DateToString(_periodEnd.Date.AddYears(-1),_session.SiteLanguage));
            }

            if (tab.GetLongLength(0) == 1 || Convert.ToDouble(tab[0, TOTAL_N]) == 0)
            {

                if (classifLevel == CstResult.PalmaresRecap.ElementType.advertiser)
                {
                    t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
                    t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                    t.AppendFormat("{0}<br>{1} {2}", GestionWeb.GetWebWord(177, _session.SiteLanguage), GestionWeb.GetWebWord(2474, _session.SiteLanguage), periodDate);
                    t.Append("</td></tr></table>");
                }
                return t;

            }

            t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");

            #region Headers
            t.Append("\r\n\t<tr height=\"30px\" >");
            if (classifLevel == TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product)
            {
                t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\"  align=\"center\">{1}<br>{2}</td>", P2, GestionWeb.GetWebWord(1314, _session.SiteLanguage), periodDate);
            }
            else
            {
                t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\"  align=\"center\">{1}<br>{2}</td>", P2, GestionWeb.GetWebWord(1184, _session.SiteLanguage), periodDate);
            }
            //Unit
            t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\"  align=\"center\">{1}</td>", P2, GestionWeb.GetWebWord(1170, _session.SiteLanguage));
            //SOV
            t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\"  align=\"center\">{1}</td>", P2, GestionWeb.GetWebWord(1186, _session.SiteLanguage));
            t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\"  align=\"center\">{1}</td>", P2, GestionWeb.GetWebWord(1171, _session.SiteLanguage));
            //Rank
            t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\"  align=\"center\">{1}</td>", P2, GestionWeb.GetWebWord(1172, _session.SiteLanguage));
            if (typeYear == CstResult.PalmaresRecap.typeYearSelected.currentYear && _session.ComparativeStudy)
            {
                t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\"  align=\"center\">{1}</td>", P2, GestionWeb.GetWebWord(1173, _session.SiteLanguage));
            }
            t.Append("</tr>");
            #endregion

            for (int i = 0; i < tab.GetLongLength(0) && i < 11; i++)
            {

                #region Style css
                if (tab[i, CstResult.PalmaresRecap.COMPETITOR] != null && Convert.ToInt32(tab[i, CstResult.PalmaresRecap.COMPETITOR]) == 2)
                {
                    styleClassTitle = competitorStyle;
                    styleClassNumber = competitorStyleNB;
                    if (_excel)
                    {
                        styleClassTitle = competitorExcelStyle;
                        styleClassNumber = competitorExcelStyleNB;
                    }
                }
                else if (tab[i, CstResult.PalmaresRecap.COMPETITOR] != null && Convert.ToInt32(tab[i, CstResult.PalmaresRecap.COMPETITOR]) == 1)
                {
                    styleClassTitle = referenceStyle;
                    styleClassNumber = referenceStyleNB;
                }
                else
                {
                    styleClassTitle = L1;
                    styleClassNumber = L3;
                }
                #endregion

                if (Convert.ToDouble(tab[i, CstResult.PalmaresRecap.TOTAL_N]) != 0)
                {
                    t.Append("\r\n\t<tr>");
                    if (firstLine)
                    {
                        if (_session.ComparaisonCriterion == CstComparaisonCriterion.universTotal)
                        {
                            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", totalTitle, GestionWeb.GetWebWord(1188, _session.SiteLanguage));
                        }
                        else if (_session.ComparaisonCriterion == CstComparaisonCriterion.sectorTotal)
                        {
                            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", totalTitle, GestionWeb.GetWebWord(1189, _session.SiteLanguage));
                        }
                        else
                        {
                            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", totalTitle, GestionWeb.GetWebWord(1190, _session.SiteLanguage));
                        }
                        firstLine = false;
                        styleClassNumber = totalNb;
                    }
                    else
                    {
                        if (tab[i, PRODUCT] != null)
                            t.AppendFormat("<td class=\"{0}\"  nowrap>{1}</td>", styleClassTitle, tab[i, PRODUCT]);
                        else
                            t.AppendFormat("<td class=\"{0}\"  nowrap>&nbsp;</td>", styleClassTitle);
                    }

                    #region Total N
                    if (tab[i, TOTAL_N].Equals(0.0))
                    {
                        total = "&nbsp;";
                    }
                    else
                    {
                        total = FctUtilities.Units.ConvertUnitValueToString(tab[i, TOTAL_N], _session.Unit,fp);
                    }
                    t.AppendFormat("<td class=\"{0}\">{1}</td>", styleClassNumber, total);
                    #endregion

                    #region SOV
                    if (tab[i, SOV].Equals(0.0))
                    {
                        total = "&nbsp;";
                    }
                    else
                    {
                        total = FctUtilities.Units.ConvertUnitValueToString(tab[i, SOV], CstWeb.CustomerSessions.Unit.grp, fp);
                    }
                    t.AppendFormat("<td class=\"{0}\">{1}</td>", styleClassNumber, total);
                    #endregion

                    #region Cumul SOV
                    t.AppendFormat("<td class=\"{0}\">{1}</td>", styleClassNumber, FctUtilities.Units.ConvertUnitValueToString(tab[i, CUMUL_SOV], CstWeb.CustomerSessions.Unit.grp, fp));
                    #endregion

                    #region Rang
                    if (tab[i, RANK] != null)
                    {
                        t.AppendFormat("<td class=\"{0}\" >{1}</td>", styleClassNumber, tab[i, RANK]);
                    }
                    else
                    {
                        t.AppendFormat("<td class=\"{0}\" >&nbsp;</td>", styleClassNumber);
                    }
                    #endregion

                    #region Progression rank
                    if (typeYear == CstResult.PalmaresRecap.typeYearSelected.currentYear && _session.ComparativeStudy)
                    {
                        if (tab[i, PROGRESS_RANK] != null && !tab[i, PROGRESS_RANK].Equals(0))
                        {
                            t.AppendFormat("<td class=\"{0}\">{1}</td>", styleClassNumber, tab[i, PROGRESS_RANK]);
                        }
                        else
                        {
                            t.AppendFormat("<td class=\"{0}\"></td>", styleClassNumber);
                        }
                    }
                    #endregion

                    t.Append("</tr>");
                }

            }

            t.Append("</table>");

            return t;

        }

        /// <summary>
        /// Build the four reports
        /// </summary>		
        /// <returns>Top report</returns>
        protected StringBuilder BuildTops()
        {

            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);

            t.Append("<table >");

            #region Advertiser
            t.Append("<tr >");
            t.Append("<td valign=\"top\">");
            t.Append(this.BuildTop(CstResult.PalmaresRecap.typeYearSelected.currentYear, CstResult.MotherRecap.ElementType.advertiser));
            t.Append("</td>");
            if (_session.ComparativeStudy)
            {
                t.Append("<td>&nbsp;</td>");
                t.Append("<td valign=\"top\" >");
                t.Append(this.BuildTop(CstResult.PalmaresRecap.typeYearSelected.previousYear, CstResult.MotherRecap.ElementType.advertiser));
                t.Append("</td>");
            }
            t.Append("</tr>");
            #endregion

            t.Append("<tr><td></td></tr>");

            #region Produit
			if (_session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) {
				t.Append("<tr>");
				t.Append("<td valign=\"top\"  >");
				t.Append(this.BuildTop(CstResult.PalmaresRecap.typeYearSelected.currentYear, CstResult.MotherRecap.ElementType.product));
				t.Append("</td>");
				if (_session.ComparativeStudy) {
					t.Append("<td>&nbsp;</td>");
					t.Append("<td valign=\"top\" >");
					t.Append(this.BuildTop(CstResult.PalmaresRecap.typeYearSelected.previousYear, CstResult.MotherRecap.ElementType.product));
					t.Append("</td>");
				}
				t.Append("</tr>");
			}
            #endregion

            t.Append("</table>");

            return t;

        }
        #endregion

    }
}