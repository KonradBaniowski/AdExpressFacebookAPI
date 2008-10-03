#region Informations
// Auteur: D. Mussuma, Y. Rkaina
// Création: 24/04/2006
// Modification:
#endregion

using System;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;
using System.Collections.Generic;

namespace TNS.AdExpress.Domain.Level
{
	/// <summary>
	/// Description résumée de GenericColumnItemsInformation.
	/// </summary>
	public class GenericColumnItemsInformation
	{

		#region variables
		/// <summary>
		/// Liste des descriptions des colonnes
		/// </summary>
		///<link>aggregation</link>
		/// <supplierCardinality>0..*</supplierCardinality>
		/// <associates>TNS.AdExpress.Web.Core.GenericColumnItemInformation</associates>
		private Dictionary<Int64, GenericColumnItemInformation> _list = new Dictionary<Int64, GenericColumnItemInformation>();
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public GenericColumnItemsInformation(Dictionary<Int64, GenericColumnItemInformation> list){
            _list = list;
		}
		#endregion

		#region Méthodes publiques
		/// <summary>
		/// Accès à la description d'un élément de la colonne par son identifiant
		/// </summary>
		public GenericColumnItemInformation Get(Int64 id){
			try{
				return _list[id];
			}
			catch(System.Exception err){
				throw(new ArgumentException("impossible to reteive a column item information with this Id",err));
			}
		}
		#endregion

	}
}
