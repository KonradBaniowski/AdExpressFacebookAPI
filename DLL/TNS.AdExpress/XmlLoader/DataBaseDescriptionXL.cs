#region Information
//  Author : G. Facon
//  Creation  date: 02/03/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.DataBaseDescription;



namespace TNS.AdExpress.XmlLoader{
    /// <summary>
    /// Load Database description
    /// </summary>
    public class DataBaseDescriptionXL {

        /// <summary>
        /// Load customer connections
        /// </summary>
        /// <param name="source">source</param>
        /// <returns>Customer connections list</returns>
        public static Dictionary<CustomerConnectionIds,CustomerConnection> LoadCustomerConnections(IDataSource source) {
            return (null);
        }
    }
}
