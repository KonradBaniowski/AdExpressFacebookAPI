#region Information
//  Author : G. Facon, G. Ragneau
//  Creation  date: 29/02/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.XmlLoader;

namespace TNS.AdExpress.Domain.DataBaseDescription {

    #region Enums

    #region Customer Connection Ids
    /// <summary>
    /// Customer Connection Ids
    /// </summary>
    public enum CustomerConnectionIds {
        /// <summary>
        /// AdExpress Data
        /// </summary>
        adexpr03=0,
    } 
    #endregion

    #region Default Connection Ids
    /// <summary>
    /// Default Connection Ids
    /// </summary>
    public enum DefaultConnectionIds {
        /// <summary>
        /// Session
        /// </summary>
        session=0,
        /// <summary>
        /// Translation
        /// </summary>
        translation=1,
        /// <summary>
        /// Product class analysis
        /// </summary>
        productClassAnalysis=2,
        /// <summary>
        /// APPM (Chronopresse)
        /// </summary>
        grp=3,
        /// <summary>
        /// Geb alert
        /// </summary>
        geb=4,
        /// <summary>
        /// Hermes tool
        /// </summary>
        hermes=5,
        /// <summary>
        /// Use to retreive publication date
        /// </summary>
        publication=6,
        /// <summary>
        /// Web Administration
        /// </summary>
        webAdministration=7,
        /// <summary>
        /// Alerts
        /// </summary>
        alert=8,
        /// <summary>
        /// Music
        /// </summary>
        music=9,
		/// <summary>
		/// rights
		/// </summary>
		rights = 10,
        /// <summary>
        /// AdEXpress Russia
        /// </summary>
        adExpressRussia = 11,
        /// <summary>
        /// Promo monitoring
        /// </summary>
        vPromo = 12,
        /// <summary>
        /// bipp
        /// </summary>
        bipp =13,
        /// <summary>
        /// Rolex
        /// </summary>
        rolex = 14

    } 
    #endregion

    #region Schema Ids
    /// <summary>
    /// Schema Ids
    /// </summary>
    public enum SchemaIds {
        /// <summary>
        /// AdExpr03
        /// </summary>
        adexpr03=0,
        /// <summary>
        /// Chronopresse
        /// </summary>
        appm01=1,
        /// <summary>
        /// Right (Mau)
        /// </summary>
        mau01=2,
        /// <summary>
        /// Mou
        /// </summary>
        mou01=3,
        /// <summary>
        /// Product class analysis
        /// </summary>
        recap01=4,
        /// <summary>
        /// Web
        /// </summary>
        webnav01=5,       
        /// <summary>
        /// 
        /// </summary>
        alert = 10,
        /// <summary>
        /// Promotion 
        /// </summary>
        promo03=11,
        /// <summary>
        /// Rolex 
        /// </summary>
        rolex03 = 12
    } 
    #endregion

