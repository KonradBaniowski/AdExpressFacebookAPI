#region Information
// Author: D. Mussuma
// Creation date: 11/08/2008
// Modification date:
#endregion
using System;
using System.Data;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Result;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using FrameWorkResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Constantes.FrameWork.Results;
using ExcelFunction = TNS.AdExpress.Web.UI.ExcelWebPage;

using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.Portofolio.Russia.Engines {
	/// <summary>
	/// Compute portofolio structure's results
	/// </summary>
	public class StructureEngine : TNS.AdExpressI.Portofolio.Engines.StructureEngine{

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicleInformation">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public StructureEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList, bool excel)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd, hourBeginningList, hourEndList, excel) {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicleInformation">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		/// <param name="ventilationTypeList">Ventilation type List</param>
		public StructureEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, List<PortofolioStructure.Ventilation> ventilationTypeList,bool excel)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd, ventilationTypeList, excel) {
		}
		#endregion

        #region GetHourIntervalWebWordCode
        /// <summary>
        ///Get hour interval web word code 
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<string, int> GetHourIntervalWebWordCode()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("0509", 2759);
            dic.Add("0609", 2750);
            dic.Add("0912", 2751);
            dic.Add("1215", 2752);
            dic.Add("1518", 2753);
            dic.Add("1821", 2754);
            dic.Add("2124", 2755);
            dic.Add("2405", 2760);
            dic.Add("2406", 2756);
            return dic;

        }
        #endregion

        #region GetPressStructureHtml
        /// <summary>
        /// Get press structure html
        /// </summary>
        /// <returns></returns>
        protected override string GetPressStructureHtml()
        {
            StringBuilder t = new StringBuilder(5000);
            string classCss = (_excel) ? "acl21" : "acl2";
            DataSet ds = null;
            DataTable dt = null;
            int oldIdVentilationType = -1;
            int labelCode;
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo;
            List<UnitInformation> unitInformationList = _webSession.GetValidUnitForResult();
            Dictionary<int, CellUnitFactory> cellFactoryList = new Dictionary<int, CellUnitFactory>();
           

            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[6];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            parameters[5] = _ventilationTypeList;

            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
            ds = portofolioDAL.GetData();

            // Selection recall
            if (_excel)
            {
                t.Append(ExcelFunction.GetLogo(_webSession));
                t.Append(ExcelFunction.GetExcelHeader(_webSession, GestionWeb.GetWebWord(1379, _webSession.SiteLanguage)));
            }

            if (ds != null && ds.Tables.Count>0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                //Get cell factory list
                for (int i = 0; i < unitInformationList.Count; i++)
                {
                    cellFactoryList.Add(unitInformationList[i].Id.GetHashCode(), GetCellFactory(unitInformationList[i]));
                }

                //Build result table
                dt = ds.Tables[0];
                t.Append("<table class=\"backGroundWhite\" border=0 cellpadding=0 cellspacing=0 >");
                CellUnit cellUnit = null;
                foreach (DataRow dr in dt.Rows)
                {
                    if (oldIdVentilationType != int.Parse(dr["ventilationType"].ToString()))
                    {
                        switch ((FrameWorkResultConstantes.PortofolioStructure.Ventilation)long.Parse(dr["ventilationType"].ToString()))
                        {                        
                            case FrameWorkResultConstantes.PortofolioStructure.Ventilation.format:
                                labelCode = 1420;
                                break;                           
                            case FrameWorkResultConstantes.PortofolioStructure.Ventilation.location:
                                labelCode = 1439;
                                break;
                            case FrameWorkResultConstantes.PortofolioStructure.Ventilation.design:
                                labelCode = 2764;
                                break;
                            case FrameWorkResultConstantes.PortofolioStructure.Ventilation.nonStandardPlacement:
                                labelCode = 2765;
                                break;
                            default:
                                throw new PortofolioException("GetVentilationLines: Ventilation type unknown.");
                        }
                        //labels
                        t.Append("\r\n\t<tr  onmouseover=\"this.className='backGroundWhite';\" onmouseout=\"this.className='violetBackGroundV3';\"  class=\"violetBackGroundV3\" height=\"20px\" >");
                        t.Append("\r\n\t<td align=\"left\" class=\"p2\" nowrap><b>" + GestionWeb.GetWebWord(labelCode, _webSession.SiteLanguage) + "</b></td>");
                       
                        //Unit columns labels
                        foreach (UnitInformation unitInformation in unitInformationList)
                        {
                            t.Append("\r\n\t<td  class=\"p2\"  nowrap>" + GestionWeb.GetWebWord(unitInformation.WebTextId, _webSession.SiteLanguage) + "</td>");
                        }
                        t.Append("</tr>");
                    }
                    //Units values		
                    t.Append("\r\n\t<tr  onmouseover=\"this.className='backGroundWhite';\" onmouseout=\"this.className='violetBackGroundV3';\"  class=\"violetBackGroundV3\" height=\"20px\" >");
                    if (dr["ventilation"] != null && dr["ventilation"] != DBNull.Value)
                        t.Append("\r\n\t<td align=\"left\" class=\"" + classCss + "\" nowrap>&nbsp;&nbsp;&nbsp;" + dr["ventilation"].ToString() + "</td>");
                    else t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>&nbsp;</td>");                    
                    foreach (UnitInformation unitInformation in unitInformationList)
                    {
                        if (dr[unitInformation.Id.ToString()] != null && dr[unitInformation.Id.ToString()] != System.DBNull.Value)
                        {
                            //t.Append("\r\n\t<td align=\"right\" class=\"" + classCss + "\" nowrap>" + WebFunctions.Units.ConvertUnitValueAndPdmToString(dr[unitInformation.Id.ToString()], unitInformation.Id, false, fp) + "</td>");


                             cellUnit = cellFactoryList[unitInformation.Id.GetHashCode()].Get(Convert.ToDouble(dr[unitInformation.Id.ToString()].ToString()));
                            t.Append("\r\n\t<td align=\"right\" class=\"" + classCss + "\" nowrap>" + cellUnit.ToString(unitInformation.StringFormat, fp) + "</td>");                            
                        }
                        else
                        {
                            //t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>&nbsp;</td>");
                             cellUnit = cellFactoryList[unitInformation.Id.GetHashCode()].Get(null);
                            t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>" + cellUnit.ToString(unitInformation.StringFormat, fp) + "</td>");

                        }
                    }
                    t.Append("</tr>");

                    oldIdVentilationType = int.Parse(dr["ventilationType"].ToString());
                }
                t.Append("</table>");
            }
            else t.Append(GetNoDataMessageHtml());
            if (_excel)
            {
                t.Append(ExcelFunction.GetFooter(_webSession));
            }
            return t.ToString();
        }
        #endregion

        #region GetStructureHtml
        /// <summary>
        /// Get structure html
        /// <remarks>Used currently for vehicle , tv,others an radio</remarks>
        /// </summary>
        /// <returns></returns>
        protected override string GetStructureHtml()
        {
            StringBuilder t = new StringBuilder(5000);
            DataSet ds = null;
            DataTable dt = null;
            List<UnitInformation> unitInformationList = new List<UnitInformation>();
            string P2 = "p2";
            string backGround = "backGroundWhite";
            string classCss = "acl1";
            string hourIntervallLabel = "";
            double? totalUnit = null;
            Dictionary<int, CellUnitFactory> cellFactoryList = new Dictionary<int, CellUnitFactory>();

            IFormatProvider fp = (_excel) ?
                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfoExcel
                : WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo;

            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[7];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            parameters[5] = _hourBeginningList;
            parameters[6] = _hourEndList;

            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
            ds = portofolioDAL.GetData();


            // Selection recall
            if (_excel)
            {
                t.Append(ExcelFunction.GetLogo(_webSession));
                t.Append(ExcelFunction.GetExcelHeader(_webSession, GestionWeb.GetWebWord(1379, _webSession.SiteLanguage)));
            }

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                dt = ds.Tables[0];

                unitInformationList = _webSession.GetValidUnitForResult();

                t.Append("<table class=\"backGroundWhite\" border=0 cellpadding=0 cellspacing=0 >");

                #region libellés colonnes
                // Première ligne
                t.Append("\r\n\t<tr height=\"20px\" >");
                if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioGeneral
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioSponsorship
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radioMusic)
                    t.Append("<td class=\"" + P2 + "\" nowrap>" + GestionWeb.GetWebWord(1299, _webSession.SiteLanguage) + "</td>");
                else if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
                         || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvGeneral
                         || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvSponsorship
                         || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvNonTerrestrials
                         || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tvAnnounces
                         || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others)
                    t.Append("<td class=\"" + P2 + "\" nowrap>" + GestionWeb.GetWebWord(1451, _webSession.SiteLanguage) + "</td>");

                for (int i = 0; i < unitInformationList.Count; i++)
                {
                    t.Append("<td class=\"" + P2 + "\" nowrap>" + GestionWeb.GetWebWord(unitInformationList[i].WebTextId, _webSession.SiteLanguage) + "</td>");
                }

                t.Append("</tr>");
                #endregion
                //Get cell factory list
                for (int i = 0; i < unitInformationList.Count; i++)
                {
                    cellFactoryList.Add(unitInformationList[i].Id.GetHashCode(), GetCellFactory(unitInformationList[i]));
                }

                CellUnit cellUnit = null;
                t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.className='backGroundWhite';\" onmouseout=\"this.className='" + backGround + "';\"  class=\"" + backGround + "\" height=\"20px\" >");
                //time interval										
                t.Append("\r\n\t<td align=\"left\" class=\"" + classCss + "\" nowrap>" + GestionWeb.GetWebWord(1401, _webSession.SiteLanguage) + "</td>");
                ////line total units
                for (int i = 0; i < unitInformationList.Count; i++)
                {
                    totalUnit = null;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[unitInformationList[i].Id.ToString()] != System.DBNull.Value && dr[unitInformationList[i].Id.ToString()] != null)
                        {
                            if (!totalUnit.HasValue) totalUnit = 0;
                            totalUnit += double.Parse(dr[unitInformationList[i].Id.ToString()].ToString());
                        }
                    }
                    if (!_excel || unitInformationList[i].Id != WebCst.CustomerSessions.Unit.duration)
                    {
                        cellUnit = cellFactoryList[unitInformationList[i].Id.GetHashCode()].Get(totalUnit);
                        t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>" + cellUnit.ToString(unitInformationList[i].StringFormat, fp) + "</td>");
                        //t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>" + WebFunctions.Units.ConvertUnitValueAndPdmToString(totalUnit, unitInformationList[i].Id, false, fp) + "</td>");
                    }
                    else
                    {
                        if (totalUnit.HasValue)
                        t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>" + string.Format(fp, unitInformationList[i].StringFormat, totalUnit) + "</td>");
                        else t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>&nbsp;</td>");
                    }
                }

                t.Append("</tr>");
                backGround = "violetBackGroundV3";

                //One line by time interval
                foreach (DataRow dr in dt.Rows)
                {
                    classCss = "acl1";

                    hourIntervallLabel = string.Empty;

                    if (_excel) hourIntervallLabel = "&nbsp;";

                    hourIntervallLabel += GestionWeb.GetWebWord(GetHourIntervalWebWordCode()[dr["HourInterval"].ToString()], _webSession.SiteLanguage);

                    t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.className='backGroundWhite';\" onmouseout=\"this.className='" + backGround + "';\"  class=\"" + backGround + "\" height=\"20px\" >");
                    //time interval										
                    t.Append("\r\n\t<td align=\"left\" class=\"" + classCss + "\" nowrap>" + hourIntervallLabel + "</td>");
                    //Unit Value
                    for (int i = 0; i < unitInformationList.Count; i++)
                    {
                        if (!_excel || unitInformationList[i].Id != WebCst.CustomerSessions.Unit.duration)
                        {
                            if (dr[unitInformationList[i].Id.ToString()] != System.DBNull.Value && dr[unitInformationList[i].Id.ToString()] != null)                        
                            cellUnit = cellFactoryList[unitInformationList[i].Id.GetHashCode()].Get(Convert.ToDouble(dr[unitInformationList[i].Id.ToString()].ToString()));
                            else cellUnit = cellFactoryList[unitInformationList[i].Id.GetHashCode()].Get(null);
                            t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>" + cellUnit.ToString(unitInformationList[i].StringFormat, fp) + "</td>");
                        }
                        else
                        {
                            if (dr[unitInformationList[i].Id.ToString()] != System.DBNull.Value && dr[unitInformationList[i].Id.ToString()] != null)                        
                            t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>" + string.Format(fp, unitInformationList[i].StringFormat, Convert.ToDouble(dr[unitInformationList[i].Id.ToString()])) + "</td>");
                            else t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>&nbsp;</td>");
                        }
                    }
                    t.Append("</tr>");

                    classCss = "acl2";
                    //backGround = "violetBackGroundV3";
                }
                t.Append("</table>");

            }
            else
            {
                t.Append("<br><table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
                t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
                t.AppendFormat("{0}", GestionWeb.GetWebWord(177, _webSession.SiteLanguage));
                t.Append("</td></tr></table>");
            }

            if (_excel)
            {
                t.Append(ExcelFunction.GetFooter(_webSession));
            }
            return t.ToString();
        }

        
        #endregion

        #region Compute data for chart for press
        /// <summary>
        /// Compute data for preess chart results 
        /// </summary>
        /// <returns>Data set for preess chart</returns>
        protected override DataTable ComputePressChartData()
        {
            DataSet ds = null;
            DataTable dt = null, dtResult = null;
            DataRow newRow = null;
            int labelCode = 0;
            long oldIdVentilationType = -1;

            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[6];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            parameters[5] = _ventilationTypeList;

            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
            ds = portofolioDAL.GetData();

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                dt = ds.Tables[0];

                dtResult = new DataTable();
                dtResult.Columns.Add("idUnit", System.Type.GetType("System.Int64"));
                dtResult.Columns.Add("unitLabel", System.Type.GetType("System.String"));
                dtResult.Columns.Add("chartDataLabel", System.Type.GetType("System.String"));
                dtResult.Columns.Add("chartDataValue", System.Type.GetType("System.Double"));
                UnitInformation unitInformation = UnitsInformation.Get(WebCst.CustomerSessions.Unit.insertion);
                foreach (DataRow dr in dt.Rows)
                {
                    if (oldIdVentilationType != int.Parse(dr["ventilationType"].ToString()))
                    {
                        switch ((FrameWorkResultConstantes.PortofolioStructure.Ventilation)long.Parse(dr["ventilationType"].ToString()))
                        {
                            case FrameWorkResultConstantes.PortofolioStructure.Ventilation.format:
                                labelCode = 1420;
                                break;
                            case FrameWorkResultConstantes.PortofolioStructure.Ventilation.location:
                                labelCode = 1439;
                                break;
                            case FrameWorkResultConstantes.PortofolioStructure.Ventilation.design:
                                labelCode = 2764;
                                break;
                            case FrameWorkResultConstantes.PortofolioStructure.Ventilation.nonStandardPlacement:
                                labelCode = 2765;
                                break;
                            default:
                                throw new PortofolioException("GetVentilationLines: Ventilation type unknown.");
                        }
                    }
                    newRow = dtResult.NewRow();
                    dtResult.Rows.Add(newRow);

                    newRow["idUnit"] = long.Parse(dr["ventilationType"].ToString());
                    newRow["unitLabel"] = GestionWeb.GetWebWord(labelCode, _webSession.SiteLanguage);
                    newRow["chartDataLabel"] = dr["ventilation"].ToString();
                    newRow["chartDataValue"] = (dr[unitInformation.Id.ToString()] != null && dr[unitInformation.Id.ToString()] != System.DBNull.Value) ? double.Parse(dr[unitInformation.Id.ToString()].ToString()) : 0;

                    oldIdVentilationType = long.Parse(dr["ventilationType"].ToString());

                }
            }

            return dtResult;

        }


        protected virtual CellUnitFactory GetCellFactory(UnitInformation unit){
            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(unit.Assembly);
            Type type = assembly.GetType(unit.CellType);
            Cell cellUnit = (Cell)type.InvokeMember("GetInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, null, null);
            cellUnit.StringFormat = unit.StringFormat;
            return (new CellUnitFactory((CellUnit)cellUnit));
        }
        #endregion

    }
}
