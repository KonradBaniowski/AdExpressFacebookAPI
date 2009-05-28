#region Informations
// Auteur: D. Mussuma
// Création: 26/05/2009
// Modification:
#endregion
using System;
using System.Collections.Generic;
using System.Text;

using WebCst = TNS.AdExpress.Constantes.Web;
using TradCst = TNS.AdExpress.Constantes.DB.Language;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Layers;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;

namespace TNS.AdExpress.Domain.Results {
	/// <summary>
	/// Info news description
	/// </summary>
	public class InfoNews {
		#region Variables
		/// <summary>
		/// Country rules Layer
		/// </summary>
		protected RulesLayer _rulesLayer = null;
		/// <summary>
		/// Info news items
		/// </summary>
		Dictionary<WebCst.ModuleInfosNews.Directories, Results.InfoNewsItem> _infoNewsItems = new Dictionary<WebCst.ModuleInfosNews.Directories, Results.InfoNewsItem>();
		/// <summary>
		/// Sorted Info news items
		/// </summary>
		List<Results.InfoNewsItem> _infoNewsSortedItems = new List<InfoNewsItem>();
		#endregion

		#region Constructor		
        /// <summary>
        /// Constructor
        /// </summary>
        public InfoNews(IDataSource source)
        {
			 InfoNewsXL.LoadInfoNews(source, ref _rulesLayer, _infoNewsItems, _infoNewsSortedItems);
		}
		#endregion

		/// <summary>
		/// Get Infos news items 
		/// </summary>
		public Dictionary<WebCst.ModuleInfosNews.Directories, Results.InfoNewsItem> InfoNewsItems {
			get { return _infoNewsItems; }
		}
		/// <summary>
		/// Get sorted infonews item 
		/// </summary>
		/// <remarks>Order correspond to directory order into XML configuration file</remarks>
		/// <returns></returns>
		public List<Results.InfoNewsItem> GetSortedInfoNewsItems() {
			return _infoNewsSortedItems;
			
		}
		/// <summary>
		/// Get/Set Rules Layer
		/// </summary>
		public RulesLayer CountryRulesLayer {
			get { return _rulesLayer; }
			set { _rulesLayer = value; }
		}

	}
}
