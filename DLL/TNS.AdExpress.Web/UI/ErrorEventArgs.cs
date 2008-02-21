#region Information
// Auteur : G Facon
// Cr�� le : 24/12/2004
//	Modifi� le : 24/12/2004
//		12/08/2005	G. Facon	Nom de variables
//		
#endregion

using System;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.UI{
	/// <summary>
	/// Argument d'une erreur survenue dans une page Web.
	/// Cette classe est � utiliser lorsque l'on cr�e un ev�nement OnError sur une page Web
	/// qui h�rite de TNS.AdExpress.Web.UI.WebPage.
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
	public class ErrorEventArgs:EventArgs{

		#region Enum�rateur
		/// <summary>
		/// Nom des arguements utilis� dans la classe TNS.AdExpress.Web.UI.ErrorEventArgs
		/// </summary>
		public enum argsName{
			/// <summary>
			/// Page qui lance l'erreur
			/// </summary>
			sender,
			/// <summary>
			/// Exception lanc�e
			/// </summary>
			error,
			/// <summary>
			/// Session du client
			/// </summary>
			custormerSession
		}
		#endregion

		#region Variables
		/// <summary>
		/// Exception lanc�e
		/// </summary>
		private System.Exception _exception;
		/// <summary>
		/// Session du client
		/// </summary>
		private WebSession _webSession;
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
		/// <param name="exception">Exception lanc�e</param>
		/// <param name="webSession">Session du client</param>
		public ErrorEventArgs(System.Web.UI.Page page,System.Exception exception,WebSession webSession){
			_page=page;
			_exception=exception;
			_webSession=webSession;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient la page qui lance l'erreur
		/// </summary>
		public System.Web.UI.Page Sender{
			get{return(_page);}
		}
		/// <summary>
		/// Obtient l'Exception lanc�e
		/// </summary>
		public System.Exception Error{
			get{return(_exception);}
		}
		/// <summary>
		/// Session du client
		/// </summary>
		public WebSession CustomerSession{
			get{return(_webSession);}
		}

		/// <summary>
		/// Acc�s aux arguments
		/// </summary>
		public object this [ErrorEventArgs.argsName name]{
			get{
				switch(name){
					case ErrorEventArgs.argsName.sender:
						return(_page);
					case ErrorEventArgs.argsName.error:
						return(_exception);
					case ErrorEventArgs.argsName.custormerSession:
						return(_webSession);
					default:
						return(null);
				}
			}
		}
		#endregion
	}
}
