using TNS.AdExpress.Web.Core.Sessions;
using System.ComponentModel;
using System.Web.UI;
using System.Text;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.Exceptions;
using System;
using System.Web.UI.WebControls;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpressI.Classification.DAL;
using System.Reflection;
using System.Data;
using TNS.AdExpress.Domain.Web.Navigation;
using System.Collections;
using TNS.AdExpress.Domain.Level;
namespace TNS.AdExpress.Web.Controls.Selections.VP.Filter
{
    /// <summary>
    /// Affiche le résultat d'une alerte plan media
    /// </summary>
    [DefaultProperty("Text"),
      ToolboxData("<{0}:VpScheduleSelectionNodeWebControl runat=server></{0}:VpScheduleSelectionNodeWebControl>")]
    public class VpScheduleSelectionNodeWebControl : VpScheduleSelectionFilterBaseWebControl {

        #region Variables
        /// <summary>
        /// Generic Detail Level
        /// </summary>
        ArrayList _levelIds = null;
        /// <summary>
        /// Generic Detail Level Component Profile
        /// </summary>
        TNS.AdExpress.Constantes.Web.GenericDetailLevel.ComponentProfile _genericDetailLevelComponentProfile;
        #endregion

        #region Assessor
        /// <summary>
        /// Get / Set Generic Detail Level
        /// </summary>
        public ArrayList LevelIds {
            get { return _levelIds; }
            set { _levelIds = value; }
        }
        /// <summary>
        /// Get / Set Generic Detail Level Component Profile
        /// </summary>
        public TNS.AdExpress.Constantes.Web.GenericDetailLevel.ComponentProfile GenericDetailLevelComponentProfile {
            get { return _genericDetailLevelComponentProfile; }
            set { _genericDetailLevelComponentProfile = value; }
        }
        #endregion

        #region Property (Style)
        /// <summary>
        /// Get / Set CssClassLvl1
        /// </summary>
        public string CssClassLvl1 { get; set; }
        /// <summary>
        /// Get / Set CssClassLvl2
        /// </summary>
        public string CssClassLvl2 { get; set; }
        /// <summary>
        /// Get / Set CssClassLvl3
        /// </summary>
        public string CssClassLvl3 { get; set; }
        /// <summary>
        /// Get / Set CssClassLvl4
        /// </summary>
        public string CssClassLvl4 { get; set; }
        /// <summary>
        /// Get / Set CssClassContainerLvl1
        /// </summary>
        public string CssClassContainerLvl1 { get; set; }
        /// <summary>
        /// Get / Set CssClassContainerLvl2
        /// </summary>
        public string CssClassContainerLvl2 { get; set; }
        /// <summary>
        /// Get / Set CssClassContainerLvl3
        /// </summary>
        public string CssClassContainerLvl3 { get; set; }
        /// <summary>
        /// Get / Set CssClassContainerLvl4
        /// </summary>
        public string CssClassContainerLvl4 { get; set; }
        /// <summary>
        /// Get / Set CssClassDisplay
        /// </summary>
        public string CssClassDisplay { get; set; }
        /// <summary>
        /// Get / Set PathPictureDisplay
        /// </summary>
        public string PathPictureDisplay { get; set; }
        /// <summary>
        /// Get / Set PathPictureDisplayOver
        /// </summary>
        public string PathPictureDisplayOver { get; set; }
        #endregion

