#region Informations
// Auteur : B.Masson
// Date de création : 24/04/2006
// Date de modification :
//	01/06/2006 Par B.Masson > Traduction pour l'email
#endregion

#region Namespaces
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using TNS.AdExpress.Anubis.Geb.UI;
using TNS.AdExpress.Domain.Translation;

using GebConfigurationAlert=TNS.AdExpress.Geb.Configuration.Alert;
using GebAlertRequest=TNS.AdExpress.Geb.AlertRequest;

using GebCommon=TNS.AdExpress.Anubis.Geb.Common;

using TNS.FrameWork;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Net.Mail;
using FrameworkDate=TNS.FrameWork.Date;
#endregion

namespace TNS.AdExpress.Anubis.Geb.BusinessFacade{
	/// <summary>
	/// Description résumée de GebExcelSystem.
	/// </summary>
	public class GebExcelSystem : GebExcelUI{
		
		#region Variables
		/// <summary>
		/// Source de données
		/// </summary>
		private IDataSource _source = null;
		/// <summary>
		/// Configuration du plugin Geb
		/// </summary>
		private GebCommon.GebConfig _config = null;
		/// <summary>
		/// Chemin du fichier excel généré
		/// </summary>
		private string _excelFilePath;
		/// <summary>
		/// Chemin du dossier contenant le fichier excel généré
		/// </summary>
		private string _excelDirectoryPath;
		/// <summary>
		/// Ligne en cours de la table static_nav_session
		/// </summary>
		private DataRow _rqDetails = null;
		/// <summary>
		/// Paramètres de l'alerte du BLOB
		/// </summary>
		private GebAlertRequest _alertParametersBlob = null;
		/// <summary>
		/// Paramètres de l'alerte de la table BDD
		/// </summary>
		private GebConfigurationAlert _alertParameters = null;
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="config">Configuration du plugin Geb</param>
		/// <param name="rqDetails">Ligne en cours de la table static_nav_session</param>
		/// <param name="alertParametersBlob">Paramètres de l'alerte du BLOB</param>
		/// <param name="alertParameters">Paramètres de l'alerte de la table BDD</param>
		public GebExcelSystem(IDataSource source, GebCommon.GebConfig config, DataRow rqDetails, GebAlertRequest alertParametersBlob, GebConfigurationAlert alertParameters):base(){
			_source = source;								// Pour BDD
			_config = config;								// Pour param : email + dossier tmp
			_rqDetails = rqDetails;							// Ligne de 'static_nav_session' en cours
			_alertParametersBlob = alertParametersBlob;		// Pour param : date_num
			_alertParameters = alertParameters;				// Pour SQL
		}
		#endregion

