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

using TNS.AdExpress.Web.Core.Sessions;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.FrameWork.Results;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebSystem=TNS.AdExpress.Web.BusinessFacade;

using Dundas.Charting.WebControl;

using AjaxPro;

using Oracle.DataAccess.Client;

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
        protected WebControl _refrenceChart = null;
        /// <summary>
        /// Advertiser level chart
        /// </summary>
        protected WebControl _advertiserChart = null;
        /// <summary>
        /// Report as table
        /// </summary>
        protected WebControl _tableChart = null;
        #endregion

		#region Accesseurs
		/// <summary>
		/// Get / Set User session
		/// </summary>
		public WebSession Session{
			set{_session = value;}
			get{return _session;}
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
            if (_session != null && _session.Graphics && _session.CurrentTab != MotherRecap.NOVELTY)
            {
				switch(_session.CurrentTab){
					case MotherRecap.MEDIA_STRATEGY :
						break;
                    case MotherRecap.PALMARES:
						break;
					case MotherRecap.SEASONALITY:
						break;
                    default:
                        break;
                }
                if (_refrenceChart != null) {
                    _refrenceChart.ID = "referenceChartWebControl_" + this.ID;
                    Controls.Add(_refrenceChart);
                }
                if (_advertiserChart != null)
                {
                    _advertiserChart.ID = "advertiserChartWebControl_" + this.ID;
                    Controls.Add(_advertiserChart);
                }
            }

			//Table mode
            else if (_session != null && !_session.Graphics)
            {
					_tableChart = new ProductClassTableWebControl();
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
