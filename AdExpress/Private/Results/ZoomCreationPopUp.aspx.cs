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


            result = "<TABLE class=\"whiteBackGround\" cellPadding=\"0\" cellSpacing=\"5\" align=\"center\" valign=\"center\" border=\"0\">";
			int i=0;
			for(i=0 ; i<files.Length; i++){
				if((i%2)==0){
					result+="<tr><td><img src=\""+files[i]+"\"></td>";
				}
				else{
					result+="<td><img src=\""+files[i]+"\"></td></tr>";
				}
			}
			if ((i%2)==1){
				result+="</tr>";
			}
			//Agrandi l'ecran pour une affiche publicitaire

            if (i < 2 && files != null && files.Length > 0 && (files[0].IndexOf(TNS.AdExpress.Constantes.Web.CreationServerPathes.IMAGES_OUTDOOR) > -1 || files[0].IndexOf(TNS.AdExpress.Constantes.Web.CreationServerPathes.IMAGES_INSTORE) > -1)) i = 2;
			result+="</table><input type=hidden value=\""+i+"\" id=\"fileNb\">";
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
