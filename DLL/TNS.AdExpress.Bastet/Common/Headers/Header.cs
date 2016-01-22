#region Informations
// Auteur: B. Masson
// Date de cr�ation: 23/02/2007
// Date de modification: 
#endregion

using System;
using System.Collections;

namespace TNS.AdExpress.Bastet.Common.Headers{
	/// <summary>
	/// Informations n�cessaires au bon fonctionnement de l'ent�te
	/// </summary>
	public class Header{
		
		#region Variables
		/// <summary>
		/// Menus de l'ent�te(List de HeaderMenuItem)
		/// </summary>
		protected ArrayList _menuItems;
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="imgUrl">Url de l'image sous jacente des menus</param>
		/// <param name="flashUrl">Url du fichier animation flash en haut de l'ent�te</param>
		/// <param name="missingFlashUrl">Lien � utiliser quand le client ne dispose pas de flash</param>
        public Header() {
			_menuItems = new ArrayList();
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Get menuItems
		/// </summary>
		public ArrayList MenuItems{
			get{
				return _menuItems;
			}
			set{
				_menuItems = value;
			}
		}
		#endregion

	}
}
