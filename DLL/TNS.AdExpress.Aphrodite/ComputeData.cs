using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.WebTeam.Utils.Application;
using TNS.FrameWork.DB.Common.Oracle;
using System.Threading;
using AdExpressConstantes=TNS.AdExpress.Constantes.Classification.DB;
using KMI.AdExpress.Aphrodite.Domain;
using TNS.FrameWork.DB.Common;
using KMI.AdExpress.Aphrodite.DAL;

namespace KMI.AdExpress.Aphrodite {
    /// <summary>
    /// Compute Data for Trebds reports
    /// </summary>
    public class ComputeData {
     
        #region Delegate
        /// <summary>
        /// The job has been started
        /// </summary>
        public delegate void StartWork(EventUI eventUI);
        /// <summary>
        /// The job has been stopped
        /// </summary>
        public delegate void StopWorkerJob(EventUI eventUI);
        /// <summary>
        /// Send a message to the MMI for a title
        /// </summary>
        public delegate void EventMessage(EventUI eventUI);
        /// <summary>
        /// Send a message to the MMI
        /// </summary>
        /// <param name="message">Message</param>
        public delegate void Message(string message);
        /// <summary>
        /// Send an error message to the MMI
        /// </summary>
        /// <param name="err">Exception</param>
        public delegate void ErrorMessage(EventUI eventUI,System.Exception err);
        #endregion

        #region Event
        /// <summary>
        /// The job has been started
        /// </summary>
        public event StartWork OnStartWork;
        /// <summary>
        /// The job has been stopped
        /// </summary>
        public event StopWorkerJob OnStopWorkerJob;
        /// <summary>
        /// Send a message to the MMI for a title
        /// </summary>
        public event EventMessage OnEventMessage;
        /// <summary>
        /// Send a message to the MMI
        /// </summary>
        public event Message OnMessageAlert;
        /// <summary>
        /// Send an error message to the MMI
        /// </summary>
        public event ErrorMessage OnError;
        #endregion

        #region Variales
        /// <summary>
        /// Thread
        /// </summary>
        private System.Threading.Thread _myThread;
        /// <summary>
        /// Media types description
        /// </summary>
        Dictionary<AdExpressConstantes.Vehicles.names,MediaTypeInformation> _mediaTypesList;
        /// <summary>
        /// Database configuration
        /// </summary>
        private DataBaseConfiguration _dataBaseConfiguration;
        /// <summary>
        /// Override Date.Now
        /// </summary>
        private DateTime _currentDay=DateTime.Now;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mediaTypesList">Media types description</param>
        /// <param name="currentDay">Day of treatment</param>
        /// <param name="dataBaseConfiguration">Database configuration</param>
        public ComputeData(Dictionary<AdExpressConstantes.Vehicles.names,MediaTypeInformation> mediaTypesList,DateTime currentDay,DataBaseConfiguration dataBaseConfiguration) {
            _mediaTypesList=mediaTypesList;
            _dataBaseConfiguration=dataBaseConfiguration;
            _currentDay=currentDay;
        }
        #endregion

        #region Treatment Thread
        /// <summary>
        /// Compute the treatment for trends report data
        /// </summary>
        public void Compute() {
            _myThread=new Thread(Load);
            _myThread.Name="AdExpress Trends Report data treatment";
            _myThread.Start();
        }
        #endregion

