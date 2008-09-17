#region Informations
// Auteur: B. Masson
// Date de cr�ation: 16/11/2005
// Date de modification:
#endregion

using System;
using System.Collections;
using System.Data;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Bastet.Exceptions;
using TNS.AdExpress.Bastet.DataAccess;

namespace TNS.AdExpress.Bastet.BusinessFacade{
	/// <summary>
	/// Classe d'entr�e de la recherche des logins
	/// </summary>
	public class LoginBusinessFacade{
		
		/// <summary>
		/// M�thode pour la recherche des logins
		/// </summary>
		/// <param name="source">Source de doon�es</param>
		/// <param name="label">Chaine � rechercher</param>
		/// <returns>Liste des logins pour chaque soci�t�</returns>
		public static DataSet GetLogins(IDataSource source, string label, bool byKeyWord){
			return(LoginDataAccess.GetSearchLogin(source, label, byKeyWord));
		}


		/// <summary>
		/// M�thode pour la recherche des logins par type client
		/// </summary>
		/// <param name="source">Source de doon�es</param>
		/// <param name="customerTypeId">Identifiants des types de client</param>
		/// <returns>Liste des logins par type client</returns>
		public static DataSet GetLoginByCustomerType(IDataSource source, string customerTypeId){
			return(LoginDataAccess.GetSearchLoginByCustomerType(source, customerTypeId));
		}

	}
}
