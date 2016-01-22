#region Informations
// Auteur: Y. Rkaina 
// Date de création: 26/04/2006
// Date de modification: 
#endregion

using System;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Controls.Buttons;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using System.Collections.Generic;
using TNS.AdExpress.Web.Controls.Selections;
using TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using System.Data;


namespace TNS.AdExpress.Web.Controls.Headers
{
	/// <summary>
	/// Description résumée de GenericDetailSelectionWebControl.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:GenericDetailSelectionWebControl runat=server></{0}:GenericDetailSelectionWebControl>")]
	public class GenericDetailSelectionWebControl : System.Web.UI.WebControls.WebControl
	{

		#region Constantes
		/// <summary>
		/// Chemin d'accès à l'icône d'aide
		/// </summary>
		protected const string HELP_BUTTON_PATH="/Images/Common/button/";
		/// <summary>
		/// Nom de base de l'icône
		/// </summary>
		protected const string HELP_BUTTON_NAME="help";
		/// <summary>
		/// Hauteur de la fenêtre cible
		/// </summary>
		protected const string TARGET_PAGE_HEIGHT="700";
		#endregion

		#region Variables

		#region En tête
		/// <summary>
		/// La période sélectionnée
		/// </summary>
		private string _zoomDate;
		#endregion

        #region Width
        /// <summary>
        /// Title Bar Width
        /// </summary>
        private string _titleBarWidth = string.Empty;
        /// <summary>
        /// Title Div Width
        /// </summary>
        private string _titleDivWidth = string.Empty;
        #endregion

        #region Partie Colonnes
        /// <summary>
		/// Nombre de colonnes personnalisées
		/// </summary>
		protected int _nbColumnItemList=0;
		/// <summary>
		/// Liste des colonnes personnalisées
		/// </summary>
		private List<GenericColumnItemInformation> _columnItemList=null;
		/// <summary>
		/// Liste des colonnes sélectionnées
		/// </summary>
        private List<GenericColumnItemInformation> _columnItemSelectedList = null;
		/// <summary>
		/// Liste des colonnes de la corbeille
		/// </summary>
        private List<GenericColumnItemInformation> _columnItemTrashList = null;
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession _customerWebSession = null;
		/// <summary>
		/// Une colonne
		/// </summary>
		GenericColumnItemInformation _genericColumnItemInformation =null;
		/// <summary>
		/// L'id vehicule de l'onglet choisit
		/// </summary>
		private Int64 _idVehicleFromTab;
		/// <summary>
		/// La dernière liste des colonnes sélectionées 
		/// </summary>
		private string _formColumnItemSelectedList="";
        /// <summary>
        /// Id of the current Column Set
        /// </summary>
        protected Int64 _columnSetId = -1;
        /// <summary>
        /// Id of the current Column Set
        /// </summary>
        protected Int64 _oldColumnSetId = -1;
        #endregion

		#region Partie Niveau de détail
		/// <summary>
		/// Nombre de niveaux de détaille personnalisés
		/// </summary>
		protected int _nbDetailLevelItemList=3;
		/// <summary>
		/// Liste des niveaux de détaille personnalisés
		/// </summary>
		private ArrayList _DetailLevelItemList=new ArrayList();
		/// <summary>
		/// Classe Css pour le libellé de la liste des niveaux par défaut
		/// </summary>
		protected string _cssDefaultListLabel="txtGris11Bold";
		/// <summary>
		/// Classe Css pour le libellé des listes personnalisées
		/// </summary>
		protected string _cssListLabel="txtViolet11Bold";
		/// <summary>
		/// Classe Css pour les listbox
		/// </summary>
		protected string _cssListBox="txtNoir11Bold";
		/// <summary>
		/// Classe Css pour le titre de la section personnalisée
		/// </summary>
		protected string _cssCustomSectionTitle="txtViolet11Bold";
		/// <summary>
		/// Liste des éléments de détail par défaut
		/// </summary>
		private List<GenericDetailLevel> _defaultDetailItemList=null;
		/// <summary>
		/// Liste des éléments de détail par personnalisées
		/// </summary>
		private List<DetailLevelItemInformation> _allowedDetailItemList=null;
		#endregion

		#region Help
		/// <summary>
		/// Langue des textes du composant
		/// </summary>
		protected int language=33;
		#endregion

        #region Theme
        /// <summary>
        /// Theme name
        /// </summary>
        private string _themeName = string.Empty;
        #endregion

        #region Save customised levels
        /// <summary>
        /// Check if is loading saved levels
        /// </summary>
        bool _isLoadingSaveLevels = false;
        /// <summary>
        ///Saved Detail Item List 
        /// </summary>
        protected string _savedDetailItemList = "";
        #endregion

        #endregion

