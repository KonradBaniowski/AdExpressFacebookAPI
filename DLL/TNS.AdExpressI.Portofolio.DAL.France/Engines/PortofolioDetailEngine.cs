#region Information
// Author: D. Mussuma
// Creation date: 11/08/2008
// Modification date:
#endregion

using System;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using Module = TNS.AdExpress.Domain.Web.Navigation.Module;

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

        
	}
}
