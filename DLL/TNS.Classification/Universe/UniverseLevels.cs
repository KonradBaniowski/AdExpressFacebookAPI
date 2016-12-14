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
	public class UniverseLevels {

		/// <summary>Instance of Levels</summary>		
		private static UniverseLevels _instance;

		/// <summary>List of UniverseLevels</summary>		
		private Dictionary<long, UniverseLevel> _levels;

		#region Accessors
		/// <summary>Get a specific UniverseLevel</summary>		
		public UniverseLevel this[long id] {
			get {
				return _levels[id];
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source"></param>
		private UniverseLevels() {		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source"></param>
		private UniverseLevels(IDataSource source) {
			this._levels = UniverseLevelsDataAccess.Load(source);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get instance of UniverseLevels		
		public static UniverseLevels getInstance(IDataSource source) {
			lock (typeof(UniverseLevels)) {
				if (_instance == null) {
					_instance = new UniverseLevels(source);
				}
			}
			return _instance;
		}


		///  <summary>Get a specific UniverseLevel</summary>
		///  <param name="id">level Identifier</param>
		///  <returns>specific level</returns>
		public static UniverseLevel Get(long id) {
			return _instance[id];
		}
		
		/// <summary>
		/// Get a list of UniverseLevel corresponding to the parameters ids
		/// </summary>
		/// <param name="levelsId">list levels Id</param>
		/// <returns>List of UniverseLevel</returns>
		public static List<UniverseLevel> GetList(List<long> levelsId ){
			try{
				List<UniverseLevel> list = new List<UniverseLevel>();
				if (levelsId != null && levelsId.Count > 0) {
					for (int i = 0; i <levelsId.Count; i++) {
						list.Add(_instance._levels[levelsId[i]]);
					}
				}
				return list;

			}
            catch(System.Exception err) {
				throw (new ApplicationException("Impossible to convert string to List<UniverseLevel>", err)); 
            }
		}		

		#endregion
	}
}
