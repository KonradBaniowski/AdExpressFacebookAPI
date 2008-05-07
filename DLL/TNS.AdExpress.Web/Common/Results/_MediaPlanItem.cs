#region Informations
// Auteur: G. Facon 
// Date de cr�ation: 05/12/2005
// Date de modification: 
#endregion

using System;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;

namespace TNS.AdExpress.Web.Core{
	/// <summary>
	/// Informations contenues dans une cellule d'un plan media
	/// </summary>
	public class MediaPlanItem{

		#region Variables
		/// <summary>
		/// P�riodicit� de l'�l�ment
		/// </summary>
		protected Int64 _periodicityId=0;
		/// <summary>
		/// Element graphique repr�sentant la cellule dans un plan m�dia
		/// </summary>
		protected FrameWorkConstantes.Results.MediaPlan.graphicItemType _graphicItemType=FrameWorkConstantes.Results.MediaPlan.graphicItemType.absent;
		/// <summary>
		/// Valeur de l'investissement
		/// </summary>
		protected double _unit=(double)0.0;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public MediaPlanItem(){
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="periodicityId">P�riodicit� de la cellule</param>
		public MediaPlanItem(Int64 periodicityId){
			_periodicityId=periodicityId;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// D�finit ou obtient l'identifiant de la p�riodicit�
		/// </summary>
		public Int64 PeriodicityId{
			set{_periodicityId=value;}
			get{return(_periodicityId);}
		}

		/// <summary>
		/// D�finit ou obtient la fa�on d'afficher la cellule
		/// </summary>
		public FrameWorkConstantes.Results.MediaPlan.graphicItemType GraphicItemType{
			set{_graphicItemType=value;}
			get{return(_graphicItemType);}
		}

		/// <summary>
		/// D�finit ou obtient la valeur de l'unit� contenue dans la cellule
		/// </summary>
		public double Unit{
			set{_unit=value;}
			get{return(_unit);}
		}

		#endregion

		#region M�thodes Externes
		/// <summary>
		/// La p�riodicit� sous forme de cha�ne de carat�res
		/// </summary>
		/// <returns>La p�riodicit� sous forme de cha�ne de carat�res</returns>
		public override string ToString() {
			return(_periodicityId.ToString()+":"+_unit.ToString());
		}

		#endregion
	}
}
