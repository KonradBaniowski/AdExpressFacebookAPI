#region Information
/*
 * Author : G Ragneau
 * Created on : 28/04/2008
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System.Collections.Generic;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.MediaSchedule;
using Aspose.Cells;
using TNS.FrameWork.WebTheme;

namespace TNS.AdExpressI.MediaSchedule{
    
    /// <summary>
    /// Media Schedule result Interface
    /// </summary>
    public interface IMediaScheduleResults
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
        /// <summary>
        /// Get /Set Active periods
        /// </summary>
         List<string> ActivePeriods
        {
            get; set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get HTML code for the media schedule
		/// </summary>
		/// <returns>HTML Code</returns>
        MediaScheduleData GetHtml();
        /// <summary>
        /// Get HTML code for the media schedule dedicated to Creative Division
		/// </summary>
		/// <returns>HTML Code</returns>
        MediaScheduleData GetHtmlCreativeDivision();
        /// <summary>
        /// Get HTML code for a pdf export of the media schedule
		/// </summary>
		/// <returns>HTML Code</returns>
        MediaScheduleData GetPDFHtml();
        /// <summary>
        /// Get HTML code for an excel export of the media schedule
		/// </summary>
        /// <param name="withValues">Specify if each values of the calendar must be shown in Media Schedule</param>
		/// <returns>HTML Code</returns>
        MediaScheduleData GetExcelHtml(bool withValues);
        /// <summary>
        /// Get HTML code for an raw excel export of the media schedule
        /// </summary>
        /// <param name="withValues">Specify if each values of the calendar must be shown in Media Schedule</param>
        /// <returns>HTML Code</returns>
        MediaScheduleData GetRawExcel(bool withValues);
        /// <summary>
        /// Get HTML code for an excel export of the media schedule dedicated to creative division
		/// </summary>
        /// <param name="withValues">Specify if each values of the calendar must be shown in Media Schedule</param>
		/// <returns>HTML Code</returns>
        MediaScheduleData GetExcelHtmlCreativeDivision(bool withValues);
        /// <summary>
        /// Get Raw data for an excel export by Anubis of the media schedule
        /// </summary>
        void GetRawData(Workbook excel, TNS.FrameWork.WebTheme.Style style);
        #endregion
    }
}
