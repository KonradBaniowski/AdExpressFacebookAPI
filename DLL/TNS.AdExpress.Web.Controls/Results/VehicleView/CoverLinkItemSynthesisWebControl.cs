using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TNS.AdExpress.Web.Controls.Results.VehicleView{
    /// <summary>
    /// Information about vehicle cover link item for synthesis tab
    /// </summary>
    public class CoverLinkItemSynthesisWebControl : CoverLinkItemWebControl{

        #region Variables
        /// <summary>
        /// Vehicle
        /// </summary>
        private string _media = string.Empty;
        /// <summary>
        /// Number page media
        /// </summary>
        private string _numberPageMedia = string.Empty;
        #endregion

        #region Constructor
        public CoverLinkItemSynthesisWebControl(string media, string numberPageMedia, string idSession, long mediaId, string dateMediaNum, string dateCoverNum)
            : base(idSession, mediaId, dateMediaNum, dateCoverNum) {

                _media = media;
                _numberPageMedia = numberPageMedia;
        }
        #endregion

        #region Render
        /// <summary>
        /// Render
        /// </summary>
        /// <returns>Html code</returns>
        override public string Render(){

            return "onclick=\"javascript:portofolioCreation('" + _idSession + "','" + _mediaId + "','" + _dateMediaNum + "','" + _dateCoverNum + "','" + _media + "','" + _numberPageMedia + "');\"";
        }
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="output">output</param>
        protected override void Render(HtmlTextWriter output) {
            output.Write(Render());
        }
        #endregion

    }
}
