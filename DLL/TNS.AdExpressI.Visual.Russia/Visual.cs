#region Information
/*
 * Author : D.Mussuma
 * Created on 16/05/2011
 * Modifications :
 *      Author - Date - Description
 * 
 * 
 * */
#endregion
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Constantes.Classification.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using System.IO;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Visual.Exceptions;
namespace TNS.AdExpressI.Visual.Russia
{
    public class Visual : TNS.AdExpressI.Visual.Visual
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
        public Visual(Int64 idVehicle, string relativePath, string idSession, bool isEncrypted,bool isCover)
            : base(idVehicle, relativePath, idSession, isEncrypted,isCover)
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

            string localPath = string.Empty, tempRelative = string.Empty;
            int advertismentId = -1;
            string baseDirectory = string.Empty;

            if (!string.IsNullOrEmpty(_idSession) && !string.IsNullOrEmpty(_relativePath) && _idSession.Substring(0, 8) == DateTime.Now.ToString("yyyyMMdd"))
            {
                switch (VehiclesInformation.Get(_idVehicle).Id)
                {
                    case Vehicles.names.press:
                        if (_isEncrypted)
                        {
                            //Decrypt path parameter if required
                            _relativePath = TNS.AdExpress.Web.Functions.QueryStringEncryption.DecryptQueryString(_relativePath);
                            _isEncrypted = false;
                        }
                        if(_isCover)
                        {

                            localPath = WebCst.CreationServerPathes.LOCAL_PATH_IMAGES_COVER;
                            tempRelative = _relativePath.Replace(WebCst.CreationServerPathes.IMAGES_PRESS_COVER + "/", "");
                        }
                        else {
                            localPath = CreationServerPathes.LOCAL_PATH_IMAGE;
                            tempRelative = _relativePath.Replace(CreationServerPathes.IMAGES + "/", "");
                        }

                        tempRelative = tempRelative.Replace("/", @"\");
                        localPath = localPath + tempRelative;
                        break;
                    case Vehicles.names.outdoor:
                        if (_isEncrypted)
                        {
                            //Decrypt path parameter if required
                            _relativePath = TNS.AdExpress.Web.Functions.QueryStringEncryption.DecryptQueryString(_relativePath);
                            _isEncrypted = false;
                        }
                        localPath = WebCst.CreationServerPathes.LOCAL_PATH_OUTDOOR;
                        tempRelative = _relativePath.Replace(WebCst.CreationServerPathes.IMAGES_OUTDOOR + "/", "");
                        tempRelative = tempRelative.Replace("/", @"\");
                        localPath = localPath + tempRelative; break;

                    case Vehicles.names.internet:
                        localPath = WebCst.CreationServerPathes.LOCAL_PATH_INTERNET;
                        if (_isEncrypted)
                        {
                            //Decrypt path parameter if required
                            _relativePath = TNS.AdExpress.Web.Functions.QueryStringEncryption.DecryptQueryString(_relativePath);
                            _isEncrypted = false;
                        }
                        tempRelative = _relativePath.Replace(WebCst.CreationServerPathes.CREA_ADNETTRACK + "/", "");
                        tempRelative = tempRelative.Replace("/", @"\");
                        localPath = localPath + tempRelative;
                        break;
                    case Vehicles.names.editorial:
                        if (_isEncrypted)
                        {
                            //Decrypt path parameter if required
                            _relativePath = TNS.AdExpress.Web.Functions.QueryStringEncryption.DecryptQueryString(_relativePath);
                            _isEncrypted = false;
                        }
                        localPath = WebCst.CreationServerPathes.LOCAL_PATH_EDITORIAL;
                        tempRelative = _relativePath.Replace(WebCst.CreationServerPathes.IMAGES_EDITORIAL + "/", "");
                        tempRelative = tempRelative.Replace("/", @"\");
                        localPath = localPath + tempRelative;
                        break;
                    case Vehicles.names.tv:
                    case Vehicles.names.tvGeneral:
                    case Vehicles.names.tvSponsorship:
                    case Vehicles.names.tvAnnounces:
                    case Vehicles.names.tvNonTerrestrials:
                       
                        if (_isEncrypted)
                        {
                            //Decrypt path parameter if required
                            _relativePath = TNS.AdExpress.Web.Functions.QueryStringEncryption.DecryptQueryString(_relativePath);
                            _isEncrypted = false;
                        }
                        advertismentId = int.Parse((_relativePath.Split('.'))[0]);
                        baseDirectory = ((advertismentId / 10000) * 10000).ToString();  
                        localPath = WebCst.CreationServerPathes.LOCAL_PATH_VIDEO;
                        tempRelative = _relativePath.Replace(WebCst.CreationServerPathes.DOWNLOAD_TV_SERVER + "/", "");
                        tempRelative = tempRelative.Replace("/", @"\");
                        localPath = localPath + baseDirectory+@"\"+ tempRelative;
                        break;
                    case Vehicles.names.radio:
                    case Vehicles.names.radioGeneral:
                    case Vehicles.names.radioMusic:
                    case Vehicles.names.radioSponsorship:
                        if (_isEncrypted)
                        {
                            //Decrypt path parameter if required
                            _relativePath = TNS.AdExpress.Web.Functions.QueryStringEncryption.DecryptQueryString(_relativePath);
                            _isEncrypted = false;
                        }
                        advertismentId = int.Parse((_relativePath.Split('.'))[0]);
                        baseDirectory = ((advertismentId / 10000) * 10000).ToString();  
                        localPath = WebCst.CreationServerPathes.LOCAL_PATH_RADIO;
                        tempRelative = _relativePath.Replace(WebCst.CreationServerPathes.DOWNLOAD_RADIO_SERVER + "/", "");
                        tempRelative = tempRelative.Replace("/", @"\");
                        localPath = localPath + baseDirectory + @"\" + tempRelative;
                        break;
                    default:
                        return null;
                }
            }
            if (!string.IsNullOrEmpty(localPath) && File.Exists(localPath))
                return File.ReadAllBytes(localPath);
            else if (!string.IsNullOrEmpty(_theme) && File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\App_Themes\" + _theme + @"\Images\Culture\Others\no_visuel.gif"))
            {
                return File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"\App_Themes\" + _theme + @"\Images\Culture\Others\no_visuel.gif");
            }
            else
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
        #endregion

    }
}
