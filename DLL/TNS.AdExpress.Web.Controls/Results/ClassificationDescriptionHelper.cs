using System;
using ClassificationTable=TNS.AdExpress.Constantes.Classification.DB.Table;

namespace TNS.AdExpress.Web.Controls.Results{
	/// <summary>
	/// Description résumée de ClassificationDescriptionHelper.
	/// </summary>
	public class ClassificationDescriptionHelper{

		#region Variables
		/// <summary>
		/// Identifiant du texte
		/// </summary>
		private Int64 _labelTextId;
		/// <summary>
		/// Type du niveau de recherche
		/// </summary>
		private ClassificationTable.name _levelType;
		#endregion

		/// <summary>
		/// Obtient ou définit l'identifiant du texte
		/// </summary>
		public string LabelTextId {
			get {return _labelTextId.ToString();}
			set {_labelTextId = Int64.Parse(value);}
		}


		/// <summary>
		/// Obtient ou définit le type du niveau de recherche
		/// </summary>
		public ClassificationTable.name LevelType{
			get {return _levelType;}
			set {_levelType = value;}
		}
	}
}
