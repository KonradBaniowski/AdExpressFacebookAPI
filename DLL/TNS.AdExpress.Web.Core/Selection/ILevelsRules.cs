#region Informations
// Auteur: D. Mussuma
// Création: 19/11/2007
// Modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.Classification.Universe;

namespace TNS.AdExpress.Web.Core.Selection {
	public interface ILevelsRules {

		/// <summary>
		/// Get List of auhorized levels
		/// </summary>
		/// <returns>Authorized levels</returns>
		List<UniverseLevel> GetAuthorizedLevels();

		/// <summary>
		/// Get List of auhorized branches
		/// </summary>
		/// <returns>Authorized branches</returns>
		List<int> GetAuthorizedBranches();
	}
}
