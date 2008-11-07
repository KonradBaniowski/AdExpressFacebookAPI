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
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.ProductClassIndicators.DAL;

namespace TNS.AdExpressI.ProductClassIndicators.Engines
{
    /// <summary>
    /// Engine to build a Evolution report or to provide computed data for evolution report or indicator
    /// </summary>
    public class EngineEvolution:Engine
    {

        #region Constantes
        /// <summary>
        /// Index of product Id column
        /// </summary>
        public const int ID_PRODUCT_INDEX = 0;
        /// <summary>
        /// Index of product column
        /// </summary>
        public const int PRODUCT = 1;
        /// <summary>
        /// Index of total column
        /// </summary>
        public const int TOTAL_N = 2;
        /// <summary>
        /// Index of evol column
        /// </summary>
        public const int EVOLUTION = 3;
        /// <summary>
        /// Index of ecart column
        /// </summary>
        public const int ECART = 4;
        /// <summary>
        /// Index of ref/comp info column
        /// </summary>
        public const int COMPETITOR = 5;
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public EngineEvolution(WebSession session, IProductClassIndicatorsDAL dalLayer) : base(session, dalLayer) { }
        #endregion

        #region GetResult
        /// <summary>
        /// Get Evolution indicator as a table
        /// </summary>
        /// <returns>StringBuilder with HTML code</returns>
        public override StringBuilder GetResult()
        {
            return BuildEvolutions();
        }
        #endregion

        #region GetData
        /// <summary>
        /// Get data ready for display
        /// </summary>
        /// <param name="classifLevel">Classification detail (product or advertiser)</param>
        /// <returns></returns>
        public object[,] GetData(CstResult.MotherRecap.ElementType classifLevel)
        {

            #region Data loading
            DataSet ds = _dalLayer.GetEvolution(classifLevel); 
            DataTable dt = ds.Tables[0];
            #endregion

            #region Variables
            long i = 0;
            #endregion

            #region Result table init
            long nbline = dt.Rows.Count;
            object[,] tabResult = new object[nbline, COMPETITOR + 1];
			CultureInfo cInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
            #endregion

            #region Build Table
            Int64 idAdv = -1;
            double ecart = -1;
            double total = -1;

            foreach (DataRow currentRow in dt.Rows)
            {

                idAdv = Convert.ToInt64(currentRow["id_advertiser"]);
                total = Convert.ToDouble(currentRow["total_N"]);
                ecart = Convert.ToDouble(currentRow["Ecart"]);

                if (total == 0 && ecart == 0)
                {
                    continue;
                }

                #region Classif Info
                if (classifLevel == CstResult.MotherRecap.ElementType.product)
                {
                    tabResult[i, ID_PRODUCT_INDEX] = currentRow["id_product"];
                    tabResult[i, PRODUCT] = currentRow["product"];

                }
                else if (classifLevel == CstResult.MotherRecap.ElementType.advertiser)
                {
                    tabResult[i, ID_PRODUCT_INDEX] = idAdv;
                    tabResult[i, PRODUCT] = currentRow["advertiser"];
                }
                #endregion

                tabResult[i, TOTAL_N] = total;

                #region Reference or competitor?
                if (_referenceIDS.Contains(idAdv))
                {
                    tabResult[i, COMPETITOR] = 1;
                }
                else if (_competitorIDS.Contains(idAdv))
                {
                    tabResult[i, COMPETITOR] = 2;
                }
                else
                {
                    tabResult[i, COMPETITOR] = 0;
                }
                #endregion

                tabResult[i, ECART] = ecart;
                tabResult[i, EVOLUTION] = ecart / (total - ecart) * 100;

                i++;
            }
            #endregion

            if (i < 1)
            {
                return new object[0, COMPETITOR + 1]; 
            }

            return tabResult;        
        
        }
        #endregion

        #region Build report
        /// <summary>
        /// Build one report
        /// </summary>
        /// <param name="classifLevel">Detail level (advertiser or product)</param>
        /// <returns>Code HTML</returns>
        protected StringBuilder BuildEvolution(CstResult.MotherRecap.ElementType classifLevel)
        {

            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);

