#region Informations
// Author: G. Facon
// Creation Date: 21/02/2008
// Modifications: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Web.Core {
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
        private Int64 _classificationLanguageId;
        /// <summary>
        /// Charset used for the language
        /// By default, the charset is set to iso-8859-1
        /// </summary>
        /// <example>France = "iso-8859-1"</example>
        private string _charset="iso-8859-1";

        #endregion

        #region Constructor
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="id">Language Id</param>
        /// <param name="imageSourceText">Language Image</param>
        /// <param name="localization">Localisation text string id</param>
        /// <param name="charset">Charset used for the language</param>
        public WebLanguage(int id,string imageSourceText,string localization,string charset) {
            if(id<0) throw (new ArgumentException("The language Id cannot be inferior to 0"));
            _id=id;
            if(imageSourceText!=null) _imageSourceText=imageSourceText;
            if(localization==null) throw (new ArgumentNullException("The localization parameter cannot be null"));
            if(localization.Length==0) throw (new ArgumentException("Invalid localization parameter"));
            _localization=localization;
            if(charset!=null && charset.Length!=0) _charset=charset;
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
        public WebLanguage(int id,string name,string imageSourceText,string localization,Int64 classificationLanguageId,string charset)
            : this(id,imageSourceText,localization,charset) {
            if(name!=null&&name.Length>0) _name=name;
            if(classificationLanguageId<0) throw (new ArgumentException("The classification language Id cannot be inferior to 0"));
            _classificationLanguageId=classificationLanguageId;
            
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
        public Int64 ClassificationLanguageId {
            get { return (_classificationLanguageId); }
        }
        /// <summary>
        /// Get Charset used for the language
        /// </summary>
        public string Charset {
            get { return (_charset); }
        }
        #endregion
    }
}
