#region Namespace
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Controls.Results;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Classification.Universe;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using CstDB = TNS.AdExpress.Constantes.DB;
using CustomerCst = TNS.AdExpress.Constantes.Customer.Right;
using FrameWorkResults = TNS.AdExpress.Constantes.FrameWork.Results;
using ProductList = TNS.AdExpress.Web.DataAccess.Selections.Products.ProductListDataAccess;
using SessionCst = TNS.AdExpress.Constantes.Web.CustomerSessions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Controls.Selections;
using TNS.AdExpressI.Date;
using TNS.AdExpress.Domain.Layers;
using System.Reflection;
using ConstantePeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
#endregion

namespace TNS.AdExpress.Web.Controls.Headers {
    /// <summary>
    /// Composant affichant le titre  et le descriptif de la page
    /// </summary>
    [ToolboxData("<{0}:ResultsOptionsWebControl runat=server></{0}:ResultsOptionsWebControl>")]
    public class ResultsOptionsWebControl : System.Web.UI.WebControls.WebControl {

        #region Variables
        /// <summary>
        /// Force le composant à montrer les niveaux de détails par accroches
        /// </summary>
        protected bool _forceMediaDetailForSlogan = false;
        /// <summary>
        /// Titre du graphique
        /// </summary>
        protected string _chartTitle = "";
        /// <summary>
        /// Titre du tableau
        /// </summary>
        protected string _tableTitle = "";
        /// <summary>
        /// Show segment flag
        /// </summary>
        private bool _showSegment = false;
        /// <summary>
        /// If mutualExclusion == true we need to add CheckBoxsMutualExclusion control
        /// </summary>
        private bool _mutualExclusion = false;
        /// <summary>
        /// If dependentSelection == true we allow a checkbox list to be selectionable only if the reference one is checked
        /// </summary>
        private bool _dependentSelection = false;
        #endregion

        #region Variables MMI
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
        /// Checkbox dédiée à l'auto-promo Evaliant
        /// </summary>
        protected System.Web.UI.WebControls.CheckBox AutopromoEvaliantCheckBox;
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
        /// Total comparaison choice for AS
        /// </summary>
        protected System.Web.UI.WebControls.RadioButtonList _totalChoiceRadioButtonList;
        /// <summary>
        /// Initialisation des éléments de référence
        /// </summary>
        protected TNS.AdExpress.Web.Controls.Headers.InitializeProductWebControl _initializeProductWebControl;
        /// <summary>
        /// Initialisation des éléments média
        /// </summary>
        protected TNS.AdExpress.Web.Controls.Headers.InitializeMediaWebControl _initializeMediaWebControl;
        /// <summary>
        /// Generic Column Level Detail Selection WebControl
        /// </summary>
        protected TNS.AdExpress.Web.Controls.Headers.GenericColumnLevelDetailSelectionWebControl _genericColumnLevelDetailSelectionWebControl;
        /// <summary>
        /// Generic Media Level Detail Selection WebControl
        /// </summary>
        protected TNS.AdExpress.Web.Controls.Headers.GenericMediaLevelDetailSelectionWebControl _genericMediaLevelDetailSelectionWebControl;
        /// <summary>
        /// Period Detail WebControl
        /// </summary>
        protected TNS.AdExpress.Web.Controls.Headers.PeriodDetailWebControl _periodDetailWebControl;
        /// <summary>
        /// Sector WebControl
        /// </summary>
        protected TNS.AdExpress.Web.Controls.Headers.SectorWebControl _sectorWebControl;
        /// <summary>
        /// Results Table Types WebControl
        /// </summary>
        protected TNS.AdExpress.Web.Controls.Headers.ResultsTableTypesWebControl _resultsTableTypesWebControl;
        /// <summary>
        /// Detail Advertiser Brand Product WebControl
        /// </summary>
        protected TNS.AdExpress.Web.Controls.Headers.DetailWebControl _detailAdvertiserBrandProductWebControl;
        /// <summary>
        /// Selected Media universe
        /// </summary>
        protected MediaItemsList _selectedMediaUniverse;
        /// <summary>
        /// Zoom Graphic CheckBox
        /// </summary>
        protected System.Web.UI.WebControls.CheckBox _zoomGraphicCheckBox;
        /// <summary>
        /// Web control used to handle mutual exclusion between a list of checkboxs
        /// </summary>
        protected CheckBoxsMutualExclusion _checkBoxsMutualExclusion;
        /// <summary>
        /// Web control used to allow a checkbox list to be selectionable only if the reference one is checked
        /// </summary>
        protected CheckBoxsDependentSelection _checkBoxsDependentSelection;
        /// <summary>
        /// CheckBox to indicate a comparative study
        /// </summary>
        protected System.Web.UI.WebControls.CheckBox _comparativeStudyCheckBox;
        /// <summary>
        /// CheckBox to indicate a comparative study
        /// </summary>
        protected DateComparativeSelection _dateComparativeSelection;
        /// <summary>
        /// CheckBox to indicate a comparative study
        /// </summary>
        protected System.Web.UI.WebControls.Label _dateComparativeSelectionLabel;
        #endregion

        #region Accessors
        /// <summary>
        /// Session du client (utile pour la langue)
        /// </summary>
        protected WebSession customerWebSession = null;
        /// <summary>Session du client</summary>
        public WebSession CustomerWebSession {
            get { return customerWebSession; }
            set { customerWebSession = value; }
        }

        /// <summary>
        /// CssClass générale 
        /// </summary>
        [Bindable(true), DefaultValue("txtNoir11Bold"),
        Description("Option choix de l'unité")]
        protected string cssClass = "txtNoir11Bold";
        /// <summary>CSS</summary>
        public string CommonCssClass {
            get { return cssClass; }
            set { cssClass = value; }
        }

        /// <summary>
        /// Option unité
        /// </summary>
        [Bindable(true),
        Description("Option choix de l'unité")]
        protected bool unitOption = true;
        /// <summary>Option d'unité</summary>
        public bool UnitOption {
            get { return unitOption; }
            set { unitOption = value; }
        }

        /// <summary>
        /// Option unité Appm
        /// </summary>		
        [Bindable(true),
        Description("Option choix de l'unité Appm")]
        protected bool unitOptionAppm = false;
        /// <summary>Option unité Appm</summary>
        public bool UnitOptionAppm {
            get { return unitOptionAppm; }
            set { unitOptionAppm = value; }
        }

        /// <summary>
        /// Option products
        /// </summary>	
        [Bindable(true),
        Description("Option list of products")]
        protected bool productsOption = false;
        /// <summary>list products Appm</summary>
        public bool ProductsOption {
            get { return productsOption; }
            set { productsOption = value; }
        }

        /// <summary>
        /// Option encart
        /// </summary>
        [Bindable(true), Description("Option choix d'un encart")]
        protected bool insertOption = false;
        /// <summary>
        /// Propriété encart
        /// </summary>
        public bool InsertOption {
            get { return insertOption; }
            set { insertOption = value; }
        }

        /// <summary>
        /// Option PDM
        /// </summary>
        [Bindable(true),
        Description("Option visualiser la PDM")]
        protected bool pdmOption = false;
        /// <summary></summary>
        public bool PdmOption {
            get { return pdmOption; }
            set { pdmOption = value; }
        }

        /// <summary>
        /// Option PDV
        /// </summary>
        [Bindable(true),
        Description("Option visualiser la PDV")]
        protected bool pdvOption = false;
        /// <summary></summary>
        public bool PdvOption {
            get { return pdvOption; }
            set { pdvOption = value; }
        }

        /// <summary>
        /// Option Evolution
        /// </summary>
        [Bindable(true),
        Description("Option visualiser une évolution")]
        protected bool evolutionOption = false;
        /// <summary></summary>
        public bool EvolutionOption {
            get { return evolutionOption; }
            set { evolutionOption = value; }
        }

        /// <summary>
        /// Option de visualisation des éléments concurrents  et references seulement
        /// </summary>
        [Bindable(true),
        Description("Option : Visualiser uniquement les éléments de références et concurrents")]
        protected bool personalizedElementsOption = false;
        /// <summary></summary>
        public bool PersonalizedElementsOption {
            get { return personalizedElementsOption; }
            set { personalizedElementsOption = value; }
        }

        /// <summary>
        /// Option Auto-promo Evaliant
        /// </summary>
        [Bindable(true),
        Description("Option Auto-promo Evaliant")]
        protected bool autopromoEvaliantOption = false;
        /// <summary></summary>
        public bool AutopromoEvaliantOption {
            get { return autopromoEvaliantOption; }
            set { autopromoEvaliantOption = value; }
        }

        /// <summary>Option pourcentage</summary>
        public System.Web.UI.WebControls.CheckBox PercentageCheckBox {
            get { return percentageCheckBox; }
            set { percentageCheckBox = value; }
        }

        /// <summary>
        /// Option niveau de détail produit
        /// </summary>
        [Bindable(true), DefaultValue(false),
        Description("Option choix du niveau de détail produit")]
        protected bool productDetailOption = false;
        /// <summary></summary>
        public bool ProductDetailOption {
            get { return productDetailOption; }
            set { productDetailOption = value; }
        }

        /// <summary>
        /// Option niveau de détail media
        /// </summary>
        [Bindable(true), DefaultValue(false),
        Description("Option choix niveau de détail media")]
        protected bool mediaDetailOption = false;
        /// <summary></summary>
        public bool MediaDetailOption {
            get { return mediaDetailOption; }
            set { mediaDetailOption = value; }
        }

        /// <summary>
        /// Option choix de tableau préformaté
        /// </summary>
        [Bindable(true), DefaultValue(false),
        Description("Option choix de tableau préformaté")]
        protected bool tblChoiceOption = false;
        /// <summary></summary>
        public bool PreformatedTableOption {
            get { return tblChoiceOption; }
            set { tblChoiceOption = value; }
        }

        /// <summary>
        /// Autopostback Vrai par défaut
        /// </summary>
        [Bindable(true),
        Description("autoPostBack")]
        protected bool autoPostBackOption = true;
        /// <summary></summary>
        public bool AutoPostBackOption {
            get { return autoPostBackOption; }
            set { autoPostBackOption = value; }
        }

        /// <summary>
        ///Option pdm
        /// </summary>
        [Bindable(false),
        Description("pdm")]
        protected bool percentage = false;
        /// <summary>Affiche en %</summary>
        public bool Percentage {
            get { return percentage; }
            set { percentage = value; }
        }

        /// <summary>
        /// Option resultat
        /// </summary>
        [Bindable(true),
        Description("Option type de résultat")]
        protected bool resultOption = true;
        /// <summary>Type de résultat</summary>
        public bool ResultOption {
            get { return resultOption; }
            set { resultOption = value; }
        }

        /// <summary>
        /// Option resultat
        /// </summary>
        [Bindable(true),
        Description("Option type de résultat")]
        protected bool forceMediaDetailForMediaPlan = false;
        /// <summary></summary>
        public bool ForceMediaDetailForMediaPlan {
            get { return forceMediaDetailForMediaPlan; }
            set { forceMediaDetailForMediaPlan = value; }
        }

        /// <summary>
        /// Force l'accès aux accroches dans le niveau de détail support
        /// </summary>
        [Bindable(true),
        Description("Option type de résultat")]
        public bool ForceMediaDetailForSlogan {
            get { return _forceMediaDetailForSlogan; }
            set { _forceMediaDetailForSlogan = value; }
        }

        /// <summary>
        /// Format de résultat
        /// </summary>		
        [Bindable(true),
        Description("Option format présentation résultat")]
        protected bool _resultFormat = false;
        /// <summary>
        ///Format de résultat 
        /// </summary>
        public bool ResultFormat {
            get { return _resultFormat; }
            set { _resultFormat = value; }
        }

