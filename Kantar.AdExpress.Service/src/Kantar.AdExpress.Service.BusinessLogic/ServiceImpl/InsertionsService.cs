using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kantar.AdExpress.Service.Core.Domain;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpressI.Insertions;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Translation;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstFlags = TNS.AdExpress.Constantes.DB.Flags;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class InsertionsService : IInsertionsService
    {
        private WebSession _customerWebSession = null;
        private int _fromDate;
        private int _toDate;
        private long? _idVehicle = long.MinValue;


        public InsertionResponse GetInsertionsGridResult(string idWebSession, string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle)
        {
            InsertionResponse insertionResponse = new InsertionResponse();
            try
            {

                IInsertionsResult insertionResult = InitInsertionCall(_customerWebSession, moduleId);

                insertionResponse.Vehicles = insertionResult.GetPresentVehicles(ids, idUnivers, false);
                if (insertionResponse.Vehicles.Count <= 0)
                {
                    return insertionResponse;
                }

                _customerWebSession = (WebSession)WebSession.Load(idWebSession);

                //TODO : TROUVER OU IL EST CHARGE
                if (idVehicle.HasValue)
                {
                    _idVehicle = idVehicle;
                }
                VehicleInformation vehicle = VehiclesInformation.Get(_idVehicle.Value);

              

                string message = string.Empty;
                if (vehicle.Id == CstDBClassif.Vehicles.names.outdoor &&
                       !_customerWebSession.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DETAIL_OUTDOOR_ACCESS_FLAG))
                {
                    insertionResponse.Message = GestionWeb.GetWebWord(1882, _customerWebSession.SiteLanguage);
                }
                else if (vehicle.Id == CstDBClassif.Vehicles.names.instore &&
                    !_customerWebSession.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DETAIL_INSTORE_ACCESS_FLAG))
                {
                    insertionResponse.Message = GestionWeb.GetWebWord(2668, _customerWebSession.SiteLanguage);
                }
                else if (vehicle.Id == CstDBClassif.Vehicles.names.indoor &&
                    !_customerWebSession.CustomerLogin.CustormerFlagAccess(CstFlags.ID_DETAIL_INDOOR_ACCESS_FLAG))
                {
                    insertionResponse.Message = GestionWeb.GetWebWord(2992, _customerWebSession.SiteLanguage);
                }

                if (string.IsNullOrEmpty(insertionResponse.Message)) return insertionResponse;

                if ((vehicle.Id == CstDBClassif.Vehicles.names.internet
                   || vehicle.Id == CstDBClassif.Vehicles.names.czinternet) && !insertionResult.CanShowInsertion(vehicle))
                {
                    insertionResponse.Message = GestionWeb.GetWebWord(2244, _customerWebSession.SiteLanguage);
                    return insertionResponse;
                }

                //date
                TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType = _customerWebSession.PeriodType;
                string periodBegin = _customerWebSession.PeriodBeginningDate;
                string periodEnd = _customerWebSession.PeriodEndDate;

                if (!string.IsNullOrEmpty(zoomDate))
                {
                    periodType = _customerWebSession.DetailPeriod == TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.weekly
                        ? TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDateWeek : TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDateMonth;
                    _fromDate = Convert.ToInt32(
                        Dates.Max(Dates.getZoomBeginningDate(zoomDate, periodType),
                            Dates.getPeriodBeginningDate(periodBegin, _customerWebSession.PeriodType)).ToString("yyyyMMdd")
                        );
                    _toDate = Convert.ToInt32(
                        Dates.Min(Dates.getZoomEndDate(zoomDate, periodType),
                            Dates.getPeriodEndDate(periodEnd, _customerWebSession.PeriodType)).ToString("yyyyMMdd")
                        );
                }
                else
                {
                    _fromDate = Convert.ToInt32(Dates.getZoomBeginningDate(periodBegin, periodType).ToString("yyyyMMdd"));
                    _toDate = Convert.ToInt32(Dates.getZoomEndDate(periodEnd, periodType).ToString("yyyyMMdd"));
                }

                if (_customerWebSession.GenericInsertionColumns.Columns.Count < 1)
                {
                    return insertionResponse;
                }

                //TODO: A GERER pour les exports Excel
                //if (this._renderType != RenderType.html)
                //{
                //    var columns = _customerWebSession.GenericInsertionColumns.Columns;
                //    var columnIds = (columns.Where(
                //        g =>
                //        g.Id != GenericColumnItemInformation.Columns.associatedFile &&
                //        g.Id != GenericColumnItemInformation.Columns.associatedFileMax &&
                //        g.Id != GenericColumnItemInformation.Columns.poster &&
                //        g.Id != GenericColumnItemInformation.Columns.visual).Select(g => g.Id.GetHashCode()))
                //        .Select(dummy => (long)dummy).ToList();
                //    _customerWebSession.GenericInsertionColumns = new GenericColumns(columnIds);

                //    result.RenderType = _renderType;
                //}

                  
                insertionResult.GetInsertionsGridResult(vehicle, _fromDate, _toDate, ids, idUnivers, zoomDate);
                insertionResponse.GridResult = insertionResult.GetInsertionsGridResult(vehicle, _fromDate, _toDate, ids, idUnivers, zoomDate);
               
            }
            catch (Exception)
            {

                insertionResponse.Message = GestionWeb.GetWebWord(959,_customerWebSession.SiteLanguage);
            }

            return insertionResponse;

        }


        public IInsertionsResult InitInsertionCall(WebSession custSession, long moduleId)
        {
            //**TODO : IdVehicules not null


            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.insertions];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the insertions rules"));
            var param = new object[2];
            param[0] = custSession;
            param[1] = moduleId;
            var result = (IInsertionsResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\"
                + cl.AssemblyName, cl.Class, false, System.Reflection.BindingFlags.CreateInstance
                | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null);


            return result;

        }


    }
}
