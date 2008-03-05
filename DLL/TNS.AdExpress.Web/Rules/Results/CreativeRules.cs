#region Info
/*
 * Author           : G RAGNEAU 
 * Date             : 21/08/2007
 * Modifications    :
 *      Author - Date - Description
 * 
 *  
 */
#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Windows.Forms;

using DBClassifCst = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.Common.Results.Creatives;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.FrameWork.WebResultUI.TableControl;
using CustCst = TNS.AdExpress.Constantes.Customer;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Exceptions;

namespace TNS.AdExpress.Web.Rules.Results {

    /// <summary>
    /// Get a table of Creatives depending on parameters
    /// </summary>
    public class CreativeRules {

        /// <summary>
        /// Get List of Creative Items
        /// </summary>
        /// <param name="session">Web Session</param>
        /// <param name="IdVehicle">Vehicle Id</param>
        /// <param name="filters">Custom Filters</param>
        /// <param name="fromDate">Period Beginning</param>
        /// <param name="toDate">Period End</param>
        /// <param name="universId">Competitor Univers Id</param>
        /// <param name="zoom">Zoom Period</param>
        /// <param name="moduleId">Identifiant du module</param>
        /// <param name="parameters">Chaine de caractère contenant des paramètres à afaire passer dans un lien quelconque.</param>
        /// <returns>List of creatives</returns>
        public static List<ITableElement> GetCreatives(WebSession session, int IdVehicle, string filters, int fromDate, int toDate, int universId, string zoom, Int64 moduleId, string parameters) {

            DBClassifCst.Vehicles.names vehicle = (DBClassifCst.Vehicles.names)IdVehicle;


            DataTable dt = CreativesDataAccess.GetData(session, vehicle, filters, fromDate, toDate, universId, moduleId).Tables[0];

            List<ITableElement> items = new List<ITableElement>();


            DataRow row;
            int rowCount = dt.Rows.Count;

            CreativeItem item = null;

            #region Set Creative Type
            switch (vehicle) {
                case DBClassifCst.Vehicles.names.adnettrack:
                    CreativeAdNetTrack itemAdNet = new CreativeAdNetTrack(-1);
                    itemAdNet.UrlParameters = filters;
                    itemAdNet.ZoomDate = zoom;
                    itemAdNet.UniversId = universId;
                    itemAdNet.ModuleId = moduleId;
                    itemAdNet.Parameters = parameters;
                    item = itemAdNet;       
                    break;
                case DBClassifCst.Vehicles.names.directMarketing:
                    item = new CreativeMailing(-1);
                    break;
                case DBClassifCst.Vehicles.names.internationalPress:
                case DBClassifCst.Vehicles.names.press:
                    item = new CreativePresse(-1);
                    break;
                case DBClassifCst.Vehicles.names.radio:
                    item = new CreativeRadio(-1);
                    break;
                case DBClassifCst.Vehicles.names.outdoor:
                    item = new CreativeOutdoor(-1);
                    break;
                case DBClassifCst.Vehicles.names.tv:
                case DBClassifCst.Vehicles.names.others:
                    item = new CreativeTV(-1);
                    ((CreativeTV)item).Vehicle = vehicle;
                    break;
            }
            #endregion

            for (int i = 0; i < rowCount; i++) {

                row = dt.Rows[i];

                items.Add(item.GetInstance(row, session));

            }

            return items;

        }


        /// <summary>
        /// Get Vehicle to purpose in Creative details
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="module">Current module</param>
        /// <param name="filters">Filters Ids</param>
        /// <param name="idModule">Module identifiant</param>
        /// <param name="universId">Current Univers Identifiant (concurrant environnement, -1 by default)</param>
        /// <returns>List of vehicle Ids</returns>
        public static List<int> GetVehicles(WebSession session, Int64 idModule, string filters, int universId) {

            #region Dates
            int dateBegin = 0;
            int dateEnd = 0;

            dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(session.PeriodBeginningDate, session.PeriodType).ToString("yyyyMMdd"));
            dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(session.PeriodEndDate, session.PeriodType).ToString("yyyyMMdd"));
            #endregion

            string list = string.Empty;
            List<int> vehicles = new List<int>();
            string[] ids = null;
            int id = -1;
            TreeNode tree = null;


