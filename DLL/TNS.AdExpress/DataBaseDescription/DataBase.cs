#region Information
//  Author : G. Facon, G. Ragneau
//  Creation  date: 29/02/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.XmlLoader;

namespace TNS.AdExpress.DataBaseDescription {

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
        list=0,
        /// <summary>
        /// List detail
        /// </summary>
        listDetail=1,
        /// <summary>
        /// Tracking
        /// </summary>
        tracking=2,
        /// <summary>
        /// Maui01 result table
        /// </summary>
        rightResult=3,
        /// <summary>
        /// Sector
        /// </summary>
        sector=4,
        /// <summary>
        /// Subsector
        /// </summary>
        subsector=5,
        /// <summary>
        /// Group
        /// </summary>
        group=6,
        /// <summary>
        /// Segment
        /// </summary>
        segment=7,
        /// <summary>
        /// Product
        /// </summary>
        product=8,
        /// <summary>
        /// Holding company
        /// </summary>
        holdingCompany=9,
        /// <summary>
        /// Advertiser
        /// </summary>
        advertiser=10,
        /// <summary>
        /// Brand
        /// </summary>
        brand=11,
        /// <summary>
        /// Right Templates
        /// </summary>
        rightTemplate=12,
        /// <summary>
        /// Right templates assignment
        /// </summary>
        rightTemplateAssignment=13,
        /// <summary>
        /// Media type
        /// </summary>
        rightMediaType=14,
        /// <summary>
        /// ProductType
        /// </summary>
        rightProductType=15,
        /// <summary>
        /// Vehicle
        /// </summary>
        vehicle=16,
        /// <summary>
        /// Category
        /// </summary>
        category=17,
        /// <summary>
        /// Media
        /// </summary>
        media=18,
        /// <summary>
        /// Basic Media
        /// </summary>
        basicMedia=19,
        /// <summary>
        /// Media Agency
        /// </summary>
        mediaAgency=20,
        /// <summary>
        /// Media agency data
        /// </summary>
        dataMediaAgency=21,
        /// <summary>
        /// Interest Center
        /// </summary>
        interestCenter=22,
        /// <summary>
        /// Media seller
        /// </summary>
        mediaSeller=23,
        /// <summary>
        /// Template media right
        /// </summary>
        rightMediaOrderTemplate=24,
        /// <summary>
        /// Template product right
        /// </summary>
        rightProductOrderTemplate=25,
        /// <summary>
        /// Gad
        /// </summary>
        gad=26,
        /// <summary>
        /// Title
        /// </summary>
        title=27,
        /// <summary>
        /// Anubis session (static_nav-session)
        /// </summary>
        anubisSession=28,
        /// <summary>
        /// customer Sessions
        /// </summary>
        customerSession=29,
        /// <summary>
        /// Customer saved sessions
        /// </summary>
        customerSessionSaved=30,
        /// <summary>
        /// Customer saved sessions backup (test)
        /// </summary>
        customerSessionBackup=31,
        /// <summary>
        /// Customer saved sessions test (test)
        /// </summary>
        customerSessionTest=32,
        /// <summary>
        /// Customer universe
        /// </summary>
        customerUniverse=33,
        /// <summary>
        /// Customer universe description
        /// </summary>
        customerUniverseDescription=34,
        /// <summary>
        /// Customer universe groups
        /// </summary>
        customerUniverseGroup=35,
        /// <summary>
        /// Aggregated data per month
        /// </summary>
        monthData=36,
        /// <summary>
        /// Aggregated data per week
        /// </summary>
        weekData=37,
        /// <summary>
        /// APPM aggregated data per month
        /// </summary>
        monthAppmData=38,
        /// <summary>
        /// APPM aggregated data per week
        /// </summary>
        weekAppmData=39,
        /// <summary>
        /// Press Data
        /// </summary>
        dataPress=40,
        /// <summary>
        /// Radio Data
        /// </summary>
        dataRadio=41,
        /// <summary>
        /// Tv Data
        /// </summary>
        dataTv=42,
        /// <summary>
        /// OutDoor Data
        /// </summary>
        dataOutDoor=43,
        /// <summary>
        /// Internet Data
        /// </summary>
        dataInternet=44,
        /// <summary>
        /// AdNetTrack Data
        /// </summary>
        dataAdNetTrack=45,
        /// <summary>
        /// APPM Data
        /// </summary>
        dataPressAPPM=46,
        /// <summary>
        /// Sponsorship Data
        /// </summary>
        dataSponsorship=47,
        /// <summary>
        /// Direct Marketing Data
        /// </summary>
        dataMarketingDirect=48,
        /// <summary>
        /// International Press Data
        /// </summary>
        dataPressInter=49,
        /// <summary>
        /// 4M presse Data
        /// </summary>
        dataPressAlert=50,
        /// <summary>
        /// 4M radio Data
        /// </summary>
        dataRadioAlert=51,
        /// <summary>
        /// 4M tv Data
        /// </summary>
        dataTvAlert=52,
        /// <summary>
        /// 4M outdoor Data
        /// </summary>
        dataOutDoorAlert=53,
        /// <summary>
        /// 4M Internet Data
        /// </summary>
        dataInternetAlert=54,
        /// <summary>
        /// 4M APPM Data
        /// </summary>
        dataPressAPPMAlert=55,
        /// <summary>
        /// 4M direct marketing Data
        /// </summary>
        dataMarketingDirectAlert=56,
        /// <summary>
        /// 4M international press Data
        /// </summary>
        dataPressInterAlert=57,
        /// <summary>
        /// Ad press color list
        /// </summary>
        color=58,
        format=59,
        /// <summary>
        /// Mail format
        /// </summary>
        mailFormat=60,
        /// <summary>
        /// Mail type
        /// </summary>
        mailType=61,
        /// <summary>
        /// Mail content
        /// </summary>
        mailContent=62,
        /// <summary>
        /// Data Mail content
        /// </summary>
        dataMailContent=63,
        /// <summary>
        /// Mail rapidity
        /// </summary>
        mailingRapidity=64,
        /// <summary>
        /// Press Location
        /// </summary>
        location=65,
        /// <summary>
        /// Press Inset
        /// </summary>
        inset=66,
        insertion=67,
        alarmMedia=68,
        applicationMedia=69,
        /// <summary>
        /// Product class analysis plurimedia data
        /// </summary>
        recapPluri=70,
        /// <summary>
        /// Product class analysis press data
        /// </summary>
        recapPress=71,
        /// <summary>
        /// Product class analysis tv data
        /// </summary>
        recapTv=72,
        /// <summary>
        /// Product class analysis radio data
        /// </summary>
        recapRadio=73,
        /// <summary>
        /// Product class analysis outdoor data
        /// </summary>
        recapOutDoor=74,
        /// <summary>
        /// Product class analysis internet data
        /// </summary>
        recapInternet=75,
        /// <summary>
        /// Product class analysis cinema data
        /// </summary>
        recapCinema=76,
        /// <summary>
        /// Product class analysis media tactic data
        /// </summary>
        recapTactic=77,
        /// <summary>
        /// Product class analysis plurimedia data aggregated by segment
        /// </summary>
        recapPluriSegment=78,
        /// <summary>
        /// Product class analysis press data aggregated by segment
        /// </summary>
        recapPressSegment=79,
        /// <summary>
        /// Product class analysis tv data aggregated by segment
        /// </summary>
        recapTvSegment=80,
        /// <summary>
        /// Product class analysis radio data aggregated by segment
        /// </summary>
        recapRadioSegment=81,
        /// <summary>
        /// Product class analysis outdoor data aggregated by segment
        /// </summary>
        recapOutDoorSegment=82,
        /// <summary>
        /// Product class analysis internet data aggregated by segment
        /// </summary>
        recapInternetSegment=83,
        /// <summary>
        /// Product class analysis cinema data aggregated by segment
        /// </summary>
        recapCinemaSegment=84,
        /// <summary>
        /// Product class analysis media tactic data aggregated by segment
        /// </summary>
        recapTacticSegment=85,
        /// <summary>
        /// Media periodicity
        /// </summary>
        periodicity=86,
        /// <summary>
        /// Appm wave list
        /// </summary>
        appmWave=87,
        /// <summary>
        /// Appm target list
        /// </summary>
        appmTarget=88,
        /// <summary>
        /// Appm target assignment
        /// </summary>
        appmTargetMediaAssignment=89,
        /// <summary>
        /// Product per agency
        /// </summary>
        productAgency=90,
        /// <summary>
        /// Customer login
        /// </summary>
        rightLogin=91,
        /// <summary>
        /// Customer contact
        /// </summary>
        rightContact=92,
        /// <summary>
        /// Customer contact group
        /// </summary>
        rightContactGroup=93,
        /// <summary>
        /// Customer Address
        /// </summary>
        rightAddress=94,
        /// <summary>
        /// Customer Company
        /// </summary>
        rightCompany=95,
        /// <summary>
        /// Connection by login tracking
        /// </summary>
        trackingConnectionByLogin=96,
        /// <summary>
        /// AdExpress Modules
        /// </summary>
        rightModule=97,
        /// <summary>
        /// AdExpress Module groups
        /// </summary>
        rightModuleGroup=98,
        /// <summary>
        /// Top module tracking
        /// </summary>
        trackingTopModule=99,
        /// <summary>
        /// Top gad tracking
        /// </summary>
        trackingTopGad=100,
        /// <summary>
        /// Top media tracking
        /// </summary>
        trackingTopMediaAgency=101,
        /// <summary>
        /// Top Excel export tracking
        /// </summary>
        trackingTopExcelExport=102,
        /// <summary>
        /// Top option tracking
        /// </summary>
        trackingTopOption=103,
        /// <summary>
        /// Top unit tracking
        /// </summary>
        trackingTopUnit=104,
        /// <summary>
        /// Top period tracking
        /// </summary>
        trackingTopPeriod=105,
        /// <summary>
        /// Unit list to track
        /// </summary>
        trackingUnit=106,
        /// <summary>
        /// Period list to track
        /// </summary>
        trackingPeriod=107,
        /// <summary>
        /// Top vehicle tracking
        /// </summary>
        trackingTopVehicle=108,
        /// <summary>
        /// Top my AdExpress tracking
        /// </summary>
        trackingTopMyAdExpress=109,
        /// <summary>
        /// Top vehicle by module tracking
        /// </summary>
        trackingTopVehicleByModule=110,
        /// <summary>
        /// Login Ip tracking
        /// </summary>
        trackingLoginIp=111,
        /// <summary>
        /// Connection duration tracking
        /// </summary>
        trackingConnectionTime=112,
        /// <summary>
        /// Agglomeration
        /// </summary>
        agglomeration=113,
        /// <summary>
        /// Push mail alert list
        /// </summary>
        alertPushMail=114,
        /// <summary>
        /// Push mail alert flag assignment
        /// </summary>
        alertFlagAssignment=115,
        /// <summary>
        /// Push mail alert universe assignment
        /// </summary>
        alertUniverseAssignment=116,
        /// <summary>
        /// Push mail alert universe
        /// </summary>
        alertUniverse=117,
        /// <summary>
        /// Push mail alert universe detail
        /// </summary>
        alertUniverseDetail=118,
        /// <summary>
        /// Country
        /// </summary>
        country=119,
        /// <summary>
        /// Advertiser in Product class analysis
        /// </summary>
        recapAdvertiser=120,
        /// <summary>
        /// Brand in Product class analysis
        /// </summary>
        recapBrand=121,
        /// <summary>
        /// Category in Product class analysis
        /// </summary>
        recapCategory=122,
        /// <summary>
        /// Group in Product class analysis
        /// </summary>
        recapGroup=123,
        /// <summary>
        /// Holding company in Product class analysis
        /// </summary>
        recapHoldingCompany=124,
        /// <summary>
        /// Media in Product class analysis
        /// </summary>
        recapMedia=125,
        /// <summary>
        /// Product in Product class analysis
        /// </summary>
        recapProduct=126,
        /// <summary>
        /// Sector in Product class analysis
        /// </summary>
        recapSector=127,
        /// <summary>
        /// Segment in Product class analysis
        /// </summary>
        recapSegment=128,
        /// <summary>
        /// SubSector in Product class analysis
        /// </summary>
        recapSubSector=129,
        /// <summary>
        /// Vehicle in Product class analysis
        /// </summary>
        recapVehicle=130,
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
        
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="source">Data source</param>
        public DataBase(IDataSource source) {
            _customerConnections=DataBaseDescriptionXL.LoadCustomerConnections(source);
            _defaultConnections=DataBaseDescriptionXL.LoadDefaultConnections(source);
            

        }
        #endregion

        #region Accessors

        #endregion

        #region Public Methodes
        /// <summary>
        /// Get default database connection
        /// </summary>
        /// <param name="defaultConnectionId">Connection Id</param>
        public IDataSource GetDefaultConnection(DefaultConnectionIds defaultConnectionId) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get customer database connection
        /// </summary>
        /// <param name="login">Customer login</param>
        /// <param name="password">Customer password</param>
        /// <param name="customerConnectionId">Connection Id</param>
        /// <returns></returns>
        public IDataSource GetCustomerConnection(string login,string password,CustomerConnectionIds customerConnectionId) {
            if(login==null ||login.Length==0) throw (new ArgumentException("Invalid login parameter"));
            if(password==null ||password.Length==0) throw (new ArgumentException("Invalid password parameter"));
            throw new System.NotImplementedException();
        }
        #endregion
    }

    

    
}
