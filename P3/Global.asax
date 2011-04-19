<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        try {
        // Code qui s'exécute au démarrage de l'application
        KMI.P3.Domain.Web.WebApplicationParameters.Initialize();
        }
        catch (System.Exception error)
        {
            string body = "";
            try
            {
                TNS.FrameWork.Exceptions.BaseException err = (TNS.FrameWork.Exceptions.BaseException)error;
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
            TNS.FrameWork.Net.Mail.SmtpUtilities errorMail = new TNS.FrameWork.Net.Mail.SmtpUtilities(KMI.P3.Domain.Web.WebApplicationParameters.ConfigurationDirectoryRoot + KMI.P3.Constantes.Web.ErrorManager.WEBSERVER_ERROR_MAIL_FILE);
            errorMail.Send("Erreur d'initialisation d'EasyMusic " + (Server.MachineName), body, true, false);
            throw (error);
        }

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code qui s'exécute à l'arrêt de l'application

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code qui s'exécute lorsqu'une erreur non gérée se produit

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code qui s'exécute lorsqu'une nouvelle session démarre

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code qui s'exécute lorsqu'une session se termine. 
        // Remarque : l'événement Session_End est déclenché uniquement lorsque le mode sessionstate
        // a la valeur InProc dans le fichier Web.config. Si le mode de session a la valeur StateServer 
        // ou SQLServer, l'événement n'est pas déclenché.

    }
       
</script>
