#region Informations
// Auteur: G. Facon
// Date de création: 
// Date de modification: 28/12/2004
//		28/12/2004 G. Facon Intégration erreur
//		17/08/2005 D. V. Mussuma :  Initialisation des variables APPM
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
using DBConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.Core.Sessions;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using WebFunctions=TNS.AdExpress.Web.Functions;
using DBFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;

namespace AdExpress{
	/// <summary>
	/// Sélection d'un module
	/// </summary>
	public partial class selectionModule : TNS.AdExpress.Web.UI.PrivateWebPage{
	
		#region Variables
		/// <summary>
		/// test
		/// </summary>
		public string resultPM;
		#endregion

		#region Variables MMI
		/// <summary>
		/// Contrôle En tête de page
		/// </summary>
		/// <summary>
		/// Contrôle liste des Modules
		/// </summary>
		/// <summary>
		/// Titre de la page
		/// </summary>
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public selectionModule():base(){
		}
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			//test
			_webSession.ExportedPDFFileName="test01";
			//Int64 idStaticNavSession=TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession,TNS.AdExpress.Anubis.Constantes.Result.type.appm);
			//TNS.AdExpress.Anubis.Constantes.Network.Server.code code=TNS.AdExpress.Anubis.BusinessFacade.Network.ClientSystem.Send(TNS.AdExpress.Anubis.Common.Network.WebClientConfiguration.IP,TNS.AdExpress.Anubis.Common.Network.WebClientConfiguration.Port,idStaticNavSession,TNS.AdExpress.Anubis.Constantes.Result.type.appm);

