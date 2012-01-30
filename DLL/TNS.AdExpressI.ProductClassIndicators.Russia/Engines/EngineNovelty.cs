using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using TNS.AdExpress.Domain.Exceptions;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using FctIndicators = TNS.AdExpressI.ProductClassIndicators.Russia.Functions;
using TNS.FrameWork;

namespace TNS.AdExpressI.ProductClassIndicators.Russia.Engines{
    /// <summary>
    /// Engine to build a Novelty report as a graph or a table
    /// This class contains specific rules for the russian version
    /// </summary>
    public class EngineNovelty : TNS.AdExpressI.ProductClassIndicators.Engines.EngineNovelty{

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public EngineNovelty(WebSession session, IProductClassIndicatorsDAL dalLayer) : base(session, dalLayer) { }
        #endregion

        #region GetData
        /// <summary>
        /// Get data ready for display
        /// </summary>
        /// <param name="classifLevel">Classification detail (product or advertiser)</param>
        /// <returns>Matrix of elements to display</returns>
        public override object[,] GetData()
        {

            #region Data loading
            //Novelties
            DataSet dsNovelties = null;
            DataTable dtNovelties = null;
            dsNovelties = _dalLayer.GetNovelty(_classifLevel);
            if (dsNovelties == null || dsNovelties.Tables.Count == 0 || dsNovelties.Tables[0] == null || dsNovelties.Tables[0].Rows.Count == 0){
                return null;
            }
            dtNovelties = dsNovelties.Tables[0];
            #endregion

            #region Build result table
            object[,] tab = new object[dtNovelties.Rows.Count, MAX_COLUMN_LENGTH];
            string eltLabelName = (_classifLevel == CstResult.MotherRecap.ElementType.product) ? "NEW_PRODUCT" : "NEW_ADVERTISER";
            string advIdName = "ID_ADVERTISER";
            string eltIDName = (_classifLevel == CstResult.MotherRecap.ElementType.product) ? "ID_PRODUCT" : advIdName;
            int i = 0;

            foreach (DataRow currentRow in dtNovelties.Rows){

                tab[i, ID_ELEMENT_COLUMN_INDEX] = Convert.ToInt64(currentRow[eltIDName]);
                tab[i, ELEMENT_COLUMN_INDEX] = currentRow[eltLabelName].ToString();

                if (_classifLevel == CstResult.MotherRecap.ElementType.advertiser){
                    if (_competitorIDS.Contains(Convert.ToInt64(currentRow[advIdName]))){
                        tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX] = CstWeb.AdvertiserPersonalisation.Type.competitor;
                    }
                    else if (_referenceIDS.Contains(Convert.ToInt64(currentRow[advIdName]))){
                        tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX] = CstWeb.AdvertiserPersonalisation.Type.reference;
                    }
                    else{
                        tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX] = CstWeb.AdvertiserPersonalisation.Type.none;
                    }
                }
                if (!FctIndicators.IsNull(currentRow["UNIT_CURRENT_MONTH"]))
                    tab[i, CURRENT_MONTH_INVEST_COLUMN_INDEX] = currentRow["UNIT_CURRENT_MONTH"];
                if (!FctIndicators.IsNull(currentRow["SOV"]))
                    tab[i, SOV_COLUMN_INDEX] =currentRow["SOV"];
                if (!FctIndicators.IsNull(currentRow["UNIT_RECENT_ACTIVE_MONTH"]))
                    tab[i, LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX] = currentRow["UNIT_RECENT_ACTIVE_MONTH"];
                if (!FctIndicators.IsNull(currentRow["RECENT_ACTIVE_MONTH_ID"]))
                    tab[i, LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX] = Convert.ToInt32(currentRow["RECENT_ACTIVE_MONTH_ID"]);
                if (!FctIndicators.IsNull(currentRow["RECENT_ACTIVE_ MONTH"] ) && currentRow["RECENT_ACTIVE_ MONTH"].ToString().Length>0)
                    tab[i, LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX] = currentRow["RECENT_ACTIVE_ MONTH"].ToString();
                if (!FctIndicators.IsNull(currentRow["PERIOD_INACTIVITY"]))
                    tab[i, INACTIVITY_PERIOD_COLUMN_INDEX] = Convert.ToInt32(currentRow["PERIOD_INACTIVITY"]);

