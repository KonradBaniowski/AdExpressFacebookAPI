
using System;
using System.Data;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;

namespace KM.AdExpressI.MyAdExpress
{
    public class MyResultsDAL
    {
        /// <summary>
		/// Donne la liste des sessions d'un client qui sont enregistrées
		/// </summary>
		/// <remarks>Testé</remarks>
		/// <param name="webSession">Session du client</param>
		/// <returns>Listes de sessions</returns>
		public static DataSet GetData(WebSession webSession)
        {

            #region Construction de la requête
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerDirectory);
            Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);

            string sql = "select " + directoryTable.Prefix + ".ID_DIRECTORY, " + directoryTable.Prefix + ".DIRECTORY, " + mySessionTable.Prefix + ".ID_MY_SESSION, " + mySessionTable.Prefix + ".MY_SESSION ";
            sql += " from " + directoryTable.SqlWithPrefix + " , " + mySessionTable.SqlWithPrefix;
            sql += " where " + directoryTable.Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin.ToString();
            sql += " and " + directoryTable.Prefix + ".ACTIVATION<" + ActivationValues.UNACTIVATED;
            sql += " and (" + mySessionTable.Prefix + ".ACTIVATION<" + ActivationValues.UNACTIVATED + " or " + mySessionTable.Prefix + ".ACTIVATION is null) ";
            sql += " and " + directoryTable.Prefix + ".ID_DIRECTORY=" + mySessionTable.Prefix + ".ID_DIRECTORY(+) ";
            sql += " order by " + directoryTable.Prefix + ".DIRECTORY, " + mySessionTable.Prefix + ".MY_SESSION ";

            #endregion

            #region Execution de la requête
            try
            {
                return (webSession.Source.Fill(sql));
            }
            catch (System.Exception err)
            {
                throw (err);
                //throw (new MyAdExpressDataAccessException("Impossible d'obtenir la liste des sessions d'un client qui sont enregistrées", err));
            }
            #endregion

        }

        /// <summary>
		/// Déplace une session
		/// </summary>
		/// <remarks>Testée</remarks>
		/// <param name="idOldDirectory">Identifiant du Répertoire source</param>
		/// <param name="idNewDirectory">Identifiant du Répertoire de destination</param>
		/// <param name="idMySession">Identifiant du résultat</param>
		/// <param name="webSession">Session du client</param>	
		public static bool MoveSession(Int64 idOldDirectory, Int64 idNewDirectory, Int64 idMySession, WebSession webSession)
        {
            bool result = false;
            Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);

            #region requête
            string sql = "UPDATE " + mySessionTable.Sql;
            sql += " SET ID_DIRECTORY=" + idNewDirectory + ", DATE_MODIFICATION=sysdate ";
            sql += " WHERE ID_DIRECTORY=" + idOldDirectory + "";
            sql += " and ID_MY_SESSION=" + idMySession + "";
            #endregion

            #region Execution de la requête
            try
            {
                webSession.Source.Update(sql);
                result = true;
            }
            catch (System.Exception err)
            {
                //throw (new MyAdExpressDataAccessException("Impossible de Déplace une session", err));
            }
            return result;
            #endregion
        }

        /// <summary>
		///  Renomme une session
		/// </summary>
		/// <remarks>Testé</remarks>
		/// <param name="newName">Nouveau nom de la session</param>
		/// <param name="idMySession">Identifiant de la session</param>
		/// <param name="webSession">web Session</param>
		public static bool RenameSession(string newName, Int64 idMySession, WebSession webSession)
        {
            bool result = false;
            Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);

            #region requête
            string sql = "UPDATE " + mySessionTable.Sql;
            sql += " SET MY_SESSION ='" + newName + "', DATE_MODIFICATION=sysdate ";
            sql += " WHERE ID_MY_SESSION=" + idMySession + "";
            #endregion

            #region Execution de la requête
            try
            {
                webSession.Source.Update(sql);
                result = true;
            }
            catch (System.Exception err)
            {
                //throw (new MyAdExpressDataAccessException("Impossible de Renommer une session sauvegardée", err));
            }
            #endregion
            return result;
        }

        /// <summary>
		/// Suppression de la sauvegarde
		/// </summary>
		public static bool DeleteSession(long idSession, WebSession webSession)
        {
            bool result = false;
            if (idSession > 0)
            {
                #region Construction de la requête SQL
                Table mySessionTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerSessionSaved);

                string sql = "delete from " + mySessionTable.Sql;
                sql += " where id_my_session=" + idSession;
                #endregion

                #region Execution de la requête
                try
                {
                    webSession.Source.Delete(sql);
                    result = true;
                }
                catch (System.Exception err)
                {
                    //throw (new MySessionDataAccessException("Impossible de supprimer la sauvegarde", err));
                }
            }
            #endregion
            
            return result;
        }

        public static bool IsDirectoryExist(WebSession webSession, string DirectoryName)
        {
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerDirectory);
            bool result = false;
            #region Construction de la requête
            string sql = "select distinct " + directoryTable.Prefix + ".ID_DIRECTORY, " + directoryTable.Prefix + ".DIRECTORY ";
            sql += "  from " + directoryTable.SqlWithPrefix;
            sql += " where " + directoryTable.Prefix + ".ID_LOGIN=" + webSession.CustomerLogin.IdLogin;
            sql += " and " + directoryTable.Prefix + ".ACTIVATION<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ";
            sql += " and UPPER(" + directoryTable.Prefix + ".DIRECTORY)=UPPER('" + DirectoryName + "') ";
            #endregion

            #region Execution de la requête
            DataSet ds;
            try
            {
                ds = webSession.Source.Fill(sql);
                if (ds.Tables[0].Rows.Count > 0)
                    result = true;               
            }
            catch (System.Exception err)
            {
                //throw (new MyAdExpressDataAccessException("Impossible de vérifier l'existance d'un répertoire", err));
            }
            return result;
            #endregion
        }

        public static bool CreateDirectory(string nameDirectory, WebSession webSession)
        {
            Table directoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.customerDirectory);
            TNS.AdExpress.Domain.DataBaseDescription.Schema schema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);
            bool result = false;
            #region Requête pour insérer les champs dans la table Group_universe_client	
            string sql = "INSERT INTO " + directoryTable.Sql + "(ID_DIRECTORY,id_login,DIRECTORY,DATE_CREATION,ACTIVATION) ";
            sql += "  values (" + schema.Label + ".seq_directory.nextval," + webSession.CustomerLogin.IdLogin + ",'" + nameDirectory + "',SYSDATE," + TNS.AdExpress.Constantes.DB.ActivationValues.ACTIVATED + ")";
            #endregion

            #region Execution de la requête
            try
            {
                webSession.Source.Insert(sql);
                result = true;
            }
            catch (System.Exception)
            {
                
            }
            return result;
            #endregion
        }
    }
}
