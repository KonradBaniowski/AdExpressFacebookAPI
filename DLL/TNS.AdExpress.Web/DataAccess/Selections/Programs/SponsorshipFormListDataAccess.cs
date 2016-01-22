#region Informations
// Auteur: Y. R'kaina
// Date de création: 28/11/2006
// Date de modification:
#endregion

using System;
using System.Collections;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.DataAccess.Selections.Programs {
	/// <summary>
	/// Chargement de liste : genres émissions, émissions
	/// </summary>
	public class SponsorshipFormListDataAccess {

		#region GetSponsorshipFormListDataAccess
		/// <summary>
		/// Charge la liste des genres d'émissions qui peuvent être sélectionnés
		/// </summary>
		/// <param name="webSession">WebSession</param>
		public static DataSet GetSponsorshipFormListDataAccess(WebSession webSession) {

			string sql="";

			ArrayList leveIds=new ArrayList();
			leveIds.Add(DetailLevelItemInformation.Levels.sponsorshipForm);
			GenericDetailLevel programLevel=new GenericDetailLevel(leveIds);

			#region requête

			sql+=" select "+ programLevel.GetSqlFields();

			sql += " from " + programLevel.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(TNS.AdExpress.Domain.DataBaseDescription.SchemaIds.adexpr03).Label);
			
			sql+=" where 0=0 ";

			sql+=programLevel.GetSqlJoinsBetweenLevels(webSession.DataLanguage);

			sql+=" order by "+programLevel.GetSqlOrderFields();

			#endregion

			#region Execution de la requête
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