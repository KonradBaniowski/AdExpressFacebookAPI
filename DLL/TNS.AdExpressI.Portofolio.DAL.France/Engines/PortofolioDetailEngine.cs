#region Information
// Author: D. Mussuma
// Creation date: 11/08/2008
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Web.Core.Utilities;
using Module = TNS.AdExpress.Domain.Web.Navigation.Module;
using DBConstantes = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpressI.Portofolio.DAL.France.Engines {
	/// <summary>
	/// Get different data for portofolio detail
	/// </summary>
    public class PortofolioDetailEngine : DAL.Engines.PortofolioDetailEngine
    {

		#region PortofolioDetailEngine
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicleInformation">Vehicle information</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public PortofolioDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd)
			: base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) {
		}
		#endregion

		
	
        protected override string GetBannerGroupByOptional(AdExpress.Constantes.DB.TableType.Type type)
        {
            string groupByOptional = string.Format(type == AdExpress.Constantes.DB.TableType.Type.webPlan
                                                       ? ",{0}.LISTNUM_TO_CHAR({1}.list_banners) " : ",{1}.HASHCODE ", WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            return groupByOptional;
        }

        #region Get Unit
        protected override string GetUnit(int i, List<UnitInformation> unitsList)
        {
            return string.Format("{0}.LISTNUM_TO_CHAR({1}) as {2}", 
                WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label,
                unitsList[i].DatabaseMultimediaField, unitsList[i].Id.ToString());
        }
        #endregion

        protected override void GetGad(ref string dataTableNameForGad, ref string dataFieldsForGad, ref string dataJointForGad)
        {
            if (
                _webSession.CustomerLogin.CustormerFlagAccess(
                    (long)TNS.AdExpress.Constantes.Customer.DB.Flag.id.leFac.GetHashCode()))
                dataTableNameForGad = ", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.leFac).SqlWithPrefix;
            else
                dataTableNameForGad = ", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad).SqlWithPrefix;
            dataFieldsForGad = ", " + SQLGenerator.GetFieldsAddressForGad();
            dataJointForGad = "and " +
                              SQLGenerator.GetJointForGad(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

        }

    }
}
