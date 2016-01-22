#region Informations
// Auteur: G. Facon 
// Date de création: 05/12/2005
// Date de modification: 
#endregion

using System;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpressI.MediaSchedule{
	/// <summary>
	/// Information of a media schedule cellInformations contenues dans une cellule d'un plan media
	/// </summary>
	public class MediaPlanItem{

		#region Variables
		/// <summary>
		/// Element Periodicity
		/// </summary>
		protected Int64 _periodicityId=0;
		/// <summary>
		/// Graphical element of a media schedule
		/// </summary>
		protected MediaPlan.graphicItemType _graphicItemType = MediaPlan.graphicItemType.absent;
		/// <summary>
		/// Unit value 
		/// </summary>
		protected double _unit=(double)0.0;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		public MediaPlanItem(){
		}
		/// <summary>
        /// Constructor
		/// </summary>
		/// <param name="periodicityId">Cell periodicity</param>
		public MediaPlanItem(Int64 periodicityId){
			_periodicityId=periodicityId;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Get / Set periodicity Id
		/// </summary>
		public Int64 PeriodicityId{
			set{_periodicityId=value;}
			get{return(_periodicityId);}
		}

		/// <summary>
		/// Get / Set Cell type
		/// </summary>
		public MediaPlan.graphicItemType GraphicItemType{
			set{_graphicItemType=value;}
			get{return(_graphicItemType);}
		}

		/// <summary>
		/// Get / Set cell value
		/// </summary>
		public double Unit{
			set{_unit=value;}
			get{return(_unit);}
		}

		#endregion

		#region Méthodes Externes
		/// <summary>
		/// Periodicity as a string
		/// </summary>
		/// <returns>Label</returns>
		public override string ToString() {
			return(_periodicityId.ToString()+":"+_unit.ToString());
		}

		#endregion
	}

    public class MediaPlanItemIds : MediaPlanItem {

        #region Properties
        /// <summary>
        /// Specific unit for distinct ids counting
        /// </summary>
        protected CellIdsNumber _idsNumber = new CellIdsNumber();
        /// <summary>
        /// Get Specific unit for distinct ids counting
        /// </summary>
        public CellIdsNumber IdsNumber {
            get {return _idsNumber;}
        }
        #endregion

        #region Constructor
        /// <summary>
		/// Constructor
		/// </summary>
		public MediaPlanItemIds():base(){
		}
		/// <summary>
        /// Constructor
		/// </summary>
		/// <param name="periodicityId">Cell periodicity</param>
        public MediaPlanItemIds(Int64 periodicityId): base(periodicityId) {
			_periodicityId=periodicityId;
		}
		#endregion

    }
}
