using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.Aphrodite.Domain.Layers {
    /// <summary>
    /// DataAccess Layer
    /// </summary>
    public class DataAccessLayer : LayerBase {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Layer Name</param>
        /// <param name="assemblyName">Assembly name</param>
        /// <param name="className">Class name</param>
        public DataAccessLayer(string name, string assemblyName, string className)
            : base(name, assemblyName, className) {
        }
        public DataAccessLayer() : base() { }
        #endregion

    }
}
