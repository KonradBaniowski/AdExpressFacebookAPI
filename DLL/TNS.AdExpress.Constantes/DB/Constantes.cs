
using System;

namespace TNS.AdExpress.Constantes.DB{

	#region Source de données
	/// <summary>
	/// Constantes liées aux sources de données
	/// </summary>
	public class DataSource{
		/// <summary>
		/// type de la source de données
		/// </summary>
		public enum Type{
			/// <summary>
			/// Fichier
			/// </summary>
			file,
			/// <summary>
			/// Fichier xml
			/// </summary>
			xml,
			/// <summary>
			/// Oracle
			/// </summary>
			oracle
		}
	}
	#endregion

	#region Class ActivationValues
	/// <summary>
	/// Constantes relatives à la notion d'Activation
	/// </summary>
	public class ActivationValues
	{
		/// <summary>
		/// Activée
		/// </summary>
		public const int ACTIVATED=0;
		/// <summary>
		/// Désactivée
		/// </summary>
		public const int DEAD=10;
		/// <summary>
		/// Désactivée
		/// </summary>
		public const int UNACTIVATED=50;
		/// <summary>
		/// Code de suppression d'un login dans ISIS
		/// </summary>
		public const int ISIS_SUPPRESSION=50;
	}
	#endregion

	#region Class ExceptionValues
	/// <summary>
	/// Constantes relatives à la notion d'Activation
	/// </summary>
	public class ExceptionValues{
		/// <summary>
		/// Ce n'est pas une exception
		/// </summary>
		public const Int16 IS_NOT_EXCEPTION=0;
		/// <summary>
		/// C'est une exception
		/// </summary>
		public const Int16 IS_EXCEPTION=10; 
	}
	#endregion

	#region Classe des Langues
	/// <summary>
	/// Constantes des codes de langues d'AdExpress
	/// </summary>
	public class Language{
		/// <summary>
		/// Langue française
		/// </summary>
		public const int FRENCH=33;
		/// <summary>
		/// Langue Anglaise
		/// </summary>
		public const int ENGLISH=44;
        /// <summary>
        /// Langue Finnoise
        /// </summary>
        public const int FINNOIS = 35;
        /// <summary>
        /// Langue Finnoise
        /// </summary>
        public const int SLOVAKIA = 421;
    }

	#endregion

	#region Classe des Operations
	/// <summary>
	/// Constante des type d'opérations pour les procédures stockées
	/// </summary>
	public class Procedure{

		#region Constantes
		/// <summary>
		/// Opération Insert
		/// </summary>
		public const string OPERATION_INSERT="INSERT";
		/// <summary>
		/// Opération Update
		/// </summary>
		public const string OPERATION_UPDATE="UPDATE";

		/// <summary>
		/// Etape de modification
		/// </summary>
		public const int STAGE_INITIALISATION=0;
		/// <summary>
		/// Etape de création
		/// </summary>
		public const int STAGE_CREATION=1;
		/// <summary>
		/// Etape d'affectation
		/// </summary>
		public const int STAGE_ASSIGNMENT=2;
		/// <summary>
		/// Etape de suppression
		/// </summary>
		public const int STAGE_SUPRESS=3;
		/// <summary>
		/// Etape de modification
		/// </summary>
		public const int STAGE_MODIFICATION=4;
		#endregion
	}

	#endregion

	#region Classe des Connexion
	/// <summary>
	/// Constantes des connexion à la base de données pour le site Web AdExpress
	/// </summary>
	public class Connection{

		#region Constantes
		/// <summary>
		/// Chaîne de connexion à la base de données pour ramener les date de parution
		/// </summary>
		//public const string PUBLICATION_DATES_CONNECTION_STRING="User Id=user_web; Password=zebony8; Data Source=adexpr03.pige;Pooling=true;Pooling=true; Max Pool Size=150; Decr Pool Size=20;Connection Timeout=120";
        /// <summary>
        /// Chaîne de connexion à la base de données pour les textes du site AdExpress
        /// </summary>
        //public const string WEB_ADMINISTRATION_CONNECTION_STRING="User Id=admin_web; Password=zepabony9; Data Source=adexpr03.pige;Pooling=true;Pooling=true; Max Pool Size=150; Decr Pool Size=20;Connection Timeout=120";
		/// <summary>
		/// Chaîne de connexion à la base de données pour les textes du site AdExpress
		/// </summary>
		//public const string TRADUCTION_CONNECTION_STRING="User Id=admin_web; Password=zepabony9; Data Source=adexpr03.pige;Pooling=true;Pooling=true; Max Pool Size=150; Decr Pool Size=20;Connection Timeout=120";
		/// <summary>
		/// Chaîne de connexion à la base de données pour les testes de développement
		/// </summary>
		public const string DEV_CONNECTION_STRING="User Id=gfacon; Password=sandie5; Data Source=adexpr03.pige;Pooling=true; Max Pool Size=150; Decr Pool Size=20; Connection Timeout=120"; 
		/// <summary>
		/// Chaîne de connexion à la base de données pour les sessions
		/// </summary>
		//public const string SESSION_CONNECTION_STRING="User Id=mou01_session_1; Password=sessionmolle1; Data Source=adexpr03.pige;Pooling=true; Max Pool Size=150; Decr Pool Size=20;Connection Timeout=120"; 
		/// <summary>
		/// Chaîne de connexion à la base de données pour les sessions
		/// </summary>
		//public const string SESSION_CONNECTION_STRING_TEST="User Id=gfacon; Password=sandie5; Data Source=adexpr03.pige;Pooling=true;Pooling=true; Max Pool Size=150; Decr Pool Size=20;Connection Timeout=120"; 
		/// <summary>
		/// Chaîne de connexion à la base de données utilisée dans les droits
		/// </summary>
		public const string RIGHT_CONNECTION_STRING=" ; Data Source="+TNS.AdExpress.Constantes.DB.TNSName.CUSTOMER_TNS_NAME+" ;Pooling=true; Max Pool Size=150; Decr Pool Size=20; Connection Timeout=120";
		/// <summary>
		/// Chaîne de connexion à la base de données pour les Recap
		/// </summary>
		//public const string RECAP_CONNECTION_STRING="User Id=recap01; Password=itulatif8; Data Source=adexpr03.pige;Pooling=true;Pooling=true; Max Pool Size=150; Decr Pool Size=20;Connection Timeout=120"; 
		/// <summary>
		/// Chaîne de connexion à la base de données pour les GRP
		/// </summary>
		//public const string GRP_CONNECTION_STRING="User Id=grp; Password=rpgdoom3; Data Source=PITAFR02.PIGE;Connection Timeout=120";
		/// <summary>
		/// Chaîne de connexion à la base de données pour GEB (alertes portefeuille)
		/// </summary>
		//public const string GEB_CONNECTION_STRING="User Id=adexpr03_proc_7; Password=pushit76; Data Source=adexpr03.pige; Pooling=False;Connection Timeout=120;";
		/// <summary>
		/// Chaîne de connexion à la base de données pour HERMES (Indicateurs AdExpress)
		/// </summary>
		//public const string HERMES_CONNECTION_STRING="User Id=adexpr03_proc_8; Password=lnampt8; Data Source=adexpr03.pige; Pooling=False;Connection Timeout=120;";
        public const string CREATIVE_CONNECTION_STRING = "User Id=CREATIVE3; Password=EXPLOV3;  Data Source=" + TNSName.CUSTOMER_TNS_NAME + " ;Pooling=true; Max Pool Size=150; Decr Pool Size=20; Connection Timeout=120";
		#endregion
	}

	#endregion

	#region Classe TNS Names
	/// <summary>
	/// Constantes des Tns names des bases oracle
	/// </summary>
	public class TNSName{

		#region Constantes
		/// <summary>
		/// Chaîne de connexion à la base de données pour les textes du site AdExpress
		/// </summary>
		public const string CUSTOMER_TNS_NAME="adexpr03.pige";		

		#endregion
	}

	#endregion
	
	#region Classes des noms de vues
	/// <summary>
	/// Classe de noms de vues
	/// </summary>
	public class Views {
		/// <summary>
		/// Préfixe pour la table PRODUCT_GROUP_ADVER_AGENCY_PREFIXE
		/// </summary>
		public const string PRODUCT_GROUP_ADVER_AGENCY_PREFIXE="pga";
	}
	#endregion

	#region Classe des Noms de tables et de champs
	/// <summary>
	/// Constantes des noms de table de la base de données pour le site Web AdExpress
	/// </summary>
	public class Tables{

