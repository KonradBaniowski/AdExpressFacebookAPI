
#region using
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Web;
using System.Globalization;

using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using CstFrameWorkResult = TNS.AdExpress.Constantes.FrameWork.Results;

using FctWeb = TNS.AdExpress.Web.Functions;
using FctExcel = TNS.AdExpress.Web.UI.ExcelWebPage;

using TNS.AdExpressI.MediaSchedule.Exceptions;
using TNS.AdExpressI.MediaSchedule.Style;

using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;

using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Constantes.FrameWork;

using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Results;

using TNS.AdExpressI.MediaSchedule.DAL;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;

using Aspose.Cells;
using TNS.AdExpressI.MediaSchedule.Functions;
#endregion

namespace TNS.AdExpressI.MediaSchedule
{
    /// <summary>
    /// Abstract IMediaScheduleResults implementation
    /// </summary>
    public abstract class MediaScheduleResults : IMediaScheduleResults
    {

        #region Constantes

        #region Line Constants
        /// <summary>
        /// Index of line "Total"
        /// </summary>
        public const int TOTAL_LINE_INDEX = 1;
        #endregion

        #region Column Indexes
        /// <summary>
        /// Index of N1 label
        /// </summary>
        public const int L1_COLUMN_INDEX = 0;
        /// <summary>
        /// Index of N2 label
        /// </summary>
        public const int L2_COLUMN_INDEX = 1;
        /// <summary>
        /// Index of N3 label
        /// </summary>
        public const int L3_COLUMN_INDEX = 2;
        /// <summary>
        /// Index of N4 label
        /// </summary>
        public const int L4_COLUMN_INDEX = 3;
        /// <summary>
        /// Index of periodicity column
        /// </summary>
        public const int PERIODICITY_COLUMN_INDEX = 4;
        /// <summary>
        /// Index of total column
        /// </summary>
        public const int TOTAL_COLUMN_INDEX = 5;
        /// <summary>
        /// Index de la colonne des pdms dans le tableau en m�moire
        /// </summary>
        public const int PDM_COLUMN_INDEX = 6;
        /// <summary>
        /// Index de la colonne du niveau 1
        /// </summary>
        public const int L1_ID_COLUMN_INDEX = 7;
        /// <summary>
        /// Index de la colonne du niveau 2
        /// </summary>
        public const int L2_ID_COLUMN_INDEX = 8;
        /// <summary>
        /// Index de la colonne du niveau 3
        /// </summary>
        public const int L3_ID_COLUMN_INDEX = 9;
        /// <summary>
        /// Index de la colonne du niveau 4
        /// </summary>
        public const int L4_ID_COLUMN_INDEX = 10;
        /// <summary>
        /// Index of the first column containing periods
        /// </summary>
        public const int FIRST_PERIOD_INDEX = 11;
        #endregion

        /// <summary>
		/// Total line label
		/// </summary>
		public const string TOTAL_STRING="TOTAL MEDIA";
        /// <summary>
        /// name of EXCEL PATTERN PERCENTAGE
        /// </summary>
        public const string EXCEL_PATTERN_NAME_PERCENTAGE = "percentage";
        
        #endregion

        #region Properties

        #region Zoom
        /// <summary>
        /// Zoom of the report
        /// </summary>
        protected string _zoom = string.Empty;
        /// <summary>
        /// User Session
        /// </summary>
        public string Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }
        #endregion

        #region Session
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// User Session
        /// </summary>
        public WebSession Session
        {
            get{ return _session;}
            set { _session = value; }
        }
        #endregion

        #region Module
        /// <summary>
        /// Current module
        /// </summary>
        protected TNS.AdExpress.Domain.Web.Navigation.Module _module = ModulesList.GetModule(CstWeb.Module.Name.ANALYSE_PLAN_MEDIA);
        /// <summary>
        /// User Session
        /// </summary>
        public TNS.AdExpress.Domain.Web.Navigation.Module Module
        {
            get{ return _module;}
            set { _module = value; }
        }
        #endregion

        #region Period
        /// <summary>
        /// Report period filter
        /// </summary>
        protected TNS.AdExpress.Web.Core.Selection.MediaSchedulePeriod _period;
        /// <summary>
        /// Report period
        /// </summary>
        public MediaSchedulePeriod Period
        {
            get{ return _period;}
            set { _period = value; }
        }
        #endregion

        #region Vehicle Ids
        /// <summary>
        /// List of selected vehicles
        /// </summary>
        protected List<VehicleInformation> _vehicles;
        /// <summary>
        /// List of selected vehicles
        /// </summary>
        protected List<VehicleInformation> Vehicles
        {
            get { 
                if(_vehicles == null){
                    _vehicles = GetVehicles();
                }
                return _vehicles;
            }
            set { _vehicles = value; }
        }
        /// <summary>
        /// Vehicle Id filter
        /// </summary>
        protected Int64 _vehicleId;
        /// <summary>
        /// Get / Set Vehicle Id filter
        /// </summary>
        public Int64 VehicleId
        {
            get{ return _vehicleId;}
            set { 
                _vehicleId = value;
                _vehicles = GetVehicles();
            }
        }
        #endregion

        #region ShowTotal
        /// <summary>
        /// Is acces to total?
        /// </summary>
        protected bool _allowTotal = true;
        /// <summary>
        /// Get / Set autorisation to access to total
        /// </summary>
        public bool AllowTotal
        {
            get { return _allowTotal; }
            set { _allowTotal = value; }
        }
        #endregion

        #region ShowPDM
        /// <summary>
        /// Is acces to PDM values
        /// </summary>
        protected bool _allowPdm = true;
        /// <summary>
        /// Get / Set autorisation to access PDM values
        /// </summary>
        public bool AllowPdm
        {
            get { return _allowPdm; }
            set { _allowPdm = value; }
        }
        #endregion

        #region ShowVersions
        /// <summary>
        /// Is acces to versions allowed?
        /// </summary>
        protected bool _allowVersion = false;
        /// <summary>
        /// Get / Set autorisation to access to versions
        /// </summary>
        public bool AllowVersion{
            get { return _allowVersion; }
            set { _allowVersion = value; }
        }
        #endregion

        #region ShowInsertions
        /// <summary>
        /// Is acces to insertions allowed?
        /// </summary>
        protected bool _allowInsertions = false;
        /// <summary>
        /// Get / Set autorisation to access to insertions
        /// </summary>
        public bool AllowInsertion
        {
            get { return _allowInsertions; }
            set { _allowInsertions = value; }
        }
        #endregion

        #region ShowValues
        /// <summary>
        /// Include values in cells
        /// </summary>
        protected bool _showValues = false;
        /// <summary>
        /// Get / Set flag to specify if report must whow all values
        /// </summary>
        public bool ShwoValues
        {
            get { return _showValues; }
            set { _showValues = value; }
        }
        #endregion

        #region Is Creative Division MS
        /// <summary>
        /// Is it a media schedule for creative division?
        /// </summary>
        protected bool _isCreativeDivisionMS = false;
        /// <summary>
        /// Get / Set flag to specify if report is a Media Schedule to Creative Division
        /// </summary>
        public bool IsCreativeDivisionMS
        {
            get { return _isCreativeDivisionMS; }
            set { _isCreativeDivisionMS = value; }
        }
        #endregion

        #region Is Excel Report
        /// <summary>
        /// Is it a excel report?
        /// </summary>
        protected bool _isExcelReport = false;
        /// <summary>
        /// Get / Set flag to specify if report is an Excel Media Schedule
        /// </summary>
        public bool IsExcelReport
        {
            get { return _isExcelReport; }
            set { _isExcelReport = value; }
        }
        #endregion

        #region Is PDF Report
        /// <summary>
        /// Is it a pdf report
        /// </summary>
        protected bool _isPDFReport = false;
        /// <summary>
        /// Get / Set flag to specify if Media Schedule output is PDF
        /// </summary>
        public bool IsPDFReport
        {
            get { return _isPDFReport; }
            set { _isPDFReport = value; }
        }
        #endregion

        #region Media Schedule Style
        /// <summary>
        /// MediaSchedule Style
        /// </summary>
        protected MediaScheduleStyle _style = null;
        /// <summary>
        /// Get / Set flag to specify if Media Schedule output is PDF
        /// </summary>
        public MediaScheduleStyle MediaScheduleStyle
        {
            get { return _style; }
            set { _style = value; }
        }

        
        #endregion

