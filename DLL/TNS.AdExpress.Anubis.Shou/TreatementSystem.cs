#region Information
//Author : D. Mussuma
//Creation : 02/02/2007
#endregion

using System;
using System.Data;
using System.Threading;


using TNS.AdExpress.Anubis.BusinessFacade;
using TNS.AdExpress.Anubis.BusinessFacade.Result;
using TNS.AdExpress.Anubis.Common;

using TNS.AdExpress.Anubis.Shou.Common;
using TNS.AdExpress.Anubis.Shou.BusinessFacade;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Domain.Web.Navigation;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Theme;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Shou
{
	/// <summary>
	/// Implementation of TNS.AdExpress.Anubis.BusinessFacade.IPlugin for Shou plug-in
	/// </summary>
	public class TreatementSystem :TNS.AdExpress.Anubis.BusinessFacade.IPlugin {

		#region Evènements
		/// <summary>
		/// Lancement du module
		/// </summary>
		public event StartWork OnStartWork;
		/// <summary>
		/// Arrêt du module
		/// </summary>
		public event StopWorkerJob OnStopWorkerJob;
		/// <summary>
		/// Message d'une alerte
		/// </summary>
		public event MessageAlert OnMessageAlert;
		/// <summary>
		/// Message d'une alerte
		/// </summary>
		public event Error OnError;
		/// <summary>
		/// Envoie des rapports
		/// </summary>
		public event SendReport OnSendReport;
		#endregion

		#region Variables
		/// <summary>
		/// Thread qui traite l'alerte
		/// </summary>
		private System.Threading.Thread _myThread;
		/// <summary>
		/// Identifiant du résultat à traiter
		/// </summary>
		private Int64 _navSessionId;
		/// <summary>
		/// Source de données pour charger la session du résultat
		/// </summary>
		private IDataSource _dataSource;
		/// <summary>
		/// Configuration du plug-in
		/// </summary>
		private ShouConfig _shouConfig;
        /// <summary>
        /// Theme
        /// </summary>
        private Theme _theme;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public TreatementSystem() {
		}
		#endregion

		#region Nom du Plug-in
		/// <summary>
		/// Obtient le nom du plug-in
		/// </summary>
		/// <returns>Le nom du plug-in</returns>
		public string GetPluginName() {
			return("Shou PDF Generator");
		
		}
		#endregion

		#region Server start and stop
		/// <summary>
		/// Lance le traitement du résultat
		/// </summary>
		/// <param name="confifurationFilePath">Chemin de configuration du plug-in</param>
		/// <param name="dataSource">Source de données pour charger la session</param>
		/// <param name="navSessionId">Identifiant de la session à traiter</param>
		/// <remarks>
		/// Le traitement se charge de charger les données nécessaires à son execution et de lancer le résultat
		/// </remarks>
		public void Treatement(string configurationFilePath,IDataSource dataSource,Int64 navSessionId) {
			_navSessionId=navSessionId;

			#region Chargement du fichier de configuration
			if(configurationFilePath==null) {
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job", new ArgumentNullException("Le nom du fichier de configuration est null."));
				return;
			}
			if(configurationFilePath.Length==0) {
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job", new ArgumentException("Le nom du fichier de configuration est vide."));
				return;
			}
			try {
				_shouConfig=new ShouConfig(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+configurationFilePath));
			}
			catch(System.Exception err) {
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration",err);
				return;
			}
            try {
                _theme = new Theme(new XmlReaderDataSource(_shouConfig.ThemePath + @"\App_Themes\" + WebApplicationParameters.Themes[((ProofDetail)ParameterSystem.LoadProofDetail(_navSessionId)).CustomerWebSession.SiteLanguage].Name + @"\" + "Styles.xml"));
            }
            catch (System.Exception err) {
                OnError(_navSessionId, "File of theme not found ! (in Plugin APPM in TreatmentSystem class)",err);
            }
			#endregion			

			_dataSource=dataSource;
			
			ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
			_myThread=new Thread(myThreadStart);
			_myThread.Name="Fiche Justificative (Shou) PDF Generator";
			_myThread.Start();
		}

		/// <summary>
		/// Arrête le traitement du résultat
		/// </summary>
		public void AbortTreatement() {
			_myThread.Abort();
		}
		#endregion

		#region Traitment du résultat
		/// <summary>
		/// Generate the PDF for Shou plug-in
		/// </summary>
		private void ComputeTreatement() {
			ShouPdfSystem pdf = null;
			try {
				OnStartWork(_navSessionId,this.GetPluginName()+" started for "+_navSessionId);

				#region Request Details
				DataRow rqDetails = ParameterSystem.GetRequestDetails(_dataSource,_navSessionId).Tables[0].Rows[0];
				#endregion

				#region PDF management
				
				pdf = new ShouPdfSystem(_dataSource, _shouConfig,rqDetails,(ProofDetail)ParameterSystem.LoadProofDetail(_navSessionId),_theme);
				string fileName = pdf.Init();
                pdf.AutoLaunch = false;
				pdf.Fill();
		
				pdf.EndDoc();
				ParameterSystem.RegisterFile(_dataSource,_navSessionId,fileName);
				pdf.Send(fileName);
				ParameterSystem.ChangeStatus(_dataSource,_navSessionId,TNS.AdExpress.Anubis.Constantes.Result.status.sent);
				#endregion

				OnStopWorkerJob(_navSessionId,"","",this.GetPluginName()+" finished for "+_navSessionId);
			}
			catch(System.Exception err) {
				ParameterSystem.ChangeStatus(_dataSource,_navSessionId,TNS.AdExpress.Anubis.Constantes.Result.status.error);
				OnError(_navSessionId,"Erreur lors du traitement du résultat.", err);
				return;
			}
			finally {
				try {
					Functions.CleanWorkDirectory(pdf.GetWorkDirectory());
				}
				catch(System.Exception e) {
					int i = 0;
				}
			}

		}
		#endregion



	}
}
