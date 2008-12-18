using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using WebFunctions = TNS.AdExpress.Web.Functions.Script;

namespace TNS.AdExpress.Web.Controls.Results{
	/// <summary>
	/// ImageDropDownListWebControl est une dropDownList permettant d'afficher des images
	/// utilisation : placé une variable dans la partie d entete de la page aspx et initialiser cette variable
	/// avec les fonctions javascript TNS.AdExpress.Web.Functions.Scripts.ImageDropDownListScripts(...)
	/// </summary>
	[DefaultProperty("List"),ToolboxData("<{0}:ImageDropDownListWebControl runat=server></{0}:ImageDropDownListWebControl>")]
	public class ImageDropDownListWebControl : System.Web.UI.WebControls.WebControl {

		#region Propriétés
		internal string texts = "";
        internal string images = "";
        internal int index;
        internal bool pictShow;
        internal string outCssClass = "";
        internal string overCssClass = "";
        internal double imageWidth = 15.0;
        internal double imageHeight = 15.0;
        /// <summary>
        /// Arrow button image
        /// </summary>
        internal string imageButtonArrow = "";

		/// <summary>
		/// Obtient ou définit la hauteur de l'image
		/// </summary>
		[Bindable(true),Category("Appearance")]
		public double ImageHeight{
			get{return imageHeight;}
			set{imageHeight = value;}
		}

		/// <summary>
		/// Obtient ou définit la largeur
		/// </summary>
		[Bindable(true),Category("Appearance")]
		public double ImageWidth{
			get{return imageWidth;}
			set{imageWidth = value;}
		}

		/// <summary>
		/// Obtient ou définit si l'image doit être montrée
		/// </summary>
		[Bindable(true),Category("Appearance")]
		public bool ShowPictures{
			get{return pictShow;}
			set{pictShow = value;}
		}
		/// <summary>
		/// Obtient ou définit la classe Css à utiliser
		/// </summary>
		[Bindable(true),Category("Appearance")]
		public string OutCssClass{
			get{return outCssClass;}
			set{outCssClass = value;}
		}

		/// <summary>
		/// 
		/// </summary>
		[Bindable(true),Category("Appearance")]
		public string OverCssClass{
			get{return overCssClass;}
			set{overCssClass = value;}
		}

		/// <summary>
		/// 
		/// </summary>
		[Bindable(true), Category("Content"),DefaultValue("")]
		public string List{
			get{return texts;}
			set{texts = value;}
		}

		/// <summary>
		/// 
		/// </summary>
		[Bindable(true), Category("Content"),DefaultValue("")]
		public string Images{
			get{return images;}
			set{images = value;}
		}

		/// <summary>
		/// 
		/// </summary>
		[Bindable(true),Category("Content")]
		public int ListIndex{
			get{return index;}
			set{index = Math.Min(value, texts.Split('|').Length - 1);}
		}

        /// <summary>
        /// Set or Get image button arrow
        /// </summary>
        public string ImageButtonArrow {
            get { return imageButtonArrow; }
            set { imageButtonArrow = value; }
        }
		#endregion

		#region PreRender

		/// <summary>
		/// PreRender
		/// </summary>
		/// <param name="e">Arguements</param>
		protected override void OnPreRender(EventArgs e) {

			if (!Page.ClientScript.IsClientScriptBlockRegistered("CommonImageDropDownListScripts"))
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"CommonImageDropDownListScripts","");
			base.OnPreRender(e);

		}


		#endregion

		#region Render

