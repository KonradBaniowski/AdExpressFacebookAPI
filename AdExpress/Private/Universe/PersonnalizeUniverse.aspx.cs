#region Informations
// Auteur: 
// Date de création: 
// Date de modification: 
//		30/12/2004 A. Obermeyer Intégration de WebPage
//		02/04/2007 Y.R'kaina Demande de confirmation de suppression
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
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using CstWeb = TNS.AdExpress.Constantes.Web;
using DataAccess=TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Translation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;

namespace AdExpress.Private.Universe{
	/// <summary>
	/// Personnaliser les Univers
	/// </summary>
	public partial class PersonnalizeUniverse : TNS.AdExpress.Web.UI.PrivateWebPage{
		
		#region Variables MMI
		/// <summary>
		/// Création d'un univers
		/// </summary>
		/// <summary>
		/// liste des groupes d'univers
		/// </summary>
		/// <summary>
		/// liste des groupes d'univers dans renommer
		/// </summary>
		/// <summary>
		/// Renommer un groupe d'univers
		/// </summary>
		/// <summary>
		/// Liste des groupes d'univers dans déplacer
		/// </summary>
		/// <summary>
		/// Mes Univers
		/// </summary>
		/// <summary>
		/// Commentaire Mes Univers
		/// </summary>
		/// <summary>
		/// En tête
		/// </summary>
		/// <summary>
		/// Titre : Création d'un groupe d'univers
		/// </summary>
		/// <summary>
		/// Nommer votre groupe d'univers
		/// </summary>
		/// <summary>
		/// Bouton création d'un groupe d'univers
		/// </summary>
		/// <summary>
		/// Titre : Supprimer un univers
		/// </summary>
		/// <summary>
		/// Bouton ouvrir pdf
		/// </summary>
		/// <summary>
		/// Commentaire Mes pdf
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText Adexpresstext8;
		/// <summary>
		/// Sélectionner le groupe d'univers à supprimer
		/// </summary>
		/// <summary>
		/// Bouton Supprimer un groupe d'Univers
		/// </summary>
		/// <summary>
		/// Titre : Renommer un Groupe d'univers
		/// </summary>
		/// <summary>
		/// Sélectionner un groupe d'univers
		/// </summary>
		/// <summary>
		/// Renommer le groupe d'univers sélectionné
		/// </summary>
		/// <summary>
		/// Bouton renommer un groupe d'univers
		/// </summary>
		/// <summary>
		/// Titre : déplacer un univers
		/// </summary>
		/// <summary>
		/// Sélectionner l'univers à déplacer
		/// </summary>
		/// <summary>
		/// Déplacer l'univers sélectionné
		/// </summary>
		/// <summary>
		/// Bouton déplacer un univers
		/// </summary>
		/// <summary>
		/// Bouton
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl ImageButtonRollOverWebControl1;
		/// <summary>
		/// Mon AdExpress
		/// </summary>
		/// <summary>
		/// Commentaire Mon AdExpress
		/// </summary>
		/// <summary>
		/// Bouton Ouvrir
		/// </summary>
		/// <summary>
		/// Bouton Personnaliser
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText15;
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Supprimer un univers
		/// </summary>
		/// <summary>
		/// Sélectionner l'univers à supprimer
		/// </summary>
		/// <summary>
		/// Bouton supprimer 
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Bouton renommer
		/// </summary>
		/// <summary>
		/// Text Box
		/// </summary>
		#endregion		
		
		#region Variables
		/// <summary>
		/// Script
		/// </summary>
		protected string script;		
		/// <summary>
		/// Liste des groupes d'univers
		/// </summary>
		protected string listRepertories;		
		/// <summary>
		/// Liste des univers
		/// </summary>
		public string listUniversToDelete;		
		/// <summary>
		/// Liste des univers
		/// </summary>
		public string listDetailGroupUnivers;
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Liste des univers dans univers à renommer
		/// </summary>
		public string listUniversToRename;
		#endregion
	
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public PersonnalizeUniverse():base(){		
		}
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
						
