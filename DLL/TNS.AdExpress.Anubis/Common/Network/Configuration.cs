#region Information
// Auteur: Guillaume Facon
// Cr�� le: 18/05/2005
// Modifi�e le: 18/05/2005
#endregion

using System;
using TNSAnubisExceptions=TNS.AdExpress.Anubis.Exceptions;

namespace TNS.AdExpress.Anubis.Common.Network{
	/// <summary>
	/// Contient la configuration r�seau du serveur
	/// </summary>
	public class Configuration{

		#region Variables
		/// <summary>
		/// Nom du serveur
		/// </summary>
		protected string _name;
		/// <summary>
		/// Adresse Ip
		/// </summary>
		protected string _ip;
		/// <summary>
		/// Port de connexion
		/// </summary>
		protected int _port;
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom du serveur</param>
		/// <param name="ip">Adresse Ip</param>
		/// <param name="port">Port de connexion</param>
		public Configuration(string name,string ip,int port){
			if(name==null || ip==null) throw(new ArgumentNullException("Au moins un des param�tres est null"));
			_name=name;
			_ip=ip;
			if(port<0) throw(new ArgumentException("Le port de connexion du est n�gatif"));
			_port=port;
		}
		#endregion

		#region Accesseurs

		/// <summary>
		/// Obtient le nom du serveur
		/// </summary>
		public string Name{
			get{return(_name);}
		}

		/// <summary>
		/// Obtient l'adresse IP du serveur
		/// </summary>
		public string IP{
			get{return(_ip);}
		}

		/// <summary>
		/// Obtient le port du serveur
		/// </summary>
		public int Port{
			get{return(_port);}
		}
		#endregion
	}
}
