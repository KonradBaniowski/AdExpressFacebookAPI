using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Results
{
    public class InsertionOptions
    {
        /// <summary>
        /// is All Period Is RestrictED To 4 MonthES
        /// </summary>
        private bool _isAllPeriodIsRestrictTo4Month = false;

       
        /// <summary>
        ///get if  Can Save Levels
        /// </summary>
        private bool _canSaveLevels = false;
        
        /// <summary>
        ///Get if  Can Save Levels
        /// </summary>
        public bool CanSaveLevels
        {
            get { return _canSaveLevels; }
        }
        /// <summary>
        /// Get if is All Period Is RestrictED To 4 MonthES
        /// </summary>
        public bool IsAllPeriodIsRestrictTo4Month
        {
            get { return _isAllPeriodIsRestrictTo4Month; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isAllPeriodIsRestrictTo4Month"></param>
        /// <param name="canSaveLevels"></param>
        public InsertionOptions(bool isAllPeriodIsRestrictTo4Month, bool canSaveLevels)
        {
            _canSaveLevels = canSaveLevels;
            _isAllPeriodIsRestrictTo4Month = isAllPeriodIsRestrictTo4Month;
        }
    }
}
