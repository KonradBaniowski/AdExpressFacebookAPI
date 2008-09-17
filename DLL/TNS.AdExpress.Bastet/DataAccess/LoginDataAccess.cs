#region Informations
// Auteur: B. Masson
// Date de création: 16/11/2005
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

namespace TNS.AdExpress.Bastet.DataAccess{
	/// <summary>
	/// Classe d'accès aux données pour la recherche de logins
	/// </summary>
	public class LoginDataAccess{

		/// <summary>
		/// Obtient la liste des logins par société
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="Label">Chaine à rechercher</param>
		/// <returns>Données de la liste de logins par société</returns>
		internal static DataSet GetSearchLogin(IDataSource source, string label, bool byKeyWord){

			#region Requête
			StringBuilder sql = new StringBuilder();

			sql.Append("select "+ DbTables.COMPANY_PREFIXE +".id_company, "+ DbTables.COMPANY_PREFIXE +".company, ");
			sql.Append(DbTables.LOGIN_PREFIXE +".id_login, "+ DbTables.LOGIN_PREFIXE +".login, ");
			sql.Append(DbTables.LOGIN_PREFIXE +".activation ");
			sql.Append(" from "+ DbSchema.RIGHT_SCHEMA +"."+ DbTables.COMPANY_TABLE +" "+ DbTables.COMPANY_PREFIXE +", ");
			sql.Append(DbSchema.RIGHT_SCHEMA +"."+ DbTables.LOGIN_TABLE +" "+ DbTables.LOGIN_PREFIXE +", ");
			sql.Append(DbSchema.RIGHT_SCHEMA +"."+ DbTables.CONTACT_TABLE +" "+ DbTables.CONTACT_PREFIXE +", ");
			sql.Append(DbSchema.RIGHT_SCHEMA +"."+ DbTables.ADDRESS_TABLE +" "+ DbTables.ADDRESS_PREFIXE +" ");
			sql.Append(" where "+ DbTables.LOGIN_PREFIXE +".id_contact = "+ DbTables.CONTACT_PREFIXE +".id_contact ");
			sql.Append(" and "+ DbTables.CONTACT_PREFIXE +".id_address = "+ DbTables.ADDRESS_PREFIXE +".id_address ");
			sql.Append(" and "+ DbTables.ADDRESS_PREFIXE +".id_company = "+ DbTables.COMPANY_PREFIXE +".id_company ");	
			if(byKeyWord){
				// Recherche par mot clef
				sql.Append(" and (");
				sql.Append(" UPPER("+ DbTables.LOGIN_PREFIXE +".login) like '%"+label.ToUpper()+"%' ");
				sql.Append(" or ");
				sql.Append(" UPPER("+ DbTables.COMPANY_PREFIXE +".company) like '%"+label.ToUpper()+"%' ");
				sql.Append(")");
			}
			else{
				// Recherche par identifiant ou liste d'identifiants
				sql.Append(" and "+ DbTables.LOGIN_PREFIXE +".id_login in ("+label+") ");
			}
			sql.Append(" order by "+ DbTables.COMPANY_PREFIXE +".company, "+ DbTables.LOGIN_PREFIXE +".login");

			#endregion

			#region Execution
			try{
				return(source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw (new LoginDataAccessException("GetSearchLogin : Impossible d'obtenir la liste des logins ayant pour chaine "+label, err));
			}
			#endregion

		}

		/// <summary>
		/// Obtient la liste des logins par type de client
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="customerTypeId">Identifiants des types de client</param>
		/// <returns>Données de la liste de logins par type de client</returns>
		internal static DataSet GetSearchLoginByCustomerType(IDataSource source, string customerTypeId){

			#region Requête
			StringBuilder sql = new StringBuilder();

			sql.Append("select "+ DbTables.LOGIN_PREFIXE +".id_login, "+ DbTables.LOGIN_PREFIXE +".login ");
			sql.Append(" from "+ DbSchema.RIGHT_SCHEMA +"."+ DbTables.LOGIN_TABLE +" "+ DbTables.LOGIN_PREFIXE +", ");
			sql.Append(DbSchema.RIGHT_SCHEMA +"."+ DbTables.CONTACT_TABLE +" "+ DbTables.CONTACT_PREFIXE +" ");
			sql.Append(" where "+ DbTables.LOGIN_PREFIXE +".id_contact = "+ DbTables.CONTACT_PREFIXE +".id_contact ");
			sql.Append(" and "+ DbTables.CONTACT_PREFIXE +".id_group_contact in ("+customerTypeId+") ");
			sql.Append(" order by "+ DbTables.LOGIN_PREFIXE +".login");
			#endregion

			#region Execution
			try{
				return(source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw (new LoginDataAccessException("GetSearchLogin : Impossible d'obtenir la liste des logins par type de client : "+customerTypeId, err));
			}
			#endregion

		}
		
	}
}
