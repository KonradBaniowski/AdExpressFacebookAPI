#region Information
//  Author : Y. R'kaina
//  Creation  date: 06/08/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.WebService.Domain.Configuration {
    /// <summary>
    /// Creative Information
    /// </summary>
    public class CreativeInformation {

        #region Variables
        /// <summary>
        /// Path
        /// </summary>
        private string _path;
        /// <summary>
        /// Cover Path
        /// </summary>
        private string _coverPath;
        /// <summary>
        /// Impersonate Information
        /// </summary>
        private ImpersonateInformation _impersonateInformation = null;
        /// <summary>
        /// Impersonation
        /// </summary>
        private Impersonation _oImp = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path</param>
        public CreativeInformation(string path) {
            _path = path;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="coverPath">Cover path </param>
        public CreativeInformation(string path,string coverPath)
        {
            _path = path;
            _coverPath = coverPath;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path</param>
        public CreativeInformation(string path, ImpersonateInformation impersonateInformation):this(path) {
            _impersonateInformation = impersonateInformation;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Path
        /// </summary>
        public string Path {
            get { return _path; }
        }
        /// <summary>
        /// Get Impersonate Information
        /// </summary>
        public ImpersonateInformation ImpersonateInfo {
            get { return _impersonateInformation; }
        }

        /// <summary>
        /// Cover Path
        /// </summary>
        public string CoverPath
        {
            get { return _coverPath; }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Open Impersonation
        /// </summary>
        /// <returns></returns>
        public void Open() {
            if (_impersonateInformation != null) {
                Close();
                _oImp = new Impersonation();
                _oImp.ImpersonateValidUser(_impersonateInformation.UserName, _impersonateInformation.Domain, _impersonateInformation.Password, Impersonation.LogonType.LOGON32_LOGON_NEW_CREDENTIALS);
            }
        }
        /// <summary>
        /// Close Impersonation
        /// </summary>
        public void Close() {
            if (_oImp != null)
                _oImp.UndoImpersonation();
            _oImp = null;
        }
        #endregion

    }
}
