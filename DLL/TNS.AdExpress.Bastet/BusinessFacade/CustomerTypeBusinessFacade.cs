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
using TNS.AdExpress.Bastet.DataAccess;

namespace TNS.AdExpress.Bastet.BusinessFacade{
	/// <summary>
	/// Classe d'entrée des types de client
	/// </summary>
	public class CustomerTypeBusinessFacade{
		
		/// <summary>
		/// Méthode pour l'obtention des types de client
		/// </summary>
		/// <param name="source">Source de doonées</param>
		/// <returns>Liste des types de client</returns>
		public static DataSet GetCustomerType(IDataSource source){
			return(CustomerTypeDataAccess.GetCustomerType(source));
		}

	}
}
