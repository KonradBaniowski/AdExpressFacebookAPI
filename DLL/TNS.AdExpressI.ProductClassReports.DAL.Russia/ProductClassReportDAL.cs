using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Common.DAL.Russia;
using TNS.AdExpressI.ProductClassReports.DAL.Exceptions;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;

namespace TNS.AdExpressI.ProductClassReports.DAL.Russia
{
    /// <summary>
    /// Default behaviour of DAL layer
    /// </summary>
    public partial class ProductClassReportDAL : TNS.AdExpressI.ProductClassReports.DAL.ProductClassReportDAL
    {
        private const string GetUniversAdvertisersProcedureName = "[dbo].[rp_ax_productanalysis_advertisers_get]";
        private const string GetDataSetProcedureName = "[dbo].[rp_ax_productanalysis_data_get]";
        private readonly CommonDAL _common;

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassReportDAL(WebSession session)
            : base(session)
        {
            _common = new CommonDAL(session, session.CurrentModule);
        }
        #endregion

        #region IProductClassReportsDAL : GetUniversAdvertisers
        /// <summary>
        /// Get Advertisers which are part of the selected univers
        /// </summary>
        /// <param name="exclude">List of Advertiser Ids to exclude from the result</param>
        /// <returns>DataSet with (id_advertiser, advertiser) records</returns>
        public override DataSet GetUniversAdvertisers(string exclude)
        {
            _keyWord = null;
            return GetUniversAdvertisers(exclude, _keyWord);

        }

        /// <summary>
        /// Get Advertisers which are part of the selected univers
        /// </summary>
        /// <param name="exclude">List of Advertiser Ids to exclude from the result</param>
        /// <param name="keyWord">Key word for advertisers to search</param>
        /// <returns>DataSet with (id_advertiser, advertiser) records</returns>
        public override DataSet GetUniversAdvertisers(string exclude, string keyWord)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetUniversAdvertisersProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@filterKeyWord", SqlDbType.VarChar);
                    cmd.Parameters.Add("@excludeAdvertisers", SqlDbType.VarChar);

                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@filterKeyWord"].Value = _common.IsNull(keyWord);
                    cmd.Parameters["@excludeAdvertisers"].Value = _common.IsNull(exclude);

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

                    foreach (DataRow row in table.Rows)
                    {
                        Debug.WriteLine(string.Format("{0:000000} - {1}", row["id_advertiser"], row["advertiser"]));
                    }
                }

                #endregion