        #region Variables MMI
        /// <summary>
		/// Choix du niveau de détail L1
		/// </summary>
		public DropDownList _l1Detail;
		/// <summary>
		/// Choix du niveau de détail L2
		/// </summary>
		public DropDownList _l2Detail;
		/// <summary>
		/// Choix du niveau de détail L3
		/// </summary>
		public DropDownList _l3Detail;
		/// <summary>
		/// Valider la sélection
		/// </summary>
		public ImageButtonRollOverWebControl _buttonOk;
        /// <summary>
        /// Save levels
        /// </summary>
        public ImageButtonRollOverWebControl _buttonSave;
        /// <summary>
        /// Loadable Customised Levels WebControl
        /// </summary>
        protected LoadableCustomisedLevelsWebControl _loadableCustomisedLevelsWebControl = null;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public GenericDetailSelectionWebControl():base(){
			this.EnableViewState = true;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Définit la session du client
		/// </summary>
		[Bindable(false)]
		public WebSession CustomerWebSession{
			//get{return _customerWebSession;}
			set{_customerWebSession=value;}
		}
		/// <summary>
		/// Définit l'id vehicule de l'onglet choisit
		/// </summary>
		[Bindable(false)]
		public Int64 IdVehicleFromTab{
			get{return _idVehicleFromTab;}
			set{_idVehicleFromTab=value;}
		}
		/// <summary>
		/// Définit la période sélectionnée
		/// </summary>
		[Bindable(false)]
		public string ZoomDate{
			get{return _zoomDate;}
			set{_zoomDate=value;}
		}
		/// <summary>
		/// Langue des textes du composant
		/// </summary>
		[Bindable(false)]
		public int Language{
			get{return language;}
			set{language=value;}
		}
        /// <summary>
        /// Get or Set Theme name
        /// </summary>
        public string ThemeName {
            get { return _themeName; }
            set { _themeName = value; }
        }
        /// <summary>
        /// Get / Set Title Bar Width
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("990")]
        public string TitleBarWidth{
            get { return _titleBarWidth; }
            set { _titleBarWidth = value; }
        }
        /// <summary>
        /// Get / Set Title Div Width
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("986")]
        public string TitleDivWidth{
            get { return _titleDivWidth; }
            set { _titleDivWidth = value; }
        }
        /// <summary>
        /// Get or Set le libellé de la liste des niveaux par défaut
        /// </summary>
        public string CssDefaultListLabel {
            get { return _cssDefaultListLabel; }
            set { _cssDefaultListLabel = value; }
        }
        /// <summary>
        /// Get or Set libellé des listes personnalisées  
        /// </summary>
        public string CssListLabel {
            get { return _cssListLabel; }
            set { _cssListLabel = value; }
        }
        /// <summary>
        /// Get or Set les listbox
        /// </summary>
        public string CssListBox {
            get { return _cssListBox; }
            set { _cssListBox = value; }
        }
        /// <summary>
        /// Get or Set le titre de la section personnalisée
        /// </summary>
        public string CssCustomSectionTitle {
            get { return _cssCustomSectionTitle; }
            set { _cssCustomSectionTitle = value; }
        }
		#endregion

		#region Javascript
		/// <summary>
		/// Génére les javascripts utilisés pour le controle des listes + Ouverture du popup pour la page d'aide
		/// </summary>
		/// <returns>Code Javascript</returns>
		private string GetLevelJavaScript(){
			StringBuilder script=new StringBuilder(2000);

			script.Append("<script language=\"JavaScript\">");

			script.Append("\r\nfunction popupRecallOpen(page,width,height){");
			script.Append("\r\n\twindow.open(page,'','width='+width+',height='+height+',toolbar=no,scrollbars=yes,resizable=no');");
			script.Append("\r\n}");

			script.Append("\r\nfunction setLevel(detail");
			for(int i=1;i<=_nbDetailLevelItemList;i++)script.Append(",l"+i.ToString()+"Detail");
			script.Append("){");
			script.Append("\r\n\t intSelect(detail);");
			for(int i=1;i<=_nbDetailLevelItemList;i++)script.Append("\r\n\t intSelect(l"+i.ToString()+"Detail);");
			script.Append("\r\n}");
			
			
			script.Append("\r\n function intSelect(id){");
			script.Append("\r\n\t var oContent = document.all.item(id);");
			script.Append("\r\n\t oContent.value=-1;");
			script.Append("\r\n}");
			
			script.Append("\r\n function setN(detail,customDetail){");
			script.Append("\r\n\t intSelect(detail);");
			script.Append("\r\n\t intSelect(customDetail);");
			script.Append("\r\n}");
			
			for(int i=1;i<=_nbDetailLevelItemList;i++){
				script.Append("\r\nfunction setN"+i.ToString()+"(");
				for(int j=1;j<=_nbDetailLevelItemList;j++)
					if(j==1)
						script.Append("N"+j.ToString()); 
					else
						script.Append(",N"+j.ToString()); 
				script.Append("){");
				for(int k=1;k<=_nbDetailLevelItemList;k++)script.Append("\r\n\t var oN"+k.ToString()+"=document.all.item(N"+k.ToString()+");");
				//script.Append("\r\n\t setN(detail,customDetail);");
				if(i==1){
					if(_nbDetailLevelItemList>=2)script.Append("\r\n\t if(oN1.value==oN2.value)oN2.value=-1;");
					if(_nbDetailLevelItemList>=3)script.Append("\r\n\t if(oN1.value==oN3.value)oN3.value=-1;");
					if(_nbDetailLevelItemList>=4)script.Append("\r\n\t if(oN1.value==oN4.value)oN4.value=-1;");
				}
				if(i==2){
					script.Append("\r\n\t if(oN2.value==oN1.value)oN1.value=-1;");
					if(_nbDetailLevelItemList>=3)script.Append("\r\n\t if(oN2.value==oN3.value)oN3.value=-1;");
					if(_nbDetailLevelItemList>=4)script.Append("\r\n\t if(oN2.value==oN4.value)oN4.value=-1;");
				}
				if(i==3){
					script.Append("\r\n\t if(oN3.value==oN1.value)oN1.value=-1;");
					script.Append("\r\n\t if(oN3.value==oN2.value)oN2.value=-1;");
					if(_nbDetailLevelItemList>=4)script.Append("\r\n\t if(oN3.value==oN4.value)oN4.value=-1;");
				}
				if(i==4){
					script.Append("\r\n\t if(oN4.value==oN1.value)oN1.value=-1;");
					script.Append("\r\n\t if(oN4.value==oN2.value)oN2.value=-1;");
					script.Append("\r\n\t if(oN4.value==oN3.value)oN3.value=-1;");
				}
				script.Append("\r\n}");
			}				
			script.Append("\r\n</script>");

			return(script.ToString());
		}

