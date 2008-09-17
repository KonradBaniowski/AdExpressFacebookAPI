#region Informations
// Auteur : B.Masson
// Date de création : 21/04/2006
// Date de modification :
#endregion

using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

using TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using GebExceptions=TNS.AdExpress.Geb.Exceptions;
using GebConfiguration=TNS.AdExpress.Geb.Configuration;

namespace TNS.AdExpress.Geb.DataAccess.Configuration{
	/// <summary>
	/// Classe du chargement de la configuration d'une alerte
	/// </summary>
	public class AlertDataAccess{
		
		/// <summary>
		/// Charge la configuration des univers d'une alerte
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="alertId">Identifiant de l'alerte</param>
		/// <returns>Données de la configuration de l'alerte</returns>
		internal static DataSet LoadUniverse(IDataSource source, Int64 alertId){

			#region Construction de la requête
			StringBuilder sql = new StringBuilder(800);

			sql.Append("select "+ Tables.ALERT_PREFIXE +".id_alert, "+ Tables.ALERT_PREFIXE +".alert, "+ Tables.ALERT_PREFIXE +".id_alert_type, "+ Tables.ALERT_PREFIXE +".email_list, "+ Tables.ALERT_PREFIXE +".date_beginning, "+ Tables.ALERT_PREFIXE +".date_end, ");
			sql.Append(" "+ Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE +".rank, ");
			sql.Append(" "+ Tables.ALERT_UNIVERSE_PREFIXE +".reference_universe, ");
			sql.Append(" "+ Tables.ALERT_UNIVERSE_DETAIL_PREFIXE +".id_alert_universe_type, "+Schema.LOGIN_SCHEMA +".listnum_to_char(universe_list) as universe_list ");
			sql.Append(" from "+ Schema.LOGIN_SCHEMA +"."+ Tables.ALERT +" "+ Tables.ALERT_PREFIXE +",");
			sql.Append(" "+ Schema.LOGIN_SCHEMA +"."+ Tables.ALERT_UNIVERSE_ASSIGNMENT +" "+ Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE +",");
			sql.Append(" "+ Schema.LOGIN_SCHEMA +"."+ Tables.ALERT_UNIVERSE +" "+ Tables.ALERT_UNIVERSE_PREFIXE +",");
			sql.Append(" "+ Schema.LOGIN_SCHEMA +"."+ Tables.ALERT_UNIVERSE_DETAIL +" "+ Tables.ALERT_UNIVERSE_DETAIL_PREFIXE);
			sql.Append(" where "+Tables.ALERT_PREFIXE+".id_alert = "+Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE+".id_alert ");
			sql.Append(" and "+Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE+".id_alert_universe = "+Tables.ALERT_UNIVERSE_PREFIXE+".id_alert_universe ");
			sql.Append(" and "+Tables.ALERT_UNIVERSE_PREFIXE+".id_alert_universe = "+Tables.ALERT_UNIVERSE_DETAIL_PREFIXE+".id_alert_universe ");
			sql.Append(" and "+Tables.ALERT_PREFIXE+".id_alert = "+ alertId);
			sql.Append(" and "+Tables.ALERT_PREFIXE+".date_beginning <= sysdate ");
			sql.Append(" and "+Tables.ALERT_PREFIXE+".date_end >= sysdate ");
			sql.Append(" and "+Tables.ALERT_PREFIXE+".activation < " + ActivationValues.UNACTIVATED);
			sql.Append(" and "+Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE+".activation < " + ActivationValues.UNACTIVATED);
			sql.Append(" and "+Tables.ALERT_UNIVERSE_PREFIXE+".activation < " + ActivationValues.UNACTIVATED);
			sql.Append(" and "+Tables.ALERT_UNIVERSE_DETAIL_PREFIXE+".activation < " + ActivationValues.UNACTIVATED);
			sql.Append(" order by "+ Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE +".rank");
			#endregion

			#region Execution de la requête
			try{
				return(source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new GebExceptions.AlertByMediaDataAccessException("Impossible de charger la configuration des univers de l'alerte ("+alertId.ToString()+") : "+sql.ToString()+" - "+err.Message, err));
			}
			#endregion

		}

		/// <summary>
		/// Charge la configuration d'une alerte
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="alertId">Identifiant de l'alerte</param>
		/// <returns>Données de la configuration de l'alerte</returns>
		internal static DataSet LoadFlag(IDataSource source, Int64 alertId){

			#region Construction de la requête
			StringBuilder sql = new StringBuilder(350);

			sql.Append("select "+ Tables.ALERT_PREFIXE +".id_alert, "+ Tables.ALERT_PREFIXE +".alert, ");
			sql.Append(" "+ Tables.ALERT_FLAG_ASSIGNMENT_PREFIXE +".id_alert_flag ");
			sql.Append(" from "+ Schema.LOGIN_SCHEMA +"."+ Tables.ALERT +" "+ Tables.ALERT_PREFIXE +",");
			sql.Append(" "+ Schema.LOGIN_SCHEMA +"."+ Tables.ALERT_FLAG_ASSIGNMENT +" "+ Tables.ALERT_FLAG_ASSIGNMENT_PREFIXE +" ");
			sql.Append(" where "+Tables.ALERT_PREFIXE+".id_alert = "+Tables.ALERT_FLAG_ASSIGNMENT_PREFIXE+".id_alert ");
			sql.Append(" and "+Tables.ALERT_PREFIXE+".id_alert = "+ alertId);
			sql.Append(" and "+Tables.ALERT_PREFIXE+".date_beginning <= sysdate ");
			sql.Append(" and "+Tables.ALERT_PREFIXE+".date_end >= sysdate ");			
			sql.Append(" and "+Tables.ALERT_PREFIXE+".activation < " + ActivationValues.UNACTIVATED);
			sql.Append(" and "+Tables.ALERT_FLAG_ASSIGNMENT_PREFIXE+".activation < " + ActivationValues.UNACTIVATED);
			#endregion

			#region Execution de la requête
			try{
				return(source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new GebExceptions.AlertByMediaDataAccessException("Impossible de charger la configuration des flags de l'alerte ("+alertId.ToString()+") : "+sql.ToString()+" - "+err.Message, err));
			}
			#endregion

		}
	}
}
