#region Informations
// Auteur: Y. R'kaina 
// Date de création: 15/01/2007
// Date de modification:
#endregion

using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using TNS.AdExpress.Web.Core.Sessions;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.FrameWork.Results;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebSystem=TNS.AdExpress.Web.BusinessFacade;

using Dundas.Charting.WebControl;
using WebResultUI=TNS.FrameWork.WebResultUI;

using AjaxPro;

using Oracle.DataAccess.Client;

namespace TNS.AdExpress.Web.Controls.Results.Appm
{
	/// <summary>
	/// Conteneur des composants destinés au Données de cadrage.
	/// </summary>
	[ToolboxData("<{0}:SectorDataContainerWebControl runat=server></{0}:SectorDataContainerWebControl>")]
	public class SectorDataContainerWebControl : System.Web.UI.WebControls.WebControl{

		#region Constantes
		/// <summary>
		/// le code texte du titre
		/// </summary>
		private const int ID_TITLE_TEXT = 2114;
		#endregion

		#region Variables
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession _customerWebSession=null;
		
		/// <summary>
		/// Source de données
		/// </summary>
        protected TNS.FrameWork.DB.Common.IDataSource _dataSource = null;

		/// <summary>
		/// Contrôle qui affiche les résultats de l'APPM sous forme de tableau html
		/// </summary>
		protected SectorDataWebControl sectorDataHtmlWebControl = null;

		/// <summary>
		/// Composant tableau de résultat
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Results.ResultWebControl resultwebcontrol;	

		/// <summary>
		/// Timeout des scripts utilisés par AjaxPro
		/// </summary>
		protected int _ajaxProTimeOut=60;

		/// <summary>
		/// Contrôle qui affiche les résultats de l'APPM sous forme graphique
		/// </summary>
		protected BaseAppmChartWebControl appmChartWebControl = null;

		/// <summary>
		/// Obtient ou définit le type de l'image
		/// </summary>		
		protected ChartImageType _imageType = ChartImageType.Jpeg;

        /// <summary>
        /// nom du skin id du controle graphique de l'analyse par famille de presse
        /// </summary>
        private string _analyseFamilyInterestPlanSkinID = string.Empty;
        /// <summary>
        /// nom du skin id du controle graphique des saisonalité
        /// </summary>
        private string _seasonalityPlanSkinID = string.Empty;
        /// <summary>
        /// nom du skin id du controle graphique de la periodicité
        /// </summary>
        private string _periodicityPlanSkinID = string.Empty;
		#endregion

		#region Propriétés
		/// <summary>
		/// Classe CSS de la ligne de niveau 1
		/// </summary>
		protected string _cssDetailSelectionL1="";
		/// <summary>
		/// Obtient ou définit la classe CSS de la ligne de niveau 1 du rappel de sélection
		/// </summary>
		[Bindable(true), 
		Category("Detail Selection"), 
		DefaultValue("")] 
		public string CssDetailSelectionL1{
			get{return _cssDetailSelectionL1;}
			set{_cssDetailSelectionL1 = value;	}
		}
		/// <summary>
		/// Classe CSS de la ligne de niveau 2
		/// </summary>
		protected string _cssDetailSelectionL2="";
		/// <summary>
		/// Obtient ou définit la classe CSS de la ligne de niveau 2 du rappel de sélection
		/// </summary>
		[Bindable(true), 
		Category("Detail Selection"), 
		DefaultValue("")] 
		public string CssDetailSelectionL2{
			get{return _cssDetailSelectionL2;}
			set{_cssDetailSelectionL2 = value;	}
		}
		/// <summary>
		/// Classe CSS de la ligne de niveau 3
		/// </summary>
		protected string _cssDetailSelectionL3="";
		/// <summary>
		/// Obtient ou définit la classe CSS de la ligne de niveau 3 du rappel de sélection
		/// </summary>
		[Bindable(true), 
		Category("Detail Selection"), 
		DefaultValue("")] 
		public string CssDetailSelectionL3{
			get{return _cssDetailSelectionL3;}
			set{_cssDetailSelectionL3 = value;	}
		}

		/// <summary>
		/// Classe CSS du titre global du tableau
		/// </summary>
		protected string _cssDetailSelectionTitleGlobal="";
		/// <summary>
		/// Obtient ou définit la classe CSS du titre global du tableau
		/// </summary>
		/// <example>Exemple de titre : Paramètre du tableau</example>
		[Bindable(true), 
		Category("Detail Selection"), 
		DefaultValue("")] 
		public string CssDetailSelectionTitleGlobal{
			get{return _cssDetailSelectionTitleGlobal;}
			set{_cssDetailSelectionTitleGlobal = value;	}
		}

