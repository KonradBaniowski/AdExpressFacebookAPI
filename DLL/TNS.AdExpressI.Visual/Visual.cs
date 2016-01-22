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
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Constantes.Classification.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using System.IO;

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
        /// <summary>
        /// Is creative's cover
        /// </summary>
        protected bool _isCover = false;
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
            :this(idVehicle,relativePath)
        {          
            _idSession = idSession;           
            _isEncrypted = isEncrypted;           

        }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="idVehicle">Vehicle identifier</param>
        /// <param name="relativePath">Relative Path</param>
        /// <param name="idSession">ID Session</param>
        /// <param name="isEncrypted">true if is encrypted</param>
        /// <param name="isCover">Is cover</param>
        public Visual(Int64 idVehicle, string relativePath, string idSession, bool isEncrypted,bool isCover)
            :this(idVehicle,relativePath,idSession,isEncrypted)
        {       
            _isCover = isCover;

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
        /// Get Is File Exist or not
        /// </summary>
        /// <returns>Is file Exist or not</returns>
        public virtual bool IsExist()
        {
            return (File.Exists(Path.Combine(WebCst.CreationServerPathes.LOCAL_PATH_IMAGE, _relativePath)));
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
        protected virtual string GetImagesPath(Vehicles.names vehicleId)
        {
            switch (vehicleId)
            {
                case Vehicles.names.press:
                case Vehicles.names.internationalPress:
                case Vehicles.names.magazine:
                case Vehicles.names.newspaper:
                    return WebCst.CreationServerPathes.IMAGES;
                case Vehicles.names.indoor:
                case Vehicles.names.outdoor:
                    return WebCst.CreationServerPathes.IMAGES_OUTDOOR;
                case Vehicles.names.internet:
                    return WebCst.CreationServerPathes.CREA_ADNETTRACK;
                case Vehicles.names.editorial:
                    return WebCst.CreationServerPathes.IMAGES_EDITORIAL;
                default:
                    throw (new Exceptions.VisualException("Unable to determine vehicle ID"));
            }
        }

    }
}
