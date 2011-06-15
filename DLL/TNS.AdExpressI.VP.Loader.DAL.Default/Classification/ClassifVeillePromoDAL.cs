using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Results;
using System.Globalization;
using TNS.AdExpressI.VP.Loader.DAL.Exceptions;
using System.Data;

namespace TNS.AdExpressI.VP.Loader.DAL.Default.Classification {
    /// <summary>
    /// Promo Veille Class
    /// </summary>
    public class ClassifVeillePromoDAL : TNS.AdExpressI.VP.Loader.DAL.Classification.ClassifVeillePromoDAL {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataBase">Data Base</param>
        public ClassifVeillePromoDAL(DataBase dataBase):base(dataBase) {
        }
        #endregion

    }
}
