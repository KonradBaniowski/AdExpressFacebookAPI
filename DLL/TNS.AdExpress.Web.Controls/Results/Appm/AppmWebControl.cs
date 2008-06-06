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
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebSystem=TNS.AdExpress.Web.BusinessFacade;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.UI.Results.MediaPlanVersions;
using CustomerCst=TNS.AdExpress.Constantes.Customer;
using APPMUIs = TNS.AdExpress.Web.UI.Results.APPM;
using TNS.AdExpress.Domain.Translation;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Controls.Results.Appm{
	/// <summary>
	/// Résultat de l'appm
	/// </summary>
	[ToolboxData("<{0}:AppmWebControl runat=server></{0}:AppmWebControl>")]
	public class AppmWebControl : TNS.AdExpress.Web.Controls.AjaxBaseWebControl{
		
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
			try{
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
		protected override void OnPreRender(EventArgs e) {
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
		/// Calcul le résultat du plan media
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTMl</returns>
		private string GetHTML(WebSession webSession){
			StringBuilder html=new StringBuilder(10000);
			MediaPlanResultData result=null;
			try{
				
				#region Obtention du résultat 
				if(webSession!=null)
					_dataSource=webSession.Source;
				
				if(webSession.CurrentTab==APPM.mediaPlanByVersion){
					#region Paramétrage des dates
					//Formatting date to be used in the tabs which use APPM Press table
					int dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
					int dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
					#endregion

					#region targets
					//base target
                    Int64 idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmBaseTargetAccess));
					//additional target
                    Int64 idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmTargetAccess));									
					#endregion

					#region Wave
                    Int64 idWave=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave,CustomerCst.Right.type.aepmWaveAccess));									
					#endregion
                    string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
					result =  APPMUIs.MediaPlanUI.GetWithVersionHTML(webSession,_dataSource,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget,false);
					
					if(result!=null && result.HTMLCode!=null && result.HTMLCode.Length>0){
						#region Construction du tableaux global
						html.Append("<table cellspacing=\"0\" cellpadding=\"0\" align=\"left\" border=\"0\">");
						#endregion

						#region Revenir aux versions sans zoom
						if(webSession.SloganIdZoom>0){
                            html.Append("\r\n\t<tr align=\"left\" class=\"violetBackGroundV3\">\r\n\t\t<td>");
                            html.Append("<a class=\"roll06\" href=\"javascript:get_back();\" onmouseover=\"back_" + this.ID + ".src='/App_Themes/" + themeName + "/Images/Common/button/back_down.gif';\" onmouseout=\"back_" + this.ID + ".src='/App_Themes/" + themeName + "/Images/Common/button/back_up.gif';\"><img align=\"absmiddle\" name=\"back_" + this.ID + "\" border=0 src=\"/App_Themes/" + themeName + "/Images/Common/button/back_up.gif\">&nbsp;" + GestionWeb.GetWebWord(1978, webSession.SiteLanguage) + "</a>");
							html.Append("\r\n\t\t</td>\r\n\t</tr>");
						}
						#endregion

						VersionsPluriMediaUI versionsUI=new VersionsPluriMediaUI(webSession,result.VersionsDetail);
                        html.Append("\r\n\t<tr class=\"violetBackGroundV3\">\r\n\t\t<td>");
						html.Append(versionsUI.GetHtml());
						html.Append("\r\n\t\t</td>\r\n\t</tr>");
						html.Append("\r\n\t<tr height=\"1\">\r\n\t\t<td>");
						html.Append("\r\n\t\t</td>\r\n\t</tr>");
						html.Append("\r\n\t<tr>\r\n\t\t<td>");
						html.Append(result.HTMLCode);
						html.Append("\r\n\t\t</td>\r\n\t</tr>");
						html.Append("</table>");
					}else {
						html.Append("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage));					
						html.Append("</div>");
					}
				}
				else {
					html.Append(WebSystem.Results.APPMSystem.GetHtml(this.Page,null,webSession,this._dataSource));
				}
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