		#region Constantes
		/// <summary>
		/// la table des listes de niveaux de détail sauvegardées
		/// </summary>
		public const string LIST="list";
		/// <summary>
		/// Préfixe pour la table des listes de niveaux de détail sauvegardées
		/// </summary>
		public const string LIST_PREFIXE="lst";
		/// <summary>
		/// la table des niveaux de détail des listes
		/// </summary>
		public const string LIST_DETAIL="list_detail";
		/// <summary>
		/// Préfixe pour la table des niveaux de détail des listes
		/// </summary>
		public const string LIST_DETAIL_PREFIXE="lstd";
		/// <summary>
		/// la table tracking
		/// </summary>
		public const string TRACKING="tracking";
		/// <summary>
		/// Préfixe pour la table tracking
		/// </summary>
		public const string TRACKING_PREFIXE="tr";
		/// <summary>
		/// la table result (MAU01)
		/// </summary>
		public const string RESULT="result";
		/// <summary>
		/// Préfixe pour la table result (MAU01)
		/// </summary>
		public const string RESULT_PREFIXE="rs";
		/// <summary>
		/// Préfixe pour la table recap
		/// </summary>
		public const string RECAP_PREFIXE="rcp";
		/// <summary>
		/// Préfixe pour la table sector
		/// </summary>
		public const string SECTOR_PREFIXE="sc";
		/// <summary>
		/// Préfixe pour la table subsector
		/// </summary>
		public const string SUBSECTOR_PREFIXE="ss";
		/// <summary>
		/// Préfixe pour la table group_
		/// </summary>
		public const string GROUP_PREFIXE="gr";
		/// <summary>
		/// Préfixe pour la table segment
		/// </summary>
		public const string SEGMENT_PREFIXE="sg";
		/// <summary>
		/// Préfixe pour la table product
		/// </summary>
		public const string PRODUCT_PREFIXE="pr";
		/// <summary>
		/// Préfixe pour la table holding
		/// </summary>
		public const string HOLDING_PREFIXE="hd";
		/// <summary>
		/// Préfixe pour la table advertiser
		/// </summary>
		public const string ADVERTISER_PREFIXE="ad";
		/// <summary>
		/// Préfixe pour la table brand
		/// </summary>
		public const string BRAND_PREFIXE="br";
		/// <summary>
		/// préfixe Template
		/// </summary>
		public const string TEMPLATE_PREFIXE="te";
		/// <summary>
		/// préfixe template_assignment
		/// </summary>
		public const string TEMPLATE_ASSIGNMENT_PREFIXE="ta";
		/// <summary>
		/// préfixe type_media
		/// </summary>
		public const string TYPE_MEDIA_PREFIXE="tm";
		/// <summary>
		/// préfixe type_product
		/// </summary>
		public const string TYPE_PRODUCT_PREFIXE="tp";
		/// <summary>
		/// Vehicle Table Name
		/// </summary>
		public const string VEHICLE="vehicle";
		/// <summary>
		/// préfixe Vehicle
		/// </summary>
		public const string VEHICLE_PREFIXE="vh";
		/// <summary>
		/// Category Table Name
		/// </summary>
		public const string CATEGORY="category";
		/// <summary>
		/// préfixe Category
		/// </summary>
		public const string CATEGORY_PREFIXE="ct";
		/// <summary>
		/// préfixe basic_media
		/// </summary>
		public const string BASIC_MEDIA_PREFIXE="bm";
		/// <summary>
		/// préfixe Media_agency
		/// </summary>
		public const string MEDIA_AGENCY_PREFIXE="ma";
		/// <summary>
		/// préfixe Data_Media_agency
		/// </summary>
		public const string DATA_MEDIA_AGENCY_PREFIXE="dma";
		/// <summary>
		/// Media Table Name
		/// </summary>
		public const string MEDIA="media"; 
		/// <summary>
		/// préfixe Media
		/// </summary>
		public const string MEDIA_PREFIXE="md";
		/// <summary>
		/// INTEREST_CENTER Table Name
		/// </summary>
		public const string INTEREST_CENTER="interest_center";
		/// <summary>
		/// préfixe INTEREST_CENTER
		/// </summary>
		public const string INTEREST_CENTER_PREFIXE="ic";
		/// <summary>
		/// préfixe Régie
		/// </summary>
		public const string MEDIA_SELLER_PREFIXE="ms";
		/// <summary>
		/// préfixe Régie
		/// </summary>
		public const string MEDIA_SELLER="media_seller";
		/// <summary>
		/// préfixe order_template_media
		/// </summary>
		public const string ORDER_TEMPLATE_MEDIA_PREFIXE="otm";
		/// <summary>
		/// préfixe order_template_product
		/// </summary>
		public const string ORDER_TEMPLATE_PRODUCT_PREFIXE="otp";
		/// <summary>
		/// préfixe gad
		/// </summary>
		public const string GAD_PREFIXE="gd";
		/// <summary>
		/// préfixe des tables web_plan_media_month et web_plan_media_week
		/// </summary>
		public const string WEB_PLAN_PREFIXE="wp";
		/// <summary>
		/// préfixe de la table title
		/// </summary>		
		public const string TITLE_PREFIXE="ti";
		/// <summary>
		/// Préfixe des tables tableaux de bord
		/// </summary>
		public const string DASH_BOARD_PREFIXE="dshb";		
		/// <summary>
		/// Table de session pour les sorties PDF
		/// </summary>
		public const string PDF_SESSION="static_nav_session";
		/// <summary>
		/// Table de session du site AdExpress
		/// </summary>
		public const string TABLE_SESSION="nav_session";
		/// <summary>
		/// Table de session du site AdExpress
		/// </summary>
		public const string MY_SESSION = "my_session";
		/// <summary>
		/// Table de session du site AdExpress
		/// </summary>
		public const string MY_SESSION_BACKUP="my_session";
		/// <summary>
		/// Table de session du site AdExpress
		/// </summary>
		public const string MY_SESSION_TEST="my_session_test";

		/// <summary>
		/// Table de sauvegarde des univers
		/// </summary>
		public const string UNIVERSE_CLIENT="universe_client";
		/// <summary>
		/// Table de sauvegarde des univers
		/// </summary>
		public const string UNIVERSE_CLIENT_TEST="universe_client_test";
		/// <summary>
		/// Table de sauvegarde des descriptipons d'univers
		/// </summary>
		public const string UNIVERSE_CLIENT_DESCRIPTION="universe_client_description";
		/// <summary>
		/// Table de sauvegarde des groupes d'univers
		/// </summary>
		public const string GROUP_UNIVERSE_CLIENT="group_universe_client";
		/// <summary>
		/// Table de liens entre les annonceurs et les adresse GAD
		/// </summary>
		public const string GAD ="gad";
        /// <summary>
        /// Table de liens entre les annonceurs et les adresse LEFAC
        /// </summary>
        public const string LEFAC = "fac";
        /// <summary>
        /// Table Agrégée des données du plan média par mois
        /// </summary>
        public const string WEB_PLAN_MEDIA_MONTH="web_plan_media_month";
		/// <summary>
		/// Table Agrégée des données du plan média par semaine
		/// </summary>
        public const string WEB_PLAN_MEDIA_WEEK = "web_plan_media_week";
		/// <summary>
		/// Table Agrégée des données du plan média APPM par mois
		/// </summary>
		public const string WEB_PLAN_APPM_MONTH="web_plan_appm_month";
		/// <summary>
		/// Table Agrégée des données du plan média APPM par semaine
		/// </summary>
		public const string WEB_PLAN_APPM_WEEK="web_plan_appm_week";
		/// <summary>
		/// Table des données désagrégées de presse inter
		/// </summary>
        public const string DATA_PRESS_INTER = "data_press";
        //public const string DATA_PRESS_INTER = "data_press";
		/// <summary>
		/// Préfixe des tables de presse inter
		/// </summary>
		public const string DATA_PRESS_INTER_PREFIXE="dpi";
		/// <summary>
		/// Table des données désagrégées de presse
		/// </summary>
        public const string DATA_PRESS = "data_press";
        //public const string DATA_PRESS = "data_press";
		/// <summary>
		/// Préfixe des tables de presse
		/// </summary>
		public const string DATA_PRESS_PREFIXE="dp";
		/// <summary>
		/// Table des données désagrégé
		/// </summary>
		public const string DATA_PRESS_APPM ="data_press_appm";		
		/// <summary>
		/// Préfixe des tables de presse
		/// </summary>
		public const string DATA_PRESS_APPM_PREFIXE="dpm";
		/// <summary>
		/// Table des données désagrégées de radio
		/// </summary>
		public const string DATA_RADIO ="data_radio";
		/// <summary>
		/// Table des données désagrégées d'adnettrack
		/// </summary>
		public const string DATA_ADNETTRACK ="data_adnettrack";
		/// <summary>
		/// Table des données désagrégées de tele
		/// 		/// </summary>
		public const string DATA_TV ="data_tv";
		/// <summary>
		/// Table des données désagrégées de publicité exterieure
		/// </summary>
		public const string DATA_OUTDOOR ="data_outdoor";
        /// <summary>
        /// Table des données désagrégées de publicité insterrieur
        /// </summary>
        public const string DATA_INSTORE = "data_instore";
		/// <summary>
		/// Table des données de parrainage
		/// </summary>
		public const string DATA_SPONSORSHIP ="data_sponsorship";
		/// <summary>
		/// Table des données de Internet
		/// </summary>
		public const string DATA_INTERNET ="data_internet";
        /// <summary>
        /// Préfixe de la table data_internet
        /// </summary>
        public const string DATA_INTERNET_PREFIXE = "di";
        /// <summary>
        /// Table des données du Marketing Direct
        /// </summary>
        public const string DATA_MARKETING_DIRECT="data_marketing_direct";

