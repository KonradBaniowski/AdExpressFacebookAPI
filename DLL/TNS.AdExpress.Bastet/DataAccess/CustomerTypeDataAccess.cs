#region Informations
// Auteur: B. Masson
// Date de création: 02/03/2006
// Date de modification:
#endregion

using System;
using System.Collections;
using System.Data;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Bastet.Exceptions;
using DbSchema = TNS.Isis.Constantes.DataBase;
using DbTables = TNS.Isis.Constantes.Tables;
using AdExpressDBConstante = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Bastet.Web;
using TNS.AdExpress.Domain.DataBaseDescription;

namespace TNS.AdExpress.Bastet.DataAccess{
	/// <summary>
	/// Classe d'accès aux données pour la récupération des types de client
	/// </summary>
	public class CustomerTypeDataAccess{
		
		/// <summary>
		/// Obtient les types de client
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <returns>Données des types de client</returns>
		internal static DataSet GetCustomerType(IDataSource source){

			#region Requête
			StringBuilder sql = new StringBuilder();
            Table rightContactGroup = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContactGroup);
            
			sql.Append(" select id_group_contact, group_contact ");
            sql.Append(" from " + rightContactGroup.Sql + " ");
			sql.Append(" where group_contact like 'Client%' ");
			sql.Append(" and activation < "+ AdExpressDBConstante.ActivationValues.UNACTIVATED);
			sql.Append(" order by group_contact");
			#endregion

			#region Execution
			try{
				return(source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw (new CustomerTypeDataAccessException("GetCustomerType : Impossible d'obtenir les types de client", err));
			}
			#endregion

		}

	}
}
