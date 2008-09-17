#region Informations
// Auteur : B.Masson
// Date de cr�ation : 21/04/2006
// Date de modification :
#endregion

using System;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.Geb.Common{
	/// <summary>
	/// Configuration du plugin Geb
	/// </summary>
	public class GebConfig{

		#region variables
		/// <summary>
		/// Chemin du fichier excel
		/// </summary>
		private string _excelPath;
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
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public GebConfig(IDataSource source){
			try{
				DataAccess.GebConfigDataAccess.Load(source,this);
			}
			catch(System.Exception e){
				throw(e);
			}
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou d�fini le chemin du fichier excel
		/// </summary>
		public string ExcelPath{
			get{return _excelPath;}
			set{_excelPath = value;}
		}

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
		#endregion

	}
}
