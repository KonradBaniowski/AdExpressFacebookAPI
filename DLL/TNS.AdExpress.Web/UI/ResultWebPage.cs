#region Information
// Auteur : G Facon
// Créé le : 24/01/2005
// Modifié le : 24/01/2005
//		12/08/2005	G. Facon	Nom de variables
//		30/11/2005	D. Mussuma	Gestion des tableaux de bord
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExeptions=TNS.AdExpress.Web.Exceptions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Constantes.Classification.DB;
using TNSExceptions = TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Units;
using System.Collections.Generic;

namespace TNS.AdExpress.Web.UI{
	/// <summary>
	/// Page mère des page Web de résultats
	/// </summary>
    public class ResultWebPage : BaseResultWebPage {

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
        public ResultWebPage()
            : base() {
		}
		#endregion

		#region Evènement
        /// <summary>
        /// OnInitComplete
        /// </summary>
        /// <param name="e">Event Argument</param>       
        protected override void OnInitComplete(EventArgs e) {

            try {

                #region Variables
                string vehicleSelection = string.Empty;
                Int64 vehicleId = -1;
                VehicleInformation vehicleInformation = null;
                List<UnitInformation> unitInformationList = null;
                Dictionary<Constantes.Web.CustomerSessions.Unit, UnitInformation> unitInformationDictionary = null;
                List<Constantes.Web.CustomerSessions.Unit> unitList = null;
                System.Windows.Forms.TreeNode firstNode = null;
                #endregion

                #region Get Vehicle Selected
                vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
                if (WebFunctions.Modules.IsDashBoardModule(_webSession) && (string.IsNullOrEmpty(vehicleSelection) || vehicleSelection.IndexOf(",") > 0))
                {
                    firstNode = _webSession.SelectionUniversMedia.FirstNode;
                    vehicleSelection = ((LevelInformation)firstNode.Tag).ID.ToString();
                       
                }
                if (Int64.TryParse(vehicleSelection, out vehicleId)) {
                    vehicleInformation = VehiclesInformation.Get(vehicleId);
                    unitInformationList = _webSession.GetValidUnitForResult();
                    unitInformationDictionary = new Dictionary<TNS.AdExpress.Constantes.Web.CustomerSessions.Unit, UnitInformation>();
                    for (int i = 0; i < unitInformationList.Count; i++) {
                        unitInformationDictionary.Add(unitInformationList[i].Id, unitInformationList[i]);
                    }
                }
                else {
                     firstNode = _webSession.CurrentUniversMedia.FirstNode;
                    if (firstNode != null
                        && (((LevelInformation)firstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess
                        || ((LevelInformation)firstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.vehicleException
                        || ((LevelInformation)firstNode.Tag).Type == TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccessForRecap                        
                        )
                    )
                    {
                        Vehicles.names vehicleName = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)firstNode.Tag).ID);
                        vehicleInformation = VehiclesInformation.Get(vehicleName);
                        unitInformationList = _webSession.GetValidUnitForResult();
                        unitInformationDictionary = new Dictionary<TNS.AdExpress.Constantes.Web.CustomerSessions.Unit, UnitInformation>();
                        for (int i = 0; i < unitInformationList.Count; i++) {
                            unitInformationDictionary.Add(unitInformationList[i].Id, unitInformationList[i]);
                        }
                    }
                    else {
                        switch (_webSession.CurrentModule) {
                            case WebConstantes.Module.Name.JUSTIFICATIFS_PRESSE:
                            case WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE:
                            case WebConstantes.Module.Name.DONNEES_DE_CADRAGE:
                                vehicleInformation = VehiclesInformation.Get(Vehicles.names.press);
                                break;
                            case WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO:
                                vehicleInformation = VehiclesInformation.Get(Vehicles.names.radio);
                                break;
                            case WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION:
                                vehicleInformation = VehiclesInformation.Get(Vehicles.names.tv);
                                break;
                            case WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO:
                                vehicleInformation = VehiclesInformation.Get(Vehicles.names.others);
                                break;
                            case WebConstantes.Module.Name.TABLEAU_DE_BORD_EVALIANT:
                                vehicleInformation = VehiclesInformation.Get(Vehicles.names.adnettrack);
                                break;  
                            default: throw new TNSExceptions.ModuleException("The module '" + _webSession.CurrentModule + "' is not implemented for default unit");
                        }
                        unitInformationDictionary = new Dictionary<TNS.AdExpress.Constantes.Web.CustomerSessions.Unit, UnitInformation>();
                        unitList = vehicleInformation.AllowUnits.AllowUnitList;
                        unitInformationDictionary = new Dictionary<TNS.AdExpress.Constantes.Web.CustomerSessions.Unit, UnitInformation>();
                        for (int i = 0; i < unitList.Count; i++) {
                            unitInformationDictionary.Add(unitList[i], UnitsInformation.Get(unitList[i]));
                        }
                    }
                }
                #endregion

                #region Get Default Unit For Vehicle Selected
                if (!_webSession.ReachedModule || !unitInformationDictionary.ContainsKey(_webSession.Unit)) {
                    _webSession.Unit = ModulesList.GetModule(_webSession.CurrentModule).GetResultPageInformation(_webSession.CurrentTab).GetDefaultUnit(vehicleInformation.Id);
                }
                #endregion

            }
            catch (Exception ex) {
                if (ex.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, ex, _webSession));
                }
            }

            base.OnInitComplete(e);
        }
		#endregion

	}
}
