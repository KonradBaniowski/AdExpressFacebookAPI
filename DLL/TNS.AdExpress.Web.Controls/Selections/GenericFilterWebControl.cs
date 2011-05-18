using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using TNS.AdExpress.Web.Core.Selection;
using System.ComponentModel;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using System.Web.UI;

namespace TNS.AdExpress.Web.Controls.Selections {
    /// <summary>
    /// Web control used to display a list of filters and allow the user to do a multiple selection
    /// </summary>
    public class GenericFilterWebControl : WebControl {

        #region Variables
        /// <summary>
        /// List of filter items
        /// </summary>
        private List<FilterItem> _filterItems;
        /// <summary>
        /// Slected Items
        /// </summary>
        private string _selectedFilterItems = string.Empty;
        ///<summary>
        /// Session du client
        /// </summary>
        ///  <label>_customerWebSession</label>
        protected WebSession _customerWebSession = null;
        /// <summary>
        /// Couleur de fond du composant
        /// </summary>
        private string _backgroundColor = "#ffffff";
        /// <summary>
        /// Classe Css pour le libellé de la liste des niveaux par défaut
        /// </summary>
        private string _cssDefaultListLabel = "txtGris11Bold";
        /// <summary>
        /// Classe Css pour le titre de la section personnalisée
        /// </summary>
        protected string _cssCustomSectionTitle = "txtViolet11Bold";
        #endregion

