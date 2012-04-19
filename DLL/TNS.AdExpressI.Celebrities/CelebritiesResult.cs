using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using TNS.AdExpress.Domain.Web.Navigation;
using Style = TNS.FrameWork.WebTheme.Style;

namespace TNS.AdExpressI.Celebrities
{
    public class CelebritiesResult : ICelebritiesResult
    {
        #region Implementation of ICelebritiesResult

        /// <summary>
        /// Define Current Module
        /// </summary>
        public Module Module
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Get HTML code for the Celebrities report
        /// </summary>
        /// <returns>HTML Code</returns>
        public CelebritiesData GetHtml()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get HTML code for a pdf export of the Celebrities report
        /// </summary>
        /// <returns>HTML Code</returns>
        public CelebritiesData GetPDFHtml()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get HTML code for an excel export of the Celebrities report
        /// </summary>
        /// <param name="withValues">Specify if each values of the calendar must be shown in Celebrities report</param>
        /// <returns>HTML Code</returns>
        public CelebritiesData GetExcelHtml(bool withValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Raw data for an excel export by LS plugin of the Celebrities report
        /// </summary>
        public void GetRawData(Workbook excel, Style style)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
