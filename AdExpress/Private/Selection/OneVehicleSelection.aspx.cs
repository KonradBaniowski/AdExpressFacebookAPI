#region Informations
// Auteur: G. Facon
// Date de cr�ation: 05/05/2004
// Date de modification: 10/05/2004
//	19/12/2004 G. Facon Int�gration de WebPage
//	01/08/2006 Modification FindNextUrl
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Web;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Classification;

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Page de S�lection des Vehicles
	/// </summary>
	public partial class OneVehicleSelection : TNS.AdExpress.Web.UI.SelectionWebPage{

		#region Variables MMI
		/// <summary>
		/// Texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText1;
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
		/// Contr�le informations 
		/// </summary>
		/// <summary>
		/// S�lection du vehicle
		/// </summary>
		
		#endregion

		#region Variables
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public OneVehicleSelection():base(){
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
                #region Test Cedexis
                //Test Cedexis
                if (WebApplicationParameters.CountryCode == TNS.AdExpress.Constantes.Web.CountryCode.FRANCE &&
                !Page.ClientScript.IsClientScriptBlockRegistered("CedexisScript"))
                {
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "CedexisScript", TNS.AdExpress.Web.Functions.Script.CedexisScript());
                }
                #endregion

				#region Textes et langage du site
				
                HeaderWebControl1.Language = _webSession.SiteLanguage;

				// Initialisation de la liste des m�dia
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;				
				#endregion			
			
				#region Remise � z�ro de CompetitorUniversMedia
				_webSession.CompetitorUniversMedia.Clear();					
				#endregion

				#region initialisation de r�f�rence univers media
				_webSession.ReferenceUniversMedia=new System.Windows.Forms.TreeNode("media");
				_webSession.Save();
				#endregion					

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){

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
			try{
            OneVehicleSelectionWebControl1.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession=_webSession;
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
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
			try
			{
                List<System.Windows.Forms.TreeNode> levelsSelected = new List<System.Windows.Forms.TreeNode>();
                System.Windows.Forms.TreeNode tmpNode;

				// On recherche les �l�ments s�lectionn�s
				foreach(ListItem currentItem in OneVehicleSelectionWebControl1.Items){
					if(currentItem.Selected)
					{
                        tmpNode = new System.Windows.Forms.TreeNode(currentItem.Text);
                        tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess,long.Parse(currentItem.Value), currentItem.Text);
                        tmpNode.Checked = true;
                        levelsSelected.Add(tmpNode);
					}
				}
                if (levelsSelected.Count==0)
                {
					Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(1052,_webSession.SiteLanguage)));
				}
				else{

                    //Reinitialize banners selection if change vehicle
                    Dictionary<Int64, VehicleInformation> vehicleInformationList = _webSession.GetVehiclesSelected();
                    if (OneVehicleSelectionWebControl1.Items.Count != vehicleInformationList.Count) {                      
                            foreach (System.Windows.Forms.TreeNode node in levelsSelected)
                            {
                                if (!vehicleInformationList.ContainsKey(((LevelInformation)node.Tag).ID))
                                {
                                    _webSession.SelectedBannersFormatList = string.Empty;
                                    break;
                                }
                            
                            }
                    }
                    else {
                        _webSession.SelectedBannersFormatList = string.Empty;
                    }

                    // Sauvegarde de la s�lection dans la session
                    //Si la s�lection comporte des �l�ments, on la vide
                    _webSession.SelectionUniversMedia.Nodes.Clear();
                
					 foreach (System.Windows.Forms.TreeNode node in levelsSelected){
                        _webSession.SelectionUniversMedia.Nodes.Add(node);
                        // Tracking
                            _webSession.OnSetVehicle(((LevelInformation)node.Tag).ID);                       
					}
					// On sauvegarde la session
					_webSession.Save();
					//Redirection vers la page suivante
					if(_nextUrl.Length>0){
						_webSession.Source.Close();
						HttpContext.Current.Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession,false);
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
