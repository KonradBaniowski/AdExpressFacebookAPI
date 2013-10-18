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
    <script src="/js/vex.combined.min.js" type="text/javascript"></script>

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
        <div id="dialog" title="Alert" style="font-size:12px; display: none; padding-top: 14px; margin-bottom: -14px;">
            <p></p>
        </div>

        <div id="dialog-confirm" title="Alert" style="font-size:12px; display: none; padding-top: 14px; margin-bottom: -14px;">
            <p></p>
        </div>
            
    <script type="text/javascript">

        $(document).ready(function () {

            ReleaseUser();
            $('#gs_VehicleName').val(selectedVehicle);
            $('#gs_ActivationName').val(selectedActivation);
            $('#gs_LoadDate').val(selectedMonth);
            InitGridComponent(currentMonth);
            
        });

        function ReleaseUser() {
        
            $.ajax({
                type: "POST",
                url: 'Home.aspx/releaseUser',
                async: false,
                data: JSON.stringify({ loginId: loginId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg, st) {
                    var resultStr = msg.d;
                    if (resultStr.length > 0) {
                        var elements = resultStr.split(";");
                        selectedVehicle = elements[0];
                        selectedActivation = elements[1];
                        selectedMonth = elements[2];
                    }
                },
                error: function () {
                    $("#dialog").html("Erreur lors de la liberation de la fiche!");
                    $("#dialog").dialog("open");
                }
            });

        }

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
                    if (promotionIdFromWS > 0)
                        document.location = "Edit.aspx?promotionId=" + promotionIdFromWS + "&sessionId=" + sessionId + "&loginId=" + loginId;
                    else {
                        $("#dialog").html("Aucune fiche n'est disponible !");
                        $("#dialog").dialog("open");
                    }

                },
                error: function () {
                    $("#dialog").html("Erreur lors de la récupération d'une fiche disponible.");
                    $("#dialog").dialog("open");
                }
            });

        
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

            $.ajax({
                type: "POST",
                url: 'Home.aspx/validateMonth',
                async: false,
                data: JSON.stringify({ month: selectedMonth }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg, st) {
                    if (st == "success") {
                        JSON.parse(msg.d);
                        $("#grid").trigger("reloadGrid");
                        InitPromotionNb(selectedMonth);
                        InitChartComponent(selectedMonth);
                        $("#dialog").html("Mois Validé");
                        $("#dialog").dialog("open");
                    }
                },
                error: function () {
                    $("#dialog").html("Erreur lors de la validation du mois!");
                    $("#dialog").dialog("open");
                }
            });

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
                    if (verif == true)
                        document.location = "Edit.aspx?promotionId=" + promotionId + "&sessionId=" + sessionId + "&loginId=" + loginId;
                    else {
                        $("#dialog").html("Cette fiche est déjà en cours de codification !");
                        $("#dialog").dialog("open");
                    }
                },
                error: function () {
                    $("#dialog").html("Une Erreur est survenue. Impossible  d\'editer la fiche!");
                    $("#dialog").dialog("open");
                }
            });

           

        }

        function linkFormat(cellvalue, options, rowObject) {
            return "<a href=\"#\" onclick=\"javascript:EditRequest('"+cellvalue+"');\">Edit</a>";
        }

        function InitGridComponent(loadDate) {

            var grid = $("#grid");
            var vehicleStr = { '-1': 'Tous', '1': 'Presse', '3': 'Tv', '7': 'Internet', '8': 'Publicité Extérieur', '2': 'Radio' };
            var activationStr = { '-1': 'Toutes', '80': 'A Codifier', '60': 'Rejetée', '90': 'Codifiée', '70': 'Litige', '0': 'Validée' };
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
                        for (var i = loadDateList.length-1; i >= 0; i--)
                            loadDateStr['' + loadDateList[i] + ''] = loadDateList[i];
                    }
                },
                error: function () {
                    $("#dialog").html("Impossible d\'initilaiser la liste des fiches. Erreur lors du chargement des dates.");
                    $("#dialog").dialog("open");
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
                postData: { selectedDate: selectedMonth, selectedVehicle: selectedVehicle, selectedActivation: selectedActivation, sessionId: sessionId, loginId: loginId, filters: '' },
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
                                var postFilters = jQuery("#grid").jqGrid('getGridParam', 'postData').filters;
                                if (postFilters.length > 0) {
                                    var j = JSON.parse(postFilters);
                                    if (j != null && j.rules != null && j.rules.length == 3) {
                                        var loadDateFormatting = j.rules[2].data.substring(3, 7) + j.rules[2].data.substring(0, 2);
                                        selectedMonth = loadDateFormatting;
                                        selectedActivation = j.rules[1].data;
                                        selectedVehicle = j.rules[0].data;
                                    }
                                }
                                InitPromotionNb(selectedMonth);
                                InitChartComponent(selectedMonth);
                            }
                        },
                        error: function (data) {
                            $("#dialog").html("Erreur lors de la récupération de la liste des fiches.");
                            $("#dialog").dialog("open");
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
                    { name: 'VehicleName', index: 'VehicleName', stype: 'select', search: true, searchoptions: { value: vehicleStr, defaultValue: selectedVehicle } },
                    { name: 'DateMediaNum', index: 'DateMediaNum', search: false },
                    { name: 'Link', index: 'Link', formatter: linkFormat, search: false, sortable: false },
                    { name: 'ActivationName', index: 'ActivationName', hidden: false, search: true, stype: 'select', searchoptions: { value: activationStr, defaultValue: selectedActivation } },
                    { name: 'LoadDate', index: 'LoadDate', hidden: false, stype: 'select', search: true, searchoptions: { value: loadDateStr, defaultValue: selectedMonth.substring(4, 6) + "/" + selectedMonth.substring(0, 4) }, sortable: false }
                ],
                rowNum: 20,
                rowList: [10, 20, 30],
                pager: jQuery("#pager"),
                toppager: true,
                sortname: "IdForm",
                sortorder: "asc",
                viewrecords: true,
                width: 900,
                height: "100%",
                gridview: true,
                rowattr: function (rd) {
                    if (rd.ActivationName == "A Codifier") {
                        return { "class": "toCodifyStyle" };
                    }
                    else if (rd.ActivationName == "Codifiée") {
                        return { "class": "codifiedStyle" };
                    }
                    else if (rd.ActivationName == "Rejetée") {
                        return { "class": "rejectedStyle" };
                    }
                    else if (rd.ActivationName == "Litige") {
                        return { "class": "pendingStyle" };
                    }
                    else if (rd.ActivationName == "Validée") {
                        return { "class": "validatedStyle" };
                    }
                },
                caption: "Promotions PSA",
                gridComplete: function () {
                    $(".loading").hide();
                }
            }).jqGrid('navGrid', '#grid_toppager', {
                edit: false, add: false, del: false, search: false, refresh: false,
                beforeRefresh: function () {
                }
            },
            {}, // default settings for edit
            {}, // add
            {}, // delete
            { closeOnEscape: true, closeAfterSearch: true }, //search
            {}
        );
            jQuery("#grid").jqGrid('filterToolbar', { autosearch: true, stringResult: true });
        }

        function RefreshChart(loadDate) {

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
                    $("#dialog").html("Erreur lors de la création du graphique.");
                    $("#dialog").dialog("open");
                }
            });

            var chartObj = jQuery.jqplot('chart1');

            chartObj.series[0].data = dataG;
            chartObj.replot();

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
                    $("#dialog").html("Erreur lors de la création du graphique.");
                    $("#dialog").dialog("open");
                }
            });

            $('#chart1').empty();

            var plot2 = jQuery.jqplot('chart1', dataG, {
                title: ' ',
                seriesColors: ['#e8e8e8', '#94d472', '#fed2d2', '#f0c95b', '#B8D0DE'],
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
                    $("#dialog").html("Erreur lors de la récupération du nombre de fiches.");
                    $("#dialog").dialog("open");
                }
            });

            var oN = document.getElementById("promotionsNb");
            oN.innerHTML = "<strong>" + promotionNb + "</strong>";
        }

    </script>

        </form>
</body>
</html>


