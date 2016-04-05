
#region Information
/*
 * Author : 
 * Creation:!
 * Modificatin:
 *		G Ragneau - 01/08/2006 - new method : bool CheckUnivers(string)
 *				...
 *		G. Facon	29/03/2007	Add AdNetTrackSelection
 * */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;


using TNS.AdExpress.Constantes.DB;
using TNS.Classification.Universe;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Constantes.Classification.DB;
using AccessCstAlias = TNS.AdExpress.Constantes.Customer.Right.type;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.FrameWork.Collections;
using TNS.FrameWork.DB.Common;
using DATracking = TNS.AdExpress.Web.Core.DataAccess.Session.TrackingDataAccess;
using TNS.AdExpress.Domain.Web.Navigation;
using CoreConstantes = TNS.AdExpress.Constantes.Web.Core;
using TNS.FrameWork.WebResultUI;
using TNS.Isis.Right.Common;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Classification;

using CustPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using TNS.AdExpress.Web.Core.Selection;


namespace TNS.AdExpress.Web.Core.Sessions
{
    /// <summary>
    /// La classe session comprend toutes les variables de sessions n�cessaires � la navigation d'un client d'AdExpress.
    /// La session est charg�e de ma base de donn�es (ou cr��e si inexistante) � la demande d'une page et sauvegard� lors de la r�ponse � la requ�te du client.
    /// </summary>

    [System.Serializable]

    public class WebSession
    {

        #region Variables

        #region Acc�s aux donn�es de la Base de donn�es
        /// <summary>
        /// Source de donn�es du client pour acc�der aux donn�es
        /// </summary>	
        protected IDataSource _source = null;
        #endregion

        #region G�n�ral

        #region Session
        /// <summary>
        /// Identifiant de session
        /// </summary>
        protected string idSession;
        #endregion

        #region Langues
        /// <summary>
        /// Langage du site
        /// </summary>
        protected int siteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH;
        #endregion

        #region Dates
        /// <summary>
        /// Date de d�but de la session (date de connexion)
        /// </summary>
        protected DateTime beginningDate = DateTime.Now;

        /// <summary>
        /// Date de dern�re modification de la session
        /// </summary>
        protected DateTime modificationDate = DateTime.Now;
        #endregion

        /// <summary>
        /// Module courant dans la navigation (atteint ou � atteindre)
        /// </summary>
        protected Int64 currentModule = 0;

        /// <summary>
        /// Code texte du module courant
        /// </summary>
        protected int moduleTraductionCode;

        /// <summary>
        /// Pr�cise si le module a d�j� �t� atteint ou non depuis qu'il est choisi.
        /// </summary>
        protected bool reachedModule = false;

        /// <summary>
        /// Adresse de la derni�re page de r�sultat atteinte
        /// </summary>
        protected string lastReachedResultUrl = "";


        /// <summary>
        /// Onglet courant (util pour un module comprenant plusieurs onglets)
        /// </summary>
        protected Int64 currentTab = 0;
        ///<summary>
        /// Droits client
        /// </summary>
        ///  <directed>True</directed>
        ///  <supplierCardinality>1</supplierCardinality>
        //protected TNS.AdExpress.Web.Core.WebRight customerLogin = null;
        protected TNS.AdExpress.Right customerLogin = null;

        /// <summary>
        /// Login company
        /// </summary>
        [System.NonSerialized]
        protected Company _company = null;

        /// <summary>
        /// ann�e de chargement pour les recaps
        /// </summary>
        protected int downLoadDate = 0;

        /// <summary>
        /// Nom du fichier PDF export�
        /// </summary>
        protected string exportedPDFFileName = "";

        /// <summary>
        /// Email des destinataires du fichier PDF � exporter
        /// </summary>
        protected string[] emailRecipient = null;
        #endregion

        #region Univers de l'etude
        /// <summary>
        /// S�lection courante de media de l'univers courant
        /// </summary>
        protected TreeNode currentUniversMedia = new TreeNode("media");

        /// <summary>
        /// S�lection courante de produits de l'univers courant
        /// </summary>
        protected TreeNode currentUniversProduct = new TreeNode("produit");

        /// <summary>
        /// S�lection courante d'annonceurs de l'univers courant
        /// </summary>
        protected TreeNode currentUniversAdvertiser = new TreeNode("advertiser");

        /// <summary>
        /// S�lection original de media de l'univers courant
        /// </summary>
        protected TreeNode selectionUniversMedia = new TreeNode("media");

        /// <summary>
        /// S�lection original de produits de l'univers courant
        /// </summary>
        protected TreeNode selectionUniversProduct = new TreeNode("produit");

        /// <summary>
        /// S�lection original d'annonceurs de l'univers courant
        /// </summary>
        protected TreeNode selectionUniversAdvertiser = new TreeNode("advertiser");

        /// <summary>
        /// S�lection de media de l'univers referrence
        /// </summary>
        protected TreeNode referenceUniversMedia = new TreeNode("media");

        /// <summary>
        /// S�lection de produits de l'univers reference
        /// </summary>
        protected TreeNode referenceUniversProduct = new TreeNode("produit");

        /// <summary>
        /// S�lection d'annonceurs de l'univers referrence
        /// </summary>
        protected TreeNode referenceUniversAdvertiser = new TreeNode("advertiser");

        /// <summary>
        /// HashTable pouvant contenir 5 s�lections de media comme univers concurrent
        /// </summary>
        protected Hashtable competitorUniversMedia = new Hashtable(5);

        /// <summary>
        /// HashTable pouvant contenir 5 s�lections de produits comme univers concurrent
        /// </summary>
        protected Hashtable competitorUniversProduct = new Hashtable(5);

        /// <summary>
        /// HashTable pouvant contenir 5 s�lections d'annonceurs comme univers concurrent
        /// La cl� est un entier qui permet de conna�tre la position de l'univers
        /// La valeur est un objet de la classe CompetitorAdvertiser que l'on trouve dans le namespace TNS.AdExpress.Web.Common.Univers
        /// Il contient le nom de l'univers ainsi que l'arbre avec la liste des annonceurs
        /// </summary>
        protected Hashtable competitorUniversAdvertiser = new Hashtable(5);

        /// <summary>
        /// Treenode temporaire
        /// </summary>
        protected TreeNode temporaryTreenode = null;

        /// <summary>
        /// Etude comparative ou non 
        /// </summary>
        protected bool comparativeStudy = false;

        /// <summary>
        /// Indique si l'utilisateur veut personnaliser la s�lection des �l�ments concurrents ou de r�f�rence
        /// </summary>
        protected bool customizedReferenceComcurrentElements = false;

        /// <summary>
        /// Indique si l'utilisateur veut effectuer une comparaison par rapport au total du march� ou au total d'une famille
        /// </summary>
        protected TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion comparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal;

        #region univers �tude bilan de campagne (APPM)
        /// <summary>
        /// S�lection courante des cibles AEPM de l'univers courant
        /// </summary>
        protected TreeNode currentUniversAEPMTarget = new TreeNode("target");
        /// <summary>
        /// S�lection originale des cibles AEPM de l'univers courant
        /// </summary>
        protected TreeNode selectionUniversAEPMTarget = new TreeNode("target");
        /// <summary>
        /// S�lection courante des vagues AEPM de l'univers courant
        /// </summary>
        protected TreeNode currentUniversAEPMWave = new TreeNode("wave");
        /// <summary>
        /// S�lection originale des vagues AEPM de l'univers courant
        /// </summary>
        protected TreeNode selectionUniversAEPMWave = new TreeNode("wave");
        /// <summary>
        /// S�lection courante des vagues OJD de l'univers courant
        /// </summary>
        protected TreeNode currentUniversOJDWave = new TreeNode("ojd");
        /// <summary>
        /// S�lection originale des vagues OJD de l'univers courant
        /// </summary>
        protected TreeNode selectionUniversOJDWave = new TreeNode("wave");
        /// <summary>
        /// Ecart APPM
        /// </summary>
        protected bool ecart = false;
        /// <summary>
        /// Principal dictionary of  universe product selection
        /// </summary>				
        protected Dictionary<int, AdExpressUniverse> _principalProductUniverses = new Dictionary<int, AdExpressUniverse>();
        /// <summary>
        /// Secondary dictionary of universe product selection
        /// </summary>	
        protected Dictionary<int, AdExpressUniverse> _secondaryProductUniverses = new Dictionary<int, AdExpressUniverse>();
        /// <summary>
        /// Principal dictionary of  universe media selection
        /// </summary>				
        protected Dictionary<int, AdExpressUniverse> _principalMediaUniverses = new Dictionary<int, AdExpressUniverse>();

        /// <summary>
        /// Secondary dictionary of universe media selection
        /// </summary>	
        protected Dictionary<int, AdExpressUniverse> _secondaryMediaUniverses = new Dictionary<int, AdExpressUniverse>();
        /// <summary>
        /// Principal dictionary of  universe advertising agnecy selection
        /// </summary>				
        protected Dictionary<int, AdExpressUniverse> _principalAdvertisingAgnecyUniverses = new Dictionary<int, AdExpressUniverse>();
        /// <summary>
        /// Secondary dictionary of universe advertising agnecy selection
        /// </summary>	
        protected Dictionary<int, AdExpressUniverse> _secondaryAdvertisingAgnecyUniverses = new Dictionary<int, AdExpressUniverse>();
        #endregion

        #endregion

        #region P�riode
        /// <summary>
        /// Dur�e de la p�riode (1 semaine, 5 mois, 2 ans...)
        /// </summary>
        protected int periodLength = 0;

        /// <summary>
        /// Date de d�but de la p�riode
        /// </summary>
        protected string periodBeginningDate = "";

        /// <summary>
        /// Date de d�but du d�tail de la p�riode  
        /// </summary>
        protected string detailPeriodBeginningDate = "";
        /// <summary>
        /// Date de d�but de publication
        /// </summary>
        protected string publicationBeginningDate = "";

        /// <summary>
        /// Date de fin de p�riode
        /// </summary>
        protected string periodEndDate = "";

        /// <summary>
        /// Date de d�but du d�tail de la p�riode  
        /// </summary>
        protected string detailPeriodEndDate = "";
        /// <summary>
        /// Date de fin de publication
        /// </summary>
        protected string publicationEndDate = "";
        /// <summary>
        /// Ann�e du fichier d'agence m�dia
        /// </summary>
        protected string mediaAgencyFileYear = "";

        /// <summary>
        /// Identifiant de type de date de parution
        /// </summary>
        protected Constantes.Web.CustomerSessions.Period.publicationType publicationDateType = Constantes.Web.CustomerSessions.Period.publicationType.staticDate;


        /// <summary>
        /// Identifiant de type de p�riode
        /// </summary>
        protected Constantes.Web.CustomerSessions.Period.Type periodType = Constantes.Web.CustomerSessions.Period.Type.dateToDateMonth;

        /// <summary>
        /// Derni�re p�riode disponible dans l'analyse sectorielle
        /// </summary>
        protected string lastAvailableRecapMonth = "";
        #endregion

        #region Pr�sentation des r�sultats
        /// <summary>
        /// Niveau de pr�cision de la p�riode (voir Constantes.cs)
        /// </summary>
        protected Constantes.Web.CustomerSessions.Period.DisplayLevel detailPeriod = Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly;

        /// <summary>
        /// Unit� utilis� (�, spots...)
        /// </summary>
        protected Constantes.Web.CustomerSessions.Unit unit = UnitsInformation.DefaultCurrency;

        /// <summary>
        /// Unit� en pourcentage
        /// </summary>
        protected bool percentage = false;

        /// <summary>
        /// Alignement du pourcentage (% horizontal ou % vertical)
        /// </summary>
        [System.NonSerialized]
        protected WebConstantes.Percentage.Alignment _percentageAlignment = WebConstantes.Percentage.Alignment.none;

        /// <summary>
        /// Comparative Period Type : dateToDate, comparativeWeekDate
        /// </summary>
        [System.NonSerialized]
        protected WebConstantes.globalCalendar.comparativePeriodType _comparativePeriodType = WebConstantes.globalCalendar.comparativePeriodType.dateToDate;

        /// <summary>
        /// Period Selection Type : Period selection type : day to day or month to month
        /// </summary>
        [System.NonSerialized]
        protected WebConstantes.globalCalendar.periodSelectiontype _periodSelectionType = WebConstantes.globalCalendar.periodSelectiontype.day;

        /// <summary>
        /// Selected Banners Format List
        /// </summary>
        [System.NonSerialized]
        protected string _selectedBannersForamtList = string.Empty;

        /// <summary>
        /// Selected Purchase Mode List
        /// </summary>
        [System.NonSerialized]
        protected string _selectedPurchaseModeList = string.Empty;

        /// <summary>
        /// Evaliant Country Access List
        /// </summary>
        [System.NonSerialized]
        protected string _evaliantCountryAccessList = string.Empty;

        /// <summary>
        /// Index d�crivant un identifiant ou un num�roe de colonne � trier.
        /// </summary>
        [Obsolete("Utiliser au maximum l'attribut _sortKey dans userparameters")]
        protected int sort = 0;

        /// <summary>
        /// Ordre de tri ("desc" ou "asc")
        /// </summary>
        [Obsolete("Utiliser au maximum l'attribut _sortOrder dans userparameters")]
        protected string sortOrder = string.Empty;

        /// <summary>
        /// D�tail pr�format� de la nomenclature media dans les tableaux 
        /// !!! type encore � pr�ciser
        /// </summary>
        protected TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle;

        /// <summary>
        /// D�tail pr�format� de la nomenclature produit dans les tableaux 
        /// !!! type encore � pr�ciser
        /// </summary>
        protected TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails preformatedProductDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group;

        /// <summary>
        /// D�tail pr�format� des p�riodes dans les tableaux 
        /// !!! type encore � pr�ciser
        /// </summary>
        protected TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedPeriodDetails preformatedPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedPeriodDetails.monthly_And_Total;

        /// <summary>
        /// Pr�formatage des tableaux dynamiques 
        /// !!! type encore � pr�ciser
        /// </summary>
        protected TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables preformatedTable = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables.media_X_Year;

        /// <summary>
        /// Utilisation de la part de march� ou non
        /// </summary>
        protected bool pdm = false;

        /// <summary>
        /// Utilisation de la part de voix ou non
        /// </summary>
        protected bool pdv = false;

        /// <summary>
        /// Affichage de l'�volution de march� ou non
        /// </summary>
        protected bool evolution = true;

        /// <summary>
        /// Afficher uniquement les �l�m�nts personnalis�s (exemple : dans les tabelaux dynamiques, uniquemenent
        /// les �l�ments de r�f�nreces et cocurrents
        /// </summary>
        protected bool personalizedElementsOnly = false;

        /// <summary>
        /// Utilisation de l'auto-promo Evaliant
        /// </summary>
        protected bool autopromoEvaliant = false;

        /// <summary>
        /// Dans le cas de r�sultat disponibles sous formes de tableaux ou de graphiques, indique si on doit affiocher le graphique
        /// </summary>
        protected bool graphics = true;

        /// <summary>
        /// Pour la presse Encarts (Total, Encarts, Hors Encarts)
        /// </summary>
        protected TNS.AdExpress.Constantes.Web.CustomerSessions.Insert insert = TNS.AdExpress.Constantes.Web.CustomerSessions.Insert.total;
        /// <summary>
        /// Auto Promo (Total, Except Auto promo Advertiser, Except Auto Promo Holding Company)
        /// </summary>
        [System.NonSerialized]
        protected TNS.AdExpress.Constantes.Web.CustomerSessions.AutoPromo _autoPromo = TNS.AdExpress.Constantes.Web.CustomerSessions.AutoPromo.total;
        /// <summary>
        /// Nouveau produit
        /// </summary>
        protected TNS.AdExpress.Constantes.Web.CustomerSessions.NewProduct newProduct = TNS.AdExpress.Constantes.Web.CustomerSessions.NewProduct.support;
        /// <summary>
        /// Niveau de d�tail produit
        /// </summary>
        protected ProductLevelSelection productDetailLevel = null;
        /// <summary>
        /// Niveau de d�tail M�dia
        /// </summary>
        protected MediaLevelSelection mediaDetailLevel = null;

        #region variables de R�partition
        /// <summary>
        /// Format de spots
        /// </summary>
        protected WebConstantes.Repartition.Format format = WebConstantes.Repartition.Format.Total;
        /// <summary>
        /// Jour Nomm�
        /// </summary>
        protected WebConstantes.Repartition.namedDay namedDay = WebConstantes.Repartition.namedDay.Total;
        /// <summary>
        /// Tranche horaire
        /// </summary>
        protected WebConstantes.Repartition.timeInterval timeInterval = WebConstantes.Repartition.timeInterval.Total;
        #endregion

        #region Nouvelles variables

        /// <summary>
        /// Liste des visuels
        /// </summary>
        protected ArrayList visuals = null;



        /// <summary>
        /// Liste destin�e � contenir les identifiants accroches de personnalisation dans les plans m�dias
        /// </summary>
        protected ArrayList _idSlogans = null;

        /// <summary>
        /// Hashtable contenant une liste d'accroches en cl�s et des couleurs associ�es en valeurs
        /// </summary>
        protected Hashtable _sloganColors = new Hashtable();
        /// <summary>
        /// Pond�ration des unit�s
        /// </summary>
        [System.NonSerialized]
        protected Weighting _unitWeighting = null;

        /// <summary>
        /// Campaign Type
        /// </summary>
        [System.NonSerialized]
        protected Constantes.Web.CustomerSessions.CampaignType _campaignType = Constantes.Web.CustomerSessions.CampaignType.none;

        /// <summary>
        /// Date to Date comparative week
        /// </summary>
        [System.NonSerialized]
        protected bool _dateToDateComparativeWeek = false;
        #endregion

        #endregion

        #region Niveaux de d�tail g�n�riques

