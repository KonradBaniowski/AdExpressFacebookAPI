#region Informations
// Auteur: G. Facon
// Cr�ation: 27/03/2006
// Modification:
#endregion

using System;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;

namespace TNS.AdExpress.Domain.Level {


	///<summary>
	/// Liste des descriptions des niveaux de d�tail
	/// </summary>
	///  <property />
	public class DetailLevelItemsInformation {

		#region variables


		///<summary>
		/// Liste des descriptions des niveaux de d�tail
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

		#region M�thodes publiques
		/// <summary>
		/// Acc�s � la description d'un �l�ment de niveau de d�tail par son identifiant
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
		/// Initialisation de la liste � partir du fichier XML
		/// </summary>
		/// <param name="source">Source de donn�es</param>
		public static void Init(IDataSource source){
			_list=DetailLevelItemsInformationXL.Load(source);
		}
		#endregion

	}
}
