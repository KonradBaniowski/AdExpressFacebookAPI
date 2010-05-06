using System;
using TNS.AdExpress.Anubis.Bastet.Exceptions;
using TNS.AdExpress.Bastet.Translation;


namespace TNS.AdExpress.Anubis.Bastet.Functions
{
	/// <summary>
	/// Description résumée de Period.
	/// </summary>
	public class Period
	{	
		/// <summary>
		/// Obtient un jour nommé en fonction de son identifiant.
		/// </summary>
		/// <param name="dayNumber">nombre représenatant le jour</param>
		/// <param name="language">langue</param>
		/// <returns>jour nommé</returns>
		public static string GetNamedDay(int dayNumber,int language) {
			
			string day="";
			
				switch(dayNumber){
					case 1:
						day = GestionWeb.GetWebWord(1554, language);// "Lundi";
						break;
					case 2:
						day = GestionWeb.GetWebWord(1555, language);// "Mardi";
						break;
					case 3:
						day = GestionWeb.GetWebWord(1556, language);// "Mercredi";
						break;
					case 4:
						day = GestionWeb.GetWebWord(1557, language);// "Jeudi";
						break;
					case 5:
						day = GestionWeb.GetWebWord(1558, language);// "Vendredi";
						break;
					case 6:
						day = GestionWeb.GetWebWord(1559, language);// "Samedi";
						break;
					case 7:
						day = GestionWeb.GetWebWord(1560, language);// "Dimanche";
						break;
					default:
						throw (new PeriodException("Day Id unknown. "));		
				}

			
			return day;
		}
	}
}
