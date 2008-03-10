#region Informations
// Auteur: D.V Mussuma 
// Date de cr�ation: 14/09/2004 
// Date de modification: 
//	30/12/2004  D.V Mussuma Int�gration de WebPage
//	01/08/2006 Modification FindNextUrl
#endregion

#region namespace
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
using System.Windows.Forms;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;

using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantesClassification=TNS.AdExpress.Constantes.Classification.DB;
#endregion

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Page de s�lection des m�dia pour l'analyse s�ctorielle
	/// </summary>
	public partial class SectorAnalysisVehicleSelection :  TNS.AdExpress.Web.UI.SelectionWebPage{								
		
		#region Variables MMI
		/// <summary>
		/// Control de s�lection des M�dia
		/// </summary>
		/// <summary>
		/// Ent�te de la page
		/// </summary>
		/// <summary>
		/// Titre de la page
		/// </summary>
		/// <summary>
		/// Texte "S�lectionner tous les m�dias"
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText2;
		/// <summary>
		/// Texte titre
		/// </summary>
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Bouton de validation 
		/// </summary>
		#endregion
		
		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public SectorAnalysisVehicleSelection():base(){			
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

			#region Textes et langage du site
			//Modification de la langue pour les Textes AdExpress
			TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);			
			ModuleTitleWebControl1.CustomerWebSession = _webSession;
			InformationWebControl1.Language = _webSession.SiteLanguage;
			validateButton.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
			validateButton.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";			
			#endregion

			#region Rappel des diff�rentes s�lections
//			ArrayList linkToShow=new ArrayList();			
//			if(_webSession.isSelectionProductSelected())linkToShow.Add(2);			
//			if(_webSession.isDatesSelected())linkToShow.Add(5);
//			recallWebControl.LinkToShow=linkToShow;
//			if(_webSession.LastReachedResultUrl.Length>0 && _webSession.isSelectionProductSelected() && _webSession.isDatesSelected())recallWebControl.CanGoToResult=true;
			#endregion

			#region Url Suivante
//			_nextUrl=this.recallWebControl.NextUrl;
//			if(_nextUrl.Length==0)_nextUrl=_currentModule.FindNextUrl(Request.Url.AbsolutePath);
//			else validateButton_Click(this, null);
			#endregion

			#region D�finition de la page d'aide
//			helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"SectorAnalysisVehicleSelectionHelp.aspx";
			#endregion

			#region Script
			// Cochage/Decochage des checkbox p�res, fils et concurrents
			if (!Page.ClientScript.IsClientScriptBlockRegistered("CheckAllChilds")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"CheckAllChilds",TNS.AdExpress.Web.Functions.Script.CheckAllChilds());
			}
			// Ouverture/fermeture des fen�tres p�res
			if (!Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DivDisplayer",TNS.AdExpress.Web.Functions.Script.DivDisplayer());
			}
			// fermer/ouvrir tous les calques
			if (!Page.ClientScript.IsClientScriptBlockRegistered("ExpandColapseAllDivs")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ExpandColapseAllDivs",TNS.AdExpress.Web.Functions.Script.ExpandColapseAllDivs());
			}	
			// S�lection de tous les fils
			if (!Page.ClientScript.IsClientScriptBlockRegistered("SelectAllChilds")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"SelectAllChilds",TNS.AdExpress.Web.Functions.Script.SelectAllChilds());
			}	
			// S�lection de tous les fils
			if (!Page.ClientScript.IsClientScriptBlockRegistered("SelectExclusiveAllChilds")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"SelectExclusiveAllChilds",TNS.AdExpress.Web.Functions.Script.SelectExclusiveAllChilds());
			}					
			#endregion

		}
		#endregion
		
		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){						
		}
		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e)
		{
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
		private void InitializeComponent()
		{
            
            this.Unload += new System.EventHandler(this.Page_UnLoad);
         

		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode(){
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			//recallWebControl.CustomerWebSession=_webSession;
			SectorAnalysisVehicleSelectionWebControl1.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
			return tmp;
		}
		#endregion

		#region Bouton valider
		/// <summary>
		/// Sauvegarde de l'univers des m�dias s�lectionn�s
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void validateButton_Click(object sender, System.EventArgs e)
		{		
			#region Variables locales
			System.Windows.Forms.TreeNode current = new System.Windows.Forms.TreeNode("ChoixMedia");
			System.Windows.Forms.TreeNode vehicle = null;
			string idVehicule="";
			string OldIdVehicule="";
			bool firstIdVeh =true;
			bool vhAdded = false;
			System.Windows.Forms.TreeNode category = null;
			System.Windows.Forms.TreeNode media = null;			
			string idCategory="";
			string OldIdCategory="";
			bool firstIdCat=true;
			int vhCpt = 0;
			int ctCpt = 0;
			int mdCpt = 0;
			#endregion
						
			#region Construction de l'arbre des m�dias - NOUVELLE VERSION
			foreach(ListItem item in this.SectorAnalysisVehicleSelectionWebControl1.Items ) {	
				//Traitement Noeud media(vehicle)
				if (item.Value.IndexOf("vh_") > -1) {
					idVehicule=item.Value.ToString();					
					//ajout de la categorie pr�c�dente 
//					if(!firstIdVeh && (vehicle.Nodes.Count > 0 || vehicle.Checked  ) && !idVehicule.Equals("") && !OldIdVehicule.Equals(idVehicule.Trim())  ) {
					if(!firstIdVeh && ((vehicle.Nodes.Count > 0 || vehicle.Checked )||(category!=null && (category.Nodes.Count>0 || category.Checked)) ) && !idVehicule.Equals("") && !OldIdVehicule.Equals(idVehicule.Trim())  ) {
					//Rajout de la derni�re categorie dans media(vehicle) puis du dernier media dans l'arbre													
//						if(!vehicle.Checked && category!=null && (category.Nodes.Count>0 || category.Checked)  && !vehicle.Nodes.Contains(category))vehicle.Nodes.Add(category);																				
						if(!vehicle.Checked && category!=null && !vehicle.Nodes.Contains(category))vehicle.Nodes.Add(category);																				
						current.Nodes.Add(vehicle);
						vhAdded=true;
						break;						
					}
					vehicle = new System.Windows.Forms.TreeNode(item.Text);
					//Creation d'un nouveau noeud m�dia (vehicle)																	
					if(item.Selected)vehicle.Tag = new LevelInformation(CstWebCustomer.Right.type.vehicleAccess,Int64.Parse(item.Value.Remove(0,3)),item.Text);					
					else vehicle.Tag = new LevelInformation(CstWebCustomer.Right.type.vehicleException,Int64.Parse(item.Value.Remove(0,3)),item.Text);					
					vehicle.Checked = item.Selected;
					if (vehicle.Checked) vhCpt++;
					idVehicule=item.Value.ToString();
					OldIdVehicule=item.Value.ToString();
					firstIdVeh=false;					
				}
				//Traitement Noeud categorie
				else if (!vehicle.Checked && item.Value.IndexOf("ct_") > -1 ) {	
					idCategory=item.Value.ToString();
					//ajout de la categorie pr�c�dente 
					if(  !firstIdCat && ( category.Nodes.Count > 0 || category.Checked) && !idCategory.Equals("") && !OldIdCategory.Equals(idCategory.Trim())) {
						vehicle.Nodes.Add(category);												
					}
					//Creation d'un nouveau noeud categorie
					category = new System.Windows.Forms.TreeNode(item.Text);
					if(item.Selected)category.Tag = new LevelInformation(CstWebCustomer.Right.type.categoryAccess,Int64.Parse(item.Value.Remove(0,3)),item.Text);					
					else category.Tag = new LevelInformation(CstWebCustomer.Right.type.categoryException,Int64.Parse(item.Value.Remove(0,3)),item.Text);										
					category.Checked = item.Selected;			
					if (category.Checked) ctCpt++;
					OldIdCategory=item.Value.ToString();
					firstIdCat=false;
				}
				//Traitement Noeud support(media)
				else if( !vehicle.Checked && !category.Checked  && item.Value.IndexOf("md_") > -1 && item.Selected ) {				
					media = new System.Windows.Forms.TreeNode(item.Text);
					media.Tag = new LevelInformation(CstWebCustomer.Right.type.mediaAccess,Int64.Parse(item.Value.Remove(0,3)),item.Text);					
					media.Checked =item.Selected ;
					if (media.Checked) mdCpt++;
					category.Nodes.Add(media);
				}				
			}
			//ajout de la derni�re node vehicle si n�cessaire
			//Rajout de la derni�re categorie dans media(vehicle) puis du dernier media dans l'arbre	
			if(!vehicle.Checked && category!=null &&  vehicle!=null && (category.Checked || category.Nodes.Count>0)  && !vehicle.Nodes.Contains(category)){
				vehicle.Nodes.Add(category);				
			}
			if (current.Nodes.Count ==0 && !vhAdded && (vehicle.Nodes.Count >0 || vehicle.Checked)  )			
				current.Nodes.Add(vehicle);										
			#endregion

			#region MAJ de la session
			if ((mdCpt+ctCpt+vhCpt)>0){
				if( mdCpt < WebConstantes.Advertiser.MAX_NUMBER_ADVERTISER
					&& vhCpt < WebConstantes.Advertiser.MAX_NUMBER_ADVERTISER
					&& ctCpt < WebConstantes.Advertiser.MAX_NUMBER_ADVERTISER){			

					#region Tracking
					_webSession.OnSetVehicle(((LevelInformation)current.Nodes[0].Tag).ID);
					#endregion

					#region Extraction du dernier moi sdispo dans les recap
					_webSession.LastAvailableRecapMonth = DBFunctions.CheckAvailableDateForMedia(((LevelInformation)vehicle.Tag).ID, _webSession);
					#endregion

					_webSession.SelectionUniversMedia = _webSession.CurrentUniversMedia = current;
					if (((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID != DBConstantesClassification.Vehicles.names.plurimedia.GetHashCode())
						_webSession.PreformatedMediaDetail = WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory;
					else
						_webSession.PreformatedMediaDetail = WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle;
					_webSession.Save();
					//Redirection Page suivante
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession+"");
				}
				else{
					foreach(ListItem item in this.SectorAnalysisVehicleSelectionWebControl1.Items ){
						if (item.Selected) item.Selected=false;
					}
					this.SectorAnalysisVehicleSelectionWebControl1.Reload = true;
					Response.Write("<script language=javascript>");
                    Response.Write(" alert(\"" + GestionWeb.GetWebWord(2265, _webSession.SiteLanguage) + "\");");
					Response.Write("</script>");
				}
			}
			else {
				this.SectorAnalysisVehicleSelectionWebControl1.Reload = true;
				Response.Write("<script language=javascript>");
				Response.Write(" alert(\""+GestionWeb.GetWebWord(1199,_webSession.SiteLanguage)+"\");");
				Response.Write("</script>");
			}
			#endregion			

		}
		#endregion

		#endregion

		#region M�thodes interne
		/// <summary>
		/// Indique si le niveau cat�gorie doit �tre montrer pour le m�dia (vehicle)
		/// </summary>
		/// <param name="idVehicle">Vehicle � traiter</param>
		/// <returns>True s'il doit �tre montrer, false sinon</returns>
		private bool showCategory(Int64 idVehicle) {
			DBConstantesClassification.Vehicles.names vehicletype=(DBConstantesClassification.Vehicles.names)int.Parse(idVehicle.ToString());
			switch(vehicletype) {
				case DBConstantesClassification.Vehicles.names.cinema:
					return(false);
				case DBConstantesClassification.Vehicles.names.plurimedia:
					return(false);
				default:
					return(true);
			}
		}
		#endregion

		#region Impl�mentation m�thodes abstraites
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
