#region Informations
// Auteur: G. Facon 
// Date de création: 13/07/2006
// Date de modification:
//		G Ragneau - 08/08/2006 - Set GetHtml as public so as to access it 
//		G Ragneau - 08/08/2006 - GetHTML : Force media plan alert module and restaure it after process (<== because of version zoom);
//		G Ragneau - 05/05/2008 - GetHTML : implement layers
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Reflection;
using AjaxPro;
using TNS.AdExpress.Web.Controls.Headers;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.UI.Results.MediaPlanVersions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using FrmFct = TNS.FrameWork.WebResultUI.Functions;
using TNS.FrameWork.Date;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.WebResultUI;
using ConstantePeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CustomCst = TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Domain.Classification;

using TNS.AdExpressI.MediaSchedule;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.Insertions;
namespace TNS.AdExpress.Web.Controls.Results.MediaPlan{
	/// <summary>
	/// Affiche le résultat d'une alerte plan media
	/// </summary>
	[DefaultProperty("Text"),
      ToolboxData("<{0}:GenericMediaScheduleWebControl runat=server></{0}:GenericMediaScheduleWebControl>")]
    public class GenericMediaScheduleWebControl : System.Web.UI.WebControls.WebControl
    {

		#region Variables
        /// <summary>
        /// Timeout des scripts utilisés par AjaxPro
        /// </summary>
        protected int _ajaxProTimeOut = 60;
        /// <summary>
        /// Session du client
        /// </summary>
        protected WebSession _customerWebSession = null;
		/// <summary>
		/// Indique si le composant doit montrer les versions
		/// </summary>
		private bool _showVersion=false;
		/// <summary>
		/// Zoom de La période sélectionnée
		/// </summary>
		protected string _zoomDate = string.Empty;
        /// <summary>
        /// Version Id
        /// </summary>
        protected long _versionId = -1;
        /// <summary>
        /// Method to vcall to refresh data
        /// </summary>
        protected string _refreshDataMethod = "refreshData";
        /// <summary>
        /// Theme name
        /// </summary>
        protected string _themeName = string.Empty;
        /// <summary>
        /// Current Module
        /// </summary>
        protected TNS.AdExpress.Domain.Web.Navigation.Module _module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA);
        #endregion

		#region Accesseurs
        
        #region AjaxProTimeOut
        /// <summary>
        /// Obtient ou définit le Timeout des scripts utilisés par AjaxPro
        /// </summary>
        [Bindable(true),
        Category("Ajax"),
        Description("Timeout des scripts utilisés par AjaxPro"),
        DefaultValue("60")]
        public int AjaxProTimeOut
        {
            get { return _ajaxProTimeOut; }
            set { _ajaxProTimeOut = value; }
        }
        #endregion

        #region CustomerWebSession
        /// <summary>
        /// Obtient ou définit la Sesion du client
        /// </summary>
        [Bindable(false)]
        public WebSession CustomerWebSession
        {
            get { return (_customerWebSession); }
            set
            {
                _customerWebSession = value;
            }
        }
        #endregion

        #region ShowVersion
        /// <summary>
		/// Obtient ou définit le mot recherché
		/// </summary>
		[Bindable(true), 
		Category("Appearance"),
		Description("Indique si le composant doit montrer les versions")]
		public bool ShowVersion{
			get{return _showVersion;}
			set{_showVersion=value;}
        }
        #endregion

        #region RenderType
        ///<summary>
		/// Type de rendu
		/// </summary>		
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

		/// <summary>
		///  Obtient ou définit Zoom date
		/// </summary>
		[Bindable(true),
		Category("Data"),
		DefaultValue(""),
		Description("Specify if component is in zoom mode.")] 
		public string ZoomDate {
			get{return _zoomDate;}
			set{_zoomDate=value;}
		}
		#endregion

        #region Refresh Data PAram
        /// <summary>
        ///  Obtient ou définit Zoom date
        /// </summary>
        [Bindable(true),
        Category("Data"),
        DefaultValue(""),
        Description("Specify the name of the method to call to refresh data.")]
        public string RefreshDataMethod
        {
            get { return _refreshDataMethod; }
        }
        #endregion

        #region Date Container Name
        /// <summary>
        ///  Obtient ou définit Zoom date
        /// </summary>
        [Bindable(true),
        Category("Data"),
        DefaultValue(""),
        Description("Specify the name of the name of the javascript var to containing zoom date.")]
        public string ZoomDateContainer
        {
            get { return "o_genericMediaSchedule.Zoom"; }
        }
        #endregion

