using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.PromoPSA.BusinessEntities;
using KMI.PromoPSA.BusinessEntities.Classification;

namespace KMI.PromoPSA.Rules {
    /// <summary>
    /// Result Interface
    /// </summary>
    public interface IResults {
        /// <summary>
        /// Get PSA Login Id
        /// </summary>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        Int64 GetPSALoginId(string login, string password);
        /// <summary>
        /// Can Access To PSA
        /// </summary>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        bool CanAccessToPSA(string login, string password);
        /// <summary>
        /// Get Adverts
        /// </summary>
        /// <param name="loadDate">loaded Date</param>
        /// <returns></returns>
        List<Advert> GetAdverts(long loadDate);
        /// <summary>
        /// Get Nb Adverts
        /// </summary>
        /// <param name="loadDate">loaded Date</param>
        /// <param name="activationCode">activation Code</param>
        /// <returns></returns>
        int GetNbAdverts(long loadDate, long activationCode);

        List<LoadDateBE> GetLoadDates();
        /// <summary>
        /// Get codification
        /// </summary>
        /// <param name="promotionId">Promotion Id</param>
        /// <returns></returns>
        Codification GetCodification(long promotionId);
        /// <summary>
        /// Get Products BySegment
        /// </summary>
        /// <param name="segmentId"></param>
        /// <returns></returns>
        List<Product> GetProductsBySegment(long segmentId);

        /// <summary>
        /// Update Codification
        /// </summary>
        /// <param name="advert"></param>
        void UpdateCodification(Advert advert);

        /// <summary>
        /// Update Codification
        /// </summary>
        /// <param name="promotionId">Promotion Id</param>
        /// <param name="activationCode">activation Code</param>
        void UpdateCodification(long promotionId,  long activationCode);

        /// <summary>
        /// Change Advert Status
        /// </summary>
        /// <param name="promotionId">Promotion Id</param>
        /// <param name="activationCode"></param>
        void ChangeAdvertStatus(long promotionId, long activationCode);
        /// <summary>
        /// Get Available Promotion Id
        /// </summary>
        /// <param name="loginId"></param>      
        /// <returns></returns>
        long GetAvailablePromotionId(long loginId);
        /// <summary>
        /// Release User 
        /// </summary>
        /// <param name="loginId">Login Id</param>
        void ReleaseUser(long loginId);
        /// <summary>
        /// Lock Advert Status
        /// </summary>
        /// <param name="loginId">Login id</param>
        /// <param name="promotionId">Promotion Id</param>
        bool LockAdvertStatus(long loginId, long promotionId);
        /// <summary>
        /// Validate Month
        /// </summary>
        /// <param name="month">Month</param>
        bool ValidateMonth(long month);
    }
}
