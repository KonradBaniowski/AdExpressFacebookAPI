#region Information
// Auteur: 
// Cr�ation: 
// Modifications: D. V. Mussuma 05/12/2005 correspondance des valeurs types unit�s et p�riodes avec la base de donn�es.
#endregion

using System;
using System.Collections;

namespace TNS.AdExpress.Constantes.Web{
	
	#region Constantes des Sessions
	/// <summary>
	/// Constantes de relatives aux noyau d'AdExpress 3.0 
	/// </summary>
	public class Core{
		/// <summary>
		/// Cl�s des param�tres enregistr�s dans une session cliente
		/// </summary>
		public enum SessionParamters{
			/// <summary>
			/// La pond�ration soit elle �tre utilis�e
			/// </summary>
			/// <remarks>Sert � la cr�ation de l'objet Weighting</remarks>
			weightingUse=1,
			/// <summary>
			/// Valeur de la pond�ration
			/// </summary>
			/// <remarks>Sert � la cr�ation de l'objet Weighting</remarks>
			weightingValue=2,
			/// <summary>
			/// Nom de la pond�ration
			/// </summary>
			/// <remarks>Sert � la cr�ation de l'objet Weighting</remarks>
			weightingDisplayName=3,
			/// <summary>
			/// Niveau de d�tail produit pour les r�sultat
			/// </summary>
			/// <remarks>Sert � la cr�ation de l'objet GenericDetailLevel, il est de type ArrayList</remarks>
			genericProductDetailLevel=4,
			/// <summary>
			/// Niveau de d�tail media pour les r�sultat
			/// </summary>
			/// <remarks>Sert � la cr�ation de l'objet GenericDetailLevel, il est de type ArrayList</remarks>
			genericMediaDetailLevel=5,
			/// <summary>
			/// Emplacement d'o� a �t� s�lectionn� le niveau de d�tail orient� produit
			/// </summary>
			genericProductDetailLevelSelectedFrom=6,
			/// <summary>
			/// Emplacement d'o� a �t� s�lectionn� le niveau de d�tail orient� media
			/// </summary>
			genericMediaDetailLevelSelectedFrom=7,
			/// <summary>
			/// Colonnes generqiues
			/// 			/// </summary>
			/// <remarks>Sert � la cr�ation de l'objet GenericColumns, il est de type ArrayList</remarks>
			genericColumns=8,
			/// <summary>
			/// Emplacement d'o� a �t� s�lectionn� les colonnes
			/// </summary>			
			genericColumnsSelectedFrom=9,
			/// <summary>
			/// Niveau de d�tail g�n�rique pour les r�sultat
			/// </summary>			
			genericDetailLevel=10,
			/// <summary>
			/// Emplacement d'o� a �t� s�lectionn� le niveau de d�tail g�n�rique
			/// </summary>			
			genericDetailLevelSelectedFrom=11,
			/// <summary>
			/// Type du niveau de d�tail g�n�rique
			/// </summary>			
			genericDetailLevelType=12,
			/// <summary>
			/// Type affiner univers version
			/// </summary>
			personnalizeSloganUniverse=13,
			/// <summary>
			/// Identifiant du slogan servant a � faire un Zoom (Int64)
			/// </summary>
			sloganIdZoom=14,
			/// <summary>
			/// Identifiant de la S�lection original des genres d'�missions de l'univers courant
			/// </summary>
			selectionUniversProgramType=15,
			/// <summary>
			/// Cl� d'acc�s � l'ordre de tri
			/// </summary>
			sortOrderKey=16,
			/// <summary>
			/// Cl� d'acc�s � la cl� identifiant la colonne � trier.
			/// </summary>
			sortKeyKey=17,
			/// <summary>
			/// Identifiant de la S�lection courante des genres d'�missions de l'univers courant
			/// </summary>
			currentUniversProgramType=18,
			/// <summary>
			/// Type d'alignement des pourcentages
			/// </summary>
			percentageAlignment=19, 
			/// <summary>
			/// Identifiant de la S�lection original des formes de parrainages de l'univers courant
			/// </summary>
			selectionUniversSponsorshipForm=20,
			/// <summary>
			/// Identifiant de la S�lection courante des formes de parrainages de l'univers courant
			/// </summary>
			currentUniversSponsorshipForm=21,
			/// <summary>
			/// Company Id
			/// </summary>
			companyId=22,
			/// <summary>
			/// Company Name
			/// </summary>
			companyLabel=23,
			/// <summary>
			/// Customer navigator information
			/// </summary>
			browser=24,
			/// <summary>
			/// Customer navigator version information
			/// </summary>
			browserVersion=25,
			/// <summary>
			/// Customer navigator user agent information
			/// </summary>
			userAgent=26,
			/// <summary>
			/// Customer Os
			/// </summary>
			customerOs=27,
			/// <summary>
			/// Customer Ip
			/// </summary>
			customerIp=28,
			/// <summary>
			/// last url set un WebPage (AdExpress base page) use for customer erro
			/// </summary>
			lastWebPage=29,
			/// <summary>
			/// Server Name
			/// </summary>
			serverName=30,
			/// <summary>
			/// AdNetTrack detail levels
			/// </summary>
			/// <remarks>Use to create GenericDetailLevel object, its type isArrayList</remarks>
			genericAdNetTrackDetailLevel=31,
			/// <summary>
			/// Use to know where the AdNetTrack level has been selected
			/// </summary>
			genericAdNetTrackDetailLevelSelectedFrom=32,
			/// <summary>
			/// Generic detail level type for AdNetTrack Level detail Object
			/// </summary>			
			genericAdNetTrackDetailLevelType=33,
			/// <summary>
			/// AdNetTrack Selection Type
			/// </summary>
			adNetTrackSelectionType=34,
			/// <summary>
			/// AdNetTrack SElection Id
			/// </summary>
			adNetTrackSelectionId=35,
            /// <summary>
            /// Date de d�but
            /// </summary>
            startDate=36,
            /// <summary>
            /// Date de fin
            /// </summary>
            endDate=37,
            /// <summary>
            /// Indique s'il s'agit d'une �tude comparative
            /// </summary>
            withComparativePeriod=38,
            /// <summary>
            /// Type de la p�riode comparative
            /// </summary>
            comparativePeriodType=39,
            /// <summary>
            /// Type de la disponibilit� des donn�es
            /// </summary>
            periodDisponibilityType=40,
            /// <summary>
            /// Column detail levels
            /// </summary>
            genericColumnDetailLevel = 41,
            /// <summary>
            /// Use to know where the column level has been selected
            /// </summary>
            genericColumnDetailLevelSelectedFrom = 42,
            /// <summary>
            /// Generic detail level type for Column Level detail Object
            /// </summary>			
            genericColumnDetailLevelType = 43,
            /// <summary>
            /// Media selection parent 
            /// </summary>
            mediaSelectionParent = 44,
			/// <summary>
			/// Colonnes generqiues
			/// </summary>
			/// <remarks>Sert � la cr�ation de l'objet GenericColumns, il est de type ArrayList</remarks>
			genericCreativesColumns=45,
			/// <summary>
			/// Emplacement d'o� a �t� s�lectionn� les colonnes
			/// </summary>			
            genericCreativesColumnsSelectedFrom = 46,
            /// <summary>
            ///  Niveau de d�tail g�n�rique pour les s�lection
            /// </summary>
            genericMediaSelectionDetailLevel = 47,
            /// <summary>
            /// Emplacement d'o� a �t� s�lectionn� le niveau de d�tail orient� media
            /// </summary>
            genericMediaSelectionDetailLevelSelectedFrom = 48,
            /// <summary>
            /// Type du niveau de d�tail g�n�rique
            /// </summary>			
            genericSelectionDetailLevelType = 49,
            /// <summary>
            /// Nb Alerts
            /// </summary>			
            nbAlerts = 50
		}
	
	}
	/// <summary>
	/// Constantes sp�cifiques aux sessions
	/// </summary>
	public class CustomerSessions
	{

		#region P�riode
		/// <summary>
		/// D�finition des caract�ristiques de p�riode
		/// </summary>
		public class Period
		{
			/// <summary>
			/// D�finition du type de p�riode pour les requ�tes
			/// </summary>		
			#region Type de p�riode
			public enum Type
			{	
				
				/// <summary>
				/// P�riode = dates de d�but et dates de fin fixes en jour
				/// </summary>
				dateToDate=0,
				/// <summary>
				/// P�riode = dates de d�but et dates de fin fixes en semaines
				/// </summary>
				dateToDateWeek=1,
				/// <summary>
				/// P�riode = dates de d�but et dates de fin fixes en mois
				/// </summary>
				dateToDateMonth=2,
				/// <summary>
				/// P�riode = n derniers jours
				/// </summary>
				nLastDays=3,
				/// <summary>
				/// P�riode = n derni�re semaine
				/// </summary>
				nLastWeek=4,
				/// <summary>
				/// P�riode = n dernier mois
				/// </summary>
				nLastMonth=5,
				/// <summary>
				/// n derni�res ann�es
				/// </summary>
				nLastYear=6,
				/// <summary>
				/// P�riode = semaine pr�c�dente
				/// </summary>
				previousWeek=7,
				/// <summary>
				/// P�riode = mois pr�c�dent
				/// </summary>
				previousMonth=8,
				/// <summary>
				/// Ann�e courante
				/// </summary>
				currentYear=9,
				/// <summary>
				/// P�riode = ann�e pr�c�dente
				/// </summary>
				previousYear=10,
				/// <summary>
				/// P�riode = avant derni�re ann�e = ann�e N-2
				/// </summary>
				nextToLastYear=11,
				/// <summary>
				/// Cumul � date
				/// </summary>
				cumlDate=12,
				/// <summary>
				/// Derniere semaine charg�e
				/// </summary>
				LastLoadedWeek=13,
				/// <summary>
				/// Dernier mois charg�
				/// </summary>
				LastLoadedMonth=14,
				/// <summary>
				/// Jour pr�c�dent
				/// </summary>
				previousDay=15,
                /// <summary>
                /// P�riode s�lectionn�e � partir du composant globalDate
                /// </summary>
                globalDate=16
			}
			#endregion

			/// <summary>
			/// D�finition du niveau de d�tail de l'affichage des p�riodes (ann�e, mois, semaine, jours).
			/// Attention, cette classe n'est pas utilisable pour sp�cifier le pr�formatage des tableaux dynamiques.
			/// </summary>
			
			#region Niveau de d�tail de l'affichage des p�riodes
			public enum DisplayLevel
			{
				/// <summary>
				/// Affichage d�taill� par jour
				/// </summary>
				dayly,
				/// <summary>
				/// Affichage d�taill� par semaine
				/// </summary>
				weekly,
				/// <summary>
				/// Affichage d�taill� par mois
				/// </summary>
				monthly,
				/// <summary>
				/// Affichage d�taill� par ann�e
				/// </summary>
				yearly
			}
			#endregion

			#region Type de dates de publication
			/// <summary>
			/// D�finition du type de date de parution
			/// </summary>
			public enum publicationType{
				/// <summary>
				/// Dates de parution � date fixe
				/// </summary>
				staticDate,
				/// <summary>
				/// Dates de parution en fonction d'un jour nomm�
				/// </summary>
				namedDayDate,
				/// <summary>
				/// Dates de partuion variables
				/// </summary>
				variableDate
			}
			#endregion

            #region PeriodBreakdown
            /// <summary>
            /// PeriodBreakDowns
            /// </summary>
            public enum PeriodBreakdownType
            {
                /// <summary>
                /// Period is daily and contained in the last four month
                /// </summary>
                data_4m,
                /// <summary>
                /// Period is daily
                /// </summary>
                data,
                /// <summary>
                /// Period is weeks
                /// </summary>
                week,
                /// <summary>
                /// Period is monthes
                /// </summary>
                month

            }
            #endregion

        }
		#endregion

		#region Unit�

		#region Enum�rateur des unit�
		/// <summary>
		/// Type d'unit�
		/// </summary>
		public enum Unit
		{
			/// <summary>
			/// Used in unit decription
			/// </summary>
            none=9999,
			/// <summary>
			/// Unit� = euros
			/// </summary>
			euro=0,
			/// <summary>
			/// Unit� = nombre de spots
			/// </summary>
			spot=1,
			/// <summary>
			/// Unit� = nombre d'insertions
			/// </summary>
			insertion=2,
			/// <summary>
			/// Unit� = dur�e des spots
			/// </summary>
			duration=3,
			/// <summary>
			/// Unit� = taille des insertions
			/// </summary>
			mmPerCol=4,
			/// <summary>
			/// Unit� = nombre de pages
			/// </summary>
			pages=5,
			/// <summary>
			/// Unit�= nombre de panneaux
			/// </summary>
			numberBoard=6,
			/// <summary>
			/// Unit�= GRP
			/// </summary>
			grp=7,
			/// <summary>
			/// Unit� = Kilo Euros
			/// </summary>
			kEuro=8,
            /// <summary>
            /// Unit� = Volumes
            /// </summary>
            volume=9,
            /// <summary>
            /// Unit� = Nombres de versions
            /// </summary>
            versionNb = 10,
		    /// <summary>
            /// Unit� = Nombre d'occurences
            /// </summary>
            occurence = 11

		}
		#endregion

