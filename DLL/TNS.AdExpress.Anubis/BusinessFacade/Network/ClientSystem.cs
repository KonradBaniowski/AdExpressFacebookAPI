#region Informations
// Auteur: G. Facon
// Date de création: 20/05/2005
// Date de modification: 20/05/2005
#endregion

using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using TNS.AdExpress.Anubis.Constantes;
using TNS.AdExpress.Anubis.Constantes.Network;
using TNSAnubisExceptions=TNS.AdExpress.Anubis.Exceptions;
using TNSAnubisCommon=TNS.AdExpress.Anubis.Common;



namespace TNS.AdExpress.Anubis.BusinessFacade.Network{
	/// <summary>
	/// Client Reseau
	/// </summary>
	public class ClientSystem{
		#region Variables
		#endregion


		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public ClientSystem(){
		}
		#endregion

		#region Methode Externes

		/// <summary>
		/// Envoie une demande résultat au serveur
		/// </summary>
		/// <param name="ip">Adresse IP du serveur</param>
		/// <param name="port">Port du serveur</param>
		/// <param name="idStaticNavSession">Session sauvegardée du client</param>
		/// <param name="resultType">Type de résultat souhaité</param>
		/// <returns>Code du serveur</returns>
		public static Server.code Send(string ip,int port,Int64 idStaticNavSession,TNS.AdExpress.Anubis.Constantes.Result.type resultType){

			#region Variables
			TcpClient client;
			String responseData = String.Empty;
			#endregion

			#region Création du client reseau
			try{
				client=new TcpClient();
				client.Connect(IPAddress.Parse(ip),port);
			}
			catch(System.Exception err){
				throw(new TNSAnubisExceptions.ClientSystemException("Impossible de créer le client reseau:",err));
			}
			#endregion

			#region Envoie la demande
			Byte[] data = System.Text.Encoding.ASCII.GetBytes(idStaticNavSession.ToString());           
			NetworkStream stream = client.GetStream();
			stream.Write(data, 0, data.Length);
			#endregion

			#region Réponse du serveur
			data = new Byte[256];
			Int32 bytes = stream.Read(data, 0, data.Length);
			responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);        
			#endregion

			#region Fermeture du client
			// Close everything.
			client.Close();
			client=null;
			#endregion

			return((Server.code)int.Parse(responseData));
		}

		#endregion
	}
}
