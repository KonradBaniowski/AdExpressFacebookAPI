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
using TNS.AdExpress.Domain.Units;

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
        virtual public object[,] GetData(CstResult.MotherRecap.ElementType classifLevel)
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

                if (classifLevel == CstResult.MotherRecap.ElementType.advertiser) 
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
				if (classifLevel == CstResult.MotherRecap.ElementType.advertiser && _referenceIDS.Contains(idAdv))
                {
                    tabResult[i, COMPETITOR] = 1;
                }
				else if (classifLevel == CstResult.MotherRecap.ElementType.advertiser && _competitorIDS.Contains(idAdv))
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

            object[,] tab = new object[i, COMPETITOR + 1];
            Array.Copy(tabResult, tab, i*(tabResult.GetLength(1)));

            SetAdvertiserPersonalisation(tab, classifLevel);

            return tab;        
        
        }
        #endregion

        #region Build report
        /// <summary>
        /// Build one report
        /// </summary>
        /// <param name="classifLevel">Detail level (advertiser or product)</param>
        /// <returns>Code HTML</returns>
        protected virtual StringBuilder BuildEvolution(CstResult.MotherRecap.ElementType classifLevel)
        {

            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
			bool inRef = false, inComp = false;
            UnitInformation selectedCurrency =  GetUnit();

            #region styles
            string cssHeader = "p2";
            string cssNoneLabel = "p7";
            string cssNoneNb = "p9";
            string cssCompLabel = (!_excel) ? "p14" : "p142";
            string cssCompNb = (!_excel) ? "p141" : "p143";
            string cssRefLabel = "p15";
			const string cssMixedLabel = "p16";
            string cssRefNb = "p151";
			string cssMixedNb = "p161";
            string cssNoData = "mediumPurple1";
            #endregion

            string periodLabel = string.Format("{0}-{1}", FctUtilities.Dates.DateToString(_periodBegin.Date, _session.SiteLanguage), FctUtilities.Dates.DateToString(_periodEnd.Date, _session.SiteLanguage));
            object[,] tab = this.GetData(classifLevel);
            bool containsDataRise = ContainsDataRise(tab);
            bool containsDataDecrease = ContainsDataDecrease(tab);

            #region No Data
            if (tab == null || tab.GetLongLength(0) == 0)
            {
                t.Append("<table class=\"backGroundWhite\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
                t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                if (classifLevel == CstResult.MotherRecap.ElementType.advertiser)
                {
                    t.AppendFormat("{0}", GestionWeb.GetWebWord(177, _session.SiteLanguage));
                }
                t.Append("</td></tr></table>");
                return t;

            }
            #endregion

            long last = tab.GetLongLength(0) - 1;
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
			

			t.Append("<table class=\"backGroundWhite\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");

            #region Headers
            t.Append("\r\n\t<tr height=\"30px\" >");

            #region Rise
            if (containsDataRise) {
                if (classifLevel == CstResult.MotherRecap.ElementType.product) {
                    t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\" align=\"center\">{1}<br>{2}</td>", cssHeader, GestionWeb.GetWebWord(1315, _session.SiteLanguage), periodLabel);
                }
                else {
                    t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\" align=\"center\">{1}<br>{2}</td>", cssHeader, GestionWeb.GetWebWord(1210, _session.SiteLanguage), periodLabel);
                }
                t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1401, _session.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_session.SiteLanguage) + ")");
                if (_evolution) {
                    t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1212, _session.SiteLanguage));
                    t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1213, _session.SiteLanguage));
                }
            }
            else {
                t.Append("<td rowspan=\"11\" style=\"width:200px; vertical-align:top;\"><table class=\"backGroundWhite\" border=0 cellpadding=0 cellspacing=0 width=\"100%\"><tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                if (classifLevel == CstResult.MotherRecap.ElementType.advertiser) {
                    t.AppendFormat("{0}", GestionWeb.GetWebWord(177, _session.SiteLanguage));
                }
                else
                    t.Append("&nbsp;");
                t.Append("</td></tr></table></td>");
            }
            #endregion

            t.Append("<td>&nbsp;</td>");

            #region Decrease
            if (containsDataDecrease) {
                if (classifLevel == CstResult.MotherRecap.ElementType.product) {
                    t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\" align=\"center\">{1}<br>{2}</td>", cssHeader, GestionWeb.GetWebWord(1209, _session.SiteLanguage), periodLabel);
                }
                else {
                    t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\" align=\"center\">{1}<br>{2}</td>", cssHeader, GestionWeb.GetWebWord(1211, _session.SiteLanguage), periodLabel);
                }
                t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1401, _session.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_session.SiteLanguage) + ")");
                if (_evolution) {
                    t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1212, _session.SiteLanguage));
                    t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1213, _session.SiteLanguage));
                }
            }
            else {
                t.Append("<td rowspan=\"11\" style=\"width:200px; vertical-align:top;\"><table class=\"backGroundWhite\" border=0 cellpadding=0 cellspacing=0 width=\"100%\"><tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                if (classifLevel == CstResult.MotherRecap.ElementType.advertiser) {
                    t.AppendFormat("{0}", GestionWeb.GetWebWord(177, _session.SiteLanguage));
                }
                else
                    t.Append("&nbsp;");
                t.Append("</td></tr></table></td>");
            }
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
					if (typeElt == 3) {
						cssLabel = cssMixedLabel;
						cssNb = cssMixedNb;
					}
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
                if (containsDataRise) {
                    ecart = ConvertToDouble(tab[i, ECART]);
                    if (ecart > 0) {
                        evol = ConvertToDouble(tab[i, EVOLUTION]);
                        total = ConvertToDouble(tab[i, TOTAL_N]);
                        t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssLabel, tab[i, PRODUCT]);
                        t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(total, _session.Unit, fp));
                        if (_evolution) {
                            if (!Double.IsInfinity(evol)) {
                                t.AppendFormat("<td class=\"{0}\" nowrap>{1}%</td>", cssNb, FctUtilities.Units.ConvertUnitValueAndPdmToString(evol, _session.Unit, true, fp));
                            }
                            else {
                                t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, GestionWeb.GetWebWord(1214, _session.SiteLanguage));
                            }

                            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(ecart, _session.Unit, fp));
                        }
                    }
                    else {
                        if (i != 10) {
                            t.AppendFormat("<td class=\"{0} whiteRightBorder\" colspan=4></td>", cssNoData);
                        }
                        else {
                            t.AppendFormat("<td class=\"{0} whiteRightBottomBorder\" colspan=4></td>", cssNoData);
                        }

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
					if (typeElt == 3) {
						cssLabel = cssMixedLabel;
						cssNb = cssMixedNb;
					}
                    else if (typeElt == 2)
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
                if (containsDataDecrease) {
                    ecart = ConvertToDouble(tab[last, ECART]);
                    if (ecart < 0) {
                        evol = ConvertToDouble(tab[last, EVOLUTION]);
                        total = ConvertToDouble(tab[last, TOTAL_N]);
                        t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssLabel, tab[last, PRODUCT]);
                        t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(total, _session.Unit, fp));
                        if (_evolution) {
                            if (!Double.IsInfinity(evol)) {
                                t.AppendFormat("<td class=\"{0}\" nowrap>{1}%</td>", cssNb, FctUtilities.Units.ConvertUnitValueAndPdmToString(evol, _session.Unit, true, fp));
                            }
                            else {
                                t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, GestionWeb.GetWebWord(1214, _session.SiteLanguage));
                            }

                            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(ecart, _session.Unit, fp));
                        }
                    }
                    else {
                        if (i != 10) {
                            t.AppendFormat("<td class=\"{0} whiteRightBorder\" colspan=4></td>", cssNoData);
                        }
                        else {
                            t.AppendFormat("<td class=\"{0} whiteRightBottomBorder\" colspan=4></td>", cssNoData);
                        }

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
        protected virtual StringBuilder BuildEvolutions()
        {

            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
            DateTime periodBegin = FctUtilities.Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            
			#region if year N-2
            if (DateTime.Now.Year > _session.DownLoadDate)
            {
                if (periodBegin.Year.Equals(System.DateTime.Now.Year - WebApplicationParameters.DataNumberOfYear))
                {
                    t.Append("<table class=\"backGroundWhite\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
                    t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                    t.Append(GestionWeb.GetWebWord(177, _session.SiteLanguage));
                    t.Append("</td></tr></table>");
                    return t;
                }
            }
            else
            {
                if (periodBegin.Year.Equals(System.DateTime.Now.Year - (WebApplicationParameters.DataNumberOfYear-1)))
                {
                    t.Append("<table class=\"backGroundWhite\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
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
            t.Append(BuildEvolution(CstResult.MotherRecap.ElementType.advertiser));
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
				t.Append(BuildEvolution(CstResult.MotherRecap.ElementType.product));
				t.Append("</td>");
				t.Append("</tr>");
			}
            #endregion

            t.Append("</table>");

            return t;

        }
        #endregion		

        #region SetAdvertiserPersonalisation
        /// <summary>
        /// Set advertiser personnalisation
        /// </summary>
        /// <param name="tab">tab Result</param>
        /// <param name="classifLevel">Classif Level</param>
        protected void SetAdvertiserPersonalisation(object[,] tab, CstResult.MotherRecap.ElementType classifLevel){

            bool inRef = false, inComp = false, inneutral = false;
            int nbTypes = 0;
            Int64 idAdv;

            if (classifLevel == CstResult.MotherRecap.ElementType.product && tab != null && tab.GetLongLength(0) > 0){

                string idProductList = "";
                long idProd = -1;
                List<long> temp = null;
                DataSet res = null;
                idAdv = -1;
                long last = tab.GetLongLength(0) - 1;

                for (int m = 0; m < tab.GetLongLength(0) && m < 10; m++){
                    if (tab[m, ID_PRODUCT_INDEX] != null)
                        idProductList += tab[m, ID_PRODUCT_INDEX].ToString() + ",";
                    if (tab[last, ID_PRODUCT_INDEX] != null)
                        idProductList += tab[last, ID_PRODUCT_INDEX].ToString() + ",";
                    last--;
                }

                last = tab.GetLongLength(0) - 1;
                
                if (idProductList != null && idProductList.Length > 0){

                    idProductList = idProductList.Substring(0, idProductList.Length - 1);
                    res = _dalLayer.GetProductsPersonnalisationType(idProductList, _strYearID);
                    if (res != null && res.Tables[0] != null && res.Tables[0].Rows.Count > 0){

                        for (int j = 0; j < tab.GetLongLength(0) && j < 10; j++){

                            tab[j, COMPETITOR] = CstWeb.AdvertiserPersonalisation.Type.none;
                            if (tab[j, ID_PRODUCT_INDEX] != null && tab[j, EngineEvolution.ID_PRODUCT_INDEX].ToString().Length > 0){
                                idProd = long.Parse(tab[j, ID_PRODUCT_INDEX].ToString());
                                foreach (DataRow row in res.Tables[0].Rows){
                                    if (long.Parse(row["id_product"].ToString()) == idProd){
                                        if (Convert.ToInt32(row["inref"]) > 0){
                                            nbTypes++;
                                            inRef = true;
                                        }
                                        if (Convert.ToInt32(row["incomp"]) > 0){
                                            nbTypes++;
                                            inComp = true;
                                        }
                                        if (Convert.ToInt32(row["inneutral"]) > 0){
                                            nbTypes++;
                                            inneutral = true;
                                        }

                                        if (nbTypes > 1)
                                            tab[j, COMPETITOR] = CstWeb.AdvertiserPersonalisation.Type.mixed;
                                        else if (inComp)
                                            tab[j, COMPETITOR] = CstWeb.AdvertiserPersonalisation.Type.competitor;
                                        else if (inRef)
                                            tab[j, COMPETITOR] = CstWeb.AdvertiserPersonalisation.Type.reference;
                                        nbTypes = 0;
                                        inRef = false;
                                        inComp = false;
                                        inneutral = false;
                                        break;
                                    }
                                }
                                nbTypes = 0;
                                inRef = false;
                                inComp = false;
                                inneutral = false;
                            }
                            if (tab[last, ID_PRODUCT_INDEX] != null && tab[last, ID_PRODUCT_INDEX].ToString().Length > 0){
                                idProd = long.Parse(tab[last, ID_PRODUCT_INDEX].ToString());
                                foreach (DataRow row in res.Tables[0].Rows){
                                    if (long.Parse(row["id_product"].ToString()) == idProd){
                                        if (Convert.ToInt32(row["inref"]) > 0){
                                            nbTypes++;
                                            inRef = true;
                                        }
                                        if (Convert.ToInt32(row["incomp"]) > 0){
                                            nbTypes++;
                                            inComp = true;
                                        }
                                        if (Convert.ToInt32(row["inneutral"]) > 0){
                                            nbTypes++;
                                            inneutral = true;
                                        }

                                        if (nbTypes > 1)
                                            tab[last, COMPETITOR] = CstWeb.AdvertiserPersonalisation.Type.mixed;
                                        else if (inComp)
                                            tab[last, COMPETITOR] = CstWeb.AdvertiserPersonalisation.Type.competitor;
                                        else if (inRef)
                                            tab[last, COMPETITOR] = CstWeb.AdvertiserPersonalisation.Type.reference;
                                        nbTypes = 0;
                                        inRef = false;
                                        inComp = false;
                                        inneutral = false;
                                        break;
                                    }
                                }
                                nbTypes = 0;
                                inRef = false;
                                inComp = false;
                                inneutral = false;
                            }
                            last--;
                            inRef = false;
                            inComp = false;
                        }
                    }
                }
                last = tab.GetLongLength(0) - 1;
            }

        }
        #endregion

        #region ConvertToDouble
        /// <summary>
        /// Convert to double according to culture info
        /// </summary>
        /// <param name="o">Object to convert</param>
        /// <returns>double</returns>
        protected virtual double ConvertToDouble(object o) {

            return Convert.ToDouble(o);
        }
        #endregion

        #region Contains Data Rise
        /// <summary>
        /// Contains Data Rise
        /// </summary>
        /// <param name="tab">Tab</param>
        /// <returns>True if contains data</returns>
        private bool ContainsDataRise(object[,] tab) {

            for (int i = 0; i < tab.GetLongLength(0) && i < 10; i++)
                if (ConvertToDouble(tab[i, ECART]) > 0)
                    return true;

            return false;
        }
        #endregion

        #region Contains Data Decrease
        /// <summary>
        /// Contains Data Decrease
        /// </summary>
        /// <param name="tab">Tab</param>
        /// <returns>True if contains data</returns>
        private bool ContainsDataDecrease(object[,] tab) {

            int last = Convert.ToInt32(tab.GetLongLength(0) - 1);

            for (int i = last; i > last-10 && i >=0 ; i--)
                if (ConvertToDouble(tab[i, ECART]) < 0)
                    return true;

            return false;
        }
        #endregion

    }
}
