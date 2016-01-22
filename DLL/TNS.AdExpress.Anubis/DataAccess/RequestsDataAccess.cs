#region Informations
// Auteur: G. Facon
// Date de création: 05/08/2005
// Date de modification: 
//		25/10/2005 : G. Ragneau - ajout de DeleteOldRequests
#endregion

using System;
using System.Collections;
using System.Data;

using TNS.AdExpress.Anubis.Common.Configuration;
using TNS.AdExpress.Anubis.Constantes;
using TNS.AdExpress.Anubis.Exceptions;

using TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;

namespace TNS.AdExpress.Anubis.DataAccess{
	/// <summary>
	/// Gestion des listes de demandes
	/// </summary>
	public class RequestsDataAccess{

		#region Get
		/// <summary>
		/// Obtient la liste des demandes à traiter
		/// </summary>
		/// <param name="dataSource">Source de données</param>
		/// <param name="notIn">Condition not in</param>
		/// <param name="plgLst">Liste des plugins acceptables par Anubis (Si null, pas de limitation)</param>
		/// <returns>liste des demandes à traiter</returns>
		internal static DataTable Get(IDataSource dataSource, string notIn, Hashtable plgLst){

			#region requête
            string sql = "select ID_STATIC_NAV_SESSION, ID_PDF_RESULT_TYPE from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01).Label + "." + Tables.PDF_SESSION;
			sql+=" where STATUS="+Constantes.Result.status.newOne.GetHashCode();
			//chargement du filtre qui interdit les requêtes déjà chargées
			if(notIn.Length>0)
				sql+=" and id_static_nav_session not in("+notIn+")";
			//chargement du filtre sur les plugins autorisés
			if (plgLst != null){
				string filter = "";
				int i = 0;
				foreach (Plugin p in plgLst.Values){
					if (i <= 0){
						filter = " and ID_PDF_RESULT_TYPE in (";
					}
					else{
						filter += ",";
					}
					filter += p.ResultType;
					i++;
				}
				if (i>0){
					filter += ")";
				}
				sql += filter;
			}
			//filter "date au plus tot"
			sql += string.Format(" and (date_exec is null OR date_exec <= to_date('{0}', 'yyyyMMddHH24mi')) ", DateTime.Now.ToString("yyyyMMddHHmm"));

			sql += " order by DATE_CREATION";
			#endregion

			try{
				return(dataSource.Fill(sql).Tables[0]);
			}
			catch(System.Exception err){
				throw(new RequestsDataAccessException("Unable to load the list of customer requests",err));
			}

		}
		#endregion

		#region DeleteOldRequests
		/// <summary>
		/// Supprime les résultats périmés suivant chaque plug in
		/// </summary>
		/// <param name="dataSource">Source de données</param>
		/// <param name="plugLst">Liste des plugins à nettoyer</param>
		/// <returns>Liste des fichiers à détruire sous la forme(nom, login,type de résultat)</returns>
		internal static DataTable DeleteOldRequests(IDataSource dataSource, Hashtable plugLst){

			DataTable fileLst = null;
			string condition = String.Empty;

			#region Liste des fichiers à supprimer

			#region requête
            string sql = "select PDF_NAME, ID_LOGIN, ID_PDF_RESULT_TYPE from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01).Label + "." + Tables.PDF_SESSION;
			bool first = true;
			foreach(object obj in plugLst.Keys){
				if (((Plugin)plugLst[obj]).ResultLongevity>0){
					if(first){
						condition += " where ";
						first = false;
					}
					else{
						condition += " or ";
					}
					condition += "(ID_PDF_RESULT_TYPE="+((Constantes.Result.status)obj).GetHashCode();
					condition += " and SYSDATE - DATE_MODIFICATION > " + ((Plugin)plugLst[obj]).ResultLongevity ;
					condition += " and STATUS=" + Constantes.Result.status.sent.GetHashCode()+ ")" ;
				}
			}
			if (condition.Length < 1){
				return null;
			}
			sql += condition;
			#endregion

			#region exécution
			try{
				fileLst = dataSource.Fill(sql).Tables[0];
			}
			catch(System.Exception err){
				throw(new RequestsDataAccessException("Unable to load the list of files to delete",err));
			}
			#endregion

			#endregion

			#region suppression des enregistrements

			#region requête
            sql = "delete " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01).Label + "." + Tables.PDF_SESSION;
			sql += condition;
			#endregion

			#region exécution
			try{
				dataSource.Delete(sql);
			}
			catch(System.Exception err){
				throw(new RequestsDataAccessException("Unable to delete the old requests",err));
			}
			#endregion

			#endregion

			return fileLst;
		}
		#endregion

	}
}
