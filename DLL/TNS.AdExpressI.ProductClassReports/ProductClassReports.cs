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
using TNS.AdExpressI.ProductClassReports.Engines;
using System.Data;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpressI.ProductClassReports.GenericEngines;

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
        /// <summary>
        /// Generic report engine
        /// </summary>
        protected GenericEngine _genericEngine = null;
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
            return this.GetProductClassReport(resultType, false);
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
            return this.GetProductClassReport(resultType, true);
        }
        #endregion

        #region IProductClassReports Generic Membres
        /// <summary>
        /// Compute Product Class Report depending on type or fesult specified in user session
        /// </summary>
        /// <returns>Data Result</returns>
        public ResultTable GetGenericProductClassReport()
        {
            return GetGenericProductClassReport((int)_session.PreformatedTable);
        }
        /// <summary>
        /// Compute Product Class Report matching the "resultType" param
        /// </summary>
        /// <param name="resultType">Type of result</param>
        /// <exception cref="TNS.AdExpressI.ProductClassReports.Exceptions.NotImplementedReportException">Thrown when the invoked report is not implemented.</exception>
        /// <returns>Data Result</returns>
        public ResultTable GetGenericProductClassReport(int resultType)
        {
            return this.GetGenericProductClassReport(resultType, false);
        }
        /// <summary>
        /// Compute Product Class Report depending on type or fesult specified in user session
        /// </summary>
        /// <returns>Data Result</returns>
        public ResultTable GetGenericProductClassReportExcel()
        {
            return GetGenericProductClassReportExcel((int)_session.PreformatedTable);
        }
        /// <summary>
        /// Compute Product Class Report matching the "resultType" param
        /// </summary>
        /// <param name="resultType">Type of result</param>
        /// <exception cref="TNS.AdExpressI.ProductClassReports.Exceptions.NotImplementedReportException">Thrown when the invoked report is not implemented.</exception>
        /// <returns>Data Result</returns>
        public ResultTable GetGenericProductClassReportExcel(int resultType)
        {
            return this.GetGenericProductClassReport(resultType, true);
        }

		/// <summary>
		/// Determine if module bridge can be show
		/// </summary>
		/// <returns>True if module bridge can be show</returns>
		public bool ShowModuleBridge() {
			List<Int64> ids = null;
			if(_session.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR) != null)
			ids = ((Module)_session.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR)).ExcludedVehicles;

			//Rule : Not show module brige if media selected is only Marketing direct
			if (ids != null && ids.Count > 0) {
				VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID);
				if (vehicleInfo != null && vehicleInfo.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing)
					return false;
			}
			return true;
		}

        #endregion

        #region GetProductClassReport
        /// <summary>
        /// Compute Product Class Report matching the "resultType" param (HTML code)
        /// </summary>
        /// <param name="resultType">Type of result</param>
        /// <param name="excel">Report as Excel output ? </param>
        /// <exception cref="TNS.AdExpressI.ProductClassReports.Exceptions.NotImplementedReportException">Thrown when the invoked report is not implemented.</exception>
        /// <returns>HTML Code</returns>
        protected virtual string GetProductClassReport(int resultType, bool excel)
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
            _engine.Excel = excel;
            return _engine.GetResult().ToString();
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
        virtual protected ResultTable GetGenericProductClassReport(int resultType, bool excel)
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
