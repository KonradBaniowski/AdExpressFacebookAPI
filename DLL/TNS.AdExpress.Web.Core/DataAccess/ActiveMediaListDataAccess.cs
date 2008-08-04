#region Information
// Author: Y. R'kaina
// Creation date: 21/06/2007
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;

namespace TNS.AdExpress.Web.Core.DataAccess{

    /// <summary>
    /// DataAccess utilisé pour renvoyer la liste des supports actifs pour un vehicle
    /// </summary>
    public class ActiveMediaListDataAccess{

        #region Get DATA
        /// <summary>
        /// Méthode utilisée pour renvoyer la liste des supports actifs pour un vehicle
        /// </summary>
        /// <param name="vehicleId">Id du vehicle</param>
        /// <returns></returns>
        public static DataSet GetActiveMediaData(Int64 vehicleId, int siteLanguage) {

            StringBuilder sql = new StringBuilder(500);

            TNS.FrameWork.DB.Common.IDataSource dataSource = new TNS.FrameWork.DB.Common.OracleDataSource(new OracleConnection(DBConstantes.Connection.DEV_CONNECTION_STRING));

            sql.Append("Select distinct m.id_media ");
            sql.Append("from " + GetVehicleTableName(vehicleId) + ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".media m");
            sql.Append(" where m.id_media = " + GetVehiclePrefixe(vehicleId) + ".id_media");
            sql.AppendFormat(" and m.id_language = {0}", siteLanguage);
            sql.Append(" and m.activation <" + DBConstantes.ActivationValues.UNACTIVATED.ToString());

            #region Execution de la requête
            try{
                return dataSource.Fill(sql.ToString());
            }
            catch (System.Exception err){
                throw (new TNS.AdExpress.Web.Core.Exceptions.ActiveMediaListDataAccessException("Impossible de charger pour les insertions: " + sql, err));
            }
            #endregion
        }
        #endregion

        #region Méthodes privées
        /// <summary>
		/// Indique la table à utilisée pour la requête
		/// </summary>
		/// <param name="vehicleId">Identifiant du media</param>
		/// <returns>La table correspondant</returns>
		private static string GetVehicleTableName(Int64 vehicleId){

			switch((DBClassificationConstantes.Vehicles.names)Convert.ToInt32(vehicleId.ToString())){
				case DBClassificationConstantes.Vehicles.names.internet:
                    return DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.DATA_INTERNET + " " + DBConstantes.Tables.DATA_INTERNET_PREFIXE;
				default:
					throw(new TNS.AdExpress.Web.Core.Exceptions.ActiveMediaListDataAccessException("Impossible de déterminer la table media à utiliser"));
			}
			
		}

        /// <summary>
        /// Indique le prefixe à utilisé pour la requête
        /// </summary>
        /// <param name="vehicleId">Identifiant du media</param>
        /// <returns>Le prefixe correspondant</returns>
        private static string GetVehiclePrefixe(Int64 vehicleId){

            switch ((DBClassificationConstantes.Vehicles.names)Convert.ToInt32(vehicleId.ToString())){
                case DBClassificationConstantes.Vehicles.names.internet:
                    return DBConstantes.Tables.DATA_INTERNET_PREFIXE;
                default:
                    throw (new TNS.AdExpress.Web.Core.Exceptions.ActiveMediaListDataAccessException("Impossible de déterminer le prefixe à utiliser"));
            }

        }
        #endregion

    }
}
