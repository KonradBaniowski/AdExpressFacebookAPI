using System;
using System.Collections.Generic;
using System.Linq;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using KMI.PromoPSA.BusinessEntities;
using KMI.PromoPSA.Dispatcher.DAL;

namespace KMI.PromoPSA.Dispatcher.Core
{
    [System.Serializable]
    public class Adverts
    {
        private static List<AdvertStatus> _adverts;

        private static DispatcherConfig _dispatcherConfig;
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Adverts()
        {
            _adverts = new List<AdvertStatus>();
        }
        #endregion

        #region Load / Reload

        /// <summary>
        /// Load / reload Object
        /// </summary> 
        /// <param name="dispatcherConfig">Dispatcher Config </param>       g
        public static void Load(DispatcherConfig dispatcherConfig)
        {
           
            _adverts = new List<AdvertStatus>();
            lock (_adverts)
            {
                _dispatcherConfig = dispatcherConfig;
                using (var db = new DbManager(new GenericDataProvider(_dispatcherConfig.ProviderDataAccess)
                    , _dispatcherConfig.ConnectionString))
                {
                    var dal = new DispatcherDAL();

                    _adverts = dal.GetAdverts(db); 
                    
                }
                          
            }
        }

        /// <summary>
        /// Load / reload Object
        /// </summary>       
        public static void ReLoad()
        {
            if (_dispatcherConfig == null) throw new Exception("Can't reload advert Collection, because advert Collection is don't load");
            Load(_dispatcherConfig);
        }
        #endregion

        public static List<AdvertStatus> GetAdvertStatus(long loginId, int nbAdvert)
        {
            lock (_adverts)
            {
                var adverts = _adverts.Where(p => p.IdLogin == Constantes.Constantes.NO_USER_VALUE
                && p.Activation == Constantes.Constantes.ACTIVATION_CODE_TO_CODIFY)
                .Take(nbAdvert).ToList();

                foreach (var advert in adverts)
                {
                    advert.IdLogin = loginId;
                }

                return adverts;
            }           
           
        }

        public static long GetAvailablePromotionId(long loginId)
        {
            lock (_adverts)
            {
                var advert = _adverts.FirstOrDefault(p => p.IdLogin == Constantes.Constantes.NO_USER_VALUE
                                                 && p.Activation == Constantes.Constantes.ACTIVATION_CODE_TO_CODIFY);

                if (advert != null)
                {
                    advert.IdLogin = loginId;
                    return advert.IdDataPromotion;
                }

                return 0;
            }    
        }

        public static long GetDuplicatedPromotionId(long loginId, long formId) {

            AdvertStatus duplicatedAdvert;

            using (var db = new DbManager(new GenericDataProvider(_dispatcherConfig.ProviderDataAccess)
                    , _dispatcherConfig.ConnectionString)) {
                var dal = new DispatcherDAL();

                duplicatedAdvert = dal.GetAdvertsByFormId(db, formId).Last();
                duplicatedAdvert.IdLogin = loginId;
            }

            lock (_adverts) {
                _adverts.Add(duplicatedAdvert);

                if (duplicatedAdvert != null) {
                    return duplicatedAdvert.IdDataPromotion;
                }

                return 0;
            }
        }

        public static void ReleaseAdvertStatus(long loginId)
        {
            lock (_adverts)
            {
                var adverts = _adverts.Where(p => p.IdLogin == loginId).ToList();

                foreach (var advert in adverts)
                {
                    advert.IdLogin = Constantes.Constantes.NO_USER_VALUE;
                }
              
            }

        }
        public static void ReleaseAdvertStatus(long loginId, long promotionId)
        {
            lock (_adverts)
            {
                var adverts = _adverts.Where(p => p.IdLogin == loginId
                    && p.IdDataPromotion == promotionId).ToList();

                foreach (var advert in adverts)
                {
                    advert.IdLogin = Constantes.Constantes.NO_USER_VALUE;
                }

            }

        }

        public static bool LockAdvertStatus(long loginId, long promotionId)
        {

            lock (_adverts)
            {
                bool status = false;
                var adverts = _adverts.Where(p => p.IdLogin == Constantes.Constantes.NO_USER_VALUE
                    && p.IdDataPromotion == promotionId).ToList();

                foreach (var advert in adverts)
                {
                    advert.IdLogin = loginId;
                    status = true;
                }
                return status;
            }

        }


        public static AdvertStatus GetAdvertStatus(long loginId, long promotionId)
        {
            lock (_adverts)
            {
                var advert = _adverts.Find(p => p.IdDataPromotion == promotionId
                  && p.IdLogin == Constantes.Constantes.NO_USER_VALUE);

                if (advert != null)
                {
                    advert.IdLogin = loginId;
                    return advert;
                }
                return null;
            }

        }

        public static void ChangeAdvertStatus(long? promotionId, long activationCode)
        {
            lock (_adverts)
            {
                var advertStatus = _adverts.Find(p => p.IdDataPromotion == promotionId);
                if (advertStatus != null)
                {
                    advertStatus.Activation = activationCode;
                    advertStatus.IdLogin = Constantes.Constantes.NO_USER_VALUE;
                }
            }

        }

        public static bool ValidateMonth(long loadDate)
        {
            lock (_adverts)
            {
                bool isValidated = false;
                var adverts = _adverts.Where(p => p.LoadDate == loadDate).ToList();

                foreach (var advert in adverts)
                {
                    advert.Activation = (advert.Activation==Constantes.Constantes.ACTIVATION_CODE_REJECTED)
                        ? Constantes.Constantes.ACTIVATION_CODE_INACTIVE : Constantes.Constantes.ACTIVATION_CODE_VALIDATED;
                    isValidated = true;
                }
                if (isValidated)
                {
                    using (var db = new DbManager(new GenericDataProvider(_dispatcherConfig.ProviderDataAccess)
                        , _dispatcherConfig.ConnectionString))
                    {
                        try
                        {
                            var dal = new DispatcherDAL();

                            //Delete rejected rows (set activation code to 50)
                            dal.UpdateMonth(db, loadDate
                               , Constantes.Constantes.ACTIVATION_CODE_INACTIVE, Constantes.Constantes.ACTIVATION_CODE_REJECTED);

                            //Validate codified rows
                            dal.UpdateMonth(db, loadDate
                               , Constantes.Constantes.ACTIVATION_CODE_VALIDATED, Constantes.Constantes.ACTIVATION_CODE_CODIFIED);

                        }
                        catch
                        {
                            isValidated = false;
                        }


                    }
                }
                return isValidated;
            }

        }
       
    }
}
