using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

using System.Windows.Forms;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Controls.Results;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using SessionCst = TNS.AdExpress.Constantes.Web.CustomerSessions;
using CustomerCst = TNS.AdExpress.Constantes.Customer.Right;
using CstDB = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using ProductList=TNS.AdExpress.Web.DataAccess.Selections.Products.ProductListDataAccess;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Controls.Headers{
	/// <summary>
	/// Composant affichant le titre  et le descriptif de la page
	/// </summary>
	[ToolboxData("<{0}:ResultsOptionsWebControl runat=server></{0}:ResultsOptionsWebControl>")]
	public class ResultsOptionsWebControl : System.Web.UI.WebControls.WebControl {

		#region Variables
		/// <summary>
		/// Force le composant à montrer les niveaux de détails par accroches
		/// </summary>
		protected bool _forceMediaDetailForSlogan=false;
		/// <summary>
		/// List des unités sélectionnable
		/// </summary>
		protected DropDownList list;
		/// <summary>
		/// List des produits selectionnée
		/// </summary>
		public DropDownList products;
		/// <summary>
		/// List des unités APPM sélectionnable
		/// </summary>
		protected DropDownList listUnitAppm;
		/// <summary>
		/// List des pages de résultats disponibles pour le module courant
		/// </summary>
		public DropDownList resultsPages;
		/// <summary>
		/// Checkbox pour pdm (pourcentage quelconque et PDM pour tout ce qui est concurrentielle)
		/// </summary>
		protected System.Web.UI.WebControls.CheckBox percentageCheckBox;
		/// <summary>
		/// Choix du niveau de détail produit
		/// </summary>
		protected DropDownList productDetail;
		/// <summary>
		/// Choix du niveau de détail media
		/// </summary>
		public DropDownList mediaDetail;
		/// <summary>
		/// Liste des encarts  sélectionnable 
		/// </summary>
		public DropDownList listInsert;
		/// <summary>
		/// Choix du type de tableau afficher dans les recap
		/// </summary>
		protected ImageDropDownListWebControl tblChoice;
		/// <summary>
		/// Checkbox dédiée à la PDM 
		/// </summary>
		protected System.Web.UI.WebControls.CheckBox PdmCheckBox;
		/// <summary>
		/// Checkbox dédiée à la PDV 
		/// </summary>
		protected System.Web.UI.WebControls.CheckBox PdvCheckBox;
		/// <summary>
		/// Checkbox dédiée à l'affichage de l'option évolution 
		/// </summary>
		protected System.Web.UI.WebControls.CheckBox EvolutionCheckBox;
		/// <summary>
		/// Checkbox dédiée à l'affichage de l'option évolution 
		/// </summary>
		protected System.Web.UI.WebControls.CheckBox PersonalizedElementsCheckBox;
		
		/// <summary>
		/// Contrôle Choix du type de résultat sous forme graphique
		/// </summary>
		protected System.Web.UI.WebControls.RadioButton graphRadioButton;
		/// <summary>
		/// Contrôle Choix du type de résultat sous forme de tableau
		/// </summary>
		protected System.Web.UI.WebControls.RadioButton tableRadioButton;

		/// <summary>
		/// Contrôle Choix du type de pourcentage (horizontal ou vertical)
		/// </summary>
		protected System.Web.UI.WebControls.DropDownList _percentageTypeDropDownList;		
		
		
		/// <summary>
		/// Initiailisation des éléments de référence
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.InitializeProductWebControl _initializeProductWebControl;
		
		/// <summary>
		/// Titre du graphique
		/// </summary>
		protected string _chartTitle="";
		/// <summary>
		/// Titre du tableau
		/// </summary>
		protected string _tableTitle="";
		
		
		/// <summary>
		/// Session du client (utile pour la langue)
		/// </summary>
		protected WebSession customerWebSession = null;
		/// <summary>Session du client</summary>
		public WebSession CustomerWebSession{
			get{return customerWebSession;}
			set{customerWebSession=value;}
		}
		#endregion

		#region Propriétés
		/// <summary>
		/// CssClass générale 
		/// </summary>
		[Bindable(true),DefaultValue("txtNoir11Bold"),
		Description("Option choix de l'unité")]
		protected string cssClass = "txtNoir11Bold";
		/// <summary>CSS</summary>
		public string CommonCssClass{
			get{return cssClass;}
			set{cssClass=value;}
		}

		/// <summary>
		/// Option unité
		/// </summary>
		[Bindable(true),
		Description("Option choix de l'unité")]
		protected bool unitOption = true;
		/// <summary>Option d'unité</summary>
		public bool UnitOption{
			get{return unitOption;}
			set{unitOption=value;}
		}

		
		/// <summary>
		/// Option unité Appm
		/// </summary>		
		[Bindable(true),
		Description("Option choix de l'unité Appm")]
		protected bool unitOptionAppm = false;
		/// <summary>Option unité Appm</summary>
		public bool UnitOptionAppm{
			get{return unitOptionAppm;}
			set{unitOptionAppm=value;}
		}

		/// <summary>
		/// Option products
		/// </summary>	
		[Bindable(true),
		Description("Option list of products")]
		protected bool productsOption = false;
		/// <summary>list products Appm</summary>
		public bool ProductsOption
		{
			get{return productsOption;}
			set{productsOption=value;}
		}

		/// <summary>
		/// Option encart
		/// </summary>
		[Bindable(true),Description("Option choix d'un encart")]		
		protected bool insertOption = false;
		/// <summary>
		/// Propriété encart
		/// </summary>
		public bool InsertOption{
			get{return insertOption;}
			set{insertOption=value;}
		}

		/// <summary>
		/// Option PDM
		/// </summary>
		[Bindable(true),
		Description("Option visualiser la PDM")]
		protected bool pdmOption = false;
		/// <summary></summary>
		public bool PdmOption{
			get{return pdmOption;}
			set{pdmOption=value;}
		}
		
		/// <summary>
		/// Option PDV
		/// </summary>
		[Bindable(true),
		Description("Option visualiser la PDV")]
		protected bool pdvOption = false;
		/// <summary></summary>
		public bool PdvOption{
			get{return pdvOption;}
			set{pdvOption=value;}
		}

		/// <summary>
		/// Option Evolution
		/// </summary>
		[Bindable(true),
		Description("Option visualiser une évolution")]
		protected bool evolutionOption = false;
		/// <summary></summary>
		public bool EvolutionOption{
			get{return evolutionOption;}
			set{evolutionOption=value;}
		}

		/// <summary>
		/// Option de visualisation des éléments concurrents  et references seulement
		/// </summary>
		[Bindable(true),
		Description("Option : Visualiser uniquement les éléments de références et concurrents")]
		protected bool personalizedElementsOption = false;
		/// <summary></summary>
		public bool PersonalizedElementsOption{
			get{return personalizedElementsOption;}
			set{personalizedElementsOption=value;}
		}

	
		/// <summary>Option pourcentage</summary>
		public  System.Web.UI.WebControls.CheckBox PercentageCheckBox{
			get{return percentageCheckBox;}
			set{percentageCheckBox=value;}
		}


		/// <summary>
		/// Option niveau de détail produit
		/// </summary>
		[Bindable(true),DefaultValue(false),
		Description("Option choix du niveau de détail produit")]
		protected bool productDetailOption = false;
		/// <summary></summary>
		public bool ProductDetailOption{
			get{return productDetailOption;}
			set{productDetailOption=value;}
		}		

		/// <summary>
		/// Option niveau de détail media
		/// </summary>
		[Bindable(true),DefaultValue(false),
		Description("Option choix niveau de détail media")]
		protected bool mediaDetailOption = false;
		/// <summary></summary>
		public bool MediaDetailOption{
			get{return mediaDetailOption;}
			set{mediaDetailOption=value;}
		}

		/// <summary>
		/// Option choix de tableau préformaté
		/// </summary>
		[Bindable(true),DefaultValue(false),
		Description("Option choix de tableau préformaté")]
		protected bool tblChoiceOption = false;
		/// <summary></summary>
		public bool PreformatedTableOption{
			get{return tblChoiceOption;}
			set{tblChoiceOption=value;}
		}

		#region Propriétés de TblChoice
		/// <summary>
		/// hauteur de l'image
		/// </summary>
		[Bindable(true),Category("Appearance")]
		private double imageHeight = 15.0 ;
		/// <summary></summary>
		public double ImageHeight{
			get{return imageHeight;}
			set{imageHeight = value;}
		}

		/// <summary>
		/// largeur de l'image
		/// </summary>
		[Bindable(true),Category("Appearance")]
		private double imageWidth = 15.0 ;
		/// <summary></summary>
		public double ImageWidth{
			get{return imageWidth;}
			set{imageWidth = value;}
		}

		/// <summary>
		/// largeur des bordures
		/// </summary>
		[Bindable(true),Category("Appearance"), DefaultValue(1.0)]
		private  double borderWidth=1.0;
		/// <summary></summary>
		public new double BorderWidth{
			get{return borderWidth;}
			set{borderWidth = Math.Max(0, value);}
		}

		/// <summary>
		/// option afficher images
		/// </summary>
		[Bindable(true),DefaultValue(true)]
		protected bool pictShow=true;
		/// <summary></summary>
		public bool ShowPictures{
			get{return pictShow;}
			set{pictShow = value;}
		}
		/// <summary>
		///feuille de style 
		/// </summary>
		[Bindable(true),DefaultValue("ddlOut")]
		protected string outCssClass="ddlOut";
		/// <summary></summary>
		public string OutCssClass{
			get{return outCssClass;}
			set{outCssClass = value;}
		}

		/// <summary>
		///feuille de style pour l'élément en roll-over 
		/// </summary>
		[Bindable(true),DefaultValue("ddlOver")]
		protected string overCssClass="ddlOver";
		/// <summary></summary>
		public string OverCssClass{
			get{return overCssClass;}
			set{overCssClass = value;}
		}

		/// <summary>
		/// liste des éléments
		/// </summary>
		[Bindable(true),DefaultValue("")]
		private string texts = "";
		/// <summary></summary>
		public string List{
			get{return texts;}
			set{texts = value;}
		}

		/// <summary>
		/// liste des unités APPM
		/// </summary>
		[Bindable(true),DefaultValue("")]
		private string textsAppm= "";
		/// <summary> liste des unités APPM</summary>
		public string ListUnitAppm{
			get{return textsAppm;}
			set{textsAppm = value;}
		}


		/// <summary>
		///noms des images 
		/// </summary>
		[Bindable(true),DefaultValue("")]
		private string images = "";
		/// <summary></summary>
		public string Images{
			get{return images;}
			set{images = value;}
		}

		/// <summary>
		/// index liste
		/// </summary>
		[Bindable(true),DefaultValue(0)]
		private int index;
		/// <summary></summary>
		public int ListIndex{
			get{return index;}
			set{index = Math.Min(value, texts.Split('|').Length - 1);}
		}
		#endregion

		/// <summary>
		/// Autopostback Vrai par défaut
		/// </summary>
		[Bindable(true),
		Description("autoPostBack")]
		protected bool autoPostBackOption = true;
		/// <summary></summary>
		public bool AutoPostBackOption{
			get{return autoPostBackOption;}
			set{autoPostBackOption=value;}
		}

		/// <summary>
		///Option pdm
		/// </summary>
		[Bindable(false),
		Description("pdm")]
		protected bool percentage = false;
		/// <summary>Affiche en %</summary>
		public bool Percentage{
			get{return percentage;}
			set{percentage=value;}
		}


		/// <summary>
		/// Option resultat
		/// </summary>
		[Bindable(true),
		Description("Option type de résultat")]
		protected bool resultOption = true;
		/// <summary>Type de résultat</summary>
		public bool ResultOption{
			get{return resultOption;}
			set{resultOption=value;}
		}

		/// <summary>
		/// Option resultat
		/// </summary>
		[Bindable(true),
		Description("Option type de résultat")]
		protected bool forceMediaDetailForMediaPlan = false;
		/// <summary></summary>
		public bool ForceMediaDetailForMediaPlan{
			get{return forceMediaDetailForMediaPlan;}
			set{forceMediaDetailForMediaPlan=value;}
		}

		/// <summary>
		/// Force l'accès aux accroches dans le niveau de détail support
		/// </summary>
		[Bindable(true),
		Description("Option type de résultat")]
		public bool ForceMediaDetailForSlogan{
			get{return _forceMediaDetailForSlogan;}
			set{_forceMediaDetailForSlogan=value;}
		}

		/// <summary>
		/// Format de résultat
		/// </summary>		
		[Bindable(true),
		Description("Option format présentation résultat")]
		protected bool _resultFormat=false;
		/// <summary>
		///Format de résultat 
		/// </summary>
		public bool ResultFormat{
			get{return _resultFormat;}
			set{_resultFormat=value;}
		}		
		
		/// <summary>
		///Titre graphique de résultat 
		/// </summary>
		public string ChartTitle{
			get{return _chartTitle;}
			set{_chartTitle=value;}
		}

		/// <summary>
		///Titre tableau de résultat 
		/// </summary>
		public string TableTitle{
			get{return _tableTitle;}
			set{_tableTitle=value;}
		}

		/// <summary>
		///Contrôle Choix du type de résultat sous forme graphique
		/// </summary>
		public System.Web.UI.WebControls.RadioButton GraphRadioButton{
			get{return graphRadioButton;}
			set{graphRadioButton=value;}
		}

		/// <summary>
		///Contrôle Choix du type de résultat sous forme tableau
		/// </summary>
		public System.Web.UI.WebControls.RadioButton TableRadioButton{
			get{return tableRadioButton;}
			set{tableRadioButton=value;}
		}
		
		/// <summary>
		/// Initialisation des éléments de références
		/// </summary>
		public TNS.AdExpress.Web.Controls.Headers.InitializeProductWebControl InitializeProductWebControl{
			get{return _initializeProductWebControl;}
			set{_initializeProductWebControl=value;}
		}

		/// <summary>
		/// Option initialisation des annonceurs de concurrents
		/// </summary>
		[Bindable(true),
		Description("Option initialisation des annoneceurs de concurrents")]
		protected bool _inializeAdvertiserOption = false;
		/// <summary>Option initialisation des annonceurs de concurrents</summary>
		public bool InializeAdVertiserOption{
			get{return _inializeAdvertiserOption;}
			set{_inializeAdvertiserOption = value;}
		}

		/// <summary>
		/// Option type de pourcentage (horizontal ou vertical)
		/// </summary>
		[Bindable(true),
		Description("Option type de pourcentage (horizontal ou vertical)")]
		protected bool _percentageTypeOption = false;
		/// <summary>Option type de pourcentage (horizontal ou vertical)</summary>
		public bool PercentageTypeOption{
			get{return _percentageTypeOption;}
			set{_percentageTypeOption = value;}
		}

		/// <summary>Contrôle Choix du type de pourcentage (horizontal ou vertical)</summary>
		[Bindable(true),
		Description("Contrôle choix  type de pourcentage (horizontal ou vertical)")]
		public System.Web.UI.WebControls.DropDownList PercentageTypeDropDownList{
			get{return _percentageTypeDropDownList;}
			set{_percentageTypeDropDownList = value;}
		}

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public ResultsOptionsWebControl():base(){
			this.EnableViewState = true;
			this.PreRender += new EventHandler(Custom_PreRender);
		}

		#endregion

		#region Evenements

		#region Init
        /// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {
			
			

			//Option Initialisation des éléments de références
			_initializeProductWebControl = new TNS.AdExpress.Web.Controls.Headers.InitializeProductWebControl();			
			_initializeProductWebControl.CustomerWebSession = customerWebSession;
			_initializeProductWebControl.AutoPostBackOption = this.autoPostBackOption;
			_initializeProductWebControl.EnableViewState = true;
			_initializeProductWebControl.initializeAdvertiserCheckBox.EnableViewState =true;
			_initializeProductWebControl.InitializeAdvertiser = InializeAdVertiserOption;			
			_initializeProductWebControl.ID=this.ID+"_initializeAdvertiser";
			Controls.Add(_initializeProductWebControl);


			if (Page.IsPostBack){
				if (unitOption){
					try{
						SessionCst.Unit unitSelected=(SessionCst.Unit) Int64.Parse(Page.Request.Form.GetValues("_units")[0]);
						if(customerWebSession.Unit!=unitSelected)customerWebSession.Unit = unitSelected;
					}
					catch(SystemException){
					}
				}
				
				if (insertOption){
					if(Page.Request.Form.GetValues("_inserts")!=null){
						customerWebSession.Insert = (SessionCst.Insert) Int64.Parse(Page.Request.Form.GetValues("_inserts")[0]);					
					}
				}
				if(resultOption){
					Int64 tabSelected=Int64.Parse(Page.Request.Form.GetValues("_resultsPages")[0]);
					if(customerWebSession.CurrentTab!=tabSelected)
						customerWebSession.CurrentTab=tabSelected;
				}
				
				if(percentage){
					try{
						if(Page.Request.Form.GetValues("_percentage")[0]!=null)customerWebSession.Percentage=true;
					}catch(System.Exception)
					{customerWebSession.Percentage=false;}						
				}
				if(pdmOption){
					try{
						if(Page.Request.Form.GetValues(this.ID+"_pdm")[0]!=null)customerWebSession.PDM=true;
					}catch(System.Exception) {
					 customerWebSession.PDM=false;}						
				}				
				if(pdvOption){
					try{
						if(Page.Request.Form.GetValues(this.ID+"_pdv")[0]!=null)customerWebSession.PDV=true;
					}catch(System.Exception) {
					 customerWebSession.PDV=false;}						
				}
				if(evolutionOption){
					try{
						if(Page.Request.Form.GetValues(this.ID+"_evol")[0]!=null)customerWebSession.Evolution=true;
					}catch(System.Exception) {
					 customerWebSession.Evolution=false;}						
				}
				if(personalizedElementsOption){
					try{
						if(Page.Request.Form.GetValues(this.ID+"_perso")[0]!=null && (Page.Request.Form.GetValues("_initializeAdvertiser")==null)
							)customerWebSession.PersonalizedElementsOnly=true;
						else{
							customerWebSession.PersonalizedElementsOnly=false;
							if(PersonalizedElementsCheckBox!=null)PersonalizedElementsCheckBox.Checked =false;
						}
					}catch(System.Exception) {
						customerWebSession.PersonalizedElementsOnly=false;}						
				}
				
				if(tblChoiceOption){
					customerWebSession.PreformatedTable = (SessionCst.PreformatedDetails.PreformatedTables) Int64.Parse(Page.Request.Form.GetValues("DDL"+this.ID)[0]);					
				}
				if (productDetailOption)
					try{
						SessionCst.PreformatedDetails.PreformatedProductDetails detailSelected=(SessionCst.PreformatedDetails.PreformatedProductDetails) int.Parse(Page.Request.Form.GetValues("productDetail_"+this.ID)[0]);
						if(customerWebSession.PreformatedProductDetail != detailSelected)
							customerWebSession.PreformatedProductDetail =detailSelected;
					}
					catch(System.Exception){}
				if (mediaDetailOption)
					try{
						customerWebSession.PreformatedMediaDetail = (SessionCst.PreformatedDetails.PreformatedMediaDetails) int.Parse(Page.Request.Form.GetValues("mediaDetail_"+this.ID)[0]);
					}
					catch(System.Exception){}

				//Sauvegarde du type d'alignement des résultats en pourcentage lorsque la page est publiée
				if (_percentageTypeOption){
					try{
						//int productID=Convert.ToInt32(products.SelectedItem.Value);
						int percentageTypeID=Convert.ToInt32(Page.Request.Form.GetValues("_percentageTypePercentageDropDownList")[0]);	
						if(customerWebSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units
							&& (WebConstantes.Percentage.Alignment)percentageTypeID==WebConstantes.Percentage.Alignment.horizontal){
							customerWebSession.PercentageAlignment = WebConstantes.Percentage.Alignment.none;	

						}
						else customerWebSession.PercentageAlignment = (WebConstantes.Percentage.Alignment)percentageTypeID;		
						
					}
					catch(SystemException){
					}
				}
			}

			#region Option format du résultat (graphique ou tableau)
			
				graphRadioButton = new System.Web.UI.WebControls.RadioButton();
				graphRadioButton.EnableViewState=true;
				graphRadioButton.ID="graphRadioButton";
				graphRadioButton.GroupName="graphTableRadioButton";																	
				Controls.Add(graphRadioButton);

				tableRadioButton = new System.Web.UI.WebControls.RadioButton();
				tableRadioButton.EnableViewState=true;
				tableRadioButton.ID="tableRadioButton";
				tableRadioButton.GroupName="graphTableRadioButton";																	
				Controls.Add(tableRadioButton);
							
			
			#endregion

		
			
			
			
			
			base.OnInit (e);
		}

		#endregion
 
		#region Load
		/// <summary>
		/// launched when the control is loaded
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnLoad(EventArgs e){
			#region initializing controls

			AdExpressUniverse adExpressUniverse = null;
			NomenclatureElementsGroup nomenclatureElementsGroup = null;
			Dictionary<int, AdExpressUniverse> adExpressUniverseDictionary = null;

			#region products APPM
			products = new DropDownList();
			products.EnableViewState=true;
			products.ID = "_products";
			Controls.Add(products);
			#endregion
						
			#endregion
			

			#region loading controls for the first time


			//Création de la liste des produits appm
			if(productsOption){
				if(!Page.IsPostBack||products.Items.Count<=0){
					
					products.CssClass =  cssClass;
					products.AutoPostBack = autoPostBackOption;
					DataTable productsTable=TNS.AdExpress.Web.DataAccess.Selections.Products.ProductListDataAccess.getProductList(customerWebSession).Tables[0];
					if(productsTable.Rows.Count>1)
						products.Items.Add(new ListItem("----------------------------------------","0"));
					if(productsTable.Rows.Count==1){
						try{
							//int productID=Convert.ToInt32(products.SelectedItem.Value);
							int productID=Convert.ToInt32(productsTable.Rows[0]["id_product"]);
							if (productID != 0) {
								adExpressUniverse = new AdExpressUniverse(Dimension.product);
								nomenclatureElementsGroup = new NomenclatureElementsGroup(0, AccessType.includes);
								nomenclatureElementsGroup.AddItem(TNSClassificationLevels.PRODUCT, productID);
								adExpressUniverse.AddGroup(0, nomenclatureElementsGroup);
								adExpressUniverseDictionary = new Dictionary<int, AdExpressUniverse>();
								adExpressUniverseDictionary.Add(0, adExpressUniverse);
								customerWebSession.SecondaryProductUniverses = adExpressUniverseDictionary;
							}
							else customerWebSession.SecondaryProductUniverses = new Dictionary<int, AdExpressUniverse>();
						}
						catch(SystemException){}
					}

					if(productsTable.Rows.Count>0) {
						
						foreach(DataRow dr in productsTable.Rows) {
							products.Items.Add(new ListItem(dr["product"].ToString(),dr["id_product"].ToString()));
						}
					}
					
				}
				try {					
					//string productTag=customerWebSession.GetSelection(customerWebSession.CurrentUniversProduct,CustomerCst.type.productAccess);
					string productTag = customerWebSession.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.PRODUCT);
					products.Items.FindByValue(productTag).Selected=true;
					
				}
				catch(System.Exception) {
					try{
						products.Items.FindByValue("0").Selected = true;				
					}
					catch(System.Exception){
					}
				}
				
			}	
			#endregion

			#region loading univers and websession
			
			if (Page.IsPostBack) {
				//saving the selected products in the current univers product when the page is posted back.
				if (productsOption){
					try{
						int productID=Convert.ToInt32(Page.Request.Form.GetValues("_products")[0]);
						if (productID != 0) {
							adExpressUniverse = new AdExpressUniverse(Dimension.product);
							nomenclatureElementsGroup = new NomenclatureElementsGroup(0, AccessType.includes);
							nomenclatureElementsGroup.AddItem(TNSClassificationLevels.PRODUCT, productID);
							adExpressUniverse.AddGroup(0, nomenclatureElementsGroup);
							adExpressUniverseDictionary = new Dictionary<int, AdExpressUniverse>();
							adExpressUniverseDictionary.Add(0, adExpressUniverse);
							customerWebSession.SecondaryProductUniverses = adExpressUniverseDictionary;
						}
						else customerWebSession.SecondaryProductUniverses = new Dictionary<int, AdExpressUniverse>();

					}
					catch(SystemException){
					}
				}
				if (unitOptionAppm){
					try{
						customerWebSession.Unit = (SessionCst.Unit) Int64.Parse(Page.Request.Form.GetValues("_unitsAppm")[0]);
					}
					catch(SystemException){
					}
				}
			}

			base.OnLoad (e);

			#endregion

		
		}

		#endregion

		#region Custom PreRender

		/// <summary>
		///custom prerender 
		/// </summary>
		/// <param name="sender">object qui lance l'évènement</param>
		/// <param name="e">arguments</param>
		private void Custom_PreRender(object sender, System.EventArgs e)
		{

            string themeName = WebApplicationParameters.Themes[customerWebSession.SiteLanguage].Name;
			
			#region Unité
			if(unitOption)
			{
				//Création de la liste des unités
				list = new DropDownList();
				list.ID = "_units";
				list.CssClass =  cssClass;
				list.AutoPostBack = autoPostBackOption;
				if (!percentage)list.Width = new System.Web.UI.WebControls.Unit("100%");
				ArrayList units;
				if(customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS){
					units = WebFunctions.Units.getUnitsFromVehicleSelection(ClassificationCst.DB.Vehicles.names.tv.GetHashCode().ToString());
				}
				else units = WebFunctions.Units.getUnitsFromVehicleSelection(customerWebSession.GetSelection(customerWebSession.SelectionUniversMedia,CustomerCst.type.vehicleAccess));
				for(int i = 0; i<units.Count; i++){
                    if ((SessionCst.Unit)units[i] != SessionCst.Unit.volume || customerWebSession.CustomerLogin.GetFlag(CstDB.Flags.ID_VOLUME_MARKETING_DIRECT) != null)
                        list.Items.Add(new ListItem(GestionWeb.GetWebWord((int)SessionCst.UnitsTraductionCodes[(SessionCst.Unit)units[i]], customerWebSession.SiteLanguage), ((int)(SessionCst.Unit)units[i]).ToString()));
                    else if(customerWebSession.Unit == SessionCst.Unit.volume)
                        customerWebSession.Unit = SessionCst.Unit.euro;
				}
				list.Items.FindByValue(((int)customerWebSession.Unit).ToString()).Selected=true;

				Controls.Add(list);
			}
					
			#endregion

			#region Unité Appm
			 if(unitOptionAppm){
				//Création de la liste des unités appm
				listUnitAppm = new DropDownList();
				listUnitAppm.ID = "_unitsAppm";
				listUnitAppm.CssClass =  cssClass;
				listUnitAppm.AutoPostBack = autoPostBackOption;
				ArrayList unitsAppm = WebFunctions.Units.getUnitsFromAppmPress();
				for(int i = 0; i<unitsAppm.Count; i++){
				
					listUnitAppm.Items.Add(new ListItem(GestionWeb.GetWebWord((int)SessionCst.UnitsTraductionCodes[(SessionCst.Unit)unitsAppm[i]],customerWebSession.SiteLanguage) ,((int)(SessionCst.Unit)unitsAppm[i]).ToString()));
				}
				listUnitAppm.Items.FindByValue(((int)customerWebSession.Unit).ToString()).Selected=true;

				Controls.Add(listUnitAppm);
			}
			#endregion
		

			#region Encart
			if(insertOption)
			{
				//Création de la liste des encarts
				listInsert = new DropDownList();
				listInsert.ID = "_inserts";
				listInsert.CssClass = cssClass;
				listInsert.AutoPostBack = autoPostBackOption;
				ArrayList inserts = WebFunctions.Units.getInserts();
				for(int j=0;j<inserts.Count;j++){
					listInsert.Items.Add(new ListItem(GestionWeb.GetWebWord((int)SessionCst.InsertsTraductionCodes[(SessionCst.Insert)inserts[j]],customerWebSession.SiteLanguage),((int)(SessionCst.Insert)inserts[j]).ToString()));
				}
				listInsert.Items.FindByValue(((int)customerWebSession.Insert).ToString()).Selected=true;
				Controls.Add(listInsert);
			}
			#endregion

			if (resultOption){
				//Création des options concernant le choix du resultat
				resultsPages = new DropDownList(); 
				resultsPages.ID = "_resultsPages";
				resultsPages.CssClass =  cssClass;
				resultsPages.AutoPostBack = autoPostBackOption;
				//customerWebSession.CustomerLogin.ModuleList();
				ArrayList resultPages=((Module)customerWebSession.CustomerLogin.GetModule(customerWebSession.CurrentModule)).GetResultPageInformationsList();
				foreach(ResultPageInformation current in resultPages){					
					if(!CanShowResult(customerWebSession,current))continue;

					resultsPages.Items.Add(new ListItem(GestionWeb.GetWebWord((int)current.IdWebText,customerWebSession.SiteLanguage),current.Id.ToString()));
				}

				resultsPages.Items.FindByValue(customerWebSession.CurrentTab.ToString()).Selected=true;
				Controls.Add(resultsPages);
			}

			if(percentage){
				percentageCheckBox=new System.Web.UI.WebControls.CheckBox();
				percentageCheckBox.ID="_percentage";
				percentageCheckBox.CssClass=cssClass;
				percentageCheckBox.AutoPostBack=autoPostBackOption;
				percentageCheckBox.Text=GestionWeb.GetWebWord(806,customerWebSession.SiteLanguage);
				percentageCheckBox.Checked=customerWebSession.Percentage;
				Controls.Add(percentageCheckBox);
			}
			

			if(pdmOption){
				PdmCheckBox=new System.Web.UI.WebControls.CheckBox();
				PdmCheckBox.ID=this.ID + "_pdm";
				PdmCheckBox.ToolTip = GestionWeb.GetWebWord(1179,customerWebSession.SiteLanguage);
				PdmCheckBox.CssClass=cssClass;
				PdmCheckBox.AutoPostBack=autoPostBackOption;
				PdmCheckBox.Text=GestionWeb.GetWebWord(806,customerWebSession.SiteLanguage);
				PdmCheckBox.Checked=customerWebSession.PDM;
				Controls.Add(PdmCheckBox);
			}
			if(pdvOption){
				PdvCheckBox=new System.Web.UI.WebControls.CheckBox();
				PdvCheckBox.ID=this.ID + "_pdv";
				PdvCheckBox.ToolTip = GestionWeb.GetWebWord(1180,customerWebSession.SiteLanguage);
				PdvCheckBox.CssClass=cssClass;
				PdvCheckBox.AutoPostBack=autoPostBackOption;
				PdvCheckBox.Text=GestionWeb.GetWebWord(1166,customerWebSession.SiteLanguage);
				PdvCheckBox.Checked=customerWebSession.PDV;
				Controls.Add(PdvCheckBox);
			}
			if(evolutionOption){
				EvolutionCheckBox=new System.Web.UI.WebControls.CheckBox();
				EvolutionCheckBox.ID=this.ID + "_evol";
				EvolutionCheckBox.CssClass=cssClass;
				EvolutionCheckBox.ToolTip = GestionWeb.GetWebWord(1178,customerWebSession.SiteLanguage);
				EvolutionCheckBox.AutoPostBack=autoPostBackOption;
				EvolutionCheckBox.Text=GestionWeb.GetWebWord(1168,customerWebSession.SiteLanguage);
				EvolutionCheckBox.Checked=customerWebSession.Evolution;
				Controls.Add(EvolutionCheckBox);
			}

			if(personalizedElementsOption){
				PersonalizedElementsCheckBox=new System.Web.UI.WebControls.CheckBox();
				PersonalizedElementsCheckBox.ID=this.ID + "_perso";
				PersonalizedElementsCheckBox.ToolTip = GestionWeb.GetWebWord(1181,customerWebSession.SiteLanguage);
				PersonalizedElementsCheckBox.CssClass=cssClass;
				PersonalizedElementsCheckBox.AutoPostBack=autoPostBackOption;
				PersonalizedElementsCheckBox.Text=GestionWeb.GetWebWord(1174,customerWebSession.SiteLanguage);
				PersonalizedElementsCheckBox.Checked=customerWebSession.PersonalizedElementsOnly;
				Controls.Add(PersonalizedElementsCheckBox);
			}

			

			if(mediaDetailOption){
				mediaDetail = new DropDownList();
				mediaDetail.Width = new System.Web.UI.WebControls.Unit("100%");
				// Pour les Plan media
				if(forceMediaDetailForMediaPlan || customerWebSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA || customerWebSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PLAN_MEDIA){
					mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1292, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString()));
					mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1142, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategory.GetHashCode().ToString()));
					mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1143, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia.GetHashCode().ToString()));
					mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1544, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMedia.GetHashCode().ToString()));
					mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1542, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter.GetHashCode().ToString()));
					mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1543, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia.GetHashCode().ToString()));

					mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1860, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller.GetHashCode().ToString()));
					mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1861, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia.GetHashCode().ToString()));
					mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1862, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia.GetHashCode().ToString()));
					mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1863, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia.GetHashCode().ToString()));
					//mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1864, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter.GetHashCode().ToString()));
					//mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1865, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia.GetHashCode().ToString()));
                    
					#region Accroches
					// Gestion des droits sur les accroches
					if(customerWebSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG)!=null &&
						(_forceMediaDetailForSlogan ||
						// Sélection par produit ou marque
						(customerWebSession.GetSelection(customerWebSession.SelectionUniversAdvertiser,TNS.AdExpress.Constantes.Customer.Right.type.productAccess).Length>0 ||
						customerWebSession.GetSelection(customerWebSession.SelectionUniversAdvertiser,TNS.AdExpress.Constantes.Customer.Right.type.brandAccess).Length>0) &&
						// Pas de famille, classe, groupe, groupe d'annonceur, annonceur
						customerWebSession.GetSelection(customerWebSession.SelectionUniversAdvertiser,TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length==0 &&
						customerWebSession.GetSelection(customerWebSession.SelectionUniversAdvertiser,TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length==0 &&
						customerWebSession.GetSelection(customerWebSession.SelectionUniversAdvertiser,TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length==0 &&
						customerWebSession.GetSelection(customerWebSession.SelectionUniversAdvertiser,TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length==0 &&
						customerWebSession.GetSelection(customerWebSession.SelectionUniversAdvertiser,TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length==0) &&
						// Niveau de détail par jour
						customerWebSession.DetailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.dayly
						){
						// On augmente la taille pour les accroches
						mediaDetail.Width=200;
						mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1866, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan.GetHashCode().ToString()));
						mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1867, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan.GetHashCode().ToString()));
						mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1868, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan.GetHashCode().ToString()));
						mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1869, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan.GetHashCode().ToString()));
						//mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1870, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan.GetHashCode().ToString()));
						//mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1871, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan.GetHashCode().ToString()));
						//mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1872, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan.GetHashCode().ToString()));
						mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1873, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganMedia.GetHashCode().ToString()));
						mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1874, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia.GetHashCode().ToString()));
						//mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1875, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia.GetHashCode().ToString()));
						mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1876, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia.GetHashCode().ToString()));
						//mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1877, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia.GetHashCode().ToString()));
						mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1878, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia.GetHashCode().ToString()));
						//mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1879, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia.GetHashCode().ToString()));
						//mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1880, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia.GetHashCode().ToString()));
					}
					#endregion

				}
				else{
					switch((ClassificationCst.DB.Vehicles.names)((LevelInformation) customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID){
						case ClassificationCst.DB.Vehicles.names.tv:
						case ClassificationCst.DB.Vehicles.names.radio:
						case ClassificationCst.DB.Vehicles.names.outdoor:
						case ClassificationCst.DB.Vehicles.names.mediasTactics:
							mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1141, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString()));
							mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1142, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategory.GetHashCode().ToString()));
							if(customerWebSession.CurrentModule!=WebConstantes.Module.Name.INDICATEUR)mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1544, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMedia.GetHashCode().ToString()));
							mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1143, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia.GetHashCode().ToString()));
							break;
						case ClassificationCst.DB.Vehicles.names.press:
						case ClassificationCst.DB.Vehicles.names.internationalPress:
						case ClassificationCst.DB.Vehicles.names.internet:
							mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1141, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString()));
							mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1142, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategory.GetHashCode().ToString()));
							break;
						default:
							mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1141, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString()));
							mediaDetail.Enabled = false;
							break;
					}
				}
				mediaDetail.ID = "mediaDetail_"+this.ID;
				mediaDetail.AutoPostBack=autoPostBackOption;
				mediaDetail.CssClass = cssClass;
				try{
					mediaDetail.Items.FindByValue(customerWebSession.PreformatedMediaDetail.GetHashCode().ToString()).Selected = true;
				}
				catch(System.Exception){
					mediaDetail.Items[0].Selected=true;
				}

				Controls.Add(mediaDetail);
			}

			if(ProductDetailOption){
				productDetail = new DropDownList();
				productDetail.Width = new System.Web.UI.WebControls.Unit("100%");
				productDetail.ID = "productDetail_" + this.ID;
				productDetail.CssClass =  cssClass;
				productDetail.AutoPostBack = autoPostBackOption;
				productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1110, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedProductDetails.group.GetHashCode().ToString()));
				productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1144, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedProductDetails.groupSegment.GetHashCode().ToString()));
				//Rights verification for Brand
				if(customerWebSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE)!=null)
				{
					productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1111, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedProductDetails.groupBrand.GetHashCode().ToString()));
				}
				productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1112, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedProductDetails.groupProduct.GetHashCode().ToString()));
				productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1145, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedProductDetails.groupAdvertiser.GetHashCode().ToString()));
				//modifications for segmentAdvertiser,segmentProduct,SegmentBrand(3 new items added in the dropdownlist)
				productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1577, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser.GetHashCode().ToString()));
				productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1578, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedProductDetails.segmentProduct.GetHashCode().ToString()));
				if(customerWebSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE)!=null)
				{
					productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1579, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedProductDetails.segmentBrand.GetHashCode().ToString()));
				}
				productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1146, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedProductDetails.advertiser.GetHashCode().ToString()));
				if(customerWebSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE)!=null)
				{
					productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1147, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedProductDetails.advertiserBrand.GetHashCode().ToString()));
				}
				productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1148, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedProductDetails.advertiserProduct.GetHashCode().ToString()));
				if(customerWebSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE)!=null)
				{
					productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1149, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedProductDetails.brand.GetHashCode().ToString()));
				}
				productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(858, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedProductDetails.product.GetHashCode().ToString()));
				try{
					productDetail.Items.FindByValue(customerWebSession.PreformatedProductDetail.GetHashCode().ToString()).Selected = true;
				}
				catch(System.Exception){
					productDetail.SelectedIndex = 0;
				}
				Controls.Add(productDetail);
			}

			if(tblChoiceOption){
				tblChoice = new ImageDropDownListWebControl();
				tblChoice.BackColor = this.BackColor;
				tblChoice.BorderColor = this.BorderColor;
				tblChoice.BorderWidth = new System.Web.UI.WebControls.Unit(this.borderWidth);
				int sponsorshipListIndex = 24;
				if (this.List!="") tblChoice.List = this.List;
				else{
					if(customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS
						|| customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES)
						tblChoice.List = "&nbsp;|&nbsp;|&nbsp;";
					else
					tblChoice.List = "&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;";
				}
				if (this.images!="") tblChoice.Images = this.images;
				else{
					if(customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS
						|| customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES) {
						tblChoice.Images= "/App_Themes/"+themeName+"/Images/Culture/Tables/Parrainage_type1.gif" +
							"|/App_Themes/"+themeName+"/Images/Culture/Tables/Parrainage_type2.gif" +
							"|/App_Themes/"+themeName+"/Images/Culture/Tables/Parrainage_type3.gif" ;							
					}else{
						tblChoice.Images= "/App_Themes/"+themeName+"/Images/Culture/Tables/type1.gif" +
							"|/App_Themes/"+themeName+"/Images/Culture/Tables/type2.gif" +
							"|/App_Themes/"+themeName+"/Images/Culture/Tables/type3.gif" +
							"|/App_Themes/"+themeName+"/Images/Culture/Tables/type4.gif" +
							"|/App_Themes/"+themeName+"/Images/Culture/Tables/type10.gif" +
							"|/App_Themes/"+themeName+"/Images/Culture/Tables/type11.gif" +
							"|/App_Themes/"+themeName+"/Images/Culture/Tables/type5.gif" +
							"|/App_Themes/"+themeName+"/Images/Culture/Tables/type6.gif" +
							"|/App_Themes/"+themeName+"/Images/Culture/Tables/type7.gif" +
							"|/App_Themes/"+themeName+"/Images/Culture/Tables/type8.gif" +
							"|/App_Themes/"+themeName+"/Images/Culture/Tables/type9.gif";
					}
				}
				if(customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS
					|| customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES) 
				tblChoice.ListIndex = customerWebSession.PreformatedTable.GetHashCode()- sponsorshipListIndex;
				else tblChoice.ListIndex = customerWebSession.PreformatedTable.GetHashCode();
				tblChoice.ImageHeight = this.imageHeight;
				tblChoice.ImageWidth = this.imageWidth;
                tblChoice.ImageButtonArrow = "/App_Themes/" + themeName + "/Images/Common/button/bt_arrow_tall_top.gif";
				tblChoice.ID = "DDL"+this.ID;
				tblChoice.OutCssClass = this.outCssClass;
				tblChoice.OverCssClass = this.overCssClass;
				tblChoice.Width = new System.Web.UI.WebControls.Unit("121");
				tblChoice.ShowPictures = this.pictShow;
				Controls.Add(tblChoice);
							
			}

			//Contrôle choix type de pourcentage
			if(_percentageTypeOption){
					
				_percentageTypeDropDownList = new System.Web.UI.WebControls.DropDownList();									
				_percentageTypeDropDownList.ID="_percentageTypePercentageDropDownList";																			
			
//				if(!Page.IsPostBack||_percentageTypeDropDownList.Items.Count<=0){
					_percentageTypeDropDownList.CssClass =  cssClass;
					_percentageTypeDropDownList.AutoPostBack = autoPostBackOption;
					_percentageTypeDropDownList.Items.Add(new ListItem("----------------------",WebConstantes.Percentage.Alignment.none.GetHashCode().ToString()));					
					_percentageTypeDropDownList.Items.Add(new ListItem(GestionWeb.GetWebWord(2065,customerWebSession.SiteLanguage),WebConstantes.Percentage.Alignment.vertical.GetHashCode().ToString()));			
					if(customerWebSession.PreformatedTable != WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
						_percentageTypeDropDownList.Items.Add(new ListItem(GestionWeb.GetWebWord(2064,customerWebSession.SiteLanguage),WebConstantes.Percentage.Alignment.horizontal.GetHashCode().ToString()));			
//				}
				try {										
					_percentageTypeDropDownList.Items.FindByValue(customerWebSession.PercentageAlignment.GetHashCode().ToString()).Selected=true;
					
				}
				catch(System.Exception) {
					try{
						_percentageTypeDropDownList.Items.FindByValue(WebConstantes.Percentage.Alignment.none.GetHashCode().ToString()).Selected = true;				
					}
					catch(System.Exception){
					}
				}
				Controls.Add(_percentageTypeDropDownList);
			}
			
		}

		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {

            string themeName = WebApplicationParameters.Themes[customerWebSession.SiteLanguage].Name;

            output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" class=\"whiteBackGround\">");
			output.Write("\n<tr>");
			output.Write("\n<td>");
			//debut tableau titre
			output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			output.Write("\n<tr>");
            output.Write("\n<td class=\"headerLeft\" colSpan=\"4\"><IMG height=\"1\" src=\"/App_Themes/"+themeName+"/Images/Common/pixel.gif\"></td>");
			output.Write("\n</tr>");
			output.Write("\n<tr>");
			output.Write("\n<td style=\"HEIGHT: 14px\" vAlign=\"top\"><IMG height=\"12\" src=\"/App_Themes/"+themeName+"/Images/Common/block_fleche.gif\" width=\"12\"></td>");
            output.Write("\n<td style=\"HEIGHT: 14px\" width=\"1%\" class=\"blockBackGround\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\" width=\"13\"></td>");
            output.Write("\n<td class=\"txtNoir11Bold titleUppercase\" width=\"100%\">" + GestionWeb.GetWebWord(792, customerWebSession.SiteLanguage) + "</td>");
            output.Write("\n<td style=\"HEIGHT: 14px\" class=\"headerLeft\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/pixel.gif\" width=\"1\"></td>");
			output.Write("\n</tr>");
			output.Write("\n<tr>");
			output.Write("\n<td></td>");
			output.Write("\n<td class=\"headerLeft\" colSpan=\"3\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/images/Common/pixel.gif\"></td>");
			output.Write("\n</tr>");
			output.Write("\n</table>");
			//fin tableau titre
			output.Write("\n</td>");
			output.Write("\n</tr>");
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");

			
			//option unité
			if (unitOption){
				output.Write("\n<tr>");
				output.Write("\n<td title=\"" + GestionWeb.GetWebWord(1182,customerWebSession.SiteLanguage) + "\" class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(304,customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				list.RenderControl(output);
				
				if(percentage){
					output.Write("&nbsp;");
					percentageCheckBox.RenderControl(output);
					
				}

				
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}

				//option unité Appm
			if (unitOptionAppm){
				output.Write("\n<tr>");
				output.Write("\n<td title=\"" + GestionWeb.GetWebWord(1182,customerWebSession.SiteLanguage) + "\" class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(304,customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				listUnitAppm.RenderControl(output);
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
				
			}

			


			//option detail media
			if (mediaDetailOption){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(1150,customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				mediaDetail.RenderControl(output);
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}
			//option choix d'un encart
			if (insertOption){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(1400,customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				listInsert.RenderControl(output);
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}
			//option detail produit
			if (productDetailOption){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(1124,customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				productDetail.RenderControl(output);
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}
			

			//option format de tableau
			if (tblChoiceOption){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(1140,customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				tblChoice.RenderControl(output);
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}

			

			//options PDM, PDV, evolution
			if (pdmOption || pdvOption || evolutionOption){
				output.Write("\n<tr>");
				output.Write("\n<td>");
				if(evolutionOption){
					if (!customerWebSession.ComparativeStudy){
						EvolutionCheckBox.Enabled = false;
						EvolutionCheckBox.Checked = false;
					}
					EvolutionCheckBox.RenderControl(output);
					output.Write("&nbsp;&nbsp;");
				}
				if(pdmOption){
					PdmCheckBox.RenderControl(output);
					output.Write("&nbsp;&nbsp;");
				}
				if(pdvOption){
					PdvCheckBox.RenderControl(output);
				}
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}


			//options de visu des elements perso
			if (personalizedElementsOption){
				output.Write("\n<tr>");
				output.Write("\n<td>");
				//if (customerWebSession.GetSelection(customerWebSession.ReferenceUniversAdvertiser,CustomerCst.type.advertiserAccess).Length<=0){
				//    if (customerWebSession.CompetitorUniversAdvertiser[0]!=null){
				//        if (customerWebSession.GetSelection((System.Windows.Forms.TreeNode)customerWebSession.CompetitorUniversAdvertiser[0] ,CustomerCst.type.advertiserAccess).Length<=0){
				//            PersonalizedElementsCheckBox.Enabled = false;
				//        }
				//    }
				//    else
				//        PersonalizedElementsCheckBox.Enabled = false;
				//}
				bool withAdvertisers = false;
				string tempString = "";
				if (customerWebSession.SecondaryProductUniverses.Count > 0 && customerWebSession.SecondaryProductUniverses.ContainsKey(0) && customerWebSession.SecondaryProductUniverses[0].Contains(0)) {
					tempString = customerWebSession.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
					if (tempString != null && tempString.Length > 0) withAdvertisers = true;
				}
				else if (customerWebSession.SecondaryProductUniverses.Count > 0 && customerWebSession.SecondaryProductUniverses.ContainsKey(1) && customerWebSession.SecondaryProductUniverses[1].Contains(0)) {
					tempString = customerWebSession.SecondaryProductUniverses[1].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
					if (tempString != null && tempString.Length > 0) withAdvertisers = true;
				}
				PersonalizedElementsCheckBox.Enabled = withAdvertisers;
					
				PersonalizedElementsCheckBox.RenderControl(output);
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}

			//Initialisation des annonceurs de références
			if(this._initializeProductWebControl!=null && this._inializeAdvertiserOption){
				output.Write("\n<tr>");
				output.Write("\n<td>");
				this._initializeProductWebControl.RenderControl(output);
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}



			//option résultat
			if (resultOption){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(793,customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				resultsPages.RenderControl(output);
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}
			//option products for APPM
			if (productsOption)
			{
				if(products.Items.Count>0)
				{
					output.Write("\n<tr>");
					output.Write("\n<td class=\"txtGris11Bold\">");
					output.Write(GestionWeb.GetWebWord(1164,customerWebSession.SiteLanguage));
					output.Write("\n</td>");
					output.Write("\n</tr>");
					output.Write("\n<tr>");
					output.Write("\n<td>");
					products.RenderControl(output);
					output.Write("\n</td>");
					output.Write("\n</tr>");
					output.Write("\n<TR>");
					output.Write("\n<TD height=\"5\"></TD>");
					output.Write("\n</TR>");
				}
			}
			
			if(_resultFormat){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				graphRadioButton.RenderControl(output);
                output.Write("<A onmouseover=\"graph.src = '/App_Themes/" + themeName + "/Images/Common/Button/chart_down.gif';\" onclick=\"graphRadioButton.checked=true;\" onmouseout=\"graph.src = '/App_Themes/" + themeName + "/Images/Common/Button/chart_up.gif';\" href=\"#\"><IMG id=graph title=\"" + ChartTitle + "\" src=\"/App_Themes/" + themeName + "/Images/Common/Button/chart_up.gif\" border=0 ></A>&nbsp;");
				tableRadioButton.RenderControl(output);
                output.Write("<A onmouseover=\"table.src = '/App_Themes/" + themeName + "/Images/Common/Button/table_down.gif';\" onclick=\"tableRadioButton.checked=true;\" onmouseout=\"table.src = '/App_Themes/" + themeName + "/Images/Common/Button/table_up.gif';\" href=\"#\"><IMG id=table title=\"" + TableTitle + "\" src=\"/App_Themes/" + themeName + "/Images/Common/Button/table_up.gif\" border=0 ></A>");
				output.Write("\n</td>");
				output.Write("\n</tr>");			
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}
			
			//Option type de pourcentage (horizontal ou vertical)
			if (_percentageTypeOption){
				
					output.Write("\n<tr>");
					output.Write("\n<td class=\"txtGris11Bold\">");
					output.Write(GestionWeb.GetWebWord(1236,customerWebSession.SiteLanguage));
					output.Write("\n</td>");
					output.Write("\n</tr>");
					output.Write("\n<tr>");
					output.Write("\n<td>");
					_percentageTypeDropDownList.RenderControl(output);
					output.Write("\n</td>");
					output.Write("\n</tr>");
					output.Write("\n<TR>");
					output.Write("\n<TD height=\"5\"></TD>");
					output.Write("\n</TR>");
				
			}
			

			//fin tableau
			output.Write("\n</table>");
		}
		#endregion

		#endregion

		#region Méthodes internes
		/// <summary>
		/// Determine si un résultat doit être montré.
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="current">Page en cours</param>
		/// <returns>Vrai si un résultat doit être montré </returns>
		private bool CanShowResult(WebSession webSession,ResultPageInformation current){
			if((webSession.CurrentModule==WebConstantes.Module.Name.ALERTE_PORTEFEUILLE)&&
						(((ClassificationCst.DB.Vehicles.names)((LevelInformation) webSession.SelectionUniversMedia.FirstNode.Tag).ID)==ClassificationCst.DB.Vehicles.names.outdoor)&&
						(current.Id.ToString()=="2"||current.Id.ToString()=="3"||current.Id.ToString()=="4")
				)return false;
			else if((webSession.CurrentModule==WebConstantes.Module.Name.BILAN_CAMPAGNE)				
				&& current.Id.ToString()=="7" && webSession.CustomerLogin.GetFlag(TNS.AdExpress.Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG)==null)return false;
			else return true;
		}

		#endregion
		
		
	}
}