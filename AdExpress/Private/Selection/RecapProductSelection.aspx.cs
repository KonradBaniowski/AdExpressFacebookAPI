#region Informations
// Auteur: D. Mussuma
// Date de création: 08/09/2004
// Date de modification: 10/05/2004
//	30/12/2004  D. Mussuma Intégration de WebPage
//	01/08/2006 Modification FindNextUrl
#endregion

#region NameSpace
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
using Oracle.DataAccess.Client;
using System.Windows.Forms;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using DBConstantesClassification = TNS.AdExpress.Constantes.Classification.DB;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;

#endregion

namespace AdExpress.Private.Selection{
	/// <summary>
	/// Page de sélection de l'univers produit dans une analyse sectorielle
	/// </summary>
	public partial class RecapProductSelection : TNS.AdExpress.Web.UI.SelectionWebPage{
		
		#region Variables
		/// <summary>
		/// Code HTML du rappel des univers sauvegardés
		/// </summary>
		public string listUniverses = "";
		/// <summary>
		/// Code javascript pour l'ouverture du div lors du chargement d'un univers
		/// </summary>
		public string openDiv="";
		/// <summary>
		/// Activation ou non de l'ouverture de la popup de sauvegarde de l univers
		/// </summary>
		protected string SaveUniversScript = "";
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose="";
		#endregion

		#region variables MMI
		/// <summary>
		/// Texte "Module X"
		/// </summary>
		/// <summary>
		/// Entete flash et menu
		/// </summary>
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <summary>
		/// Bouton sauvegarder l'univers
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText1;
		/// <summary>
		/// Bouton Chargement d'un univers
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl universLoadButton;
		/// <summary>
		/// Control de sélection des groupes et variétés
		/// </summary>
		/// <summary>
		/// Menu contextuel
		/// </summary>
		/// <summary>
		/// Control pour chargement d'un univers
		/// </summary>
		#endregion
			
		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public RecapProductSelection():base()
		{			
			#region Initialisation des annonceurs de références et concurrents
			//_webSession.ReferenceUniversAdvertiser=new System.Windows.Forms.TreeNode("produit");
			//_webSession.CompetitorUniversAdvertiser=new Hashtable(5);
			_webSession.SecondaryProductUniverses = new Dictionary<int, AdExpressUniverse>();
			_webSession.Save();			
			#endregion

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

				#region Flash d'attente
				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null){
					string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
					//if(nomInput!=MenuWebControl2.ID && nomInput!=validateButton.ID && nomInput!=saveUniversButton.ID){
					if(nomInput!=MenuWebControl2.ID && nomInput!=validateButton.ID){
						Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
						Page.Response.Flush();
						divClose=LoadingSystem.GetHtmlCloseDiv();
					}
				}
				else{
					Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
					Page.Response.Flush();
					divClose=LoadingSystem.GetHtmlCloseDiv();
				}
				#endregion

				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);					
				ModuleTitleWebControl1.CustomerWebSession = _webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				recapProductSelectionWebControl.WebSession=_webSession;						
				//saveUniversButton.ImageUrl="/Images/"+_siteLanguage+"/button/enregistrer_univers_up.gif";
				//saveUniversButton.RollOverImageUrl="/Images/"+_siteLanguage+"/button/enregistrer_univers_down.gif";
				//validateButton.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				//validateButton.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";	
				#endregion							

