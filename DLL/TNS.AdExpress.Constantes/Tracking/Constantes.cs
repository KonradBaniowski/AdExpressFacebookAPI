#region Information
// Auteur: G. Facon
// Cr�ation: 09/11/2005 
// Modifications:
#endregion

using System;

namespace TNS.AdExpress.Constantes.Tracking{
	/// <summary>
	/// Ev�nement du tracking
	/// </summary>
	public class Event{
		/// <summary>
		/// Type d'�v�nement
		/// </summary>
		public enum Type{
			/// <summary>
			/// Nouvelle connection
			/// </summary>
			newConnection=1,
			/// <summary>
			/// Entr�e dans un module
			/// </summary>
			setModule=2,
			/// <summary>
			/// Utilisation d'un media
			/// </summary>
			setVehicle=3,
			/// <summary>
			/// Utilisation du GAD
			/// </summary>
			useGad=4,
			/// <summary>
			/// Utilisation du d�tail agence media
			/// </summary>
			setMediaAgency=5,
			/// <summary>
			/// Entr�e d'une p�riode
			/// </summary>
			setPeriodType=6,
			/// <summary>
			/// Entr�e d'une unit�
			/// </summary>
			setUnit=7,
			/// <summary>
			/// Entr�e dans un r�sultat
			/// </summary>
			setResult=8,
			/// <summary>
			/// Utilisation d'un export
			/// </summary>
			useFileExport=9,
			/// <summary>
			/// Enregistrement d'un resultat dans mon Adexpress
			/// </summary>
			useMyAdExpressSave=10
		}

	}

	/// <summary>
	/// P�riode du tracking
	/// </summary>
	public class Period{

		/// <summary>
		/// Type de p�riode
		/// </summary>
		public enum Type{
			/// <summary>
			/// P�riode journali�re
			/// </summary>
			dayly,
			/// <summary>
			/// P�riode mensuelle
			/// </summary>
			monthly
		}

	}
}
