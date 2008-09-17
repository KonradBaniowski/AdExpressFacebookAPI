#region Information
 // Author : Y. R'kaina
 // Creation : 05/02/2007
 // Modifications :
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

using TNS.AdExpress.Anubis.Amset.Common;
using TNS.AdExpress.Anubis.Amset.Exceptions;
using TNS.AdExpress.Anubis.Amset.UI;
using TNS.AdExpress.Anubis.Common;

using TNS.FrameWork;
using TNS.FrameWork.Net.Mail;
using TNS.FrameWork.DB.Common;

using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.Translation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Anubis.Amset.BusinessFacade{
	/// <summary>
	/// Génère le document excel pour les résultats Amset
	/// </summary>
	public class AmsetExcelSystem: AmsetExcel{

		#region Variables
		private IDataSource _dataSource = null;
		/// <summary>
		/// Satet Configuration
		/// </summary>
		private AmsetConfig _config = null;
		/// <summary>
		/// Customer Client request
		/// </summary>
		private DataRow _rqDetails = null;
		/// <summary>
		/// WebSession to process
		/// </summary>
		private WebSession _webSession = null;
		/// <summary>
		/// Chemin du fichier excel généré
		/// </summary>
		private string _excelFilePath;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public AmsetExcelSystem(IDataSource dataSource, AmsetConfig config, DataRow rqDetails, WebSession webSession):base(){
			this._dataSource = dataSource;
			this._config = config;
			this._rqDetails = rqDetails;
			this._webSession = webSession;
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
				_excelFilePath = fName;
				return shortFName;
			}
			catch(Exception e){
				throw(new AmsetExcelSystemException("Erreur initialisation",e));
			}
		}
		#endregion

		#region Fill
		/// <summary>
		/// Génère les résultats Sector Data dans un document excel
		/// </summary>
		public void Fill(){

			//Page principale
			this.MainPageDesign(_webSession);
			//Paramètres d'étude			
			UI.SessionParameter.SetExcelSheet(this._excel,_webSession,_dataSource);
			//Synthesis
			UI.Synthesis.SetExcelSheet(this._excel,_webSession,_dataSource);
			//Average
			UI.Average.SetExcelSheet(this._excel,_webSession,_dataSource);
			//Seasonality
			if (_webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly)
				UI.Seasonality.SetExcelSheet(this._excel,_webSession,_dataSource);
			//Interest Family
			UI.InterestFamily.SetExcelSheet(this._excel,_webSession,_dataSource);
			//Periodicity
			UI.Periodicity.SetExcelSheet(this._excel,_webSession,_dataSource);
			//Affinities
			UI.Affinities.SetExcelSheet(this._excel,_webSession,_dataSource);
			
			if(_excel!=null){
				this.Save(_excelFilePath);
			}
		}
		#endregion

		#region Send
		/// <summary>
		/// Envoie le mail à l'utilisateur avec le fichier excel attaché
		/// </summary>
		/// <param name="fileName"></param>
		internal void Send(){
			ArrayList to = new ArrayList();
			foreach(string s in _webSession.EmailRecipient){
				to.Add(s);
			}
			SmtpUtilities mail = new SmtpUtilities(_config.CustomerMailFrom, to,
				Text.SuppressAccent(GestionWeb.GetWebWord(2107,_webSession.SiteLanguage)),
				Text.SuppressAccent(GestionWeb.GetWebWord(1921,_webSession.SiteLanguage)+"\" "+_webSession.ExportedPDFFileName
				+ "\""+String.Format(GestionWeb.GetWebWord(1751,_webSession.SiteLanguage),_config.WebServer)				
				+ "<br><br>"
				+ GestionWeb.GetWebWord(1776,_webSession.SiteLanguage)),
				true, _config.CustomerMailServer, _config.CustomerMailPort);
			
			mail.mailKoHandler += new TNS.FrameWork.Net.Mail.SmtpUtilities.mailKoEventHandler(mail_mailKoHandler);
			mail.SendWithoutThread(false);
		}
		#endregion

		#region Méthodes privées
		
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
				throw(new AmsetExcelSystemException("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() +".",e));
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
		private void mail_mailKoHandler(object source, string message){
			throw new Exceptions.AmsetExcelSystemException("Echec lors de l'envoi mail client pour l'export excel du module Données de cadrage : " + message);
		}
		#endregion

	}
}
