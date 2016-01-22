using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using TNS.FrameWork.Exceptions;
using TNSMail = TNS.FrameWork.Net.Mail;

namespace WebServiceSetAdScopeLogin
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                Parameters.ServiceParams.Initialize();
            }
            catch (System.Exception error)
            {
                string body = "";
                try
                {
                    BaseException err = (BaseException)error;
                    body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>L'initialisation du server a &eacute;chou&eacute;.</font><br>Erreur" + err.GetHtmlDetail() + "</font></html>";
                }  
                catch (System.Exception)
                {
                    try
                    {
                        body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>L'initialisation du server a &eacute;chou&eacute;.</font><br>Erreur(" + error.GetType().FullName + "):" + error.Message + "<br><br><b><u>Source:</u></b><font color=#008000>" + error.StackTrace.Replace("at ", "<br>at ") + "</font></html>";
                    }
                    catch (System.Exception es)
                    {
                        throw (es);
                    }
                }
                TNSMail.SmtpUtilities errorMail = new TNSMail.SmtpUtilities(AppDomain.CurrentDomain.BaseDirectory + WebServiceSetAdScopeLogin.Constantes.ConfigFile.CONFIGURATION_DIRECTORY_NAME + @"\" + Constantes.ErrorManager.WEBSERVER_ERROR_MAIL_FILE);
                errorMail.Send("Erreur d'initialisation du web service AdScope Modification login " + (Server.MachineName), body, true, false);
                throw (error);
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}