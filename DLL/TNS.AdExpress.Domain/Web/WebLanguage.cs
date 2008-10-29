#region Informations
// Author: G. Facon
// Creation Date: 21/02/2008
// Modifications: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Domain.Web {
    /// <summary>
    /// Web site Language configuration
    /// </summary>
    public class WebLanguage {

        #region Variables
        /// <summary>
        /// Language Id
        /// </summary>
        /// <example>France = 33</example>
        private int _id;
        /// <summary>
        /// Language name
        /// </summary>
        private string _name="";
        /// <summary>
        /// Language Image
        /// It can be used in a WebControl to select the language
        /// For example it can be the contry name or the flag...
        /// </summary>
        private string _imageSourceText="";
        /// <summary>
        /// Localisation text string id
        /// </summary>
        /// <example>France = "fr"</example>
        private string _localization;
        /// <summary>
        /// Classification language id
        /// By default, if it is not configured the constructor sets it to the language Id
        /// </summary>
        private int _classificationLanguageId;
        /// <summary>
        /// Charset used for the language
        /// By default, the charset is set to iso-8859-1
        /// </summary>
        /// <example>France = "iso-8859-1"</example>
        private string _charset="iso-8859-1";
        /// <summary>
        /// Content Encoding used for aspx page
        /// By default, the content encoding is set to utf-8
        /// </summary>
        /// <example>France = "utf-8"</example>
        private string _contentEncoding = "utf-8";
        /// <summary>
        /// Content Encoding used for excel aspx page
        /// By default, the content encoding is set to utf-8
        /// </summary>
        /// <example>France = "utf-8"</example>
        private string _excelContentEncoding = "utf-8";
		/// <summary>
		/// NLS SORT to use any linguistic sort for an ORDER BY clause
		/// <example> France ="FRENCH"</example>
		/// </summary>
		private string _nlsSort="";
        /// <summary>
        /// Extended culture info for the specific language
        /// </summary>
        private AdExpressCultureInfo _cInfo = null;
        /// <summary>
        /// Rss Description
        /// </summary>
        private Rss _rss;
        #endregion

        #region Constructor
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="id">Language Id</param>
        /// <param name="imageSourceText">Language Image</param>
        /// <param name="localization">Localisation text string id</param>
        /// <param name="charset">Charset used for the language</param>
        /// <param name="contentEncoding">Content Encoding used for the aspx page</param>
        /// <param name="excelContentEncoding">Content Encoding used for the excel aspx page</param>
        /// <param name="cInfo">Extended culture info</param>
        public WebLanguage(int id, string imageSourceText, string localization, string charset, string contentEncoding, string excelContentEncoding) {
            if(id<0) throw (new ArgumentException("The language Id cannot be inferior to 0"));
            _id=id;
            if(imageSourceText!=null) _imageSourceText=imageSourceText;
            if(localization==null) throw (new ArgumentNullException("The localization parameter cannot be null"));
            if(localization.Length==0) throw (new ArgumentException("Invalid localization parameter"));
            _localization=localization;
            if(charset!=null && charset.Length!=0) _charset=charset;
            if (contentEncoding != null && contentEncoding.Length != 0) _contentEncoding = contentEncoding;
            if (ExcelContentEncoding != null && ExcelContentEncoding.Length != 0) _excelContentEncoding = excelContentEncoding;
            _name="Country name is not defined:"+_id.ToString();
            _classificationLanguageId=_id;

        }
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="id">Language Id</param>
        /// <param name="name">Language name</param>
        /// <param name="imageSourceText">Language Image</param>
        /// <param name="localization">Localisation text string id</param>
        /// <param name="classificationLanguageId">Classification language Id</param>
        /// <param name="charset">Charset used for the language</param>
        /// <param name="contentEncoding">Content Encoding used for the aspx page</param>
        /// <param name="excelContentEncoding">Content Encoding used for the excel aspx page</param>
        /// <param name="nlsSort">nls sort</param>
        /// <param name="cInfo">Culture info object</param>
        /// <param name="rss">Rss object</param>
        public WebLanguage(int id, string name, string imageSourceText, string localization, int classificationLanguageId, string charset, string contentEncoding, string excelContentEncoding, string nlsSort, AdExpressCultureInfo cInfo, Rss rss)
            : this(id,imageSourceText,localization,charset,contentEncoding,excelContentEncoding) {
            if(name!=null&&name.Length>0) _name=name;
			if (nlsSort != null && nlsSort.Length > 0) _nlsSort = nlsSort;		
            if(classificationLanguageId<0) throw (new ArgumentException("The classification language Id cannot be inferior to 0"));
            _classificationLanguageId=classificationLanguageId;
            _cInfo = cInfo;
            _rss = rss;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Language Id
        /// </summary>
        public int Id {
            get {return (_id);}
        }
        /// <summary>
        /// Get  Language name
        /// </summary>
        public string Name {
            get { return (_name); }
        } 
        /// <summary>
        /// Get Language Image
        /// </summary>
        public string ImageSourceText {
            get { return (_imageSourceText); }
        }
        /// <summary>
        /// Get Localisation text string id
        /// </summary>
        public string Localization {
            get { return (_localization); }
        }
        /// <summary>
        /// Get Classification language id
        /// </summary>
        public int ClassificationLanguageId {
            get { return (_classificationLanguageId); }
        }
        /// <summary>
        /// Get Charset used for the language
        /// </summary>
        public string Charset {
            get { return (_charset); }
        }
        /// <summary>
        /// Get Content Encoding used for the aspx page
        /// </summary>
        public string ContentEncoding {
            get { return (_contentEncoding); }
        }
        /// <summary>
        /// Get Content Encoding used for the excel aspx page
        /// </summary>
        public string ExcelContentEncoding {
            get { return (_excelContentEncoding); }
        }
		/// <summary>
		/// Get NLS SORT to use any linguistic sort for an ORDER BY clause
		/// </summary>
        public string NlsSort
        {
            get { return (_nlsSort); }
        }
        /// <summary>
        /// Get Set AdExpress Culture Info for this language
        /// </summary>
        public AdExpressCultureInfo CultureInfo
        {
            get { return _cInfo; }
            set { _cInfo = value; }
        }
        /// <summary>
        /// Get Set rss feed
        /// </summary>
        public Rss Rss {
            get { return _rss; }
        }
        #endregion
    }
}
