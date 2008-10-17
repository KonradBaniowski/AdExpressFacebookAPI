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
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Data source</param>
        public MSCreativesDetails(IDataSource source) {
            MSCreativesDetailsXL.Load(source, _detailColumnsList);
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
        #endregion

    }
}
