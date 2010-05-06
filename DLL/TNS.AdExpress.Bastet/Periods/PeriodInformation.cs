#region Information
//  Author : G. Facon
//  Creation  date: 05/08/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Bastet.Periods {
    /// <summary>
    /// Unit description
    /// </summary>
    public class PeriodInformation {

        #region Variables
        /// <summary>
        /// Text Id
        /// </summary>
        private Int64 _webTextId;
        /// <summary>
        /// DataBase Id
        /// </summary>
        private Int64 _dataBaseId;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Unit Id</param>
        /// <param name="format">String format for output</param>
        /// <param name="webTextId">Text Id</param>
        /// <param name="baseId">Parent Id</param>
        /// <param name="databaseField">Field name in occurencies data</param>
        /// <param name="databaseMultimediaField">Field name in aggregated data</param>
        public PeriodInformation(Int64 webTextId, Int64 dataBaseId) {
            _webTextId=webTextId;
            _dataBaseId = dataBaseId;
        }
        #endregion

        #region Accessors

        /// <summary>
        /// Get Web Text Id
        /// </summary>
        public Int64 WebTextId {
            get { return (_webTextId); }
        }

        /// <summary>
        /// Get DataBase Id
        /// </summary>
        public Int64 DataBaseId {
            get { return (_dataBaseId); }
        }
        #endregion

    }
}
