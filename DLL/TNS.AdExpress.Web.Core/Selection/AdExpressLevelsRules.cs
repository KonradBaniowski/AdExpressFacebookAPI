using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Classification.Universe;
using TNS.Classification;

namespace TNS.AdExpress.Web.Core.Selection {
	public class AdExpressLevelsRules : ILevelsRules {
		/// <summary>
		/// Session 
		/// </summary>
		WebSession _webSession = null;

		/// <summary>
		/// Branch Ids
		/// </summary>
		List<int> _branchesIds = null;

		/// <summary>
		/// Universe levels
		/// </summary>
		List<UniverseLevel> _universeLevels = null;

		/// <summary>
		/// Nomenclature dimension  (product, media,..)
		/// </summary>
		TNS.Classification.Universe.Dimension _dimension = TNS.Classification.Universe.Dimension.product;

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="branchesIds">Branch Ids</param>
		/// <param name="universeLevels">Universe levels</param>
		public AdExpressLevelsRules(WebSession webSession, List<int> branchesIds, List<UniverseLevel> universeLevels, TNS.Classification.Universe.Dimension dimension) {
			if (webSession == null) throw (new ArgumentException("Invalid argument webSession"));
			_webSession = webSession;
			_branchesIds = branchesIds;
			_universeLevels = universeLevels;
			_dimension = dimension;
		}
		#endregion

		#region Methods implementation
		/// <summary>
		/// Get List of auhorized levels
		/// </summary>
		/// <returns>Authorized levels</returns>
		public virtual List<UniverseLevel> GetAuthorizedLevels() {
			List<UniverseLevel> tempList = new List<UniverseLevel>();
			switch(_dimension){
				case TNS.Classification.Universe.Dimension.product:
					for(int i=0; i<_universeLevels.Count; i++){
						if ((_universeLevels[i].ID == TNS.Classification.Universe.TNSClassificationLevels.BRAND && _webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE) == null)//Pas de droit pour les Marques
							|| (_universeLevels[i].ID == TNS.Classification.Universe.TNSClassificationLevels.HOLDING_COMPANY && _webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_HOLDING_COMPANY) == null)//Pas de droit pour les groupes de soci�t�s
							) continue;
						tempList.Add(_universeLevels[i]);
					}
					break;
				case TNS.Classification.Universe.Dimension.media:
					return _universeLevels;
					
			}
			return tempList;
		}

		/// <summary>
		/// Get List of auhorized branches
		/// </summary>
		/// <returns>Authorized branches</returns>
		public virtual List<int> GetAuthorizedBranches() {
			List<int> tempList = new List<int>();
			switch (_dimension) {
				case TNS.Classification.Universe.Dimension.product:
					for (int i = 0; i < _branchesIds.Count; i++) {
						if (_webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE) == null &&
							(UniverseBranches.Get(_branchesIds[i]) != null && UniverseBranches.Get(_branchesIds[i]).Contains(TNS.Classification.Universe.TNSClassificationLevels.BRAND) && UniverseBranches.Get(_branchesIds[i]).Levels.Count <= 2)
							)//Pas de droit pour les Marques
							 continue;
						tempList.Add(_branchesIds[i]);
					}
					break;
				case TNS.Classification.Universe.Dimension.media:
					return _branchesIds;
			}
			return tempList;
		}
		#endregion
	}
}
