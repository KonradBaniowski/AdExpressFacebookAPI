#region Informations
// Auteur: D. Mussuma
// Date de création: 10/05/2006
// Date de modification: 
#endregion

using System;

namespace AdExpress.Exceptions{
	/// <summary>
	/// Description résumée de MediaInsertionsCreationsResultsException.
	/// </summary>
	public class MediaInsertionsCreationsResultsException:System.Exception{
		/// <summary>
		/// Code du texte
		/// </summary>
		private int translationTextCode=0;
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MediaInsertionsCreationsResultsException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public MediaInsertionsCreationsResultsException(string message):base(message){
		}

		/// <summary>
		/// Constructeur qui utilise la table de traduction
		/// </summary>
		/// <param name="translationTextCode">identifiant</param>
		public MediaInsertionsCreationsResultsException(int translationTextCode):base(){
			this.translationTextCode=translationTextCode;
		}

		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient le code du texte de l'exception
		/// </summary>
		public int TranslationTextCode{
			get{return(this.translationTextCode);}
		}
		#endregion

	}
}
