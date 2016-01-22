using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Results;
using System.IO;
using System.Globalization;
using TNS.AdExpress.VP.Loader.Domain.Classification;
using TNS.AdExpress.Constantes.DB;
using System.Data;

namespace TNS.AdExpressI.VP {
    /// <summary>
    /// Promo Veille Class
    /// </summary>
    public abstract class VeillePromo : IVeillePromo {

        #region Variables
        /// <summary>
        /// Data Base
        /// </summary>
        protected DataBase _dataBase = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataBase">Data Base</param>
        public VeillePromo(DataBase dataBase) {
            if (dataBase == null) throw new ArgumentNullException("DataBase parameter is null");
            _dataBase = dataBase;
        }
        #endregion

    }
}
