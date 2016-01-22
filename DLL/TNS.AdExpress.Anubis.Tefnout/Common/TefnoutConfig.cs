#region Information
//Author : D. Mussuma
//Creation : 29/05/2006
/* Modifications
 * */
#endregion
using System;
using TNS.FrameWork.DB.Common;


namespace TNS.AdExpress.Anubis.Tefnout.Common
{
	/// <summary>
	/// Static class containing every cofiguration elements for the Tefnout Plug-in
	/// </summary>
	public class TefnoutConfig
	{
		#region variables
		/// <summary>
		/// Chemin du fichier excel
		/// </summary>
		private string _excelPath;
		/// <summary>
		/// Serveur de mail d'envoie des résultats
		/// </summary>
		private string _customerMailServer;
		/// <summary>
		/// Port du serveur de mail d'envoie des résultats
		/// </summary>
		private int _customerMailPort=25;
		/// <summary>
		/// Mail d'envoie des résultats
		/// </summary>
		private string _customerMailFrom;
		/// <summary>
		/// Sujet des mails de résultats
		/// </summary>
		private string _customerMailSubject;
		/// Serveur web de stats
		/// </summary>
		private string _webServer;
        /// Theme xml file Path
        /// </summary>
        private string _themePath;
		#endregion

		#region Propriétés
		/// <summary>
		/// Obtient le serveur des mails de résultats
		/// </summary>
		public string CustomerMailServer{
			get{return _customerMailServer;}
			set{_customerMailServer = value;}
		}
		/// <summary>
		/// Obtient le port du serveur des mails des résultats
		/// </summary>
		public int CustomerMailPort{
			get{return _customerMailPort;}
			set{_customerMailPort = value;}
		}
		/// <summary>
		/// Obtient le mail d'envoie des résultats 
		/// </summary>
		public string CustomerMailFrom{
			get{return _customerMailFrom;}
			set{_customerMailFrom = value;}
		}
		/// <summary>
		/// Obtient le sujet des mails de résultat
		/// </summary>
		public string CustomerMailSubject{
			get{return _customerMailSubject;}
			set{_customerMailSubject = value;}
		}
		/// <summary>
		/// Chemin du fichier excel
		/// </summary>
		public string ExcelPath{
			get{return _excelPath;}
			set{_excelPath = value;}
		}
		
		/// <summary>
		/// Serveur web de stats
		/// </summary>
		public string WebServer{
			get{return _webServer;}
			set{_webServer = value;}
		}

        /// Get / Set Theme xml file Path
        /// </summary>
        public string ThemePath {
            get { return _themePath; }
            set { _themePath = value; }
        }
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="dataSource">Source de données</param>
		public TefnoutConfig(IDataSource source){
			try{
				DataAccess.TefnoutConfigDataAccess.Load(source,this);
			}
			catch(System.Exception e){
				throw(e);
			}
		}
		#endregion
	}
}
