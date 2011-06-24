#region Information
//  Author : Y. Rkaina && D. Mussuma
//  Creation  date: 15/07/2009
//  Modifications:
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Layers {

    /// <summary>
    /// Core Layer
    /// </summary>
    public class ControlLayer : LayerBase {

        #region Variables
        /// <summary>
        /// Skin ID
        /// </summary>
        protected string _skinId = string.Empty;
        /// <summary>
        /// Validation Method
        /// </summary>
        protected string _validationMethod = string.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Layer Name</param>
        /// <param name="assemblyName">Assembly name</param>
        /// <param name="className">Class name</param>
        /// <param name="skinId">Skin ID</pparam>
        /// <param name="validationMethod">Validation Methods</param>
        public ControlLayer(string name, string assemblyName, string className, string skinId, string validationMethod)
            : base(name, assemblyName, className) {
            _skinId = skinId;
            _validationMethod = validationMethod;
        }
        #endregion

        #region Assessor
        /// <summary>
        /// Skin ID
        /// </summary>
        public string SkinId {
            get { return _skinId; }
        }
        /// <summary>
        /// Validation Method
        /// </summary>
        public string ValidationMethod {
            get { return _validationMethod; }
        }
        #endregion
    }
}
