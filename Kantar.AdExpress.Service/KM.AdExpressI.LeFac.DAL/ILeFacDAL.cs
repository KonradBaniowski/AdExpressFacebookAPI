using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpressI.LeFac.DAL
{
    public interface ILeFacDAL
    {
        /// <summary>
        /// Obtient les données de LeFac
        /// </summary>
        /// <returns>Données</returns>
        DataSet GetData();
    }
}