        #region Accessors
        /// <summary>
        /// Set list of filter items
        /// </summary>
        [Bindable(false)]
        public List<FilterItem> FilterItems {
            set { _filterItems = value; }
        }
        /// <summary>
        /// Set selected items
        /// </summary>
        [Bindable(false)]
        public string SelectedFilterItems {
            set { _selectedFilterItems = value; }
        }
        /// <summary>
        /// Définit la session du client
        /// </summary>
        [Bindable(false)]
        public WebSession CustomerWebSession {
            set { _customerWebSession = value; }
        }
        /// <summary>
        /// Obtient ou définit la couleur de fond du composant
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue("#ffffff"),
        Description("Couleur de fond du composant")]
        public string BackGroundColor {
            get { return (_backgroundColor); }
            set { _backgroundColor = value; }
        }
        /// <summary>
        /// Obtient ou définit la classe Css pour le libellé de la liste des niveaux par défaut
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue("txtGris11Bold"),
        Description("Classe Css pour le libellé de la liste des niveaux par défaut")]
        public string CssDefaultListLabel {
            get { return (_cssDefaultListLabel); }
            set { _cssDefaultListLabel = value; }
        }
        /// <summary>
        /// Obtient ou définit la classe Css pour le titre de la section personnalisée
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue("txtViolet11Bold"),
        Description("Classe Css pour le titre de la section personnalisée")]
        public string CssCustomSectionTitle {
            get { return (_cssCustomSectionTitle); }
            set { _cssCustomSectionTitle = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filterItems">List of filter items</param>
        public GenericFilterWebControl() {
        }
        #endregion

        #region Render
        /// <summary>
        /// Render builder
        /// </summary>
        /// <returns>Html code</returns>
        public string Render() {

            StringBuilder sb = new StringBuilder(5000);
            const int ELEMENT_NB = 2;
            int count = 0;
            string checkedText = "";
            string[] selectedFilterItems = null;

            if (_selectedFilterItems != null && _selectedFilterItems.Length > 0) selectedFilterItems = _selectedFilterItems.Split(',');
                 
            // table de personnalisation
            sb.Append("<table class=\"backgroundGenericFilterWebControl genericFilterWebControlBorder\" cellSpacing=\"0\" cellPadding=\"0\" width=\"" + this.Width + "\" border=\"0\">");
            sb.Append("<tr onclick=\"DivDisplayer('genericFilterContent');\" class=\"cursorHand\">");
            //Titre de la section
            sb.Append("<td class=\"" + _cssCustomSectionTitle + "\">&nbsp;" + GestionWeb.GetWebWord(2155, _customerWebSession.SiteLanguage) + "&nbsp;</td>");
            // Image d'ouverture de la section
            sb.Append("<td align=\"right\" class=\"arrowBackGroundGenericFilterWebControl\"></td>");
            sb.Append("</tr>");
            sb.Append("</table>");

            // Section
            sb.Append("\r\n<div id=\"genericFilterContent\" class=\"GenericFilterWebControlSelectionSection\" style=\"DISPLAY: none; WIDTH: " + this.Width + "px;\">");
            sb.Append("\r\n<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" ID=\"Section\">");

            // Filter items list
            foreach (FilterItem filterItem in _filterItems) {
                if (count == 0) sb.Append("<tr>");
                sb.Append("<td width="+(this.Width.Value/2)+">");
                if (ContainValue(selectedFilterItems, filterItem.Id.ToString())) checkedText = "checked";
                else checkedText = "";
                sb.AppendFormat("<input id=\"checkbox_{0}_{1}\" type=\"checkbox\" " + checkedText + " onclick=\"if(this.checked){{AddFilter_{0}({1});}}else {{RemoveFilter_{0}({1});}};\" ><label for=\"label_{0}_{1}\" class=\"txtViolet11Bold\">{2}</label>", this.ID, filterItem.Id, filterItem.Label);
                sb.Append("</td>");
                count++;
                if (count == ELEMENT_NB){
                    sb.Append("</tr>");
                    count = 0;
                }
            }

            if (count > 0) sb.Append("</tr>");

            sb.Append("</table>");
            sb.Append("<input id=\"idGenericFilter\" type=\"hidden\" name=\"_genericFilter\" value=\""+_selectedFilterItems+"\">");

            return sb.ToString();

        }
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="output">output</param>
        protected override void Render(HtmlTextWriter output) {
            RenderFiltersScripts(output);
            output.Write(Render());
        }
        #endregion

        #region Scripts
        /// <summary>
        /// Render Filters Scripts
        /// </summary>
        /// <param name="output">output</param>
        protected void RenderFiltersScripts(HtmlTextWriter output) {
            
            output.WriteLine("\r\n  <SCRIPT language=javascript>\r\n<!--                                      ");

            output.WriteLine("\r\n  var activeBannersFormatList = new Array();                                ");

            string[] selectedFilterItems = null;
            int index = 0;

            if (_selectedFilterItems != null && _selectedFilterItems.Length > 0) {
                
                selectedFilterItems = _selectedFilterItems.Split(',');

                foreach (string s in selectedFilterItems) {
                    output.WriteLine("\r\n  activeBannersFormatList[" + (index++) + "] = " + s + ";           ");
                }
            }

            #region Find Filter
            output.WriteLine("\r\n\tfunction FindValue(tab, value){                                           ");
            output.WriteLine("\r\n\t\t var i = -1;                                                            ");
            output.WriteLine("\r\n\t\t for (var j=0;j<tab.length;j=j+1){                                      ");
            output.WriteLine("\r\n\t\t\t if(tab[j] == value){                                                 ");
            output.WriteLine("\r\n\t\t\t\t return j;                                                          ");
            output.WriteLine("\r\n\t\t\t }                                                                    ");
            output.WriteLine("\r\n\t\t }                                                                      ");
            output.WriteLine("\r\n\t\t return(i);                                                             ");
            output.WriteLine("\r\n\t}                                                                         ");
            #endregion

            #region AddFilter
            output.WriteLine("\r\n  function AddFilter_" + this.ID + "(filterValue){                          ");
            output.WriteLine("\r\n    activeBannersFormatList.push(filterValue);                              ");
            output.WriteLine("\r\n    initHiddenGenericFilter()                                               ");
            output.WriteLine("\r\n  }                                                                         ");
            #endregion

            #region RemoveFilter
            output.WriteLine("\r\n  function RemoveFilter_" + this.ID + "(filterValue){                       ");
            output.WriteLine("\r\n      var fIndex = FindValue(activeBannersFormatList, filterValue);         ");
            output.WriteLine("\r\n      if (fIndex > -1){                                                     ");
            output.WriteLine("\r\n          activeBannersFormatList.splice(fIndex,1);                         ");
            output.WriteLine("\r\n      }                                                                     ");
            output.WriteLine("\r\n    initHiddenGenericFilter()                                               ");
            output.WriteLine("\r\n  }                                                                         ");
            #endregion

            #region initHiddenGenericFilter
            output.WriteLine("\r\n  function initHiddenGenericFilter(){                                       ");
            output.WriteLine("\r\n    var hiddenObj = document.getElementById('idGenericFilter');             ");
            output.WriteLine("\r\n    if(activeBannersFormatList.length ==0 ) hiddenObj.value='';             ");
            output.WriteLine("\r\n    for(var i = 0; i < activeBannersFormatList.length; i++)                 ");
            output.WriteLine("\r\n        if (i == 0) hiddenObj.value =  activeBannersFormatList[i];          ");
            output.WriteLine("\r\n        else        hiddenObj.value +=  ',' + activeBannersFormatList[i];   ");
            output.WriteLine("\r\n  }                                                                         ");
            #endregion

            output.WriteLine("\r\n-->\r\n</SCRIPT>                                                            ");

        }
        #endregion

        #region Contain Value
        /// <summary>
        /// Contain Value
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="value">Value to check</param>
        /// <returns></returns>
        private bool ContainValue(string[] list, string value) {

            if (list != null) {
                foreach (string s in list)
                    if (s == value) return true;
            }

            return false;
        }
        #endregion

    }
}
