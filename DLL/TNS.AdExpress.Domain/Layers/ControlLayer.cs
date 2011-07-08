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
        /// <summary>
        /// Text Id
        /// </summary>
        protected Int64 _textId = 0;
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
        /// <param name="textId">Text Id</param>
        public ControlLayer(string name, string controlId, string assemblyName, string className, string skinId, string validationMethod, bool display, Int64 textId)
            : base(name, assemblyName, className) {
            _skinId = skinId;
            _validationMethod = validationMethod;
            _controlId = controlId;
            _display = display;
            _textId = textId;
        }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Display
        /// </summary>
        public bool Display {
            get { return _display; }
        }

        /// <summary>
        /// Get Control ID
        /// </summary>
        public string ControlId {
            get { return _controlId; }
        }
        /// <summary>
        /// Get Skin ID
        /// </summary>
        public string SkinId {
            get { return _skinId; }
        }
        /// <summary>
        /// Get Validation Method
        /// </summary>
        public string ValidationMethod {
            get { return _validationMethod; }
        }
        /// <summary>
        /// Get Text Id
        /// </summary>
        public Int64 TextId {
            get { return _textId; }
        }
        #endregion
    }
}
