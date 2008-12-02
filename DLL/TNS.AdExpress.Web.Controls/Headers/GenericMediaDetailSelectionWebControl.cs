#region Informations
// Auteur: D. Mussuma 
// Date de création: 01/12/2008
// Date de modification: 

#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
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
using TNS.AdExpress.Web.Core.DataAccess.Session;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.Web.Controls.Headers {
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:GenericMediaDetailSelectionWebControl runat=server></{0}:GenericMediaDetailSelectionWebControl>")]
    public class GenericMediaDetailSelectionWebControl : WebControl {

     
	//    #region Variables
	//    /// <summary>
	//    /// Nombre de niveaux de détaille personnalisés
	//    /// </summary>
	//    protected int _nbDetailLevelItemList = 2;
	//    /// <summary>
	//    /// Liste des niveaux de détaille personnalisés
	//    /// </summary>
	//    protected ArrayList _DetailLevelItemList = new ArrayList();
	//    ///<summary>
	//    /// Session du client
	//    /// </summary>
	//    ///  <label>_customerWebSession</label>
	//    protected WebSession _customerWebSession = null;
	//    /// <summary>
	//    /// Module Courrant
	//    /// </summary>
	//    protected Module _currentModule = null;
	//    /// <summary>
	//    /// Couleur de fond du composant
	//    /// </summary>
	//    protected string _backgroundColor = "#ffffff";
	//    /// <summary>
	//    /// Classe Css pour le libellé de la liste des niveaux par défaut
	//    /// </summary>
	//    protected string _cssDefaultListLabel = "txtGris11Bold";
	//    /// <summary>
	//    /// Classe Css pour le libellé des listes personnalisées
	//    /// </summary>
	//    protected string _cssListLabel = "txtViolet11Bold";
	//    /// <summary>
	//    /// Classe Css pour les listbox
	//    /// </summary>
	//    protected string _cssListBox = "txtNoir11Bold";
	//    /// <summary>
	//    /// Classe Css pour le titre de la section personnalisée
	//    /// </summary>
	//    protected string _cssCustomSectionTitle = "txtViolet11Bold";
                
	//    ///<summary>
	//    /// Type des niveaux de détail
	//    /// </summary>
	//    ///  <label>_genericDetailLevelType</label>
	//    protected WebConstantes.GenericDetailLevel.Type _genericDetailLevelType = WebConstantes.GenericDetailLevel.Type.mediaSchedule;
	//    ///<summary>
	//    /// Profile du composant
	//    /// </summary>
	//    ///  <label>_componentProfile</label>
	//    protected WebConstantes.GenericDetailLevel.ComponentProfile _componentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.media;       
	//    /// <summary>
	//    /// Force le composant à s'initialiser avec les valeurs du module
	//    /// </summary>
	//    protected Int64 _forceModuleId = -1;
	//    /// <summary>
	//    /// Niveau de détail utilisé
	//    /// </summary>
	//    protected GenericDetailLevel _customerGenericDetailLevel = null;

	//    /// <summary>
	//    /// Indique s'il faut obliger le client à selection le niveau support
	//    /// </summary>
	//    protected bool _forceMediaSelection = true;

	//    #endregion

	//    #region Variables MMI
	//    /// <summary>
	//    /// Choix du niveau de détail media par défaut
	//    /// </summary>
	//    public DropDownList _defaultDetail;
	//    /// <summary>
	//    /// Choix du niveau de détail media par défaut
	//    /// </summary>
	//    public DropDownList _customDetail;
	//    /// <summary>
	//    /// Choix du niveau de détail L1
	//    /// </summary>
	//    public DropDownList _l1Detail;
	//    /// <summary>
	//    /// Choix du niveau de détail L2
	//    /// </summary>
	//    public DropDownList _l2Detail;

	//    /// <summary>
	//    /// Choix du niveau de détail L3
	//    /// </summary>
	//    public DropDownList _l3Detail;
	//    #endregion
       
	//    #region Constructeur
	//    /// <summary>
	//    /// Constructeur
	//    /// </summary>
	//    public GenericMediaDetailSelectionWebControl(): base() {
	//        this.EnableViewState = true;
	//    }
	//    #endregion

	//    #region Accesseurs
	//    /// <summary>
	//    /// Force le composant à s'initialiser avec les valeurs du module
	//    /// </summary>
	//    public Int64 ForceModuleId {
	//        set { _forceModuleId = value; }
	//    }
	//    /// <summary>
	//    /// Obtient ou définit le type des niveaux de détail
	//    /// </summary>
	//    [Bindable(true),
	//    Category("Appearance"),
	//    Description("Type des niveaux de détail")]
	//    public WebConstantes.GenericDetailLevel.Type GenericDetailLevelType {
	//        get { return (_genericDetailLevelType); }
	//        set { _genericDetailLevelType = value; }
	//    }
	//    /// <summary>
	//    /// Obtient ou définit Le profile du coposant
	//    /// </summary>
	//    [Bindable(true),
	//    Category("Appearance"),
	//    Description("Profile du composant"),
	//    DefaultValue("media")
	//    ]
	//    public WebConstantes.GenericDetailLevel.ComponentProfile GenericDetailLevelComponentProfile {
	//        get { return (_componentProfile); }
	//        set { _componentProfile = value; }
	//    }

       
      
	//    #region Css
	//    /// <summary>
	//    /// Obtient ou définit la couleur de fond du composant
	//    /// </summary>
	//    [Bindable(true),
	//    Category("Appearance"),
	//    DefaultValue("#ffffff"),
	//    Description("Couleur de fond du composant")]
	//    public string BackGroundColor {
	//        get { return (_backgroundColor); }
	//        set { _backgroundColor = value; }
	//    }

	//    /// <summary>
	//    /// Obtient ou définit la classe Css pour le libellé de la liste des niveaux par défaut
	//    /// </summary>
	//    [Bindable(true),
	//    Category("Appearance"),
	//    DefaultValue("txtGris11Bold"),
	//    Description("Classe Css pour le libellé de la liste des niveaux par défaut")]
	//    public string CssDefaultListLabel {
	//        get { return (_cssDefaultListLabel); }
	//        set { _cssDefaultListLabel = value; }
	//    }
	//    /// <summary>
	//    /// Obtient ou définit la classe Css pour le libellé des listes personnalisées
	//    /// </summary>
	//    [Bindable(true),
	//    Category("Appearance"),
	//    DefaultValue("txtViolet11Bold"),
	//    Description("Classe Css pour le libellé des listes personnalisées")]
	//    public string CssListLabel {
	//        get { return (_cssListLabel); }
	//        set { _cssListLabel = value; }
	//    }
	//    /// <summary>
	//    /// Obtient ou définit la classe Css pour les listbox
	//    /// </summary>
	//    [Bindable(true),
	//    Category("Appearance"),
	//    DefaultValue("txtNoir11Bold"),
	//    Description("Classe Css pour les listbox")]
	//    public string CssListBox {
	//        get { return (_cssListBox); }
	//        set { _cssListBox = value; }
	//    }
	//    /// <summary>
	//    /// Obtient ou définit la classe Css pour le titre de la section personnalisée
	//    /// </summary>
	//    [Bindable(true),
	//    Category("Appearance"),
	//    DefaultValue("txtViolet11Bold"),
	//    Description("Classe Css pour le titre de la section personnalisée")]
	//    public string CssCustomSectionTitle {
	//        get { return (_cssCustomSectionTitle); }
	//        set { _cssCustomSectionTitle = value; }
	//    }
	//    #endregion

	//    /// <summary>
	//    /// Obtient ou définit le nombre de niveaux de détaille personnalisés
	//    /// </summary>
	//    [Bindable(true),
	//    Category("Appearance"),
	//    DefaultValue("3")]
	//    public int NbDetailLevelItemList {
	//        get { return (_nbDetailLevelItemList); }
	//        set {
	//            if (value < 1 || value > 3) throw (new ArgumentOutOfRangeException("The value of NbDetailLevelItemList must be between 1 and 3"));
	//            _nbDetailLevelItemList = value;
	//        }
	//    }


	//    /// <summary>
	//    /// Définit la session du client
	//    /// </summary>
	//    [Bindable(false)]
	//    public WebSession CustomerWebSession {
	//        set { _customerWebSession = value; }
	//    }		
	//    /// <summary>
	//    /// Indique s'il faut obliger le client à selection le niveau support
	//    /// </summary>
	//    [Bindable(true), DefaultValue(true)]
	//    public bool ForceMediaSelection {
	//        set { _forceMediaSelection = value; }
	//    }
	//    #endregion

	//    #region Javascript
	//    /// <summary>
	//    /// Génére les javascripts utilisés pour le controle des listes
	//    /// </summary>
	//    /// <returns>Code Javascript</returns>
	//    private string GetLevelJavaScript() {
	//        StringBuilder script = new StringBuilder(2000);

	//        script.Append("<script language=\"JavaScript\">");          

	//        //script.Append("\r\nfunction setLevel(detail");
	//        script.Append("\r\nfunction setLevel(");
	//        for (int i = 1; i <= _nbDetailLevelItemList; i++) {
	//            if (i > 1) script.Append(",");
	//            script.Append("l" + i.ToString() + "Detail");
	//        }
	//        script.Append("){");
	//        //script.Append("\r\n\t intSelect(detail);");
	//        for (int i = 1; i <= _nbDetailLevelItemList; i++) script.Append("\r\n\t intSelect(l" + i.ToString() + "Detail);");
	//        script.Append("\r\n}");

	//        script.Append("\r\n function intSelect(id){");
	//        script.Append("\r\n\t var oContent = document.all.item(id);");
	//        script.Append("\r\n\t oContent.value=-1;");
	//        script.Append("\r\n}");

	//        script.Append("\r\n function setN(detail,customDetail){");
	//        script.Append("\r\n\t intSelect(detail);");
	//        //script.Append("\r\n\t intSelect(customDetail);");
	//        script.Append("\r\n}");

	//        for (int i = 1; i <= _nbDetailLevelItemList; i++) {
	//            script.Append("\r\nfunction setN" + i.ToString() + "(detail,customDetail");
	//            for (int j = 1; j <= _nbDetailLevelItemList; j++) script.Append(",N" + j.ToString());
	//            script.Append("){");
	//            for (int k = 1; k <= _nbDetailLevelItemList; k++) script.Append("\r\n\t var oN" + k.ToString() + "=document.all.item(N" + k.ToString() + ");");
	//            script.Append("\r\n\t var oDetail=document.all.item(detail);");
	//            script.Append("\r\n\t setN(detail,customDetail);");
	//            if (i == 1) {
	//                if (_nbDetailLevelItemList >= 2) script.Append("\r\n\t if(oN1.value==oN2.value)oN2.value=-1;");
	//                if (_nbDetailLevelItemList >= 3) script.Append("\r\n\t if(oN1.value==oN3.value)oN3.value=-1;");
	//                if (_nbDetailLevelItemList >= 4) script.Append("\r\n\t if(oN1.value==oN4.value)oN4.value=-1;");
	//                if (_forceMediaSelection && _nbDetailLevelItemList >= 2 && CanAddDetailLevelItem(DetailLevelItemInformation.Levels.media,_currentModule)) script.Append("\r\n\t if(oN1.value!=-1 && oN2.value==-1 && oDetail.value==-1)oN2.value=" + DetailLevelItemInformation.Levels.media.GetHashCode() + ";");  
	//            }
	//            if (i == 2) {
	//                script.Append("\r\n\t if(oN2.value==oN1.value)oN1.value=-1;");
	//                if (_nbDetailLevelItemList >= 3) script.Append("\r\n\t if(oN2.value==oN3.value)oN3.value=-1;");
	//                if (_nbDetailLevelItemList >= 4) script.Append("\r\n\t if(oN2.value==oN4.value)oN4.value=-1;");
	//                if (_forceMediaSelection && CanAddDetailLevelItem(_customerWebSession.MediaSelectionParent, _currentModule)) script.Append("\r\n\t if(oN2.value!=-1 && oN1.value==-1 && oDetail.value==-1)oN1.value=" + _customerWebSession.MediaSelectionParent.GetHashCode() + ";");  
	//            }
	//            if (i == 3) {
	//                script.Append("\r\n\t if(oN3.value==oN1.value)oN1.value=-1;");
	//                script.Append("\r\n\t if(oN3.value==oN2.value)oN2.value=-1;");
	//                if (_nbDetailLevelItemList >= 4) script.Append("\r\n\t if(oN3.value==oN4.value)oN4.value=-1;");
	//            }
               
	//            script.Append("\r\n}");
	//        }
	//        script.Append("\r\n</script>");

	//        return (script.ToString());
	//    }
	//    #endregion

	//    #region Evènements

	//    #region Init
	//    /// <summary>
	//    /// Ibnitialization
	//    /// </summary>
	//    /// <param name="e">Parameters</param>
	//    protected void Init(EventArgs e) {
	//        base.OnInit(e);
	//    }
	//    /// <summary>
	//    /// Initialisation
	//    /// </summary>
	//    /// <param name="e">Arguements</param>
	//    protected override void OnInit(EventArgs e) {
	//        string onChange = "";
	//        base.OnInit(e);

	//        if (_forceModuleId < 0)
	//            _currentModule = ModulesList.GetModule(_customerWebSession.CurrentModule);
	//        else
	//            _currentModule = ModulesList.GetModule(_forceModuleId);

	//        #region on vérifie que le niveau sélectionné à le droit d'être utilisé
	//        bool canAddDetail = false;
	//        switch (_componentProfile) {
	//            case WebConstantes.GenericDetailLevel.ComponentProfile.media:
	//                try {
	//                    canAddDetail = CanAddDetailLevel(_customerWebSession.GenericMediaSelectionDetailLevel, _customerWebSession.CurrentModule);
	//                }
	//                catch { }
	//                if (!canAddDetail) {
	//                    // Niveau de détail par défaut
	//                    ArrayList levelsIds = new ArrayList();
	//                    switch (_customerWebSession.CurrentModule) {                           
	//                        default:
	//                            levelsIds.Add((int)DetailLevelItemInformation.Levels.interestCenter);
	//                            levelsIds.Add((int)DetailLevelItemInformation.Levels.media);
	//                            break;
	//                    }
	//                    _customerWebSession.GenericMediaSelectionDetailLevel = new GenericDetailLevel(levelsIds, WebConstantes.GenericDetailLevel.SelectedFrom.unknown);
	//                }
	//                break;               
	//        }
	//        #endregion

	//        #region Niveau de détaille par défaut

	//        _defaultDetail = new DropDownList();
	//        _defaultDetail.EnableViewState = true;
	//        _defaultDetail.Width = new System.Web.UI.WebControls.Unit(this.Width.Value);
	//        _defaultDetail.ID = "defaultDetail_" + this.ID;
	//        _defaultDetail.AutoPostBack = false;
	//        _defaultDetail.CssClass = _cssListBox;

	//        _defaultDetail.Items.Add(new ListItem("-------", "-1"));
	//        int DefaultDetailLevelId = 0;
	//        List<GenericDetailLevel> DefaultDetailLevels = GetDefaultDetailLevels();
	//        foreach (GenericDetailLevel currentLevel in DefaultDetailLevels) {
	//            if (CanAddDetailLevel(currentLevel, _customerWebSession.CurrentModule))
	//                _defaultDetail.Items.Add(new ListItem(currentLevel.GetLabel(_customerWebSession.SiteLanguage), DefaultDetailLevelId.ToString()));
	//            DefaultDetailLevelId++;
	//        }
	//        Controls.Add(_defaultDetail);
	//        #endregion

	//        #region Niveau de détaille par personnalisé
          

	//        //_customDetail = new DropDownList();
	//        //_customDetail.Width = new System.Web.UI.WebControls.Unit(this.Width.Value - 4);
	//        //_customDetail.ID = "customDetail_" + this.ID;
	//        //_customDetail.AutoPostBack = false;
	//        //_customDetail.CssClass = _cssListBox;

	//        //_customDetail.Items.Add(new ListItem("-------", "-1"));
	//        //foreach (GenericDetailLevelSaved currentGenericLevel in genericDetailLevelsSaved) {
	//        //    if (CanAddDetailLevel(currentGenericLevel, _customerWebSession.CurrentModule) && currentGenericLevel.GetNbLevels <= _nbDetailLevelItemList) {
	//        //        _customDetail.Items.Add(new ListItem(currentGenericLevel.GetLabel(_customerWebSession.SiteLanguage), currentGenericLevel.Id.ToString()));
	//        //        _genericDetailLevelsSaved.Add(currentGenericLevel.Id, currentGenericLevel);
	//        //    }
	//        //}

	//        //Controls.Add(_customDetail);
	//        #endregion

	//        #region Niveau de détaille par défaut
	//        #region L1
	//        if (_nbDetailLevelItemList >= 1) {
	//            _l1Detail = new DropDownList();
	//            DetailLevelItemInit(_l1Detail, 1);
	//        }
	//        #endregion

	//        #region L2
	//        if (_nbDetailLevelItemList >= 2) {
	//            _l2Detail = new DropDownList();
	//            DetailLevelItemInit(_l2Detail, 2);
	//        }
	//        #endregion

	//        #region L3
	//        if (_nbDetailLevelItemList >= 3) {
	//            _l3Detail = new DropDownList();
	//            DetailLevelItemInit(_l3Detail, 3);
	//        }
	//        #endregion

           

	//        #endregion

	//        #region Ecriture des onchange

	//        #region OnChange _defaultDetail
	//        //onChange = "javascript:setLevel('" + _customDetail.ID + "'"; ;
	//        onChange = "javascript:setLevel("; 
	//        for (int i = 1; i <= _nbDetailLevelItemList; i++) {
	//            //onChange += ",'" + "l" + i.ToString() + "Detail_" + this.ID + "'";
	//            if (i > 1) onChange += ",";
	//            onChange += "'" + "l" + i.ToString() + "Detail_" + this.ID + "'";
	//        }
	//        onChange += ");";
	//        _defaultDetail.Attributes["onchange"] = onChange;
	//        #endregion

	//        #region OnChange _customDetail
	//        //onChange = "javascript:setLevel('" + _defaultDetail.ID + "'";
	//        //for (int i = 1; i <= _nbDetailLevelItemList; i++) {
	//        //    onChange += ",'" + "l" + i.ToString() + "Detail_" + this.ID + "'";
	//        //}
	//        //onChange += ");";
	//        //_customDetail.Attributes["onchange"] = onChange;
	//        #endregion

	//        #endregion

	//    }

	//    #endregion

	//    #region Load
	//    /// <summary>
	//    /// Chargement du composant
	//    /// </summary>
	//    /// <param name="e">Arguments</param>
	//    protected override void OnLoad(EventArgs e) {
	//        base.OnLoad(e);
	//        ArrayList levels = new ArrayList();
	//        switch (_componentProfile) {
	//            case WebConstantes.GenericDetailLevel.ComponentProfile.media:
	//                _customerGenericDetailLevel = _customerWebSession.GenericMediaSelectionDetailLevel;
	//                break;              
	//        }

	//        #region Gestion de la sélection
	//        if (Page.IsPostBack) {
	//            if (int.Parse(_defaultDetail.SelectedValue) >= 0) {
	//                if (Page.Request.Form.GetValues("defaultDetail_" + this.ID) != null && Page.Request.Form.GetValues("defaultDetail_" + this.ID).Length > 0) {
	//                    _customerGenericDetailLevel = GetDefaultDetailLevels()[int.Parse(Page.Request.Form.GetValues("defaultDetail_" + this.ID)[0])];
	//                }
	//                // Pb ICI TODO
	//                _customerGenericDetailLevel.FromControlItem = WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels;
	//            }
	//            //if (int.Parse(_customDetail.SelectedValue) >= 0) {
	//            //    if (Page.Request.Form.GetValues("customDetail_" + this.ID) != null && Page.Request.Form.GetValues("customDetail_" + this.ID).Length > 0) {
	//            //        _customerGenericDetailLevel = (GenericDetailLevel)_genericDetailLevelsSaved[Int64.Parse(Page.Request.Form.GetValues("customDetail_" + this.ID)[0])];
	//            //    }
	//            //    _customerGenericDetailLevel.FromControlItem = WebConstantes.GenericDetailLevel.SelectedFrom.savedLevels;
	//            //}
	//            if (_nbDetailLevelItemList >= 1 && int.Parse(_l1Detail.SelectedValue) >= 0) {
	//                levels.Add(int.Parse(_l1Detail.SelectedValue));
	//            }
	//            if (_nbDetailLevelItemList >= 2 && int.Parse(_l2Detail.SelectedValue) >= 0) {
	//                levels.Add(int.Parse(_l2Detail.SelectedValue));
	//            }
	//            if (_nbDetailLevelItemList >= 3 && int.Parse(_l3Detail.SelectedValue) >= 0) {
	//                levels.Add(int.Parse(_l3Detail.SelectedValue));
	//            }                
	//            if (levels.Count > 0) {

	//                if (_forceMediaSelection && levels.Count == 1) {
	//                    //On force la selection par défaut si il n'y a qu'un niveau personnalisé selectionné
	//                    _customerGenericDetailLevel.FromControlItem = WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels;
	//                    if (_nbDetailLevelItemList >= 3) _l3Detail.SelectedIndex = 0;
	//                    if (_nbDetailLevelItemList >= 2) _l2Detail.SelectedIndex = 0;
	//                    if (_nbDetailLevelItemList >= 1) _l1Detail.SelectedIndex = 0;
						
	//                }
	//                else _customerGenericDetailLevel = new GenericDetailLevel(levels, WebConstantes.GenericDetailLevel.SelectedFrom.customLevels);
	//            }
	//            switch (_customerGenericDetailLevel.FromControlItem) {
	//                case WebConstantes.GenericDetailLevel.SelectedFrom.customLevels:
	//                    if (levels.Count <= 0) {
	//                        if (_nbDetailLevelItemList >= 1 && _customerGenericDetailLevel.GetNbLevels >= 1 && _customerGenericDetailLevel.LevelIds[0] != null) {
	//                            _l1Detail.SelectedValue = ((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[0]).GetHashCode().ToString();
	//                        }
	//                        if (_nbDetailLevelItemList >= 2 && _customerGenericDetailLevel.GetNbLevels >= 2 && _customerGenericDetailLevel.LevelIds[1] != null) {
	//                            _l2Detail.SelectedValue = ((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[1]).GetHashCode().ToString();
	//                        }
	//                        if (_nbDetailLevelItemList >= 3 && _customerGenericDetailLevel.GetNbLevels >= 3 && _customerGenericDetailLevel.LevelIds[2] != null) {
	//                            _l3Detail.SelectedValue = ((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[2]).GetHashCode().ToString();
	//                        }                           
	//                    }
	//                    break;                   
	//                case WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels:
	//                    if (int.Parse(_defaultDetail.SelectedValue) < 0) {
	//                        int index = -1;
	//                        foreach (GenericDetailLevel currentLevel in GetDefaultDetailLevels()) {
	//                            if (CanAddDetailLevel(currentLevel, _customerWebSession.CurrentModule)) index++;
	//                            if (currentLevel.EqualLevelItems(_customerGenericDetailLevel)) _defaultDetail.SelectedValue = index.ToString();
	//                        }
	//                    }
	//                    break;
	//            }
	//        }
	//        else {
	//            if (_customerWebSession.GenericMediaDetailLevel.FromControlItem == WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels) {
	//                int index = -1;
	//                foreach (GenericDetailLevel currentLevel in GetDefaultDetailLevels()) {
	//                    if (CanAddDetailLevel(currentLevel, _customerWebSession.CurrentModule)) index++;
	//                    if (currentLevel.EqualLevelItems(_customerGenericDetailLevel)) _defaultDetail.SelectedValue = index.ToString();
	//                }
	//            }               
	//            if (_customerGenericDetailLevel.FromControlItem == WebConstantes.GenericDetailLevel.SelectedFrom.customLevels) {
	//                if (_nbDetailLevelItemList >= 1 && _customerGenericDetailLevel.GetNbLevels >= 1 && _customerGenericDetailLevel.LevelIds[0] != null) {
	//                    _l1Detail.SelectedValue = ((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[0]).GetHashCode().ToString();
	//                }
	//                if (_nbDetailLevelItemList >= 2 && _customerGenericDetailLevel.GetNbLevels >= 2 && _customerGenericDetailLevel.LevelIds[1] != null) {
	//                    _l2Detail.SelectedValue = ((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[1]).GetHashCode().ToString();
	//                }
	//                if (_nbDetailLevelItemList >= 3 && _customerGenericDetailLevel.GetNbLevels >= 3 && _customerGenericDetailLevel.LevelIds[2] != null) {
	//                    _l3Detail.SelectedValue = ((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[2]).GetHashCode().ToString();
	//                }                    
	//            }
	//        }
	//        #endregion
	//        switch (_componentProfile) {
	//            case WebConstantes.GenericDetailLevel.ComponentProfile.media:
	//                _customerWebSession.GenericMediaSelectionDetailLevel = _customerGenericDetailLevel;
	//                break;              
	//        }

	//    }
	//    #endregion

	//    #region Prérender
	//    /// <summary>
	//    /// Préparation du rendu des niveaux de détails personnalisés.
	//    /// </summary>
	//    /// <param name="e">Sender</param>
	//    protected override void OnPreRender(EventArgs e) {
	//        base.OnPreRender(e);
	//        if (!this.Page.ClientScript.IsClientScriptBlockRegistered("genericDetailSelectionControl")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "genericDetailSelectionControl", GetLevelJavaScript());
	//        if (!this.Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DivDisplayer", TNS.AdExpress.Web.Functions.Script.DivDisplayer());
	//    }
	//    #endregion

	//    #region Render
	//    /// <summary> 
	//    /// Génère ce contrôle dans le paramètre de sortie spécifié.
	//    /// </summary>
	//    /// <param name="output"> Le writer HTML vers lequel écrire </param>
	//    protected override void Render(HtmlTextWriter output) {

	//        string themeName = WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name;

	//        output.Write("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" bgcolor=\"" + _backgroundColor + "\">");
	//        output.Write("<tr>");
	//        output.Write("<td class=\"" + _cssDefaultListLabel + "\">" + GestionWeb.GetWebWord(1606, _customerWebSession.SiteLanguage) + "</td>");//1886
	//        output.Write("</tr>");
	//        output.Write("<tr>");
	//        output.Write("<td>");
	//        // Liste par défaut
	//        _defaultDetail.RenderControl(output);
	//        output.Write("</td>");
	//        output.Write("</tr>");
	//        // Espace blanc
	//        output.Write("<tr>");
	//        output.Write("<td><img src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\" border=\"0\" height=\"10\"></td>");
	//        output.Write("</tr>");
	//        // table de personnalisation
	//        output.Write("<tr>");
	//        output.Write("<td>");
	//        output.Write("<table class=\"whiteBackGround violetBorder\" cellSpacing=\"0\" cellPadding=\"0\" width=\"" + this.Width + "\" border=\"0\">");
	//        output.Write("<tr onclick=\"DivDisplayer('detailledLevelContent');\" class=\"cursorHand\">");
	//        //Titre de la section
	//        output.Write("<td class=\"" + _cssCustomSectionTitle + "\">&nbsp;" + GestionWeb.GetWebWord(1896, _customerWebSession.SiteLanguage) + "&nbsp;</td>");
	//        // Image d'ouverture de la section
	//        output.Write("<td align=\"right\" class=\"arrowBackGround\"></td>");
	//        output.Write("</tr>");
	//        output.Write("</table>");
	//        // Section
	//        output.Write("\r\n<div id=\"detailledLevelContent\" class=\"GenericMediaLevelDetailSelectionSection\" style=\"DISPLAY: none; WIDTH: " + this.Width + "px;\">");
	//        output.Write("\r\n<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" ID=\"Section\">");
	//        //output.Write("\r\n<tr>");
	//        //// Niveaux personnalisés déjà enregistrés
	//        //output.Write("\r\n<td class=\"" + _cssListLabel + "\">" + GestionWeb.GetWebWord(1897, _customerWebSession.SiteLanguage) + " :</td>");
	//        //output.Write("\r\n</tr>");
	//        //output.Write("\r\n<tr>");
	//        //output.Write("\r\n<td>");
	//        //_customDetail.RenderControl(output);
	//        //output.Write("\r\n</td>");
	//        //output.Write("\r\n</tr>");
	//        // Bouton supprimer
	//        //output.Write("\r\n<tr>");
	//        //output.Write("\r\n<td align=\"right\"><a class=\"roll03\" href=\"javascript: remove();\"  onmouseover=\"deleteButton.src='/App_Themes/" + themeName + "/Images/Common/button/bt_delete_down.gif';\" onmouseout=\"deleteButton.src ='/App_Themes/" + themeName + "/Images/Common/button/bt_delete_up.gif';\"><img name=deleteButton border=0 src=\"/App_Themes/" + themeName + "/Images/Common/button/bt_delete_up.gif\" alt=\"" + GestionWeb.GetWebWord(1951, _customerWebSession.SiteLanguage) + "\"></a></td>");
	//        //output.Write("\r\n</tr>");
	//        output.Write("\r\n<tr>");
	//        output.Write("\r\n<td>");
	//        // Sélection des niveaux de détail
	//        output.Write("\r\n<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" ID=\"levelSelection\">");
	//        output.Write("\r\n<tr>");
	//        // Construction des niveaux
	//        output.Write("\r\n<td colspan=\"2\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(1899, _customerWebSession.SiteLanguage) + " :</td>");
	//        output.Write("\r\n</tr>");

	//        // Niveaux
	//        int i = 0;
	//        foreach (DropDownList currentList in _DetailLevelItemList) {
	//            i++;
	//            output.Write("\r\n<tr>");
	//            output.Write("\r\n<td class=\"" + _cssListLabel + "\">" + GestionWeb.GetWebWord(1898, _customerWebSession.SiteLanguage) + i.ToString() + ":&nbsp;</td>");
	//            output.Write("\r\n<td>");
	//            currentList.RenderControl(output);
	//            output.Write("\r\n</td>");
	//            output.Write("\r\n</tr>");
	//            // Espace blanc
	//            output.Write("\r\n<tr>");
	//            output.Write("\r\n<td colspan=2><img src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\" border=\"0\" height=\"5\"></td>");
	//            output.Write("\r\n</tr>");
	//        }
	//        // Bouton de sauvegarde
	//        //output.Write("\r\n<tr>");
	//        //output.Write("\r\n<td colspan=2 align=\"right\">");
	//        //if (_saveASPXFilePath != null && _saveASPXFilePath.Length > 1)
	//        //    output.Write("<a class=\"roll03\" href=\"javascript: save();\"  onmouseover=\"saveLevelDetailButton.src='/App_Themes/" + themeName + "/Images/Common/button/save_down.gif';\" onmouseout=\"saveLevelDetailButton.src ='/App_Themes/" + themeName + "/Images/Common/button/save_up.gif';\"><img name=saveLevelDetailButton border=0 src=\"/App_Themes/" + themeName + "/Images/Common/button/save_up.gif\" alt=\"" + GestionWeb.GetWebWord(1952, _customerWebSession.SiteLanguage) + "\"></a>");
	//        //else
	//        //    output.Write("&nbsp;");
	//        //output.Write("</td>");
	//        //// Fin niveau de détaille N1...
	//        //output.Write("\r\n</tr>");
	//        output.Write("\r\n</table>");
	//        output.Write("\r\n</td>");
	//        output.Write("\r\n</tr>");
	//        //Fin table Avant Div
	//        output.Write("\r\n</table>");
	//        output.Write("\r\n</div>");
	//        output.Write("\r\n</td>");
	//        output.Write("\r\n</tr>");
	//        output.Write("\r\n</table>");
	//    }
	//    #endregion

	//    #endregion

	//    #region Méthode privée
	//    /// <summary>
	//    /// Return allowed detail level items
	//    /// </summary>
	//    /// <remarks>
	//    /// AdNetTrack selection [Ok]
	//    /// </remarks>
	//    /// <returns>Detail level list</returns>
	//    private  List<DetailLevelItemInformation> GetAllowedDetailLevelItems() {
	//        return GetVehicleAllowedDetailLevelItems();
	//    }

	//    /// <summary>
	//    /// Return allowed detail level list contained in the module object
	//    /// </summary>
	//    /// <returns>Detail level list</returns>
	//    private ArrayList GetModuleAllowedDetailLevelItems() {

	//        switch (_componentProfile) {
	//            case WebConstantes.GenericDetailLevel.ComponentProfile.media:
	//                return (_currentModule.AllowedMediaDetailLevelItems);                              
	//            default:
	//                return (new ArrayList());
	//        }

	//    }

	//    /// <summary>
	//    /// Return allowed detail level list for vehicle list seleceted
	//    /// </summary>
	//    /// <returns>Detail level list</returns>
	//    private List<DetailLevelItemInformation> GetVehicleAllowedDetailLevelItems() {

	//        List<Int64> vehicleList = new List<Int64>();
	//        string listStr = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
	//        if (listStr != null && listStr.Length > 0) {
	//            string[] list = listStr.Split(',');
	//            for (int i = 0; i < list.Length; i++)
	//                vehicleList.Add(Convert.ToInt64(list[i]));
	//        }
	//        else {
	//            //When a vehicle is not checked but one or more category, this get the vehicle correspondly
	//            string Vehicle = ((LevelInformation)_customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
	//            vehicleList.Add(Convert.ToInt64(Vehicle));
	//        }

	//        return VehiclesInformation.GetSelectionDetailLevelList(vehicleList);
	//    }

	//    /// <summary>
	//    /// Retourne le niveau de détail par defaut
	//    /// </summary>
	//    /// <remarks>
	//    /// AdNetTrack selection [Ok]
	//    /// </remarks>
	//    /// <returns>Niveau de détail</returns>
	//    protected List<GenericDetailLevel> GetDefaultDetailLevels() {

	//        string listStr = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
	//        return VehiclesInformation.Get(long.Parse(listStr)).SelectionDefaultMediaDetailLevels;
	//    }


	//    /// <summary>
	//    /// Initialise une sélection d'éléments de niveau de détaille
	//    /// </summary>
	//    /// <remarks>
	//    /// AdNetTrack selection [Ok]
	//    /// </remarks>
	//    /// <param name="dropDownList">Liste</param>
	//    /// <param name="level">Niveau</param>
	//    protected void DetailLevelItemInit(DropDownList dropDownList, int level) {
	//        string onChange;
	//        dropDownList.Width = new System.Web.UI.WebControls.Unit(this.Width.Value - 23);
	//        dropDownList.ID = "l" + level.ToString() + "Detail_" + this.ID;
	//        dropDownList.AutoPostBack = false;
	//        dropDownList.CssClass = _cssListBox;
	//        dropDownList.Items.Add(new ListItem("-------", "-1"));
	//       List<DetailLevelItemInformation> AllowedDetailLevelItems = GetAllowedDetailLevelItems();
	//        foreach (DetailLevelItemInformation currentDetailLevelItem in AllowedDetailLevelItems) {
	//            if (CanAddDetailLevelItem(currentDetailLevelItem, _customerWebSession.CurrentModule,level)) {
	//                dropDownList.Items.Add(new ListItem(GestionWeb.GetWebWord(currentDetailLevelItem.WebTextId, _customerWebSession.SiteLanguage), currentDetailLevelItem.Id.GetHashCode().ToString()));
	//            }
	//        }
	//        _DetailLevelItemList.Add(dropDownList);

	//        #region OnChange
	//        //onChange = "javascript:setN" + level + "('" + _defaultDetail.ID + "','" + _customDetail.ID + "'";
	//        onChange = "javascript:setN" + level + "('" + _defaultDetail.ID + "',''";
	//        for (int i = 1; i <= _nbDetailLevelItemList; i++) {
	//            onChange += ",'" + "l" + i.ToString() + "Detail_" + this.ID + "'";
	//        }
	//        onChange += ");";
	//        dropDownList.Attributes["onchange"] = onChange;
	//        #endregion

	//        Controls.Add(dropDownList);
	//    }

	//    /// <summary>
	//    /// Test si un niveau de détail peut être montré
	//    /// </summary>
	//    /// <remarks>
	//    /// AdNetTrack selection [Ok] (redéfinie)
	//    /// </remarks>
	//    /// <param name="currentDetailLevel">Niveau de détail</param>
	//    /// <param name="module">Module courrant</param>
	//    /// <returns>True s'il peut être ajouté</returns>
	//    protected virtual bool CanAddDetailLevel(GenericDetailLevel currentDetailLevel, Int64 module) {
	//        foreach (DetailLevelItemInformation currentDetailLevelItem in currentDetailLevel.Levels) {               
	//            if (!CanAddDetailLevelItem(currentDetailLevelItem, module)) return (false);
	//        }
	//        return (true);
	//    }

	//    /// <summary>
	//    /// Test si l'élément de niveau de détail peut être montré
	//    /// </summary>
	//    /// <remarks>
	//    /// AdNetTrack selection [Ko]
	//    /// </remarks>
	//    /// <param name="currentDetailLevelItem">Elément de niveau de détail</param>
	//    /// <param name="module">Module</param>
	//    /// <returns>True si oui false sinon</returns>
	//    private bool CanAddDetailLevelItem(DetailLevelItemInformation currentDetailLevelItem, Int64 module) {
	//        List<DetailLevelItemInformation> AllowedDetailLevelItems = GetAllowedDetailLevelItems();
	//        if (!AllowedDetailLevelItems.Contains(currentDetailLevelItem)) return (false);
	//        return (true);
	//    }        

	//    /// <summary>
	//    /// Test si l'élément de niveau de détail peut être montré
	//    /// </summary>
	//    /// <remarks>
	//    /// AdNetTrack selection [Ko]
	//    /// </remarks>
	//    /// <param name="currentDetailLevelItem">Elément de niveau de détail</param>
	//    /// <param name="module">Module</param>
	//    /// <returns>True si oui false sinon</returns>
	//    private bool CanAddDetailLevelItem(DetailLevelItemInformation currentDetailLevelItem, Int64 module, int level) {

	//        if (_forceMediaSelection && level == 1 && CanAddDetailLevelItem( currentDetailLevelItem,  module) && currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.media) 
	//            return (false);

	//        if (_forceMediaSelection && level == 2 && CanAddDetailLevelItem(currentDetailLevelItem, module) && currentDetailLevelItem.Id != DetailLevelItemInformation.Levels.media) 
	//            return (false);
			
	//        return (true);
	//    }        

	//    #endregion

	}
}
