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

namespace TNS.AdExpressI.Classification.DAL.MediaBrand {
	/// <summary>
	/// Load a list of vehicle items
	/// </summary>
	public class VpCircuitLevelListDAL:ClassificationLevelListDAL{

		#region Constructors
	
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public VpCircuitLevelListDAL(int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.vpCircuit), language, source) {
		}

          /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public VpCircuitLevelListDAL(int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.vpCircuit), language, source, dbSchema)
        {
        }

		/// <summary>
		/// Constructor
		/// </summary>
		///<param name="idList">classification items' identifier list</param>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
        public VpCircuitLevelListDAL(string idList, int language, IDataSource source)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.vpCircuit), idList, language, source)
        {
		}
        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public VpCircuitLevelListDAL(string idList, int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.vpCircuit), idList, language, source, dbSchema)
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
