using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Linq;
using System.Web.Services;
using TNS.AdExpress.Constantes.Web;
using System.IO;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.JamesBond.DAL;
using TNS.AdExpress.WebService.Domain.Configuration;
using System.Drawing;
using TNS.FrameWork.Exceptions;
using TNSMail = TNS.FrameWork.Net.Mail;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;
using Image = Utilities.Media.Image.Image;

namespace WebServiceCreativeView
{
    /// <summary>
    /// Description résumée de Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Pour autoriser l'appel de ce service Web depuis un script à l'aide d'ASP.NET AJAX, supprimez les marques de commentaire de la ligne suivante. 
    // [System.Web.Script.Services.ScriptService]
    public class CreativeView : System.Web.Services.WebService
    {
        #region Constante
        /// <summary>
        /// Name of the configuration directory
        /// </summary>
        private const string CONFIGARION_DIRECTORY_NAME = "Configuration";
        #endregion

        #region Get Binaries
        /// <summary>
        /// Get Binaries of the creative
        /// </summary>
        /// <param name="relativePath">Relative path file</param>
        /// <param name="idVehicle">Vehicle identifier</param>
        /// <param name="isBlur">Return creative real (if false) or blur (if true)</param>
        /// <param name="isCover">true if creative is a cover</param>
        /// <returns>Binaries of the creative</returns>
        [WebMethod]
        public byte[] GetBinaries(string relativePath, Int64 idVehicle, bool isBlur, bool isCover)
        {
            VehicleCreativesInformation vehicleCreativesInformation = null;

            try
            {

                vehicleCreativesInformation = VehiclesCreativesInformation.GetVehicleCreativesInformation(idVehicle);
                if (vehicleCreativesInformation != null)
                {
                    vehicleCreativesInformation.Open();
                    var creativeInfoPath = isCover ? vehicleCreativesInformation.CreativeInfo.CoverPath : vehicleCreativesInformation.CreativeInfo.Path;
                    if (File.Exists(Path.Combine(creativeInfoPath, relativePath)))
                    {
                        return GetCreativeByte(vehicleCreativesInformation, creativeInfoPath, relativePath, idVehicle, isBlur);
                    }
                }

            }
            catch (Exception exc)
            {
                string body = "";
                string pathConf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIGARION_DIRECTORY_NAME);
                string countryName = WebParamtersXL.LoadDirectoryName(new XmlReaderDataSource(System.IO.Path.Combine(pathConf, ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME)));
                string pathConfCountry = Path.Combine(pathConf, countryName);


                try
                {
                    var err = (BaseException)exc;
                    body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>A error occure in the AdExpress creative webservice.</font><br>Error" + err.GetHtmlDetail() + "</font>";
                    if (!string.IsNullOrEmpty(relativePath)) body += "<br><b><u>File path:</u></b><font color=#008000>" + relativePath + "</font>";
                    body += "<br><b><u>Id Media Type :</u></b><font color=#008000>" + idVehicle + "</font>";
                    body += "</html>";
                }
                catch (System.Exception)
                {
                    try
                    {
                        body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>A error occure in the AdExpress creative webservice.</font><br>Erreur(" + exc.GetType().FullName + "):" + exc.Message + "<br><br><b><u>Source:</u></b><font color=#008000>" + exc.StackTrace.Replace("at ", "<br>at ") + "</font>";
                        if (!string.IsNullOrEmpty(relativePath)) body += "<br><b><u>File path:</u></b><font color=#008000>" + relativePath + "</font>";
                        body += "<br><b><u>Id Media Type :</u></b><font color=#008000>" + idVehicle + "</font>";
                        body += "</html>";
                    }
                    catch (System.Exception)
                    {
                        body = "Undefined Exception";
                    }
                }
                var errorMail = new TNSMail.SmtpUtilities(pathConfCountry + @"\" + TNS.AdExpress.Constantes.Web.ErrorManager.WEBSERVER_ERROR_MAIL_FILE);
                errorMail.SendWithoutThread("Error AdExpress Creative WebService  " + (Server.MachineName), body, true, false);
            }
            finally
            {
                if (vehicleCreativesInformation != null)
                    vehicleCreativesInformation.Close();
            }
            return null;
        }
        #endregion

