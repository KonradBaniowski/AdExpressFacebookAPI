#region Informations
// Auteur: D. Mussuma
// Date de création: 08/12/2006
// Date de modification: 
#endregion

using System;

namespace AdExpress.Exceptions{
	/// <summary>
	/// Description résumée de AdExpressException.
	/// </summary>
	public class SponsorshipDateSelectionException:System.Exception{
		/// <summary>
		/// Code du texte
		/// </summary>
		private int translationTextCode=0;
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SponsorshipDateSelectionException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SponsorshipDateSelectionException(string message):base(message){
		}

		/// <summary>
		/// Constructeur qui utilise la table de traduction
		/// </summary>
		/// <param name="translationTextCode">identifiant</param>
		public SponsorshipDateSelectionException(int translationTextCode):base(){
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
