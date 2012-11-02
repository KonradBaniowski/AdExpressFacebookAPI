using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Rolex;
using TNS.AdExpressI.VP;
using TNS.AdExpress.Domain.Level;
using System.Collections;

namespace TNS.AdExpress.Web.Controls.Results.Rolex
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:RolexResultWebControl runat=server></{0}:RolexResultWebControl>")]
    public class RolexScheduleResultWebControl : AjaxResultBaseWebControl
    {
        #region GetJavascript
        /// <summary>
        /// Get Rolex File Javascript Method
        /// </summary>
        /// <returns>Validation Javascript Method</returns>
        protected string GetRolexFileJavascript()
        {
            StringBuilder js = new StringBuilder();
            js.Append("\r\n<script language=\"javascript\">\r\n");
            js.Append(" var imagesName_ItemsCollectionNavigator_" + this.ID + "= new Array();");
           
            js.Append(" var  currentPanelIndex_ItemsCollectionNavigator_" + this.ID + "=0;");
         
            js.Append(" var topPos_" + this.ID + " = 0; ");

            js.Append("\r\nfunction displayRolexFile_" + this.ID + "(idLevels,periodBeginningDate,periodEndDate, display){");
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


            js.Append("\r\n\t if( document.documentElement && ( document.documentElement.scrollTop || document.documentElement.scrollLeft ) ) {");
            js.Append("\r\n\t  topPos_" + this.ID + " = (document.documentElement.scrollTop + ((myHeight - 650) / 2)) ; ");
            js.Append("\r\n\t  if(topPos_" + this.ID + " <=0) topPos_" + this.ID + " = 10;");
            js.Append("\r\n\t document.getElementById('res_rolexfile_" + this.ID + "').style.top = topPos_" + this.ID + " + \"px\";");
            js.Append("\r\n\tdocument.getElementById('res_rolexfile_" + this.ID + "').style.left = (document.documentElement.scrollLeft + ((myWidth - 650) / 2)) + \"px\";");
            js.Append("\r\n\t } else ");
            js.Append("\r\n\t {");
            js.Append("\r\n\t\t  topPos_" + this.ID + " = (document.body.scrollTop + ((myHeight - 650) / 2)); ");
            js.Append("\r\n\t\t  if(topPos_" + this.ID + " <=0) topPos_" + this.ID + " = 10;");
            js.Append("\r\n\t\t document.getElementById('res_rolexfile_" + this.ID + "').style.top = topPos_" + this.ID + " + \"px\";");
            js.Append("\r\n\t\t document.getElementById('res_rolexfile_" + this.ID + "').style.left = (document.body.scrollLeft + ((myWidth - 650) / 2)) + \"px\";");
            js.Append("\r\n\t }");


            js.Append("\r\n\t\tdocument.getElementById('res_rolexfile_" + this.ID + "').style.display = '';");


            js.Append("\r\n\tgetRolexFile_" + this.ID + "(idLevels,periodBeginningDate,periodEndDate);");

            js.Append("\r\n\t} else {");
           
            js.Append("\r\n\t\tdocument.getElementById('res_backgroud_" + this.ID + "').style.display = 'none';");
            js.Append("\r\n\t\tdocument.getElementById('res_rolexfile_" + this.ID + "').style.display = 'none';");
            js.Append("  imagesName_ItemsCollectionNavigator_" + this.ID + "= new Array();");
           
            //js.Append("  currentPanelIndex_ItemsCollectionNavigator_" + this.ID + "=0;");
           
            js.Append("\r\n\t}");

            js.Append("\r\n}\r\n");

            js.Append("\r\nfunction getRolexFile_" + this.ID + "(idLevels,periodBeginningDate,periodEndDate){");
            js.Append("\r\n\tvar oN=document.getElementById('res_rolexfile_" + this.ID + "');");

            js.Append("\r\n\toN.innerHTML='" + GetHTML().Replace("'", "\\'") + "';");
            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".GetRolexFileData('" + this._webSession.IdSession
                + "', ''+idLevels+'',''+periodBeginningDate+'',''+periodEndDate+'', resultParameters_" 
                + this.ID + ",styleParameters_" + this.ID + ",getRolexFile_" + this.ID + "_callback);");
            js.Append("\r\n}");

            js.Append("\r\nfunction getRolexFile_" + this.ID + "_callback(res){");
                    
            js.Append("\r\n\tvar oN=document.getElementById('res_rolexfile_" + this.ID + "');");

            js.Append("\r\n\tif(res.value.length>1){");
            js.Append("\r\n\t\t imagesName_ItemsCollectionNavigator_" + this.ID + "= new Array();");
            js.Append("\r\n\t\t if(res!=null && res.value!=null &&  res.value[1]!=null){ ");
            js.Append(" \r\n\t\t\t imagesName_ItemsCollectionNavigator_" + this.ID + " = res.value[1].split(',');");
            js.Append(" \r\n\t\t\t PreloadImages_" + this.ID + "(imagesName_ItemsCollectionNavigator_" + this.ID + ");");
            js.Append("\r\n\t\t } ");
            js.Append("\r\n\t}");

            js.Append("\r\n\tif(res.value.length>0)");
            js.Append("\r\n\t\toN.innerHTML=res.value[0];");
            js.Append("\r\n}\r\n");

            

          

            js.Append("\r\nfunction ZoomRolexImage_" + this.ID + "(imgRolex){");
            js.Append("\r\n\t window.open(imgRolex, '', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\", toolbar=1, directories=0, status=0, menubar=1, width=1024, height=600, scrollbars=1, location=0, resizable=1\");");
            js.Append("\r\n}\r\n");

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

            js.Append("\r\n\t   var oContent = document.getElementById('res_rolexfile_" + this.ID + "');");
            js.Append("\r\n\t if(oContent != null && ( oContent.style.display  != 'none')){");
            js.Append("\r\n\t\t     if( document.documentElement && ( document.documentElement.scrollTop || document.documentElement.scrollLeft ) ) {");
            js.Append("\r\n\t\t\t       oContent.style.top = (document.documentElement.scrollTop + ((myHeight - 650) / 2)) + \"px\";");
            js.Append("\r\n\t\t        oContent.style.left = (document.documentElement.scrollLeft + ((myWidth - 650) / 2)) + \"px\";");
            js.Append("\r\n\t\t     } else ");
            js.Append("\r\n\t\t     {");
            js.Append("\r\n\t\t\t       oContent.style.top = (document.body.scrollTop + ((myHeight - 650) / 2)) + \"px\";");
            js.Append("\r\n\t\t\t       oContent.style.left = (document.body.scrollLeft + ((myWidth - 650) / 2)) + \"px\";");
            js.Append("\r\n\t       }");
            js.Append("\r\n\t }");
            js.Append("\r\n}\r\n");

            js.Append("\r\n function CloseDivs_" + this.ID + "(){");
            js.Append("\r\n\t document.getElementById('res_backgroud_" + this.ID + "').style.display = 'none';");
            js.Append("\r\n\t document.getElementById('res_rolexfile_" + this.ID + "').style.display = 'none';");
            js.Append("\r\n}\r\n");

            //PreloadImages_
            js.Append("\r\n function PreloadImages_" + this.ID + "(images){");
            js.Append("\r\n\t  var img = new Image(); ");
            js.Append("\r\n\t  for(i=0; i<images.length; i++){ ");
            js.Append("\r\n\t\t img.src = images[i]; ");
            js.Append("\r\n\t\t } ");
            js.Append("\r\n}\r\n");

            //DisplayPics_
            js.Append("\r\n function DisplayPics_" + this.ID + "(indexIncr, btLeft, btRight, curImg, imagesStr){");
            js.Append("\r\n\t   var rolexDirectoryPath = '" + Constantes.Web.CreationServerPathes.IMAGES_ROLEX + "/';");
            js.Append("\r\n\t  var imagesArr = imagesStr.split(',');");
            js.Append("\r\n\t  var tempArr = curImg.src.split('=');");
            js.Append("\r\n\t  var curPos = tempArr[1];");
            js.Append("\r\n\t  var pos = parseInt(curPos) + parseInt(indexIncr);");

            js.Append("\r\n\t  btLeft.style.visibility = (pos <= 0) ? 'hidden' : 'visible';");
            js.Append("\r\n\t  btRight.style.visibility = (pos >= imagesArr.length - 1) ? 'hidden' : 'visible';");

            js.Append("\r\n\t  if(pos > -1 && pos <= imagesArr.length - 1)");
            js.Append("\r\n\t\t  {");
            js.Append("\r\n\t\t  curImg.src = rolexDirectoryPath + imagesArr[pos] + '?p=' + pos;");
            js.Append("\r\n\t\t  }");
            js.Append("\r\n}\r\n");

            js.Append("\r\n\r\n</script>");
            return js.ToString();
        }
        #endregion

        #region Evènements

        #region PréRender
        /// <summary>
        /// Prérendu
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e)
        {
            Page.Response.Write("<div id=\"res_rolexfile_" + this.ID + "\" class=\"rolexPrf\" style=\"display:none;\"></div>");
            Page.Response.Write("<div id=\"res_backgroud_" + this.ID + "\" class=\"rolexScheduleResultFilterWebControlBackgroud\" onclick=\"CloseDivs_" + this.ID + "();\" style=\"display:none;\"></div>");
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
            StringBuilder html = new StringBuilder(1000);
            html.Append(GetRolexFileJavascript());
            output.Write(html.ToString());
            base.Render(output);
        }
        #endregion

        #endregion

        #region GetHTML
        /// <summary>
        /// Compute VP schedule
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <returns>Code HTMl</returns>
        protected override string GetAjaxHTML()
        {
            Domain.Web.Navigation.Module _module = ModulesList.GetModule(Constantes.Web.Module.Name.ROLEX);
            object[] param = new object[1] { _webSession };
            var rolexResult = (IRolexResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory 
                + @"Bin\" + _module.CountryRulesLayer.AssemblyName, _module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            rolexResult.ResultControlId = this.ID;
            return (rolexResult.GetHtml());
        }

        #endregion

        #region  Enregistrement des paramètres de construction du résultat
        /// <summary>
        /// Set Current Result Parameters Script
        /// </summary>
        /// <returns></returns>
        protected override string SetCurrentResultParametersScript()
        {
            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n\t obj.ResultControlId = '" + this.ID + "';");
            return (js.ToString());
        }
        #endregion

        #region Chargement des paramètres AjaxPro.JavaScriptObject et WebSession
        /// <summary>
        /// Load Current Result Parameters
        /// </summary>
        /// <param name="o"></param>
        protected override void LoadCurrentResultParameters(AjaxPro.JavaScriptObject o)
        {
            if (o != null)
            {
                if (o.Contains("ResultControlId"))
                {
                    string resultControlId = o["ResultControlId"].Value;
                    this.ID = resultControlId.Replace("\"", "");
                }
            }
        }
        #endregion

        /// <summary>
        /// Get Rolex file HTML  code
        /// </summary>
        /// <param name="resultParameters">Result parameters (session Id, theme...)</param>
        /// <returns>Code HTML</returns>
        [AjaxPro.AjaxMethod]
        public virtual object[] GetRolexFileData(string idSession, string idLevels, string periodBeginningDate,string periodEndDate
            ,AjaxPro.JavaScriptObject resultParameters, AjaxPro.JavaScriptObject styleParameters)
        {
            object[] tab = new object[2];

            try
            {
                LoadResultParameters(resultParameters);
                LoadStyleParameters(styleParameters);
                _webSession = (WebSession)WebSession.Load(idSession);

                Domain.Web.Navigation.Module _module = ModulesList.GetModule(Constantes.Web.Module.Name.ROLEX);
                var param = new object[] { _webSession, periodBeginningDate, periodEndDate };
                var rolexResult = (IRolexResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\"
                    + _module.CountryRulesLayer.AssemblyName, _module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                    | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                rolexResult.Theme = _themeName;
                rolexResult.ResultControlId = this.ID;

                List<string> visualList;              
                var list = new List<string>(idLevels.Split(',')).ConvertAll(p=>Convert.ToInt64(p));         

                string html = rolexResult.GetRolexFileHtml(_webSession.GenericMediaDetailLevel, list,out visualList);

                if (visualList != null && visualList.Count>0)
                    tab[1] =  string.Join(",", visualList.ToArray());

                tab[0] = html;
            }
            catch (System.Exception err)
            {
                throw new Exception(OnAjaxMethodError(err, _webSession), err);
            }
            return (tab);
        }
    }
}
