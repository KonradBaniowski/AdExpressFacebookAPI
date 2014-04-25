using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.AdExpress.AdVolumeChecker.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;

namespace KMI.AdExpress.AdVolumeChecker.Domain {
    /// <summary>
    /// /// <summary>
    /// Filter information Collection
    /// </summary>
    /// </summary>
    public class FilterInformations {

        #region Variables
        /// <summary>
        /// Filter Information List
        /// </summary>
        private static List<FilterInformation> _filterInformationList;
        #endregion

        #region Init
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="source">XML File</param>
        public static void Init(IDataSource source) {
            _filterInformationList = FilterInformationsXL.Load(source);
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Filter Information List
        /// </summary>
        public static List<FilterInformation> FilterInformationList {
            get { return (_filterInformationList); }
        }
        #endregion

    }
}
