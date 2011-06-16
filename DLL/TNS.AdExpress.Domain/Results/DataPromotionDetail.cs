using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Results
{
    public class DataPromotionDetail {

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
        protected DateTime _dateBegin;


        /// <summary>
        /// Date End
        /// </summary>
        protected DateTime _dateEnd;

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
        /// Promorion visual List
        /// </summary>
        protected List<string> _conditionVisual;


        /// <summary>
        /// Condition text
        /// </summary>
        protected string _conditionText;

        /// <summary>
        /// Condition Brand
        /// </summary>
        protected string _promotionBrand;


        /// <summary>
        /// promotion visual List
        /// </summary>
        protected List<string> _promotionVisual;


        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="idProduct">ID Product</param>
        /// <param name="idBrand">ID brand</param>
        /// <param name="dateBegin">Date Beginning </param>
        /// <param name="dateEnd">Date End</param>
        /// <param name="idSegment">ID Segment</param>
        /// <param name="idCategory">ID category</param>
        /// <param name="idCircuit">ID circuit</param>
        /// <param name="promotionContent">Promotion Content</param>
        /// <param name="conditionVisual">Condition Visual</param>
        /// <param name="conditionText">Condition Text</param>
        /// <param name="promotionBrand">Promotion Brand</param>
        /// <param name="promotionVisual">Promotion Visual</param>
        public DataPromotionDetail(long idProduct, long idBrand, DateTime dateBegin, DateTime dateEnd, long idSegment, long idCategory, long idCircuit
            , string promotionContent, List<string> conditionVisual, string conditionText, string promotionBrand, List<string> promotionVisual) {
            if (idProduct < 1) throw new ArgumentException(" Paramter idProduct  is invalid");
            _idProduct = idProduct;
            if (idBrand < 1) throw new ArgumentException(" Paramter idBrand  is invalid");
            _idBrand = idBrand;
            _dateBegin = dateBegin;
            _dateEnd = dateEnd;
            if (idSegment < 1) throw new ArgumentException(" Paramter idSegment is invalid");
            _idSegment = idSegment;
            if (idCategory < 1) throw new ArgumentException(" Paramter idCatgory is invalid");
            _idCategory = idCategory;
            if (idCircuit < 1) throw new ArgumentException(" Paramter idCircuit is invalid");
            _idCircuit = idCircuit;
            _promotionContent = promotionContent;
            _conditionVisual = conditionVisual;
            _conditionText = conditionText;
            _promotionBrand = promotionBrand;
            _promotionVisual = promotionVisual;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Id product
        /// </summary>
        public long IdProduct {
            get { return _idProduct; }
        }
        /// <summary>
        /// Get ID Brand
        /// </summary>
        public long IdBrand {
            get { return _idBrand; }
        }
        /// <summary>
        /// Get Date begin
        /// </summary>
        public DateTime DateBegin {
            get { return _dateBegin; }
        }

        /// <summary>
        /// Get Date End
        /// </summary>
        public DateTime DateEnd {
            get { return _dateEnd; }
        }
        /// <summary>
        /// Get ID segment
        /// </summary>
        public long IdSegment {
            get { return _idSegment; }
        }
        /// <summary>
        /// ID category
        /// </summary>
        public long IdCategory {
            get { return _idCategory; }
        }
        /// <summary>
        /// ID circuit
        /// </summary>
        public long IdCircuit {
            get { return _idCircuit; }
        }
        /// <summary>
        /// Get Promotion content
        /// </summary>
        public string PromotionContent {
            get { return _promotionContent; }
        }
        /// <summary>
        /// Promotion visual 
        /// </summary>
        public List<string> ConditionVisual {
            get { return _conditionVisual; }
        }
        /// <summary>
        /// Get Condition text
        /// </summary>
        public string ConditionText {
            get { return _conditionText; }
        }
        /// <summary>
        /// Get Condition Brand
        /// </summary>
        public string PromotionBrand {
            get { return _promotionBrand; }
        }
        /// <summary>
        /// Get promotion visual
        /// </summary>
        public List<string> PromotionVisual {
            get { return _promotionVisual; }
        }

        #endregion

    }
}
