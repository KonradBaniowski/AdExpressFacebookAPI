
#region Informations
// Auteur: D. Mussuma
// Date de création : 15/01/2007
// Date de modification : 
#endregion

using System;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.IO;

using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Sessions;

using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;
using Oracle.DataAccess.Client;

namespace TNS.AdExpress.Web.Controls.Results
{
	/// <summary>
	/// Contrôle donnant accès au téléchargement des créations radio et télévisions
	/// suivant  la disponibilité de la création.
	/// </summary>
	[ToolboxData("<{0}:AlertsDownloadCreationsWebControl runat=server></{0}:AlertsDownloadCreationsWebControl>")]
	public class AlertsDownloadCreationsWebControl : System.Web.UI.WebControls.WebControl
	{
		#region Variables
		/// <summary>
		/// Identifiant du media
		/// </summary>
		private string _idMedia = null;
		/// <summary>
		/// Identifiant du produit
		/// </summary>
		private string _idProduct = null;
		/// <summary>
		/// Date de début
		/// </summary>
		private string _dateBegin = null;
		/// <summary>
		/// Date de fin
		/// </summary>
		private string _dateEnd = null;
		/// <summary>
		/// Clé d'authentification
		/// </summary>
		private string _authentificationKey = null;
					
		/// <summary>
		/// Message d'erreur
		/// </summary>
		private string _errorMessage = null;
		
	
		/// <summary>
		/// Langue du site
		/// </summary>
		public int _siteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH;
	
		/// <summary>
		/// Identifiant du média
		/// </summary>
		protected Int64 _idVehicle = 0;


		/// <summary>
		/// Rendu des zones de texte 
		/// </summary>
		public string explanationTextResult ="";

		/// <summary>
		/// Chemin du fichier  Real média en lecture
		/// </summary>
		protected string _pathReadingRealFile = null;

		/// <summary>
		/// Chemin du fichier  Real média en téléchargement
		/// </summary>
		protected string _pathDownloadingRealFile = null;

		/// <summary>
		/// Chemin du fichier  Windows média en lecture
		/// </summary>
		protected string _pathReadingWindowsFile = null;

		/// <summary>
		/// Chemin du fichier  Windows média en téléchargement
		/// </summary>
		protected string _pathDownloadingWindowsFile = null;

		/// <summary>
		/// Identifiant création
		/// </summary>
		string _creationfile="";

	
		/// <summary>
		/// Média considéré
		/// </summary>
		protected CstClassification.DB.Vehicles.names _vehicle=0;

		/// <summary>
		/// Indique si l'utilisateur à accès à la page
		/// </summary>
		protected bool _isAuthorizeToViewCreation = false;

		/// <summary>
		/// Indique si fichier real  media existe
		/// </summary>
		protected bool _realFormatFound = true;

		/// <summary>
		/// Indique si fichier windows media  existe
		/// </summary>
		protected bool _windowsFormatFound = true;

		/// <summary>
		/// Identifiant de la version
		/// </summary>
		protected string _idSlogan = null;
		#endregion

		#region constantes
		/// <summary>
		/// Constante cookie pour fichier format windows media player 
		/// </summary>
		private const string WINDOWS_MEDIA_PLAYER_FORMAT ="windowsPlayerFormat";
		

		/// <summary>
		///  Constante cookie pour fichier format real media player 
		/// </summary>
		private const string REAL_MEDIA_PLAYER_FORMAT ="realPalyerFormat";
		#endregion

