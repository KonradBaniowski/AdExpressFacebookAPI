using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.Layers;

namespace TNS.AdExpress.Domain.Results
{
    public class VpConfigurationDetail {

        #region Variables
        /// <summary>
        /// Result Control Layer
        /// </summary>
        protected ControlLayer _resultControlLayer;
        /// <summary>
        /// Selection Control Layer List
        /// </summary>
        protected List<ControlLayer> _selectionControlLayerList;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultControlLayer">Result Control Layer</param>
        /// <param name="selectionControlLayerList">Selection Control Layer List</param>
        public VpConfigurationDetail(ControlLayer resultControlLayer, List<ControlLayer> selectionControlLayerList) {
            if (resultControlLayer == null) throw new ArgumentNullException("ResultControlLayer Parameter is null");
            if (selectionControlLayerList == null) throw new ArgumentNullException("SelectionControlLayerList Parameter is null");
            if (selectionControlLayerList.Count<=0) throw new ArgumentException("SelectionControlLayerList Parameter is invalid");

            _resultControlLayer = resultControlLayer;
            _selectionControlLayerList = selectionControlLayerList;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Result Control Layer
        /// </summary>
        public ControlLayer ResultControlLayer {
            get { return _resultControlLayer; }
        }
        /// <summary>
        /// Get Selection Control Layer List
        /// </summary>
        public List<ControlLayer> SelectionControlLayerList {
            get { return _selectionControlLayerList; }
        }
        #endregion

    }
}
