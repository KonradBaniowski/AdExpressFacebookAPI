#region Information
/*
 * Author : G Ragneau
 * Created on 13/08/2008
 * Modifications :
 *      Ahtour - Date - Description
 * 
 * 
 * */
#endregion

#region Using
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpressI.PresentAbsentDAL.Exceptions;
using TNS.FrameWork.DB;
using CstDBDesc = TNS.AdExpress.Domain.DataBaseDescription;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctWeb = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Constantes.DB;
using System.Data.SqlClient;

using CustomerRightType = TNS.AdExpress.Constantes.Customer.Right.type;
using UniverseAccessType = TNS.Classification.Universe.AccessType;
using TNS.AdExpressI.Common.DAL.Russia;
#endregion


namespace TNS.AdExpressI.PresentAbsent.DAL.Russia
{
    /// <summary>
    /// Extract data for different type of results of the module Present / Absent Report.
    /// It contains the following public methods :
    /// The method <code>DataTable GetData(); </code> get data of the following types of results :
    /// - Result "Portofolio" : get data corresponding to active products items in the set being searched.
    /// - Result "Present in more than one vehicle" : get data corresponding to products items present 
    /// in more tha one vehicle
    /// - Result "Absent" :  get data corresponding to products items absent from the reference vehicles 
    /// in comparaison with the competing vehicles
    /// - Result "Exclusive to one vehicle" : get data corresponding to the reference vehicles' exclusive 
    /// products items in comparaison with the competing vehicles.
    /// - Result "strenghs" : get data corresponding to the product items whose market share is greater 
    /// than that of the total of the product set, int the reference vehicle set.
    /// - Result "Prospects" : get data corresponding to the product items whose market share is less 
    /// than that of the total of the product set, int the reference vehicle set.
    /// 
    /// All previous results call  method <code>DataSet GetData()</code>.
    /// 
    /// - Result "Summary" : get data corresponding (calls method <code>GetSynthesisData()</code>)to the summary of Present, Absent and 
    ///Exclusive items on the main hierarchical levels of the product brand classification.         
    /// 
    /// The method <code>DataSet GetSynthesisData();</code>
    /// loads data for result tab "Summary" : get data corresponding to the summary of Present, Absent and 
    ///Exclusive items on the main hierarchical levels of the product brand classification. 
    ///  
    /// In the result page, client can choose the vehicle-level detail in column by selecting it from the drop-down menu.
    /// The method  <code>DataSet GetColumnDetails();</code>
    /// gets the list of items corresponding to the vehicle-level selected.	
    ///	The method <code>DataSet GetNbParutionData();</code>
    /// Get for each vehicles and period selected  the number of publications
    /// </summary>
    public class PresentAbsentDAL : PresentAbsent.DAL.PresentAbsentDAL
    {
        //public struct AdTypeClassification
        //{
        //    public string AdTypeAccess1;
        //    public string AdTypeAccess2;
        //    public string AdTypeExcept;
        //}

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        public PresentAbsentDAL(WebSession session)
            : base(session)
        {
            common = new CommonDAL(session, session.CurrentModule);
        }
        #endregion

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

