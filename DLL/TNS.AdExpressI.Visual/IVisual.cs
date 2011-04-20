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
    }
}
