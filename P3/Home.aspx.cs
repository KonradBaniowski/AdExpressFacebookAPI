using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMI.P3.Web.UI;
using KMI.P3.Domain.Web;
using TNS.Isis.Right.Common;
using System.Web.UI.HtmlControls;

public partial class Home : PrivateWebPage
{
    #region Variable
    public string _url = "";
    #endregion

    #region On PreInit
    /// <summary>
    /// On preinit event
    /// </summary>
    /// <param name="e">Arguments</param>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

    }
    #endregion

    #region On Init
    /// <summary>
    /// On Init event
    /// </summary>
    /// <param name="e">Arguments</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        HtmlLink link = new HtmlLink();
        link.Attributes.Add("href", Page.ResolveClientUrl("~/Marketing/CSS/Marketing.css"));
        link.Attributes.Add("type", "text/css");
        link.Attributes.Add("rel", "stylesheet");
        Page.Header.Controls.Add(link);
        LoginLinksWebControl1.CustomerWebSession = _webSession;
      
    }
    #endregion

    #region On Load
    /// <summary>
    /// OnLoad
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        
        P3Login p3Login = new P3Login(_webSession.Source, _webSession.Login, _webSession.Password);

        try{
            _url += WebApplicationParameters.CustomerTypeInfo[p3Login.LoginContact.GroupContactId].FilePath;
        }
        catch(System.Exception){
            _url += WebApplicationParameters.CustomerTypeInfo[0].FilePath;
        }
        
    }
    #endregion
}