				//Modification de la langue pour les Textes AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[3].Controls,_webSession.SiteLanguage);

				//Liste des répertoires dans déplacer un univers
				TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI myAdexpress=new TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI(_webSession,TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI.type.universe,500);
				listRepertories= myAdexpress.GetSelectionTableHtmlUI(4,"");
				//script=myAdexpress.Script;
			
				//liste des univers dans supprimer les univers
				listUniversToDelete=myAdexpress.GetSelectionTableHtmlUI(6,"");

				// Liste des univers dans détail
				listDetailGroupUnivers=myAdexpress.GetSelectionTableHtmlUI(5,"");

				// Liste des univers dans renommer un univers
				listUniversToRename=myAdexpress.GetSelectionTableHtmlUI(7,"");

				// En Tête			
				HeaderWebControl1.ActiveMenu = CstWeb.MenuTraductions.MY_ADEXPRESS;

				#region Script
				//Script
				if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent",TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				}
			
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent1",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
				}
				#endregion

				#region Rollover des boutons
				//personalizeImagebuttonrolloverwebcontrol.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/personnaliser_up.gif";
				//personalizeImagebuttonrolloverwebcontrol.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/personnaliser_down.gif";

				//openImageButtonRollOverWebControl.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/ouvrir_up.gif";
				//openImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/ouvrir_down.gif";

				//createRepertoryImageButtonRollOverWebControl.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_up.gif";	
				//createRepertoryImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_down.gif";

				//deleteImageButtonRollOverWebControl.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_up.gif";
				//deleteImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_down.gif";
				deleteImageButtonRollOverWebControl.Attributes.Add("onclick", "javascript: return confirm('Etes vous sûr de vouloir supprimer ce groupe d\\'univers ?');");

				//renameImageButtonRollOverWebControl.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_up.gif";
				//renameImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_down.gif";
	
				//renameUniverseImagebutton.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_up.gif";
				//renameUniverseImagebutton.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_down.gif";
			
				//moveImageButtonRollOverWebControl.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_up.gif";
				//moveImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_down.gif";
			
				//deleteUniversImageButtonRollOverWebControl.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/supprimer_up.gif";
				//deleteUniversImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/supprimer_down.gif";
				deleteUniversImageButtonRollOverWebControl.Attributes.Add("onclick", "javascript: return confirm('Etes vous sûr de vouloir supprimer cet univers ?');");
			
				//DetailUniversbuttonrolloverwebcontrol.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/detail_up.gif";
				//DetailUniversbuttonrolloverwebcontrol.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/detail_down.gif";
				
				//pdfOpenImageButtonRollOverWebControl.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/ouvrir_up.gif";
				//pdfOpenImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/ouvrir_down.gif";
				#endregion

