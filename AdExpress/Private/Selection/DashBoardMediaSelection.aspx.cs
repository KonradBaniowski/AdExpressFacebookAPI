#region Informations
// Auteur: D. V. Mussuma
// Date de création: 03/03/2005
// Date de modification: 03/03/2005
//	01/08/2006 Modification FindNextUrl
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Windows.Forms;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Navigation;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using constEvent=TNS.AdExpress.Constantes.FrameWork.Selection;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;

namespace AdExpress.Private.Selection {
    /// <summary>
    /// Description résumée de DashBoardMediaSelection.
    /// </summary>
    public partial class DashBoardMediaSelection : TNS.AdExpress.Web.UI.SelectionWebPage {

        #region Varaibles MMI
        /// <summary>
        /// Contrôle : titre du module
        /// </summary>
        /// <summary>
        /// Contrôle En-tête
        /// </summary>
        /// <summary>
        /// Chargement de l'univers
        /// </summary>
        /// <summary>
        ///Sélection des média
        /// </summary>
        /// <summary>
        /// Sauvegarde d'un univers
        /// </summary>
        /// <summary>
        /// Bouton : valider
        /// </summary>
        /// <summary>
        /// Contrôle chargement d'un univers
        /// </summary>
        #endregion

        #region Variables
        /// <summary>
        /// Evènement lancé
        /// </summary>
        public int eventButton = -1;
        /// <summary>
        /// Liste des univers
        /// </summary>
        public string listUniverses = "";
        /// <summary>
        /// Script
        /// </summary>
        public string saveScript = "";
        /// <summary>
        /// Fermeture du flash
        /// </summary>
        public string divClose = "";
        /// <summary>
        /// Code javascript pour l'ouverture du div lors du chargement d'un univers
        /// </summary>
        public string openDiv = "";
        /// <summary>
        /// A1
        /// </summary>
        /// <summary>
        /// Menu contextuel
        /// </summary>
        /// <summary>
        /// Type de branche au niveau de la nomenclature
        /// </summary>
        protected TNS.AdExpress.Constantes.Classification.Branch.type branchType = TNS.AdExpress.Constantes.Classification.Branch.type.media;


        #endregion

        #region Constructeur(s)
        /// <summary>
        /// Constructeur
        /// </summary>
        public DashBoardMediaSelection()
            : base() {
        }
        #endregion

        #region Evènements

        #region Chargement
        /// <summary>
        /// Evènement de chargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        protected void Page_Load(object sender, System.EventArgs e) {
            try {

                #region Textes et langage du site
                //Modification de la langue pour les Textes AdExpress
                TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls, _webSession.SiteLanguage);
                ModuleTitleWebControl2.CustomerWebSession = _webSession;
                InformationWebControl1.Language = _webSession.SiteLanguage;
                #endregion

                #region Evènements Boutons
                // Bouton valider
                if (Request.Form.Get("__EVENTTARGET") == "validImageButtonRollOverWebControl" || Request.Form.Get("__EVENTTARGET") == "MenuWebControl2") {
                    eventButton = constEvent.eventSelection.VALID_EVENT;
                }
                // Bouton ok
                else if (Request.Form.Get("__EVENTTARGET") == "OkImageButtonRollOverWebControl") {
                    eventButton = constEvent.eventSelection.OK_EVENT;
                }
                //Bouton Charger
                else if (Request.Form.Get("__EVENTTARGET") == "loadImageButtonRollOverWebControl") {
                    eventButton = constEvent.eventSelection.LOAD_EVENT;
                }
                //Bouton Enregistrer
                else if (Request.Form.Get("__EVENTTARGET") == "saveImageButtonRollOverWebControl") {
                    eventButton = constEvent.eventSelection.SAVE_EVENT;
                }
                // Bouton OK du pop up qui enregistre l'univers
                else if (HttpContext.Current.Request.QueryString.Get("saveUnivers") != null) {
                    eventButton = constEvent.eventSelection.OK_POP_UP_EVENT;
                }
                #endregion

                #region Chargement univers
                // Bouton Charger 
                if (eventButton == constEvent.eventSelection.LOAD_EVENT) {
                    if (!loadUniversMedia()) {
                        eventButton = -1;
                        this.loadImageButtonRollOverWebControl_Click(null, null);
                    }
                }
                #endregion

                #region Définition du contrôle MediaSelectionWebControl1
                MediaSelectionWebControl1.EventButton = eventButton;
                #endregion

                #region boutons RollOver
                validImageButtonRollOverWebControl.ImageUrl = "/Images/" + _siteLanguage + "/button/valider_up.gif";
                validImageButtonRollOverWebControl.RollOverImageUrl = "/Images/" + _siteLanguage + "/button/valider_down.gif";