            switch (idModule) {
                case WebCst.Module.Name.ALERTE_CONCURENTIELLE:
                case WebCst.Module.Name.ALERTE_POTENTIELS:
                    int positionUnivers = 1;
                    id = -1;
                    DataSet ds = null;
                    while ((tree = (TreeNode)session.CompetitorUniversMedia[positionUnivers]) != null) {
                        foreach (TreeNode nd in tree.Nodes) {

                            ds = MediaCreationDataAccess.GetIdsVehicle(session, -1, ((LevelInformation)nd.Tag).ID);
                            foreach (DataRow dr in ds.Tables[0].Rows) {
                                id = Convert.ToInt32(dr["id_vehicle"]);
                                if (id == DBClassifCst.Vehicles.names.internet.GetHashCode()) {
                                    //Remplacer média Internet par Adnettrack
                                    id = DBClassifCst.Vehicles.names.adnettrack.GetHashCode();
                                }
                                if ( ! vehicles.Contains(id))
                                    vehicles.Add(id);

                            }
                        }
                        positionUnivers++;
                    }
                    break;
                case WebCst.Module.Name.ALERTE_PORTEFEUILLE:
                case WebCst.Module.Name.ANALYSE_CONCURENTIELLE:
                case WebCst.Module.Name.ANALYSE_DYNAMIQUE:
                case WebCst.Module.Name.ANALYSE_PORTEFEUILLE:
                case WebCst.Module.Name.ANALYSE_POTENTIELS:
                    id = -1;
                    if (session.SelectionUniversMedia != null)
                        tree = session.SelectionUniversMedia;
                    id = (int)((LevelInformation)tree.Nodes[0].Tag).ID;
                    if (id == DBClassifCst.Vehicles.names.internet.GetHashCode()) {
                        //Remplacer média Internet par Adnettrack
                        id = DBClassifCst.Vehicles.names.adnettrack.GetHashCode();
                    }
                    vehicles.Add(id);
                    break;
                case WebCst.Module.Name.ALERTE_PLAN_MEDIA:
                case WebCst.Module.Name.ANALYSE_PLAN_MEDIA:
                    ids = filters.Split(',');
                    string idVehicle = string.Empty;
                    ArrayList tmp = new ArrayList();
                    GetImpactedVehicleIds(session, ids[0], ids[1], ids[2], ids[3], ref tmp, ref idVehicle, dateBegin, dateEnd);
                    id = -1;
                    foreach (object o in tmp) {
                        id = Convert.ToInt32(o);
                        if (id == DBClassifCst.Vehicles.names.internet.GetHashCode()) {
                            //Remplacer média Internet par Adnettrack
                            id = DBClassifCst.Vehicles.names.adnettrack.GetHashCode();
                        }
                        vehicles.Add(id);
                    }
                    break;
                case WebCst.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
                case WebCst.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
                    ids = filters.Split(',');
                    id = Convert.ToInt32(ids[0]);
                    if (id == DBClassifCst.Vehicles.names.internet.GetHashCode()) {
                        //Remplacer média Internet par Adnettrack
                        id = DBClassifCst.Vehicles.names.adnettrack.GetHashCode();
                    }
                    vehicles.Add(id);
                    break;
                case WebCst.Module.Name.ANALYSE_DES_DISPOSITIFS:
                case WebCst.Module.Name.ANALYSE_DES_PROGRAMMES:
                    vehicles.Add(DBClassifCst.Vehicles.names.tv.GetHashCode());
                    break;
            }

            if (vehicles.Count <= 0) {
                vehicles.Add(DBClassifCst.Vehicles.names.others.GetHashCode());
                vehicles.Add(DBClassifCst.Vehicles.names.directMarketing.GetHashCode());
                vehicles.Add(DBClassifCst.Vehicles.names.adnettrack.GetHashCode());
                vehicles.Add(DBClassifCst.Vehicles.names.press.GetHashCode());
                vehicles.Add(DBClassifCst.Vehicles.names.outdoor.GetHashCode());
                vehicles.Add(DBClassifCst.Vehicles.names.radio.GetHashCode());
                vehicles.Add(DBClassifCst.Vehicles.names.tv.GetHashCode());

            }

            return CreativesDataAccess.GetPresentVehicles(session, filters, dateBegin, dateEnd, universId, idModule, vehicles) ;
        }

        #region Liste des médias (vehicle) impactés
        /// <summary>
        /// Obtient la liste des médias (vehicle) impactés
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="idMediaLevel1">Média Niveau 1</param>
        /// <param name="idMediaLevel2">Média Niveau 2</param>
        /// <param name="idMediaLevel3">Média Niveau 3</param>
        /// <param name="idMediaLevel4">Média Niveau 4</param>
        /// <param name="vehicleArr">Liste des médias impactés</param>
        /// <param name="idVehicle">Média en cours</param>
        /// <param name="dateBegin">Period Beginning</param>
        /// <param name="dateEnd">Period End</param>
        public static void GetImpactedVehicleIds(WebSession webSession, string idMediaLevel1, string idMediaLevel2, string idMediaLevel3, string idMediaLevel4, ref ArrayList vehicleArr, ref string idVehicle, int dateBegin, int dateEnd) {

            #region variables
            ListDictionary mediaImpactedList = null;
            #endregion

            try {
                //Obtention média impactés				
                mediaImpactedList = Functions.MediaDetailLevel.GetImpactedMedia(webSession, long.Parse(idMediaLevel1), long.Parse(idMediaLevel2), long.Parse(idMediaLevel3), long.Parse(idMediaLevel4));
            }
            catch (System.Exception ex) {
                throw (new CreativeRulesException("Impossible d'obtenir les médias impactés par le détail média.", ex));
            }

            #region Identification de(s) média(s) (Vehicle)
            if (!webSession.isCompetitorAdvertiserSelected()) {
                vehicleArr = MediaInsertionsCreationsRules.GetIdsVehicle(webSession, mediaImpactedList, dateBegin.ToString(), dateEnd.ToString());

                if (idVehicle == null || idVehicle.Length == 0 || long.Parse(idVehicle) == -1) {

                    if (vehicleArr != null && vehicleArr.Count > 0) {
                        idVehicle = vehicleArr[0].ToString();
                    }
                }
            }
            else idVehicle = idMediaLevel1;
            #endregion
        }
        #endregion

    }
}

