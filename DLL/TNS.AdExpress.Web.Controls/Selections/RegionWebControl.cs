#region Informations
// Auteur: D. V. Mussuma
// Date de création: 5/05/2005

#endregion
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using System.Windows.Forms;

using TNS.AdExpress.Web.DataAccess.Selections;
using TNS.AdExpress.Web.DataAccess.Selections.Products;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using constEvent = TNS.AdExpress.Constantes.FrameWork.Selection;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebModule = TNS.AdExpress.Constantes.Web.Module;
using TNS.AdExpress.Web.Controls.Exceptions;
using RightConstantes = TNS.AdExpress.Constantes.Customer.Right;
using TNS.Classification;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.Classification;
using TNS.Classification.Universe;
using System.Reflection;

namespace TNS.AdExpress.Web.Controls.Selections {
    /// <summary>
    /// Composant affichant la liste des familles;
    /// Il est utilisé dans la sélection de familles	
    /// en fonction des droits client.
    /// </summary>
    [DefaultProperty("Text"),
        ToolboxData("<{0}:RegionWebControl runat=server></{0}:RegionWebControl>")]
    public class RegionWebControl : System.Web.UI.WebControls.CheckBoxList {

        #region Variables
        /// <summary>
        /// Dataset contenant la liste des familles
        /// </summary>
        protected DataSet dsRegionList;
        /// <summary>
        /// Session
        /// </summary>
        protected WebSession webSession;
        /// <summary>
        /// Indique si on reconstruit la liste d'items dans checkboxlist
        /// </summary>
        protected bool reBuildCheckboxlist = false;
        /// <summary>
        /// Titre du webControl
        /// </summary>
        protected string title;
        /// <summary>
        /// Evènement qui a été lancé
        /// </summary>
        protected int eventButton;
        /// <summary>
        /// valeur qui permet de savoir si des éléments ont
        /// été sélectionnés
        /// </summary>
        protected int nbElement = 1;
        /// <summary>
        ///  Vérifie si le chargement est possible
        /// </summary>
        protected bool loadIsPossible = true;
        /// <summary>
        /// Vérifie si les familles n'ont pas déjà été sélectionner
        /// </summary>
        protected bool saveValue = false;
        /// <summary>
        /// Entier qui détermine qu'elle bouton 
        /// a été cliquer 1 pour valider
        /// 2 pour ok
        /// </summary>
        int buttonTarget;
        /// <summary>
        /// Indique la présence d'enregistrements média
        /// </summary>
        public bool hasRecords = true;
        /// <summary>
        /// Liste des produits à exclure pour la Presse
        /// </summary>
        private IList excludeSector = null;
        /// <summary>
        /// Indique si on charge l'arbre Selection des familles présent dans la session ou non
        /// </summary>
        public bool LoadSession = false;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public RegionWebControl() : base() {
            this.EnableViewState = true;
        }
        #endregion

        #region Accesseurs
        /// <summary>
        /// Obtient ou définit le titre du tableau de familles
        /// </summary>
        public string Title {
            get { return title; }
            set { title = value; }
        }
        /// <summary>
        /// Obtient le dataset avec la liste des familles
        /// </summary>
        public DataSet DsRegionList {
            get { return dsRegionList; }
        }
        /// <summary>
        /// Obtient ou définit la webSession 
        /// </summary>
        public virtual WebSession WebSession {
            get { return webSession; }
            set { webSession = value; }
        }
        /// <summary>
        /// Obtient ou définit reBuildCheckboxlist
        /// </summary>
        public bool ReBuildCheckboxlist {
            get { return reBuildCheckboxlist; }
            set { reBuildCheckboxlist = value; }
        }
        /// <summary>
        /// Définit l'évènement qui a été lancer
        /// </summary>
        public virtual int EventButton {
            set { eventButton = value; }
        }
        /// <summary>
        /// Obtient ou définit la valeur qui permet de savoir si des éléments ont
        /// été sélectionnés
        /// </summary>
        public int NbElement {
            get { return nbElement; }
            set { nbElement = value; }
        }
        /// <summary>
        /// Obtient ou définit la valeur du boutton qui a été
        /// cliqué
        /// </summary>
        public int ButtonTarget {
            get { return buttonTarget; }
            set { buttonTarget = value; }
        }
        #endregion

        #region Evènements

