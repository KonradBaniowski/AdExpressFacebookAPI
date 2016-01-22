using System;
using System.Collections;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.BusinessFacade{

	#region Déclaration des types d'évènements

	/// <summary>
	/// Lancement du module
	/// </summary>
	public delegate void StartWork(Int64 navSessionId,string message);
	/// <summary>
	/// Arrêt du module
	/// </summary>
	public delegate void StopWorkerJob(Int64 navSessionId,string resultFilePath,string mail, string evtMessage);
	/// <summary>
	/// Message d'une alerte
	/// </summary>
	public delegate void MessageAlert(Int64 navSessionId,string message);
	/// <summary>
	/// Erreur
	/// </summary>
	public delegate void Error(Int64 navSessionId,string message,System.Exception e);
	/// <summary>
	/// Envoie des rapports
	/// </summary>
	public delegate void SendReport(string reportTitle, TimeSpan duration, DateTime endExecutionDateTime, string reportCore, ArrayList mailList, ArrayList errorList, string from, string mailServer, int mailPort, Int64 navSessionId);
	#endregion

	/// <summary>
	/// Interface à implémenté pour un plug-in de l'alerte
	/// </summary>
	public interface IPlugin{

		#region Evènements
		/// <summary>
		/// Lancement du module
		/// </summary>
		event StartWork OnStartWork;
		/// <summary>
		/// Arrêt du module
		/// </summary>
		event StopWorkerJob OnStopWorkerJob;
		/// <summary>
		/// Message d'une alerte
		/// </summary>
		event MessageAlert OnMessageAlert;
		/// <summary>
		/// Erreur
		/// </summary>
		event Error OnError;
		/// <summary>
		/// Envoie des rapports
		/// </summary>
		event SendReport OnSendReport;
		#endregion

		/// <summary>
		/// Obtient le nom du plug-in
		/// </summary>
		/// <returns></returns>
		string GetPluginName();

		/// <summary>
		/// Lance le traitement du résultat
		/// </summary>
		/// <param name="confifurationFilePath">Chemin de configuration du plug-in</param>
		/// <param name="dataSource">Source de données pour charger la session</param>
		/// <param name="navSessionId">Identifiant de la session à traiter</param>
		/// <remarks>
		/// Le traitement se charge de charger les données nécessaires à son execution et de lancer le résultat
		/// </remarks>
		void Treatement(string confifurationFilePath,IDataSource dataSource,Int64 navSessionId);
		/// <summary>
		/// Arrête le traitement du job
		/// </summary>
		void AbortTreatement();
	}
}

