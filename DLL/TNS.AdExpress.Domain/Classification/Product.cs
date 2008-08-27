#region Informations
// Author: G. Facon
// Creation Date: 27/08/2008 
// Modification Date: 
#endregion

using System;
using System.Collections;

using WebConstantes=TNS.AdExpress.Constantes.Web;
using DomainException=TNS.AdExpress.Domain.Exceptions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Domain.Classification {
	/// <summary>
	/// This classe contains product classification items Lists used in AdExpress web site
	/// </summary>
	/// <example>
	/// For exemple, in all the results we have to exclude some product
	/// </example>
    public class Product:ListContener<ProductItemsList>{

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        static Product() {
            _baalXmlNodeName="ProductUniverse";
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
