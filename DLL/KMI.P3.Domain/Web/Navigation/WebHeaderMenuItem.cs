using System;
using System.Collections.Generic;
using System.Text;

namespace KMI.P3.Domain.Web.Navigation
{
    /// <summary>
    /// Information spécifiques à un menu d'une entête homepage ou générique
    /// </summary>
    public class WebHeaderMenuItem
    {

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
        /// On doit ajouter la langue à l'url
        /// </summary>
        protected bool _giveLanguage;
        /// <summary>
        /// On doit ajouter la session à l'url
        /// </summary>
        protected bool _giveSession;
        /// <summary>
        /// Précise la fenêtre cible de l'item courant (_blank,...)
        /// </summary>
        protected string _target;
        /// <summary>
        /// Précise si on veut afficher la page demandé dans une pop up
        /// </summary>
        protected bool _displayInPopUp;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="idMenu">Identifiant du menu</param>
        /// <param name="targetUrl">Url cible du menu</param>
        /// <param name="giveLanguage">Indique si la langue doit être indiquée dans l'url</param> 
        /// <param name="giveSession">Indique si l'identifiant de la session doit être indiquée dans l'url</param>
        /// <param name="target">Indique la fenêtre cible du lien courant</param>
        /// <param name="displayInPopUp">Indique si on veut afficher la page demandé dans une pop up</param>
        public WebHeaderMenuItem(Int64 idMenu, string targetUrl, bool giveLanguage, bool giveSession, string target, bool displayInPopUp)
        {
            _targetUrl = targetUrl;
            _idMenu = idMenu;
            _giveLanguage = giveLanguage;
            _giveSession = giveSession;
            _target = target;
            _displayInPopUp = displayInPopUp;
        }
        #endregion

        #region Accesseurs
        /// <summary>
        /// Obtient l'identifiant du menu
        /// </summary>
        public Int64 IdMenu
        {
            get { return (_idMenu); }
        }

        /// <summary>
        /// Obtient l'Url cible du menu
        /// </summary>
        public string TargetUrl
        {
            get { return _targetUrl; }
        }

        /// <summary>
        /// Obtient si la langue doit être passé en paramètre
        /// </summary>
        public bool GiveLanguage
        {
            get { return (_giveLanguage); }
        }

        /// <summary>
        /// Obtient si la session doit être passé en paramètre
        /// </summary>
        public bool GiveSession
        {
            get { return (_giveSession); }
        }

        /// <summary>
        /// Obtient la cible
        /// </summary>
        public string Target
        {
            get { return _target; }
        }

        /// <summary>
        /// Obtient si on veut afficher la page demandée dans une pop up
        /// </summary>
        public bool DisplayInPopUp
        {
            get { return (_displayInPopUp); }
        }
        #endregion
    }
}
