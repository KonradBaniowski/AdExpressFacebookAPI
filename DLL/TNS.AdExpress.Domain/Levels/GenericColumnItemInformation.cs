#region Informations
// Auteur: D. Mussuma, Y. Rkaina
// Création: 24/04/2006
// Modification:
#endregion

using System;
using System.Collections;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;

namespace TNS.AdExpress.Domain.Level
{
    /// <summary>
    /// Description résumée de GenericColumn.
    /// </summary>
    public class GenericColumnItemInformation
    {

        #region Enum

        /// <summary>
        /// Eléments compsant les Colonnes
        /// </summary>
        public enum Columns
        {
            /// <summary>
            /// Media
            /// </summary>
            vehicle = 1,

            /// <summary>
            /// Catégorie
            /// </summary>
            category = 2,

            /// <summary>
            /// Support
            /// </summary>
            media = 3,

            /// <summary>
            /// Centre d'interet
            /// </summary>
            interestCenter = 4,

            /// <summary>
            /// Régie
            /// </summary>
            mediaSeller = 5,

            /// <summary>
            /// Version
            /// </summary>
            slogan = 6,

            /// <summary>
            /// Annonceur
            /// </summary>
            advertiser = 7,

            /// <summary>
            /// Groupe
            /// </summary>
            group = 8,

            /// <summary>
            /// Produit
            /// </summary>
            product = 9,

            /// <summary>
            /// Spot
            /// </summary>
            associatedFile = 10,

            /// <summary>
            /// Format
            /// </summary>
            format = 11,

            /// <summary>
            /// Surface
            /// </summary>
            areaPage = 12,

            /// <summary>
            /// Couleur
            /// </summary>
            color = 13,

            /// <summary>
            /// Prix
            /// </summary>
            expenditureEuro = 14,

            /// <summary>
            /// Descriptif
            /// </summary>
            location = 15,

            /// <summary>
            /// Top de diffusion radio
            /// </summary>
            idTopDiffusion = 16,

            /// <summary>
            /// Top de diffusion télévision
            /// </summary>
            topDiffusion = 17,

            /// <summary>
            /// Durée
            /// </summary>
            duration = 18,

            /// <summary>
            /// Position radio
            /// </summary>
            rank = 19,

            /// <summary> 
            /// Position télévision
            /// </summary>
            idRank = 20,

            /// <summary> 
            /// Durée écran
            /// </summary>
            durationCommercialBreak = 21,

            /// <summary> 
            /// Nombre Spots écran télévision
            /// </summary>
            numberMessageCommercialBrea = 22,

            /// <summary> 
            /// Nombre Spots écran radio
            /// </summary>
            numberSpotComBreak = 23,

            /// <summary> 
            /// Position hap
            /// </summary>
            rankWap = 24,

            /// <summary> 
            /// Durée écran hap
            /// </summary>
            durationComBreakWap = 25,

            /// <summary> 
            /// Nombre spots hap
            /// </summary>
            numberSpotComBreakWap = 26,

            /// <summary> 
            /// Code écran
            /// </summary>
            idCommercialBreak = 27,

            /// <summary> 
            /// Nombre de panneaux
            /// </summary>
            numberBoard = 28,

            /// <summary> 
            /// Format du panneau
            /// </summary>
            typeBoard = 29,

            /// <summary> 
            /// Type de réseau
            /// </summary>
            typeSale = 30,

            /// <summary> 
            ///Outdoor Network 
            /// </summary>
            posterNetwork = 31,

            /// <summary> 
            /// Agglomeration
            /// </summary>
            agglomeration = 32,

            /// <summary> 
            /// Date
            /// </summary>
            dateMediaNum = 33,

            /// <summary> 
            /// Page
            /// </summary>
            mediaPaging = 34,

            /// <summary> 
            /// Visuel
            /// </summary>
            visual = 35,

            /// <summary>
            /// Position Presse
            /// </summary>
            rankMedia = 36,

            /// <summary>
            /// Genre d'émissions
            /// </summary>
            programType = 37,

            /// <summary>
            /// Emission
            /// </summary>
            program = 38,

            /// <summary>
            /// Forme de parrainage
            /// </summary>
            formSponsorship = 39,

            /// <summary>
            /// Jour de la semaine
            /// </summary>
            dayOfWeek = 40,

            /// <summary>
            /// Affiche
            /// </summary>
            poster = 41,

            /// <summary>
            /// Date de Parution
            /// </summary>
            dateParution = 42,

            /// <summary>
            /// Agence Media
            /// </summary>
            agenceMedia = 43,

            /// <summary>
            /// Famille
            /// </summary>
            famille = 44,

            /// <summary>
            /// Surface en mmc
            /// </summary>
            surfaceMMC = 45,

            /// <summary>
            /// Rang famille
            /// </summary>
            rangFamille = 46,

            /// <summary>
            /// Rang groupe
            /// </summary>
            rangGroupe = 47,

            /// <summary>
            /// Rang support
            /// </summary>
            rangSupport = 48,

            /// <summary>
            /// Date de diffusion
            /// </summary>
            dateDiffusion = 49,

            /// <summary>
            /// Code écran
            /// </summary>
            codeEcran = 50,

            /// <summary>
            /// Prix du 30 sec
            /// </summary>
            prix30Sec = 51,

            /// <summary>
            /// Type page
            /// </summary>
            typePage = 52,

            /// <summary>
            /// Plan Media
            /// </summary>
            planMedia = 53,

            /// <summary>
            /// Id pub presse
            /// </summary>
            idPressAdvertisment = 54,

            /// <summary>
            /// Id Cobranding
            /// </summary>
            idCobranding = 55,

            /// <summary>
            /// Id Outdoor
            /// </summary>            
            idDataOutDoor = 56,

            /// <summary>
            /// Cover date
            /// </summary>
            dateCoverNum = 57,

            /// <summary>
            /// ID VMC data
            /// </summary>
            idVMC = 58,

            /// <summary>
            /// Mailing rapidity
            /// </summary>
            rapidity = 59,

            /// <summary>
            /// Type of document
            /// </summary>
            typeDoc = 60,

            /// <summary>
            /// Volume
            /// </summary>
            volume = 61,

            /// <summary>
            /// Weight
            /// </summary>
            weight = 62,

