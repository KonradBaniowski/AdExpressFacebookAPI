#region Information
// Author: Y. R'kaina
// Creation date: 29/08/2008
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core.Sessions;
using AbsctractDAL = TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpressI.Portofolio.DAL.Finland {

    public class PortofolioDAL : AbsctractDAL.PortofolioDAL {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="idMedia">Id media</param>
        public PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate)
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate) {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="adBreak">Screen code</param>
        public PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate, string adBreak)
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate, adBreak) {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="hour Beginning List">hour Beginning List</param>
        /// <param name="hourEndList">hour EndList</param>		
        public PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate, Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList)
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate, hourBeginningList, hourEndList) {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="ventilationTypeList">ventilation Type List</param>	
        public PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate, List<PortofolioStructure.Ventilation> ventilationTypeList)
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate, ventilationTypeList) {

        }
        #endregion

        #region Synthesis membres
        /// <summary>
        /// Get synthesis data
        /// </summary>
        /// <param name="synthesisDataType">Synthesis Data Type</param>
        /// <returns></returns>
        public override DataSet GetSynthisData(PortofolioSynthesis.dataType synthesisDataType) {
            Engines.SynthesisEngine res = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, synthesisDataType);
            return res.GetData();
        }
        #endregion
    }
}
