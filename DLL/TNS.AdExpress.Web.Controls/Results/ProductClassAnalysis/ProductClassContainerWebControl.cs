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
        protected WebControl _tableChart = null;
        /// <summary>
        /// Big Charts ?
        /// </summary>
        protected bool _bigSize = false;
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
            if (_session != null && _session.Graphics && _session.CurrentTab != MotherRecap.NOVELTY && _session.CurrentTab != MotherRecap.SYNTHESIS)
            {
                Navigation.Module module = ModulesList.GetModule(CstWeb.Module.Name.INDICATEUR);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Indicator result"));
                object[] param = new object[1] { _session };
                IProductClassIndicators productClassIndicator = (IProductClassIndicators)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);

				switch(_session.CurrentTab){
					case MotherRecap.MEDIA_STRATEGY :
                        _advertiserChart = productClassIndicator.GetMediaStrategyChart();
						break;
                    case MotherRecap.PALMARES:
                        _advertiserChart = productClassIndicator.GetTopsChart(MotherRecap.ElementType.advertiser);
                        _referenceChart = productClassIndicator.GetTopsChart(MotherRecap.ElementType.product);
                        break;
					case MotherRecap.SEASONALITY:
                        _advertiserChart = productClassIndicator.GetSeasonalityChart(_bigSize);
                        break;
                    case EvolutionRecap.EVOLUTION:
                        _advertiserChart = productClassIndicator.GetEvolutionChart(MotherRecap.ElementType.advertiser);
                        _referenceChart = productClassIndicator.GetEvolutionChart(MotherRecap.ElementType.product);
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
                    //_tableChart.Session= _session;
                    _tableChart.ID = "tableChartWebControl_" + this.ID;
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
			base.Render(output);
		}
		#endregion

		#endregion

		
	}
}
