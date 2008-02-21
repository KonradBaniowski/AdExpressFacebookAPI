#region Informations
// Auteur: A.DADOUCH
// Date de création: 22/08/2005
// 25/11/2005 Par B.Masson > webSession.Source
#endregion

using System;
using System.Data;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using TNS.FrameWork.DB.Common;
using ClassificationCst=TNS.AdExpress.Constantes.Classification;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using System.Text;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	/// Classe qui récupère les informations fichier resultats
	/// </summary>
	public class FilesItemDataAccess{

		/// <summary>
		///  Obtient la liste des fichiers PDF 
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dataSource">DataSource pour la creation de DataSet</param>
		/// <param name="typeList">Liste des types possibles pour les fichiers résultats</param> 
		/// <returns></returns>
		public static DataSet GetData(WebSession webSession,IDataSource dataSource, string typeList){
			
			#region Requete
			StringBuilder sql = new StringBuilder(1000);
			sql.Append("select pdf_name,id_static_nav_session , PDF_USER_FILENAME , ID_LOGIN, id_pdf_result_type   from ");
			sql.Append(DBConstantes.Schema.UNIVERS_SCHEMA+".STATIC_NAV_SESSION  ");
			sql.Append(" where  STATUS in (3) and id_pdf_result_type in ("+typeList+")");
			sql.Append(" and ID_LOGIN="+webSession.CustomerLogin.IdLogin.ToString());
			sql.Append(" order by id_pdf_result_type,pdf_name,DATE_CREATION ");
			#endregion

			#region Execution de la requête
			try{
				//return(dataSource.Fill(sql.ToString()));
				return(webSession.Source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.FilesItemDataAccessException("Impossible de charger les données de la Periodicity Plan ",err));
			}
			#endregion
			
		}

	}
}
