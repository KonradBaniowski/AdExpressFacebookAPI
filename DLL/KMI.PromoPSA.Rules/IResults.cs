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
        /// <param name="loadDate"></param>
        /// <returns></returns>
        List<Advert> GetAdverts(long loadDate);
        /// <summary>
        /// Get Nb Adverts
        /// </summary>
        /// <param name="loadDate"></param>
        /// <param name="activationCode"></param>
        /// <returns></returns>
        int GetNbAdverts(long loadDate, long activationCode);

        List<LoadDateBE> GetLoadDates();
        /// <summary>
        /// Get codification
        /// </summary>
        /// <param name="idForm">id Form</param>
        /// <returns></returns>
        Codification GetCodification(long idForm);
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
        /// Change Advert Status
        /// </summary>
        /// <param name="idForm"></param>
        /// <param name="activationCode"></param>
        void ChangeAdvertStatus(long? idForm, long activationCode);
    }
}
