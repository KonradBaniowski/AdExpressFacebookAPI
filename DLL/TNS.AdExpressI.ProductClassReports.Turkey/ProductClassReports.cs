using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.ProductClassReports.Exceptions;
using TNS.AdExpressI.ProductClassReports.GenericEngines;
using TNS.FrameWork.WebResultUI;
using CstTblFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables;

namespace TNS.AdExpressI.ProductClassReports.Turkey
{
    public class ProductClassReports : TNS.AdExpressI.ProductClassReports.ProductClassReports
    {
        public ProductClassReports(WebSession session) : base(session)
        {
        }

        #region Generic GetProductClassReport
        /// <summary>
        /// Compute Product Class Report matching the "resultType" param
        /// </summary>
        /// <param name="resultType">Type of result</param>
        /// <param name="excel">Report as Excel output ? </param>
        /// <exception cref="TNS.AdExpressI.ProductClassReports.Exceptions.NotImplementedReportException">Thrown when the invoked report is not implemented.</exception>
        /// <returns>Data Result</returns>
        protected override ResultTable GetGenericProductClassReport(int resultType, bool excel)
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
                    _genericEngine = new GenericEngines.GenericEngine_Classif1Classif2_X_Monthes(_session, resultType);
                    break;
                case (int)CstTblFormat.productYear_X_Media:
                    _genericEngine = new GenericEngine_Classif1Year_X_Classif2(_session, resultType);
                    break;
                case (int)CstTblFormat.mediaYear_X_Mensual:
                case (int)CstTblFormat.productYear_X_Mensual:
                    _genericEngine = new GenericEngine_Classif1Year_X_Monthes(_session, resultType, true);
                    break;
                case (int)CstTblFormat.mediaYear_X_Cumul:
                case (int)CstTblFormat.productYear_X_Cumul:
                    _genericEngine = new GenericEngine_Classif1Year_X_Monthes(_session, resultType);
                    break;
                default:
                    throw new NotImplementedReportException(
                        $"Tableau {_session.PreformatedTable} ({_session.PreformatedTable.GetHashCode()}) is not implemented.");
            }
            _genericEngine.Excel = excel;
            var result = _genericEngine.GetResult();
            return result;
        }
        #endregion

    }


}
