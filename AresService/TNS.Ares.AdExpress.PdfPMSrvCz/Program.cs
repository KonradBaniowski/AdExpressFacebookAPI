using System.ServiceProcess;

namespace TNS.Ares.AdExpress.PdfPMSrvCz
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new PdfPMService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
