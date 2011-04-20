﻿#region Information
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

namespace TNS.AdExpressI.Visual.WebService
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
        #endregion

        #region IVisual Members
        /// <summary>
        /// Get virtual path
        /// </summary>
        /// <param name="isBlur">Is Blur or not</param>
        /// <returns>Virtual path</returns>
        public override string GetVirtualPath(bool isBlur) {

            switch (VehiclesInformation.Get(_idVehicle).Id) {
                case Vehicles.names.press:
                case Vehicles.names.internationalPress:
                case Vehicles.names.magazine:
                case Vehicles.names.newspaper:
                    return "/Private/CreativeView.aspx?path=" + _relativePath + "&id_vehicle=" + _idVehicle + "&is_blur=" + isBlur;
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
        public override byte[] GetBinaries(bool isBlur) {
            CreativeView.CreativeView a = new TNS.AdExpressI.Visual.WebService.CreativeView.CreativeView();
            return a.GetBinaries(_relativePath, _idVehicle, isBlur);
        }
        #endregion

    }
}