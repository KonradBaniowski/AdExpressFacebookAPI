using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using System.Data;

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpressI.ProductClassIndicators.Exceptions;
using TNS.AdExpress.Domain.Web;
using System.Globalization;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Translation;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using CstComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion;
using FctIndicators = TNS.AdExpressI.ProductClassIndicators.Russia.Functions;
using TNS.FrameWork.WebResultUI.Functions;
namespace TNS.AdExpressI.ProductClassIndicators.Russia.Engines{
    /// <summary>
    /// Engine to build a Top report or to provide computed data for top report or indicator
    /// This class contains specific rules for the russian version
    /// </summary>
    public class EngineTop : TNS.AdExpressI.ProductClassIndicators.Engines.EngineTop {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public EngineTop(WebSession session, IProductClassIndicatorsDAL dalLayer) : base(session, dalLayer) { }
        #endregion

        #region GetData
        /// <summary>
        /// Get data ready for display
        /// </summary>
        /// <param name="typeYear">Type of study (current or previous year)</param>
        /// <param name="classifLevel">Classification detail (product or advertiser)</param>
        /// <returns></returns>
        override public object[,] GetData(CstResult.PalmaresRecap.typeYearSelected typeYear, CstResult.MotherRecap.ElementType classifLevel){

            #region Data loading
            DataSet ds = _dalLayer.GetTops(typeYear, classifLevel);
            DataTable dt;
            string idProductList = "";
            #endregion

            // Check if there is no data
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0){
                return null;
            }
            dt = ds.Tables[0];
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo;


            #region Init Result Table
            int nbLine = Math.Min(dt.Rows.Count , TOPS_NUMBER + 1);
            object[,] tabResult = new object[nbLine, COMPETITOR + 1];
            long idLev = -1;
            long idAdv = -1;
            DataRow row = null;
            DataColumn column = null;
            string strIdColName = (classifLevel == CstResult.MotherRecap.ElementType.advertiser) ? "ID_ADVERTISER" : "ID_PRODUCT";
            string strLabColName = (classifLevel == CstResult.MotherRecap.ElementType.advertiser) ? "TOP_ADVERTISER" : "TOP_PRODUCT";
            #endregion

            #region Check if there is an empty cell in DataSet
            for (int i = 0; i < dt.Rows.Count; i++){

                row = dt.Rows[i];

                for (int j = 0; j < dt.Columns.Count; j++){

                    if ((i == 0 && (j < dt.Columns.Count - 2)) || i > 0){

                        column = row.Table.Columns[j];
                        if (row[j] == null)
                            throw new ProductClassIndicatorsException("The filed " + column.ColumnName + " can't be empty !!!");
                    }
                }
            }
            #endregion

            #region Building table result
            // Total row
            row = dt.Rows[0];

            tabResult[0, ID_PRODUCT_INDEX] = Convert.ToInt64(row[strIdColName]);
            tabResult[0, PRODUCT] = row[strLabColName].ToString();
            if (!FctIndicators.IsNull(row["UNIT"])) tabResult[0, TOTAL_N] = FctIndicators.ConvertToDouble(row["UNIT"], fp);
            if (!FctIndicators.IsNull(row["SOV"])) tabResult[0, SOV] = FctIndicators.ConvertToDouble(row["SOV"], fp);
            if (!FctIndicators.IsNull(row["SOV_CUMUL"])) tabResult[0, CUMUL_SOV] = FctIndicators.ConvertToDouble(row["SOV_CUMUL"], fp);

            // Table result rows
            for (int i = 1; i < nbLine; i++) {

                row = dt.Rows[i];

                if (classifLevel == CstResult.MotherRecap.ElementType.advertiser)
                    idAdv = Convert.ToInt64(row[strIdColName]);
                else
                    idLev = Convert.ToInt64(row[strIdColName]);

                tabResult[i, ID_PRODUCT_INDEX] = Convert.ToInt64(row[strIdColName]);
                tabResult[i, PRODUCT] = row[strLabColName].ToString();
                if (!FctIndicators.IsNull(row["UNIT"])) tabResult[i, TOTAL_N] = FctIndicators.ConvertToDouble(row["UNIT"], fp);
                if (!FctIndicators.IsNull(row["SOV"])) tabResult[i, SOV] = FctIndicators.ConvertToDouble(row["SOV"], fp);
                if (!FctIndicators.IsNull(row["SOV_CUMUL"])) tabResult[i, CUMUL_SOV] = FctIndicators.ConvertToDouble(row["SOV_CUMUL"], fp);
                if (!FctIndicators.IsNull(row["RANKING"])) tabResult[i, RANK] = Convert.ToInt32(row["RANKING"]);
                if (!FctIndicators.IsNull(row["CHANGE_RANKING"])) tabResult[i, PROGRESS_RANK] = FctIndicators.ConvertToDouble(row["CHANGE_RANKING"], fp);

                if (classifLevel == CstResult.MotherRecap.ElementType.advertiser){
                    if (_competitorIDS.Contains(idAdv)){
                        tabResult[i, COMPETITOR] = CstWeb.AdvertiserPersonalisation.Type.competitor;
                    }
                    else if (_referenceIDS.Contains(idAdv)){
                        tabResult[i, COMPETITOR] = CstWeb.AdvertiserPersonalisation.Type.reference;
                    }
                    else{
                        tabResult[i, COMPETITOR] = CstWeb.AdvertiserPersonalisation.Type.none;
                    }
                }
                else{
                    idProductList += idLev + ",";
                }
            }