		#region Init
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {
			
			try{
				#region Récupération des paramètres de l'url
				//Récupération des paramètres de l'url
				string[] parametersList = null;
				if(Page.Request.QueryString.Get("parameters")!=null) 
					parametersList = Page.Request.QueryString.Get("parameters").ToString().Split(',');

				if(parametersList!=null){

					if(parametersList.Length>0)_idMedia = parametersList[0].ToString();
					if(parametersList.Length>1)_idProduct = parametersList[1].ToString();
					if(parametersList.Length>2)_dateBegin = parametersList[2].ToString();
					if(parametersList.Length>3)_dateEnd = parametersList[3].ToString();
					if(parametersList.Length>4)_authentificationKey = parametersList[4].ToString();
					_siteLanguage = (parametersList.Length>=6)? int.Parse(parametersList[5].ToString()): TNS.AdExpress.Constantes.DB.Language.FRENCH;
					
					//Calcul de la clé d'authentification pour le téléchargement de la création
					if(_idProduct!=null && _idMedia!=null && _dateBegin!=null && _dateEnd !=null){
						double creationdownloadKey = (Int64)Math.Abs((Int64)(Int64.Parse(_idProduct.ToString())*4)
							+9*Int64.Parse(_idMedia)+((int)(int.Parse(_dateBegin)*5)/2)-((int)(int.Parse(_dateEnd)*3)/9));
					
						if(creationdownloadKey != double.Parse(_authentificationKey))_errorMessage = GestionWeb.GetWebWord(2123, _siteLanguage);
					}
					
				}else _errorMessage = GestionWeb.GetWebWord(958, _siteLanguage);

	
				if(_errorMessage == null){
					if(Page.Request.QueryString.Get("idVehicle")!=null){
						_idVehicle = Int64.Parse(Page.Request.QueryString.Get("idVehicle"));
						_vehicle = (CstClassification.DB.Vehicles.names) _idVehicle;
					}
					if(Page.Request.QueryString.Get("creation")!=null) _creationfile = Page.Request.QueryString.Get("creation");
					if (Page.Request.QueryString.Get("idSlogan") != null) _idSlogan = Page.Request.QueryString.Get("idSlogan");
				}
				#endregion
			

				#region Construction et vérification des chemins d'accès aux fichiers real ou wm
			
			
				if(_errorMessage == null){
					//Vérification de l'existence des fichiers et construction des chemins d'accès suivant la volonté de 
					//lire ou de télécharger le fichier
					bool isNewRealAudioFilePath = false;
					bool isNewWindowsAudioFilePath = false;

					switch(_vehicle){
						case CstClassification.DB.Vehicles.names.radio:

							//Vérification de l'existence du fichier real
							#region Old version
							//if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_RADIO+_creationfile.Replace("/","\\"))){
							//    _realFormatFound = true;
							//}
							//else _realFormatFound = false;
							#endregion
							if (_idSlogan != null && _idSlogan.Length>0 && File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_CREATIVES_RADIO + @"\ra\2\" + _idSlogan.Substring(0, 3) + @"\2" + _idSlogan + ".ra")) {
								isNewRealAudioFilePath = _realFormatFound = true;
							}
							else if (_creationfile != null && _creationfile.Length > 0 && File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_RADIO + _creationfile.Replace("/", "\\"))) {
								_realFormatFound = true;
							}
							else _realFormatFound = false;

							//Vérification de l'existence du fichier wm

							#region Old version
							//if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_RADIO+(_creationfile.Replace("/","\\")).Replace("rm","wma"))){
							//    _windowsFormatFound = true;
							//}
							//else _windowsFormatFound = false;
							#endregion
							if (_idSlogan != null && _idSlogan.Length>0 && File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_CREATIVES_RADIO + @"\wma\2\" + _idSlogan.Substring(0, 3) + @"\2" + _idSlogan + ".wma")) {
								isNewWindowsAudioFilePath = _windowsFormatFound = true;
							}
							else if (_creationfile != null && _creationfile.Length > 0 && File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_RADIO + (_creationfile.Replace("/", "\\")).Replace("rm", "wma"))) {
								_windowsFormatFound = true;
							}
							else _windowsFormatFound = false;


							//Construction des chemins real et wm					

							//Fichiers en lectures (streaming)
							#region Old version
							//_pathReadingRealFile = CstWeb.CreationServerPathes.READ_REAL_RADIO_SERVER+_creationfile;
							//_pathReadingWindowsFile = CstWeb.CreationServerPathes.READ_WM_RADIO_SERVER+_creationfile.Replace("rm","wma");
							#endregion
							_pathReadingRealFile = (isNewRealAudioFilePath) ? CstWeb.CreationServerPathes.READ_REAL_CREATIVES_RADIO_SERVER + "/2/" + _idSlogan.Substring(0, 3) + "/2" + _idSlogan + ".ra" : ((_realFormatFound) ? CstWeb.CreationServerPathes.READ_REAL_RADIO_SERVER + _creationfile : "");
							_pathReadingWindowsFile = (isNewWindowsAudioFilePath) ? CstWeb.CreationServerPathes.READ_WM_CREATIVES_RADIO_SERVER + "/2/" + _idSlogan.Substring(0, 3) + "/2" + _idSlogan + ".wma" : ((_windowsFormatFound) ? CstWeb.CreationServerPathes.READ_WM_RADIO_SERVER + _creationfile.Replace("rm", "wma") : "");

							//Fichiers à télécharger 
							#region Old version
							//_pathDownloadingRealFile = CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER+_creationfile;
							//_pathDownloadingWindowsFile = CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER+_creationfile.Replace("rm","wma");
							#endregion
							_pathDownloadingRealFile = (isNewRealAudioFilePath) ? CstWeb.CreationServerPathes.DOWNLOAD_CREATIVES_RADIO_SERVER + "/ra/2/" + _idSlogan.Substring(0, 3) + "/2" + _idSlogan + ".ra" : ((_realFormatFound) ? CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER + _creationfile : "");
							_pathDownloadingWindowsFile = (isNewWindowsAudioFilePath) ? CstWeb.CreationServerPathes.DOWNLOAD_CREATIVES_RADIO_SERVER + "/wma/2/" + _idSlogan.Substring(0, 3) + "/2" + _idSlogan + ".wma" : ((_windowsFormatFound) ? CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER + _creationfile.Replace("rm", "wma") : "");

							break;

						case CstClassification.DB.Vehicles.names.tv:

							if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO+@"\rm\3\"+_creationfile.Substring(0,3)+@"\3"+_creationfile+".rm")){
								_realFormatFound = true;
							}
							else _realFormatFound = false;

							if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO+@"\wmv\3\"+_creationfile.Substring(0,3)+@"\3"+_creationfile+".wmv")){
								_windowsFormatFound = true;
							}
							else _windowsFormatFound=false;
					
