using System;
using System.Collections.Generic;
using System.Text;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.Core.Sessions;
using AbsctractDAL=TNS.AdExpress.Portofolio.DAL;

namespace TNS.AdExpressI.Portofolio.DAL.Default {

    public class PortofolioDAL:AbsctractDAL.PortofolioDAL {
    
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleName">Vehicle name</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        public PortofolioDAL(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName,Int64 idMedia,string beginingDate,string endDate):base(webSession,vehicleName,idMedia,beginingDate,endDate) {

        }
        #endregion
    }
}
