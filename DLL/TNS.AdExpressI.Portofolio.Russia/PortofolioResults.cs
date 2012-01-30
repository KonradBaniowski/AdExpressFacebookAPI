#region Information
// Author: Y. Rkaian & D. Mussuma
// Creation date: 17/03/2007
// Modification date:
#endregion

using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using FrameWorkConstantes = TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using CustomerConstantes=TNS.AdExpress.Constantes.Customer;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Web.Navigation;
using WebCst=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Result;
using WebFunctions = TNS.AdExpress.Web.Functions;

using TNS.AdExpress.Domain.Level;
using DBCst=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;

using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpressI.Portofolio.VehicleView;

namespace TNS.AdExpressI.Portofolio.Russia {
    /// <summary>
    /// Portofolio Results
    /// </summary>
    public class PortofolioResults : TNS.AdExpressI.Portofolio.PortofolioResults {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
		public PortofolioResults(WebSession webSession):base(webSession) {
        }
	
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="adBreak">Screen code</param>
		/// <param name="dayOfWeek">Day of week</param>
		public PortofolioResults(WebSession webSession, string adBreak, string dayOfWeek)
            : base(webSession, adBreak, dayOfWeek) {
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>		
		/// <param name="tableType">tableType</param>
		public PortofolioResults(WebSession webSession,TNS.AdExpress.Constantes.DB.TableType.Type tableType):base(webSession, tableType) {
		}
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>		
        /// <param name="resultType">resultType</param>
        public PortofolioResults(WebSession webSession, int resultType)
            : base(webSession, resultType)
        {
        }
        #endregion

		#region Implementation of abstract methods
		/// <summary>
		/// Get ResultTable for some portofolio result
		///  - DETAIL_PORTOFOLIO
		///  - CALENDAR
		///  - SYNTHESIS (only result table)
		/// </summary>
		/// <param name="webSession">Customer session</param>
		/// <returns>Result Table</returns>
		public override ResultTable GetResultTable() {
            TNS.AdExpressI.Portofolio.Engines.Engine result = null;
			try {
				switch (_webSession.CurrentTab) {
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
						result = new Engines.PortofolioDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, _showInsertions, _showCreatives);
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
						result = new Engines.CalendarEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
						result = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
						break;
					default:
						throw (new PortofolioException("Impossible to identified current tab "));
				}
			}
			catch (System.Exception err) {
				throw (new PortofolioException("Impossible to compute portofolio results", err));
			}

			return result.GetResultTable();
		}

        ///// <summary>
        ///// Get view of the vehicle (HTML)
        ///// </summary>
        ///// <param name="excel">True for excel result</param>
        ///// <param name="resultType">Result Type (Synthesis, MediaDetail)</param>
        ///// <returns>HTML code</returns>
        //public override string GetVehicleViewHtml(bool excel, int resultType) {
        //    Engines.SynthesisEngine result = null;
        //    result = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
        //    return result.GetVehicleViewHtml(excel,resultType);
        //}			

		

