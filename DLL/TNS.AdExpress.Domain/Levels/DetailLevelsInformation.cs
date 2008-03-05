using System;
using System.Collections;
using TNS.FrameWork.DB.Common;
using ConstantesWeb=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.XmlLoader;

namespace TNS.AdExpress.Domain.Level {


	///<since>28/03/2006</since>
	///  <author>G.Facon</author>
	///  <summary>Liste des niveaux de détails</summary>
	public class DetailLevelsInformation {

		#region Variables
		///<author>G. Facon</author>
		///  <since>28/03/2006</since>
		///<summary>
		///Liste des niveaux de détails prédéfinis
		///</summary>
		///<link>aggregation</link>
		/// <supplierCardinality>0..*</supplierCardinality>
		/// <associates>TNS.AdExpress.Web.Core.Sessions.GenericDetailLevel</associates>
		private  static System.Collections.Hashtable _list;
		

		#endregion

		#region Constructeurs
		///<author>G. Facon</author>
		///  <since>28/03/2006</since>
		///  <summary>
		///  Constructeur
		///  </summary>
		static DetailLevelsInformation( ) {
		}
		#endregion

		#region Méthodes publiques
		///<author>G. Facon</author>
		///  <since>28/03/2006</since>
		///  <summary>Initialisation de la liste</summary>
		public static void Init(IDataSource source) {
			_list=DetailLevelsInformationXL.Load(source);
		}
		
		///<author>G. Facon</author>
		///  <since>28/03/2006</since>
		///  <summary>Obtient la description du niveau de detail</summary>
		///  <param name="detailLevelId">Identifiant du niveau de détail souhaité</param>
		///  <returns>Description du niveau de detail</returns>
		public static GenericDetailLevel Get(int detailLevelId){
			return((GenericDetailLevel)_list[detailLevelId]);
		}
	
		#endregion
	}
}
