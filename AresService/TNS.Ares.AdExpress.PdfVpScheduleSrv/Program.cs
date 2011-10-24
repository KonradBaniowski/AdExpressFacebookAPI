﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace TNS.Ares.AdExpress.PdfVpScheduleSrv
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
				new VpScheduleService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}