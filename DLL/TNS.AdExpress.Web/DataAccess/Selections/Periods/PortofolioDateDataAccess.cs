#region Informations
// Auteur: A. Obermeyer
// Date de cr�ation: 29/11/2004
// Date de modification:
// Par: 
#endregion

using System;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using CstProject = TNS.AdExpress.Constantes.Project;

namespace TNS.AdExpress.Web.DataAccess.Selections.Periods{
	/// <summary>
	/// Classe d'acc�s aux donn�es pour le module portefeuille
	/// </summary>
	public class PortofolioDateDataAccess{

		/// <summary>
		/// Retourne la liste des dates
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant suppport</param>
		/// <param name="dateBegin">Date de d�but</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <param name="conditionEndDate">Mettre une condition sur la date de fin</param>
		/// <returns></returns>	
		public static DataSet GetListDate(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd,bool conditionEndDate){
			string tableName="";
			try{
				tableName = GetTable((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));				
			}
			catch(Exceptions.PortofolioDateException err){
				throw(new PortofolioDateException("Erreur lors de la s�lection de la table",err));
			}

			#region Construction de la requ�te
			StringBuilder sql=new StringBuilder(500);					
			
			sql.Append("select distinct date_media_num ");			
			
			if((int)idVehicle==DBClassificationConstantes.Vehicles.names.press.GetHashCode() 
				|| (int)idVehicle==DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode() ){			
				sql.Append(", disponibility_visual ");
				sql.Append(", number_page_media ");
				sql.Append(", date_cover_num ");
			}
			sql.Append(" from ");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp ");

			if((int)idVehicle==DBClassificationConstantes.Vehicles.names.press.GetHashCode() 
				|| (int)idVehicle==DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode() ){				
			sql.Append(","+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.APPLICATION_MEDIA+" am ");	
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"." + DBConstantes.Tables.ALARM_MEDIA + " al ");
			}
			sql.Append(" where wp.id_media="+idMedia+" ");
			// P�riode			
			
				if(dateBegin.Length>0)
					sql.Append(" and wp.date_media_num>="+dateBegin);
				if(dateEnd.Length>0 && conditionEndDate)
					sql.Append(" and wp.date_media_num<="+dateEnd); 		

			if((int)idVehicle==DBClassificationConstantes.Vehicles.names.press.GetHashCode() 
				|| (int)idVehicle==DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode() ){
				
			//		sql.Append(" and am.id_language_data_i(+) = wp.id_language_data_i ");
                    sql.Append(" and am.date_debut(+) = wp.date_media_num ");				
				
				sql.Append(" and am.id_project(+) = "+ CstProject.ADEXPRESS_ID +" ");
				sql.Append(" and am.id_media(+) = wp.id_media ");

				sql.Append(" and wp.id_media=al.id_media(+)");

                sql.Append(" and wp.date_media_num=al.DATE_ALARM(+)");
				
				sql.Append(" and al.id_media(+)="+idMedia+" ");
				sql.Append(" and al.ID_LANGUAGE_I(+)="+webSession.SiteLanguage+" ");
				sql.Append(" and  al.DATE_ALARM(+)>="+dateBegin+" ");
				if(dateEnd.Length>0 && conditionEndDate)
					sql.Append(" and  al.DATE_ALARM(+)<="+dateEnd+" ");

			}
			// Tri			
			sql.Append(" order by wp.date_media_num desc");
			
			#endregion

			#region Execution de la requ�te
			try{
				return(webSession.Source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new PortofolioDateException("Erreur lors de la s�lection de la table",err));
			}
			#endregion

		}
		
