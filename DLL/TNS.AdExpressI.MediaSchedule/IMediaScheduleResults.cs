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


using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.MediaSchedule;
using Aspose.Cells;
using TNS.AdExpress.Domain.Theme;

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
        /// Get HTML code for an excel export of the media schedule dedicated to creative division
		/// </summary>
        /// <param name="withValues">Specify if each values of the calendar must be shown in Media Schedule</param>
		/// <returns>HTML Code</returns>
        MediaScheduleData GetExcelHtmlCreativeDivision(bool withValues);
        /// <summary>
        /// Get Raw data for an excel export by Anubis of the media schedule
        /// </summary>
        void GetRawData(Workbook excel, TNS.AdExpress.Domain.Theme.Style style);
        #endregion
    }
}
