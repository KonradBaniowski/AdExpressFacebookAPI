using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.VP.Loader.Domain.XmlLoader;
using System.IO;

namespace TNS.AdExpress.VP.Loader.Domain {
    public class CommonAppData {

        #region Variables
        /// <summary>
        /// Source File
        /// </summary>
        string _sourceFile = null;
        /// <summary>
        /// File Load
        /// </summary>
        FileInfo _fileLoad = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor public
        /// </summary>
        /// <param name="pathFile">Path File Save</param>
        public CommonAppData(string pathFileSave) {
            if (pathFileSave == null) throw new ArgumentNullException("pathFileSave parameter is null");
            if (pathFileSave.Length <= 0) throw new ArgumentException("pathFileSave parameter is invalid");
            if (!Directory.Exists(Path.GetDirectoryName(pathFileSave))) Directory.CreateDirectory(Path.GetDirectoryName(pathFileSave));
            if (!File.Exists(pathFileSave)) {
                CommonAppDataXL.Create(pathFileSave, string.Empty);
            }
            _sourceFile = pathFileSave;
            _fileLoad = CommonAppDataXL.Load(_sourceFile);
        }
        /// <summary>
        /// Constructor for serialize object
        /// </summary>
        public CommonAppData() { }
        #endregion

        #region Assessor
        /// <summary>
        /// Get / Set Path FIle Load
        /// </summary>
        public FileInfo PathFile {
            get { return _fileLoad; }
            set { _fileLoad = value; SavePath(); }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Save Path
        /// </summary>
        private void SavePath() {
            CommonAppDataXL.Save(_sourceFile, _fileLoad);
        }
        #endregion
    }
}
