#region Informations
// Auteur: G. Facon
// Date de création: 05/05/2004
// Date de modification: 10/05/2004
//	30/12/2004 A. Obermeyer Intégration de WebPage
//	19/07/2005 G. Facon Ajout du IsPostBack dans le page_load
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
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using WebConstantes=TNS.AdExpress.Constantes.Web;

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Page de Sélection des Vehicles
	/// </summary>
	public partial class VehicleSelection : TNS.AdExpress.Web.UI.SelectionWebPage{

		#region Varaibles MMI
		/// <summary>
		/// Texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText1;
		/// <summary>
		/// Liste des modules à sélectionner
		/// </summary>
		/// <summary>
		/// Entête de la page
		/// </summary>
		/// <summary>
		/// Bouton validation
		/// </summary>
		/// <summary>
		/// Titre de la page
		/// </summary>
		/// <summary>
		/// Texte "Sélectionner tous les médias"
		/// </summary>
		/// <summary>
		/// Main Menu
		/// </summary>
		/// <summary>
		/// Contrôle information
		/// </summary>
		/// <summary>
		/// Texte titre
		/// </summary>
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public VehicleSelection():base(){
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
			try{			
				if (!IsPostBack){

					#region Textes et langage du site
					//Modification de la langue pour les Textes AdExpress
                    //for (int i = 0; i < this.Controls.Count; i++) {
                    //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                    //}
					
					//validateButton.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
					//validateButton.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
					#endregion					

				}

				//Annuler l'univers de version
				if(_webSession.CurrentModule == WebConstantes.Module.Name.ALERTE_PLAN_MEDIA){
					_webSession.IdSlogans = new ArrayList();
					_webSession.SloganIdZoom=-1;
					_webSession.Save();
				}

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

			#region Gestion du cas ou l'utilisateur n'a droit quèà un média
			if (VehicleSelectionWebControl2.Items.Count ==1){
				VehicleSelectionWebControl2.Items[0].Selected = true;
				this.validateButton_Click(this.validateButton, null);
			}
			#endregion			
		}
		#endregion
		
		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation de la page de la page
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
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
		private void InitializeComponent(){
           
            this.Unload += new System.EventHandler(this.Page_UnLoad);
           
		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {

			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			ModuleTitleWebControl1.CustomerWebSession = _webSession;
			InformationWebControl1.Language = _webSession.SiteLanguage;
			VehicleSelectionWebControl2.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession=_webSession;
			return tmp;

		}
		#endregion

		#region Bouton valider
		/// <summary>
		/// Sauvegarde de l'univers des vehicles sélectionnés
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void validateButton_Click(object sender, System.EventArgs e) {
			try{
				string vehiclesSelection="";
				// On recherche les éléments sélectionnés
				foreach(ListItem currentItem in VehicleSelectionWebControl2.Items){
					if(currentItem.Selected)vehiclesSelection+=currentItem.Value+",";
				}
                if(vehiclesSelection.Length < 1) {
                    //_webSession.Source.Close();
                    Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(1052, _webSession.SiteLanguage)));
                }
                else {
                    vehiclesSelection.Substring(0, vehiclesSelection.Length - 1);
                    // Sauvegarde de la sélection dans la session
                    //Si la sélection comporte des éléments, on la vide
                    _webSession.SelectionUniversMedia.Nodes.Clear();
                    System.Windows.Forms.TreeNode tmpNode;
                    foreach(ListItem currentItem in VehicleSelectionWebControl2.Items) {
                        if(currentItem.Selected) {
                            tmpNode = new System.Windows.Forms.TreeNode(currentItem.Text);
                            tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, long.Parse(currentItem.Value), currentItem.Text);
                            tmpNode.Checked = true;
                            //_webSession.CurrentUnivers. .Nodes.Add(tmpNode);
                            _webSession.SelectionUniversMedia.Nodes.Add(tmpNode);
                            // Tracking
                            _webSession.OnSetVehicle(long.Parse(currentItem.Value));
                        }
                    }
                    
                    //verification que l unite deja sélectionnée convient pour tous les medias
                    ArrayList unitList = WebFunctions.Units.getUnitsFromVehicleSelection(_webSession.GetSelection(_webSession.SelectionUniversMedia, CstWebCustomer.Right.type.vehicleAccess));

                    if(unitList.Count == 0) {
                        // Message d'erreur pour indiquer qu'il n'y a pas d'unité commune dans la sélection de l'utilisateur
                        Response.Write("<script language=javascript>");
                        Response.Write("	alert(\"" + GestionWeb.GetWebWord(2541, this._siteLanguage) + "\");");
                        Response.Write("</script>");
                    }
                    else {
                        // On sauvegarde la session
                        _webSession.Save();
                        //Redirection vers la page suivante
                        if(_nextUrl.Length > 0) {
                            _webSession.Source.Close();
                            HttpContext.Current.Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
                        }
                    }

                    #region Ancienne vérification sur l'unité
                    //if(WebFunctions.Units.getUnitsFromVehicleSelection(
                    //    _webSession.GetSelection(_webSession.SelectionUniversMedia, CstWebCustomer.Right.type.vehicleAccess))
                    //    .IndexOf(_webSession.Unit) == -1) {
                    //    // On met euro par défaut
                    //    _webSession.Unit = TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.euro;
                    //}
                    //// On sauvegarde la session
                    //_webSession.Save();
                    ////Redirection vers la page suivante
                    //if(_nextUrl.Length > 0) {
                    //    _webSession.Source.Close();
                    //    HttpContext.Current.Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
                    //}
                    #endregion

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

		#region Implémentation méthodes abstarites
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
