#region Informations
// Auteur:D. V. Mussuma
// Création: 05/06/2007
// Modification:
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Web.Core.Sessions;
using CstDB = TNS.AdExpress.Constantes.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using CstClassification = TNS.AdExpress.Constantes.Classification;

namespace TNS.AdExpress.Web.Functions {
	/// <summary>
	/// Function to manage site rights 
	/// </summary>
	public class Rights {

        /// <summary>
        /// Check if Has Press Copyright
        /// </summary>
        /// <param name="idMedia"></param>
        /// <returns>true if has right</returns>
        public static  bool HasPressCopyright(long idMedia)
        {
            string ids = Lists.GetIdList(WebCst.GroupList.ID.media, WebCst.GroupList.Type.mediaExcludedForCopyright);
            if (!string.IsNullOrEmpty(ids))
            {
                var notAllowedMediaIds = ids.Split(',').Select(p => Convert.ToInt64(p)).ToList();
                return !notAllowedMediaIds.Contains(idMedia);
            }
            return true;
        }

        /// <summary>
        /// Check if parution date is before the year 2015
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool ParutionDateBefore2015(string date)
        {

            string year = date.Substring(0, 4);

            if (int.Parse(year) < 2015)
                return true;
            else
                return false;

        }
	}
}
