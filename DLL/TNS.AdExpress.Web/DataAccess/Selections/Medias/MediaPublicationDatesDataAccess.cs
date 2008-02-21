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
using TNS.AdExpress.Web.Core.Navigation;
using WebConstantes=TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.DataAccess.Selections.Medias
{
	/// <summary>
	/// Description résumée de MediaPublicationDatesDataAccess.
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
			IDataSource dataSource=new OracleDataSource(new OracleConnection(TNS.AdExpress.Constantes.DB.Connection.PUBLICATION_DATES_CONNECTION_STRING));
			#endregion

			#region Construction of the query
		
			#region Select
			sql.Append(" select distinct ");
			sql.Append("id_media,date_media_num as publication_date");			
			#endregion

			#region From
			sql.Append(" from ");			
			sql.Append(DBSchema.APPM_SCHEMA+"."+DBTables.DATA_PRESS_APPM+" "+DBTables.DATA_PRESS_APPM_PREFIXE);			
			#endregion

			#region Where
			sql.Append(" where ");
			//sql.Append("date_parution_num >="+dateBegin+" and date_parution_num<="+dateEnd);				
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
		/// <param name="periodType">Type de période</param>
		/// <param name="moduleType">Type de module</param>
		/// <param name="idVehicle">ID média</param>
		/// <returns></returns>
		public static string GetLatestPublication(WebSession webSession,WebConstantes.CustomerSessions.Period.DisplayLevel periodType,WebConstantes.Module.Type moduleType,Int64 idVehicle){
			
			string sql="";
			
			#region Construction de la requête
			sql+=" select max("+SQLGenerator.GetDateFieldName(moduleType,periodType)+") last_date ";
			sql+=" from "+DBSchema.ADEXPRESS_SCHEMA+".";
			
			switch(periodType){
				case WebConstantes.CustomerSessions.Period.DisplayLevel.yearly:
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
					sql+=DBTables.WEB_PLAN_MEDIA_MONTH;
					break;
				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:				
					sql+=DBTables.WEB_PLAN_MEDIA_WEEK;
					break;
				default:
					throw(new WebExceptions.MediaPublicationDatesDataAccessException("Le détails période sélectionné est incorrect pour le choix de la table"));
			}
			#endregion
			sql+="  where id_vehicle="+idVehicle;

			#region Execution de la requête
			try{
				DataSet ds = webSession.Source.Fill(sql);
				if(ds != null &&  ds.Tables[0].Rows.Count>0  )
					return(ds.Tables[0].Rows[0]["last_date"].ToString());
				return null;
			}
			catch(System.Exception err){
				throw(new WebExceptions.MediaPublicationDatesDataAccessException ("Erreur dans la récupération de la date de dernière parution d'un média",err));
			}
			#endregion
			
		}
        /// <summary>
        /// Get last publication date
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <param name="idVehicle">Id média</param>
		/// <param name="dataSource">Data source</param>
        /// <returns>Date</returns>
        public static string GetLatestPublication(WebSession webSession, Int64 idVehicle,IDataSource dataSource) {

            string sql = string.Empty;
            int positionUnivers = 1;
            string mediaList = string.Empty;

            #region Construction de la requête


            switch ((DBClassificationConstantes.Vehicles.names)idVehicle) {
                case DBClassificationConstantes.Vehicles.names.internet:
                    sql += " select max(" + DBConstantes.Fields.DATE_MEDIA_NUM + ") last_date ";
                    sql += " from " + DBSchema.ADEXPRESS_SCHEMA + ".";
                    sql += DBTables.DATA_INTERNET;
                    break;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    sql += " select max(" + DBConstantes.Fields.DATE_MEDIA_NUM + ") last_date ";
                    sql += " from " + DBSchema.ADEXPRESS_SCHEMA + ".";
                    sql += DBTables.DATA_MARKETING_DIRECT;
                    break;
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    sql += " select min(last_date) as last_date ";
                    sql += " from (";
                    sql += " select id_media, max(" + DBConstantes.Fields.DATE_MEDIA_NUM + ") last_date ";
                    sql += " from " + DBSchema.ADEXPRESS_SCHEMA + ".";
                    sql += DBTables.DATA_PRESS;

                    #region Sélection de Médias
                    if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE)
                        mediaList += webSession.GetSelection((TreeNode)webSession.ReferenceUniversMedia, CustormerConstantes.Right.type.mediaAccess)+",";
                    else {
                        while (webSession.CompetitorUniversMedia[positionUnivers] != null) {
                            mediaList += webSession.GetSelection((TreeNode)webSession.CompetitorUniversMedia[positionUnivers], CustormerConstantes.Right.type.mediaAccess) + ",";
                            positionUnivers++;
                        }
                    }
		            if (mediaList.Length>0)sql+=" where id_media in ("+mediaList.Substring(0,mediaList.Length-1)+")";
		            #endregion

                    sql += " group by id_media )";
                    break;
            }
            #endregion

            #region Execution de la requête
            try {
				DataSet ds = dataSource.Fill(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                    return (ds.Tables[0].Rows[0]["last_date"].ToString());
                return null;
            }
            catch (System.Exception err) {
                throw (new WebExceptions.MediaPublicationDatesDataAccessException("Erreur dans la récupération de la date de dernière parution d'un média", err));
            }
            #endregion
        
        }

	}
}
