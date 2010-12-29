#region Informations
// Auteur: B. Masson, G.Facon
// Date de création: 15/11/2005
// Date de modification: 17/11/2005
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using TNS.AdExpress.Bastet.BusinessFacade;
using TNS.AdExpress.Bastet.Common;
using TNSSources=TNS.FrameWork.DB.Common;
using IsisCommon=TNS.Isis.Right.Common;
using obout = obout_ASPTreeView_2_NET;
using Localization = Bastet.Localization;
using TNS.AdExpress.Bastet.Translation;

namespace BastetWeb{
	/// <summary>
	/// Page de gestion des logins clients
	/// </summary>
	public partial class LoginSelection : PrivateWebPage{

		#region Variables
		/// <summary>
		/// Résultat de la construction du TreeView
		/// </summary>
		public string result="";
		/// <summary>
		/// Code html utilisé durant la construction du TreeView
		/// </summary>
		public string html;
		/// <summary>
		/// Option de recherche sélectionné
		/// </summary>
		int selectedOptionSearch = 1;
		/// <summary>
		/// Liste des logins dans le input hidden
		/// </summary>
		public string _logingsHidden="";

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
		/// Options de recherche
		/// </summary>
		public string _label_search_option = string.Empty;
		/// <summary>
		/// Société / Login
		/// </summary>
		public string _label_company_login = string.Empty;
		/// <summary>
		/// Tout
		/// </summary>
		public string _label_all = string.Empty;
		/// <summary>
		/// Aucun résultat trouvé
		/// </summary>
		public string _label_no_result_find = string.Empty;
		/// <summary>
		/// Résultat de votre recherche
		/// </summary>
		public string _label_result_search = string.Empty;
		/// <summary>
		/// Sélectionner/désélectionner tous les logins
		/// </summary>
		public string _label_select_unselect_all_login = string.Empty;
		/// <summary>
		/// Ajouter
		/// </summary>
		public string _bt_add = string.Empty;
		/// <summary>
		/// Supprimer
		/// </summary>
		public string _bt_suppress = string.Empty;
		/// <summary>
		/// Le champs ne peut pas être vide
		/// </summary>
		public string _msg_err_champ_notNull = string.Empty;
		/// <summary>
		/// Vous devez sélectionner au moins un élément de la liste
		/// </summary>
		public string _msg_err_select_one_element = string.Empty;
		/// <summary>
		/// Vous ne pouvez pas sauvegarder plus de 1000 éléments
		/// </summary>
		public string _msg_err_dont_save_more_1000_element = string.Empty;
		#endregion
		