				#region Chargement de la liste des répertoires
				if (!IsPostBack){
					DataSet ds= TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetGroupUniverses(_webSession);		
		
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

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){			
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
				DataSet ds= TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetGroupUniverses(_webSession);		
	
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

				//Liste des répertoires dans déplacer un univers
				TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI myAdexpress=new TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI(_webSession,TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI.type.universe,500);
				listRepertories= myAdexpress.GetSelectionTableHtmlUI(4,"");
				listUniversToDelete=myAdexpress.GetSelectionTableHtmlUI(6,"");
				listDetailGroupUnivers=myAdexpress.GetSelectionTableHtmlUI(5,"");
				listUniversToRename=myAdexpress.GetSelectionTableHtmlUI(7,""); 
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}

		}
		#endregion

		#region Bouton ouvrir
		/// <summary>
		/// Gestion du bouton ouvrir
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void openImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
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

		#region Bouton Personnaliser
		/// <summary>
		/// Gestion du bouton personnaliser
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
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

		#region Bouton créer un groupe d'univers
		/// <summary>
		/// Gestion du bouton créer
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void createRepertoryImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
		
			try{
				string directoryName = this.createRepertoryTextBox.Text;
				directoryName = CheckedText.CheckedAccentText(directoryName);
				if (directoryName.Length!=0 && directoryName.Length<TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT){

					if(!DataAccess.UniversListDataAccess.IsGroupUniverseExist(_webSession,directoryName)){
						if(DataAccess.UniversListDataAccess.CreateGroupUniverse(directoryName,_webSession)){
							// Validation : confirmation de la création du groupe d'univers
							Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(934,_webSession.SiteLanguage)));
						
							this.createRepertoryTextBox.Text="";
						
							// Rafraichi la liste des répertoires
							this.refreshDirectories();
						
						}
						else{
							// Erreur : Echec de la création du groupe d'univers
							Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(933,_webSession.SiteLanguage)));
						}
					}
					else{
						//Erreur : groupe d'univers déjà existant
						Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(928,_webSession.SiteLanguage)));
					}

				}
				else if(directoryName.Length==0){
					// Erreur : Le champs est vide
					Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(837,_webSession.SiteLanguage)));
				}
				else{
					// Erreur : suppérieur à 50 caractères
					Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(823,_webSession.SiteLanguage)));
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion
		
		#region Bouton Suppimer un groupe d'univers
		/// <summary>
		/// Gestion  du bouton supprimer 
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void deleteImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			
			try{

				if(directoryDropDownList.Items.Count!=0){
					if(DataAccess.UniversListDataAccess.GetGroupUniverses(_webSession).Tables[0].Rows.Count>1){
						if(!DataAccess.UniversListDataAccess.IsUniversInGroupUniverseExist(_webSession,Int64.Parse(directoryDropDownList.SelectedItem.Value))){
							DataAccess.UniversListDataAccess.DropGroupUniverse(Int64.Parse(directoryDropDownList.SelectedItem.Value),_webSession);

							// Rafraichi la liste des répertoires
							this.refreshDirectories();

						}
						else{
							// Erreur : Impossible de supprimer le groupe d'univers. Le groupe d'univers n'est pas vide.
							Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(931,_webSession.SiteLanguage)));
						}
					}
					else{
						//Erreur : Impossible de supprimer votre unique groupe d'univers.
						Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(929,_webSession.SiteLanguage)));
					}
				}
				else{
					//Erreur : aucun groupe d'univers
					Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(927,_webSession.SiteLanguage)));
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Bouton renommer un groupe d'univers
		/// <summary>
		/// Gestion du bouton renommer
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void renameImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			
			try{
				if(renameDirectoryDropDownList.Items.Count!=0){
					string newDirectoryName = renameDirectoryTextBox.Text;
					newDirectoryName = CheckedText.CheckedAccentText(newDirectoryName);
				
					if (newDirectoryName.Length!=0 && newDirectoryName.Length<TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT){

						if(!DataAccess.UniversListDataAccess.IsGroupUniverseExist(_webSession,newDirectoryName)){
							DataAccess.UniversListDataAccess.RenameGroupUniverse(newDirectoryName,Int64.Parse(renameDirectoryDropDownList.SelectedItem.Value),_webSession);
							// Vide le champs de saisie
							renameDirectoryTextBox.Text="";
							// Rafraichi la liste des répertoires
							this.refreshDirectories();
						}
						else{
							//Erreur : groupe d'univers déjà existant
							Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(928,_webSession.SiteLanguage)));
						}
					}
					else if(newDirectoryName.Length==0){
						// Erreur : Le champs est vide
						Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(837,_webSession.SiteLanguage)));
					}
					else{
						// Erreur : suppérieur à 50 caractères
						Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(823,_webSession.SiteLanguage)));
					}
				}
				else{
					//Erreur : aucun groupe d'univers
					Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(927,_webSession.SiteLanguage)));
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Bouton Déplacer un univers
		/// <summary>
		/// Gestion du bouton déplacer
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
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
						DataAccess.UniversListDataAccess.MoveUniverse(idOldRepertory,Int64.Parse(moveDirectoryDropDownList.SelectedItem.Value),idSession,_webSession);
						this.refreshDirectories();
					}
					else if (idSession==0){
						//Erreur : Aucun univers n'a été sélectionné
						Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(926,_webSession.SiteLanguage)));
					}
				}
				else{
					//Erreur : aucun groupe d'univers
					Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(927,_webSession.SiteLanguage)));
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Suppression d'un univers
		/// <summary>
		/// Suppression d'un univers
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void deleteUniversImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			
			try{
			
				Int64 idUnivers=0;
				string [] tabParent;
				foreach (string currentKey in Request.Form.AllKeys){
					tabParent=currentKey.Split('_');
					if(tabParent[0]=="CKB1") {
						idUnivers=Int64.Parse(tabParent[1]);		
					}
				}
				if (idUnivers!=0){
				
					if (TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.DropUniverse(idUnivers,_webSession)){
						// Validation : confirmation de suppression de l'univers		
						Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(937,_webSession.SiteLanguage)));
						// Actualise la page
						this.OnLoad(null);
					}
					else{
						// Erreur : la suppression de la requête a échouée
						Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(830,_webSession.SiteLanguage)));
					}
				}
				else{
					// Erreur : veuillez sélectionner une requête
					Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(831,_webSession.SiteLanguage)));
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Bouton Détail
		/// <summary>
		/// Affiche une popup avec le détail de l'univers sélectionné
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void DetailUniversbuttonrolloverwebcontrol_Click(object sender, System.EventArgs e) {
			
			try{
				string[] tabParent=null;
				Int64 idUniverse=0;
				
		
				string idSession=_webSession.IdSession;

				foreach (string currentKey in Request.Form.AllKeys){
					tabParent=currentKey.Split('_');
					if(tabParent[0]=="CKB2") {
						idUniverse=Int64.Parse(tabParent[1]);	
					}
				}
				if (idUniverse!=0){			

					Response.Write("<script language=javascript>");
					Response.Write("	window.open('/Private/Universe/UniverseDetailPopUp.aspx?idUniverse="+idUniverse+"&idSession="+_webSession.IdSession+"','','width=660,height=700,toolbar=no,scrollbars=yes,resizable=no');");
					Response.Write("</script>");
				
				
				}else{
					// Erreur : veuillez sélectionner une requête
					Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(831,_webSession.SiteLanguage)));
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Bouton Renommer un univers
		/// <summary>
		/// Renomme un univers
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void renameUniverseImagebutton_Click(object sender, System.EventArgs e) {
			
			try{
			
				Int64 idUnivers=0;
				string [] tabParent;
				foreach (string currentKey in Request.Form.AllKeys){
					tabParent=currentKey.Split('_');
					if(tabParent[0]=="CKB3") {
						idUnivers=Int64.Parse(tabParent[1]);		
					}
				}
				string newUniverseName =renameUniverseTextBox.Text;
				newUniverseName = CheckedText.CheckedAccentText(newUniverseName);
				
				if(idUnivers!=0){
					if (newUniverseName.Length!=0 && newUniverseName.Length<TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT){
				
						if(!DataAccess.UniversListDataAccess.IsUniverseExist(_webSession,newUniverseName)){
							DataAccess.UniversListDataAccess.RenameUniverse(newUniverseName,idUnivers,_webSession);					
							renameUniverseTextBox.Text="";
							// Rafraichi la liste des répertoires
							this.refreshDirectories();
						}
						else{
							//Erreur : L'univers existe déjà
							Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(1101,_webSession.SiteLanguage)));
						}			
					}
					else if(newUniverseName.Length==0){
						// Erreur : Le champs est vide
						Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(837,_webSession.SiteLanguage)));
					}
					else{
						// Erreur : suppérieur à 50 caractères
						Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(823,_webSession.SiteLanguage)));
					}

				}else{
					// Erreur : aucun élément de sélectionné
					Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(926,_webSession.SiteLanguage)));
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#endregion

	}
}
