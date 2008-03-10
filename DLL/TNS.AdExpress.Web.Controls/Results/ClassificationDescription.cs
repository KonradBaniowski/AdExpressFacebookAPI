#region Informations
// Auteur: G. Facon
// Date de cr�ation: 14/06/2006
// Date de modification: 
#endregion


using System;
using TNS.FrameWork.DB.Common;
using Classification=TNS.AdExpress.DataAccess.Classification;
using ClassificationTable=TNS.AdExpress.Constantes.Classification.DB.Table;

namespace TNS.AdExpress.Web.Controls.Results{
	/// <summary>
	/// Description r�sum�e de ClassificationDescription.
	/// </summary>
	public class ClassificationDescription{
		#region Variables
		/// <summary>
		/// Type du niveau
		/// </summary>
		private ClassificationTable.name _levelType=ClassificationTable.name.sector;
		/// <summary>
		/// Texte de pr�sentation
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
		/// Obtient ou d�finit le type du niveau
		/// </summary>
		public ClassificationTable.name LevelType{
			get{return(_levelType);}
			set{_levelType=value;}
		}

		/// <summary>
		/// Obtient ou d�finit le texte
		/// </summary>
		public Int64 labelTextId{
			get{return(_labelTextId);}
			set{_labelTextId=value;}
		}
		#endregion
	}
}
