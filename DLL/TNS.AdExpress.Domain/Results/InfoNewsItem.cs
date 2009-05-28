#region Informations
// Auteur: D. Mussuma
// Création: 25/05/2009
// Modification:
#endregion
using System;
using System.Collections.Generic;
using System.Text;

using WebCst = TNS.AdExpress.Constantes.Web;
using TradCst = TNS.AdExpress.Constantes.DB.Language;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Domain.Results {
	public class InfoNewsItem {

		#region Variables
		/// <summary>
		/// Id directory
		/// </summary>
		protected WebCst.ModuleInfosNews.Directories _id;
		/// <summary>
		/// Directory texte code
		/// </summary>
		protected Int64 _webTextId = 0;
		/// <summary>
		/// Virtual directory path
		/// </summary>
		protected string _virtualPath = "";
		/// <summary>
		/// Physical directory path
		/// </summary>
		protected string _physicalPath = "";
		/// <summary>
		/// Nb max item to show
		/// <example>If -1 show all directory's items</example>
		/// </summary>
		protected int _nbMaxItemsToShow = -1;
		#endregion

		#region Accessors
		/// <summary>
		/// Get directory texte code
		/// </summary>
		public Int64 WebTextId {
			get { return (_webTextId); }
		}
		/// <summary>
		/// Get Nb max item to show
		/// </summary>
		public int NbMaxItemsToShow {
			get { return (_nbMaxItemsToShow); }
		}
		/// <summary>
		/// Get Physical directory path
		/// </summary>
		public string PhysicalPath {
			get { return (_physicalPath); }
		}
		/// <summary>
		/// Get Virtual directory path
		/// </summary>
		public string VirtualPath {
			get { return (_virtualPath); }
		}

		/// <summary>
		/// Get Id directory
		/// </summary>
		public WebCst.ModuleInfosNews.Directories Id {
			get { return (_id); }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">id</param>
		/// <param name="webTextId">web Text Id</param>
		/// <param name="virtualPath">directory virtual Path </param>
		/// <param name="physicalPath">directory physical Path </param>
		/// <param name="nbMaxItemsToShow">nb Max Items To Show</param>
		public InfoNewsItem(WebCst.ModuleInfosNews.Directories id, Int64 webTextId, string virtualPath, string physicalPath, int nbMaxItemsToShow) {
			if (id < 0) throw (new ArgumentException("Invalid argument id"));
			if (virtualPath == null || virtualPath.Length < 1) throw (new ArgumentException("Invalid argument virtualPath"));
			if (physicalPath == null || physicalPath.Length < 1) throw (new ArgumentException("Invalid argument physicalPath"));
			if (webTextId < 0) throw (new ArgumentException("Invalid argument webTextId"));

			_id = id;
			_virtualPath = virtualPath;
			_webTextId = webTextId;
			_physicalPath = physicalPath;
			_nbMaxItemsToShow = nbMaxItemsToShow;
		}

		#endregion
	}
}
