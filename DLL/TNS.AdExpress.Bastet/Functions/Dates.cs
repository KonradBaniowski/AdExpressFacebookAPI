#region Informations
// Auteur: B. Masson
// Date de création: 05/03/2007
// Date de modification:
#endregion

using System;
using System.Text;

namespace TNS.AdExpress.Bastet.Functions{
	/// <summary>
	/// Fonction sur les dates
	/// </summary>
	public class Dates{
		
		/// <summary>
		/// Méthode pour obtenir le libellé d'un mois
		/// </summary>
		/// <param name="period">Période</param>
		/// <returns>Libellé du mois</returns>
		internal static string GetPeriodTxt(string period){
			StringBuilder txt = new StringBuilder(20);
			switch(period.Substring(4,2)){
				case "01":
					txt.Append("Janvier");
					break;
				case "02":
					txt.Append("Février");
					break;
				case "03":
					txt.Append("Mars");
					break;
				case "04":
					txt.Append("Avril");
					break;
				case "05":
					txt.Append("Mai");
					break;
				case "06":
					txt.Append("Juin");
					break;
				case "07":
					txt.Append("Juillet");
					break;
				case "08":
					txt.Append("Août");
					break;
				case "09":
					txt.Append("Septembre");
					break;
				case "10":
					txt.Append("Octobre");
					break;
				case "11":
					txt.Append("Novembre");
					break;
				case "12":
					txt.Append("Décembre");
					break;
			}
			txt.Append(" " + period.Substring(0,4));
			return txt.ToString();
		}

		/// <summary>
		/// Méthode pour obtenir la première lettre d'un jour de semaine
		/// </summary>
		/// <param name="dayOfWeek">Jour</param>
		/// <returns>Première lettre d'un jour de semaine</returns>
		internal static string GetDayOfWeek(string dayOfWeek){
			string txt="";
			switch(dayOfWeek){
				case "Monday":
					txt="L";
					break;
				case "Tuesday":
					txt="M";
					break;
				case "Wednesday":
					txt="M";
					break;
				case "Thursday":
					txt="J";
					break;
				case "Friday":
					txt="V";
					break;
				case "Saturday":
					txt="S";
					break;
				case "Sunday":
					txt="D";
					break;
			}
			return txt;
		}


	}
}
