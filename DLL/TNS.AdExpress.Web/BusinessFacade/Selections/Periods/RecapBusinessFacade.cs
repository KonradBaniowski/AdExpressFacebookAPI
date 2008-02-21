#region Informations
// Auteur: G. Facon 
// Date de cr�ation: 04/01/05
// Date de modification: 
#endregion

using System;
using WebDA=TNS.AdExpress.Web.DataAccess;

namespace TNS.AdExpress.Web.BusinessFacade.Selections.Periods{
	/// <summary>
	/// Traite les p�riodes des r�caps
	/// </summary>
	public class RecapBusinessFacade{

		/// <summary>
		/// Obtient la derni�re ann�e charg�e dans la base de donn�es pour les recap.
		/// </summary>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.RecapDataAccessException">Erreur lors de l'ouverture, fermeture de la base donn�es ou de l'execution de la requ�te</exception>
		/// <returns>Derni�re ann�e charg�e</returns>
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
