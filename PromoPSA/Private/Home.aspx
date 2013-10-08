<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Private_Home" %>
<%@ Register TagPrefix="cc1" Namespace="KMI.PromoPSA.Web.Controls.Header" Assembly="KMI.PromoPSA.Web.Controls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>PSA</title>
    <meta http-equiv="Content-Type" content="text/html;"/>
	<meta content="C#" name="CODE_LANGUAGE"/>
	<meta content="JavaScript" name="vs_defaultClientScript"/>
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
	<meta http-equiv="expires" content="Wed, 23 Feb 1999 10:49:02 GMT"/>
	<meta http-equiv="expires" content="0"/>
	<meta http-equiv="pragma" content="no-cache"/>
	<meta content="no-cache" name="Cache-control"/>
    
    <script src="/js/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="/js/jquery-ui.min.js" type="text/javascript"></script>
    <script src="/js/i18n/grid.locale-fr.js" type="text/javascript"></script>
    <script src="/js/jquery.jqGrid.min.js" type="text/javascript"></script>
        <!--[if lt IE 9]><script language="javascript" type="text/javascript" src="/js/excanvas.js"></script><![endif]-->
    <script src="/js/jquery.jqplot.min.js" type="text/javascript" ></script>
    <script src="/js/jqplot.pieRenderer.js" type="text/javascript" ></script>
    <script src="/js/jqplot.json2.min.js" type="text/javascript" ></script>
    <script src="/js/json2.js" type="text/javascript" ></script>

