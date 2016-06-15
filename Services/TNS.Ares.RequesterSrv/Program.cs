using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using TNS.Ares.Domain.LS;

namespace TNS.Ares.RequesterSrv {
    /// <summary>
    /// Class d'entrée principal de l'application
    /// </summary>
    public class RequesterSrvMain {

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        private RequesterSrvMain() {
        }
        #endregion

        #region Main
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        public static void Main(string[] args) {
            System.ServiceProcess.ServiceBase.Run(new RequesterSrvHandler(new RequesterSrvMain()));
        }
        #endregion
    }
}
