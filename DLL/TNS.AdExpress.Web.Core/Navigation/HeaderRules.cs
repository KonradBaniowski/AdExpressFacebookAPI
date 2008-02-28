#region Informations
// Auteur: G. Ragneau
// Date de Création:
// Date de Modification:
//	G. Facon	11/08/2005	Nom des variables
#endregion

using System;
using System.Collections;
using TNS.AdExpress.Web.Core.DataAccess.Navigation;


namespace TNS.AdExpress.Web.Core.Navigation{
	/// <summary>
	/// Classe statique qui contient les différentes entêtes du site
	/// Cette classe est chargé au démarrage du portail Web
	/// </summary>
	public class HeaderRules{

		#region Variables


		///<summary>
		/// Ensemble de entêtes disponibles dans le site
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.Navigation.Header</associates>
		///  <label>_headers</label>
		protected static System.Collections.Hashtable _headers;
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		static HeaderRules(){
			_headers = new Hashtable();
		}
		#endregion

		#region Initialisation de la classe
		/// <summary>
		/// Initialise la classe
		/// </summary>
		/// <param name="pathXMLFile">Xml File name</param>
		public static void Init(string pathXMLFile){
			HeaderDataAccess.Load(pathXMLFile, _headers);
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Get headers
		/// </summary>
		public static Hashtable Headers{
			get{
				return _headers;
			}
		}
		#endregion
	}
}
