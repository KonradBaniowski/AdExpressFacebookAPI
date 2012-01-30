using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using System.Data;

using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpressI.ProductClassIndicators.Exceptions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using FctIndicators = TNS.AdExpressI.ProductClassIndicators.Russia.Functions;
using TNS.AdExpress.Domain.Units;
using System.Globalization;
using TNS.FrameWork.WebResultUI.Functions;

namespace TNS.AdExpressI.ProductClassIndicators.Russia.Engines{
    /// <summary>
    /// Engine to build a Evolution report or to provide computed data for evolution report or indicator
    /// This class contains specific rules for the russian version
    /// </summary>
    public class EngineEvolution : TNS.AdExpressI.ProductClassIndicators.Engines.EngineEvolution
    {

        #region Constantes
        /// <summary>
        /// Top classfication levels
        /// </summary>
        private const int TOP_CLASSIFICATION_LEVELS = 20;
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public EngineEvolution(WebSession session, IProductClassIndicatorsDAL dalLayer) : base(session, dalLayer) { }
        #endregion

        #region GetData
        /// <summary>
        /// Get data ready for display
        /// </summary>
        /// <param name="classifLevel">Classification detail (product or advertiser)</param>
        /// <returns></returns>
        public override object[,] GetData(CstResult.MotherRecap.ElementType classifLevel)
        {

            #region Data loading
            DataSet ds = _dalLayer.GetEvolution(classifLevel);
            DataTable dt;
            DataRow row = null;
            DataColumn column = null;
            #endregion

            // Check if there is no data
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0){
                return null;
            }

            dt = ds.Tables[0];

            #region Result table init
            long nbline = dt.Rows.Count;
            object[,] tabResult = new object[TOP_CLASSIFICATION_LEVELS, COMPETITOR + 1];
            #endregion

            #region Check if there is an empty cell in DataSet
            for (int k = 0; k < dt.Rows.Count; k++){

                row = dt.Rows[k];

                for (int j = 0; j < dt.Columns.Count; j++){
                        column = row.Table.Columns[j];
                        if (row[j] == null)
                            throw new ProductClassIndicatorsException("The filed " + column.ColumnName + " can't be empty !!!");
                }
            }
            #endregion

            #region Build Table
            BuildTable(ds.Tables[0], tabResult, classifLevel, true);
            BuildTable(ds.Tables[1], tabResult, classifLevel, false);

            SetAdvertiserPersonalisation(tabResult, classifLevel);
            #endregion

            return tabResult;

        }
        #endregion

        #region BuildTable
        /// <summary>
        /// Build table
        /// </summary>
        /// <param name="dt">Data table</param>
        /// <param name="tabResult">Result table</param>
        /// <param name="classifLevel">Classification level</param>
        /// <param name="increase">Building type</param>
        private void BuildTable(DataTable dt, object[,] tabResult, CstResult.MotherRecap.ElementType classifLevel, bool increase){

            Int64 idAdv = -1;
            int i = 0;
            string strIdColName = (classifLevel == CstResult.MotherRecap.ElementType.advertiser) ? "ID_ADVERTISER" : "ID_PRODUCT";
            string strLabColName = (classifLevel == CstResult.MotherRecap.ElementType.advertiser) ? "TOP_ADVERTISER" : "TOP_PRODUCT";

            if (!increase) i = TOP_CLASSIFICATION_LEVELS-1;

            foreach (DataRow currentRow in dt.Rows){

                #region Classification
                if (classifLevel == CstResult.MotherRecap.ElementType.advertiser)
                    idAdv = Convert.ToInt64(currentRow[strIdColName]);

                if (classifLevel == CstResult.MotherRecap.ElementType.product)
                {
                    tabResult[i, ID_PRODUCT_INDEX] = currentRow[strIdColName];
                    tabResult[i, PRODUCT] = currentRow[strLabColName];

                }
                else if (classifLevel == CstResult.MotherRecap.ElementType.advertiser)
                {
                    tabResult[i, ID_PRODUCT_INDEX] = idAdv;
                    tabResult[i, PRODUCT] = currentRow[strLabColName];
                }
                #endregion

                tabResult[i, TOTAL_N] = currentRow["UNIT"];

                if (!FctIndicators.IsNull(currentRow["CHANGE"]) && currentRow["CHANGE"].ToString().Equals("NEW"))
                    tabResult[i, EVOLUTION] = double.PositiveInfinity;
                else
                    tabResult[i, EVOLUTION] = currentRow["CHANGE"];

                tabResult[i, ECART] = currentRow["DIFFERENCE"];

                #region Reference or competitor
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

                if (increase) i++;
                else i--;
            }
        
        }
        #endregion

