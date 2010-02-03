#region Informations
// Auteur: G. Facon 
// Date de cr�ation: 07/06/2004  
// Date de modification: 07/06/2004 
#endregion

using System;
using System.Collections;
using System.Drawing;

namespace TNS.AdExpress.Constantes.FrameWork.Results{

	#region Common
	/// <summary>
	/// CLasse qui peut cntenir des �l�ments de graphismes communs � tous les modules.
	/// </summary>
	public class CommonMother{
		
		#region Constantes des libell�s des branches m�dias
		/// <summary>
		/// List des libell�s de la branche m�dia
		/// </summary>
		public const string MEDIA_LABEL_LIST="VEHICLE,CATEGORY,INTERESTCENTER,MEDIASELLER,MEDIA,SLOGAN";	
		/// <summary>
		/// Libell� slogan
		/// </summary>
		public const string SLOGAN_LABEL="SLOGAN";
		/// <summary>
		/// Libell� (m�dia) VEHICLE
		/// </summary>
		public const string VEHICLE_LABEL="VEHICLE";
		/// <summary>
		/// Libell�  cat�gorie
		/// </summary>
		public const string CATEGORY_LABEL="CATEGORY";
		/// <summary>
		/// Libell� centre d'int�r�t
		/// </summary>
		public const string INTERESTCENTER_LABEL="INTERESTCENTER";
		/// <summary>
		/// Libell� r�gie
		/// </summary>
		public const string MEDIASELLER_LABEL="MEDIASELLER";
		/// <summary>
		/// Libell� support (media)
		/// </summary>
		public const string MEDIA_LABEL="MEDIA";
		#endregion

		/// <summary>
		/// Couleurs disponibles dans Excell
		/// </summary>
		public static Color[] excelColors = new Color[34]{
														Color.FromArgb(153,51,0),
														Color.FromArgb(51,51,0),
														Color.FromArgb(0,51,0),
														Color.FromArgb(0,51,102),
														Color.FromArgb(0,0,128),
														Color.FromArgb(51,51,153),
														Color.FromArgb(128,0,0),
														Color.FromArgb(255,102,0),
														Color.FromArgb(128,128,0),
														Color.FromArgb(0,128,0),
														Color.FromArgb(0,128,128),
														Color.FromArgb(0,0,255),
														Color.FromArgb(102,102,153),
														Color.FromArgb(255,0,0),
														Color.FromArgb(255,153,0),
														Color.FromArgb(153,204,0),
														Color.FromArgb(51,153,102),
														Color.FromArgb(51,204,204),
														Color.FromArgb(51,102,255),
														Color.FromArgb(128,0,128),
														Color.FromArgb(255,0,255),
														Color.FromArgb(255,204,0),
														Color.FromArgb(255,255,0),
														Color.FromArgb(0,255,0),
														Color.FromArgb(0,255,255),
														Color.FromArgb(0,204,255),
														Color.FromArgb(153,51,102),
														Color.FromArgb(255,153,204),
														Color.FromArgb(255,204,153),
														Color.FromArgb(255,255,153),
														Color.FromArgb(204,255,204),
														Color.FromArgb(204,255,255),
														Color.FromArgb(153,204,255),
														Color.FromArgb(204,153,255)
		};
								
	}
	#endregion

	#region Plans m�dia

	/// <summary>
	/// Constantes des plans m�dia
	/// </summary>
	/// 	/// <remarks>
	/// Cette classe est h�rit� par les autres classes des constantes des plans m�dia
	/// </remarks>
	public class MotherMediaPlan:CommonMother{

		#region Enum�rateur
		/// <summary>
		/// 		/// Type des �l�ments graphiques d'un plan m�dia
		/// </summary>
		public enum graphicItemType{
			/// <summary>
			/// Absent
			/// </summary>
			absent,
			/// <summary>
			/// Pr�sent
			/// </summary>
			present,
			/// <summary>
			/// Etendu
			/// </summary>
			extended
		}
		#endregion
		

	
	}
	/// <summary>
	/// Constantes des tableaux du plan m�dia g�n�rique
	/// </summary>
	public class DetailledMediaPlan:MotherMediaPlan{
		#region Constantes
		/// <summary>
		/// Index de la colonne du libell� du niveau 1
		/// </summary>
		public const int L1_COLUMN_INDEX=0;
		/// <summary>
		/// Index de la colonne du libell� du niveau 2
		/// </summary>
		public const int L2_COLUMN_INDEX=1;
		/// <summary>
		/// Index de la colonne du libell� du niveau 3
		/// </summary>
		public const int L3_COLUMN_INDEX=2;
		/// <summary>
		/// Index de la colonne du libell� du niveau 4
		/// </summary>
		public const int L4_COLUMN_INDEX=3;
		/// <summary>
		/// Index de la colonne des p�riodicit� dans le tableau en m�moire
		/// </summary>
		public const int PERIODICITY_COLUMN_INDEX=4;
		/// <summary>
		/// Index de la colonne des totaux dans le tableau en m�moire
		/// </summary>
		public const int TOTAL_COLUMN_INDEX=5;
		/// <summary>
		/// Index de la colonne des pdms dans le tableau en m�moire
		/// </summary>
		public const int PDM_COLUMN_INDEX=6;
		/// <summary>
		/// Index de la colonne du niveau 1
		/// </summary>
		public const int L1_ID_COLUMN_INDEX=7;
		/// <summary>
		/// Index de la colonne du niveau 2
		/// </summary>
		public const int L2_ID_COLUMN_INDEX=8;
		/// <summary>
		/// Index de la colonne du niveau 3
		/// </summary>
		public const int L3_ID_COLUMN_INDEX=9;
		/// <summary>
		/// Index de la colonne du niveau 4
		/// </summary>
		public const int L4_ID_COLUMN_INDEX=10;
		/// <summary>
		/// Index de la colonne de la premi�re p�riode
		/// </summary>
		public const int FIRST_PEDIOD_INDEX=11;
		/// <summary>
		/// ch�ine de la ligne Total
		/// </summary>
		public const string TOTAL_STRING="TOTAL MEDIA";
		/// <summary>
		/// 		/// Index de la ligne des totaux dans le tableau en m�moire
		/// </summary>
		public const int TOTAL_LINE_INDEX=1;
		/// <summary>
		/// Index de la ligne des p�riodes dans le tableau en m�moire
		/// </summary>
		public const int PERIOD_LINE_INDEX=0;
		#endregion
	}


	/// <summary>
	/// Constantes des tableaux du plan m�dia
	/// </summary>
	public class MediaPlan:MotherMediaPlan{

		#region Constantes
		/// <summary>
		/// Index de la colonne des vehicles dans le tableau en m�moire
		/// </summary>
		public const int L1_COLUMN_INDEX=0;
		/// <summary>
		/// Index de la colonne des cat�gories dans le tableau en m�moire
		/// </summary>
		public const int L2_COLUMN_INDEX=1;
		/// <summary>
		/// Index de la colonne des m�dias dans le tableau en m�moire
		/// </summary>
		public const int L3_COLUMN_INDEX=2;
		/// <summary>
		/// Index de la colonne des vehicles dans le tableau en m�moire
		/// </summary>
		public const int VEHICLE_COLUMN_INDEX=0;
		/// <summary>
		/// Index de la colonne des cat�gories dans le tableau en m�moire
		/// </summary>
		public const int CATEGORY_COLUMN_INDEX=1;
		/// <summary>
		/// Index de la colonne des m�dias dans le tableau en m�moire
		/// </summary>
		public const int MEDIA_COLUMN_INDEX=2;
		/// <summary>
		/// Index de la colonne des p�riodicit� dans le tableau en m�moire
		/// </summary>
		public const int PERIODICITY_COLUMN_INDEX=3;
		/// <summary>
		/// Index de la colonne des totaux dans le tableau en m�moire
		/// </summary>
		public const int TOTAL_COLUMN_INDEX=4;
		/// <summary>
		/// Index de la colonne des pdms dans le tableau en m�moire
		/// </summary>
		public const int PDM_COLUMN_INDEX=5;
		/// <summary>
		/// Index de la colonne de la premi�re p�riode
		/// </summary>
		public const int FIRST_PEDIOD_INDEX=6;
		/// <summary>
		/// ch�ine de la ligne Total
		/// </summary>
		public const string TOTAL_STRING="TOTAL MEDIA";
		/// <summary>
		/// Index de la ligne des totaux dans le tableau en m�moire
		/// </summary>
		public const int TOTAL_LINE_INDEX=1;
		
		#endregion
	}

	/// <summary>
	/// Constantes des tableaux de l'alerte plan m�dia
	/// </summary>
	public class MediaPlanAlert:MotherMediaPlan{

		#region Constantes
		/// <summary>
		/// Index de la colonne des vehicles dans le tableau en m�moire
		/// </summary>
		public const int VEHICLE_COLUMN_INDEX=0;
		/// <summary>
		/// Index de la colonne des cat�gories dans le tableau en m�moire
		/// </summary>
		public const int CATEGORY_COLUMN_INDEX=1;
		/// <summary>
		/// Index de la colonne des m�dias dans le tableau en m�moire
		/// </summary>
		public const int MEDIA_COLUMN_INDEX=2;
		/// <summary>
		/// Identifiant du media
		/// </summary>
		public const int ID_MEDIA_COLUMN_INDEX=3;
		/// <summary>
		/// Index de la colonne des p�riodicit� dans le tableau en m�moire
		/// </summary>
		public const int PERIODICITY_COLUMN_INDEX=4;
		/// <summary>
		/// Index de la colonne des totaux dans le tableau en m�moire
		/// </summary>
		public const int TOTAL_COLUMN_INDEX=5;
		/// <summary>
		/// Index de la colonne des pdms dans le tableau en m�moire
		/// </summary>
		public const int PDM_COLUMN_INDEX=6;
		//// <summary>
		//// Index de la colonne Identifiant du vehicle
		//// </summary>
		//public const int ID_VEHICLE_COLUMN_INDEX=7;
		//// <summary>
		//// Index de la colonne Identifiant de la cat�gorie
		//// </summary>
		//public const int ID_CATEGORY_COLUMN_INDEX=8;
		/// <summary>
		/// Index de la colonne de la premi�re p�riode
		/// </summary>
		public const int FIRST_PEDIOD_INDEX=9;
		/// <summary>
		/// ch�ine de la ligne Total
		/// </summary>
		public const string TOTAL_STRING="TOTAL MEDIA";
		/// <summary>
		/// 		/// Index de la ligne des totaux dans le tableau en m�moire
		/// </summary>
		public const int TOTAL_LINE_INDEX=1;
		/// <summary>
		/// Index de la ligne des p�riodes dans le tableau en m�moire
		/// </summary>
		public const int PERIOD_LINE_INDEX=0;
		/// <summary>
		/// Index de la colonne des Identifient vehicule
		/// </summary>
		public const int ID_VEHICLE_COLUMN_INDEX=7;
		/// <summary>
		/// Index de la colonne des Identifiant cat�gorie
		/// </summary>
		public const int ID_CATEGORY_COUMN_INDEX=8;