		/// <summary>
		/// Codes de traduction des diff�rentes unit�s
		/// </summary>
		/// <example>
		/// UnitsTraductionCodes[TNS.AdExpress.Constantes.Web.CustomerSessions.Units.duration] renvoie le code de traduction de l'unit� dur�e
		/// </example>
		public static Hashtable UnitsTraductionCodes;
		/// <summary>
		/// Codes de traduction des diff�rentes unit�s pour les pages Excel
		/// </summary>
		public static Hashtable XLSUnitsTraductionCodes;
		/// <summary>
		/// Codes de traduction des encarts
		/// </summary>
		public static Hashtable InsertsTraductionCodes;

		#endregion

		#region Tri
		/// <summary>
		/// Classe d�finissant l'ordre de tri des r�sultats affich�s
		/// </summary>
		public class SortOrder
		{
			
			/// <summary>
			/// Tri croissant
			/// </summary>
			public const string CROISSANT = "asc";
			/// <summary>
			/// Tri d�croissant
			/// </summary>
			public const string DECROISSANT = "desc";

		}
		#endregion

		#region Etudes
		/// <summary>
		/// Crit�re de comparaison ( comparaison sur le total univers, march� ou famille)
		/// </summary>	
		
		public enum ComparisonCriterion
		{
			/// <summary>
			/// Comparaison par rapport au total march�
			/// </summary>
			marketTotal,
			/// <summary>
			/// Comparaison par rapport au total des familles concern�es par la s�lection
			/// </summary>
			sectorTotal,
			/// <summary>
			/// Comparaison par rapport au total de l'univers s�lectionn�
			/// </summary>
			universTotal
		}
		#endregion

		#region Niveaux de d�tail pr�format�s
		/// <summary>
		/// Constantes des niveaux de d�tails pr�format�s (produits, m�dias, p�riodes)
		/// </summary>
		public class PreformatedDetails
		{
			/// <summary>
			/// Niveau de d�tails pr�format�s pour la nomenclature produit et annonceur
			/// </summary>
			public enum PreformatedProductDetails
			{
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau famille
				/// </summary>
				sector,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau famille > produit
				/// </summary>
				sectorProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau famille > classe > groupe
				/// </summary>
				sectorSubsectorGroup,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau famille > classe
				/// </summary>
				sectorSubsector,
				/// <summary>
				///  Nomenclature produit d�taill�e au niveau famille > annonceur 
				/// </summary>
				sectorAdvertiser,
				/// <summary>
				///  Nomenclature produit d�taill�e au niveau famille > annonceur > produit 
				/// </summary>
				sectorAdvertiserProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe
				/// </summary>
				group,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe > vari�t�
				/// </summary>
				groupSegment,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe > marque
				/// </summary>
				groupBrand,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe > produit
				/// </summary>
				groupProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe > annonceur
				/// </summary>
				groupAdvertiser,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe > marque > produit
				/// </summary>
				groupBrandProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe > annonceur > marque
				/// </summary>
				groupAdvertiserBrand,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe > annonceur > produit
				/// </summary>
				groupAdvertiserProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe > annonceur > marque > produit
				/// </summary>
				groupAvertiserBrandProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe d'agence > agence
				/// </summary>
				group_agencyAgency,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe d'agence > agence > Annonceur
				/// </summary>
				group_agencyAgencyAdvertiser,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe d'agence > agence > Produit
				/// </summary>
				group_agencyAgencyProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau agence > produit
				/// </summary>
				agencyProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau agence > annonceur
				/// </summary>
				agencyAdvertiser,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau annonceur
				/// </summary>
				advertiser,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau annonceur > marque
				/// </summary>
				advertiserBrand,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau annonceur > produit
				/// </summary>
				advertiserProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau annonceur > groupe > marque
				/// </summary>
				advertiserGroupBrand,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau annonceur > groupe > produit
				/// </summary>
				advertiserGroupProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau anonceur > marque > produit
				/// </summary>
				advertiserBrandProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau annonceur > groupe > marque > produit
				/// </summary>
				advertiserGroupBrandProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau annonceur > groupe > cat�gorie > produit
				/// </summary>
				advertiserGroupSegmentProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau annonceur > groupe > cat�gorie > marque > produit
				/// </summary>
				advertiserGroupSegmentBrandProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau marque
				/// </summary>
				brand,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe de soci�t�
				/// </summary>
				holdingCompany,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe de soci�t� > annonceur
				/// </summary>
				holdingCompanyAdvertiser,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe de soci�t� > annonceur > marques
				/// </summary>
				holdingCompanyAdvertiserBrand,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau groupe de soci�t� > annonceur > produits
				/// </summary>
				holdingCompanyAdvertiserProduct,

				//modifications for the addition of segmentAdvertiser, segmentProduct, segmentBrand
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau vari�t� -> announceur
				/// </summary>
				segmentAdvertiser,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau vari�t� -> produit
				/// </summary>
				segmentProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau vari�t� -> marque
				/// </summary>
				segmentBrand,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau vari�t� -> Annoneur -> Produit
				/// </summary>
				segmentAdvertiserProduct,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau vari�t� -> Annoceur -> marque
				/// </summary>
				segmentAdvertiserBrand,
				/// <summary>
				/// Nomenclature produit d�taill�e au niveau produit
				/// </summary>
				product,
				/// <summary>
				///  Nomenclature produit d�taill�e au niveau Famille -> Groupe de soci�t� -> Annonceur
				/// </summary>
                sectorHoldingCompanyAdvertiser,
                /// <summary>
                /// Nomenclature produit d�taill�e au niveau Classe
                /// </summary>
                subSector,
                /// <summary>
                /// Nomenclature produit d�taill�e au niveau Classe -> Annonceur
                /// </summary>
                subSectorAdvertiser
			}

	
			/// <summary>
			/// Niveau de d�tails pr�format�s pour la nomenclature support
			/// </summary>
			public enum PreformatedMediaDetails
			{
				/// <summary>
				/// Tableau donnant le total des M�dias
				/// </summary>
				vehiculeTotal,
				/// <summary>
				/// Tableau d�taill� par m�dia
				/// </summary>
				vehicle,
				/// <summary>
				/// Tableau d�taill� par m�dia et cat�gories
				/// </summary>
				vehicleCategory,
				/// <summary>
				/// Tableau d�taill� par m�dia,cat�gories et supports
				/// </summary>
				vehicleCategoryMedia,
				/// <summary>
				/// Tableau d�taill� par m�dia et centre d'int�r�ts
				/// </summary>
				vehicleInterestCenter,
				/// <summary>
				/// Tableau d�taill� par m�dia , centre d'int�r�ts et supports
				/// </summary>
				vehicleInterestCenterMedia,
				/// <summary>
				/// Tableau d�taill� par m�dia et supports
				/// </summary>
				vehicleMedia,
				/// <summary>
				/// Tableau d�taill� par Media et r�gie
				/// </summary>
				vehicleMediaSeller,
				/// <summary>
				/// Tableau d�taill� par media, r�gie et supports
				/// </summary>
				vehicleMediaSellerMedia,
				/// <summary>
				/// Tableau d�taill� par r�gie et supports
				/// </summary>
				mediaSellerMedia,
				/// <summary>
				/// Tableau d�taill� par r�gie, media et supports
				/// </summary>
				mediaSellerVehicleMedia,
				/// <summary>
				/// Tableau d�taill� par r�gie et centre d'int�r�t
				/// </summary>
				mediaSellerInterestCenter,
				/// <summary>
				/// Tableau d�taill� par r�gie, centre d'int�r�t et supports
				/// </summary>
				mediaSellerInterestCenterMedia,
				/// <summary>
				/// Tableau d�taill� par Media / Cat�gories / Supports / Accroches
				/// </summary>
				vehicleCategoryMediaSlogan,
				/// <summary>
				/// Tableau d�taill� par Media / Supports / Accroches 
				/// </summary>
				vehicleMediaSlogan,
				/// <summary>
				/// Tableau d�taill� par Media / Centres d'int�r�t / Supports / Accroches 
				/// </summary>
				vehicleInterestCenterMediaSlogan,
				/// <summary>
				/// Tableau d�taill� par Media / R�gies / Supports / Accroches
				/// </summary>
				vehicleMediaSellerMediaSlogan,
				/// <summary>
				/// Tableau d�taill� par R�gies / Supports / Accroches
				/// </summary>
				mediaSellerMediaSlogan,
				/// <summary>
				/// Tableau d�taill� par R�gies / Media / Supports/ Accroches
				/// </summary>
				mediaSellerVehicleMediaSlogan,
				/// <summary>
				/// Tableau d�taill� par R�gies / Centres d'int�r�t / Supports / Accroches
				/// </summary>
				mediaSellerInterestCenterMediaSlogan,
				/// <summary>
				/// Tableau d�taill� par Accroches / Supports 
				/// </summary>
				sloganMedia,
				/// <summary>
				/// Tableau d�taill� par Accroches / Media / Supports
				/// </summary>
				sloganVehicleMedia,
				/// <summary>
				/// Tableau d�taill� par Accroches / Media / Cat�gories / Supports 
				/// </summary>
				sloganVehicleCategoryMedia,
				/// <summary>
				/// Tableau d�taill� par Accroches / Media / Centres d'int�r�t / Supports
				/// </summary>
				sloganVehicleInterestCenterMedia,
				/// <summary>
				/// Tableau d�taill� par Accroches / Media / R�gies/Supports
				/// </summary>
				sloganVehicleMediaSellerMedia,
				/// <summary>
				/// Tableau d�taill� par Accroches / R�gies / Supports
				/// </summary>
				sloganMediaSellerMedia,
				/// <summary>
				/// Tableau d�taill� par Accroches / R�gies / Media / Supports
				/// </summary>
				sloganMediaSellerVehicleMedia,
				/// <summary>
				/// Tableau d�taill� par Accroches / R�gies /Centres d'int�r�t / Supports
				/// </summary>
				sloganMediaSellerInterestCenterMedia,
				/// <summary>
				/// Tableau d�taill� par Media / Titre/ Supports
				/// </summary>
				vehicleTitleMedia,
				/// <summary>
				/// Tableau d�taill� par Supports
				/// </summary>
				Media,
				/// <summary>
				/// Tableau d�taill� par Genre d'�missions / Emissions
				/// </summary>
				programTypeProgram,
				/// <summary>
				/// Tableau d�taill� par Formes de parrainage
				/// </summary>
				sponsorshipForm,
				/// <summary>
				/// Tableau d�taill� par Media / Titre/ Supports
				/// </summary>
				vehicleCountryMedia
						
			}

	
			/// <summary>
			/// Niveau de d�tails pr�format�s pour les p�riodes
			/// </summary>
			public enum PreformatedPeriodDetails
			{
				/// <summary>
				/// Tableau cumul� ann�e
				/// </summary>
				yearTotal,
				/// <summary>
				/// Tableau mois par mois + cumul (par d�faut)
				/// </summary>
				monthly_And_Total,
				/// <summary>
				/// Tableau mensuel cumul�
				/// </summary>
				monthlyCumulative,
				/// <summary>
				/// Tableau trimestriel + cumul
				/// </summary>
				quarterly_And_Total,
				/// <summary>
				/// Tableau trimestriel cumul�
				/// </summary>
				quarterlyCumulative,
				/// <summary>
				/// Tableau semestriel + cumul
				/// </summary>
				semester_And_Cumul,
				/// <summary>
				///	Tableau semestriel cumul�
				/// </summary>
				semesterCumulative
			}

			
			/// <summary>
			/// Constantes indiquant le type de pr�formatage des lignes et colonnes de tableaux
			/// </summary>
			public enum PreformatedTables
			{
				/// <summary>
				/// Tableau avec:
				///		en lignes : Supports
				///		en colonnes : Ann�e
				/// </summary>
				media_X_Year,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Produits
				///		en colonnes : Ann�e
				/// </summary>
				product_X_Year,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Produits > Supports
				///		en colonnes : Ann�e
				/// </summary>
				productMedia_X_Year,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Supports > Produits
				///		en colonnes : Ann�e
				/// </summary>
				mediaProduct_X_Year,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Produits > Supports
				///		en colonnes : Ann�e + Mois
				/// </summary>
				productMedia_X_YearMensual,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Supports > Produits
				///		en colonnes : Ann�e + Mois
				/// </summary>
				mediaProduct_X_YearMensual,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Produits > Ann�e
				///		en colonnes : Supports
				/// </summary>
				productYear_X_Media,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Produits > Ann�e
				///		en colonnes : Cumul + Mensuel
				/// </summary>
				productYear_X_Mensual,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Produits > Ann�e
				///		en colonnes : Mois cumul�s
				/// </summary>
				productYear_X_Cumul,				
				/// <summary>
				/// Tableau avec:
				///		en lignes : Supports > Ann�e
				///		en colonnes : Cumul + Mensuel
				/// </summary>
				mediaYear_X_Mensual,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Supports > Ann�e
				///		en colonnes : Mois cumul�s
				/// </summary>
				mediaYear_X_Cumul,
				/// <summary>
				/// Tableau avec:
				///		en lignes : M�dia>Centre d'int�r�ts>Supports 
				///		en colonnes : Unit�s
				/// </summary>
				vehicleInterestCenterMedia_X_Units,
				/// <summary>
				/// Tableau avec:
				///		en lignes : M�dia>Centre d'int�r�ts>Supports 
				///		en colonnes : Cumul + Mensuel
				/// </summary>
				vehicleInterestCenterMedia_X_Mensual,
				/// <summary>
				/// Tableau avec:
				///		en lignes : cumul famille + familles 
				///		en colonnes : Cumul + Mensuel
				/// </summary>
				sector_X_Mensual,
				/// <summary>
				/// Tableau avec:
				///		en lignes : cumul famille + familles 
				///		en colonnes : Cumul + Format
				/// </summary>
				sector_X_Format,
				/// <summary>
				/// Tableau avec:
				///		en lignes : M�dia>Centre d'int�r�ts>Supports  
				///		en colonnes : Cumul + Format
				/// </summary>
				vehicleInterestCenterMedia_X_Format,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Unit�s  
				///		en colonnes : Cumul + Format
				/// </summary>
				units_X_Format,
				/// <summary>
				/// Tableau avec:
				///		en lignes : cumul famille + familles   
				///		en colonnes : Cumul + Jour Nomm�
				/// </summary>
				sector_X_NamedDay,
				/// <summary>
				/// Tableau avec:
				///		en lignes : M�dia>Centre d'int�r�ts>Supports  
				///		en colonnes : Cumul + Jour Nomm�
				/// </summary>
				vehicleInterestCenterMedia_X_NamedDay,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Unit�s
				///		en colonnes : Cumul + Jour Nomm�
				/// </summary>
				units_X_NamedDay,
				/// <summary>
				/// Tableau avec:
				///		en lignes : cumul famille + familles   
				///		en colonnes : Cumul + Tranche horaire
				/// </summary>
				sector_X_TimeSlice,
				/// <summary>
				/// Tableau avec:
				///		en lignes : M�dia>Centre d'int�r�ts>Supports  
				///		en colonnes : Cumul + Tranche horaire
				/// </summary>
				vehicleInterestCenterMedia_X_TimeSlice,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Unit�s
				///		en colonnes : Cumul + Tranche horaire
				/// </summary>
				units_X_TimeSlice,
				/// <summary>
				/// Tableau avec:
				///		en lignes : M�dia>Centre d'int�r�ts>Supports 
				///		en colonnes :  cumul famille + familles   
				/// </summary>
				vehicleInterestCenterMedia_X_Sector,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Autres dimensions 
				///		en colonnes :  cumul Periode + Periode   
				/// </summary>
				othersDimensions_X_Period,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Autres dimensions 
				///		en colonnes :  cumul supports + supports   
				/// </summary>
				othersDimensions_X_Media,
				/// <summary>
				/// Tableau avec:
				///		en lignes : Autres dimensions 
				///		en colonnes :  cumul unit�s + unit�s   
				/// </summary>
				othersDimensions_X_Units,
                /// <summary>
                /// Tableau avec :
                ///     en lignes : Formats
                ///     en colonnes : Nb de banni�res, Nb d'occurrences
                /// </summary>
                format_X_Units,
                /// <summary>
                /// Tableau avec :
                ///     en lignes : Dimensions
                ///     en colonnes : Nb de banni�res, Nb d'occurrences
                /// </summary>
                dimension_X_Units,
                /// <summary>
                /// Tableau avec :
                ///     en lignes : Dimensions top 20
                ///     en colonnes : Nb de banni�res, Nb d'occurrences
                /// </summary>
                dimension_Top20_X_Units,
                /// <summary>
                /// Tableau avec :
                ///     en lignes : Dimensions
                ///     en colonnes : Format
                /// </summary>
                dimension_Format
			}

		}
		#endregion