        /// <summary>
        /// Option de calcul de comparaison Total
        /// </summary>		
        [Bindable(true),
        Description("Option de calcul de comparaison Total")]
        protected bool _totalChoice = false;
        /// <summary>
        /// Option de calcul de comparaison Total 
        /// </summary>
        public bool TotalChoice {
            get { return _totalChoice; }
            set { _totalChoice = value; }
        }

        /// <summary>
        /// Show Zoom graphic option
        /// </summary>		
        [Bindable(true),
        Description("Show Zoom graphic option")]
        protected bool _zoomGraphic = false;
        /// <summary>
        ///Set/Get Show Zoom graphic option 
        /// </summary>
        public bool ZoomGraphic {
            get { return _zoomGraphic; }
            set { _zoomGraphic = value; }
        }

        /// <summary>
        /// Get Zoom Graphic status
        /// </summary>
        public bool IsZoomGraphicChecked {
            get {
                if(_zoomGraphicCheckBox != null && _zoomGraphicCheckBox.Visible) return (_zoomGraphicCheckBox.Checked);
                return (false);
            }
        }

        /// <summary>
        ///Titre graphique de résultat 
        /// </summary>
        public string ChartTitle {
            get { return _chartTitle; }
            set { _chartTitle = value; }
        }

        /// <summary>
        ///Titre tableau de résultat 
        /// </summary>
        public string TableTitle {
            get { return _tableTitle; }
            set { _tableTitle = value; }
        }

        /// <summary>
        ///Contrôle Choix du type de résultat sous forme graphique
        /// </summary>
        public System.Web.UI.WebControls.RadioButton GraphRadioButton {
            get { return graphRadioButton; }
            set { graphRadioButton = value; }
        }

        /// <summary>
        ///Contrôle Choix du type de résultat sous forme tableau
        /// </summary>
        public System.Web.UI.WebControls.RadioButton TableRadioButton {
            get { return tableRadioButton; }
            set { tableRadioButton = value; }
        }

        /// <summary>
        /// Initialisation des éléments de références
        /// </summary>
        public TNS.AdExpress.Web.Controls.Headers.InitializeProductWebControl InitializeProductWebControl {
            get { return _initializeProductWebControl; }
            set { _initializeProductWebControl = value; }
        }
        /// <summary>
        /// Initialisation des éléments média
        /// </summary>
        public TNS.AdExpress.Web.Controls.Headers.InitializeMediaWebControl InitializeMediaWebControl {
            get { return _initializeMediaWebControl; }
            set { _initializeMediaWebControl = value; }
        }

        /// <summary>
        /// Option initialisation des annonceurs de concurrents
        /// </summary>
        [Bindable(true),
        Description("Option initialisation des annonceurs de concurrents")]
        protected bool _inializeAdvertiserOption = false;
        /// <summary>Option initialisation des annonceurs de concurrents</summary>
        public bool InializeAdVertiserOption {
            get { return _inializeAdvertiserOption; }
            set { _inializeAdvertiserOption = value; }
        }
        /// <summary>
        /// Option initialisation des produits
        /// </summary>
        [Bindable(true),
        Description("Option initialisation des produits")]
        protected bool _initializeProductOption = false;
        /// <summary>Option initialisation des produits</summary>
        public bool InializeProductOption {
            get { return _initializeProductOption; }
            set { _initializeProductOption = value; }
        }
        /// <summary>
        /// Option initialisation des slogans
        /// </summary>
        [Bindable(true),
        Description("Option initialisation des slogans")]
        protected bool _inializeSlogansOption = false;
        /// <summary>Option initialisation des slogans</summary>
        public bool InializeSlogansOption {
            get { return _inializeSlogansOption; }
            set { _inializeSlogansOption = value; }
        }
        /// <summary>
        /// Option initialisation des supports
        /// </summary>
        [Bindable(true),
        Description("Option initialisation des supports")]
        protected bool _initializeMediaOption = false;
        /// <summary></summary>
        public bool InializeMediaOption {
            get { return _initializeMediaOption; }
            set { _initializeMediaOption = value; }
        }

        /// <summary>
        /// Option type de pourcentage (horizontal ou vertical)
        /// </summary>
        [Bindable(true),
        Description("Option type de pourcentage (horizontal ou vertical)")]
        protected bool _percentageTypeOption = false;
        /// <summary>Option type de pourcentage (horizontal ou vertical)</summary>
        public bool PercentageTypeOption {
            get { return _percentageTypeOption; }
            set { _percentageTypeOption = value; }
        }

        /// <summary>Contrôle Choix du type de pourcentage (horizontal ou vertical)</summary>
        [Bindable(true),
        Description("Contrôle choix  type de pourcentage (horizontal ou vertical)")]
        public System.Web.UI.WebControls.DropDownList PercentageTypeDropDownList {
            get { return _percentageTypeDropDownList; }
            set { _percentageTypeDropDownList = value; }
        }

        /// <summary>
        /// Selected media universe
        /// </summary>
        public MediaItemsList SelectedMediaUniverse {
            get { return _selectedMediaUniverse; }
            set { _selectedMediaUniverse = value; }
        }

        /// <summary>
        /// Period Detail Options
        /// </summary>		
        [Bindable(true),
        Description("Period Detail Options")]
        protected bool _periodDetailOptions = false;
        /// <summary>
        /// Get/Set Period Detail Options
        /// </summary>
        public bool PeriodDetailOptions {
            get { return _periodDetailOptions; }
            set { _periodDetailOptions = value; }
        }

        /// <summary>
        /// Sector Selection Options
        /// </summary>
        [Bindable(true),
        Description("Sector Selection Options")]
        protected bool _sectorSelectionOptions = false;
        /// <summary>
        /// Get/Set Sector Selection Options
        /// </summary>
        public bool SectorSelectionOptions {
            get { return _sectorSelectionOptions; }
            set { _sectorSelectionOptions = value; }
        }

        /// <summary>
        /// Results Table Types Options
        /// </summary>
        [Bindable(true),
        Description("Results Table Types Options")]
        protected bool _resultsTableTypesOptions = false;
        /// <summary>
        /// Get/Set Results Table Types Options
        /// </summary>
        public bool ResultsTableTypesOptions {
            get { return _resultsTableTypesOptions; }
            set { _resultsTableTypesOptions = value; }
        }

        /// <summary>
        /// Detail Advertiser Brand Product Options
        /// </summary>
        [Bindable(true),
        Description("Detail Advertiser Brand Product Options")]
        protected bool _detailAdvertiserBrandProductOptions = false;
        /// <summary>
        /// Get/Set Detail Advertiser Brand Product Options
        /// </summary>
        public bool DetailAdvertiserBrandProductOptions {
            get { return _detailAdvertiserBrandProductOptions; }
            set { _detailAdvertiserBrandProductOptions = value; }
        }

        /// <summary>
        /// Get / Set Mutual Exclusion option
        /// </summary>
        public bool MutualExclusion {
            get { return _mutualExclusion; }
            set { _mutualExclusion = value; }
        }

        /// <summary>
        /// Get / Set Dependent Selection option
        /// </summary>
        public bool DependentSelection
        {
            get { return _dependentSelection; }
            set { _dependentSelection = value; }
        }

        /// <summary>
        /// Comparative Study Option
        /// </summary>
        [Bindable(true),
        Description("Comparative Study Option")]
        protected bool comparativeStudyOption = false;
        /// <summary></summary>
        public bool ComparativeStudyOption
        {
            get { return comparativeStudyOption; }
            set { comparativeStudyOption = value; }
        }

        /// <summary>
        /// Comparative Study Date Type Option (use when ComparativeStudyOption is used)
        /// </summary>
        [Bindable(true),
        Description("Comparative Study Date Type Option")]
        protected bool comparativeStudyDateTypeOption = false;
        /// <summary></summary>
        public bool ComparativeStudyDateTypeOption {
            get { return comparativeStudyDateTypeOption; }
            set { comparativeStudyDateTypeOption = value; }
        }

        #region Propriétés de TblChoice
        /// <summary>
        /// hauteur de l'image
        /// </summary>
        [Bindable(true), Category("Appearance")]
        private double imageHeight = 15.0;
        /// <summary></summary>
        public double ImageHeight {
            get { return imageHeight; }
            set { imageHeight = value; }
        }

        /// <summary>
        /// largeur de l'image
        /// </summary>
        [Bindable(true), Category("Appearance")]
        private double imageWidth = 15.0;
        /// <summary></summary>
        public double ImageWidth {
            get { return imageWidth; }
            set { imageWidth = value; }
        }

        /// <summary>
        /// largeur des bordures
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(1.0)]
        private double borderWidth = 1.0;
        /// <summary></summary>
        public new double BorderWidth {
            get { return borderWidth; }
            set { borderWidth = Math.Max(0, value); }
        }

        /// <summary>
        /// option afficher images
        /// </summary>
        [Bindable(true), DefaultValue(true)]
        protected bool pictShow = true;
        /// <summary></summary>
        public bool ShowPictures {
            get { return pictShow; }
            set { pictShow = value; }
        }
        /// <summary>
        ///feuille de style 
        /// </summary>
        [Bindable(true), DefaultValue("ddlOut")]
        protected string outCssClass = "ddlOut";
        /// <summary></summary>
        public string OutCssClass {
            get { return outCssClass; }
            set { outCssClass = value; }
        }

        /// <summary>
        ///feuille de style pour l'élément en roll-over 
        /// </summary>
        [Bindable(true), DefaultValue("ddlOver")]
        protected string overCssClass = "ddlOver";
        /// <summary></summary>
        public string OverCssClass {
            get { return overCssClass; }
            set { overCssClass = value; }
        }

        /// <summary>
        /// liste des éléments
        /// </summary>
        [Bindable(true), DefaultValue("")]
        private string texts = "";
        /// <summary></summary>
        public string List {
            get { return texts; }
            set { texts = value; }
        }

        /// <summary>
        /// liste des unités APPM
        /// </summary>
        [Bindable(true), DefaultValue("")]
        private string textsAppm = "";
        /// <summary> liste des unités APPM</summary>
        public string ListUnitAppm {
            get { return textsAppm; }
            set { textsAppm = value; }
        }

        /// <summary>
        ///noms des images 
        /// </summary>
        [Bindable(true), DefaultValue("")]
        private string images = "";
        /// <summary></summary>
        public string Images {
            get { return images; }
            set { images = value; }
        }

        /// <summary>
        /// index liste
        /// </summary>
        [Bindable(true), DefaultValue(0)]
        private int index;
        /// <summary></summary>
        public int ListIndex {
            get { return index; }
            set { index = Math.Min(value, texts.Split('|').Length - 1); }
        }
        #endregion

        #region GenericColumnLevelDetailSelectionWebControl
        /// <summary>
        /// Generic Column Level Detail Selection Options
        /// </summary>		
        [Bindable(true),
        Description("Generic Column Level Detail Selection Options")]
        protected bool _GenericColumnLevelDetailSelectionOptions = false;
        /// <summary>
        /// Generic Column Level Detail Selection Options
        /// </summary>
        public bool GenericColumnLevelDetailSelectionOptions {
            get { return _GenericColumnLevelDetailSelectionOptions; }
            set { _GenericColumnLevelDetailSelectionOptions = value; }
        }