		/// <summary>
		/// Classe CSS d'un titre
		/// </summary>
		protected string _cssDetailSelectionTitle="";
		/// <summary>
		/// Obtient ou définit la classe CSS d'un titre
		/// </summary>
		[Bindable(true), 
		Category("Detail Selection"), 
		DefaultValue("")] 
		public string CssDetailSelectionTitle{
			get{return _cssDetailSelectionTitle;}
			set{_cssDetailSelectionTitle = value;	}
		}
		/// <summary>
		/// Classe CSS d'une donnée d'un titre
		/// </summary>
		protected string _cssDetailSelectionTitleData="";
		/// <summary>
		/// Obtient ou définit la classe CSS d'une donnée d'un titre
		/// </summary>
		[Bindable(true), 
		Category("Detail Selection"), 
		DefaultValue("")] 
		public string CssDetailSelectionTitleData{
			get{return _cssDetailSelectionTitleData;}
			set{_cssDetailSelectionTitleData = value;	}
		}
		/// <summary>
		/// Classe CSS d'une bordure
		/// </summary>
		protected string _cssDetailSelectionBordelLevel="";
		/// <summary>
		/// Obtient ou définit la classe CSS d'une donnée d'un titre
		/// </summary>
		[Bindable(true), 
		Category("Detail Selection"), 
		DefaultValue("")] 
		public string CssDetailSelectionBordelLevel{
			get{return _cssDetailSelectionBordelLevel;}
			set{_cssDetailSelectionBordelLevel = value;	}
		}
		/// <summary>
		/// code du titre à afficher 
		/// </summary>
		protected Int64 _idTitleText;
		/// <summary>
		/// Get / Set le code du titre à afficher
		/// </summary>
		[Bindable(true),
		Category("Id Title Text"),
		DefaultValue(0)]
		public Int64 IdTitleText{
			get{return(_idTitleText);}
			set{_idTitleText=value;}
		}
		
		#region Ajax
		/// <summary>
		/// Obtient ou définit le Timeout des scripts utilisés par AjaxPro
		/// </summary>
		[Bindable(true), 
		Category("Ajax"),
		Description("Timeout des scripts utilisés par AjaxPro"),
		DefaultValue("60")]
		public int AjaxProTimeOut{
			get{return _ajaxProTimeOut;}
			set{_ajaxProTimeOut=value;}
		}
		#endregion

		#region RenderType
		/// <summary>
		/// Type de rendu
		/// </summary>
		protected WebResultUI.RenderType _renderType=WebResultUI.RenderType.html; 
		/// <summary>
		/// Type de rendu
		/// </summary>
		[Bindable(true), 
		Category("Render Type"), 
		DefaultValue("RenderType.html"),
		Description("Type de rendu")] 
		public WebResultUI.RenderType OutputType{
			get{return _renderType;}
			set{_renderType = value;}
		}
		#endregion

		#region Cell
		/// <summary>
		/// Css des cellules
		/// </summary>
		private string _cssCell=""; 
		/// <summary>
		/// Get / Set Css des cellules
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("")] 
		public string CssCell{
			get{return _cssCell;}
			set{_cssCell = value;}
		}
		#endregion

		#region Level 1
		/// <summary>
		/// Classe CSS de la ligne de niveau 1
		/// </summary>
		protected string _cssL1="";
		/// <summary>
		/// Obtient ou définit la classe CSS de la ligne de niveau 1
		/// </summary>
		[Bindable(true), 
		Category("Level1"), 
		DefaultValue("")] 
		public string CssL1{
			get{return _cssL1;}
			set{_cssL1 = value;	}
		}

		/// <summary>
		/// Code html spécifique
		/// </summary>
		protected string _htmlCodeL1="";
		/// <summary>
		/// Obtient ou définit le code html spécifique
		/// </summary>
		[Bindable(true), 
		Category("Level1"), 
		DefaultValue("")] 
		public string HtmlCodeL1{
			get{return _htmlCodeL1;}
			set{_htmlCodeL1 = value;}
		}

		/// <summary>
		/// Couleur de fond de la ligne
		/// </summary>
		protected string _backgroudColorL1="";
		/// <summary>
		/// Obtient ou définit Couleur de fond de la ligne
		/// </summary>
		[Bindable(true), 
		Category("Level1"), 
		DefaultValue("")] 
		public string BackgroudColorL1{
			get{return _backgroudColorL1;}
			set{_backgroudColorL1 = value;}
		}