		#region Constructeur statique
		/// <summary>
		/// Constructeur statique de la classe CustomerSessions : initialise la HashTable UnitsTraductionCodes avec comme cl� les unit�s pr�sentes dans TNS.AdExpress.Constantes.Web.CustomerSessions.Units et comme valeur un entier repr�sentant le code de traduction de l'unit�
		/// </summary>
		static CustomerSessions()
		{
			UnitsTraductionCodes = new Hashtable();
			UnitsTraductionCodes.Add(Unit.euro,938);
			UnitsTraductionCodes.Add(Unit.spot,939);
			UnitsTraductionCodes.Add(Unit.insertion,940);
			UnitsTraductionCodes.Add(Unit.duration,280);
			UnitsTraductionCodes.Add(Unit.mmPerCol,941);
			UnitsTraductionCodes.Add(Unit.pages,943);
			UnitsTraductionCodes.Add(Unit.numberBoard,1604);
			UnitsTraductionCodes.Add(Unit.grp,1679);
			UnitsTraductionCodes.Add(Unit.kEuro,1789); // Nouveau
            UnitsTraductionCodes.Add(Unit.volume,2216); // Nouveau

			XLSUnitsTraductionCodes = new Hashtable();
			XLSUnitsTraductionCodes.Add(Unit.euro,938);
			XLSUnitsTraductionCodes.Add(Unit.spot,939);
			XLSUnitsTraductionCodes.Add(Unit.insertion,940);
			XLSUnitsTraductionCodes.Add(Unit.duration,1307);
			XLSUnitsTraductionCodes.Add(Unit.mmPerCol,941);
			XLSUnitsTraductionCodes.Add(Unit.pages,943);
			XLSUnitsTraductionCodes.Add(Unit.numberBoard,1604);
			XLSUnitsTraductionCodes.Add(Unit.grp,1679);
			XLSUnitsTraductionCodes.Add(Unit.kEuro,1789); // Nouveau
            XLSUnitsTraductionCodes.Add(Unit.volume, 2216); // Nouveau
			
			InsertsTraductionCodes = new Hashtable();
			InsertsTraductionCodes.Add(Insert.total,1401);
			InsertsTraductionCodes.Add(Insert.insert,1402);
			InsertsTraductionCodes.Add(Insert.withOutInsert,1403);
		}
		#endregion

		#region Encarts
		/// <summary>
		/// Crit�re pour les encarts
		/// </summary>
		public enum Insert
		{
			/// <summary>
			/// Total
			/// </summary>
			total,
			/// <summary>
			/// Encarts
			/// </summary>
			insert,
			/// <summary>
			/// Hors encarts
			/// </summary>
			withOutInsert
		
		}	
		
		/// <summary>
		/// Type d'encart
		/// </summary>
		public enum InsertType
		{
			/// <summary>
			/// inset
			/// </summary>
			encart=108,
			/// <summary>
			/// Flying inset
			/// </summary>
			flyingEncart=85,
			/// <summary>
			/// excart
			/// </summary>
			Excart=999,
			/// <summary>
			/// Subscriber inset
			/// </summary>
			subScriberEncart = 1983
		}	
		#endregion

		#region Nouveau produit
		/// <summary>
		/// Nouveau produit (on l'utilise dans l'alerte d'un portefeuille)
		/// </summary>
		public enum NewProduct
		{
			/// <summary>
			/// Nouveau produit dans la pige
			/// </summary>
			pige=1,
			/// <summary>
			/// Nouveau produit dans le support
			/// </summary>
			support=2
		}

		#endregion

	}
	#endregion

	#region Cookies
	/// <summary>
	/// List every cookie used in the web site
	/// </summary>
	public class Cookies{
		/// <summary>
		/// Cooky dedicated to the language management
		/// </summary>
		public static string LANGUAGE = "language";

		/// <summary>
		/// Cooky dedicated to the header repetition
		/// </summary>
		public static string IsShowParenHeader = "isShowParenHeader";

		/// <summary>
		/// Cooky dedicated to the header repetition of media insertions
		/// </summary>
		public static string IsShowMediaInsertionsParenHeader = "isShowMediaInsertionsParenHeader";

		/// <summary>
		/// Cooky dedicated to the current page size
		/// </summary>
		public static string CurrentPageSize = "currentPageSize";
		/// <summary>
		/// Cooky dedicated to the current page size in th ecreatives pages
		/// </summary>
		public static string CurrentPageSizeCreatives = "currentPageSizeCreatives";

		/// <summary>
		/// Cooky dedicated to the type of media file open
		/// </summary>
		public static string SpotFileType = "spotFileType";

		/// <summary>
		/// Cooky dedicated to the possibility to register email for Remoting file (pdf, excel...) export
		/// </summary>
		public static string IsRegisterEmailForRemotingExport = "isRegisterEmailForRemotingExport";

        /// <summary>
        /// Cookie dedicated to the alert email list
        /// </summary>
        public static string AlertEmailList = "alertEmailList";

        /// <summary>
        /// Cookie dedicated to auto-connection when clicking on an alert
        /// link
        /// </summary>
        public static string AlertAutoConnectCookie = "alertAutoConnect";

		/// <summary>
		/// Cooky dedicated to save email for Remoting file (pdf, excel...) export
		/// </summary>
		public static string SavedEmailForRemotingExport = "savedEmailForRemotingExport";

		/// <summary>
		/// Cooky dedicated to the current page size of media insertions
		/// </summary>
		public static string CurrentMediaInsertionsPageSize = "CurrentMediaInsertionsPageSize";
        /// <summary>
        /// Cooky dedicated to the creatives page size
        /// </summary>
        public static string CreativesPageSize = "CreativesPageSize";

		
	}
	#endregion

	#region Constantes liens hyperText
	/// <summary>
	/// Classe des liens
	/// </summary>
	public class Links
	{
		/// <summary>
		/// Lien vers la page de zoom sur une p�riode dans une analyse plan media
		/// </summary>
		public const string ZOOM_PLAN_MEDIA="/Private/Results/MediaPlanResults.aspx";
        /// <summary>
        /// Lien vers la page de zoom sur une p�riode dans une analyse plan media d'une appm
        /// </summary>
        public const string APPM_ZOOM_PLAN_MEDIA = "/Private/Results/APPMResults.aspx";
        /// <summary>
		/// Lien vers la pop-up de zoom sur une p�riode dans une analyse plan media
		/// </summary>
		public const string ZOOM_PLAN_MEDIA_POP_UP="/Private/Results/ZoomMediaPlanAlertPopUpResults.aspx";
		/// <summary>
		/// Lien vers la page de zoom sur une p�riode dans une analyse plan media concurentiel
		/// </summary>
		public const string ZOOM_COMPETITOR_PLAN_MEDIA="/Private/Results/ZoomCompetitorMediaPlanAnalysisResults.aspx";
		
		/// <summary>
		/// Chemin d'acc�s des fichiers d'aide
		/// </summary>
		public const string HELP_FILE_PATH="/Private/Helps/";

		/// <summary>
		/// Lien site AdExpress
		/// </summary>
		public const string ADEXPRESS_SITE="www.tnsadexpress.com";
        /// <summary>
        /// Lien vers la pop-up de plan m�dia g�n�rique
        /// </summary>
        public const string MEDIA_SCHEDULE_POP_UP = "/Private/Results/MediaSchedulePopUp.aspx";
    }
	#endregion

