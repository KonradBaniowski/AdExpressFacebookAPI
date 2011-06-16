using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.VP.Loader.Domain.XmlLoader;
using System.IO;

namespace TNS.AdExpress.VP.Loader.Domain {
    public class Parameters {

        #region Variables
        /// <summary>
        /// Visual Promotion Path Out
        /// </summary>
        string _visualPromotionPathOut = null;
        /// <summary>
        /// Visual Condition Path Out
        /// </summary>
        string _visualConditionPathOut = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor public
        /// </summary>
        /// <param name="pathOut">Path Out</param>
        public Parameters(string visualPromotionPathOut, string visualConditionPathOut) {
            _visualPromotionPathOut = visualPromotionPathOut;
            _visualConditionPathOut = visualConditionPathOut;
        }
        /// <summary>
        /// Constructor for serialize object
        /// </summary>
        public Parameters() { }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Visual Promotion Path Out
        /// </summary>
        public string VisualPromotionPathOut {
            get { return _visualPromotionPathOut; }
        }
        /// <summary>
        /// Get Visual Condition Path Out
        /// </summary>
        public string VisualConditionPathOut {
            get { return _visualConditionPathOut; }
        }
        #endregion

    }
}
