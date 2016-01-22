#region Informations
// Auteur: D.V Mussuma 
// Date de création: 14/09/2004 
// Date de modification: 
//	30/12/2004  D.V Mussuma Intégration de WebPage
//	01/08/2006 Modification FindNextUrl
#endregion

#region namespace
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
using TNS.AdExpress.Domain.Web.Navigation;

using DBFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantesClassification = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpressI.Date.DAL;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Layers;
using System.Reflection;
using TNS.AdExpress.Domain.Classification;
#endregion

namespace AdExpress.Private.Selection.Russia
{
    /// <summary>
    /// Page de sélection des média pour l'analyse séctorielle en Russie
    /// </summary>
    public partial class SectorAnalysisVehicleSelection : TNS.AdExpress.Web.UI.SelectionWebPage
    {

        #region Variables MMI
        /// <summary>
        /// Control de sélection des Média
        /// </summary>
        /// <summary>
        /// Entête de la page
        /// </summary>
        /// <summary>
        /// Titre de la page
        /// </summary>
        /// <summary>
        /// Texte "Sélectionner tous les médias"
        /// </summary>
        protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText2;
        /// <summary>
        /// Texte titre
        /// </summary>
        /// <summary>
        /// Menu contextuel
        /// </summary>
        /// <summary>
        /// Bouton de validation 
        /// </summary>
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur
        /// </summary>
        public SectorAnalysisVehicleSelection()
            : base()
        {
        }
        #endregion

        #region Evènements

        #region Chargement de la page
        /// <summary>
        /// Evènement de chargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                #region Textes et langage du site
                //Modification de la langue pour les Textes AdExpress         
                ModuleTitleWebControl1.CustomerWebSession = _webSession;
                InformationWebControl1.Language = _webSession.SiteLanguage;

                #endregion

