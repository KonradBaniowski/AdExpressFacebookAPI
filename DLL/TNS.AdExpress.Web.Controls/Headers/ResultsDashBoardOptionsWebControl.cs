#region Informations
// Auteur: D. V. Mussuma 
// Date de création: 08/02/2005 
// Date de modification: 23/03/2005 
//09/11/2005  D. Mussuma Intégration des statistiques
#endregion

#region NameSpace
using System;
using System.Data;
using System.Web.UI;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Selections;
using TNS.AdExpress.Web.DataAccess.Selections.Products;
using CstWeb = TNS.AdExpress.Constantes.Web;
using SessionCst = TNS.AdExpress.Constantes.Web.CustomerSessions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Web.Controls.Results;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.DataAccess.Selections.Medias;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
#endregion

namespace TNS.AdExpress.Web.Controls.Headers
{
	/// <summary>
	/// Composant qui affiche les options pour le groupe de modules
	/// tableaux de bord.
	/// </summary>
	[ToolboxData("<{0}:ResultsDashBoardOptionsWebControl runat=server></{0}:ResultsDashBoardOptionsWebControl>")]
	public class ResultsDashBoardOptionsWebControl : System.Web.UI.WebControls.WebControl{	
	
		#region Variables
		
		/// <summary>
		/// List des unités sélectionnable
		/// </summary>
		protected DropDownList list;
		/// <summary>
		/// Liste des encarts  sélectionnable 
		/// </summary>
		public DropDownList listInsert;
		/// <summary>
		/// Checkbox dédiée à la PDV 
		/// </summary>
		protected System.Web.UI.WebControls.CheckBox PdvCheckBox;
		/// <summary>
		/// Checkbox dédiée à l'affichage de l'option évolution 
		/// </summary>
		protected System.Web.UI.WebControls.CheckBox EvolutionCheckBox;
		/// <summary>
		/// Checkbox de cumul de la période
		/// </summary>
		protected System.Web.UI.WebControls.CheckBox CumulPeriodCheckBox;
		
		/// <summary>
		/// Choix du mois pour un affichage mensuel.
		/// </summary>
		protected DropDownList monthlyDate;		
		/// <summary>
		/// Choix de la semaine pour un affichage hebdomadaire
		/// </summary>
		protected DropDownList weeklyDate;	
		/// <summary>
		/// Choix du niveau de détail media
		/// </summary>
		public DropDownList mediaDetail;
		/// <summary>
		/// Choix d'un famille de produits
		/// </summary>
		protected DropDownList sectorList;
		/// <summary>
		/// Choix d'un centre d'interet de produits
		/// </summary>
		protected DropDownList interestCenterList;
		/// <summary>
		/// Année sélectionnée
		/// </summary>
		protected int selectedYear=DateTime.Now.Year;
		/// <summary>
		/// Calendrier en mémoire
		/// </summary>
		private YearMonthWeekCalendar memoryCalendar=null;
		/// <summary>
		/// Checkbox dédiée à la PDM 
		/// </summary>
		protected System.Web.UI.WebControls.CheckBox PdmCheckBox;
		/// <summary>
		/// Checkbox pour pdm (pourcentage quelconque et PDM pour tout ce qui est concurrentielle)
		/// </summary>
		protected  System.Web.UI.WebControls.CheckBox  percentageCheckBox;
		/// <summary>
		/// Choix du format de spots
		/// </summary>
		protected DropDownList format;	
		/// <summary>
		/// Choix du jour nommé
		/// </summary>
		protected DropDownList namedDay;	
		/// <summary>
		/// Choix de la tranche horaire
		/// </summary>
		protected DropDownList drpTimeInterval;	
		/// <summary>
		/// famille sélectionnée
		/// </summary>
		protected string currentSectors = "";
		/// <summary>
		/// Liste des familles
		/// </summary>
		protected DataSet dsSectorList = null;
		/// <summary>
		/// famille sélectionnée
		/// </summary>
		protected string currentInterestCenter = "";
		/// <summary>
		/// Liste des centre d'interet
		/// </summary>
		protected DataSet dsInterestCenterList= null;
		/// <summary>
		/// Choix de centre d'interet
		/// </summary>
		protected DropDownList drpInterestCenterList ;
		/// <summary>
		/// Choix du type de tableau à afficher 
		/// </summary>
		protected ImageDropDownListWebControl tblChoice;
		/// <summary>
		/// index table des tableuax de bord
		/// </summary>
		public const int DashBoardEnumIndex=11; 
		/// <summary>
		///identification du Média  sélectionné
		/// </summary>
		protected string Vehicle="";
		/// <summary>
		///Type de Média sélectionné
		/// </summary>
		private DBClassificationConstantes.Vehicles.names vehicleType;
		
		#endregion

		#region Propriétés
		
