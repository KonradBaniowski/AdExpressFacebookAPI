using System.Data;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Sessions;
using System.Collections.Generic;

namespace TNS.AdExpressI.Rolex.DAL
{
    public interface IRolexDAL
    {
        /// <summary>
        /// User Session
        /// </summary>
        WebSession Session { get; set; }

        /// <summary>
        /// Get Min Max Period
        /// </summary>
        /// <returns></returns>
        DataSet GetMinMaxPeriod();
        /// <summary>
        /// Retreive the data for Rolex schedule result
        /// </summary>
        /// <param name="detailLevel">detail Level</param>
        /// <returns>
        /// DataSet  
        /// </returns>
        DataSet GetData(GenericDetailLevel detailLevel);

        /// <summary>
        /// Retreive the data for Rolex schedule result
        /// </summary>
        /// <param name="selectedDetailLevel">selected Detail Level</param>
        /// <param name="selectedLevelValues">selected Level Values</param>
        /// <param name="detailLevel">detail Level</param>
        /// <returns>
        /// DataSet  
        /// </returns>
        DataSet GetFileData(GenericDetailLevel selectedDetailLevel, List<long> selectedLevelValues, GenericDetailLevel detailLevel);
    }
}