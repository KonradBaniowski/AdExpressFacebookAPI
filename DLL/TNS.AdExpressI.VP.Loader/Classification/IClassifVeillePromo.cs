using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Results;
using System.Data;
using TNS.AdExpress.VP.Loader.Domain.Classification;

namespace TNS.AdExpressI.VP.Loader.Classification {
    /// <summary>
    /// Promo Veille Interface
    /// </summary>
    public interface IClassifVeillePromo : IVeillePromo {

        /// <summary>
        /// Get Data all product
        /// </summary>
        /// <returns>Data all product</returns>
        Dictionary<Int64, ProductListByCategoryListBySegment> GetAllProduct();
        /// <summary>
        /// Get Data all brand
        /// </summary>
        /// <returns>Data all product</returns>
        Dictionary<Int64, BrandListByCircuit> GetAllBrand();
    }
}
