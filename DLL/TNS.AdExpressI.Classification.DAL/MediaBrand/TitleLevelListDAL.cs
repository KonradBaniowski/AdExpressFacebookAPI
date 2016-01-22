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
	/// Load a list of title items
	/// </summary>
	public class TitleLevelListDAL:ClassificationLevelListDAL{
		
		#region Constructors
	
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public TitleLevelListDAL(int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.title), language, source) {
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public TitleLevelListDAL(int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.title), language, source, dbSchema)
        {
        }

		/// <summary>
		/// Constructor
		/// </summary>
		///<param name="idList">classification items' identifier list</param>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public TitleLevelListDAL(string idList, int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.title), idList, language, source) {
		}
         /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public TitleLevelListDAL(string idList, int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.title), idList, language, source, dbSchema)
        {
        }
		#endregion
	}
}
