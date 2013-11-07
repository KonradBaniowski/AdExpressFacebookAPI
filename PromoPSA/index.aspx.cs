using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMI.PromoPSA.Rules.Exceptions;
using KMI.PromoPSA.Web.UI;
using KMI.PromoPSA.Web.Core;
using KMI.PromoPSA.Web.Core.Sessions;
using KMI.PromoPSA.Web.Domain;
using KMI.PromoPSA.Web.Domain.Translation;
using KMI.PromoPSA.Rules;
using System.Web.Services;
using KMI.PromoPSA.Web.Functions;
using System.Web.SessionState;
using System.Reflection;

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

    #region Verif User
    /// <summary>
    /// Verif User
    /// </summary>
    /// <param name="loginId">Login Id</param>
    /// <returns>True if succed</returns>
    [WebMethod]
    public static string verifUser(string login, string password) {

        WebSession webSession = null;

        try {
                Right right = new Right(login, password);
                
                if (right.CanAccessToPSA()) {
                    if (WebSessions.Contains(right.IdLogin)) {
                        return "true";
                    }
                }
                return "false";
        }
        catch (Exception e) {
            string message = " Erreur lors de la liberation de la fiche.<br/>";
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            message += string.Format("Login Id : {0}", webSession.CustomerLogin.IdLogin);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur lors de la liberation de la fiche", e);
        }

    }
    #endregion

    private void regenerateId() {
        System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
        string oldId = manager.GetSessionID(Context);
        string newId = manager.CreateSessionID(Context);
        bool isAdd = false, isRedir = false;
        manager.SaveSessionID(Context, newId, out isRedir, out isAdd);
        HttpApplication ctx = (HttpApplication)HttpContext.Current.ApplicationInstance;
        HttpModuleCollection mods = ctx.Modules;
        System.Web.SessionState.SessionStateModule ssm = (SessionStateModule)mods.Get("Session");
        System.Reflection.FieldInfo[] fields = ssm.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        SessionStateStoreProviderBase store = null;
        System.Reflection.FieldInfo rqIdField = null, rqLockIdField = null, rqStateNotFoundField = null;
        foreach (System.Reflection.FieldInfo field in fields) {
            if (field.Name.Equals("_store")) store = (SessionStateStoreProviderBase)field.GetValue(ssm);
            if (field.Name.Equals("_rqId")) rqIdField = field;
            if (field.Name.Equals("_rqLockId")) rqLockIdField = field;
            if (field.Name.Equals("_rqSessionStateNotFound")) rqStateNotFoundField = field;
        }
        object lockId = rqLockIdField.GetValue(ssm);
        if ((lockId != null) && (oldId != null)) store.ReleaseItemExclusive(Context, oldId, lockId);
        rqStateNotFoundField.SetValue(ssm, true);
        rqIdField.SetValue(ssm, newId);
    }

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
                    if (WebSessions.Contains(right.IdLogin)) {

                        IResults results = new Results();
                        results.ReleaseUser(webSession.CustomerLogin.IdLogin);
                        WebSessions.Remove(webSession.CustomerLogin.IdLogin);
                    }
                    
                    WebSessions.Add(webSession);
                    webSession.Save(Session);
                    if (!Page.ClientScript.IsClientScriptBlockRegistered("Login")) {
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Login", KMI.PromoPSA.Web.Functions.Script.Login("/Private/Home.aspx"));
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
            catch (System.Exception eee) {
                // Connexion refusé
                Response.Write("<script language=\"javascript\">alert(\"" + GestionWeb.GetWebWord(44, WebApplicationParameters.DefaultLanguage) + "\");</script>");
            }
        }

    }
    #endregion

    #endregion


}