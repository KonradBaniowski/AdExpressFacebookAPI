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
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using DBFunctions = TNS.AdExpress.Web.DataAccess.Functions;
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
					
					// Vehicle
				   if (((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess || ((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.vehicleException) {
						advertiserAdexpressText.Code = 1089;
					}
					// Category
					else if (((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess || ((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.categoryException) {
						advertiserAdexpressText.Code = 1090;
					}
                   // Region
                   else if (((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.regionAccess || ((LevelInformation)treeNodeUniverse.FirstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.regionException)
                   {
                       advertiserAdexpressText.Code = 2816;
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

					string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
					TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl selectItemsInClassificationWebControl = new TNS.AdExpress.Web.Controls.Selections.SelectItemsInClassificationWebControl();
					selectItemsInClassificationWebControl.TreeViewIcons = "/App_Themes/"+themeName+"/Styles/TreeView/Icons";
					selectItemsInClassificationWebControl.TreeViewScripts = "/App_Themes/" + themeName + "/Styles/TreeView/Scripts";
					selectItemsInClassificationWebControl.TreeViewStyles = "/App_Themes/" + themeName + "/Styles/TreeView/Css";
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
					selectItemsInClassificationWebControl.DBSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label;
					for (int k = 0; k < adExpressUniverse.Count; k++) {
						if (adExpressUniverse.ContainsKey(k)) {
                            advertiserText += selectItemsInClassificationWebControl.ShowUniverse(adExpressUniverse[k], _webSession.DataLanguage, DBFunctions.GetDataSource(_webSession));
						}
					}
				}

				//Liste des Univers
				if (treeNodeUniverse != null) 
				advertiserText=TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(treeNodeUniverse,false,true,true,600,true,false,_webSession.SiteLanguage,2,1,true,_webSession.DataLanguage,_webSession.CustomerDataFilters.DataSource);

				
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
