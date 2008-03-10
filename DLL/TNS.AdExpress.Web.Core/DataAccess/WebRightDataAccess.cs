#region Informations
// Author:  
// Creation date:
// Modification date: 15/06/2004 (G. Facon)
//	11/08/2005	G. Facon	New Exception Management and property name
//	25/01/2007	B.Masson	Add fields for ModuleCategory in moduleListDB() method
#endregion

using System;
using System.Collections;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Web.Navigation;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using WebExceptions=TNS.AdExpress.Web.Core.Exceptions;
/*
namespace TNS.AdExpress.Web.Core.DataAccess{
	///<summary>
	/// Class used to manage custormer web rights
	/// </summary>
	[System.Serializable]
	public class WebRightDataAccess:TNS.AdExpress.Rules.Customer.Right{

		#region Variables
		/// <summary>
		/// Modules list
		/// </summary>
		protected Hashtable htModulesList=new Hashtable();
		#endregion		

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public WebRightDataAccess(string login, string password,IDataSource source):base(login,password,source){			
		}
		#endregion

		/// <summary>
		/// Get table corresponding to a module classification
		/// ModuleGroup / ModuleCategory / Module		
		/// </summary>
		/// <returns>dtModule</returns>
		/// <exception cref="TNS.AdExpress.Exceptions.AdExpressCustomerDBException">Thrown when is impossible to load module classification</exception>
		/// <exception cref="System.Exception">Thrown when is impossible to read data from the data base</exception>
		protected DataTable moduleListDB(){
			DataSet ds=null;

			#region Request for 3 levels : ModuleGroup / ModuleCategory / Module)
			string	sql=" select mo.id_module_group,mo.id_module, mc.id_module_category ";
			sql+=" from "+DBConstantes.Schema.RIGHT_SCHEMA+".module_assignment ma,"+DBConstantes.Schema.RIGHT_SCHEMA+".module mo,"+DBConstantes.Schema.RIGHT_SCHEMA+".module_group mg, "+DBConstantes.Schema.RIGHT_SCHEMA+".module_category mc ";
			sql+=" where ma.id_module=mo.id_module";
			sql+=" and ma.id_module not in("+TNS.AdExpress.Constantes.Web.Module.NOT_USED_ID_LIST+") ";
			sql+=" and mo.id_module_group=mg.id_module_group";
			sql+=" and mo.id_module_category = mc.id_module_category(+) ";
			sql+=" and ma.id_login="+idLogin+"";
			sql+=" and mg.id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+"";
			sql+=" and ma.date_beginning_module<=sysdate";
			sql+=" and ma.date_end_module>=sysdate";
			sql+=" and ma.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and mo.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and mg.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" order by mg.module_group, mc.module_category, mo.module";
			#endregion

			#region Execution
			try{
				IDataSource sourceTmp=new OracleDataSource("User Id="+login+"; Password="+password+""+TNS.AdExpress.Constantes.DB.Connection.RIGHT_CONNECTION_STRING);
				ds=sourceTmp.Fill(sql);
				if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows!=null){
					ds.Tables[0].TableName="listModule";
					ds.Tables[0].Columns[0].ColumnName="idGroupModule";
					ds.Tables[0].Columns[1].ColumnName="idModule";
					ds.Tables[0].Columns[2].ColumnName="idModuleCategory";
				}
				else
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Impossible to load module classification"));
				foreach(DataRow currentRow in ds.Tables[0].Rows){
					if (!htModulesList.ContainsKey((Int64)currentRow[1]))
						htModulesList.Add((Int64)currentRow[1],ModulesList.GetModule((Int64)currentRow[1]));
				}
			}
			catch(System.Exception err){	
				throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Error while data reading",err));				
			}
			#endregion

			return(ds.Tables[0]);
		}

		/// <summary>
		/// Get all the AdExpress Flags
		/// </summary>
		/// <returns>Hashtable contain for each AdExpress Flag its Id</returns>
		/// <exception cref="System.Exception">Thrown when is impossible to load the cusotmer accessible flags for AdExpress from the database</exception>
		protected Hashtable LoadFlagRight(IDataSource source){
			DataSet ds=null;
			Hashtable htFlag=new Hashtable();
			
			#region Request construction
			string sql="select FL.id_flag, flag ";
			sql+=" from "+DBConstantes.Schema.RIGHT_SCHEMA+".FLAG_PROJECT_ASSIGNMENT FP, ";
			sql+=" "+DBConstantes.Schema.RIGHT_SCHEMA+".FLAG FL" ;
			sql+=" where FL.id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+" ";
			sql+=" and FP.id_login="+idLogin+" ";
			sql+=" and FP.ID_FLAG=FL.ID_FLAG ";
			sql+=" and FP.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and FL.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			#endregion
			
			#region Request execution
			try{
				ds=source.Fill(sql);
				if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0){
					foreach(DataRow currentRow in ds.Tables[0].Rows){
						htFlag.Add((Int64)currentRow[0],currentRow[1].ToString());
					}
				}
			}
			catch(System.Exception err){
				throw(new WebExceptions.WebRightDataAccessException("Impossible to load the cusotmer accessible flags for AdExpress from the database",err));
			}
			#endregion

			return(htFlag);
		}

	}
}
*/