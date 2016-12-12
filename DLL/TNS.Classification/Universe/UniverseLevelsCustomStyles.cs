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
	public class UniverseLevelsCustomStyles {

		/// <summary>Instance of UniverseLevelsCustomStyles</summary>		
		private static UniverseLevelsCustomStyles _instance;

		/// <summary>List of style by level</summary>		
		private Dictionary<long, System.Web.UI.WebControls.Style> _levelsStyles;

		#region Accessors
		/// <summary>Get a specific style</summary>			
		public System.Web.UI.WebControls.Style this[long id] {
			get {
				return (_levelsStyles.ContainsKey(id)) ? _levelsStyles[id] : null;
			}
		}		
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source"></param>
		private UniverseLevelsCustomStyles() {		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source"></param>
		private UniverseLevelsCustomStyles(IDataSource source) {
			this._levelsStyles = UniverseLevelsCustomStylesDataAccess.Load(source);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get instance of UniverseLevels		
		public static UniverseLevelsCustomStyles getInstance(IDataSource source) {
			lock (typeof(UniverseLevelsCustomStyles)) {
				if (_instance == null) {
					_instance = new UniverseLevelsCustomStyles(source);
				}
			}
			return _instance;
		}


		///  <summary>Get a specific style</summary>
		///  <param name="id">level Identifier </param>
		///  <returns>specific level</returns>
		public static System.Web.UI.WebControls.Style Get(long id) {
			return _instance[id];
		}

		#endregion

	}
}
