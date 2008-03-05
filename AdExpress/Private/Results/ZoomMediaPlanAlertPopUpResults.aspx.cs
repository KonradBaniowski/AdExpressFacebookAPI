#region Information
// Auteur : 
// Créé le : 
// Date de modification :  
//	12/01/2006	B.Masson > Ajout icone pour export excel des unités (Plan média)
#endregion

#region Namespace
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Domain.Translation;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using BusinessFacadeResults=TNS.AdExpress.Web.BusinessFacade.Results;
using WebConstantes = TNS.AdExpress.Constantes.Web;
#endregion

namespace AdExpress.Private.Results{
	/// <summary>
	///  Pop-up Zoom du plan media
	/// </summary>
	public partial class ZoomMediaPlanAlertPopUpResults : TNS.AdExpress.Web.UI.PrivateWebPage{

		#region Variables
		/// <summary>
		/// Résultat
		/// </summary>
		public string result="";
		/// <summary>
		/// Nombre de page de retour en arrière
		/// </summary>
		public int backPageNb = 1;
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// Contextual Menu
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.InformationWebControl InformationWebControl1;
		protected TNS.AdExpress.Web.Controls.Headers.InformationWebControl InformationWebControl2;
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idSession="";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public ZoomMediaPlanAlertPopUpResults():base(){			
			idSession=HttpContext.Current.Request.QueryString.Get("idSession");
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region Variables
				System.Text.StringBuilder HtmlTxt = new System.Text.StringBuilder(3000);
				string zoomDate = string.Empty;
				#endregion

				#region Gestion du flash d'attente
				Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
				Page.Response.Flush();
				#endregion

                #region Script
                if (!this.Page.ClientScript.IsClientScriptBlockRegistered("OpenCreatives")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenCreatives", WebFunctions.Script.OpenCreatives());
                #endregion

                #region Résultat
                try {
					zoomDate = Page.Request.QueryString.Get("zoomDate");

					result = BusinessFacadeResults.MediaPlanSystem.GetHtml(this,this._webSession,this._dataSource,zoomDate,Page.Request.Url.AbsolutePath);

				}
				catch(System.Exception){
					Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(958, _webSession.SiteLanguage)));
				}
				#endregion

				#region TEMP : Script pour détection flash pour info sur le clic droit
				if (!Page.ClientScript.IsClientScriptBlockRegistered("detectFlash")){
					string tmp = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/scripts/FlashChecking.js\"></SCRIPT>";
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"detectFlash",tmp);
				}
				#endregion

				if(result.Length > 0){

					HtmlTxt.Append("<TABLE  bgColor=\"#ffffff\" style=\"MARGIN-TOP: 25px; MARGIN-LEFT: 0px; MARGIN-RIGHT: 25px;\"");
					HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"3\" align=\"center\" border=\"0\">");

					#region TEMP : Info sur le clic droit de la souris
					HtmlTxt.Append("<tr><td>");
					HtmlTxt.Append("\n<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#DED8E5\">");
					HtmlTxt.Append("\n<tr><td>");
					HtmlTxt.Append("\n<script language=\"javascript\" type=\"text/javascript\">");
					HtmlTxt.Append("\nif(hasRightFlashVersion==true){");
					HtmlTxt.Append("\ndocument.writeln('<object id=\"infoOptionFlash\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\" width=\"400\" height=\"20\" VIEWASTEXT>');");
					HtmlTxt.Append("\ndocument.writeln('<param name=\"movie\" value=\"/Flash/"+_siteLanguage+"/infoOptionsOneLine.swf\">');");
					HtmlTxt.Append("\ndocument.writeln('<param name=\"quality\" value=\"high\">');");
					HtmlTxt.Append("\ndocument.writeln('<param name=\"menu\" value=\"false\">');");
					HtmlTxt.Append("\ndocument.writeln('<param name=\"wmode\" value=\"transparent\">');");
					HtmlTxt.Append("\ndocument.writeln('<embed src=\"/Flash/"+_siteLanguage+"/infoOptionsOneLine.swf\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" width=\"400\" height=\"20\"></embed>');");
					HtmlTxt.Append("\ndocument.writeln('</object></td>');");
					HtmlTxt.Append("\n}\nelse{");
					HtmlTxt.Append("\ndocument.writeln('<img src=\"/Images/"+_siteLanguage+"/FlashReplacement/infoOptionsOneLine.gif\"></td>');");
					HtmlTxt.Append("\n}");
					HtmlTxt.Append("\n</script>");
					HtmlTxt.Append("\n</tr>");
					HtmlTxt.Append("\n</table>");
					HtmlTxt.Append("</td></tr>");
					#endregion				

					#region Libellé du module et période
					//libéllé alerte plan média et période
					HtmlTxt.Append("<tr height=\"10\" vAlign=\"center\"><td class=\"txtViolet14Bold\">");
					HtmlTxt.Append(GestionWeb.GetWebWord(751,_webSession.SiteLanguage) 
                        );
                        //+ " ( " + GestionWeb.GetWebWord(896, _webSession.SiteLanguage) + WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType),_webSession.SiteLanguage)
                        //+ GestionWeb.GetWebWord(897, _webSession.SiteLanguage) + WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType),_webSession.SiteLanguage)+ " )");
					HtmlTxt.Append("");
					HtmlTxt.Append("</td></tr><tr><td>");
					#endregion

					result=HtmlTxt.ToString()+result+"</td></tr></table>";

					MenuWebControl2.ForcePrint = "/Private/Results/Excel/ZoomMediaPlanAnalysisResults.aspx?idSession=" + this._webSession.IdSession + "&zoomDate=" + zoomDate ;
					MenuWebControl2.ForceExcelUnit = "/Private/Results/ValueExcel/ZoomMediaPlanAnalysisResults.aspx?idSession=" + this._webSession.IdSession + "&zoomDate=" + zoomDate ;

				}
			
				#region MAJ Session
				//Sauvegarde de la session
				_webSession.Save();
				#endregion

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region DeterminePostBack
		/// <summary>
		/// Détermine la valeur de PostBack
		/// Initialise la propriété CustomerSession des composants "options de résultats" et gestion de la navigation"
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();

			MenuWebControl2.CustomerWebSession = this._webSession;
			MenuWebControl2.ForbidHelpPages = true;

			return tmp;
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation de la page
		/// </summary>
		/// <param name="e">Argumznts</param>
		override protected void OnInit(EventArgs e){
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent(){
           

		}
		#endregion
	}
}