                saveImageButtonRollOverWebControl.ImageUrl = "/Images/" + _siteLanguage + "/button/enregistrer_univers_up.gif";
                saveImageButtonRollOverWebControl.RollOverImageUrl = "/Images/" + _siteLanguage + "/button/enregistrer_univers_down.gif";
                #endregion

                #region Script
                //Gestion de la sélection d'un radiobutton dans la liste des univers
                if (!Page.ClientScript.IsClientScriptBlockRegistered("InsertIdMySession4")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "InsertIdMySession4", TNS.AdExpress.Web.Functions.Script.InsertIdMySession4());
                }
                // Ouverture/fermeture des fenêtres pères
                if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "showHideContent", TNS.AdExpress.Web.Functions.Script.ShowHideContent());
                }

                // Sélection/désélection de tous les fils (cas 2 niveau)
                if (!Page.ClientScript.IsClientScriptBlockRegistered("AllSelection")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AllSelection", TNS.AdExpress.Web.Functions.Script.AllSelection());
                }
                if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowHideContent1", TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
                }
                // Sélection de tous les fils
                if (!Page.ClientScript.IsClientScriptBlockRegistered("Integration")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Integration", TNS.AdExpress.Web.Functions.Script.Integration());
                }
                // Sélection de tous les éléments (cas 1 niveau)
                if (!Page.ClientScript.IsClientScriptBlockRegistered("AllLevelSelection")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AllLevelSelection", TNS.AdExpress.Web.Functions.Script.AllLevelSelection());
                }
                #endregion

            }
            catch (System.Exception exc) {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }
        #endregion

        #region PreRender
        /// <summary>
        /// PreRender
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e) {
            try {
                if (!MediaSelectionWebControl1.hasRecords) {
                    validImageButtonRollOverWebControl.Visible = false;
                    saveImageButtonRollOverWebControl.Visible = false;
                }
            }
            catch (System.Exception exc) {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }

        #endregion

        #region DeterminePostBackMode
        /// <summary>
        /// Initialisation de certains composants
        /// </summary>
        /// <returns>?</returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            LoadableUniversWebControl1.CustomerWebSession = _webSession;
            MediaSelectionWebControl1.CustomerWebSession = _webSession;
            branchType = GetBranchType(_webSession);
            LoadableUniversWebControl1.ListBranchType = branchType.GetHashCode().ToString();
            LoadableUniversWebControl1.SelectionPage = false;
            MenuWebControl2.CustomerWebSession = _webSession;
            return tmp;
        }
        #endregion

        #endregion

        #region Code généré par le Concepteur Web Form
        /// <summary>
        /// Intialisation des composants
        /// </summary>
        /// <param name="e">arguments</param>
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

        #region Valider la sélection
        /// <summary>
        /// Valider la sélection
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">argument(s)</param>
        protected void validImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {

            #region Variables locales
            System.Windows.Forms.TreeNode current = new System.Windows.Forms.TreeNode("ChoixMedia");
            System.Windows.Forms.TreeNode vehicle = null;
            string idVehicle = "";
            string OldIdVehicle = "";
            bool firstIdVeh = true;
            bool vhAdded = false;
            System.Windows.Forms.TreeNode InterestCenter = null;
            System.Windows.Forms.TreeNode media = null;
            string idInterestCenter = "";
            string OldIdInterestCenter = "";
            bool firstIdICenter = true;
            int vhCpt = 0;
            int icCpt = 0;
            int mdCpt = 0;
            #endregion

            #region Construction de l'arbre des médias
            foreach (ListItem item in this.MediaSelectionWebControl1.Items) {
                //Traitement Noeud media(vehicle)
                if (item.Value.IndexOf("vh_") > -1) {
                    idVehicle = item.Value.ToString();
                    //ajout du centre d'intérêts précédent 
                    if (!firstIdVeh && ((vehicle.Nodes.Count > 0 || vehicle.Checked) || (!OldIdVehicle.Equals(idVehicle.Trim()) && (InterestCenter.Nodes.Count > 0 || InterestCenter.Checked)))
                        && !idVehicle.Equals("") && !OldIdVehicle.Equals(idVehicle.Trim())) {
                        //Rajout du dernier centre d'intérêt dans media(vehicle) puis du dernier media dans l'arbre													
                        if (!vehicle.Checked && InterestCenter != null && (InterestCenter.Nodes.Count > 0 || InterestCenter.Checked) && !vehicle.Nodes.Contains(InterestCenter))
                            vehicle.Nodes.Add(InterestCenter);
                        current.Nodes.Add(vehicle);
                        vhAdded = true;
                        InterestCenter = new System.Windows.Forms.TreeNode(item.Text);
                        break;
                    }
                    vehicle = new System.Windows.Forms.TreeNode(item.Text);
                    //Creation d'un nouveau noeud média (vehicle)																	
                    if (item.Selected) vehicle.Tag = new LevelInformation(CstWebCustomer.Right.type.vehicleAccess, Int64.Parse(item.Value.Remove(0, 3)), item.Text);
                    else vehicle.Tag = new LevelInformation(CstWebCustomer.Right.type.vehicleException, Int64.Parse(item.Value.Remove(0, 3)), item.Text);
                    vehicle.Checked = item.Selected;
                    if (vehicle.Checked) vhCpt++;
                    idVehicle = item.Value.ToString();
                    OldIdVehicle = item.Value.ToString();
                    firstIdVeh = false;
                }
                //Traitement Noeud centre d'intérêts
                else if (!vehicle.Checked && item.Value.IndexOf("ic_") > -1) {
                    idInterestCenter = item.Value.ToString();
                    //ajout du centre d'intérêts précédent
                    if (!firstIdICenter && (InterestCenter.Nodes.Count > 0 || InterestCenter.Checked) && !idInterestCenter.Equals("") && !OldIdInterestCenter.Equals(idInterestCenter.Trim())) {
                        vehicle.Nodes.Add(InterestCenter);
                    }
                    //Creation d'un nouveau noeud centre d'intérêts
                    InterestCenter = new System.Windows.Forms.TreeNode(item.Text);
                    if (item.Selected) InterestCenter.Tag = new LevelInformation(CstWebCustomer.Right.type.interestCenterAccess, Int64.Parse(item.Value.Remove(0, 3)), item.Text);
                    else InterestCenter.Tag = new LevelInformation(CstWebCustomer.Right.type.interestCenterException, Int64.Parse(item.Value.Remove(0, 3)), item.Text);
                    InterestCenter.Checked = item.Selected;
                    if (InterestCenter.Checked) icCpt++;
                    OldIdInterestCenter = item.Value.ToString();
                    firstIdICenter = false;
                }
                //Traitement Noeud support(media)
                else if (!vehicle.Checked && !InterestCenter.Checked && item.Value.IndexOf("md_") > -1 && item.Selected) {
                    media = new System.Windows.Forms.TreeNode(item.Text);
                    media.Tag = new LevelInformation(CstWebCustomer.Right.type.mediaAccess, Int64.Parse(item.Value.Remove(0, 3)), item.Text);
                    media.Checked = item.Selected;
                    if (media.Checked) mdCpt++;
                    InterestCenter.Nodes.Add(media);
                }
            }
            //ajout de la dernière node vehicle si nécessaire
            //Rajout du dernier centre d'intérêts dans media(vehicle) puis du dernier media dans l'arbre	
            if (!vehicle.Checked && InterestCenter != null && vehicle != null && (InterestCenter.Checked || InterestCenter.Nodes.Count > 0) && !vehicle.Nodes.Contains(InterestCenter)) {
                vehicle.Nodes.Add(InterestCenter);
            }
            if (current.Nodes.Count == 0 && !vhAdded && (vehicle.Nodes.Count > 0 || vehicle.Checked))
                current.Nodes.Add(vehicle);

            #endregion

            #region MAJ de la session
            if ((mdCpt + icCpt + vhCpt) > 0) {
                if (mdCpt < WebConstantes.Advertiser.MAX_NUMBER_ADVERTISER
                    && vhCpt < WebConstantes.Advertiser.MAX_NUMBER_ADVERTISER
                    && icCpt < WebConstantes.Advertiser.MAX_NUMBER_ADVERTISER) {
                    if (eventButton == constEvent.eventSelection.SAVE_EVENT)
                        _webSession.CurrentUniversMedia = current;
                    else _webSession.SelectionUniversMedia = current;
                    if (eventButton == constEvent.eventSelection.VALID_EVENT)
                        _webSession.CurrentUniversMedia = new System.Windows.Forms.TreeNode("media");
                    if (_webSession.SelectionUniversMedia != null && _webSession.SelectionUniversMedia.FirstNode != null && ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID != DBClassificationConstantes.Vehicles.names.plurimedia.GetHashCode())
                        _webSession.PreformatedMediaDetail = WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter;
                    else
                        _webSession.PreformatedMediaDetail = WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle;
                    _webSession.Save();
                    //Redirection Page suivante
                    if (eventButton == constEvent.eventSelection.VALID_EVENT) {
                        DBFunctions.closeDataBase(_webSession);
                        Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession + "");
                    }

                }
                else {
                    foreach (ListItem item in this.MediaSelectionWebControl1.Items) {
                        if (item.Selected) item.Selected = false;
                    }
                    Response.Write("<script language=javascript>");
                    Response.Write(" alert(\"" + GestionWeb.GetWebWord(2265, _webSession.SiteLanguage) + "\");");
                    Response.Write("</script>");
                }
            }
            else {
                Response.Write("<script language=javascript>");
                Response.Write(" alert(\"" + GestionWeb.GetWebWord(1199, _webSession.SiteLanguage) + "\");");
                Response.Write("</script>");
            }
            #endregion

        }
        #endregion

        #region Enregistrer la sélection
        /// <summary>
        /// Enregistrer la sélection
        /// </summary>
        /// <param name="sender">objet qui lance l'évènement</param>
        /// <param name="e">argument(s)</param>
        protected void saveImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
            TNS.AdExpress.Constantes.Classification.Branch.type branchType = GetBranchType(_webSession);
            try {
                if (MediaSelectionWebControl1.NbElement != constEvent.error.CHECKBOX_NULL) {
                    Int64 idUniverseClientDescription = WebConstantes.LoadableUnivers.VEHICLE_INTEREST_CENTER_MEDIA;
                    this.MediaSelectionWebControl1.LoadSession = true;
                    validImageButtonRollOverWebControl_Click(null, null);
                    saveScript = WebFunctions.Script.SaveUniverseOpen(_webSession.IdSession, branchType, idUniverseClientDescription);
                }
                else {
                    // Erreur : Il faut sélectionner au moins un média
                    Response.Write("<script language=javascript>");
                    Response.Write("	alert(\"" + GestionWeb.GetWebWord(1052, _webSession.SiteLanguage) + "\");");
                    Response.Write("history.go(-1);");
                    Response.Write("</script>");
                }
            }
            catch (System.Exception exc) {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }
        #endregion

        #region Méthodes internes

        #region  charger univers
        /// <summary>
        /// Bouton chargement
        /// </summary>
        protected bool loadUniversMedia() {

            string[] tabParent = null;
            Int64 idUniverse = 0;
            bool selectionnedUnivers = false;
            foreach (string currentKey in Request.Form.AllKeys) {
                tabParent = currentKey.Split('_');
                if (tabParent[0] == "UNIVERSE") {
                    idUniverse = Int64.Parse(tabParent[1]);
                }
            }
            if (idUniverse != 0) {
                System.Windows.Forms.TreeNode treeNodeUniverse = (System.Windows.Forms.TreeNode)((ArrayList)TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetTreeNodeUniverse(idUniverse, _webSession))[0];
                _webSession.CurrentUniversMedia = treeNodeUniverse;
                _webSession.Save();
                selectionnedUnivers = true;
            }

            return selectionnedUnivers;
        }
        #endregion

        #region Charger
        /// <summary>
        /// Chargement de l'univers
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        private void loadImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
            if (eventButton != 4) {
                Response.Write("<script language=javascript>");
                Response.Write("	alert(\"" + GestionWeb.GetWebWord(926, _webSession.SiteLanguage) + "\");");
                Response.Write("history.go(-1);");
                Response.Write("</script>");
            }
        }
        #endregion

        #region Branche media
        /// <summary>
        /// Donne le type de la branche media 
        /// </summary>
        /// <param name="_webSession">session du client</param>
        /// <returns>branche média</returns>
        private TNS.AdExpress.Constantes.Classification.Branch.type GetBranchType(WebSession _webSession) {
            switch (_webSession.CurrentModule) {
                case WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE:
                    return TNS.AdExpress.Constantes.Classification.Branch.type.mediaPress;
                case WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO:
                    return TNS.AdExpress.Constantes.Classification.Branch.type.mediaRadio;
                case WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION:
                    return TNS.AdExpress.Constantes.Classification.Branch.type.mediaTv;
                case WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO:
                    return TNS.AdExpress.Constantes.Classification.Branch.type.mediaOthers;
                default:
                    throw (new WebExceptions.DashBoardMediaSelectionException(" BranchType(WebSession _webSession) :Impossible d'identifier lea branche média."));
            }
        }

        #endregion

        #endregion

        #region Implémentation méthodes abstraites
        /// <summary>
        /// Event launch to fire validation of the page
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Arguments</param>
        protected override void ValidateSelection(object sender, System.EventArgs e) {
			eventButton = constEvent.eventSelection.VALID_EVENT;
			validImageButtonRollOverWebControl_Click(sender, e);
        }
        /// <summary>
        /// Retrieve next Url from the menu
        /// </summary>
        /// <returns>Next Url</returns>
        protected override string GetNextUrlFromMenu() {
            return (this.MenuWebControl2.NextUrl);
        }
        #endregion

    }
}
