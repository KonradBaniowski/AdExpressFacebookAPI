using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Domain.Theme {
    /// <summary>
    /// Style Object contains a list of Tag
    /// </summary>
    public class Style {

        #region Variables
        /// <summary>
        /// List of property name which content skin (name of fontPdf, name of  picture, name of styleCell...)
        /// </summary>
        private Dictionary<string,Tag> _tagList = new Dictionary<string,Tag>();
        #endregion

        #region Assessor
        /// <summary>
        /// Get List of property name which content skin (name of fontPdf, name of  picture, name of styleCell...)
        /// </summary>
        public Dictionary<string, Tag> TagList {
            get { return _tagList; }
        }
        #endregion

        #region Constructor
        public Style(Dictionary<string, Tag> tagList) {
            if (tagList == null) throw new ArgumentNullException("Tag List Parameter is null ! (in Construtor of Style)");
            _tagList = tagList;
        }
        #endregion

    }
}
