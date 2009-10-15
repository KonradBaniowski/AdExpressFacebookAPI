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
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Bastet.Web;

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
            Table rightCompany = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightCompany);
            Table rightLogin = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
            Table rightContact = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContact);
            Table rightAddress = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightAddress);

            sql.Append("select " + rightCompany.Prefix + ".id_company, " + rightCompany.Prefix + ".company, ");
			sql.Append(rightLogin.Prefix +".id_login, "+ rightLogin.Prefix +".login, ");
			sql.Append(rightLogin.Prefix +".activation ");
            sql.Append(" from " + rightCompany.SqlWithPrefix + ", ");
            sql.Append(rightLogin.SqlWithPrefix + ", ");
            sql.Append(rightContact.SqlWithPrefix + ", ");
            sql.Append(rightAddress.SqlWithPrefix + " ");
			sql.Append(" where "+ rightLogin.Prefix +".id_contact = "+ rightContact.Prefix +".id_contact ");
			sql.Append(" and "+ rightContact.Prefix +".id_address = "+ rightAddress.Prefix +".id_address ");
			sql.Append(" and "+ rightAddress.Prefix +".id_company = "+ rightCompany.Prefix +".id_company ");	
			if(byKeyWord){
				// Recherche par mot clef
				sql.Append(" and (");
				sql.Append(" UPPER("+ rightLogin.Prefix +".login) like '%"+label.ToUpper()+"%' ");
				sql.Append(" or ");
				sql.Append(" UPPER("+ rightCompany.Prefix +".company) like '%"+label.ToUpper()+"%' ");
				sql.Append(")");
			}
			else{
				// Recherche par identifiant ou liste d'identifiants
				sql.Append(" and "+ rightLogin.Prefix +".id_login in ("+label+") ");
			}
			sql.Append(" order by "+ rightCompany.Prefix +".company, "+ rightLogin.Prefix +".login");

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
            Table rightLogin = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
            Table rightContact = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContact);

			sql.Append("select "+ rightLogin.Prefix +".id_login, "+ rightLogin.Prefix +".login ");
            sql.Append(" from " + rightLogin.SqlWithPrefix + ", ");
            sql.Append(rightContact.SqlWithPrefix + " ");
			sql.Append(" where "+ rightLogin.Prefix +".id_contact = "+ rightContact.Prefix +".id_contact ");
			sql.Append(" and "+ rightContact.Prefix +".id_group_contact in ("+customerTypeId+") ");
			sql.Append(" order by "+ rightLogin.Prefix +".login");
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