		/// <summary>
		/// Préfixe des tables de parrainage
		/// </summary>
		public const string DATA_SPONSORSHIP_PREFIXE="dsp";
		/// <summary>
		/// Table des données désagrégées de presse inter pour 4 mois
		/// </summary>
        public const string ALERT_DATA_PRESS_INTER = "data_press_4M";
        //public const string ALERT_DATA_PRESS_INTER = "data_press_4M";
		/// <summary>
		/// Table des données désagrégées de presse pour 4 mois
		/// </summary>
        public const string ALERT_DATA_PRESS = "data_press_4M";
        //public const string ALERT_DATA_PRESS = "data_press_4M";
		/// <summary>
		/// Table des données désagrégées d'adnettrack pour 4 mois
		/// </summary>
		public const string ALERT_DATA_ADNETTRACK ="data_adnettrack_4M"; 
		/// <summary>
		/// Table des données désagrégées de radio pour 4 mois
		/// </summary>
		public const string ALERT_DATA_RADIO ="data_radio_4M";
		//public const string ALERT_DATA_RADIO ="data_radio_rechargement";
		/// <summary>
		/// Table des données désagrégées de tele pour 4 mois
		/// </summary>
		public const string ALERT_DATA_TV ="data_tv_4M";
		/// <summary>
		/// Table des données désagrégées de publicité exterieure pour 4 mois
		/// </summary>
		public const string ALERT_DATA_OUTDOOR ="data_outdoor_4M";
		/// <summary>
		/// Table des données désagrégées de internet pour 4 mois
		/// </summary>
		public const string ALERT_DATA_INTERNET ="data_internet_4M";
        /// <summary>
        /// Table des données désagrégées du marketing direct pour 4 mois
        /// </summary>
        public const string ALERT_DATA_MARKETING_DIRECT = "data_marketing_direct_4M";
		/// <summary>
		/// Table des couleurs
		/// </summary>
		public const string COLOR = "color";
		/// <summary>
		/// Table des couleurs
		/// </summary>
		public const string COLOR_PREFIXE = "cl";
		/// <summary>
		/// Table des formats
		/// </summary>
		public const string FORMAT = "format";
		/// <summary>
		/// Table des formats
		/// </summary>
		public const string FORMAT_PREFIXE = "fo";
        /// <summary>
        /// Table des mails format
        /// </summary>
        public const string MAIL_FORMAT = "mail_format";
        /// <summary>
        /// Préfixe de la table mail_format
        /// </summary>
        public const string MAIL_FORMAT_PREFIXE = "mf";
        /// <summary>
        /// Table des mails type
        /// </summary>
        public const string MAIL_TYPE = "mail_type";
        /// <summary>
        /// Préfixe de la table mail_type
        /// </summary>
        public const string MAIL_TYPE_PREFIXE = "mt";
        /// <summary>
        /// Table des mails content
        /// </summary>
        public const string MAIL_CONTENT = "mail_content";
        /// <summary>
        /// Préfixe de la table mail_content
        /// </summary>
        public const string MAIL_CONTENT_PREFIXE = "mc";
        /// <summary>
        /// Table DATA_MAIL_CONTENT
        /// </summary>
        public const string DATA_MAIL_CONTENT = "data_mail_content";
        /// <summary>
        /// Préfixe de la table DATA_MAIL_CONTENT
        /// </summary>
        public const string DATA_MAIL_CONTENT_PREFIXE = "dmc";
        /// <summary>
        /// Table des affranchissements
        /// </summary>
        public const string MAILING_RAPIDITY = "mailing_rapidity";
        /// <summary>
        /// Préfixe de la table des affranchissements
        /// </summary>
        public const string MAILING_RAPIDITY_PREFIXE = "mr";
		/// <summary>
		/// Table des locations
		/// </summary>
		public const string LOCATION = "location";
		/// <summary>
		/// Table des locations
		/// </summary>
		public const string LOCATION_PREFIXE = "lo";
		/// <summary>
		/// Table liant data_press a location
		/// </summary>
		public const string DATA_LOCATION = "data_location";
		/// <summary>
		/// Table liant data_press a location
		/// </summary>
		public const string DATA_LOCATION_PREFIXE = "dl";
		/// <summary>
		/// Table des encarts
		/// </summary>
		public const string INSERT = "inset";
		/// <summary>
		/// Table des insertions
		/// </summary>
		public const string INSERTION = "insertion";
		/// <summary>
		/// Table des insertions
		/// </summary>
		public const string INSERTION_PREFIXE = "dt";
		/// <summary>
		/// Table famille
		/// </summary>
		public const string SECTOR = "sector";
		/// <summary>
		/// Table classe
		/// </summary>
		public const string SUBSECTOR = "subsector";
		/// <summary>
		/// Table des groups
		/// </summary>
		public const string GROUP_="group_";
		/// <summary>
		/// Table variété
		/// </summary>
		public const string SEGMENT = "segment";
		/// <summary>
		/// Table produit
		/// </summary>
		public const string PRODUCT = "product";
		/// <summary>
		/// Table annonceur
		/// </summary>
		public const string ADVERTISER = "advertiser";
		/// <summary>
		/// Table brand
		/// </summary>
		public const string BRAND = "brand";
		/// <summary>
		/// Table alarm_media
		/// </summary>
		public const string ALARM_MEDIA = "alarm_media";
		/// <summary>
		/// Table alarm_media
		/// </summary>
		public const string ALARM_MEDIA_PREFIXE = "al";
		/// <summary>
		/// Table application_media
		/// </summary>
		public const string APPLICATION_MEDIA="application_media";
		/// <summary>
		/// Prefixe Table application_media
		/// </summary>
		public const string APPLICATION_MEDIA_PREFIXE="appliMd";
		/// <summary>
		/// Table des données désagrégées de PLURIMEDIA
		/// </summary>
		public const string RECAP_PLURI ="recap_pluri";
		/// <summary>
		/// Table des données désagrégées de tv
		/// </summary>
		public const string RECAP_TV ="recap_tv";
		/// <summary>
		/// Table des données désagrégées de cinema
		/// </summary>
		public const string RECAP_CINEMA ="recap_cinema";
		/// <summary>
		/// Table des données désagrégées de internet
		/// </summary>
		public const string RECAP_INTERNET ="recap_internet";
		/// <summary>
		/// Table des données désagrégées de publicité exterieur
		/// </summary>
		public const string RECAP_OUTDOOR ="recap_outdoor";
		/// <summary>
		/// Table des données désagrégées de PRESSE
		/// </summary>
		public const string RECAP_PRESS ="recap_press";
		/// <summary>
		/// Table des données désagrégées de RADIO
		/// </summary>
		public const string RECAP_RADIO ="recap_radio";
		/// <summary>
		/// Table des données désagrégées de Media Tactique
		/// </summary>
		public const string RECAP_MEDIA_TACTIC ="recap_tactic";
		/// <summary>
		/// Table des données désagrégées de PLURIMEDIA niveau Media/segment
		/// </summary>
		public const string RECAP_PLURI_SEGMENT ="recap_pluri_segment";
		/// <summary>
		/// Table des données désagrégées de tv niveau Media/segment
		/// </summary>
		public const string RECAP_TV_SEGMENT ="recap_tv_segment";
		/// <summary>
		/// Table des données désagrégées de cinema niveau Media/segment
		/// </summary>
		public const string RECAP_CINEMA_SEGMENT ="recap_cinema_segment";
		/// <summary>
		/// Table des données désagrégées de internet niveau Media/segment
		/// </summary>
		public const string RECAP_INTERNET_SEGMENT ="recap_internet_segment";
		/// <summary>
		/// Table des données désagrégées de publicité exterieur niveau Media/segment
		/// </summary>
		public const string RECAP_OUTDOOR_SEGMENT ="recap_outdoor_segment";
		/// <summary>
		/// Table des données désagrégées de PRESSE niveau Media/segment
		/// </summary>
		public const string RECAP_PRESS_SEGMENT ="recap_press_segment";
		/// <summary>
		/// Table des données désagrégées de RADIO niveau Media/segment
		/// </summary>
		public const string RECAP_RADIO_SEGMENT ="recap_radio_segment";	 
		/// <summary>
		/// Table des données désagrégées de Media Tactique niveau Media/segment
		/// </summary>
		public const string RECAP_MEDIA_TACTIC_SEGMENT ="recap_tactic_segment";
		/// <summary>
		/// Table des données désagrégées de téléphonie mobile
		/// </summary>
		public const string RECAP_MOBILE_TELEPHONY = "recap_message";
		/// <summary>
		/// Table des données désagrégées de téléphonie mobile niveau Media/segment
		/// </summary>
		public const string RECAP_MOBILE_TELEPHONY_SEGMENT = "recap_message_segment";
		/// <summary>
		/// Table des données désagrégées de l'emailing
		/// </summary>
		public const string RECAP_EMAILING = "recap_emailing";
		/// <summary>
		/// Table des données désagrégées de l'emailing
		/// </summary>
		public const string RECAP_EMAILING_SEGMENT = "recap_emailing_segment";	 
		/// <summary>
		/// Table périodicity
		/// </summary>
		public const string PERIODICITY = "periodicity";
		/// <summary>
		/// Table périodicity
		/// </summary>
		public const string PERIODICITY_PREFIXE= "prc";
		/// <summary>
		/// Table des vagues Grp
		/// </summary>
		public const string WAVE="wave";
		/// <summary>
		/// Préfixe des tables de vague AEPM
		/// </summary>
		public const string WAVE_PREFIXE="wv";
		/// <summary>
		/// Préfixe des tables de cible AEPM
		/// </summary>
		public const string TARGET_PREFIXE="trg";
		/// <summary>
		/// Préfixe des tables de cible AEPM
		/// </summary>
		public const string TARGET_MEDIA_ASSIGNEMNT_PREFIXE="tma";
		/// <summary>
		/// Table des cibles AEPM
		/// </summary>
		public const string TARGET="target";
		/// <summary>
		/// Target Media Assignment table name
		/// </summary>
		public const string TARGET_MEDIA_ASSIGNEMNT="target_media_assignment";
		/// <summary>
		/// Tables agences média
		/// </summary>
		public const string PRODUCT_GROUP_ADV_AGENCY="PRODUCT_AGENCY_";
		/// <summary>
		/// Table des logins
		/// </summary>
		public const string LOGIN="LOGIN";
		/// <summary>
		/// préfixe Table des logins
		/// </summary>
		public const string LOGIN_PREFIXE="log";
		/// <summary>
		/// Table des contacts
		/// </summary>
		public const string CONTACT="CONTACT";
		/// <summary>
		/// préfixe Table des contacts
		/// </summary>
		public const string CONTACT_PREFIXE="ctt";
		/// <summary>
		/// préfixe Table des groupes de contacts
		/// </summary>
		public const string GROUP_CONTACT_PREFIXE="gct";
		/// <summary>
		/// Table des adresses
		/// </summary>
		public const string ADDRESS="ADDRESS";
		/// <summary>
		///préfixe Table des adresses
		/// </summary>
		public const string ADDRESS_PREFIXE="adr";
		/// <summary>
		/// Table des sociétés
		/// </summary>
		public const string COMPANY="COMPANY";
		/// <summary>
		/// préfixes Table sociétés
		/// </summary>
		public const string COMPANY_PREFIXE="cpn";