    #region Table Ids
    /// <summary>
    /// Table Ids
    /// </summary>
    public enum TableIds {
        /// <summary>
        /// List saved levels
        /// </summary>
        list = 0,
        /// <summary>
        /// List detail
        /// </summary>
        listDetail = 1,
        /// <summary>
        /// Tracking
        /// </summary>
        tracking = 2,
        /// <summary>
        /// Maui01 result table
        /// </summary>
        rightResult = 3,
        /// <summary>
        /// Sector
        /// </summary>
        sector = 4,
        /// <summary>
        /// Subsector
        /// </summary>
        subsector = 5,
        /// <summary>
        /// Group
        /// </summary>
        group = 6,
        /// <summary>
        /// Segment
        /// </summary>
        segment = 7,
        /// <summary>
        /// Product
        /// </summary>
        product = 8,
        /// <summary>
        /// Holding company
        /// </summary>
        holdingCompany = 9,
        /// <summary>
        /// Advertiser
        /// </summary>
        advertiser = 10,
        /// <summary>
        /// Brand
        /// </summary>
        brand = 11,
        /// <summary>
        /// Right Templates
        /// </summary>
        rightTemplate = 12,
        /// <summary>
        /// Right templates assignment
        /// </summary>
        rightTemplateAssignment = 13,
        /// <summary>
        /// Media type
        /// </summary>
        rightMediaType = 14,
        /// <summary>
        /// ProductType
        /// </summary>
        rightProductType = 15,
        /// <summary>
        /// Vehicle
        /// </summary>
        vehicle = 16,
        /// <summary>
        /// Category
        /// </summary>
        category = 17,
        /// <summary>
        /// Media
        /// </summary>
        media = 18,
        /// <summary>
        /// Basic Media
        /// </summary>
        basicMedia = 19,
        /// <summary>
        /// Media Agency
        /// </summary>
        mediaAgency = 20,
        /// <summary>
        /// Media agency data
        /// </summary>
        dataMediaAgency = 21,
        /// <summary>
        /// Interest Center
        /// </summary>
        interestCenter = 22,
        /// <summary>
        /// Media seller
        /// </summary>
        mediaSeller = 23,
        /// <summary>
        /// Template media right
        /// </summary>
        rightMediaOrderTemplate = 24,
        /// <summary>
        /// Template product right
        /// </summary>
        rightProductOrderTemplate = 25,
        /// <summary>
        /// Gad
        /// </summary>
        gad = 26,
        /// <summary>
        /// Title
        /// </summary>
        title = 27,
        /// <summary>
        /// Anubis session (static_nav-session)
        /// </summary>
        anubisSession = 28,
        /// <summary>
        /// customer Sessions
        /// </summary>
        customerSession = 29,
        /// <summary>
        /// Customer saved sessions
        /// </summary>
        customerSessionSaved = 30,
        /// <summary>
        /// Customer saved sessions backup (test)
        /// </summary>
        customerSessionBackup = 31,
        /// <summary>
        /// Customer saved sessions test (test)
        /// </summary>
        customerSessionTest = 32,
        /// <summary>
        /// Customer universe
        /// </summary>
        customerUniverse = 33,
        /// <summary>
        /// Customer universe description
        /// </summary>
        customerUniverseDescription = 34,
        /// <summary>
        /// Customer universe groups
        /// </summary>
        customerUniverseGroup = 35,
        /// <summary>
        /// Aggregated data per month
        /// </summary>
        monthData = 36,
        /// <summary>
        /// Aggregated data per week
        /// </summary>
        weekData = 37,
        /// <summary>
        /// APPM aggregated data per month
        /// </summary>
        monthAppmData = 38,
        /// <summary>
        /// APPM aggregated data per week
        /// </summary>
        weekAppmData = 39,
        /// <summary>
        /// Press Data
        /// </summary>
        dataPress = 40,
        /// <summary>
        /// Radio Data
        /// </summary>
        dataRadio = 41,
        /// <summary>
        /// Tv Data
        /// </summary>
        dataTv = 42,
        /// <summary>
        /// OutDoor Data
        /// </summary>
        dataOutDoor = 43,
        /// <summary>
        /// Internet Data
        /// </summary>
        dataInternet = 44,
        /// <summary>
        /// AdNetTrack Data (Evaliant)
        /// </summary>
        dataAdNetTrack = 45,
        /// <summary>
        /// APPM Data
        /// </summary>
        dataPressAPPM = 46,
        /// <summary>
        /// Sponsorship Data
        /// </summary>
        dataSponsorship = 47,
        /// <summary>
        /// Direct Marketing Data
        /// </summary>
        dataMarketingDirect = 48,
        /// <summary>
        /// International Press Data
        /// </summary>
        dataPressInter = 49,
        /// <summary>
        /// 4M press Data
        /// </summary>
        dataPressAlert = 50,
        /// <summary>
        /// 4M radio Data
        /// </summary>
        dataRadioAlert = 51,
        /// <summary>
        /// 4M tv Data
        /// </summary>
        dataTvAlert = 52,
        /// <summary>
        /// 4M outdoor Data
        /// </summary>
        dataOutDoorAlert = 53,
        /// <summary>
        /// 4M Internet Data
        /// </summary>
        dataInternetAlert = 54,
        /// <summary>
        /// 4M APPM Data
        /// </summary>
        dataPressAPPMAlert = 55,
        /// <summary>
        /// 4M direct marketing Data
        /// </summary>
        dataMarketingDirectAlert = 56,
        /// <summary>
        /// 4M international press Data
        /// </summary>
        dataPressInterAlert = 57,
        /// <summary>
        /// Ad press color list
        /// </summary>
        color = 58,
        /// <summary>
        /// Format
        /// </summary>
        format = 59,
        /// <summary>
        /// Mail format
        /// </summary>
        mailFormat = 60,
        /// <summary>
        /// Mail type
        /// </summary>
        mailType = 61,
        /// <summary>
        /// Mail content
        /// </summary>
        mailContent = 62,
        /// <summary>
        /// Data Mail content
        /// </summary>
        dataMailContent = 63,
        /// <summary>
        /// Mail rapidity
        /// </summary>
        mailingRapidity = 64,
        /// <summary>
        /// Press Location
        /// </summary>
        location = 65,
        /// <summary>
        /// Press Inset
        /// </summary>
        inset = 66,
        /// <summary>
        /// Insertion
        /// </summary>
        insertion = 67,
        /// <summary>
        /// alarm media
        /// </summary>
        alarmMedia = 68,
        /// <summary>
        /// Application media
        /// </summary>
        applicationMedia = 69,
        /// <summary>
        /// Product class analysis plurimedia data
        /// </summary>
        recapPluri = 70,
        /// <summary>
        /// Product class analysis press data
        /// </summary>
        recapPress = 71,
        /// <summary>
        /// Product class analysis tv data
        /// </summary>
        recapTv = 72,
        /// <summary>
        /// Product class analysis radio data
        /// </summary>
        recapRadio = 73,
        /// <summary>
        /// Product class analysis outdoor data
        /// </summary>
        recapOutDoor = 74,
        /// <summary>
        /// Product class analysis internet data
        /// </summary>
        recapInternet = 75,
        /// <summary>
        /// Product class analysis cinema data
        /// </summary>
        recapCinema = 76,
        /// <summary>
        /// Product class analysis media tactic data
        /// </summary>
        recapTactic = 77,
        /// <summary>
        /// Product class analysis plurimedia data aggregated by segment
        /// </summary>
        recapPluriSegment = 78,
        /// <summary>
        /// Product class analysis press data aggregated by segment
        /// </summary>
        recapPressSegment = 79,
        /// <summary>
        /// Product class analysis tv data aggregated by segment
        /// </summary>
        recapTvSegment = 80,
        /// <summary>
        /// Product class analysis radio data aggregated by segment
        /// </summary>
        recapRadioSegment = 81,
        /// <summary>
        /// Product class analysis outdoor data aggregated by segment
        /// </summary>
        recapOutDoorSegment = 82,
        /// <summary>
        /// Product class analysis internet data aggregated by segment
        /// </summary>
        recapInternetSegment = 83,
        /// <summary>
        /// Product class analysis cinema data aggregated by segment
        /// </summary>
        recapCinemaSegment = 84,
        /// <summary>
        /// Product class analysis media tactic data aggregated by segment
        /// </summary>
        recapTacticSegment = 85,
        /// <summary>
        /// Media periodicity
        /// </summary>
        periodicity = 86,
        /// <summary>
        /// Appm wave list
        /// </summary>
        appmWave = 87,
        /// <summary>
        /// Appm target list
        /// </summary>
        appmTarget = 88,
        /// <summary>
        /// Appm target assignment
        /// </summary>
        appmTargetMediaAssignment = 89,
        /// <summary>
        /// Product per agency
        /// </summary>
        productAgency = 90,
        /// <summary>
        /// Customer login
        /// </summary>
        rightLogin = 91,
        /// <summary>
        /// Customer contact
        /// </summary>
        rightContact = 92,
        /// <summary>
        /// Customer contact group
        /// </summary>
        rightContactGroup = 93,
        /// <summary>
        /// Customer Address
        /// </summary>
        rightAddress = 94,
        /// <summary>
        /// Customer Company
        /// </summary>
        rightCompany = 95,
        /// <summary>
        /// Connection by login tracking
        /// </summary>
        trackingConnectionByLogin = 96,
        /// <summary>
        /// AdExpress Modules
        /// </summary>
        rightModule = 97,
        /// <summary>
        /// AdExpress Module groups
        /// </summary>
        rightModuleGroup = 98,
        /// <summary>
        /// Top module tracking
        /// </summary>
        trackingTopModule = 99,
        /// <summary>
        /// Top gad tracking
        /// </summary>
        trackingTopGad = 100,
        /// <summary>
        /// Top media tracking
        /// </summary>
        trackingTopMediaAgency = 101,
        /// <summary>
        /// Top Excel export tracking
        /// </summary>
        trackingTopExcelExport = 102,
        /// <summary>
        /// Top option tracking
        /// </summary>
        trackingTopOption = 103,
        /// <summary>
        /// Top unit tracking
        /// </summary>
        trackingTopUnit = 104,
        /// <summary>
        /// Top period tracking
        /// </summary>
        trackingTopPeriod = 105,
        /// <summary>
        /// Unit list to track
        /// </summary>
        trackingUnit = 106,
        /// <summary>
        /// Period list to track
        /// </summary>
        trackingPeriod = 107,
        /// <summary>
        /// Top vehicle tracking
        /// </summary>
        trackingTopVehicle = 108,
        /// <summary>
        /// Top my AdExpress tracking
        /// </summary>
        trackingTopMyAdExpress = 109,
        /// <summary>
        /// Top vehicle by module tracking
        /// </summary>
        trackingTopVehicleByModule = 110,
        /// <summary>
        /// Login Ip tracking
        /// </summary>
        trackingLoginIp = 111,
        /// <summary>
        /// Connection duration tracking
        /// </summary>
        trackingConnectionTime = 112,
        /// <summary>
        /// Agglomeration
        /// </summary>
        agglomeration = 113,
        /// <summary>
        /// Push mail alert list
        /// </summary>
        alertPushMail = 114,
        /// <summary>
        /// Push mail alert flag assignment
        /// </summary>
        alertFlagAssignment = 115,
        /// <summary>
        /// Push mail alert universe assignment
        /// </summary>
        alertUniverseAssignment = 116,
        /// <summary>
        /// Push mail alert universe
        /// </summary>
        alertUniverse = 117,
        /// <summary>
        /// Push mail alert universe detail
        /// </summary>
        alertUniverseDetail = 118,
        /// <summary>
        /// Country
        /// </summary>
        country = 119,
        /// <summary>
        /// Advertiser in Product class analysis
        /// </summary>
        recapAdvertiser = 120,
        /// <summary>
        /// Brand in Product class analysis
        /// </summary>
        recapBrand = 121,
        /// <summary>
        /// Category in Product class analysis
        /// </summary>
        recapCategory = 122,
        /// <summary>
        /// Group in Product class analysis
        /// </summary>
        recapGroup = 123,
        /// <summary>
        /// Holding company in Product class analysis
        /// </summary>
        recapHoldingCompany = 124,
        /// <summary>
        /// Media in Product class analysis
        /// </summary>
        recapMedia = 125,
        /// <summary>
        /// Product in Product class analysis
        /// </summary>
        recapProduct = 126,
        /// <summary>
        /// Sector in Product class analysis
        /// </summary>
        recapSector = 127,
        /// <summary>
        /// Segment in Product class analysis
        /// </summary>
        recapSegment = 128,
        /// <summary>
        /// SubSector in Product class analysis
        /// </summary>
        recapSubSector = 129,
        /// <summary>
        /// Vehicle in Product class analysis
        /// </summary>
        recapVehicle = 130,
        /// <summary>
        /// Module assignment
        /// </summary>
        rightModuleAssignment = 131,
        /// <summary>
        /// Right assignment
        /// </summary>
        rightAssignment = 132,
        /// <summary>
        /// My login
        /// </summary>
        rightMyLogin = 133,
        /// <summary>
        /// Product customer rights
        /// </summary>
        rightProductOrder = 134,
        /// <summary>
        /// Media customer rights
        /// </summary>
        rightMediaOrder = 135,
        /// <summary>
        /// Module right frequency
        /// </summary>
        rightFrequency = 136,
        /// <summary>
        /// Module Category
        /// </summary>
        rightModuleCategory = 137,
        /// <summary>
        /// Flags 
        /// </summary>
        rightFlag = 138,
        /// <summary>
        /// Flags assignment
        /// </summary>
        rightProjectFlagAssignment = 139,
        /// <summary>
        /// AdNetTrack Data on last 4M (Evaliant 4M)
        /// </summary>
        dataAdNetTrackAlert = 140,
        /// <summary>
        /// Data location
        /// </summary>
        dataLocation = 141,
        /// <summary>
        /// Recap emailing
        /// </summary>
        recapEmailing = 142,
        /// <summary>
        /// Recap Emailing aggregated bu segment
        /// </summary>
        recapEmailingSegment = 143,
        /// <summary>
        /// Recap Cell phone
        /// </summary>
        recapMobileTel = 144,
        /// <summary>
        /// Recap Cell phone aggregated bu segment
        /// </summary>
        recapMobileTelSegment = 145,
        /// <summary>
        /// AdNetTrack (for Internet Version)
        /// </summary>
        dataInternetVersion = 147,
        /// <summary>
        /// AdNetTrack 4M (for Internet Version)
        /// </summary>
        dataInternetVersionAlert = 148,
        /// <summary>
        /// Product Group Advertising Agency
        /// </summary>
        productGroupAgency = 149,
        /// <summary>
        /// Advertising Agency
        /// </summary>
        advertisingAgency = 150,
        /// <summary>
        /// Group Advertising Agency
        /// </summary>
        groupAdvertisingAgency = 151,
        /// <summary>
        /// Product class analysis years loaded information
        /// </summary>
        recapInfo = 152,
        /// <summary>
        /// Cinema Data
        /// </summary>
        dataCinema = 153,
        /// <summary>
        /// 4M cinema Data
        /// </summary>
        dataCinemaAlert = 154,
        /// <summary>
        /// Customer directory
        /// </summary>
        customerDirectory = 155,
        /// <summary>
        /// Banners
        /// </summary>
        banners = 156,
        /// <summary>
        /// Format banners
        /// </summary>
        formatBanners = 157,
        /// <summary>
        /// Dimension banners
        /// </summary>
        dimensionBanners = 158,
        /// <summary>
        /// Product class analysis direct marketing data aggregated by segment
        /// </summary>
        recapDirectMarketingSegment = 159,
        /// <summary>
        /// Recap Direct Marketing
        /// </summary>
        recapDirectMarketing = 160,
        /// <summary>
        /// Evaliant mobile Data
        /// </summary>
        dataEvaliantMobile = 161,
        /// <summary>
        /// 4M Evaliant mobile Data
        /// </summary>
        dataEvaliantMobileAlert = 162,
        /// <summary>
        /// Banners
        /// </summary>
        banners_mobile = 163,
        /// <summary>
        /// Banners
        /// </summary>
        tendencyMonth = 164,
        /// <summary>
        /// Banners
        /// </summary>
        totalTendencyMonth = 165,
        /// <summary>
        /// Banners
        /// </summary>
        tendencyWeek = 166,
        /// <summary>
        /// Banners
        /// </summary>
        totalTendencyWeek = 167,
        /// <summary>
        /// dataNewspaper
        /// </summary>
        dataNewspaper = 168,
        /// <summary>
        /// dataNewspaper4M
        /// </summary>
        dataNewspaperAlert = 169,
        /// <summary>
        /// dataMagazine
        /// </summary>
        dataMagazine = 170,
        /// <summary>
        /// dataMagazine4M
        /// </summary>
        dataMagazineAlert = 171,
        /// <summary>
        /// Recap Newspaper
        /// </summary>
        recapNewspaper = 172,
        /// <summary>
        /// Recap Magazine
        /// </summary>
        recapMagazine = 173,
        /// <summary>
        /// Recap Newspaper Segment
        /// </summary>
        recapNewspaperSegment = 174,
        /// <summary>
        /// Recap Magazine Segment
        /// </summary>
        recapMagazineSegment = 175,
        /// <summary>
        /// Data location magazine
        /// </summary>
        dataLocationMagazine = 176,
        /// <summary>
        /// Data location Newspaper
        /// </summary>
        dataLocationNewspaper = 177,
        /// <summary>
        /// Product class analysis outdoor data
        /// </summary>
        recapInStore = 178,
        /// <summary>
        /// Product class analysis instore data aggregated by segment
        /// </summary>
        recapInStoreSegment = 179,
        /// <summary>
        /// Instore Data
        /// </summary>
        dataInStore = 180,
        /// <summary>
        /// 4M instore Data
        /// </summary>
        dataInStoreAlert = 181,
        /// <summary>
        /// version
        /// </summary>
        slogan = 182,
        /// <summary>
        /// Connection by login IP Time slot tracking
        /// </summary>
        trackingConnectionByLoginIpTimeslot = 183,
        /// <summary>
        /// Cinema Data Retailer
        /// </summary>
        dataCinemaRetailer = 184,
        /// <summary>
        /// 4M cinema Data Retailer
        /// </summary>
        dataCinemaAlertRetailer = 185,
        /// <summary>
        /// Internet Data Retailer
        /// </summary>
        dataInternetRetailer = 186,
        /// <summary>
        /// 4M Internet Data Retailer
        /// </summary>
        dataInternetAlertRetailer = 187,
        /// <summary>
        /// Data location magazine Retailer
        /// </summary>
        dataLocationMagazineRetailer = 188,
        /// <summary>
        /// Data location Newspaper Retailer
        /// </summary>
        dataLocationNewspaperRetailer = 189,
        /// <summary>
        /// dataMagazine Retailer
        /// </summary>
        dataMagazineRetailer = 190,
        /// <summary>
        /// dataMagazine4M Retailer
        /// </summary>
        dataMagazineAlertRetailer = 191,
        /// <summary>
        /// dataNewspaper Retailer
        /// </summary>
        dataNewspaperRetailer = 192,
        /// <summary>
        /// dataNewspaper4M Retailer
        /// </summary>
        dataNewspaperAlertRetailer = 193,
        /// <summary>
        /// OutDoor Data Retailer
        /// </summary>
        dataOutDoorRetailer = 194,
        /// <summary>
        /// 4M outdoor Data Retailer
        /// </summary>
        dataOutDoorAlertRetailer = 195,
        /// <summary>
        /// Radio Data Retailer
        /// </summary>
        dataRadioRetailer = 196,
        /// <summary>
        /// 4M radio Data Retailer
        /// </summary>
        dataRadioAlertRetailer = 197,
        /// <summary>
        /// Tv Data Retailer
        /// </summary>
        dataTvRetailer = 198,
        /// <summary>
        /// 4M tv Data Retailer
        /// </summary>
        dataTvAlertRetailer = 199,
        /// <summary>
        /// Aggregated data per month Retailer
        /// </summary>
        monthDataRetailer = 201,
        /// <summary>
        /// Aggregated data per week Retailer
        /// </summary>
        weekDataRetailer = 202,
        /// <summary>
        /// Product class analysis cinema data Retailer Retailer
        /// </summary>
        recapCinemaRetailer = 203,
        /// <summary>
        /// Product class analysis cinema data Retailer Retailer
        /// </summary>
        recapCinemaSegmentRetailer = 204,
        /// <summary>
        /// Product class analysis internet data Retailer
        /// </summary>
        recapInternetRetailer = 205,
        /// <summary>
        /// Product class analysis internet data aggregated by segment Retailer
        /// </summary>
        recapInternetSegmentRetailer = 206,
        /// <summary>
        /// Recap Magazine Retailer
        /// </summary>
        /// 
        recapMagazineRetailer = 207,
        /// <summary>
        /// Recap Magazine Segment Retailer
        /// </summary>
        recapMagazineSegmentRetailer = 208,
        /// <summary>
        /// Recap Newspaper Retailer
        /// </summary>
        recapNewspaperRetailer = 209,
        /// <summary>
        /// Recap Newspaper Segment Retailer
        /// </summary>
        recapNewspaperSegmentRetailer = 210,
        /// <summary>
        /// Product class analysis outdoor data Retailer
        /// </summary>
        recapOutDoorRetailer = 211,
        /// <summary>
        /// Product class analysis outdoor data aggregated by segment Retailer
        /// </summary>
        recapOutDoorSegmentRetailer = 212,
        /// <summary>
        /// Product class analysis plurimedia data Retailer
        /// </summary>
        recapPluriRetailer = 213,
        /// <summary>
        /// Product class analysis plurimedia data aggregated by segment Retailer
        /// </summary>
        recapPluriSegmentRetailer = 214,
        /// <summary>
        /// Product class analysis radio data Retailer
        /// </summary>
        recapRadioRetailer = 215,
        /// <summary>
        /// Product class analysis radio data aggregated by segment Retailer
        /// </summary>
        recapRadioSegmentRetailer = 216,
        /// <summary>
        /// Product class analysis tv data Retailer
        /// </summary>
        recapTvRetailer = 217,
        /// <summary>
        /// Product class analysis tv data aggregated by segment Retailer
        /// </summary>
        recapTvSegmentRetailer = 218,

