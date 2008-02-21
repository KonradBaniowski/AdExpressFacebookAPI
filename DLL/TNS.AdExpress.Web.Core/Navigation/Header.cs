#region Informations
// Auteur: G. Ragneau
// Date de Cr�ation:
// Date de Modification:
//	G. Facon	11/08/2005	Nom des variables
#endregion

using System;
using System.Collections;

namespace TNS.AdExpress.Web.Core.Navigation{
	/// <summary>
	/// Informations n�cessaires au bon fonctionnement de l'ent�te.
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
		/// Lien � utiliser quand le client ne dispose pas de flash
		/// </summary>
		protected string _missingFlashUrl;
		///<summary>
		/// Menus de l'ent�te(List de HeaderMenuItem)
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
		/// <param name="flashUrl">Url du fichier animation flash en haut de l'ent�te</param>
		/// <param name="missingFlashUrl">Lien � utiliser quand le client ne dispose pas de flash</param>
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
