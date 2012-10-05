using System;
using BLToolkit.Data.DataProvider;

namespace TNS.AdExpress.Rolex.Loader.DAL
{
     public partial class DataAccessDAL
    {
        protected readonly object _instance = new object();
        public DataAccessDbConnectionFactory DataAccessDb { get; set; }
        private static Int64 _idUser;
        private static Int64 _idLanguage =33;
         private const string ROLEX_SCHEMA = "ROLEX03.";
         private const long ACTIVATED = 0;

        public DataAccessDAL(String connexionString, String providerName)
        {
            DataAccessDb = new DataAccessDbConnectionFactory(connexionString, new GenericDataProvider(providerName));

        }
    }
}