        /// <summary>
        /// Tableau de bord par mois
        /// </summary>
        dashboardPluriMonth=219,
        /// <summary>
        /// Tableau de bord par semaine
        /// </summary>
        dashboardPluriWeek = 220,
        /// <summary>
        /// Tableau de bord radio par jour
        /// </summary>
        dashboardRadioDay = 221,
        /// <summary>
        /// Tableau de bord répartition radio part mois
        /// </summary>
        dashboardRadioRepMonth = 222,
        /// <summary>
        /// Tableau de bord répartition radio par semaine
        /// </summary>
        dashboardRadioRepWeek = 223,
        /// <summary>
        /// Tableau de bord télé par jour
        /// </summary>
        dashboardTvDay = 224,
        /// <summary>
        /// Tableau de bord télé par mois
        /// </summary>
        dashboardTvRepMonth = 225,
        /// <summary>
        /// Tableau de bord répartition télé par semaine
        /// </summary>
        dashboardTvRepWeek = 226,
        /// <summary>
        /// Tableau de bord par mois
        /// </summary>
        dashboardPluriMonthRetailer = 227,
        /// <summary>
        /// Tableau de bord par semaine
        /// </summary>
        dashboardPluriWeekRetailer = 228,

        /// <summary>
        /// Table promotion detail  product
        /// </summary>
        promoProduct = 229,
        /// <summary>
        /// Table promotion product category
        /// </summary>
        promoCategory = 230,
        /// <summary>
        /// Table promotion Brand
        /// </summary>
        promoBrand = 231,
        /// <summary>
        /// Table promotion Circuit
        /// </summary>
        promoCircuit = 232,
        /// <summary>
        /// Table data promotion
        /// </summary>
        dataPromotion = 233,
        /// <summary>
        /// Table promotion product segment
        /// </summary>
        promoSegment = 234,
        /// <summary>
        /// Customer Insertions Levels groups
        /// </summary>
        customerInsertionLevelsGroup = 235,
        /// <summary>
        /// Customer insertions levels saved
        /// </summary>
        insertionLevelsSave = 236,
        /// <summary>
        /// Group Format Banners
        /// </summary>
        groupFormatBanners = 237,
        /// <summary>
        /// Right Group Format
        /// </summary>
        rightGroupFormat = 238,
        /// <summary>
        /// Table data rolex
        /// </summary>
        dataRolex = 239,
        /// <summary>
        /// Table rolex  Location
        /// </summary>
        rolexLocation = 240,
        /// <summary>
        /// Table rolex site
        /// </summary>
        site = 241,
        /// <summary>
        /// Table rolex presence type
        /// </summary>
        presenceType = 242,
        /// <summary>
        /// Table Data mail
        /// </summary>
        dataMail = 243,

