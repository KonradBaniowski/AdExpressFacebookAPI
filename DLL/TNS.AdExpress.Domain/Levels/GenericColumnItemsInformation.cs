#region Informations
// Auteur: D. Mussuma, Y. Rkaina
// Création: 24/04/2006
// Modification:
#endregion

using System;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;

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
		private static System.Collections.Hashtable _list=new Hashtable();
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public GenericColumnItemsInformation(){
		}
		#endregion

		#region Méthodes publiques
		/// <summary>
		/// Accès à la description d'un élément de la colonne par son identifiant
		/// </summary>
		public static GenericColumnItemInformation Get(Int64 id){
			try{
				return((GenericColumnItemInformation)_list[id]);
			}
			catch(System.Exception err){
				throw(new ArgumentException("impossible to reteive a column item information with this Id",err));
			}
		}

		/// <summary>
		/// Initialisation de la liste à partir du fichier XML
		/// </summary>
		/// <param name="source">Source de données</param>
		public static void Init(IDataSource source){
			_list=GenericColumnItemsInformationXL.Load(source);
		}
		#endregion

	}
}
