using System;

namespace TNS.AdExpress.Constantes.FrameWork{

	/// <summary>
	/// Classe utilis�e pour indiquer la fin d'un tableau
	/// </summary>
	public class MemoryArrayEnd{
		/// <summary>
		/// Constructeur
		/// </summary>
		static MemoryArrayEnd(){
		}
	}

	/// <summary>
	/// Classe utilis�e pour indiquer q'une ligne de doit pas �tre affich�e
	/// </summary>
	public class MemoryArrayNotShowLine{
		/// <summary>
		/// Constructeur
		/// </summary>
		static MemoryArrayNotShowLine(){
		}
	}

	/// <summary>
	/// Constantes des dates li�es au p�riodicit�
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
			/// Fin du troisi�me trimestre
			/// </summary>
			THIRD=9,
			/// <summary>
			/// Fin du quatri�me trimestre
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
			/// 2�me bimestriel
			/// </summary>
			SECOND=4,
			/// <summary>
			/// 3�me bimestriel
			/// </summary>
			THIRD=6,
			/// <summary>
			/// 4�me bimestriel
			/// </summary>
			FOURST=8,
			/// <summary>
			/// 5�me bimestriel
			/// </summary>
			FIFTH=10,
			/// <summary>
			/// 6�me bimestriel
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
