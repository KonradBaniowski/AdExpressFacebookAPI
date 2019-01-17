using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Portofolio.DAL;

namespace TNS.AdExpressI.Portofolio.Turkey.Engines
{
    public class PortofolioDetailEngine : Portofolio.Engines.PortofolioDetailEngine
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">vehicleInformation</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public PortofolioDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, bool showInsertions, bool showCreatives)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd, showInsertions, showCreatives)
        {
        }
        #endregion

        protected override List<UnitInformation> GetValidUnitForResult()
        {
            return _webSession.GetSelectedUnits();
        }

        /// <summary>
        /// Get data for ResultTable result
        /// </summary>
        /// <returns></returns>
        protected override long CountDataRows()
        {
            DataSet ds = null;
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[5];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
            return portofolioDAL.CountData();
        }
    }
}
