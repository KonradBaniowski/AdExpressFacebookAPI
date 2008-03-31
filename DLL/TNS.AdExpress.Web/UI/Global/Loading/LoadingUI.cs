#region Information
/*
 * auteur : G Facon
 * créé le :
 * modifié le : 22/07/2004
 * par : Guillaume Ragneau
 * */
#endregion

using System;
using System.Text;
using System.Web.UI;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.UI.Global.Loading{
	/// <summary>
	/// Gestion du flash d'attente
	/// </summary>
	public class LoadingUI{

		/// <summary>
		/// Enregistre le code flash dans la page et renvoie le code html d'affichage
		/// </summary>
		/// <param name="language">Langue du flash</param>
		/// <param name="page">Page recevant le code</param>
		/// <returns>Code HTML généré</returns>
		internal static string GetHtmlDiv(int language, Page page){

			StringBuilder t=new StringBuilder(1000);

			#region Enregistrement du script
//			if (!page.ClientScript.IsClientScriptBlockRegistered("detectFlash")){
//				string tmp = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/scripts/FlashChecking.js\"></SCRIPT>";
//				page.ClientScript.RegisterClientScriptBlock(this.GetType(),"detectFlash","");
//				t.Append(tmp);
//			}
//			#endregion
//
//			#region Construction du code html
//			t.Append("\n<div id=\"loading\" style=\"BORDER-RIGHT: #644883 0px solid; BORDER-LEFT: #644883 0px solid; WIDTH: 100%; BORDER-BOTTOM: #644883 0px solid; BACKGROUND-COLOR: #644883; text-align:center;\">");
//
//			t.Append("\n<script language=\"JavaScript\">");
//
//			t.Append("\nif (hasRightFlashVersion==true){");
//			t.Append("\ndocument.write(\"<OBJECT id='Object1' codeBase='http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0'");
//			t.Append(" height='170' width='370' classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000' VIEWASTEXT>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='_cx' VALUE='19394'>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='_cy' VALUE='12806'>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='FlashVars' VALUE=''>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='Movie' VALUE='/Flash/"+language.ToString()+"/loading.swf'>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='Src' VALUE='/Flash/"+language.ToString()+"/loading.swf'>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='WMode' VALUE='Window'>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='Play' VALUE='-1'>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='Loop' VALUE='-1'>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='Quality' VALUE='High'>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='SAlign' VALUE=''>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='Menu' VALUE='0'>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='Base' VALUE=''>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='AllowScriptAccess' VALUE='always'>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='Scale' VALUE='ShowAll'>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='DeviceFont' VALUE='0'>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='EmbedMovie' VALUE='0'>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='BGColor' VALUE=''>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='SWRemote' VALUE=''>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='MovieData' VALUE=''>\");");
//			t.Append("\ndocument.write(\"<PARAM NAME='SeamlessTabbing' VALUE='1'>\");");
//			t.Append("\ndocument.write(\"<embed src='/Flash/"+language.ToString()+"/loading.swf' quality='high' pluginspage='http://www.macromedia.com/go/getflashplayer'");
//			t.Append(" type='application/x-shockwave-flash' height='170' width='370'> </embed>\");");
//			t.Append("\ndocument.write(\"</OBJECT>\");");
//			t.Append("\n}");
//
//			t.Append("\nelse{");
//			t.Append("\ndocument.write(\"<Img height='170' width=370' src='/Images/"+language.ToString()+"/FlashReplacement/loadingFlash.GIF'>\");");
//			t.Append("\n}");
//
//			t.Append("\n</script>\n</div>");
			#endregion

            t.Append("\r\n<LINK href=\"/App_Themes/"+WebApplicationParameters.Themes[language].Name+"/Css/Loading.css\" type=\"text/css\" rel=\"stylesheet\">");
			t.Append("\r\n<DIV id=waitDiv style=\"LEFT: 40%; VISIBILITY: hidden; POSITION: absolute; TOP: 50%\">");
			t.Append("\r\n<DIV align=center>");
            t.Append("\r\n\t<TABLE cellPadding=6 class=\"violetBorderColorLoading greyBackGroundLoading\" border=2 >");
			t.Append("\r\n\t\t<TR>");
            t.Append("\r\n\t\t\t<TD align=middle class=\"violetBorderLoading\"><FONT face=\"Arial, Helvetica, sans-serif\" class=\"txtVioletLoading\" size=3><B>" + GestionWeb.GetWebWord(1911, language) + "</B></FONT> ");
			t.Append("\r\n\t\t\t<BR><IMG height=20 src=\"/App_Themes/"+WebApplicationParameters.Themes[language].Name+"/Images/Common/await.gif\" width=200 border=0> ");
			//t.Append("\r\n\t\t\t<BR><FONT face=\"Verdana, Arial, Helvetica, sans-serif\" color=#000000 size=2>Veuillez Patienter s'il vous plait</FONT>");
			t.Append("\r\n\t\t\t</TD>");
			t.Append("\r\n\t\t</TR>");
			t.Append("\r\n\t</TABLE>");
			t.Append("\r\n</DIV>");
			t.Append("\r\n</DIV>");
			t.Append("\r\n<SCRIPT language=javascript> ");
			t.Append("\r\n<!-- ");
			t.Append("\r\nvar DHTML = (document.getElementById || document.all || document.layers);");
			t.Append("\r\n function ap_getObj(name) { ");
			t.Append("\r\n\t if (document.getElementById) { ");
			t.Append("\r\n\t\t return document.getElementById(name).style; ");
			t.Append("\r\n\t } ");
			t.Append("\r\n\t else if (document.all) { ");
			t.Append("\r\n\t\t return document.all[name].style;");
			t.Append("\r\n\t} ");
			t.Append("\r\n\t else if (document.layers) { ");
			t.Append("\r\n\t\t return document.layers[name]; ");
			t.Append("\r\n\t }"); 
			t.Append("\r\n }");
			t.Append("\r\n function ap_showWaitMessage(div,flag)  {");
			t.Append("\r\n\t\t if (!DHTML) return;");
			t.Append("\r\n\t\t var x = ap_getObj(div);");
			t.Append("\r\n\t\t x.visibility = (flag) ? 'visible':'hidden'");
			t.Append("\r\n\t\t if(! document.getElementById) ");
			t.Append("\r\n\t\t\t if(document.layers) x.left=280/2; ");
			t.Append("\r\n\t\t\t return true; ");
			t.Append("\r\n\t} ");
			t.Append("\r\n ap_showWaitMessage('waitDiv', 1);  ");
			t.Append("\r\n //--> ");
			t.Append("\r\n </SCRIPT>");
			t.Append("\r\n <!-- fin wait script -->");


			return(t.ToString());
		}
		
		/// <summary>
		/// Construit le script de fermeture du flash d'attente
		/// </summary>
		/// <returns>Code html de la fermeture du flash</returns>
		internal static string GetHtmlCloseDiv(){
			//return("<script language=\"javascript\">loading.style.display=\"none\";</script>");
			return("\r\n<script language=\"javascript\">ap_showWaitMessage('waitDiv', 0);</script>\r\n");
		}


	}
}
