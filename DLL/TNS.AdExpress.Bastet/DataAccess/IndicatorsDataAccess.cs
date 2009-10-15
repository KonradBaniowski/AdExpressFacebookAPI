#region Informations
// Auteur: B. Masson
// Date de création: 23/02/2007
// Date de modification: 
#endregion

#region Namespaces
using System;
using System.Collections;
using System.Data;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Bastet.Exceptions;
using DBSchema = TNS.AdExpress.Constantes.DB.Schema;
using DBTable = TNS.AdExpress.Constantes.DB.Tables;
using DBLanguage = TNS.AdExpress.Constantes.DB.Language;
using DBActivation = TNS.AdExpress.Constantes.DB.ActivationValues;
#endregion

namespace TNS.AdExpress.Bastet.DataAccess{
	/// <summary>
	/// Classe des données des indicateurs
	/// </summary>
	public class IndicatorsDataAccess{

		/// <summary>
		/// Chargement des données des indicateurs
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="vehicleList">Liste des médias</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <returns>Données des indicateurs</returns>
        internal static DataSet GetDatas(IDataSource source, string vehicleList, DateTime dateBegin, DateTime dateEnd, int dataLanguageId) {

			#region Construction de la requête
			StringBuilder sql = new StringBuilder();

			sql.Append("select vh.id_vehicle, vh.vehicle, ct.id_category, ct.category, dc.id_media, md.media, dc.id_diffusion, dc.date_media_num, dc.nb_line ");
			sql.Append("from "+DBSchema.ADEXPRESS_SCHEMA+".data_control dc, "+DBSchema.ADEXPRESS_SCHEMA+".media md, "+DBSchema.ADEXPRESS_SCHEMA+".basic_media bm, "+DBSchema.ADEXPRESS_SCHEMA+".category ct, "+DBSchema.ADEXPRESS_SCHEMA+".vehicle vh ");
			// Jointures
			sql.Append("where dc.id_media = md.id_media and md.id_basic_media = bm.id_basic_media ");
			sql.Append("and bm.id_category = ct.id_category and ct.id_vehicle = vh.id_vehicle ");
			// Language
            sql.Append("and dc.id_language=" + dataLanguageId + " and md.id_language=" + dataLanguageId + " ");
            sql.Append("and bm.id_language=" + dataLanguageId + " and ct.id_language=" + dataLanguageId + " and vh.id_language=" + dataLanguageId + " ");
			// Activation (DEAD = 10 pour ne montrer que les supports ACTIFS uniquement)
			sql.Append("and md.activation<"+DBActivation.DEAD+" and bm.activation<"+DBActivation.DEAD+" ");
			sql.Append("and ct.activation<"+DBActivation.DEAD+" and vh.activation<"+DBActivation.DEAD+" ");
			// Paramètres
			sql.Append("and dc.date_media_num >= "+dateBegin.ToString("yyyyMMdd")+" ");
            sql.Append("and dc.date_media_num <= " + dateEnd.ToString("yyyyMMdd") + " ");
			sql.Append("and vh.id_vehicle in ("+vehicleList+") ");
			sql.Append("group by dc.id_data_control,vh.id_vehicle,vh.vehicle,ct.id_category,ct.category,dc.id_media,md.media,dc.id_diffusion,dc.date_media_num,dc.nb_line ");
			sql.Append("order by vh.vehicle, ct.category, md.media, dc.id_diffusion,dc.date_media_num ");
			#endregion

			#region Execution de la requête
			try{
				return(source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw (new IndicatorsDataAccessException("GetDatas Indicators : Impossible d'obtenir les données des indicateurs sur la période du "+dateBegin+" au "+dateEnd+" > "+err.Message, err));
			}
			#endregion

		}
		
	}
}
