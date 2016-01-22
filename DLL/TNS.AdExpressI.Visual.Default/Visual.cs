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

namespace TNS.AdExpressI.Visual.Default
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
            : base(idVehicle, relativePath) {
        }
        #endregion

    }
}
