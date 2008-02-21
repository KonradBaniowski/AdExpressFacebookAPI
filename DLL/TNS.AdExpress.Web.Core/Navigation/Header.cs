#region Informations
// Auteur: G. Ragneau
// Date de Création:
// Date de Modification:
//	G. Facon	11/08/2005	Nom des variables
#endregion

using System;
using System.Collections;

namespace TNS.AdExpress.Web.Core.Navigation{
	/// <summary>
	/// Informations nécessaires au bon fonctionnement de l'entête.
	/// </summary>
	public class Header{

		#region Variables
		/// <summary>
		/// Url de l'image sous jacente des menus
		/// </summary>
		protected string _imgUrl;
		/// <summary>
		/// Url de l'animation flash au dessus des menus
		/// </summary>
		protected string _flashUrl;
		/// <summary>
		/// Lien à utiliser quand le client ne dispose pas de flash
		/// </summary>
		protected string _missingFlashUrl;
		///<summary>
		/// Menus de l'entête(List de HeaderMenuItem)
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.Navigation.HeaderMenuItem</associates>
		///  <label>_menuItems</label>
		protected System.Collections.ArrayList _menuItems;
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="imgUrl">Url de l'image sous jacente des menus</param>
		/// <param name="flashUrl">Url du fichier animation flash en haut de l'entête</param>
		/// <param name="missingFlashUrl">Lien à utiliser quand le client ne dispose pas de flash</param>
		public Header(string imgUrl, string flashUrl, string missingFlashUrl){
			_imgUrl=imgUrl;
			_missingFlashUrl = missingFlashUrl;
			_flashUrl = flashUrl;
			_menuItems = new ArrayList();
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Get l'url de l'image sous jacente des menus
		/// </summary>
		public string ImgUrl{
			get{return _imgUrl;}
		}

		/// <summary>
		/// Get l'url de l'animation flash au dessus des menus
		/// </summary>
		public string FlashUrl{
			get{return _flashUrl;}
		}

		/// <summary>
		/// Get l'url de l'animation flash au dessus des menus
		/// </summary>
		public string MissingFlashUrl{
			get{return _missingFlashUrl;}
		}

		/// <summary>
		/// Get and set menuItems
		/// </summary>
		public ArrayList MenuItems{
			get{return _menuItems;}
			set{_menuItems = value;}
		}
		#endregion

	}
}