	#region Modules
	/// <summary>
	/// Constante des groupes de modules
	/// </summary>
	public class Module{
		/// <summary>
		/// Modules qui ne doivent pas �tre utilis�s
		/// </summary>
		public const string NOT_USED_ID_LIST="1153";
		/// <summary>
		/// Types des groupes de modules
		/// </summary>
		public enum Type{
			/// <summary>
			/// Groupe de modules d'alerte
			/// </summary>
			alert,
			/// <summary>
			/// Groupe de modules d'analyse
			/// </summary>
			analysis,
			/// <summary>
			/// groupe de modules de l'analyse sectorielle
			/// </summary>
			recap,
			/// <summary>
			/// Groupe de modules tableaux de bord
			/// </summary>
			dashBoard,
			/// <summary>
			/// Groupe de modules ChronoPresse
			/// </summary>
			chronoPress,
			/// <summary>
			/// Groupe de modules Parrainage TV
			/// </summary>
			tvSponsorship
		}
		/// <summary>
		/// Cat�gorie de module
		/// </summary>
		public enum Category{
			/// <summary>
			/// Aucun
			/// </summary>
			none=0,
			/// <summary>
			/// Media
			/// </summary>
			vehicle=1,
			/// <summary>
			/// Annonceur
			/// </summary>
			advertiser=2,
			/// <summary>
			/// Tableaux de bord
			/// </summary>
			dashboard=3
		}

		#region Identifiant Module
		/// <summary>
		/// Liste des identifiants des modules
		/// </summary>
		public class Name{
			/// <summary>
			/// identifiant alerte plan media
			/// </summary>
			public const int ALERTE_PLAN_MEDIA=198;
			/// <summary>
			/// Identifiant alerte plan media concurrentielle
			/// </summary>
			public const int ALERTE_PLAN_MEDIA_CONCURENTIELLE=199;
			/// <summary>
			/// Identifiant alerte concurrentielle
			/// </summary>
			public const int ALERTE_CONCURENTIELLE=277;
			/// <summary>
			/// Identifiant alerte dynamique
			/// </summary>
			public const int ALERTE_DYNAMIQUE=200;
			/// <summary>
			/// Identifiant alerte de potentielle
			/// </summary>	
			public const int ALERTE_POTENTIELS=280;
			/// <summary>
			/// Identifiant Alerte portefeuille d'un support
			/// </summary>
			public const int ALERTE_PORTEFEUILLE=282;
			/// <summary>
			/// Identifiant Analyse plan media
			/// </summary>
			public const int ANALYSE_PLAN_MEDIA=196;
			/// <summary>
			/// Identifiant analyse plan media concurentielle
			/// </summary>
			public const int ANALYSE_PLAN_MEDIA_CONCURENTIELLE=184;
			/// <summary>
			/// Identifiant analyse concurentielle
			/// </summary>
			public const int ANALYSE_CONCURENTIELLE=278;
			/// <summary>
			/// Identifiant analyse dynamique
			/// </summary>
			public const int ANALYSE_DYNAMIQUE=197;
			/// <summary>
			/// Identifiant analyse de potentielle
			/// </summary>
			public const int ANALYSE_POTENTIELS=281;
			/// <summary>
			/// Identifiant analyse portefeuille
			/// </summary>
			public const int ANALYSE_PORTEFEUILLE=283;
			/// <summary>
			/// Identifiant tableau dynamique
			/// </summary>
			public const int TABLEAU_DYNAMIQUE=194;
			/// <summary>
			/// Identifiant indicateur
			/// </summary>
			public const int INDICATEUR=195;
			/// <summary>
			/// Identifiant tendances
			/// </summary>
			public const int TENDACES=450;
			/// <summary>
			/// Identifiant tableau de bord
			/// </summary>
			public const int TABLEAU_DE_BORD=475;
			/// <summary>
			/// Identifiant tableau de bord presse
			/// </summary>
			public const int TABLEAU_DE_BORD_PRESSE=475;
			/// <summary>
			/// Identifiant tableau de bord radio
			/// </summary>
			public const int TABLEAU_DE_BORD_RADIO=601;
			/// <summary>
			/// Identifiant tableau de bord t�l�vision
			/// </summary>
			public const int TABLEAU_DE_BORD_TELEVISION=602;
			/// <summary>
			/// Identifiant tableau de bord Pan Euro
			/// </summary>
			public const int TABLEAU_DE_BORD_PAN_EURO=1036;
            /// <summary>
            /// Evaliant report Id
            /// </summary>
            public const int TABLEAU_DE_BORD_EVALIANT = 4370;
			/// <summary>
			/// Bilan de Campagne
			/// </summary>
			public const int BILAN_CAMPAGNE=546; 
			/// <summary>
			/// Analyse des dispositifs
			/// </summary>
			public const int ANALYSE_DES_DISPOSITIFS = 1780; 
			/// <summary>
			/// Analyse des programmes
			/// </summary>
			public const int ANALYSE_DES_PROGRAMMES = 1781; 
			/// <summary>
			/// Donn�es de cadrage
			/// </summary>
			public const int DONNEES_DE_CADRAGE = 1920;
			/// <summary>
			/// Justificatifs Presse
			/// </summary>
			public const int JUSTIFICATIFS_PRESSE = 1950;
            /// <summary>
            /// Les nouvelles cr�ations (Evaliant)
            /// </summary>
            public const int NEW_CREATIVES = 4369;
            /// <summary>
            /// Alert AdExpress
            /// </summary>
            public const int ALERT_ADEXPRESS = 5931;
		}
		#endregion

	}
	#endregion

	#region Module Items
	/// <summary>
	/// Constantes des identifiants de traduction des menus de l'entete
	/// </summary>
	public class MenuTraductions
	{
		/// <summary>
		/// Item Identification
		/// </summary>
		public const int IDENTIFIANT = 759;
		/// <summary>
		/// Item modules
		/// </summary>
		public const int MODULES = 760;
		/// <summary>
		/// Item Mon AdExpress
		/// </summary>
		public const int MY_ADEXPRESS = 761;
		/// <summary>
		/// Item News
		/// </summary>
		public const int NEWS = 762;
		/// <summary>
		/// Item Fichier
		/// </summary>
		public const int RESULT_FILES = 765;
		/// <summary>
		/// Item Contact
		/// </summary>
		public const int DATA_UPDATE = 1253;
		/// <summary>
		/// Item Contact
		/// </summary>
		public const int CONTACT = 763;
		/// <summary>
		/// Item Contact
		/// </summary>
		public const int INFO_REQUEST = 756;
		/// <summary>
		/// Item Tns Media Intelligence
		/// </summary>
		public const int TNS_HEADER = 758;
		/// <summary>
		/// Item Presentation
		/// </summary>
		public const int PRESENTATION = 757;
	}
	#endregion

    #region Country Code
    /// <summary>
    /// Constantes concerning country code
    /// </summary>
    public class CountryCode
    {
        /// <summary>
        /// Country Code For France
        /// </summary>
        public const string FRANCE = "33";
        /// <summary>
        /// Country Code For Finland
        /// </summary>
        public const string FINLAND = "35";
        /// <summary>
        /// Country Code For Czech Republic
        /// </summary>
        public const string CZECH_REPUBLIC = "420";
        /// <summary>
        /// Country Code For Poland
        /// </summary>
        public const string POLAND = "48";

    }
    #endregion

	#region D�tail s�lection
	/// <summary>
	/// Constantes pour le rappel de la s�lection
	/// </summary>
	public class DetailSelection {
		/// <summary>
		/// Type d'�l�ments du rappel de la s�lection
		/// </summary>
		public enum Type{
			/// <summary>
			/// Nom du module
			/// </summary>
			moduleName=1,
			/// <summary>
			/// Nom du r�sultat
			/// </summary>
			resultName=2,
			/// <summary>
			/// Date s�lectionn�e
			/// </summary>
			dateSelected=3,
			/// <summary>
			/// Unit� s�lectionn�e
			/// </summary>
			unitSelected=4,
			/// <summary>
			/// Media s�lectionn�s
			/// </summary>
			vehicleSelected=5,
			/// <summary>
			/// Produit s�lectionn�
			/// </summary>
			productSelected=6,
			/// <summary>
			/// Niveau de d�tail support
			/// </summary>
			mediaLevelDetail=7,
			/// <summary>
			/// Niveau de d�tail produit
			/// </summary>
			productLevelDetail=8,
			/// <summary>
			/// Support s�lectionn�s pour les concurrents
			/// </summary>
			competitorMediaSelected=9,
			/// <summary>
			/// Selection en encart (Presse)
			/// </summary>
			insetSelected=10,
			/// <summary>
			/// Selection nouveau dans la pige/support
			/// </summary>
			newInMedia=11,
			/// <summary>
			/// Etude comparative
			/// </summary>
			comparativeStudy=12,
			/// <summary>
			/// Format s�lectionn�
			/// </summary>
			formatSelected=13,
			/// <summary>
			/// Jour nomm� s�lectionn�
			/// </summary>
			daySelected=14,
			/// <summary>
			/// Tranche horaire s�lectionn�e
			/// </summary>
			timeSlotSelected=15,
			/// <summary>
			/// Vague s�lectionn�e
			/// </summary>
			waveSelected=16,
			/// <summary>
			/// Cible s�lectionn�e
			/// </summary>
			targetSelected=17,
			/// <summary>
			/// Niveau de d�tail support g�n�rique
			/// </summary>
			genericMediaLevelDetail=18,
			/// <summary>
			/// Version s�lectionn�e
			/// </summary>
			sloganSelected=19,
			/// <summary>
			/// Niveau de d�tail produit g�n�rique
			/// </summary>
			genericProductLevelDetail = 20,
			/// <summary>
			/// Genre �missions\ �missions s�lectionn�s 
			/// </summary>
			programTypeSelected = 21,
			/// <summary>
			/// Formes de parrainnage s�lectionn�s
			/// </summary>
			sponsorshipFormSelected = 22,
			/// <summary>
			/// Type de pourcentage s�lectionn�
			/// </summary>
			percentageAlignmentSelected = 23,
            /// <summary>
            /// La date de l'�tude
            /// </summary>
            studyDate = 24,
            /// <summary>
            /// La date de la p�riode comparative
            /// </summary>
            comparativeDate = 25,
            /// <summary>
            /// Le type de la p�riode comparative
            /// </summary>
            comparativePeriodType = 26,
            /// <summary>
            /// Le type de la disponibilit� des donn�es
            /// </summary>
            periodDisponibilityType = 27,
            /// <summary>
            /// Niveau de d�tail colonne g�n�rique
            /// </summary>
            genericColumnLevelDetail = 28,
            /// <summary>
            /// Personnaliser les supports (affiner dans les r�sultats)
            /// </summary>
            mediaPersonnalizedSelected = 29,
            /// <summary>
            /// Evaliant auto promo
            /// </summary>
            isAutoPromo = 30
		}
	}

	#endregion

	#region Personnalisation des annonceurs concurrents et de r�f�rence
	/// <summary>
	/// Constante de la personnalisation des annonceurs
	/// </summary>
	public class AdvertiserPersonalisation
	{
		/// <summary>
		/// type d'annonceur "normal", de reference, concurrent
		/// </summary>
		public enum Type
		{
			/// <summary>
			/// Pas de type
			/// </summary>
			none,
			/// <summary>
			/// R�f�rence
			/// </summary>
			reference,
			/// <summary>
			/// Concurrent
			/// </summary>
			competitor,
			/// <summary>
			/// Reference et Concurrent
			/// </summary>
			mixed
		}
	}
	#endregion

	#region Nombres d'�l�ments s�lectionnables dans la s�lection des Annonceurs
	/// <summary>
	/// Classe des constantes sur les Annonceurs
	/// </summary>
	public class Advertiser
	{
		/// <summary>
		/// Nombre maximun d'annonceurs pouvant �tre s�lectionn�s
		/// </summary>
		public const int MAX_NUMBER_ADVERTISER=999;
	}
	#endregion

	#region Nombre maximum d'univers concurrent pouvant �tre s�lectionn�s
	/// <summary>
	/// Nombre maximum d'univers concurrent pouvant �tre s�lectionn�s
	/// </summary>
	public class CompetitorUnivers
	{
		/// <summary>
		/// Nombre maximun d'annonceurs pouvant �tre s�lectionn�s
		/// </summary>
		public const int MAX_NUMBER_UNIVERS=10;
	}
	#endregion

	#region Taille maximale dans une saisie de nom de requ�te
	/// <summary>
	/// Classe des contantes sur MonAdExpress
	/// </summary>
	public class MySession
	{
		/// <summary>
		/// Nombre de caract�res maximum pour le nom d'un r�sultat
		/// </summary>
		public const int MAX_LENGHT_TEXT=50;
	}
	#endregion