        #region Media Schedule Sheet Style
        /// <summary>
        /// MediaSchedule Sheet Style
        /// </summary>
        protected MediaScheduleSheetStyle _styleSheet = null;
        /// <summary>
        /// Get / Set flag to specify if Media Schedule output is PDF
        /// </summary>
        public MediaScheduleSheetStyle MediaScheduleSheetStyle
        {
            get { return _styleSheet; }
            set { _styleSheet = value; }
        }

        
        #endregion

        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period)
        {
            _session = session;
            _period = period;
            _allowInsertions = AllowInsertions();
            _allowVersion = AllowVersions();
        }
        /// <summary>
        /// Constructor of a Media Schedule on a specifi vehicle
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        /// <param name="idVehicle">Vehicle Filter</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period, Int64 idVehicle):this(session, period)
        {
            this._vehicleId = idVehicle;
        }
        /// <summary>
        /// Constructor of a Media Schedule on a zoomed period
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        /// <param name="zoom">Report zoom</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period, string zoom):this(session, period)
        {
            this._zoom = zoom ;
        }
        /// <summary>
        /// Constructor of a Media Schedule on a specifi vehicle and a zoomed period
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        /// <param name="idVehicle">Vehicle Id</param>
        /// <param name="zoom">Report zoom</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period, Int64 idVehicle, string zoom):this(session, period)
        {
            this._vehicleId = idVehicle;
            this._zoom = zoom ;
        }
        #endregion

        #region IMediaScheduleResults Membres
        /// <summary>
        /// Get HTML code for the media schedule
        /// </summary>
        /// <returns>HTML Code</returns>
        public virtual MediaScheduleData GetHtml()
        {
            _isCreativeDivisionMS = false;
            _showValues = false;
            _isExcelReport = false;
            _isPDFReport = false;
            _allowInsertions = AllowInsertions();
            _allowVersion = AllowVersions();
            _allowTotal = _allowPdm = ((!VehiclesInformation.Contains(_vehicleId) || (VehiclesInformation.Contains(_vehicleId) && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.adnettrack)) && _module.Id != TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE);
            _style = new DefaultMediaScheduleStyle();
            return ComputeDesign(ComputeData());
        }

        /// <summary>
        /// Get HTML code for the media schedule dedicated to Creative Division
        /// </summary>
        /// <returns>HTML Code</returns>
        public virtual MediaScheduleData GetHtmlCreativeDivision()
        {
            _isCreativeDivisionMS = true;
            _showValues = false;
            _isExcelReport = false;
            _isPDFReport = false;
            _allowInsertions = AllowInsertions();
            _allowVersion = AllowVersions();
            _allowTotal = _allowPdm = (!VehiclesInformation.Contains(_vehicleId) || (VehiclesInformation.Contains(_vehicleId) && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.adnettrack));
            _style = new DefaultMediaScheduleStyle();
            return ComputeDesign(ComputeData());

        }

        /// <summary>
        /// Get HTML code for a pdf export of the media schedule
        /// </summary>
        /// <returns>HTML Code</returns>
        public virtual MediaScheduleData GetPDFHtml()
        {
            _isCreativeDivisionMS = false;
            _showValues = false;
            _isExcelReport = false;
            _isPDFReport = true;
            _allowInsertions = AllowInsertions();
            _allowVersion = AllowVersions();
            _allowTotal = _allowPdm = (!VehiclesInformation.Contains(_vehicleId) || (VehiclesInformation.Contains(_vehicleId) && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.adnettrack));
            _style = new PDFMediaScheduleStyle();
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Get HTML code for an excel export of the media schedule
        /// </summary>
        /// <param name="withValues">Specify if each values of the calendar must be shown in Media Schedule</param>
        /// <returns>HTML Code</returns>
        public virtual MediaScheduleData GetExcelHtml(bool withValues)
        {
            _isCreativeDivisionMS = false;
            _showValues = withValues;
            _isExcelReport = true;
            _isPDFReport = false;
            _allowInsertions = AllowInsertions();
            _allowVersion = AllowVersions();
            _allowTotal = _allowPdm = (!VehiclesInformation.Contains(_vehicleId) || (VehiclesInformation.Contains(_vehicleId) && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.adnettrack)) && _module.Id != TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE; 
            _style = new ExcelMediaScheduleStyle();
            return ComputeDesign(ComputeData());
        }
        /// <summary>
        /// Get HTML code for an excel export of the media schedule dedicated to creative division
        /// </summary>
        /// <param name="withValues">Specify if each values of the calendar must be shown in Media Schedule</param>
        /// <returns>HTML Code</returns>
        public virtual MediaScheduleData GetExcelHtmlCreativeDivision(bool withValues)
        {
            _isCreativeDivisionMS = true;
            _showValues = withValues;
            _isExcelReport = true;
            _isPDFReport = false;
            _allowInsertions = AllowInsertions();
            _allowVersion = AllowVersions();
            _allowTotal = _allowPdm = (!VehiclesInformation.Contains(_vehicleId) || (VehiclesInformation.Contains(_vehicleId) && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.adnettrack));
            _style = new ExcelMediaScheduleStyle();
            return ComputeDesign(ComputeData());
        }

        /// <summary>
        /// Get Excel for an excel export by Anubis of the media schedule
        /// </summary>
        public virtual void GetRawData(Workbook excel) {
            _isCreativeDivisionMS = false;
            _showValues = false;
            _allowTotal = false; 
            _allowPdm = false;
            _styleSheet = new ExcelSheetMediaScheduleStyle(excel);
            ComputeDesignExcel(ComputeData(), excel);
        }

        #endregion

        #region Protected Methods

        #region Compute Data
        /// <summary>
        /// Compute data from database
        /// </summary>
        /// <returns>Formatted table ready for UI design</returns>
        protected virtual object[,] ComputeData()
        {
            object[,] oTab = null;

            GenericDetailLevel detailLevel;

            #region Data
            DataSet ds = null;
            DataTable dt = null;
            //ds = GenericMediaScheduleDataAccess.GetAdNetTrackData(_session, _period);
            //ds = GenericMediaScheduleDataAccess.GetData(_session, _period);
            object[] param = null;
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("Data access layer is null for the Media Schedule result"));
            param = new object[2];
            param[0] = _session;
            param[1] = _period;
            IMediaScheduleResultDAL mediaScheduleDAL = (IMediaScheduleResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);

            if (VehiclesInformation.Contains(_vehicleId) && VehiclesInformation.DatabaseIdToEnum(_vehicleId) == CstDBClassif.Vehicles.names.adnettrack)
            {
                detailLevel = _session.GenericAdNetTrackDetailLevel;
                ds = mediaScheduleDAL.GetMediaScheduleAdNetTrackData();
            }
            else
            {
                detailLevel = _session.GenericMediaDetailLevel;                
                ds = mediaScheduleDAL.GetMediaScheduleData();
            }

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0] == null)
            {
                return (new object[0, 0]);
            }
            dt = ds.Tables[0];
            #endregion

            #region Variables
            //DataTable dt = ds.Tables[0];
            //int nbL1 = 0;
            //int nbL2 = 0;
            //int nbL3 = 0;
            //int nbL4 = 0;
            //Int64 oldIdL1 = -1;
            //Int64 oldIdL2 = -1;
            //Int64 oldIdL3 = -1;
            //Int64 oldIdL4 = -1;
            //Int64 currentLineIndex = 1;
            //Int64 currentTotalIndex = 1;
            //Int64 currentL1Index = 2;
            //Int64 currentL2Index = 0;
            //Int64 currentL3Index = 1;
            //Int64 currentL4Index = 1;
            //Int64 currentL3PDMIndex = 0;
            //Int64 currentL2PDMIndex = 0;
            //Int64 currentL1PDMIndex = 0;
            //bool forceEntry = true;
            //AtomicPeriodWeek weekDate = null;
            //double unit = 0.0;
            //int currentDate = 0;
            //int oldCurrentDate = 0;
            //Int64 i;
            //int numberOflineToAdd = 0;
            //int nbCol = 0;
            //int nbline = 0;
            //int k, mpi, nbDays, nbMonth = 0;
            //bool forceL2 = false;
            //bool forceL3 = false;
            //bool forceL4 = false;
            #endregion

            #region Count nb of elements for each classification level
            int nbLevels = detailLevel.GetNbLevels;
            Int64 oldIdL1 = -1;
            Int64 oldIdL2 = -1;
            Int64 oldIdL3 = -1;
            Int64 oldIdL4 = -1;
            int nbL1 = 0;
            int nbL2 = 0;
            int nbL3 = 0;
            int nbL4 = 0;
            bool newL2 = false;
            bool newL3 = false;
            bool newL4 = false;
            foreach (DataRow currentRow in dt.Rows)
            {
                if (nbLevels >= 1 && oldIdL1 != GetLevelId(currentRow, 1, detailLevel))
                {
                    newL2 = true;
                    nbL1++;
                    oldIdL1 = GetLevelId(currentRow, 1, detailLevel);
                }
                if (nbLevels >= 2 && (oldIdL2 != GetLevelId(currentRow, 2, detailLevel) || newL2))
                {
                    newL3 = true;
                    newL2 = false;
                    nbL2++;
                    oldIdL2 = GetLevelId(currentRow, 2, detailLevel);
                }
                if (nbLevels >= 3 && (oldIdL3 != GetLevelId(currentRow, 3, detailLevel) || newL3))
                {
                    newL4 = true;
                    newL3 = false;
                    nbL3++;
                    oldIdL3 = GetLevelId(currentRow, 3, detailLevel);
                }
                if (nbLevels >= 4 && (oldIdL4 != GetLevelId(currentRow, 4, detailLevel) || newL4))
                {
                    newL4 = false;
                    nbL4++;
                    oldIdL4 = GetLevelId(currentRow, 4, detailLevel);
                }
            }
            newL2 = newL3 = newL4 = false;
            oldIdL1 = oldIdL2 = oldIdL3 = oldIdL4 = -1;
            #endregion

            //No Data
            if (nbL1 == 0)
            {
                return (new object[0, 0]);
            }

            #region Create periods table
            List<Int64> periodItemsList = new List<Int64>();
            Dictionary<int, int> years_index = new Dictionary<int, int>();
            int currentDate = _period.Begin.Year;
            int oldCurrentDate = _period.End.Year;
            int firstPeriodIndex = FIRST_PERIOD_INDEX;

            switch (_period.PeriodDetailLEvel)
            {
                case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:
                    AtomicPeriodWeek currentWeek = new AtomicPeriodWeek(_period.Begin);
                    AtomicPeriodWeek endWeek = new AtomicPeriodWeek(_period.End);
                    currentDate = currentWeek.Year;
                    oldCurrentDate = endWeek.Year;
                    endWeek.Increment();
                    while (!(currentWeek.Week == endWeek.Week && currentWeek.Year == endWeek.Year))
                    {
                        periodItemsList.Add(currentWeek.Year*100+currentWeek.Week);
                        currentWeek.Increment();
                    }
                    break;
                case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:
                    DateTime dateCurrent = _period.Begin;
                    DateTime dateEnd = _period.End;
                    dateEnd = dateEnd.AddMonths(1);
                    while (!(dateCurrent.Month == dateEnd.Month && dateCurrent.Year == dateEnd.Year))
                    {
                        periodItemsList.Add(Int64.Parse(dateCurrent.ToString("yyyyMM")));
                        dateCurrent = dateCurrent.AddMonths(1);
                    }
                    break;
                case CstWeb.CustomerSessions.Period.DisplayLevel.dayly:
                    DateTime currentDateTime = _period.Begin;
                    DateTime endDate = _period.End;
                    while (currentDateTime <= endDate)
                    {
                        periodItemsList.Add(Int64.Parse(DateString.DateTimeToYYYYMMDD(currentDateTime)));
                        currentDateTime = currentDateTime.AddDays(1);
                    }
                    break;
                default:
                    throw (new MediaScheduleException("Unable to build periods table."));
            }
            if (currentDate != oldCurrentDate) {
                for (int k = currentDate; k <= oldCurrentDate; k++) {
                    years_index.Add(k, firstPeriodIndex);
                    firstPeriodIndex++;
                }
            }
            currentDate = 0;
            oldCurrentDate = 0;
            #endregion

            #region Indexes tables
            // Column number
            int nbCol = periodItemsList.Count + FIRST_PERIOD_INDEX + years_index.Count;
            // Line number
            int nbline = nbL1 + nbL2 + nbL3 + nbL4 + 3 + 1;
            // Result table
            oTab = new object[nbline, nbCol];
            // L3 elements indexes
            Int64[] tabL3Index = new Int64[nbL3 + 1];
            // L2 elements indexes
            Int64[] tabL2Index = new Int64[nbL2 + 1];
            // L1 elements indexes
            Int64[] tabL1Index = new Int64[nbL1 + 1];
            #endregion

            #region Column Labels
            currentDate = 0;
            while (currentDate < periodItemsList.Count)
            {
                oTab[0, currentDate + firstPeriodIndex] = periodItemsList[currentDate].ToString(); ;
                currentDate++;
            }
            foreach (int i in years_index.Keys)
            {
                oTab[0, years_index[i]] = i;
            }
            #endregion

            #region Init totals
            Int64 currentTotalIndex = 1;
            Int64 currentLineIndex = 1;
            oTab[currentTotalIndex, 0] = TOTAL_STRING;
            oTab[currentTotalIndex, TOTAL_COLUMN_INDEX] = (double)0.0;
            foreach (int i in years_index.Keys)
            {
                oTab[currentLineIndex, int.Parse(years_index[i].ToString())] = (double)0.0;
            }
            #endregion

            #region Build result table

            #region Create MediaPlanItem for total
            for (int mpi = firstPeriodIndex; mpi < nbCol; mpi++)
            {
                oTab[TOTAL_LINE_INDEX, mpi] = new MediaPlanItem(-1);
            }
            #endregion

            Int64 currentL3PDMIndex = 0;
            Int64 currentL2PDMIndex = 0;
            Int64 currentL1PDMIndex = 0;
            Int64 currentL1Index = 2;
            Int64 currentL2Index = 0;
            Int64 currentL3Index = 1;
            Int64 currentL4Index = 1;
            int numberOflineToAdd = 0;
            double unit = 0.0;

            try
            {

                foreach (DataRow currentRow in dt.Rows)
                {

                    #region New L1
                    if (nbLevels >= 1 && oldIdL1 != GetLevelId(currentRow, 1, detailLevel))
                    {
                        // Next L2 is new
                        newL2 = true;
                        // PDM
                        if (oldIdL1 != -1)
                        {
                            for (int i = 0; i < currentL2PDMIndex; i++)
                            {
                                if ((double)oTab[currentL1Index, TOTAL_COLUMN_INDEX] != 0)
                                    oTab[tabL2Index[i], PDM_COLUMN_INDEX] = (double)oTab[tabL2Index[i], TOTAL_COLUMN_INDEX] / (double)oTab[currentL1Index, TOTAL_COLUMN_INDEX] * 100.0;
                                else
                                    oTab[tabL2Index[i], PDM_COLUMN_INDEX] = 0.0;
                            }
                            tabL2Index = new Int64[nbL2 + 1];
                            currentL2PDMIndex = 0;
                        }
                        // L1 PDMs
                        tabL1Index[currentL1PDMIndex] = currentLineIndex + 1;
                        currentL1PDMIndex++;

                        currentLineIndex++;
                        oTab[currentLineIndex, L1_COLUMN_INDEX] = GetLevelLabel(currentRow, 1, detailLevel);
                        oTab[currentLineIndex, L1_ID_COLUMN_INDEX] = GetLevelId(currentRow, 1, detailLevel);
                        oTab[currentLineIndex, TOTAL_COLUMN_INDEX] = (double)0.0;
                        if (nbLevels <= 1) oTab[currentLineIndex, PERIODICITY_COLUMN_INDEX] = currentRow["period_count"].ToString();
                        //Init years totals
                        foreach (int i in years_index.Keys)
                        {
                            oTab[currentLineIndex, years_index[i]] = (double)0.0;
                        }
                        currentL1Index = currentLineIndex;
                        oldIdL1 = GetLevelId(currentRow, 1, detailLevel);
                        currentDate = 0;
                        numberOflineToAdd++;
                        oTab[currentLineIndex, L2_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L2_ID_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L3_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L3_ID_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L4_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L4_ID_COLUMN_INDEX] = null;
                        // Create MediaPlan Items
                        for (int mpi = firstPeriodIndex; mpi < nbCol; mpi++)
                        {
                            oTab[currentLineIndex, mpi] = new MediaPlanItem(-1);
                        }

                    }
                    #endregion

                    #region New L2
                    if (nbLevels >= 2 && (oldIdL2 != GetLevelId(currentRow, 2, detailLevel) || newL2))
                    {
                        // Next L3 is new
                        newL3 = true;
                        newL2 = false;
                        //Level 3 PDMs
                        if (oldIdL2 != -1)
                        {
                            for (int i = 0; i < currentL3PDMIndex; i++)
                            {
                                if ((double)oTab[currentL2Index, TOTAL_COLUMN_INDEX] != 0)
                                    oTab[tabL3Index[i], PDM_COLUMN_INDEX] = (double)oTab[tabL3Index[i], TOTAL_COLUMN_INDEX] / (double)oTab[currentL2Index, TOTAL_COLUMN_INDEX] * 100.0;
                                else
                                    oTab[tabL3Index[i], PDM_COLUMN_INDEX] = 0.0;
                            }
                            tabL3Index = new Int64[nbL3 + 1];
                            currentL3PDMIndex = 0;

                        }
                        // Prepare L2 PDMs
                        tabL2Index[currentL2PDMIndex] = currentLineIndex + 1;
                        currentL2PDMIndex++;

                        currentLineIndex++;
                        oTab[currentLineIndex, L2_COLUMN_INDEX] = GetLevelLabel(currentRow, 2, detailLevel);
                        oTab[currentLineIndex, L2_ID_COLUMN_INDEX] = GetLevelId(currentRow, 2, detailLevel);
                        oTab[currentLineIndex, TOTAL_COLUMN_INDEX] = (double)0.0;
                        oTab[currentLineIndex, L1_ID_COLUMN_INDEX] = oldIdL1;
                        if (nbLevels <= 2) oTab[currentLineIndex, PERIODICITY_COLUMN_INDEX] = currentRow["period_count"].ToString();
                        //Init years totals
                        foreach (int i in years_index.Keys)
                        {
                            oTab[currentLineIndex, years_index[i]] = (double)0.0;
                        }
                        currentL2Index = currentLineIndex;
                        oldIdL2 = GetLevelId(currentRow, 2, detailLevel);
                        currentDate = 0;
                        numberOflineToAdd++;
                        oTab[currentLineIndex, L1_COLUMN_INDEX] = null;

                        oTab[currentLineIndex, L3_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L3_ID_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L4_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L4_ID_COLUMN_INDEX] = null;
                        // Cr�ation des MediaPlanItem
                        for (int mpi = firstPeriodIndex; mpi < nbCol; mpi++)
                        {
                            oTab[currentLineIndex, mpi] = new MediaPlanItem(-1);
                        }
                    }
                    #endregion

                    #region New L3
                    if (nbLevels >= 3 && (oldIdL3 != GetLevelId(currentRow, 3, detailLevel) || newL3))
                    {
                        // Next L4 is different
                        newL4 = true;
                        newL3 = false;
                        //  L4 PDMs
                        if (oldIdL3 != -1)
                        {
                            for (Int64 i = currentL3Index + 1; i <= currentLineIndex; i++)
                            {
                                if ((double)oTab[currentL3Index, TOTAL_COLUMN_INDEX] != 0)
                                    oTab[i, PDM_COLUMN_INDEX] = (double)oTab[i, TOTAL_COLUMN_INDEX] / (double)oTab[currentL3Index, TOTAL_COLUMN_INDEX] * 100.0;
                                else
                                    oTab[i, PDM_COLUMN_INDEX] = 0.0;
                            }
                        }
                        // Pr�paration des PDM des L3
                        tabL3Index[currentL3PDMIndex] = currentLineIndex + 1;
                        currentL3PDMIndex++;

                        currentLineIndex++;
                        oTab[currentLineIndex, L3_COLUMN_INDEX] = GetLevelLabel(currentRow, 3, detailLevel);
                        oTab[currentLineIndex, L3_ID_COLUMN_INDEX] = GetLevelId(currentRow, 3, detailLevel);
                        oTab[currentLineIndex, TOTAL_COLUMN_INDEX] = (double)0.0;
                        oTab[currentLineIndex, L1_ID_COLUMN_INDEX] = oldIdL1;
                        oTab[currentLineIndex, L2_ID_COLUMN_INDEX] = oldIdL2;
                        if (nbLevels <= 3) oTab[currentLineIndex, PERIODICITY_COLUMN_INDEX] = currentRow["period_count"].ToString();
                        //Init totals
                        foreach (int i in years_index.Keys)
                        {
                            oTab[currentLineIndex, years_index[i]] = (double)0.0;
                        }
                        currentL3Index = currentLineIndex;
                        oldIdL3 = GetLevelId(currentRow, 3, detailLevel);
                        currentDate = 0;
                        numberOflineToAdd++;
                        oTab[currentLineIndex, L1_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L2_COLUMN_INDEX] = null;

                        oTab[currentLineIndex, L4_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L4_ID_COLUMN_INDEX] = null;
                        // Cr�ation des MediaPlanItem
                        for (int mpi = firstPeriodIndex; mpi < nbCol; mpi++)
                        {
                            oTab[currentLineIndex, mpi] = new MediaPlanItem(-1);
                        }
                    }
                    #endregion

                    #region New L4
                    if (nbLevels >= 4 && (oldIdL4 != GetLevelId(currentRow, 4, detailLevel) || newL4))
                    {
                        newL4 = false;
                        currentLineIndex++;
                        oTab[currentLineIndex, L4_COLUMN_INDEX] = GetLevelLabel(currentRow, 4, detailLevel);
                        oTab[currentLineIndex, L4_ID_COLUMN_INDEX] = GetLevelId(currentRow, 4, detailLevel);
                        oTab[currentLineIndex, TOTAL_COLUMN_INDEX] = (double)0.0;
                        oTab[currentLineIndex, L1_ID_COLUMN_INDEX] = oldIdL1;
                        oTab[currentLineIndex, L2_ID_COLUMN_INDEX] = oldIdL2;
                        oTab[currentLineIndex, L3_ID_COLUMN_INDEX] = oldIdL3;
                        if (nbLevels <= 4) oTab[currentLineIndex, PERIODICITY_COLUMN_INDEX] = currentRow["period_count"].ToString();
                        //Init year totals
                        foreach (int i in years_index.Keys)
                        {
                            oTab[currentLineIndex, years_index[i]] = (double)0.0;
                        }
                        currentL4Index = currentLineIndex;
                        oldIdL4 = GetLevelId(currentRow, 4, detailLevel);
                        currentDate = 0;
                        oTab[currentLineIndex, L1_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L2_COLUMN_INDEX] = null;
                        oTab[currentLineIndex, L3_COLUMN_INDEX] = null;
                        // Create MediaPlanItem
                        for (int mpi = firstPeriodIndex; mpi < nbCol; mpi++)
                        {
                            oTab[currentLineIndex, mpi] = new MediaPlanItem(-1);
                        }
                    }
                    #endregion

                    #region Treat present
                    try
                    {
                        while (periodItemsList[currentDate] != Int64.Parse(currentRow["date_num"].ToString()))
                        {
                            //tab[currentLineIndex,FIRST_PERIOD_INDEX+currentDate]=false;
                            oTab[currentLineIndex, firstPeriodIndex + currentDate] = new MediaPlanItem((long)-1);
                            currentDate++;
                        }
                    }
                    catch (System.Exception e)
                    {
                        Console.Write(e.Message);
                    }
                    // Set periodicity
                    unit = double.Parse(currentRow[FctWeb.SQLGenerator.GetUnitAlias(_session)].ToString());
                    oTab[currentLineIndex, firstPeriodIndex + currentDate] = new MediaPlanItem(Int64.Parse(currentRow["period_count"].ToString()));

                    if (nbLevels >= 4)
                    {
                        oTab[currentL4Index, TOTAL_COLUMN_INDEX] = (double)oTab[currentL4Index, TOTAL_COLUMN_INDEX] + unit;
                        ((MediaPlanItem)oTab[currentL4Index, firstPeriodIndex + currentDate]).Unit += unit;
                    }
                    if (nbLevels >= 3)
                    {
                        oTab[currentL3Index, TOTAL_COLUMN_INDEX] = (double)oTab[currentL3Index, TOTAL_COLUMN_INDEX] + unit;
                        ((MediaPlanItem)oTab[currentL3Index, firstPeriodIndex + currentDate]).Unit += unit;
                    }
                    if (nbLevels >= 2)
                    {
                        oTab[currentL2Index, TOTAL_COLUMN_INDEX] = (double)oTab[currentL2Index, TOTAL_COLUMN_INDEX] + unit;
                        ((MediaPlanItem)oTab[currentL2Index, firstPeriodIndex + currentDate]).Unit += unit;
                    }
                    if (nbLevels >= 1)
                    {
                        oTab[currentL1Index, TOTAL_COLUMN_INDEX] = (double)oTab[currentL1Index, TOTAL_COLUMN_INDEX] + unit;
                        ((MediaPlanItem)oTab[currentL1Index, firstPeriodIndex + currentDate]).Unit += unit;
                    }
                    oTab[currentTotalIndex, TOTAL_COLUMN_INDEX] = (double)oTab[currentTotalIndex, TOTAL_COLUMN_INDEX] + unit;
                    ((MediaPlanItem)oTab[currentTotalIndex, firstPeriodIndex + currentDate]).Unit += unit;

                    //Years total
                    int k = int.Parse(currentRow["date_num"].ToString().Substring(0, 4));
                    if (years_index.Count > 0)
                    {
                        k = int.Parse(years_index[k].ToString());
                        if (nbLevels >= 4) oTab[currentL4Index, k] = (double)oTab[currentL4Index, k] + unit;
                        if (nbLevels >= 3) oTab[currentL3Index, k] = (double)oTab[currentL3Index, k] + unit;
                        if (nbLevels >= 2) oTab[currentL2Index, k] = (double)oTab[currentL2Index, k] + unit;
                        if (nbLevels >= 1) oTab[currentL1Index, k] = (double)oTab[currentL1Index, k] + unit;
                        oTab[currentTotalIndex, k] = (double)oTab[currentTotalIndex, k] + unit;
                    }
                    currentDate++;
                    oldCurrentDate = currentDate;
                    while (oldCurrentDate < periodItemsList.Count)
                    {
                        oTab[currentLineIndex, firstPeriodIndex + currentDate] = new MediaPlanItem(-1);
                        oldCurrentDate++;
                    }
                    #endregion
                }
            }
            catch (System.Exception)
            {
                long nbColDebug = oTab.GetLength(0);
                long nbLineDebug = oTab.GetLength(1);
                long nbRowsDebug = dt.Rows.Count;
                long cli = currentLineIndex;
                int cd = currentDate;
            }
            #endregion

            #region Compute PDMs
            if (nbL1 > 0)
            {
                // PDM L4
                if (nbLevels >= 4)
                {
                    for (Int64 i = currentL3Index + 1; i <= currentLineIndex; i++)
                    {
                        if ((double)oTab[currentL3Index, TOTAL_COLUMN_INDEX] != 0)
                            oTab[i, PDM_COLUMN_INDEX] = (double)oTab[i, TOTAL_COLUMN_INDEX] / (double)oTab[currentL3Index, TOTAL_COLUMN_INDEX] * 100.0;
                        else
                            oTab[i, PDM_COLUMN_INDEX] = 0.0;
                    }
                }
                // PDM L3
                if (nbLevels >= 3)
                {
                    for (int i = 0; i < currentL3PDMIndex; i++)
                    {
                        if ((double)oTab[currentL2Index, TOTAL_COLUMN_INDEX] != 0)
                            oTab[tabL3Index[i], PDM_COLUMN_INDEX] = (double)oTab[tabL3Index[i], TOTAL_COLUMN_INDEX] / (double)oTab[currentL2Index, TOTAL_COLUMN_INDEX] * 100.0;
                        else
                            oTab[tabL3Index[i], PDM_COLUMN_INDEX] = 0.0;
                    }
                }
                // PDM L2
                if (nbLevels >= 2)
                {
                    for (int i = 0; i < currentL2PDMIndex; i++)
                    {
                        if ((double)oTab[currentL1Index, TOTAL_COLUMN_INDEX] != 0)
                            oTab[tabL2Index[i], PDM_COLUMN_INDEX] = (double)oTab[tabL2Index[i], TOTAL_COLUMN_INDEX] / (double)oTab[currentL1Index, TOTAL_COLUMN_INDEX] * 100.0;
                        else
                            oTab[tabL2Index[i], PDM_COLUMN_INDEX] = 0.0;
                    }
                }
                // PDM L1
                if (nbLevels >= 1)
                {
                    for (int i = 0; i < currentL1PDMIndex; i++)
                    {
                        if ((double)oTab[currentTotalIndex, TOTAL_COLUMN_INDEX] != 0)
                            oTab[tabL1Index[i], PDM_COLUMN_INDEX] = (double)oTab[tabL1Index[i], TOTAL_COLUMN_INDEX] / (double)oTab[currentTotalIndex, TOTAL_COLUMN_INDEX] * 100.0;
                        else
                            oTab[tabL1Index[i], PDM_COLUMN_INDEX] = 0.0;
                    }
                }
                // PDM Total
                oTab[currentTotalIndex, PDM_COLUMN_INDEX] = (double)100.0;
            }
            #endregion

            #region Debug: Voir le tableau
