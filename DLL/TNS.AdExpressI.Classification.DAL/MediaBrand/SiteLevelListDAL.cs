#region Informations
// Auteur: D. Mussuma
// Date de création: 17/08/2009
// Date de modification: 
#endregion

using System;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpressI.Classification.DAL.MediaBrand {
	/// <summary>
	/// Load a list of site items
	/// </summary>
	public class SiteLevelListDAL : ClassificationLevelListDAL {
		
		#region Constructors
	
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public SiteLevelListDAL(int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.site), language, source) {
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public SiteLevelListDAL(int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.site), language, source, dbSchema)
        {
        }

		/// <summary>
		/// Constructor
		/// </summary>
		///<param name="idList">classification items' identifier list</param>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public SiteLevelListDAL(string idList, int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.site), idList, language, source) {
		}
         /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public SiteLevelListDAL(string idList, int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.site), idList, language, source, dbSchema)
        {
        }
		#endregion

        /// <summary>
        /// Set Schema
        /// </summary>
        protected override void SetSchema()
        {
            string rolexSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.rolex03).Label;
            if (string.IsNullOrEmpty(_dbSchema) || _dbSchema != rolexSchema) _dbSchema = rolexSchema;

        }
	}
}
