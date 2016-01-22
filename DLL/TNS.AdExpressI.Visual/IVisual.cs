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
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpressI.Visual
{
    public interface IVisual
    {
        /// <summary>
        /// Theme
        /// </summary>
        string Theme { set; }
        /// <summary>
        /// Get virtual path
        /// </summary>
        /// <param name="isBlur">Is Blur or not</param>
        /// <returns>Virtual path</returns>
        string GetVirtualPath(bool isBlur);
        /// <summary>
        /// Get binaries visual
        /// </summary>
        /// <param name="isBlur">Is Blur or not</param>
        /// <returns>Binaries visual</returns>
        byte[] GetBinaries(bool isBlur);
        /// <summary>
        /// Get Is File Exist or not
        /// </summary>
        /// <returns>Is file Exist or not</returns>
        bool IsExist();
        /// <summary>
        /// Get Content Type
        /// </summary>
        /// <returns>Content Type string </returns>
        string GetContentType();
        /// <summary>
        /// Add header
        /// </summary>
        /// <returns></returns>
        string AddHeader();
    }
}
