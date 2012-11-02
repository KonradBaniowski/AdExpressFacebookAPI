using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Controls.Selections.Rolex.Filter;
using TNS.AdExpress.Web.Controls.Selections.VP.Filter;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Rolex;


namespace TNS.AdExpress.Web.Controls.Results.Rolex
{
    /// <summary>
    /// Rolex Result Filter WebControl
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:RolexResultFilterWebControl runat=server></{0}:RolexResultFilterWebControl>")]
    public class RolexScheduleResultFilterWebControl : AjaxResultBaseWebControl
    {
        #region Variables
        /// <summary>
        /// Filter Result Web Control List
        /// </summary>
        Dictionary<Int64, RolexScheduleSelectionFilterBaseWebControl> _filterResultWebControlList = new Dictionary<long, RolexScheduleSelectionFilterBaseWebControl>();
        #endregion

        #region GetJavascript

        #region Menu
        /// <summary>
        /// Get Javascript Menu
        /// </summary>
        /// <returns>Javascript Menu</returns>
        protected string GetJavascriptMenu()
        {
            StringBuilder js = new StringBuilder();
            js.Append("\r\n<script language=\"javascript\">\r\n<!--");

            foreach (RolexScheduleSelectionFilterBaseWebControl cRolexSelectionFilterBaseWebControl in _filterResultWebControlList.Values)
            {
                js.Append("\r\nvar isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " = false;");
            }

            js.Append("\r\nfunction valid_menu_" + this.ID + "(controlId){");
            foreach (RolexScheduleSelectionFilterBaseWebControl cRolexSelectionFilterBaseWebControl in _filterResultWebControlList.Values)
            {
                js.Append("\r\n\tif(controlId == '" + cRolexSelectionFilterBaseWebControl.ID + "'){");
                js.Append("\r\n\t" + cRolexSelectionFilterBaseWebControl.DisplayMethod + "(true);");
                js.Append("\r\n\tif(isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + "  == false){");
                js.Append("\r\n\tisLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " = true;");
                js.Append("\r\n\t" + cRolexSelectionFilterBaseWebControl.ValidationMethodName + "();");
                js.Append("\r\n\t}");
                js.Append("\r\n\tdocument.getElementById('menu_" + cRolexSelectionFilterBaseWebControl.ID + "').className = '" + CssClassOptionMenuSelected + "';");
                js.Append("\r\n\t} else {");
                js.Append("\r\n\t" + cRolexSelectionFilterBaseWebControl.DisplayMethod + "(false);");
                js.Append("\r\n\tdocument.getElementById('menu_" + cRolexSelectionFilterBaseWebControl.ID + "').className = '';");
                js.Append("\r\n\t}");

            }
            js.Append("\r\n}");

            js.Append("\r\n-->\r\n</script>");
            return js.ToString();
        }
        #endregion

        #region Valid Data
        /// <summary>
        /// Get Javascript Valid Data
        /// </summary>
        /// <returns>Javascript Valid Data</returns>
        protected string GetJavascriptValidData()
        {
            StringBuilder js = new StringBuilder();
            js.Append("\r\n<script language=\"javascript\">\r\n<!--");

            RolexScheduleSelectionFilterBaseWebControl cRolexSelectionFilterBaseWebControlDate = null;
            int i = 0;

            #region Valid Data
            js.Append("\r\nfunction validData_" + this.ID + "(){");

            js.Append("\r\n\tvar dateParameters = null;");
            js.Append("\r\n\tvar locationParameters = null;");
            js.Append("\r\n\tvar presenceTypeParameters = null;");
            js.Append("\r\n\tvar detailLevelParameters = null;");
            js.Append("\r\n\tvar dateParameters = null;");

            i = 0;
            cRolexSelectionFilterBaseWebControlDate = null;
            foreach (RolexScheduleSelectionFilterBaseWebControl cRolexSelectionFilterBaseWebControl in _filterResultWebControlList.Values)
            {
                switch (i)
                {
                    case 0:
                        js.Append("\r\n\tif(isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " == true){");
                        js.Append("\r\n\t\tmediaParameters = " + cRolexSelectionFilterBaseWebControl.GetValuesSelectedMethod + ";");
                        js.Append("\r\n\t} else");
                        js.Append("\r\n\t\tmediaParameters = null;");
                        break;
                    case 1:
                        js.Append("\r\n\tif(isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " == true){");
                        js.Append("\r\n\t\tlocationParameters= " + cRolexSelectionFilterBaseWebControl.GetValuesSelectedMethod + ";");
                        js.Append("\r\n\t} else");
                        js.Append("\r\n\t\tlocationParameters = null;");
                        break;
                    case 2:
                        js.Append("\r\n\tif(isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " == true){");
                        js.Append("\r\n\t\tpresenceTypeParameters= " + cRolexSelectionFilterBaseWebControl.GetValuesSelectedMethod + ";");
                        js.Append("\r\n\t} else");
                        js.Append("\r\n\t\tpresenceTypeParameters = null;");
                        break;
                    case 3:
                        js.Append("\r\n\tif(isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " == true){");

                        js.Append("\r\n\t\tdetailLevelParameters = " + cRolexSelectionFilterBaseWebControl.GetValuesSelectedMethod + ";");
                        js.Append("\r\n\tif(detailLevelParameters!=null && detailLevelParameters.length==2 && detailLevelParameters[0].split(',').length==3 && detailLevelParameters[0].split(',')[0]=='none' ");
                        js.Append("\r\n\t && detailLevelParameters[0].split(',')[1] == 'none' ");
                        js.Append("\r\n\t && detailLevelParameters[0].split(',')[2]=='none'){");
                        js.Append("\r\n\t\tisLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " =false;");
                        js.Append("\r\n\t} ");
                        js.Append("\r\n\t} else");
                        js.Append("\r\n\t\tdetailLevelParameters = null;");
                        break;
                    case 4:
                        cRolexSelectionFilterBaseWebControlDate = cRolexSelectionFilterBaseWebControl;
                        js.Append("\r\n\tif(isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " == true){");
                        js.Append("\r\n\t\tdateParameters = " + cRolexSelectionFilterBaseWebControl.GetValuesSelectedMethod + ";");
                        js.Append("\r\n\t\tif(dateParameters != null){ ");
                        js.Append(CheckDates());
                        js.Append("\r\n\t} else");
                        js.Append("\r\n\t\tdateParameters = null;");

                        break;
                }
                i++;
            }
            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".ValidData('" + _webSession.IdSession + "'");
            js.Append(", mediaParameters");
            js.Append(", presenceTypeParameters");
            js.Append(", locationParameters");
            js.Append(", detailLevelParameters");
            js.Append(", dateParameters");
            js.Append(", validData_" + this.ID + "_callback);");
            js.Append("\r\nreturn false;");
            js.Append("\r\n}");

            js.Append("\r\nfunction validData_" + this.ID + "_callback(res){");
            js.Append("\r\n\tif(res != null && res.value!=null){ ");
            js.Append("\r\n\t\t\t alert(res.value);");
            js.Append("\r\n\t}");
            js.Append(DisplayMethod + "(false);");
            js.Append(ValidationMethod);
            js.Append("\r\n\tif(isLoaded_" + cRolexSelectionFilterBaseWebControlDate.ID + ") RefreshRolexScheduleSelectionWebControl('DateSelection');");

            js.Append("\r\n}\r\n");

            #endregion

            #region Cancel Data
            js.Append("\r\nfunction cancelData_" + this.ID + "(){");

            js.Append("\r\n\tvar dateParameters = null;");
            js.Append("\r\n\tvar locationParameters = null;");
            js.Append("\r\n\tvar presenceTypeParameters=null;");
            js.Append("\r\n\tvar detailLevelParameters = null;");
            js.Append("\r\n\tvar dateParameters = null;");

            i = 0;
            foreach (RolexScheduleSelectionFilterBaseWebControl cRolexSelectionFilterBaseWebControl in _filterResultWebControlList.Values)
            {
                switch (i)
                {
                    case 0:
                        js.Append("\r\n\tif(isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " == true){");
                        js.Append("\r\n\t\tmediaParameters = " + cRolexSelectionFilterBaseWebControl.GetValuesSelectedMethod + ";");
                        js.Append("\r\n\t\tif(mediaParameters != null) isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " =false;");
                        js.Append("\r\n\t} else");
                        js.Append("\r\n\t\tmediaParameters = null;");
                        break;
                    case 1:
                        js.Append("\r\n\tif(isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " == true){");
                        js.Append("\r\n\t\tlocationParameters = " + cRolexSelectionFilterBaseWebControl.GetValuesSelectedMethod + ";");
                        js.Append("\r\n\t\tif(locationParameters != null) isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " =false;");
                        js.Append("\r\n\t} else");
                        js.Append("\r\n\t\tlocationParameters = null;");
                        break;
                    case 2:
                        js.Append("\r\n\tif(isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " == true){");
                        js.Append("\r\n\t\tpresenceTypeParameters = " + cRolexSelectionFilterBaseWebControl.GetValuesSelectedMethod + ";");
                        js.Append("\r\n\t\tif(presenceTypeParameters != null) isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " =false;");
                        js.Append("\r\n\t} else");
                        js.Append("\r\n\t\tpresenceTypeParameters = null;");
                        break;
                    case 3:
                        js.Append("\r\n\tif(isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " == true){");
                        js.Append("\r\n\t\tdetailLevelParameters = " + cRolexSelectionFilterBaseWebControl.GetValuesSelectedMethod + ";");
                        js.Append("\r\n\t\tif(detailLevelParameters != null) isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " =false;");
                        js.Append("\r\n\t} else");
                        js.Append("\r\n\t\tdetailLevelParameters = null;");
                        break;
                    case 4:
                        js.Append("\r\n\tif(isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " == true){");
                        js.Append("\r\n\t\tdateParameters = " + cRolexSelectionFilterBaseWebControl.GetValuesSelectedMethod + ";");
                        js.Append("\r\n\t\tif(dateParameters != null) isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + " =false;");
                        js.Append("\r\n\t} else");
                        js.Append("\r\n\t\tdateParameters = null;");
                        break;
                }
                i++;
            }
            js.Append(DisplayMethod + "(false);");
            js.Append("\r\n}\r\n");

            #endregion

            js.Append("\r\n-->\r\n</script>");
            return js.ToString();
        }
        #endregion

        #region GetValidationJavascriptContent
        /// <summary>
        /// Get Validation Javascript Method
        /// </summary>
        /// <returns>Validation Javascript Method</returns>
        protected override string GetValidationJavascriptContent()
        {
            return base.GetValidationJavascriptContent();
        }
        #endregion

        #region GetDisplayJavascriptContent
        /// <summary>
        /// Get Display Javascript Method
        /// </summary>
        /// <returns>Display Javascript Method</returns>
        protected override string GetDisplayJavascriptContent()
        {
            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n\tif(display) {");
            js.Append("\r\n\t\tif(document.body && document.body.scrollHeight){");
            js.Append("\r\n\t\t document.getElementById('res_backgroud_" + this.ID + "').style.height=document.body.scrollHeight;");
            js.Append("\r\n\t\t document.getElementById('res_backgroud_" + this.ID + "').style.width=document.body.scrollWidth;");
            js.Append("\r\n\t\t} else {");
            js.Append("\r\n\t\tdocument.getElementById('res_backgroud_" + this.ID + "').style.height=document.body.clientHeight + \"px\";");
            js.Append("\r\n\t\tdocument.getElementById('res_backgroud_" + this.ID + "').style.width=document.body.clientWidth + \"px\";");
            js.Append("\r\n\t\t}");
            js.Append("\r\n\t\tdocument.getElementById('res_backgroud_" + this.ID + "').style.display = '';");


            js.Append("\r\n\t\tvar myWidth = 0, myHeight = 0;");
            js.Append("\r\n\t\tif( typeof( window.innerWidth ) == 'number' ) {");
            //Non-IE
            js.Append("\r\n\t\tmyWidth = window.innerWidth;");
            js.Append("\r\n\t\tmyHeight = window.innerHeight;");
            js.Append("\r\n\t\t} else if( document.documentElement && ( document.documentElement.clientWidth || document.documentElement.clientHeight ) ) {");
            //IE 6+ in 'standards compliant mode'
            js.Append("\r\n\t\tmyWidth = document.documentElement.clientWidth;");
            js.Append("\r\n\t\tmyHeight = document.documentElement.clientHeight;");
            js.Append("\r\n\t\t} else if( document.body && ( document.body.clientWidth || document.body.clientHeight ) ) {");
            //IE 4 compatible
            js.Append("\r\n\t\tmyWidth = document.body.clientWidth;");
            js.Append("\r\n\t\tmyHeight = document.body.clientHeight;");
            js.Append("\r\n\t\t}");


            js.Append("\r\n\tif( document.documentElement && ( document.documentElement.scrollTop || document.documentElement.scrollLeft ) ) {");
            js.Append("\r\n\tdocument.getElementById('" + this.ID + "').style.top = (document.documentElement.scrollTop + ((myHeight - 550) / 2)) + \"px\";");
            js.Append("\r\n\tdocument.getElementById('" + this.ID + "').style.left = (document.documentElement.scrollLeft + ((myWidth - 750) / 2)) + \"px\";");
            js.Append("\r\n\t } else ");
            js.Append("\r\n\t {");
            js.Append("\r\n\t\t document.getElementById('" + this.ID + "').style.top = (document.body.scrollTop + ((myHeight - 550) / 2)) + \"px\";");
            js.Append("\r\n\t\t document.getElementById('" + this.ID + "').style.left = (document.body.scrollLeft + ((myWidth - 750) / 2)) + \"px\";");
            js.Append("\r\n\t }");        

            int i = 0;
            foreach (RolexScheduleSelectionFilterBaseWebControl cRolexSelectionFilterBaseWebControl in _filterResultWebControlList.Values)
            {

                js.Append("\r\n\tif(!isLoaded_" + cRolexSelectionFilterBaseWebControl.ID + "){");
                js.Append("\r\n\t\tif(document.getElementById('menu_" + cRolexSelectionFilterBaseWebControl.ID + "').className == '" + CssClassOptionMenuSelected + "'){");
                js.Append("\r\n\t\tvalid_menu_" + this.ID + "('" + cRolexSelectionFilterBaseWebControl.ID + "');");
                js.Append("\r\n\t\t} else {");
                js.Append("\r\n\t\t\t" + cRolexSelectionFilterBaseWebControl.InitializeResultMethod + ";");
                js.Append("\r\n\t\t}");
                js.Append("\r\n\t}");

                i++;
            }

            js.Append("\r\n\t} else {");
            js.Append("\r\n\t\tdocument.getElementById('res_backgroud_" + this.ID + "').style.display = 'none';");
            js.Append("\r\n\t}");
            return (base.GetDisplayJavascriptContent() + js.ToString());
        }

        /// <summary>
        /// Get Display Javascript Method
        /// </summary>
        /// <returns>Display Javascript Method</returns>
        protected string GetJavascriptScrollContent()
        {
            StringBuilder js = new StringBuilder();
            js.Append("\r\n<script language=\"javascript\">\r\n<!--");
            js.Append("\r\n function ScrollContent_" + this.ID + "(){");


            js.Append("\r\n\t   var myWidth = 0, myHeight = 0;");
            js.Append("\r\n\t\tif( typeof( window.innerWidth ) == 'number' ) {");
            //Non-IE
            js.Append("\r\n\t\tmyWidth = window.innerWidth;");
            js.Append("\r\n\t\tmyHeight = window.innerHeight;");
            js.Append("\r\n\t\t} else if( document.documentElement && ( document.documentElement.clientWidth || document.documentElement.clientHeight ) ) {");
            //IE 6+ in 'standards compliant mode'
            js.Append("\r\n\t\tmyWidth = document.documentElement.clientWidth;");
            js.Append("\r\n\t\tmyHeight = document.documentElement.clientHeight;");
            js.Append("\r\n\t\t} else if( document.body && ( document.body.clientWidth || document.body.clientHeight ) ) {");
            //IE 4 compatible
            js.Append("\r\n\t\tmyWidth = document.body.clientWidth;");
            js.Append("\r\n\t\tmyHeight = document.body.clientHeight;");
            js.Append("\r\n\t\t}");

            js.Append("\r\n\t   var oContent = document.getElementById('" + this.ID + "');");
            js.Append("\r\n\t if(oContent != null && ( oContent.style.display  != 'none')){");
            js.Append("\r\n\t\t     if( document.documentElement && ( document.documentElement.scrollTop || document.documentElement.scrollLeft ) ) {");
            js.Append("\r\n\t\t\t       oContent.style.top = (document.documentElement.scrollTop + ((myHeight - 550) / 2)) + \"px\";");
            js.Append("\r\n\t\t        oContent.style.left = (document.documentElement.scrollLeft + ((myWidth - 750) / 2)) + \"px\";");
            js.Append("\r\n\t\t     } else ");
            js.Append("\r\n\t\t     {");
            js.Append("\r\n\t\t\t       oContent.style.top = (document.body.scrollTop + ((myHeight - 550) / 2)) + \"px\";");
            js.Append("\r\n\t\t\t       oContent.style.left = (document.body.scrollLeft + ((myWidth - 750) / 2)) + \"px\";");
            js.Append("\r\n\t       }");
            js.Append("\r\n\t }");
            js.Append("\r\n}\r\n");
          
            js.Append("\r\n-->\r\n</script>");
            return js.ToString();

        }
        #endregion

        #region GetInitializeJavascriptContent
        /// <summary>
        /// Get Initialize Javascript Method
        /// </summary>
        /// <returns>Initialize Javascript Method</returns>
        protected override string GetInitializeJavascriptContent()
        {
            return base.GetInitializeJavascriptContent();
        }
        #endregion

        #endregion

        #region Property (Style)
        /// <summary>
        /// Get / Set RolexScheduleSelectionNodeMediaWebControlSkinId
        /// </summary>
        public string RolexScheduleSelectionNodeMediaWebControlSkinId { get; set; }
        /// <summary>
        /// Get / Set RolexScheduleSelectionNodeLocationWebControlSkinId
        /// </summary>
        public string RolexScheduleSelectionNodeLocationWebControlSkinId { get; set; }
        /// <summary>
        /// Get / Set RolexScheduleSelectionNodePresenceTypeWebControlSkinId
        /// </summary>
        public string RolexScheduleSelectionNodePresenceTypeWebControlSkinId { get; set; }
        /// <summary>
        /// Get / Set RolexScheduleSelectionDetailLevelWebControlSkinId
        /// </summary>
        public string RolexScheduleSelectionDetailLevelWebControlSkinId { get; set; }
        /// <summary>
        /// Get / Set CssClassOption
        /// </summary>
        public string CssClassOption { get; set; }
        /// <summary>
        /// Get / Set CssClassOptionHeader
        /// </summary>
        public string CssClassOptionHeader { get; set; }
        /// <summary>
        /// Get / Set CssClassOptionMenu
        /// </summary>
        public string CssClassOptionMenu { get; set; }
        /// <summary>
        /// Get / Set CssClassOptionMenuSelected
        /// </summary>
        public string CssClassOptionMenuSelected { get; set; }
        /// <summary>
        /// Get / Set CssClassResult
        /// </summary>
        public string CssClassResult { get; set; }
        /// <summary>
        /// Get / Set CssClassResultContent
        /// </summary>
        public string CssClassResultContent { get; set; }
        /// <summary>
        /// Get / Set CssClassOptionButtons
        /// </summary>
        public string CssClassOptionButtons { get; set; }
        /// <summary>
        /// Get / Set CssClassOptionButtonCancel
        /// </summary>
        public string CssClassOptionButtonCancel { get; set; }
        /// <summary>
        /// Get / Set PicturePathButtonCancel
        /// </summary>
        public string PicturePathButtonCancel { get; set; }
        /// <summary>
        /// Get / Set PicturePathButtonCancelOver
        /// </summary>
        public string PicturePathButtonCancelOver { get; set; }
        /// <summary>
        /// Get / Set CssClassOptionButtonValidation
        /// </summary>
        public string CssClassOptionButtonValidation { get; set; }
        /// <summary>
        /// Get / Set PicturePathButtonValidation
        /// </summary>
        public string PicturePathButtonValidation { get; set; }
        /// <summary>
        /// Get / Set PicturePathButtonValidationOver
        /// </summary>
        public string PicturePathButtonValidationOver { get; set; }
        #endregion

        #region Evènements

        #region Initialisation
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeResultToLoad = false;
        }
        #endregion

        #region Load
        /// <summary>
        /// Chargement du composant
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Domain.Web.Navigation.Module module = ModulesList.GetModule(_webSession.CurrentModule);
            if (module.DefaultMediaDetailLevels != null && module.DefaultMediaDetailLevels.Count > 0)
            {

                var rolexSelectionNodeWebControl = new RolexScheduleSelectionNodeWebControl();
                rolexSelectionNodeWebControl.ID = this.ID + "_Media";
                rolexSelectionNodeWebControl.LevelIds = ((GenericDetailLevel)(module.DefaultMediaDetailLevels[0])).LevelIds;
                rolexSelectionNodeWebControl.WebSession = _webSession;
                rolexSelectionNodeWebControl.Display = true;
                rolexSelectionNodeWebControl.SkinID = RolexScheduleSelectionNodeMediaWebControlSkinId;
                rolexSelectionNodeWebControl.GenericDetailLevelComponentProfile = Constantes.Web.GenericDetailLevel.ComponentProfile.media;
                _filterResultWebControlList.Add(2977, rolexSelectionNodeWebControl);
                Controls.Add(rolexSelectionNodeWebControl);

                rolexSelectionNodeWebControl = new RolexScheduleSelectionNodeWebControl();
                rolexSelectionNodeWebControl.ID = this.ID + "_Location";
                rolexSelectionNodeWebControl.LevelIds = ((GenericDetailLevel)(module.DefaultMediaDetailLevels[1])).LevelIds;
                rolexSelectionNodeWebControl.WebSession = _webSession;
                rolexSelectionNodeWebControl.Display = false;
                rolexSelectionNodeWebControl.SkinID = RolexScheduleSelectionNodeLocationWebControlSkinId;
                rolexSelectionNodeWebControl.GenericDetailLevelComponentProfile = Constantes.Web.GenericDetailLevel.ComponentProfile.location;
                _filterResultWebControlList.Add(1439, rolexSelectionNodeWebControl);
                Controls.Add(rolexSelectionNodeWebControl);

                rolexSelectionNodeWebControl = new RolexScheduleSelectionNodeWebControl();
                rolexSelectionNodeWebControl.ID = this.ID + "_PresenceType";
                rolexSelectionNodeWebControl.LevelIds = ((GenericDetailLevel)(module.DefaultMediaDetailLevels[2])).LevelIds;
                rolexSelectionNodeWebControl.WebSession = _webSession;
                rolexSelectionNodeWebControl.Display = false;
                rolexSelectionNodeWebControl.SkinID = RolexScheduleSelectionNodePresenceTypeWebControlSkinId;
                rolexSelectionNodeWebControl.GenericDetailLevelComponentProfile = Constantes.Web.GenericDetailLevel.ComponentProfile.presenceType;
                _filterResultWebControlList.Add(2978, rolexSelectionNodeWebControl);
                Controls.Add(rolexSelectionNodeWebControl);

                var roledxScheduleSelectionDetailLevelWebControl = new RolexScheduleSelectionDetailLevelWebControl();
                roledxScheduleSelectionDetailLevelWebControl.ID = this.ID + "_DetailLevel";
                roledxScheduleSelectionDetailLevelWebControl.WebSession = _webSession;
                roledxScheduleSelectionDetailLevelWebControl.Display = false;
                roledxScheduleSelectionDetailLevelWebControl.SkinID = RolexScheduleSelectionDetailLevelWebControlSkinId;
                _filterResultWebControlList.Add(2871, roledxScheduleSelectionDetailLevelWebControl);
                Controls.Add(roledxScheduleSelectionDetailLevelWebControl);

                var rolexScheduleSelectionDatesWebControl = new RolexScheduleSelectionDatesWebControl();
                rolexScheduleSelectionDatesWebControl.ID = this.ID + "_Dates";
                rolexScheduleSelectionDatesWebControl.WebSession = _webSession;
                rolexScheduleSelectionDatesWebControl.Display = false;
                _filterResultWebControlList.Add(1755, rolexScheduleSelectionDatesWebControl);
                Controls.Add(rolexScheduleSelectionDatesWebControl);
            }
        }
        #endregion

        #region PréRender
        /// <summary>
        /// Prérendu
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e)
        {
            Page.Response.Write("<div id=\"res_backgroud_" + this.ID + "\" class=\"rolexScheduleResultFilterWebControlBackgroud\" onclick=\"cancelData_" + this.ID + "();\" style=\"display:none;\"></div>");
            base.OnPreRender(e);
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output)
        {
            output.Write(GetJavascriptMenu());
            output.Write(GetJavascriptValidData());
            output.Write(GetJavascriptScrollContent());
            base.Render(output);
        }
        #endregion

        #endregion

        #region GetHTML
        /// <summary>
        /// Compute VP schedule
        /// </summary>
        /// <returns>Code HTMl</returns>
        protected override string GetHTML()
        {
            StringBuilder html = new StringBuilder(1000);

            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" align=\"center\" width=\"100%\" height=\"100%\">");
            html.Append("<tr>");
            html.Append("<td class=\"" + CssClassOption + "\">");

            #region Menu
            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" align=\"center\" width=\"100%\" height=\"100%\" class=\"" + CssClassOption + "\">");
            html.Append("<tr><td class=\"" + CssClassOptionHeader + "\">");
            html.Append(GestionWeb.GetWebWord(2863, _webSession.SiteLanguage));
            html.Append("</td></tr>");
            html.Append("<tr><td class=\"" + CssClassOptionMenu + "\">");
            html.Append("<ul>");
            foreach (KeyValuePair<Int64, RolexScheduleSelectionFilterBaseWebControl> kvp in _filterResultWebControlList)
            {
                html.Append("<li><a");
                if (kvp.Value.Display)
                    html.Append(" class=\"" + CssClassOptionMenuSelected + "\"");
                html.Append(" id=\"menu_" + kvp.Value.ID + "\" href=\"#\" onclick=\"javascript:valid_menu_" + this.ID + "('" + kvp.Value.ID + "');\">");
                html.Append(GestionWeb.GetWebWord(kvp.Key, _webSession.SiteLanguage));
                html.Append("</a></li>");
            }
            html.Append("</ul>");
            html.Append("</td></tr>");
            html.Append("</table>");
            #endregion

            html.Append("</td>");
            html.Append("<td class=\"" + CssClassResult + "\">");

            html.Append("<div class=\"" + CssClassResult + "\">");
            html.Append("<div class=\"" + CssClassResultContent + "\">");
            #region Result
            foreach (RolexScheduleSelectionFilterBaseWebControl cRolexSelectionFilterBaseWebControl in _filterResultWebControlList.Values)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                    {
                        using (HtmlTextWriter memoryWriter = new HtmlTextWriter(streamWriter))
                        {
                            cRolexSelectionFilterBaseWebControl.RenderControl(memoryWriter);
                            memoryWriter.Flush();
                            memoryStream.Position = 0;
                            using (StreamReader reader = new StreamReader(memoryStream))
                            {
                                html.Append(reader.ReadToEnd());
                            }
                        }
                    }
                }
            }
            #endregion
            html.Append("</div>");
            html.Append("</div>");

            html.Append("</td>");
            html.Append("</tr>");

            html.Append("<tr>");
            html.Append("<td colspan=\"2\" class=\"" + CssClassOptionButtons + "\" align=\"right\">");

            #region Buttons
            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" class=\"" + CssClassOptionButtons + "\">");
            html.Append("<tr>");
            html.Append("<td class=\"" + CssClassOptionButtonCancel + "\">");
            html.Append("<img src=\"" + PicturePathButtonCancel + "\" onmouseover=\"javascript:this.src='" + PicturePathButtonCancelOver + "';\" onmouseout=\"javascript:this.src='" + PicturePathButtonCancel + "';\" onclick=\"javascript:cancelData_" + this.ID + "();\"/>");
            html.Append("</td>");
            html.Append("<td class=\"" + CssClassOptionButtonValidation + "\">");
            html.Append("<img src=\"" + PicturePathButtonValidation + "\" onmouseover=\"javascript:this.src='" + PicturePathButtonValidationOver + "';\" onmouseout=\"javascript:this.src='" + PicturePathButtonValidation + "';\" onclick=\"javascript:validData_" + this.ID + "();\"/>");
            html.Append("</td>");
            html.Append("</tr>");
            html.Append("</table>");
            #endregion

            html.Append("</td>");
            html.Append("</tr>");

            html.Append("</table>");
            return html.ToString();

        }
        #endregion

        #region ValidData
        /// <summary>
        /// Obtention du code HTML à insérer dans le composant
        /// </summary>
        /// <param name="sessionId">Session du client</param>
        /// <param name="oParams">Tableaux de paramètres</param>
        /// <returns>Code HTML</returns>
        [AjaxPro.AjaxMethod]
        public string ValidData(string sessionId, object[] mediaParameters, object[] presenceTypeParameters, object[] locationParameters, object[] detailLevelParameters, object[] dateParameters)
        {
            string html = null;
            try
            {

                #region Obtention de la session
                _webSession = (WebSession)WebSession.Load(sessionId);
                #endregion

                Domain.Web.Navigation.Module module = ModulesList.GetModule(_webSession.CurrentModule);

                if (mediaParameters != null)
                {
                    _webSession.CurrentUniversMedia = _webSession.SelectionUniversMedia = GetTreeNode(mediaParameters, (GenericDetailLevel)(module.DefaultMediaDetailLevels[0]), GetAccessType);
                }
                if (presenceTypeParameters != null)
                {
                     _webSession.SelectedPresenceTypes = GetSelectedItems(presenceTypeParameters);
                }
                if (locationParameters != null)
                {
                    _webSession.SelectedLocations = GetSelectedItems(locationParameters);
                }
                if (detailLevelParameters != null)
                {
                    if (((string)detailLevelParameters[0]).Split(',')[0] != "none" || ((string)detailLevelParameters[0]).Split(',')[1] != "none" || ((string)detailLevelParameters[0]).Split(',')[2] != "none")
                    {
                        var detailTabList = new List<DetailLevelItemInformation.Levels>();
                        foreach (string detailTab in ((string)detailLevelParameters[0]).Split(','))
                        {
                            if (!string.IsNullOrEmpty(detailTab) && detailTab != "none")
                                detailTabList.Add((DetailLevelItemInformation.Levels)Enum.Parse(typeof(DetailLevelItemInformation.Levels), detailTab));
                        }
                        _webSession.GenericMediaDetailLevel = new GenericDetailLevel(new ArrayList(detailTabList));
                    }
                }
                if (dateParameters != null)
                {                    
                    _webSession.SetDates(new DateTime(Int32.Parse(((string)dateParameters[0]).Split('_')[0]), Int32.Parse(((string)dateParameters[0]).Split('_')[1]), 1)
                        , new DateTime(Int32.Parse(((string)dateParameters[1]).Split('_')[0]), Int32.Parse(((string)dateParameters[1]).Split('_')[1]), 1).AddMonths(1).AddDays(-1));
                }
                _webSession.Save();
            }
            catch (Exception)
            {
                return (GestionWeb.GetWebWord(1973, _webSession.SiteLanguage));
            }
            return (html);
        }

        private List<long> GetSelectedItems(object[] parameters)
        {
            if(parameters!=null && parameters.Length>0)
            {
                return (new List<object>(parameters)).ConvertAll(Convert.ToInt64);
            }
            return new List<long>();
        }
        #endregion

        #region GetAjaxHTML
        /// <summary>
        /// GetAjaxHTML
        /// </summary>
        /// <returns></returns>
        protected override string GetAjaxHTML()
        {
            throw new NotImplementedException();
        }
        #endregion


        

        
    }
}
