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
        /// Generic Detail Level Component Profile
        /// </summary>
        public TNS.AdExpress.Constantes.Web.GenericDetailLevel.ComponentProfile GenericDetailLevelComponentProfile {
            set { _genericDetailLevelComponentProfile = value; }
        }
        #endregion

        #region AjaxEventScript
        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected override string GetAjaxEventScript() {
            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\nfunction setNode_" + this.ID + "(){");
            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".SetNode('" + this._webSession.IdSession + "', document.getElementById('dateSelection_" + this.ID + "').options[document.getElementById('dateSelection_" + this.ID + "').selectedIndex].value, resultParameters_" + this.ID + ",styleParameters_" + this.ID + ",setNode_" + this.ID + "_callback);");
            js.Append("\r\n}");

            js.Append("\r\nfunction setNode_" + this.ID + "_callback(res){");
            js.Append("if (res.error != null && res.error.Message) { alert(res.error.Message); }");
            js.Append("else {");
            js.Append(ValidationMethod);
            js.Append("}");

            js.Append("\r\n}\r\n");
            return js.ToString();
        }
        #endregion   

        #region Enregistrement des paramètres de construction du résultat
        protected override string SetCurrentResultParametersScript() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\t obj.LevelIds = '" + string.Join(";", new List<DetailLevelItemInformation.Levels>((DetailLevelItemInformation.Levels[])_levelIds.ToArray(typeof(DetailLevelItemInformation.Levels))).ConvertAll<string>(p => p.ToString()).ToArray()) + "';");
            return (js.ToString());
        }
        protected override void LoadCurrentResultParameters(AjaxPro.JavaScriptObject o) {
            if (o.Contains("LevelIds")) {
                string[] strLevelIds = o["LevelIds"].Value.Replace("\"", "").Split(';');
                this.LevelIds = new ArrayList();
                foreach (string cString in strLevelIds) {
                    LevelIds.Add((DetailLevelItemInformation.Levels)Enum.Parse(typeof(DetailLevelItemInformation.Levels), cString));
                }
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
        public void SetNode(string idSession, string periodType, AjaxPro.JavaScriptObject resultParameters, AjaxPro.JavaScriptObject styleParameters) {

            try {
                _webSession = (WebSession)WebSession.Load(idSession);

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
            DataSet ds = GetData();

            return (html.ToString());
        }
        #endregion

        #region GetData
        /// <summary>
        /// Get Data 
        /// </summary>
        /// <returns></returns>
        protected DataSet GetData() {
            StringBuilder html = new StringBuilder(1000);
            DataSet ds;
            object[] parameters = new object[3];
            parameters[0] = _webSession;
            parameters[1] = new GenericDetailLevel(_levelIds);
            parameters[2] = string.Empty;
            ClassificationDAL classificationDAL = (ClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification].AssemblyName, WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

            if (_genericDetailLevelComponentProfile == TNS.AdExpress.Constantes.Web.GenericDetailLevel.ComponentProfile.product)
                ds = classificationDAL.GetDetailProduct();
            else
                ds = classificationDAL.GetDetailMedia();

            return ds;
        }
        #endregion


    }
}

