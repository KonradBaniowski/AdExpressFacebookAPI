using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.Web.Controls.Headers {
    /// <summary>
    /// Composant permettant de sélectionner 1 niveau de détail colonne se basant sur les fichiers de configuration XML
    /// </summary>
    [ToolboxData("<{0}:GenericColumnLevelDetailSelectionWebControl runat=server></{0}:GenericColumnLevelDetailSelectionWebControl>")]
    public class GenericColumnLevelDetailSelectionWebControl : System.Web.UI.WebControls.WebControl {

        #region Variables
        /// <summary>
        /// Nombre de niveaux de détaille colonne personnalisés
        /// </summary>
        protected int _nbColumnDetailLevelItemList = 1;
        /// <summary>
        /// Liste des niveaux de détaille colonne personnalisés
        /// </summary>
        protected ArrayList _columnDetailLevelItemList = new ArrayList();
        ///<summary>
        /// Session du client
        /// </summary>
        ///  <label>_customerWebSession</label>
        protected WebSession _customerWebSession = null;
        /// <summary>
        /// Module Courrant
        /// </summary>
        protected Module _currentModule = null;
        ///<summary>
        /// Type des niveaux de détail colonne
        /// </summary>
        ///  <label>_genericDetailLevelType</label>
        protected WebConstantes.GenericDetailLevel.Type _genericColumnDetailLevelType = WebConstantes.GenericDetailLevel.Type.mediaSchedule;
        /// <summary>
        /// Niveau de détail colonne utilisé
        /// </summary>
        protected GenericDetailLevel _genericColumnDetailLevel = null;
        /// <summary>
        /// Couleur de fond du composant
        /// </summary>
        protected string _backgroundColor = "#ffffff";
        /// <summary>
        /// Classe Css pour le libellé de la liste des niveaux colonne
        /// </summary>
        protected string _cssListLabel = "txtGris11Bold";
        /// <summary>
        /// Classe Css pour les listbox
        /// </summary>
        protected string _cssListBox = "txtNoir11Bold";
        #endregion

        #region Variables MMI
        /// <summary>
        /// Choix du niveau de détail colonne
        /// </summary>
        public DropDownList _columnDetailLevel;
        #endregion

        #region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
        public GenericColumnLevelDetailSelectionWebControl() : base() {
			this.EnableViewState = true;
		}
		#endregion

        #region Accesseurs

        #region Css
        /// <summary>
        /// Obtient ou définit la couleur de fond du composant
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue("#ffffff"),
        Description("Couleur de fond du composant")]
        public string BackGroundColor {
            get { return (_backgroundColor); }
            set { _backgroundColor = value; }
        }
        /// <summary>
        /// Obtient ou définit la classe Css pour le libellé de la liste des niveaux par défaut
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue("txtGris11Bold"),
        Description("Classe Css pour le libellé de la liste des niveaux par défaut")]
        public string CssListLabel {
            get { return (_cssListLabel); }
            set { _cssListLabel = value; }
        }
        /// <summary>
        /// Obtient ou définit la classe Css pour les listbox
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue("txtNoir11Bold"),
        Description("Classe Css pour les listbox")]
        public string CssListBox {
            get { return (_cssListBox); }
            set { _cssListBox = value; }
        }
        #endregion

        #region Propriétés
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
        /// Obtient ou définit le nombre de niveaux de détaille personnalisés
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue("4")]
        public int NbColumnDetailLevelItemList {
            get { return (_nbColumnDetailLevelItemList); }
            set {
                if (value < 1 || value > 1) throw (new ArgumentOutOfRangeException("The value of NbDetailLevelItemList must be between 1 and 4"));
                _nbColumnDetailLevelItemList = value;
            }
        }
        
        /// <summary>
        /// Définit la session du client
        /// </summary>
        [Bindable(false)]
        public WebSession CustomerWebSession {
            set { _customerWebSession = value; }
        }
        #endregion

        #endregion

        #region Evènements

        #region Init
        /// <summary>
        /// Ibnitialization
        /// </summary>
        /// <param name="e">Parameters</param>
        protected void Init(EventArgs e) {
            base.OnInit(e);
        }
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguements</param>
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);

            _currentModule = ModulesList.GetModule(_customerWebSession.CurrentModule);

            #region on vérifie que le niveau sélectionné à le droit d'être utilisé
            bool canAddDetail = false;
            try {
                canAddDetail = CanAddDetailLevel(_customerWebSession.GenericColumnDetailLevel, _customerWebSession.CurrentModule);
            }
            catch { }
            if (!canAddDetail) {
                // Niveau de détail par défaut
                ArrayList levelsIds = new ArrayList();
                levelsIds.Add((int)DetailLevelItemInformation.Levels.media);
                _customerWebSession.GenericColumnDetailLevel = new GenericDetailLevel(levelsIds, WebConstantes.GenericDetailLevel.SelectedFrom.unknown);
            }
            #endregion

            #region Niveau de détaille colonne
            if (_nbColumnDetailLevelItemList == 1) {
                _columnDetailLevel = new DropDownList();
                ColumnDetailLevelItemInit(_columnDetailLevel);
            }
            #endregion

        }
        #endregion

        #region Load
        /// <summary>
        /// Chargement du composant
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            ArrayList levels = new ArrayList();
            
            _genericColumnDetailLevel = _customerWebSession.GenericColumnDetailLevel;

            #region Gestion de la sélection
            if (Page.IsPostBack) {
                if (_nbColumnDetailLevelItemList == 1 && int.Parse(_columnDetailLevel.SelectedValue) >= 0) {
                    levels.Add(int.Parse(_columnDetailLevel.SelectedValue));
                }
                if (levels.Count > 0) {
                    _genericColumnDetailLevel = new GenericDetailLevel(levels, WebConstantes.GenericDetailLevel.SelectedFrom.customLevels);
                }
            }
            else {
                if (_nbColumnDetailLevelItemList == 1 && _genericColumnDetailLevel.GetNbLevels == 1 && _genericColumnDetailLevel.LevelIds[0] != null) {
                    _columnDetailLevel.SelectedValue = ((DetailLevelItemInformation.Levels)_genericColumnDetailLevel.LevelIds[0]).GetHashCode().ToString();
                }
            }
            #endregion

            _customerWebSession.GenericColumnDetailLevel = _genericColumnDetailLevel;
        }
        #endregion

        #region Prérender
        /// <summary>
        /// Préparation du rendu des niveaux de détails personnalisés.
        /// </summary>
        /// <param name="e">Sender</param>
        protected override void OnPreRender(EventArgs e) {
            base.OnPreRender(e);
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output) {
            output.Write("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" bgcolor=\"" + _backgroundColor + "\"  width=\"" + this.Width + "\">");
            output.Write("<tr>");
            output.Write("<td class=\"" + _cssListLabel + "\">" + GestionWeb.GetWebWord(2300, _customerWebSession.SiteLanguage) + "</td>");
            output.Write("</tr>");
            output.Write("<tr>");
            output.Write("<td>");
            // Liste par défaut
            _columnDetailLevel.RenderControl(output);
            output.Write("</td>");
            output.Write("</tr>");
            //// Espace blanc
            //output.Write("<tr>");
            //output.Write("<td><img src=\"/App_Themes/"+WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name+"/Images/Common/pixel.gif\" border=\"0\" height=\"10\"></td>");
            //output.Write("</tr>");
            output.Write("\r\n</table>");
        }
        #endregion

        #endregion

        #region Méthode privée

        #region DetailLevelItemInit
        /// <summary>
        /// Initialise une sélection d'éléments de niveau de détaille
        /// </summary>
        /// <param name="dropDownList">Liste</param>
        protected void ColumnDetailLevelItemInit(DropDownList dropDownList) {
            
            dropDownList.Width = new System.Web.UI.WebControls.Unit(this.Width.Value);
            dropDownList.ID = "ColumnDetail_" + this.ID;
            dropDownList.AutoPostBack = false;
            dropDownList.CssClass = _cssListBox;
            dropDownList.Items.Add(new ListItem("-------", "-1"));
            ArrayList AllowedColumnDetailLevelItems = GetAllowedColumnDetailLevelItems();
            foreach (DetailLevelItemInformation currentColumnDetailLevelItem in AllowedColumnDetailLevelItems) {
                if (CanAddDetailLevelItem(currentColumnDetailLevelItem, _customerWebSession.CurrentModule)) {
                    dropDownList.Items.Add(new ListItem(GestionWeb.GetWebWord(currentColumnDetailLevelItem.WebTextId, _customerWebSession.SiteLanguage), currentColumnDetailLevelItem.Id.GetHashCode().ToString()));
                }
            }
            _columnDetailLevelItemList.Add(dropDownList);

            Controls.Add(dropDownList);
        }
        #endregion

        #region CanAddDetailLevelItem
        /// <summary>
		/// Test si l'élément de niveau de détail peut être montré
		/// </summary>
		/// <param name="currentColumnDetailLevelItem">Elément de niveau de détail</param>
		/// <param name="module">Module</param>
		/// <returns>True si oui false sinon</returns>
        private bool CanAddDetailLevelItem(DetailLevelItemInformation currentColumnDetailLevelItem, Int64 module) {

            switch (currentColumnDetailLevelItem.Id) {
                case DetailLevelItemInformation.Levels.mediaSeller:
                    return (!_customerWebSession.isCompetitorAdvertiserSelected());
                default:
                    return (true);
            }

        }
        #endregion

        #region CanAddDetailLevel
        /// <summary>
        /// Test si un niveau de détail peut être montré
        /// </summary>
        /// <param name="currentColumnDetailLevel">Niveau de détail</param>
        /// <param name="module">Module courrant</param>
        /// <returns>True s'il peut être ajouté</returns>
        protected virtual bool CanAddDetailLevel(GenericDetailLevel currentColumnDetailLevel, Int64 module) {
            ArrayList AllowedDetailLevelItems = GetAllowedColumnDetailLevelItems();
            foreach (DetailLevelItemInformation currentColumnDetailLevelItem in currentColumnDetailLevel.Levels) {
                if (!AllowedDetailLevelItems.Contains(currentColumnDetailLevelItem)) return (false);
                if (!CanAddDetailLevelItem(currentColumnDetailLevelItem, module)) return (false);
            }
            return (true);
        }
        #endregion

        #region GetAllowedColumnDetailLevelItems
        /// <summary>
        /// Retourne les éléments des niveaux de détail colonne autorisés
        /// </summary>
        /// <returns>Niveaux de détail colonne</returns>
        private ArrayList GetAllowedColumnDetailLevelItems() {

            List<DetailLevelItemInformation.Levels> vehicleAllowedDetailLevelList = GetVehicleAllowedDetailLevelItems();
            ArrayList allowedColumnDetailLevelList = _currentModule.AllowedColumnDetailLevelItems;
            ArrayList list = new ArrayList();

            List<DetailLevelItemInformation.Levels> vehicleAllowedColumnLevelList = GetVehicleAllowedColumnsLevelItems();

            foreach (DetailLevelItemInformation currentLevel in allowedColumnDetailLevelList)
                if (vehicleAllowedDetailLevelList.Contains(currentLevel.Id)
                    && vehicleAllowedColumnLevelList.Contains(currentLevel.Id))
                    list.Add(currentLevel);

            return list;

        }
        /// <summary>
        /// Return allowed detail level list for vehicle list seleceted
        /// </summary>
        /// <returns>Detail level list</returns>
        private List<DetailLevelItemInformation.Levels> GetVehicleAllowedDetailLevelItems() {

            List<Int64> vehicleList = new List<Int64>();
            string listStr = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
            if (listStr != null && listStr.Length > 0) {
                string[] list = listStr.Split(',');
                for (int i = 0; i < list.Length; i++)
                    vehicleList.Add(Convert.ToInt64(list[i]));
            }
            else {
                //When a vehicle is not checked but one or more category, this get the vehicle correspondly
                string Vehicle = ((LevelInformation)_customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
                vehicleList.Add(Convert.ToInt64(Vehicle));
            }           
            return VehiclesInformation.GetCommunDetailLevelList(vehicleList);
        }

        /// <summary>
        /// Return allowed column detail level list for vehicle list seleceted
        /// </summary>
        /// <returns>Detail level list</returns>
        private List<DetailLevelItemInformation.Levels> GetVehicleAllowedColumnsLevelItems()
        {

            List<Int64> vehicleList = new List<Int64>();
            string listStr = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
            if (listStr != null && listStr.Length > 0)
            {
                string[] list = listStr.Split(',');
                for (int i = 0; i < list.Length; i++)
                    vehicleList.Add(Convert.ToInt64(list[i]));
            }
            else
            {
                //When a vehicle is not checked but one or more category, this get the vehicle correspondly
                string Vehicle = ((LevelInformation)_customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
                vehicleList.Add(Convert.ToInt64(Vehicle));
            }
            return VehiclesInformation.GetColumnsDetailLevelList(vehicleList);
        }
        #endregion

        #region CheckProductDetailLevelAccess
        /// <summary>
        /// Vérifie si le client à le droit de voir un détail produit dans les plan media
        /// </summary>
        /// <returns>True si oui false sinon</returns>
        private bool CheckProductDetailLevelAccess() {
            return (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG));
           
        }
        #endregion

        #endregion
    }
}
