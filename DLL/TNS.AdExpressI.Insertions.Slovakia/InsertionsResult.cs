using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using CstDB = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpressI.Insertions.Slovakia
{
    public class InsertionsResult : Insertions.InsertionsResult
    {
        #region Constructor
        /// <summary>
        /// DEfault constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="moduleId">Module ID</param>
        public InsertionsResult(WebSession session, Int64 moduleId) : base(session, moduleId)
        {
        }
        #endregion

        #region Add Visuals
        /// <summary>
        /// Add Visuals
        /// </summary>
        /// <param name="row">Data row</param>
        /// <param name="visuals">Visuals list</param>
        /// <param name="getCreativePath">extact creative path</param>
        protected override void AddVisuals(DataRow row, List<string> visuals, Func<string, string> getCreativePath)
        {
            //associated_file
            if (row["id_slogan"] != DBNull.Value)
            {
                var files = row["id_slogan"].ToString().Split(',');
                visuals.AddRange(files.Select(getCreativePath));
            }
        }

        private void AddEvaliantVisuals(DataRow row, List<string> visuals, Func<string, string> getCreativePath)
        {
            if (row["associated_file"] != DBNull.Value)
            {
                var files = row["associated_file"].ToString().Split(',');
                visuals.AddRange(files.Select(getCreativePath));
            }
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
        protected override bool IsVisualAvailable(DataRow row, out long idMedia, out long dateCoverNum, out long dateMediaNum)
        {
            dateCoverNum = -1;
            dateMediaNum = -1;
            idMedia = -1;

            if (row.Table.Columns.Contains("id_media") && row["id_media"] != DBNull.Value)
            {
                idMedia = Convert.ToInt64(row["id_media"]);
            }
            return (idMedia > 0);
        }
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
        protected override List<string> GetPath(VehicleInformation vehicle, DataRow row, List<GenericColumnItemInformation> columns, List<string> columnNames)
        {
            var visuals = new List<string>();

            switch (vehicle.Id)
            {
                case Vehicles.names.press:
                case Vehicles.names.newspaper:
                case Vehicles.names.magazine:
                case Vehicles.names.internationalPress:

                    if (CheckPressFlagAccess(vehicle)) break;

                    Int64 idMedia;
                    Int64 dateCoverNum;
                    Int64 dateMediaNum;

                    if (IsVisualAvailable(row, out idMedia, out dateCoverNum, out dateMediaNum))
                    {
                        //visuel(s) disponible(s)
                        string[] files = row["visual"].ToString().Split(',');
                        for (int fileIndex = 0; fileIndex < files.Length; fileIndex++)
                        {
                            if (files[fileIndex].Length > 0)
                            {
                                visuals.Add(CreationServerPathes.IMAGES + "/" + files[fileIndex]);
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
                    AddEvaliantVisuals(row, visuals, GetCreativePathAdNetTrack);
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

        protected override void SetRawLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine, List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, string divideCol)
        {
            int index1 = -1;
            int index2 = 0;
            long TOP_DIFFUSION_VISIBILITY_DATE = 20170904;
            var MEDIA_IDS_TOP_DIFFUSION_VISIBILITY = new List<long> { 3168, 18560, 18564, 24982 };
            foreach (GenericColumnItemInformation columnItemInformation1 in columns)
            {
                ++index1;
                ++index2;
                if (cells[index1] is CellUnit)
                {
                    int val1 = 1;
                    if (columnItemInformation1.IsSum)
                        val1 = Math.Max(val1, row[divideCol].ToString().Split(',').Length);
                    double num1 = 0.0;
                    if (row[columnsName[index1]] != DBNull.Value)
                        num1 = Convert.ToDouble(row[columnsName[index1]]) / (double)val1;
                    switch (columns[index1].Id)
                    {
                        case GenericColumnItemInformation.Columns.topDiffusion:
                            if (IsTvVehicle(vehicle))
                            {
                                long num2 = Convert.ToInt64(row[WebApplicationParameters.GenericColumnItemsInformation.Get(89L).DataBaseField].ToString());
                                if (MediasWithoutTopDif != null &&
                                    MediasWithoutTopDif.Contains(
                                        Convert.ToInt64(
                                            row[
                                                WebApplicationParameters.GenericColumnItemsInformation.Get(3L)
                                                    .DataBaseIdField].ToString()))
                                    || CategoriesWithoutTopDif != null && CategoriesWithoutTopDif.Contains(num2))
                                {
                                    num1 = 0.0;
                                }
                                else
                                {
                                    long dateMediaNum = Convert.ToInt64(row["date_media_num"]);
                                    long idMedia = Convert.ToInt64(row["id_media"]);

                                    /*A partir de la date_media 04 / 09, ces chaines 
                                     * 3168 - FRANCE O
                                     * 18560 - HD1
                                     * 18564 - NUMERO 23
                                     * 24982 – RMC DECOUVERTE S
                                     * devront être gérées comme de la pige réelle (catégorie 31), 
                                    donc elles ne constitueront plus un cas particulier. En revanche attention, toutes les lignes qui ont une date_media au 03 / 09 
                                        et avant doivent rester gérées comme aujourd’hui.Le changement dans la règle de transfert ne concerne que les données à partir du 04 / 09.
                                        */
                                    if (MEDIA_IDS_TOP_DIFFUSION_VISIBILITY.Contains(idMedia) && dateMediaNum < TOP_DIFFUSION_VISIBILITY_DATE)
                                        num1 = 0.0;
                                }


                            }
                            break;
                        case GenericColumnItemInformation.Columns.volume:
                            if (!_session.CustomerLogin.CustormerFlagAccess(252L))
                            {
                                num1 = 0.0;

                            }
                            break;
                        case GenericColumnItemInformation.Columns.weight:
                            if (!_session.CustomerLogin.CustormerFlagAccess(256L))
                            {
                                num1 = 0.0;

                            }
                            break;
                    }
                    if (tab[cLine, index2] == null)
                    {
                        if (num1 == 0.0 && columns[index1].Id == GenericColumnItemInformation.Columns.topDiffusion)
                            tab[cLine, index2] = new CellAiredTime(null);
                        else
                            tab[cLine, index2] = ((CellUnit)cells[index1]).Clone(num1);
                    }
                    else
                        ((CellUnit)tab[cLine, index2]).Add(num1);
                }
                else
                {
                    string str = string.Empty;
                    if (_getInsertionsExcel && columns[index1].Id == GenericColumnItemInformation.Columns.dateMediaNum)
                    {
                        int num = Convert.ToInt32(row[columnsName[index1]]);
                        int year = num / 10000;
                        int month = (num - 10000 * year) / 100;
                        int day = num - (10000 * year + 100 * month);
                        tab[cLine, index2] = new CellDate(new DateTime(year, month, day), string.Format("{{0:{0}}}", columnItemInformation1.StringFormat));
                    }
                    else
                    {
                        switch (columns[index1].Id)
                        {
                            case GenericColumnItemInformation.Columns.slogan:
                                string label1 = row[columnsName[index1]].ToString();
                                if (!this._session.CustomerLogin.CustormerFlagAccess(221L))
                                    label1 = string.Empty;
                                if (tab[cLine, index2] == null || ((CellLabel)tab[cLine, index2]).Label.Length <= 0)
                                {
                                    tab[cLine, index2] = new CellLabel(label1);
                                    break;
                                }
                                ((CellLabel)tab[cLine, index2]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, index2]).Label, label1);
                                break;
                            case GenericColumnItemInformation.Columns.product:
                                string label2 = row[columnsName[index1]].ToString();
                                if (!this._session.CustomerLogin.CustormerFlagAccess(271L))
                                    label2 = string.Empty;
                                if (tab[cLine, index2] == null || ((CellLabel)tab[cLine, index2]).Label.Length <= 0)
                                {
                                    tab[cLine, index2] = (ICell)new CellLabel(label2);
                                    break;
                                }
                                ((CellLabel)tab[cLine, index2]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, index2]).Label, label2);
                                break;
                            case GenericColumnItemInformation.Columns.associatedFile:
                                switch (vehicle.Id)
                                {
                                    case Vehicles.names.radio:
                                        var sloganField = WebApplicationParameters.GenericColumnItemsInformation.Get(GenericColumnItemInformation.Columns.slogan.GetHashCode()).DataBaseIdField;
                                        tab[cLine, index2] = row[sloganField].ToString().Length <= 0 ? new CellRadioCreativeLink(string.Empty, _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radio))
                                            : new CellRadioCreativeLink(row[sloganField].ToString(), _session, VehiclesInformation.EnumToDatabaseId(Vehicles.names.radio));
                                        break;
                                    case Vehicles.names.tv:
                                    case Vehicles.names.others:
                                        string creative = row["id_slogan"].ToString();
                                        tab[cLine, index2] = creative.Length <= 0 ? new CellTvCreativeLink(string.Empty, _session, vehicle.DatabaseId)
                                            : new CellTvCreativeLink(creative, _session, vehicle.DatabaseId);
                                        break;
                                }
                                break;
                            case GenericColumnItemInformation.Columns.dayOfWeek:
                                int num = Convert.ToInt32(row[columnsName[index1]]);
                                int year = num / 10000;
                                int month = (num - 10000 * year) / 100;
                                int day = num - (10000 * year + 100 * month);
                                tab[cLine, index2] = new CellDate(new DateTime(year, month, day), string.Format("{{0:{0}}}", columnItemInformation1.StringFormat));
                                break;
                            case GenericColumnItemInformation.Columns.mailFormat:
                                string label3 = row[columnsName[index1]].ToString() == "20" ?
                                    GestionWeb.GetWebWord(2241L, _session.SiteLanguage) : GestionWeb.GetWebWord(2240L, _session.SiteLanguage);
                                if (tab[cLine, index2] == null)
                                {
                                    tab[cLine, index2] = new CellLabel(label3);
                                    break;
                                }
                                ((CellLabel)tab[cLine, index2]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, index2]).Label, label3);
                                break;
                            default:
                                if (tab[cLine, index2] == null)
                                {
                                    tab[cLine, index2] = new CellLabel(row[columnsName[index1]].ToString());
                                    break;
                                }
                                ((CellLabel)tab[cLine, index2]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, index2]).Label, row[columnsName[index1]].ToString());
                                break;
                        }
                    }
                }
            }
        }

        protected override Insertions.Cells.CellCreativesRadioInformation GetCellCreativesRadioInformation(VehicleInformation vehicle,
            List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells)
        {
            return new Cells.CellCreativesRadioInformation(_session, vehicle, columns, columnsName, cells, _module);
        }

        protected override Insertions.Cells.CellCreativesRadioInformation GetCellCreativesRadioInformation(VehicleInformation vehicle, List<GenericColumnItemInformation> columns,
            List<string> columnsName, List<Cell> cells, long idColumnsSet)
        {
            return new Cells.CellCreativesRadioInformation(_session, vehicle, columns, columnsName, cells, _module, idColumnsSet);
        }
    }
}
