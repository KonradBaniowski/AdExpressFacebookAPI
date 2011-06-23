using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace TNS.AdExpressI.VP.DAL
{
    public  interface IVeillePromoDAL
    {


        /// <summary>
        /// Retreive the data for Veille promo schedule result
        /// </summary>
        /// <returns>
        /// DataSet        
        DataSet GetData();
    }
}