        #region GetData
        /// <summary>
        ///Load data for the module absent/present report.
        ///The module absent/present report allows client to view all active product items
        /// from a selection of competing media vehicles (media type, media category, media vehicle).
        /// This method returns the data for the folloing results :
        /// - Result "Portofolio" : get data corresponding to active products items in the set being searched.
        /// - Result "Present in more than one vehicle" : get data corresponding to products items present 
        /// in more tha one vehicle
        /// - Result "Absent" :  get data corresponding to products items absent from the reference vehicles 
        /// in comparaison with the competing vehicles
        /// - Result "Exclusive to one vehicle" : get data corresponding to the reference vehicles' exclusive 
        /// products items in comparaison with the competing vehicles.
        /// - Result "strenghs" : get data corresponding to the product items whose market share is greater 
        /// than that of the total of the product set, int the reference vehicle set.
        /// - Result "Prospects" : get data corresponding to the product items whose market share is less 
        /// than that of the total of the product set, int the reference vehicle set.
        /// 
        /// - Calls the following methods :
        ///<code> CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
        ///   DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
        ///   GetUniversFilter(CstDB.TableType.Type type, string dateField, CustomerPeriod customerPeriod);       
        ///  </code>
        /// </summary> 
        /// <example>
        /// GetData method is called in the Bussiness Layer like in the following example. 
        /// The DAL layer is called by reflection with method's parameters. And the the method is called to get the data table of the module.
        /// <code> Dataset dt = null;
        ///     if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the present absent result"));
        ///    object[] parameters = new object[1];
        ///   parameters[0] = _session;
        ///   IPresentAbsentResultDAL presentAbsentDAL = (IPresentAbsentResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
        ///   dt = presentAbsentDAL.GetData();
        ///   </code>
        /// </example>
        /// <remarks> The Data set obtained contains, a data table for the line TOTAL of the result table, 
        /// and N data table corresponding to the N classification level selected. Data Table will be structured as follows :
        /// DATATABLE [ID_MEDIA,"identifier of current column level" ,ID LEVEL 1, LABEL LEVEL 1,...,ID LEVEL N, LABEL LEVEL N,ID_ADRESS,UNIT FIELD]
        /// ID_MEDIA : contains identifiers of vehicles selected.
        /// "identifier of current column level"  : contains identifiers of vehicle-level detail in column.
        /// ID LEVEL N : contains identifiers of classification-level detail in row into the final result table.
        /// LABEL LEVEL N : contains labels of classification-level detail in row into the final result table.
        /// ID_ADRESS : Identifier for compagny description.
        /// UNIT : example EURO (according to unit selected by the client.
        /// </remarks>
        /// <returns>Present/Absent report data set</returns>
        /// <exception cref="TNS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
        /// Exception throwed when an error occurs in the method</exception>
        public override DataSet GetData()
        {

            #region Variables
            DataSet dsResult = new DataSet();
            #endregion

            #region Report detail
            //Get level detail in column
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
            //Get Vehicle Level detail level informations
            DetailLevelItemInformation mediaDetailLevelItemInformation = DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.media);

            // Id of field name of last level (Vehicle)
            string id_name_base = mediaDetailLevelItemInformation.DataBaseIdField;

            // Id of field name of selected level
            string id_name_selected = columnDetailLevel.DataBaseIdField;
            #endregion

            #region Customer Selections

            // Period selected
            string DateStart = _session.CustomerDataFilters.BeginningDate;
            string DateStop = _session.CustomerDataFilters.EndDate;

            // Get unit selected field
            string id_unit = _session.CustomerDataFilters.SelectedUnit.Id.ToString();

            // Media selected
            string id_media = _session.CustomerDataFilters.SelectedMediaType;

            // Vehicles selected and devided by groups (first group - reference, other - competing
            /*Dictionary<CstCustomer.Right.type, string> selectedVehicles = _session.CustomerDataFilters.SelectedVehicles;
            string VehicleList = selectedVehicles[CstCustomer.Right.type.mediaAccess];*/
            Dictionary<int, string> res = _session.CustomerDataFilters.CompetingMediaUniverses;
            // References
            string VehicleList = res[1];
            // Competings
            for (int i = 2; i <= res.Count; i++)
            {
                VehicleList = VehicleList + "-" + res[i];
            }


            #endregion

            #region RowLevels
            int row_levels_cnt = _session.GenericProductDetailLevel.GetNbLevels;
            // Expects no more then 3
            if (row_levels_cnt > 3)
            {
                throw (new PresentAbsentDALException("More then 3 row levels selected"));
            }
            if (row_levels_cnt == 0)
            {
                throw (new PresentAbsentDALException("Number of row levels selected = 0"));
            }
            string[] id_levels = new string[3] { "", "", "" };
            DetailLevelItemInformation row_level;
            for (int i = 0; i < row_levels_cnt; i++)
            {
                row_level = (DetailLevelItemInformation)_session.GenericProductDetailLevel.Levels[i];
                id_levels[i] = row_level.DataBaseField;
            }
            #endregion

            AdTypeClassification adTypes = common.GetAdTypeClassification();


