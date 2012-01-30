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
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpressI.Insertions.DAL;
using TNS.AdExpressI.Insertions;

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
            file="";
			
            try{
                VehicleInformation vehicleInformation = VehiclesInformation.Get(int.Parse(Page.Request.QueryString.Get("idVehicle")));
				//vehicle = (CstClassification.DB.Vehicles.names) int.Parse(Page.Request.QueryString.Get("idVehicle"));
                vehicle = vehicleInformation.Id;
				//_idSlogan = Page.Request.QueryString.Get("idSlogan");
				file = Page.Request.QueryString.Get("creation");
				if (
                (vehicle==CstClassification.DB.Vehicles.names.radio
                || vehicle == CstClassification.DB.Vehicles.names.radioGeneral
                || vehicle == CstClassification.DB.Vehicles.names.radioMusic
                || vehicle == CstClassification.DB.Vehicles.names.radioSponsorship
				) 
                    && file != null && file.Length > 0) {
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

            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.creativePopUp];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the creative pop up"));
            object[] param = new object[8];
            param[0] = this;
            param[1] = vehicle;
            param[2] = _idSlogan;
            param[3] = file;
            param[4] = _webSession;
            param[5] = title;
            param[6] = _hasCreationReadRights;
            param[7] = _hasCreationDownloadRights;
            TNS.AdExpressI.Insertions.CreativeResult.CreativePopUp result = (TNS.AdExpressI.Insertions.CreativeResult.CreativePopUp)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null, null);
            
            streamingCreationsResult = result.CreativePopUpRender();
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

	}
}
