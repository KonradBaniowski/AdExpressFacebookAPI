using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using Oracle.DataAccess.Client;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using TNS.AdExpress.Web.Core.Sessions;
using System.Data;
using TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.StaticNavSession.DAL.Default
{
    public class StaticNavSessionDAL : TNS.AdExpress.StaticNavSession.DAL.StaticNavSessionDAL
    {
        public StaticNavSessionDAL(IDataSource source)
            : base(source)
        {
        }

        public override int InsertData(WebSession webSession, Anubis.Constantes.Result.type resultType, string fileName)
        {
            int idStaticNavSession = -1;

            // Opening connection
            OracleConnection connection = (OracleConnection)webSession.CustomerLogin.Source.GetSource();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            string insertCommand = "";
            MemoryStream ms = new MemoryStream(); ;
            BinaryFormatter bf = new BinaryFormatter();
            byte[] binaryData = null;

            try
            {
                OracleCommand command = connection.CreateCommand();

                // Serializing session
                bf.Serialize(ms, webSession);
                binaryData = new byte[ms.GetBuffer().Length];
                binaryData = ms.GetBuffer();

                // Preparing PL/SQL block
                insertCommand = @"
                    BEGIN
                     SELECT " + Schema.UNIVERS_SCHEMA + @".SEQ_STATIC_NAV_SESSION.NEXTVAL INTO :new_id FROM dual;
                     INSERT INTO " + Schema.UNIVERS_SCHEMA + "." + Tables.PDF_SESSION + @"
                        (ID_LOGIN, ID_STATIC_NAV_SESSION, STATIC_NAV_SESSION, ID_PDF_RESULT_TYPE, PDF_USER_FILENAME, STATUS, DATE_CREATION, DATE_MODIFICATION)
                        VALUES(" + webSession.CustomerLogin.IdLogin + ", :new_id, :blobtodb, " + resultType.GetHashCode() + ", '" + fileName + @"', 0, SYSDATE, SYSDATE);
                    END;";

                command.CommandText = insertCommand;

                // Fill parametres
                OracleParameter param2 = command.Parameters.Add("new_id", OracleDbType.Int64);
                param2.Direction = ParameterDirection.Output;
                OracleParameter param = command.Parameters.Add("blobtodb", OracleDbType.Blob);
                param.Direction = ParameterDirection.Input;
                param.Value = binaryData;

                // Execute PL/SQL block
                command.ExecuteNonQuery();
                idStaticNavSession = int.Parse(param2.Value.ToString());
            }
            catch (System.Exception e)
            {
            }
            finally
            {
                // Fermeture des structures
                if (ms != null) ms.Close();
                if (bf != null) bf = null;
                connection.Close();
                connection.Dispose();
            }

            return (idStaticNavSession);
        }
    }
}
