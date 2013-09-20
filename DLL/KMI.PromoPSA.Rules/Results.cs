using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using KMI.PromoPSA.BusinessEntities;
using KMI.PromoPSA.BusinessEntities.Classification;
using KMI.PromoPSA.DAL;
using KMI.PromoPSA.Rules.Dispacher;
using KMI.PromoPSA.Rules.Exceptions;
using KMI.PromoPSA.Web.Domain;
using KMI.PromoPSA.Web.Domain.Configuration;
using KMI.PromoPSA.Constantes;
using AdvertStatus = KMI.PromoPSA.BusinessEntities.AdvertStatus;

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

        #region Methods WebService Dispatcher

        #region Get Web Service Dispatcher
        /// <summary>
        /// Get Web Service Dispatcher
        /// </summary>
        /// <returns>Web Service Dispatcher</returns>
        public static Dispacher.Dispacher GetWebServiceDispacher()
        {
            var psaDispatcher = new Dispacher.Dispacher
                {
                    Url = PromoPSAWebServices.GerWebService(WebServices.Names.dispacher).Url,
                    Timeout = PromoPSAWebServices.GerWebService(WebServices.Names.dispacher).Timeout
                };

            try
            {
                psaDispatcher.IsAccessible();
            }
            catch (Exception err)
            {
                throw new WebServiceRightException("Error, Can't access to Dispatcher webservice", err);
            }
            return psaDispatcher;
        }
        #endregion

        /// <summary>
        /// Change Advert Status
        /// </summary>
        /// <param name="idForm"></param>
        /// <param name="activationCode"></param>
        public void ChangeAdvertStatus(long? idForm, long activationCode)
        {
            Dispacher.Dispacher dispacher = GetWebServiceDispacher();
            dispacher.ChangeAdvertStatus(idForm,activationCode);
        }

        public long GetAvailableIdForm(long loginId)
        {
            Dispacher.Dispacher dispacher = GetWebServiceDispacher();
           return dispacher.GetAvailableIdForm(loginId);
        }

        public void ReleaseUser(long loginId) {
            Dispacher.Dispacher dispacher = GetWebServiceDispacher();
            dispacher.ReleaseAdvertStatus(loginId);
        }

        public bool LockAdvertStatus(long loginId, long formId) {
            Dispacher.Dispacher dispacher = GetWebServiceDispacher();
            return  dispacher.LockAdvertStatus(loginId, formId);
        }

        public bool ValidateMonth(long month) {
            Dispacher.Dispacher dispacher = GetWebServiceDispacher();
            return dispacher.ValidateMonth(month);
        }

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

        #region GetCodification

        public Codification GetCodification(long idForm)
        {
            var codification = new Codification();
            using (var db = new DbManager(new GenericDataProvider
                (WebApplicationParameters.DBConfig.ProviderDataAccess)
                                          , WebApplicationParameters.DBConfig.ConnectionString))
            {
                var dal = new PromoPsaDAL();
                codification.Advert = dal.GetOneAdvert(db, idForm).First();
                codification.CurrentBrand = codification.Advert.IdBrand;
                codification.CurrentProduct = codification.Advert.IdProduct;
                codification.CurrentSegment = codification.Advert.IdSegment;

                var dal2 = new ClassificationDAL();
                codification.Brands = dal2.GetBrands(db, Constantes.Constantes.DEFAULT_LANGUAGE);
                codification.Products = dal2.GetProducts(db, Constantes.Constantes.DEFAULT_LANGUAGE);
                codification.Segments = dal2.GetSegments(db, Constantes.Constantes.DEFAULT_LANGUAGE);

                if (codification.CurrentSegment > 0)
                {
                    codification.CurrentProducts = dal2.GetProductBySegment(db, 
                        Constantes.Constantes.DEFAULT_LANGUAGE, codification.CurrentSegment);
                }

            }
            return codification;
        }

        public List<Product> GetProductsBySegment(long segmentId)
        {
            List<Product> products;
            using (var db = new DbManager(new GenericDataProvider
                                              (WebApplicationParameters.DBConfig.ProviderDataAccess)
                                          , WebApplicationParameters.DBConfig.ConnectionString))
            {
                var dal = new ClassificationDAL();
                products = dal.GetProductBySegment(db,
                        Constantes.Constantes.DEFAULT_LANGUAGE, segmentId);
            }
            return products;
        }

        public void UpdateCodification(Advert advert)
        {
            using (var db = new DbManager(new GenericDataProvider
                                              (WebApplicationParameters.DBConfig.ProviderDataAccess)
                                          , WebApplicationParameters.DBConfig.ConnectionString))
            {
                var dal = new PromoPsaDAL();
               dal.UpdateCodification(db,advert);
            }
        }

       
        #endregion

        #endregion
    }
}