        #region GetIsExist
        /// <summary>
        /// Get Binaries of the creative
        /// </summary>
        /// <param name="relativePath">Relative path file</param>
        /// <param name="idVehicle">Vehicle identifier</param>
        /// <param name="isCover">true if creative is a cover</param>
        /// <returns>Binaries of the creative</returns>
        [WebMethod]
        public bool GetIsExist(string relativePath, Int64 idVehicle, bool isCover)
        {
            VehicleCreativesInformation vehicleCreativesInformation = null;

            try
            {

                vehicleCreativesInformation = VehiclesCreativesInformation.GetVehicleCreativesInformation(idVehicle);
                if (vehicleCreativesInformation != null)
                {
                    vehicleCreativesInformation.Open();
                    var creativeInfoPath = isCover ? vehicleCreativesInformation.CreativeInfo.CoverPath : vehicleCreativesInformation.CreativeInfo.Path;
                    return (File.Exists(Path.Combine(creativeInfoPath, relativePath)));
                }

            }
            catch (Exception exc)
            {
                string body = "";
                string pathConf = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIGARION_DIRECTORY_NAME);
                string countryName = WebParamtersXL.LoadDirectoryName(new XmlReaderDataSource(Path.Combine(pathConf, ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME)));
                string pathConfCountry = System.IO.Path.Combine(pathConf, countryName);


                try
                {
                    var err = (BaseException)exc;
                    body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>A error occure in the AdExpress creative webservice.</font><br>Error" + err.GetHtmlDetail() + "</font>";
                    if (!string.IsNullOrEmpty(relativePath)) body += "<br><b><u>File path:</u></b><font color=#008000>" + relativePath + "</font>";
                    body += "<br><b><u>Id Media Type :</u></b><font color=#008000>" + idVehicle + "</font>";
                    body += "</html>";
                }
                catch (System.Exception)
                {
                    try
                    {
                        body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>A error occure in the AdExpress creative webservice.</font><br>Erreur(" + exc.GetType().FullName + "):" + exc.Message + "<br><br><b><u>Source:</u></b><font color=#008000>" + exc.StackTrace.Replace("at ", "<br>at ") + "</font>";
                        if (!string.IsNullOrEmpty(relativePath)) body += "<br><b><u>File path:</u></b><font color=#008000>" + relativePath + "</font>";
                        body += "<br><b><u>Id Media Type :</u></b><font color=#008000>" + idVehicle + "</font>";
                        body += "</html>";
                    }
                    catch (System.Exception)
                    {
                        body = "Undefined Exception";
                    }
                }
                var errorMail = new TNSMail.SmtpUtilities(pathConfCountry + @"\" + ErrorManager.WEBSERVER_ERROR_MAIL_FILE);
                errorMail.SendWithoutThread("Error AdExpress Creative WebService  " + (Server.MachineName), body, true, false);
            }
            finally
            {
                if (vehicleCreativesInformation != null)
                    vehicleCreativesInformation.Close();
            }
            return false;
        }
        #endregion

