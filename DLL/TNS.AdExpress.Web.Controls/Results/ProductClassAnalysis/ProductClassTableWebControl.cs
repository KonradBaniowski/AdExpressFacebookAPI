#region Informations
// Auteur: G. Facon 
// Date de création: 21/07/2006
// Date de modification:
#endregion

using System;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using AjaxPro;

using Navigation = TNS.AdExpress.Domain.Web.Navigation;
using CstWeb = TNS.AdExpress.Constantes.Web;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.ProductClassIndicators;
using System.Reflection;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork;

namespace TNS.AdExpress.Web.Controls.Results.ProductClassAnalysis{
	/// <summary>
	/// Product class indicators : report as a table
	/// </summary>
    [ToolboxData("<{0}:ProductClassTableWebControl runat=server></{0}:ProductClassTableWebControl>")]
    public class ProductClassTableWebControl : TNS.AdExpress.Web.Controls.AjaxBaseWebControl
    {

        #region Variables
        /// <summary>
        /// Excel format?
        /// </summary>
        protected bool _excel = false;
        #endregion

        #region Accesseurs
        /// <summary>
        /// Get / Set Excel format ?
        /// </summary>
        public bool Excel
        {
            get { return _excel; }
            set { _excel = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassTableWebControl(WebSession session)
        {
            _customerWebSession = session;
            this._ajaxProTimeOut = 150;
        }
        /// <summary>
        /// Default Constructor
        /// </summary>
        [Obsolete()]
        public ProductClassTableWebControl(){}
        #endregion

        #region Abstract method implemantation
        /// <summary>
		/// Get HTML to inject
		/// </summary>
		/// <param name="sessionId">User session id</param>
		/// <returns>Code HTML</returns>
		[AjaxPro.AjaxMethod]
		public override string GetData(string sessionId){
			WebSession webSession=null;
			
			string html = null;
			
            try{

				_customerWebSession=(WebSession)WebSession.Load(sessionId);

                html = GetHTML(_customerWebSession);

			}
			catch(System.Exception err){
				return(OnAjaxMethodError(err,webSession));
			}
			
            return(html);
		}
		#endregion

        #region Render
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!_excel)
            {
                base.RenderControl(writer);
            }
            else
            {
                StringBuilder t = new StringBuilder();
                //header
                t.AppendLine(ExcelWebPage.GetLogo(_customerWebSession));
                t.AppendLine(ExcelWebPage.GetExcelHeader(_customerWebSession, false, true, false, GestionWeb.GetWebWord(1310, _customerWebSession.SiteLanguage)));
                //result
                t.AppendLine(this.GetHTML(_customerWebSession));
                //footer
                t.AppendLine(ExcelWebPage.GetFooter(_customerWebSession));

                writer.WriteLine(Convertion.ToHtmlString(t.ToString()));

            }

        }
        #endregion

        #region Méthodes internes
        /// <summary>
		/// Compute result
		/// </summary>
		/// <param name="session">User session</param>
		/// <returns>Code HTMl</returns>
		private string GetHTML(WebSession session){

            StringBuilder html=new StringBuilder(10000);
			try{

                Navigation.Module module = ModulesList.GetModule(CstWeb.Module.Name.INDICATEUR);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Indicator result"));
                object[] param = new object[1] { _customerWebSession };
                IProductClassIndicators productClassIndicator = (IProductClassIndicators)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                productClassIndicator.Excel = _excel;

                switch (_customerWebSession.CurrentTab)
                {
                    case MotherRecap.MEDIA_STRATEGY:
                        return productClassIndicator.GetMediaStrategyTable();
                        break;
                    case MotherRecap.PALMARES:
                        return productClassIndicator.GetTopsTable();
                        break;
                    case MotherRecap.SEASONALITY:
                        return productClassIndicator.GetSeasonalityTable();
                        break;
                    case EvolutionRecap.EVOLUTION:
                        return productClassIndicator.GetEvolutionTable();
                        break;
                    case MotherRecap.NOVELTY:
                        if (_customerWebSession.Graphics)
                        {
                            return productClassIndicator.GetNoveltyChart();
                        }
                        else
                        {
                            return productClassIndicator.GetNoveltyTable();
                        }
                        break;
                    default:
                        return productClassIndicator.GetSummary();
                        break;
                }

			}
			catch(System.Exception err){
				return(OnAjaxMethodError(err,session));
			}
			return html.ToString();
		}
		#endregion

	}
}
