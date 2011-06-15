#region Informations
// Auteur: G. Facon
// Date de cr�ation: 23/12/2004 
// Date de modification: 23/12/2004 
#endregion

#define DEBUG

using System;
using System.Text;
using System.Globalization;
using TNSMail=TNS.FrameWork.Net.Mail; 
using TNS.FrameWork;
using TNS.AdExpress.VP.Loader.Domain.Web;

namespace RenaultLoader.Exceptions {
	

	/// <summary>
	/// Classe gestion exceptions Avec envoie de mail lorsqu'il y a une erreur sur une page Web.
	/// Cette exception doit �tre lanc� qui si l'utilisateur est authentifi�.
	/// </summary>
	[Serializable]
	public class CustomerWebException:System.Exception{

		#region Variables
		/// <summary>
		/// stackTrace
		/// </summary>
		protected string stackTrace;
		#endregion

		#region Constructeur	
		/// <summary>
		/// Constructeur de base
		/// </summary>
		/// <param name="page">Page Web qui lance l'erreur</param>
		/// <param name="webSession">Session du client</param>
		public CustomerWebException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="stackTrace">stackTrace</param>
		public CustomerWebException(string message,string stackTrace):base(message){
			this.stackTrace=stackTrace;
		}
		#endregion

		#region M�thodes internes
		/// <summary>
		/// Envoie un mail d'erreur
		/// </summary>
        public void SendMail() {
            string body = "";


            #region Identifiaction du client
            body += "<html><b><u>" + Environment.MachineName + ":</u></b><br>" + "<font color=#FF0000>Client Error:<br></font>";
            body += "<hr>";
            body += "<u>Error Message:</u><br>" + Message + "<br>";
            body += "<u>Source:</u><br>" + Source + "<br>";
            body += "<u>StackTrace:</u><br>" + stackTrace.Replace("at ", "<br>at ") + "<br>";
            body += "<hr>";
            body += "</html>";
            #endregion

            body += "</html>";

            TNSMail.SmtpUtilities errorMail = new TNSMail.SmtpUtilities(ApplicationParameters.CountryConfigurationDirectoryRoot + "CustomerErrorMail.xml");


            //Send mail
            errorMail.SendWithoutThread("Error AdExpress Client (" + Environment.MachineName + ")", Convertion.ToHtmlString(body), true, false);

        }
        /// <summary>
        /// Envoie un mail d'erreur
        /// </summary>
        public string FormatError() {
            string body = "";


            #region Identifiaction du client
            body += Environment.MachineName + ": Client Error:";
            body += "\n\n";
            body += "Error Message:\n" + Message + "\n";
            body += "Source:\n" + Source + "\n";
            body += "StackTrace:\n" + stackTrace.Replace("at ", "\nat ") + "\n";
            body += "\n";
            #endregion


            return body;

        }
		#endregion

	}
}
