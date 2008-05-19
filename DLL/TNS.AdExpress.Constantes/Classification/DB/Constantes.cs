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
			interest_center

		}
	}
	#endregion

	#region Vehicles
	/// <summary>
	/// Valeurs des vehicles dans la base de données
	/// </summary>
	public class Vehicles{
		/// <summary>
		/// Véhicles
		/// </summary>
		public enum names{
			/// <summary>
			/// Presse
			/// </summary>
			press=1,
			/// <summary>
			/// Radio
			/// </summary>
			radio=2,
			/// <summary>
			/// Télévision
			/// </summary>
			tv=3,
			/// <summary>
			/// Media tactique
			/// </summary>
			mediasTactics=4,			
			/// <summary>
			/// Les autres média
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
			/// Prublicité extéreur
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
			emailing = 13
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
}
