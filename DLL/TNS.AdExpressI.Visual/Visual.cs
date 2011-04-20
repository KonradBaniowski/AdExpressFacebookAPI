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
        #endregion

        #region IVisual Members
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
                    return WebCst.CreationServerPathes.IMAGES + "/" + _relativePath;
                    break;
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
        #endregion

    }
}
