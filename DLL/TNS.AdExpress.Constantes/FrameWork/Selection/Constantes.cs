using System;

namespace TNS.AdExpress.Constantes.FrameWork.Selection {

	/// <summary>
	/// Evènement dans les pages de sélections
	/// </summary>
	public class eventSelection{
			
		#region Constantes
		/// <summary>
		/// Evènement OK
		/// </summary>
		public const int OK_EVENT=0;
		/// <summary>
		/// Evènement Valider
		/// </summary>
		public const int VALID_EVENT=1;
		/// <summary>
		/// Evènement suivant
		/// </summary>
		public const int NEXT_EVENT=2;
		/// <summary>
		/// Evènement chargé 
		/// </summary>
		public const int LOAD_EVENT=3;
		/// <summary>
		/// Evènement enregistrer
		/// </summary>
		public const int SAVE_EVENT=4;
		/// <summary>
		/// Evènement bouton ok de la pop up enregistrant l'univers
		/// </summary>
		public const int OK_POP_UP_EVENT=5;
		/// <summary>
		/// Evènement Initialisation
		/// </summary>
		public const int INITIALIZE_EVENT=6;
		/// <summary>
		/// Evènement Initialisation globale
		/// </summary>
		public const int ALL_INITIALIZE_EVENT=7;
		/// <summary>
		/// Evènement OKBis
		/// </summary>
		public const int OK_OPTION_MEDIA_EVENT=8;
		/// <summary>
		/// Evènement Rppel des options
		/// </summary>
		public const int RECALL_OPTIONS_EVENT = 9;

		#endregion

	}
	
	/// <summary>
	/// Erreur dans les pages de sélections
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
		/// Les supports ont déjà été sélectionner
		/// </summary>	
		public const int MEDIA_SELECTED_ALREADY=3; 
		/// <summary>
		/// Le nombre maximum d'éléments qu'on peut avoir
		/// </summary>
		public const int MAX_ELEMENTS=4;
		/// <summary>
		/// Erreur de niveaux de détails
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
