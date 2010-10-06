#region Information
/*
 * Author : Y Rkaina && D. Mussuma
 * Created on : 15/07/2009
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;

using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web.Navigation;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.DataBaseDescription;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using VehicleClassificationCst = TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;

namespace TNS.AdExpressI.Classification.DAL {
	/// <summary>
	/// This class provides SQL queries to get the media classification level's items.
	/// The data are filtered by customer's media rights and selected universe.
	/// 
	/// In this class the method <code>DataTable GetData();</code> returns a data set 
	/// with media's identifiers ("idVehicle" column) and media's labels ("vehicle" column).
	/// </summary>
	public class VehiclesDAL : EngineDAL{		

		#region Constructors
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>
		public VehiclesDAL(WebSession session):base(session) {			
		}		
		#endregion

		#region Publics methods

		#region GetData()
        /// <summary>
        /// This method provides SQL queries to get the media typeclassification level's items.
        /// The data are filtered by customer's media rights and selected working set.		
        /// </summary>
        /// <returns>Data set 
        /// with media type's identifiers ("idMediaType" column) and media type's labels ("mediaType" column).
        /// </returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Impossible to execute query
        /// </exception>
		public virtual DataSet GetData() {
			DataSet ds = null;
			string sql = "";			

			//Determine if data row is active or obsolete
			int activationCode = DBConstantes.ActivationValues.UNACTIVATED;

            //Gets media type level’s table description(table label, prefix ...)
			vehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle);

            //Gets sub media level’s table description(table label, prefix)
			categoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category);
			
			//Gets basic media level’s table description(table label, prefix)
			basicMediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia);

            //Gets vehicle level’s table description(table label, prefix)
			mediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media);

			#region Building SQL Query

            /*Clause SELECT*/
			//Selects the identifier and label of media type level
			sql = "Select distinct " + vehicleTable.Prefix + ".id_vehicle," + vehicleTable.Prefix + ".vehicle ";

            /*Clause FROM*/
            sql += " from " + vehicleTable.SqlWithPrefix + ",";//Obtain the data table of Media type level
            sql += categoryTable.SqlWithPrefix + ",";//Obtain the data table of Sub Media level
            sql += basicMediaTable.SqlWithPrefix + ",";//Obtain the data table of Basic Media level
            sql += mediaTable.SqlWithPrefix + " ";//Obtain the data table of Vehicle level

            /*Clause WHERE*/
			sql += " where";
			//Defines for each table, data language identifier (example 33 for French, 44 for English)
			sql += " " + vehicleTable.Prefix + ".id_language=" + _session.DataLanguage.ToString();
			sql += " and " + categoryTable.Prefix + ".id_language=" + _session.DataLanguage.ToString();
			sql += " and " + basicMediaTable.Prefix + ".id_language=" + _session.DataLanguage.ToString();
			sql += " and " + mediaTable.Prefix + ".id_language=" + _session.DataLanguage.ToString();

            /*Defines activation code of each data row. In france for example the activation code of 
            the data must be inferior to 50. Otherwise, the data is considered as disabled.*/
			sql += " and " + vehicleTable.Prefix + ".activation<" + activationCode;
			sql += " and " + categoryTable.Prefix + ".activation<" + activationCode;
			sql += " and " + basicMediaTable.Prefix + ".activation<" + activationCode;
			sql += " and " + mediaTable.Prefix + ".activation<" + activationCode;

			/* Join between the various tables of vehicle classification.*/            		
			sql += " and " + vehicleTable.Prefix + ".id_vehicle=" + categoryTable.Prefix + ".id_vehicle";
			sql += " and " + categoryTable.Prefix + ".id_category=" + basicMediaTable.Prefix + ".id_category";
			sql += " and " + basicMediaTable.Prefix + ".id_basic_media=" + mediaTable.Prefix + ".id_basic_media";


			#endregion

            //Obtains the customer media universe 
            sql += GetAllowedMediaUniverse();

            //Obtains the media rights of the customer
			sql += GetMediaRights(true);

            //Order by media type label and media identifier
			sql += " order by " + vehicleTable.Prefix + ".vehicle," + vehicleTable.Prefix + ".id_vehicle";

			#region Execution of the query
			try {
                //Exectuing the SQL query
				ds = _session.Source.Fill(sql);

                /*The result must be a data table with firt field the identifier of the media ("idMediaType")
                 * and the second the label of the media ("mediaType").*/
                ds.Tables[0].Columns[0].ColumnName = "idMediaType";
				ds.Tables[0].Columns[1].ColumnName = "mediaType";
                
				return FilteringWithMediaAgencyFlag(ds);
			}
			catch (System.Exception err) {
				throw (new Exceptions.DetailMediaDALException("Impossible to execute query", err));
			}
			#endregion

		}
		#endregion

	
		#endregion

        #region  FilteringWithMediaAgencyFlag
        /// <summary>
        /// Filtering with media agency flag by media type
        /// </summary>
        /// <param name="ds">Data Set</param>
        /// <returns>Data Set </returns>
        protected DataSet FilteringWithMediaAgencyFlag(DataSet ds)
        {

            switch (_session.CurrentModule)
            {
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES:
                    DataTable dataTable = new DataTable();
                    DataColumn dataColumn;
                    DataTable dt = ds.Tables[0];

                    //ID Vehicle
                    dataColumn = new DataColumn();
                    dataColumn.DataType = Type.GetType("System.Int64");
                    dataColumn.ColumnName = "idMediaType";
                    dataTable.Columns.Add(dataColumn);

                    //Vehicle label
                    dataColumn = new DataColumn();
                    dataColumn.DataType = Type.GetType("System.String");
                    dataColumn.ColumnName = "mediaType";
                    dataTable.Columns.Add(dataColumn);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr;
                        foreach (DataRow row in dt.Rows)
                        {
                            Int64 idV = Convert.ToInt64(row["idVehicle"].ToString());
                            if (_session.CustomerLogin.CustomerMediaAgencyFlagAccess(idV))
                            {
                                dr = dataTable.NewRow();
                                dr["idMediaType"] = idV;
                                dr["mediaType"] = row["vehicle"].ToString();
                                dataTable.Rows.Add(dr);
                            }
                        }
                    }
                    DataSet newDS = new DataSet();
                    newDS.Tables.Add(dataTable);
                    return (newDS);
                default: return ds;
            }
        }
        #endregion
	}
}
