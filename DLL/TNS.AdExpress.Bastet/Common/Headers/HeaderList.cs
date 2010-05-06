#region Informations
// Auteur: B. Masson
// Date de création: 23/02/2007
// Date de modification: 
#endregion

using System;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Bastet.XmlLoader;

namespace TNS.AdExpress.Bastet.Common.Headers{
	/// <summary>
	/// Classe statique qui contient les différentes entêtes du site
	/// Cette classe est chargé au démarrage du portail Web
	/// </summary>
	public class HeaderList{

		#region Variables
		/// <summary>
		/// Ensemble de entêtes disponibles dans le site
		/// </summary>
		protected static Hashtable _list;
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		static HeaderList(){
			_list = new Hashtable();
		}
		#endregion

		#region Initialisation de la classe
		/// <summary>
		/// Initialise la classe
		/// </summary>
		/// <param name="pathXMLFile"></param>
		public static void Init(IDataSource source){
			_list.Clear();
            _list = HeaderXL.GetDescription(source);
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient les headers
		/// </summary>
		public static Hashtable List{
			get{return _list;}
		}
		#endregion
	}
}
