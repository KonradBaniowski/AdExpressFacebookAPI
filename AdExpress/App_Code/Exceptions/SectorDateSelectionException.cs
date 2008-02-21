#region Informations
// Auteur: D.V. Mussuma
// Date de création: 20/09/2004
// Date de modification: 20/09/2004
#endregion
using System;

namespace AdExpress.Exceptions
{
	/// <summary>
	/// Gere les exceptions générés dans SectorDateSelectionException.
	/// </summary>
	public class SectorDateSelectionException:System.Exception
	{
		/// <summary>
		/// Code du texte
		/// </summary>
		private int translationTextCode=0;

		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SectorDateSelectionException():base()
		{			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SectorDateSelectionException(string message):base(message)
		{			
		}
		/// <summary>
		/// Constructeur qui utilise la table de traduction
		/// </summary>
		/// <param name="translationTextCode">identifiant</param>
		public SectorDateSelectionException(int translationTextCode):base()
		{	
			this.translationTextCode=translationTextCode;
		}
	}
}
