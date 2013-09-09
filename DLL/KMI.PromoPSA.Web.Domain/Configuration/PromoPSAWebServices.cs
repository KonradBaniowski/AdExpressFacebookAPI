using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.PromoPSA.Web.Domain.Exceptions;
using KMI.PromoPSA.Web.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;

namespace KMI.PromoPSA.Web.Domain.Configuration {
    /// <summary>
    /// Promo PSA Web Services Configurations
    /// </summary>
    public class PromoPSAWebServices {

        #region variables
        ///<summary>
        /// Vehicles description list
        /// </summary>
        private static Dictionary<WebServices.Names, PromoPSAWebService> _listWebService = new Dictionary<WebServices.Names, PromoPSAWebService>();
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructor
        /// </summary>
        static PromoPSAWebServices() {
        }
        #endregion

        #region Méthodes publiques

        #region Init
        /// <summary>
        /// Initialisation de la liste à partir du fichier XML
        /// </summary>
        /// <param name="source">Source de données</param>
        public static void Init(IDataSource source) {
            _listWebService.Clear();

            List<PromoPSAWebService> Services = PromoPSAWebServicesXL.Load(source);
            try {
                foreach (PromoPSAWebService currentWebService in Services) {
                    _listWebService.Add(currentWebService.WebServiceName, currentWebService);
                }
            }
            catch (System.Exception err) {
                throw (new PromoPSAWebServicesException("Impossible to build the vehicle list", err));
            }
        }
        #endregion

        #region Get Web Service
        /// <summary>
        /// Get Web Service
        /// </summary>
        /// <param name="webServiceName">eb Service Name</param>
        /// <returns>Web Service Configuration</returns>
        public static PromoPSAWebService GerWebService(WebServices.Names webServiceName) {
            return _listWebService[webServiceName];
        }
        #endregion

        #endregion

    }
}
