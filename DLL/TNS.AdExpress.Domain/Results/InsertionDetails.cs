#region Informations
// Auteur: D. Mussuma, Y. Rkaina
// Cr�ation: 16/05/2006
// Modification:
/*
 * 
 *      27/09/2008 - G Ragneau - Moved from TNS.AdExpress.Web.Core
 * 
 * 
 * 
 * */
#endregion

using System;
using System.Collections;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Domain.Results
{
    /// <summary>
    /// Description r�sum�e de InsertionDetailInformation.
    /// </summary>
    public class InsertionDetails
    {
        #region Variables
        /// <summary>
        /// Les listes des modules autoris�es par  liste de colonnes
        /// </summary>
        ///<link>aggregation</link>
        /// <supplierCardinality>0..*</supplierCardinality>
        /// <associates>TNS.AdExpress.Web.Core.GenericColumnItemInformation</associates>
        protected Dictionary<Int64, Dictionary<Int64, Int64>> _allowedModulesByDetailColums = new Dictionary<Int64, Dictionary<Int64, Int64>>();
        /// <summary>
        /// Niveaux de d�tails orient�s support par d�faut
        /// </summary>
        ///<link>aggregation</link>
        /// <supplierCardinality>0..*</supplierCardinality>
        /// <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
        protected Dictionary<Int64, List<GenericDetailLevel>> _defaultMediaDetailLevels = new Dictionary<Int64, List<GenericDetailLevel>>();
        ///<summary>
        /// El�ments orient� media de niveau de d�tail autoris�s
        /// </summary>
        ///  <link>aggregation</link>
        ///  <supplierCardinality>0..*</supplierCardinality>
        ///  <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
        protected Dictionary<Int64, List<DetailLevelItemInformation>> _allowedMediaDetailLevelItems = new Dictionary<Int64, List<DetailLevelItemInformation>>();
        /// <summary>
        /// Liste des d�tails de colonnes a charger
        /// </summary>
        protected Dictionary<Int64, List<GenericColumnItemInformation>> _detailColumnsList = new Dictionary<Int64, List<GenericColumnItemInformation>>();
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public InsertionDetails(IDataSource source)
        {
            InsertionDetailsXL.Load(source, _allowedModulesByDetailColums, _detailColumnsList, _defaultMediaDetailLevels, _allowedMediaDetailLevelItems);
        }
        #endregion

        /// <summary>
        /// Retourne la liste des colonnes associ�s au m�dia
        /// </summary>
        /// <param name="idVehicle">Identifiant du m�dia</param>
        /// <param name="idModule">Identifiant du module</param>
        /// <returns>Liste des colonnes</returns>
        public List<GenericColumnItemInformation> GetDetailColumns(Int64 idVehicle, Int64 idModule)
        {
            try
            {
                return _detailColumnsList[_allowedModulesByDetailColums[idVehicle][idModule]];

            }
            catch (System.Exception)
            {
                return (null);
            }
        }

        /// <summary>
        /// Get Detail Column Set Id
        /// </summary>
        /// <param name="idVehicle">Current vehicle Id</param>
        /// <param name="idModule">Current module Id</param>
        /// <returns>Column Detail Id matchong idVehicle and Module id</returns>
        public Int64 GetDetailColumnsId(Int64 idVehicle, Int64 idModule)
        {
            try
            {
                return _allowedModulesByDetailColums[idVehicle][idModule];

            }
            catch (System.Exception)
            {
                return (-1);
            }
        }

        /// <summary>
        /// Retourne la liste des niveaux de d�tail m�dia par d�faut pour un m�dia
        /// </summary>
        /// <param name="idVehicle">Identifiant du m�dia</param>
        /// <returns>Liste des colonnes</returns>
        public List<GenericDetailLevel> GetDefaultMediaDetailLevels(Int64 idVehicle)
        {
            try
            {
                return _defaultMediaDetailLevels[idVehicle];
            }
            catch (System.Exception)
            {
                return (null);
            }
        }

        /// <summary>
        /// Retourne la liste des niveaux de d�tail m�dia autoris�s pour un m�dia
        /// </summary>
        /// <param name="idVehicle">Identifiant du m�dia</param>
        /// <returns>Liste des colonnes</returns>
        public List<DetailLevelItemInformation> GetAllowedMediaDetailLevelItems(Int64 idVehicle)
        {
            try
            {
                return _allowedMediaDetailLevelItems[idVehicle];
            }
            catch (System.Exception)
            {
                return (null);
            }
        }
    }
}