</head>
<body class="bodyStyle">
    <form id="form1" runat="server">
        <div class="header" >
            <div style="float:left;">
                <asp:Image ID="Image1" runat="server" SkinID="logo_tns_home" />
            </div>
        </div>
        <div class="promotionsInformationDiv">
            <table cellspacing="0" cellpadding="0" border="0" height="100%">
                <tr>
                    <td><cc1:DisconnectUserWebControl runat="server" id="DisconnectUserWebControl1" SkinID="DisconnectUserWebControl"/></td>
                    <td><cc1:LoginInformationWebControl runat="server" id="LoginInformationWebControl1" SkinID="LoginInformationWebControl"/></td>
                    <td><cc1:PromotionInformationWebControl runat="server" id="PromotionInformationWebControl1" SkinID="PromotionInformationWebControl"/></td>
                </tr>
            </table>
        </div>
        <div id="chart1" style="height:220px; width:460px; margin-left: 450px;"></div>
        <table style="margin-left:350px; margin-bottom: 20px;">
            <tr>
                <td>
                    <a href="#" class="validateMonth" onclick="javascript:VerifDialog();">Valider Mois</a>
                </td>
                <td class="buttonSpace">
                    <a href="#" class="startCodification" onclick="javascript:StartCodif();">Commencer Codification</a>
                </td>
            </tr>
        </table>
        <div style=" margin-left:250px;">
            <table id="grid" style="margin-right:auto; margin-left:auto;"></table>
            <div id="pager" style="height:24px;"></div>

        </div>
        <div id="dialog" title="Alert" style="font-size:12px;">
            <p>Cette fiche est déjà en cours de codification !</p>
        </div>
            
    <script type="text/javascript">
        $(document).ready(function () {
            
            InitChartComponent(currentMonth);
            InitGridComponent(currentMonth);
            
        });

        $(function () {
            $("#dialog").dialog({
                autoOpen: false,
                dialogClass: "alert",
                modal:true,
                show: {
                    effect: "highlight",
                    duration: 1000
                },
                hide: {
                    effect: "fade",
                    duration: 1000
                }
            });
        });
        
        function StartCodif() {

            var promotionIdFromWS;

            $.ajax({
                type: "POST",
                url: 'Home.aspx/getAvailablePromotionId ',
                async: false,
                data: JSON.stringify({ loginId: loginId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg, st) {
                    if (st == "success") {
                        promotionIdFromWS = JSON.parse(msg.d);
                    }
                },
                error: function () {
                    alert("Error with AJAX callback");
                }
            });

            if (promotionIdFromWS > 0)
                document.location = "Edit.aspx?promotionId=" + promotionIdFromWS + "&sessionId=" + sessionId + "&loginId=" + loginId;
            else
                alert("Aucune fiche n'est disponible !");

        }

        function VerifDialog() {

            var r = confirm("Valider Mois");

            if (r == true) {
                ValidateMonth();
            }
            else {
                return;
            }
        }

        function ValidateMonth() {

            var verif;

            $.ajax({
                type: "POST",
                url: 'Home.aspx/validateMonth',
                async: false,
                data: JSON.stringify({ month: selectedMonth }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg, st) {
                    if (st == "success") {
                        verif = JSON.parse(msg.d);
                    }
                },
                error: function () {
                    alert("Error with AJAX callback");
                }
            });

            if (verif == true)
                alert("Mois Validé");

        }
        
        function EditRequest(promotionId) {

            var verif;

            $.ajax({
                type: "POST",
                url: 'Home.aspx/checkPromotionIdAvailability',
                async: false,
                data: JSON.stringify({ loginId: loginId, promotionId: promotionId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg, st) {
                    if (st == "success") {
                        verif = JSON.parse(msg.d);
                    }
                },
                error: function () {
                    alert("Error with AJAX callback");
                }
            });

            if (verif == true)
                document.location = "Edit.aspx?promotionId=" + promotionId + "&sessionId=" + sessionId + "&loginId=" + loginId;
            else
                $("#dialog").dialog("open");

        }

        function linkFormat(cellvalue, options, rowObject) {
            return "<a href=\"#\" onclick=\"javascript:EditRequest('"+cellvalue+"');\">Edit</a>";
        }

        function InitGridComponent(loadDate) {

            var grid = $("#grid");
            var vehicleStr = { '0': '', '1': 'Presse', '3': 'Tv', '7': 'Internet', '8': 'Publicité Extérieur' };
            var activationStr = { '0': '', '40': 'A Codifier', '30': 'Rejetée', '20': 'Codifiée', '90': 'Litige' };
            var loadDateStr = new Object();
            var loadDateList = new Array();

            $.ajax({
                url: 'Home.aspx/getLoadDates',
                type: "POST",
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, st) {
                    if (st == "success") {
                        loadDateList = JSON.parse(data.d);
                        for (var i = 0; i < loadDateList.length; i++)
                            loadDateStr['' + loadDateList[i] + ''] = loadDateList[i];
                    }
                },
                error: function () {
                    alert("Error with AJAX callback");
                }
            });



            $("#grid").jqGrid({
                // setup custom parameter names to pass to server
                prmNames: {
                    search: "isSearch",
                    nd: null,
                    rows: "numRows",
                    page: "page",
                    sort: "sortField",
                    order: "sortOrder"
                },
                // add by default to avoid webmethod parameter conflicts
                postData: { searchString: '', searchField: '', searchOper: '', loadingDate: loadDate, sessionId: sessionId, loginId: loginId },
                // setup ajax call to webmethod
                datatype: function (postdata) {
                    $(".loading").show(); // make sure we can see loader text
                    $.ajax({
                        url: 'Home.aspx/getGridData',
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify(postdata),
                        dataType: "json",
                        success: function (data, st) {
                            if (st == "success") {
                                var grid = $("#grid")[0];
                                grid.addJSONData(JSON.parse(data.d));
                                var myPostData = $('#grid').jqGrid("getGridParam", "postData");
                                if (myPostData.searchField == "LoadDate") {
                                    var loadDateFormatting = myPostData.searchString.substring(3, 7) + myPostData.searchString.substring(0, 2);
                                    selectedMonth = loadDateFormatting;
                                    InitPromotionNb(loadDateFormatting);
                                    InitChartComponent(loadDateFormatting);
                                }
                            }
                        },
                        error: function () {
                            alert("Error with AJAX callback");
                        }
                    });
                },
                // this is what jqGrid is looking for in json callback
                jsonReader: {
                    root: "rows",
                    page: "page",
                    total: "totalpages",
                    records: "totalrecords",
                    cell: "cell",
                    id: "id", //index of the column with the PK in it 
                    userdata: "userdata",
                    repeatitems: true
                },
                colNames: ['Numero de fiche', 'Media', 'Date de parution', 'Edit', 'Activation', 'Date de chargement'],
                colModel: [
                    { name: 'IdForm', index: 'IdForm', search: false },
                    { name: 'VehicleName', index: 'VehicleName', stype: 'select', searchoptions: { sopt: ['eq', 'ne'], value: vehicleStr, defaultValue: '1' } },
                    { name: 'DateMediaNum', index: 'DateMediaNum', search: false },
                    { name: 'Link', index: 'Link', formatter: linkFormat, search: false, sortable: false },
                    { name: 'ActivationName', index: 'ActivationName', hidden: false, stype: 'select', searchoptions: { sopt: ['eq', 'ne'], value: activationStr, defaultValue: '1' } },
                    { name: 'LoadDate', index: 'LoadDate', hidden: false, stype: 'select', searchoptions: { sopt: ['eq', 'ne'], value: loadDateStr, defaultValue: '1' }, sortable: false }
                ],
                rowNum: 20,
                rowList: [10, 20, 30],
                pager: jQuery("#pager"),
                toppager:true,
                sortname: "IdForm",
                sortorder: "asc",
                viewrecords: true,
                width: 900,
                height: "100%",
                //height: 500,
                gridview: true,
                rowattr: function (rd) {
                    if (rd.ActivationName == "A Codifier") { // verify that the testing is correct in your case
                        return { "class": "toCodifyStyle" };
                    }
                    else if (rd.ActivationName == "Codifiée") { // verify that the testing is correct in your case
                        return { "class": "codifiedStyle" };
                    }
                    else if (rd.ActivationName == "Rejetée") { // verify that the testing is correct in your case
                        return { "class": "rejectedStyle" };
                    }
                    else if (rd.ActivationName == "Litige") { // verify that the testing is correct in your case
                        return { "class": "pendingStyle" };
                    }
                },
                caption: "Promotions PSA",
                gridComplete: function () {
                    $(".loading").hide();
                }
            }).jqGrid('navGrid', '#grid_toppager', {
                edit: false, add: false, del: false,
                beforeRefresh: function () {
                    //alert('Here');
                    var myPostData = $('#grid').jqGrid("getGridParam", "postData");
                    if (myPostData.searchField == "LoadDate") {
                        myPostData.searchString = currentMonth.substring(4, 6) + "/" + currentMonth.substring(0, 4);
                    }
                    //jQuery(table).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');
                }
            },
            {}, // default settings for edit
            {}, // add
            {}, // delete
            { closeOnEscape: true, closeAfterSearch: true }, //search
            {}
        )
            //$("#grid").jqGrid('navGrid', '#grid_toppager');
        }

        function InitChartComponent(loadDate) {

            var dataG;

            $.ajax({
                url: 'Home.aspx/getChartData',
                type: "POST",
                async: false,
                data: JSON.stringify({ loadingDate: loadDate }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, st) {
                    if (st == "success") {
                        dataG = JSON.parse(data.d);
                    }
                },
                error: function () {
                    alert("Error with AJAX callback");
                }
            });

            plot2 = jQuery.jqplot('chart1', dataG, {
                title: ' ',
                seriesColors: ['#e8e8e8', '#94d472', '#fed2d2', '#f0c95b'],
                grid: {
                    background: '#ffffff'     // CSS color spec for background color of grid.
                },
                seriesDefaults: {
                    shadow: false,
                    renderer: jQuery.jqplot.PieRenderer,
                    rendererOptions: {
                        startAngle: 180,
                        sliceMargin: 4,
                        showDataLabels: true
                    }
                },
                legend: {
                    show: true,
                    location: 'e'
                }
            });

        }

        function InitPromotionNb(loadDate) {
            
            var promotionNb;

            $.ajax({
                type: "POST",
                url: 'Home.aspx/getPromotionsNb',
                async: false,
                data: JSON.stringify({ loadingDate: loadDate }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg, st) {
                    if (st == "success") {
                        promotionNb = JSON.parse(msg.d);
                    }
                },
                error: function () {
                    alert("Error with AJAX callback");
                }
            });

            var oN = document.getElementById("promotionsNb");
            oN.innerHTML = "<strong>" + promotionNb + "</strong>";
        }

    </script>

        </form>
</body>
</html>


