using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpressI.Portofolio.Russia.VehicleView
{ /// <summary>
    /// Information about vehicle cover link item for synthesis tab
    /// </summary>
    public class CoverLinkItemSynthesis : TNS.AdExpressI.Portofolio.VehicleView.CoverLinkItemSynthesis
    {
         #region Variables
        /// <summary>
        /// src attribute 
        /// </summary>
        protected string _src = string.Empty;
        /// <summary>
        /// src attribute 
        /// </summary>
        protected string _src2 = string.Empty;
        #endregion

        #region Constructor
        public CoverLinkItemSynthesis(string media, string numberPageMedia, string idSession, long mediaId, string dateMediaNum, string dateCoverNum, string src,string src2) 
            : base(media, numberPageMedia, idSession,  mediaId, dateMediaNum, dateCoverNum)
        {
            _src = src;
            _src2 = src2;
        }
        #endregion

        #region Render
        /// <summary>
        /// Render
        /// </summary>
        /// <returns>Html code</returns>
        override public string Render(){

            return  !string.IsNullOrEmpty(_src2)  ? string.Format(" \"javascript:OpenWindow('{0}');\"", _src2):string.Empty;         
        }
     
        #endregion

    }
}
