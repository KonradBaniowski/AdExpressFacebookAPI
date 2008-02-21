#region Information
/*
 * Author : G. Ragneau
 * Creation : 24/08/2005
 * Modification : 
 *		
 * */
#endregion

using System;
using System.Data;
using TNS.FrameWork.DB.Common;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Selections.Products;

namespace TNS.AdExpress.Web.BusinessFacade.Selections.Products
{
	/// <summary>
	/// Set of tools for group management
	/// </summary>
	public class GroupSystem
	{

		/// <summary>
		/// Get list of groups matcing with the CurrentAdvertiserSelection in the user session.
		/// Do not forget to load CurrentAdvertiserSelection.....
		/// </summary>
		/// <param name="webSession">User session</param>
		/// <param name="dataSource">Data Source</param>
		/// <returns>List of groups</returns>
		public static DataSet ListFromSelection(IDataSource dataSource, WebSession webSession){
			try{
				return GroupDataAccess.ListFromSelection(dataSource, webSession);
			}
			catch(System.Exception e){
				throw(e);
			}
		}

	}
}
