using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Results
{
    public class DataPromotionDetail
    {
        #region Variables
        /// <summary>
        /// ID product
        /// </summary>
        protected long _idProduct;
       
        /// <summary>
        /// ID brand
        /// </summary>
        protected long _idBrand;

        
        /// <summary>
        /// Date begin
        /// </summary>
        protected long _dateBegin;

      
        /// <summary>
        /// Date End
        /// </summary>
        protected long _dateEnd;
       
        /// <summary>
        /// ID segment
        /// </summary>
        protected long _idSegment;

        /// <summary>
        /// ID category
        /// </summary>
        protected long _idCategory;
      
        /// <summary>
        /// ID circuit
        /// </summary>
        protected long _idCircuit;
      
        /// <summary>
        /// Promotion content
        /// </summary>
        protected string _promotionContent;
       
        /// <summary>
        /// Promorion visual 
        /// </summary>
        protected string _conditionVisual;

      
        /// <summary>
        /// Condition text
        /// </summary>
        protected string _conditionText;
       
        /// <summary>
        /// Condition Brand
        /// </summary>
        protected string _promotionBrand;

     
        /// <summary>
        /// promotion visual
        /// </summary>
        protected string _promotionVisual;

     
        #endregion

        #region Accessors
        /// <summary>
        /// Get \Set Id product
        /// </summary>
         public long IdProduct
        {
            get { return _idProduct; }
            set { _idProduct = value; }
        }
         /// <summary>
         /// Get \Set ID Brand
         /// </summary>
         public long IdBrand
         {
             get { return _idBrand; }
             set { _idBrand = value; }
         }
         /// <summary>
         /// Get \Set Date begin
         /// </summary>
         public long DateBegin
         {
             get { return _dateBegin; }
             set { _dateBegin = value; }
         }

         /// <summary>
         /// Get \Set Date End
         /// </summary>
         public long DateEnd
         {
             get { return _dateEnd; }
             set { _dateEnd = value; }
         }
         /// <summary>
         /// Get \Set ID segment
         /// </summary>
         public long IdSegment
         {
             get { return _idSegment; }
             set { _idSegment = value; }
         }
         /// <summary>
         /// ID category
         /// </summary>
         public long IdCategory
         {
             get { return _idCategory; }
             set { _idCategory = value; }
         }
         /// <summary>
         /// ID circuit
         /// </summary>
         public long IdCircuit
         {
             get { return _idCircuit; }
             set { _idCircuit = value; }
         }
         /// <summary>
         /// Get \Set Promotion content
         /// </summary>
         public string PromotionContent
         {
             get { return _promotionContent; }
             set { _promotionContent = value; }
         }
         /// <summary>
         /// Promorion visual 
         /// </summary>
         public string ConditionVisual
         {
             get { return _conditionVisual; }
             set { _conditionVisual = value; }
         }
         /// <summary>
         /// Get \Set Condition text
         /// </summary>
         public string ConditionText
         {
             get { return _conditionText; }
             set { _conditionText = value; }
         }
         /// <summary>
         /// Get \Set Condition Brand
         /// </summary>
         public string PromotionBrand
         {
             get { return _promotionBrand; }
             set { _promotionBrand = value; }
         }
         /// <summary>
         /// Get \Set promotion visual
         /// </summary>
         public string PromotionVisual
         {
             get { return _promotionVisual; }
             set { _promotionVisual = value; }
         }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
         /// <param name="idProduct">ID Product</param>
        /// <param name="idBrand">ID brand</param>
         /// <param name="dateBegin">Date Beginning </param>
        /// <param name="dateEnd">Date End</param>
        /// <param name="idSegment">ID Segment</param>
        /// <param name="idCatgory">ID category</param>
        /// <param name="idCircuit">ID circuit</param>
         /// <param name="promotionContent">Promotion Content</param>
         /// <param name="conditionVisual">Condition Visual</param>
        /// <param name="conditionText">Condition Text</param>
         /// <param name="promotionBrand">Promotion Brand</param>
         /// <param name="promotionVisual">Promotion Visual</param>
         public DataPromotionDetail(long idProduct, long idBrand, long dateBegin, long dateEnd,long idSegment, long idCatgory, long idCircuit
             , string promotionContent, string conditionVisual, string conditionText, string promotionBrand, string promotionVisual)
         {
             if (idProduct < 1) throw new ArgumentException(" Paramter idProduct  is invalid");
             _idProduct = idProduct;
             if (idBrand < 1) throw new ArgumentException(" Paramter idBrand  is invalid");
             _idBrand = idBrand;
             if (dateBegin.ToString().Length != 8) throw new ArgumentException(" Paramter dateBegin is invalid");
             _dateBegin = dateBegin;
             if (dateEnd.ToString().Length != 8) throw new ArgumentException(" Paramter dateEnd is invalid");
             _dateEnd = dateEnd;
             if (idSegment < 1) throw new ArgumentException(" Paramter idSegment is invalid");
             _idSegment = idSegment;
             if (idCatgory < 1) throw new ArgumentException(" Paramter idCatgory is invalid");
             _idCategory = idCatgory;
             if (idCircuit < 1) throw new ArgumentException(" Paramter idCircuit is invalid");
             _idCircuit = idCircuit;
             if (string.IsNullOrEmpty(promotionContent)) throw new ArgumentNullException(" Paramter promotionContent canot be null");
             _promotionContent = promotionContent;
             _conditionVisual = conditionVisual;
             _conditionText = conditionText;
             _promotionBrand = promotionBrand;
             _promotionVisual = promotionVisual;
         }
    }
}