            #region styles
            string cssHeader = "p2";
            string cssNoneLabel = "p7";
            string cssNoneNb = "p9";
            string cssCompLabel = (!_excel) ? "p14" : "p142";
            string cssCompNb = (!_excel) ? "p141" : "p143";
            string cssRefLabel = "p15";
            string cssRefNb = "p151";
            string cssNoData = "mediumPurple1";
            #endregion


            object[,] tab = this.GetData(classifLevel);
            string periodLabel = string.Format("{0}-{1}",FctUtilities.Dates.DateToString( _periodBegin.Date ,_session.SiteLanguage), FctUtilities.Dates.DateToString(_periodEnd.Date,_session.SiteLanguage));
            long last = tab.GetLongLength(0) - 1;
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

            #region No Data
            if (tab.GetLongLength(0) == 0)
            {
                t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
                t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                if (classifLevel == CstResult.MotherRecap.ElementType.advertiser)
                {
                    t.AppendFormat("{0}<br>{1} {2}", GestionWeb.GetWebWord(177, _session.SiteLanguage), GestionWeb.GetWebWord(2474, _session.SiteLanguage), periodLabel);
                }
                t.Append("</td></tr></table>");
                return t;

            }
            #endregion

            t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");

            #region Headers
            t.Append("\r\n\t<tr height=\"30px\" >");

