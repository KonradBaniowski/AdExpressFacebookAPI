#region Information
// Auteur : G. Facon
// Créé le : 25/10/2005
// Modifié le : 
//
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Classification.DAL.MediaBrand;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using AdExpressException = TNS.AdExpress.Exceptions;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress;
using TNS.AdExpress.Domain.Web;

namespace AdExpress.Private
{
    /// <summary>
    /// Point d'entrer du site AdExpress Pour TNS Creative Explorer
    /// </summary>
    public partial class Creative : System.Web.UI.Page
    {

        #region Constantes
        /// <summary>
        /// Login de TNS Creative Explorer
        /// </summary>
        private const string LOGIN = "CREATIVE3";
        /// <summary>
        /// Password de TNS Creative Explorer
        /// </summary>
        private const string PASSWORD = "EXPLOV3";
        #endregion

        #region Variables
        /// <summary>
        /// Langue de la page
        /// </summary>
        private int _siteLanguage;

        private string _versionListIdString;
        #endregion

        #region Evènements

        #region Chargement de la Page
        /// <summary>
        /// Chargement de la Page
        /// </summary>
        /// <param name="sender">Objet Source</param>
        /// <param name="e">Paramètres</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {

            System.Windows.Forms.TreeNode tmpNode;

            NomenclatureElementsGroup nomenclatureElementsGroup = null;
            AdExpressUniverse adExpressUniverse = null;
            Dictionary<int, AdExpressUniverse> universeDictionary = null;
            List<long> products = null;
            WebSession webSession = null;

            try
            {

                #region On récupère les paramètres de TNS Creative Explorer
                string dateBegin = HttpContext.Current.Request.QueryString.Get("date_debut");
                string dateEnd = HttpContext.Current.Request.QueryString.Get("date_fin");
                string vehicleListIdString = HttpContext.Current.Request.QueryString.Get("idVehicle");
                string vehicleId = string.Empty;
                string productListIdString = HttpContext.Current.Request.QueryString.Get("listeProduct");
                _versionListIdString = HttpContext.Current.Request.QueryString.Get("listeVersion");
                _siteLanguage = int.Parse(HttpContext.Current.Request.QueryString.Get("langueSite"));

                #endregion

                #region On test les paramètres
                if (dateBegin.Length != 8 || dateEnd.Length != 8) throw (new AdExpressException.AdExpressCustomerException("Paramètres dates invalides"));
                if (vehicleListIdString.Length < 1) throw (new AdExpressException.AdExpressCustomerException("Paramètre media invalide"));
                if (productListIdString.Length < 1) throw (new AdExpressException.AdExpressCustomerException("Paramètre produit invalide"));
                //Verification de la page d'appel
                //Page.Request.UrlReferrer								 
                #endregion

                List<long> vehicleListId = vehicleListIdString.Split(',').Select(Int64.Parse).ToList();
                if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.POLAND))
                {
                    vehicleListId = SwicthToAdExpressVehicle(vehicleListId, webSession);
                }
                string[] productListId = productListIdString.Split(',');

                #region On crée la Session (authentification)

                //WebSession webSession=null;
                //Creer l'objet Right

                Right loginRight = new Right(LOGIN, PASSWORD, _siteLanguage);

