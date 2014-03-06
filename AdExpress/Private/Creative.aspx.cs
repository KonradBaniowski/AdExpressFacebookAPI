#region Information
// Auteur : G. Facon
// Cr�� le : 25/10/2005
// Modifi� le : 
//
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        #endregion

        #region Ev�nements

        #region Chargement de la Page
        /// <summary>
        /// Chargement de la Page
        /// </summary>
        /// <param name="sender">Objet Source</param>
        /// <param name="e">Param�tres</param>
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

                #region On r�cup�re les param�tres de TNS Creative Explorer
                string dateBegin = HttpContext.Current.Request.QueryString.Get("date_debut");
                string dateEnd = HttpContext.Current.Request.QueryString.Get("date_fin");
                string vehicleListIdString = HttpContext.Current.Request.QueryString.Get("idVehicle");
                string vehicleId = string.Empty;
                string productListIdString = HttpContext.Current.Request.QueryString.Get("listeProduct");
                string[] versionListId;
                string versionListIdString = HttpContext.Current.Request.QueryString.Get("listeVersion");
                _siteLanguage = int.Parse(HttpContext.Current.Request.QueryString.Get("langueSite"));

                #endregion

                #region On test les param�tres
                if (dateBegin.Length != 8 || dateEnd.Length != 8) throw (new AdExpressException.AdExpressCustomerException("Param�tres dates invalides"));
                if (vehicleListIdString.Length < 1) throw (new AdExpressException.AdExpressCustomerException("Param�tre media invalide"));
                if (productListIdString.Length < 1) throw (new AdExpressException.AdExpressCustomerException("Param�tre produit invalide"));
                //Verification de la page d'appel
                //Page.Request.UrlReferrer								 
                #endregion

                List<long> vehicleListId = vehicleListIdString.Split(',').Select(Int64.Parse).ToList();
                if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.POLAND))
                {
                    vehicleListId = SwicthToAdExpressVehicle(vehicleListId);
                }
                string[] productListId = productListIdString.Split(',');

                #region On cr�e la Session (authentification)

                //WebSession webSession=null;
                //Creer l'objet Right

                Right loginRight = new Right(LOGIN, PASSWORD, _siteLanguage);

                //V�rifier le droit d'acc�s au site
                if (loginRight.CanAccessToAdExpress())
                {
                    // Regarde � partir de quel tables charger les droits clients
                    // (template ou droits propres au login)
                    loginRight.SetModuleRights();
                    loginRight.SetFlagsRights();
                    loginRight.SetRights();
                    if (WebApplicationParameters.VehiclesFormatInformation.Use)
                        loginRight.SetBannersAssignement();

                    //Creer une session
                    webSession = new WebSession(loginRight);
                    // On Met � jour la session avec les param�tres
                    webSession.CurrentModule = WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA;
                    webSession.Insert = TNS.AdExpress.Constantes.Web.CustomerSessions.Insert.total;
                    webSession.Unit = UnitsInformation.DefaultCurrency;

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
                        if (!string.IsNullOrEmpty(versionListIdString))
                        {
                            versionListId = versionListIdString.Split(',');
                            webSession.IdSlogans = new ArrayList();
                            if (vehicleListId.Count > 1)
                                throw (new AdExpressException.AdExpressCustomerException("On ne peut pas avoir un plan media pluri media si on passe un num�ro de version en param�tre"));


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

                    #region Niveau de d�tail g�n�rique
                    var levels = new ArrayList { 1, 2, 3 };
                    webSession.GenericMediaDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                    #endregion

                    webSession.SiteLanguage = _siteLanguage;
                    //Sauvegarder la session
                    webSession.Save();
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

            // On affiche le r�sultat

        }
        #endregion

        #endregion

        #region Code g�n�r� par le Concepteur Web Form
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Param�tres</param>
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette m�thode avec l'�diteur de code.
        /// </summary>
        private void InitializeComponent()
        {
        }

        private List<long> SwicthToAdExpressVehicle(List<long> vehicles)
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
                    case 9: newVehicle.Add(6);
                        break;
                    default: newVehicle.Add(p); break;

                }
            });
            return newVehicle;
        }
        #endregion
    }
}