							//Construction des chemins real et wm	
					
							//Fichiers en lectures (streaming)
							_pathReadingRealFile = CstWeb.CreationServerPathes.READ_REAL_TV_SERVER+"/3/"+_creationfile.Substring(0,3)+"/3"+_creationfile+".rm";
							_pathReadingWindowsFile = CstWeb.CreationServerPathes.READ_WM_TV_SERVER+"/3/"+_creationfile.Substring(0,3)+"/3"+_creationfile+".wmv";
												
							//Fichiers à télécharger 
							_pathDownloadingRealFile = CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER+"/rm/3/"+_creationfile.Substring(0,3)+"/3"+_creationfile+".rm";
							_pathDownloadingWindowsFile = CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER+"/wmv/3/"+_creationfile.Substring(0,3)+"/3"+_creationfile+".wmv";
						
							break;

						case CstClassification.DB.Vehicles.names.others:
					
							_realFormatFound=false;

							if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_PAN_EURO+@"\"+_creationfile.Substring(0,4)+@"\"+_creationfile.Substring(4,2)+@"\"+_creationfile+".wmv")){
								_windowsFormatFound=true;
							}
							else _windowsFormatFound=false;
					
							//Construction des chemins real et wm	

							//Fichiers en lectures (streaming)
							_pathReadingWindowsFile = CstWeb.CreationServerPathes.READ_WM_PAN_EURO_SERVER+"/"+_creationfile.Substring(0,4)+"/"+_creationfile.Substring(4,2)+"/"+_creationfile+".wmv";
						
							//Fichiers à télécharger 
							_pathDownloadingWindowsFile = CstWeb.CreationServerPathes.DOWNLOAD_PAN_EURO+"/"+_creationfile.Substring(0,4)+"/"+_creationfile.Substring(4,2)+"/"+_creationfile+".wmv";
						
