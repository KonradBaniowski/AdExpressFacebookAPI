#region Information
// Auteur : D. Mussuma
// Créé le : 26/03/2007
// Modifié le :
#endregion

using System;
using System.Data;
using System.Text;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;

using WebBusinessFacade=TNS.AdExpress.Web.BusinessFacade;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using CstClassification = TNS.AdExpress.Constantes.Classification;

using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Web.DataAccess.Results;

using WebFunctions = TNS.AdExpress.Web.Functions;
using ControlsFunctions = TNS.AdExpress.Web.Controls.Functions;

using TNS.FrameWork;
using TNS.FrameWork.Date;
using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;

using Oracle.DataAccess.Client;

using AjaxPro;

namespace TNS.AdExpress.Web.Controls.Results
{
	/// <summary>
	/// Génère le code HTML des tableaux présentants le détails des insertions d'un média
	/// </summary>
	[ToolboxData("<{0}:MediaInsertionsCreationsResultsWebControl runat=server></{0}:MediaInsertionsCreationsResultsWebControl>")]
	public class MediaInsertionsCreationsResultsWebControl : System.Web.UI.WebControls.WebControl
	{
		#region Constantes
		/// <summary>
		/// Jpeg text
		/// </summary>
		private const string JPEG_TEXT="JPEG";
		/// <summary>
		/// Gif text
		/// </summary>
		private const string GIF_TEXT="GIF";
		/// <summary>
		/// Flash text
		/// </summary>
		private const string FLASH_TEXT="SWF";
		/// <summary>
		/// CSS texte de la bannière publicitaire
		/// </summary>
		private const string BANNER_CSS_TEXT = "txtViolet11";
		/// <summary>
		/// CSS lien de la bannière publicitaire
		/// </summary>
		private const string BANNER_CSS_LINK = "roll06";
		#endregion

		#region Variables
		/// <summary>
		/// Timeout des scripts utilisés par AjaxPro
		/// </summary>
		protected int _ajaxProTimeOut = 120;
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession _customerWebSession = null;
		
		/// <summary>
		/// Liste des paramètres postés par le navigateur
		/// </summary>				
		protected string _ids = null;

		/// <summary>
		/// La période sélectionnée
		/// </summary>
		protected string _zoomDate = null;

		/// <summary>
		/// Identifiant du média courant
		/// </summary>
		protected string _idVehicle;

		/// <summary>
		/// Indique s'il faut afficher le bouton de rappel de sélection
		/// </summary>
		protected bool _withSelectionCallBack = false;

		#endregion
	
		#region Propriétés

		#region Propriétés de Pagination

		/// <summary>
		/// Nombre par defaut de ligne dans une page
		/// </summary>
		private int _defaultPageSize = 10;

		/// <summary>
		/// Nombre par defaut de ligne dans une page
		/// </summary>
		[Bindable(true), 
		Category("Paging"), 
		DefaultValue(10),
		Description("Nombre par defaut de ligne dans une page")] 
		public int DefaultPageSize {
			get {return _defaultPageSize;}
			set {_defaultPageSize = value;}
		}

		/// <summary>
		/// Obtient  / définit les options de pagination
		/// </summary>
		private string _pageSizeOptions = "10,15,20"; 

		/// <summary>
		/// Obtient  / définit les options de pagination
		/// </summary>
		[Bindable(true), 	
		Category("Paging"),
		DefaultValue("10")] 
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

		#region Propriétés pour utilisation Ajax
		/// <summary>
		/// Obtient ou définit le Timeout des scripts utilisés par AjaxPro
		/// </summary>
		[Bindable(true), 
		Category("Ajax"),
		Description("Timeout des scripts utilisés par AjaxPro"),
		DefaultValue("120")]
		public int AjaxProTimeOut{
			get{return _ajaxProTimeOut;}
			set{_ajaxProTimeOut=value;}
		}
		#endregion

		#region Propriétés pour session client
		/// <summary>
		/// Obtient ou définit la Sesion du client
		/// </summary>
		[Bindable(false)] 
		public WebSession CustomerWebSession {
			get{return(_customerWebSession);}
			set{_customerWebSession = value;}
		}
		#endregion

