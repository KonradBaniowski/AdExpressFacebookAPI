using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class P3MP : System.Web.UI.MasterPage
{
    #region On Init
    /// <summary>
    /// On OnInit event
    /// </summary>
    /// <param name="e">Arguments</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }
    #endregion

    #region On Load
    /// <summary>
    /// Page Load
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        _srcLogoPortailPolePub.Src = "/App_Themes/" + this.Page.Theme + "/images/logo_p3.gif";
        _srcLogoHeader.Src = "/App_Themes/" + this.Page.Theme + "/images/header.gif";
        Response.Cache.SetNoStore();
    }
    #endregion
}
