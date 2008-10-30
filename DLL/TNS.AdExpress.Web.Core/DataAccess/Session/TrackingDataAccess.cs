#region Information
// Author: G. Facon
// Création Date: 09/11/2005 
// Modification Date:
#endregion

using System;
using DBSchemas=TNS.AdExpress.Constantes.DB.Schema;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using DBActivation=TNS.AdExpress.Constantes.DB.ActivationValues;
using TNS.FrameWork.DB.Common;
using TrackingConstantes=TNS.AdExpress.Constantes.Tracking;
using WebExceptions=TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;

namespace TNS.AdExpress.Web.Core.DataAccess.Session{


	///<summary>
	/// Tracking Event Log in the data base 
	/// </summary>
	///  <stereotype>utility</stereotype>
	public class TrackingDataAccess {

		/// <summary>
		/// New connection event insertion
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <param name="sessionId">Customer session Id</param>
		/// <param name="loginId">Customer login Id</param>
		/// <param name="IP">Customer Ip</param>
		public static void NewConnection(IDataSource source,Int64 sessionId,Int64 loginId,string IP){
			Table trackingTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.tracking);
			Schema trackingSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);
			#region Request construction
			string sql = "insert into " + trackingTable.Sql+ " (id_tracking,id_nav_session,id_login,id_event,value_string,date_creation,date_modification,activation) values ";
			sql += "(" + trackingSchema.Label + ".seq_" + trackingTable.Label + ".nextval," + sessionId.ToString() + "," + loginId.ToString() + "," + TrackingConstantes.Event.Type.newConnection.GetHashCode().ToString() + ",'" + IP + "',sysdate,sysdate," + DBActivation.ACTIVATED + ")";
			#endregion

