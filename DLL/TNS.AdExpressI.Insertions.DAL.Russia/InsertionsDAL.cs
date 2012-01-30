using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Common.DAL.Russia;
using TNS.AdExpressI.Insertions.DAL.Exceptions;
using NavigationModule = TNS.AdExpress.Domain.Web.Navigation.Module;
using PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;

namespace TNS.AdExpressI.Insertions.DAL.Russia
{
    public partial class InsertionsDAL : TNS.AdExpressI.Insertions.DAL.InsertionsDAL
    {
        private const string GetVehiclesIdsProcedureName = "[dbo].[rp_ax_insver_vehicles_id_get]";
        private const string GetPresentVehiclesProcedureName = "[dbo].[rp_ax_insver_present_vehicles_get]";
        private const string GetInsertionsDataProcedureName = "[dbo].[rp_ax_insver_insertions_data_get]";
        private const string GetCreativesDataProcedureName = "[dbo].[rp_ax_insver_creatives_data_get]";
        private const string GetVersionProcedureName = "[dbo].[rp_ax_insver_version_get]";
        private const string GetVersionsProcedureName = "[dbo].[rp_ax_insver_versions_get]";
        private readonly CommonDAL _common;

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="module">Current Module</param>
        public InsertionsDAL(WebSession session, Int64 moduleId)
            : base(session, moduleId)
        {
            _common = new CommonDAL(session, moduleId);
        }
        #endregion

        #region Get Vehicles Ids
        /// <summary>
        /// Get Media types identifiers according to levels of classification selected by the customer
        /// <example> If the filters variable contain : (media type id = 1)/(sub media id = 11)
        ///     // get list of media type identifiers (in our classification we use vehicle for media type)
        ///     select distinct id_vehicle 
        ///     // A view that contains all the media classification (id_vehicle(media type), id_media(vehicle), id_interest_center, id_basic_media, id_title ...) 
        ///     from adexpr03.all_media_44
        ///     // The data is filtered by the media type 1
        ///     where id_vehicle = 1
        ///     // The data is filtered by the sub media 11
        ///     and id_category = 11
        /// </example>
        /// </summary>
        /// <param name="filters">Dictionary of [classification levels, value(corresponds to the value of the classification level in the database)]</param>
        /// <returns>Media types identifiers list</returns>
        public override Int64[] GetVehiclesIds(Dictionary<DetailLevelItemInformation, Int64> filters)
        {
            try
            {
                List<Int64> result = new List<Int64>();

                #region Get filter data

                string levelMedia = string.Empty;
                string levelVehicle = string.Empty;

                foreach (DetailLevelItemInformation item in filters.Keys)
                {
                    if (filters[item] >= 0)
                    {
                        switch (item.Id)
                        {
                            case DetailLevelItemInformation.Levels.vehicle:
                                levelMedia = Convert.ToString(filters[item]);
                                break;
                            case DetailLevelItemInformation.Levels.media:
                                levelVehicle = Convert.ToString(filters[item]);
                                break;
                        }
                    }
                }

                #endregion

                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetVehiclesIdsProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@levelMedia", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@levelVehicle", SqlDbType.NVarChar);

                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@levelMedia"].Value = _common.IsNull(levelMedia);
                    cmd.Parameters["@levelVehicle"].Value = _common.IsNull(levelVehicle);

                    // SP Execute
                    DataTable returnTable = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(returnTable);

                        foreach (DataRow row in returnTable.Rows)
                        {
                            result.Add(Convert.ToInt64(row["id"]));
                        }
                    }

                    conn.Close();
                }

