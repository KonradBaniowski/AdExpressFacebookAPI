#region Informations
// Auteur: Y. R'kaina
// Date de création: 07/03/2007
// Date de modification: 
#endregion

using System;
using System.Collections;
using System.Text;
using System.IO;

using TNS.FrameWork;
using TNS.FrameWork.Net.Mail;
using TNS.Ares.Domain.Exceptions;

namespace TNS.Ares.Domain.Mail {
	/// <summary>
	/// Description résumée de ReportingSystem.
	/// </summary>
	public class ReportingSystem{

		#region Constantes
		/// <summary>
		/// Nom de l'image "logo_Tns.gif"
		/// </summary>
		private const string LOGO_PATH = "logo_Tns.gif";
		/// <summary>
		///  Nom de l'image "degra_coinGH.gif"
		/// </summary>
		private const string DEGRA_COIN_GH = "degra_coinGH.gif";
		/// <summary>
		///  Nom de l'image "pixel.gif"
		/// </summary>
		private const string PIXEL = "pixel.gif"; 
		/// <summary>
		/// Nom de l'image "degra_fond_verti_haut.gif"
		/// </summary>
		private const string DEGRA_FOND_VERTI_HAUT = "degra_fond_verti_haut.gif";
		/// <summary>
		/// Nom de l'image "degra_coinDH.gif"
		/// </summary>
		private const string DEGRA_COIN_DH = "degra_coinDH.gif";
		/// <summary>
		/// Nom de l'image "degra_fond_hori_G.gif"
		/// </summary>
		private const string DEGRA_FOND_HORI_G = "degra_fond_hori_G.gif";
		/// <summary>
		/// Nom de l'image "degra_fond_hori_D.gif"
		/// </summary>
		private const string DEGRA_FOND_HORI_D = "degra_fond_hori_D.gif";
		/// <summary>
		/// Nom de l'image "degra_coinGB.gif"
		/// </summary>
		private const string DEGRA_COIN_GB = "degra_coinGB.gif";
		/// <summary>
		/// Nom de l'image "degra_fond_verti_bas.gif"
		/// </summary>
		private const string DEGRA_FOND_VERTI_BAS = "degra_fond_verti_bas.gif";
		/// <summary>
		/// Nom de l'image "degra_coinDB.gif"
		/// </summary>
		private const string DEGRA_COIN_DB = "degra_coinDB.gif";
		#endregion

		#region Variables
		/// <summary>
		/// Chemin du répertoire qui contient les images
		/// </summary>
		private string _directoryPath=AppDomain.CurrentDomain.BaseDirectory+@"images\Report\";
		/// <summary>
		/// Le titre du rapport
		/// </summary>
		private string _reportTitle=string.Empty;
		/// <summary>
		/// Le temps d'exécution
		/// </summary>
		private TimeSpan _duration;
		/// <summary>
		/// L'heure de fin d'exécution
		/// </summary>
		private DateTime _endExecutionDateTime;
		/// <summary>
		/// Le corps du rapports
		/// </summary>
		private string _reportCore=string.Empty;
		/// <summary>
		/// Liste des mails
		/// </summary>
		private ArrayList _mailList=new ArrayList();
		/// <summary>
		/// Liste des erreurs survenues lors du traitement
		/// </summary>
		private ArrayList _errorList=new ArrayList();
		/// <summary>
		/// L'expediteur du mail
		/// </summary>
		private string _from;
		/// <summary>
		/// Le serveur de mail
		/// </summary>
        private string _mailServer;
		/// <summary>
		/// Le port utilisé pour l'envoie de mail
		/// </summary>
		private int _mailPort;
		/// <summary>
		/// Id Session
		/// </summary>
		private Int64 _navSessionId;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="reportTitle">Titre du rapport</param>
		/// <param name="duration">Durée d'exécution</param>
		/// <param name="endExecutionDateTime">Date de fin du traitement</param>
		/// <param name="reportCore">le corps du rapport</param>
		/// <param name="mailList">La liste des mails</param>
		/// <param name="errorList">La liste des erreurs</param>
		/// <param name="from">l'expediteur du mail</param>
		/// <param name="mailServer">le serveur du mail</param>
		/// <param name="mailPort">le port utilisé pour l'envoie de mail</param>
		/// <param name="navSessionId">Id session</param>
		public ReportingSystem(string reportTitle, TimeSpan duration, DateTime endExecutionDateTime, string reportCore, ArrayList mailList, ArrayList errorList,string from, string mailServer, int mailPort, Int64 navSessionId){
			_reportTitle = reportTitle;
			_duration = duration;
			_endExecutionDateTime = endExecutionDateTime;
			_reportCore = reportCore;
			_mailList = mailList;
			_errorList = errorList;
			_from = from;
			_mailServer = mailServer;
			_mailPort = mailPort;
			_navSessionId = navSessionId;
		}
		#endregion