			#region Execution
			try{
				source.Insert(sql);
			}
			catch(System.Exception err){
				throw(new WebExceptions.TrackingDataAccessExeption("Impossible to save new connection Event: "+sql,err));
			}
			#endregion

		
		}
		
		/// <summary>
		/// New module selection event insertion
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <param name="sessionId">Customer session Id</param>
		/// <param name="loginId">Customer login Id</param>
		/// <param name="ModuleId">Module Id</param>
		public static void SetModule(IDataSource source,Int64 sessionId,Int64 loginId,Int64 ModuleId){
			Table trackingTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.tracking);
			Schema trackingSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);

			#region Request construction
			string sql = "insert into " + trackingTable.Sql+ " (id_tracking,id_nav_session,id_login,id_event,value,date_creation,date_modification,activation) values ";
			sql += "(" + trackingSchema.Label + ".seq_" + trackingTable.Label + ".nextval," + sessionId.ToString() + "," + loginId.ToString() + "," + TrackingConstantes.Event.Type.setModule.GetHashCode().ToString() + "," + ModuleId + ",sysdate,sysdate," + DBActivation.ACTIVATED + ")";
			#endregion

			#region Execution
			try{
				source.Insert(sql);
			}
			catch(System.Exception err){
				throw(new WebExceptions.TrackingDataAccessExeption("Impossible to save new module selection event: "+sql,err));
			}
			#endregion
		}

		/// <summary>
		/// New period selection event insertion
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <param name="sessionId">Customer session Id</param>
		/// <param name="loginId">Customer login Id</param>
		/// <param name="ModuleId">Module Id</param>
		/// <param name="periodTypeId">Period type Id</param>
		public static void SetPeriodType(IDataSource source,Int64 sessionId,Int64 loginId,Int64 ModuleId,int periodTypeId){
			Table trackingTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.tracking);
			Schema trackingSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);

			#region Request construction
			string sql = "insert into " + trackingTable .Sql+ " (id_tracking,id_nav_session,id_login,id_module,id_event,value,date_creation,date_modification,activation) values ";
			sql += "(" + trackingSchema.Label + ".seq_" + trackingTable.Label + ".nextval," + sessionId + "," + loginId + "," + ModuleId + "," + TrackingConstantes.Event.Type.setPeriodType.GetHashCode() + "," + periodTypeId + ",sysdate,sysdate," + DBActivation.ACTIVATED + ")";
			#endregion

			#region Execution
			try{
				source.Insert(sql);
			}
			catch(System.Exception err){
				throw(new WebExceptions.TrackingDataAccessExeption("Impossible to save New period selection event: "+sql,err));
			}
			#endregion
		}
		
		/// <summary>
		/// New unit selection event insertion
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <param name="sessionId">Customer session Id</param>
		/// <param name="loginId">Customer login Id</param>
		/// <param name="ModuleId">Module Id</param>
		/// <param name="unitTypeId">identifiant du type de période</param>
		public static void SetUnit(IDataSource source,Int64 sessionId,Int64 loginId,Int64 ModuleId,int unitTypeId){
			Table trackingTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.tracking);
			Schema trackingSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);
			
			#region Request construction
			string sql = "insert into " + trackingTable.Sql+ " (id_tracking,id_nav_session,id_login,id_module,id_event,value,date_creation,date_modification,activation) values ";
			sql += "(" + trackingSchema.Label + ".seq_" + trackingTable.Label + ".nextval," + sessionId + "," + loginId + "," + ModuleId + "," + TrackingConstantes.Event.Type.setUnit.GetHashCode() + "," + unitTypeId + ",sysdate,sysdate," + DBActivation.ACTIVATED + ")";
			#endregion

			#region Execution
			try{
				source.Insert(sql);
			}
			catch(System.Exception err){
				throw(new WebExceptions.TrackingDataAccessExeption("Impossible to save the new unit use"+sql,err));
			}
			#endregion
		}
		
		/// <summary>
		/// New result event insertion
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <param name="sessionId">Customer session Id</param>
		/// <param name="loginId">Customer login Id</param>
		/// <param name="ModuleId">Module Id</param>
		/// <param name="resultId">Result Id</param>
		public static void SetResult(IDataSource source,Int64 sessionId,Int64 loginId,Int64 ModuleId,Int64 resultId){
			Table trackingTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.tracking);
			Schema trackingSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);

			#region Request construction
			string sql = "insert into " + trackingTable.Sql+ " (id_tracking,id_nav_session,id_login,id_module,id_event,value,date_creation,date_modification,activation) values ";
			sql += "(" + trackingSchema.Label + ".seq_" + trackingTable.Label + ".nextval," + sessionId + "," + loginId + "," + ModuleId + "," + TrackingConstantes.Event.Type.setResult.GetHashCode() + "," + resultId + ",sysdate,sysdate," + DBActivation.ACTIVATED + ")";
			#endregion

			#region Execution
			try{
				source.Insert(sql);
			}
			catch(System.Exception err){
				throw(new WebExceptions.TrackingDataAccessExeption("Impossible to save new result event: "+sql,err));
			}
			#endregion
		}

		/// <summary>
		/// Use gad event insertion
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <param name="sessionId">Customer session Id</param>
		/// <param name="loginId">Customer login Id</param>
		/// <param name="ModuleId">Module Id</param>
		/// <param name="resultId">Result Id</param>
		public static void UseGad(IDataSource source,Int64 sessionId,Int64 loginId,Int64 ModuleId,Int64 resultId){
			Table trackingTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.tracking);
			Schema trackingSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);

			#region Request construction
			string sql="insert into "+ trackingTable.Sql+" (id_tracking,id_nav_session,id_login,id_module,id_result,id_event,date_creation,date_modification,activation) values ";
			sql+="("+trackingSchema.Label+".seq_"+trackingTable.Label+".nextval,"+sessionId+","+loginId+","+ModuleId+","+resultId+","+TrackingConstantes.Event.Type.useGad.GetHashCode()+",sysdate,sysdate,"+DBActivation.ACTIVATED+")";
			#endregion

			#region Execution
			try{
				source.Insert(sql);
			}
			catch(System.Exception err){
				throw(new WebExceptions.TrackingDataAccessExeption("Impossible to save the gad use: "+sql,err));
			}
			#endregion
		}

		/// <summary>
		/// Use of a saved result
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <param name="sessionId">Customer session Id</param>
		/// <param name="loginId">Customer login Id</param>
		/// <param name="ModuleId">Module Id</param>
		/// <param name="resultId">Result Id</param>
		public static void UseMyAdExpressSave(IDataSource source,Int64 sessionId,Int64 loginId,Int64 ModuleId,Int64 resultId){
			Table trackingTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.tracking);
			Schema trackingSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);

			#region Request construction
			string sql="insert into "+ trackingTable.Sql+" (id_tracking,id_nav_session,id_login,id_module,id_result,id_event,date_creation,date_modification,activation) values ";
			sql+="("+trackingSchema.Label+".seq_"+trackingTable.Label+".nextval,"+sessionId+","+loginId+","+ModuleId+","+resultId+","+TrackingConstantes.Event.Type.useMyAdExpressSave.GetHashCode()+",sysdate,sysdate,"+DBActivation.ACTIVATED+")";
			#endregion

			#region Execution
			try{
				source.Insert(sql);
			}
			catch(System.Exception err){
				throw(new WebExceptions.TrackingDataAccessExeption("Impossible to save the Use of a saved result event: "+sql,err));
			}
			#endregion
		}

		/// <summary>
		/// Use export file event insertion
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <param name="sessionId">Customer session Id</param>
		/// <param name="loginId">Customer login Id</param>
		/// <param name="ModuleId">Module Id</param>
		/// <param name="resultId">Result Id</param>
		public static void UseFileExport(IDataSource source,Int64 sessionId,Int64 loginId,Int64 ModuleId,Int64 resultId){
			Table trackingTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.tracking);
			Schema trackingSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);

			#region Request construction
			string sql="insert into "+ trackingTable.Sql+" (id_tracking,id_nav_session,id_login,id_module,id_result,id_event,date_creation,date_modification,activation) values ";
			sql+="("+trackingSchema.Label+".seq_"+trackingTable.Label+".nextval,"+sessionId+","+loginId+","+ModuleId+","+resultId+","+TrackingConstantes.Event.Type.useFileExport.GetHashCode()+",sysdate,sysdate,"+DBActivation.ACTIVATED+")";
			#endregion

			#region Execution
			try{
				source.Insert(sql);
			}
			catch(System.Exception err){
				throw(new WebExceptions.TrackingDataAccessExeption("Impossible to save Use export file event: "+sql,err));
			}
			#endregion
		}

		/// <summary>
		/// Use Media Agency event insertion
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <param name="sessionId">Customer session Id</param>
		/// <param name="loginId">Customer Login Id</param>
		/// <param name="ModuleId">Module Id</param>
		/// <param name="resultId">Result Id</param>
		public static void SetMediaAgency(IDataSource source,Int64 sessionId,Int64 loginId,Int64 ModuleId,Int64 resultId){
			Table trackingTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.tracking);
			Schema trackingSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);

			#region Request construction
			string sql="insert into "+ trackingTable.Sql+" (id_tracking,id_nav_session,id_login,id_module,id_result,id_event,date_creation,date_modification,activation) values ";
			sql+="("+trackingSchema.Label+".seq_"+trackingTable.Label+".nextval,"+sessionId+","+loginId+","+ModuleId+","+resultId+","+TrackingConstantes.Event.Type.setMediaAgency.GetHashCode()+",sysdate,sysdate,"+DBActivation.ACTIVATED+")";
			#endregion

			#region Execution
			try{
				source.Insert(sql);
			}
			catch(System.Exception err){
				throw(new WebExceptions.TrackingDataAccessExeption("Impossible to save the Use Media Agency event: "+sql,err));
			}
			#endregion
		}
		

		/// <summary>
		/// Vehicle selection event insertion
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <param name="sessionId">Customer session Id</param>
		/// <param name="loginId">Customer login Id</param>
		/// <param name="ModuleId">Module Id</param>
		/// <param name="vehicleId">Vehicle Id</param>
		public static void SetVehicle(IDataSource source,Int64 sessionId,Int64 loginId,Int64 ModuleId,Int64 vehicleId){
			Table trackingTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.tracking);
			Schema trackingSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01);

			#region Request construction
			string sql="insert into "+ trackingTable.Sql+" (id_tracking,id_nav_session,id_login,id_module,id_event,value,date_creation,date_modification,activation) values ";
			sql+="("+trackingSchema.Label+".seq_"+trackingTable.Label+".nextval,"+sessionId+","+loginId+","+ModuleId+","+TrackingConstantes.Event.Type.setVehicle.GetHashCode()+","+vehicleId+",sysdate,sysdate,"+DBActivation.ACTIVATED+")";
			#endregion

			#region Execution
			try{
				source.Insert(sql);
			}
			catch(System.Exception err){
				throw(new WebExceptions.TrackingDataAccessExeption("Impossible to save the vehicle selection"+sql,err));
			}
			#endregion
		}
	}
}
