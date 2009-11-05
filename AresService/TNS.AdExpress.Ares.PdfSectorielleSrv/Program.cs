﻿using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace TNS.AdExpress.Ares.PdfSectorielleSrv
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
				new PdfSectorielleService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}