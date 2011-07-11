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
	/// Load a list of advertisers items
	/// </summary>
	public class AdvertiserLevelListDAL:ClassificationLevelListDAL{
		

		#region Constructors
	
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public AdvertiserLevelListDAL(int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.advertiser), language, source) {
		}

          /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public AdvertiserLevelListDAL(int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.advertiser), language, source, dbSchema)
        {
        }
         /// Constructor of items list of classification's level
        /// </summary>
        /// <remarks>Use only in TNS AdExpress website</remarks>
        /// <param name="detailLevelItemInformation">Detail level information to build the list</param>
        /// <param name="language">Data language identifier</param>
        /// <param name="source">Data source</param>
        public AdvertiserLevelListDAL(DetailLevelItemInformation detailLevelItemInformation, int language, IDataSource source, string dbSchema) :
        base(detailLevelItemInformation,language,source,dbSchema){
    }
		/// <summary>
		/// Constructor
		/// </summary>
		///<param name="idList">classification items' identifier list</param>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public AdvertiserLevelListDAL(string idList,int language, IDataSource source)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.advertiser), idList, language, source)
        {
		}
         /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public AdvertiserLevelListDAL(string idList, int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.advertiser), idList, language, source, dbSchema)
        {
        }
		#endregion
	}
}
