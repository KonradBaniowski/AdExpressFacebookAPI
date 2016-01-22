using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Web.Core.Sessions;
using System.Data;
using TNS.AdExpress.Classification;
using TNS.Classification.Universe;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.DataAccess.Selections.Products;

namespace TNS.AdExpress.Web.Controls.Selections
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:DropDownCheckBoxListWebControl runat=server></{0}:DropDownCheckBoxListWebControl>")]
    public class DropDownCheckBoxListWebControl :   CompositeControl
    {
        /// <summary>
        /// Contrôle chkList.
        /// </summary>      
        protected CheckBoxList _chkList;

        /// <summary>
        ///  Contrôle tex box.
        /// </summary>
        protected TextBox _txtSelectedItem;

        /// <summary>
        /// DataTable
        /// </summary>
        private DataTable _dt;

        /// <summary>
        /// Session
        /// </summary>
        protected WebSession _webSession;
        /// <summary>
        /// Label Css Class
        /// </summary>
        private string _backgroundColor = "#ffffff";
        /// <summary>
        /// Label Code
        /// </summary>
        private int _textCode = 1847;
     
        /// <summary>
        /// Label Css Class
        /// </summary>
        private string _labelCssCass = "txtGris11Bold";
        /// <summary>
        /// Container Height
        /// </summary>
        protected string _containerHeight = "0";

        /// <summary>
        /// Max Items Not Scrollable
        /// </summary>
        protected int _maxItemsNotScrollable = 0;

        /// <summary>
        /// Get / Set Max Items Not Scrollable
        /// </summary>
        public int MaxItemsNotScrollable
        {
            get { return _maxItemsNotScrollable; }
            set { _maxItemsNotScrollable = value; }
        }

        /// <summary>
        /// Get / Set Container Height
        /// </summary>
        public string ContainerHeight
        {
            get { return _containerHeight; }
            set { _containerHeight = value; }
        }
        /// <summary>
        /// Get / Set Client Session
        /// </summary>
        public WebSession Session
        {
            get { return _webSession; }
            set { _webSession = value; }
        }
        /// <summary>
        /// Get / Set BackGround Color
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("#ffffff")]
        public string BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }
        /// <summary>
        /// Get / Set Label Code
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(2288)]
        public int TextCode
        {
            get { return _textCode; }
            set { _textCode = value; }
        }
       
        /// <summary>
        /// Get / Set Label Css Class
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("txtGris11Bold")]
        public string LabelCssClass
        {
            get { return _labelCssCass; }
            set { _labelCssCass = value; }
        }


        protected override void RecreateChildControls()
        {
            EnsureChildControls();
        }


        /// <summary>
        /// Create child controls
        /// </summary>
        protected override void CreateChildControls()
        {
            Controls.Clear();
            base.CreateChildControls();

            _chkList = new CheckBoxList();
            _chkList.ID = "_chkList";
            _chkList.EnableViewState = true;
            _chkList.CssClass = "multidropdown";
            _chkList.AppendDataBoundItems = true;
            _chkList.Attributes.Add("style", "display:none");


           
            _dt = ProductClassificationListDataAccess.SectorList(_webSession).Tables[0]; 

            var dtLine = new DataTable();
            dtLine.Columns.Add("id_sector");
            dtLine.Columns.Add("sector");

            var row = dtLine.NewRow();
            row["id_sector"] = 0;
            row["sector"] = GestionWeb.GetWebWord(856, _webSession.SiteLanguage);
            dtLine.Rows.Add(row);

            dtLine.Merge(_dt);

            _chkList.DataSource = dtLine;
            _chkList.DataTextField = "sector";
            _chkList.DataValueField = "id_sector";
            _chkList.DataBind();
          

            _txtSelectedItem = new TextBox
                                   {ID = "_txtSelectedItem", CssClass = "select-box", ReadOnly = true};
            _txtSelectedItem.Attributes.Add("unselectable", "on");
            _txtSelectedItem.Text = GestionWeb.GetWebWord(1535,_webSession.SiteLanguage);
            _txtSelectedItem.EnableViewState = true;


            Controls.Add(_txtSelectedItem);
            Controls.Add(_chkList);
                

        }

        /// <summary>
        /// Get /Set Selected sector ids
        /// </summary>
        public string SelectedSectorsIds
        {
            get
            {
                var s = ViewState["SelectedSectorsIds"];

                return s == null ? "" : s.ToString();
            }
            set { ViewState["SelectedSectorsIds"] = value; }
        }

        /// <summary>
        ///  Contrôle tex box.
        /// </summary>
        public TextBox TxtSelectedItem
        {
            get { return _txtSelectedItem; }
        }

        /// <summary>
        /// Contrôle chkList.
        /// </summary>      
        public CheckBoxList ChkList
        {
            get { return _chkList; }
        }

        /// <summary>
        /// On PreRender
        /// </summary>
        /// <param name="e">Event Args</param>
        protected override void OnPreRender(EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count>0)
                SelectedSectorsIds = _webSession.PrincipalProductUniverses[0].GetLevel(TNSClassificationLevels.SECTOR, AccessType.includes);
                SelectItems(_chkList.Items, SelectedSectorsIds);
            }

            string itemsIds = "";


            foreach (ListItem it in _chkList.Items)
            {
                if (it.Selected && it.Value != "0")
                {
                    itemsIds += it.Value + ",";
                }
            }
            if (!string.IsNullOrEmpty(itemsIds)) itemsIds = itemsIds.Substring(0, itemsIds.Length - 1);
            SelectedSectorsIds = itemsIds;

            if (Page.IsPostBack)
            {
                try
                {
                    // Save SectorID in websession
                    if (!string.IsNullOrEmpty(itemsIds))
                    {


                        var universeDictionary = new Dictionary<int, AdExpressUniverse>();
                        if (_webSession != null && _webSession.PrincipalProductUniverses != null)
                            _webSession.PrincipalProductUniverses.Clear();

                        var adExpressUniverse = new AdExpressUniverse(Dimension.product);
                        var nGroup = new NomenclatureElementsGroup(0, AccessType.includes);
                        nGroup.AddItems(TNSClassificationLevels.SECTOR, itemsIds);
                        adExpressUniverse.AddGroup(0, nGroup);
                        universeDictionary.Add(0, adExpressUniverse);
                        if (_webSession != null) _webSession.PrincipalProductUniverses = universeDictionary;
                    }
                    else
                    {
                       _webSession.PrincipalProductUniverses = new Dictionary<int, AdExpressUniverse>();
                    }
                    if (_webSession != null) _webSession.Save();
                }
                catch (Exception er)
                {
                    throw new DropDownCheckBoxListWebControlException("Impossible to get sector(s) selected", er);
                }
            }

            if (!Page.ClientScript.IsClientScriptBlockRegistered("DropDownScript_" + ClientID))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "DropDownScript_" + ClientID, DropDownScript());
            }
            base.OnPreRender(e);
        }


        /// <summary>
        /// Génère le rendu du contenu du contrôle via le writer spécifié. Cette méthode est principalement utilisée par des développeurs de contrôles.
        /// </summary>
        /// <param name="output"><see cref="T:System.Web.UI.HtmlTextWriter"/> qui représente le flux de sortie utilisé pour rendre le contenu HTML sur le client. 
        ///             </param>
        protected override void RenderContents(HtmlTextWriter output)
        {

            output.WriteLine("<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" bgcolor=\"{0}\">", _backgroundColor);
            output.WriteLine("<tr><td class=\"{0}\">", _labelCssCass);
            output.Write(GestionWeb.GetWebWord(_textCode, _webSession.SiteLanguage));
            output.Write(" :</td></tr>");
            output.WriteLine("<tr><td>");
            _txtSelectedItem.RenderControl(output);
            output.Write("<div id =\"chkList__listContainer\"  ");
            if (_dt != null && _dt.Rows.Count > _maxItemsNotScrollable)
            {
                output.Write("style= height:{0};", _containerHeight);
            }
            output.Write(" class=\"listContainer\">");
            _chkList.RenderControl(output);
            output.Write("</div>");
            output.Write("</td></tr>");
            output.WriteLine("</table>");

         

        }

        /// <summary>
        /// Drop Down Script
        /// </summary>
        /// <returns></returns>
        protected string DropDownScript()
        {
            var st = new StringBuilder();
            st.Append("\n\t <script type=\"text/javascript\" >");
            st.Append("\n\t jQuery(document).ready(function($) {");
            st.Append("\n\t $(\"#" + _txtSelectedItem.ClientID + "\").click(function() {");
            st.Append("\n\t\t var offset = $(this).position();");
            st.Append("\n\t\t var pTop = offset.top + 20;");
            st.Append("\n\t\t var pLeft = offset.left;");

             st.Append("\n\t\t $(\"#" + _chkList.ClientID + "\").css(\"top\", pTop).css(\"left\", pLeft).toggle();");

            //Hide Container Div
            st.Append("\n\t if(document.getElementById('chkList__listContainer')!=null ) {");
            st.Append("\n\t\t if(document.getElementById('chkList__listContainer').style.display=='block' ) document.getElementById('chkList__listContainer').style.display = 'none';");
            st.Append("\n\t\t else document.getElementById('chkList__listContainer').style.display = 'block';");
            st.Append("\n\t }");


            st.Append("\n\t\t return false;");
            st.Append("\n\t });");

            st.Append("\n\t $(\"#" + _chkList.ClientID + "\").click(function(e) {");
            st.Append("\n\t\t e.stopPropagation();");
            st.Append("\n\t  });");

            st.Append("\n\t $(\"#" + _chkList.ClientID + " #" + _chkList.ClientID + "_0\").click(function() {");
            st.Append("\n\t\t if ($(this).attr(\"checked\"))");
            st.Append("\n\t\t\t $(\"#" + _chkList.ClientID + " input[type=checkbox]\").attr(\"checked\", \"checked\");");
            st.Append("\n\t\t else ");
            st.Append("\n\t\t\t $(\"#" + _chkList.ClientID + " input[type=checkbox]\").attr(\"checked\", \"\");");
            st.Append("\n\t  });");

            st.Append("\n\t $(document).click(function(e) {");
            // log(e.target.id);
            st.Append("\n\t\t $(\"#" + _chkList.ClientID + "\").hide();");

            //hide container
            st.Append("\n\t\t $(\"#chkList__listContainer\").hide();");

            st.Append("});");
            st.Append("\n\t});");
            st.Append("</script>");

            return st.ToString();
        }

        /// <summary>
        /// Selects all items of the ListItemCollection that are in the IdList.
        /// </summary>
        /// <param name="items">Items</param>
        /// <param name="idList">1,2,5,8</param>
        public void SelectItems(ListItemCollection items, string idList)
        {
            if (string.IsNullOrEmpty(idList))
                return;

            foreach (ListItem item in items)
            {
                if (idList.Contains(item.Value))
                    item.Selected = true;
                item.Attributes.Add("style","");
            }
        }

    }
}
