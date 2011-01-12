#region Informations
// Auteur: G. Facon 
// Date de création: 09/03/2006
// Date de modification: 
/*
 *		G RAgneau - 12/12/2006 - Patch ligne 519.
 * 
 * */
#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using WebConstantes=TNS.AdExpress.Constantes.Web;
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

namespace TNS.AdExpress.Web.Controls.Headers{
	/// <summary>
	/// Description résumée de GenericMediaLevelDetailSelectionWebControl.
	/// </summary>
	[ToolboxData("<{0}:GenericMediaLevelDetailSelectionWebControl runat=server></{0}:GenericMediaLevelDetailSelectionWebControl>")]
	public class GenericMediaLevelDetailSelectionWebControl : System.Web.UI.WebControls.WebControl{

		#region Variables
		/// <summary>
		/// Nombre de niveaux de détaille personnalisés
		/// </summary>
		protected int _nbDetailLevelItemList=4;
		/// <summary>
		/// Liste des niveaux de détaille personnalisés
		/// </summary>
		protected ArrayList _DetailLevelItemList=new ArrayList();
		///<summary>
		/// Session du client
		/// </summary>
		///  <label>_customerWebSession</label>
		protected WebSession _customerWebSession = null;
		/// <summary>
		/// Module Courrant
		/// </summary>
		protected Module _currentModule=null;
		/// <summary>
		/// Couleur de fond du composant
		/// </summary>
		protected string _backgroundColor="#ffffff";
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
		/// Chemin de la page de sauvegarde
		/// </summary>
		protected string _saveASPXFilePath="test.aspx";
		/// <summary>
		/// Chemin de la page de sauvegarde
		/// </summary>
		protected string _removeASPXFilePath="test.aspx";
		///<summary>
		/// Type des niveaux de détail
		/// </summary>
		///  <label>_genericDetailLevelType</label>
		protected WebConstantes.GenericDetailLevel.Type _genericDetailLevelType=WebConstantes.GenericDetailLevel.Type.mediaSchedule;
		///<summary>
		/// Profile du composant
		/// </summary>
		///  <label>_componentProfile</label>
		protected WebConstantes.GenericDetailLevel.ComponentProfile _componentProfile=WebConstantes.GenericDetailLevel.ComponentProfile.media;
		///<summary>
		/// Niveaux sauvegardé
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.Sessions.GenericDetailLevelSaved</associates>
		///  <label>_genericDetailLevelsSaved</label>
		protected Hashtable _genericDetailLevelsSaved=new Hashtable();
		/// <summary>
		/// Force le composant à s'initialiser avec les valeurs du module
		/// </summary>
		protected Int64 _forceModuleId=-1;
		/// <summary>
		/// Niveau de détail utilisé
		/// </summary>
		protected GenericDetailLevel _customerGenericDetailLevel=null;

		#endregion

		#region Variables MMI
		/// <summary>
		/// Choix du niveau de détail media par défaut
		/// </summary>
		public DropDownList _defaultDetail;
		/// <summary>
		/// Choix du niveau de détail media par défaut
		/// </summary>
		public DropDownList _customDetail;
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
		/// Choix du niveau de détail L4
		/// </summary>
		public DropDownList _l4Detail;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public GenericMediaLevelDetailSelectionWebControl():base(){
			this.EnableViewState = true;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Force le composant à s'initialiser avec les valeurs du module
		/// </summary>
		public Int64 ForceModuleId{
			set{_forceModuleId=value;}
		}
		/// <summary>
		/// Obtient ou définit le type des niveaux de détail
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		Description("Type des niveaux de détail")]
		public WebConstantes.GenericDetailLevel.Type GenericDetailLevelType{
			get{return(_genericDetailLevelType);}
			set{_genericDetailLevelType=value;}
		}
		/// <summary>
		/// Obtient ou définit Le profile du coposant
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		Description("Profile du composant"),
		DefaultValue("media")
		]
		public WebConstantes.GenericDetailLevel.ComponentProfile GenericDetailLevelComponentProfile{
			get{return(_componentProfile);}
			set{_componentProfile=value;}
		}