#if(DEBUG)
            System.Text.StringBuilder HTML = new System.Text.StringBuilder(2000);
            HTML.Append("<html><table><tr>");
            for (int z = 0; z <= currentLineIndex; z++)
            {
                for (int r = 0; r < nbCol; r++)
                {
                    if (oTab[z, r] != null) HTML.Append("<td>" + oTab[z, r].ToString() + "</td>");
                    else HTML.Append("<td>&nbsp;</td>");
                }
                HTML.Append("</tr><tr>");
            }
            HTML.Append("</tr></table></html>");
            Console.WriteLine(HTML.ToString());
#endif
            #endregion

            #region Periodicity treatement
            MediaPlanItem item = null;
            MediaPlanItem tmp = null;
            MediaPlan.graphicItemType graphicType;
            try
            {
                for (int i = 1; i < nbline; i++)
                {
                    if (oTab[i, 0] != null) if (oTab[i, 0].GetType() == typeof(MemoryArrayEnd)) break;
                    // N1 line
                    if (oTab[i, L1_COLUMN_INDEX] != null) currentL1Index = i;
                    // N2 line
                    if (oTab[i, L2_COLUMN_INDEX] != null) currentL2Index = i;
                    // N3 line
                    if (oTab[i, L3_COLUMN_INDEX] != null) currentL3Index = i;
                    // N4 line
                    if (oTab[i, L4_COLUMN_INDEX] != null) currentL4Index = i;
                    // lower level
                    if ((nbLevels == 1 && currentL1Index == i) || (nbLevels == 2 && currentL2Index == i) || (nbLevels == 3 && currentL3Index == i) || (nbLevels == 4 && currentL4Index == i))
                    {
                        for (int j = firstPeriodIndex; j < nbCol; j++)
                        {
                            item = ((MediaPlanItem)oTab[i, j]);

                            for (int k = 0; k < item.PeriodicityId && (j + k) < nbCol; k++)
                            {
                                if (k == 0)
                                {
                                    graphicType = MediaPlan.graphicItemType.present;
                                }
                                else
                                {
                                    graphicType = MediaPlan.graphicItemType.extended;
                                }
                                ((MediaPlanItem)oTab[i, j + k]).GraphicItemType = graphicType;
                                if (nbLevels > 3 && (tmp = (MediaPlanItem)oTab[currentL3Index, j + k]).GraphicItemType != MediaPlan.graphicItemType.present) tmp.GraphicItemType = graphicType;
                                if (nbLevels > 2 && (tmp = (MediaPlanItem)oTab[currentL2Index, j + k]).GraphicItemType != MediaPlan.graphicItemType.present) tmp.GraphicItemType = graphicType;
                                if (nbLevels > 1 && (tmp = (MediaPlanItem)oTab[currentL1Index, j + k]).GraphicItemType != MediaPlan.graphicItemType.present) tmp.GraphicItemType = graphicType;
                                if (oTab[TOTAL_LINE_INDEX, j + k] == null) oTab[TOTAL_LINE_INDEX, j + k] = new MediaPlanItem();
                                if ((tmp = (MediaPlanItem)oTab[TOTAL_LINE_INDEX, j + k]).GraphicItemType != MediaPlan.graphicItemType.present) tmp.GraphicItemType = graphicType;

                            }

                        }
                    }
                }
            }
            catch (System.Exception err)
            {
                throw (new MediaScheduleException("Unable to compute periodicities : ", err));
            }
            #endregion

            #region Ecriture de fin de tableau
            nbCol = oTab.GetLength(1);
            nbline = oTab.GetLength(0);
            if (currentLineIndex + 1 < nbline)
                oTab[currentLineIndex + 1, 0] = new MemoryArrayEnd();
            #endregion

            ds.Dispose();

            #region Debug: Voir le tableau
