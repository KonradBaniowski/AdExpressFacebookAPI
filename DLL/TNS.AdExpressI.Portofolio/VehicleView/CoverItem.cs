﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpressI.Portofolio.VehicleView
{  /// <summary>
    /// Information about vehicle cover item
    /// </summary>
   public class CoverItem
    {
        #region Variables
        /// <summary>
        /// Image id
        /// </summary>
        private int _id;
        /// <summary>
        /// alt attribute for Html img tag 
        /// </summary>
        private string _alt = string.Empty;
        /// <summary>
        /// src attribute for Html img tag
        /// </summary>
        private string _src = string.Empty;
        /// <summary>
        /// Cover link item
        /// </summary>
        private CoverLinkItem _coverLinkItem = null;
        #endregion

        #region Accessors
        /// <summary>
        /// Get src
        /// </summary>
        public string Src {
            get { return _src; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Image id</param>
        /// <param name="alt">alt attribute for Html img tag</param>
        /// <param name="src">src attribute for Html img tag</param>
        /// <param name="coverLinkItem">Cover link item</param>
        public CoverItem(int id, string alt, string src, CoverLinkItem coverLinkItem)
        {
            _id = id;
            _alt = alt;
            _src = src;
            _coverLinkItem = coverLinkItem;
        }
        #endregion

        #region Render
        /// <summary>
        /// Cover item render
        /// </summary>
        /// <returns>Html code</returns>
        public string Render() {
            if(_coverLinkItem != null)
                return "<img id=\"" + _id + "\" alt=\"" + _alt + "\" src='' width=\"180\" height=\"218\" style=\"cursor : pointer;\" " + _coverLinkItem.Render() + ">";
            else
                return "<img id=\"" + _id + "\" alt=\"" + _alt + "\" src='' width=\"180\" height=\"218\">";
        }
      
        #endregion
    }
}