
using System;
using System.Text;
using System.Web;
using System.Collections;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;

using WebBusinessFacade=TNS.AdExpress.Web.BusinessFacade;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes=TNS.AdExpress.Constantes.Web;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;

using Oracle.DataAccess.Client;

using AjaxPro;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;

using LostWon = TNS.AdExpressI.LostWon;
using PresentAbsent = TNS.AdExpressI.PresentAbsent;
using Portofolio=TNS.AdExpressI.Portofolio;
using Domain=TNS.AdExpress.Domain.Web.Navigation;
using System.Reflection;

namespace TNS.AdExpress.Web.Controls.Results{
	/// <summary>
	/// Description résumée de ResultWebControl.
	/// </summary>
	[ToolboxData("<{0}:ResultWebControl runat=server></{0}:ResultWebControl>")]
	public class ResultWebControl : WebControlResultTable{

		#region Variables
		/// <summary>
		/// Timeout des scripts utilisés par AjaxPro
		/// </summary>
		protected int _ajaxProTimeOut=60;
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession _customerWebSession=null;
		/// <summary>
		/// Rappel de la sélection
		/// </summary>
		TNS.AdExpress.Web.Controls.Selections.DetailSelectionWebControl detailSelectionWebControl=null;
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
        /// Right border css class for level 1  
        /// </summary>
        protected string _cssDetailSelectionRightBorderL1 = "";
        /// <summary>
        /// Get or set Right border css class for level 1  
        /// </summary>
        [Bindable(true),
        Category("Detail Selection"),
        DefaultValue("")]
        public string CssDetailSelectionRightBorderL1 {
            get { return _cssDetailSelectionRightBorderL1; }
            set { _cssDetailSelectionRightBorderL1 = value; }
        }
        /// <summary>
        /// Right border css class for level 2
        /// </summary>
        protected string _cssDetailSelectionRightBorderL2 = "";
        /// <summary>
        /// Get or set Right border css class for level 2
        /// </summary>
        [Bindable(true),
        Category("Detail Selection"),
        DefaultValue("")]
        public string CssDetailSelectionRightBorderL2 {
            get { return _cssDetailSelectionRightBorderL2; }
            set { _cssDetailSelectionRightBorderL2 = value; }
        }
        /// <summary>
        /// Right border css class for level 3
        /// </summary>
        protected string _cssDetailSelectionRightBorderL3 = "";
        /// <summary>
        /// Get or set Right border css class for level 3
        /// </summary>
        [Bindable(true),
        Category("Detail Selection"),
        DefaultValue("")]
        public string CssDetailSelectionRightBorderL3 {
            get { return _cssDetailSelectionRightBorderL3; }
            set { _cssDetailSelectionRightBorderL3 = value; }
        }
        /// <summary>
        /// Right Bottom border css class for level 1  
        /// </summary>
        protected string _cssDetailSelectionRightBottomBorderL1 = "";
        /// <summary>
        /// Get or set Right Bottom border css class for level 1  
        /// </summary>
        [Bindable(true),
        Category("Detail Selection"),
        DefaultValue("")]
        public string CssDetailSelectionRightBottomBorderL1 {
            get { return _cssDetailSelectionRightBottomBorderL1; }
            set { _cssDetailSelectionRightBottomBorderL1 = value; }
        }
        /// <summary>
        /// Right Bottom border css class for level 2
        /// </summary>
        protected string _cssDetailSelectionRightBottomBorderL2 = "";
        /// <summary>
        /// Get or set Right Bottom border css class for level 2
        /// </summary>
        [Bindable(true),
        Category("Detail Selection"),
        DefaultValue("")]
        public string CssDetailSelectionRightBottomBorderL2 {
            get { return _cssDetailSelectionRightBottomBorderL2; }
            set { _cssDetailSelectionRightBottomBorderL2 = value; }
        }
        /// <summary>
        /// Right Bottom border css class for level 3
        /// </summary>
        protected string _cssDetailSelectionRightBottomBorderL3 = "";
        /// <summary>
        /// Get or set Right Bottom border css class for level 3
        /// </summary>
        [Bindable(true),
        Category("Detail Selection"),
        DefaultValue("")]
        public string CssDetailSelectionRightBottomBorderL3 {
            get { return _cssDetailSelectionRightBottomBorderL3; }
            set { _cssDetailSelectionRightBottomBorderL3 = value; }
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
		/// <summary>
		/// Get / Set Result title
		/// </summary>
		[Obsolete("Il ne faut pas utiliser cette propriété",true)]
		public new string Title{
			get{throw(new NotImplementedException());}
			set{throw(new NotImplementedException());}
		}
		
		#region Pagination
		/// <summary>
		/// Nombre par defaut de ligne dans une page
		/// </summary>
		private int _defaultPageSize = 100;
		/// <summary>
		/// Nombre par defaut de ligne dans une page
		/// </summary>
		[Bindable(true), 
		Category("Paging"), 
		DefaultValue(100),
		Description("Nombre par defaut de ligne dans une page")] 
		public int DefaultPageSize {
			get {return _defaultPageSize;}
			set {_defaultPageSize = value;}
		}

		/// <summary>
		/// Obtient  / définit les options de pagination
		/// </summary>
		private string _pageSizeOptions = "100,200,500,1000"; 
		/// <summary>
		/// Obtient  / définit les options de pagination
		/// </summary>
		[Bindable(true), 	
		Category("Paging"),
		DefaultValue("100")] 
		public string PageSizeOptions {
			get {return _pageSizeOptions;}
			set {_pageSizeOptions = value;}
		}

		/// <summary>
		/// Obtient  / définit le nombre maximale d'index de page à afficher
		/// </summary>
		private int _numberIndexPage = 5; 
		/// <summary>
		/// Obtient  / définit les options de pagination
		/// </summary>
		[Bindable(true), 	
		Category("Paging"),
		DefaultValue(5)] 
		public int NumberIndexPage {
			get {return _numberIndexPage;}
			set {_numberIndexPage = value;}
		}

		/// <summary>
		/// Indique si la pagination est autorisée
		/// </summary>
		protected bool _allowPaging = false;
		/// <summary>
		/// Indique si la pagination est autorisée
		/// </summary>
		[Bindable(true),
		Category("Paging"),
		DefaultValue("false")] 
		public bool AllowPaging {
			get {return _allowPaging;}
			set {_allowPaging = value;}
		}

		/// <summary>
		/// Obtient / définit la taille (nombre de lignes) d'une page 
		/// </summary>
		private int _pageSize = 0;
		/// <summary>
		///  Obtient / définit la taille (nombre de lignes) d'une page 
		/// </summary>
		[Bindable(true), 	
		Category("Paging"),
		DefaultValue(0)] 
		public int PageSize {
			get {return _pageSize;}
			set {_pageSize = value;}
		}
		#endregion

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

		#region Web Session
		/// <summary>
		/// Obtient ou définit la Sesion du client
		/// </summary>
		[Bindable(false)] 
		public WebSession CustomerWebSession {
			get{return(_customerWebSession);}
			set{_customerWebSession = value;}
		}
		#endregion

		#endregion

		#region Init
		/// <summary>
		/// Initialisation du composant
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
            DetailSelectionInit();
		}
        /// <summary>
        /// Initialisation du composant DetailSelectionWebControl
        /// </summary>
        protected virtual void DetailSelectionInit() {
            this._sSortKey = _customerWebSession.SortKey;
            this._sortOrder = _customerWebSession.Sorting;
            switch (_renderType) {
                case RenderType.rawExcel:
                case RenderType.excel:
                    detailSelectionWebControl = new TNS.AdExpress.Web.Controls.Selections.DetailSelectionWebControl();
                    break;
            }
        }
		#endregion

		#region OnAjaxMethodError
		/// <summary>
		/// Appelé sur erreur à l'exécution des méthodes Ajax
		/// </summary>
		/// <param name="errorException">Exception</param>
		/// <param name="customerSession">Session utilisateur</param>
		/// <returns>Message d'erreur</returns>
		protected string OnAjaxMethodError(Exception errorException,WebSession customerSession) {
			TNS.AdExpress.Web.Exceptions.CustomerWebException cwe=null;
			try{
				BaseException err=(BaseException)errorException;
				cwe=new TNS.AdExpress.Web.Exceptions.CustomerWebException(err.Message,err.GetHtmlDetail(),customerSession);
			}
			catch(System.Exception){
				try{
					cwe=new TNS.AdExpress.Web.Exceptions.CustomerWebException(errorException.Message,errorException.StackTrace,customerSession);
				}
				catch(System.Exception es){
					throw(es);
				}
			}
			cwe.SendMail();
			return GetMessageError(customerSession,1973);
//			if(customerSession!=null)
//				return(GestionWeb.GetWebWord(1973,customerSession.SiteLanguage));
//			else
//				return(GestionWeb.GetWebWord(1973,33));
		}
		#endregion

		#region Script

		#region script AjaxProTimeOutScript
		/// <summary>
		/// Génère le code JavaSript pour ajuster le time out d'AjaxPro
		/// </summary>
		/// <returns>Code JavaScript</returns>
		private string AjaxProTimeOutScript(){
			StringBuilder js=new StringBuilder(100);
			js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
			js.Append("\r\nAjaxPro.timeoutPeriod="+_ajaxProTimeOut.ToString()+"*1000;"); 
			js.Append("\r\n-->\r\n</SCRIPT>");
			return(js.ToString());
		}
		#endregion

        #region Enregistrement des paramètres de construction du résultat
        /// <summary>
        /// Génération du JavaScript pour les paramètres du résultat
        /// </summary>
        /// <returns>Script</returns>
        private string ResultParametersScript(){
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
            js.Append("\r\n\t var resultParameters = new Object();");
            js.Append(SetResultParametersScript());
            js.Append("\r\n\t SetResultParameters(resultParameters);");
            js.Append("\r\n-->\r\n</SCRIPT>");
            return (js.ToString());
        }

