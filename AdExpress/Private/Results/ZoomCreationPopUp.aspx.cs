using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

namespace AdExpress.Private.Results{
	/// <summary>
	/// Popup affichant les créations presse en gd format
	/// La fenêtre prend en argument dans l'url la liste des fichiers dans un variable création
	/// </summary>
	public partial class ZoomCreationPopUp : TNS.AdExpress.Web.UI.WebPage{

		#region Variables
		/// <summary>
		/// Code résultat
		/// </summary>
		public string result="";		
		#endregion

		#region Evènements

		#region Chargement de la page
		protected void Page_Load(object sender, System.EventArgs e)
		{
			string[] files=Page.Request.QueryString.Get("creation").Split(',');

            bool flash = false;
            result = "<TABLE class=\"whiteBackGround\" cellPadding=\"0\" cellSpacing=\"5\" align=\"center\" valign=\"center\" border=\"0\">";

            Regex f = new Regex(@"\.swf");
           
            int i=0;
			for(i=0 ; i<files.Length; i++){
                if (f.IsMatch(files[i]))
                {
                  
                    // Flash banner
                    result = string.Format("\n <OBJECT classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://active.macromedia.com/flash5/cabs/swflash.cab#version=5,0,0,0\" width=\"100%\" height=\"100%\">");
                    result += string.Format("\n <PARAM name=\"movie\" value=\"{0}\">", files[i]);
                    result += string.Format("\n <PARAM name=\"play\" value=\"true\">");
                    result += string.Format("\n <PARAM name=\"quality\" value=\"high\">");
                    result += string.Format("\n <EMBED src=\"{0}\" play=\"true\" swliveconnect=\"true\" quality=\"high\" width=\"100%\" height=\"100%\">",
                          files[i]);
                    result += string.Format("\n </OBJECT>");
                    flash = true;
                   

                }
                else
                {
                    if ((i % 2) == 0)
                    {
                        result += "<tr><td><img src=\"" + files[i] + "\"></td>";
                    }
                    else
                    {
                        result += "<td><img src=\"" + files[i] + "\"></td></tr>";
                    }
                }
			}
			if ((i%2)==1 &&  !flash){
				result+="</tr>";
			}
			//Agrandi l'ecran pour une affiche publicitaire

            if (i < 2 && files != null && files.Length > 0 && (files[0].IndexOf(TNS.AdExpress.Constantes.Web.CreationServerPathes.IMAGES_OUTDOOR) > -1 || files[0].IndexOf(TNS.AdExpress.Constantes.Web.CreationServerPathes.IMAGES_INSTORE) > -1 && !flash)) i = 2;
			if( !flash)result+="</table><input type=hidden value=\""+i+"\" id=\"fileNb\">";
        }
		#endregion

		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{
           
		}
		#endregion
	}
}
