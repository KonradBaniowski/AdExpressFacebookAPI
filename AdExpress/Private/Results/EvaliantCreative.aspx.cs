using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

namespace AdExpress.Private.Results
{

    public partial class EvaliantCreative : TNS.AdExpress.Web.UI.WebPage 
    {

        #region Variables
        /// <summary>
        /// Code résultat
        /// </summary>
        public string result = "";
        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {

            string file = Page.Request.QueryString.Get("creation");
            string info = null;
            if (Page.Request.QueryString.Get("info") != null)
            {
                info = Page.Request.QueryString.Get("info");
                Regex reg = new Regex(@"([0-9]*)\*([0-9]*)");
                Match m = reg.Match(info);
                if(m.Success){
                    string[] values = m.Value.Split('*');
                    width.Value = values[0];
                    height.Value = values[1];
                }
            }
           

            result = "<TABLE class=\"whiteBackGround\" cellPadding=\"0\" cellSpacing=\"5\" align=\"center\" valign=\"center\" border=\"0\"><tr><td>";

            Regex f = new Regex(@"\.swf");
            Regex flv = new Regex(@"\.flv");
            if(f.IsMatch(file)){

                // Flash banner
                result += string.Format("\n <OBJECT classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://active.macromedia.com/flash5/cabs/swflash.cab#version=5,0,0,0\" width=\"{0}\" height=\"{1}\">",
                    width.Value,
                    height.Value);
                result += string.Format("\n <PARAM name=\"movie\" value=\"{0}\">", file);
                result += string.Format("\n <PARAM name=\"play\" value=\"true\">");
                result += string.Format("\n <PARAM name=\"quality\" value=\"high\">");
                result += string.Format("\n <EMBED src=\"{0}\" play=\"true\" swliveconnect=\"true\" quality=\"high\" width=\"{1}\" height=\"{2}\">",
                     file,
                    width.Value,
                    height.Value);
                result += string.Format("\n </OBJECT>");

            }
            else if (flv.IsMatch(file)) {
                result +="<object id=\"video\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://active.macromedia.com/flash5/cabs/swflash.cab#version=5,0,0,0\" width=\"400\" height=\"315\">";
                result +="<param name=\"movie\" value=\"/Player/playerflv.swf\" />";
                result +="<param name=\"allowfullscreen\" value=\"true\" />";
                result +="<param name=\"allowscriptaccess\" value=\"always\" />";
                result += "<param name=\"flashvars\" value=\"file=" + file + "\" />";
                result +="<embed type=\"application/x-shockwave-flash\"";
                result +="src=\"/Player/playerflv.swf\" ";
                result +="width=\"400\" ";
                result +="height=\"315\"";
                result +="allowscriptaccess=\"always\" ";
                result +="allowfullscreen=\"false\"";
                result += "flashvars=\"file=" + file + "\" ";
                result +="/>";
                result += "</object>";
            }
            else {

                result += string.Format("<img src=\"{0}\">", file);

            }

            result += "</td></tr></table>";

        }
    }

}