		#endregion
	}


	/// <summary>
	/// Constantes des tableaux d'un plan m�dia concurentiel
	/// </summary>
	public class CompetitorMediaPlan:MotherMediaPlan{

		#region Constantes
		/// <summary>
		/// Index de la colonne des vehicles dans le tableau en m�moire
		/// </summary>
		public const int VEHICLE_COLUMN_INDEX=0;
		/// <summary>
		/// Index de la colonne des cat�gories dans le tableau en m�moire
		/// </summary>
		public const int CATEGORY_COLUMN_INDEX=1;
		/// <summary>
		/// Index de la colonne des m�dias dans le tableau en m�moire
		/// </summary>
		public const int MEDIA_COLUMN_INDEX=2;
		/// <summary>
		/// Index de la colonne des Advertisers dans le tableau en m�moire
		/// </summary>
		public const int ADVERTISER_COLUMN_INDEX=3;
		/// <summary>
		/// Index de la colonne des Identifiants de vehicles dans le tableau en m�moire
		/// </summary>
		public const int ID_VEHICLE_COLUMN_INDEX = 4;
		/// <summary>
		/// Index de la colonne des p�riodicit� dans le tableau en m�moire
		/// </summary>
		public const int PERIODICITY_COLUMN_INDEX=5;
		/// <summary>
		/// Index de la colonne des totaux dans le tableau en m�moire
		/// </summary>
		public const int TOTAL_COLUMN_INDEX=6;
		/// <summary>
		/// Index de la colonne des pdms dans le tableau en m�moire
		/// </summary>
		public const int PDM_COLUMN_INDEX=7;		
		/// <summary>
		/// Index de la colonne de la premi�re p�riode
		/// </summary>
		public const int FIRST_PEDIOD_INDEX=8;
		/// <summary>
		/// ch�ine de la ligne Total
		/// </summary>
		public const string TOTAL_STRING="TOTAL MEDIA";
		/// <summary>
		/// Index de la ligne des totaux dans le tableau en m�moire
		/// </summary>
		public const int TOTAL_LINE_INDEX=1;
		
		#endregion
	}

	/// <summary>
	/// Constantes des tableaux de l'alerte plan m�dia
	/// </summary>
	public class CompetitorMediaPlanAlert:MotherMediaPlan{

		#region Constantes
		/// <summary>
		/// Index de la colonne des vehicles dans le tableau en m�moire
		/// </summary>
		public const int VEHICLE_COLUMN_INDEX=0;
		/// <summary>
		/// Index de la colonne des cat�gories dans le tableau en m�moire
		/// </summary>
		public const int CATEGORY_COLUMN_INDEX=1;
		/// <summary>
		/// Index de la colonne des m�dias dans le tableau en m�moire
		/// </summary>
		public const int MEDIA_COLUMN_INDEX=2;
		/// <summary>
		/// Identifiant du media
		/// </summary>
		public const int ID_MEDIA_COLUMN_INDEX=3;
		/// <summary>
		/// Index de la colonne des advertiser dans le tableau en m�moire
		/// </summary>
		public const int ADVERTISER_COLUMN_INDEX=4;
		/// <summary>
		/// Identifiant advertiser
		/// </summary>
		public const int ID_ADVERTISER_COLUMN_INDEX=5;
		/// <summary>
		/// Index de la colonne des p�riodicit� dans le tableau en m�moire
		/// </summary>
		public const int PERIODICITY_COLUMN_INDEX=6;
		/// <summary>
		/// Index de la colonne des totaux dans le tableau en m�moire
		/// </summary>
		public const int TOTAL_COLUMN_INDEX=7;
		/// <summary>
		/// Index de la colonne des pdms dans le tableau en m�moire
		/// </summary>
		public const int PDM_COLUMN_INDEX=8;
		//// <summary>
		//// Index de la colonne Identifiant du vehicle
		//// </summary>
		//public const int ID_VEHICLE_COLUMN_INDEX=7;
		//// <summary>
		//// Index de la colonne Identifiant de la cat�gorie
		//// </summary>
		//public const int ID_CATEGORY_COLUMN_INDEX=8;
		/// <summary>
		/// Index de la colonne de la premi�re p�riode
		/// </summary>
		public const int FIRST_PEDIOD_INDEX=11;
		/// <summary>
		/// ch�ine de la ligne Total
		/// </summary>
		public const string TOTAL_STRING="TOTAL MEDIA";
		/// <summary>
		/// Index de la ligne des totaux dans le tableau en m�moire
		/// </summary>
		public const int TOTAL_LINE_INDEX=1;
		/// <summary>
		/// Index de la ligne des p�riodes dans le tableau en m�moire
		/// </summary>
		public const int PERIOD_LINE_INDEX=0;
		/// <summary>
		/// Index de la colonne des Identifient vehicule
		/// </summary>
		public const int ID_VEHICLE_COLUMN_INDEX=9;
		/// <summary>
		/// Index de la colonne des Identifiant cat�gorie
		/// </summary>
		public const int ID_CATEGORY_COlUMN_INDEX=10;

		#endregion
	}

	/// <summary>
	/// Constantes des tableaux de APPM plan m�dia
	/// </summary>
	public class MediaPlanAPPM : MotherMediaPlan{
		#region Constantes
//		/// <summary>
//		/// Index de la colonne des vehicles dans le tableau en m�moire
//		/// </summary>
//		public const int VEHICLE_COLUMN_INDEX=0;
		/// <summary>
		/// Index de la colonne des Identifiant cat�gorie
		/// </summary>
		public const int ID_CATEGORY_COUMN_INDEX=0;
		/// <summary>
		/// Index de la colonne des cat�gories dans le tableau en m�moire
		/// </summary>
		public const int CATEGORY_COLUMN_INDEX=1;
		/// <summary>
		/// Identifiant du media
		/// </summary>
		public const int ID_MEDIA_COLUMN_INDEX=2;
		/// <summary>
		/// Index de la colonne des m�dias dans le tableau en m�moire
		/// </summary>
		public const int MEDIA_COLUMN_INDEX=3;
		/// <summary>
		/// Index de la colonne des p�riodicit� dans le tableau en m�moire
		/// </summary>
		public const int PERIODICITY_COLUMN_INDEX=4;
//		/// <summary>
//		/// Index de la colonne des budgets dans le tableau en m�moire
//		/// </summary>
//		public const int BUDGET_COLUMN_INDEX=5;
//		/// <summary>
//		/// Index de la colonne des pages dans le tableau en m�moire
//		/// </summary>
//		public const int PAGES_COLUMN_INDEX=6;
//		/// <summary>
//		/// Index de la colonne des pages dans le tableau en m�moire
//		/// </summary>
//		public const int GRP_COLUMN_INDEX=7;
//		/// <summary>
//		/// Index de la colonne des Identifient vehicule
//		/// </summary>
//		public const int ID_VEHICLE_COLUMN_INDEX=8;
		/// <summary>
		/// Index de la colonne de la premi�re p�riode
		/// </summary>
		public const int FIRST_PEDIOD_INDEX=5;
		/// <summary>
		/// ch�ine de la ligne Total
		/// </summary>
		public const string TOTAL_STRING="TOTAL MEDIA";
		/// <summary>
		/// Index de la ligne des totaux dans le tableau en m�moire
		/// </summary>
		public const int TOTAL_LINE_INDEX=1;
		/// <summary>
		/// Index de la colonne du total
		/// </summary>
		public const int TOTAL_COUMN_INDEX=1;
		/// <summary>
		/// Index de la colonne de l'identifiant total
		/// </summary>
		public const int ID_TOTAL_COUMN_INDEX=0;
		/// <summary>
		/// Valeur de l'identifiant du total
		/// </summary>
		public const int ID_TOTAL_STRING=-1;


		#region Constantes pour plan m�dias avec versions
		/// <summary>
		/// Index de la colonne du libell� du niveau 1
		/// </summary>
		public const int L1_COLUMN_INDEX=0;
		/// <summary>
		/// Index de la colonne du libell� du niveau 2
		/// </summary>
		public const int L2_COLUMN_INDEX=1;
		/// <summary>
		/// Index de la colonne du libell� du niveau 3
		/// </summary>
		public const int L3_COLUMN_INDEX=2;
		/// <summary>
		/// Index de la colonne du libell� du niveau 4
		/// </summary>
		public const int L4_COLUMN_INDEX=3;
		
		/// <summary>
		/// Index de la colonne des totaux dans le tableau en m�moire
		/// </summary>
		public const int TOTAL_COLUMN_INDEX=5;
		
		/// <summary>
		/// Index de la colonne du niveau 1
		/// </summary>
		public const int L1_ID_COLUMN_INDEX=6;
		/// <summary>
		/// Index de la colonne du niveau 2
		/// </summary>
		public const int L2_ID_COLUMN_INDEX=7;
		/// <summary>
		/// Index de la colonne du niveau 3
		/// </summary>
		public const int L3_ID_COLUMN_INDEX=8;
		/// <summary>
		/// Index de la colonne du niveau 4
		/// </summary>
		public const int L4_ID_COLUMN_INDEX=9;
		
		/// <summary>
		/// Index de la colonne DES LIbell�s des Versions
		/// </summary>
		public const int VERSIONS_COLUMN_INDEX=10;
		/// <summary>
		/// Index de la colonne de la premi�re p�riode
		/// </summary>
		public const int FIRST_PEDIOD_COLUMN_INDEX=11;
				
		/// <summary>
		/// Index de la ligne des p�riodes dans le tableau en m�moire
		/// </summary>
		public const int PERIOD_LINE_INDEX=0;
		#endregion

		
		#endregion
	}

	/// <summary>
	/// Constantes des tableaux des plans m�dias AdNetTrack
	/// </summary>
	public class AdNetTrackMediaSchedule{
		/// <summary>
		/// Type de plan m�dia AdNetTrack
		/// </summary>
		public enum Type{
			/// <summary>
			/// annonceur
			/// </summary>
			advertiser = 0,
			/// <summary>
			/// produit
			/// </summary>
			product = 1,
			/// <summary>
			/// banni�re
			/// </summary>
			visual = 2
		}

		
	}
	#endregion

	#region Analyse concurrentielle
	/// <summary>
	/// Constantes des tableaux d'alertes
	/// </summary>
	public class CompetitorAlert{

		#region Constantes

