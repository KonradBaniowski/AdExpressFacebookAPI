using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <param name="dispatcherConfig">Dispatcher Config </param>       
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

        public static List<AdvertStatus> GetAdvertStatus(long userId, int nbAdvert)
        {
            lock (_adverts)
            {
                var adverts = _adverts.Where(p => p.IdUser == Constantes.Constantes.NO_USER_VALUE
                && p.Activation == Constantes.Constantes.ACTIVATION_CODE_TO_CODIFY)
                .Take(nbAdvert).ToList();

                foreach (var advert in adverts)
                {
                    advert.IdUser = userId;
                }

                return adverts;
            }           
           
        }

        public static void ReleaseAdvertStatus(long userId)
        {
            lock (_adverts)
            {
                var adverts = _adverts.Where(p => p.IdUser == userId).ToList();

                foreach (var advert in adverts)
                {
                    advert.IdUser = Constantes.Constantes.NO_USER_VALUE;
                }
              
            }

        }
        public static void ReleaseAdvertStatus(long userId,long idForm)
        {
            lock (_adverts)
            {
                var adverts = _adverts.Where(p => p.IdUser == userId
                    && p.IdForm==idForm).ToList();

                foreach (var advert in adverts)
                {
                    advert.IdUser = Constantes.Constantes.NO_USER_VALUE;
                }

            }

        }




        public static AdvertStatus GetAdvertStatus(long userId, long idForm)
        {
            lock (_adverts)
            {
              var  advert = _adverts.Find(p => p.IdForm == idForm 
                  && p.IdUser == Constantes.Constantes.NO_USER_VALUE);

                if (advert != null)
                {
                    advert.IdUser = userId;
                    return advert;
                }
                return null;
            }

        }

        public static void ChangeAdvertStatus(long idForm, long activationCode)
        {
            lock (_adverts)
            {
                var advertStatus = _adverts.Find(p => p.IdForm == idForm);
                if (advertStatus != null) advertStatus.Activation = activationCode;
            }

        }
       
    }
}
