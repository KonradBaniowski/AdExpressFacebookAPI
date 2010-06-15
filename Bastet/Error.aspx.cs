#region Informations
// Auteur: B. Masson
// Date de cr�ation: 18/11/2005
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
using Localization = Bastet.Localization;
using TNS.AdExpress.Bastet.Translation;
using TNS.AdExpress.Bastet;

namespace BastetWeb{
	/// <summary>
	/// Page d'erreur de Bastet
	/// </summary>
	public partial class Error : PrivateWebPage{

		#region Variables
		/// <summary>
		/// Message d'erreur
		/// </summary>
		public string _messageError = null;
		/// <summary>
		/// Page de destination
		/// </summary>
		public string _page = "";
		#endregion
        
		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			int errorId = -1;
			if(Page.Request.QueryString.Get("ErrorId")!=null){
				errorId = int.Parse(Page.Request.QueryString.Get("ErrorId"));
			}
			switch(errorId){
				case 1:
					// Acc�s refus�, vous devez vous authentifier
                    _messageError = GestionWeb.GetWebWord(35, _siteLanguage);
					_page = @"\Index.aspx";
					break;
				case 2:
					// Erreur : aucun email enregistr� en session
                    _messageError = GestionWeb.GetWebWord(36, _siteLanguage);
					_page = @"\MailSelection.aspx";
					break;
				case 3:
					// Erreur : aucune date enregistr�e en session
                    _messageError = GestionWeb.GetWebWord(37, _siteLanguage);
					_page = @"\DateSelection.aspx";
					break;
				default:
					// Une erreur est survenue,<br>impossible d'afficher la page demand�e
                    _messageError = GestionWeb.GetWebWord(38, _siteLanguage);
					_page = @"\Index.aspx";
					break;
			}
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
		}
		#endregion

		#endregion

	}
}
