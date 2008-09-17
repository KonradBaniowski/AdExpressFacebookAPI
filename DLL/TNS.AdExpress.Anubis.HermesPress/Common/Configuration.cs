using System;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.HermesPress.Common{
	/// <summary>
	/// Description résumée de Configuration.
	/// </summary>
	public class Configuration{

		#region Variables
		/// <summary>
		/// Serveur de mail d'envoi des résultats
		/// </summary>
		private string _customerMailServer;
		/// <summary>
		/// Port du serveur de mail d'envoi des résultats
		/// </summary>
		private int _customerMailPort=25;
		/// <summary>
		/// Mail d'envoi des résultats
		/// </summary>
		private string _customerMailFrom;
		/// <summary>
		/// Chemin du fichier de configuration
		/// </summary>
		private string _configurationReportFilePath;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="source">Source de données</param>
		public Configuration(IDataSource source){
			try{
				DataAccess.ConfigurationDataAccess.Load(source,this);
			}
			catch(System.Exception e){
				throw(e);
			}
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou défini le serveur des mails de résultats
		/// </summary>
		public string CustomerMailServer{
			get{return _customerMailServer;}
			set{_customerMailServer = value;}
		}
		/// <summary>
		/// Obtient ou défini le port du serveur des mails des résultats
		/// </summary>
		public int CustomerMailPort{
			get{return _customerMailPort;}
			set{_customerMailPort = value;}
		}
		/// <summary>
		/// Obtient ou défini le mail d'envoi des résultats 
		/// </summary>
		public string CustomerMailFrom{
			get{return _customerMailFrom;}
			set{_customerMailFrom = value;}
		}
		/// <summary>
		/// Obtient ou défini le chemin du fichier de configuration
		/// </summary>
		public string ConfigurationReportFilePath{
			get{return _configurationReportFilePath;}
			set{_configurationReportFilePath = value;}
		}
		#endregion

	}
}
