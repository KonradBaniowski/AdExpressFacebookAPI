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
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;

using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;

using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;

namespace AdExpress.Private.Results{
	/// <summary>
	/// PopUp donnant accès au téléchargement des créations radio et télévisions
	/// suivant les droits du clients et la disponibilité de la créations.
	/// </summary>
    public partial class DownloadCreationsPopUp : WebPage{

		#region Variables
		/// <summary>
		/// Identifiant d'une version
		/// </summary>
		string _idSlogan = null;
		/// <summary>
		/// Titre de la PopUp
		/// </summary>
		public string title="";
		/// <summary>
		/// Session utilisateur
		/// </summary>
		private WebSession _webSession;

		/// <summary>
		/// Indique si l'utilisateur à le droit de lire les créations
		/// </summary>
		private bool _hasCreationReadRights = false;
		
		/// <summary>
		/// Indique si l'utilisateur à le droit de télécharger les créations
		/// </summary>
		private bool _hasCreationDownloadRights = false;

		/// <summary>
		/// Rendu des crétaions en lecture
		/// </summary>
		public string streamingCreationsResult ="";

		/// <summary>
		/// Rendu des zones de texte 
		/// </summary>
		public string explanationTextResult ="";

		/// <summary>
		/// Chemin du fichier  Real média en lecture
		/// </summary>
		protected string pathReadingRealFile = null;

		/// <summary>
		/// Chemin du fichier  Real média en téléchargement
		/// </summary>
		protected string pathDownloadingRealFile = null;

		/// <summary>
		/// Chemin du fichier  Windows média en lecture
		/// </summary>
		protected string pathReadingWindowsFile = null;

		/// <summary>
		/// Chemin du fichier  Windows média en téléchargement
		/// </summary>
		protected string pathDownloadingWindowsFile = null;

		/// <summary>
		/// Code html de fermeture du flash d'attente
		/// </summary>
		public string divClose="";
		
		/// <summary>
		/// Identifiant création
		/// </summary>
		string file="";

	
		/// <summary>
		/// Média considéré
		/// </summary>
		protected CstClassification.DB.Vehicles.names vehicle=0;
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

		#region Evènement

		#region Chargement
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			string[] fileArr = null;

			#region Chargement de la session
			try{
				_webSession = (WebSession)WebSession.Load(Page.Request.QueryString.Get("idSession"));
					
			}
			catch(System.Exception){
				//_webSession.Source.Close();
				Response.Redirect("/Public/Message.aspx?msgTxt="+GestionWeb.GetWebWord(891, CstDB.Language.ENGLISH)+"&title="+GestionWeb.GetWebWord(887, CstDB.Language.ENGLISH));
			}
			#endregion
			
			//Titre de la popUp
			title = GestionWeb.GetWebWord(876, _webSession.SiteLanguage);			

			#region Construction et vérification des chemins d'accès aux fichiers real ou wm
			
			 file="";
			try{
				vehicle = (CstClassification.DB.Vehicles.names) int.Parse(Page.Request.QueryString.Get("idVehicle"));
				//_idSlogan = Page.Request.QueryString.Get("idSlogan");
				file = Page.Request.QueryString.Get("creation");
				if (vehicle != null && vehicle==CstClassification.DB.Vehicles.names.radio && file != null && file.Length > 0) {
					fileArr = file.Split(',');
					if (fileArr != null && fileArr.Length > 0) {
						file = fileArr[0];
						if (fileArr.Length > 1) _idSlogan = fileArr[1];
					}
				}
			}
			catch(System.Exception){
				_webSession.Source.Close();
				Response.Redirect("/Public/Message.aspx?msgTxt="+GestionWeb.GetWebWord(880, _webSession.SiteLanguage)+"&title="+GestionWeb.GetWebWord(887, _webSession.SiteLanguage));
			}

			#region Droits et sélection des options de résultats

			if (!Page.IsPostBack) {

                //L'utilisateur a accès au créations en lecture ?
                _hasCreationReadRights =_webSession.CustomerLogin.ShowCreatives(vehicle);
					
				if (_webSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DOWNLOAD_ACCESS_FLAG)) {
					//L'utilisateur a accès aux créations en téléchargement
					_hasCreationDownloadRights = true;
				}

			}
			#endregion

