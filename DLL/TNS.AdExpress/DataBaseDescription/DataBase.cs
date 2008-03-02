#region Information
//  Author : G. Facon, G. Ragneau
//  Creation  date: 29/02/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.DataBaseDescription {

    #region Enums

    #region Database types
    /// <summary>
    /// Database types
    /// </summary>
    public enum DatabaseTypes {
        /// <summary>
        /// Oracle
        /// </summary>
        oracle=0,
        /// <summary>
        /// MySql
        /// </summary>
        mySQL=1,
        /// <summary>
        /// Posgres
        /// </summary>
        postgres=2,
        /// <summary>
        /// SQL Server
        /// </summary>
        sqlServer=3
    } 
    #endregion

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
        listDetail=1,
        /// <summary>
        /// Tracking
        /// </summary>
        tracking=2,
        /// <summary>
        /// Maui01 result table
        /// </summary>
        result=3,
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
        product=8,
        holdingCompany=9,
        advertiser=10,
        brand=11,
        template=12,
        templateAssignment=13,
        mediaType=14,
        productType=15,
        vehicle=16,
        category=17,
        media=18,
        basicMedia=19,
        mediaAgency=20,
        dataMediaAgency=21,
        interestCenter=22,
        mediaSeller=23,
        mediaOrderTemplate=24,
        productOrderTemplate=25,
        gad=26,
        title=27,
        anubisSession=28,
        session=29,
        mySession=30,
        mySessionBackup=31,
        mySessionTest=32,
        customerUniverse=33,
        customerUniverseDescription=34,
        customerUniverseGroup=35,
        monthData=36,
        weekData=37,
        monthAppmData=38,
        weekAppmData=39,
        dataPress=40,
        dataRadio=41,
        dataTv=42,
        dataOutDoor=43,
        dataInternet=44,
        dataAdNetTrack=45,
        dataPressAPPM=46,
        dataSponsorship=47,
        dataMarketingDirect=48,
        dataPressInter=49,
        dataPressAlert=50,
        dataRadioAlert=51,
        dataTvAlert=52,
        dataOutDoorAlert=53,
        dataInternetAlert=54,
        dataPressAPPMAlert=55,
        dataMarketingDirectAlert=56,
        dataPressInterAlert=57,
        color=58,
        format=59,
        mailFormat=60,
        mailType=61,
        mailContent=62,
        dataMailContent=63,
        mailingRapidity=64,
        location=65,
        inset=66,
        insertion=67,
        alarmMedia=68,
        applicationMedia=69,
        recapPluri=70,
        recapPress=71,
        recapTv=72,
        recapRadio=73,
        recapOutDoor=74,
        recapInternet=75,
        recapCinema=76,
        recapTactic=77,
        recapPluriSegment=78,
        recapPressSegment=79,
        recapTvSegment=80,
        recapRadioSegment=81,
        recapOutDoorSegment=82,
        recapInternetSegment=83,
        recapCinemaSegment=84,
        recapTacticSegment=85,
        periodicity=86,
        wave=87,
        target=88,
        targetMediaAssignment=89,
        productAgency=90,
        login=91,
        contact=92,
        contactGroup=93,
        address=94,
        company=95,
        connectionByLogin=96,
        module=97,
        moduleGroup=98,
        trackingTopModule=99,
        trackingTopGad=100,
        trackingTopMediaAgency=101,
        trackingTopExcelExport=102,
        trackingTopOption=103,
        trackingTopUnit=104,
        trackingTopPeriod=105,
        trackingUnit=106,
        trackingPeriod=107,
        trackingTopVehicle=108,
        trackingTopMyAdExpress=109,
        trackingTopVehicleByModule=110,
        trackingLoginIp=111,
        trackingConnectionTime=112,
        agglomeration=113,
        alertPushMail=114,
        alertFlagAssignment=115,
        alertUniverseAssignment=116,
        alertUniverse=117,
        alertUniverseDetail=118,
        country=119,
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
        /// Constructor
        /// </summary>
        /// <param name="defaultConnections">Default connections list</param>
        /// <param name="customerConnections">Customer Connections List</param>
        /// <param name="schemas">Schemas List</param>
        /// <param name="tables">Tables List</param>
        public DataBase(Dictionary<DefaultConnectionIds,DefaultConnection> defaultConnections,
                        Dictionary<CustomerConnectionIds,CustomerConnection> customerConnections,
                        Dictionary<SchemaIds,Schema> schemas,
                        Dictionary<TableIds,Table> tables
            ) {
            if(defaultConnections==null || defaultConnections.Count==0) throw (new ArgumentException("Invalid defaultConnections parameter"));
            if(customerConnections==null || customerConnections.Count==0) throw (new ArgumentException("Invalid customerConnections parameter"));
            if(schemas==null || schemas.Count==0) throw (new ArgumentException("Invalid schemas parameter"));
            if(tables==null || tables.Count==0) throw (new ArgumentException("Invalid tables parameter"));
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
