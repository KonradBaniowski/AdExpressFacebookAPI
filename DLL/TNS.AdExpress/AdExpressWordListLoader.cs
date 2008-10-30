using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.DataAccess.Translation;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;

namespace TNS.AdExpress {
    /// <summary>
    /// Load AdExpress Word list
    /// </summary>
    public class AdExpressWordListLoader {
        /// <summary>
        /// Load AdExpress Word list
        /// </summary>
        public static void LoadLists(){
            WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.translation);
        }
    }
}
