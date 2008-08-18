#region Informations
// Auteur: A. Obermeyer
// Date de création: 
// Date de modification: 
//		30/12/2004 A. Obermeyer Intégration de WebPage
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
using DataAccess=TNS.AdExpress.Web.DataAccess.MyAdExpress;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Translation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using CstWeb = TNS.AdExpress.Constantes.Web;

namespace AdExpress.Private.MyAdExpress{
	/// <summary>
	/// Gestion de Mon AdExpress.
	/// </summary>
	public partial class PersonnalizeSession : TNS.AdExpress.Web.UI.PrivateWebPage{   
		
		#region MMI
		/// <summary>
		/// Commentaire Mes pdf
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText Adexpresstext8;
		#endregion

		#region Variables
		/// <summary>
		/// Liste des répertoires
		/// </summary>
		protected string listRepertories;
		/// <summary>
		/// Javascript pour l'affichage des TreeNodes
		/// </summary>
		protected string script;
		/// <summary>
		/// Liste des résultats à renommer
		/// </summary>
		public string listSesssionsToRename;
		#endregion
	
		#region Evènements
		
		#region Constructeur
		/// <summary>
		/// Constructeur 
		/// </summary>
		public PersonnalizeSession():base(){
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){			
			try{
				//Modification de la langue pour les Textes AdExpress
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[3].Controls,_webSession.SiteLanguage);

				//Liste des répertoires dans déplacer un univers
				TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI myAdexpress=new TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI(_webSession,TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI.type.mySession,500);
				listRepertories= myAdexpress.GetSelectionTableHtmlUI(4,"");
				//script=myAdexpress.Script;

				// Liste des univers dans renommer un univers
				listSesssionsToRename = myAdexpress.GetSelectionTableHtmlUI(5, "");

				HeaderWebControl1.ActiveMenu = CstWeb.MenuTraductions.MY_ADEXPRESS;

				#region Script
				//Script
				if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "showHideContent", TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				}

				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowHideContent1", TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
				}

