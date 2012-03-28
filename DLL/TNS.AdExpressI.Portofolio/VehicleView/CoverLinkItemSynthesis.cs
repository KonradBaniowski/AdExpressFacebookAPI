﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpressI.Portofolio.VehicleView
{ /// <summary>
    /// Information about vehicle cover link item for synthesis tab
    /// </summary>
    public class CoverLinkItemSynthesis :CoverLinkItem
    {
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
        public CoverLinkItemSynthesis(string media, string numberPageMedia, string idSession, long mediaId, string dateMediaNum, string dateCoverNum)
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
     
        #endregion

    }
}