        #region Compute Data
        /// <summary>
        /// Compute data for trends report
        /// </summary>
        private void Load() {
            EventUI ThreadEventUI=new EventUI(EventType.inProgress,"Trends Data Loading Thread");
            OnStartWork(ThreadEventUI);

            try {

                #region Database connection
                IDataSource source=new OracleDataSource(_dataBaseConfiguration.ConnectionString);
                #endregion


                foreach(MediaTypeInformation currentMediaType in _mediaTypesList.Values) {

                    List<string> monthList=Period.DownLoadListMonthPeriod(_currentDay);

                    #region Remove data
                    EventUI RemoveData=new EventUI(EventType.inProgress,"Remove data for "+ currentMediaType.VehicleId.ToString());
                    OnStartWork(RemoveData);
                    try {
                        //TrendsDAL.Remove(currentMediaType,source);
                    }
                    catch(System.Exception err) {
                        OnError(RemoveData,err);
                        throw (err);
                    }
                    OnStopWorkerJob(RemoveData);
                    #endregion

                    #region Load Data
                    if(currentMediaType.MonthTrendsTable.Length>0) {
                        foreach(string currentMonth in monthList) {
                            EventUI computeData=new EventUI(EventType.inProgress,"Compute Month: "+ currentMonth +" for Media type :"+currentMediaType.VehicleId.ToString());
                            OnStartWork(computeData);
                            try {
                                //InsertMonth(currentMonth,currentMediaType,source);
                            }
                            catch(System.Exception err) {
                                OnError(computeData,err);
                                throw (err);
                            }
                            OnStopWorkerJob(computeData);
                        }
                    }
                    #endregion

                    #region Compute SubTotals
                    if(currentMediaType.MonthTrendsTable.Length>0) {
                        foreach(string currentMonth in monthList) {
                            EventUI computeTotalData=new EventUI(EventType.inProgress,"Compute Subtotal - month: "+ currentMonth +" for Media type :"+currentMediaType.VehicleId.ToString());
                            OnStartWork(computeTotalData);
                            try {
                                //TrendsDAL.InsertSubTotal(currentMonth,source,currentMediaType,false,TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_FALSE,TNS.AdExpress.Constantes.DB.Hathor.TYPE_TENDENCY_SUBTOTAL);
                            }
                            catch(System.Exception err) {
                                OnError(computeTotalData,err);
                                throw (err);
                            }
                            OnStopWorkerJob(computeTotalData);
                        }
                    }
                    #endregion

                    #region Compute Totals
                    if(currentMediaType.MonthTrendsTable.Length>0) {
                        foreach(string currentMonth in monthList) {
                            EventUI computeSubTotalData=new EventUI(EventType.inProgress,"Compute Subtotal - month: "+ currentMonth +" for Media type :"+currentMediaType.VehicleId.ToString());
                            OnStartWork(computeSubTotalData);
                            try {
                                //TrendsDAL.InsertSubTotal(currentMonth,source,currentMediaType,true,TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_FALSE,TNS.AdExpress.Constantes.DB.Hathor.TYPE_TENDENCY_TOTAL);
                            }
                            catch(System.Exception err) {
                                OnError(computeSubTotalData,err);
                                throw (err);
                            }
                            OnStopWorkerJob(computeSubTotalData);
                        }
                    }
                    #endregion

                    #region Cumul à date
                    EventUI computeCumulData=new EventUI(EventType.inProgress,"Compute Cumul for Media type :"+currentMediaType.VehicleId.ToString());
                    OnStartWork(computeCumulData);
                    try {
                        //InsertCumul(currentMediaType,_currentDay,source);
                        //TrendsDAL.InsertSubTotal(TNS.AdExpress.Constantes.DB.Hathor.DATE_PERIOD_CUMULATIVE,source,currentMediaType,false,TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_TRUE,TNS.AdExpress.Constantes.DB.Hathor.TYPE_TENDENCY_SUBTOTAL);
                        //TrendsDAL.InsertSubTotal(TNS.AdExpress.Constantes.DB.Hathor.DATE_PERIOD_CUMULATIVE,source,currentMediaType,true,TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_TRUE,TNS.AdExpress.Constantes.DB.Hathor.TYPE_TENDENCY_TOTAL);

                    }
                    catch(System.Exception err) {
                        OnError(computeCumulData,err);
                        throw (err);
                    }
                    OnStopWorkerJob(computeCumulData);
                    #endregion

                    #region Calcul des Pdm
                    EventUI computePdmData=new EventUI(EventType.inProgress,"Compute Pdm for Media type :"+currentMediaType.VehicleId.ToString());
                    OnStartWork(computePdmData);
                    try {
                        // Pdm 
                        //TrendsDAL.InsertPDM(currentMediaType,source,false);
                        // Pdm total
                        //TrendsDAL.InsertPDM(currentMediaType,source,true);
                    }
                    catch(System.Exception err) {
                        OnError(computePdmData,err);
                        throw (err);
                    }
                    OnStopWorkerJob(computePdmData);
                    #endregion

                }
            }
            catch(System.Exception err){
                OnError(ThreadEventUI,err);
                return;
            }
            OnStopWorkerJob(ThreadEventUI);


            
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Compute periods and insert data in table month
        /// </summary>
        /// <param name="period">Month period</param>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="source">Data source</param>
        private void InsertMonth(string period,MediaTypeInformation mediaTypeInformation,IDataSource source) {

            #region Periods
            string year="";
            // Format MM
            string periodId="";
            // Format YYYYMMDD
            string periodBeginning="";
            string periodEnding="";
            string periodBeginningPrev="";
            string periodEndingPrev="";
            string cumul=TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_FALSE;

            //Recherche des différentes date pour les lignes de la table tendency_month;
            year=period.Substring(0,4);
            //Mois
            periodId=period.Substring(4,2);
            periodBeginning=year+periodId+"01";
            DateTime dT=new DateTime(int.Parse(year),int.Parse(periodId),1);
            DateTime dtPrev=new DateTime(int.Parse(year)-1,int.Parse(periodId),1);

            dT=dT.AddMonths(1);
            dT=dT.AddDays(-1);
            periodEnding=year+periodId+dT.Day.ToString();

            periodBeginningPrev=dtPrev.Year.ToString()+periodId+"01";
            dtPrev=dtPrev.AddMonths(1);
            dtPrev=dtPrev.AddDays(-1);

            periodEndingPrev=dtPrev.Year.ToString()+periodId+dtPrev.Day.ToString();
            #endregion	

            //TrendsDAL.InsertMonth(periodBeginning,periodEnding,periodBeginningPrev,periodEndingPrev,periodId,period,year,mediaTypeInformation,cumul,source);
        }



        /// <summary>
        /// Compute periods and insert data in table month
        /// </summary>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="source">Data source</param>
        private void InsertCumul(MediaTypeInformation mediaTypeInformation,DateTime currentDay,IDataSource source) {
            
            string periodBeginning="";
            string periodEnding="";
            string periodBeginningPrev="";
            string periodEndingPrev="";
            string cumul=TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_TRUE;
            string periodId=TNS.AdExpress.Constantes.DB.Hathor.ID_PERIOD_CUMULATIVE;
            string year=TNS.AdExpress.Constantes.DB.Hathor.YEAR_PERIOD_CUMULATIVE;

            DateTime dT=currentDay.AddMonths(-1);
            string studyYear=dT.Year.ToString();

            DateTime LastDate=new DateTime(currentDay.Year,currentDay.Month,1).AddDays(-1);
            periodEnding=LastDate.Year.ToString();
            if(LastDate.Month.ToString().Length<2) periodEnding+="0";
            periodEnding+=LastDate.Month.ToString()+LastDate.Day.ToString();

            DateTime LastDatePreviousPeriod=new DateTime(currentDay.AddYears(-1).Year,currentDay.Month,1).AddDays(-1);
            periodEndingPrev=LastDatePreviousPeriod.Year.ToString();
            if(LastDatePreviousPeriod.Month.ToString().Length<2) periodEndingPrev+="0";
            periodEndingPrev+=LastDatePreviousPeriod.Month.ToString()+LastDatePreviousPeriod.Day.ToString();
            
            periodBeginning=studyYear+"0101";
            string previousStudyYear=dT.AddYears(-1).Year.ToString();
            periodBeginningPrev=previousStudyYear+"0101";

            string period=studyYear;
            if(dT.Month.ToString().Length<2)period+="0";
            period+=dT.Month.ToString();

            //TrendsDAL.InsertMonth(periodBeginning,periodEnding,periodBeginningPrev,periodEndingPrev,periodId,period,year,mediaTypeInformation,cumul,source);
        }
        #endregion
        
    }
}