		/// <summary>
		/// Table des connection par login
		/// </summary>
		public const string CONNECTION_BY_LOGIN="CONNECTION_BY_LOGIN";
		/// <summary>
		/// préfixes Table des connections par login
		/// </summary>
		public const string CONNECTION_BY_LOGIN_PREFIXE="cnxlog";	
	
		/// <summary>
		/// Table des MODULES
		/// </summary>
		public const string MODULE="MODULE";
		/// <summary>
		/// préfixes Table MODULES
		/// </summary>
		public const string MODULE_PREFIXE="mdl";
		/// <summary>
		/// Table des GROUPES DE MODULES
		/// </summary>
		public const string MODULE_GROUP="MODULE_GROUP";
		/// <summary>
		/// préfixes Table GROUPES DE MODULES
		/// </summary>
		public const string MODULE_GROUP_PREFIXE="mdlg";
		/// <summary>
		/// Table des TOP DE MODULES
		/// </summary>
		public const string TOP_MODULE="TOP_MODULE";
		/// <summary>
		/// préfixes Table TOP DE MODULES
		/// </summary>
		public const string TOP_MODULE_PREFIXE="tmdl";
		/// <summary>
		/// Table des TOP d'utilisation fichier GAD
		/// </summary>
		public const string TOP_GAD="TOP_GAD";
		/// <summary>
		/// préfixes Table d'utilisation fichier GAD
		/// </summary>
		public const string TOP_GAD_PREFIXE="tg";
		/// <summary>
		/// Table des TOP d'utilisation fichier des agences média
		/// </summary>
		public const string TOP_MEDIA_AGENCY="TOP_MEDIA_AGENCY";
		/// <summary>
		/// préfixes Table d'utilisation fichier des agences média
		/// </summary>
		public const string TOP_MEDIA_AGENCY_PREFIXE="tma";
		/// <summary>
		/// Table des TOP export excel
		/// </summary>
		public const string TOP_EXPORT_EXCEL="TOP_EXPORT_EXCEL";
		/// <summary>
		/// préfixes Table des TOP export excel
		/// </summary>
		public const string TOP_EXPORT_EXCEL_PREFIXE="txls";

		/// <summary>
		/// Table des TOP des options
		/// </summary>
		public const string TOP_OPTION="TOP_OPTION";
		/// <summary>
		/// préfixes Table des TOP otpions
		/// </summary>
		public const string TOP_OPTION_PREFIXE="topt";

		/// <summary>
		/// Table des TOP unités utilisées
		/// </summary>
		public const string TOP_UNIT="TOP_UNIT";
		/// <summary>
		/// préfixes Table des TOP  unités utilisées
		/// </summary>
		public const string TOP_UNIT_PREFIXE="tun";

		/// <summary>
		/// Table des TOP périodes utilisées
		/// </summary>
		public const string TOP_PERIODE="TOP_EXPORT_EXCEL";
		/// <summary>
		/// préfixes Table des TOP périodes utilisées
		/// </summary>
		public const string TOP_PERIODE_PREFIXE="tpd";

		/// <summary>
		/// Table UNITES utilisées
		/// </summary>
		public const string UNIT="UNIT";

		/// <summary>
		/// préfixes Table UNITES utilisées
		/// </summary>
		public const string UNIT_PREFIXE="ut";

		/// <summary>
		/// Table periodes utilisées
		/// </summary>
		public const string PERIODE="PERIODE";
		/// <summary>
		/// préfixes Table periodes utilisées
		/// </summary>
		public const string PERIODE_PREFIXE="pd";
		/// <summary>
		/// Table médias utilisées
		/// </summary>
		public const string TOP_VEHICLE="TOP_VEHICLE";
		/// <summary>
		/// préfixes Table médias utilisées
		/// </summary>
		public const string TOP_VEHICLE_PREFIXE="tpv";
		/// <summary>
		/// Table médias utilisées
		/// </summary>
		public const string TOP_MY_ADEXPRESS="TOP_MY_ADEXPRESS";
		/// <summary>
		/// préfixes Table médias utilisées
		/// </summary>
		public const string TOP_MY_ADEXPRESS_PREFIXE="tpa";
		/// <summary>
		/// Table médias utilisées par module
		/// </summary>
		public const string TOP_VEHICLE_BY_MODULE="TOP_VEHICLE_BY_MODULE";
		/// <summary>
		/// préfixes Table médias utilisées par module
		/// </summary>
		public const string TOP_VEHICLE_BY_MODULE_PREFIXE="tvm";
		/// <summary>
		/// Table IP adresse par client
		/// </summary>
		public const string IP_BY_LOGIN="IP_BY_LOGIN";
		/// <summary>
		/// préfixes Table IP adresse par client
		/// </summary>
		public const string IP_BY_LOGIN_PREFIXE="ipl";
		
		/// <summary>
		/// Table des durées moyenne des connections
		/// </summary>
		public const string CONNECTION_TIME="CONNECTION_TIME";
		/// <summary>
		/// préfixes Table des durées moyenne des connections
		/// </summary>
		public const string CONNECTION_TIME_PREFIXE="cnxt";	
		/// <summary>
		/// préfixes Table AGGLOMERATION
		/// </summary>
		public const string AGGLOMERATION_PREFIXE="agglo";
		/// <summary>
		/// Table SLOGAN
		/// </summary>
		public const string SLOGAN="SLOGAN";
		/// <summary>
		/// préfixes Table SLOGAN
		/// </summary>
		public const string SLOGAN_PREFIXE="slg";

		/// <summary>
		/// Table ALERT_PUSH_MAIL
		/// </summary>
		public const string ALERT_PUSH_MAIL="alert_push_mail";
		/// <summary>
		/// Préfixe de la table ALERT_PUSH_MAIL
		/// </summary>
		public const string ALERT_PUSH_MAIL_PREFIXE="apm";
		/// <summary>
		/// Table ALERT
		/// </summary>
		public const string ALERT="alert";
        /// <summary>
        /// Table ALERT_OCCURRENCE
        /// </summary>
        public const string ALERT_OCCURENCE = "alert_occurence";
		/// <summary>
		/// Préfixe de la table ALERT
		/// </summary>
		public const string ALERT_PREFIXE="al";
		/// <summary>
		/// Table ALERT_FLAG_ASSIGNMENT
		/// </summary>
		public const string ALERT_FLAG_ASSIGNMENT="alert_flag_assignment";
		/// <summary>
		/// Préfixe de la table ALERT_FLAG_ASSIGNMENT
		/// </summary>
		public const string ALERT_FLAG_ASSIGNMENT_PREFIXE="afa";
		/// <summary>
		/// Table ALERT_UNIVERSE_ASSIGNMENT
		/// </summary>
		public const string ALERT_UNIVERSE_ASSIGNMENT="alert_universe_assignment";
		/// <summary>
		/// Préfixe de la table ALERT_UNIVERSE_ASSIGNMENT
		/// </summary>
		public const string ALERT_UNIVERSE_ASSIGNMENT_PREFIXE="aua";
		/// <summary>
		/// Table ALERT_UNIVERSE
		/// </summary>
		public const string ALERT_UNIVERSE="alert_universe";
		/// <summary>
		/// Préfixe de la table ALERT_UNIVERSE
		/// </summary>
		public const string ALERT_UNIVERSE_PREFIXE="au";
		/// <summary>
		/// Table ALERT_UNIVERSE_DETAIL
		/// </summary>
		public const string ALERT_UNIVERSE_DETAIL="alert_universe_detail";
		/// <summary>
		/// Préfixe de la table ALERT_UNIVERSE_DETAIL
		/// </summary>
		public const string ALERT_UNIVERSE_DETAIL_PREFIXE="aud";

		/// <summary>
		/// préfixe de la table COUNTRY
		/// </summary>		
		public const string COUNTRY_PREFIXE="ctr";

		/// <summary>
		/// Table COUNTRY
		/// </summary>
		public const string COUNTRY="country";

		/// <summary>
		/// Préfixe table ADVERTISING_AGENCY_PREFIXE
		/// </summary>
		public const string ADVERTISING_AGENCY_PREFIXE = "adva";
		/// <summary>
		/// Table ADVERTISING_AGENCY
		/// </summary>
		public const string ADVERTISING_AGENCY = "ADVERTISING_AGENCY";
		/// <summary>
		/// Préfixe table GROUP_ADVERTISING_AGENCY_PREFIXE
		/// </summary>
		public const string GROUP_ADVERTISING_AGENCY_PREFIXE = "gadv";
		/// <summary>
		/// Préfixe table GROUP_ADVERTISING_AGENCY
		/// </summary>
		public const string GROUP_ADVERTISING_AGENCY = "GROUP_ADVERTISING_AGENCY";
		#endregion
	}

	/// <summary>
	/// Représente les champs de la base de données pour le site Web AdExpress
	/// </summary>
	public class Fields
	{

