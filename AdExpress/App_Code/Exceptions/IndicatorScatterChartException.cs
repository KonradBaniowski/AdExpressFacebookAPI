using System;

namespace AdExpress.Exceptions
{
	/// <summary>
	/// Description résumée de IndicatorScatterChartException.
	/// </summary>
	public class IndicatorScatterChartException : System.Exception
	{
		/// <summary>
		/// Code du texte
		/// </summary>
		private int translationTextCode=0;

		/// <summary>
		/// constructeur
		/// </summary>
		public IndicatorScatterChartException():base()
		{
			
		}
		/// <summary>
		/// constructeur 
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public IndicatorScatterChartException(string message):base(message) {
			
		}

		/// <summary>
		/// Constructeur qui utilise la table de traduction
		/// </summary>
		/// <param name="translationTextCode">identifiant</param>
		public IndicatorScatterChartException(int translationTextCode):base(){
			this.translationTextCode=translationTextCode;
		}

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
