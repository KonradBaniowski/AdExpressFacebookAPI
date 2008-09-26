#region Information
/*
 * Author : D. Mussuma
 * Creation : 29/05/2006
 * Modifications :
 *		
 * */
#endregion

using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;

using TNS.AdExpress.Anubis.Satet.Common;
using TNS.AdExpress.Anubis.Satet.UI;
using TNS.AdExpress.Anubis.Common;

using TNS.FrameWork;
using TNS.FrameWork.Net.Mail;
using TNS.FrameWork.DB.Common;

using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;


namespace TNS.AdExpress.Anubis.Satet.BusinessFacade
{
	/// <summary>
	/// G�n�re le document excel pour les r�sultats APPM
	/// </summary>
	public class SatetExcelSystem : SatetExcel
	{
		#region Variables
		private IDataSource _dataSource = null;
		/// <summary>
		/// Satet Configuration
		/// </summary>
		private SatetConfig _config = null;
		/// <summary>
		/// Customer Client request
		/// </summary>
		private DataRow _rqDetails = null;
		/// <summary>
		/// WebSession to process
		/// </summary>
		private WebSession _webSession = null;
		/// <summary>
		/// Chemin du fichier excel g�n�r�
		/// </summary>
		private string _excelFilePath;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public SatetExcelSystem(IDataSource dataSource, SatetConfig config, DataRow rqDetails, WebSession webSession):base(){
		
			this._dataSource = dataSource;
			this._config = config;
			this._rqDetails = rqDetails;
			this._webSession = webSession;
		}
		#endregion

		#region Init
		/// <summary>
		/// Initialisation des param�tres du fichier
		/// </summary>
		internal string Init(){
			try{
				string shortFName = "";
				string fName =  GetFileName(_rqDetails, ref shortFName);	
				_excelFilePath = fName;
				return shortFName;
			}
			catch(Exception e){
				throw(new Exceptions.SatetDataAccessException("Erreur initialisation",e));
			}
		}
		#endregion

		#region Fill
		/// <summary>
		/// G�n�re les r�sultats APPM dans un document excel
		/// </summary>
		public void Fill(){

			//Page principale
			this.MainPageDesign(_webSession);
			//Param�tres d'�tude			
			UI.SessionParameter.SetExcelSheet(this._excel,_webSession,_dataSource);
			//Synth�se
			UI.Synthesis.SetExcelSheet(this._excel,_webSession,_dataSource);
			//Calendrier d'actions
			UI.MediaPlan.SetExcelSheet(this._excel,_webSession,_dataSource);
			//Analyse par titre
			UI.SupportPlan.SetExcelSheet(this._excel,_webSession,_dataSource);
			//Analyse des parts de voix
			UI.PDVPlan.SetExcelSheet(this._excel,_webSession,_dataSource);
			//Analyse par p�riodicit�
			UI.PeriodicityPlan.SetExcelSheet(this._excel,_webSession,_dataSource);
			//Analyse par famille de presse
			UI.AnalyseFamilyInterestPlan.SetExcelSheet(this._excel,_webSession,_dataSource);
			//Affinit�s
			UI.Affinities.SetExcelSheet(this._excel,_webSession,_dataSource);
			
			if(_excel!=null){
				this.Save(_excelFilePath);
			}

		}
		#endregion

		#region Send
		/// <summary>
		/// Envoie le mail � l'utilisateur avec le fichier excel attach�
		/// </summary>
		/// <param name="fileName"></param>
		internal void Send(){
			ArrayList to = new ArrayList();
			foreach(string s in _webSession.EmailRecipient){
				to.Add(s);
			}
			//			to.Add("dede.mussuma@tnsmi.fr");//test
			SmtpUtilities mail = new SmtpUtilities(_config.CustomerMailFrom, to,
				Text.SuppressAccent(GestionWeb.GetWebWord(1920,_webSession.SiteLanguage)),
				Text.SuppressAccent(GestionWeb.GetWebWord(1921,_webSession.SiteLanguage)+"\" "+_webSession.ExportedPDFFileName
				+ "\""+String.Format(GestionWeb.GetWebWord(1751,_webSession.SiteLanguage),_config.WebServer)				
				+ "<br><br>"
				+ GestionWeb.GetWebWord(1776,_webSession.SiteLanguage)),
				true, _config.CustomerMailServer, _config.CustomerMailPort);
			
			mail.mailKoHandler += new TNS.FrameWork.Net.Mail.SmtpUtilities.mailKoEventHandler(mail_mailKoHandler);
			mail.SendWithoutThread(false);
		}
		#endregion

		#region M�thodes priv�es
		
		#region Nom du fichier Excel
		/// <summary>
		/// Generate a valid file name from customer request
		/// </summary>
		/// <param name="rqDetails">Details of the customer request</param>
		/// <param name="shortName">Return value : short name of the File (the method return the complet path)</param>
		/// <returns>Complet File Name String (path + short name)</returns>
		private string GetFileName(DataRow rqDetails,ref string shortName){
			string txtFileName;

			try{

				txtFileName = this._config.ExcelPath;
				txtFileName += @"\" + rqDetails["ID_LOGIN"].ToString() ;

				if(!Directory.Exists(txtFileName)){
					Directory.CreateDirectory(txtFileName);
				}
				shortName = DateTime.Now.ToString("yyyyMMddHHmmss_")
					+ rqDetails["id_static_nav_session"].ToString()
					+ "_"
					+ TNS.AdExpress.Anubis.Common.Functions.GetRandomString(30,40);

				txtFileName += @"\" + shortName + ".xls";

				string checkPath = 	Regex.Replace(txtFileName, @"(\.xls)+", ".xls", RegexOptions.IgnoreCase | RegexOptions.Multiline);


				int i = 0;
				while(System.IO.File.Exists(checkPath)){
					if(i<=1){
						checkPath = Regex.Replace(txtFileName, @"(\.xls)+", "_"+(i+1)+".xls", RegexOptions.IgnoreCase | RegexOptions.Multiline);
					}
					else{
						checkPath = Regex.Replace(txtFileName, "(_"+i+@"\.xls)+", "_"+(i+1)+".xls", RegexOptions.IgnoreCase | RegexOptions.Multiline);
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

		#endregion

		#region Evenement Envoi mail client
		/// <summary>
		/// Rise exception when the customer mail has not been sent
		/// </summary>
		/// <param name="source">Error source></param>
		/// <param name="message">Error message</param>
		private void mail_mailKoHandler(object source, string message) {
			throw new Exceptions.SatetDataAccessException("Echec lors de l'envoi mail client pour l'export texte des insertions  : " + message);
		}
		#endregion

		
	}
}