		/// <summary>
		/// R�sultat Portefeuille
		/// </summary>
		public const int PORTEFEUILLE=0;
		/// <summary>
		/// R�sultat pr�sence commune
		/// </summary>
		public const int COMMON=1;
		/// <summary>
		/// Absents
		/// </summary>
		public const int ABSENT=2;
		/// <summary>
		/// Exclusifs
		/// </summary>
		public const int EXCLUSIF=3;
		/// <summary>
		/// Synth�se
		/// </summary>
		public const int SYNTHESIS=4;

		/// <summary>
		/// Index de la premi�re colonne � traiter
		/// </summary>
		public	const int FIRST_COLUMN_T0_SHOW_INDEX=0;
		/// <summary>
		/// Index du premier Media 
		/// </summary>
		public	const int FIRST_MEDIA_INDEX=8;
		/// <summary>
		/// Index de la ligne du total
		/// </summary>
		public 	const int TOTAL_LINE_INDEX=3;
		/// <summary>
		/// Identifiant de la ligne Total
		/// </summary>
		public 	const Int64 TOTAL_IDENTIFICATION=Int64.MinValue;
		/// <summary>
		/// Index du total
		/// </summary>
		public 	const int TOTAL_INDEX=7;
		/// <summary>
		/// Index de la ligne permettant de diff�rencier les sous groupes
		/// </summary>
		public const int TOTAL_GROUP_LINE_INDEX=0;
		/// <summary>
		/// Index de la ligne permettant de diff�rencier les m�dias
		/// </summary>
		public const int MEDIA_GROUP_LINE_INDEX=1;
		/// <summary>
		/// Index de la premi�re ligne de r�sultat
		/// </summary>
		public	const int FIRST_LINE_RESULT_INDEX=4;
		/// <summary>
		/// Index de la ligne label
		/// </summary>
		public	const int MEDIA_LINE_LABEL_INDEX=2;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N
		/// </summary>
		public	const int IDL1_INDEX=0;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N-1
		/// </summary>
		public	const int IDL2_INDEX=2;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N-3
		/// </summary>
		public	const int IDL3_INDEX=4;
		/// <summary>
		/// Index du label de l'�l�ment N
		/// </summary>
		public	const int LABELL1_INDEX=1;
		/// <summary>
		/// Index du label de l'�l�ment N-1
		/// </summary>
		public	const int LABELL2_INDEX=3;
		/// <summary>
		/// Index du label de l'�l�ment N-2
		/// </summary>
		public	const int LABELL3_INDEX=5;
		/// <summary>
		/// Index de la colonne contenant l'identifiant de l'adresse
		/// </summary>
		public const int ADDRESS_COLUMN_INDEX=6;
		/// <summary>
		/// Num�ro d'univers m�dia sur l'ann�e N
		/// </summary>
		public	const int N_UNIVERSE_POSITION=1;
		/// <summary>
		/// Identifiant de la colonne des niveaux (produit)
		/// </summary>
		public const int LEVEL_HEADER_ID=0;
		/// <summary>
		/// Identifiant de la colonne d'acc�s aux Cr�ation
		/// </summary>
		public const int CREATIVE_HEADER_ID=1;
        /// <summary>
        /// Identifiant de la colonne des insertions
        /// </summary>
        public const int INSERTION_HEADER_ID = 2;
		/// <summary>
		/// Identifiant de la colonne des plans media
		/// </summary>
		public const int MEDIA_SCHEDULE_HEADER_ID=3;
		/// <summary>
		/// Identifiant de la colonne du total des univers
		/// </summary>
		public const int TOTAL_HEADER_ID=4;
		/// <summary>
		/// Identifiant de la colonne du sous total des univers
		/// </summary>
		public const int SUB_TOTAL_HEADER_ID=5;
		/// <summary>
		/// Identifiant de la colonne Pr�sent
		/// </summary>
		public const int PRESENT_HEADER_ID=6;
		/// <summary>
		/// Identifiant de la colonne absent
		/// </summary>
		public const int ABSENT_HEADER_ID=7;
		/// <summary>
		/// Identifiant de la colonne exclusif
		/// </summary>
		public const int EXCLUSIVE_HEADER_ID=8;
		/// <summary>
		/// Identifiant de la colonne Nombre
		/// </summary>
		public const int ITEM_NUMBER_HEADER_ID=9;
		/// <summary>
		/// Identifiant de la colonne Nombre
		/// </summary>
		public const int UNIT_HEADER_ID=10;
		/// <summary>
		/// Identifiant de la colonne Nombre
		/// </summary>
		public const int REFERENCE_MEDIA_HEADER_ID=11;
		/// <summary>
		/// Identifiant de la colonne Nombre
		/// </summary>
		public const int COMPETITOR_MEDIA_HEADER_ID=12;

		/// <summary>
		/// D�but Identifiant d'un groupe
		/// </summary>
		public const int START_ID_GROUP=13;

		#region Constantes Synth�se
		/// <summary>
		/// Index ligne des annonceurs
		/// </summary>
		public const int ADVERTISERS_LINE_INDEX=0;	
		/// <summary>
		/// Index ligne des marques
		/// </summary>
		public const int BRANDS_LINE_INDEX=1;
		/// <summary>
		/// Index ligne des produits
		/// </summary>
		public const int PRODUCTS_LINE_INDEX=2;		
		/// <summary>
		/// Index ligne des familles
		/// </summary>
		public const int SECTORS_LINE_INDEX=3;
		/// <summary>
		/// Index ligne des classes
		/// </summary>
		public const int SUBSECTORS_LINE_INDEX=4;
		/// <summary>
		/// Index ligne des groupes
		/// </summary>
		public const int GROUPS_LINE_INDEX=5;
		/// <summary>
		/// Index ligne des groupes d'agences
		/// </summary>
		public const int AGENCIES_GROUPS_LINE_INDEX=6;
		/// <summary>
		/// Index ligne des agences
		/// </summary>
		public const int AGENCY_LINE_INDEX=7;
		


		/// <summary>
		/// Index colonne libell� de produits 
		/// </summary>
		public const int PRODUCTS_LABEL_COLUMN_INDEX=0;
		/// <summary>
		/// Index colonne nombre de produits communs
		/// </summary>
		public const int NUMBER_COMMON_COLUMN_INDEX=1;
		/// <summary>
		/// Index colonne unit� de supports de r�f�rence communs
		/// </summary>
		public const int MEDIA_REFERENCE_COMMON_COLUMN_INDEX=2;
		/// <summary>
		/// Index colonne unit� de supports concurrents communs
		/// </summary>
		public const int MEDIA_COMPETITOR_COMMON_COLUMN_INDEX=3;
		/// <summary>
		/// Index colonne nombre de supports de r�f�rence absents
		/// </summary>
		public const int NUMBER_ABSENT_COLUMN_INDEX=4;
		/// <summary>
		/// Index colonne unit� de supports de r�f�rence absents
		/// </summary>
		public const int MEDIA_REFERENCE_ABSENT_COLUMN_INDEX=5;
		/// <summary>
		/// Index colonne unit� de supports de concurrents absents
		/// </summary>
		public const int MEDIA_COMPETITOR_ABSENT_COLUMN_INDEX=6;
		/// <summary>
		/// Index colonne nombre de supports de r�f�rence exclusif
		/// </summary>
		public const int NUMBER_EXCLUSIVE_COLUMN_INDEX=7;
		/// <summary>
		/// Index colonne unit� de supports de r�f�rence exclusif
		/// </summary>
		public const int MEDIA_REFERENCE_EXCLUSIVE_COLUMN_INDEX=8;
		/// <summary>
		/// Index colonne unit� de supports de concurrents exclusif
		/// </summary>
		public const int MEDIA_COMPETITOR_EXCLUSIVE_COLUMN_INDEX=9;		

		#endregion

		#endregion
	}
	#endregion

	#region Analyse de Potentiels
	/// <summary>
	/// Constantes des tableaux pour l'alerte, l'analyse de potentiels
	/// </summary>
	public class MarketShare{
	
		#region Constantes
		/// <summary>
		/// R�sultat Portefeuille
		/// </summary>
		public const int PORTEFEUILLE=0;
		/// <summary>
		/// R�sultat pr�sence commune
		/// </summary>
		public const int FORCES=1;
		/// <summary>
		/// Absents
		/// </summary>
		public const int POTENTIELS=2;
		#endregion		
	}
	#endregion

    #region Module concurrentielle & Potentiels
    /// <summary>
    /// Constantes du module r�sultat de la fusion des deux modules concurrentielle et Potentiels
    /// </summary>
    public class CompetitorMarketShare {

        #region Constantes
        /// <summary>
        /// R�sultat Portefeuille
        /// </summary>
        public const int PORTEFEUILLE = 0;
        /// <summary>
        /// R�sultat pr�sence commune
        /// </summary>
        public const int COMMON = 1;
        /// <summary>
        /// Absents
        /// </summary>
        public const int ABSENT = 2;
        /// <summary>
        /// Exclusifs
        /// </summary>
        public const int EXCLUSIF = 3;
        /// <summary>
        /// Forces
        /// </summary>
        public const int FORCES = 4;
        /// <summary>
        /// Potentiels
        /// </summary>
        public const int POTENTIELS = 5;
        /// <summary>
        /// Synth�se
        /// </summary>
        public const int SYNTHESIS = 6;
        #endregion

    }
    #endregion

    #region Analyse dynamic
    /// <summary>
	/// Constantes des tableaux d'alertes
	/// </summary>
	public class DynamicAnalysis{

		#region Constantes

		/// <summary>
		/// R�sultat Portefeuille
		/// </summary>
		public const int PORTEFEUILLE=0;
		/// <summary>
		/// R�sultat des fidl�les
		/// </summary>
		public const int LOYAL=1;
		/// <summary>
		/// R�sultat des fidl�les en baisse
		/// </summary>
		public const int LOYAL_DECLINE=2;
		/// <summary>
		/// R�sultat des fidl�les en d�veloppement
		/// </summary>
		public const int LOYAL_RISE=3;
		/// <summary>
		/// Gagn�s
		/// </summary>
		public const int WON=4;
		/// <summary>
		/// Perdus
		/// </summary>
		public const int LOST=5;
		/// <summary>
		/// Synth�se
		/// </summary>
		public const int SYNTHESIS=6;
	
