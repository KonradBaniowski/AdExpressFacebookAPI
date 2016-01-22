<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Mail" %>
<%@ Import Namespace="KMI.PromoPSA.Dispatcher.Core" %>

<%-- ReSharper disable CSharpWarnings::CS0612 --%>
<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        DispatcherConfig dispatcherConfig = GetConfiguration();
        try
        {
            Adverts.Load(dispatcherConfig);
        }
        catch (Exception ex)
        {

            SendErrorDetails(ex, dispatcherConfig);
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
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

    private void SendErrorDetails(Exception ex, DispatcherConfig dispatcherConfig)
    {
        MailMessage email = new MailMessage();
        email.From = dispatcherConfig.CustomerMailFrom;
        email.To = string.Join(";", dispatcherConfig.Recipients.ToArray());
        email.Subject = " Error PSA Dispacher Webservice(" + Server.MachineName+")";
          
        SmtpMail.SmtpServer = dispatcherConfig.CustomerMailServer;
        string body =string.Empty;

        try
        {
            body = "<html><head><meta http-equiv=\"Content-Language\" content=\"fr\">";
            body += "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1252\">";
            body += "</head><body>";
            body += string.Format("<b><u>{0}:</u></b><br>" + "<font color=#FF0000>An Exception occurs in dispacher Webservice.</font>"
                , Server.MachineName);
            Exception currentException = ex;
            while (currentException != null)
            {
                body += "<br>Error(" + currentException.GetType().FullName + "):" + currentException.Message + "<br><br><b><u>Source:</u></b><font color=#008000>" + currentException.StackTrace.Replace("at ", "<br>at ") + "</font>";
                body += "<br><hr>";
                currentException = currentException.InnerException;
            }
            body += "</body></html>";
            email.Body = body;
            SmtpMail.Send(email);
        }
        catch (Exception ex2)
        {
            Exception currentException = ex2;
            while (currentException != null)
            {
                body += "<br>Error(" + currentException.GetType().FullName + "):" + currentException.Message + "<br><br><b><u>Source:</u></b><font color=#008000>" + currentException.StackTrace.Replace("at ", "<br>at ") + "</font>";
                body += "<br><hr>";
                currentException = currentException.InnerException;
            }
            body += "</body></html>";
             body += " An dispacher Webservice  Exception occured during email sending.";
             email.Body = body;
             SmtpMail.Send(email);           
        }
        
    }

    private DispatcherConfig GetConfiguration()
    {
        var dispatcherConfig = new DispatcherConfig();

        dispatcherConfig.UserName = System.Web.Configuration.WebConfigurationManager.AppSettings["DbUserName"];
        dispatcherConfig.Password = System.Web.Configuration.WebConfigurationManager.AppSettings["DbUserName"];

        String connexionString =
               String.Format(
                   GetConnectionStringByProvider(System.Web.Configuration.WebConfigurationManager.AppSettings["ProviderDataAccess"]),
                   System.Web.Configuration.WebConfigurationManager.AppSettings["DbUserName"],
                   System.Web.Configuration.WebConfigurationManager.AppSettings["DbPassword"],
                   System.Web.Configuration.WebConfigurationManager.AppSettings["DbDataSource"]);
        dispatcherConfig.ConnectionString = connexionString;


        dispatcherConfig.ProviderDataAccess = System.Web.Configuration.WebConfigurationManager.AppSettings["ProviderDataAccess"];
        dispatcherConfig.CustomerMailFrom = System.Web.Configuration.WebConfigurationManager.AppSettings["Sender"];
        dispatcherConfig.CustomerMailServer = System.Web.Configuration.WebConfigurationManager.AppSettings["SmtpServer"];
        if (!string.IsNullOrEmpty(System.Web.Configuration.WebConfigurationManager.AppSettings["Recipients"]))
            dispatcherConfig.Recipients = new ArrayList(ConfigurationManager.AppSettings["Recipients"].Split(';'));
        if (!string.IsNullOrEmpty(System.Web.Configuration.WebConfigurationManager.AppSettings["SmtpPort"]))
            dispatcherConfig.CustomerMailPort = Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["SmtpPort"]);


        return dispatcherConfig;
    }

    private string GetConnectionStringByProvider(string providerName)
    {
        // Return null on failure.
        string returnValue = null;

        // Get the collection of connection strings.
        ConnectionStringSettingsCollection settings = ConfigurationManager.ConnectionStrings;

        // Walk through the collection and return the first 
        // connection string matching the providerName.
        if (settings != null)
        {
            foreach (ConnectionStringSettings cs in settings)
            {
                if (cs.ProviderName == providerName)
                {
                    returnValue = cs.ConnectionString;
                    break;
                }
            }
        }
        return returnValue;
    }


       
</script>
<%-- ReSharper restore CSharpWarnings::CS0612 --%>
