using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.PromoPSA.Web.UI
{
    /// <summary>
    /// Argument d'une erreur survenue dans une page Web.
    /// Cette classe est à utiliser lorsque l'on crée un evènement OnError sur une page Web
    /// qui hérite de TNS.AdExpress.Web.UI.WebPage.
    /// <seealso cref="TNS.AdExpress.Web.UI.WebPage"/>
    /// </summary>
    /// <example>
    /// <code>
    /// try{
    ///		...
    ///	}
    ///	catch(System.Exception exc){
    ///		this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,webSession));
    ///	}
    /// </code>
    /// </example>
    public class ErrorEventArgs : EventArgs
    {

        #region Enumérateur
        /// <summary>
        /// Nom des arguements utilisé dans la classe TNS.AdExpress.Web.UI.ErrorEventArgs
        /// </summary>
        public enum argsName
        {
            /// <summary>
            /// Page qui lance l'erreur
            /// </summary>
            sender,
            /// <summary>
            /// Exception lancée
            /// </summary>
            error,
        }
        #endregion

        #region Variables
        /// <summary>
        /// Exception lancée
        /// </summary>
        private System.Exception _exception;
        /// <summary>
        /// Page qui lance l'erreur
        /// </summary>
        private System.Web.UI.Page _page;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="page">Page qui lance l'erreur</param>
        /// <param name="exception">Exception lancée</param>
        public ErrorEventArgs(System.Web.UI.Page page, System.Exception exception)
        {
            _page = page;
            _exception = exception;
        }
        #endregion

        #region Accesseurs
        /// <summary>
        /// Obtient la page qui lance l'erreur
        /// </summary>
        public System.Web.UI.Page Sender
        {
            get { return (_page); }
        }
        /// <summary>
        /// Obtient l'Exception lancée
        /// </summary>
        public System.Exception Error
        {
            get { return (_exception); }
        }

        /// <summary>
        /// Accès aux arguments
        /// </summary>
        public object this[ErrorEventArgs.argsName name]
        {
            get
            {
                switch (name)
                {
                    case ErrorEventArgs.argsName.sender:
                        return (_page);
                    case ErrorEventArgs.argsName.error:
                        return (_exception);
                    default:
                        return (null);
                }
            }
        }
        #endregion

    }
}