        #region AjaxEventScript
        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected override string GetAjaxEventScript() {
            StringBuilder js = new StringBuilder(1000);

            js.Append("\r\nvar isChanged_" + this.ID + " = false;");

            js.Append("\r\nfunction onCheck_" + this.ID + "(checkBoxElem, parentList){");
            js.Append("\r\n\tisChanged_" + this.ID + " = true;");
            js.Append("\r\n\tvar tab = parentList.split('_');");
            js.Append("\r\n\tvar parentId = ''");
            js.Append("\r\n\tif(parentList && parentList.length > 0){");

            js.Append("\r\n\t\tif(!checkBoxElem.checked){");
            js.Append("\r\n\t\t\tfor(var i=0; i<tab.length; i++){ ");
            js.Append("\r\n\t\t\t\tif(i>0) parentId += '_';");
            js.Append("\r\n\t\t\t\tparentId += tab[i];");
            js.Append("\r\n\t\t\t\tdocument.getElementById('" + this.ID + "_' + parentId).checked = false;");
            js.Append("\r\n\t\t\t}");
            js.Append("\r\n\t\t}");
            js.Append("\r\n\t}");
            js.Append("\r\n\treturn false;");
            js.Append("\r\n}\r\n");

            js.Append("\r\nfunction checkChild_" + this.ID + "(checkBoxElem){");
            js.Append("\r\n\tvar elems = document.getElementById('" + this.ID + "').getElementsByTagName('input');");
            js.Append("\r\n\tfor(var i=0; i<elems.length; i++){ ");
            js.Append("\r\n\tif(elems[i].id.substr(" + (this.ID + "_").Length + " ).match(\"^\"+checkBoxElem.id.substr(" + (this.ID + "_").Length + " )+\"_\")==checkBoxElem.id.substr(" + (this.ID + "_").Length + " )+\"_\"){");
            js.Append("\r\n\telems[i].checked = checkBoxElem.checked;");
            js.Append("\r\n\t}");
            js.Append("\r\n\t}");
            js.Append("\r\n\treturn false;");
            js.Append("\r\n}\r\n");

            js.Append("\r\nfunction getAllChild_" + this.ID + "(){");
            js.Append("\r\n\tvar tab = new Array();");
            js.Append("\r\n\tvar elems = document.getElementById('" + this.ID + "').getElementsByTagName('input');");
            js.Append("\r\n\tfor(var i=0; i<elems.length; i++){ ");
            js.Append("\r\n\tif(elems[i].id.substr(" + (this.ID + "_").Length + " ).split('_').length==" + _levelIds.Count + "){");
            js.Append("\r\n\ttab.push(elems[i].id.substr(" + (this.ID + "_").Length + " ));");
            js.Append("\r\n\t}");
            js.Append("\r\n\t}");
            js.Append("\r\n\treturn tab;");
            js.Append("\r\n}");
            return js.ToString();
        }
        #endregion   

        #region GetValuesSelectedMethodScriptContent
        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected override string GetValuesSelectedMethodScriptContent() {
            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n\tif(isChanged_" + this.ID + " == false) return null;");
            js.Append("\r\n\tvar tab = new Array();");
            js.Append("\r\n\tvar elems = document.getElementById('" + this.ID + "').getElementsByTagName('input');");
            js.Append("\r\n\tfor(var i=0; i<elems.length; i++){ ");
            //js.Append("\r\n\tif(elems[i].checked && elems[i].id.substr(" + (this.ID + "_").Length + " ).split('_').length==" + _levelIds.Count + "){");
            js.Append("\r\n\tif(elems[i].checked){");
            js.Append("\r\n\ttab.push(elems[i].id.substr(" + (this.ID + "_").Length + " ));");
            js.Append("\r\n\t}");
            js.Append("\r\n\t}");
            js.Append("\r\n\treturn tab;");
 
            return js.ToString();
        }
        #endregion

        #region GetValuesSelectedMethodScriptContent
        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected override string GetInitializeResultMethodContent() {
            return base.GetInitializeResultMethodContent() + "isChanged_" + this.ID + " = false;";
        }
        #endregion

