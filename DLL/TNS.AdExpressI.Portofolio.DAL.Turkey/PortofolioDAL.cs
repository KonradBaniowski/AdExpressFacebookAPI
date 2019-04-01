using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Portofolio.DAL.Engines;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using AbsctractDAL = TNS.AdExpressI.Portofolio.DAL;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;

namespace TNS.AdExpressI.Portofolio.DAL.Turkey
{
    public class PortofolioDAL : AbsctractDAL.PortofolioDAL
    {
        #region Variables
        /// <summary>
        /// Time Slot
        /// </summary>
        protected string _timeSlot;
        #endregion

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
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="timeSlot">Time Slot</param>
        public PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate, string timeSlot)
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate)
        {
            _timeSlot = timeSlot;
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
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate, hourBeginningList, hourEndList) { }

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
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate, ventilationTypeList) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="level">Level</param>
        public PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate, DetailLevelItemInformation level)
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate, level) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public PortofolioDAL() { }
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
                            res = new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
                            break;
                        default:
                            throw (new PortofolioDALException("Impossible to identified current vehicle "));
                    }
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
                    res = GetStructData();
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.PROGRAM_TYPOLOGY_BREAKDOWN:
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.PROGRAM_BREAKDOWN:
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SUBTYPE_SPOTS_BREAKDOWN:
                    res = new Engines.BreakdownEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, _level);
                    break;
                default:
                    throw (new PortofolioDALException("Impossible to identified current tab "));
            }

            return res.GetData();
        }
        public override long CountData()
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
                            res = new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
                            break;
                        default:
                            throw (new PortofolioDALException("Impossible to identified current vehicle "));
                    }
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
                    res = GetStructData();
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.PROGRAM_TYPOLOGY_BREAKDOWN:
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.PROGRAM_BREAKDOWN:
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SUBTYPE_SPOTS_BREAKDOWN:
                    res = new Engines.BreakdownEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, _level);
                    break;
                default:
                    throw (new PortofolioDALException("Impossible to identified current tab "));
            }

            return res.CountData();
        }
        #endregion

        #region Synthesis membres
        /// <summary>
        /// Get synthesis data
        /// </summary>
        /// <param name="synthesisDataType">Synthesis Data Type</param>
        /// <returns></returns>
        public override DataSet GetSynthisData(PortofolioSynthesis.dataType synthesisDataType)
        {
            var res = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, synthesisDataType);
            return res.GetData();
        }
        #endregion

        /// <summary>
        /// récupère les écrans
        /// </summary>
        /// <returns>Ecrans</returns>
        public override DataSet GetCustomEcranData()
        {
            var res = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
            return res.GetCustomEcranData();
        }

        /// <summary>
        /// Get Inssertion data
        /// </summary>
        /// <returns></returns>
        public override DataSet GetInsertionData()
        {
            var res = new Engines.InsertionDetailEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, _timeSlot);
            return res.GetData();
        }

        #region Get Struct Data
        /// <summary>
        /// Get structure data 
        /// </summary>
        /// <remarks>Used for tv or radio</remarks>
        /// <returns>DataSet</returns>
        protected override StructureEngine GetStructData()
        {
            Engines.StructureEngine res = null;

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
                    res = new Engines.StructureEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, _hourBeginningList, _hourEndList);
                    break;
                default:
                    throw (new PortofolioDALException("Impossible to identified current vehicle "));
            }
            return res;
        }
        #endregion
    }
}
