using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.FrameWork.WebResultUI;
using AbstractResult = TNS.AdExpressI.Portofolio;
using FrameWorkResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results;

namespace TNS.AdExpressI.Portofolio.Turkey
{
    public class PortofolioResults : AbstractResult.PortofolioResults
    {
        #region Variables
        /// <summary>
        /// Time Slot
        /// </summary>
        protected string _timeSlot;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        public PortofolioResults(WebSession webSession) : base(webSession) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="timeSlot">Time Slot</param>
        /// <param name="dayOfWeek">Day of week</param>
        public PortofolioResults(WebSession webSession, string timeSlot, string dayOfWeek)
            : this(webSession)
        {
            _timeSlot = timeSlot;
            _dayOfWeek = dayOfWeek;
        }
        #endregion

        #region Implementation of abstract methods
        /// <summary>
        /// Get ResultTable for some portofolio result
        ///  - DETAIL_PORTOFOLIO
        ///  - CALENDAR
        ///  - SYNTHESIS (only result table)
        /// </summary>
        /// <returns>Result Table</returns>
        public override ResultTable GetResultTable()
        {
            Engines.SynthesisEngine result = null;
            try
            {
                switch (_webSession.CurrentTab)
                {
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
                        return base.GetResultTable();
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
                        return GetStructureEngine(true).GetResultTable();
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
                        return new Engines.PortofolioDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, _showInsertions, _showCreatives).GetResultTable();
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:
                        new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd).GetResultTable();
                        break;
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
                        result = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
                        break;
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.PROGRAM_TYPOLOGY_BREAKDOWN:
                        return new Engines.BreakdownEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, false, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.programTypology)).GetResultTable();
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.PROGRAM_BREAKDOWN:
                        return new Engines.BreakdownEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, false, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.program)).GetResultTable();
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.SUBTYPE_SPOTS_BREAKDOWN:
                        return new Engines.BreakdownEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, false, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.spotSubType)).GetResultTable();
                    default:
                        throw (new PortofolioException("Impossible to identified current tab "));
                }
            }
            catch (System.Exception err)
            {
                throw (new PortofolioException("Impossible to compute portofolio results", err));
            }

            return result.GetResultTable();
        }

        public override GridResult GetGridResult()
        {
            if (_webSession.CurrentTab != AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS)
            {
                GridResult gridResult = new GridResult();
                long nbRows = CountDataRows();
                if (HandleGridMaxRows(nbRows, gridResult)) return gridResult;
            }
          
            return base.GetGridResult();
        }
        #endregion

        public override GridResult GetBreakdownGridResult(bool excel, DetailLevelItemInformation level)
        {
            return GetBreakdownEngine(excel, level).GetGridResult();
        }

        public Engines.BreakdownEngine GetBreakdownEngine(bool excel, DetailLevelItemInformation level)
        {
            Engines.BreakdownEngine result = null;
            result = new Engines.BreakdownEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, excel, level);
            return result;
        }

        public override GridResult GetDetailMediaGridResult(bool excel)
        {
            Engines.MediaDetailEngine result = new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, excel);
            StringBuilder t = new StringBuilder(5000);

            GridResult gridResult = null;

            switch (_vehicleInformation.Id)
            {
                case AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                    gridResult = result.GetAllPeriodSpotsGridResult(GestionWeb.GetWebWord(1836, _webSession.SiteLanguage));
                    return gridResult;
                default:
                    throw new PortofolioException("Vehicle unknown.");
            }
        }

        public override GridResult GetDetailMediaPopUpGridResult()
        {
            GridResult gridResult = new GridResult();
            gridResult.HasData = false;

            ResultTable data = GetInsertionDetailResultTable(false);

            if (data != null)
            {
                ComputeGridData(gridResult, data);
            }
            else
            {
                gridResult.HasData = false;
                return (gridResult);
            }

            return gridResult;

        }

        public override GridResult GetStructureGridResult(bool excel)
        {
            return GetStructureEngine(excel).GetGridResult();
        }

        public override Portofolio.Engines.StructureEngine GetStructureEngine(bool excel)
        {
            Portofolio.Engines.StructureEngine result = null;
            switch (_vehicleInformation.Id)
            {
                case AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.radioGeneral:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.radioSponsorship:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.radioMusic:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.others:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.tvGeneral:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.tvSponsorship:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.tvNonTerrestrials:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.tvAnnounces:
                    Dictionary<string, double> hourBeginningList = new Dictionary<string, double>();
                    Dictionary<string, double> hourEndList = new Dictionary<string, double>();
                    GetHourIntevalList(hourBeginningList, hourEndList);
                    result = new Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, hourBeginningList, hourEndList, excel);
                    break;
                default:
                    throw new PortofolioException("Vehicle unknown.");
            }
            return result;
        }

        /// <summary>
        /// Get structure chart data
        /// </summary>
        /// <returns></returns>
        public override DataTable GetStructureChartData()
        {
            Portofolio.Engines.StructureEngine result = null;

            switch (_vehicleInformation.Id)
            {
                case AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.radioGeneral:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.radioSponsorship:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.radioMusic:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.others:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.tvGeneral:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.tvSponsorship:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.tvNonTerrestrials:
                case AdExpress.Constantes.Classification.DB.Vehicles.names.tvAnnounces:
                    Dictionary<string, double> hourBeginningList = new Dictionary<string, double>();
                    Dictionary<string, double> hourEndList = new Dictionary<string, double>();
                    GetHourIntevalList(hourBeginningList, hourEndList);
                    result = new Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, hourBeginningList, hourEndList, false);
                    break;
                default:
                    throw new PortofolioException("Vehicle unknown.");
            }
            return result.GetChartData();
        }

        #region Insetion detail
        /// <summary>
        /// Get media insertion detail
        /// </summary>
        /// <returns></returns>
        public override ResultTable GetInsertionDetailResultTable(bool excel)
        {
            Engines.InsertionDetailEngine result = new Engines.InsertionDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, _timeSlot, _dayOfWeek, excel);
            return result.GetResultTable();
        }
        #endregion

        protected override void ComputeGridData(GridResult gridResult, ResultTable data)
        {
            data.Sort(ResultTable.SortOrder.NONE, 1); //Important, pour hierarchie du tableau Infragistics
            data.CultureInfo = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo;

            int i, j, k;
            //int creativeIndexInResultTable = -1;
            object[,] gridData = new object[data.LinesNumber, data.ColumnsNumber + 2]; //+2 car ID et PID en plus  -  //_data.LinesNumber
            List<object> columns = new List<object>();
            List<object> schemaFields = new List<object>();
            List<object> columnsFixed = new List<object>();


            columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "ID" });
            columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "PID" });

            if (data.NewHeaders != null)
            {
                for (j = 0; j < data.NewHeaders.Root.Count; j++)
                {
                    //Key pour "Spot" = 869
                    //key pour "Plan Media du produit" = 1478
                    //Key pour "Visuel" = 1909

                    columns.Add(new { headerText = data.NewHeaders.Root[j].Label, key = data.NewHeaders.Root[j].Key, dataType = "string", width = "*" });
                    schemaFields.Add(new { name = data.NewHeaders.Root[j].Key });
                    columnsFixed.Add(new { columnKey = data.NewHeaders.Root[j].Key, isFixed = true, allowFixing = true });
                }
            }
            else
            {
                columns.Add(new { headerText = "", key = "Visu", dataType = "string", width = "*" });
                schemaFields.Add(new { name = "Visu" });
            }

            try
            {
                for (i = 0; i < data.LinesNumber; i++) //_data.LinesNumber
                {
                    gridData[i, 0] = i; // Pour column ID
                    gridData[i, 1] = data.GetSortedParentIndex(i); // Pour column PID

                    for (k = 1; k < data.ColumnsNumber - 1; k++)
                    {
                        gridData[i, k + 1] = data[i, k].RenderString();
                    }
                }
            }
            catch (Exception err)
            {
                throw (new Exception(err.Message));
            }

            //if (columns.Count > 10)
            //    gridResult.NeedFixedColumns = true;
            gridResult.NeedFixedColumns = false;

            gridResult.HasData = true;
            gridResult.Columns = columns;
            gridResult.Schema = schemaFields;
            gridResult.ColumnsFixed = columnsFixed;
            gridResult.Data = gridData;
        }

        protected override void SetSort(ref GridResult gridResult, ref ResultTable resultTable, List<int> indexInResultTableAllowSortingList)
        {
            if (_webSession.CurrentTab == FrameWorkResultConstantes.Portofolio.SYNTHESIS
                    || string.IsNullOrEmpty(_webSession.SortKey) ||
               (!string.IsNullOrEmpty(_webSession.SortKey) && !indexInResultTableAllowSortingList.Contains(Convert.ToInt32(_webSession.SortKey))))
            {
                if (_webSession.CurrentTab != FrameWorkResultConstantes.Portofolio.SYNTHESIS)
                    resultTable.Sort(ResultTable.SortOrder.NONE, 1); //Important, pour hierarchie du tableau Infragistics
                gridResult.SortOrder = ResultTable.SortOrder.NONE.GetHashCode();
                gridResult.SortKey = 1;
                _webSession.Sorting = ResultTable.SortOrder.NONE;
                _webSession.SortKey = "1";
                _webSession.Save();
            }
            else
            {
                resultTable.Sort(_webSession.Sorting, Convert.ToInt32(_webSession.SortKey)); //Important, pour hierarchie du tableau Infragistics
                gridResult.SortOrder = _webSession.Sorting.GetHashCode();
                gridResult.SortKey = Convert.ToInt32(_webSession.SortKey);
            }
        }

        /// <summary>
        /// Get hour interval list
        /// </summary>
        /// <param name="hourBeginningList">hour begininng list</param>
        /// <param name="hourEndList">hour end list</param>
        protected override void GetHourIntevalList(Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList)
        {
            if (hourBeginningList == null) hourBeginningList = new Dictionary<string, double>();
            if (hourEndList == null) hourEndList = new Dictionary<string, double>();

            switch (_vehicleInformation.Id)
            {
                case AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                    hourBeginningList.Add("0007", 240000); hourEndList.Add("0007", 70000);
                    hourBeginningList.Add("0712", 70000); hourEndList.Add("0712", 120000);
                    hourBeginningList.Add("1214", 120000); hourEndList.Add("1214", 140000);
                    hourBeginningList.Add("1417", 140000); hourEndList.Add("1417", 170000);
                    hourBeginningList.Add("1719", 170000); hourEndList.Add("1719", 190000);
                    hourBeginningList.Add("1922", 190000); hourEndList.Add("1922", 220000);
                    hourBeginningList.Add("2224", 220000); hourEndList.Add("2224", 240000);
                    break;
                default:
                    throw new PortofolioException("GetHourIntevalList(): Vehicle unknown.");
            }
        }

        private bool HandleGridMaxRows(long nbRows, GridResult gridResult)
        {

            if (nbRows == 0)
            {
                gridResult.HasData = false;
                return true;

            }
            if (nbRows > AdExpress.Constantes.Web.Core.MAX_ALLOWED_DATA_ROWS)
            {
                gridResult.HasData = true;
                gridResult.HasMoreThanMaxRowsAllowed = true;
                return true;

            }
            return false;
        }

        public override long CountDataRows()
        {
            Engines.SynthesisEngine result = null;
           
            try
            {
                switch (_webSession.CurrentTab)
                {
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
                        return new Engines.CalendarEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd).CountData(); 
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
                        return GetStructureEngine(true).CountData();
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
                        return  new Engines.PortofolioDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, _showInsertions, _showCreatives).CountData();
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:
                        return new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd).CountData();                       
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
                        result = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
                        break;
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.PROGRAM_TYPOLOGY_BREAKDOWN:
                        return new Engines.BreakdownEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, false, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.programTypology)).CountData();
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.PROGRAM_BREAKDOWN:
                        return new Engines.BreakdownEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, false, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.program)).CountData();
                    case AdExpress.Constantes.FrameWork.Results.Portofolio.SUBTYPE_SPOTS_BREAKDOWN:
                        return new Engines.BreakdownEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, false, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.spotSubType)).CountData();
                    default:
                        throw (new PortofolioException("Impossible to identified current tab "));
                }
            }
            catch (System.Exception err)
            {
                throw (new PortofolioException("Impossible to compute portofolio results", err));
            }

            return result.CountData();
        }
    }
}
