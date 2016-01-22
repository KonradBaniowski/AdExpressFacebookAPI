using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Results;
using System.Data;

namespace TNS.AdExpressI.VP.Loader.DAL.Classification {
    /// <summary>
    /// Promo Veille Interface
    /// </summary>
    public interface IClassifVeillePromoDAL : IVeillePromoDAL {

        /// <summary>
        /// Get Data all product from data base
        /// </summary>
        /// <returns>Data all product from data base</returns>
        DataSet GetAllProduct();
        /// <summary>
        /// Get Data all brand from data base
        /// </summary>
        /// <returns>Data all product from data base</returns>
        DataSet GetAllBrand();
    }
}
