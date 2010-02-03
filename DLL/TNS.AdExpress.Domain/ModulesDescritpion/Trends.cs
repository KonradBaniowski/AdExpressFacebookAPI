using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.AdExpress.Constantes.Classification.DB;

namespace TNS.AdExpress.Domain.ModulesDescritpion
{
    public class Trends
    {

        protected Dictionary<Vehicles.names, TrendsDateOpeningOption> _trendsDateOpeningOptions = new Dictionary<Vehicles.names, TrendsDateOpeningOption>();

        #region Constructor		
        /// <summary>
        /// Constructor
        /// </summary>
        public Trends(IDataSource source)
        {
            TrendsXL.LoadTrends(source, _trendsDateOpeningOptions);
		}
		#endregion

        /// <summary>
        /// Get Infos news items 
        /// </summary>
        public Dictionary<Vehicles.names, TrendsDateOpeningOption> TrendsDateOpeningOptions
        {
            get { return _trendsDateOpeningOptions; }
        }

    }
}
