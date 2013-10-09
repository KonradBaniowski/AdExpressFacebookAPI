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
using KMI.PromoPSA.Web.Functions;
using KMI.PromoPSA.Web.UI;

public partial class Private_Home : PrivateWebPage
{

    #region Page Load
    /// <summary>
    /// Page Load
    /// </summary>
    /// <param name="sender">Object Sender</param>
    /// <param name="e">Event Args</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        DisconnectUserWebControl1.WebSession = _webSession;
        LoginInformationWebControl1.WebSession = _webSession;
        PromotionInformationWebControl1.WebSession = _webSession;
        IResults results = new Results();
        List<LoadDateBE> list = results.GetLoadDates();
        var loadDate = list.Max(x => x.LoadDate);
        string scriptGlobalVariables = string.Format("var currentMonth = '{0}'; \n var sessionId = '{1}';" + "\n var loginId = '{2}';" + "\n var selectedMonth = '{0}';"
            , loadDate.Value, _webSession.IdSession, _webSession.CustomerLogin.IdLogin);
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "globaVariables", scriptGlobalVariables, true);
    }
    #endregion

    #region Get Available Id Form
    /// <summary>
    /// Get Available promotion Id
    /// </summary>
    /// <param name="loginId">Login Id</param>
    /// <returns>True if succed</returns>
    [WebMethod]
    public static long getAvailablePromotionId(string loginId)
    {
        try
        {
        
            IResults results = new Results();
            return results.GetAvailablePromotionId(Int64.Parse(loginId));
        }
        catch (Exception e)
        {
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
    public static bool ValidateMonth(string month)
    {

        IResults results = new Results();
        return results.ValidateMonth(Int64.Parse(month));

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
    public static bool checkPromotionIdAvailability(string loginId, string promotionId)
    {

        try
        {
            IResults results = new Results();
            return results.LockAdvertStatus(Int64.Parse(loginId), Int64.Parse(promotionId));

          
        }
        catch (Exception e)
        {
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
                                    new object[] {"Nombre de promotions validée : " + advertsNbValidated + "", advertsNbValidated }
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
        string searchField, string searchString, string searchOper, string loadingDate, string sessionId, string loginId)
    {
        string result = null;

        IResults results = new Results();
        IEnumerable<Advert> list;
        if (isSearch && !string.IsNullOrEmpty(searchField)
            && !string.IsNullOrEmpty(searchString) && searchField.Equals("LoadDate"))
        {
            string strDate = searchString.Substring(3, 4) + searchString.Substring(0, 2);
            list = results.GetAdverts(Int64.Parse(strDate));
        }
        else
        {
            list = results.GetAdverts(Int64.Parse(loadingDate));
        }


        try
        {

            if (isSearch)
            {

                switch (searchField)
                {
                    case "IdForm":
                        switch (searchOper)
                        {
                            case "eq":
                                list = list.Where(x => x.IdForm == Int64.Parse(searchString));
                                break;
                            case "ne":
                                list = list.Where(x => x.IdForm != Int64.Parse(searchString));
                                break;
                        }
                        break;
                    case "VehicleName":
                        switch (searchOper)
                        {
                            case "eq":
                                list = list.Where(x => x.IdVehicle == Int64.Parse(searchString));
                                break;
                            case "ne":
                                list = list.Where(x => x.IdVehicle != Int64.Parse(searchString));
                                break;
                        }
                        break;
                    case "DateMediaNum":
                        switch (searchOper)
                        {
                            case "eq":
                                list = list.Where(x => x.DateMediaNumFormated == searchString);
                                break;
                            case "ne":
                                list = list.Where(x => x.DateMediaNumFormated != searchString);
                                break;
                        }
                        break;
                    case "ActivationName":
                        switch (searchOper)
                        {
                            case "eq":
                                list = list.Where(x => x.Activation == Int64.Parse(searchString));
                                break;
                            case "ne":
                                list = list.Where(x => x.Activation != Int64.Parse(searchString));
                                break;
                        }
                        break;
                    case "LoadDate":
                        switch (searchOper)
                        {
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

            switch (sortField)
            {
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
            var jsonData = new
            {
                totalpages = totalPages, //--- number of pages
                page = pageIndex, //--- current page
                totalrecords = totalRecords, //--- total items
                rows = (
                    from row in sortedRecords
                    select new
                    {
                        i = row.IdForm,
                        cell = new string[] {
                            row.IdForm.ToString(), row.VehicleName, row.DateMediaNumFormated,
                            row.IdDataPromotion.ToString(), row.ActivationName, row.LoadDateFormated
                    }
                    }
               ).ToArray()
            };

            result = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);

        }
        catch (Exception ex)
        {
            //Debug.WriteLine(ex);
        }
        finally
        {
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
    public static string getLoadDates()
    {

        string result = null;
        IResults results = new Results();
        List<LoadDateBE> loadDates = results.GetLoadDates();

        //--- format json
        var list = new List<Object>();
        int i = 0;

        foreach (LoadDateBE loadDate in loadDates)
        {
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
    public static string getPromotionsNb(string loadingDate)
    {

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