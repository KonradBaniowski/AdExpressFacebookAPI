using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TNS.AdExpress.Web.Controls.Results.VehicleView{
    /// <summary>
    /// Information about vehicle cover link item
    /// </summary>
    public class CoverLinkItemWebControl : WebControl{

        #region Variables
        /// <summary>
        /// Session Id
        /// </summary>
        protected string _idSession = string.Empty;
        /// <summary>
        /// Vehicle Id
        /// </summary>
        protected long _mediaId = 0;
        /// <summary>
        /// Date media num
        /// </summary>
        protected string _dateMediaNum = string.Empty;
        /// <summary>
        /// Date cover num
        /// </summary>
        protected string _dateCoverNum = string.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_idSession">Session Id</param>
        /// <param name="_mediaId">Vehicle Id</param>
        /// <param name="_dateMediaNum">Date media num</param>
        /// <param name="_dateCoverNum">Date cover num</param>
        public CoverLinkItemWebControl(string idSession, long mediaId, string dateMediaNum, string dateCoverNum){
            _idSession = idSession;
            _mediaId = mediaId;
            _dateMediaNum = dateMediaNum;
            _dateCoverNum = dateCoverNum;
        }
        #endregion

        #region Render
        /// <summary>
        /// Render
        /// </summary>
        /// <returns>Html code</returns>
        virtual public string Render() {

            return "onclick=\"javascript:portofolioDetailMedia('" + _idSession + "','" + _mediaId + "','" + _dateMediaNum + "','"+_dateCoverNum+"');\"";
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