        /// <summary>
        /// Table Data mail 4M
        /// </summary>
        dataMailAlert = 244,
        /// <summary>
        /// InDoor Data
        /// </summary>
        dataInDoor = 245,
        /// <summary>
        /// 4M outdoor Data
        /// </summary>
        dataInDoorAlert = 246,
        /// <summary>
        /// Product class analysis indoor data
        /// </summary>
        recapInDoor = 247,
        /// <summary>
        /// Product class analysis indoor data aggregated by segment
        /// </summary>
        recapInDoorSegment = 248,
        /// <summary>
        /// MMS Data
        /// </summary>
        dataMms = 249,
        /// <summary>
        /// 4M MMS Data
        /// </summary>
        dataMmsAlert = 250,
        /// <summary>
        /// Media Group
        /// </summary>
        mediaGroup = 251,
        /// <summary>
        /// Auto Promo
        /// </summary>
        autoPromo = 252,
        /// <summary>
        /// Recap MMS
        /// </summary>
        recapMms = 253,
        /// <summary>
        /// Recap MMS Segment
        /// </summary>
        recapMmsSegment = 254,
        /// <summary>
        /// Search Data
        /// </summary>
        dataSearch = 255,
        /// <summary>
        /// 4M Search Data
        /// </summary>
        dataSearchAlert = 256,
        /// <summary>
        /// Recap Search
        /// </summary>
        recapSearch = 257,
        /// <summary>
        /// Recap Search Segment
        /// </summary>
        recapSearchSegment = 258,
        /// <summary>
        /// Banners Country
        /// </summary>
        bannersCountry = 259,
        /// <summary>
        /// Purchase Mode MMS
        /// </summary>
        purchaseModeMMS = 260,
        /// <summary>
        /// Search Data Retailer
        /// </summary>
        dataSearchRetailer = 261,
        /// <summary>
        /// 4M Search Data Retailer
        /// </summary>
        dataSearchAlertRetailer = 262,
        /// <summary>
        /// Recap Search Retailer
        /// </summary>
        recapSearchRetailer = 263,
        /// <summary>
        /// Recap Search Segment Retailer
        /// </summary>
        recapSearchSegmentRetailer = 264,
        /// <summary>
        /// Social Data
        /// </summary>
        dataSocial = 265,
        /// <summary>
        /// 4M Social Data
        /// </summary>
        dataSocialAlert = 266,
        /// <summary>
        /// Recap Social
        /// </summary>
        recapSocial = 267,
        /// <summary>
        /// Recap Social Segment
        /// </summary>
        recapSocialSegment = 268,
      

