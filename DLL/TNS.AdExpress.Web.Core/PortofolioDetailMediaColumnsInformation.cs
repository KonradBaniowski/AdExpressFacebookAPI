#region Informations
// Auteur: Y. Rkaina
// Cr�ation: 20/08/2007
// Modification:
#endregion

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Web.Core {

    public class PortofolioDetailMediaColumnsInformation {

        #region Variables
        /// <summary>
        /// Liste des d�tails de colonnes a charger
        /// </summary>
        protected static Hashtable _defaultMediaDetailColumns;
        #endregion

        #region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
        static PortofolioDetailMediaColumnsInformation() {
            _defaultMediaDetailColumns = new Hashtable();
		}
		#endregion

        #region Initialisation de la classe
        /// <summary>
        /// Initialise la classe
        /// </summary>
        /// <param name="source">source de donn�es</param>
        public static void Init(IDataSource source) {
            DataAccess.PortofolioDetailMediaColumnsInformationDataAccess.Load(source, _defaultMediaDetailColumns);
        }
        #endregion

        #region GetDefaultMediaDetailColumns
        /// <summary>
        /// Retourne la liste des colonnes par d�faut pour un m�dia
        /// </summary>
        /// <param name="idVehicle">Identifiant du m�dia</param>
        /// <returns>Liste des colonnes</returns>
        public static ArrayList GetDefaultMediaDetailColumns(Int64 idVehicle) {
            try {
                return (ArrayList)_defaultMediaDetailColumns[idVehicle];
            }
            catch (System.Exception) {
                return (null);
            }
        }
        #endregion

    }
}
