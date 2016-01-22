#region Informations
// Auteur: D. V. Mussuma
// Date de création: 2/10/2009
// Date de modification:  
//	01/08/2006 Modification FindNextUrl
#endregion
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using constEvent = TNS.AdExpress.Constantes.FrameWork.Selection;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;

public partial class Private_Selection_Russia_SubMediaSelection : TNS.AdExpress.Web.UI.SelectionWebPage
{

    /// <summary>
    /// TextBox : mot clé
    /// </summary>
    /// <summary>
    /// Evènement lancé
    /// </summary>
    public int eventButton = -1;
    /// <summary>
    /// Fermeture du Flash d'attente
    /// </summary>
    public string divClose = "";

    #region Constructor
    /// <summary>
    /// Constructeur
    /// </summary>
    public Private_Selection_Russia_SubMediaSelection()
        : base()
    {
    }
    #endregion

    #region  Page Load
    /// <summary>
    /// Page Load
    /// </summary>
    /// <param name="sender">Objet qui lance l'évènement</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, System.EventArgs e)
    {
        try
        {

            #region Textes et langage du site
            ModuleTitleWebControl1.CustomerWebSession = _webSession;
            #endregion

            //Rollover des boutons            
            validateButton.ToolTip = GestionWeb.GetWebWord(1536, _webSession.SiteLanguage);

            #region évènemment
            // Connaître le boutton qui a été cliquer 
            eventButton = 0;

            // Boutton valider
            if (Request.Form.Get("__EVENTTARGET") == "validateButton")
            {
                eventButton = constEvent.eventSelection.VALID_EVENT;
            }
            // Controle Recall
            else if (Request.Form.Get("__EVENTTARGET") == "MenuWebControl2")
            {
                eventButton = constEvent.eventSelection.VALID_EVENT;
            }

            #endregion

            // Définition du contrôle SubMediaSelectionWebControl1
            SubMediaSelectionWebControl1.WebSession = _webSession;


            #region Script
            //Gestion de la sélection d'un radiobutton dans la liste des univers
            if (!Page.ClientScript.IsClientScriptBlockRegistered("InsertIdMySession4"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "InsertIdMySession4", TNS.AdExpress.Web.Functions.Script.InsertIdMySession4());
            }
            // Ouverture/fermeture des fenêtres pères
            if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "showHideContent", TNS.AdExpress.Web.Functions.Script.ShowHideContent());
            }

            // Gestion lorsqu'on clique sur entrée
            if (!Page.ClientScript.IsClientScriptBlockRegistered("trapEnter"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "trapEnter", TNS.AdExpress.Web.Functions.Script.TrapEnter("OkImageButtonRollOverWebControl"));
            }

            if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowHideContent1", TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
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

    #region DeterminePostBackMode
    /// <summary>
    /// Initialisation de certains composants
    /// </summary>
    /// <returns>?</returns>
    protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
    {
        System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();

        try
        {

            SubMediaSelectionWebControl1.WebSession = _webSession;
            MenuWebControl2.CustomerWebSession = _webSession;
            MenuWebControl2.ForbidResultIcon = true;
            MenuWebControl2.ForbidOptionPagesList = new ArrayList();
            MenuWebControl2.ForbidOptionPagesList.Add(4);
            _webSession.SelectionUniversMedia.FirstNode.Nodes.Clear();
           
        }
        catch (System.Exception exc)
        {
            if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
            {
                this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
            }
        }
        return tmp;
    }
    #endregion

    #region Bouton Valider
    /// <summary>
    /// Valide la sélection
    /// </summary>
    /// <param name="sender">Objet qui lance l'évènement</param>
    /// <param name="e">Arguments</param>
    protected void validateButton_Click(object sender, System.EventArgs e)
    {
        try
        {
            _webSession.CurrentUniversMedia.Nodes.Clear();
            System.Windows.Forms.TreeNode tmpNode;
            foreach (ListItem currentItem in SubMediaSelectionWebControl1.Items)
            {
                if (currentItem.Selected)
                {
                    tmpNode = new System.Windows.Forms.TreeNode(currentItem.Text);
                    tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess, long.Parse(currentItem.Value), currentItem.Text);
                    tmpNode.Checked = true;
                    //_webSession.CurrentUniversMedia.Nodes.Add(tmpNode);
                    _webSession.SelectionUniversMedia.FirstNode.Nodes.Add(tmpNode);
                    break;//select only one sub media
                }
            }
            if (_webSession.SelectionUniversMedia.FirstNode != null && _webSession.SelectionUniversMedia.FirstNode.Nodes.Count > 0)
            {
                _webSession.Save();
                _webSession.Source.Close();
                Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession + "");
            }
            else
            {

                // Erreur : Aucun élément n'est sélectionné
                Response.Write("<script language=javascript>");
                Response.Write("alert(\"" + GestionWeb.GetWebWord(878, _webSession.SiteLanguage) + "\");");
                Response.Write("history.go(-1);");
                Response.Write("</script>");

            }


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

    #region Construction de l'arbre
    /// <summary>
    /// Fonction qui construit un arbre à partir des éléments sélectionnés dans le webControl
    /// sectorSelectionWebControl
    /// </summary>
    /// <returns></returns>
    private System.Windows.Forms.TreeNode TreeBuilding()
    {
        System.Windows.Forms.TreeNode submedias = new System.Windows.Forms.TreeNode("submedias");
        System.Windows.Forms.TreeNode tmp;
        foreach (ListItem item in this.SubMediaSelectionWebControl1.Items)
        {
            if (item.Selected)
            {
                tmp = new System.Windows.Forms.TreeNode(item.Text);
                tmp.Tag = new LevelInformation(CstWebCustomer.Right.type.categoryAccess, Int64.Parse(item.Value), item.Text);
                tmp.Checked = true;
                submedias.Nodes.Add(tmp);
            }
        }
        return submedias;
    }
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
