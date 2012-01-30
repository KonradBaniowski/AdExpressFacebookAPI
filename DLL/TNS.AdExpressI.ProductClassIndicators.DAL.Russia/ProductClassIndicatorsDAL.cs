using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Common.DAL.Russia;
using TNS.AdExpressI.ProductClassIndicators.DAL.DALEngines;
using TNS.AdExpressI.ProductClassIndicators.DAL.Exceptions;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;

namespace TNS.AdExpressI.ProductClassIndicators.DAL.Russia
{
    /// <summary>
    /// Product Class Indicators DAL
    /// </summary>
    public partial class ProductClassIndicatorsDAL : TNS.AdExpressI.ProductClassIndicators.DAL.ProductClassIndicatorsDAL
    {
        private const string GetTopsProcedureName = "[dbo].[rp_ax_productindicator_tops_get]";
        private const string GetEvolutionProcedureName = "[dbo].[rp_ax_productindicator_evolution_get]";
        private const string GetNoveltyProcedureName = "[dbo].[rp_ax_productindicator_novelty_get]";
        private const string GetSeasonalityProcedureName = "[dbo].[rp_ax_productindicator_seasonality_get]";
        private const string GetMediaStrategyProcedureName = "[dbo].[rp_ax_productindicator_strategy_get]";
        private const string GetSummaryProcedureName = "[dbo].[rp_ax_productindicator_summary_get]";
        private const string GetPersonnalisationProcedureName = "[dbo].[rp_ax_productindicator_personnalisation_get]";
        private static DataSet _improveTopsDataSet = new DataSet();
        private static DataSet _improveEvolutionDataSet = new DataSet();
        private static DataSet _improveNoveltyDataSet = new DataSet();
        private readonly CommonDAL _common;

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassIndicatorsDAL(WebSession session)
            : base(session)
        {
            DALEngine dalE = new DALEngine(_session, _dalUtilities);

            _periodBegin = dalE.PeriodBegin;
            _periodEnd = dalE.PeriodBegin;

            _common = new CommonDAL(_session, _session.CurrentModule);
        }
        #endregion

        #region IProductClassIndicatorsDAL Membres

        /// <summary>
        /// Implements  data access layer for Ranking of the top ten advertisers and products in year N and N-1.
        /// (league table  results) 
        /// </summary>
        /// <param name="classifLevel">Classification detail (product or advertiser)</param>
        /// <param name="typeYear">Type of PERIOD study </param>        
        public override DataSet GetTops(CstResult.PalmaresRecap.typeYearSelected typeYear, CstResult.MotherRecap.ElementType classifLevel)
        {
            string tableName = string.Empty;
            if (typeYear == CstResult.PalmaresRecap.typeYearSelected.currentYear
                && classifLevel == CstResult.MotherRecap.ElementType.product)
            {
                return GetImproveTableByName(ref _improveTopsDataSet, new string[] { "currentYear_product" });
            }

            if (typeYear == CstResult.PalmaresRecap.typeYearSelected.previousYear
                && classifLevel == CstResult.MotherRecap.ElementType.advertiser)
            {
                return GetImproveTableByName(ref _improveTopsDataSet, new string[] { "previousYear_advertiser" });
            }

            if (typeYear == CstResult.PalmaresRecap.typeYearSelected.previousYear
                && classifLevel == CstResult.MotherRecap.ElementType.product)
            {
                return GetImproveTableByName(ref _improveTopsDataSet, new string[] { "previousYear_product" });
            }

            try
            {
                _improveTopsDataSet.Tables.Clear();
                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetTopsProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@report_format", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@report_typeYear", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@report_classifLevel", SqlDbType.NVarChar);

                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@report_format"].Value = "tops";
                    cmd.Parameters["@report_typeYear"].Value = typeYear;
                    cmd.Parameters["@report_classifLevel"].Value = classifLevel;

                    SetSqlParameters(ref cmd, _session.CurrentModule, 0);

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(_improveTopsDataSet);

                        if (_improveTopsDataSet.Tables.Count == 4)
                        {
                            _improveTopsDataSet.Tables[0].TableName = "currentYear_advertiser";
                            _improveTopsDataSet.Tables[1].TableName = "previousYear_advertiser";
                            _improveTopsDataSet.Tables[2].TableName = "currentYear_product";
                            _improveTopsDataSet.Tables[3].TableName = "previousYear_product";
                        }
                    }

                    conn.Close();
                }

