using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Results;

namespace TNS.AdExpressI.VP.Loader.DAL {
    /// <summary>
    /// Promo Veille Interface
    /// </summary>
    public interface IVeillePromoDAL {

        /// <summary>
        /// Get Source
        /// </summary>
        IDataSource Source { get; }
    }
}