	#region Constantes des insertions et cr�ations
	/// <summary>
	/// Constantes n�cessaires a la construction des tableaux detail insertion m�dia
	/// </summary>
	public class VehicleInsertionsColumnIndex{
		/// <summary>index de la colonne date</summary>
		public const int DATE_INDEX=0;
		/// <summary>index de la colonne annonceur</summary>
		public const int ADVERTISER_INDEX=1;
		/// <summary>index de la colonne produit</summary>
		public const int PRODUCT_INDEX=2;
		/// <summary>index de la colonne groupe</summary>
		public const int GROUP_INDEX=3;
	}
	/// <summary>
	/// Constantes n�cessaires a la construction des tableaux detail insertion PRESS
	/// </summary>
	public class PressInsertionsColumnIndex
	{
		/// <summary>index de la colonne date</summary>
		public const int DATE_INDEX=0;
		/// <summary>index de la colonne n� de page</summary>
		public const int MEDIA_PAGING_INDEX=1;
		/// <summary>index de la colonne groupe</summary>
		public const int GROUP_INDEX=2;
		/// <summary>index de la colonne annonceur</summary>
		public const int ADVERTISER_INDEX=3;
		/// <summary>index de la colonne produit</summary>
		public const int PRODUCT_INDEX=4;
		/// <summary>index de la colonne format</summary>
		public const int FORMAT_INDEX=5;
		/// <summary>index de la colonne surface</summary>
		public const int AREA_PAGE_INDEX=6;
		/// <summary>index de la colonne couleur</summary>
		public const int COLOR_INDEX=7;
		/// <summary>index de la colonne prix</summary>
		public const int EXPENDITURE_INDEX=8;
		/// <summary>index de la colonne localisation</summary>
		public const int LOCATION_INDEX=9;
		/// <summary>index de la colonne visuels</summary>
		public const int VISUAL_INDEX=10;
		/// <summary>
		/// Index de la colonne vehicule
		/// </summary>
		public const int VEHICLE_INDEX=11;
		/// <summary>
		/// Index de la colonne cat�gorie
		/// </summary>
		public const int CATEGORY_INDEX=12;
		/// <summary>
		/// Index de la colonne support
		/// </summary>
		public const int MEDIA_INDEX=13;
		/// <summary>
		/// Index de la colonne Identifiant support
		/// </summary>
		public const int ID_MEDIA_INDEX=14;		
		/// <summary>
		/// Index de la colonne Identifiant de centre d'int�r�ts
		/// </summary>
		public const int INTEREST_CENTER_INDEX=15;
		/// <summary>
		/// Index de la colonne Identifiant de r�gies
		/// </summary>
		public const int MEDIA_SELLER_INDEX=16;
		/// <summary>
		/// Index de la colonne Identifiant de l'accroche
		/// </summary>
		public const int ID_SLOGAN_INDEX=17;

	}

    /// <summary>
	/// Constantes n�cessaires a la construction des tableaux des versions Mraketing Direct
	/// </summary>
    public class MDVersionsColumnIndex{

        /// <summary>
        /// Index de la colonne cat�gorie
        /// </summary>
        public const int  CATEGORY_INDEX=0;
        /// <summary>
        /// Index de la colonne id cat�gorie
        /// </summary>
        public const int ID_MEDIA_INDEX = 1;
        /// <summary>
        /// Index de la colonne support
        /// </summary>
        public const int MEDIA_INDEX = 2;
        /// <summary>
        /// Index de la colonne date
        /// </summary>
        public const int DATE_INDEX = 3;
        /// <summary>
        /// Index de la colonne annonceur
        /// </summary>
        public const int ADVERTISER_INDEX = 4;
		/// <summary>
		/// Index de la colonne mail content
		/// </summary>
		public const int MAIL_CONTENT_INDEX = 5;
        /// <summary>
        /// Index de la colonne groupe
        /// </summary>
        public const int GROUP_INDEX = 6;
        /// <summary>
        /// Index de la colonne poids
        /// </summary>
        public const int WEIGHT_INDEX = 7;
        /// <summary>
        /// Index de la colonne visuel
        /// </summary>
        public const int ASSOCIATED_FILE_INDEX = 8;
        /// <summary>
        /// Index de la colonne investissement
        /// </summary>
        public const int EXPENDITURE_INDEX = 9;
        /// <summary>
        /// Index de la colonne volume
        /// </summary>
        public const int VOLUME_INDEX = 10;
        /// <summary>
        /// Index de la colonne version
        /// </summary>
        public const int SLOGAN_INDEX = 11;
        /// <summary>
        /// Index de la colonne format
        /// </summary>
        public const int FORMAT_INDEX = 12;
        /// <summary>
        /// Index de la colonne mail format
        /// </summary>
        public const int MAIL_FORMAT_INDEX = 13;
        /// <summary>
        /// Index de la colonne mail type
        /// </summary>
        public const int MAIL_TYPE_INDEX = 14;
        /// <summary>
        /// Index de la colonne mail format de la table WEB_PLAN_MEDIA_x
        /// </summary>
        public const int WP_MAIL_FORMAT_INDEX = 15;
        /// <summary>
        /// Index de la colonne nombre d'items
        /// </summary>
        public const int OBJECT_COUNT_INDEX = 16;
        /// <summary>
        /// Index de la colonne mailing rapidity
        /// </summary>
        public const int MAILING_RAPIDITY_INDEX = 17;       
		/// <summary>
		/// Index de la colonne produit
		/// </summary>
		public const int PRODUCT_INDEX = 18;
               
    }
    
	/// <summary>
	/// Constantes n�cessaires a la construction des tableaux detail insertion RADIO
	/// </summary>
	public class RadioInsertionsColumnIndex : VehicleInsertionsColumnIndex
	{
		
		/// <summary>index de la colonne date de diffusion</summary>
		public const int TOP_DIFFUSION_INDEX=4;
		/// <summary>index de la colonne dur�e</summary>
		public const int DURATION_INDEX=5;
		/// <summary>index de la colonne rang</summary>
		public const int RANK_INDEX=6;
		/// <summary>index de la colonne dir�e de la page publicitaire</summary>
		public const int BREAK_DURATION_INDEX=7;
		/// <summary>index de la colonne nb total de spots</summary>
		public const int BREAK_SPOTS_NB_INDEX=8;
		/// <summary>index de la colonne rang hap</summary>
		public const int RANK_WAP_INDEX=9;
		/// <summary>index de la colonne dir�e hap</summary>
		public const int DURATION_BREAK_WAP_INDEX=10;
		/// <summary>index de la colonne nbspots hap</summary>
		public const int BREAK_SPOTS_WAP_NB_INDEX=11;
		/// <summary>index de la colonne prix</summary>
		public const int EXPENDITURE_INDEX=12;
		/// <summary>index de la colonne vehicule</summary>
		public const int VEHICLE_INDEX=13;
		/// <summary>index de la colonne category</summary>
		public const int CATEGORY_INDEX=14;
		/// <summary>index de la colonne media</summary>
		public const int MEDIA_INDEX=15;
		/// <summary>index de la colonne fichiers</summary>
		public const int FILE_INDEX=16;		
		/// <summary>
		/// Index de la colonne Identifiant de centre d'int�r�ts
		/// </summary>
		public const int INTEREST_CENTER_INDEX=17;
		/// <summary>
		/// Index de la colonne Identifiant de r�gies
		/// </summary>
		public const int MEDIA_SELLER_INDEX=18;
		/// <summary>
		/// Index de la colonne Identifiant de l'accroche
		/// </summary>
		public const int ID_SLOGAN_INDEX=19;
	}

	/// <summary>
	/// Constantes n�cessaires a la construction des tableaux detail insertion TV
	/// </summary>
	public class TVInsertionsColumnIndex : VehicleInsertionsColumnIndex
	{
		
		/// <summary>index de la colonne heure de diffusion</summary>
		public const int TOP_DIFFUSION_INDEX=4;
		/// <summary>
		/// Index de la colonne de code �cran
		/// </summary>
		public const int ID_COMMERCIAL_BREAK_INDEX=5;
		/// <summary>index de la colonne duree</summary>
		public const int DURATION_INDEX=6;
		/// <summary>index de la colonne rang</summary>
		public const int RANK_INDEX=7;
		/// <summary>index de la colonne dur�e de tout les spots</summary>
		public const int BREAK_DURATION_INDEX=8;
		/// <summary>index de la colonne nb totazl de spots</summary>
		public const int BREAK_SPOTS_NB_INDEX=9;
		/// <summary>index de la colonne prix</summary>
		public const int EXPENDITURE_INDEX=10;
		/// <summary>index de la colonne fichiers visuels</summary>
		public const int FILES_INDEX=11;
		/// <summary>index de la colonne vehicule</summary>
		public const int VEHICLE_INDEX=12;
		/// <summary>index de la colonne category</summary>
		public const int CATEGORY_INDEX=13;
		/// <summary>index de la colonne media</summary>
		public const int MEDIA_INDEX=14;		
		/// <summary>
		/// Index de la colonne Identifiant de centre d'int�r�ts
		/// </summary>
		public const int INTEREST_CENTER_INDEX=15;
		/// <summary>
		/// Index de la colonne Identifiant de r�gies
		/// </summary>
		public const int MEDIA_SELLER_INDEX=16;
		/// <summary>
		/// Index de la colonne Identifiant de l'accroche
		/// </summary>
		public const int ID_SLOGAN_INDEX=17;
	}

	/// <summary>
	/// OutDoorInsertionsColumnIndex
	/// </summary>
	public class OutDoorInsertionsColumnIndex : VehicleInsertionsColumnIndex {
		/// <summary>
		/// index de la colonne nombre de panneaux
		/// </summary>
		public const int NUMBER_BOARD_INDEX=4;
		/// <summary>
		/// index de la colonne format
		/// </summary>
		public const int TYPE_BOARD_INDEX=5;
		/// <summary>
		/// index de la colonne type de r�seaux
		/// </summary>
		public const int TYPE_SALE_INDEX=6;
		/// <summary>
		/// index de la colonne r�seau afficheur
		/// </summary>
		public const int POSTER_NETWORK_INDEX=7;
		/// <summary>index de la colonne prix</summary>
		public const int EXPENDITURE_INDEX=8;		
		/// <summary>index de la colonne vehicule</summary>
		public const int VEHICLE_INDEX=9;
		/// <summary>index de la colonne category</summary>
		public const int CATEGORY_INDEX=10;
		/// <summary>index de la colonne media</summary>
		public const int MEDIA_INDEX=11;
		/// <summary>index de la colonne fichiers </summary>
		public const int FILES_INDEX=12;
		/// <summary>index de la colonne agglomeration </summary>
		public const int AGGLOMERATION_INDEX=13;
		/// <summary>
		/// Index de la colonne Identifiant de centre d'int�r�ts
		/// </summary>
		public const int INTEREST_CENTER_INDEX=14;
		/// <summary>
		/// Index de la colonne Identifiant de r�gies
		/// </summary>
		public const int MEDIA_SELLER_INDEX=15;
		/// <summary>
		/// Index de la colonne Identifiant de l'accroche
		/// </summary>
		public const int ID_SLOGAN_INDEX = 16;
	}
    /// <summary>
    /// OutDoorInsertionsColumnIndex
    /// </summary>
    public class InStoreInsertionsColumnIndex : OutDoorInsertionsColumnIndex {
    }
	#endregion

