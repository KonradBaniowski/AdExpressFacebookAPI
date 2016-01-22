#region Informations
///////////////////////////////////////////////////////////
//  SobekConfig.cs
//  Created on:  23-May.-2006 16:51:11
//  Original author: D.V. Mussuma,Y. Rkaina
///////////////////////////////////////////////////////////
#endregion

using System;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.Pachet.Common
{
	/// <summary>
	/// Description résumée de SobekConfig.
	/// </summary>
	public class PachetConfig
	{
		#region variables
		/// <summary>
		/// Chemin du fichier texte
		/// </summary>
		private string _textFilePath;
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
        /// <summary>
        /// default detail level
        /// </summary>
        private int _detailLevel = 0;
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
		/// Chemin du fichier texte
		/// </summary>
		public string TextFilePath{
			get{return _textFilePath;}
			set{_textFilePath = value;}
		}
		
		/// <summary>
		/// Serveur web de stats
		/// </summary>
		public string WebServer{
			get{return _webServer;}
			set{_webServer = value;}
		}

	    /// <summary>
	    /// default detail level
	    /// </summary>
	    public int DetailLevel
	    {
	        get { return _detailLevel; }
	        set { _detailLevel = value; }
	    }

	    #endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="dataSource">Source de données</param>
		public PachetConfig(IDataSource source){
			try{
                DataAccess.PachetConfigDataAccess.Load(source, this);
			}
			catch(System.Exception e){
				throw(e);
			}
		}
		#endregion
	}
}
