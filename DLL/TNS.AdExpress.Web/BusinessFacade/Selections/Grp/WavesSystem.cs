#region Informations
// Auteur: D. V. Mussuma
// Date de création: 24/08/2005 
//Date de modification :
#endregion

using System;
using System.Windows.Forms;
using TNS.AdExpress.Web.Core.Sessions;
using SelectionsGrp=TNS.AdExpress.Web.Rules.Selections.Grp;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Web.BusinessFacade.Selections.Grp
{
	/// <summary>
	/// Sélection des vagues APPM
	/// </summary>
	public class WavesSystem
	{	
		/// <summary>
		/// Obtient une vague automatiquement en fonction de la date de début sélectionnée par le client.
		/// </summary>
		/// <param name="webSession">sesion du client</param>
		/// <param name="dataSource">source de données</param>
		/// <returns>vague sélectionnée</returns>
		public static TreeNode GetWaves(WebSession webSession, IDataSource dataSource){	
			return SelectionsGrp.WavesRules.GetWaves(webSession,dataSource);
		}
	}
}