#if(DEBUG)
            //						string HTML1="<html><table><tr>";
            //						for(int z=0;z<=currentLineIndex;z++){
            //							for(int r=0;r<nbCol;r++){
            //								if(tab[z,r]!=null)HTML1+="<td>"+tab[z,r].ToString()+"</td>";
            //								else HTML1+="<td>&nbsp;</td>";
            //							}
            //							HTML1+="</tr><tr>";
            //						}
            //						HTML1+="</tr></table></html>";
#endif
            #endregion

            return oTab;
        }

        #region Get Level Id / Label
        /// <summary>
        /// Get Level Id
        /// </summary>
        /// <param name="dr">Data Source</param>
        /// <param name="level">Requested level</param>
        /// <param name="detailLevel">Levels breakdown</param>
        /// <returns>Level Id</returns>
        protected virtual Int64 GetLevelId(DataRow dr, int level, GenericDetailLevel detailLevel)
        {
            return (Int64.Parse(dr[detailLevel.GetColumnNameLevelId(level)].ToString()));
        }
        /// <summary>
        /// Get Level Label
        /// </summary>
        /// <param name="dr">Data Source</param>
        /// <param name="level">Requested level</param>
        /// <param name="detailLevel">Levels breakdown</param>
        /// <returns>Level Label</returns>
        protected string GetLevelLabel(DataRow dr, int level, GenericDetailLevel detailLevel)
        {
            return (dr[detailLevel.GetColumnNameLevelLabel(level)].ToString());
        }

        #endregion

        #endregion

        #region Design table
        /// <summary>
        /// Provide html Code to present Media Schedule
        /// </summary>
        /// <param name="data">Preformated Data</param>
        /// <returns>HTML code</returns>
        protected virtual MediaScheduleData ComputeDesign(object[,] data)
        {
            MediaScheduleData oMediaScheduleData = new MediaScheduleData();
            StringBuilder t = new System.Text.StringBuilder(5000);
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);

            #region Theme Name
            string themeName = WebApplicationParameters.Themes[_session.SiteLanguage].Name;
            #endregion

            #region No data
            if (data.GetLength(0) == 0)
            {
                oMediaScheduleData.HTMLCode = string.Empty;
                return (oMediaScheduleData);
            }
            #endregion

            #region Init Variables
            int yearBegin = _period.Begin.Year;
            int yearEnd = _period.End.Year;
            if(_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.weekly)
            {
                yearBegin = new AtomicPeriodWeek(_period.Begin).Year;
                yearEnd = new AtomicPeriodWeek(_period.End).Year;
            }
            int nbColYear = yearEnd - yearBegin;
            if (nbColYear > 0) nbColYear++;
            int firstPeriodIndex = FIRST_PERIOD_INDEX + nbColYear;

            int nbColTab = data.GetLength(1);
            int nbPeriod = nbColTab - firstPeriodIndex - 1;
            int nbline = data.GetLength(0);

            try { _session.SloganColors.Add((Int64)0, _style.VersionCell0); }catch (System.Exception) { }
            oMediaScheduleData.PeriodNb = (Int64)Math.Round((double)(nbColTab - firstPeriodIndex) / 7);

            bool isExport = _isExcelReport || _isPDFReport;
            int labColSpan = (isExport && !_allowTotal) ? 2 : 1;
            #endregion

            #region Rappel de s�lection
            if (_isExcelReport)
            {
                if (_isCreativeDivisionMS)
                {
                    t.Append(FctExcel.GetExcelHeaderForCreativeMediaPlan(_session));
                }
                else
                {
                    if (_module.Id != CstWeb.Module.Name.BILAN_CAMPAGNE)
                    {
                        t.Append(FctExcel.GetLogo(_session));
                        if (_session.CurrentModule == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA)
                        {
                            t.Append(FctExcel.GetExcelHeader(_session, true, false, Zoom, (int)_session.DetailPeriod));
                        }
                        else
                        {
                            t.Append(FctExcel.GetExcelHeaderForMediaPlanPopUp(_session, false, "", "", Zoom, (int)_session.DetailPeriod));
                        }
                    }
                    else
                    {
                        t.Append(FctExcel.GetAppmLogo(_session));
                        t.Append(FctExcel.GetExcelHeader(_session, GestionWeb.GetWebWord(1474,_session.SiteLanguage)));
                    }
                }
            }
            #endregion

            #region Colonnes

            #region basic columns (product, total, PDM, version, insertion, years totals)
            int rowSpanNb = 3;
            if (_period.PeriodDetailLEvel != CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
            {
                rowSpanNb = 2;
            }
            t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
            // Product Column (Force nowrap in this column)
            t.AppendFormat("\r\n\t\t<td colSpan=\"{4}\" rowspan=\"{3}\" width=\"250px\" class=\"{0}\">{1}{2}</td>"
                , _style.CellTitle
                , GestionWeb.GetWebWord(804, _session.SiteLanguage)
                , (!isExport) ? string.Empty : "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                , rowSpanNb
                , labColSpan);
            // Total Column
            if (_allowTotal)
            {
                t.AppendFormat("\r\n\t\t<td rowspan={2} class=\"{0}\">{1}", _style.CellTitle, GestionWeb.GetWebWord(805, _session.SiteLanguage), rowSpanNb);
                int nbtot = FctWeb.Units.ConvertUnitValueToString(data[1, TOTAL_COLUMN_INDEX].ToString(), _session.Unit).Length;
                int nbSpace = (nbtot - 1) / 3;
                int nbCharTotal = nbtot + nbSpace - 5;
                if (nbCharTotal < 5) nbCharTotal = 0;
                for (int h = 0; h < nbCharTotal; h++)
                {
                    t.Append("&nbsp;");
                }
                t.Append("</td>");
            }
            //PDM
            if (_allowPdm)
            {
                t.AppendFormat("\r\n\t\t<td rowspan=\"{2}\" class=\"{0}\">{1}</td>", _style.CellTitle, GestionWeb.GetWebWord(806, _session.SiteLanguage), rowSpanNb);
            }
            //Version
            if (_allowVersion)
            {
                t.AppendFormat("\r\n\t\t<td rowspan=\"{2}\" class=\"{0}\">{1}</td>", _style.CellTitle, GestionWeb.GetWebWord(1994, _session.SiteLanguage), rowSpanNb);
            }
            // Insertions
            if (_allowInsertions)
            {
                t.AppendFormat("\r\n\t\t<td rowspan=\"{2}\" class=\"{0}\">{1}</td>", _style.CellTitle, GestionWeb.GetWebWord(UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].WebTextId, _session.SiteLanguage), rowSpanNb);
            }
            // Years necessary if the period consists of several years
            for (int k = FIRST_PERIOD_INDEX; k < firstPeriodIndex && _allowTotal; k++)
            {
                t.AppendFormat("\r\n\t\t<td rowspan=\"{2}\" class=\"{0}\">{1}</td>", _style.CellTitle, data[0, k], rowSpanNb);
            }
            #endregion

            #region Period
            nbPeriod = 0;
            int prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
            StringBuilder periods = new StringBuilder();
            StringBuilder headers = new StringBuilder();
            string periodClass;
            string link = string.Empty;
            switch (_session.CurrentModule)
            {
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_POTENTIELS:
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE:
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE:
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:
                case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PORTEFEUILLE:
                    link = TNS.AdExpress.Constantes.Web.Links.MEDIA_SCHEDULE_POP_UP;
                    break;
                case TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE:
                    link = TNS.AdExpress.Constantes.Web.Links.APPM_ZOOM_PLAN_MEDIA;
                    break;
                default:
                    link = TNS.AdExpress.Constantes.Web.Links.ZOOM_PLAN_MEDIA;
                    break;
            }



            switch (_period.PeriodDetailLEvel)
            {
                case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:
                case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:
                    prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
                    for (int j = firstPeriodIndex; j < nbColTab; j++)
                    {
                        if (prevPeriod != int.Parse(data[0, j].ToString().Substring(0, 4)))
                        {
                            if (nbPeriod < 3)
                                headers.AppendFormat("<td colspan={0} class=\"{1}\">&nbsp;</td>", nbPeriod, _style.CellYear1);
                            else
                                headers.AppendFormat("<td colspan={0} class=\"{1}\">{2}</td>", nbPeriod, _style.CellYear1, prevPeriod);
                            nbPeriod = 0;
                            prevPeriod = int.Parse(data[0, j].ToString().Substring(0, 4));

                        }

                        switch (_period.PeriodDetailLEvel)
                        {
                            case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:

                                #region Period Color Management
                                // First Period or last period is incomplete
                                periodClass = _style.CellPeriod;
                                if ((j == firstPeriodIndex && _period.Begin.Day != 1)
                                   || (j == (nbColTab - 1) && _period.End.Day != _period.End.AddDays(1 - _period.End.Day).AddMonths(1).AddDays(-1).Day))
                                {
                                    periodClass = _style.CellPeriodIncomplete;
                                }
                                #endregion

                                if (!isExport)
                                {
                                    periods.AppendFormat("<td class=\"{0}\" width=\"17px\"><a class=\"{1}\" href=\"{2}?idSession={3}&zoomDate={4}\">&nbsp;{5}&nbsp;</td>"
                                        , periodClass
                                        , _style.CellPeriod
                                        , link
                                        , _session.IdSession
                                        , data[0, j]
                                        , MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1));
                                }
                                else
                                {
                                    periods.AppendFormat("<td class=\"{0}\">&nbsp;{1}&nbsp;</td>"
                                        , periodClass
                                        , MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1));
                                }
                                break;
                            case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:

                                #region Period Color Management
                                periodClass = _style.CellPeriod;
                                if ((j == firstPeriodIndex && _period.Begin.DayOfWeek != DayOfWeek.Monday)
                                   || (j == (nbColTab - 1) && _period.End.DayOfWeek != DayOfWeek.Sunday))
                                {
                                    periodClass = _style.CellPeriodIncomplete;
                                }
                                #endregion

                                if (!isExport)
                                {

                                    periods.AppendFormat("<td class=\"{0}\" width=\"17px\"><a class=\"{1}\" href=\"{2}?idSession={3}&zoomDate={4}\">&nbsp;{5}&nbsp;</a></td>"
                                        , periodClass
                                        , _style.CellPeriod
                                        , link
                                        , _session.IdSession
                                        , data[0, j]
                                        , data[0, j].ToString().Substring(4, 2));
                                }
                                else
                                {
                                    periods.AppendFormat("<td class=\"{0}\">&nbsp;{1}&nbsp;</td>"
                                        , periodClass
                                        , data[0, j].ToString().Substring(4, 2));
                                }
                                break;

                        }
                        nbPeriod++;
                    }
                    // Compute last date
                    if (nbPeriod < 3)
                        headers.AppendFormat("<td colspan={0} class=\"{1}\">&nbsp;</td>", nbPeriod, _style.CellYear);
                    else
                        headers.AppendFormat("<td colspan={0} class=\"{1}\">{2}</td>", nbPeriod, _style.CellYear, prevPeriod);

                    t.AppendFormat("{0}</tr>", headers);

                    t.AppendFormat("<tr>{0}</tr>", periods);
                    
                    oMediaScheduleData.Headers = t.ToString();

                   // t.Append("\r\n\t<tr>");

                    break;
                case CstWeb.CustomerSessions.Period.DisplayLevel.dayly:
                    StringBuilder days = new StringBuilder();
                    periods.Append("<tr>");
                    days.Append("\r\n\t<tr>");
                    DateTime currentDay = DateString.YYYYMMDDToDateTime((string)data[0, firstPeriodIndex]);
                    prevPeriod = currentDay.Month;
                    currentDay = currentDay.AddDays(-1);
                    for (int j = firstPeriodIndex; j < nbColTab; j++)
                    {
                        currentDay = currentDay.AddDays(1);
                        if (currentDay.Month != prevPeriod)
                        {
                            if (nbPeriod >= 8)
                                headers.AppendFormat("<td colspan=\"{0}\" class=\"{1}\" align=center>{2}</td>", nbPeriod, _style.CellTitle, FctWeb.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM")));
                            else
                                headers.AppendFormat("<td colspan=\"{0}\" class=\"{1}\" align=center>&nbsp;</td>", nbPeriod, _style.CellTitle);
                            nbPeriod = 0;
                            prevPeriod = currentDay.Month;
                        }
                        nbPeriod++;
                        //Period Number
                        periods.AppendFormat("<td class=\"{0}\" nowrap>&nbsp;{1}&nbsp;</td>", _style.CellPeriod, currentDay.ToString("dd"));
                        //Period day
                        if (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
                            days.AppendFormat("<td class=\"{0}\">{1}</td>", _style.CellDayWE, DayString.GetCharacters(currentDay,cultureInfo,1));
                        else
                            days.AppendFormat("<td class=\"{0}\">{1}</td>", _style.CellDay, DayString.GetCharacters(currentDay, cultureInfo, 1));

                    }
                    if (nbPeriod >= 8)
                        headers.AppendFormat("<td colspan=\"{0}\" class=\"{1}\" align=center>{2}</td>", nbPeriod, _style.CellTitle, FctWeb.Dates.getPeriodTxt(_session, currentDay.ToString("yyyyMM")));
                    else
                        headers.AppendFormat("<td colspan=\"{0}\" class=\"{1}\" align=center>&nbsp;</td>", nbPeriod, _style.CellTitle);

                    periods.Append("</tr>");
                    days.Append("</tr>");
                    headers.Append("</tr>");

                    t.Append(headers);
                    t.Append(periods);
                    t.Append(days);

                    oMediaScheduleData.Headers = t.ToString();
                    break;

            }
            #endregion

            #endregion

            #region Media Schedule
            int i = -1;
            try
            {
                int colorItemIndex = 1;
                int colorNumberToUse = 0;
                int sloganIndex = GetSloganIdIndex();
                Int64 sloganId = -1;
                string stringItem = "&nbsp;";
                string cssPresentClass = string.Empty;
                string cssExtendedClass = string.Empty;
                string cssClasse = string.Empty;
                string cssClasseNb = string.Empty;

                for (i = 1; i < nbline; i++)
                {

                    #region Color Management
                    if (sloganIndex != -1 && data[i, sloganIndex] != null &&
                        ((_session.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) == _session.GenericMediaDetailLevel.GetNbLevels) ||
                        (_session.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) < _session.GenericMediaDetailLevel.GetNbLevels && data[i, sloganIndex + 1] == null)))
                    {
                        sloganId = Convert.ToInt64(data[i, sloganIndex]);
                        if (!_session.SloganColors.ContainsKey(sloganId))
                        {
                            colorNumberToUse = (colorItemIndex % _style.CellVersions.Count) + 1;
                            cssClasse = _style.CellVersions[colorNumberToUse];
                            cssClasseNb = _style.CellVersions[colorNumberToUse];
                            _session.SloganColors.Add(sloganId, _style.CellVersions[colorNumberToUse]);
                            if (_allowVersion)
                            {
                                oMediaScheduleData.VersionsDetail.Add(sloganId, new VersionItem(sloganId, cssClasse));
                            }
                            else if (isExport)
                            {
                                switch (VehiclesInformation.DatabaseIdToEnum(_vehicleId))
                                {
                                    case CstDBClassif.Vehicles.names.directMarketing:
                                        oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportMDVersionItem(sloganId, cssClasse));
                                        break;
                                    case CstDBClassif.Vehicles.names.outdoor:
                                        oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportOutdoorVersionItem(sloganId, cssClasse));
                                        break;
                                    default:
                                        oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportVersionItem(sloganId, cssClasse));
                                        break;
                                }

                            }
                            colorItemIndex++;
                        }
                        if (sloganId != 0 && !oMediaScheduleData.VersionsDetail.ContainsKey(sloganId))
                        {
                            if (_allowVersion)
                            {
                                oMediaScheduleData.VersionsDetail.Add(sloganId, new VersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
                            }
                            else if (isExport)
                            {
                                switch (VehiclesInformation.DatabaseIdToEnum(_vehicleId))
                                {
                                    case CstDBClassif.Vehicles.names.directMarketing:
                                        oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportMDVersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
                                        break;
                                    case CstDBClassif.Vehicles.names.outdoor:
                                        oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportOutdoorVersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
                                        break;
                                    default:
                                        oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportVersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
                                        break;
                                }

                            }
                        }
                        cssPresentClass = _session.SloganColors[sloganId].ToString();
                        cssExtendedClass = _session.SloganColors[sloganId].ToString();
                        stringItem = "x";
                    }
                    else
                    {
                        cssPresentClass = _style.CellPresent;
                        cssExtendedClass = _style.CellExtended;
                        stringItem = "&nbsp;";
                    }
                    #endregion

                    #region Line Treatement
                    for (int j = 0; j < nbColTab; j++)
                    {
                        switch (j)
                        {
                            #region Level 1
                            case L1_COLUMN_INDEX:
                                if (data[i, j] != null)
                                {
                                    if (i == TOTAL_LINE_INDEX)
                                    {
                                        cssClasse = _style.CellLevelTotal;
                                        cssClasseNb = _style.CellLevelTotalNb;
                                    }
                                    else
                                    {
                                        cssClasse = _style.CellLevelL1;
                                        cssClasseNb = _style.CellLevelL1Nb;
                                    }
                                    if (data[i, j].GetType() == typeof(MemoryArrayEnd))
                                    {
                                        i = int.MaxValue - 2;
                                        j = int.MaxValue - 2;
                                        break;
                                    }
                                    AppenLabelTotalPDM(data, t, i, cssClasse, cssClasseNb, j, string.Empty, labColSpan);
                                    if (_allowVersion)
                                    {
                                        if (i != TOTAL_LINE_INDEX)
                                        {
                                            AppendCreativeLink(data, t, themeName, i, cssClasse, j);
                                        }
                                        else
                                        {
                                            t.AppendFormat("<td align=\"center\" class=\"{0}\"></td>", cssClasse);
                                        }

                                    }
                                    if (_allowInsertions)
                                    {
                                        if (i != TOTAL_LINE_INDEX)
                                        {
                                            AppendInsertionLink(data, t, themeName, i, cssClasse, j);
                                        }
                                        else
                                        {
                                            t.AppendFormat("<td align=\"center\" class=\"{0}\"></td>", cssClasse);
                                        }
                                    }
                                    //Totals
                                    for (int k = 1; k <= nbColYear; k++)
                                    {
                                        AppendYearsTotal(data, t, i, cssClasseNb, j + 10 + k);
                                    }
                                    j = j + 10 + nbColYear;
                                }
                                break;
                            #endregion

                            #region Level 2
                            case L2_COLUMN_INDEX:
                                if (data[i, j] != null)
                                {
                                    AppenLabelTotalPDM(data, t, i, _style.CellLevelL2, _style.CellLevelL2Nb, j, "&nbsp;", labColSpan);
                                    if (_allowVersion)
                                    {
                                        AppendCreativeLink(data, t, themeName, i, _style.CellLevelL2, j);
                                    }
                                    if (_allowInsertions)
                                    {
                                        AppendInsertionLink(data, t, themeName, i, _style.CellLevelL2, j);
                                    }

                                    for (int k = 1; k <= nbColYear; k++)
                                    {
                                        AppendYearsTotal(data, t, i, _style.CellLevelL2Nb, j + 9 + k);
                                    }
                                    j = j + 9 + nbColYear;
                                }
                                break;
                            #endregion

                            #region Level 3
                            case L3_COLUMN_INDEX:
                                if (data[i, j] != null)
                                {
                                    AppenLabelTotalPDM(data, t, i, _style.CellLevelL3, _style.CellLevelL3Nb, j, "&nbsp;&nbsp;", labColSpan);
                                    if (_allowVersion)
                                    {
                                        AppendCreativeLink(data, t, themeName, i, _style.CellLevelL3, j);
                                    }
                                    if (_allowInsertions)
                                    {
                                        AppendInsertionLink(data, t, themeName, i, _style.CellLevelL3, j);
                                    }

                                    for (int k = 1; k <= nbColYear; k++)
                                    {
                                        AppendYearsTotal(data, t, i, _style.CellLevelL3Nb, j + 8 + k);
                                    }
                                    j = j + 8 + nbColYear;
                                }
                                break;
                            #endregion

                            #region Level 4
                            case L4_COLUMN_INDEX:
                                AppenLabelTotalPDM(data, t, i, _style.CellLevelL4, _style.CellLevelL4Nb, j, "&nbsp;&nbsp;&nbsp;", labColSpan);
                                if (_allowVersion)
                                {
                                    AppendCreativeLink(data, t, themeName, i, _style.CellLevelL4, j);
                                }
                                if (_allowInsertions)
                                {
                                    AppendInsertionLink(data, t, themeName, i, _style.CellLevelL4, j);
                                }

                                for (int k = 1; k <= nbColYear; k++)
                                {
                                    AppendYearsTotal(data, t, i, _style.CellLevelL4Nb, j + 7 + k);
                                }
                                j = j + 7 + nbColYear;
                                break;
                            #endregion

                            #region Other
                            default:
                                if (data[i, j] == null)
                                {
                                    t.AppendFormat("<td class=\"{0}\">&nbsp;</td>", _style.CellNotPresent);
                                    break;
                                }
                                if (data[i, j].GetType() == typeof(MediaPlanItem))
                                {
                                    switch (((MediaPlanItem)data[i, j]).GraphicItemType)
                                    {
                                        case DetailledMediaPlan.graphicItemType.present:
                                            if (_showValues)
                                            {
                                                t.AppendFormat("<td class=\"{0}\">{1}</td>", cssPresentClass, FctWeb.Units.ConvertUnitValueToString(((MediaPlanItem)data[i, j]).Unit.ToString(), _session.Unit));
                                            }
                                            else
                                            {
                                                t.AppendFormat("<td class=\"{0}\">{1}</td>", cssPresentClass, stringItem);
                                            }
                                            break;
                                        case DetailledMediaPlan.graphicItemType.extended:
                                            t.AppendFormat("<td class=\"{0}\">&nbsp;</td>", cssExtendedClass);
                                            break;
                                        default:
                                            t.AppendFormat("<td class=\"{0}\">&nbsp;</td>", _style.CellNotPresent);
                                            break;
                                    }
                                }
                                break;
                            #endregion

                        }
                    }
                    t.Append("</tr>");
                    #endregion

                }
            }
            catch (System.Exception err)
            {
                throw (new MediaScheduleException("Error i=" + i));
            }
            t.Append("</table>");
            #endregion

            // Release table
            data = null;

            if (_isExcelReport && !_isCreativeDivisionMS)
            {
                t.Append(FctExcel.GetFooter(_session));
            }

            oMediaScheduleData.HTMLCode = t.ToString();
            return oMediaScheduleData;
        }

        #region Table Building Functions
        /// <summary>
        /// Append Total for a specific year
        /// </summary>
        /// <param name="data">Data Source</param>
        /// <param name="t">StringBuilder to fill</param>
        /// <param name="line">Current line</param>
        /// <param name="cssClasseNb">Line syle</param>
        /// <param name="tmpCol">Year column in data source</param>
        protected virtual void AppendYearsTotal(object[,] data, StringBuilder t, int line, string cssClasseNb, int tmpCol)
        {
            if (_allowTotal)
            {
                string s = FctWeb.Units.ConvertUnitValueToString(data[line, tmpCol].ToString(), _session.Unit).Trim();
                if (s.Length <= 0)
                {
                    s = "&nbsp;";
                }

                t.AppendFormat("<td class=\"{0}\" nowrap>{1}</td>"
                    , cssClasseNb
                    , s
                    );
            }
        }

        /// <summary>
        /// Append Label, Total and PDM
        /// </summary>
        /// <param name="data">Data Source</param>
        /// <param name="t">StringBuilder to fill</param>
        /// <param name="line">Current line</param>
        /// <param name="cssClasse">Line syle</param>
        /// <param name="cssClasseNb">Line syle for numbers</param>
        /// <param name="col">Column to consider</param>
        /// <param name="padding">Stirng to insert before Label</param>
        protected virtual void AppenLabelTotalPDM(object[,] data, StringBuilder t, int line, string cssClasse, string cssClasseNb, int col, string padding, int labColSpan)
        {
            t.AppendFormat("\r\n\t<tr>\r\n\t\t<td class=\"{0}\" colSPan=\"{6}\">{5}{1}</td>"
                , cssClasse
                , data[line, col]
                , cssClasseNb
                , FctWeb.Units.ConvertUnitValueToString(data[line, TOTAL_COLUMN_INDEX].ToString(), _session.Unit)
                , ((double)data[line, PDM_COLUMN_INDEX]).ToString("0.00")
                , padding
                , labColSpan);
            if (_allowTotal)
            {

                t.AppendFormat("<td class=\"{0}\">{1}</td>"
                    , cssClasseNb
                    , FctWeb.Units.ConvertUnitValueToString(data[line, TOTAL_COLUMN_INDEX].ToString(), _session.Unit)
                    , ((double)data[line, PDM_COLUMN_INDEX]).ToString("0.00"));
            }
            if (_allowPdm)
            {
                t.AppendFormat("<td class=\"{0}\">{1}</td>"
                    , cssClasseNb
                    , ((double)data[line, PDM_COLUMN_INDEX]).ToString("0.00"));
            }
        }

        /// <summary>
        /// Append Lionk to insertion popup
        /// </summary>
        /// <param name="data">Data Source</param>
        /// <param name="t">StringBuilder to fill</param>
        /// <param name="themeName">Name of the current theme</param>
        /// <param name="line">Current line</param>
        /// <param name="cssClasse">Line syle</param>
        /// <param name="level">Column index of the current level (except for level 4 which represent by level 3)</param>
        protected virtual void AppendInsertionLink(object[,] data, StringBuilder t, string themeName, int line, string cssClasse, int level)
        {
            if (data[line, level] != null)
            {
                t.AppendFormat("<td align=\"center\" class=\"{0}\"><a href=\"javascript:OpenInsertions('{1}','{2}','{3}');\"><img border=0 src=\"/App_Themes/{4}/Images/Common/picto_plus.gif\"></a></td>"
                    , cssClasse
                    , _session.IdSession
                    , GetLevelFilter(data, line, level)
                    , _zoom
                    , themeName
                );
            }
            else
            {
                t.AppendFormat("<td align=\"center\" class=\"{0}\">&nbsp;</td>", cssClasse);
            }
        }

        /// <summary>
        /// Append Link to version popup
        /// </summary>
        /// <param name="data">Data Source</param>
        /// <param name="t">StringBuilder to fill</param>
        /// <param name="themeName">Name of the current theme</param>
        /// <param name="line">Current line</param>
        /// <param name="cssClasse">Line syle</param>
        /// <param name="level">Column index of the current level (except for level 4 which represent by level 3)</param>
        protected virtual void AppendCreativeLink(object[,] data, StringBuilder t, string themeName, int line, string cssClasse, int level)
        {
            if (data[line, level] != null)
            {
                t.AppendFormat("<td align=\"center\" class=\"{0}\"><a href=\"javascript:OpenCreatives('{1}','{2}','{3}','-1','{4}');\"><img border=0 src=\"/App_Themes/{5}/Images/Common/picto_plus.gif\"></a></td>"
                    , cssClasse
                    , _session.IdSession
                    , GetLevelFilter(data, line, level)
                    , _zoom
                    , CstWeb.Module.Name.ANALYSE_PLAN_MEDIA
                    , themeName);
            }
            else
            {
                t.AppendFormat("<td align=\"center\" class=\"{0}\">&nbsp;</td>", cssClasse);
            }
        }

        /// <summary>
        /// Build level filters
        /// </summary>
        /// <param name="data">Data source</param>
        /// <param name="line">Current line</param>
        /// <param name="level">Current level</param>
        /// <returns>Filters as "id1,id2,id3,id4,-1" (idX replace by -1 if required depending on the curretn level)</returns>
        protected virtual string GetLevelFilter(object[,] data, int line, int level)
        {
            switch (level)
            {
                case L1_COLUMN_INDEX:
                    return string.Format("{0},-1,-1,-1,-1", data[line, L1_ID_COLUMN_INDEX]);
                case L2_COLUMN_INDEX:
                    return string.Format("{0},{1},-1,-1,-1", data[line, L1_ID_COLUMN_INDEX], data[line, L2_ID_COLUMN_INDEX]);
                    break;
                case L3_COLUMN_INDEX:
                    return string.Format("{0},{1},{2},-1,-1", data[line, L1_ID_COLUMN_INDEX], data[line, L2_ID_COLUMN_INDEX], data[line, L3_ID_COLUMN_INDEX]);
                case L4_COLUMN_INDEX:
                    return string.Format("{0},{1},{2},{3},-1", data[line, L1_ID_COLUMN_INDEX], data[line, L2_ID_COLUMN_INDEX], data[line, L3_ID_COLUMN_INDEX], data[line, L4_ID_COLUMN_INDEX]);
            }
            return string.Empty;
        }
        #endregion

        #region Slogan Index
        /// <summary>
        /// Get column which contains id_slogan, -1 if missing
        /// </summary>
        /// <returns>Index of column if found, -1 either</returns>
        protected virtual int GetSloganIdIndex()
        {
            int rank = _session.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan);
            switch (rank)
            {
                case 1:
                    return (DetailledMediaPlan.L1_ID_COLUMN_INDEX);
                case 2:
                    return (DetailledMediaPlan.L2_ID_COLUMN_INDEX);
                case 3:
                    return (DetailledMediaPlan.L3_ID_COLUMN_INDEX);
                case 4:
                    return (DetailledMediaPlan.L4_ID_COLUMN_INDEX);
                default:
                    return (-1);
            }
        }
        #endregion

        #endregion

        #region Excel design Table
        /// <summary>
        /// Provide Excel page to present Media Schedule for ANUBIS
        /// </summary>
        /// <param name="data">Preformated Data</param>
        /// <param name="excel">Object Excel for compute a page lan media</param>
        protected virtual void ComputeDesignExcel(object[,] data, Workbook excel) {

            if (data.GetLength(0) != 0) {

                #region Init Variables
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
                MediaScheduleData oMediaScheduleData = new MediaScheduleData();
                UnitInformation unitInformation = _session.GetSelectedUnit();

                #region Excel
                Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
                Cells cells = sheet.Cells;
                string formatTotal = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo.GetExcelFormatPattern(unitInformation.Format);
                string formatPdm = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo.GetExcelFormatPattern(EXCEL_PATTERN_NAME_PERCENTAGE);

                bool premier = true;
                int header = 1;
                string prevYearString = string.Empty;
                int nbMaxRowByPage = 42;
                int s = 1;
                int cellRow = 5;
                int startIndex = cellRow;
                int upperLeftColumn = 10;
                string vPageBreaks = "";
                double columnWidth = 0, indexLogo = 0, index;
                bool verif = true;
                int colSupport = 1;
                int colTotal = 2;
                int colPdm = 2;
                int colTotalYears = 2;
                int colVersion = 2;
                int colInsertion = 2;
                int colFirstMediaPlan = 2;

                int colorItemIndex = 1;
                int colorNumberToUse = 0;
                int sloganIndex = GetSloganIdIndex();
                Int64 sloganId = -1;
                string stringItem = "";
                Aspose.Cells.Style presentstyle = null;
                Aspose.Cells.Style extendedStyle = null;
                Aspose.Cells.Style style = null;
                Aspose.Cells.Style styleNb = null;
                Aspose.Cells.Style stylePdmNb = null;
                #endregion

                int yearBegin = _period.Begin.Year;
                int yearEnd = _period.End.Year;
                if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.weekly) {
                    yearBegin = new AtomicPeriodWeek(_period.Begin).Year;
                    yearEnd = new AtomicPeriodWeek(_period.End).Year;
                }
                int nbColYear = yearEnd - yearBegin;
                if (nbColYear > 0) nbColYear++;
                int firstPeriodIndex = FIRST_PERIOD_INDEX + nbColYear;

                int nbColTab = data.GetLength(1);
                int nbPeriod = nbColTab - firstPeriodIndex - 1;
                int nbPeriodTotal = 0;
                int nbline = data.GetLength(0);
                int nbColTabFirst = 0;
                int nbColTabCell = 0;

                try { _session.SloganColors.Add((Int64)0, _style.VersionCell0); }catch (System.Exception) { }
                oMediaScheduleData.PeriodNb = (Int64)Math.Round((double)(nbColTab - firstPeriodIndex) / 7);

                int labColSpan = 1;
                #endregion

                #region Rappel de s�lection
                /*if (_isExcelReport) {
                    if (_isCreativeDivisionMS) {
                        t.Append(FctExcel.GetExcelHeaderForCreativeMediaPlan(_session));
                    }
                    else {
                        if (_module.Id != CstWeb.Module.Name.BILAN_CAMPAGNE) {
                            t.Append(FctExcel.GetLogo(_session));
                            if (_session.CurrentModule == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA) {
                                t.Append(FctExcel.GetExcelHeader(_session, true, false, Zoom, (int)_session.DetailPeriod));
                            }
                            else {
                                t.Append(FctExcel.GetExcelHeaderForMediaPlanPopUp(_session, false, "", "", Zoom, (int)_session.DetailPeriod));
                            }
                        }
                        else {
                            t.Append(FctExcel.GetAppmLogo(_session));
                            t.Append(FctExcel.GetExcelHeader(_session, GestionWeb.GetWebWord(1474, _session.SiteLanguage)));
                        }
                    }
                }*/
                #endregion

                #region basic columns (product, total, PDM, years totals)
                int rowSpanNb = 3;
                if (_period.PeriodDetailLEvel != CstWeb.CustomerSessions.Period.DisplayLevel.dayly) {
                    rowSpanNb = 2;
                }

                #region Title first column (Product Column)
                cells.Merge(cellRow - 1, colSupport, rowSpanNb, labColSpan);
                WorkSheet.PutCellValue(cells, GestionWeb.GetWebWord(804, _session.SiteLanguage), cellRow - 1, colSupport, colFirstMediaPlan, _styleSheet.CellTitle, null);
                cells[cellRow-1, colSupport].Style.HorizontalAlignment = TextAlignmentType.Left;
                cells[cellRow - 1, colSupport].Style.VerticalAlignment = TextAlignmentType.Top;
                cells[cellRow, colSupport].SetStyle(_styleSheet.CellTitle);
                if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly) {
                    cells[cellRow + 1, colSupport].SetStyle(_styleSheet.CellTitle);
                }
                nbColTabFirst++;
                #endregion

                #region Total Column
                if (_allowTotal) {
                    colTotal = 2;
                    colPdm ++;
                    colVersion++;
                    colInsertion++;
                    colTotalYears++;
                    colFirstMediaPlan++;
                    cells.Merge(cellRow - 1, colTotal, rowSpanNb, labColSpan);
                    WorkSheet.PutCellValue(cells, GestionWeb.GetWebWord(805, _session.SiteLanguage), cellRow - 1, colTotal, colFirstMediaPlan, _styleSheet.CellTitle,null);
                    cells[cellRow, colTotal].SetStyle(_styleSheet.CellTitle);
                    if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly) {
                        cells[cellRow + 1, colTotal].SetStyle(_styleSheet.CellTitle);
                    }
                    int nbtot = FctWeb.Units.ConvertUnitValueToString(data[1, TOTAL_COLUMN_INDEX].ToString(), _session.Unit).Length;
                    int nbSpace = (nbtot - 1) / 3;
                    int nbCharTotal = nbtot + nbSpace - 5;
                    nbColTabFirst++;
                }
                else {
                    colTotal = 0;
                    colPdm = 2;
                    colVersion = 2;
                    colInsertion = 2;
                    colFirstMediaPlan = 2;
                    colTotalYears = 2;
                }
                #endregion

                #region PDM Column
                if (_allowPdm) {
                    colVersion++;
                    colInsertion++;
                    colFirstMediaPlan++;
                    colTotalYears++;
                    cells.Merge(cellRow - 1, colPdm, rowSpanNb, labColSpan);
                    WorkSheet.PutCellValue(cells, GestionWeb.GetWebWord(806, _session.SiteLanguage), cellRow - 1, colPdm, colFirstMediaPlan, _styleSheet.CellTitle,null);
                    cells[cellRow, colPdm].SetStyle(_styleSheet.CellTitle);
                    if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly) {
                        cells[cellRow + 1, colPdm].SetStyle(_styleSheet.CellTitle);
                    }
                    nbColTabFirst++;
                }
                else {
                    colPdm = 0;
                }
                #endregion

                #region Total Years
                if (FIRST_PERIOD_INDEX != firstPeriodIndex && _allowTotal) {
                    int nbAddCol = 1;
                    if (nbColYear != 0)
                        nbAddCol = nbColYear;
                    colFirstMediaPlan += nbAddCol;
                    // Years necessary if the period consists of several years
                    for (int k = FIRST_PERIOD_INDEX, l = 0; k < firstPeriodIndex; k++, l++) {
                        cells.Merge(cellRow - 1, colTotalYears + l, rowSpanNb, labColSpan);
                        cells[cellRow, colTotalYears + l].SetStyle(_styleSheet.CellTitle);
                        if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly) {
                            cells[cellRow + 1, colTotalYears + l].SetStyle(_styleSheet.CellTitle);
                        }
                        WorkSheet.PutCellValue(cells, data[0, k], cellRow-1, colTotalYears + l, colFirstMediaPlan, _styleSheet.CellTitle,null);
                        nbColTabFirst++;
                    }
                    
                }
                else {
                    colTotalYears = 0;
                }
                #endregion

                #region Period
                nbPeriod = 0;
                int prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
                int lastPeriod = prevPeriod;
                bool first = true;
                Aspose.Cells.Style periodStyle = null;
                switch (_period.PeriodDetailLEvel) {
                    case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:
                    case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:
                        prevPeriod = int.Parse(data[0, firstPeriodIndex].ToString().Substring(0, 4));
                        for (int j = firstPeriodIndex, currentColMediaPlan = colFirstMediaPlan; j < nbColTab; j++, currentColMediaPlan++) {
                            if (prevPeriod != int.Parse(data[0, j].ToString().Substring(0, 4))) {
                                cells.Merge(startIndex - 1, nbColTabFirst + 1, 1, nbPeriod);
                                if (nbPeriod < 3)
                                    WorkSheet.PutCellValue(cells, "", startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, _styleSheet.CellYear1, null);
                                else
                                    WorkSheet.PutCellValue(cells, prevPeriod, startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, _styleSheet.CellYear1, null);
                                
                                cells[startIndex - 1, nbColTabFirst + nbPeriod].SetStyle(_styleSheet.CellYear1);
                                nbColTabFirst += nbPeriod;
                                nbPeriod = 0;
                                prevPeriod = int.Parse(data[0, j].ToString().Substring(0, 4));

                            }

                            switch (_period.PeriodDetailLEvel) {
                                case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:

                                    #region Period Color Management
                                    // First Period or last period is incomplete
                                    periodStyle = _styleSheet.CellPeriod;
                                    if ((j == firstPeriodIndex && _period.Begin.Day != 1)
                                       || (j == (nbColTab - 1) && _period.End.Day != _period.End.AddDays(1 - _period.End.Day).AddMonths(1).AddDays(-1).Day)) {
                                        periodStyle = _styleSheet.CellPeriodIncomplete;
                                    }
                                    #endregion

                                    WorkSheet.PutCellValue(cells, MonthString.GetCharacters(int.Parse(data[0, j].ToString().Substring(4, 2)), cultureInfo, 1), startIndex, currentColMediaPlan, colFirstMediaPlan, periodStyle, null);
                                    break;
                                case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:

                                    #region Period Color Management
                                    periodStyle = _styleSheet.CellPeriod;
                                    if ((j == firstPeriodIndex && _period.Begin.DayOfWeek != DayOfWeek.Monday)
                                       || (j == (nbColTab - 1) && _period.End.DayOfWeek != DayOfWeek.Sunday)) {
                                        periodStyle = _styleSheet.CellPeriodIncomplete;
                                    }
                                    #endregion

                                    WorkSheet.PutCellValue(cells, int.Parse(data[0, j].ToString().Substring(4, 2)), startIndex, currentColMediaPlan, colFirstMediaPlan, periodStyle, null);
                                    break;

                            }
                            nbPeriod++;
                            nbPeriodTotal++;
                        }
                        // Compute last date
                        cells.Merge(startIndex - 1, nbColTabFirst+1, 1, nbPeriod);
                        if (nbPeriod < 3)
                            WorkSheet.PutCellValue(cells, "", startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, _styleSheet.CellYear, null);
                        else
                            WorkSheet.PutCellValue(cells, prevPeriod, startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, _styleSheet.CellYear, null);

                        for (int k = colFirstMediaPlan; k < (nbPeriodTotal + colFirstMediaPlan); k++) {
                            cells[startIndex-1, k].Style.Number = 1;
                            if(_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.weekly)
                                cells[startIndex, k].Style.Number = 1;
                        }

                        break;
                    case CstWeb.CustomerSessions.Period.DisplayLevel.dayly:
                        DateTime currentDay = DateString.YYYYMMDDToDateTime((string)data[0, firstPeriodIndex]);
                        prevPeriod = currentDay.Month;
                        currentDay = currentDay.AddDays(-1);
                        for (int j = firstPeriodIndex, currentColMediaPlan = colFirstMediaPlan; j < nbColTab; j++, currentColMediaPlan++) {
                            currentDay = currentDay.AddDays(1);
                            if (currentDay.Month != prevPeriod) {
                                cells.Merge(startIndex - 1, nbColTabFirst+1, 1, nbPeriod);
                                if (nbPeriod >= 8)
                                    WorkSheet.PutCellValue(cells, FctWeb.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM")), startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, _styleSheet.CellYear1, null);
                                else
                                    WorkSheet.PutCellValue(cells, "", startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, _styleSheet.CellYear1, null);
                                cells[startIndex - 1, nbColTabFirst + nbPeriod].SetStyle(_styleSheet.CellYear1);
                                nbColTabFirst += nbPeriod;
                                nbPeriod = 0;
                                prevPeriod = currentDay.Month;
                            }
                            nbPeriod++;
                            nbPeriodTotal++;
                            //Period Number
                            WorkSheet.PutCellValue(cells, currentDay.ToString("dd"), startIndex, currentColMediaPlan, colFirstMediaPlan, _styleSheet.CellPeriod, null);
                            //Period day
                            if (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
                                WorkSheet.PutCellValue(cells, DayString.GetCharacters(currentDay, cultureInfo, 1), startIndex + 1, currentColMediaPlan, colFirstMediaPlan, _styleSheet.CellDayWE, null);
                            else
                                WorkSheet.PutCellValue(cells, DayString.GetCharacters(currentDay, cultureInfo, 1), startIndex + 1, currentColMediaPlan, colFirstMediaPlan, _styleSheet.CellDay, null);
                        }


                        cells.Merge(startIndex - 1, nbColTabFirst+1, 1, nbPeriod);
                        if (nbPeriod >= 8)
                            WorkSheet.PutCellValue(cells, FctWeb.Dates.getPeriodTxt(_session, currentDay.AddDays(-1).ToString("yyyyMM")), startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, _styleSheet.CellYear, null);
                        else
                            WorkSheet.PutCellValue(cells, "", startIndex - 1, nbColTabFirst + 1, colFirstMediaPlan, _styleSheet.CellYear, null);


                        for (int k = colFirstMediaPlan; k < (nbPeriodTotal + colFirstMediaPlan); k++) {
                            cells[startIndex, k].Style.Number = 1;
                        }


                        break;

                }
                #endregion

                #endregion

                #region init Row Media Shedule
                cellRow++;

                if (_period.PeriodDetailLEvel == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
                    cellRow++;
                #endregion

                #region Media Schedule
                int i = -1;
                try {
                    first = true;
                    nbColTabCell = colFirstMediaPlan;
                    int currentColMediaPlan = 0;
                    for (i = 1; i < nbline; i++) {

                        #region Color Management
                        if (sloganIndex != -1 && data[i, sloganIndex] != null &&
                            ((_session.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) == _session.GenericMediaDetailLevel.GetNbLevels) ||
                            (_session.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) < _session.GenericMediaDetailLevel.GetNbLevels && data[i, sloganIndex + 1] == null))) {
                            sloganId = Convert.ToInt64(data[i, sloganIndex]);
                            if (!_session.SloganColors.ContainsKey(sloganId)) {
                                colorNumberToUse = (colorItemIndex % _style.CellVersions.Count) + 1;
                                _session.SloganColors.Add(sloganId, _styleSheet.CellVersions[colorNumberToUse]);
                                switch (VehiclesInformation.DatabaseIdToEnum(_vehicleId)) {
                                    case CstDBClassif.Vehicles.names.directMarketing:
                                        //oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportMDVersionItem(sloganId, style));
                                        break;
                                    case CstDBClassif.Vehicles.names.outdoor:
                                        //oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportOutdoorVersionItem(sloganId, style));
                                        break;
                                    default:
                                        //oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportVersionItem(sloganId, style));
                                        break;
                                }
                                colorItemIndex++;
                            }
                            if (sloganId != 0 && !oMediaScheduleData.VersionsDetail.ContainsKey(sloganId)) {
                                    switch (VehiclesInformation.DatabaseIdToEnum(_vehicleId)) {
                                        case CstDBClassif.Vehicles.names.directMarketing:
                                            //oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportMDVersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
                                            break;
                                        case CstDBClassif.Vehicles.names.outdoor:
                                            //oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportOutdoorVersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
                                            break;
                                        default:
                                            //oMediaScheduleData.VersionsDetail.Add(sloganId, new ExportVersionItem(sloganId, _session.SloganColors[sloganId].ToString()));
                                            break;
                                    }

                            }
                            presentstyle = (Aspose.Cells.Style)_session.SloganColors[sloganId];
                            extendedStyle = (Aspose.Cells.Style)_session.SloganColors[sloganId];
                            stringItem = "x";
                        }
                        else {
                            presentstyle = _styleSheet.CellPresent;
                            extendedStyle = _styleSheet.CellExtended;
                            stringItem = "";
                        }
                        #endregion

                        #region Line Treatement
                        currentColMediaPlan = colFirstMediaPlan;
                        for (int j = 0; j < nbColTab; j++) {
                            switch (j) {
                                #region Level 1
                                case L1_COLUMN_INDEX:
                                    if (data[i, j] != null) {

                                        if (data[i, j].GetType() == typeof(MemoryArrayEnd)) {
                                            i = int.MaxValue - 2;
                                            j = int.MaxValue - 2;
                                            break;
                                        }

                                        #region Style define
                                        if (i == TOTAL_LINE_INDEX) {
                                            style = _styleSheet.CellLevelTotal;
                                            styleNb = _styleSheet.CellLevelTotalNb;
                                            stylePdmNb = _styleSheet.CellLevelTotalPdmNb;
                                        }
                                        else {
                                            style = _styleSheet.CellLevelL1;
                                            styleNb = _styleSheet.CellLevelL1Nb;
                                            stylePdmNb = _styleSheet.CellLevelL1PdmNb;
                                        }
                                        #endregion

                                        #region Label
                                        WorkSheet.PutCellValue(cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, style, null);
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                            WorkSheet.PutCellValue(cells, ((double)data[i, TOTAL_COLUMN_INDEX]), cellRow, colTotal, colFirstMediaPlan, styleNb, formatTotal);
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                            WorkSheet.PutCellValue(cells, ((double)data[i, PDM_COLUMN_INDEX]), cellRow, colPdm, colFirstMediaPlan, stylePdmNb, formatPdm);
                                        #endregion

                                        #region Totals years
                                        for (int k = 1; k <= nbColYear && _allowTotal; k++) {
                                            WorkSheet.PutCellValue(cells, ((double)data[i, j + 10 + k]), cellRow, colTotalYears + (k - 1), colFirstMediaPlan, styleNb, formatTotal);
                                        }
                                        #endregion

                                        j = j + 10 + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 2
                                case L2_COLUMN_INDEX:
                                    if (data[i, j] != null) {

                                        #region Style define
                                        if (premier) {
                                            style = _styleSheet.CellLevelL2_1;
                                            styleNb = _styleSheet.CellLevelL2_1Nb;
                                            stylePdmNb = _styleSheet.CellLevelL2_1PdmNb;
                                        }
                                        else {
                                            style = _styleSheet.CellLevelL2_2;
                                            styleNb = _styleSheet.CellLevelL2_2Nb;
                                            stylePdmNb = _styleSheet.CellLevelL2_2PdmNb;
                                        }
                                        #endregion

                                        #region Label
                                        WorkSheet.PutCellValue(cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, style, null);
                                        #endregion

                                        #region Total
                                        if (_allowTotal)
                                            WorkSheet.PutCellValue(cells, ((double)data[i, TOTAL_COLUMN_INDEX]), cellRow, colTotal, colFirstMediaPlan, styleNb, formatTotal);
                                        #endregion

                                        #region PDM
                                        if (_allowPdm)
                                            WorkSheet.PutCellValue(cells, ((double)data[i, PDM_COLUMN_INDEX]), cellRow, colPdm, colFirstMediaPlan, stylePdmNb, formatPdm);
                                        #endregion

                                        #region Totals years
                                        for (int k = 1; k <= nbColYear && _allowTotal; k++) {
                                            WorkSheet.PutCellValue(cells, ((double)data[i, j + 9 + k]), cellRow, colTotalYears + (k - 1), colFirstMediaPlan, styleNb, formatTotal);
                                        }
                                        #endregion

                                        premier = !premier;
                                        
                                        j = j + 9 + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 3
                                case L3_COLUMN_INDEX:
                                    if (data[i, j] != null) {
                                        WorkSheet.PutCellValue(cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, _styleSheet.CellLevelL3, null);
                                        /*
                                        AppenLabelTotalPDM(data, t, i, _style.CellLevelL3, _style.CellLevelL3Nb, j, "&nbsp;&nbsp;", labColSpan);
                                        */
                                        for (int k = 1; k <= nbColYear; k++) {
                                            //AppendYearsTotal(data, t, i, _style.CellLevelL3Nb, j + 8 + k);
                                        }
                                        j = j + 8 + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 4
                                case L4_COLUMN_INDEX:
                                    WorkSheet.PutCellValue(cells, data[i, j].ToString(), cellRow, colSupport, colFirstMediaPlan, _styleSheet.CellLevelL4, null);
                                    //AppenLabelTotalPDM(data, t, i, _style.CellLevelL4, _style.CellLevelL4Nb, j, "&nbsp;&nbsp;&nbsp;", labColSpan);

                                    for (int k = 1; k <= nbColYear; k++) {
                                        //AppendYearsTotal(data, t, i, _style.CellLevelL4Nb, j + 7 + k);
                                    }
                                    j = j + 7 + nbColYear;
                                    break;
                                #endregion

                                #region Other
                                default:
                                    if (data[i, j] == null) {
                                        WorkSheet.PutCellValue(cells, "", cellRow, currentColMediaPlan, colFirstMediaPlan, _styleSheet.CellNotPresent, null);
                                        currentColMediaPlan++;
                                        break;
                                    }
                                    if (data[i, j].GetType() == typeof(MediaPlanItem)) {
                                        switch (((MediaPlanItem)data[i, j]).GraphicItemType) {
                                            case DetailledMediaPlan.graphicItemType.present:
                                                if (_showValues) {
                                                    WorkSheet.PutCellValue(cells, ((MediaPlanItem)data[i, j]).Unit, cellRow, currentColMediaPlan, colFirstMediaPlan, _styleSheet.CellPresent, formatTotal);
                                                }
                                                else {
                                                    WorkSheet.PutCellValue(cells, stringItem, cellRow, currentColMediaPlan, colFirstMediaPlan, _styleSheet.CellPresent, null);
                                                }
                                                break;
                                            case DetailledMediaPlan.graphicItemType.extended:
                                                WorkSheet.PutCellValue(cells, "", cellRow, currentColMediaPlan, colFirstMediaPlan, _styleSheet.CellExtended, null);
                                                break;
                                            default:
                                                WorkSheet.PutCellValue(cells, "", cellRow, currentColMediaPlan, colFirstMediaPlan, _styleSheet.CellNotPresent, null);
                                                break;
                                        }
                                        currentColMediaPlan++;
                                    }
                                    break;
                                #endregion
                            }
                        }
                        if (first) {
                            first = !first;
                            nbColTabCell += currentColMediaPlan-1;
                        }
                        cellRow++;
                        #endregion

                    }
                }
                catch (System.Exception err) {
                    throw (new MediaScheduleException("Error i=" + i));
                }               
                #endregion

                #region Mise en forme de la page

                #region Ajustement de la taile des cellules en fonction du contenu
                sheet.AutoFitColumn(colSupport);

                for (int c = colFirstMediaPlan; c <= (nbColTabCell+1 - colFirstMediaPlan); c++) {
                    if (_showValues) {
                        sheet.AutoFitColumn(c);
                    }
                    else {
                        cells.SetColumnWidth((byte)c, 2);
                    }
                }
                
                #endregion
                
                if (_session.DetailPeriod == CstWeb.CustomerSessions.Period.DisplayLevel.monthly) {
                    for (index = 0; index < 30; index++) {
                        columnWidth += cells.GetColumnWidth((byte)index);
                        if ((columnWidth < 124) && verif)
                            indexLogo++;
                        else
                            verif = false;
                    }
                    upperLeftColumn = (int)indexLogo - 1;
                    vPageBreaks = cells[cellRow, (int)indexLogo].Name;
                    WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1773, _session.SiteLanguage), data.GetLength(0) + 3, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, header.ToString());
                }
                else {
                    if (nbColTabCell > 44) {
                        upperLeftColumn = nbColTabCell - 4;
                        vPageBreaks = cells[cellRow, nbColTabCell - 4].Name;
                    }
                    else {
                        for (index = 0; index < 30; index++) {
                            columnWidth += cells.GetColumnWidth((byte)index);
                            if ((columnWidth < 124) && verif)
                                indexLogo++;
                            else
                                verif = false;
                        }
                        upperLeftColumn = (int)indexLogo - 1;
                        vPageBreaks = cells[cellRow, (int)indexLogo].Name;
                    }

                    WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1773, _session.SiteLanguage), ref s, upperLeftColumn, header.ToString());
                }
                #endregion
            }
        }
        #endregion

        #region Creatives and insertions rights
        /// <summary>
        /// Chack that session verifies conditions to allow access to the versions
        /// </summary>
        /// <returns>True if access authorised</returns>
        protected virtual bool AllowVersions()
        {
            bool allow = false;
            foreach (VehicleInformation v in Vehicles)
            {
                allow = allow || (v.ShowCreations && _session.CustomerLogin.ShowCreatives(v.Id));
            }						

            return (
                allow
                && !_isCreativeDivisionMS
                && !_isExcelReport
                && !_isPDFReport
                && (!VehiclesInformation.Contains(_vehicleId) || (VehiclesInformation.Contains(_vehicleId) && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.adnettrack))
                && _module.Id != TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE
				&&  _session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)
                );
        }
        /// <summary>
        /// Check the session verifies conditions to allow access to the insertions
        /// </summary>
        /// <returns>True if access authorised</returns>
        protected virtual bool AllowInsertions()
        {
            bool allow = false;
            foreach (VehicleInformation v in Vehicles)
            {
                allow = allow || v.ShowInsertions;
            }

            return
                allow
                && !_isCreativeDivisionMS
                && !_isExcelReport
                && !_isPDFReport
                && (!VehiclesInformation.Contains(_vehicleId) || (VehiclesInformation.Contains(_vehicleId) && VehiclesInformation.DatabaseIdToEnum(_vehicleId) != CstDBClassif.Vehicles.names.adnettrack))
                && (_module.Id != TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE || _session.DetailPeriod == CstWeb.CustomerSessions.Period.DisplayLevel.dayly)
				&& _session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)
                ;
        }
        #endregion

        #region GetVehicles
        /// <summary>
        /// Get List of studied vehicles
        /// </summary>
        /// <returns>List of vehicles</returns>
        protected List<VehicleInformation> GetVehicles()
        {
            List<VehicleInformation> vs = null;
            if (_vehicleId > 0)
            {
                vs = new List<VehicleInformation>();
                vs.Add(VehiclesInformation.Get(_vehicleId));
            }
            else
            {
                string vehicles = _session.GetSelection(_session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
                vs = new List<VehicleInformation>(Array.ConvertAll<string, VehicleInformation>(vehicles.Split(','), new Converter<string, VehicleInformation>(delegate(string str) { return VehiclesInformation.Get(Convert.ToInt64(str)); })));
            }
            return vs;
        }
        #endregion

        #endregion

    }
}
