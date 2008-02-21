#region Informations
// Auteur: D. Mussuma
// Création: 21/02/2007
// Modification:
#endregion

using System;
using System.Collections;
using System.IO;
using System.Web;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core;

namespace TNS.AdExpress.Web.Core {
	/// <summary>
	/// Description résumée de CssStylesList.
	/// </summary>
	public class CssStylesList {
		///<summary>
		/// HashTable contenant la liste des styles CSS
		/// </summary>	
		protected static System.Collections.Hashtable _cssList;

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		static CssStylesList(){
			_cssList = new Hashtable();
		
		}
		#endregion


		#region Initialisation de la classe
		///<summary>
		/// Initialise la classe
		/// </summary>
		public static void Init(IDataSource source){
			_cssList = DataAccess.CssStylesListDataAccess.Load(source);			
		}
		#endregion

		

		/// <summary>
		/// Retourne des styles css
		/// </summary>
		/// <param name="cssKey">Identifiant du style css</param>
		/// <returns>styles css</returns>
		public static string GetCssStyles(string cssKey){
			try{
				return _cssList[cssKey].ToString();
			}
			catch(System.Exception){				
				return null;
			}
		}
	}
}
