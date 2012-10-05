using System;
using System.Net.Mail;
using TNS.FrameWork;

namespace RolexLoader.Exceptions
{

        /// <summary>
        /// Classe gestion exceptions Avec envoie de mail lorsqu'il y a une erreur sur une page Web.
        /// Cette exception doit être lancé qui si l'utilisateur est authentifié.
        /// </summary>
        [Serializable]
      public class CustomerWebException : System.Exception
        {

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
            public CustomerWebException()
                : base()
            {
            }

            /// <summary>
            /// Constructeur
            /// </summary>
            /// <param name="message">Message d'erreur</param>
            /// <param name="stackTrace">stackTrace</param>
            public CustomerWebException(string message, string stackTrace)
                : base(message)
            {
                this.stackTrace = stackTrace;
            }
            #endregion

            #region Méthodes internes
            /// <summary>
            /// Envoie un mail d'erreur
            /// </summary>
            public void SendMail(string host,int port,string from,string recipients)
            {
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

            

                var errorMail = new SmtpClient(host,port);
                //Send mail
                errorMail.Send(from, recipients, "Error AdExpress Client (" + Environment.MachineName + ")", Convertion.ToHtmlString(body));

            }
            /// <summary>
            /// Envoie un mail d'erreur
            /// </summary>
            public string FormatError()
            {
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
