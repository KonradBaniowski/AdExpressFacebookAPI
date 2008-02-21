#region Informations
// Auteur: A. Obermeyer
// Date de création: 28/12/2004
// Date de modification:
// Par: 
#endregion

using System;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Navigation;
using TNS.AdExpress.Web.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebFunctions=TNS.AdExpress.Web.Functions;
using CstProject = TNS.AdExpress.Constantes.Project;

namespace TNS.AdExpress.Web.DataAccess.Selections.Periods{
	/// <summary>
	/// Accès aux données pour la sélection des dates 
	/// </summary>
	public class PortofolioAnalysisDateDataAccess{

//		/// <summary>
//		/// Retourne la liste des dates
//		/// </summary>
//		/// <param name="webSession">Session client</param>
//		/// <param name="idVehicle">identifiant vehicle</param>
//		/// <param name="idMedia">identifiant suppport</param>
//		/// <param name="dateBegin">Date de début</param>
//		/// <param name="dateEnd">Date de fin</param>
//		/// <param name="conditionEndDate">Mettre une condition pour la date de fin</param>
//		/// <returns></returns>	
//		public static DataSet getListDate(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd,bool conditionEndDate){
//			string tableName="";
//			try{
//				tableName= WebFunctions.SQLGenerator.getTableNameForAnalysisResult(webSession.DetailPeriod);
//			}
//			catch(Exceptions.PortofolioDateException err){
//				throw(new PortofolioDateException("Erreur lors de la sélection de la table",err));
//			}
//
//			#region Construction de la requête
//			StringBuilder sql=new StringBuilder(500);					
//			
//			sql.Append("select distinct first_day_m as date_media_num ");			
//			
//			if((int)idVehicle==DBClassificationConstantes.Vehicles.names.press.GetHashCode() 
//				|| (int)idVehicle==DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode() ){			
//				sql.Append(", disponibility_visual ");
//				sql.Append(", number_page_media ");
//			}
//			sql.Append(" from ");
//			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp ");
//
//			if((int)idVehicle==DBClassificationConstantes.Vehicles.names.press.GetHashCode() 
//				|| (int)idVehicle==DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode() ){				
//				sql.Append(","+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.APPLICATION_MEDIA+" am ");	
//				sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".alarm_media al ");
//			}
//			sql.Append(" where wp.id_media="+idMedia+" ");
//			// Période			
//			
//			if(dateBegin.Length>0)
//				sql.Append(" and wp.first_day_w>="+dateBegin);
//			if(dateEnd.Length>0 && conditionEndDate)
//				sql.Append(" and wp.first_day_w<="+dateEnd); 		
//
//			if((int)idVehicle==DBClassificationConstantes.Vehicles.names.press.GetHashCode() 
//				|| (int)idVehicle==DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode() ){
//				
//			//	sql.Append(" and am.id_language_data_i(+) = wp.id_language_data_i ");
//				sql.Append(" and am.date_debut(+) = wp.first_day_w ");				
//				
//				sql.Append(" and am.id_project(+) = "+ CstProject.ADEXPRESS_ID +" ");
//				sql.Append(" and am.id_media(+) = wp.id_media ");
//
//				sql.Append(" and wp.id_media=al.id_media(+)");
//				
//				sql.Append(" and wp.date_media_num=al.DATE_ALARM(+)");
//				
//				sql.Append(" and al.id_media(+)="+idMedia+" ");
//				sql.Append(" and al.ID_LANGUAGE_I(+)="+webSession.SiteLanguage+" ");
//				sql.Append(" and  al.DATE_ALARM(+)>="+dateBegin+" ");
//				if(dateEnd.Length>0 && conditionEndDate)
//					sql.Append(" and  al.DATE_ALARM(+)<="+dateEnd+" ");
//
//			}
//			// Tri			
//			sql.Append(" order by wp.first_day_w desc");
//			
//			#endregion
//
//			#region Execution de la requête
//			try{
//				return(webSession.Source.Fill(sql.ToString()));
//			}
//			catch(System.Exception err){
//				throw(new PortofolioDateException("Erreur lors de la sélection de la table",err));
//			}
//			#endregion
//
//			#region Ancien code
////			#region Execution de la requête
////			DataSet ds=new DataSet();
////			OracleConnection connection=new OracleConnection(webSession.CustomerLogin.OracleConnectionString);
////			OracleCommand sqlCommand=null;
////			OracleDataAdapter sqlAdapter=null;
////
////			#region Ouverture de la base de données
////			bool DBToClosed=false;
////			// On teste si la base est déjà ouverte
////			if (connection.State==System.Data.ConnectionState.Closed){
////				DBToClosed=true;
////				try{
////					connection.Open();
////				}
////				catch(System.Exception et){
////					throw(new PortofolioDateException("Impossible d'ouvrir la base de données:"+et.Message));
////				}
////			}
////			#endregion
////
////			try{
////				sqlCommand=new OracleCommand(sql.ToString(),connection);
////				sqlAdapter=new OracleDataAdapter(sqlCommand);
////				sqlAdapter.Fill(ds);
////			}
////				#region Traitement d'erreur du chargement des données
////			catch(System.Exception ex){
////				try{
////					// Fermeture de la base de données
////					if (sqlAdapter!=null){
////						sqlAdapter.Dispose();
////					}
////					if(sqlCommand!=null) sqlCommand.Dispose();
////					if (DBToClosed) connection.Close();
////				}
////				catch(System.Exception et){
////					throw(new PortofolioDateException ("Impossible de fermer la base de données, lors de la gestion d'erreur "+ex.Message+" : "+et.Message));
////				}
////				throw(new PortofolioDateException ("Impossible de charger les données pour afficherles créations:"+sql+" "+ex.Message));
////			}
////			#endregion
////
////			#region Fermeture de la base de données
////			try{
////				// Fermeture de la base de données
////				if (sqlAdapter!=null){
////					sqlAdapter.Dispose();
////				}
////				if(sqlCommand!=null)sqlCommand.Dispose();
////				if (DBToClosed) connection.Close();
////			}
////			catch(System.Exception et){
////				throw(new PortofolioDateException ("Impossible de fermer la base de données :"+et.Message));
////			}
////			#endregion
////
////			#endregion	
////
////			return (ds);
//			#endregion
//
//		}
		
	}
}
