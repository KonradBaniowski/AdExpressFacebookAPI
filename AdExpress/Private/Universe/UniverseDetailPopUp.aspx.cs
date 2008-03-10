#region Informations
// Auteur: A. Obermeyer
// Date de création: 
// Date de modification: 
//		30/12/2004 A. Obermeyer Intégration de WebPage
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using TNS.AdExpress.Web.Core.Sessions;

namespace AdExpress.Private.Universe{

	/// <summary>
	/// Détail d'un Univers
	/// </summary>
	public partial class UniverseDetailPopUp : TNS.AdExpress.Web.UI.PrivateWebPage{
	
	
		#region Variables MMI
		/// <summary>
		/// Bouton Fermer
		/// </summary>
		/// <summary>
		/// Détail d'un Univers
		/// </summary>
		/// <summary>
		/// Nom de l'univers
		/// </summary>
		/// <summary>
		/// Type de l'univers
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Tableau avec la liste d'univers
		/// </summary>
		public string advertiserText; 
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public UniverseDetailPopUp():base(){					
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			
			try{
				// identifiant Univers
				Int64 idUniverse;
				advertiserAdexpressText.Language = _webSession.SiteLanguage;
				detailUniversAdExpressText.Language = _webSession.SiteLanguage;
				//Variables URL
				idUniverse= Int64.Parse(Page.Request.QueryString.Get("idUniverse"));
				System.Windows.Forms.TreeNode treeNodeUniverse = null;
				//Arbre Universe 
				Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> adExpressUniverse = null;
				if (TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.IsUniverseBelongToClientDescription(_webSession, idUniverse, TNS.AdExpress.Constantes.Web.LoadableUnivers.GENERIC_UNIVERSE))
					adExpressUniverse = (Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>)TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetObjectUniverses(idUniverse, _webSession);
				else  treeNodeUniverse=(System.Windows.Forms.TreeNode)((ArrayList)TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetTreeNodeUniverse(idUniverse,_webSession))[0];
							
				//Type d'arbre
				#region Type d'arbre
				if (treeNodeUniverse != null) {
					//// Holding Company
					//if (((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess || ((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException) {
					//    advertiserAdexpressText.Code = 814;
					//}
					//// Advertiser
					//else if (((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess || ((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.advertiserException) {
					//    advertiserAdexpressText.Code = 813;

					//}
					//// Product
					//else if (((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.productAccess || ((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.productException) {
					//    advertiserAdexpressText.Code = 815;
					//}
					//// Sector
					//else if (((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess || ((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.sectorException) {
					//    advertiserAdexpressText.Code = 965;

					//}
					//// SubSector
					//else if (((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess || ((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.subSectorException) {
					//    advertiserAdexpressText.Code = 966;
					//}
					//// Group
					//else if (((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.groupAccess || ((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.groupException) {
					//    advertiserAdexpressText.Code = 967;
					//}
					// Vehicle
				   if (((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess || ((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.vehicleException) {
						advertiserAdexpressText.Code = 1089;
					}
					// Category
					else if (((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess || ((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.categoryException) {
						advertiserAdexpressText.Code = 1090;
					}
					// Media
					else if (((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess || ((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.mediaException) {
						advertiserAdexpressText.Code = 1087;
					}
					// Program Type
					else if (((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.programTypeAccess || ((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.programTypeException) {
						advertiserAdexpressText.Code = 2062;
					}
					// Sponsorship Form
					else if (((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.sponsorshipFormAccess || ((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.sponsorshipFormException) {
						advertiserAdexpressText.Code = 2063;
					}

				}
				#endregion

				if (adExpressUniverse != null && adExpressUniverse.Count > 0) {					
					advertiserAdexpressText.Code = 1759;
					
					TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl selectItemsInClassificationWebControl = new TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl();
					selectItemsInClassificationWebControl.TreeViewIcons = "/Styles/TreeView/Icons";
					selectItemsInClassificationWebControl.TreeViewScripts = "/Styles/TreeView/Scripts";
					selectItemsInClassificationWebControl.TreeViewStyles = "/Styles/TreeView/Css";
					selectItemsInClassificationWebControl.ChildNodeExcludeCss = "txtChildNodeExcludeCss";
					selectItemsInClassificationWebControl.ChildNodeIncludeCss = "txtChildNodeIncludeCss";
					selectItemsInClassificationWebControl.ParentNodeChildExcludeCss = "txtParentNodeChildExcludeCss";
					selectItemsInClassificationWebControl.ParentNodeChildIncludeCss = "txtParentNodeChildIncludeCss";
					selectItemsInClassificationWebControl.TreeExcludeFrameBodyCss = "treeExcludeFrameBodyCss";
					selectItemsInClassificationWebControl.TreeExcludeFrameCss = "treeExcludeFrameCss";
					selectItemsInClassificationWebControl.TreeExcludeFrameHeaderCss = "treeExcludeFrameHeaderCss";
					selectItemsInClassificationWebControl.TreeIncludeFrameBodyCss = "treeIncludeFrameBodyCss";
					selectItemsInClassificationWebControl.TreeIncludeFrameCss = "treeIncludeFrameCss";
					selectItemsInClassificationWebControl.TreeIncludeFrameHeaderCss = "treeIncludeFrameHeaderCss";
					selectItemsInClassificationWebControl.SiteLanguage = _webSession.SiteLanguage;
					selectItemsInClassificationWebControl.DBSchema = TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA;
					for (int k = 0; k < adExpressUniverse.Count; k++) {
						if (adExpressUniverse.ContainsKey(k)) {
							advertiserText += selectItemsInClassificationWebControl.ShowUniverse(adExpressUniverse[k], _webSession.SiteLanguage, _webSession.Source);
						}
					}
				}

				//Liste des Univers
				if (treeNodeUniverse != null) 
				advertiserText=TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(treeNodeUniverse,false,true,true,600,true,false,_webSession.SiteLanguage,2,1,true);

				//Bouton Fermer
				closeImageButtonRollOverWebControl.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/fermer_up.gif";
				closeImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/fermer_down.gif";
			
				//Script pour l'ouverture/fermeture du tableau
				if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent",TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				}
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ShowHideContent1",TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
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

		#region Bouton Fermer
		/// <summary>
		/// Fermeture de la popup
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void closeImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			Response.Write("<script language=javascript>");
			Response.Write("	window.close();");
			Response.Write("</script>");
		}
		#endregion
	}
}
