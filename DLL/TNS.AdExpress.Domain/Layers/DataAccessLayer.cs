#region Information
//  Author : G. Facon
//  Creation  date: 20/03/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Layers {
    /// <summary>
    /// DataAccess Layer
    /// </summary>
    public class DataAccessLayer:LayerBase {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Layer Name</param>
        /// <param name="assemblyName">Assembly name</param>
        /// <param name="className">Class name</param>
        public DataAccessLayer(string name,string assemblyName,string className)
            : base(name,assemblyName,className) {
        } 
        #endregion
    }
}
