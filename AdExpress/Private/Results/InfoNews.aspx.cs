#region Informations
// Auteur: B. Masson
// Date de cr�ation: 16/11/2004 
// Date de modification: 19/11/2004 
//    31/12/2004 A. Obermeyer Int�gration de WebPage
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

using WebCst = TNS.AdExpress.Constantes.Web;
using TradCst = TNS.AdExpress.Constantes.DB.Language;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Web;
using System.Reflection;
using  TNS.AdExpressI.InfoNews;
namespace AdExpress.Private.Results{
	/// <summary>
	/// Module infos/news
	/// </summary>
	public partial class InfoNews : TNS.AdExpress.Web.UI.PrivateWebPage{

		#region Variable MMI
		/// <summary>
		/// Contr�le En t�te de page
		/// </summary>
		/// <summary>
		/// Titre de la page
		/// </summary>
		#endregion
				
		#region Variables
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idsession;		
		/// <summary>
		/// Affichage du tableau de r�sultat
		/// </summary>
		public string result;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public InfoNews():base(){
			// Chargement de la Session
			try{				
				idsession=HttpContext.Current.Request.QueryString.Get("idSession");	
			}
			catch(System.Exception){
				Response.Write(WebFunctions.Script.ErrorCloseScript("Your session is unavailable. Please reconnect via the Homepage"));
				Response.Flush();	
			}
		}
		#endregion
		
		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{
	
				#region Textes et Langage du site
				//Modification de la langue pour les Textes AdExpress
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);
			
				HeaderWebControl1.ActiveMenu = WebCst.MenuTraductions.NEWS;
                HeaderWebControl1.Language = _webSession.SiteLanguage;
                CustomizePageTitleWebControl1.Language = _webSession.SiteLanguage;
                CustomizePageTitleWebControl1.CodeDescriptionList = "2846," + WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CompanyNameTexts.CompanyNameCode + ",2847";
				#endregion

				#region Script
				// Ouverture/fermeture des fen�tres p�res
				if (!Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {
					//page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DivDisplayer",TNS.AdExpress.Web.Functions.Script.DivDisplayer());
					Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "DivDisplayer", TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				}
				#endregion

				#region R�sultat
				TNS.AdExpress.Domain.Results.InfoNews infosnews =  WebApplicationParameters.InfoNewsInformations;
				if (infosnews != null) {
					if (infosnews.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the InfoNews result"));
					object[] parameters = new object[2];
					parameters[0] = _webSession;
					parameters[1] = Page.Theme;

					TNS.AdExpressI.InfoNews.IInfoNewsResult infoNewsResult = (TNS.AdExpressI.InfoNews.IInfoNewsResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + infosnews.CountryRulesLayer.AssemblyName, infosnews.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
					result = infoNewsResult.GetHtml();
				}
				#endregion

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){
			
		}
		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
		/// </summary>
		private void InitializeComponent()
		{
          
            this.Unload += new System.EventHandler(this.Page_UnLoad);
		}
		#endregion

	}
}
