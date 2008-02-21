#region Information
// Auteur: G. Facon
// Création: 09/11/2005 
// Modifications:
#endregion

using System;

namespace TNS.AdExpress.Constantes.Tracking{
	/// <summary>
	/// Evènement du tracking
	/// </summary>
	public class Event{
		/// <summary>
		/// Type d'évènement
		/// </summary>
		public enum Type{
			/// <summary>
			/// Nouvelle connection
			/// </summary>
			newConnection=1,
			/// <summary>
			/// Entrée dans un module
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
			/// Utilisation du détail agence media
			/// </summary>
			setMediaAgency=5,
			/// <summary>
			/// Entrée d'une période
			/// </summary>
			setPeriodType=6,
			/// <summary>
			/// Entrée d'une unité
			/// </summary>
			setUnit=7,
			/// <summary>
			/// Entrée dans un résultat
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
	/// Période du tracking
	/// </summary>
	public class Period{

		/// <summary>
		/// Type de période
		/// </summary>
		public enum Type{
			/// <summary>
			/// Période journalière
			/// </summary>
			dayly,
			/// <summary>
			/// Période mensuelle
			/// </summary>
			monthly
		}

	}
}
