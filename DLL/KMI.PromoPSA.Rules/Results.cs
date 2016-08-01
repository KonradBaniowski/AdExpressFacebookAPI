﻿using System;
using System.Collections.Generic;
using System.Linq;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using KMI.PromoPSA.BusinessEntities;
using KMI.PromoPSA.BusinessEntities.Classification;
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

        #region UpdateCodification

        public void UpdateCodification(long promotionId, long activationCode)
        {
            using (var db = new DbManager(new GenericDataProvider
                                             (WebApplicationParameters.DBConfig.ProviderDataAccess)
                                         , WebApplicationParameters.DBConfig.ConnectionString))
            {
                var dal = new PromoPsaDAL();
                dal.UpdateCodification(db, promotionId, activationCode);
            }
        }

        public void UpdateCodification(Advert advert)
        {
            using (var db = new DbManager(new GenericDataProvider
                                              (WebApplicationParameters.DBConfig.ProviderDataAccess)
                                          , WebApplicationParameters.DBConfig.ConnectionString))
            {
                var dal = new PromoPsaDAL();
                var dal2 = new ClassificationDAL();
                long idCircuit = 0;
                long idCategory = 0;

                if (advert.IdBrand > 0)
                    idCircuit = dal2.GetBrand(db, Constantes.Constantes.DEFAULT_LANGUAGE,
                        advert.IdBrand).IdCircuit;
                if (advert.IdProduct > 0)
                    idCategory = dal2.GetProduct(db, Constantes.Constantes.DEFAULT_LANGUAGE,
                      advert.IdProduct).IdCategory;

                advert.IdCategory = idCategory;
                advert.IdCircuit = idCircuit;

                dal.UpdateCodification(db, advert);
            }
        }

        public long InsertPromotion(Advert advert) {
            using (var db = new DbManager(new GenericDataProvider
                                              (WebApplicationParameters.DBConfig.ProviderDataAccess)
                                          , WebApplicationParameters.DBConfig.ConnectionString)) {
                var dal = new PromoPsaDAL();

                return dal.InsertPromotion(db, advert);
            }
        }

        #endregion

        /// <summary>
        /// Change Advert Status
        /// </summary>
        /// <param name="promotionId">Promotion Id</param>
        /// <param name="activationCode"></param>
        public void ChangeAdvertStatus(long promotionId, long activationCode)
        {
            Dispacher.Dispacher dispacher = GetWebServiceDispacher();
            dispacher.ChangeAdvertStatus(promotionId, activationCode);
        }

        public long GetAvailablePromotionId(long loginId)
        {
            Dispacher.Dispacher dispacher = GetWebServiceDispacher();
           return dispacher.GetAvailablePromotionId(loginId);
        }

        public long GetDuplicatedPromotionId(long loginId, long promotionId) {
            Dispacher.Dispacher dispacher = GetWebServiceDispacher();
            return dispacher.GetDuplicatedPromotionId(loginId, promotionId);
        }

        public void ReleaseUser(long loginId) {
           
            Dispacher.Dispacher dispacher = GetWebServiceDispacher();
            dispacher.ReleaseAdvertStatus(loginId);
        }

        public bool LockAdvertStatus(long loginId, long formId) {
            Dispacher.Dispacher dispacher = GetWebServiceDispacher();
            return dispacher.LockAdvertStatus(loginId, formId);
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

        #region Get Adverts Details
        /// <summary>
        /// Get Adverts Details
        /// </summary>
        /// <param name="loadDate">Load Date</param>
        /// <returns>Advert List</returns>
        public List<Advert> GetAdvertsDetails(long loadDate) {
            List<Advert> adverts;
            using (var db = new DbManager(new GenericDataProvider(WebApplicationParameters.DBConfig.ProviderDataAccess)
                                          , WebApplicationParameters.DBConfig.ConnectionString)) {
                var dal = new PromoPsaDAL();
                adverts = dal.GetAdvertsDetails(db, loadDate);
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

        #region Get Segments
        /// <summary>
        /// Get Segments
        /// </summary>
        /// <returns>Segments</returns>
        public List<Segment> GetSegments() {

            List<Segment> segments;
            using (var db = new DbManager(new GenericDataProvider(WebApplicationParameters.DBConfig.ProviderDataAccess)
                                          , WebApplicationParameters.DBConfig.ConnectionString)) {
                var dal = new ClassificationDAL();
                segments = dal.GetSegments(db, Constantes.Constantes.DEFAULT_LANGUAGE);
            }

            return segments;
        }
        #endregion

        #region Get Products
        /// <summary>
        /// Get Products
        /// </summary>
        /// <returns>Products</returns>
        public List<Product> GetProducts() {

            List<Product> products;
            using (var db = new DbManager(new GenericDataProvider(WebApplicationParameters.DBConfig.ProviderDataAccess)
                                          , WebApplicationParameters.DBConfig.ConnectionString)) {
                var dal = new ClassificationDAL();
                products = dal.GetProducts(db, Constantes.Constantes.DEFAULT_LANGUAGE);
            }

            return products;
        }
        #endregion

        #region Get Brands
        /// <summary>
        /// Get Brands
        /// </summary>
        /// <returns>Products</returns>
        public List<Brand> GetBrands() {

            List<Brand> brands;
            using (var db = new DbManager(new GenericDataProvider(WebApplicationParameters.DBConfig.ProviderDataAccess)
                                          , WebApplicationParameters.DBConfig.ConnectionString)) {
                var dal = new ClassificationDAL();
                brands = dal.GetBrands(db, Constantes.Constantes.DEFAULT_LANGUAGE);
            }

            return brands;
        }
        #endregion

        #region GetCodification

        public Codification GetCodification(long promotionId)
        {
          
            var codification = new Codification();
            using (var db = new DbManager(new GenericDataProvider
                (WebApplicationParameters.DBConfig.ProviderDataAccess)
                                          , WebApplicationParameters.DBConfig.ConnectionString))
            {
                var dal = new PromoPsaDAL();
                codification.Advert = dal.GetOneAdvertByPromotionId(db, promotionId).First();
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

      

       
        #endregion

        #endregion
    }
}