using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;

namespace TNS.AdExpressI.Celebrities
{
    public interface ICelebritiesResult
    {
        #region Properties
        /// <summary>
        /// Define Current Module
        /// </summary>
        TNS.AdExpress.Domain.Web.Navigation.Module Module
        {
            get;
            set;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Get HTML code for the Celebrities report
        /// </summary>
        /// <returns>HTML Code</returns>
        CelebritiesData GetHtml();
       
        /// <summary>
        /// Get HTML code for a pdf export of the Celebrities report
        /// </summary>
        /// <returns>HTML Code</returns>
        CelebritiesData GetPDFHtml();
        /// <summary>
        /// Get HTML code for an excel export of the Celebrities report
        /// </summary>
        /// <param name="withValues">Specify if each values of the calendar must be shown in Celebrities report</param>
        /// <returns>HTML Code</returns>
        CelebritiesData GetExcelHtml(bool withValues);       
        /// <summary>
        /// Get Raw data for an excel export by LS plugin of the Celebrities report
        /// </summary>
        void GetRawData(Workbook excel, TNS.FrameWork.WebTheme.Style style);
        #endregion
    }
}
