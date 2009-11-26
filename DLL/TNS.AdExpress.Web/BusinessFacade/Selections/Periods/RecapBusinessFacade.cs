#region Informations
// Auteur: G. Facon 
// Date de création: 04/01/05
// Date de modification: 
#endregion

using System;
using WebDA=TNS.AdExpress.Web.DataAccess;

namespace TNS.AdExpress.Web.BusinessFacade.Selections.Periods{
	/// <summary>
	/// Traite les périodes des récaps
	/// </summary>
	public class RecapBusinessFacade{

		/// <summary>
		/// Obtient la dernière année chargée dans la base de données pour les recap.
		/// </summary>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.RecapDataAccessException">Erreur lors de l'ouverture, fermeture de la base données ou de l'execution de la requête</exception>
		/// <returns>Dernière année chargée</returns>
		public static int GetLastLoadedYear(){
			try{
				return(WebDA.Selections.Periods.RecapDataAccess.GetLastLoadedYear());
			}
			catch(System.Exception ex){
				throw(ex);
			}
		}

	}
}