            #region Execution of the query
            try
            {
                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    // SP Construction    
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = "[dbo].[rp_ax_present_absent_data_get]";

                    // SP set Parameters
                    cmd.Parameters.AddWithValue("@id_language", Convert.ToInt16(_session.DataLanguage));
                    cmd.Parameters.AddWithValue("@selected_vehicle", VehicleList);
                    cmd.Parameters.AddWithValue("@id_Media", Convert.ToInt16(id_media));
                    cmd.Parameters.AddWithValue("@start", DateStart);
                    cmd.Parameters.AddWithValue("@stop", DateStop);
                    cmd.Parameters.AddWithValue("@cmpn_type", _session.CustomerDataFilters.CampaignType.ToString());
                    cmd.Parameters.AddWithValue("@id_name_base", id_name_base);
                    cmd.Parameters.AddWithValue("@id_name_selected", id_name_selected);
                    cmd.Parameters.AddWithValue("@id_unit", id_unit);
                    cmd.Parameters.AddWithValue("@id_level1", id_levels[0].ToString());
                    cmd.Parameters.AddWithValue("@id_level2", id_levels[1].ToString());
                    cmd.Parameters.AddWithValue("@id_level3", id_levels[2].ToString());
                    cmd.Parameters.AddWithValue("@report_type", Convert.ToInt16(_session.CurrentTab));
                    cmd.Parameters.AddWithValue("@customer_media_access", MediaRight.MediaRightMediaAccess);
                    cmd.Parameters.AddWithValue("@customer_media_except", MediaRight.MediaRightMediaExcept);
                    cmd.Parameters.AddWithValue("@customer_vehicle_access", MediaRight.MediaRightVehicleAccess);
                    cmd.Parameters.AddWithValue("@customer_vehicle_except", MediaRight.MediaRightVehicleExcept);
                    cmd.Parameters.AddWithValue("@customer_region_access", Common.IsNull(MediaRight.MediaRightRegionAccess));
                    cmd.Parameters.AddWithValue("@customer_region_except", Common.IsNull(MediaRight.MediaRightRegionExcept));

                    cmd.Parameters.AddWithValue("@customer_sector_access", ProductRight.ProductRightCategory1Access);
                    cmd.Parameters.AddWithValue("@customer_sector_except", ProductRight.ProductRightCategory1Except);
                    cmd.Parameters.AddWithValue("@customer_subsector_access", ProductRight.ProductRightCategory2Access);
                    cmd.Parameters.AddWithValue("@customer_subsector_except", ProductRight.ProductRightCategory2Except);
                    cmd.Parameters.AddWithValue("@customer_group_access", ProductRight.ProductRightCategory3Access);
                    cmd.Parameters.AddWithValue("@customer_group_except", ProductRight.ProductRightCategory3Except);
                    cmd.Parameters.AddWithValue("@customer_subgroup_access", ProductRight.ProductRightCategory4Access);
                    cmd.Parameters.AddWithValue("@customer_subgroup_except", ProductRight.ProductRightCategory4Except);
                    cmd.Parameters.AddWithValue("@customer_advertiser_access", Common.IsNull(ProductRight.ProductRightAdvertiserAccess));
                    cmd.Parameters.AddWithValue("@customer_advertiser_except", Common.IsNull(ProductRight.ProductRightAdvertiserExcept));
                    cmd.Parameters.AddWithValue("@customer_brand_access", Common.IsNull(ProductRight.ProductRightBrandAccess));
                    cmd.Parameters.AddWithValue("@customer_brand_except", Common.IsNull(ProductRight.ProductRightBrandExcept));
                    cmd.Parameters.AddWithValue("@selected_sector_access1", ProductClassification.Category1Access1);
                    cmd.Parameters.AddWithValue("@selected_sector_access2", ProductClassification.Category1Access2);
                    cmd.Parameters.AddWithValue("@selected_sector_except", ProductClassification.Category1Except);
                    cmd.Parameters.AddWithValue("@selected_subsector_access1", ProductClassification.Category2Access1);
                    cmd.Parameters.AddWithValue("@selected_subsector_access2", ProductClassification.Category2Access2);
                    cmd.Parameters.AddWithValue("@selected_subsector_except", ProductClassification.Category2Except);
                    cmd.Parameters.AddWithValue("@selected_group_access1", ProductClassification.Category3Access1);
                    cmd.Parameters.AddWithValue("@selected_group_access2", ProductClassification.Category3Access2);
                    cmd.Parameters.AddWithValue("@selected_group_except", ProductClassification.Category3Except);
                    cmd.Parameters.AddWithValue("@selected_subgroup_access1", ProductClassification.Category4Access1);
                    cmd.Parameters.AddWithValue("@selected_subgroup_access2", ProductClassification.Category4Access2);
                    cmd.Parameters.AddWithValue("@selected_subgroup_except", ProductClassification.Category4Except);
                    cmd.Parameters.AddWithValue("@adtypeClassAdTypeAccess1", adTypes.AdTypeAccess1);
                    cmd.Parameters.AddWithValue("@adtypeClassAdTypeAccess2", adTypes.AdTypeAccess2);
                    cmd.Parameters.AddWithValue("@adtypeClassAdTypeExcept", adTypes.AdTypeExcept);

                    // SP Execute
                    SqlDataReader rs = cmd.ExecuteReader();

                    // Number of tables depends of number of row levels
                    if (row_levels_cnt == 1)
                        dsResult.Load(rs, LoadOption.OverwriteChanges, "total", "level1");
                    else if (row_levels_cnt == 2)
                        dsResult.Load(rs, LoadOption.OverwriteChanges, "total", "level1", "level2");
                    else if (row_levels_cnt == 3)
                        dsResult.Load(rs, LoadOption.OverwriteChanges, "total", "level1", "level2", "level3");

                    // Columns name depends of id_level and unit name
                    for (int i = 0; i < dsResult.Tables.Count; i++)
                    {
                        // name of statictics column for all table
                        DataTable dt = dsResult.Tables[i];
                        dt.Columns[dt.Columns.Count - 1].ColumnName = id_unit;
                        // name of subcategory column for all table (if necessary)
                        if (id_name_selected != "id_media")
                            dt.Columns[1].ColumnName = id_name_selected;
                        if (i == 0)
                            continue;

                        // Rename column or row level tables according its id_level
                        if (i == 1)
                        {
                            // Id name level1   
                            dt.Columns[dt.Columns.Count - 3].ColumnName = String.Format("id_{0}", id_levels[i - 1]);
                            // Label name level1   
                            dt.Columns[dt.Columns.Count - 2].ColumnName = id_levels[i - 1];
                        }
                        // Rename column or row level tables according its id_level
                        else if (i == 2)
                        {
                            // Id name level1   
                            dt.Columns[dt.Columns.Count - 5].ColumnName = String.Format("id_{0}", id_levels[i - 2]);
                            // Label name level1
                            dt.Columns[dt.Columns.Count - 4].ColumnName = id_levels[i - 2];
                            // Id name level2   
                            dt.Columns[dt.Columns.Count - 3].ColumnName = String.Format("id_{0}", id_levels[i - 1]);
                            // Label name level2
                            dt.Columns[dt.Columns.Count - 2].ColumnName = id_levels[i - 1];
                        }
                        // Rename column or row level tables according its id_level
                        else if (i == 3)
                        {
                            // Id name level1   
                            dt.Columns[dt.Columns.Count - 7].ColumnName = String.Format("id_{0}", id_levels[i - 3]);
                            // Label name level1
                            dt.Columns[dt.Columns.Count - 6].ColumnName = id_levels[i - 3];
                            // Id name level2   
                            dt.Columns[dt.Columns.Count - 5].ColumnName = String.Format("id_{0}", id_levels[i - 2]);
                            // Label name level2
                            dt.Columns[dt.Columns.Count - 4].ColumnName = id_levels[i - 2];
                            // Id name level3   
                            dt.Columns[dt.Columns.Count - 3].ColumnName = String.Format("id_{0}", id_levels[i - 1]);
                            // Label name level3
                            dt.Columns[dt.Columns.Count - 2].ColumnName = id_levels[i - 1];
                        }
                    }

                    conn.Close();
                }
            }
            catch (System.Exception err)
            {
                throw (new PresentAbsentDALException("Unable to load the results for row details:", err));
            }
            #endregion