    #region chemins d'acces aux creations (presse, radio, tv)
    /// <summary>
    /// Classe des serveurs d'acc�s aux cr�ations
    /// </summary>
    public class CreationServerPathes
    {
        /// <summary>
        /// R�pertoire virtuel des cr�ations presse
        /// </summary>
        public static string IMAGES = string.Empty;
        /// <summary>
        /// R�pertoire virtuel des cr�ations adnettrack
        /// </summary>
        public static string CREA_ADNETTRACK = string.Empty;
        /// <summary>
        /// R�pertoire virtuel des cr�ations adnettrack
        /// </summary>
        public static string CREA_EVALIANT_MOBILE = string.Empty;
        /// <summary>
        /// R�pertoire virtuel des cr�ations marketing direct
        /// </summary>
        public static string IMAGES_MD = string.Empty;
        /// <summary>
        /// R�pertoire virtuel des cr�ations de la publicit� ext�rieure
        /// </summary>
        public static string IMAGES_OUTDOOR = string.Empty;
        /// <summary>
        /// R�pertoire virtuel des cr�ations de la publicit� INTERRIEUR
        /// </summary>
        public static string IMAGES_INSTORE = string.Empty;
        /// <summary>
        /// Serveur Real en streaming en radio
        /// </summary>
        public static string READ_REAL_RADIO_SERVER = string.Empty;
        /// <summary>
        /// Serveur Real en streaming en radio (nouveau chemin)
        /// </summary>
        public static string READ_REAL_CREATIVES_RADIO_SERVER = string.Empty;
        /// <summary>
        /// Serveur WMA en streaming en radio
        /// </summary>
        public static string READ_WM_RADIO_SERVER = string.Empty;
        /// <summary>
        /// Serveur WMA en streaming en radio (nouveau chemin)
        /// </summary>
        public static string READ_WM_CREATIVES_RADIO_SERVER = string.Empty;
        /// <summary>
        /// R�pertoire des spots radio
        /// </summary>
        public static string DOWNLOAD_RADIO_SERVER = string.Empty;
        /// <summary>
        /// R�pertoire des spots radio
        /// </summary>
        public static string DOWNLOAD_CREATIVES_RADIO_SERVER = string.Empty;
        /// <summary>
        /// Serveur WMV en streaming en TV
        /// </summary>
        public static string READ_WM_TV_SERVER = string.Empty;
        /// <summary>
        /// Serveur WMV en streaming en Pan euro
        /// </summary>
        public static string READ_WM_PAN_EURO_SERVER = string.Empty;
        /// <summary>
        /// Serveur Real en streaming en TV
        /// </summary>
        public static string READ_REAL_TV_SERVER = string.Empty;
        /// <summary>
        /// R�pertoire des spots tv
        /// </summary>
        public static string DOWNLOAD_TV_SERVER = string.Empty;
        /// <summary>
        /// R�pertoire des PanEuro
        /// </summary>
        public static string DOWNLOAD_PAN_EURO = string.Empty;
        /// <summary>
        /// R�pertoire pour t�l�charger les spots radio
        /// </summary>
        //public static string LOCAL_PATH_RADIO=@"\\wallis\radio\Spot_Quanti\";
        public static string LOCAL_PATH_RADIO = string.Empty;
        /// <summary>
        /// Nouveau R�pertoire pour t�l�charger les spots radio
        /// </summary>		
        public static string LOCAL_PATH_CREATIVES_RADIO = string.Empty;
        /// <summary>
        /// R�pertoire pour t�l�charger les spots tv
        /// </summary>
        //public static string LOCAL_PATH_VIDEO = @"\\stream_rescue\stream\fr";
        public static string LOCAL_PATH_VIDEO = string.Empty;
        /// <summary>
        /// R�pertoire pour t�l�charger les paneuro
        /// </summary>
        public static string LOCAL_PATH_PAN_EURO = string.Empty;
        /// <summary>
        /// R�pertoire contenant les visuels de la presse
        /// </summary>
        public static string LOCAL_PATH_IMAGE = "\\\\frmitch-fs03/quanti_multimedia_perf/AdexDatas/Press/SCANS/";
        /// <summary>
        /// R�pertoire contenant les visuels du Marketing Direct
        /// </summary>
        public static string LOCAL_PATH_MD_IMAGE = string.Empty;
        /// <summary>
        /// R�pertoire contenant les logos de la publicit� ext�rieur
        /// </summary>
        public static string LOCAL_PATH_OUTDOOR = string.Empty;
        /// <summary>
        /// R�pertoire contenant les logos de la publicit� interrieur
        /// </summary>
        public static string LOCAL_PATH_INSTORE = string.Empty;
        /// <summary>
        /// Premi�re page
        /// </summary>	
        public static string COUVERTURE = "COE001.JPG";
        /// <summary>
        /// R�pertoire contenant les logos de la presse
        /// </summary>
        public static string LOCAL_PATH_LOGO_PRESS = string.Empty;
        /// <summary>
        /// R�pertoire contenant les logos de la presse
        /// </summary>
        public static string LOGO_PRESS = string.Empty;


    }
    #endregion

	#region Universe
	/// <summary>
	/// Classe de constantes des univers
	/// </summary>
	public class Universe{
		/// <summary>
		/// Action au niveau des univers
		/// </summary>
		public enum Type{
			/// <summary>
			/// Sauvegarde d'un univers
			/// </summary>
			saveUniverse=0,
			/// <summary>
			/// Chargement de l'univers
			/// </summary>
			loadUniverse=1		
		}
	}
	#endregion

	#region Univers d'�tude m�dia d'AdExpress
	/// <summary>
	/// Classe de constantes des univers AdExpress.
	/// Elle contient les identifiants des listes d'univers qui servent � restreindre les
	/// univers d'�tude.
	/// </summary>
	public class AdExpressUniverse{
		/// <summary>
		/// Identifiant de la liste des m�dia pour les alertes et analyses
		/// </summary>
//		public const int ALERTE_MEDIA_LIST_ID=0;
		/// <summary>
		/// Identifiant de la liste des m�dia pour les analyses sectorielles
		/// </summary>
		public const int RECAP_MEDIA_LIST_ID =  341; //1;
		/// <summary>
		/// Liste des produits � exclure dans les alertes et analyse (radio)
		/// </summary>
        public const int EXCLUDE_PRODUCT_LIST_ID = 385;
		/// <summary>
		/// Liste des produits en autopromo pour la radio
		/// </summary>
		public const int AUTOPROMO_RADIO_PRODUCT_LIST_ID=3;
		/// <summary>
		/// Liste des supports presse pour les tendances
		/// </summary>
		//public const int TENDENCY_PRESS_MEDIA_LIST_ID=4;
		/// <summary>
		/// Liste des supports presse pour les tendances
		/// </summary>
		//public const int TENDENCY_RADIO_MEDIA_LIST_ID=5;
		/// <summary>
		/// Liste des supports presse pour les tendances
		/// </summary>
		//public const int TENDENCY_TV_MEDIA_LIST_ID=6;
		/// <summary>
		/// Liste des supports presse pour les tendances
		/// </summary>
		//public const int TENDENCY_PANEURO_MEDIA_LIST_ID=7;
		/// <summary>
		/// Liste des produits � exclure dans le tableau de bord presse
		/// </summary>
		public const int DASHBOARD_PRESS_EXCLUDE_PRODUCT_LIST_ID=387; //8
		/// <summary>
		/// Liste des m�dia pour les pan euro
		/// </summary>
		public const int DASHBOARD_PANEURO_MEDIA_LIST_ID = 340; //9;
		/// <summary>
		/// Liste des m�dia pour le parrainage t�l�
		/// </summary>
		public const int TV_SPONSORINGSHIP_MEDIA_LIST_ID = 339; //10;
		
		/// <summary>
		/// Identifiant de la liste des m�dia par d�faut pour les alertes et analyses
		/// </summary>
		public const int MEDIA_DEFAULT_LIST_ID = 327;
		/// <summary>
		/// Identifiant de la liste des m�dia pour le module alerte plan m�dia
		/// </summary>
		public const int MEDIA_SCHEDULE_ALERT_LIST_ID = 328;
		/// <summary>
		/// Identifiant de la liste des m�dia pour le module alerte plan m�dia concurrentiel
		/// </summary>
		public const int COMPARATIVE_MEDIA_SCHEDULE_ALERT_LIST_ID = 329;
		/// <summary>
		/// Identifiant de la liste des m�dia pour le module analyse plan m�dia concurrentiel
		/// </summary>
		public const int COMPARATIVE_MEDIA_SCHEDULE_ANALYSIS_LIST_ID = 330;
		/// <summary>
		/// Identifiant de la liste des m�dia pour le module analyse plan m�dia 
		/// </summary>
		public const int MEDIA_SCHEDULE_ANALYSIS_LIST_ID = 331;
		/// <summary>
		/// Identifiant de la liste des m�dia pour le module alerte concurrentielle
		/// </summary>
		public const int COMPETITIVE_ALERT_LIST_ID = 332;
		/// <summary>
		/// Identifiant de la liste des m�dia pour le module analyse concurrentielle
		/// </summary>
		public const int COMPETITIVE_REPORTS_LIST_ID = 333;
		/// <summary>
		/// Identifiant de la liste des m�dia pour le module Alerte de potentiels
		/// </summary>
		public const int PROSPECTING_ALERT_LIST_ID = 334;
		/// <summary>
		/// Identifiant de la liste des m�dia pour le module Analyse de potentiels
		/// </summary>
		public const int PROSPECTING_REPORTS_LIST_ID = 335;
		/// <summary>
		/// Identifiant de la liste des m�dia pour le module Anlyse dynamique
		/// </summary>
		public const int TREND_REPORTS_LIST_ID = 336;
		/// <summary>
		/// Identifiant de la liste des m�dia pour le module Analyse portefeuille d'un support
		/// </summary>
		public const int VEHICLE_PORTOFOLIO_LIST_ID = 337;
		/// <summary>
		/// Identifiant de la liste des m�dia pour le module Alerte portefeuille d'un support
		/// </summary>
		public const int VEHICLE_PORTOFOLIO_ALERT_LIST_ID = 338;
		/// <summary>
		/// Identifiant de la liste des m�dia pour le module Tendances d'un support
		/// </summary>
		public const int TRENDS_LIST_ID = 345;
        /// <summary>
        /// Identifiant de la liste des m�dia pour lesquels on utilise la date de kiosque pour acc�der aux visuels
        /// </summary>
        public const int CREATIVES_KIOSQUE_LIST_ID = 365;
	}
	#endregion

	#region Niveau d'univers
	/// <summary>
	/// D�finition des niveaux produit ou support qui peuvent �tre charg�s
	/// </summary>
	public class LoadableUnivers
	{
		/// <summary>
		/// Niveau Groupe de soci�t�-Annonceur 
		/// </summary>
		public const Int64	HOLDING_ADVERTISER=1;
		/// <summary>
		/// Niveau Annonceur-Produit
		/// </summary>
		public const Int64	ADVERTISER_PRODUCT=2;
		/// <summary>
		/// Niveau Produit 
		/// </summary>
		public const Int64	PRODUCT=3;		
		/// <summary>
		/// Niveau Famille-Classe 
		/// </summary>
		public const Int64	SECTOR_SUBSECTOR=4;
		/// <summary>
		/// Niveau Classe-Groupe
		/// </summary>
		public const Int64	SUBSECTOR_GROUP=5;
		/// <summary>
		/// Niveau Groupe-Vari�t� 
		/// </summary>
		public const Int64	GROUP_SEGMENT=6;
		/// <summary>
		/// Niveau Groupe 
		/// </summary>
		public const Int64	GROUP=7;
		/// <summary>
		/// Niveau Categorie-Media 
		/// </summary>
		public const Int64	CATEGORY_MEDIA=8;
		/// <summary>
		/// Niveau Centre d'int�r�ts-Media 
		/// </summary>
		public const Int64	INTEREST_CENTER_MEDIA=9;
		/// <summary>
		/// Niveau Brand_product
		/// </summary>
		public const Int64	BRAND_PRODUCT=10;
		/// <summary>
		/// Niveau Famille
		/// </summary>
		public const Int64	SECTOR=11;
		/// <summary>
		/// Niveau M�dia ,Centre d'int�r�ts, supports 
		/// </summary>
		public const Int64	VEHICLE_INTEREST_CENTER_MEDIA=12; 
		/// <summary>
		/// Niveau Genres �missions-�missions 
		/// </summary>
		public const Int64	PROGRAM_TYPE_PROGRAM=13;  
		/// <summary>
		/// Niveau Forme de parrainages 
		/// </summary>
		public const Int64	SPONSORSHIP_FORM=14;

		/// <summary>
		/// Niveau vari�t� 
		/// </summary>
		public const Int64 SEGMENT = 15;

		/// <summary>
		/// Generic Universe
		/// </summary>
		public const Int64 GENERIC_UNIVERSE = 16;  
	}

	
	#endregion

	#region Constantes Contact par mail
	/// <summary>
	/// Classe des constantes des contactes pour les mail
	/// </summary>
	public class Contact{
		/// <summary>
		/// Mail du contact qui doit recevoir les demandes d'informations
		/// </summary>
		public const string mailRecipient = "corinne.in-albon@tns-global.com";
		
	}
	#endregion

	#region Gestion d'erreur
	/// <summary>
	/// Constantes de gestion d'erreur
	/// </summary>
	public class ErrorManager{

		#region Constantes
		/// <summary>
		/// Chemin du fichier de configuration du mail des erreurs client
		/// </summary>
		public const string CUSTOMER_ERROR_MAIL_FILE=@"ErrorMail.xml";
		/// <summary>
		/// Chemin du fichier de configuration du mail des erreurs du serveur
		/// </summary>
		public const string WEBSERVER_ERROR_MAIL_FILE=@"CustomerErrorMail.xml";
		#endregion

		#region Enum�rateur
		/// <summary>
		/// Type  d'une page de s�lection
		/// </summary>
		public enum selectedUnivers
		{
			/// <summary>
			/// Pas d'erreur sur s�lection
			/// </summary>
			none,
			/// <summary>
			/// Erreur sur s�lection vehicle
			/// </summary>
			vehicle,
			/// <summary>
			/// Erreur sur s�lection de media
			/// </summary>
			media,
			/// <summary>
			/// Erreur sur s�lection de produits
			/// </summary>
			product,
			/// <summary>
			/// Erreur sur s�lection de la p�riode
			/// </summary>
			period,
			/// <summary>
			/// Erreur sur le nombre de groupes de media s�lectionn�s
			/// </summary>
			mediaNumber,
			/// <summary>
			/// Erreur sur s�lection des cibles
			/// </summary>
			target
		}

