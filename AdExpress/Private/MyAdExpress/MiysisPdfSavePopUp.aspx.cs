#region Informations
// Auteur: Y. Rkaina
// Date de création: 10/08/2006 
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
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Constantes.Customer;
using System.Text.RegularExpressions;

namespace AdExpress.Private.MyAdExpress{
	/// <summary>
	/// Description résumée de MiysisPdfSavePopUp.
	/// </summary>
	public partial class MiysisPdfSavePopUp : TNS.AdExpress.Web.UI.PrivateWebPage{

		#region Varaibles MMI
		/// <summary>
		/// Libellé du nom de fichier
		/// </summary>
		/// <summary>
		/// Libellé du mail
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
		

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public MiysisPdfSavePopUp():base(){			
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

			#region Textes et Langage du site
			TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);
			#endregion

			#region Rollover des boutons
			validateRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
			validateRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";

			closeRollOverWebControl.ImageUrl="/Images/"+_siteLanguage+"/button/fermer_up.gif";
			closeRollOverWebControl.RollOverImageUrl="/Images/"+_siteLanguage+"/button/fermer_down.gif";
			#endregion
		
		}
		#endregion

		/// <summary>
		/// Femeture de la fenêtre
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void closeRollOverWebControl_Click(object sender, System.EventArgs e){
			this.ClientScript.RegisterClientScriptBlock(this.GetType(),"closeScript",WebFunctions.Script.CloseScript());
		}

		/// <summary>
		/// Lancer une génération
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void validateRollOverWebControl_Click(object sender, System.EventArgs e){
			string fileName=tbxFileName.Text;
			string mail=tbxMail.Text;
			
			if(fileName==null || mail==null || fileName.Length==0 || mail.Length==0) {
				this.ClientScript.RegisterClientScriptBlock(this.GetType(),"alert",WebFunctions.Script.Alert(GestionWeb.GetWebWord(1748,_siteLanguage)));
			} 
			else if (!WebFunctions.CheckedText.CheckedMailText(mail)) {
				this.ClientScript.RegisterClientScriptBlock(this.GetType(),"alert",WebFunctions.Script.Alert(GestionWeb.GetWebWord(2041,_siteLanguage)));
			} 
			else {
				_webSession.ExportedPDFFileName=fileName;
				string[] mails=new string[1];
				mails[0]=mail;
				_webSession.EmailRecipient=mails;
				Int64 idStaticNavSession=TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession,TNS.AdExpress.Anubis.Constantes.Result.type.miysis);
				closeRollOverWebControl_Click(this,null);
			}
		}

		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguements</param>
		override protected void OnInit(EventArgs e)
		{
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
		private void InitializeComponent()
		{
            
		}
		#endregion
	}
}
