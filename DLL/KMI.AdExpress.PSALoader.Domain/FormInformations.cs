using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.PSALoader.Domain {
    /// <summary>
    /// This class contains a list of Form information
    /// </summary>
    public class FormInformations {

        #region Variables
        /// <summary>
        /// Load Date
        /// </summary>
        private string _loadDate;
        /// <summary>
        /// Form Information List
        /// </summary>
        private List<FormInformation> _formInformationList;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="formInformationList">Form Information List</param>
        /// <param name="loadDate">Load Date</param>
        public FormInformations(List<FormInformation> formInformationList, string loadDate) {
            _formInformationList = formInformationList;
            _loadDate = loadDate;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Load Date
        /// </summary>
        public string LoadDate {
            get {
                string[] dateElements = _loadDate.Split('/');
                return (DateTime.Now.Year.ToString().Substring(0, 2) + dateElements[2] + dateElements[1] ); 
                //return (dateElements[0] + dateElements[1] + DateTime.Now.Year.ToString().Substring(0,2) + dateElements[2]); 
            }
        }
        /// <summary>
        /// Get Form Information List
        /// </summary>
        public List<FormInformation> FormInformationList {
            get { return (_formInformationList); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get Form Information By Form Id
        /// </summary>
        public FormInformation getFormInformation(Int64 formId) {
            foreach (FormInformation formInformation in _formInformationList)
                if (formInformation.FormId == formId)
                    return formInformation;

            return null;
        }
        #endregion

    }
}