            /// <summary>
            /// Item number
            /// </summary>
            itemNb = 63,

            /// <summary>
            /// Mail format (standard or specific)
            /// </summary>
            mailFormat = 64,

            /// <summary>
            /// Mail Content
            /// </summary>
            content = 65,

            /// <summary>
            /// Type of mail (blister...)
            /// </summary>
            mailType = 66,

            /// <summary>
            /// Expenditure as a sum
            /// </summary>
            sumExpenditure = 67,

            /// <summary>
            /// Somme Surface page
            /// </summary>
            sumSurface = 68,

            /// <summary>
            /// Somme Durée
            /// </summary>
            sumDuration = 69,

            /// <summary>
            /// Nombre de supports
            /// </summary>
            countMedia = 70,

            /// <summary>
            /// Nombre de supports press
            /// </summary>
            countMediaPress = 71,

            /// <summary>
            /// Number of Insertions
            /// </summary>
            sumInsert = 72,

            /// <summary>
            /// Date kiosque (press)
            /// </summary>
            dateKiosque = 73,

            /// <summary>
            /// Nb spots
            /// </summary>
            sumSpot = 74,

            /// <summary>
            /// Max path (for unicity purpose)
            /// </summary>
            associatedFileMax = 75,

            /// <summary>
            /// Max path (for unicity purpose)
            /// </summary>
            numberBoardSum = 76,

            /// <summary>
            /// AdNetTrack product
            /// </summary>
            productAdNetTrack = 77,

            /// <summary>
            /// Banner Format
            /// </summary>
            bannerFormat = 78,

            /// <summary>
            /// Banner dimension
            /// </summary>
            bannerDimension = 79,

            /// <summary>
            /// Banner url
            /// </summary>
            bannerUrl = 80,

            /// <summary>
            /// Banner hashcode
            /// </summary>
            bannerHashcode = 81,

            /// <summary>
            /// Sub Sector
            /// </summary>
            subSector = 82,

            /// <summary>
            /// Banner Internet format
            /// </summary>
            bannerInternetFormat = 83,

            /// <summary>
            /// Banner Internet DImension
            /// </summary>
            bannerInternetDimension = 84,

            /// <summary>
            /// Euros
            /// </summary>
            euros = 85,

            /// <summary>
            /// First date parution
            /// </summary>
            firstDateParution = 86,

            /// <summary>
            /// Advertiser address id
            /// </summary>
            addressId = 87,

            /// <summary>
            /// Banner first parution
            /// </summary>
            bannerFirstParution = 88,

            /// <summary>
            /// Cetgory Identifiant
            /// </summary>
            idCategory = 89,

            /// <summary>
            /// Row number
            /// </summary>
            rowNum = 90,

            /// <summary>
            /// Banner Evaliant mobile DImension
            /// </summary>
            bannerEvaliantMobileDimension = 91,

            /// <summary>
            /// product evaliant mobile
            /// </summary>
            productEvaliantMobile = 92,

            /// <summary>
            /// Descriptif Magazine
            /// </summary>
            locationMagazine = 93,

            /// <summary>
            /// Descriptif NewsPaper
            /// </summary>
            locationNewsPaper = 94,

            /// <summary>
            /// region
            /// </summary>
            region = 95,

            /// <summary>
            /// National Channel
            /// </summary>
            nationalChannel = 96,

            /// <summary>
            /// TV Company
            /// </summary>
            tVCompany = 97,

            /// <summary>
            /// TV Channel
            /// </summary>
            tVChannel = 98,

            /// <summary>
            /// 
            /// </summary>
            timestart = 99,

            /// <summary>
            /// Sub brand
            /// </summary>
            subbrand = 100,

            /// <summary>
            /// Brand
            /// </summary>
            brand = 101,

            /// <summary>
            ///  Internet Square
            /// </summary>
            internetSquare = 102,

            /// <summary>
            /// Radio holding
            /// </summary>
            radioholding = 103,

            /// <summary>
            /// Category Level 4
            /// </summary>
            segment = 104,

            /// <summary>
            /// Advertisment
            /// </summary>
            advertisment = 105,

            /// <summary>
            /// advertisment Description
            /// </summary>
            advertismentDescription = 106,

            /// <summary>
            /// Advertisement Type
            /// </summary>
            advertisementType = 107,

            /// <summary>
            /// First issue date
            /// </summary>
            firstIssueDate = 108,

            /// <summary>
            ///Clip position 
            /// </summary>
            clipPosition = 109,

            /// <summary>
            /// Clips count
            /// </summary>
            clipsCount = 110,

            /// <summary>
            /// Break flight start
            /// </summary>
            breakFlightStart = 111,

            /// <summary>
            ///Break 
            /// </summary>
            break_ = 112,

            /// <summary>
            ///Break distribution 
            /// </summary>
            breakDistribution = 113,

            /// <summary>
            /// Programme flight start
            /// </summary>
            programmeFlightStart = 114,

            /// <summary>
            ///costUSD 
            /// </summary>
            costUSD = 115,

            /// <summary>
            /// CostRUB
            /// </summary>
            costRUB = 116,

            /// <summary>
            /// station
            /// </summary>
            station = 117,

            /// <summary>
            /// Allocation
            /// </summary>
            allocation = 118,

            /// <summary>
            /// Publishing house
            /// </summary>
            publishinghouse = 119,

            /// <summary>
            ///Edition 
            /// </summary>
            edition = 120,

            /// <summary>
            /// Edition type
            /// </summary>
            editionType = 121,

            /// <summary>
            /// St Format
            /// </summary>
            stFormat = 122,

            /// <summary>
            ///Position 
            /// </summary>
            position = 123,

            /// <summary>
            /// stDesign
            /// </summary>
            stDesign = 124,

            /// <summary>
            ///Ad PageNo 
            /// </summary>
            adPageNo = 125,

            /// <summary>
            /// Issue Local Number
            /// </summary>
            issueLocalNumber = 126,

            /// <summary>
            /// Volume Page
            /// </summary>
            volumePage = 127,

            /// <summary>
            /// Volume Page A2 
            /// </summary>
            volumePageA2 = 128,

            /// <summary>
            /// Carrier type 
            /// </summary>
            carriertype = 129,