		/// <summary>
		/// Num�ro d'univers m�dia sur l'ann�e N
		/// </summary>
		public	const int N_UNIVERSE_POSITION=1;
		/// <summary>
		/// Num�ro d'univers m�dia sur l'ann�e N-1
		/// </summary>
		public	const int N1_UNIVERSE_POSITION=2;
		/// <summary>
		/// Num�ro d'univers m�dia sur l'ann�e N-1
		/// </summary>
		public	const int EVOL_UNIVERSE_POSITION=3;
		/// <summary>
		/// Identifiant des colonnes sous total
		/// </summary>
		public	const int SUBTOTAL_ID=5;
		/// <summary>
		/// Identifiant des colonnes des libell�s
		/// </summary>
		public	const int LEVEL_ID=6;
		/// <summary>
		/// Identifiant des colonnes des plans media
		/// </summary>
		public	const int MEDIA_SCHEDULE_ID=7;
		/// <summary>
		/// Identifiant de la colonnes fid�le
		/// </summary>
		public	const int LOYAL_HEADER_ID=8;
		/// <summary>
		/// Identifiant de la colonnes fid�le en baisse
		/// </summary>
		public	const int LOYAL_DECLINE_HEADER_ID=9;
		/// <summary>
		/// Identifiant de la colonnes fid�le en hausse
		/// </summary>
		public	const int LOYAL_RISE_HEADER_ID=10;
		/// <summary>
		/// Identifiant de la colonnes gagn�s
		/// </summary>
		public	const int WON_HEADER_ID=11;
		/// <summary>
		/// Identifiant de la colonnes perdus
		/// </summary>
		public	const int LOST_HEADER_ID=12;
		/// <summary>
		/// Identifiant de la colonnes du nombre d'�l�ments
		/// </summary>
		public	const int ITEM_NUMBER_HEADER_ID=13;
		/// <summary>
		/// Identifiant de la colonnes des unit�s
		/// </summary>
		public	const int UNIT_HEADER_ID=14;

		/// <summary>
		/// Correspond au nombre d'univers +1 (pour les tests dans les boucles)
		/// </summary>
		public	const int NB_UNIVERSES_TEST=4;

		/// <summary>
		/// Index de la premi�re colonne � traiter
		/// </summary>
		public	const int FIRST_COLUMN_T0_SHOW_INDEX=0;
		/// <summary>
		/// Index du premier Media (*)
		/// </summary>
		public	const int FIRST_MEDIA_INDEX=7;
		/// <summary>
		/// Index de la ligne du total
		/// </summary>
		public 	const int TOTAL_LINE_INDEX=3;
		/// <summary>
		/// Identifiant de la ligne Total
		/// </summary>
		public 	const Int64 TOTAL_IDENTIFICATION=Int64.MinValue;
		/// <summary>
		/// Index de la ligne permettant de diff�rencier les sous groupes
		/// </summary>
		public const int TOTAL_GROUP_LINE_INDEX=0;
		/// <summary>
		/// Index de la ligne permettant de diff�rencier les m�dias
		/// </summary>
		public const int MEDIA_GROUP_LINE_INDEX=1;
		/// <summary>
		/// Index de la premi�re ligne de r�sultat
		/// </summary>
		public	const int FIRST_LINE_RESULT_INDEX=4;
		/// <summary>
		/// Index de la ligne label
		/// </summary>
		public	const int MEDIA_LINE_LABEL_INDEX=2;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N
		/// </summary>
		public	const int IDL1_INDEX=0;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N-1
		/// </summary>
		public	const int IDL2_INDEX=2;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N-3
		/// </summary>
		public	const int IDL3_INDEX=4;
		/// <summary>
		/// Index du label de l'�l�ment N
		/// </summary>
		public	const int LABELL1_INDEX=1;
		/// <summary>
		/// Index du label de l'�l�ment N-1
		/// </summary>
		public	const int LABELL2_INDEX=3;
		/// <summary>
		/// Index du label de l'�l�ment N-2
		/// </summary>
		public	const int LABELL3_INDEX=5;
		/// <summary>
		/// Index de la colonne contenant l'identifiant de l'adresse
		/// </summary>
		public const int ADDRESS_COLUMN_INDEX=6;

		#region Constantes Synth�se
		/// <summary>
		/// Index ligne des annonceurs
		/// </summary>
		public const int ADVERTISERS_LINE_INDEX=0;	
		/// <summary>
		/// Index ligne des marques
		/// </summary>
		public const int BRANDS_LINE_INDEX=1;
		/// <summary>
		/// Index ligne des produits
		/// </summary>
		public const int PRODUCTS_LINE_INDEX=2;		
		/// <summary>
		/// Index ligne des familles
		/// </summary>
		public const int SECTORS_LINE_INDEX=3;
		/// <summary>
		/// Index ligne des classes
		/// </summary>
		public const int SUBSECTORS_LINE_INDEX=4;
		/// <summary>
		/// Index ligne des groupes
		/// </summary>
		public const int GROUPS_LINE_INDEX=5;
		/// <summary>
		/// Index ligne des groupes d'agences
		/// </summary>
		public const int AGENCIES_GROUPS_LINE_INDEX=6;
		/// <summary>
		/// Index ligne des agences
		/// </summary>
		public const int AGENCY_LINE_INDEX=7;
		


		/// <summary>
		/// Index colonne libell� de produits 
		/// </summary>
		public const int PRODUCTS_LABEL_COLUMN_INDEX=0;
		/// <summary>
		/// Index colonne nombre de produits fid�les
		/// </summary>
		public const int NUMBER_LOYAL_COLUMN_INDEX=1;
		/// <summary>
		/// Index colonne unit� de produits fid�les ann�e N
		/// </summary>
		public const int N_LOYAL_COLUMN_INDEX=2;
		/// <summary>
		/// Index colonne unit� de produits fid�les  ann�e N-1
		/// </summary>
		public const int N1_LOYAL_COLUMN_INDEX=3;
		/// <summary>
		/// Index colonne nombre de produits en baissefid�les
		/// </summary>
		public const int NUMBER_LOYAL_DECLINE_COLUMN_INDEX=4;
		/// <summary>
		/// Index colonne unit� de produits fid�les en baisse ann�e N
		/// </summary>
		public const int N_LOYAL_DECLINE_COLUMN_INDEX=5;
		/// <summary>
		/// Index colonne unit� de produits fid�les  en baisseann�e N-1
		/// </summary>
		public const int N1_LOYAL_DECLINE_COLUMN_INDEX=6;
		/// <summary>
		/// Index colonne nombre de produits fid�les en d�veloppement
		/// </summary>
		public const int NUMBER_LOYAL_RISE_COLUMN_INDEX=7;
		/// <summary>
		/// Index colonne unit� de produits fid�les en d�veloppement ann�e N
		/// </summary>
		public const int N_LOYAL_RISE_COLUMN_INDEX=8;
		/// <summary>
		/// Index colonne unit� de produits fid�les en d�veloppement ann�e N-1
		/// </summary>
		public const int N1_LOYAL_RISE_COLUMN_INDEX=9;
		/// <summary>
		/// Index colonne nombre de produits gagn�s
		/// </summary>
		public const int NUMBER_WON_COLUMN_INDEX=10;
		/// <summary>
		/// Index colonne unit� de produits gagn�s ann�e N
		/// </summary>
		public const int N_WON_COLUMN_INDEX=11;
		/// <summary>
		/// Index colonne unit� de produits gagn�s  ann�e N-1
		/// </summary>
		public const int N1_WON_COLUMN_INDEX=12;
		/// <summary>
		/// Index colonne nombre de produits perdus
		/// </summary>
		public const int NUMBER_LOST_COLUMN_INDEX=13;
		/// <summary>
		/// Index colonne unit� de produits perdus ann�e N
		/// </summary>
		public const int N_LOST_COLUMN_INDEX=14;
		/// <summary>
		/// Index colonne unit� de produits perdus  ann�e N-1
		/// </summary>
		public const int N1_LOST_COLUMN_INDEX=15;

		#endregion

		#endregion
	}
	#endregion

	#region Recap

	/// <summary>
	/// Classe M�re des R�cap
	/// </summary>
	public class MotherRecap{
		#region Enum�rateurs
		/// <summary>
		/// Type de tableau
		/// </summary>
		public enum ElementType{
			/// <summary>
			/// Affichage des produits
			/// </summary>
			product,
			/// <summary>
			/// Affichage des annonceurs
			/// </summary>
			advertiser		
		}
		#endregion

		/// <summary>
		/// Saisonnalit�
		/// </summary>
		public const int SEASONALITY=0;
		/// <summary>
		/// Palmares
		/// </summary>
		public const int PALMARES=1;		
		/// <summary>
		/// Nouveaut�s
		/// </summary>
		public const int NOVELTY=2;		
		
		/// <summary>
		/// Strat�gie M�dia
		/// </summary>
		public const int MEDIA_STRATEGY=4;

		/// <summary>
		/// Identifiant planche synth�se
		/// </summary>
		public const int SYNTHESIS=5;
	
	}


	#region Palmar�s
	/// <summary>
	/// Partie palmares des Recap
	/// </summary>
	public class PalmaresRecap:MotherRecap{

		#region Enum�rateurs
		/// <summary>
		/// Ann�e de r�f�rence
		/// </summary>
		public enum typeYearSelected{
			/// <summary>
			/// Ann�e courante
			/// </summary>
			currentYear,
			/// <summary>
			/// Ann�e pr�c�dente
			/// </summary>
			previousYear
		}
		#endregion

		#region Constantes
		/// <summary>
		/// partie tableau
		/// </summary>
		public const int TABLE=0;
		/// <summary>
		/// partie graphique
		/// </summary>
		public const int CHART=1;
//		/// <summary>
//		/// Saisonnalit�
//		/// </summary>
//		public const int SEASONALITY=0;
//		/// <summary>
//		/// Palmares
//		/// </summary>
//		public const int PALMARES=1;		
//		/// <summary>
//		/// Nouveaut�s
//		/// </summary>
//		public const int NOVELTY=2;		
		/// <summary>
		/// Evolution
		/// </summary>
		public const int EVOLUTION=3;
//		/// <summary>
//		/// Strat�gie M�dia
//		/// </summary>
//		public const int MEDIA_STRATEGY=4;
		/// <summary>
		/// Index de la colonne id_produit
		/// </summary>
		public const int ID_PRODUCT_INDEX=0;
		/// <summary>
		/// Index de la colonne produit
		/// </summary>
		public const int PRODUCT=1;
		/// <summary>
		/// Index de la colonne total
		/// </summary>
		public const int TOTAL_N=2;
		/// <summary>
		/// Index de la colonne SOV
		/// </summary>
		public const int SOV=3;
		/// <summary>
		/// Index de la colonne cumul sov
		/// </summary>
		public const int CUMUL_SOV=4;
		/// <summary>
		/// Index de la colonne rang
		/// </summary>
		public const int RANK=5;
		/// <summary>
		/// Index de la colonne 
		/// </summary>
		public const int PROGRESS_RANK=6;
		/// <summary>
		/// Index de la colonne total N-1
		/// </summary>
		public const int TOTAL_N1=7;
		/// <summary>
		/// Index permettant de savoir si l'on est en pr�sence d'un �l�ment concurent
		/// </summary>
		public const int COMPETITOR=8;
		#endregion
	
	}
	#endregion

	#region Evolution
	/// <summary>
	/// Partie Evolution des Recap
	/// </summary>
	public class EvolutionRecap:MotherRecap{

