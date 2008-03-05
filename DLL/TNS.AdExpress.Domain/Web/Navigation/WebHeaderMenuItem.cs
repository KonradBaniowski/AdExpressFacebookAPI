#region Informations
// Auteur: G. Ragneau
// Date de Cr�ation:
// Date de Modification:
//	G. Facon	11/08/2005	Nom des variables
#endregion

using System;

namespace TNS.AdExpress.Domain.Web.Navigation {
	/// <summary>
	/// Information sp�cifiques � un menu d'une ent�te homepage ou g�n�rique
	/// </summary>
	public class WebHeaderMenuItem{

		#region Variables
		/// <summary>
		/// Identifiant du texte du menu dans Web_Text
		/// </summary>
		protected Int64 _idMenu;
		/// <summary>
		/// Url cible du menu
		/// </summary>
		protected string _targetUrl;
		/// <summary>
		/// On doit ajouter la langue � l'url
		/// </summary>
		protected bool _giveLanguage;
		/// <summary>
		/// On doit ajouter la session � l'url
		/// </summary>
		protected bool _giveSession;
		/// <summary>
		/// Pr�cise la fen�tre cible de l'item courant (_blank,...)
		/// </summary>
		protected string _target;
        /// <summary>
        /// Pr�cise si on veut afficher la page demand� dans une pop up
        /// </summary>
        protected bool _displayInPopUp;
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="idMenu">Identifiant du menu</param>
		/// <param name="targetUrl">Url cible du menu</param>
		/// <param name="giveLanguage">Indique si la langue doit �tre indiqu�e dans l'url</param> 
		/// <param name="giveSession">Indique si l'identifiant de la session doit �tre indiqu�e dans l'url</param>
		/// <param name="target">Indique la fen�tre cible du lien courant</param>
        /// <param name="displayInPopUp">Indique si on veut afficher la page demand� dans une pop up</param>
        public WebHeaderMenuItem(Int64 idMenu, string targetUrl, bool giveLanguage, bool giveSession, string target, bool displayInPopUp) {
			_targetUrl = targetUrl;
			_idMenu = idMenu;
			_giveLanguage=giveLanguage;
			_giveSession=giveSession;
			_target=target;
            _displayInPopUp = displayInPopUp;
		}
		#endregion
		
		#region Accesseurs
		/// <summary>
		/// Obtient l'identifiant du menu
		/// </summary>
		public Int64 IdMenu{
			get{return(_idMenu);}
		}

		/// <summary>
		/// Obtient l'Url cible du menu
		/// </summary>
		public string TargetUrl{
			get{return _targetUrl;}
		}

		/// <summary>
		/// Obtient si la langue doit �tre pass� en param�tre
		/// </summary>
		public bool GiveLanguage{
			get{return(_giveLanguage);}
		}

		/// <summary>
		/// Obtient si la session doit �tre pass� en param�tre
		/// </summary>
		public bool GiveSession{
			get{return(_giveSession);}
		}

		/// <summary>
		/// Obtient la cible
		/// </summary>
		public string Target{
			get{return _target;}
		}

        /// <summary>
        /// Obtient si on veut afficher la page demand�e dans une pop up
        /// </summary>
        public bool DisplayInPopUp {
            get { return (_displayInPopUp); }
        }
		#endregion
	}
}
