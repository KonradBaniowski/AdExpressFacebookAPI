using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Domain.Results
{
    public class VpConfigurationDetail {

        #region Variables
        /// <summary>
        /// Result Control Layer List
        /// </summary>
        protected List<ControlLayer> _resultControlLayerList;
        /// <summary>
        /// Selection Control Layer List
        /// </summary>
        protected List<ControlLayer> _selectionControlLayerList;

        protected DetailLevelItemInformation.Levels _defaultPersoLevel;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultControlLayerList">Result Control Layer List</param>
        /// <param name="selectionControlLayerList">Selection Control Layer List</param>
        public VpConfigurationDetail(List<ControlLayer> resultControlLayerList, List<ControlLayer> selectionControlLayerList,DetailLevelItemInformation.Levels defaultPersoLevel) {
            if (resultControlLayerList == null) throw new ArgumentNullException("resultControlLayerList Parameter is null");
            if (selectionControlLayerList.Count <= 0) throw new ArgumentException("resultControlLayerList Parameter is invalid");
            if (selectionControlLayerList == null) throw new ArgumentNullException("SelectionControlLayerList Parameter is null");
            if (selectionControlLayerList.Count<=0) throw new ArgumentException("SelectionControlLayerList Parameter is invalid");

            _resultControlLayerList = resultControlLayerList;
            _selectionControlLayerList = selectionControlLayerList;
            _defaultPersoLevel = defaultPersoLevel;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Result Control LayerList
        /// </summary>
        public List<ControlLayer> ResultControlLayerList {
            get { return _resultControlLayerList; }
        }
        /// <summary>
        /// Get Selection Control Layer List
        /// </summary>
        public List<ControlLayer> SelectionControlLayerList {
            get { return _selectionControlLayerList; }
        }
        /// <summary>
        /// Get default Perso Level
        /// </summary>
        public DetailLevelItemInformation.Levels DefaultPersoLevel
        {
            get { return _defaultPersoLevel; }
        }
        #endregion

    }
}
