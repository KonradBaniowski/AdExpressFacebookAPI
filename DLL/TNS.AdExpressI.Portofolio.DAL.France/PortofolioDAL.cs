#region Information
// Author: Y. R'kaina
// Creation date: 29/08/2008
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections.Generic;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core.Sessions;
using AbsctractDAL = TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpressI.Portofolio.DAL.France {

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
        /// <param name="hourBeginningList">hour Beginning List</param>
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
        #region IPortofolioDAL Membres

        /// <summary>
        /// Get synthesis data
        /// </summary>
        /// <returns></returns>
        public override DataSet GetData()
        {
            DAL.Engines.Engine res = null;

            switch (_webSession.CurrentTab)
            {
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
                    res = new Engines.PortofolioDetailEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
                    res = new Engines.CalendarEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:
                    switch (_vehicleInformation.Id)
                    {
                        case DBClassificationConstantes.Vehicles.names.others:
                        case DBClassificationConstantes.Vehicles.names.tv:
                        case DBClassificationConstantes.Vehicles.names.tvGeneral:
                        case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                        case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                        case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                        case DBClassificationConstantes.Vehicles.names.radio:
                        case DBClassificationConstantes.Vehicles.names.radioGeneral:
                        case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                        case DBClassificationConstantes.Vehicles.names.radioMusic:
                            res = new DAL.Engines.MediaDetailEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
                            break;
                        default:
                            throw (new PortofolioDALException("Impossible to identified current vehicle "));
                    }
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
                    res = GetStructData();
                    break;
                default:
                    throw (new PortofolioDALException("Impossible to identified current tab "));
            }

            return res.GetData();
        }
        #endregion

        #region Synthesis membres
        /// <summary>
        /// Get synthesis data
        /// </summary>
        /// <param name="synthesisDataType">Synthesis Data Type</param>
        /// <returns></returns>
        public override DataSet GetSynthisData(PortofolioSynthesis.dataType synthesisDataType) {
            var res = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, synthesisDataType);
            return res.GetData();
        }
        #endregion
    }
}
