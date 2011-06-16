using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.VP.Loader.Domain.XmlLoader;
using System.IO;

namespace TNS.AdExpress.VP.Loader.Domain {
    public class PictureMatching {

        #region Variables
        /// <summary>
        /// Path In
        /// </summary>
        string _pathIn = null;
        /// <summary>
        /// Path Out
        /// </summary>
        string _pathOut = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor public
        /// </summary>
        /// <param name="pathIn">Path In</param>
        /// <param name="pathOut">Path Out</param>
        public PictureMatching(string pathIn, string pathOut) {
            _pathIn = pathIn;
            _pathOut = pathOut;
        }
        /// <summary>
        /// Constructor for serialize object
        /// </summary>
        public PictureMatching() { }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Path IN
        /// </summary>
        public string PathIn {
            get { return _pathIn; }
        }
        /// <summary>
        /// Get Path Out
        /// </summary>
        public string PathOut {
            get { return _pathOut; }
        }
        #endregion

    }
}
