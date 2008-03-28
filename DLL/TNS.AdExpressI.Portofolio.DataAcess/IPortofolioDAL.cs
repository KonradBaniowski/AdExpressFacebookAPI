#region Information
// Author: G. Facon
// Creation date: 26/03/2007
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Portofolio.DAL {
    /// <summary>
    /// Portofolio Data access Interface
    /// </summary>
    public interface IPortofolioDAL {
        /// <summary>
        /// Get Data for the Media portofolio
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetMediaPortofolio();
    }
}