        ///<summary>
        /// Niveau de d�tail orient� produit
        /// </summary>
        ///  <label>_genericDetailLevelType</label>
        [System.NonSerialized]
        protected GenericDetailLevel _genericProductDetailLevel = null;
        ///<summary>
        /// Niveau de d�tail orient� media
        /// </summary>
        ///  <remarks>
        /// Il peut contenir des produits pour les plans media
        /// </remarks>
        ///  <label>_genericMediaDetailLevel</label>
        [System.NonSerialized]
        protected GenericDetailLevel _genericMediaDetailLevel = null;
        ///<summary>
        /// Niveau de d�tail orient� selection media
        /// </summary>       
        ///  <label>_genericMediaSelectionDetailLevel</label>
        [System.NonSerialized]
        protected GenericDetailLevel _genericMediaSelectionDetailLevel = null;
        ///<summary>
        /// AdNetTrack Detail levels
        /// </summary>
        ///  <remarks>
        /// It can contain media or product detail levels
        /// </remarks>
        ///  <label>_genericAdNetTrackDetailLevel</label>
        [System.NonSerialized]
        protected GenericDetailLevel _genericAdNetTrackDetailLevel = null;
        ///<summary>
        /// Niveau de d�tail g�n�rique
        /// </summary>
        ///  <label>_genericDetailLevel</label>
        [System.NonSerialized]
        protected GenericDetailLevel _genericDetailLevel = null;
        ///<summary>
        /// Niveau de d�tail colonne g�n�rique
        /// </summary>
        ///  <label>_genericColumnDetailLevel</label>
        [System.NonSerialized]
        protected GenericDetailLevel _genericColumnDetailLevel = null;
        #endregion

        #region Colonnes g�n�riques
        /// <summary>
        /// Colonnes g�n�riques
        /// </summary>
        [System.NonSerialized]
        protected GenericColumns _genericColumns = null;
        /// <summary>
        /// Colonnes g�n�riques
        /// </summary>
        [System.NonSerialized]
        protected GenericColumns _genericCreativesColumns = null;
        #endregion

        #region Media Selection Parent
        /// <summary>
        /// Media Selection Parent
        /// </summary>
        [System.NonSerialized]
        protected DetailLevelItemInformation.Levels _mediaSelectionParent;
        #endregion

        #region P�riode s�lection�e
        /// <summary>
        /// P�riode s�lectionn�e � partir de GlobalDateSelection
        /// </summary>
        /// <label>_customerPeriod</label>
        [System.NonSerialized]
        protected CustomerPeriod _customerPeriod = null;
        #endregion

        #region Univers de l'etude (genres �missions/�missions)
        /// <summary>
        /// S�lection original des genres d'�missions de l'univers courant
        /// </summary>
        [System.NonSerialized]
        protected TreeNode _selectionUniversProgramType = new TreeNode("programtype");
        /// <summary>
        /// S�lection courante des genres d'�missions de l'univers courant
        /// </summary>
        [System.NonSerialized]
        protected TreeNode _currentUniversProgramType = new TreeNode("programtype");
        #endregion

        #region Univers de l'�tude (Forme de parrainages)
        /// <summary>
        /// S�lection original des formes de parrainages de l'univers courant
        /// </summary>
        [System.NonSerialized]
        protected TreeNode _selectionUniversSponsorshipForm = new TreeNode("sponsorshipform");
        /// <summary>
        /// S�lection courante des formes de parrainages de l'univers courant
        /// </summary>
        [System.NonSerialized]
        protected TreeNode _currentUniversSponsorshipForm = new TreeNode("sponsorshipform");
        #endregion

        #region AdNetTrack
        /// <summary>
        /// AdNetTrack Selection
        /// </summary>
        [System.NonSerialized]
        protected AdNetTrackProductSelection _adNetTrackSelection = null;
        #endregion

        #region Param�tres de la session
        /// <summary>
        /// Contient les nouvelles variables sessions
        /// </summary>
        protected Hashtable userParameters = new Hashtable();
        #endregion

        /// <summary>
        /// Customer data filters
        /// </summary>
        [System.NonSerialized]
        TNS.AdExpress.Web.Core.CustomerDataFilters _customerDataFilters = null;

        #region Default promotion Personnalized level
        /// <summary>
        /// Default promotion Personnalized level
        /// </summary>
        [System.NonSerialized]
        protected DetailLevelItemInformation.Levels _personnalizedLevel;
        #endregion

        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public WebSession()
        {
        }
        /// <summary>
        /// Constructeur de la classe session
        /// </summary>
        /// <param name="login">Ensemble des droit du client connect�</param>
        public WebSession(TNS.AdExpress.Right login)
        {

            try
            {
                this.customerLogin = login;
                //Construction de l'identifiant de session
                idSession = DateTime.Now.ToString("yyyyMMddHHmmss") + Convert.ToString(customerLogin.IdLogin);
            }
            catch (System.Exception e)
            {
                throw new WebSessionException("WebSession.WebSession(...) : Param�tre \"login\" invalide : " + e.Message);
            }
        }

        #endregion

        #region Accesseurs