        #region Enregistrement des paramètres de construction du résultat
        protected override string SetCurrentResultParametersScript() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\t obj.LevelIds = '" + string.Join(";", new List<DetailLevelItemInformation.Levels>((DetailLevelItemInformation.Levels[])_levelIds.ToArray(typeof(DetailLevelItemInformation.Levels))).ConvertAll<string>(p => p.ToString()).ToArray()) + "';");
            js.Append("\r\n\t obj.GenericDetailLevelComponentProfile = '" + GenericDetailLevelComponentProfile.ToString() + "'");
            return (base.SetCurrentResultParametersScript() + js.ToString());
        }
        protected override void LoadCurrentResultParameters(AjaxPro.JavaScriptObject o) {
            if (o.Contains("LevelIds")) {
                string[] strLevelIds = o["LevelIds"].Value.Replace("\"", "").Split(';');
                this.LevelIds = new ArrayList();
                foreach (string cString in strLevelIds) {
                    LevelIds.Add((DetailLevelItemInformation.Levels)Enum.Parse(typeof(DetailLevelItemInformation.Levels), cString));
                }
            }
            if (o.Contains("GenericDetailLevelComponentProfile")) {
                GenericDetailLevelComponentProfile = (Constantes.Web.GenericDetailLevel.ComponentProfile)Enum.Parse(typeof(Constantes.Web.GenericDetailLevel.ComponentProfile), o["GenericDetailLevelComponentProfile"].Value.Replace("\"", ""));
            }
        }
        #endregion

