using System;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.HermesPress.Common{
	/// <summary>
	/// Description r�sum�e de Configuration.
	/// </summary>
	public class Configuration{

		#region Variables
		/// <summary>
		/// Serveur de mail d'envoi des r�sultats
		/// </summary>
		private string _customerMailServer;
		/// <summary>
		/// Port du serveur de mail d'envoi des r�sultats
		/// </summary>
		private int _customerMailPort=25;
		/// <summary>
		/// Mail d'envoi des r�sultats
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
		/// <param name="source">Source de donn�es</param>
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
		/// Obtient ou d�fini le serveur des mails de r�sultats
		/// </summary>
		public string CustomerMailServer{
			get{return _customerMailServer;}
			set{_customerMailServer = value;}
		}
		/// <summary>
		/// Obtient ou d�fini le port du serveur des mails des r�sultats
		/// </summary>
		public int CustomerMailPort{
			get{return _customerMailPort;}
			set{_customerMailPort = value;}
		}
		/// <summary>
		/// Obtient ou d�fini le mail d'envoi des r�sultats 
		/// </summary>
		public string CustomerMailFrom{
			get{return _customerMailFrom;}
			set{_customerMailFrom = value;}
		}
		/// <summary>
		/// Obtient ou d�fini le chemin du fichier de configuration
		/// </summary>
		public string ConfigurationReportFilePath{
			get{return _configurationReportFilePath;}
			set{_configurationReportFilePath = value;}
		}
		#endregion

	}
}