			//Vérification de l'existence des fichiers et construction des chemins d'accès suivant la volonté de 
			//lire ou de télécharger le fichier
			bool realFormatFound = true;
			bool windowsFormatFound = true;
			bool isNewRealAudioFilePath = false; 
			bool isNewWindowsAudioFilePath = false;
			
			switch(vehicle){
				case CstClassification.DB.Vehicles.names.radio:

					//Vérification de l'existence du fichier real
					if (_idSlogan != null && File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_CREATIVES_RADIO + @"\ra\2\" + _idSlogan.Substring(0, 3) + @"\2" + _idSlogan + ".ra")) {
						isNewRealAudioFilePath = realFormatFound = true;
					}
					else if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_RADIO+file.Replace("/","\\"))){
						realFormatFound = true;
					}
					else realFormatFound=false;

					//Vérification de l'existence du fichier wm
					if (_idSlogan != null && File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_CREATIVES_RADIO + @"\wma\2\" + _idSlogan.Substring(0, 3) + @"\2" + _idSlogan + ".wma")) {
						isNewWindowsAudioFilePath = windowsFormatFound = true;
					}
					else if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_RADIO+(file.Replace("/","\\")).Replace("rm","wma"))){
						 windowsFormatFound = true;
					}
					else windowsFormatFound=false;

					#region Test
#if DEBUG	
			//isNewRealAudioFilePath = isNewWindowsAudioFilePath = true;
