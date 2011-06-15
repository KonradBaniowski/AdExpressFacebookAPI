using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpressI.VP.Loader.DAL.Exceptions;
using System.Data;

namespace TNS.AdExpressI.VP.Loader.DAL.Default.Data {
    /// <summary>
    /// Promo Veille Class
    /// </summary>
    public class DataVeillePromoDAL : TNS.AdExpressI.VP.Loader.DAL.Data.DataVeillePromoDAL {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataBase">Data Base</param>
        /// <param name="dataLanguage">Data Language</param>
        public DataVeillePromoDAL(DataBase dataBase):base(dataBase) {
        }
        #endregion

    }
}
