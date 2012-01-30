using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Classification.Universe;
using TNS.Classification;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Classification;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;

namespace TNS.AdExpress.Web.Core.Selection {
	/// <summary>
	/// AdExpress levels rights rules
	/// </summary>
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
			for (int i = 0; i < _universeLevels.Count; i++) {
				if (!CanAddLevel(_universeLevels[i].ID)) continue;
				tempList.Add(_universeLevels[i]);
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
				case TNS.Classification.Universe.Dimension.advertisingAgency:
					for (int i = 0; i < _branchesIds.Count; i++) {
						if (!CanAddBranch(_branchesIds[i]))continue;
						tempList.Add(_branchesIds[i]);
					}
					break;
				case TNS.Classification.Universe.Dimension.media:
                case TNS.Classification.Universe.Dimension.advertisementType:
					return _branchesIds;
			}
			return tempList;
		}
		#endregion

		#region Internal methods

		#region CanAddLevel
		/// <summary>
		/// Checks if classification level can be added
		/// </summary>
		/// <param name="levelId">level Id</param>
		/// <returns>Checks if classification level can be added</returns>
        protected virtual bool CanAddLevel(long levelId)
        {
			
			if ((levelId == TNS.Classification.Universe.TNSClassificationLevels.BRAND && !_webSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE))//No brand rights
				|| (levelId == TNS.Classification.Universe.TNSClassificationLevels.HOLDING_COMPANY && !_webSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_HOLDING_COMPANY))//No holding group rights
				|| (levelId == TNS.Classification.Universe.TNSClassificationLevels.PRODUCT && !_webSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))//No Product level rights (For Finland)
                || (levelId == TNS.Classification.Universe.TNSClassificationLevels.SEGMENT && !_webSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG))//No Segment level rights (For Finland)
				) return false;

			switch (_dimension) {								
				case TNS.Classification.Universe.Dimension.media:
					return IsValidLevels(levelId);
				case TNS.Classification.Universe.Dimension.product:
                case TNS.Classification.Universe.Dimension.advertisingAgency:
				default :
					return true; 	
			}
			
		}
		#endregion

		#region CanAddBranch
		/// <summary>
		/// Checks if branch can be added
		/// </summary>
		/// <param name="branchId">branch Id</param>
		/// <returns>Checks if branch can be added</returns>
        protected virtual bool CanAddBranch(int branchId)
        {

			if (!_webSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE) &&
				(UniverseBranches.Get(branchId) != null && UniverseBranches.Get(branchId).Contains(TNS.Classification.Universe.TNSClassificationLevels.BRAND) && UniverseBranches.Get(branchId).Levels.Count <= 2)
				)//No brand rights
				return false;

			
			return true;
		}
		#endregion

		/// <summary>
		/// determine universe level validity
		/// </summary>
		/// <returns>Tur if level is valid</returns>
		public virtual bool IsValidLevels(long levelId) {
			bool val = true;
			try {
                Module moduleDescription = ModulesList.GetModule(_webSession.CurrentModule);
                List<Int64> vehicleList = new List<Int64>();
                string listStr = _webSession.GetSelection(_webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
                if (listStr != null && listStr.Length > 0)
                {
                    string[] list = listStr.Split(',');
                    for (int i = 0; i < list.Length; i++)
                        vehicleList.Add(Convert.ToInt64(list[i]));
                }
                else
                {
                    //When a vehicle is not checked but one or more category, this get the vehicle correspondly
                    Int64 Vehicle = ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID;
                    vehicleList.Add(Vehicle);
                }
                for (int j = 0; j < vehicleList.Count; j++)
                {
                    if (VehiclesInformation.Contains(vehicleList[j]))
                    {
                        if (!VehiclesInformation.Get(vehicleList[j]).AllowedUniverseLevels.Contains(levelId))
                        {
                            val = false; break;
                        }
                    }
                }

				return val;
			}
			catch {
				throw (new Exceptions.UniversListException("Impossible to determine universe level validity"));
			}
		}


		#endregion
	}
}