		#region Constantes
		/// <summary>
		/// Index de la colonne id_produit
		/// </summary>
		public const int ID_PRODUCT_INDEX=0;
		/// <summary>
		/// Index de la colonne produit
		/// </summary>
		public const int PRODUCT=1;
		/// <summary>
		/// Index de la colonne total
		/// </summary>
		public const int TOTAL_N=2;
		/// <summary>
		/// Index de la colonne Evolution
		/// </summary>
		public const int EVOLUTION=3;		
		/// <summary>
		/// Index de la colonne ECART
		/// </summary>
		public const int ECART=4;	
		/// <summary>
		/// Index permettant de savoir si l'on est en pr�sence d'un �l�ment concurent
		/// </summary>
		public const int COMPETITOR=5;
		#endregion
	
	}
	#endregion

	#region  Saisonnalit�
	/// <summary>
	/// Partie saisonnalit� des indicateurs
	/// </summary>
	public class Seasonality:MotherRecap{

		#region constantes pr�sentation tableau
		/// <summary>
		///Colonne des MOIS (libell�s)
		/// </summary>
		public const Int64 MONTH_COLUMN_INDEX=0;
		/// <summary>
		///Colonne des MOIS (identifiant)
		/// </summary>
		public const Int64 VALUE_MONTH_COLUMN_INDEX=1;
		/// <summary>
		/// Colonne des MOIS Comparatifs(libell�s)
		/// </summary>
		public const Int64 COMPAR_MONTH_COLUMN_INDEX=2;
		/// <summary>
		/// Colonne des MOIS Comparatifs(identifiant)
		/// </summary>
		public const Int64 COMPAR_VALUE_MONTH_COLUMN_INDEX=3;
		/// <summary>
		/// Colonne Libelle annonceur
		/// </summary>
		public const Int64 ADVERTISER_COLUMN_INDEX=4;
		/// <summary>
		/// Colonne id annonceur
		/// </summary>
		public const Int64 ID_ADVERTISER_COLUMN_INDEX=5;
		/// <summary>
		/// Index colonne des investissements
		/// /// </summary>
		public const Int64 INVESTMENT_COLUMN_INDEX=6;
		/// <summary>
		/// Index colonne des investissements (option etude comparative)
		/// </summary>
		public const Int64 COMPAR_INVESTMENT_COLUMN_INDEX=7;
		/// <summary>
		/// Colonne evolution 
		/// </summary>
		public const Int64 EVOLUTION_COLUMN_INDEX=8;
		/// <summary>
		/// Colonne nombre de r�f�rences
		/// </summary>
		public const Int64 REFERENCE_COLUMN_INDEX=9;
		/// <summary>
		/// Colonne budget moyen
		/// </summary>
		public const Int64 AVERAGE_BUDGET_COLUMN_INDEX=10;	
		/// <summary>										
		/// Colonne 1er Annonceur			
		/// </summary>
		public const Int64 FIRST_ADVERTISER_COLUMN_INDEX=11;
		/// <summary>
		/// Colonne investissement 1er Annonceur			
		/// </summary>
		public const Int64 FIRST_ADVERTISER_INVEST_COLUMN_INDEX=12;
		/// <summary>
		/// Colonne SOV 1er Annonceur			
		/// </summary>
		public const Int64 FIRST_ADVERTISER_SOV_COLUMN_INDEX=13;
		/// <summary>
		/// Colonne 1er r�f�rence			
		/// </summary>
		public const Int64 FIRST_PRODUCT_COLUMN_INDEX=14;
		/// <summary>
		/// Colonne investissement 1er r�f�rence			
		/// </summary>
		public const Int64 FIRST_PRODUCT_INVEST_COLUMN_INDEX=15;
		/// <summary>
		/// Colonne SOV 1er r�f�rence			
		/// </summary>
		public const Int64 FIRST_PRODUCT_SOV_COLUMN_INDEX=16;	
		#endregion
		
		#region constantes pr�sentation graphique
		/// <summary>
		/// Colonne ID mois 
		/// </summary>
		/// <remarks>pour pr�sentation tableau</remarks>
		public const Int64 ID_MONTH_COLUMN_INDEX=0;
		/// <summary>
		/// Colonne ID ELement (annonceur de r�f�rence ou concurrents, ou total )
		/// </summary>
		/// <remarks>pour pr�sentation tableau</remarks>
		public const Int64 ID_ELEMENT_COLUMN_INDEX=1;
		/// <summary>
		/// Colonne Libell� El�ment (annonceur de r�f�rence ou concurrents, ou total )
		/// </summary>
		/// <remarks>pour pr�sentation tableau</remarks>
		public const Int64 LABEL_ELEMENT_COLUMN_INDEX=2;
		/// <summary>
		/// Colonne Investissement
		/// </summary>
		/// <remarks>pour pr�sentation tableau</remarks>
		public const Int64 INVEST_COLUMN_INDEX=3;
		/// <summary>
		/// Colonne Id Total March�
		/// </summary>
		/// <remarks>pour pr�sentation tableau</remarks>
		public const Int64 ID_TOTAL_MARKET_COLUMN_INDEX=-1;
		/// <summary>
		/// Colonne Id Total famille
		/// </summary>
		/// <remarks>pour pr�sentation tableau</remarks>
		public const Int64 ID_TOTAL_SECTOR_COLUMN_INDEX=-2;
		/// <summary>
		/// Colonne Id Total famille
		/// </summary>
		/// <remarks>pour pr�sentation tableau</remarks>
		public const Int64 ID_TOTAL_UNIVERSE_COLUMN_INDEX=-3;
		/// <summary>
		/// Colonne nombre colonnes
		/// </summary>
		/// <remarks>pour pr�sentation tableau</remarks>
		public const Int64 NB_MAX_COLUMNS_COLUMN_INDEX=4;
		#endregion
		
	}
	#endregion

	#region  Nouveaut�s
	/// <summary>
	/// Constantes pour Indicateurs Nouveaut�s
	/// </summary>
	public class Novelty:MotherRecap{
	
		#region Constantes
		/// <summary>
		///Colonne des lib�ll�s Annonceurs ou produit
		///</summary>
		public const int ELEMENT_COLUMN_INDEX=0;	
		/// <summary>
		/// ID ELEMENT  (annonceur ou produit)
		/// </summary>
		public const int ID_ELEMENT_COLUMN_INDEX=1;
		/// <summary>
		/// Colonne Inevstissement dernier mois actif ann�e N-1
		/// </summary>
		public const int LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX=2;
		///<summary>
		///Colonne lib�ll�  dernier mois actif ann�e N-1
		/// </summary>				
		public const int LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX=3;		
		/// <summary>
		/// Periode d'inactivit� (nombre de mois)
		/// </summary>
		public const int INACTIVITY_PERIOD_COLUMN_INDEX=4;		
		/// <summary>
		/// Investissement mois en cours
		/// </summary>
		public const int CURRENT_MONTH_INVEST_COLUMN_INDEX=5;		
		/// <summary>
		/// SOV
		/// </summary>
		public const int SOV_COLUMN_INDEX=6;
		/// <summary>
		///Colonne des ID ANNONCEURS POUR LA PERSONNALISATION DES ELMENTS
		///</summary>
		public const int PERSONNALISATION_ELEMENT_COLUMN_INDEX=7;
		/// <summary>
		///Colonne des ID DERNIER MOIS ACTIF
		///</summary>
		public const int LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX=8;
		/// <summary>
		/// nombre maximal de colonne pour le tableau de r�sultats dans la classe m�tier (rules)
		/// </summary>
		public const int MAX_COLUMN_LENGTH=9;
		/// <summary>
		/// SYMBOLE POURCENTAGE
		/// </summary>
		public const string PERCENT_SYMBOL="%";
		#endregion
			
	}
	#endregion
	
	#region Strat�gie m�dia
	/// <summary>
	/// Classe M�re de strat�gie M�dia
	/// </summary>
	public class MotherMediaStrategy:MotherRecap{
		
		#region Constantes index colonnes
		/// <summary>
		/// COLONNE  INVESTISSEMENT ANNONCEUR DE REFERENCE OU CONCURRENT ANNEE N
		/// </summary>
		public const int REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX = 0;		
		/// <summary>
		/// COLONNE ID  Vehicle
		/// </summary>
		public const int ID_VEHICLE_COLUMN_INDEX = 1;
		/// <summary>
		/// COLONNE LIBELLE Vehicle
		/// </summary>
		public const int LABEL_VEHICLE_COLUMN_INDEX = 2;
		/// <summary>
		/// COLONNE ID CATEGORIE
		/// </summary>
		public const int ID_CATEGORY_COLUMN_INDEX = 3;
		/// <summary>
		/// COLONNE LIBELLE CATEGORIE
		/// </summary>
		public const int LABEL_CATEGORY_COLUMN_INDEX = 4;
		/// <summary>
		/// COLONNE ID MEDIA
		/// </summary>
		public const int ID_MEDIA_COLUMN_INDEX = 5;
		/// <summary>
		/// COLONNE LIBELLE MEDIA
		/// </summary>
		public const int LABEL_MEDIA_COLUMN_INDEX = 6;
		/// <summary>
		/// COLONNE ID ANNONCEURS DE REFERENCE OU CONCURRENTS
		/// </summary>
		public const int ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX = 7;
		/// <summary>
		/// OLONNE LIBELLE ANNONCEURS DE REFERENCE OU CONCURRENTS
		/// </summary>
		public const int LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX = 8;
		/// <summary>
		/// COLONNE MEDIA (VEHICLE) INVESTISSMENT TOTAL UNIVERS
		/// </summary>
		public const int TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX = 9;
		/// <summary>
		/// COLONNE MEDIA (VEHICLE) INVESTISSEMENT TOTAL FAMILLE
		/// </summary>
		public const int TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX = 10;
		/// <summary>
		/// COLONNE MEDIA (VEHICLE) INVESTISSEMENT TOTAL MARCHE
		/// </summary>
		public const int TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX = 11;		
		/// <summary>
		/// COLONNE CATEGORIE INVESTISSMENT TOTAL UNIVERS
		/// </summary>
		public const int TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX = 12;
		/// <summary>
		/// COLONNE CATEGORIE INVESTISSEMENT TOTAL FAMILLE
		/// </summary>
		public const int TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX = 13;
		/// <summary>
		/// COLONNE CATEGORIE INVESTISSEMENT TOTAL MARCHE
		/// </summary>
		public const int TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX = 14;
		/// <summary>
		/// COLONNE CATEGORIE INVESTISSMENT TOTAL UNIVERS
		/// </summary>
		public const int TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX = 15;
		/// <summary>
		/// COLONNE CATEGORIE INVESTISSEMENT TOTAL FAMILLE
		/// </summary>
		public const int TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX = 16;
		/// <summary>
		/// COLONNE CATEGORIE INVESTISSEMENT TOTAL MARCHE
		/// </summary>
		public const int TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX = 17;
		/// <summary>
		/// COLONNE INVESTISSMENT TOTAL UNIVERS
		/// </summary>
		public const int TOTAL_UNIV_INVEST_COLUMN_INDEX = 18;
		/// <summary>
		/// COLONNE INVESTISSEMENT TOTAL FAMILLE
		/// </summary>
		public const int TOTAL_SECTOR_INVEST_COLUMN_INDEX = 19;
		/// <summary>
		/// COLONNE  INVESTISSEMENT TOTAL MARCHE
		/// </summary>
		public const int TOTAL_MARKET_INVEST_COLUMN_INDEX = 20;
		/// <summary>
		/// COLONNE  INVESTISSEMENT ANNONCEUR DE REFERENCE OU CONCURRENT ANNEE N POUR TOTAL 		
		/// </summary>
		public const int TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX = 21;				
			
