using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace TNS.AdExpressI.VP.DAL
{
    public  interface IVeillePromoDAL
    {

         /// <summary>
        /// Get Min Max Period
        /// </summary>
        /// <returns></returns>
        DataSet GetMinMaxPeriod();

        /// <summary>
        /// Retreive the data for Veille promo schedule result
        /// </summary>
        /// <returns>
        /// DataSet        
        DataSet GetData();
    }
}
