using System;
using TNS.AdExpress.Anubis.Bastet.Exceptions;
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
			switch(language){
				case 33:
				switch(dayNumber){
					case 1:
						day="Lundi";
						break;
					case 2:
						day="Mardi";
						break;
					case 3:
						day="Mercredi";
						break;
					case 4:
						day="Jeudi";
						break;
					case 5:
						day="Vendredi";
						break;
					case 6:
						day="Samedi";
						break;
					case 7:
						day="Dimanche";
						break;										
				}
					break;
				case 44:
				switch(dayNumber){
					case 1:
						day="Monday";
						break;
					case 2:
						day="Tuesday";
						break;
					case 3:
						day="Wednesday";
						break;
					case 4:
						day="Thursday";
						break;
					case 5:
						day="Friday";
						break;
					case 6:
						day="Saturday";
						break;
					case 7:
						day="Sunday";
						break;										
				}
					break;
				default:
					throw(new PeriodException("La langue sélectionnée n'est pas correcte: "));
			}

			return day;
		}
	}
}
