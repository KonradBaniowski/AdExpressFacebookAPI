using System;
using System.Diagnostics;
using BLToolkit.Data.DataProvider;
using Convert = BLToolkit.Common.Convert;

namespace TNS.AdExpress.Rolex.Loader.DAL
{
  public  class DataAccessDbConnectionFactory : IDataAccessDbConnectionFactory
    {
        private readonly string _connexionString;
        private readonly DataProviderBase _provider;


        public DataAccessDbConnectionFactory(string connexionString, DataProviderBase provider)
        {
            _connexionString = connexionString;
            _provider = provider;
        }

        public static long GetSequenceId(String schema, String tableName, DataAccessDb db)
        {
            string strQuery = string.Empty;
            try
            {
                strQuery = string.Format("SELECT {0}.SEQ_" + tableName + ".NEXTVAL FROM DUAL", schema);
                db.SetCommand(strQuery);
                object objResult = db.ExecuteScalar();
                return Convert.ToInt64(objResult);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(tableName + " : " + exception.Message);
                throw new Exception("Impossible to get sequence ID " + strQuery, exception);
            }
        }

        public DataAccessDb CreateDbManager()
        {
            return new DataAccessDb(_connexionString, _provider);
        }
    }
}
