#region Informations
// Auteur: D. V. Mussuma
// Date de création: 30/06/2005
// Date de modification:
//	01/08/2006 Modification FindNextUrl 
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
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Affiche les cibles AEPM à sélectionner
	/// </summary>
	public partial class AEPMTargetSelection : TNS.AdExpress.Web.UI.SelectionWebPage{

		#region variables MMI
		/// <summary>
		/// Contrôle titre du module
		/// </summary>
		/// <summary>
		/// Contrôle affichage menu en-tête
		/// </summary>
		/// <summary>
		/// Contrôle de validation 
		/// </summary>
		/// <summary>
		/// Contrôle d'affichage des Cibles AEPM de bases
		/// </summary>
		/// <summary>
		/// Contrôle d'affichage des Cibles AEPM supplémentaires
		/// </summary>
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Evènement lancé
		/// </summary>
		public int eventButton=-1;	
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public AEPMTargetSelection():base(){

			#region Ouverture la base de données
			// Ouverture de la base de données
			if(_webSession.CustomerLogin.Connection!=null){
				if(_webSession.CustomerLogin.Connection.State==System.Data.ConnectionState.Closed) _webSession.CustomerLogin.Connection.Open();
			}
			#endregion
		
		}
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){

			#region Textes et langage du site
			//Modification de la langue pour les Textes AdExpress
			TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_siteLanguage);

			// Initialisation de la liste des média
			ModuleTitleWebControl1.CustomerWebSession = _webSession;
			InformationWebControl1.Language = _webSession.SiteLanguage;
			validateButton.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
			validateButton.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
			#endregion

			#region Rappel des différentes sélections
//			ArrayList linkToShow=new ArrayList();			
//			if(_webSession.isDatesSelected())linkToShow.Add(4);
//			if(_webSession.isAdvertisersSelected())linkToShow.Add(2);
//			recallWebControl.LinkToShow=linkToShow;					
//			if(_webSession.LastReachedResultUrl.Length>0 && _webSession.isAdvertisersSelected() && _webSession.isDatesSelected())recallWebControl.CanGoToResult=true;
			#endregion

			#region évènemment
			// Connaître le boutton qui a été cliquer 
			eventButton=0;			

			// Boutton valider
			if(Request.Form.Get("__EVENTTARGET")=="validateButton" ){
				eventButton=1;
			}
			#endregion

			#region Url Suivante
//			_nextUrl=this.recallWebControl.NextUrl;
//			if(_nextUrl.Length==0)_nextUrl=_currentModule.FindNextUrl(Request.Url.AbsolutePath);
//			else validateButton_Click(this, null);
			#endregion

			#region Définition de la page d'aide
//			helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"AEPMTargetSelectionHelp.aspx";
			#endregion							
			
			#region Définition du contrôle targetSelectionWebControl		
			targetSelectionWebControl1.EventButton=	targetSelectionWebControl2.EventButton=eventButton;
			if(targetSelectionWebControl1.noneWave && targetSelectionWebControl2.noneWave)
				validateButton.Visible=false;	
			#endregion
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation de la page
		/// </summary>
		/// <param name="e">arguments</param>
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

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			//recallWebControl.CustomerWebSession=_webSession;

			targetSelectionWebControl1.CustomerWebSession=_webSession;
			targetSelectionWebControl1.Code=1658;
			targetSelectionWebControl1.IsThisTargetBase=true;

			targetSelectionWebControl2.CustomerWebSession=_webSession;
			targetSelectionWebControl2.Code=1659;
			targetSelectionWebControl2.IsThisTargetBase=false;

			MenuWebControl2.CustomerWebSession = _webSession;
				
			return tmp;
		}
		#endregion

		#region Bouton valider
		/// <summary>
		/// Sauvegarde de l'univers des cibles sélectionnées
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void validateButton_Click(object sender, System.EventArgs e) {		
			try{
				string targetSelection="";				
				foreach(ListItem currentItem in targetSelectionWebControl2.Items){
					if(currentItem.Selected)targetSelection+=currentItem.Value+",";
				}
				if(targetSelection.Length<1){
					//DBFunctions.closeDataBase(_webSession);
					Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(1788,_webSession.SiteLanguage)));
				}
				else{
					targetSelection.Substring(0,targetSelection.Length-1);					
					// Sauvegarde de la sélection dans la session
					//Si la sélection comporte des éléments, on la vide
					_webSession.SelectionUniversAEPMTarget.Nodes.Clear();
				    System.Windows.Forms.TreeNode tmpNode;
					//cible de base
					foreach(ListItem currentItem in targetSelectionWebControl1.Items){
						if(currentItem.Selected){
							tmpNode=new System.Windows.Forms.TreeNode(currentItem.Text);
							tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.aepmBaseTargetAccess,long.Parse(currentItem.Value),currentItem.Text);
							tmpNode.Checked=true;
							_webSession.SelectionUniversAEPMTarget.Nodes.Add(tmpNode);
						}
					}
					//cibles supplémentaires
					foreach(ListItem currentItem in targetSelectionWebControl2.Items){
						if(currentItem.Selected){
							tmpNode=new System.Windows.Forms.TreeNode(currentItem.Text);
							tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.aepmTargetAccess,long.Parse(currentItem.Value),currentItem.Text);
							tmpNode.Checked=true;
							_webSession.SelectionUniversAEPMTarget.Nodes.Add(tmpNode);
						}
					}		
					// On sauvegarde la session
					_webSession.Save();
					//Redirection vers la page suivante
					if(_nextUrl.Length>0){
						DBFunctions.closeDataBase(_webSession);
						Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
					}
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

		#region Implémentation méthodes abstraites
		/// <summary>
		/// Event launch to fire validation of the page
		/// </summary>
		/// <param name="sender">Sender Object</param>
		/// <param name="e">Event Arguments</param>
		protected override void ValidateSelection(object sender, System.EventArgs e){
			this.validateButton_Click(sender,e);
		}
		/// <summary>
		/// Retrieve next Url from the menu
		/// </summary>
		/// <returns>Next Url</returns>
		protected override string GetNextUrlFromMenu(){
			return(this.MenuWebControl2.NextUrl);
		}
		#endregion

	}
}
