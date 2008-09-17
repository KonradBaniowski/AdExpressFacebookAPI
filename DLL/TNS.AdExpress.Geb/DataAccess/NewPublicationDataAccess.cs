#region Informations
// Auteur : B.Masson
// Date de cr�ation : 13/04/2006
// Date de modification :
//	19/04/2006 Par B.Masson > Ajout de nouvelles m�thode "DeleteMedia" et "UpdateMediaStatusType"
#endregion

using System;
using System.Data;
using System.Text;
using TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using GebExceptions=TNS.AdExpress.Geb.Exceptions;

namespace TNS.AdExpress.Geb.DataAccess{
	/// <summary>
	/// Classe des donn�es des nouveaux supports pour GebServer
	/// </summary>
	public class NewPublicationDataAccess{
		
		#region Chargement des nouveaux supports
		/// <summary>
		/// Charge les donn�es des nouveaux supports pig�s en presse
		/// </summary>
		/// <param name="source">Source de donn�es</param>
		/// <returns>Donn�es</returns>
		internal static DataSet GetData(IDataSource source){

			#region Construction de la requ�te
			StringBuilder sql = new StringBuilder(180);
			sql.Append(" select id_alert_push_mail, id_media, date_media_num, date_creation ");
			sql.Append(" from "+Schema.ADEXPRESS_SCHEMA+"."+ Tables.ALERT_PUSH_MAIL);
			sql.Append(" where id_status_type = "+ StatusType.NEW_ID_STATUS_TYPE);
			sql.Append(" order by date_creation,id_alert_push_mail");
			#endregion

			#region Execution de la requ�te
			try{
				return(source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new GebExceptions.NewPublicationDataAccessException("Impossible de charger les donn�es des nouveaux supports : "+sql.ToString()+" - "+err.Message, err));
			}
			#endregion
		}
		#endregion

		#region Suppression du supports dans la table
		/// <summary>
		/// Supprime le support de la table des nouveaux supports
		/// </summary>
		/// <param name="source">Source de donn�es</param>
		/// <param name="alertPushMailId">Identifiant de l'alerte</param>
		internal static void DeleteMedia(IDataSource source, Int64 alertPushMailId){
			if(alertPushMailId.ToString().Length < 0) throw(new GebExceptions.NewPublicationDataAccessException("L'identifiant de l'alerte est incorrecte"));
			if(alertPushMailId.ToString()==null) throw(new GebExceptions.NewPublicationDataAccessException("L'identifiant de l'alerte est vide"));

			#region Construction de la requ�te
			StringBuilder sql = new StringBuilder();
			sql.Append(" delete ");
			sql.Append(" from "+Schema.ADEXPRESS_SCHEMA+"."+ Tables.ALERT_PUSH_MAIL);
			sql.Append(" where id_alert_push_mail = "+alertPushMailId);
			#endregion

			#region Execution de la requ�te
			try{
				source.Delete(sql.ToString());
			}
			catch(System.Exception err){
				throw(new GebExceptions.NewPublicationDataAccessException("Impossible de supprimer l'alerte ("+alertPushMailId.ToString()+") de la table des nouveaux supports : "+sql.ToString()+" - "+err.Message, err));
			}
			#endregion
		}
		#endregion

		#region Changement de statut du support
		/// <summary>
		/// Change le statut du support
		/// </summary>
		/// <param name="source">Source de donn�es</param>
		/// <param name="alertPushMailId">Identifiant de l'alerte</param>
		internal static void UpdateMediaStatusType(IDataSource source, Int64 alertPushMailId){
			if(alertPushMailId.ToString().Length < 0) throw(new GebExceptions.NewPublicationDataAccessException("L'identifiant du support est incorrecte"));
			if(alertPushMailId.ToString()==null) throw(new GebExceptions.NewPublicationDataAccessException("L'identifiant du support est vide"));

			#region Construction de la requ�te
			StringBuilder sql = new StringBuilder();
			sql.Append(" update "+Schema.ADEXPRESS_SCHEMA+"."+ Tables.ALERT_PUSH_MAIL);
			sql.Append(" set id_status_type = "+ StatusType.ERROR_ID_STATUS_TYPE +", ");
			sql.Append(" date_modification = sysdate ");
			sql.Append(" where id_alert_push_mail = "+alertPushMailId);
			#endregion

			#region Execution de la requ�te
			try{
				source.Update(sql.ToString());
			}
			catch(System.Exception err){
				throw(new GebExceptions.NewPublicationDataAccessException("Impossible de changer le statut de l'alerte ("+alertPushMailId.ToString()+") dans la table des nouveaux supports : "+sql.ToString()+" - "+err.Message, err));
			}
			#endregion
		}
		#endregion

	}
}
