using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstFlags = TNS.AdExpress.Constantes.DB.Flags;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstVMCFormat = TNS.AdExpress.Constantes.DB.Format;


namespace TNS.AdExpressI.Insertions.Poland
{
    public class InsertionsResult : Insertions.InsertionsResult
    {
        protected const string JPG_EXTENSION = "jpg";
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
            var fileList = currentRow["associated_file"].ToString().Split(',');

           
            string pathWebImagette = string.Format("{0}/{1}/{2}/", CreationServerPathes.IMAGES, currentRow["id_media"].ToString()
                , currentRow[dateField].ToString());
            string pathWeb = pathWebImagette;

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

        protected override string GetOutDoorVignettes(long idVehicle, DataRow currentRow, string vignettes,
                                                    string imagesList)
        {
            bool first = true;
            string pathWeb;
            string[] fileList = currentRow["associated_file"].ToString().Split(',');
            string idAssociatedFile = currentRow["associated_file"].ToString();

     
            
             pathWeb = string.Format(@"{0}/{1}/", CreationServerPathes.IMAGES_OUTDOOR, JPG_EXTENSION);

            if (fileList != null && fileList.Length > 0)
            {
                for (int j = 0; j < fileList.Length; j++)
                {
                    string fileName = fileList[j];
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                    vignettes += string.Format("<img src='{0}{2}/{1}' border=\"0\" width=\"50\" height=\"64\" >"
                        , pathWeb, fileName, fileNameWithoutExtension.Substring(0,5));

                    if (first) imagesList = string.Format("{0}{2}/{1}", pathWeb, fileName
                        , fileNameWithoutExtension.Substring(0, 5));
                    else
                    {
                        imagesList += string.Format(",{0}{2}/{1}", pathWeb, fileName, 
                            fileNameWithoutExtension.Substring(0, 5));
                    }
                    first = false;
                }

                if (vignettes.Length > 0)
                {
                    vignettes = string.Format("<a href=\"javascript:openPressCreation('{0}');\">{1}</a>"
                        , imagesList, vignettes);
                    vignettes += "\n<br>";
                }
            }
            else vignettes = GestionWeb.GetWebWord(843, _session.SiteLanguage) + "<br>";
            return vignettes;
        }

        protected override string GetCreativePathOutDoor(string file, bool bigSize)
        {
        
            return string.Format("{0}/{1}/{2}/{3}"
                , CreationServerPathes.IMAGES_OUTDOOR
                , JPG_EXTENSION
                , file.Substring(0, 5)
                , file);

        }


        protected override void SetRawLine(VehicleInformation vehicle, ResultTable tab, DataRow row, int cLine,
           List<GenericColumnItemInformation> columns, List<string> columnsName, List<Cell> cells, string divideCol)
        {
            int i = -1;
            int j = 0;
            foreach (GenericColumnItemInformation g in columns)
            {

                i++;
                j++;
                if (cells[i] is CellUnit)
                {
                    int div = 1;
                    if (g.IsSum)
                    {
                        div = Math.Max(div, row[divideCol].ToString().Split(',').Length);
                    }
                    Double val = 0;
                    if (row[columnsName[i]] != DBNull.Value)
                    {
                        val = Convert.ToDouble(row[columnsName[i]]) / div;
                    }                   
                    if (tab[cLine, j] == null)
                    {
                        tab[cLine, j] = ((CellUnit)cells[i]).Clone(val);
                    }
                    else
                    {
                        ((CellUnit)tab[cLine, j]).Add(val);
                    }
                }
                else
                {
                    string s = string.Empty;
                    switch (columns[i].Id)
                    {
                        case GenericColumnItemInformation.Columns.associatedFile:
                            switch (vehicle.Id)
                            {
                              
                                case CstDBClassif.Vehicles.names.tv:                               
                                    s = row[columnsName[i]].ToString();
                                    if (s.Length > 0)
                                        tab[cLine, j] = new CellTvCreativeLink(s, _session, vehicle.DatabaseId);
                                    else
                                        tab[cLine, j] = new CellTvCreativeLink(string.Empty, _session, vehicle.DatabaseId);
                                    break;
                                case CstDBClassif.Vehicles.names.radio:
                                    if(row[columnsName[i]]!=DBNull.Value && row[columnsName[i]].ToString().Length>0)
                                    {
                                        string fileName =  Path.GetFileName(row[columnsName[i]].ToString());
                                        string idSlogan = Path.GetFileNameWithoutExtension(row[columnsName[i]].ToString());
                                        tab[cLine, j] = new CellRadioCreativeLink(string.Format("{0},{1}", fileName,idSlogan), _session,
                                                                                  VehiclesInformation.EnumToDatabaseId(CstDBClassif.Vehicles.names.radio));
                                                                                  
                                    }else
                                    {
                                         tab[cLine, j] = new CellRadioCreativeLink(string.Empty, _session,
                                                                                  VehiclesInformation.EnumToDatabaseId(CstDBClassif.Vehicles.names.radio));
                                    }
                                    break;
                                
                            }
                            break;
                        case GenericColumnItemInformation.Columns.dayOfWeek:
                            Int32 n = Convert.ToInt32(row[columnsName[i]]);
                            int y = n / 10000;
                            int m = (n - (10000 * y)) / 100;
                            int d = n - (10000 * y + 100 * m);
                            tab[cLine, j] = new CellDate(new DateTime(y, m, d), string.Format("{{0:{0}}}", g.StringFormat));
                            break;
                        case GenericColumnItemInformation.Columns.mailFormat:
                            string cValue = row[columnsName[i]].ToString();
                            if (cValue != CstVMCFormat.FORMAT_ORIGINAL)
                            {
                                cValue = GestionWeb.GetWebWord(2240, _session.SiteLanguage);
                            }
                            else
                            {
                                cValue = GestionWeb.GetWebWord(2241, _session.SiteLanguage);
                            }
                            if (tab[cLine, j] == null)
                            {
                                tab[cLine, j] = new CellLabel(cValue);
                            }
                            else
                            {
                                ((CellLabel)tab[cLine, j]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, j]).Label, cValue);
                            }
                            break;
                        case GenericColumnItemInformation.Columns.product:
                            s = row[columnsName[i]].ToString();
                            if (!_session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                            {
                                s = string.Empty;
                            }
                            if (tab[cLine, j] == null || ((CellLabel)tab[cLine, j]).Label.Length <= 0)
                            {
                                tab[cLine, j] = new CellLabel(s);
                            }
                            else
                            {
                                ((CellLabel)tab[cLine, j]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, j]).Label, s);
                            }
                            break;
                        case GenericColumnItemInformation.Columns.slogan:
                            s = row[columnsName[i]].ToString();
                            if (!_session.CustomerLogin.CustormerFlagAccess(CstFlags.ID_SLOGAN_ACCESS_FLAG))
                            {
                                s = string.Empty;
                            }
                            if (tab[cLine, j] == null || ((CellLabel)tab[cLine, j]).Label.Length <= 0)
                            {
                                tab[cLine, j] = new CellLabel(s);
                            }
                            else
                            {
                                ((CellLabel)tab[cLine, j]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, j]).Label, s);
                            }
                            break;
                        default:
                            if (tab[cLine, j] == null)
                            {
                                tab[cLine, j] = new CellLabel(row[columnsName[i]].ToString());
                            }
                            else
                            {
                                ((CellLabel)tab[cLine, j]).Label = string.Format("{0}, {1}", ((CellLabel)tab[cLine, j]).Label, row[columnsName[i]].ToString());
                            }
                            break;
                    }
                }

            }
        }


    }
}
