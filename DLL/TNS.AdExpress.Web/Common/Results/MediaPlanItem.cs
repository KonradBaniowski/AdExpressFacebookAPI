#region Informations
// Auteur: G. Facon 
// Date de création: 05/12/2005
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
		/// Périodicité de l'élément
		/// </summary>
		protected Int64 _periodicityId=0;
		/// <summary>
		/// Element graphique représentant la cellule dans un plan média
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
		/// <param name="periodicityId">Périodicité de la cellule</param>
		public MediaPlanItem(Int64 periodicityId){
			_periodicityId=periodicityId;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Définit ou obtient l'identifiant de la périodicité
		/// </summary>
		public Int64 PeriodicityId{
			set{_periodicityId=value;}
			get{return(_periodicityId);}
		}

		/// <summary>
		/// Définit ou obtient la façon d'afficher la cellule
		/// </summary>
		public FrameWorkConstantes.Results.MediaPlan.graphicItemType GraphicItemType{
			set{_graphicItemType=value;}
			get{return(_graphicItemType);}
		}

		/// <summary>
		/// Définit ou obtient la valeur de l'unité contenue dans la cellule
		/// </summary>
		public double Unit{
			set{_unit=value;}
			get{return(_unit);}
		}

		#endregion

		#region Méthodes Externes
		/// <summary>
		/// La périodicité sous forme de chaîne de caratères
		/// </summary>
		/// <returns>La périodicité sous forme de chaîne de caratères</returns>
		public override string ToString() {
			return(_periodicityId.ToString()+":"+_unit.ToString());
		}

		#endregion
	}
}