		#region RenderType


		///<summary>
		/// Type de rendu
		/// </summary>
		///  <property />
		///  <property />
		///  <property />
		///  <property />
		///  <property />
		///  <property />
		protected RenderType _renderType=RenderType.html; 
		/// <summary>
		/// Type de rendu
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("RenderType.html"),
		Description("Type de rendu")] 
		public RenderType OutputType {
			get{return _renderType;}
			set{_renderType = value;}
		}
		#endregion

		/// <summary>
		/// La liste des paramètres postès^par le navigateur
		/// </summary>
		[Bindable(false)]
		public string Ids {
			set{_ids = value;}
		}

		/// <summary>
		/// Définit la période sélectionnée
		/// </summary>
		[Bindable(false)]
		public string ZoomDate {
			get{return _zoomDate;}
			set{_zoomDate=value;}
		}

		/// <summary>
		/// Définit l'identifiant du média courant
		/// </summary>
		[Bindable(false)]
		public string IdVehicle{
			set{_idVehicle = value;}
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

		#region AjaxEventScript
		/// <summary>
		/// Génération des fonctions javascript nécessaires au bon fonctionnement du control
		/// </summary>
		/// <returns>Code Javascript</returns>
		protected  string AjaxEventScript(){
			StringBuilder js=new StringBuilder(3000);

			js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
			
			#region Variables Globales

			js.Append("\r\n var html;");
			js.Append("\r\n var globalTable,tabIndex,tab;");
			js.Append("\r\n var oN;");	
			js.Append("\r\n var pageCount = 0, pageSize = 0; minPageSize =0;");
			js.Append("\r\n var numberIndexPage = 0;");
			js.Append("\r\n var currentPageIndex  = 1, leftPageIndex  = 0, rightPageIndex  = 0;");
			js.Append("\r\n var tempIndex;");			
			js.Append("\r\n var isShowParenHeader = false,  withHeader = false;");
			js.Append("\r\n var pageSizeOptionsList='10,15,20';");
			js.Append("\r\n\t var o = new Object();");		
			js.Append("\r\n\n SetInsertionsParameters(o);"); 
			js.Append("\r\n\n var detailSelectionHtml = null;"); 
		
			#endregion

			//Préchargement des images des boutons de pagination
			ControlsFunctions.Scripts.PreLoadImages(this.ID,js);
			

			#region Get_ResultWebControl()
			js.Append("\r\n function get_"+this.ID+"(){");
			js.Append("\r\n\t oN=document.getElementById('res_"+this.ID+"');");
			js.Append("\r\n\t oN.innerHTML='"+GetLoadingHTML()+"';");
							
			if(_allowPaging)
				js.Append("\r\n\t "+this.GetType().Namespace+"."+this.GetType().Name+".GetData('"+_customerWebSession.IdSession+"',o,get_"+this.ID+"_callback);");
			else
				js.Append("\r\n\t "+this.GetType().Namespace+"."+this.GetType().Name+".GetDataWithoutPagination('"+_customerWebSession.IdSession+"',o,get_"+this.ID+"_withoutpagination_callback);");

	
			js.Append("\r\n\t }");
			#endregion

			#region get_ResultWebControl_withoutpagination_callback(res)
			js.Append("\r\n function get_"+this.ID+"_withoutpagination_callback(res){");
			js.Append("\r\n\t oN=document.getElementById('res_"+this.ID+"');");
			
			js.Append("\r\n\t\t var sb = new StringBuilder();");							
			js.Append("\r\n\t if(res!=null && res.value != null){ ");
			js.Append("\r\n\t\t sb.append('<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td class=\"nav\" height=\"27\" align=\"left\" background=\"/Images/Common/Result/header.gif\">');");				
			js.Append("\r\n\t\t sb.append(GetNavigationBar('isUp'));");	
			js.Append("\r\n\t\t sb.append('</td></tr>');");
			js.Append("\r\n\t\t sb.append('<tr> <td align=\"center\" style=\"padding:10px;border-left-color:#644883; border-left-width:1px; border-left-style:solid;border-right-color:#644883; border-right-width:1px; border-right-style:solid;\">');");			
			js.Append("\r\n\t\t sb.append(res.value);");
			js.Append("\r\n\t\t sb.append('</td></tr>');");		
			js.Append("\r\n\t\t sb.append('<tr><td class=\"nav\" height=\"27\" align=\"left\" background=\"/Images/Common/Result/footer.gif\">');");	
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
			js.Append("\r\n\t   var cook = GetCookie(\""+TNS.AdExpress.Constantes.Web.Cookies.CurrentMediaInsertionsPageSize+"\"); ");

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
			
			js.Append("\r\n\t globalTable = null;");
			js.Append("\r\n\t res = null;");
			js.Append("\r\n\t }");

			js.Append("\r\n\t else{");
			js.Append("\r\n\t oN.innerHTML='<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,_customerWebSession.SiteLanguage)+"</div>';");
			js.Append("\r\n\t }\r\n");
			js.Append("\r\n\t }\r\n");
			#endregion			

			js.Append("\r\naddEvent(window, \"load\", get_"+this.ID+");");

			//Fonction Page de résultat
			js.Append(ControlsFunctions.Scripts.GetResultPage());
		
			// Fonction pagination		
			js.Append(ControlsFunctions.Scripts.Paginate());					

			// Fonction Barre de navigation
			js.Append(ControlsFunctions.Scripts.GetNavigationBar(this.ID,_customerWebSession,_withSelectionCallBack));

			//Fonction options nombre de lignes par page
			js.Append(ControlsFunctions.Scripts.PageSizeOptions(WebConstantes.Cookies.CurrentMediaInsertionsPageSize,_customerWebSession));
						
			//Fonction Option ajout en-tête parent
			js.Append(ControlsFunctions.Scripts.HeaderParentOption(WebConstantes.Cookies.IsShowMediaInsertionsParenHeader,_customerWebSession));			

			//Fonction ajout de(s) ligne(s) parent
			js.Append(ControlsFunctions.Scripts.GetParentLines());
						

			#region  Fonction Paramètres des insertions
			js.Append("\r\n\nfunction SetInsertionsParameters(obj){");
			js.Append("\r\n\t obj.IdVehicle = '"+_idVehicle+"';");
			js.Append("\r\n\t obj.Ids = '"+_ids+"';");
            if (_zoomDate.Length < 1)
            {
                DateTime begin;
                begin = WebFunctions.Dates.getPeriodBeginningDate(_customerWebSession.PeriodBeginningDate, _customerWebSession.PeriodType);

                switch (_customerWebSession.DetailPeriod)
                {
                    case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                        _zoomDate = begin.ToString("YYYYMM");
                        break;
                    case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                        AtomicPeriodWeek tmp = new AtomicPeriodWeek(begin);
                        _zoomDate = string.Format("{0}{1}", tmp.Year, tmp.Week.ToString("0#"));
                        break;
                }
            }
			js.Append("\r\n\t obj.ZoomDate = '"+_zoomDate+"';");
			js.Append("\r\n }");
			#endregion

			#region Sorting
			js.Append("\r\n function sort_"+this.ID+"(id,params){");
			js.Append("\r\n var tab1 = params.split(\",\");");
			js.Append("\r\n o.Sort = tab1[1];");
			js.Append("\r\n o.Key = tab1[0];");
			js.Append("\r\n tab = null;");
			js.Append("\r\n tabIndex = null;");
			js.Append("\r\n get_"+this.ID+"();");
			js.Append("\r\n }");
			#endregion

			//Fonction StringBuilder
			js.Append(ControlsFunctions.Scripts.StringBuilder());			

			#region Cookies

			//Fonction setCookie
			js.Append(ControlsFunctions.Scripts.SetCookie());
			
			//Fonction GetCookie
			js.Append(ControlsFunctions.Scripts.GetCookie());

			//Fonction DeleteCookie
			js.Append(ControlsFunctions.Scripts.DeleteCookie());
			
			#endregion

			js.Append("\r\n-->\r\n</SCRIPT>");
			return(js.ToString());
		}
		
		#endregion

		#region [AjaxPro.AjaxMethod]

		#region Chargement des paramètres AjaxPro.JavaScriptObject et WebSession
		/// <summary>
		/// Charge les paramètres navigant entre le client et le serveur
		/// </summary>
		/// <param name="o">Tableau de paramètres javascript</param>
		protected void LoadParams(AjaxPro.JavaScriptObject o){
			if(o!=null){

				if(o.Contains("IdVehicle")){
					_idVehicle = o["IdVehicle"].Value.Replace("\"","");					
				}

				if(o.Contains("Ids")){
					_ids = o["Ids"].Value.Replace("\"",""); 					
				}

				if(o.Contains("ZoomDate")){
					_zoomDate = o["ZoomDate"].Value.Replace("\"",""); 					
				}

				
			}
		}
		#endregion

		#region GetData
		/// <summary>
		/// Obtention des tableaux à transmettre côté client
		/// </summary>
		/// <param name="idSession">Identifiant de session utilisateur</param>
		/// <param name="o">Tableaux de paramètres</param>
		/// <returns>Tableau d'objet contenant ls différentes lignes (html) du tableau de résultat</returns>
		[AjaxPro.AjaxMethod]
		public  object[] GetData(string idSession, AjaxPro.JavaScriptObject o ){
			int j=0;
			object[] tab=null,  globalTable = null;
			int[] tabIndex = null;
				
			try {

				_customerWebSession=(WebSession)WebSession.Load(idSession);
//				_data = GetResultTable(_customerWebSession);
//				if (_data != null){
//					StringBuilder output=new StringBuilder(10000);
					this.LoadParams(o);					
					long nbLineToSchow = 0;
					tab = GetHTMLTable(ref nbLineToSchow);//tableau des résultats (chaque ligne est en HTML)
					if(tab!=null){
						j++;

//						tabIndex = GetTableIndex(_data, nbLineToSchow);
						if(tabIndex!=null && tabIndex.Length>0)
							j++;				

						globalTable = new object[j];
						globalTable[0]=tab;
						if(j>1)globalTable[1]=tabIndex;
					}
//				}

				_customerWebSession.Save();
			}
			catch(System.Exception err) {
				string clientErrorMessage = ControlsFunctions.Errors.OnAjaxMethodError(err,this._customerWebSession);
				throw new Exception(clientErrorMessage);
			}
			return(globalTable);
		}
		#endregion

		#region GetData (sans gestion de pagination)
		/// <summary>
		/// Obtention des tableaux à transmettre côté client
		/// </summary>
		/// <returns>object</returns>
		[AjaxPro.AjaxMethod]
		public string GetDataWithoutPagination(string idSession, AjaxPro.JavaScriptObject o) {
			
			try {
				_customerWebSession = (WebSession)WebSession.Load(idSession);
		
//				_customerWebSession.Save();
//

				this.LoadParams(o);					
				return GetHTML();

			}
			catch(System.Exception err){
				return(ControlsFunctions.Errors.OnAjaxMethodError(err,this._customerWebSession));
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
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("openDownload")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"openDownload", WebFunctions.Script.OpenDownload());
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("openPressCreation")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "openPressCreation", WebFunctions.Script.OpenPressCreation());
		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
			
			switch(_renderType){
				case RenderType.html:
					StringBuilder html=new StringBuilder(1000);
//					html.Append(GetWindowExpandMenuHTML());
					html.Append(AjaxProTimeOutScript());
					html.Append(AjaxEventScript());
					html.Append(GetLoadingHTML());
					//html.Append(SelectionCallBackScript());
					output.Write(html.ToString());
					break;
				case RenderType.rawExcel:
//					_data = GetResultTable(_customerWebSession); 
//					if(_data!=null){
//						output.WriteLine(detailSelectionWebControl.GetHeader());
//						output.WriteLine(base.GetRawExcel());
//					}
					break;
				case RenderType.excel:
//					_data=GetResultTable(_customerWebSession);
//					if(_data!=null){
//						output.WriteLine(detailSelectionWebControl.GetHeader());
//						output.WriteLine(base.GetExcel());
//					}
					break;
			}
		}
		#endregion

		#endregion		
		
		#region Méthodes pubiques

		#region GetHTML 
		/// <summary>
		/// Génère le code HTML représentant le tableau de résultat
		/// </summary>
		/// <returns>Code Html</returns>
		public string GetHTML(){
						
			string[] ids = null;
			string result  = null;
			bool uiEmpty = false;//TODO gérer l'affichage de choix des niveaux de détail en lignes

			if(_ids !=null){
				ids = _ids.Split(',');
			}		
			
			if(_customerWebSession.CurrentModule==WebConstantes.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE){
				_customerWebSession.PreformatedMediaDetail = WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia;
			}
			
			if(_customerWebSession.CurrentModule==WebConstantes.Module.Name.ALERTE_PORTEFEUILLE){
				if(_customerWebSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.NOVELTY){
                    System.Windows.Forms.TreeNode tree = new System.Windows.Forms.TreeNode();
					tree.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.productAccess,long.Parse(ids[4]),"");
					_customerWebSession.ProductDetailLevel=new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.product,tree);
				}
			}

