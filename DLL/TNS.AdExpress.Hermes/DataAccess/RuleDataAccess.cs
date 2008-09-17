#region Informations
// Auteur : B.Masson
// Date de création : 13/02/2007
// Date de modification :
// 23/05/2008 Par B.Masson > 'date_media' changé en 'date_media_num' dans méthode GetMediaPressList
#endregion

#region Namespaces
using System;
using System.Collections;
using System.Data;
using System.Text;
using Oracle.DataAccess.Client;
using TNS.FrameWork.DB.Common;
using HermesExceptions=TNS.AdExpress.Hermes.Exceptions;
using DBCst=TNS.AdExpress.Constantes.DB;
using TNS.Baal.Common;
using TNS.Baal.ExtractList;

using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBActivation=TNS.AdExpress.Constantes.DB.ActivationValues;
using DBLanguage=TNS.AdExpress.Constantes.DB.Language;
#endregion

namespace TNS.AdExpress.Hermes.DataAccess{
	/// <summary>
	/// Classe pour la vérification d'une règle
	/// </summary>
	public class RuleDataAccess{

		#region Chargement des supports presse pigés
		/// <summary>
		/// Chargement des supports presse pigés au jour donné
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="mediaId">Identifiant de support</param>
		/// <param name="date">Date</param>
		/// <returns>Parutions du jour</returns>
		public static DataSet GetMediaPressList(IDataSource source, string mediaId, Int64 date){

			#region Construction de la requête
			StringBuilder sql=new StringBuilder(400);
            sql.Append("select id_media, date_media_num, date_cover_num ");
			sql.Append("from pitafr01.media_view ");
			sql.Append("where id_media in ("+mediaId+") ");
			sql.Append("and date_end_data >= to_date('"+date.ToString()+" 00:00:00','YYYYMMDD HH24:MI:SS') ");
			sql.Append("and date_end_data <= to_date('"+date.ToString()+" 23:59:59','YYYYMMDD HH24:MI:SS') ");
			sql.Append("order by date_media_num ");
			#endregion

			#region Execution de la requête
			try{
				return(source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new HermesExceptions.RuleDataAccessException("Error : "+err.Message, err));
			}
			#endregion

		}
		#endregion

		#region Chargement des données de vérification des supports
		/// <summary>
		/// Chargement des données de vérification des supports radio tv
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="parameters">Liste des paramètres de l'objet</param>
		/// <returns>Données de la vérification des supports</returns>
		public static DataSet GetDataControl(IDataSource source, RuleParameters parameters){

			#region Variables
			StringBuilder sql=new StringBuilder(500);
			string categoriesId = String.Empty;
			string mediasId = String.Empty;
			bool first=true;
			#endregion

			#region Chargement de la liste des supports (Baal)
			Liste list = Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId((int)parameters.MediaListId);
			ArrayList levels = (ArrayList)list.Levels;
			if(levels.Count==0)
				throw(new HermesExceptions.RuleDataAccessException("La liste ("+parameters.MediaListId.ToString()+") ne contient pas de support"));
			
			// Listes des supports et des catégories
			mediasId = list.GetLevelIds(Baal.ExtractList.Constantes.Levels.media);
			categoriesId = list.GetLevelIds(Baal.ExtractList.Constantes.Levels.category);
			#endregion

			#region Construction de la requête
			sql.Append("select id_media,"+Int64.Parse(parameters.Day.ToString("yyyyMMdd"))+" as date_media_num, sum(nbLine) as nbLine ");
			sql.Append("from (");
			
			// Début 1ère requête
			sql.Append("(select id_media, count(rownum) as nbLine ");	
			sql.Append("from "+DBCst.Schema.ADEXPRESS_SCHEMA+"."+parameters.TableName+" ");
			sql.Append("where date_media_num="+Int64.Parse(parameters.Day.ToString("yyyyMMdd"))+" ");
			if(mediasId.Length>0){
				sql.Append("and (id_media in ("+mediasId+") ");
				first=false;
			}
			if(categoriesId.Length>0){
				if(!first) sql.Append(" or ");
				else sql.Append(" and (");
				sql.Append("id_category in ("+categoriesId+") ");
				first=false;
			}
			if(!first) sql.Append(" ) ");
			switch(parameters.DiffusionId){
				case 1: // tranche 6/9
				case 2: // tranche 24h
					sql.Append("and id_top_diffusion >= "+parameters.HourBegin.ToString("HHmmss")+" and id_top_diffusion <= "+parameters.HourEnd.ToString("HHmmss")+" ");
					break;
			}
			sql.Append("group by id_media) ");	
			// Fin 1ère requête

			sql.Append("union");
			first=true;

			// Début 2ème requête
			sql.Append("(select id_media, 0 as nbLine ");
			sql.Append("from "+DBCst.Schema.ADEXPRESS_SCHEMA+"."+DBTables.MEDIA+" "+DBTables.MEDIA_PREFIXE+","+DBCst.Schema.ADEXPRESS_SCHEMA+".basic_media bm ");
			sql.Append("where "+DBTables.MEDIA_PREFIXE+".id_language="+DBLanguage.FRENCH+" ");
			sql.Append("and bm.id_language="+DBLanguage.FRENCH+" ");
			sql.Append("and "+DBTables.MEDIA_PREFIXE+".activation<"+DBActivation.DEAD+" ");
			sql.Append("and bm.activation<"+DBActivation.DEAD+" ");
			sql.Append("and "+DBTables.MEDIA_PREFIXE+".id_basic_media=bm.id_basic_media ");
			if(mediasId.Length>0){
				sql.Append("and (id_media in ("+mediasId+") ");
				first=false;
			}
			if(categoriesId.Length>0){
				if(!first) sql.Append(" or ");
				else sql.Append(" and (");
				sql.Append("id_category in ("+categoriesId+") ");
				first=false;
			}
			if(!first) sql.Append(" ) ");
			sql.Append(") ");
			// Fin 2ème requête
			
			sql.Append(")");
			sql.Append("group by id_media ");
			sql.Append("order by id_media ");

			#endregion

			#region Execution de la requête
			try{
				return(source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new HermesExceptions.RuleDataAccessException("Error : "+err.Message, err));
			}
			#endregion

		}

