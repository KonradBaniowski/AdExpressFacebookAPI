#region Informations
// Auteur : B.Masson
// Date de création : 24/04/2006
// Date de modification :
#endregion

using System;
using System.Data;
using System.Text;

using TNS.FrameWork.DB.Common;
using GebConfiguration=TNS.AdExpress.Geb.Configuration;
using GebAlertRequest=TNS.AdExpress.Geb.AlertRequest;
using GebExceptions=TNS.AdExpress.Anubis.Geb.Exceptions;

using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using CstWeb=TNS.AdExpress.Constantes.Web;
using WebFunctions=TNS.AdExpress.Web.Functions;
using CstProject = TNS.AdExpress.Constantes.Project;

namespace TNS.AdExpress.Anubis.Geb.DataAccess{
	/// <summary>
	/// Classe d'obtention des données de l'alerte
	/// </summary>
	public class GebExcelDataAccess{

		/// <summary>
		/// Calcul du détail support
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="alertParametersBlob">Paramètres de l'alerte du BLOB</param>
		/// <param name="alertParameters">Paramètres de l'alerte de la table BDD</param>
		/// <returns>Données du détail support</returns>
		internal static DataSet GetDetailMedia(IDataSource source, GebAlertRequest alertParametersBlob, GebConfiguration.Alert alertParameters){

			#region Constantes
			const string AUTOPROMO_GROUP_ID = "549";
			const string AUTOPROMO_SEGMENT_ID = "56202";
			#endregion

			#region Construction de la requête
			StringBuilder sql=new StringBuilder(5000);

			// Select
			sql.Append("select advertiser, ");
			sql.Append("product, ");
			sql.Append("sector, ");
			sql.Append("group_, ");
			sql.Append("area_page, ");
			sql.Append("area_mmc,format, ");
			sql.Append("wp.expenditure_euro, ");
			sql.Append("wp.date_media_num, ");
			sql.Append("wp.media_paging, ");
			sql.Append("rank_sector, ");
			sql.Append("rank_group_, ");
			sql.Append("rank_media, ");
			sql.Append("color, ");
			sql.Append("location, ");
			sql.Append("wp.date_media_num, ");
			sql.Append("media, ");
			sql.Append("LPAD(RTRIM(wp.Media_paging,' '),10,'0') as ChampPage, ");
			sql.Append("wp.id_advertisement, ");
            sql.Append("visual, ");
            sql.Append("wp.date_cover_num ");
			
			// From
			sql.Append(" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.ALERT_DATA_PRESS+" wp ");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".advertiser ad");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".product pr");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".sector se");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".group_ gr");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media me ");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".color co ");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".format fo ");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".location lo ");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".data_location dl ");

			// Conditions
			sql.Append(" where wp.id_media="+alertParameters.MediaId);
			sql.Append(" and wp.insertion=1 ");
			sql.Append(" and wp.date_media_num = "+alertParametersBlob.DateMediaNum);
			sql.Append(" and co.id_color(+)=wp.id_color ");
			sql.Append(" and fo.id_format(+)=wp.id_format ");
			sql.Append(" and lo.id_location(+)=dl.id_location ");
			sql.Append(" and dl.id_media(+)=wp.id_media ");
			sql.Append(" and dl.date_media_num(+) =wp.date_media_num ");
			sql.Append(" and dl.id_advertisement (+)=wp.id_advertisement ");

			sql.Append(" and co.id_language="+DBConstantes.Language.FRENCH);
			sql.Append(" and fo.id_language="+DBConstantes.Language.FRENCH);
			sql.Append(" and lo.id_language (+)="+DBConstantes.Language.FRENCH);

			sql.Append(" and co.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and fo.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and lo.activation(+)<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and dl.activation(+)<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);

			sql.Append(" and ad.id_advertiser=wp.id_advertiser ");
			sql.Append(" and pr.id_product=wp.id_product ");
			sql.Append(" and se.id_sector=wp.id_sector ");
			sql.Append(" and gr.id_group_=wp.id_group_ ");			
			sql.Append(" and  me.id_media=wp.id_media ");

			sql.Append(" and ad.id_language="+DBConstantes.Language.FRENCH);
			sql.Append(" and pr.id_language="+DBConstantes.Language.FRENCH);
			sql.Append(" and se.id_language="+DBConstantes.Language.FRENCH);
			sql.Append(" and gr.id_language="+DBConstantes.Language.FRENCH);
			sql.Append(" and me.id_language="+DBConstantes.Language.FRENCH);
			
			sql.Append(" and ad.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and pr.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);			
			sql.Append(" and se.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and gr.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and me.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);

			#region Univers produit de l'alerte
			bool first=true;
			string tmp="";
			if(alertParameters.SectorListId.Length>0){
				if(first){
					tmp+=" and (";
					first=false;
				}
				else tmp+=" or ";
				tmp+=" wp.id_sector in("+alertParameters.SectorListId+") ";
			}
			if(alertParameters.SubSectorListId.Length>0){
				if(first){
					tmp+=" and (";
					first=false;
				}
				else tmp+=" or ";
				tmp+=" wp.id_subsector in("+alertParameters.SubSectorListId+") ";
			}
			if(alertParameters.GroupListId.Length>0){
				if(first){
					tmp+=" and (";
					first=false;
				}
				else tmp+=" or ";
				tmp+=" wp.id_group_ in("+alertParameters.GroupListId+") ";
			}
			if(alertParameters.SegmentListId.Length>0){
				if(first){
					tmp+=" and (";
					first=false;
				}
				else tmp+=" or ";
				tmp+=" wp.id_segment in("+alertParameters.SegmentListId+") ";
			}
			if(tmp.Length>0){
				tmp+=") ";
				sql.Append(tmp);
			}
			#endregion

			if(!alertParameters.Autopromo){
				// HORS AUTOPROMO
				//sql.Append(" and wp.id.group_ not in(549) and wp.id_segment not in(56202)");
				sql.Append(" and wp.id_group_ not in("+AUTOPROMO_GROUP_ID+") and wp.id_segment not in("+AUTOPROMO_SEGMENT_ID+")");
			}
			if(!alertParameters.Inset){
				// HORS ENCART
				sql.Append(" and wp.id_inset is null"); // ??? C avec encart : is not null ???
			}

			// Order by
			sql.Append(" order by wp.Id_type_page, ChampPage, wp.id_product, wp.id_advertisement");
			#endregion

			#region Execution de la requête
			try{
				return source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new GebExceptions.GebExcelDataAccessException("Impossible de charger les données du détail du support : "+sql,err));
			}
			#endregion

		}

	}
}