            if (classifLevel == CstResult.MotherRecap.ElementType.product){
                if (idProductList != null && idProductList.Length > 0){
                    idProductList = idProductList.Substring(0, idProductList.Length - 1);
                    SetAdvertiserPersonalisation(tabResult, idProductList, nbLine, typeYear);
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
        protected override StringBuilder BuildTop(CstResult.PalmaresRecap.typeYearSelected typeYear, CstResult.MotherRecap.ElementType classifLevel)
        {

            #region Variables
            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
            bool firstLine = true;
            string styleClassTitle = "";
            string styleClassNumber = "";
            string total = "";
            string periodDate = "";
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo;
            #endregion

            #region Constantes
            const string P2 = "p2";
            //Labels
            const string L1 = "p7";
            const string competitorStyle = "p14";
            const string competitorExcelStyle = "p142";
            const string referenceStyle = "p15";
            const string mixedStyle = "p16";
            //Numbers
            const string L3 = "p9";
            const string competitorStyleNB = "p141";
            const string competitorExcelStyleNB = "p143";

            const string referenceStyleNB = "p151";
            const string mixedStyleNB = "p161";
            // Total
            const string totalTitle = "pmtotal";
            const string totalNb = "pmtotalnb";
            #endregion

            object[,] tab = this.GetData(typeYear, classifLevel);

            CultureInfo cInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
            TNS.AdExpress.Domain.Web.TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
            DateTimeFormatInfo myDTFI = cInfo.DateTimeFormat;
            UnitInformation selectedCurrency = _session.GetSelectedUnit();

            if (typeYear == CstResult.PalmaresRecap.typeYearSelected.currentYear)
            {
                periodDate = string.Format("{0}-{1}", FctUtilities.Dates.DateToString(_periodBegin.Date, _session.SiteLanguage), FctUtilities.Dates.DateToString(_periodEnd.Date, _session.SiteLanguage));
            }
            else
            {
                periodDate = string.Format("{0}-{1}", FctUtilities.Dates.DateToString(_periodBegin.Date.AddYears(-1), _session.SiteLanguage), FctUtilities.Dates.DateToString(_periodEnd.Date.AddYears(-1), _session.SiteLanguage));
            }

            if (tab == null || tab.GetLongLength(0) == 1 || FctIndicators.IsNull(tab[0, TOTAL_N]))
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
            t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\"  align=\"center\">{1}</td>", P2, GestionWeb.GetWebWord(1401, _session.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_session.SiteLanguage) + ")");
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
                if (tab[i, CstResult.PalmaresRecap.COMPETITOR] != null && Convert.ToInt32(tab[i, CstResult.PalmaresRecap.COMPETITOR]) == 3)
                {
                    styleClassTitle = mixedStyle;
                    styleClassNumber = mixedStyleNB;
                }
                else if (tab[i, CstResult.PalmaresRecap.COMPETITOR] != null && Convert.ToInt32(tab[i, CstResult.PalmaresRecap.COMPETITOR]) == 2)
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

                if (!FctIndicators.IsNull(tab[i, CstResult.PalmaresRecap.TOTAL_N]))
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
                        if (!FctIndicators.IsNull(tab[i, PRODUCT])) {
                            if(_excel)
                                t.AppendFormat("<td class=\"{0}\"  nowrap>{1}</td>", styleClassTitle, String.Format("{0}", tab[i, PRODUCT]));
                            else
                                t.AppendFormat("<td class=\"{0}\"  nowrap>{1}</td>", styleClassTitle, TNS.FrameWork.WebResultUI.Functions.TextWrap.WrapHtml(String.Format("{0}", tab[i, PRODUCT]), textWrap.NbChar, textWrap.Offset));
                        }
                        else
                            t.AppendFormat("<td class=\"{0}\"  nowrap>&nbsp;</td>", styleClassTitle);
                    }

                    #region Total N
                    if (FctIndicators.IsNull(tab[i, TOTAL_N]))
                    {
                        total = "&nbsp;";
                    }
                    else
                    {
                        total = FctIndicators.ConvertUnitValueToString(tab[i, TOTAL_N], _session.Unit, fp);
                    }
                    t.AppendFormat("<td class=\"{0}\">{1}</td>", styleClassNumber, total);
                    #endregion

                    #region SOV
                    if (FctIndicators.IsNull(tab[i, SOV]))
                    {
                        total = "&nbsp;";
                    }
                    else
                    {
                        total = string.Format(fp, "{0:percentWOSign}", tab[i, SOV]);
                    }
                    t.AppendFormat("<td class=\"{0}\">{1}</td>", styleClassNumber, total);
                    #endregion

                    #region Cumul SOV
                     if (FctIndicators.IsNull(tab[i, CUMUL_SOV]))                    
                    t.AppendFormat("<td class=\"{0}\">{1}</td>", styleClassNumber, "&nbsp;");
                      else t.AppendFormat("<td class=\"{0}\">{1}</td>", styleClassNumber, string.Format(fp, "{0:percentWOSign}", tab[i, CUMUL_SOV]));
                    #endregion

                    #region Rang
                    if (!FctIndicators.IsNull(tab[i, RANK]))
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
                        if (!FctIndicators.IsNull(tab[i, PROGRESS_RANK]))
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

       
        #endregion		

    }
}