        /// <summary>
        /// Chargement des données de vérification d'un support en presse (ANCIENNE VERSION)
        /// </summary>
        /// <param name="source">Source de données</param>
        /// <param name="tableName">Nom de la table</param>
        /// <param name="mediaId">Identifiant du support</param>
        /// <param name="date">Date</param>
        /// <returns>Nombre de ligne</returns>
        public static Int64 GetDataControl(IDataSource source, string tableName, Int64 mediaId, Int64 date) {

            #region Construction de la requête
            StringBuilder sql = new StringBuilder(500);
            sql.Append("select count(rownum) ");
            sql.Append("from " + DBCst.Schema.ADEXPRESS_SCHEMA + "." + tableName + " ");
            sql.Append("where id_media=" + mediaId + " and date_media_num=" + date + " ");
            #endregion

            #region Execution de la requête
            try {
                DataSet ds = source.Fill(sql.ToString());
                return (Int64.Parse(ds.Tables[0].Rows[0][0].ToString()));
            }
            catch(System.Exception err) {
                throw (new HermesExceptions.RuleDataAccessException("Error : " + err.Message, err));
            }
            #endregion

        }
		#endregion

		#region Ecriture des données de vérification d'un support
		/// <summary>
		/// Ecriture des données de vérification d'un support
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="mediaId">Identifiant du support</param>
		/// <param name="dateMediaNum">Date du media</param>
		/// <param name="nbLine">Nombre de ligne</param>
		/// <param name="diffusionId">Identifiant de diffusion en radio</param>
		public static void SetDataControl(IDataSource source, Int64 mediaId, Int64 dateMediaNum, Int64 nbLine, Int64 diffusionId){
			
			#region Construction de la requête
			StringBuilder sql=new StringBuilder(500);
			sql.Append("insert into "+DBCst.Schema.ADEXPRESS_SCHEMA+".data_control values(");			
			// ID_DATA_CONTROL (Séquence)
			sql.Append( GetIdFromSequence(source) +",");
			// ID_MEDIA
			sql.Append( mediaId +",");
			// ID_LANGUAGE
			sql.Append( DBCst.Language.FRENCH +",");
			// NB_LINE
			sql.Append(nbLine +",");
			// DATE_MEDIA_NUM
			sql.Append( dateMediaNum +",");
			// DATE_CREATION
			sql.Append("sysdate,");
			// COMMENTARY
			sql.Append("null,");
			// ACTIVATION
			sql.Append( DBCst.ActivationValues.ACTIVATED +",");
			// ID_DIFFUSION
			switch(diffusionId){
				case 1: // tranche 6/9
				case 2: // tranche 9/24
					sql.Append( diffusionId );
					break;
				default:
					sql.Append("null");
					break;
			}
			sql.Append(")");
			#endregion

			#region Execution de la requête
			try{
				source.Insert(sql.ToString());
			}
			catch(System.Exception err){
				throw(new HermesExceptions.RuleDataAccessException("Error : "+err.Message, err));
			}
			#endregion

		}
		#endregion

		#region Méthode privée
		/// <summary>
		/// Obtient un identifiant depuis une séquence de SEQ_DATA_CONTROL
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <returns>Identifiant</returns>
		private static Int64 GetIdFromSequence(IDataSource source){
			try{
				string sql="select "+ DBCst.Schema.ADEXPRESS_SCHEMA +".SEQ_DATA_CONTROL.NEXTVAL from dual";
				DataSet ds = source.Fill(sql);
				return(Int64.Parse(ds.Tables[0].Rows[0][0].ToString()));
			}
			catch(System.Exception err){
				throw(new HermesExceptions.RuleDataAccessException("Error : "+err.Message, err));
			}
		}
		#endregion

	}
}
