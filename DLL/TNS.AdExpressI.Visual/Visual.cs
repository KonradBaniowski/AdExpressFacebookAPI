#region Information
/*
 * Author : A.Rousseau
 * Created on 01/04/2011
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
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Constantes.Classification.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using System.IO;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Visual
{
    public abstract class Visual : IVisual
    {
        #region Variables
        /// <summary>
        /// Vehicle identifier
        /// </summary>
        protected Int64 _idVehicle;
        /// <summary>
        /// Relative Path
        /// </summary>
        protected string _relativePath = string.Empty;
        /// <summary>
        /// ID User session
        /// </summary>
        protected string _idSession = null;
        /// <summary>
        /// Is  path encrypted
        /// </summary>
        protected bool _isEncrypted = false;
        /// <summary>
        /// Theme
        /// </summary>
        protected string _theme = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="idVehicle">Vehicle identifier</param>
        /// <param name="relativePath">Relative Path</param>
        public Visual(Int64 idVehicle, string relativePath) {
            _idVehicle = idVehicle;
            _relativePath = relativePath;
        }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="idVehicle">Vehicle identifier</param>
        /// <param name="relativePath">Relative Path</param>
        /// <param name="idSession">ID Session</param>
        /// <param name="isEncrypted">true if is encrypted</param>
        public Visual(Int64 idVehicle, string relativePath,string idSession, bool isEncrypted)
        {
            _idVehicle = idVehicle;
            _relativePath = relativePath;
            _idSession = idSession;           
            _isEncrypted = isEncrypted;           

        }
        #endregion

        #region IVisual Members
        /// <summary>
        /// Theme
        /// </summary>
       public string Theme {
           set { _theme = value; }
       }

        /// <summary>
        /// Get virtual path
        /// </summary>
        /// <param name="isBlur">Is Blur or not</param>
        /// <returns>Virtual path</returns>
        public virtual string GetVirtualPath(bool isBlur) {

            switch (VehiclesInformation.Get(_idVehicle).Id) {
                case Vehicles.names.press:
                case Vehicles.names.internationalPress:
                case Vehicles.names.magazine:
                case Vehicles.names.newspaper:
                   return GetImagesPath(VehiclesInformation.Get(_idVehicle).Id) + "/" + _relativePath;                  
                default:
                    return null;
            }

        }

        /// <summary>
        /// Get binaries visual
        /// </summary>
        /// <param name="isBlur">Is Blur or not</param>
        /// <returns>Binaries visual</returns>
        public virtual byte[] GetBinaries(bool isBlur) {
            if (File.Exists(Path.Combine(WebCst.CreationServerPathes.LOCAL_PATH_IMAGE, _relativePath)))
                return File.ReadAllBytes(Path.Combine(WebCst.CreationServerPathes.LOCAL_PATH_IMAGE, _relativePath));
            else
                return null;
        }

        /// <summary>
        /// Get Content Type
        /// </summary>
        /// <returns>Content Type string </returns>
        public virtual string GetContentType()
        {
            return "image/jpeg";
        }

        /// <summary>
        ///Add Header
        /// </summary>
        /// <returns>Content header string </returns>
        public virtual string AddHeader()
        {
            return string.Empty;
        }

        #endregion

         /// <summary>
        /// Get images path
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        protected virtual string GetImagesPath(TNS.AdExpress.Constantes.Classification.DB.Vehicles.names vehicleId)
        {
            switch (vehicleId)
            {
                case Vehicles.names.press:
                case Vehicles.names.internationalPress:
                case Vehicles.names.magazine:
                case Vehicles.names.newspaper:
                    return WebCst.CreationServerPathes.IMAGES;
                case Vehicles.names.outdoor:
                    return WebCst.CreationServerPathes.IMAGES_OUTDOOR;
                case Vehicles.names.internet:
                    return WebCst.CreationServerPathes.CREA_ADNETTRACK;
                case Vehicles.names.editorial:
                    return WebCst.CreationServerPathes.IMAGES_EDITORIAL;
                default:
                    throw (new TNS.AdExpressI.Visual.Exceptions.VisualException("Unable to determine vehicle ID"));
            }
        }

    }
}
