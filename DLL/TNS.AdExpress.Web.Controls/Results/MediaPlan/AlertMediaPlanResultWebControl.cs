#region Informations
// Auteur: G. Facon 
// Date de création: 13/07/2006
// Date de modification:
//		G Ragneau - 08/08/2006 - Set GetHtml as public so as to access it 
//		G Ragneau - 08/08/2006 - GetHTML : Force media plan alert module and restaure it after process (<== because of version zoom);
#endregion

using System;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using AjaxPro;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.UI.Results.MediaPlanVersions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.WebResultUI;
using ConstantePeriod=TNS.AdExpress.Constantes.Web.CustomerSessions.Period;

namespace TNS.AdExpress.Web.Controls.Results.MediaPlan{
	/// <summary>
	/// Affiche le résultat d'une alerte plan media
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:AlertMediaPlanResultWebControl runat=server></{0}:AlertMediaPlanResultWebControl>")]
	public class AlertMediaPlanResultWebControl : TNS.AdExpress.Web.Controls.AjaxBaseWebControl{

		#region Variables
		/// <summary>
		/// Indique si le composant doit montrer les versions
		/// </summary>
		private bool _showVersion=false;

		/// <summary>
		/// Zoom de La période sélectionnée
		/// </summary>
		protected string _zoomDate = string.Empty;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou définit le mot recherché
		/// </summary>
		[Bindable(true), 
		Category("Appearance"),
		Description("Indique si le composant doit montrer les versions")]
		public bool ShowVersion{
			get{return _showVersion;}
			set{_showVersion=value;}
		}

		#region RenderType

		///<summary>
		/// Type de rendu
		/// </summary>		
		protected RenderType _renderType=RenderType.html; 
		/// <summary>
		/// Type de rendu
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("RenderType.html"),
		Description("Type de rendu")] 
		public RenderType OutputType {
			get{return _renderType;}
			set{_renderType = value;}
		}

		/// <summary>
		///  Obtient ou définit Zoom date
		/// </summary>
		[Bindable(true),
		Category("Banner Description"),
		DefaultValue("")]
		public string ZoomDate {
			get{return _zoomDate;}
			set{_zoomDate=value;}
		}
		#endregion

		#endregion

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
			string html;
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

		#region Evènements

		#region PréRender
		/// <summary>
		/// Prérendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) 
		{
			base.OnPreRender (e);
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("OpenInsertions")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenInsertions", WebFunctions.Script.OpenInsertions());
            if(!this.Page.ClientScript.IsClientScriptBlockRegistered("OpenCreatives")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"OpenCreatives",WebFunctions.Script.OpenCreatives());
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("openPressCreation")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "openPressCreation", WebFunctions.Script.OpenPressCreation());
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("OpenInternetCreation")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenInternetCreation", WebFunctions.Script.OpenInternetCreation());
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("Popup")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Popup", WebFunctions.Script.Popup());
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("openDownload")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "openDownload", WebFunctions.Script.OpenDownload());
			
			
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

		#endregion

		#region Méthodes internes
		/// <summary>
		/// Calcul le résultat du plan media
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTMl</returns>
		public virtual string GetHTML(WebSession webSession){
			StringBuilder html=new StringBuilder(10000);
			MediaPlanResultData result=null;
			Int64 module = webSession.CurrentModule;
            //WebConstantes.CustomerSessions.Period.Type periodType = webSession.PeriodType;
            //string periodBeginningDate = webSession.PeriodBeginningDate;
            //string periodEndDate = webSession.PeriodEndDate;
            object[,] tab = null;
			try{
				
				webSession.CurrentModule = WebConstantes.Module.Name.ALERTE_PLAN_MEDIA;
                //if (webSession.PeriodType == WebConstantes.CustomerSessions.Period.Type.globalDate)
                //{
                //    webSession.PeriodType = WebConstantes.CustomerSessions.Period.Type.dateToDateMonth;
                //    webSession.PeriodBeginningDate = webSession.PeriodBeginningDate.Substring(0, 6);
                //    webSession.PeriodEndDate = webSession.PeriodEndDate.Substring(0, 6);
                //}
				tab = TNS.AdExpress.Web.Rules.Results.GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(webSession);

				if(tab!=null && tab.GetLength(0)>0){

					#region Obtention du résultat du calendrier d'action				
					result=TNS.AdExpress.Web.UI.Results.GenericMediaPlanAlertUI.GetMediaPlanAlertWithMediaDetailLevelHtmlUI(tab,webSession);
                    //result=TNS.AdExpress.Web.UI.Results.GenericMediaScheduleUI.GetHtml(tab,webSession,ConstantePeriod.DisplayLevel.monthly,"20060101","20071001");
					#endregion
				
					#region Construction du tableaux global
					html.Append("<table cellSpacing=\"0\" cellPadding=\"0\"  border=\"0\">");
					#endregion

					#region Revenir aux versions sans zoom
					if(webSession.SloganIdZoom>0){
						html.Append("\r\n\t<tr align=\"left\" bgcolor=\"#B1A3C1\">\r\n\t\t<td>");
						//todo txt en BDD
						html.Append("<a class=\"roll06\" href=\"javascript:get_back();\" onmouseover=\"back_"+this.ID+".src='/Images/Common/button/back_down.gif';\" onmouseout=\"back_"+this.ID+".src='/Images/Common/button/back_up.gif';\"><img align=\"absmiddle\" name=\"back_"+this.ID+"\" border=0 src=\"/Images/Common/button/back_up.gif\">&nbsp;" + GestionWeb.GetWebWord(1978 , webSession.SiteLanguage) + "</a>");
						html.Append("\r\n\t\t</td>\r\n\t</tr>");
					}
					#endregion

					//VersionsVehicleUI versionsVehicleUI=new VersionsVehicleUI(webSession,result.VersionsDetail,TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press);					
					
					VersionsPluriMediaUI versionsUI=new VersionsPluriMediaUI(webSession,result.VersionsDetail);					
					html.Append("\r\n\t<tr bgcolor=\"#B1A3C1\">\r\n\t\t<td>");
					html.Append(versionsUI.GetHtml());
					html.Append("\r\n\t\t</td>\r\n\t</tr>");
					
					html.Append("\r\n\t<tr height=\"1\">\r\n\t\t<td>");
					html.Append("\r\n\t\t</td>\r\n\t</tr>");
					html.Append("\r\n\t<tr>\r\n\t\t<td>");

					html.Append(result.HTMLCode);
					html.Append("\r\n\t\t</td>\r\n\t</tr>");
					html.Append("</table>");
				}else{
					html.Append("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage));
					html.Append("<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/"+webSession.SiteLanguage+"/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/"+webSession.SiteLanguage+"/button/back_up.gif';\">");
					html.Append("<img src=\"/Images/"+webSession.SiteLanguage+"/button/back_up.gif\" border=0 name=bouton></a></div>");
				}

			}
			catch(System.Exception err){
				return(OnAjaxMethodError(err,webSession));
			}
			finally{
				webSession.CurrentModule = module;
                //webSession.PeriodType = periodType;
                //webSession.PeriodBeginningDate = periodBeginningDate;
                //webSession.PeriodEndDate = periodEndDate;
			}
			return(html.ToString());
		}
		#endregion
		
	}
}

