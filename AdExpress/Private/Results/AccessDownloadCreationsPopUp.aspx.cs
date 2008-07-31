#region Information
/*
 * auteur : Guillaume Ragneau
 * créé le :
 * modifié le : 30/08/2004
 * par : Guillaume Ragneau
 * */
#endregion

using System;
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

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;

using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;


namespace AdExpress.Private.Results {
	/// <summary>
	/// PopUp donnant accès au téléchargement des créations radio et télévisions
	/// suivant les droits du clients et la disponibilité de la créations.
	/// </summary>
	public partial class AccessDownloadCreationsPopUp : System.Web.UI.Page {
		
		#region Variables
		/// <summary>
		/// Ensemble de radio button donnant accès aux créations (lecture / téléchargement)
		/// </summary>
		/// <summary>
		/// Session utilisateur
		/// </summary>
		private WebSession _webSession;
		/// <summary>
		/// Titre de la PopUp
		/// </summary>
		public string title="";
		/// <summary>
		/// Mode d'emploi de la PopUp
		/// </summary>
		/// <summary>
		/// Code Html inséré dans la page aspx
		/// </summary>
		public string creationsLine="";

		#endregion

		#region Evènement

		#region Chargement
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e) {

			#region Chargement de la session
			try{
				_webSession = (WebSession)WebSession.Load(Page.Request.QueryString.Get("idSession"));
			}
			catch(System.Exception){
				_webSession.Source.Close();
				Response.Redirect("/Public/Message.aspx?msgTxt="+GestionWeb.GetWebWord(891, CstDB.Language.ENGLISH)+"&title="+GestionWeb.GetWebWord(887, CstDB.Language.ENGLISH));
			}
			#endregion

			#region Design fenêtre
			//Titre de la popUp
			title = GestionWeb.GetWebWord(876, _webSession.SiteLanguage);
			//Construction des boutons radio permettant la visualisation ou la sauvegarde d'un création
			if (!Page.IsPostBack){
				RadioButtonList1.Items.Clear();
				if(_webSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_CREATION_ACCESS_FLAG)){
					//L'utilisateur a accès au créations en lecture
					RadioButtonList1.Items.Add(GestionWeb.GetWebWord(873,_webSession.SiteLanguage));
				}
				if(_webSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DOWNLOAD_ACCESS_FLAG)){
					//L'utilisateur a accès aux créations en téléchargement
					RadioButtonList1.Items.Add(GestionWeb.GetWebWord(874,_webSession.SiteLanguage));
				}
				RadioButtonList1.Items[0].Selected = true;
			}
			#endregion

			#region Choix de l'utilisateur
			bool read = true;
			if(RadioButtonList1.Items[RadioButtonList1.SelectedIndex].Text.CompareTo(GestionWeb.GetWebWord(873,_webSession.SiteLanguage))!=0)
				//l'utilisateur veut télécharger le fichier
				read=false;
			#endregion
		
			#region Construction et vérification des chemins d'accès aux fichiers real ou wm
			//Média considéré
			CstClassification.DB.Vehicles.names vehicle=0;
			string file="";
			try{
				vehicle = (CstClassification.DB.Vehicles.names) int.Parse(Page.Request.QueryString.Get("idVehicle"));
				file = Page.Request.QueryString.Get("creation");
			}
			catch(System.Exception){
				_webSession.Source.Close();
				Response.Redirect("/Public/Message.aspx?msgTxt="+GestionWeb.GetWebWord(880, _webSession.SiteLanguage)+"&title="+GestionWeb.GetWebWord(887, _webSession.SiteLanguage));
			}

			//Vérification de l'existence des fichiers et construction des chemins d'accès suivant la volonté de 
			//lire ou de télécharger le fichier
			bool realFormatFound=true;
			bool windowsFormatFound=true;
			string path1="";
			string path2="";
			string tmp ="";

			switch(vehicle){
				case CstClassification.DB.Vehicles.names.radio:
					//Vérification de l'existence du fichier real
					if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_RADIO+file.Replace("/","\\"))){
						realFormatFound=true;
					}
					else realFormatFound=false;
					//Vérification de l'existence du fichier wm
					if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_RADIO+(file.Replace("/","\\")).Replace("rm","wma"))){
						windowsFormatFound=true;
					}
					else windowsFormatFound=false;
					//Construction des chemons real et wm
					if(read){
						path1=CstWeb.CreationServerPathes.READ_REAL_RADIO_SERVER+file;
						path2=CstWeb.CreationServerPathes.READ_WM_RADIO_SERVER+file.Replace("rm","wma");
					}
					else {
						path1=CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER+file;
						path2=CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER+file.Replace("rm","wma");
					}
					break;
				case CstClassification.DB.Vehicles.names.tv:
					if (File.Exists(tmp=CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO+@"\rm\3\"+file.Substring(0,3)+@"\3"+file+".rm")){
						realFormatFound=true;
					}
					else realFormatFound=false;
					if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO+@"\wmv\3\"+file.Substring(0,3)+@"\3"+file+".wmv")){
						windowsFormatFound=true;
					}
					else windowsFormatFound=false;					
					if(read){
						path1=CstWeb.CreationServerPathes.READ_REAL_TV_SERVER+"/3/"+file.Substring(0,3)+"/3"+file+".rm";
						path2=CstWeb.CreationServerPathes.READ_WM_TV_SERVER+"/3/"+file.Substring(0,3)+"/3"+file+".wmv";
					}
					else {
						path1=CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER+"/rm/3/"+file.Substring(0,3)+"/3"+file+".rm";
						path2=CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER+"/wmv/3/"+file.Substring(0,3)+"/3"+file+".wmv";
					}
					break;
				case CstClassification.DB.Vehicles.names.others:
					//if (File.Exists(tmp=CstWeb.CreationServerPathes.LOCAL_PATH_PAN_EURO+@"\rm\3\"+file.Substring(0,3)+@"\3"+file+".rm")){
					//	realFormatFound=true;
					//	}
					//else
					realFormatFound=false;
					//string test=CstWeb.CreationServerPathes.LOCAL_PATH_PAN_EURO+@"\"+file.Substring(0,4)+@"\"+file.Substring(4,2)+@"\"+file+".wmv";
					if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_PAN_EURO+@"\"+file.Substring(0,4)+@"\"+file.Substring(4,2)+@"\"+file+".wmv")){
						windowsFormatFound=true;
					}
					else windowsFormatFound=false;					
					if(read){
					//	path1=CstWeb.CreationServerPathes.READ_REAL_TV_SERVER+"/3/"+file.Substring(0,3)+"/3"+file+".rm";						
						path2=CstWeb.CreationServerPathes.READ_WM_PAN_EURO_SERVER+"/"+file.Substring(0,4)+"/"+file.Substring(4,2)+"/"+file+".wmv";
					}
					else {
					//	path1=CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER+"/rm/3/"+file.Substring(0,3)+"/3"+file+".rm";
						path2=CstWeb.CreationServerPathes.DOWNLOAD_PAN_EURO+"/"+file.Substring(0,4)+"/"+file.Substring(4,2)+"/"+file+".wmv";
					}
					break;
				default:
					_webSession.Source.Close();
					Response.Redirect("/Public/Message.aspx?msgTxt="+GestionWeb.GetWebWord(890, _webSession.SiteLanguage)+"&title="+GestionWeb.GetWebWord(887, _webSession.SiteLanguage));
					break;
			}
			#endregion

			#region Design Tableau d'images real ou wm suivant la disponibilité des fichiers
			if(realFormatFound || windowsFormatFound){
				//Texte explicatif
				if (read){
					Label1.Text=GestionWeb.GetWebWord(919,_webSession.SiteLanguage);
				}
				else{
					Label1.Text=GestionWeb.GetWebWord(920,_webSession.SiteLanguage);
				}
				//Ajout des images à cliquer
				if (realFormatFound && windowsFormatFound){
					//Les deux fichiers sont disponibles
					creationsLine+="<TR vAlign=\"middle\">";
					creationsLine+="<TD align=\"center\">";
					creationsLine+="<a href=\""+path2+"\" class=\"image\"><img src=\"/Images/Common/wmp.gif\" border=\"0\" height=\"93\" width=\"120\"></a>";
					creationsLine+="</TD>";
					creationsLine+="<TD align=\"center\">";
					creationsLine+="<a href=\""+path1+"\" class=\"image\"><img src=\"/Images/Common/real.jpg\" border=\"0\"></a>";
					creationsLine+="</TD>";
					creationsLine+="</TR>";
				}
				else{
					if (realFormatFound){
						//Seule le fichier real est disponible
						creationsLine+="<TR vAlign=\"middle\">";
						creationsLine+="<TD colSpan=\"2\" align=\"center\">";
						creationsLine+="<a href=\""+path1+"\" class=\"image\"><img src=\"/Images/Common/real.jpg\" border=\"0\"></a>";
						creationsLine+="</TD>";
						creationsLine+="</TR>";
					}
					else{
						//Seul le fichier wm est dispo
						creationsLine+="<TR vAlign=\"middle\">";
						creationsLine+="<TD colSpan=\"2\" align=\"center\">";
						creationsLine+="<a href=\""+path2+"\" class=\"image\"><img src=\"/Images/Common/wmp.gif\" border=\"0\"></a>";
						creationsLine+="</TD>";
						creationsLine+="</TR>";
					}
				}
			}
			else{
				//Aucun fichier n'est dispo
				RadioButtonList1.Visible=false;
				creationsLine+="<TR vAlign=\"middle\">";
				creationsLine+="<TD colSpan=\"2\" align=\"center\" class=\"txtViolet11Bold\">";
				creationsLine+=GestionWeb.GetWebWord(892,_webSession.SiteLanguage);
				creationsLine+="</TD>";
				creationsLine+="</TR>";
			}
			#endregion
			
		}
		#endregion

		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e) {
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
		private void InitializeComponent() {
          
		}
		#endregion

	}
}
