#region Information
// Auteur: Guillaume Facon
// Créé le: 20/05/2005
// Modifiée le: 20/05/2005
#endregion

using System;
using TNSAnubisExceptions=TNS.AdExpress.Anubis.Exceptions;

namespace TNS.AdExpress.Anubis.Common.Network{
	/// <summary>
	/// Contient la configuration réseau du serveur
	/// </summary>
	public class WebClientConfiguration{

		#region Variables
		/// <summary>
		/// Nom du serveur
		/// </summary>
		protected static string _name;
		/// <summary>
		/// Adresse Ip
		/// </summary>
		protected static string _ip;
		/// <summary>
		/// Port de connexion
		/// </summary>
		protected static int _port;
		#endregion

		#region Methode Externes
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom du serveur</param>
		/// <param name="ip">Adresse Ip</param>
		/// <param name="port">Port de connexion</param>
		public static void Init(string name,string ip,int port){
			if(name==null || ip==null) throw(new ArgumentNullException("Au moins un des paramètres est null"));
			_name=name;
			_ip=ip;
			if(port<0) throw(new ArgumentNullException("Le port de connexion du est négatif"));
			_port=port;
		}
		#endregion

		#region Accesseurs

		/// <summary>
		/// Obtient le nom du serveur
		/// </summary>
		public static string Name{
			get{return(_name);}
			}

		/// <summary>
		/// Obtient l'adresse IP du serveur
		/// </summary>
		public static string IP{
			get{return(_ip);}
		}

		/// <summary>
		/// Obtient le port du serveur
		/// </summary>
		public static int Port{
			get{return(_port);}
		}
		#endregion
	}
}