                return result.ToArray();
            }
            catch (Exception err)
            {
                throw (new InsertionsDALException("GetVehiclesIds. Unable to load the results:", err));
            }
        }
        #endregion

        #region GetPresentVehicles
        /// <summary>
        /// Get list of media type for which there're data in the database
        /// </summary>
        /// <param name="vehicles">List of Media type object to check</param>
        /// <param name="filters">List identiifers selected by the customer. Used as data filters in the query ex. "</param>
        /// <param name="fromDate">User Period beginning</param>
        /// <param name="toDate">User Period End</param>
        /// <param name="universId">User Univers Selection</param>
        /// <param name="module">Current Module</param>
        /// <param name="sloganNotNull">True if slogan not null</param>
        /// <returns>List of vehicles present</returns>
        public override List<VehicleInformation> GetPresentVehicles(List<VehicleInformation> vehicles, string filters, int fromDate, int toDate, int universId, NavigationModule module, bool sloganNotNull)
        {
            try
            {
                List<VehicleInformation> result = new List<VehicleInformation>();

                foreach (VehicleInformation v in vehicles)
                {
                    using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                    {
                        SqlCommand cmd = conn.CreateCommand();
                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = GetPresentVehiclesProcedureName;
                        cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                        cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                        cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);

                        SetSqlParameters(ref cmd, _module.Id, v.DatabaseId, fromDate, toDate, universId, filters);

                        //SP Execute

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if (table.Rows.Count == 1 && table.Columns.Contains("rows"))
                            {
                                int rows = Convert.ToInt32(table.Rows[0]["rows"]);
                                if (rows > 0)
                                {
                                    result.Add(VehiclesInformation.Get(v.DatabaseId));
                                }
                            }
                        }

                        conn.Close();
                    }
                }

                return result;
            }
            catch (Exception err)
            {
                throw (new InsertionsDALException("GetPresentVehicles. Unable to load the results:", err));
            }
        }
        #endregion

        #region Insertions
        /// <summary>
        /// Extract advertising detail for insertions details 
        /// </summary>
        /// <param name="vehicle">Vehicle Information (differents informations about a media type like databaseId, showInsertions..., this object is more detailed above)</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// <param name="universId">User Univers Selection (correspondig to which is the current univers if we have a competitor study)</param>
        /// <param name="filters">Filters Identifiers (A list of values correspondig to the classification levels)</param>
        /// <returns>Advertising detail Data</returns>
        public override DataSet GetInsertionsData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetInsertionsDataProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@level1", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@level2", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@level3", SqlDbType.NVarChar);

                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@level1"].Value = (_session.DetailLevel.LevelIds.Count > 0) ? _session.DetailLevel.LevelIds[0] : string.Empty;
                    cmd.Parameters["@level2"].Value = (_session.DetailLevel.LevelIds.Count > 1) ? _session.DetailLevel.LevelIds[1] : string.Empty;
                    cmd.Parameters["@level3"].Value = (_session.DetailLevel.LevelIds.Count > 2) ? _session.DetailLevel.LevelIds[2] : string.Empty;

                    SetSqlParameters(ref cmd, _module.Id, vehicle.DatabaseId, fromDate, toDate, universId, filters);

                    // SP Execute

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);

                        if (ds.Tables.Count == 1)
                        {
                            DataTable dt = ds.Tables[0];

                            #region Fill level1

                            if (_session.DetailLevel.Levels.Count > 0)
                            {
                                DetailLevelItemInformation detailLevelItemInformation1 = (DetailLevelItemInformation)_session.DetailLevel.Levels[0];
                                string levelFieldAlias1 = detailLevelItemInformation1.DataBaseAliasField ?? detailLevelItemInformation1.DataBaseField;
                                string levelIdFieldAlias1 = detailLevelItemInformation1.DataBaseAliasIdField ?? detailLevelItemInformation1.DataBaseIdField;
                                if (dt.Columns.Contains(levelFieldAlias1))
                                {
                                    dt.Columns[levelFieldAlias1].ColumnName = "temp1";
                                }
                                if (dt.Columns.Contains(levelIdFieldAlias1))
                                {
                                    dt.Columns[levelIdFieldAlias1].ColumnName = "id_temp1";
                                }

                                dt.Columns["level1"].ColumnName = levelFieldAlias1;
                                dt.Columns["id_level1"].ColumnName = levelIdFieldAlias1;
                            }

                            #endregion

                            #region Fill level2

                            if (_session.DetailLevel.Levels.Count > 1)
                            {
                                DetailLevelItemInformation detailLevelItemInformation2 = (DetailLevelItemInformation)_session.DetailLevel.Levels[1];
                                string levelFieldAlias2 = detailLevelItemInformation2.DataBaseAliasField ?? detailLevelItemInformation2.DataBaseField;
                                string levelIdFieldAlias2 = detailLevelItemInformation2.DataBaseAliasIdField ?? detailLevelItemInformation2.DataBaseIdField;
                                if (dt.Columns.Contains(levelFieldAlias2))
                                {
                                    dt.Columns[levelFieldAlias2].ColumnName = "temp2";
                                }
                                if (dt.Columns.Contains(levelIdFieldAlias2))
                                {
                                    dt.Columns[levelIdFieldAlias2].ColumnName = "id_temp2";
                                }

                                dt.Columns["level2"].ColumnName = levelFieldAlias2;
                                dt.Columns["id_level2"].ColumnName = levelIdFieldAlias2;
                            }

                            #endregion

                            #region Fill level3

                            if (_session.DetailLevel.Levels.Count > 2)
                            {
                                DetailLevelItemInformation detailLevelItemInformation3 = (DetailLevelItemInformation)_session.DetailLevel.Levels[2];
                                string levelFieldAlias3 = detailLevelItemInformation3.DataBaseAliasField ?? detailLevelItemInformation3.DataBaseField;
                                string levelIdFieldAlias3 = detailLevelItemInformation3.DataBaseAliasIdField ?? detailLevelItemInformation3.DataBaseIdField;
                                if (dt.Columns.Contains(levelFieldAlias3))
                                {
                                    dt.Columns[levelFieldAlias3].ColumnName = "temp3";
                                }
                                if (dt.Columns.Contains(levelIdFieldAlias3))
                                {
                                    dt.Columns[levelIdFieldAlias3].ColumnName = "id_temp3";
                                }

                                dt.Columns["level3"].ColumnName = levelFieldAlias3;
                                dt.Columns["id_level3"].ColumnName = levelIdFieldAlias3;
                            }

                            #endregion
                        }
                    }

                    conn.Close();
                }

                return ds;
            }
            catch (Exception err)
            {
                throw (new InsertionsDALException("GetInsertionsData. Unable to load the results:", err));
            }
        }
        #endregion

        #region Creatives
        /// <summary>
        /// Extract advertising detail for creatives details 
        /// </summary>
        /// <param name="vehicle">Vehicle Information (differents informations about a media type like databaseId, showInsertions..., this object is more detailed above)</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// <param name="universId">User Univers Selection (correspondig to which is the current univers if we have a competitor study)</param>
        /// <param name="filters">Filters Identifiers (A list of values correspondig to the classification levels)</param>
        /// <returns>Advertising detail Data</returns>		
        public override DataSet GetCreativesData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetCreativesDataProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);

                    SetSqlParameters(ref cmd, _module.Id, vehicle.DatabaseId, fromDate, toDate, universId, filters);

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);
                    }

                    conn.Close();
                }

                return ds;
            }
            catch (Exception err)
            {
                throw (new InsertionsDALException("GetCreativesData. Unable to load the results:", err));
            }
        }
        #endregion

        #region Get version detail
        /// <summary>
        /// Get one verion informations like :
        /// ID version , product label,group label,advertiser label
        /// </summary>
        /// <remarks>Use only for media type Tv in France</remarks>
        /// <param name="idVersion">ID version</param>    
        /// <param name="idVehicle">ID Media type</param>
        /// <returns>ID version , product label,group label,advertiser label</returns>
        public override DataSet GetVersion(string idVersion, long idVehicle)
        {
            try
            {
                DataSet ds = new DataSet();

                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetVersionProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@moduleId", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@mediaId", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@version", SqlDbType.NVarChar);

                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@moduleId"].Value = _module.Id;
                    cmd.Parameters["@mediaId"].Value = idVehicle;
                    cmd.Parameters["@version"].Value = idVersion.Substring(0, idVersion.IndexOf("."));

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);
                    }

                    conn.Close();
                }

                return ds;
            }
            catch (Exception err)
            {
                throw (new InsertionsDALException("GetVersion. Unable to load the results:", err));
            }
        }
        #endregion

        #region Get versions

        /// <summary>
        /// Get version(s) data. The query will return the following fields, in order :
        /// "id_advertiser" : advertiser identifier.
        /// "advertiser" : advertiser label.
        /// "id_product" : product identifier.
        /// "product" : product label.
        /// "id_vehicle" : media type identifier.
        /// "vehicle" :  media type label.
        /// "id_slogan" : version identifier.
        /// "date_media_num" : publication date with format YYYYMMDD.
        /// "id_media" : vehicle identifier.
        /// "advertDimension" : It will be the duration of spot for media type RADIO  and TV. 
        /// The FORMAT of Ad for media type PRESS. The dimension and extension for INTERNET.
        /// The type of board for media type Outdoor.
        /// "associated_file" : creative file.
        /// 
        /// The query data will be filtered with the following parameters :
        /// 
        /// - The List of media type selected : 
        /// <code> string mediaTypeStringIds = _session.CustomerDataFilters.SelectedMediaType;</code>
        /// - The selected period represented by parameters <code>beginningDate</code> and <code>endDate</code> which correspond respectively to the begin and end dates of period.
        /// - The customer  product classification rights, obtained via variable <code>_session.CustomerDataFilters.ProductsRights</code>.
        /// - The customer media classification rights, obtained via variable <code>_session.CustomerDataFilters.MediaRights</code>.
        /// - The  product classification items selected via variable <code>_session.CustomerDataFilters.PrincipalProductUniverses</code>
        /// - Allowed media type via variable <code>module.AllowedMediaUniverse.VehicleList</code>
        /// 
        /// The query will be as the following example :        
        /// <code> 
        /// //Select fields
        /// SELECT distinct id_advertiser,advertiser,id_product,product,id_vehicle, vehicle  ,  id_slogan, date_media_num ,id_media, advertDimension, associated_file
        /// 
        /// FROM My_Table
        /// 
        /// WHERE
        /// 
        /// //Filtering with selected media types
        /// id_vehicle in (100,103,200,400,500,600)
        /// 
        /// //Filtering with period
        ///  AND date_media_num 20110101 //beginingDate
        ///  AND date_media_num 20110131 //endDate
        ///  
        /// //Filtering with product classification rights
        /// AND id_advertiser in (20055,46888,2233655,82100)
        /// 
        /// //Filtering with media classification rights
        /// AND id_media in (77748,8655,835,7536)
        /// 
        /// //Filtering with product items selected
        /// AND id_product in (546546,8922,625887,1354,798692)
        /// 
        /// //Filtering with Allowed media type via
        /// AND id_vehicle in (100,103,200,400,500,600)
        /// 
        /// ORDER BY advertiser,id_advertiser,product ,id_product, vehicle,id_vehicle,id_slogan,associated_file,date_media_num
        /// </code>
        /// </summary>
        /// <param name="beginningDate">date beginning (YYYYMMDD)</param>
        /// <param name="endDate">date end (YYYYMMDD)</param>
        /// <returns>versions data ["id_advertiser","advertiser","id_product","product","id_vehicle","vehicle","id_slogan","date_media_num","id_media","advertDimension", "associated_file" </returns>
        public override DataSet GetVersions(string beginDate, string endDate)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetVersionsProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);

                    SetSqlParameterForVersions(ref cmd, _session.CurrentModule, 0, beginDate, endDate);

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);
                    }

                    conn.Close();
                }

                return ds;
            }
            catch (Exception err)
            {
                throw new InsertionsDALException("GetVersions. Impossible to obtain versions data", err);
            }
        }

        #endregion

        #region Set Sql Parameters
        private void SetSqlParameters(ref SqlCommand cmd, long moduleId, long mediaId, int fromDate, int toDate, int universId, string filters)
        {
            string beginDate = fromDate.ToString();
            string endDate = toDate.ToString();

            if (_session.PeriodType == PeriodType.dateToDate)
            {
                int begin = Convert.ToInt32(_session.PeriodBeginningDate);
                beginDate = begin > fromDate ? begin.ToString() : fromDate.ToString();

                int end = Convert.ToInt32(_session.PeriodEndDate);
                endDate = end < toDate ? end.ToString() : toDate.ToString();
            }

            #region Set Date and Identity data

            cmd.Parameters.AddWithValue("@fromDate", beginDate);
            cmd.Parameters.AddWithValue("@toDate", endDate);
            cmd.Parameters.AddWithValue("@moduleId", moduleId);
            cmd.Parameters.AddWithValue("@mediaId", mediaId);

            #endregion

            string selectedVehicle = _common.GetSelectedVehicles();
            string selectedMedia = _common.GetSelectedMediaTypes();
            string selectedVersion = _common.GetSelectedVersions();
            string selectedCampaignType = _common.GetSelectedCampaignType();

            #region Set Selected data

            cmd.Parameters.AddWithValue("@selectedMedia", _common.IsNull(selectedMedia));
            cmd.Parameters.AddWithValue("@selectedVehicle", _common.IsNull(selectedVehicle));
            cmd.Parameters.AddWithValue("@selectedVersion", _common.IsNull(selectedVersion));
            cmd.Parameters.AddWithValue("@selectedCampaignType", _common.IsNull(selectedCampaignType));
            cmd.Parameters.AddWithValue("@selectedSloganList", _common.IsNull(_session.SloganIdList));

            #endregion

            DetailLevel detailLevel = _common.GetDetailLevel(filters);

            #region Set Filter Details

            cmd.Parameters.AddWithValue("@detailAdvertisment", _common.IsNull(detailLevel.DetailAdvertisment));
            cmd.Parameters.AddWithValue("@detailAdType", _common.IsNull(detailLevel.DetailAdType));
            cmd.Parameters.AddWithValue("@detailRegion", _common.IsNull(detailLevel.DetailRegion));
            cmd.Parameters.AddWithValue("@detailMedia", _common.IsNull(detailLevel.DetailMedia));
            cmd.Parameters.AddWithValue("@detailVehicle", _common.IsNull(detailLevel.DetailVehicle));
            cmd.Parameters.AddWithValue("@detailAdvertiser", _common.IsNull(detailLevel.DetailAdvertiser));
            cmd.Parameters.AddWithValue("@detailBrand", _common.IsNull(detailLevel.DetailBrand));
            cmd.Parameters.AddWithValue("@detailSubBrand", _common.IsNull(detailLevel.DetailSubBrand));
            cmd.Parameters.AddWithValue("@detailProduct", _common.IsNull(detailLevel.DetailProduct));
            cmd.Parameters.AddWithValue("@detailCategory1", _common.IsNull(detailLevel.DetailCategory1));
            cmd.Parameters.AddWithValue("@detailCategory2", _common.IsNull(detailLevel.DetailCategory2));
            cmd.Parameters.AddWithValue("@detailCategory3", _common.IsNull(detailLevel.DetailCategory3));
            cmd.Parameters.AddWithValue("@detailCategory4", _common.IsNull(detailLevel.DetailCategory4));

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

            ProductLevel productLevel = _common.GetProductLevel(false);

            #region Set Filter Levels Data

            cmd.Parameters.AddWithValue("@productLevelAdvertiserAccess", _common.IsNull(productLevel.ProductLevelAdvertiserAccess));
            cmd.Parameters.AddWithValue("@productLevelBrandAccess", _common.IsNull(productLevel.ProductLevelBrandAccess));
            cmd.Parameters.AddWithValue("@productLevelSubBrandAccess", _common.IsNull(productLevel.ProductLevelSubBrandAccess));
            cmd.Parameters.AddWithValue("@productLevelProductAccess", _common.IsNull(productLevel.ProductLevelProductAccess));
            cmd.Parameters.AddWithValue("@productLevelCategory1Access", _common.IsNull(productLevel.ProductLevelCategory1Access));
            cmd.Parameters.AddWithValue("@productLevelCategory2Access", _common.IsNull(productLevel.ProductLevelCategory2Access));
            cmd.Parameters.AddWithValue("@productLevelCategory3Access", _common.IsNull(productLevel.ProductLevelCategory3Access));
            cmd.Parameters.AddWithValue("@productLevelCategory4Access", _common.IsNull(productLevel.ProductLevelCategory4Access));

            #endregion

            ProductClassification productClass = _common.GetProductClassification(universId);

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

            MediaClassification mediaClass = _common.GetMediaScheduleClassification();

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

            RegionClassification regionClass = _common.GetRegionClassification();

            #region Set Region Classification Data

            cmd.Parameters.AddWithValue("@regionClassRegionAccess1", _common.IsNull(regionClass.RegionAccess1));
            cmd.Parameters.AddWithValue("@regionClassRegionAccess2", _common.IsNull(regionClass.RegionAccess2));
            cmd.Parameters.AddWithValue("@regionClassRegionExcept", _common.IsNull(regionClass.RegionExcept));
            cmd.Parameters.AddWithValue("@regionClassVehicleAccess1", _common.IsNull(regionClass.VehicleAccess1));
            cmd.Parameters.AddWithValue("@regionClassVehicleAccess2", _common.IsNull(regionClass.VehicleAccess2));
            cmd.Parameters.AddWithValue("@regionClassVehicleExcept", _common.IsNull(regionClass.VehicleExcept));

            #endregion

            AdTypeClassification adtypeClass = _common.GetAdTypeClassification();

            #region Set Media Classification Data

            cmd.Parameters.AddWithValue("@adtypeClassAdTypeAccess1", _common.IsNull(adtypeClass.AdTypeAccess1));
            cmd.Parameters.AddWithValue("@adtypeClassAdTypeAccess2", _common.IsNull(adtypeClass.AdTypeAccess2));
            cmd.Parameters.AddWithValue("@adtypeClassAdTypeExcept", _common.IsNull(adtypeClass.AdTypeExcept));

            #endregion
        }
        #endregion

        #region Set Sql Parameters
        private void SetSqlParameterForVersions(ref SqlCommand cmd, long moduleId, long mediaId, string beginDate, string endDate)
        {
            #region Set Date and Identity data

            cmd.Parameters.AddWithValue("@fromDate", beginDate);
            cmd.Parameters.AddWithValue("@toDate", endDate);
            cmd.Parameters.AddWithValue("@moduleId", moduleId);
            cmd.Parameters.AddWithValue("@mediaId", mediaId);

            #endregion

            string selectedVehicle = _common.GetSelectedVehicles();
            string selectedMedia = _common.GetSelectedMediaTypes();
            string selectedVersion = _common.GetSelectedVersions();
            string selectedCampaignType = _session.CustomerDataFilters.CampaignType.ToString();
            string resultPageMediaAccess = string.Empty;

            #region Set Selected data

            cmd.Parameters.AddWithValue("@selectedMedia", _common.IsNull(selectedMedia));
            cmd.Parameters.AddWithValue("@selectedVehicle", _common.IsNull(selectedVehicle));
            cmd.Parameters.AddWithValue("@selectedVersion", _common.IsNull(selectedVersion));
            cmd.Parameters.AddWithValue("@selectedCampaignType", _common.IsNull(selectedCampaignType));
            cmd.Parameters.AddWithValue("@resultPageMediaAccess", _common.IsNull(resultPageMediaAccess));

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

            ProductLevel productLevel = _common.GetProductLevel(false);

            #region Set Filter Levels Data

            cmd.Parameters.AddWithValue("@productLevelAdvertiserAccess", _common.IsNull(productLevel.ProductLevelAdvertiserAccess));
            cmd.Parameters.AddWithValue("@productLevelBrandAccess", _common.IsNull(productLevel.ProductLevelBrandAccess));
            cmd.Parameters.AddWithValue("@productLevelSubBrandAccess", _common.IsNull(productLevel.ProductLevelSubBrandAccess));
            cmd.Parameters.AddWithValue("@productLevelProductAccess", _common.IsNull(productLevel.ProductLevelProductAccess));
            cmd.Parameters.AddWithValue("@productLevelCategory1Access", _common.IsNull(productLevel.ProductLevelCategory1Access));
            cmd.Parameters.AddWithValue("@productLevelCategory2Access", _common.IsNull(productLevel.ProductLevelCategory2Access));
            cmd.Parameters.AddWithValue("@productLevelCategory3Access", _common.IsNull(productLevel.ProductLevelCategory3Access));
            cmd.Parameters.AddWithValue("@productLevelCategory4Access", _common.IsNull(productLevel.ProductLevelCategory4Access));

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

            MediaClassification mediaClass = _common.GetMediaScheduleClassification();

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

            RegionClassification regionClass = _common.GetRegionClassification();

            #region Set Region Classification Data

            cmd.Parameters.AddWithValue("@regionClassRegionAccess1", _common.IsNull(regionClass.RegionAccess1));
            cmd.Parameters.AddWithValue("@regionClassRegionAccess2", _common.IsNull(regionClass.RegionAccess2));
            cmd.Parameters.AddWithValue("@regionClassRegionExcept", _common.IsNull(regionClass.RegionExcept));
            cmd.Parameters.AddWithValue("@regionClassVehicleAccess1", _common.IsNull(regionClass.VehicleAccess1));
            cmd.Parameters.AddWithValue("@regionClassVehicleAccess2", _common.IsNull(regionClass.VehicleAccess2));
            cmd.Parameters.AddWithValue("@regionClassVehicleExcept", _common.IsNull(regionClass.VehicleExcept));

            #endregion

            AdTypeClassification adtypeClass = _common.GetAdTypeClassification();

            #region Set AdType Classification Data

            cmd.Parameters.AddWithValue("@adtypeClassAdTypeAccess1", _common.IsNull(adtypeClass.AdTypeAccess1));
            cmd.Parameters.AddWithValue("@adtypeClassAdTypeAccess2", _common.IsNull(adtypeClass.AdTypeAccess2));
            cmd.Parameters.AddWithValue("@adtypeClassAdTypeExcept", _common.IsNull(adtypeClass.AdTypeExcept));

            #endregion
        }
        #endregion
    }
}
