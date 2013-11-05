#region Informations
// Auteur: G. Facon 
// Date de création: 13/07/2006
// Date de modification:
//		G Ragneau - 08/08/2006 - Set GetHtml as public so as to access it 
//		G Ragneau - 08/08/2006 - GetHTML : Force media plan alert module and restaure it after process (<== because of version zoom);
//		G Ragneau - 05/05/2008 - GetHTML : implement layers
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Reflection;
using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.VP;
namespace TNS.AdExpress.Web.Controls.Results.VP
{
    /// <summary>
    /// Affiche le résultat d'une alerte plan media
    /// </summary>
    [DefaultProperty("Text"),
      ToolboxData("<{0}:GenericMediaScheduleWebControl runat=server></{0}:GenericMediaScheduleWebControl>")]
    public class VpScheduleResultWebControl : VpScheduleAjaxResultBaseWebControl
    {

        #region GetJavascript
        /// <summary>
        /// Get Promo File Javascript Method
        /// </summary>
        /// <returns>Validation Javascript Method</returns>
        protected string GetPromoFileJavascript()
        {
            StringBuilder js = new StringBuilder();
            js.Append("\r\n<script language=\"javascript\">\r\n");
            js.Append(" var imagesName_ItemsCollectionNavigator_" + this.ID + "= new Array();");
            js.Append(" var imagesName_ItemsCollectionNavigator2_" + this.ID + "= new Array();");
            js.Append(" var  currentPanelIndex_ItemsCollectionNavigator_" + this.ID + "=0;");
            js.Append(" var currentPanelIndex_ItemsCollectionNavigator2_" + this.ID + "=0;");
            js.Append(" var topPos_" + this.ID + " = 0; ");

            js.Append("\r\nfunction displayPromoFile_" + this.ID + "(idPromo, display){");
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
            js.Append("\r\n\t document.getElementById('res_promofile_" + this.ID + "').style.top = topPos_" + this.ID + " + \"px\";");
            js.Append("\r\n\tdocument.getElementById('res_promofile_" + this.ID + "').style.left = (document.documentElement.scrollLeft + ((myWidth - 650) / 2)) + \"px\";");         
            js.Append("\r\n\t } else ");
            js.Append("\r\n\t {");
            js.Append("\r\n\t\t  topPos_" + this.ID + " = (document.body.scrollTop + ((myHeight - 650) / 2)); ");
            js.Append("\r\n\t\t  if(topPos_" + this.ID + " <=0) topPos_" + this.ID + " = 10;");
            js.Append("\r\n\t\t document.getElementById('res_promofile_" + this.ID + "').style.top = topPos_" + this.ID + " + \"px\";");
            js.Append("\r\n\t\t document.getElementById('res_promofile_" + this.ID + "').style.left = (document.body.scrollLeft + ((myWidth - 650) / 2)) + \"px\";");
            js.Append("\r\n\t }");


            js.Append("\r\n\t\tdocument.getElementById('res_promofile_" + this.ID + "').style.display = '';");
         

            js.Append("\r\n\tgetPromoFile_" + this.ID + "(idPromo);");

            js.Append("\r\n\t} else {");
            js.Append("\r\n\t\tdocument.getElementById('res_backgroud_" + this.ID + "').style.display = 'none';");
            js.Append("\r\n\t\tdocument.getElementById('res_promofile_" + this.ID + "').style.display = 'none';");
            js.Append("  imagesName_ItemsCollectionNavigator_" + this.ID + "= new Array();");
            js.Append("  imagesName_ItemsCollectionNavigator2_" + this.ID + "= new Array();");
            js.Append("  currentPanelIndex_ItemsCollectionNavigator_" + this.ID + "=0;");
            js.Append("  currentPanelIndex_ItemsCollectionNavigator2_" + this.ID + "=0;");
            js.Append("\r\n\t}");

            js.Append("\r\n}\r\n");

            js.Append("\r\nfunction getPromoFile_" + this.ID + "(idPromo){");
            js.Append("\r\n\tvar oN=document.getElementById('res_promofile_" + this.ID + "');");
            js.Append("\r\n\toN.innerHTML='" + GetHTML().Replace("'", "\\'") + "';");
            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".GetPromoFileData('" + this._webSession.IdSession + "', idPromo, resultParameters_" + this.ID + ",styleParameters_" + this.ID + ",getPromoFile_" + this.ID + "_callback);");
            js.Append("\r\n}");