		/// <summary>
		/// R�cup�re la date de derni�re parution d'un m�dia
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant m�dia</param>
		/// <returns></returns>
		public static string LastPublication(WebSession webSession,Int64 idVehicle,Int64 idMedia){
			
			string sql="";
			//string lastDate="";
//			string insertOrNumberboard="insertion";
//			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.outdoor){
//				insertOrNumberboard="number_board";
//			}
			string table=GetTable((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));

			#region Construction de la requ�te
			sql+=" select max(DATE_MEDIA_NUM) last_date ";
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+table+" ";
			sql+=" where id_media="+idMedia+" ";
			//sql+=" and "+insertOrNumberboard+"=1 ";
			#endregion

			#region Execution de la requ�te
			try{
				DataSet ds=webSession.Source.Fill(sql);
				if(ds.Tables.Count>0 && ds.Tables[0].Rows.Count>0)
					return(ds.Tables[0].Rows[0]["last_date"].ToString());
				else
					throw(new PortofolioDateException ("Impossible de r�cup�rer la date de derni�re parution d'un m�dia"));
			}
			catch(System.Exception err){
				throw(new PortofolioDateException ("Erreur dans la r�cup�ration de la date de derni�re parution d'un m�dia",err));
			}
			#endregion

			#region Ancien code
//			#region Execution de la requ�te
//			OracleConnection connection=new OracleConnection(webSession.CustomerLogin.OracleConnectionString);
//			OracleCommand sqlCommand=null;
//			OracleDataReader sqlOracleDataReader=null;
//
//			OracleDataAdapter sqlAdapter=null;
//
//			#region Ouverture de la base de donn�es
//			bool DBToClosed=false;
//			// On teste si la base est d�j� ouverte
//			if (connection.State==System.Data.ConnectionState.Closed){
//				DBToClosed=true;
//				try{
//					connection.Open();
//				}
//				catch(System.Exception et){
//					throw(new PortofolioDateException("Impossible d'ouvrir la base de donn�es:"+et.Message));
//				}
//			}
//			#endregion
//
//			try{
//				sqlCommand=new OracleCommand(sql,connection);
//				sqlAdapter=new OracleDataAdapter(sqlCommand);
//				sqlOracleDataReader=sqlCommand.ExecuteReader();				
//				if(sqlOracleDataReader.Read()){
//					lastDate=sqlOracleDataReader["last_date"].ToString();
//				}
//
//			}
//			#region Traitement d'erreur du chargement des donn�es
//			catch(System.Exception ex){
//				try{
//					// Fermeture de la base de donn�es
//					if (sqlAdapter!=null){
//						sqlAdapter.Dispose();
//					}
//					if(sqlCommand!=null) sqlCommand.Dispose();
//					if (DBToClosed) connection.Close();
//				}
//				catch(System.Exception et){
//					throw(new PortofolioDateException ("Impossible de fermer la base de donn�es, lors de la gestion d'erreur "+ex.Message+" : "+et.Message));
//				}
//				throw(new PortofolioDateException ("Impossible de charger les donn�es pour afficherles cr�ations:"+sql+" "+ex.Message));
//			}
//			#endregion
//
//			#region Fermeture de la base de donn�es
//			try{
//				// Fermeture de la base de donn�es
//				if (sqlAdapter!=null){
//					sqlAdapter.Dispose();
//				}
//				if(sqlCommand!=null)sqlCommand.Dispose();
//				if (DBToClosed) connection.Close();
//			}
//			catch(System.Exception et){
//				throw(new PortofolioDateException ("Impossible de fermer la base de donn�es :"+et.Message));
//			}
//			#endregion
//
//			#endregion				
//
//			return lastDate;
			#endregion

		}

		/// <summary>
		/// Donne la table � utiliser pour le vehicle indiqu�
		/// </summary>
		/// <param name="idVehicle">Identifiant du vehicle</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Lanc�e quand le cas du vehicle sp�cifi� n'est pas trait�
		/// </exception>
		/// <returns>Chaine contenant le nom de la table correspondante</returns>
		private static string GetTable(DBClassificationConstantes.Vehicles.names idVehicle){		
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.press:
					return DBConstantes.Tables.ALERT_DATA_PRESS;
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return DBConstantes.Tables.ALERT_DATA_PRESS_INTER;
				case DBClassificationConstantes.Vehicles.names.radio:
					return DBConstantes.Tables.ALERT_DATA_RADIO;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return DBConstantes.Tables.ALERT_DATA_TV;
				case DBClassificationConstantes.Vehicles.names.outdoor:
					return DBConstantes.Tables.ALERT_DATA_OUTDOOR;
				default:
					throw new Exceptions.MediaCreationDataAccessException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce m�dia n'est pas g�rer. Pas de table correspondante.");
			}			
		}

	}
}