                //Vérifier le droit d'accès au site
                if (loginRight.CanAccessToAdExpress())
                {
                    // Regarde à partir de quel tables charger les droits clients
                    // (template ou droits propres au login)
                    loginRight.SetModuleRights();
                    loginRight.SetFlagsRights();
                    loginRight.SetRights();
                    if (WebApplicationParameters.VehiclesFormatInformation.Use)
                        loginRight.SetBannersAssignement();

                    //Creer une session
                    webSession = new WebSession(loginRight);
                    // On Met à jour la session avec les paramètres
                    webSession.CurrentModule = WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA;
                    webSession.Insert = TNS.AdExpress.Constantes.Web.CustomerSessions.Insert.total;

                    if (vehicleListId.Count == 1 && VehiclesInformation.Get(vehicleListId[0]).Id == Vehicles.names.adnettrack)
                    {
                        webSession.SiteLanguage = _siteLanguage;
                        webSession.Unit = WebConstantes.CustomerSessions.Unit.occurence;
                        SetAdNetTrackProductSelection(webSession, Convert.ToInt64(_versionListIdString));
                        //webSession.DataLanguage = _siteLanguage;
                        TNS.AdExpress.Web.Functions.ProductDetailLevel.SetProductLevel(webSession, Convert.ToInt32(productListId[0]), DetailLevelItemInformation.Levels.product);
                    }
                    else webSession.Unit = UnitsInformation.DefaultCurrency;

                    webSession.LastReachedResultUrl = string.Empty;

                    #region Media
                    try
                    {
                        webSession.CurrentUniversMedia = new System.Windows.Forms.TreeNode("media");
                        var vehicleLabels = new VehicleLevelListDAL(_siteLanguage, loginRight.Source);

                        foreach (long currentVehicleId in vehicleListId)
                        {

                            tmpNode = new System.Windows.Forms.TreeNode(vehicleLabels[currentVehicleId])
                            {
                                Tag =
                                    new LevelInformation(
                                        TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess,
                                        currentVehicleId, vehicleLabels[currentVehicleId])
                            };
                            webSession.CurrentUniversMedia.Nodes.Add(tmpNode);
                        }
                        webSession.SelectionUniversMedia = (System.Windows.Forms.TreeNode)webSession.CurrentUniversMedia.Clone();
                    }
                    catch (System.Exception err)
                    {
                        throw (new AdExpressException.AdExpressCustomerException("Impossible de construire l'univers media : " + err.Message));
                    }
                    #endregion

                    #region Produit
                    try
                    {
                        nomenclatureElementsGroup = new NomenclatureElementsGroup(0, AccessType.includes);

                        products = productListId.Select(Int64.Parse).ToList();

                        nomenclatureElementsGroup.AddItems(TNSClassificationLevels.PRODUCT, products);
                        adExpressUniverse = new AdExpressUniverse(Dimension.product);
                        adExpressUniverse.AddGroup(0, nomenclatureElementsGroup);
                        universeDictionary = new Dictionary<int, AdExpressUniverse>();
                        universeDictionary.Add(0, adExpressUniverse);
                        webSession.PrincipalProductUniverses = universeDictionary;
                    }
                    catch (System.Exception err)
                    {
                        throw (new AdExpressException.AdExpressCustomerException("Impossible de construire l'univers produit : " + err.Message));
                    }
                    #endregion

                    #region Dates
                    webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.dayly;
                    webSession.PeriodType = WebConstantes.CustomerSessions.Period.Type.dateToDate;
                    webSession.PeriodBeginningDate = dateBegin;
                    webSession.PeriodEndDate = dateEnd;
                    #endregion

                    #region Versions
                    try
                    {
                        if (!string.IsNullOrEmpty(_versionListIdString))
                        {
                            string[] versionListId = _versionListIdString.Split(',');
                            webSession.IdSlogans = new ArrayList();
                            if (vehicleListId.Count > 1)
                                throw (new AdExpressException.AdExpressCustomerException("On ne peut pas avoir un plan media pluri media si on passe un numéro de version en paramètre"));


                            foreach (string idVersion in versionListId.Where(idVersion => !idVersion.Equals("0")))
                            {
                                if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.FRANCE))
                                {
                                    vehicleId = idVersion.Substring(0, vehicleListIdString.Length);
                                    if (!vehicleId.Equals(vehicleListIdString))
                                        throw (new AdExpressException.AdExpressCustomerException("La version " + idVersion + " n'appartient pas au media : " + vehicleListIdString));
                                    webSession.IdSlogans.Add(Int64.Parse(idVersion.Substring(vehicleListIdString.Length)));
                                }
                                else
                                {
                                    webSession.IdSlogans.Add(Int64.Parse(idVersion));
                                }

                            }
                        }
                        else
                        {
                            webSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.weekly;
                        }
                    }
                    catch (System.Exception err)
                    {
                        throw (new AdExpressException.AdExpressCustomerException("Impossible de construire la liste des versions : " + err.Message));
                    }
                    #endregion

                    #region Niveau de détail générique
                    var levels = new ArrayList { 1, 2, 3 };
                    webSession.GenericMediaDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                    webSession.GenericAdNetTrackDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                    #endregion

                    webSession.SiteLanguage = _siteLanguage;
                    //Sauvegarder la session
                    webSession.Save();
                    if (vehicleListId.Count == 1)
                        Response.Redirect("/Private/Results/CreativeMediaPlanResults.aspx?idSession=" + webSession.IdSession + "&idvehicle=" + vehicleListId[0]);
                    else
                        Response.Redirect("/Private/Results/CreativeMediaPlanResults.aspx?idSession=" + webSession.IdSession);
                    Response.Flush();

                }
                throw (new AdExpressException.AdExpressCustomerException("Connection impossible"));
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    WebFunctions.CreativeErrorTreatment.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, webSession), Page, _siteLanguage);
                }
            }
                #endregion

            // On affiche le résultat

        }
        #endregion

        #endregion

        #region Code généré par le Concepteur Web Form
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Paramètres</param>
        override protected void OnInit(EventArgs e)
        {
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

        private List<long> SwicthToAdExpressVehicle(List<long> vehicles, WebSession webSession)
        {
            var newVehicle = new List<long>();

            vehicles.ForEach(p =>
            {
                switch (p)
                {
                    case 1: newVehicle.Add(3);
                        break;
                    case 3: newVehicle.Add(1);
                        break;
                    case 8: newVehicle.Add(5);
                        break;
                    case 9: newVehicle.Add(6); break;
                    case 20: newVehicle.Add(9); break;
                    case 7:
                        newVehicle.Add(!string.IsNullOrEmpty(_versionListIdString) ? 8 : p);
                        break;
                    case 6:
                        newVehicle.Add(8); break;
                    default: newVehicle.Add(p); break;

                }
            });
            return newVehicle;
        }

        private void SetAdNetTrackProductSelection(WebSession webSession, long id)
        {

            webSession.AdNetTrackSelection =
                new AdNetTrackProductSelection(TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type.visual, id);

        }
        #endregion
    }
}
