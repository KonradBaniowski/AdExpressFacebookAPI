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
        /// Display
        /// </summary>
        protected bool _display = true;
        /// <summary>
        /// Control ID
        /// </summary>
        protected string _controlId = string.Empty;
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
        /// <param param name="controlId">Control Id</param>
        /// <param name="validationMethod">Validation Methods</param>
        /// <param name="display">Display</param>
        public ControlLayer(string name, string controlId, string assemblyName, string className, string skinId, string validationMethod, bool display)
            : base(name, assemblyName, className) {
            _skinId = skinId;
            _validationMethod = validationMethod;
            _controlId = controlId;
            _display = display;
        }
        #endregion

        #region Assessor
        /// <summary>
        /// Display
        /// </summary>
        public bool Display {
            get { return _display; }
        }

        /// <summary>
        /// Control ID
        /// </summary>
        public string ControlId {
            get { return _controlId; }
        }
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