        #region Theme name
        /// <summary>
        /// Set or Get the theme name
        /// </summary>
        public string ThemeName {
            get { return _themeName; }
            set { _themeName = value; }
        }
        #endregion

        #region Theme name
        /// <summary>
        /// Set or Get the current module
        /// </summary>
        public TNS.AdExpress.Domain.Web.Navigation.Module Module
        {
            get { return _module; }
            set { _module = value; }
        }
        #endregion
        #endregion

        #region Builder
        /// <summary>
        /// Default builder
        /// </summary>
        public GenericMediaScheduleWebControl()
        {
        }
        #endregion

        #region Javascript
        
        #region AjaxProTimeOutScript 
        /// <summary>
        /// Génère le code JavaSript pour ajuster le time out d'AjaxPro
        /// </summary>
        /// <returns>Code JavaScript</returns>
        private string AjaxProTimeOutScript()
        {
            StringBuilder js = new StringBuilder(100);
            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
            js.Append("\r\nAjaxPro.timeoutPeriod=" + _ajaxProTimeOut.ToString() + "*1000;");
            js.Append("\r\n-->\r\n</SCRIPT>");
            return (js.ToString());
        }
        #endregion

        #region AjaxEventScript
        /// <summary>
        /// Evenement Ajax
        /// </summary>
        /// <returns></returns>
        protected string  AjaxEventScript(){

            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n<SCRIPT language=javascript>\r\n");
            js.Append("\r\nfunction get_" + this.ID + "(){");
            js.Append("\r\n\tvar oN=document.getElementById('res_" + this.ID + "');");
            js.Append("\r\n\toN.innerHTML='" + GetLoadingHTML() + "';");
            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".GetData(o_genericMediaSchedule,get_" + this.ID + "_callback);");
            js.Append("\r\n}");

            js.Append("\r\nfunction get_" + this.ID + "_callback(res){");
            js.Append("\r\n\tvar oN=document.getElementById('res_" + this.ID + "');");
            js.Append("\r\n\toN.innerHTML=res.value;");
            js.Append("\r\n\tUpdateParameters(oN);");
            js.Append("\r\n}\r\n");
            js.Append("\r\naddEvent(window, \"load\", get_" + this.ID + ");");

