#region Informations
// Auteur: B. Masson, G.Facon
// Date de cr�ation: 22/11/2005
// Date de modification: 
#endregion

using System;
using System.Collections;

namespace BastetWeb{
	/// <summary>
	/// Argument d'une erreur survenue dans une page Web.
	/// Cette classe est � utiliser lorsque l'on cr�e un ev�nement OnError sur une page Web
	/// qui h�rite de BasePage.cs
	/// <seealso cref="TNS.AdExpress.Web.UI.WebPage"/>
	/// </summary>
	/// <example>
	/// <code>
	/// try{
	///		...
	///	}
	///	catch(System.Exception exc){
	///		this.OnError(new ... .ErrorEventArgs(this,exc));
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
		}
		#endregion

		#region Variables
		/// <summary>
		/// Exception lanc�e
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
		/// <param name="exception">Exception lanc�e</param>
		public ErrorEventArgs(System.Web.UI.Page page,System.Exception exception){
			_page=page;
			_exception=exception;
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
		/// Acc�s aux arguments
		/// </summary>
		public object this [ErrorEventArgs.argsName name]{
			get{
				switch(name){
					case ErrorEventArgs.argsName.sender:
						return(_page);
					case ErrorEventArgs.argsName.error:
						return(_exception);
					default:
						return(null);
				}
			}
		}
		#endregion

	}
}
