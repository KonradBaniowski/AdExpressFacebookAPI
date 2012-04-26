using System;
using System.Collections.Generic;
using TNS.AdExpress.Anubis.Miysis;

namespace TNS.AdExpress.Anubis.Celebrities{
    /// <summary>
    /// Description résumée de TreatementSystem.
    /// </summary>
    public class TreatementSystem : Miysis.TreatementSystem {

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public TreatementSystem() : base() {
        }
        #endregion

        #region Nom du Plug-in
        /// <summary>
        /// Obtient le nom du plug-in
        /// </summary>
        /// <returns>Le nom du plug-in</returns>
        public override string GetPluginName() {
            return ("Celebrities PDF Generator");
        }
        #endregion

    }
}
