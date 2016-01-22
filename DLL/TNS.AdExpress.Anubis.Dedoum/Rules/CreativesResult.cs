using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using TNS.AdExpress.Anubis.Dedoum.Common;
using TNS.AdExpress.Anubis.Dedoum.Exceptions;
using TNS.AdExpress.Anubis.Dedoum.Functions;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Insertions.DAL;
using TNS.FrameWork.DB.Common;
using System.Data;
using Utils = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Anubis.Dedoum.Rules
{
    /// <summary>
    /// Create zip file of creatives
    /// </summary>
    public class CreativesResult
    {


        /// <summary>
        /// Process data from database so as to get the desired information
        /// </summary>
        /// <remarks>Able to process one or more media</remarks>
        /// <param name="dataSource">Data Source</param>
        /// <param name="webSession">User Session</param>	
        /// <param name="rqDetails">row detail</param>	
        /// <param name="dedoumConfig">Config</param>
        /// <param name="fileName">file Name</param>
        /// <returns>DataSet ready to be displayed</returns>
        public static bool ExportCreatives(IDataSource dataSource, WebSession webSession, DataRow rqDetails, DedoumConfig dedoumConfig, string fileName)
        {
            FileStream fsOut = null;
            ZipOutputStream zipStream = null;
            bool hasFiles = false;
            IInsertionsDAL _dalLayer = null;
            FileInfo copyFile = null;
            List<GenericColumnItemInformation> cols = null;
            string fileToCopy = null,  creativeDirName =null,  targetPath =null, currentDir = null,  destDirname =null,    zipFilename,tempDir = "";
             string temp = Path.GetTempPath();
             string OUT_PUT_PATH = Path.Combine(temp, "TempZip");
             if (!Directory.Exists( OUT_PUT_PATH))
                 Directory.CreateDirectory(OUT_PUT_PATH);

            try
            {

               

                object[] param = new object[2];
                param[0] = webSession;
                param[1] = webSession.CurrentModule;

                // Sélection du vehicle
                string vehicleSelection = webSession.GetSelection(webSession.SelectionUniversMedia, Right.type.vehicleAccess);
                if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw new DedoumException("La sélection de médias est incorrecte");
                VehicleInformation vehicleInformation = VehiclesInformation.Get(long.Parse(vehicleSelection));
                if (vehicleInformation == null) throw (new DedoumException("La sélection de médias est incorrecte"));

                //Periods
                string fromDate = Utils.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd");
                string toDate = Utils.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd");

                //Get Columns
               cols = new List<GenericColumnItemInformation>();
                List<int> lv = webSession.CreativesExportOptions;
                if (lv != null && lv.Count > 0)
                {
                    for (int i = 0; i < lv.Count; i++)
                    {
                        cols.Add(WebApplicationParameters.GenericColumnItemsInformation.Get(lv[i]));
                    }

                }                
                if (dedoumConfig.ColumnsCongifs != null && dedoumConfig.ColumnsCongifs.Count > 0 && dedoumConfig.ColumnsCongifs.ContainsKey(vehicleInformation.DatabaseId))
                {
                    string[] colArr = dedoumConfig.ColumnsCongifs[vehicleInformation.DatabaseId].Split(',');
                    for (int j = 0; j < colArr.Length; j++)
                    {
                      cols.Add(WebApplicationParameters.GenericColumnItemsInformation.Get(Convert.ToInt64(colArr[j]))); 
                    }
                }

      
                //Get data
                CoreLayer cl = WebApplicationParameters.CoreLayers[Constantes.Web.Layers.Id.insertionsDAL];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the insertions DAL"));
                _dalLayer = (IInsertionsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null, null);

                DataSet ds = _dalLayer.GetCreativesData(vehicleInformation, Convert.ToInt32(fromDate),
                                                        Convert.ToInt32(toDate), cols);
           
                //Build zip file
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                     destDirname = fileName;
                     zipFilename = destDirname + ".zip";                   
                     currentDir = Path.Combine(OUT_PUT_PATH,destDirname); tempDir = "";

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        //Create final directory structure
                        if (dr["associated_file"] != DBNull.Value)
                        {
                            if (!Directory.Exists(currentDir)) Directory.CreateDirectory(currentDir);

                            if (lv != null && lv.Count > 0)
                            {
                                for (int i = 0; i < lv.Count; i++)
                                {
                                    tempDir += dr[cols[i].DataBaseField].ToString() + @"\";
                                    if (!Directory.Exists(currentDir + @"\" + tempDir))
                                        Directory.CreateDirectory(currentDir + @"\" + tempDir);
                                }
                            }

                             fileToCopy = Path.Combine(dedoumConfig.CreativesSourcePath,
                                                             dr["associated_file"].ToString());
                            FileAttributes fas = File.GetAttributes(fileToCopy);
                            //Check if creatives is directory and copy all is content
                            if ((fas & FileAttributes.Directory) == FileAttributes.Directory)
                            {
                                creativeDirName = Path.GetFileNameWithoutExtension(fileToCopy);
                                if (!string.IsNullOrEmpty(creativeDirName))
                                {
                                    targetPath = Path.Combine(currentDir + @"\" + tempDir, creativeDirName);
                                    if (Directory.Exists(fileToCopy))
                                    {
                                        string[] files = Directory.GetFiles(fileToCopy);
                                        // Copy the files and overwrite destination files if they already exist.
                                        foreach (string s in files)
                                        {
                                            // Use static Path methods to extract only the file name from the path.
                                            string fName = Path.GetFileName(s);
                                            if (!string.IsNullOrEmpty(fName))
                                            {
                                                if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
                                                string destFile = Path.Combine(targetPath, fName);
                                                copyFile = new FileInfo(s);
                                                copyFile.CopyTo(destFile, true);
                                                copyFile = null;
                                                hasFiles = true;

                                            }
                                        }
                                    }
                                }                                
                            }
                            else
                            {
                                //If creative is file copy it target path
                                string creativeName = Path.GetFileName(dr["associated_file"].ToString());
                                if (!string.IsNullOrEmpty(creativeName))
                                {
                                    string fileNameDestination = Path.Combine(currentDir + @"\" + tempDir,
                                                                              creativeName);
                                    if (File.Exists(fileToCopy))
                                    {
                                        copyFile = new FileInfo(fileToCopy);
                                        copyFile.CopyTo(fileNameDestination, true);
                                        copyFile = null;
                                        hasFiles = true;
                                    }
                                }
                            }
                            tempDir = "";

                        }

                    }

                    if (hasFiles)
                    {
                        try{
                        //ZIP root directory
                        fsOut = File.Create(Path.Combine(OUT_PUT_PATH ,zipFilename));
                        zipStream = new ZipOutputStream(fsOut);
                        zipStream.SetLevel(9); //0-9, 9 being the highest level of compression
                        zipStream.Password = null;
                        // This setting will strip the leading part of the folder path in the entries, to
                        // make the entries relative to the starting folder.
                        // To include the full path for each entry up to the drive root, assign folderOffset = 0.
                        int folderOffset = currentDir.Length + (currentDir.EndsWith("\\") ? 0 : 1);
                        FileCompression.CompressFolder(currentDir, zipStream, folderOffset);
                        zipStream.IsStreamOwner = true;	// Makes the Close also Close the underlying stream
                        zipStream.Close();

                        //Copy zip t ostorage server
                        if (!Directory.Exists(dedoumConfig.CreativesPath + @"\" + rqDetails["ID_LOGIN"].ToString()))
                            Directory.CreateDirectory(dedoumConfig.CreativesPath + @"\" + rqDetails["ID_LOGIN"].ToString());
                        File.Copy(Path.Combine(OUT_PUT_PATH, zipFilename), Path.Combine(dedoumConfig.CreativesPath + @"\" + rqDetails["ID_LOGIN"].ToString(), zipFilename), true);

                        //Delete local Zip file                       
                        File.Delete(Path.Combine(OUT_PUT_PATH, zipFilename));
                        Directory.Delete(Path.Combine(OUT_PUT_PATH , destDirname), true);
                        return hasFiles;
                        }
                        catch (Exception ex)
                        {
                            throw new DedoumException("Impossible to generate ZIP file in plugin Dedoum" + ex.Message, ex);

                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw new DedoumException("Impossible to create Creatives ZIP file in plugin Dedoum" + err.Message, err);

            }

            return false;
        }
    }
}
