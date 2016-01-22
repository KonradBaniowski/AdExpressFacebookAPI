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
	/// Load a list of groups items
	/// </summary>
	public class GroupLevelListDAL : ClassificationLevelListDAL {
		
		#region Constructors
	
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public GroupLevelListDAL(int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.group), language, source) {
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public GroupLevelListDAL(int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.group), language, source, dbSchema)
        {
        }

		/// <summary>
		/// Constructor
		/// </summary>
		///<param name="idList">classification items' identifier list</param>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public GroupLevelListDAL(string idList, int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.group), idList, language, source) {
		}
        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public GroupLevelListDAL(string idList, int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.group), idList, language, source, dbSchema)
        {
        }
		#endregion
	}
}