		#endregion
		
	}

	/// <summary>
	/// Constantes pour start�gie m�dia
	/// </summary>
	public class MediaStrategy : MotherMediaStrategy {

		#region Constantes index colonnes		
		/// <summary>
		/// COLONNE PDM
		/// </summary>
		public const int PDM_COLUMN_INDEX = 22;
		/// <summary>
		/// COLONNE EVOLUTION
		/// </summary>
		public const int EVOL_COLUMN_INDEX = 23;
		/// <summary>
		/// COLONNE ID PREMIER ANNONCEUR
		/// </summary>
		public const int ID_FIRST_ADVERT_COLUMN_INDEX = 24;
		/// <summary>
		/// COLONNE LIBELLE PREMIER ANNONCEUR
		/// </summary>
		public const int LABEL_FIRST_ADVERT_COLUMN_INDEX = 25;
		/// <summary>
		/// COLONNE INVESTISSSEMENT  PREMIER ANNONCEUR
		/// </summary>
		public const int INVEST_FIRST_ADVERT_COLUMN_INDEX = 26;
		/// <summary>
		/// COLONNE ID PREMIERE REFENCE
		/// </summary>
		public const int ID_FIRST_REF_COLUMN_INDEX = 27;
		/// <summary>
		/// COLONNE LIBELLE PREMIER ANNONCEUR
		/// </summary>
		public const int LABEL_FIRST_REF_COLUMN_INDEX = 28;
		/// <summary>
		/// COLONNE INVESTISSSEMENT  PREMIER ANNONCEUR
		/// </summary>
		public const int INVEST_FIRST_REF_COLUMN_INDEX = 29;				
		/// <summary>
		/// NOMBRE MAXIMAL DE COLONNES POUR LE TABLEAU
		/// </summary>
		public const int NB_MAX_COLUMNS = 30;	
		/// <summary>
		/// NOMBRE MAXIMAL DE COLONNES POUR LE TABLEAU (sortie graphique)
		/// </summary>
		public const int NB_CHART_MAX_COLUMNS = 22;	
		#endregion
		
		#region Enum�rateurs
		/// <summary>
		/// Type de tableau
		/// </summary>
		public enum InvestmentType{
			/// <summary>
			/// Affichage des produits
			/// </summary>
			total,
			/// <summary>
			/// Affichage des annonceurs
			/// </summary>
			advertiser		
		}
		/// <summary>
		/// Niveau  m�dia
		/// </summary>
		public enum MediaLevel{
			/// <summary>
			/// Niveau m�dia
			/// </summary>
			vehicleLevel,
			/// <summary>
			/// Niveau support
			/// </summary>
			categoryLevel,
			/// <summary>
			/// Niveau support
			/// </summary>
			mediaLevel
		}
		#endregion
	}
	#endregion

	#region Synth�se
	/// <summary>
	/// Constantes de la planche synth�se des indicateurs
	/// </summary>
	public class SynthesisRecap : MotherRecap{
		/// <summary>
		/// Colonne libell� des univers � calculer 
		/// </summary>
		public const int LABEL_COLUMN_INDEX =0;
		/// <summary>
		/// Ligne budget keuros total s�lection
		/// </summary>
		public const int TOTAL_UNIV_INVEST_LINE_INDEX=1;
		/// <summary>
		/// Ligne budget keuros total famille
		/// </summary>
		public const int TOTAL_SECTOR_INVEST_LINE_INDEX=2;
		/// <summary>
		/// Ligne PDV s�lection / total famille
		/// </summary>
		public const int PDV_UNIV_TOTAL_SECTOR_LINE_INDEX=3;
		/// <summary>
		/// Ligne budget keuros total march�
		/// </summary>
		public const int TOTAL_MARKET_INVEST_LINE_INDEX=4;
		/// <summary>
		/// Ligne PDV s�lection / total march�
		/// </summary>
		public const int PDV_UNIV_TOTAL_MARKET_LINE_INDEX=5;
		/// <summary>
		/// Ligne nombre annonceurs actifs du total s�lection
		/// </summary>
		public const int NB_ADVERTISER_LINE_INDEX=6;
		/// <summary>
		/// Budget moyen pat annonceurs actifs du total s�lection
		/// </summary>
		public const int AVERAGE_INVEST_BY_ADVERTISER_LINE_INDEX=7;
		/// <summary>
		/// Ligne nombre annonceurs actifs
		/// </summary>
		public const int NB_PRODUCT_LINE_INDEX=8;
		/// <summary>
		/// Budget moyen pat annonceurs actifs du total s�lection
		/// </summary>
		public const int AVERAGE_INVEST_BY_PRODUCT_LINE_INDEX=9;
		/// <summary>
		/// Colonne valeurs ann�e N
		/// </summary>
		public const int TOTAL_N_COLUMN_INDEX = 1;
		/// <summary>
		/// Colonne valeurs ann�e N-1
		/// </summary>
		public const int TOTAL_N1_COLUMN_INDEX = 2;

		/// <summary>
		/// Colonne evolution
		/// </summary>
		public const int EVOLUTION_COLUMN_INDEX = 3;
		/// <summary>
		/// Colonne ecart
		/// </summary>
		public const int ECART_COLUMN_INDEX = 4;

	}
	#endregion


	#endregion

	#region Tendances
	/// <summary>
	/// Constantes pour les tendances
	/// </summary>
	public class Tendencies{
	
		#region Constantes
		/// <summary>
		/// Ligne colonne total
		/// </summary>
		public const int TOTAL_LINE=0;
		/// <summary>
		/// Index de la colonne category 
		/// </summary>
		public const int CATEGORY_INDEX=0;
		/// <summary>
		/// Index de la colonne media
		/// </summary>
		public const int MEDIA_INDEX=1;
		/// <summary>
		/// Index de la colonne investissement ann�e N
		/// </summary>
		public const int INVEST_N=2;
		/// <summary>
		/// Index de la colonne investissement ann�e N-1
		/// </summary>
		public const int INVEST_N1=3;
		/// <summary>
		/// Index de la colonne Evolution pour l'investissement
		/// </summary>
		public const int EVOL_INVEST=4;
		/// <summary>
		/// Index de la colonne duree ann�e N
		/// </summary>
		public const int DURATION_N=5;
		/// <summary>
		/// Index de la colonne duree ann�e N-1
		/// </summary>
		public const int DURATION_N1=6;
		/// <summary>
		/// Index de la colonne Evolution pour la duree
		/// </summary>
		public const int EVOL_DURATION=7;
		/// <summary>
		/// Index de la colonne Surface ann�e N
		/// </summary>
		public const int SURFACE_N=5;
		/// <summary>
		/// Index de la colonne Surface ann�e N-1
		/// </summary>
		public const int SURFACE_N1=6;
		/// <summary>
		/// Index de la colonne Evolution pour la surface
		/// </summary>
		public const int EVOL_SURFACE=7;
		/// <summary>
		/// Index de la colonne Insertion ann�e N
		/// </summary>
		public const int INSERTION_N=8;
		/// <summary>
		/// Index de la colonne Insertion ann�e N1
		/// </summary>
		public const int INSERTION_N1=9;
		/// <summary>
		/// Index de la colonne Evolution pour les insertions
		/// </summary>
		public const int EVOL_INSERTION=10;
		/// <summary>
		/// Index de la colonne MMC ann�e N
		/// </summary>
		public const int MMC_N=11;
		/// <summary>
		/// Index de la colonne MMC ann�e N1
		/// </summary>
		public const int MMC_N1=12;
		/// <summary>
		/// Index de la colonne Evolution pour les MMC
		/// </summary>
		public const int EVOL_MMC=13;
		/// <summary>
		/// Nombre de colonne dans la presse
		/// </summary>
		public const int NBRE_COLUMN_PRESS=14;
		/// <summary>
		/// Nombre de colonne dans la radio/tele
		/// </summary>
		public const int NBRE_COLUMN_RADIO_TV=11;

		#endregion

        public enum DateOpeningOption
        {   ///Cumul
            yearToDate = 0,

            //Monthly
            monthly = 1,

            //Weekly
            weekly
        }
	}
	
	#endregion

	#region Portefeuille d'un support
	/// <summary>
	/// Constante pour le Portefeuille d'un support
	/// </summary>
	public class Portofolio{

		/// <summary>
		/// Synth�se publicitaire
		/// </summary>
		public const int SYNTHESIS=0;
		/// <summary>
		/// D�tail du Portefeuille
		/// </summary>
		public const int DETAIL_PORTOFOLIO=1;		
		/// <summary>
		/// Nouveaut�s
		/// </summary>
		public const int NOVELTY=2;		
		/// <summary>
		/// D�tail Support
		/// </summary>
		public const int DETAIL_MEDIA=3;
		/// <summary>
		/// Structure
		/// </summary>
		public const int STRUCTURE=4;
		/// <summary>
		/// Performances
		/// </summary>
		public const int PERFORMANCES=5;
		/// <summary>
		/// Calendrier d'action
		/// </summary>
		public const int CALENDAR=6;