        #region  Chargement
        /// <summary>
        /// Chargement du contrôle
        /// </summary>
        /// <param name="e">arguments</param>
        protected override void OnLoad(EventArgs e) {

            if (webSession != null) {

                //Chargement des données	
                TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
                object[] param = new object[2];
                param[0] = webSession;
                param[1] = Dimension.media;
                TNS.AdExpressI.Classification.DAL.ClassificationDAL classficationDAL = (TNS.AdExpressI.Classification.DAL.ClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                dsRegionList = classficationDAL.GetItems(TNS.Classification.Universe.TNSClassificationLevels.REGION, "%");

            }
            else throw (new WebControlInitializationException("Impossible d'initialiser le composant, la session n'est pas définie."));

            if (dsRegionList.Equals(System.DBNull.Value) || dsRegionList.Tables[0] == null || dsRegionList.Tables[0].Rows.Count == 0)
                hasRecords = false;

            // Sélection de tous les fils
            if (!Page.ClientScript.IsClientScriptBlockRegistered("SelectAllChilds")) {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SelectAllChilds", TNS.AdExpress.Web.Functions.Script.SelectAllChilds());
            }
            base.OnLoad(e);

        }
        #endregion

        #region PreRender
        /// <summary>
        /// Construction de la liste des  éléméents familles
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e) {

            #region variables locales
            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            #endregion

            //Initialisation de la liste d'items
            this.Items.Clear();

            //Construction de la liste de checkbox				
            foreach (DataRow currentRow in dsRegionList.Tables[0].Rows) {
                this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["item"].ToString(), currentRow["id_item"].ToString()));
            }
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output) {

            System.Text.StringBuilder t = new System.Text.StringBuilder(1000);

            #region Construction du code HTML
            if (dsRegionList != null && dsRegionList.Tables[0].Rows.Count != 0) {

                int i = 0;
                string checkBox = "";
                string checkSc = "";
                Int64 idRegion = -1;

                #region Affichage des familles

                #region Début du tableau global
                t.Append("<tr vAlign=\"top\"   align=\"center\" class=\"backGroundWhite\"><td><br><div width=\"100%\" vAlign=\"top\" id=\"regions\">");
                t.Append("<table width=\"100%\" class=\"violetBorder txtViolet11Bold lightPurple\" vAlign=\"top\" cellSpacing=\"0\">");
                t.Append("<tr ><td colspan=\"3\" class=\"txtViolet11Bold backGroundWhite violetBorderBottom\">" + title + "</td></tr>");

                t.Append("<tr><td colspan=\"3\"><a href=\"javascript: SelectAllChilds('regions')\" title=\"" + GestionWeb.GetWebWord(2800, webSession.SiteLanguage) + "\" class=\"roll04\">" + GestionWeb.GetWebWord(2800, webSession.SiteLanguage) + "</a></td></tr>");
                #endregion

                #region Ajout des regions
                foreach (DataRow currentRow in dsRegionList.Tables[0].Rows) {

                    idRegion = Convert.ToInt64(currentRow["id_item"]);

                    #region Etat de cochage de la region
                    try {
                        checkSc = checkBox = "";

                        //Vérification de l'état de la famille (sélectionnée ou non)
                        if (LoadSession && webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0 && webSession.PrincipalProductUniverses[0].GetGroup(webSession.PrincipalProductUniverses[0].Count()).Contains(idRegion)) {
                            checkSc = checkBox = " Checked ";
                        }
                    }
                    //Exceptions si le centre d'intérêt n'est pas trouver dans l'arbre
                    catch (System.NullReferenceException) {
                        //Gestion éléments sauvegardés
                    }
                    #endregion

                    if ((i % 3) < 1) t.Append("<tr>");
                    t.Append("<td width=\"33%\" ><input type=\"checkBox\" " + checkBox + " ");
                    t.Append(" id=\"SectorSelectionWebControl_" + i + "\" name=\"SectorSelectionWebControl$" + i + "\"/  value=\"" + currentRow["id_item"] + "\" >" + currentRow["item"].ToString() + "</td>");
                    if ((i % 3) > 1) t.Append("</tr>");
                    i++;
                }

                if (i > 0 && ((i - 1) % 3) < 2) t.Append("</tr>");
                #endregion

                #region Fin du tableau
                t.Append("</table></div></td></tr>");
                #endregion

                #endregion
            }
            else {

                #region Pas de familles
                t.Append("<tr vAlign=\"top\" ><td class=\"txtGris11Bold whiteBackGround\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
                t.Append(" " + GestionWeb.GetWebWord(1534, webSession.SiteLanguage) + "</p> ");
                t.Append(" </td> ");
                t.Append(" </tr> ");
                #endregion
            }
            #endregion

            output.Write(t.ToString());
        }
        #endregion

        #endregion

        #region Méthodes internes
        /// <summary>
        /// Construction d'une liste d'éléments famille sélectionnés 
        /// </summary>
        /// <param name="tree">arbre</param>
        /// <returns>liste des fils d'un noeud</returns>
        private Hashtable LoadSectorsFromTreeNode(System.Windows.Forms.TreeNode tree) {
            Hashtable t = new Hashtable();
            foreach (System.Windows.Forms.TreeNode node in tree.Nodes) {
                t.Add(((LevelInformation)node.Tag).ID, node.Index);
            }
            return t;
        }
        /// <summary>
        /// Construction d'une liste d'éléments famille sélectionnés 
        /// </summary>
        /// <param name="adExpressUniverse">adExpress universe</param>
        /// <returns>liste des fils d'un noeud</returns>
        private Hashtable LoadSectorsFromUniverse(Dictionary<int, AdExpressUniverse> adExpressUniverse) {
            Hashtable t = new Hashtable();
            return t;
        }

        #region création de l'arbre
        /// <summary>
        /// Création de l'arbre à partir de la liste des Items
        /// </summary>
        /// <returns>familles sélectionnées</returns>
        protected System.Windows.Forms.TreeNode createTreeNode() {

            System.Windows.Forms.TreeNode productTree = new System.Windows.Forms.TreeNode("sector");

            // product sélectionnés		
            productTree = ((System.Windows.Forms.TreeNode)webSession.CurrentUniversProduct);
            int compteurChild = 0;

            if (webSession.CurrentUniversProduct.FirstNode != null) {
                webSession.CurrentUniversProduct.Nodes.Clear();
            }

            #region foreach
            System.Windows.Forms.TreeNode tmp;
            foreach (ListItem item in this.Items) {

                if (item.Selected) {
                    tmp = new System.Windows.Forms.TreeNode(item.Text);
                    tmp.Tag = new LevelInformation(CstWebCustomer.Right.type.sectorAccess, Int64.Parse(item.Value), item.Text);
                    tmp.Checked = true;
                    productTree.Nodes.Add(tmp);
                    compteurChild++;
                }
            }
            #endregion

            // Fournit une valeur à nbElement pour vérifier la validité du nombre 
            // d'éléments sélectionné
            if (compteurChild == 0) {
                nbElement = 3;
            }

            return productTree;
        }
        #endregion

        #endregion
    }
}
