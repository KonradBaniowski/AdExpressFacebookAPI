using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.AdVolumeChecker.Domain.DataModel {
    /// <summary>
    /// Classification Level Item
    /// </summary>
    public class ClassificationLevelItem {

        #region Accessors
        /// <summary>
        /// Get/Set Classification Level Item Name
        /// </summary>
        public string ClassificationLevelItemName { get; private set; }
        #endregion

        #region constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="classificationLevelItemName">Classification Level Item Name</param>
        public ClassificationLevelItem(string classificationLevelItemName) {
            this.ClassificationLevelItemName = classificationLevelItemName;
        }
        #endregion
        
    }
}
