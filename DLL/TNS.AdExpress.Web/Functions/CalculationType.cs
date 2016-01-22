#region Informations
// Auteur: D. V. Mussuma 
// Date de création: 13/10/2004 
// Date de modification: 13/10/2004 
// G. Facon		11/08/2005 New Exception Management and property name 
#endregion

using System;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Exceptions;
using WebConstantes=TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Functions{
	/// <summary>
	/// Identifie le type de calcul total à éffectuer :
	/// univers ou famille ou marché
	/// </summary>
	public class CalculationType{
		
		/// <summary>
		/// Vérifie que c'est un calcul sur le total marché
		/// </summary>
		/// <param name="criterion">critère calcul</param>
		/// <returns>vrai si calcul concerne le total marché</returns>
		public static bool IsMarketTotalCriterion(WebConstantes.CustomerSessions.ComparisonCriterion criterion){
			switch(criterion){
				case WebConstantes.CustomerSessions.ComparisonCriterion.marketTotal :
					return true;
				case WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal :
				case WebConstantes.CustomerSessions.ComparisonCriterion.universTotal :
					return false;
				default :					
					throw new CalculationTypeException("Impossible de déterminer le type de calcul total sélectionné.");;
			}
		}

		/// <summary>
		/// Vérifie que c'est un calcul sur le total famille
		/// </summary>
		/// <param name="criterion">critère calcul</param>
		/// <returns>vrai si calcul concerne le total famille</returns>
		public static bool IsSectorTotalCriterion(WebConstantes.CustomerSessions.ComparisonCriterion criterion){
			switch(criterion){
				case WebConstantes.CustomerSessions.ComparisonCriterion.marketTotal :	
				case WebConstantes.CustomerSessions.ComparisonCriterion.universTotal :
					return false;
				case WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal :
					return true;
				
				default :					
					throw new CalculationTypeException("Impossible de déterminer le type de calcul total sélectionné.");;
			}
		}

		/// <summary>
		/// Vérifie que c'est un calcul sur le total univers
		/// </summary>
		/// <param name="criterion">critère calcul</param>
		/// <returns>vrai si calcul concerne le total univers</returns>
		public static bool IsUniverseTotalCriterion(WebConstantes.CustomerSessions.ComparisonCriterion criterion){
			switch(criterion){
				case WebConstantes.CustomerSessions.ComparisonCriterion.marketTotal :					
				case WebConstantes.CustomerSessions.ComparisonCriterion.sectorTotal :
					return false;
				case WebConstantes.CustomerSessions.ComparisonCriterion.universTotal :
					return true;
				default :					
					throw new CalculationTypeException("Impossible de déterminer le type de calcul total sélectionné.");;
			}
		}
	}

}
