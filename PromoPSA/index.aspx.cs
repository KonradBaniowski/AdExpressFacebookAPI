using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMI.PromoPSA.Rules.Exceptions;
using KMI.PromoPSA.Web;
using KMI.PromoPSA.Web.Core;
using KMI.PromoPSA.Web.Core.Sessions;
using KMI.PromoPSA.Web.Domain;
using KMI.PromoPSA.Web.Domain.Translation;

public partial class index : WebPage {

    #region Variables
    /// <summary>
    /// Has Error
    /// </summary>
    private bool _hasError = false;
    #endregion


    #region Event

    #region Page_Load
    /// <summary>
    /// page load
    /// </summary>
    /// <param name="sender">Object Sender</param>
    /// <param name="e">Event</param>
    protected void Page_Load(object sender, EventArgs e) {

        try {
            passwordTextBox.Enabled = true;
            loginTextBox.Enabled = true;
            Button1.Enabled = true;
            loginLabel.Text = GestionWeb.GetWebWord(46, WebApplicationParameters.DefaultLanguage);
            passwordLabel.Text = GestionWeb.GetWebWord(47, WebApplicationParameters.DefaultLanguage);
            Button1.Text = GestionWeb.GetWebWord(48, WebApplicationParameters.DefaultLanguage);
        }
        catch (Exception err) {
            if (err.GetType() != typeof(System.Threading.ThreadAbortException)) {
                _hasError = true;
                passwordTextBox.Enabled = false;
                loginTextBox.Enabled = false;
                Button1.Enabled = false;
                this.OnError(new KMI.PromoPSA.Web.UI.ErrorEventArgs(this, err));
            }
        }

    }
    #endregion


    #region Validation Login
    /// <summary>
    /// Validation Login
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button1_Click(object sender, EventArgs e) {

        if (!_hasError) {
            try {
                string loginText = loginTextBox.Text.Trim();
                string passwordText = passwordTextBox.Text.Trim();


                Right right = new Right(loginText, passwordText);
                if (right.CanAccessToPSA()) {
                    WebSession webSession = new WebSession(right, Session, Page);

                    //If login already use
                    if (WebSessions.Contains(webSession.CustomerLogin.IdLogin)) {
                        if (!Page.ClientScript.IsClientScriptBlockRegistered("LoginOutSession")) {
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "LoginOutSession", KMI.PromoPSA.Web.Functions.Script.LoginAlreadyInUse(webSession.SiteLanguage, webSession.CustomerLogin.IdLogin));
                        }
                    }
                    //ELSE
                    else {
                        WebSessions.Add(webSession);
                        webSession.Save(Session);
                        if (!Page.ClientScript.IsClientScriptBlockRegistered("Login")) {
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Login", KMI.PromoPSA.Web.Functions.Script.Login("/Private/Home.aspx"));
                        }
                    }

                }

                else {
                    // Connexion refusé : Cet identifiant ne possède aucun droit
                    Response.Write("<script language=\"javascript\">alert(\"" + GestionWeb.GetWebWord(44, WebApplicationParameters.DefaultLanguage) + " :\\n" + GestionWeb.GetWebWord(45, WebApplicationParameters.DefaultLanguage) + "\");</script>");
                }
            }
            catch (WebServiceRightException err) {
                //WebService Indisponible
                Response.Write("<script language=\"javascript\">alert(\"" + GestionWeb.GetWebWord(93, WebApplicationParameters.DefaultLanguage) + "\");</script>");
                this.OnError(new KMI.PromoPSA.Web.UI.ErrorEventArgs(this, err));
            }
            catch {
                // Connexion refusé
                Response.Write("<script language=\"javascript\">alert(\"" + GestionWeb.GetWebWord(44, WebApplicationParameters.DefaultLanguage) + "\");</script>");
            }
        }

    }
    #endregion

    #endregion


}