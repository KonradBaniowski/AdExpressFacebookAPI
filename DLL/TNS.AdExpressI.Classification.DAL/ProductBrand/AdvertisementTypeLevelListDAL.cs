#region Informations
// Auteur: D. Mussuma
// Date de création: 17/08/2009
// Date de modification: 
#endregion

using System;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Level;


namespace TNS.AdExpressI.Classification.DAL.ProductBrand {
	/// <summary>
	/// Load a list of advertisement type items
	/// </summary>
	public class AdvertisementTypeLevelListDAL:ClassificationLevelListDAL{
		

		#region Constructors
	
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public AdvertisementTypeLevelListDAL(int language, IDataSource source)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.advertisementType), language, source)
        {
		}
		/// <summary>
		/// Constructor
		/// </summary>
		///<param name="idList">classification items' identifier list</param>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
        public AdvertisementTypeLevelListDAL(string idList, int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.advertisementType),idList, language, source) {
		}
		#endregion
	}
}
