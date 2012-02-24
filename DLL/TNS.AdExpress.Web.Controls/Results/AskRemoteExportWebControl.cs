using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxPro;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Insertions.DAL;
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Web.Controls.Translation;
using TNS.AdExpress.Web.Controls.Buttons;
using WebFunctions = TNS.AdExpress.Web.Functions;
namespace TNS.AdExpress.Web.Controls.Results
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:AskRemoteExportWebControl runat=server></{0}:AskRemoteExportWebControl>")]
    public class AskRemoteExportWebControl : WebControl
    {
        #region Variables
        /// <summary>
        /// Timeout des scripts utilisés par AjaxPro
        /// </summary>
        protected int _ajaxProTimeOut = 300;
        /// <summary>
        /// Session du client
        /// </summary>
        protected WebSession _webSession = null;
        /// <summary>
        /// Specify if the ajax scripts have been rendered;
        /// </summary>
        protected bool _ajaxRendered = false;
        /// <summary>
        /// Check if can group creative export by classification level
        /// </summary>
        protected bool _canGroupCreativesExport = false;
        /// <summary>
        ///  Max rows for creative export 
        /// </summary>
        protected int _nbMaxRowsAllowed = 5000;
        /// <summary>
        /// result type identifier
        /// </summary>
        private string _resultType = "";
        /// <summary>
        /// Button close control
        /// </summary>
        ImageButtonRollOverWebControl closeRollOverWebControl;
        /// <summary>
        /// file name 's text box 
        /// </summary>
        TextBox tbxFileName;
        /// <summary>
        /// mail 's text box  
        /// </summary>
        TextBox tbxMail;
        /// <summary>
        /// List of option to group creative export
        /// </summary>
        CheckBoxList groupAdByCheckBoxList;
        /// <summary>
        /// List of detai level item
        /// </summary>
        RadioButtonList levelsRadioButtonList;
        /// <summary>
        /// option register email
        /// </summary>
        CheckBox cbxRegisterMail;       
        #endregion

        #region Accessors
        /// <summary>
        ///GET /SET  option register email
        /// </summary>
        public CheckBox CbxRegisterMail
        {
            get { return cbxRegisterMail; }
            set { cbxRegisterMail = value; }
        }
        /// <summary>
        ///GET /SET  List of option to group creative export
        /// </summary>
        public CheckBoxList GroupAdByCheckBoxList
        {
            get { return groupAdByCheckBoxList; }
            set { groupAdByCheckBoxList = value; }
        }
        /// <summary>
        ///GET /SET  mail 's text box 
        /// </summary>
        public TextBox TbxMail
        {
            get { return tbxMail; }
            set { tbxMail = value; }
        }
        /// <summary>
        ///GET /SET  file name 's text box 
        /// </summary>
        public TextBox TbxFileName
        {
            get { return tbxFileName; }
            set { tbxFileName = value; }
        }
        /// <summary>
        ///GET /SET  option register email
        /// </summary>
        public ImageButtonRollOverWebControl CloseRollOverWebControl
        {
            get { return closeRollOverWebControl; }
            set { closeRollOverWebControl = value; }
        }
        /// <summary>
        ///GET /SET  result type identifier
        /// </summary>
        public string ResultType
        {
            get { return _resultType; }
            set { _resultType = value; }
        }
        /// <summary>
        ///GET /SET  option register email
        /// </summary>
        public bool CanGroupCreativesExport
        {
            get { return _canGroupCreativesExport; }
            set { _canGroupCreativesExport = value; }

        }
        /// <summary>
        ///  GET /SET  Session 
        /// </summary>
        [Bindable(false)]
        public WebSession CustomerWebSession
        {
            get { return (_webSession); }
            set { _webSession = value; }
        }
        /// <summary>
        /// Get /set List of detai level item
        /// </summary>
        public RadioButtonList LevelsRadioButtonList
        {
            get { return levelsRadioButtonList; }
            set { levelsRadioButtonList = value; }
        }
      
        #endregion

        #region JavaScript
        /// <summary>
        /// Génère le code JavaSript pour ajuster le time out d'AjaxPro
        /// </summary>
        /// <returns>Code JavaScript</returns>
        protected string AjaxProTimeOutScript()
        {
            StringBuilder js = new StringBuilder(100);
            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
            js.Append("\r\nAjaxPro.timeoutPeriod=" + _ajaxProTimeOut.ToString() + "*1000;");
            js.Append("\r\n-->\r\n</SCRIPT>");
            return (js.ToString());
        }


        /// <summary>
        /// Evenement Ajax
        /// </summary>
        /// <returns></returns>
        protected virtual string AjaxEventScript()
        {

            const int maxRows = 0;
            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");

            js.Append("\r\n var validatButtonName_img=\"'validateRollOverWebControl_img'\";\r\n");
            js.Append("\r\n var validateRollOverWebControlName=\'validateRollOverWebControl\';\r\n");


            js.Append("\r\n	validateRollOverWebControl_img_out = new Image(); validateRollOverWebControl_img_out.src = \"/App_Themes/" + Page.Theme + "/Images/Culture/button/valider_up.gif\";\r\n");
            js.Append("\r\n	validateRollOverWebControl_img_over = new Image(); validateRollOverWebControl_img_over.src = \"/App_Themes/" + Page.Theme + "/Images/Culture/button/valider_down.gif\";\r\n");

            if (!string.IsNullOrEmpty(_resultType) && Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.dedoum.GetHashCode())
            {

                js.Append("\r\nfunction get_" + this.ID + "(){");
                js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".CountDataRows('" + _webSession.IdSession + "',get_" + this.ID + "_callback);");
                js.Append("\r\n}");

                js.Append("\r\nfunction get_" + this.ID + "_callback(res){");
                js.Append("\r\n\tvar oN=document.getElementById('res_rowsLimit');");
                js.Append("\r\n\tvar oN1=document.getElementById('res_wait');");
                js.Append("\r\n\t if(oN1!=null){ \r\n");
                js.Append("\r\n\t oN1.style.display='none'; \r\n");
                js.Append("\r\n\t}\r\n");
                js.Append("\r\n\tif(res.value>" + _nbMaxRowsAllowed + ") {");
                js.Append("\r\n\toN.innerHTML='<div align=\"center\" id=\"res_rowsLimit\"><tr><td  class=\"txtViolet11Bold\" colSpan=\"2\">" + GestionWeb.GetWebWord(2939, _webSession.SiteLanguage) + "</td></tr></div>';");//style=\"border-right: #FF0000 1px solid; border-top: #FF0000 1px solid; border-left: #FF0000 1px solid; border-bottom: #FF0000 1px solid; border-color:red; color:red;\"
                js.Append("\r\n\t}\r\n");
                js.Append("\r\n\t else{\r\n");
                js.Append("\r\n\toN.innerHTML='';");
                js.Append("\r\n\tvar oN3=document.getElementById('res_Content');");
                js.Append("\r\n\t if(oN3!=null){ \r\n");
                js.Append("\r\n\t oN3.style.display=''; \r\n");
                js.Append("\r\n\t }\r\n");
                js.Append("\r\n\t var oN2=document.getElementById('validationDiv');");
                js.Append("\r\n\t if(oN2!=null){ \r\n");
                js.Append("\r\n\t oN2.innerHTML='<a id=\"validateRollOverWebControl\"  onmouseover=\"rolloverServerControl_display('+validatButtonName_img+',validateRollOverWebControl_img_over);\" onmouseout=\"rolloverServerControl_display('+validatButtonName_img+',validateRollOverWebControl_img_out);\" href=\"javascript:__doPostBack('+'&quot;validateRollOverWebControl&quot;'+','+'&quot;&quot;'+')\"><img name=\"validateRollOverWebControl_img\" src=\"/App_Themes/KMAE-Fr/Images/Culture/button/valider_up.gif\" style=\"border-width:0px;\"/></a>&nbsp;';");
                js.Append("\r\n\t }\r\n");
                js.Append("\r\n\t}\r\n");
                js.Append("\r\n}\r\n");
                js.Append("\r\naddEvent(window, \"load\", get_" + this.ID + ");");
            }
            else
            {
                js.Append("\r\nfunction getButtonValidate(){");
                js.Append("\r\n\t var oN2=document.getElementById('validationDiv');");
                js.Append("\r\n\t if(oN2!=null){ \r\n");
                js.Append("\r\n\t oN2.innerHTML='<a id=\"validateRollOverWebControl\"  onmouseover=\"rolloverServerControl_display('+validatButtonName_img+',validateRollOverWebControl_img_over);\" onmouseout=\"rolloverServerControl_display('+validatButtonName_img+',validateRollOverWebControl_img_out);\" href=\"javascript:__doPostBack('+'&quot;validateRollOverWebControl&quot;'+','+'&quot;&quot;'+')\"><img name=\"validateRollOverWebControl_img\" src=\"/App_Themes/KMAE-Fr/Images/Culture/button/valider_up.gif\" style=\"border-width:0px;\"/></a>&nbsp;';");
                js.Append("\r\n\t }\r\n");
                js.Append("\r\n}\r\n");
                js.Append("\r\naddEvent(window, \"load\", getButtonValidate);");
            }
            js.Append("\r\n-->\r\n</SCRIPT>");
            return (js.ToString());
        }
        #endregion

        #region Init
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e)
        {
            _resultType = Page.Request.QueryString.Get("resultType");
            tbxFileName = new TextBox();
            tbxFileName.ID = "tbxFileName";
            tbxFileName.EnableViewState = true;
            tbxFileName.Width = new Unit(300);
            Controls.Add(tbxFileName);

            #region Cookies enregistrement des préférences
            cbxRegisterMail = new CheckBox();
            cbxRegisterMail.ID = "cbxRegisterMail";
            cbxRegisterMail.Visible = false;
            cbxRegisterMail.EnableViewState = true;
            Controls.Add(cbxRegisterMail);

            tbxMail = new TextBox();
            tbxMail.ID = "tbxMail";
            tbxMail.Width = new Unit(300);
            tbxMail.EnableViewState = true;
            Controls.Add(tbxMail);

            //Vérifie si le navigateur accepte les cookies
            if (Page.Request.Browser.Cookies)
            {
                cbxRegisterMail.Text = GestionWeb.GetWebWord(2117, _webSession.SiteLanguage);
                cbxRegisterMail.CssClass = "txtViolet11Bold";
                HttpCookie isRegisterEmailForRemotingExport = null, savedEmailForRemotingExport = null;
                cbxRegisterMail.Visible = true; //RegisterMailLabel.Visible = true;

                if (!Page.IsPostBack)
                {
                    WebFunctions.Cookies.LoadSavedEmailForRemotingExport(Page, isRegisterEmailForRemotingExport, savedEmailForRemotingExport, cbxRegisterMail, tbxMail);
                }
            }
            else cbxRegisterMail.Visible = false; // = RegisterMailLabel.Visible = false;

            if (!string.IsNullOrEmpty(_resultType) && Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.dedoum.GetHashCode())
            {

                groupAdByCheckBoxList = new CheckBoxList();
                groupAdByCheckBoxList.CssClass = "txtViolet11";
                groupAdByCheckBoxList.ID = "groupAdByCheckBoxList";
                if (!string.IsNullOrEmpty(_resultType) && Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.dedoum.GetHashCode())
                {
                    groupAdByCheckBoxList.Items.Add(new ListItem(GestionWeb.GetWebWord(2933, _webSession.SiteLanguage), GenericColumnItemInformation.Columns.media.GetHashCode().ToString()));
                    groupAdByCheckBoxList.Items.Add(new ListItem(GestionWeb.GetWebWord(1146, _webSession.SiteLanguage), GenericColumnItemInformation.Columns.advertiser.GetHashCode().ToString()));
                    groupAdByCheckBoxList.Items.Add(new ListItem(GestionWeb.GetWebWord(1164, _webSession.SiteLanguage), GenericColumnItemInformation.Columns.product.GetHashCode().ToString()));
                }
                groupAdByCheckBoxList.EnableViewState = true;
                Controls.Add(groupAdByCheckBoxList);
            }

            if (Module.Name.ANALYSE_DES_DISPOSITIFS == _webSession.CurrentModule)
            {
                levelsRadioButtonList= new RadioButtonList();
                levelsRadioButtonList.CssClass = "txtViolet11";
                levelsRadioButtonList.ID = this.ID+"_levelsRadioButtonList";
                var module = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
                //var detailLevel = DetailLevelsInformation.Get(module.AsyncExportDetailLevel);
                //for(int v=1; v<=detailLevel.GetNbLevels; v++)
                //{
                //    var di = detailLevel[v];
                //    levelsRadioButtonList.Items.Add(new ListItem(GestionWeb.GetWebWord(di.WebTextId, _webSession.SiteLanguage),di.Id.GetHashCode().ToString()));
                //}
                levelsRadioButtonList.EnableViewState = true;
                Controls.Add(levelsRadioButtonList);
            }
            #endregion
            base.OnInit(e);
        }
        #region Load
        /// <summary>
        /// Chargement du composant
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e)
        {



            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            base.OnLoad(e);
        }
        #endregion

        #region RenderContents
        protected override void Render(HtmlTextWriter output)
        {

            StringBuilder html = new StringBuilder(1000);
            if (!string.IsNullOrEmpty(_resultType) && Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.dedoum.GetHashCode())
            {
                html.Append(AjaxProTimeOutScript());
            }
            html.Append(AjaxEventScript());
            output.Write(html.ToString());

            //output.Write("<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" height=\"100%\" border=\"0\">");
            //output.Write(" <!-- Header -->");
            //output.Write(" <tr><td class=\"popUpHeaderBackground popUpTextHeader\">&nbsp;");
            //AdExpressText saveTitle = new AdExpressText();
            //if (!string.IsNullOrEmpty(_resultType) && Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.dedoum.GetHashCode())
            //{
            //    _canGroupCreativesExport = true;
            //    saveTitle.Code = 2932;
            //}
            //else saveTitle.Code = 1747;
            //saveTitle.ID = "saveTitle";
            //saveTitle.Language = _webSession.SiteLanguage;
            //saveTitle.RenderControl(output);
            //output.Write("</td></tr>");
            output.Write("  <!-- Content -->");
            output.Write(" <tr> <td style=\"height:100%;background-color:#FFF;padding:10;\" valign=\"top\"> <table id=\"SaveData\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" border=\"0\">");//class=\"redBackGround\" 
            if (!string.IsNullOrEmpty(_resultType) && Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.dedoum.GetHashCode())
            {
                output.Write("<tr class=\"backGroundWhite\">");
                output.Write("	<td class=\"txtViolet11Bold\">");
                output.Write(" <div align=\"center\" id=\"res_wait\">"+GestionWeb.GetWebWord(2940,_webSession.SiteLanguage)+"</div>");
                output.Write("</td>");
                output.Write("</tr>");
                output.Write(
                    "<tr><td style=\"border-right: #FF0000 1px solid; border-top: #FF0000 1px solid; border-left: #FF0000 1px solid; border-bottom: #FF0000 1px solid; border-color:red; color:red;\" class=\"txtViolet11Bold\" colSpan=\"2\"><div align=\"center\" id=\"res_rowsLimit\"><img src=\"/App_Themes/" +
                    Page.Theme + "/Images/Common/waitAjax.gif\"></div></td></tr>");

                output.Write("<tr><td><div id=\"res_Content\" style=\"display:none;\"><table>");//Debut div content
            }

            output.Write("<tr height=\"30\" class=\"backGroundWhite\">");
            output.Write("	<td class=\"txtViolet11Bold\" width=\"150\">");
            AdExpressText FileNameLabel = new AdExpressText();
            FileNameLabel.Code = 1746;
            FileNameLabel.ID = "FileNameLabel";
            FileNameLabel.Language = _webSession.SiteLanguage;
            FileNameLabel.RenderControl(output);
            output.Write("</td>");
            output.Write("	<td>");

            tbxFileName.RenderControl(output);
            output.Write("</td>");
            output.Write("</tr>");
            output.Write("<tr class=\"backGroundWhite\">");
            output.Write("	<td class=\"txtViolet11Bold\" width=\"150\">");
            AdExpressText MailLabel = new AdExpressText();
            MailLabel.Code = 1136;
            MailLabel.ID = "MailLabel";
            MailLabel.Language = _webSession.SiteLanguage;
            MailLabel.RenderControl(output);
            output.Write("</td>");
            output.Write("<td>");

            tbxMail.RenderControl(output);
            output.Write("</td>");
            output.Write("</tr>");

            if (!string.IsNullOrEmpty(_resultType) && Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.dedoum.GetHashCode())
            {
                output.Write("<tr class=\"backGroundWhite\"><td colSpan=\"2\">&nbsp;</td></tr>");
                output.Write("<tr class=\"backGroundWhite\"><td class=\"txtViolet11Bold\" colSpan=\"2\">");
                AdExpressText Adexpresstext2 = new AdExpressText();
                Adexpresstext2.Code = 2934;
                Adexpresstext2.ID = "Adexpresstext2";
                Adexpresstext2.Language = _webSession.SiteLanguage;
                Adexpresstext2.RenderControl(output);
                output.Write(":</td></tr><tr class=\"backGroundWhite\"><td colSpan=\"2\">");
                groupAdByCheckBoxList.RenderControl(output);
                output.Write("</td></tr>");
            }
            if (Module.Name.ANALYSE_DES_DISPOSITIFS == _webSession.CurrentModule)
            {
                output.Write("<tr class=\"backGroundWhite\"><td colSpan=\"2\">&nbsp;</td></tr>");
                output.Write("<tr class=\"backGroundWhite\"><td class=\"txtViolet11Bold\" colSpan=\"2\">");
                AdExpressText Adexpresstext3 = new AdExpressText();
                Adexpresstext3.Code = 1886;
                Adexpresstext3.ID = "Adexpresstext3";
                Adexpresstext3.Language = _webSession.SiteLanguage;
                Adexpresstext3.RenderControl(output);
                output.Write("</td></tr><tr class=\"backGroundWhite\"><td colSpan=\"2\">");
                levelsRadioButtonList.RenderControl(output);
                output.Write("</td></tr>");
            }
            output.Write("<tr class=\"backGroundWhite\"><td colSpan=\"2\">&nbsp;</td></tr>");
            output.Write("<tr class=\"backGroundWhite\"><td class=\"txtViolet11Bold\" colspan=\"2\">");


            cbxRegisterMail.RenderControl(output);
        #endregion
            output.Write("</td></tr>");
            output.Write("</table>");
            output.Write("</td></tr>");

            if (!string.IsNullOrEmpty(_resultType) && Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.dedoum.GetHashCode())
            {
                output.Write("</table></div></td></tr>");//Fin div content
            }

            output.Write("  <!-- Footer -->");

            //output.Write(" <tr><td class=\"popUpFooterBackground\" align=\"right\"><table><tr><td>  <div id=\"validationDiv\"></div></td><td>");

            //closeRollOverWebControl.RenderControl(output);
            //output.Write("&nbsp;</td></tr></table>");

            //output.Write("</td></tr>");
            //output.Write("</table>");
        }
        #endregion


        #region   CountDataRows
        [AjaxMethod]
        public long CountDataRows(string idSession)
        {
             long nb = 0;

            #region Get NB Rows
            _webSession = (WebSession)WebSession.Load(idSession);
            List<GenericColumnItemInformation> cols = null;
            IInsertionsDAL _dalLayer = null;
            object[] param = new object[2];
            param[0] = _webSession;
            param[1] = _webSession.CurrentModule;

            // Sélection du vehicle
            string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, Constantes.Customer.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw new Exception("La sélection de médias est incorrecte");
            VehicleInformation vehicleInformation = VehiclesInformation.Get(long.Parse(vehicleSelection));
            if (vehicleInformation == null) throw new Exception("La sélection de médias est incorrecte");

            //Periods
            string fromDate = Core.Utilities.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType).ToString("yyyyMMdd");
            string toDate = Core.Utilities.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType).ToString("yyyyMMdd");

            //Get Columns
            cols = new List<GenericColumnItemInformation>();
            cols.Add(vehicleInformation.Id == Vehicles.names.adnettrack
                         ? WebApplicationParameters.GenericColumnItemsInformation.Get(81)
                         : WebApplicationParameters.GenericColumnItemsInformation.Get(6));

            //Get data
            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.insertionsDAL];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the insertions DAL"));
            _dalLayer = (IInsertionsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null, null);

            nb = _dalLayer.CountCreativeData(vehicleInformation, Convert.ToInt32(fromDate),
                                                    Convert.ToInt32(toDate), cols);
            #endregion
            return nb;

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

        /// <summary>
        /// Message d'erreur
        /// </summary>
        /// <param name="customerSession">Session du client</param>
        /// <param name="code">Code message</param>
        /// <returns>Message d'erreur</returns>
        protected string GetMessageError(WebSession customerSession, int code)
        {
            string errorMessage = "<div align=\"center\" class=\"txtViolet11Bold\" style=\"WIDTH: 400px; POSITION: relative;\">";
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