		#region Constantes
		/// <summary>
		/// Champ date de la table web_plan_media_month
		/// </summary>
		public const string WEB_PLAN_MEDIA_MONTH_DATE_FIELD="month_media_num";
		/// <summary>
		/// Champ date de la table web_plan_media_week
		/// </summary>
		public const string WEB_PLAN_MEDIA_WEEK_DATE_FIELD="week_media_num";
		/// <summary>
		/// Champ du total euro des tables agrégées pour le plan média
		/// </summary>
		public const string WEB_PLAN_MEDIA_MONTH_EURO_FIELD="totalunite";
		/// <summary>
		/// Champ du total mmc des tables agrégées pour le plan média
		/// </summary>
		public const string WEB_PLAN_MEDIA_MONTH_MMC_FIELD="totalmmc";
		/// <summary>
		/// Champ du total pages des tables agrégées pour le plan média
		/// </summary>
		public const string WEB_PLAN_MEDIA_MONTH_PAGES_FIELD="totalpages";
		/// <summary>
		/// Champ du total insertions-spots des tables agrégées pour le plan média
		/// </summary>
		public const string WEB_PLAN_MEDIA_MONTH_INSERT_FIELD="totalinsert";
		/// <summary>
		/// Champ du total durée des tables agrégées pour le plan média
		/// </summary>
		public const string WEB_PLAN_MEDIA_MONTH_DUREE_FIELD="totalduree";
        /// <summary>
        /// Champ du total volume des tables agrégées pour le plan média
        /// </summary>
        public const string WEB_PLAN_MEDIA_MONTH_VOLUME_FIELD="totalvolume";

        /// <summary>
        /// Champ volume d'une version de la table data_marketing_direct
        /// </summary>
        public const string VOLUME = "VOLUME";

		/// <summary>
		/// Champ dépense en fcs des tables data-press, data_radio et data_tv
		/// </summary>
		public const string EXPENDITURE_FF="EXPENDITURE_FF";

		/// <summary>
		/// Champ dépense en €uros des tables data-press, data_radio et data_tv
		/// </summary>
		public const string EXPENDITURE_EURO="EXPENDITURE_EURO";

		/// <summary>
		/// Champ durée d'un spot des tables data_radio et data_tv
		/// </summary>
		public const string DURATION="DURATION";

		/// <summary>
		/// Champ nombre d'insertions des tables data_press, data_radio et data_tv
		/// </summary>
		public const string INSERTION="INSERTION";

		/// <summary>
		/// Champ nombre de page de l'insertion de la tables data_press
		/// </summary>
		public const string AREA_PAGE="AREA_PAGE";
		
		/// <summary>
		/// Champ mm/cc de la tables data_press
		/// </summary>
		public const string AREA_MMC="AREA_MMC";
		/// <summary>
		/// champ nombre_panneaux de la table data_outdoor
		/// </summary>
		public const string NUMBER_BOARD="NUMBER_BOARD";	
		/// <summary>
		/// Champ nombre de grp
		/// </summary>
		public const string GRP="GRP";
		/// <summary>
		/// champ des groups
		/// </summary>
		public const string GROUP_="group_";
		/// <summary>
		/// champ variété
		/// </summary>
		public const string SEGMENT = "segment";
		/// <summary>
		/// champ produit
		/// </summary>
		public const string PRODUCT = "product";
		/// <summary>
		/// champ annonceur
		/// </summary>
		public const string ADVERTISER = "advertiser";
		/// <summary>
		/// champ date
		/// </summary>
		public const string DATE_MEDIA_NUM = "date_media_num";
		/// <summary>
		/// champ TOP diffusion (radio)
		/// </summary>
		public const string ID_TOP_DIFFUSION = "ID_TOP_DIFFUSION";
		/// <summary>
		/// champ TOP diffusion (tv)
		/// </summary>
		public const string TOP_DIFFUSION = "TOP_DIFFUSION";
		/// <summary>
		/// champ position (radio)
		/// </summary>
		public const string RANK = "RANK";	
		/// <summary>
		/// champ position (tv)
		/// </summary>
		public const string ID_RANK = "ID_RANK";	
		/// <summary>
		/// champ durée
		/// </summary>
		public const string DURATION_COMMERCIAL_BREAK = "DURATION_COMMERCIAL_BREAK";
		/// <summary>
		/// champ nombre de spots
		/// </summary>
		public const string NUMBER_SPOT_COM_BREAK = "NUMBER_SPOT_COM_BREAK";
		/// <summary>
		/// champ rang hap
		/// </summary>
		public const string RANK_WAP = "RANK_WAP";		
		/// <summary>
		/// champ durée hap
		/// </summary>
		public const string DURATION_COM_BREAK_WAP = "DURATION_COM_BREAK_WAP";
		/// <summary>
		/// champ nombre spot hap
		/// </summary>
		public const string NUMBER_SPOT_COM_BREAK_WAP = "NUMBER_SPOT_COM_BREAK_WAP";
		/// <summary>
		/// champ nombre spot 
		/// </summary>
		public const string NUMBER_MESSAGE_COMMERCIAL_BREA = "NUMBER_MESSAGE_COMMERCIAL_BREA";		
		/// <summary>
		/// champ durée 
		/// </summary>
		public const string ID_COMMERCIAL_BREAK_INDEX = "ID_COMMERCIAL_BREAK_INDEX";		
		/// <summary>
		/// champ réseau afficheur
		/// </summary>
		public const string POSTER_NETWORK = "POSTER_NETWORK";
		/// <summary>
		/// champ type de réseau
		/// </summary>
		public const string TYPE_SALE = "TYPE_SALE";
		/// <summary>
		/// champ type de panneaux
		/// </summary>
		public const string TYPE_BOARD = "TYPE_BOARD";	
		/// <summary>
		/// Champ identifiant accroche
		/// </summary>
		public const string ID_SLOGAN="id_slogan";	
		/// <summary>
		/// Champ identifiant régie
		/// </summary>
		public const string ID_MEDIA_SELLER="id_media_seller";	
		/// <summary>
		/// Champ identifiant support
		/// </summary>
		public const string ID_MEDIA="id_media";	
		/// <summary>
		/// Champ identifiant catégorie
		/// </summary>
		public const string ID_CATEGORY="id_category";
		/// <summary>
		/// Champ identifiant média
		/// </summary>
		public const string ID_VEHICLE="id_vehicle";
		/// <summary>
		/// Champ identifiant centre d'intérêts
		/// </summary>
		public const string ID_INTEREST_CENTER="id_interest_center";	
		/// <summary>
		/// Champ identifiant annnonceur
		/// </summary>
		public const string ID_ADVERTISER="id_advertiser";
		/// <summary>
		/// Champ identifiant marque
		/// </summary>
		public const string ID_BRAND="id_brand";
		/// <summary>
		/// Champ identifiant produit
		/// </summary>
		public const string ID_PRODUCT="id_product";
		/// <summary>
		/// Champ identifiant famille
		/// </summary>
		public const string ID_SECTOR="id_sector";
		/// <summary>
		/// Champ identifiant classe
		/// </summary>
		public const string ID_SUBSECTOR="id_subsector";
		/// <summary>
		/// Champ identifiant groupe
		/// </summary>
		public const string ID_GROUP_="id_group_";
        /// <summary>
        /// Champ identifiant support
        /// </summary>
        public const string ID_BASIC_MEDIA = "id_basic_media";	
		#endregion
	
	}
	/// <summary>
	/// Représente les Valeurs du champ Type_Sale pour affichage
	/// </summary>
	public class TypeSaleOutdoor
	{
		/// <summary>
		/// valeur du champ Type_sale pour affichage
		/// </summary>
		public const string NATIONALE="N";
		/// <summary>
		/// valeur du champ Type_sale pour affichage
		/// </summary>
		public const string REGIONALE="R";
		/// <summary>
		/// valeur du champ Type_sale pour affichage
		/// </summary>
		public const string LOCALE="L";
		/// <summary>
		/// valeur du champ Type_sale pour affichage
		/// </summary>
		public const string MIXTE="M";
	
	}

	#endregion

    #region Classe des Types de tables
    /// <summary>
    /// Type d'une table (WEB_PLAN, DATA_VEHICLE, DATA_VEHICLE_4M)
    /// </summary>
    public class TableType {
        /// <summary>
        /// Type de la table
        /// </summary>
        public enum Type {
            /// <summary>
            /// DATA_VEHICLE_4M
            /// </summary>
            dataVehicle4M,
            /// <summary>
            /// DATA_VEHICLE
            /// </summary>
            dataVehicle,
            /// <summary>
            /// WEB_PLAN
            /// </summary>
            webPlan
        }
    }
    #endregion

    #region Classe des valeurs des supports et des catégories
    /// <summary>
    /// Constantes des valeurs des supports
    /// </summary>
    public class Media{

        #region constantes
        /// <summary>
        /// La valeur du média courrier adressé genaral dans la base de données
        /// </summary>
        public const string COURRIER_ADRESSE_GENERAL = "5602";
        /// <summary>
        /// la valeur du média courrier adressé presse dans la base de données
        /// </summary>
        public const string COURRIER_ADRESSE_PRESSE = "5631";
        /// <summary>
        /// la valeur du média courrier adressé gestion dans la base de données
        /// </summary>
        public const string COURRIER_ADRESSE_GESTION = "5630";
        /// <summary>
        /// la valeur du média publicité non adressée dans la base de données
        /// </summary>
        public const string PUBLICITE_NON_ADRESSEE= "5657";

        /// <summary>
        /// la valeur du média "COURRIER ADRESSE"  dans la base de données
        /// </summary>
        /// <remarks>Cette valeur est associée au courrier valorisé</remarks>
        public const long COURRIER_ADRESSE_VALO = 19229;
        /// <summary>
        /// la valeur du média "IMPRIME PUBLICITAIRE"  dans la base de données
        /// </summary>
        /// <remarks>Cette valeur est associée au courrier valorisé</remarks>
        public const long IMPRIME_PUBLICITAIRE_VALO = 19230;
        #endregion

    }
    /// <summary>
    /// Constantes des valeurs des catégories
    /// </summary>
    public class Category{

        #region Constantes
        /// <summary>
        /// La valeur de la catégorie courrier adressé dans la base de données
        /// </summary>
        public const string COURRIER_ADRESSE = "70";
        /// <summary>
        /// la valeur de la gatégorie publicité non adressée dans la base de données
        /// </summary>
        public const string PUBLICITE_NON_ADRESSEE = "69";