			try{
				if (Page.Request.QueryString.Get("m")!=null){
					Int64 tmp = _webSession.CurrentModule = Int64.Parse(Page.Request.QueryString.Get("m").ToString());
					// Forcer le tracking à enregistrer l'option
					if(_webSession.CurrentModule == CstWeb.Module.Name.INDICATEUR)
						_webSession.CurrentTab = TNS.AdExpress.Constantes.FrameWork.Results.SynthesisRecap.SYNTHESIS;//Règle : planche synthèse par défaut dans le module indicateurs
					else{
						if(_webSession.CurrentTab==0)_webSession.OnSetResult();
						else
							_webSession.CurrentTab = 0;
					}

					_webSession.ModuleTraductionCode  = (int)ModulesList.GetModuleWebTxt(tmp);
				
					#region Initialisation des données
					_webSession.ReachedModule  = false;
					_webSession.CurrentUniversMedia=new System.Windows.Forms.TreeNode("media");
					_webSession.CurrentUniversAdvertiser=new System.Windows.Forms.TreeNode("advertiser");
					_webSession.CurrentUniversProduct= new System.Windows.Forms.TreeNode("produit");					

					_webSession.SelectionUniversAdvertiser=new System.Windows.Forms.TreeNode("advertiser");
					_webSession.SelectionUniversMedia=new System.Windows.Forms.TreeNode("media");
					_webSession.SelectionUniversProduct=new System.Windows.Forms.TreeNode("produit");

					_webSession.ReferenceUniversAdvertiser=new System.Windows.Forms.TreeNode("advertiser");
					_webSession.ReferenceUniversMedia=new System.Windows.Forms.TreeNode("media");
					_webSession.ReferenceUniversProduct=new System.Windows.Forms.TreeNode("produit"); 
					_webSession.SelectionUniversProgramType=new System.Windows.Forms.TreeNode("programtype");
					_webSession.SelectionUniversSponsorshipForm=new System.Windows.Forms.TreeNode("sponsorshipform"); 
					_webSession.CurrentUniversProgramType=new System.Windows.Forms.TreeNode("programtype");
					_webSession.CurrentUniversSponsorshipForm=new System.Windows.Forms.TreeNode("sponsorshipform"); 

					_webSession.CompetitorUniversAdvertiser=new Hashtable(5);
					//_webSession.CompetitorUniversAdvertiser.Add(0, new System.Windows.Forms.TreeNode("advertiser"));
					_webSession.CompetitorUniversMedia=new Hashtable(5);
					_webSession.CompetitorUniversProduct=new Hashtable(5);

					_webSession.TemporaryTreenode=null;
					_webSession.PeriodLength=0;
					_webSession.PeriodBeginningDate="";
					_webSession.Insert=TNS.AdExpress.Constantes.Web.CustomerSessions.Insert.total;
					_webSession.PeriodEndDate="";
					_webSession.Graphics=true;

                    _webSession.Unit = UnitsInformation.DefaultCurrency;

                    _webSession.PreformatedProductDetail=WebFunctions.ProductDetailLevel.GetDefault(_webSession);

					_webSession.LastReachedResultUrl="";
					_webSession.Percentage=false;
					_webSession.ProductDetailLevel=null;
					// Niveau de détail support par défaut pour les plan media
//					if(_webSession.CurrentModule ==TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PLAN_MEDIA
//						|| _webSession.CurrentModule ==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
					if(_webSession.CurrentModule !=TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE &&_webSession.CurrentModule !=TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR){
						
						if(_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS)
							_webSession.PreformatedMediaDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.Media;
                        else if(_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES)
                            _webSession.PreformatedMediaDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle;
						else _webSession.PreformatedMediaDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory;
						#region Niveau de détail media (Generic)
						ArrayList levels=new ArrayList();
                        switch(_webSession.CurrentModule){
                            case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS:
                                // Support
                                levels.Add(3);
                                break;
                            case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES:
                                levels.Add(8);
                                break;
                            case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
                            case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
                                // Media/catégorie/Support
                                levels.Add(1);
							    levels.Add(2);
                                levels.Add(3);
                                break;
                            case TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES:
                                // Famille
                                levels.Add(11);
                                break;
                            default:
                                // Media/catégorie
                                levels.Add(1);
							    levels.Add(2);

								
                                break;
                        }			
						_webSession.GenericMediaDetailLevel=new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
						_webSession.GenericAdNetTrackDetailLevel=new GenericDetailLevel(levels,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
						#endregion

						#region Niveau de détail produit (Generic)
                        CstWeb.GenericDetailLevel.SelectedFrom selectedFrom;
						// Initialisation à annonceur
						levels.Clear();
                        switch(_webSession.CurrentModule) {
                            case TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES:
                                // Famille
                                levels.Add(11);
                                selectedFrom = TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels;
                                break;
                            case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES:
                                levels.Add(15);
                                levels.Add(16);
                                levels.Add(8);
                                selectedFrom = TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.customLevels;
                                break;
                            default:
                                levels.Add(8);
                                selectedFrom = TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels;
                                break;
                        }
						_webSession.GenericProductDetailLevel=new GenericDetailLevel(levels,selectedFrom);
						#endregion

                        #region Niveau de détail colonne (Generic)
                        levels.Clear();
                        levels.Add(3);
                        _webSession.GenericColumnDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                        #endregion
                    }
					_webSession.MediaDetailLevel=null;
					if(_webSession.CurrentModule ==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_PRESSE
						|| _webSession.CurrentModule ==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_RADIO
						|| _webSession.CurrentModule ==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_TELEVISION
						|| _webSession.CurrentModule ==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_PAN_EURO
                        || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_EVALIANT
					)
						_webSession.PreformatedTable =CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units;
					
					else if(_webSession.CurrentModule ==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES
						|| _webSession.CurrentModule ==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS						
					)
						_webSession.PreformatedTable =CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period;
					else 
						_webSession.PreformatedTable =CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.media_X_Year;
                    
                    if(_webSession.CurrentModule ==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS) {
                        _webSession.SelectionUniversMedia.Nodes.Clear();
                        System.Windows.Forms.TreeNode tmpNode=new System.Windows.Forms.TreeNode("TELEVISION");
						tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, VehiclesInformation.EnumToDatabaseId(DBConstantes.Vehicles.names.tv), "TELEVISION");
                        _webSession.SelectionUniversMedia.Nodes.Add(tmpNode);						
                    }
					#endregion

					#region paramètres rajoutés pour Bilan de campagne
					//Setting vehicle as press for APPM
					if(_webSession.CurrentModule ==TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE){
						System.Windows.Forms.TreeNode tmpNode=new System.Windows.Forms.TreeNode();
						tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess,DBConstantes.Vehicles.names.press.GetHashCode(), GestionWeb.GetWebWord(1298, _webSession.SiteLanguage));
						tmpNode.Checked=true;
						_webSession.SelectionUniversMedia.Nodes.Add(tmpNode);
					}
					#endregion

					#region paramètres rajoutés pour tableaux de bord
					_webSession.DetailPeriodBeginningDate="";
					_webSession.DetailPeriodEndDate="";
					_webSession.Format = CstWeb.Repartition.Format.Total;
					_webSession.TimeInterval = CstWeb.Repartition.timeInterval.Total;
					_webSession.NamedDay = CstWeb.Repartition.namedDay.Total;
					#endregion
					
					//Rajouté le 17/08/05 par D.V. Mussuma
					_webSession.CurrentUniversAEPMWave = new System.Windows.Forms.TreeNode("wave"); 
					_webSession.CurrentUniversAEPMTarget = new System.Windows.Forms.TreeNode("target");
					_webSession.CurrentUniversOJDWave = new System.Windows.Forms.TreeNode("ojdWave");
					_webSession.SelectionUniversAEPMWave = new System.Windows.Forms.TreeNode("wave"); 
					_webSession.SelectionUniversAEPMTarget =  new System.Windows.Forms.TreeNode("target");
					_webSession.SelectionUniversOJDWave = new System.Windows.Forms.TreeNode("ojdWave");
				
					#region Paramètres pour les accroches - G Ragneau - 23/12/2005
					_webSession.IdSlogans = new ArrayList();
					_webSession.SloganColors = new Hashtable();
					_webSession.SloganIdZoom=-1;
					#endregion

					//Rajouté le 14/12/05 par D.V. Mussuma
					_webSession.PercentageAlignment = CstWeb.Percentage.Alignment.none;

                    //Initialisaiton des tris des tableaux génériques (G.Ragneau - 12/10/2007)
                    _webSession.Sort = 0;
                    _webSession.SortKey = string.Empty;

                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES ||
                        _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {
                        _webSession.ComparativeStudy = false;
                        _webSession.Evolution = false;
                    }

					//Défintion des medias et  périodes par défaut pour les modules d'Analyses Sectorielles
					if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
						|| _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR) {
						
						SetRecapDefaultMediaSelection();
						SetRecapDefaultPeriodSelection();
					}
					//Nouveaux univers produit
					_webSession.PrincipalProductUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
					_webSession.SecondaryProductUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
                    //Nouveaux univers media
                    _webSession.PrincipalMediaUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
                    _webSession.SecondaryMediaUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
                    //New advertising agency univers
                    _webSession.PrincipalAdvertisingAgnecyUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
                    _webSession.SecondaryAdvertisingAgnecyUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();

