using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Web.Navigation
{
    public class RightMenuLinksItem
    {

        #region Variables
		/// <summary>
		/// Directory texte code
		/// </summary>
		protected Int64 _webTextId = 0;
		/// <summary>
		/// URL
		/// </summary>
		protected string _url = string.Empty;
        /// <summary>
        /// Use language
        /// </summary>
		protected bool _useLanguage = false;
        /// <summary>
        /// Use session ID
        /// </summary>
        protected bool _useSessionId = false;
        /// <summary>
        /// Display in popup
        /// </summary>
        protected bool _displayInPopUp = false;
        /// <summary>
        /// Target
        /// </summary>
        protected string _target = string.Empty;
        /// <summary>
        /// Icon name in CSS
        /// </summary>
        protected string _IconName = string.Empty;
        /// <summary>
        /// Name of javascript function
        /// </summary>
        protected string _javascriptFunctionName = string.Empty;
        /// <summary>
        /// PopUp width
        /// </summary>
        protected string _width = string.Empty;
        /// <summary>
        /// PopUp height
        /// </summary>
        protected string _height = string.Empty;
		#endregion

		#region Accessors
		/// <summary>
		/// Get directory texte code
		/// </summary>
		public Int64 WebTextId {
			get { return _webTextId; }
		}
        /// <summary>
        /// Get URL
        /// </summary>
        public string Url
        {
            get { return _url; }
        }
        /// <summary>
        /// Get Use language
        /// </summary>
        public bool UseLanguage
        {
            get { return _useLanguage;  }
        }
        /// <summary>
        /// Get Use session ID
        /// </summary>
        public bool UseSessionId
        {
            get { return _useSessionId; }
        }
        /// <summary>
        /// Get Display in popup
        /// </summary>
        public bool DisplayInPopUp
        {
            get { return _displayInPopUp; }
        }
        /// <summary>
        /// Get Target
        /// </summary>
        public string Target
        {
            get { return _target; }
        }
        /// <summary>
        /// Get Icon name in CSS
        /// </summary>
        public string IconName
        {
            get { return _IconName; }
        }
        /// <summary>
        /// Get Name of javascript function
        /// </summary>
        public string JavascriptFunctionName
        {
            get { return _javascriptFunctionName; }
        }
        /// <summary>
        /// Get PopUp width
        /// </summary>
        public string WidthPopUp
        {
            get { return _width; }
        }
        /// <summary>
        /// Get PopUp height
        /// </summary>
        public string HeightPopUp
        {
            get { return _height; }
        }
		#endregion

		#region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webTextId">webTextId</param>
        /// <param name="url">Url</param>
        /// <param name="useLanguage">Use language</param>
        /// <param name="useSessionId">Use session ID</param>
        /// <param name="displayInPopUp">Display in popup</param>
        /// <param name="target">Target</param>
        /// <param name="cssIconName">Icon name in CSS</param>
        /// <param name="javascriptFunctionName">Name of javascript function</param>
        /// <param name="width">PopUp width</param>
        /// <param name="height">PopUp height</param>
        public RightMenuLinksItem(Int64 webTextId, string url, bool useLanguage, bool useSessionId
            , bool displayInPopUp, string target, string cssIconName, string javascriptFunctionName
            , string width, string height)
        {
			if(webTextId < 0) throw (new ArgumentException("Invalid argument webTextId"));
            if(url == null) throw (new ArgumentNullException("Url argument is null"));
            if(url.Length < 0) throw (new ArgumentException("Url argument webTextId"));

			_webTextId = webTextId;
            _url = url;
            _useLanguage = useLanguage;
            _useSessionId = useSessionId;
            _displayInPopUp = displayInPopUp;
            _target = target;

            _IconName = cssIconName;
            _javascriptFunctionName = javascriptFunctionName;
            _width = width;
            _height = height;
		}
		#endregion

    }
}
