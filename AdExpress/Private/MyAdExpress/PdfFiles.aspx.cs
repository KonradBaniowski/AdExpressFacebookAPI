#region Informations
// Auteur: A. DADOUCH
// Date de création: 23/08/2005
#endregion

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
using TradCst = TNS.AdExpress.Constantes.DB.Language;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess;
using Oracle.DataAccess.Client;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstCustomerSession=TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Web.DataAccess.MyAdExpress;
using TNS.AdExpress.Web.Core.Translation;
using TNS.FrameWork.Date;
using TNS.AdExpress.Web.Core;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebModule=TNS.AdExpress.Constantes.Web.Module;
using WebRules=TNS.AdExpress.Web.Rules;
using WebFunctions=TNS.AdExpress.Web.Functions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using WebCst = TNS.AdExpress.Constantes.Web;

namespace AdExpress.Private.MyAdExpress{
	/// <summary>
	/// Page "Mon téléchargement" pour la mise à disposition des fichiers PDF de l'APPM
	/// </summary>
	public partial class PdfFiles : TNS.AdExpress.Web.UI.WebPage{
		
		#region MMI
		/// <summary>
		/// Contrôle En tête de page
		/// </summary>
		/// <summary>
		/// Mon AdExpress
		/// </summary>
		/// <summary>
		/// Commentaire Mon AdExpress
		/// </summary>
		/// <summary>
		/// Bouton retour Menu principal de Mon AdExpress
		/// </summary>
		/// <summary>
		/// Bouton de personalisation
		/// </summary>
		/// <summary>
		/// Bouton Supprimer
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl deleteImageButtonRollOverWebControl;
		/// <summary>
		/// Sélectionnez un résultat
		/// </summary>
		/// <summary>
		/// Mes Univers
		/// </summary>
		/// <summary>
		/// Bouton ouvrir Univers
		/// </summary>
		/// <summary>
		///  Bouton ouvrir pdf
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl pdfopenImageButtonRollOverWebControl;		
		/// <summary>
		/// Commentaire Mes Univers
		/// </summary>
		
		/// <summary>
		/// Commentaire Mes pdf
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idsession;				
		/// <summary>
		/// Liste resultat
		/// </summary>
		protected string result;
		/// <summary>
		/// Script
		/// </summary>
		protected string script;
		/// <summary>
		/// id Session
		/// </summary>
		public Int64 idMySession=0;
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// idSession
		/// </summary>
		public string idSession;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public PdfFiles():base(){
			// Chargement de la Session
//			try{				
//				idsession=HttpContext.Current.Request.QueryString.Get("idSession");	
//			}
//			catch(System.Exception){
//				Response.Write(WebFunctions.Script.ErrorCloseScript("Your session is unavailable. Please reconnect via the Homepage"));
//				Response.Flush();	
//			}
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
		
			try{
	
				#region Textes et Langage du site
				//Modification de la langue pour les Textes AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
			
				HeaderWebControl1.ActiveMenu = CstWeb.MenuTraductions.MY_ADEXPRESS;
				#endregion

				#region Rollover des boutons
				ImageButtonRollOverWebControl1.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/ouvrir_up.gif";
				ImageButtonRollOverWebControl1.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/ouvrir_down.gif";

				personalizeImagebuttonrolloverwebcontrol.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/personnaliser_up.gif";
				personalizeImagebuttonrolloverwebcontrol.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/personnaliser_down.gif";

				universOpenImageButtonRollOverWebControl.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/personnaliser_up.gif";
				universOpenImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/personnaliser_down.gif";
				#endregion

				#region Résultat
				TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI myAdexpress=new TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI(_webSession,TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI.type.mySession,500);
				result = TNS.AdExpress.Web.BusinessFacade.Results.PdfFilesSystem.GetHtml(this.Page,_webSession,this._dataSource);
				#endregion

				//Charge le script
				script=myAdexpress.Script;
			
				// Gestion lorsqu'il n'y a pas de répertoire
							if(result.Length==0){
								AdExpressText6.Code=833;
							}	
			
				#region Script
				// popup
				if (!Page.ClientScript.IsClientScriptBlockRegistered("myAdExpress")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"myAdExpress",TNS.AdExpress.Web.Functions.Script.MyAdExpress(idSession,_webSession));
				}
				// Champ hidden 
				if (!Page.ClientScript.IsClientScriptBlockRegistered("insertHidden")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"insertHidden",TNS.AdExpress.Web.Functions.Script.InsertHidden());
				}
			}
				#endregion

			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e) {
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent() {
          
            this.Unload += new System.EventHandler(this.Page_UnLoad);


		}
		#endregion

		#region Bouton Ouvrir
		/// <summary>
		/// Gestion du bouton Ouvrir
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void ImageButtonRollOverWebControl1_Click(object sender, System.EventArgs e) {
			try{
				DBFunctions.closeDataBase(_webSession);
				Response.Redirect("/Private/MyAdexpress/SearchSession.aspx?idSession="+_webSession.IdSession+"");
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Bouton Personnaliser
		/// <summary>
		/// Gestion du bouton Personnaliser
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void personalizeImagebuttonrolloverwebcontrol_Click(object sender, System.EventArgs e) {
			try{
				DBFunctions.closeDataBase(_webSession);
				Response.Redirect("/Private/MyAdexpress/PersonnalizeSession.aspx?idSession="+_webSession.IdSession+"");
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Bouton Personnaliser
		/// <summary>
		/// Gestion du bouton Personnaliser
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void universOpenImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			try{
				DBFunctions.closeDataBase(_webSession);
				Response.Redirect("/Private/Universe/PersonnalizeUniverse.aspx?idSession="+_webSession.IdSession+"");
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

//		#region Bouton supprimer
//		/// <summary>
//		/// Gestion du bouton supprimer
//		/// </summary>
//		/// <param name="sender"></param>
//		/// <param name="e"></param>
//		private void deleteImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
//			try{
//			
//				string[] tabParent=null;
//				Int64 idMySession=0;
//
//				foreach (string currentKey in Request.Form.AllKeys){
//					tabParent=currentKey.Split('_');
//					if(tabParent[0]=="CKB") {
//						idMySession=Int64.Parse(tabParent[1]);		
//					}
//				}
//				if (idMySession!=0){
//					//MySessionDataAccess deleteSession=new MySessionDataAccess(idMySession,new OracleConnection(_webSession.CustomerLogin.OracleConnectionString));
//					MySessionDataAccess deleteSession=new MySessionDataAccess(idMySession,_webSession);
//					if (deleteSession.Delete()){
//						// Validation : confirmation de suppression de la requête
//						Response.Write("<script language=javascript>");
//						Response.Write("	alert(\""+GestionWeb.GetWebWord(286,_webSession.SiteLanguage)+"\");");					
//						Response.Write("</script>");
//						// Actualise la page
//						this.OnLoad(null);
//					}
//					else{
//						// Erreur : la suppression de la requête a échouée
//						Response.Write("<script language=javascript>");
//						Response.Write("	alert(\""+GestionWeb.GetWebWord(830,_webSession.SiteLanguage)+"\");");					
//						Response.Write("</script>");
//					}
//				}
//				else{
//					// Erreur : veuillez sélectionner une requête
//					Response.Write("<script language=javascript>");
//					Response.Write("	alert(\""+GestionWeb.GetWebWord(831,_webSession.SiteLanguage)+"\");");					
//					Response.Write("</script>");
//				}
//			}
//			catch(System.Exception exc){
//				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
//					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
//				}
//			}
//		}
//		#endregion
		
		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page:
		///		Fermeture des connections BD
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

	}
}

