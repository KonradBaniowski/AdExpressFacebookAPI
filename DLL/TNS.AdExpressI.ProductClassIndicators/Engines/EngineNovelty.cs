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
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
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
    /// Engine to build a Novelty report as a graph or a table
    /// </summary>
    public class EngineNovelty:Engine
    {

        #region Constants
        /// <summary>
        /// Maximum number of columns in result table
        /// </summary>
        public const int MAX_COLUMN_LENGTH = 9;
        		/// <summary>
		///Label column index (advertiser or product)
		///</summary>
		public const int ELEMENT_COLUMN_INDEX=0;
		/// <summary>
		/// ID elemtn column index  (advertiser or product)
		/// </summary>
		public const int ID_ELEMENT_COLUMN_INDEX=1;
        /// <summary>
        /// Index of column with last active month investments
        /// </summary>
        public const int LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX = 2;
        ///<summary>
        /// Index of column with last active month label
        /// </summary>				
        public const int LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX = 3;
        /// <summary>
        /// Index of column "Inactivity length"
        /// </summary>
        public const int INACTIVITY_PERIOD_COLUMN_INDEX = 4;
        /// <summary>
        /// Index of investment column
        /// </summary>
        public const int CURRENT_MONTH_INVEST_COLUMN_INDEX = 5;
        /// <summary>
        /// Index of SOV column
        /// </summary>
        public const int SOV_COLUMN_INDEX = 6;
        /// <summary>
		///Advertiser ID column index
		///</summary>
		public const int PERSONNALISATION_ELEMENT_COLUMN_INDEX=7;
		/// <summary>
        /// Index of column with last active month id
		///</summary>
		public const int LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX=8;        
        #endregion

        #region Attributes
        /// <summary>
        /// Classification detail
        /// </summary>
        protected CstResult.MotherRecap.ElementType _classifLevel = CstResult.MotherRecap.ElementType.advertiser;
        #endregion

        #region Accessors
        /// <summary>
        /// Get / Set Classification detail
        /// </summary>
        public CstResult.MotherRecap.ElementType ClassifLevel{
            get
            {
                return _classifLevel;
            }
            set
            {
                _classifLevel = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public EngineNovelty(WebSession session, IProductClassIndicatorsDAL dalLayer) : base(session, dalLayer) { }
        #endregion

        #region GetResult
        /// <summary>
        /// Get Novelty indicator as a table
        /// </summary>
        /// <returns>StringBuilder with HTML code</returns>
        public override StringBuilder GetResult()
        {
            return BuildTables();
        }
        #endregion

        #region GetGraph
        /// <summary>
        /// Get Graph
        /// </summary>
        /// <returns>StringBuilder with HTML code</returns>
        public StringBuilder GetGraph()
        {
            return BuildGraphes();
        }

        #endregion

        #region GetData
        /// <summary>
        /// Get data ready for display
        /// </summary>
        /// <param name="classifLevel">Classification detail (product or advertiser)</param>
        /// <returns></returns>
        virtual public object[,] GetData()
        {


            #region Data loading
            string monthColumn = FctUtilities.Dates.CurrentActiveMonth(_periodEnd, _session);
            //Novelties
            DataSet dsNovelties = null;
            DataTable dtNovelties = null;
            dsNovelties = _dalLayer.GetNovelty(_classifLevel);
            if (dsNovelties == null || dsNovelties.Tables[0] == null || dsNovelties.Tables[0].Rows.Count == 0)
            {
                throw new NoDataException();
            }
            dtNovelties = dsNovelties.Tables[0];            
            double total = _dalLayer.GetMonthTotal(CstComparaisonCriterion.universTotal, CstResult.PalmaresRecap.typeYearSelected.currentYear);          
            #endregion

            #region Get N1 months indexes       
            int lastN1month = 0;
            int firstN1month = 0;
            if (_session.ComparativeStudy){
                int j = dtNovelties.Columns.Count-2;
                while (j > 0 && !dtNovelties.Columns[j].ColumnName.ToUpper().Equals(monthColumn.ToUpper()))
                {
                    j--;
                }
                if (j > 0)
                {
                    firstN1month = j+1;
                    lastN1month = dtNovelties.Columns.Count - 2;
                }
            }
            #endregion

            #region Build result table
            object[,] tab = new object[dtNovelties.Rows.Count, MAX_COLUMN_LENGTH];
            string eltLabelName = (_classifLevel==CstResult.MotherRecap.ElementType.product) ? "product":"advertiser";
            string advIdName = "id_advertiser";
            string eltIDName = (_classifLevel==CstResult.MotherRecap.ElementType.product) ? "id_product":advIdName;
            int i = 0;
            foreach (DataRow currentRow in dtNovelties.Rows)
            {
                tab[i, ID_ELEMENT_COLUMN_INDEX] = Convert.ToInt64(currentRow[eltIDName]);
                tab[i, ELEMENT_COLUMN_INDEX] = currentRow[eltLabelName].ToString();
				if (_classifLevel == CstResult.MotherRecap.ElementType.advertiser) {
					if (_competitorIDS.Contains(Convert.ToInt64(currentRow[advIdName]))) {
						tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX] = CstWeb.AdvertiserPersonalisation.Type.competitor;
					}
					else if (_referenceIDS.Contains(Convert.ToInt64(currentRow[advIdName]))) {
						tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX] = CstWeb.AdvertiserPersonalisation.Type.reference;
					}
					else {
						tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX] = CstWeb.AdvertiserPersonalisation.Type.none;
					}					
				}

                double d = Convert.ToDouble(currentRow[monthColumn]);
                tab[i, CURRENT_MONTH_INVEST_COLUMN_INDEX] = d;
                tab[i, SOV_COLUMN_INDEX] = d * Convert.ToDouble(100) / total;

                #region Last active month
                /*
                 * Get last active month by going from the last month to the first one and stopping with the first with investment > 0
                 * 
                 * */
                if (_session.ComparativeStudy)
                {
                    for (int j = lastN1month; j >= firstN1month; j--)
                    {
                        double dN1 = Convert.ToDouble(currentRow[j]);
                        if (dN1 > 0.0)
                        {
                            tab[i, LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX] = dN1;
                            DateTime t = FctUtilities.Dates.GetDateFromAlias(dtNovelties.Columns[j].ColumnName);
                            tab[i, LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX] = t.Month;
                            tab[i, LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX] = string.Format("{0} {1}", FctUtilities.Dates.GetMonthLabel(t.Month, _session.SiteLanguage), t.Year);
                            //inactivity length = (Active month - 1) + (12 - Last active month)
                            int nbMonth = (_periodEnd.Month - 1) + (12 - t.Month);
                            if (nbMonth > 0)
                                tab[i, INACTIVITY_PERIOD_COLUMN_INDEX] = nbMonth;
                            break;
                        }
                    }
                }
                #endregion

                i++;
            }
            #endregion

            return tab;

        }
        #endregion

        #region Build report

        #region Table
        /// <summary>
        /// Build Novelty as a table (advertiser and product)
        /// </summary>
        /// <param name="classifLevel">Detail level (advertiser or product)</param>
        /// <returns>Novelty Table</returns>
        protected virtual StringBuilder BuildTables()
        {
            StringBuilder str = new StringBuilder();
            StringBuilder tmp = null;

            this._classifLevel = CstResult.MotherRecap.ElementType.advertiser;
            try
            {
                tmp = this.BuildTable();
            }
            catch(NoDataException e){}
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
			if (_session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) {
				this._classifLevel = CstResult.MotherRecap.ElementType.product;
				try {
					tmp = this.BuildTable();
				}
				catch (NoDataException e1) { }
				str.Append("<center>");
				if (tmp != null && tmp.Length > 0) {
					str.Append(tmp.ToString());
				}
				else {
					str.Append("<br><table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
					str.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
					str.AppendFormat("{0} {1}", GestionWeb.GetWebWord(177, _session.SiteLanguage), GestionWeb.GetWebWord(1238, _session.SiteLanguage));
					str.Append("</td></tr></table>");
				}
				str.Append("</center>");
			}
            return str;

        }
        /// <summary>
        /// Build Novelty as a table
        /// </summary>
        /// <param name="classifLevel">Detail level (advertiser or product)</param>
        /// <returns>Novelty Table</returns>
        protected virtual StringBuilder BuildTable()
        {

            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
			DataSet res = null;
            UnitInformation selectedCurrency = GetUnit();

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
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

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
            if (CstResult.MotherRecap.ElementType.advertiser == _classifLevel){
                t.AppendFormat("<tr><td nowrap class=\"p2\">{0}</td>", GestionWeb.GetWebWord(1222, _session.SiteLanguage));
            }
            else{ 
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
			if (_classifLevel == TNS.AdExpress.Constantes.FrameWork.Results.MotherRecap.ElementType.product) {
				string idProductList = "";
				for (int i = 0; i < tab.GetLength(0); i++) {
					if (tab[i, ID_ELEMENT_COLUMN_INDEX] != null)
						idProductList += tab[i, ID_ELEMENT_COLUMN_INDEX] + ",";
				}
				if (idProductList != null && idProductList.Length>0) idProductList = idProductList.Substring(0, idProductList.Length - 1);
				res = _dalLayer.GetProductsPersonnalisationType(idProductList,_strYearID);
			}
			#endregion

            #region Build table
            string cssLabel, cssNb;
            for (int i = 0; i < tab.GetLength(0); i++)
            {
             
				#region Ref or competitor
				if (_classifLevel == TNS.AdExpress.Constantes.FrameWork.Results.MotherRecap.ElementType.product)
					SetAdvertiserPersonalisation(tab, res, i);

				if (tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX] != null && Convert.ToInt32(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) == 3) {
					cssLabel = cssMixedLabel;
					cssNb = cssMixedNB;
				}
				else if (tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX] != null && Convert.ToInt32(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) == 1) {
					cssLabel = cssRefLabel;
					cssNb = cssRefNb;
				}
				else if (tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX] != null && Convert.ToInt32(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) == 2) {
					cssLabel = cssCompLabel;
					cssNb = cssCompNb;
				}
				else {
					cssLabel = cssLabelNone;
					cssNb = cssNbNone;
				}

				#endregion

                t.Append("<tr>");
                //Label column
                t.AppendFormat("<td nowrap class={0}>{1}</td>", cssLabel, tab[i, ELEMENT_COLUMN_INDEX]);
                //Comparative study
                if (_session.ComparativeStudy)
                {
                    //Investments
                    if (tab[i, LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX] != null)
                        t.AppendFormat("<td nowrap class={0}>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(tab[i, LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX], _session.Unit, fp));
                    else
                        t.AppendFormat("<td nowrap class={0}>&nbsp;</td>", cssNb);
                    //Last active month label
                    if (tab[i, LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX] != null)
                        t.AppendFormat("<td nowrap class={0}>{1}</td>", cssLabel, Convertion.ToHtmlString(tab[i, LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX].ToString()));
                    else 
                        t.AppendFormat("<td nowrap class={0}>&nbsp;</td>", cssLabel);
                    //INactivity length
                    if (tab[i, INACTIVITY_PERIOD_COLUMN_INDEX] != null) 
                        t.AppendFormat("<td nowrap class={0}>{1}</td>", cssNb, tab[i, INACTIVITY_PERIOD_COLUMN_INDEX]);
                    else
                        t.AppendFormat("<td nowrap class={0}>&nbsp;</td>", cssNb);
                }
                //Current month Investments
                if (tab[i, CURRENT_MONTH_INVEST_COLUMN_INDEX] != null)
                    t.AppendFormat("<td nowrap class={0}>{1}</td>", cssNb, FctUtilities.Units.ConvertUnitValueToString(tab[i, CURRENT_MONTH_INVEST_COLUMN_INDEX], _session.Unit, fp));
                else
                    t.AppendFormat("<td nowrap class={0}>&nbsp;</td>", cssNb);
                //SOV
                if (tab[i, SOV_COLUMN_INDEX] != null) 
                    t.AppendFormat("<td nowrap class={0}>{1}%</td>", cssNb, FctUtilities.Units.ConvertUnitValueAndPdmToString(tab[i, SOV_COLUMN_INDEX], _session.Unit, true, fp));
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
        protected virtual StringBuilder BuildGraph()
        {

            StringBuilder t = new StringBuilder(10000);
            object[,] tab = this.GetData();
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
			DataSet res = null;
            UnitInformation selectedCurrency = GetUnit();

            #region No data
            if (tab==null || tab.GetLength(0) == 0)
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
				if (_classifLevel == TNS.AdExpress.Constantes.FrameWork.Results.MotherRecap.ElementType.product) {
					string idProductList = "";
					for (int i = 0; i < tab.GetLength(0); i++) {
						if (tab[i, ID_ELEMENT_COLUMN_INDEX] != null)
							idProductList += tab[i, ID_ELEMENT_COLUMN_INDEX] + ",";
					}
					if (idProductList != null && idProductList.Length > 0) idProductList = idProductList.Substring(0, idProductList.Length - 1);
					res = _dalLayer.GetProductsPersonnalisationType(idProductList,_strYearID);					
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
					                   					                   
						if (tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX] != null && Convert.ToInt32(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) == 3) {
							cssLabel = cssMixedLabel;
							cssNb = cssMixedNB;
						}
						else if (tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX] != null && Convert.ToInt32(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) == 2) {
							cssLabel = cssCompLabel;
							cssNb = cssCompNb;
						}
						else if (tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX] != null && Convert.ToInt32(tab[i, PERSONNALISATION_ELEMENT_COLUMN_INDEX]) == 1)
                        {
                            cssLabel = cssRefLabel;
                            cssNb = cssRefNb;
                        }						
						else {
							cssLabel = cssLabelNone;
							cssNb = cssNbNone;
						}
                   
                    #endregion

                    t.Append("<tr>");
                    //Label
                    t.AppendFormat("<td nowrap class={0}>{1}</td>", cssLabel, tab[i, ELEMENT_COLUMN_INDEX]);
                    //INactivity
                    if (tab[i, INACTIVITY_PERIOD_COLUMN_INDEX] != null)
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
                        if (tab[i, LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX] != null)
                        {
                            if (tab[i, LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX].Equals(k))
                            {
                                t.AppendFormat("<td nowrap class=pmcategorynb>{0}</td>", FctUtilities.Units.ConvertUnitValueToString(tab[i, LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX], _session.Unit, fp));
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
                    if (tab[i, CURRENT_MONTH_INVEST_COLUMN_INDEX] != null)
                    {
                        t.AppendFormat("<td nowrap class=\"pmcategorynb\">{0}</td>", FctUtilities.Units.ConvertUnitValueToString(tab[i, CURRENT_MONTH_INVEST_COLUMN_INDEX], _session.Unit, fp));
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
				if (_session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) {
					this._classifLevel = CstResult.MotherRecap.ElementType.product;
					try {
						tmp = this.BuildGraph();
					}
					catch (NoDataException) { }
					str.Append("<center>");
					if (tmp != null && tmp.Length > 0) {
						str.Append(tmp.ToString());
					}
					else {
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

		#region SetAdvertiserPersonalisation
		/// <summary>
		/// Set advertiser personnalisation
		/// </summary>
		/// <param name="tabResult">tab Result</param>
		/// <param name="res">dictionary[id_product,id_advertiser]</param>
		/// <param name="rowIndex">row index</param>
		protected virtual void SetAdvertiserPersonalisation(object[,] tabResult, DataSet res, int rowIndex) {
			List<long> temp = null;
			long idProd = -1;
			int nbTypes = 0;
			bool inRef = false, inComp = false, inneutral = false;
			tabResult[rowIndex, PERSONNALISATION_ELEMENT_COLUMN_INDEX] = CstWeb.AdvertiserPersonalisation.Type.none;
			if (res != null && res.Tables[0] != null && res.Tables[0].Rows.Count > 0) {
				if (tabResult[rowIndex, ID_ELEMENT_COLUMN_INDEX] != null) {
					idProd = long.Parse(tabResult[rowIndex, ID_ELEMENT_COLUMN_INDEX].ToString());
					foreach (DataRow row in res.Tables[0].Rows) {
						if (long.Parse(row["id_product"].ToString()) == idProd) {
							if (Convert.ToInt32(row["inref"]) > 0) {
								nbTypes++;
								inRef = true;
							}
							if (Convert.ToInt32(row["incomp"]) > 0) {
								nbTypes++;
								inComp = true;
							}
							if (Convert.ToInt32(row["inneutral"]) > 0) {
								nbTypes++;
								inneutral = true;
							}

							if (nbTypes > 1)
								tabResult[rowIndex, PERSONNALISATION_ELEMENT_COLUMN_INDEX] = CstWeb.AdvertiserPersonalisation.Type.mixed;
							else if (inComp)
								tabResult[rowIndex, PERSONNALISATION_ELEMENT_COLUMN_INDEX] = CstWeb.AdvertiserPersonalisation.Type.competitor;
							else if (inRef)
								tabResult[rowIndex, PERSONNALISATION_ELEMENT_COLUMN_INDEX] = CstWeb.AdvertiserPersonalisation.Type.reference;							
							nbTypes = 0;
							inRef = false;
							inComp = false;
							inneutral = false;
							break;
						}
					}							
				}
			}
		}
		#endregion

        #endregion

    }
}
