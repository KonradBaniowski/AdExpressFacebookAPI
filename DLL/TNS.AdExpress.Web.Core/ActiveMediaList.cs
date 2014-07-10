#region Information
// Author: Y. R'kaina
// Creation date: 21/07/2007
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        private static Dictionary<long, string> _htActiveMedia;

        #endregion

        #region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		static ActiveMediaList(){
            _htActiveMedia = new Dictionary<long,string>();
		}
		#endregion

        #region Init
        /// <summary>
        /// Méthode utilisée pour l'initialisation des listes des supports actifs
        /// </summary>
        public static void Init(int siteLanguage){

           
            var vehicleIdsList = VehiclesInformation.GetDatabaseIds().Split(',');

            foreach (string currentVehicle in vehicleIdsList)
            {
                var vehicle = VehiclesInformation.Get(Int64.Parse(currentVehicle));
                if (vehicle.ShowActiveMedia)
                {
                    long vehicleDatabaseId = VehiclesInformation.EnumToDatabaseId(vehicle.Id);
                    var ds = ActiveMediaListDataAccess.GetActiveMediaData(vehicleDatabaseId, siteLanguage);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        _htActiveMedia.Add(vehicleDatabaseId,
                     string.Join(",", ds.Tables[0].Rows.Cast<DataRow>().Select(row => Convert.ToString(row["id_media"]))));

                    }
                }
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

            if (_htActiveMedia != null && _htActiveMedia.ContainsKey(vehicleId))
            {
                return _htActiveMedia[vehicleId];
            }

            return sql;

        }
        #endregion

    }
}
