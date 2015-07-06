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

namespace TNS.AdExpressI.Insertions.France
{
    public class InsertionsResult : Insertions.InsertionsResult
    {

        #region Constructor
        /// <summary>
        /// DEfault constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="moduleId">Module ID</param>
        public InsertionsResult(WebSession session, Int64 moduleId)
            : base(session, moduleId)
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
            if (row["associated_file"] != DBNull.Value)
            {
                var files = row["associated_file"].ToString().Split(',');
                if (row.Table.Columns.Contains("id_country") && row["id_country"] != DBNull.Value)
                {
                    switch (row["id_country"].ToString())
                    {
                        case CountryCode.FRANCE: AddCountryAcronym(files, CountryAcronym.FRANCE); break;
                        case CountryCode.ITALY: AddCountryAcronym(files, CountryAcronym.ITALY); break;
                        case CountryCode.UK: AddCountryAcronym(files, CountryAcronym.UK); break;
                        default: break;
                    }
                }
                visuals.AddRange(files.Select(getCreativePath));
            }
        }
        #endregion

        #region Get Evaliant Vignettes
        /// <summary>
        /// Get Evaliant Vignettes
        /// </summary>
        /// <param name="currentRow">Current row</param>
        /// <param name="vignettes">Vignettes</param>
        /// <param name="themeName">Theme Name</param>
        /// <param name="creativePath">Creative Path</param>
        /// <returns>HTML code</returns>
        protected override string GetEvaliantVignettes(DataRow currentRow, string vignettes, string themeName, string creativePath)
        {
            if (currentRow["associated_file"] != DBNull.Value && !string.IsNullOrEmpty(currentRow["associated_file"].ToString()))
            {

                string path = currentRow["associated_file"].ToString().Replace(@"\", "/");

                if (currentRow.Table.Columns.Contains("id_country") && currentRow["id_country"] != DBNull.Value)
                {
                    switch (currentRow["id_country"].ToString())
                    {
                        case CountryCode.FRANCE: path = CountryAcronym.FRANCE + "/Banners/" + path; break;
                        case CountryCode.ITALY: path = CountryAcronym.ITALY + "/Banners/" + path; break;
                        case CountryCode.UK: path = CountryAcronym.UK + "/Banners/" + path; break;
                        default: break;
                    }
                }

                vignettes =
                    string.Format(
                        "<a href=\"javascript:openEvaliantCreative('{1}/{0}', '{3}');\"><img border=\"0\" src=\"/App_Themes/{2}/Images/Common/Button/adnettrack.gif\"></a>",
                        path,
                        creativePath, themeName, currentRow["advertDimension"]);
            }
            return vignettes;
        }
        #endregion

        #region GetVignettes

        protected override string GetVignettes(long idVehicle, DataRow currentRow, string vignettes, string themeName, string picto)
        {
            if (currentRow["id_slogan"] != DBNull.Value && !string.IsNullOrEmpty(currentRow["id_slogan"].ToString()))
                vignettes =
                    string.Format(
                        "<a href=\"javascript:openDownload('{0}','{1}','{2}');\"><img border=\"0\" src=\"/App_Themes/{3}/Images/Common/{4}\"></a>"
                        , currentRow["id_slogan"].ToString(), _session.IdSession, idVehicle, themeName, picto);
            return vignettes;
        }

        #endregion


        #region Add Country Acronym
        /// <summary>
        /// Add Country Acronym
        /// </summary>
        /// <param name="files">Files</param>
        /// <param name="countryAcronym">Country Acronym</param>
        /// <returns>Files with country acronym</returns>
        private string[] AddCountryAcronym(string[] files, string countryAcronym)
        {

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = countryAcronym + "/Banners/" + files[i];
            }

            return files;
        }
        #endregion

        protected override void SetRawLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine, List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, string divideCol)
        {
            int index1 = -1;
            int index2 = 0;
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
                                if (MediasWithoutTopDif != null && MediasWithoutTopDif.Contains(Convert.ToInt64(row[WebApplicationParameters.GenericColumnItemsInformation.Get(3L).DataBaseIdField].ToString()))
                                    || CategoriesWithoutTopDif != null && CategoriesWithoutTopDif.Contains(num2))
                                    num1 = 0.0;

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
                        tab[cLine, index2] = ((CellUnit)cells[index1]).Clone(num1);
                    else
                        ((CellUnit)tab[cLine, index2]).Add(num1);
                }
                else
                {
                    string str = string.Empty;
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
                                    string creative = row[columnsName[index1]].ToString();
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