            js.Append("\r\nfunction getPromoFile_" + this.ID + "_callback(res){");
            js.Append("\r\n\tvar oN=document.getElementById('res_promofile_" + this.ID + "');");
            js.Append("\r\n\tif(res.value.length>1){");
            js.Append("\r\n\t\timagesName_ItemsCollectionNavigator_" + this.ID + "= new Array();");
            js.Append("\r\n\t\tif(res!=null && res.value!=null &&  res.value[1]!=null)imagesName_ItemsCollectionNavigator_" + this.ID + " = res.value[1].split(',');");            
            js.Append("\r\n\t\timagesName_ItemsCollectionNavigator2_" + this.ID + "= new Array();");
            js.Append("\r\n\t\tif(res!=null && res.value!=null &&  res.value[2]!=null)imagesName_ItemsCollectionNavigator2_" + this.ID + " = res.value[2].split(',');");
            js.Append("\r\n\t}");
            js.Append("\r\n\tif(res.value.length>0)");
            js.Append("\r\n\t\toN.innerHTML=res.value[0];");
            js.Append("\r\n}\r\n");

            js.Append("\r\nfunction DisplayPics_" + this.ID + "(indexIncr, promoCondition, img_prev, img_next, img_pr){");
            js.Append("\r\n\t var currentIndexNavigator = (promoCondition) ? currentPanelIndex_ItemsCollectionNavigator2_" + this.ID + " : currentPanelIndex_ItemsCollectionNavigator_" + this.ID + ";");
            js.Append("\r\n\tcurrentIndexNavigator = currentIndexNavigator + indexIncr;");
            js.Append("\r\n\t if (promoCondition) {");
            js.Append("\r\n\t currentPanelIndex_ItemsCollectionNavigator2_" + this.ID + " = currentIndexNavigator;");
            js.Append("\r\n\t if(imagesName_ItemsCollectionNavigator2_" + this.ID +" != null && imagesName_ItemsCollectionNavigator2_" + this.ID+".length>0)");
            js.Append("\r\n\timg_pr.src = imagesName_ItemsCollectionNavigator2_" + this.ID + "[currentIndexNavigator];");
            js.Append("\r\n\t } else {");
            js.Append("\r\n\tcurrentPanelIndex_ItemsCollectionNavigator_" + this.ID + "  = currentIndexNavigator;");
            js.Append("\r\n\t if(imagesName_ItemsCollectionNavigator_" + this.ID + " != null && imagesName_ItemsCollectionNavigator_" + this.ID + ".length>0)");
            js.Append("\r\n\timg_pr.src = imagesName_ItemsCollectionNavigator_" + this.ID + "[currentIndexNavigator];");
            js.Append("\r\n\t}");
            js.Append("\r\n\t if (currentIndexNavigator > 0) {");
            js.Append("\r\n\timg_prev.style.visibility = 'visible';");
            js.Append("\r\n\t } else img_prev.style.visibility = 'hidden';");
            js.Append("\r\n\tif (promoCondition && imagesName_ItemsCollectionNavigator2_" + this.ID + "!=null && currentIndexNavigator != imagesName_ItemsCollectionNavigator2_" + this.ID + ".length-1) {");
            js.Append("\r\n\timg_next.style.visibility = 'visible';");
            js.Append("\r\n\t}else if (!promoCondition && imagesName_ItemsCollectionNavigator_" + this.ID + "!=null && currentIndexNavigator != imagesName_ItemsCollectionNavigator_" + this.ID + ".length-1) {");
            js.Append("\r\n\timg_next.style.visibility = 'visible';");
            js.Append("\r\n\t} else img_next.style.visibility = 'hidden';");                    
            js.Append("\r\n}\r\n");

