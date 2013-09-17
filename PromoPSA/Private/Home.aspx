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
    <script src="/js/i18n/grid.locale-fr.js" type="text/javascript"></script>
    <script src="/js/jquery.jqGrid.min.js" type="text/javascript"></script>
    <!--[if lt IE 9]><script language="javascript" type="text/javascript" src="excanvas.js"></script><![endif]-->
    <script src="/js/jquery.jqplot.min.js" type="text/javascript" ></script>
    <script src="/js/jqplot.pieRenderer.js" type="text/javascript" ></script>
     <script src="/js/jqplot.json2.min.js" type="text/javascript" ></script>
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
        <div style=" margin-left:250px;">
            <table id="grid" style="margin-right:auto; margin-left:auto;"></table>
            <div id="pager"></div>

        </div>
    
    <script type="text/javascript">
        $(document).ready(function () {
            
            var dataG ;

            $.ajax({
                url: 'Home.aspx/getChartData',
                type: "POST",
                async: false,
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
                seriesColors: ['#e8e8e8', '#94d472', '#fed2d2'],
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
                }, legend: { show: true, location: 'e' }
            });

            var grid = $("#grid");
            var vehicleStr = {'0':'' , '1': 'Presse', '3': 'Tv', '7': 'Internet', '8': 'Publicité Extérieur' };
            var activationStr = { '0': '', '40': 'A Codifier', '30': 'Rejetée', '20': 'Codifiée' };
            var loadDateStr;

            $.ajax({
                url: 'Home.aspx/getLoadDates',
                type: "POST",
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, st) {
                    if (st == "success") {
                        loadDateStr = JSON.parse(data.d);
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
                postData: { searchString: '', searchField: '', searchOper: '' },
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
                colNames: ['Id Form', 'Media', 'Parution Date', 'Edit', 'Activation', 'Load Date'],
                colModel: [
                    { name: 'IdForm', index: 'IdForm', width: 55, search: false },
                    { name: 'VehicleName', index: 'VehicleName', width: 200, stype: 'select', searchoptions: { sopt: ['eq', 'ne'], value: vehicleStr, defaultValue: '1' } },
                    { name: 'DateMediaNum', index: 'DateMediaNum', width: 200, search: false },
                    { name: 'Link', index: 'Link', formatter: linkFormat, width: 55, search: false, sortable: false },
                    { name: 'ActivationName', index: 'ActivationName', hidden: false, stype: 'select', searchoptions: { sopt: ['eq', 'ne'], value: activationStr, defaultValue: '1' } },
                    { name: 'LoadDate', index: 'LoadDate', hidden: false, searchoptions: { sopt: ['eq', 'ne'], value: loadDateStr, defaultValue: '1' }, sortable: false }
                ],
                rowNum: 20,
                rowList: [10, 20, 30],
                pager: jQuery("#pager"),
                sortname: "IdForm",
                sortorder: "asc",
                viewrecords: true,
                width: 800,
                height: 479,
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
                },
                caption: "Promotions PSA",
                gridComplete: function () {
                    $(".loading").hide();
                }
            }).jqGrid('navGrid', '#pager', { edit: false, add: false, del: false },
            {}, // default settings for edit
            {}, // add
            {}, // delete
            { closeOnEscape: true, closeAfterSearch: true }, //search
            {}
        )
        });
        
        function linkFormat(cellvalue, options, rowObject) {
            return '<a href="' + cellvalue + '" >Edit</a>';
        }

    </script>

        </form>
</body>
</html>


