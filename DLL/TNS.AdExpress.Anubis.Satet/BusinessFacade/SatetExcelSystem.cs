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

using TNS.FrameWork;
using TNS.FrameWork.Net.Mail;
using TNS.FrameWork.DB.Common;

using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Level;
using TNS.FrameWork.WebTheme;
using TNS.AdExpress.Anubis.Satet.Exceptions;

namespace TNS.AdExpress.Anubis.Satet.BusinessFacade
{
	/// <summary>
	/// Génère le document excel pour les résultats APPM
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
		/// Chemin du fichier excel généré
		/// </summary>
		private string _excelFilePath;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
        public SatetExcelSystem(IDataSource dataSource, SatetConfig config, DataRow rqDetails, WebSession webSession, Theme theme)
            : base(theme.GetStyle("Satet")) {
		
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
				throw(new Exceptions.SatetDataAccessException("Erreur initialisation",e));
			}
		}
		#endregion

		#region Fill
		/// <summary>
		/// Génère les résultats APPM dans un document excel
		/// </summary>
		public void Fill() {
            try {
                _webSession.PreformatedMediaDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory;

                // Initialisation à media\catégorie
                #region Niveau de détail media\Catégorie pour les autres planches (Generic)
                ArrayList levels = new ArrayList();
                levels.Add(1);
                levels.Add(2);
                _webSession.GenericMediaDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                #endregion

                //Page principale
                this.MainPageDesign(_webSession, _style);
                //Paramètres d'étude			
                UI.SessionParameter.SetExcelSheet(this._excel, _webSession, _dataSource, _style);
                //Synthèse
                UI.Synthesis.SetExcelSheet(this._excel, _webSession, _dataSource, _style);
                //Calendrier d'actions			
                #region Niveau de détail Categorie\Support pour plan media (Generic)
                //Categorie\Support 
                levels = new ArrayList();
                levels.Add(2);
                levels.Add(3);
                _webSession.GenericMediaDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                #endregion
                UI.MediaPlan.SetExcelSheet(this._excel, _webSession, _dataSource, _style);

                //Analyse par titre
                #region Niveau de détail media\Catégorie pour les autres planches (Generic)
                levels = new ArrayList();
                levels.Add(1);
                levels.Add(2);
                _webSession.GenericMediaDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                #endregion
                UI.SupportPlan.SetExcelSheet(this._excel, _webSession, _dataSource, _style);
                //Analyse des parts de voix
                UI.PDVPlan.SetExcelSheet(this._excel, _webSession, _dataSource, _style);
                //Analyse par périodicité
                UI.PeriodicityPlan.SetExcelSheet(this._excel, _webSession, _dataSource, _style);
                //Analyse par famille de presse
                UI.AnalyseFamilyInterestPlan.SetExcelSheet(this._excel, _webSession, _dataSource, _style);
                //Affinités
                UI.Affinities.SetExcelSheet(this._excel, _webSession, _dataSource, _style);

                if (_excel != null) {
                    this.Save(_excelFilePath);
                }
            }
            catch (Exception e) {
                throw new SatetDataAccessException("Error in Fill Excel", e);
            }
		}
		#endregion

		#region Send
		/// <summary>
		/// Envoie le mail à l'utilisateur avec le fichier excel attaché
		/// </summary>
		/// <param name="fileName"></param>
		internal void Send(){
            try {
                ArrayList to = new ArrayList();
                foreach (string s in _webSession.EmailRecipient) {
                    to.Add(s);
                }
                //			to.Add("dede.mussuma@tnsmi.fr");//test
                SmtpUtilities mail = new SmtpUtilities(_config.CustomerMailFrom, to,
                    Text.SuppressAccent(GestionWeb.GetWebWord(1920, _webSession.SiteLanguage)),
                    Text.SuppressAccent(GestionWeb.GetWebWord(1921, _webSession.SiteLanguage) + "\" " + _webSession.ExportedPDFFileName
                    + "\"" + String.Format(GestionWeb.GetWebWord(1751, _webSession.SiteLanguage), _config.WebServer)
                    + "<br><br>"
                    + GestionWeb.GetWebWord(1776, _webSession.SiteLanguage)),
                    true, _config.CustomerMailServer, _config.CustomerMailPort);

                mail.mailKoHandler += new TNS.FrameWork.Net.Mail.SmtpUtilities.mailKoEventHandler(mail_mailKoHandler);
                mail.SendWithoutThread(false);
            }
            catch (Exception e) {
                throw new SatetDataAccessException("Impossibnle to send mail to client", e);
            }
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
					+ TNS.Ares.Functions.GetRandomString(30,40);

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
