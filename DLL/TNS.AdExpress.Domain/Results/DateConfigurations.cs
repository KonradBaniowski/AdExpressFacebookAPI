using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.Layers;

namespace TNS.AdExpress.Domain.Results {

    public class DateConfigurations {

        #region Variables
        /// <summary>
        /// Vp Date Configuration Type
        /// </summary>
        protected TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type _dateTypeDefault;
        /// <summary>
        /// Vp Date Configuration List
        /// </summary>
        protected List<DateConfiguration> _vpDateConfigurationList;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dateTypeDefault">Vp Date Configuration Type</param>
        /// <param name="vpDateConfigurationList">Vp Date Configuration List</param>
        public DateConfigurations(TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type dateTypeDefault, List<DateConfiguration> vpDateConfigurationList) {
            _dateTypeDefault = dateTypeDefault;
            _vpDateConfigurationList = vpDateConfigurationList;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Date Type Default
        /// </summary>
        public TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type DateTypeDefault {
            get { return _dateTypeDefault; }
        }
        /// <summary>
        /// Get Vp Date Configuration List
        /// </summary>
        public List<DateConfiguration> VpDateConfigurationList {
            get { return _vpDateConfigurationList; }
        }
        #endregion

    }
}