        #region GetCreativePath
        [WebMethod]
        public string GetCreativePath(string idVersion, Int64 idVehicle)
        {
            VehicleCreativesInformation vehicleCreativesInformation = null;
            try
            {
                idVersion = DecryptIdVersion(idVersion);

                vehicleCreativesInformation = VehiclesCreativesInformation.GetVehicleCreativesInformation(idVehicle);
                if (vehicleCreativesInformation != null)
                {
                    vehicleCreativesInformation.Open();
                    return GetPath(idVersion, idVehicle, vehicleCreativesInformation);
                }
            }
            catch (Exception exc)
            {

                string body = "";
                string pathConf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIGARION_DIRECTORY_NAME);
                string countryName = WebParamtersXL.LoadDirectoryName(new XmlReaderDataSource(Path.Combine(pathConf, ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME)));
                string pathConfCountry = Path.Combine(pathConf, countryName);


                try
                {
                    var err = (BaseException)exc;
                    body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>A error occure in the AdExpress creative webservice.</font><br>Error" + err.GetHtmlDetail() + "</font>";
                    if (!string.IsNullOrEmpty(idVersion)) body += "<br><b><u>ID path:</u></b><font color=#008000>" + idVersion + "</font>";
                    body += "<br><b><u>Id Media Type :</u></b><font color=#008000>" + idVehicle + "</font>";
                    body += "</html>";
                }
                catch (System.Exception)
                {
                    try
                    {
                        body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>A error occure in the AdExpress creative webservice.</font><br>Erreur(" + exc.GetType().FullName + "):"
                            + exc.Message + "<br><br><b><u>Source:</u></b><font color=#008000>" + exc.StackTrace.Replace("at ", "<br>at ") + "</font>";
                        if (!string.IsNullOrEmpty(idVersion)) body += "<br><b><u>ID Version path:</u></b><font color=#008000>" + idVersion + "</font>";
                        body += "<br><b><u>Id Media Type :</u></b><font color=#008000>" + idVehicle + "</font>";
                        body += "</html>";
                    }
                    catch (System.Exception)
                    {
                        body = "Undefined Exception";
                    }
                }
                var errorMail = new TNSMail.SmtpUtilities(pathConfCountry + @"\" + ErrorManager.WEBSERVER_ERROR_MAIL_FILE);
                errorMail.SendWithoutThread("Error AdExpress Creative WebService  " + (Server.MachineName), body, true, false);
            }
            finally
            {
                if (vehicleCreativesInformation != null)
                    vehicleCreativesInformation.Close();
            }
            return string.Empty;
        }

        private static string DecryptIdVersion(string idVersion)
        {
               //Decrypt id verion
            if (string.IsNullOrEmpty(idVersion)) throw new ArgumentNullException("Parameter idVersion cannot be null");
            idVersion = TNS.AdExpress.Web.Core.Utilities.QueryStringEncryption.DecryptQueryString(idVersion);
            return idVersion;
        }


        private string GetPath(string idVersion, Int64 idVehicle, VehicleCreativesInformation vehicleCreativesInformation)
        {
            switch (idVehicle)
            {
                case 2:
                    return GetRadioPath(idVersion, idVehicle, vehicleCreativesInformation);
                case 3:
                    return GetTvPath(idVersion, idVehicle, vehicleCreativesInformation);
            }
            return string.Empty;

        }


        private string GetRadioPath(string idVersion, Int64 idVehicle, VehicleCreativesInformation vehicleCreativesInformation)
        {
            string file = string.Empty;


            if (!string.IsNullOrEmpty(idVersion))
            {
                CreationServerPathes.READ_WM_CREATIVES_RADIO_SERVER = "mms://streamwm.secodip.com/stream/fr/wma";//TODO mettre en configuration
                CreationServerPathes.READ_REAL_CREATIVES_RADIO_SERVER = "rtsp://streamrl.secodip.com/stream/fr/ra";//TODO mettre en configuration
                CreationServerPathes.READ_WM_RADIO_SERVER = "mms://streamwm.secodip.com/radiostream";//TODO mettre en configuration
                CreationServerPathes.READ_REAL_RADIO_SERVER = "rtsp://streamrl.secodip.com/radiostream";//TODO mettre en configuration
              
                //Manages  WM file
                string tempPath = vehicleCreativesInformation.CreativeInfo.Path + @"wma\2\" + idVersion.Substring(0, 3) + @"\2" + idVersion + ".wma";
                if (File.Exists(tempPath))
                {
                    return CreationServerPathes.READ_WM_CREATIVES_RADIO_SERVER + "/2/" + idVersion.Substring(0, 3) +
                           "/2" + idVersion + ".wma";
                }
                var creativeDAL = new CreativeDAL(idVersion, idVehicle);
                DataSet ds = creativeDAL.GetRadioData();


                if (ds != null && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["ASSOCIATED_FILE"] != DBNull.Value)
                {
                    file = ds.Tables[0].Rows[0]["ASSOCIATED_FILE"].ToString();
                }
                if (!string.IsNullOrEmpty(file)  && File.Exists(CreationServerPathes.LOCAL_PATH_RADIO + (file.Replace("/", "\\")).Replace("rm", "wma")))
                {                          
                    return CreationServerPathes.READ_WM_RADIO_SERVER + file.Replace("rm", "wma");
                }

                //Manages  RM file
                tempPath = vehicleCreativesInformation.CreativeInfo.Path + @"ra\2\" + idVersion.Substring(0, 3) + @"\2" + idVersion + ".ra";
                if (File.Exists(tempPath))
                {
                    return CreationServerPathes.READ_REAL_CREATIVES_RADIO_SERVER + "/2/" + idVersion.Substring(0, 3) +
                           "/2" + idVersion + ".ra";
                }
                if (!string.IsNullOrEmpty(file) && File.Exists(CreationServerPathes.LOCAL_PATH_RADIO + file.Replace("/", "\\")))
                {                   
                    return CreationServerPathes.READ_REAL_RADIO_SERVER + file;
                }
            }
            return string.Empty;

        }

