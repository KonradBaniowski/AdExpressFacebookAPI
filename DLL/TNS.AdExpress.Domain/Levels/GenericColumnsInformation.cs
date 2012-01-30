#region Informations
// Auteur: D. Mussuma, Y. Rkaina
// Création: 25/04/2006
// Modification:
#endregion

using System;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;
using System.Collections.Generic;

namespace TNS.AdExpress.Domain.Level {
	/// <summary>
	/// Description résumée de GenericColumnsInformation.
	/// </summary>
	public class GenericColumnsInformation{

		#region Variables
		///<summary>
		///Liste des colonnes par média
		///</summary>
		///<link>aggregation</link>
		/// <supplierCardinality>0..*</supplierCardinality>
		/// <associates>TNS.AdExpress.Web.Core.Sessions.GenericColumns</associates>
        private Dictionary<Int64, List<GenericColumnItemInformation>> _list = new Dictionary<Int64, List<GenericColumnItemInformation>>();
        /// <summary>
        /// Foreach Column Set and each Column Item, Specify is column is visible or not
        /// </summary>
        private Dictionary<Int64, Dictionary<GenericColumnItemInformation.Columns, bool>> _visibility = new Dictionary<Int64, Dictionary<GenericColumnItemInformation.Columns, bool>>();
        /// <summary>
        /// Specify Key columns foreach Column set
        /// </summary>
        private Dictionary<Int64, List<GenericColumnItemInformation>> _keys = new Dictionary<Int64, List<GenericColumnItemInformation>>();
        /// <summary>
        /// Specify Filter columns foreach Column set
        /// </summary>
        private Dictionary<Int64, Dictionary<GenericColumnItemInformation.Columns, bool>> _filters = new Dictionary<Int64, Dictionary<GenericColumnItemInformation.Columns, bool>>();
        /// <summary>
        /// Foreach Column Set and each Column Item, Specify if column is in basket or not
        /// </summary>
        private Dictionary<Int64, Dictionary<GenericColumnItemInformation.Columns, bool>> _basketList = new Dictionary<Int64, Dictionary<GenericColumnItemInformation.Columns, bool>>();
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
        public GenericColumnsInformation(IDataSource source)
        {
            GenericColumnsInformationXL.Load(source, _list, _visibility, _keys, _filters, _basketList);
        }
		#endregion

		#region Méthodes publiques
		///  <summary>Obtient la liste des colonnes</summary>
		///  <param name="idDetailColumn">Identifiant du détail colonne</param>
		///  <returns>La liste des colonnes</returns>
        public List<GenericColumnItemInformation> GetGenericColumnItemInformationList(Int64 idDetailColumn)
        {
			return _list[idDetailColumn];
		}
		///  <summary>Get list of columns entering in key</summary>
		///  <param name="idDetailColumn">Column detail Id</param>
		///  <returns>List of columns</returns>
        public List<GenericColumnItemInformation> GetKeys(Int64 idDetailColumn)
        {
			return _keys[idDetailColumn];
		}
		///  <summary>Get the vibility tag for a specific column in a specific set of columns</summary>
		///  <param name="idDetailColumn">Column detail Id</param>
		///  <returns>True if the column is visible, false neither</returns>
        public bool IsVisible(Int64 idDetailColumn, GenericColumnItemInformation.Columns columnId)
        {
			return _visibility[idDetailColumn][columnId];
		}
		///  <summary>Get the filter tag for a specific column in a specific set of columns</summary>
		///  <param name="idDetailColumn">Column detail Id</param>
        ///  <returns>True if the column can be a filter, false neither</returns>
        public bool IsFilter(Int64 idDetailColumn, GenericColumnItemInformation.Columns columnId)
        {
			return _filters[idDetailColumn][columnId];
		}
        ///  <summary>Get if in basket tag for a specific column in a specific set of columns</summary>
        ///  <param name="idDetailColumn">Column detail Id</param>
        ///  <returns>True if the column is in basket, false neither</returns>
        public bool InBasket(Int64 idDetailColumn, GenericColumnItemInformation.Columns columnId){
            return _basketList[idDetailColumn][columnId];
        }
		#endregion


	}
}
