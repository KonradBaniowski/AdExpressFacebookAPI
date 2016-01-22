#region Informations
// Auteur : B.Masson
// Date de cr�ation : 14/04/2006
// Date de modification :
//	30/05/2006 > Requ�te dans GetData
#endregion

using System;
using System.Data;
using System.Text;
using TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using GebExceptions=TNS.AdExpress.Geb.Exceptions;

namespace TNS.AdExpress.Geb.DataAccess{
	/// <summary>
	/// Classe pour le chargement des donn�es des alertes pour GebServer
	/// </summary>
	public class AlertByMediaDataAccess{
	
		/// <summary>
		/// Charge les donn�es des alertes
		/// </summary>
		/// <param name="source">Source de donn�es</param>
		/// <returns>Donn�es</returns>
		/// <param name="dsNewMedia">dsNewMedia � virer qd la table sera cr��e</param>
		internal static DataSet GetData(IDataSource source, DataSet dsNewMedia){

			#region Construction de la requ�te
			StringBuilder sql = new StringBuilder(550);

			sql.Append("select "+Tables.ALERT_PREFIXE+".id_alert, "+ Schema.LOGIN_SCHEMA +".listnum_to_char(universe_list) as id_media ");
			sql.Append(" from "+ Schema.LOGIN_SCHEMA +"."+ Tables.ALERT +" "+ Tables.ALERT_PREFIXE +",");
			sql.Append(" "+ Schema.LOGIN_SCHEMA +"."+ Tables.ALERT_UNIVERSE_ASSIGNMENT +" "+ Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE +",");
			sql.Append(" "+ Schema.LOGIN_SCHEMA +"."+ Tables.ALERT_UNIVERSE +" "+ Tables.ALERT_UNIVERSE_PREFIXE +",");
			sql.Append(" "+ Schema.LOGIN_SCHEMA +"."+ Tables.ALERT_UNIVERSE_DETAIL +" "+ Tables.ALERT_UNIVERSE_DETAIL_PREFIXE);
			sql.Append(" where "+Tables.ALERT_PREFIXE+".id_alert = "+Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE+".id_alert ");
			sql.Append(" and "+Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE+".id_alert_universe = "+Tables.ALERT_UNIVERSE_PREFIXE+".id_alert_universe ");
			sql.Append(" and "+Tables.ALERT_UNIVERSE_PREFIXE+".id_alert_universe = "+Tables.ALERT_UNIVERSE_DETAIL_PREFIXE+".id_alert_universe ");
			sql.Append(" and "+Tables.ALERT_UNIVERSE_DETAIL_PREFIXE+".id_alert_universe_type = "+ AlertUniverseType.SUPPORT_VALUE);
			sql.Append(" and "+Tables.ALERT_PREFIXE+".date_beginning <= sysdate ");
			sql.Append(" and "+Tables.ALERT_PREFIXE+".date_end >= sysdate ");
			sql.Append(" and "+Tables.ALERT_PREFIXE+".activation < " + ActivationValues.UNACTIVATED);
			sql.Append(" and "+Tables.ALERT_UNIVERSE_DETAIL_PREFIXE+".activation < " + ActivationValues.UNACTIVATED);
			#endregion

			#region Execution de la requ�te
			try{
				return(source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new GebExceptions.AlertByMediaDataAccessException("Impossible de charger les alertes : "+sql.ToString()+" - "+err.Message, err));
			}
			#endregion
		}
		
	}
}