            /// <summary>
            /// District
            /// </summary>
            District = 130,

            /// <summary>
            /// Address
            /// </summary>
            address = 131,

            /// <summary>
            /// Outdoor agency
            /// </summary>
            outdoorAgency = 132,

            /// <summary>
            /// Surface Height
            /// </summary>
            surfaceHeight = 133,

            /// <summary>
            /// SurfaceWeight
            /// </summary>
            SurfaceWeight = 134,

            /// <summary>
            /// Surface side
            /// </summary>
            surfaceSide = 135,

            /// <summary>
            /// Surface Count
            /// </summary>
            surfaceCount = 136,

            /// <summary>
            /// Volume M2
            /// </summary>
            volumeM2 = 137,

            /// <summary>
            /// Clip expected duration
            /// </summary>
            clipExpectedDuration = 138,

            /// <summary>
            /// Budget USD
            /// </summary>
            budgetUSD = 139,

            /// <summary>
            /// Budget RUB
            /// </summary>
            budgetRUB = 140,

            /// <summary>
            ///Press PagesA2 
            /// </summary>
            pressPagesA2 = 141,

            /// <summary>
            /// Outdoor Square
            /// </summary>
            outdoorSquare = 142,

            /// <summary>
            /// Price USD
            /// </summary>
            priceUSD = 143,

            /// <summary>
            /// Price RUB
            /// </summary>
            priceRUB = 144,

            /// <summary>
            /// Design
            /// </summary>
            design = 145,

            /// <summary>
            /// Geo
            /// </summary>
            geo = 146,

            /// <summary>
            /// Advertisment placement
            /// </summary>
            advertismentPlacement = 147,

            /// <summary>
            /// Holding
            /// </summary>
            holding = 148,

            /// <summary>
            /// Site
            /// </summary>
            site = 149,

            /// <summary>
            /// Site section
            /// </summary>
            siteSection = 150,

            /// <summary>
            /// Site subsection
            /// </summary>
            siteSubsection = 151,

            /// <summary>
            /// Landing page
            /// </summary>
            landingPage = 152,

            /// <summary>
            /// Advertisment file type
            /// </summary>
            advertismentFileType = 153,

            /// <summary>
            /// Advertisment display type
            /// </summary>
            advertismentDisplayType = 154,

            /// <summary>
            /// Advertisment position
            /// </summary>
            advertismentPosition = 155,

            /// <summary>
            /// Advertisment St format
            /// </summary>
            advertismentStFormat = 156,

            /// <summary>
            ///  Advertisment format
            /// </summary>
            advertismentFormat = 157,

            /// <summary>
            /// Age
            /// </summary>
            age = 158,

            /// <summary>
            /// Gender
            /// </summary>
            gender = 159,

            /// <summary>
            /// Distribution type 
            /// </summary>
            distributionType = 160,

            /// <summary>
            /// Column
            /// </summary>
            column = 161,

            /// <summary>
            ///  Type of publication
            /// </summary>
            publicationType = 162,

            /// <summary>
            /// Square (Editorial)
            /// </summary>
            square = 163,

            /// <summary>
            ///Mentions
            /// </summary>
            mentions = 164,

            /// <summary>
            ///Ad Break (For Russia diffrent than code ecran)
            /// </summary>
            adBreak = 165,

            /// <summary>
            ///Time slot
            /// </summary>
            timeSlot = 166,

            /// <summary>
            /// Banner Width
            /// </summary>
            bannerWidth = 167,

            /// <summary>
            /// Banner Height
            /// </summary>
            bannerHeight = 168,

            /// <summary>
            /// Cinema type advertisement (CZ)
            /// </summary>
            typeAdvertisement = 169,

            /// <summary>
            /// Profession
            /// </summary>
            profession = 170,

            /// <summary>
            /// Name
            /// </summary>
            name = 171,

            /// <summary>
            /// Rubric
            /// </summary>
            rubric = 172,

            /// <summary>
            /// Presence type
            /// </summary>
            presenceType = 173,

            /// <summary>
            /// Audience
            /// </summary>
            audience = 174,

            /// <summary>
            /// Country
            /// </summary>
            country = 175,

            /// <summary>
            /// Programme Genre
            /// </summary>
            programmeGenre = 176,

            /// <summary>
            /// Banner Id
            /// </summary>
            bannerId = 177,

            /// <summary>
            /// Media Group
            /// </summary>
            mediaGroup = 178,

            /// <summary>
            /// Date Media real
            /// </summary>
            dateMediaReal = 179,

            /// <summary>
            /// Holding Company
            /// </summary>
            holdingCompany = 180,

            /// <summary>
            /// Group Advertising Agency
            /// </summary>
            groupAdvertisingAgency = 181,

            /// <summary>
            /// Advertising Agency
            /// </summary>
            advertisingAgency = 182,

            /// <summary>
            /// Title
            /// </summary>
            title = 183,

            /// <summary>
            /// Basic Media
            /// </summary>
            basicMedia = 184,

            /// <summary>
            /// Syndicate
            /// </summary>
            syndicate = 185,

            /// <summary>
            /// Periodic
            /// </summary>
            periodic = 186,

            /// <summary>
            /// Network
            /// </summary>
            network = 187,

            /// <summary>
            /// Network Outdoor
            /// </summary>
            networkOutdoor = 188,

            /// <summary>
            /// Target
            /// </summary>
            target = 189,

            /// <summary>
            /// Wave
            /// </summary>
            wave = 190,

            /// <summary>
            /// Presence = location
            /// </summary>
            presence = 191,

            /// <summary>
            /// Media Owner
            /// </summary>
            MediaOwner = 192,

            /// <summary>
            /// Periodicity
            /// </summary>
            Periodicity = 193,

            /// <summary>
            /// Program Status
            /// </summary>
            ProgramStatus = 194,

            /// <summary>
            /// Program Typology
            /// </summary>
            ProgramTypology = 195,

            /// <summary>
            /// Program Beginning Time
            /// </summary>
            ProgramBeginningTime = 196,

            /// <summary>
            ///Program Ending Time
            /// </summary>
            ProgramEndingTime = 197,

            /// <summary>
            ///Spot Type
            /// </summary>
            SpotType = 198,