                return GetImproveTableByName(ref _improveTopsDataSet, new string[] { "currentYear_advertiser" });
            }
            catch (Exception err)
            {
                throw (new ProductClassIndicatorsDALException("GetTops. Unable to load the results:", err));
            }
        }

        /// <summary>
        /// Implements default data access layer for Ranking of the top ten increases and decreases in year n as against year N-1.
        /// (Change results)
        /// </summary>
        /// <param name="classifLevel">Classification detail (product or advertiser)</param>
        /// <param name="typeYear">Type of study</param>
        public override DataSet GetEvolution(CstResult.MotherRecap.ElementType classifLevel)
        {
            string tableName = string.Empty;
            if (classifLevel == CstResult.MotherRecap.ElementType.product)
            {
                return GetImproveTableByName(ref _improveEvolutionDataSet, new string[] { "productIncrease", "productDecrease" });
            }

            try
            {
                _improveEvolutionDataSet.Tables.Clear();
                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetEvolutionProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@report_format", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@report_classifLevel", SqlDbType.NVarChar);

                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@report_format"].Value = "evolution";
                    cmd.Parameters["@report_classifLevel"].Value = classifLevel;

                    SetSqlParameters(ref cmd, _session.CurrentModule, 0);

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(_improveEvolutionDataSet);

                        if (_improveEvolutionDataSet.Tables.Count == 4)
                        {
                            _improveEvolutionDataSet.Tables[0].TableName = "advertiserIncrease";
                            _improveEvolutionDataSet.Tables[1].TableName = "advertiserDecrease";
                            _improveEvolutionDataSet.Tables[2].TableName = "productIncrease";
                            _improveEvolutionDataSet.Tables[3].TableName = "productDecrease";
                        }
                    }

                    conn.Close();
                }

                return GetImproveTableByName(ref _improveEvolutionDataSet, new string[] { "advertiserIncrease", "advertiserDecrease" });
            }
            catch (Exception err)
            {
                throw (new ProductClassIndicatorsDALException("GetEvolution. Unable to load the results:", err));
            }
        }

        /// <summary>
        /// Implements default data access layer for New products and campaign re-launches from year N to N-1.
        /// (What's new indicator)
        /// </summary>
        /// <param name="classifLevel">Classification detail (product or advertiser)</param>
        public override DataSet GetNovelty(CstResult.MotherRecap.ElementType classifLevel)
        {
            string tableName = string.Empty;
            if (classifLevel == CstResult.MotherRecap.ElementType.product)
            {
                return GetImproveTableByName(ref _improveNoveltyDataSet, new string[] { "product" });
            }

            try
            {
                _improveNoveltyDataSet.Tables.Clear();
                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetNoveltyProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@report_format", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@report_classifLevel", SqlDbType.NVarChar);

                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@report_format"].Value = "novelty";
                    cmd.Parameters["@report_classifLevel"].Value = classifLevel;

                    SetSqlParameters(ref cmd, _session.CurrentModule, 0);

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(_improveNoveltyDataSet);

                        if (_improveNoveltyDataSet.Tables.Count == 2)
                        {
                            _improveNoveltyDataSet.Tables[0].TableName = "advertiser";
                            _improveNoveltyDataSet.Tables[1].TableName = "product";
                        }
                    }

                    conn.Close();
                }

                return GetImproveTableByName(ref _improveNoveltyDataSet, new string[] { "advertiser" });
            }
            catch (Exception err)
            {
                throw (new ProductClassIndicatorsDALException("GetNovelty. Unable to load the results:", err));
            }
        }

        /// <summary>
        /// Get required data to build a Seasonal variations table
        /// </summary>       
        public override DataSet GetSeasonalityTblData(bool withAdvertisers, bool withRights)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetSeasonalityProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@report_format", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@report_withAdvertisers", SqlDbType.Bit);
                    cmd.Parameters.Add("@report_withRights", SqlDbType.Bit);

                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@report_format"].Value = "seasonality";
                    cmd.Parameters["@report_withAdvertisers"].Value = withAdvertisers;
                    cmd.Parameters["@report_withRights"].Value = withRights;

                    SetSqlParameters(ref cmd, _session.CurrentModule, 0);

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);
                    }

                    conn.Close();
                }

                #region Table debug info

                foreach (DataTable table in ds.Tables)
                {
                    string message = table.TableName;

                    foreach (DataColumn column in table.Columns)
                    {
                        message += string.Format(" [{0}]", column.ColumnName);
                    }

                    Debug.WriteLine(message);
                }

                #endregion

                return ds;
            }
            catch (Exception err)
            {
                throw (new ProductClassIndicatorsDALException("GetSeasonalityTblData. Unable to load the results:", err));
            }
        }

        /// <summary>
        /// Get required data to build a Seasonal variations graph
        /// </summary>       
        public override DataSet GetSeasonalityGraphData(bool withAdvertisers, bool withRights)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetSeasonalityProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@report_format", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@report_withAdvertisers", SqlDbType.Bit);
                    cmd.Parameters.Add("@report_withRights", SqlDbType.Bit);

                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@report_format"].Value = "seasonality";
                    cmd.Parameters["@report_withAdvertisers"].Value = withAdvertisers;
                    cmd.Parameters["@report_withRights"].Value = withRights;

                    SetSqlParameters(ref cmd, _session.CurrentModule, 0);

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);
                    }

                    conn.Close();
                }

                #region Table debug info

                foreach (DataTable table in ds.Tables)
                {
                    string message = table.TableName;

                    foreach (DataColumn column in table.Columns)
                    {
                        message += string.Format(" [{0}]", column.ColumnName);
                    }

                    Debug.WriteLine(message);
                }

                #endregion

                return ds;
            }
            catch (Exception err)
            {
                throw (new ProductClassIndicatorsDALException("GetSeasonalityGraphData. Unable to load the results:", err));
            }
        }

        /// <summary>
        ///Get media strategy data 
        /// <remarks>Use specially in Russia</remarks>
        /// </summary>       
        /// <returns>DataTable to build media Strategy Indicator</returns>
        public override DataSet GetMediaStrategyData()
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetMediaStrategyProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@report_format", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@media_level1", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@media_level2", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@media_level3", SqlDbType.NVarChar);

                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@report_format"].Value = "mediastrategy";

                    int mediaLevelCount = 0;

                    #region Set Media Levels
                    switch (_session.PreformatedMediaDetail)
                    {
                        case CstFormat.PreformatedMediaDetails.vehicle:
                            cmd.Parameters["@media_level1"].Value = "vehicle";
                            mediaLevelCount = 1;
                            break;
                        case CstFormat.PreformatedMediaDetails.region:
                            cmd.Parameters["@media_level1"].Value = "region";
                            mediaLevelCount = 1;
                            break;
                        case CstFormat.PreformatedMediaDetails.Media:
                            cmd.Parameters["@media_level1"].Value = "media";
                            mediaLevelCount = 1;
                            break;
                        case CstFormat.PreformatedMediaDetails.vehicleRegion:
                            cmd.Parameters["@media_level1"].Value = "vehicle";
                            cmd.Parameters["@media_level2"].Value = "region";
                            mediaLevelCount = 2;
                            break;
                        case CstFormat.PreformatedMediaDetails.vehicleMedia:
                            cmd.Parameters["@media_level1"].Value = "vehicle";
                            cmd.Parameters["@media_level2"].Value = "media";
                            mediaLevelCount = 2;
                            break;
                        case CstFormat.PreformatedMediaDetails.regionVehicle:
                            cmd.Parameters["@media_level1"].Value = "region";
                            cmd.Parameters["@media_level2"].Value = "vehicle";
                            mediaLevelCount = 2;
                            break;
                        case CstFormat.PreformatedMediaDetails.regionMedia:
                            cmd.Parameters["@media_level1"].Value = "region";
                            cmd.Parameters["@media_level2"].Value = "media";
                            mediaLevelCount = 2;
                            break;
                        case CstFormat.PreformatedMediaDetails.vehicleRegionMedia:
                            cmd.Parameters["@media_level1"].Value = "vehicle";
                            cmd.Parameters["@media_level2"].Value = "region";
                            cmd.Parameters["@media_level3"].Value = "media";
                            mediaLevelCount = 3;
                            break;
                    }
                    #endregion

                    SetSqlParameters(ref cmd, _session.CurrentModule, 0);

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);

                        #region Set table names

                        int currentTable = 0;
                        if (mediaLevelCount >= 1)
                        {
                            ds.Tables[currentTable].TableName = "Level 1";

                            if (!string.IsNullOrEmpty(_session.CustomerDataFilters.ReferencesAdvertisers) && !string.IsNullOrEmpty(_session.CustomerDataFilters.CompetingAdvertisers))
                            {
                                ds.Tables[++currentTable].TableName = "Level 1 Ref";
                            }
                        }

                        if (mediaLevelCount >= 2)
                        {
                            ds.Tables[++currentTable].TableName = "Level 2";

                            if (!string.IsNullOrEmpty(_session.CustomerDataFilters.ReferencesAdvertisers) && !string.IsNullOrEmpty(_session.CustomerDataFilters.CompetingAdvertisers))
                            {
                                ds.Tables[++currentTable].TableName = "Level 2 Ref";
                            }
                        }

                        if (mediaLevelCount >= 3)
                        {
                            ds.Tables[++currentTable].TableName = "Level 3";

                            if (!string.IsNullOrEmpty(_session.CustomerDataFilters.ReferencesAdvertisers) && !string.IsNullOrEmpty(_session.CustomerDataFilters.CompetingAdvertisers))
                            {
                                ds.Tables[++currentTable].TableName = "Level 3 Ref";
                            }
                        }

                        #endregion
                    }

                    conn.Close();
                }

                #region Table debug info

                foreach (DataTable table in ds.Tables)
                {
                    string message = table.TableName;

                    foreach (DataColumn column in table.Columns)
                    {
                        message += string.Format(" [{0}]", column.ColumnName);
                    }

                    Debug.WriteLine(message);
                }

                #endregion

                return ds;
            }
            catch (Exception err)
            {
                throw (new ProductClassIndicatorsDALException("GetMediaStrategyData. Unable to load the results:", err));
            }
        }

        /// <summary>
        /// Get summary data
        /// </summary>
        /// <remarks>Use specially in Russia</remarks>
        /// </summary>       
        /// <returns>DataTable to build Summary Indicator</returns>
        public override DataSet GetSummary()
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetSummaryProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@report_format", SqlDbType.NVarChar);

                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@report_format"].Value = "summary";

                    SetSqlParameters(ref cmd, _session.CurrentModule, 0);

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);
                    }

                    conn.Close();
                }

                #region Table debug info

                foreach (DataTable table in ds.Tables)
                {
                    string message = table.TableName;

                    foreach (DataColumn column in table.Columns)
                    {
                        message += string.Format(" [{0}]", column.ColumnName);
                    }

                    Debug.WriteLine(message);
                }

                #endregion

                return ds;
            }
            catch (Exception err)
            {
                throw (new ProductClassIndicatorsDALException("GetSummary. Unable to load the results:", err));
            }
        }

        /// <summary>
        /// Get Products Personnalisation type
        /// </summary>
        /// <param name="idProductList">Id product list</param>
        /// <returns>DataTable[id_product,inref,incomp,inneutral]</returns>
        public override DataSet GetProductsPersonnalisationType(string idProductList, string strYearId)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetPersonnalisationProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@report_format", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@report_typeYear", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@report_classifLevel", SqlDbType.NVarChar);

                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@report_format"].Value = "personnalisation";
                    cmd.Parameters["@report_typeYear"].Value = strYearId;
                    cmd.Parameters["@report_classifLevel"].Value = idProductList;

                    SetSqlParameters(ref cmd, _session.CurrentModule, 0);

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);
                    }

                    conn.Close();
                }

                #region Table debug info

                foreach (DataTable table in ds.Tables)
                {
                    string message = table.TableName;

                    foreach (DataColumn column in table.Columns)
                    {
                        message += string.Format(" [{0}]", column.ColumnName);
                    }

                    Debug.WriteLine(message);
                }

                #endregion

                return ds;
            }
            catch (Exception err)
            {
                throw (new ProductClassIndicatorsDALException("GetProductsPersonnalisationType. Unable to load the results:", err));
            }
        }

        #endregion

        #region Get Improve Table By Name
        public DataSet GetImproveTableByName(ref DataSet improveDataSet, string[] tableNames)
        {
            DataSet ds = new DataSet();
            foreach (string tableName in tableNames)
            {
                if (improveDataSet.Tables.Contains(tableName))
                {
                    ds.Tables.Add(improveDataSet.Tables[tableName].Clone());
                    foreach (DataRow dataRow in improveDataSet.Tables[tableName].Rows)
                    {
                        ds.Tables[tableName].ImportRow(dataRow);
                    }
                }
            }

            return ds;
        }
        #endregion

        #region Set Sql Parameters
        private void SetSqlParameters(ref SqlCommand cmd, long moduleId, long mediaId)
        {
            string beginDate = _session.PeriodBeginningDate;
            string endDate = _session.PeriodEndDate;

            #region Set Date and Identity data

            cmd.Parameters.AddWithValue("@fromDate", beginDate);
            cmd.Parameters.AddWithValue("@toDate", endDate);
            cmd.Parameters.AddWithValue("@moduleId", moduleId);
            cmd.Parameters.AddWithValue("@mediaId", mediaId);

            #endregion

            bool comparativeStudy = _session.ComparativeStudy;
            string selectedVehicle = _common.GetSelectedVehicles();
            string selectedCampaignType = _session.CustomerDataFilters.CampaignType.ToString();
            string selectedUnit = _session.CustomerDataFilters.SelectedUnit.Id.ToString();
            string selectedTotal = _session.ComparaisonCriterion.ToString();

            #region Set Selected data

            cmd.Parameters.AddWithValue("@selectedComparativePeriod", comparativeStudy);
            cmd.Parameters.AddWithValue("@selectedVehicle", _common.IsNull(selectedVehicle));
            cmd.Parameters.AddWithValue("@selectedCampaignType", _common.IsNull(selectedCampaignType));
            cmd.Parameters.AddWithValue("@selectedUnit", selectedUnit);
            cmd.Parameters.AddWithValue("@selectedTotal", _common.IsNull(selectedTotal));

            #endregion

            string referenceAdvertisers = _session.CustomerDataFilters.ReferencesAdvertisers;
            string competingAdvertisers = _session.CustomerDataFilters.CompetingAdvertisers;

            #region Set Advertisers Data

            cmd.Parameters.AddWithValue("@referenceAdvertisers", _common.IsNull(referenceAdvertisers));
            cmd.Parameters.AddWithValue("@competingAdvertisers", _common.IsNull(competingAdvertisers));

            #endregion

            MediaRight mediaRight = _common.GetMediaRight();

            #region Set Media Right Data

            cmd.Parameters.AddWithValue("@mediaRightMediaAccess", _common.IsNull(mediaRight.MediaRightMediaAccess));
            cmd.Parameters.AddWithValue("@mediaRightMediaExcept", _common.IsNull(mediaRight.MediaRightMediaExcept));
            cmd.Parameters.AddWithValue("@mediaRightRegionAccess", _common.IsNull(mediaRight.MediaRightRegionAccess));
            cmd.Parameters.AddWithValue("@mediaRightRegionExcept", _common.IsNull(mediaRight.MediaRightRegionExcept));
            cmd.Parameters.AddWithValue("@mediaRightVehicleAccess", _common.IsNull(mediaRight.MediaRightVehicleAccess));
            cmd.Parameters.AddWithValue("@mediaRightVehicleExcept", _common.IsNull(mediaRight.MediaRightVehicleExcept));

            #endregion

            ProductRight productRight = _common.GetProductRight();

            #region Set Product Right Data

            cmd.Parameters.AddWithValue("@productRightAdvertiserAccess", _common.IsNull(productRight.ProductRightAdvertiserAccess));
            cmd.Parameters.AddWithValue("@productRightAdvertiserExcept", _common.IsNull(productRight.ProductRightAdvertiserExcept));
            cmd.Parameters.AddWithValue("@productRightBrandAccess", _common.IsNull(productRight.ProductRightBrandAccess));
            cmd.Parameters.AddWithValue("@productRightBrandExcept", _common.IsNull(productRight.ProductRightBrandExcept));
            cmd.Parameters.AddWithValue("@productRightCategory1Access", _common.IsNull(productRight.ProductRightCategory1Access));
            cmd.Parameters.AddWithValue("@productRightCategory1Except", _common.IsNull(productRight.ProductRightCategory1Except));
            cmd.Parameters.AddWithValue("@productRightCategory2Access", _common.IsNull(productRight.ProductRightCategory2Access));
            cmd.Parameters.AddWithValue("@productRightCategory2Except", _common.IsNull(productRight.ProductRightCategory2Except));
            cmd.Parameters.AddWithValue("@productRightCategory3Access", _common.IsNull(productRight.ProductRightCategory3Access));
            cmd.Parameters.AddWithValue("@productRightCategory3Except", _common.IsNull(productRight.ProductRightCategory3Except));
            cmd.Parameters.AddWithValue("@productRightCategory4Access", _common.IsNull(productRight.ProductRightCategory4Access));
            cmd.Parameters.AddWithValue("@productRightCategory4Except", _common.IsNull(productRight.ProductRightCategory4Except));

            #endregion

            ProductClassification productClass = _common.GetProductClassification();

            #region Set Product Classification Data

            cmd.Parameters.AddWithValue("@productClassAdvertiserAccess1", _common.IsNull(productClass.AdvertiserAccess1));
            cmd.Parameters.AddWithValue("@productClassAdvertiserAccess2", _common.IsNull(productClass.AdvertiserAccess2));
            cmd.Parameters.AddWithValue("@productClassAdvertiserExcept", _common.IsNull(productClass.AdvertiserExcept));
            cmd.Parameters.AddWithValue("@productClassBrandAccess1", _common.IsNull(productClass.BrandAccess1));
            cmd.Parameters.AddWithValue("@productClassBrandAccess2", _common.IsNull(productClass.BrandAccess2));
            cmd.Parameters.AddWithValue("@productClassBrandExcept", _common.IsNull(productClass.BrandExcept));
            cmd.Parameters.AddWithValue("@productClassSubBrandAccess1", _common.IsNull(productClass.SubBrandAccess1));
            cmd.Parameters.AddWithValue("@productClassSubBrandAccess2", _common.IsNull(productClass.SubBrandAccess2));
            cmd.Parameters.AddWithValue("@productClassSubBrandExcept", _common.IsNull(productClass.SubBrandExcept));
            cmd.Parameters.AddWithValue("@productClassProductAccess1", _common.IsNull(productClass.ProductAccess1));
            cmd.Parameters.AddWithValue("@productClassProductAccess2", _common.IsNull(productClass.ProductAccess2));
            cmd.Parameters.AddWithValue("@productClassProductExcept", _common.IsNull(productClass.ProductExcept));
            cmd.Parameters.AddWithValue("@productClassCategory1Access1", _common.IsNull(productClass.Category1Access1));
            cmd.Parameters.AddWithValue("@productClassCategory1Access2", _common.IsNull(productClass.Category1Access2));
            cmd.Parameters.AddWithValue("@productClassCategory1Except", _common.IsNull(productClass.Category1Except));
            cmd.Parameters.AddWithValue("@productClassCategory2Access1", _common.IsNull(productClass.Category2Access1));
            cmd.Parameters.AddWithValue("@productClassCategory2Access2", _common.IsNull(productClass.Category2Access2));
            cmd.Parameters.AddWithValue("@productClassCategory2Except", _common.IsNull(productClass.Category2Except));
            cmd.Parameters.AddWithValue("@productClassCategory3Access1", _common.IsNull(productClass.Category3Access1));
            cmd.Parameters.AddWithValue("@productClassCategory3Access2", _common.IsNull(productClass.Category3Access2));
            cmd.Parameters.AddWithValue("@productClassCategory3Except", _common.IsNull(productClass.Category3Except));
            cmd.Parameters.AddWithValue("@productClassCategory4Access1", _common.IsNull(productClass.Category4Access1));
            cmd.Parameters.AddWithValue("@productClassCategory4Access2", _common.IsNull(productClass.Category4Access2));
            cmd.Parameters.AddWithValue("@productClassCategory4Except", _common.IsNull(productClass.Category4Except));

            #endregion

            MediaClassification mediaClass = _common.GetMediaClassification();

            #region Set Media Classification Data

            cmd.Parameters.AddWithValue("@mediaClassMediaAccess1", _common.IsNull(mediaClass.MediaAccess1));
            cmd.Parameters.AddWithValue("@mediaClassMediaAccess2", _common.IsNull(mediaClass.MediaAccess2));
            cmd.Parameters.AddWithValue("@mediaClassMediaExcept", _common.IsNull(mediaClass.MediaExcept));
            cmd.Parameters.AddWithValue("@mediaClassRegionAccess1", _common.IsNull(mediaClass.RegionAccess1));
            cmd.Parameters.AddWithValue("@mediaClassRegionAccess2", _common.IsNull(mediaClass.RegionAccess2));
            cmd.Parameters.AddWithValue("@mediaClassRegionExcept", _common.IsNull(mediaClass.RegionExcept));
            cmd.Parameters.AddWithValue("@mediaClassVehicleAccess1", _common.IsNull(mediaClass.VehicleAccess1));
            cmd.Parameters.AddWithValue("@mediaClassVehicleAccess2", _common.IsNull(mediaClass.VehicleAccess2));
            cmd.Parameters.AddWithValue("@mediaClassVehicleExcept", _common.IsNull(mediaClass.VehicleExcept));

            #endregion

            AdTypeClassification adtypeClass = _common.GetAdTypeClassification();

            #region Set Media Classification Data

            cmd.Parameters.AddWithValue("@adtypeClassAdTypeAccess1", _common.IsNull(adtypeClass.AdTypeAccess1));
            cmd.Parameters.AddWithValue("@adtypeClassAdTypeAccess2", _common.IsNull(adtypeClass.AdTypeAccess2));
            cmd.Parameters.AddWithValue("@adtypeClassAdTypeExcept", _common.IsNull(adtypeClass.AdTypeExcept));

            #endregion
        }
        #endregion
    }
}
