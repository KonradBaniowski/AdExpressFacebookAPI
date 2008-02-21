#region Information
// Auteur: A.Obermeyer
// Cr�� le: 19/04/2005
// Modifi�e le: 
//	23/08/2005	G. Facon		Solution temporaire pour les IDataSource
//	10/11/2005	D. V. Mussuma	Utilisation de IDataSource depuis WebSession
//	25/11/2005	B.Masson		webSession.Source
#endregion

#region Using
using System;
using System.Data;
using TNS.AdExpress.Web.Exceptions;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{

	/// <summary>
	/// Classe utilis�e pour le controle du DataAccess
	/// </summary>
	public class MediaAgencyDataAccess{

		/// <summary>
		/// Fournit la liste des ann�es pour les agences m�dias
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Liste des ann�es pour les agences m�dias</returns>
		public static DataSet GetListYear(WebSession webSession){

			#region Construction de la requ�te
			string sql=" select year from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".MEDIA_AGENCY_DISPO_YEAR ";
				   sql+=" where id_vehicle="+((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID;
				   sql+=" order by year desc ";
			#endregion

			#region Execution de la requ�te
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new MediaAgencyException ("Impossible de charger la liste des ann�es disponible pour les agences media: "+sql,err));
			}
			#endregion

		}
		
	}
}
