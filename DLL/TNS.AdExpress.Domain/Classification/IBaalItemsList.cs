#region Informations
// Author: G. Facon
// Creation Date: 27/08/2008 
// Modification Date: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Classification {
    /// <summary>
    /// Interface for List initialized from Baal
    /// </summary>
    public interface IBaalItemsList {
        /// <summary>
        /// Init list from baal
        /// </summary>
        /// <param name="idVehicleItemsList">Baal Id List</param>
        /// <param name="levels">Levels to load</param>
        void InitFromBaal(int idVehicleItemsList,List<TNS.Baal.ExtractList.Constantes.Levels> levels);
    }
}