		/// <summary>
		/// Type de fichier
		/// </summary>
		public enum formatFile
		{
			/// <summary>
			/// excel
			/// </summary>
			excel,
			/// <summary>
			/// html
			/// </summary>
			html
		
		}

		#endregion
	
	}
	#endregion

	#region Infos/News
	/// <summary>
	/// Classe des sous modules d'infos/news
	/// </summary>
	public class ModuleInfosNews
	{
		public enum Directories {
			/// <summary>
			/// adExNews
			/// </summary>
			adExNews = 0,
			/// <summary>
			/// adExReport
			/// </summary>
			adExReport = 1,
			/// <summary>
			/// adExReview
			/// </summary>
			adExReview = 2,
			/// <summary>
			/// novelties
			/// </summary>
			novelties = 3,
			/// <summary>
			/// advertising Expenditure Category
			/// </summary>
			advertisingExpenditureCategory = 4,
			/// <summary>
			/// top 150 Brand Advertisers
			/// </summary>
			top150BrandAdvertisers = 5,
			/// <summary>
			/// top 20 Retailers
			/// </summary>
			top20Retailers = 6,
			/// <summary>
			/// monthly Advertising Expenditure Trends
			/// </summary>
			monthlyAdvertisingExpenditureTrends = 7,
			/// <summary>
			/// advertising Year Event Materials
			/// </summary>
			advertisingYearEventMaterials = 8,
			/// <summary>
			/// Metholodology
			/// </summary>
			Metholodology = 9,
			/// <summary>
			/// module Descriptions
			/// </summary>
			moduleDescriptions = 10,
			/// <summary>
			/// adExReport 360
			/// </summary>
			adExReport360 = 11,
            /// <summary>
            /// Archive
            /// </summary>
            archive=12,
            /// <summary>
            /// Instructions
            /// </summary>
            instructions=13,
            /// <summary>
            /// adExReport 360
            /// </summary>
            otherInfo=14,
            /// <summary>
            /// 2007 back data
            /// </summary>
            backData2007 = 15,
            /// <summary>
            /// 2006 back data
            /// </summary>
            backData2006 = 16,
            /// <summary>
            /// dealer vs. brand advertising
            /// </summary>
            dealerVsBrandAdvertising = 17

		}

		/// <summary>
		/// Sous module ADEXNEWS
		/// </summary>
		public const string ADEXNEWS ="AdExNews";
		/// <summary>
		/// Sous module ADEXREPORT
		/// </summary>
		public const string ADEXREPORT ="AdExReport";
		/// <summary>
		/// Sous module ADEXREVIEW
		/// </summary>
		public const string ADEXREVIEW ="AdExReview";
		/// <summary>
		/// Sous module NOUVEAUTES
		/// </summary>
		public const string NOUVEAUTES ="Nouveautes";
		
		/// <summary>
		/// R�pertoire des fichiers ADEXNEWS
		/// </summary>
		public static string LOCAL_PATH_ADEXNEWS = string.Empty;
		/// <summary>
		/// R�pertoire des fichiers ADEXREPORT
		/// </summary>
        public static string LOCAL_PATH_ADEXREPORT = string.Empty;
		/// <summary>
		/// R�pertoire des fichiers ADEXREVIEW
		/// </summary>
        public static string LOCAL_PATH_ADEXREVIEW = string.Empty;
		/// <summary>
		/// R�pertoire des fichiers NOUVEAUTES
		/// </summary>
        public static string LOCAL_PATH_NOUVEAUTES = string.Empty;
		/// <summary>
		/// R�pertoire des fichiers NOUVEAUTES
		/// </summary>
        public static string LOCAL_PATH_MEDIA_UPDATE = string.Empty;
		
		/// <summary>
		/// Nom du r�pertoire virtuel IIS pour ADEXNEWS
		/// </summary>
		public const string LINK_ADEXNEWS = "/AdExNews/";
		/// <summary>
		/// Nom du r�pertoire virtuel IIS pour ADEXREPORT
		/// </summary>
		public const string LINK_ADEXREPORT = "/AdExReport/";
		/// <summary>
		/// Nom du r�pertoire virtuel IIS pour ADEXREVIEW
		/// </summary>
		public const string LINK_ADEXREVIEW = "/AdExReview/";
		/// <summary>
		/// Nom du r�pertoire virtuel IIS pour NOUVEAUTES
		/// </summary>
		public const string LINK_NOUVEAUTES = "/Nouveautes/";

		/// <summary>
		/// Nombre d'�l�ment � afficher pour ADEXNEWS
		/// </summary>
		public const int NB_ELEMENTS_ADEXNEWS = 12;
		/// <summary>
		/// Nombre d'�l�ment � afficher pour ADEXREPORT
		/// </summary>
		public const int NB_ELEMENTS_ADEXREPORT = 12;
		/// <summary>
		/// Nombre d'�l�ment � afficher pour ADEXREVIEW
		/// </summary>
		public const int NB_ELEMENTS_ADEXREVIEW = 4;
		/// <summary>
		/// Nombre d'�l�ment � afficher pour NOUVEAUTES
		/// </summary>
		public const int NB_ELEMENTS_NOUVEAUTES = 0;
	}
	#endregion

	#region Fichier Resultats Sauvegard�s (Pdf,..)
	/// <summary>
	/// 
	/// </summary>
	public class FilesResults {


		/// <summary>
		/// pdf
		/// </summary>
		public const string PDF="PDF";		
		/// <summary>
		/// Type de fichier utilis�
		/// </summary>
		public enum Type
		{
			/// <summary>
			/// Fichier PDF
			/// </summary>
			pdf,
			/// <summary>
			/// Fichier texte
			/// </summary>
			text,	
			/// <summary>
			/// Fichier excel
			/// </summary>
			excel
		}
		/// <summary>
		/// Nom du r�pertoire virtuel IIS pour pdf
		/// </summary>
		public const string LINK_FILE = "/AdExCustomerFiles/";
		/// <summary>
		/// R�pertoire des fichiers pdf
		/// </summary>
		public const string LOCAL_PATH_PDF = @"\\testarossa2\AdexDatas\AdExCustomerFiles\1084\";
		/// <summary>
		/// Nombre d'�l�ment � afficher pour pdf
		/// </summary>
		public const int NB_ELEMENTS_PDF = 12; //Nombre d'�l�ment max = 12 ???

		//\\testarossa2\AdexDatas\AdExCustomerFiles\1084
	}
	#endregion

	#region Items �l�ments � trier
	/// <summary>
	/// Constantes des �l�ments � trier
	/// </summary>
	public class SortedItems
	{
		
		/// <summary>
		/// euros
		/// </summary>
		public const string Euro="Euro";
		/// <summary>
		/// SOV
		/// </summary>
		public const string SOV="SOV";
		/// <summary>
		/// SOV cumul�
		/// </summary>
		public const string SOV_Cumul="SOV_Cumul";
		/// <summary>
		/// Evolution
		/// </summary>
		public const string Evolution="Evolution";
		/// <summary>
		/// Ecart
		/// </summary>
		public const string Variation="Variation";
		/// <summary>
		/// Rang
		/// </summary>
		public const string Rank="Rank";
		
	}

	#endregion

	#region Constantes r�partition
	/// <summary>
	/// Constantes r�partition
	/// </summary>
	public class Repartition{
		/// <summary>
		/// Code du type de repartition
		/// </summary>
		public enum repartitionCode{
			/// <summary>
			/// Code format
			/// </summary>
			Format = 1,
			/// <summary>
			/// Code jour nomm�
			/// </summary>
			namedDay = 2,
			/// <summary>
			/// Code tranche horaire
			/// </summary>
			timeInterval=3
		}

		#region Format
		/// <summary>
		/// diff�rentes dur�es de spots
		/// </summary>
		public enum Format{
			/// <summary>
			/// total
			/// </summary>
			Total=0,
			/// <summary>
			/// spots 1 � 9 secondes
			/// </summary>
			Spot_1_9=19,
			/// <summary>
			/// spots de 10 secondes
			/// </summary>
			Spot_10 = 10,
			/// <summary>
			/// Spots de 11 � 19 secondes
			/// </summary>
			Spot_11_19 = 1119,
			/// <summary>
			/// Spots de 20 secondes
			/// </summary>
			Spot_20 = 20,
			/// <summary>
			/// Spots 21 � 29 secondes
			/// </summary>
			Spot_21_29 = 2129,
			/// <summary>
			/// Spots de 30 secondes
			/// </summary>
			Spot_30 = 30,
			/// <summary>
			/// Spots de 31 � 45 secondes
			/// </summary>
			Spot_31_45 = 3145,
			/// <summary>
			/// Spots de plus de 45 secondes
			/// </summary>
			Spot_45=45,


		}
		#endregion

		#region Jour Nomm�
		/// <summary>
		/// jours nomm�s
		/// </summary>
		public enum namedDay{
			/// <summary>
			/// Total
			/// </summary>
			Total=0,
			/// <summary>
			/// Lundi
			/// </summary>
			Monday=1,
			/// <summary>
			/// Mardi
			/// </summary>
			Tuesday =2,
			/// <summary>
			/// Mercredi
			/// </summary>
			Wednesdays=3,
			/// <summary>
			/// Jeudi
			/// </summary>
			Thursday=4,
			/// <summary>
			/// Vendredi
			/// </summary>
			Friday=5,
			/// <summary>
			/// Samedi
			/// </summary>
			Saturday=6,
			/// <summary>
			/// Dimanche
			/// </summary>
			Sunday=7,
			/// <summary>
			/// Week end
			/// </summary>
			Week_end = 67,
			/// <summary>
			/// Seamine 5 de jours (ouvr�s)
			/// </summary>
			Week_5_day = 12345,
//			/// <summary>
//			/// Une semaine
//			/// </summary>
//			Week = 7

		}
		#endregion

		#region Tranche Horaire
		/// <summary>
		/// Tranches horaires
		/// </summary>
		public enum timeInterval{
			/// <summary>
			/// total
			/// </summary>
			Total=0,
			/// <summary>
			/// De 5 h � 6h59
			/// </summary>
			Slice_5h_6h59=5659,
			/// <summary>
			/// De 7h � 8h59
			/// </summary>
			Slice_7h_8h59=7859,
			/// <summary>
			/// De 7h 0 12h
			/// </summary>
			Slice_7h_12h=712,
			/// <summary>
			/// De 9h � 12h59
			/// </summary>
			Slice_9h_12h59=91259,
			/// <summary>
			/// De 12 � 14h
			/// </summary>
			Slice_12h_14h=1214,
			/// <summary>
			/// De 13h � 18h59
			/// </summary>
			Slice_13h_18h59=131859,
			/// <summary>
			/// De 14h � 17h
			/// </summary>
			Slice_14h_17h=1417,
			/// <summary>
			/// De 17h � 19h
			/// </summary>
			Slice_17h_19h=1719,
			/// <summary>
			/// De 19h � 22h
			/// </summary>
			Slice_19h_22h=1922,
			/// <summary>
			/// De 19h � 24h
			/// </summary>
			Slice_19h_24h=1924,
			/// <summary>
			/// De 22h � 24h
			/// </summary>
			Slice_22h_24h=2224,
			/// <summary>
			/// De 24h � 7h
			/// </summary>
			Slice_24h_7h=247
		}
		#endregion
	}
	#endregion

	#region Niveaux de d�tail g�n�riques
	/// <summary>
	/// Constantes des niveaux de s�lections g�n�riques
	/// </summary>
	public class GenericDetailLevel{
		/// <summary>
		/// Type du niveau de d�tail
		/// </summary>
		public enum Type{
			/// <summary>
			/// Inconnu
			/// </summary>
			unknown=0,
			/// <summary>
			/// Plan media
			/// </summary>
			mediaSchedule=1,
			/// <summary>
			/// Analyse dynamique, Concurrentielle, potentiel
			/// </summary>
			dynamicCompetitorPotential=2,
			/// <summary>
			/// Analyse des dispositifs
			/// </summary>
			devicesAnalysis = 3,
			/// <summary>
			/// Analyse des programmes
			/// </summary>
			programAnalysis = 4,
			/// <summary>
			/// AdNetTrack Media schedule
			/// </summary>
			adnettrack=5

		}
	
