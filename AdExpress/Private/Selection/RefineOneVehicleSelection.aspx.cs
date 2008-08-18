#region Informations
// Auteur: D. Mussuma
// Date de cr�ation: 12/12/2006
// Date de modification:
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Windows.Forms;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;

namespace AdExpress.Private.Selection
{
	/// <summary>
	///  Page de S�lection affiner des Vehicles
	/// </summary>
	public partial class RefineOneVehicleSelection : TNS.AdExpress.Web.UI.SelectionWebPage{
	
		#region Variables MMI
		/// <summary>
		/// Texte
		/// </summary>
		//protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText1;
		/// <summary>
		/// Ent�te de la page
		/// </summary>
		/// <summary>
		/// Bouton validation
		/// </summary>
		/// <summary>
		/// Titre de la page
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Contr�le Informaton 
		/// </summary>
		/// <summary>
		/// S�lection du vehicle
		/// </summary>
		
		#endregion

		#region Variables
		private string _absoluteUrlPath="";
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public RefineOneVehicleSelection():base(){
	}
		#endregion

		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress
                //for (int i = 0; i < this.Controls.Count; i++) {
                //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                //}

				// Initialisation de la liste des m�dia
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
                titreAdexpresstext.Language = _webSession.SiteLanguage;
                HeaderWebControl1.Language = _webSession.SiteLanguage;

				//validateButton.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				//validateButton.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				#endregion			
			
				#region Remise � z�ro de CompetitorUniversMedia
				_webSession.CompetitorUniversMedia.Clear();					
				#endregion

				#region initialisation de r�f�rence univers media
				_webSession.ReferenceUniversMedia=new System.Windows.Forms.TreeNode("media");
				_webSession.Save();
				#endregion

				_absoluteUrlPath = Page.Request.Url.AbsolutePath;

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region PreRender
		/// <summary>
		/// PreRender
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			
		}
		#endregion

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e) {

			#region Gestion du cas ou l'utilisateur n'a droit qu�� un m�dia
			if (OneVehicleSelectionWebControl1.Items.Count ==1){
				OneVehicleSelectionWebControl1.Items[0].Selected = true;
				this.validateButton_Click(this.validateButton, null);
			}
			#endregion
		}
		#endregion
		
		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Ev�nement d'initialisation de la page de la page
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
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
		private void InitializeComponent(){
            

		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			OneVehicleSelectionWebControl1.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession=_webSession;
			return tmp;

		}
		#endregion

		#region Bouton valider
		/// <summary>
		/// Sauvegarde de l'univers des vehicles s�lectionn�s
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void validateButton_Click(object sender, System.EventArgs e) {
			try{
				string vehiclesSelection="";
				// On recherche les �l�ments s�lectionn�s
				foreach(ListItem currentItem in OneVehicleSelectionWebControl1.Items){
					if(currentItem.Selected)vehiclesSelection+=currentItem.Value+",";
				}
				if(vehiclesSelection.Length<1){
					//_webSession.Source.Close();
					Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(1052,_webSession.SiteLanguage)));
				}
				else{
					vehiclesSelection.Substring(0,vehiclesSelection.Length-1);
					// Sauvegarde de la s�lection dans la session
					//Si la s�lection comporte des �l�ments, on la vide
					_webSession.SelectionUniversMedia.Nodes.Clear();
					System.Windows.Forms.TreeNode tmpNode;
					foreach(ListItem currentItem in OneVehicleSelectionWebControl1.Items){
						if(currentItem.Selected){
							tmpNode=new System.Windows.Forms.TreeNode(currentItem.Text);
							tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess,long.Parse(currentItem.Value),currentItem.Text);
							tmpNode.Checked=true;
							//_webSession.CurrentUnivers. .Nodes.Add(tmpNode);
							_webSession.SelectionUniversMedia.Nodes.Add(tmpNode);
							// Tracking
							_webSession.OnSetVehicle(long.Parse(currentItem.Value));
						}
					}
					//verification que l unite deja s�lectiuonn�e convient pour tous les medias
					if(WebFunctions.Units.getUnitsFromVehicleSelection(
						_webSession.GetSelection(_webSession.SelectionUniversMedia,CstWebCustomer.Right.type.vehicleAccess))
						.IndexOf(_webSession.Unit)==-1){
						// On met euro par d�faut
						if(_webSession.GetSelection(_webSession.SelectionUniversMedia,CstWebCustomer.Right.type.vehicleAccess)==DBClassificationConstantes.Vehicles.names.press.GetHashCode().ToString() 
							|| _webSession.GetSelection(_webSession.SelectionUniversMedia,CstWebCustomer.Right.type.vehicleAccess)==DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode().ToString()){
							_webSession.Unit=TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.pages;
						}
						else{
							_webSession.Unit=TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.euro;
						}
					}
					// On sauvegarde la session
					_webSession.Save();
					//Redirection vers la page suivante
					if(_absoluteUrlPath.Length>0){
						_webSession.Source.Close();
						_nextUrl = this._currentModule.GetOptionalNextUrl(_absoluteUrlPath);
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

		#endregion

		#region Impl�mentation m�thodes abstraites
		/// <summary>
		/// Validation de la s�lection de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected override void ValidateSelection(object sender, System.EventArgs e){
			this.validateButton_Click(sender,e);
		}

		/// <summary>
		/// Obtient l'url suivante
		/// </summary>
		/// <returns>Url suivante</returns>
		protected override string GetNextUrlFromMenu(){
			return(this.MenuWebControl2.NextUrl);
		}
		#endregion

	}
}
