using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.WebService.Domain;
using TNS.AdExpress.WebService.Domain.XmlLoader;
using TNS.FrameWork.Exceptions;
using TNSMail = TNS.FrameWork.Net.Mail;
using TNS.AdExpress.WebService.Domain.Configuration;

namespace WebServiceCreativeView {
    public class Global : System.Web.HttpApplication {

        #region Constante
        /// <summary>
        /// Name of the configuration directory
        /// </summary>
        private const string CONFIGARION_DIRECTORY_NAME = "Configuration";
        #endregion

        #region Methods
        /// <summary>
        /// Application Start
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">Event arg</param>
        protected void Application_Start(object sender, EventArgs e) {
            string pathConfCountry = string.Empty;
            try {
                
                string pathConf = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIGARION_DIRECTORY_NAME);
                string countryName = WebParamtersXL.LoadDirectoryName(new XmlReaderDataSource(System.IO.Path.Combine(pathConf, TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME)));
                pathConfCountry = System.IO.Path.Combine(pathConf, countryName);
              
                VehiclesCreativesInformation.Init(VehiclesCreativesInformationDataAccess.Load(new XmlReaderDataSource(System.IO.Path.Combine(pathConfCountry, "VehicleCreativesAccess.xml"))));

                Media.LoadBaalLists(new XmlReaderDataSource(System.IO.Path.Combine(pathConfCountry, TNS.AdExpress.Constantes.Web.ConfigurationFile.BAAL_CONFIGURATION_FILENAME)));
            }
            catch (System.Exception error) {
                string body = "";
                try {
                    BaseException err = (BaseException)error;
                    body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>Server intialization failed.</font><br>Erreur" + err.GetHtmlDetail() + "</font></html>";
                }
                catch (System.Exception) {
                    try {
                        body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>Server intialization failed.</font><br>Erreur(" + error.GetType().FullName + "):" + error.Message + "<br><br><b><u>Source:</u></b><font color=#008000>" + error.StackTrace.Replace("at ", "<br>at ") + "</font></html>";
                    }
                    catch (System.Exception es) {
                        throw (es);
                    }
                }
                TNSMail.SmtpUtilities errorMail = new TNSMail.SmtpUtilities(pathConfCountry +@"\"+ TNS.AdExpress.Constantes.Web.ErrorManager.WEBSERVER_ERROR_MAIL_FILE);
                errorMail.Send("Initialization error of AdExpress creatives Web service" + (Server.MachineName), body, true, false);
                throw (error);
            }
        }

        protected void Application_End(object sender, EventArgs e) {

        }
        #endregion
    }
}