            js.Append("\r\nfunction ZoomPromotionImage_" + this.ID + "(imgPromo){");
            js.Append("\r\n\t window.open(imgPromo, '', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\", toolbar=1, directories=0, status=0, menubar=1, width=1024, height=600, scrollbars=1, location=0, resizable=1\");");
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
          
            js.Append("\r\n\t   var oContent = document.getElementById('res_promofile_" + this.ID + "');");
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
            js.Append("\r\n\t document.getElementById('res_promofile_" + this.ID + "').style.display = 'none';");
            js.Append("\r\n}\r\n");

            //js.Append("\r\nif (window.addEventListener)");
            //js.Append("\r\n\twindow.addEventListener(\"scroll\", ScrollContent_" + this.ID + ", false);");
            //js.Append("\r\nelse if (window.attachEvent)");
            //js.Append("\r\n\twindow.attachEvent(\"onscroll\", ScrollContent_" + this.ID + "); ");
 

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
            Page.Response.Write("<div id=\"res_promofile_" + this.ID + "\" class=\"vpPrf\" style=\"display:none;\"></div>");
            Page.Response.Write("<div id=\"res_backgroud_" + this.ID + "\" class=\"vpScheduleResultFilterWebControlBackgroud\" onclick=\"CloseDivs_" + this.ID + "();\" style=\"display:none;\"></div>");         
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
            html.Append(GetPromoFileJavascript());
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
            TNS.AdExpress.Domain.Web.Navigation.Module _module = ModulesList.GetModule(WebConstantes.Module.Name.VP);
            var param = new object[1] { _webSession };
            var vpScheduleResult = (IVeillePromo)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + _module.CountryRulesLayer.AssemblyName, _module.CountryRulesLayer.Class,
                false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            vpScheduleResult.ResultControlId = this.ID;
            vpScheduleResult.Theme = _themeName;
            return (vpScheduleResult.GetHtml());
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
        /// Get VP Promotion file HTML  code
        /// </summary>
        /// <param name="o">Result parameters (session Id, theme...)</param>
        /// <returns>Code HTML</returns>
        [AjaxPro.AjaxMethod]
        public virtual object[] GetPromoFileData(string idSession, long idDataPromotion, AjaxPro.JavaScriptObject resultParameters, AjaxPro.JavaScriptObject styleParameters)
        {
            object[] tab = new object[3];

            try
            {
                

                string html;
                LoadResultParameters(resultParameters);
                LoadStyleParameters(styleParameters);
                _webSession = (WebSession)WebSession.Load(idSession);

                TNS.AdExpress.Domain.Web.Navigation.Module _module = ModulesList.GetModule(WebConstantes.Module.Name.VP);
                var param = new object[2] { _webSession, idDataPromotion };
                var vpScheduleResult = (IVeillePromo)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                    + @"Bin\" + _module.CountryRulesLayer.AssemblyName, _module.CountryRulesLayer.Class
                    , false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                vpScheduleResult.Theme = _themeName;
                vpScheduleResult.ResultControlId = this.ID;

                Dictionary<string, List<string>> visualList = vpScheduleResult.GetPromoFileList();
                if (visualList != null && visualList.ContainsKey("p"))
                    tab[1] = (visualList != null && visualList.ContainsKey("p")) ? string.Join(",", visualList["p"].ToArray()) : "";
                if (visualList != null && visualList.ContainsKey("pc"))
                    tab[2] = (visualList != null && visualList.ContainsKey("pc")) ? string.Join(",", visualList["pc"].ToArray()) : "";

                html = vpScheduleResult.GetPromoFileHtml();
                tab[0] = html;
               
               
            }
            catch (System.Exception err)
            {
                throw new Exception (OnAjaxMethodError(err, _webSession), err);
            }
            return (tab);
        }
    }
}

