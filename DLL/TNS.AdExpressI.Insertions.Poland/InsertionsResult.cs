using System;
using System.Data;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Insertions.Poland
{
    public class InsertionsResult : Insertions.InsertionsResult
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="moduleId">Module ID</param>
        public InsertionsResult(WebSession session, long moduleId)
            : base(session, moduleId)
        {
        }

        #endregion

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
            Int64 disponibility = -1;
            Int64 activation = -1;
            dateCoverNum = -1;
            dateMediaNum = -1;
            idMedia = -1;

            if (row.Table.Columns.Contains("id_media") && row["id_media"] != DBNull.Value)
            {
                idMedia = Convert.ToInt64(row["id_media"]);
            }
            if (row.Table.Columns.Contains("date_media_num") && row["date_media_num"] != DBNull.Value)
            {
                dateMediaNum = Convert.ToInt64(row["date_media_num"]);
            }
            if (row.Table.Columns.Contains("dateKiosque") && row["dateKiosque"] != DBNull.Value)
            {
                dateMediaNum = Convert.ToInt64(row["dateKiosque"]);
            }
            return (idMedia > 0 && row["visual"] != DBNull.Value);
        }

        #region GetCreativePathPress
        /// <summary>
        /// Get Creative Path Press
        /// </summary>
        /// <param name="file">file</param>
        /// <param name="idMedia">id Media</param>
        /// <param name="dateCoverNum">date Cover 
        /// </param>
        /// <param name="bigSize">true if big size else false</param>
        /// <param name="dateMediaNum">date Media </param>
        /// <returns>creative path</returns>
        protected override string GetCreativePathPress(string file, Int64 idMedia, Int64 dateCoverNum, bool bigSize, Int64 dateMediaNum)
        {         

            return string.Format("{0}/{1}/{2}/{3}", CreationServerPathes.IMAGES, idMedia, dateMediaNum, file);

        }
        #endregion

        protected override string GetPressVignettes(DataRow currentRow, string dateField, string vignettes,
                                              string imagesList)
        {
            bool first = true;
            var fileList = currentRow["visual"].ToString().Split(',');

           
            string pathWebImagette = string.Format("{0}/{1}/{2}/", CreationServerPathes.IMAGES, currentRow["id_media"].ToString()
                , currentRow[dateField].ToString());
            string pathWeb = string.Format("{0}/{1}/{2}/", CreationServerPathes.IMAGES, currentRow["id_media"].ToString()
                , currentRow[dateField].ToString());

            foreach (string file in fileList)
            {
                vignettes += string.Format("<img src='{0}{1}' border=\"0\" width=\"50\" height=\"64\" >", pathWebImagette, file);
                if (first) imagesList = string.Format("{0}{1}", pathWeb, file);
                else
                {
                    imagesList += string.Format(",{0}{1}", pathWeb, file);
                }
                first = false;
            }

            if (vignettes.Length > 0)
            {
                vignettes = string.Format("<a href=\"javascript:openPressCreation('{0}');\">{1}</a>", imagesList.Replace("/Imagette", ""), vignettes);
                vignettes += "\n<br>";
            }
            return vignettes;
        }



    }
}
