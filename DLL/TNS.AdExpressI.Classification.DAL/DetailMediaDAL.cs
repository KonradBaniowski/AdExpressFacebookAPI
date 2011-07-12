#region Information
/*
 * Author : Y Rkaina && D. Mussuma
 * Created on : 16/07/2009
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web.Navigation;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.DataBaseDescription;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using VehicleClassificationCst = TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;


namespace TNS.AdExpressI.Classification.DAL {
    /// <summary>
    /// provides the queries to obtain the list of vehicles organised by Media genre or Sub media or Media Owner or Title.
    /// <example>
    /// If the user wants to select vehicles displayed by Sub media\Vehicle, the clause select of the query will be
    /// : select distinct id_category as idDetailMedia, category as detailMedia, id_media, media from ...
    /// </example>
    /// Use the methods <code>GetData();</code> or <code>GetData(string keyWord);</code>
    /// </summary>	   
    /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
    /// Exception throwed when an error occurs in the building or execution of the SQL query.</exception>
    public class DetailMediaDAL : DetailDAL
    {

        #region Constructor(s)
        /// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="genericDetailLevel">generic detail level selected by the user</param>
		/// <param name="listMedia">List of media selected by the user</param>
		public DetailMediaDAL(WebSession session, GenericDetailLevel genericDetailLevel)
            : base(session, genericDetailLevel) {
		}
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="genericDetailLevel">generic detail level selected by the user</param>
		/// <param name="listMedia">List of media selected by the user</param>
		public DetailMediaDAL(WebSession session, GenericDetailLevel genericDetailLevel, string listMedia)
			: base(session, genericDetailLevel, listMedia) {		
		}
		
		#endregion

        /// <summary>
        /// Get View
        /// </summary>
        /// <returns>View</returns>
        protected override string GetView() {
            if (_session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.VP)
                return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allPromoBrand).Sql;
            else
                return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMedia).Sql + _session.DataLanguage;
        }

	}
}
