#region Informations
// Auteur: G. Facon
// Date de création: 10/06/2004
// Date de modification: 14/06/2004
#endregion

using System;

namespace AdExpress.Exceptions{
	/// <summary>
	/// Description résumée de AdExpressException.
	/// </summary>
	public class AnalyseDateSelectionException:System.Exception{
		/// <summary>
		/// Code du texte
		/// </summary>
		private int translationTextCode=0;
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AnalyseDateSelectionException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AnalyseDateSelectionException(string message):base(message){
		}

		/// <summary>
		/// Constructeur qui utilise la table de traduction
		/// </summary>
		/// <param name="translationTextCode">identifiant</param>
		public AnalyseDateSelectionException(int translationTextCode):base(){
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
