#region Informations
// Auteur: G. Facon
// Date de création: 01/06/2005
// Date de modification: 01/06/2005
#endregion

using System;
using TNS.AdExpress.Anubis.Constantes;

namespace TNS.AdExpress.Anubis.Common.Parameters{
	/// <summary>
	/// Paramètre pour les résultats Bilan de campagne (APPM)
	/// </summary>
	[System.Serializable]
	public class Appm:Generic{
		#region Variables
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="resultType">Type de résultat</param>
		public Appm(Result.type resultType):base(resultType){

		}
		#endregion
	}
}
