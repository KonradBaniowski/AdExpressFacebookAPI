using System;
using TNS.AdExpress.Constantes.FrameWork.Results;

namespace TNS.AdExpressI.Rolex
{
  public  class RolexItem
    {

      #region Variables
		
		/// <summary>
		/// Graphical element of a media schedule
		/// </summary>
		protected MediaPlan.graphicItemType _graphicItemType = MediaPlan.graphicItemType.absent;

     

      #endregion

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		public RolexItem(){
		}
        /// <summary>
        /// Constructor
        /// </summary>
        public RolexItem(MediaPlan.graphicItemType graphicItemType,long level1, long level2, long level3, DateTime dateBegin, DateTime dateEnd)
        {
            Level1 = level1;
            Level2 = level2;
            Level3 = level3;
            DateBegin = dateBegin;
            DateEnd = dateEnd;
            _graphicItemType = graphicItemType;
           
        }

      #endregion

		#region Accessors

        public long Level1 { get; set; }
        public long Level2 { get; set; }
        public long Level3 { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }

		/// <summary>
		/// Get / Set Cell type
		/// </summary>
		public MediaPlan.graphicItemType GraphicItemType{
			set{_graphicItemType=value;}
			get{return(_graphicItemType);}
		}

		
		#endregion

		
    }
}
