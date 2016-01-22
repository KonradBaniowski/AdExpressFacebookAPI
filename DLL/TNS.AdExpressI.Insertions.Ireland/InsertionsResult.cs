using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Sessions;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.Insertions.Ireland {
    public class InsertionsResult : TNS.AdExpressI.Insertions.InsertionsResult {

        #region Constructor
        /// <summary>
        /// DEfault constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="moduleId">Module ID</param>
        public InsertionsResult(WebSession session, Int64 moduleId) : base(session, moduleId) {
        }
        #endregion

        #region Is Visual Available
        /// <summary>
        /// Check if visual is available
        /// </summary>
        /// <param name="row">Data  row</param>
        /// <param name="idMedia">Id vehicle</param>
        /// <param name="dateCoverNum">cover date</param>
        /// <param name="dateMediaNum">date</param>
        /// <returns>True if viual is available</returns>
        protected override bool IsVisualAvailable(DataRow row, out long idMedia, out long dateCoverNum, out long dateMediaNum) {
            dateCoverNum = -1;
            dateMediaNum = -1;
            idMedia = -1;

            if (row.Table.Columns.Contains("id_media") && row["id_media"] != DBNull.Value) {
                idMedia = Convert.ToInt64(row["id_media"]);
            }
            /*if (row.Table.Columns.Contains("date_media_num") && row["date_media_num"] != DBNull.Value) {
                dateMediaNum = Convert.ToInt64(row["date_media_num"]);
            }*/
            //return (idMedia > 0 && dateMediaNum > 0);
            return (idMedia > 0);
        }
        #endregion

        #region AddVisuals
        /// <summary>
        /// Add Visuals
        /// </summary>
        /// <param name="row">Data row</param>
        /// <param name="visuals">Visuals list</param>
        /// <param name="getCreativePath">extact creative path</param>
        /*protected override void AddVisuals(DataRow row, List<string> visuals, Func<string, string> getCreativePath) {
            if (row["associated_file"] != DBNull.Value) {
                string[] files = row["associated_file"].ToString().Split(',');
                string year = row["dateParution"].ToString().Substring(6, 4);
                string extension = row["format_banners"].ToString();

                for (int i = 0; i < files.Count(); i++)
                    files[i] = year + "/" + files[i].Substring(0, 1) + "/" + files[i].Substring(1, 1) + "/" + files[i].Substring(2, 1) + "/" + files[i] + "." + extension;

                visuals.AddRange(files.Select(getCreativePath));
            }
        }*/
        #endregion

        #region GetPath
        /// <summary>
        /// Get Path
        /// </summary>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="row">data row</param>
        /// <param name="columns">columns</param>
        /// <param name="columnNames">columnNames</param>
        /// <returns></returns>
        protected override List<string> GetPath(VehicleInformation vehicle, DataRow row, List<GenericColumnItemInformation> columns, List<string> columnNames) {
            var visuals = new List<string>();

            switch (vehicle.Id) {
                case Vehicles.names.press:
                case Vehicles.names.newspaper:
                case Vehicles.names.magazine:
                case Vehicles.names.internationalPress:

                    //if (CheckPressFlagAccess(vehicle)) break;

                    Int64 idMedia;
                    Int64 dateCoverNum;
                    Int64 dateMediaNum;

                    if (IsVisualAvailable(row, out idMedia, out dateCoverNum, out dateMediaNum)) {
                        //visuel(s) disponible(s)
                        string[] files = row["associated_file"].ToString().Split(',');
                        for (int fileIndex = 0; fileIndex < files.Length; fileIndex++) {
                            if (files[fileIndex].Length > 0) {
                                visuals.Add(files[fileIndex]);
                            }
                        }
                    }
                    break;
                case Vehicles.names.indoor:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_INDOOR_CREATION_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathOutDoor);
                    break;
                case Vehicles.names.outdoor:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_OUTDOOR_CREATION_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathOutDoor);
                    break;
                case Vehicles.names.instore:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_INSTORE_CREATION_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathInStore);
                    break;
                case Vehicles.names.directMarketing:
                case Vehicles.names.mailValo:
                    if (
                        !_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DIRECT_MARKETING_CREATION_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathVMC);
                    break;
                case Vehicles.names.radio:
                case Vehicles.names.radioGeneral:
                case Vehicles.names.radioSponsorship:
                case Vehicles.names.radioMusic:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_RADIO_CREATION_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathRadio);
                    break;
                case Vehicles.names.tv:
                case Vehicles.names.others:
                case Vehicles.names.tvGeneral:
                case Vehicles.names.tvSponsorship:
                case Vehicles.names.tvNonTerrestrials:
                case Vehicles.names.tvAnnounces:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_TV_CREATION_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathTv);
                    break;
                case Vehicles.names.adnettrack:
                case Vehicles.names.czinternet:
                case Vehicles.names.internet:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_INTERNET_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathAdNetTrack);
                    break;
                case Vehicles.names.evaliantMobile:
                    if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_EVALIANT_MOBILE_ACCESS_FLAG))
                        break;
                    AddVisuals(row, visuals, GetCreativePathEvaliantMobile);
                    break;
            }

            return visuals;

        }
        #endregion

        #region GetPressVignettes
        /// <summary>
        /// Get Press Vignettes
        /// </summary>
        /// <param name="currentRow">Current Row</param>
        /// <param name="dateField">Date Field</param>
        /// <param name="vignettes">Vignettes</param>
        /// <param name="imagesList">Images List</param>
        /// <returns>HTML</returns>
        protected override string GetPressVignettes(DataRow currentRow, string dateField, string vignettes,
                                                   string imagesList) {
            bool first = true;
            var fileList = currentRow["associated_file"].ToString().Split(',');

            foreach (string file in fileList) {
                vignettes += string.Format("<img src='{0}' border=\"0\" width=\"50\" height=\"64\" >", file);
                if (first) imagesList = string.Format("{0}", file);
                else {
                    imagesList += string.Format(",{0}", file);
                }
                first = false;
            }

            if (vignettes.Length > 0) {
                vignettes = string.Format("<a href=\"javascript:openPressCreation('{0}');\">{1}</a>", imagesList, vignettes);
                vignettes += "\n<br>";
            }
            return vignettes;
        }
        #endregion

    }
}
