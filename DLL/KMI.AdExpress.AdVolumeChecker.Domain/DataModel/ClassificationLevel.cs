using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.AdVolumeChecker.Domain.DataModel {
    /// <summary>
    /// Classification Level
    /// </summary>
    public class ClassificationLevel {

        #region Variables
        /// <summary>
        /// Classification Level Items
        /// </summary>
        readonly List<ClassificationLevelItem> _classificationLevelItems = new List<ClassificationLevelItem>();
        #endregion

        #region Accessors
        /// <summary>
        /// Get/Set Classification Level Name
        /// </summary>
        public string classificationLevelName { get; private set; }
        /// <summary>
        /// Get Classification Level Items
        /// </summary>
        public List<ClassificationLevelItem> ClassificationLevelItems {
            get { return _classificationLevelItems; }
        }
        #endregion

        #region Contructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="classificationLevelName">Classification Level Name</param>
        public ClassificationLevel(string classificationLevelName) {
            this.classificationLevelName = classificationLevelName;
        }
        #endregion


    }
}
