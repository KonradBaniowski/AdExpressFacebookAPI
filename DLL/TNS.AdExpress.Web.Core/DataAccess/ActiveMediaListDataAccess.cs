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

using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;

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
		/// <param name="siteLanguage">Site language</param>
        /// <returns></returns>
        public static DataSet GetActiveMediaData(Int64 vehicleId, int siteLanguage) {

            StringBuilder sql = new StringBuilder(500);
						
			TNS.FrameWork.DB.Common.IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.webAdministration);
			Table mediaTable;
			string tableName = Utilities.SQLGenerator.GetDataTableName(TNS.AdExpress.Constantes.Web.CustomerSessions.Period.PeriodBreakdownType.data, vehicleId, false);
			mediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media);
			sql.Append("Select distinct " + mediaTable.Prefix + ".id_media ");
			sql.Append("from " + tableName + " , " + mediaTable.SqlWithPrefix);
			sql.Append(" where " + mediaTable.Prefix + ".id_media = " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media");
			sql.AppendFormat(" and " + mediaTable.Prefix + ".id_language = "+ siteLanguage);
			sql.Append(" and " + mediaTable.Prefix + ".activation <" + DBConstantes.ActivationValues.UNACTIVATED.ToString());

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

     

    }
}
