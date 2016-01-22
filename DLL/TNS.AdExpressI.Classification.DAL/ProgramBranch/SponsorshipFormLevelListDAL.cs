#region Informations
// Auteur: D. Mussuma
// Date de création: 17/08/2009
// Date de modification: 
#endregion

using System;
using TNS.AdExpressI.Classification.DAL;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Level;
namespace TNS.AdExpressI.Classification.DAL.ProgramBranch {
	/// <summary>
    /// Load a list of  SponsorshipForm level's items
	/// </summary>
	public class SponsorshipFormLevelListDAL : ClassificationLevelListDAL {

		#region Constructors
	
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public SponsorshipFormLevelListDAL(int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.sponsorshipForm), language, source) {
		}

         /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public SponsorshipFormLevelListDAL(int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.sponsorshipForm), language, source, dbSchema)
        {
        }
		/// <summary>
		/// Constructor
		/// </summary>
		///<param name="idList">classification items' identifier list</param>
		/// <param name="language">Data language</param>
		/// <param name="source">Data source</param>
		public SponsorshipFormLevelListDAL(string idList, int language, IDataSource source)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.sponsorshipForm), idList, language, source)
        {
		}
        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public SponsorshipFormLevelListDAL(string idList, int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.sponsorshipForm), idList, language, source, dbSchema)
        {
        }
		#endregion
	}
}
