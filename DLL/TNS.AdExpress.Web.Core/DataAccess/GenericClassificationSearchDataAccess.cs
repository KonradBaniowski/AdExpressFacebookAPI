#region Information
// Author: Y. R'kaina
// Creation date: 14/12/2006
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using WebExceptions=TNS.AdExpress.Web.Core.Exceptions;

namespace TNS.AdExpress.Web.Core.DataAccess{
	/// <summary>
	/// This class is used to search data by using detail levels
	/// </summary>
	public class GenericClassificationSearchDataAccess{

		#region External methods

		#region Search result list for two detail levels
		/// <summary>
		/// Search result by using two detail levels
		/// </summary>
		/// <returns>DataSet</returns>
		/// <param name="webSession">WebSession</param>
		/// <param name="newText">Text used for research</param>
		/// <param name="listParentLevel">Parent detail level list</param>
		/// <param name="listLevel">Detail level list</param>
		/// <param name="level">Detail level N2</param>
		/// <param name="parentLevel">Detail level N1</param>
		/// <exception cref="System.Exception">Thrown when is impossible to get data in GetSearchResultListDataAccess method</exception>
		public static DataSet GetSearchResultListDataAccess(WebSession webSession, string newText, string listParentLevel, string listLevel, DetailLevelItemInformation.Levels level, DetailLevelItemInformation.Levels parentLevel){

			string sql="";

			ArrayList leveIds=new ArrayList();
			leveIds.Add(parentLevel);
			leveIds.Add(level);
			GenericDetailLevel programLevel=new GenericDetailLevel(leveIds);
			DetailLevelItemInformation currentLevelItem = DetailLevelItemsInformation.Get((int)level);
			DetailLevelItemInformation parentLevelItem = DetailLevelItemsInformation.Get((int)parentLevel);

			#region Request construction
			sql+=" select distinct " + programLevel.GetSqlFields();
 			sql+=" from "+programLevel.GetSqlTables(TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA);
			sql+=" where 0=0 ";
			sql+=programLevel.GetSqlJoinsBetweenLevels(webSession.SiteLanguage);

			if(newText.Length>0){
				sql+=" and (" + parentLevelItem.DataBaseField + " like upper('%"+newText+"%')";
				if(listParentLevel.Length!=0){
					sql+=" or " + parentLevelItem.DataBaseTableNamePrefix + "." + parentLevelItem.DataBaseIdField + " in ("+listParentLevel+")";
				}
			}
			else{
				if(listParentLevel.Length!=0){
					sql+=" and ( " + parentLevelItem.DataBaseTableNamePrefix + "." + parentLevelItem.DataBaseIdField + " in ("+listParentLevel+")";
				}
			}

			if(newText.Length>0){
				sql+=" or " + currentLevelItem.DataBaseField + " like upper('%"+newText+"%')";
			}

			if(listLevel.Length!=0 && (listParentLevel.Length!=0 || newText.Length>0)){
				sql+=" or " + currentLevelItem.DataBaseTableNamePrefix + "." + currentLevelItem.DataBaseIdField + " in("+listLevel+")";
			}
			else if(listLevel.Length!=0){
				sql+=" and ( " + currentLevelItem.DataBaseTableNamePrefix + "." + currentLevelItem.DataBaseIdField + " in("+listLevel+")";
			}

			sql+=")";
			sql+=" order by "+programLevel.GetSqlOrderFields();
			#endregion

			#region Request execution
			try{
				return(webSession.Source.Fill(sql));
			}
			catch(System.Exception err){
				throw(new WebExceptions.GenericClassificationSearchDataAccessException("Impossible to get data in GetSearchResultListDataAccess method",err));
			}
			#endregion

		}
		#endregion

		#region Search result list for one detail level
		/// <summary>
		/// Search result by using one detail level
		/// </summary>
		/// <returns>DataSet contains detail level information</returns>
		/// <param name="webSession">WebSession</param>
		/// <param name="newText">Text used for research</param>
		/// <param name="listLevel">Detail level list</param>
		/// <param name="level">detail level N2</param>
 		/// <exception cref="System.Exception">Thrown when is impossible to get data in GetSearchResultListDataAccess method</exception>
		public static DataSet GetSearchResultListDataAccess(WebSession webSession, string newText, string listLevel, DetailLevelItemInformation.Levels level){

			string sql="";

			ArrayList leveIds=new ArrayList();
			leveIds.Add(level);
			GenericDetailLevel programLevel=new GenericDetailLevel(leveIds);
			DetailLevelItemInformation currentLevelItem = DetailLevelItemsInformation.Get((int)level);
			
			#region Request construction
			sql+=" select "+ programLevel.GetSqlFields();
			sql+=" from "+programLevel.GetSqlTables(TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA);
			sql+=" where 0=0 ";
			sql+=programLevel.GetSqlJoinsBetweenLevels(webSession.SiteLanguage);
			
			if(newText.Length>0){
				sql+=" and ( " + currentLevelItem.DataBaseField+ " like upper('%"+newText+"%')";
				if(listLevel.Length!=0){
					sql+=" or " + currentLevelItem.DataBaseIdField + " in ("+listLevel+")";
				}
			}
			else{
				if(listLevel.Length!=0){
					sql+=" and ( " + currentLevelItem.DataBaseIdField + " in ("+listLevel+")";
				}
			}

			sql+=")";
			sql+=" order by "+programLevel.GetSqlOrderFields();
			#endregion

			#region Request execution
			try{
				return(webSession.Source.Fill(sql));
			}
			catch(System.Exception err){
				throw(new WebExceptions.GenericClassificationSearchDataAccessException("Impossible to get data in GetSearchResultListDataAccess method",err));
			}
			#endregion
		
		}
		#endregion

		#endregion

	}
}
