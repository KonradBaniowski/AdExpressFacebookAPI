
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
    }
}