		/// <summary>
		/// Option unité
		/// </summary>
		[Bindable(true),
		Description("Option choix de l'unité")]
		protected bool unitOption = true;
		/// <summary></summary>
		public bool UnitOption{
			get{return unitOption;}
			set{unitOption=value;}
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

		/// <summary>Option pourcentage</summary>
		public  System.Web.UI.WebControls.CheckBox PercentageCheckBox{
			get{return percentageCheckBox;}
			set{percentageCheckBox=value;}
		}
		/// <summary>
		///Option pourcentage
		/// </summary>
		[Bindable(false),
		Description("pourcentage")]
		protected bool percentage = false;
		/// <summary></summary>
		public bool Percentage{
			get{return percentage;}
			set{percentage=value;}
		}
		/// <summary>
		/// CssClass générale 
		/// </summary>
		[Bindable(true),DefaultValue("txtNoir11Bold"),
		Description("classe de style")]
		protected string cssClass = "txtNoir11Bold";
		/// <summary></summary>
		public string CommonCssClass{
			get{return cssClass;}
			set{cssClass=value;}
		}

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
		/// Option affichage cumul période
		/// </summary>
		[Bindable(true),
		Description("Option cumuler la période.")]
		protected bool cumulPeriodOption = false;
		/// <summary>
		/// Propriété cumul période
		/// </summary>
		public bool CumulPeriodOption{
			get{return cumulPeriodOption;}
			set{cumulPeriodOption=value;}
		}
		
		/// <summary>
		/// Option activation/inactivation cumul période
		/// </summary>
		[Bindable(true),
		Description("Option activer cumule de la période.")]
		protected bool isCumulPeriodChecked = false;
		/// <summary>
		/// Propriété activation/désactivation cumul période
		/// </summary>
		public bool IsCumulPeriodChecked{
			get{return isCumulPeriodChecked;}
			set{isCumulPeriodChecked=value;}
		}

		/// <summary>
		/// Option date sous forme mensuelle
		/// </summary>
		[Bindable(true),Description("Option date sous forme mensuelle.")]
		protected bool monthlyDateOption=false;
		/// <summary>
		/// Propriété date sous forme mensuelle
		/// </summary>
		public bool MonthlyDateOption{
			get{return monthlyDateOption;}
			set{monthlyDateOption=value;}
		}
		
		/// <summary>
		/// Option date sous forme hebdomadaire
		/// </summary>
		[Bindable(true),Description("Option date sous forme hebdomadaire.")]
		protected bool weeklyDateOption=false;
		/// <summary>
		/// Propriété date sous forme hebdomadaire
		/// </summary>
		public bool WeeklyDateOption{
			get{return weeklyDateOption;}
			set{weeklyDateOption=value;}
		}

		/// <summary>
		/// Option d'affichage du titre
		/// </summary>
		[Bindable(true),Description("Option d'affichage du titre.")]
		protected bool titleOption=false;
		/// <summary>
		/// Propriété d'affichage du titre
		/// </summary>
		public bool TitleOption{
			get{return titleOption;}
			set{titleOption=value;}
		}
		/// <summary>
		/// Ontient et définit l'année à afficher
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue("2005")] 
		public int SelectedYear{
			get{return selectedYear;}
			set{
				selectedYear = value;
				if(memoryCalendar==null)new YearMonthWeekCalendar(value); 
				else memoryCalendar.Year=value;
			}
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
		/// Option format de spot
		/// </summary>
		protected bool formatOption = false;
		/// <summary>
		/// Propriété choix du format de spots
		/// </summary>
		[Bindable(true),DefaultValue(false),
		Description("Option choix du format de spots")]
		public bool FormatOption{
			get{return formatOption;}
			set{formatOption= value;}
		}
		/// <summary>
		/// Option tranche horaire
		/// </summary>
		protected bool timeIntervalOption = false;
		/// <summary>
		/// Propriété choix d'une tranche horaire
		/// </summary>
		[Bindable(true),DefaultValue(false),Description("Option choix d'une tranche horaire.")]
		public bool TimeIntervalOption{
			get{return timeIntervalOption;}
			set{timeIntervalOption = value;}
		}
		/// <summary>
		/// Option jour nommé
		/// </summary>
		protected bool namedDayOption=false;
		/// <summary>
		/// Propriété choix d'un jour nommé
		/// </summary>
		[Bindable(true),DefaultValue(false),Description("Option choix d'un jour nommé.")]
		public bool NamedDayOption{
			get{return namedDayOption;}
			set{namedDayOption = value;}
		}
		
		/// <summary>
		/// Option choix d'une famille
		/// </summary>
		protected bool sectorListOption=false;
		/// <summary>
		/// Propriété choix d'une famille
		/// </summary>
		[Bindable(true),DefaultValue(false),Description("Option choix d'une famille.")]
		public bool SectorListOption{
			get{return sectorListOption;}
			set{sectorListOption = value;}
		}

		/// <summary>
		/// Option choix du centre d'interet
		/// </summary>
		protected bool interestCenterListOption=false;
		/// <summary>
		/// Propriété choix du centre d'interet
		/// </summary>
		[Bindable(true),DefaultValue(false),Description("Option choix d'un centre d'interet.")]
		public bool InterestCenterListOption{
			get{return interestCenterListOption;}
			set{interestCenterListOption = value;}
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

        /// <summary>
		///noms des images 
		/// </summary>
		[Bindable(true),DefaultValue("")]
        private string _imageButtonArrow = "";
		/// <summary></summary>
        public string ImageButtonArrow {
            get { return _imageButtonArrow; }
            set { _imageButtonArrow = value; }
		}
		#endregion

		/// <summary>
		/// Session du client (utile pour la langue)
		/// </summary>
		protected WebSession customerWebSession = null;
		/// <summary></summary>
		public WebSession CustomerWebSession{
			get{return customerWebSession;}
			set{customerWebSession=value;}
		}
		#endregion

		#region constructeur(s)
		/// <summary>
		/// Constructeur
		/// </summary>
		public ResultsDashBoardOptionsWebControl():base() {
			this.EnableViewState = true;
			this.PreRender+=new EventHandler(Custom_PreRender);			
		}
		#endregion

		#region Evènements

		#region Init
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {
			string listSector="";
			string listInterestCenter="";
			//string nameSector="";
			//DataSet dsSector;
			if (Page.IsPostBack){
				//Option Unités
				if (unitOption){
					try{
						if(Page.Request.Form.GetValues("_units")!=null && Page.Request.Form.GetValues("_units")[0]!= customerWebSession.Unit.ToString())
						customerWebSession.Unit = (SessionCst.Unit) Int64.Parse(Page.Request.Form.GetValues("_units")[0]);
					}
					catch(SystemException){
					}
				}
				//Options Encarts
				if (insertOption){
					if(Page.Request.Form.GetValues("_inserts")!=null){
						customerWebSession.Insert = (SessionCst.Insert) Int64.Parse(Page.Request.Form.GetValues("_inserts")[0]);					
					}
				}
				//Option pourcentage
				if(percentage){
					try{
						if(Page.Request.Form.GetValues("_percentage")[0]!=null)customerWebSession.Percentage=true;
					}catch(System.Exception) {
						customerWebSession.Percentage=false;}						
				}
				//PDV
				if(pdvOption){
					try{
						if(Page.Request.Form.GetValues(this.ID+"_pdv")[0]!=null)customerWebSession.PDV=true;
					}catch(System.Exception) {
						customerWebSession.PDV=false;}						
				}
				//PDM
				if(pdmOption){
					try{
						if(Page.Request.Form.GetValues(this.ID+"_pdm")[0]!=null)customerWebSession.PDM=true;
					}catch(System.Exception) {
						customerWebSession.PDM=false;}						
				}	
				//Evolution
				if(evolutionOption){
					try{
						if(Page.Request.Form.GetValues(this.ID+"_evol")[0]!=null)customerWebSession.Evolution=true;
					}catch(System.Exception) {
						customerWebSession.Evolution=false;}						
				}
				//Option détail média
				if (mediaDetailOption)
					try{
						customerWebSession.PreformatedMediaDetail = (SessionCst.PreformatedDetails.PreformatedMediaDetails) int.Parse(Page.Request.Form.GetValues("mediaDetail_"+this.ID)[0]);
					}
					catch(System.Exception){}
				//Option format
				if(formatOption){
					try{
						customerWebSession.Format = (CstWeb.Repartition.Format) int.Parse(Page.Request.Form.GetValues("format_"+this.ID)[0]);
					}
					catch(System.Exception){}
				}
				//option jour nommé
				if(NamedDayOption){
					try{
						customerWebSession.NamedDay = (CstWeb.Repartition.namedDay) int.Parse(Page.Request.Form.GetValues("namedDay_"+this.ID)[0]);
					}
					catch(System.Exception){}
				}
				//Option Tranche horaire
				if(timeIntervalOption){
					try{
						customerWebSession.TimeInterval = (CstWeb.Repartition.timeInterval) int.Parse(Page.Request.Form.GetValues("timeInterval_"+this.ID)[0]);
					}
					catch(System.Exception){}
				}
				//Option choix d'un mois
				if(monthlyDateOption){
					try{
						customerWebSession.DetailPeriodBeginningDate = Page.Request.Form.GetValues("monthlyDate_"+this.ID)[0].ToString();
						customerWebSession.DetailPeriodEndDate= Page.Request.Form.GetValues("monthlyDate_"+this.ID)[0].ToString();
					}
					catch(System.Exception){}
				}
				
				//Option choix d'une semaine
				if(weeklyDateOption){
					try{
						customerWebSession.DetailPeriodBeginningDate = Page.Request.Form.GetValues("weeklyDate_"+this.ID)[0].ToString();
						customerWebSession.DetailPeriodEndDate= Page.Request.Form.GetValues("weeklyDate_"+this.ID)[0].ToString();
					}
					catch(System.Exception){}
				}
				//Option choix d'une famille
				if(sectorListOption){
					try{	
						listSector = Page.Request.Form.GetValues("sectorList_"+this.ID)[0].ToString();					
						if(WebFunctions.CheckedText.IsStringEmpty(listSector) && !listSector.Equals("0")) {
							#region Ancienne version
							//System.Windows.Forms.TreeNode sectors = new System.Windows.Forms.TreeNode("sector");							
							//System.Windows.Forms.TreeNode tmp;
							//    // Ouverture de la base de données
							//    #region Connexion à la base de données
							//    if (customerWebSession.CustomerLogin.Connection==null)
							//        customerWebSession.CustomerLogin.Connection=new OracleConnection(customerWebSession.CustomerLogin.OracleConnectionString);
							//    #endregion

							//    dsSector=ProductClassificationListDataAccess.SectorList(customerWebSession,listSector);
							//    if (dsSector != null && dsSector.Tables[0].Rows.Count == 1) {
							//        tmp = new System.Windows.Forms.TreeNode("sector");
							//        tmp.Tag = new LevelInformation(CustomerRightConstante.type.sectorAccess, Int64.Parse(listSector), dsSector.Tables[0].Rows[0]["sector"].ToString());
							//        tmp.Checked = true;
							//        sectors.Nodes.Add(tmp);
							//        customerWebSession.SelectionUniversProduct = sectors;
							//    }
							#endregion
							Dictionary<int, AdExpressUniverse> universeDictionary = new Dictionary<int, AdExpressUniverse>();
							customerWebSession.SecondaryProductUniverses.Clear();
							AdExpressUniverse adExpressUniverse = new AdExpressUniverse(Dimension.product);
							NomenclatureElementsGroup nGroup = new NomenclatureElementsGroup(0, AccessType.includes);
							nGroup.AddItem(TNSClassificationLevels.SECTOR, long.Parse(listSector));
							adExpressUniverse.AddGroup(0, nGroup);
							universeDictionary.Add(0, adExpressUniverse);
							customerWebSession.SecondaryProductUniverses = universeDictionary;							
						}
						else{
							customerWebSession.SecondaryProductUniverses = new Dictionary<int, AdExpressUniverse>();
						}
																							
					}
					catch(System.Exception){}
				}
				//Option choix d'un centre d'intérêt
				if(InterestCenterListOption){
					try{	
						listInterestCenter = Page.Request.Form.GetValues("interestCenterList_"+this.ID)[0].ToString();					
						if(WebFunctions.CheckedText.IsStringEmpty(listInterestCenter) && !listInterestCenter.Equals("0")) {						
							System.Windows.Forms.TreeNode interestCenters = new System.Windows.Forms.TreeNode("media");							
							System.Windows.Forms.TreeNode tmp;
//							if (isDetailElement(customerWebSession.CurrentUniversMedia)) {
								// Ouverture de la base de données
                                //#region Connexion à la base de données
                                //if (customerWebSession.CustomerLogin.Connection==null)
                                //    customerWebSession.CustomerLogin.Connection=new OracleConnection(customerWebSession.CustomerLogin.OracleConnectionString);
                                //#endregion								

								dsInterestCenterList=VehicleListDataAccess.InterestCenterList(customerWebSession,listInterestCenter);
								if(dsInterestCenterList != null && dsInterestCenterList.Tables[0].Rows.Count==1) {
									tmp = new System.Windows.Forms.TreeNode("interestCenter");
									tmp.Tag = new LevelInformation(CustomerRightConstante.type.interestCenterAccess,Int64.Parse(listInterestCenter),dsInterestCenterList.Tables[0].Rows[0]["interest_center"].ToString());
									tmp.Checked = true;
									interestCenters.Nodes.Add(tmp);
									customerWebSession.CurrentUniversMedia = interestCenters;
								}
//							}
							
						}
						else{
							customerWebSession.CurrentUniversMedia =  new System.Windows.Forms.TreeNode("media");
						}
					
																		
					}
					catch(System.Exception){}
				}

				
				//				if(tblChoiceOption){					
				//					customerWebSession.PreformatedTable = (SessionCst.PreformatedDetails.PreformatedTables) Int64.Parse(Page.Request.Form.GetValues("DDL"+this.ID)[0]);
				//				}
				customerWebSession.Save();
							
			}	
					
			base.OnInit(e);
		}
		#endregion
				
		#region Custom_PreRender
		/// <summary>
		///custom prerender 
		/// </summary>
		/// <param name="sender">object qui lance l'évènement</param>
		/// <param name="e">arguments</param>
		private void Custom_PreRender(object sender, System.EventArgs e){	
			//identification du Média  sélectionné
			Vehicle = ((LevelInformation)customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
			vehicleType = (DBClassificationConstantes.Vehicles.names)int.Parse(Vehicle);
			
			#region Unité
			if(unitOption){
				//Création de la liste des unités
				list = new DropDownList();
				list.ID = "_units";
				list.CssClass =  cssClass;
				list.AutoPostBack = autoPostBackOption;
				if (!percentage)list.Width = new System.Web.UI.WebControls.Unit("100%");
//				ArrayList units = WebFunctions.Units.getUnitsFromVehicleSelection(customerWebSession.GetSelection(customerWebSession.SelectionUniversMedia,CustomerRightConstante.type.vehicleAccess));
				ArrayList units = WebFunctions.Units.getUnitsFromVehicleSelection(Vehicle);
				for(int i = 0; i<units.Count; i++){
					list.Items.Add(new ListItem(GestionWeb.GetWebWord((int)SessionCst.UnitsTraductionCodes[(SessionCst.Unit)units[i]],customerWebSession.SiteLanguage) ,((int)(SessionCst.Unit)units[i]).ToString()));
				}
				list.Items.FindByValue(((int)customerWebSession.Unit).ToString()).Selected=true;

				Controls.Add(list);
			}
			#endregion

			#region Encart
			if(insertOption){
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
			
			#region pourcentage
			if(percentage){
				percentageCheckBox=new  System.Web.UI.WebControls.CheckBox();
				percentageCheckBox.ID="_percentage";
				percentageCheckBox.CssClass=cssClass;
				percentageCheckBox.AutoPostBack=autoPostBackOption;
				percentageCheckBox.Text=GestionWeb.GetWebWord(1588,customerWebSession.SiteLanguage);				
				percentageCheckBox.Checked=customerWebSession.Percentage;
				Controls.Add(percentageCheckBox);
			}
			#endregion

			#region PDM
			//Option PDM
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
			#endregion
			
			#region PDV
			//Option PDV
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
			#endregion

			#region Evolution			
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

			#endregion

			#region cumul période
			//Option cumul période
			if(cumulPeriodOption){
				CumulPeriodCheckBox=new System.Web.UI.WebControls.CheckBox();
				CumulPeriodCheckBox.ID="_cumulPeriodCheckBox";
				CumulPeriodCheckBox.ToolTip = GestionWeb.GetWebWord(1528,customerWebSession.SiteLanguage);
				CumulPeriodCheckBox.CssClass=cssClass;
				CumulPeriodCheckBox.AutoPostBack=autoPostBackOption;				
				CumulPeriodCheckBox.Attributes["onclick"]="javascript:SelectPeriod('_cumulPeriodCheckBox');";  				
				CumulPeriodCheckBox.Text=GestionWeb.GetWebWord(1527,customerWebSession.SiteLanguage);				
				if(customerWebSession.DetailPeriod == Constantes.Web.CustomerSessions.Period.DisplayLevel.yearly)
				isCumulPeriodChecked=true;			
				CumulPeriodCheckBox.Checked=isCumulPeriodChecked;						
				Controls.Add(CumulPeriodCheckBox);
				#region Script
				if (!this.Page.ClientScript.IsClientScriptBlockRegistered("SelectPeriod"))this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"OpenCreationCompetitorAlert",WebFunctions.Script.SelectPeriod("ResultsDateListWebControl1","ResultsDateListWebControl2","_cumulPeriodCheckBox"));						
				#endregion
			}

		
			#endregion
			
			#region choix du détail média
			//Option choix du détail média
			if(mediaDetailOption){
				mediaDetail = new DropDownList();
				mediaDetail.Width = new System.Web.UI.WebControls.Unit("100%");
				mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1542, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter.GetHashCode().ToString()));
				mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1543, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia.GetHashCode().ToString()));
				mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1544, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMedia.GetHashCode().ToString()));
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
			#endregion

