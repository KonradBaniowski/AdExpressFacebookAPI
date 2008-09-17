#region Informations
// Auteur: B. Masson
// Date de création: 23/02/2007
// Date de modification: 
#endregion

using System;

namespace TNS.AdExpress.Bastet.Common.Headers{
	/// <summary>
	/// Information spécifiques à un menu d'une entête
	/// </summary>
	public class HeaderMenuItem{

		#region Variables
		/// <summary>
		/// Identifiant du menu
		/// </summary>
		protected int _id;
		/// <summary>
		/// L'élément doit il être montré
		/// </summary>
		protected bool _display=false;
		/// <summary>
		/// Identifiant du texte du menu dans Web_Text
		/// </summary>
		protected Int64 _idWebText;
		/// <summary>
		/// Texte du menu
		/// </summary>
		protected string _text;
		/// <summary>
		/// Url cible du menu
		/// </summary>
		protected string _targetUrl;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">Identifiant du menu</param>
		/// <param name="display">Booléen pour dire si l'élément doit être montré</param>
		/// <param name="idWebText">Identifiant du texte du menu</param>
		/// <param name="targetUrl">Url cible du menu</param>
		public HeaderMenuItem(int id, bool display,Int64 idWebText,string text,string targetUrl){
			_id=id;
			_display=display;
			this._targetUrl = targetUrl;
			this._idWebText = idWebText;
			this._text = text;
		}
		#endregion
		
		#region Accesseurs
		/// <summary>
		/// Obtient l'identifiant du menu
		/// </summary>
		public int Id{
			get{return(_id);}
		}

		/// <summary>
		/// Obtient le booléen pour montrer l'élément
		/// </summary>
		public bool Display{
			get{return(_display);}
		}

		/// <summary>
		/// Obtient l'identifiant du menu
		/// </summary>
		public Int64 IdWebText{
			get{return(_idWebText);}
		}

		/// <summary>
		/// Obtient le texte du menu
		/// </summary>
		public string Text{
			get{return(_text);}
		}

		/// <summary>
		/// Obtient l'Url cible du menu
		/// </summary>
		public string TargetUrl{
			get{return _targetUrl;}
		}
		#endregion

	}
}
