using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web.Navigation;
using System.Data.SqlClient;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;

namespace TNS.AdExpressI.Classification.DAL.SqlServerRussia {
	public class ClassificationDAL : TNS.AdExpressI.Classification.DAL.ClassificationDAL {
		#region Constructors
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>	
		public ClassificationDAL(WebSession session)
			: base(session) {
		}
		/// <summary>
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="dimension">Product or vehicle classification brand</param>
		public ClassificationDAL(WebSession session, TNS.Classification.Universe.Dimension dimension)
			: base(session, dimension) {
		}
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="genericDetailLevel">generic detail level selected by the user</param>
		/// <param name="vehicleList">List of media selected by the user</param>
		public ClassificationDAL(WebSession session, GenericDetailLevel genericDetailLevel, string vehicleList)
			: base(session, genericDetailLevel, vehicleList) {

		}
		#endregion

        /// <summary>
        /// This method provides SQL queries to get the media classification level's items.
        /// The data are filtered by customer's media rights and selected working set.		
        /// </summary>
        /// <returns>Data table 
        /// with media's identifiers ("idMediaType" column) and media's labels ("mediaType" column).
        /// </returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Impossible to execute query
        /// </exception>
        public override DataSet GetMediaType()
        {
            DataSet ds = null;
            string sql = "";
            Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, string> rights = _session.CustomerDataFilters.MediaRights;

            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_session.CurrentModule);

            // List of Media available for current module
            string ModuleMediaAccess = module.AllowedMediaUniverse.VehicleList;

            // Lists of the current customer's rights by different type of classification
            string CustomerMediaAccess = "";
            string CustomerMediaExcept = "";
            string CustomerCategoryAccess = "";
            string CustomerCategoryExcept = "";
            string CustomerVehicleAccess = "";
            string CustomerVehicleExcept = "";

            // Get the Media in acccess for the current customer
            if (rights.ContainsKey(CustomerRightConstante.type.vehicleAccess) && rights[CustomerRightConstante.type.vehicleAccess].Length > 0)
            {
                CustomerMediaAccess = rights[CustomerRightConstante.type.vehicleAccess];
            }
            // Get the Media in exception for the current customer
            if (rights.ContainsKey(CustomerRightConstante.type.vehicleException) && rights[CustomerRightConstante.type.vehicleException].Length > 0)
            {
                CustomerMediaExcept = rights[CustomerRightConstante.type.vehicleException];
            }

            // Get the Category in acccess for the current customer
            if (rights.ContainsKey(CustomerRightConstante.type.categoryAccess) && rights[CustomerRightConstante.type.categoryAccess].Length > 0)
            {
                CustomerCategoryAccess = rights[CustomerRightConstante.type.categoryAccess];
            }
            // Get the Category in exception for the current customer
            if (rights.ContainsKey(CustomerRightConstante.type.categoryException) && rights[CustomerRightConstante.type.categoryException].Length > 0)
            {
                CustomerCategoryExcept = rights[CustomerRightConstante.type.categoryException];
            }

            // Get the Vehicle in acccess for the current customer
            if (rights.ContainsKey(CustomerRightConstante.type.mediaAccess) && rights[CustomerRightConstante.type.mediaAccess].Length > 0)
            {
                CustomerVehicleAccess = rights[CustomerRightConstante.type.mediaAccess];
            }
            // Get the Vehicle in exception for the current customer
            if (rights.ContainsKey(CustomerRightConstante.type.mediaException) && rights[CustomerRightConstante.type.mediaException].Length > 0)
            {
                CustomerVehicleExcept = rights[CustomerRightConstante.type.mediaException];
            }

            sql = "exec [AX_CTL].[dbo].[cl_media_get]" +
                "  @id_language = " + _session.DataLanguage.ToString() +
                ", @module_media_access = '" + ModuleMediaAccess + "'" +
                ", @customer_media_access = '" + CustomerMediaAccess + "'" +
                ", @customer_media_except = '" + CustomerMediaExcept + "'" +
                ", @customer_category_access = '" + CustomerCategoryAccess + "'" +
                ", @customer_category_except = '" + CustomerCategoryExcept + "'" +
                ", @customer_vehicle_access = '" + CustomerVehicleAccess + "'" +
                ", @customer_vehicle_except = '" + CustomerVehicleExcept + "'";


            #region Execution of the query
            try
            {
                //Exectuing the SQL query
                //TNS.FrameWork.DB.Common.IDataSource source = new TNS.FrameWork.DB.Common.RussiaSqlServerDataSource(GetConnectionString()); //_session.Source.Fill(sql);
                //ds = source.Fill(sql);               
                ds = _session.Source.Fill(sql);

                /*The result must be a data table with firt field the identifier of the media ("idMediaType")
                 * and the second the label of the media ("mediaType").*/
                ds.Tables[0].Columns[0].ColumnName = "idMediaType";
                ds.Tables[0].Columns[1].ColumnName = "mediaType";

            }
            catch (System.Exception err)
            {
                throw (new Exceptions.DetailMediaDALException("Impossible to execute query", err));
            }
            #endregion

