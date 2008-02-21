using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Web.Core.Result {
    public interface ILinkRules {
        /// <summary>
        /// Indique si le lien doit �tre montr�e dans la Cellule
        /// </summary>
        /// <returns>True s'il doit �tre montr�, false sinon</returns>
        bool ShowLink();
        /// <summary>
        /// Obtient la hierarchie de la nomenclature sur 4 niveaux
        /// </summary>
        /// <remarks>
        /// La valeur -1 correspond � un niveau dont il ne faut pas tenir compte
        /// </remarks>
        /// <example>
        /// hierarchie: "1,235,366665,-1" ou "-1,-1,3336,-1"
        /// </example>
        /// <returns>Cha�ne repr�sentant la hierarchie</returns>
        string GetHierarchy();
    }
}
