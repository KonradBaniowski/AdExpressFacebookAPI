#region Informations
// Auteur: B. Masson
// Date de création: 09/03/2004
// Date de modification: 16/03/2004
#endregion

using System;
using System.Collections;

namespace TNS.AdExpress.Constantes.Classification.DB{
	
	#region Class Table
	/// <summary>
	/// Cette classe définit les constantes liées aux tables de la base de données
	/// </summary>
	public class Table{
		/// <summary>
		/// Nom des tables
		/// </summary>
		public enum name{
			/// <summary>
			/// Table Sector
			/// </summary>
			sector,
			/// <summary>
			/// Table Subsector
			/// </summary>
			subsector,
			/// <summary>
			/// Table Group_
			/// </summary>
			group_,
			/// <summary>
			/// Table Segment
			/// </summary>
			segment,
			/// <summary>
			/// Table Produit
			/// </summary>
			product,
			/// <summary>
			/// Table Advertiser
			/// </summary>
			advertiser,
			/// <summary>
			/// Table Holding_company
			/// </summary>
			holding_company,
			/// <summary>
			/// Table Vehicle
			/// </summary>
			vehicle,
			/// <summary>
			/// Table Category
			/// </summary>
			category,
			/// <summary>
			/// Table Media
			/// </summary>
			media,
			/// <summary>
			/// Titre
			/// </summary>
			title,
			/// <summary>
			/// Marque
			/// </summary>
			brand,
			/// <summary>
			/// Régie
			/// </summary>
			media_seller,
			/// <summary>
			/// centre d'intérêt
			/// </summary>
			interest_center,
             /// <summary>
            /// Basic Media
            /// </summary>
            basic_media,
            /// <summary>
            /// Region
            /// </summary>
            region

		}
	}
    #endregion

    #region Vehicles
    /// <summary>
    /// 
    /// </summary>
   
    /// <summary>
    /// Media type List
    /// </summary>
    public class Vehicles{
		/// <summary>
        /// Media type
		/// </summary>
		public enum names{
			/// <summary>
			/// Press
			/// </summary>
			press=1,
			/// <summary>
			/// Radio
			/// </summary>
			radio=2,
			/// <summary>
			/// Television
			/// </summary>
			tv=3,
			/// <summary>
			/// Media tactique
			/// </summary>
			mediasTactics=4,			
			/// <summary>
			/// Others media types
			/// </summary>
			others=5,	
			/// <summary>
			/// AdNetTrack
			/// </summary>
			adnettrack=6,			
			/// <summary>
			/// Internet
			/// </summary>
			internet=7,
			/// <summary>
			/// Outdoor
			/// </summary>
			outdoor=8,
			/// <summary>
			/// Cinéma
			/// </summary>
			cinema=9,
			/// <summary>
			/// Marketing Direct
			/// </summary>
			directMarketing = 10,
			/// <summary>
			/// Presse internationale
			/// </summary>
			internationalPress=11,
			/// <summary>
			/// Plurimédia
			/// </summary>
			plurimedia=50,
			/// <summary>
			/// Mobile telephony
			/// </summary>
			mobileTelephony = 12,
			/// <summary>
			/// Emailing
			/// </summary>
			emailing = 13,
			/// <summary>
			/// Evaliant mobile
			/// </summary>
			evaliantMobile = 15,
            /// <summary>
            /// Evaliant mobile
            /// </summary>
            newspaper = 16,
            /// <summary>
            /// Evaliant mobile
            /// </summary>
            magazine = 17,
            /// <summary>
            /// Radio General
			/// </summary>
			radioGeneral=18,
            /// <summary>
            /// Radio Sponsorship
			/// </summary>
			radioSponsorship=19,
            /// <summary>
			/// Radio Music
			/// </summary>
			radioMusic=20,
            /// <summary>
            /// Television General
            /// </summary>
            tvGeneral = 21,
            /// <summary>
            /// Television SponsorShip
            /// </summary>
            tvSponsorship = 22,
            /// <summary>
            /// Television Announces
            /// </summary>
            tvAnnounces = 23,
            /// <summary>
            /// Television non Terrestrials
			/// </summary>
			tvNonTerrestrials=24,
            /// <summary>
            /// inDoor
            /// </summary>
            indoor = 25,
             /// <summary>
            /// Television Niche Channels
			/// </summary>
			tvNicheChannels=26,
            /// <summary>
            /// IN Store 
            /// </summary>
            instore = 27,
            /// <summary>
            /// Editorial
			/// </summary>
			editorial=28,
            /// <summary>
            /// CZ Internet Media
            /// </summary>
            czinternet=29,
            /// <summary>
            /// TV Clipping
            /// </summary>
            tvClipping =30,
            /// <summary>
            /// Press Clipping
            /// </summary>
            pressClipping = 31,
            /// <summary>
            /// Mail valorized with France Pub data
            /// </summary>
            mailValo = 32,
            /// <summary>
            /// MMS
            /// </summary>
            mms = 33,
            /// <summary>
            /// Search
            /// </summary>
            search = 34,
            /// <summary>
            /// Search
            /// </summary>
            social = 35,
            /// <summary>
            /// Plurimédia without Mms
            /// </summary>
            PlurimediaWithoutMms = 51,           
            /// <summary>
            /// Plurimédia Online
            ///  </summary>
            plurimediaOnline = 53,
            /// <summary>
            /// Plurimédia With Search
            ///  </summary>
            plurimediaWithSearch = 54,
            /// <summary>
            /// Plurimédia Offline
            ///  </summary>
            plurimediaOffline = 55,
            /// <summary>
            /// DOOH
            /// </summary>
            dooh = 52,
            /// <summary>
            /// Web Promotion
            /// </summary>
            webPromotion =9999
		}
	}
	#endregion