        /// <summary>
		/// Couleur de la bordure droite de selection de la ligne
		/// </summary>
		protected string _cssDetailSelectionRightBorderL1="";
		/// <summary>
		/// Obtient ou définit Couleur de la bordure droite de selection de la ligne
		/// </summary>
		[Bindable(true), 
		Category("Level1"), 
		DefaultValue("")] 
		public string CssDetailSelectionRightBorderL1{
			get{return _backgroudColorL1;}
			set{_backgroudColorL1 = value;}
		}

        /// <summary>
		/// Couleur de la bordure du bas de selection de la ligne
		/// </summary>
		protected string _cssDetailSelectionRightBottomBorderL1="";
		/// <summary>
		/// Obtient ou définit Couleur de la bordure du bas de selection de la ligne
		/// </summary>
		[Bindable(true), 
		Category("Level1"), 
		DefaultValue("")] 
		public string CssDetailSelectionRightBottomBorderL1{
			get{return _cssDetailSelectionRightBottomBorderL1;}
			set{_cssDetailSelectionRightBottomBorderL1 = value;}
		}
		#endregion

		#region Level 2
		/// <summary>
		/// Classe CSS de la ligne de niveau 2
		/// </summary>
		protected string _cssL2="";
		/// <summary>
		/// Obtient ou définit la classe CSS de la ligne de niveau 2
		/// </summary>
		[Bindable(true), 
		Category("Level2"), 
		DefaultValue("")] 
		public string CssL2{
			get{return _cssL2;}
			set{_cssL2 = value;	}
		}

		/// <summary>
		/// Code html spécifique
		/// </summary>
		protected string _htmlCodeL2="";
		/// <summary>
		/// Obtient ou définit le code html spécifique
		/// </summary>
		[Bindable(true), 
		Category("Level2"), 
		DefaultValue("")] 
		public string HtmlCodeL2{
			get{return _htmlCodeL2;}
			set{_htmlCodeL2 = value;}
		}

		/// <summary>
		/// Couleur de fond de la ligne
		/// </summary>
		protected string _backgroudColorL2="";
		/// <summary>
		/// Obtient ou définit Couleur de fond de la ligne
		/// </summary>
		[Bindable(true), 
		Category("Level2"), 
		DefaultValue("")] 
		public string BackgroudColorL2{
			get{return _backgroudColorL2;}
			set{_backgroudColorL2 = value;}
		}

        /// <summary>
        /// Couleur de la bordure droite de selection de la ligne
        /// </summary>
        protected string _cssDetailSelectionRightBorderL2 = "";
        /// <summary>
        /// Obtient ou définit Couleur de la bordure droite de selection de la ligne
        /// </summary>
        [Bindable(true),
        Category("Level2"),
        DefaultValue("")]
        public string CssDetailSelectionRightBorderL2 {
            get { return _backgroudColorL2; }
            set { _backgroudColorL2 = value; }
        }

        /// <summary>
        /// Couleur de la bordure du bas de selection de la ligne
        /// </summary>
        protected string _cssDetailSelectionRightBottomBorderL2 = "";
        /// <summary>
        /// Obtient ou définit Couleur de la bordure du bas de selection de la ligne
        /// </summary>
        [Bindable(true),
        Category("Level2"),
        DefaultValue("")]
        public string CssDetailSelectionRightBottomBorderL2 {
            get { return _cssDetailSelectionRightBottomBorderL2; }
            set { _cssDetailSelectionRightBottomBorderL2 = value; }
        }
		#endregion

		#region Level 3
		/// <summary>
		/// Classe CSS de la ligne de niveau 3
		/// </summary>
		protected string _cssL3="";
		/// <summary>
		/// Obtient ou définit la classe CSS de la ligne de niveau 3
		/// </summary>
		[Bindable(true), 
		Category("Level3"), 
		DefaultValue("")] 
		public string CssL3{
			get{return _cssL3;}
			set{_cssL3 = value;	}
		}

		/// <summary>
		/// Code html spécifique
		/// </summary>
		protected string _htmlCodeL3="";
		/// <summary>
		/// Obtient ou définit le code html spécifique
		/// </summary>
		[Bindable(true), 
		Category("Level3"), 
		DefaultValue("")] 
		public string HtmlCodeL3{
			get{return _htmlCodeL3;}
			set{_htmlCodeL3 = value;}
		}

