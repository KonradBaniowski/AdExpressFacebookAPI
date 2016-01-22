using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.VehicleView;
using TNS.FrameWork.WebResultUI;
using AbstractResult = TNS.AdExpressI.Portofolio;
using CoverLinkItemSynthesis = TNS.AdExpressI.Portofolio.Ireland.VehicleView.CoverLinkItemSynthesis;

namespace TNS.AdExpressI.Portofolio.Ireland {
    /// <summary>
    /// Ireland Portofolio class result
    /// </summary>
    public class PortofolioResults : AbstractResult.PortofolioResults {

        #region Constantes
        /// <summary>
        /// Cover File Name
        /// </summary>
        const string COVERFILE_NAME = "coe001.jpg";
        /// <summary>
        /// Magazine Folder Name
        /// </summary>
        const string MAGAZINE_FOLDER_NAME = "magazine";
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        public PortofolioResults(WebSession webSession)
            : base(webSession) {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        public PortofolioResults(WebSession webSession, string adBreak, string dayOfWeek)
            : base(webSession, adBreak, dayOfWeek) {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="periodBeginning">Period Beginning</param>
        /// <param name="periodEnd">period end</param>
        /// <param name="tableType">tableType</param>
        public PortofolioResults(WebSession webSession, TNS.AdExpress.Constantes.DB.TableType.Type tableType)
            : base(webSession, tableType) {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>		
        /// <param name="resultType">resultType</param>
        public PortofolioResults(WebSession webSession, int resultType)
            : base(webSession, resultType) {
        }
        #endregion

        #region Get Result Table
        /// <summary>
        /// Get ResultTable for some portofolio result
        ///  - DETAIL_PORTOFOLIO
        ///  - CALENDAR
        ///  - SYNTHESIS (only result table)
        /// </summary>
        /// <param name="webSession">Customer session</param>
        /// <returns>Result Table</returns>
        public override ResultTable GetResultTable() {
            Engines.SynthesisEngine result = null;
            try {
                switch (_webSession.CurrentTab) {
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
                        return base.GetResultTable();
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
                        result = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
                        break;
                    default:
                        throw (new PortofolioException("Impossible to identified current tab "));
                }
            }
            catch (System.Exception err) {
                throw (new PortofolioException("Impossible to compute portofolio results", err));
            }

            return result.GetResultTable();
        }
        #endregion

        /// <summary>
        /// Get media insertion detail
        /// </summary>
        /// <returns></returns>
        public override ResultTable GetInsertionDetailResultTable(bool excel) {
            var result = new Engines.InsertionDetailEngine(_webSession, _vehicleInformation,
                _idMedia, _periodBeginning, _periodEnd, _adBreak, _dayOfWeek, excel);
            return result.GetResultTable();
        }

        #region Get visual list
        /// <summary>
        /// Get visual list
        /// </summary>
        /// <param name="beginDate">begin Date</param>
        /// <param name="endDate"></param>
        /// <returns>Visual List</returns>
        public override Dictionary<string, string> GetVisualList(string beginDate, string endDate) {
            var dic = new Dictionary<string, string>();

            if (_webSession.CustomerLogin.ShowVehiclePages(_vehicleInformation.Id)) {
                if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
                var parameters = new object[5];
                parameters[0] = _webSession;
                parameters[1] = _vehicleInformation;
                parameters[2] = _idMedia;
                parameters[3] = beginDate;
                parameters[4] = endDate;
                string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
                var portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                    , AppDomain.CurrentDomain.BaseDirectory, _module.CountryDataAccessLayer.AssemblyName),
                  _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance
                   | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);

                var ds = portofolioDAL.GetListDate(true, _tableType);

                foreach (DataRow dr in ds.Tables[0].Rows) {
                    if (dr["media"] != DBNull.Value) {
                        dic.Add(dr["date_media_num"].ToString(), string.Format("{0}/{1}/{2}/{3}", CreationServerPathes.IMAGES, _idMedia,
                                                                               dr["date_media_num"].ToString(), COVERFILE_NAME));
                    }
                    else
                        dic.Add(dr["date_media_num"].ToString(), string.Format("/App_Themes/{0}/Images/Culture/Others/no_visuel.gif", themeName));
                }
            }
            return dic;
        }
        #endregion

        #region GetVehicleItems
        /// <summary>
        /// Get vehicle cover items
        /// </summary>
        /// <returns>cover items</returns>
        public override List<VehicleItem> GetVehicleItems() {
            #region Variables

            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
            var sb = new StringBuilder(5000);
            CoverItem coverItem = null;
            VehicleItem vehicleItem = null;
            var itemsCollection = new List<VehicleItem>();

            #endregion

            if (_vehicleInformation.Id == Vehicles.names.press) {


                Hashtable htValue = null;
                DataTable dtVisuel = null;
                bool showVehiclePages = _webSession.CustomerLogin.ShowVehiclePages(_vehicleInformation.Id);

                if (showVehiclePages || _resultType == AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA) {
                    var parameters = new object[1];
                    parameters[0] = _webSession;
                    var portofolioResult =
                        (IPortofolioResults)
                        AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(
                            AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryRulesLayer.AssemblyName,
                            _module.CountryRulesLayer.Class, false,
                            BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters,
                            null, null);

                    portofolioResult.GetVehicleViewData(out dtVisuel, out htValue);
                }


                var cultureInfo =
                    new CultureInfo(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Localization);

                if (dtVisuel != null) {
                    for (int i = 0; i < dtVisuel.Rows.Count; i++) {
                        var dayDT = new DateTime(int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(0, 4)),
                             int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(4, 2)),
                             int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(6, 2)));


                        coverItem = GetCoverItem(dtVisuel, i, themeName);


                        if (htValue.Count > 0) {
                            if (htValue.ContainsKey(dtVisuel.Rows[i]["date_media_num"])) {
                                vehicleItem = new VehicleItem(dayDT,
                                                              ((string[])
                                                               htValue[dtVisuel.Rows[i]["date_media_num"]])[1],
                                                              int.Parse(
                                                                  ((string[])
                                                                   htValue[dtVisuel.Rows[i]["date_media_num"]])[0])
                                                                 .ToString("### ### ### ###"),
                                                              _webSession.SiteLanguage, coverItem);
                            }
                            else {
                                vehicleItem = new VehicleItem(dayDT, "0", "0", _webSession.SiteLanguage, coverItem);

                            }
                            vehicleItem.ShowCover = showVehiclePages;
                        }

                        itemsCollection.Add(vehicleItem);

                    }
                }


            }



