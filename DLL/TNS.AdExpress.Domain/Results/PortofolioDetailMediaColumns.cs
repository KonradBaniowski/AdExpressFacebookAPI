#region Informations
// Auteur: Y. Rkaina
// Création: 20/08/2007
// Modification:
/*
 *      27/09/2008 - G Ragneau - Moved from TNS.AdExpress.Web.Core
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * */
#endregion

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Domain.Results {

    public class PortofolioDetailMediaColumns {

        #region Variables
        /// <summary>
        /// Liste des détails de colonnes a charger
        /// </summary>
        protected Dictionary<Int64, List<GenericColumnItemInformation>> _defaultMediaDetailColumns;
        #endregion

        #region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
        public PortofolioDetailMediaColumns(IDataSource source)
        {
            _defaultMediaDetailColumns = new Dictionary<Int64, List<GenericColumnItemInformation>>();
            XmlLoader.PortofolioDetailMediaColumnsXL.Load(source, _defaultMediaDetailColumns);
		}
		#endregion

        #region GetDefaultMediaDetailColumns
        /// <summary>
        /// Retourne la liste des colonnes par défaut pour un média
        /// </summary>
        /// <param name="idVehicle">Identifiant du média</param>
        /// <returns>Liste des colonnes</returns>
        public List<GenericColumnItemInformation> GetDefaultMediaDetailColumns(Int64 idVehicle)
        {
            try {
                return _defaultMediaDetailColumns[idVehicle];
            }
            catch (System.Exception) {
                return (null);
            }
        }
        #endregion

    }
}