        /// <summary>
        /// la valeur du média "COURRIER ADRESSE"  dans la base de données
        /// </summary>
        /// <remarks>Cette valeur est associée au courrier valorisé</remarks>
        public const long COURRIER_ADRESSE_VALO = 271;
        /// <summary>
        /// la valeur du média "IMPRIME PUBLICITAIRE"  dans la base de données
        /// </summary>
        /// <remarks>Cette valeur est associée au courrier valorisé</remarks>
        public const long IMPRIME_PUBLICITAIRE_VALO = 272;

        /// <summary>
        /// ID category corresponding to digital TV
        /// </summary>
        public const Int64 ID_DIGITAL_TV = 78;
		/// <summary>
		/// KEY category corresponding to digital TV
		/// </summary>
		public const string KEY_DIGITAL_TV = "KEY_DIGITAL_TV";

		/// <summary>
		/// ID category corresponding to thematic TV
		/// </summary>
		public const Int64 ID_THEMATIC_TV = 35;
		/// <summary>
		/// key category corresponding to excluded thematic TV
		/// </summary>
		public const string KEY_EXCLUDE_THEMATIC_TV = "KEY_EXCLUDE_THEMATIC_TV";

		/// <summary>
		/// ID category corresponding to Sponsorship TV
		/// </summary>
		public const Int64 ID_SPONSORSHIP_TV = 68;

		/// <summary>
		/// key category corresponding to excluded Sponsorship TV
		/// </summary>
		public const string KEY_EXCLUDE_SPONSORSHIP_TV = "KEY_EXCLUDE_SPONSORSHIP_TV";
        /// <summary>
        /// ID Category APPLICATION_MOBILE
        /// </summary>
        public const long ID_APPLICATION_MOBILE = 231;
        #endregion

    }

    /// <summary>
    /// Constantes des valeurs des formats
    /// </summary>
    public class Format{

        #region Label
        /// <summary>
        /// La valeur du format standard pour le courrier adressé general
        /// </summary>
        public const string FORMAT_STANDARD = "10";
        /// <summary>
        /// La valeur du format original pour le courrier adressé general
        /// </summary>
        public const string FORMAT_ORIGINAL = "20";
        #endregion

        #region Id
        /// <summary>
        /// La valeur du format standard pour le courrier adressé general
        /// </summary>
        public const int ID_FORMAT_STANDARD = 10;
        /// <summary>
        /// La valeur du format original pour le courrier adressé general
        /// </summary>
        public const int ID_FORMAT_ORIGINAL = 20;
        #endregion

    }

    /// <summary>
    /// Constantes des valeurs des mails contents
    /// </summary>
    public class MailContent{

        #region Label
        /// <summary>
        /// LETTRE ACCOMP. PERSONALIS
        /// </summary>
        public const string LETTRE_ACCOMP_PERSONALIS = "LETTRE ACCOMP. PERSONALIS";
        /// <summary>
        /// ENV. RETOUR PRE-IMPRIMEE
        /// </summary>
        public const string ENV_RETOUR_PRE_IMPRIMEE = "ENV. RETOUR PRE-IMPRIMEE";
        /// <summary>
        /// ENV. RETOUR A TIMBRER
        /// </summary>
        public const string ENV_RETOUR_A_TIMBRER = "ENV. RETOUR A TIMBRER";
        /// <summary>
        /// COUPON DE REDUCTION
        /// </summary>
        public const string COUPON_DE_REDUCTION = "COUPON DE REDUCTION";
        /// <summary>
        /// ECHANTILLON
        /// </summary>
        public const string ECHANTILLON = "ECHANTILLON";
        /// <summary>
        /// BON DE COMMANDE
        /// </summary>
        public const string BON_DE_COMMANDE = "BON DE COMMANDE";
        /// <summary>
        /// JEUX CONCOUR
        /// </summary>
        public const string JEUX_CONCOUR = "JEUX CONCOUR";
        /// <summary>
        /// CATALOGUE BROCHURE
        /// </summary>
        public const string CATALOGUE_BROCHURE = "CATALOGUE BROCHURE";
        /// <summary>
        /// CADEAU
        /// </summary>
        public const string CADEAU = "CADEAU";
        /// <summary>
        /// ACCELERATEUR REPONSE
        /// </summary>
        public const string ACCELERATEUR_REPONSE = "ACCELERATEUR REPONSE";
        #endregion

        #region Id
        /// <summary>
        /// LETTRE ACCOMP. PERSONALIS
        /// </summary>
        public const int ID_LETTRE_ACCOMP_PERSONALIS = 1105;
        /// <summary>
        /// ENV. RETOUR PRE-IMPRIMEE
        /// </summary>
        public const int ID_ENV_RETOUR_PRE_IMPRIMEE = 1106;
        /// <summary>
        /// ENV. RETOUR A TIMBRER
        /// </summary>
        public const int ID_ENV_RETOUR_A_TIMBRER = 1107;
        /// <summary>
        /// COUPON DE REDUCTION
        /// </summary>
        public const int ID_COUPON_DE_REDUCTION = 1108;
        /// <summary>
        /// ECHANTILLON
        /// </summary>
        public const int ID_ECHANTILLON = 1109;
        /// <summary>
        /// BON DE COMMANDE
        /// </summary>
        public const int ID_BON_DE_COMMANDE = 1110;
        /// <summary>
        /// JEUX CONCOUR
        /// </summary>
        public const int ID_JEUX_CONCOUR = 1111;
        /// <summary>
        /// CATALOGUE BROCHURE
        /// </summary>
        public const int ID_CATALOGUE_BROCHURE = 1112;
        /// <summary>
        /// CADEAU
        /// </summary>
        public const int ID_CADEAU = 1113;
        /// <summary>
        /// ACCELERATEUR REPONSE
        /// </summary>
        public const int ID_ACCELERATEUR_REPONSE = 1114;
        #endregion

    }
    #endregion

    #region schéma
    /// <summary>
	/// Schéma de la base de données
	/// </summary>
	public class Schema{
		/// <summary>
		/// Droits clients
		/// </summary>
		public const string RIGHT_SCHEMA="mau01";
		/// <summary>
		/// Univers client
		/// </summary>
		//public const string UNIVERS_SCHEMA="web_nav_01";
		public const string UNIVERS_SCHEMA="webnav01";
		/// <summary>
		/// Login
		/// </summary>
		public const string LOGIN_SCHEMA="mau01";
		/// <summary>
		/// AdExpress
		/// </summary>	
		public const string	ADEXPRESS_SCHEMA="adexpr03";
		/// <summary>
		/// Recap 
		/// </summary>	
		public const string	RECAP_SCHEMA="recap01";
		/// <summary>
		/// Sessions
		/// </summary>
		public const string APPLICATION_SCHEMA="mou01";
		/// <summary>
		///  APPM
		/// </summary>
		public const string APPM_SCHEMA="APPM01";
        /// <summary>
        /// Alert
        /// </summary>
        public const string ALERT_SCHEMA="mau01";
	}
	#endregion

	#region Classe des flags
	/// <summary>
	/// Classe de constantes des Flags du projet AdExpress
	/// </summary>
	public class Flags{
		/// <summary>
		/// Identifiant du flag d'accès aux créations
		/// </summary>
		public const Int64 ID_CREATION_ACCESS_FLAG=200;
		/// <summary>
		/// Texte du flag d'accès aux créations
		/// </summary>
		public const string CREATION_ACCESS_FLAG="Accéder aux créations";
		/// <summary>
		/// Identifiant du flag de download des créations
		/// </summary>
		public const Int64 ID_DOWNLOAD_ACCESS_FLAG=201;
		/// <summary>
		/// Texte du flag de download des créations
		/// </summary>
		public const string DOWNLOAD_ACCESS_FLAG="Télécharger les créations";
		/// <summary>
		/// Identifiant du flag detail des insertion
		/// </summary>
		public const Int64 ID_DETAIL_OUTDOOR_ACCESS_FLAG=216;
		/// <summary>
		/// Texte du flag de detail des insertion
		/// </summary>
		public const string DETAIL_OUTDOOR_ACCESS_FLAG=" Accès détail publicité extérieure";
		/// <summary>
		/// Identifiant du flag detail des insertion
		/// </summary>
		public const Int64 ID_DETAIL_INTERNET_ACCESS_FLAG=231;
		/// <summary>
		/// Texte du flag de detail des insertion
		/// </summary>
		public const string DETAIL_INTERNET_ACCESS_FLAG=" Accéder aux créations Internet";
        /// <summary>
        /// Identifiant du flag detail des insertion
        /// </summary>
        public const Int64 ID_DETAIL_EVALIANT_MOBILE_ACCESS_FLAG = 276;
        /// <summary>
        /// Texte du flag de detail des insertion
        /// </summary>
        public const string DETAIL_EVALIANT_MOBILE_ACCESS_FLAG = " Accéder aux création Evaliant Mobile";
        /// <summary>
        /// Identifiant du flag d'accès aux créations FLV d'EVALIANT
        /// </summary>
        public const Int64 ID_FLV_EVALIANT_CREATION_ACCESS_FLAG = 318;
		/// <summary>
		/// Identifiant du flag Agences Média
		/// </summary>
		public const Int64 ID_MEDIA_AGENCY=146;
		/// <summary>
		/// Texte du flag Agences Media
		/// </summary>
		public const string MEDIA_AGENCY="Agences Media";
		/// <summary>
		/// Identifiant du flag Marque 
		/// </summary>
		public const Int64 ID_MARQUE=211;
		/// <summary>
		/// Identifiant du flag Groupe de société
		/// </summary>
		public const Int64 ID_HOLDING_COMPANY=212;
		/// <summary>
		/// Identifiant du flag des accroches
		/// </summary>
		public const Int64 ID_SLOGAN_ACCESS_FLAG=221;
		/// <summary>
		/// Identifiant du flag d'accès aux détails produit des plans media
		/// </summary>
		public const Int64 MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG=226;

