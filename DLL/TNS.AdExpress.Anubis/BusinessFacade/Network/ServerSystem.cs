#region Informations
// Auteur: G. Facon
// Date de création: 19/05/2005
// Date de modification: 19/05/2005
#endregion

using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using TNS.AdExpress.Anubis.BusinessFacade.Core;
using TNSAnubisExceptions=TNS.AdExpress.Anubis.Exceptions;
using TNSAnubisCommon=TNS.AdExpress.Anubis.Common;
using TNSAnubisConstantes=TNS.AdExpress.Anubis.Constantes;

namespace TNS.AdExpress.Anubis.BusinessFacade.Network{
	/// <summary>
	/// Serveur acceptant les demandes clients
	/// </summary>
	public class ServerSystem{

		#region Variables
		/// <summary>
		/// Thread du serveur
		/// </summary>
		protected System.Threading.Thread _myThread;
		/// <summary>
		/// Configuration reseau du serveur
		/// </summary>
		protected TNSAnubisCommon.Network.Configuration _serverConfiguration;
		/// <summary>
		/// Le serveur doit il attendre des connexions
		/// </summary>
		protected bool waitForConnection=true;
	
		#endregion

		#region Evènements
		/// <summary>
		/// Démarrage du serveur reseau
		/// </summary>
		public delegate void NetworkServerStartHandler();
		/// <summary>
		/// arrêt du serveur reseau
		/// </summary>
		public delegate void NetworkServerStopHandler();
		/// <summary>
		/// Erreur du serveur reseau
		/// </summary>
		public delegate void NetworkServerErrorHandler(string message);
		/// <summary>
		/// Nouvelle connexion
		/// </summary>
		public delegate void NetworkServerNewConnectionHandler(string message);
		/// <summary>
		/// Evènement du démarrage du serveur reseau
		/// </summary>
		public event NetworkServerStartHandler NetworkServerStart;
		/// <summary>
		/// arrêt du serveur reseau
		/// </summary>
		public event NetworkServerStopHandler NetworkServerStop;
		/// <summary>
		/// Erreur du serveur reseau
		/// </summary>
		public event NetworkServerErrorHandler NetworkServerError;
		/// <summary>
		/// Nouvelle connexion
		/// </summary>
		public event NetworkServerNewConnectionHandler NetworkServerNewConnection;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="xmlFilePath">Chemin du fichier de configuration du serveur de demande de résultat</param>
		public ServerSystem(string xmlFilePath){

			#region Chargement de la configuration du serveur
			try{
				_serverConfiguration=ConfigurationSystem.Load(xmlFilePath);
			}
			catch(System.Exception err){
				throw(new TNSAnubisExceptions.ServerSystemException("Impossible de charger la configuration du serveur de demande",err));
			}
			#endregion

		}
		#endregion

		#region Méthodes Externes
		/// <summary>
		/// Lancement du serveur
		/// </summary>
		public void Treatement() {
			//OnStartPlugin("Début du traitement d'Horus");
			
			#region Lancement de la thread
			try{
				ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
				_myThread=new Thread(myThreadStart);
				_myThread.Name="Anubis Network Server";
				_myThread.Start();
			}
			catch(System.Exception err){
				throw(new TNSAnubisExceptions.ServerSystemException("Erreur lors de la création du serveur",err));
			}
			#endregion
		}
		#endregion

		#region Méthodes Internes
		/// <summary>
		/// Execution du serveur
		/// </summary>
		private void ComputeTreatement(){

			#region Variables
			Byte[] bytes = new Byte[256];
			String data = null;
			NetworkStream stream;
			TcpListener listener;
			Int32 numberOfBytesRead;
			#endregion

			#region Création du serveur d'écoute
			try{
				listener=new TcpListener(IPAddress.Parse(_serverConfiguration.IP),_serverConfiguration.Port);
				listener.Start();
				NetworkServerStart();
			}
			catch(System.Exception err){
				NetworkServerError("Impossible de démarrer le listener: "+err.Message+" "+err.StackTrace);
				return;
			}
			#endregion
			
			#region Ecoute
			while(waitForConnection){
				try{
					TcpClient client = listener.AcceptTcpClient();
					data = "";
					stream=null;
					stream = client.GetStream();

					do{
						numberOfBytesRead = stream.Read(bytes, 0, bytes.Length);  
						data = 
							String.Concat(data, System.Text.Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead));  
					}
					while(stream.DataAvailable);
					NetworkServerNewConnection("Data received: "+data);
					try{
						// Insère la demande dans le pool
						PoolSystem.Put(new TNSAnubisCommon.Core.PoolItem(data));
					}
					catch(System.Exception err){
						NetworkServerError("Impossible to add the request in the pool: "+err.Message+" - "+err.StackTrace);
					}

					//!!!!! Réponse (pour l'instant toujours ok !!!!!
					Byte[] msg = System.Text.Encoding.ASCII.GetBytes("0");
					stream.Write(msg, 0, msg.Length);

					client.Close();
				}
				catch(System.Exception err){
					NetworkServerError("Network error: "+err.Message+" "+err.StackTrace);
				}
			}
			NetworkServerStop();
			#endregion	
	
		}
		#endregion

	}
}
