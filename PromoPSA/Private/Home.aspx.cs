using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMI.PromoPSA.Rules;
using KMI.PromoPSA.Web.UI;

public partial class Private_Home : PrivateWebPage {
    /// <summary>
    /// Page Load
    /// </summary>
    /// <param name="sender">Object Sender</param>
    /// <param name="e">Event Args</param>
    protected void Page_Load(object sender, EventArgs e) {
        DisconnectUserWebControl1.WebSession = _webSession;
        LoginInformationWebControl1.WebSession = _webSession;
        PromotionInformationWebControl1.WebSession = _webSession;

        /*IResults results = new Results();
        LinqAtRuntimeGrid.DataSource = results.GetAdverts(201309); 
        LinqAtRuntimeGrid.DataBind();*/
    }
}