            #region Rise
            if (classifLevel == CstResult.MotherRecap.ElementType.product)
            {
                t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\" align=\"center\">{1}<br>{2}</td>", cssHeader, GestionWeb.GetWebWord(1315, _session.SiteLanguage), periodLabel);
            }
            else
            {
                t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\" align=\"center\">{1}<br>{2}</td>", cssHeader, GestionWeb.GetWebWord(1210, _session.SiteLanguage), periodLabel);
            }
            t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1170, _session.SiteLanguage));
            t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1212, _session.SiteLanguage));
            t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1213, _session.SiteLanguage));
            #endregion

            t.Append("<td>&nbsp;</td>");

            #region Decrease
            if (classifLevel == CstResult.MotherRecap.ElementType.product)
            {
                t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\" align=\"center\">{1}<br>{2}</td>", cssHeader, GestionWeb.GetWebWord(1209, _session.SiteLanguage), periodLabel);
            }
            else
            {
                t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\" align=\"center\">{1}<br>{2}</td>", cssHeader, GestionWeb.GetWebWord(1211, _session.SiteLanguage), periodLabel);
            }
            t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1170, _session.SiteLanguage));
            t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1212, _session.SiteLanguage));
            t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1213, _session.SiteLanguage));
            #endregion

            t.Append("</tr>");
            #endregion

            #region Table building
            string cssLabel = string.Empty;
            string cssNb = string.Empty;
            int typeElt = 0;
            double evol = 0;
            double total = 0;
            double ecart = 0;
            for (int i = 0; i < tab.GetLongLength(0) && i < 10; i++)
            {

                #region Style
                cssLabel = cssNoneLabel;
                cssNb = cssNoneNb;
                if (tab[i, COMPETITOR] != null)
                {
                    typeElt = Convert.ToInt32(tab[i, COMPETITOR]);
                    if (typeElt == 2)
                    {
                        cssLabel = cssCompLabel;
                        cssNb = cssCompNb;
                    }
                    else if (typeElt == 1)
                    {
                        cssLabel = cssRefLabel;
                        cssNb = cssRefNb;
                    }
                }
                #endregion

                t.Append("\r\n\t<tr>");

                #region Rise
                ecart = Convert.ToDouble(tab[i, ECART]);
                if (ecart > 0)
                {
                    evol = Convert.ToDouble(tab[i, EVOLUTION]);
                    total = Convert.ToDouble(tab[i, TOTAL_N]);
                    t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssLabel, tab[i, PRODUCT]);
                    t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(total, _session.Unit, fp));
                    if (!Double.IsInfinity(evol))
                    {
                        t.AppendFormat("<td class=\"{0}\" nowrap>{1}%</td>", cssNb, FctUtilities.Units.ConvertUnitValueAndPdmToString(evol, _session.Unit, true, fp));
                    }
                    else
                    {
                        t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, GestionWeb.GetWebWord(1214, _session.SiteLanguage));
                    }
                    t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(ecart, _session.Unit, fp));
                }
                else
                {
                    if (i != 10)
                    {
                        t.AppendFormat("<td class=\"{0} whiteRightBorder\" colspan=4></td>", cssNoData);
                    }
                    else
                    {
                        t.AppendFormat("<td class=\"{0} whiteRightBottomBorder\" colspan=4></td>", cssNoData);
                    }

                }
                #endregion

                t.Append("<td>&nbsp;</td>");

                #region Style
                cssLabel = cssNoneLabel;
                cssNb = cssNoneNb;
                if (tab[last, COMPETITOR] != null)
                {
                    typeElt = Convert.ToInt32(tab[last, COMPETITOR]);
                    if (typeElt == 2)
                    {
                        cssLabel = cssCompLabel;
                        cssNb = cssCompNb;
                    }
                    else if (typeElt == 1)
                    {
                        cssLabel = cssRefLabel;
                        cssNb = cssRefNb;
                    }
                }
                #endregion

                #region Decrease
                ecart = Convert.ToDouble(tab[last, ECART]);
                if (ecart < 0)
                {
                    evol = Convert.ToDouble(tab[last, EVOLUTION]);
                    total = Convert.ToDouble(tab[last, TOTAL_N]);
                    t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssLabel, tab[last, PRODUCT]);
                    t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(total, _session.Unit, fp));
                    if (!Double.IsInfinity(evol))
                    {
                        t.AppendFormat("<td class=\"{0}\" nowrap>{1}%</td>", cssNb, FctUtilities.Units.ConvertUnitValueAndPdmToString(evol, _session.Unit, true, fp));
                    }
                    else
                    {
                        t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, GestionWeb.GetWebWord(1214, _session.SiteLanguage));
                    }
                    t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(ecart, _session.Unit, fp));
                }
                else
                {
                    if (i != 10)
                    {
                        t.AppendFormat("<td class=\"{0} whiteRightBorder\" colspan=4></td>", cssNoData);
                    }
                    else
                    {
                        t.AppendFormat("<td class=\"{0} whiteRightBottomBorder\" colspan=4></td>", cssNoData);
                    }

                }
                #endregion

                t.Append("\r\n\t</tr>");

                last--;
            }
            #endregion

            t.Append("</table>");

            return t;

        }

        /// <summary>
        /// Build the four reports
        /// </summary>		
        /// <returns>Top report</returns>
        protected StringBuilder BuildEvolutions()
        {

            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
            DateTime periodBegin = FctUtilities.Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);

            #region if year N-2
            if (DateTime.Now.Year > _session.DownLoadDate)
            {
                if (periodBegin.Year.Equals(System.DateTime.Now.Year - 3))
                {
                    t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
                    t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                    t.Append(GestionWeb.GetWebWord(177, _session.SiteLanguage));
                    t.Append("</td></tr></table>");
                    return t;
                }
            }
            else
            {
                if (periodBegin.Year.Equals(System.DateTime.Now.Year - 2))
                {
                    t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
                    t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                    t.Append(GestionWeb.GetWebWord(177, _session.SiteLanguage));
                    t.Append("</td></tr></table>");
                    return t;
                }
            }
            #endregion

            t.Append("<table class=\"tableFont\">");

            #region Advertiser
            t.Append("<tr>");
            t.Append("<td valign=\"top\">");
            t.Append(this.BuildEvolution(CstResult.MotherRecap.ElementType.advertiser));
            t.Append("</td>");
            t.Append("</tr>");
            #endregion

            if (_excel)
            {
                t.Append("<tr>");
                t.Append("<td valign=\"top\">&nbsp;");
                t.Append("</td>");
                t.Append("</tr>");

            }

            #region Produit
			if (_session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) {
				t.Append("<tr>");
				t.Append("<td valign=\"top\">");
				t.Append(this.BuildEvolution(CstResult.MotherRecap.ElementType.product));
				t.Append("</td>");
				t.Append("</tr>");
			}
            #endregion

            t.Append("</table>");

            return t;

        }
        #endregion

    }
}