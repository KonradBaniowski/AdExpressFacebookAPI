using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Portofolio.DAL;
using AbstractResult = TNS.AdExpressI.Portofolio.Engines;

namespace TNS.AdExpressI.Portofolio.Turkey.Engines
{
    public class CalendarEngine : AbstractResult.CalendarEngine
    {

        public CalendarEngine(WebSession webSession, VehicleInformation vehicleInformation, long idMedia,
            string periodBeginning, string periodEnd)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd)
        {
        }

        protected override long CountDataRows()
        {
          
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[5];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
            return portofolioDAL.CountData();         
        }
    }
}
