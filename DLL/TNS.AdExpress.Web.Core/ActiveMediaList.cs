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

using TNS.AdExpress.Web.Core.DataAccess;

namespace TNS.AdExpress.Web.Core{
    /// <summary>
    /// Classe utilis�e pour la cr�ation des listes des supports actifs
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
        private static string[] _vehicleList = new string[1]{"7"};
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
        /// M�thode utilis�e pour l'initialisation des listes des supports actifs
        /// </summary>
        public static void Init(){

            ArrayList activeMediaList=new ArrayList();
            DataSet ds;

            foreach (string str in _vehicleList) {

                ds = ActiveMediaListDataAccess.GetActiveMediaData(Int64.Parse(str));

                foreach (DataRow row in ds.Tables[0].Rows) { 
                    activeMediaList.Add(row["id_media"]);
                }
                   
                _htActiveMedia.Add(Int64.Parse(str),activeMediaList);

            }
            
        }
        #endregion

        #region Get Active Media List
        /// <summary>
        /// M�thode utilis�e pour generer la liste des supports actifs qui peut �tre utilis�e dans une requ�te SQL
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
