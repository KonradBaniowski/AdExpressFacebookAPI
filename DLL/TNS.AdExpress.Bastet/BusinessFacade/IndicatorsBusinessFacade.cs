#region Informations
// Auteur: B. Masson
// Date de création: 23/02/2007
// Date de modification:
#endregion

using System;
using System.Collections;
using System.Data;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Bastet.Exceptions;
using TNS.AdExpress.Bastet.UI;

namespace TNS.AdExpress.Bastet.BusinessFacade{
	/// <summary>
	/// Classe d'entrée du résultat des indicateurs
	/// </summary>
	public class IndicatorsBusinessFacade{

		/// <summary>
		/// Méthode pour l'obtention du résultat HTML
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="vehicleList">Liste des médias</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <returns>HTML</returns>
        public static string GetIndicators(IDataSource source, string vehicleList, DateTime dateBegin, DateTime dateEnd, int siteLanguageId, int dataLanguageId) {
			return(IndicatorsUI.GetHtml(source,vehicleList,dateBegin,dateEnd,siteLanguageId, dataLanguageId));			
		}

		/// <summary>
		/// Méthode pour l'obtention du résultat Excel
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="vehicleList">Liste des médias</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <returns>HTML</returns>
        public static string GetIndicatorsExcel(IDataSource source, string vehicleList, DateTime dateBegin, DateTime dateEnd, int siteLanguageId, int dataLanguageId) {
			return(IndicatorsUI.GetExcel(source,vehicleList,dateBegin,dateEnd,siteLanguageId,  dataLanguageId));			
		}

	}
}
