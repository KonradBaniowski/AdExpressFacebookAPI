using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using KMI.PromoPSA.BusinessEntities;
using KMI.PromoPSA.DAL;
using KMI.PromoPSA.Rules.Exceptions;
using KMI.PromoPSA.Web.Domain;
using KMI.PromoPSA.Web.Domain.Configuration;

namespace KMI.PromoPSA.Rules {
    /// <summary>
    /// Results
    /// </summary>
    public class Results : IResults {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Results() {
        }
        #endregion

        #region Methods WebService Right

        #region Get PSA Login Id
        /// <summary>
        /// Get PSA Login Id
        /// </summary>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        /// <returns>Login Id</returns>
        public Int64 GetPSALoginId(string login, string password) {
            Rights.Rights rights = GetWebServiceIsisRights();
            return rights.GetEvaliantLoginId(login, password);
        }
        #endregion

        #region Can Access To PSA
        /// <summary>
        /// Can Access To PSA
        /// </summary>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        /// <returns>True if can access</returns>
        public bool CanAccessToPSA(string login, string password) {
            Rights.Rights rights = GetWebServiceIsisRights();
            return rights.CanAccessToEvaliant(login, password);
        }
        #endregion

        #region Get Web Service Isis Rights
        /// <summary>
        /// Get Web Service Classification
        /// </summary>
        /// <returns>Web Service Classification</returns>
        public static Rights.Rights GetWebServiceIsisRights() {
            Rights.Rights isisRights = new Rights.Rights();
            isisRights.Url = Web.Domain.Configuration.PromoPSAWebServices.GerWebService(WebServices.Names.rights).Url;
            isisRights.Timeout = Web.Domain.Configuration.PromoPSAWebServices.GerWebService(WebServices.Names.rights).Timeout;
            try {
                isisRights.IsAccessible();
            }
            catch (Exception err) {
                throw new WebServiceRightException("Error, Can't access to webservice right", err);
            }
            return isisRights;
        }
        #endregion

        #endregion

        #region Data Methods

        #region Get Adverts
        /// <summary>
        /// Get Adverts
        /// </summary>
        /// <param name="loadDate">Load Date</param>
        /// <returns>Advert List</returns>
        public List<Advert> GetAdverts(long loadDate) {
            List<Advert> adverts;
            using (var db = new DbManager(new GenericDataProvider(WebApplicationParameters.DBConfig.ProviderDataAccess)
                                          , WebApplicationParameters.DBConfig.ConnectionString)) {
                var dal = new PromoPsaDAL();
                adverts = dal.GetAdverts(db, loadDate);
            }
            return adverts;
        }
        #endregion

        #region Get Nb Adverts
        /// <summary>
        /// Get Nb Adverts
        /// </summary>
        /// <param name="loadDate">Load Date</param>
        /// <param name="activationCode"></param>
        /// <returns></returns>
        public int GetNbAdverts(long loadDate, long activationCode) {

            int advertsNb;
            using (var db = new DbManager(new GenericDataProvider(WebApplicationParameters.DBConfig.ProviderDataAccess)
                                          , WebApplicationParameters.DBConfig.ConnectionString)) {
                var dal = new PromoPsaDAL();
                advertsNb = dal.GetNbAdverts(db, loadDate, activationCode);
            }
            return advertsNb;

        }
        #endregion

        #region Get load Date
        /// <summary>
        /// Get load Date
        /// </summary>
        /// <returns>Load Dates</returns>
        public List<LoadDateBE> GetLoadDates() {

            List<LoadDateBE> loadDates;
            using (var db = new DbManager(new GenericDataProvider(WebApplicationParameters.DBConfig.ProviderDataAccess)
                                          , WebApplicationParameters.DBConfig.ConnectionString)) {
                var dal = new PromoPsaDAL();
                loadDates = dal.GetLoadDates(db);
            }

            return loadDates;
        }
        #endregion

        #endregion
    }
}
