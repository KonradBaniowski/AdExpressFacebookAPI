#region Informations
// Auteur: D. Mussuma
// Date de création: 17/08/2009
// Date de modification: 
#endregion

using System;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Level;
using TNS.Classification.Universe;

namespace TNS.AdExpressI.Classification.DAL {
	/// <summary>
	/// Interface which provides all methods to create items list of classification level.
	/// </summary>
	public interface IClassificationLevelListDALFactory {

		/// <summary>	
		/// Get all items list of a classification's level
		/// </summary>
		/// <param name="detailLevelItemInformation">Detail level informations</param>
		ClassificationLevelListDAL CreateClassificationLevelListDAL(DetailLevelItemInformation detailLevelItemInformation);


		/// Get partial items list of a classification's level
		/// </summary>
		/// <param name="detailLevelItemInformation">Detail level informations</param>
		/// <param name="idList">classification items' identifier list</param>
		ClassificationLevelListDAL CreateClassificationLevelListDAL(DetailLevelItemInformation detailLevelItemInformation, string idList);

        ClassificationLevelListDAL CreateClassificationLevelListDAL(DetailLevelItemInformation detailLevelItemInformation, string idList, string dbSchema);

        /// Get partial items list of a classification's level
        /// </summary>
        /// <param name="levelType">Classification level type</param>
        /// <param name="idList">classification items' identifier list</param>
        ClassificationLevelListDAL CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type levelType, string idList);

		

        ClassificationLevelListDAL CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type levelType, string idList, string dbSchema);
        /// <summary>
        /// Get if data items shiould be in lower case
        /// </summary>
        bool ToLowerCase { get; }	
        ClassificationLevelListDAL CreateDefaultClassificationLevelListDAL(UniverseLevel level, string idList);

        ClassificationLevelListDAL CreateDefaultClassificationLevelListDAL(UniverseLevel level, string idList, string dbSchema);

    }
}