            js.Append("\r\n\r\n</SCRIPT>");
            return (js.ToString());
        }
        #endregion

        #region AjaxVersionEventScript
        /// <summary>
        /// Generate Scripts required by the use of Ajax
        /// </summary>
        /// <returns></returns>
		private string AjaxVersionEventScript(){
			StringBuilder js=new StringBuilder(2000);
			js.Append("\r\n<SCRIPT language=javascript>\r\n");

            #region Update parameters
            js.Append("\r\n");
            js.Append("\r\nfunction UpdateParameters(obj){");
            js.Append("\r\n\tif (obj != null){");
            js.Append("\r\n\t\tvar AllScripts=obj.getElementsByTagName(\"script\");");
            js.Append("\r\n\t\tfor (var i=0; i<AllScripts.length; i++) {");
            js.Append("\r\n\t\t\tvar s=AllScripts[i];");
            js.Append("\r\n\t\t\teval(s.innerHTML);");
            js.Append("\r\n\t\t}");
            js.Append("\r\n\t}");
            js.Append("\r\n}");
            #endregion

            #region RefreshData
            js.Append("\r\nvar idVersion = -1;");
            js.AppendFormat("\r\nfunction {0}()", this._refreshDataMethod);
            js.Append("{\r\n\tif (idVersion > -1){");
            js.Append("\r\n\t\tget_version(idVersion);");
            js.Append("\r\n\t}else{");
            js.Append("\r\n\t\tget_" + this.ID + "();");
            js.Append("\r\n\t}");
            js.Append("\r\n}");
            #endregion

            #region get_back
            js.Append("\r\nfunction get_back(){");
            js.Append("\r\n\tidVersion = -1;");
			js.Append("\r\n\tvar oN=document.getElementById('res_"+this.ID+"');");
			js.Append("\r\n\toN.innerHTML='"+GetLoadingHTML()+"';");
            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".GetBack(o_genericMediaSchedule,get_back_callback);");
            js.Append("\r\n}");
            #endregion

            #region get_back_callback
            js.Append("\r\nfunction get_back_callback(res){");
            js.Append("\r\n\tvar oN=document.getElementById('res_" + this.ID + "');");
			js.Append("\r\n\toN.innerHTML=res.value;");
            js.Append("\r\n\tUpdateParameters(oN);");
            js.Append("\r\n}\r\n");
            #endregion

            #region get_version
            js.Append("\r\nfunction get_version(versionId){");
            js.Append("\r\n\tidVersion = versionId;");
            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".GetVersionData(versionId,o_genericMediaSchedule,get_version_callback);");
			js.Append("\r\n}");

			js.Append("\r\nfunction get_version_callback(res){");
            js.Append("\r\n\tvar oN=document.getElementById('res_" + this.ID + "');");
			js.Append("\r\n\toN.innerHTML=res.value;");
            js.Append("\r\n\tUpdateParameters(oN);");
            js.Append("\r\n}\r\n");
            #endregion

			js.Append("\r\n\r\n</SCRIPT>");
			return(js.ToString());
        }
        #endregion

        #endregion

        #region Implémentation des méthodes abstraites + Autres Ajax

        #region Chargement des paramètres AjaxPro.JavaScriptObject
        /// <summary>
        /// Charge les paramètres navigant entre le client et le serveur
        /// </summary>
        /// <param name="o">Tableau de paramètres javascript</param>
        protected void LoadParams(AjaxPro.JavaScriptObject o)
        {

            if (o != null)
            {

                if (o.Contains("IdSession"))
                {
                    this._customerWebSession = (WebSession)WebSession.Load(o["IdSession"].Value.Replace("\"", ""));
                }
                if (o.Contains("IdModule"))
                {
                    this._module = ModulesList.GetModule(Int64.Parse(o["IdModule"].Value.Replace("\"", "")));
                }

                if (o.Contains("Zoom"))
                {
                    _zoomDate = o["Zoom"].Value.Replace("\"", "");
                }

                if (o.Contains("themeName")) {
                    _themeName = o["themeName"].Value.Replace("\"", "");
                }

                if (o.Contains("versionKeys"))
                {
                    if (o["versionKeys"].Value.Replace("\"", "").Length > 0)
                    {
                        string[] keys = o["versionKeys"].Value.Replace("\"", "").Split(',');
                        string[] values = o["versionStyle"].Value.Replace("\"", "").Split(',');
                        for (int i = 0; i < keys.Length; i++)
                        {
                            if (!this._customerWebSession.SloganColors.ContainsKey(Int64.Parse(keys[i])))
                            {
                                this._customerWebSession.SloganColors.Add(Int64.Parse(keys[i]), values[i]);
                            }
                        }
                    }
                }

            }
        }
        #endregion

        #region GetData
        /// <summary>
        /// Obtention du code HTML à insérer dans le composant
        /// </summary>
        /// <param name="o">Result parameters (session Id, zoom...)</param>
        /// <returns>Code HTML</returns>
        [AjaxPro.AjaxMethod]
        public string GetData(AjaxPro.JavaScriptObject o)
        {
			string html;
			try{

                LoadParams(o);

                html=GetHTML(this.CustomerWebSession);

			}
			catch(System.Exception err){
                return (OnAjaxMethodError(err, this.CustomerWebSession));
			}
			return(html);
        }
        #endregion

        #region GetVersionData
        /// <summary>
		/// Obtention du code HTML à insérer dans le composant pour le zoom d'une version
		/// </summary>
		/// <param name="versionId">Identifiant de la version</param>
        /// <param name="o">Analysys parameters</param>
		/// <returns>Code HTML</returns>
		[AjaxPro.AjaxMethod]
		public string GetVersionData(string versionId,AjaxPro.JavaScriptObject o){

			string html;
			try{
                
                LoadParams(o);

                this.CustomerWebSession.SloganIdZoom = Int64.Parse(versionId);

                html = GetHTML(this.CustomerWebSession);

                this.CustomerWebSession.Save();

			}
			catch(System.Exception err){
                return (OnAjaxMethodError(err, this.CustomerWebSession));
			}

			return(html);

        }
        #endregion

        #region GetBack
        /// <summary>
		/// Obtention du code HTML avec annulationdu zoom d'une version
		/// </summary>
		/// <param name="o">Analysis parameters</param>
		/// <returns>Code HTML</returns>
		[AjaxPro.AjaxMethod]
		public string GetBack(AjaxPro.JavaScriptObject o){

			string html;
			try{
                
                LoadParams(o);

                this.CustomerWebSession.SloganIdZoom = long.MinValue;

                html = GetHTML(this.CustomerWebSession);
				
                this.CustomerWebSession.Save();
			}
			catch(System.Exception err){
                return (OnAjaxMethodError(err, this.CustomerWebSession));
			}

			return(html);

        }
        #endregion

        #endregion

        #region Evènements

        #region Initialisation
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        /// <summary>
        /// Appelé sur erreur à l'exécution des méthodes Ajax
        /// </summary>
        /// <param name="errorException">Exception</param>
        /// <param name="customerSession">Session utilisateur</param>
        /// <returns>Message d'erreur</returns>
        protected string OnAjaxMethodError(Exception errorException, WebSession customerSession)
        {
            TNS.AdExpress.Web.Exceptions.CustomerWebException cwe = null;
            try
            {
                BaseException err = (BaseException)errorException;
                cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException(err.Message, err.GetHtmlDetail(), customerSession);
            }
            catch (System.Exception)
            {
                try
                {
                    cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException(errorException.Message, errorException.StackTrace, customerSession);
                }
                catch (System.Exception es)
                {
                    throw (es);
                }
            }
            cwe.SendMail();
            return GetMessageError(customerSession, 1973);
        }
        #endregion

        #region Load
        /// <summary>
        /// Chargement du composant
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript1","/ajaxpro/prototype.ashx");
            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript2", "/ajaxpro/core.ashx");
            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript3", "/ajaxpro/converter.ashx");
            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript4", "/ajaxpro/TNS.AdExpress.Web.Controls.Results.MediaPlan.GenericMediaScheduleWebControl,TNS.AdExpress.Web.Controls.ashx");

            #region parameters
            StringBuilder js = new StringBuilder();
            js.Append("\r\n<SCRIPT language=javascript>\r\n");
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("IdSession", this.CustomerWebSession.IdSession);
            parameters.Add("Zoom", _zoomDate);
            parameters.Add("IdModule", _module.Id);
            parameters.Add("versionKeys", string.Empty);
            parameters.Add("versionStyle", string.Empty);
            parameters.Add("themeName", _themeName);
            js.Append(FrmFct.Scripts.GetAjaxParametersScripts("genericMediaSchedule", "o_genericMediaSchedule", parameters));
            js.Append("\r\n</SCRIPT>\r\n");
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AjaxScript4", js.ToString()); 
            #endregion



            base.OnLoad(e);
        }
        #endregion

        #region PréRender
        /// <summary>
		/// Prérendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) 
		{

			base.OnPreRender (e);
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("OpenInsertions")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenInsertions", WebFunctions.Script.OpenInsertions());
            if(!this.Page.ClientScript.IsClientScriptBlockRegistered("OpenCreatives")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"OpenCreatives",WebFunctions.Script.OpenCreatives());
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("openPressCreation")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "openPressCreation", WebFunctions.Script.OpenPressCreation());
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("OpenInternetCreation")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenInternetCreation", WebFunctions.Script.OpenInternetCreation());
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("Popup")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Popup", WebFunctions.Script.Popup());
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("openDownload")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "openDownload", WebFunctions.Script.OpenDownload());
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("AppmInsertions")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AppmInsertions", WebFunctions.Script.PopUpInsertion(false));
			
			
		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){

			base.Render(output);
            StringBuilder html = new StringBuilder(1000);
            html.Append(AjaxProTimeOutScript());
            html.Append(AjaxEventScript());
            html.Append(AjaxVersionEventScript());
            html.Append(GetLoadingHTML());
            output.Write(html.ToString());

		}
		#endregion

		#endregion

		#region Méthodes internes
		/// <summary>
		/// Calcul le résultat du plan media
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTMl</returns>
		public virtual string GetHTML(WebSession webSession){

			StringBuilder html=new StringBuilder(10000);
            StringBuilder periodNavigation = new StringBuilder(10000);
            MediaScheduleData result = null;
			//MediaPlanResultData result=null;
            MediaSchedulePeriod period = null;
			Int64 moduleId = webSession.CurrentModule;
            //object[,] tab = null;
            ConstantePeriod.DisplayLevel periodDisplay = webSession.DetailPeriod;
            object[] param = null;
            long oldCurrentTab = _customerWebSession.CurrentTab;
            System.Windows.Forms.TreeNode oldReferenceUniversMedia = _customerWebSession.ReferenceUniversMedia;
			try{
				
				//webSession.CurrentModule = WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA;

                #region Period Detail
                DateTime begin;
                DateTime end;
                if (_zoomDate != null && _zoomDate != string.Empty)
                {
                    if (webSession.DetailPeriod == ConstantePeriod.DisplayLevel.weekly)
                    {
                        begin = WebFunctions.Dates.getPeriodBeginningDate(_zoomDate, ConstantePeriod.Type.dateToDateWeek);
                        end = WebFunctions.Dates.getPeriodEndDate(_zoomDate, ConstantePeriod.Type.dateToDateWeek);
                    }
                    else
                    {
                        begin = WebFunctions.Dates.getPeriodBeginningDate(_zoomDate, ConstantePeriod.Type.dateToDateMonth);
                        end = WebFunctions.Dates.getPeriodEndDate(_zoomDate, ConstantePeriod.Type.dateToDateMonth);
                    }
                    begin = WebFunctions.Dates.Max(begin,
                        WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType));
                    end = WebFunctions.Dates.Min(end,
                        WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType));

                    webSession.DetailPeriod = ConstantePeriod.DisplayLevel.dayly;
                    if (webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                        period = new MediaSchedulePeriod(begin, end, ConstantePeriod.DisplayLevel.dayly, webSession.ComparativePeriodType);
                    else
                        period = new MediaSchedulePeriod(begin, end, ConstantePeriod.DisplayLevel.dayly);

                }
                else
                {
                    begin = WebFunctions.Dates.getPeriodBeginningDate(_customerWebSession.PeriodBeginningDate, _customerWebSession.PeriodType);
                    end = WebFunctions.Dates.getPeriodEndDate(_customerWebSession.PeriodEndDate, _customerWebSession.PeriodType);
                    if (webSession.DetailPeriod == ConstantePeriod.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                    {
                        webSession.DetailPeriod = ConstantePeriod.DisplayLevel.monthly;
                    }

                    if (webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                        period = new MediaSchedulePeriod(begin, end, webSession.DetailPeriod, webSession.ComparativePeriodType);
                    else
                        period = new MediaSchedulePeriod(begin, end, webSession.DetailPeriod);

                }
                #endregion

                //tab = TNS.AdExpress.Web.Rules.Results.GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(webSession, period, -1);
                if (_module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Media Schedule result"));
                if (_zoomDate.Length > 0)
                {
                    param = new object[3];
                    param[2] = _zoomDate;
                }
                else
                {
                    param = new object[2];
                }
                _customerWebSession.CurrentModule = _module.Id;
                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA) _customerWebSession.CurrentTab = 0;
                _customerWebSession.ReferenceUniversMedia = new System.Windows.Forms.TreeNode("media");
                param[0] = _customerWebSession;
                param[1] = period;
                IMediaScheduleResults mediaScheduleResult = (IMediaScheduleResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryRulesLayer.AssemblyName, _module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                mediaScheduleResult.Module = _module;
                result = mediaScheduleResult.GetHtml();

                #region Data
				if(result!=null && result.HTMLCode.Length>0){
                    				
					#region Construction du tableaux global
                    html.Append("<table width=100% align=\"left\" cellSpacing=\"0\" cellPadding=\"0\"  border=\"0\">");
                    html.Append("<tr><td class=\"nav\" height=\"27\" align=\"left\" background=\"/App_Themes/"+_themeName+"/Images/Common/Result/header.gif\">&nbsp;</td></tr>");
                    html.Append("<tr><td align=\"center\" style=\"padding:10px;\" class=\"MSVioletRightLeftBorder\">");
					html.Append("<table align=\"center\" cellSpacing=\"0\" cellPadding=\"0\"  border=\"0\">");
					#endregion

					#region Revenir aux versions sans zoom
                    TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.creativesUtilities];
                    if (cl == null) throw (new NullReferenceException("Core layer is null for the creatives utilities class"));
                    TNS.AdExpress.Web.Core.Utilities.Creatives creativesUtilities = (TNS.AdExpress.Web.Core.Utilities.Creatives)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null, null);


                    if (creativesUtilities.IsSloganZoom(webSession.SloganIdZoom))
                    {
                        html.Append("\r\n\t<tr align=\"left\" class=\"violetBackGroundV3\">\r\n\t\t<td>");
						//todo txt en BDD
						html.Append("<a class=\"roll06\" href=\"javascript:get_back();\" onmouseover=\"back_"+this.ID+".src='/App_Themes/"+_themeName+"/Images/Common/button/back_down.gif';\" onmouseout=\"back_"+this.ID+".src='/App_Themes/"+_themeName+"/Images/Common/button/back_up.gif';\"><img align=\"absmiddle\" name=\"back_"+this.ID+"\" border=0 src=\"/App_Themes/"+_themeName+"/Images/Common/button/back_up.gif\">&nbsp;" + GestionWeb.GetWebWord(1978 , webSession.SiteLanguage) + "</a>");
						html.Append("\r\n\t\t</td>\r\n\t</tr>");
					}
					#endregion

                    if (result.VersionsDetail.Count > 0) {
                        VersionsPluriMediaUI versionsUI = new VersionsPluriMediaUI(webSession, period, _zoomDate);
                        html.Append("\r\n\t<tr class=\"violetBackGroundV3\">\r\n\t\t<td>");
                        html.Append(versionsUI.GetMSCreativesHtml());
                        html.Append("\r\n\t\t</td>\r\n\t</tr>");
                    }

					html.Append("\r\n\t<tr height=\"1\">\r\n\t\t<td>");
					html.Append("\r\n\t\t</td>\r\n\t</tr>");
					html.Append("\r\n\t<tr>\r\n\t\t<td>");

					html.Append(result.HTMLCode);
					html.Append("\r\n\t\t</td>\r\n\t</tr>");
					html.Append("</table>");
                    html.Append("</td></tr>");
                    html.Append("<tr><td class=\"nav\" height=\"27\" align=\"left\" background=\"/App_Themes/"+_themeName+"/Images/Common/Result/footer.gif\">&nbsp;</td></tr>");
                    html.Append("</table>");

				}else{

					html.Append("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage));
					html.Append("<br><br></div>");

                }
                #endregion

                #region UpDate Parameters
                if (webSession.SloganColors != null && webSession.SloganColors.Count > 0)
                {
                    html.Append("\r\n<script language=javascript>\r\n");
                    bool first = true;
                    StringBuilder keys = new StringBuilder();
                    StringBuilder values = new StringBuilder();
                    keys.Append("\r\n\to_genericMediaSchedule.versionKeys='");
                    values.Append("\r\n\to_genericMediaSchedule.versionStyle='");
                    foreach (Int64 key in webSession.SloganColors.Keys)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            keys.Append(",");
                            values.Append(",");
                        }
                        keys.Append(key);
                        values.Append(webSession.SloganColors[key]);
                    }
                    keys.Append("';");
                    values.Append("';");
                    html.Append(keys);
                    html.Append(values);
                    html.Append("\r\n</script>\r\n");
                }
                #endregion

            }
			catch(System.Exception err){
				return(OnAjaxMethodError(err,webSession));
			}
			finally{
				webSession.CurrentModule = moduleId;
                webSession.DetailPeriod = periodDisplay;
                webSession.CurrentTab = oldCurrentTab;
                webSession.CurrentTab = oldCurrentTab;
                webSession.ReferenceUniversMedia = oldReferenceUniversMedia;
			}
			return(html.ToString());
		}

        /// <summary>
        /// Obtient le code HTML du loading
        /// </summary>
        /// <returns></returns>
        protected string GetLoadingHTML()
        {
            return ("<div id=\"res_" + this.ID + "\"><div align=\"center\" width = \"100%\"><img src=\"/App_Themes/"+_themeName+"/Images/Common/waitAjax.gif\"></div></div>");
        }

        /// <summary>
        /// Message d'erreur
        /// </summary>
        /// <param name="customerSession">Session du client</param>
        /// <param name="code">Code message</param>
        /// <returns>Message d'erreur</returns>
        protected string GetMessageError(WebSession customerSession, int code)
        {
            string errorMessage = "<div align=\"center\" class=\"txtViolet11Bold\">";
            if (customerSession != null)
                errorMessage += GestionWeb.GetWebWord(code, customerSession.SiteLanguage) + ". " + GestionWeb.GetWebWord(2099, customerSession.SiteLanguage);
            else
                errorMessage += GestionWeb.GetWebWord(code, 33) + ". " + GestionWeb.GetWebWord(2099, 33);

            errorMessage += "</div>";
            return errorMessage;
        }
        #endregion

	}
}