        /// <summary>
        /// Script permettant d'ouvrir la fenetre modale d'enregistrement des niveaux d'insertion
        /// Elle est ouverte par la fenetre appelante mais se ferme independemment de cette derniere
        /// Le script est destinée à être placé dans un lien ou un onClick, onLoad...
        /// </summary>
        /// <returns>Code JavaScript</returns>
        private string SaveInsertionsLevels()
        {
            StringBuilder script = new StringBuilder(300);
            script.Append("\n <SCRIPT language=JavaScript>");
            script.Append("\n\t function SaveInsertionsLevels(idSession,idVehicle){\n");
            script.Append("\n\t var detailLevels='';");
            script.Append("\n\t var columsLevelsIds=document.getElementById('columnItemSelectedList').value;");
            for (int i = 1; i <= _nbDetailLevelItemList; i++)
            {
                string detailLevelId = "'" + "l" + i.ToString() + "Detail_" + this.ID + "'";
                script.Append("\n\t if(document.getElementById( " + detailLevelId + ") != null && document.getElementById( " + detailLevelId + ").options[document.getElementById( " + detailLevelId + ").selectedIndex].value != -1){");
                script.Append("\n\t\t detailLevels= detailLevels + document.getElementById( " + detailLevelId + ").options[document.getElementById( " + detailLevelId + ").selectedIndex].value +','; ");
                script.Append("\n\t } ");
               
            }
            script.Append("\n\t if(detailLevels.length>0) detailLevels = detailLevels.substring(0,detailLevels.length-1); ");
            script.Append("\n\t\t window.showModalDialog('/Private/MyAdExpress/InsertionLevelSave.aspx?idSession="+_customerWebSession.IdSession+"&idV=' + idVehicle+'&cl='+ columsLevelsIds+'&dl='+ detailLevels+'&atd=" + DateTime.Now.ToString("yyyyMMddhhmmss") + " ',null, 'dialogHeight:300px;dialogWidth:450px;help:no;resizable:no;scroll:no;status:no;');");
            script.Append("\n\t }\n");
            script.Append("</script>\n");
            return script.ToString();
        }
		#endregion

		#region Evènements
		
