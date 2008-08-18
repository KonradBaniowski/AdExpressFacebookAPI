#region Informations
// Auteur: G. Rageau
// Date de création : 24/09/2004
// Date de modification : 
// 30/12/2004 D. Mussuma > Intégration de WebPage
// 24/10/2005 B. Masson	> Mise en place KEuros
#endregion

#region Namespace
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using WebFunction = TNS.AdExpress.Web.Functions.Script;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;

using TNS.AdExpressI.ProductClassReports;
#endregion

namespace AdExpress{
	/// <summary>
	/// Tableaux dynamiques de l'analyse sectorielle
	/// </summary>
	public partial class ASDynamicTables :  TNS.AdExpress.Web.UI.ResultWebPage{
		
		#region Variables
		/// <summary>
		/// Code HTML du résultat
		/// </summary>
		public string result="";				
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();				
		/// <summary>
		/// JKavaScripts à insérer
		/// </summary>
		public string scripts="";
		/// <summary>
		/// JKavaScripts bodyOnclick
		/// </summary>
		public string scriptBody="";		
		#endregion

		#region Variable MMI
		/// <summary>
		/// Contrôle du titre du module
		/// </summary>
		/// <summary>
		/// Contrôle des options d'analyse 
		///</summary>
		/// <summary>
		/// Contrôle de la navigation inter module (n'est pas utilisé)
		/// </summary>
		/// <summary>
		/// Contrôle du header d'AdExpress
		/// </summary>
		/// <summary>
		/// Bouton valider sélection
		/// </summary>
		/// <summary>
		/// Contextual Menu
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.InitializeProductWebControl InitializeProductWebControl1;
//		/// <summary>
//		/// Annule la personnnalisation des éléments de référence ou concurrents
//		/// </summary>
//		protected TNS.AdExpress.Web.Controls.Headers.InitializeProductWebControl InitializeProductWebControl1;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public ASDynamicTables():base(){			
			_webSession.CurrentModule = WebConstantes.Module.Name.TABLEAU_DYNAMIQUE;
			_webSession.CurrentTab = 0;
			// On réinitialise en KEuro car d'anciennes sessions peuvent être en Euro
			_webSession.Unit = WebConstantes.CustomerSessions.Unit.kEuro;
		}
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Evènement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){

			try{				
				#region Flash d'attente
				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null){
					string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
					if(nomInput!=MenuWebControl2.ID){
						Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
						Page.Response.Flush();
					}
				}
				else{
					Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
					Page.Response.Flush();
				}
				#endregion

				#region Url Suivante
//				_nextUrl=this.recallWebControl.NextUrl;
				if(_nextUrl.Length!=0){
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
				}			
				#endregion

				#region Validation du menu
				if(Page.Request.QueryString.Get("validation")=="ok"){
					_webSession.Save();				
				}
				#endregion
			
				#region Texte et langage du site
                //for (int i = 0; i < this.Controls.Count; i++) {
                //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                //}
				Moduletitlewebcontrol2.CustomerWebSession=_webSession;
				ModuleBridgeWebControl1.CustomerWebSession=_webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
//				ExportWebControl1.CustomerWebSession=_webSession;
				ValidateSelectionButton.ToolTip = GestionWeb.GetWebWord(1183,_webSession.SiteLanguage);
				#endregion
	
				#region Définition de la page d'aide
//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"ASDynamicTablesHelp.aspx";
				#endregion

				#region Calcul du résultat
				scripts = WebFunction.ImageDropDownListScripts(ResultsOptionsWebControl1.ShowPictures);
				scriptBody = "javascript:openMenuTest();";
				//result = DynamicTablesUI.GetDynamicTableUI(_webSession,false);
                TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(_webSession.CurrentModule);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Product Class Analysis"));
                object[] param = new object[1];
                param[0] = _webSession;
                IProductClassReports productClassReport = (IProductClassReports)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                result = productClassReport.GetProductClassReport();
                #endregion

                #region Script
                // Script
                if (!Page.ClientScript.IsClientScriptBlockRegistered("ImageDropDownListScripts")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ImageDropDownListScripts", scripts);
                }
                #endregion

                #region MAJ de la Session
                _webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
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

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

		#region Initialisation
		/// <summary>
		/// Initialisation des controls de la page (ViewState et valeurs modifiées pas encore chargés)
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e){
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
		private void InitializeComponent(){
          
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
			ResultsOptionsWebControl1.CustomerWebSession = _webSession;
//			recallWebControl.CustomerWebSession=_webSession;
//			InitializeProductWebControl1.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
			return tmp;
		}
		#endregion

		protected void ValidateSelectionButton_Click(object sender, System.EventArgs e) {
		}
		#endregion

		#region Abstract Methods
		/// <summary>
		/// Get next Url from contextual menu
		/// </summary>
		/// <returns></returns>
		protected override string GetNextUrlFromMenu() {
			return this.MenuWebControl2.NextUrl;
		}
		#endregion

	}
}
