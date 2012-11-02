using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.Exceptions;
using System;
using System.IO;

namespace TNS.AdExpress.Web.Controls.Results
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:AjaxResultBaseWebControl runat=server></{0}:AjaxResultBaseWebControl>")]
    public abstract class AjaxResultBaseWebControl : ResultBaseWebControl
    {
        #region Variables
        /// <summary>
        /// Timeout des scripts utilisés par AjaxPro
        /// </summary>
        protected int _ajaxProTimeOut = 60;
        /// <summary>
        /// Theme name
        /// </summary>
        protected string _themeName = string.Empty;
        #endregion

        #region Accesors

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

        #region InitializeResultToLoad
        /// <summary>
        /// Obtient ou définit le Timeout des scripts utilisés par AjaxPro
        /// </summary>
        public bool InitializeResultToLoad { get; set; }
        #endregion

        #endregion

        #region Javascript

        #region GetJavascript
        /// <summary>
        /// Get Validation Javascript Method
        /// </summary>
        /// <returns>Validation Javascript Method</returns>
        protected override string GetValidationJavascriptContent()
        {
            return "get_" + this.ID + "();";
        }
        #endregion

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
        protected string AjaxEventScript()
        {

            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n<script language=\"javascript\">\r\n");
            js.Append("\r\nfunction get_" + this.ID + "(){");
            js.Append("\r\n\tvar oN=document.getElementById('" + this.ID + "');");
            if (InitializeResultToLoad) js.Append("\r\n\toN.innerHTML='" + GetHTML().Replace("'", "\\'") + "';");
            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".GetData('" + this._webSession.IdSession + "', resultParameters_" + this.ID + ",styleParameters_" + this.ID + ",get_" + this.ID + "_callback);");
            js.Append("\r\n}");

            js.Append("\r\nfunction get_" + this.ID + "_callback(res){");
            js.Append("\r\n\tvar oN=document.getElementById('" + this.ID + "');");
            js.Append("\r\n\toN.innerHTML=res.value;");
            // js.Append("\r\n\tUpdateParameters(oN);");
            js.Append("\r\n}\r\n");

            if (InitializeResultToLoad)
            {
                js.Append("\r\nif(window.addEventListener)");
                js.Append("\r\n\twindow.addEventListener(\"load\", get_" + this.ID + ", false);");
                js.Append("\r\nelse if(window.attachEvent)");
                js.Append("\r\n\twindow.attachEvent(\"onload\",get_" + this.ID + "); ");
            }
            js.Append("\r\n\r\n</script>");
            return (js.ToString());
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
            InitializeResultToLoad = true;
            base.OnInit(e);
        }
        #endregion

        #region Load
        /// <summary>
        /// Chargement du composant
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            /*AjaxPro.Utility.RegisterTypeForAjax(this.GetType(), this.Page);*/
            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript1", "/ajaxpro/prototype.ashx");
            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript2", "/ajaxpro/core.ashx");
            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript3", "/ajaxpro/converter.ashx");
            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript4" + this.ID, "/ajaxpro/" + this.GetType().Namespace + "." + this.GetType().Name + ",TNS.AdExpress.Web.Controls.ashx");

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
            base.OnPreRender(e);
            _themeName = this.Page.Theme;
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output)
        {
            StringBuilder html = new StringBuilder(1000);
            html.Append(AjaxProTimeOutScript());
            html.Append(ResultParametersScript());
            html.Append(StyleParametersScript());
            html.Append(AjaxEventScript());
            output.Write(html.ToString());
            base.Render(output);
        }
        #endregion

        #endregion

        #region Ajax Methods

        #region Enregistrement des paramètres de construction du résultat
        /// <summary>
        /// Génération du JavaScript pour les paramètres du résultat
        /// </summary>
        /// <returns>Script</returns>
        protected string ResultParametersScript()
        {
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
            js.Append("\r\n\t var resultParameters_" + this.ID + " = new Object();");
            js.Append(SetResultParametersScript());
            js.Append("\r\n\t SetResultParameters_" + this.ID + "(resultParameters_" + this.ID + ");");
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
        private string SetResultParametersScript()
        {
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n\tfunction SetResultParameters_" + this.ID + "(obj){");
            js.Append(SetCurrentResultParametersScript());
            js.Append("\r\n\t}");
            return (js.ToString());
        }
        protected virtual string SetCurrentResultParametersScript()
        {
            return (string.Empty);
        }
        #endregion

        #region Enregistrement des paramètres pour les styles
        /// <summary>
        /// Génération du JavaScript des paramètres pour les styles
        /// </summary>
        /// <returns>Script</returns>
        protected string StyleParametersScript()
        {
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
            js.Append("\r\n\t var styleParameters_" + this.ID + " = new Object();");
            js.Append(SetStyleParametersScript());
            js.Append("\r\n\t SetCssStyles_" + this.ID + "(styleParameters_" + this.ID + ");");
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
        protected string SetStyleParametersScript()
        {
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n\nfunction SetCssStyles_" + this.ID + "(obj){");
            js.Append("\r\n\t obj.Theme = '" + _themeName + "';");
            js.Append(SetCurrentStyleParametersScript());
            js.Append("\r\n }");
            return (js.ToString());
        }
        protected virtual string SetCurrentStyleParametersScript()
        {
            return (string.Empty);
        }
        #endregion

        #region Chargement des paramètres AjaxPro.JavaScriptObject et WebSession
        /// <summary>
        /// Charge les paramètres des résultats navigant entre le client et le serveur
        /// </summary>
        /// <param name="o">Tableau de paramètres javascript</param>
        protected void LoadResultParameters(AjaxPro.JavaScriptObject o)
        {
            if (o != null)
            {
                LoadCurrentResultParameters(o);
            }
        }
        protected virtual void LoadCurrentResultParameters(AjaxPro.JavaScriptObject o)
        {
        }
        /// <summary>
        /// Charge les paramètres des sytles navigant entre le client et le serveur
        /// </summary>
        /// <param name="o">Tableau de paramètres javascript</param>
        protected void LoadStyleParameters(AjaxPro.JavaScriptObject o)
        {
            if (o != null)
            {
                if (o.Contains("Theme"))
                {
                    _themeName = o["Theme"].Value;
                    _themeName = _themeName.Replace("\"", "");
                }
                LoadCurrentStyleParameters(o);
            }
        }
        protected virtual void LoadCurrentStyleParameters(AjaxPro.JavaScriptObject o)
        {
        }
        #endregion

        #region GetData
        /// <summary>
        /// Get VP schedule HTML  code
        /// </summary>
        /// <param name="o">Result parameters (session Id, theme...)</param>
        /// <returns>Code HTML</returns>
        [AjaxPro.AjaxMethod]
        public virtual string GetData(string idSession, AjaxPro.JavaScriptObject resultParameters, AjaxPro.JavaScriptObject styleParameters)
        {
            string html;
            try
            {
                this.LoadResultParameters(resultParameters);
                this.LoadStyleParameters(styleParameters);
                _webSession = (WebSession)WebSession.Load(idSession);

                html = GetAjaxHTML();

            }
            catch (System.Exception err)
            {
                return (OnAjaxMethodError(err, _webSession));
            }
            return (html);
        }
        #endregion

        #endregion

        #region Methods

        #region GetAjaxHTML
        /// <summary>
        /// Compute VP schedule
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <returns>Code HTMl</returns>
        protected abstract string GetAjaxHTML();
        #endregion

        #region GetHTML
        /// <summary>
        /// Get  loading HTML  
        /// </summary>
        /// <returns></returns>
        protected override string GetHTML()
        {
            return ("<div align=\"center\" width = \"100%\"><img src=\"/App_Themes/" + _themeName + "/Images/Common/waitAjax.gif\"></div>");
        }
        #endregion

        #region GetMessageError

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

        #region OnAjaxMethodError
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

        #region GetTreeNode
        /// <summary>
        /// Get Tree Node
        /// </summary>
        /// <returns>TreeNode</returns>
       protected  System.Windows.Forms.TreeNode GetTreeNode(object[] parameters, GenericDetailLevel genericDetailLevel
           ,Func<int , int , GenericDetailLevel ,TNS.AdExpress.Constantes.Customer.Right.type> GetAccessType)
        {
            System.Windows.Forms.TreeNode treeNode = new System.Windows.Forms.TreeNode();
            System.Windows.Forms.TreeNode cNodeL1 = new System.Windows.Forms.TreeNode();
            System.Windows.Forms.TreeNode cNodeL2 = new System.Windows.Forms.TreeNode();
            System.Windows.Forms.TreeNode cNodeL3 = new System.Windows.Forms.TreeNode();
            System.Windows.Forms.TreeNode cNodeL4 = new System.Windows.Forms.TreeNode();


            TNS.AdExpress.Constantes.Customer.Right.type accessType;
            List<Int64> lvlListOld = new List<Int64>();
            Dictionary<Int64, IDictionary> nodesList = new Dictionary<long, IDictionary>();
            foreach (string item in parameters)
            {
                List<Int64> lvlList = (new List<string>(item.Split('_'))).ConvertAll<Int64>(p => Int64.Parse(p));

                if (lvlList.Count > 0 && genericDetailLevel.Levels.Count > 0 && !treeNode.Nodes.ContainsKey(genericDetailLevel[1].Id.ToString()))
                {
                    accessType = GetAccessType(1, lvlList.Count, genericDetailLevel);
                    if (!treeNode.Nodes.ContainsKey(lvlList[0].ToString()))
                    {
                        cNodeL1 = new System.Windows.Forms.TreeNode(lvlList[0].ToString());
                        cNodeL1.Name = lvlList[0].ToString();
                        cNodeL1.Checked = (1 == lvlList.Count);
                        cNodeL1.Tag = new LevelInformation(accessType, lvlList[0], string.Empty);
                        treeNode.Nodes.Add(cNodeL1);
                    }

                }
                if (lvlList.Count > 1 && genericDetailLevel.Levels.Count > 1 && !treeNode.Nodes.ContainsKey(genericDetailLevel[2].Id.ToString()))
                {
                    accessType = GetAccessType(2, lvlList.Count, genericDetailLevel);
                    if (!cNodeL1.Nodes.ContainsKey(lvlList[1].ToString()))
                    {
                        cNodeL2 = new System.Windows.Forms.TreeNode(lvlList[1].ToString());
                        cNodeL2.Name = lvlList[1].ToString();
                        cNodeL2.Checked = (2 == lvlList.Count);
                        cNodeL2.Tag = new LevelInformation(accessType, lvlList[1], string.Empty);
                        cNodeL1.Nodes.Add(cNodeL2);
                    }

                }
                if (lvlList.Count > 2 && genericDetailLevel.Levels.Count > 2 && !treeNode.Nodes.ContainsKey(genericDetailLevel[3].Id.ToString()))
                {
                    accessType = GetAccessType(3, lvlList.Count, genericDetailLevel);
                    if (!cNodeL2.Nodes.ContainsKey(lvlList[2].ToString()))
                    {
                        cNodeL3 = new System.Windows.Forms.TreeNode(lvlList[2].ToString());
                        cNodeL3.Name = lvlList[2].ToString();
                        cNodeL3.Checked = (3 == lvlList.Count);
                        cNodeL3.Tag = new LevelInformation(accessType, lvlList[2], string.Empty);
                        cNodeL2.Nodes.Add(cNodeL3);
                    }

                }
                if (lvlList.Count > 3 && genericDetailLevel.Levels.Count > 3 && !treeNode.Nodes.ContainsKey(genericDetailLevel[4].Id.ToString()))
                {
                    accessType = GetAccessType(4, lvlList.Count, genericDetailLevel);
                    if (!cNodeL3.Nodes.ContainsKey(lvlList[3].ToString()))
                    {
                        cNodeL4 = new System.Windows.Forms.TreeNode(lvlList[3].ToString());
                        cNodeL4.Name = lvlList[3].ToString();
                        cNodeL4.Checked = (4 == lvlList.Count);
                        cNodeL4.Tag = new LevelInformation(accessType, lvlList[3], string.Empty);
                        cNodeL3.Nodes.Add(cNodeL4);
                    }

                }
            }
            return treeNode;
        }
        #endregion

       #region GetAccessType
       /// <summary>
       /// Get Access Type
       /// </summary>
       /// <returns>Access Type</returns>
       protected  virtual TNS.AdExpress.Constantes.Customer.Right.type GetAccessType(int lvl, int nbLvl, GenericDetailLevel genericDetailLevel)
       {
           TNS.AdExpress.Constantes.Customer.Right.type accesType;
           switch (genericDetailLevel[lvl].Id)
           {
               case DetailLevelItemInformation.Levels.vpBrand:
                   if (lvl == nbLvl)
                       accesType = TNS.AdExpress.Constantes.Customer.Right.type.vpBrandAccess;
                   else
                       accesType = TNS.AdExpress.Constantes.Customer.Right.type.vpBrandException;
                   break;
               case DetailLevelItemInformation.Levels.vpCircuit:
                   if (lvl == nbLvl)
                       accesType = TNS.AdExpress.Constantes.Customer.Right.type.circuitAccess;
                   else
                       accesType = TNS.AdExpress.Constantes.Customer.Right.type.circuitException;
                   break;
               case DetailLevelItemInformation.Levels.vpProduct:
                   if (lvl == nbLvl)
                       accesType = TNS.AdExpress.Constantes.Customer.Right.type.vpProductAccess;
                   else
                       accesType = TNS.AdExpress.Constantes.Customer.Right.type.vpProductException;
                   break;
               case DetailLevelItemInformation.Levels.vpSegment:
                   if (lvl == nbLvl)
                       accesType = TNS.AdExpress.Constantes.Customer.Right.type.vpSegmentAccess;
                   else
                       accesType = TNS.AdExpress.Constantes.Customer.Right.type.vpSegmentException;
                   break;
               case DetailLevelItemInformation.Levels.vpSubSegment:
                   if (lvl == nbLvl)
                       accesType = TNS.AdExpress.Constantes.Customer.Right.type.vpSubSegmentAccess;
                   else
                       accesType = TNS.AdExpress.Constantes.Customer.Right.type.vpSubSegmentException;
                   break;
               case DetailLevelItemInformation.Levels.site:
                   if (lvl == nbLvl)
                       accesType = TNS.AdExpress.Constantes.Customer.Right.type.siteAccess;
                   else
                       accesType = TNS.AdExpress.Constantes.Customer.Right.type.siteException;
                   break;
               default:
                   throw new BaseException("Impossible to retrieve the current level");
           }



           return accesType;
       }
       #endregion


       protected virtual string CheckDates()
       {
           StringBuilder js = new StringBuilder(2000);
           js.Append("\r\n\t\t\t var monthBegin = dateParameters[0].split('_')[1];");
           js.Append("\r\n\t\t\t var yearBegin = dateParameters[0].split('_')[0];");
           js.Append("\r\n\t\t\t var monthEnd = dateParameters[1].split('_')[1];");
           js.Append("\r\n\t\t\t var yearEnd = dateParameters[1].split('_')[0];");
           js.Append("\r\n\t\t\t if(monthBegin=='none'||yearBegin=='none'||monthEnd=='none'||yearEnd=='none'){");
           js.Append("\r\n\t\t\t\t alert('" + GestionWeb.GetWebWord(886, _webSession.SiteLanguage) + "');");
           js.Append("\r\n\t\t\t\t return false;");
           js.Append("\r\n\t\t\t }");

           js.Append("\r\n\t\t\t var dateBegin = yearBegin+monthBegin;");
           js.Append("\r\n\t\t\t var dateEnd = yearEnd+monthEnd;");
           js.Append("\r\n\t\t\t if(parseInt(dateBegin)>parseInt(dateEnd)){");
           js.Append("\r\n\t\t\t\t alert('" + GestionWeb.GetWebWord(1855, _webSession.SiteLanguage) + "'); ");
           js.Append("\r\n\t\t\t\t return false;");
           js.Append("\r\n\t\t\t }");
           js.Append("\r\n\t\t} ");
           return js.ToString();
       }

        #endregion
    }
}
