#region Information
/*
 * Author : G Ragneau
 * Created on : 30/06/2008
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;


using TNS.AdExpressI.ProductClassReports.Exceptions;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Classification;

using CstTblFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables;
using TNS.Classification.Universe;
using TNS.AdExpressI.ProductClassReports.Russia.GenericEngines;
using System.Data;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpressI.ProductClassReports.Russia
{
    /// <summary>
    /// Implements default results of the Product Class Analysis.
    /// </summary>
    public class ProductClassReports : TNS.AdExpressI.ProductClassReports.ProductClassReports
    {
        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassReports(WebSession session):base(session) {
        }
        #endregion

        #region Generic GetProductClassReport
        /// <summary>
        /// Compute Product Class Report matching the "resultType" param
        /// </summary>
        /// <param name="resultType">Type of result</param>
        /// <param name="excel">Report as Excel output ? </param>
        /// <exception cref="TNS.AdExpressI.ProductClassReports.Exceptions.NotImplementedReportException">Thrown when the invoked report is not implemented.</exception>
        /// <returns>Data Result</returns>
        override protected ResultTable GetGenericProductClassReport(int resultType, bool excel)
        {
            switch (resultType)
            {
                case (int)CstTblFormat.media_X_Year:
                case (int)CstTblFormat.product_X_Year:
                    _genericEngine = new GenericEngine_Classif1_X_Year(_session, resultType);
                    break;
                case (int)CstTblFormat.productMedia_X_Year:
                case (int)CstTblFormat.mediaProduct_X_Year:
                    _genericEngine = new GenericEngine_Classif1Classif2_X_Years(_session, resultType);
                    break;
                case (int)CstTblFormat.productMedia_X_YearMensual:
                case (int)CstTblFormat.mediaProduct_X_YearMensual:
                    _genericEngine = new GenericEngine_Classif1Classif2_X_Monthes(_session, resultType);
                    break;
                case (int)CstTblFormat.productYear_X_Media:
                    _genericEngine = new GenericEngine_Classif1Year_X_Classif2(_session, resultType);
                    break;
                case (int)CstTblFormat.mediaYear_X_Mensual:
                case (int)CstTblFormat.productYear_X_Mensual:
                    _genericEngine = new GenericEngine_Classif1Year_X_Monthes(_session, resultType,true);
                    break;
                case (int)CstTblFormat.mediaYear_X_Cumul:
                case (int)CstTblFormat.productYear_X_Cumul:
                    _genericEngine = new GenericEngine_Classif1Year_X_Monthes(_session, resultType);
                    break;
                default:
                    throw new NotImplementedReportException(string.Format("Tableau {0} ({1}) is not implemented.", _session.PreformatedTable, _session.PreformatedTable.GetHashCode()));
            }
            _genericEngine.Excel = excel;
            return _genericEngine.GetResult();
        }
        #endregion

    }
}
