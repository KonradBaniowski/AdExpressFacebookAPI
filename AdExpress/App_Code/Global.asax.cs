using System;
using System.Collections;
using System.ComponentModel;
using System.Web;

using System.Web.SessionState;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.ClassificationList;
using TNS.AdExpress.Web.Core.Navigation;
using TNS.AdExpress.Rules.Customer;
using TNS.AdExpress.Web;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNSMail=TNS.FrameWork.Net.Mail;
using AnubisConstantes=TNS.AdExpress.Anubis.Constantes;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.BusinessFacade.Selections.Medias;
using TNS.AdExpress.Web.Core;

using TNS.Classification;

namespace AdExpress {
	/// <summary>
	/// Description résumée de [!output SAFE_CLASS_NAME].
	/// </summary>
	public class Global : System.Web.HttpApplication{
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public Global(){
			InitializeComponent();
		}
		#endregion
	

		/// <summary>
		/// L'application est lancée
		/// </summary>
		/// <param name="sender">Objet Source</param>
		/// <param name="e">Arguments</param>
		protected void Application_Start(Object sender, EventArgs e){
		
			try{
				// Initialisation des listes de texte
				GestionWeb.Init();
				// Initialisation des descriptions des éléments de niveaux de détail
				DetailLevelItemsInformation.Init(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+TNS.AdExpress.Constantes.Web.ConfigurationFile.GENERIC_DETAIL_LEVEL_ITEMS_CONFIGURATION_PATH)); 
				// Initialisation des descriptions des niveaux de détail
				DetailLevelsInformation.Init(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+TNS.AdExpress.Constantes.Web.ConfigurationFile.GENERIC_DETAIL_LEVEL_CONFIGURATION_PATH)); 				
				// Initialisation des descriptions des colonnes génériques 
				GenericColumnItemsInformation.Init(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+TNS.AdExpress.Constantes.Web.ConfigurationFile.GENERIC_COLUMNS_ITEMS_CONFIGURATION_PATH)); 
				// Initialisation des descriptions des colonnes génériques prédéfinis
				GenericColumnsInformation.Init(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+TNS.AdExpress.Constantes.Web.ConfigurationFile.GENERIC_COLUMNS_ITEMS_CONFIGURATION_PATH));
				// Initialisation des descriptions des niveaux de détail pour les insertions
				InsertionDetailInformation.Init(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+TNS.AdExpress.Constantes.Web.ConfigurationFile.MEDIA_PLANS_INSERTION_CONFIGURATION_COLUMNS_ITEMS_CONFIGURATION_PATH));
                //Initialisation des colonnes par defaut pour le détail média du module portefeuille
                PortofolioDetailMediaColumnsInformation.Init(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory + TNS.AdExpress.Constantes.Web.ConfigurationFile.PORTOFOLIO_DETAIL_MEDIA_CONFIGURATION));
				// Chargement des niveaux de détail AdNetTrack
				AdNetTrackDetailLevelsDescription.Init(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+TNS.AdExpress.Constantes.Web.ConfigurationFile.ADNETTRACK_DETAIL_LEVEL_CONFIGURATION_PATH));
				// Chargement des noms de modules et des catégories de modules
				ModulesList.Init(TNS.AdExpress.Constantes.Web.ConfigurationFile.MODULE_CONFIGURATION_PATH, TNS.AdExpress.Constantes.Web.ConfigurationFile.MODULE_CATEGORY_CONFIGURATION_PATH);
