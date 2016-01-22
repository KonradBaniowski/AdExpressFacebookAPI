#region Informations
// Auteur: G. Facon 
// Date de cr�ation: 13/07/2006
// Date de modification:
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using DomainResults = TNS.AdExpress.Domain.Results;

namespace TNS.AdExpress.Web.Common.Results
{


    ///<summary>
    /// Donn�es servant � construire un plan media
    /// </summary>
    ///  <since>13/07/2006</since>
    ///  <author>G. Facon</author>
    public class MediaPlanResultData : ResultData
    {

        #region Variables
        ///<summary>
        /// Liste des informations des versions utilis�es par le r�sultats
        /// </summary>
        ///  <since>13/07/2006</since>
        ///  <author>G. Facon</author>
        private Dictionary<Int64, DomainResults.VersionItem> _versionsDetail = new Dictionary<Int64, DomainResults.VersionItem>();
        #endregion

        #region Constructeurs
        ///<summary>
        /// Constructeur
        /// </summary>
        ///  <since>13/07/2006</since>
        ///  <author>G. Facon</author>
        public MediaPlanResultData()
            : base()
        {
        }
        #endregion

        #region Accesseurs
        ///<summary>
        /// Obtient la liste des informations des versions utilis�es par le r�sultat
        /// </summary>
        ///  <since>13/07/2006</since>
        ///  <author>G. Facon</author>
        public Dictionary<Int64, DomainResults.VersionItem> VersionsDetail
        {
            get { return (_versionsDetail); }
        }
        #endregion

        #region M�thodes public
        /// <summary>
        /// Obtient la liste des versions utilis�e par le r�sultat
        /// </summary>
        /// <remarks>Format: 25001,15002,32555</remarks>
        /// <returns>liste des versions utilis�e par le r�sultat</returns>
        public string GetVersionList()
        {
            StringBuilder list = new StringBuilder(1000);
            foreach (Int64 curentVersion in _versionsDetail.Keys)
            {
                list.Append(curentVersion.ToString() + ",");
            }
            string listString = list.ToString();
            if (listString.Length > 0) return (listString.Substring(0, listString.Length - 1));
            return (listString);
        }
        #endregion

    }
}
