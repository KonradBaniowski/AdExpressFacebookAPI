using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Functions;

namespace TNS.AdExpressI.Portofolio.Russia.VehicleView
{
    public class CoverLinkItem : Portofolio.VehicleView.CoverLinkItem
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


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="idSession">Session Id</param>
        /// <param name="mediaId">Vehicle Id</param>
        /// <param name="dateMediaNum">Date media num</param>
        /// <param name="dateCoverNum">Date cover num</param>
        /// <param name="src">source</param>
        public CoverLinkItem(string idSession, long mediaId, string dateMediaNum, string dateCoverNum,string src,string src2)  
            : base(idSession, mediaId, dateMediaNum, dateCoverNum)
        {
            _src = src;
            _src2 = src2;
        }

        #region Render
        /// <summary>
        /// Render
        /// </summary>
        /// <returns>Html code</returns>
        public override string Render()
        {
            return string.Format(" \"javascript:OpenWindow('{0}');\"", _src2);            
        }

        #endregion
    }
}
