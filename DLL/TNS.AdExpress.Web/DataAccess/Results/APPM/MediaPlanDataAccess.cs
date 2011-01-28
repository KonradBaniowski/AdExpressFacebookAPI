#region Information
//Authors: K.Shehzad, D.Mussuma
//Date of Creation: 24/08/2005
//Date of modification:
#endregion
using System;
using System.Data;
using System.Text;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Functions;
using TNS.FrameWork.DB.Common;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using  DBCst=TNS.AdExpress.Constantes.DB;
namespace TNS.AdExpress.Web.DataAccess.Results.APPM
{
	/// <summary>
	/// This class calculates and returns the datasets which are used to construct APPM Media Plan
	/// </summary>
	public class MediaPlanDataAccess
	{
		

		#region APPM Data with Versions
		/// <summary>
		/// Calculates and returns the dataset for the Media Plan 	 
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <param name="dataSource">dataSource for creating Datasets</param>
		/// <param name="dateBegin">Starting date</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="baseTarget">Base Target</param>
		/// <param name="additionalTarget">Additional Target</param>
		/// <returns>dataset for media plan of APPM</returns>
		public static DataSet GetDataWithVersions(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget){
			
			#region Variables
			string mediaTableName=null;
			string mediaFieldName=null;
			string orderFieldName=null;
			string mediaJoinCondition=null;
			string groupByFieldName=null;
			string sql="";
			#endregion
				
				try{

					#region Construction de la requête					

					// Obtient les tables de la nomenclature
					mediaTableName=webSession.GenericMediaDetailLevel.GetSqlTables(DBCst.Schema.ADEXPRESS_SCHEMA);
					if(mediaTableName.Length>0)mediaTableName+=",";
													
					// Obtient les champs de la nomenclature					
					mediaFieldName=webSession.GenericMediaDetailLevel.GetSqlFields();
					// Obtient l'ordre des champs
					orderFieldName=webSession.GenericMediaDetailLevel.GetSqlOrderFields();
					// obtient la clause group by
					groupByFieldName=webSession.GenericMediaDetailLevel.GetSqlGroupByFields();
					// Obtient les jointures pour la nomenclature
					mediaJoinCondition=webSession.GenericMediaDetailLevel.GetSqlJoins(webSession.SiteLanguage,DBTables.DATA_PRESS_APPM_PREFIXE);

				
					// Sélection de la nomenclature Support
					sql+="select "+mediaFieldName+" ";

					// Sélection de la date
					sql+=", date_media_num  as publication_date";

					//Péridicité
					sql+=","+DBTables.DATA_PRESS_APPM_PREFIXE+".id_periodicity";
					
					// Tables
					sql+=" from "+mediaTableName;
					sql+=" "+DBSchema.APPM_SCHEMA+"."+DBTables.DATA_PRESS_APPM+" "+DBTables.DATA_PRESS_APPM_PREFIXE+", ";			
					sql+=DBSchema.APPM_SCHEMA+"."+DBTables.TARGET_MEDIA_ASSIGNEMNT+" "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE;						
					
					//Conditions media
					sql+="  where 0=0 and "+DBTables.DATA_PRESS_APPM_PREFIXE+".id_slogan!=0  "+mediaJoinCondition+"";
					
					
					//Media
					sql+=" and " + DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE + ".id_media_secodip = " + DBTables.DATA_PRESS_APPM_PREFIXE + ".id_media ";
					
					//dates						
					sql+=" and  date_media_num>="+dateBegin+" and  date_media_num<="+dateEnd;

				

					// Sous sélection de version
					string slogans=webSession.SloganIdList;
					// Zoom sur une version
					if(webSession.SloganIdZoom>0 ){
						sql+=" and "+DBTables.DATA_PRESS_APPM_PREFIXE+".id_slogan ="+webSession.SloganIdZoom+" ";
					}
					else{
						// affiner les version
						if(slogans.Length>0){
							sql+=" and "+DBTables.DATA_PRESS_APPM_PREFIXE+".id_slogan in("+slogans+") ";
						}
					}
			
					// Gestion des sélections et des droits
					//product selection
					if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
						sql += "  " + webSession.PrincipalProductUniverses[0].GetSqlConditions(DBTables.DATA_PRESS_APPM_PREFIXE, true);						
	
					//on one target
					sql+=" and "+DBTables.TARGET_MEDIA_ASSIGNEMNT_PREFIXE+".id_target in("+additionalTarget+") "; 			
					//outside encart
					sql+=" and "+DBCst.Tables.DATA_PRESS_APPM_PREFIXE+".id_inset is null ";

					//Media Universe
					sql+=WebFunctions.SQLGenerator.GetResultMediaUniverse(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE);			

					//media rights
					sql+=SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true);
					//product rights
                    TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(webSession.CurrentModule);
                   sql+=WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true, module.ProductRightBranches);
											
