#region Informations
// Auteur: D. Mussuma
// Date de création: 
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
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using CstWeb = TNS.AdExpress.Constantes.Web;
using DataAccess = TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Translation;
using DBFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Domain.Classification;
using TNS.Ares.Domain.LS;
using WebCst = TNS.AdExpress.Constantes.Web;
using TNS.Alert.Domain;
using TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using TNS.AdExpress.Domain.Web;

public partial class Private_MyAdExpress_PersonnalizeInsertion : TNS.AdExpress.Web.UI.PrivateWebPage
{

    #region Variables
    /// <summary>
    /// Script
    /// </summary>
    protected string script;
    /// <summary>
    /// Liste of group of saved insertions
    /// </summary>
    protected string listInsertionRepertories;
    /// <summary>
    /// List of saved insertion levels to delete
    /// </summary>
    public string listInsertionsToDelete;
    /// <summary>
    /// List of saved insertion levels to rename
    /// </summary>
    public string listInsertionToRename;
    #endregion



    #region Properties

    public bool IsAlertsActivated
    {
        get
        {
            return (AlertConfiguration.IsActivated && _webSession.CustomerLogin.HasModuleAssignmentAlertsAdExpress());
        }
    }

    #endregion

    #region Page_Load
    /// <summary>
    /// Page load
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {           
            //List of save insertions to move
            listInsertionRepertories = GetSelectionTableHtmlUI(4, 500, "idMyInsertion2");

            //List of saved insertions to delete
            listInsertionsToDelete = GetSelectionTableHtmlUI(6, 500,"idMyInsertion3");

            // List of  saved insertions to rename
            listInsertionToRename = GetSelectionTableHtmlUI(7, 500,"idMyInsertion1");

            // Header			
            HeaderWebControl1.ActiveMenu = CstWeb.MenuTraductions.MY_ADEXPRESS;

            #region Script
            //Script
            if (!Page.ClientScript.IsClientScriptBlockRegistered("showHideContent"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "showHideContent", TNS.AdExpress.Web.Functions.Script.ShowHideContent());
            }

            if (!Page.ClientScript.IsClientScriptBlockRegistered("ShowHideContent1"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowHideContent1", TNS.AdExpress.Web.Functions.Script.ShowHideContent1(1));
            }
            #endregion

            #region Rollover des boutons

            insertionDeleteImageButtonRollOverWebControl.Attributes.Add("onclick", "javascript: return confirm('" + GestionWeb.GetWebWord(2925, _webSession.SiteLanguage) + "');");
            deleteInsertionImageButtonRollOverWebControl.Attributes.Add("onclick", "javascript: return confirm('" + GestionWeb.GetWebWord(2926, _webSession.SiteLanguage) + "');");
            #endregion

            #region Load directory list
            if (!IsPostBack)
            {
                DataSet ds = InsertionLevelDAL.GetGroupInsertionLevels(_webSession); //TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetGroupUniverses(_webSession);

                insertionDirectoryDropDownList.DataSource = ds.Tables[0];
                insertionDirectoryDropDownList.DataTextField = ds.Tables[0].Columns["GROUP_INSERTION_SAVE"].ToString();
                insertionDirectoryDropDownList.DataValueField = ds.Tables[0].Columns["ID_GROUP_INSERTION_SAVE"].ToString();
                insertionDirectoryDropDownList.DataBind();

                renameInsertionDirectoryDropDownList.DataSource = ds.Tables[0];
                renameInsertionDirectoryDropDownList.DataTextField = ds.Tables[0].Columns["GROUP_INSERTION_SAVE"].ToString();
                renameInsertionDirectoryDropDownList.DataValueField = ds.Tables[0].Columns["ID_GROUP_INSERTION_SAVE"].ToString();
                renameInsertionDirectoryDropDownList.DataBind();

                moveInsertionDirectoryDropDownList.DataSource = ds.Tables[0];
                moveInsertionDirectoryDropDownList.DataTextField = ds.Tables[0].Columns["GROUP_INSERTION_SAVE"].ToString();
                moveInsertionDirectoryDropDownList.DataValueField = ds.Tables[0].Columns["ID_GROUP_INSERTION_SAVE"].ToString();
                moveInsertionDirectoryDropDownList.DataBind();
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

    #region GetSelectionTableHtmlUI
    /// <summary>
    /// Get save insertion classified by directories 
    /// </summary>
    /// <returns>HTml string of save insertion list</returns>
    protected string GetSelectionTableHtmlUI(int valueTable, int width,string inputHiddenKey)
    {
        DataSet dsListDirectory = InsertionLevelDAL.GetData(_webSession);
        System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
        Int64 idParent;
        Int64 idParentOld = long.MinValue;
        string textParent;
        string textParentOld;
        int start = -1;
        int compteur = 0;


        if (dsListDirectory.Tables[0].Rows.Count != 0)
        {
            foreach (DataRow currentRow in dsListDirectory.Tables[0].Rows)
            {
                idParent = Convert.ToInt64(currentRow["ID_GROUP_INSERTION_SAVE"]);
                textParent = currentRow["GROUP_INSERTION_SAVE"].ToString();
                //First
                if (idParent != idParentOld && start != 0)
                {

                    t.Append("<table class=\"violetBorder txtViolet11Bold\" cellpadding=0 cellspacing=0   width=500>");

                    t.Append("<tr onclick=\"showHideContent" + valueTable + "('" + idParent + "');\" class=\"cursorHand\">");
                    t.Append("<td align=\"left\" height=\"10\" valign=\"middle\">");
                    t.Append("<label ID=\"" + currentRow["ID_GROUP_INSERTION_SAVE"] + valueTable + "\">&nbsp;");
                    t.AppendFormat("{0}", textParent);
                    t.Append("</label>");
                    t.Append("</td>");
                    t.Append("<td class=\"arrowBackGround\"></td>");
                    t.Append("</tr>");
                    t.Append("</table>");
                    t.Append("<div id=\"" + idParent + "Content" + valueTable + "\" style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\" >");
                    t.Append("<table class=\"violetBorderWithoutTop paleVioletBackGround\" width=" + width + ">");


                    idParentOld = idParent;
                    textParentOld = textParent;
                    start = 0;
                    compteur = 0;

                }
                else if (idParent != idParentOld)
                {
                    t.Append("</table>");
                    t.Append("</div>");
                    t.Append("<table class=\"violetBorderWithoutTop txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=" + width + ">");
                    t.Append("<tr onclick=\"showHideContent" + valueTable + "('" + idParent + "');\" class=\"cursorHand\">");
                    t.Append("<td align=\"left\" height=\"10\" valign=\"middle\">");
                    t.Append("<label ID=\"" + currentRow["ID_GROUP_INSERTION_SAVE"] + valueTable + "\">&nbsp;");
                    t.AppendFormat("{0}", textParent);
                    t.Append("</label>");
                    t.Append("</td>");
                    t.Append("<td class=\"arrowBackGround\"></td>");
                    t.Append("</tr>");
                    t.Append("</table>");
                    t.Append("<div id=\"" + idParent + "Content" + valueTable + "\"  style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\">");
                    t.Append("<table class=\"violetBorderWithoutTop paleVioletBackGround\" width=" + width + ">");


                    idParentOld = idParent;
                    textParentOld = textParent;
                    compteur = 0;
                }

                if (currentRow["ID_INSERTION_SAVE"] != System.DBNull.Value)
                {
                    if (compteur == 0)
                    {
                        t.Append("<tr><td class=\"txtViolet10\" width=50%>");
                        t.Append("<input type=\"radio\" ID=\"" + currentRow["ID_INSERTION_SAVE"] + "_" + currentRow["ID_GROUP_INSERTION_SAVE"] + "\" onclick=\"GetIdMyInsertionSave('" + currentRow["ID_INSERTION_SAVE"] + "','" + currentRow["ID_GROUP_INSERTION_SAVE"] + "','" + inputHiddenKey + "');\" value=\"" + currentRow["ID_INSERTION_SAVE"] + "\" name=\"insertion\">" + currentRow["INSERTION_SAVE"].ToString() + "<br>");
                        t.Append("</td>");
                        compteur = 1;
                    }
                    else
                    {
                        t.Append("<td class=\"txtViolet10\" width=50%>");
                        t.Append("<input type=\"radio\" ID=\"" + currentRow["ID_INSERTION_SAVE"] + "_"+currentRow["ID_GROUP_INSERTION_SAVE"] + "\" onclick=\"GetIdMyInsertionSave('" + currentRow["ID_INSERTION_SAVE"] + "','" + currentRow["ID_GROUP_INSERTION_SAVE"] + "','" + inputHiddenKey + "');\" value=\"" + currentRow["ID_INSERTION_SAVE"] + "\" name=\"insertion\">" + currentRow["INSERTION_SAVE"].ToString() + "<br>");
                        t.Append("</td></tr>");
                        compteur = 0;
                    }
                }
                else
                {
                    t.Append("<tr><td class=\"txtViolet10\" width=50%>");
                    t.Append(GestionWeb.GetWebWord(285, _webSession.SiteLanguage));
                    t.Append("</td></tr>");

                }
            }
            if (compteur != 0)
            {
                t.Append("</tr>");
            }
            t.Append("</table>");
            t.Append("</div>");

            return (t.ToString());
        }
        return ("");
    }
    #endregion

    #region createInsertionRepertoryImageButtonRollOverWebControl_Click
    /// <summary>
    /// Create saved insertion directory
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">arguments</param>
    protected void createInsertionRepertoryImageButtonRollOverWebControl_Click(object sender, EventArgs e)
    {
        try
        {
            string directoryName = this.createRepertoryTextBox.Text;
            directoryName = CheckedText.CheckedAccentText(directoryName);
            if (directoryName.Length != 0 && directoryName.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT)
            {
                if (!InsertionLevelDAL.IsGroupInsertionSaveExist(_webSession, directoryName))
                {
                    if (InsertionLevelDAL.CreateGroupInsertionSave(directoryName, _webSession))
                    {
                        // Validation : confirmation of directory creation
                        Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(835, _webSession.SiteLanguage)));

                        createRepertoryTextBox.Text = "";

                        // Rafraichi la liste des répertoires
                        refreshDirectories();

                    }
                    else
                    {
                        // Erreor: can't create directory
                        Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(836, _webSession.SiteLanguage)));
                    }
                }
                else
                {
                    //Erreur : directory already exists
                    Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(834, _webSession.SiteLanguage)));
                }

            }
            else if (directoryName.Length == 0)
            {
                // Erreur : the field is empty
                Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(837, _webSession.SiteLanguage)));
            }
            else
            {
                // Erreur : superior to 50 char
                Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(823, _webSession.SiteLanguage)));
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

    #region insertionDeleteImageButtonRollOverWebControl_Click
    /// <summary>
    /// Delete directory
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">arguments</param>
    protected void insertionDeleteImageButtonRollOverWebControl_Click(object sender, EventArgs e)
    {
        try
        {
            if (insertionDirectoryDropDownList.Items.Count != 0)
            {
                if (InsertionLevelDAL.GetGroupInsertionLevels(_webSession).Tables[0].Rows.Count > 1)
                {
                    if (!InsertionLevelDAL.IsInsertionInGroupInsertionExist(_webSession, Int64.Parse(insertionDirectoryDropDownList.SelectedItem.Value)))
                    {
                        InsertionLevelDAL.DropGroupInsertionSave(Int64.Parse(insertionDirectoryDropDownList.SelectedItem.Value), _webSession);

                        // Refresh directory list
                        this.refreshDirectories();

                    }
                    else
                    {
                        // Erreur : Impossible to delete directory whic  contains items
                        Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(838, _webSession.SiteLanguage)));
                    }
                }
                else
                {
                    //Erreur : Impossible to delete your    directoy
                    Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(840, _webSession.SiteLanguage)));
                }
            }
            else
            {
                //Erreur : aucun groupe d'univers
                Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(839, _webSession.SiteLanguage)));
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

    #region renameInsertionImageButtonRollOverWebControl_Click

    /// <summary>
    /// Rename directory
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">arguments</param>
    protected void renameInsertionImageButtonRollOverWebControl_Click(object sender, EventArgs e)
    {
        try
        {
            if (renameInsertionDirectoryDropDownList.Items.Count != 0)
            {
                string newDirectoryName = renameInsertionDirectoryTextBox.Text;
                newDirectoryName = CheckedText.CheckedAccentText(newDirectoryName);

                if (newDirectoryName.Length != 0 && newDirectoryName.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT)
                {

                    if (!InsertionLevelDAL.IsGroupInsertionSaveExist(_webSession, newDirectoryName))
                    {
                        InsertionLevelDAL.RenameGroupInsertionSave(newDirectoryName, Int64.Parse(renameInsertionDirectoryDropDownList.SelectedItem.Value), _webSession);
                        // Vide le champs de saisie
                        renameInsertionDirectoryTextBox.Text = "";
                        // Rafraichi la liste des répertoires
                        this.refreshDirectories();
                    }
                    else
                    {
                        //Erreur : groupe d'insertion déjà existant
                        Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(2260, _webSession.SiteLanguage)));
                    }
                }
                else if (newDirectoryName.Length == 0)
                {
                    // Erreur : Le champs est vide
                    Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(837, _webSession.SiteLanguage)));
                }
                else
                {
                    // Erreur : suppérieur à 50 caractères
                    Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(823, _webSession.SiteLanguage)));
                }
            }
            else
            {
                //Erreur : aucun groupe d'univers
                Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(839, _webSession.SiteLanguage)));
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

    #region renameInsertionImagebutton_Click
    /// <summary>
    /// Rename inseertion
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">arguements</param>
    protected void renameInsertionImagebutton_Click(object sender, EventArgs e)
    {
        try
        {

            Int64 idInsertionSave = 0;
            string currentKey = Request.Form["idMyInsertion1"];
        
            if (!string.IsNullOrEmpty(currentKey))
            {
                string[] tabParent = tabParent = currentKey.Split('_');
                if (tabParent != null && tabParent.Length==3 && tabParent[0] == "CKB")
                idInsertionSave = Int64.Parse(tabParent[1]);
            }

            string newInsertionName = renameUniverseTextBox.Text;
            newInsertionName = CheckedText.CheckedAccentText(newInsertionName);

            if (idInsertionSave != 0)
            {
                if (newInsertionName.Length != 0 && newInsertionName.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT)
                {

                    if (!InsertionLevelDAL.IsInsertionLevelsExist(_webSession, newInsertionName))
                    {
                        InsertionLevelDAL.RenameInsertionSave(newInsertionName, idInsertionSave, _webSession);
                        renameUniverseTextBox.Text = "";
                        // Rafraichi la liste des répertoires
                        this.refreshDirectories();
                    }
                    else
                    {
                        //Erreur : L'univers existe déjà
                        Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(2260, _webSession.SiteLanguage)));
                    }
                }
                else if (newInsertionName.Length == 0)
                {
                    // Erreur : Le champs est vide
                    Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(837, _webSession.SiteLanguage)));
                }
                else
                {
                    // Erreur : suppérieur à 50 caractères
                    Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(823, _webSession.SiteLanguage)));
                }

            }
            else
            {
                // Erreur : aucun élément de sélectionné
                Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(926, _webSession.SiteLanguage)));
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

    #region moveInsertionImageButtonRollOverWebControl_Click
    /// <summary>
    /// moveInsertionImageButtonRollOverWebControl_Click
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">arguments</param>
    protected void moveInsertionImageButtonRollOverWebControl_Click(object sender, EventArgs e)
    {
        try
        {
            if (moveInsertionDirectoryDropDownList.Items.Count != 0)
            {
                //string[] tabParent = null;
                Int64 idOldDirectory = 0;
                Int64 idInsertionSave = 0;
               
                string currentKey = Request.Form["idMyInsertion2"];

                if (!string.IsNullOrEmpty(currentKey))
                {
                    string[] tabParent = tabParent = currentKey.Split('_');
                    if (tabParent != null && tabParent.Length == 3 && tabParent[0] == "CKB")
                    {
                        idOldDirectory = Int64.Parse(tabParent[2]);
                        idInsertionSave = Int64.Parse(tabParent[1]);
                    }
                }
                if (idOldDirectory != 0)
                {
                    InsertionLevelDAL.MoveInsertionSave(idOldDirectory, Int64.Parse(moveInsertionDirectoryDropDownList.SelectedItem.Value), idInsertionSave, _webSession);
                    this.refreshDirectories();
                }
                else if (idInsertionSave == 0)
                {
                    //Erreur : none  saved insertion selected
                    Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(831, _webSession.SiteLanguage)));
                }
            }
            else
            {
                //Erreur : none directory
                Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(839, _webSession.SiteLanguage)));
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

    #region deleteInsertionImageButtonRollOverWebControl_Click
    /// <summary>
    /// Delet saved insertion
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">arguments</param>
    protected void deleteInsertionImageButtonRollOverWebControl_Click(object sender, EventArgs e)
    {
        try
        {

            Int64 idInsertionSave = 0;
            string currentKey = Request.Form["idMyInsertion3"];

            if (!string.IsNullOrEmpty(currentKey))
            {
                string[] tabParent = tabParent = currentKey.Split('_');
                if (tabParent != null && tabParent.Length == 3 && tabParent[0] == "CKB")
                    idInsertionSave = Int64.Parse(tabParent[1]);
            }
            if (idInsertionSave != 0)
            {

                if (InsertionLevelDAL.DropInsertionSave(idInsertionSave, _webSession))
                {
                    // Validation : confirmation of delete	
                    Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(286, _webSession.SiteLanguage)));
                    // Actualize la page
                    this.OnLoad(null);
                }
                else
                {
                    // Erreur : cant' suppress saved insertion
                    Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(830, _webSession.SiteLanguage)));
                }
            }
            else
            {
                // Erreur : please select a item
                Response.Write(TNS.AdExpress.Web.Functions.Script.Alert(GestionWeb.GetWebWord(831, _webSession.SiteLanguage)));
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

    #region Fonction pour rafraichir la liste des répertoires
    /// <summary>
    /// Rafraichi la liste des répertoires
    /// </summary>
    private void refreshDirectories()
    {

        try
        {
            // Rafresh the list
            DataSet ds = InsertionLevelDAL.GetGroupInsertionLevels(_webSession);

            insertionDirectoryDropDownList.DataSource = ds.Tables[0];
            insertionDirectoryDropDownList.DataTextField = ds.Tables[0].Columns["GROUP_INSERTION_SAVE"].ToString();
            insertionDirectoryDropDownList.DataValueField = ds.Tables[0].Columns["ID_GROUP_INSERTION_SAVE"].ToString();
            insertionDirectoryDropDownList.DataBind();

            renameInsertionDirectoryDropDownList.DataSource = ds.Tables[0];
            renameInsertionDirectoryDropDownList.DataTextField = ds.Tables[0].Columns["GROUP_INSERTION_SAVE"].ToString();
            renameInsertionDirectoryDropDownList.DataValueField = ds.Tables[0].Columns["ID_GROUP_INSERTION_SAVE"].ToString();
            renameInsertionDirectoryDropDownList.DataBind();

            moveInsertionDirectoryDropDownList.DataSource = ds.Tables[0];
            moveInsertionDirectoryDropDownList.DataTextField = ds.Tables[0].Columns["GROUP_INSERTION_SAVE"].ToString();
            moveInsertionDirectoryDropDownList.DataValueField = ds.Tables[0].Columns["ID_GROUP_INSERTION_SAVE"].ToString();
            moveInsertionDirectoryDropDownList.DataBind();

            //Liste des répertoires dans déplacer un univers
            listInsertionRepertories = GetSelectionTableHtmlUI(4, 500, "idMyInsertion2");
            listInsertionsToDelete = GetSelectionTableHtmlUI(6, 500, "idMyInsertion3");
            listInsertionToRename = GetSelectionTableHtmlUI(7, 500, "idMyInsertion1");
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

    #region Bouton ouvrir
    /// <summary>
    /// Gestion du bouton ouvrir
    /// </summary>
    /// <param name="sender">Objet qui lance l'évènement</param>
    /// <param name="e">Arguments</param>
    protected void openImageButtonRollOverWebControl_Click(object sender, System.EventArgs e)
    {
        try
        {
            _webSession.Source.Close();
            Response.Redirect("/Private/MyAdexpress/SearchSession.aspx?idSession=" + _webSession.IdSession + "");
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

    #region Bouton Personnaliser
    /// <summary>
    /// Gestion du bouton personnaliser
    /// </summary>
    /// <param name="sender">Objet qui lance l'évènement</param>
    /// <param name="e">Arguments</param>
    protected void personalizeImagebuttonrolloverwebcontrol_Click(object sender, System.EventArgs e)
    {
        try
        {
            _webSession.Source.Close();
            Response.Redirect("/Private/MyAdexpress/PersonnalizeSession.aspx?idSession=" + _webSession.IdSession + "");
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

    #region Bouton ouvir Pdf
    /// <summary>
    /// Gestion du bouton ouvir Pdf
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void pdfOpenImageButtonRollOverWebControl_Click(object sender, System.EventArgs e)
    {
        try
        {
            _webSession.Source.Close();
            Response.Redirect("/Private/MyAdexpress/PdfFiles.aspx?idSession=" + _webSession.IdSession + "");
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

    #region Bouton Alertes

    protected void alertOpenImageButtonRollOver_Click(object sender, System.EventArgs e)
    {
        try
        {
            _webSession.Source.Close();
            Response.Redirect("/Private/Alerts/ShowAlerts.aspx?idSession=" + _webSession.IdSession + "");
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

    #region Bouton Alertes Personnaliser
    /// <summary>
    /// Gestion du bouton Personnaliser
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void personalizeAlertesImagebuttonrolloverwebcontrol_Click(object sender, System.EventArgs e)
    {
        try
        {
            _webSession.Source.Close();
            Response.Redirect("/Private/Alerts/PersonalizeAlerts.aspx?idSession=" + _webSession.IdSession + "");
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

    /// <summary>
    /// Personnalize universe
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">Arguments</param>
    protected void universOpenImageButtonRollOverWebControl_Click(object sender, EventArgs e)
    {
        try
        {
            _webSession.Source.Close();
            Response.Redirect("/Private/Universe/PersonnalizeUniverse.aspx?idSession=" + _webSession.IdSession + "");
        }
        catch (System.Exception exc)
        {
            if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
            {
                this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
            }
        }
    }
}
