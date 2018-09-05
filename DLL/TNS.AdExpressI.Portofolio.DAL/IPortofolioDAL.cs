#region Information
// Author: Y. Rkaina & D. Mussuma
// Creation date: 26/08/2008
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Results;
using DBConstantes = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpressI.Portofolio.DAL {
    /// <summary>
    /// Portofolio Data access Interface
    /// </summary>
    public interface IPortofolioDAL {

		/// <summary>
		/// Get data
		/// </summary>
		/// <returns></returns>
		DataSet GetData();		

		/// <summary>
		/// Get insertion data
		/// </summary>
		/// <returns></returns>
		DataSet GetInsertionData();

		#region Synthesis Interface methods
		/// <summary>
		/// Get synthsesis data
		/// </summary>
        /// <param name="synthesisDataType">Synthesis Data Type</param>
		/// <returns></returns>
		DataSet GetSynthisData(PortofolioSynthesis.dataType synthesisDataType);
		#endregion

        /// <summary>
        /// Get Investment By Media
        /// </summary>
        /// <returns>Data Set</returns>
        Hashtable GetInvestmentByMedia();
        /// <summary>
        /// Get adbreak data
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetEcranData();

        /// <summary>
        /// Get custom adbreak data
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetCustomEcranData();

        /// <summary>
        /// Get dates list
        /// </summary>
        /// <param name="conditionEndDate">Add condition end date</param>
        /// <returns>Data Set</returns>
        DataSet GetListDate(bool conditionEndDate, DBConstantes.TableType.Type tableType);			
		/// <summary>
		/// Checks if media belong t category
		/// </summary>
		/// <param name="idMedia">Id media</param>
		/// <param name="idCategory">Id Category</param>
		/// <returns></returns>
		bool IsMediaBelongToCategory(Int64 idMedia, string idCategory);
        /// <summary>
        /// Get Table of issue
        /// </summary>
        /// <returns></returns>
        DataSet TableOfIssue();

        DataSet GetPortfolioAlertData(long alertId, long alertTypeId, string dateMediaNum);

        PortfolioAlertParams GetPortfolioAlertParams(long alertId);

        //DataSet GetPortfolioAlertParamsUniverse(long alertId);

        //DataSet GetPortfolioAlertParamsFlag(long alertId);

    }
}
