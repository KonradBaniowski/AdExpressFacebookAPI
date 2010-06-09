using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KMI.WebTeam.Utils.XmlLoader;
using Constantes=KMI.AdExpress.Aphrodite.Constantes;
using AdExpressConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.DB.BusinessFacade.Oracle;
using System.Windows.Threading;
using KMI.AdExpress.Aphrodite;
using TNS.FrameWork.DB.Common.Oracle;
using KMI.AdExpress.Aphrodite.Domain;
using KMI.AdExpress.Aphrodite.Domain.XmlLoaders;
using KMI.WebTeam.Utils.Application;
using TNS.FrameWork.Date;
using System.Threading;

namespace Aphrodite {
    /// <summary>
    /// Main Window
    /// </summary>
    public partial class Main:Window {


        #region Delegates
        /// <summary>
        /// Delegate for listview message
        /// </summary>
        public delegate void MessageCallBack(EventUI eventUI);
        /// <summary>
        /// Delegate for listview error message
        /// </summary>
        public delegate void ErrorMessageCallBack(EventUI eventUI,System.Exception err);

        #endregion


        #region variables
        /// <summary>
        /// Event list
        /// </summary>
        protected ObservableCollectionThreadSafe<EventUI> _eventsList=new ObservableCollectionThreadSafe<EventUI>();
        /// <summary>
        /// Dispacher
        /// </summary>
        protected Dispatcher _dispatcher;
        /// <summary>
        /// Path of the configuration directory
        /// </summary>
        protected string _configurationPathDirecory;
        /// <summary>
        /// Media types description
        /// </summary>
        protected Dictionary<AdExpressConstantes.Vehicles.names,MediaTypeInformation> _mediaTypesList;
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
        public Main() {
            InitializeComponent();
            _eventsListView.DataContext = _eventsList;
            _dispatcher=Application.Current.Dispatcher;

            #region Main directoty initialization
            _configurationPathDirecory=AppDomain.CurrentDomain.BaseDirectory+Constantes.Application.APPLICATION_CONFIGURATION_DIRECTORY+@"\";
            _configurationPathDirecory+=MainApplicationConfigurationPathXL.LoadDirectoryName(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+Constantes.Application.APPLICATION_CONFIGURATION_DIRECTORY+@"\"+Constantes.Application.APPLICATION_CONFIGURATION_FILE))+@"\";
            #endregion

            #region Media type description loading
            _mediaTypesList=MediaTypeInformationXL.Load(new XmlReaderDataSource(_configurationPathDirecory+Constantes.Application.MEDIA_TYPES_CONFIGURATION_FILE));
            #endregion

            #region Database initialisation
            _dataBaseConfiguration=DataBaseConfigurationBussinessFacade.GetOne(_configurationPathDirecory+Constantes.Application.DATABASE_CONFIGURATION_FILE);
            #endregion

            #region Command line
            string[] cmdLines=System.Environment.GetCommandLineArgs();
            string argName;
            string argValue;
            for(int i=1;i<cmdLines.Length;i++){
                argName="";
                argValue="";
                if(cmdLines[i].Trim().IndexOf('-')==0) {
                    argName=cmdLines[i].Trim().Trim('-').Trim();
                    if(argName.Length==0 && i<cmdLines.Length-2) {
                        i++;
                        argName=cmdLines[i].Trim();
                    }
                    if(i<cmdLines.Length-1) {
                        i++;
                        argValue=cmdLines[i].Trim();
                    }
                }
                if(argName.Length>0 && argValue.Length>0) {
                    if(argName.ToUpper().CompareTo("CURRENTMONTH")==0) {
                        _currentDay=DateString.YYYYMMDDToDateTime(argValue+"01");
                    }
                
                }

            }
            #endregion

        }
        #endregion

        #region Event
        /// <summary>
        /// Main window Loaded
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">EventArgs</param>
        private void Window_Loaded(object sender,RoutedEventArgs e) {

            #region Thread
            ComputeData trendsData=new ComputeData(_mediaTypesList,_currentDay,_dataBaseConfiguration);
            trendsData.OnStartWork+=new ComputeData.StartWork(trendsData_OnStartWork);
            trendsData.OnEventMessage+=new ComputeData.EventMessage(trendsData_OnEventMessage);
            trendsData.OnMessageAlert+=new ComputeData.Message(trendsData_OnMessageAlert);
            trendsData.OnStopWorkerJob+=new ComputeData.StopWorkerJob(trendsData_OnStopWorkerJob);
            trendsData.OnError+=new ComputeData.ErrorMessage(trendsData_OnError);
            trendsData.Compute();
            #endregion

        }

        /// <summary>
        /// Closing Application
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event args</param>
        private void Window_Closing(object sender,System.ComponentModel.CancelEventArgs e) {
            //_errorXML.Flush();
            //_errorXML.Close();

        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="eventUI">Event</param>
        /// <param name="err">Exception</param>
        void trendsData_OnError(KMI.WebTeam.Utils.Application.EventUI eventUI,Exception err) {
            ErrorMessageCallBack callBack=Error;

            if(_dispatcher != null && _dispatcher.Thread != Thread.CurrentThread) {
                _dispatcher.Invoke(callBack,new object[] { eventUI,err });
            }
            else {
                Error(eventUI,err);
            }
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="eventUI">Event</param>
        /// <param name="err">Exception</param>
        private void Error(EventUI eventUI,Exception err) {
            eventUI.EvType=EventType.error;
            eventUI.Message+="- error: "+err.Message;
            if(!_eventsList.Contains(eventUI)) _eventsList.Add(eventUI);
            //_eventErrorsList.Add(eventUI);
            //_errorXML.WriteLine("Error:" +eventUI.Message);
            //_errorXML.WriteLine(err.Message);
            //_errorXML.WriteLine(err.Source);
        }

        /// <summary>
        /// Stop job Event
        /// </summary>
        /// <param name="eventUI">Event</param>
        void trendsData_OnStopWorkerJob(KMI.WebTeam.Utils.Application.EventUI eventUI) {
            MessageCallBack callBack=StopWork;

            if(_dispatcher != null && _dispatcher.Thread != Thread.CurrentThread) {
                _dispatcher.Invoke(callBack,new object[] { eventUI });
            }
            else {
                StopWork(eventUI);
            }
        }
        /// <summary>
        /// Stop job
        /// </summary>
        /// <param name="eventUI">Event</param>
        private void StopWork(EventUI eventUI) {
            // Set Done, if in progress
            if(eventUI.EvType==EventType.inProgress) {
                eventUI.EvType=EventType.Done;
            }
            if(!_eventsList.Contains(eventUI))_eventsList.Add(eventUI);
        }

        /// <summary>
        /// Alert Message
        /// </summary>
        /// <param name="message">Message</param>
        void trendsData_OnMessageAlert(string message) {
            MessageCallBack callBack=StopWork;

            if(_dispatcher != null && _dispatcher.Thread != Thread.CurrentThread) {
                _dispatcher.Invoke(callBack,new object[] { message });
            }
            else {
                AlertMessage(message);
            }
            
        }

        /// <summary>
        /// Alert Message
        /// </summary>
        /// <param name="message">Message to show</param>
        private void AlertMessage(string message) {
            writeAlert(message);   
        }

        /// <summary>
        /// Message
        /// </summary>
        /// <param name="eventUI">Event to show</param>
        void trendsData_OnEventMessage(KMI.WebTeam.Utils.Application.EventUI eventUI) {
            MessageCallBack callBack=StopWork;

            if(_dispatcher != null && _dispatcher.Thread != Thread.CurrentThread) {
                _dispatcher.Invoke(callBack,new object[] { eventUI });
            }
            else {
                Message(eventUI);
            }
            
        }

        /// <summary>
        /// Message
        /// </summary>
        /// <param name="eventUI">Event to show</param>
        private void Message(EventUI eventUI){
            _eventsList.Add(eventUI);
        }

        /// <summary>
        /// Start job Event
        /// </summary>
        /// <param name="eventUI">Event</param>
        void trendsData_OnStartWork(KMI.WebTeam.Utils.Application.EventUI eventUI) {
            MessageCallBack callBack=StartWork;

            if(_dispatcher != null && _dispatcher.Thread != Thread.CurrentThread) {
                _dispatcher.Invoke(callBack,new object[] { eventUI });
            }
            else {
                StartWork(eventUI);
            }
        }
        /// <summary>
        /// Start job
        /// </summary>
        /// <param name="eventUI">Event</param>
        private void StartWork(EventUI eventUI) {
            _eventsList.Add(eventUI);
        }

        #endregion

        #region Message Management
        /// <summary>
        /// Write an alert
        /// </summary>
        /// <param name="message">Message</param>
        private void writeAlert(string message) {
            _eventsList.Add(new EventUI(EventType.alert,message));
        }
        /// <summary>
        /// Write an information
        /// </summary>
        /// <param name="message">Message</param>
        private void writeInformation(string message) {
            _eventsList.Add(new EventUI(EventType.information,message));
        }
        /// <summary>
        /// Write an in progress message
        /// </summary>
        /// <param name="message">Message</param>
        private EventUI writeInProgress(string message) {
            EventUI currentEvent=new EventUI(EventType.inProgress,message);
            _eventsList.Add(currentEvent);
            return (currentEvent);
        }
        /// <summary>
        /// Write an error
        /// </summary>
        /// <param name="message">Message</param>
        private void writeError(string message) {
            //_eventErrorsList.Add(new EventUI(EventType.error,message));
            //_errorXML.WriteLine(message);

        }
        #endregion
    }
}