            /// <summary>
            ///Spot Sub Type
            /// </summary>
            SpotSubType = 199,

            /// <summary>
            /// Spot Genre
            /// </summary>
            SpotGenre = 200,

            /// <summary>
            /// Spot Sub Genre
            /// </summary>
            SpotSubGenre = 201,

            /// <summary>
            /// Language Of TheAd
            /// </summary>
            LanguageOfTheAd = 202,

            /// <summary>
            /// Version Name
            /// </summary>
            VersionName = 203,

            /// <summary>
            /// Creative Agency
            /// </summary>
            CreativeAgency = 204,

            /// <summary>
            /// Campaign
            /// </summary>
            Campaign = 205,

            /// <summary>
            /// Ad Slogan
            /// </summary>
            AdSlogan = 206,

            /// <summary>
            ///  Type of Advertising Company
            /// </summary>
            TypeOfAdvertisingCompany = 207,

            /// <summary>
            /// Purchasing Advertising
            /// </summary>
            PurchasingAdvertising = 208,

            /// <summary>
            ///  Spot End Time
            /// </summary>
            SpotEndTime = 209,

            /// <summary>
            ///   Spots Place in Commercial Break (In or Outside Commercial Break)
            /// </summary>
            SpotsPlaceInCommercialBreakInOrOutsideCommercialBreak = 210,

            /// <summary>
            ///  Number of Spots in Commercial Break
            /// </summary>
            NumberOfSpotsInCommercialBreak = 211,

            /// <summary>
            ///    Spot Rank Type
            /// </summary>
            SpotRankType = 212,

            /// <summary>
            ///  Commercial Break Beginning Time 
            /// </summary>
            CommercialBreakBeginningTime = 213,

            /// <summary>
            ///  Commercial Break Ending Time
            /// </summary>
            CommercialBreakEndingTime = 214,

            /// <summary>
            ///  Number of Commercial Breaks in the Program
            /// </summary>
            NumberOfCommercialBreaksInTheProgram = 215,

            /// <summary>
            ///  Commercial Item Place the Program
            /// </summary>
            CommercialItemPlaceTheProgram = 216,

            /// <summary>
            /// Number of Spots
            /// </summary>
            NumberofSpots = 217,

            /// <summary>
            /// Spots Place ( or position ) in the Program when adding all breaks in the program
            /// </summary>
            SpotsPlaceIntheProgramwhenaddingallbreaksIntheprogram = 218,

            /// <summary>
            /// Commercial Item Counter - Total number of commercial items before and within the program
            /// </summary>
            CommercialItemCounter = 219,

            /// <summary>
            /// Commercial item Place \ Position in the Program
            /// </summary>
            CommercialitemPlacePositionIntheProgram = 220,

            /// <summary>
            /// Commercial Item Category Name
            /// </summary>
            CommercialItemCategoryName = 221,

            /// <summary>
            /// Commercial Item Category Code
            /// </summary>
            CommercialItemCategoryCode = 222,

            /// <summary>
            /// Commercial Item Position within the program
            /// </summary>
            CommercialItemPositionwithintheprogram = 223,

            /// <summary>
            /// AdSpends par TV Spot in TL
            /// </summary>
            AdSpendsPerTvSpotInTl = 224,

            /// <summary>
            /// AdSpends par TV Spot in USD
            /// </summary>
            AdSpendsPerTvSpotInUsd = 225,

            /// <summary>
            /// Second Product
            /// </summary>
            SecondProduct = 226,

            /// <summary>
            /// Second Product Type
            /// </summary>
            SecondProductType = 227,

            /// <summary>
            /// "Second Product Brand
            /// </summary>
            SecondProductBrand = 228,

            /// <summary>
            /// Second Product Advertising Company
            /// </summary>
            SecondProductAdvertisingCompany = 229,

            /// <summary>
            /// Type of Second Product's Advertising Company
            /// </summary>
            SecondProductAdvertisingCompanyType = 230,

            /// <summary>
            /// Second Product: Holding of The Advertising Company
            /// </summary>
            SecondProductHoldingCompany = 231,

            /// <summary>
            /// Second Product Sub Sector
            /// </summary>
            SecondProductSubSector = 232,

            /// <summary>
            /// Second Product Main Sector
            /// </summary>
            SecondProductMainSector = 233,

            /// <summary>
            /// Collective Product
            /// </summary>
            CollectiveProduct = 234,

            /// <summary>
            /// Collective Product Type
            /// </summary>
            CollectiveProductType = 235,

            /// <summary>
            /// Collective Product Brand
            /// </summary>
            CollectiveProductBrand = 236,

            /// <summary>
            /// Collective Product Advertising Company
            /// </summary>
            CollectiveProductAdvertisingCompany = 237,

            /// <summary>
            /// Collective Product Type of Advertising Company
            /// </summary>
            CollectiveProductTypeofAdvertisingCompany = 238,

            /// <summary>
            /// Collective Product : Holding of The Advertising Company
            /// </summary>
            CollectiveProductHoldingheAdvertisingCompany = 239,

            /// <summary>
            /// Collective Product Main Sector
            /// </summary>
            CollectiveProductMainSector = 240,

            /// <summary>
            /// Collective Product Sub Sector
            /// </summary>
            CollectiveProductSubSector = 241,

