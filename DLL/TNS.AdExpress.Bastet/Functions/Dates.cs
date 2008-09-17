#region Informations
// Auteur: B. Masson
// Date de cr�ation: 05/03/2007
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
		/// M�thode pour obtenir le libell� d'un mois
		/// </summary>
		/// <param name="period">P�riode</param>
		/// <returns>Libell� du mois</returns>
		internal static string GetPeriodTxt(string period){
			StringBuilder txt = new StringBuilder(20);
			switch(period.Substring(4,2)){
				case "01":
					txt.Append("Janvier");
					break;
				case "02":
					txt.Append("F�vrier");
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
					txt.Append("Ao�t");
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
					txt.Append("D�cembre");
					break;
			}
			txt.Append(" " + period.Substring(0,4));
			return txt.ToString();
		}

		/// <summary>
		/// M�thode pour obtenir la premi�re lettre d'un jour de semaine
		/// </summary>
		/// <param name="dayOfWeek">Jour</param>
		/// <returns>Premi�re lettre d'un jour de semaine</returns>
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
