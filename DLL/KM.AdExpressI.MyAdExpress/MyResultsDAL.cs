
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
		public static void RenameSession(string newName, Int64 idMySession, WebSession webSession)
        {
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
            }
            catch (System.Exception err)
            {
                //throw (new MyAdExpressDataAccessException("Impossible de Renommer une session sauvegardée", err));
            }
            #endregion
        }
    }
}
