using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMI.PromoPSA.Rules;
using KMI.PromoPSA.Web.Core.Sessions;

public partial class disconnectUser : System.Web.UI.Page {
    /// <summary>
    /// Page Load
    /// </summary>
    /// <param name="sender">Object Sender</param>
    /// <param name="e">Event</param>
    protected void Page_Load(object sender, EventArgs e) {

        try {
            Int64 id = -1;
            try {
                id = Convert.ToInt64(Page.Request.QueryString["id"]);
            }
            catch { }

            if (id > 0 && WebSessions.Contains(Convert.ToInt64(Page.Request.QueryString["id"]))) {
                /*object[] parameters = null;
                IResults result = ((IResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + WebApplicationParameters.RulesLayer.AssemblyName, WebApplicationParameters.RulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null));
                result.DisconnectUser(Convert.ToInt64(Page.Request.QueryString["id"]));*/
                WebSessions.Remove(Convert.ToInt64(Page.Request.QueryString["id"]));
            }
            else {
                WebSession webSession = (WebSession)Session[KMI.PromoPSA.Constantes.WebSession.WEB_SESSION];
                if (webSession != null && WebSessions.Contains(webSession.CustomerLogin.IdLogin)) {
                    webSession.Disconect(Session);
                }
            }
            if (!Page.ClientScript.IsClientScriptBlockRegistered("goToIndex")) {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "goToIndex", KMI.PromoPSA.Web.Functions.Script.Login(Page.ResolveUrl("~/index.aspx")));
            }
        }
        catch { }
      
    }
}