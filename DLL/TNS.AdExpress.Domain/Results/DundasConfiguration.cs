using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Domain.Results {
    /// <summary>
    /// Dundas Configuration
    /// </summary>
    public class DundasConfiguration {

        #region Variables
        /// <summary>
        /// Virtual Path File Temporary
        /// </summary>
        protected string _imageURL = string.Empty;
        #endregion

        #region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
        public DundasConfiguration(IDataSource source)
        {
            XmlLoader.DundasConfigurationXL.Load(source, this);
		}
		#endregion

        #region internal Methods
        /// <summary>
        /// Init Image URL
        /// </summary>
        /// <param name="imageURL">Image URL</param>
        internal void InitImageURL(string imageURL) {
            if (imageURL == null) throw new ArgumentNullException("Parameter imageURL is null in function InitImageURL in class DundasConfiguration.");
            if (imageURL.Length < 1) throw new ArgumentException("Parameter imageURL is invalid in function InitImageURL in class DundasConfiguration.");
            _imageURL = imageURL;
        }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Virtual Path File Temporary
        /// </summary>
        public string ImageURL {
            get { return _imageURL; }
        }
        #endregion
    }
}
