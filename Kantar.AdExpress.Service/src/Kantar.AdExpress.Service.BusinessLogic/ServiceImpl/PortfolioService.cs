#define Debug
using System;
using Kantar.AdExpress.Service.Core.BusinessService;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.Portofolio;
using System.Reflection;
using System.Collections.Generic;
using Kantar.AdExpress.Service.Core.Domain.Creative;
using TNS.AdExpress.Constantes.DB;
using FrameWorkResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Classification;
using CustomerConstantes = TNS.AdExpress.Constantes.Customer;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Web;
using System.Globalization;
using TNS.FrameWork.Date;
using System.IO;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Portofolio.VehicleView;
using NLog;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Utilities.Exceptions;
using System.Web;
using TNS.AdExpress.Domain.Level;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class PortfolioService : IPortfolioService
    {
        private WebSession _customerSession = null;
        protected VehicleInformation _vehicleInformation;
        private static Logger Logger= LogManager.GetCurrentClassLogger();
        protected string _subFolder { get; set; }

        public GridResult GetGridResult(string idWebSession, HttpContextBase httpContext)
        {
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            IPortofolioResults portofolioResult = null;
            GridResult gridResult = new GridResult();
            try
            {
                string sortKey = httpContext.Request.Cookies["sortKey"].Value;
                string sortOrder = httpContext.Request.Cookies["sortOrder"].Value;

                if (!string.IsNullOrEmpty(sortKey) && !string.IsNullOrEmpty(sortOrder))
                {
                    _customerSession.SortKey = sortKey;
                    _customerSession.Sorting = (ResultTable.SortOrder)Enum.Parse(typeof(ResultTable.SortOrder), sortOrder);
                    _customerSession.Save();
                }

                var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
                var parameters = new object[1];
                parameters[0] = _customerSession;
                portofolioResult = (IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                    + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                    | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
                switch (_customerSession.CurrentTab)
                {

                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:
                        gridResult = portofolioResult.GetDetailMediaGridResult(false);
                        break;
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
                        gridResult = portofolioResult.GetStructureGridResult(false);
                        break;
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.PROGRAM_TYPOLOGY_BREAKDOWN:
                        gridResult = portofolioResult.GetBreakdownGridResult(false, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.programTypology));
                        break;
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.PROGRAM_BREAKDOWN:
                        gridResult = portofolioResult.GetBreakdownGridResult(false, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.program));
                        break;
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SUBTYPE_SPOTS_BREAKDOWN:
                        gridResult = portofolioResult.GetBreakdownGridResult(false, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.spotSubType));
                        break;
                    default:
                        gridResult = portofolioResult.GetGridResult();
                        break;
                }
            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return gridResult;
        }

        public List<GridResult> GetGraphGridResult(string idWebSession, HttpContextBase httpContext)
        {
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            List<GridResult> result = new List<GridResult>();
            try
            {
                TNS.AdExpress.Domain.Web.Navigation.Module module = _customerSession.CustomerLogin.GetModule(_customerSession.CurrentModule);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
                object[] parameters = new object[1];
                parameters[0] = _customerSession;
                IPortofolioResults portofolioResult = (IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
                result = portofolioResult.GetGraphGridResult();
            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return result;
        }


        public ResultTable GetResultTable(string idWebSession, HttpContextBase httpContext)
        {
            ResultTable data = null;
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
                var parameters = new object[1];
                parameters[0] = _customerSession;
                var portofolioResult = (IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                    + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                    | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
                data = portofolioResult.GetResultTable();
            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return data;
        }

        public List<VehicleCover> GetVehicleCovers(string idWebSession, int resultType, HttpContextBase httpContext)
        {
            List<VehicleCover> vehicleCovers = new List<VehicleCover>();
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                _vehicleInformation = GetVehicleInformation();

                if (ShowVehicleItems(resultType) && HasCovers(_vehicleInformation.Id))
                {
                    vehicleCovers = new List<VehicleCover>();
                    TNS.AdExpress.Domain.Web.Navigation.Module module = _customerSession.CustomerLogin.GetModule(_customerSession.CurrentModule);
                    object[] parameters = new object[2];
                    parameters[0] = _customerSession;
                    parameters[1] = resultType;
                    var portofolioResult = (IPortofolioResults)AppDomain.CurrentDomain
                         .CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}", AppDomain.CurrentDomain.BaseDirectory
                         , module.CountryRulesLayer.AssemblyName), module.CountryRulesLayer.Class, false,
                         BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);

                    var itemsCollection = portofolioResult.GetVehicleItems();

                    var cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_customerSession.SiteLanguage].Localization);
                    string day = string.Empty;
                    itemsCollection.ForEach(p =>
                    {
                            //Set vehicle cover
                            day = string.Format("{0} {1}", DayString.GetCharacters(p.ParutionDate, cultureInfo),
                      DateString.dateTimeToDD_MM_YYYY(p.ParutionDate, _customerSession.SiteLanguage));
                        long mediaId = (p.CoverItem != null && p.CoverItem.CoverLinkItem != null) ? p.CoverItem.CoverLinkItem.MediaId : 0;
                        string src = (p.CoverItem != null && !string.IsNullOrEmpty(p.CoverItem.Src)) ? p.CoverItem.Src : string.Empty;
                        string nbPage = string.Empty;
                        string media = string.Empty;
                        string coverDate = string.Empty;
                        if (p.CoverItem != null && p.CoverItem.CoverLinkItem != null && p.CoverItem.CoverLinkItem is CoverLinkItemSynthesis)
                        {
                            CoverLinkItemSynthesis coverLinkItemSynthesis = p.CoverItem.CoverLinkItem as CoverLinkItemSynthesis;
                            nbPage = coverLinkItemSynthesis.NumberPageMedia;
                            media = coverLinkItemSynthesis.Media;
                            coverDate = coverLinkItemSynthesis.DateCoverNum;
                        }


                            //TODO : enlever le chemin en dur
                            var vehicleCover = new VehicleCover
                        {
                            DayN = p.ParutionDate.ToString("yyyyMMdd"),
                            ParutionDate = day,
                            CoverDate = coverDate,
                            Id = mediaId,
                            Invest = p.TotalInvestment,
                            NbInser = p.InsertionNumber,
                            Src = src,
                            Media = media,
                            NbPage = nbPage
                        };
                        vehicleCovers.Add(vehicleCover);

                    });
                }
            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return vehicleCovers;
        }


        public List<VehiclePage> GetVehiclePages(string idWebSession, string mediaId, string dateMediaNum, string dateCoverNum, string nbPage, string media, HttpContextBase httpContext, string subFolder = null)
        {
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            _subFolder = subFolder;
            List<VehiclePage> vehiclePages = new List<VehiclePage>();
            try
            {
                string pathWeb = string.Format("{0}/{1}/{2}/{3}",
                WebConstantes.CreationServerPathes.IMAGES, mediaId, dateCoverNum,
                (string.IsNullOrEmpty(_subFolder)) ? "imagette" : _subFolder);

                string pathWebZoom = string.Format("{0}/{1}/{2}",
                  WebConstantes.CreationServerPathes.IMAGES, mediaId, dateCoverNum);


                string path = string.Format("{0}{1}\\{2}\\{3}",
                    WebConstantes.CreationServerPathes.LOCAL_PATH_IMAGE, mediaId, dateCoverNum,
                    (string.IsNullOrEmpty(_subFolder)) ? "imagette" : _subFolder);

                string[] files = Directory.GetFiles(path, "*.jpg");
                Array.Sort(files);

                var cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_customerSession.SiteLanguage].Localization);
                var dayDT = new DateTime(int.Parse(dateMediaNum.Substring(0, 4)),
                    int.Parse(dateMediaNum.Substring(4, 2)), int.Parse(dateMediaNum.ToString().Substring(6, 2)));
                string day = string.Format("{0} {1}", DayString.GetCharacters(dayDT, cultureInfo),
                    Dates.DateToString(dayDT, _customerSession.SiteLanguage));
                if (files.Length > 0)
                {
                    foreach (string name in files)
                    {
                        //TODO : enlever le chemin en dur
                        var vehiclePage = new VehiclePage
                        {
                            NbPage = nbPage,
                            ParutionDate = day,
                            CoverDate = dateCoverNum,
                            Src = string.Format("{0}/{1}", pathWeb, Path.GetFileName(name)),
                            SrcZoom = string.Format("{0}/{1}", pathWebZoom, Path.GetFileName(name)),
                            Title = media

                        };
                        vehiclePages.Add(vehiclePage);
                    }
                }
            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return vehiclePages;
        }

        /// <summary>
        /// Check if can Show Vehicle Items
        /// </summary>
        /// <returns></returns>
        private bool ShowVehicleItems(int resultType)
        {
            if (TNS.AdExpress.Domain.AllowedFlags.ContainFlag(Flags.ID_PRESS_VEHICLE_PAGES_ACCESS_FLAG))
            {
                if (resultType == FrameWorkResultConstantes.Portofolio.DETAIL_MEDIA) return true;//Will be managed in The Rules
                return _customerSession.CustomerLogin.ShowVehiclePages(_vehicleInformation.Id);
            }
            return _customerSession.CustomerLogin.ShowCreatives(_vehicleInformation.Id);
        }

        #region Vehicle Selection
        /// <summary>
        /// Get Vehicle Selection
        /// </summary>
        /// <returns>Vehicle label</returns>
        private string GetVehicle()
        {
            string vehicleSelection = _customerSession.GetSelection(_customerSession.SelectionUniversMedia, CustomerConstantes.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new Exception("The media selection is invalid"));
            return (vehicleSelection);
        }
        /// <summary>
        /// Get vehicle selection
        /// </summary>
        /// <returns>Vehicle</returns>
        private VehicleInformation GetVehicleInformation()
        {
            try
            {
                return (VehiclesInformation.Get(Int64.Parse(GetVehicle())));
            }
            catch (System.Exception err)
            {
                throw (new Exception("Impossible to retreive vehicle selection", err));
            }
        }
        private bool HasCovers(DBClassificationConstantes.Vehicles.names id)
        {
            bool hasCovers = false;
            switch (id)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                    hasCovers = true;
                    break;
                default:
                    break;
            }
            return hasCovers;
        }


        #endregion

        public long CountDataRows(string idWebSession, HttpContextBase httpContext)
        {
            long nbRows = 0;
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                

                var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
                var parameters = new object[1];
                parameters[0] = _customerSession;
                var portofolioResult = (IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                    + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                    | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
                nbRows = portofolioResult.CountDataRows();
            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());
                throw;
            }
            return nbRows;
        }

    }
}