        #region Enregistrement des paramètres pour les styles
        protected override string SetCurrentStyleParametersScript() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\t obj.CssClassLvl1 = '" + CssClassLvl1 + "';");
            js.Append("\r\n\t obj.CssClassLvl2 = '" + CssClassLvl2 + "';");
            js.Append("\r\n\t obj.CssClassLvl3 = '" + CssClassLvl3 + "';");
            js.Append("\r\n\t obj.CssClassLvl4 = '" + CssClassLvl4 + "';");
            js.Append("\r\n\t obj.CssClassContainerLvl1 = '" + CssClassContainerLvl1 + "';");
            js.Append("\r\n\t obj.CssClassContainerLvl2 = '" + CssClassContainerLvl2 + "';");
            js.Append("\r\n\t obj.CssClassContainerLvl3 = '" + CssClassContainerLvl3 + "';");
            js.Append("\r\n\t obj.CssClassContainerLvl4 = '" + CssClassContainerLvl4 + "';");
            js.Append("\r\n\t obj.CssClassDisplay = '" + CssClassDisplay + "';");
            js.Append("\r\n\t obj.PathPictureDisplay = '" + PathPictureDisplay + "';");
            js.Append("\r\n\t obj.PathPictureDisplayOver = '" + PathPictureDisplayOver + "';");
            return (base.SetCurrentStyleParametersScript() + js.ToString());
        }

        protected override void LoadCurrentStyleParameters(AjaxPro.JavaScriptObject o) {
            if (o.Contains("CssClassLvl1")) {
                CssClassLvl1 = o["CssClassLvl1"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassLvl2")) {
                CssClassLvl2 = o["CssClassLvl2"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassLvl3")) {
                CssClassLvl3 = o["CssClassLvl3"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassLvl4")) {
                CssClassLvl4 = o["CssClassLvl4"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassContainerLvl1")) {
                CssClassContainerLvl1 = o["CssClassContainerLvl1"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassContainerLvl2")) {
                CssClassContainerLvl2 = o["CssClassContainerLvl2"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassContainerLvl3")) {
                CssClassContainerLvl3 = o["CssClassContainerLvl3"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassContainerLvl4")) {
                CssClassContainerLvl4 = o["CssClassContainerLvl4"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassDisplay")) {
                CssClassDisplay = o["CssClassDisplay"].Value.Replace("\"", "");
            }
            if (o.Contains("PathPictureDisplay")) {
                PathPictureDisplay = o["PathPictureDisplay"].Value.Replace("\"", "");
            }
            if (o.Contains("PathPictureDisplayOver")) {
                PathPictureDisplayOver = o["PathPictureDisplayOver"].Value.Replace("\"", "");
            }
        }
        #endregion

        #region GetAjaxHTML
        /// <summary>
        /// Get  loading HTML  
        /// </summary>
        /// <returns></returns>
        protected override string GetAjaxHTML() {
            StringBuilder html = new StringBuilder(1000);
            GenericDetailLevel genericDetailLevel = new GenericDetailLevel(_levelIds);
            List<Int64> selectionIdList = new List<long>();
            string checkboxChecked = string.Empty;
            string selection = string.Empty;
            DataSet ds = GetData(genericDetailLevel);
            int i = 0;
            int previousLvl = 0;
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0) {
                List<Int64> levelList = new List<Int64>();
                Int64 cValue;
                List<Int64> levelParentList = new List<Int64>();
                List<Int64> levelOldList = new List<Int64>();
                foreach (DataRow cRow in ds.Tables[0].Rows) {
                    levelList = new List<Int64>();
                    levelParentList = new List<Int64>();
                    for (i = 0; i < genericDetailLevel.Levels.Count; i++) {
                        cValue = Int64.Parse(cRow[genericDetailLevel[i + 1].DataBaseIdField].ToString());
                        levelList.Add(cValue);

                        if (levelOldList == null || levelOldList.Count <= 0 || levelOldList[i] != levelList[i]) {
                            string cClass = string.Empty;
                            string cClassContainer = string.Empty;
                            switch (i) {
                                case 0: cClass = CssClassLvl1; cClassContainer = CssClassContainerLvl1; break;
                                case 1: cClass = CssClassLvl2; cClassContainer = CssClassContainerLvl2; break;
                                case 2: cClass = CssClassLvl3; cClassContainer = CssClassContainerLvl3; break;
                                case 3: cClass = CssClassLvl4; cClassContainer = CssClassContainerLvl4; break;
                            }
                            if (_genericDetailLevelComponentProfile == TNS.AdExpress.Constantes.Web.GenericDetailLevel.ComponentProfile.media)
                                selection = _webSession.GetSelection(_webSession.SelectionUniversMedia, GetAccessType(i + 1, genericDetailLevel));
                            else
                                selection = _webSession.GetSelection(_webSession.SelectionUniversProduct, GetAccessType(i + 1, genericDetailLevel));
                            if (!string.IsNullOrEmpty(selection))
                                selectionIdList = new List<Int64>(new List<string>(selection.Split(',')).ConvertAll<Int64>(p => Int64.Parse(p)));
                            else
                                selectionIdList = new List<Int64>();
                            if (selectionIdList.Contains(cValue))
                                checkboxChecked = " checked ";
                            else
                                checkboxChecked = string.Empty;
                            string cId = string.Format("{0}", string.Join("_", levelList.ConvertAll<string>(p => p.ToString()).ToArray()));

                            if (previousLvl >= 0 && previousLvl >= i)
                                for(int j=i; j<previousLvl; j++)
                                    html.Append("</td></tr></table></div></div>");

                            if (previousLvl >= 0 && i != genericDetailLevel.Levels.Count - 1)
                                html.AppendFormat("<div id=\"div_{1}_{0}\" width=\"100%\" class=\"{2}\">", cId, this.ID, cClass);

                            
                            html.AppendFormat("<div id=\"div_header_{1}_{0}\" width=\"100%\" >", cId, this.ID);
                            html.AppendFormat("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" height=\"100%\" class=\"{0}\">", cClass);
                            html.Append("<tr><td>");
                            html.AppendFormat("<input type=\"checkbox\" value=\"{1}_{0}\" id=\"{1}_{0}\" onclick=\"javascript:{2} onCheck_" + this.ID + "(this, '{3}'); \" " + checkboxChecked + "/>"
                                , cId
                                , this.ID
                                , ((i < genericDetailLevel.Levels.Count - 1) ? "checkChild_" + this.ID + "(this);" : string.Empty)
                                , ((i > 0) ? string.Join("_", levelParentList.ConvertAll<string>(p => p.ToString()).ToArray()) : string.Empty));
                            html.AppendFormat("<label for=\"{2}_{1}\">{0}</label>"
                                , cRow[genericDetailLevel[i + 1].DataBaseField].ToString()
                                , cId
                                , this.ID
                                );
                            if (previousLvl >= 0 && i != genericDetailLevel.Levels.Count - 1) {
                                string onclickFunction = string.Empty;
                                onclickFunction = string.Format("displayContainer_{1}_{0}(); function displayContainer_{1}_{0}() {2}if(document.getElementById('div_container_{1}_{0}').style.display=='none'){2}document.getElementById('div_container_{1}_{0}').style.display='';{3} else {2}document.getElementById('div_container_{1}_{0}').style.display='none';{3}{3}"
                                    , cId, this.ID, "{", "}");

                                html.Append("&nbsp;<img src=\"" + PathPictureDisplay + "\" class=\"" + CssClassDisplay + "\" onmouseout=\"this.src='" + PathPictureDisplay + "'\" onmouseover=\"this.src='" + PathPictureDisplayOver + "'\" onclick=\"javascript:return " + onclickFunction + "\"/>");
                            }
                            html.Append("</td></tr>");
                            html.Append("</table>");
                            html.Append("</div>");
                            if (previousLvl >= 0 && i != genericDetailLevel.Levels.Count - 1) {
                                html.AppendFormat("<div id=\"div_container_{1}_{0}\" width=\"100%\">", cId, this.ID);
                                html.AppendFormat("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" class=\"{0}\"><tr><td>", cClassContainer);
                            }

                            previousLvl = i;
                        }
                        levelParentList.Add(cValue);
                    }
                    levelOldList = levelList;
                }
                if (previousLvl >= 0 && previousLvl >= i)
                    for (int j = i; j < previousLvl; j++)
                        html.Append("</td></tr></table></div></div>");
            }

            return (html.ToString());
        }
        #endregion

        #region GetData
        /// <summary>
        /// Get Data 
        /// </summary>
        /// <returns></returns>
        protected DataSet GetData(GenericDetailLevel genericDetailLevel) {
            StringBuilder html = new StringBuilder(1000);
            DataSet ds;
            object[] parameters = new object[3];
            parameters[0] = _webSession;
            parameters[1] = genericDetailLevel;
            parameters[2] = string.Empty;
            ClassificationDAL classificationDAL = (ClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification].AssemblyName, WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

            if (_genericDetailLevelComponentProfile == TNS.AdExpress.Constantes.Web.GenericDetailLevel.ComponentProfile.product)
                ds = classificationDAL.GetDetailProduct();
            else
                ds = classificationDAL.GetDetailMedia();

            return ds;
        }
        #endregion

        #region GetAccessType
        /// <summary>
        /// Get Access Type
        /// </summary>
        /// <returns>Access Type</returns>
        TNS.AdExpress.Constantes.Customer.Right.type GetAccessType(int lvl, GenericDetailLevel genericDetailLevel) {
            TNS.AdExpress.Constantes.Customer.Right.type accesType;
            switch (genericDetailLevel[lvl].Id) {
                case DetailLevelItemInformation.Levels.vpBrand:
                        accesType = TNS.AdExpress.Constantes.Customer.Right.type.vpBrandAccess;
                    break;
                case DetailLevelItemInformation.Levels.vpCircuit:
                        accesType = TNS.AdExpress.Constantes.Customer.Right.type.circuitAccess;
                    break;
                case DetailLevelItemInformation.Levels.vpProduct:
                    accesType = TNS.AdExpress.Constantes.Customer.Right.type.vpProductAccess;
                    break;
                case DetailLevelItemInformation.Levels.vpSegment:
                    accesType = TNS.AdExpress.Constantes.Customer.Right.type.vpSegmentAccess;
                    break;
                case DetailLevelItemInformation.Levels.vpSubSegment:
                    accesType = TNS.AdExpress.Constantes.Customer.Right.type.vpSubSegmentAccess;
                    break;
                default:
                    throw new BaseException("Impossible to retrieve the current level");
            }



            return accesType;
        }
        #endregion

    }
}

