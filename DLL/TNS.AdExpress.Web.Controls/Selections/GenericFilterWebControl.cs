﻿using System;
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
        /// <summary>
        /// Nb Element by Column
        /// </summary>
        protected int _nbElemByColumn = 2;
        /// <summary>
        /// Test Label Id
        /// </summary>
        protected int _textLabelId;
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
        /// <summary>
        /// Set Nb Elem By Column
        /// </summary>
        [Bindable(false)]
        public int NbElemByColumn {
            set { _nbElemByColumn = value; }
        }
        /// <summary>
        /// Set Text Label Id
        /// </summary>
        [Bindable(false)]
        public int TextLabelId {
            set { _textLabelId = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public GenericFilterWebControl() {
        }
        #endregion

        #region OnPreRender
        /// <summary>
        /// Pre Render
        /// </summary>
        /// <param name="e">Event Argument</param>
        protected override void OnPreRender(EventArgs e) {
            base.OnPreRender(e);
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) 
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DivDisplayer", TNS.AdExpress.Web.Functions.Script.DivDisplayer());
        }
        #endregion

        #region Render
        /// <summary>
        /// Render builder
        /// </summary>
        /// <returns>Html code</returns>
        public string Render() {

            StringBuilder sb = new StringBuilder(5000);
            int count = 0;
            string checkedText = "";
            string disabledText = string.Empty;
            string[] selectedFilterItems = null;

            if (!string.IsNullOrEmpty(_selectedFilterItems)) selectedFilterItems = _selectedFilterItems.Split(',');
                 
            // table de personnalisation
            sb.Append("<table class=\"backgroundGenericFilterWebControl genericFilterWebControlBorder\" cellSpacing=\"0\" cellPadding=\"0\" width=\"" + this.Width + "\" border=\"0\">");
            sb.Append("<tr onclick=\"DivDisplayer('genericFilterContent_"+this.ID+"');\" class=\"cursorHand\">");
            //Titre de la section
            sb.Append("<td class=\"" + _cssCustomSectionTitle + "\">&nbsp;" + GestionWeb.GetWebWord(_textLabelId, _customerWebSession.SiteLanguage) + "&nbsp;</td>");
            // Image d'ouverture de la section
            sb.Append("<td align=\"right\" class=\"arrowBackGroundGenericFilterWebControl\"></td>");
            sb.Append("</tr>");
            sb.Append("</table>");

            // Section
            sb.Append("\r\n<div id=\"genericFilterContent_"+this.ID+"\" class=\"GenericFilterWebControlSelectionSection\" style=\"DISPLAY: none; WIDTH: " + this.Width + "px;\">");
            sb.Append("\r\n<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" ID=\"Section_"+this.ID+"\">");

            // Filter items list
            foreach (FilterItem filterItem in _filterItems) {
                if (count == 0) sb.Append("<tr>");
                sb.Append("<td width=" + (this.Width.Value / _nbElemByColumn) + ">");
                if (ContainValue(selectedFilterItems, filterItem.Id.ToString())) checkedText = "checked";
                else checkedText = "";
                if (!filterItem.IsEnable) disabledText = "disabled=\"disabled\"";
                else disabledText = string.Empty;
                sb.AppendFormat("<input id=\"checkbox_{0}_{1}\" type=\"checkbox\" " + checkedText + " " + disabledText + " onclick=\"if(this.checked){{AddFilter_{0}({1});}}else {{RemoveFilter_{0}({1});}};\" ><label for=\"checkbox_{0}_{1}\" class=\"txtViolet11Bold\" style=\"display:inline-block;white-space:nowrap;\">{2}</label>", this.ID, filterItem.Id, filterItem.Label.ToUpper());
                sb.Append("</td>");
                count++;
                if (count == _nbElemByColumn) {
                    sb.Append("</tr>");
                    count = 0;
                }
            }

            if (count > 0) sb.Append("</tr>");

            sb.Append("</table>");
            sb.Append("<input id=\"idGenericFilter_"+this.ID+"\" type=\"hidden\" name=\"_genericFilter_"+this.ID+"\" value=\""+_selectedFilterItems+"\">");

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

            output.WriteLine("\r\n  var filterList_"+this.ID+" = new Array();                                ");

            string[] selectedFilterItems = null;
            int index = 0;

            if (_selectedFilterItems != null && _selectedFilterItems.Length > 0) {
                
                selectedFilterItems = _selectedFilterItems.Split(',');

                foreach (string s in selectedFilterItems) {
                    output.WriteLine("\r\n  filterList_"+this.ID+"[" + (index++) + "] = " + s + ";           ");
                }
            }

            #region Find Filter
            output.WriteLine("\r\n\tfunction FindValue_"+this.ID+"(tab, value){                                           ");
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
            output.WriteLine("\r\n    filterList_"+this.ID+".push(filterValue);                              ");
            output.WriteLine("\r\n    initHiddenGenericFilter_"+this.ID+"()                                               ");
            output.WriteLine("\r\n  }                                                                         ");
            #endregion

            #region RemoveFilter
            output.WriteLine("\r\n  function RemoveFilter_" + this.ID + "(filterValue){                       ");
            output.WriteLine("\r\n      var fIndex = FindValue_"+this.ID+"(filterList_"+this.ID+", filterValue);         ");
            output.WriteLine("\r\n      if (fIndex > -1){                                                     ");
            output.WriteLine("\r\n          filterList_"+this.ID+".splice(fIndex,1);                         ");
            output.WriteLine("\r\n      }                                                                     ");
            output.WriteLine("\r\n    initHiddenGenericFilter_"+this.ID+"()                                               ");
            output.WriteLine("\r\n  }                                                                         ");
            #endregion

            #region initHiddenGenericFilter
            output.WriteLine("\r\n  function initHiddenGenericFilter_"+this.ID+"(){                                       ");
            output.WriteLine("\r\n    var hiddenObj = document.getElementById('idGenericFilter_"+this.ID+"');             ");
            output.WriteLine("\r\n    if(filterList_"+this.ID+".length ==0 ) hiddenObj.value='';             ");
            output.WriteLine("\r\n    for(var i = 0; i < filterList_"+this.ID+".length; i++)                 ");
            output.WriteLine("\r\n        if (i == 0) hiddenObj.value =  filterList_"+this.ID+"[i];          ");
            output.WriteLine("\r\n        else        hiddenObj.value +=  ',' + filterList_"+this.ID+"[i];   ");
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