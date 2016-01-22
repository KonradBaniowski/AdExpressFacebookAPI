#region Information
/*
 * Author : D Mussuma
 * Creation : 05/07/2010
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion

#region Using

using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpressI.LostWon.DAL;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using System.Data.SqlClient;
using TNS.AdExpressI.DynamicReport.DAL.Exceptions;
using TNS.AdExpressI.Common.DAL.Russia;
using System.Collections;
#endregion

namespace TNS.AdExpressI.LostWon.DAL.Russia
{
    public partial class LostWonResultDAL : LostWon.DAL.LostWonResultDAL
    {
        /// <summary>
        /// rp_ax_lost_won_columndetails_get
        /// </summary>
        private const string GetColumnDetailsProcedureName = "[dbo].[rp_ax_lost_won_columndetails_get]";

        /// <summary>
        /// rp_ax_lost_won_nbpublication_get
        /// </summary>
        private const string GetNbParutionDataProcedureName = "[dbo].[rp_ax_lost_won_nbpublication_get]";
        //private const string GetSynthesisDataProcedureName = "[dbo].[rp_ax_lost_won_data_get]";

        /// <summary>
        /// rp_ax_lost_won_data_get2
        /// </summary>
        private const string GetDataProcedureName = "[dbo].[rp_ax_lost_won_data_get]";

        CommonDAL common;

        public CommonDAL Common
        {
            get { return common; }
        }

        MediaRight MediaRight
        {
            get
            {
                return Common.GetMediaRight();
            }
        }

        ProductRight ProductRight
        {
            get { return Common.GetProductRight(); }
        }

        ProductClassification ProductClassification
        {
            get
            {
                return Common.GetProductClassification();
            }
        }
        AdTypeClassification AdTypes
        {
            get
            {
                return Common.GetAdTypeClassification();
            }
        }

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public LostWonResultDAL(WebSession session)
            : base(session)
        {
            common = new CommonDAL(session, session.CurrentModule);
        }

        #endregion

        #region GetColumnDetails
        /// <summary>    
        /// In the result page, client can choose the vehicle-level detail in column by selecting it from the drop-down menu.
        /// Then this method gets the list of identifiers of items corresponding to the vehicle-level selected.		
        /// </summary>
        /// <returns>Data set with list of vehicle-level items. </returns>		
        /// <remarks>The query must always contains the field of vehicle level ( "id_media" )</remarks>
        /// <exception cref="TNS.AdExpressI.DynamicReport.DAL.Exceptions.DynamicDALException">
        /// Exception throwed when an error occurs in the method</exception>
        public override DataSet GetColumnDetails()
        {
            try
            {
                DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
                string selectedLevel = columnDetailLevel.DataBaseIdField;

                Dictionary<CstCustomer.Right.type, string> selectedVehicles = _session.CustomerDataFilters.SelectedVehicles;
                string selectedVehicle = selectedVehicles[CstCustomer.Right.type.mediaAccess];

                using (SqlConnection conn = (SqlConnection)Common.GetDataSource().GetSource())
                {
                    Hashtable parameters = new Hashtable();

                    parameters["@id_language"] = Convert.ToInt16(_session.DataLanguage);
                    parameters["@id_module"] = _session.CurrentModule;

                    parameters["@id_media"] = Convert.ToInt16(_session.CustomerDataFilters.SelectedMediaType);
                    parameters["@selectedLevel"] = selectedLevel;
                    parameters["@selectedVehicle"] = selectedVehicle;

                    return GetColumnDetails(conn, parameters);
                }
            }
            catch (Exception err)
            {
                throw (new DynamicDALException("GetColumnDetails. Unable to load the results:", err));
            }
        }

        public static DataSet GetColumnDetails(SqlConnection conn, Hashtable parameters)
        {
            DataSet ds = new DataSet();


            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = GetColumnDetailsProcedureName;
            cmd.CommandTimeout = CommonDAL.ProcedureTimeout;

            conn.Open();

            CommonDAL.InitParams(parameters, cmd);

            // SP Execute
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(ds);
            }

            conn.Close();

            return ds;

        }

        #endregion

        #region GetNbParutionData
        /// <summary>
        /// Get for each vehicle and period selected the number of publications for the principal and comparative period.
        /// The columns returns will be as : [ID_MEDIA]	[NBPARUTION] [YEARPARUTION] 
        /// With "ID_MEDIA" : ID of vehicle.
        /// With "NBPARUTION" : vehicle number of publications
        /// With "YEARPARUTION" : identifier of principal period( value 1) or identifier of comparative period
        /// </summary>
        /// <returns> Number of publications data</returns>
        public override DataSet GetNbParutionData()
        {

            try
            {
                Dictionary<int, string> mediaUniverses = _session.CustomerDataFilters.CompetingMediaUniverses;
                string selectedVehicle = mediaUniverses[1];
                for (int i = 2; i <= mediaUniverses.Count; ++i)
                {
                    selectedVehicle = selectedVehicle + "," + mediaUniverses[i];
                }

                using (SqlConnection conn = (SqlConnection)Common.GetDataSource().GetSource())
                {
                    Hashtable parameters = new Hashtable();

                    parameters["@selected_vehicle"] = selectedVehicle;
                    parameters["@start1"] = _session.CustomerPeriodSelected.StartDate;
                    parameters["@stop1"] = _session.CustomerPeriodSelected.EndDate;
                    parameters["@start2"] = _session.CustomerPeriodSelected.ComparativeStartDate;
                    parameters["@stop2"] = _session.CustomerPeriodSelected.ComparativeEndDate;

                    return GetNbParutionData(conn, parameters);
                }


            }
            catch (Exception err)
            {
                throw (new DynamicDALException("GetNbParutionData. Unable to load the results:", err));
            }
        }

        public static DataSet GetNbParutionData(SqlConnection conn, Hashtable parameters)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = conn.CreateCommand();
            conn.Open();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = GetNbParutionDataProcedureName;
            cmd.CommandTimeout = CommonDAL.ProcedureTimeout;

            CommonDAL.InitParams(parameters, cmd);

            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(ds);
            }

            conn.Close();
            return ds;
        }
        #endregion

        #region GetSynthesisData
        /// <summary>
        /// Load Data for summary tab in Lots\ Won  Module.
        /// 
        /// The columns returns will be as : [id_level][id_report] [num] [stat1] [stat2]
        /// Where :
        ///id_level	: item calculated			
        ///	report type: 1-RECURRING, 2 - RECURRING (SLIDERS), 3 - RECURRING (CLIMBERS), 4-WON, 5-LOST			
        ///num :	number of unique items			
        ///stat1 :	summary of units for Reference period	
        ///stat2 :	summary of units for Comparative period	
        /// </summary>
        /// <returns>Data</returns>
        public override DataSet GetSynthesisData()
        {
            return GetData();
        }
        #endregion


        /// <summary>
        /// Gets something.
        /// </summary>
        /// <param name="id">The id.</param>
        public void GetSomething(int id)
        {

        }
        #region GetData

        /// <summary>
        /// Get Data to build Lost\Won report (except summary)
        /// </summary>
        /// <returns>Lost\Won report</returns>
        public override DataSet GetData()
        {
            try
            {
                Dictionary<int, string> mediaUniverses = _session.CustomerDataFilters.CompetingMediaUniverses;
                string selectedVehicle = mediaUniverses[1];
                for (int i = 2; i <= mediaUniverses.Count; ++i)
                {
                    selectedVehicle = selectedVehicle + "," + mediaUniverses[i];
                }

                //Get level detail in column
                DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
                //Get Vehicle Level detail level informations
                DetailLevelItemInformation mediaDetailLevelItemInformation = DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.media);

                MediaRight mediaRight = common.GetMediaRight();
                ProductRight productRight = common.GetProductRight();
                ProductClassification productClassification = common.GetProductClassification();
                AdTypeClassification adTypes = common.GetAdTypeClassification();

                string id_column_selected = ((DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0]).DataBaseIdField;

                DataSet ds = new DataSet();
                using (SqlConnection conn = (SqlConnection)Common.GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = GetDataProcedureName;

                    // SP set Parameters
                    cmd.Parameters.AddWithValue("@id_language", Convert.ToInt16(_session.DataLanguage));
                    cmd.Parameters.AddWithValue("@selected_vehicle", selectedVehicle);

                    cmd.Parameters.AddWithValue("@id_media", Convert.ToInt16(_session.CustomerDataFilters.SelectedMediaType));
                    cmd.Parameters.AddWithValue("@start1", _session.CustomerPeriodSelected.StartDate);
                    cmd.Parameters.AddWithValue("@stop1", _session.CustomerPeriodSelected.EndDate);
                    cmd.Parameters.AddWithValue("@start2", _session.CustomerPeriodSelected.ComparativeStartDate);
                    cmd.Parameters.AddWithValue("@stop2", _session.CustomerPeriodSelected.ComparativeEndDate);

                    cmd.Parameters.AddWithValue("@id_name_base", mediaDetailLevelItemInformation.DataBaseIdField);
                    cmd.Parameters.AddWithValue("@id_name_selected", columnDetailLevel.DataBaseIdField);
                    cmd.Parameters.AddWithValue("@id_unit", _session.GetSelectedUnit().Id.ToString());
                    cmd.Parameters.AddWithValue("@id_level1", (_session.GenericProductDetailLevel.LevelIds.Count > 0) ? _session.GenericProductDetailLevel.LevelIds[0].ToString() : string.Empty);
                    cmd.Parameters.AddWithValue("@id_level2", (_session.GenericProductDetailLevel.LevelIds.Count > 1) ? _session.GenericProductDetailLevel.LevelIds[1].ToString() : string.Empty);
                    cmd.Parameters.AddWithValue("@id_level3", (_session.GenericProductDetailLevel.LevelIds.Count > 2) ? _session.GenericProductDetailLevel.LevelIds[2].ToString() : string.Empty);
                    cmd.Parameters.AddWithValue("@report_type", Convert.ToInt16(_session.CurrentTab));

                    cmd.Parameters.AddWithValue("@customer_media_access", Common.IsNull(mediaRight.MediaRightMediaAccess));
                    cmd.Parameters.AddWithValue("@customer_media_except", Common.IsNull(mediaRight.MediaRightMediaExcept));
                    cmd.Parameters.AddWithValue("@customer_vehicle_access", Common.IsNull(mediaRight.MediaRightVehicleAccess));
                    cmd.Parameters.AddWithValue("@customer_vehicle_except", Common.IsNull(mediaRight.MediaRightVehicleExcept));

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

                    cmd.Parameters.AddWithValue("@cmpn_type", _session.CampaignType.ToString());



                    // SP Execute
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);

                        // If report_type is SYNTHESIS - nothing to do, return
                        if (Convert.ToInt16(_session.CurrentTab) == 6)
                            return ds;

                        if (ds.Tables.Count > 0)
                        {
                            ds.Tables[0].TableName = "total";
                            if (ds.Tables[0].Columns.Contains("unit")) ds.Tables[0].Columns["unit"].ColumnName = _session.GetSelectedUnit().Id.ToString();
                            // name of subcategory column for all table (if necessary)
                            if (id_column_selected != "id_media")
                                if (ds.Tables[0].Columns.Contains("id_category"))
                                    ds.Tables[0].Columns["id_category"].ColumnName = id_column_selected;
                        }

                        for (int i = 1; i < ds.Tables.Count; ++i)
                        {
                            ds.Tables[i].TableName = string.Format("level{0}", i);

                            if (ds.Tables[i].Columns.Contains("unit"))
                                ds.Tables[i].Columns["unit"].ColumnName = _session.GetSelectedUnit().Id.ToString();

                            // name of subcategory column for all table (if necessary)
                            if (id_column_selected != "id_media")
                                if (ds.Tables[i].Columns.Contains("id_category"))
                                    ds.Tables[i].Columns["id_category"].ColumnName = id_column_selected;

                            for (int j = 0; j < i && _session.GenericProductDetailLevel.Levels.Count >= i; ++j)
                            {
                                DetailLevelItemInformation info = (DetailLevelItemInformation)_session.GenericProductDetailLevel.Levels[j];
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
                throw (new DynamicDALException("GetData. Unable to load the results:", err));
            }
        }
        #endregion

    }
}