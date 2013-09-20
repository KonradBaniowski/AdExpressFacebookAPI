using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.SessionState;
using KMI.PromoPSA.BusinessEntities;
using KMI.PromoPSA.BusinessEntities.Classification;
using KMI.PromoPSA.Rules;
using KMI.PromoPSA.Web.Core.Sessions;
using KMI.PromoPSA.Web.UI;
using WebSession = KMI.PromoPSA.Constantes.WebSession;
using System.Web.SessionState;
public partial class Private_Edit : PrivateWebPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        DisconnectUserWebControl1.WebSession = _webSession;
        LoginInformationWebControl1.WebSession = _webSession;
        PromotionInformationWebControl1.WebSession = _webSession;
       
    }

    [WebMethod]
    public static void SaveCodification(Advert advert, string loginId)
    {
        IResults results = new Results();
        results.UpdateCodification(advert);
        results.ChangeAdvertStatus(advert.IdForm, advert.Activation);

        var session = WebSessions.Get(Convert.ToInt64(loginId));
        int t= 0;
    }
    [WebMethod]
    public static Codification GetCodification(long idForm)
    {
        IResults results = new Results();
        return results.GetCodification(idForm);
    }

    [WebMethod]
    public static List<Product> GetProductsBySegment(long segmentId)
    {
        IResults results = new Results();
        return results.GetProductsBySegment(segmentId);
    }

   
}