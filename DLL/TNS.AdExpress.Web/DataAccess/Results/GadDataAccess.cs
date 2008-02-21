#region Informations
// Auteur: B. Masson
// Date de création: 27/09/2004
// Date de modification: 27/09/2004
//	23/08/2005	G. Facon		Solution temporaire pour les IDataSource
//	10/11/2005	D. V. Mussuma	Utilisation de IDataSource depuis WebSession
//	25/11/2005	B.Masson		webSession.Source
#endregion

#region Using
using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	/// Classe qui récupère les informations du GAD
	/// </summary>
	public class GadDataAccess{

		/// <summary>
		/// Obtient les données du gad
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="idAddress">Identifiant de l'adresse</param>
		/// <returns>Données</returns>
		public static DataSet GetData(WebSession webSession, string idAddress){

			#region Construction de la requête
			string sql="select ad.company, ad.street, ad.street2, ad.code_postal, ad.town,";
			sql+=" ct.telephone, ct.fax, ct.email ";
			//G Ragneau - intégration Doc Marketing - 22/01
			sql += string.Format(", gd.id_gad");
			sql += string.Format(", (select doc_marketing_key from {0}.DOC_MARKETING_KEY where date_media_num=to_number(to_char(sysdate, 'yyyyMMdd'))) as docKey", DBConstantes.Schema.UNIVERS_SCHEMA);
			//fin modif
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".address ad, "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".contact ct ";
			
			//G Ragneau - intégration Doc Marketing - 22/01
			sql += string.Format(",{0}.GAD gd", DBConstantes.Schema.ADEXPRESS_SCHEMA);
			//fin modif

			sql+=" where ad.id_address="+idAddress;
			sql+=" and ad.id_address=ct.id_address ";
			sql+=" and ad.activation<"+DBConstantes.ActivationValues.UNACTIVATED+"";
			sql+=" and ct.activation<"+DBConstantes.ActivationValues.UNACTIVATED+"";
			//G Ragneau - intégration Doc Marketing - 22/01
			sql += string.Format(" and gd.activation < {0} ", DBConstantes.ActivationValues.UNACTIVATED);
			sql += string.Format(" and gd.id_address(+)=ad.id_address ");
			//fin modif
			#endregion

			#region Execution de la requête
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new GadDataAccessException("Impossible de charger les données du gad "+sql,err));
			}
			#endregion

		}
	}
}
