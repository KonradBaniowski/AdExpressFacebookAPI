using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Results;
using System.Globalization;
using System.Data;
using TNS.AdExpressI.VP.Loader.Exceptions;

namespace TNS.AdExpressI.VP.Loader.Default.Data {
    /// <summary>
    /// Promo Veille Class
    /// </summary>
    public class DataVeillePromo : TNS.AdExpressI.VP.Loader.Data.DataVeillePromo {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataBase">Data Base</param>
        /// <param name="dataLanguage">Data Language</param>
        public DataVeillePromo(DataBase dataBase)
            : base(dataBase) {
        }
        #endregion

    }
}