                i++;
            }
            #endregion

            return tab;
        }
        #endregion

        #region Build report

        #region Table
     
        /// <summary>
        /// Build Novelty as a table
        /// </summary>
        /// <param name="classifLevel">Detail level (advertiser or product)</param>
        /// <returns>Novelty Table</returns>
        protected override StringBuilder BuildTable()
        {

            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
            TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
            DataSet res = null;
            UnitInformation selectedCurrency = _session.GetSelectedUnit();

            #region Styles
            string cssLabelNone = "p7";
            string cssNbNone = "p9";
            string cssRefLabel = "p15";
            string cssRefNb = "p151";
            string cssMixedLabel = "p16";
            string cssMixedNB = "p161";
            string cssCompLabel = (this._excel) ? "p142" : "p14";
            string cssCompNb = (this._excel) ? "p143" : "p141";
            #endregion

            object[,] tab = this.GetData();
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo;

            #region No data
            if (tab == null || tab.GetLength(0) == 0)
            {
                return t;
            }
            #endregion

            #region Period
            string monthAlias = FctUtilities.Dates.CurrentActiveMonth(_periodEnd, _session);
            #endregion

            t.Append("\n<TABLE border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"p2\"><TR><TD>");

            #region Headers
            //Advertiser/Product Column
            if (CstResult.MotherRecap.ElementType.advertiser == _classifLevel)
            {
                t.AppendFormat("<tr><td nowrap class=\"p2\">{0}</td>", GestionWeb.GetWebWord(1222, _session.SiteLanguage));
            }
            else
            {
                t.AppendFormat("<tr><td nowrap class=\"p2\">{0}</td>", GestionWeb.GetWebWord(1223, _session.SiteLanguage));
            }
            //Investments last active month (kE)
            if (_session.ComparativeStudy)
            {
                t.AppendFormat("<td nowrap class=\"p2\">{0}</td>", GestionWeb.GetWebWord(1219, _session.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_session.SiteLanguage) + ")");
                //Last active month label
                t.AppendFormat("<td nowrap class=\"p2\">{0}</td>", GestionWeb.GetWebWord(1219, _session.SiteLanguage));
                //Période d'inactivité
                t.AppendFormat("<td nowrap class=\"p2\">{0}</td>", GestionWeb.GetWebWord(1220, _session.SiteLanguage));
            }
            //Current month (KE)
            t.AppendFormat("<td nowrap class=\"p2\">{0}{1}-{2} </td>", (GestionWeb.GetWebWord(2786, _session.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_session.SiteLanguage) + ") "), _periodEnd.ToString("MM"), _periodEnd.Year);
            //SOV			
            t.AppendFormat("<td nowrap class=\"p2\">{0}</td>", GestionWeb.GetWebWord(437, _session.SiteLanguage));
            t.Append("\n</tr>");
            #endregion

            #region Get id products
            if (_classifLevel == TNS.AdExpress.Constantes.FrameWork.Results.MotherRecap.ElementType.product)
            {
                string idProductList = "";
                for (int i = 0; i < tab.GetLength(0); i++)
                {
                    if (!FctIndicators.IsNull(tab[i, ID_ELEMENT_COLUMN_INDEX] ))
                        idProductList += tab[i, ID_ELEMENT_COLUMN_INDEX] + ",";
                }
                if (idProductList != null && idProductList.Length > 0) idProductList = idProductList.Substring(0, idProductList.Length - 1);
                res = _dalLayer.GetProductsPersonnalisationType(idProductList, _strYearID);
            }
            #endregion

            #region Build table
            string cssLabel, cssNb;
            for (int i = 0; i < tab.GetLength(0); i++)
            {

                #region Ref or competitor
                if (_classifLevel == TNS.AdExpress.Constantes.FrameWork.Results.MotherRecap.ElementType.product)
                    SetAdvertiserPersonalisation(tab, res, i);

                if (!FctIndicators.IsNull(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) && Convert.ToInt32(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) == 3)
                {
                    cssLabel = cssMixedLabel;
                    cssNb = cssMixedNB;
                }
                else if (!FctIndicators.IsNull(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) && Convert.ToInt32(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) == 1)
                {
                    cssLabel = cssRefLabel;
                    cssNb = cssRefNb;
                }
                else if (!FctIndicators.IsNull(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) && Convert.ToInt32(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) == 2)
                {
                    cssLabel = cssCompLabel;
                    cssNb = cssCompNb;
                }
                else
                {
                    cssLabel = cssLabelNone;
                    cssNb = cssNbNone;
                }

                #endregion

                t.Append("<tr>");
                //Label column
                if (!FctIndicators.IsNull(tab[i, ELEMENT_COLUMN_INDEX])) {
                    if (_excel)
                        t.AppendFormat("<td nowrap class={0}>{1}</td>", cssLabel, String.Format("{0}", tab[i, ELEMENT_COLUMN_INDEX]));
                    else
                        t.AppendFormat("<td nowrap class={0}>{1}</td>", cssLabel, TNS.FrameWork.WebResultUI.Functions.TextWrap.WrapHtml(String.Format("{0}", tab[i, ELEMENT_COLUMN_INDEX]), textWrap.NbChar, textWrap.Offset));
                }
                else t.AppendFormat("<td nowrap class={0}>{1}</td>", cssLabel, "&nbsp;");
                //Comparative study
                if (_session.ComparativeStudy)
                {
                    //Investments
                    if (!FctIndicators.IsNull(tab[i, LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX]))
                        t.AppendFormat("<td nowrap class={0}>{1}</td>", cssNb, FctIndicators.ConvertUnitValueToString(tab[i, LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX], _session.Unit, fp));
                    else
                        t.AppendFormat("<td nowrap class={0}>&nbsp;</td>", cssNb);
                    //Last active month label
                    if (!FctIndicators.IsNull(tab[i, LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX]))
                        t.AppendFormat("<td nowrap class={0}>{1}</td>", cssLabel, Convertion.ToHtmlString(tab[i, LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX].ToString()));
                    else
                        t.AppendFormat("<td nowrap class={0}>&nbsp;</td>", cssLabel);
                    //INactivity length
                    if (!FctIndicators.IsNull(tab[i, INACTIVITY_PERIOD_COLUMN_INDEX] ))
                        t.AppendFormat("<td nowrap class={0}>{1}</td>", cssNb, tab[i, INACTIVITY_PERIOD_COLUMN_INDEX]);
                    else
                        t.AppendFormat("<td nowrap class={0}>&nbsp;</td>", cssNb);
                }
                //Current month Investments
                if (!FctIndicators.IsNull(tab[i, CURRENT_MONTH_INVEST_COLUMN_INDEX]))
                    t.AppendFormat("<td nowrap class={0}>{1}</td>", cssNb, FctIndicators.ConvertUnitValueToString(tab[i, CURRENT_MONTH_INVEST_COLUMN_INDEX], _session.Unit, fp));
                else
                    t.AppendFormat("<td nowrap class={0}>&nbsp;</td>", cssNb);
                //SOV
                if (!FctIndicators.IsNull(tab[i, SOV_COLUMN_INDEX]))
                    t.AppendFormat("<td nowrap class={0}>{1}%</td>", cssNb, FctIndicators.ConvertUnitValueAndPdmToString(tab[i, SOV_COLUMN_INDEX], _session.Unit, true, fp));
                else
                    t.AppendFormat("<td nowrap class={0}>&nbsp;</td>", cssNb);
                t.Append("</tr>");
            }
            #endregion

            t.Append("\n</TD></TR></TABLE>");

            return t;

        }
        #endregion

        #region Graph
        /// <summary>
        /// Build Novelty as a graph (product OR advertiser)
        /// </summary>		
        /// <returns>Novelty graph</returns>
        protected override StringBuilder BuildGraph()
        {

            StringBuilder t = new StringBuilder(10000);
            object[,] tab = this.GetData();
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.DataLanguage].CultureInfo;
            TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].textWrap;
            DataSet res = null;
            UnitInformation selectedCurrency = _session.GetSelectedUnit();

            #region No data
            if (tab == null || tab.GetLength(0) == 0)
            {
                return t;
            }
            #endregion

            if (_session.ComparativeStudy)
            {

                #region Styles
                string cssLabelNone = "p7";
                string cssNbNone = "p9";
                string cssColored = "p9ind";
                string cssRefLabel = "p15";
                string cssRefNb = "p151";
                string cssMixedLabel = "p16";
                string cssMixedNB = "p161";
                string cssCompLabel = (this._excel) ? "p142" : "p14";
                string cssCompNb = (this._excel) ? "p143" : "p141";
                #endregion

                #region Legend
                t.AppendFormat("<table align=\"left\"><tr><td nowrap class=pmcategorynb width=25>&nbsp;&nbsp;</td><td class=txtNoir11>{0}</td></tr>", GestionWeb.GetWebWord(1225, _session.SiteLanguage));
                t.AppendFormat("<tr><td nowrap class={0} width=25>&nbsp;&nbsp;</td><td class=txtNoir11>{1}</td></tr></table><br/><br/>", cssColored, GestionWeb.GetWebWord(1226, _session.SiteLanguage));
                #endregion

                #region Build graph
                t.Append("\n<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"p2\"><TR><TD>");

                //Mois actif
                string monthAlias = FctUtilities.Dates.CurrentActiveMonth(_periodEnd, _session);

                #region headers
                //Label column
                if (CstResult.MotherRecap.ElementType.advertiser == _classifLevel)
                {
                    t.AppendFormat("<tr><td nowrap class=\"p2\">{0}</td>", GestionWeb.GetWebWord(1222, _session.SiteLanguage));
                }
                else
                {
                    t.AppendFormat("<tr><td nowrap class=\"p2\">{0}</td>", GestionWeb.GetWebWord(1223, _session.SiteLanguage));
                }
                //Iinactivity
                t.AppendFormat("<td nowrap class=\"p2\">{0}</td>", GestionWeb.GetWebWord(1220, _session.SiteLanguage));
                if (!_excel)
                {
                    t.Append("<td bgcolor=\"#646262\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
                }

                //N-1 year cells
                for (int j = 1; j <= 12; j++)
                {
                    t.AppendFormat("<td nowrap class=\"p2\">&nbsp;{0}-{1}</td>", j.ToString("00"), _periodEnd.AddYears(-1).Year);
                }
                //Year separator
                if (!_excel)
                {
                    t.Append("<td bgcolor=\"#646262\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
                }

                //N cells
                for (int m = 1; m < _periodEnd.Month; m++)
                {
                    t.AppendFormat("<td nowrap class=\"p2\">&nbsp;{0}-{1}</td>", m.ToString("00"), _periodEnd.Year);
                }
                //Separator
                if (!_excel)
                {
                    t.Append("<td bgcolor=\"#646262\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
                }
                //Currnet month
                t.AppendFormat("<td nowrap align=center class=\"p2\">{2}<br>{0}-{1}</td>", _periodEnd.Month.ToString("00"), _periodEnd.Year, (GestionWeb.GetWebWord(2786, _session.SiteLanguage) + " (" + selectedCurrency.GetUnitSignWebText(_session.SiteLanguage) + ")"));
                t.Append("\n</tr>");
                #endregion

                #region Get id products
                if (_classifLevel == TNS.AdExpress.Constantes.FrameWork.Results.MotherRecap.ElementType.product)
                {
                    string idProductList = "";
                    for (int i = 0; i < tab.GetLength(0); i++)
                    {
                        if (!FctIndicators.IsNull(tab[i, ID_ELEMENT_COLUMN_INDEX] ))
                            idProductList += tab[i, ID_ELEMENT_COLUMN_INDEX] + ",";
                    }
                    if (idProductList != null && idProductList.Length > 0) idProductList = idProductList.Substring(0, idProductList.Length - 1);
                    res = _dalLayer.GetProductsPersonnalisationType(idProductList, _strYearID);
                }
                #endregion

                #region data
                string cssLabel = string.Empty;
                string cssNb = string.Empty;
                bool found = false;

                for (int i = 0; i < tab.GetLength(0); i++)
                {
                    found = false;

                    #region Ref or competitor
                    if (_classifLevel == TNS.AdExpress.Constantes.FrameWork.Results.MotherRecap.ElementType.product)
                        SetAdvertiserPersonalisation(tab, res, i);

                    if (!FctIndicators.IsNull(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) && Convert.ToInt32(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) == 3)
                    {
                        cssLabel = cssMixedLabel;
                        cssNb = cssMixedNB;
                    }
                    else if (!FctIndicators.IsNull(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX])&& Convert.ToInt32(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) == 2)
                    {
                        cssLabel = cssCompLabel;
                        cssNb = cssCompNb;
                    }
                    else if (!FctIndicators.IsNull(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) && Convert.ToInt32(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) == 1)
                    {
                        cssLabel = cssRefLabel;
                        cssNb = cssRefNb;
                    }
                    else
                    {
                        cssLabel = cssLabelNone;
                        cssNb = cssNbNone;
                    }

                    #endregion

                    t.Append("<tr>");
                    //Label
                    if(_excel)
                        t.AppendFormat("<td nowrap class={0}>{1}</td>", cssLabel, String.Format("{0}", tab[i, ELEMENT_COLUMN_INDEX]));
                    else
                        t.AppendFormat("<td nowrap class={0}>{1}</td>", cssLabel, TNS.FrameWork.WebResultUI.Functions.TextWrap.WrapHtml(String.Format("{0}", tab[i, ELEMENT_COLUMN_INDEX]), textWrap.NbChar, textWrap.Offset));
                    //INactivity
                    if (!FctIndicators.IsNull(tab[i, INACTIVITY_PERIOD_COLUMN_INDEX]))
                    {
                        t.AppendFormat("<td nowrap class={0}>{1}</td>", cssNb, tab[i, INACTIVITY_PERIOD_COLUMN_INDEX]);
                    }
                    else
                    {
                        t.AppendFormat("<td nowrap class={0}>&nbsp;</td>", cssNb);
                    }
                    //Separator
                    if (!_excel)
                    {
                        t.Append("<td bgcolor=\"#646262\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
                    }

                    //N-1 cells
                    for (int k = 1; k <= 12; k++)
                    {
                        //Last active monthe label
                        if (!FctIndicators.IsNull(tab[i, LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX]))
                        {
                            if (tab[i, LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX].Equals(k))
                            {
                                t.AppendFormat("<td nowrap class=pmcategorynb>{0}</td>", FctIndicators.ConvertUnitValueToString(tab[i, LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX], _session.Unit, fp));
                                found = true;
                            }
                            else if (found)
                            {
                                t.AppendFormat("<td nowrap class={0}>&nbsp;</td>", cssColored);
                            }
                            else
                            {
                                t.Append("<td nowrap>&nbsp;</td>");
                            }
                        }
                        else
                        {
                            t.Append("<td nowrap>&nbsp;</td>");
                        }
                    }
                    //Separator
                    if (!_excel)
                    {
                        t.Append("<td bgcolor=\"#646262\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
                    }
                    //N Cells
                    for (int l = 1; l < _periodEnd.Month; l++)
                    {
                        if (found)
                        {
                            t.AppendFormat("<td nowrap class={0}>&nbsp;</td>", cssColored);
                        }
                        else
                        {
                            t.Append("<td nowrap>&nbsp;</td>");
                        }
                    }
                    //Separator
                    if (!_excel)
                    {
                        t.Append("<td bgcolor=\"#646262\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
                    }

                    //Current month
                    if (!FctIndicators.IsNull(tab[i, CURRENT_MONTH_INVEST_COLUMN_INDEX]))
                    {
                        t.AppendFormat("<td nowrap class=\"pmcategorynb\">{0}</td>", FctIndicators.ConvertUnitValueToString(tab[i, CURRENT_MONTH_INVEST_COLUMN_INDEX], _session.Unit, fp));
                    }
                    else
                    {
                        t.AppendFormat("<td nowrap class={0}>&nbsp;</td>", cssNb);
                    }

                    t.Append("\n</tr>");
                }
                #endregion

                //table end
                t.Append("\n</td></tr></table>");
                #endregion

            }

            return t;

        }
        /// <summary>
        /// Build Novelty as a graph (product and advertiser)
        /// </summary>		
        /// <returns>Novelty graph</returns>
        protected StringBuilder BuildGraphes()
        {

            StringBuilder str = new StringBuilder();

            if (_session.ComparativeStudy)
            {
                StringBuilder tmp = null;

                this._classifLevel = CstResult.MotherRecap.ElementType.advertiser;
                try
                {
                    tmp = this.BuildGraph();
                }
                catch (NoDataException) { }
                str.Append("<center>");
                if (tmp != null && tmp.Length > 0)
                {
                    str.Append(tmp.ToString());
                }
                else
                {
                    str.Append("<br><table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
                    str.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                    str.AppendFormat("{0} {1}", GestionWeb.GetWebWord(177, _session.SiteLanguage), GestionWeb.GetWebWord(1239, _session.SiteLanguage));
                    str.Append("</td></tr></table>");
                }
                str.Append("</center><br>");
                if (_session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                {
                    this._classifLevel = CstResult.MotherRecap.ElementType.product;
                    try
                    {
                        tmp = this.BuildGraph();
                    }
                    catch (NoDataException) { }
                    str.Append("<center>");
                    if (tmp != null && tmp.Length > 0)
                    {
                        str.Append(tmp.ToString());
                    }
                    else
                    {
                        str.Append("<br><table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
                        str.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                        str.AppendFormat("{0} {1}", GestionWeb.GetWebWord(177, _session.SiteLanguage), GestionWeb.GetWebWord(1238, _session.SiteLanguage));
                        str.Append("</td></tr></table>");
                    }
                    str.Append("</center>");
                }
            }
            else
            {
                str.Append("<br><table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
                str.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                str.AppendFormat("{0}", GestionWeb.GetWebWord(177, _session.SiteLanguage));
                str.AppendFormat("<br>{0}", GestionWeb.GetWebWord(1224, _session.SiteLanguage));
                str.Append("</td></tr></table>");

            }

            return str;
        }
        #endregion

        

        #endregion


    }
}
