#region Informations
// Auteur: D. V. Mussuma
// Date de création: 24/11/2005
// Date de modification:
#endregion

using System;
using System.Data;
using System.Text;

using BastetCommon=TNS.AdExpress.Bastet.Common;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using AnubisBastet=TNS.AdExpress.Anubis.Bastet;
using TNS.AdExpress.Bastet.Web;
using TNS.AdExpress.Domain.DataBaseDescription;



namespace TNS.AdExpress.Anubis.Bastet.DataAccess
{
	/// <summary>
	/// Obtient les données des module et groupes de modules les plus utilisés
	/// </summary>
	public class Module
	{	
		/// <summary>
		/// Obtient les données des module et groupes de modules les plus utilisés
		/// </summary>
		/// <param name="parameters">parametres des statistiques</param>
		/// <param name="toGroup">vrai si données concernent uniquement les groupes de modules</param>
		/// <returns>données des module et groupes de modules les plus utilisés</returns>
		public static DataTable TopUsed(BastetCommon.Parameters parameters,bool toGroup)
		{
			try{

				#region Requête
				StringBuilder sql = new StringBuilder(3000);
				Table moduleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModule);
				Table moduleGroupTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightModuleGroup);
				Table topModuleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingTopModule);

				//select
				sql.Append(" select ");
				if(!toGroup)sql.Append(" "+topModuleTable.Prefix+".ID_MODULE,"+moduleTable.Prefix+".MODULE,");
				sql.Append(" "+topModuleTable.Prefix+".ID_MODULE_GROUP,"+moduleGroupTable.Prefix+".MODULE_GROUP");
				sql.Append(",sum("+topModuleTable.Prefix+".CONNECTION_NUMBER) as CONNECTION_NUMBER  ");

				//From
				sql.Append(" from " + topModuleTable.SqlWithPrefix);
				sql.Append(" ," + moduleTable.SqlWithPrefix+ "," + moduleGroupTable.SqlWithPrefix);
			
				//Where
                sql.Append(" where " + topModuleTable.Prefix + ".date_connection  between " + parameters.PeriodBeginningDate.ToString("yyyyMMdd") + " and " + parameters.PeriodEndDate.ToString("yyyyMMdd"));
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+topModuleTable.Prefix+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+moduleTable.Prefix+".id_module="+topModuleTable.Prefix+".id_module ");
				sql.Append(" and "+moduleGroupTable.Prefix+".id_module_group="+topModuleTable.Prefix+".id_module_group ");
			
				//Gourp by
				sql.Append(" group by  ");
				if(!toGroup)sql.Append("  "+topModuleTable.Prefix+".ID_MODULE,"+moduleTable.Prefix+".MODULE,");
				sql.Append("  "+topModuleTable.Prefix+".ID_MODULE_GROUP,"+moduleGroupTable.Prefix+".MODULE_GROUP");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc");
				#endregion
				
				#region Execution
			
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion

			}
			catch(System.Exception err){
				throw (new  AnubisBastet.Exceptions.BastetDataAccessException(" TopUsed : Impossible to get most used module list ", err));
				
			}
			
		}
	}
}
