#region Informations
/*
 * Author : G Ragneau
 * Created on : 23/09/2008
 * Modifications :
 *      Date - Author - Description
 * 
 * 
 * 
 * 
 * 
 * */
/*
 * history: moved from TNS.AdExpress.Web
 * Auteur:D. V. Mussuma
 * Création: 12/12/2005
 * Modification:
 *      12/01/2006 B. Masson Ajout des niveaux de détail supports

 * */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using TNS.AdExpress.Web.Core.Sessions;

using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Core.Utilities{
	/// <summary>
	/// Fonctions des niveaux de détail média
	/// </summary>
	public class MediaDetailLevel{

		/// <summary>
		/// Get List of filters in generic detail levels
		/// </summary>
        /// <param name="session">User session</param>	
		/// <param name="idLevel1">Level 1 ID</param>
        /// <param name="idLevel2">Level 2 ID</param>
        /// <param name="idLevel3">Level 3 ID</param>
        /// <param name="idLevel4">Level 4 ID</param>
		/// <returns>List of levels matching filters</returns>
        public static Dictionary<DetailLevelItemInformation, long> GetFilters(WebSession session, Int64 idLevel1, Int64 idLevel2, Int64 idLevel3, Int64 idLevel4) {
            string temp = "";
            Dictionary<DetailLevelItemInformation, long> mediaCol = null;
            int indexLevel = 0;
            if (session.PreformatedMediaDetail.ToString().Length > 0)
            {
                temp = session.PreformatedMediaDetail.ToString().ToUpper();
                mediaCol = new Dictionary<DetailLevelItemInformation, long>();

                if (session.GenericMediaDetailLevel != null && session.GenericMediaDetailLevel.Levels != null && session.GenericMediaDetailLevel.Levels.Count > 0)
                {
                    for (int i = 0; i < session.GenericMediaDetailLevel.Levels.Count; i++)
                    {
                        if (session.GenericMediaDetailLevel.Levels[i] != null
                            && ((DetailLevelItemInformation)session.GenericMediaDetailLevel.Levels[i]).DataBaseIdField != null
                            && ((DetailLevelItemInformation)session.GenericMediaDetailLevel.Levels[i]).DataBaseIdField.Length > 0
                            ) {
                            indexLevel++;
                            switch (indexLevel) {
                                case 1:
                                    mediaCol.Add((DetailLevelItemInformation)session.GenericMediaDetailLevel.Levels[i], idLevel1); //identifiant du média est la clé
                                    break;
                                case 2:
                                    mediaCol.Add((DetailLevelItemInformation)session.GenericMediaDetailLevel.Levels[i], idLevel2);
                                    break;
                                case 3:
                                    mediaCol.Add((DetailLevelItemInformation)session.GenericMediaDetailLevel.Levels[i], idLevel3);
                                    break;
                                case 4:
                                    mediaCol.Add((DetailLevelItemInformation)session.GenericMediaDetailLevel.Levels[i], idLevel4);
                                    break;
                                default: 
                                    mediaCol.Add((DetailLevelItemInformation)session.GenericMediaDetailLevel.Levels[i], -1);
                                    break;
                            }
                        }
                    }
                }

            }
            return mediaCol;

        }
		
	}
}
