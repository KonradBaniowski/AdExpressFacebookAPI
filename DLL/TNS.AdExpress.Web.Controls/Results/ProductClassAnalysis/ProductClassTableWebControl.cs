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

using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.Controls.Results.ProductClassAnalysis{
	/// <summary>
	/// Product class indicators : report as a table
	/// </summary>
    [ToolboxData("<{0}:ProductClassTableWebControl runat=server></{0}:ProductClassTableWebControl>")]
    public class ProductClassTableWebControl : TNS.AdExpress.Web.Controls.AjaxBaseWebControl
    {

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

				html = GetHTML(webSession);

			}
			catch(System.Exception err){
				return(OnAjaxMethodError(err,webSession));
			}
			
            return(html);
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
					

			}
			catch(System.Exception err){
				return(OnAjaxMethodError(err,session));
			}
			return html.ToString();
		}
		#endregion

	}
}