        private string GetTvPath(string idVersion, Int64 idVehicle, VehicleCreativesInformation vehicleCreativesInformation)
        {
            if (!string.IsNullOrEmpty(idVersion))
            {
                CreationServerPathes.READ_WM_TV_SERVER = @"mms://streamwm.secodip.com/stream/fr/wmv";//TODO mettre en configuration
                CreationServerPathes.READ_REAL_TV_SERVER = @"rtsp://streamrl.secodip.com/stream/fr/rm";//TODO mettre en configuration

                //Manages  WM file
                string tempPath = vehicleCreativesInformation.CreativeInfo.Path + @"wmv\3\" + idVersion.Substring(0, 3) + @"\3" + idVersion + ".wmv";
                if (File.Exists(tempPath))
                {
                    return CreationServerPathes.READ_WM_TV_SERVER + "/3/" + idVersion.Substring(0, 3) + "/3" + idVersion + ".wmv";
                }
                //Manages  RM file
                tempPath = vehicleCreativesInformation.CreativeInfo.Path + @"rm\3\" + idVersion.Substring(0, 3) + @"\3" + idVersion + ".rm";
                if (File.Exists(tempPath))
                {
                    return CreationServerPathes.READ_REAL_TV_SERVER + "/3/" + idVersion.Substring(0, 3) + "/3" + idVersion + ".rm";
                }
            }
            return string.Empty;

        }
        #endregion

        #region GetCreativeBinaries

        [WebMethod]
        public object[] GetCreativeBinaries(string idVersion, Int64 idVehicle)
        {
            VehicleCreativesInformation vehicleCreativesInformation = null;
            try
            {              
                idVersion = DecryptIdVersion(idVersion);

                List<string> list = GetPressPathes(idVersion, idVehicle);
                var listByte = new List<byte[]>();

                vehicleCreativesInformation = VehiclesCreativesInformation.GetVehicleCreativesInformation(idVehicle);
                if (vehicleCreativesInformation != null)
                {
                    vehicleCreativesInformation.Open();
                    var creativeInfoPath = vehicleCreativesInformation.CreativeInfo.Path;
                    listByte.AddRange(from file in list where File.Exists(Path.Combine(creativeInfoPath, file)) select GetCreativeByte(vehicleCreativesInformation, creativeInfoPath, file, idVehicle, false));
                }

                if (listByte.Count > 0)
                {
                    var res = new object[listByte.Count];
                    for (int i = 0; i < listByte.Count; i++)
                    {
                        res[i] = listByte[i];
                    }
                    return res;
                }

            }
            catch (Exception exc)
            {

                string body = "";
                string pathConf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIGARION_DIRECTORY_NAME);
                string countryName = WebParamtersXL.LoadDirectoryName(new XmlReaderDataSource(Path.Combine(pathConf, ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME)));
                string pathConfCountry = Path.Combine(pathConf, countryName);