					// Ordre
					sql+="  Group by "+groupByFieldName+" ";
					// et la date
					sql+=", date_media_num ";
					sql+=","+DBTables.DATA_PRESS_APPM_PREFIXE+".id_periodicity ";

					// Ordre
					
						sql+="  Order by "+orderFieldName+" ";
						// et la date
						sql+=", date_media_num ";
						sql+=","+DBTables.DATA_PRESS_APPM_PREFIXE+".id_periodicity";
					
					#endregion
				}
				catch(System.Exception err) {
					throw(new WebExceptions.APPMMediaPlanDataAccessException("GetDataWithVersions:: Error while constructing the query for the Media Plan APPM ",err));
				}	

			#region Execution of the query
			try {
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception ex) {
				throw(new WebExceptions.APPMMediaPlanDataAccessException("GetData:: Error while executing the query for the Media Plan APPM ",ex));
			}		
			#endregion			
		}
		#endregion		

		#region Get all the publications for the selected media 
		//TODO : unreferenced method
		/// <summary>
		///  Calculates and returns the dataset for the Media Plan 	 
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Starting date</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="baseTarget">Base Target</param>
		/// <param name="additionalTarget">Additional Target</param>
		/// <param name="idsMedia">The ids of the media for whom all publications are to be searched</param>
		/// <returns>dataset for synthesis of APPM</returns>
		public static DataSet GetAllPublications(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget,string idsMedia)
		{
			#region variables
			StringBuilder sql = new StringBuilder(1000);
			#endregion

			#region Table and field names
			string dateField=DBTables.WEB_PLAN_PREFIXE+"."+WebFunctions.SQLGenerator.getDateFieldNameForAnalysisResult(webSession.DetailPeriod);
			string dataTableName=WebFunctions.SQLGenerator.getTableNameForAnalysisResult(webSession.DetailPeriod);
			#endregion
			
			#region Construction of the query
			
			#region Select
			//Vehicle
			sql.Append(" select ");			
			//Media and its periodicity
			sql.Append(DBTables.WEB_PLAN_PREFIXE+".id_media,"+DBTables.WEB_PLAN_PREFIXE+".id_periodicity,");
			//Units			
			//Date of publication
			sql.Append(dateField+" as publication_date");
			#endregion

			#region From
			sql.Append(" from ");			
			sql.Append(DBSchema.ADEXPRESS_SCHEMA+"."+dataTableName+" "+DBTables.WEB_PLAN_PREFIXE);			
			#endregion

			#region Where
			sql.Append(" where ");			
			sql.Append(dateField+" >="+dateBegin+" and "+dateField+" <="+dateEnd);
			sql.Append(" and "+DBTables.WEB_PLAN_PREFIXE+".id_media in("+idsMedia+")");			
			#endregion

			#region Group by
			sql.Append(" group by ");			
			sql.Append(DBTables.WEB_PLAN_PREFIXE+".id_media,"+DBTables.WEB_PLAN_PREFIXE+".id_periodicity , "+dateField);
			#endregion

			#region Order by
			sql.Append(" order by ");			
			sql.Append(DBTables.WEB_PLAN_PREFIXE+".id_media, "+dateField);
			#endregion

			#endregion

			#region Execution of the query
			try
			{
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err)
			{
				throw(new WebExceptions.APPMMediaPlanDataAccessException("GetData:: Error while executing the query for the Media Plan APPM ",err));
			}		
			#endregion			
		}
		#endregion
		
	}
}