            return ds;


        }

        public override DataSet GetDetailMedia(string keyWord)
        {
            //Calling the engine which compute data
            DetailMediaDAL engineDal = new DetailMediaDAL(_session, _genericDetailLevel, _vehicleList);
            return engineDal.GetData(keyWord);
        }
        public override DataSet GetDetailMedia()
        {
            DataSet ds = new DataSet();
            StringBuilder sql = new StringBuilder();
            //Get data filters object which contains query's filters methods
            TNS.AdExpress.Web.Core.CustomerDataFilters dataFilters = new TNS.AdExpress.Web.Core.CustomerDataFilters(_session);

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "cl_vehicle_get";

                    cmd.Parameters.Add("@id_SubMedia", SqlDbType.SmallInt);
                    cmd.Parameters["@id_SubMedia"].Value = Int16.Parse(_session.CustomerDataFilters.SelectedMediaCategory);

                    conn.Open();

                    SqlDataReader rs = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    using (rs)
                    {
                        dt.Load(rs);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        // add datatables to dataset 
                        dt.Columns[0].ColumnName = "id_category";
                        dt.Columns[1].ColumnName = "category";
                        dt.Columns[2].ColumnName = "id_media";
                        dt.Columns[3].ColumnName = "media";
                        ds.Tables.Add(dt);

                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                string sTemp = ex.Message;
            }



            return ds;
        }

        /// <summary>
        /// Get the list of sub media corresponding to media type selected
        /// </summary>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Exception throwed when an error occurs in the building or execution of the SQL query.</exception>
        /// <returns>Dataset with  a data table of 2 columns : "Id_SubMedia", "SubMedia".
        /// The column "Id_SubMedia" corresponds to the identifier of the level sub media.
        /// The column "SubMedia" corresponds to the label of the level sub media.       
        /// </returns>
        public override DataSet GetSubMediaData()
        {
            DataSet ds;
            string sql = "";
            Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, string> rights = _session.CustomerDataFilters.MediaRights;

            // Lists of the current customer's rights by different type of classification
            string CustomerCategoryAccess = "";
            string CustomerCategoryExcept = "";
            string CustomerVehicleAccess = "";
            string CustomerVehicleExcept = "";

            // Get the Category in acccess for the current customer
            if (rights.ContainsKey(CustomerRightConstante.type.categoryAccess) && rights[CustomerRightConstante.type.categoryAccess].Length > 0)
            {
                CustomerCategoryAccess = rights[CustomerRightConstante.type.categoryAccess];
            }
            // Get the Category in exception for the current customer
            if (rights.ContainsKey(CustomerRightConstante.type.categoryException) && rights[CustomerRightConstante.type.categoryException].Length > 0)
            {
                CustomerCategoryExcept = rights[CustomerRightConstante.type.categoryException];
            }

            // Get the Vehicle in acccess for the current customer
            if (rights.ContainsKey(CustomerRightConstante.type.mediaAccess) && rights[CustomerRightConstante.type.mediaAccess].Length > 0)
            {
                CustomerVehicleAccess = rights[CustomerRightConstante.type.mediaAccess];
            }
            // Get the Vehicle in exception for the current customer
            if (rights.ContainsKey(CustomerRightConstante.type.mediaException) && rights[CustomerRightConstante.type.mediaException].Length > 0)
            {
                CustomerVehicleExcept = rights[CustomerRightConstante.type.mediaException];
            }

            sql = "exec [AX_CTL].[dbo].[cl_submedia_get]" +
                "  @id_language = " + _session.DataLanguage.ToString() +
                " , @id_Media = " + _session.CustomerDataFilters.SelectedMediaType +
                ", @customer_category_access = '" + CustomerCategoryAccess + "'" +
                ", @customer_category_except = '" + CustomerCategoryExcept + "'" +
                ", @customer_vehicle_access = '" + CustomerVehicleAccess + "'" +
                ", @customer_vehicle_except = '" + CustomerVehicleExcept + "'";

            #region Execution of the query
            try
            {
                //Exectuing the SQL query
                ds = _session.Source.Fill(sql);

                /*The result must be a data table with firt field the identifier of the media ("idMediaType")
                 * and the second the label of the media ("mediaType").*/
                ds.Tables[0].Columns[0].ColumnName = "id_SubMedia";
                ds.Tables[0].Columns[1].ColumnName = "SubMedia";

            }
            catch (System.Exception err)
            {
                throw (new Exceptions.DetailMediaDALException("Impossible to execute query", err));
            }
            #endregion

            return ds;
        }

        
        private string GetConnectionString()
        {
            #region Building the connection string

            string Server = "194.58.36.199";
            string Username = "TNSRussie";
            string Password = "adextest1";
            string Database = "AX_CTL";

            string ConnectionString = "Data Source=" + Server + ";";
            ConnectionString += "User ID=" + Username + ";";
            ConnectionString += "Password=" + Password + ";";
            ConnectionString += "Initial Catalog=" + Database;

            #endregion

            return ConnectionString;
        }
	}
}
