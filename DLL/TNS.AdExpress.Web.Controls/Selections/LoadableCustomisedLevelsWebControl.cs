using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Controls.Buttons;

namespace TNS.AdExpress.Web.Controls.Selections
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:LoadableCustomisedLevelsWebControl runat=server></{0}:LoadableCustomisedLevelsWebControl>")]
    public class LoadableCustomisedLevelsWebControl : WebControl
    {

        #region Variables
        /// <summary>
        /// Session
        /// </summary>
        protected WebSession _webSession = null;
        /// <summary>
        /// bouton RollOver
        /// </summary>
       // protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl buttonRollOver;
        /// <summary>
        /// Id media type
        /// </summary>
        protected long _idVehicle = long.MinValue;
        /// <summary>
        /// List of customised  level
        /// </summary>
        protected List<int> _columnItemList = null;
        /// <summary>
        /// Liste customised detail level 
        /// </summary>
        protected List<int> _allowedDetailItemList = null;
        /// <summary>
        /// Valider la sélection
        /// </summary>
        public ImageButtonRollOverWebControl _buttonOk;

        /// <summary>
        /// Theme name
        /// </summary>
        protected string _themeName = string.Empty;
        /// <summary>
        /// id Insertion Save
        /// </summary>
        protected long _idInsertionSave = long.MinValue;
        #endregion

        #region Properties
        /// <summary>
        ///Get Session
        /// </summary>
        public WebSession CustomerWebSession
        {
            get { return _webSession; }
            set { _webSession = value; }
        }
        /// <summary>
        ///Get Id Vehicle
        /// </summary>
        public long IdVehicle
        {
            get { return _idVehicle; }
            set { _idVehicle = value; }
        }
        /// <summary>
        ///Get List of customised  level
        /// </summary>
        public List<int> ColumnItemList
        {
            get { return _columnItemList; }
            set { _columnItemList = value; }
        }
        /// <summary>
        ///Get  Liste customised detail level 
        /// </summary>
        public List<int> AllowedDetailItemList
        {
            get { return _allowedDetailItemList; }
            set { _allowedDetailItemList = value; }
        }
        /// <summary>
        ///Get/Set Theme Name
        /// </summary>
        public string ThemeName
        {
            get { return  _themeName; }
            set { _themeName = value; }
        }
        /// <summary>
        ///Get/Set Id Insertion Save
        /// </summary>
        public long IdInsertionSave
        {
            get { return _idInsertionSave; }
            set { _idInsertionSave = value; }
        }
        #endregion

        #region Events

        #region Constructor
        public LoadableCustomisedLevelsWebControl():base()
        {
            this.EnableViewState = true;
            _buttonOk = new ImageButtonRollOverWebControl();
          }
        #endregion
        #region OnPreRender
        /// <summary>
        /// Rendu de la page
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e)
        {

            base.OnPreRender(e);
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("SetSelectedInsertionSave")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SetSelectedInsertionSave", SetSelectedInsertionSave());	 

        }
        #endregion

        	
		#region Init
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            #region bouton OK
         
            _buttonOk.ImageUrl = "/App_Themes/" + _themeName + "/Images/Culture/button/charger_up.gif";
            _buttonOk.RollOverImageUrl = "/App_Themes/" + _themeName + "/Images/Culture/button/charger_down.gif";
            Controls.Add(_buttonOk);
            #endregion
           
        }
        #endregion

        #region Render
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="output">Le writer HTML vers lequel écrire</param>
        protected override void Render(HtmlTextWriter output)
        {
            bool hasData = false;
            output.Write("<table class=\"violetBorder\" cellSpacing=\"0\" cellPadding=\"0\" width=\"200\" border=\"0\">");
            output.Write("<tr class=\"cursorHand\" onclick=\"showHideContent6('listAdvertiser');\">");
            output.Write("<td class=\"txtViolet11Bold\" >&nbsp;" + GestionWeb.GetWebWord(1897, _webSession.SiteLanguage) + "&nbsp;</td>");
            output.Write("<td align=\"right\" class=\"arrowBackGround\">");
            output.Write("</td>");
            output.Write("</tr>");
            output.Write("</table>");

            output.Write("<div id=\"listAdvertiserContent6\" class=\"violetBorderWithoutTop\" style=\"DISPLAY: none; WIDTH: 620px;\">");
            output.Write("<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" align=\"center\" class=\"backGroundWhite\" border=\"0\">");
            output.Write("<tr>");
            output.Write("<td width=\"199\"><IMG height=\"1\" src=\"/App_Themes/" + _themeName + "/images/Common/pixel.gif\"></td>");
            output.Write("<td class=\"violetBorderTop\" width=\"421\"><IMG height=\"1\" src=\"/App_Themes/" + _themeName + "/images/Common/pixel.gif\"></td>");
            output.Write("</tr>");
            output.Write("<tr>");
            output.Write("<td class=\"txtGris11Bold\" style=\"PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 2px; PADDING-TOP: 0px\" colSpan=\"2\">&nbsp;</td>");
            output.Write("</tr>");
            output.Write("<tr>");
            output.Write("  <td style=\"PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 2px; PADDING-TOP: 0px\" colSpan=\"2\">");

          
            DataSet ds = InsertionLevelDAL.GetData(_webSession, _idVehicle);
            DataTable dt = GetAllowedSavedLevels(ds);

            if (dt != null &&  dt.Rows.Count > 0)
            {
                hasData = true;
              
                System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
                Int64 idParent;
                Int64 idParentOld = long.MinValue;
                string textParent;
                string textParentOld;
                int start = -1;
                int compteur = 0;
                int _width = 600;
                int valueTable = 4;

                foreach (DataRow currentRow in dt.Rows)
                {
                    idParent = Convert.ToInt32( currentRow["ID_GROUP_INSERTION_SAVE"]);
                    textParent = currentRow["GROUP_INSERTION_SAVE"].ToString();

                    //First
                    //Group saved levels label
                    if (idParent != idParentOld && start != 0)
                    {
                        t.Append("<table class=\"violetBorder txtViolet11Bold\" cellpadding=0 cellspacing=0   width=" + _width + ">");
                        t.Append("<tr onClick=\"showHideContent" + valueTable + "('" + idParent + "');\" class=\"cursorHand\">");
                        t.Append("<td align=\"left\" height=\"10\" valign=\"middle\">");
                        t.Append("<label ID=\"" + currentRow["ID_GROUP_INSERTION_SAVE"] + valueTable + "\">&nbsp;");
                        t.Append("" + textParent + "");
                        t.Append("</label>");
                        t.Append("</td>");
                        t.Append("<td class=\"arrowBackGround\"></td>");
                        t.Append("</tr>");
                        t.Append("</table>");
                        t.Append("<div id=\"" + idParent + "Content" + valueTable + "\" style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\" >");
                        t.Append("<table class=\"violetBorderWithoutTop paleVioletBackGround\" width=" + _width + ">");
                        idParentOld = idParent;
                        textParentOld = textParent;
                        start = 0;
                        compteur = 0;
                    }
                    else if (idParent != idParentOld)
                    {
                        t.Append("</table>");
                        t.Append("</div>");
                        t.Append("<table class=\"violetBorderWithoutTop txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=" + _width + ">");
                        t.Append("<tr onClick=\"showHideContent" + valueTable + "('" + idParent + "');\" class=\"cursorHand\">");
                        t.Append("<td align=\"left\" height=\"10\" valign=\"middle\">");
                        t.Append("<label ID=\"" + currentRow["ID_GROUP_INSERTION_SAVE"] + valueTable + "\">&nbsp;");
                        t.Append("" + textParent + "");
                        t.Append("</label>");
                        t.Append("</td>");
                        t.Append("<td class=\"arrowBackGround\"></td>");
                        t.Append("</tr>");
                        t.Append("</table>");
                        t.Append("<div id=\"" + idParent + "Content" + valueTable + "\"  style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\">");
                        t.Append("<table class=\"violetBorderWithoutTop paleVioletBackGround\" width=" + _width + ">");                     
                        idParentOld = idParent;
                        textParentOld = textParent;
                        compteur = 0;
                    }

                    //Saved levels label
                    string radioButtonState = "";
                    if (currentRow["ID_INSERTION_SAVE"] != System.DBNull.Value)
                    {
                        if(_idInsertionSave == Convert.ToInt64(currentRow["ID_INSERTION_SAVE"] ))radioButtonState = " checked ";
                        if (compteur == 0)
                        {
                            t.Append("<tr><td class=\"txtViolet10\" width=50%>");
                            t.Append("<input type=\"radio\" "+radioButtonState+" ID=\"insertionSave_" + currentRow["ID_INSERTION_SAVE"] + "\" onClick=\"SetSelectedInsertionSave(" + currentRow["ID_INSERTION_SAVE"] + ");\" value=\"" + currentRow["ID_INSERTION_SAVE"] + "\" name=\"insertionSave\">" + currentRow["INSERTION_SAVE"].ToString() + "<br>");
                            t.Append("</td>");
                            compteur = 1;
                        }
                        else
                        {
                            t.Append("<td class=\"txtViolet10\" width=50%>");
                            t.Append("<input type=\"radio\" " + radioButtonState + " ID=\"insertionSave_" + currentRow["ID_INSERTION_SAVE"] + "\" onClick=\"SetSelectedInsertionSave(" + currentRow["ID_INSERTION_SAVE"] + ");\"  value=\"" + currentRow["ID_INSERTION_SAVE"] + "\" name=\"insertionSave\">" + currentRow["INSERTION_SAVE"].ToString() + "<br>");
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
                    radioButtonState = "";
                }
                if (compteur != 0)
                {
                    t.Append("</tr>");
                }
                t.Append("</table>");
                t.Append("</div>");
                output.Write(t.ToString());
            }
            else
            {   //No saved levels
                output.Write("<tr><td class=\"txtViolet12Bold\" colspan=3 align=middle>");
                output.Write("&nbsp;&nbsp;&nbsp;" + GestionWeb.GetWebWord(285, _webSession.SiteLanguage));
                output.Write("</td></tr>");
            }
            output.Write("<input id=\"idMySavedLevels\" type=\"hidden\" name=\"idMySavedLevels\">");
            output.Write("</td>");
            output.Write("</tr>");
          

            if (hasData)
            {
                output.Write("<tr>");
                output.Write("<td style=\"PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 5px; PADDING-TOP: 2px; TEXT-ALIGN: right\" colSpan=\"2\">");
                //output.Write("<a id=\"loadImageButtonRollOverWebControl\" onmouseover=\"rolloverServerControl_display('loadImageButtonRollOverWebControl_img',loadImageButtonRollOverWebControl_img_over);\" onmouseout=\"rolloverServerControl_display('loadImageButtonRollOverWebControl_img',loadImageButtonRollOverWebControl_img_out);\" href=\"javascript:__doPostBack('loadImageButtonRollOverWebControl','')\"><img name=\"loadImageButtonRollOverWebControl_img\" src=\"/App_Themes/" + themeName + "/Images/Culture/button/charger_up.gif\" border=\"0\" /></a>");
               _buttonOk.RenderControl(output);
                output.Write("</td>");
                output.Write("</tr>");
            }
           
           
            output.Write("</table>");
            output.Write("</div>");

        }
        #endregion

        #endregion


        protected DataTable GetAllowedSavedLevels(DataSet ds)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID_GROUP_INSERTION_SAVE", typeof(long));
            dt.Columns.Add("GROUP_INSERTION_SAVE", typeof(string));
            dt.Columns.Add("ID_INSERTION_SAVE", typeof(long));
            dt.Columns.Add("INSERTION_SAVE", typeof(string));
            bool isValidLevels = true;

            foreach(DataRow dr in ds.Tables[0].Rows){
               
                List<int> detailLevels = new List<string>(dr["LEVE_LIST"].ToString().Split(',')).ConvertAll(i => int.Parse(i));
                List<int> columnsLevels = new List<string>(dr["COLUMN_LIST"].ToString().Split(',')).ConvertAll(i => int.Parse(i));

                //Check allowed detail levels 
                for (int j = 0; j< detailLevels.Count; j++)
                {
                    if (!_allowedDetailItemList.Contains(detailLevels[j]))
                    {
                        isValidLevels = false;
                        break;
                    }
                }
                //Check allowed columns levels 
                if (isValidLevels)
                {
                    for (int k = 0; k < columnsLevels.Count; k++)
                    {
                        if (!_columnItemList.Contains(columnsLevels[k]))
                        {
                            isValidLevels = false;
                            break;
                        }
                    }
                }
                if (isValidLevels) dt.Rows.Add(Convert.ToInt64(dr["ID_GROUP_INSERTION_SAVE"]), dr["GROUP_INSERTION_SAVE"].ToString(), Convert.ToInt64(dr["ID_INSERTION_SAVE"]), dr["INSERTION_SAVE"].ToString());
                isValidLevels = true;
            }

            return dt;
        }

        /// <summary>
        /// Set Selected Insertion Save
        /// </summary>
        /// <returns></returns>
        protected string SetSelectedInsertionSave()
        {
            StringBuilder t = new StringBuilder(300);
            t.Append("\n <!—gestion de l’univers sélectionné -->\n");
            t.Append("\n <SCRIPT language=JavaScript>\n");
            t.Append("\t function SetSelectedInsertionSave(val){\n");
            t.Append("\t\t document.getElementById('idMySavedLevels').value=val;\n");
            t.Append("\t }\n");
            t.Append("</script>\n");
            return t.ToString();
        }


    }
}
