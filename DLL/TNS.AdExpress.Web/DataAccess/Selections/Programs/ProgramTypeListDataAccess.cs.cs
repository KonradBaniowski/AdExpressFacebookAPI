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
	
			sql+=" from "+programLevel.GetSqlTables(TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA);
			
			sql+=" where 0=0 ";

			sql+=programLevel.GetSqlJoinsBetweenLevels(webSession.SiteLanguage);

			sql+=" order by "+programLevel.GetSqlOrderFields();

			#endregion

			#region Execution de la requ�te
			try{
				return(webSession.Source.Fill(sql));
			}
			catch(System.Exception err){
				throw(new TNS.AdExpress.Web.Exceptions.ProgramTypeListException("Impossible de charger le dataset",err));
			}
			#endregion
		
		}

		#endregion

	}
}