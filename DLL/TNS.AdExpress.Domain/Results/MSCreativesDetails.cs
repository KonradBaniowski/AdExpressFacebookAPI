#region Informations
// Auteur: Y. Rkaina
// Création: 17/10/2008
// Modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Domain.Results {
    /// <summary>
    /// Creatives details for MediaSchedule result and Anubis PDF plugins
    /// </summary>
    public class MSCreativesDetails {

        #region Variables
        /// <summary>
        /// Detail column list to load
        /// </summary>
        protected Dictionary<Int64, List<GenericColumnItemInformation>> _detailColumnsList = new Dictionary<Int64, List<GenericColumnItemInformation>>();
        /// <summary>
        /// Detail column by vehicle
        /// </summary>
        protected Dictionary<Int64, Int64> _detailColumnsByVehicle = new Dictionary<Int64, Int64>();
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Data source</param>
        public MSCreativesDetails(IDataSource source) {
            MSCreativesDetailsXL.Load(source, _detailColumnsList, _detailColumnsByVehicle);
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get vehicle detail column
        /// </summary>
        /// <param name="idVehicle">Vehicle id</param>
        /// <returns>Liste des colonnes</returns>
        public List<GenericColumnItemInformation> GetDetailColumns(Int64 idVehicle) {
            try {
                return _detailColumnsList[idVehicle];
            }
            catch (System.Exception) {
                return (null);
            }
        }
        /// <summary>
        /// Get Detail Column Set Id
        /// </summary>
        /// <param name="idVehicle">Current vehicle Id</param>
        /// <returns>Column Detail Id matching idVehicle</returns>
        public Int64 GetDetailColumnsId(Int64 idVehicle) {
            try {
                return _detailColumnsByVehicle[idVehicle];

            }
            catch (System.Exception) {
                return (-1);
            }
        }
        /// <summary>
        /// Contains vehicle or not
        /// </summary>
        /// <param name="idVehicle">Current vehicle Id</param>
        /// <returns>Contains vehicle or not</returns>
        public bool ContainsVehicle(Int64 idVehicle) {
            return (_detailColumnsByVehicle != null && _detailColumnsByVehicle.ContainsKey(idVehicle)) ;
        }
        #endregion

    }
}
