
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpressI.Visual.WebService.CreativeView;
using WebCst = TNS.AdExpress.Constantes.Web;
using System.IO;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Visual.Russia.WebService
{
    public class Visual : AdExpressI.Visual.WebService.Visual
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="idVehicle">Vehicle identifier</param>
        /// <param name="relativePath">Relative Path</param>
        public Visual(Int64 idVehicle, string relativePath)
            : base(idVehicle, relativePath)
        {
        }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="idVehicle">Vehicle identifier</param>
        /// <param name="relativePath">Relative Path</param>
        /// <param name="idSession">ID Session</param>
        /// <param name="isEncrypted">true if is encrypted</param>
        public Visual(Int64 idVehicle, string relativePath, string idSession, bool isEncrypted)
            : base(idVehicle, relativePath, idSession, isEncrypted)
        {
        }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="idVehicle">Vehicle identifier</param>
        /// <param name="relativePath">Relative Path</param>
        /// <param name="idSession">ID Session</param>
        /// <param name="isCover">Is cover</param>
        /// <param name="isEncrypted">true if is encrypted</param>
        public Visual(Int64 idVehicle, string relativePath, string idSession, bool isEncrypted, bool isCover)
            : base(idVehicle, relativePath, idSession, isEncrypted, isCover)
        {
        }
        #endregion

        #region IVisual Members
        /// <summary>
        /// Get virtual path
        /// </summary>
        /// <param name="isBlur">Is Blur or not</param>
        /// <returns>Virtual path</returns>
        public override string GetVirtualPath(bool isBlur)
        {
            string url = "";
            switch (VehiclesInformation.Get(_idVehicle).Id)
            {
                case Vehicles.names.press:
                case Vehicles.names.pressClipping:
                case Vehicles.names.internationalPress:
                case Vehicles.names.outdoor:
                case Vehicles.names.internet:
                case Vehicles.names.editorial:
                    url = "/Private/CreativeView.aspx?path=" + _relativePath + "&id_vehicle=" + _idVehicle + "&is_blur=" + isBlur;
                    if (!string.IsNullOrEmpty(_idSession)) url += "&idSession=" + _idSession;
                    url += (_isEncrypted) ? "&crypt=1" : "&crypt=0";
                    return url;
                default:
                    return null;
            }

        }

        /// <summary>
        /// Get binaries visual
        /// </summary>
        /// <param name="isBlur">Is Blur or not</param>
        /// <returns>Binaries visual</returns>
        public override byte[] GetBinaries(bool isBlur)
        {
            if (!string.IsNullOrEmpty(_idSession) && !string.IsNullOrEmpty(_relativePath) && _idSession.Substring(0, 8) == DateTime.Now.ToString("yyyyMMdd"))
            {
                var tempRelative = GetRelativePath();
                var a = GetWebService();
                var res = a.GetBinaries(tempRelative, _idVehicle, isBlur, _isCover);
                if (res != null)
                {
                    return res;
                }
                else if (VehiclesInformation.Get(_idVehicle).Id == Vehicles.names.internet && (!string.IsNullOrEmpty(_relativePath) && Path.GetExtension(_relativePath).Equals(".swf"))
                    && File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Images\Common\pixel.gif"))
                {
                    return File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"\Images\Common\pixel.gif");
                }
                else if (!string.IsNullOrEmpty(_theme) && File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\App_Themes\" + _theme + @"\Images\Culture\Others\no_visuel.gif"))
                {
                    return File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"\App_Themes\" + _theme + @"\Images\Culture\Others\no_visuel.gif");
                }
                else return null;
            }
            return null;
        }

        /// <summary>
        /// Get Content Type
        /// </summary>
        /// <returns>Content Type string </returns>
        public override string GetContentType()
        {
            if (_isEncrypted)
            {
                //Decrypt path parameter if required
                _relativePath = TNS.AdExpress.Web.Functions.QueryStringEncryption.DecryptQueryString(_relativePath);
                _isEncrypted = false;
            }
            string extension = Path.GetExtension(_relativePath).ToUpper();
            switch (extension)
            {
                case ".JPEG":
                case ".JPG": return "image/jpeg";
                case ".GIF": return "image/gif";
                case ".SWF": return "application/x-shockwave-flash";
                case ".PNG": return "image/png";
                case ".AVI": return "video/x-msvideo";
                case ".WAV": return "audio/x-wav";
                default: return "image/gif";
            }

        }

        /// <summary>
        ///Add Header
        /// </summary>
        /// <returns>Content header string </returns>
        public override string AddHeader()
        {
            string extension = Path.GetExtension(_relativePath);
            return "attachment; filename=CreativeView" + extension;
        }
        /// <summary>
        /// Get Is File Exist or not
        /// </summary>
        /// <returns>Is file Exist or not</returns>
        public override bool IsExist()
        {
            if (!string.IsNullOrEmpty(_idSession) && !string.IsNullOrEmpty(_relativePath) && _idSession.Substring(0, 8) == DateTime.Now.ToString("yyyyMMdd"))
            {
                var tempRelative = GetRelativePath();
                var a = GetWebService();
                return a.GetIsExist(tempRelative, _idVehicle, _isCover);
            }
            return false;
        }

        #endregion


        protected string GetRelativePath()
        {
            string tempRelative;
            switch (VehiclesInformation.Get(_idVehicle).Id)
            {
                case Vehicles.names.press:
                    tempRelative = _isCover
                                       ? GetPathOtherVehicle(WebCst.CreationServerPathes.IMAGES_PRESS_COVER)
                                       : GetPathOtherVehicle(WebCst.CreationServerPathes.IMAGES);
                    break;
                case Vehicles.names.pressClipping:
                    tempRelative = GetPathOtherVehicle(WebCst.CreationServerPathes.IMAGES_PRESS_CLIPPING);
                    break;
                case Vehicles.names.outdoor:
                    tempRelative = GetPathOtherVehicle(WebCst.CreationServerPathes.IMAGES_OUTDOOR);
                    break;
                case Vehicles.names.internet:
                    tempRelative = GetPathOtherVehicle(WebCst.CreationServerPathes.CREA_ADNETTRACK);
                    break;
                case Vehicles.names.editorial:
                    tempRelative = GetPathOtherVehicle(WebCst.CreationServerPathes.IMAGES_EDITORIAL);
                    break;            
                case Vehicles.names.tv:
                case Vehicles.names.tvGeneral:
                case Vehicles.names.tvSponsorship:
                case Vehicles.names.tvAnnounces:
                case Vehicles.names.tvNonTerrestrials:
                case Vehicles.names.radio:
                case Vehicles.names.radioGeneral:
                case Vehicles.names.radioMusic:
                case Vehicles.names.radioSponsorship:
                    tempRelative = GetPathForTvOrRadio();
                    break;
                default:
                    return null;
            }

            return tempRelative;
        }

        /// <summary>
        /// Get path
        /// </summary>
        /// <param name="serverPath">server Path</param>
        /// <returns>path</returns>
        protected string GetPathOtherVehicle(string serverPath)
        {
            if (_isEncrypted)
            {
                //Decrypt path parameter if required
                _relativePath = AdExpress.Web.Functions.QueryStringEncryption.DecryptQueryString(_relativePath);
                _isEncrypted = false;
            }

            var tempRelative = _relativePath.Replace(serverPath + "/", "");
            tempRelative = tempRelative.Replace("/", @"\");
            return tempRelative;
        }
        /// <summary>
        /// Get path
        /// </summary>
        /// <returns>path</returns>
        protected string GetPathForTvOrRadio()
        {
            string tempRelative;
            if (_isEncrypted)
            {
                //Decrypt path parameter if required
                tempRelative = AdExpress.Web.Functions.QueryStringEncryption.DecryptQueryString(_relativePath);
                _isEncrypted = false;
            }
            else
                tempRelative = _relativePath;
            int advertismentId = int.Parse((_relativePath.Split('.'))[0]);
            var baseDirectory = ((advertismentId / 10000) * 10000).ToString();
            tempRelative = baseDirectory + @"\" + tempRelative;
            return tempRelative;
        }
    }
}
