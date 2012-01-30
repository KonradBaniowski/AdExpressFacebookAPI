#region Information
/*
 * Author : G Ragneau
 * Creation : 18/04/2008
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion

#region Using
using System.Data;

using TNS.AdExpress.Web.Core.Sessions;
#endregion

namespace TNS.AdExpressI.LostWon.DAL
{

    /// <summary>
    /// Dynamic Report DAL Contract
    /// </summary>
    public interface ILostWonResultDAL
    {
        DataSet GetData();
        DataSet GetSynthesisData();
        //DataSet GetMediaDetails();
        /// <summary>    
        /// In the result page, client can choose the vehicle-level detail in column by selecting it from the drop-down menu.
        /// Then this method gets the list of identifiers of items corresponding to the vehicle-level selected.		
        /// </summary>
        /// <returns>Data set with list of vehicle-level items. </returns>		
        /// <remarks>The query must always contains the field of vehicle level ( "id_media" )</remarks>
        /// <exception cref="TNS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
        /// Exception throwed when an error occurs in the method</exception>
        DataSet GetColumnDetails();

		DataSet GetNbParutionData();


    }

}