				#region Script
				//Gestion de la sélection d'un radiobutton dans la liste des univers
				if (!Page.ClientScript.IsClientScriptBlockRegistered("InsertIdMySession4")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"InsertIdMySession4",TNS.AdExpress.Web.Functions.Script.InsertIdMySession4());
				}			
				//Ouverture/Fermeture de la div des univers
				if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showHideContent",TNS.AdExpress.Web.Functions.Script.ShowHideContent());
				}
				// Ouverture/fermeture des fenêtres pères
				if (!Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {			
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DivDisplayer",TNS.AdExpress.Web.Functions.Script.DivDisplayer());
				}			
				// Sélection de tous les fils
				if (!Page.ClientScript.IsClientScriptBlockRegistered("SelectAllChilds")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"SelectAllChilds",TNS.AdExpress.Web.Functions.Script.SelectAllChilds());
				}			
				// Intégration automatique
				if (!Page.ClientScript.IsClientScriptBlockRegistered("GroupIntegration")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"GroupIntegration",TNS.AdExpress.Web.Functions.Script.GroupIntegration());
				}	
				// Intégration automatique
				if (!Page.ClientScript.IsClientScriptBlockRegistered("ExpandColapseAllDivs")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ExpandColapseAllDivs",TNS.AdExpress.Web.Functions.Script.ExpandColapseAllDivs());
				}	
				#endregion

				#region Charger
				if(Request.Form.Get("__EVENTTARGET")=="loadImageButtonRollOverWebControl"){
					universLoadButton_Click(null,null);
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
		/// Evènement d'initialisation de la page 
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

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation de certains composants
		/// </summary>
		/// <returns>?</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			//recallWebControl.CustomerWebSession=_webSession;
			LoadableUniversWebControl.CustomerWebSession=_webSession;
			LoadableUniversWebControl.ListBranchType=TNS.AdExpress.Constantes.Classification.Branch.type.product.GetHashCode().ToString();
			LoadableUniversWebControl.SelectionPage = true;
			LoadableUniversWebControl.Dimension_ = TNS.Classification.Universe.Dimension.product;
			LoadableUniversWebControl.ForGenericUniverse = true;
			MenuWebControl2.CustomerWebSession = _webSession;
			return tmp;
		}
		#endregion

		#region Bouton Valider
		/// <summary>
		/// Valider la sélection
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">argument(s)</param>
		protected void validateButton_Click(object sender, System.EventArgs e) {
			System.Windows.Forms.TreeNode current = new System.Windows.Forms.TreeNode("produit");

			#region Variables locales
			//System.Windows.Forms.TreeNode group = null;
			//System.Windows.Forms.TreeNode tmp = null;
			//bool first = true;

			int gpCpt = 0;
			int sgInCpt = 0;
			int sgNotInCpt = 0;

			string errorMessage = null;

			//Pour univers generiques
			System.Collections.Generic.Dictionary<int, AdExpressUniverse> universeDictionary = null;
			AdExpressUniverse adExpressUniverse = null;
			List<long> itemList = new List<long>();
			NomenclatureElementsGroup nomenclatureExludeGroup = null, nomenclatureIncludeGroup = null;
			List<long> groupIncludeSelectedList = null, segmentIncludeSelectedList = null, segmentExcludeSelectedList = null;
			bool isCurrentGroupSelected = false;
			#endregion

			#region Construction de l'arbre
			_webSession.PrincipalProductUniverses.Clear();
			foreach(ListItem item in this.recapProductSelectionWebControl.Items ){
				if (item.Value.IndexOf("gp_") > -1){
					//ajout de la node précédente si nécessaire

					#region  Ancienne version
					//if (!first && (group.Checked || group.Nodes.Count>0)){
					//    current.Nodes.Add(group);
					//}
					//group = new System.Windows.Forms.TreeNode(item.Text);
					//group.Tag = new LevelInformation(CstWebCustomer.Right.type.groupAccess,Int64.Parse(item.Value.Remove(0,3)),item.Text);
					//group.Checked = item.Selected;
					//if (group.Checked) gpCpt++;
					//first=false;
					#endregion
					
					if (item.Selected) {
						if (groupIncludeSelectedList == null) groupIncludeSelectedList = new List<long>();
						groupIncludeSelectedList.Add(Int64.Parse(item.Value.Remove(0, 3)));
						gpCpt++;
						isCurrentGroupSelected = true;
					}
					else {
						isCurrentGroupSelected = false;
					}
					//first = false;
				}
				else{
					//if (group.Checked && !item.Selected){
					if (isCurrentGroupSelected && !item.Selected) {
						
						#region Ancienne version
						//sgNotInCpt++;
						//tmp = new System.Windows.Forms.TreeNode(item.Text);
						//tmp.Tag = new LevelInformation(CstWebCustomer.Right.type.segmentException ,Int64.Parse(item.Value.Remove(0,3)),item.Text);
						//tmp.Checked = item.Selected;
						//group.Nodes.Add(tmp);
						#endregion

						if (segmentExcludeSelectedList == null) segmentExcludeSelectedList = new List<long>();
						segmentExcludeSelectedList.Add(Int64.Parse(item.Value.Remove(0, 3)));
						sgNotInCpt++;
					}
					//else if(!group.Checked && item.Selected){
					else if (!isCurrentGroupSelected && item.Selected) {
						
						#region Ancienne version
						//sgInCpt++;
						//tmp = new System.Windows.Forms.TreeNode(item.Text);
						//tmp.Tag = new LevelInformation(CstWebCustomer.Right.type.segmentAccess ,Int64.Parse(item.Value.Remove(0,3)),item.Text);
						//tmp.Checked = item.Selected;
						//group.Nodes.Add(tmp);
						#endregion

						if (segmentIncludeSelectedList == null) segmentIncludeSelectedList = new List<long>();
						segmentIncludeSelectedList.Add(Int64.Parse(item.Value.Remove(0, 3)));
						sgInCpt++;
					}
				}
			}

			#region Ancienne version
			////ajout de la dernière node si nécessaire
			//if (group.Checked || group.Nodes.Count>0){
			//    current.Nodes.Add(group);
			//}
			#endregion

			//Add segment exlude list into universe 
			int ncIndex = 0;
			if (segmentExcludeSelectedList != null && segmentExcludeSelectedList.Count > 0) {
				nomenclatureExludeGroup = new NomenclatureElementsGroup(ncIndex, AccessType.excludes);
				nomenclatureExludeGroup.AddItems(TNSClassificationLevels.SEGMENT, segmentExcludeSelectedList);
				adExpressUniverse = new AdExpressUniverse(Dimension.product);
				adExpressUniverse.AddGroup(ncIndex, nomenclatureExludeGroup);
				ncIndex++;
			}

			//Add segment or group include list into universe 
			if ((segmentIncludeSelectedList != null && segmentIncludeSelectedList.Count > 0) || (groupIncludeSelectedList != null && groupIncludeSelectedList.Count > 0)) {
				nomenclatureIncludeGroup = new NomenclatureElementsGroup(ncIndex, AccessType.includes);
				if((segmentIncludeSelectedList != null && segmentIncludeSelectedList.Count > 0))
					nomenclatureIncludeGroup.AddItems(TNSClassificationLevels.SEGMENT, segmentIncludeSelectedList);
				if ((groupIncludeSelectedList != null && groupIncludeSelectedList.Count > 0))
					nomenclatureIncludeGroup.AddItems(TNSClassificationLevels.GROUP_, groupIncludeSelectedList);
				if (adExpressUniverse == null) adExpressUniverse = new AdExpressUniverse(Dimension.product);
				 adExpressUniverse.AddGroup(ncIndex, nomenclatureIncludeGroup);				
			}


			#endregion

			#region MAJ de la session
			if ( gpCpt > WebConstantes.Advertiser.MAX_NUMBER_ADVERTISER 
				|| sgInCpt > WebConstantes.Advertiser.MAX_NUMBER_ADVERTISER
				|| sgNotInCpt > WebConstantes.Advertiser.MAX_NUMBER_ADVERTISER){
				foreach(ListItem item in this.recapProductSelectionWebControl.Items ){
					if(item.Selected)item.Selected=false;
				}
				Response.Write("<script language=javascript>");
                Response.Write("	alert(\"" + GestionWeb.GetWebWord(2265, _webSession.SiteLanguage) + "\");");
				Response.Write("</script>");
			}
			else if ( gpCpt < 1 && sgInCpt < 1 && sgNotInCpt < 1){
				Response.Write("<script language=javascript>");
				Response.Write("	alert(\""+GestionWeb.GetWebWord(1198,_webSession.SiteLanguage)+"\");");
				Response.Write("</script>");
			}
			else {
				//Ancienne version
				//_webSession.SelectionUniversProduct = _webSession.CurrentUniversProduct = current;

				//Save universe into web session
				universeDictionary = new System.Collections.Generic.Dictionary<int, AdExpressUniverse>();
				universeDictionary.Add(0, adExpressUniverse);
			
				_webSession.PrincipalProductUniverses = universeDictionary;				

				if (errorMessage == null) {
					_webSession.Save();
					if (_nextUrl.Length > 0) { 
						_webSession.Source.Close();
						HttpContext.Current.Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
					}
				}
				else {
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\"" + errorMessage + "\");");
					Response.Write("</script>");
				}
			}
			#endregion
		}
		#endregion

		#region Bouton Enregistrer
		/// <summary>
		/// Evenement Bouton enregistrer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void saveUniversButton_Click(object sender, System.EventArgs e) {
			//Int64 idUniverseClientDescription=WebConstantes.LoadableUnivers.GROUP_SEGMENT;
			Int64 idUniverseClientDescription = WebConstantes.LoadableUnivers.GENERIC_UNIVERSE;
			this.recapProductSelectionWebControl.LoadSession = true;
			_nextUrl = string.Empty;
			validateButton_Click(null, null);
			SaveUniversScript = WebFunctions.Script.SaveUniverseOpen(_webSession.IdSession, TNS.AdExpress.Constantes.Classification.Branch.type.product,idUniverseClientDescription);
		}
		#endregion

		#region Charger un univers
		/// <summary>
		/// Evenement bouton charger
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void universLoadButton_Click(object sender, System.EventArgs e) {
				
			#region Variables
			string[] tabParent=null;
			Int64 idUniverse=0;
			#endregion

			#region univers sélectionné ?
			foreach (string currentKey in Request.Form.AllKeys){
				tabParent=currentKey.Split('_');
				if(tabParent[0]=="UNIVERSE") {							
					idUniverse=Int64.Parse(tabParent[1]);		
				}
			}
			#endregion

			#region Univers identifié
			if(idUniverse!=0){
				#region ancienne version
				//System.Windows.Forms.TreeNode treeNodeUniverse=(System.Windows.Forms.TreeNode)((ArrayList)TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetTreeNodeUniverse(idUniverse,_webSession))[0];
				//_webSession.SelectionUniversProduct=_webSession.CurrentUniversProduct=treeNodeUniverse;
				#endregion

				System.Collections.Generic.Dictionary<int, AdExpressUniverse> universeDictionary = (Dictionary<int, AdExpressUniverse>)TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetObjectUniverses(idUniverse, _webSession);
				_webSession.PrincipalProductUniverses = universeDictionary;
				_webSession.Save();
				this.recapProductSelectionWebControl.LoadSession = true;

				// Ouverture/fermeture des fenêtres pères
				if (!Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DivDisplayer",TNS.AdExpress.Web.Functions.Script.DivDisplayer());
				}				
				
				#region Ouvre les divs contenant les éléments chargés
				string[] listSectorTable=null;
				string listSector = TNS.AdExpress.Web.DataAccess.Selections.Products.ProductClassificationListDataAccess.ListSectorInUniverse(_webSession, universeDictionary[0]);
				//string listSector=TNS.AdExpress.Web.DataAccess.Selections.Products.ProductClassificationListDataAccess.ListSectorInTreeNode(_webSession,treeNodeUniverse);
				if (listSector.Length > 0) {
					listSectorTable = listSector.Split(',');				
				}

				openDiv="<script language=\"JavaScript\">";
				foreach (string item in listSectorTable) {
					openDiv+="DivDisplayer('sc_"+item+"');";
				}				
				openDiv+="</script>";
				#endregion

			}
			#endregion

			#region Pas d'univers sélectionner
			else{
				Response.Write("<script language=javascript>");
				Response.Write("	alert(\""+GestionWeb.GetWebWord(926,_webSession.SiteLanguage)+"\");");
				Response.Write("history.go(-1);");
				Response.Write("</script>");
			}
			#endregion
			
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

		#region Métohdes internes
		/// <summary>
		/// Set default media selection
		/// <remarks>Plurimedia will be the default choice</remarks>
		/// </summary>
		private void SetDefaultMediaSelection() {			
		
			if (!_webSession.isMediaSelected()) {
				System.Windows.Forms.TreeNode current = new System.Windows.Forms.TreeNode("ChoixMedia");
				System.Windows.Forms.TreeNode vehicle = null;

				vehicle = new System.Windows.Forms.TreeNode(GestionWeb.GetWebWord(210,_webSession.SiteLanguage));
				
				//Creating new plurimedia	node																
				vehicle.Tag = new LevelInformation(CstWebCustomer.Right.type.vehicleAccess, DBConstantesClassification.Vehicles.names.plurimedia.GetHashCode(), GestionWeb.GetWebWord(210, _webSession.SiteLanguage));
				vehicle.Checked = true;
				current.Nodes.Add(vehicle);

				//Tracking
				_webSession.OnSetVehicle(DBConstantesClassification.Vehicles.names.plurimedia.GetHashCode());

				// Extraction Last Available Recap Month
				_webSession.LastAvailableRecapMonth = DBFunctions.CheckAvailableDateForMedia(DBConstantesClassification.Vehicles.names.plurimedia.GetHashCode(), _webSession);

				_webSession.SelectionUniversMedia = _webSession.CurrentUniversMedia = current;
				_webSession.PreformatedMediaDetail = WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle;				
			}
		}

		/// <summary>
		/// Set default period selection
		/// <remarks>Current or last years will be the default period</remarks>
		/// </summary>
		private string  SetDefaultPeriodSelection() {
			
			DateTime downloadDate = new DateTime(_webSession.DownLoadDate, 12, 31);
			string absolutEndPeriod = "";

			if (!_webSession.isDatesSelected()) {
				try {
					
					//Choix par défaut année courante
					_webSession.PeriodType = WebConstantes.CustomerSessions.Period.Type.currentYear;
					_webSession.PeriodLength = 1;
					// Cas où l'année de chargement est inférieur à l'année en cours
					if (DateTime.Now.Year > _webSession.DownLoadDate) {
						_webSession.PeriodBeginningDate = downloadDate.ToString("yyyy01");
						_webSession.PeriodEndDate = downloadDate.ToString("yyyyMM");
					}
					else {
						_webSession.PeriodBeginningDate = DateTime.Now.ToString("yyyy01");
						_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
					}

					//Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
					//du dernier mois dispo en BDD
					//traitement de la notion de fréquence
					absolutEndPeriod = WebFunctions.Dates.CheckPeriodValidity(_webSession, _webSession.PeriodEndDate);

					if ((int.Parse(absolutEndPeriod) < int.Parse(_webSession.PeriodBeginningDate)) || (absolutEndPeriod.Substring(4, 2).Equals("00"))) {
						throw (new TNS.AdExpress.Web.Exceptions.NoDataException());
					}

					_webSession.PeriodEndDate = absolutEndPeriod;
					_webSession.DetailPeriod = CstPeriodDetail.monthly;

					//Activation de l'option etude comparative 
					_webSession.ComparativeStudy = true;
					
				}
				catch (TNS.AdExpress.Web.Exceptions.NoDataException) {
					
					//Sinon choix par défaut année précédente
					_webSession.PeriodType = WebConstantes.CustomerSessions.Period.Type.previousYear;
					_webSession.PeriodLength = 1;

					// Cas où l'année de chargement est inférieur à l'année en cours
					if (DateTime.Now.Year > _webSession.DownLoadDate) {
						_webSession.PeriodBeginningDate = downloadDate.AddYears(-1).ToString("yyyy01");
						_webSession.PeriodEndDate = downloadDate.AddYears(-1).ToString("yyyy12");
					}
					else {
						_webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
						_webSession.PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy12");
					}
					_webSession.DetailPeriod = CstPeriodDetail.monthly;					
					_webSession.ComparativeStudy = true;										
				}
				catch (System.Exception exc) {
					if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
						this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
					}
				}
			}

			return null;
		}
		#endregion
	}
}