			try{				
				result = MediaInsertionsCreationsResultsUI.GetMediaInsertionsCreationsResultsUI(_customerWebSession,ids[0], ids[1], ids[2], ids[3],ids[4],this.Page,_idVehicle,_zoomDate,ref uiEmpty);
		
			}
			catch(System.Exception err){
				throw(new System.Exception(err.Message+" Impossible de rendre le résultat"));
			}
			
			return (result); 
		}
		#endregion

		#region GetHTMLTable
		/// <summary>
		/// Obtient le tableau HTML à afficher
		/// </summary>
		/// <returns></returns>
		public object[] GetHTMLTable(ref long nbLineToSchow){
			
			object[] newtab = null;
			string[] ids = null;
			ListDictionary mediaImpactedList = null;
			DataSet ds = null;
			int dateBegin = 0;
			int dateEnd = 0;
			TNS.AdExpress.Web.Controls.Results.BannerInformationWebControl dn;
			
			int i = 0;

            #region Cas du media Internet
            if ((CstClassification.DB.Vehicles.names)int.Parse(_idVehicle) == CstClassification.DB.Vehicles.names.adnettrack) { 
                newtab = new object[1]; 
				newtab[0] =  GetUIEmpty(_customerWebSession.SiteLanguage,2244);
		    	return newtab;
            }
            #endregion

            #region Mise en forme des dates et du media
            WebConstantes.CustomerSessions.Period.Type periodType = _customerWebSession.PeriodType;
            string periodBegin = _customerWebSession.PeriodBeginningDate;
            string periodEnd = _customerWebSession.PeriodEndDate;

            if (ZoomDate != null && ZoomDate.Length > 0)
            {
                dateBegin = Convert.ToInt32(
                    WebFunctions.Dates.Max(WebFunctions.Dates.getPeriodBeginningDate(ZoomDate, periodType),
                        WebFunctions.Dates.getPeriodBeginningDate(periodBegin, periodType)).ToString("yyyyMMdd")
                    );
                dateEnd = Convert.ToInt32(
                    WebFunctions.Dates.Min(WebFunctions.Dates.getPeriodEndDate(ZoomDate, periodType),
                        WebFunctions.Dates.getPeriodEndDate(periodEnd, periodType)).ToString("yyyyMMdd")
                    );
            }
            else
            {
                dateBegin = Convert.ToInt32(WebFunctions.Dates.getPeriodBeginningDate(periodBegin, periodType).ToString("yyyyMMdd"));
                dateEnd = Convert.ToInt32(WebFunctions.Dates.getPeriodEndDate(periodEnd, periodType).ToString("yyyyMMdd"));
            }
            #endregion

			if(_ids !=null){
				ids = _ids.Split(',');
			}
			
			if(_customerWebSession.CurrentModule==WebConstantes.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE){
				_customerWebSession.PreformatedMediaDetail = WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia;
			}
			
			if(_customerWebSession.CurrentModule==WebConstantes.Module.Name.ALERTE_PORTEFEUILLE){
				if(_customerWebSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.NOVELTY){
                    System.Windows.Forms.TreeNode tree = new System.Windows.Forms.TreeNode();
					tree.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.productAccess,long.Parse(ids[4]),"");
					_customerWebSession.ProductDetailLevel=new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.product,tree);
				}
			}

			if( _customerWebSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_INTERNET_ACCESS_FLAG]!=null){
				
				mediaImpactedList = WebFunctions.MediaDetailLevel.GetImpactedMedia(_customerWebSession,long.Parse(ids[0]),long.Parse(ids[1]),long.Parse(ids[2]),long.Parse(ids[3]));	
				ds = MediaCreationDataAccess.GetAdNetTrackData(_customerWebSession,mediaImpactedList,dateBegin,dateEnd,long.Parse(_idVehicle));
			
				if(ds != null && ds.Tables[0].Rows.Count>0){

				
				
					newtab = new object[ds.Tables[0].Rows.Count]; 
					foreach(DataRow dr in ds.Tables[0].Rows){
											
						dn = new TNS.AdExpress.Web.Controls.Results.BannerInformationWebControl();
						dn.FilePath = dr["ASSOCIATED_FILE"].ToString();
						dn.Dimension = dr["dimension"].ToString();
						switch(dr["format"].ToString()){
							case GIF_TEXT : 
								dn.FormatId = 0;
								break;
							case JPEG_TEXT :
								dn.FormatId = 1;
								break;
							case FLASH_TEXT :
								dn.FormatId = 3;
								break;
						}
					
						dn.LinkBanner = dr["url"].ToString();
						dn.ProductLabel = dr["product"].ToString();
						dn.ProductLId = Int64.Parse(dr["id_product"].ToString());
						dn.AdvertiserLabel = dr["advertiser"].ToString();
						dn.AdvertiserLId = Int64.Parse(dr["id_advertiser"].ToString());
						dn.Hashcode = Int64.Parse(dr["hashcode"].ToString());
						dn.CustomerWebSession = _customerWebSession;
						dn.CssText = BANNER_CSS_TEXT;
						dn.CssLink = BANNER_CSS_LINK;
						dn.ZoomDate = _zoomDate;
						dn.UrlParameters = _ids;
						newtab[i] =  "<tr width=\"100%\"><td>"+dn.GetRender()+"</td></tr>";	
					
						i++;
					}				
				}
			}else{
				newtab = new object[1]; 
				newtab[0] =  GetUIEmpty(_customerWebSession.SiteLanguage,2158);
			}
			return newtab;
		}
		#endregion

		#endregion

		#region Méthodes privées

		#region GetLoadingHTML
		/// <summary>
		/// Obtient le code HTML du loading
		/// </summary>
		/// <returns></returns>
		protected string GetLoadingHTML(){
		
			return("<div align=\"center\" id=\"res_"+this.ID+"\"><img src=\"/Images/Common/waitAjax.gif\"></div>");
			
		}

		#endregion

		#region Rappel selection
		/// <summary>
		/// Obtient le code HTML du rappel des sélection
		/// </summary>
		/// <returns></returns>
		protected string GetWindowExpandMenuHTML(){
			return("<div class=Window align=\"center\" id=\"windowExpandMenu_"+this.ID+"\" style=\"display:none; POSITION: absolute; width: 200px;\"><img src=\"/Images/Common/waitAjax.gif\"></div>");
		}
		#endregion

		/// <summary>
		/// Génère le code html précisant qu'il n 'y a pas de données à afficher
		/// </summary>
		/// <param name="language">Langue du site</param>
		/// <param name="code">code traduction</param>
		/// <returns>Code html Généré</returns>
		public static string GetUIEmpty(int language,int code){
			StringBuilder HtmlTxt = new StringBuilder(10000);
			
			HtmlTxt.Append("<TR vAlign=\"top\" >");
			HtmlTxt.Append("<TD vAlign=\"top\">");
			HtmlTxt.Append("<TABLE vAlign=\"top\" cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
			HtmlTxt.Append("<tr><td></td></tr>");
			HtmlTxt.Append("<TR vAlign=\"top\">");
			HtmlTxt.Append("<TD vAlign=\"top\" height=\"50\" class=\"txtViolet11Bold\">");
			HtmlTxt.Append(GestionWeb.GetWebWord(code,language));			
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");
			HtmlTxt.Append("</TABLE>");
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");

			return HtmlTxt.ToString();
		}

		#endregion

	}
}