            return itemsCollection;
        }

        protected virtual CoverItem GetCoverItem(DataTable dtVisuel, int i, string themeName) {
            CoverItem coverItem = null;
            string pathWeb;
            if (dtVisuel.Rows[i]["media"] != DBNull.Value) {
                pathWeb = string.Format("{0}/{1}/{2}/{3}", CreationServerPathes.IMAGES, _idMedia,
                                        dtVisuel.Rows[i]["date_media_num"].ToString(), COVERFILE_NAME);
            }
            else {
                pathWeb = "/App_Themes/" + themeName + "/Images/Culture/Others/no_visuel.gif";
            }


            if (dtVisuel.Rows[i]["media"] != DBNull.Value) {
                if (_resultType == AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS) {
                    var coverLinkItemSynthesis = new CoverLinkItemSynthesis(dtVisuel.Rows[i]["media"].ToString(),
                                                                            string.Empty,
                                                                            _webSession.IdSession, _idMedia,
                                                                            dtVisuel.Rows[i]["date_media_num"].ToString(),
                                                                            dtVisuel.Rows[i]["date_media_num"].ToString(),
                                                                            MAGAZINE_FOLDER_NAME);
                    coverItem = new CoverItem(i + 1,
                                              GestionWeb.GetWebWord(1409, _webSession.SiteLanguage),
                                              pathWeb, coverLinkItemSynthesis);
                }
                else if (_resultType == AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA) {
                    var coverLinkItem = new CoverLinkItem(_webSession.IdSession, _idMedia,
                                                          dtVisuel.Rows[i]["date_media_num"].ToString(), string.Empty);
                    coverItem = new CoverItem(i + 1, string.Empty, pathWeb, coverLinkItem);

                }
            }
            else if (_resultType == AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS)
                coverItem = new CoverItem(i + 1, GestionWeb.GetWebWord(1409, _webSession.SiteLanguage),
                                          pathWeb, null);
            else if (_resultType == AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA)
                coverItem = new CoverItem(i + 1, string.Empty, pathWeb, null);
            return coverItem;
        }

        #endregion

    }
}
