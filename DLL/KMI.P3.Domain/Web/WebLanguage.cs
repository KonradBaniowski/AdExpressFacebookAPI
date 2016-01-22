using System;
using System.Collections.Generic;
using System.Text;
using KMI.P3.Domain.Translation;

namespace KMI.P3.Domain.Web
{
    /// <summary>
    /// Web site Language configuration
    /// </summary>
    public class WebLanguage
    {

        #region Variables
        /// <summary>
        /// Language Id
        /// </summary>
        /// <example>France = 33</example>
        private int _id;
        /// <summary>
        /// Language name
        /// </summary>
        private string _name = "";
        /// <summary>
        /// Language Image
        /// It can be used in a WebControl to select the language
        /// For example it can be the contry name or the flag...
        /// </summary>
        private string _imageSourceText = "";
        /// <summary>
        /// Localisation text string id
        /// </summary>
        /// <example>France = "fr"</example>
        private string _localization;      
        /// <summary>
        /// Charset used for the language
        /// By default, the charset is set to iso-8859-1
        /// </summary>
        /// <example>France = "iso-8859-1"</example>
        private string _charset = "iso-8859-1";
        /// <summary>
        /// Content Encoding used for aspx page
        /// By default, the content encoding is set to utf-8
        /// </summary>
        /// <example>France = "utf-8"</example>
        private string _contentEncoding = "utf-8";       
        /// <summary>
        /// NLS SORT to use any linguistic sort for an ORDER BY clause
        /// <example> France ="FRENCH"</example>
        /// </summary>
        private string _nlsSort = "";
        /// <summary>
        /// Extended culture info for the specific language
        /// </summary>
        private P3CultureInfo _cInfo = null;
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
        public WebLanguage(int id, string imageSourceText, string localization, string charset, string contentEncoding)
        {
            if (id < 0) throw (new ArgumentException("The language Id cannot be inferior to 0"));
            _id = id;
            if (imageSourceText != null) _imageSourceText = imageSourceText;
            if (localization == null) throw (new ArgumentNullException("The localization parameter cannot be null"));
            if (localization.Length == 0) throw (new ArgumentException("Invalid localization parameter"));
            _localization = localization;
            if (charset != null && charset.Length != 0) _charset = charset;
            if (contentEncoding != null && contentEncoding.Length != 0) _contentEncoding = contentEncoding;
            _name = "Country name is not defined:" + _id.ToString();          

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
        public WebLanguage(int id, string name, string imageSourceText, string localization,  string charset, string contentEncoding, string nlsSort,P3CultureInfo cInfo)
            : this(id, imageSourceText, localization, charset, contentEncoding)
        {
            if (name != null && name.Length > 0) _name = name;
            if (nlsSort != null && nlsSort.Length > 0) _nlsSort = nlsSort;          
            _cInfo = cInfo;           
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Language Id
        /// </summary>
        public int Id
        {
            get { return (_id); }
        }
        /// <summary>
        /// Get  Language name
        /// </summary>
        public string Name
        {
            get { return (_name); }
        }
        /// <summary>
        /// Get Language Image
        /// </summary>
        public string ImageSourceText
        {
            get { return (_imageSourceText); }
        }
        /// <summary>
        /// Get Localisation text string id
        /// </summary>
        public string Localization
        {
            get { return (_localization); }
        }      
        /// <summary>
        /// Get Charset used for the language
        /// </summary>
        public string Charset
        {
            get { return (_charset); }
        }
        /// <summary>
        /// Get Content Encoding used for the aspx page
        /// </summary>
        public string ContentEncoding
        {
            get { return (_contentEncoding); }
        }       
       
        /// <summary>
        /// Get NLS SORT to use any linguistic sort for an ORDER BY clause
        /// </summary>
        public string NlsSort
        {
            get { return (_nlsSort); }
        }
        /// <summary>
        /// Get Set P3 Culture Info for this language
        /// </summary>
        public P3CultureInfo CultureInfo
        {
            get { return _cInfo; }
            set { _cInfo = value; }
        }
        /// <summary>
       
       
        #endregion

        
    }
}
