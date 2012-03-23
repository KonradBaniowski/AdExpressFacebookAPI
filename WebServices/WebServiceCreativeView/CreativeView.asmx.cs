using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Services;
using TNS.AdExpress.Constantes.Web;
using System.IO;
using TNS.AdExpress.WebService.Domain;
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
        public byte[] GetBinaries(string relativePath, Int64 idVehicle, bool isBlur,bool isCover)
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
                string pathConf = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIGARION_DIRECTORY_NAME);
                string countryName = WebParamtersXL.LoadDirectoryName(new XmlReaderDataSource(System.IO.Path.Combine(pathConf, TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME)));
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
                var errorMail = new TNSMail.SmtpUtilities(pathConfCountry + @"\" + TNS.AdExpress.Constantes.Web.ErrorManager.WEBSERVER_ERROR_MAIL_FILE);
                errorMail.SendWithoutThread("Error AdExpress Creative WebService  " + (Server.MachineName), body, true, false);
                return null;
            }
            finally
            {
                if (vehicleCreativesInformation != null)
                    vehicleCreativesInformation.Close();
            }
            return null;
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