		/// <summary>
		/// Couleur de fond de la ligne
		/// </summary>
		protected string _backgroudColorL3="";
		/// <summary>
		/// Obtient ou définit Couleur de fond de la ligne
		/// </summary>
		[Bindable(true), 
		Category("Level3"), 
		DefaultValue("")] 
		public string BackgroudColorL3{
			get{return _backgroudColorL3;}
			set{_backgroudColorL3 = value;}
		}

        /// <summary>
        /// Couleur de la bordure droite de selection de la ligne
        /// </summary>
        protected string _cssDetailSelectionRightBorderL3 = "";
        /// <summary>
        /// Obtient ou définit Couleur de la bordure droite de selection de la ligne
        /// </summary>
        [Bindable(true),
        Category("Level3"),
        DefaultValue("")]
        public string CssDetailSelectionRightBorderL3 {
            get { return _backgroudColorL3; }
            set { _backgroudColorL3 = value; }
        }

        /// <summary>
        /// Couleur de la bordure du bas de selection de la ligne
        /// </summary>
        protected string _cssDetailSelectionRightBottomBorderL3 = "";
        /// <summary>
        /// Obtient ou définit Couleur de la bordure du bas de selection de la ligne
        /// </summary>
        [Bindable(true),
        Category("Level3"),
        DefaultValue("")]
        public string CssDetailSelectionRightBottomBorderL3 {
            get { return _cssDetailSelectionRightBottomBorderL3; }
            set { _cssDetailSelectionRightBottomBorderL3 = value; }
        }
		#endregion

		#region Level Total
		/// <summary>
		/// Ligne de niveau Total
		/// </summary>
		protected string _cssLTotal="";
		/// <summary>
		/// Get / Set Css de la ligne de niveau Total
		/// </summary>
		[Bindable(true), 
		Category("Total"), 
		DefaultValue("")] 
		public string CssLTotal{
			get{return _cssLTotal;}
			set{_cssLTotal = value;}
		}
		
		/// <summary>
		/// Code html spécifique
		/// </summary>
		protected string _htmlCodeLTotal="";
		/// <summary>
		/// Get / Set code html spécifique pour la ligne total
		/// </summary>
		[Bindable(true), 
		Category("Total"), 
		DefaultValue("")] 
		public string HtmlCodeLTotal{
			get{return _htmlCodeLTotal;}
			set{_htmlCodeLTotal = value;}
		}
		#endregion

		#region Level Affinities
		/// <summary>
		/// Ligne de niveau Total
		/// </summary>
		protected string _cssLAffinities="";
		/// <summary>
		/// Get / Set Css de la ligne de niveau Affinities
		/// </summary>
		[Bindable(true), 
		Category("Css Affinities"), 
		DefaultValue("")] 
		public string CssLAffinities{
			get{return _cssLAffinities;}
			set{_cssLAffinities = value;}
		}
		
		/// <summary>
		/// Code html spécifique
		/// </summary>
		protected string _htmlCodeLAffinities="";
		/// <summary>
		/// Get / Set code html spécifique pour la ligne Affinities
		/// </summary>
		[Bindable(true), 
		Category("Css Affinities"), 
		DefaultValue("")] 
		public string HtmlCodeLAffinities{
			get{return _htmlCodeLAffinities;}
			set{_htmlCodeLAffinities = value;}
		}
		#endregion

		#region Level Header
		/// <summary>
		/// Ligne de niveau Header
		/// </summary>
		protected string _cssLHeader="";
		/// <summary>
		/// Obtient ou définit le CSS de la ligne de niveau Header
		/// </summary>
		[Bindable(true), 
		Category("Header"), 
		DefaultValue("")] 
		public string CssLHeader{
			get{return _cssLHeader;}
			set{_cssLHeader = value;}
		}

		/// <summary>
		/// Code html spécifique
		/// </summary>
		protected string _htmlCodeLHeader="";
		/// <summary>
		/// Code html spécifique
		/// </summary>
		[Bindable(true), 
		Category("Header"), 
		DefaultValue("")] 
		public string HtmlCodeLHeader{
			get{return _htmlCodeLHeader;}
			set{_htmlCodeLHeader = value;}
		}
		#endregion

		#region Title
		/// <summary>
		/// Style du titre
		/// </summary>
		protected string _cssTitle="";
		/// <summary>
		/// Get / Set le style du titre
		/// </summary>
		[Bindable(true),
		Category("CssTitle"),
		DefaultValue("")]
		public string CssTitle{
			get{return _cssTitle;}
			set{_cssTitle = value;}
		}
		#endregion

