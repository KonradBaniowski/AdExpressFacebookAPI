using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.AdExpress.AdVolumeChecker.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;

namespace KMI.AdExpress.AdVolumeChecker.Domain {
    /// <summary>
    /// Media information Collection
    /// </summary>
    public class MediaInformations {

        #region Variables
        /// <summary>
        /// Media Information List
        /// </summary>
        private static List<MediaInformation> _mediaInformationList;
        #endregion

        #region Init
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="source">XML File</param>
        public static void Init(IDataSource source) {
            _mediaInformationList = MediaInformationsXL.Load(source);
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Form Information List
        /// </summary>
        public static List<MediaInformation> MediaInformationList {
            get { return (_mediaInformationList); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get Form Information By Form Id
        /// </summary>
        public MediaInformation GetMediaInformation(Int64 mediaId) {
            foreach (MediaInformation mediaInformation in _mediaInformationList)
                if (mediaInformation.Id == mediaId)
                    return mediaInformation;

            return null;
        }
        #endregion

    }
}