        /// <summary>
        /// Génération du JavaScript pour définir les paramètres de résultat
        /// </summary>
        /// <remarks>
        /// Pour surcharge lors de l'heritage, il faut générer une fonction JavaScript comme ci-dessous:
        /// <code>
        /// function SetResultParameters(obj){
        /// ...
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        protected virtual string SetResultParametersScript(){
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n\tfunction SetResultParameters(obj){");
            js.Append("\r\n\t}");
            return (js.ToString());
        }
        #endregion

        #region Enregistrement des paramètres pour les styles
        /// <summary>
        /// Génération du JavaScript des paramètres pour les styles
        /// </summary>
        /// <returns>Script</returns>
        private string StyleParametersScript(){
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
            js.Append("\r\n\t var styleParameters = new Object();");
            js.Append(SetStyleParametersScript());
            js.Append("\r\n\t SetCssStyles(styleParameters);");
            js.Append("\r\n-->\r\n</SCRIPT>");
            return (js.ToString());
        }

        /// <summary>
        /// Génération du JavaScript pour définir les paramètres des styles
        /// </summary>
        /// <remarks>
        /// Pour surcharge lors de l'heritage, il faut générer une fonction JavaScript comme ci-dessous:
        /// <code>
        /// function SetCssStyles(obj){
        /// ...
        /// }
        /// </code>
        /// </remarks>
        /// <returns>Script</returns>
        protected virtual string SetStyleParametersScript(){
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n\nfunction SetCssStyles(obj){");
            js.Append("\r\n\t obj.CssDetailSelectionL1 = '" + CssDetailSelectionL1 + "';");
            js.Append("\r\n\t obj.CssDetailSelectionL2 = '" + CssDetailSelectionL2 + "';");
            js.Append("\r\n\t obj.CssDetailSelectionL3 = '" + CssDetailSelectionL3 + "';");
            js.Append("\r\n\t obj.CssDetailSelectionRightBorderL1 = '" + CssDetailSelectionRightBorderL1 + "';");
            js.Append("\r\n\t obj.CssDetailSelectionRightBorderL2 = '" + CssDetailSelectionRightBorderL2 + "';");
            js.Append("\r\n\t obj.CssDetailSelectionRightBorderL3 = '" + CssDetailSelectionRightBorderL3 + "';");
            js.Append("\r\n\t obj.CssDetailSelectionRightBottomBorderL1 = '" + CssDetailSelectionRightBottomBorderL1 + "';");
            js.Append("\r\n\t obj.CssDetailSelectionRightBottomBorderL2 = '" + CssDetailSelectionRightBottomBorderL2 + "';");
            js.Append("\r\n\t obj.CssDetailSelectionRightBottomBorderL3 = '" + CssDetailSelectionRightBottomBorderL3 + "';");
            js.Append("\r\n\t obj.CssDetailSelectionTitleGlobal = '" + CssDetailSelectionTitleGlobal + "';");
            js.Append("\r\n\t obj.CssDetailSelectionTitle = '" + CssDetailSelectionTitle + "';");
            js.Append("\r\n\t obj.CssDetailSelectionTitleData = '" + CssDetailSelectionTitleData + "';");
            js.Append("\r\n\t obj.CssDetailSelectionBordelLevel = '" + CssDetailSelectionBordelLevel + "';");
            js.Append("\r\n\t obj.CssLHeader = '" + CssLHeader + "';");
            js.Append("\r\n\t obj.CssL1 = '" + CssL1 + "';");
            js.Append("\r\n\t obj.BackgroudColorL1 = '" + BackgroudColorL1 + "';");
            js.Append("\r\n\t obj.HighlightBackgroundColorL1 = '" + HighlightBackgroundColorL1 + "';");
            js.Append("\r\n\t obj.HtmlCodeL1 = '" + _htmlCodeL1 + "';");
            js.Append("\r\n\t obj.CssL2 = '" + CssL2 + "';");
            js.Append("\r\n\t obj.BackgroudColorL2 = '" + BackgroudColorL2 + "';");
            js.Append("\r\n\t obj.HighlightBackgroundColorL2 = '" + HighlightBackgroundColorL2 + "';");
            js.Append("\r\n\t obj.HtmlCodeL2 = '" + _htmlCodeL2 + "';");
            js.Append("\r\n\t obj.CssL3 = '" + CssL3 + "';");
            js.Append("\r\n\t obj.BackgroudColorL3 = '" + BackgroudColorL3 + "';");
            js.Append("\r\n\t obj.HighlightBackgroundColorL3 = '" + HighlightBackgroundColorL3 + "';");
            js.Append("\r\n\t obj.HtmlCodeL3 = '" + _htmlCodeL3 + "';");
            js.Append("\r\n\t obj.CssL4 = '" + CssL4 + "';");
            js.Append("\r\n\t obj.BackgroudColorL4 = '" + BackgroudColorL4 + "';");
            js.Append("\r\n\t obj.HighlightBackgroundColorL4 = '" + HighlightBackgroundColorL4 + "';");
            js.Append("\r\n\t obj.HtmlCodeL4 = '" + _htmlCodeL4 + "';");
            js.Append("\r\n\t obj.CssLTotal = '" + CssLTotal + "';");
            js.Append("\r\n\t obj.ImgBtnCroiOverPath = '" + this.ImgBtnCroiOverPath + "';");
            js.Append("\r\n\t obj.ImgBtnCroiPath = '" + this.ImgBtnCroiPath + "';");
            js.Append("\r\n\t obj.ImgBtnDeCroiOverPath = '" + this.ImgBtnDeCroiOverPath + "';");
            js.Append("\r\n\t obj.ImgBtnDeCroiPath = '" + this.ImgBtnDeCroiPath + "';");
            js.Append("\r\n\t obj.CssTitle = '" + this._cssTitle + "';");
            js.Append("\r\n\t obj.IdTitleText = '" + this._idTitleText + "';");
            js.Append("\r\n }");
            return (js.ToString());
        }
        #endregion

        #region Enregistrement des paramètres pour les tris
        /// <summary>
        /// Génération du JavaScript des paramètres pour les tris
        /// </summary>
        /// <returns>Script</returns>
        private string SortParametersScript(){
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
            js.Append("\r\n\t var sortParameters = new Object();");
            js.Append(SetSortParametersScript());
            js.Append("\r\n\t SetSortParameters(sortParameters);");
            js.Append("\r\n-->\r\n</SCRIPT>");
            return (js.ToString());
        }

        /// <summary>
        /// Génération du JavaScript pour définir les paramètres de tri
        /// </summary>
        /// <remarks>
        /// Pour surcharge lors de l'heritage, il faut générer une fonction JavaScript comme ci-dessous:
        /// <code>
        /// function SetSortParameters(obj){
        /// ...
        /// }
        /// </code>
        /// </remarks>
        /// <returns>JavaScript</returns>
        protected virtual string SetSortParametersScript(){
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n\nfunction SetSortParameters(obj){");
            js.Append("\r\n\t obj.Id = '" + this.ID + "';");
            js.Append("\r\n\t obj.Sort = '" + this._sortOrder.GetHashCode() + "';");
            js.Append("\r\n\t obj.Key = '" + this._sSortKey + "';");
            js.Append("\r\n }");
            return (js.ToString());
        }
        #endregion

        #region AjaxEventScript
        /// <summary>
		/// Génération des fonctions javascript nécessaires au bon fonctionnement du control
		/// </summary>
		/// <returns>Code Javascript</returns>
		protected  string AjaxEventScript(){
			StringBuilder js=new StringBuilder(3000);
            string themeName = WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name;
            //Enregistrement des styles
            js.Append(StyleParametersScript());
            //Enregistrement des tris
            js.Append(SortParametersScript());
            //Enregistrement des paramètres du résultat
            js.Append(ResultParametersScript());

            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
			
			#region Variables Globales
			js.Append("\r\n var html;");
			js.Append("\r\n var globalTable,tabIndex,tab;");
			js.Append("\r\n var oN;");	
			js.Append("\r\n var pageCount = 0, pageSize = 0; minPageSize =0;");
			js.Append("\r\n var numberIndexPage = 0;");
			js.Append("\r\n var currentPageIndex  = 1, leftPageIndex  = 0, rightPageIndex  = 0;");
			js.Append("\r\n var tempIndex;");			
			js.Append("\r\n var isShowParenHeader=false,  withHeader = true;");
			js.Append("\r\n var pageSizeOptionsList='100,200,500,1000';");         
            
		
            
            js.Append("\r\n\n var detailSelectionHtml=null;"); 

			js.AppendFormat("\r\n\t {0}_img_last_out = new Image(); {0}_img_last_out.src ='{1}';\r\n{0}_img_last_in = new Image(); {0}_img_last_in.src ='{2}';\r\n{0}_img_first_out = new Image(); {0}_img_first_out.src ='{3}';\r\n{0}_img_first_in = new Image(); {0}_img_first_in.src ='{4}';\r\n"
				, this.ID
                , "/App_Themes/" + themeName + "/Images/Common/Result/bt_last_up.gif"
                , "/App_Themes/" + themeName + "/Images/Common/Result/bt_last_down.gif"
                , "/App_Themes/" + themeName + "/Images/Common/Result/bt_first_up.gif"
                , "/App_Themes/" + themeName + "/Images/Common/Result/bt_first_down.gif"
				);	
			js.AppendFormat("\r\n\t {0}_img_next_out = new Image(); {0}_img_next_out.src ='{1}';\r\n{0}_img_next_in = new Image(); {0}_img_next_in.src ='{2}';\r\n{0}_img_previous_out = new Image(); {0}_img_previous_out.src ='{3}';\r\n{0}_img_previous_in = new Image(); {0}_img_previous_in.src ='{4}';\r\n"
				, this.ID
                , "/App_Themes/" + themeName + "/Images/Common/Result/bt_next_up.gif"
                , "/App_Themes/" + themeName + "/Images/Common/Result/bt_next_down.gif"
                , "/App_Themes/" + themeName + "/Images/Common/Result/bt_previous_up.gif"
                , "/App_Themes/" + themeName + "/Images/Common/Result/bt_previous_down.gif"
				);
			js.AppendFormat("\r\n\t {0}_img_detail_out = new Image(); {0}_img_detail_out.src ='{1}';\r\n{0}_img_detail_in = new Image(); {0}_img_detail_in.src ='{2}';\r\n"
				, this.ID
                , "/App_Themes/" + themeName + "/Images/Common/Result/bt_detail_up.gif"
                , "/App_Themes/" + themeName + "/Images/Common/Result/bt_detail_down.gif"
				);
			#endregion

			#region Get_ResultWebControl()
			js.Append("\r\n function get_"+this.ID+"(){");
			js.Append("\r\n\t oN=document.getElementById('res_"+this.ID+"');");
			js.Append("\r\n\t oN.innerHTML='"+GetLoadingHTML()+"';");
							
			if(_allowPaging)
                js.Append("\r\n\t " + this.GetType().Namespace + "." + this.GetType().Name + ".GetData('" + _customerWebSession.IdSession + "',resultParameters,styleParameters,sortParameters,get_" + this.ID + "_callback);");
			else
                js.Append("\r\n\t " + this.GetType().Namespace + "." + this.GetType().Name + ".GetDataWithoutPagination('" + _customerWebSession.IdSession + "',resultParameters,styleParameters,sortParameters,get_" + this.ID + "_withoutpagination_callback);");

	
			js.Append("\r\n\t }");
			#endregion

			#region get_ResultWebControl_withoutpagination_callback(res)
			js.Append("\r\n function get_"+this.ID+"_withoutpagination_callback(res){");
			js.Append("\r\n\t oN=document.getElementById('res_"+this.ID+"');");
			js.AppendFormat("\r\n\t {0}_img_croi_out = new Image(); {0}_img_croi_out.src ='{1}';\r\n{0}_img_croi_in = new Image(); {0}_img_croi_in.src ='{2}';\r\n{0}_img_decroi_out = new Image(); {0}_img_decroi_out.src ='{3}';\r\n{0}_img_decroi_in = new Image(); {0}_img_decroi_in.src ='{4}';\r\n"
				, this.ID
				, this.ImgBtnCroiPath
				, this.ImgBtnCroiOverPath
				, this.ImgBtnDeCroiPath
				, this.ImgBtnDeCroiOverPath
				);
			js.Append("\r\n\t\t var sb = new StringBuilder();");							
			js.Append("\r\n\t if(res!=null && res.value != null){ ");
            js.Append("\r\n\t\t sb.append('<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td class=\"nav\" height=\"27\" align=\"left\" background=\"/App_Themes/" + themeName + "/Images/Common/Result/header.gif\">');");				
			js.Append("\r\n\t\t sb.append(GetNavigationBar('isUp'));");	
			js.Append("\r\n\t\t sb.append('</td></tr>');");
            js.Append("\r\n\t\t sb.append('<tr> <td align=\"center\" class=\"resultTableBorder\">');");			
			js.Append("\r\n\t\t sb.append(res.value);");
			js.Append("\r\n\t\t sb.append('</td></tr>');");
            js.Append("\r\n\t\t sb.append('<tr><td class=\"nav\" height=\"27\" align=\"left\" background=\"/App_Themes/" + themeName + "/Images/Common/Result/footer.gif\">');");	
			js.Append("\r\n\t\t sb.append(GetNavigationBar('isDown'));");
			js.Append("\r\n\t\t sb.append('</td></tr></table>');");
			js.Append("\r\n\t oN.innerHTML = sb.toString();");	
			js.Append("\r\n\t } ");			
			js.Append("\r\n\t else { ");
			js.Append("\r\n\t oN.innerHTML='<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,_customerWebSession.SiteLanguage)+"</div>';");
			js.Append("\r\n\t } ");
			js.Append("\r\n }");
			#endregion

			#region get_ResultWebControl_callback(res)
			js.Append("\r\n function get_"+this.ID+"_callback(res){");//res.error
			js.Append("\r\n\t if(res.error != null){ ");
				js.Append("\r\n\t oN.innerHTML = res.error.Message;");
			js.Append("\r\n\t }\r\n");
			js.Append("\r\n\t else if(res.value != null){ ");
			js.Append("\r\n\t currentPageIndex = 1;");
			js.Append("\r\n\t leftPageIndex  = 0;");
			js.Append("\r\n\t rightPageIndex  = 0;");
			js.Append("\r\n\t pageCount  = 0;");

			//Sauvergarde Nombre de lignes dans cookie
			 js.Append("\r\n\t   var cook = GetCookie(\""+TNS.AdExpress.Constantes.Web.Cookies.CurrentPageSize+"\"); ");
			 js.Append("\r\n\t if(cook != null){");
			 js.Append("\r\n\t pageSize = cook;");
			 js.Append("\r\n\t }");
			 js.Append("\r\n\t else {");
			 js.Append("\r\n\t if(pageSize<=0)pageSize ="+_defaultPageSize+";");
			 js.Append("\r\n\t }");

			js.Append("\r\n\t pageSizeOptionsList ='"+_pageSizeOptions+"';");
			js.Append("\r\n\t numberIndexPage ='"+_numberIndexPage+"';");
			js.Append("\r\n\t oN=document.getElementById('res_"+this.ID+"');");	
		
			js.Append("\r\n\t globalTable = res.value;");
			js.Append("\r\n\t if(globalTable!=null){");
			js.Append("\r\n\t\t if(globalTable[0]!=null && globalTable[0].length>0){");//Recupère tableau de résultat
			js.Append("\r\n\t\t\t tab = globalTable[0];");
			js.Append("\r\n\t\t }");
			js.Append("\r\n\t\t if(globalTable.length>1 && globalTable[1]!=null){");//Recupère tableau d'index
			js.Append("\r\n\t\t  tabIndex = globalTable[1];");
			js.Append("\r\n\t\t }");
			js.Append("\r\n\t }");			
			js.Append("\r\n\t if(tab!=null && tab.length>0){");//Total pages
			js.Append("\r\n\t\t pageCount = Math.ceil((tab.length - 1)/pageSize);");
			js.Append("\r\n\t }");
			
			//page de résultat
			js.Append("\r\n\t GetResultPage(oN);");
			js.AppendFormat("\r\n\t {0}_img_croi_out = new Image(); {0}_img_croi_out.src ='{1}';\r\n{0}_img_croi_in = new Image(); {0}_img_croi_in.src ='{2}';\r\n{0}_img_decroi_out = new Image(); {0}_img_decroi_out.src ='{3}';\r\n{0}_img_decroi_in = new Image(); {0}_img_decroi_in.src ='{4}';\r\n"
				, this.ID
				, this.ImgBtnCroiPath
				, this.ImgBtnCroiOverPath
				, this.ImgBtnDeCroiPath
				, this.ImgBtnDeCroiOverPath
				);

			js.Append("\r\n\t globalTable = null;");
			js.Append("\r\n\t res = null;");
			js.Append("\r\n\t }");
			js.Append("\r\n\t else{");
				js.Append("\r\n\t oN.innerHTML='<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,_customerWebSession.SiteLanguage)+"</div>';");
			js.Append("\r\n\t }\r\n");
			js.Append("\r\n\t }\r\n");
			#endregion

			#region get_ResultWebControl_DetailSelection
			js.Append("\r\nfunction get_"+this.ID+"_DetailSelection_callback(res){");
			js.Append("\r\n\t if(detailSelectionHtml==null){");
			js.Append("\r\n\tvar oN=document.getElementById('windowExpandMenu_"+this.ID+"');");			
			js.Append("\r\n\toN.innerHTML=res.value;");
			js.Append("\r\n\tdetailSelectionHtml=res.value;");
			js.Append("\r\n\t }\r\n");
			js.Append("\r\n}\r\n");

			js.Append("\r\nfunction get_"+this.ID+"_DetailSelection(){");
			js.Append("\r\n\t if(detailSelectionHtml==null){");

            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".GetDetailSelection('" + _customerWebSession.IdSession + "','" + this.ID + "',resultParameters,styleParameters,sortParameters,get_" + this.ID + "_DetailSelection_callback);");

			js.Append("\r\n\t }\r\n");
			js.Append("\r\n}");
			#endregion

			// ID du Div du détail de sélection
			js.Append("var divName='windowExpandMenu_"+this.ID+"'");

			js.Append("\r\naddEvent(window, \"load\", get_"+this.ID+");");

			#region Fonction Page de résultat
			js.Append("\r\n\nfunction GetResultPage(obj){");			
			
			js.Append("\r\n\t var htmlNavigationBarUp=''; ");
			js.Append("\r\n\t var htmlNavigationBarDown=''; ");
			js.Append("\r\n\t\t var sb = new StringBuilder();");	
			js.Append("\r\n var i;");	
		
			js.Append("\r\n\t htmlNavigationBarUp=GetNavigationBar('isUp'); ");

            js.Append("\r\n\t\t sb.append('<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td class=\"nav\" height=\"27\" align=\"left\" background=\"/App_Themes/" + themeName + "/Images/Common/Result/header.gif\">');");				
			js.Append("\r\n\t\t sb.append(htmlNavigationBarUp);");	
			js.Append("\r\n\t\t sb.append('</td></tr>');");


            js.Append("\r\n\t\t sb.append('<tr> <td align=\"center\" class=\"resultTableBorder\">');");
            js.Append("\r\n\t\t sb.append('<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0>');");	
			js.Append("\r\n\t\t sb.append( tab[0]);");	
			js.Append("\r\n\t if(currentPageIndex==1) ");
			js.Append("\r\n\t\t i=(currentPageIndex*pageSize - pageSize ) + 1 ; ");
			js.Append("\r\n\t else ");
			js.Append("\r\n\t\t i=(currentPageIndex*pageSize - pageSize ); ");
			
			#region Répétion en-tête parent
			//Repete parent si page courante commance par niveau le plus bas	
			js.Append("\r\n\t if(isShowParenHeader  && tabIndex!=null && pageCount>1){");
			js.Append("\r\n\t GetParentLines(i,sb);");
			js.Append("\r\n\t }");
			#endregion
		
			js.Append("\r\n\t for( i ; i< (currentPageIndex*pageSize) && i <tab.length ; i++){ ");					
			js.Append("\r\n\t\t sb.append(tab[i]);");		
			js.Append("\r\n\t }");								
			js.Append("\r\n\t\t sb.append('</table></td></tr>');");
											
			js.Append("\r\n\t htmlNavigationBarDown=GetNavigationBar('isDown'); ");
            js.Append("\r\n\t\t sb.append('<tr><td class=\"nav\" height=\"27\" align=\"left\" background=\"/App_Themes/" + themeName + "/Images/Common/Result/footer.gif\">');");	
			js.Append("\r\n\t\t sb.append(htmlNavigationBarDown);");
			js.Append("\r\n\t\t sb.append('</td></tr></table>');");
			js.Append("\r\n\t obj.innerHTML=sb.toString();");	
			js.Append("\r\n\t\t sb=null;");

			js.Append("\r\n }");
			#endregion

			#region Fonction pagination
			
			js.Append("\r\n\nfunction paginate(pageIndex){");
			js.Append("\r\n\t currentPageIndex = pageIndex;");

			//page de résultat
            js.Append("\r\n\t oN.innerHTML='<img src=\"/App_Themes/" + themeName + "/Images/Common/waitAjax.gif\">';");
			js.Append("\r\n\t setTimeout(\"GetResultPage(oN)\",0);");
						
			js.Append("\r\n}");
			#endregion

			#region Fonction Barre de navigation
			js.Append("\r\n\nfunction GetNavigationBar(up){");
			js.Append("\r\n\t var htmlNavigationBar = ''; ");
			js.Append("\r\n\t var nbIndexPage = numberIndexPage - 1; ");
			js.Append("\r\n\t if( pageCount <= nbIndexPage ) { ");
				js.Append("\r\n\t  nbIndexPage = pageCount - 1; ");
			js.Append("\r\n\t\t }");
			
			js.Append("\r\n\t if( pageCount > 0 ) { ");
			
			js.Append("\r\n\t\t if( pageCount > 1 ) { ");
            js.Append("\r\n\t\t htmlNavigationBar += ' <font class=\"pinkTextColor\">'+currentPageIndex+'</font> ';");
			js.Append("\r\n\t\t leftPageIndex = rightPageIndex = currentPageIndex;");
			js.Append("\r\n\t\t while(nbIndexPage>0 && pageCount>1){ ");
				js.Append("\r\n\t\t\t if( currentPageIndex!=1 && leftPageIndex>1) {");
				js.Append("\r\n\t\t\t\t leftPageIndex--; ");
				js.Append("\r\n\t\t\t\t nbIndexPage--; ");						
				js.Append("\r\n\t\t\t\t\t htmlNavigationBar =' <a class=\"navlink\" href=\"javascript:paginate('+leftPageIndex+');\">'+leftPageIndex+'</a> '+htmlNavigationBar ;");						
				js.Append("\r\n\t\t\t }");
				js.Append("\r\n\t\t\t if( currentPageIndex < pageCount && rightPageIndex < pageCount) {");
				js.Append("\r\n\t\t\t\t rightPageIndex++; ");
				js.Append("\r\n\t\t\t\t nbIndexPage--; ");													
				js.Append("\r\n\t\t\t\t\t htmlNavigationBar = htmlNavigationBar+' <a class=\"navlink\" href=\"javascript:paginate('+rightPageIndex+');\">'+rightPageIndex+'</a> ' ;");						
			js.Append("\r\n\t\t\t }");
			js.Append("\r\n\t\t }");

			//Page précédente
			js.Append("\r\n\t\t var pres='';");
			js.Append("\r\n\t\t\t tempIndex = currentPageIndex-1;");
			js.Append("\r\n\t\t\t if(currentPageIndex > 1) {");
			js.Append("\r\n\t\t\t\t  pres = pres+ '<a ';");
			js.Append("\r\n\t\t\t\t  pres = pres +' onmouseover=\"'+up+'_previous_img.src=" + this.ID + "_img_previous_in.src;\"';");
			js.Append("\r\n\t\t\t\t  pres = pres +' onmouseout=\"'+up+'_previous_img.src=" + this.ID + "_img_previous_out.src;\"' ;");
			js.Append("\r\n\t\t\t\t  pres = pres +' href=\"javascript:paginate('+tempIndex+');\" ';");
            js.Append("\r\n\t\t\t\t  pres = pres +'><IMG border=0 alt=\"\" name=\"'+up+'_previous_img\" src=\"/App_Themes/" + themeName + "/Images/Common/Result/bt_previous_up.gif\"></a>';");
			js.Append("\r\n\t\t\t  }");
			js.Append("\r\n\t\t\t  else {");
            js.Append("\r\n\t\t\t\t  pres = pres +'<IMG border=0 alt=\"\" name=\"'+up+'_previous_img\" src=\"/App_Themes/" + themeName + "/Images/Common/Result/bt_previous_up.gif\">';");
			js.Append("\r\n\t\t\t  }");
			js.Append("\r\n\t\t\t  htmlNavigationBar=pres+htmlNavigationBar;");

			//Première Page 
			js.Append("\r\n\t\t var firstpage='';");
			js.Append("\r\n\t\t\t tempIndex = 1;");
			js.Append("\r\n\t\t\t if(currentPageIndex > 1) {");
			js.Append("\r\n\t\t\t  firstpage = firstpage+ '<a ';");
			js.Append("\r\n\t\t\t  firstpage = firstpage +' onmouseover=\"'+up+'_first_img.src=" + this.ID + "_img_first_in.src;\"';");
			js.Append("\r\n\t\t\t  firstpage = firstpage +' onmouseout=\"'+up+'_first_img.src=" + this.ID + "_img_first_out.src;\"' ;");
			js.Append("\r\n\t\t\t  firstpage = firstpage +' href=\"javascript:paginate('+tempIndex+');\" ';");
            js.Append("\r\n\t\t\t  firstpage = firstpage +'><IMG border=0 alt=\"\" name=\"'+up+'_first_img\" src=\"/App_Themes/" + themeName + "/Images/Common/Result/bt_first_up.gif\"></a>';");
			js.Append("\r\n\t\t\t  }");
			js.Append("\r\n\t\t\t  else {");
            js.Append("\r\n\t\t\t  firstpage = firstpage +'<IMG border=0 alt=\"\" name=\"'+up+'_first_img\" src=\"/App_Themes/" + themeName + "/Images/Common/Result/bt_first_up.gif\">';");
			js.Append("\r\n\t\t\t  }");
			js.Append("\r\n\t\t\t  htmlNavigationBar= '&nbsp;&nbsp;'+firstpage+htmlNavigationBar;");

			//Page suivante
			js.Append("\r\n\t\t\t  tempIndex = currentPageIndex +1;");
			js.Append("\r\n\t\t\t if(currentPageIndex < pageCount) {");
			js.Append("\r\n\t\t\t\t  htmlNavigationBar = htmlNavigationBar +'<a ';");
			js.Append("\r\n\t\t\t\t  htmlNavigationBar = htmlNavigationBar +' onmouseover=\"'+up+'_next_img.src=" + this.ID + "_img_next_in.src;\"';");
			js.Append("\r\n\t\t\t\t  htmlNavigationBar = htmlNavigationBar +' onmouseout=\"'+up+'_next_img.src=" + this.ID + "_img_next_out.src;\"' ;");
			js.Append("\r\n\t\t\t\t  htmlNavigationBar = htmlNavigationBar +' href=\"javascript:paginate('+tempIndex+');\" ';");
            js.Append("\r\n\t\t\t\t  htmlNavigationBar = htmlNavigationBar +'><IMG border=0 alt=\"\" name=\"'+up+'_next_img\" src=\"/App_Themes/" + themeName + "/Images/Common/Result/bt_next_up.gif\"></a>';");
			js.Append("\r\n\t\t\t  }");
			js.Append("\r\n\t\t\t  else {");
            js.Append("\r\n\t\t\t\t  htmlNavigationBar = htmlNavigationBar +'<IMG border=0 alt=\"\" name=\"'+up+'_next_img\" src=\"/App_Themes/" + themeName + "/Images/Common/Result/bt_next_up.gif\">';");
			js.Append("\r\n\t\t\t  }");

			//Dernière Page 			
			js.Append("\r\n\t\t var lastpage='';");
			js.Append("\r\n\t\t\t tempIndex = pageCount;");
			js.Append("\r\n\t\t\t if(currentPageIndex < pageCount) {");
			js.Append("\r\n\t\t\t\t  lastpage = lastpage+ '<a ';");
			js.Append("\r\n\t\t\t\t  lastpage = lastpage +' onmouseover=\"'+up+'_last_img.src=" + this.ID + "_img_last_in.src;\"';");
			js.Append("\r\n\t\t\t\t  lastpage = lastpage +' onmouseout=\"'+up+'_last_img.src=" + this.ID + "_img_last_out.src;\"' ;");
			js.Append("\r\n\t\t\t\t  lastpage = lastpage +' href=\"javascript:paginate('+tempIndex+');\" ';");
            js.Append("\r\n\t\t\t\t  lastpage = lastpage +'><IMG border=0 alt=\"\" name=\"'+up+'_last_img\" src=\"/App_Themes/" + themeName + "/Images/Common/Result/bt_last_up.gif\"></a>';");
			js.Append("\r\n\t\t\t  }");
			js.Append("\r\n\t\t\t  else {");
            js.Append("\r\n\t\t\t\t  lastpage = lastpage +'<IMG border=0 alt=\"\" name=\"'+up+'_last_img\" src=\"/App_Themes/" + themeName + "/Images/Common/Result/bt_last_up.gif\">';");
			js.Append("\r\n\t\t\t  }");
			js.Append("\r\n\t\t\t  htmlNavigationBar=htmlNavigationBar+lastpage;");
			js.Append("\r\n\t\t } ");		

			//Option ajout répétion en-tête élément parent 
			js.Append("\r\n\t\t\t if(tabIndex!=null && pageCount>1) {");
			js.Append("\r\n\t\t htmlNavigationBar=HeaderParentOption(htmlNavigationBar,up); ");
			js.Append("\r\n\t\t }");

			//Sélection options taille page
			js.Append("\r\n\t\t htmlNavigationBar=PageSizeOptions(pageSizeOptionsList,htmlNavigationBar,up); }");
					
			//Appel calque rappel sélection (Rappel de sélection : 1989)
			//js.Append("\r\n htmlNavigationBar ='&nbsp;<a href=\"javascript:afficher(divName);\" ><img src=\"/Images/Common/Result/bt_detail_up.gif\" border=0 title=\"Détail de sélection\" align=\"absmiddle\"></a>'+htmlNavigationBar;");
            js.Append("\r\n htmlNavigationBar ='&nbsp;<a href=\"javascript:afficher(divName);\"     onmouseover=\"'+up+'_detail_img.src=" + this.ID + "_img_detail_in.src;\" onmouseout=\"'+up+'_detail_img.src=" + this.ID + "_img_detail_out.src;\"     ><img src=\"/App_Themes/" + themeName + "/Images/Common/Result/bt_detail_up.gif\" border=0 title=\"" + GestionWeb.GetWebWord(1989, _customerWebSession.SiteLanguage) + "\" align=\"absmiddle\" name=\"'+up+'_detail_img\"></a>&nbsp;'+htmlNavigationBar;");
			//get_"+this.ID+"_DetailSelection(
			js.Append("\r\n\t return(htmlNavigationBar); ");
			js.Append("\r\n }");
			#endregion
			
			#region Fonction options nombre de lignes par page
			//Fonction options nombre de lignes par page
			js.Append("\r\n\n function PageSizeOptions(pagesizeList,htmlNavigationBar,up){");			
			js.Append("\r\n\t var list = pagesizeList.split(\",\");");
			js.Append("\r\n\t var isSelected = '';");
			js.Append("\r\n\t var tempSelect;");
			js.Append("\r\n\t var n;");
			js.Append("\r\n\t tempSelect='<span >'; ");			
			js.Append("\r\n\t tempSelect= tempSelect +' "+ GestionWeb.GetWebWord(2045,_customerWebSession.SiteLanguage)+" <select name=\"pageSizeOptions\" id=\"'+up+'pageSizeOptions\" onChange=\"ChangePageSize(this.value)\"  class=\"txtNoir11\">'; ");
			js.Append("\r\n\t for( n=0; n<list.length; n++){ ");
			js.Append("\r\n\t\t if(pageSize==list[n]){ ");
			js.Append("\r\n\t\t isSelected='selected';");
			js.Append("\r\n\t\t }");
			js.Append("\r\n\t\t if(n==0){ ");
			js.Append("\r\n\t\t minPageSize=list[n];");
			js.Append("\r\n\t\t }");
			js.Append("\r\n\t\t tempSelect = tempSelect + '<option value=\"'+list[n]+'\" '+isSelected+'>'+list[n]+'</option>';");
			js.Append("\r\n\t\t isSelected='';");
			js.Append("\r\n\t }");
			js.Append("\r\n\t tempSelect= tempSelect +'</select></span>&nbsp;'; ");
			js.Append("\r\n\t if(tab!=null && minPageSize>=tab.length){");
			js.Append("\r\n\t\t tempSelect= '';");
			js.Append("\r\n\t }");
			
			js.Append("\r\n\t htmlNavigationBar = tempSelect + htmlNavigationBar;");						
			js.Append("\r\n return(htmlNavigationBar);}");
						
			js.Append("\r\n\n function ChangePageSize(pagesizeIndex){");			
			js.Append("\r\n\t pageSize = pagesizeIndex;  ");

			js.Append("\r\n\t setCookie(\""+TNS.AdExpress.Constantes.Web.Cookies.CurrentPageSize+"\",pagesizeIndex,365); ");//

			js.Append("\r\n\t currentPageIndex  = 1, leftPageIndex  = 0, rightPageIndex  = 0;");
			js.Append("\r\n\t if(tab!=null && tab.length>0){");//Total pages
			js.Append("\r\n\t\t pageCount = Math.ceil((tab.length - 1)/pageSize);");
			js.Append("\r\n\t }");
			
			//page de résultat	
            js.Append("\r\n\t oN.innerHTML='<img src=\"/App_Themes/" + themeName + "/Images/Common/waitAjax.gif\">';");
			js.Append("\r\n\t setTimeout(\"GetResultPage(oN)\",0);");
			js.Append("\r\n }");
			#endregion

			#region Fonction Option ajout en-tête parent
			js.Append("\r\n\n function HeaderParentOption(htmlNavigationBar,up){");
			js.Append("\r\n\t var tempCheckBox,temp;");
			js.Append("\r\n\t var cook = GetCookie(\""+TNS.AdExpress.Constantes.Web.Cookies.IsShowParenHeader+"\"); ");
			js.Append("\r\n\t if(cook != null){");
			js.Append("\r\n\t if(cook==\"false\")temp = false; ");
			js.Append("\r\n\t else temp = true;");
			js.Append("\r\n\t isShowParenHeader =  temp;");
			js.Append("\r\n\t }");
			js.Append("\r\n\t tempCheckBox='<input type=checkbox style=\"WIDTH: 15px; HEIGHT: 15px\" id=\"'+up+'_headerParentOption\" '; ");
			js.Append("	if(isShowParenHeader){ ");
			js.Append("	tempCheckBox += ' checked '; ");
			js.Append("\r\n }");			
			js.Append("\r\n	tempCheckBox += ' onClick=\"SetHeaderParentOption(this.checked)\">'; ");
			js.Append("\r\n\t htmlNavigationBar = '&nbsp;&nbsp;|&nbsp;&nbsp;"+ GestionWeb.GetWebWord(2046,_customerWebSession.SiteLanguage)+"&nbsp;' + tempCheckBox + htmlNavigationBar;");//TODO texte à charger depuis la base depuis traducteur
			js.Append("\r\n return(htmlNavigationBar);}");

			js.Append("\r\n\n function SetHeaderParentOption(showparent){");			
			js.Append("\r\n\t isShowParenHeader = showparent;");

			js.Append("\r\n\t setCookie(\""+TNS.AdExpress.Constantes.Web.Cookies.IsShowParenHeader+"\",isShowParenHeader,365); ");//Cookie rappel en-tête


			//page de résultat

            js.Append("\r\n\t oN.innerHTML='<img src=\"/App_Themes/" + themeName + "/Images/Common/waitAjax.gif\">';");
			js.Append("\r\n\t setTimeout(\"GetResultPage(oN)\",0);");
			js.Append("\r\n }");
			#endregion

			#region Fonction ajout de(s) ligne(s) parent

			js.Append("\r\n\n function GetParentLines(lineIndex,sBuilder){");
			js.Append("\r\n\t if( tabIndex[lineIndex]!=null && tabIndex[lineIndex]!=0 ){"); //&& lineIndex!=0){");
			js.Append("\r\n\t\t GetParentLines(tabIndex[lineIndex],sBuilder);");
			js.Append("\r\n\t\t sBuilder.append(tab[tabIndex[lineIndex]]);");
			js.Append("\r\n\t}");
			js.Append("\r\n }");
			
			#endregion

			#region fonction Styles CSS
            //js.Append("\r\n\nfunction SetCssStyles(obj){");
            //js.Append("\r\n\t obj.CssDetailSelectionL1 = '"+CssDetailSelectionL1+"';");
            //js.Append("\r\n\t obj.CssDetailSelectionL2 = '"+CssDetailSelectionL2+"';");
            //js.Append("\r\n\t obj.CssDetailSelectionL3 = '"+CssDetailSelectionL3+"';");
            //js.Append("\r\n\t obj.CssDetailSelectionTitleGlobal = '"+CssDetailSelectionTitleGlobal+"';");
            //js.Append("\r\n\t obj.CssDetailSelectionTitle = '"+CssDetailSelectionTitle+"';");
            //js.Append("\r\n\t obj.CssDetailSelectionTitleData = '"+CssDetailSelectionTitleData+"';");
            //js.Append("\r\n\t obj.CssDetailSelectionBordelLevel = '"+CssDetailSelectionBordelLevel+"';");
            //js.Append("\r\n\t obj.CssLHeader = '"+CssLHeader+"';");
            //js.Append("\r\n\t obj.CssL1 = '"+CssL1+"';");
            //js.Append("\r\n\t obj.BackgroudColorL1 = '"+BackgroudColorL1+"';");		
            //js.Append("\r\n\t obj.HighlightBackgroundColorL1 = '"+HighlightBackgroundColorL1+"';");
            //js.Append("\r\n\t obj.HtmlCodeL1 = '"+_htmlCodeL1+"';");
            //js.Append("\r\n\t obj.CssL2 = '"+CssL2+"';");
            //js.Append("\r\n\t obj.BackgroudColorL2 = '"+BackgroudColorL2+"';");
            //js.Append("\r\n\t obj.HighlightBackgroundColorL2 = '"+HighlightBackgroundColorL2+"';");
            //js.Append("\r\n\t obj.HtmlCodeL2 = '"+_htmlCodeL2+"';");
            //js.Append("\r\n\t obj.CssL3 = '"+CssL3+"';");
            //js.Append("\r\n\t obj.BackgroudColorL3 = '"+BackgroudColorL3+"';");
            //js.Append("\r\n\t obj.HighlightBackgroundColorL3 = '"+HighlightBackgroundColorL3+"';");
            //js.Append("\r\n\t obj.HtmlCodeL3 = '"+_htmlCodeL3+"';");
            //js.Append("\r\n\t obj.CssL4 = '"+CssL4+"';");
            //js.Append("\r\n\t obj.BackgroudColorL4 = '"+BackgroudColorL4+"';");
            //js.Append("\r\n\t obj.HighlightBackgroundColorL4 = '"+HighlightBackgroundColorL4+"';");
            //js.Append("\r\n\t obj.HtmlCodeL4 = '"+_htmlCodeL4+"';");
            //js.Append("\r\n\t obj.CssLTotal = '"+CssLTotal+"';");
            //js.Append("\r\n\t obj.ImgBtnCroiOverPath = '"+this.ImgBtnCroiOverPath+"';");
            //js.Append("\r\n\t obj.ImgBtnCroiPath = '"+this.ImgBtnCroiPath+"';");
            //js.Append("\r\n\t obj.ImgBtnDeCroiOverPath = '"+this.ImgBtnDeCroiOverPath+"';");
            //js.Append("\r\n\t obj.ImgBtnDeCroiPath = '"+this.ImgBtnDeCroiPath+"';");
            //js.Append("\r\n\t obj.Id = '"+this.ID+"';");
            //js.Append("\r\n\t obj.Sort = '"+this._sortOrder.GetHashCode()+"';");
            //js.Append("\r\n\t obj.Key = '"+this._sSortKey+"';");
            //js.Append("\r\n\t obj.CssTitle = '"+this._cssTitle+"';");
            //js.Append("\r\n\t obj.IdTitleText = '"+this._idTitleText+"';");
            //js.Append("\r\n }");
			#endregion

			#region Sorting
			js.Append("\r\n function sort_"+this.ID+"(id,params){");
			js.Append("\r\n var tab1 = params.split(\",\");");
            js.Append("\r\n sortParameters.Sort = tab1[1];");
            js.Append("\r\n sortParameters.Key = tab1[0];");
			js.Append("\r\n tab = null;");
			js.Append("\r\n tabIndex = null;");
			js.Append("\r\n get_"+this.ID+"();");
			js.Append("\r\n }");
			#endregion

			#region Fonction StringBuilder
			js.Append("\r\n function StringBuilder(value){");
			js.Append("\r\n\t this.strings = new Array(\"\");");	
			js.Append("\r\n\t this.append(value);");
			js.Append("\r\n }");

			js.Append("\r\n StringBuilder.prototype.append = function (value){");
			js.Append("\r\n\t if (value){");	
			js.Append("\r\n\t this.strings.push(value);");
			js.Append("\r\n\t }");
			js.Append("\r\n  }");

			js.Append("\r\n StringBuilder.prototype.clear = function (){");
			js.Append("\r\n\t this.strings.length = 1;");				
			js.Append("\r\n  }");

			js.Append("\r\n StringBuilder.prototype.toString = function (){");
			js.Append("\r\n\t return this.strings.join(\"\");");				
			js.Append("\r\n  }");
			#endregion

			#region Cookies
			js.Append("\r\n function setCookie(name,value,days) {");
			js.Append("\r\n\t  var expire = new Date ();");	
			js.Append("\r\n\t  expire.setTime (expire.getTime() + (24 * 60 * 60 * 1000) * days);");
			js.Append("\r\n\t document.cookie = name + \"=\" + escape(value) + \"; expires=\" +expire.toGMTString();");						
			js.Append("\r\n  }");

			js.Append("\r\n function GetCookie(name) {");
			js.Append("\r\n\t var startIndex = document.cookie.indexOf(name);");
			js.Append("\r\n\t  if (startIndex != -1) {");
			js.Append("\r\n\t var endIndex = document.cookie.indexOf(\";\", startIndex);");
			js.Append("\r\n\t   if (endIndex == -1) endIndex = document.cookie.length;");
			js.Append("\r\n\t   return unescape(document.cookie.substring(startIndex+name.length+1, endIndex));");
			js.Append("\r\n\t  }");
			js.Append("\r\n\t  else {");
			js.Append("\r\n\t  return null;");
			js.Append("\r\n\t  }");
			js.Append("\r\n  }");

			js.Append("\r\n function DeleteCookie(name) {");
			js.Append("\r\n\t  var expire = new Date ();");		
			js.Append("\r\n\t  expire.setTime (expire.getTime() - (24 * 60 * 60 * 1000));");		
			js.Append("\r\n\t  document.cookie = name + \"=; expires=\" + expire.toGMTString();");		
			js.Append("\r\n  }");
			#endregion

			js.Append("\r\n-->\r\n</SCRIPT>");
			return(js.ToString());
		}
		#endregion

		#region Script Rappel des sélections
		/// <summary>
		/// Génération des fonctions javascript nécessaires au rappel des sélections
		/// </summary>
		/// <returns>Code Javascript</returns>
		protected string SelectionCallBackScript(){
			StringBuilder js=new StringBuilder(3000);
            string themeName = WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name;

			js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
		
			#region Variables globales
			js.Append("\r\n\t var GECKO=(navigator.product==(\"Gecko\"));");
			
			js.Append("\r\n\t if(GECKO){");
            js.Append("\r\n\t\t document.write('<link href=\"/Css/" + themeName + "/DefaultGecko.css\" rel=\"stylesheet\" type=\"text/css\">');");
			js.Append("\r\n}\r\n");

			js.Append("\r\n\t else{");
            js.Append("\r\n\t\t document.write('<link href=\"/Css/" + themeName + "/Default.css\" rel=\"stylesheet\" type=\"text/css\">');");
			js.Append("\r\n\t }");

			js.Append("\r\n var IE=document.all;");
			js.Append("\r\n var grabX=0,grabY=0;");
			js.Append("\r\n var oldBackground0,oldBackground1,oldBorder;");
			js.Append("\r\n var controlOver=false;");
			js.Append("\r\n var GrabTarget;");
			#endregion			
			
			#region Fonction grab
			js.Append("\r\n function grab(evt,Target){");
			js.Append("\r\n\t GrabTarget=document.getElementById(Target);");
			js.Append("\r\n\t if(IE){");
			js.Append("\r\n\t\t grabX=evt.offsetX;");
			js.Append("\r\n\t\t grabY=evt.offsetY;");
			js.Append("\r\n\t\t GrabTarget.style.filter=\"Alpha(Opacity=80)\";");
			js.Append("\r\n\t\t evt.cancelBubble=true;");
			js.Append("\r\n\t }");
			js.Append("\r\n\tif(GECKO){");
			js.Append("\r\n\t\t	grabX=evt.layerX;");
			js.Append("\r\n\t\t	grabY=evt.layerY;");
			js.Append("\r\n\t\t	GrabTarget.style.MozOpacity=\"0.6\";");
			js.Append("\r\n\t\t	evt.stopPropagation();");
			js.Append("\r\n\t }");
			js.Append("\r\n\t document.onmouseup=release;");
			js.Append("\r\n\t document.onmousemove=move;");
			js.Append("\r\n\t return(false);");
			js.Append("\r\n }");
			#endregion

			#region Fonction release
			js.Append("\r\nfunction release(evt){");
			js.Append("\r\n\tif(IE){");
			js.Append("\r\n\t\tevt=event;");
			js.Append("\r\n\t\tGrabTarget.style.filter=\"Alpha(Opacity=100)\";");
			js.Append("\r\n\t\tevt.cancelBubble=true;");
			js.Append("\r\n\t}");
			js.Append("\r\n\tif(GECKO){");
			js.Append("\r\n\t\tControlPart=findControlPart(GrabTarget,\"GrabCaption\");");
			js.Append("\r\n\t\tif(ControlPart!=null) deselectControl(null,ControlPart);");
			js.Append("\r\n\t\tGrabTarget.style.MozOpacity=\"1.0\";"); 
			js.Append("\r\n\t\tevt.stopPropagation();");
			js.Append("\r\n\t}");
			js.Append("\r\n\tdocument.onmouseup=null;");
			js.Append("\r\n\tdocument.onmousemove=null;");
			js.Append("\r\nreturn(false);");
			js.Append("\r\n}");
			#endregion

			#region Fonction move
			js.Append("\r\nfunction move(evt){");
			js.Append("\r\n\t if(IE){");
			js.Append("\r\n\t\t evt=event;");
			js.Append("\r\n\t\t evt.cancelBubble=true;");
			js.Append("\r\n\t\t GrabTarget.style.top=(event.y+document.body.scrollTop-grabY);");
			js.Append("\r\n\t\t GrabTarget.style.left=(event.x+document.body.scrollLeft-grabX);");
			js.Append("\r\n\t }");
			js.Append("\r\n\t if(GECKO){");
			js.Append("\r\n\t\t evt.stopPropagation();");
			js.Append("\r\n\t\t GrabTarget.style.top=(evt.pageY-grabY);");
			js.Append("\r\n\t\t GrabTarget.style.left=(evt.pageX-grabX);");
			js.Append("\r\n\t }");
			js.Append("\r\n\t return(false);");
			js.Append("\r\n}");
			#endregion

			#region Fonction selectMenu
			js.Append("\r\nfunction selectMenu(evt,MenuElement){");
			js.Append("\r\n\tChilds=MenuElement.getElementsByTagName(\"td\");");
			js.Append("\r\n\toldBackground0=Childs[0].style.backgroundColor;");
			js.Append("\r\n\toldBackground1=Childs[1].style.backgroundColor;");
			js.Append("\r\n\toldBorder=Childs[1].style.border;");
			js.Append("\r\n\tif(IE){");
			js.Append("\r\n\t\tMenuElement.style.filter=\"blendTrans(duration=0.3)\";");
			js.Append("\r\n\t\tif(MenuElement.filters.blendTrans.status!=2) MenuElement.filters.blendTrans.Apply();");
			js.Append("\r\n\t}");
			js.Append("\r\n\tif(IE){");
			js.Append("\r\n\t\tif(MenuElement.filters.blendTrans.status!=2) MenuElement.filters.blendTrans.Play();");
			js.Append("\r\n\t\tevt.cancelBubble=true;");
			js.Append("\r\n\t}");
			js.Append("\r\n\tif(GECKO) evt.stopPropagation();");
			js.Append("\r\n\treturn(false);");
			js.Append("\r\n}");
			#endregion

			#region Fonction deselectMenu
			js.Append("\r\nfunction deselectMenu(evt,MenuElement){");
			js.Append("\r\n\tChilds=MenuElement.getElementsByTagName(\"td\");");
			js.Append("\r\n\tif(IE){");
			js.Append("\r\n\t\tMenuElement.style.filter=\"blendTrans(duration=0.3)\";");
			js.Append("\r\n\t\tif(MenuElement.filters.blendTrans.status!=2) MenuElement.filters.blendTrans.Apply();");
			js.Append("\r\n\t}");
			js.Append("\r\n\tChilds[0].style.backgroundColor=oldBackground0;");
			js.Append("\r\n\tChilds[1].style.backgroundColor=oldBackground1;");
			js.Append("\r\n\tChilds[1].style.borderStyle=\"none\";");
			js.Append("\r\n\tif(IE){");
			js.Append("\r\n\t\tif(MenuElement.filters.blendTrans.status!=2) MenuElement.filters.blendTrans.Play();");
			js.Append("\r\n\t\tevt.cancelBubble=true;");
			js.Append("\r\n\t\t}");
			js.Append("\r\n\tif(GECKO) evt.stopPropagation();");
			js.Append("\r\n\treturn(false);");
			js.Append("\r\n}");
			#endregion

			#region Fonction selectControl
			js.Append("\r\nfunction selectControl(evt,Control){");
			js.Append("\r\n\tif(!controlOver){");
			js.Append("\r\n\t\tChilds=Control.getElementsByTagName(\"td\");");
			js.Append("\r\n\t\tfor(i=0;i<Childs.length;i++){");
			js.Append("\r\n\t\t\toldBackground0=Childs[i].style.backgroundColor;");
			js.Append("\r\n\t\t}");
			js.Append("\r\n\t\tcontrolOver=true;");
			js.Append("\r\n\t}");
			js.Append("\r\n\tif(IE) evt.cancelBubble=true;");
			js.Append("\r\n\tif(GECKO) evt.stopPropagation();");
			js.Append("\r\n\treturn(false);");
			js.Append("\r\n}");
			#endregion

			#region Fonction deselectControl
			js.Append("\r\nfunction deselectControl(evt,Control){");
			js.Append("\r\n\tif(controlOver){");
			js.Append("\r\n\t\tChilds=Control.getElementsByTagName(\"td\");");
			js.Append("\r\n\t\tfor(i=0;i<Childs.length;i++) Childs[i].style.backgroundColor=oldBackground0;");
			js.Append("\r\n\t\tcontrolOver=false;");
			js.Append("\r\n\t}");
			js.Append("\r\n\t\tif(IE&&evt!=null) evt.cancelBubble=true;");
			js.Append("\r\n\t\tif(GECKO&&evt!=null) evt.stopPropagation();");
			js.Append("\r\n\treturn(false);");
			js.Append("\r\n}");
			#endregion

			#region Fonction expand
			js.Append("\r\nfunction expand(evt,Control,Target){");
			js.Append("\r\n\t Target = document.getElementById(Target);");
			js.Append("\r\n\tif(Target.style.display==\"none\") Target.style.display=\"\";");
			js.Append("\r\n\t else Target.style.display=\"none\";");
			js.Append("\r\n\t if(GECKO) evt.stopPropagation();");
			js.Append("\r\n\t deselectControl(null,Control);");
			js.Append("\r\n}");
			#endregion

			#region Fonction findControlPart
			js.Append("\r\nfunction findControlPart(Target,PartClass){");
			js.Append("\r\n\tControlParts=Target.getElementsByTagName(\"table\")");
			js.Append("\r\n\tfor(i=0;i<ControlParts.length;i++){");
			js.Append("\r\n\t\tif(ControlParts[i].className==PartClass) return(ControlParts[i]);");
			js.Append("\r\n\t}");
			js.Append("\r\n\treturn(null);");
			js.Append("\r\n}");
			#endregion

			#region Fonction masquer
			js.Append("\r\nfunction masquer(myDiv){");
			js.Append("\r\n\t document.getElementById(myDiv).style.display = 'none';");
			js.Append("\r\n}");
			#endregion

			#region Fonction afficher
			js.Append("\r\nfunction afficher(myDiv){");
			js.Append("\r\n\t get_"+this.ID+"_DetailSelection();");

			// Positionne le div au centre de l'écran avec scrollbar inclu
			js.Append("\r\n\t var position_x = document.body.scrollLeft+(screen.width-600)/2;");
			js.Append("\r\n\t var position_y = document.body.scrollTop +(screen.height-400)/2;");
			js.Append("\r\n\t document.getElementById(myDiv).style.top = position_y;");
			js.Append("\r\n\t document.getElementById(myDiv).style.left = position_x;");

			// Affichage du div
			js.Append("\r\n\t document.getElementById(myDiv).style.display = 'block';");

//			js.Append("\r\n\t if(IE){");
//			js.Append("\r\n\t document.getElementById(myDiv).style.display = 'block';");
//			js.Append("\r\n\t }");
//
//			js.Append("\r\n\t if(GECKO){");
//			//js.Append("\r\n\t var elt = document.getElementById(myDiv);");
//			//js.Append("\r\n\t elt.style.display = 'block';");
//			//js.Append("\r\n\t elt.innerHTML = document.getElementById(myDiv).innerHTML;");
//			
//			//js.Append("\r\n\t document.getElementById(myDiv).setAttribute('style','display:block');");
//
//			js.Append("\r\n\t document.getElementById(myDiv).style.display = 'block';");
//			js.Append("\r\n\t }");

			js.Append("\r\n}");
			#endregion

			js.Append("\r\n-->\r\n</SCRIPT>");
			return(js.ToString());
		}

		#endregion

		#endregion

		#region [AjaxPro.AjaxMethod]
		
		#region Chargement des paramètres AjaxPro.JavaScriptObject et WebSession
        /// <summary>
        /// Charge les paramètres des résultats navigant entre le client et le serveur
        /// </summary>
        /// <param name="o">Tableau de paramètres javascript</param>
        protected virtual void LoadResultParameters(AjaxPro.JavaScriptObject o) {
            if (o != null) {
            }
        }
        /// <summary>
		/// Charge les paramètres des tris navigant entre le client et le serveur
		/// </summary>
        /// <remarks>
        /// Paramètres:
        /// <list type="string">
        /// <item>Sort</item>
        /// <item>Key</item>
        /// <item>Id</item>
        /// </list>
        /// </remarks>
		/// <param name="o">Tableau de paramètres javascript</param>
        protected virtual void LoadSortParameters(AjaxPro.JavaScriptObject o) {
            if (o != null){
                this._sortOrder = _customerWebSession.Sorting;
                if (o.Contains("Sort")){
                    _customerWebSession.Sorting = this._sortOrder = (ResultTable.SortOrder)Convert.ToInt16(o["Sort"].Value.Replace("\"", ""));
                }
                this._sSortKey = _customerWebSession.SortKey;
                if (o.Contains("Key")){
                    _customerWebSession.SortKey = this._sSortKey = o["Key"].Value.Replace("\"", "");
                }
                if (o.Contains("Id")){
                    this.ID = o["Id"].Value.Replace("\"", "");
                }
            }
        }
		/// <summary>
		/// Charge les paramètres des sytles navigant entre le client et le serveur
		/// </summary>
		/// <param name="o">Tableau de paramètres javascript</param>
        protected virtual void LoadStyleParameters(AjaxPro.JavaScriptObject o) {
			if(o!=null){
				if(o.Contains("CssLHeader")){
					_cssLHeader = o["CssLHeader"].Value; 
					_cssLHeader = _cssLHeader.Replace("\"","");
				}
				if(o.Contains("CssDetailSelectionL1")){
					_cssDetailSelectionL1 = o["CssDetailSelectionL1"].Value; 
					_cssDetailSelectionL1 = _cssDetailSelectionL1.Replace("\"","");
				}
				if(o.Contains("CssDetailSelectionL2")){
					_cssDetailSelectionL2 = o["CssDetailSelectionL2"].Value; 
					_cssDetailSelectionL2 = _cssDetailSelectionL2.Replace("\"","");
				}
				if(o.Contains("CssDetailSelectionL3")){
					_cssDetailSelectionL3 = o["CssDetailSelectionL3"].Value; 
					_cssDetailSelectionL3 = _cssDetailSelectionL3.Replace("\"","");
				}
                if (o.Contains("CssDetailSelectionRightBorderL1")) {
                    _cssDetailSelectionRightBorderL1 = o["CssDetailSelectionRightBorderL1"].Value;
                    _cssDetailSelectionRightBorderL1 = _cssDetailSelectionRightBorderL1.Replace("\"", "");
                }
                if (o.Contains("CssDetailSelectionRightBorderL2")) {
                    _cssDetailSelectionRightBorderL2 = o["CssDetailSelectionRightBorderL2"].Value;
                    _cssDetailSelectionRightBorderL2 = _cssDetailSelectionRightBorderL2.Replace("\"", "");
                }
                if (o.Contains("CssDetailSelectionRightBorderL3")) {
                    _cssDetailSelectionRightBorderL3 = o["CssDetailSelectionRightBorderL3"].Value;
                    _cssDetailSelectionRightBorderL3 = _cssDetailSelectionRightBorderL3.Replace("\"", "");
                }
                if (o.Contains("CssDetailSelectionRightBottomBorderL1")) {
                    _cssDetailSelectionRightBottomBorderL1 = o["CssDetailSelectionRightBottomBorderL1"].Value;
                    _cssDetailSelectionRightBottomBorderL1 = _cssDetailSelectionRightBottomBorderL1.Replace("\"", "");
                }
                if (o.Contains("CssDetailSelectionRightBottomBorderL2")) {
                    _cssDetailSelectionRightBottomBorderL2 = o["CssDetailSelectionRightBottomBorderL2"].Value;
                    _cssDetailSelectionRightBottomBorderL2 = _cssDetailSelectionRightBottomBorderL2.Replace("\"", "");
                }
                if (o.Contains("CssDetailSelectionRightBottomBorderL3")) {
                    _cssDetailSelectionRightBottomBorderL3 = o["CssDetailSelectionRightBottomBorderL3"].Value;
                    _cssDetailSelectionRightBottomBorderL3 = _cssDetailSelectionRightBottomBorderL3.Replace("\"", "");
                }
				if(o.Contains("CssDetailSelectionTitleGlobal")){
					_cssDetailSelectionTitleGlobal = o["CssDetailSelectionTitleGlobal"].Value; 
					_cssDetailSelectionTitleGlobal = _cssDetailSelectionTitleGlobal.Replace("\"","");
				}
				if(o.Contains("CssDetailSelectionTitle")){
					_cssDetailSelectionTitle = o["CssDetailSelectionTitle"].Value; 
					_cssDetailSelectionTitle = _cssDetailSelectionTitle.Replace("\"","");
				}
				if(o.Contains("CssDetailSelectionTitleData")){
					_cssDetailSelectionTitleData = o["CssDetailSelectionTitleData"].Value; 
					_cssDetailSelectionTitleData = _cssDetailSelectionTitleData.Replace("\"","");
				}
				if(o.Contains("CssDetailSelectionBordelLevel")){
					_cssDetailSelectionBordelLevel = o["CssDetailSelectionBordelLevel"].Value; 
					_cssDetailSelectionBordelLevel = _cssDetailSelectionBordelLevel.Replace("\"","");
				}
				if(o.Contains("CssL1")){
					_cssL1 = o["CssL1"].Value; 
					_cssL1 = _cssL1.Replace("\"","");
				}
				if(o.Contains("BackgroudColorL1")){
					_backgroudColorL1 = o["BackgroudColorL1"].Value; 
					_backgroudColorL1 = _backgroudColorL1.Replace("\"","");
				}
				if(o.Contains("HighlightBackgroundColorL1")){
					_highlightBackgroundColorL1 = o["HighlightBackgroundColorL1"].Value; 
					_highlightBackgroundColorL1 = _highlightBackgroundColorL1.Replace("\"","");
				}
				if(o.Contains("HtmlCodeL1")){
					_htmlCodeL1 = o["HtmlCodeL1"].Value; 
					_htmlCodeL1 = _htmlCodeL1.Replace("\"","");
				}				
				if(o.Contains("CssL2")){
					_cssL2 = o["CssL2"].Value; 
					_cssL2 = _cssL2.Replace("\"","");
				}
				if(o.Contains("BackgroudColorL2")){
					_backgroudColorL2 = o["BackgroudColorL2"].Value; 
					_backgroudColorL2 = _backgroudColorL2.Replace("\"","");
				}
				if(o.Contains("HighlightBackgroundColorL2")){
					_highlightBackgroundColorL2 = o["HighlightBackgroundColorL2"].Value; 
					_highlightBackgroundColorL2 = _highlightBackgroundColorL2.Replace("\"","");
				}
				if(o.Contains("HtmlCodeL2")){
					_htmlCodeL2 = o["HtmlCodeL2"].Value; 
					_htmlCodeL2 = _htmlCodeL2.Replace("\"","");
				}	
				if(o.Contains("CssL3")){
					_cssL3 = o["CssL3"].Value;
					_cssL3 = _cssL3.Replace("\"","");
				}
				if(o.Contains("BackgroudColorL3")){
					_backgroudColorL3 = o["BackgroudColorL3"].Value; 
					_backgroudColorL3 = _backgroudColorL3.Replace("\"","");
				}
				if(o.Contains("HighlightBackgroundColorL3")){
					_highlightBackgroundColorL3 = o["HighlightBackgroundColorL3"].Value; 
					_highlightBackgroundColorL3 = _highlightBackgroundColorL3.Replace("\"","");
				}
				if(o.Contains("HtmlCodeL3")){
					_htmlCodeL3 = o["HtmlCodeL3"].Value; 
					_htmlCodeL3 = _htmlCodeL3.Replace("\"","");
				}	
				if(o.Contains("CssL4")){
					_cssL4 = o["CssL4"].Value; 
					_cssL4 = _cssL4.Replace("\"","");
				}
				if(o.Contains("BackgroudColorL4")){
					_backgroudColorL4 = o["BackgroudColorL4"].Value; 
					_backgroudColorL4 = _backgroudColorL4.Replace("\"","");
				}
				if(o.Contains("HighlightBackgroundColorL4")){
					_highlightBackgroundColorL4 = o["HighlightBackgroundColorL4"].Value; 
					_highlightBackgroundColorL4 = _highlightBackgroundColorL4.Replace("\"","");
				}
				if(o.Contains("HtmlCodeL4")){
					_htmlCodeL4 = o["HtmlCodeL4"].Value; 
					_htmlCodeL4 = _htmlCodeL4.Replace("\"","");
				}	
				if(o.Contains("CssLTotal")){
					_cssLTotal = o["CssLTotal"].Value; 
					_cssLTotal = _cssLTotal.Replace("\"","");
				}
				if(o.Contains("ImgBtnCroiOverPath")){
					this.ImgBtnCroiOverPath = o["ImgBtnCroiOverPath"].Value.Replace("\"","");
				}
				if(o.Contains("ImgBtnDeCroiOverPath")){
					this.ImgBtnDeCroiOverPath = o["ImgBtnDeCroiOverPath"].Value.Replace("\"","");
				}
				if(o.Contains("ImgBtnCroiPath")){
					this.ImgBtnCroiPath = o["ImgBtnCroiPath"].Value.Replace("\"","");
				}
				if(o.Contains("ImgBtnDeCroiPath")){
					this.ImgBtnDeCroiPath = o["ImgBtnDeCroiPath"].Value.Replace("\"","");
				}
				if(o.Contains("CssTitle")){
					this.CssTitle = o["CssTitle"].Value.Replace("\"","");
				}
				if(o.Contains("IdTitleText")){
					this.IdTitleText = Int64.Parse(o["IdTitleText"].Value.Replace("\"",""));
				}

			}
		}
		#endregion

		#region GetData
		/// <summary>
		/// Obtention des tableaux à transmettre côté client
		/// </summary>
		/// <param name="idSession">Identifiant de session utilisateur</param>
        /// <param name="resultParameters">Tableaux de paramètres pour les paramètres du résultat</param>
        /// <param name="styleParameters">Tableaux de paramètres pour les styles</param>
        /// <param name="sortParameters">Tableaux de paramètres pour le tri</param>
		/// <returns>Tableau d'objet contenant ls différentes lignes (html) du tableau de résultat</returns>
		[AjaxPro.AjaxMethod]
        public object[] GetData(string idSession, AjaxPro.JavaScriptObject resultParameters,AjaxPro.JavaScriptObject styleParameters, AjaxPro.JavaScriptObject sortParameters){
			int j=0;
			object[] tab=null,  globalTable=null;
			int[] tabIndex=null;
				
			try {
                this.LoadResultParameters(resultParameters);
				_customerWebSession=(WebSession)WebSession.Load(idSession);
				_data = GetResultTable(_customerWebSession);
				if (_data != null){
					StringBuilder output=new StringBuilder(10000);
                    this.LoadStyleParameters(styleParameters);
                    this.LoadSortParameters(sortParameters);
					this.InitCss();
					long nbLineToSchow = 0;
					tab = GetHTMLTable(ref nbLineToSchow);//tableau des résultats (chaque ligne est en HTML)
					if(tab!=null){
						j++;

						tabIndex = GetTableIndex(_data, nbLineToSchow);
						if(tabIndex!=null && tabIndex.Length>0)
							j++;				

						globalTable = new object[j];
						globalTable[0]=tab;
						if(j>1)globalTable[1]=tabIndex;
					}
				}

				_customerWebSession.Save();
			}
			catch(System.Exception err) {
				string clientErrorMessage = OnAjaxMethodError(err,this._customerWebSession);
				throw new Exception(clientErrorMessage);
			}
			return(globalTable);
		}
		#endregion

		#region GetData (sans gestion de pagination)
		/// <summary>
		/// Obtention des tableaux à transmettre côté client
		/// </summary>
        /// <param name="idSession">Identifiant de session utilisateur</param>
        /// <param name="resultParameters">Tableaux de paramètres pour les paramètres du résultat</param>
        /// <param name="styleParameters">Tableaux de paramètres pour les styles</param>
        /// <param name="sortParameters">Tableaux de paramètres pour le tri</param>
		/// <returns>object</returns>
		[AjaxPro.AjaxMethod]
        public string GetDataWithoutPagination(string idSession, AjaxPro.JavaScriptObject resultParameters, AjaxPro.JavaScriptObject styleParameters, AjaxPro.JavaScriptObject sortParameters) {
			
			try {
                this.LoadResultParameters(resultParameters);
				_customerWebSession=(WebSession)WebSession.Load(idSession);
			
				_data = GetResultTable(_customerWebSession);	
				_customerWebSession.Save();

				if (_data !=null && _data.LinesNumber>0){
                    LoadStyleParameters(styleParameters);
                    LoadSortParameters(sortParameters);
					return GetHTML();
				}	
				return null;
			}
			catch(System.Exception err){
				return(OnAjaxMethodError(err,this._customerWebSession));
			}
			
		}
		#endregion

		#region GetDetailSelection
		/// <summary>
		/// Obtention du détail de sélection
		/// </summary>
        /// <param name="idSession">Identifiant de session utilisateur</param>
        /// <param name="webCtrlId"></param>
        /// <param name="resultParameters">Tableaux de paramètres pour les paramètres du résultat</param>
        /// <param name="styleParameters">Tableaux de paramètres pour les styles</param>
        /// <param name="sortParameters">Tableaux de paramètres pour le tri</param>
		/// <returns>Détail des sélections</returns>
		[AjaxPro.AjaxMethod]
        public string GetDetailSelection(string idSession, string webCtrlId, AjaxPro.JavaScriptObject resultParameters, AjaxPro.JavaScriptObject styleParameters, AjaxPro.JavaScriptObject sortParameters) {

			#region Variables
			//string htmlTab = "code html du rappel de sélection";
			string htmlTab = String.Empty;
			StringBuilder html= new StringBuilder(5000);
			#endregion
			
			#region Chargement de la session
			try {
				_customerWebSession=(WebSession)WebSession.Load(idSession);
			}
			catch(System.Exception err) {
				return(OnAjaxMethodError(err,this._customerWebSession));
			}
			#endregion

            #region Theme init
            string themeName = WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name;
            #endregion

            #region Chargement du code html du détail de sélection
            htmlTab = LoadHtmlDetailSelection(resultParameters, styleParameters, sortParameters);
			#endregion

			#region html du tableau du détail de sélection
			html.Append("<TABLE class=\"Control\" id=\"ExpandMenu1\" cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
			html.Append("<TR>");
			html.Append("<TD>");
			html.Append("<TABLE class=\"GrabCaption\" cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
			html.Append("<TR>");

			html.Append("<TD class=\"TopLine\" onmousedown=\"grab(event,'windowExpandMenu_"+webCtrlId+"');\" >&nbsp;"+GestionWeb.GetWebWord(870,_customerWebSession.SiteLanguage)+"</TD>");
            html.Append("<TD class=\"TopLine cursorHand\" width=\"15\" align=\"center\" onclick=\"javascript:masquer('windowExpandMenu_" + webCtrlId + "');\" ><IMG src=\"/App_Themes/" + themeName + "/Images/Common/Result/bt_close.gif\" border=\"0\"></TD>");

			html.Append("</TR>");
			html.Append("</TABLE>");
			html.Append("<TABLE id=\"ExpandMenu1:_ctl1\" cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
			html.Append("<TR>");
			html.Append("<TD>");
			            	
			html.Append("<TABLE class=\"MenuElement\" cellSpacing=\"0\" cellPadding=\"0\" width=\"650\" border=\"0\">");
			html.Append("<TR>");
			html.Append("<TD>"+htmlTab+"</TD>");
			html.Append("</TR>");
			html.Append("</TABLE>");
			              
			html.Append("</TD>");
			html.Append("</TR>");
			html.Append("</TABLE>");
			html.Append("<TABLE class=\"ExpandCaption\" onmouseover=\"selectControl(event,this);\" onclick=\"expand(event,this,'ExpandMenu1:_ctl1');\" onmouseout=\"deselectControl(event,this);\" cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
			html.Append("<TR>");
            html.Append("<TD class=\"BottomLine\" align=\"middle\"><IMG src=\"/App_Themes/" + themeName + "/Images/Common/Result/bt_arrow_down.gif\" ></TD>");
			html.Append("</TR>");
			html.Append("</TABLE>");
			html.Append("</TD>");
			html.Append("</TR>");
			html.Append("</TABLE>");
			#endregion

			return html.ToString();
        }

        #region Chargement du code html du détail de sélection
        /// <summary>
        /// Chargement du code html du détail de sélection
        /// </summary>
        /// <returns></returns>
        protected virtual string LoadHtmlDetailSelection(AjaxPro.JavaScriptObject resultParameters, AjaxPro.JavaScriptObject styleParameters, AjaxPro.JavaScriptObject sortParameters) {

            try {
                // Chargement des paramètres javascript
                this.LoadResultParameters(resultParameters);
                this.LoadStyleParameters(styleParameters);
                this.LoadSortParameters(sortParameters);

                // Composant DetailSelctionWebControl pour la récupération du code html
                TNS.AdExpress.Web.Controls.Selections.DetailSelectionWebControl detailSelectionWebControl = new TNS.AdExpress.Web.Controls.Selections.DetailSelectionWebControl();
                detailSelectionWebControl.WebSession = _customerWebSession;
                detailSelectionWebControl.PeriodBeginning = _customerWebSession.PeriodBeginningDate;
                detailSelectionWebControl.PeriodEnd = _customerWebSession.PeriodEndDate;
                detailSelectionWebControl.OutputType = _renderType;
                detailSelectionWebControl.CssLevel1 = CssDetailSelectionL1;
                detailSelectionWebControl.CssLevel2 = CssDetailSelectionL2;
                detailSelectionWebControl.CssLevel3 = CssDetailSelectionL3;
                detailSelectionWebControl.CssRightBorderLevel1 = CssDetailSelectionRightBorderL1;
                detailSelectionWebControl.CssRightBorderLevel2 = CssDetailSelectionRightBorderL2;
                detailSelectionWebControl.CssRightBorderLevel3 = CssDetailSelectionRightBorderL3;
                detailSelectionWebControl.CssRightBottomBorderLevel1 = CssDetailSelectionRightBottomBorderL1;
                detailSelectionWebControl.CssRightBottomBorderLevel2 = CssDetailSelectionRightBottomBorderL2;
                detailSelectionWebControl.CssRightBottomBorderLevel3 = CssDetailSelectionRightBottomBorderL3;
                detailSelectionWebControl.CssTitleGlobal = CssDetailSelectionTitleGlobal;
                detailSelectionWebControl.CssTitle = CssDetailSelectionTitle;
                detailSelectionWebControl.CssTitleData = CssDetailSelectionTitleData;
                detailSelectionWebControl.CssBorderLevel = CssDetailSelectionBordelLevel;
                return(detailSelectionWebControl.GetHeader());
            }
            catch (System.Exception err) {
                return (OnAjaxMethodError(err, this._customerWebSession));
            }
        
        }
        #endregion

        #endregion

        #endregion

        #region Evènements

        #region Load
        /// <summary>
		/// Chargement du composant
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnLoad(EventArgs e) {
			AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
			base.OnLoad (e);
		}
		#endregion

		#region Prérender
		/// <summary>
		/// Prérender
		/// </summary>
		/// <param name="e">Arguements</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender (e);
            DetailSelectionPreRender();
		}
        /// <summary>
        /// Prérender
        /// </summary>
        protected virtual void DetailSelectionPreRender() {
            switch (_renderType) {
                case RenderType.rawExcel:
                case RenderType.excel:
                    detailSelectionWebControl.WebSession = _customerWebSession;
                    detailSelectionWebControl.PeriodBeginning = _customerWebSession.PeriodBeginningDate;
                    detailSelectionWebControl.PeriodEnd = _customerWebSession.PeriodEndDate;
                    detailSelectionWebControl.OutputType = _renderType;
                    detailSelectionWebControl.CssLevel1 = CssDetailSelectionL1;
                    detailSelectionWebControl.CssLevel2 = CssDetailSelectionL2;
                    detailSelectionWebControl.CssLevel3 = CssDetailSelectionL3;
                    detailSelectionWebControl.CssRightBorderLevel1 = CssDetailSelectionRightBorderL1;
                    detailSelectionWebControl.CssRightBorderLevel2 = CssDetailSelectionRightBorderL2;
                    detailSelectionWebControl.CssRightBorderLevel3 = CssDetailSelectionRightBorderL3;
                    detailSelectionWebControl.CssRightBottomBorderLevel1 = CssDetailSelectionRightBottomBorderL1;
                    detailSelectionWebControl.CssRightBottomBorderLevel2 = CssDetailSelectionRightBottomBorderL2;
                    detailSelectionWebControl.CssRightBottomBorderLevel3 = CssDetailSelectionRightBottomBorderL3;
                    detailSelectionWebControl.CssTitleGlobal = CssDetailSelectionTitleGlobal;
                    detailSelectionWebControl.CssTitle = CssDetailSelectionTitle;
                    detailSelectionWebControl.CssTitleData = CssDetailSelectionTitleData;
                    detailSelectionWebControl.CssBorderLevel = CssDetailSelectionBordelLevel;
                    break;
            }
        }
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
			if(_customerWebSession==null){
				output.WriteLine("<table cellpadding=0 cellspacing=0 class=\"violetBackGround\">");
				output.WriteLine("<tr>");
				output.WriteLine("<td>Tableau résultat générique</td>");
				output.WriteLine("</tr>");
				output.WriteLine("</table>");
			}
			switch(_renderType){
				case RenderType.html:
					StringBuilder html=new StringBuilder(1000);
					html.Append(GetWindowExpandMenuHTML());
					html.Append(AjaxProTimeOutScript());
					html.Append(AjaxEventScript());
					html.Append(GetLoadingHTML());
					html.Append(SelectionCallBackScript());
					output.Write(html.ToString());
					break;
				case RenderType.rawExcel:
					_data=GetResultTable(_customerWebSession); 
					if(_data!=null){
						output.WriteLine(detailSelectionWebControl.GetLogo(_customerWebSession));
						output.WriteLine(detailSelectionWebControl.GetHeader());
						output.WriteLine(base.GetRawExcel());
						output.WriteLine(detailSelectionWebControl.GetFooter());
					}
					break;
				case RenderType.excel:
					_data=GetResultTable(_customerWebSession);
					if(_data!=null){
						output.WriteLine(detailSelectionWebControl.GetLogo(_customerWebSession));
						output.WriteLine(detailSelectionWebControl.GetHeader());
						output.WriteLine(base.GetExcel());
						output.WriteLine(detailSelectionWebControl.GetFooter());
					}
					break;
			}
		}
		#endregion

		#endregion		

		#region Méthodes

		#region GetHeadersLabels
		/// <summary>
		/// Méthode générant un tableau avec les libellés corresponfdant aux niveaux de nomenclatures dans leur ordre d'apparition et sans les niveaux totaux ou heades
		/// </summary>
		/// <returns></returns>
		public override string[] GetLevelHeadersLabels(){

			string[] tLabels = null;
			int i = 0;
			switch(_customerWebSession.CurrentModule){
				case Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE:
				case Constantes.Web.Module.Name.ALERTE_CONCURENTIELLE:
				case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE :
				case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
				case WebConstantes.Module.Name.ALERTE_POTENTIELS:
				case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:
				case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
					tLabels = new string[_customerWebSession.GenericProductDetailLevel.GetNbLevels];
					foreach(DetailLevelItemInformation level in _customerWebSession.GenericProductDetailLevel.Levels){
						tLabels[i] = level.Name;
						i++;
					}
					break;
				default:
					return base.GetLevelHeadersLabels();
			}
			return tLabels;
		}
		#endregion

		#region GetResultTable
		/// <summary>
		/// Obtient le tableau de résultats en fonction du module
		/// </summary>
		/// <param name="customerWebSession">Session du client</param>
		/// <returns>tableau de résultats</returns>
		protected virtual ResultTable GetResultTable(WebSession customerWebSession){
			
            //if(customerWebSession.CustomerLogin.Connection==null){
            //    TNS.FrameWork.DB.Common.IDataSource dataSource = new TNS.FrameWork.DB.Common.OracleDataSource(new OracleConnection(customerWebSession.CustomerLogin.OracleConnectionString));
            //    customerWebSession.CustomerLogin.Connection=(OracleConnection)dataSource.GetSource();
            //}
            object[] param = null;
            Domain.Web.Navigation.Module module=customerWebSession.CustomerLogin.GetModule(customerWebSession.CurrentModule);
			switch(customerWebSession.CurrentModule){
				case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
				case WebConstantes.Module.Name.ALERTE_POTENTIELS:
					return WebBusinessFacade.Results.MarketShareSystem.GetResultTable(customerWebSession);
				case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE :
                    if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Lost/Won result"));
                    param = new object[1];
                    param[0] = customerWebSession;
                    LostWon.ILostWonResult lostWonResult = (LostWon.ILostWonResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                    return lostWonResult.GetResult();
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
				case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE :
                    if(module.CountryRulesLayer==null)throw(new NullReferenceException("Rules layer is null for the present/absent result"));
                    param=new object[1];                     
                    param[0]=customerWebSession;
                    PresentAbsent.IPresentAbsentResult presentAbsentResult = (PresentAbsent.IPresentAbsentResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                    return presentAbsentResult.GetResult();
					//return WebBusinessFacade.Results.CompetitorSystem.GetHtml(customerWebSession);
				case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS :
				case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES :
					return WebBusinessFacade.Results.SponsorshipSystem.GetResultTable(customerWebSession);
				case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE :
				case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE :
                    if(module.CountryRulesLayer==null)throw(new NullReferenceException("Rules layer is null for the portofolio result"));
                    object[] parameters=new object[1];                     
                    parameters[0]=customerWebSession;
                    Portofolio.IPortofolioResults portofolioResult=(Portofolio.IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory+@"Bin\"+module.CountryRulesLayer.AssemblyName,module.CountryRulesLayer.Class,false,BindingFlags.CreateInstance|BindingFlags.Instance|BindingFlags.Public,null,parameters,null,null,null);
                    //Portofolio.IResults result=(Portofolio.IResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory+@"Bin\"+module.CountryRulesLayer.AssemblyName,module.CountryRulesLayer.Class);
                    portofolioResult.GetResultTable();
					return WebBusinessFacade.Results.PortofolioSystem.GetResultTable(customerWebSession); 
				case WebConstantes.Module.Name.DONNEES_DE_CADRAGE :
					return WebBusinessFacade.Results.SectorDataSystem.GetHtml(customerWebSession);
				case WebConstantes.Module.Name.JUSTIFICATIFS_PRESSE :
					return WebBusinessFacade.Results.ProofSystem.GetResultTable(customerWebSession);
				default :

					return null;
			}
			
		}
		#endregion

		#region GetHTMLTable
		/// <summary>
		/// Obtient le tableau HTML à afficher
		/// </summary>
		/// <returns></returns>
		public object[] GetHTMLTable(ref long nbLineToSchow){

			int i=0,j=0;
			object[]tab = null;
			string temp = "";
			string lineStart="";
			object[]newtab=null;
			if(_data!=null && _data.LinesNumber>0){

				#region Tri des données
				int iCol = (int)this._data.GetHeadersIndexInResultTable(this._sSortKey);
				if (iCol >= 0 && !this._sortOrder.Equals(ResultTable.SortOrder.NONE)){
					this._data.Sort(this._sortOrder, iCol);
				}
				#endregion

				#region Génération du tableau
				tab = new object[_data.LinesNumber];
				

			
				//entêtes de tableau
				this._sJSSortMethod = "sort_"+this.ID;
				if (_data.NewHeaders!=null)
					temp += _data.NewHeaders.Render(this.ID, _cssLHeader,this._sImgCroiOverPath,this._sImgCroiPath,this._sImgDeCroiOverPath,this._sImgDeCroiPath,this._sJSSortMethod);
                // Lorsque le tableau ne contient pas une ligne total, on met le header dans la première case du tableau tab
                // et on met la premier ligne du résultat dans la deuxième case de tab, ainsi on n'affiche pas la première ligne du résultat
                // au début de chaque page de pagination
                if (temp.Length > 0 && _data.LinesNumber > 0 && ((LineStart)_data[0, 0]).LineType != LineType.total) {
                    tab = new object[_data.LinesNumber + 1];
                    tab[nbLineToSchow] = temp;
                    nbLineToSchow++;
                    temp = "";
                    newtab = new object[nbLineToSchow];
                    Array.Copy(tab, newtab, nbLineToSchow);
                }
				try{
					for(i = 0; i < _data.LinesNumber; i++) {
						//Utilisation des styles au niveau des balises <TR>
						lineStart= _data[i,0].Render();
						if(lineStart.Length==0)continue;
						temp +=lineStart;
						for (j = 1; j < _data.ColumnsNumber-1; j++) {
					
							if(j==_data.LevelColumn) {
								try {	                            
									temp+=_data[i,j].Render();
								}
								catch(System.Exception) {
									temp+=_data[i,j].Render();
								}
							}
							else {
								temp+= _data[i,j].Render();
							}
						}
						temp+=_data[i,_data.ColumnsNumber-1].Render();

						tab[nbLineToSchow] = temp;
						nbLineToSchow++;
						temp ="";
						newtab=new object[nbLineToSchow];
						Array.Copy(tab,newtab,nbLineToSchow);

					}
				}
				catch(System.Exception err){
					throw(new System.Exception(err.Message+" Impossible de rendre la cellule ["+i+","+j+"]"));
				}
				#endregion
			}
			return newtab;
		}
		#endregion		

		#region Construit  tableau index pour répétition du niveau parent
		/// <summary>
		/// Obtient le tableau d'index
		/// </summary>
		/// <param name="data">Tableau de données à traiter</param>
		/// <param name="nbLineToSchow">Nombre de ligne affichée</param>
		/// <returns>tableau d'index</returns>
		private int[] GetTableIndex(ResultTable data, long nbLineToSchow){
		
			int[] tab;
			int nbLinesType=data.LinesStart.Count;

			tab = new int[nbLineToSchow];

			int[] tLevIndex = new int[nbLinesType];
			for(int i = 0; i < tLevIndex.Length; i++){
				tLevIndex[i] = -1;
			}

			LineStart lStart = null;
			CellLevel curLevel = null;
			int tabLine = 0;
			for(int i = 0; i < data.LinesNumber; i++){
				lStart = data.GetLineStart(i);
				if (lStart.LineType==LineType.total){
					tab[tabLine]=0;
					tabLine++;
				}
				else if (!(lStart is LineHide)){
					//recherche du niveau
					for(int j=1;j<data.ColumnsNumber - 2;j++){
						curLevel = data[i,j] as CellLevel;
						if (curLevel != null){
							tLevIndex[curLevel.Level] = tabLine;
							if (curLevel.Level > 1){
								tab[tabLine] = tLevIndex[curLevel.Level-1];
							}
							else{
								tab[tabLine] = 0;
							}
							tabLine++;
							break;
						}
					}
				}
				
			}


			return tab;
		}
		#endregion

		#region GetLoadingHTML
		/// <summary>
		/// Obtient le code HTML du loading
		/// </summary>
		/// <returns></returns>
		protected string GetLoadingHTML(){
            string themeName = WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name;

            return ("<div align=\"center\" id=\"res_" + this.ID + "\"><img src=\"/App_Themes/" + themeName + "/Images/Common/waitAjax.gif\"></div>");
			
		}

		#endregion

		#region Rappel selection
		/// <summary>
		/// Obtient le code HTML du rappel des sélection
		/// </summary>
		/// <returns></returns>
		protected string GetWindowExpandMenuHTML(){
            string themeName = WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name;

            return ("<div class=Window align=\"center\" id=\"windowExpandMenu_" + this.ID + "\" style=\"display:none; POSITION: absolute; width: 200px;\"><img src=\"/App_Themes/" + themeName + "/Images/Common/waitAjax.gif\"></div>");
		}
		#endregion

		#region GetHtmlTitle()
		/// <summary>
		/// Pour récupérer le code html du titre
		/// </summary>
		/// <returns></returns>
		protected override string GetHtmlTitle(){ 
			if(_idTitleText>0){
				_title = GestionWeb.GetWebWord(_idTitleText,CustomerWebSession.SiteLanguage);
				return(base.GetHtmlTitle());
			}
			else
				return("");
		} 
		#endregion

		/// <summary>
		/// Message d'erreur
		/// </summary>
		/// <param name="customerSession">Session du client</param>
		/// <param name="code">Code message</param>
		/// <returns>Message d'erreur</returns>
		protected string GetMessageError(WebSession customerSession, int code){
			string errorMessage="<div align=\"center\" class=\"txtViolet11Bold\">";
			if(customerSession!=null)
				errorMessage += GestionWeb.GetWebWord(code,customerSession.SiteLanguage)+". "+GestionWeb.GetWebWord(2099,customerSession.SiteLanguage);			
			else
				errorMessage += GestionWeb.GetWebWord(code,33)+". "+GestionWeb.GetWebWord(2099,33);
			
			errorMessage +="</div>";
			return errorMessage;
		}

		#endregion
	}
}
