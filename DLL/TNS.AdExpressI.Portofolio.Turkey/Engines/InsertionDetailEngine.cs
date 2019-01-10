using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.FrameWork.WebResultUI;
using AbstractResult = TNS.AdExpressI.Portofolio.Engines;

namespace TNS.AdExpressI.Portofolio.Turkey.Engines
{
    public class InsertionDetailEngine : AbstractResult.InsertionDetailEngine
    {
        #region Variables
        /// <summary>
        /// Time Slot
        /// </summary>
        private string _timeSlot;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">vehicle Information</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        /// <param name="timeSlot">Time Slot</param>
        /// <param name="dayOfWeek">Day of week</param>
        /// <param name="excel">Excel</param>
        public InsertionDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, string timeSlot, string dayOfWeek, bool excel)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd)
        {
            _timeSlot = timeSlot;
            _dayOfWeek = dayOfWeek;
            _showMediaSchedule = webSession.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) != null ? true : false;
            _excel = excel;
        }
        #endregion

        #region ComputeResultTable
        /// <summary>
        /// Get portofolio media detail insertions 
        /// </summary>
        /// <returns>Result table</returns>
        protected override ResultTable ComputeResultTable()
        {

            #region Variables
            ResultTable tab = null;
            DataSet ds;
            DataTable dt = null;
            Headers headers;
            int iNbLine = 0;
            Assembly assembly;
            Type type;
            bool allPeriod = true;
            bool isDigitalTV = false;
            _vehicle = VehiclesInformation.Get(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);
            #endregion

            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            var parameters = new object[6];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            parameters[5] = _timeSlot;
            if (!string.IsNullOrEmpty(_timeSlot) || !string.IsNullOrEmpty(_dayOfWeek)) _allPeriod = false;
            var portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                , AppDomain.CurrentDomain.BaseDirectory, _module.CountryDataAccessLayer.AssemblyName),
                _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);


            _showTopDiffusion = CanShowTopDiffusion(_vehicleInformation, portofolioDAL);

            #region Product detail level (Generic)
            // Initialisation to product
            var levels = new ArrayList { 10 };
            _webSession.GenericProductDetailLevel = new GenericDetailLevel(levels,
                TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
            #endregion

            #region Columns levels (Generic)
            _columnItemList = WebApplicationParameters.GenericColumnsInformation.GetGenericColumnItemInformationList(_vehicle.DetailColumnId);

            var columnIdList = _columnItemList.Select(column => (int)column.Id).Select(dummy => (long)dummy).ToList();

            _webSession.GenericInsertionColumns = new GenericColumns(columnIdList);
            _webSession.Save();
            #endregion

            #region Data loading
            try
            {
                ds = portofolioDAL.GetInsertionData(); //portofolioDAL.GetGenericDetailMedia();
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {

                    dt = ds.Tables[0];
                }
            }
            catch (System.Exception err)
            {
                throw (new PortofolioException("Error while extracting portofolio media detail data", err));
            }
            #endregion

            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {

                #region Rigths management des droits
                // Show creatives

                if (_webSession.CustomerLogin.ShowCreatives(_vehicleInformation.Id) && _vehicleInformation.ShowCreations) _showCreative = true;
                // Show media agency

                if (_webSession.CustomerLogin.CustomerMediaAgencyFlagAccess(_vehicleInformation.DatabaseId) && dt.Columns.Contains("advertising_agency"))
                    _showMediaAgency = true;

                //Show column product
                _showProduct = _webSession.CustomerLogin.CustormerFlagAccess(AdExpress.Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
                #endregion

                #region Table nb rows
                iNbLine = dt.Rows.Count;
                #endregion

                #region Initialisation of result table
                try
                {
                    headers = new Headers();
                    _columnItemList = WebApplicationParameters.GenericColumnsInformation.GetGenericColumnItemInformationList(_vehicle.DetailColumnId);

                    foreach (GenericColumnItemInformation column in _columnItemList)
                    {

                        switch (column.Id)
                        {
                            case GenericColumnItemInformation.Columns.associatedFile://Visual radio/tv
                            case GenericColumnItemInformation.Columns.visual://Visual press
                                if (_showCreative)
                                    headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(column.WebTextId, _webSession.SiteLanguage), column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.agenceMedia://media agency
                                if (_showMediaAgency)
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(column.WebTextId, _webSession.SiteLanguage), column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.planMedia://Plan media
                                if (_showMediaSchedule)
                                    headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(column.WebTextId, _webSession.SiteLanguage), column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.dateDiffusion:
                            case GenericColumnItemInformation.Columns.dateParution:
                                if (_showDate)
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(column.WebTextId, _webSession.SiteLanguage), column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.topDiffusion:
                                if (_showTopDiffusion)
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(column.WebTextId, _webSession.SiteLanguage), column.WebTextId));
                                break;
                            case GenericColumnItemInformation.Columns.product:
                                if (_showProduct && WebApplicationParameters.GenericColumnsInformation.IsVisible(_vehicle.DetailColumnId, column.Id))
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(column.WebTextId, _webSession.SiteLanguage), column.WebTextId));
                                break;
                            default:
                                if (WebApplicationParameters.GenericColumnsInformation.IsVisible(_vehicle.DetailColumnId, column.Id))
                                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(column.WebTextId, _webSession.SiteLanguage), column.WebTextId));
                                break;
                        }

                    }

                    tab = new ResultTable(iNbLine, headers);
                }
                catch (System.Exception err)
                {
                    throw (new PortofolioException("Error while initiating headers of portofolio media detail", err));
                }
                #endregion

                SetResultTable(dt, tab);

            }

            return tab;
        }

        #endregion

        protected override double GetTopDiffusion(double value)
        {
            if (value.ToString().Length == 6)
            {
                //Hour
                string h = value.ToString().Substring(0, 2);
                // Minutes Seconds
                string ms = value.ToString().Substring(2, 4);

                // For Turkey 25h = 1h
                if (h == "25")
                    return Convert.ToDouble("1" + ms);

                if (h == "24")
                    return Convert.ToDouble(ms);

                return value;
            }

            return value;
        }

        protected override int GetTvId()
        {
            return Convert.ToInt32(VehiclesInformation.EnumToDatabaseId(Vehicles.names.tv));
        }

        /// <summary>
        /// Get column value depending on if the value has a separator or not
        /// </summary>
        /// <param name="column">Column</param>
        /// <param name="value">Value of the cell</param>
        /// <returns>Value</returns>
        /// <remarks>We have add this method to solve cobranding problem for Russia</remarks>
        protected override object GetColumnValue(GenericColumnItemInformation column, object value)
        {

            string s = string.Empty;
            if (column.IsContainsSeparator)
            {
                if (value != null)
                    s = SplitStringValue(value.ToString());
                else
                    s = string.Empty;
                return s;
            }

            if (column.CellType.Equals("TNS.FrameWork.WebResultUI.CellNumber") && value == DBNull.Value)
                return 0;

            return value;

        }
    }
}
