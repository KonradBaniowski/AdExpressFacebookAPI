using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Results;
using System.Globalization;
using System.Data;
using TNS.AdExpressI.VP.Loader.Exceptions;

namespace TNS.AdExpressI.VP.Loader.Default.Classification {
    /// <summary>
    /// Promo Veille Class
    /// </summary>
    public class ClassifVeillePromo : TNS.AdExpressI.VP.Loader.Classification.ClassifVeillePromo {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataBase">Data Base</param>
        public ClassifVeillePromo(DataBase dataBase):base(dataBase) {
        }
        #endregion

    }
}