		#region Structure
		/// <summary>
		/// Get Structure html result
		/// </summary>
		/// <param name="excel">True if export excel</param>
		/// <returns>html code</returns>
        public override string GetStructureHtml(bool excel) {
			Engines.StructureEngine result = null;
			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					List<FrameWorkResultConstantes.PortofolioStructure.Ventilation> ventilationTypeList = GetVentilationTypeList();					
					result = new Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, ventilationTypeList,excel);
					break;
				case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
					Dictionary<string, double> hourBeginningList = new Dictionary<string,double>();
					Dictionary<string, double> hourEndList = new Dictionary<string, double>();
					GetHourIntevalList(hourBeginningList, hourEndList);
                    result = new Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, hourBeginningList, hourEndList, excel);									
					break;
				default:
					throw new PortofolioException("Vehicle unknown.");
			}
			return result.GetHtmlResult();
		}


		/// <summary>
		/// Get structure chart data
		/// </summary>
		/// <returns></returns>
		public override DataTable GetStructureChartData() {
			Engines.StructureEngine result = null;

			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					List<FrameWorkResultConstantes.PortofolioStructure.Ventilation> ventilationTypeList = GetVentilationTypeList();
                    result = new Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, ventilationTypeList, false);
					break;
				case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
					Dictionary<string, double> hourBeginningList = new Dictionary<string, double>();
					Dictionary<string, double> hourEndList = new Dictionary<string, double>();
					GetHourIntevalList(hourBeginningList, hourEndList);
                    result = new Engines.StructureEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, hourBeginningList, hourEndList, false);
					break;
				default:
					throw new PortofolioException("Vehicle unknown.");
			}
			return result.GetChartData();
		}
		#endregion

		#region HTML detail media
		/// <summary>
		/// Get media detail html
		/// </summary>
		/// <param name="excel">true for excel result</param>
		/// <returns>HTML Code</returns>
		public override string GetDetailMediaHtml(bool excel) {
			Engines.MediaDetailEngine result = null;
			StringBuilder t = new StringBuilder(5000);

			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					result = new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
					result.GetAllPeriodInsertions(t, GestionWeb.GetWebWord(1837, _webSession.SiteLanguage));
                    //t.Append(result.GetVehicleViewHtml(excel, FrameWorkResultConstantes.Portofolio.DETAIL_MEDIA));
					return t.ToString();
				case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
					result = new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, excel);
					return result.GetHtmlResult();
				default:
					throw new PortofolioException("Vehicle unknown.");
			}
		}
		#endregion

		#region Insetion detail
		/// <summary>
		/// Get media insertion detail
		/// </summary>
		/// <returns></returns>
		public override ResultTable GetInsertionDetailResultTable(bool excel) {
			Engines.InsertionDetailEngine result = new Engines.InsertionDetailEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd, _adBreak, _dayOfWeek,excel);
			return result.GetResultTable();
		}
		#endregion	
	
        #region Gets Visula list
        /// <summary>
        /// Get cover list by publication dates
        /// </summary>
        /// <param name="beginDate">Begin date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Dates parution</returns>
        public override Dictionary<string, string> GetVisualList(string beginDate, string endDate)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[5];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = beginDate;
            parameters[4] = endDate;
            string themeName = TNS.AdExpress.Domain.Web.WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
            DataSet ds = portofolioDAL.GetListDate(true, _tableType);
          
            dic.Clear();
            
            //TODO : Would be implemented when Russian will have full scan of cover and according to their cover rules.
                  
            return dic;
        }
        #endregion

        #region GetVehicleItems
        /// <summary>
        /// Get vehicle cover items
        /// </summary>
        public override List<VehicleItem> GetVehicleItems()
        {
            #region Variables
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
            StringBuilder sb = new StringBuilder(5000);
            string pathWeb = "";
            CoverItem coverItem = null;
            CoverLinkItem coverLinkItem = null;
            CoverLinkItemSynthesis coverLinkItemSynthesis = null;
            VehicleItem vehicleItem = null;
            List<VehicleItem> itemsCollection = new List<VehicleItem>();
            DataSet ds = null;
            #endregion

            #region Accès aux tables
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[5];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

            ds = portofolioDAL.TableOfIssue();

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0] == null)
                return itemsCollection;

            DataTable dt = ds.Tables[0];           
            #endregion

            // Vérifie si le client a le droit aux créations
            if (_webSession.CustomerLogin.ShowCreatives(_vehicleInformation.Id))
            {

                if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                    || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                    )
                {


                    CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Localization);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //date_media_num

                        if (dt.Rows[i]["visual"] != System.DBNull.Value)
                            pathWeb = string.Empty;
                        else
                            pathWeb = "/App_Themes/" + themeName + "/Images/Culture/Others/no_visuel.gif";

                        DateTime dayDT = new DateTime(int.Parse(dt.Rows[i]["date_media_num"].ToString().Substring(0, 4)), int.Parse(dt.Rows[i]["date_media_num"].ToString().Substring(4, 2)), int.Parse(dt.Rows[i]["date_media_num"].ToString().Substring(6, 2)));

                        if (dt.Rows[i]["visual"] != System.DBNull.Value )
                        {

                            if (_resultType == FrameWorkResultConstantes.Portofolio.SYNTHESIS)
                            {

                                coverLinkItemSynthesis = new CoverLinkItemSynthesis(dt.Rows[i]["media"].ToString(), dt.Rows[i]["number_page_media"].ToString(), _webSession.IdSession, _idMedia, dt.Rows[i]["date_media_num"].ToString(), dt.Rows[i]["date_media_num"].ToString());
                                coverItem = new CoverItem(i + 1, GestionWeb.GetWebWord(1409, _webSession.SiteLanguage), pathWeb, coverLinkItemSynthesis);
                            }
                            else if (_resultType == FrameWorkResultConstantes.Portofolio.DETAIL_MEDIA)
                            {
                                coverLinkItem = new CoverLinkItem(_webSession.IdSession, _idMedia, dt.Rows[i]["date_media_num"].ToString(), "");
                                coverItem = new CoverItem(i + 1, "", pathWeb, coverLinkItem);
                            }
                        }
                        else
                            if (_resultType == FrameWorkResultConstantes.Portofolio.SYNTHESIS)
                                coverItem = new CoverItem(i + 1, GestionWeb.GetWebWord(1409, _webSession.SiteLanguage), pathWeb, null);
                            else if (_resultType == FrameWorkResultConstantes.Portofolio.DETAIL_MEDIA)
                                coverItem = new CoverItem(i + 1, "", pathWeb, null);


                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Rows[i]["RUBLES"] != System.DBNull.Value)
                            {
                                vehicleItem = new Russia.VehicleView.VehicleItem(dayDT, dt.Rows[i]["insertions"].ToString(), int.Parse(dt.Rows[i]["RUBLES"].ToString()).ToString("### ### ### ###"), int.Parse(dt.Rows[i]["USD"].ToString()).ToString("### ### ### ###"), _webSession.SiteLanguage, coverItem);
                            }
                            else
                            {
                                vehicleItem = new Russia.VehicleView.VehicleItem(dayDT, "0", "0", "0",_webSession.SiteLanguage, coverItem);

                            }
                        }

                        itemsCollection.Add(vehicleItem);

                    }


                }

            }

            return itemsCollection;
        }
        #endregion
        
        #endregion

        #region Methods

        #region GetHourIntevalList
        /// <summary>
		/// Get hour interval list
		/// </summary>
		/// <param name="hourBeginningList">hour begininng list</param>
		/// <param name="hourEndList">hour end list</param>
		protected override void GetHourIntevalList(Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList) {
			if (hourBeginningList == null) hourBeginningList = new Dictionary<string, double>();
			if (hourEndList == null) hourEndList = new Dictionary<string, double>();

			switch (_vehicleInformation.Id) {				
				case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
					hourBeginningList.Add("0609", 60000); hourEndList.Add("0609", 90000);
					hourBeginningList.Add("0912", 90000); hourEndList.Add("0912", 120000);
					hourBeginningList.Add("1215", 120000); hourEndList.Add("1215", 150000);
                    hourBeginningList.Add("1518", 150000); hourEndList.Add("1518", 180000);
                    hourBeginningList.Add("1821", 180000); hourEndList.Add("1821", 210000);
                    hourBeginningList.Add("2124", 210000); hourEndList.Add("2124", 240000);
                    hourBeginningList.Add("2406", 240000); hourEndList.Add("2406", 60000);
					break;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
				default:
                    base.GetHourIntevalList(hourBeginningList, hourEndList);
                    break;
			}
        }
        #endregion

        /// <summary>
        /// Get ventilation type list
        /// </summary>
        /// <returns>ventilation type list</returns>
        protected override List<FrameWorkResultConstantes.PortofolioStructure.Ventilation> GetVentilationTypeList()
        {
            List<FrameWorkResultConstantes.PortofolioStructure.Ventilation> ventilationTypeList = new List<TNS.AdExpress.Constantes.FrameWork.Results.PortofolioStructure.Ventilation>();
            ventilationTypeList.Add(FrameWorkResultConstantes.PortofolioStructure.Ventilation.format);
            ventilationTypeList.Add(FrameWorkResultConstantes.PortofolioStructure.Ventilation.design);
            ventilationTypeList.Add(FrameWorkResultConstantes.PortofolioStructure.Ventilation.location);
            ventilationTypeList.Add(FrameWorkResultConstantes.PortofolioStructure.Ventilation.nonStandardPlacement);
            return ventilationTypeList;
        }
        
        #endregion


    }
}
