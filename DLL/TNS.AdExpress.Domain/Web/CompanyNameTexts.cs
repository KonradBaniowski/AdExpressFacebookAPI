#region Informations
// Author: Y.R'kaina
// Creation Date: 05/01/2011
// Modifications: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Domain.Web
{
    /// <summary>
    /// Company name texts management
    /// </summary>
    public class CompanyNameTexts
    {

        #region Variables
        /// <summary>
        /// Company Name code
        /// </summary>
        private int _companyNameCode = 0;
        /// <summary>
        /// Short Company Name code
        /// </summary>
        private int _companyShortNameCode = 0;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CompanyNameTexts()
        {
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Company Name Code
        /// </summary>
        public int CompanyNameCode
        {
            get { return (_companyNameCode); }
            set { _companyNameCode = value; }
        }
        /// <summary>
        /// Get Short Company Name Code
        /// </summary>
        public int CompanyShortNameCode
        {
            get { return (_companyShortNameCode); }
            set { _companyShortNameCode = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get Company Name (KANTAR MEDIA, TNS MEDIA Intelligence)
        /// </summary>
        public string GetCompanyName(int siteLanguage)
        {
            return GestionWeb.GetWebWord(_companyNameCode, siteLanguage);
        }
        /// <summary>
        /// Get Short Company Name (KANTAR, TNS)
        /// </summary>
        public string GetCompanyShortName(int siteLanguage)
        {
            return GestionWeb.GetWebWord(_companyShortNameCode, siteLanguage);
        }
        #endregion

    }
}
