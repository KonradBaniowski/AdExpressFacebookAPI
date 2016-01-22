using System;
using System.Data;
using System.Data.SqlClient;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Common.DAL.Russia;
using TNS.AdExpressI.MediaSchedule.DAL.Exceptions;

namespace TNS.AdExpressI.MediaSchedule.DAL.Russia
{
    public partial class MediaScheduleResultDAL : DAL.MediaScheduleResultDAL
    {
        private const string GetMediaScheduleDataProcedureName = "[dbo].[rp_ax_mediaschedule_data_get]";
        private readonly CommonDAL _common;

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Report period</param>
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period)
            : base(session, period)
        {
            _common = new CommonDAL(session, session.CurrentModule);
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Report period</param>
        /// <param name="idVehicle">Id of the vehicle</param>
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period, Int64 idVehicle)
            : base(session, period, idVehicle)
        {
            _common = new CommonDAL(session, session.CurrentModule);
        }
        #endregion

        #region GetMediaScheduleData RUSSIA
        /// <summary>
        /// Get Data tables  to build Media Schedule report
        /// The method will return :
        /// - 1 Data table for line TOTAL
        /// - N data tables for N levels
        ///
        /// The format of the data will be :
        /// [identifier Level1, Label level 1,...,identifier LevelN, Label level N,date_num,period_count,unit selected]
        /// </summary>
        /// <returns>DataSet containing Data</returns>
        public override DataSet GetMediaScheduleData()
        {
            try
            {
                string selectedUnit = _session.GetSelectedUnit().Id.ToString();

                DataSet ds = new DataSet();
                using (SqlConnection conn = (SqlConnection)_common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = GetMediaScheduleDataProcedureName;
                    cmd.CommandTimeout = CommonDAL.ProcedureTimeout;
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@id_level", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@level1", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@level2", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@level3", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@level4", SqlDbType.NVarChar);


                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@id_level"].Value = "total";
                    cmd.Parameters["@level1"].Value = DBNull.Value;
                    cmd.Parameters["@level2"].Value = DBNull.Value;
                    cmd.Parameters["@level3"].Value = DBNull.Value;
                    cmd.Parameters["@level4"].Value = DBNull.Value;

                    cmd.Parameters["@id_level"].Value = string.Format("level{0}", _session.GenericMediaDetailLevel.GetNbLevels);

                    GenericDetailLevel detailLevel = _session.GenericMediaDetailLevel;
                    for (int tableLevels = 0; tableLevels < _session.GenericMediaDetailLevel.GetNbLevels; ++tableLevels)
                    {
                        string paramName = string.Format("@level{0}", tableLevels + 1);
                        cmd.Parameters[paramName].Value = detailLevel.LevelIds[tableLevels];
                    }

                    SetSqlParameters(ref cmd, _session.CurrentModule, 0);

                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);
                    }

                    int tableIndex = 0;
                    if (ds.Tables.Count == _session.GenericMediaDetailLevel.GetNbLevels + 1)
                    {
                        ds.Tables[tableIndex++].TableName = "total";
                        ds.Tables["total"].Columns["unit"].ColumnName = selectedUnit;
                        for (int tableLevels = 0; tableLevels < _session.GenericMediaDetailLevel.GetNbLevels; ++tableLevels)
                        {
                            string tableName = string.Format("level{0}", tableLevels + 1);

                            ds.Tables[tableIndex++].TableName = tableName;
                            ds.Tables[tableName].Columns["unit"].ColumnName = selectedUnit;

                            for (int level = 0; level < tableLevels + 1; ++level)
                            {
                                #region Rename table field names

                                DetailLevelItemInformation levelItemInfo = (DetailLevelItemInformation)detailLevel.Levels[level];
                                string temp = string.Format("temp{0}", level + 1);
                                string fieldLevel = string.Format("level{0}", level + 1);
                                string idtemp = string.Format("id_temp{0}", level + 1);
                                string fieldIdLevel = string.Format("id_level{0}", level + 1);

                                string levelFieldAlias = levelItemInfo.DataBaseAliasField ?? levelItemInfo.DataBaseField;
                                if (ds.Tables[tableName].Columns.Contains(levelFieldAlias))
                                {
                                    ds.Tables[tableName].Columns[levelFieldAlias].ColumnName = temp;
                                }

                                string levelIdFieldAlias = levelItemInfo.DataBaseAliasIdField ?? levelItemInfo.DataBaseIdField;
                                if (ds.Tables[tableName].Columns.Contains(levelIdFieldAlias))
                                {
                                    ds.Tables[tableName].Columns[levelIdFieldAlias].ColumnName = idtemp;
                                }


                                ds.Tables[tableName].Columns[fieldLevel].ColumnName = levelFieldAlias;
                                ds.Tables[tableName].Columns[fieldIdLevel].ColumnName = levelIdFieldAlias;

                                #endregion
                            }
                        }
                    }

                    conn.Close();
                }

                return ds;
            }
            catch (Exception err)
            {
                throw (new MediaScheduleDALException("GetMediaScheduleData. Unable to load the results:", err));
            }
        }
        #endregion

        #region Set Sql Parameters
        private void SetSqlParameters(ref SqlCommand cmd, long moduleId, long mediaId)
        {
            string beginDate = FrameWork.Date.DateString.DateTimeToYYYYMMDD(_period.Begin);
            string endDate = FrameWork.Date.DateString.DateTimeToYYYYMMDD(_period.End);

            #region Set Date and Identity data

            cmd.Parameters.AddWithValue("@fromDate", beginDate);
            cmd.Parameters.AddWithValue("@toDate", endDate);
            cmd.Parameters.AddWithValue("@moduleId", moduleId);
            cmd.Parameters.AddWithValue("@mediaId", mediaId);

            #endregion

            string selectedVehicle = _common.GetSelectedVehicles();
            string selectedMedia = (_vehicleId < 0) ? _session.CustomerDataFilters.SelectedMediaType : _vehicleId.ToString();
            string selectedVersion = _common.GetSelectedVersions(_period.PeriodDetailLEvel);
            string selectedCampaignType = _session.CustomerDataFilters.CampaignType.ToString();
            string selectedPeriod = _period.PeriodDetailLEvel.ToString();
            string selectedUnit = _session.GetSelectedUnit().Id.ToString();

            //List of Media type allowed for current result Page [ex : 100,101,102,103,200,201,202,300,400,500,600,700]
            ResultPageInformation resPageInfo = _session.CustomerLogin.GetModule(_session.CurrentModule).GetResultPageInformation(_session.CurrentTab);
            string resultPageMediaAccess = (resPageInfo != null) ? resPageInfo.AllowedMediaUniverse.VehicleList : string.Empty;

            #region Set Selected data

            cmd.Parameters.AddWithValue("@selectedMedia", _common.IsNull(selectedMedia));
            cmd.Parameters.AddWithValue("@selectedVehicle", _common.IsNull(selectedVehicle));
            cmd.Parameters.AddWithValue("@selectedVersion", _common.IsNull(selectedVersion));
            cmd.Parameters.AddWithValue("@selectedCampaignType", _common.IsNull(selectedCampaignType));
            cmd.Parameters.AddWithValue("@selectedPeriod", selectedPeriod);
            cmd.Parameters.AddWithValue("@selectedUnit", selectedUnit);
            cmd.Parameters.AddWithValue("@selectedSloganList", _common.IsNull(_session.SloganIdList));
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

            ProductLevel productLevel = _common.GetProductLevel(true);

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
