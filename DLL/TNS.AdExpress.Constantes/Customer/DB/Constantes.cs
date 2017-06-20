using System;

namespace TNS.AdExpress.Constantes.Customer.DB {
	/// <summary>
	/// Description r�sum�e de Constantes.
	/// </summary>
	public class MediaProductIdType {
			
		#region Constantes
	
		/// <summary>
		/// Identifiant Sector
		/// </summary>
		public const Int64 ID_SECTOR_TYPE=61;
		/// <summary>
		/// Identifiant SubSector
		/// </summary>
		public const Int64 ID_SUBSECTOR_TYPE=62;
		/// <summary>
		/// Identifiant Group
		/// </summary>
		public const Int64 ID_GROUP_TYPE=63;
		/// <summary>
		/// Identifiant Segment
		/// </summary>
		public const Int64 ID_SEGMENT_TYPE=64;
		/// <summary>
		/// Identifiant Holding company
		/// </summary>
		public const Int64 ID_HOLDING_COMPANY_TYPE=67;
		/// <summary>
		/// Identifiant Advertiser
		/// </summary>
		public const Int64 ID_ADVERTISER_TYPE=65;
		/// <summary>
		/// Identifiant Vehicle
		/// </summary>
		public const Int64 ID_VEHICLE_TYPE=1;
		/// <summary>
		/// Identifiant Category
		/// </summary>
		public const Int64 ID_CATEGORY_TYPE=2;
		/// <summary>
		/// Identifiant Media
		/// </summary>
		public const Int64 ID_MEDIA_TYPE=3;
        /// <summary>
        /// Identifiant Region
        /// </summary>
        public const Int64 ID_REGION_TYPE = 10;
        /// <summary>
        /// Identifiant Brand
        /// </summary>
        public const Int64 ID_BRAND_TYPE = 66;
        /// <summary>
        /// Identifiant VP Circuit
        /// </summary>
        public const Int64 ID_CIRCUIT_TYPE = 14;
        /// <summary>
        /// Identifiant VP Brand
        /// </summary>
        public const Int64 ID_VP_BRAND_TYPE = 205;

        /// <summary>
        /// Identifiant VP Segment
        /// </summary>
        public const Int64 ID_VP_SEGMENT_TYPE = 204;
        /// <summary>
        /// Identifiant VP Sub Segment
        /// </summary>
        public const Int64 ID_VP_SUB_SEGMENT_TYPE = 203;
        /// <summary>
        /// Identifiant VP Product
        /// </summary>
        public const Int64 ID_VP_PRODUCT_TYPE = 202;

        /// <summary>
        /// Identifiant Veille Promo Vehicle
        /// </summary>
        public const Int64 ID_VP_VEHICLE_TYPE = 14;
        /// <summary>
        /// Identifiant Brand
        /// </summary>
        public const Int64 ID_PRODUCT_TYPE = 81;
        #endregion
    }

	/// <summary>
	/// Fr�quence
	/// </summary>
	public class Frequency{
			
		#region Constantes
		/// <summary>
		/// Fr�quence quotidienne
		/// </summary>
		public const Int64 DAILY=1;
		/// <summary>
		/// Fr�quence par d�fault
		/// </summary>
		public const Int64 DEFAULT=1;
		/// <summary>
		/// Fr�quence hebdomadaire
		/// </summary>
		public const Int64 WEEKLY =2;
		/// <summary>
		/// Fr�quence Bimensuel
		/// </summary>
		public const Int64 SEMI_MONTHLY=3;
		/// <summary>
		/// Mensuelle
		/// </summary>
		public const Int64 MONTHLY=4;
		/// <summary>
		/// Fr�quence bimestrielle
		/// </summary>
		public const Int64 TWO_MONTHLY=5;
		/// <summary>
		/// Fr�quence trimestriel  
		/// </summary>
		public const Int64 QUATERLY=6;
		/// <summary>
		///  Fr�quence semestrielle
		/// </summary>
		public const Int64 SEMI_ANNUAL=7;
		/// <summary>
		///  Fr�quence annuelle
		/// </summary>
		public const Int64 ANNUAL=8;

		#endregion
	
	}

	/// <summary>
	/// Flag AdExpress
	/// </summary>
	public class Flag{
		/// <summary>
		/// Identifiant des flags
		/// </summary>
		public enum id{
            /// <summary>
            /// Flag du gad
            /// </summary>
            leFac = 373,
            /// <summary>
            /// Flag du gad
            /// </summary>
            gad =141,
			/// <summary>
			/// Visualiser les cr�ations
			/// </summary>
			showCreation=200,
			/// <summary>
			/// T�lechatger les Cr�ations
			/// </summary>
			downloadCreation=201		
		}	
	}
}
