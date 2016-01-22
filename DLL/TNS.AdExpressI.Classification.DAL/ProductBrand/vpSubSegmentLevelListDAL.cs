#region Informations
// Auteur: D. Mussuma
// Date de création: 17/08/2009
// Date de modification: 
#endregion

using System;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;


namespace TNS.AdExpressI.Classification.DAL.ProductBrand {
	/// <summary>
	/// Load a list of advertisers items
	/// </summary>
	public class vpSubSegmentLevelListDAL:ClassificationLevelListDAL{
		

		#region Constructors
	
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public vpSubSegmentLevelListDAL(int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.vpSubSegment), language, source) {
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public vpSubSegmentLevelListDAL(int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.vpSubSegment), language, source, dbSchema)
        {
        }
		/// <summary>
		/// Constructor
		/// </summary>
		///<param name="idList">classification items' identifier list</param>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
        public vpSubSegmentLevelListDAL(string idList, int language, IDataSource source)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.vpSubSegment), idList, language, source)
        {
		}
        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public vpSubSegmentLevelListDAL(string idList, int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.vpSubSegment), idList, language, source, dbSchema)
        {
        }
		#endregion

        /// <summary>
        /// Set Schema
        /// </summary>
        protected override void SetSchema()
        {
            string promoSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.promo03).Label;
            if (string.IsNullOrEmpty(_dbSchema) || _dbSchema != promoSchema) _dbSchema = promoSchema;

        }
	}
}
