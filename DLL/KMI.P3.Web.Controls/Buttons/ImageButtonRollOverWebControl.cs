using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KMI.P3.Web.Controls.Buttons
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ImageButtonRollOverWebControl runat=server></{0}:ImageButtonRollOverWebControl>")]
    public class ImageButtonRollOverWebControl : System.Web.UI.WebControls.LinkButton 
    {

        #region Accesseurs

        /// <summary>
        /// Specifies the image url for the button when in its normal state
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(""),
        Description("Specifies the image url for the button when in its normal state.")]
        public string ImageUrl
        {
            get
            {
                object o = ViewState["RollOverImageUrl"];
                if (o != null)
                    return (string)o;
                else
                    return String.Empty;
            }

            set
            {
                ViewState["RollOverImageUrl"] = value;
            }
        }

        /// <summary>
        /// Specifies the image url for the button when the mouse is hovered above the button
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(""),
        Description("Specifies the image url for the button when the mouse is hovered above the button.")]
        public string RollOverImageUrl
        {
            get
            {
                object o = ViewState["RollOverRollOverImageUrl"];
                if (o != null)
                    return (string)o;
                else
                    return String.Empty;
            }

            set
            {
                ViewState["RollOverRollOverImageUrl"] = value;
            }
        }

        #endregion

        #region Evènements

        /// <summary>
        /// PréRendu.
        /// On génère le code JavaScript
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e)
        {
            string imageLoadScriptKey = "rolloverImageBuildingCode" + this.ClientID;
            string script =
                @"<script language=""JavaScript"">
					<!--
						" + this.ClientID + "_img_out = new Image(); " +
                this.ClientID + @"_img_out.src = """ + this.ImageUrl + @""";
						" + this.ClientID + "_img_over = new Image(); " +
                this.ClientID + @"_img_over.src = """ + this.RollOverImageUrl + @""";
					// -->
				  </script>";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), imageLoadScriptKey, script);

            this.Attributes["onmouseover"] = "rolloverServerControl_display('" + this.ClientID + "_img'," + this.ClientID + "_img_over);";
            this.Attributes["onmouseout"] = "rolloverServerControl_display('" + this.ClientID + "_img'," + this.ClientID + "_img_out);";

            const string displayScriptKey = "rolloverImageLoadScript";
            if (!Page.ClientScript.IsClientScriptBlockRegistered(displayScriptKey))
            {
                script = @"<script language=""JavaScript"">
				<!--
					function rolloverServerControl_display(imgName, imgUrl) {
						if (document.images && typeof imgUrl != 'undefined')
							document[imgName].src = imgUrl.src;
					}
				// -->
				</script>";

                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), displayScriptKey, script);
            }

            base.OnPreRender(e);
        }


        /// <summary>
        /// Rendu
        /// </summary>
        /// <param name="writer">Flux HTML</param>
        protected override void RenderContents(HtmlTextWriter writer)
        {


            if (this.ImageUrl != String.Empty)
            {
                Image myImage = new Image();
                myImage.Attributes["name"] = this.ClientID + "_img";
                myImage.ImageUrl = this.ImageUrl;
                myImage.AlternateText = this.Text;
                myImage.Height = this.Height;
                myImage.Width = this.Width;
                myImage.ToolTip = this.ToolTip;
                myImage.RenderControl(writer);
            }
            else
                base.RenderContents(writer);
        }

        #endregion

    }
}