			#region choix du tableau à afficher

			//Option choix du tableau à afficher
			if(tblChoiceOption){
				tblChoice = new ImageDropDownListWebControl();
				tblChoice.BackColor = this.BackColor;
				tblChoice.BorderColor = this.BorderColor;
				tblChoice.BorderWidth = new System.Web.UI.WebControls.Unit(this.borderWidth);
                tblChoice.ImageButtonArrow = this._imageButtonArrow;
				if (this.List!="") tblChoice.List = this.List;
				else{
					if (vehicleType!= DBClassificationConstantes.Vehicles.names.press){  
						tblChoice.List = "&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;";	
					}else tblChoice.List = "&nbsp;|&nbsp;|&nbsp;|&nbsp;";
				}
				if (this.images!="") tblChoice.Images = this.images;
				else{
                    string themeName = WebApplicationParameters.Themes[customerWebSession.SiteLanguage].Name;
					if (vehicleType!= DBClassificationConstantes.Vehicles.names.press){
                        tblChoice.Images = "/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype1.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype2.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype3.gif" +
							"|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype4.gif" +						
							"|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype5.gif" +
							"|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype6.gif" +
							"|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype7.gif" +
							"|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype8.gif" +
							"|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype9.gif"+
							"|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype10.gif" +
							"|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype11.gif" +
							"|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype12.gif" +
							"|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype13.gif" ; 
					}else {
						tblChoice.Images= "/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype1.gif" +
							"|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype2.gif" +
							"|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype3.gif" +
							"|/App_Themes/" + themeName + "/Images/Culture/Tables/TBtype13.gif" ; 
					}
				}
				int numberImagesForPress=4;
				int numberImagesForOthersMedia=13;
				if(vehicleType== DBClassificationConstantes.Vehicles.names.press && customerWebSession.PreformatedTable== CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector){
					//index du tableau Media\Famille pour le média presse
					tblChoice.ListIndex = customerWebSession.PreformatedTable.GetHashCode()-DashBoardEnumIndex-(numberImagesForOthersMedia-numberImagesForPress);
				}
				else tblChoice.ListIndex = customerWebSession.PreformatedTable.GetHashCode()-DashBoardEnumIndex;
				tblChoice.ImageHeight = this.imageHeight;
				tblChoice.ImageWidth = this.imageWidth;
				tblChoice.ID = "DDL"+this.ID;
				tblChoice.OutCssClass = this.outCssClass;
				tblChoice.OverCssClass = this.overCssClass;
				tblChoice.Width = new System.Web.UI.WebControls.Unit("121");
				tblChoice.ShowPictures = this.pictShow;
				Controls.Add(tblChoice);
			}
			#endregion

