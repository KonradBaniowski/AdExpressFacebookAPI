using System;

namespace TNS.AdExpress.Constantes.FrameWork.Selection {

	/// <summary>
	/// Ev�nement dans les pages de s�lections
	/// </summary>
	public class eventSelection{
			
		#region Constantes
		/// <summary>
		/// Ev�nement OK
		/// </summary>
		public const int OK_EVENT=0;
		/// <summary>
		/// Ev�nement Valider
		/// </summary>
		public const int VALID_EVENT=1;
		/// <summary>
		/// Ev�nement suivant
		/// </summary>
		public const int NEXT_EVENT=2;
		/// <summary>
		/// Ev�nement charg� 
		/// </summary>
		public const int LOAD_EVENT=3;
		/// <summary>
		/// Ev�nement enregistrer
		/// </summary>
		public const int SAVE_EVENT=4;
		/// <summary>
		/// Ev�nement bouton ok de la pop up enregistrant l'univers
		/// </summary>
		public const int OK_POP_UP_EVENT=5;
		/// <summary>
		/// Ev�nement Initialisation
		/// </summary>
		public const int INITIALIZE_EVENT=6;
		/// <summary>
		/// Ev�nement Initialisation globale
		/// </summary>
		public const int ALL_INITIALIZE_EVENT=7;
		/// <summary>
		/// Ev�nement OKBis
		/// </summary>
		public const int OK_OPTION_MEDIA_EVENT=8;
		/// <summary>
		/// Ev�nement Rppel des options
		/// </summary>
		public const int RECALL_OPTIONS_EVENT = 9;

		#endregion

	}
	
	/// <summary>
	/// Erreur dans les pages de s�lections
	/// </summary>
	public class error{
		
		#region Constantes
		/// <summary>
		/// Impossible de charger l'univers
		/// </summary>
		public const int LOAD_NOT_POSSIBLE=1;
		/// <summary>
		/// Aucune checkBox de cocher
		/// </summary>	
		public const int CHECKBOX_NULL=2;
		/// <summary>
		/// Les supports ont d�j� �t� s�lectionner
		/// </summary>	
		public const int MEDIA_SELECTED_ALREADY=3; 
		/// <summary>
		/// Le nombre maximum d'�l�ments qu'on peut avoir
		/// </summary>
		public const int MAX_ELEMENTS=4;
		/// <summary>
		/// Erreur de niveaux de d�tails
		/// </summary>
		public const int SECURITY_EXCEPTION = 5;
		/// <summary>
		/// Impossible de valider la selection
		/// </summary>
		public const int VALIDATION_NOT_POSSIBLE = 6;
		#endregion
	}	

	#region APPM
	/// <summary>
	/// Vagues APPM
	/// </summary>
	public class Wave{
		/// <summary>
		/// Type de vague
		/// </summary>
		public enum Type{
			/// <summary>
			/// vague AEPM
			/// </summary>
			AEPM,
			/// <summary>
			/// vague OJD
			/// </summary>
			OJD
		}
	}
	
	#endregion
}
