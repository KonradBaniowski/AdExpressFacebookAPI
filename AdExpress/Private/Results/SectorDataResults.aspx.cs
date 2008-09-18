#region Informations
// Auteur: Y. R'kaina
// Date de création: 15/01/2007
// Date de modification: 
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
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using FrameWorkConstantes= TNS.AdExpress.Constantes.FrameWork.Results;
using ConstResults=TNS.AdExpress.Constantes.FrameWork.Results;
using WebBF=TNS.AdExpress.Web.BusinessFacade;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using Dundas.Charting.WebControl;
using TNS.AdExpress.Domain.Level;

namespace AdExpress.Private.Results
{
	/// <summary>
	/// Page d'affichage des résultats du données de cadrage.
	/// </summary>
	public partial class SectorDataResults : TNS.AdExpress.Web.UI.ResultWebPage{

		#region variables
		/// <summary>
		/// Code HTML des résultats
		/// </summary>
		public string result="";
		/// <summary>
		/// Date récupérer dans l'url
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
		///  booléen précisant si l'on doit afficher l'option filtre par annonceur, marque ou produit
		/// </summary>
		public bool displayDetailOption=false;
		#endregion
	
		#region variables MMI
		/// <summary>
		/// Contrôle Titre du module
		/// </summary>
		/// <summary>
		/// Contrôle Options des résultats
		/// </summary>
		/// <summary>
		/// Bouton de validation
		/// </summary>
		/// <summary>
		/// Contrôle de la barre d'en-tête
		/// </summary>
		/// <summary>
		/// Conteneur des composants destinés au données de cadrage.
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

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// Suivant l'indicateur choisi une méthode contenue dans UI est appelé
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

				if(_webSession.CurrentTab==1)
					displayDetailOption=true;

				if((_webSession.PreformatedProductDetail!=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser)&&(_webSession.PreformatedProductDetail!=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.brand)&&(_webSession.PreformatedProductDetail!=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product))
					_webSession.PreformatedProductDetail =WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser;

				// Bouton ok Option
				if(Request.Form.Get("__EVENTTARGET")=="okImageButton") {
					if( Page.Request.Form.GetValues("productDetail_DetailWebControl1")!=null &&
						Page.Request.Form.GetValues("productDetail_DetailWebControl1")[0].ToString()=="AdvertiserBrandProduct_"+TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser.GetHashCode().ToString()){
						_webSession.PreformatedProductDetail =WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser;																	
					}
					else if(Page.Request.Form.GetValues("productDetail_DetailWebControl1")!=null && Page.Request.Form.GetValues("productDetail_DetailWebControl1")[0].ToString()=="AdvertiserBrandProduct_"+TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.brand.GetHashCode().ToString()){
						_webSession.PreformatedProductDetail =WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.brand;
					}
					else if(Page.Request.Form.GetValues("productDetail_DetailWebControl1")!=null && Page.Request.Form.GetValues("productDetail_DetailWebControl1")[0].ToString()=="AdvertiserBrandProduct_"+TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product.GetHashCode().ToString()){
						_webSession.PreformatedProductDetail =WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product;
					}
				}
				
				#region Niveau de détail media (Generic)
				_webSession.PreformatedMediaDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory;
					
				// Initialisation à media\catégorie
				ArrayList levels=new ArrayList();
				levels.Add(1);
				levels.Add(2);
				_webSession.GenericMediaDetailLevel=new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
				#endregion
				
				_webSession.Save();
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
		/// <returns>collections triées de valeurs</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode(){			
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();			
			Moduletitlewebcontrol2.CustomerWebSession=_webSession;
			ResultsOptionsWebControl1.CustomerWebSession=_webSession;
			MenuWebControl2.CustomerWebSession = _webSession;
			DetailWebControl1.CustomerWebSession = _webSession;
			DetailWebControl1.ShowProduct = _webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
			#region chargement de l'univers
						
			//_webSession.CurrentUniversAdvertiser=(System.Windows.Forms.TreeNode)((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)_webSession.CompetitorUniversAdvertiser[1]).TreeCompetitorAdvertiser;			

			#endregion

			//Conteneur des composants de l'APPM
			SectorDataContainerWebControl1.CustomerWebSession = _webSession;
			SectorDataContainerWebControl1.ImageType = ChartImageType.Flash;
			_webSession.Save();
			return tmp;
		}
		#endregion

		#endregion

		#region Code généré par le Concepteur Web Form
		override protected void OnInit(EventArgs e){
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			AppmOptionsWebControl();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
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

		#region méthodes internes

		#region webcontrols to show and hide
		/// <summary>
		/// Contrôles fils à afficher dans le contrôle du choix des options 
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
					if(_webSession.Unit ==WebConstantes.CustomerSessions.Unit.kEuro){
						//unité en euro pour cette planche
						_webSession.Unit=WebConstantes.CustomerSessions.Unit.euro;
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