		/// <summary>
		/// Render
		/// </summary>
		/// <param name="writer">Sortie</param>
		protected override void Render(HtmlTextWriter writer) {
			
			#region Variables locales
			int i = 0;
			string img = "";
			//string className = "";
			
			string bgColor = ColorToStr(this.BackColor);
			string borderColor = ColorToStr(this.BorderColor);
			double borderWidth = Math.Max(1, this.BorderWidth.Value);

			string[] textList = texts.Split('|');
			string[] imageList = images.Split('|');

			#endregion

			#region Génération du code HTML

			#region Entete combolist
            writer.Write("\n\t <input class=\"" + OutCssClass + "\" type=\"hidden\" name=\"" + this.ID + "\"  id=\"" + this.ID + "\" value=\"" + index + "\">");
			writer.Write("\n\t <div>");
			writer.Write("\n\t\t <table width=\"" + this.Width + "\" border=\"0\" cellSpacing=\"0\">");
			writer.Write("\n\t\t\t <tr>");
			writer.Write("\n\t\t\t <td><table width=\"100%\" border=\"0\" cellPadding=\"0\" cellSpacing=\"0\"" + 
				" style=\"border: " + borderWidth + "px solid " + borderColor + "; cursor: hand;\""+
				" onClick=\"javascript:ShowScrollerImageDDL('" + this.ID + "');\" bgColor=\"" + bgColor + "\">");

			writer.Write("\n\t\t\t\t <tr>");

			if (index>-1){
				try{
					if (imageList.GetUpperBound(0) >= index)
						img = imageList[index];
					else
                        img = "/Images/Common/pixel.gif";
				}
				catch(System.Exception){
                    img = "/Images/Common/pixel.gif";
				}

				writer.Write("\n\t\t\t\t\t <td  height=\""+ (imageHeight+4.0)+"\" class=\"" + OutCssClass + "\" valign=\"middle\" nowrap>");
				if (pictShow) writer.Write("&nbsp;<img class=\"" + OutCssClass + "\" id=\"ComboImg_" + this.ID + "\" src=\"" + img + "\" width=\""+ imageWidth + "\" height=\""+ imageHeight + "\" align=\"absmiddle\">");
				writer.Write("<label id=\"ComBoText_" + this.ID + "\">" + textList[index] + "</label>");
                writer.Write("<img class=\"" + OutCssClass + "\" src=\"/Images/Common/pixel.gif\" width=\"5\" align=\"absmiddle\"></td>");
			}
			else{
                writer.Write("\n\t\t\t\t\t <td nowrap><img class=\"" + OutCssClass + "\" src=\"/Images/Common/pixel.gif\" witdh=\"2\" align=\"absmiddle\">");
                if (pictShow) writer.Write("<img class=\"" + OutCssClass + "\" id=\"ComboImg_\"" + this.ID + "\" src=\"/Images/Common/pixel.gif\" width=\"15\" height=\"15\" align=\"absmiddle\">");
				writer.Write("<label id=\"ComBoText_" + this.ID + "></label>");
                writer.Write("<img class=\"" + OutCssClass + "\" src=\"/Images/Common/pixel.gif\" width=\"2\" align=\"absmiddle\"></td>)");
			}
			writer.WriteLine("\t\t\t\t\t <td height=\""+ (imageHeight+4.0)+"\" valign=\"middle\" ><img class=\"" + OutCssClass + "\" src=\""+imageButtonArrow+"\" border=\"0\"></td>");
			writer.WriteLine("\t\t\t\t </tr>");
			writer.WriteLine("\t\t\t </table></td>");
			writer.WriteLine("\t\t\t </tr>");
			
			#endregion

			#region Partie scrollable
			writer.WriteLine("\t\t\t <tr> ");
			writer.WriteLine("\t\t\t <td><div id=\"scroller_" + this.ID + "\" style=\"display: none; overflow: visible; background-color: " + bgColor + "; layer-background-color: " + bgColor + "; border: 1px none #000000; position:absolute;\">");
			writer.WriteLine("\t\t\t <table width=\"100%\" border=\"0\" cellPadding=\"1\" cellSpacing=\"0\" style=\"border: " + borderWidth + "px solid " + borderColor + "; cursor: hand;\">");

			//Les items
			for (i=0; i<= textList.GetUpperBound(0); i++){
				try{
					if (imageList.GetUpperBound(0) >= i){
						//Une image est définie pour le controle en cours
						writer.WriteLine("<tr id=\"scroller_item_" +this.ID + "_" + i + "\" class=\"" + OutCssClass + "\" onMouseOver=\"ChangeItemClassDDL(this, '" + OverCssClass + "');\" onMouseOut=\"ChangeItemClassDDL(this, '" + OutCssClass + "');\" onClick=\"ItemClickDDL('" + this.ID + "',this, " + i + ", '" + textList[i].Replace("'", "\'") + "', '" + imageList[i] + "');\">");
						if (pictShow){
							writer.WriteLine("<td align=\"center\" valign=\"middle\" width=\""+ (imageWidth+2.0) + "\" height=\""+ (imageHeight+4) + "\">");
							writer.WriteLine("<img src=\"" + imageList[i] + "\" width=\""+ imageWidth + "\" height=\""+ imageHeight + "\" align=\"absmiddle\">");
							writer.WriteLine("</td>");
						}
						else{
							throw new System.Exception();
						}
					}
				}
				catch(System.Exception){
					//Aucune image n'est définie, donc on y met un pixel transparent
                    writer.WriteLine("<tr id=\"scroller_item_" + this.ID + "_" + i + "\" class=\"" + OutCssClass + "\" onMouseOver=\"ChangeItemClassDDL(this, '" + OverCssClass + "')\" onMouseOut=\"ChangeItemClassDDL(this, '" + OutCssClass + "')\" onClick=\"ItemClickDDL('" + this.ID + "',this, " + i + ", '" + textList[i].Replace("'", "\'") + "', '/Images/Common/pixel.gif')\">");
					if (pictShow) {
						writer.WriteLine("<td width=\"1\">");
                        writer.WriteLine("<img src=\"/Images/Common/pixel.gif\" height=\"15\" align=\"absmiddle\">");
						writer.WriteLine("</td>");
					}
				}
				writer.WriteLine("<td width=\"22px\" nowrap>" + textList[i] + "</td>");
				writer.WriteLine("</tr>");
			}
			//Fermeture des balises --
			writer.WriteLine("</table>");
			writer.WriteLine("</div></td>");
			writer.WriteLine("</tr>");

			#endregion

			writer.WriteLine("</table>");
			writer.WriteLine("</div>");
			
			#endregion

		}


		#endregion

		#region Méthodes internes
		/// <summary>
		/// Convertit une couleur en couleur HTML
		/// </summary>
		/// <param name="color">Couleur à convertire</param>
		/// <returns>Couleur HTMl</returns>
		internal string ColorToStr(System.Drawing.Color color){
			string s;
			if (!color.IsEmpty){
				return "#" + (s = "000000" + color.ToArgb().ToString("X")).Substring(s.Length-6);
			}
			return "#FFFFFF";
		}

		#endregion

	}
}
