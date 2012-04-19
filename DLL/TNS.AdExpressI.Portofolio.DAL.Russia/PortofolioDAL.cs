#region Information
// Author: Y. Rkaina && D. Mussuma
// Creation date: 08/08/2008
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Data.SqlClient;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes = TNS.AdExpress.Constantes.DB;

using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Classification;

using TNS.AdExpress.Constantes.FrameWork.Results;

using CustomerRightType = TNS.AdExpress.Constantes.Customer.Right.type;
using UniverseAccessType = TNS.Classification.Universe.AccessType;
using TNS.AdExpressI.Common.DAL.Russia;

namespace TNS.AdExpressI.Portofolio.DAL.Russia
{
    /// <summary>
    /// Portofolio Data Access Layer
    /// </summary>
    public class PortofolioDAL : TNS.AdExpressI.Portofolio.DAL.PortofolioDAL
    {
        CommonDAL common;

        public CommonDAL Common
        {
            get { return common; }
        }

        #region Special methods and data

        /// <summary>
        /// rp_ax_vportfolio_summary_get
        /// </summary>
        private const string GetSummaryDataProcedureName = "[dbo].[rp_ax_vportfolio_summary_get]";

        /// <summary>
        /// rp_ax_vportfolio_breakdown_get
        /// </summary>
        private const string GetBreakdownDataProcedureName = "[dbo].[rp_ax_vportfolio_breakdown_get]";


        /// <summary>
        /// rp_ax_vportfolio_calendar_get
        /// </summary>
        private const string GetCalendarDataProcedureName = "[dbo].[rp_ax_vportfolio_calendar_get]";

        /// <summary>
        /// rp_ax_vportfolio_portfolio_get
        /// </summary>
        private const string GetPortfolioDataProcedureName = "[dbo].[rp_ax_vportfolio_portfolio_get]";

        /// <summary>
        /// rp_ax_vportfolio_insertion_get
        /// </summary>
        private const string GetInsertionDataProcedureName = "[dbo].[rp_ax_vportfolio_insertion_get]";

        /// <summary>
        /// rp_ax_vportfolio_prvisual_get
        /// </summary>
        private const string GetPressVisualDataProcedureName = "[dbo].[rp_ax_vportfolio_prvisual_get]";

        /// <summary>
        /// rp_ax_vportfolio_prissue_get
        /// </summary>
        private const string GetPressIssueDataProcedureName = "[dbo].[rp_ax_vportfolio_prissue_get]";

        private int SummaryDataEntryPoint = 0;
        private DataSet dsSummary = new DataSet();

        protected TNS.FrameWork.DB.Common.IDataSource GetDataSource()
        {
            TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.sourceProvider];
            object[] param = new object[1];
            param[0] = _webSession;
            if (cl == null) throw (new NullReferenceException("Core layer is null for the source provider layer"));
            TNS.AdExpress.Web.Core.ISourceProvider sourceProvider = (TNS.AdExpress.Web.Core.SourceProvider)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            return sourceProvider.GetSource();
        }

