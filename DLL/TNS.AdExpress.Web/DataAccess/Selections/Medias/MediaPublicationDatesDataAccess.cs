#region Information
//Authors: K.Shehzad, D.Mussuma
//Date of Creation: 29/08/2005
//Date of modification:
#endregion
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Functions;
using TNS.FrameWork.DB.Common;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer; 
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Translation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.Web.DataAccess.Selections.Medias
{
	/// <summary>
	/// Description r�sum�e de MediaPublicationDatesDataAccess.
	/// </summary>
	public class MediaPublicationDatesDataAccess
	{
		#region Get all the publications for the selected media 
		/// <summary>
		///  Calculates and returns the dataset for the Media Plan 	 
		/// </summary>
		/// <param name="dateBegin">Starting date</param>
		/// <param name="dateEnd">Ending date</param>		
		/// <returns>dataset with all the publications of media within the defined period</returns>
		internal static DataSet GetAllPublications(int dateBegin, int dateEnd)
		{
			#region variables
			StringBuilder sql = new StringBuilder(350);
            IDataSource dataSource=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.publication);
			#endregion

			#region Construction of the query
		
			#region Select
			sql.Append(" select distinct ");
			sql.Append("id_media,date_media_num as publication_date");			
			#endregion

			#region From
			sql.Append(" from ");			
			sql.Append(WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataPressAPPM).SqlWithPrefix); 
			#endregion

			#region Where
			sql.Append(" where ");
			sql.Append("date_media_num >=" + dateBegin + " and date_media_num<=" + dateEnd);				
			#endregion			

			#region Order by
			sql.Append(" order by ");
			sql.Append("id_media,date_media_num");
			#endregion

			#endregion

			#region Execution of the query
			try{
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.MediaPublicationDatesDataAccessException("GetAllPublications:: Error while executing the query for the Media Publication Data Access ",err));
			}		
			#endregion	
			
		}
		#endregion

		/// <summary>
		/// Get Internet last month publication date
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="periodType">Type de p�riode</param>
		/// <param name="moduleType">Type de module</param>
		/// <param name="idVehicle">ID m�dia</param>
		/// <returns></returns>
		public static string GetLatestPublication(WebSession webSession,WebConstantes.CustomerSessions.Period.DisplayLevel periodType,WebConstantes.Module.Type moduleType,Int64 idVehicle){
			
			string sql="";
			
			#region Construction de la requ�te
			sql+=" select max("+SQLGenerator.GetDateFieldName(moduleType,periodType)+") last_date ";
			sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label + ".";
			
			switch(periodType){
				case WebConstantes.CustomerSessions.Period.DisplayLevel.yearly:
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
					sql += WebApplicationParameters.DataBaseDescription.GetTable(TableIds.monthData).Label;
					break;
				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
					sql += WebApplicationParameters.DataBaseDescription.GetTable(TableIds.weekData).Label; 
					break;
				default:
					throw(new WebExceptions.MediaPublicationDatesDataAccessException("Le d�tails p�riode s�lectionn� est incorrect pour le choix de la table"));
			}
			#endregion
			sql+="  where id_vehicle="+idVehicle;

			#region Execution de la requ�te
			try{
				DataSet ds = webSession.Source.Fill(sql);
				if(ds != null &&  ds.Tables[0].Rows.Count>0  )
					return(ds.Tables[0].Rows[0]["last_date"].ToString());
				return null;
			}
			catch(System.Exception err){
				throw(new WebExceptions.MediaPublicationDatesDataAccessException ("Erreur dans la r�cup�ration de la date de derni�re parution d'un m�dia",err));
			}
			#endregion
			
		}
        /// <summary>
        /// Get last publication date
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <param name="idVehicle">Id m�dia</param>
		/// <param name="dataSource">Data source</param>
        /// <returns>Date</returns>
        public static string GetLatestPublication(WebSession webSession, Int64 idVehicle,IDataSource dataSource) {

            string sql = string.Empty;
            int positionUnivers = 1;
            string mediaList = string.Empty;
            string tableName = string.Empty;

            #region Construction de la requ�te

            switch (VehiclesInformation.DatabaseIdToEnum(idVehicle)) {
                case DBClassificationConstantes.Vehicles.names.internet:
                    tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataInternet).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataMarketingDirect).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataPress).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataTv).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.radio:
                    tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataRadio).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.outdoor:
                    tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataOutDoor).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.cinema:
                    tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataCinema).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                    tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataAdNetTrack).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataEvaliantMobile).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataNewspaper).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.magazine:
                    tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataMagazine).Sql;
                    break;

            }

            switch (VehiclesInformation.DatabaseIdToEnum(idVehicle)) {
                case DBClassificationConstantes.Vehicles.names.internet:
                    sql += " select max(" + DBConstantes.Fields.DATE_MEDIA_NUM + ") last_date ";
                    sql += " from " + tableName;
                    break;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    sql += " select max(" + DBConstantes.Fields.DATE_MEDIA_NUM + ") last_date ";
                    sql += " from " + tableName;
                    break;
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.cinema:
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    sql += " select min(last_date) as last_date ";
                    sql += " from (";

                    #region Media selection
                    if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE)
                        mediaList += webSession.GetSelection((TreeNode)webSession.ReferenceUniversMedia, CustormerConstantes.Right.type.mediaAccess) + ",";
                    else {
                        while (webSession.CompetitorUniversMedia[positionUnivers] != null) {
                            mediaList += webSession.GetSelection((TreeNode)webSession.CompetitorUniversMedia[positionUnivers], CustormerConstantes.Right.type.mediaAccess) + ",";
                            positionUnivers++;
                        }
                    }
                    #endregion

                    if (mediaList.Length > 0) {

                        string[] strs = mediaList.Substring(0, mediaList.Length - 1).Split(',');
                        int i = 0;

                        while (i < strs.Length) {
                            if (i > 0) {
                                sql += " UNION ";
                            }

                            sql += " select id_media, max(" + DBConstantes.Fields.DATE_MEDIA_NUM + ") last_date ";
                            sql += " from " + tableName;
                            sql += " where id_media = " + strs[i]+ "";
                            sql += " group by id_media ";

                            i += 1;
                        }
                    }

                    sql += " )";
                    break;
            }
            #endregion

            #region Execution de la requ�te
            try {
				DataSet ds = dataSource.Fill(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                    return (ds.Tables[0].Rows[0]["last_date"].ToString());
                return null;
            }
            catch (System.Exception err) {
                throw (new WebExceptions.MediaPublicationDatesDataAccessException("Erreur dans la r�cup�ration de la date de derni�re parution d'un m�dia", err));
            }
            #endregion
        
        }

	}
}