        /// <summary>
        /// Alert description
        /// </summary>
        alert = 300,
        /// <summary>
        /// Alert Occurences
        /// </summary>
        alertOccurence = 301,
        /// <summary>
        /// Static Nav Session
        /// </summary>
        staticNav = 400
    }
    #endregion

    #region View Ids
    /// <summary>
    /// View Ids
    /// </summary>
    public enum ViewIds {
        /// <summary>
        /// All Media
        /// </summary>
        allMedia=0,
        /// <summary>
        /// All Product
        /// </summary>
        allProduct=1,
		 /// <summary>
        /// All Recap Media
        /// </summary>
        allRecapMedia=2,
        /// <summary>
        /// All Recap Product
        /// </summary>
        allRecapProduct=3,
        /// <summary>
        /// All advertising agency
        /// </summary>
        allAdvAgency=4,
        /// <summary>
        /// All promotion brand
        /// </summary>
        allPromoBrand = 5,
        /// <summary>
        /// All Promo Product
        /// </summary>
        allPromoProduct = 6,
        /// <summary>
        /// All Promo Product
        /// </summary>
        allFacebookProduct = 7
    }
    #endregion

    #endregion

    /// <summary>
    /// Database description
    /// </summary>
    public class DataBase {

        #region Variables
        /// <summary>
        /// Default connections list
        /// </summary>
        private Dictionary<DefaultConnectionIds,DefaultConnection> _defaultConnections;        
        /// <summary>
        /// Customer Connections List
        /// </summary>
        private Dictionary<CustomerConnectionIds,CustomerConnection> _customerConnections;
        /// <summary>
        /// Schemas List
        /// </summary>
        private Dictionary<SchemaIds,Schema> _schemas;
        /// <summary>
        /// Tables List
        /// </summary>
        private Dictionary<TableIds,Table> _tables;
        /// <summary>
        /// Views List
        /// </summary>
        private Dictionary<ViewIds, View> _views;
        /// <summary>
        /// Default Result Table Prefix
        /// </summary>
        private string _defaultResultTablePrefix;
        
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="source">Data source</param>
        public DataBase(IDataSource source) {
            _customerConnections=DataBaseDescriptionXL.LoadCustomerConnections(source);
            _defaultConnections=DataBaseDescriptionXL.LoadDefaultConnections(source);
            _schemas=DataBaseDescriptionXL.LoadSchemas(source);
            _tables=DataBaseDescriptionXL.LoadTables(source,_schemas);
            _views=DataBaseDescriptionXL.LoadViews(source, _schemas);
            _defaultResultTablePrefix=DataBaseDescriptionXL.LoadDefaultResultTablePrefix(source);
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get default result table prefix 
        /// </summary>
        public string DefaultResultTablePrefix {
            get { return (_defaultResultTablePrefix); }
        }
        #endregion

        #region Public Methodes
        
        /// <summary>
        /// Get default database connection
        /// </summary>
        /// <param name="defaultConnectionId">Connection Id</param>
        public IDataSource GetDefaultConnection(DefaultConnectionIds defaultConnectionId) {
            return (_defaultConnections[defaultConnectionId].GetDataSource());
        }
		/// <summary>
		/// Get default database connection
		/// </summary>
		/// <param name="defaultConnectionId">Connection Id</param>
		/// <param name="nlsSort">NLS sort code</param>
		public IDataSource GetDefaultConnection(DefaultConnectionIds defaultConnectionId, string nlsSort) {
			return (_defaultConnections[defaultConnectionId].GetDataSource(nlsSort));
		}
        /// <summary>
        /// Get schema label
        /// </summary>
        /// <param name="schema">Schema Id</param>
        /// <returns>schema label</returns>
        public Schema GetSchema(SchemaIds schemaId) {
            try {
                return (_schemas[schemaId]);
            }
            catch(System.Exception err) {
                throw (new DataBaseException("Impossible to retreive schema label",err));
            }
        }

        /// <summary>
        /// Get customer database connection
        /// </summary>
        /// <param name="login">Customer login</param>
        /// <param name="password">Customer password</param>
        /// <param name="customerConnectionId">Connection Id</param>
        /// <returns>Data Source</returns>
        public IDataSource GetCustomerConnection(string login,string password,CustomerConnectionIds customerConnectionId) {
            if(login==null ||login.Length==0) throw (new ArgumentException("Invalid login parameter"));
            if(password==null ||password.Length==0) throw (new ArgumentException("Invalid password parameter"));
            return(_customerConnections[customerConnectionId].GetDataSource(login,password));
        }
		/// <summary>
		/// Get customer database connection
		/// </summary>
		/// <param name="login">Customer login</param>
		/// <param name="password">Customer password</param>
		/// <param name="customerConnectionId">Connection Id</param>
		/// <param name="isUTf8">True if is utf-8 encoding</param>
		/// <param name="nlsSort">NLS SORT code</param>
		/// <returns>Data Source</returns>
		public IDataSource GetCustomerConnection(string login, string password,string nlsSort,CustomerConnectionIds customerConnectionId) {
			if (login == null || login.Length == 0) throw (new ArgumentException("Invalid login parameter"));
			if (password == null || password.Length == 0) throw (new ArgumentException("Invalid password parameter"));
			return (_customerConnections[customerConnectionId].GetDataSource(login, password, nlsSort));
		}
        /// <summary>
        /// Get customer database connection
        /// </summary>
        /// <param name="login">Customer login</param>
        /// <param name="password">Customer password</param>
        /// <param name="customerConnectionId">Connection Id</param>
        /// <returns>Data Source</returns>
        public IDataSource GetAdExpr03CustomerConnection(string login,string password) {
            return (GetCustomerConnection(login,password,CustomerConnectionIds.adexpr03));
			
        }

        /// <summary>
        /// Get table label with schema label and prefix
        /// Schema.Table prefix
        /// </summary>
        /// <remarks>
        /// A space is put before the string
        /// </remarks>
        /// <example> adexpr03.data_press_4M wp</example>
        /// <param name="tableId">Table Id</param>
        /// <returns>SQL Table code</returns>
        public string GetSqlTableLabelWithPrefix(TableIds tableId) {
            try {
                return (_tables[tableId].SqlWithPrefix);
            }
            catch(System.Exception err) {
                throw (new DataBaseException("Impossible to retreive sql table code",err));
            }
        }

        /// <summary>
        /// Get table object
        /// </summary>
        /// <param name="tableId">Table Id</param>
        /// <returns>Table Object</returns>
        public Table GetTable(TableIds tableId) {
            try {
                Table table=_tables[tableId];
                if(table==null) throw (new DataBaseException("Table Object is null"));
                return (table);
            }
            catch(System.Exception err) {
                throw (new DataBaseException("Impossible to retreive table object",err));
            }
        }

        /// <summary>
        /// Get view label with schema label and prefix
        /// Schema.View prefix
        /// </summary>
        /// <remarks>
        /// A space is put before the string
        /// </remarks>
        /// <example>adexpr03.all_media am</example>
        /// <param name="viewId">View Id</param>
        /// <returns>SQL View code</returns>
        public string GetSqlViewLabelWithPrefix(ViewIds viewId) {
            try {
                return (_views[viewId].SqlWithPrefix);
            }
            catch (System.Exception err) {
                throw (new DataBaseException("Impossible to retreive sql view code", err));
            }
        }

        /// <summary>
        /// Get view object
        /// </summary>
        /// <param name="viewId">View Id</param>
        /// <returns>View Object</returns>
        public View GetView(ViewIds viewId) {
            try {
                View view = _views[viewId];
                if (view == null) throw (new DataBaseException("View Object is null"));
                return (view);
            }
            catch (System.Exception err) {
                throw (new DataBaseException("Impossible to retreive view object", err));
            }
        }
        #endregion
    }

    

    
}
