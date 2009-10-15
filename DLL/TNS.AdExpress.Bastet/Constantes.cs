#region Informations
// Auteur: B. Masson
// Date de création: 05/03/2007
// Date de modification:
#endregion

using System;

namespace TNS.AdExpress.Bastet{
	/// <summary>
	/// Constantes
	/// </summary>
	public class Constantes{

		/// <summary>
		/// Index de la colonne des médias
		/// </summary>
		public const int VEHICLE_COLUMN_INDEX = 0;
		/// <summary>
		/// Index de la colonne des cCatégories
		/// </summary>
		public const int CATEGORY_COLUMN_INDEX = 1;
		/// <summary>
		/// Index de la colonne des identifiants de diffusion
		/// </summary>
		public const int ID_DIFFUSION_COLUMN_INDEX = 2;
		/// <summary>
		/// Index de la colonne des supports
		/// </summary>
		public const int MEDIA_COLUMN_INDEX = 3;
		/// <summary>
		/// Index de la colonne des identifiants des médias
		/// </summary>
		public const int ID_VEHICLE_COLUMN_INDEX = 4;
		/// <summary>
		/// Index de la colonne des identifiants des catégories
		/// </summary>
		public const int ID_CATEGORY_COUMN_INDEX = 5;
		/// <summary>
		/// Index de la colonne des identifiants des supports
		/// </summary>
		public const int ID_MEDIA_COLUMN_INDEX = 6;
		/// <summary>
		/// Index de la colonne de la 1ère date
		/// </summary>
		public const int FIRST_PEDIOD_COlUMN_INDEX = 7;

	}

    public class WebSession {

        /// <summary>
        /// Language
        /// </summary>
        public const string LANGUAGE = "Language";
        /// <summary>
        /// Language
        /// </summary>
        public const string LOGIN = "Login";
        /// <summary>
        /// Language
        /// </summary>
        public const string MAILS = "Mails";
        /// <summary>
        /// Language
        /// </summary>
        public const string DATE_BEGIN = "DateBegin";
        /// <summary>
        /// Language
        /// </summary>
        public const string DATE_END = "DateEnd";
        /// <summary>
        /// Vehicle List
        /// </summary>
        public const string VEHICLE_LIST = "vehicleList";
    }

    #region Cookies
    /// <summary>
    /// List every cookie used in the web site
    /// </summary>
    public class Cookies {
        /// <summary>
        /// Cooky dedicated to the language management
        /// </summary>
        public static string LANGUAGE = "language";
    }
    #endregion
}