#endif

					#endregion
					//Construction des chemins real et wm					
					if (_hasCreationReadRights) {
						//Fichiers en lectures (streaming)
						pathReadingRealFile = (isNewRealAudioFilePath) ? CstWeb.CreationServerPathes.READ_REAL_CREATIVES_RADIO_SERVER + "/2/" + _idSlogan.Substring(0, 3) + "/2" + _idSlogan + ".ra" : CstWeb.CreationServerPathes.READ_REAL_RADIO_SERVER + file;
						pathReadingWindowsFile = (isNewWindowsAudioFilePath) ? CstWeb.CreationServerPathes.READ_WM_CREATIVES_RADIO_SERVER + "/2/" + _idSlogan.Substring(0, 3) + "/2" + _idSlogan + ".wma" : CstWeb.CreationServerPathes.READ_WM_RADIO_SERVER + file.Replace("rm", "wma");
					}
					
					if(_hasCreationDownloadRights) {
						//Fichiers à télécharger 
						pathDownloadingRealFile = (isNewRealAudioFilePath) ? CstWeb.CreationServerPathes.DOWNLOAD_CREATIVES_RADIO_SERVER + "/ra/2/" + _idSlogan.Substring(0, 3) + "/2" + _idSlogan + ".ra" : CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER + file;
						pathDownloadingWindowsFile = (isNewWindowsAudioFilePath) ? CstWeb.CreationServerPathes.DOWNLOAD_CREATIVES_RADIO_SERVER + "/wma/2/" + _idSlogan.Substring(0, 3) + "/2" + _idSlogan + ".wma" : CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER + file.Replace("rm", "wma");
					}
					break;

				case CstClassification.DB.Vehicles.names.tv:

					if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO+@"\rm\3\"+file.Substring(0,3)+@"\3"+file+".rm")){
						realFormatFound=true;
					}
					else realFormatFound=false;
					if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO+@"\wmv\3\"+file.Substring(0,3)+@"\3"+file+".wmv")){
						windowsFormatFound=true;
					}
					else windowsFormatFound=false;
					
					//Construction des chemins real et wm	
					if(_hasCreationReadRights){
						//Fichiers en lectures (streaming)
						pathReadingRealFile = CstWeb.CreationServerPathes.READ_REAL_TV_SERVER+"/3/"+file.Substring(0,3)+"/3"+file+".rm";
						pathReadingWindowsFile = CstWeb.CreationServerPathes.READ_WM_TV_SERVER+"/3/"+file.Substring(0,3)+"/3"+file+".wmv";
					}

					if(_hasCreationDownloadRights) {
						//Fichiers à télécharger 
						pathDownloadingRealFile = CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER+"/rm/3/"+file.Substring(0,3)+"/3"+file+".rm";
						pathDownloadingWindowsFile = CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER+"/wmv/3/"+file.Substring(0,3)+"/3"+file+".wmv";
					}
					break;

				case CstClassification.DB.Vehicles.names.others:
					
					realFormatFound=false;

					if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_PAN_EURO+@"\"+file.Substring(0,4)+@"\"+file.Substring(4,2)+@"\"+file+".wmv")){
						windowsFormatFound=true;
					}
					else windowsFormatFound=false;
					
					//Construction des chemins real et wm	
					if(_hasCreationReadRights){
						pathReadingWindowsFile = CstWeb.CreationServerPathes.READ_WM_PAN_EURO_SERVER+"/"+file.Substring(0,4)+"/"+file.Substring(4,2)+"/"+file+".wmv";
					}

					if(_hasCreationDownloadRights) {
						pathDownloadingWindowsFile = CstWeb.CreationServerPathes.DOWNLOAD_PAN_EURO+"/"+file.Substring(0,4)+"/"+file.Substring(4,2)+"/"+file+".wmv";
					}
					break;
				default:
					_webSession.Source.Close();
					Response.Redirect("/Public/Message.aspx?msgTxt="+GestionWeb.GetWebWord(890, _webSession.SiteLanguage)+"&title="+GestionWeb.GetWebWord(887, _webSession.SiteLanguage));
					break;
			}
			#endregion

			#region Design Tableau d'images real ou wm suivant la disponibilité des fichiers
			
			#region Test
			#if DEBUG	
			//realFormatFound = true;
			//windowsFormatFound = true;
			#endif	

			#endregion		
			StringBuilder res = new StringBuilder(2000);

			
			if((realFormatFound || windowsFormatFound) && (_hasCreationReadRights || _hasCreationDownloadRights)){

				#region scripts
				//Gestion de l'ouverture du fichier windows media
				if (!Page.ClientScript.IsClientScriptBlockRegistered("GetObjectWindowsMediaPlayerRender")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"GetObjectWindowsMediaPlayerRender",GetObjectWindowsMediaPlayerRender(_webSession.SiteLanguage));
				}

				//Gestion de l'ouverture du fichier real media
				if (!Page.ClientScript.IsClientScriptBlockRegistered("GetObjectRealPlayer")) {
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"GetObjectRealPlayer",GetObjectRealPlayer());
				}
				#endregion

				res.Append("<TABLE cellSpacing=\"0\" cellPadding=\"10\" width=\"770\" border=\"0\" align=\"center\" height=\"100%\" ><TR>");
				//Rendu créations en lecture				
				if(_hasCreationReadRights)res.Append(ManageCreationsDownloadWithCookies(pathReadingRealFile,pathReadingWindowsFile,true,realFormatFound,windowsFormatFound)); 

				
				//Tableau des options
				res.Append(GetCreationsOptionsRender(realFormatFound,windowsFormatFound,pathDownloadingRealFile,pathDownloadingWindowsFile,false,2079));
			
			}else{
				//Aucun fichier n'est dispo	
				res.Append("<TABLE height=\"40%\"><TR><TD>&nbsp;</TD></TR></TABLE>");
				res.Append("<TABLE cellSpacing=\"0\" cellPadding=\"10\" width=\"440\" border=\"0\" align=\"center\" height=\"10%\" ><TR valign=\"middle\">");
                res.Append("<TD  align=\"center\" class=\"txtViolet11Bold whiteBackGround\" >");
				res.Append( GestionWeb.GetWebWord(892,_webSession.SiteLanguage));
				res.Append("</TD>");
				
			}
			res.Append("</TR></TABLE>");

			streamingCreationsResult = res.ToString();
			#endregion
		}
		#endregion

		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		#region Méthodes internes

		#region Gestion du Rendu
	
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
			if(CstClassification.DB.Vehicles.names.tv==vehicle)
			ds = TNS.AdExpress.Web.DataAccess.Results.VersionDataAccess.GetVersion(file,_webSession.SiteLanguage.ToString(),_webSession.Source);
			
			if((ds!=null && ds.Tables[0].Rows.Count>0) || (_hasCreationDownloadRights)){
                result.Append("<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"394\" align=\"center\" class=\"whiteBackGround\"><TBODY><TR><TD vAlign=\"top\">");
				//Détail version
			
				if(ds!=null && ds.Tables[0].Rows.Count>0){
			
					result.Append("<TR><TD><TABLE height=\"100%\"  ><TD>");

					result.Append("<TR class=\"txtViolet11Bold\" nowrap><TD>");	
					result.Append(GestionWeb.GetWebWord(859,_webSession.SiteLanguage));
					result.Append("</TD>");
					result.Append("<TD>");	
					result.Append("&nbsp;:&nbsp;"+ds.Tables[0].Rows[0]["group_"].ToString());
					result.Append("</TD></TR>");

					result.Append("<TR class=\"txtViolet11Bold\" nowrap><TD>");	
					result.Append(GestionWeb.GetWebWord(857,_webSession.SiteLanguage));
					result.Append("</TD>");
					result.Append("<TD>");	
					result.Append("&nbsp;:&nbsp;"+ds.Tables[0].Rows[0]["advertiser"].ToString());
					result.Append("</TD></TR>");			

					result.Append("<TR class=\"txtViolet11Bold\" nowrap><TD>");	
					result.Append(GestionWeb.GetWebWord(858,_webSession.SiteLanguage));
					result.Append("</TD>");
					result.Append("<TD>");	
					result.Append("&nbsp;:&nbsp;"+ds.Tables[0].Rows[0]["product"].ToString());
					result.Append("</TD></TR>");

					result.Append("</TD></TABLE></TD></TR>");

					result.Append("<TR height=100%><TD>&nbsp;</TD></TR>");

					withDetail = true;

				}
		
				if(_hasCreationDownloadRights){
					if(windowsFormatFound){
						result.Append("<tr vAlign=\"middle\"><td align=\"left\" ><span  id=txt_"+code+" class=\"txtViolet11Bold\">"+
							GestionWeb.GetWebWord(code,_webSession.SiteLanguage)+
							"</span></td></tr>");
						result.Append("<TR><TD align=\"left\">");
                        result.Append("<img src=/App_Themes/"+this.Theme+"/Images/Common/icoWindowsMediaPlayer.gif align=absmiddle>&nbsp;<a href=\"" + path2 + "\"  class=txtViolet11>" + GestionWeb.GetWebWord(2086, _webSession.SiteLanguage) + "</a>");
						result.Append("</td></tr>");
				
					}

					if(realFormatFound){
						if(!windowsFormatFound){
							result.Append("<tr vAlign=\"middle\"><td align=\"left\" ><span  id=txt_"+code+" class=\"txtViolet11Bold\">"+
								GestionWeb.GetWebWord(code,_webSession.SiteLanguage)+
								"</span></td></tr>");
						}
                        result.Append("<tr><td align=\"left\"><img src=/App_Themes/" + this.Theme + "/Images/Common/icoRealPlayer.gif align=absmiddle>&nbsp;<a href=\"" + path1 + "\"   class=txtViolet11>" + GestionWeb.GetWebWord(2085, _webSession.SiteLanguage) + "</a>");				
						result.Append("</td></tr>");
				
				
					}
					if(!withDetail)result.Append("<TR height=100%><TD>&nbsp;</TD></TR>");
				}

				result.Append("</TD></TR></TBODY></TABLE></TD>");
			}
		
			return(result.ToString());
			
			

			
		}

		/// <summary>
		/// Obtient le texte qui propose de télécharger le(s) lecteur(s) de fichier média
		/// </summary>
		/// <param name="realFormatFound">Indique si fichier real media existe</param>
		/// <param name="windowsFormatFound">Indique si fichier windows media existe</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Texte télécharger le(s) lecteur(s) de fichier média</returns>
		private static string DownLoadPlayerRender(bool windowsFormatFound, bool realFormatFound,WebSession webSession){
			StringBuilder result = new StringBuilder(1000);
			
				result.Append("<tr vAlign=\"middle\"><td align=\"left\" ><span  id=txt_ex class=\"txtViolet11Bold\">"+
				GestionWeb.GetWebWord(2087,webSession.SiteLanguage)+"</span>");

			if(windowsFormatFound){								
				result.Append("&nbsp;<a href=\"http://www.microsoft.com/Windows/MediaPlayer/\"  target=\"_blank\" class=txtViolet11>"+GestionWeb.GetWebWord(2088,webSession.SiteLanguage)+"</a>");							
			}

			if(realFormatFound){
				if(windowsFormatFound)result.Append(",");
				result.Append("&nbsp;<a href=\"http://www.real.com\"  target=\"_blank\" class=txtViolet11>"+GestionWeb.GetWebWord(2090,webSession.SiteLanguage)+"</a>");											
			}
			
			if(windowsFormatFound)result.Append(".</td></tr>");

			return(result.ToString());
		
		}

	
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
			if(Request.Browser.Cookies){
			//if(false){

				//Si les cookies existent				
				cspotFileType =  Request.Cookies[CstWeb.Cookies.SpotFileType];
										
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
                res.Append(" document.write('<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"368\" align=\"center\" class=\"whiteBackGround\"><TBODY><TR><TD>');");
				res.Append("\n  document.write('"+DownLoadPlayerRender(windowsFormatFound, realFormatFound,_webSession)+"');");
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

		/// <summary>
		/// Script d'ouveture d'un fichier Media Payer
		/// </summary>
		/// <returns>Script</returns>
		private string GetObjectWindowsMediaPlayerRender(int siteLanguage){

			StringBuilder res = new StringBuilder(2000);
			res.Append("<script language=\"JavaScript\" type=\"text/javascript\">"); 			
			res.Append(" function GetObjectWindowsMediaPlayerRender(filepath){");
            res.Append(" document.write('<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"368\" align=\"center\" class=\"whiteBackGround\"><TBODY><TR><TD>');");
			//Lecture par Media player
			res.Append(" document.write('<object id=\"video1\"  classid=\"CLSID:22D6F312-B0F6-11D0-94AB-0080C74C7E95\" height=\"288\" width=\"352\" align=\"middle\"  codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=6,4,5,715\"  standby=\""+GestionWeb.GetWebWord(1911,siteLanguage)+"\" type=\"application/x-oleobject\">');"); 			
			res.Append(" document.write('<param name=\"FileName\" value='+filepath+' >');");
			res.Append(" document.write('<param name=\"AutoStart\" value=\"true\">');");
			res.Append(" document.write('<embed type=\"application/x-mplayer2\" pluginspage=\"http://www.microsoft.com/Windows/MediaPlayer/\"  src='+filepath+' name=\"video1\" height=\"288\" width=\"352\" AutoStart=true>'); ");		
			res.Append(" document.write('</embed>');");
			res.Append(" document.write('</object>');");
			res.Append(" document.write('</TD></TR></TBODY></TABLE></TD>');");
			res.Append(" }");			
			res.Append("</script>");
			return res.ToString();
		}


		/// <summary>
		/// Script d'ouveture d'un fichier Real Payer
		/// </summary>
		/// <returns>Script</returns>
		private string GetObjectRealPlayer(){
			StringBuilder res = new StringBuilder(2000);
			res.Append("<script language=\"JavaScript\" type=\"text/javascript\">");		
			res.Append(" function GetObjectRealPlayer(filepath){");
            res.Append(" document.write('<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"368\" align=\"center\" class=\"whiteBackGround\"><TBODY><TR><TD>');");
			//Lecture par Real player
			res.Append(" document.write('<object classid=\"clsid:CFCDAA03-8BE4-11cf-B84B-0020AFBBCCFA\"  ID=\"Realaudio1\" height=\"288\" width=\"352\">');");
			res.Append(" document.write('<param name=\"console\" value=\"video\">');");
			res.Append(" document.write('<param name=\"controls\" value=\"all\">');");
			res.Append(" document.write('<param name=\"autostart\" value=\"true\">');");
			res.Append(" document.write('<param name=\"src\" value='+filepath+' >');");
			res.Append(" document.write('<embed  src='+filepath+' height=\"288\" width=\"352\" type=\"audio/x-pn-realaudio-plugin\"  controls=\"all\" pluginspage=\"http://www.real.com\" console=\"video\" autostart=\"true\">');");
			res.Append(" document.write('</embed>');");
			res.Append(" document.write('</object>');");
			res.Append(" document.write('</TD></TR></TBODY></TABLE></TD>');");
			res.Append(" }");			
			res.Append("</script>");
			return res.ToString();
		}

		
		#endregion
				
		#endregion

	}
}
