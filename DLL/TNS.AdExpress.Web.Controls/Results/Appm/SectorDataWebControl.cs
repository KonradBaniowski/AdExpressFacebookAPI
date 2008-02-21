#region Informations
// Auteur: Y. R'kaina 
// Date de création: 15/01/2007
#endregion

using System;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using AjaxPro;

using TNS.AdExpress.Web.Core.Sessions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebSystem=TNS.AdExpress.Web.BusinessFacade;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.UI.Results.MediaPlanVersions;
using TNS.AdExpress.Constantes.Customer;
using APPMUIs = TNS.AdExpress.Web.UI.Results.APPM;
using TNS.AdExpress.Web.Core.Translation;
using Oracle.DataAccess.Client;

namespace TNS.AdExpress.Web.Controls.Results.Appm
{
	/// <summary>
	/// Résultat de Données de cadrage
	/// </summary>
	[ToolboxData("<{0}:SectorDataWebControl runat=server></{0}:SectorDataWebControl>")]
	public class SectorDataWebControl : TNS.AdExpress.Web.Controls.AjaxBaseWebControl{
	
		/// <summary>
		/// Source de données
		/// </summary>
        protected TNS.FrameWork.DB.Common.IDataSource _dataSource = null;

		#region Javascript
		private string AjaxVersionEventScript(){
			StringBuilder js=new StringBuilder(2000);
			js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");

			js.Append("\r\nfunction get_back(){");
			js.Append("\r\n\tvar oN=document.getElementById('res_"+this.ID+"');");
			js.Append("\r\n\toN.innerHTML='"+GetLoadingHTML()+"';");
			js.Append("\r\n\t"+this.GetType().Namespace+"."+this.GetType().Name+".GetBack('"+_customerWebSession.IdSession+"',get_back_callback);");
			js.Append("\r\n}");

			js.Append("\r\nfunction get_back_callback(res){");
			js.Append("\r\n\tvar oN=document.getElementById('res_"+this.ID+"');");
			js.Append("\r\n\toN.innerHTML=res.value;");
			js.Append("\r\n}\r\n");

			js.Append("\r\nfunction get_version(versionId){");
			js.Append("\r\n\t"+this.GetType().Namespace+"."+this.GetType().Name+".GetVersionData(versionId,'"+_customerWebSession.IdSession+"',get_version_callback);");
			js.Append("\r\n}");

			js.Append("\r\nfunction get_version_callback(res){");
			js.Append("\r\n\tvar oN=document.getElementById('res_"+this.ID+"');");
			js.Append("\r\n\toN.innerHTML=res.value;");
			js.Append("\r\n}\r\n");

			js.Append("\r\n-->\r\n</SCRIPT>");
			return(js.ToString());
		}
		#endregion

		#region Implémentation des méthodes abstraites + Autres Ajax
		/// <summary>
		/// Obtention du code HTML à insérer dans le composant
		/// </summary>
		/// <param name="sessionId">Session du client</param>
		/// <returns>Code HTML</returns>
		[AjaxPro.AjaxMethod]
		public override string GetData(string sessionId){
			WebSession webSession=null;
			
			string html = null;
			try{
				#region Obtention de la session
				webSession=(WebSession)WebSession.Load(sessionId);
				#endregion

				html=GetHTML(webSession);
				webSession.Save();
			}
			catch(System.Exception err){
				return(OnAjaxMethodError(err,webSession));
			}
			return(html);
		}

		/// <summary>
		/// Obtention du code HTML à insérer dans le composant pour le zoom d'une version
		/// </summary>
		/// <param name="sessionId">Session du client</param>
		/// <param name="versionId">Identifiant de la version</param>
		/// <returns>Code HTML</returns>
		[AjaxPro.AjaxMethod]
		public string GetVersionData(string versionId,string sessionId){
			WebSession webSession=null;
			string html;
			try
			{
				#region Obtention de la session
				webSession=(WebSession)WebSession.Load(sessionId);
				#endregion

				#region Mise à jour des version
				webSession.SloganIdZoom=Int64.Parse(versionId);
				webSession.Save();
				#endregion

				html=GetHTML(webSession);
			}
			catch(System.Exception err){
				return(OnAjaxMethodError(err,webSession));
			}

			return(html);
		
		}

		/// <summary>
		/// Obtention du code HTML avec annulationdu zoom d'une version
		/// </summary>
		/// <param name="sessionId">Session du client</param>
		/// <returns>Code HTML</returns>
		[AjaxPro.AjaxMethod]
		public string GetBack(string sessionId){
			WebSession webSession=null;
			string html;
			try{
				#region Obtention de la session
				webSession=(WebSession)WebSession.Load(sessionId);
				#endregion

				#region Mise à jour des version
				webSession.SloganIdZoom=-1;
				webSession.Save();
				#endregion

				html=GetHTML(webSession);
			}
			catch(System.Exception err){
				return(OnAjaxMethodError(err,webSession));
			}

			return(html);
		}
		#endregion

		#region PréRender
		/// <summary>
		/// Prérendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e){
			base.OnPreRender (e);
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("openCreation"))this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"openCreation",WebFunctions.Script.OpenCreation());
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("openPressCreation"))this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"openPressCreation",WebFunctions.Script.OpenPressCreation());
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("Popup"))this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"Popup",WebFunctions.Script.Popup());
		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
			output.Write(AjaxVersionEventScript());
			base.Render(output);
		}
		#endregion

		#region Méthodes internes
		/// <summary>
		/// Calcul le résultat
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTMl</returns>
		private string GetHTML(WebSession webSession){
			StringBuilder html=new StringBuilder(10000);
			
			try{
				#region Obtention du résultat 
				if(webSession!=null)
					_dataSource=new OracleDataSource(new OracleConnection(webSession.CustomerLogin.OracleConnectionString));
				
				html.Append(WebSystem.Results.SectorDataSystem.GetHtml(webSession));
				#endregion
			}
			catch(System.Exception err){
				return(OnAjaxMethodError(err,webSession));
			}
			return html.ToString();
		}
		#endregion

	}
}