                try
                {
                    var err = (BaseException)exc;
                    body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>A error occure in the AdExpress creative webservice.</font><br>Error" + err.GetHtmlDetail() + "</font>";
                    if (!string.IsNullOrEmpty(idVersion)) body += "<br><b><u>ID path:</u></b><font color=#008000>" + idVersion + "</font>";
                    body += "<br><b><u>Id Media Type :</u></b><font color=#008000>" + idVehicle + "</font>";
                    body += "</html>";
                }
                catch (System.Exception)
                {
                    try
                    {
                        body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>A error occure in the AdExpress creative webservice.</font><br>Erreur(" + exc.GetType().FullName + "):" + exc.Message + "<br><br><b><u>Source:</u></b><font color=#008000>" + exc.StackTrace.Replace("at ", "<br>at ") + "</font>";
                        if (!string.IsNullOrEmpty(idVersion)) body += "<br><b><u>ID Version path:</u></b><font color=#008000>" + idVersion + "</font>";
                        body += "<br><b><u>Id Media Type :</u></b><font color=#008000>" + idVehicle + "</font>";
                        body += "</html>";
                    }
                    catch (System.Exception)
                    {
                        body = "Undefined Exception";
                    }
                }
                var errorMail = new TNSMail.SmtpUtilities(pathConfCountry + @"\" + ErrorManager.WEBSERVER_ERROR_MAIL_FILE);
                errorMail.SendWithoutThread("Error AdExpress Creative WebService  " + (Server.MachineName), body, true, false);
            }
            finally
            {
                if (vehicleCreativesInformation != null)
                    vehicleCreativesInformation.Close();
            }
            return null;

        }

        private List<string> GetPressPathes(string idVersion, Int64 idVehicle)
        {


            var creativeDAL = new CreativeDAL(idVersion, idVehicle);
            List<string> list = null;
            DataSet ds = creativeDAL.GetPressData();

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                String idMedia = dr["id_media"].ToString();
                string[] files = dr["visual"].ToString().Split(',');              
                list = new List<string>();

                string[] mediaList = null;
                try
                {
                    mediaList = Media.GetItemsList(AdExpressUniverse.CREATIVES_KIOSQUE_LIST_ID).MediaList.Split(',');
                }
                catch (Exception) { }

                long disponibility = 50;
                if (dr["disponibility_visual"] != System.DBNull.Value)
                {
                    disponibility = Convert.ToInt64(dr["disponibility_visual"]);
                }
                if (disponibility <= 10)
                {
                    if (Array.IndexOf(mediaList, idMedia.ToString()) > -1)
                    {
                        for (int fileIndex = 0; fileIndex < files.Length; fileIndex++)
                        {
                            if (files[fileIndex].Length > 0)
                            {
                                string path = string.Format(@"{0}\{1}\{2}", idMedia, dr["date_media_num"].ToString(), files[fileIndex]);
                                list.Add(path);
                            }
                        }
                    }
                    else
                    {

                        for (int fileIndex = 0; fileIndex < files.Length; fileIndex++)
                        {
                            if (files[fileIndex].Length > 0)
                            {
                                string path = string.Format(@"{0}\{1}\{2}", idMedia, dr["date_cover_num"].ToString(), files[fileIndex]);
                                list.Add(path);
                            }
                        }
                    }
                }


            }
            return list;
        }
        #endregion

        #region GetCreativeByte

        /// <summary>
        /// Get Creative Byte
        /// </summary>
        /// <param name="vehicleCreativesInformation">vehicle Creatives Information</param>
        /// <param name="creativeInfoPath">creative Info Path</param>
        /// <param name="relativePath">relative Path</param>
        /// <param name="idVehicle">idVehicle</param>
        /// <param name="isBlur">isBlur</param>
        /// <returns>Binaries of the creative</returns>
        protected byte[] GetCreativeByte(VehicleCreativesInformation vehicleCreativesInformation,
                                         string creativeInfoPath, string relativePath, Int64 idVehicle, bool isBlur)
        {
            byte[] imageBytes = null;
            var pathFile = Path.Combine(creativeInfoPath, relativePath);
            if (isBlur)
            {
                MemoryStream fs = null;
                BinaryReader br = null;
                try
                {
                    var bitmap = new Bitmap(pathFile);
                    var bitmapBlur = Image.BoxBlur(bitmap, 5);

                    fs = new MemoryStream();
                    bitmapBlur.Save(fs, ImageFormat.Jpeg);
                    imageBytes = fs.ToArray();
                }
                finally
                {
                    if (br != null) br.Close();
                    if (fs != null) fs.Close();
                }
            }
            else
            {
                imageBytes = File.ReadAllBytes(pathFile);
            }
            return imageBytes;

        }

        #endregion

    }
}
