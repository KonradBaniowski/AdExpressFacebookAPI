#region Information
// Author: Y. R'kaina
// Creation date: 21/07/2007
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Constantes.Classification.DB;

using TNS.AdExpress.Web.Core.DataAccess;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.Web.Core{
    /// <summary>
    /// Classe utilisée pour la création des listes des supports actifs
    /// </summary>
    public class ActiveMediaList{

        #region Variables
        /// <summary>
        /// Hashtable contenat pour chaque vehicle la liste des supports actifs qui lui correspond
        /// </summary>
        private static Hashtable _htActiveMedia;
        /// <summary>
        /// Liste des vehicles
        /// </summary>
		private static Vehicles.names[] _vehicleList = new Vehicles.names[1] { Vehicles.names.internet };
        #endregion

        #region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		static ActiveMediaList(){
            _htActiveMedia = new Hashtable();
		}
		#endregion

        #region Init
        /// <summary>
        /// Méthode utilisée pour l'initialisation des listes des supports actifs
        /// </summary>
        public static void Init(int siteLanguage){

            ArrayList activeMediaList=new ArrayList();
            DataSet ds;

			foreach (Vehicles.names str in _vehicleList) {

                ds = ActiveMediaListDataAccess.GetActiveMediaData(VehiclesInformation.EnumToDatabaseId(str), siteLanguage);

                foreach (DataRow row in ds.Tables[0].Rows) { 
                    activeMediaList.Add(row["id_media"]);
                }

				_htActiveMedia.Add(VehiclesInformation.EnumToDatabaseId(str), activeMediaList);

            }
            
        }
        #endregion

        #region Get Active Media List
        /// <summary>
        /// Méthode utilisée pour generer la liste des supports actifs qui peut être utilisée dans une requête SQL
        /// </summary>
        /// <returns>La liste des supports actifs</returns>
        public static string GetActiveMediaList(Int64 vehicleId) {

            string sql = string.Empty;

            foreach (Int64 mediaId in (ArrayList)_htActiveMedia[vehicleId]) {
                 sql += mediaId + ",";
            }

            if (sql.Length > 1){
                sql = sql.Substring(0, sql.Length - 1);
            }

            return sql;

        }
        #endregion

    }
}
