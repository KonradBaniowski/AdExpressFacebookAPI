using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using KMI.PromoPSA.BusinessEntities;
using KMI.PromoPSA.Dispatcher.Core;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
[XmlInclude(typeof(AdvertStatus))]
[SoapInclude(typeof(AdvertStatus))]
public class Dispacher : System.Web.Services.WebService
{
    public Dispacher()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    #region IsAccessible
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public bool IsAccessible()
    {
        return true;
    }
    #endregion

    #region AdvertReload
    /// <summary>
    /// Advert Reload
    /// </summary>
    [WebMethod]
    public void AdvertReload()
    {
        Adverts.ReLoad();
    }
    #endregion

    #region GetAdvertStatus

    [WebMethod]
    public List<AdvertStatus> GetAdvertStatus(long loginId, int nbAdvert)
    {
        return Adverts.GetAdvertStatus(loginId, nbAdvert);
    }


    #endregion

    #region GetAdvertStatus

    [WebMethod]
    public long GetAvailableIdForm(long loginId)
    {
        return Adverts.GetAvailableIdForm(loginId);
    }

    #endregion


    #region ReleaseAdvertStatus

    [WebMethod]
    public void ReleaseAdvertStatus(long loginId)
    {
        Adverts.ReleaseAdvertStatus(loginId);
    }

    #endregion

    #region LockAdvertStatus

    [WebMethod]
    public bool LockAdvertStatus(long loginId, long idForm)
    {
       return Adverts.LockAdvertStatus(loginId, idForm);
    }

    #endregion
    
    #region ReleaseOneAdvertStatus

    [WebMethod]
    public void ReleaseOneAdvertStatus(long loginId, long idForm)
    {
        Adverts.ReleaseAdvertStatus(loginId, idForm);
    }

    #endregion

    #region GetOneAdvertStatus

    [WebMethod]
    public AdvertStatus GetOneAdvertStatus(long loginId, long idForm)
    {
        return Adverts.GetAdvertStatus(loginId, idForm);
    }

    #endregion

    #region ChangeAdvertStatus

    [WebMethod]
    public void ChangeAdvertStatus(long idForm, long activationCode)
    {
        Adverts.ChangeAdvertStatus(idForm, activationCode);
    }

    #endregion

    #region ChangeAdvertStatus

    [WebMethod]
    public bool ValidateMonth(long loadDate)
    {
        return  Adverts.ValidateMonth(loadDate);
    }

    #endregion
    
}