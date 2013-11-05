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

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.DataBaseDescription;


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

        protected override string GetCustomerRights()
        {
            if (_session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.VP)
                return _session.GetVpBrandsRights(string.Empty, true);
            return GetMediaRights(string.Empty, true);
        }
	}
}