		#region javascript filpath
		/// <summary>
		/// Chemin du fichier contenant les javascripts du contrôle
		/// </summary>
		protected string _javascriptFilePath="";
		/// <summary>
		/// La table de résultat
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("")] 
		public string JavascriptFilePath{
			get{return(_javascriptFilePath);}
			set{_javascriptFilePath = value;}
		}
		#endregion

		#region pagination
		/// <summary>
		/// Indique si la pagination est autorisée
		/// </summary>
		protected bool _allowPaging = false;
		/// <summary>
		/// Indique si la pagination est autorisée
		/// </summary>
		[Bindable(true),
		Category("ResultWebControl"),
		DefaultValue("false")] 
		public bool AllowPaging{
			get {return _allowPaging;}
			set {_allowPaging = value;}
		}
		#endregion

		#endregion
		
		#region Accesseurs
		/// <summary>
		/// Obtient ou définit la source de données
		/// </summary>
        public TNS.FrameWork.DB.Common.IDataSource Source
		{
			set{_dataSource = value;}
			get{return _dataSource;}
		}
		
		/// <summary>
		/// Obtient ou définit la Session du client
		/// </summary>
		public WebSession CustomerWebSession{
			set{_customerWebSession = value;}
			get{return _customerWebSession;}
		}

		/// <summary>
		/// Obtient ou définit le type de l'image pour le résultat sous forme graphique de l'APPM
		/// </summary>
		public ChartImageType ImageType{
			set{_imageType = value;}
			get{return _imageType;}
		}
        /// <summary>
        /// Obtient ou définit le nom du skin id du controle graphique de l'analyse par famille de presse
        /// </summary>
        public string AnalyseFamilyInterestPlanSkinID {
            set { _analyseFamilyInterestPlanSkinID = value; }
            get { return _analyseFamilyInterestPlanSkinID; }
        }
        /// <summary>
        /// Obtient ou définit le nom du skin id du controle graphique des saisonnalité
        /// </summary>
        public string SeasonalityPlanSkinID {
            set { _seasonalityPlanSkinID = value; }
            get { return _seasonalityPlanSkinID; }
        }
        /// <summary>
        /// Obtient ou définit le nom du skin id du controle graphique de la periodicité
        /// </summary>
        public string PeriodicityPlanSkinID {
            set { _periodicityPlanSkinID = value; }
            get { return _periodicityPlanSkinID; }
        }
		#endregion

		#region Evènements

		#region Initialisation
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e){
			base.OnInit(e);
		}
		#endregion

		#region Load
		/// <summary>
		/// Chargement des composants
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnLoad(EventArgs e){
			Controls.Clear();

			bool isGraph=false;

			#region Chargement du Contrôle qui affiche les résultats sous forme de Tableaux
			
			if(_customerWebSession!=null){

				switch (_customerWebSession.CurrentTab){

					case APPM.sectorDataSynthesis:	
						resultwebcontrol = GetResultWebControl();
						resultwebcontrol.CssL1=CssL1;
						resultwebcontrol.CssL2=CssL2;
						resultwebcontrol.CssTitle=CssTitle;
						resultwebcontrol.IdTitleText=ID_TITLE_TEXT;
						Controls.Add(resultwebcontrol);
						break;
					case APPM.sectorDataAverage:	
						resultwebcontrol = GetResultWebControl();
						resultwebcontrol.CssL1=CssL1;
						resultwebcontrol.CssL2=CssL2;
						resultwebcontrol.CssL3=CssL3;
						resultwebcontrol.CssLHeader=CssLHeader;
						Controls.Add(resultwebcontrol);
						break;
					case APPM.sectorDataAffinities:	
						resultwebcontrol = GetResultWebControl();
						resultwebcontrol.CssL1=CssLAffinities;
						resultwebcontrol.CssLTotal=CssLTotal;
						resultwebcontrol.CssLHeader=CssLHeader;
						Controls.Add(resultwebcontrol);
						break;
					case APPM.sectorDataSeasonality:	
						if(OutputType==WebResultUI.RenderType.excel){
							resultwebcontrol = GetResultWebControl();
							resultwebcontrol.CssL1=CssL1;
							resultwebcontrol.CssL2=CssL2;
							resultwebcontrol.CssLTotal=CssLTotal;
							resultwebcontrol.CssLHeader=CssLHeader;
							Controls.Add(resultwebcontrol);
						}
						else
							isGraph=true;
						break;
					case APPM.sectorDataInterestFamily:	
						if(OutputType==WebResultUI.RenderType.excel){
							resultwebcontrol = GetResultWebControl();
							resultwebcontrol.CssL1=CssL1;
							resultwebcontrol.CssL2=CssL2;
							resultwebcontrol.CssLTotal=CssLTotal;
							resultwebcontrol.CssLHeader=CssLHeader;
							Controls.Add(resultwebcontrol);
						}
						else
							isGraph=true;
						break;
					case APPM.sectorDataPeriodicity:	
						if(OutputType==WebResultUI.RenderType.excel){
							resultwebcontrol = GetResultWebControl();
							resultwebcontrol.CssL1=CssL1;
							resultwebcontrol.CssL2=CssL2;
							resultwebcontrol.CssLTotal=CssLTotal;
							resultwebcontrol.CssLHeader=CssLHeader;
							Controls.Add(resultwebcontrol);
						}
						else
							isGraph=true;
						break;
					default:
						isGraph=true;
						break;
				}			
			}
			
			#endregion

			if(isGraph){

				#region Chargement du Contrôle qui affiche les résultats sous forme de Graphiques
				if(_customerWebSession!=null && _customerWebSession.Graphics && _customerWebSession.CurrentTab!=APPM.sectorDataSynthesis && _customerWebSession.CurrentTab!=APPM.sectorDataAverage){
					switch(_customerWebSession.CurrentTab){

						case APPM.sectorDataPeriodicity:
							appmChartWebControl = new PeriodicityPlanAppmChartWebControl(_customerWebSession,_dataSource,this._imageType,_periodicityPlanSkinID);
							break;
						case APPM.sectorDataInterestFamily:
							appmChartWebControl = new AnalyseFamilyInterestPlanAppmChartWebControl(_customerWebSession,_dataSource,this._imageType,_analyseFamilyInterestPlanSkinID);
							break;
						case APPM.sectorDataSeasonality:
							appmChartWebControl = new SeasonalityPlanSectorDataChartWebControl(_customerWebSession,_dataSource,this._imageType,_seasonalityPlanSkinID);
							break;
					}
					if(appmChartWebControl!=null){
						//appmChartWebControl.SetDesignMode();
						appmChartWebControl.EnableViewState=false;
						appmChartWebControl.ID="appmChartWebControl_"+this.ID;																			
						Controls.Add(appmChartWebControl);
					}
				}
				#endregion

			}
			
			base.OnLoad (e);
		}
		
		#endregion

		#region PréRender
		/// <summary>
		/// Prérendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e){
			base.OnPreRender (e);
		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
			if(_customerWebSession==null){
				output.Write("APPM Sector Data container");
			}
			base.Render(output);
		}
		#endregion

		#endregion

		#region Méthode privée
		private ResultWebControl GetResultWebControl(){
			ResultWebControl resultwebcontrol = new ResultWebControl();
			resultwebcontrol.CustomerWebSession = _customerWebSession;
			resultwebcontrol.ID="sectorDataHtmlWebControl_"+this.ID;
			resultwebcontrol.JavascriptFilePath=JavascriptFilePath;
			resultwebcontrol.AllowPaging=AllowPaging;
			resultwebcontrol.AjaxProTimeOut=AjaxProTimeOut;
			resultwebcontrol.OutputType=OutputType;
			resultwebcontrol.CssDetailSelectionBordelLevel=CssDetailSelectionBordelLevel;
			resultwebcontrol.CssDetailSelectionL1=CssDetailSelectionL1;
			resultwebcontrol.CssDetailSelectionL2 = CssDetailSelectionL2;

            resultwebcontrol.CssDetailSelectionRightBorderL1 = CssDetailSelectionRightBorderL1;
            resultwebcontrol.CssDetailSelectionRightBorderL2 = CssDetailSelectionRightBorderL2;
            resultwebcontrol.CssDetailSelectionRightBottomBorderL1 = CssDetailSelectionRightBottomBorderL1;
            resultwebcontrol.CssDetailSelectionRightBottomBorderL2 = CssDetailSelectionRightBottomBorderL2;
			resultwebcontrol.CssDetailSelectionTitle=CssDetailSelectionTitle;
			resultwebcontrol.CssDetailSelectionTitleData=CssDetailSelectionTitleData;
			resultwebcontrol.CssDetailSelectionTitleGlobal=CssDetailSelectionTitleGlobal;
			return resultwebcontrol;
		}
		#endregion

	}
}
