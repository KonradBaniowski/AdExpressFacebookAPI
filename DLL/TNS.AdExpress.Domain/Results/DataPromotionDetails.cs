using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Results
{
    public class DataPromotionDetails {

        #region Variables
        /// <summary>
        /// Date Traitment
        /// </summary>
        protected DateTime _dateTraitment;
        /// <summary>
        /// Data Promotion Detail List
        /// </summary>
        protected List<DataPromotionDetail> _dataPromotionDetailList;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dateTraitment">Date Traitment</param>
        /// <param name="dataPromotionDetailList">Data Promotion Detail List</param>
        public DataPromotionDetails(DateTime dateTraitment, List<DataPromotionDetail> dataPromotionDetailList) {
            if (dataPromotionDetailList == null) throw new ArgumentNullException(" Paramter dataPromotionDetailList is null");
            _dateTraitment = dateTraitment;
            _dataPromotionDetailList = dataPromotionDetailList;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Date Traitment
        /// </summary>
        public DateTime DateTraitment {
            get { return _dateTraitment; }
        }
        /// <summary>
        /// Get Data Promotion Detail List
        /// </summary>
        public List<DataPromotionDetail> DataPromotionDetailList {
            get { return _dataPromotionDetailList; }
        }
        #endregion

    }
}
