using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMI.PromoPSA.BusinessEntities;
using KMI.PromoPSA.BusinessEntities.Classification;
using KMI.PromoPSA.Constantes;
using KMI.PromoPSA.Rules;
using KMI.PromoPSA.Web.Core.Sessions;
using KMI.PromoPSA.Web.Functions;
using KMI.PromoPSA.Web.UI;
using Newtonsoft.Json.Linq;

public partial class Private_Home : PrivateWebPage {

    #region Page Load
    /// <summary>
    /// Page Load
    /// </summary>
    /// <param name="sender">Object Sender</param>
    /// <param name="e">Event Args</param>
    protected void Page_Load(object sender, EventArgs e) {

        try {
            DisconnectUserWebControl1.WebSession = _webSession;
            LoginInformationWebControl1.WebSession = _webSession;
            PromotionInformationWebControl1.WebSession = _webSession;
            IResults results = new Results();
            results.ReleaseUser(_webSession.CustomerLogin.IdLogin);
            List<LoadDateBE> list = results.GetLoadDates();
            var loadDate = list.Max(x => x.LoadDate);
            string scriptGlobalVariables;

            if (_webSession.SelectedDate.Length > 0 && _webSession.SelectedVehicle.Length > 0 && _webSession.SelectedActivation.Length > 0)
                scriptGlobalVariables = string.Format("var currentMonth = '{0}'; \n var sessionId = '{1}';" + "\n var loginId = '{2}';" + "\n var selectedMonth = '{3}';" + "\n var selectedVehicle = '{4}';" + "\n var selectedActivation = '{5}';" + "\n var selectedSegment = '{6}';" + "\n var selectedProduct = '{7}';" + "\n var selectedBrand = '{8}';"
                    , loadDate.Value, _webSession.IdSession, _webSession.CustomerLogin.IdLogin, _webSession.SelectedDate.Substring(3, 4) + _webSession.SelectedDate.Substring(0, 2), _webSession.SelectedVehicle, _webSession.SelectedActivation, _webSession.SelectedSegment, _webSession.SelectedProduct, _webSession.SelectedBrand);
            else
                scriptGlobalVariables = string.Format("var currentMonth = '{0}'; \n var sessionId = '{1}';" + "\n var loginId = '{2}';" + "\n var selectedMonth = '{0}';" + "\n var selectedVehicle = '-1';" + "\n var selectedActivation = '-1';" + "\n var selectedSegment = '-1';" + "\n var selectedProduct = '-1';" + "\n var selectedBrand = '-1';"
                , loadDate.Value, _webSession.IdSession, _webSession.CustomerLogin.IdLogin);

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "globaVariables", scriptGlobalVariables, true);
        }
        catch (Exception ex) {
            string message = " Erreur lors de l'initialisation de la Home.<br/>";
            if (!string.IsNullOrEmpty(ex.Message)) message += string.Format("{0}<br/>", ex.Message);
            message += string.Format("Login Id : {0}", _webSession.CustomerLogin.IdLogin);
            Utils.SendErrorMail(message, ex);
            throw new Exception("Erreur lors de l'obtetion d'une fiche à codifier", ex);
        }

    }
    #endregion

    #region Release User
    /// <summary>
    /// Release User
    /// </summary>
    /// <param name="loginId">Login Id</param>
    /// <returns>True if succed</returns>
    [WebMethod]
    public static string releaseUser(string loginId) {

        try {
            IResults results = new Results();
            results.ReleaseUser(Int64.Parse(loginId));

            KMI.PromoPSA.Web.Core.Sessions.WebSession webSession = WebSessions.Get(Int64.Parse(loginId));

            if (webSession.SelectedVehicle.Length > 0 && webSession.SelectedActivation.Length > 0 && webSession.SelectedDate.Length > 0 && webSession.SelectedSegment.Length > 0 && webSession.SelectedProduct.Length > 0 && webSession.SelectedBrand.Length > 0)
                return webSession.SelectedVehicle + ";" + webSession.SelectedActivation + ";" + webSession.SelectedDate.Substring(3, 4) + webSession.SelectedDate.Substring(0, 2) + ";" + webSession.SelectedSegment + ";" + webSession.SelectedProduct + ";" + webSession.SelectedBrand;
            else
                return "";
        }
        catch (Exception e) {
            string message = " Erreur lors de la liberation de la fiche.<br/>";
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            message += string.Format("Login Id : {0}", loginId);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur lors de la liberation de la fiche", e);
        }

    }
    #endregion

    #region Get Available Id Form
    /// <summary>
    /// Get Available promotion Id
    /// </summary>
    /// <param name="loginId">Login Id</param>
    /// <returns>True if succed</returns>
    [WebMethod]
    public static long getAvailablePromotionId(string loginId) {
        
        try {
            IResults results = new Results();
            return results.GetAvailablePromotionId(Int64.Parse(loginId));
        }
        catch (Exception e) {
            string message = " Erreur lors de l'obtetion d'une fiche à codifier.<br/>";
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);          
            message += string.Format("Login Id : {0}", loginId);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur lors de l'obtetion d'une fiche à codifier", e);
        }

    }
    #endregion

    #region Validate Month
    /// <summary>
    /// Validate Month
    /// </summary>
    /// <param name="month"></param>
    /// <returns>True if succed</returns>
    [WebMethod]
    public static bool ValidateMonth(string month) {

        try {
            IResults results = new Results();
            return results.ValidateMonth(Int64.Parse(month));
        }
        catch (Exception e) {
            string message = " Erreur lors de la validation d'un mois.<br/>";
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            message += string.Format("Month : {0}", month);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur lors de l'obtetion d'une fiche à codifier", e);
        }

    }
    #endregion

    #region Check Form Id Availability
    /// <summary>
    /// Check Form Id Availability
    /// </summary>
    /// <param name="loginId">login Id</param>
    /// <param name="promotionId">Promotion Id</param>
    /// <returns>True if form available</returns>
    [WebMethod]
    public static bool checkPromotionIdAvailability(string loginId, string promotionId) {

        try {
            IResults results = new Results();
            return results.LockAdvertStatus(Int64.Parse(loginId), Int64.Parse(promotionId));
        }
        catch (Exception e) {
            string message = " Erreur lors de l'obtetion d'une fiche à codifier.<br/>";
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            message += string.Format("Promotion numero : {0}<br/>", promotionId);
            message += string.Format("Login Id : {0}", loginId);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur lors de l'obtetion d'une fiche à codifier", e);
        }
    }
    #endregion

    #region Get Chart Data
    /// <summary>
    /// Get Chart Data
    /// </summary>
    /// <returns>Chart Data</returns>
    [WebMethod]
    public static string getChartData(string loadingDate) {

        try {

            string result = null;
            IResults results = new Results();
            int advertsNbToCodify = results.GetNbAdverts(Int64.Parse(loadingDate), Constantes.ACTIVATION_CODE_TO_CODIFY);
            int advertsNbCodified = results.GetNbAdverts(Int64.Parse(loadingDate), Constantes.ACTIVATION_CODE_CODIFIED);
            int advertsNbRejected = results.GetNbAdverts(Int64.Parse(loadingDate), Constantes.ACTIVATION_CODE_REJECTED);
            int advertsNbPending = results.GetNbAdverts(Int64.Parse(loadingDate), Constantes.ACTIVATION_CODE_WAITING);
            int advertsNbValidated = results.GetNbAdverts(Int64.Parse(loadingDate), Constantes.ACTIVATION_CODE_VALIDATED);

            //--- format json
            var jsonData = new[] {new[]{
                                    new object[] {"Nombre de promotions à codifier : " + advertsNbToCodify + "", advertsNbToCodify },
                                    new object[] {"Nombre de promotions codifiées : " + advertsNbCodified + "", advertsNbCodified },
                                    new object[] {"Nombre de promotions rejetées : " + advertsNbRejected + "", advertsNbRejected },
                                    new object[] {"Nombre de promotions en litige : " + advertsNbPending + "", advertsNbPending },
                                    new object[] {"Nombre de promotions validées : " + advertsNbValidated + "", advertsNbValidated }
                                    }
            }.ToArray();

            result = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);

            return result;

        }
        catch (Exception e) {
            string message = " Erreur lors de la récupération des données pour le graphique.<br/>";
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            message += string.Format("Date de chargement : {0}", loadingDate);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur lors de la création du graphique", e);
        }
    }
    #endregion

    #region Get Grid Data
    /// <summary>
    /// Get Grid Data
    /// </summary>
    /// <param name="numRows">Rows number</param>
    /// <param name="page">Select Page</param>
    /// <param name="sortField">Sort Field</param>
    /// <param name="sortOrder">Sort Order</param>
    /// <param name="isSearch">Is Search</param>
    /// <param name="searchField">Search Field</param>
    /// <param name="searchString">Search String</param>
    /// <param name="searchOper">Search Operation</param>
    /// <returns>Grid Data</returns>
    [WebMethod]
    public static string getGridData(int? numRows, int? page, string sortField, string sortOrder, bool isSearch,
        string selectedDate, string selectedVehicle, string selectedActivation, string selectedSegment, string selectedProduct, string selectedBrand, string sessionId, string loginId, string filters) { //, string filters

        string result = null;
        Dictionary<string, string> searchFilters = new Dictionary<string, string>();

        try {

            if (filters.Length > 0) {
                dynamic t = Newtonsoft.Json.JsonConvert.DeserializeObject(filters);

                List<string> fields = new List<string>();
                KMI.PromoPSA.Web.Core.Sessions.WebSession webSession = WebSessions.Get(Int64.Parse(loginId));
                string value = string.Empty;

                foreach (var rule in t.rules) {
                    switch ((string)rule.field.Value) {
                        case "LoadDate":
                            webSession.SelectedDate = rule.data.Value;
                            value = rule.data.Value;
                            break;
                        case "VehicleName":
                            webSession.SelectedVehicle = rule.data.Value;
                            value = rule.data.Value.Split('-')[1];
                            break;
                        case "ActivationName":
                            webSession.SelectedActivation = rule.data.Value;
                            value = rule.data.Value.Split('-')[1];
                            break;
                        case "Segment":
                            webSession.SelectedSegment = rule.data.Value;
                            value = rule.data.Value.Split('-')[1];
                            break;
                        case "TypeDePiece":
                            webSession.SelectedProduct = rule.data.Value;
                            value = rule.data.Value.Split('-')[1];
                            break;
                        case "Enseigne":
                            webSession.SelectedBrand = rule.data.Value;
                            value = rule.data.Value.Split('-')[1];
                            break;
                    }
                    if (rule.data.Value != "-1")
                        searchFilters.Add(rule.field.Value, value);
                }
            }
            else {
                if (selectedDate.Length == 7)
                    searchFilters.Add("LoadDate", selectedDate);
                else
                    searchFilters.Add("LoadDate", selectedDate.Substring(4, 2) + "/" + selectedDate.Substring(0,4));

                if (selectedVehicle == "-1")
                    searchFilters.Add("VehicleName", selectedVehicle);
                else
                    searchFilters.Add("VehicleName", selectedVehicle.Split('-')[1]);

                if (selectedActivation == "-1")
                    searchFilters.Add("ActivationName", selectedActivation);
                else
                    searchFilters.Add("ActivationName", selectedActivation.Split('-')[1]);

                if (selectedSegment == "-1")
                    searchFilters.Add("Segment", selectedSegment);
                else
                    searchFilters.Add("Segment", selectedSegment.Split('-')[1]);

                if (selectedProduct == "-1")
                    searchFilters.Add("TypeDePiece", selectedProduct);
                else
                    searchFilters.Add("TypeDePiece", selectedProduct.Split('-')[1]);

                if (selectedBrand == "-1")
                    searchFilters.Add("Enseigne", selectedBrand);
                else
                    searchFilters.Add("Enseigne", selectedBrand.Split('-')[1]);
            }

            IResults results = new Results();
            IEnumerable<Advert> list;
            if ((isSearch || searchFilters.Count > 0) && searchFilters.Keys.Contains("LoadDate")) {
                string strDate = searchFilters["LoadDate"].Substring(3, 4) + searchFilters["LoadDate"].Substring(0, 2);
                list = results.GetAdvertsDetails(Int64.Parse(strDate));
            }
            else {
                list = results.GetAdvertsDetails(Int64.Parse(selectedDate));
            }

            if (isSearch || searchFilters.Count > 0) {

                foreach (string searchField in searchFilters.Keys) {

                    switch (searchField) {
                        case "IdForm":
                            list = list.Where(x => x.IdForm == Int64.Parse(searchFilters[searchField]));
                            break;
                        case "VehicleName":
                            if (Int64.Parse(searchFilters[searchField]) != -1)
                                list = list.Where(x => x.IdVehicle == Int64.Parse(searchFilters[searchField]));
                            break;
                        case "DateMediaNum":
                            list = list.Where(x => x.DateMediaNumFormated == searchFilters[searchField]);
                            break;
                        case "ActivationName":
                            if (Int64.Parse(searchFilters[searchField]) != -1)
                                list = list.Where(x => x.Activation == Int64.Parse(searchFilters[searchField]));
                            break;
                        case "LoadDate":
                            list = list.Where(x => x.LoadDateFormated == searchFilters[searchField]);
                            break;
                        case "Segment":
                            if (Int64.Parse(searchFilters[searchField]) != -1)
                                list = list.Where(x => x.IdSegment == Int64.Parse(searchFilters[searchField]));
                            break;
                        case "TypeDePiece":
                            if (Int64.Parse(searchFilters[searchField]) != -1)
                                list = list.Where(x => x.IdProduct == Int64.Parse(searchFilters[searchField]));
                            break;
                        case "Enseigne":
                            if (Int64.Parse(searchFilters[searchField]) != -1)
                                list = list.Where(x => x.IdBrand == Int64.Parse(searchFilters[searchField]));
                            break;
                    }

                }
            }

            //--- setup calculations
            int pageIndex = page ?? 1; //--- current page
            int pageSize = numRows ?? 10; //--- number of rows to show per page
            int totalRecords = list.Count(); //--- number of total items from query
            int totalPages = (int)Math.Ceiling((decimal)totalRecords / (decimal)pageSize); //--- number of pages

            //--- filter dataset for paging and sorting
            IOrderedEnumerable<Advert> orderedRecords = null;

            switch (sortField) {
                case "IdForm": orderedRecords = list.OrderBy(x => x.IdForm); break;
                case "VehicleName": orderedRecords = list.OrderBy(x => x.VehicleName); break;
                case "DateMediaNum": orderedRecords = list.OrderBy(x => x.DateMediaNum); break;
                case "ActivationName": orderedRecords = list.OrderBy(x => x.ActivationName); break;
                case "Segment": orderedRecords = list.OrderBy(x => x.Segment); break;
                case "TypeDePiece": orderedRecords = list.OrderBy(x => x.Product); break;
                case "Enseigne": orderedRecords = list.OrderBy(x => x.Brand); break;
                case "Marque": orderedRecords = list.OrderBy(x => x.PromotionBrand); break;
            }

            IEnumerable<Advert> sortedRecords = orderedRecords.ToList();
            if (sortOrder == "desc") sortedRecords = sortedRecords.Reverse();
            sortedRecords = sortedRecords
              .Skip((pageIndex - 1) * pageSize) //--- page the data
              .Take(pageSize);

            //--- format json
            var jsonData = new {
                totalpages = totalPages, //--- number of pages
                page = pageIndex, //--- current page
                totalrecords = totalRecords, //--- total items
                rows = (
                    from row in sortedRecords
                    select new {
                        i = row.IdForm,
                        cell = new string[] {
                            row.IdForm.ToString(), row.VehicleName, row.Segment, row.Product, row.Brand, row.PromotionBrand, row.DateMediaNumFormated,
                            row.IdDataPromotion.ToString(), row.ActivationName, row.LoadDateFormated
                    }
                    }
               ).ToArray()
            };

            result = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);

        }
        catch (Exception e) {
            string message = " Erreur lors de la récupération des données pour le tableau.<br/>";
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur lors de la création du tableau", e);
        }

        return result;
    }
    #endregion

    #region Get Load Dates
    /// <summary>
    /// Get Chart Data
    /// </summary>
    /// <returns>Chart Data</returns>
    [WebMethod]
    public static string getLoadDates() {
        try {
            string result = null;
            IResults results = new Results();
            List<LoadDateBE> loadDates = results.GetLoadDates();

            //--- format json
            var list = new List<Object>();
            int i = 0;

            foreach (LoadDateBE loadDate in loadDates) {
                string date = loadDate.LoadDate.ToString();
                list.Add(date.Substring(4, 2) + "/" + date.Substring(0, 4));
                i++;
            }

            var jsonData = list.ToArray();

            result = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);

            return result;
        }
        catch (Exception e) {
            string message = " Erreur lors de l'obtetion des dates de chargement.<br/>";
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur lors de l'obtetion des dates de chargement", e);
        }
    }
    #endregion

    #region Get Classification
    /// <summary>
    /// Get Classification
    /// </summary>
    /// <returns>Classification Data</returns>
    [WebMethod]
    public static string getClassification() {
        try {
            string result = null;
            IResults results = new Results();
            List<Segment> segments = results.GetSegments();
            List<Product> products = results.GetProducts();
            List<Brand> brands = results.GetBrands();

            //--- format json
            var list = new List<Object>();
            int listIndex = 0;
            bool first = true;

            foreach (Segment segment in segments) {
                if (first) {
                    list.Add(segment.Id + "_" + segment.Label);
                    first = false;
                }
                else
                    list[listIndex] += ";" + segment.Id + "_" + segment.Label;
            }

            first = true;
            listIndex++;

            foreach (Product product in products) {
                if (first) {
                    list.Add(product.Id + "_" + product.Label);
                    first = false;
                }
                else
                    list[listIndex] += ";" + product.Id + "_" + product.Label;
            }

            first = true;
            listIndex++;

            foreach (Brand brand in brands) {
                if (first) {
                    list.Add(brand.Id + "_" + brand.Label);
                    first = false;
                }
                else
                    list[listIndex] += ";" + brand.Id + "_" + brand.Label;
            }

            var jsonData = list.ToArray();

            result = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);

            return result;
        }
        catch (Exception e) {
            string message = " Erreur lors de l'obtetion des dates de chargement.<br/>";
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur lors de l'obtetion des dates de chargement", e);
        }
    }
    #endregion

    #region Get Promotions Number
    /// <summary>
    /// Get Promotions Number
    /// </summary>
    /// <returns>Promotions Number</returns>
    [WebMethod]
    public static string getPromotionsNb(string loadingDate) {

        try {
            string result = null;
            IResults results = new Results();
            var list = results.GetAdverts(Int64.Parse(loadingDate));
            int promoNumber = list.Count;

            var jsonData = promoNumber;

            result = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);

            return result;
        }
        catch (Exception e) {
            string message = " Erreur lors de l'obtetion du nombre de promotion.<br/>";
            if (!string.IsNullOrEmpty(e.Message)) message += string.Format("{0}<br/>", e.Message);
            message += string.Format("Date de chargement : {0}", loadingDate);
            Utils.SendErrorMail(message, e);
            throw new Exception("Erreur lors de l'obtetion du nombre de promotion", e);
        }
    }
    #endregion

}