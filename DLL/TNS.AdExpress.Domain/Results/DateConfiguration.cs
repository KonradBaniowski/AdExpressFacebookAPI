using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.Layers;

namespace TNS.AdExpress.Domain.Results {
    
    public class DateConfiguration {

        #region Variables
        /// <summary>
        /// Date Type
        /// </summary>
        protected TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type _dateType;
        /// <summary>
        /// Text Id
        /// </summary>
        protected Int64 _textId;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dateType">Date Type</param>
        /// <param name="textId">Text Id</param>
        public DateConfiguration(TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type dateType, Int64 textId) {
            _dateType = dateType;
            _textId = textId;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Date Type
        /// </summary>
        public TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type DateType {
            get { return _dateType; }
        }
        /// <summary>
        /// Get Text Id
        /// </summary>
        public Int64 TextId {
            get { return _textId; }
        }
        #endregion

    }
}
