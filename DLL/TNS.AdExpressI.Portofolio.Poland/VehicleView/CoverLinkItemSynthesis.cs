using TNS.AdExpressI.Portofolio.VehicleView;

namespace TNS.AdExpressI.Portofolio.Poland.VehicleView
{ /// <summary>
    /// Information about vehicle cover link item for synthesis tab
    /// </summary>
    public class CoverLinkItemSynthesis : Portofolio.VehicleView.CoverLinkItemSynthesis
    {
        /// <summary>
        /// sub folder
        /// </summary>
        protected string _subFolder = string.Empty;

        #region Constructor
        public CoverLinkItemSynthesis(string media, string numberPageMedia, string idSession, long mediaId,
            string dateMediaNum, string dateCoverNum, string subFolder)
            : base(media, numberPageMedia, idSession, mediaId,dateMediaNum,dateCoverNum)
        {
            _subFolder = subFolder;               
        }
        #endregion

        #region Render
        /// <summary>
        /// Render
        /// </summary>
        /// <returns>Html code</returns>
        override public string Render(){

            return string.Format("onclick=\"javascript:portofolioCreation2('{0}','{1}','{2}','{3}','{4}','{5}','{6}');\""
                , _idSession, _mediaId, _dateMediaNum, _dateCoverNum, _media, _numberPageMedia, _subFolder);
        }
     
        #endregion

    }
}