		/// <summary>
		/// Identifiant du flag d'accès aux  plans media AdNetTrack
		/// </summary>
		public const Int64 MEDIA_SCHEDULE_ADNETTRACK_ACCESS_FLAG = 236;

		/// <summary>
		/// Identifiant du flag d'accès aux créations TV
		/// </summary>
		public const Int64 ID_TV_CREATION_ACCESS_FLAG = 241;

		/// <summary>
		/// Identifiant du flag d'accès aux créations Radio
		/// </summary>
		public const Int64 ID_RADIO_CREATION_ACCESS_FLAG = 242;

		/// <summary>
		/// Identifiant du flag d'accès aux créations créations Presse
		/// </summary>
		public const Int64 ID_PRESS_CREATION_ACCESS_FLAG = 243;

		/// <summary>
		/// Identifiant du flag d'accès aux créations créations Affisource
		/// </summary>
		public const Int64 ID_OUTDOOR_CREATION_ACCESS_FLAG = 244;

		/// <summary>
		/// Identifiant du flag d'accès aux créations créations Autres
		/// </summary>
		public const Int64 ID_OTHERS_CREATION_ACCESS_FLAG = 245;

		/// <summary>
		/// Identifiant du flag d'accès aux créations créations Presse Internationale
		/// </summary>
		public const Int64 ID_INTERNATIONAL_PRESS_CREATION_ACCESS_FLAG = 246;

		/// <summary>
		/// Identifiant du flag d'accès aux créations créations Marketing Direct
		/// </summary>
		public const Int64 ID_DIRECT_MARKETING_CREATION_ACCESS_FLAG = 251;
        /// <summary>
        /// Identifiant du flag Accès Volumes (Marketing Direct)
        /// </summary>
        public const Int64 ID_VOLUME_MARKETING_DIRECT = 252;
        /// <summary>
        /// Identifiant du flag Accès Poids (Marketing Direct)
        /// </summary>
        public const Int64 ID_POIDS_MARKETING_DIRECT = 256;
		/// <summary>
		/// Identifiant du flag Accès aux (Parrainage TV) catégories panoramiques dans l'analyse sectorielle.
		/// </summary>
		public const Int64 ID_SPONSORSHIP_TV_ACCESS_FLAG = 261;
        /// <summary>
        /// Right Flag  of digital tv spot's details
        /// </summary>
        public const Int64 ID_DETAIL_DIGITAL_TV_ACCESS_FLAG = 266;
		/// <summary>
		/// Right Flag  of  product level
		/// </summary>
		/// <remarks>Used for Finland</remarks>
		public const Int64 ID_PRODUCT_LEVEL_ACCESS_FLAG = 271;
        /// <summary>
        /// Identifiant du flag detail des insertion
        /// </summary>
        public const Int64 ID_DETAIL_INSTORE_ACCESS_FLAG = 286;
        /// <summary>
        /// Identifiant du flag d'accès aux créations créations Affisource
        /// </summary>
        public const Int64 ID_INSTORE_CREATION_ACCESS_FLAG = 287;
        /// <summary>
        /// Right Flag  of  segment level
        /// </summary>
        /// <remarks>Used for Finland</remarks>
        public const Int64 ID_SEGMENT_LEVEL_ACCESS_FLAG = 293;
        /// <summary>
        /// Right Flag  of  Group level
        /// </summary>
        /// <remarks>Used for Finland</remarks>
        public const Int64 ID_GROUP_LEVEL_ACCESS_FLAG = 294;
        /// <summary>
        /// Identifier of PRESS's media agency flag
        /// </summary>
        public const Int64 ID_PRESS_MEDIA_AGENCY_FLAG = 298;  
        /// <summary>
        /// Identifier of RADIO's media agency flag
        /// </summary>
        public const Int64 ID_RADIO_MEDIA_AGENCY_FLAG = 299;
        /// <summary>
        /// Identifier of TV's media agency flag
        /// </summary>
        public const Int64 ID_TV_MEDIA_AGENCY_FLAG = 300;  
        /// <summary>
        /// Identifier of INTERNET's media agency flag
        /// </summary>
        public const Int64 ID_INTERNET_MEDIA_AGENCY_FLAG = 302;
        /// <summary>
        /// Identifier of OUTDOOR's media agency flag
        /// </summary>
        public const Int64 ID_OUTDOOR_MEDIA_AGENCY_FLAG = 301;

        /// <summary>
        /// Identifier of INTERNATIONAL PRESS's media agency flag
        /// </summary>
        public const Int64 ID_INTERNATIONAL_PRESS_MEDIA_AGENCY_FLAG = 303;
        /// <summary>
        /// Identifier of OTHERS' media agency flag
        /// </summary>
        public const Int64 ID_OTHERS_MEDIA_AGENCY_FLAG = 304;
        /// <summary>
        /// Identifier of MARKETING DIRECT 's media agency flag
        /// </summary>
        public const Int64 ID_MARKETING_DIRECT_MEDIA_AGENCY_FLAG = 305;
        /// <summary>
        /// Identifier of EVALIANT MOBILE's media agency flag
        /// </summary>
        public const Int64 ID_EVALIANT_MOBILE_MEDIA_AGENCY_FLAG = 306;
        /// <summary>
        /// Identifier of INTERNATIONAL EVALIANT's media agency flag
        /// </summary>
        public const Int64 ID_INTERNET_EVALIANT_MEDIA_AGENCY_FLAG = 307;
        /// <summary>
        /// Identifier of MEDIA TACTIQUE's media agency flag
        /// </summary>
        public const Int64 ID_MEDIA_TACTIQUE_MEDIA_AGENCY_FLAG = 308;
        /// <summary>
        /// Identifier of MEDIA CINEMA's media agency flag
        /// </summary>
        public const Int64 ID_MEDIA_CINEMA_MEDIA_AGENCY_FLAG = 309;
        /// <summary>
        /// Identifier of CAtegory Application mobile creative flag
        /// </summary>
        public const Int64 ID_APPLICATION_MOBILE_CREATIVE_FLAG = 323;
        /// <summary>
        /// Identifier of export evaliant creatives flag
        /// </summary>
        public const Int64 ID_EXPORT_INTERNET_EVALIANT_CREATIVE_FLAG = 328;       
       /// <summary>
        /// Identifier of  PRESS CLIPPING creatives flag
        /// </summary>
        public const Int64 ID_PRESS_CLIPPING_CREATION_ACCESS_FLAG = 334;
        /// <summary>
        /// Identifier of  EDITORIAL creatives flag
        /// </summary>
        public const Int64 ID_EDITORIAL_CREATION_ACCESS_FLAG = 336;

        /// <summary>
        /// Indoor creative access flag identifier
        /// </summary>
        public const Int64 ID_INDOOR_CREATION_ACCESS_FLAG = 337;

        /// <summary>
        /// Indoor insertion detail acccess  flag 
        /// </summary>
        public const Int64 ID_DETAIL_INDOOR_ACCESS_FLAG = 338;

        /// <summary>
        /// Press Vehicle pages access  flag 
        /// </summary>
        public const Int64 ID_PRESS_VEHICLE_PAGES_ACCESS_FLAG = 339;

        /// <summary>
        /// Identifiant du flag Marque Media
        /// </summary>
        public const Int64 ID_MEDIA_GROUP = 343;

        /// <summary>
        /// Identifier of DISPLAY's media agency flag
        /// </summary>
        public const Int64 ID_DISPLAY_MEDIA_AGENCY_FLAG = 348;

        /// <summary>
        /// Identifiant du flag Accès Volumes (DISPLAY)
        /// </summary>
        public const Int64 ID_VOLUME_DISPLAY = 353;

        ///// <summary>
        ///// Identifier of SEARCH's media agency flag
        ///// </summary>
        //public const Int64 ID_SEARCH_MEDIA_AGENCY_FLAG = 358;

        /// <summary>
        /// Identifier of SEARCH's media agency flag
        /// </summary>
        public const Int64 ID_PURCHASE_MODE_DISPLAY_FLAG = 358;
        /// <summary>
        /// Identifiant du flag d'accès aux créations créations DOOH
        /// </summary>
        public const Int64 ID_DOOH_CREATION_ACCESS_FLAG = 363;

        /// <summary>
        /// Identifier of DOOH's media agency flag
        /// </summary>
        public const Int64 ID_DOOH_MEDIA_AGENCY_FLAG = 364;
        /// <summary>
        /// Identifiant du flag detail des insertion
        /// </summary>
        public const Int64 ID_DETAIL_DOOH_ACCESS_FLAG = 365;
        /// <summary>
        /// Identifiant du flag accès aux codes SIRET
        /// </summary>
        public const Int64 ID_CODE_SIRET_ACCESS_FLAG = 368;

    }
	#endregion

	#region Classe pour les tendances

	/// <summary>
	/// Constantes utilisées pour Hathor
	/// </summary>
	public class Hathor{
	
		/// <summary>
		/// Tables dans Hathor
		/// </summary>
		public class Tables{
			/// <summary>
			/// Table tendency month
			/// </summary>
			public const string TENDENCY_MONTH="tendency_month";
			/// <summary>
			/// Table total tendency month
			/// </summary>
			public const string TOTAL_TENDENCY_MONTH="total_tendency_month";
			/// <summary>
			/// Table tendency week
			/// </summary>
			public const string TENDENCY_WEEK="tendency_week";
			/// <summary>
			/// Table total tendency week
			/// </summary>
			public const string TOTAL_TENDENCY_WEEK="total_tendency_week";
			/// <summary>
			/// Préfixe de la table tendency month
			/// </summary>
			public const string TENDENCY_MONTH_PREFIXE="tm";
			/// <summary>
			/// Préfixe de la tabme total tendendy month
			/// </summary>
			public const string TOTAL_TENDENCY_MONTH_PREFIXE="ttm";

		}
		