            /// <summary>
            /// Sequence Of the Week In The Year
            /// </summary>
            WeekNumber = 242,
            /// <summary>
            /// Program turkey
            /// </summary>
            ProgramTk = 243,
            /// <summary>
            /// Program Commercial Break Rank
            /// </summary>
            ProgramComBreakRank = 244,
            /// <summary>
            /// Day
            /// </summary>
            Day = 245,
            /// <summary>
            /// Month
            /// </summary>
            Month = 246,
            /// <summary>
            /// Year
            /// </summary>
            Year = 247,
            /// <summary>
            /// Month Year
            /// </summary>
            MonthYear = 248,
            /// <summary>
            /// Day Name
            /// </summary>
            DayName = 249,
            /// <summary>
            /// Spot Place in the Commercial Break out of all spots
            /// </summary>
            SpotPlaceCommercialBreak = 250,
            /// <summary>
            /// Com break place / Number of com breaks in the Program
            /// </summary>
            ComBreakCustom = 251,
            /// <summary>
            /// Category Name / Position / Sequence # of Commercial Break
            /// </summary>
            ComItemCustom = 252,
            /// <summary>
            /// Sequence # of commercial items / blocks broadcasted throughout the program
            /// </summary>
            PorgramComItemCustom = 253,
            /// <summary>
            /// Commercial Item Category Name and Position
            /// </summary>
            ComItemCategoryCustom = 254,
            /// <summary>
            /// Media Code
            /// </summary>
            MediaCode = 255,
            /// <summary>
            /// Version Code
            /// </summary>
            VersionCode = 256,
            /// <summary>
            /// Campaign Code
            /// </summary>
            CampaignCode = 257,
            /// <summary>
            /// Program Genre
            /// </summary>
            ProgramGenre = 258,
            /// <summary>
            /// Creative Duration
            /// </summary>
            CreativeDuration = 259,
            /// <summary>
            /// Date First Appearance
            /// </summary>
            DateFirstAppearance = 260,
            /// <summary>
            /// Media First Appeared
            /// </summary>
            MediaFirstAppeared = 261,
            /// <summary>
            /// Media First Appeared
            /// </summary>
            FirstTopDiffusion = 262
        }

        #endregion

        #region Variables
        /// <summary>
        /// Identitifant de la Colonne
        /// </summary>
        private GenericColumnItemInformation.Columns _id;
        /// <summary>
		/// Identitifant du niveau de détail
		/// </summary>
		private Int64 _idLevel;
        /// <summary>
        /// Nom de la Colonne
        /// </summary>
        private string _name;
        /// <summary>
        /// Identifiant du texte de la Colonne
        /// </summary>
        private Int64 _webTextId;
        /// <summary>
        /// Champ identifiant pour le niveau de détail de la base de données AdExpress 3
        /// </summary>
        private string _dataBaseIdField;
        /// <summary>
        /// Champ libellé logique pour l'identifiant de la colonne de la base de données AdExpress 3
        /// </summary>
        private string _dataBaseAliasIdField = null;
        /// <summary>
        /// Champ libellé pour l'alias de la colonne de la base de données AdExpress 3
        /// </summary>
        private string _dataBaseField;
        /// <summary>
        /// Champ libellé logique pour la colonne de la base de données AdExpress 3
        /// </summary>
        private string _dataBaseAliasField = null;
        /// <summary>
        /// Nom de la table pour la colonne de la base de données AdExpress 3
        /// </summary>
        private string _dataBaseTableName = null;
        /// <summary>
        /// Préfixe de la table pour la colonne de la base de données AdExpress 3
        /// </summary>
        private string _dataBaseTableNamePrefix = null;
        /// <summary>
        /// Doit convertir la valeur null d'un identifiant en 0
        /// </summary>
        private bool _convertNullDbId = false;
        /// <summary>
        /// Doit convertir la valeur null d'un champ en 0
        /// </summary>
        private bool _convertNullDbField = false;
        /// <summary>
        /// Indique si la colonne est présente dans un export excel
        /// </summary>
        private bool _notInExcelExport = false;
        /// <summary>
        /// Indique si la colonne s'affiche dans la page web ou pas
        /// </summary>
        private bool _visible = true;
        /// <summary>
        /// Contient toutes les contraintes de la colonne
        /// </summary>
        private Hashtable _constraints = new Hashtable();

        /// <summary>
        /// Indique le prexife de la table cible pour une jointure sur avec un autre table
        /// </summary>
        private string _dbRelatedTablePrefixeForJoin = "";

        /// <summary>
        /// Operation sql
        /// </summary>
        private string _sqlOperation = "innerJoin";

        /// <summary>
        /// Correspondance avec le niveau de détail regroupé en ligne
        /// </summary>
        private int _idDetailLevelMatching = 0;

        /// <summary>
        /// Le type de la cellule
        /// </summary>
        private string _cellType = string.Empty;