		#region D�tail d'un portefeuille
		/// <summary>
		/// Index de la colonne id Media
		/// </summary>
		public	const int MEDIA_ID_COLUMN_INDEX=0;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N
		/// </summary>
		public	const int IDL1_INDEX=1;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N-1
		/// </summary>
		public	const int IDL2_INDEX=2;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N-2
		/// </summary>
		public	const int IDL3_INDEX=3;
		/// <summary>
		/// Index du label de l'�l�ment N
		/// </summary>
		public	const int LABELL1_INDEX=4;
		/// <summary>
		/// Index du label de l'�l�ment N-1
		/// </summary>
		public	const int LABELL2_INDEX=5;
		/// <summary>
		/// Index du label de l'�l�ment N-2
		/// </summary>
		public	const int LABELL3_INDEX=6;			
		/// <summary>
		/// Index de la colonne euro
		/// </summary>
		public const int EURO_COLUMN_INDEX=7;
		/// <summary>
		/// Index de la colonne Mm/Col
		/// </summary>
		public const int MMC_COLUMN_INDEX=8;
		/// <summary>
		/// Index de la colonne nombres de pages
		/// </summary>
		public const int PAGES_COLUMN_INDEX=9;
		/// <summary>
		/// Index de la colonne nombre insertions/spots
		/// </summary>
		public const int INSERTIONS_COLUMN_INDEX=11;
		/// <summary>
		/// Index de la colonne dur�e
		/// </summary>
		public const int DURATION_COLUMN_INDEX=10;
		/// <summary>
		/// Index de la colonne contenant l'identifiant de l'adresse
		/// </summary>
		public const int ADDRESS_COLUMN_INDEX=12;
		/// <summary>
		/// Index de la ligne total
		/// </summary>
		public static int TOTAL_LINE_INDEX=0;
		/// <summary>
		/// Nombre total de colonnes
		/// </summary>
		public 	const int TOTAL_INDEX=13;
		#endregion
		
	}

	/// <summary>
	/// Constantes pour le calendrier d'action d'un portefeuille
	/// </summary>
	public class PortofolioCalendar{
	
		/// <summary>
		/// Index de la colonne id Media
		/// </summary>
		public	const int MEDIA_ID_COLUMN_INDEX=0;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N
		/// </summary>
		public	const int IDL1_INDEX=1;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N-1
		/// </summary>
		public	const int IDL2_INDEX=2;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N-3
		/// </summary>
		public	const int IDL3_INDEX=3;
		/// <summary>
		/// Index du label de l'�l�ment N
		/// </summary>
		public	const int LABELL1_INDEX=4;
		/// <summary>
		/// Index du label de l'�l�ment N-1
		/// </summary>
		public	const int LABELL2_INDEX=5;
		/// <summary>
		/// Index du label de l'�l�ment N-2
		/// </summary>
		public	const int LABELL3_INDEX=6;
		/// <summary>
		/// Index de la colonne contenant l'identifiant de l'adresse
		/// </summary>
		public const int ADDRESS_COLUMN_INDEX=7;
		/// <summary>
		/// Index de la colonne total
		/// </summary>
		public const int TOTAL_VALUE_COLUMN_INDEX=8;
		/// <summary>
		/// Index de la colonne PDM
		/// </summary>
		public const int PDM_COLUMN_INDEX=9;
		/// <summary>
		/// Index de la colonne euro
		/// </summary>
		public const int VALUE_COLUMN_INDEX=10;
		/// <summary>
		/// Nombre total de colonnes
		/// </summary>
		public 	const int TOTAL_INDEX=11;
		/// <summary>
		/// Index de la ligne contenant les dates
		/// </summary>
		public const int DATE_INDEX=0;
		/// <summary>
		/// Ligne total
		/// </summary>
		public const int TOTAL_LINE=1;
	
	}

	/// <summary>
	/// constantes utilis�s en radio tv pour le d�tail d'un support
	/// </summary>	
	public class PortofolioDetailMedia{
		/// <summary>
		/// END_ARRAY
		/// </summary>
		public const int END_ARRAY=-1;
		/// <summary>
		/// Investissement le lundi
		/// </summary>
		public const int MONDAY_VALUE=0;
		/// <summary>
		/// Nombre de pub le lundi
		/// </summary>
		public const int MONDAY_INSERTION=1;
		/// <summary>
		/// Investissement le mardi
		/// </summary>
		public const int TUESDAY_VALUE=2;
		/// <summary>
		/// Nombre de pub le mardi
		/// </summary>
		public const int TUESDAY_INSERTION=3;
		/// <summary>
		/// Investissement le mercredi
		/// </summary>
		public const int WEDNESDAY_VALUE=4;
		/// <summary>
		/// Nombre de pub le mercredi
		/// </summary>
		public const int WEDNESDAY_INSERTION=5;
		/// <summary>
		/// Investissement le jeudi
		/// </summary>
		public const int THURSDAY_VALUE=6;
		/// <summary>
		/// Nombre de pub le jeudi
		/// </summary>
		public const int THURSDAY_INSERTION=7;
		/// <summary>
		/// Investissement le vendredi
		/// </summary>
		public const int FRIDAY_VALUE=8;
		/// <summary>
		///  Nombre de pub le vendredi
		/// </summary>
		public const int FRIDAY_INSERTION=9;
		/// <summary>
		/// Investissement le samedi
		/// </summary>
		public const int SATURDAY_VALUE=10;
		/// <summary>
		/// Nombre de pub le samedi
		/// </summary>
		public const int SATURDAY_INSERTION=11;
		/// <summary>
		/// Investissement le dimanche
		/// </summary>
		public const int SUNDAY_VALUE=12;
		/// <summary>
		/// Nombre de pub le dimanche
		/// </summary>
		public const int SUNDAY_INSERTION=13;
		/// <summary>
		/// index ecran
		/// </summary>
		public const int ECRAN=14;
		/// <summary>
		/// Nombre de colonne
		/// </summary>
		public const int TOTAL_INDEX=15;

		#region Indexes des colonnes � trier
		/// <summary>
		/// index colonne des annonceurs
		/// </summary>
		public const int ADVERTISER_COLUMN_INDEX=0;
		/// <summary>
		/// index colonne des produits
		/// </summary>
		public const int PRODUCT_COLUMN_INDEX=1;
		#endregion

	}

	public class PortofolioSynthesis {
		/// <summary>
		/// Investment result type
		/// </summary>
		public const int INVESTMENT_DATA = 0;
		/// <summary>
		/// Insertion number result type
		/// </summary>
		public const int INSERTION_NUMBER_DATA = 1;
		/// <summary>
		/// Category medida seller result type
		/// </summary>
		public const int CATEGORY_MEDIA_SELLER_DATA = 2;
		/// <summary>
		/// Type sale result type
		/// </summary>
		public const int TYPE_SALE_NUMBER_DATA = 3;
		/// <summary>
		/// Number product advertiser result type
		/// </summary>
		public const int NUMBER_PRODUCT_ADVERTISER_DATA = 4;
		/// <summary>
		/// Number inset result type
		/// </summary>
		public const int NUMBER_INSET_DATA = 5;
		/// <summary>
		/// Number page result type
		/// </summary>
		public const int NUMBER_PAGE_DATA = 6;
        /// <summary>
        /// Data type for portofolio synthesis
        /// </summary>
        public enum dataType { 
            /// <summary>
            /// Period selected
            /// </summary>
            periodSelected,
            /// <summary>
            /// Periodicity
            /// </summary>
            periodicity,
            /// <summary>
            /// Media
            /// </summary>
            media,
            /// <summary>
            /// Category
            /// </summary>
            category,
            /// <summary>
            /// Media seller
            /// </summary>
            mediaSeller,
            /// <summary>
            /// Volume
            /// </summary>
            volume,
            /// <summary>
            /// Interest center
            /// </summary>
            interestCenter,
            /// <summary>
            /// Number board
            /// </summary>
            numberBoard,
            /// <summary>
            /// Type sale
            /// </summary>
            typeSale,
            /// <summary>
            /// Number of pages
            /// </summary>
            pageNumber,
            /// <summary>
            /// Number of pages of advertisements
            /// </summary>
            adNumber,
            /// <summary>
            /// Number of pages of advertisements excluding insets
            /// </summary>
            adNumberExcludingInsets,
            /// <summary>
            /// Number of pages of advertisements including insets
            /// </summary>
            adNumberIncludingInsets,
            /// <summary>
            /// No. of spots (excl. self-promotion)
            /// </summary>
            numberSpot,
            /// <summary>
            /// Number of ad breaks
            /// </summary>
            numberAdBreaks,
            /// <summary>
            /// Total duration
            /// </summary>
            totalDuration,
            /// <summary>
            /// Total ad spends in euros
            /// </summary>
            investment,
            /// <summary>
            /// Number of products
            /// </summary>
            numberProduct,
            /// <summary>
            /// Number of new products in the advert tracking selection
            /// </summary>
            numberNewProductInTracking,
            /// <summary>
            /// Number of new products in the vehicle
            /// </summary>
            numberNewProductInVehicle,
            /// <summary>
            /// Number of advertisers
            /// </summary>
            numberAdvertiser,
            /// <summary>
            /// Average duration of a spot
            /// </summary>
            averageDurationEcran,
            /// <summary>
            /// Average number of spots per ad break
            /// </summary>
            numberSpotByEcran,
            /// <summary>
            /// Duration
            /// </summary>
            duration,
            /// <summary>
            /// Insertion
            /// </summary>
            insertion,
            /// <summary>
            /// Number of banners (Evaliant)
            /// </summary>
            numberBanners,
        }
	}

	#region Structure
	/// <summary>
	/// Constante pour structure de portefeuille d'un support
	/// </summary>
	public class PortofolioStructure{

		#region Enum�rateurs
		/// <summary>
		/// Type de ventilation pour la presse
		/// </summary>
		public enum Ventilation{
			/// <summary>
			/// format du m�dia presse
			/// </summary>
			format,
			/// <summary>
			/// couleur du m�dia presse
			/// </summary>
			color,
			/// <summary>
			/// Emplacement du m�dia presse
			/// </summary>
			location,
			/// <summary>
			/// Encarts du m�dia presse
			/// </summary>
			insert
		}
		#endregion

		/// <summary>
		/// Colonne des tranches horaires des m�dia
		/// </summary>
		public const int MEDIA_HOURS_COLUMN_INDEX=0;
		/// <summary>
		/// Colonne des investissements par tranches horaires 
		/// </summary>
		public const int EUROS_COLUMN_INDEX=1;
		/// <summary>
		/// Colonne des des spots par tranches horaires 
		/// </summary>
		public const int SPOT_COLUMN_INDEX=2;
		/// <summary>
		/// Colonne des dur�es par tranches horaires des m�dia
		/// </summary>
		public const int DURATION_COLUMN_INDEX=3;
		/// <summary>
		/// Colonne des ventilation ( frormat,couleur,emplacements,encarts) pour la presse
		/// </summary>
		public const int VENTILATION_COLUMN_INDEX=0;
		/// <summary>
		/// Colonne des tranches horaires des m�dia
		/// </summary>
		public const int INSERTIONS_COLUMN_INDEX=1;
		/// <summary>
		/// Nombre de colonnes max pour la radio ou tv
		/// </summary>
		public const int RADIO_TV_NB_MAX_COLUMNS=4;
		/// <summary>
		/// Nombre de lignes max pour la radio 
		/// </summary>
		public const int RADIO_NB_MAX_LINES=6;
		/// <summary>
		/// Nombre de lignes max pour la  tv
		/// </summary>
		public const int TV_NB_MAX_LINES=8;
		/// <summary>
		/// Nombre de colonnes max pour la presse
		/// </summary>
		public const int PRESS_NB_MAX_COLUMNS=2;
	}	
	#endregion

