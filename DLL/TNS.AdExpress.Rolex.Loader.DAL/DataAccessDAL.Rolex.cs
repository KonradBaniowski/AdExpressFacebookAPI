using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Cells;
using TNS.AdExpress.Rolex.Loader.DAL.DbType;
using TNS.AdExpress.Rolex.Loader.DAL.Exceptions;
using TNS.AdExpress.Rolex.Loader.Domain;
using TNS.FrameWork.Date;

namespace TNS.AdExpress.Rolex.Loader.DAL
{
    public partial class DataAccessDAL
    {
        public List<RolexDetail> GetRolexDetails(string filePath,out List<PictureMatching> pictures,DataAccessDb db)
        {
            var rolexDetails = new List<RolexDetail>();
            Workbook excel = null;
            const int startLineData = 1;
            //const int startColumnData = 0;
            const int columnVehicle = 0;
            const int columnLocation = 1, columnCommentary = 2, columnVisibility = 3;
            const int columnVisuals = 4;

            try
            {
                #region Initialize Excel Object
                excel = new Workbook();
                var license = new License();
                license.SetLicense("Aspose.Cells.lic");
                FileStream fileStream;
                AtomicPeriodWeek atomicPeriodWeek;
                #endregion

                #region Load file
                using (fileStream = new FileStream(filePath, FileMode.Open))
                {
                    try
                    {
                        string str = Path.GetFileNameWithoutExtension(fileStream.Name).Replace("Rolex_", "");
                         atomicPeriodWeek = (new AtomicPeriodWeek(int.Parse(str.Substring(0, 4)), int.Parse(str.Substring(4, 2))));
                    }
                    catch (Exception e)
                    {
                        throw new DataAccessDALExcelFormatException("Incorrect File Name: " + fileStream.Name, e);
                    }

                    try
                    {
                        excel.Open((Stream)fileStream);
                    }
                    catch (Exception e)
                    {
                        throw new DataAccessDALExcelOpenFileException("Impossible to open excel file Name: " + fileStream.Name, e);
                    }
                    finally
                    {
                        fileStream.Close();
                    }
                }
                #endregion


                try
                {
                    Worksheet sheet = excel.Worksheets[0];
                    Cells cells = sheet.Cells;

                    //Get vehicles
                    var vehicles = SelectMedia(db, _idLanguage);

                    //Get visibility types
                    var typePresences = SelectPresenceType(db, _idLanguage);

                    //Get location
                    var locations = SelectLocation(db, _idLanguage);

                    //Get date begin                    
                    var dateBegin = atomicPeriodWeek.FirstDay;

                    //Get date end
                    var dateEnd = atomicPeriodWeek.LastDay; 

                    pictures = new List<PictureMatching>();

                    int line;
                    for (line = startLineData; !IsEndLine(columnLocation, columnVisibility, columnVisuals, cells, line, columnVehicle); line++)
                    {
                        //Get vehicle
                        var vehicle = GetVehicle(cells,cells[line, columnVehicle].Value, vehicles, line, columnVehicle);

                        //Get location
                        var location = GetLocation(cells,cells[line, columnLocation].Value, locations, line, columnLocation);

                        //Get vehicle
                        var typePresenceIds = GetTypePresences(cells,cells[line, columnVisibility].Value, typePresences, line, columnVisibility);
                      
                        //Get commentary
                        var commentary = (cells[line, columnCommentary].Value!=null) ? Convert.ToString(cells[line, columnCommentary].Value) : string.Empty;

                        //Get visuals
                        var visuals = GetVisuals(fileStream, columnVisuals, cells, line);

                        var picturesTemp = GetPictureFileName(visuals, db, line, columnVisuals);

                        pictures.AddRange(picturesTemp.Values);


                        rolexDetails.Add(new RolexDetail(vehicle.IdSite, location.IdEmplacement, typePresenceIds, dateBegin, dateEnd, (from el in picturesTemp.Values select el.PathOut).ToList<string>(), commentary));
                    }

                }
                catch (DataAccessDALClassificationException ex)
                {
                    throw new DataAccessDALClassificationException("Error in Classification while treating Excel file", ex);
                }
                return rolexDetails;
            }
            finally
            {
                excel = null;
            }


        }

