#region Informations
// Auteur: Y. R'kaina
// Date de création: 06/03/2007
// Date de modification:
#endregion

using System;
using System.Collections;
using System.Xml;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.Exceptions;

namespace TNS.AdExpress.Anubis.BusinessFacade{
	/// <summary>
	/// Description résumée de ReportingPluginBase.
	/// </summary>
	public abstract class ReportingPluginBase{

		#region Variables
		/// <summary>
		/// Le titre du rapport
		/// </summary>
		public string _reportTitle=string.Empty;
        /// <summary>
        /// Le temps d'exécution
        /// </summary>
		public TimeSpan _duration;
		/// <summary>
		/// L'heure de fin d'exécution
		/// </summary>
		public DateTime _endExecutionDateTime;
		/// <summary>
		/// Le corps du rapports
		/// </summary>
		public string _reportCore=string.Empty;
		/// <summary>
		/// Liste des mails
		/// </summary>
		public ArrayList _mailList=new ArrayList();
		/// <summary>
		/// Liste des erreurs survenues lors du traitement
		/// </summary>
		public ArrayList _errorList=new ArrayList();
		#endregion

		#region Chargement de la configuration
		/// <summary>
		/// Chargement de la configuration
		/// </summary>
		/// <param name="configurationFilePath">Le chemin du fichier de configuration</param>
		public void LoadReportingConfig(string configurationFilePath){
			XmlTextReader Reader;
			string Value="";
			IDataSource dataSrc;

			dataSrc = new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+configurationFilePath);

			try{
				Reader=(XmlTextReader)dataSrc.GetSource();
				// Parse XML file
				while(Reader.Read()){
					if(Reader.NodeType==XmlNodeType.Element){
						switch(Reader.LocalName){
							case "ReportTitle":
								Value=Reader.GetAttribute("title");
								if (Value!=null) _reportTitle = Value;
								break;
							case "Mail":
								Value=Reader.GetAttribute("to");
								if(Value!=null) _mailList.Add(Value);
								break;
						}
					}
				}
				Reader.Close();
			}
			catch(System.Exception err){
				throw (new ReportingPluginBaseDataAccessException("Unable to load reporting configuration",err));
			}
		}
		#endregion

		#region Gestion des erreurs
		/// <summary>
		/// Gestion des erreurs
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public void AddErrorMessage(string message){
			try{
				if(message.Length>0)
					_errorList.Add(message);
			}
			catch(System.Exception err){
				throw (new ReportingPluginErrorMessageException("Unable to add error message to the list of errors",err));
			}
		}
		#endregion

	}
}
