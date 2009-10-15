using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web.Navigation;

namespace TNS.AdExpressI.Classification.DAL.Default {

    /// <summary>
    /// This class provides all the SQL queries to search or select items of the product or vehicle
    /// clasification brand.
    /// The data can be filtered according to the rights or the selections of the customer.
    /// It contains the methods :
    /// - <code>GetMediaType();</code> Which provides to the customer the media items to select into a module.
    /// - <code>GetDetailMedia();</code> get the list of vehicles organised by Media genre or Sub media or Media Owner or Title.
    /// According to the vehicle classification levels choosen by the customer.
    /// - <code>GetDetailMedia(string keyWord);</code> Obtains the list of vehicles organised by Media genre or Sub media or Media Owner or Title.
    /// According to the vehicle classification levels choosen by the customer. Depends also on the classification label searched by the customer (keyWord).
    /// <code>GetItems;</code> All the methods with the same name provide to the customer the possibility to search items in product or vehicle classification brand.
    /// <code>GetRecapItems</code> provides to the customer the possibility to search items in product or vehicle classification brand. 
    /// <remarks>This methods is used only intio the modules
    /// " Product class analysis: Graphic key reports " and "Product class analysis: Detailed reports".</remarks>
    /// </summary>
    /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
    /// Impossible to execute query
    /// </exception>
    /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
    /// execution or building of the query to search classification items</exception>
	public class ClassificationDAL : TNS.AdExpressI.Classification.DAL.ClassificationDAL {
		#region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>	
		public ClassificationDAL(WebSession session)
			: base(session) {
		}
		/// <summary>
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="dimension">Product or vehicle classification brand</param>
		public ClassificationDAL(WebSession session, TNS.Classification.Universe.Dimension dimension)
			: base(session,dimension) {
		}
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>
		/// <param name="genericDetailLevel">generic detail level selected by the user</param>
        /// <param name="vehicleList">List of media selected by the user</param>
        public ClassificationDAL(WebSession session, GenericDetailLevel genericDetailLevel, string vehicleList)
            : base(session, genericDetailLevel, vehicleList)
        {
			
		}
		#endregion

     
	}
}
