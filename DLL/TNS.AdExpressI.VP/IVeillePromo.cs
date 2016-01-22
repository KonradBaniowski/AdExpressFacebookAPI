using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpressI.VP
{
    public interface IVeillePromo
    {

        

        #region Properties
        /// <summary>
        /// Define Current Module
        /// </summary>
        string ResultControlId
        {
            set;
        }
        /// <summary>
        /// Define Theme
        /// </summary>
        string Theme
        {
            set;
        }
        /// <summary>
        /// Get Period Beginning Date 
        /// </summary>
        string PeriodBeginningDate
        {
            get;
        }

        /// <summary>
        /// Get Period EndDate
        /// </summary>
         string PeriodEndDate
        {
            get;
        }
        #endregion

          /// <summary>
        /// Get Data
        /// </summary>
        /// <returns>data promo</returns>
        VeillePromoScheduleData GetData();

          /// <summary>
        /// Get HTML code for the promotion schedule
        /// </summary>
        /// <returns>HTML Code</returns>
        string GetHtml();


        /// <summary>
        /// Get HTML code for the promotion file
        /// </summary>
        /// <param name="idDataPromotion">Promotion Id</param>
        /// <returns>HTML Code</returns>
        string GetPromoFileHtml();
        /// <summary>
        /// Get HTML code for the promotion file
        /// </summary>
        /// <param name="idDataPromotion">Promotion Id</param>
        /// <returns>HTML Code</returns>
        Dictionary<string, List<string>> GetPromoFileList();

         /// <summary>
        /// Split text Content
        /// </summary>
        /// <param name="promoContent">promo Content</param>
        /// <param name="nbCol">nb Col </param>
        /// <returns>Split text </returns>
        string SplitContent(string promoContent, int nbCol);
    }
}
