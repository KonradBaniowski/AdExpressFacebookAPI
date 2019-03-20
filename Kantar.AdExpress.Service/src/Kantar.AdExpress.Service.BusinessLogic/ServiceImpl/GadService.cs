using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Utilities.Exceptions;
using TNS.AdExpressI.GAD.DAL;
using TNS.FrameWork.DB.Common;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class GadService : IGadService
    {
        private static Logger Logger= LogManager.GetCurrentClassLogger();
        public Gad GetGadInfos(string idWebSession, string idAddress, string advertiser, HttpContextBase httpContext)
        {
            WebSession customerWebSession = (WebSession)WebSession.Load(idWebSession);
            Gad gad = new Gad();
            try
            {
                var param = new object[2];
                param[0] = customerWebSession;
                param[1] = idAddress;
                TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.gadDAL];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the gad DAL"));
                dynamic gadDal = (IGadDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                DataSet data = gadDal.GetData();
                string siret = string.Empty;
                string company = string.Empty;
                string street = string.Empty;
                string street2 = string.Empty;
                string codePostal = string.Empty;
                string town = string.Empty;
                string phone = string.Empty;
                string fax = string.Empty;
                string email = string.Empty;
                string docMarketingId = string.Empty;
                string docMarketingKey = string.Empty;
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
                    if (customerWebSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.Customer.DB.Flag.id.gad.GetHashCode()))
                    {
                        docMarketingId = myRow["id_gad"].ToString();
                        docMarketingKey = myRow["docKey"].ToString();
                    }
                    if (customerWebSession.CustomerLogin.CustormerFlagAccess(Flags.ID_CODE_SIRET_ACCESS_FLAG) && myRow["siret_number"] != DBNull.Value)
                    {
                        siret = myRow["siret_number"].ToString();
                    }
                }

                if (!string.IsNullOrEmpty(company) && !string.IsNullOrEmpty(docMarketingId))
                {
                    docMarketingTarget = string.Format("http://www3.docmarketing.fr/front/societe/{0},{1}.html", company, docMarketingId);
                }
                else
                {
                    docMarketingTarget = "";
                }

                gad = new Gad
                {
                    Advertiser = advertiser,
                    Siret = siret,
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
                if (customerWebSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, customerWebSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return gad;
        }

        public Gad GetGadInfos(string idAddress, string advertiser, HttpContextBase httpContext)
        {
            Gad gad = new Gad();
            TNS.FrameWork.DB.Common.IDataSource source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);

            try
            {
                var param = new object[2];
                param[0] = source;
                param[1] = idAddress;
                TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.gadDAL];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the gad DAL"));
                dynamic gadDal = (IGadDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                DataSet data = gadDal.GetData();
                string siret = string.Empty;
                string company = string.Empty;
                string street = string.Empty;
                string street2 = string.Empty;
                string codePostal = string.Empty;
                string town = string.Empty;
                string phone = string.Empty;
                string fax = string.Empty;
                string email = string.Empty;
                string docMarketingId = string.Empty;
                string docMarketingKey = string.Empty;
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
                }

                if (!string.IsNullOrEmpty(company) && !string.IsNullOrEmpty(docMarketingId))
                {
                    docMarketingTarget = string.Format("http://www3.docmarketing.fr/front/societe/{0},{1}.html", company, docMarketingId);
                }
                else
                {
                    docMarketingTarget = "";
                }

                gad = new Gad
                {
                    Advertiser = advertiser,
                    Siret = siret,
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
                throw new Exception();
            }

            return gad;
        }
    }
}