	#endregion

	#region Tableaux de bord
	/// <summary>
	/// Constantes des tableaux de bord
	/// </summary>
	public class DashBoard{

		#region contantes
		/// <summary>
		/// Identifiant �l�ment de niveau 1
		/// </summary>
		public const int ID_ELMT_L1_COLUMN_INDEX=0;
		/// <summary>
		/// Libell� �l�ment de niveau 1
		/// </summary>
		public const int LABEL_ELMT_L1_COLUMN_INDEX=1;
		/// <summary>
		/// Identifiant �l�ment de niveau 2
		/// </summary>
		public const int ID_ELMT_L2_COLUMN_INDEX=2;
		/// <summary>
		/// Libell� �l�ment de niveau 3
		/// </summary>
		public const int LABEL_ELMT_L2_COLUMN_INDEX=3;
		/// <summary>
		/// Identifiant �l�ment de niveau 3
		/// </summary>
		public const int ID_ELMT_L3_COLUMN_INDEX=4;
		/// <summary>
		/// Libell� �l�ment de niveau 3
		/// </summary>
		public const int LABEL_ELMT_L3_COLUMN_INDEX=5;
		/// <summary>
		/// Colonne �volution
		/// </summary>
		public const int EVOL_COLUMN_INDEX = 6;	
		/// <summary>
		/// Colonne  P�riode N
		/// </summary>
		public const int PERIOD_N_COLUMN_INDEX = 7;
		/// <summary>
		/// Colonne  P�riode N-1
		/// </summary>
		public const int PERIOD_N1_COLUMN_INDEX = 8;
		/// <summary>
		/// Colonne pourcentage de repartition
		/// </summary>
		public const int REPARTITION_PERCENT = 9;
		/// <summary>
		/// Colonne PDM horizontal
		/// </summary>
		public const int PDM_COLUMN_INDEX = 10;	
		/// <summary>
		/// Colonne PDM horizontal
		/// </summary>
		public const int PDV_COLUMN_INDEX = 11;	
		/// <summary>
		/// Colonne somme total d'un type r�partition
		/// </summary>
		public const int TOTAL_COLUMN_INDEX = 12;
		/// <summary>
		/// Nombre de colonnes total pour les formats de r�partition
		/// </summary>
		public const int NB_TOTAL_FORMAT_COLUMNS=8;
		/// <summary>
		/// Nombre de colonnes total pour les jours nomm�s de r�partition
		/// </summary>
		public const int NB_TOTAL_NAMED_DAY_COLUMNS=9;
		/// <summary>
		/// Nombre de colonne total les tranches horaires t�l�vision
		/// </summary>
		public const int NB_TOTAL_TV_TIMESLICE_COLUMNS=7;
		/// <summary>
		///  Nombre de colonne total les tranches horaires radio
		/// </summary>
		public const int NB_TOTAL_RADIO_TIMESLICE_COLUMNS=5;
		/// <summary>
		///  Nombre de colonne total pour les unit�s avec radio ou tv
		/// </summary>
		public const int NB_TOTAL_RADIO_TV_UNIT_COLUMNS =3;
		/// <summary>
		/// Nombre de colonne total pour les unit�s avec press
		/// </summary>
		public const int NB_TOTAL_PRESS_UNIT_COLUMNS =4;
		/// <summary>
		/// Nombre total de colonnes fixes
		/// </summary>
		public const int NB_TOTAL_CONST_COLUMNS = 13;	
		#endregion	
	}	
	
	
	#endregion

	#region APPM
	/// <summary>
	/// Constantes APPM
	/// </summary>
	public class APPM{
		#region constantes
		/// <summary>
		/// Cible de base Ensemble
		/// </summary>
		public const string BaseTarget ="Ensemble 15 ans et plus";
		/// <summary>
		/// Synth�se APPM
		/// </summary>
		public const int synthesis=0;
		/// <summary>
		/// P�riodicit�s du plan APPM
		/// </summary>
		public const int periodicityPlan=4;
		/// <summary>
		/// Valorisation and efficiency per media
		/// </summary>
		public const int supportPlan=2;
		/// <summary>
		/// affinities of targets
		/// </summary>
		public const int affinities=6;
		/// <summary>
		/// Analyse des parts de voix du plan
		/// </summary>
		public const int PDVPlan=3;
		/// <summary>
		/// Analyse des familles d'int�r�ts du plan
		/// </summary>
		public const int interestFamily=5;
//		/// <summary>
//		/// Types d'empacement du plan
//		/// </summary>
//		public const int locationPlanType=7;
		/// <summary>
		///  Plan m�dia paar vesions
		/// </summary>
		public const int mediaPlanByVersion=7;
		/// <summary>
		/// Plan m�dia 
		/// </summary>
		public const int mediaPlan=1;
		/// <summary>
		/// Sector Data synthesis 
		/// </summary>
		public const int sectorDataSynthesis=0;
		/// <summary>
		/// Sector Data Average 
		/// </summary>
		public const int sectorDataAverage=1;
		/// <summary>
		/// Sector Data Seasonality 
		/// </summary>
		public const int sectorDataSeasonality=2;
		/// <summary>
		/// Sector Data Interest family 
		/// </summary>
		public const int sectorDataInterestFamily=3;
		/// <summary>
		/// Sector Data Periodicity
		/// </summary>
		public const int sectorDataPeriodicity=4;
		/// <summary>
		/// Sector Data Affinities
		/// </summary>
		public const int sectorDataAffinities=5;

		#region constantes des tableaux de donn�es de cadrage
		/// <summary>
		/// l'index de la premi�re colonne
		/// </summary>
		public const int FIRST_COLUMN_INDEX=1;
		/// <summary>
		/// l'index de la deuxi�me colonne
		/// </summary>
		public const int SECOND_COLUMN_INDEX=2;
		/// <summary>
		/// L'index de la colonne des moyennes
		/// </summary>
		public const int AVERAGE_COLUMN_INDEX=2;
		/// <summary>
		/// L'index de la colonne des min
		/// </summary>
		public const int MIN_COLUMN_INDEX=3;
		/// <summary>
		/// L'index de la colonne des max
		/// </summary>
		public const int MAX_COLUMN_INDEX=4;
		/// <summary>
		/// L'index de la colonne GRP
		/// </summary>
		public const int GRP_COLUMN_INDEX=2;
		/// <summary>
		/// L'index de la colonne Affinities GRP
		/// </summary>
		public const int AFFINITIES_GRP_COLUMN_INDEX=3;
		/// <summary>
		/// L'index de la colonne CGRP
		/// </summary>
		public const int CGRP_COLUMN_INDEX=4;
		/// <summary>
		/// L'index de la colonne Affinities CGRP
		/// </summary>
		public const int AFFINITIES_CGRP_COLUMN_INDEX=5;
		/// <summary>
		/// L'index de la colonne type de l'�tude (saisonnalit�, famille de presse, p�riodicit�)
		/// </summary>
		public const int TYPE_COLUMN_INDEX=1;
		/// <summary>
		/// L'index de la colonne des unit�s
		/// </summary>
		public const int UNIT_COLUMN_INDEX=2;
		/// <summary>
		/// L'index de la colonne des r�partitions
		/// </summary>
		public const int DISTRIBUTION_COLUMN_INDEX=3;
		/// <summary>
		/// L'index de la colonne des unit�s dans le cas de GRP
		/// </summary>
		public const int GRP_UNIT_COLUMN_INDEX=4;
		/// <summary>
		/// L'index de la colonne des r�partitions dans le cas de GRP
		/// </summary>
		public const int GRP_DISTRIBUTION_COLUMN_INDEX=5;
		#endregion

		#endregion

		/// <summary>
		/// Type de la date de d�but
		/// </summary>
		public enum TypeDateBegin
		{
			/// <summary>
			/// Date de d�but est comprise dans au moins une vague
			/// </summary>
			inSideWave,
			/// <summary>
			/// Date de d�but est comprise dans aucune vague
			/// </summary>
			outSideWave,
			/// <summary>
			/// Aucnun cas
			/// </summary>
			none
		}

		
	}
	#endregion

	#region Parrainage TV

	
	/// <summary>
	/// Constantes des tableaux du parrainage tv
	/// </summary>
	public class TvSponsorship{

	
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N-1
		/// </summary>
		public	const int IDL1_INDEX=0;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N-2
		/// </summary>
		public	const int IDL2_INDEX=2;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N-3
		/// </summary>
		public	const int IDL3_INDEX=4;
		/// <summary>
		/// Index de l'identifiant de l'�l�ment N-4
		/// </summary>
		public	const int IDL4_INDEX=6;
		/// <summary>
		/// Index du label de l'�l�ment N
		/// </summary>
		public	const int LABELL1_INDEX=1;
		/// <summary>
		/// Index du label de l'�l�ment N-1
		/// </summary>
		public	const int LABELL2_INDEX=3;
		/// <summary>
		/// Index du label de l'�l�ment N-2
		/// </summary>
		public	const int LABELL3_INDEX=5;
		/// <summary>
		/// Index du label de l'�l�ment N-2
		/// </summary>
		public	const int LABELL4_INDEX=7;

		/// <summary>
		/// Index de la ligne du total
		/// </summary>
		public 	const int TOTAL_LINE_INDEX=0;

		/// <summary>
		/// Index de la colonne des cr�ations
		/// </summary>
		public 	const int CREATIVE_COLUMN_INDEX=8;

		/// <summary>
		/// Index de la colonne contenant l'identifiant de l'adresse
		/// </summary>
		public const int ADDRESS_COLUMN_INDEX=9;

		/// <summary>
		/// Index de la colonne du total 
		/// </summary>
		public 	const int TOTAL_COLUMN_INDEX=10;

		/// <summary>
		/// Index de la colonne du premier �l�ment r�sultat
		/// </summary>
		public	const int FIRST_RESULT_ITEM_COLUMN_INDEX=11;

		/// <summary>
		/// Identifiant de la colonne des niveaux (produit ou supports)
		/// </summary>
		public const int LEVEL_HEADER_ID=0;

		/// <summary>
		/// Identifiant de la colonne d'acc�s aux Cr�ation
		/// </summary>
		public const int CREATIVE_HEADER_ID=1;
        /// <summary>
        /// Identifiant de la colonne Insertions
        /// </summary>
        public const int INSERTIONS_HEADER_ID = 2;

		/// <summary>
		/// Identifiant de la colonne du total des univers
		/// </summary>
		public const int TOTAL_HEADER_ID=3;

		/// <summary>
		/// D�but Identifiant d'un groupe
		/// </summary>
		public const int START_ID_GROUP=10;

	}	

	#endregion

}

