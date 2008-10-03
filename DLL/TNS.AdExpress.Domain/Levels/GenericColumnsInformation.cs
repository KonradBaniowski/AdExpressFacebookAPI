#region Informations
// Auteur: D. Mussuma, Y. Rkaina
// Cr�ation: 25/04/2006
// Modification:
#endregion

using System;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;
using System.Collections.Generic;

namespace TNS.AdExpress.Domain.Level {
	/// <summary>
	/// Description r�sum�e de GenericColumnsInformation.
	/// </summary>
	public class GenericColumnsInformation{

		#region Variables
		///<summary>
		///Liste des colonnes par m�dia
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

		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
        public GenericColumnsInformation(IDataSource source)
        {
            GenericColumnsInformationXL.Load(source, _list, _visibility, _keys);
        }
		#endregion

		#region M�thodes publiques
		///  <summary>Obtient la liste des colonnes</summary>
		///  <param name="idDetailColumn">Identifiant du d�tail colonne</param>
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
		///  <returns>List of columns</returns>
        public bool IsVisible(Int64 idDetailColumn, GenericColumnItemInformation.Columns columnId)
        {
			return _visibility[idDetailColumn][columnId];
		}
		#endregion


	}
}
