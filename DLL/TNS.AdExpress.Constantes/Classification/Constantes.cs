#region Informations
// Auteur: B. Masson
// Date de création: 09/03/2004
// Date de modification: 16/03/2004
#endregion

using System;
using System.Collections;

namespace TNS.AdExpress.Constantes.Classification{

	/// <summary>
	/// Cette classe définit le type de branche au niveau nomenclature
	/// </summary>
	public class Branch{
		
		#region Enumérateur
		/// <summary>
		/// Types de nomenclature
		/// </summary>
		public enum type{
			/// <summary>
			/// Aucun
			/// </summary>
			nothing=0,
			/// <summary>
			/// Type de support
			/// </summary>
			media=1,
			/// <summary>
			/// Type de produit
			/// </summary>
			product=2,
			/// <summary>
			/// Type d'annonceur
			/// </summary>
			advertiser=3,
			/// <summary>
			/// Type press
			/// </summary>
			mediaPress=4,
			/// <summary>
			/// Type radio
			/// </summary>
			mediaRadio=5,
			/// <summary>
			/// type tv
			/// </summary>
			mediaTv=6,
			/// <summary>
			/// Marque
			/// </summary>
			brand=7,
			/// <summary>
			/// type Outdoor
			/// </summary>
			mediaOutdoor=8,
			/// <summary>
			/// type Pan euro
			/// </summary>
			mediaOthers=9,
			/// <summary>
			/// type Presse inter
			/// </summary>
			mediaInternationalPress=10,
			/// <summary>
			/// type tv
			/// </summary>
			mediaTvSponsorship=11,
			/// <summary>
			/// type genre émission
			/// </summary>
			programType=12,
			/// <summary>
			/// forme de parrainage
			/// </summary>
			sponsorshipForm=13,
			/// <summary>
			/// type Internet
			/// </summary>
			mediaInternet=14,
            /// <summary>
            /// type Adnettrack
            /// </summary>
            mediaAdnettrack = 15,
            /// <summary>
            /// type Instore
            /// </summary>
            mediaInstore = 16,
		}
		#endregion

	}

	/// <summary>
	/// Classe qui définit les niveaux de des nomenclature
	/// </summary>
	public class Level{

		#region Enumérateur
		/// <summary>
		/// Niveaux de nomenclature
		/// </summary>
		public enum type{
			/// <summary>
			/// Niveau de nomenclature produit
			/// </summary>
			product,
			/// <summary>
			/// Niveau de nomenclature variété
			/// </summary>
			segment,
			/// <summary>
			/// Niveau de nomenclature groupe
			/// </summary>
			group,
			/// <summary>
			/// Niveau de nomenclature classe
			/// </summary>
			subsector,
			/// <summary>
			/// Niveau de nomenclature famille
			/// </summary>
			sector,
			/// <summary>
			/// Niveau de nomenclature annonceur
			/// </summary>
			advertiser,
			/// <summary>
			/// Niveau de nomenclature groupe de société
			/// </summary>
			holding_company,
			/// <summary>
			/// Niveau de nomenclature support
			/// </summary>
			media,
			/// <summary>
			/// Niveau de nomenclature catégorie
			/// </summary>
			category,
			/// <summary>
			/// Niveau de nomenclature média
			/// </summary>
			vehicle,
			/// <summary>
			/// Niveau de nomenclature marque
			/// </summary>
			brand

		}
		#endregion

	}
}
