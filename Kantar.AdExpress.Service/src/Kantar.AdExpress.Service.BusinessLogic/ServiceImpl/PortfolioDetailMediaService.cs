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
using TNS.AdExpress.Domain.Level;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Web;
using System.Collections;
using WebCst = TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.WebResultUI;
using System.Reflection;
using TNS.AdExpressI.Portofolio;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class PortfolioDetaimMediaService : IPortfolioDetailMediaService
    {
        private WebSession _customerWebSession = null;


        public GridResult GetDetailMediaGridResult(string idWebSession, string idMedia, string dayOfWeek, string ecran)
        {
            GridResult GridResultResponse = new GridResult();
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);

            IPortofolioResults detailMediaResult = InitDetailMediaCall(_customerWebSession, dayOfWeek, ecran);

            GridResultResponse = detailMediaResult.GetDetailMediaPopUpGridResult();

            return GridResultResponse;
        }


        public IPortofolioResults InitDetailMediaCall(WebSession custSession, string dayOfWeek, string ecran)
        {
            TNS.AdExpress.Domain.Web.Navigation.Module module = custSession.CustomerLogin.GetModule(custSession.CurrentModule);
            if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
            var parameters = new object[3];
            parameters[0] = custSession;
            parameters[1] = ecran;
            parameters[2] = dayOfWeek;
            var portofolioResult = (TNS.AdExpressI.Portofolio.IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);

            return portofolioResult;
        }

        public bool IsIndeRadioMessage(string idWebSession)
        {
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);

            //Les indes Radio
            if (WebApplicationParameters.CountryCode.Equals(WebCst.CountryCode.FRANCE)
                && Vehicle == CstDBClassif.Vehicles.names.radio
                //&& (_customerWebSession.GenericInsertionColumns != null && _customerWebSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.idTopDiffusion)) 
                )
            {
                #region Columns levels (Generic)
                VehicleInformation vehicleInfos = VehiclesInformation.Get(Vehicle);
                var columnItems = WebApplicationParameters.GenericColumnsInformation.GetGenericColumnItemInformationList(vehicleInfos.DetailColumnId);
                var isColumnTopDif = columnItems.Any(column => column.Id == GenericColumnItemInformation.Columns.idTopDiffusion);
                #endregion

                if (isColumnTopDif)
                {
                    return true;
                }
            }
            return false;
        }

        private CstDBClassif.Vehicles.names Vehicle
        {
            get
            {
                #region Obtention du vehicle

                string vehicleSelection = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
                if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new TNS.AdExpress.Exceptions.AdExpressCustomerException("La sélection de médias est incorrecte"));
                CstDBClassif.Vehicles.names vehicle = VehiclesInformation.DatabaseIdToEnum(long.Parse(vehicleSelection));

                #endregion

                return vehicle;
            }
        }

    }
}
