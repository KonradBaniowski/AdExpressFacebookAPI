#region Informations
// Auteur: D. Mussuma
// Date de cr�ation: 30/05/2006 
// Date de modification: 
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
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Constantes.Web;

namespace AdExpress.Private.MyAdExpress
{
	/// <summary>
	/// Description r�sum�e de APPMExcelSavePopUp.
	/// </summary>
	public partial class APPMExcelSavePopUp : TNS.AdExpress.Web.UI.PrivateWebPage
	{
		#region Varaibles MMI
		/// <summary>
		/// Libell� du nom de fichier
		/// </summary>
		/// <summary>
		/// Libell� du mail
		/// </summary>
		/// <summary>
		/// Nom du fichier
		/// </summary>
		/// <summary>
		/// Mail
		/// </summary>
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <summary>
		/// Titre de la sauvegarde
		/// </summary>
		/// <summary>
		/// Bouton fermer
		/// </summary>
		/// <summary>
		/// Case � cocher m�moriser adresse email 
		/// </summary>

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public APPMExcelSavePopUp():base() {			
		}
		#endregion
	
		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page
		/// </summary>		
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e) {

			#region Textes et Langage du site
			TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);
			#endregion

			#region Rollover des boutons
			validateRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
			validateRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";

			closeRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/fermer_up.gif";
			closeRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/fermer_down.gif";
			#endregion

			#region Gestion des cookies

			#region Cookies enregistrement des pr�f�rences
				
			//V�rifie si le navigateur accepte les cookies
			if(Request.Browser.Cookies){
				cbxRegisterMail.Text = GestionWeb.GetWebWord(2117,_webSession.SiteLanguage);
				cbxRegisterMail.CssClass = "txtViolet11Bold";
					
				HttpCookie isRegisterEmailForRemotingExport = null, savedEmailForRemotingExport = null ;
				cbxRegisterMail.Visible = true; //RegisterMailLabel.Visible = true;

				if(!Page.IsPostBack){					
					WebFunctions.Cookies.LoadSavedEmailForRemotingExport(Page,isRegisterEmailForRemotingExport, savedEmailForRemotingExport,cbxRegisterMail,tbxMail);
				}
			}else cbxRegisterMail.Visible = false; // = RegisterMailLabel.Visible = false;

			#endregion

			#endregion

		
		}
		#endregion

		/// <summary>
		/// Femeture de la fen�tre
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void closeRollOverWebControl_Click(object sender, System.EventArgs e) {
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "closeScript", WebFunctions.Script.CloseScript());
			
		
		}

		/// <summary>
		/// Lancer une g�n�ration
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void validateRollOverWebControl_Click(object sender, System.EventArgs e) {
			string fileName=tbxFileName.Text;
			string mail=tbxMail.Text;
			if(fileName==null || mail==null || fileName.Length==0 || mail.Length==0) {
				this.ClientScript.RegisterClientScriptBlock(this.GetType(),"alert",WebFunctions.Script.Alert(GestionWeb.GetWebWord(1748,_siteLanguage)));
			}
			else if (!WebFunctions.CheckedText.CheckedMailText(mail)) {
				this.ClientScript.RegisterClientScriptBlock(this.GetType(),"alert",WebFunctions.Script.Alert(GestionWeb.GetWebWord(2041,_siteLanguage)));
			} 
			else {

				#region Gestion des cookies
					
				#region Cookies enregistrement des pr�f�rences
				
				//V�rifie si le navigateur accepte les cookies
				if(Request.Browser.Cookies){					
					WebFunctions.Cookies.SaveEmailForRemotingExport(Page,mail,cbxRegisterMail);
				}
				#endregion

				#endregion

				_webSession.ExportedPDFFileName=fileName;
				string[] mails=new string[1];
				mails[0]=mail;
				_webSession.EmailRecipient=mails;
				Int64 idStaticNavSession;
				switch(_webSession.CurrentModule){
					case Module.Name.BILAN_CAMPAGNE :
						idStaticNavSession=TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession,TNS.AdExpress.Anubis.Constantes.Result.type.appmExcel);
						break;
					case Module.Name.DONNEES_DE_CADRAGE :
						idStaticNavSession=TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession,TNS.AdExpress.Anubis.Constantes.Result.type.amset);
						break;
					//default :
                        //throw new AdExpress.Exceptions.PdfSavePopUpException(" Impossssile d'identifier le module.");
				}
				//Int64 idStaticNavSession=TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession,TNS.AdExpress.Anubis.Constantes.Result.type.appmExcel);
				closeRollOverWebControl_Click(this,null);
			}
		}

		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguements</param>
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
		private void InitializeComponent() {
           
		}
		#endregion
	}
}
