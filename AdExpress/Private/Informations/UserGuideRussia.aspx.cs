using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Private_Informations_UserGuideRussia : TNS.AdExpress.Web.UI.PrivateWebPage
{
  
    public Private_Informations_UserGuideRussia()
    {
        // Get Session
        try
        {
            string idSession = HttpContext.Current.Request.QueryString.Get("idSession");
           
        }
        catch (System.Exception)
        {
            Response.Write(TNS.AdExpress.Web.Functions.Script.ErrorCloseScript("Your session is unavailable. Please reconnect via the Homepage"));
            Response.Flush();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        
    }
}
