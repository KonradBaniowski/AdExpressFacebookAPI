using System;
using System.Collections.Generic;
using System.Text;
using VpCst = TNS.AdExpress.Constantes.FrameWork.Results;

namespace TNS.AdExpressI.VP
{
    public class VeillePromoItem
    {

        #region Variables
        /// <summary>
        /// item presence of a VP schedule
        /// </summary>
        protected VpCst.VeillePromo.itemType _itemType = VpCst.VeillePromo.itemType.absent;
          /// <summary>
        /// Id data promotion
        /// </summary>
        protected long _idDataPromotion;

        /// <summary>
        /// Date begin
        /// </summary>
        protected DateTime _dateBegin;


        /// <summary>
        /// Date End
        /// </summary>
        protected DateTime _dateEnd;
     

        /// <summary>
        /// Promotion content
        /// </summary>
        protected string _promotionContent;

        ///<summary>Css Class</summary>        
        protected string _cssClass;

        #endregion

        #region Accessors
        /// <summary>
        /// Get Item type
        /// </summary>
        public VpCst.VeillePromo.itemType ItemType
        {
            get { return _itemType; }
        }
          /// <summary>
        /// Get Id data promotion
        /// </summary>
        public long IdDataPromotion
        {
            get { return _idDataPromotion; }
        }
      
        /// <summary>
        /// Get Date begin
        /// </summary>
        public DateTime DateBegin
        {
            get { return _dateBegin; }
        }

        /// <summary>
        /// Get Date End
        /// </summary>
        public DateTime DateEnd
        {
            get { return _dateEnd; }
        }
                  
        /// <summary>
        /// Get Promotion content
        /// </summary>
        public string PromotionContent
        {
            get { return _promotionContent; }
        }



        ///<summary>Get CssClass</summary>     
        public string CssClass
        {
            get
            {
                return (_cssClass);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemType">Item type</param>      
        public VeillePromoItem(VpCst.VeillePromo.itemType itemType)
        {
            _itemType = itemType;
           
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemType">Item type</param>
        /// <param name="idDataPromotion">ID Data Promotion</param>      
        /// <param name="dateBegin">Date Beginning </param>
        /// <param name="dateEnd">Date End</param>       
        /// <param name="promotionContent">Promotion Content</param>
        public VeillePromoItem(VpCst.VeillePromo.itemType itemType, long idDataPromotion, DateTime dateBegin, DateTime dateEnd, string promotionContent,string cssClass) :this(itemType){
                _idDataPromotion = idDataPromotion;                  
            _dateBegin = dateBegin;
            _dateEnd = dateEnd;           
            _promotionContent = promotionContent;
            _cssClass = cssClass;
        }
        #endregion
    }
}
