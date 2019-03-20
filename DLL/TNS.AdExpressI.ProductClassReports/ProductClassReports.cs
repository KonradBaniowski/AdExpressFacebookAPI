#region Information
/*
 * Author : G Ragneau
 * Created on : 30/06/2008
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;


using TNS.AdExpressI.ProductClassReports.Exceptions;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Classification;

using CstTblFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables;
using TNS.Classification.Universe;
using TNS.AdExpressI.ProductClassReports.Engines;
using System.Data;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpressI.ProductClassReports.GenericEngines;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Units;
using WebCst = TNS.AdExpress.Constantes.Web;
using FctWeb = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.Core.Result;

namespace TNS.AdExpressI.ProductClassReports
{
    /// <summary>
    /// Implements default results of the Product Class Analysis.
    /// </summary>
    public abstract class ProductClassReports : IProductClassReports
    {

        #region Attributes
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Type of result
        /// </summary>
        protected int _tableType;
        /// <summary>
        /// Report engine
        /// </summary>
        protected Engine _engine = null;
        /// <summary>
        /// Generic report engine
        /// </summary>
        protected GenericEngine _genericEngine = null;
        #endregion

        #region Accessors
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession Session
        {
            get { return _session; }
            set { _session = value; }
        }
        /// <summary>
        /// Type of result
        /// </summary>
        protected int TableType
        {
            get { return _tableType; }
            set { _tableType = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassReports(WebSession session)
        {
            _session = session;
        }
        #endregion

        #region IProductClassReports Membres
        /// <summary>
        /// Compute Product Class Report depending on type or fesult specified in user session (HTML code)
        /// </summary>
        /// <returns>HTML Code</returns>
        public string GetProductClassReport()
        {
            return GetProductClassReport((int)_session.PreformatedTable);
        }
        /// <summary>
        /// Compute Product Class Report matching the "resultType" param (HTML code)
        /// </summary>
        /// <param name="resultType">Type of result</param>
        /// <exception cref="TNS.AdExpressI.ProductClassReports.Exceptions.NotImplementedReportException">Thrown when the invoked report is not implemented.</exception>
        /// <returns>HTML Code</returns>
        public string GetProductClassReport(int resultType)
        {
            return this.GetProductClassReport(resultType, false);
        }
        /// <summary>
        /// Compute Product Class Report depending on type or fesult specified in user session (HTML code)
        /// </summary>
        /// <returns>HTML Code</returns>
        public string GetProductClassReportExcel()
        {
            return GetProductClassReportExcel((int)_session.PreformatedTable);
        }
        /// <summary>
        /// Compute Product Class Report matching the "resultType" param (HTML code)
        /// </summary>
        /// <param name="resultType">Type of result</param>
        /// <exception cref="TNS.AdExpressI.ProductClassReports.Exceptions.NotImplementedReportException">Thrown when the invoked report is not implemented.</exception>
        /// <returns>HTML Code</returns>
        public string GetProductClassReportExcel(int resultType)
        {
            return this.GetProductClassReport(resultType, true);
        }
        #endregion

        #region IProductClassReports Generic Membres
        /// <summary>
        /// Compute Product Class Report depending on type or fesult specified in user session
        /// </summary>
        /// <returns>Data Result</returns>
        public ResultTable GetGenericProductClassReport()
        {
            return GetGenericProductClassReport((int)_session.PreformatedTable);
        }
        /// <summary>
        /// Compute Product Class Report matching the "resultType" param
        /// </summary>
        /// <param name="resultType">Type of result</param>
        /// <exception cref="TNS.AdExpressI.ProductClassReports.Exceptions.NotImplementedReportException">Thrown when the invoked report is not implemented.</exception>
        /// <returns>Data Result</returns>
        public ResultTable GetGenericProductClassReport(int resultType)
        {
            return this.GetGenericProductClassReport(resultType, false);
        }
        /// <summary>
        /// Compute Product Class Report depending on type or fesult specified in user session
        /// </summary>
        /// <returns>Data Result</returns>
        public ResultTable GetGenericProductClassReportExcel()
        {
            return GetGenericProductClassReportExcel((int)_session.PreformatedTable);
        }
        /// <summary>
        /// Compute Product Class Report matching the "resultType" param
        /// </summary>
        /// <param name="resultType">Type of result</param>
        /// <exception cref="TNS.AdExpressI.ProductClassReports.Exceptions.NotImplementedReportException">Thrown when the invoked report is not implemented.</exception>
        /// <returns>Data Result</returns>
        public ResultTable GetGenericProductClassReportExcel(int resultType)
        {
            return this.GetGenericProductClassReport(resultType, true);
        }

        /// <summary>
        /// Determine if module bridge can be show
        /// </summary>
        /// <returns>True if module bridge can be show</returns>
        public bool ShowModuleBridge()
        {
            List<Int64> ids = null;
            if (_session.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR) != null)
                ids = ((Module)_session.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR)).ExcludedVehicles;

            //Rule : Not show module brige if media selected is only Marketing direct
            if (ids != null && ids.Count > 0)
            {
                VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID);
                if (vehicleInfo != null && vehicleInfo.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing)
                    return false;
            }
            return true;
        }

        #endregion

        #region GetProductClassReport
        /// <summary>
        /// Compute Product Class Report matching the "resultType" param (HTML code)
        /// </summary>
        /// <param name="resultType">Type of result</param>
        /// <param name="excel">Report as Excel output ? </param>
        /// <exception cref="TNS.AdExpressI.ProductClassReports.Exceptions.NotImplementedReportException">Thrown when the invoked report is not implemented.</exception>
        /// <returns>HTML Code</returns>
        protected virtual string GetProductClassReport(int resultType, bool excel)
        {
            switch (resultType)
            {
                case (int)CstTblFormat.media_X_Year:
                case (int)CstTblFormat.product_X_Year:
                    _engine = new Engine_Classif1_X_Year(_session, resultType);
                    break;
                case (int)CstTblFormat.productMedia_X_Year:
                case (int)CstTblFormat.mediaProduct_X_Year:
                    _engine = new Engine_Classif1Classif2_X_Years(_session, resultType);
                    break;
                case (int)CstTblFormat.productMedia_X_YearMensual:
                case (int)CstTblFormat.mediaProduct_X_YearMensual:
                    _engine = new Engine_Classif1Classif2_X_Monthes(_session, resultType);
                    break;
                case (int)CstTblFormat.productYear_X_Media:
                    _engine = new Engine_Classif1Year_X_Classif2(_session, resultType);
                    break;
                case (int)CstTblFormat.mediaYear_X_Mensual:
                case (int)CstTblFormat.mediaYear_X_Cumul:
                case (int)CstTblFormat.productYear_X_Cumul:
                case (int)CstTblFormat.productYear_X_Mensual:
                    _engine = new Engine_Classif1Year_X_Monthes(_session, resultType);
                    break;
                default:
                    throw new NotImplementedReportException(string.Format("Tableau {0} ({1}) is not implemented.", _session.PreformatedTable, _session.PreformatedTable.GetHashCode()));
            }
            _engine.Excel = excel;
            return _engine.GetResult().ToString();
        }
        #endregion

        #region Generic GetProductClassReport
        /// <summary>
        /// Compute Product Class Report matching the "resultType" param
        /// </summary>
        /// <param name="resultType">Type of result</param>
        /// <param name="excel">Report as Excel output ? </param>
        /// <exception cref="TNS.AdExpressI.ProductClassReports.Exceptions.NotImplementedReportException">Thrown when the invoked report is not implemented.</exception>
        /// <returns>Data Result</returns>
        virtual protected ResultTable GetGenericProductClassReport(int resultType, bool excel)
        {
            switch (resultType)
            {
                case (int)CstTblFormat.media_X_Year:
                case (int)CstTblFormat.product_X_Year:
                    _genericEngine = new GenericEngine_Classif1_X_Year(_session, resultType);
                    break;
                case (int)CstTblFormat.productMedia_X_Year:
                case (int)CstTblFormat.mediaProduct_X_Year:
                    _genericEngine = new GenericEngine_Classif1Classif2_X_Years(_session, resultType);
                    break;
                case (int)CstTblFormat.productMedia_X_YearMensual:
                case (int)CstTblFormat.mediaProduct_X_YearMensual:
                    _genericEngine = new GenericEngine_Classif1Classif2_X_Monthes(_session, resultType);
                    break;
                case (int)CstTblFormat.productYear_X_Media:
                    _genericEngine = new GenericEngine_Classif1Year_X_Classif2(_session, resultType);
                    break;
                case (int)CstTblFormat.mediaYear_X_Mensual:
                case (int)CstTblFormat.productYear_X_Mensual:
                    _genericEngine = new GenericEngine_Classif1Year_X_Monthes(_session, resultType, true);
                    break;
                case (int)CstTblFormat.mediaYear_X_Cumul:
                case (int)CstTblFormat.productYear_X_Cumul:
                    _genericEngine = new GenericEngine_Classif1Year_X_Monthes(_session, resultType);
                    break;
                default:
                    throw new NotImplementedReportException(string.Format("Tableau {0} ({1}) is not implemented.", _session.PreformatedTable, _session.PreformatedTable.GetHashCode()));
            }
            _genericEngine.Excel = excel;
            var result = _genericEngine.GetResult();
            return result;
        }
        #endregion

        #region Get Grid Result
        public GridResult GetGridResult()
        {
            ResultTable resultTable = GetGenericProductClassReport();
            GridResult gridResult = new GridResult();

            if (resultTable != null && resultTable.LinesNumber>0)
            {
                if (resultTable.LinesNumber > WebCst.Core.MAX_ALLOWED_ROWS_NB)
                {
                    gridResult.HasData = true;
                    gridResult.HasMoreThanMaxRowsAllowed = true;
                    return (gridResult);
                }

                resultTable.CultureInfo = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
                object[,] gridData = new object[resultTable.LinesNumber, resultTable.ColumnsNumber]; //+2 car ID et PID en plus  -  //_data.LinesNumber
                List<object> columns = new List<object>();
                List<object> schemaFields = new List<object>();
                List<object> columnsFixed = new List<object>();
                List<int> indexInResultTableAllowSortingList = new List<int>();

                //Hierachical ids for Treegrid
                columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
                schemaFields.Add(new { name = "ID" });
                columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
                schemaFields.Add(new { name = "PID" });
                List<object> groups = null;
                string colKey = string.Empty;

                gridResult.HasData = true;

                if (resultTable.NewHeaders != null)
                {
                    for (int j = 0; j < resultTable.NewHeaders.Root.Count; j++)
                    {
                        groups = null;
                        colKey = string.Empty;
                        if (resultTable.NewHeaders.Root[j].Count > 0)
                        {
                            groups = new List<object>();

                            int nbGroupItems = resultTable.NewHeaders.Root[j].Count;
                            for (int g = 0; g < nbGroupItems; g++)
                            {
                                colKey = string.Format("g{0}", resultTable.NewHeaders.Root[j][g].IndexInResultTable);

                                var cell = resultTable[0, resultTable.NewHeaders.Root[j][g].IndexInResultTable];

                                if (cell is CellEvol)
                                    groups.Add(new { headerText = resultTable.NewHeaders.Root[j][g].Label, key = colKey, dataType = "string", width = "90", columnCssClass = "colStyle" });
                                else
                                    groups.Add(new { headerText = resultTable.NewHeaders.Root[j][g].Label, key = colKey, dataType = "string", width = "*", columnCssClass = "colStyle" });

                                indexInResultTableAllowSortingList.Add(resultTable.NewHeaders.Root[j][g].IndexInResultTable);
                                schemaFields.Add(new { name = colKey });
                            }
                            columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = "_gr" + colKey, group = groups });
                            columnsFixed.Add(new { columnKey = "_gr" + colKey, isFixed = false, allowFixing = false });
                        }
                        else
                        {
                            colKey = string.Format("g{0}", resultTable.NewHeaders.Root[j].IndexInResultTable);
                            if (j == 0)
                            {
                                var cell = resultTable[0, resultTable.NewHeaders.Root[j].IndexInResultTable];
                                columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = colKey, dataType = "string", width = "350" });

                                indexInResultTableAllowSortingList.Add(resultTable.NewHeaders.Root[j].IndexInResultTable);
                                columnsFixed.Add(new { columnKey = colKey, isFixed = true, allowFixing = false });
                            }
                            else
                            {
                                var cell = resultTable[0, resultTable.NewHeaders.Root[j].IndexInResultTable];
                                if (cell is CellEvol)
                                    columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = colKey, dataType = "string", width = "90", columnCssClass = "colStyle" });
                                else
                                    columns.Add(new { headerText = resultTable.NewHeaders.Root[j].Label, key = colKey, dataType = "string", width = "*", columnCssClass = "colStyle" });

                                indexInResultTableAllowSortingList.Add(resultTable.NewHeaders.Root[j].IndexInResultTable);
                                columnsFixed.Add(new { columnKey = colKey, isFixed = false, allowFixing = false });
                            }
                            schemaFields.Add(new { name = colKey });
                        }
                    }
                }

                if (string.IsNullOrEmpty(_session.SortKey) ||
                (!string.IsNullOrEmpty(_session.SortKey) && !indexInResultTableAllowSortingList.Contains(Convert.ToInt32(_session.SortKey))))
                {
                    resultTable.Sort(ResultTable.SortOrder.NONE, 1); //Important, pour hierarchie du tableau Infragistics
                    gridResult.SortOrder = ResultTable.SortOrder.NONE.GetHashCode();
                    gridResult.SortKey = 1;
                    _session.Sorting = ResultTable.SortOrder.NONE;
                    _session.SortKey = "1";
                    _session.Save();
                }
                else
                {
                    resultTable.Sort(_session.Sorting, Convert.ToInt32(_session.SortKey)); //Important, pour hierarchie du tableau Infragistics
                    gridResult.SortOrder = _session.Sorting.GetHashCode();
                    gridResult.SortKey = Convert.ToInt32(_session.SortKey);
                }

                //table body rows
                for (int i = 0; i < resultTable.LinesNumber; i++) //_data.LinesNumber
                {
                    gridData[i, 0] = i; // Pour column ID
                    gridData[i, 1] = resultTable.GetSortedParentIndex(i); // Pour column PID

                    for (int k = 1; k < resultTable.ColumnsNumber - 1; k++)
                    {
                        var cell = resultTable[i, k];

                        if (cell is CellEvol)
                        {
                            gridData[i, k + 1] = cell.Render().Replace("/I", "../Content/Img").Replace("<td>","").Replace("</td>", "");
                        }
                        else if (cell is CellPDM || cell is CellPDV)
                        {
                            double value = ((CellUnit)cell).Value;

                            if (value == 0)
                                gridData[i, k + 1] = "";
                            else
                                gridData[i, k + 1] = cell.RenderString();
                        }
                        else if (cell is CellUnit)
                        {
                            if (cell.RenderString().Equals("0"))
                                gridData[i, k + 1] = "";
                            else
                                gridData[i, k + 1] = cell.RenderString();
                        }
                        else
                            gridData[i, k + 1] = cell.RenderString();
                    }
                }

                gridResult.NeedFixedColumns = true;
                gridResult.HasData = true;
                gridResult.Columns = columns;
                gridResult.Schema = schemaFields;
                gridResult.ColumnsFixed = columnsFixed;
                gridResult.Data = gridData;
            }
            else
            {
                gridResult.HasData = false;
            }

            return gridResult;
        }

        private object GetColumnDef(ICell cell, string headerText, ref string key, string width)
        {

            AdExpressCultureInfo cInfo = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

            //if (cell is CellPercent)
            //    return new { headerText = headerText, key = key, dataType = "number", format = "percent", columnCssClass = "colStyle", width = width, allowSorting = true };
            //else if (cell is CellEvol)
            //{
            //    key += "-evol";
            //    return new { headerText = headerText, key = key, dataType = "number", format = "percent", columnCssClass = "colStyle", width = width, allowSorting = true };
            //}
            //else if (cell is CellPDM)
            //{
            //    key += "-pdm";
            //    return new { headerText = headerText, key = key, dataType = "number", format = "percent", columnCssClass = "colStyle", width = width, allowSorting = true };
            //}
            //else if (cell is CellDuration)
            //{
            //    key += "-unit-duration";
            //    return new { headerText = headerText, key = key, dataType = "number", format = "duration", columnCssClass = "colStyle", width = width, allowSorting = true };
            //}
            //else if (cell is CellInsertion)
            //{
            //    string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(WebCst.CustomerSessions.Unit.insertion).StringFormat);
            //    key += "-unit-insertion";
            //    return new { headerText = headerText, key = key, dataType = "number", format = format, columnCssClass = "colStyle", width = width, allowSorting = true };
            //}
            //else if (cell is CellMMC)
            //{
            //    string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(WebCst.CustomerSessions.Unit.mmPerCol).StringFormat);
            //    key += "-unit-mmPerCol";
            //    return new { headerText = headerText, key = key, dataType = "number", format = format, columnCssClass = "colStyle", width = width, allowSorting = true };
            //}
            //else if (cell is CellPage)
            //{
            //    string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(WebCst.CustomerSessions.Unit.pages).StringFormat);
            //    key += "-unit-pages";
            //    return new { headerText = headerText, key = key, dataType = "number", format = format, columnCssClass = "colStyle", width = width, allowSorting = true };
            //}
            //else if (cell is CellEuro)
            //{
            //    string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(WebCst.CustomerSessions.Unit.euro).StringFormat);
            //    key += "-unit-euro";
            //    return new { headerText = headerText, key = key, dataType = "number", format = format, columnCssClass = "colStyle", width = width, allowSorting = true };
            //}
            //else if (cell is CellKEuro)
            //{
            //    string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(WebCst.CustomerSessions.Unit.kEuro).StringFormat);
            //    key += "-unit-euro";
            //    return new { headerText = headerText, key = key, dataType = "number", format = format, columnCssClass = "colStyle", width = width, allowSorting = true };
            //}
            //else if (cell is CellGRP)
            //{
            //    string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(WebCst.CustomerSessions.Unit.grp).StringFormat);
            //    key += "-unit-euro";
            //    return new { headerText = headerText, key = key, dataType = "number", format = format, columnCssClass = "colStyle", width = width, allowSorting = true };
            //}
            //else if (cell is CellVolume)
            //{
            //    string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(WebCst.CustomerSessions.Unit.volume).StringFormat);
            //    key += "-unit-euro";
            //    return new { headerText = headerText, key = key, dataType = "number", format = format, columnCssClass = "colStyle", width = width, allowSorting = true };
            //}
            //else
                return new { headerText = headerText, key = key, dataType = "string", width = width };
        }

        private WebCst.CustomerSessions.Unit GetUnit(ICell cell)
        {
            if (cell is CellDuration)
                return WebCst.CustomerSessions.Unit.duration;
            else if (cell is CellEuro)
                return WebCst.CustomerSessions.Unit.euro;
            else if (cell is CellGRP)
                return WebCst.CustomerSessions.Unit.grp;
            else if (cell is CellInsertion)
                return WebCst.CustomerSessions.Unit.insertion;
            else if (cell is CellKEuro)
                return WebCst.CustomerSessions.Unit.kEuro;
            else if (cell is CellMMC)
                return WebCst.CustomerSessions.Unit.mmPerCol;
            else if (cell is CellPage)
                return WebCst.CustomerSessions.Unit.pages;
            else if (cell is CellVolume)
                return WebCst.CustomerSessions.Unit.volume;
            else
                return WebCst.CustomerSessions.Unit.none;
        }
        #endregion
    }
}
