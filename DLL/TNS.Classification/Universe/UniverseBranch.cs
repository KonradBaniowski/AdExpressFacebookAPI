#region Informations
// Auteur: D. Mussuma
// Création: 19/11/2007
// Modification:
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Classification.Universe {
	/// <summary>
	/// Class corresponding to  a universe branch. A branch is an ordered collection of universe level
	/// </summary>
	public class UniverseBranch {
	
		///<summary>Branch Id.</summary>		
		private int _id;

		/// <summary>
		/// Level Label ID
		/// </summary>
		private int _labelId = 0;

		///<summary>Branch levels.</summary>		
		private List<UniverseLevel> _levels;
		

		/// <summary>
		/// Get Branch  ID
		/// </summary>		
		public int ID {
			get { return _id; }
		}

		/// <summary>
		/// Get Level Label ID
		/// </summary>
		public int LabelId {
			get { return _labelId; }
		}

		/// <summary>
		/// Get Levels 
		/// </summary>
		public List<UniverseLevel> Levels {
			get { return _levels; }
		}

		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">level Identifier</param>
		/// <param name="levels">Branch  levels</param>
		/// <param name="labelId">Label Identifier</param>
		public UniverseBranch(int id, List<UniverseLevel> levels, int labelId) {
			if (levels == null) throw (new ArgumentException("Invalid argument levels"));
			if (id < 0) throw (new ArgumentException("Invalid argument id"));
			if (labelId < 0) throw (new ArgumentException("Invalid argument labelId"));
			_id = id;
			_levels = levels;
			_labelId = labelId;
		}

		/// <summary>
		/// Checks if collection contains level
		/// </summary>
		/// <param name="levelId">level Id</param>
		/// <returns>True if contains level id , false else</returns>
		public bool Contains(long levelId) {
			if (_levels != null && _levels.Count > 0) {
				for (int i = 0; i < _levels.Count; i++) {
					if (_levels[i].ID == levelId) return true;
				}
			}
			return false;
		}
	}
}