            return dsResult;

        }
        #endregion

        #region GetColumnDetails
        /// <summary>    
        /// In the result page, client can choose the vehicle-level detail in column by selecting it from the drop-down menu.
        /// Then this method gets the list of identifiers of items corresponding to the vehicle-level selected.		
        /// </summary>
        /// <returns>Data set with list of vehicle-level items. </returns>		
        /// <remarks>The query must always contains the field of vehicle level ( "id_media" )</remarks>
        /// <exception cref="TNS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
        /// Exception throwed when an error occurs in the method</exception>
        public override DataSet GetColumnDetails()
        {

            #region Variables

            // Result DataSet
            DataSet ds = new DataSet();

            //Get Vehicle Level detail level informations
            DetailLevelItemInformation mediaDetailLevelItemInformation = DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.media);

            //Get vehicle-level detail in column selected by the client
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];

            // Id's field name of last level (Vehicle)
            string id_name_base = mediaDetailLevelItemInformation.DataBaseIdField;

            // Id's field name of selected level
            string id_name_selected = columnDetailLevel.DataBaseIdField;

            // List of selected vehicles
            Dictionary<CstCustomer.Right.type, string> selectedVehicles = _session.CustomerDataFilters.SelectedVehicles;
            string VehicleList = selectedVehicles[CstCustomer.Right.type.mediaAccess];

            // Selected Media
            string selectedMedia = _session.CustomerDataFilters.SelectedMediaType;

            #endregion

            #region Execution of the query
            try
            {
                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    // SP Construction    
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[rp_column_detail_get]";
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@id_name_base", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@id_name_selected", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@selected_vehicle", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@id_media", SqlDbType.SmallInt);

                    // SP set Parameters
                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_session.DataLanguage);
                    cmd.Parameters["@id_name_base"].Value = id_name_base;
                    cmd.Parameters["@id_name_selected"].Value = id_name_selected;
                    cmd.Parameters["@selected_vehicle"].Value = VehicleList;
                    cmd.Parameters["@id_media"].Value = Convert.ToInt16(selectedMedia);

                    // SP Execute
                    SqlDataReader rs = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(rs);

                    if (dt.Rows.Count > 0)
                    {
                        ds.Tables.Add(dt);
                    }

                    conn.Close();
                }
            }
            catch (System.Exception err)
            {
                throw (new PresentAbsentDALException("Unable to load the list of supports for columns details:", err));
            }
            #endregion

            return ds;

        }
        #endregion

        #region Get Universes Filter
        /// <summary>
        /// Get differents universes filters for the query. 
        /// Filters can be users rigths, classification items selected or period selected.
        /// </summary>
        /// <param name="type">Data table type</param>
        /// <param name="dateField">date field</param>
        /// <param name="customerPeriod">customer selected period</param>
        /// <returns>SQl string for universes filter</returns>
        protected override string GetUniversFilter(CstDB.TableType.Type type, string dateField, CustomerPeriod customerPeriod)
        {
            StringBuilder sql = new StringBuilder();
            return sql.ToString();
        }
        #endregion

        #region GetSynthesisData
        /// <summary>
        ///Loads data for result tab "Summary" : get data corresponding to the summary of Present, Absent and 
        ///Exclusive items on the main hierarchical levels of the product brand classification.         
        ///The data table returned contains data in the following order :
        /// <code>select id_sector,id_subsector, d_group_
        /// , id_advertiser,id_brand,id_product,ID_GROUP_ADVERTISING_AGENCY,ID_ADVERTISING_AGENCY
        /// ,id_media, date_num, euro
        /// </code>
        /// So you ca notice that the firts columns correspond to identifiers of the classification product ( from id_sector to id_product).
        /// Then the data columns of advertising agency ("ID_GROUP_ADVERTISING_AGENCY,ID_ADVERTISING_AGENCY").
        /// and column of the identifier of vehicle ("id_media"). 
        /// The column fo current dates of the selection. The "date_num" is an alias.
        /// The field euro is an example of unit's field which depends on the unit selected. 
        /// The unit label changes according to the unit selected by the client (ex. duration, spot).
        ///</summary>
        ///<remarks> Use the protected method <code>GetSynthesisRequest(CstDB.TableType.Type type);</code> which get summary data
        ///accordind to the type of period (monthly, weekly, dayly).</remarks>
        /// <returns>Summary result's data set</returns>
        /// <exception cref="TNS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
        /// Exception throwed when an error occurs in the method</exception>
        public override DataSet GetSynthesisData()
        {

            //Symmary levels to use in the query
            //List of TNS.AdExpress.Domain.Level.DetailLevelItemInformation objects
            //For each DetailLevelItemInformation object use "DataBaseIdField"

            GenericDetailLevel levels = GetSummaryLevels();

            #region Variables
            DataSet dsResult = new DataSet();
            #endregion

            #region Customer Selections

            // Period selected
            string DateStart = _session.CustomerDataFilters.BeginningDate;
            string DateStop = _session.CustomerDataFilters.EndDate;

            // Get unit selected field
            string id_unit = _session.CustomerDataFilters.SelectedUnit.Id.ToString();

            // Media selected
            string id_media = _session.CustomerDataFilters.SelectedMediaType;

            // Vehicles selected and devided by groups (first group - reference, other - competing
            /*Dictionary<CstCustomer.Right.type, string> selectedVehicles = _session.CustomerDataFilters.SelectedVehicles;
            string VehicleList = selectedVehicles[CstCustomer.Right.type.mediaAccess];*/
            Dictionary<int, string> res = _session.CustomerDataFilters.CompetingMediaUniverses;
            // References
            string VehicleList = res[1];
            // Competings
            for (int i = 2; i <= res.Count; i++)
            {
                VehicleList = VehicleList + "-" + res[i];
            }

            #endregion

            AdTypeClassification adTypes = common.GetAdTypeClassification();

            #region Execution of the query
            try
            {
                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    // SP Construction    
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[rp_ax_present_absent_data_get]";


                    // SP set Parameters
                    cmd.Parameters.AddWithValue("@id_language", Convert.ToInt16(_session.DataLanguage));
                    cmd.Parameters.AddWithValue("@selected_vehicle", VehicleList);
                    cmd.Parameters.AddWithValue("@id_Media", Convert.ToInt16(id_media));
                    cmd.Parameters.AddWithValue("@start", DateStart);
                    cmd.Parameters.AddWithValue("@stop", DateStop);
                    cmd.Parameters.AddWithValue("@cmpn_type", _session.CustomerDataFilters.CampaignType.ToString());
                    cmd.Parameters.AddWithValue("@id_name_base", "");
                    cmd.Parameters.AddWithValue("@id_name_selected", "");
                    cmd.Parameters.AddWithValue("@id_unit", id_unit);
                    cmd.Parameters.AddWithValue("@id_level1", "");
                    cmd.Parameters.AddWithValue("@id_level2", "");
                    cmd.Parameters.AddWithValue("@id_level3", "");
                    cmd.Parameters.AddWithValue("@report_type", 6);
                    cmd.Parameters.AddWithValue("@customer_media_access", MediaRight.MediaRightMediaAccess);
                    cmd.Parameters.AddWithValue("@customer_media_except", MediaRight.MediaRightMediaExcept);
                    cmd.Parameters.AddWithValue("@customer_vehicle_access", MediaRight.MediaRightVehicleAccess);
                    cmd.Parameters.AddWithValue("@customer_vehicle_except", MediaRight.MediaRightVehicleExcept);
                    cmd.Parameters.AddWithValue("@customer_region_access", Common.IsNull(MediaRight.MediaRightRegionAccess));
                    cmd.Parameters.AddWithValue("@customer_region_except", Common.IsNull(MediaRight.MediaRightRegionExcept));

                    cmd.Parameters.AddWithValue("@customer_sector_access", ProductRight.ProductRightCategory1Access);
                    cmd.Parameters.AddWithValue("@customer_sector_except", ProductRight.ProductRightCategory1Except);
                    cmd.Parameters.AddWithValue("@customer_subsector_access", ProductRight.ProductRightCategory2Access);
                    cmd.Parameters.AddWithValue("@customer_subsector_except", ProductRight.ProductRightCategory2Except);
                    cmd.Parameters.AddWithValue("@customer_group_access", ProductRight.ProductRightCategory3Access);
                    cmd.Parameters.AddWithValue("@customer_group_except", ProductRight.ProductRightCategory3Except);
                    cmd.Parameters.AddWithValue("@customer_subgroup_access", ProductRight.ProductRightCategory4Access);
                    cmd.Parameters.AddWithValue("@customer_subgroup_except", ProductRight.ProductRightCategory4Except);
                    cmd.Parameters.AddWithValue("@customer_advertiser_access", Common.IsNull(ProductRight.ProductRightAdvertiserAccess));
                    cmd.Parameters.AddWithValue("@customer_advertiser_except", Common.IsNull(ProductRight.ProductRightAdvertiserExcept));
                    cmd.Parameters.AddWithValue("@customer_brand_access", Common.IsNull(ProductRight.ProductRightBrandAccess));
                    cmd.Parameters.AddWithValue("@customer_brand_except", Common.IsNull(ProductRight.ProductRightBrandExcept));
                    cmd.Parameters.AddWithValue("@selected_sector_access1", ProductClassification.Category1Access1);
                    cmd.Parameters.AddWithValue("@selected_sector_access2", ProductClassification.Category1Access2);
                    cmd.Parameters.AddWithValue("@selected_sector_except", ProductClassification.Category1Except);
                    cmd.Parameters.AddWithValue("@selected_subsector_access1", ProductClassification.Category2Access1);
                    cmd.Parameters.AddWithValue("@selected_subsector_access2", ProductClassification.Category2Access2);
                    cmd.Parameters.AddWithValue("@selected_subsector_except", ProductClassification.Category2Except);
                    cmd.Parameters.AddWithValue("@selected_group_access1", ProductClassification.Category3Access1);
                    cmd.Parameters.AddWithValue("@selected_group_access2", ProductClassification.Category3Access2);
                    cmd.Parameters.AddWithValue("@selected_group_except", ProductClassification.Category3Except);
                    cmd.Parameters.AddWithValue("@selected_subgroup_access1", ProductClassification.Category4Access1);
                    cmd.Parameters.AddWithValue("@selected_subgroup_access2", ProductClassification.Category4Access2);
                    cmd.Parameters.AddWithValue("@selected_subgroup_except", ProductClassification.Category4Except);
                    cmd.Parameters.AddWithValue("@adtypeClassAdTypeAccess1", adTypes.AdTypeAccess1);
                    cmd.Parameters.AddWithValue("@adtypeClassAdTypeAccess2", adTypes.AdTypeAccess2);
                    cmd.Parameters.AddWithValue("@adtypeClassAdTypeExcept", adTypes.AdTypeExcept);

                    // SP Execute
                    SqlDataReader rs = cmd.ExecuteReader();
                    dsResult.Load(rs, LoadOption.OverwriteChanges, "Synthesis", "level");
                    conn.Close();
                }
            }
            catch (System.Exception err)
            {
                throw (new PresentAbsentDALException("Unable to load the results for Total report:", err));
            }
            #endregion

            return dsResult;

        }
        #endregion

        #region Get Number of Publication by vehicle
        /// <summary>
        /// Get for each vehicles and period selected  the number of publications		
        /// </summary>
        /// <remarks> Only for medium PRESS</remarks>
        /// <returns>Data table with in first colum the identifier of vehicle (id_media)
        /// and in the second column the number of publications (NbParution)</returns>
        /// <exception cref="TNS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
        /// Exception throwed when an error occurs in the method</exception>
        public override DataSet GetNbParutionData()
        {

            #region Variables
            DataSet dsResult = new DataSet();
            #endregion


            // Period selected
            string DateStart = _session.CustomerDataFilters.BeginningDate;
            string DateStop = _session.CustomerDataFilters.EndDate;

            // All Vehicles selected
            /*Dictionary<CstCustomer.Right.type, string> selectedVehicles = _session.CustomerDataFilters.SelectedVehicles;
            string VehicleList = selectedVehicles[CstCustomer.Right.type.mediaAccess];*/
            Dictionary<int, string> res = _session.CustomerDataFilters.CompetingMediaUniverses;
            // References
            string VehicleList = res[1];
            // Competings
            for (int i = 2; i <= res.Count; i++)
            {
                VehicleList = VehicleList + "," + res[i];
            }

            #region Execution of the query
            try
            {
                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    // SP Construction    
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[rp_ax_present_absent_nbpublication_get]";
                    cmd.Parameters.Add("@selected_vehicle", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@start", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@stop", SqlDbType.NVarChar);

                    // SP set Parameters
                    cmd.Parameters["@selected_vehicle"].Value = VehicleList;
                    cmd.Parameters["@start"].Value = DateStart;
                    cmd.Parameters["@stop"].Value = DateStop;

                    // SP Execute
                    SqlDataReader rs = cmd.ExecuteReader();
                    dsResult.Load(rs, LoadOption.OverwriteChanges, "Synthesis", "level");
                    conn.Close();
                }
            }
            catch (System.Exception err)
            {
                throw (new PresentAbsentDALException("Unable to load the results for NBpublication:", err));
            }
            #endregion

            return dsResult;

        }
        #endregion

        public object IsNull(string list)
        {
            if (string.IsNullOrEmpty(list) || list.Equals("-1") || list.Equals(long.MinValue.ToString()))
            {
                return Convert.DBNull;
            }

            return list;
        }

        public string GetCustomerRightTypeValue(Dictionary<CustomerRightType, string> rights, CustomerRightType type)
        {
            if (rights.ContainsKey(type) && rights[type].Length > 0)
            {
                return rights[type];
            }

            return null;
        }
    }
}