		#region Destructeur
		~GebExcelSystem(){

		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient le chemin du fichier excel généré
		/// </summary>
		public string ExcelFilePath{
			get{return _excelFilePath;}
		}

		/// <summary>
		/// Ohemin le chemin du dossier contenant le fichier excel généré
		/// </summary>
		public string ExcelDirectoryPath{
			get{return _excelDirectoryPath;}
		}
		#endregion

		#region Fill
		/// <summary>
		/// Exécution des requêtes
		/// </summary>
		public void Fill(){
			// Détail support
            this.DetailMediaResult(_source,_alertParametersBlob,_alertParameters,_config);
			
			// Sauvegarde du fichier
			_excel.Save(_excelFilePath);
		}
		#endregion

		#region Excel Filename
		/// <summary>
		/// Generate a valid file name from customer request
		/// </summary>
		/// <param name="rqDetails">Details of the customer request</param>
		/// <param name="shortName">Return value : short name of the File (the method return the complet path)</param>
		/// <returns>Complet File Name String (path + short name)</returns>
		private string GetFileName(DataRow rqDetails,ref string shortName){
			string excelFileName;
			try{
				excelFileName = this._config.ExcelPath;
				excelFileName += @"\" + rqDetails["ID_LOGIN"].ToString() ;

				_excelDirectoryPath = excelFileName;

				if(!Directory.Exists(excelFileName)){
					Directory.CreateDirectory(excelFileName);
				}
				shortName = "PortofolioAlert_"+DateTime.Now.ToString("yyyyMMddHHmmss_") + rqDetails["id_static_nav_session"].ToString();
				excelFileName += @"\" + shortName + ".xls";

				string checkPath = 	Regex.Replace(excelFileName, @"(\.xls)+", ".xls", RegexOptions.IgnoreCase | RegexOptions.Multiline);
				int i = 0;
				while(System.IO.File.Exists(checkPath)){
					if(i<=1){
						checkPath = Regex.Replace(excelFileName, @"(\.xls)+", "_"+(i+1)+".xls", RegexOptions.IgnoreCase | RegexOptions.Multiline);
					}
					else{
						checkPath = Regex.Replace(excelFileName, "(_"+i+@"\.xls)+", "_"+(i+1)+".xls", RegexOptions.IgnoreCase | RegexOptions.Multiline);
					}
					i++;
				}
				return checkPath;
			}
			catch(System.Exception e){
				throw(new Exception("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() +".",e));
			}
		}
		#endregion

		#region Init
		/// <summary>
		/// Initialisation des paramètres du fichier
		/// </summary>
		internal string Init(){
			try{
				string shortFName = "";
				string fName =  GetFileName(_rqDetails, ref shortFName);	
				_excelFilePath = AppDomain.CurrentDomain.BaseDirectory+fName;
		
				return shortFName;
			}
			catch(System.Exception e){
				throw(e);
			}
		}
		#endregion

		#region Send
		/// <summary>
		/// Envoie le mail à l'utilisateur avec le fichier excel attaché
		/// </summary>
		/// <param name="fileName">Nom du fichier</param>
		internal void Send(){

			#region Variables
			ArrayList to = new ArrayList();
			string objet = "";
			string messageBody = "";
			StringBuilder t = new StringBuilder();
            string couvPath=String.Empty;
            string dateLabel = String.Empty;
                //=@"\\frmitch-fs03\quanti_multimedia_perf\AdexDatas\Press\SCANS\"+_alertParameters.MediaId+@"\"+_alertParametersBlob.DateMediaNum+@"\imagette\coe001.jpg";
			//string couvPathHtml=@"http://www.tnsadexpress.com/ImagesPresse/"+_alertParameters.MediaId+@"/"+_alertParametersBlob.DateMediaNum+@"/imagette/coe001.jpg";
			#endregion


            #region Objet du mail
            objet = GestionWeb.GetWebWord(1929,_alertParameters.LanguageId)
                + " " + _alertParameters.MediaName
                + " " + GestionWeb.GetWebWord(1729,_alertParameters.LanguageId)
                + " " + FrameworkDate.DateString.YYYYMMDDToDD_MM_YYYY(_alertParametersBlob.DateMediaNum,_alertParameters.LanguageId);
            #endregion

            #region Corps du message

            #region Message
            if(!_resultIsNull) {
                // Corps du message
                messageBody = GestionWeb.GetWebWord(1941,_alertParameters.LanguageId);

                // Couverture
                if(_mediaAntidated)
                    couvPath = @"\\frmitch-fs03\quanti_multimedia_perf\AdexDatas\Press\SCANS\" + _alertParameters.MediaId + @"\" + _alertParametersBlob.DateMediaNum + @"\imagette\coe001.jpg";
                else
                    couvPath = @"\\frmitch-fs03\quanti_multimedia_perf\AdexDatas\Press\SCANS\" + _alertParameters.MediaId + @"\" + this._dateCoverNum + @"\imagette\coe001.jpg";
            }
            else {
                // Pas de résultat
                messageBody = GestionWeb.GetWebWord(1942,_alertParameters.LanguageId);
            }
            #endregion

            t.Append("<html>");

            #region Style Css
            t.Append("<style type=\"text/css\">");
            t.Append("<!--");
            t.Append("body{font-family:Arial,Helvetica,sans-serif;font-size:12px;}");
            t.Append("a:link {font-size: 12px;text-decoration: none;color: #FF0099;}");
            t.Append("a:visited {font-size: 12px;text-decoration: none;color: #FF0099;}");
            t.Append("a:hover {font-size: 12px;text-decoration: underline;color: #FF0099;}");
            t.Append("a:active {font-size: 12px;text-decoration: none;color: #FF0099;}");
            t.Append("-->");
            t.Append("</style>");
            #endregion

            #region Body
            t.Append("<body><p>"+messageBody+"</p>");
            t.Append("<p align=\"center\">"+_alertParameters.MediaName + " " + GestionWeb.GetWebWord(1729,_alertParameters.LanguageId) + " " + FrameworkDate.DateString.YYYYMMDDToDD_MM_YYYY(_alertParametersBlob.DateMediaNum,_alertParameters.LanguageId) + "<br>");

            if(File.Exists(couvPath))
                t.Append("<img src=\"cid:123456789@GEG\" border=0>");

            t.Append("</p><p align=\"center\"><a href=\"http://www.tnsmediaintelligence.fr\">www.tnsmediaintelligence.fr</a></p>");
            t.Append("</body>");
            #endregion

            t.Append("</html>");
            #endregion

            #region Envoi du mail
            // Liste des destinataires
            foreach(string s in _alertParameters.EmailList) {
                to.Add(s);
            }
            SmtpUtilities mail = new SmtpUtilities(_config.CustomerMailFrom,to,objet,Convertion.ToHtmlString(t.ToString()),true,_config.CustomerMailServer,_config.CustomerMailPort);
            mail.mailKoHandler += new TNS.FrameWork.Net.Mail.SmtpUtilities.mailKoEventHandler(mail_mailKoHandler);

            // Fichier en pièce jointe uniquement s'il y a un résultat
            if(!_resultIsNull) {
                mail.Attach(_excelFilePath,SmtpUtilities.AttachmentType.ATTACH_EXCEL);
            }
            // Affichage de l'image dans le corps du message
            if(File.Exists(couvPath)) {
                mail.Attach(couvPath,SmtpUtilities.AttachmentType.ATTACH_JPEG_IN_MAIL,"123456789@GEG");
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
		private void mail_mailKoHandler(object source, string message) {
			throw new Exceptions.GebExcelSystemException("Echec lors de l'envoi du mail client pour le fichier excel de l'alerte portefeuille  : " + message);
		}
		#endregion

	}
}