        #region Acc�s aux donn�es de la Base de donn�es
        /// <summary>
        /// Acc�s aux donn�es de la Base de donn�es
        /// </summary>
        public IDataSource Source
        {
            get
            {

                if (_source == null)
                    //_source = new OracleDataSource(CustomerLogin.OracleConnectionString);
                    _source = customerLogin.Source;

                return (_source);
            }
            set
            {
                _source = value;
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Session
        /// <summary>
        /// Get/Set Identifiant de session
        /// </summary>
        public string IdSession
        {
            get { return idSession; }
            set
            {
                idSession = value;
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Langues
        /// <summary>
        /// Obtient ou d�finit la langue choisie par l'utilisateur
        /// </summary>
        public int SiteLanguage
        {
            get { return (siteLanguage); }
            set
            {
                siteLanguage = value;
                modificationDate = DateTime.Now;
            }
        }
        /// <summary>
        /// Get data language
        /// </summary>
        public int DataLanguage
        {            
            get
            {
                if (WebApplicationParameters.AllowedLanguages.ContainsKey(siteLanguage))
                    return (WebApplicationParameters.AllowedLanguages[siteLanguage].ClassificationLanguageId);
                else
                {
                    throw (new NullReferenceException("Data language id is null"));
                }
            }
        }
        #endregion

        #region Dates
        /// <summary>
        /// Get Date de d�but de la session (date de connexion)
        /// </summary>
        public DateTime BeginningDate
        {
            get { return beginningDate; }
            set
            {
                beginningDate = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get Date de dern�re modification de la session
        /// </summary>
        public DateTime ModificationDate
        {
            get { return modificationDate; }
            set
            {
                modificationDate = value;
            }
        }

        /// <summary>
        /// Get/Set Ann�e de chargement pour les recaps
        /// </summary>	
        public int DownLoadDate
        {
            get { return downLoadDate; }
            set
            {
                if (downLoadDate != value)
                {
                    downLoadDate = value;
                }
            }
        }

        /// <summary>
        /// Get/Set Dur�e de la p�riode (1 semaine, 5 mois, 2 ans...)
        /// </summary>
        public int PeriodLength
        {
            get { return periodLength; }
            set
            {
                if (periodLength != value)
                {
                    periodLength = value;
                    modificationDate = DateTime.Now;
                }
            }
        }


        /// <summary>
        /// Get/Set Date de d�but de la p�riode AU FORMAT yyyy/mm/dd
        /// </summary>
        public string PeriodBeginningDate
        {
            get { return periodBeginningDate; }
            set
            {
                periodBeginningDate = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set Date de d�but du d�tail de la p�riode AU FORMAT yyyy/mm/dd
        /// </summary>
        public string DetailPeriodBeginningDate
        {
            get { return detailPeriodBeginningDate; }
            set
            {
                detailPeriodBeginningDate = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set Date de d�but de publication AU FORMAT yyyy/mm/dd
        /// </summary>
        public string PublicationBeginningDate
        {
            get { return publicationBeginningDate; }
            set
            {
                publicationBeginningDate = value;
                modificationDate = DateTime.Now;
            }
        }
        /// <summary>
        /// Get/Set Ann�e du fichier d'agences m�dia
        /// </summary>
        public string MediaAgencyFileYear
        {
            get { return mediaAgencyFileYear; }
            set
            {
                mediaAgencyFileYear = value;
                modificationDate = DateTime.Now;
            }
        }
        /// <summary>
        /// Get/Set Date de fin de p�riode AU FORMAT yyyy/mm/dd
        /// </summary>
        public string PeriodEndDate
        {
            get { return periodEndDate; }
            set
            {
                periodEndDate = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set Date de fin de publication AU FORMAT yyyy/mm/dd
        /// </summary>
        public string PublicationEndDate
        {
            get { return publicationEndDate; }
            set
            {
                publicationEndDate = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set Date de fin du d�tail de la p�riode AU FORMAT yyyy/mm/dd
        /// </summary>
        public string DetailPeriodEndDate
        {
            get { return detailPeriodEndDate; }
            set
            {
                detailPeriodEndDate = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set Identifiant de type de p�riode
        /// </summary>
        public Constantes.Web.CustomerSessions.Period.Type PeriodType
        {
            get { return periodType; }
            set
            {
                if (periodType != value)
                {
                    periodType = value;
                    OnSetPeriodType();
                    modificationDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Get/Set Identifiant de type de date de publication
        /// </summary>
        public Constantes.Web.CustomerSessions.Period.publicationType PublicationDateType
        {
            get { return publicationDateType; }
            set
            {
                publicationDateType = value;
                //OnSetPeriodType();
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set date correspondant au dernier mois dispo dans la base des recap
        /// </summary>
        public string LastAvailableRecapMonth
        {
            get { return lastAvailableRecapMonth; }
            set
            {
                lastAvailableRecapMonth = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get customer data filters
        /// </summary>
        public TNS.AdExpress.Web.Core.CustomerDataFilters CustomerDataFilters
        {
            get
            {
                if (_customerDataFilters == null) _customerDataFilters = new CustomerDataFilters(this);
                return _customerDataFilters;
            }
        }
        #endregion

        #region Modules
        /// <summary>
        /// Get/Set Module courant dans la navigation (atteint ou � atteindre)
        /// </summary>
        public Int64 CurrentModule
        {
            get { return currentModule; }
            set
            {
                if (currentModule != value)
                {
                    currentModule = value;
                    OnSetModule();
                    modificationDate = DateTime.Now;
                    // On test les s�lection de support par defaut pour certain module
                    switch (currentModule)
                    {
                        case WebConstantes.Module.Name.JUSTIFICATIFS_PRESSE:
                        case WebConstantes.Module.Name.BILAN_CAMPAGNE:
                        case WebConstantes.Module.Name.DONNEES_DE_CADRAGE:
                        case WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE:
                            OnSetVehicle(Vehicles.names.press.GetHashCode());
                            break;
                        case WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO:
                            OnSetVehicle(Vehicles.names.radio.GetHashCode());
                            break;
                        case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
                        case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
                        case WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION:
                            OnSetVehicle(Vehicles.names.tv.GetHashCode());
                            break;
                        case WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO:
                            OnSetVehicle(Vehicles.names.others.GetHashCode());
                            break;

                    }
                }
            }
        }

        /// <summary>
        /// Get/Set Module courant dans la navigation (atteint ou � atteindre)
        /// </summary>
        public int ModuleTraductionCode
        {
            get { return moduleTraductionCode; }
            set
            {
                moduleTraductionCode = value;
                modificationDate = DateTime.Now;
            }
        }


        /// <summary>
        /// Get/Set reachedModule qui indique si le module courant a d�j� �t� atteint depuis sa s�lection.
        /// </summary>
        public bool ReachedModule
        {
            get { return this.reachedModule; }
            set
            {
                this.reachedModule = value;
            }
        }
        #endregion

        #region R�sultats
        /// <summary>
        /// Get/Set l'adresse de la derni�re page de r�sultat atteinte
        /// </summary>
        public string LastReachedResultUrl
        {
            get { return lastReachedResultUrl; }
            set
            {
                lastReachedResultUrl = value;
            }
        }

        /// <summary>
        /// Get/Set Onglet courant (util pour un module comprenant plusieurs onglets)
        /// </summary>
        public Int64 CurrentTab
        {
            get { return currentTab; }
            set
            {
                if (currentTab != value)
                {
                    currentTab = value;
                    OnSetResult();
                    modificationDate = DateTime.Now;
                }
            }
        }
        #endregion

        #region Droits clients
        /// <summary>
        /// Get Droits client
        /// </summary>
        public TNS.AdExpress.Right CustomerLogin
        {
            get { return customerLogin; }
            set
            {
                customerLogin = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get company Information
        /// </summary>
        public Company LoginCompany
        {
            get
            {
                if (_company == null)
                {
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.companyId) && userParameters.ContainsKey(CoreConstantes.SessionParamters.companyLabel))
                    {
                        _company = new Company((Int64)userParameters[CoreConstantes.SessionParamters.companyId], userParameters[CoreConstantes.SessionParamters.companyLabel].ToString());
                    }
                    else
                    {
                        Login login = new Login(CustomerLogin.IdLogin, CustomerLogin.Login, CustomerLogin.PassWord);

                        login.Source = Source;
                        _company = login.LoginCompany;
                    }
                }
                return (_company);
            }
            set
            {
                _company = value;
                userParameters[CoreConstantes.SessionParamters.companyId] = value.CompanyId;
                userParameters[CoreConstantes.SessionParamters.companyLabel] = value.Label;
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Gestion Fichier pour Anubis
        /// <summary>
        /// Get/Set nom du fichier pdf export�
        /// </summary>	
        public string ExportedPDFFileName
        {
            get { return exportedPDFFileName; }
            set
            {
                if (exportedPDFFileName != value)
                {
                    exportedPDFFileName = value;
                }
            }
        }

        /// <summary>
        /// Get/Set email(s) de(s( destinataire(s) du fichier pdf � exporter
        /// </summary>	
        public string[] EmailRecipient
        {
            get { return emailRecipient; }
            set { emailRecipient = value; }
        }
        #endregion

        #region Univers de l'etude
        /// <summary>
        /// Get/Set s�lection de media de l'univers courant
        /// </summary>
        public TreeNode CurrentUniversMedia
        {
            get { return currentUniversMedia; }
            set
            {
                currentUniversMedia = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set S�lection de produit de l'univers courant
        /// </summary>
        public TreeNode CurrentUniversProduct
        {
            get { return currentUniversProduct; }
            set
            {
                currentUniversProduct = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set S�lection d'annonceurs de l'univers courant
        /// </summary>
        public TreeNode CurrentUniversAdvertiser
        {
            get { return currentUniversAdvertiser; }
            set
            {
                currentUniversAdvertiser = value;
                modificationDate = DateTime.Now;
            }
        }


        /// <summary>
        /// Get/Set s�lection de media de l'univers courant
        /// </summary>
        public TreeNode SelectionUniversMedia
        {
            get { return selectionUniversMedia; }
            set
            {
                selectionUniversMedia = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set S�lection de produit de l'univers courant
        /// </summary>
        public TreeNode SelectionUniversProduct
        {
            get { return selectionUniversProduct; }
            set
            {
                selectionUniversProduct = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set S�lection d'annonceurs de l'univers courant
        /// </summary>
        public TreeNode SelectionUniversAdvertiser
        {
            get { return selectionUniversAdvertiser; }
            set
            {
                selectionUniversAdvertiser = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set s�lection de media de l'univers de reference
        /// </summary>
        public TreeNode ReferenceUniversMedia
        {
            get { return referenceUniversMedia; }
            set
            {
                referenceUniversMedia = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set S�lection de produit de l'univers de reference
        /// </summary>
        public TreeNode ReferenceUniversProduct
        {
            get { return referenceUniversProduct; }
            set
            {
                referenceUniversProduct = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set S�lection d'annonceurs de l'univers de reference
        /// </summary>
        public TreeNode ReferenceUniversAdvertiser
        {
            get { return referenceUniversAdvertiser; }
            set
            {
                referenceUniversAdvertiser = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set s�lection de media de l'univers competiteur
        /// </summary>
        public Hashtable CompetitorUniversMedia
        {
            get { return competitorUniversMedia; }
            set
            {
                competitorUniversMedia = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set S�lection de produit de l'univers competiteur
        /// </summary>
        public Hashtable CompetitorUniversProduct
        {
            get { return competitorUniversProduct; }
            set
            {
                competitorUniversProduct = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set S�lection d'annonceurs de l'univers competiteur
        /// </summary>
        public Hashtable CompetitorUniversAdvertiser
        {
            get { return competitorUniversAdvertiser; }
            set
            {
                competitorUniversAdvertiser = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set treenode temporaire
        /// </summary>
        public TreeNode TemporaryTreenode
        {
            get { return temporaryTreenode; }
            set
            {
                temporaryTreenode = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set la personnalisation des �l�ments concurrents et de r�f�rence
        /// </summary>
        public bool CustomizedReferenceComcurrentElements
        {
            get { return customizedReferenceComcurrentElements; }
            set
            {
                if (customizedReferenceComcurrentElements != value)
                {
                    customizedReferenceComcurrentElements = value;
                    modificationDate = DateTime.Now;
                }
            }
        }


        /// <summary>
        /// Get/Set Criteria of total computing market , sector or working set
        /// </summary>			
        public TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion ComparaisonCriterion
        {
            get { return comparaisonCriterion; }
            set
            {
                if (comparaisonCriterion != value)
                {
                    comparaisonCriterion = value;
                    modificationDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Get/Set if can realized a comparative study
        /// </summary>
        public bool ComparativeStudy
        {
            get { return comparativeStudy; }
            set
            {
                if (comparativeStudy != value)
                {
                    comparativeStudy = value;
                }
            }
        }

        #region univers �tude bilan de campagne (APPM)
        /// <summary>
        /// S�lection courante des cibles AEPM de l'univers courant
        /// </summary>
        public TreeNode CurrentUniversAEPMTarget
        {
            get { return currentUniversAEPMTarget; }
            set
            {
                currentUniversAEPMTarget = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// S�lection originale des cibles AEPM de l'univers courant
        /// </summary>
        public TreeNode SelectionUniversAEPMTarget
        {
            get { return selectionUniversAEPMTarget; }
            set
            {
                selectionUniversAEPMTarget = value;
                modificationDate = DateTime.Now;
            }
        }
        /// <summary>
        /// S�lection courante des vagues AEPM de l'univers courant
        /// </summary>	
        public TreeNode CurrentUniversAEPMWave
        {
            get { return currentUniversAEPMWave; }
            set
            {
                currentUniversAEPMWave = value;
                modificationDate = DateTime.Now;
            }
        }
        /// <summary>
        /// S�lection originale des vagues AEPM de l'univers courant
        /// </summary>	
        public TreeNode SelectionUniversAEPMWave
        {
            get { return selectionUniversAEPMWave; }
            set
            {
                selectionUniversAEPMWave = value;
                modificationDate = DateTime.Now;
            }
        }
        /// <summary>
        /// S�lection courante des vagues OJD de l'univers courant
        /// </summary>		
        public TreeNode CurrentUniversOJDWave
        {
            get { return currentUniversOJDWave; }
            set
            {
                currentUniversOJDWave = value;
                modificationDate = DateTime.Now;
            }
        }
        /// <summary>
        /// S�lection originale des vagues OJD de l'univers courant
        /// </summary>		
        public TreeNode SelectionUniversOJDWave
        {
            get { return selectionUniversOJDWave; }
            set
            {
                selectionUniversOJDWave = value;
                modificationDate = DateTime.Now;
            }
        }

        #region Nouveaux univers produits et medias
        /// <summary>
        ///Get/Set Principal dictionary of  universe product selection
        /// </summary>
        public Dictionary<int, AdExpressUniverse> PrincipalProductUniverses
        {
            get
            {
                return (_principalProductUniverses);
            }
            set
            {
                _principalProductUniverses = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        ///Get/Set secondary dictionary of  universe product selection
        /// </summary>
        public Dictionary<int, AdExpressUniverse> SecondaryProductUniverses
        {
            get
            {
                return (_secondaryProductUniverses);
            }
            set
            {
                _secondaryProductUniverses = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        ///Get/Set Principal dictionary of  universe media selection
        /// </summary>
        public Dictionary<int, AdExpressUniverse> PrincipalMediaUniverses
        {
            get
            {
                return (_principalMediaUniverses);
            }
            set
            {
                _principalMediaUniverses = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        ///Get/Set secondary dictionary of  universe media selection
        /// </summary>
        public Dictionary<int, AdExpressUniverse> SecondaryMediaUniverses
        {
            get
            {
                return (_secondaryMediaUniverses);
            }
            set
            {
                _secondaryMediaUniverses = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        ///Get/Set Principal dictionary of  universe advertising agnecy selection
        /// </summary>
        public Dictionary<int, AdExpressUniverse> PrincipalAdvertisingAgnecyUniverses
        {
            get
            {
                return (_principalAdvertisingAgnecyUniverses);
            }
            set
            {
                _principalAdvertisingAgnecyUniverses = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        ///Get/Set secondary dictionary of  universe advertising agnecy selection
        /// </summary>
        public Dictionary<int, AdExpressUniverse> SecondaryAdvertisingAgnecyUniverses
        {
            get
            {
                return (_secondaryAdvertisingAgnecyUniverses);
            }
            set
            {
                _secondaryAdvertisingAgnecyUniverses = value;
                modificationDate = DateTime.Now;
            }
        }
        /// <summary>
        ///Get/Set Principal dictionary of  universe profession selection
        /// </summary>
        public Dictionary<int, AdExpressUniverse> PrincipalProfessionUniverses
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.principalProfessionUniverses))
                {
                    return Utilities.Converters.ConvertToUniverseDictionary(Dimension.profession, Convert.ToString(userParameters[CoreConstantes.SessionParamters.principalProfessionUniverses]));
                }
                return new Dictionary<int, AdExpressUniverse>();
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.principalProfessionUniverses))
                        userParameters.Remove(CoreConstantes.SessionParamters.principalProfessionUniverses);
                    userParameters.Add(CoreConstantes.SessionParamters.principalProfessionUniverses, Utilities.Converters.ConvertUniverseToString(value));
                }
                else if (userParameters.ContainsKey(CoreConstantes.SessionParamters.principalProfessionUniverses))
                    userParameters.Remove(CoreConstantes.SessionParamters.principalProfessionUniverses);
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        /// <summary>
        ///Get/Set  dictionary of  universe Advertisement Type selection
        /// </summary>
        public Dictionary<int, AdExpressUniverse> AdvertisementTypeUniverses
        {
            get
            {
                Dictionary<int, AdExpressUniverse> list = new Dictionary<int, AdExpressUniverse>();
                AdExpressUniverse adexUniverse = new AdExpressUniverse(TNS.Classification.Universe.Dimension.advertisementType);
                TNS.Classification.Universe.NomenclatureElementsGroup group = null;
                int g = 0;
                //Get Avertisement universe  
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.advertisementTypeIncluded) && userParameters[CoreConstantes.SessionParamters.advertisementTypeIncluded] != null)
                {
                    group = new TNS.Classification.Universe.NomenclatureElementsGroup(g, TNS.Classification.Universe.AccessType.includes);
                    group.AddItems(TNS.Classification.Universe.TNSClassificationLevels.ADVERTISEMENT_TYPE, Convert.ToString(userParameters[CoreConstantes.SessionParamters.advertisementTypeIncluded]));
                    adexUniverse.AddGroup(adexUniverse.Count(), group);
                    g++;
                }
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.advertisementTypeIntersection) && userParameters[CoreConstantes.SessionParamters.advertisementTypeIntersection] != null)
                {
                    group = new TNS.Classification.Universe.NomenclatureElementsGroup(g, TNS.Classification.Universe.AccessType.includes);
                    group.AddItems(TNS.Classification.Universe.TNSClassificationLevels.ADVERTISEMENT_TYPE, Convert.ToString(userParameters[CoreConstantes.SessionParamters.advertisementTypeIntersection]));
                    adexUniverse.AddGroup(adexUniverse.Count(), group);
                    g++;
                }
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.advertisementTypeExcluded) && userParameters[CoreConstantes.SessionParamters.advertisementTypeExcluded] != null)
                {
                    group = new TNS.Classification.Universe.NomenclatureElementsGroup(g, TNS.Classification.Universe.AccessType.excludes);
                    group.AddItems(TNS.Classification.Universe.TNSClassificationLevels.ADVERTISEMENT_TYPE, Convert.ToString(userParameters[CoreConstantes.SessionParamters.advertisementTypeExcluded]));
                    adexUniverse.AddGroup(adexUniverse.Count(), group);
                }

                if (adexUniverse != null && adexUniverse.Count() > 0)
                {
                    list.Add(list.Count, adexUniverse);
                }

                return (list);


            }
            set
            {
                //Empty old selected items                   
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.advertisementTypeIncluded))
                    userParameters.Remove(CoreConstantes.SessionParamters.advertisementTypeIncluded);
                //userParameters[CoreConstantes.SessionParamters.advertisementTypeIncluded] = string.Empty;                
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.advertisementTypeExcluded))
                    userParameters.Remove(CoreConstantes.SessionParamters.advertisementTypeExcluded);
                //userParameters[CoreConstantes.SessionParamters.advertisementTypeExcluded] = string.Empty;
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.advertisementTypeIntersection))
                    userParameters.Remove(CoreConstantes.SessionParamters.advertisementTypeIntersection);
                //userParameters[CoreConstantes.SessionParamters.advertisementTypeIntersection] = string.Empty;

                AdExpressUniverse adexUniverse = (value != null && value.Count > 0) ? value[0] : null;
                if (adexUniverse != null)
                {

                    //Get included intems                  
                    List<TNS.Classification.Universe.NomenclatureElementsGroup> groups = adexUniverse.GetIncludes();
                    if (groups != null && groups.Count > 0)
                    {
                        for (int i = 0; i < groups.Count; i++)
                        {

                            switch (i)
                            {
                                case 0:
                                    if (!userParameters.ContainsKey(CoreConstantes.SessionParamters.advertisementTypeIncluded))
                                        userParameters.Add(CoreConstantes.SessionParamters.advertisementTypeIncluded, groups[i].GetAsString(TNS.Classification.Universe.TNSClassificationLevels.ADVERTISEMENT_TYPE));
                                    else userParameters[CoreConstantes.SessionParamters.advertisementTypeIncluded] = groups[i].GetAsString(TNS.Classification.Universe.TNSClassificationLevels.ADVERTISEMENT_TYPE);
                                    break;
                                case 1:
                                    if (!userParameters.ContainsKey(CoreConstantes.SessionParamters.advertisementTypeIntersection))
                                        userParameters.Add(CoreConstantes.SessionParamters.advertisementTypeIntersection, groups[i].GetAsString(TNS.Classification.Universe.TNSClassificationLevels.ADVERTISEMENT_TYPE));
                                    else userParameters[CoreConstantes.SessionParamters.advertisementTypeIntersection] = groups[i].GetAsString(TNS.Classification.Universe.TNSClassificationLevels.ADVERTISEMENT_TYPE);
                                    break;
                                default: throw (new Exception("Avertisement Type Universes must be limited to 2 included groups"));
                            }

                        }
                    }
                    //Get excluded intems                  
                    groups = adexUniverse.GetExludes();
                    if (groups != null && groups.Count > 0)
                    {
                        for (int i = 0; i < groups.Count; i++)
                        {

                            if (!userParameters.ContainsKey(CoreConstantes.SessionParamters.advertisementTypeExcluded))
                                userParameters.Add(CoreConstantes.SessionParamters.advertisementTypeExcluded, groups[i].GetAsString(TNS.Classification.Universe.TNSClassificationLevels.ADVERTISEMENT_TYPE));
                            else userParameters[CoreConstantes.SessionParamters.advertisementTypeExcluded] = groups[i].GetAsString(TNS.Classification.Universe.TNSClassificationLevels.ADVERTISEMENT_TYPE);

                        }
                    }

                }
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        /// <summary>
        /// Obtient ou d�finit Ecart
        /// </summary>
        public bool Ecart
        {
            get { return ecart; }
            set
            {
                if (ecart != value)
                {
                    ecart = value;
                    modificationDate = DateTime.Now;
                }
            }
        }

        #endregion

        #endregion

        #region Pr�sentation des r�sultats
        /// <summary>
        /// Get/Set Niveau de detail de la p�riode (voir Constantes.cs)
        /// </summary>
        public Constantes.Web.CustomerSessions.Period.DisplayLevel DetailPeriod
        {
            get { return detailPeriod; }
            set
            {
                if (detailPeriod != value)
                {
                    detailPeriod = value;
                    modificationDate = DateTime.Now;
                }
            }
        }


        /// <summary>
        /// Get/Set Unit� utilis� (�, spots...)
        /// </summary>
        public Constantes.Web.CustomerSessions.Unit Unit
        {
            get { return unit; }
            set
            {
                if (unit != value)
                {
                    unit = value;
                    OnSetUnit();
                    modificationDate = DateTime.Now;
                }
            }
        }


        /// <summary>
        /// Get/Set Index utilis� pour trier les r�sultats d'un tableau
        /// </summary>
        [Obsolete("Utiliser au maximum la propri�t� SortKey")]
        public int Sort
        {
            get { return sort; }
            set
            {
                if (sort != value)
                {
                    sort = value;
                    modificationDate = DateTime.Now;
                }
            }
        }
        /// <summary>
        /// Get/Set Ordre de tri : "desc", "asc", "" pour none (util pour les requ�tes BDD)
        /// </summary>
        [Obsolete("Utiliser au maximum la propri�t� Sorting")]
        public string SortOrder
        {
            get
            {
                if (!Sorting.Equals(ResultTable.SortOrder.NONE))
                {
                    return Sorting.ToString();
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// Get/Set la cl� de tri des r�sultats
        /// </summary>
        public ResultTable.SortOrder Sorting
        {
            get
            {
                if (!userParameters.ContainsKey(CoreConstantes.SessionParamters.sortOrderKey))
                {
                    userParameters.Add(CoreConstantes.SessionParamters.sortOrderKey, ResultTable.SortOrder.NONE.GetHashCode());
                }
                return (ResultTable.SortOrder)userParameters[CoreConstantes.SessionParamters.sortOrderKey];
            }
            set
            {
                if (!userParameters.ContainsKey(CoreConstantes.SessionParamters.sortOrderKey))
                {
                    userParameters.Add(CoreConstantes.SessionParamters.sortOrderKey, value.GetHashCode());
                }
                else
                {
                    userParameters[CoreConstantes.SessionParamters.sortOrderKey] = value.GetHashCode();
                }
            }
        }

        /// <summary>
        /// Get/Set La cl� de tri des r�sultats
        /// </summary>
        public string SortKey
        {
            get
            {
                if (!userParameters.ContainsKey(CoreConstantes.SessionParamters.sortKeyKey))
                {
                    userParameters.Add(CoreConstantes.SessionParamters.sortKeyKey, string.Empty);
                }
                return userParameters[CoreConstantes.SessionParamters.sortKeyKey].ToString();
            }
            set
            {
                if (!userParameters.ContainsKey(CoreConstantes.SessionParamters.sortKeyKey))
                {
                    userParameters.Add(CoreConstantes.SessionParamters.sortKeyKey, value);
                }
                else
                {
                    userParameters[CoreConstantes.SessionParamters.sortKeyKey] = value;
                }
            }
        }
        /// <summary>
        /// Get/Set r�sultats exprim�s en pourcentage
        /// </summary>
        public bool Percentage
        {
            get { return percentage; }
            set
            {
                if (percentage != value)
                {
                    percentage = value;
                    modificationDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Obtient et d�finit le type d'aligement des r�sultats en pourcentages
        /// </summary>
        public WebConstantes.Percentage.Alignment PercentageAlignment
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.percentageAlignment))
                {
                    _percentageAlignment = (WebConstantes.Percentage.Alignment)userParameters[CoreConstantes.SessionParamters.percentageAlignment];
                }

                return (_percentageAlignment);
            }
            set
            {
                _percentageAlignment = value;
                userParameters[CoreConstantes.SessionParamters.percentageAlignment] = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get or Set the Comparative Period Type
        /// </summary>
        public WebConstantes.globalCalendar.comparativePeriodType ComparativePeriodType
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.msComparativePeriodType))
                {
                    _comparativePeriodType = (WebConstantes.globalCalendar.comparativePeriodType)userParameters[CoreConstantes.SessionParamters.msComparativePeriodType];
                }

                return (_comparativePeriodType);
            }
            set
            {
                _comparativePeriodType = value;
                userParameters[CoreConstantes.SessionParamters.msComparativePeriodType] = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get or Set Is Select Retailer Display
        /// </summary>
        public bool IsRetailerDisplay
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.retailerDisplay))
                {
                    try
                    {
                        return (bool)userParameters[CoreConstantes.SessionParamters.retailerDisplay];
                    }
                    catch
                    {
                        userParameters[CoreConstantes.SessionParamters.retailerDisplay] = false;
                    }
                }
                return false;
            }
            set
            {
                userParameters[CoreConstantes.SessionParamters.retailerDisplay] = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get or Set Is Select Retailer Display
        /// </summary>
        public bool IsSelectRetailerDisplay
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.selectRetailerDisplay))
                {
                    try
                    {
                        return (bool)userParameters[CoreConstantes.SessionParamters.selectRetailerDisplay];
                    }
                    catch
                    {
                        userParameters[CoreConstantes.SessionParamters.selectRetailerDisplay] = false;
                    }
                }
                return false;
            }
            set
            {
                userParameters[CoreConstantes.SessionParamters.selectRetailerDisplay] = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get or Set the Period Selection Type
        /// </summary>
        public WebConstantes.globalCalendar.periodSelectiontype PeriodSelectionType
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.periodSelectiontype))
                {
                    _periodSelectionType = (WebConstantes.globalCalendar.periodSelectiontype)userParameters[CoreConstantes.SessionParamters.periodSelectiontype];
                }

                return (_periodSelectionType);
            }
            set
            {
                _periodSelectionType = value;
                userParameters[CoreConstantes.SessionParamters.periodSelectiontype] = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get or Set Evaliant Country Access List
        /// </summary>
        public string EvaliantCountryAccessList {
            get {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.evaliantCountryAccessList)) {
                    _evaliantCountryAccessList = userParameters[CoreConstantes.SessionParamters.evaliantCountryAccessList].ToString();
                }

                return (_evaliantCountryAccessList);
            }
            set {
                _evaliantCountryAccessList = value;
                userParameters[CoreConstantes.SessionParamters.evaliantCountryAccessList] = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get or Set Selected Banners Foramt List
        /// </summary>
        public string SelectedBannersFormatList
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.selectedBannersFormatList))
                {
                    _selectedBannersForamtList = userParameters[CoreConstantes.SessionParamters.selectedBannersFormatList].ToString();
                }

                return (_selectedBannersForamtList);
            }
            set
            {
                _selectedBannersForamtList = value;
                userParameters[CoreConstantes.SessionParamters.selectedBannersFormatList] = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get or Set Selected Purchase Mode List
        /// </summary>
        public string SelectedPurchaseModeList {
            get {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.selectedPurchaseModeList)) {
                    _selectedPurchaseModeList = userParameters[CoreConstantes.SessionParamters.selectedPurchaseModeList].ToString();
                }

                return (_selectedPurchaseModeList);
            }
            set {
                _selectedPurchaseModeList = value;

                if (_selectedPurchaseModeList.Length == 0) {
                    var purchaseModeList = new List<FilterItem>(PurchaseModeList.GetList().Values);
                    _selectedPurchaseModeList = string.Join(",", purchaseModeList.FindAll(p => p.IsEnable).ConvertAll(p => p.Id.ToString()).ToArray());
                }

                userParameters[CoreConstantes.SessionParamters.selectedPurchaseModeList] = _selectedPurchaseModeList;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set le d�tail pr�format� de la nomenclature media dans les tableaux 
        /// </summary>
        public TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails PreformatedMediaDetail
        {
            get { return preformatedMediaDetail; }
            set
            {
                if (preformatedMediaDetail != value)
                {
                    preformatedMediaDetail = value;
                    modificationDate = DateTime.Now;
                }
            }
        }


        /// <summary>
        /// Get/Set le d�tail pr�format� de la nomenclature produit dans les tableaux 
        /// </summary>
        public TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails PreformatedProductDetail
        {
            get { return preformatedProductDetail; }
            set
            {
                if (preformatedProductDetail != value)
                {
                    preformatedProductDetail = value;
                    OnSetMediaAgency();
                    modificationDate = DateTime.Now;
                }
            }
        }


        /// <summary>
        /// Get/Set le d�tail pr�format� des p�riodes dans les tableaux 
        /// </summary>
        public TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedPeriodDetails PreformatedPeriodDetail
        {
            get { return preformatedPeriodDetail; }
            set
            {
                if (preformatedPeriodDetail != value)
                {
                    preformatedPeriodDetail = value;
                    modificationDate = DateTime.Now;
                }
            }
        }


        /// <summary>
        /// Get/Set le pr�formatage des tableaux dynamiques 
        /// </summary>
        public TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables PreformatedTable
        {
            get { return preformatedTable; }
            set
            {
                if (preformatedTable != value)
                {
                    preformatedTable = value;
                    modificationDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Get/Set PDM qui indique si on utilise la part de march� ou non 
        /// </summary>
        public bool PDM
        {
            get { return pdm; }
            set
            {
                if (pdm != value)
                {
                    pdm = value;
                    modificationDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Get \Set Date to Date comparative week
        /// </summary>
        public bool DateToDateComparativeWeek {
            get {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.dateToDateComparativeType)) {
                    _dateToDateComparativeWeek = (bool)userParameters[CoreConstantes.SessionParamters.dateToDateComparativeType];
                }

                return (_dateToDateComparativeWeek);
            }
            set {
                _dateToDateComparativeWeek = value;
                userParameters[CoreConstantes.SessionParamters.dateToDateComparativeType] = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Get/Set PDV qui indique si on utilise la part de voix ou non 
        /// </summary>
        public bool PDV
        {
            get { return pdv; }
            set
            {
                if (pdv != value)
                {
                    pdv = value;
                    modificationDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Get/Set Evolution qui indique si on indique une �volution (quelconque) ou non 
        /// </summary>
        public bool Evolution
        {
            get { return evolution; }
            set
            {
                if (evolution != value)
                {
                    evolution = value;
                    modificationDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Get/Set l'affichage uniquement des �l�m�nts personnalis�s (exemple : dans les tabelaux dynamiques, 
        /// uniquemenent les �l�ments de r�f�rences et cocurrents
        /// </summary>
        public bool PersonalizedElementsOnly
        {
            get { return personalizedElementsOnly; }
            set
            {
                if (personalizedElementsOnly != value)
                {
                    personalizedElementsOnly = value;
                    modificationDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Get/Set AutopromoEvaliant qui indique si on utilise l'auto-promo ou non pour Evaliant
        /// </summary>
        public bool AutopromoEvaliant
        {
            get { return autopromoEvaliant; }
            set
            {
                if (autopromoEvaliant != value)
                {
                    autopromoEvaliant = value;
                    modificationDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Get/Set l'affichage uniquement des �l�m�nts personnalis�s (exemple : dans les tableaux dynamiques, 
        /// uniquemenent les �l�ments de r�f�rences et cocurrents
        /// </summary>
        public bool Graphics
        {
            get { return graphics; }
            set
            {
                if (graphics != value)
                {
                    graphics = value;
                    modificationDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Obtient/d�finit le type d'encart (Total, hors Encarts, Encarts)
        /// </summary>
        public TNS.AdExpress.Constantes.Web.CustomerSessions.Insert Insert
        {
            get { return insert; }
            set
            {
                if (insert != value)
                {
                    insert = value;
                    modificationDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Get \Set Auto Promo 
        /// </summary>
        public TNS.AdExpress.Constantes.Web.CustomerSessions.AutoPromo AutoPromo {
            get {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.autoPromo)) {
                    _autoPromo = (WebConstantes.CustomerSessions.AutoPromo)userParameters[CoreConstantes.SessionParamters.autoPromo];
                }

                return (_autoPromo);
            }
            set {
                _autoPromo = value;
                userParameters[CoreConstantes.SessionParamters.autoPromo] = value;
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Obtient/d�finit le type de nouveau produit
        /// </summary>
        public TNS.AdExpress.Constantes.Web.CustomerSessions.NewProduct NewProduct
        {
            get { return newProduct; }
            set
            {
                if (newProduct != value)
                {
                    newProduct = value;
                    modificationDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Obtient/d�finit le type de nouveau produit
        /// </summary>
        public ProductLevelSelection ProductDetailLevel
        {
            get { return productDetailLevel; }
            set
            {
                if (productDetailLevel != value)
                {
                    productDetailLevel = value;
                    modificationDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Obtient/d�finit le type de nouveau produit
        /// </summary>
        public MediaLevelSelection MediaDetailLevel
        {
            get { return mediaDetailLevel; }
            set
            {
                if (mediaDetailLevel != value)
                {
                    mediaDetailLevel = value;
                    modificationDate = DateTime.Now;
                }
            }
        }

        #region Accesseurs R�partition
        /// <summary>
        /// Obtient /d�finit le format spot
        /// </summary>
        public WebConstantes.Repartition.Format Format
        {
            get { return format; }
            set
            {
                if (format != value)
                {
                    format = value;
                    modificationDate = DateTime.Now;
                }
            }
        }
        /// <summary>
        /// Obtient/d�finit le jour nomm�
        /// </summary>
        public WebConstantes.Repartition.namedDay NamedDay
        {
            get { return namedDay; }
            set
            {
                if (namedDay != value)
                {
                    namedDay = value;
                    modificationDate = DateTime.Now;
                }
            }
        }
        /// <summary>
        /// Obtient/d�finit la tranche horaire
        /// </summary>
        public WebConstantes.Repartition.timeInterval TimeInterval
        {
            get { return timeInterval; }
            set
            {
                if (timeInterval != value)
                {
                    timeInterval = value;
                    modificationDate = DateTime.Now;
                }
            }
        }
        #endregion

        #endregion

        #region Visuels
        /// <summary>
        /// Obtient ou d�finit la version pour faire un zoom
        /// </summary>
        public Int64 SloganIdZoom
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.sloganIdZoom))
                {
                    return ((Int64)userParameters[CoreConstantes.SessionParamters.sloganIdZoom]);
                }
                else
                {
                    return (long.MinValue);
                }
            }
            set
            {
                userParameters[CoreConstantes.SessionParamters.sloganIdZoom] = value;
                modificationDate = DateTime.Now;
            }
        }
        /// <summary>
        /// Liste des visuels
        /// </summary>
        public ArrayList Visuals
        {
            get { return visuals; }
            set
            {
                visuals = value;
                modificationDate = DateTime.Now;
            }
        }
        /// <summary>
        /// Obtient ou d�finit l'identifiant de la Promotion
        /// </summary>
        public Int64 IdPromotion
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.idPromotion))
                {
                    return ((Int64)userParameters[CoreConstantes.SessionParamters.idPromotion]);
                }
                else
                {
                    return (-1);
                }
            }
            set
            {
                userParameters[CoreConstantes.SessionParamters.idPromotion] = value;
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Gestion des accroches
        /// <summary>
        /// Liste destin�e � contenir les identifiants accroches de personnalisation dans les plans m�dias
        /// </summary>
        public ArrayList IdSlogans
        {
            get { return _idSlogans; }
            set
            {
                _idSlogans = value;
                modificationDate = DateTime.Now;
            }
        }
        /// <summary>
        /// Obtient la liste des identifiants accroches de personnalisation dans les plans m�dias
        /// </summary>
        public String SloganIdList
        {
            get
            {
                string slogans = "";
                if (_idSlogans != null)
                {
                    foreach (Int64 currentSlogan in _idSlogans)
                        slogans += currentSlogan.ToString() + ",";
                    if (slogans.Length > 0) slogans = slogans.Substring(0, slogans.Length - 1);
                }
                return slogans;
            }
        }

        /// <summary>
        /// Hashtable contenant une liste d'accroches en cl�s et des couleurs associ�es en valeurs
        /// </summary>
        public Hashtable SloganColors
        {
            get { return _sloganColors; }
            set
            {
                _sloganColors = value;
                modificationDate = DateTime.Now;
            }
        }


        #endregion

        #region AdNetTrack
        /// <summary>
        /// Get AdNetTrack Selection for media schedule
        /// </summary>
        public AdNetTrackProductSelection AdNetTrackSelection
        {
            get
            {
                TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type adNetTrackSelectionType;
                Int64 adNetTrackSelectionId;
                if (_adNetTrackSelection == null)
                {
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.adNetTrackSelectionType))
                    {
                        adNetTrackSelectionType = (TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type)userParameters[CoreConstantes.SessionParamters.adNetTrackSelectionType];
                    }
                    else
                    {
                        throw (new NullReferenceException("AdNetTrack selection type is null"));
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.adNetTrackSelectionId))
                    {
                        adNetTrackSelectionId = (Int64)userParameters[CoreConstantes.SessionParamters.adNetTrackSelectionId];
                    }
                    else
                    {
                        throw (new NullReferenceException("AdNetTrack selection id is null"));
                    }
                    _adNetTrackSelection = new AdNetTrackProductSelection(adNetTrackSelectionType, adNetTrackSelectionId);
                }
                return (_adNetTrackSelection);
            }
            set
            {
                _adNetTrackSelection = value;
                userParameters[CoreConstantes.SessionParamters.adNetTrackSelectionType] = value.SelectionType;
                userParameters[CoreConstantes.SessionParamters.adNetTrackSelectionId] = value.Id;
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Niveaux de d�tail g�n�riques
        /// <summary>
        /// Obtient et d�finit le niveau de d�tail g�n�rique
        /// </summary>
        public GenericDetailLevel DetailLevel
        {
            get
            {
                ArrayList levelIds = null;
                WebConstantes.GenericDetailLevel.SelectedFrom selectedFrom;
                WebConstantes.GenericDetailLevel.Type type;
                if (_genericDetailLevel == null)
                {
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericDetailLevel))
                    {
                        levelIds = (ArrayList)userParameters[CoreConstantes.SessionParamters.genericDetailLevel];
                    }
                    else
                    {
                        // TODO chargement du niveau de d�tail par d�faut en fonction du module
                        throw (new NotImplementedException("Undefine default GenericDetailLevel"));
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericDetailLevelSelectedFrom))
                    {
                        selectedFrom = (WebConstantes.GenericDetailLevel.SelectedFrom)userParameters[CoreConstantes.SessionParamters.genericDetailLevelSelectedFrom];
                    }
                    else
                    {
                        selectedFrom = WebConstantes.GenericDetailLevel.SelectedFrom.unknown;
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericDetailLevelType))
                    {
                        type = (WebConstantes.GenericDetailLevel.Type)userParameters[CoreConstantes.SessionParamters.genericDetailLevelType];
                    }
                    else
                    {
                        type = WebConstantes.GenericDetailLevel.Type.unknown;
                    }
                    _genericDetailLevel = new GenericDetailLevel(levelIds, selectedFrom, type);
                }
                return (_genericDetailLevel);
            }
            set
            {
                _genericDetailLevel = value;
                userParameters[CoreConstantes.SessionParamters.genericDetailLevel] = value.LevelIds;
                userParameters[CoreConstantes.SessionParamters.genericDetailLevelSelectedFrom] = value.FromControlItem;
                userParameters[CoreConstantes.SessionParamters.genericDetailLevelType] = value.Type;
                OnSetMediaAgencyInGenericDetailLevel(_genericDetailLevel);
                modificationDate = DateTime.Now;
            }
        }
        /// <summary>
        /// Obtient et d�finit le niveau de d�tail orient� produit
        /// </summary>
        public GenericDetailLevel GenericProductDetailLevel
        {
            get
            {
                ArrayList levelIds = null;
                WebConstantes.GenericDetailLevel.SelectedFrom selectedFrom;
                WebConstantes.GenericDetailLevel.Type type;
                if (_genericProductDetailLevel == null)
                {
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericProductDetailLevel))
                    {
                        levelIds = (ArrayList)userParameters[CoreConstantes.SessionParamters.genericProductDetailLevel];
                    }
                    else
                    {
                        // TODO chargement du niveau de d�tail par d�faut en fonction du module
                        throw (new NotImplementedException("Undefine default GenericProductDetailLevel"));
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericProductDetailLevelSelectedFrom))
                    {
                        selectedFrom = (WebConstantes.GenericDetailLevel.SelectedFrom)userParameters[CoreConstantes.SessionParamters.genericProductDetailLevelSelectedFrom];
                    }
                    else
                    {
                        selectedFrom = WebConstantes.GenericDetailLevel.SelectedFrom.unknown;
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericDetailLevelType))
                    {
                        type = (WebConstantes.GenericDetailLevel.Type)userParameters[CoreConstantes.SessionParamters.genericDetailLevelType];
                    }
                    else
                    {
                        type = WebConstantes.GenericDetailLevel.Type.unknown;
                    }
                    _genericProductDetailLevel = new GenericDetailLevel(levelIds, selectedFrom, type);
                }
                return (_genericProductDetailLevel);
            }
            set
            {
                _genericProductDetailLevel = value;
                userParameters[CoreConstantes.SessionParamters.genericProductDetailLevel] = value.LevelIds;
                userParameters[CoreConstantes.SessionParamters.genericProductDetailLevelSelectedFrom] = value.FromControlItem;
                userParameters[CoreConstantes.SessionParamters.genericDetailLevelType] = value.Type;
                OnSetMediaAgencyInGenericDetailLevel(_genericProductDetailLevel);
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Obtient et d�finit le niveau de d�tail orient� Media
        /// </summary>
        public GenericDetailLevel GenericMediaDetailLevel
        {
            get
            {
                ArrayList levelIds = null;
                WebConstantes.GenericDetailLevel.SelectedFrom selectedFrom;
                WebConstantes.GenericDetailLevel.Type type;
                if (_genericMediaDetailLevel == null)
                {
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericMediaDetailLevel))
                    {
                        levelIds = (ArrayList)userParameters[CoreConstantes.SessionParamters.genericMediaDetailLevel];
                    }
                    else
                    {
                        // TODO chargement du niveau de d�tail par d�faut en fonction du module
                        throw (new NotImplementedException("Undefine default GenericMediaDetailLevel"));
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericMediaDetailLevelSelectedFrom))
                    {
                        selectedFrom = (WebConstantes.GenericDetailLevel.SelectedFrom)userParameters[CoreConstantes.SessionParamters.genericMediaDetailLevelSelectedFrom];
                    }
                    else
                    {
                        selectedFrom = WebConstantes.GenericDetailLevel.SelectedFrom.unknown;
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericDetailLevelType))
                    {
                        type = (WebConstantes.GenericDetailLevel.Type)userParameters[CoreConstantes.SessionParamters.genericDetailLevelType];
                    }
                    else
                    {
                        type = WebConstantes.GenericDetailLevel.Type.unknown;
                    }
                    _genericMediaDetailLevel = new GenericDetailLevel(levelIds, selectedFrom, type);
                }
                return (_genericMediaDetailLevel);
            }
            set
            {
                _genericMediaDetailLevel = value;
                userParameters[CoreConstantes.SessionParamters.genericMediaDetailLevel] = value.LevelIds;
                userParameters[CoreConstantes.SessionParamters.genericMediaDetailLevelSelectedFrom] = value.FromControlItem;
                userParameters[CoreConstantes.SessionParamters.genericDetailLevelType] = value.Type;
                OnSetMediaAgencyInGenericDetailLevel(_genericMediaDetailLevel);
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Obtient et d�finit le niveau de d�tail orient� Media
        /// </summary>
        public GenericDetailLevel GenericMediaSelectionDetailLevel
        {
            get
            {
                ArrayList levelIds = null;
                WebConstantes.GenericDetailLevel.SelectedFrom selectedFrom;
                WebConstantes.GenericDetailLevel.Type type;
                if (_genericMediaSelectionDetailLevel == null)
                {
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericMediaSelectionDetailLevel))
                    {
                        levelIds = (ArrayList)userParameters[CoreConstantes.SessionParamters.genericMediaSelectionDetailLevel];
                    }
                    else
                    {
                        // TODO chargement du niveau de d�tail par d�faut en fonction du module
                        throw (new NotImplementedException("Undefine default genericMediaSelectionDetailLevel"));
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericMediaSelectionDetailLevelSelectedFrom))
                    {
                        selectedFrom = (WebConstantes.GenericDetailLevel.SelectedFrom)userParameters[CoreConstantes.SessionParamters.genericMediaSelectionDetailLevelSelectedFrom];
                    }
                    else
                    {
                        selectedFrom = WebConstantes.GenericDetailLevel.SelectedFrom.unknown;
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericSelectionDetailLevelType))
                    {
                        type = (WebConstantes.GenericDetailLevel.Type)userParameters[CoreConstantes.SessionParamters.genericSelectionDetailLevelType];
                    }
                    else
                    {
                        type = WebConstantes.GenericDetailLevel.Type.unknown;
                    }
                    _genericMediaSelectionDetailLevel = new GenericDetailLevel(levelIds, selectedFrom, type);
                }
                return (_genericMediaSelectionDetailLevel);
            }
            set
            {
                _genericMediaSelectionDetailLevel = value;
                userParameters[CoreConstantes.SessionParamters.genericMediaSelectionDetailLevel] = value.LevelIds;
                userParameters[CoreConstantes.SessionParamters.genericMediaSelectionDetailLevelSelectedFrom] = value.FromControlItem;
                userParameters[CoreConstantes.SessionParamters.genericSelectionDetailLevelType] = value.Type;
                OnSetMediaAgencyInGenericDetailLevel(_genericMediaSelectionDetailLevel);
                modificationDate = DateTime.Now;
            }
        }


        /// <summary>
        /// Get/ Set AdNetTrack Detail Levels
        /// </summary>
        public GenericDetailLevel GenericAdNetTrackDetailLevel
        {
            get
            {
                ArrayList levelIds = null;
                WebConstantes.GenericDetailLevel.SelectedFrom selectedFrom;
                WebConstantes.GenericDetailLevel.Type type;
                if (_genericAdNetTrackDetailLevel == null)
                {
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericAdNetTrackDetailLevel))
                    {
                        levelIds = (ArrayList)userParameters[CoreConstantes.SessionParamters.genericAdNetTrackDetailLevel];
                    }
                    else
                    {
                        // TODO chargement du niveau de d�tail par d�faut en fonction du module
                        throw (new NotImplementedException("Undefine default GenericMediaDetailLevel"));
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericAdNetTrackDetailLevelSelectedFrom))
                    {
                        selectedFrom = (WebConstantes.GenericDetailLevel.SelectedFrom)userParameters[CoreConstantes.SessionParamters.genericAdNetTrackDetailLevelSelectedFrom];
                    }
                    else
                    {
                        selectedFrom = WebConstantes.GenericDetailLevel.SelectedFrom.unknown;
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericAdNetTrackDetailLevelType))
                    {
                        type = (WebConstantes.GenericDetailLevel.Type)userParameters[CoreConstantes.SessionParamters.genericAdNetTrackDetailLevelType];
                    }
                    else
                    {
                        type = WebConstantes.GenericDetailLevel.Type.unknown;
                    }
                    _genericAdNetTrackDetailLevel = new GenericDetailLevel(levelIds, selectedFrom, type);
                }
                return (_genericAdNetTrackDetailLevel);
            }
            set
            {
                _genericAdNetTrackDetailLevel = value;
                userParameters[CoreConstantes.SessionParamters.genericAdNetTrackDetailLevel] = value.LevelIds;
                userParameters[CoreConstantes.SessionParamters.genericAdNetTrackDetailLevelSelectedFrom] = value.FromControlItem;
                userParameters[CoreConstantes.SessionParamters.genericAdNetTrackDetailLevelType] = value.Type;
                OnSetMediaAgencyInGenericDetailLevel(_genericAdNetTrackDetailLevel);
                modificationDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Obtient et d�finit le niveau de d�tail colonne g�n�rique
        /// </summary>
        public GenericDetailLevel GenericColumnDetailLevel
        {
            get
            {
                ArrayList levelIds = null;
                WebConstantes.GenericDetailLevel.SelectedFrom selectedFrom;
                WebConstantes.GenericDetailLevel.Type type;
                if (_genericColumnDetailLevel == null)
                {
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericColumnDetailLevel))
                    {
                        levelIds = (ArrayList)userParameters[CoreConstantes.SessionParamters.genericColumnDetailLevel];
                    }
                    else
                    {
                        // TODO chargement du niveau de d�tail par d�faut en fonction du module
                        throw (new NotImplementedException("Undefine default GenericColumnDetailLevel"));
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericColumnDetailLevelSelectedFrom))
                    {
                        selectedFrom = (WebConstantes.GenericDetailLevel.SelectedFrom)userParameters[CoreConstantes.SessionParamters.genericColumnDetailLevelSelectedFrom];
                    }
                    else
                    {
                        selectedFrom = WebConstantes.GenericDetailLevel.SelectedFrom.unknown;
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericColumnDetailLevelType))
                    {
                        type = (WebConstantes.GenericDetailLevel.Type)userParameters[CoreConstantes.SessionParamters.genericColumnDetailLevelType];
                    }
                    else
                    {
                        type = WebConstantes.GenericDetailLevel.Type.unknown;
                    }
                    _genericColumnDetailLevel = new GenericDetailLevel(levelIds, selectedFrom, type);
                }
                return (_genericColumnDetailLevel);
            }
            set
            {
                _genericColumnDetailLevel = value;
                userParameters[CoreConstantes.SessionParamters.genericColumnDetailLevel] = value.LevelIds;
                userParameters[CoreConstantes.SessionParamters.genericColumnDetailLevelSelectedFrom] = value.FromControlItem;
                userParameters[CoreConstantes.SessionParamters.genericColumnDetailLevelType] = value.Type;
                modificationDate = DateTime.Now;
            }
        }

        #endregion

        #region Colonnes g�n�riques
        /// <summary>
        /// Obtient et d�finit les colonnes du d�tail des insertions
        /// </summary>
        public GenericColumns GenericInsertionColumns
        {
            get
            {
                List<Int64> columnsIds = null;
                WebConstantes.GenericColumn.SelectedFrom selectedFrom;
                if (_genericColumns == null)
                {
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericColumns))
                    {
                        columnsIds = (List<Int64>)userParameters[CoreConstantes.SessionParamters.genericColumns];
                    }
                    else
                    {
                        throw (new NotImplementedException("Undefine default GenericInsertionColumns"));
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericColumnsSelectedFrom))
                    {
                        selectedFrom = (WebConstantes.GenericColumn.SelectedFrom)userParameters[CoreConstantes.SessionParamters.genericColumnsSelectedFrom];
                    }
                    else
                    {
                        selectedFrom = WebConstantes.GenericColumn.SelectedFrom.unknown;
                    }
                    _genericColumns = new GenericColumns(columnsIds, selectedFrom);
                }
                return (_genericColumns);
            }
            set
            {
                _genericColumns = value;
                userParameters[CoreConstantes.SessionParamters.genericColumns] = value.ColumnIds;
                userParameters[CoreConstantes.SessionParamters.genericColumnsSelectedFrom] = value.FromControlItem;
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Colonnes g�n�riques
        /// <summary>
        /// Obtient et d�finit les colonnes du d�tail des cr�ations
        /// </summary>
        public GenericColumns GenericCreativesColumns
        {
            get
            {
                List<Int64> columnsIds = null;
                WebConstantes.GenericColumn.SelectedFrom selectedFrom;
                if (_genericCreativesColumns == null)
                {
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericCreativesColumns))
                    {
                        columnsIds = (List<Int64>)userParameters[CoreConstantes.SessionParamters.genericCreativesColumns];
                    }
                    else
                    {
                        throw (new NotImplementedException("Undefine default GenericInsertionColumns"));
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.genericCreativesColumnsSelectedFrom))
                    {
                        selectedFrom = (WebConstantes.GenericColumn.SelectedFrom)userParameters[CoreConstantes.SessionParamters.genericCreativesColumnsSelectedFrom];
                    }
                    else
                    {
                        selectedFrom = WebConstantes.GenericColumn.SelectedFrom.unknown;
                    }
                    _genericCreativesColumns = new GenericColumns(columnsIds, selectedFrom);
                }
                return (_genericCreativesColumns);
            }
            set
            {
                _genericCreativesColumns = value;
                userParameters[CoreConstantes.SessionParamters.genericCreativesColumns] = value.ColumnIds;
                userParameters[CoreConstantes.SessionParamters.genericCreativesColumnsSelectedFrom] = value.FromControlItem;
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Media Selection Parent
        /// <summary>
        /// GET / SET Media Selection Parent
        /// </summary>
        public DetailLevelItemInformation.Levels MediaSelectionParent
        {
            get
            {
                if (_mediaSelectionParent.ToString() == "0")
                {
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.mediaSelectionParent))
                    {
                        _mediaSelectionParent = (DetailLevelItemInformation.Levels)userParameters[CoreConstantes.SessionParamters.mediaSelectionParent];
                    }
                }
                return _mediaSelectionParent;

            }
            set
            {
                _mediaSelectionParent = value;
                userParameters[CoreConstantes.SessionParamters.mediaSelectionParent] = value;
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region P�riode s�lectionn�e
        /// <summary>
        /// Obtient ou d�finit la p�riode s�lectionn�e
        /// </summary>
        public CustomerPeriod CustomerPeriodSelected
        {
            get
            {
                bool withComparativePeriod = false, withComparativePeriodPersonnalized = false;
                WebConstantes.globalCalendar.comparativePeriodType comparativeCalendarPeriodType = WebConstantes.globalCalendar.comparativePeriodType.dateToDate;
                WebConstantes.globalCalendar.periodDisponibilityType periodDisponibilityType = WebConstantes.globalCalendar.periodDisponibilityType.currentDay;
                string startDate = "", endDate = "", now = string.Empty, startDateComparative = "", endDateComparative = "";
                int endDateI, periodEndDateI, nowI;
                if (_customerPeriod == null || (_customerPeriod.StartDate.Length == 0 && _customerPeriod.EndDate.Length == 0))
                {
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.startDate))
                    {
                        startDate = (string)userParameters[CoreConstantes.SessionParamters.startDate];
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.endDate))
                    {
                        endDate = (string)userParameters[CoreConstantes.SessionParamters.endDate];
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.withComparativePeriod))
                    {
                        withComparativePeriod = (bool)userParameters[CoreConstantes.SessionParamters.withComparativePeriod];
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.comparativePeriodType))
                    {
                        comparativeCalendarPeriodType = (WebConstantes.globalCalendar.comparativePeriodType)userParameters[CoreConstantes.SessionParamters.comparativePeriodType];
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.periodDisponibilityType))
                    {
                        periodDisponibilityType = (WebConstantes.globalCalendar.periodDisponibilityType)userParameters[CoreConstantes.SessionParamters.periodDisponibilityType];
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.withComparativePeriodPersonnalized))
                    {
                        withComparativePeriodPersonnalized = (bool)userParameters[CoreConstantes.SessionParamters.withComparativePeriodPersonnalized];
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.startDateComparative))
                    {
                        startDateComparative = (string)userParameters[CoreConstantes.SessionParamters.startDateComparative];
                    }
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.endDateComparative))
                    {
                        endDateComparative = (string)userParameters[CoreConstantes.SessionParamters.endDateComparative];
                    }

                    now = DateTime.Now.ToString("yyyyMMdd");
                    endDateI = int.Parse(endDate);
                    periodEndDateI = int.Parse(periodEndDate);
                    nowI = int.Parse(now);
                    if (periodEndDateI > nowI)
                    {
                        if (endDateI < nowI)
                            endDate = now;
                    }
                    else
                        if (periodEndDateI > endDateI)
                            endDate = periodEndDate;

                    if (withComparativePeriodPersonnalized)
                    {
                        _customerPeriod = new CustomerPeriod(startDate, endDate, startDateComparative, endDateComparative);
                    }
                    else
                    {
                        if (withComparativePeriod)
                            _customerPeriod = new CustomerPeriod(startDate, endDate, withComparativePeriod, comparativeCalendarPeriodType, periodDisponibilityType);
                        else
                            _customerPeriod = new CustomerPeriod(startDate, endDate);
                    }
                }
                return (_customerPeriod);
            }
            set
            {
                _customerPeriod = value;
                if (value == null)
                {
                    userParameters.Remove(CoreConstantes.SessionParamters.startDate);
                    userParameters.Remove(CoreConstantes.SessionParamters.endDate);
                    userParameters.Remove(CoreConstantes.SessionParamters.withComparativePeriod);
                    userParameters.Remove(CoreConstantes.SessionParamters.comparativePeriodType);
                    userParameters.Remove(CoreConstantes.SessionParamters.periodDisponibilityType);
                    userParameters.Remove(CoreConstantes.SessionParamters.withComparativePeriodPersonnalized);
                    userParameters.Remove(CoreConstantes.SessionParamters.startDateComparative);
                    userParameters.Remove(CoreConstantes.SessionParamters.endDateComparative);
                }
                else
                {
                    userParameters[CoreConstantes.SessionParamters.startDate] = value.StartDate;
                    userParameters[CoreConstantes.SessionParamters.endDate] = value.EndDate;
                    userParameters[CoreConstantes.SessionParamters.withComparativePeriod] = value.WithComparativePeriod;
                    userParameters[CoreConstantes.SessionParamters.comparativePeriodType] = value.ComparativePeriodType;
                    userParameters[CoreConstantes.SessionParamters.periodDisponibilityType] = value.PeriodDisponibilityType;
                    userParameters[CoreConstantes.SessionParamters.withComparativePeriodPersonnalized] = value.WithComparativePeriodPersonnalized;
                    userParameters[CoreConstantes.SessionParamters.startDateComparative] = value.ComparativeStartDate;
                    userParameters[CoreConstantes.SessionParamters.endDateComparative] = value.ComparativeEndDate;
                }
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Univers de l'etude (genres �missions/�missions)
        /// <summary>
        /// Obtient et d�finit la s�lection original des genres d'�missions de l'univers courant
        /// </summary>
        public TreeNode SelectionUniversProgramType
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.selectionUniversProgramType))
                {
                    _selectionUniversProgramType = (TreeNode)userParameters[CoreConstantes.SessionParamters.selectionUniversProgramType];
                }
                else
                {
                    return (new TreeNode("programtype"));
                }
                return (_selectionUniversProgramType);
            }
            set
            {
                _selectionUniversProgramType = value;
                userParameters[CoreConstantes.SessionParamters.selectionUniversProgramType] = value;
                modificationDate = DateTime.Now;
            }
        }
        /// <summary>
        /// Obtient et d�finit la s�lection courante des genres d'�missions de l'univers courant
        /// </summary>
        public TreeNode CurrentUniversProgramType
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.currentUniversProgramType))
                {
                    _currentUniversProgramType = (TreeNode)userParameters[CoreConstantes.SessionParamters.currentUniversProgramType];
                }
                else
                {
                    return (new TreeNode("programtype"));
                }
                return (_currentUniversProgramType);
            }
            set
            {
                _currentUniversProgramType = value;
                userParameters[CoreConstantes.SessionParamters.currentUniversProgramType] = value;
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Univers de l'etude (forme de parrainages)
        /// <summary>
        /// Obtient et d�finit la s�lection original des formes de parrainages de l'univers courant
        /// </summary>
        public TreeNode SelectionUniversSponsorshipForm
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.selectionUniversSponsorshipForm))
                {
                    _selectionUniversSponsorshipForm = (TreeNode)userParameters[CoreConstantes.SessionParamters.selectionUniversSponsorshipForm];
                }
                else
                {
                    return (new TreeNode("sponsorshipform"));
                }
                return (_selectionUniversSponsorshipForm);
            }
            set
            {
                _selectionUniversSponsorshipForm = value;
                userParameters[CoreConstantes.SessionParamters.selectionUniversSponsorshipForm] = value;
                modificationDate = DateTime.Now;
            }
        }
        /// <summary>
        /// Obtient et d�finit la s�lection courante des genres d'�missions de l'univers courant
        /// </summary>
        public TreeNode CurrentUniversSponsorshipForm
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.currentUniversSponsorshipForm))
                {
                    _currentUniversSponsorshipForm = (TreeNode)userParameters[CoreConstantes.SessionParamters.currentUniversSponsorshipForm];
                }
                else
                {
                    return (new TreeNode("sponsorshipform"));
                }
                return (_currentUniversSponsorshipForm);
            }
            set
            {
                _currentUniversSponsorshipForm = value;
                userParameters[CoreConstantes.SessionParamters.currentUniversSponsorshipForm] = value;
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Pond�ration des unit�s
        /// <summary>
        /// Pond�ration des unit�s
        /// Valeur par d�faut: use=false;value=1.0;displayName=""
        /// </summary>
        /// <remarks>Utilise la table de param�tres dynamique userParameters</remarks>
        public Weighting UnitWeighting
        {
            get
            {
                if (_unitWeighting == null)
                {
                    _unitWeighting = new Weighting();
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.weightingUse)) _unitWeighting.Use = (bool)userParameters[CoreConstantes.SessionParamters.weightingUse];
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.weightingDisplayName)) _unitWeighting.DisplayName = (string)userParameters[CoreConstantes.SessionParamters.weightingDisplayName];
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.weightingValue)) _unitWeighting.Value = (float)userParameters[CoreConstantes.SessionParamters.weightingValue];
                }
                return (_unitWeighting);
            }
            set
            {
                _unitWeighting = value;
                userParameters[CoreConstantes.SessionParamters.weightingUse] = value.Use;
                userParameters[CoreConstantes.SessionParamters.weightingDisplayName] = value.DisplayName;
                userParameters[CoreConstantes.SessionParamters.weightingValue] = value.Value;
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Personnalized level
        /// <summary>
        /// GET / SET promotion Personnalized level
        /// </summary>
        public DetailLevelItemInformation.Levels PersonnalizedLevel
        {
            get
            {
                if (_personnalizedLevel.ToString() == "0")
                {
                    if (userParameters.ContainsKey(CoreConstantes.SessionParamters.personnalizedLevel))
                    {
                        _personnalizedLevel = (DetailLevelItemInformation.Levels)userParameters[CoreConstantes.SessionParamters.personnalizedLevel];
                    }
                }
                return _personnalizedLevel;

            }
            set
            {
                _personnalizedLevel = value;
                userParameters[CoreConstantes.SessionParamters.personnalizedLevel] = value;
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Ne pas utiliser sauf dans monAdExpress
        /// <summary>
        /// Contient les nouvelles variables de session
        /// </summary>
        public Hashtable UserParameters
        {
            get { return (userParameters); }
            set
            {
                userParameters = value;
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Information Navigation
        /// <summary>
        /// Get/Set navigator informations
        /// </summary>
        public string Browser
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.browser))
                    return (userParameters[CoreConstantes.SessionParamters.browser].ToString());
                return ("Not set");
            }
            set { userParameters[CoreConstantes.SessionParamters.browser] = value; }

        }
        /// <summary>
        /// Get/Set navigator informations version
        /// </summary>
        public string BrowserVersion
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.browserVersion))
                    return (userParameters[CoreConstantes.SessionParamters.browserVersion].ToString());
                return ("Not set");
            }
            set { userParameters[CoreConstantes.SessionParamters.browserVersion] = value; }

        }
        #endregion

        /// <summary>
        /// Get/Set navigator user agent version
        /// </summary>
        public string UserAgent
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.userAgent))
                    return (userParameters[CoreConstantes.SessionParamters.userAgent].ToString());
                return ("Not set");
            }
            set { userParameters[CoreConstantes.SessionParamters.userAgent] = value; }

        }

        /// <summary>
        /// Get/Set navigator user agent version
        /// </summary>
        public string CustomerOs
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.customerOs))
                    return (userParameters[CoreConstantes.SessionParamters.customerOs].ToString());
                return ("Not set");
            }
            set { userParameters[CoreConstantes.SessionParamters.customerOs] = value; }

        }

        /// <summary>
        /// Get/Set navigator user agent version
        /// </summary>
        public string CustomerIp
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.customerIp))
                    return (userParameters[CoreConstantes.SessionParamters.customerIp].ToString());
                return ("Not set");
            }
            set { userParameters[CoreConstantes.SessionParamters.customerIp] = value; }

        }

        /// <summary>
        /// Get/Set last url set un WebPage (AdExpress base page) use for customer error
        /// </summary>
        public string LastWebPage
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.lastWebPage))
                    return (userParameters[CoreConstantes.SessionParamters.lastWebPage].ToString());
                return ("Not set");
            }
            set { userParameters[CoreConstantes.SessionParamters.lastWebPage] = value; }

        }

        /// <summary>
        /// Get/Set server name
        /// </summary>
        public string ServerName
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.serverName))
                    return (userParameters[CoreConstantes.SessionParamters.serverName].ToString());
                return ("Not set");
            }
            set { userParameters[CoreConstantes.SessionParamters.serverName] = value; }

        }

        /// <summary>
        /// Get/Set domain name
        /// </summary>
        public string DomainName
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.domainName))
                    return (userParameters[CoreConstantes.SessionParamters.domainName].ToString());
                return ("Not set");
            }
            set { userParameters[CoreConstantes.SessionParamters.domainName] = value; }
        }


        #region Campaign type
        /// <summary>
        /// Get \Set Campaign Type 
        /// </summary>
        public WebConstantes.CustomerSessions.CampaignType CampaignType
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.campaignType))
                {
                    _campaignType = (WebConstantes.CustomerSessions.CampaignType)userParameters[CoreConstantes.SessionParamters.campaignType];
                }

                return (_campaignType);
            }
            set
            {
                _campaignType = value;
                userParameters[CoreConstantes.SessionParamters.campaignType] = value;
                modificationDate = DateTime.Now;
            }
        }
        #endregion

        #region Presence type
        /// <summary>
        /// Get Presence types selected
        /// </summary>
        public List<long> SelectedPresenceTypes
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.presenceType))
                {
                    return (new List<string>(userParameters[CoreConstantes.SessionParamters.presenceType].ToString().Split(','))).ConvertAll(Convert.ToInt64);
                }
                return new List<long>();
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    userParameters[CoreConstantes.SessionParamters.presenceType] = string.Join(",", value);
                }
                else if (userParameters.ContainsKey(CoreConstantes.SessionParamters.presenceType)) 
                    userParameters.Remove(CoreConstantes.SessionParamters.presenceType);
            }
        }
        #endregion

        #region location
        /// <summary>
        /// Get Locations selected
        /// </summary>
        public List<long> SelectedLocations
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.location))
                {
                    return (new List<string>(userParameters[CoreConstantes.SessionParamters.location].ToString().Split(','))).ConvertAll(Convert.ToInt64);
                }
                return new List<long>();
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    userParameters[CoreConstantes.SessionParamters.location] = string.Join(",", value);
                }
                else if (userParameters.ContainsKey(CoreConstantes.SessionParamters.location))
                    userParameters.Remove(CoreConstantes.SessionParamters.location);
            }
        }
        #endregion

        /// <summary>
        /// Get/Set Selected Levels Value
        /// </summary>
        public List<long> SelectedLevelsValue
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.selectedLevelsValue))
                {
                    return (new List<string>(userParameters[CoreConstantes.SessionParamters.selectedLevelsValue].ToString().Split(','))).ConvertAll(Convert.ToInt64);
                }
                return new List<long>();
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    userParameters[CoreConstantes.SessionParamters.selectedLevelsValue] = string.Join(",", value);
                }
                else if (userParameters.ContainsKey(CoreConstantes.SessionParamters.selectedLevelsValue))
                    userParameters.Remove(CoreConstantes.SessionParamters.selectedLevelsValue);
            }
        }

        #region Export creatives options
        /// <summary>
        /// Get \Set Campaign Type 
        /// </summary>
        public List<int> CreativesExportOptions
        {
            get
            {
                if (userParameters.ContainsKey(CoreConstantes.SessionParamters.creativesExportOptions))
                {
                    return (List<int>)userParameters[CoreConstantes.SessionParamters.creativesExportOptions];
                }

                return new List<int>();
            }
            set
            {
                userParameters[CoreConstantes.SessionParamters.creativesExportOptions] = value;
                modificationDate = DateTime.Now;
            }
        }

      
        #endregion

        /// <summary>
        /// Get / Set Exclu Web
        /// </summary>
        public bool IsExcluWeb
        {
            get
            {
                return userParameters.ContainsKey(CoreConstantes.SessionParamters.excluWeb) 
                    && (Convert.ToBoolean(userParameters[CoreConstantes.SessionParamters.excluWeb]));
            }
            set { userParameters[CoreConstantes.SessionParamters.excluWeb] = value; }
        }


        public void CopyFrom(WebSession session)
        {
            if (session != null)
            {
            }
        }

        #region Blob
        /// <summary>
        /// M�thode qui sauvegarde l'objet webSession courant dans la table de sauvegarde des sessions
        ///		Ouverture de la BD
        ///		S�rialisation en m�moire
        ///		Requ�te BD de sauvegarde dnas un Blob
        ///		Lib�ration des ressources
        /// </summary>
        /// <exception cref="TNS.AdExpress.Web.Core.Exceptions.WebSessionException">
        /// Lanc�e dans les cas suivant:
        ///		connection � la BD impossible � ouvrir
        ///		probl�me � la s�rtialisation ou � la sauvegarde de l'objet dans la BD
        ///		impossible de lib�rer les ressources
        /// </exception>
        public void Save()
        {

            customerLogin.ClearModulesList();

            #region On force la mise � jour des donn�es des objets dans la HashTable

            #region Company
            if (_company != null)
            {
                userParameters[CoreConstantes.SessionParamters.companyId] = _company.CompanyId;
                userParameters[CoreConstantes.SessionParamters.companyLabel] = _company.Label;
            }
            #endregion

            #region Niveaux de d�tail

            #region Media
            if (_genericMediaDetailLevel != null)
            {
                userParameters[CoreConstantes.SessionParamters.genericMediaDetailLevel] = _genericMediaDetailLevel.LevelIds;
                userParameters[CoreConstantes.SessionParamters.genericMediaDetailLevelSelectedFrom] = _genericMediaDetailLevel.FromControlItem;
                userParameters[CoreConstantes.SessionParamters.genericDetailLevelType] = _genericMediaDetailLevel.Type;
            }
            #endregion

            #region AdNetTrack
            if (_genericAdNetTrackDetailLevel != null)
            {
                userParameters[CoreConstantes.SessionParamters.genericAdNetTrackDetailLevel] = _genericAdNetTrackDetailLevel.LevelIds;
                userParameters[CoreConstantes.SessionParamters.genericAdNetTrackDetailLevelSelectedFrom] = _genericAdNetTrackDetailLevel.FromControlItem;
                userParameters[CoreConstantes.SessionParamters.genericAdNetTrackDetailLevelType] = _genericAdNetTrackDetailLevel.Type;
            }
            #endregion

            #region Produit
            if (_genericProductDetailLevel != null)
            {
                userParameters[CoreConstantes.SessionParamters.genericProductDetailLevel] = _genericProductDetailLevel.LevelIds;
                userParameters[CoreConstantes.SessionParamters.genericProductDetailLevelSelectedFrom] = _genericProductDetailLevel.FromControlItem;
                userParameters[CoreConstantes.SessionParamters.genericDetailLevelType] = _genericProductDetailLevel.Type;
            }
            #endregion

            #region Ind�fini
            if (_genericDetailLevel != null)
            {
                userParameters[CoreConstantes.SessionParamters.genericDetailLevel] = _genericDetailLevel.LevelIds;
                userParameters[CoreConstantes.SessionParamters.genericDetailLevelSelectedFrom] = _genericDetailLevel.FromControlItem;
                userParameters[CoreConstantes.SessionParamters.genericDetailLevelType] = _genericDetailLevel.Type;
            }
            #endregion


            #endregion

            #region AdNetTrack
            if (_adNetTrackSelection != null)
            {
                userParameters[CoreConstantes.SessionParamters.adNetTrackSelectionType] = _adNetTrackSelection.SelectionType;
                userParameters[CoreConstantes.SessionParamters.adNetTrackSelectionId] = _adNetTrackSelection.Id;
            }
            #endregion

            #endregion

            #region Ouverture de la base de donn�es
            OracleConnection cnx = ((OracleConnection)WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.session).GetSource());
            try
            {
                cnx.Open();
            }
            catch (System.Exception e)
            {
                throw (new WebSessionException("WebSession.Save() : Impossible d'ouvrir la base de donn�es" + e.Message));
            }
            #endregion

            #region S�rialisation et sauvegarde de la session
            OracleCommand sqlCommand = null;
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;

            try
            {
                //"Serialization"
                ms = new MemoryStream();
                bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                binaryData = new byte[ms.GetBuffer().Length];
                binaryData = ms.GetBuffer();

                //create anonymous PL/SQL command
                string label = "";
                label = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.mou01).Label;

                string block = " BEGIN " +
                    " DELETE " + label + "." + Tables.TABLE_SESSION + " WHERE ID_NAV_SESSION=" + this.idSession + "; " +
                    " INSERT INTO " + label + "." + Tables.TABLE_SESSION + "(id_nav_session, nav_session) VALUES(" + this.idSession + ", :1); " +
                    " END; ";

                sqlCommand = new OracleCommand(block);
                sqlCommand.Connection = cnx;
                sqlCommand.CommandType = CommandType.Text;
                //Fill parametres
                OracleParameter param = sqlCommand.Parameters.Add("blobtodb", OracleDbType.Blob);
                param.Direction = ParameterDirection.Input;
                param.Value = binaryData;

                //Execute PL/SQL block
                sqlCommand.ExecuteNonQuery();

            }
            #endregion

            #region Gestion des erreurs dues � la s�rialisation et � la sauvegarde de l'objet
            catch (System.Exception e)
            {
                // Fermeture des structures
                try
                {
                    if (ms != null) ms.Close();
                    if (bf != null) bf = null;
                    if (sqlCommand != null) sqlCommand.Dispose();
                    cnx.Close();
                    cnx.Dispose();
                }
                catch (System.Exception et)
                {
                    throw (new WebSessionException("WebSession.Save() : Impossible de lib�rer les ressources apr�s �chec de la m�thode : " + et.Message));
                }
                throw (new WebSessionException("WebSession.Save() : Echec de la sauvegarde de l'objet dans la base de donn�e : " + e.Message));
            }
            //pas d'erreur
            try
            {
                // Fermeture des structures
                ms.Close();
                bf = null;
                sqlCommand.Dispose();
                cnx.Close();
                cnx.Dispose();
            }
            catch (System.Exception et)
            {
                throw (new WebSessionException("WebSession.Save() : Impossible de fermer la base de donn�es : " + et.Message));
            }

            #endregion

        }


        /// <summary>
        /// M�thode pour la r�cup�ration et la "deserialization" d'un objet WebSession � partir du champ BLOB de la table des sessions
        ///		Ouverture de la BD
        ///		Requ�te BD de s�lection d'un un Blob
        ///		D�s�rialisation
        ///		Lib�ration des ressources
        /// </summary>
        /// <returns>Retourne l'objet r�cup�r� ou null si il y a eu un probl�me non g�r�</returns>
        /// <param name="idWebSession">Identifiant de la session</param>
        /// <exception cref="TNS.AdExpress.Web.Core.Exceptions.WebSessionException">
        /// Lanc�e dans les cas suivant:
        ///		connection � la BD impossible � ouvrir
        ///		probl�me � la s�lection de l'enregistrement ou � la d�s�rialisation
        ///		impossible de lib�rer les ressources
        /// </exception>
        public static Object Load(string idWebSession)
        {

            #region Ouverture de la base de donn�es
            OracleConnection cnx = ((OracleConnection)WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.session).GetSource());
            try
            {
                cnx.Open();
            }
            catch (System.Exception e)
            {
                throw (new WebSessionException("WebSession.Load(...) : Impossible d'ouvrir la base de donn�es : " + e.Message));
            }
            #endregion

            #region Chargement et deserialization de l'objet
            OracleCommand sqlCommand = null;
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;
            Object o = null;
            int i = 0;
            try
            {
                binaryData = new byte[0];
                i = 1;
                //create anonymous PL/SQL command
                string label = "";
                label = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.mou01).Label;

                string block = " BEGIN " +
                    " SELECT nav_session INTO :1 FROM " + label + "." + Tables.TABLE_SESSION + " WHERE id_nav_session = " + idWebSession + "; " +
                    " END; ";
                i = 2;
                sqlCommand = new OracleCommand(block);
                i = 3;
                sqlCommand.Connection = cnx;
                i = 4;
                sqlCommand.CommandType = CommandType.Text;
                i = 5;
                //Initialize parametres
                OracleParameter param = sqlCommand.Parameters.Add("blobfromdb", OracleDbType.Blob);
                i = 6;
                param.Direction = ParameterDirection.Output;
                i = 7;

                //Execute PL/SQL block
                sqlCommand.ExecuteNonQuery();
                i = 8;
                //R�cup�ration des octets du blob
                binaryData = (byte[])((OracleBlob)(sqlCommand.Parameters[0].Value)).Value;
                i = 9;

                //Deserialization oft the object
                ms = new MemoryStream();
                i = 10;
                ms.Write(binaryData, 0, binaryData.Length);
                i = 11;
                bf = new BinaryFormatter();
                i = 12;
                ms.Position = 0;
                i = 13;
                o = bf.Deserialize(ms);
                i = 14;
            }
            #endregion

            #region Gestion des erreurs de chargement et de deserialization de l'objet
            catch (System.Exception e)
            {
                try
                {
                    // Fermeture des structures
                    if (ms != null) ms.Close();
                    if (bf != null) bf = null;
                    if (binaryData != null) binaryData = null;
                    if (sqlCommand != null) sqlCommand.Dispose();
                    cnx.Close();
                }
                catch (System.Exception et)
                {
                    throw (new WebSessionException("WebSession.Load(...) : Impossible de lib�rer les ressources apr�s �chec de la m�thode : " + et.Message));
                }
                throw (new WebSessionException("WebSession.Load(...) : Probl�me au chargement de la session � partir de la base de donn�es : " + e.Message + " " + i.ToString()));
            }
            try
            {
                // Fermeture des structures
                if (ms != null) ms.Close();
                if (bf != null) bf = null;
                if (binaryData != null) binaryData = null;
                if (sqlCommand != null) sqlCommand.Dispose();
                cnx.Close();
            }
            catch (System.Exception et)
            {
                throw (new WebSessionException("WebSession.Load(...) : Impossible de fermer la base de donn�es : " + et.Message));
            }
            #endregion

            //retourne l'objet deserialized ou null si il y a eu un probleme
            return (o);
        }
        #endregion

        #region Obtenir un niveau de nomenclature d'un certain 'type' dans une s�lection 'univers'

        /// <summary>
        /// Get l'ensemble des �l�ments de l'univers v�rifiant certaines caract�ristiques de nomenclature sous forme de chaine de caract�res.
        /// Les �l�ments sont s�par�s par des virgules.
        /// </summary>
        /// <param name="univers">Univers � parcourir</param>
        /// <param name="level">Niveau et type de nomenclature consid�r�</param>
        /// <returns>Cha�ne de caract�re contenant la liste </returns>
        public string GetSelection(TreeNode univers, TNS.AdExpress.Constantes.Customer.Right.type level)
        {
            string list = "";
            constructList(ref list, univers, level);
            return list;
        }

        /// <summary>
        /// Get l'ensemble des �l�ments de l'univers v�rifiant certaines caract�ristiques de nomenclature sous forme de chaine de caract�res.
        /// M�thode r�cursive qui parcoure chaque niveau de chaque branch d'un treenode
        ///		Extraction du niveau de recherche
        ///		Extraction du niveau courant
        ///		Si les niveaux sont diff�rents, on descend d'un niveau dans la branche courante si il y a des fils
        ///		Sinon, on teste si le type d'acc�s est bien le m�me (acces ou exception) et (si e niveau a bien �t� s�lectionn� ou si le niveau n'as pas de fils).
        ///			si c le cas, on ajoute le nom du niveau a la liste des niveaux qui v�rifient toutes ces conditions
        /// </summary>
        /// <param name="list">Chaine de caract�re � remplir</param>
        /// <param name="univers">Univers � parcourir</param>
        /// <param name="level">Niveau et type de nomenclature consid�r� � rechercher</param>
        private void constructList(ref string list, TreeNode univers, TNS.AdExpress.Constantes.Customer.Right.type level)
        {
            string searchLevel;
            //r�cup�ration du niveau � rechercher
            string currentLevel = level.ToString().EndsWith("Access") ? level.ToString().Replace("Access", "") : level.ToString().Replace("Exception", "");

            //r�cup�ration du niveau courant
            if (univers.Tag != null)
            {
                searchLevel = ((LevelInformation)univers.Tag).Type.ToString();
                searchLevel = searchLevel.EndsWith("Access") ? searchLevel.Replace("Access", "") : searchLevel.Replace("Exception", "");
            }
            else searchLevel = "";

            if (searchLevel.CompareTo(currentLevel) != 0)
            {
                //niveaux diff�rents
                if (univers.Nodes != null)
                {
                    //le niveau courant a des fils
                    for (int i = 0; i < univers.Nodes.Count; i++)
                    {
                        //pour chaque fils, on cherche si il n est pas le niveau rechercher
                        constructList(ref list, univers.Nodes[i], level);
                    }
                }
            }
            else
            {
                //niveaux identiques
                if (((LevelInformation)univers.Tag).Type == level &&
                    (univers.Checked || univers.Nodes.Count == 0))
                {
                    //niveau de m�me type ("access" ou "exception"), s�lectionn� par l'utilisateur et sans fils ==> valide
                    list = list + (list.Length > 0 ? "," : "") + ((LevelInformation)univers.Tag).ID.ToString();
                }
            }
        }
        #endregion

        #region Etat de la s�lection des dates, annonceurs, m�dias
        /// <summary>
        /// Indique si les dates ont �t� s�lectionn�es
        /// </summary>
        /// <returns>True si les date de d�but et de fin ont �t� enregistr�es, false sinon</returns>
        public bool isDatesSelected()
        {
            if (this.periodBeginningDate.Length > 0 && periodEndDate.Length > 0) return (true);
            return (false);
        }

        /// <summary>
        /// Indique si une p�riode d'�tude existe
        /// </summary>
        /// <returns>True si les date de d�but et de fin de customerPeriod ont �t� enregistr�es, false sinon</returns>
        public bool isStudyPeriodSelected()
        {
            try
            {
                if (CustomerPeriodSelected != null)
                {
                    if (CustomerPeriodSelected.StartDate.Length > 0 && CustomerPeriodSelected.EndDate.Length > 0) return (true);
                }
                return (false);
            }
            catch (System.Exception) { return (false); }
        }

        /// <summary>
        /// Indique si une p�riode comparative existe
        /// </summary>
        /// <returns>True si les date de d�but (comparative) et de fin (comparative) de customerPeriod ont �t� enregistr�es, false sinon</returns>
        public bool isPeriodComparative()
        {
            try
            {
                if (CustomerPeriodSelected != null)
                {
                    if (CustomerPeriodSelected.ComparativeStartDate.Length > 0 && CustomerPeriodSelected.ComparativeEndDate.Length > 0) return (true);
                }
                return (false);
            }
            catch (System.Exception) { return (false); }
        }

        /// <summary>
        /// Indique si un type de p�riode comparative est utilis�
        /// </summary>
        /// <returns>True si les date de d�but (comparative) et de fin (comparative) de customerPeriod ont �t� enregistr�es, false sinon</returns>
        public bool isComparativePeriodTypeSelected()
        {
            try
            {
                if (CustomerPeriodSelected != null)
                {
                    switch (CustomerPeriodSelected.ComparativePeriodType)
                    {
                        case TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType.comparativeWeekDate:
                        case TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType.dateToDate:
                        case TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType.manual:
                            return true;
                    }
                }
                return (false);
            }
            catch (System.Exception) { return (false); }
        }

        /// <summary>
        /// Indique si un type de disponibilit� des donn�es est utilis�
        /// </summary>
        /// <returns>True si les date de d�but (comparative) et de fin (comparative) de customerPeriod ont �t� enregistr�es, false sinon</returns>
        public bool isPeriodDisponibilityTypeSelected()
        {
            try
            {
                if (CustomerPeriodSelected != null)
                {
                    switch (CustomerPeriodSelected.PeriodDisponibilityType)
                    {
                        case TNS.AdExpress.Constantes.Web.globalCalendar.periodDisponibilityType.currentDay:
                        case TNS.AdExpress.Constantes.Web.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                            return true;
                    }
                }
                return (false);
            }
            catch (System.Exception) { return (false); }
        }

        /// <summary>
        /// Indicate if Profesion has been selected
        /// </summary>
        /// <returns>True Profesion has been selected</returns>
        public bool IsProfessionSelected()
        {

            if (PrincipalProfessionUniverses.Count > 0) return (true);
            return (false);
        }

        #region Annonceurs
        /// <summary>
        /// Indique si des annonceurs ont �t� s�lectionn�s
        /// </summary>
        /// <returns>True si des annonceurs ont �t� enregistr�es, false sinon</returns>
        public bool isAdvertisersSelected()
        {
            if (PrincipalProductUniverses.Count > 0) return (true);
            return (false);
        }

        /// <summary>
        /// Indique si des annonceurs ont �t� s�lectionn�s
        /// </summary>
        /// <returns>True si des annonceurs ont �t� enregistr�es, false sinon</returns>
        public bool isCurrentAdvertisersSelected()
        {
            #region Ancienne version
            //if (this.currentUniversAdvertiser.Nodes.Count > 0) return (true);
            //else return (false);
            #endregion

            if (PrincipalProductUniverses.Count > 0) return (true);
            return (false);
        }

        /// <summary>
        /// Indique si des annonceurs ont �t� s�lectionn�s dans r�f�rences
        /// </summary>
        /// <returns>True si des annonceurs ont �t� enregistr�es, false sinon</returns>
        public bool isReferenceAdvertisersSelected()
        {
            #region Ancienne version
            //if (this.ReferenceUniversAdvertiser.Nodes.Count > 0) return (true);
            //else return (false);
            #endregion

            if (SecondaryProductUniverses.Count > 0) return (true);
            else return (false);
        }

        /// <summary>
        /// Indique si des annonceurs ont �t� s�lectionn�s dans l'arbre CompetitorUniversAdvertiser
        /// </summary>
        /// <returns></returns>
        public bool isCompetitorAdvertiserSelected()
        {
            #region anciene version
            //if (this.CompetitorUniversAdvertiser.Count > 0) return (true);
            //else return (false);
            #endregion

            if (PrincipalProductUniverses.Count > 1) return (true);
            return (false);

        }

        /// <summary>
        /// Retourne le nombre d'univers annonceurs s�lectionn�
        /// </summary>
        /// <returns></returns>
        public int advertiserUniversNumber()
        {
            int advertiserNumber = 0;
            if (this.SecondaryProductUniverses.Count > 0)
            {
                advertiserNumber++;
            }
            advertiserNumber += this.PrincipalProductUniverses.Count;
            return (advertiserNumber);
        }
        #endregion

        #region Advertising Agency
        /// <summary>
        /// Return if an advertising agency univers is selected
        /// </summary>
        /// <returns>True if an advertising agency univers is selected</returns>
        public bool isAdvertisingAgencySelected()
        {
            if (this.PrincipalAdvertisingAgnecyUniverses.Count > 0) return (true);
            return (false);
        }
        #endregion

        #region Media

        /// <summary>
        /// Indique si un vehicle a �t� s�lectionn�s
        /// </summary>
        /// <returns>True si un vehicle �t� enregistr�es, false sinon</returns>
        public bool isVehicleSelected()
        {
            if (this.GetSelection(this.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess).Length > 0) return (true);
            return (false);
        }

        /// <summary>
        /// Indique si des m�dias ont �t� s�lectionn�s
        /// </summary>
        /// <returns>True si des m�dias ont �t� enregistr�es, false sinon</returns>
        public bool isMediaSelected()
        {
            if (this.selectionUniversMedia.Nodes.Count > 0) return (true);
            else return (false);
        }

        /// <summary>
        /// Indique si des m�dias ont �t� s�lection�s dans comp�titorUniversMedia
        /// </summary>
        /// <returns></returns>	
        public bool isCompetitorMediaSelected()
        {
            return (_principalMediaUniverses != null && _principalMediaUniverses.Count > 1);
        }

        /// <summary>
        /// Indique si des m�dias dans ref�r�nceMedia ont �t� s�lectionn�s
        /// </summary>
        /// <returns>True si des m�dias ont �t� enregistr�es, false sinon</returns>
        public bool isReferenceMediaSelected()
        {
            //if (this.referenceUniversMedia.Nodes.Count > 0) return (true);
            return (_principalMediaUniverses !=null &&  _principalMediaUniverses.Count>0);         
        }


        /// <summary>
        /// Retourne le nombre d'univers media s�lectionn�
        /// </summary>
        /// <returns></returns>
        public int mediaUniversNumber()
        {
            //OLD
            //int mediaNumber = 0;

            //if (this.ReferenceUniversMedia.Nodes.Count > 0)
            //{
            //    mediaNumber++;
            //}
            //mediaNumber += this.CompetitorUniversMedia.Count;

            return _principalMediaUniverses.Count;

        }

        /// <summary>
        /// Indique si des supports ont �t� s�lectionn�s
        /// </summary>
        /// <returns>True si des supports ont �t� enregistr�es, false sinon</returns>
        public bool IsPrincipalMediaUniversesSelected()
        {
            return (_principalMediaUniverses != null && _principalMediaUniverses.Count > 0);
        }
        #endregion

        #region Vagues
        /// <summary>
        /// Indique si des vagues ont �t� s�lectionn�s
        /// </summary>
        /// <returns>True si des vagues ont �t� enregistr�es, false sinon</returns>
        public bool IsWaveSelected()
        {
            if (this.SelectionUniversAEPMWave.Nodes.Count > 0) return (true);
            else return (false);
        }

        #endregion

        #region Cibles
        /// <summary>
        /// Indique si des cibles ont �t� s�lectionn�es
        /// </summary>
        /// <returns>True si des cibles ont �t� enregistr�es, false sinon</returns>
        public bool IsTargetSelected()
        {
            if (this.SelectionUniversAEPMTarget.Nodes.Count > 0) return (true);
            else return (false);
        }
        #endregion

        #region Genre d'�mission
        /// <summary>
        /// Indique si des genres d'�missions ont �t� s�lectionn�s
        /// </summary>
        /// <returns>True si des genres d'�missions ont �t� enregistr�s, false sinon</returns>
        public bool IsCurrentUniversProgramTypeSelected()
        {
            if (this.CurrentUniversProgramType.Nodes.Count > 0) return (true);
            else return (false);
        }
        #endregion

        #region Forme de parrainage
        /// <summary>
        /// Indique si des formes de parrainage ont �t� s�lectionn�es
        /// </summary>
        /// <returns>True si des formes de parrainage ont �t� enregistr�es, false sinon</returns>
        public bool IsCurrentUniversSponsorshipFormSelected()
        {
            if (this.CurrentUniversSponsorshipForm.Nodes.Count > 0) return (true);
            else return (false);
        }
        #endregion

        #region Produit
        /// <summary>
        /// Indique si des produits ont �t� s�lectionn�s
        /// </summary>
        /// <returns>True si des produits ont �t� enregistr�es, false sinon</returns>
        public bool isSelectionProductSelected()
        {
            if (this.PrincipalProductUniverses.Count > 0) return (true);
            else return (false);
        }

        /// <summary>
        /// Indique si des produits ont �t� s�lectionn�s
        /// </summary>
        /// <returns>True si des produits ont �t� enregistr�es, false sinon</returns>
        public bool IsCurrentUniversProductSelected()
        {
            if (this.PrincipalProductUniverses.Count > 0) return (true);
            else return (false);
        }

        /// <summary>
        /// Retourne le nombre d'univers produit s�lectionn�
        /// </summary>
        /// <returns></returns>
        public int productUniversNumber()
        {
            int productNumber = 0;

            if (this.ReferenceUniversProduct.Nodes.Count > 0)
            {
                productNumber++;
            }
            productNumber += this.CompetitorUniversProduct.Count;

            return (productNumber);

        }
        /// <summary>
        /// Check if Advertisement Type Universe is Selected
        /// </summary>
        /// <returns></returns>
        public bool IsAdvertisementTypeUniverseSelected()
        {
            return (AdvertisementTypeUniverses != null && AdvertisementTypeUniverses.Count > 0);
        }
        #endregion

        #region Generic Checking
        /// <summary>
        /// Check if a univers is selected via a call to a specified method
        /// </summary>
        /// <param name="method">Method to call</param>
        /// <returns>The result of the called method or false in any case of error</returns>
        public bool CheckUnivers(string method)
        {
            try
            {
                return ((bool)this.GetType().InvokeMember(method, System.Reflection.BindingFlags.InvokeMethod, null, this, null));
            }
            catch (System.Exception exc)
            {
                return false;
            }
        }
        #endregion

        #endregion

        #region Tracking

        /// <summary>
        /// Ins�re l'�v�nement nouvelle connexion dans le tracking system
        /// </summary>
        public void OnNewConnection(string IP)
        {
            try
            {
                DATracking.NewConnection(Source, Int64.Parse(idSession), CustomerLogin.IdLogin, IP);
            }
            catch (System.Exception) { }
        }

        /// <summary>
        /// Ins�re l'�v�nement nouveau module s�lectionn� dans le tracking system
        /// </summary>
        private void OnSetModule()
        {
            try
            {

                DATracking.SetModule(Source, Int64.Parse(idSession), CustomerLogin.IdLogin, currentModule);
            }
            catch (System.Exception) { }
        }

        /// <summary>
        /// Ins�re l'�v�nement nouveau media s�lectionn� dans le tracking system
        /// </summary>
        /// <param name="vehicleId">Identifiant du media (Vehicle)</param>
        public void OnSetVehicle(Int64 vehicleId)
        {
            try
            {
                DATracking.SetVehicle(Source, Int64.Parse(IdSession), CustomerLogin.IdLogin, CurrentModule, vehicleId);
            }
            catch (System.Exception) { }
        }

        /// <summary>
        /// Ins�re l'�v�nement Gad utilis� dans le tracking system
        /// </summary>
        public void OnUseGad()
        {
            try
            {
                Module moduleSelected = customerLogin.GetModule(currentModule);
                Int64 resultId = moduleSelected.GetResultId(int.Parse(currentTab.ToString()));
                DATracking.UseGad(Source, Int64.Parse(idSession), CustomerLogin.IdLogin, currentModule, resultId);
            }
            catch (System.Exception) { }

        }

        /// <summary>
        /// Ins�re l'�v�nement agence media s�lectionn�e dans le tracking system
        /// </summary>
        private void OnSetMediaAgencyInGenericDetailLevel(GenericDetailLevel currentDetailLevel)
        {
            if (currentDetailLevel.ContainLevelToTrack(DetailLevelItemInformation.Levels.groupMediaAgency) || currentDetailLevel.ContainLevelToTrack(DetailLevelItemInformation.Levels.agency))
            {
                try
                {
                    Module moduleSelected = customerLogin.GetModule(currentModule);
                    Int64 resultId = moduleSelected.GetResultId(int.Parse(currentTab.ToString()));
                    //customerLogin.HtModulesList.Clear();
                    DATracking.SetMediaAgency(Source, Int64.Parse(idSession), CustomerLogin.IdLogin, currentModule, resultId);
                }
                catch (System.Exception) { }
            }
        }

        /// <summary>
        /// Ins�re l'�v�nement agence media s�lectionn�e dans le tracking system
        /// </summary>
        private void OnSetMediaAgency()
        {
            switch (preformatedProductDetail)
            {
                case TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
                    try
                    {
                        //customerLogin.ModuleList();
                        Module moduleSelected = customerLogin.GetModule(currentModule);
                        Int64 resultId = moduleSelected.GetResultId(int.Parse(currentTab.ToString()));
                        //customerLogin.HtModulesList.Clear();
                        DATracking.SetMediaAgency(Source, Int64.Parse(idSession), CustomerLogin.IdLogin, currentModule, resultId);
                    }
                    catch (System.Exception) { }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Ins�re l'�v�nement type de p�riode s�lectionn� dans le tracking system
        /// </summary>
        private void OnSetPeriodType()
        {
            try
            {
                DATracking.SetPeriodType(Source, Int64.Parse(idSession), CustomerLogin.IdLogin, currentModule, (int)periodType);
            }
            catch (System.Exception) { }

        }

        /// <summary>
        /// Ins�re l'�v�nement type d'unit� s�lectionn� dans le tracking system
        /// </summary>
        private void OnSetUnit()
        {
            try
            {
                DATracking.SetUnit(Source, Int64.Parse(idSession), CustomerLogin.IdLogin, currentModule, (int)unit);
            }
            catch (System.Exception) { }
        }

        /// <summary>
        /// Ins�re l'�v�nement nouveau r�sultat s�lectionn�e dans le tracking system
        /// </summary>
        public void OnSetResult()
        {
            try
            {
                //customerLogin.ModuleList();
                Module moduleSelected = customerLogin.GetModule(currentModule);
                Int64 resultId = moduleSelected.GetResultId(int.Parse(currentTab.ToString()));
                //customerLogin.HtModulesList.Clear();

                DATracking.SetResult(Source, Int64.Parse(idSession), CustomerLogin.IdLogin, currentModule, resultId);
            }
            catch (System.Exception) { }

        }

        /// <summary>
        /// Ins�re l'�v�nement utilisation d'un export dans le tracking system
        /// </summary>
        public void OnUseFileExport()
        {
            try
            {
                //customerLogin.ModuleList();
                Module moduleSelected = customerLogin.GetModule(currentModule);
                Int64 resultId = moduleSelected.GetResultId(int.Parse(currentTab.ToString()));
                DATracking.UseFileExport(Source, Int64.Parse(idSession), CustomerLogin.IdLogin, currentModule, resultId);
            }
            catch (System.Exception) { }
        }

        /// <summary>
        /// Ins�re l'�v�nement sauvegarde d'un r�sultat dans le tracking system
        /// </summary>
        public void OnUseMyAdExpressSave()
        {
            try
            {
                //customerLogin.ModuleList();
                Module moduleSelected = customerLogin.GetModule(currentModule);
                Int64 resultId = moduleSelected.GetResultId(int.Parse(currentTab.ToString()));
                //customerLogin.HtModulesList.Clear();

                DATracking.UseMyAdExpressSave(Source, Int64.Parse(idSession), CustomerLogin.IdLogin, currentModule, resultId);
            }
            catch (System.Exception) { }

        }

        #endregion

        #region M�thodes externes

        #region Unit�s
        /// <summary>
        /// Obtient l'identifiant du texte correspondant � l'unit� s�lectionn� par la client
        /// </summary>
        /// <returns>Identifiant du texte</returns>
        public Int64 GetUnitLabelId()
        {
            try
            {
                return this.GetSelectedUnit().WebTextId;
            }
            catch
            {
                throw (new UnitException("Unit selection is not managed"));
            }
        }

        /// <summary>
        /// CellUnitfactory according to the unit chosen by customer acquires
        /// </summary>
        /// <returns></returns>
        public CellUnitFactory GetCellUnitFactory()
        {
            try
            {
                //System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(@"TNS.FrameWork.WebResultUI");
                UnitInformation selectedUnit = GetSelectedUnit();
                System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(selectedUnit.Assembly);
                Type type = assembly.GetType(selectedUnit.CellType);
                Cell cellUnit = (Cell)type.InvokeMember("GetInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, null, null);
                cellUnit.StringFormat = selectedUnit.StringFormat;
                return (new CellUnitFactory((CellUnit)cellUnit));
            }
            catch
            {
                throw (new UnitException("Unit selection is not managed"));
            }
        }

        /// <summary>
        /// Get Valid Unit List for current result
        /// </summary>
        /// <returns>Valid Unit List for current result</returns>
        public List<UnitInformation> GetValidUnitForResult()
        {
            try
            {
                Module moduleDescription = ModulesList.GetModule(currentModule);
                ResultPageInformation resultPageInformation = (ResultPageInformation)moduleDescription.GetResultPageInformation((int)currentTab);
                string listStr = GetSelection(SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
                Dictionary<Int64, VehicleInformation> vehcileInformationList = GetVehiclesSelected();
                List<Vehicles.names> vehicleList = new List<Vehicles.names>();
                foreach (var cVehicleInformation in vehcileInformationList.Values)
                {
                    vehicleList.Add(cVehicleInformation.Id);
                }

                List<UnitInformation> units = resultPageInformation.GetValidUnits(vehicleList);
                for (int i = units.Count - 1; i >= 0; i--)
                {
                    switch (units[i].Id)
                    {
                        case TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.volume:
                            if (!this.CustomerLogin.CustormerFlagAccess(Flags.ID_VOLUME_MARKETING_DIRECT)) {
                                units.RemoveAt(i);
                            }
                            break;
                        case WebConstantes.CustomerSessions.Unit.volumeMms:
                            if (!this.CustomerLogin.CustormerFlagAccess(Flags.ID_VOLUME_DISPLAY)) {
                                units.RemoveAt(i);
                            }
                            break;
                        case WebConstantes.CustomerSessions.Unit.insertion:
                            if (vehicleList.Contains(Vehicles.names.mms) && !this.CustomerLogin.CustormerFlagAccess(Flags.ID_VOLUME_DISPLAY))
                                units.RemoveAt(i);
                            break;
                        default:
                            break;
                    }
                }

                return units;
            }
            catch
            {
                throw (new UnitException("Unit selection is not managed"));
            }
        }

        /// <summary>
        /// Get selected unit information
        /// </summary>
        /// <returns>Selected unit information</returns>
        public Domain.Units.UnitInformation GetSelectedUnit()
        {
            try
            {
                return UnitsInformation.Get(this.unit);
            }
            catch
            {
                throw (new UnitException("Unit selection is not managed"));
            }
        }

        /// <summary>
        /// Get Valid Campaign type List for current result
        /// </summary>
        /// <returns>Valid Campaign type List for current result</returns>
        public List<Domain.CampaignTypes.CampaignTypeInformation> GetValidCampaignTypeForResult()
        {
            try
            {
                Module moduleDescription = ModulesList.GetModule(currentModule);
                ResultPageInformation resultPageInformation = (ResultPageInformation)moduleDescription.GetResultPageInformation((int)currentTab);

                return resultPageInformation.GetValidCampaignTypes();
            }
            catch
            {
                throw (new Exception("Campaign Type selection is not managed"));
            }
        }

        /// <summary>
        /// Get Default Campaign type for current result
        /// </summary>
        /// <returns>Default Campaign type for current result</returns>
        public TNS.AdExpress.Constantes.Web.CustomerSessions.CampaignType GetDefaultCampaignType()
        {
            try
            {
                Module moduleDescription = ModulesList.GetModule(currentModule);
                ResultPageInformation resultPageInformation = (ResultPageInformation)moduleDescription.GetResultPageInformation((int)currentTab);

                return resultPageInformation.DefaultCampaignType;
            }
            catch
            {
                throw (new Exception("Campaign Type default selection is not managed"));
            }
        }

        #endregion

        #region Get Vehicles Selected
        /// <summary>
        /// Get Vehicles Selected
        /// </summary>
        /// <returns>Vehicles Selected</returns>
        public Dictionary<Int64, VehicleInformation> GetVehiclesSelected()
        {
            try
            {
                string listStr = GetSelection(SelectionUniversMedia, Constantes.Customer.Right.type.vehicleAccess);
                var vehicleList = new Dictionary<Int64, VehicleInformation>();
                if (!string.IsNullOrEmpty(listStr))
                {
                    string[] list = listStr.Split(',');
                    for (int i = 0; i < list.Length; i++)
                        vehicleList.Add(Convert.ToInt64(list[i]), VehiclesInformation.Get(Convert.ToInt64(list[i])));
                }
                else if (SelectionUniversMedia.FirstNode != null && SelectionUniversMedia.FirstNode.Tag != null)
                {

                    //When a vehicle is not checked but one or more category, this get the vehicle correspondly
                    vehicleList.Add(((LevelInformation)SelectionUniversMedia.FirstNode.Tag).ID, VehiclesInformation.Get(((LevelInformation)SelectionUniversMedia.FirstNode.Tag).ID));
                }


                return vehicleList;
            }
            catch
            {
                throw (new VehicleException("Vehicle selection is not managed"));
            }
        }
        #endregion

        #region Get Format
        /// <summary>
        /// Get Common Valid Format List
        /// </summary>
        /// <param name="vehicleInformationList">Vehicle Information List</param>
        /// <returns>Common Valid Format List</returns>
        public Dictionary<Int64, FilterItem> GetValidFormatList(Dictionary<Int64, VehicleInformation> vehicleInformationList)
        {
            try
            {
                var activeBannersFormatList = new Dictionary<Int64, FilterItem>();
                if (WebApplicationParameters.VehiclesFormatInformation.Use)
                {
                    List<long> formatIdList = null;
                    
                    if(vehicleInformationList.ContainsKey(Vehicles.names.adnettrack.GetHashCode()))
                        formatIdList = CustomerLogin.GetBannersFormatAssignement(WebApplicationParameters.VehiclesFormatInformation.GetRightBannersTypeList(vehicleInformationList));
                    
                    List<Int64> vehicleIdList = new List<VehicleInformation>(vehicleInformationList.Values).ConvertAll(p => p.DatabaseId);
                    if (formatIdList == null || formatIdList.Count == 0)
                    {
                        activeBannersFormatList = VehiclesFormatList.GetList(vehicleIdList);
                    }
                    else
                    {
                        foreach (var cFilterItem in VehiclesFormatList.GetList(vehicleIdList).Values)
                        {
                            activeBannersFormatList.Add(
                                cFilterItem.Id
                                , new FilterItem(
                                    cFilterItem.Id
                                    , cFilterItem.Label
                                    , formatIdList.Contains(cFilterItem.Id)
                                    )
                                );
                        }
                    }
                }
                return activeBannersFormatList;
            }
            catch (Exception e)
            {
                throw (new BannersFormatException("Valid Format List is not managed", e));
            }
        }
        /// <summary>
        /// Get Valid Format List
        /// </summary>
        /// <param name="vehicleInformationList">Vehicle Information List</param>
        /// <returns>Valid Format List</returns>
        public List<Int64> GetValidFormatSelectedList(List<VehicleInformation> vehicleInformationList)
        {
            var vehicleInfoList = new Dictionary<Int64, VehicleInformation>();
            foreach (var cVehicleInformation in vehicleInformationList)
            {
                if (!vehicleInfoList.ContainsKey(cVehicleInformation.DatabaseId))
                    vehicleInfoList.Add(cVehicleInformation.DatabaseId, cVehicleInformation);
            }
            return GetValidFormatSelectedList(vehicleInfoList);
        }

        /// <summary>
        /// Get Valid Format List
        /// </summary>
        /// <param name="vehicleInformationList">Vehicle Information List</param>
        /// <returns>Valid Format List</returns>
        public List<Int64> GetValidFormatSelectedList(Dictionary<Int64, VehicleInformation> vehicleInformationList)
        {
            return GetValidFormatSelectedList(vehicleInformationList, false);
        }

        /// <summary>
        /// Get Valid Format List
        /// </summary>
        /// <param name="vehicleInformationList">Vehicle Information List</param>
        /// <param name="returnAlwaysValues">
        /// If value is true : return empty list only if no vehicle is defined in selection 
        /// If value is false : return empty list when if no vehicle is defined in selection OR if the table banners contains all format selected
        /// </param>
        /// <returns>Valid Format List</returns>
        public List<Int64> GetValidFormatSelectedList(Dictionary<Int64, VehicleInformation> vehicleInformationList, bool returnAlwaysValues)
        {
            try
            {
                var activeBannersFormatList = new List<Int64>();
                if (WebApplicationParameters.VehiclesFormatInformation.Use && vehicleInformationList != null && vehicleInformationList.Count > 0)
                {
                    var validFormatItemList = new List<FilterItem>(GetValidFormatList(vehicleInformationList).Values);
                    var validFormatList = validFormatItemList.FindAll(p => p.IsEnable).ConvertAll(p => p.Id);

                    if (!string.IsNullOrEmpty(SelectedBannersFormatList))
                    {
                        var selectedBannersFormatList = new List<string>(SelectedBannersFormatList.Split(',')).ConvertAll<Int64>(Int64.Parse);
                        activeBannersFormatList = selectedBannersFormatList.FindAll(validFormatList.Contains);
                    }
                    else if (returnAlwaysValues || validFormatItemList.Count != validFormatList.Count)
                    {
                        activeBannersFormatList = validFormatList;
                    }
                    else
                    {
                        activeBannersFormatList = new List<Int64>();
                    }
                }
                return activeBannersFormatList;
            }
            catch
            {
                throw (new BannersFormatException("Valid Format Selected List is not managed"));
            }
        }
        #endregion

        #region Universes
        /// <summary>
        /// Get universe level list Id
        /// </summary>
        /// <param name="universes">Universes</param>
        /// <remarks>Each Id is unique in the list</remarks>		
        /// <returns>level List Id</returns>
        public static List<Int64> GetLevelListId(Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universes)
        {
            List<long> levelsIds = new List<long>();

            for (int i = 0; i < universes.Count; i++)
            {
                levelsIds = universes[i].GetLevelListId();
                for (int j = 0; j < levelsIds.Count; j++)
                {
                    if (!levelsIds.Contains(levelsIds[j]))
                    {
                        levelsIds.Add(levelsIds[j]);
                    }
                }
            }
            return levelsIds;
        }
        #endregion

        #region Dates

        public CustomerPeriod UpdateDates(DateTime FirstDayNotEnable, DateTime dateRef)
        {
            bool isLastCompletePeriod = false;
            DateTime lastDayEnable = dateRef;
            DateTime tmp = dateRef;

            // In the case of a LostWin study, if the period disponibility
            // selected is a complete period, we change the flag and the
            // lastDayEnable variable
            if (this.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE &&
                this.CustomerPeriodSelected.PeriodDisponibilityType == WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod)
            {
                lastDayEnable = FirstDayNotEnable.AddDays(-1);
                isLastCompletePeriod = true;
            }

            // Updating dates depending on the period type.
            // Only updating if the period type is a sliding
            // one
            switch (this.periodType)
            {
                #region nLastYears
                case CustPeriodType.nLastYear:
                    this.PeriodBeginningDate = dateRef.AddYears(1 - this.PeriodLength).ToString("yyyy0101");
                    this.PeriodEndDate = dateRef.ToString("yyyyMMdd");
                    break;
                #endregion

                #region nLastMonths
                case CustPeriodType.nLastMonth:
                    // Setting default values
                    this.PeriodBeginningDate = lastDayEnable.AddMonths(1 - this.PeriodLength).ToString("yyyyMM01");
                    this.PeriodEndDate = lastDayEnable.ToString("yyyyMMdd");

                    // In case we need a complete period, we check if the
                    // month corresponding to the lastDayEnable is the end
                    // of the month
                    DateTime lastDayOfMonth = new DateTime(lastDayEnable.Year, lastDayEnable.Month, 1).AddMonths(1).AddDays(-1);

                    // If the lastDayEnable is not the end of its month
                    // we have to set the period to a month earlier
                    if (isLastCompletePeriod && lastDayEnable != lastDayOfMonth)
                    {
                        this.PeriodBeginningDate = lastDayEnable.AddMonths(0 - this.PeriodLength).ToString("yyyyMM01");
                        this.PeriodEndDate = lastDayEnable.AddDays(-(lastDayEnable.Day + 1)).ToString("yyyyMMdd");
                    }
                    break;
                #endregion

                #region nLastWeeks
                case CustPeriodType.nLastWeek:
                    // Setting local variables
                    DateTime PeriodEnd = lastDayEnable;
                    DateTime lastDayOfWeek = lastDayEnable.AddDays(7 - lastDayEnable.DayOfWeek.GetHashCode());

                    // Checking if we need the last complete period
                    // and if so, if the lastDayEnable is the last day
                    // of its week. Otherwise, setting PeriodEnd to the
                    // previous week
                    if (isLastCompletePeriod && lastDayOfWeek != lastDayEnable)
                        PeriodEnd = lastDayEnable.AddDays(-lastDayEnable.DayOfWeek.GetHashCode());

                    // Setting beginning and end dates
                    this.PeriodBeginningDate = PeriodEnd.AddDays(-(this.PeriodLength * 7 - 1)).ToString("yyyyMMdd");
                    this.PeriodEndDate = PeriodEnd.ToString("yyyyMMdd");
                    break;
                #endregion

                #region nLastDays
                case CustPeriodType.nLastDays:
                    this.PeriodBeginningDate = lastDayEnable.AddDays(1 - this.PeriodLength).ToString("yyyyMMdd");
                    this.PeriodEndDate = lastDayEnable.ToString("yyyyMMdd");
                    break;
                #endregion

                #region previousYear
                case CustPeriodType.previousYear:
                    this.PeriodBeginningDate = dateRef.AddYears(-1).ToString("yyyy0101");
                    this.PeriodEndDate = dateRef.AddYears(-1).ToString("yyyy1231");
                    break;
                #endregion

                #region previousMonth

                case CustPeriodType.previousMonth:
                    tmp = tmp.AddDays(-tmp.Day);
                    this.PeriodEndDate = tmp.ToString("yyyyMMdd");
                    this.PeriodBeginningDate = tmp.ToString("yyyyMM01");
                    break;

                #endregion

                #region previousWeek

                case CustPeriodType.previousWeek:
                    tmp = tmp.AddDays(-tmp.DayOfWeek.GetHashCode());
                    this.PeriodEndDate = tmp.ToString("yyyyMMdd");
                    this.PeriodBeginningDate = tmp.AddDays(-6).ToString("yyyyMMdd");
                    break;

                #endregion

                #region previousDay

                case CustPeriodType.previousDay:
                    this.PeriodBeginningDate = tmp.AddDays(-1).ToString("yyyyMMdd");
                    this.PeriodEndDate = this.PeriodBeginningDate;
                    break;

                #endregion

                #region currentYear

                case CustPeriodType.currentYear:
                    this.PeriodBeginningDate = tmp.ToString("yyyy0101");
                    this.PeriodEndDate = tmp.ToString("yyyyMMdd");
                    break;

                #endregion
            }

            // Formatting dates depending on the module.
            // Some only need a yyyyMM format
            switch (this.CurrentModule)
            {
                case TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR:
                case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE:
                case TNS.AdExpress.Constantes.Web.Module.Name.TENDACES:
                    this.PeriodBeginningDate = this.PeriodBeginningDate.Substring(0, 6);
                    this.PeriodEndDate = this.PeriodEndDate.Substring(0, 6);
                    break;

            }

            return (null);

        }
        #endregion

        #region SetVpDates
        /// <summary>
        /// Set Vp renault Date
        /// </summary>
        /// <param name="periodType">Period Type</param>
        public void SetDates(TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType)
        {
            DateTime cDateTime = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
            PeriodType = periodType;
            switch (PeriodType)
            {
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.allHistoric:
                    PeriodBeginningDate = cDateTime.ToString("yyyyMMdd");
                    PeriodEndDate = (new DateTime(cDateTime.Year, cDateTime.Month, cDateTime.AddMonths(1).AddDays(-1).Day)).ToString("yyyyMMdd");
                    break;
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.cumulWithNextMonth:
                    PeriodBeginningDate = (new DateTime(DateTime.Now.Year, 1, 1)).ToString("yyyyMMdd");
                    PeriodEndDate = (new DateTime(cDateTime.Year, cDateTime.AddMonths(1).Month, cDateTime.AddMonths(2).AddDays(-1).Day)).ToString("yyyyMMdd");
                    break;
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.currentMonth:
                    PeriodBeginningDate = cDateTime.ToString("yyyyMMdd");
                    PeriodEndDate = (new DateTime(cDateTime.Year, cDateTime.Month, cDateTime.AddMonths(1).AddDays(-1).Day)).ToString("yyyyMMdd");
                    break;
                default:
                    throw new NotImplementedException("The period type '" + periodType.ToString() + "' is not implemented");
            }

        }
        /// <summary>
        /// Set Vp renault Date
        /// </summary>
        /// <param name="dateBegin">Period Type</param>
        /// <param name="periodEnd">periodEnd</param>
        public void SetDates(DateTime dateBegin, DateTime periodEnd)
        {
            PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.personalize;
            PeriodBeginningDate = dateBegin.ToString("yyyyMMdd");
            PeriodEndDate = periodEnd.ToString("yyyyMMdd");
        }
        #endregion

        #endregion

        #region ToBinaryData
        public byte[] ToBinaryData()
        {
            MemoryStream ms = null;
            BinaryFormatter bf = null;
            byte[] binaryData = null;

            ms = new MemoryStream();
            bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            binaryData = new byte[ms.GetBuffer().Length];
            binaryData = ms.GetBuffer();
            if (ms != null) ms.Close();
            if (bf != null) bf = null;
            return (binaryData);
        }
        #endregion




    }

}
