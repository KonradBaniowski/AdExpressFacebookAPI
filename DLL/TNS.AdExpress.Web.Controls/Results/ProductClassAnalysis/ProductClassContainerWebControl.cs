#region Informations
// Auteur: D. Mussuma 
// Date de création: 28/07/2006
// Date de modification:
#endregion

using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using Navigation = TNS.AdExpress.Domain.Web.Navigation;
using CstWeb = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.FrameWork.Results;
using Dundas.Charting.WebControl;
using AjaxPro;

using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.ProductClassIndicators;
using System.Reflection;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Web.Controls.Results.ProductClassAnalysis
{
	/// <summary>
	/// WebControl container to display different report of Product Class Indicators
	/// </summary>
	[ToolboxData("<{0}:ProductClassContainerWebControl runat=server></{0}:ProductClassContainerWebControl>")]
	public class ProductClassContainerWebControl :   System.Web.UI.WebControls.WebControl{
	
		
		#region Variables
		/// <summary>
		/// User session
		/// </summary>
		protected WebSession _session = null;
        /// <summary>
        /// Product level chart (or default if only one)
        /// </summary>
        protected WebControl _referenceChart = null;
        /// <summary>
        /// Advertiser level chart
        /// </summary>
        protected WebControl _advertiserChart = null;
        /// <summary>
        /// Report as table
        /// </summary>
        protected ProductClassTableWebControl _tableChart = null;
        /// <summary>
        /// Big Charts ?
        /// </summary>
        protected bool _bigSize = false;
        /// <summary>
        /// Excel format?
        /// </summary>
        protected bool _excel = false;
        /// <summary>
        /// Chart Type
        /// </summary>
        protected ChartImageType _chartType = ChartImageType.Flash;
        /// <summary>
        /// Include Advertiser dimension
        /// </summary>
        protected bool _withAdvertiser = false;
        /// <summary>
        /// Include Reference dimension
        /// </summary>
        protected bool _withReference = false;
        #endregion

		#region Accesseurs
		/// <summary>
		/// Get / Set User session
		/// </summary>
		public WebSession Session{
			set{_session = value;}
			get{return _session;}
		}
		/// <summary>
		/// Get / Set Big Size Param
		/// </summary>
		public bool BigSize{
			set{_bigSize = value;}
            get { return _bigSize; }
		}
        /// <summary>
        /// Get / Set Excel format ?
        /// </summary>
        [Bindable(true)]
        public bool Excel
        {
            get { return _excel; }
            set { _excel = value; }
        }
        /// <summary>
        /// Get / Set Chart Type
        /// </summary>
        public ChartImageType ChartType{
            get { return _chartType; }
            set { _chartType = value; }
        }
        #endregion
		
		#region Evènements

		#region Load
		/// <summary>
		/// Load components
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnLoad(EventArgs e){
			Controls.Clear();
		
			//Graphical mode
            if (_session != null && !_excel && _session.Graphics && _session.CurrentTab != MotherRecap.NOVELTY && _session.CurrentTab != MotherRecap.SYNTHESIS)
            {
                Navigation.Module module = ModulesList.GetModule(CstWeb.Module.Name.INDICATEUR);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Indicator result"));
                object[] param = new object[1] { _session };
                IProductClassIndicators productClassIndicator = (IProductClassIndicators)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                productClassIndicator.ChartType = _chartType;
				switch(_session.CurrentTab){
					case MotherRecap.MEDIA_STRATEGY :
                        _advertiserChart = productClassIndicator.GetMediaStrategyChart();
						break;
                    case MotherRecap.PALMARES:
                        _withAdvertiser = true;
						//_withReference = true;
                        _advertiserChart = productClassIndicator.GetTopsChart(MotherRecap.ElementType.advertiser);
						if (_session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) {
							_withReference = true;
							_referenceChart = productClassIndicator.GetTopsChart(MotherRecap.ElementType.product);
						}
                        break;
					case MotherRecap.SEASONALITY:
                        _advertiserChart = productClassIndicator.GetSeasonalityChart(_bigSize);
                        break;
                    case EvolutionRecap.EVOLUTION:
                        _withAdvertiser = true;                        
                        _advertiserChart = productClassIndicator.GetEvolutionChart(MotherRecap.ElementType.advertiser);
						if (_session.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) {
							_referenceChart = productClassIndicator.GetEvolutionChart(MotherRecap.ElementType.product);
							_withReference = true;
						}
                        break;
                    default:
                        break;
                }
                if (_advertiserChart != null)
                {
                    _advertiserChart.ID = "advertiserChartWebControl_" + this.ID;
                    Controls.Add(_advertiserChart);
                }
                if (_referenceChart != null) {
                    _referenceChart.ID = "referenceChartWebControl_" + this.ID;
                    Controls.Add(_referenceChart);
                }
            }

			//Table mode
            else if (_session != null)
            {
					_tableChart = new ProductClassTableWebControl(_session);
                    _tableChart.Excel = _excel;
                    _tableChart.ID = "tableChartWebControl_" + this.ID;
					_tableChart.AjaxProTimeOut = 240000;
                    Controls.Add(_tableChart);
			}			
			base.OnLoad (e);
		}
		
		#endregion
		
		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {
            if (_withReference || _withAdvertiser)
            {
                if (_withAdvertiser && !_advertiserChart.Visible)
                {
                    output.WriteLine("<br><table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
                    output.WriteLine("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                    output.WriteLine("{0} {1}", GestionWeb.GetWebWord(177, _session.SiteLanguage), GestionWeb.GetWebWord(1239, _session.SiteLanguage));
                    output.WriteLine("</td></tr></table>");
                }
                if (_withReference && !_referenceChart.Visible)
                {
                    output.WriteLine("<br><table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
                    output.WriteLine("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                    output.WriteLine("{0} {1}", GestionWeb.GetWebWord(177, _session.SiteLanguage), GestionWeb.GetWebWord(1238, _session.SiteLanguage));
                    output.WriteLine("</td></tr></table>");
                }
            }
            else if ((_advertiserChart != null && !_advertiserChart.Visible) || (_referenceChart != null && !_referenceChart.Visible))
            {
                output.WriteLine("<br><table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
                output.WriteLine("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                output.WriteLine("{0}", GestionWeb.GetWebWord(177, _session.SiteLanguage));
                output.WriteLine("</td></tr></table>");
            }
			base.Render(output);

		}
		#endregion

		#endregion

		
	}
}