                    //Initialisation de customerPeriod
                    try {
                        if (_webSession.CustomerPeriodSelected != null)
                            _webSession.CustomerPeriodSelected = null;
                    }
                    catch (System.Exception) { }

					_webSession.Save();
					Response.Redirect(ModulesList.GetModuleNextUrl(tmp)+"?idSession="+_webSession.IdSession);
				}
				else{
					//test page résultat plan média
					resultPM=_webSession.IdSession;

					//Modification de la langue pour les Textes AdExpress

					// Initialisation de la liste des modules
					HeaderWebControl1.Language = _webSession.SiteLanguage;
					HeaderWebControl1.ActiveMenu = CstWeb.MenuTraductions.MODULES;
					ModuleSelection2WebControl1.CustomerSession = _webSession;
					LoginInformationWebControl1.CustomerSession = _webSession;
					ActualitiesWebControl1.LanguageId = _webSession.SiteLanguage;
				}

                MenuWebControl1.CustomerWebSession = _webSession;
			}
			catch(System.Exception exc) {
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation des controls de la page (ViewState et valeurs modifiées pas encore chargés)
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e){
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
		private void InitializeComponent(){    
		}
		#endregion

		#region Méthodes internes
		/// <summary>
		/// Set default media selection
		/// <remarks>Plurimedia will be the default choice</remarks>
		/// </summary>
		private void SetRecapDefaultMediaSelection() {

			if (!_webSession.isMediaSelected()) {
				System.Windows.Forms.TreeNode current = new System.Windows.Forms.TreeNode("ChoixMedia");
				System.Windows.Forms.TreeNode vehicle = null;

				vehicle = new System.Windows.Forms.TreeNode(GestionWeb.GetWebWord(210, _webSession.SiteLanguage));

				//Creating new plurimedia	node																
				vehicle.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, DBConstantes.Vehicles.names.plurimedia.GetHashCode(), GestionWeb.GetWebWord(210, _webSession.SiteLanguage));
				vehicle.Checked = true;
				current.Nodes.Add(vehicle);

				//Tracking
				_webSession.OnSetVehicle(DBConstantes.Vehicles.names.plurimedia.GetHashCode());

				// Extraction Last Available Recap Month
				_webSession.LastAvailableRecapMonth = DBFunctions.CheckAvailableDateForMedia(DBConstantes.Vehicles.names.plurimedia.GetHashCode(), _webSession);

				_webSession.SelectionUniversMedia = _webSession.CurrentUniversMedia = current;
				_webSession.PreformatedMediaDetail = CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle;
			}
		}

		/// <summary>
		/// Set default period selection
		/// <remarks>Current or last years will be the default period</remarks>
		/// </summary>
		private void SetRecapDefaultPeriodSelection() {

			DateTime downloadDate = new DateTime(_webSession.DownLoadDate, 12, 31);
			string absolutEndPeriod = "";

		
				try {

					//Choix par défaut année courante
					_webSession.PeriodType = CstWeb.CustomerSessions.Period.Type.currentYear;
					_webSession.PeriodLength = 1;
					// Cas où l'année de chargement est inférieur à l'année en cours
					if (DateTime.Now.Year > _webSession.DownLoadDate) {
						_webSession.PeriodBeginningDate = downloadDate.ToString("yyyy01");
						_webSession.PeriodEndDate = downloadDate.ToString("yyyyMM");
					}
					else {
						_webSession.PeriodBeginningDate = DateTime.Now.ToString("yyyy01");
						_webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
					}

					//Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
					//du dernier mois dispo en BDD
					//traitement de la notion de fréquence
					absolutEndPeriod = Dates.CheckPeriodValidity(_webSession, _webSession.PeriodEndDate);

					if ((int.Parse(absolutEndPeriod) < int.Parse(_webSession.PeriodBeginningDate)) || (absolutEndPeriod.Substring(4, 2).Equals("00"))) {
						throw (new TNS.AdExpress.Domain.Exceptions.NoDataException());
					}

					_webSession.PeriodEndDate = absolutEndPeriod;
					_webSession.DetailPeriod = CstPeriodDetail.monthly;

					//Activation de l'option etude comparative 
					_webSession.ComparativeStudy = true;

				}
				catch (TNS.AdExpress.Domain.Exceptions.NoDataException) {

					//Sinon choix par défaut année précédente
					_webSession.PeriodType = CstWeb.CustomerSessions.Period.Type.previousYear;
					_webSession.PeriodLength = 1;

					// Cas où l'année de chargement est inférieur à l'année en cours
					if (DateTime.Now.Year > _webSession.DownLoadDate) {
						_webSession.PeriodBeginningDate = downloadDate.AddYears(-1).ToString("yyyy01");
						_webSession.PeriodEndDate = downloadDate.AddYears(-1).ToString("yyyy12");
					}
					else {
						_webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
						_webSession.PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy12");
					}
					_webSession.DetailPeriod = CstPeriodDetail.monthly;
					_webSession.ComparativeStudy = true;
				}
				catch (System.Exception exc) {
					if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
						this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
					}
				}
			

			
		}
		#endregion

	}
}
