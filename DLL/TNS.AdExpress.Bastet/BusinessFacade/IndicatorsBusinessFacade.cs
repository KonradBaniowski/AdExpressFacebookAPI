#region Informations
// Auteur: B. Masson
// Date de cr�ation: 23/02/2007
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
	/// Classe d'entr�e du r�sultat des indicateurs
	/// </summary>
	public class IndicatorsBusinessFacade{

		/// <summary>
		/// M�thode pour l'obtention du r�sultat HTML
		/// </summary>
		/// <param name="source">Source de donn�es</param>
		/// <param name="vehicleList">Liste des m�dias</param>
		/// <param name="dateBegin">Date de d�but</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <returns>HTML</returns>
        public static string GetIndicators(IDataSource source, string vehicleList, DateTime dateBegin, DateTime dateEnd, int siteLanguageId, int dataLanguageId) {
			return(IndicatorsUI.GetHtml(source,vehicleList,dateBegin,dateEnd,siteLanguageId, dataLanguageId));			
		}

		/// <summary>
		/// M�thode pour l'obtention du r�sultat Excel
		/// </summary>
		/// <param name="source">Source de donn�es</param>
		/// <param name="vehicleList">Liste des m�dias</param>
		/// <param name="dateBegin">Date de d�but</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <returns>HTML</returns>
        public static string GetIndicatorsExcel(IDataSource source, string vehicleList, DateTime dateBegin, DateTime dateEnd, int siteLanguageId, int dataLanguageId) {
			return(IndicatorsUI.GetExcel(source,vehicleList,dateBegin,dateEnd,siteLanguageId,  dataLanguageId));			
		}

	}
}