        #region Build report
        /// <summary>
        /// Build one report
        /// </summary>
        /// <param name="classifLevel">Detail level (advertiser or product)</param>
        /// <returns>Code HTML</returns>
        protected override StringBuilder BuildEvolution(CstResult.MotherRecap.ElementType classifLevel)
        {
            TNS.AdExpress.Domain.Web.TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
            bool inRef = false, inComp = false;
            UnitInformation selectedCurrency = _session.GetSelectedUnit();

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

            #region No Data
            if (tab == null || tab.GetLongLength(0) == 0)
            {
                t.Append("<table class=\"backGroundWhite\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
                t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                if (classifLevel == CstResult.MotherRecap.ElementType.advertiser)
                {
                    t.AppendFormat("{0}<br>{1} {2}", GestionWeb.GetWebWord(177, _session.SiteLanguage), GestionWeb.GetWebWord(2474, _session.SiteLanguage), periodLabel);
                }
                t.Append("</td></tr></table>");
                return t;

            }
            #endregion

            long last = tab.GetLongLength(0) - 1;
            AdExpressCultureInfo fp = WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo;
            

            t.Append("<table class=\"backGroundWhite\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");

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
            t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1401, _session.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_session.SiteLanguage) + ")");
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
            t.AppendFormat("<td class=\"{0}\" nowrap valign=\"top\">{1}</td>", cssHeader, GestionWeb.GetWebWord(1401, _session.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_session.SiteLanguage) + ")");
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
            //double total = 0;
            double ecart = 0;
            for (int i = 0; i < tab.GetLongLength(0) && i < 10; i++)
            {

                #region Style
                cssLabel = cssNoneLabel;
                cssNb = cssNoneNb;
                if (tab[i, COMPETITOR] != null)
                {
                    typeElt = Convert.ToInt32(tab[i, COMPETITOR]);
                    if (typeElt == 3)
                    {
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
               
                if (!FctIndicators.IsNull(tab[i, ECART]))
                {
                    ecart = Convert.ToDouble(tab[i, ECART], fp);
                    if (ecart > 0)
                    {
                        if(_excel)
                            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssLabel, String.Format("{0}", tab[i, PRODUCT]));
                        else
                            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssLabel, TNS.FrameWork.WebResultUI.Functions.TextWrap.WrapHtml(String.Format("{0}", tab[i, PRODUCT]), textWrap.NbChar, textWrap.Offset));
                        if (!FctIndicators.IsNull(tab[i, TOTAL_N]))
                            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctIndicators.ConvertUnitValueToString(tab[i, TOTAL_N], _session.Unit, fp));
                        else t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, "&nbsp;");

                        if (!FctIndicators.IsNull(tab[i, EVOLUTION]))
                        {
                            evol = Convert.ToDouble(tab[i, EVOLUTION], fp);
                            if (!Double.IsInfinity(evol))
                            {
                                t.AppendFormat("<td class=\"{0}\" nowrap>{1}%</td>", cssNb, FctIndicators.ConvertUnitValueAndPdmToString(tab[i, EVOLUTION], _session.Unit, true, fp));
                            }
                            else
                            {
                                t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, GestionWeb.GetWebWord(1214, _session.SiteLanguage));
                            }
                        }
                        else t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, GestionWeb.GetWebWord(1214, _session.SiteLanguage));

                        if (!FctIndicators.IsNull(tab[i, ECART]))
                            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctIndicators.ConvertUnitValueToString(tab[i, ECART], _session.Unit, fp));
                        else t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, "&nbsp;");
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
                    if (typeElt == 3)
                    {
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
                if (!FctIndicators.IsNull(tab[last, ECART]))
                {
                    ecart = Convert.ToDouble(tab[last, ECART], fp);
                    if (ecart < 0)
                    {
                        if (_excel)
                            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssLabel, String.Format("{0}", tab[last, PRODUCT]));
                        else
                            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssLabel, TNS.FrameWork.WebResultUI.Functions.TextWrap.WrapHtml(String.Format("{0}", tab[last, PRODUCT]), textWrap.NbChar, textWrap.Offset));
                        
                        if (!FctIndicators.IsNull(tab[last, TOTAL_N]))
                            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctIndicators.ConvertUnitValueToString(tab[last, TOTAL_N], _session.Unit, fp));
                        else t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, "&nbsp;");
                        if (!FctIndicators.IsNull(tab[last, EVOLUTION]))
                        {
                            evol = Convert.ToDouble(tab[last, EVOLUTION], fp);
                            if (!Double.IsInfinity(evol))
                            {
                                t.AppendFormat("<td class=\"{0}\" nowrap>{1}%</td>", cssNb, FctIndicators.ConvertUnitValueAndPdmToString(tab[last, EVOLUTION], _session.Unit, true, fp));
                            }
                            else
                            {
                                t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, GestionWeb.GetWebWord(1214, _session.SiteLanguage));
                            }
                        }
                        else t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, GestionWeb.GetWebWord(1214, _session.SiteLanguage));

                        if (!FctIndicators.IsNull(tab[last, ECART]))
                            t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, FctIndicators.ConvertUnitValueToString(tab[last, ECART], _session.Unit, fp));
                        else t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>", cssNb, "&nbsp;");
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

        #endregion


        #region GetUnit
        /// <summary>
        /// Get unit
        /// </summary>
        /// <returns>unit</returns>
        protected override UnitInformation GetUnit()
        {
            return _session.GetSelectedUnit();

        }
        #endregion
        

    }
}
