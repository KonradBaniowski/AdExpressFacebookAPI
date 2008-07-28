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
using TNS.AdExpress.Web.Core.Sessions;

using CstTblFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables;
using TNS.Classification.Universe;
using TNS.AdExpressI.ProductClassReports.Engines;

namespace TNS.AdExpressI.ProductClassReports
{
    /// <summary>
    /// Implements default results of the Product Class Analysis.
    /// </summary>
    public abstract class ProductClassReports : IProductClassReports
    {

        #region Attributes
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Type of result
        /// </summary>
        protected int _tableType;
        /// <summary>
        /// Report engine
        /// </summary>
        protected Engine _engine = null;
        #endregion

        #region Accessors
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession Session
        {
            get{return _session;}
            set { _session = value; }
        }
        /// <summary>
        /// Type of result
        /// </summary>
        protected int TableType
        {
            get { return _tableType; }
            set { _tableType = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassReports(WebSession session)
        {
            _session = session;
        }
        #endregion

        #region IProductClassReports Membres
        /// <summary>
        /// Compute Product Class Report depending on type or fesult specified in user session (HTML code)
        /// </summary>
        /// <returns>HTML Code</returns>
        public string GetProductClassReport()
        {
            return GetProductClassReport((int)_session.PreformatedTable);
        }
        /// <summary>
        /// Compute Product Class Report matching the "resultType" param (HTML code)
        /// </summary>
        /// <param name="resultType">Type of result</param>
        /// <exception cref="TNS.AdExpressI.ProductClassReports.Exceptions.NotImplementedReportException">Thrown when the invoked report is not implemented.</exception>
        /// <returns>HTML Code</returns>
        public string GetProductClassReport(int resultType)
        {

            switch (resultType)
            {
                case (int)CstTblFormat.media_X_Year:
                case (int)CstTblFormat.product_X_Year:
                    _engine = new Engine_Classif1_X_Year(_session, resultType);
                    break;
                case (int)CstTblFormat.productMedia_X_Year:
                case (int)CstTblFormat.mediaProduct_X_Year:
                    _engine = new Engine_Classif1Classif2_X_Years(_session, resultType);
                    break;
                case (int)CstTblFormat.productMedia_X_YearMensual:
                case (int)CstTblFormat.mediaProduct_X_YearMensual:
                     _engine = new Engine_Classif1Classif2_X_Monthes(_session, resultType);
                    break;
                case (int)CstTblFormat.productYear_X_Media:
                     _engine = new Engine_Classif1Year_X_Classif2(_session, resultType);
                    break;
                case (int)CstTblFormat.mediaYear_X_Mensual:
                case (int)CstTblFormat.mediaYear_X_Cumul:
                case (int)CstTblFormat.productYear_X_Cumul:
                case (int)CstTblFormat.productYear_X_Mensual:
                    _engine = new Engine_Classif1Year_X_Monthes(_session, resultType);
                    break;
                default:
                    throw new NotImplementedReportException(string.Format("Tableau {0} ({1}) is not implemented.", _session.PreformatedTable, _session.PreformatedTable.GetHashCode()));
            }
            return _engine.GetResult().ToString();

        }
        /// <summary>
        /// Compute Product Class Report depending on type or fesult specified in user session (HTML code)
        /// </summary>
        /// <returns>HTML Code</returns>
        public string GetProductClassReportExcel()
        {
            return GetProductClassReportExcel((int)_session.PreformatedTable);
        }
        /// <summary>
        /// Compute Product Class Report matching the "resultType" param (HTML code)
        /// </summary>
        /// <param name="resultType">Type of result</param>
        /// <exception cref="TNS.AdExpressI.ProductClassReports.Exceptions.NotImplementedReportException">Thrown when the invoked report is not implemented.</exception>
        /// <returns>HTML Code</returns>
        public string GetProductClassReportExcel(int resultType)
        {

            switch (resultType)
            {
                case (int)CstTblFormat.media_X_Year:
                case (int)CstTblFormat.product_X_Year:
                    //GetDynamicTableUI_1_2(webSession, html, data, excel);
                    break;
                case (int)CstTblFormat.productMedia_X_Year:
                case (int)CstTblFormat.mediaProduct_X_Year:
                    //GetDynamicTableUI_3_4_10_11(webSession, html, data, excel, false);
                    break;
                case (int)CstTblFormat.productMedia_X_YearMensual:
                case (int)CstTblFormat.mediaProduct_X_YearMensual:
                    //GetDynamicTableUI_3_4_10_11(webSession, html, data, excel, true);
                    break;
                case (int)CstTblFormat.productYear_X_Media:
                    //GetDynamicTableUI_5(webSession, html, data, excel);
                    break;
                case (int)CstTblFormat.mediaYear_X_Mensual:
                case (int)CstTblFormat.mediaYear_X_Cumul:
                case (int)CstTblFormat.productYear_X_Cumul:
                case (int)CstTblFormat.productYear_X_Mensual:
                    //GetDynamicTableUI_6_7_8_9(webSession, html, data, excel);
                    break;
                default:
                    throw new NotImplementedReportException(string.Format("Tableau {0} ({1}) is not implemented.", _session.PreformatedTable, _session.PreformatedTable.GetHashCode()));
            }

            return string.Empty;

        }

        #endregion
    }
}