                return ds;
            }
            catch (Exception err)
            {
                throw (new ProductClassReportsDALException("GetUniversAdvertisers. Unable to load the results:", err));
            }
        }

        #endregion

        #region DAL Access
        /// <summary>
        /// Build and execute request
        /// It returns required data to the building of a report considering user session parameters.
        /// It builds the request by adding different clauses like select, from joins, product and media selection, rights, language and activation, sorts et groupings
        /// <seealso cref="TNS.AdExpress.Web.Core.Sessions._session"/>
        /// </summary>
        /// <remarks>!!!!!The rules class uses order of the fields in the select clause. It is stringly unadvised to change order of product and medias classification.
        ///	Uses methods:
        ///		void appendSelectClause(Stringuilder);
        ///		void appendFromClause(Stringuilder, String);
        ///		void appendJointClause(Stringuilder);
        ///		void appendSelectionClause(Stringuilder);
        ///		void appendRightClause(Stringuilder);
        ///		void appendActivationLanguageClause(Stringuilder, String);
        ///		void appendRegroupmentAndOrderClause(Stringuilder);
        /// </remarks>
        /// <exception cref="TNS.AdExpress.Domain.Exceptions.NoDataException()">
        /// Thrown when periods is unvalid
        /// </exception>
        /// <exception cref="TNS.AdExpress.Domain.Exceptions.DeliveryFrequencyException()">
        /// Thrown when data delivering frequency is not valid
        /// </exception>
        /// <exception cref="TNS.AdExpressI.ProductClassReport.DAL.Exceptions.ProductClassReportDALException()">
        /// Thrown when errors while connecting to database, runiing request, closing database
        /// </exception>
        /// <returns>DAL result</returns>
        protected override DataSet GetDataSet()
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetDataSetProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@report_format", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@media_level1", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@media_level2", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@media_level3", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@product_level1", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@product_level2", SqlDbType.NVarChar);

                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@report_format"].Value = _reportFormat;

                    int mediaLevelCount = 0;
                    int productLevelCount = 0;

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
                        case CstFormat.PreformatedMediaDetails.vehicleRegionMedia:
                            cmd.Parameters["@media_level1"].Value = "vehicle";
                            cmd.Parameters["@media_level2"].Value = "region";
                            cmd.Parameters["@media_level3"].Value = "media";
                            mediaLevelCount = 3;
                            break;
                    }
                    #endregion

                    #region Set Product Levels
                    switch (_session.PreformatedProductDetail)
                    {
                        //advertiser
                        case CstFormat.PreformatedProductDetails.advertiser:
                            cmd.Parameters["@product_level1"].Value = "advertiser";
                            productLevelCount = 1;
                            break;
                        case CstFormat.PreformatedProductDetails.advertiserBrand:
                            cmd.Parameters["@product_level1"].Value = "advertiser";
                            cmd.Parameters["@product_level2"].Value = "brand";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.advertiserGroup:
                            cmd.Parameters["@product_level1"].Value = "advertiser";
                            cmd.Parameters["@product_level2"].Value = "group";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.advertiserProduct:
                            cmd.Parameters["@product_level1"].Value = "advertiser";
                            cmd.Parameters["@product_level2"].Value = "product";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.advertiserSegment:
                            cmd.Parameters["@product_level1"].Value = "advertiser";
                            cmd.Parameters["@product_level2"].Value = "segment";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.advertiserSubbrand:
                            cmd.Parameters["@product_level1"].Value = "advertiser";
                            cmd.Parameters["@product_level2"].Value = "subbrand";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.advertiserSubSector:
                            cmd.Parameters["@product_level1"].Value = "advertiser";
                            cmd.Parameters["@product_level2"].Value = "subsector";
                            productLevelCount = 2;
                            break;

                        //brand
                        case CstFormat.PreformatedProductDetails.brand:
                            cmd.Parameters["@product_level1"].Value = "brand";
                            productLevelCount = 1;
                            break;
                        case CstFormat.PreformatedProductDetails.brandGroup:
                            cmd.Parameters["@product_level1"].Value = "brand";
                            cmd.Parameters["@product_level2"].Value = "group";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.brandProduct:
                            cmd.Parameters["@product_level1"].Value = "brand";
                            cmd.Parameters["@product_level2"].Value = "product";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.brandSegment:
                            cmd.Parameters["@product_level1"].Value = "brand";
                            cmd.Parameters["@product_level2"].Value = "segment";
                            productLevelCount = 2;
                            break;

                        //group
                        case CstFormat.PreformatedProductDetails.group:
                            cmd.Parameters["@product_level1"].Value = "group";
                            productLevelCount = 1;
                            break;
                        case CstFormat.PreformatedProductDetails.groupAdvertiser:
                            cmd.Parameters["@product_level1"].Value = "group";
                            cmd.Parameters["@product_level2"].Value = "advertiser";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.groupBrand:
                            cmd.Parameters["@product_level1"].Value = "group";
                            cmd.Parameters["@product_level2"].Value = "brand";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.groupProduct:
                            cmd.Parameters["@product_level1"].Value = "group";
                            cmd.Parameters["@product_level2"].Value = "product";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.groupSegment:
                            cmd.Parameters["@product_level1"].Value = "group";
                            cmd.Parameters["@product_level2"].Value = "segment";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.groupSubbrand:
                            cmd.Parameters["@product_level1"].Value = "group";
                            cmd.Parameters["@product_level2"].Value = "subbrand";
                            productLevelCount = 2;
                            break;

                        //product
                        case CstFormat.PreformatedProductDetails.product:
                            cmd.Parameters["@product_level1"].Value = "product";
                            productLevelCount = 1;
                            break;

                        //sector
                        case CstFormat.PreformatedProductDetails.sector:
                            cmd.Parameters["@product_level1"].Value = "sector";
                            productLevelCount = 1;
                            break;
                        case CstFormat.PreformatedProductDetails.sectorAdvertiser:
                            cmd.Parameters["@product_level1"].Value = "sector";
                            cmd.Parameters["@product_level2"].Value = "advertiser";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.sectorBrand:
                            cmd.Parameters["@product_level1"].Value = "sector";
                            cmd.Parameters["@product_level2"].Value = "brand";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.sectorProduct:
                            cmd.Parameters["@product_level1"].Value = "sector";
                            cmd.Parameters["@product_level2"].Value = "product";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.sectorSubsector:
                            cmd.Parameters["@product_level1"].Value = "sector";
                            cmd.Parameters["@product_level2"].Value = "subsector";
                            productLevelCount = 2;
                            break;

                        //segment
                        case CstFormat.PreformatedProductDetails.segment:
                            cmd.Parameters["@product_level1"].Value = "segment";
                            productLevelCount = 1;
                            break;
                        case CstFormat.PreformatedProductDetails.segmentAdvertiser:
                            cmd.Parameters["@product_level1"].Value = "segment";
                            cmd.Parameters["@product_level2"].Value = "advertiser";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.segmentBrand:
                            cmd.Parameters["@product_level1"].Value = "segment";
                            cmd.Parameters["@product_level2"].Value = "brand";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.segmentProduct:
                            cmd.Parameters["@product_level1"].Value = "segment";
                            cmd.Parameters["@product_level2"].Value = "product";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.segmentSubbrand:
                            cmd.Parameters["@product_level1"].Value = "segment";
                            cmd.Parameters["@product_level2"].Value = "subbrand";
                            productLevelCount = 2;
                            break;

                        //subbrand
                        case CstFormat.PreformatedProductDetails.subbrand:
                            cmd.Parameters["@product_level1"].Value = "subbrand";
                            productLevelCount = 1;
                            break;


                        //subSector
                        case CstFormat.PreformatedProductDetails.subSector:
                            cmd.Parameters["@product_level1"].Value = "subsector";
                            productLevelCount = 1;
                            break;
                        case CstFormat.PreformatedProductDetails.subSectorAdvertiser:
                            cmd.Parameters["@product_level1"].Value = "subsector";
                            cmd.Parameters["@product_level2"].Value = "advertiser";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.subSectorBrand:
                            cmd.Parameters["@product_level1"].Value = "subsector";
                            cmd.Parameters["@product_level2"].Value = "brand";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.subSectorGroup:
                            cmd.Parameters["@product_level1"].Value = "subsector";
                            cmd.Parameters["@product_level2"].Value = "group";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.subSectorProduct:
                            cmd.Parameters["@product_level1"].Value = "subsector";
                            cmd.Parameters["@product_level2"].Value = "product";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.subSectorSector:
                            cmd.Parameters["@product_level1"].Value = "subsector";
                            cmd.Parameters["@product_level2"].Value = "sector";
                            productLevelCount = 2;
                            break;
                        case CstFormat.PreformatedProductDetails.subSectorSubbrand:
                            cmd.Parameters["@product_level1"].Value = "subsector";
                            cmd.Parameters["@product_level2"].Value = "subbrand";
                            productLevelCount = 2;
                            break;
                        default:
                            throw new Exception(string.Format("The value of \"PreformatedProductDetail\" is {0}", _session.PreformatedProductDetail));
                    }
                    #endregion

                    string referenceAdvertisers = _session.CustomerDataFilters.ReferencesAdvertisers;
                    string competingAdvertisers = _session.CustomerDataFilters.CompetingAdvertisers;

                    #region Set Advertisers Data

                    cmd.Parameters.Add("@referenceAdvertisers", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@competingAdvertisers", SqlDbType.NVarChar);

                    cmd.Parameters["@referenceAdvertisers"].Value = _common.IsNull(referenceAdvertisers);
                    cmd.Parameters["@competingAdvertisers"].Value = _common.IsNull(competingAdvertisers);

                    #endregion

                    SetSqlParameters(ref cmd, _session.CurrentModule, 0);

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);

                        #region Set table names

                        switch (_reportFormat)
                        {
                            case CstFormat.PreformatedTables.media_X_Year:
                            case CstFormat.PreformatedTables.mediaYear_X_Mensual:
                            case CstFormat.PreformatedTables.mediaYear_X_Cumul:
                                if (ds.Tables.Count == (mediaLevelCount + 1))
                                {
                                    int currentTable = 0;
                                    ds.Tables[currentTable].TableName = "total";

                                    if (mediaLevelCount > 0)
                                    {
                                        ds.Tables[++currentTable].TableName = "classif1_1";
                                    }

                                    if (mediaLevelCount > 1)
                                    {
                                        ds.Tables[++currentTable].TableName = "classif1_2";
                                    }

                                    if (mediaLevelCount > 2)
                                    {
                                        ds.Tables[++currentTable].TableName = "classif1_3";
                                    }
                                }
                                break;
                            case CstFormat.PreformatedTables.product_X_Year:
                            case CstFormat.PreformatedTables.productYear_X_Mensual:
                            case CstFormat.PreformatedTables.productYear_X_Cumul:
                                if (ds.Tables.Count == (productLevelCount + 1))
                                {
                                    int currentTable = 0;
                                    ds.Tables[currentTable].TableName = "total";

                                    if (productLevelCount > 0)
                                    {
                                        ds.Tables[++currentTable].TableName = "classif1_1";
                                    }

                                    if (productLevelCount > 1)
                                    {
                                        ds.Tables[++currentTable].TableName = "classif1_2";
                                    }
                                }
                                break;
                            case CstFormat.PreformatedTables.productMedia_X_Year:
                            case CstFormat.PreformatedTables.productMedia_X_YearMensual:
                            case CstFormat.PreformatedTables.productYear_X_Media:
                                if (ds.Tables.Count == ((mediaLevelCount + 1) + productLevelCount * (mediaLevelCount + 1)))
                                {
                                    int currentTable = 0;
                                    ds.Tables[currentTable].TableName = "total";
                                    for (int i = 1; i < mediaLevelCount + 1; ++i)
                                    {
                                        ds.Tables[++currentTable].TableName = string.Format("total_{0}", i);
                                    }

                                    if (productLevelCount > 0)
                                    {
                                        ds.Tables[++currentTable].TableName = "classif1_1";
                                        for (int i = 1; i < mediaLevelCount + 1; ++i)
                                        {
                                            ds.Tables[++currentTable].TableName = string.Format("classif2_1_{0}", i);
                                        }
                                    }

                                    if (productLevelCount > 1)
                                    {
                                        ds.Tables[++currentTable].TableName = "classif1_2";
                                        for (int i = 1; i < mediaLevelCount + 1; ++i)
                                        {
                                            ds.Tables[++currentTable].TableName = string.Format("classif2_2_{0}", i);
                                        }
                                    }
                                }
                                break;
                            case CstFormat.PreformatedTables.mediaProduct_X_Year:
                            case CstFormat.PreformatedTables.mediaProduct_X_YearMensual:
                                if (ds.Tables.Count == ((productLevelCount + 1) + mediaLevelCount * (productLevelCount + 1)))
                                {
                                    int currentTable = 0;
                                    ds.Tables[currentTable].TableName = "total";
                                    for (int i = 1; i < productLevelCount + 1; ++i)
                                    {
                                        ds.Tables[++currentTable].TableName = string.Format("total_{0}", i);
                                    }

                                    if (mediaLevelCount > 0)
                                    {
                                        ds.Tables[++currentTable].TableName = "classif1_1";
                                        for (int i = 1; i < productLevelCount + 1; ++i)
                                        {
                                            ds.Tables[++currentTable].TableName = string.Format("classif2_1_{0}", i);
                                        }
                                    }

                                    if (mediaLevelCount > 1)
                                    {
                                        ds.Tables[++currentTable].TableName = "classif1_2";
                                        for (int i = 1; i < productLevelCount + 1; ++i)
                                        {
                                            ds.Tables[++currentTable].TableName = string.Format("classif2_2_{0}", i);
                                        }
                                    }

                                    if (mediaLevelCount > 2)
                                    {
                                        ds.Tables[++currentTable].TableName = "classif1_3";
                                        for (int i = 1; i < productLevelCount + 1; ++i)
                                        {
                                            ds.Tables[++currentTable].TableName = string.Format("classif2_3_{0}", i);
                                        }
                                    }
                                }
                                break;
                        }

                        #endregion

                        #region Check PersonalizedElementsOnly

                        if (_session.PersonalizedElementsOnly)
                        {
                            foreach (DataTable table in ds.Tables)
                            {
                                if (table.Columns.Contains("INREF") && table.Columns.Contains("INCOMP"))
                                {
                                    foreach (DataRow row in table.Rows)
                                    {
                                        if (Convert.ToBoolean(row["INREF"]) == false && Convert.ToBoolean(row["INCOMP"]) == false)
                                        {
                                            row.Delete();
                                        }
                                    }

                                    table.AcceptChanges();
                                }
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
                throw (new ProductClassReportsDALException("GetDataSet. Unable to load the results:", err));
            }
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
            string selectedCampaignType = _common.GetSelectedCampaignType();
            string selectedUnit = _session.CustomerDataFilters.SelectedUnit.Id.ToString();

            #region Set Selected data

            cmd.Parameters.AddWithValue("@selectedComparativePeriod", comparativeStudy);
            cmd.Parameters.AddWithValue("@selectedCampaignType", _common.IsNull(selectedCampaignType));
            cmd.Parameters.AddWithValue("@selectedUnit", selectedUnit);

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
