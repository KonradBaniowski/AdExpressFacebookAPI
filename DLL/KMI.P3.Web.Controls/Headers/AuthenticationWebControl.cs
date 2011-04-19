using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.Isis.Right.Common;
using KMI.P3.Web.Core.Sessions;
using KMI.P3.Web.Controls.Buttons;
using KMI.P3.Domain.Web;
using KMI.P3.Domain.Translation;
using WebFunctions = KMI.P3.Web.Functions;
using KMI.P3.Domain.DataBaseDescription;




namespace KMI.P3.Web.Controls.Headers
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:AuthenticationWebControl runat=server></{0}:AuthenticationWebControl>")]
    public class AuthenticationWebControl :  WebControl 
    {
        #region Variables
        /// <summary>
        /// Login
        /// </summary>
        private string _login = string.Empty;
        /// <summary>
        /// Password
        /// </summary>
        private string _password = string.Empty;
        /// <summary>
        /// Web session
        /// </summary>
        protected WebSession _webSession = null;
        #endregion

        #region Variables MMI
        /// <summary>
        /// Login label
        /// </summary>
        private Label _loginLabel;
        /// <summary>
        /// Password label
        /// </summary>
        private Label _passwordLabel;
        /// <summary>
        /// Login Text Box
        /// </summary>
        private TextBox _loginTextBox;
        /// <summary>
        /// Password Text Box
        /// </summary>
        private TextBox _passwordTextBox;
        /// <summary>
        /// Ok Button
        /// </summary>
        public ImageButtonRollOverWebControl _buttonOk;
        #endregion

        #region Evènements

        #region Init
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguements</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            #region Login label init
            _loginLabel = new Label();
            _loginLabel.ID = "loginLabel";
            _loginLabel.Text = "Login";
            Controls.Add(_loginLabel);
            #endregion

            #region Login textbox init
            _loginTextBox = new TextBox();
            _loginTextBox.ID = "loginTextBox";
            _loginTextBox.CssClass = "input";
            Controls.Add(_loginTextBox);
            #endregion

            #region Password label init
            _passwordLabel = new Label();
            _passwordLabel.ID = "passwordLabel";
            _passwordLabel.Text = "Mot de passe";
            Controls.Add(_passwordLabel);
            #endregion

            #region Password textbox init
            _passwordTextBox = new TextBox();
            _passwordTextBox.ID = "passwordTextBox";
            _passwordTextBox.TextMode = TextBoxMode.Password;
            _passwordTextBox.CssClass = "input";
            Controls.Add(_passwordTextBox);
            #endregion

            #region Ok image button init
            _buttonOk = new ImageButtonRollOverWebControl();
            _buttonOk.ID = "okImageButton";
            _buttonOk.ImageUrl = "/App_Themes/" + this.Page.Theme + "/Images/Common/Button/bt_validate_up.gif";
            _buttonOk.RollOverImageUrl = "/App_Themes/" + this.Page.Theme + "/Images/Common/Button/bt_validate_down.gif";
            _buttonOk.CssClass = "submitButton";
            Controls.Add(_buttonOk);
            #endregion
        }
        #endregion

        #region Chargement
        /// <summary>
        /// Evènement de chargement du composant
        /// </summary>
        /// <param name="e">Argument</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _passwordTextBox.Attributes.Add("onkeypress", "javascript:trapEnter(event);");
            if (!Page.ClientScript.IsClientScriptBlockRegistered("trapEnter"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "trapEnter", WebFunctions.Script.TrapEnter("okImageButton"));
            }

            if (Page.IsPostBack)
            {

                try
                {
                    _login = _loginTextBox.Text.Trim();
                    _password = _passwordTextBox.Text.Trim();
                    P3Login p3Login = new P3Login(WebApplicationParameters.DataBaseDescription.GetCustomerConnection(_login, _password, CustomerConnectionIds.p3), _login, _password); 
                    if(p3Login.CanAccessToProject())
                    {
                      
                        if (_webSession == null) _webSession = new WebSession(p3Login);                      
                        _webSession.SiteLanguage = WebApplicationParameters.DefaultLanguage;
                        _webSession.Save();
                        Page.Response.Redirect(Page.ResolveUrl(KMI.P3.Constantes.Web.URL.HOME_URL + "?idSession=" + _webSession.SessionId));
                    }
                    else
                    {
                        Page.Response.Write("<script language=javascript>");
                        Page.Response.Write("	alert(\"" + GestionWeb.GetWebWord(17, WebApplicationParameters.DefaultLanguage) + "\");");
                        Page.Response.Write("</script>");
                    }
                }
                catch (System.Exception)
                {
                    // L'accès est impossible
                    Page.Response.Write("<script language=javascript>");
                    Page.Response.Write("	alert(\"" + GestionWeb.GetWebWord(17, WebApplicationParameters.DefaultLanguage) + "\");");
                    Page.Response.Write("</script>");
                }
            }
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output)
        {

            #region Render
            output.Write("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"" + this.CssClass + "\" align=\"center\">");
            output.Write("<tr>");
            output.Write("<th>");
            _loginLabel.RenderControl(output);
            output.Write("</th>");
            output.Write("<td>");
            _loginTextBox.RenderControl(output);
            output.Write("</td>");
            output.Write("</tr>");
            output.Write("<tr>");
            output.Write("<th>");
            _passwordLabel.RenderControl(output);
            output.Write("</th>");
            output.Write("<td>");
            _passwordTextBox.RenderControl(output);
            output.Write("</td>");
            output.Write("</tr>");
            output.Write("<tr>");
            output.Write("<td colspan=\"2\" style=\"text-align:right;\">");
            _buttonOk.RenderControl(output);
            output.Write("</td>");
            output.Write("</tr>");
            output.Write("</table>");
            #endregion

        }
        #endregion

        #endregion
    }
}