                #region Script
                // Cochage/Decochage des checkbox pères, fils et concurrents
                if (!Page.ClientScript.IsClientScriptBlockRegistered("CheckAllChilds"))
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CheckAllChilds", TNS.AdExpress.Web.Functions.Script.CheckAllChilds());
                }
                // Ouverture/fermeture des fenêtres pères
                if (!Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer"))
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DivDisplayer", TNS.AdExpress.Web.Functions.Script.DivDisplayer());
                }
                // fermer/ouvrir tous les calques
                if (!Page.ClientScript.IsClientScriptBlockRegistered("ExpandColapseAllDivs"))
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ExpandColapseAllDivs", TNS.AdExpress.Web.Functions.Script.ExpandColapseAllDivs());
                }
                // Sélection de tous les fils
                if (!Page.ClientScript.IsClientScriptBlockRegistered("SelectAllChilds"))
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SelectAllChilds", TNS.AdExpress.Web.Functions.Script.SelectAllChilds());
                }
                // Sélection de tous les fils
                if (!Page.ClientScript.IsClientScriptBlockRegistered("SelectExclusiveAllChilds"))
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SelectExclusiveAllChilds", TNS.AdExpress.Web.Functions.Script.SelectExclusiveAllChilds());
                }
                #endregion
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }

        }
        #endregion

        #region Déchargement de la page
        /// <summary>
        /// Evènement de déchargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        protected void Page_UnLoad(object sender, System.EventArgs e)
        {
        }
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

            this.Unload += new System.EventHandler(this.Page_UnLoad);


        }
        #endregion

        #region DeterminePostBackMode
        /// <summary>
        /// Initialisation de certains composants
        /// </summary>
        /// <returns>?</returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
        {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            SectorAnalysisVehicleSelectionWebControl1.CustomerWebSession = _webSession;
            MenuWebControl2.CustomerWebSession = _webSession;
            return tmp;
        }
        #endregion

        #region Bouton valider
        /// <summary>
        /// Sauvegarde de l'univers des médias sélectionnés
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        protected void validateButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                #region Variables locales
                System.Windows.Forms.TreeNode current = new System.Windows.Forms.TreeNode("ChoixMedia");
                System.Windows.Forms.TreeNode vehicle = null;
                string idVehicule = "";
                string OldIdVehicule = "";
                bool firstIdVeh = true;
                bool vhAdded = false;
                System.Windows.Forms.TreeNode region = null;
                System.Windows.Forms.TreeNode media = null;
                string idRegion = "";
                string OldIdRegion = "";
                bool firstIdCat = true;
                int vhCpt = 0;
                int ctCpt = 0;
                int mdCpt = 0;
                #endregion

                #region Construction de l'arbre des médias
                foreach (ListItem item in this.SectorAnalysisVehicleSelectionWebControl1.Items)
                {
                    //Traitement Noeud media(vehicle)
                    if (item.Value.IndexOf("vh_") > -1)
                    {
                        idVehicule = item.Value.ToString();
                        //ajout de la Region précédente 
                        if (!firstIdVeh && ((vehicle.Nodes.Count > 0 || vehicle.Checked) || (region != null && (region.Nodes.Count > 0 || region.Checked))) && !idVehicule.Equals("") && !OldIdVehicule.Equals(idVehicule.Trim()))
                        {
                            //Rajout de la dernière region dans media(vehicle) puis du dernier media dans l'arbre													
                            if (!vehicle.Checked && region != null && !vehicle.Nodes.Contains(region)) vehicle.Nodes.Add(region);
                            current.Nodes.Add(vehicle);
                            vhAdded = true;
                            break;
                        }
                        vehicle = new System.Windows.Forms.TreeNode(item.Text);
                        //Creation d'un nouveau noeud média (vehicle)																	
                        if (item.Selected)
                            vehicle.Tag = new LevelInformation(CstWebCustomer.Right.type.vehicleAccess, Int64.Parse(item.Value.Remove(0, 3)), item.Text);
                        else vehicle.Tag = new LevelInformation(CstWebCustomer.Right.type.vehicleException, Int64.Parse(item.Value.Remove(0, 3)), item.Text);
                        vehicle.Checked = item.Selected;
                        if (vehicle.Checked) vhCpt++;
                        idVehicule = item.Value.ToString();
                        OldIdVehicule = item.Value.ToString();
                        firstIdVeh = false;
                    }
                    //Traitement Noeud Region
                    else if (!vehicle.Checked && item.Value.IndexOf("ct_") > -1)
                    {
                        idRegion = item.Value.ToString();
                        //ajout de la Region précédente 
                        if (!firstIdCat && (region.Nodes.Count > 0 || region.Checked) && !idRegion.Equals("") && !OldIdRegion.Equals(idRegion.Trim()))
                        {
                            vehicle.Nodes.Add(region);
                        }
                        //Creation d'un nouveau noeud Region
                        region = new System.Windows.Forms.TreeNode(item.Text);
                        string[] catArr = idRegion.Split('_');
                        if (item.Selected)
                            region.Tag = new LevelInformation(CstWebCustomer.Right.type.regionAccess, Int64.Parse(catArr[1]), item.Text);
                        else region.Tag = new LevelInformation(CstWebCustomer.Right.type.regionException, Int64.Parse(catArr[1]), item.Text);
                        region.Checked = item.Selected;
                        if (region.Checked) ctCpt++;
                        OldIdRegion = item.Value.ToString();
                        firstIdCat = false;
                    }
                    //Traitement Noeud support(media)
                    else if (!vehicle.Checked && !region.Checked && item.Value.IndexOf("md_") > -1 && item.Selected)
                    {
                        media = new System.Windows.Forms.TreeNode(item.Text);
                        media.Tag = new LevelInformation(CstWebCustomer.Right.type.mediaAccess, Int64.Parse(item.Value.Remove(0, 3)), item.Text);
                        media.Checked = item.Selected;
                        if (media.Checked) mdCpt++;
                        region.Nodes.Add(media);
                    }
                }
                //ajout de la dernière node vehicle si nécessaire
                //Rajout de la dernière region dans media(vehicle) puis du dernier media dans l'arbre	
                if (!vehicle.Checked && region != null && vehicle != null && (region.Checked || region.Nodes.Count > 0) && !vehicle.Nodes.Contains(region))
                {
                    vehicle.Nodes.Add(region);
                }
                if (current.Nodes.Count == 0 && !vhAdded && (vehicle.Nodes.Count > 0 || vehicle.Checked))
                    current.Nodes.Add(vehicle);
                #endregion

                #region MAJ de la session
                if ((mdCpt + ctCpt + vhCpt) > 0)
                {
                    if (mdCpt < WebConstantes.Advertiser.MAX_NUMBER_ADVERTISER
                        && vhCpt < WebConstantes.Advertiser.MAX_NUMBER_ADVERTISER
                        && ctCpt < WebConstantes.Advertiser.MAX_NUMBER_ADVERTISER)
                    {

                        #region Tracking
                        _webSession.OnSetVehicle(((LevelInformation)current.Nodes[0].Tag).ID);
                        #endregion

                        #region Extraction du dernier moi sdispo dans les recap
                        CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                        object[] param = new object[1];
                        param[0] = _webSession;
                        IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                            , AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName), cl.Class, false
                            , BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                        _webSession.LastAvailableRecapMonth = dateDAL.CheckAvailableDateForMedia(((LevelInformation)vehicle.Tag).ID);

                        #endregion

                        _webSession.SelectionUniversMedia = _webSession.CurrentUniversMedia = current;
                        if (((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID != VehiclesInformation.Get(DBConstantesClassification.Vehicles.names.plurimedia).DatabaseId)
                            _webSession.PreformatedMediaDetail = WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleRegion;
                        else
                            _webSession.PreformatedMediaDetail = WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle;
                        _webSession.Save();
                        //Redirection Page suivante
                        _webSession.Source.Close();
                        Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession + "");
                    }
                    else
                    {
                        foreach (ListItem item in this.SectorAnalysisVehicleSelectionWebControl1.Items)
                        {
                            if (item.Selected) item.Selected = false;
                        }
                        this.SectorAnalysisVehicleSelectionWebControl1.Reload = true;
                        Response.Write("<script language=javascript>");
                        Response.Write(" alert(\"" + GestionWeb.GetWebWord(2265, _webSession.SiteLanguage) + "\");");
                        Response.Write("</script>");
                    }
                }
                else
                {
                    this.SectorAnalysisVehicleSelectionWebControl1.Reload = true;
                    Response.Write("<script language=javascript>");
                    Response.Write(" alert(\"" + GestionWeb.GetWebWord(1199, _webSession.SiteLanguage) + "\");");
                    Response.Write("</script>");
                }
                #endregion

            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
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
        protected override void ValidateSelection(object sender, System.EventArgs e)
        {
            this.validateButton_Click(sender, e);
        }
        /// <summary>
        /// Retrieve next Url from the menu
        /// </summary>
        /// <returns>Next Url</returns>
        protected override string GetNextUrlFromMenu()
        {
            return (this.MenuWebControl2.NextUrl);
        }
        #endregion

    }
}