        private bool IsEndLine(int columnLocation, int columnVisibility, int columnVisuals, Cells cells, int line, int columnVehicle)
        {
            return (cells[line, columnVehicle].Value == null && cells[line, columnLocation].Value == null &&
                    cells[line, columnVisibility].Value == null && cells[line, columnVisuals].Value == null);
        }

        private List<string> GetVisuals(FileStream fileStream, int columnVisuals, Cells cells, int line)
        {          
            List<string> visuals = null;
            if (cells[line, columnVisuals].Value != null)
            {
                visuals =
                    new List<string>(((string) cells[line, columnVisuals].Value).Split(new[] {';', ','})).ConvertAll
                        <string>(
                            file =>
                            Path.GetFullPath(
                                Path.Combine(Path.GetDirectoryName(fileStream.Name),
                                             file.Trim())));

                for (int i = 0; i < visuals.Count; i++)
                {
                    if (!File.Exists(visuals[i]))
                    {
                       string[] files;
                        try
                        {
                             files = Directory.GetFiles(Path.GetDirectoryName(visuals[i]),
                                                      Path.GetFileName(visuals[i]) + ".*",
                                                      SearchOption.TopDirectoryOnly);
                        }
                        catch (Exception)
                        {

                            throw new DataAccessDALExcelVisualException(new CellExcel(line + 1, columnVisuals + 1), "Invalid visuals");
                        }
                       
                        if (files.Length > 1)
                            throw new DataAccessDALBadPictureNumberException(new CellExcel(line+1, columnVisuals+1),
                                "Impossible to retrieve the file. " + files.Length + " files are found");
                        if (files.Length <= 0)
                            throw new DataAccessDALBadPictureNameException(new CellExcel(line+1, columnVisuals+1),
                                "Impossible to retrieve the file '" + visuals[i] + "'");
                    }
                }
            }
            else throw new DataAccessDALExcelVisualException(new CellExcel(line+1, columnVisuals+1), "Invalid visuals");
            return visuals;
        }

        public void Delete(long firstWeek,long lastWeek, DataAccessDb db)
        {
            lock (_instance)
            {
                var query = new StringBuilder();
                query.Append(" BEGIN ");
                query.AppendFormat(" DELETE FROM {0}DATA_ROLEX WHERE LOAD_DATE>={1} and LOAD_DATE<={2} ;", ROLEX_SCHEMA, firstWeek, lastWeek);
                query.Append(" END; ");
                var dbCmd = db.SetCommand(query.ToString());
                dbCmd.ExecuteNonQuery();
            }
        }

        public bool HasData(long firstWeek,long lastWeek, DataAccessDb db)
        {
            var query = (from r in db.Rolex where r.LoadDate >= firstWeek && r.LoadDate <= lastWeek select r);
            return query.Any();
        }

        public void Insert(List<RolexDetail> rolexDetails, long weekDate, DataAccessDb db)
        {
            if (!HasData(weekDate, weekDate, db))
            {
                if (rolexDetails != null && rolexDetails.Count > 0)
                {

                    var sql = new StringBuilder(200);
                    var queryInsertPattern = string.Format(" INSERT INTO {0}DATA_ROLEX (ID_DATA_ROLEX, ID_LANGUAGE_DATA_I, ID_SITE, ID_LOCATION, ID_PRESENCE_TYPE,DATE_BEGIN_NUM, DATE_END_NUM, VISUAL, ID_PAGE,ACTIVATION, COMMENTARY,LOAD_DATE) ", ROLEX_SCHEMA);
                    const string queryValuesPattern = " VALUES ({0}, {1}, {2}, {3}, {4}, {5},{6}, '{7}',{8}, {9},'{10}', {11}); ";

                    var query = new StringBuilder();
                    query.Append("BEGIN ");

                    foreach (RolexDetail rolexDetail in rolexDetails)
                    {
                        long idPage = db.GetPageIdSequenceId();
                        for (int i = 0; i < rolexDetail.IdPresenceTypes.Count; i++)
                        {
                            var comments = (string.IsNullOrEmpty(rolexDetail.Commentary)? "": Regex.Replace(rolexDetail.Commentary, "[']", "''"));
                            query.Append(queryInsertPattern);
                            query.AppendFormat(queryValuesPattern, "ROLEX03.SEQ_DATA_ROLEX.NEXTVAL", _idLanguage,
                                               rolexDetail.IdSite, rolexDetail.IdLocation, rolexDetail.IdPresenceTypes[i],
                                               rolexDetail.DateBegin.ToString("yyyyMMdd")
                                               , rolexDetail.DateEnd.ToString("yyyyMMdd"),
                                               String.Join(",", rolexDetail.Visuals),idPage, ACTIVATED, comments, weekDate);
                        }
                    }
                    query.Append(" END; ");
                    var dbCmd = db.SetCommand(query.ToString());
                    dbCmd.ExecuteNonQuery();
                }
            }
            else throw new DataAccessDALInsertException("Data Allready Exist for this period");
           

        }

