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

				//select
				sql.Append(" select ");
				if(!toGroup)sql.Append(" "+DBTables.TOP_MODULE_PREFIXE+".ID_MODULE,"+DBTables.MODULE_PREFIXE+".MODULE,");
				sql.Append(" "+DBTables.TOP_MODULE_PREFIXE+".ID_MODULE_GROUP,"+DBTables.MODULE_GROUP_PREFIXE+".MODULE_GROUP");
				sql.Append(",sum("+DBTables.TOP_MODULE_PREFIXE+".CONNECTION_NUMBER) as CONNECTION_NUMBER  ");

				//From
				sql.Append(" from "+DBSchema.UNIVERS_SCHEMA+".TOP_MODULE "+DBTables.TOP_MODULE_PREFIXE);
				sql.Append(" ,"+DBSchema.LOGIN_SCHEMA+".MODULE "+DBTables.MODULE_PREFIXE+","+DBSchema.LOGIN_SCHEMA+".MODULE_GROUP "+DBTables.MODULE_GROUP_PREFIXE);
			
				//Where
				sql.Append(" where "+DBTables.TOP_MODULE_PREFIXE+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+DBTables.TOP_MODULE_PREFIXE+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+DBTables.MODULE_PREFIXE+".id_module="+DBTables.TOP_MODULE_PREFIXE+".id_module ");
				sql.Append(" and "+DBTables.MODULE_GROUP_PREFIXE+".id_module_group="+DBTables.TOP_MODULE_PREFIXE+".id_module_group ");
			
				//Gourp by
				sql.Append(" group by  ");
				if(!toGroup)sql.Append("  "+DBTables.TOP_MODULE_PREFIXE+".ID_MODULE,"+DBTables.MODULE_PREFIXE+".MODULE,");
				sql.Append("  "+DBTables.TOP_MODULE_PREFIXE+".ID_MODULE_GROUP,"+DBTables.MODULE_GROUP_PREFIXE+".MODULE_GROUP");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc");
				#endregion
				
				#region Execution
			
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion

			}
			catch(System.Exception err){
				throw (new  AnubisBastet.Exceptions.BastetDataAccessException(" TopUsed : Impossible d'obtenir la liste des modules les plus utilisés ", err));
				
			}
			
		}
	}
}