			#region choix d'une famille

			//Option choix d'une famille
			if(sectorListOption){
				sectorList = new DropDownList();				
				FillSectorList();
				sectorList.ID = "sectorList_"+this.ID;
				sectorList.AutoPostBack=autoPostBackOption;
				sectorList.CssClass = cssClass;				
				Controls.Add(sectorList);
			}
			#endregion

			#region choix du centre d'interet

			//Option choix du centre d'interet
			if(interestCenterListOption){
				interestCenterList = new DropDownList();				
				FillInterestCentreList();
				interestCenterList.ID = "interestCenterList_"+this.ID;
				interestCenterList.AutoPostBack=autoPostBackOption;
				interestCenterList.CssClass = cssClass;				
				Controls.Add(interestCenterList);
			}
			#endregion
						
			#region choix du format spots
			//Option choix du format spots
			if(formatOption){
				format = new DropDownList();				
				switch((ClassificationCst.DB.Vehicles.names)((LevelInformation) customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID){					
					case ClassificationCst.DB.Vehicles.names.tv:
					case ClassificationCst.DB.Vehicles.names.radio:	
					case ClassificationCst.DB.Vehicles.names.others:
						format.Items.Add(new ListItem(GestionWeb.GetWebWord(1401, customerWebSession.SiteLanguage),CstWeb.Repartition.Format.Total.GetHashCode().ToString()));
						format.Items.Add(new ListItem(GestionWeb.GetWebWord(1545, customerWebSession.SiteLanguage),CstWeb.Repartition.Format.Spot_1_9.GetHashCode().ToString()));
						format.Items.Add(new ListItem(GestionWeb.GetWebWord(1546, customerWebSession.SiteLanguage),CstWeb.Repartition.Format.Spot_10.GetHashCode().ToString()));
						format.Items.Add(new ListItem(GestionWeb.GetWebWord(1547, customerWebSession.SiteLanguage),CstWeb.Repartition.Format.Spot_11_19.GetHashCode().ToString()));
						format.Items.Add(new ListItem(GestionWeb.GetWebWord(1548, customerWebSession.SiteLanguage),CstWeb.Repartition.Format.Spot_20.GetHashCode().ToString()));
						format.Items.Add(new ListItem(GestionWeb.GetWebWord(1549, customerWebSession.SiteLanguage),CstWeb.Repartition.Format.Spot_21_29.GetHashCode().ToString()));
						format.Items.Add(new ListItem(GestionWeb.GetWebWord(1550, customerWebSession.SiteLanguage),CstWeb.Repartition.Format.Spot_30.GetHashCode().ToString()));
						format.Items.Add(new ListItem(GestionWeb.GetWebWord(1551, customerWebSession.SiteLanguage),CstWeb.Repartition.Format.Spot_31_45.GetHashCode().ToString()));
						format.Items.Add(new ListItem(GestionWeb.GetWebWord(1552, customerWebSession.SiteLanguage),CstWeb.Repartition.Format.Spot_45.GetHashCode().ToString()));
						break;									
					default:												
						break;
				}
				format.ID = "format_"+this.ID;
				format.AutoPostBack=autoPostBackOption;
				format.CssClass = cssClass;				
				try{
					format.Items.FindByValue(customerWebSession.Format.GetHashCode().ToString()).Selected = true;
				}
				catch(System.Exception){
					format.Items[0].Selected=true;
				}

				Controls.Add(format);
			}
			#endregion

			#region choix du jour nommé
			//Option choix du jour nommé
			if(namedDayOption){
				namedDay = new DropDownList();				
				switch((ClassificationCst.DB.Vehicles.names)((LevelInformation) customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID){
					case ClassificationCst.DB.Vehicles.names.tv:
					case ClassificationCst.DB.Vehicles.names.radio:
					case ClassificationCst.DB.Vehicles.names.others:
						//namedDay.Items.Add(new ListItem(GestionWeb.GetWebWord(1401, customerWebSession.SiteLanguage),CstWeb.Repartition.namedDay.Total.GetHashCode().ToString()));
						namedDay.Items.Add(new ListItem(GestionWeb.GetWebWord(848, customerWebSession.SiteLanguage),CstWeb.Repartition.namedDay.Total.GetHashCode().ToString()));
						namedDay.Items.Add(new ListItem(GestionWeb.GetWebWord(1553, customerWebSession.SiteLanguage),CstWeb.Repartition.namedDay.Week_5_day.GetHashCode().ToString()));
						namedDay.Items.Add(new ListItem(GestionWeb.GetWebWord(1554, customerWebSession.SiteLanguage),CstWeb.Repartition.namedDay.Monday.GetHashCode().ToString()));
						namedDay.Items.Add(new ListItem(GestionWeb.GetWebWord(1555, customerWebSession.SiteLanguage),CstWeb.Repartition.namedDay.Tuesday.GetHashCode().ToString()));
						namedDay.Items.Add(new ListItem(GestionWeb.GetWebWord(1556, customerWebSession.SiteLanguage),CstWeb.Repartition.namedDay.Wednesdays.GetHashCode().ToString()));
						namedDay.Items.Add(new ListItem(GestionWeb.GetWebWord(1557, customerWebSession.SiteLanguage),CstWeb.Repartition.namedDay.Thursday.GetHashCode().ToString()));
						namedDay.Items.Add(new ListItem(GestionWeb.GetWebWord(1558, customerWebSession.SiteLanguage),CstWeb.Repartition.namedDay.Friday.GetHashCode().ToString()));
						namedDay.Items.Add(new ListItem(GestionWeb.GetWebWord(1559, customerWebSession.SiteLanguage),CstWeb.Repartition.namedDay.Saturday.GetHashCode().ToString()));
						namedDay.Items.Add(new ListItem(GestionWeb.GetWebWord(1560, customerWebSession.SiteLanguage),CstWeb.Repartition.namedDay.Sunday.GetHashCode().ToString()));
						namedDay.Items.Add(new ListItem(GestionWeb.GetWebWord(1561, customerWebSession.SiteLanguage),CstWeb.Repartition.namedDay.Week_end.GetHashCode().ToString()));						
						break;									
					default:												
						break;
				}
				namedDay.ID = "namedDay_"+this.ID;
				namedDay.AutoPostBack=autoPostBackOption;
				namedDay.CssClass = cssClass;				
				try{
					namedDay.Items.FindByValue(customerWebSession.NamedDay.GetHashCode().ToString()).Selected = true;
				}
				catch(System.Exception){
					namedDay.Items[0].Selected=true;
				}

				Controls.Add(namedDay);
			}
			#endregion

			#region choix de la tranche horaire
			//Option choix de la tranche horaire
			if(timeIntervalOption){
				drpTimeInterval = new DropDownList();				
				switch((ClassificationCst.DB.Vehicles.names)((LevelInformation) customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID){
					case ClassificationCst.DB.Vehicles.names.tv:
					case ClassificationCst.DB.Vehicles.names.others:
						drpTimeInterval.Items.Add(new ListItem(GestionWeb.GetWebWord(1401, customerWebSession.SiteLanguage),CstWeb.Repartition.timeInterval.Total.GetHashCode().ToString()));
						drpTimeInterval.Items.Add(new ListItem(GestionWeb.GetWebWord(1564, customerWebSession.SiteLanguage),CstWeb.Repartition.timeInterval.Slice_7h_12h.GetHashCode().ToString()));
						drpTimeInterval.Items.Add(new ListItem(GestionWeb.GetWebWord(1566, customerWebSession.SiteLanguage),CstWeb.Repartition.timeInterval.Slice_12h_14h.GetHashCode().ToString()));
						drpTimeInterval.Items.Add(new ListItem(GestionWeb.GetWebWord(1568, customerWebSession.SiteLanguage),CstWeb.Repartition.timeInterval.Slice_14h_17h.GetHashCode().ToString()));
						drpTimeInterval.Items.Add(new ListItem(GestionWeb.GetWebWord(1569, customerWebSession.SiteLanguage),CstWeb.Repartition.timeInterval.Slice_17h_19h.GetHashCode().ToString()));
						drpTimeInterval.Items.Add(new ListItem(GestionWeb.GetWebWord(1570, customerWebSession.SiteLanguage),CstWeb.Repartition.timeInterval.Slice_19h_22h.GetHashCode().ToString()));
						drpTimeInterval.Items.Add(new ListItem(GestionWeb.GetWebWord(1572, customerWebSession.SiteLanguage),CstWeb.Repartition.timeInterval.Slice_22h_24h.GetHashCode().ToString()));
						drpTimeInterval.Items.Add(new ListItem(GestionWeb.GetWebWord(1573, customerWebSession.SiteLanguage),CstWeb.Repartition.timeInterval.Slice_24h_7h.GetHashCode().ToString()));						
						break;
					case ClassificationCst.DB.Vehicles.names.radio:	
						drpTimeInterval.Items.Add(new ListItem(GestionWeb.GetWebWord(1401, customerWebSession.SiteLanguage),CstWeb.Repartition.timeInterval.Total.GetHashCode().ToString()));
						drpTimeInterval.Items.Add(new ListItem(GestionWeb.GetWebWord(1562, customerWebSession.SiteLanguage),CstWeb.Repartition.timeInterval.Slice_5h_6h59.GetHashCode().ToString()));
						drpTimeInterval.Items.Add(new ListItem(GestionWeb.GetWebWord(1563, customerWebSession.SiteLanguage),CstWeb.Repartition.timeInterval.Slice_7h_8h59.GetHashCode().ToString()));
						drpTimeInterval.Items.Add(new ListItem(GestionWeb.GetWebWord(1565, customerWebSession.SiteLanguage),CstWeb.Repartition.timeInterval.Slice_9h_12h59.GetHashCode().ToString()));
						drpTimeInterval.Items.Add(new ListItem(GestionWeb.GetWebWord(1567, customerWebSession.SiteLanguage),CstWeb.Repartition.timeInterval.Slice_13h_18h59.GetHashCode().ToString()));
						drpTimeInterval.Items.Add(new ListItem(GestionWeb.GetWebWord(1571, customerWebSession.SiteLanguage),CstWeb.Repartition.timeInterval.Slice_19h_24h.GetHashCode().ToString()));
						break;
					default:						
						break;
				}
				drpTimeInterval.ID = "timeInterval_"+this.ID;
				drpTimeInterval.AutoPostBack=autoPostBackOption;
				drpTimeInterval.CssClass = cssClass;				
				try{
					drpTimeInterval.Items.FindByValue(customerWebSession.TimeInterval.GetHashCode().ToString()).Selected = true;
				}
				catch(System.Exception){
					drpTimeInterval.Items[0].Selected=true;
				}
				Controls.Add(drpTimeInterval);
			}
			#endregion
			
			#region choix d'un mois à étudier
			//Option choix d'un mois à étudier
			if(monthlyDateOption){
				monthlyDate = new DropDownList();				
				FillMonthlyDate(monthlyDate);
				monthlyDate.ID = "monthlyDate_"+this.ID;
				monthlyDate.AutoPostBack=autoPostBackOption;
				monthlyDate.CssClass = cssClass;				
				Controls.Add(monthlyDate);
			}
			#endregion

			#region choix d'une semaine
			//Option choix d'une semaine
			if(weeklyDateOption){
				weeklyDate = new DropDownList();				
				FillWeeklyDate();
				weeklyDate.ID = "weeklyDate_"+this.ID;
				weeklyDate.AutoPostBack=autoPostBackOption;
				weeklyDate.CssClass = cssClass;				
				Controls.Add(weeklyDate);
			}
			#endregion
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
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			
			if(titleOption){
				output.Write("\n<tr>");
				output.Write("\n<td>");
				//debut tableau titre
				output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
				output.Write("\n<tr>");
                output.Write("\n<td class=\"headerLeft\" colSpan=\"4\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\"></td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
                output.Write("\n<td style=\"HEIGHT: 14px\" vAlign=\"top\"><IMG height=\"12\" src=\"/App_Themes/" + themeName + "/Images/Common/block_fleche.gif\" width=\"12\"></td>");
                output.Write("\n<td style=\"HEIGHT: 14px\" width=\"1%\" background=\"/App_Themes/" + themeName + "/Images/Common/block_dupli.gif\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\" width=\"13\"></td>");
				output.Write("\n<td class=\"txtNoir11Bold\" style=\"PADDING-RIGHT: 5px; PADDING-LEFT: 5px; TEXT-TRANSFORM: uppercase; HEIGHT: 14px\" width=\"100%\">"+GestionWeb.GetWebWord(792,customerWebSession.SiteLanguage)+"</td>");
                output.Write("\n<td style=\"HEIGHT: 14px\" class=\"headerLeft\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\" width=\"1\"></td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td></td>");
                output.Write("\n<td class=\"headerLeft\" colSpan=\"3\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\"></td>");
				output.Write("\n</tr>");
				output.Write("\n</table>");
				//fin tableau titre
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

			//option choix d'une famille
			if (sectorListOption){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(1103,customerWebSession.SiteLanguage)+" : ");
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				sectorList.RenderControl(output);				
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}

			//option choix d'une centre d'interet
			if (interestCenterListOption){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(1576,customerWebSession.SiteLanguage)+" : ");
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				interestCenterList.RenderControl(output);				
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}
			//option unité
			if (unitOption){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(849,customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				list.RenderControl(output);								
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
				
			}
			//Option choix du format spots
			if (formatOption){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(1420,customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				format.RenderControl(output);				
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}
			//Option choix d'un jour nommé
			if (namedDayOption){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(1574,customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				namedDay.RenderControl(output);				
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}
			//Option choix d'une tranche horaire
			if (timeIntervalOption){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(1575,customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				drpTimeInterval.RenderControl(output);				
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}
			//Option choix d'un mois
			if (monthlyDateOption){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(1526,customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				monthlyDate.RenderControl(output);				
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}
			//Option choix d'une semaine
			if (weeklyDateOption){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				output.Write(GestionWeb.GetWebWord(1525,customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				weeklyDate.RenderControl(output);				
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
			//options PDM ,PDV,cumul date,pourcentage
			if (pdmOption || pdvOption || percentage || cumulPeriodOption){
				output.Write("\n<tr>");
				output.Write("\n<td>");
				if(cumulPeriodOption){					
					CumulPeriodCheckBox.RenderControl(output);
					output.Write("&nbsp;&nbsp;");
				}				
				if(pdmOption){
					PdmCheckBox.RenderControl(output);
					output.Write("&nbsp;&nbsp;");
				}	
				if(pdvOption){
					PdvCheckBox.RenderControl(output);
					output.Write("&nbsp;&nbsp;");
				}
				if(percentage){					
					percentageCheckBox.RenderControl(output);
					output.Write("&nbsp;&nbsp;");
					
				}
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}
			//Evolution
			if (evolutionOption){
				output.Write("\n<tr>");
				output.Write("\n<td>");				
					if (!customerWebSession.ComparativeStudy){
						EvolutionCheckBox.Enabled = false;
						EvolutionCheckBox.Checked = false;
					}
					EvolutionCheckBox.RenderControl(output);
					output.Write("&nbsp;&nbsp;");			
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}
			
			output.Write("\n</table>");
		}
		#endregion

		#endregion

		#region Méthodes internes	
		/// <summary>
		///Crée une liste de dates hebdomadaires	 
		/// </summary>					
		private void FillWeeklyDate(){
			//Variables
			 AtomicPeriodWeek currentPeriod;
			int year = int.Parse(customerWebSession.PeriodBeginningDate.ToString().Substring(0,4));
			int BeginningWeek=int.Parse(customerWebSession.PeriodBeginningDate.ToString().Substring(4,2));
			int EndWeek=int.Parse(customerWebSession.PeriodEndDate.ToString().Substring(4,2));;
			//Crée une liste de dates hebdomadaires																			
			string split="/";			
			string ItemValue="";
			weeklyDate.Items.Add(new ListItem("-------------------------------------------","0"));					
			for(int i=BeginningWeek;i<=EndWeek;i++){
				currentPeriod = new AtomicPeriodWeek(year,i);
				ItemValue=currentPeriod.Week.ToString().Length==1?currentPeriod.Year.ToString()+"0"+currentPeriod.Week.ToString() : currentPeriod.Year.ToString()+currentPeriod.Week.ToString();
				weeklyDate.Items.Add(new ListItem(GestionWeb.GetWebWord(124,customerWebSession.SiteLanguage)+"  "+ currentPeriod.FirstDay.Day.ToString()+split+currentPeriod.FirstDay.Month.ToString()+split+currentPeriod.FirstDay.Year.ToString()+"  "+GestionWeb.GetWebWord(125,customerWebSession.SiteLanguage)+"  "+currentPeriod.LastDay.Day.ToString()+split+currentPeriod.LastDay.Month.ToString()+split+currentPeriod.LastDay.Year.ToString(),ItemValue));										
			}
			if(customerWebSession.DetailPeriod==CstWeb.CustomerSessions.Period.DisplayLevel.weekly){
				try{
//					if(!Page.IsPostBack)weeklyDate.Items.FindByValue("0").Selected = true;	
//					else 
						weeklyDate.Items.FindByValue(customerWebSession.DetailPeriodBeginningDate).Selected = true;
				}
				catch(System.Exception){
					weeklyDate.Items.FindByValue("0").Selected = true;				
				}
			}
		}

		/// <summary>
		/// Crée une liste de dates mensuelles	
		/// </summary>
		/// <param name="monthlyDate">liste des mois</param>
		private void FillMonthlyDate(DropDownList monthlyDate){
			string ItemValue="";
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[customerWebSession.SiteLanguage].Localization);
 			int BeginningMonth=int.Parse(customerWebSession.PeriodBeginningDate.ToString().Substring(4,2));
			int EndMonth=int.Parse(customerWebSession.PeriodEndDate.ToString().Substring(4,2));;
			 monthlyDate.Items.Add(new ListItem("----------------","0"));							
			//liste des mois 
			 for(int i= BeginningMonth;i<=EndMonth;i++){	
			 ItemValue=i.ToString().Length==1?customerWebSession.PeriodBeginningDate.ToString().Substring(0,4)+"0"+i.ToString():customerWebSession.PeriodBeginningDate.ToString().Substring(0,4)+i.ToString();
				monthlyDate.Items.Add(new ListItem(MonthString.GetCharacters(i,cultureInfo,0),ItemValue));												
			}										
			if(customerWebSession.DetailPeriod==CstWeb.CustomerSessions.Period.DisplayLevel.monthly){
				try{
//					if(!Page.IsPostBack)monthlyDate.Items.FindByValue("0").Selected = true;	
//					else
						monthlyDate.Items.FindByValue(customerWebSession.DetailPeriodBeginningDate).Selected = true;
				}
				catch(System.Exception){
					monthlyDate.Items.FindByValue("0").Selected = true;				
				}
			}
		}
		/// <summary>
		/// Crée une liste de familles de produits
		/// </summary>
		/// <returns>liste de famille</returns>
		private void FillSectorList(){	
			
			#region variables locales
			ProductItemsList adexpressProductItemsList;
			IList excludeSector = null;
			string delimStr = ",";
			char [] delimiter = delimStr.ToCharArray();
			#endregion
			
			//Famille à exclure pour la presse
			if(customerWebSession.CurrentModule==CstWeb.Module.Name.TABLEAU_DE_BORD_PRESSE){
				adexpressProductItemsList=Product.GetItemsList(CstWeb.AdExpressUniverse.DASHBOARD_PRESS_EXCLUDE_PRODUCT_LIST_ID);
				if(adexpressProductItemsList.GetSectorItemsList.Length>0)excludeSector=adexpressProductItemsList.GetSectorItemsList.Split(delimiter);
			}
			//Liste des familles sélectionnables
			sectorList.Items.Add(new ListItem("-------------------------------------------","0"));	
			dsSectorList=ProductClassificationListDataAccess.GetListSectorSelected(customerWebSession);
			if(dsSectorList!=null && dsSectorList.Tables[0]!=null && dsSectorList.Tables[0].Rows.Count>0){
				foreach(DataRow dr in dsSectorList.Tables[0].Rows){
					if(excludeSector==null || (excludeSector!=null && excludeSector.Count>0 && !excludeSector.Contains(dr["id_sector"].ToString()))){	
						sectorList.Items.Add(new ListItem(dr["sector"].ToString(),dr["id_sector"].ToString()));	
					}
				}
			}								
			try{
				//currentSectors=customerWebSession.GetSelection(customerWebSession.SelectionUniversProduct,CustomerRightConstante.type.sectorAccess);					
				currentSectors = customerWebSession.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.SECTOR);	
				sectorList.Items.FindByValue(currentSectors).Selected = true;
			}
			catch(System.Exception){
				sectorList.Items.FindByValue("0").Selected = true;				
			}
			
		}

		/// <summary>
		/// Crée une liste de centre d'interet
		/// </summary>
		/// <returns>liste de de centre d'interet</returns>
		private void FillInterestCentreList(){			
			interestCenterList.Items.Add(new ListItem("-------------------------------------------","0"));	
			dsInterestCenterList=VehicleListDataAccess.getListInterestCenterSelected(customerWebSession);
			if(dsInterestCenterList!=null && dsInterestCenterList.Tables[0]!=null && dsInterestCenterList.Tables[0].Rows.Count>0){
				foreach(DataRow dr in dsInterestCenterList.Tables[0].Rows){
				interestCenterList.Items.Add(new ListItem(dr["INTEREST_CENTER"].ToString(),dr["id_INTEREST_CENTER"].ToString()));						
				}
			}								
			try{
//				if(Page.IsPostBack){
					currentInterestCenter=customerWebSession.GetSelection(customerWebSession.CurrentUniversMedia,CustomerRightConstante.type.interestCenterAccess);					
					interestCenterList.Items.FindByValue(currentInterestCenter).Selected = true;
//				}
			}
			catch(System.Exception){
				interestCenterList.Items.FindByValue("0").Selected = true;				
			}
			
		}
		
	
		/// <summary>
		/// Condition pour afficher les repartitions pour TV et Radio seulement
		/// </summary>
		/// <returns>false s'il doit être montrer, true sinon</returns>
		private bool showRepartition() {
			Int64 idVehicle = ((LevelInformation)customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID;
			ClassificationCst.DB.Vehicles.names vehicletype=(ClassificationCst.DB.Vehicles.names)idVehicle;
			switch(vehicletype) {
				case ClassificationCst.DB.Vehicles.names.tv:
				case ClassificationCst.DB.Vehicles.names.radio:
				case ClassificationCst.DB.Vehicles.names.others:
					return(true);
				default:
					return(false);
			}
		}		

		/// <summary>
		/// verifie si la nomenclature est détaillée au niveau d'un l'élément
		/// </summary>
		/// <param name="tree">éléments sélectionnés</param>
		/// <returns>vraie si la nomenclature est détaillée et faux sinon</returns>
        private static bool isDetailElement(System.Windows.Forms.TreeNode tree)
        {		 	
			return!(tree==null || tree.Nodes==null 
				|| tree.Nodes.Count==0);				
		}
		
		#endregion
			
	}
}