                if(!Page.ClientScript.IsClientScriptBlockRegistered("script")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "script", myAdexpress.Script);
                }
				#endregion
			
				#region Chargement de la liste des répertoires
				if (!IsPostBack){
					DataSet ds= TNS.AdExpress.Web.DataAccess.MyAdExpress.MySessionsDataAccess.GetDirectories(_webSession);		
		
					directoryDropDownList.DataSource=ds.Tables[0];
					directoryDropDownList.DataTextField=ds.Tables[0].Columns[1].ToString();
					directoryDropDownList.DataValueField=ds.Tables[0].Columns[0].ToString();
					directoryDropDownList.DataBind();

					renameDirectoryDropDownList.DataSource=ds.Tables[0];
					renameDirectoryDropDownList.DataTextField=ds.Tables[0].Columns[1].ToString();
					renameDirectoryDropDownList.DataValueField=ds.Tables[0].Columns[0].ToString();
					renameDirectoryDropDownList.DataBind();

					moveDirectoryDropDownList.DataSource=ds.Tables[0];
					moveDirectoryDropDownList.DataTextField=ds.Tables[0].Columns[1].ToString();
					moveDirectoryDropDownList.DataValueField=ds.Tables[0].Columns[0].ToString();
					moveDirectoryDropDownList.DataBind();
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

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
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
           
            this.Unload += new System.EventHandler(this.Page_UnLoad);
		}
		#endregion

		#region Fonction pour rafraichir la liste des répertoires
		/// <summary>
		/// Rafraichi la liste des répertoires
		/// </summary>
		private void refreshDirectories(){
			
			try{
				// Rafraichi la liste
				DataSet ds= TNS.AdExpress.Web.DataAccess.MyAdExpress.MySessionsDataAccess.GetDirectories(_webSession);	
	
				directoryDropDownList.DataSource=ds.Tables[0];
				directoryDropDownList.DataTextField=ds.Tables[0].Columns[1].ToString();
				directoryDropDownList.DataValueField=ds.Tables[0].Columns[0].ToString();
				directoryDropDownList.DataBind();

				renameDirectoryDropDownList.DataSource=ds.Tables[0];
				renameDirectoryDropDownList.DataTextField=ds.Tables[0].Columns[1].ToString();
				renameDirectoryDropDownList.DataValueField=ds.Tables[0].Columns[0].ToString();
				renameDirectoryDropDownList.DataBind();

				moveDirectoryDropDownList.DataSource=ds.Tables[0];
				moveDirectoryDropDownList.DataTextField=ds.Tables[0].Columns[1].ToString();
				moveDirectoryDropDownList.DataValueField=ds.Tables[0].Columns[0].ToString();
				moveDirectoryDropDownList.DataBind();

				//Liste des répertoires dans déplacer une session
				TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI myAdexpress=new TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI(_webSession,TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI.type.mySession,500);				
				listSesssionsToRename = myAdexpress.GetSelectionTableHtmlUI(5, ""); 
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Fonction pour rafraichir la liste des répertoires dans "déplacer un résultat"
		/// <summary>
		/// Fonction pour rafraichir la liste des répertoires dans "déplacer un résultat"
		/// </summary>
		private void refreshListDirectories(){ 
			//Liste des répertoires dans déplacer un univers
			TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI myAdexpress=new TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI(_webSession,TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI.type.mySession,500);
			listRepertories= myAdexpress.GetSelectionTableHtmlUI(4,"");			
		}
		#endregion

		#region bouton Ouvrir
		/// <summary>
		/// Gestion du bouton Ouvrir
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void ImageButtonRollOverWebControl1_Click(object sender, System.EventArgs e) {
			try{
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

		#region Bouton ouvir Pdf
		/// <summary>
		/// Gestion du bouton ouvir Pdf
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void pdfOpenImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			try{
                _webSession.Source.Close();
				Response.Redirect("/Private/MyAdexpress/PdfFiles.aspx?idSession="+_webSession.IdSession+"");
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}

		#endregion

		#region Bouton créer un répertoire
		/// <summary>
		/// Gestion du bouton créer un répertoire
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void createRepertoryImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
		
			try{

				string directoryName = this.createRepertoryTextBox.Text;
				directoryName = CheckedText.CheckedAccentText(directoryName);
				if (directoryName.Length!=0 && directoryName.Length<TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT){

					if(!DataAccess.MySessionsDataAccess.IsDirectoryExist(_webSession,directoryName)){
						if(DataAccess.MySessionsDataAccess.CreateDirectory(directoryName,_webSession)){
							// Validation : confirmation de la création du répertoire
							Response.Write("<script language=javascript>");
							Response.Write("	alert(\""+GestionWeb.GetWebWord(835,_webSession.SiteLanguage)+"\");");					
							Response.Write("</script>");
						
							this.createRepertoryTextBox.Text="";
						
							// Rafraichi la liste des répertoires
							this.refreshDirectories();
							this.refreshListDirectories();
						
						}
						else{
							// Erreur : Echec de la création du répertoire
							Response.Write("<script language=javascript>");
							Response.Write("	alert(\""+GestionWeb.GetWebWord(836,_webSession.SiteLanguage)+"\");");
							Response.Write("</script>");
						}
					}
					else{
						//Erreur : Répertoire déjà existant
						Response.Write("<script language=javascript>");
						Response.Write("	alert(\""+GestionWeb.GetWebWord(834,_webSession.SiteLanguage)+"\");");
						Response.Write("</script>");
					}

				}
				else if(directoryName.Length==0){
					// Erreur : Le champs est vide
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(837,_webSession.SiteLanguage)+"\");");
					Response.Write("</script>");
				}
				else{
					// Erreur : suppérieur à 50 caractères
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(823,_webSession.SiteLanguage)+"\");");
					Response.Write("</script>");
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Bouton supression d'un répertoire
		/// <summary>
		/// Gestion du bouton supprimer un répertoire
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void deleteImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			
			try{
				if(directoryDropDownList.Items.Count!=0){
					if(DataAccess.MySessionsDataAccess.GetDirectories(_webSession).Tables[0].Rows.Count>1){
						if(!DataAccess.MySessionsDataAccess.IsSessionsInDirectoryExist(_webSession,Int64.Parse(directoryDropDownList.SelectedItem.Value))){
							DataAccess.MySessionsDataAccess.DropDirectory(Int64.Parse(directoryDropDownList.SelectedItem.Value),_webSession);

							// Rafraichi la liste des répertoires
							this.refreshDirectories();
							this.refreshListDirectories();

						}
						else{
							// Erreur : Impossible de supprimer le répertoire. Le répertoire n'est pas vide.
							Response.Write("<script language=javascript>");
							Response.Write("	alert(\""+GestionWeb.GetWebWord(838,_webSession.SiteLanguage)+"\");");
							Response.Write("</script>");
						}
					}
					else{
						//Erreur : Impossible de supprimer votre unique répertoire.
						Response.Write("<script language=javascript>");
						Response.Write("	alert(\""+GestionWeb.GetWebWord(840,_webSession.SiteLanguage)+"\");");
						Response.Write("</script>");
					}
				}
				else{
					//Erreur : aucun répertoire
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(839,_webSession.SiteLanguage)+"\");");
					Response.Write("</script>");
				}				
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion	

		#region Bouton renommer un dossier
		/// <summary>
		/// Gestion du bouton renommer un dossier
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void renameImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			
			try{
				if(renameDirectoryDropDownList.Items.Count!=0){
					string newDirectoryName = renameDirectoryTextBox.Text;
					newDirectoryName = CheckedText.CheckedAccentText(newDirectoryName);
				
					if (newDirectoryName.Length!=0 && newDirectoryName.Length<TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT){

						if(!DataAccess.MySessionsDataAccess.IsDirectoryExist(_webSession,newDirectoryName)){
							DataAccess.MySessionsDataAccess.RenameDirectory(newDirectoryName,Int64.Parse(renameDirectoryDropDownList.SelectedItem.Value),_webSession);

							// Vide le champs de saisie
							renameDirectoryTextBox.Text="";

							// Rafraichi la liste des répertoires
							this.refreshDirectories();
							this.refreshListDirectories();

						}
						else{
							//Erreur : Répertoire déjà existant
							Response.Write("<script language=javascript>");
							Response.Write("	alert(\""+GestionWeb.GetWebWord(834,_webSession.SiteLanguage)+"\");");
							Response.Write("</script>");
						}
					}
					else if(newDirectoryName.Length==0){
						// Erreur : Le champs est vide
						Response.Write("<script language=javascript>");
						Response.Write("	alert(\""+GestionWeb.GetWebWord(837,_webSession.SiteLanguage)+"\");");
						Response.Write("</script>");
					}
					else{
						// Erreur : suppérieur à 50 caractères
						Response.Write("<script language=javascript>");
						Response.Write("	alert(\""+GestionWeb.GetWebWord(823,_webSession.SiteLanguage)+"\");");
						Response.Write("</script>");
					}
				}
				else{
					//Erreur : aucun répertoire
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(839,_webSession.SiteLanguage)+"\");");
					Response.Write("</script>");
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Bouton Déplacer une session
		/// <summary>
		/// Gestion du bouton déplacer
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void moveImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
		
			try{
				if(moveDirectoryDropDownList.Items.Count!=0){
					string[] tabParent=null;
					Int64 idOldRepertory=0;
					Int64 idSession=0;
		
					foreach (string currentKey in Request.Form.AllKeys){
						tabParent=currentKey.Split('_');
						if(tabParent[0]=="CKB") {
							idOldRepertory=Int64.Parse(tabParent[2]);		
							idSession=Int64.Parse(tabParent[1]);		
						}
					}
			
					if (idOldRepertory!=0){
						DataAccess.MySessionsDataAccess.MoveSession(idOldRepertory,Int64.Parse(moveDirectoryDropDownList.SelectedItem.Value),idSession,_webSession);
						this.refreshDirectories();
						this.refreshListDirectories();
					}
					else if (idSession==0){
						//Erreur : Aucune requête n'a été sélectionnée
						Response.Write("<script language=javascript>");
						Response.Write("	alert(\""+GestionWeb.GetWebWord(831,_webSession.SiteLanguage)+"\");");
						Response.Write("</script>");
					}
				}
				else{
					//Erreur : aucun répertoire
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(839,_webSession.SiteLanguage)+"\");");
					Response.Write("</script>");
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}				
		}
		#endregion

		#region Bouton personnaliser Mes Univers
		/// <summary>
		/// Gestion du bouton Personnaliser de Mes Univers
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
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

		#endregion

		#region Bouton renommer une session
		/// <summary>
		/// Renomme une session
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void renameSessionsImagebutton_Click(object sender, EventArgs e) {

			string[] tabParent = null;
			Int64 idSession = 0;

			foreach (string currentKey in Request.Form.AllKeys) {
				tabParent = currentKey.Split('_');
				if (tabParent[0] == "CKB1") {
					idSession = Int64.Parse(tabParent[1]);
				}
			}		
			string newSessionName = CheckedText.CheckedAccentText(renameSessionsTextBox.Text);

			if (idSession != 0) {

				if (newSessionName.Length > 0 && newSessionName.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT) {
					if (!DataAccess.MySessionsDataAccess.IsSessionExist(_webSession, newSessionName)) {
						DataAccess.MySessionsDataAccess.RenameSession(newSessionName, idSession, _webSession);
						this.refreshDirectories();
						this.refreshListDirectories();
					}
					else {
						// Erreur : une requête déjà existante porte le même nom
						Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(2260, _webSession.SiteLanguage)));
					}
				}
				else if (newSessionName.Length == 0) {
					// Erreur : Le champs est vide
					Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(837, _webSession.SiteLanguage)));					
				}
				else {
					// Erreur : suppérieur à 50 caractères
					Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(823, _webSession.SiteLanguage)));
				}
			}
			else {
				// Erreur : aucun élément de sélectionné
				Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(831, _webSession.SiteLanguage)));
			}


		}
		#endregion

	}
}
