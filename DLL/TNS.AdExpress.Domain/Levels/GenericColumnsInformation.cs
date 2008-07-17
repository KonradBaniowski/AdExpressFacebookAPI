#region Informations
// Auteur: D. Mussuma, Y. Rkaina
// Cr�ation: 25/04/2006
// Modification:
#endregion

using System;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;

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
		private  static System.Collections.Hashtable _list;

		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public GenericColumnsInformation(){
		}
		#endregion

		#region M�thodes publiques
		///  <summary>Initialisation de la liste</summary>
		public static void Init(IDataSource source) {
			_list=GenericColumnsInformationXL.Load(source);
		}
		///  <summary>Obtient la liste des colonnes</summary>
		///  <param name="idDetailColumn">Identifiant du d�tail colonne</param>
		///  <returns>La liste des colonnes</returns>
		public static ArrayList GetGenericColumnItemInformationList(Int64 idDetailColumn){
			return((ArrayList)_list[idDetailColumn]);
		}
		#endregion


	}
}
