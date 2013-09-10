using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMI.PromoPSA.BusinessEntities;
using KMI.PromoPSA.Rules;
using KMI.PromoPSA.Web.UI;

public partial class Private_Home : PrivateWebPage {
    /// <summary>
    /// Page Load
    /// </summary>
    /// <param name="sender">Object Sender</param>
    /// <param name="e">Event Args</param>
    protected void Page_Load(object sender, EventArgs e) {
        DisconnectUserWebControl1.WebSession = _webSession;
        LoginInformationWebControl1.WebSession = _webSession;
        PromotionInformationWebControl1.WebSession = _webSession;

        /*IResults results = new Results();
        LinqAtRuntimeGrid.DataSource = results.GetAdverts(201309); 
        LinqAtRuntimeGrid.DataBind();*/
    }


    [WebMethod]
    public static string getGridData(int? numRows, int? page, string sortField, string sortOrder, bool isSearch, string searchField, string searchString, string searchOper) {
        string result = null;

        IResults results = new Results();
        var list = results.GetAdverts(201309);


        try {

            //if (isSearch) {
            //    searchOper = getOperator(searchOper); // need to associate correct operator to value sent from jqGrid
            //    string whereClause = String.Format("{0} {1} {2}", searchField, searchOper, "@" + searchField);

            //    //--- associate value to field parameter
            //    Dictionary<string, object> param = new Dictionary<string, object>();
            //    param.Add("@" + searchField, searchString);

            //    query = query.Where(whereClause, new object[1] { param });
            //}


            //--- setup calculations
            int pageIndex = page ?? 1; //--- current page
            int pageSize = numRows ?? 10; //--- number of rows to show per page
            int totalRecords = list.Count(); //--- number of total items from query
            int totalPages = (int)Math.Ceiling((decimal)totalRecords / (decimal)pageSize); //--- number of pages

            //--- filter dataset for paging and sorting
            IOrderedEnumerable<Advert> orderedRecords = null;

            switch (sortField) {
                case "IdForm": orderedRecords = list.OrderBy(x => x.IdForm); break;
                case "IdVehicle": orderedRecords = list.OrderBy(x => x.IdVehicle); break;
                case "DateMediaNum": orderedRecords = list.OrderBy(x => x.DateMediaNum); break;
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
                        row.IdForm.ToString(), row.IdVehicle.ToString(), row.DateMediaNum.ToString(), ("Edit.aspx?formId=" +  row.IdForm.ToString())
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


}