//				TNS.AdExpress.Web.Core.Sessions.GenericDetailLevel t=(TNS.AdExpress.Web.Core.Sessions.GenericDetailLevel) ModulesList.GetModule(198).DefaultMediaDetailLevels[1]; 
//				string g=t.GetLabel(33);
				// Chargement des noms d'entetes
				HeaderRules.Init(TNS.AdExpress.Constantes.Web.ConfigurationFile.HEADER_CONFIGURATION_PATH);
				// Initialisation du cache des login
				//Logins.init();
				// Chargement de la configuration du serveur
				//Configuration.load(AppDomain.CurrentDomain.BaseDirectory+@"config\AdExpressConfiguration.xml");

				// Initialisation des listes d'univers média d'AdExpress
				TNS.AdExpress.Web.Core.ClassificationList.Media.Init();
				// Initialisation des listes d'univers produit d'AdExpress
				TNS.AdExpress.Web.Core.ClassificationList.Product.Init();

                ActiveMediaList.Init(); 

				//Initialisation de la configuration Reseau pour les résultats PDF
				//AnubisCommon.Network.Configuration networkConfig=AnubisBF.Network.ConfigurationSystem.Load(AppDomain.CurrentDomain.BaseDirectory+@"config\"+AnubisConstantes.Application.Configuration.NETWORK_FILE);
				//AnubisCommon.Network.WebClientConfiguration.Init(networkConfig.Name,networkConfig.IP,networkConfig.Port);
				//Initializing the hashtable containing publication dates for press media
				MediaPublicationDatesSystem.Init();


			//Initialisation des listes CSS
//			  CssStylesList.Init(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+TNS.AdExpress.Constantes.Web.ConfigurationFile.CSS_CONFIGURATION_PATH)); 
			
				//Charge les niveaux d'univers
				TNS.Classification.Universe.UniverseLevels.getInstance(new TNS.FrameWork.DB.Common.XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory + TNS.AdExpress.Constantes.Web.ConfigurationFile.UNIVERSE_LEVELS_CONFIGURATION_PATH));

				//Charge les styles personnalisés des niveaux d'univers
				TNS.Classification.Universe.UniverseLevelsCustomStyles.getInstance(new TNS.FrameWork.DB.Common.XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory + TNS.AdExpress.Constantes.Web.ConfigurationFile.UNIVERSE_LEVELS_CUSTOM_STYLES_CONFIGURATION_PATH));

				//Charge la hierachie de niveau d'univers
				TNS.Classification.Universe.UniverseBranches.getInstance(new TNS.FrameWork.DB.Common.XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory + TNS.AdExpress.Constantes.Web.ConfigurationFile.UNIVERSE_BRANCHES_CONFIGURATION_PATH));

			}
			catch(System.Exception error){
				string body="";
				try{
					BaseException err=(BaseException)error;
					body="<html><b><u>"+Server.MachineName+":</u></b><br>"+"<font color=#FF0000>L'initialisation du server a &eacute;chou&eacute;.</font><br>Erreur"+err.GetHtmlDetail()+"</font></html>";
				}
				catch(System.Exception){
					try{
						body="<html><b><u>"+Server.MachineName+":</u></b><br>"+"<font color=#FF0000>L'initialisation du server a &eacute;chou&eacute;.</font><br>Erreur("+error.GetType().FullName+"):"+error.Message+"<br><br><b><u>Source:</u></b><font color=#008000>"+error.StackTrace.Replace("at ","<br>at ")+"</font></html>";
					}
					catch(System.Exception es){
						throw(es);
					}
				}
				TNSMail.SmtpUtilities errorMail=new TNSMail.SmtpUtilities(AppDomain.CurrentDomain.BaseDirectory+WebConstantes.ErrorManager.WEBSERVER_ERROR_MAIL_PATH);
				errorMail.Send("Erreur d'initialisation d'AdExpress "+(Server.MachineName),body,true,false);
				throw(error);
			}
		}


		#region Autres évènements non gérés
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Session_Start(Object sender, EventArgs e){

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_BeginRequest(Object sender, EventArgs e){
			//string titi=Request.Path;

		}
		/// <summary>
		///		
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_EndRequest(Object sender, EventArgs e){

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_AuthenticateRequest(Object sender, EventArgs e){

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_Error(Object sender, EventArgs e){			
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Session_End(Object sender, EventArgs e){

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_End(Object sender, EventArgs e){						

		}
		#endregion
			
		#region Web Form Designer generated code
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent(){    
		}
		#endregion
	}
}