	#region Periodicity
	/// <summary>
	/// Constantes des périodicités
	/// </summary>
	public class Periodicity{
		/// <summary>
		/// Type de périodicité
		/// </summary>
		public enum type{
			/// <summary>
			/// Mensuel 
			/// </summary>
			mensuel=1,
			/// <summary>
			///  Bimestriel
			/// </summary>
			bimestriel=2,
			/// <summary>
			/// Trimestriel
			/// </summary>
			trimestriel=3,
			/// <summary>
			/// Hebdomadaire
			/// </summary>
			hebdomadaire=4,
			/// <summary>
			/// Semestriel
			/// </summary>
			semestriel=6,
			/// <summary>
			/// 8 par an
			/// </summary>
			huitParAn=7,
			/// <summary>
			/// 7 par an
			/// </summary>
			septParAn=8,
			/// <summary>
			/// Quotidienne
			/// </summary>
			quotidienne=9,
			/// <summary>
			/// Bimensuel
			/// </summary>
			bimensuel=10,
			/// <summary>
			/// Annuel
			/// </summary>
			annuel=12,
			/// <summary>
			/// ponctuel
			/// </summary>
			ponctuel=13,
			/// <summary>
			/// Bizarre
			/// </summary>
			bizarre=14,
			/// <summary>
			/// Test
			/// </summary>
			test=15,
			/// <summary>
			/// Hors Serie
			/// </summary>
			horsSerie=16,
			/// <summary>
			/// Supplement
			/// </summary>
			supplement=17,
			/// <summary>
			/// 5 par an
			/// </summary>
			cinqParAn=18,
			/// <summary>
			/// Seconde
			/// </summary>
			seconde=70,
			/// <summary>
			/// Minute
			/// </summary>
			minute=71,
			/// <summary>
			/// Heure
			/// </summary>
			heure=72,
			/// <summary>
			/// Indetermine
			/// </summary>
			indetermine=99
		}

		#region Constantes
		/// <summary>
		/// Nombre de jour pour un hebdomadaire
		/// </summary>
		public const int DAY_NUMBER_IN_WEEK=7;
		/// <summary>
		/// Nombre de jour pour un bimensuel
		/// </summary>
		public const int DAY_NUMBER_IN_BIWEEKLY=14;
		/// <summary>
		/// Nombre de mois pour un bimestriel
		/// </summary>
		public const int MONTH_NUMBER_IN_BIMONTHLY=2;
		/// <summary>
		/// Nombre de mois pour un trimestriel
		/// </summary>
		public const int MONTH_NUMBER_IN_QUATERLY=3;

		

		#endregion
	}
	#endregion

	#region encarts
/// <summary>
/// constantes des types d'encarts
/// </summary>
	public class insertType{
		#region constantes encarts
		/// <summary>
		/// Encart volant
		/// </summary>
		public const int FLYING_INSERT=85;
		/// <summary>
		/// Encart
		/// </summary>
		public const int INSERT=108;
		/// <summary>
		/// Excart
		/// </summary>
		public const int EXCART=999;
		/// <summary>
		/// Hors encart
		/// </summary>
		public const int EXCEPT_INSERT=0;
		#endregion
	}
	#endregion

	#region Pan euro
	/// <summary>
	/// Constantes pour pan euro
	/// </summary>
	public class panEuro{
		/// <summary>
		/// Valeur de l'identifiant pan euro
		/// </summary>
		public const int PAN_EURO_CATEGORY=30;
//		/// <summary>
//		/// Valeur de l'identifiant tv pan euro parrainage
//		/// </summary>
//		public const int PAN_EURO_PARRAINAGE_CATEGORY=15;

	}

	#endregion

    #region Formats
    /// <summary>
    /// Formats
    /// </summary>
    public class Formats {
        /// <summary>
        /// None
        /// </summary>
        public const Int64 None = 0;
        /// <summary>
        /// Banners
        /// </summary>
        public const Int64 Banners = 1;
        /// <summary>
        /// In-Stream Video
        /// </summary>
        public const Int64 InStream = 2;
    }
    #endregion
}
