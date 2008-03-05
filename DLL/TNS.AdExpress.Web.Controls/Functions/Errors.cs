#region Information
// Auteur : D. Mussuma
// Créé le : 26/03/2007
// Modifié le :
#endregion
using System;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;

using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Controls.Functions
{
	/// <summary>
	///  Ensemble de fonctions de gestion d'erreurs.
	/// </summary>
	public class Errors
	{
		#region OnAjaxMethodError
		/// <summary>
		/// Appelé sur erreur à l'exécution des méthodes Ajax
		/// </summary>
		/// <param name="errorException">Exception</param>
		/// <param name="customerSession">Session utilisateur</param>
		/// <returns>Message d'erreur</returns>
		internal static string OnAjaxMethodError(Exception errorException,WebSession customerSession) {
			TNS.AdExpress.Web.Exceptions.CustomerWebException cwe = null;
			try{
				BaseException err=(BaseException)errorException;
				cwe=new TNS.AdExpress.Web.Exceptions.CustomerWebException(err.Message,err.GetHtmlDetail(),customerSession);
			}
			catch(System.Exception){
				try{
					cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException(errorException.Message,errorException.StackTrace,customerSession);
				}
				catch(System.Exception es){
					throw(es);
				}
			}
            cwe.SendMail();
            return GetMessageError(customerSession, 1973);
		}
		#endregion

		#region Méthodes privées
		/// <summary>
		/// Message d'erreur
		/// </summary>
		/// <param name="customerSession">Session du client</param>
		/// <param name="code">Code message</param>
		/// <returns>Message d'erreur</returns>
		protected static string GetMessageError(WebSession customerSession, int code){
			string errorMessage="<div align=\"center\" class=\"txtViolet11Bold\">";
			if(customerSession!=null)
				errorMessage += GestionWeb.GetWebWord(code,customerSession.SiteLanguage)+". "+GestionWeb.GetWebWord(2099,customerSession.SiteLanguage);			
			else
				errorMessage += GestionWeb.GetWebWord(code,33)+". "+GestionWeb.GetWebWord(2099,33);
			
			errorMessage +="</div>";
			return errorMessage;
		}

		#endregion
	}
}
