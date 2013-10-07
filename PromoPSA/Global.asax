<%@ Application Language="C#" %>
<%@ Import Namespace="KMI.PromoPSA.Rules" %>
<%@ Import Namespace="KMI.PromoPSA.Web.Core.Sessions" %>
<%@ Import Namespace="KMI.PromoPSA.Web.Domain" %>
<%@ Import Namespace="TNS.FrameWork.Exceptions" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        try
        {
            Int64 dd = WebApplicationParameters.DefaultLanguage;

       
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
                    body = string.Format("<html><b><u>{0}:</u></b><br>" + "<font color=#FF0000>L'initialisation du server a &eacute;chou&eacute;.</font><br>Erreur({1}):{2}<br><br><b><u>Source:</u></b><font color=#008000>{3}</font></html>"
                        , Server.MachineName, error.GetType().FullName, error.Message, error.StackTrace.Replace("at ", "<br>at "));
                }
                catch (System.Exception es)
                {
                    throw (es);
                }
            }
            TNS.FrameWork.Net.Mail.SmtpUtilities errorMail = new TNS.FrameWork.Net.Mail.SmtpUtilities(AppDomain.CurrentDomain.BaseDirectory
                + KMI.PromoPSA.Constantes.Configuration.DIRECTORY_CONFIGURATION + KMI.PromoPSA.Constantes.Configuration.FILE_ERROR_MAIL);
            errorMail.SendWithoutThread("PSA Promo Web site Error (" + Server.MachineName + ")", body, true, false);
            throw (error);
        }

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        Session[KMI.PromoPSA.Constantes.WebSession.WEB_LANGUAGE_ID] = KMI.PromoPSA.Constantes.Constantes.DEFAULT_LANGUAGE;

    }

    void Session_End(object sender, EventArgs e) 
    {
        WebSession webSession = (WebSession)Session[KMI.PromoPSA.Constantes.WebSession.WEB_SESSION];
        if (webSession != null && WebSessions.Contains(webSession.CustomerLogin.IdLogin))
        {           
            IResults results = new Results();
            results.ReleaseUser(webSession.CustomerLogin.IdLogin);           
            WebSessions.Remove(webSession.CustomerLogin.IdLogin);
        }

    }
       
</script>