		#region removeASPXFilePath
		/// <summary>
		/// Obtient ou définit la Page permettant de sauvegarer le niveaux de détail
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("test.aspx"),
		Description("Page permettant de supprimer un niveaux de détail sauvegardé")]
		public string RemoveASPXFilePath{
			get{return(_removeASPXFilePath);}
			set{_removeASPXFilePath=value;}
		}

		#endregion

		#region saveASPXFilePath
		/// <summary>
		/// Obtient ou définit la Page permettant de sauvegarer le niveaux de détail
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("test.aspx"),
		Description("Page permettant de sauvegarer le niveaux de détail")]
		public string SaveASPXFilePath{
			get{return(_saveASPXFilePath);}
			set{_saveASPXFilePath=value;}
		}

		#endregion

		#region Css
		/// <summary>
		/// Obtient ou définit la couleur de fond du composant
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("#ffffff"),
		Description("Couleur de fond du composant")]
		public string BackGroundColor{
			get{return(_backgroundColor);}
			set{_backgroundColor=value;}
		}

		/// <summary>
		/// Obtient ou définit la classe Css pour le libellé de la liste des niveaux par défaut
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("txtGris11Bold"),
		Description("Classe Css pour le libellé de la liste des niveaux par défaut")]
		public string CssDefaultListLabel{
			get{return(_cssDefaultListLabel);}
			set{_cssDefaultListLabel=value;}
		}
		/// <summary>
		/// Obtient ou définit la classe Css pour le libellé des listes personnalisées
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("txtViolet11Bold"),
		Description("Classe Css pour le libellé des listes personnalisées")]
		public string CssListLabel{
			get{return(_cssListLabel);}
			set{_cssListLabel=value;}
		}
		/// <summary>
		/// Obtient ou définit la classe Css pour les listbox
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("txtNoir11Bold"),
		Description("Classe Css pour les listbox")]
		public string CssListBox{
			get{return(_cssListBox);}
			set{_cssListBox=value;}
		}
		/// <summary>
		/// Obtient ou définit la classe Css pour le titre de la section personnalisée
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("txtViolet11Bold"),
		Description("Classe Css pour le titre de la section personnalisée")]
		public string CssCustomSectionTitle{
			get{return(_cssCustomSectionTitle);}
			set{_cssCustomSectionTitle=value;}
		}
		#endregion

		/// <summary>
		/// Obtient ou définit le nombre de niveaux de détaille personnalisés
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("4")]
		public int NbDetailLevelItemList{
			get{return(_nbDetailLevelItemList);}
			set{
				if(value<1 || value>4)throw(new ArgumentOutOfRangeException("The value of NbDetailLevelItemList must be between 1 and 4"));
				_nbDetailLevelItemList=value;
			}
		}
		

		/// <summary>
		/// Définit la session du client
		/// </summary>
		[Bindable(false)]
		public WebSession CustomerWebSession{
			//get{return _customerWebSession;}
			set{_customerWebSession=value;}
		}
		#endregion
		
		#region Javascript
		/// <summary>
		/// Génére les javascripts utilisés pour le controle des listes
		/// </summary>
		/// <returns>Code Javascript</returns>
		private string GetLevelJavaScript(){
			StringBuilder script=new StringBuilder(2000);

			script.Append("<script language=\"JavaScript\">");
			
			script.Append("\r\nfunction remove(){");
			script.Append("\r\n\t var oN=document.all.item('customDetail_"+this.ID+"');");
			script.Append("\r\n\t var Id=oN.value;");
			script.Append("\r\n\t var Name=oN.name;");
			script.Append("\r\n\t var Index=oN.selectedIndex;");
			script.Append("\r\n\t url='"+_removeASPXFilePath+"?idSession="+_customerWebSession.IdSession+"&genericDetailLevelSavedId='+Id+'&genericDetailControlName='+Name+'&genericDetailLevelSavedIndex='+Index;");
			script.Append("\r\n\t window.open(url, '', \"top=\"+(screen.height-100)/2+\", left=\"+(screen.width-300)/2+\",toolbar=0, directories=0, status=0, menubar=0, width=300, height=100, scrollbars=0, location=0, resizable=0\");");
			script.Append("\r\n}");


//			script.Append("\r\nfunction remove(){");
//			script.Append("\r\n\t var oN=document.all.item('customDetail_"+this.ID+"');");
//			script.Append("\r\n\t var tmp=oN.value;");
//			script.Append("\r\n\t url='"+_removeASPXFilePath+"?idSession="+_customerWebSession.IdSession+"&genericDetailLevelSavedId='+tmp;");
//			script.Append("\r\n\t window.open(url, '', \"toolbar=0, directories=0, status=0, menubar=0, width=440, height=300, scrollbars=0, location=0, resizable=0\");");
//			script.Append("\r\n}");

			script.Append("\r\nfunction save(){");
			script.Append("\r\n\t var tmp=''");
			script.Append("\r\n\t var url=''");
			script.Append("\r\n\t var new_object = new Option(\"\",\"\");");
			script.Append("\r\n\t var oT=document.all.item('customDetail_"+this.ID+"');");
			script.Append("\r\n\t oT.options.add(new_object);");
			for(int k=1;k<=_nbDetailLevelItemList;k++){
				script.Append("\r\n\t var oN"+k.ToString()+"=document.all.item('l"+k.ToString()+"Detail_"+this.ID+"');");
				script.Append("\r\n\t tmp=tmp+oN"+k.ToString()+".value+',';");
			}
			script.Append("\r\n\t url='"+_saveASPXFilePath+"?idSession="+_customerWebSession.IdSession+"&detailLevel='+tmp+'&detailLevelType="+((int)_genericDetailLevelType).ToString()+"&genericDetailControlName=customDetail_"+this.ID+"'");
			script.Append("\r\n\t window.open(url, '', \"top=\"+(screen.height-100)/2+\", left=\"+(screen.width-300)/2+\",toolbar=0, directories=0, status=1, menubar=0, width=300, height=100, scrollbars=0, location=0, resizable=0\");");
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
				script.Append("\r\nfunction setN"+i.ToString()+"(detail,customDetail");
				for(int j=1;j<=_nbDetailLevelItemList;j++)script.Append(",N"+j.ToString()); 
				script.Append("){");
				for(int k=1;k<=_nbDetailLevelItemList;k++)script.Append("\r\n\t var oN"+k.ToString()+"=document.all.item(N"+k.ToString()+");");
				script.Append("\r\n\t setN(detail,customDetail);");
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
		#endregion

		#region Evènements

		#region Init
        /// <summary>
        /// Ibnitialization
        /// </summary>
        /// <param name="e">Parameters</param>
        protected  void Init(EventArgs e)
        {
            base.OnInit(e);
        }
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguements</param>
		protected override void OnInit(EventArgs e) {
			string onChange="";
			base.OnInit (e);
			
			if(_forceModuleId<0)
				_currentModule=ModulesList.GetModule(_customerWebSession.CurrentModule);
			else
				_currentModule=ModulesList.GetModule(_forceModuleId);

			#region on vérifie que le niveau sélectionné à le droit d'être utilisé
			bool canAddDetail = false;
			switch(_componentProfile){
				case WebConstantes.GenericDetailLevel.ComponentProfile.media:
					try{
						canAddDetail = CanAddDetailLevel(_customerWebSession.GenericMediaDetailLevel,_customerWebSession.CurrentModule);
					}
					catch{}
					if(!canAddDetail){
						// Niveau de détail par défaut
						ArrayList levelsIds=new ArrayList();
						switch(_customerWebSession.CurrentModule){
							case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
								levelsIds.Add((int)DetailLevelItemInformation.Levels.media);								
								break;
							case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
								levelsIds.Add((int)DetailLevelItemInformation.Levels.advertiser);
								levelsIds.Add((int)DetailLevelItemInformation.Levels.sector);
								break;
							default:
								levelsIds.Add((int)DetailLevelItemInformation.Levels.vehicle);
								levelsIds.Add((int)DetailLevelItemInformation.Levels.category);
								break;
						}
						_customerWebSession.GenericMediaDetailLevel=new GenericDetailLevel(levelsIds,WebConstantes.GenericDetailLevel.SelectedFrom.unknown);
					}
					break;
				case WebConstantes.GenericDetailLevel.ComponentProfile.product:
					try{
						canAddDetail = CanAddDetailLevel(_customerWebSession.GenericProductDetailLevel,_customerWebSession.CurrentModule);
					}
					catch{}
					if(!canAddDetail){
						// Niveau de détail par défaut
						ArrayList levelsIds=new ArrayList();
						levelsIds.Add((int)DetailLevelItemInformation.Levels.advertiser);
						_customerWebSession.GenericProductDetailLevel=new GenericDetailLevel(levelsIds,WebConstantes.GenericDetailLevel.SelectedFrom.unknown);
					}
					break;
			}
			#endregion

			#region Niveau de détaille par défaut

			_defaultDetail = new DropDownList();
			_defaultDetail.EnableViewState=true;
			_defaultDetail.Width = new System.Web.UI.WebControls.Unit(this.Width.Value);
			_defaultDetail.ID = "defaultDetail_"+this.ID;
			//_defaultMediaDetail.Attributes["onchage"]="javascript:setLevel('"++"','"++"');";
			_defaultDetail.AutoPostBack=false;
			_defaultDetail.CssClass=_cssListBox;

			_defaultDetail.Items.Add(new ListItem("-------","-1"));
			int DefaultDetailLevelId=0;
			ArrayList DefaultDetailLevels=GetDefaultDetailLevels();
			foreach(GenericDetailLevel currentLevel in DefaultDetailLevels){
				if(CanAddDetailLevel(currentLevel,_customerWebSession.CurrentModule))
					_defaultDetail.Items.Add(new ListItem(currentLevel.GetLabel(_customerWebSession.SiteLanguage),DefaultDetailLevelId.ToString()));
				DefaultDetailLevelId++;
			}
			Controls.Add(_defaultDetail);
			#endregion

			#region Niveau de détaille par personnalisé
			// Obtient les niveaux de détail sauvegardés		
			ArrayList genericDetailLevelsSaved=GetGenericDetailLevelsSaved();
			
			_customDetail = new DropDownList();
			_customDetail.Width = new System.Web.UI.WebControls.Unit(this.Width.Value-4);
			_customDetail.ID = "customDetail_"+this.ID;
			//_defaultMediaDetail.Attributes["onchage"]="javascript:setLevel('"++"','"++"');";
			_customDetail.AutoPostBack=false;
			_customDetail.CssClass=_cssListBox;

			_customDetail.Items.Add(new ListItem("-------","-1"));
			foreach(GenericDetailLevelSaved currentGenericLevel in genericDetailLevelsSaved){
				if(CanAddDetailLevel(currentGenericLevel,_customerWebSession.CurrentModule) && currentGenericLevel.GetNbLevels<=_nbDetailLevelItemList){
					_customDetail.Items.Add(new ListItem(currentGenericLevel.GetLabel(_customerWebSession.SiteLanguage),currentGenericLevel.Id.ToString()));
					_genericDetailLevelsSaved.Add(currentGenericLevel.Id,currentGenericLevel);
				}
			}

			Controls.Add(_customDetail);
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

			#region L4
			if(_nbDetailLevelItemList>=4){
				_l4Detail= new DropDownList();
				DetailLevelItemInit(_l4Detail,4);
			}
			#endregion

			#endregion

			#region Ecriture des onchange

			#region OnChange _defaultDetail 
			onChange="javascript:setLevel('"+_customDetail.ID+"'";;
			for(int i=1;i<=_nbDetailLevelItemList;i++){
				onChange+=",'"+"l"+i.ToString()+"Detail_"+this.ID+"'";
			}
			onChange+=");";
			_defaultDetail.Attributes["onchange"]=onChange;
			#endregion

			#region OnChange _customDetail 
			onChange="javascript:setLevel('"+_defaultDetail.ID+"'";
			for(int i=1;i<=_nbDetailLevelItemList;i++){
				onChange+=",'"+"l"+i.ToString()+"Detail_"+this.ID+"'";
			}
			onChange+=");";
			_customDetail.Attributes["onchange"]=onChange;
			#endregion

			#endregion

		}

		#endregion

		#region Load
		/// <summary>
		/// Chargement du composant
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnLoad(EventArgs e) {
			base.OnLoad (e);
			ArrayList levels=new ArrayList();
			switch(_componentProfile){
				case WebConstantes.GenericDetailLevel.ComponentProfile.media:
					_customerGenericDetailLevel=_customerWebSession.GenericMediaDetailLevel;
					break;
				case WebConstantes.GenericDetailLevel.ComponentProfile.product:
					_customerGenericDetailLevel=_customerWebSession.GenericProductDetailLevel;
					break;
			}

			#region Gestion de la sélection
			if (Page.IsPostBack){
				if(int.Parse(_defaultDetail.SelectedValue)>=0){
					if (Page.Request.Form.GetValues("defaultDetail_"+this.ID) != null && Page.Request.Form.GetValues("defaultDetail_"+this.ID).Length>0){
						//added by gragneau on december, 12th
						_customerGenericDetailLevel=(GenericDetailLevel) GetDefaultDetailLevels()[int.Parse(Page.Request.Form.GetValues("defaultDetail_"+this.ID)[0])];
					}
					// Pb ICI TODO
					_customerGenericDetailLevel.FromControlItem=WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels;
				}
				if(int.Parse(_customDetail.SelectedValue)>=0){
					//_customerWebSession.GenericMediaDetailLevel=(GenericDetailLevel) _currentModule.DefaultMediaDetailLevels[int.Parse(Page.Request.Form.GetValues("defaultDetail_"+this.ID)[0])];
					if (Page.Request.Form.GetValues("customDetail_"+this.ID) != null && Page.Request.Form.GetValues("customDetail_"+this.ID).Length>0){
						_customerGenericDetailLevel=(GenericDetailLevel)_genericDetailLevelsSaved[Int64.Parse(Page.Request.Form.GetValues("customDetail_"+this.ID)[0])];
					}
					_customerGenericDetailLevel.FromControlItem=WebConstantes.GenericDetailLevel.SelectedFrom.savedLevels;
				}
				if(_nbDetailLevelItemList>=1 && int.Parse(_l1Detail.SelectedValue)>=0){
					levels.Add(int.Parse(_l1Detail.SelectedValue));
				}
				if(_nbDetailLevelItemList>=2 && int.Parse(_l2Detail.SelectedValue)>=0){
					levels.Add(int.Parse(_l2Detail.SelectedValue));
				}
				if(_nbDetailLevelItemList>=3 &&int.Parse(_l3Detail.SelectedValue)>=0){
					levels.Add(int.Parse(_l3Detail.SelectedValue));
				}
				if(_nbDetailLevelItemList>=4 && int.Parse(_l4Detail.SelectedValue)>=0){
					levels.Add(int.Parse(_l4Detail.SelectedValue));
				}
				if(levels.Count>0){
					_customerGenericDetailLevel=new GenericDetailLevel(levels,WebConstantes.GenericDetailLevel.SelectedFrom.customLevels);
					//_customerWebSession.GenericMediaDetailLevel.FromControlItem=WebConstantes.GenericDetailLevel.SelectedFrom.customLevels;
				}
                switch (_customerGenericDetailLevel.FromControlItem)
                {
                    case WebConstantes.GenericDetailLevel.SelectedFrom.customLevels:
                        if (levels.Count <= 0)
                        {
                            if (_nbDetailLevelItemList >= 1 && _customerGenericDetailLevel.GetNbLevels >= 1 && _customerGenericDetailLevel.LevelIds[0] != null)
                            {
                                _l1Detail.SelectedValue = ((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[0]).GetHashCode().ToString();
                            }
                            if (_nbDetailLevelItemList >= 2 && _customerGenericDetailLevel.GetNbLevels >= 2 && _customerGenericDetailLevel.LevelIds[1] != null)
                            {
                                _l2Detail.SelectedValue = ((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[1]).GetHashCode().ToString();
                            }
                            if (_nbDetailLevelItemList >= 3 && _customerGenericDetailLevel.GetNbLevels >= 3 && _customerGenericDetailLevel.LevelIds[2] != null)
                            {
                                _l3Detail.SelectedValue = ((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[2]).GetHashCode().ToString();
                            }
                            if (_nbDetailLevelItemList >= 4 && _customerGenericDetailLevel.GetNbLevels >= 4 && _customerGenericDetailLevel.LevelIds[3] != null)
                            {
                                _l4Detail.SelectedValue = ((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[3]).GetHashCode().ToString();
                            }

                        }
                        break;
                    case WebConstantes.GenericDetailLevel.SelectedFrom.savedLevels:
                        if (int.Parse(_customDetail.SelectedValue) < 0)
                        {
                            foreach (GenericDetailLevelSaved currentLevel in _genericDetailLevelsSaved.Values)
                            {
                                if (currentLevel.EqualLevelItems(_customerGenericDetailLevel))
                                {
                                    _customDetail.SelectedValue = currentLevel.Id.ToString();
                                    break;
                                }
                            }
                        }
                        break;
                    case WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels:
                        if (int.Parse(_defaultDetail.SelectedValue) < 0)
                        {
                            int index = -1;
                            foreach (GenericDetailLevel currentLevel in GetDefaultDetailLevels())
                            {
                                if (CanAddDetailLevel(currentLevel, _customerWebSession.CurrentModule)) index++;
                                if (currentLevel.EqualLevelItems(_customerGenericDetailLevel)) _defaultDetail.SelectedValue = index.ToString();
                            }
                        }
                        break;
                }
			}
			else{
				if(_customerWebSession.GenericMediaDetailLevel.FromControlItem==WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels){
					int index=-1;
					foreach(GenericDetailLevel currentLevel in GetDefaultDetailLevels()){
						if(CanAddDetailLevel(currentLevel,_customerWebSession.CurrentModule))index++;
						if(currentLevel.EqualLevelItems(_customerGenericDetailLevel))_defaultDetail.SelectedValue=index.ToString();
					}
				}
				if(_customerGenericDetailLevel.FromControlItem==WebConstantes.GenericDetailLevel.SelectedFrom.savedLevels){
//					TODO					
					foreach(GenericDetailLevelSaved currentLevel in _genericDetailLevelsSaved.Values){
						if(CanAddDetailLevel(currentLevel,_customerWebSession.CurrentModule) && currentLevel.EqualLevelItems(_customerWebSession.GenericMediaDetailLevel))_customDetail.SelectedValue=currentLevel.Id.ToString();
					}
				}
				if(_customerGenericDetailLevel.FromControlItem==WebConstantes.GenericDetailLevel.SelectedFrom.customLevels){
					if(_nbDetailLevelItemList>=1 && _customerGenericDetailLevel.GetNbLevels>=1 && _customerGenericDetailLevel.LevelIds[0]!=null){
						_l1Detail.SelectedValue=((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[0]).GetHashCode().ToString();
					}
					if(_nbDetailLevelItemList>=2 && _customerGenericDetailLevel.GetNbLevels>=2 && _customerGenericDetailLevel.LevelIds[1]!=null){
						_l2Detail.SelectedValue=((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[1]).GetHashCode().ToString();
					}
					if(_nbDetailLevelItemList>=3 && _customerGenericDetailLevel.GetNbLevels>=3 && _customerGenericDetailLevel.LevelIds[2]!=null){
						_l3Detail.SelectedValue=((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[2]).GetHashCode().ToString();
					}
					if(_nbDetailLevelItemList>=4 && _customerGenericDetailLevel.GetNbLevels>=4 && _customerGenericDetailLevel.LevelIds[3]!=null){
						_l4Detail.SelectedValue=((DetailLevelItemInformation.Levels)_customerGenericDetailLevel.LevelIds[3]).GetHashCode().ToString();
					}
					
				}
			}
			#endregion
			switch(_componentProfile){
				case WebConstantes.GenericDetailLevel.ComponentProfile.media:
					_customerWebSession.GenericMediaDetailLevel=_customerGenericDetailLevel;
					break;
				case WebConstantes.GenericDetailLevel.ComponentProfile.product:
					_customerWebSession.GenericProductDetailLevel=_customerGenericDetailLevel;
					break;
			}

		}
		#endregion

		#region Prérender
		/// <summary>
		/// Préparation du rendu des niveaux de détails personnalisés.
		/// </summary>
		/// <param name="e">Sender</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender (e);
			if(!this.Page.ClientScript.IsClientScriptBlockRegistered("genericDetailSelectionControl"))this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"genericDetailSelectionControl",GetLevelJavaScript());
			if(!this.Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer"))this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DivDisplayer",TNS.AdExpress.Web.Functions.Script.DivDisplayer());
		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){

            string themeName = WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name;

			output.Write("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" bgcolor=\""+_backgroundColor+"\">");
			output.Write("<tr>");
			output.Write("<td class=\""+_cssDefaultListLabel+"\">"+ GestionWeb.GetWebWord(1886,_customerWebSession.SiteLanguage) +"</td>");
			output.Write("</tr>");
			output.Write("<tr>");
			output.Write("<td>");
			// Liste par défaut
			_defaultDetail.RenderControl(output);
			output.Write("</td>");
			output.Write("</tr>");
			// Espace blanc
			output.Write("<tr>");
			output.Write("<td><img src=\"/App_Themes/"+themeName+"/Images/Common/pixel.gif\" border=\"0\" height=\"10\"></td>");
			output.Write("</tr>");
			// table de personnalisation
			output.Write("<tr>");
			output.Write("<td>");
            output.Write("<table class=\"backgroundGenericMediaLevelDetail genericMediaLevelDetailBorder\" cellSpacing=\"0\" cellPadding=\"0\" width=\"" + this.Width + "\" border=\"0\">");
            output.Write("<tr onclick=\"DivDisplayer('detailledLevelContent');\" class=\"cursorHand\">");
			//Titre de la section
			output.Write("<td class=\""+_cssCustomSectionTitle+"\">&nbsp;"+ GestionWeb.GetWebWord(1896,_customerWebSession.SiteLanguage) +"&nbsp;</td>");
			// Image d'ouverture de la section
            output.Write("<td align=\"right\" class=\"arrowBackGroundGenericMediaLevelDetail\"></td>");
			output.Write("</tr>");
			output.Write("</table>");
			
            // Section
            output.Write("\r\n<div id=\"detailledLevelContent\" class=\"GenericMediaLevelDetailSelectionSection\" style=\"DISPLAY: none; WIDTH: " + this.Width + "px;\">");
			output.Write("\r\n<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" ID=\"Section\">");

            // Niveaux personnalisés déjà enregistrés
            output.Write("\r\n<tr>");
			output.Write("\r\n<td class=\""+_cssListLabel+"\">"+ GestionWeb.GetWebWord(1897,_customerWebSession.SiteLanguage) +" :</td>");
			output.Write("\r\n</tr>");
			output.Write("\r\n<tr>");
			output.Write("\r\n<td>");
			_customDetail.RenderControl(output);
			output.Write("\r\n</td>");
			output.Write("\r\n</tr>");
            // Espace blanc
            output.Write("\r\n<tr>");
            output.Write("\r\n<td colspan=2><img src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\" border=\"0\" height=\"5\"></td>");
            output.Write("\r\n</tr>");
			// Bouton supprimer
			output.Write("\r\n<tr height=\"15\">");
            output.Write("\r\n<td align=\"right\"><a class=\"roll03\" href=\"javascript: remove();\"  onmouseover=\"deleteButton.src='/App_Themes/" + themeName + "/Images/Common/button/bt_delete2_down.gif';\" onmouseout=\"deleteButton.src ='/App_Themes/" + themeName + "/Images/Common/button/bt_delete2_up.gif';\"><img name=deleteButton border=0 src=\"/App_Themes/" + themeName + "/Images/Common/button/bt_delete2_up.gif\" alt=\"" + GestionWeb.GetWebWord(1951, _customerWebSession.SiteLanguage) + "\"></a>&nbsp;</td>");	
			output.Write("\r\n</tr>");
			output.Write("\r\n<tr>");
			output.Write("\r\n<td>");
			
            // Sélection des niveaux de détail
			output.Write("\r\n<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" ID=\"levelSelection\">");
			output.Write("\r\n<tr>");
			// Construction des niveaux
			output.Write("\r\n<td colspan=\"2\" class=\"txtViolet11Bold\">"+ GestionWeb.GetWebWord(1899,_customerWebSession.SiteLanguage) +" :</td>");
			output.Write("\r\n</tr>");
			
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
				output.Write("\r\n<td colspan=2><img src=\"/App_Themes/"+themeName+"/Images/Common/pixel.gif\" border=\"0\" height=\"5\"></td>");
				output.Write("\r\n</tr>");
			}
			// Bouton de sauvegarde
            output.Write("\r\n<tr height=\"15\">");
			output.Write("\r\n<td colspan=2 align=\"right\">");
			if(_saveASPXFilePath!=null && _saveASPXFilePath.Length>1)
                output.Write("<a class=\"roll03\" href=\"javascript: save();\"  onmouseover=\"saveLevelDetailButton.src='/App_Themes/" + themeName + "/Images/Common/button/bt_save_down.gif';\" onmouseout=\"saveLevelDetailButton.src ='/App_Themes/" + themeName + "/Images/Common/button/bt_save_up.gif';\"><img name=saveLevelDetailButton border=0 src=\"/App_Themes/" + themeName + "/Images/Common/button/bt_save_up.gif\" alt=\"" + GestionWeb.GetWebWord(1952, _customerWebSession.SiteLanguage) + "\"></a>&nbsp;");
			else
				output.Write("&nbsp;");
			output.Write("</td>");	

			// Fin niveau de détaille N1...
			output.Write("\r\n</tr>");
			output.Write("\r\n</table>");
			output.Write("\r\n</td>");
			output.Write("\r\n</tr>");
			//Fin table Avant Div
			output.Write("\r\n</table>");
			output.Write("\r\n</div>");
			output.Write("\r\n</td>");
			output.Write("\r\n</tr>");
			output.Write("\r\n</table>");	
		}
		#endregion

		#endregion

		#region Méthode privée
		/// <summary>
		/// Return allowed detail level items
		/// </summary>
		/// <remarks>
		/// AdNetTrack selection [Ok]
		/// </remarks>
		/// <returns>Detail level list</returns>
		private ArrayList GetAllowedDetailLevelItems(){

            List<DetailLevelItemInformation.Levels> vehicleAllowedDetailLevelList;
            ArrayList allowedDetailLevelList;
            ArrayList list = new ArrayList();

            switch (_componentProfile) { 
                case WebConstantes.GenericDetailLevel.ComponentProfile.adnettrack:
                    return GetModuleAllowedDetailLevelItems();
                case WebConstantes.GenericDetailLevel.ComponentProfile.media:
                case WebConstantes.GenericDetailLevel.ComponentProfile.product:
                default:

                    vehicleAllowedDetailLevelList = GetVehicleAllowedDetailLevelItems();
                    allowedDetailLevelList = GetModuleAllowedDetailLevelItems();

                    foreach (DetailLevelItemInformation currentLevel in allowedDetailLevelList)
                        if (vehicleAllowedDetailLevelList.Contains(currentLevel.Id))
                            list.Add(currentLevel);

                    return list;
            }
		}

        /// <summary>
        /// Return allowed detail level list contained in the module object
        /// </summary>
        /// <returns>Detail level list</returns>
        private ArrayList GetModuleAllowedDetailLevelItems() {

            switch (_componentProfile) {
                case WebConstantes.GenericDetailLevel.ComponentProfile.media:
                    return (_currentModule.AllowedMediaDetailLevelItems);
                case WebConstantes.GenericDetailLevel.ComponentProfile.product:
                    return (_currentModule.AllowedProductDetailLevelItems);
                case WebConstantes.GenericDetailLevel.ComponentProfile.adnettrack:
                    return (AdNetTrackDetailLevelsDescription.AllowedAdNetTrackLevelItems);
                default:
                    return (new ArrayList());
            }

        }

        /// <summary>
        /// Return allowed detail level list for vehicle list seleceted
        /// </summary>
        /// <returns>Detail level list</returns>
        private List<DetailLevelItemInformation.Levels> GetVehicleAllowedDetailLevelItems(){

            List<Int64> vehicleList = GetVehicles();          
            List<DetailLevelItemInformation.Levels> levelList = VehiclesInformation.GetCommunDetailLevelList(vehicleList);
           
            return levelList;

        }

      

		/// <summary>
		/// Retourne le niveau de détail par defaut
		/// </summary>
		/// <remarks>
		/// AdNetTrack selection [Ok]
		/// </remarks>
		/// <returns>Niveau de détail</returns>
		protected  ArrayList GetDefaultDetailLevels(){
			switch(_componentProfile){
				case WebConstantes.GenericDetailLevel.ComponentProfile.media:
					return(_currentModule.DefaultMediaDetailLevels);
				case WebConstantes.GenericDetailLevel.ComponentProfile.product:
					return(_currentModule.DefaultProductDetailLevels);
				case WebConstantes.GenericDetailLevel.ComponentProfile.adnettrack:
					return(AdNetTrackDetailLevelsDescription.DefaultAdNetTrackDetailLevels);
				default:
					return(new ArrayList());
			}
		}

		/// <summary>
		/// Charge les niveaux de détail sauvegardés
		/// </summary>
		/// <returns>Niveaux de détail sauvegardés</returns>
		protected ArrayList GetGenericDetailLevelsSaved(){
			DataSet ds;
			int currentIndex=0;
			ArrayList genericDetailLevelsSaved=new ArrayList();
			Int64 currentId=-1;
			int currentLevelId;
			try{
				ds=GenericDetailLevelDataAccess.Load(_customerWebSession);
				if(ds!=null && ds.Tables.Count>0 && ds.Tables[0].Rows.Count>0){
					foreach(DataRow currentRow in ds.Tables[0].Rows){
						if(currentId!=Int64.Parse(currentRow["id_list"].ToString())){
							currentId=Int64.Parse(currentRow["id_list"].ToString());
							currentIndex=genericDetailLevelsSaved.Add(new GenericDetailLevelSaved(currentId,new ArrayList()));
						}
						currentLevelId=int.Parse(currentRow["id_type_level"].ToString());
						((GenericDetailLevelSaved)genericDetailLevelsSaved[currentIndex]).AddLevel(currentLevelId);
					}
				}
			}
			catch(System.Exception err){
				string t=err.Message;
			}
			return(genericDetailLevelsSaved);
		}


		/// <summary>
		/// Initialise une sélection d'éléments de niveau de détaille
		/// </summary>
		/// <remarks>
		/// AdNetTrack selection [Ok]
		/// </remarks>
		/// <param name="dropDownList">Liste</param>
		/// <param name="level">Niveau</param>
		protected void DetailLevelItemInit(DropDownList dropDownList,int level){
			string onChange;
			dropDownList.Width = new System.Web.UI.WebControls.Unit(this.Width.Value-23);
			dropDownList.ID="l"+level.ToString()+"Detail_"+this.ID;
			//_defaultMediaDetail.Attributes["onchage"]="javascript:setLevel('"++"','"++"');";
			dropDownList.AutoPostBack=false;
			dropDownList.CssClass=_cssListBox;
			dropDownList.Items.Add(new ListItem("-------","-1"));
			ArrayList AllowedDetailLevelItems=GetAllowedDetailLevelItems();
			foreach(DetailLevelItemInformation currentDetailLevelItem in AllowedDetailLevelItems){
				if(CanAddDetailLevelItem(currentDetailLevelItem,_customerWebSession.CurrentModule)){
					dropDownList.Items.Add(new ListItem(GestionWeb.GetWebWord(currentDetailLevelItem.WebTextId,_customerWebSession.SiteLanguage),currentDetailLevelItem.Id.GetHashCode().ToString()));
				}
			}
			_DetailLevelItemList.Add(dropDownList);

			#region OnChange
			onChange="javascript:setN"+level+"('"+_defaultDetail.ID+"','"+_customDetail.ID+"'";
			for(int i=1;i<=_nbDetailLevelItemList;i++){
				onChange+=",'"+"l"+i.ToString()+"Detail_"+this.ID+"'";// Changer aussi dans la méthode javascript save
			}
			onChange+=");";
			dropDownList.Attributes["onchange"]=onChange;
			#endregion

			Controls.Add(dropDownList);
		}

		/// <summary>
		/// Test si un niveau de détail peut être montré
		/// </summary>
		/// <remarks>
		/// AdNetTrack selection [Ok] (redéfinie)
		/// </remarks>
		/// <param name="currentDetailLevel">Niveau de détail</param>
		/// <param name="module">Module courrant</param>
		/// <returns>True s'il peut être ajouté</returns>
		protected virtual bool CanAddDetailLevel(GenericDetailLevel currentDetailLevel,Int64 module){
			ArrayList AllowedDetailLevelItems=GetAllowedDetailLevelItems();
			foreach(DetailLevelItemInformation currentDetailLevelItem in currentDetailLevel.Levels){
				if( !AllowedDetailLevelItems.Contains(currentDetailLevelItem))return(false);
				if(!CanAddDetailLevelItem(currentDetailLevelItem,module))return(false);
			}
			return(true);
		}

		/// <summary>
		/// Test si l'élément de niveau de détail peut être montré
		/// </summary>
		/// <remarks>
		/// AdNetTrack selection [Ko]
		/// </remarks>
		/// <param name="currentDetailLevelItem">Elément de niveau de détail</param>
		/// <param name="module">Module</param>
		/// <returns>True si oui false sinon</returns>
		private bool CanAddDetailLevelItem(DetailLevelItemInformation currentDetailLevelItem,Int64 module){
            List<Int64> vehicleList = null;
            switch(module){
				case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE:
				case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
				case WebConstantes.Module.Name.ALERTE_POTENTIELS:
				case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
				case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
				case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:
				case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                case WebConstantes.Module.Name.NEW_CREATIVES:
				switch(_componentProfile){
					case WebConstantes.GenericDetailLevel.ComponentProfile.media:
						switch(currentDetailLevelItem.Id){
								#region Annonceur 
							case DetailLevelItemInformation.Levels.advertiser:
								if(
									// Droit sur les niveaux de détail produit
									CheckProductDetailLevelAccess() &&
									// Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
									(_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.productAccess).Length>0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.brandAccess).Length>0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length>0||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0) &&
									// Pas de famille, classe
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length==0 &&
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length==0
									)
									return(true);
								return(false);
								#endregion

								#region  product
							case DetailLevelItemInformation.Levels.product:
								if (
									// Droit sur les niveaux de détail produit
									CheckProductDetailLevelAccess() &&
									// Products rights (For Finland)
									_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG) && 
									// Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
									(_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.productAccess).Length > 0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.brandAccess).Length > 0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length > 0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length > 0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length > 0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length > 0) &&
									// Pas de famille, classe
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length == 0 &&
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length == 0
									)
									return (true);
								return (false);
							#endregion

								#region Marques
							case DetailLevelItemInformation.Levels.brand:
								if(
									// Droit sur les niveaux de détail produit
									CheckProductDetailLevelAccess() &&
									// Droit des Marques
									_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE) && 
									// Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
									(_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.productAccess).Length>0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.brandAccess).Length>0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length>0||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0) &&
									// Pas de famille, classe, groupe, groupe d'annonceur
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length==0 &&
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length==0
									)
									return(true);
								return(false);
							
								#endregion

								#region Famille, classe, groupe, variété
							case DetailLevelItemInformation.Levels.sector:
							case DetailLevelItemInformation.Levels.subSector:
							case DetailLevelItemInformation.Levels.group:
								if(CheckProductDetailLevelAccess())return(true);
								return(false);
                            case DetailLevelItemInformation.Levels.segment:
                                if (CheckProductDetailLevelAccess() && 
                                    _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG)) return (true);
                                return (false);
								#endregion

								#region Groupe de société
							case DetailLevelItemInformation.Levels.holdingCompany:
								if(
									CheckProductDetailLevelAccess() &&
									// Droit sur les groupe de société
									_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG) &&
									// Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
									(_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.productAccess).Length>0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.brandAccess).Length>0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length>0||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length>0||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length>0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length>0) &&
									// Pas de famille, classe
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length==0 &&
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length==0
									)return(true);
								return(false);
								#endregion

								#region Agences et groupe d'agence
							case DetailLevelItemInformation.Levels.groupMediaAgency:
							case DetailLevelItemInformation.Levels.agency:
                                vehicleList = GetVehicles();                               
								if(
									CheckProductDetailLevelAccess() &&
									// Droit sur les agences media
                                    _customerWebSession.CustomerLogin.CustomerMediaAgencyFlagAccess(vehicleList)									
									)return(true);
								return(false);
								#endregion

								#region Version
							case DetailLevelItemInformation.Levels.slogan:
								if(
									currentDetailLevelItem.Id==DetailLevelItemInformation.Levels.slogan &&_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG) &&
									// Sélection par produit ou marque ou annonceur
									(
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.productAccess).Length>0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.brandAccess).Length>0 ||
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length>0) &&
									// Pas de famille, classe, groupe,variété, groupe d'annonceur webSession.ProductDetailLevel.LevelProduct
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length==0 &&
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length==0 &&
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length==0 &&
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length==0 &&
									_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement,TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length==0 &&
									// Niveau de détail par jour
									_customerWebSession.DetailPeriod==TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly
									
									)
									return(true);
								return(false);
								#endregion
							default:
								return(true);
						}
					case WebConstantes.GenericDetailLevel.ComponentProfile.product:
						switch(currentDetailLevelItem.Id) {

							#region  sector, subsector, group ,segment,Annonceur
							case DetailLevelItemInformation.Levels.sector:
							case DetailLevelItemInformation.Levels.subSector:
							case DetailLevelItemInformation.Levels.group:
							case DetailLevelItemInformation.Levels.advertiser:						
								return(true);
                            case DetailLevelItemInformation.Levels.segment:
                                if (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG)) return true;
                                return false;
								#endregion

							#region product
							case DetailLevelItemInformation.Levels.product:
								return (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG));
							#endregion

							#region Marques
							// Droit des Marques
							case DetailLevelItemInformation.Levels.brand:
							return (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE));
								
								#endregion

							#region Groupe de société
							case DetailLevelItemInformation.Levels.holdingCompany:
								return(_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY));
								
								#endregion

							#region Agences et groupe d'agence
							case DetailLevelItemInformation.Levels.groupMediaAgency:
							case DetailLevelItemInformation.Levels.agency:
                                vehicleList = GetVehicles();
                                return (_customerWebSession.CustomerLogin.CustomerMediaAgencyFlagAccess(vehicleList));
								
								#endregion

							default:
								return(false);
						}
					default:
						return(true);
				}

				case WebConstantes.Module.Name.ALERTE_PLAN_MEDIA:
				case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
 				switch(currentDetailLevelItem.Id){
						#region Annonceur 
					case DetailLevelItemInformation.Levels.advertiser:						
						if (
							// Droit sur les niveaux de détail produit
							CheckProductDetailLevelAccess() &&
							// Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
							(_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes)
							) &&
							// Pas de famille, classe
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
							)
							return (true);
						return (false);
						#endregion

						#region Product
					case DetailLevelItemInformation.Levels.product:
						if (
							// Droit sur les niveaux de détail produit
							CheckProductDetailLevelAccess() &&
							// Products rights
							_customerWebSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG) && 
							// Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
							(_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes)
							) &&
							// Pas de famille, classe
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
							)
							return (true);
						return (false);
					#endregion

						#region Marques
					case DetailLevelItemInformation.Levels.brand:						

						if (
							CheckProductDetailLevelAccess() &&
							// Droit sur les groupe de société
							_customerWebSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE) && 
							// Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
							(_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes)
							) &&
							// Pas de famille, classe
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
							)
							return (true);
						return (false);
						#endregion

						#region Famille, classe, groupe, variété
					case DetailLevelItemInformation.Levels.sector:
					case DetailLevelItemInformation.Levels.subSector:
					case DetailLevelItemInformation.Levels.group:
						if(CheckProductDetailLevelAccess())return(true);
						return(false);
                    case DetailLevelItemInformation.Levels.segment:
                        if (CheckProductDetailLevelAccess() &&
                            _customerWebSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.DB.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG)) return (true);
                        return (false);
						#endregion

						#region Groupe de société
					case DetailLevelItemInformation.Levels.holdingCompany:						

						if (
							CheckProductDetailLevelAccess() &&
							// Droit sur les groupe de société
							_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG) &&
                            _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY) &&
							// Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
							(_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes)
							) &&
							// Pas de famille, classe
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
							)
							return (true);
						return (false);
						#endregion

						#region Agences et groupe d'agence
					case DetailLevelItemInformation.Levels.groupMediaAgency:
					case DetailLevelItemInformation.Levels.agency:
                        vehicleList = GetVehicles();  
						if(
							CheckProductDetailLevelAccess() &&
							// Droit sur les agences media
                             _customerWebSession.CustomerLogin.CustomerMediaAgencyFlagAccess(vehicleList)									
							)return(true);
						return(false);
						#endregion

						#region Version
					case DetailLevelItemInformation.Levels.slogan:						

						if (
							currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.slogan && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG) &&
							// Sélection par produit ou marque ou annonceur						
							(_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT,AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND,AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes)							
							) &&														
							// Pas de famille, classe, groupe,variété, groupe d'annonceur webSession.ProductDetailLevel.LevelProduct
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes) &&														
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) &&
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes) &&														
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) &&							
							// Niveau de détail par jour
							_customerWebSession.DetailPeriod == TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly

							)
							return (true);
						return (false);

						#endregion

					default:
						return(true);
				}

                case WebConstantes.Module.Name.ANALYSE_MANDATAIRES:
                switch (currentDetailLevelItem.Id)
                {
                    #region Annonceur
                    case DetailLevelItemInformation.Levels.advertiser:
                        if (
                            // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                            (_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISING_AGENCY, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_ADVERTISING_AGENCY, AccessType.includes) 
                            ) &&
                            // Pas de famille, classe
                            !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                            !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                            )
                            return (true);
                        return (false);
                    #endregion

                    #region Product
                    case DetailLevelItemInformation.Levels.product:
                        if (
                            // Products rights
                            _customerWebSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG) &&
                            // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                            (_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISING_AGENCY, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_ADVERTISING_AGENCY, AccessType.includes) 
                            ) &&
                            // Pas de famille, classe
                            !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                            !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                            )
                            return (true);
                        return (false);
                    #endregion

                    #region Marques
                    case DetailLevelItemInformation.Levels.brand:

                        if (
                             // Droit sur les groupe de société
                            _customerWebSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE) &&
                            // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                            (_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISING_AGENCY, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_ADVERTISING_AGENCY, AccessType.includes) 
                            ) &&
                            // Pas de famille, classe
                            !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                            !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                            )
                            return (true);
                        return (false);
                    #endregion

                    #region Famille, classe, groupe, variété
                    case DetailLevelItemInformation.Levels.sector:
                    case DetailLevelItemInformation.Levels.subSector:
                    case DetailLevelItemInformation.Levels.group:
                        return (true);
                    case DetailLevelItemInformation.Levels.segment:
                        if (_customerWebSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.DB.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG)) return (true);
                        return (false);
                    #endregion

                    #region Groupe de société
                    case DetailLevelItemInformation.Levels.holdingCompany:

                        if (
                            // Droit sur les groupe de société
                            _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY) &&
                            // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                            (_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISING_AGENCY, AccessType.includes) ||
                            _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_ADVERTISING_AGENCY, AccessType.includes) 
                            ) &&
                            // Pas de famille, classe
                            !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                            !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                            )
                            return (true);
                        return (false);
                    #endregion

                    #region Agences et groupe d'agence
                    case DetailLevelItemInformation.Levels.groupMediaAgency:
                    case DetailLevelItemInformation.Levels.agency:
                        vehicleList = GetVehicles();
                        if (
                            // Droit sur les agences media
                             _customerWebSession.CustomerLogin.CustomerMediaAgencyFlagAccess(vehicleList)
                            ) return (true);
                        return (false);
                    #endregion

                    #region Version
                    case DetailLevelItemInformation.Levels.slogan:
                        return (false);

                    #endregion

                    default:
                        return (true);
                }

				case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
				switch(currentDetailLevelItem.Id){

						#region Annonceur 
					case DetailLevelItemInformation.Levels.advertiser:						

						if (
							// Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
							(_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT,AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND,AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) 							
							) &&
							// Pas de famille, classe
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes) 														
							)
							return (true);
						return (false);

						#endregion

						#region product
					case DetailLevelItemInformation.Levels.product:

						if (
							// Products level rights
							_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG) &&
							// Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
							(_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes)
							) &&
							// Pas de famille, classe
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
							)
							return (true);
						return (false);

					#endregion

						#region Marques
					case DetailLevelItemInformation.Levels.brand:						

						if (
							// Droit des Marques
							_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE) &&
							// Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
							(_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes)
							) &&
							// Pas de famille, classe
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
							)
							return (true);
						return (false);

						#endregion

						#region Famille, classe, groupe
					case DetailLevelItemInformation.Levels.sector:
					case DetailLevelItemInformation.Levels.subSector:	
					case DetailLevelItemInformation.Levels.group:				
						return(true);					
						#endregion

						#region Groupe de société
					case DetailLevelItemInformation.Levels.holdingCompany:						

						if (
							// Droit sur les groupe de société
							_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY) &&
							// Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
							(_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes)
							) &&
							// Pas de famille, classe
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
							)
							return (true);
						return (false);
						#endregion					

						#region Variété
						case DetailLevelItemInformation.Levels.segment:
						return(false);
						#endregion

						#region Agences et groupe d'agence
					case DetailLevelItemInformation.Levels.groupMediaAgency:
					case DetailLevelItemInformation.Levels.agency:						
						return(false);
						#endregion

						#region Version
					case DetailLevelItemInformation.Levels.slogan:						

						if (
							currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.slogan && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG) &&
							// Sélection par produit ou marque ou annonceur ou Groupe de sociétés
							(_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||							
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
							_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes)
							) &&
							// Pas de famille, classe, groupe, groupe d'annonceur 							
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes) && 
							!_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes)  
							)
							return (true);
						return (false);

						#endregion

					default:
						return(true);
				}
				case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
				switch(currentDetailLevelItem.Id){

                        #region Emissions, Genres d'émissions,formes de parrainage,support
                        case DetailLevelItemInformation.Levels.program:
                        case DetailLevelItemInformation.Levels.programType:
                        case DetailLevelItemInformation.Levels.sponsorshipForm:
                        case DetailLevelItemInformation.Levels.media:
                            return (true);
                        #endregion
    						
						#region famille, classe, groupe, Annonceur, produit
					case DetailLevelItemInformation.Levels.sector:
					case DetailLevelItemInformation.Levels.subSector:
					case DetailLevelItemInformation.Levels.group:
					case DetailLevelItemInformation.Levels.advertiser:						
						return(true);
						#endregion

						#region Products
					case DetailLevelItemInformation.Levels.product:
						// Product level rights
						if (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
							return (true);
						return (false);
					#endregion

						#region Marques
					case DetailLevelItemInformation.Levels.brand:
						// Droit des Marques
						if(_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE))
							return(true);
						return(false);
						#endregion

						#region Version
					case DetailLevelItemInformation.Levels.slogan:
						if(
							currentDetailLevelItem.Id==DetailLevelItemInformation.Levels.slogan &&_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG)														
							)
							return(true);
						return(false);
						#endregion

						#region Groupe de société
					case DetailLevelItemInformation.Levels.holdingCompany:
						if(							
							// Droit sur les groupe de société
							_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY) 							
							)return(true);
						return(false);
						#endregion												

					default:
						return(false);
				}
			default:
				return(true);
			}
		}

		/// <summary>
		/// Vérifie si le client à le droit de voir un détail produit dans les plan media
		/// </summary>
		/// <remarks>
		/// AdNetTrack selection [Ok]
		/// </remarks>
		/// <returns>True si oui false sinon</returns>
		protected bool CheckProductDetailLevelAccess(){
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


