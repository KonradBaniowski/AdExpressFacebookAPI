using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMI.PromoPSA.BusinessEntities;
using KMI.PromoPSA.Constantes;
using KMI.PromoPSA.Rules;
using KMI.PromoPSA.Web.UI;

public partial class Private_Home : PrivateWebPage {

    #region Page Load
    /// <summary>
    /// Page Load
    /// </summary>
    /// <param name="sender">Object Sender</param>
    /// <param name="e">Event Args</param>
    protected void Page_Load(object sender, EventArgs e) {
        DisconnectUserWebControl1.WebSession = _webSession;
        LoginInformationWebControl1.WebSession = _webSession;
        PromotionInformationWebControl1.WebSession = _webSession;
        IResults results = new Results();
        List<LoadDateBE> list = results.GetLoadDates();
        var loadDate = list.Max(x => x.LoadDate);
        string scriptGlobalVariables = "var currentMonth = '" + loadDate.Value + "'; \n var sessionId = '" + _webSession.IdSession + "';" + "\n var loginId = '" + _webSession.CustomerLogin.IdLogin + "';";
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "globaVariables", scriptGlobalVariables, true);
    }
    #endregion

    #region Get Chart Data
    /// <summary>
    /// Get Chart Data
    /// </summary>
    /// <returns>Chart Data</returns>
    [WebMethod]
    public static string getChartData(string loadingDate) {

        string result = null;
        IResults results = new Results();
        int advertsNbToCodify = results.GetNbAdverts(Int64.Parse(loadingDate), Constantes.ACTIVATION_CODE_TO_CODIFY);
        int advertsNbCodified = results.GetNbAdverts(Int64.Parse(loadingDate), Constantes.ACTIVATION_CODE_CODIFIED);
        int advertsNbRejected = results.GetNbAdverts(Int64.Parse(loadingDate), Constantes.ACTIVATION_CODE_REJECTED);

        //--- format json
        var jsonData = new[] {new[]{
                                    new object[] {"Nombre de promotions à codifier : " + advertsNbToCodify + "", advertsNbToCodify },
                                    new object[] {"Nombre de promotions codifiées : " + advertsNbCodified + "", advertsNbCodified },
                                    new object[] {"Nombre de promotions rejetées : " + advertsNbRejected + "", advertsNbRejected }
                                    }
            }.ToArray();

        result = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);

        return result;

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
    public static string getGridData(int? numRows, int? page, string sortField, string sortOrder, bool isSearch, string searchField, string searchString, string searchOper, string loadingDate, string sessionId, string loginId) {
        string result = null;

        IResults results = new Results();
        IEnumerable<Advert> list = results.GetAdverts(Int64.Parse(loadingDate));

        try {

            if (isSearch) {

                switch (searchField) {
                    case "IdForm":
                        switch (searchOper) { 
                            case "eq" :
                                list = list.Where(x => x.IdForm == Int64.Parse(searchString));
                                break;
                            case "ne":
                                list = list.Where(x => x.IdForm != Int64.Parse(searchString));
                                break;
                        }
                        break;
                    case "VehicleName":
                        switch (searchOper) {
                            case "eq":
                                list = list.Where(x => x.IdVehicle == Int64.Parse(searchString));
                                break;
                            case "ne":
                                list = list.Where(x => x.IdVehicle != Int64.Parse(searchString));
                                break;
                        }
                        break;
                    case "DateMediaNum":
                        switch (searchOper) {
                            case "eq":
                                list = list.Where(x => x.DateMediaNumFormated == searchString);
                                break;
                            case "ne":
                                list = list.Where(x => x.DateMediaNumFormated != searchString);
                                break;
                        }
                        break;
                    case "ActivationName":
                        switch (searchOper) {
                            case "eq":
                                list = list.Where(x => x.Activation == Int64.Parse(searchString));
                                break;
                            case "ne":
                                list = list.Where(x => x.Activation != Int64.Parse(searchString));
                                break;
                        }
                        break;
                    case "LoadDate":
                        switch (searchOper) {
                            case "eq":
                                list = list.Where(x => x.LoadDateFormated == searchString);
                                break;
                            case "ne":
                                list = list.Where(x => x.LoadDateFormated != searchString);
                                break;
                        }
                        break;
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
                        row.IdForm.ToString(), row.VehicleName, row.DateMediaNumFormated, ("Edit.aspx?formId=" +  row.IdForm.ToString() + "&sessionId=" + sessionId+ "&loginId=" + loginId), row.ActivationName, row.LoadDateFormated
                    }
                    }
               ).ToArray()
            };

            result = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);

        }
        catch (Exception ex) {
            //Debug.WriteLine(ex);
        }
        finally {
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

        string result = null;
        IResults results = new Results();
        List<LoadDateBE> loadDates = results.GetLoadDates();

        //--- format json
        List<Object> list = new List<Object>();
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
    #endregion

    #region Get Promotions Number
    /// <summary>
    /// Get Promotions Number
    /// </summary>
    /// <returns>Promotions Number</returns>
    [WebMethod]
    public static string getPromotionsNb(string loadingDate) {

        string result = null;
        IResults results = new Results();
        var list = results.GetAdverts(Int64.Parse(loadingDate));
        int promoNumber = list.Count;

        var jsonData = promoNumber;

        result = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);

        return result;

    }
    #endregion

}