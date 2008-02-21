#region Information
// Author: G. Facon
// Creation Date: 25/04/2006
// Modification Date:
#endregion

using System;
using System.Data;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Sessions;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBActivation=TNS.AdExpress.Constantes.DB.ActivationValues;
using WebExceptions=TNS.AdExpress.Web.Core.Exceptions;
using Oracle.DataAccess.Client;

namespace TNS.AdExpress.Web.Core.DataAccess.Session{


	///<summary>
	/// Saved Detail Level management in DB
	/// </summary>
	///  <stereotype>utility</stereotype>
	public class GenericDetailLevelDataAccess {

		#region Public Methods
		/// <summary>
		/// Save a detail level
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		public static DataSet Load(WebSession webSession){

			#region Request
			string sql="select "+DBTables.LIST_DETAIL_PREFIXE+".id_list,"+DBTables.LIST_DETAIL_PREFIXE+".id_type_level,"+DBTables.LIST_DETAIL_PREFIXE+".rank ";
			sql+=" from "+DBSchema.UNIVERS_SCHEMA+"."+DBTables.LIST_DETAIL+" "+DBTables.LIST_DETAIL_PREFIXE+","+DBSchema.UNIVERS_SCHEMA+"."+DBTables.LIST+" "+DBTables.LIST_PREFIXE+" ";
			sql+=" where id_login="+webSession.CustomerLogin.IdLogin;
			sql+=" and "+DBTables.LIST_PREFIXE+".id_list="+DBTables.LIST_DETAIL_PREFIXE+".id_list ";
			sql+=" and "+DBTables.LIST_PREFIXE+".activation<"+DBActivation.UNACTIVATED.ToString();
			sql+=" order by "+DBTables.LIST_DETAIL_PREFIXE+".id_list,"+DBTables.LIST_DETAIL_PREFIXE+".rank ";
			#endregion

			#region Request execution
			try{
				return(webSession.Source.Fill(sql));
			}
			catch(System.Exception err){
				throw(new WebExceptions.GenericDetailLevelDataAccessExeption("Impossible to obtain the saved levels of detail ",err));
			}
			#endregion


		}

		/// <summary>
		/// Remove a saved detail level
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="genericDetailLevelSavedID">Level Id to remove</param>
		public static void Remove(WebSession webSession,Int64 genericDetailLevelSavedID){
			IDbTransaction transaction=null;
			try{
				#region Remove items
				string sqlItems="delete from "+DBSchema.UNIVERS_SCHEMA+"."+DBTables.LIST_DETAIL;
				sqlItems+=" where id_list="+genericDetailLevelSavedID.ToString();
				IDbConnection connection=(IDbConnection) webSession.Source.GetSource();
				webSession.Source.Open();
				transaction = connection.BeginTransaction();
				webSession.Source.Delete(sqlItems);
				#endregion

				#region Remove the list
				string sqlList="delete from "+DBSchema.UNIVERS_SCHEMA+"."+DBTables.LIST;
				sqlList+=" where id_list="+genericDetailLevelSavedID.ToString();
				webSession.Source.Delete(sqlList);
				transaction.Commit();
				#endregion
			}
			catch(System.Exception err){
				transaction.Rollback();
				throw(new WebExceptions.GenericDetailLevelDataAccessExeption("Impossible to remove the detail level",err));
			}
			finally{
				webSession.Source.Close();
			}
		}

		/// <summary>
		/// Save a detail level
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="genericDetailLevel">Level to save</param>
		public static Int64 Save(WebSession webSession,GenericDetailLevel genericDetailLevel){

			#region Get a new Id
			Int64 listId;
			try{
				listId =GetNewListId(webSession.Source);
			}
			catch(System.Exception err){
				throw(new WebExceptions.GenericDetailLevelDataAccessExeption("Impossible to get a new Id to save the detail level",err));
			}
			#endregion

			IDbTransaction transaction=null;
			try{
				#region Save the list
				string sqlList="insert into "+DBSchema.UNIVERS_SCHEMA+"."+DBTables.LIST+" values ";
				sqlList+="("+listId.ToString()+","+genericDetailLevel.Type.GetHashCode()+","+webSession.CustomerLogin.IdLogin+",sysdate,sysdate,'',"+DBActivation.ACTIVATED.ToString()+")";
				IDbConnection connection=(IDbConnection) webSession.Source.GetSource();
				webSession.Source.Open();
				transaction = connection.BeginTransaction();
				webSession.Source.Insert(sqlList);
				#endregion

				#region Save list values
				string sqlLevel;
				int i=0;
				foreach(DetailLevelItemInformation.Levels currentLevel in genericDetailLevel.LevelIds){
					sqlLevel="insert into "+DBSchema.UNIVERS_SCHEMA+"."+DBTables.LIST_DETAIL+" values ";
					sqlLevel+="("+listId.ToString()+","+currentLevel.GetHashCode().ToString()+","+i.ToString()+")";
					i++;
					webSession.Source.Insert(sqlLevel);
				}
				//Commit
				transaction.Commit();
				#endregion
			}
			catch(System.Exception err){
				transaction.Rollback();
				throw(new WebExceptions.GenericDetailLevelDataAccessExeption("Impossible to save the detail level",err));
			}
			finally{
				webSession.Source.Close();
			}
			return(listId);

		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Get a new detail level Id
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <returns>New detail level Id</returns>
		private static Int64 GetNewListId(IDataSource source){
			DataSet ds=null;
			#region Request
			string sql="select "+DBSchema.UNIVERS_SCHEMA+".SEQ_"+DBTables.LIST+".nextval from dual";
			#endregion

			#region Request Execution
			try{
				ds=source.Fill(sql);
			}
			catch(System.Exception err){
				throw(new WebExceptions.GenericDetailLevelDataAccessExeption("Impossible to get a Id",err));
			}
			if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count==1)return(Int64.Parse(ds.Tables[0].Rows[0][0].ToString()));
			throw(new WebExceptions.GenericDetailLevelDataAccessExeption("the data obtained to determine the identifier of the list are invalid"));
			#endregion
		}
		#endregion

	}
}