        private List<long> GetTypePresences(Cells cells, object value, IEnumerable<DataPresenceType> visibilities, int lineId, int columnId)
        {
            List<long> typePresences = null;
            try
            {
                var val = Convert.ToString(cells[lineId, columnId].Value);
                if (!string.IsNullOrEmpty(val))
                {
                    List<string> strArr = val.Split(',').Select(e => e.Trim().ToUpper()).ToList();
                    typePresences = visibilities.Where(el => strArr.Contains(el.TypePresence.Trim().ToUpper())).Select(el => el.IdTypePresence).ToList();
                }
            }
            catch (Exception ex)
            {
                throw (new DataAccessDALExcelPresenceTypeException(new CellExcel(lineId+1, columnId+1), " Impossible de retrouver le type de présence saisi", ex));
            }
            if (typePresences != null && typePresences.Count >0) return typePresences;
            throw (new DataAccessDALExcelPresenceTypeException(new CellExcel(lineId+1, columnId+1), " Impossible de retrouver le type de présence saisi "));
        }

        private DataLocation GetLocation(Cells cells, object value, IEnumerable<DataLocation> locations, int lineId, int columnId)
        {
            List<DataLocation> locs = null;
            try
            {
                var val = Convert.ToString(cells[lineId, columnId].Value);
                if (locations != null)
                {
                    locs = (from m in locations where m.Emplacement.Trim().ToUpper() == val.Trim().ToUpper() select m).ToList();
                }
            }
            catch (Exception ex)
            {
                throw (new DataAccessDALExcelLocationException(new CellExcel(lineId+1, columnId+1), " Impossible de retrouver l'emplacement saisi ", ex));
            }                    
            if (locs != null && locs.Count == 1) return locs[0];
            throw (new DataAccessDALExcelLocationException(new CellExcel(lineId+1, columnId+1), " Impossible de retrouver l'emplacement saisi "));
        }

        private DataMedia GetVehicle( Cells cells,object value, IEnumerable<DataMedia> vehicles, int lineId, int columnId)
        {
            List<DataMedia> dataMedias = null;
            try
            {
                var val = Convert.ToString(cells[lineId, columnId].Value);
                if (vehicles != null)
                {
                    dataMedias = (from m in vehicles where m.Site.Trim().ToUpper() == val.Trim().ToUpper() select m).ToList();
                }
            }
            catch (Exception ex)
            {
                throw (new DataAccessDALExcelSiteException(new CellExcel(lineId+1, columnId+1), " Impossible de retrouver le Site saisi ", ex));
            }
          
            if (dataMedias != null && dataMedias.Count == 1) return dataMedias[0];
            throw (new DataAccessDALExcelSiteException(new CellExcel(lineId+1, columnId+1), "  Impossible de retrouver le Site saisi  " + value));
        }

        /// <summary>
        /// Get Picture File Name
        /// </summary>
        /// <param name="fileList">File List</param>
        /// <param name="db">database </param>
        /// <param name="line">line index</param>
        /// <param name="columnVisuals">column visual</param>
        /// <returns>Picture File Name List</returns>
        public Dictionary<string, PictureMatching> GetPictureFileName(List<string> fileList, DataAccessDb db, int line, int columnVisuals)
        {
                var filePictureList = new Dictionary<string, PictureMatching>();
                if (fileList != null)
                {
                    foreach (string cFile in fileList)
                    {
                        long id = db.GetVisualSequenceId();

                        try
                        {
                            filePictureList.Add(cFile, new PictureMatching(cFile, id.ToString(CultureInfo.InvariantCulture) + Path.GetExtension(cFile)));
                        }
                        catch (ArgumentException e)
                        {                         
                                throw new DataAccessDALRedundantPictureNameException(new CellExcel(line + 1, columnVisuals + 1),
                                   "Redundant image name in the same cell. ");
                        }
                       
                    }
                }
                return filePictureList;
          
        }


    }
}
