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
namespace TNS.AdExpress.Web.Controls.Selections.VP
{
    /// <summary>
    /// Affiche le résultat d'une alerte plan media
    /// </summary>
    [DefaultProperty("Text"),
      ToolboxData("<{0}:VpScheduleSelectionNodeWebControl runat=server></{0}:VpScheduleSelectionNodeWebControl>")]
    public class VpScheduleSelectionNodeWebControl : VpScheduleAjaxSelectionBaseWebControl {

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
        #endregion

        #region AjaxEventScript
        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected override string GetAjaxEventScript() {
            StringBuilder js = new StringBuilder(1000);

            js.Append("\r\nfunction setNode_" + this.ID + "(){");
            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".SetNode('" + this._webSession.IdSession + "', getValues_" + this.ID + "(), resultParameters_" + this.ID + ",styleParameters_" + this.ID + ",setNode_" + this.ID + "_callback);");
            js.Append("\r\n}");

            js.Append("\r\nfunction setNode_" + this.ID + "_callback(res){");
            js.Append("if (res.error != null && res.error.Message) { alert(res.error.Message); }");
            js.Append("else {");
            js.Append(ValidationMethod);
            js.Append("}");
            js.Append("\r\n}\r\n");

            js.Append("\r\nfunction checkChild_" + this.ID + "(checkBoxElem){");
            js.Append("var elems = document.getElementById('" + this.ID + "').getElementsByTagName('input');");
            js.Append("for(var i=0; i<elems.length; i++){ ");
            js.Append("if(checkBoxElem.id.match(\"^\"+elems[i].id.substr(0," + this.ID.Length + " ))==elems[i].id.substr(0," + this.ID.Length + " )){");
            js.Append("elems[i].checked = checkBoxElem.checked;");
            js.Append("}");
            js.Append("}");
            js.Append("}");

            js.Append("\r\nfunction getValues_" + this.ID + "(){");
            js.Append("var tab = new Array();");
            js.Append("var elems = document.getElementById('"+this.ID+"').getElementsByTagName('input');");
            js.Append("for(var i=0; i<elems.length; i++){ ");
            js.Append("if(elems[i].checked && elems[i].id.substr(0," + this.ID.Length + " ).split('_').length==" + _levelIds.Count + "){");
            js.Append("tab.push(elems[i].id.substr(0," + this.ID.Length + " ));");
            js.Append("}");
            js.Append("}");
            js.Append("return tab;");
            js.Append("}");

            


            return js.ToString();
        }

        /// <summary>
        /// Get Validation Javascript Method
        /// </summary>
        /// <returns>Validation Javascript Method</returns>
        protected override string GetValidationJavascriptContent() {
            return base.GetValidationJavascriptContent() + " setNode_" + this.ID + "(); ";
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

        }
        #endregion

        #region SetNode
        /// <summary>
        /// Set Date in WebSession
        /// </summary>
        /// <param name="o">Result parameters (session Id, theme...)</param>
        /// <returns>Code HTML</returns>
        [AjaxPro.AjaxMethod]
        public void SetNode(string idSession, string[] childSelected, AjaxPro.JavaScriptObject resultParameters, AjaxPro.JavaScriptObject styleParameters) {
            try {
                _webSession = (WebSession)WebSession.Load(idSession);

                //if (_genericDetailLevelComponentProfile == TNS.AdExpress.Constantes.Web.GenericDetailLevel.ComponentProfile.product)
                    //_webSession.CurrentUniversProduct.;
                //else
                    //_webSession.CurrentUniversMedia;

                _webSession.Save();                
            }
            catch (System.Exception err) {
                throw new Exception (OnAjaxMethodError(err, _webSession), err);
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

            DataSet ds = GetData(genericDetailLevel);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0) {
                List<Int64> levelList = new List<Int64>();
                List<Int64> levelOldList = new List<Int64>();
                foreach (DataRow cRow in ds.Tables[0].Rows) {
                    levelList = new List<Int64>();
                    for (int i = 0; i < genericDetailLevel.Levels.Count; i++) {
                        levelList.Add(Int64.Parse(cRow[genericDetailLevel[i+1].DataBaseIdField].ToString()));

                        if (levelOldList == null || levelOldList.Count <= 0 || levelOldList[i] != levelList[i]) {
                            string cClass = string.Empty;
                            switch (i) {
                                case 0: cClass = CssClassLvl1; break;
                                case 1: cClass = CssClassLvl2; break;
                                case 2: cClass = CssClassLvl3; break;
                                case 3: cClass = CssClassLvl4; break;
                            }

                            string cId = string.Format("{0}", string.Join("_", levelList.ConvertAll<string>(p => p.ToString()).ToArray()));

                            html.AppendFormat("<div id=\"div_{1}_{0}\" width=\"100%\" class=\"{2}\">", cId, this.ID, cClass);
                            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" height=\"100%\">");
                            html.Append("<tr><td>");
                            html.AppendFormat("<input type=\"checkbox\" value=\"{1}_{0}\" id=\"{1}_{0}\" onclick=\"{2}\"/>"
                                , cId
                                , this.ID
                                , ((i < genericDetailLevel.Levels.Count - 1) ? "javascript:checkChild_{1}(this);" : string.Empty));
                            html.AppendFormat("<label for=\"{2}_{1}\">{0}</label>"
                                , cRow[genericDetailLevel[i+1].DataBaseField].ToString()
                                , cId
                                , this.ID);
                            html.Append("</td></tr>");
                            html.Append("</table>");
                            html.Append("</div>");
                        }
                    }
                    levelOldList = levelList;
                }
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

        #region WriteHtmlLevelLine
        private string WriteHtmlLevelLine(DetailLevelItemInformation detailLevelItemInformation, DataRow cRow, List<Int64>  cLevelParentValue) {
            StringBuilder html = new StringBuilder(1000);

            

            return (html.ToString());
        }
        #endregion
    }
}

