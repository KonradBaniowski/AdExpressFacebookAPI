using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.DataBaseDescription {

    #region Enums
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
    public enum CustomerConnectionIds {
        /// <summary>
        /// AdExpress Data
        /// </summary>
        adexpr03=0,
    }

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

    public enum SchemaIds {
    }

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


    public class DataBase {

        #region Variables

        /// <summary>
        /// Connection List
        /// </summary>
        private Dictionary<CustomerConnectionIds,CustomerConnection> _customerConnections;
        /// <summary>
        /// Schema List
        /// </summary>
        private Dictionary<SchemaIds,Schema> _schemas;
        /// <summary>
        /// Tables List
        /// </summary>
        private Dictionary<TableIds,Table> _tables;
        #endregion
        private System.Collections.Generic.Dictionary<TNS.AdExpress.DataBaseDescription.CustomerConnectionIds,TNS.AdExpress.DataBaseDescription.DefaultConnection> _defaultConnections;

        /// <param name="databaseType">Database type</param>
        /// <param name="source">Data source</param>
        public DataBase(IDataSource source) {
            throw new System.NotImplementedException();
        }

        #region Accessors

        #endregion

        #region Public Methodes
        /// <summary>
        /// Get Database Connection
        /// </summary>
        /// <param name="connectionId">Connection Id</param>
        public IDataSource GetConnection(CustomerConnectionIds connectionId) {
            throw new System.NotImplementedException();
        }
        #endregion
    }

    

    
}
