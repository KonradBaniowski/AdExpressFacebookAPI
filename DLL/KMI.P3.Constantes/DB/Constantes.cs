using System;
using System.Collections.Generic;
using System.Text;

namespace KMI.P3.Constantes.DB
{
    #region Class ActivationValues
    /// <summary>
    /// Constantes relatives à la notion d'Activation
    /// </summary>
    public class ActivationValues
    {
        /// <summary>
        /// Activée
        /// </summary>
        public const int ACTIVATED = 0;
        /// <summary>
        /// Désactivée
        /// </summary>
        public const int DEAD = 10;
        /// <summary>
        /// Désactivée
        /// </summary>
        public const int UNACTIVATED = 50;
        /// <summary>
        /// Code de suppression d'un login dans ISIS
        /// </summary>
        public const int ISIS_SUPPRESSION = 50;
    }
    #endregion

    #region Classe des Langues
    /// <summary>
    /// Constantes des codes de langues d'AdExpress
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Langue française
        /// </summary>
        public const int FRENCH = 33;
        /// <summary>
        /// Langue Anglaise
        /// </summary>
        public const int ENGLISH = 44;
    }

    #endregion
}
