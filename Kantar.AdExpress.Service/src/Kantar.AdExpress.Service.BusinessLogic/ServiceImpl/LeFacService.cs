using System;
using System.Web;
using Kantar.AdExpress.Service.Core.BusinessService;
using TNS.AdExpress.Web.Core.Sessions;
using System.Data;
using System.Reflection;
using TNS.AdExpress.Constantes.DB;
using Kantar.AdExpress.Service.Core.Domain;
using NLog;
using TNS.AdExpress.Web.Utilities.Exceptions;
using KM.AdExpressI.LeFac.DAL;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class LeFacService : ILeFacService
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        public Core.Domain.LeFac GetLeFacInfos(string idWebSession, string idAddress, string advertiser, HttpContextBase httpContext)
        {
            WebSession customerWebSession = (WebSession)WebSession.Load(idWebSession);
            LeFac leFac = new LeFac();
            try
            {
                var param = new object[2];
                param[0] = customerWebSession;
                param[1] = idAddress;
                TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.leFacDAL];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the leFac DAL"));
                dynamic leFacDal = (ILeFacDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                DataSet data = leFacDal.GetData();
                string siren = string.Empty;
                string company = string.Empty;
                string street = string.Empty;
                string street2 = string.Empty;
                string codePostal = string.Empty;
                string town = string.Empty;
                string phone = string.Empty;
                string fax = string.Empty;
                string email = string.Empty;
                string docMarketingTarget = string.Empty;

                foreach (DataRow myRow in data.Tables[0].Rows)
                {

                    company = myRow["company"].ToString();
                    street = myRow["street"].ToString();
                    street2 = myRow["street2"].ToString();
                    codePostal = myRow["code_postal"].ToString();
                    town = myRow["town"].ToString();
                    phone = myRow["telephone"].ToString();
                    fax = myRow["fax"].ToString();
                    email = myRow["email"].ToString();
                    siren = myRow["siren_number"].ToString();
                }

                leFac = new LeFac
                {
                    Advertiser = advertiser,
                    Siren = siren,
                    Company = company,
                    Street = street,
                    Street2 = street2,
                    CodePostal = codePostal,
                    Town = town,
                    Phone = phone,
                    Fax = fax,
                    Email = email,
                    DocMarketingTarget = docMarketingTarget
                };
            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, customerWebSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return leFac;
        }
    }
}
