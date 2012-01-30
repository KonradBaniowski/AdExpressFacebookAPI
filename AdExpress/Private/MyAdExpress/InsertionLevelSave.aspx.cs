using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Translation;

using TNS.AdExpress.Web.Core.DataAccess.ClassificationList;



    /// <summary>
    /// Save insertion report's customised levels 
    /// </summary>
    public partial class Private_MyAdExpress_InsertionLevelSave : TNS.AdExpress.Web.UI.PrivateWebPage
    {
        #region Variables
        /// <summary>
        /// Columns Levels Id
        /// </summary>
        protected string _columnsLevelsId = "";
        /// <summary>
        /// Columns Levels Id
        /// </summary>
        protected string _generciDetailLevelsId = "";

        protected long _idVehicle = long.MinValue;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructeur 
        /// </summary>
        public Private_MyAdExpress_InsertionLevelSave()
            : base()
        {
        }
        #endregion

        #region Page Load
        /// <summary>
        /// Page loading
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">Arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _columnsLevelsId = Page.Request.QueryString.Get("cl");
                _generciDetailLevelsId = Page.Request.QueryString.Get("dl");
                _idVehicle = long.Parse(Page.Request.QueryString.Get("idV"));

                #region List of insertion levels directories
                DataSet dsDir = TNS.AdExpress.Web.Core.DataAccess.ClassificationList.InsertionLevelDAL.GetGroupInsertionLevels(_webSession);
                directoryDropDownList.Items.Clear();
                directoryDropDownList.DataSource = dsDir.Tables[0];
                directoryDropDownList.DataTextField = dsDir.Tables[0].Columns[1].ToString();
                directoryDropDownList.DataValueField = dsDir.Tables[0].Columns[0].ToString();
                directoryDropDownList.DataBind();

                DataSet ds = TNS.AdExpress.Web.Core.DataAccess.ClassificationList.InsertionLevelDAL.GetData(_webSession, _idVehicle);

                #endregion

                directoryDropDownList.EnableViewState = false;
                insertionLevelsSavedDropDownList.EnableViewState = false;

                if (!this.Page.ClientScript.IsClientScriptBlockRegistered("InsertionsJavaScriptFunctions"))
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "InsertionsJavaScriptFunctions", InsertionsJavaScriptFunctions(ds, dsDir));
                directoryDropDownList.Attributes.Add("onChange", "fillInsertionLevelsSaved(this.options[this.selectedIndex].value);");
                insertionLevelsSavedDropDownList.Attributes.Add("onChange", "selectInsertionLevelsSave();");
                insertionLevelsSavedTextBox.Attributes.Add("onKeypress", "deleteInsertionLevelsSave();");
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

        #region oKImageButtonRollOverWebControl_Click
        /// <summary>
        /// Managing  button OK event
        /// </summary>
        /// <param name="sender">Objet source</param>
        /// <param name="e">Arguments</param>
        protected void oKImageButtonRollOverWebControl_Click(object sender, EventArgs e)
        {
            try
            {
                if (directoryDropDownList.Items.Count != 0)
                {
                    string idSelectedInsertionLevelsSaved = Request.Form.GetValues("insertionLevelsSavedDropDownList")[0];
                    string idSelectedDirectory = Request.Form.GetValues("directoryDropDownList")[0];

                    if (string.IsNullOrEmpty(_columnsLevelsId) || _columnsLevelsId.Equals("-1") || string.IsNullOrEmpty(_generciDetailLevelsId))
                    {
                        // Error : No item selected
                        Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(878, _webSession.SiteLanguage)));
                    }
                    else
                    {
                        string insetionLevelsName = WebFunctions.CheckedText.CheckedAccentText(insertionLevelsSavedTextBox.Text);

                        if (string.IsNullOrEmpty(insetionLevelsName) && !idSelectedInsertionLevelsSaved.Equals("0"))
                        {
                            if (InsertionLevelDAL.UpdateInsertionLevels(Int64.Parse(idSelectedInsertionLevelsSaved), _webSession, _idVehicle, _generciDetailLevelsId, _columnsLevelsId))
                            {
                                // Validation : success to save request	
                                _webSession.Source.Close();
                                Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(826, _webSession.SiteLanguage)));
                            }
                            else
                            {
                                // Erreur : unable to save request	
                                _webSession.Source.Close();
                                Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(825, _webSession.SiteLanguage)));
                            }
                        }
                        else if (!string.IsNullOrEmpty(insetionLevelsName) && insetionLevelsName.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT)
                        {
                            if (!InsertionLevelDAL.IsInsertionLevelsExist(_webSession, insetionLevelsName))
                            {
                                if (!string.IsNullOrEmpty(idSelectedDirectory) && InsertionLevelDAL.SaveInsertionLeves(Int64.Parse(idSelectedDirectory), insetionLevelsName, _idVehicle, _generciDetailLevelsId, _columnsLevelsId, _webSession))
                                {
                                    // Validation : confirm insertion levels save
                                    _webSession.Source.Close();
                                    Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(826, _webSession.SiteLanguage)));
                                }
                                else
                                {
                                    // Error :can't save result
                                    _webSession.Source.Close();
                                    Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(825, _webSession.SiteLanguage)));
                                }
                            }
                            else
                            {
                                // Error : result already exists
                                _webSession.Source.Close();
                                Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(824, _webSession.SiteLanguage)));
                            }
                        }
                        else if (string.IsNullOrEmpty(insetionLevelsName))
                        {
                            // Erreur : Filed is empty
                            _webSession.Source.Close();
                            Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(837, _webSession.SiteLanguage)));
                        }
                        else
                        {
                            // Erreur : text lenght is superior  to 50 
                            _webSession.Source.Close();
                            Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(823, _webSession.SiteLanguage)));
                        }
                    }

                }
                else
                {
                    // Erreur : Impossible to save, no directory saved
                    Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(711, _webSession.SiteLanguage)));
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

        #region cancelImageButtonRollOverWebControl_Click
        /// <summary>
        ///  Managing  button Cancel event
        /// </summary>
        /// <param name="sender">Objet source</param>
        /// <param name="e">Arguments</param>
        protected void cancelImageButtonRollOverWebControl_Click(object sender, EventArgs e)
        {
            Response.Write("<script language=javascript>");
            Response.Write("	window.close();");
            Response.Write("</script>");

        }
        #endregion

        #region Javascript
        /// <summary>
        /// Generates javascript used for backup levels of detail insertion
        /// </summary>
        /// <param name="ds">List of saved insertion detail level</param>
        /// <param name="dsDir">Liste of directories</param>
        /// <returns>Code Javascript</returns>
        private string InsertionsJavaScriptFunctions(DataSet ds, DataSet dsDir)
        {


            StringBuilder script = new StringBuilder(2000);
            int i = 0, k = 0;

            script.Append("<script language=\"JavaScript\">");
            script.Append("\r\n  var directories = new Array(); ");
            insertionLevelsSavedDropDownList.Items.Clear();
            insertionLevelsSavedDropDownList.Items.Insert(k, new ListItem("------------------", "0"));
            k++;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                if (dr["ID_INSERTION_SAVE"] != System.DBNull.Value)
                {
                    script.Append("\r\n directories[" + i + "] = new Array();");
                    script.Append("\r\n directories[" + i + "][\"ID_GROUP_INSERTION_SAVE\"] = \"" + dr["ID_GROUP_INSERTION_SAVE"].ToString() + "\"; ");
                    script.Append("\r\n directories[" + i + "][\"ID_INSERTION_SAVE\"] = \"" + dr["ID_INSERTION_SAVE"].ToString() + "\"; ");
                    script.Append("\r\n directories[" + i + "][\"INSERTION_SAVE\"] = \"" + dr["INSERTION_SAVE"].ToString() + "\"; ");
                    i++;
                }
                if (dsDir.Tables[0].Rows.Count > 0 && Int64.Parse(dsDir.Tables[0].Rows[0]["ID_GROUP_INSERTION_SAVE"].ToString()) == Int64.Parse(dr["ID_GROUP_INSERTION_SAVE"].ToString())
                    && dr["ID_INSERTION_SAVE"] != System.DBNull.Value)
                {
                    insertionLevelsSavedDropDownList.Items.Insert(k, new ListItem(dr["INSERTION_SAVE"].ToString(), dr["ID_INSERTION_SAVE"].ToString()));
                    k++;
                }

            }

            script.Append("\r\n function verif()");
            script.Append("\r\n {");
            script.Append("\r\n\t if (document.layers)");
            script.Append("\r\n\t {");
            script.Append("\r\n\t theForm = document.forms.Form1;");
            script.Append("\r\n\t }");
            script.Append("\r\n\t else");
            script.Append("\r\n\t {");
            script.Append("\r\n\t theForm = document.Form1;");
            script.Append("\r\n\t }");
            script.Append("\r\n}");

            script.Append("\r\n function selectInsertionLevelsSave()");
            script.Append("\r\n {");
            script.Append("\r\n\t theForm.insertionLevelsSavedTextBox.value=\"\";");
            script.Append("\r\n }");

            script.Append("\r\n function deleteInsertionLevelsSave()");
            script.Append("\r\n {");
            script.Append("\r\n\t theForm.insertionLevelsSavedDropDownList.options.selectedIndex = 0;");
            script.Append("\r\n }");

            script.Append("\r\n function fillInsertionLevelsSaved(codeDirectory)");
            script.Append("\r\n {");
            script.Append("\r\n\t verif();");
            script.Append("\r\n\t if(codeDirectory>0)");
            script.Append("\r\n\t {");
            script.Append("\r\n\t\t theForm.insertionLevelsSavedDropDownList.options.length = 0;");
            script.Append("\r\n\t\t\t\t theForm.insertionLevelsSavedDropDownList.options[theForm.insertionLevelsSavedDropDownList.options.length] = new Option(\"------------------\",\"0\");");
            script.Append("\r\n\t\t for (var i=0;i<directories.length;i++)");
            script.Append("\r\n\t\t {");
            script.Append("\r\n\t\t\t if(i==0)");
            script.Append("\r\n\t\t\t {");
            script.Append("\r\n\t\t\t }");
            script.Append("\r\n\t\t\t if(parseInt(directories[i][\"ID_GROUP_INSERTION_SAVE\"]) == parseInt(codeDirectory))");
            script.Append("\r\n\t\t\t {");
            script.Append("\r\n\t\t\t\t theForm.insertionLevelsSavedDropDownList.options[theForm.insertionLevelsSavedDropDownList.options.length] = new Option(directories[i][\"INSERTION_SAVE\"],directories[i][\"ID_INSERTION_SAVE\"]);");
            script.Append("\r\n\t\t\t }");
            script.Append("\r\n\t\t }");
            script.Append("\r\n\t\t theForm.insertionLevelsSavedDropDownList.options.selectedIndex = 0;");
            script.Append("\r\n\t }");
            script.Append("\r\n\t else");
            script.Append("\r\n\t {");
            script.Append("\r\n\t\t theForm.insertionLevelsSavedDropDownList.options[theForm.insertionLevelsSavedDropDownList.options.length] = new Option(\"------------------\",\"0\");");
            script.Append("\r\n\t }");

            script.Append("\r\n }");
            script.Append("\r\n </script>");

            return (script.ToString());
        }
        #endregion
    }