		/// <summary>
		/// Provenance de la s�lection
		/// </summary>
		public enum SelectedFrom{
			/// <summary>
			/// 
			/// </summary>
			unknown=0,
			/// <summary>
			/// Niveaux de d�tail par d�faut
			/// </summary>
			defaultLevels=1,
			/// <summary>
			/// Niveaux de d�tail sauvegard�s
			/// </summary>
			savedLevels=2,
			/// <summary>
			/// Niveaux de d�tail d�finis par le client
			/// </summary>
			customLevels=3
		}
		/// <summary>
		/// Profile du composant Media ou Produit
		/// </summary>
		public enum ComponentProfile{
			/// <summary>
			/// Profile media
			/// </summary>
			/// <remarks>
			/// Le composant se basera sur la variable: _customerWebSession.GenericMediaDetailLevel
			/// </remarks>
			media=1,
			/// <summary>
			/// Profile produit
			/// </summary>
			/// <remarks>
			/// Le composant se basera sur la variable: _customerWebSession.GenericProductDetailLevel 
			/// </remarks>
			product=2,
			/// <summary>
			/// Profile adnettrack
			/// </summary>
			/// <remarks>
			/// Le composant se basera sur la variable: _customerWebSession.GenericAdNetTrackDetailLevel 
			/// </remarks>
			adnettrack=3
		}

		/// <summary>
		/// Droit niveau de d�tail m�dia
		/// </summary>
		public enum MediaDetailLevel{
			/// <summary>
			/// Niveaux de d�tail m�dia par d�faut
			/// </summary>
			defaultMediaDetailLevel,
			/// <summary>
			/// Niveaux de d�tail produit par d�faut
			/// </summary>
			defaultProductDetailLevel,
			/// <summary>
			/// Niveaux de d�tail m�dia authoris�
			/// </summary>
			allowedMediaLevelItem,
			/// <summary>
			/// Niveaux de d�tail produit authoris�
			/// </summary>
			allowedProductLevelItem,
			/// <summary>
			/// Allowed AdNetTrack Level Item
			/// </summary>
			allowedAdNetTrackLevelItem,
			/// <summary>
			/// Default AdNetTrack Level
			/// </summary>
			defaultAdNetTrackDetailLevel
		}
	}
	#endregion

	#region Colonnes g�n�riques
	/// <summary>
	/// Constantes des colonnes g�n�riques
	/// </summary>
	public class GenericColumn{
	
		/// <summary>
		/// Provenance de la s�lection
		/// </summary>
		public enum SelectedFrom{
			/// <summary>
			/// Inconnu
			/// </summary>
			unknown=0,
			/// <summary>
			/// Colonnes par d�faut
			/// </summary>
			defaultColumns=1,
			/// <summary>
			/// Colonnes sauvegard�es
			/// </summary>
			savedColumns=2,
			/// <summary>
			/// Colonnes d�finis par le client
			/// </summary>
			customColumns=3,
			/// <summary>
			/// Changement d'onglet
			/// </summary>
			vehiculeTab=4
		}
		
		/// <summary>
		/// Identifiant colonne m�dia
		/// </summary>
		public const int VEHICLE_COLUMN_ID=1;
		/// <summary>
		/// Identifiant colonne cat�gorie
		/// </summary>
		public const int CATEGORY_COLUMN_ID=2;
		/// <summary>
		/// Identifiant colonne support
		/// </summary>
		public const int MEDIA_COLUMN_ID=3;
		/// <summary>
		/// Identifiant colonne Centre d'int�r�t
		/// </summary>
		public const int INTEREST_CENTER_COLUMN_ID=4;
		/// <summary>
		/// Identifiant colonne R�gie
		/// </summary>
		public const int MEDIA_SELLER_COLUMN_ID=5;
		/// <summary>
		/// Identifiant colonne Accroche
		/// </summary>
		public const int SLOGAN_COLUMN_ID=6;
		/// <summary>
		/// Identifiant colonne annonceur
		/// </summary>
		public const int ADVERTISER_COLUMN_ID=7;
		/// <summary>
		/// Identifiant colonne m�dia
		/// </summary>
		public const int GROUP_COLUMN_ID=8;
		/// <summary>
		/// Identifiant colonne groupe
		/// </summary>
		public const int PRODUCT_COLUMN_ID=9;
		/// <summary>
		/// Identifiant colonne insertion/spot
		/// </summary>
		public const int INSERTION_COLUMN_ID=10;
		/// <summary>
		/// Identifiant colonne format
		/// </summary>
		public const int FORMAT_COLUMN_ID=11;
		/// <summary>
		/// Identifiant colonne surface page
		/// </summary>
		public const int AREA_PAGE_COLUMN_ID=12;		
		/// <summary>
		/// Identifiant colonne couleur
		/// </summary>
		public const int COLOR_COLUMN_ID=13;
		/// <summary>
		/// Identifiant colonne prix
		/// </summary>
		public const int EXPENDITURE_EURO_COLUMN_ID=14;
		/// <summary>
		/// Identifiant colonne descriptif
		/// </summary>
		public const int LOCATION_COLUMN_ID=15;
		/// <summary>
		/// Identifiant colonne TOP diffusion radio
		/// </summary>
		public const int RADIO_TOP_DIFFUSION_COLUMN_ID=16;
		/// <summary>
		/// Identifiant colonne TOP diffusion t�l�
		/// </summary>
		public const int TV_TOP_DIFFUSION_COLUMN_ID_COLUMN_ID=17;
		/// <summary>
		/// Identifiant colonne Dur�e
		/// </summary>
		public const int DURATION_COLUMN_ID=18;
		/// <summary>
		/// Identifiant colonne rang radio
		/// </summary>
		public const int RADIO_RANK_COLUMN_ID=19;

		/// <summary>
		/// Identifiant colonne rang t�l�
		/// </summary>
		public const int TV_RANK_COLUMN_ID=20;
		/// <summary>
		/// Identifiant colonne Dur�e �cran
		/// </summary>
		public const int DURATION_COMMERCIAL_BREAK_COLUMN_ID=21;
		/// <summary>
		/// Identifiant colonne Nombre Spots �cran t�l�vision
		/// </summary>
		public const int NUMBER_MESSAGE_COMMERCIAL_BREAK_COLUMN_ID=22;
		/// <summary>
		/// Identifiant colonne Nombre Spots �cran radio
		/// </summary>
		public const int NUMBER_SPOT_COM_BREAK_COLUMN_ID=23;
		/// <summary>
		/// Identifiant colonne Position hap
		/// </summary>
		public const int RANK_WAP_COLUMN_ID=24;
		/// <summary>
		/// Identifiant colonne Dur�e �cran hap
		/// </summary>
		public const int DURATION_COM_BREAK_WAP_COLUMN_ID=25;
		/// <summary>
		/// Identifiant colonne Nombre spots hap
		/// </summary>
		public const int NUMBER_SPOT_COM_BEAK_WAP_COLUMN_ID=26;
		/// <summary>
		/// Identifiant colonne Code �cran
		/// </summary>
		public const int COMMERCIAL_BREAK_COLUMN_ID=27;
		/// <summary>
		/// Identifiant colonne Nombre de panneaux
		/// </summary>
		public const int NUMBER_BOARD_COLUMN_ID=28;
		/// <summary>
		/// Identifiant colonne Format du panneau
		/// </summary>
		public const int TYPE_BOARD_COLUMN_ID=29;
		/// <summary>
		/// Identifiant colonne Type de r�seau
		/// </summary>
		public const int TYPE_SALE_COLUMN_ID=30;
		/// <summary>
		/// Identifiant colonne R�seau afficheur
		/// </summary>
		public const int POSTER_NETWORK_COLUMN_ID=31;
		/// <summary>
		/// Identifiant colonne Agglomeration
		/// </summary>
		public const int AGGLOMERATION_COLUMN_ID=32;
		/// <summary>
		/// Identifiant colonne DATE
		/// </summary>
		public const int DATE_MEDIA_NUM_COLUMN_ID=33;
		/// <summary>
		/// Identifiant Page
		/// </summary>
		public const int MEDIA_PAGING_COLUMN_ID=34;
		/// <summary>
		/// Identifiant Visuel
		/// </summary>
		public const int VISUAL_COLUMN_ID=35;
		


	}
	#endregion

	#region Constantes Type de pourcentage
	/// <summary>
	/// Constantes  des pourcentages
	/// </summary>
	public class Percentage{
		/// <summary>
		/// Alignement du pourcentage
		/// </summary>
		public enum Alignment{
			/// <summary>
			/// Aucun
			/// </summary>
			none,
			/// <summary>
			/// Pourcentage horizontal
			/// </summary>
			horizontal,
			/// <summary>
			/// Pourcentage vertical
			/// </summary>
			vertical
		}
	}

	#endregion

    #region Global Calendar Constantes
    /// <summary>
    /// Les contantes du composant globalCalendar
    /// </summary>
    public class globalCalendar {

        /// <summary>
        /// Type de la p�riode comparative
        /// </summary>
        public enum comparativePeriodType { 
            /// <summary>
            /// Date semaine comparative
            /// </summary>
            comparativeWeekDate=0,
            /// <summary>
            /// Date � date
            /// </summary>
            dateToDate=1,
            /// <summary>
            /// Manuelle
            /// </summary>
            manual=2
        }
        /// <summary>
        /// Type de la disponibilit� des donn�es (jour courant ou derni�re p�riode compl�te
        /// </summary>
        public enum periodDisponibilityType { 
            /// <summary>
            /// Jour courant
            /// </summary>
            currentDay=0,
            /// <summary>
            /// Derni�re p�riode compl�te
            /// </summary>
            lastCompletePeriod=1
        }
    }
    #endregion

    #region Images
    /// <summary>
	/// Constantes  des images
	/// </summary>
	public class Images {
		/// <summary>
		/// Image logo TNS
		/// </summary>
		public const string LOGO_TNS = "/Images/common/logo_Tns.gif";

		/// <summary>
		/// Image logo TNS
		/// </summary>
		public const string LOGO_TNS_2 = "/Images/common/logo_Tns_2.gif";

		/// <summary>
		/// Image logo APPM
		/// </summary>
		public const string LOGO_APPM = "/Images/common/logo_Appm.gif";
	}
	#endregion

	#region Lists management
	/// <summary>
	///groups lists type
	/// </summary>
	public class GroupList {
		/// <summary>
		/// Action au niveau des univers
		/// </summary>
		public enum ID {
			/// <summary>
			/// inset group of list
			/// </summary>
			inset = 0,
			/// <summary>
			/// category group of list
			/// </summary>
			category = 1,
			/// <summary>
			/// media group of list
			/// </summary>
			media = 2
		}
		/// <summary>
		/// Category of vehicle list
		/// </summary>
		public enum Type {
			/// <summary>
			/// ID encart
			/// </summary>
			encart=0,
			/// <summary>
			/// ID flying encart
			/// </summary>
			flyingEncart=1,
			/// <summary>
			/// ID excart
			/// </summary>
			Excart=2,
			/// <summary>
			/// List thematic TV
			/// </summary>
			thematicTv = 3,
			/// <summary>
			/// List digital TV 
			/// </summary>
			digitalTv = 4,
			/// <summary>
			/// product Class Analysis Sponsor ship TV
			/// </summary>
			productClassAnalysisSponsorShipTv = 5,
			/// <summary>
			/// ID subscriber encart
			/// </summary>			
			subScriberEncart=6,
			/// <summary>
			/// ID media in "Select All" option on selection page
			/// </summary>			
            mediaInSelectAll = 7,

			/// <summary>
			/// ID media for VMC Rights in recap
			/// <remarks>Used to force VMC rights in Recap</remarks>
			/// </summary>			
			forceVmcRightsInRecap = 8,
			/// <summary>
			/// ID media to force exclusion of Sponsorship TNT
			/// </summary>			
			excludeDigitalSponsorship = 9


		}
	}
	#endregion

	#region Common Layers
	/// <summary>
	/// contains all constantes to identify the common web site layers
	/// </summary>
	public class Layers {
		public enum Id {
			/// <summary>
			/// Classification layer Id
			/// </summary>
			classification = 0,
			/// <summary>
			/// Utilities layer Id
			/// </summary>
			utilities = 1,
			/// <summary>
			/// Dates layer Id
			/// </summary>
			date = 2,
            /// <summary>
			/// Classification level list layer Id
			/// </summary>
			classificationLevelList = 3,
            /// <summary>
            /// Data Access layer id
            /// </summary>
            dataAccess = 4,
            /// <summary>
            /// Date DataAccess
            /// </summary>
            dateDAL = 5,
            /// <summary>
            /// Source provider layer
            /// </summary>
            sourceProvider = 6
		}
	}

	#endregion

}