		#region Init
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguements</param>
		protected override void OnInit(EventArgs e){
			base.OnInit (e);

			#region initialisation de la liste des colonnes

			string[] columnItemSelectedList=null;
			List<Int64>  genericColumnList=null; 
			ArrayList levels=new ArrayList();
            string columnSetContainer = string.Format("columnSetId_{0}", this.ID);
            if (this.Page.Request.Form.GetValues(columnSetContainer) != null && this.Page.Request.Form.GetValues(columnSetContainer)[0].Length > 0)
            {
                _oldColumnSetId = Convert.ToInt64(this.Page.Request.Form.GetValues(columnSetContainer)[0]);
            }
 
			//if ( (!_idVehicleFromTab.Equals(null)) && (_idVehicleFromTab>0)){
            if ((!_idVehicleFromTab.Equals(null)) && (_idVehicleFromTab > -1)) {
                
                _columnSetId = WebApplicationParameters.InsertionsDetail.GetDetailColumnsId(_idVehicleFromTab, _customerWebSession.CurrentModule);

				#region Initialisation de la liste des colonnes pour un type de Vehicle
				List<GenericColumnItemInformation> tmp = WebApplicationParameters.InsertionsDetail.GetDetailColumns(_idVehicleFromTab,_customerWebSession.CurrentModule);
                _columnItemList = new List<GenericColumnItemInformation>();
                foreach(GenericColumnItemInformation column in tmp){
                    if (WebApplicationParameters.GenericColumnsInformation.IsVisible(_columnSetId, column.Id))
                    {
                        _columnItemList.Add(column);
                    }
                }

				_columnItemList=ColumnRight(_columnItemList);
				_nbColumnItemList=_columnItemList.Count;
				#endregion

				#region Initialisation de la liste des niveaux de détail
				_allowedDetailItemList = WebApplicationParameters.InsertionsDetail.GetAllowedMediaDetailLevelItems(_idVehicleFromTab);
                _defaultDetailItemList = WebApplicationParameters.InsertionsDetail.GetDefaultMediaDetailLevels(_idVehicleFromTab);
                #endregion  

                #region Load saved customised levels
                List<int> allowedDetailLevelItems = (WebApplicationParameters.InsertionOptions.CanSaveLevels) ? GetAllowedDetailItemList():null;
                string idMySavedLevels = "";
                if (WebApplicationParameters.InsertionOptions.CanSaveLevels && Page.IsPostBack && Page.Request.Form.Get("__EVENTTARGET") == "loadImageButtonRollOverWebControl")
                {
                    //Load saved customised levels                    
                    if (this.Page.Request.Form.GetValues("idMySavedLevels") != null)
                    {
                        idMySavedLevels = this.Page.Request.Form.GetValues("idMySavedLevels")[0];
                        if (!string.IsNullOrEmpty(idMySavedLevels))
                        {
                            _columnItemSelectedList = new List<GenericColumnItemInformation>();
                             _columnItemTrashList = new List<GenericColumnItemInformation>();

                            DataSet ds = InsertionLevelDAL.GetInsertionSaveData(_customerWebSession, idMySavedLevels);
                            if (ds != null &&  ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0)
                            {                             
                                columnItemSelectedList = ds.Tables[0].Rows[0]["COLUMN_LIST"].ToString().Split(',');
                                _savedDetailItemList = ds.Tables[0].Rows[0]["LEVE_LIST"].ToString();
                                if (columnItemSelectedList != null && columnItemSelectedList.Length > 0 && !columnItemSelectedList[0].Equals(""))
                                {
                                    for (int i = 0; i < columnItemSelectedList.Length; i++)
                                    {
                                        if(WebApplicationParameters.GenericColumnItemsInformation.Contains(Int64.Parse(columnItemSelectedList[i]))){
                                            _genericColumnItemInformation = WebApplicationParameters.GenericColumnItemsInformation.Get(Int64.Parse(columnItemSelectedList[i]));
                                        _columnItemSelectedList.Add(_genericColumnItemInformation);
                                        }
                                    }
                                    if (_columnItemSelectedList != null && _columnItemSelectedList.Count > 0)
                                    {
                                        #region La liste des colonnes de la corbeille
                                        int verif = 0;
                                        
                                        foreach (GenericColumnItemInformation ColumnI in _columnItemList)
                                        {
                                            foreach (GenericColumnItemInformation ColumnJ in _columnItemSelectedList)
                                            {
                                                if (ColumnI.Id == ColumnJ.Id)
                                                    verif = 1;
                                            }
                                            if (verif == 0)
                                            {
                                                _columnItemTrashList.Add(ColumnI);
                                            }
                                            verif = 0;
                                        }
                                        #endregion

                                        genericColumnList = new List<Int64>();
                                        foreach (GenericColumnItemInformation Column in _columnItemSelectedList)
                                            genericColumnList.Add((int)Column.Id);

                                    }
                                }
                                if (_columnItemSelectedList!=null && _columnItemSelectedList.Count > 0 && !string.IsNullOrEmpty(_savedDetailItemList))
                                {
                                    List<int> detailLevels = new List<string>(_savedDetailItemList.Split(',')).ConvertAll(i => int.Parse(i));
                                    for (int j = 0; j < detailLevels.Count; j++)
                                    {
                                        if (allowedDetailLevelItems.Contains(detailLevels[j]))
                                        levels.Add(detailLevels[j]);
                                    }
                                }
                                if (_columnItemSelectedList != null && _columnItemSelectedList.Count > 0 && levels!=null && levels.Count > 0)
                                {
                                    _isLoadingSaveLevels = true;
                                    _customerWebSession.GenericInsertionColumns = new GenericColumns(genericColumnList);
                                    _customerWebSession.DetailLevel = new GenericDetailLevel(levels, WebConstantes.GenericDetailLevel.SelectedFrom.savedLevels);
                                }
                            }
                        }
                    }
                }
                #endregion

                if (!_isLoadingSaveLevels)
                {
                    if (Page.IsPostBack && _columnSetId == _oldColumnSetId)
                    {

                        #region La liste des colonnes sélectionnées
                        //récupération de la liste des id des colonnes sélectionnées
                        if (this.Page.Request.Form.GetValues("columnItemSelectedList") != null)
                        {
                            columnItemSelectedList = this.Page.Request.Form.GetValues("columnItemSelectedList");

                            _columnItemSelectedList = new List<GenericColumnItemInformation>();
                            _columnItemTrashList = new List<GenericColumnItemInformation>();

                            columnItemSelectedList = columnItemSelectedList[0].Split(',');

                            if (!columnItemSelectedList[0].Equals(""))
                            {
                                if (columnItemSelectedList[0].Equals("-1"))
                                    _columnItemTrashList = _columnItemList;
                                else
                                {
                                    for (int i = 0; i < columnItemSelectedList.Length; i++)
                                    {
                                        _genericColumnItemInformation = WebApplicationParameters.GenericColumnItemsInformation.Get(Int64.Parse(columnItemSelectedList[i]));
                                        _columnItemSelectedList.Add(_genericColumnItemInformation);
                                    }

                                    #region La liste des colonnes de la corbeille
                                    int verif = 0;

                                    foreach (GenericColumnItemInformation ColumnI in _columnItemList)
                                    {
                                        foreach (GenericColumnItemInformation ColumnJ in _columnItemSelectedList)
                                        {
                                            if (ColumnI.Id == ColumnJ.Id)
                                                verif = 1;
                                        }
                                        if (verif == 0)
                                            _columnItemTrashList.Add(ColumnI);
                                        verif = 0;
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                _columnItemSelectedList = _columnItemList;
                            }

                            genericColumnList = new List<Int64>();
                            foreach (GenericColumnItemInformation Column in _columnItemSelectedList)
                                genericColumnList.Add((int)Column.Id);
                            _customerWebSession.GenericInsertionColumns = new GenericColumns(genericColumnList);
                        }
                        #endregion

                    }
                    else
                    {
                        genericColumnList = new List<Int64>();
                        _columnItemSelectedList = new List<GenericColumnItemInformation>();
                        _columnItemTrashList = new List<GenericColumnItemInformation>();
                        foreach (GenericColumnItemInformation Column in _columnItemList)
                        {
                            if (WebApplicationParameters.GenericColumnsInformation.InBasket(_columnSetId, Column.Id))
                                _columnItemTrashList.Add(Column);
                            else
                            {
                                genericColumnList.Add((int)Column.Id);
                                _columnItemSelectedList.Add(Column);
                            }
                        }
                        _customerWebSession.GenericInsertionColumns = new GenericColumns(genericColumnList);

                        foreach (GenericDetailLevel detailItem in _defaultDetailItemList)
                            levels = detailItem.LevelIds;

                        _customerWebSession.DetailLevel = new GenericDetailLevel(levels, WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels);
                        _customerWebSession.Save();
                    }
                }
				#endregion

				#region Niveau de détaille par défaut
				#region L1
				if(_nbDetailLevelItemList>=1){
					_l1Detail= new DropDownList();
					DetailLevelItemInit(_l1Detail,1);
				}
				#endregion

				#region L2
				if(_nbDetailLevelItemList>=2){
					_l2Detail= new DropDownList();
					DetailLevelItemInit(_l2Detail,2);
				}
				#endregion

				#region L3
				if(_nbDetailLevelItemList>=3){
					_l3Detail= new DropDownList();
					DetailLevelItemInit(_l3Detail,3);
				}
				#endregion
				#endregion

                #region bouton Save
                if (WebApplicationParameters.InsertionOptions.CanSaveLevels)
                {
                    _buttonSave = new ImageButtonRollOverWebControl();
                    _buttonSave.ID = "saveImageButton";
                    _buttonSave.ImageUrl = "/App_Themes/" + _themeName + "/Images/Common/Button/bt_save_up.gif";
                    _buttonSave.RollOverImageUrl = "/App_Themes/" + _themeName + "/Images/Common/Button/bt_save_down.gif";
                    _buttonSave.Attributes.Add("onclick","SaveInsertionsLevels(" + _customerWebSession.IdSession + "," + _idVehicleFromTab + ");return false;");                                      
                    Controls.Add(_buttonSave);
                }
                #endregion

				#region bouton OK
				_buttonOk=new ImageButtonRollOverWebControl();
				_buttonOk.ID="okImageButton";
				_buttonOk.ImageUrl="/App_Themes/"+_themeName+"/Images/Common/Button/ok_up.gif";
				_buttonOk.RollOverImageUrl="/App_Themes/"+_themeName+"/Images/Common/Button/ok_down.gif";
				Controls.Add(_buttonOk);
				#endregion

           
				if(!this.Page.ClientScript.IsClientScriptBlockRegistered("genericDetailSelectionControl"))this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"genericDetailSelectionControl",GetLevelJavaScript());
                if (WebApplicationParameters.InsertionOptions.CanSaveLevels)
                {                   
                    _loadableCustomisedLevelsWebControl = new LoadableCustomisedLevelsWebControl();
                    _loadableCustomisedLevelsWebControl.AllowedDetailItemList = allowedDetailLevelItems;
                    List<int> colList = new List<int>();
                    foreach (GenericColumnItemInformation Column in _columnItemList)
                        colList.Add((int)Column.Id);
                    _loadableCustomisedLevelsWebControl.ColumnItemList = colList;
                    _loadableCustomisedLevelsWebControl.CustomerWebSession = _customerWebSession;
                    _loadableCustomisedLevelsWebControl.IdVehicle = _idVehicleFromTab;
                    if (!string.IsNullOrEmpty(idMySavedLevels) && _isLoadingSaveLevels) _loadableCustomisedLevelsWebControl.IdInsertionSave = Convert.ToInt64(idMySavedLevels);
                    _loadableCustomisedLevelsWebControl.ThemeName = _themeName;
                    _loadableCustomisedLevelsWebControl._buttonOk.ID = "loadImageButtonRollOverWebControl";
                    Controls.Add(_loadableCustomisedLevelsWebControl);
                    if (!this.Page.ClientScript.IsClientScriptBlockRegistered("SaveInsertionsLevels")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SaveInsertionsLevels", SaveInsertionsLevels());
                    
                }
			}
		}
		#endregion

		#region Load
		/// <summary>
		/// Chargement du composant
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnLoad(EventArgs e){
			base.OnLoad (e);
			
			if ((!_idVehicleFromTab.Equals(null))&&(_idVehicleFromTab!=0)){

				#region gestion de la sélection

				ArrayList levels=new ArrayList();

				if (Page.IsPostBack){
					#region Gestion de la sélection niveau détail
                    int levelId = -1;

                    if (_isLoadingSaveLevels)
                    {
                        if (_customerWebSession.DetailLevel.LevelIds.Count >= 1)
                        {
                            _l1Detail.SelectedValue = ((DetailLevelItemInformation.Levels)_customerWebSession.DetailLevel.LevelIds[0]).GetHashCode().ToString();
                        }
                        else _l1Detail.SelectedValue = "-1";
                        if (_customerWebSession.DetailLevel.LevelIds.Count >= 2)
                        {
                            _l2Detail.SelectedValue = ((DetailLevelItemInformation.Levels)_customerWebSession.DetailLevel.LevelIds[1]).GetHashCode().ToString();
                        }
                        else _l2Detail.SelectedValue = "-1";
                        if (_customerWebSession.DetailLevel.LevelIds.Count >= 3)
                        {
                            _l3Detail.SelectedValue = ((DetailLevelItemInformation.Levels)_customerWebSession.DetailLevel.LevelIds[2]).GetHashCode().ToString();
                        }
                        else _l3Detail.SelectedValue = "-1";
                    }
					if(_nbDetailLevelItemList>=1 && (levelId = int.Parse(_l1Detail.SelectedValue))>=0 && CanAddDetailLevelItem(DetailLevelItemsInformation.Get(levelId))){
                        levels.Add(levelId);
					}
                    if (_nbDetailLevelItemList >= 2 && (levelId = int.Parse(_l2Detail.SelectedValue)) >= 0 && CanAddDetailLevelItem(DetailLevelItemsInformation.Get(levelId)))
                    {
						levels.Add(levelId);
					}
                    if (_nbDetailLevelItemList >= 3 && (levelId = int.Parse(_l3Detail.SelectedValue)) >= 0 && CanAddDetailLevelItem(DetailLevelItemsInformation.Get(levelId)))
                    {
                        levels.Add(levelId);
					}
					if(levels.Count>0){
						_customerWebSession.DetailLevel=new GenericDetailLevel(levels,WebConstantes.GenericDetailLevel.SelectedFrom.customLevels);
						_customerWebSession.Save();
					}
					else{
						if(_customerWebSession.DetailLevel.LevelIds.Count>=1)
							_l1Detail.SelectedValue=((DetailLevelItemInformation.Levels)_customerWebSession.DetailLevel.LevelIds[0]).GetHashCode().ToString();
						if(_customerWebSession.DetailLevel.LevelIds.Count>=2)
							_l2Detail.SelectedValue=((DetailLevelItemInformation.Levels)_customerWebSession.DetailLevel.LevelIds[1]).GetHashCode().ToString();
						if(_customerWebSession.DetailLevel.LevelIds.Count>=3)
							_l3Detail.SelectedValue=((DetailLevelItemInformation.Levels)_customerWebSession.DetailLevel.LevelIds[2]).GetHashCode().ToString();
					}
					#endregion
				}

				#endregion

				if(!this.Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer"))this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DivDisplayer",TNS.AdExpress.Web.Functions.Script.DivDisplayer());	 
			}
		}
		#endregion

		#region Prérender
		/// <summary>
		/// Préparation du rendu des niveaux de détails personnalisés.
		/// </summary>
		/// <param name="e">Sender</param>
		protected override void OnPreRender(EventArgs e){	

			if ((!_idVehicleFromTab.Equals(null))&&(_idVehicleFromTab!=0)){
				//La generation du script qui permet la gestion de drag and drop
				if(!this.Page.ClientScript.IsClientScriptBlockRegistered("GenericDetailSelectionScript"))
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "GenericDetailSelectionScript", TNS.AdExpress.Web.Functions.DetailSelectionScript.GenericDetailSelectionScript(_nbColumnItemList, ((_columnItemTrashList==null) ? 0 : _nbColumnItemList - _columnItemTrashList.Count)));
			}
			base.OnPreRender (e);
		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output)
        {

           

			#region Début du tableau (support et dates)
            output.Write("<table width=\"500\" class=\"backGroundWhite\" style=\"MARGIN-LEFT: 5px; MARGIN-RIGHT: 0px;BORDER:SOLID 0px;\"");
			output.Write("cellPadding=\"0\" cellSpacing=\"0\" align=\"left\" border=\"0\">");
			#endregion

			if ((!_idVehicleFromTab.Equals(null))&&(_idVehicleFromTab!=0))
			{

               
                #region Saved levels
                if (WebApplicationParameters.InsertionOptions.CanSaveLevels)
                {
                    output.Write("<tr>");
                    output.Write("<td>");
                    _loadableCustomisedLevelsWebControl.RenderControl(output);
                    output.Write("<br/></td>");
                    output.Write("</tr>");
                }
                #endregion

				output.Write("<tr>");
				output.Write("<td>");
                #region Title Bar
                    output.Write("<table class=\"backGroundWhite violetBorder\" style=\"MARGIN-LEFT: 0px;\" cellSpacing=\"0\" cellPadding=\"0\" width=\""+_titleBarWidth+"px\" border=\"0\">");
                        output.Write("<tr onclick=\"DivDisplayer('detailledLevelContent');\" style=\"CURSOR: pointer\">");
                            output.Write("<td class=\"txtViolet11Bold\">&nbsp;" + GestionWeb.GetWebWord(1896, _customerWebSession.SiteLanguage) + "&nbsp;</td>");
                            output.Write("<td align=\"right\" class=\"arrowBackGround\"></td>");
                        output.Write("</tr>");
                    output.Write("</table>");
                #endregion
                    output.Write("\n<div id=\"detailledLevelContent\" style=\"MARGIN-LEFT: 0px; DISPLAY: none; WIDTH: " + _titleDivWidth + "px; padding-left:2px; vertical-align:top;\" class=\"detailledLevelCss\">");
                    output.Write("<table width=\"" + _titleDivWidth + "px\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" ID=\"Section\">");
				            output.Write("<tr>");
				                output.Write("<td>");
                                    #region Div columns
				                    output.Write("<table id=\"Table2\" height=\"32\" width=\""+_titleDivWidth+"\" border=\"0\">");

				                        //output.Write("<tr style=\"DISPLAY: block; FLOAT: left; LIST-STYLE-TYPE: none\">");
                                    output.Write("<tr>");
				                        foreach(GenericColumnItemInformation Column in _columnItemSelectedList){
                                            output.Write("\n<td id=\"zone" + (int)Column.Id + "\" width=\"45px\" height=\"27\" class=\"simpleDroptd\" noWrap align=\"left\">");
						                        output.Write("\n<div class=\"simpleColumn\" id=\""+(int)Column.Id+"\">");
					                                output.Write("\n"+GestionWeb.GetWebWord(Column.WebTextId,_customerWebSession.SiteLanguage)+"");
					                            output.Write("\n</div>");
					                        output.Write("\n</td>");
				                        }
                                        if (_columnItemTrashList != null)
                                        {
                                            foreach (GenericColumnItemInformation Column in _columnItemTrashList)
                                            {
                                                output.Write("\n<td id=\"zone" + (int)Column.Id + "\" width=\"45px\" height=\"27\" class=\"simpleDroptd\" noWrap align=\"left\">");
                                                output.Write("\n</td>");
                                            }
                                        }
                                        output.Write("</tr>");
				                        output.Write("</table>");
                                        #endregion

                    #region details Level + corbeille

                    
                    output.Write("<p>");

				    output.Write("<TABLE id=\"Table3\" style=\"MARGIN-BOTTOM: 8px\" cellSpacing=\"0\" cellPadding=\"0\" height=\"114\">");
				    output.Write("<TR>");
				    output.Write("<TD>");
				    output.Write("</TD>");
					output.Write("<TD align=left colSpan=2><A class=roll03 onmouseover=\"saveButton1.src='/App_Themes/" + _themeName + "/Images/Common/button/restore_down.gif';\" onmouseout=\"saveButton1.src ='/App_Themes/" + _themeName + "/Images/Common/button/restore_up.gif';\" href=\"javascript:initialiser();\"><IMG alt=\"" + GestionWeb.GetWebWord(2576, _customerWebSession.SiteLanguage) + "\" src=\"/App_Themes/" + _themeName + "/Images/Common/button/restore_up.gif\" border=0 name=saveButton1></A><IMG src=\"/App_Themes/" + _themeName + "/Images/Common/pixel.gif\" width=\"165\" height=\"1\">");
				    output.Write("<A class=roll03 onmouseover=\"saveButton2.src='/App_Themes/"+_themeName+"/Images/Common/button/delete_down.gif';\" onmouseout=\"saveButton2.src ='/App_Themes/"+_themeName+"/Images/Common/button/delete_up.gif';\" href=\"javascript:vider();\"><IMG alt=\""+GestionWeb.GetWebWord(2575, _customerWebSession.SiteLanguage)+"\" src=\"/App_Themes/"+_themeName+"/Images/Common/button/delete_up.gif\" border=0 name=saveButton2></A></TD>");
				    output.Write("<TD colSpan=2 align=right><A class=roll03 onmouseover=\"saveButton2.src='/App_Themes/"+_themeName+"/Images/Common/button/delete_down.gif';\" onmouseout=\"saveButton2.src ='/App_Themes/"+_themeName+"/Images/Common/button/delete_up.gif';\" href=\"javascript:vider();\"></A>&nbsp;</TD>");
				    output.Write("</TR>");
    			    
				    output.Write("<TR>");

                    #region Niveau de détail
                    output.Write("<TD  valign=top width=\"" + _titleDivWidth + "\">");
                    output.Write("<TABLE id=\"Section\" cellSpacing=\"2\" cellPadding=\"2\" border=\"0\" class=\"dimgrayBorder\">");
				    output.Write("<TR>");
				    output.Write("<TD>");
				    output.Write("<TABLE id=\"levelSelection\" cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
				    output.Write("<TR>");
				    output.Write("<TD class=\"txtViolet11Bold\" colSpan=\"2\">");
				    output.Write("<P class=\"MsoNormal\">" + GestionWeb.GetWebWord(1899, _customerWebSession.SiteLanguage) + " :" + "</P>");
				    output.Write("</TD>");
				    output.Write("</TR>");
    		
				    // Niveaux
				    int i=0;
				    foreach(DropDownList currentList in _DetailLevelItemList){
					    i++;
					    output.Write("\r\n<tr>");
					    output.Write("\r\n<td class=\""+_cssListLabel+"\">"+ GestionWeb.GetWebWord(1898,_customerWebSession.SiteLanguage) +i.ToString()+":&nbsp;</td>");
					    output.Write("\r\n<td>");
					    currentList.RenderControl(output);
					    output.Write("\r\n</td>");
					    output.Write("\r\n</tr>");
					    // Espace blanc
					    output.Write("\r\n<tr>");
                        output.Write("\r\n<td colspan=2><img src=\"/App_Themes/"+_themeName+"/Images/Common/pixel.gif\" border=\"0\" height=\"5\"></td>");
					    output.Write("\r\n</tr>");
				    }

				    // Fin niveau de détaille N1...
				    output.Write("\r\n</table>");
				    output.Write("\r\n</td>");
				    output.Write("\r\n</tr>");
				    

				    output.Write("</TABLE>");
				    output.Write("</TD>");
                    #endregion

                    #region Corbeille
				    output.Write("<TD align=right valign=top>");
				    output.Write("<table id=Table4 align=left>");            
				    output.Write("<tr>");		
				    
				    output.Write("<TD>");
                    output.Write("<DIV class=\"simpleDropPanel\" id=\"droponme\">");
					output.Write("<DIV class=\"title\">" + GestionWeb.GetWebWord(1950, _customerWebSession.SiteLanguage) + "</DIV>");
				
				    foreach(GenericColumnItemInformation Column in _columnItemTrashList){
					    output.Write("\n<DIV id=\""+(int)Column.Id+"\">");
					    output.Write("\n<A>"+GestionWeb.GetWebWord(Column.WebTextId,_customerWebSession.SiteLanguage)+"</A>");
					    output.Write("\n</DIV>");
				    }
				
				    output.Write("</DIV>");
				    output.Write("</TD>");

				    
                    output.Write("</tr>");

                    output.Write("</table>");
                output.Write("</td>");
                #endregion		

                output.Write("</tr>");
                output.Write("</table>");
                output.Write("</p>");
                #endregion

                #region initialisation du bouton
                output.Write("<div style=\"MARGIN-LEFT: 0px;\"><table width=\"100%\"><tr><td align=\"right\">");
                if (WebApplicationParameters.InsertionOptions.CanSaveLevels)
                {                   
                    _buttonSave.RenderControl(output);
                    output.Write("&nbsp;");
                }              
                _buttonOk.RenderControl(output);
                output.Write("</td></tr></table></div>");
                #endregion

                

				output.Write("</td></tr>");
				output.Write("</table>");
                output.Write("</div>");
				output.Write("</TD>");
				output.Write("</TR>");

                if (VehiclesInformation.DatabaseIdToEnum(_idVehicleFromTab) == DBClassificationConstantes.Vehicles.names.radio                   
                    ) {
                    output.Write("<tr><td align=\"right\">");
                    output.Write("<FONT face=Arial size=1 class=\"txtViolet\">" + GestionWeb.GetWebWord(1949, _customerWebSession.SiteLanguage) + "</FONT>");
                    output.Write("</td></tr>");
                }

                #region hidden fields
                output.Write("<tr><td>");
                output.Write("<INPUT id=\"columnSetId_{0}\" type=\"hidden\" name=\"columnSetId_{0}\" value=\"{1}\">", this.ID, _columnSetId);
                output.Write("<INPUT id=\"columnItemSelectedList\" type=\"hidden\" name=\"columnItemSelectedList\" value=\""+ColumnIdListInit()+"\">");
                output.Write("</td></tr>");
                #endregion

                output.Write("</table>");

                output.Write(WebFunctions.DetailSelectionScript.DragAndDropPostBackScript(_columnItemSelectedList, _columnItemTrashList, _nbColumnItemList));

			}
		}
		#endregion

		#endregion

		#region Méthode privée
		/// <summary>
		/// Initialise une sélection d'éléments de niveau de détaille
		/// </summary>
		/// <param name="dropDownList">Liste</param>
		/// <param name="level">Niveau</param>
		private void DetailLevelItemInit(DropDownList dropDownList,int level){
			string onChange;
			dropDownList.Width = new System.Web.UI.WebControls.Unit(180.00-23);
			dropDownList.ID="l"+level.ToString()+"Detail_"+this.ID;
			dropDownList.AutoPostBack=false;
			dropDownList.CssClass=_cssListBox;
			dropDownList.Items.Add(new ListItem("-------","-1"));

			if (((_customerWebSession.CustomerLogin.CustormerFlagAccess(Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)) 
                && (VehiclesInformation.DatabaseIdToEnum(_idVehicleFromTab) == DBClassificationConstantes.Vehicles.names.outdoor))
                || ((_customerWebSession.CustomerLogin.CustormerFlagAccess(Flags.ID_DETAIL_INSTORE_ACCESS_FLAG)) 
                && (VehiclesInformation.DatabaseIdToEnum(_idVehicleFromTab) == DBClassificationConstantes.Vehicles.names.instore))
                || ((_customerWebSession.CustomerLogin.CustormerFlagAccess(Flags.ID_DETAIL_INDOOR_ACCESS_FLAG))
                && (VehiclesInformation.DatabaseIdToEnum(_idVehicleFromTab) == DBClassificationConstantes.Vehicles.names.indoor))
                || (VehiclesInformation.DatabaseIdToEnum(_idVehicleFromTab) != DBClassificationConstantes.Vehicles.names.outdoor 
                    && VehiclesInformation.DatabaseIdToEnum(_idVehicleFromTab) != DBClassificationConstantes.Vehicles.names.instore
                    && VehiclesInformation.DatabaseIdToEnum(_idVehicleFromTab) != DBClassificationConstantes.Vehicles.names.indoor )
                ) {
				
				foreach(DetailLevelItemInformation currentDetailLevelItem in _allowedDetailItemList){
					if(CanAddDetailLevelItem(currentDetailLevelItem)){
						dropDownList.Items.Add(new ListItem(GestionWeb.
                            GetWebWord(currentDetailLevelItem.WebTextId,_customerWebSession.SiteLanguage),currentDetailLevelItem.Id.GetHashCode().ToString()));
					}
				}
			
				if(level==1)
					dropDownList.SelectedValue=((DetailLevelItemInformation.Levels)_customerWebSession.DetailLevel.LevelIds[0]).GetHashCode().ToString();
				if((level==2)&&(_customerWebSession.DetailLevel.LevelIds.Count>1))
					dropDownList.SelectedValue=((DetailLevelItemInformation.Levels)_customerWebSession.DetailLevel.LevelIds[1]).GetHashCode().ToString();
				if((level==3)&&(_customerWebSession.DetailLevel.LevelIds.Count>2))
					dropDownList.SelectedValue=((DetailLevelItemInformation.Levels)_customerWebSession.DetailLevel.LevelIds[2]).GetHashCode().ToString();
			}

			_DetailLevelItemList.Add(dropDownList);

			#region OnChange
			onChange="javascript:setN"+level+"('";
			for(int i=1;i<=_nbDetailLevelItemList;i++)
			{
				if (i==1)
					onChange+="l"+i.ToString()+"Detail_"+this.ID+"'";
				else
					onChange+=",'"+"l"+i.ToString()+"Detail_"+this.ID+"'";
			}
			onChange+=");";
			dropDownList.Attributes["onchange"]=onChange;
			
			#endregion

			Controls.Add(dropDownList);
		}
        /// <summary>
        /// Get Allowed Detail Item List
        /// </summary>
        /// <returns>ID of Allowed Detail Item List</returns>
        protected List<int> GetAllowedDetailItemList()
        {
            List<int> list = new List<int>();
            foreach (DetailLevelItemInformation currentDetailLevelItem in _allowedDetailItemList)
            {
                if (CanAddDetailLevelItem(currentDetailLevelItem))
                {
                    list.Add(currentDetailLevelItem.Id.GetHashCode());
                }
            }
            return list;
        }
		///<summary>
		/// Test si un niveau de détail peut être montré
		/// </summary>
		///  <param name="currentDetailLevel">
		///  </param>
		///  <returns>
		///  </returns>
		///  <url>element://model:project::TNS.AdExpress.Web.Controls/design:view:::a6hx33ynklrfe4g_v</url>
		private bool CanAddDetailLevelItem(DetailLevelItemInformation currentDetailLevel){
           
			switch(currentDetailLevel.Id){
				case DetailLevelItemInformation.Levels.slogan:
					return _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG);
				case DetailLevelItemInformation.Levels.interestCenter:
				case DetailLevelItemInformation.Levels.mediaSeller:
					return(!_customerWebSession.isCompetitorAdvertiserSelected());
				case DetailLevelItemInformation.Levels.brand:
					return ((CheckProductDetailLevelAccess()) && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE));
				case DetailLevelItemInformation.Levels.product:
					return ((CheckProductDetailLevelAccess()) && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG));
				case DetailLevelItemInformation.Levels.advertiser:
					return(CheckProductDetailLevelAccess());
				case DetailLevelItemInformation.Levels.sector:
				case DetailLevelItemInformation.Levels.subSector:
				case DetailLevelItemInformation.Levels.group:
                    return (CheckProductDetailLevelAccess() && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_GROUP_LEVEL_ACCESS_FLAG));
				case DetailLevelItemInformation.Levels.segment:
                    return (CheckProductDetailLevelAccess() && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG));
				case DetailLevelItemInformation.Levels.holdingCompany:
					return(CheckProductDetailLevelAccess());
				case DetailLevelItemInformation.Levels.groupMediaAgency:
				case DetailLevelItemInformation.Levels.agency:
                    List<Int64> vehicleList = GetVehicles();
                    return (_customerWebSession.CustomerLogin.CustomerMediaAgencyFlagAccess(vehicleList));
                case DetailLevelItemInformation.Levels.mediaGroup:
                    return _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MEDIA_GROUP);
				default:
					return(true);
			}
		}

		/// <summary>
		/// Verifie les droits pour une liste de colonnes
		/// </summary>
		/// <param name="columnItemList">Liste des colonnes par média</param>
		/// <returns></returns>
        private List<GenericColumnItemInformation> ColumnRight(List<GenericColumnItemInformation> columnItemList)
        {
            List<GenericColumnItemInformation> columnList = new List<GenericColumnItemInformation>();

			foreach(GenericColumnItemInformation column in columnItemList)
				if(CanAddColumnItem(column))
					columnList.Add(column);
			
		return(columnList);		
		}
	
		/// <summary>
		/// Verifie les droits pour une colonne
		/// </summary>
		/// <param name="currentColumn">Colonne courante</param>
		/// <returns></returns>
		private bool CanAddColumnItem(GenericColumnItemInformation currentColumn){
			switch(currentColumn.Id){
				case GenericColumnItemInformation.Columns.slogan:
					return _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG);
				case GenericColumnItemInformation.Columns.interestCenter:
				case GenericColumnItemInformation.Columns.mediaSeller:
					return(!_customerWebSession.isCompetitorAdvertiserSelected());
				case GenericColumnItemInformation.Columns.visual:
				case GenericColumnItemInformation.Columns.associatedFile:			
					return _customerWebSession.CustomerLogin.ShowCreatives(VehiclesInformation.DatabaseIdToEnum(_idVehicleFromTab));
				case GenericColumnItemInformation.Columns.product:
					return _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
				default:
					return(true);
			}
		}
		/// <summary>
		/// Initialisation de la liste des id des colonnes sélectionnées
		/// </summary>
		private string ColumnIdListInit(){
			bool isFirst=true;

			if (!Page.IsPostBack || _columnSetId!=_oldColumnSetId){
                foreach (GenericColumnItemInformation Column in _columnItemSelectedList){
					if(isFirst){
						_formColumnItemSelectedList+=(int)Column.Id;
						isFirst=false;
					}
					else
						_formColumnItemSelectedList+=","+(int)Column.Id;
				}
			}
			else{
				foreach(GenericColumnItemInformation Column in _columnItemSelectedList){
					if(isFirst){
						_formColumnItemSelectedList+=(int)Column.Id;
						isFirst=false;
					}
					else
						_formColumnItemSelectedList+=","+(int)Column.Id;
				}
				if (_columnItemSelectedList.Count==0)
					_formColumnItemSelectedList="-1";
			}
			return (_formColumnItemSelectedList);
		}

		/// <summary>
		/// Vérifie si le client à le droit de voir un détail produit dans les plan media
		/// </summary>
		/// <returns>True si oui false sinon</returns>
		private bool CheckProductDetailLevelAccess(){
			return (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG));
			
		}

        private List<Int64> GetVehicles()
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
            return vehicleList;
        }
		#endregion


	}
}
