#region Informations
// Auteur: B. Masson, G.Facon
// Date de création: 14/11/2005
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
using System.Text.RegularExpressions;
using IsisCommon=TNS.Isis.Right.Common;
using Localization = Bastet.Localization;
using TNS.AdExpress.Bastet.Translation;

namespace BastetWeb{
	/// <summary>
	/// Page de gestion des emails
	/// </summary>
	public partial class MailSelection : PrivateWebPage{

		#region Variables
		/// <summary>
		/// Gestion des emails
		/// </summary>
		public string _email_manage = string.Empty;
		/// <summary>
		/// Gestion de la période
		/// </summary>
		public string _period_manage = string.Empty;
		/// <summary>
		/// Gestion des logins
		/// </summary>
		public string _login_manage = string.Empty;
		/// <summary>
		/// validation
		/// </summary>
		public string _validation = string.Empty;
		/// <summary>
		/// Ajouter un email destinataire :
		/// </summary>
		public string _label_addEmail = string.Empty;
		/// <summary>
		/// Liste des destinataires :
		/// </summary>
		public string _label_emailList = string.Empty;
		/// <summary>
		/// Email non valide
		/// </summary>
		public string _msg_err_invalid_email = string.Empty;
		/// <summary>
		/// Le champs ne peut pas être vide
		/// </summary>
		public string _msg_err_champ_notNull = string.Empty;
		/// <summary>
		/// La liste est vide
		/// </summary>
		public string _msg_err_list_empty = string.Empty;
		/// <summary>
		/// La liste est vide, vous devez préciser au moins 1 email
		/// </summary>
		public string _msg_err_list_empty_enter_one_email = string.Empty;
		#endregion

		#region Variables MMI
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{
				if(Session["Login"] == null) throw(new SystemException("Aucun login en session"));

				if(!IsPostBack){
                    mailListBox.Items.Add(((IsisCommon.Login)Session["Login"]).LoginContact.Email);
				}
                HeaderWebControl1.LanguageId = _siteLanguage;
                HeaderWebControl1.Type_de_page = TNS.AdExpress.Bastet.WebControls.PageType.generic;
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new ErrorEventArgs(this, exc));
				}
			}
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
            base.OnInit(e);
			InitializeComponent();
			InitializeComponentCustom();
		}

		/// <summary>
		/// Initialisation des composants
		/// </summary>
		private void InitializeComponentCustom(){

			// Etapes durant navigation dans le site
            _email_manage = GestionWeb.GetWebWord(6, _siteLanguage);
            _period_manage = GestionWeb.GetWebWord(7, _siteLanguage);
            _login_manage = GestionWeb.GetWebWord(8, _siteLanguage);
            _validation = GestionWeb.GetWebWord(9, _siteLanguage);
			
			// Textes
            _label_addEmail = _validation = GestionWeb.GetWebWord(10, _siteLanguage);
            _label_emailList = _validation = GestionWeb.GetWebWord(11, _siteLanguage);
			
			// Boutons
            this.validateButton.Text = _validation = GestionWeb.GetWebWord(1, _siteLanguage);
            this.addMailButton.Text = _validation = GestionWeb.GetWebWord(2, _siteLanguage);
            this.deleteMailButton.Text = _validation = GestionWeb.GetWebWord(3, _siteLanguage);
			
			// Messages
            _msg_err_invalid_email = GestionWeb.GetWebWord(26, _siteLanguage);
            _msg_err_champ_notNull = GestionWeb.GetWebWord(27, _siteLanguage);
            _msg_err_list_empty = GestionWeb.GetWebWord(28, _siteLanguage);
            _msg_err_list_empty_enter_one_email = GestionWeb.GetWebWord(29, _siteLanguage);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent(){    

		}
		#endregion

		#region Ajouter un email
		/// <summary>
		/// Ajouter un email
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void addMailButton_Click(object sender, System.EventArgs e) {
			try{
				if(mailTextBox.Text.Trim().Length > 0){
					if(IsValidEmail(mailTextBox.Text.Trim())){
						mailListBox.Items.Add(mailTextBox.Text);
						mailTextBox.Text = null;
					}
					else{
						// Javascript Erreur : Email non valide
						Response.Write("<script _siteLanguage=Javascript>");
						Response.Write("alert('"+_msg_err_invalid_email+"');");
						Response.Write("</script>");
					}
				}
				else{
					// Javascript Erreur : Le champs ne peut pas être vide
					Response.Write("<script _siteLanguage=Javascript>");
					Response.Write("alert('"+_msg_err_champ_notNull+"');");
					Response.Write("</script>");
				}
			}
			catch(System.Exception err){
			}
		}
		#endregion

		#region Supprimer un email
		/// <summary>
		/// Supprimer un email
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void deleteMailButton_Click(object sender, System.EventArgs e) {
			try{
				if(mailListBox.Items.Count > 0){
					mailListBox.Items.Remove(mailListBox.SelectedItem.Text);
				}
				else{
					// Javascript Erreur : La liste est vide
					Response.Write("<script _siteLanguage=Javascript>");
					Response.Write("alert('"+_msg_err_list_empty+"');");
					Response.Write("</script>");
				}
			}
			catch(System.Exception err){
			}
		}
		#endregion

		#region Valider
		/// <summary>
		/// Valider
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void validateButton_Click(object sender, System.EventArgs e) {
			try{
				if(mailListBox.Items.Count > 0){
					ArrayList mailList = new ArrayList();
					for(int i=0; i < mailListBox.Items.Count; i++) {
						mailList.Add(mailListBox.Items[i].Text);
					}
					Session.Add("Mails",mailList);
					Response.Redirect("DateSelection.aspx");
				}
				else{
					// Javascript Erreur : La liste est vide, vous devez préciser au moins 1 email
					Response.Write("<script _siteLanguage=Javascript>");
					Response.Write("alert('"+_msg_err_list_empty_enter_one_email+"');");
					Response.Write("</script>");
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new ErrorEventArgs(this, exc));
				}
			}
		}
		#endregion

		#endregion

		#region Méthode privée
		/// <summary>
		/// Fonction qui test si l'email est bien de type chaine@chaine
		/// </summary>
		/// <param name="stringToTest">Chaine à tester</param>
		/// <returns>True si la syntaxe est correcte, false sinon</returns>
		private static bool IsValidEmail(string stringToTest){
			Regex rule = new Regex(@"^([\w\-.]+)@([a-zA-Z0-9\-.]+)$");
			if (!rule.Match(stringToTest).Success){
				return(false);
			}
			return(true);
		}
		#endregion

	}
}
