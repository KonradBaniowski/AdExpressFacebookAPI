#region Informations
// Auteur: D. V. Mussuma 
// Date de création: 13/10/2004 
// Date de modification: 13/10/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions
{
	/// <summary>
	/// Classe gestion exceptions pour  CalculationTypeException.cs
	/// qui indique le typde de calcul total sélectionné (univers, marché, famille)
	/// </summary>
	public class CalculationTypeException:BaseException{

		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public CalculationTypeException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public CalculationTypeException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public CalculationTypeException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
