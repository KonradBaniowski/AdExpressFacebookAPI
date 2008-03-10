#region Informations
// Auteur: G. Facon
// Date de création: 14/06/2006
// Date de modification: 
#endregion


using System;
using TNS.FrameWork.DB.Common;
using Classification=TNS.AdExpress.DataAccess.Classification;
using ClassificationTable=TNS.AdExpress.Constantes.Classification.DB.Table;

namespace TNS.AdExpress.Web.Controls.Results{
	/// <summary>
	/// Description résumée de ClassificationDescription.
	/// </summary>
	public class ClassificationDescription{
		#region Variables
		/// <summary>
		/// Type du niveau
		/// </summary>
		private ClassificationTable.name _levelType=ClassificationTable.name.sector;
		/// <summary>
		/// Texte de présentation
		/// </summary>
		private Int64 _labelTextId=0;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public ClassificationDescription(){
		}
		/// <summary>
		/// Constucteur
		/// </summary>
		public ClassificationDescription(ClassificationTable.name levelType,Int64 labelTextId){
			_levelType=levelType;
			_labelTextId=labelTextId;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou définit le type du niveau
		/// </summary>
		public ClassificationTable.name LevelType{
			get{return(_levelType);}
			set{_levelType=value;}
		}

		/// <summary>
		/// Obtient ou définit le texte
		/// </summary>
		public Int64 labelTextId{
			get{return(_labelTextId);}
			set{_labelTextId=value;}
		}
		#endregion
	}
}
