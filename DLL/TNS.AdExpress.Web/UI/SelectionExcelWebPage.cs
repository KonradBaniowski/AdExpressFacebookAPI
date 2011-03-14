using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Web.UI
{
    public class SelectionExcelWebPage: ExcelWebPage
    {

        #region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
        public SelectionExcelWebPage()
            : base()
        {
					
		}
		#endregion

        /// <summary>
        /// Set Selection Error variable
        /// Check if the study univers are selected for the result page (if not we redirect the user to the error page)
        /// </summary>
        protected override void SetSelectionError()
        {
           
        }
    }
}
