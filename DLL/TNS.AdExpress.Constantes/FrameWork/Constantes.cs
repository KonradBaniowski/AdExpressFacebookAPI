using System;

namespace TNS.AdExpress.Constantes.FrameWork{

	/// <summary>
	/// Classe utilisée pour indiquer la fin d'un tableau
	/// </summary>
	public class MemoryArrayEnd{
		/// <summary>
		/// Constructeur
		/// </summary>
		static MemoryArrayEnd(){
		}
	}

	/// <summary>
	/// Classe utilisée pour indiquer q'une ligne de doit pas être affichée
	/// </summary>
	public class MemoryArrayNotShowLine{
		/// <summary>
		/// Constructeur
		/// </summary>
		static MemoryArrayNotShowLine(){
		}
	}

	/// <summary>
	/// Constantes des dates liées au périodicité
	/// </summary>
	public class Dates{

		/// <summary>
		/// Dates du dernier mois des Trimestres
		/// </summary>
		public enum QUARTER{
			/// <summary>
			/// Fin du premier trimestre
			/// </summary>
			FIRST=3,
			/// <summary>
			/// Fin du second trimestre
			/// </summary>
			SECOND=6,
			/// <summary>
			/// Fin du troisième trimestre
			/// </summary>
			THIRD=9,
			/// <summary>
			/// Fin du quatrième trimestre
			/// </summary>
			FOURTH=12,
		}

		/// <summary>
		/// Dates du dernier mois des Semestres
		/// </summary>
		public enum SEMESTER{
			/// <summary>
			/// Fin du premier semestre
			/// </summary>
			FIRST=6,
			/// <summary>
			/// Fin du second semestre
			/// </summary>
			SECOND=12,
		}

		/// <summary>
		/// Dates du dernier mois des bimestriels
		/// </summary>
		public enum TWICE_MONTH{
			/// <summary>
			/// 1ier bimestriel
			/// </summary>
			FIRST=2,
			/// <summary>
			/// 2ème bimestriel
			/// </summary>
			SECOND=4,
			/// <summary>
			/// 3ème bimestriel
			/// </summary>
			THIRD=6,
			/// <summary>
			/// 4ème bimestriel
			/// </summary>
			FOURST=8,
			/// <summary>
			/// 5ème bimestriel
			/// </summary>
			FIFTH=10,
			/// <summary>
			/// 6ème bimestriel
			/// </summary>
			SIXTH=12,
		}
		/// <summary>
		/// Dates du dernier mois des bimestriels
		/// </summary>
		public enum Pattern {
			/// <summary>
			/// short date pattern
			/// </summary>
			shortDatePattern			
		}

	}
}
