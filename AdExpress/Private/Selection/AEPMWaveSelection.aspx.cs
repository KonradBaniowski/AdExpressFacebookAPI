#region Informations
// Auteur: D. V. Mussuma
// Date de création: 27/06/2005
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
using TNS.AdExpress.Web.Core.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
namespace AdExpress.Private.Selection
{
	/// <summary>
	/// Affiche les vagues AEPM à sélectionner
	/// </summary>
	public partial class AEPMWaveSelection : TNS.AdExpress.Web.UI.WebPage{	
		
		#region variables MMI
		/// <summary>
		/// Contrôle titre du module
		/// </summary>
		/// <summary>
		/// Contrôlede rappel
		/// </summary>
		/// <summary>
		/// Contrôle de la page d'aide
		/// </summary>
		/// <summary>
		/// Contrôle affichage menu en-tête
		/// </summary>
		/// <summary>
		/// Contrôle titre
		/// </summary>
		/// <summary>
		/// Contrôle d'affichage des vagues AEPM
		/// </summary>
		/// <summary>
		/// Contrôle de validation 
		/// </summary>
		
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public AEPMWaveSelection():base(){
			#region Ouverture la base de données
			// Ouverture de la base de données
			if(_webSession.CustomerLogin.Connection!=null){
				if(_webSession.CustomerLogin.Connection.State==System.Data.ConnectionState.Closed) _webSession.CustomerLogin.Connection.Open();
			}
			#endregion						
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">objet qui lance l'évènement</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
				
			#region Textes et langage du site
			//Modification de la langue pour les Textes AdExpress
			TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_siteLanguage);

			// Initialisation de la liste des média
			ModuleTitleWebControl1.CustomerWebSession = _webSession;
			validateButton.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
			validateButton.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
			#endregion

			#region Rappel des différentes sélections
			ArrayList linkToShow=new ArrayList();			
			if(_webSession.isDatesSelected())linkToShow.Add(5);
			recallWebControl.LinkToShow=linkToShow;				
			if(_webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TENDACES && _webSession.LastReachedResultUrl.Length>0){
				recallWebControl.CanGoToResult=true;
			}
			#endregion

			#region Url Suivante
			_nextUrl=this.recallWebControl.NextUrl;
			if(_nextUrl.Length==0)_nextUrl=_currentModule.FindNextUrl(Request.Url.AbsolutePath);
			else validateButton_Click(this, null);
			#endregion
					
			#region définition de la page d'aide
			helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"OneVehicleSelectionHelp.aspx";
			#endregion	
			
			if(waveSelectionWebControl1.noneWave)
				validateButton.Visible=false;
		}	

		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			recallWebControl.CustomerWebSession=_webSession;	
			waveSelectionWebControl1.CustomerWebSession=_webSession;
			waveSelectionWebControl1.CheckMostRecentWave=true;				
			return tmp;

		}
		#endregion
	
		#region Bouton valider
		/// <summary>
		/// Sauvegarde de l'univers de la vague sélectionné
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void validateButton_Click(object sender, System.EventArgs e) {
			try{
				string waveSelection="";
				// On recherche les éléments sélectionnés
				foreach(ListItem currentItem in waveSelectionWebControl1.Items){
					if(currentItem.Selected)waveSelection+=currentItem.Value+",";
				}
				if(waveSelection.Length<1){
					//DBFunctions.closeDataBase(_webSession);
					Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(1652,_webSession.SiteLanguage)));
				}
				else{
					waveSelection.Substring(0,waveSelection.Length-1);
					// Sauvegarde de la sélection dans la session
					//Si la sélection comporte des éléments, on la vide
					_webSession.SelectionUniversAEPMWave.Nodes.Clear();
					System.Windows.Forms.TreeNode tmpNode;
					foreach(ListItem currentItem in waveSelectionWebControl1.Items){
						if(currentItem.Selected){
							tmpNode=new System.Windows.Forms.TreeNode(currentItem.Text);
							tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.aepmWaveAccess,long.Parse(currentItem.Value),currentItem.Text);							
							_webSession.SelectionUniversAEPMWave.Nodes.Add(tmpNode);
						}
					}					
					// On sauvegarde la session
					_webSession.Save();
					//Redirection vers la page suivante
					if(_nextUrl.Length>0){
						DBFunctions.closeDataBase(_webSession);
						HttpContext.Current.Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
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
	}
}
