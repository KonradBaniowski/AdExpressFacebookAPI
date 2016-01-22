#region Informations
// Auteur: A. DADOUCH
// Date de création: 23/08/2005
#endregion

using System;
using TNS.AdExpress.Web.UI.MyAdExpress;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.Alert.Domain;
using TNS.AdExpress.Domain.Web;

namespace AdExpress.Private.MyAdExpress{
	/// <summary>
	/// Page "Mon téléchargement" pour la mise à disposition des fichiers PDF de l'APPM
	/// </summary>
	public partial class PdfFiles : TNS.AdExpress.Web.UI.PrivateWebPage{
		
		#region MMI		
		/// <summary>
		/// Bouton Supprimer
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl deleteImageButtonRollOverWebControl;		
		/// <summary>
		///  Bouton ouvrir pdf
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl pdfopenImageButtonRollOverWebControl;			
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

        #region Properties

        public bool IsAlertsActivated
        {
            get {
                return (AlertConfiguration.IsActivated && _webSession.CustomerLogin.HasModuleAssignmentAlertsAdExpress()); 
            }
        }
        /// <summary>
        /// Get if can save insertion customised levels
        /// </summary>
        public bool CanSaveInsertionCustomisedLevels
        {
            get
            {
                return (WebApplicationParameters.InsertionOptions.CanSaveLevels);
            }
        }
        #endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
        protected void Page_Load(object sender, EventArgs e) {

            try {

                #region Textes et Langage du site            

                HeaderWebControl1.ActiveMenu = CstWeb.MenuTraductions.MY_ADEXPRESS;
                #endregion

                #region Résultat
                MySessionsUI myAdexpress = new MySessionsUI(_webSession, MySessionsUI.type.mySession, WebApplicationParameters.CustomStyles.MySessionsWidth);
                result = TNS.AdExpress.Web.BusinessFacade.Results.PdfFilesSystem.GetHtml(Page, _webSession, _dataSource);
                #endregion
             
                // Gestion lorsqu'il n'y a pas de répertoire
                if(result.Length == 0) {
                    AdExpressText6.Code = 833;
                }

                #region Script
                // script
                if(!Page.ClientScript.IsClientScriptBlockRegistered("script")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "script", myAdexpress.Script);
                }
                // popup
                if(!Page.ClientScript.IsClientScriptBlockRegistered("myAdExpress")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "myAdExpress", TNS.AdExpress.Web.Functions.Script.MyAdExpress(idSession, _webSession));
                }
                // Champ hidden 
                if(!Page.ClientScript.IsClientScriptBlockRegistered("insertHidden")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "insertHidden", TNS.AdExpress.Web.Functions.Script.InsertHidden());
                }
                #endregion

            }
            catch(Exception exc) {
                if(exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
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
				//_webSession.Source.Close();
                _webSession.Source.Close();
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
                _webSession.Source.Close();
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
                _webSession.Source.Close();
				Response.Redirect("/Private/Universe/PersonnalizeUniverse.aspx?idSession="+_webSession.IdSession+"");
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

        #region Bouton Alertes

        protected void alertOpenImageButtonRollOver_Click(object sender, System.EventArgs e)
        {
            try
            {
                _webSession.Source.Close();
                Response.Redirect("/Private/Alerts/ShowAlerts.aspx?idSession=" + _webSession.IdSession + "");
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }
        #endregion

        #region Bouton Alertes Personnaliser
        /// <summary>
        /// Gestion du bouton Personnaliser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void personalizeAlertesImagebuttonrolloverwebcontrol_Click(object sender, System.EventArgs e) {
            try {
                _webSession.Source.Close();
                Response.Redirect("/Private/Alerts/PersonalizeAlerts.aspx?idSession=" + _webSession.IdSession + "");
            }
            catch (System.Exception exc) {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }
        #endregion

		
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

        /// <summary>
        /// Open insertion saved pages
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">arguments</param>
        protected void insertionOpenImageButtonRollOverWebControl_Click(object sender, EventArgs e)
        {
            try
            {
                _webSession.Source.Close();
                Response.Redirect("/Private/MyAdExpress/PersonnalizeInsertion.aspx?idSession=" + _webSession.IdSession + "");
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }
}
}