        /// <summary>
        /// String output format
        /// </summary>
        private string _strFormat = string.Empty;
        /// <summary>
        /// Specify if column is a sum of units
        /// </summary>
        private bool _isSum = false;
        /// <summary>
        /// Specify if column is a count of units
        /// </summary>
        private bool _isCountDistinct = false;
        /// <summary>
        /// Specify if column is a max of units
        /// </summary>
        private bool _isMax = false;
        /// <summary>
        /// Specify if column is a min of units
        /// </summary>
        private bool _isMin = false;
        /// <summary>
        /// Specify if column is submitted to language rule or not
        /// </summary>
        private bool _useLanguage = true;
        /// <summary>
        /// Specify if column is submitted to   activation rule or not
        /// </summary>
        private bool _useActivation = true;
        /// <summary>
        /// Specify if column is in basket from the beginning or not
        /// </summary>
        private bool _inBasket = false;
        /// <summary>
        /// Indique si les données peuvent contenir un separateur
        /// </summary>
        private bool _isContainsSeparator = false;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="id">Identitifant de la colonne</param>
        /// <param name="name">Nom de la colonne</param>
        /// <param name="webTextId">Identifiant du texte de la colonne</param>
        /// <param name="dataBaseIdField">Champ identifiant pour la colonne de la base de données AdExpress 3</param>
        /// <param name="dataBaseField">Champ libellé pour la colonne de la base de données AdExpress 3</param>
        /// <param name="dataBaseAliasField">Champ libellé logique pour la colonne de la base de données AdExpress 3</param>
        /// <param name="dataBaseTableName">Nom de la table pour la colonne de la base de données AdExpress 3</param>
        /// <param name="dataBaseTableNamePrefix">Préfixe de la table pour la colonne de la base de données AdExpress 3</param>
        /// <param name="convertNullDbId">Doit convertir la valeur null d'un identifiant en 0</param>
        /// <param name="convertNullDbField">Doit convertir la valeur null d'un champ en 0</param>
        /// <param name="dataBaseAliasIdField">Champ libellé logique pour l'identifiant de la colonne de la base de données AdExpress 3</param>
        public GenericColumnItemInformation(Int64 id, string name, Int64 webTextId, string dataBaseIdField, string dataBaseAliasIdField, bool convertNullDbId, string dataBaseField, string dataBaseAliasField, bool convertNullDbField, string dataBaseTableName, string dataBaseTableNamePrefix, string cellType, string strFormat, Int64 idLevel)
        {
            if (id < 0) throw (new ArgumentException("Invalid argument id"));
            if (name == null || name.Length < 1) throw (new ArgumentException("Invalid argument name"));
            if (webTextId < 0) throw (new ArgumentException("Invalid argument webTextId"));
            //			if((dataBaseIdField==null ||dataBaseIdField.Length<1) && (dataBaseField==null ||dataBaseField.Length<1))throw(new ArgumentException("Invalid argument dataBaseIdField and dataBaseField"));
            //			if(dataBaseField==null ||dataBaseField.Length<1)throw(new ArgumentException("Invalid argument dataBaseField"));  
            try
            {
                _id = (GenericColumnItemInformation.Columns)id;
            }
            catch (System.Exception err)
            {
                throw (new ArgumentException("Incorrect column Id", "id", err));
            }
            _name = name;
            _strFormat = strFormat;
            _webTextId = webTextId;
            _dataBaseIdField = dataBaseIdField;
            _dataBaseField = dataBaseField;
            _convertNullDbId = convertNullDbId;
            _convertNullDbField = convertNullDbField;
            _idLevel = idLevel;
            if (dataBaseAliasField != null && dataBaseAliasField.Length > 0) _dataBaseAliasField = dataBaseAliasField;
            if (dataBaseAliasIdField != null && dataBaseAliasIdField.Length > 0) _dataBaseAliasIdField = dataBaseAliasIdField;
            if (dataBaseTableName != null && dataBaseTableName.Length > 0) _dataBaseTableName = dataBaseTableName;
            if (dataBaseTableNamePrefix != null && dataBaseTableNamePrefix.Length > 0) _dataBaseTableNamePrefix = dataBaseTableNamePrefix;
            if (cellType != null && cellType.Length > 0) _cellType = cellType;
            if (_convertNullDbId && dataBaseAliasIdField == null) throw (new GenericDetailLevelException("Id alias have to be defined if convertNullDbId is true"));
            if (_convertNullDbField && dataBaseAliasField == null) throw (new GenericDetailLevelException("Field alias have to be defined if convertNullDbId is true"));
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="id">Identitifant de la colonne</param>
        /// <param name="name">Nom de la colonne</param>
        /// <param name="webTextId">Identifiant du texte de la colonne</param>
        /// <param name="dataBaseIdField">Champ identifiant pour la colonne de la base de données AdExpress 3</param>
        /// <param name="dataBaseField">Champ libellé pour la colonne de la base de données AdExpress 3</param>
        /// <param name="dataBaseAliasField">Champ libellé logique pour la colonne de la base de données AdExpress 3</param>
        /// <param name="dataBaseTableName">Nom de la table pour la colonne de la base de données AdExpress 3</param>
        /// <param name="dataBaseTableNamePrefix">Préfixe de la table pour la colonne de la base de données AdExpress 3</param>
        /// <param name="convertNullDbId">Doit convertir la valeur null d'un identifiant en 0</param>
        /// <param name="convertNullDbField">Doit convertir la valeur null d'un champ en 0</param>
        /// <param name="dataBaseAliasIdField">Champ libellé logique pour l'identifiant de la colonne de la base de données AdExpress 3</param>		
        /// <param name="dbRelatedTablePrefixeForJoin">Préfixe de la table pour la colonne de la base de données AdExpress 3 pour jointure</param>
        /// <param name="sqlOperation">Opertation sql</param>
        public GenericColumnItemInformation(Int64 id, string name, Int64 webTextId, string dataBaseIdField, string dataBaseAliasIdField, bool convertNullDbId, string dataBaseField, string dataBaseAliasField, bool convertNullDbField, string dataBaseTableName, string dataBaseTableNamePrefix, string cellType, string strFormat, Int64 idLevel, string dbRelatedTablePrefixeForJoin, string sqlOperation)
            : this(id, name, webTextId, dataBaseIdField, dataBaseAliasIdField, convertNullDbId, dataBaseField, dataBaseAliasField, convertNullDbField, dataBaseTableName, dataBaseTableNamePrefix, cellType, strFormat, idLevel)
        {

            if (dbRelatedTablePrefixeForJoin != null && dbRelatedTablePrefixeForJoin.Length > 0) _dbRelatedTablePrefixeForJoin = dbRelatedTablePrefixeForJoin;
            if (sqlOperation != null && sqlOperation.Length > 0) _sqlOperation = sqlOperation;
        }
        #endregion

        #region Accesseurs
        /// <summary>
        /// Specify if column is submitted to language rule or not
        /// </summary>
        public bool UseLanguageRule
        {
            get { return _useLanguage; }
            set { _useLanguage = value; }
        }
        /// <summary>
        /// Specify if column is submitted to activation rule or not
        /// </summary>
        public bool UseActivationRule
        {
            get { return _useActivation; }
            set { _useActivation = value; }
        }
        /// <summary>
        /// Specify if column is a sum of units
        /// </summary>
        public bool IsSum
        {
            get { return _isSum; }
            set { _isSum = value; }
        }
        /// <summary>
        /// Specify if column is a max of units
        /// </summary>
        public bool IsMax
        {
            get { return _isMax; }
            set { _isMax = value; }
        }
        /// <summary>
        /// Specify if column is a min of units
        /// </summary>
        public bool IsMin
        {
            get { return _isMin; }
            set { _isMin = value; }
        }
        /// <summary>
        /// Specify if column is a cout of units
        /// </summary>
        public bool IsCountDistinct
        {
            get { return _isCountDistinct; }
            set { _isCountDistinct = value; }
        }
        /// <summary>
        /// Obtient l'identitifant de la colonne
        /// </summary>
        public GenericColumnItemInformation.Columns Id
        {
            get { return (_id); }
        }
        /// <summary>
        /// Obtient l'id du level correspondant
        /// </summary>
        public Int64 IdLevel
        {
            get { return (_idLevel); }
        }
        /// <summary>
        /// Obtient le nom de la colonne
        /// </summary>
        public string Name
        {
            get { return (_name); }
        }
        /// <summary>
        /// Obtient l'identifiant du texte de la colonne
        /// </summary>
        public Int64 WebTextId
        {
            get { return (_webTextId); }
        }
        /// <summary>
        /// Obtient le champ identifiant pour la colonne de la base de données AdExpress 3
        /// </summary>
        public string DataBaseIdField
        {
            get { return (_dataBaseIdField); }
        }
        /// <summary>
        /// Obtient le champ libellé pour la colonne de la base de données AdExpress 3
        /// </summary>
        public string DataBaseField
        {
            get { return (_dataBaseField); }
        }
        /// <summary>
        /// Obtient le champ libellé logique pour la colonne de la base de données AdExpress 3
        /// </summary>
        public string DataBaseAliasField
        {
            get { return (_dataBaseAliasField); }
        }
        /// <summary>
        /// Obtient le champ libellé logique pour l'identifiant de la colonne de la base de données AdExpress 3
        /// </summary>
        public string DataBaseAliasIdField
        {
            get { return (_dataBaseAliasIdField); }
            set { _dataBaseAliasIdField = value; }
        }
        /// <summary>
        /// Obtient le nom de la table pour la colonne de la base de données AdExpress 3
        /// </summary>
        public string DataBaseTableName
        {
            get { return (_dataBaseTableName); }
        }
        /// <summary>
        /// Obtient le préfixe de la table pour la colonne de la base de données AdExpress 3
        /// </summary>
        public string DataBaseTableNamePrefix
        {
            get { return (_dataBaseTableNamePrefix); }
            set { _dataBaseTableNamePrefix = value; }
        }
        /// <summary>
        /// Indique si on doit convertir la valeur null d'un identifiant en 0
        /// </summary>
        public bool ConvertNullDbId
        {
            get { return (_convertNullDbId); }
        }
        /// <summary>
        /// Indique si on doit convertir la valeur null d'un champ en 0
        /// </summary>
        public bool ConvertNullDbField
        {
            get { return (_convertNullDbField); }
        }

        /// <summary>
        /// Indique si la colonne est présente dans un export excel
        /// </summary>
        public bool NotInExcelExport
        {
            get { return (_notInExcelExport); }
            set { _notInExcelExport = value; }
        }

        /// <summary>
        /// Indique si la colonne s'affiche dans la page web
        /// </summary>
        [Obsolete("Obsolete because it applys the property to all sets of columns IDs which false because an information could be displayed in one set but not in an other")]
        public bool Visible
        {
            get { return (_visible); }
            set { _visible = value; }
        }

        /// <summary>
        /// Champ(s) libellé(s) logique pour la colonne de la base de données AdExpress 3 qui contistuent des contraintes pour la colonne résultat à afficher
        /// </summary>
        public Hashtable Constraints
        {
            get { return _constraints; }
            set { _constraints = value; }
        }

        /// <summary>
        ///Indique le prexife de la table cible pour une jointure sur avec un autre table
        /// </summary>
        public string DbRelatedTablePrefixeForJoin
        {
            get { return _dbRelatedTablePrefixeForJoin; }
            set { _dbRelatedTablePrefixeForJoin = value; }
        }

        /// <summary>
        ///Indique le prexife de la table cible pour une jointure sur avec un autre table
        /// </summary>
        public string SqlOperation
        {
            get { return _sqlOperation; }
            set { _sqlOperation = value; }
        }


        /// <summary>
        /// Correspondance avec le niveau de détail regroupé en ligne
        /// </summary>
        public int IdDetailLevelMatching
        {
            get { return _idDetailLevelMatching; }
            set { _idDetailLevelMatching = value; }
        }

        /// <summary>
        /// Indique le type de la cellule
        /// </summary>
        public string CellType
        {
            get { return _cellType; }
            set { _cellType = value; }
        }

        /// <summary>
        /// Get / Set Output string format
        /// </summary>
        public string StringFormat
        {
            get { return _strFormat; }
            set { _strFormat = value; }
        }

        /// <summary>
        /// Indicate if the column is in the basket from the beginning
        /// </summary>
        public bool InBasket
        {
            get { return (_inBasket); }
            set { _inBasket = value; }
        }

        /// <summary>
        ///Obtient \ Définit si les données peuvent contenir un separateur
        /// </summary>
        public bool IsContainsSeparator
        {
            get { return _isContainsSeparator; }
            set { _isContainsSeparator = value; }
        }
        #endregion

        #region Méthode publiques

        #region SQL

        #region select
        /// <summary>
        /// Obtient le code SQL pour le champ
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Code SQL</returns>
        public string GetSqlField()
        {
            string sql = "";
            string prefix = "";
            if (_dataBaseTableNamePrefix != null && _dataBaseTableNamePrefix.Length > 0 && _dataBaseField != null && _dataBaseField.Length > 0) prefix = _dataBaseTableNamePrefix + ".";
            if (_convertNullDbField) sql += "nvl(" + prefix + _dataBaseField + ",0)";
            else sql += prefix + _dataBaseField;
            if (sql.Length > 0 && _isSum) sql = string.Format("sum({0})", sql);
            if (sql.Length > 0 && _isCountDistinct) sql = string.Format("count(distinct {0})", sql);
            if (sql.Length > 0 && _isMax) sql = string.Format("max({0})", sql);
            if (sql.Length > 0 && _isMin) sql = string.Format("min({0})", sql);
            if (_dataBaseAliasField != null) sql += " as " + _dataBaseAliasField;
            return (sql);
        }
        /// <summary>
        /// Obtient le code SQL pour le champ sans le préfixe de la table
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Code SQL</returns>
        public string GetSqlFieldWithoutTablePrefix()
        {
            string sql = "";
            if (_convertNullDbField) sql += "nvl(" + _dataBaseField + ",0)";
            else sql += _dataBaseField;
            if (sql.Length > 0 && _isSum) sql = string.Format("sum({0})", sql);
            if (sql.Length > 0 && _isCountDistinct) sql = string.Format("count(distinct {0})", sql);
            if (sql.Length > 0 && _isMax) sql = string.Format("max({0})", sql);
            if (sql.Length > 0 && _isMin) sql = string.Format("min({0})", sql);
            if (_dataBaseAliasField != null) sql += " as " + _dataBaseAliasField;
            return (sql);
        }
        /// <summary>
        /// Obtient le code SQL pour l'identifiant du champ
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Code SQL</returns>
        public string GetSqlFieldId()
        {
            string sql = "";
            string prefix = "";
            if (_dataBaseTableNamePrefix != null && _dataBaseTableNamePrefix.Length > 0 && _dataBaseIdField != null && _dataBaseIdField.Length > 0)
                prefix = _dataBaseTableNamePrefix + ".";
            if (_convertNullDbId) sql += "nvl(" + prefix + _dataBaseIdField + ",0)";
            else sql += prefix + _dataBaseIdField;
            if (sql.Length > 0 && _isSum) sql = string.Format("sum({0})", sql);
            if (sql.Length > 0 && _isCountDistinct) sql = string.Format("count(distinct {0})", sql);
            if (sql.Length > 0 && _isMax) sql = string.Format("max({0})", sql);
            if (sql.Length > 0 && _isMin) sql = string.Format("min({0})", sql);
            if (_dataBaseAliasIdField != null) sql += " as " + _dataBaseAliasIdField;
            return (sql);
        }
        /// <summary>
        /// Obtient le code SQL pour l'identifiant du champ sans le préfixe de la table
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Code SQL</returns>
        public string GetSqlFieldIdWithoutTablePrefix()
        {
            string sql = "";
            if (_convertNullDbField) sql += "nvl(" + _dataBaseIdField + ",0)";
            else sql += _dataBaseIdField;
            if (sql.Length > 0 && _isSum) sql = string.Format("sum({0})", sql);
            if (sql.Length > 0 && _isCountDistinct) sql = string.Format("count(distinct {0})", sql);
            if (sql.Length > 0 && _isMax) sql = string.Format("max({0})", sql);
            if (sql.Length > 0 && _isMin) sql = string.Format("min({0})", sql);
            if (_dataBaseAliasIdField != null) sql += " as " + _dataBaseAliasIdField;
            return (sql);
        }
        #endregion

        #region Order by
        /// <summary>
        /// Obtient le code SQL du le champ pour la commande order
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Code SQL</returns>
        public string GetSqlFieldForOrder()
        {
            if (_dataBaseAliasField != null) return (_dataBaseAliasField);
            if (_dataBaseField != null)
            {
                if (_dataBaseTableNamePrefix != null)
                    return (_dataBaseTableNamePrefix + "." + _dataBaseField);
                else return (_dataBaseField);
            }
            else return "";
        }

        /// <summary>
        /// Obtient le code SQL du le champ pour la commande order sans le préfixe de la table
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Code SQL</returns>
        public string GetSqlFieldForOrderWithoutTablePrefix()
        {
            if (_dataBaseAliasField != null) return (_dataBaseAliasField);
            return (_dataBaseField);
        }

        /// <summary>
        /// Obtient le code SQL de l'identifiant de champ pour la commande order
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Code SQL</returns>
        public string GetSqlIdFieldForOrder()
        {
            if (_dataBaseAliasIdField != null) return (_dataBaseAliasIdField);
            if (_dataBaseIdField != null)
            {
                if (_dataBaseTableNamePrefix != null)
                    return (_dataBaseTableNamePrefix + "." + _dataBaseIdField);
                else return _dataBaseIdField;
            }
            else return "";
        }

        /// <summary>
        /// Obtient le code SQL de l'identifiant de champ pour la commande order sans le préfixe de la table
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Code SQL</returns>
        public string GetSqlIdFieldForOrderWithoutTablePrefix()
        {
            if (_dataBaseAliasIdField != null) return (_dataBaseAliasIdField);
            return (_dataBaseIdField);
        }
        #endregion

        #region Group by
        /// <summary>
        /// Obtient le code SQL du le champ pour la commande Group By sans le préfixe de la table
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Code SQL</returns>
        public string GetSqlFieldForGroupBy()
        {

            //			if(_dataBaseAliasField!=null)return(_dataBaseAliasField);
            if (_dataBaseField != null)
            {
                if (_dataBaseTableNamePrefix != null)
                    return (_dataBaseTableNamePrefix + "." + _dataBaseField);
                else return _dataBaseField;
            }
            else return "";
        }
        /// <summary>
        /// Obtient le code SQL du le champ pour la commande Group By
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Code SQL</returns>
        public string GetSqlFieldForGroupByWithoutTablePrefix()
        {
            return string.Empty;
        }
        /// <summary>
        /// Obtient le code SQL de l'identifiant de champ pour la commande Group By
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Code SQL</returns>
        public string GetSqlIdFieldForGroupBy()
        {

            if (_dataBaseAliasIdField != null) return (_dataBaseAliasIdField);
            if (_dataBaseIdField != null)
            {
                if (_dataBaseTableNamePrefix != null)
                    return (_dataBaseTableNamePrefix + "." + _dataBaseIdField);
                else return _dataBaseIdField;
            }
            else return "";
        }
        /// <summary>
        /// Obtient le code SQL de l'identifiant de champ pour la commande Group By sans le préfixe de la table
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Code SQL</returns>
        public string GetSqlIdFieldForGroupByWithoutTablePrefix()
        {
            return (_dataBaseIdField);
        }
        #endregion

        #endregion

        #region Rules
        /// <summary>
        /// Obtient le libélle du le champ pour la commande order
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Code SQL</returns>
        public string GetFieldForRules()
        {
            if (_dataBaseAliasField != null) return (_dataBaseAliasField);
            return (_dataBaseField);
        }
        /// <summary>
        /// Obtient le libélle de l'identifiant du le champ pour la commande order
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Code SQL</returns>
        public string GetIdFieldForRules()
        {
            if (_dataBaseAliasIdField != null) return (_dataBaseAliasIdField);
            return (_dataBaseIdField);
        }
        #endregion

        /// <summary>
        /// Obtient le nom du champ avec le nom logique (s'il existe)
        /// </summary>
        /// <remarks>La virgule n'est pas ajoutée</remarks>
        /// <returns>Nom du champ avec le nom logique</returns>
        public string GetTableNameWithPrefix()
        {
            string tmp = "";
            if (_dataBaseTableName == null) return (null);
            tmp += _dataBaseTableName + " ";
            if (_dataBaseTableNamePrefix != null) tmp += _dataBaseTableNamePrefix + " ";
            return (tmp);
        }

        #endregion


    }
}
