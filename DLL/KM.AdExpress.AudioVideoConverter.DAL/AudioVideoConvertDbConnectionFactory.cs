using BLToolkit.Data.DataProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KM.AdExpress.AudioVideoConverter.DAL
{


    public class AudioVideoConvertDbConnectionFactory : IAudioVideoConvertDbConnectionFactory
    {
        private readonly string _connexionString;
        private readonly DataProviderBase _provider;


        public AudioVideoConvertDbConnectionFactory(string connexionString, DataProviderBase provider)
        {
            _connexionString = connexionString;
            _provider = provider;
        }

        public static long GetSequenceId(String schema, String tableName, AudioVideoConverterDB db)
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

        public AudioVideoConverterDB CreateDbManager()
        {
            return new AudioVideoConverterDB(_connexionString, _provider);
        }
    }
}
