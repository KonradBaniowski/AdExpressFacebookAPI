#region Informations
// Auteur: D. Mussuma
// Date de création: 17/08/2009
// Date de modification: 
#endregion

using System;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.Classification.DAL;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Level;
namespace TNS.AdExpressI.Classification.DAL.GrpBranch {
	/// <summary>
	/// Load a list of Program level's items
	/// </summary>
	public class TargetLevelListDAL : ClassificationLevelListDAL {

		#region Constructors
	
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public TargetLevelListDAL(int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.target), language, source) {
		}

         /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public TargetLevelListDAL(int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.target), language, source, dbSchema)
        {
        }
		/// <summary>
		/// Constructor
		/// </summary>
		///<param name="idList">classification items' identifier list</param>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public TargetLevelListDAL(string idList, int language, IDataSource source)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.target), idList, language, source)
        {
		}
        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public TargetLevelListDAL(string idList, int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.target), idList, language, source, dbSchema)
        {
        }
		#endregion

        /// <summary>
        /// Set Schema
        /// </summary>
        protected override void SetSchema()
        {
            if (string.IsNullOrEmpty(_dbSchema)) _dbSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.appm01).Label;

        }
	}
}
