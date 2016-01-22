#region Informations
///////////////////////////////////////////////////////////
//  SobekConfig.cs
//  Created on:  23-May.-2006 16:51:11
//  Original author: D.V. Mussuma,Y. Rkaina
///////////////////////////////////////////////////////////
#endregion

using System;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.Sobek.Common
{
	/// <summary>
	/// Description r�sum�e de SobekConfig.
	/// </summary>
	public class SobekConfig
	{
		#region variables
		/// <summary>
		/// Chemin du fichier texte
		/// </summary>
		private string _textFilePath;
		/// <summary>
		/// Serveur de mail d'envoie des r�sultats
		/// </summary>
		private string _customerMailServer;
		/// <summary>
		/// Port du serveur de mail d'envoie des r�sultats
		/// </summary>
		private int _customerMailPort=25;
		/// <summary>
		/// Mail d'envoie des r�sultats
		/// </summary>
		private string _customerMailFrom;
		/// <summary>
		/// Sujet des mails de r�sultats
		/// </summary>
		private string _customerMailSubject;
		/// Serveur web de stats
		/// </summary>
		private string _webServer;
		#endregion

		#region Propri�t�s
		/// <summary>
		/// Obtient le serveur des mails de r�sultats
		/// </summary>
		public string CustomerMailServer{
			get{return _customerMailServer;}
			set{_customerMailServer = value;}
		}
		/// <summary>
		/// Obtient le port du serveur des mails des r�sultats
		/// </summary>
		public int CustomerMailPort{
			get{return _customerMailPort;}
			set{_customerMailPort = value;}
		}
		/// <summary>
		/// Obtient le mail d'envoie des r�sultats 
		/// </summary>
		public string CustomerMailFrom{
			get{return _customerMailFrom;}
			set{_customerMailFrom = value;}
		}
		/// <summary>
		/// Obtient le sujet des mails de r�sultat
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
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="dataSource">Source de donn�es</param>
		public SobekConfig(IDataSource source){
			try{
				DataAccess.SobekConfigDataAccess.Load(source,this);
			}
			catch(System.Exception e){
				throw(e);
			}
		}
		#endregion
	}
}
