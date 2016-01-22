#region Informations
// Author: G. Facon
// Creation date: 14/08/2008
// Modification: 
#endregion


using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Translation {
    /// <summary>
    /// Interface indicates that the class use the language parameter
    /// </summary>
    public interface ITranslation {
        int Language {
            get;
            set;
        }
    }
}