		/// <summary>
		/// champs
		/// </summary>
		public class Fields{
			/// <summary>
			/// Champ Insertion dans les tables quotidiennes
			/// </summary>
			public const string DATA_TABLE_INSERT="insertion";
			/// <summary>
			/// Champ Investissement dans les tables quotidiennes
			/// </summary>
			public const string DATA_TABLE_EXPENDITURE_EURO="expenditure_euro";
			/// <summary>
			/// Champ mmc dans les tables quotidiennes
			/// </summary>
			public const string DATA_TABLE_AREA_MMC="area_mmc";			
			/// <summary>
			/// Champ duree dans les tables quotidiennes
			/// </summary>
			public const string DATA_TABLE_DURATION="duration";	
			/// <summary>
			/// Champ page dans les tables quotidiennes
			/// </summary>
			public const string DATA_TABLE_AREA_PAGE="area_page";	
	
		}


		/// <summary>
		/// Constante indiquant que l'on traite de la press
		/// </summary>
		public const string ID_PRESS="1";
		/// <summary>
		/// Indique si le resultat est en pdm
		/// </summary>
		public const string PDM_TRUE="0";
		/// <summary>
		/// Indique si le resultat n'est pas en pdm
		/// </summary>
		public const string PDM_FALSE="10";
		/// <summary>
		/// Indique que c'est un cumul de mois
		/// </summary>
		public const string CUMULATIVE_TRUE="0";
		/// <summary>
		/// Indique que ce n'est pas un cumul à date
		/// </summary>
		public const string CUMULATIVE_FALSE="10";
		/// <summary>
		/// Indique que l'on a regrouper les médias
		/// </summary>
		public const string TYPE_TENDENCY_SUBTOTAL="1";
		/// <summary>
		/// Indique que l'on se trouve ds le cas total
		/// </summary>
		public const string TYPE_TENDENCY_TOTAL="0";
		/// <summary>
		/// Year le Cumul à date
		/// </summary>
		public const string YEAR_PERIOD_CUMULATIVE="9000";
		/// <summary>
		/// Date période pour le cumul à date
		/// </summary>
		public const string DATE_PERIOD_CUMULATIVE="900090";
		/// <summary>
		/// id periode pour le cumul à date
		/// </summary>
		public const string ID_PERIOD_CUMULATIVE="90";
        /// <summary>
        /// Excluded product list
        /// </summary>
        public const string LIST_EXCLUDE_PRODUCT = "180000,50000,50001,50300";
        /// <summary>
        /// Comparative Type = Default : current month with previous month and current week number with previous week number
        /// </summary>
        public const string COMPARATIVE_TYPE_DEFAULT = "0";
        /// <summary>
        /// Comparative Type = Date to Date
        /// </summary>
        public const string COMPARATIVE_TYPE_DATE_TO_DATE = "10";
	}


	#endregion

	#region Classe pour tableau de bord
	/// <summary>
	/// Constantes utilisées pour tableaux de bord
	/// </summary>
	public class DashBoard {
        #region constantes
        /// <summary>
		/// Tables des tableaux de bord
		/// </summary>
		/*public class Tables{
			/// <summary>
			/// Constante tableau de bord par mois
			/// </summary>
			public const string	TABLEAU_BORD_PLURI_MONTH="TABLEAU_BORD_PLURI_MONTH";
			/// <summary>
			/// Constante tableau de bord par semaine
			/// </summary>
			public const string	TABLEAU_BORD_PLURI_WEEK="TABLEAU_BORD_PLURI_WEEK";
			/// <summary>
			/// constante tableau de bord radio par jour
			/// </summary>
			public const string	TABLEAU_BORD_RADIO_DAY="TABLEAU_BORD_RADIO_DAY";
			/// <summary>
			/// constante tableau de bord télé par jour
			/// </summary>
			public const string TABLEAU_BORD_TV_DAY="TABLEAU_BORD_TV_DAY";
			/// <summary>
			/// constante tableau de bord télé par mois
			/// </summary>
			public const string TABLEAU_BORD_TV_REP_MTH ="TABLEAU_BORD_TV_REP_MTH";
			/// <summary>
			/// constante tableau de bord répartition télé par semaine
			/// </summary>
			public const string TABLEAU_BORD_TV_REP_WEEK ="TABLEAU_BORD_TV_REP_WEEK";
			/// <summary>
			/// Constante tableau de bord répartition radio part mois
			/// </summary>
			public const string TABLEAU_BORD_RADIO_R_MTH ="TABLEAU_BORD_RADIO_R_MTH";
			/// <summary>
			/// Constante tableau de bord répartition radio par semaine
			/// </summary>
			public const string TABLEAU_BORD_RADIO_R_WEEK ="TABLEAU_BORD_RADIO_R_WEEK";
            /// <summary>
            /// Month Dashbord Table Constante
            /// </summary>
            public const string TABLEAU_BORD_EVALIANT_MONTH = "TABLEAU_BORD_EVAL_R_MTH";
            /// <summary>
            /// Week Dashbord Table Constante
            /// </summary>
            public const string TABLEAU_BORD_EVALIANT_WEEK = "TABLEAU_BORD_EVAL_R_WEEK";
		}*/
		/// <summary>
		/// Champs des tableaux de bord
		/// </summary>
		public class Fields{
			/// <summary>
			/// Champ format de repartition
			/// </summary>
			public const string ID_FORMAT_REPARTITION ="ID_FORMAT_REPARTITION";
			/// <summary>
			/// Champ Tranche horaire télévision
			/// </summary>
			public const string TV_TIME_SLICE =" ID_COM_BREAK_REPARTITION_TV";
			/// <summary>
			/// Champ Tranche horaire radio
			/// </summary>
			public const string RADIO_TIME_SLICE =" ID_TOP_DIFF_REPARTITION_RADIO";
			/// <summary>
			/// Champ format spot 
			/// </summary>
			public const string DURATION="duration";
			/// <summary>
			/// Champ jour nommé
			/// </summary>
			public const string ID_DAY="id_day";
		}
				
		#endregion
	}
		#endregion

	#region Classe des types de statut des alertes push mail
	/// <summary>
	/// Classe de constantes des types de statut des alertes push mail
	/// </summary>
	public class StatusType{
		/// <summary>
		/// Identifiant du type de statut (new)
		/// </summary>
		public const Int64 NEW_ID_STATUS_TYPE=0;
		/// <summary>
		/// Identifiant du type de statut (error)
		/// </summary>
		public const Int64 ERROR_ID_STATUS_TYPE=4;
	}

	/// <summary>
	/// Constantes de la table Alert_type
	/// </summary>
	public class AlertFlag{
		/// <summary>
		/// Identifiant du flag hors encart
		/// </summary>
		public const Int64 EXCEPT_INSET_ID=1;
		/// <summary>
		/// Identifiant du flag hors autopromo
		/// </summary>
		public const Int64 EXCEPT_AUTOPROMO_ID=2;
	}

	/// <summary>
	/// Constantes de la table Alert_flag
	/// </summary>
	public class AlertType{
		/// <summary>
		/// Identifiant du type alerte push mail portefeuille
		/// </summary>
		public const Int64 PORTOFOLIO_ID=1;
	}

	/// <summary>
	/// Constantes de la table Alert_universe
	/// </summary>
	public class AlertUniverseType{
		/// <summary>
		/// Identifiant de Famille
		/// </summary>
		public const Int64 FAMILLE_VALUE=1;
		/// <summary>
		/// Identifiant de Classe
		/// </summary>
		public const Int64 CLASSE_VALUE=2;
		/// <summary>
		/// Identifiant de Groupe
		/// </summary>
		public const Int64 GROUPE_VALUE=3;
		/// <summary>
		/// Identifiant de Variété
		/// </summary>
		public const Int64 VARIETE_VALUE=4;
		/// <summary>
		/// Identifiant d'annonceur
		/// </summary>
		public const Int64 ANNONCEUR_VALUE=10;
		/// <summary>
		/// Identifiant de groupe de société
		/// </summary>
		public const Int64 GROUPE_SOCIETE_VALUE=11;
		/// <summary>
		/// Identifiant de Média
		/// </summary>
		public const Int64 MEDIA_VALUE=5;
		/// <summary>
		/// Identifiant de Catégorie
		/// </summary>
		public const Int64 CATEGORIE_VALUE=6;
		/// <summary>
		/// Identifiant de Support
		/// </summary>
		public const Int64 SUPPORT_VALUE=7;
		/// <summary>
		/// Identifiant de Titre
		/// </summary>
		public const Int64 TITRE_VALUE=8;
		/// <summary>
		/// Identifiant de Régie
		/// </summary>
		public const Int64 REGIE_VALUE=9;
	}
	#endregion

	#region Classe des contraintes des données
	/// <summary>
	/// Classe des contraintes des données
	/// </summary>
	public class Constraints{
		/// <summary>
		/// Contrainte de type champ de données
		/// </summary>
		public const Int64 DB_FIELD_CONTRAINT_TYPE=1;
		
		/// <summary>
		/// Contrainte de type Jointures de tables
		/// </summary>
		public const Int64 DB_JOIN_CONTRAINT_TYPE=2;

		/// <summary>
		/// Contrainte de type tables
		/// </summary>
		public const Int64 DB_TABLE_CONTRAINT_TYPE=3;

		/// <summary>
		/// Contrainte de type order des données
		/// </summary>
		public const Int64 DB_ORDER_CONTRAINT_TYPE=4;

		/// <summary>
		/// Contrainte de type groupement des données
		/// </summary>
		public const Int64 GROUP_BY_CONTRAINT_TYPE=5;
	}
	#endregion

}
