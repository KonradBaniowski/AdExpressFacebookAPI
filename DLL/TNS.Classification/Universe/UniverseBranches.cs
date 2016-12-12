#region Informations
// Auteur: D. Mussuma
// Création: 19/11/2007
// Modification:
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.Classification.DataAccess;

namespace TNS.Classification.Universe {
	/// <summary>
	/// Class corresponding to  a collection of universe branches
	/// </summary>
	public class UniverseBranches{
		
		/// <summary>Instance of branches</summary>		
		private static UniverseBranches _instance;

		/// <summary>List of Branches</summary>		
		private Dictionary<int, UniverseBranch> _branches;

		#region Accessors
		/// <summary>Get a specific Branch List (branch)</summary>		
		public UniverseBranch this[int id] {
			get {
				return _branches[id];
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source"></param>
		private UniverseBranches() {		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source"></param>
		private UniverseBranches(IDataSource source) {
			this._branches = UniverseBranchesDataAccess.Load(source);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get instance of Branches		
		public static UniverseBranches getInstance(IDataSource source) {
			lock (typeof(UniverseBranches)) {
				if (_instance == null) {
					_instance = new UniverseBranches(source);
				}
			}
			return _instance;
		}

		///  <summary>Get a specific branch</summary>
		///  <param name="detailLevelId">Identifier</param>
		///  <returns>specific level list</returns>
		public static UniverseBranch Get(int id) {
			return _instance[id];
		}

		#endregion
	}
}