		#region Méthodes externes
		/// <summary>
		/// Création d'un rapport
		/// </summary>
		public string SetReport(){
			
			StringBuilder t = new StringBuilder();

			if(_reportCore.Length==0){

				t.Append("Avertissement : Le corps du rapport est vide");
			}
			else{

				#region Html
				t.Append("<html>");
				t.Append("<head>");
				t.Append("<style type=\"text/css\">BODY { MARGIN: 0px; BACKGROUND-COLOR: #ffffff }");
				t.Append("</style>");
				t.Append("</head>");
				t.Append("<body>");
				t.Append("<table cellSpacing=\"0\" cellPadding=\"0\" width=\"0\" border=\"0\" ID=\"Table1\">");
				t.Append("<tbody>");
				t.Append("<tr>");
				t.Append("<td background=\"cid:degraCoinGH\"><IMG height=\"28\" src=\"cid:pixel\" width=\"28\"></td>");
				t.Append("<td align=\"left\" background=\"cid:degraFondVertiHaut\">");
				t.Append("<table cellSpacing=\"0\" cellPadding=\"0\" width=\"0\" border=\"0\" ID=\"Table2\">");
				t.Append("<tbody>");
				t.Append("<tr>");
				t.Append("<td><img height=\"1\" src=\"cid:pixel\" width=\"1\"></td>");
				t.Append("<td><img height=\"1\" src=\"cid:pixel\" width=\"560\"></td>");
				t.Append("</tr>");
				t.Append("</tbody>");
				t.Append("</table>");
				t.Append("</td>");
				t.Append("<td background=\"cid:degraCoinDH\"><IMG height=\"28\" src=\"cid:pixel\" width=\"28\"></TD>");
				t.Append("</tr>");
				t.Append("<TR>");
				t.Append("<td background=\"cid:degraFondHoriG\"><img src=\"cid:pixel\" width=\"28\"></td>");
				t.Append("<td vAlign=\"top\"><table width=\"808\" height=\"126\" ID=\"Table3\" cellpadding=\"4\" border=\"0\">");
				t.Append("<tr>");
				t.Append("<td height=\"19\"><IMG src=\"cid:logoPath\" align=\"right\">");
				t.Append("</td>");
				t.Append("</tr>");
				t.Append("<tr>");
				t.Append("<td>");
				t.Append("<font color=\"#4b3e5a\" size=\"4\" face=\"Arial\">"+_reportTitle+"</font>");
				t.Append("</td>");
				t.Append("</tr>");
				t.Append("<tr>");
				t.Append("<td><br/><br/><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">Numéro de demande : </font><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\"><strong>"+_navSessionId+"</strong></font><br>");
				t.Append("<font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">Date de fin du traitement : </font>");
				if(!_endExecutionDateTime.Equals(null))
					t.Append("<font color=\"#4b3e5a\" size=\"2\" face=\"Arial\"><strong>"+_endExecutionDateTime+"</strong></font>");
				t.Append("<br>");
				t.Append("<font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">Durée de traitement : </font>");
				if(_duration!=TimeSpan.Zero)
					t.Append("<font color=\"#4b3e5a\" size=\"2\" face=\"Arial\"><strong>"+ _duration.Minutes + "m " + _duration.Seconds + "s</strong></font>");
				t.Append("<br>");
				t.Append("<br>");
				t.Append("<table style=\"MARGIN: 10px\" cellspacing=\"30\" cellpadding=\"0\" ID=\"Table4\">");
				t.Append("<tr>");
				t.Append("<td width=\"800\" style=\"border-color: #D3C3E6; border-left-width:1px;border-right-width:1px;border-top-width:1px;border-bottom-width:1px;border-left-style:solid;border-right-style:solid;border-top-style:solid;border-bottom-style:solid;\">");
				t.Append("<table id=\"Table5\" cellSpacing=\"0\" cellPadding=\"0\" width=\"800\" border=\"0\">");
				t.Append("<tr>");
				t.Append("<td align=\"center\" bgcolor=\"#f5ecff\" style=\"border-color: #D3C3E6; border-bottom-width:1px;border-bottom-style:solid;\"><font color=\"#4b3e5a\" size=\"3\" face=\"Arial\"><strong>Synthèse</strong></font></td>");
				t.Append("</tr>");
				t.Append("<tr>");
				t.Append("<td>");
				t.Append(_reportCore);
				t.Append("</td>");
				t.Append("</tr>");
				t.Append("</table>");
				t.Append("</td>");
				t.Append("</tr>");
				if(_errorList!=null)
				{
					t.Append("<tr>");
					t.Append("<td width=\"800\" style=\"border-color: #D3C3E6; border-left-width:1px;border-right-width:1px;border-top-width:1px;border-bottom-width:1px;border-left-style:solid;border-right-style:solid;border-top-style:solid;border-bottom-style:solid;\">");
					t.Append("<table id=\"Table6\" cellSpacing=\"0\" cellPadding=\"0\" width=\"800\" border=\"0\">");
					t.Append("<tr>");
					t.Append("<td  align=\"center\" bgcolor=\"#f5ecff\" style=\"border-color: #D3C3E6; border-bottom-width:1px;border-bottom-style:solid;\"><font color=\"#4b3e5a\" size=\"3\" face=\"Arial\"><strong>Liste des erreurs</strong></font></td>");
					t.Append("</tr>");
					t.Append("<tr>");
					t.Append("<td>&nbsp;</td>");
					t.Append("</tr>");
					t.Append("<tr>");
					t.Append("<td><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">&nbsp; Nombre d'erreurs : </font><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\"><strong>"+_errorList.Count+"</strong></font></td>");
					t.Append("</tr>");
					t.Append("<tr>");
					t.Append("<td>&nbsp;</td>");
					t.Append("</tr>");
					foreach(string message in _errorList)
					{ 
						t.Append("<tr>");
						t.Append("<td><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">&nbsp; "+message+"</font></li></td>");
						t.Append("</tr>");
					}
					t.Append("</table>");
					t.Append("</td>");
					t.Append("</tr>");
				}
				t.Append("</table>");
				t.Append("</td>");
				t.Append("</tr>");
				t.Append("</table>");
				t.Append("</td>");
				t.Append("<td background=\"cid:degraFondHoriD\"><IMG height=\"1\" src=\"cid:pixel\" width=\"1\"></td>");
				t.Append("</TR>");
				t.Append("<TR>");
				t.Append("<td background=\"cid:degraCoinGB\"><IMG height=\"1\" src=\"cid:pixel\" width=\"1\"></td>");
				t.Append("<td background=\"cid:degraFondVertiBas\"><img height=\"28\" src=\"cid:pixel\" width=\"1\"></td>");
				t.Append("<td background=\"cid:degraCoinDB\"><img height=\"1\" src=\"cid:pixel\" width=\"1\"></td>");
				t.Append("</tr>");
				t.Append("</tbody></table>");
				t.Append("</body>");
				t.Append("</html>");
				#endregion

			}
			return t.ToString();
		}

		/// <summary>
		/// Envoie d'un rapport
		/// </summary>
		/// <param name="html">Le code html du rapport</param>
		public void SendReport(string html){
		
			#region Variables
			ArrayList to = new ArrayList();
			string objet = "";
			string logoPath=@""+ _directoryPath + LOGO_PATH;
			string degraCoinGH=@""+ _directoryPath + DEGRA_COIN_GH;
			string pixel=@""+ _directoryPath + PIXEL; 
			string degraFondVertiHaut=@""+ _directoryPath + DEGRA_FOND_VERTI_HAUT;
			string degraCoinDH=@""+ _directoryPath + DEGRA_COIN_DH;
			string degraFondHoriG=@""+ _directoryPath + DEGRA_FOND_HORI_G;
			string degraFondHoriD=@""+ _directoryPath + DEGRA_FOND_HORI_D;
			string degraCoinGB=@""+ _directoryPath + DEGRA_COIN_GB;
			string degraFondVertiBas=@""+ _directoryPath + DEGRA_FOND_VERTI_BAS;
			string degraCoinDB=@""+ _directoryPath + DEGRA_COIN_DB;
			#endregion

			#region Objet
			if(_errorList.Count>0)
				objet="Error : \"" + _reportTitle+"\"";
			else
				objet=_reportTitle;
			#endregion

			#region Envoi du mail
			// Liste des destinataires
			foreach(string s in _mailList){
				to.Add(s);
			}
			SmtpUtilities mail = new SmtpUtilities(_from, to, objet, Convertion.ToHtmlString(html.ToString()), true, _mailServer, _mailPort);
			mail.mailKoHandler += new TNS.FrameWork.Net.Mail.SmtpUtilities.mailKoEventHandler(mail_mailKoHandler);
			
			// Affichage de l'image dans le corps du message
			if(_reportCore.Length>0&&File.Exists(logoPath)&&File.Exists(degraCoinGH)&&File.Exists(pixel)&&File.Exists(degraFondVertiHaut)&&File.Exists(degraCoinDH)&&File.Exists(degraFondHoriG)&&File.Exists(degraFondHoriD)&&File.Exists(degraCoinGB)&&File.Exists(degraFondVertiBas)&&File.Exists(degraCoinDB)){
				mail.Attach(logoPath,SmtpUtilities.AttachmentType.ATTACH_JPEG_IN_MAIL,"logoPath");
				mail.Attach(degraCoinGH,SmtpUtilities.AttachmentType.ATTACH_JPEG_IN_MAIL,"degraCoinGH");
				mail.Attach(pixel,SmtpUtilities.AttachmentType.ATTACH_JPEG_IN_MAIL,"pixel");
				mail.Attach(degraFondVertiHaut,SmtpUtilities.AttachmentType.ATTACH_JPEG_IN_MAIL,"degraFondVertiHaut");
				mail.Attach(degraCoinDH,SmtpUtilities.AttachmentType.ATTACH_JPEG_IN_MAIL,"degraCoinDH");
				mail.Attach(degraFondHoriG,SmtpUtilities.AttachmentType.ATTACH_JPEG_IN_MAIL,"degraFondHoriG");
				mail.Attach(degraFondHoriD,SmtpUtilities.AttachmentType.ATTACH_JPEG_IN_MAIL,"degraFondHoriD");
				mail.Attach(degraCoinGB,SmtpUtilities.AttachmentType.ATTACH_JPEG_IN_MAIL,"degraCoinGB");
				mail.Attach(degraFondVertiBas,SmtpUtilities.AttachmentType.ATTACH_JPEG_IN_MAIL,"degraFondVertiBas");
				mail.Attach(degraCoinDB,SmtpUtilities.AttachmentType.ATTACH_JPEG_IN_MAIL,"degraCoinDB");
			}
			mail.SendWithoutThread(false);
			#endregion

		}
		#endregion

		#region Evenement Envoi mail client
		/// <summary>
		/// Rise exception when the customer mail has not been sent
		/// </summary>
		/// <param name="source">Error source></param>
		/// <param name="message">Error message</param>
		private void mail_mailKoHandler(object source, string message){
			throw new ReportingSystemException("Echec lors de l'envoi du rapport d'execution à l'administrateur : " + message);
		}
		#endregion

	}
}
