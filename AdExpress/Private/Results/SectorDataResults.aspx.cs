#region Informations
// Auteur: Y. R'kaina
// Date de cr�ation: 15/01/2007
// Date de modification: 
#endregion

#region Namespaces
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Dundas.Charting.WebControl;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using ConstResults = TNS.AdExpress.Constantes.FrameWork.Results;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using DBFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using FrameWorkConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using WebBF = TNS.AdExpress.Web.BusinessFacade;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebExceptions = TNS.AdExpress.Web.Exceptions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Units;
#endregion

namespace AdExpress.Private.Results
{
	/// <summary>
	/// Page d'affichage des r�sultats du donn�es de cadrage.
	/// </summary>
    public partial class SectorDataResults : TNS.AdExpress.Web.UI.BaseResultWebPage {

		#region variables
		/// <summary>
		/// Code HTML des r�sultats
		/// </summary>
		public string result="";
		/// <summary>
		/// Date r�cup�rer dans l'url
		/// </summary>
		public string date="";				
		/// <summary> 
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();	
		/// <summary>
		/// Commentaire Agrandissement de l'image
		/// </summary>
		public string zoomTitle="";
		/// <summary>
		/// Affiche les graphiques
		/// </summary>
		public bool displayChart=false;
		/// <summary>
		///  bool�en pr�cisant si l'on doit afficher l'option filtre par annonceur, marque ou produit
		/// </summary>
		public bool displayDetailOption=false;
		#endregion
	
		#region variables MMI
		/// <summary>
		/// Contr�le Titre du module
		/// </summary>
		/// <summary>
		/// Contr�le Options des r�sultats
		/// </summary>
		/// <summary>
		/// Bouton de validation
		/// </summary>
		/// <summary>
		/// Contr�le de la barre d'en-t�te
		/// </summary>
		/// <summary>
		/// Conteneur des composants destin�s au donn�es de cadrage.
		/// </summary>
		/// <summary>
		/// Contextual Menu
		/// </summary>
		/// <summary>
		/// filtre par annonceur, marque ou produit
		/// </summary>
		/// <summary>
		/// 
		/// </summary>
		#endregion

		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// Suivant l'indicateur choisi une m�thode contenue dans UI est appel�
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region Url Suivante
				if(_nextUrl.Length!=0){
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
				}
				#endregion

				#region Textes et Langage du site
                this._dataSource = _webSession.Source;
				//TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				_siteLanguage=_webSession.SiteLanguage;
                InformationWebControl1.Language = _siteLanguage;
                HeaderWebControl1.Language = _siteLanguage;
				#endregion

				SectorDataContainerWebControl1.Source = this._dataSource;
                DetailPeriodWebControl1.WebSession = _webSession;

				#region Niveau de d�tail media (Generic)
				_webSession.PreformatedMediaDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory;
					
				// Initialisation � media\cat�gorie
				ArrayList levels=new ArrayList();
				levels.Add(1);
				levels.Add(2);
				_webSession.GenericMediaDetailLevel=new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
				#endregion
				
				//_webSession.Save();
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		
		#endregion

		#region PreRender
		/// <summary>
		/// to calculate the result for the page
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnPreRender(EventArgs e){
			try{

				#region MAJ _webSession
				_webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
				_webSession.ReachedModule=true;
				_webSession.Save();
				#endregion

			}	
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
			base.OnPreRender (e);
		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// Initialisation des composants
		/// </summary>
		/// <returns>collections tri�es de valeurs</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode(){			
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();			
			Moduletitlewebcontrol2.CustomerWebSession=_webSession;
			ResultsOptionsWebControl1.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;

			// Conteneur des composants de l'APPM
			SectorDataContainerWebControl1.CustomerWebSession = _webSession;
			SectorDataContainerWebControl1.ImageType = ChartImageType.Flash;
			_webSession.Save();
			return tmp;
		}
		#endregion

		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		override protected void OnInit(EventArgs e){
			//
			// CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			AppmOptionsWebControl();
			base.OnInit(e);
		}
		
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
		/// </summary>
		private void InitializeComponent(){
          
		}
		#endregion

		#region Abstract Methods
		/// <summary>
		/// Retrieve Next Url from Contextual Menu
		/// </summary>
		/// <returns>Next URL</returns>
		protected override string GetNextUrlFromMenu(){
			return MenuWebControl2.NextUrl;
		}

		#endregion

		#region m�thodes internes

		#region webcontrols to show and hide
		/// <summary>
		/// Contr�les fils � afficher dans le contr�le du choix des options 
		/// </summary>
		private void AppmOptionsWebControl(){

			switch (_webSession.CurrentTab){
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.sectorDataSeasonality:
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.sectorDataInterestFamily:
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.sectorDataPeriodicity:
					ResultsOptionsWebControl1.UnitOptionAppm=true;		
					_webSession.Graphics =true;
					break;
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.sectorDataAffinities:
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.sectorDataAverage:
				case TNS.AdExpress.Constantes.FrameWork.Results.APPM.sectorDataSynthesis :
                    if (_webSession.Unit == UnitsInformation.DefaultKCurrency) {
                        //unit� en euro pour cette planche
                        _webSession.Unit = UnitsInformation.DefaultCurrency;
                        _webSession.Save();
                    }
					_webSession.Graphics =false;
					break;
			}
		}
		#endregion

		#endregion

	}
}
