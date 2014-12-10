using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Insertions.France {
    public class InsertionsResult : TNS.AdExpressI.Insertions.InsertionsResult {

        #region Constructor
        /// <summary>
        /// DEfault constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="moduleId">Module ID</param>
        public InsertionsResult(WebSession session, Int64 moduleId)
            : base(session, moduleId) {
        }
        #endregion

        #region Add Visuals
        /// <summary>
        /// Add Visuals
        /// </summary>
        /// <param name="row">Data row</param>
        /// <param name="visuals">Visuals list</param>
        /// <param name="getCreativePath">extact creative path</param>
        protected override void AddVisuals(DataRow row, List<string> visuals, Func<string, string> getCreativePath) {
            if (row["associated_file"] != DBNull.Value) {
                var files = row["associated_file"].ToString().Split(',');
                if (row.Table.Columns.Contains("id_country") && row["id_country"] != DBNull.Value) {
                    switch (row["id_country"].ToString()) {
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
        protected override string GetEvaliantVignettes(DataRow currentRow, string vignettes, string themeName, string creativePath) {
            if (currentRow["associated_file"] != DBNull.Value && !string.IsNullOrEmpty(currentRow["associated_file"].ToString())) {
                
                string path = currentRow["associated_file"].ToString().Replace(@"\", "/");

                if (currentRow.Table.Columns.Contains("id_country") && currentRow["id_country"] != DBNull.Value) {
                    switch (currentRow["id_country"].ToString()) {
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

        #region Add Country Acronym
        /// <summary>
        /// Add Country Acronym
        /// </summary>
        /// <param name="files">Files</param>
        /// <param name="countryAcronym">Country Acronym</param>
        /// <returns>Files with country acronym</returns>
        private string[] AddCountryAcronym(string[] files, string countryAcronym) {

            for (int i = 0; i < files.Length; i++) {
                files[i] = countryAcronym + "/Banners/" + files[i];
            }

            return files;
        }
        #endregion

    }
}
