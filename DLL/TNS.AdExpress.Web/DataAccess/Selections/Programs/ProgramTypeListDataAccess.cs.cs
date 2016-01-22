#region Informations
// Auteur: Y. R'kaina
// Date de cr�ation: 28/11/2006
// Date de modification:
#endregion

using System;
using System.Collections;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.DataAccess.Selections.Programs{
	/// <summary>
	/// Chargement de liste : genres �missions, �missions
	/// </summary>
	public class ProgramTypeListDataAccess {

		#region GetProgramTypeListDataAccess
		/// <summary>
		/// Charge la liste des genres d'�missions qui peuvent �tre s�lectionn�s
		/// </summary>
		/// <param name="webSession">WebSession</param>
		public static DataSet GetProgramTypeListDataAccess(WebSession webSession) {

			ArrayList leveIds=new ArrayList();
			leveIds.Add(DetailLevelItemInformation.Levels.programType);
			leveIds.Add(DetailLevelItemInformation.Levels.program);
			GenericDetailLevel programLevel=new GenericDetailLevel(leveIds);

			string sql="";

			#region requ�te
			
			sql+=" select distinct "+programLevel.GetSqlFields();		
	
			sql+=" from "+programLevel.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(TNS.AdExpress.Domain.DataBaseDescription.SchemaIds.adexpr03).Label);
			
			sql+=" where 0=0 ";

			sql+=programLevel.GetSqlJoinsBetweenLevels(webSession.DataLanguage);

			sql+=" order by "+programLevel.GetSqlOrderFields();

			#endregion

			#region Query Execution 
			try{
				return(webSession.Source.Fill(sql));
			}
			catch(System.Exception err){
				throw(new TNS.AdExpress.Web.Exceptions.ProgramTypeListException("Impossible to execute query",err));
			}
			#endregion
		
		}

		#endregion

	}
}