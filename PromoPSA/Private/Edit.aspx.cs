using System;
using System.Collections.Generic;
using System.Web.Services;
using KMI.PromoPSA.BusinessEntities;
using KMI.PromoPSA.BusinessEntities.Classification;
using KMI.PromoPSA.Rules;
using KMI.PromoPSA.Web.Exceptions;
using KMI.PromoPSA.Web.Functions;
using KMI.PromoPSA.Web.UI;

public partial class Private_Edit : PrivateWebPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        DisconnectUserWebControl1.WebSession = _webSession;
        LoginInformationWebControl1.WebSession = _webSession;
        PromotionInformationWebControl1.WebSession = _webSession;

    }

    [WebMethod]
    public static long SaveCodification(Advert advert, string loginId, string multiPromo)
    {

        try
        {
            IResults results = new Results();
            results.UpdateCodification(advert);
            results.ChangeAdvertStatus(advert.IdDataPromotion, advert.Activation);
            if (Convert.ToInt64(multiPromo) == 0)
                return results.GetAvailablePromotionId(Convert.ToInt64(loginId));
            else {
                long formId = results.InsertPromotion(advert);
                return results.GetDuplicatedPromotionId(Convert.ToInt64(loginId), formId);
            }
        }
        catch (Exception e)
        {
            string message = " Erreur lors de la sauvegarde de la fiche.<br/>";
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            if (advert != null) message += string.Format("Promotion numero : {0}<br/>", advert.IdDataPromotion);
            message += string.Format("Login Id : {0}", loginId);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur de sauvegarde", e);
        }
    }

   

    [WebMethod]
    public static long RejectForm(long promotionId, string loginId, long activationCode)
    {
        try
        {
            IResults results = new Results();
            results.UpdateCodification(promotionId, activationCode);
            results.ChangeAdvertStatus(promotionId, activationCode);
            return results.GetAvailablePromotionId(Convert.ToInt64(loginId));
        }
        catch (Exception e)
        {
            string message = " Erreur lors du rejet de la fiche.<br/>";
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            message += string.Format("Promotion numero : {0}<br/>", promotionId);
            if (!string.IsNullOrEmpty(loginId)) message += string.Format("Login Id : {0}", loginId);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur de rejet de fiche", e);
        }
    }

    [WebMethod]
    //public static long PendingForm(long promotionId, string loginId, long activationCode)
    public static long PendingForm(Advert advert, string loginId)
    {
        try
        {
            IResults results = new Results();
            /*results.UpdateCodification(promotionId, activationCode);
            results.ChangeAdvertStatus(promotionId, activationCode);*/
            results.UpdateCodification(advert);
            results.ChangeAdvertStatus(advert.IdDataPromotion, advert.Activation);
            return results.GetAvailablePromotionId(Convert.ToInt64(loginId));
        }
        catch (Exception e)
        {
            string message = " Erreur lors de la mise en litige de la fiche.<br/>";
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            //message += string.Format("Promotion numero : {0}<br/>", promotionId);
            message += string.Format("Promotion numero : {0}<br/>", advert.IdDataPromotion);
            if (!string.IsNullOrEmpty(loginId)) message += string.Format("Login Id : {0}", loginId);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur de la mise en litige de la fiche", e);
        }
    }
    
    [WebMethod]
    public static Codification GetCodification(long idDataPromotion)
    {
        try
        {
            IResults results = new Results();
            return results.GetCodification(idDataPromotion);
        }
        catch (Exception e)
        {
            string message = " Erreur pour obtenir la fiche de codification.<br/>";
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            message += string.Format("Promotion numero : {0}<br/>", idDataPromotion);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur obtention fiche de codification", e);
        }
    }

    [WebMethod]
    public static void ReleaseUser(string loginId)
    {
        try
        {
            IResults results = new Results();
            results.ReleaseUser(Convert.ToInt64(loginId));
        }
        catch (Exception e)
        {
            string message = string.Format(" Erreur lors de la liberatrion de(s) fiches du login.{0}<br/>"
                , loginId);
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            if (!string.IsNullOrEmpty(loginId)) message += string.Format("Login Id : {0}", loginId);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur libération de fiches", e);
        }
    }

    [WebMethod]
    public static List<Product> GetProductsBySegment(long segmentId)
    {
        try
        {
            IResults results = new Results();
            return results.GetProductsBySegment(segmentId);
        }
        catch (Exception e)
        {
            string message = string.Format(" Erreur pour obtenir la liste des produits du segment.{0}<br/>", segmentId);
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            message += string.Format("Segment Id : {0}", segmentId);
           Utils.SendErrorMail(message, e);
            throw new Exception("Erreur obtention produit par segment", e);
        }
    }

}