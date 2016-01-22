#region Informations
// Auteur: D. Mussuma
// Date de création: 17/08/2009
// Date de modification: 
#endregion

using System;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpressI.Classification.DAL.MediaBrand {
	/// <summary>
	/// Load a list of region  items
	/// </summary>
	public class RegionLevelListDAL : ClassificationLevelListDAL {
		
		#region Constructors
	
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public RegionLevelListDAL(int language, IDataSource source)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.region), language, source)
        {
		}
		/// <summary>
		/// Constructor
		/// </summary>
		///<param name="idList">classification items' identifier list</param>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
        public RegionLevelListDAL(string idList, int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.region), idList, language, source) {
		}
		#endregion
	}
}
