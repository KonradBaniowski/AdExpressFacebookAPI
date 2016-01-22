using System;
using System.Diagnostics;
using BLToolkit.Data.DataProvider;

namespace KM.AdExpress.AlertPreRoll.DAL
{
    public class PreRollDbConnectionFactory : IPreRollDbConnectionFactory
    {
        private readonly string _connexionString;
        private readonly DataProviderBase _provider;


        public PreRollDbConnectionFactory(string connexionString, DataProviderBase provider)
        {
            _connexionString = connexionString;
            _provider = provider;
        }

        public static long GetSequenceId(String schema, String tableName, PreRollDb db)
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

        public PreRollDb CreateDbManager()
        {
            return new PreRollDb(_connexionString, _provider);
        }
    }
}
