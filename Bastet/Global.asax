<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        new TNS.AdExpress.Bastet.Web.WebApplicationParameters();
        // Code qui s'exécute au démarrage de l'application
         TNS.AdExpress.Bastet.Common.Headers.HeaderList.Init(new TNS.FrameWork.DB.Common.XmlReaderDataSource(TNS.AdExpress.Bastet.Web.WebApplicationParameters.CountryConfigurationDirectoryRoot + @"HeaderConfiguration.xml"));
         TNS.Ares.Domain.DataBase.DataBaseConfiguration.Load(new TNS.FrameWork.DB.Common.XmlReaderDataSource(TNS.AdExpress.Bastet.Web.WebApplicationParameters.CountryConfigurationDirectoryRoot + TNS.Ares.Constantes.ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));
         TNS.Ares.Domain.LS.PluginConfiguration.Load(new TNS.FrameWork.DB.Common.XmlReaderDataSource(TNS.AdExpress.Bastet.Web.WebApplicationParameters.CountryConfigurationDirectoryRoot + TNS.Ares.Constantes.ConfigurationFile.PLUGIN_CONFIGURATION_FILENAME));
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
