using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpressI.VP
{
    public interface IVeillePromo
    {

          /// <summary>
        /// Get HTML code for the promotion schedule
        /// </summary>
        /// <returns>HTML Code</returns>
        string GetHtml();
    }
}
