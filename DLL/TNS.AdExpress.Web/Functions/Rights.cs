#region Informations
// Auteur:D. V. Mussuma
// Création: 05/06/2007
// Modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using CstDB = TNS.AdExpress.Constantes.DB;

using CstClassification = TNS.AdExpress.Constantes.Classification;

namespace TNS.AdExpress.Web.Functions {
	/// <summary>
	/// Function to manage site rights 
	/// </summary>
	public class Rights {

		/// <summary>
		/// NE PAS UTILISER !!!!!!!!!!!
		/// </summary>
		/// <param name="webSession">User web session</param>
		/// <param name="vehicle">media</param>
		/// <returns>True if user has access to creation</returns>
        [Obsolete("You can't use this metod. Use TNS.AdExpress.Web.Core.WebRight.ShowCreatives", true)]
		public static bool CanAccessToCreation(WebSession webSession, CstClassification.DB.Vehicles.names vehicle) {
			
			switch (vehicle) {
				case CstClassification.DB.Vehicles.names.press:
					return (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_PRESS_CREATION_ACCESS_FLAG) != null);
				case CstClassification.DB.Vehicles.names.internationalPress:
					return (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_INTERNATIONAL_PRESS_CREATION_ACCESS_FLAG) != null);
				case CstClassification.DB.Vehicles.names.outdoor:
					return (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_OUTDOOR_CREATION_ACCESS_FLAG) != null);
				case CstClassification.DB.Vehicles.names.tv:
					return (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_TV_CREATION_ACCESS_FLAG) != null);
                case CstClassification.DB.Vehicles.names.others:
                    return (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_OTHERS_CREATION_ACCESS_FLAG) != null);
				case CstClassification.DB.Vehicles.names.radio:
					return (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_RADIO_CREATION_ACCESS_FLAG) != null);
				case CstClassification.DB.Vehicles.names.internet:
					return (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_INTERNET_ACCESS_FLAG) != null);
				case CstClassification.DB.Vehicles.names.directMarketing:
					return (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DIRECT_MARKETING_CREATION_ACCESS_FLAG) != null);
				default: return false;
			}
		}
	}
}