        /// <summary>
        /// Generic Column Detail Level Type
        /// </summary>
        protected WebConstantes.GenericDetailLevel.Type _genericColumnDetailLevelType;
        /// <summary>
        /// Obtient ou définit le type des niveaux de détail colonne
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        Description("Type des niveaux de détail colonne")]
        public WebConstantes.GenericDetailLevel.Type GenericColumnDetailLevelType {
            get { return (_genericColumnDetailLevelType); }
            set { _genericColumnDetailLevelType = value; }
        }

        /// <summary>
        /// Nb Column Detail Level Item List
        /// </summary>
        protected int _nbColumnDetailLevelItemList = 1;
        /// <summary>
        /// Obtient ou définit le nombre de niveaux de détaille personnalisés
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue("4")]
        public int NbColumnDetailLevelItemList {
            get { return (_nbColumnDetailLevelItemList); }
            set {
                if(value < 1 || value > 1) throw (new ArgumentOutOfRangeException("The value of NbDetailLevelItemList must be between 1 and 4"));
                _nbColumnDetailLevelItemList = value;
            }
        }
        #endregion

        #region GenericMediaLevelDetailSelectionWebControl
        /// <summary>
        /// Generic Media Level Detail Selection Options
        /// </summary>		
        [Bindable(true),
        Description("Generic Media Level Detail Selection Options")]
        protected bool _GenericMediaLevelDetailSelectionOptions = false;
        /// <summary>
        /// Generic Media Level Detail Selection Options
        /// </summary>
        public bool GenericMediaLevelDetailSelectionOptions {
            get { return _GenericMediaLevelDetailSelectionOptions; }
            set { _GenericMediaLevelDetailSelectionOptions = value; }
        }

        /// <summary>
        /// Generic Detail Level Type
        /// </summary>
        protected WebConstantes.GenericDetailLevel.Type _genericDetailLevelType;
        /// <summary>
        /// Obtient ou définit le type des niveaux de détail
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        Description("Type des niveaux de détail")]
        public WebConstantes.GenericDetailLevel.Type GenericDetailLevelType {
            get { return (_genericDetailLevelType); }
            set { _genericDetailLevelType = value; }
        }

        ///<summary>
        /// Profile du composant
        /// </summary>
        protected WebConstantes.GenericDetailLevel.ComponentProfile _componentProfile;
        /// <summary>
        /// Obtient ou définit Le profile du coposant
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        Description("Profile du composant"),
        DefaultValue("media")
        ]
        public WebConstantes.GenericDetailLevel.ComponentProfile GenericDetailLevelComponentProfile {
            get { return (_componentProfile); }
            set { _componentProfile = value; }
        }

        /// <summary>
        /// Page permettant de sauvegarer le niveaux de détail
        /// </summary>
        protected string _removeASPXFilePath = string.Empty;
        /// <summary>
        /// Obtient ou définit la Page permettant de sauvegarer le niveaux de détail
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue("test.aspx"),
        Description("Page permettant de supprimer un niveaux de détail sauvegardé")]
        public string RemoveASPXFilePath {
            get { return (_removeASPXFilePath); }
            set { _removeASPXFilePath = value; }
        }

        /// <summary>
        /// Page permettant de sauvegarer le niveaux de détail
        /// </summary>
        protected string _saveASPXFilePath = string.Empty;
        /// <summary>
        /// Obtient ou définit la Page permettant de sauvegarer le niveaux de détail
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue("test.aspx"),
        Description("Page permettant de sauvegarer le niveaux de détail")]
        public string SaveASPXFilePath {
            get { return (_saveASPXFilePath); }
            set { _saveASPXFilePath = value; }
        }
        
        /// <summary>
        /// Nb Detail Level Item List
        /// </summary>
        protected int _nbDetailLevelItemList = 1;
        /// <summary>
        /// Obtient ou définit le nombre de niveaux de détaille personnalisés
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue("4")]
        public int NbDetailLevelItemList {
            get { return (_nbDetailLevelItemList); }
            set {
                if(value < 1 || value > 4) throw (new ArgumentOutOfRangeException("The value of NbDetailLevelItemList must be between 1 and 4"));
                _nbDetailLevelItemList = value;
            }
        }
        #endregion

        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public ResultsOptionsWebControl()
            : base() {
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

            if (customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {
                comparativeStudyOption = comparativeStudyOption && WebApplicationParameters.UseComparativeMediaSchedule;
                comparativeStudyDateTypeOption = comparativeStudyDateTypeOption && WebApplicationParameters.UseComparativeMediaSchedule;
            }

            #region Options Initialisation des éléments de référence
            _initializeProductWebControl = new TNS.AdExpress.Web.Controls.Headers.InitializeProductWebControl();
            _initializeProductWebControl.CustomerWebSession = customerWebSession;
            _initializeProductWebControl.AutoPostBackOption = this.autoPostBackOption;
            _initializeProductWebControl.EnableViewState = true;
            _initializeProductWebControl.CommonCssClass = "txtInitialize";
            _initializeProductWebControl.initializeAdvertiserCheckBox.EnableViewState = true;
            _initializeProductWebControl.InitializeAdvertiser = InializeAdVertiserOption;
            _initializeProductWebControl.initializeProductCheckBox.EnableViewState = true;
            _initializeProductWebControl.InitializeProduct = InializeProductOption;
            _initializeProductWebControl.InitializeSlogans = InializeSlogansOption;
            _initializeProductWebControl.ID = this.ID + "_initializeAdvertiser";
            Controls.Add(_initializeProductWebControl);
            #endregion

            #region Options Initialisation des éléments média
            _initializeMediaWebControl = new TNS.AdExpress.Web.Controls.Headers.InitializeMediaWebControl();
            _initializeMediaWebControl.CustomerWebSession = customerWebSession;
            _initializeMediaWebControl.AutoPostBackOption = this.autoPostBackOption;
            _initializeMediaWebControl.EnableViewState = true;
            _initializeMediaWebControl.CommonCssClass = "txtInitialize";
            _initializeMediaWebControl.InitializeMedia = InializeMediaOption;
            _initializeMediaWebControl.ID = this.ID + "_initializeMediaWebControl";
            Controls.Add(_initializeMediaWebControl);
            #endregion

            #region Options Period Detail
            if(PeriodDetailOptions) {
                _periodDetailWebControl = new PeriodDetailWebControl();
                _periodDetailWebControl.Session = customerWebSession;
                _periodDetailWebControl.LanguageCode = customerWebSession.SiteLanguage;
                _periodDetailWebControl.ID = this.ID + "_periodDetailWebControl";
                _periodDetailWebControl.SkinID = "PeriodDetailWebControl";
                Controls.Add(_periodDetailWebControl);

                if(Page.Request.QueryString.Get("zoomDate") != null
                    && Page.Request.QueryString.Get("zoomDate") != string.Empty) {
                    _periodDetailWebControl.Visible = false;
                }
                else {
                    _periodDetailWebControl.Visible = true;
                    customerWebSession.DetailPeriod = _periodDetailWebControl.SelectedValue;

                    if(customerWebSession.DetailPeriod == TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly) {
                        if(TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(customerWebSession.PeriodBeginningDate, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate)
                            < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3)) {
                            customerWebSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly;
                        }
                    }
                }

                if(!Page.IsPostBack) {
                    _periodDetailWebControl.Select(customerWebSession.DetailPeriod);
                }
                else {
                    customerWebSession.DetailPeriod = _periodDetailWebControl.SelectedValue;
                }
            }
            #endregion

            #region IsPostBack
            if(Page.IsPostBack) {
                if(unitOption) {
                    try {
                        SessionCst.Unit unitSelected = (SessionCst.Unit)Int64.Parse(Page.Request.Form.GetValues("_units")[0]);
                        if(customerWebSession.Unit != unitSelected) customerWebSession.Unit = unitSelected;
                    }
                    catch(SystemException) { }
                }

                if(insertOption && WebApplicationParameters.AllowInsetOption) {
                    if(Page.Request.Form.GetValues("_inserts") != null) {
                        customerWebSession.Insert = (SessionCst.Insert)Int64.Parse(Page.Request.Form.GetValues("_inserts")[0]);
                    }
                }
                if(resultOption) {
                    Int64 tabSelected = Int64.Parse(Page.Request.Form.GetValues("_resultsPages")[0]);
                    if(customerWebSession.CurrentTab != tabSelected)
                        customerWebSession.CurrentTab = tabSelected;
                }

                if(percentage) {
                    try {
                        if(Page.Request.Form.GetValues("_percentage")[0] != null) customerWebSession.Percentage = true;
                    }
                    catch(System.Exception) { customerWebSession.Percentage = false; }
                }
                if(pdmOption) {
                    try {
                        if(Page.Request.Form.GetValues(this.ID + "_pdm")[0] != null) customerWebSession.PDM = true;
                    }
                    catch(System.Exception) {
                        customerWebSession.PDM = false;
                    }
                }
                if(pdvOption) {
                    try {
                        if(Page.Request.Form.GetValues(this.ID + "_pdv")[0] != null) customerWebSession.PDV = true;
                    }
                    catch(System.Exception) {
                        customerWebSession.PDV = false;
                    }
                }
                if(evolutionOption) {
                    try {
                        if(Page.Request.Form.GetValues(this.ID + "_evol")[0] != null) customerWebSession.Evolution = true;
                    }
                    catch(System.Exception) {
                        customerWebSession.Evolution = false;
                    }
                }
                if (comparativeStudyOption)
                {
                    try {
                        if (Page.Request.Form.GetValues(this.ID + "_comparativeStudy")[0] != null) customerWebSession.ComparativeStudy = true;
                        if (comparativeStudyDateTypeOption) {
                            customerWebSession.ComparativePeriodType = (TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType)Enum.Parse(typeof(TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType), Page.Request.Form["selectionType_" + this.ID + "_comparativeStudyDateTypeOption"].ToString());
                        }
                    }
                    catch (System.Exception) {
                        customerWebSession.ComparativeStudy = false;
                    }
                    if (!customerWebSession.ComparativeStudy) {
                        customerWebSession.ComparativePeriodType = TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType.dateToDate;
                    }
                }
                if(personalizedElementsOption) {
                    try {
                        if(Page.Request.Form.GetValues(this.ID + "_perso")[0] != null && (Page.Request.Form.GetValues("_initializeAdvertiser") == null)
                            ) customerWebSession.PersonalizedElementsOnly = true;
                        else {
                            customerWebSession.PersonalizedElementsOnly = false;
                            if(PersonalizedElementsCheckBox != null) PersonalizedElementsCheckBox.Checked = false;
                        }
                    }
                    catch(System.Exception) {
                        customerWebSession.PersonalizedElementsOnly = false;
                    }
                }
                if(autopromoEvaliantOption) {
                    try {
                        if(Page.Request.Form.GetValues(this.ID + "_autopromoEvaliant")[0] != null) customerWebSession.AutopromoEvaliant = true;
                    }
                    catch(System.Exception) {
                        customerWebSession.AutopromoEvaliant = false;
                    }
                }

                if(tblChoiceOption) {
                    customerWebSession.PreformatedTable = (SessionCst.PreformatedDetails.PreformatedTables)Int64.Parse(Page.Request.Form.GetValues("DDL" + this.ID)[0]);
                }
                if(productDetailOption)
                    try {
                        SessionCst.PreformatedDetails.PreformatedProductDetails detailSelected = (SessionCst.PreformatedDetails.PreformatedProductDetails)int.Parse(Page.Request.Form.GetValues("productDetail_" + this.ID)[0]);
                        if(customerWebSession.PreformatedProductDetail != detailSelected)
                            customerWebSession.PreformatedProductDetail = detailSelected;
                    }
                    catch(System.Exception) { }
                if(mediaDetailOption)
                    try {
                        customerWebSession.PreformatedMediaDetail = (SessionCst.PreformatedDetails.PreformatedMediaDetails)int.Parse(Page.Request.Form.GetValues("mediaDetail_" + this.ID)[0]);
                    }
                    catch(System.Exception) { }

                // Sauvegarde du type d'alignement des résultats en pourcentage lorsque la page est publiée
                if(_percentageTypeOption) {
                    try {
                        //int productID=Convert.ToInt32(products.SelectedItem.Value);
                        int percentageTypeID = Convert.ToInt32(Page.Request.Form.GetValues("_percentageTypePercentageDropDownList")[0]);
                        if(customerWebSession.PreformatedTable == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units
                            && (WebConstantes.Percentage.Alignment)percentageTypeID == WebConstantes.Percentage.Alignment.horizontal) {
                            customerWebSession.PercentageAlignment = WebConstantes.Percentage.Alignment.none;
                        }
                        else customerWebSession.PercentageAlignment = (WebConstantes.Percentage.Alignment)percentageTypeID;
                    }
                    catch(SystemException) { }
                }
            }
            #endregion

            #region Evolution For 'Manadataires' module
            /* For 'Mandataires' module the evolution option is false by default,
             * so we need to initialize it to false
             * */
            if (_dependentSelection && !customerWebSession.ComparativeStudy)
                customerWebSession.Evolution = false;
            #endregion

            #region Options format du résultat (graphique ou tableau)
            graphRadioButton = new System.Web.UI.WebControls.RadioButton();
            graphRadioButton.EnableViewState = true;
            graphRadioButton.ID = "graphRadioButton";
            graphRadioButton.GroupName = "graphTableRadioButton";
            Controls.Add(graphRadioButton);

            tableRadioButton = new System.Web.UI.WebControls.RadioButton();
            tableRadioButton.EnableViewState = true;
            tableRadioButton.ID = "tableRadioButton";
            tableRadioButton.GroupName = "graphTableRadioButton";
            Controls.Add(tableRadioButton);
            #endregion

            #region Options Set result pages
            if(resultOption) {
                SetResultPageOption();
            }
            #endregion

            #region Options spécifique AS
            if(TotalChoice) {
                _totalChoiceRadioButtonList = new RadioButtonList();
                _totalChoiceRadioButtonList.ID = this.ID + "_totalChoice";
                _totalChoiceRadioButtonList.CssClass = "txtBlanc11Bold";
                _totalChoiceRadioButtonList.AutoPostBack = autoPostBackOption;
                _totalChoiceRadioButtonList.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1188, customerWebSession.SiteLanguage), TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal.GetHashCode().ToString()));
                _totalChoiceRadioButtonList.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1189, customerWebSession.SiteLanguage), TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal.GetHashCode().ToString()));
                _totalChoiceRadioButtonList.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1190, customerWebSession.SiteLanguage), TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal.GetHashCode().ToString()));
                Controls.Add(_totalChoiceRadioButtonList);
            }

            if(ZoomGraphic) {
                _zoomGraphicCheckBox = new System.Web.UI.WebControls.CheckBox();
                _zoomGraphicCheckBox.ID = this.ID + "_zoomGraphicCheckBox";
                _zoomGraphicCheckBox.AutoPostBack = autoPostBackOption;
                Controls.Add(_zoomGraphicCheckBox);
            }
            #endregion

            #region Options Results Table Types (An. Dispositifs)
            if(ResultsTableTypesOptions) {
                _resultsTableTypesWebControl = new ResultsTableTypesWebControl();
                _resultsTableTypesWebControl.CustomerWebSession = customerWebSession;
                _resultsTableTypesWebControl.ID = "DDLResultsTableTypesWebControl1";
                _resultsTableTypesWebControl.ShowPictures = true;
                _resultsTableTypesWebControl.OutCssClass = this.outCssClass;
                _resultsTableTypesWebControl.OverCssClass = this.overCssClass;
                _resultsTableTypesWebControl.SkinID = "ResultsTableTypesWebControl1";
                _resultsTableTypesWebControl.ImageHeight = 26;
                _resultsTableTypesWebControl.ImageWidth = 139;
                Controls.Add(_resultsTableTypesWebControl);
            }
            #endregion

            #region Options Generic Column Level Detail
            if(GenericColumnLevelDetailSelectionOptions) {
                _genericColumnLevelDetailSelectionWebControl = new GenericColumnLevelDetailSelectionWebControl();
                _genericColumnLevelDetailSelectionWebControl.CustomerWebSession = customerWebSession;
                _genericColumnLevelDetailSelectionWebControl.GenericColumnDetailLevelType = GenericColumnDetailLevelType;
                _genericColumnLevelDetailSelectionWebControl.NbColumnDetailLevelItemList = NbColumnDetailLevelItemList;
                _genericColumnLevelDetailSelectionWebControl.Width = 194;
                Controls.Add(_genericColumnLevelDetailSelectionWebControl);
            }
            #endregion

            #region Options Generic Media Level Detail
            if(GenericMediaLevelDetailSelectionOptions) {
                _genericMediaLevelDetailSelectionWebControl = new GenericMediaLevelDetailSelectionWebControl();
                _genericMediaLevelDetailSelectionWebControl.CustomerWebSession = customerWebSession;
                _genericMediaLevelDetailSelectionWebControl.GenericDetailLevelComponentProfile = GenericDetailLevelComponentProfile;
                _genericMediaLevelDetailSelectionWebControl.GenericDetailLevelType = GenericDetailLevelType;
                _genericMediaLevelDetailSelectionWebControl.NbDetailLevelItemList = NbDetailLevelItemList;
                _genericMediaLevelDetailSelectionWebControl.RemoveASPXFilePath = RemoveASPXFilePath;
                _genericMediaLevelDetailSelectionWebControl.SaveASPXFilePath = SaveASPXFilePath;
                _genericMediaLevelDetailSelectionWebControl.Width = 194;
                Controls.Add(_genericMediaLevelDetailSelectionWebControl);
            }
            #endregion

            #region Options Sector Selection
            if(SectorSelectionOptions) {
                _sectorWebControl = new SectorWebControl();
                _sectorWebControl.Session = customerWebSession;
                _sectorWebControl.LanguageCode = customerWebSession.SiteLanguage;
                _sectorWebControl.ID = this.ID + "_sectorWebControl";
                _sectorWebControl.SkinID = "SectorWebControl";
                Controls.Add(_sectorWebControl);
            }
            #endregion

            #region Options Detail Advertiser Brand Product
            if(DetailAdvertiserBrandProductOptions) {
                _detailAdvertiserBrandProductWebControl = new DetailWebControl();
                _detailAdvertiserBrandProductWebControl.CustomerWebSession = customerWebSession;
                _detailAdvertiserBrandProductWebControl.ShowProduct = customerWebSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
                _detailAdvertiserBrandProductWebControl.ID = "productDetail_DetailWebControl1";
                _detailAdvertiserBrandProductWebControl.AdvertiserBrandProductOption = true;
                _detailAdvertiserBrandProductWebControl.AutoPostBackOption = autoPostBackOption;
                _detailAdvertiserBrandProductWebControl.CommonCssClass = "txtBlanc11Bold";
                Controls.Add(_detailAdvertiserBrandProductWebControl);
            }
            #endregion

            base.OnInit(e);
        }
        #endregion

        #region Load
        /// <summary>
        /// launched when the control is loaded
        /// </summary>
        /// <param name="e">arguments</param>
        protected override void OnLoad(EventArgs e) {

            #region Initializing controls
            AdExpressUniverse adExpressUniverse = null;
            NomenclatureElementsGroup nomenclatureElementsGroup = null;
            Dictionary<int, AdExpressUniverse> adExpressUniverseDictionary = null;
            bool showProduct = customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
            _showSegment = customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG);

            #region products APPM
            if(showProduct) {
                products = new DropDownList();
                products.EnableViewState = true;
                products.ID = "_products";
                Controls.Add(products);
            }
            #endregion

            #endregion

            #region Loading controls for the first time
            // Création de la liste des produits appm
            if(productsOption && showProduct) {
                if(!Page.IsPostBack || products.Items.Count <= 0) {
                    products.CssClass = cssClass;
                    products.AutoPostBack = autoPostBackOption;
                    DataTable productsTable = TNS.AdExpress.Web.DataAccess.Selections.Products.ProductListDataAccess.getProductList(customerWebSession).Tables[0];
                    if(productsTable.Rows.Count > 1)
                        products.Items.Add(new ListItem("----------------------------------------", "0"));
                    if(productsTable.Rows.Count == 1) {
                        try {
                            //int productID=Convert.ToInt32(products.SelectedItem.Value);
                            int productID = Convert.ToInt32(productsTable.Rows[0]["id_product"]);
                            if(productID != 0) {
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
                        catch(SystemException) { }
                    }

                    if(productsTable.Rows.Count > 0) {
                        foreach(DataRow dr in productsTable.Rows) {
                            products.Items.Add(new ListItem(dr["product"].ToString(), dr["id_product"].ToString()));
                        }
                    }
                }
                try {
                    //string productTag=customerWebSession.GetSelection(customerWebSession.CurrentUniversProduct,CustomerCst.type.productAccess);
                    string productTag = customerWebSession.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.PRODUCT);
                    products.Items.FindByValue(productTag).Selected = true;
                }
                catch(System.Exception) {
                    try {
                        products.Items.FindByValue("0").Selected = true;
                    }
                    catch(System.Exception) { }
                }
            }
            #endregion

            #region Loading univers and websession
            if(Page.IsPostBack) {
                // Saving the selected products in the current univers product when the page is posted back.
                if(productsOption && showProduct) {
                    try {
                        int productID = Convert.ToInt32(Page.Request.Form.GetValues("_products")[0]);
                        if(productID != 0) {
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
                    catch(SystemException) { }
                }
                if(unitOptionAppm) {
                    try {
                        customerWebSession.Unit = (SessionCst.Unit)Int64.Parse(Page.Request.Form.GetValues("_unitsAppm")[0]);
                    }
                    catch(SystemException) { }
                }
            }
            #endregion

            #region InitializeProductWebControl display
            if(this._initializeProductWebControl != null 
                && (this._inializeAdvertiserOption || this.InializeProductOption || this.InializeSlogansOption)) {
                
                switch(customerWebSession.CurrentModule) {
                    case WebConstantes.Module.Name.ALERTE_PLAN_MEDIA:
                    case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                        if((!WebFunctions.ProductDetailLevel.CanCustomizeUniverseSlogan(customerWebSession)
                            || !customerWebSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG))
                            || customerWebSession.DetailPeriod != TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly) {
                            _initializeProductWebControl.Visible = false;
                        }
                        else
                            _initializeProductWebControl.Visible = true;
                        break;

                    case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
                        _initializeProductWebControl.Visible = false;
                        break;

                    case WebConstantes.Module.Name.BILAN_CAMPAGNE:
                        if(!WebFunctions.ProductDetailLevel.CanCustomizeUniverseSlogan(customerWebSession) 
                            || !customerWebSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG)
                            || customerWebSession.CurrentTab != TNS.AdExpress.Constantes.FrameWork.Results.APPM.mediaPlanByVersion) {
                            _initializeProductWebControl.Visible = false;
                        }
                        else
                            _initializeProductWebControl.Visible = true;
                        break;

                    case WebConstantes.Module.Name.INDICATEUR:
                        if(customerWebSession.CurrentTab == SynthesisRecap.SYNTHESIS)
                            _initializeProductWebControl.Visible = false;
                        else
                            _initializeProductWebControl.Visible = true;
                        break;

                    default:
                        _initializeProductWebControl.Visible = true;
                        break;
                }

            }
            #endregion

            #region InitializeMediaWebControl display
            if(this._initializeMediaWebControl != null && this._initializeMediaOption) {
                switch(customerWebSession.CurrentModule) {
                    case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
                        _initializeMediaWebControl.Visible = false;
                        break;
                    default:
                        _initializeMediaWebControl.Visible = true;
                        break;
                }
            }
            #endregion

            #region Total Choice (AS)
            if(TotalChoice) {
                if(_totalChoiceRadioButtonList.Items.FindByValue(TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal.GetHashCode().ToString()) == null) _totalChoiceRadioButtonList.Items.Insert(0, new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1188, customerWebSession.SiteLanguage), TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal.GetHashCode().ToString()));
                if(_totalChoiceRadioButtonList.Items.FindByValue(TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal.GetHashCode().ToString()) == null) _totalChoiceRadioButtonList.Items.Insert(1, new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1189, customerWebSession.SiteLanguage), TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal.GetHashCode().ToString()));
                if(_totalChoiceRadioButtonList.Items.FindByValue(TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal.GetHashCode().ToString()) == null) _totalChoiceRadioButtonList.Items.Insert(2, new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1190, customerWebSession.SiteLanguage), TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.marketTotal.GetHashCode().ToString()));

                switch(customerWebSession.CurrentTab) {
                    case SynthesisRecap.PALMARES:
                        if(graphRadioButton.Checked) {
                            _totalChoiceRadioButtonList.Items.Clear();
                        }
                        break;
                    case SynthesisRecap.SEASONALITY:
                    case SynthesisRecap.MEDIA_STRATEGY:
                        _totalChoiceRadioButtonList.Items.Remove(_totalChoiceRadioButtonList.Items.FindByValue(TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal.GetHashCode().ToString()));
                        break;
                    default:
                        _totalChoiceRadioButtonList.Items.Clear();
                        break;
                }
                bool hasSelected = false;
                foreach(ListItem currentItem in _totalChoiceRadioButtonList.Items) {
                    if(currentItem.Selected) {
                        hasSelected = true;
                        break;
                    }
                }
                if(_totalChoiceRadioButtonList.Items.Count > 0 && !hasSelected) _totalChoiceRadioButtonList.Items[0].Selected = true;
                if(_totalChoiceRadioButtonList.Items.Count > 0) {
                    try {
                        customerWebSession.ComparaisonCriterion = (TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion)Convert.ToInt32(_totalChoiceRadioButtonList.Items.FindByValue(_totalChoiceRadioButtonList.SelectedItem.Value).Value);
                    }
                    catch(System.Exception) { }
                }
            }
            #endregion

            #region Zoom Graphics
            if(ZoomGraphic) {
                switch(customerWebSession.CurrentTab) {
                    case SynthesisRecap.SEASONALITY:
                        if(graphRadioButton.Checked)
                            _zoomGraphicCheckBox.Visible = true;
                        else
                            _zoomGraphicCheckBox.Visible = false;
                        break;
                    default:
                        _zoomGraphicCheckBox.Checked = false;
                        _zoomGraphicCheckBox.Visible = false;
                        break;
                }
            }
            #endregion

            #region Options Generic Media Level Detail
            if(GenericMediaLevelDetailSelectionOptions) {

                switch(customerWebSession.CurrentModule) {
                    case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:
                    case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                        switch(customerWebSession.CurrentTab) {
                            case Portofolio.SYNTHESIS:
                            case Portofolio.DETAIL_MEDIA:
                            case Portofolio.STRUCTURE:
                                _genericMediaLevelDetailSelectionWebControl.Visible = false;
                                break;
                            case Portofolio.DETAIL_PORTOFOLIO:
                            case Portofolio.CALENDAR:
                                _genericMediaLevelDetailSelectionWebControl.Visible = true;
                                break;
                        }
                        break;

                    case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
                        _genericMediaLevelDetailSelectionWebControl.GenericDetailLevelType = WebConstantes.GenericDetailLevel.Type.devicesAnalysis;
                        break;
                    case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
                        _genericMediaLevelDetailSelectionWebControl.GenericDetailLevelType = WebConstantes.GenericDetailLevel.Type.programAnalysis;
                        break;

                    default:
                        _genericMediaLevelDetailSelectionWebControl.Visible = true;
                        break;
                }
            }
            #endregion

            #region Options Results Table Types (An. Dispositifs)
            if(ResultsTableTypesOptions) {
                 switch(customerWebSession.PreformatedTable) {
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Media:
                        UnitOption = true;
                        break;
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period:
                        UnitOption = true;
                        break;
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units:
                        if(customerWebSession.PercentageAlignment == WebConstantes.Percentage.Alignment.horizontal)
                            customerWebSession.PercentageAlignment = WebConstantes.Percentage.Alignment.none;
                        UnitOption = false;
                        break;
                }
            }
            #endregion

            #region Options Detail Advertiser Brand Product
            if(DetailAdvertiserBrandProductOptions) {
                
                if(customerWebSession.CurrentTab == 1)
                    _detailAdvertiserBrandProductWebControl.Visible = true;
                else
                    _detailAdvertiserBrandProductWebControl.Visible = false;
            }
            #endregion

            base.OnLoad(e);
        }
        #endregion

        #region Custom PreRender
        /// <summary>
        ///custom prerender 
        /// </summary>
        /// <param name="sender">object qui lance l'évènement</param>
        /// <param name="e">arguments</param>
        private void Custom_PreRender(object sender, System.EventArgs e) {

            string themeName = WebApplicationParameters.Themes[customerWebSession.SiteLanguage].Name;
            List<System.Web.UI.WebControls.CheckBox> _checkBoxListMutualExclusion = new List<System.Web.UI.WebControls.CheckBox>();
            List<System.Web.UI.WebControls.CheckBox> _checkBoxListDependentSelection = new List<System.Web.UI.WebControls.CheckBox>();

            #region Unité
            if(unitOption) {
                //Création de la liste des unités
                list = new DropDownList();
                list.ID = "_units";
                list.CssClass = cssClass;
                list.AutoPostBack = autoPostBackOption;
                if(!percentage) list.Width = new System.Web.UI.WebControls.Unit("100%");
                //ArrayList units;
                List<UnitInformation> units = customerWebSession.GetValidUnitForResult();

                foreach(UnitInformation currentUnit in units) {
                    if(currentUnit.Id != SessionCst.Unit.volume || customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_VOLUME_MARKETING_DIRECT))
                        list.Items.Add(new ListItem(GestionWeb.GetWebWord(currentUnit.WebTextId, customerWebSession.SiteLanguage), currentUnit.Id.GetHashCode().ToString()));
                    else if(customerWebSession.Unit == SessionCst.Unit.volume)
                        customerWebSession.Unit = UnitsInformation.DefaultCurrency;
                }
                if(!units.Contains(UnitsInformation.Get(customerWebSession.Unit))) {
                    customerWebSession.Unit = units[0].Id;
                }
                customerWebSession.Save();
                list.Items.FindByValue(customerWebSession.Unit.GetHashCode().ToString()).Selected = true;

                Controls.Add(list);
            }
            #endregion

            #region Unité Appm
            if(unitOptionAppm) {
                //Création de la liste des unités appm
                listUnitAppm = new DropDownList();
                listUnitAppm.ID = "_unitsAppm";
                listUnitAppm.CssClass = cssClass;
                listUnitAppm.AutoPostBack = autoPostBackOption;
                ArrayList unitsAppm = WebFunctions.Units.getUnitsFromAppmPress();
                for(int i = 0; i < unitsAppm.Count; i++) {

                    listUnitAppm.Items.Add(new ListItem(GestionWeb.GetWebWord((int)SessionCst.UnitsTraductionCodes[(SessionCst.Unit)unitsAppm[i]], customerWebSession.SiteLanguage), ((int)(SessionCst.Unit)unitsAppm[i]).ToString()));
                }
                listUnitAppm.Items.FindByValue(((int)customerWebSession.Unit).ToString()).Selected = true;

                Controls.Add(listUnitAppm);
            }
            #endregion

            #region Encart
            if(insertOption && WebApplicationParameters.AllowInsetOption) {
                //Création de la liste des encarts
                listInsert = new DropDownList();
                listInsert.ID = "_inserts";
                listInsert.CssClass = cssClass;
                listInsert.AutoPostBack = autoPostBackOption;
                ArrayList inserts = WebFunctions.Units.getInserts();
                for(int j = 0; j < inserts.Count; j++) {
                    listInsert.Items.Add(new ListItem(GestionWeb.GetWebWord((int)SessionCst.InsertsTraductionCodes[(SessionCst.Insert)inserts[j]], customerWebSession.SiteLanguage), ((int)(SessionCst.Insert)inserts[j]).ToString()));
                }
                listInsert.Items.FindByValue(((int)customerWebSession.Insert).ToString()).Selected = true;
                Controls.Add(listInsert);
            }
            #endregion

            #region Percentage
            if(percentage) {
                percentageCheckBox = new System.Web.UI.WebControls.CheckBox();
                percentageCheckBox.ID = "_percentage";
                percentageCheckBox.CssClass = "txtBlanc11Bold";
                percentageCheckBox.AutoPostBack = autoPostBackOption;
                percentageCheckBox.Text = GestionWeb.GetWebWord(806, customerWebSession.SiteLanguage);
                percentageCheckBox.Checked = customerWebSession.Percentage;
                Controls.Add(percentageCheckBox);
            }
            #endregion

            #region PDM
            if (pdmOption) {
                PdmCheckBox = new System.Web.UI.WebControls.CheckBox();
                PdmCheckBox.ID = this.ID + "_pdm";
                PdmCheckBox.ToolTip = GestionWeb.GetWebWord(1179, customerWebSession.SiteLanguage);
                PdmCheckBox.CssClass = "txtBlanc11Bold";
                PdmCheckBox.AutoPostBack = autoPostBackOption;
                PdmCheckBox.Text = GestionWeb.GetWebWord(806, customerWebSession.SiteLanguage);
                PdmCheckBox.Checked = customerWebSession.PDM;
                if (!_mutualExclusion)
                    Controls.Add(PdmCheckBox);
                else
                    _checkBoxListMutualExclusion.Add(PdmCheckBox);
            }
            #endregion

            #region PDV
            if(pdvOption) {
                PdvCheckBox = new System.Web.UI.WebControls.CheckBox();
                PdvCheckBox.ID = this.ID + "_pdv";
                PdvCheckBox.ToolTip = GestionWeb.GetWebWord(1180, customerWebSession.SiteLanguage);
                PdvCheckBox.CssClass = "txtBlanc11Bold";
                PdvCheckBox.AutoPostBack = autoPostBackOption;
                PdvCheckBox.Text = GestionWeb.GetWebWord(1166, customerWebSession.SiteLanguage);
                PdvCheckBox.Checked = customerWebSession.PDV;
                if (!_mutualExclusion)
                    Controls.Add(PdvCheckBox);
                else
                    _checkBoxListMutualExclusion.Add(PdvCheckBox);
            }
            #endregion

            #region CheckBoxs Mutual Exclusion
            if (_mutualExclusion)
            {
                _checkBoxsMutualExclusion = new CheckBoxsMutualExclusion(_checkBoxListMutualExclusion);
                Controls.Add(_checkBoxsMutualExclusion);
            }
            #endregion

            #region Evolution
            if (evolutionOption) {
                EvolutionCheckBox = new System.Web.UI.WebControls.CheckBox();
                EvolutionCheckBox.ID = this.ID + "_evol";
                EvolutionCheckBox.CssClass = "txtBlanc11Bold";
                EvolutionCheckBox.ToolTip = GestionWeb.GetWebWord(1178, customerWebSession.SiteLanguage);
                EvolutionCheckBox.AutoPostBack = autoPostBackOption;
                EvolutionCheckBox.Text = GestionWeb.GetWebWord(1168, customerWebSession.SiteLanguage);
                EvolutionCheckBox.Checked = customerWebSession.Evolution;
                if (!_dependentSelection)
                    Controls.Add(EvolutionCheckBox);
                else
                    _checkBoxListDependentSelection.Add(EvolutionCheckBox);
            }
            #endregion

            #region Comparative Study
            if (comparativeStudyOption)
            {
                _comparativeStudyCheckBox = new System.Web.UI.WebControls.CheckBox();
                _comparativeStudyCheckBox.ID = this.ID + "_comparativeStudy";
                _comparativeStudyCheckBox.ToolTip = GestionWeb.GetWebWord(1118, customerWebSession.SiteLanguage);
                _comparativeStudyCheckBox.CssClass = "txtBlanc11Bold";
                _comparativeStudyCheckBox.AutoPostBack = autoPostBackOption;
                _comparativeStudyCheckBox.Text = GestionWeb.GetWebWord(1118, customerWebSession.SiteLanguage);
                _comparativeStudyCheckBox.Checked = customerWebSession.ComparativeStudy && IsValidPeriodComparative();
                _comparativeStudyCheckBox.Enabled = IsValidPeriodComparative();

                if (comparativeStudyDateTypeOption) {
                    _dateComparativeSelection = new DateComparativeSelection(customerWebSession, comparativeStudyDateTypeOption, false);
                    _dateComparativeSelection.ID = this.ID + "_comparativeStudyDateTypeOption";
                    _dateComparativeSelectionLabel = new System.Web.UI.WebControls.Label();
                    _dateComparativeSelectionLabel.ID = this.ID + "_comparativeStudyDateTypeOptionSelection";
                    _dateComparativeSelectionLabel.CssClass = "txtComparativeStudy";
                    if (_comparativeStudyCheckBox.Checked && _comparativeStudyCheckBox.Enabled) {
                        if (customerWebSession.ComparativePeriodType == TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType.comparativeWeekDate)
                            _dateComparativeSelectionLabel.Text = GestionWeb.GetWebWord(2295, customerWebSession.SiteLanguage);
                        else if (customerWebSession.ComparativePeriodType == TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType.dateToDate)
                            _dateComparativeSelectionLabel.Text = GestionWeb.GetWebWord(2294, customerWebSession.SiteLanguage);

                        _comparativeStudyCheckBox.Attributes.Add("onClick", "javacript:if(this.checked){this.checked=false; " + _dateComparativeSelection.JavascriptFunctionOnDisplay + "}else{document.getElementById('selectionType_" + _dateComparativeSelection.ID + "').value=document.getElementById('selectionType_" + _dateComparativeSelection.ID + "').value='';document.getElementById('" + _dateComparativeSelectionLabel.ID + "').innerHTML='';}");
                        _dateComparativeSelectionLabel.Attributes.Add("onClick", "javacript:" + _dateComparativeSelection.JavascriptFunctionOnDisplay);
                    }
                    if (!_dependentSelection) {
                        Controls.Add(_dateComparativeSelectionLabel);
                        Controls.Add(_dateComparativeSelection);
                    }
                }

                if (!_dependentSelection)
                    Controls.Add(_comparativeStudyCheckBox);
            }
            #endregion

            #region CheckBoxs Dependent Selection
            if (_dependentSelection)
            {
                _checkBoxsDependentSelection = new CheckBoxsDependentSelection(_comparativeStudyCheckBox, _checkBoxListDependentSelection);
                Controls.Add(_checkBoxsDependentSelection);
            }
            #endregion

            #region Personalized Elements
            if(personalizedElementsOption) {
                PersonalizedElementsCheckBox = new System.Web.UI.WebControls.CheckBox();
                PersonalizedElementsCheckBox.ID = this.ID + "_perso";
                PersonalizedElementsCheckBox.ToolTip = GestionWeb.GetWebWord(1181, customerWebSession.SiteLanguage);
                PersonalizedElementsCheckBox.CssClass = "txtInitialize";
                PersonalizedElementsCheckBox.AutoPostBack = autoPostBackOption;
                PersonalizedElementsCheckBox.Text = GestionWeb.GetWebWord(1174, customerWebSession.SiteLanguage);
                PersonalizedElementsCheckBox.Checked = customerWebSession.PersonalizedElementsOnly;
                Controls.Add(PersonalizedElementsCheckBox);
            }
            #endregion

            #region Autopromo
            if(autopromoEvaliantOption) {
                AutopromoEvaliantCheckBox = new System.Web.UI.WebControls.CheckBox();
                AutopromoEvaliantCheckBox.ID = this.ID + "_autopromoEvaliant";
                AutopromoEvaliantCheckBox.ToolTip = GestionWeb.GetWebWord(2476, customerWebSession.SiteLanguage);
                AutopromoEvaliantCheckBox.CssClass = "txtBlanc11Bold";
                AutopromoEvaliantCheckBox.AutoPostBack = autoPostBackOption;
                AutopromoEvaliantCheckBox.Text = GestionWeb.GetWebWord(2476, customerWebSession.SiteLanguage);
                AutopromoEvaliantCheckBox.Checked = customerWebSession.AutopromoEvaliant;
                Controls.Add(AutopromoEvaliantCheckBox);
            }
            #endregion

            #region Media Detail
            if(mediaDetailOption) {
                mediaDetail = new DropDownList();
                mediaDetail.Width = new System.Web.UI.WebControls.Unit("100%");
                // Pour les Plan media
                if(forceMediaDetailForMediaPlan || customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA || customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PLAN_MEDIA) {
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1292, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1142, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategory.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1143, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1544, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMedia.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1542, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1543, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia.GetHashCode().ToString()));

                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1860, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1861, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1862, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1863, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia.GetHashCode().ToString()));
                    //mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1864, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter.GetHashCode().ToString()));
                    //mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1865, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia.GetHashCode().ToString()));

                    #region Accroches
                    // Gestion des droits sur les accroches
                    if(customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_SLOGAN_ACCESS_FLAG) &&
                        (_forceMediaDetailForSlogan ||
                        // Sélection par produit ou marque
                        (customerWebSession.GetSelection(customerWebSession.SelectionUniversAdvertiser, TNS.AdExpress.Constantes.Customer.Right.type.productAccess).Length > 0 ||
                        customerWebSession.GetSelection(customerWebSession.SelectionUniversAdvertiser, TNS.AdExpress.Constantes.Customer.Right.type.brandAccess).Length > 0) &&
                        // Pas de famille, classe, groupe, groupe d'annonceur, annonceur
                        customerWebSession.GetSelection(customerWebSession.SelectionUniversAdvertiser, TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length == 0 &&
                        customerWebSession.GetSelection(customerWebSession.SelectionUniversAdvertiser, TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length == 0 &&
                        customerWebSession.GetSelection(customerWebSession.SelectionUniversAdvertiser, TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length == 0 &&
                        customerWebSession.GetSelection(customerWebSession.SelectionUniversAdvertiser, TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length == 0 &&
                        customerWebSession.GetSelection(customerWebSession.SelectionUniversAdvertiser, TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length == 0) &&
                        // Niveau de détail par jour
                        customerWebSession.DetailPeriod == WebConstantes.CustomerSessions.Period.DisplayLevel.dayly
                        ) {
                        // On augmente la taille pour les accroches
                        mediaDetail.Width = 200;
                        mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1866, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan.GetHashCode().ToString()));
                        mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1867, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan.GetHashCode().ToString()));
                        mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1868, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan.GetHashCode().ToString()));
                        mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1869, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan.GetHashCode().ToString()));
                        //mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1870, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan.GetHashCode().ToString()));
                        //mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1871, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan.GetHashCode().ToString()));
                        //mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1872, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan.GetHashCode().ToString()));
                        mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1873, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganMedia.GetHashCode().ToString()));
                        mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1874, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia.GetHashCode().ToString()));
                        //mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1875, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia.GetHashCode().ToString()));
                        mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1876, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia.GetHashCode().ToString()));
                        //mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1877, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia.GetHashCode().ToString()));
                        mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1878, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia.GetHashCode().ToString()));
                        //mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1879, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia.GetHashCode().ToString()));
                        //mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1880, customerWebSession.SiteLanguage),SessionCst.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia.GetHashCode().ToString()));
                    }
                    #endregion

                }
                else if(customerWebSession.CurrentModule == WebConstantes.Module.Name.INDICATEUR || customerWebSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DYNAMIQUE) {
                    VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID);

                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1141, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString()));

                    if(vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category))
                        mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1142, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategory.GetHashCode().ToString()));
                    if(customerWebSession.CurrentModule != WebConstantes.Module.Name.INDICATEUR && vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.media))
                        mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1544, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMedia.GetHashCode().ToString()));
                    if(vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category) && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.media))
                        mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1143, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia.GetHashCode().ToString()));

                    if((mediaDetail.Items.Count == 1) && (mediaDetail.Items[0].Value == SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString()))
                        mediaDetail.Enabled = false;
                }
                else if (customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES) {
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1141, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1382, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.category.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(18, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.Media.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1142, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategory.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1544, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMedia.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1860, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1383, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSeller.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(2812, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicle.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1862, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia.GetHashCode().ToString()));
                    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(2813, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSellerCategory.GetHashCode().ToString()));
                }
                else{
                    VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID);
                    switch (vehicleInfo.Id){
                        case ClassificationCst.DB.Vehicles.names.tv:
                        case ClassificationCst.DB.Vehicles.names.radio:
                        case ClassificationCst.DB.Vehicles.names.outdoor:
                        case ClassificationCst.DB.Vehicles.names.instore:
                        case ClassificationCst.DB.Vehicles.names.mediasTactics:
                            mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1141, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString()));
                            mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1142, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategory.GetHashCode().ToString()));
                            if (customerWebSession.CurrentModule != WebConstantes.Module.Name.INDICATEUR) mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1544, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMedia.GetHashCode().ToString()));
                            mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1143, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia.GetHashCode().ToString()));
                            break;
                        case ClassificationCst.DB.Vehicles.names.press:
                        case ClassificationCst.DB.Vehicles.names.newspaper:
                        case ClassificationCst.DB.Vehicles.names.magazine:
                        case ClassificationCst.DB.Vehicles.names.internationalPress:
                        case ClassificationCst.DB.Vehicles.names.internet:
                        case ClassificationCst.DB.Vehicles.names.mobileTelephony:
                        case ClassificationCst.DB.Vehicles.names.emailing:
                            mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1141, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString()));
                            mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1142, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategory.GetHashCode().ToString()));
                            break;
                        default:
                            mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1141, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString()));
                            mediaDetail.Enabled = false;
                            break;
                    }
                }
                mediaDetail.ID = "mediaDetail_" + this.ID;
                mediaDetail.AutoPostBack = autoPostBackOption;
                mediaDetail.CssClass = cssClass;
                try {
                    mediaDetail.Items.FindByValue(customerWebSession.PreformatedMediaDetail.GetHashCode().ToString()).Selected = true;
                }
                catch(System.Exception) {
                    mediaDetail.Items[0].Selected = true;
                }

                Controls.Add(mediaDetail);
            }
            #endregion

            #region Product Detail
            if(ProductDetailOption) {
                productDetail = new DropDownList();
                productDetail.Width = new System.Web.UI.WebControls.Unit("100%");
                productDetail.ID = "productDetail_" + this.ID;
                productDetail.CssClass = cssClass;
                productDetail.AutoPostBack = autoPostBackOption;

                /* WARNING !!! : This patch is just temporarily used in order to add specific levels for the Finland version of the site
                * the levels are : Category, Sub Category, Category/Advertiser and Sub Category/Advertiser
                * */
                if(WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.FINLAND)) {
                    productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(175, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.sector.GetHashCode().ToString()));
                    productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1491, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser.GetHashCode().ToString()));
                    productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(552, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.subSector.GetHashCode().ToString()));
                    productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(2610, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.subSectorAdvertiser.GetHashCode().ToString()));
                }

                productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1110, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.group.GetHashCode().ToString()));
                if(_showSegment)
                    productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1144, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.groupSegment.GetHashCode().ToString()));

                // Rights verification for Brand
                if(customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE)) {
                    productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1111, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.groupBrand.GetHashCode().ToString()));
                }
                if(customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                    productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1112, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.groupProduct.GetHashCode().ToString()));
                productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1145, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.groupAdvertiser.GetHashCode().ToString()));

                // Modifications for segmentAdvertiser,segmentProduct,SegmentBrand(3 new items added in the dropdownlist)
                if(_showSegment)
                    productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1577, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser.GetHashCode().ToString()));
                if((customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) && _showSegment)
                    productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1578, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.segmentProduct.GetHashCode().ToString()));
                if(customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE) && _showSegment) {
                    productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1579, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.segmentBrand.GetHashCode().ToString()));
                }
                productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1146, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.advertiser.GetHashCode().ToString()));
                if(customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE)) {
                    productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1147, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.advertiserBrand.GetHashCode().ToString()));
                }
                if(customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                    productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1148, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.advertiserProduct.GetHashCode().ToString()));
                if(customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE)) {
                    productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1149, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.brand.GetHashCode().ToString()));
                }
                if(customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                    productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(858, customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedProductDetails.product.GetHashCode().ToString()));

                try {
                    productDetail.Items.FindByValue(customerWebSession.PreformatedProductDetail.GetHashCode().ToString()).Selected = true;
                }
                catch(System.Exception) {
                    productDetail.SelectedIndex = 0;
                }
                Controls.Add(productDetail);
            }
            #endregion

            #region Table Choice
            if(tblChoiceOption) {
                tblChoice = new ImageDropDownListWebControl();
                tblChoice.BackColor = this.BackColor;
                tblChoice.BorderColor = this.BorderColor;
                tblChoice.BorderWidth = new System.Web.UI.WebControls.Unit(this.borderWidth);
                int sponsorshipListIndex = 24;
                if(this.List != "") tblChoice.List = this.List;
                else {
                    if(customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS
                        || customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES)
                        tblChoice.List = "&nbsp;|&nbsp;|&nbsp;";
                    else
                        tblChoice.List = "&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;|&nbsp;";
                }
                if(this.images != "") tblChoice.Images = this.images;
                else {
                    if(customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS
                        || customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES) {
                        tblChoice.Images = "/App_Themes/" + themeName + "/Images/Culture/Tables/Parrainage_type1.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/Parrainage_type2.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/Parrainage_type3.gif";
                    }
                    else {
                        tblChoice.Images = "/App_Themes/" + themeName + "/Images/Culture/Tables/type1.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/type2.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/type3.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/type4.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/type10.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/type11.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/type5.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/type6.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/type7.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/type8.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/type9.gif";
                    }
                }
                if(customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS
                    || customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES)
                    tblChoice.ListIndex = customerWebSession.PreformatedTable.GetHashCode() - sponsorshipListIndex;
                else tblChoice.ListIndex = customerWebSession.PreformatedTable.GetHashCode();
                tblChoice.ImageHeight = this.imageHeight;
                tblChoice.ImageWidth = this.imageWidth;
                tblChoice.ImageButtonArrow = "/App_Themes/" + themeName + "/Images/Common/button/bt_arrow_tall_top.gif";
                tblChoice.ID = "DDL" + this.ID;
                tblChoice.OutCssClass = this.outCssClass;
                tblChoice.OverCssClass = this.overCssClass;
                tblChoice.Width = new System.Web.UI.WebControls.Unit("121");
                tblChoice.ShowPictures = this.pictShow;

                Controls.Add(tblChoice);
            }
            #endregion

            #region Percentage Type
            // Contrôle choix type de pourcentage
            if(_percentageTypeOption) {

                _percentageTypeDropDownList = new System.Web.UI.WebControls.DropDownList();
                _percentageTypeDropDownList.ID = "_percentageTypePercentageDropDownList";

                //if(!Page.IsPostBack||_percentageTypeDropDownList.Items.Count<=0){
                _percentageTypeDropDownList.CssClass = cssClass;
                _percentageTypeDropDownList.AutoPostBack = autoPostBackOption;
                _percentageTypeDropDownList.Items.Add(new ListItem("----------------------", WebConstantes.Percentage.Alignment.none.GetHashCode().ToString()));
                _percentageTypeDropDownList.Items.Add(new ListItem(GestionWeb.GetWebWord(2065, customerWebSession.SiteLanguage), WebConstantes.Percentage.Alignment.vertical.GetHashCode().ToString()));
                if(customerWebSession.PreformatedTable != WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                    _percentageTypeDropDownList.Items.Add(new ListItem(GestionWeb.GetWebWord(2064, customerWebSession.SiteLanguage), WebConstantes.Percentage.Alignment.horizontal.GetHashCode().ToString()));
                //}
                try {
                    _percentageTypeDropDownList.Items.FindByValue(customerWebSession.PercentageAlignment.GetHashCode().ToString()).Selected = true;

                }
                catch(System.Exception) {
                    try {
                        _percentageTypeDropDownList.Items.FindByValue(WebConstantes.Percentage.Alignment.none.GetHashCode().ToString()).Selected = true;
                    }
                    catch(System.Exception) {
                    }
                }
                Controls.Add(_percentageTypeDropDownList);
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
            bool showProduct = customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

            output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" class=\"backGroundOptions\">");
            output.Write("\n<tr>");
            output.Write("\n<td>");

            #region Titre
            output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
            output.Write("\n<tr>");
            output.Write("\n<td class=\"headerLeft\" colspan=\"3\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\"></td>");
            output.Write("\n</tr>");
            output.Write("\n<tr>");
            output.Write("\n<td style=\"HEIGHT: 14px\" width=\"1%\" class=\"blockBackGround\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\" width=\"13\"></td>");
            output.Write("\n<td class=\"txtNoir11Bold backGroundWhite titleUppercase\" width=\"100%\">" + GestionWeb.GetWebWord(792, customerWebSession.SiteLanguage) + "</td>");
            output.Write("\n<td style=\"HEIGHT: 14px\" class=\"headerLeft\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\" width=\"1\"></td>");
            output.Write("\n</tr>");
            output.Write("\n<tr>");
            output.Write("\n<td class=\"headerLeft\" colspan=\"3\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/images/Common/pixel.gif\"></td>");
            output.Write("\n</tr>");
            output.Write("\n</table>");
            #endregion

            output.Write("\n</td>");
            output.Write("\n</tr>");
            output.Write("\n<TR>");
            output.Write("\n<TD height=\"5\"></TD>");
            output.Write("\n</TR>");

            #region Option résultat
            if(resultOption) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td class=\"txtBlanc11Bold\">");
                output.Write(GestionWeb.GetWebWord(793, customerWebSession.SiteLanguage) + " : ");
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                resultsPages.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Option unité

            // ANCIENNE VERSION AFFICHEE
            //if(unitOption) {
            //    output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
            //    output.Write("\n<td title=\"" + GestionWeb.GetWebWord(1182, customerWebSession.SiteLanguage) + "\" class=\"txtBlanc11Bold\">");
            //    output.Write(GestionWeb.GetWebWord(304, customerWebSession.SiteLanguage) + " : ");
            //    output.Write("\n</td>");
            //    output.Write("\n</tr>");
            //    output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
            //    output.Write("\n<td>");
            //    output.Write("\n<div>");
            //    output.Write("\n<div class=\"unitSelectOption\">");
            //    list.RenderControl(output);
            //    output.Write("\n</div>");

            //    output.Write("\n<div class=\"percentageOption\">");
            //    if(percentage) {
            //        percentageCheckBox.RenderControl(output);
            //    }
            //    output.Write("\n</div>");
            //    output.Write("\n</div>");
            //    output.Write("\n</td>");
            //    output.Write("\n</tr>");
            //    output.Write("\n<TR>");
            //    output.Write("\n<TD height=\"5\"></TD>");
            //    output.Write("\n</TR>");
            //}

            if(unitOption) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td title=\"" + GestionWeb.GetWebWord(1182, customerWebSession.SiteLanguage) + "\" class=\"txtBlanc11Bold\">");
                output.Write(GestionWeb.GetWebWord(304, customerWebSession.SiteLanguage) + " : ");
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");

                output.Write("\n<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" width=\"100%\" >");
                output.Write("\n<tr>");
                output.Write("\n<td>");
                list.RenderControl(output);
                output.Write("\n</td>");
                if(percentage) {
                    output.Write("\n<td>");
                    percentageCheckBox.RenderControl(output);
                    output.Write("\n</td>");
                }
                output.Write("\n</tr>");
                output.Write("\n</table>");

                output.Write("\n</td>");
                output.Write("\n</tr>");

                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Option unité Appm
            if(unitOptionAppm) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td title=\"" + GestionWeb.GetWebWord(1182, customerWebSession.SiteLanguage) + "\" class=\"txtBlanc11Bold\">");
                output.Write(GestionWeb.GetWebWord(304, customerWebSession.SiteLanguage) + " : ");
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                listUnitAppm.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Option detail media
            if(mediaDetailOption) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td class=\"txtBlanc11Bold\">");
                output.Write(GestionWeb.GetWebWord(1150, customerWebSession.SiteLanguage));
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                mediaDetail.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Option choix d'un encart
            if(insertOption && WebApplicationParameters.AllowInsetOption) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td class=\"txtBlanc11Bold\">");
                output.Write(GestionWeb.GetWebWord(1400, customerWebSession.SiteLanguage));
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                listInsert.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Option Sector Selection
            if(SectorSelectionOptions) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                _sectorWebControl.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Option Period Detail
            if(PeriodDetailOptions) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                _periodDetailWebControl.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Option type de pourcentage (horizontal ou vertical)
            if(_percentageTypeOption) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td class=\"txtBlanc11Bold\">");
                output.Write(GestionWeb.GetWebWord(1236, customerWebSession.SiteLanguage) + " : ");
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                _percentageTypeDropDownList.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Option Generic Column Level Detail
            if(GenericColumnLevelDetailSelectionOptions) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                _genericColumnLevelDetailSelectionWebControl.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Option Generic Media Level Detail
            if(GenericMediaLevelDetailSelectionOptions && _genericMediaLevelDetailSelectionWebControl.Visible) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td><hr class=\"hrSpacer\" />");
                _genericMediaLevelDetailSelectionWebControl.RenderControl(output);
                output.Write("\n<hr class=\"hrSpacer\" /></td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Option detail produit
            if(productDetailOption) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td class=\"txtBlanc11Bold\">");
                output.Write(GestionWeb.GetWebWord(1124, customerWebSession.SiteLanguage));
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                productDetail.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Option format de tableau
            if(tblChoiceOption) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td class=\"txtBlanc11Bold\">");
                output.Write(GestionWeb.GetWebWord(1140, customerWebSession.SiteLanguage));
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                tblChoice.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Options PDM, PDV, evolution
            if (pdmOption || pdvOption || evolutionOption || comparativeStudyOption) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td class=\"txtBlanc11Bold\">");

                if (!_dependentSelection)
                {
                    if (comparativeStudyOption) {
                       
                        if (comparativeStudyDateTypeOption) {

                            #region javascript
                            output.Write("\n<script language=\"JavaScript\" type=\"text/JavaScript\">\n");

                            output.Write("\nfunction OnValideComparativeSelection_" + this.ID + "(){");
                            output.Write("\n\tdocument.getElementById('" + _comparativeStudyCheckBox.ID + "').checked = document.getElementById('selectionType_" + _dateComparativeSelection.ID + "').value!=null && document.getElementById('selectionType_" + _dateComparativeSelection.ID + "').value.length>0;");
                            output.Write("\n\tif(document.getElementById('" + _comparativeStudyCheckBox.ID + "').checked ==true){");
                            output.Write("\n\t\tif(document.getElementById('selectionType_" + _dateComparativeSelection.ID + "').value== '" + WebConstantes.globalCalendar.comparativePeriodType.comparativeWeekDate+"')");
                            output.Write("\n\t\t\tdocument.getElementById('" + _dateComparativeSelectionLabel.ID + "').innerHTML='" + GestionWeb.GetWebWord(2295, customerWebSession.SiteLanguage) + "';");
                            output.Write("\n\t\telse if(document.getElementById('selectionType_" + _dateComparativeSelection.ID + "').value== '" + WebConstantes.globalCalendar.comparativePeriodType.dateToDate + "')");
                            output.Write("\n\t\t\tdocument.getElementById('" + _dateComparativeSelectionLabel.ID + "').innerHTML='" + GestionWeb.GetWebWord(2294, customerWebSession.SiteLanguage) + "';");
                            output.Write("\n\t}");
                            output.Write("\n}\n");

                            output.Write("\n</script>\n");
                            _dateComparativeSelection.JavascriptFunctionOnValidate = "OnValideComparativeSelection_" + this.ID + "();";
                            #endregion

                            output.Write("<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\"><tr><td>");
                            _comparativeStudyCheckBox.RenderControl(output);
                            output.Write("</td></tr><tr><td>");
                            _dateComparativeSelectionLabel.RenderControl(output);
                            output.Write("</td></tr></table>");

                            _dateComparativeSelection.RenderControl(output);
                        }
                        else {
                            _comparativeStudyCheckBox.RenderControl(output);
                        }
                    }

                    if (evolutionOption)
                    {
                        if (!customerWebSession.ComparativeStudy)
                        {
                            EvolutionCheckBox.Enabled = false;
                            EvolutionCheckBox.Checked = false;
                        }
                        EvolutionCheckBox.RenderControl(output);
                        output.Write("&nbsp;&nbsp;");
                    }
                }
                else {
                    if (!customerWebSession.ComparativeStudy)
                    {
                        //EvolutionCheckBox.Enabled = false;
                        EvolutionCheckBox.InputAttributes.Add("disabled","true");
                        EvolutionCheckBox.Checked = false;
                        customerWebSession.Evolution = false;
                    }
                    _checkBoxsDependentSelection.RenderControl(output);
                    output.Write("\n</td>");
                    output.Write("\n</tr>");
                    output.Write("\n<TR>");
                    output.Write("\n<TD height=\"5\"></TD>");
                    output.Write("\n</TR>");
                }

                if (!_mutualExclusion){
                    if (pdmOption)
                    {
                        PdmCheckBox.RenderControl(output);
                        output.Write("&nbsp;&nbsp;");
                    }
                    if (pdvOption)
                    {
                        PdvCheckBox.RenderControl(output);
                    }
                }
                else
                {
                    output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                    output.Write("\n<td class=\"txtBlanc11Bold\">");
                    _checkBoxsMutualExclusion.RenderControl(output);
                }
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Option auto-promo Evaliant
            if(autopromoEvaliantOption) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                AutopromoEvaliantCheckBox.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Eléments personnalisés
            if(personalizedElementsOption) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");

                bool withAdvertisers = false;
                string tempString = "";
                if(customerWebSession.SecondaryProductUniverses.Count > 0 && customerWebSession.SecondaryProductUniverses.ContainsKey(0) && customerWebSession.SecondaryProductUniverses[0].Contains(0)) {
                    tempString = customerWebSession.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
                    if(tempString != null && tempString.Length > 0) withAdvertisers = true;
                }
                else if(customerWebSession.SecondaryProductUniverses.Count > 0 && customerWebSession.SecondaryProductUniverses.ContainsKey(1) && customerWebSession.SecondaryProductUniverses[1].Contains(0)) {
                    tempString = customerWebSession.SecondaryProductUniverses[1].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
                    if(tempString != null && tempString.Length > 0) withAdvertisers = true;
                }
                PersonalizedElementsCheckBox.Enabled = withAdvertisers;

                PersonalizedElementsCheckBox.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Option products for APPM
            if(productsOption && showProduct) {
                if(products.Items.Count > 0) {
                    output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                    output.Write("\n<td class=\"txtBlanc11Bold\">");
                    output.Write(GestionWeb.GetWebWord(1164, customerWebSession.SiteLanguage) + " : ");
                    output.Write("\n</td>");
                    output.Write("\n</tr>");
                    output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                    output.Write("\n<td>");
                    products.RenderControl(output);
                    output.Write("\n</td>");
                    output.Write("\n</tr>");
                    output.Write("\n<TR>");
                    output.Write("\n<TD height=\"5\"></TD>");
                    output.Write("\n</TR>");
                }
            }
            #endregion

            #region Result format
            if(_resultFormat) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td class=\"txtBlanc11Bold\">");
                graphRadioButton.RenderControl(output);
                if(graphRadioButton.Visible) output.Write("<A onmouseover=\"graph.src = '/App_Themes/" + themeName + "/Images/Common/Button/chart_down.gif';\" onclick=\"graphRadioButton.checked=true;\" onmouseout=\"graph.src = '/App_Themes/" + themeName + "/Images/Common/Button/chart_up.gif';\" href=\"#\"><IMG id=graph title=\"" + ChartTitle + "\" src=\"/App_Themes/" + themeName + "/Images/Common/Button/chart_up.gif\" border=0 ></A>&nbsp;");
                tableRadioButton.RenderControl(output);
                if(tableRadioButton.Visible) output.Write("<A onmouseover=\"table.src = '/App_Themes/" + themeName + "/Images/Common/Button/table_down.gif';\" onclick=\"tableRadioButton.checked=true;\" onmouseout=\"table.src = '/App_Themes/" + themeName + "/Images/Common/Button/table_up.gif';\" href=\"#\"><IMG id=table title=\"" + TableTitle + "\" src=\"/App_Themes/" + themeName + "/Images/Common/Button/table_up.gif\" border=0 ></A>");
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Zoom Graphic
            if(ZoomGraphic && _zoomGraphicCheckBox.Visible) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td class=\"txtBlanc11Bold\">");
                _zoomGraphicCheckBox.RenderControl(output);
                output.Write("<A onmouseover=\"zoom.src = '/App_Themes/" + themeName + "/Images/Common/Button/loupe_zoom_down.gif';\" ");
                output.Write("onclick=\"" + _zoomGraphicCheckBox.ID + ".checked=!" + _zoomGraphicCheckBox.ID + ".checked;\" ");
                output.Write("onmouseout=\"zoom.src = '/App_Themes/" + themeName + "/Images/Common/Button/loupe_zoom_up.gif';\" ");
                output.Write("href=\"#\"><IMG id=\"zoom\" src=\"/App_Themes/" + themeName + "/Images/Common/Button/loupe_zoom_up.gif\" border=0 ></A> ");
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Total comparaison (AS)
            if(TotalChoice && _totalChoiceRadioButtonList.Items.Count > 0) {
                output.Write("\n<tr  >");
                output.Write("\n<td class=\"txtBlanc11Bold\">");
                _totalChoiceRadioButtonList.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Options Results Table Types (An. Dispositifs)
            if(ResultsTableTypesOptions) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                _resultsTableTypesWebControl.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Options Detail Advertiser Brand Product
            if(DetailAdvertiserBrandProductOptions) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                _detailAdvertiserBrandProductWebControl.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

            #region Initialisation
            if(this._initializeProductWebControl != null 
                && 
                (this._inializeAdvertiserOption || this.InializeProductOption || this.InializeSlogansOption)
                ) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                this._initializeProductWebControl.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            if(this._initializeMediaWebControl != null && this.InializeMediaOption) {
                output.Write("\n<tr class=\"backGroundOptionsPadding\" >");
                output.Write("\n<td>");
                this._initializeMediaWebControl.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
            #endregion

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
        protected bool CanShowResult(WebSession webSession, ResultPageInformation current) {
            switch(webSession.CurrentModule) {
                case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    return CanShowPortofolioResult(webSession, current);
                case WebConstantes.Module.Name.BILAN_CAMPAGNE:
                    if((webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE)
                   && current.Id == FrameWorkResults.APPM.mediaPlanByVersion && !webSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_SLOGAN_ACCESS_FLAG)) return false;
                    else return true;
                default: return true;
            }
        }
        /// <summary>
        /// Determine si un résultat doit être montré.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="current">Page en cours</param>
        protected bool CanShowPortofolioResult(WebSession webSession, ResultPageInformation current) {
            #region VehicleInformation
            VehicleInformation vehicleInformation = VehiclesInformation.Get(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
            #endregion

            switch(vehicleInformation.Id) {
                case ClassificationCst.DB.Vehicles.names.directMarketing:
                case ClassificationCst.DB.Vehicles.names.internet:
                    return ((current.Id == FrameWorkResults.Portofolio.SYNTHESIS || current.Id == FrameWorkResults.Portofolio.DETAIL_PORTOFOLIO));
                case ClassificationCst.DB.Vehicles.names.outdoor:
                case ClassificationCst.DB.Vehicles.names.instore:
                case ClassificationCst.DB.Vehicles.names.cinema:
                case ClassificationCst.DB.Vehicles.names.adnettrack:
                case ClassificationCst.DB.Vehicles.names.evaliantMobile:
                    return (current.Id == FrameWorkResults.Portofolio.SYNTHESIS || current.Id == FrameWorkResults.Portofolio.DETAIL_PORTOFOLIO || (current.Id == FrameWorkResults.Portofolio.CALENDAR && webSession.CustomerPeriodSelected.IsSliding4M));
                case ClassificationCst.DB.Vehicles.names.others:
                case ClassificationCst.DB.Vehicles.names.tv:
                case ClassificationCst.DB.Vehicles.names.radio:
                case ClassificationCst.DB.Vehicles.names.press:
                case ClassificationCst.DB.Vehicles.names.newspaper:
                case ClassificationCst.DB.Vehicles.names.magazine:
                case ClassificationCst.DB.Vehicles.names.internationalPress:
                    return (current.Id == FrameWorkResults.Portofolio.SYNTHESIS || current.Id == FrameWorkResults.Portofolio.DETAIL_PORTOFOLIO || (webSession.CustomerPeriodSelected.IsSliding4M));
                default: throw new Exceptions.ResultsOptionsWebControlException("ResultsOptionsWebControl : Vehicle unknown.");
            }
        }

        /// <summary>
        /// Set result pages options
        /// </summary>
        protected void SetResultPageOption() {
            // Création des options concernant le choix du resultat
            resultsPages = new DropDownList();
            resultsPages.ID = "_resultsPages";
            resultsPages.CssClass = cssClass;
            resultsPages.AutoPostBack = autoPostBackOption;
            List<long> resultToShow = new List<long>();
            List<ResultPageInformation> resultPages = ((TNS.AdExpress.Domain.Web.Navigation.Module)customerWebSession.CustomerLogin.GetModule(customerWebSession.CurrentModule)).GetValidResultsPage(_selectedMediaUniverse);

            foreach(ResultPageInformation current in resultPages) {
                if(!CanShowResult(customerWebSession, current)) continue;
                resultToShow.Add(current.Id);
                resultsPages.Items.Add(new ListItem(GestionWeb.GetWebWord((int)current.IdWebText, customerWebSession.SiteLanguage), current.Id.ToString()));
            }

            SetDefaultTab(resultToShow);
            Controls.Add(resultsPages);
        }

        /// <summary>
        /// Set default current tab
        /// </summary>
        /// <param name="resultToShow">tab list</param>
        /// <remarks>Synthesis tab is define by default for portofolio module depending on poeriod selected or vehicle</remarks>
        protected void SetDefaultTab(List<long> resultToShow) {
            switch(customerWebSession.CurrentModule) {
                case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    if(resultToShow != null && resultToShow.Count > 0 && resultToShow.Contains(customerWebSession.CurrentTab)) {
                        resultsPages.Items.FindByValue(customerWebSession.CurrentTab.ToString()).Selected = true;
                    }
                    else {
                        customerWebSession.CurrentTab = FrameWorkResults.Portofolio.SYNTHESIS;
                        resultsPages.Items.FindByValue(FrameWorkResults.Portofolio.SYNTHESIS.ToString()).Selected = true;
                        customerWebSession.Save();
                    }
                    break;
                default: resultsPages.Items.FindByValue(customerWebSession.CurrentTab.ToString()).Selected = true; break;
            }
        }

        /// <summary>
        /// Is Valid Period Comparative
        /// </summary>
        /// <returns></returns>
        protected bool IsValidPeriodComparative() {

            if (customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {
                DateTime dtBegin = WebFunctions.Dates.getPeriodBeginningDate(customerWebSession.PeriodBeginningDate, customerWebSession.PeriodType);
                DateTime dtEnd = WebFunctions.Dates.getPeriodEndDate(customerWebSession.PeriodEndDate, customerWebSession.PeriodType);

                //Check Year
                return (DateTime.Compare(new DateTime(DateTime.Now.Year - 1, 1, 1, 0, 0, 0), dtBegin) < 0
                    && dtBegin.Year == dtEnd.Year);
            }
            else return customerWebSession.ComparativeStudy;
        }
        #endregion

    }
}