		#region Variables MMI
		/// <summary>
		/// Composant Obout TreeView
		/// </summary>
		obout_ASPTreeView_2_NET.Tree oTree = new obout_ASPTreeView_2_NET.Tree();
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
                if (Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.LOGIN] == null) throw (new SystemException("Aucun login en session"));
                if (Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.MAILS] == null) throw (new SystemException("Aucun email en session"));
                if (Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.DATE_BEGIN] == null) throw (new SystemException("Aucune date de début en session"));
                if (Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.DATE_END] == null) throw (new SystemException("Aucune date de fin en session"));

                HeaderWebControl1.LanguageId = _siteLanguage;
                HeaderWebControl1.Type_de_page = TNS.AdExpress.Bastet.WebControls.PageType.generic;
			
				#region Liste des logins du hidden "selectedLoginItem"
				if(Request.Form.GetValues("selectedLoginItem")!=null) 
					_logingsHidden = Request.Form.GetValues("selectedLoginItem")[0];
				#endregion

				#region Mise à jour de la listBox
				if(_logingsHidden.Length > 0){
					// Variables
					string[] loginsHidden = _logingsHidden.Split(',');
					string[] loginsTmp = null;
					string login = null;
					string loginId = null;
					// Initialisation et remplissage de la listBox
					loginListBox.Items.Clear();
					for(int i = 0 ; i < loginsHidden.Length - 1 ; i++){
						// Split le loginId % login
						loginsTmp = loginsHidden.GetValue(i).ToString().Split('%');
						loginId = loginsTmp.GetValue(0).ToString();
						login = loginsTmp.GetValue(1).ToString();				
						// Remplissage
						loginListBox.Items.Add(new ListItem(login, loginId));
					}
				}
				#endregion

				#region Option de recherche du Hidden "selectedOptionSearch"
				if(Request.Form.GetValues("selectedOptionSearch")!=null)
					selectedOptionSearch = int.Parse(Request.Form.GetValues("selectedOptionSearch")[0]);
				#endregion

				#region Types de client
				if(!IsPostBack){
                    TNSSources.IDataSource source = ((IsisCommon.Login)Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.LOGIN]).Source;
					DataSet ds = CustomerTypeBusinessFacade.GetCustomerType(source);
					CustomerTypeCheckBoxList.DataSource = ds;
					CustomerTypeCheckBoxList.DataValueField = ds.Tables[0].Columns[0].ToString();
					CustomerTypeCheckBoxList.DataTextField = ds.Tables[0].Columns[1].ToString();
					CustomerTypeCheckBoxList.DataBind();
				}
				#endregion

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
			InitializeComponent();
			InitializeComponentCustom();
			base.OnInit(e);
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

			// Boutons
            this.searchLoginButton.Text = GestionWeb.GetWebWord(4, _siteLanguage);
            this.validateButton.Text = GestionWeb.GetWebWord(5, _siteLanguage);
            this.validateButton2.Text = GestionWeb.GetWebWord(5, _siteLanguage);
            this.searchByTypeButton.Text = GestionWeb.GetWebWord(2, _siteLanguage);
            _bt_add = GestionWeb.GetWebWord(2, _siteLanguage);
            _bt_suppress = GestionWeb.GetWebWord(3, _siteLanguage);

			// Textes
            _label_search_option = GestionWeb.GetWebWord(19, _siteLanguage);
            _label_company_login = GestionWeb.GetWebWord(20, _siteLanguage);
            _label_all = GestionWeb.GetWebWord(21, _siteLanguage);
            _label_result_search = GestionWeb.GetWebWord(23, _siteLanguage);
            _label_select_unselect_all_login = GestionWeb.GetWebWord(24, _siteLanguage);

			// Messages
            _label_no_result_find = GestionWeb.GetWebWord(22, _siteLanguage);
            _msg_err_champ_notNull = GestionWeb.GetWebWord(27, _siteLanguage);
            _msg_err_select_one_element = GestionWeb.GetWebWord(33, _siteLanguage);
            _msg_err_dont_save_more_1000_element = GestionWeb.GetWebWord(34, _siteLanguage);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent(){    

		}
		#endregion

		#region Rechercher de logins
		/// <summary>
		/// Rechercher
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void searchLoginButton_Click(object sender, System.EventArgs e){
			try{
				if(searchLoginTextBox.Text.Trim().Length > 0){
					// Variables
					DataTable dt = null;
                    TNSSources.IDataSource source = ((IsisCommon.Login)Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.LOGIN]).Source;
				
					if(IsNumberSearch(searchLoginTextBox.Text)) 
						// Recherche par identifiant (X) ou par liste d'identifiants (X,X,X,...)
						dt = LoginBusinessFacade.GetLogins(source, searchLoginTextBox.Text.Trim(), false).Tables[0];
					else
						// Recherche par mot clef
						dt = LoginBusinessFacade.GetLogins(source, searchLoginTextBox.Text.Trim(), true).Tables[0];

					// Affichage du résultat
					if(dt.Rows.Count != 0) 
						displayTreeViewLabel.Text = ConstructTreeView(dt);
					else 
						// Aucun résultat trouvé
						displayTreeViewLabel.Text = "<font class=txtRouge12Bold>"+_label_no_result_find+"</font>";
				}
				else{
					// Javascript Erreur : Le champs ne peut pas être vide
					Response.Write("<script _siteLanguage=Javascript>");
					Response.Write("alert('"+_msg_err_champ_notNull+"');");
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

		#region Rechercher de logins par type de client
		/// <summary>
		/// Recherche les logins par type
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void searchByTypeButton_Click(object sender, System.EventArgs e) {
			try{
				// Variables
				DataTable dt = null;
                TNSSources.IDataSource source = ((IsisCommon.Login)Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.LOGIN]).Source;
				string customerTypeId = "";

				// Récupération des identifiants des types sélectionnés
				for(int i=0; i<CustomerTypeCheckBoxList.Items.Count; i++){
					if(CustomerTypeCheckBoxList.Items[i].Selected)
						customerTypeId += CustomerTypeCheckBoxList.Items[i].Value +",";
				}

				if(customerTypeId.Length > 0){
					// GetDatas
					dt = LoginBusinessFacade.GetLoginByCustomerType(source,customerTypeId.Substring(0, customerTypeId.Length-1)).Tables[0];

					// Ajout des logins dans la liste
					loginListBox.Items.Clear();
					for(int j=0; j<dt.Rows.Count; j++){
						loginListBox.Items.Add(dt.Rows[j][1].ToString());
						loginListBox.Items[j].Value = "chk_"+ dt.Rows[j][0].ToString();
						loginListBox.Items[j].Text = dt.Rows[j][1].ToString();

						_logingsHidden += loginListBox.Items[j].Value+",";
					}
				}
				else{
					// Javascript Erreur : Vous devez sélectionner au moins un élément de la liste
					Response.Write("<script _siteLanguage=Javascript>");
					Response.Write("alert('"+_msg_err_select_one_element+"');");
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

		#region Valider la demande
		/// <summary>
		/// Valider la demande
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void validateButton_Click(object sender, System.EventArgs e) {
			try{
				// Mise à null de la liste de login si l'option TOUT est coché
				if(selectedOptionSearch == 2) _logingsHidden = "";

				// Liste des logins
				string loginsIdList = GetLoginIdList(_logingsHidden);
				Int64 countLogins = loginsIdList.Split(',').GetLength(0);
				
				if(countLogins < 1000){
					// Sauvegarde de la demande
                    Parameters param = new Parameters(((IsisCommon.Login)Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.LOGIN]).Source, ((IsisCommon.Login)Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.LOGIN]).LoginId, (DateTime)Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.DATE_BEGIN], (DateTime)Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.DATE_END], loginsIdList, (ArrayList)Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.MAILS], _siteLanguage);
					param.Save();
				
					// Redirection pour effectuer une nouvelle demande
					Response.Redirect("MailSelection.aspx");
				}
				else{
					// Javascript Erreur : Vous ne pouvez pas sauvegarder plus de 1000 éléments
					Response.Write("<script _siteLanguage=Javascript>");
					Response.Write("alert('"+_msg_err_dont_save_more_1000_element+"');");
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

		#region Méthode privées
		/// <summary>
		/// Construction du TreeView
		/// </summary>
		/// <param name="dt">Données</param>
		/// <returns>Code HMLT du TreeView</returns>
		private string ConstructTreeView(DataTable dt){
					
			#region Variables
			Int64 companyId		= 0;
			Int64 oldCompanyId	= 0;
			Int64 loginId		= 0;
			Int64 oldLoginId	= 0;
			int activation		= -1;
			string fontColor	= null;
			string imgNode		= null;
			#endregion

			#region Construction du TreeView
			// Résultat de votre recherche
			oTree.AddRootNode("<b>"+_label_result_search+"</b>", "xpLens.gif");
				
			// Sélectionner/désélectionner tous les logins
			html = "<input type='checkbox' class='chk' onclick='ob_t2c(this)' >&nbsp;<font color=#FF0099>"+_label_select_unselect_all_login+"</font>";
			oTree.Add("root", "rootA", html, true, "square_blueS.gif", null);

			// Sociétés
			for(int i = 0 ; i < dt.Rows.Count; i++){
				companyId = (Int64)dt.Rows[i][0];
				if(oldCompanyId != companyId){
					oldCompanyId = companyId;
					html = "<input type='checkbox' class='chk' onclick='ob_t2c(this)' >&nbsp;<b>"+ dt.Rows[i][1].ToString() +" ("+ dt.Rows[i][0].ToString() +")</b>";
					oTree.Add("rootA", "a"+oldCompanyId, html, true, "company.gif", null);
				}
				// Logins
				for(int j = 0 ; j < dt.Columns.Count ; j++){
					loginId = (Int64)dt.Rows[i][2];
					if(oldLoginId != loginId){
						oldLoginId = loginId;
						
						// Image Actif / Non actif
						activation = int.Parse(dt.Rows[i][4].ToString());
						if(activation == 0){
							imgNode = "square_greenS.gif";
							fontColor = "#000000";
						}
						else{
							imgNode = "square_redS.gif";
							fontColor = "#FF0000";
						}

						html = "<input type='checkbox' class='chk' id='chk_"+ dt.Rows[i][2].ToString() +"' name='"+ dt.Rows[i][3].ToString() +"'>&nbsp;<font color="+fontColor+">"+dt.Rows[i][3].ToString() +" ("+ dt.Rows[i][2].ToString() +")</font>";
						oTree.Add("a"+oldCompanyId, "a"+oldCompanyId+"_"+j, html, true, imgNode, null);
					}
				}
			}
			#endregion
				
			#region Paramètre visuels du TreeView
			oTree.FolderIcons = "/TreeIcons/Icons";
			oTree.FolderStyle = "/TreeIcons/Styles/Classic";
            oTree.FolderScript = "/TreeIcons/Icons";
			oTree.Width = "300px";
			#endregion

			return (oTree.HTML());

		}

		/// <summary>
		/// Construit la liste des logins depuis les valeurs du hidden
		/// </summary>
		/// <param name="list">Liste des éléments du hidden</param>
		/// <returns>Liste formatée</returns>
		private static string GetLoginIdList(string list){
			string newList = "";
			if(list.Length > 0){
				string[] loginsHidden = list.Split(',');
				string[] loginsTmp = null;
				string loginId = null;
				for(int i = 0 ; i < loginsHidden.Length - 1 ; i++){
					// Split le loginId % login
					loginsTmp = loginsHidden.GetValue(i).ToString().Split('%');
					loginId = loginsTmp.GetValue(0).ToString().Substring(4);
					
					newList+= loginId +",";
				}
				newList = newList.Substring(0, newList.Length-1);
			}
			return (newList);
		}

		/// <summary>
		/// Fonction qui test si la chaine de caractère contient que des lettres
		/// </summary>
		/// <param name="stringToTest">Chaine à tester</param>
		/// <returns>True si la syntaxe est correcte, false sinon</returns>
		private static bool IsNumberSearch(string stringToTest){
			Regex rule = new Regex("^([0-9, ])+$");
			if (!rule.Match(stringToTest).Success){
				return(false);
			}
			return(true);
		}
		#endregion

	}
}