        public void GetRightSelectParams(ref SqlCommand cmd)
        {
            MediaRight mediaRight = Common.GetMediaRight();
            ProductRight productRight = Common.GetProductRight();
            ProductClassification productClassification = Common.GetProductClassification();
            AdTypeClassification adTypes = Common.GetAdTypeClassification();

            cmd.Parameters.AddWithValue("@customer_media_access", Common.IsNull(mediaRight.MediaRightMediaAccess));
            cmd.Parameters.AddWithValue("@customer_media_except", Common.IsNull(mediaRight.MediaRightMediaExcept));
            cmd.Parameters.AddWithValue("@customer_vehicle_access", Common.IsNull(mediaRight.MediaRightVehicleAccess));
            cmd.Parameters.AddWithValue("@customer_vehicle_except", Common.IsNull(mediaRight.MediaRightVehicleExcept));
            cmd.Parameters.AddWithValue("@customer_region_access", Common.IsNull(mediaRight.MediaRightRegionAccess));
            cmd.Parameters.AddWithValue("@customer_region_except", Common.IsNull(mediaRight.MediaRightRegionExcept));

            cmd.Parameters.AddWithValue("@customer_sector_access", Common.IsNull(productRight.ProductRightCategory1Access));
            cmd.Parameters.AddWithValue("@customer_sector_except", Common.IsNull(productRight.ProductRightCategory1Except));
            cmd.Parameters.AddWithValue("@customer_subsector_access", Common.IsNull(productRight.ProductRightCategory2Access));
            cmd.Parameters.AddWithValue("@customer_subsector_except", Common.IsNull(productRight.ProductRightCategory2Except));
            cmd.Parameters.AddWithValue("@customer_group_access", Common.IsNull(productRight.ProductRightCategory3Access));
            cmd.Parameters.AddWithValue("@customer_group_except", Common.IsNull(productRight.ProductRightCategory3Except));
            cmd.Parameters.AddWithValue("@customer_subgroup_access", Common.IsNull(productRight.ProductRightCategory4Access));
            cmd.Parameters.AddWithValue("@customer_subgroup_except", Common.IsNull(productRight.ProductRightCategory4Except));
            cmd.Parameters.AddWithValue("@customer_advertiser_access", Common.IsNull(productRight.ProductRightAdvertiserAccess));
            cmd.Parameters.AddWithValue("@customer_advertiser_except", Common.IsNull(productRight.ProductRightAdvertiserExcept));
            cmd.Parameters.AddWithValue("@customer_brand_access", Common.IsNull(productRight.ProductRightBrandAccess));
            cmd.Parameters.AddWithValue("@customer_brand_except", Common.IsNull(productRight.ProductRightBrandExcept));

            cmd.Parameters.AddWithValue("@selected_sector_access1", Common.IsNull(productClassification.Category1Access1));
            cmd.Parameters.AddWithValue("@selected_sector_access2", Common.IsNull(productClassification.Category1Access2));
            cmd.Parameters.AddWithValue("@selected_sector_except", Common.IsNull(productClassification.Category1Except));
            cmd.Parameters.AddWithValue("@selected_subsector_access1", Common.IsNull(productClassification.Category2Access1));
            cmd.Parameters.AddWithValue("@selected_subsector_access2", Common.IsNull(productClassification.Category2Access2));
            cmd.Parameters.AddWithValue("@selected_subsector_except", Common.IsNull(productClassification.Category2Except));
            cmd.Parameters.AddWithValue("@selected_group_access1", Common.IsNull(productClassification.Category3Access1));
            cmd.Parameters.AddWithValue("@selected_group_access2", Common.IsNull(productClassification.Category3Access2));
            cmd.Parameters.AddWithValue("@selected_group_except", Common.IsNull(productClassification.Category3Except));
            cmd.Parameters.AddWithValue("@selected_subgroup_access1", Common.IsNull(productClassification.Category4Access1));
            cmd.Parameters.AddWithValue("@selected_subgroup_access2", Common.IsNull(productClassification.Category4Access2));
            cmd.Parameters.AddWithValue("@selected_subgroup_except", Common.IsNull(productClassification.Category4Except));

            cmd.Parameters.AddWithValue("@adtypeClassAdTypeAccess1", Common.IsNull(adTypes.AdTypeAccess1));
            cmd.Parameters.AddWithValue("@adtypeClassAdTypeAccess2", Common.IsNull(adTypes.AdTypeAccess2));
            cmd.Parameters.AddWithValue("@adtypeClassAdTypeExcept", Common.IsNull(adTypes.AdTypeExcept));

            return;
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle name</param>
        /// <param name="idMedia">Media Id</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        public PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate)
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate)
        {
            common = new CommonDAL(webSession, webSession.CurrentModule);

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle name</param>
        /// <param name="idMedia">Media Id</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="adBreak"></param>		
        public PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate, string adBreak) :
            base(webSession, vehicleInformation, idMedia, beginingDate, endDate, adBreak)
        {
            common = new CommonDAL(webSession, webSession.CurrentModule);

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle name</param>
        /// <param name="idMedia">Media Id</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="hour Beginning List">hour Beginning List</param>
        /// <param name="hourEndList">hour EndList</param>		
        public PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate, Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList)
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate, hourBeginningList, hourEndList)
        {
            common = new CommonDAL(webSession, webSession.CurrentModule);

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle name</param>
        /// <param name="idMedia">Media Id</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="adBreak"></param>
        /// <param name="ventilationTypeList">ventilation Type List</param>
        public PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate, List<PortofolioStructure.Ventilation> ventilationTypeList)
            :
         base(webSession, vehicleInformation, idMedia, beginingDate, endDate, ventilationTypeList)
        {
            common = new CommonDAL(webSession, webSession.CurrentModule);

        }
        #endregion

        #region IPortofolioDAL Membres

        #region GetData
        /// <summary>
        /// Get synthesis data
        /// </summary>
        /// <returns></returns>
        public override DataSet GetData()
        {
            DataSet ds = null;

            switch (_webSession.CurrentTab)
            {
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
                    ds = PortofolioDetail();
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
                    ds = Calendar();
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:
                    switch (_vehicleInformation.Id)
                    {
                        case DBClassificationConstantes.Vehicles.names.others:
                        case DBClassificationConstantes.Vehicles.names.tv:
                        case DBClassificationConstantes.Vehicles.names.tvGeneral:
                        case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                        case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                        case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                        case DBClassificationConstantes.Vehicles.names.radio:
                        case DBClassificationConstantes.Vehicles.names.radioGeneral:
                        case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                        case DBClassificationConstantes.Vehicles.names.radioMusic:
                            ds = MediaDetail();
                            break;
                        default:
                            throw (new PortofolioDALException("Impossible to identified current vehicle "));
                    }
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
                    ds = Structure();
                    break;
                default:
                    throw (new PortofolioDALException("Impossible to identified current tab "));
            }

            return ds;
        }
        #endregion

        #region GetInsertionData
        /// <summary>
        /// Get Inssertion data
        /// </summary>
        /// <returns></returns>
        public override DataSet GetInsertionData()
        {
            try
            {

                DataSet ds = new DataSet();

                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = GetInsertionDataProcedureName;

                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@id_media", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@id_vehicle", SqlDbType.Int);
                    cmd.Parameters.Add("@start", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@stop", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@cmpn_type", SqlDbType.NVarChar);


                    // SP set Parameters
                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_webSession.DataLanguage);
                    cmd.Parameters["@id_media"].Value = Convert.ToInt16(_vehicleInformation.DatabaseId);
                    cmd.Parameters["@id_vehicle"].Value = Convert.ToInt32(_idMedia);
                    cmd.Parameters["@start"].Value = _beginingDate;
                    cmd.Parameters["@stop"].Value = _endDate;
                    cmd.Parameters["@cmpn_type"].Value = _webSession.CampaignType.ToString();

                    GetRightSelectParams(ref cmd);

                    // SP Execute
                    SqlDataReader dr;
                    dr = cmd.ExecuteReader();
                    ds.Load(dr, LoadOption.OverwriteChanges, "result");
                    conn.Close();
                }

                return ds;
            }
            catch (Exception err)
            {
                throw (new PortofolioDALException("GetData. Unable to load the results:", err));
            }
        }
        #endregion

        #region Synthesis membres

        #region GetSynthisData
        /// <summary>
        /// Implements  data access layer for Portofolio Synthesis Result
        /// </summary>
        /// <param name="synthesisDataType">Synthesis Data Type</param>
        /// <returns>Data Set with Data Table Result</returns>
        public override DataSet GetSynthisData(PortofolioSynthesis.dataType synthesisDataType)
        {
            try
            {

                DataSet ds = new DataSet();

                // Call SP at once, and fill all data
                if (SummaryDataEntryPoint == 0)
                {

                    using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                    {
                        SqlCommand cmd = conn.CreateCommand();
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 300;
                        cmd.CommandText = GetSummaryDataProcedureName;

                        cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                        cmd.Parameters.Add("@id_media", SqlDbType.SmallInt);
                        cmd.Parameters.Add("@id_vehicle", SqlDbType.Int);
                        cmd.Parameters.Add("@start", SqlDbType.NVarChar);
                        cmd.Parameters.Add("@stop", SqlDbType.NVarChar);
                        cmd.Parameters.Add("@cmpn_type", SqlDbType.NVarChar);


                        // SP set Parameters
                        cmd.Parameters["@id_language"].Value = Convert.ToInt16(_webSession.DataLanguage);
                        cmd.Parameters["@id_media"].Value = Convert.ToInt16(_vehicleInformation.DatabaseId);
                        cmd.Parameters["@id_vehicle"].Value = Convert.ToInt32(_idMedia);
                        cmd.Parameters["@start"].Value = _beginingDate;
                        cmd.Parameters["@stop"].Value = _endDate;
                        cmd.Parameters["@cmpn_type"].Value = _webSession.CampaignType.ToString();

                        GetRightSelectParams(ref cmd);

                        // SP Execute
                        SqlDataReader dr;
                        dr = cmd.ExecuteReader();
                        dsSummary.Load(dr, LoadOption.OverwriteChanges, "result");
                        conn.Close();
                        SummaryDataEntryPoint = 1;
                    }
                }

                if (synthesisDataType == PortofolioSynthesis.dataType.numberProduct ||
                    synthesisDataType == PortofolioSynthesis.dataType.numberSponsorship ||
                    synthesisDataType == PortofolioSynthesis.dataType.numberBrand ||
                    synthesisDataType == PortofolioSynthesis.dataType.numberSubBrand ||
                    synthesisDataType == PortofolioSynthesis.dataType.numberAdvertiser ||
                    synthesisDataType == PortofolioSynthesis.dataType.pageSelfPromo)
                {
                    DataSet ds1 = new DataSet();
                    ds1.Tables.Add();
                    ds1.Tables[0].Columns.Add("nbLines");
                    ds1.Tables[0].Rows.Add(dsSummary.Tables[0].Rows[0][synthesisDataType.ToString()]);
                    return ds1;
                }
                else if (synthesisDataType == PortofolioSynthesis.dataType.numberAdBreaks)
                {
                    DataSet ds1 = new DataSet();
                    ds1.Tables.Add();
                    ds1.Tables[0].Columns.Add("insertion");
                    ds1.Tables[0].Columns.Add("ecran_duration");
                    ds1.Tables[0].Rows.Add(dsSummary.Tables[0].Rows[0]["numberAdBreaks"], dsSummary.Tables[0].Rows[0]["ecran_duration"]);
                    return ds1;
                }

                return dsSummary;
            }
            catch (Exception err)
            {
                throw (new PortofolioDALException("GetData. Unable to load the results:", err));
            }

        }
        #endregion

        #region Get Data Ecran
        /// <summary>
        /// récupère les écrans
        /// </summary>
        /// <returns>Ecrans</returns>
        public override DataSet GetEcranData()
        {
            throw new NotImplementedException("This methods is not implemented");
        }
        #endregion

        #endregion

        #region Get dates list
        /// <summary>
        /// Get vehicle publication date ( as YYYYMMDD) and cover file name (e.g dates list 367942.jpg).
        /// The result data will have the following column :
        /// [date_media_num] [visual]
        /// </summary>
        /// <example>
        /// --SLQ QUERY
        /// select distinct date_media_num, visual from PRESS_TABLE
        /// where idMedia = 300010590
        /// and id_language = 7
        /// and beginingDate = 20111205
        /// and endDate = _endDate
        /// order by date_media_num
        /// 
        /// --RESULT EXAMPLE
        /// [20111205][367942.jpg]
        /// [20111212][368426.jpg]
        /// [20111219][368827.jpg]
        /// </example>
        /// <param name="conditionEndDate">Add condition end date</param>
        /// <returns>DataSet</returns>
        public override DataSet GetListDate(bool conditionEndDate, DBConstantes.TableType.Type tableType)
        {
            try
            {
                DataSet ds = new DataSet();

                //Filter sql statement with parameters:
                // _idMedia : represents vehicle Identifier selected by customer
                //_webSession.DataLanguage : represents data language Identifier
                // _beginingDate  : represents period beginning date (YYYYMMDD)
                // _endDate  : represents period end date (YYYYMMDD)

                //TODO : Implement Russian sql query 

                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = GetPressVisualDataProcedureName;

                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@id_media", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@id_vehicle", SqlDbType.Int);
                    cmd.Parameters.Add("@start", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@stop", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@cmpn_type", SqlDbType.NVarChar);

                    // SP set Parameters
                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_webSession.DataLanguage);
                    cmd.Parameters["@id_media"].Value = Convert.ToInt16(_vehicleInformation.DatabaseId);
                    cmd.Parameters["@id_vehicle"].Value = Convert.ToInt32(_idMedia);
                    cmd.Parameters["@start"].Value = _beginingDate;
                    cmd.Parameters["@stop"].Value = _endDate;
                    cmd.Parameters["@cmpn_type"].Value = _webSession.CampaignType.ToString();

                    GetRightSelectParams(ref cmd);

                    // SP Execute
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        ds.Load(dr, LoadOption.OverwriteChanges, "result");
                        conn.Close();
                    }
                }

                return ds;
            }
            catch (Exception err)
            {
                throw (new PortofolioDALException("GetListDate. Unable to load the results:", err));
            }
        }
        #endregion

        #region TableOfIssue
        /// <summary>
        /// Implements  data access layer for table of issue. 
        /// </summary>
        /// <returns>Data Set with Data Table Result</returns>
        public override DataSet TableOfIssue()
        {
            try
            {

                DataSet ds = new DataSet();

                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = GetPressIssueDataProcedureName;

                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@id_media", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@id_vehicle", SqlDbType.Int);
                    cmd.Parameters.Add("@start", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@stop", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@cmpn_type", SqlDbType.NVarChar);

                    // SP set Parameters
                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_webSession.DataLanguage);
                    cmd.Parameters["@id_media"].Value = Convert.ToInt16(_vehicleInformation.DatabaseId);
                    cmd.Parameters["@id_vehicle"].Value = Convert.ToInt32(_idMedia);
                    cmd.Parameters["@start"].Value = _beginingDate;
                    cmd.Parameters["@stop"].Value = _endDate;
                    cmd.Parameters["@cmpn_type"].Value = _webSession.CampaignType.ToString();

                    GetRightSelectParams(ref cmd);

                    // SP Execute
                    SqlDataReader dr;
                    dr = cmd.ExecuteReader();
                    ds.Load(dr, LoadOption.OverwriteChanges, "result");
                    conn.Close();
                }

                return ds;
            }
            catch (Exception err)
            {
                throw (new PortofolioDALException("GetData. Unable to load the results:", err));
            }

        }
        #endregion

        #endregion

        #region Protected Methods

        #region PortofolioDetail
        /// <summary>
        /// Implements  data access layer for Portofolio Detail Result. 
        /// </summary>
        /// <returns>Data Set with Data Tables Result</returns>
        protected DataSet PortofolioDetail()
        {
            try
            {

                MediaRight mediaRight = Common.GetMediaRight();
                ProductRight productRight = Common.GetProductRight();
                ProductClassification productClassification = Common.GetProductClassification();
                AdTypeClassification adTypes = Common.GetAdTypeClassification();

                DataSet ds = new DataSet();

                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = GetPortfolioDataProcedureName;




                    // SP set Parameters
                    cmd.Parameters.AddWithValue("@id_language", Convert.ToInt16(_webSession.DataLanguage));
                    cmd.Parameters.AddWithValue("@id_media", Convert.ToInt16(_vehicleInformation.DatabaseId));
                    cmd.Parameters.AddWithValue("@id_vehicle", Convert.ToInt32(_idMedia));
                    cmd.Parameters.AddWithValue("@start", _beginingDate);
                    cmd.Parameters.AddWithValue("@stop", _endDate);

                    cmd.Parameters.AddWithValue("@id_level1", (_webSession.GenericProductDetailLevel.LevelIds.Count > 0) ? _webSession.GenericProductDetailLevel.LevelIds[0].ToString() : string.Empty);
                    cmd.Parameters.AddWithValue("@id_level2", (_webSession.GenericProductDetailLevel.LevelIds.Count > 1) ? _webSession.GenericProductDetailLevel.LevelIds[1].ToString() : string.Empty);
                    cmd.Parameters.AddWithValue("@id_level3", (_webSession.GenericProductDetailLevel.LevelIds.Count > 2) ? _webSession.GenericProductDetailLevel.LevelIds[2].ToString() : string.Empty);

                    cmd.Parameters.AddWithValue("@customer_media_access", Common.IsNull(mediaRight.MediaRightMediaAccess));
                    cmd.Parameters.AddWithValue("@customer_media_except", Common.IsNull(mediaRight.MediaRightMediaExcept));
                    cmd.Parameters.AddWithValue("@customer_vehicle_access", Common.IsNull(mediaRight.MediaRightVehicleAccess));
                    cmd.Parameters.AddWithValue("@customer_vehicle_except", Common.IsNull(mediaRight.MediaRightVehicleExcept));
                    cmd.Parameters.AddWithValue("@customer_region_access", Common.IsNull(mediaRight.MediaRightRegionAccess));
                    cmd.Parameters.AddWithValue("@customer_region_except", Common.IsNull(mediaRight.MediaRightRegionExcept));

                    cmd.Parameters.AddWithValue("@customer_sector_access", Common.IsNull(productRight.ProductRightCategory1Access));
                    cmd.Parameters.AddWithValue("@customer_sector_except", Common.IsNull(productRight.ProductRightCategory1Except));
                    cmd.Parameters.AddWithValue("@customer_subsector_access", Common.IsNull(productRight.ProductRightCategory2Access));
                    cmd.Parameters.AddWithValue("@customer_subsector_except", Common.IsNull(productRight.ProductRightCategory2Except));
                    cmd.Parameters.AddWithValue("@customer_group_access", Common.IsNull(productRight.ProductRightCategory3Access));
                    cmd.Parameters.AddWithValue("@customer_group_except", Common.IsNull(productRight.ProductRightCategory3Except));
                    cmd.Parameters.AddWithValue("@customer_subgroup_access", Common.IsNull(productRight.ProductRightCategory4Access));
                    cmd.Parameters.AddWithValue("@customer_subgroup_except", Common.IsNull(productRight.ProductRightCategory4Except));

                    cmd.Parameters.AddWithValue("@customer_advertiser_access", Common.IsNull(productRight.ProductRightAdvertiserAccess));
                    cmd.Parameters.AddWithValue("@customer_advertiser_except", Common.IsNull(productRight.ProductRightAdvertiserExcept));
                    cmd.Parameters.AddWithValue("@customer_brand_access", Common.IsNull(productRight.ProductRightBrandAccess));
                    cmd.Parameters.AddWithValue("@customer_brand_except", Common.IsNull(productRight.ProductRightBrandExcept));

                    cmd.Parameters.AddWithValue("@selected_sector_access1", Common.IsNull(productClassification.Category1Access1));
                    cmd.Parameters.AddWithValue("@selected_sector_access2", Common.IsNull(productClassification.Category1Access2));
                    cmd.Parameters.AddWithValue("@selected_sector_except", Common.IsNull(productClassification.Category1Except));
                    cmd.Parameters.AddWithValue("@selected_subsector_access1", Common.IsNull(productClassification.Category2Access1));
                    cmd.Parameters.AddWithValue("@selected_subsector_access2", Common.IsNull(productClassification.Category2Access2));
                    cmd.Parameters.AddWithValue("@selected_subsector_except", Common.IsNull(productClassification.Category2Except));
                    cmd.Parameters.AddWithValue("@selected_group_access1", Common.IsNull(productClassification.Category3Access1));
                    cmd.Parameters.AddWithValue("@selected_group_access2", Common.IsNull(productClassification.Category3Access2));
                    cmd.Parameters.AddWithValue("@selected_group_except", Common.IsNull(productClassification.Category3Except));
                    cmd.Parameters.AddWithValue("@selected_subgroup_access1", Common.IsNull(productClassification.Category4Access1));
                    cmd.Parameters.AddWithValue("@selected_subgroup_access2", Common.IsNull(productClassification.Category4Access2));
                    cmd.Parameters.AddWithValue("@selected_subgroup_except", Common.IsNull(productClassification.Category4Except));
                    cmd.Parameters.AddWithValue("@cmpn_type", _webSession.CampaignType.ToString());

                    cmd.Parameters.AddWithValue("@adtypeClassAdTypeAccess1", Common.IsNull(adTypes.AdTypeAccess1));
                    cmd.Parameters.AddWithValue("@adtypeClassAdTypeAccess2", Common.IsNull(adTypes.AdTypeAccess2));
                    cmd.Parameters.AddWithValue("@adtypeClassAdTypeExcept", Common.IsNull(adTypes.AdTypeExcept));

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "total";
                        }

                        for (int i = 1; i < ds.Tables.Count; ++i)
                        {
                            ds.Tables[i].TableName = string.Format("level{0}", i);

                            for (int j = 0; j < i && _webSession.GenericProductDetailLevel.Levels.Count >= i; ++j)
                            {
                                DetailLevelItemInformation info = (DetailLevelItemInformation)_webSession.GenericProductDetailLevel.Levels[j];
                                string levelFieldAlias = info.DataBaseAliasField ?? info.DataBaseField;
                                string levelIdFieldAlias = info.DataBaseAliasIdField ?? info.DataBaseIdField;
                                string nameFieldAlias = string.Format("level{0}", j + 1);
                                string nameIdFieldAlias = string.Format("id_level{0}", j + 1);
                                string tempFieldAlias = string.Format("temp{0}", j + 1);
                                string tempIdFieldAlias = string.Format("id_temp{0}", j + 1);
                                if (ds.Tables[i].Columns.Contains(levelFieldAlias))
                                {
                                    ds.Tables[i].Columns[levelFieldAlias].ColumnName = tempFieldAlias;
                                }
                                if (ds.Tables[i].Columns.Contains(levelIdFieldAlias))
                                {
                                    ds.Tables[i].Columns[levelIdFieldAlias].ColumnName = tempIdFieldAlias;
                                }

                                ds.Tables[i].Columns[nameFieldAlias].ColumnName = levelFieldAlias;
                                ds.Tables[i].Columns[nameIdFieldAlias].ColumnName = levelIdFieldAlias;
                            }
                        }
                    }
                    conn.Close();
                }

                return ds;
            }
            catch (Exception err)
            {
                throw (new PortofolioDALException("GetData. Unable to load the results:", err));
            }


        }
        #endregion

        #region MediaDetail
        /// <summary>
        /// Implements  data access layer for Media Detail Result. 
        /// </summary>
        /// <returns>Data Set with Data Tables Result</returns>
        protected DataSet MediaDetail()
        {
            return VehicleBreakdown_Structure("breakdown");
        }
        #endregion

        #region Calendar
        /// <summary>
        /// Implements  data access layer for Calendar Result. 
        /// </summary>
        /// <returns>Data Set with Data Tables Result</returns>
        protected DataSet Calendar()
        {
            try
            {

                DataSet ds = new DataSet();

                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = GetCalendarDataProcedureName;

                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@id_media", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@id_vehicle", SqlDbType.Int);
                    cmd.Parameters.Add("@start", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@stop", SqlDbType.NVarChar);

                    cmd.Parameters.Add("@id_unit", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@id_level1", SqlDbType.VarChar);
                    cmd.Parameters.Add("@id_level2", SqlDbType.VarChar);
                    cmd.Parameters.Add("@id_level3", SqlDbType.VarChar);

                    cmd.Parameters.Add("@cmpn_type", SqlDbType.NVarChar);

                    // SP set Parameters
                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_webSession.DataLanguage);
                    cmd.Parameters["@id_media"].Value = Convert.ToInt16(_vehicleInformation.DatabaseId);
                    cmd.Parameters["@id_vehicle"].Value = Convert.ToInt32(_idMedia);
                    cmd.Parameters["@start"].Value = _beginingDate;
                    cmd.Parameters["@stop"].Value = _endDate;

                    cmd.Parameters["@id_unit"].Value = _webSession.GetSelectedUnit().Id.ToString();
                    cmd.Parameters["@id_level1"].Value = (_webSession.GenericProductDetailLevel.LevelIds.Count > 0) ? _webSession.GenericProductDetailLevel.LevelIds[0].ToString() : string.Empty;
                    cmd.Parameters["@id_level2"].Value = (_webSession.GenericProductDetailLevel.LevelIds.Count > 1) ? _webSession.GenericProductDetailLevel.LevelIds[1].ToString() : string.Empty;
                    cmd.Parameters["@id_level3"].Value = (_webSession.GenericProductDetailLevel.LevelIds.Count > 2) ? _webSession.GenericProductDetailLevel.LevelIds[2].ToString() : string.Empty;
                    cmd.Parameters["@cmpn_type"].Value = _webSession.CampaignType.ToString();

                    GetRightSelectParams(ref cmd);

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "total";
                            if (ds.Tables[0].Columns.Contains("unit")) ds.Tables[0].Columns["unit"].ColumnName = _webSession.GetSelectedUnit().Id.ToString();
                        }

                        for (int i = 1; i < ds.Tables.Count; ++i)
                        {
                            ds.Tables[i].TableName = string.Format("level{0}", i);

                            if (ds.Tables[i].Columns.Contains("unit"))
                                ds.Tables[i].Columns["unit"].ColumnName = _webSession.GetSelectedUnit().Id.ToString();

                            for (int j = 0; j < i && _webSession.GenericProductDetailLevel.Levels.Count >= i; ++j)
                            {
                                DetailLevelItemInformation info = (DetailLevelItemInformation)_webSession.GenericProductDetailLevel.Levels[j];
                                string levelFieldAlias = info.DataBaseAliasField ?? info.DataBaseField;
                                string levelIdFieldAlias = info.DataBaseAliasIdField ?? info.DataBaseIdField;
                                string nameFieldAlias = string.Format("level{0}", j + 1);
                                string nameIdFieldAlias = string.Format("id_level{0}", j + 1);
                                string tempFieldAlias = string.Format("temp{0}", j + 1);
                                string tempIdFieldAlias = string.Format("id_temp{0}", j + 1);
                                if (ds.Tables[i].Columns.Contains(levelFieldAlias))
                                {
                                    ds.Tables[i].Columns[levelFieldAlias].ColumnName = tempFieldAlias;
                                }
                                if (ds.Tables[i].Columns.Contains(levelIdFieldAlias))
                                {
                                    ds.Tables[i].Columns[levelIdFieldAlias].ColumnName = tempIdFieldAlias;
                                }

                                ds.Tables[i].Columns[nameFieldAlias].ColumnName = levelFieldAlias;
                                ds.Tables[i].Columns[nameIdFieldAlias].ColumnName = levelIdFieldAlias;
                            }
                        }
                    }
                    conn.Close();
                }

                return ds;
            }
            catch (Exception err)
            {
                throw (new PortofolioDALException("GetData. Unable to load the results:", err));
            }

        }
        #endregion

        #region Structure
        /// <summary>
        /// Implements  data access layer for Structure Result. 
        /// </summary>
        /// <returns>Data Set with Data Tables Result</returns>
        protected DataSet Structure()
        {
            return VehicleBreakdown_Structure("structure");
        }
        #endregion

        #region VehicleBreakdown_Structure
        /// <summary>
        /// Implements  data access layer for Media Detail Result or Structure result. 
        /// </summary>
        /// <returns>Data Set with Data Tables Result</returns>
        protected DataSet VehicleBreakdown_Structure(string report_type)
        {

            try
            {

                DataSet ds = new DataSet();

                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = GetBreakdownDataProcedureName;

                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@id_media", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@id_vehicle", SqlDbType.Int);
                    cmd.Parameters.Add("@start", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@stop", SqlDbType.NVarChar);

                    cmd.Parameters.Add("@report_type", SqlDbType.NVarChar);

                    cmd.Parameters.Add("@cmpn_type", SqlDbType.NVarChar);


                    // SP set Parameters
                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_webSession.DataLanguage);
                    cmd.Parameters["@id_media"].Value = Convert.ToInt16(_vehicleInformation.DatabaseId);
                    cmd.Parameters["@id_vehicle"].Value = Convert.ToInt32(_idMedia);
                    cmd.Parameters["@start"].Value = _beginingDate;
                    cmd.Parameters["@stop"].Value = _endDate;

                    cmd.Parameters["@report_type"].Value = report_type;
                    cmd.Parameters["@cmpn_type"].Value = _webSession.CampaignType.ToString();

                    GetRightSelectParams(ref cmd);

                    // SP Execute
                    SqlDataReader dr;
                    dr = cmd.ExecuteReader();
                    ds.Load(dr, LoadOption.OverwriteChanges, "result");
                    conn.Close();
                }

                return ds;
            }
            catch (Exception err)
            {
                throw (new PortofolioDALException("GetData. Unable to load the results:", err));
            }
        }
        #endregion

        #endregion
    }
}
