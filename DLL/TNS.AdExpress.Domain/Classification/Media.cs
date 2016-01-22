#region Informations
// Author: G. Facon
// Creation Date: 27/08/2008 
// Modification Date: 
#endregion

using System;
using System.Collections.Generic;

using WebConstantes=TNS.AdExpress.Constantes.Web;
using DomainException=TNS.AdExpress.Domain.Exceptions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Domain.Classification {
	/// <summary>
	/// This classe contains media classification items Lists used in AdExpress web site
	/// </summary>
	/// <example>
	/// For exemple the tendency omdule computes results for Press, radio and Tv.
	/// As customers can have access to other media, it is necessary to restrict the accessible media. For this one uses a list media defined in this class.
	/// </example>
    public class Media:ListContener<MediaItemsList> {
        
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        static Media() {
            _baalXmlNodeName="MediaUniverse";
        }
        #endregion

        /// <summary>
        /// Load Baal Lists from XML file
        /// </summary>
        /// <param name="source">DataSource</param>
        public static new void LoadBaalLists(IDataSource source) {
            ListContener<MediaItemsList>.LoadBaalLists(source);
        }
    }
}
