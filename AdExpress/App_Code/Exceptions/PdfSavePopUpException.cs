#region Informations
// Auteur: D.V. Mussuma
// Date de création: 02/02/2007
// Date de modification: 02/02/2007
#endregion
using System;

namespace AdExpress.Exceptions
{
	/// <summary>
	/// Gere les exceptions générés dans PdfSavePopUpException.
	/// </summary>
	public class PdfSavePopUpException:System.Exception
	{
		/// <summary>
		/// Code du texte
		/// </summary>
		private int translationTextCode=0;

		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PdfSavePopUpException():base()
		{			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PdfSavePopUpException(string message):base(message)
		{			
		}
		/// <summary>
		/// Constructeur qui utilise la table de traduction
		/// </summary>
		/// <param name="translationTextCode">identifiant</param>
		public PdfSavePopUpException(int translationTextCode):base()
		{	
			this.translationTextCode=translationTextCode;
		}
	}
}