							break;
						default:						
							_errorMessage = GestionWeb.GetWebWord(890, _siteLanguage);
							break;
					}

					#region Scripts
					//Gestion de l'ouverture du fichier windows media
					if (!this.Page.ClientScript.IsClientScriptBlockRegistered("GetObjectWindowsMediaPlayerRender")) {
						this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"GetObjectWindowsMediaPlayerRender",WebFunctions.Script.GetObjectWindowsMediaPlayerRender(_siteLanguage));
					}

					//Gestion de l'ouverture du fichier real media
					if (!this.Page.ClientScript.IsClientScriptBlockRegistered("GetObjectRealPlayer")) {
						this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"GetObjectRealPlayer", WebFunctions.Script.GetObjectRealPlayer());
					}
					#endregion
				}
				#endregion

			}
			catch(System.Exception ex){
				_errorMessage = GestionWeb.GetWebWord(1489, _siteLanguage);
				OnMethodError(ex);
			}

			base.OnInit (e);
		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output)
		{
//			#region Test
//						#if DEBUG	
////						_realFormatFound = true;
////						_windowsFormatFound = true;
//						#endif	
//
//			#endregion

			try{
				if(_errorMessage == null){

					if((_realFormatFound || _windowsFormatFound)){

						#region Design Tableau  real ou wm suivant la disponibilité des fichiers
						//Debut tableau global
						output.Write("<TABLE cellSpacing=\"0\" cellPadding=\"10\" width=\"770\" border=\"0\" align=\"center\" height=\"100%\" ><TR>");
					
						//Rendu créations en lecture
						output.Write(ManageCreationsDownloadWithCookies(_pathReadingRealFile,_pathReadingWindowsFile,true,_realFormatFound,_windowsFormatFound)); 

						//Tableau des options
						output.Write(GetCreationsOptionsRender(_realFormatFound,_windowsFormatFound,_pathDownloadingRealFile,_pathDownloadingWindowsFile,false,2079));

						//Fin tableau global
						output.Write("</TR></TABLE>");
						#endregion

					}else //Aucun fichier disponible
						SetErrorMessage(GestionWeb.GetWebWord(892,_siteLanguage),output);	
				}else{
					//erreur				
					SetErrorMessage(_errorMessage,output);				
				}
			
			}
			catch(System.Exception ex){
				_errorMessage = GestionWeb.GetWebWord(1489, _siteLanguage);
				OnMethodError(ex);
			}
		}
		#endregion

		#region Méthodes internes

		#region Messages d'erreur
		/// <summary>
		/// Definit le message d'erreur
		/// </summary>
		/// <param name="message">Message</param>
		/// <param name="output">Flux de sortie</param>
		private void SetErrorMessage(string message,HtmlTextWriter output){
			output.Write("<TABLE height=\"40%\"><TR><TD>&nbsp;</TD></TR></TABLE>");
			output.Write("<TABLE cellSpacing=\"0\" cellPadding=\"10\" width=\"440\" border=\"0\" align=\"center\" height=\"10%\" ><TR valign=\"middle\">");			
			output.Write("<TD  align=\"center\" bgColor=\"#ffffff\" class=\"txtViolet11Bold\" >");
			output.Write(message);
			output.Write("</TD>");
			output.Write("</TR></TABLE>");
		}
		#endregion

		#region Rendu son ou video
		/// <summary>
		/// Ouvre un fichier media en fonction des cookies 
		/// </summary>
		/// <param name="path1">chemin fichier rm</param>
		/// <param name="path2">chemin fichier wmv</param>
		/// <param name="read">Indique si fichier en lecture</param>
		/// <param name="realFormatFound">Indique si format trouvé est Real </param>
		/// <param name="windowsFormatFound">Indique si format trouvé est Windows</param>
		/// <returns>script gesion cookies traitement fichier</returns>
		private string ManageCreationsDownloadWithCookies(string path1, string path2,bool read,bool realFormatFound,bool windowsFormatFound){
			
			StringBuilder res = new StringBuilder(2000);
			HttpCookie cspotFileType = null;

			//Vérifie si le navigateur accepte les cookies
			if(this.Page.Request.Browser.Cookies){
				//if(false){

				//Si les cookies existent				
				cspotFileType =  this.Page.Request.Cookies[CstWeb.Cookies.SpotFileType];
										
				//Ouvrir le fichier en lecture seule
				if (realFormatFound && windowsFormatFound){
					//Lire le type de média stocké en cookie
					if(cspotFileType != null && cspotFileType.Value != null){
						switch(cspotFileType.Value){

							case WINDOWS_MEDIA_PLAYER_FORMAT :
								res.Append("<script language=\"JavaScript\" type=\"text/javascript\">GetObjectWindowsMediaPlayerRender('"+path2+"');</script>");							
								break;
						
							case REAL_MEDIA_PLAYER_FORMAT :
								res.Append("<script language=\"JavaScript\" type=\"text/javascript\">GetObjectRealPlayer('"+path1+"');</script>");
								break;

							default :
								break;

						}
					}else{
						//Sinon lire un fichier média par défaut (en fonction du média player du client)
						res.Append(ReadDefaultMediaPlayerRender(realFormatFound,windowsFormatFound,path1,path2));
					}

				}else if(realFormatFound){
					res.Append("<script language=\"JavaScript\" type=\"text/javascript\">\n GetObjectRealPlayer('"+path1+"');");
					res.Append("\n setCookie('"+CstWeb.Cookies.SpotFileType+"','"+REAL_MEDIA_PLAYER_FORMAT+"');");
					res.Append("</script>");
				}else{
					res.Append("<script language=\"JavaScript\" type=\"text/javascript\">\n GetObjectWindowsMediaPlayerRender('"+path2+"');");
					res.Append("\n setCookie('"+CstWeb.Cookies.SpotFileType+"','"+WINDOWS_MEDIA_PLAYER_FORMAT+"');");
					res.Append("</script>");
				}																																			

			
			}else{
				//Sinon lire un fichier média par défaut (en fonction du média player du client)
				res.Append(ReadDefaultMediaPlayerRender(realFormatFound,windowsFormatFound,path1,path2));
			}
								   
			return res.ToString();
			
		}

		#endregion

		#region Obtient le rendu pour téléchargement d'une création

		/// <summary>
		/// Obtient le rendu pour téléchargement d'une création
		/// </summary>
		/// <param name="realFormatFound">Indique si fichier real media existe</param>
		/// <param name="windowsFormatFound">Indique si fichier windows media existe</param>
		/// <param name="path1">Chemin fichier real media</param>
		/// <param name="path2">Chemin fichier windows media </param>
		/// <param name="manageQuote">Indique si les quote sont gérés</param>
		/// <param name="code">Code traduction</param>
		/// <returns>rendu par défaut ouverture fichier</returns>
		private string GetCreationsOptionsRender(bool realFormatFound, bool windowsFormatFound,string path1,string path2,bool manageQuote,long code){
			
			StringBuilder result = new StringBuilder(1000);
			bool withDetail = false;

			DataSet ds = null;
			//Connection
			string login ="gfacon", password="sandie5";
			string oracleConnectionString="User Id="+login+"; Password="+password+""+TNS.AdExpress.Constantes.DB.Connection.RIGHT_CONNECTION_STRING;
			TNS.FrameWork.DB.Common.IDataSource dataSource =null;

			//TODO recuperer les infos de la créations
			if(CstClassification.DB.Vehicles.names.tv==_vehicle){
				//Connection
				dataSource = new OracleDataSource(new OracleConnection(oracleConnectionString));

				ds = TNS.AdExpress.Web.DataAccess.Results.VersionDataAccess.GetVersion(_creationfile,_siteLanguage.ToString(),dataSource);
			}else{
				//Connection
				dataSource = new OracleDataSource(new OracleConnection(oracleConnectionString));

				ds = TNS.AdExpress.Web.DataAccess.Results.AlertsInsertionsCreationsDataAccess.GetProductInformations(dataSource,_idProduct,_siteLanguage.ToString());
			}
				result.Append("<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"394\" align=\"center\" bgColor=\"#ffffff\"><TBODY><TR><TD vAlign=\"top\">");
				//Détail version
			
				if(ds!=null && ds.Tables[0].Rows.Count>0){
			
					result.Append("<TR><TD><TABLE height=\"100%\"  ><TD>");

					result.Append("<TR class=\"txtViolet11Bold\" nowrap><TD>");	
					result.Append(GestionWeb.GetWebWord(859,_siteLanguage));
					result.Append("</TD>");
					result.Append("<TD>");	
					result.Append("&nbsp;:&nbsp;"+ds.Tables[0].Rows[0]["group_"].ToString());
					result.Append("</TD></TR>");

					result.Append("<TR class=\"txtViolet11Bold\" nowrap><TD>");	
					result.Append(GestionWeb.GetWebWord(857,_siteLanguage));
					result.Append("</TD>");
					result.Append("<TD>");	
					result.Append("&nbsp;:&nbsp;"+ds.Tables[0].Rows[0]["advertiser"].ToString());
					result.Append("</TD></TR>");			

					result.Append("<TR class=\"txtViolet11Bold\" nowrap><TD>");	
					result.Append(GestionWeb.GetWebWord(858,_siteLanguage));
					result.Append("</TD>");
					result.Append("<TD>");	
					result.Append("&nbsp;:&nbsp;"+ds.Tables[0].Rows[0]["product"].ToString());
					result.Append("</TD></TR>");

					result.Append("</TD></TABLE></TD></TR>");

					result.Append("<TR height=100%><TD>&nbsp;</TD></TR>");

					withDetail = true;

				}
		
				
					if(windowsFormatFound){
						result.Append("<tr vAlign=\"middle\"><td align=\"left\" ><span  id=txt_"+code+" class=\"txtViolet11Bold\">"+
							GestionWeb.GetWebWord(code,_siteLanguage)+
							"</span></td></tr>");
						result.Append("<TR><TD align=\"left\">");	
						result.Append("<img src=/Images/Common/icoWindowsMediaPlayer.gif align=absmiddle>&nbsp;<a href=\""+path2+"\"  class=txtViolet11>"+GestionWeb.GetWebWord(2086,_siteLanguage)+"</a>");
						result.Append("</td></tr>");
				
					}

					if(realFormatFound){
						if(!windowsFormatFound){
							result.Append("<tr vAlign=\"middle\"><td align=\"left\" ><span  id=txt_"+code+" class=\"txtViolet11Bold\">"+
								GestionWeb.GetWebWord(code,_siteLanguage)+
								"</span></td></tr>");
						}
						result.Append("<tr><td align=\"left\"><img src=/Images/Common/icoRealPlayer.gif align=absmiddle>&nbsp;<a href=\""+path1+"\"   class=txtViolet11>"+GestionWeb.GetWebWord(2085,_siteLanguage)+"</a>");				
						result.Append("</td></tr>");
				
				
					}
					if(!withDetail)result.Append("<TR height=100%><TD>&nbsp;</TD></TR>");
				

				result.Append("</TD></TR></TBODY></TABLE></TD>");
		
			return(result.ToString());
			
			

			
		}
		#endregion

		#region Obtient le texte qui propose de télécharger le(s) lecteur(s) de fichier média
		/// <summary>
		/// Obtient le texte qui propose de télécharger le(s) lecteur(s) de fichier média
		/// </summary>
		/// <param name="realFormatFound">Indique si fichier real media existe</param>
		/// <param name="windowsFormatFound">Indique si fichier windows media existe</param>
		/// <param name="siteLanguage">Langue</param>
		/// <returns>Texte télécharger le(s) lecteur(s) de fichier média</returns>
		private static string DownLoadPlayerRender(bool windowsFormatFound, bool realFormatFound,int siteLanguage){
			StringBuilder result = new StringBuilder(1000);
			
			result.Append("<tr vAlign=\"middle\"><td align=\"left\" ><span  id=txt_ex class=\"txtViolet11Bold\">"+
				GestionWeb.GetWebWord(2087,siteLanguage)+"</span>");

			if(windowsFormatFound){								
				result.Append("&nbsp;<a href=\"http://www.microsoft.com/Windows/MediaPlayer/\"  target=\"_blank\" class=txtViolet11>"+GestionWeb.GetWebWord(2088,siteLanguage)+"</a>");							
			}

			if(realFormatFound){
				if(windowsFormatFound)result.Append(",");
				result.Append("&nbsp;<a href=\"http://www.real.com\"  target=\"_blank\" class=txtViolet11>"+GestionWeb.GetWebWord(2090,siteLanguage)+"</a>");											
			}
			
			if(windowsFormatFound)result.Append(".</td></tr>");

			return(result.ToString());
		
		}
		#endregion

		#region Lecture par défaut d'un fichier
		/// <summary>
		/// Rendu par défaut pour la lecture d'un fichier media
		/// </summary>
		/// <param name="realFormatFound">Indique si fichier real media existe</param>
		/// <param name="windowsFormatFound">Indique si fichier windows media existe</param>
		/// <param name="path1">Chemin fichier real media</param>
		/// <param name="path2">Chemin fichier windows media </param>
		/// <returns>Rendu html lecture d'un fichier</returns>
		private string ReadDefaultMediaPlayerRender(bool realFormatFound, bool windowsFormatFound,string path1,string path2){
			
			StringBuilder res = new StringBuilder(2000);
			//Ajout des images à cliquer
			if (realFormatFound && windowsFormatFound){
					
				//Les deux fichiers sont disponibles
				res.Append("<script language=\"JavaScript\" type=\"text/javascript\">");

				//Si l'utilisateur possède le pugin Windows Media Player on charge le fichier windows Media 
				res.Append(" \n if(pluginlist.indexOf(\"Windows Media Player\")!=-1){");
				res.Append("	\n\t GetObjectWindowsMediaPlayerRender('"+path2+"');"); 
				res.Append(" \n\t setCookie('"+CstWeb.Cookies.SpotFileType+"','"+WINDOWS_MEDIA_PLAYER_FORMAT+"');");
				res.Append(" \n } "); 

				//Sinon si l'utilisateur possède le pugin Windows RealPlayer on charge le fichier real Media 
				res.Append(" \n else if(pluginlist.indexOf(\"RealPlayer\")!=-1){ ");
				res.Append("	\n\t GetObjectRealPlayer('"+path1+"');");
				res.Append(" \n\t setCookie('"+CstWeb.Cookies.SpotFileType+"','"+REAL_MEDIA_PLAYER_FORMAT+"');");
				res.Append(" \n } "); 
				res.Append("\n else{ ");
				res.Append(" document.write('<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"368\" align=\"center\" bgColor=\"#ffffff\"><TBODY><TR><TD>');");
				res.Append("\n  document.write('"+DownLoadPlayerRender(windowsFormatFound, realFormatFound,_siteLanguage)+"');");
				res.Append("  document.write('</TD></TR></TBODY></TABLE></TD>');");
				res.Append("\n } "); 
				res.Append("</script>");											
			}
			else{
				if (realFormatFound){
					//Seule le fichier real est disponible
					res.Append( "<script language=\"JavaScript\" type=\"text/javascript\">\n GetObjectRealPlayer('"+path1+"');");
					res.Append( "\n setCookie('"+CstWeb.Cookies.SpotFileType+"','"+REAL_MEDIA_PLAYER_FORMAT+"');");
					res.Append( "</script>");
									
				}
				else{
					//Seul le fichier wm est dispo
					res.Append( "<script language=\"JavaScript\" type=\"text/javascript\">\n GetObjectWindowsMediaPlayerRender('"+path2+"');");
					res.Append( "\n setCookie('"+CstWeb.Cookies.SpotFileType+"','"+WINDOWS_MEDIA_PLAYER_FORMAT+"');");
					res.Append( "</script>");
				}
			}
			return res.ToString();
		}
		#endregion

		#endregion

		#region OnMethodError
		/// <summary>
		/// Appelé sur erreur à l'exécution 
		/// </summary>
		/// <param name="errorException">Exception</param>		
		protected void OnMethodError(Exception errorException) {
			TNS.AdExpress.Web.Exceptions.CustomerWebException cwe=null;
			try{
				BaseException err=(BaseException)errorException;
				cwe=new TNS.AdExpress.Web.Exceptions.CustomerWebException(this.Page,err.Message,err.StackTrace,null);
			}
			catch(System.Exception){
				try{
					cwe=new TNS.AdExpress.Web.Exceptions.CustomerWebException(this.Page,errorException.Message,errorException.StackTrace,null);
				}
				catch(System.Exception es){
					throw(es);
				}
			}
			cwe.SendMail();			
		}
		#endregion
	}
}
