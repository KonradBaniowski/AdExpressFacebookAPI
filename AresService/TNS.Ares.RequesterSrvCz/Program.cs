using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace TNS.Ares.RequesterSrvCz
{
    public class RequesterSrvMain
    {

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        private RequesterSrvMain()
        {
        }
        #endregion

        #region Main
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        public static void Main(string[] args)
        {
            System.ServiceProcess.ServiceBase.Run(new RequesterSrvHandler(new RequesterSrvMain()));
        }
        #endregion
    }
}
