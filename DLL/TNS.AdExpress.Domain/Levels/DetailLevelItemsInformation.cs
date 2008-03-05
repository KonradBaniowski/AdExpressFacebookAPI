#region Informations
// Auteur: G. Facon
// Création: 27/03/2006
// Modification:
#endregion

using System;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;

namespace TNS.AdExpress.Domain.Level {


	///<summary>
	/// Liste des descriptions des niveaux de détail
	/// </summary>
	///  <property />
	public class DetailLevelItemsInformation {

		#region variables


		///<summary>
		/// Liste des descriptions des niveaux de détail
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
		///  <label>_list</label>
		private static System.Collections.Hashtable _list=new Hashtable();
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		static DetailLevelItemsInformation(){
		}
		#endregion

		#region Accesseurs
		
		#endregion

		#region Méthodes publiques
		/// <summary>
		/// Accès à la description d'un élément de niveau de détail par son identifiant
		/// </summary>
		public static DetailLevelItemInformation Get(int id){
			try{
				return((DetailLevelItemInformation)_list[id]);
			}
			catch(System.Exception err){
				throw(new ArgumentException("impossible to reteive a detail level item information with this Id",err));
			}
		}

		/// <summary>
		/// Initialisation de la liste à partir du fichier XML
		/// </summary>
		/// <param name="source">Source de données</param>
		public static void Init(IDataSource source){
			_list=DetailLevelItemsInformationXL.Load(source);
		}
		#endregion

	}
}
