#region Informations
// Auteur : B.Masson
// Date de création : 21/04/2006
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
		/// Obtient ou défini le chemin du fichier excel
		/// </summary>
		public string ExcelPath{
			get{return _excelPath;}
			set{_excelPath = value;}
		}

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
		#endregion

	}
}
