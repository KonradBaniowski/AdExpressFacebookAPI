$(document).ready(function () {
    if (typeof jQuery === "undefined") { throw new Error("jQuery") }

    var listChart = ["chartReferKPI", "chartReferExpenditure", "chartPDM", "chartConcurKPI", "chartConcurExpenditure", "chartConcurEngagement", "chartConcurDecompositionEngagement", "chartConcurPlurimediaStacked"];

    $('#export-type').removeClass("hide");
    $('#export-type').selectpicker();

    $("#grid").igTreeGrid({
        primaryKey: "ID",
        width: "100%",
        defaultColumnWidth: 200,
        avgRowHeight: 60,
        autoGenerateColumns: true
    });
    var ds;
    var cols;
    var colsFixed;
    var needFixedColumns = false;
    var gridWidth;

    LoadSocialMediaUniverses();
    CallSocialMediaResult();

    function UnitFormatter(val) {
        if (val > 0)
            return $.ig.formatter(val, "number");

        return "";
    }

    function GetColumnsFormatter(columns, unit) {

        if (columns != null) {

            columns.forEach(function (elem) {
                if (elem.key != "ID" && elem.key != "PID" && elem.key != "PageName" && elem.key != "IdPage") {
                    elem.formatter = UnitFormatter;
                }
            });

            return columns;
        }

        return columns;
    }

    function CallSocialMediaResult() {
        $("#exportCharts").hide();
        $("#gridEmpty").hide();
        $.ajax({
            url: '/SocialMedia/SocialMediaResult',
            contentType: "application/x-www-form-urlencoded",
            type: "POST",
            datatype: "json",
            error: function (xmlHttpRequest, errorText, thrownError) {
                bootbox.alert(errorText);
                $("#gridLoader").addClass("hide");
            },
            success: function (data) {

                if (data != null && data != "") {
                    $('#combo > #pageAdverBrandTypeCombo').html('');
                    //ComboBox Init
                    $.each(data.combo, function (index, value) {
                        text = "";
                        for (var i = 0; i < Number(value.Level) ; i++) {
                            text += "&nbsp;&nbsp;&nbsp;&nbsp;"
                        }
                        text += value.Text;
                        $('#combo > #pageAdverBrandTypeCombo').append('<option ' + value.Selected + ' value="' + value.Value + '" title="' + value.Text + '">' + text + '</option>');
                    })
                    $('#pageAdverBrandTypeCombo').selectpicker('refresh');
                    $('#seriesType').val("like");
                    $('#seriesType').selectpicker('refresh');

                    dataTreeGrid = data.datagrid;
                    //cols = GetColumnsFormatter(data.columns, data.unit);
                    cols = data.columns;
                    colsFixed = data.columnsfixed;
                    needFixedColumns = data.needfixedcolumns;

                    for (i = 0; i < cols.length; i++) {
                        //if (cols[i].key == "PageName")
                        //    cols[i].template = $("#titleTmpl").html();
                        if (cols[i].key == "IdPageFacebook")
                            cols[i].template = $("#linkToPostTmpl").html();
                        if (cols[i].key == "Url")
                            cols[i].template = $("#linkUrlTmpl").html();
                        if (cols[i].key == "checkBox")
                            cols[i].template = $("#checkBoxTmpl").html();
                        if (cols[i].key == "PageName")
                            cols[i].template = $("#pageNameTmpl").html();
                        if (cols[i].key == "NumberFan") {
                            cols[i].template = $("#fanTmpl").html();
                        }
                    }

                    var schema = new $.ig.DataSchema("array", {
                        fields: data.schema
                    });

                    ds = new $.ig.DataSource({
                        type: "json",
                        schema: schema,
                        dataSource: dataTreeGrid,
                        callback: renderGrid
                    });

                    ds.dataBind();

                    
                    CallTopPostsAll();
                    if (data.concurSelected) {
                        $("#resaccord > #volet3").show();
                        CallReferChart(data.concurSelected);
                        CallPDMChart();
                        CallConcurChart();
                        CallPlurimediaStackedChart();
                    }
                    else {
                        CallReferChart(data.concurSelected);
                        $("#resaccord > #volet3").hide();
                    }

                }
                else {
                    $("#gridLoader").addClass("hide");
                    $("#grid").addClass("hide");
                    $("#resaccord").addClass("hide");
                    $("#KPIButtonFix").addClass("hide");
                    $("#gridEmpty").show();
                }
            }
        });
    }

    var renderGrid = function (success, error) {
        if (success) {

            $("#grid").igTreeGrid("destroy");
            $("#gridLoader").addClass("hide");
            $("#grid").removeClass("hide");
            $("#resaccord").removeClass("hide");
            $("#KPIButtonFix").removeClass("hide");
            $("#grid").igTreeGrid({
                dataSource: ds.dataView(),
                columns: cols,
                height: "100%",
                autoGenerateColumns: false,
                primaryKey: "ID",
                foreignKey: "PID",
                width: "1140px",
                autofitLastColumn: false,
                initialExpandDepth: 1

            })

        } else {
            bootbox.alert(error);
        }
    }

    //** charge les images au fur et a mesure que le teableau s'affiche (image page facebook)
    $("#grid").on("igtreegridrowsrendered igtreegridrowexpanded", function (evt, ui) {

        $(".imgPageFacebook").each(function () {
            var datas = $(this).attr('data-post').toString();

            var link = "/Image/GetPageImage?itemId=" + datas;
            $(this).attr("src", link);
        });
    });

    $("#grid").on("igtreegridrowexpanded igtreegridrowcollapsed igtreegriddatarendered", function (evt, ui) {
        /*Follow scroll*/
        var element = $('#grid_table');
        var bottom = element.offset().top + element.outerHeight(true);
        //$('#KPIButtonFix').data('bs.affix').options.offset = bottom;
        $(window).off('.affix');
        $('#KPIButtonFix').removeData('bs.affix').removeClass('affix affix-top affix-bottom');
        $('#KPIButtonFix').affix({ offset: { top: bottom } });
    });


    /** Change universe **/
    $(document).on("click", "#btn-universe-choice", function (event) {
        var selectedValue = $('#universe-choice').val();
        var params = {
            universeId: selectedValue
        };
        if (selectedValue != '0') {
            $("#gridLoader").removeClass("hide");
            $("#grid").addClass("hide");
            $("#resaccord").addClass("hide");
            $("#KPIButtonFix").addClass("hide");

            $.each(listChart, function (index, value) {
                try {
                    $("#" + value).igDataChart("destroy");
                } catch (err) { }
            });

            // event.preventDefault();
            $.ajax({
                url: '/Universe/ChangeMarketUniverse',
                contentType: "application/x-www-form-urlencoded",
                data: params,
                type: "POST",
                datatype: "json",
                error: function (xmlHttpRequest, errorText, thrownError) {
                    bootbox.alert(thrownError);
                },
                success: function (data) {
                    CallSocialMediaResult();
                }
            });
        }

    });



    /**Export des chartes**/
    function downloadCanvas(link, dataUrl, filename) {
        link.href = dataUrl;
        link.download = filename;
    }
    $(document).on("click", "#exportCharts", function (event) {
        var indexI = 0;
        var indexP = 0;

        var canvasAll = document.getElementById('exportCanvas');
        var contextAll = canvasAll.getContext('2d');

        contextAll.fillStyle = "rgba(0, 0, 0, 0.3)";
        contextAll.canvas.width = 800;
        contextAll.canvas.height = (listChart.length / 2) * 300;
        contextAll.fillRect(0, 0, canvasAll.width, canvasAll.height);

        $.each(listChart, function (index, value) {
            try{
                canvas = $("#" + value).igDataChart("exportImage", 400, 300);

                if (index % 2 === 0) {
                    contextAll.drawImage(canvas, 0, 300 * indexP);
                    indexP++
                }
                else {
                    contextAll.drawImage(canvas, 400, 300 * indexI);
                    indexI++
                }
            }catch(err){}

        });

        var dataURL = canvasAll.toDataURL();
        downloadCanvas(this, dataURL, 'charts.png');

        contextAll.clearRect(0, 0, canvas.width, canvas.height);

    });


    /** click on page name  **/
    $(document).on("click", ".pageNameFacebookLink", function (event) {

        var id = $(this).attr('data-pagefb');

        /***TODO : a mettre dans le success ***/
        $('#pageAdverBrandTypeCombo').val(id);
        $('#pageAdverBrandTypeCombo').selectpicker('refresh');

        var postPosition = $('#post-id').offset().top;
        $('html, body').animate({ scrollTop: postPosition }, 'slow');
        /*************************************/

        var array = id.split(",");
        var ids = []
        $.each(array, function (index, value) {
            ids.push(Number(value));
        });
       

        $.ajax({
            url: '/SocialMedia/GetPostbyIdpage',
            contentType: "application/x-www-form-urlencoded",
            type: "POST",
            datatype: "json",
            data: {
                ids: ids
            },
            error: function (xmlHttpRequest, errorText, thrownError) {
                bootbox.alert(thrownError);
            },
            success: function (data) {
                $('#top-post').html('').append(data);
                getData();
            }
        });

    });

});

$(window).resize(function () {
    $('#KPIButtonFix').width($('#KPIButtonFix').parent().width());
});
$(window).scroll(function () {
    $('#KPIButtonFix').width($('#KPIButtonFix').parent().width());
});

function getData(e) {

    $("[id^='chart-']").each(function (index) {
        var serieType = $('#seriesType').val();
        var brush = "#FFE100";
        switch (serieType) {
            case "share": brush = "#B8DC00"; break;
            case "comment": brush = "#00C8FF"; break;
            default: brush = "#FFE100"; break;
        }
        var dis = $(this);
        index = index + 1;
        var data = $("[id='" + serieType + "-" + index + "']");
        var arrayData = [];
        $.each(data, function (indexLike, value) {
            var datas = $(value).attr('name').split(",");
            var elem = {
                "DAY": "J0",
                Data: 0
            };
            arrayData.push(elem);
            $.each(datas, function (index, value) {
                index = index + 1;
                var elem = {
                    "DAY": "J" + index,
                    Data: Number(value)
                };
                arrayData.push(elem);
            });
        });

        dis.igDataChart({
            width: "100%",
            height: "250px",
            dataSource: arrayData,
            axes: [
                {
                    name: "Days",
                    type: "categoryX",
                    label: "DAY",
                    labelTextColor: "white"
                },
                {
                    name: "Value",
                    type: "numericY",
                    minimumValue: 0,
                    title: serieType.toUpperCase(),
                    majorStroke: "white",
                    labelTextColor: "white"
                }
            ],

            series: [{
                name: serieType,
                type: "line",
                title: serieType,
                xAxis: "Days",
                yAxis: "Value",
                brush: brush,
                valueMemberPath: "Data",
                isTransitionInEnabled: true,
                isHighlightingEnabled: true,
                thickness: 3,
                showTooltip: true,
            }]
        });
    });
}

function getDataReferKPI(e) {

    var dis = $("#chartReferKPI");
    var data = $(".elmtsChart");
    var serieType = $('#seriesType').val();
    var title = "Saisonnalité des " + serieType.toUpperCase() + " vs POST";
    var arrayData = [];
    $.each(data, function () {

        var datas = $(this).children(".monthRef").attr('name').split(",");
        $.each(datas, function (index, value) {
            var elem = {
                Month: value.substring(4, 6) + "/" + value.substring(2, 4),
                Comment: Number($(".elmtsChart").children(".commentRef").attr('name').split(",")[index]),
                Like: Number($(".elmtsChart").children(".likeRef").attr('name').split(",")[index]),
                Share: Number($(".elmtsChart").children(".shareRef").attr('name').split(",")[index]),
                Post: Number($(".elmtsChart").children(".postRef").attr('name').split(",")[index])
            };
            arrayData.push(elem);
        });

    });

    var listSerie = [
            {
                type: "column",
                isHighlightingEnabled: true,
                isTransitionInEnabled: true,
                name: "like",
                title: "Like",
                xAxis: "Month",
                yAxis: "KPI",
                valueMemberPath: "Like",
                brush: "#FFE100",
                outline: "#FFE100",
                showTooltip: true,
                tooltipTemplate: "LikeTooltipTemplate",
                legend: { element: "legendChartKPIFb" }
            },
            {
                type: "spline",
                isHighlightingEnabled: true,
                isTransitionInEnabled: true,
                name: "Posts",
                title: "Posts",
                xAxis: "Month",
                yAxis: "PostsAxe",
                valueMemberPath: "Post",
                brush: "#FF0080",
                outline: "#FF0080",
                showTooltip: true,
                tooltipTemplate: "PostTooltipTemplate",
                thickness: 3,
                legend: { element: "legendChartKPIFb" }
            },
            {
                type: "column",
                isHighlightingEnabled: true,
                isTransitionInEnabled: true,
                name: "share",
                title: "Share",
                xAxis: "Month",
                yAxis: "KPI",
                valueMemberPath: "Share",
                brush: "#B8DC00",
                outline: "#B8DC00",
                showTooltip: true,
                tooltipTemplate: "ShareTooltipTemplate",
                legend: { element: "legendChartKPIFb" }
            },
            {
                type: "column",
                isHighlightingEnabled: true,
                isTransitionInEnabled: true,
                name: "comment",
                title: "Comment",
                xAxis: "Month",
                yAxis: "KPI",
                valueMemberPath: "Comment",
                brush: "#00C8FF",
                outline: "#00C8FF",
                showTooltip: true,
                tooltipTemplate: "CommentTooltipTemplate",
                legend: { element: "legendChartKPIFb" }
            }
    ];

    dis.igDataChart({
        width: "100%",
        height: "300px",
        title: title,
        subtitle: "Mois par mois",
        titleTextColor: "white",
        subtitleTextColor: "white",
        horizontalZoomable: true,
        verticalZoomable: true,
        dataSource: arrayData,
        //overviewPlusDetailPaneVisibility: "visible",
        axes: [
                {
                    type: "categoryX",
                    name: "Month",
                    label: "Month",
                    title: "Mois",
                    labelTextColor: "white",
                }, {
                    type: "numericY",
                    name: "KPI",
                    title: "KPI",
                    majorStroke: "white",
                    stroke: "rgba(0,0,0,0)",
                    minimumValue: 0,
                    labelTextColor: "white",
                }, {
                    type: "numericY",
                    name: "PostsAxe",
                    labelLocation: "outsideRight",
                    title: "POST",
                    minimumValue: 0,
                    majorStroke: "rgba(0,0,0,0)",
                    stroke: "rgba(0,0,0,0)",
                    labelTextColor: "white",
                }
        ],

        series: listSerie.slice(0, 2)
    });

    $("#legendChartKPIFb").igChartLegend({ width: "100%" });

    $('#seriesType').on('change', function () {
        var serieType = $(this).val();
        var dis = $("#chartReferKPI");
        var series = dis.igDataChart("option", "series");
        var newListSeries = [];

        newListSeries = jQuery.grep(listSerie, function (value) {
            return value.name == serieType;
        });

        //TODO : Faire une boucle (fait comme ça pour avoir les Posts (type line) au debut de la list)!!
        dis.igDataChart("option", "series", [{ name: "like", remove: true }]);
        dis.igDataChart("option", "series", [{ name: "share", remove: true }]);
        dis.igDataChart("option", "series", [{ name: "comment", remove: true }]);
        dis.igDataChart("option", "series", [{ name: "Posts", remove: true }]);
        for (var i = 0; i < listSerie.length; i++) {
            if (listSerie[i].type != "column") {
                newListSeries.push(listSerie[i]);
            }
        }

        //for (var i = 0; i < series.length; i++) {
        //    if (series[i].type != "column") {
        //        newListSeries.push(series[i]);
        //    }
        //    if (series[i].name != serieType) { // && series[i].type == "column"
        //        dis.igDataChart("option", "series", [{ name: series[i].name, remove: true }]);
        //    }
        //}

        dis.igDataChart("option", "series", newListSeries);

        var serieType = $('#seriesType').val();
        dis.igDataChart("option", "title", "Saisonnalité des " + serieType.toUpperCase() + " vs POST");
        var axes = dis.igDataChart("option", "axes");

        //dis.igDataChart("option", "axes", [{ name: axes[1].name, remove: true }]);
        //dis.igDataChart("option", "axes",
        //    [{
        //        type: "numericY",
        //        name: "KPI",
        //        title: serieType.substr(0, 1).toUpperCase() + serieType.substr(1),
        //        majorStroke: "white",
        //        stroke: "rgba(0,0,0,0)",
        //        labelTextColor: "white",
        //    }]
        //);

    });
}

function getDataReferExpenditure(e) {

    var disExpenditure = $("#chartReferExpenditure");
    var data = $(".elmtsChart");

    var arrayData = [];
    $.each(data, function () {

        var datas = $(this).children(".monthRef").attr('name').split(",");
        $.each(datas, function (index, value) {
            var elem = {
                Month: value.substring(4, 6) + "/" + value.substring(2, 4),
                Expenditure: Number($(".elmtsChart").children(".expenditureRef").attr('name').split(",")[index]),
                Post: Number($(".elmtsChart").children(".postRef").attr('name').split(",")[index])
            };
            arrayData.push(elem);
        });

    });

    disExpenditure.igDataChart({
        height: "300px",
        width: "100%",
        title: "Saisonnalité des B€X vs POST",
        subtitle: "Mois par mois",
        titleTextColor: "white",
        subtitleTextColor: "white",
        horizontalZoomable: true,
        verticalZoomable: true,
        dataSource: arrayData,
        //overviewPlusDetailPaneVisibility: "visible",
        axes: [
                {
                    type: "categoryX",
                    name: "Month",
                    label: "Month",
                    title: "Mois",
                    labelTextColor: "white"
                }, {
                    type: "numericY",
                    name: "ExpenditureAxe",
                    title: "Brand Exposure",
                    majorStroke: "white",
                    stroke: "rgba(0,0,0,0)",
                    labelTextColor: "white"
                }, {
                    type: "numericY",
                    name: "PostsAxe",
                    labelLocation: "outsideRight",
                    title: "POST",
                    minimumValue: 0,
                    majorStroke: "rgba(0,0,0,0)",
                    stroke: "rgba(0,0,0,0)",
                    labelTextColor: "white"
                }
        ],

        series: [
            {
                type: "column",
                isHighlightingEnabled: true,
                isTransitionInEnabled: true,
                name: "Expenditure",
                title: "Brand Exposure",
                xAxis: "Month",
                yAxis: "ExpenditureAxe",
                valueMemberPath: "Expenditure",
                brush: "#FF8C00",
                outline: "#FF8C00",
                showTooltip: true,
                tooltipTemplate: "ExpenditureTooltipTemplate",
                legend: { element: "legendChartKPIFb" }
            },
            {
                type: "spline",
                isHighlightingEnabled: true,
                isTransitionInEnabled: true,
                name: "Posts",
                title: "Posts",
                xAxis: "Month",
                yAxis: "PostsAxe",
                valueMemberPath: "Post",
                brush: "#FF0080",
                outline: "#FF0080",
                showTooltip: true,
                tooltipTemplate: "PostTooltipTemplate",
                thickness: 3
            }
        ]
    });
}

function getDataPDM(e) {

    var disPDM = $("#chartPDM");
    var data = $(".elmtsChartPDM");

    var arrayData = [];
    $.each(data, function () {

        var datas = $(this).children(".monthRef").attr('name').split(",");
        $.each(datas, function (index, value) {
            var elem = {
                Month: value.substring(4, 6) + "/" + value.substring(2, 4),
                ReferentPercent: Number($(".elmtsChartPDM").children(".refPercentRef").attr('name').split(",")[index]),
                ConcurrentPercent: Number($(".elmtsChartPDM").children(".concPercentRef").attr('name').split(",")[index]),
                ReferentFBPercent: Number($(".elmtsChartPDM").children(".refFBPercentRef").attr('name').split(",")[index]),
                ConcurrentFBPercent: Number($(".elmtsChartPDM").children(".concFBPercentRef").attr('name').split(",")[index])
            };
            arrayData.push(elem);
        });

    });

    var listSeries = [
        function () { // a self executing function to create the series initialization object
            var seriesObj = {
                name: "PDM",
                xAxis: "Month",
                yAxis: "PDMAxe",
                type: "stacked100Column",
                series: [{
                    name: "ReferentPercent",
                    title: "ReferentPercent",
                    type: "stackedFragment",
                    valueMemberPath: "ReferentPercent",
                    brush: "#3C6BBF", outline: "#3C6BBF",
                    showTooltip: true,
                    tooltipTemplate: "referentPDMTooltipTemplate"
                },
                    {
                        name: "ConcurrentPercent",
                        title: "ConcurrentPercent",
                        type: "stackedFragment",
                        valueMemberPath: "ConcurrentPercent",
                        brush: "#E7E7E7", outline: "#E7E7E7",
                        showTooltip: true,
                        tooltipTemplate: "concurrentPDMTooltipTemplate"
                    }
                ]
            };
            return seriesObj;
        }()
    ];
    listSeries.push(
            {
                type: "spline",
                isHighlightingEnabled: true,
                isTransitionInEnabled: true,
                name: "ReferentFBPercent",
                title: "ReferentFBPercent",
                xAxis: "Month",
                yAxis: "PDMAxe",
                valueMemberPath: "ReferentFBPercent",
                brush: "#FF0080", outline: "#FF0080",
                thickness: 3,
                showTooltip: true,
                tooltipTemplate: "referentFaceBookPDMTooltipTemplate"
            }
        );

    disPDM.igDataChart({
        height: "300px",
        width: "100%",
        title: "PDM des référents vs univers Marché",
        subtitle: "INVESTISSMENTS PLURIMEDIA et B€X / mois par mois",
        titleTextColor: "white",
        subtitleTextColor: "white",
        horizontalZoomable: true,
        verticalZoomable: true,
        dataSource: arrayData,
        //overviewPlusDetailPaneVisibility: "visible",
        axes: [
                {
                    type: "categoryX",
                    name: "Month",
                    label: "Month",
                    title: "Mois",
                    labelTextColor: "white",
                    isTransitionInEnabled: true,
                    isHighlightingEnabled: true
                }, {
                    type: "numericY",
                    name: "PDMAxe",
                    title: "PDM (%)",
                    majorStroke: "white",
                    labelTextColor: "white",
                    isTransitionInEnabled: true,
                    isHighlightingEnabled: true
                }
        ],
        series: listSeries

    });
}

function getDataConcurKPI(e) {

    var dis = $("#chartConcurKPI");
    var data = $(".elmtsChartConcur");
    var serieType = $('#seriesType').val();
    var labelKPISelected = serieType.substr(0, 1).toUpperCase() + serieType.substr(1)
    brushes = ["#3C6BBF", "#8D3CC0", "#2B6077", "#14C896", "#B8292F", "#3C7EC0", "#B2912F", "#4D4D4D", "#60BD68", "#DECF3F"];
    intTabBrush = 0;

    var listSerie = [];
    var arrayMonth = [];
    $.each(data, function () {
        var arrayData = [];
        var datas = $(this).children(".monthConcur").attr('name').split(",");
        var label = $(this).children(".labelConcur").attr('name');
        var currentElmnt = $(this);
        arrayMonth = [];
        $.each(datas, function (index, value) {
            var elem = {
                Month: value.substring(4, 6) + "/" + value.substring(2, 4),
                Comment: Number(currentElmnt.children(".commentConcur").attr('name').split(",")[index]),
                Like: Number(currentElmnt.children(".likeConcur").attr('name').split(",")[index]),
                Share: Number(currentElmnt.children(".shareConcur").attr('name').split(",")[index]),
                Post: Number(currentElmnt.children(".postConcur").attr('name').split(",")[index])
            };
            arrayData.push(elem);
            arrayMonth.push({ Month: value.substring(4, 6) + "/" + value.substring(2, 4) });
        });
        listSerie.push(
                {
                    dataSource: arrayData,
                    type: (arrayMonth.length < 2) ? "point" : "spline",
                    markerType: (arrayMonth.length < 2) ? "circle" : "unset",
                    isHighlightingEnabled: true,
                    isTransitionInEnabled: true,
                    markerBrush: brushes[intTabBrush],
                    markerOutline: brushes[intTabBrush],
                    name: label,
                    title: label,
                    xAxis: "Month",
                    yAxis: "KPIAxe",
                    showTooltip: true,
                    valueMemberPath: labelKPISelected,
                    thickness: 3,
                    tooltipTemplate: "<div>Mois: <label class='bold'>${item.Month}</label></div><div>" + label + ": <label class='bold'>${item." + labelKPISelected + "}</label></div>"
                }
            );
        intTabBrush = (intTabBrush < brushes.length - 1) ? intTabBrush + 1 : 0
    });

    //var series = $(".selector").igMap("option", "series");
    //series[0].brush;

    dis.igDataChart({
        dataSource: arrayMonth,
        width: "100%",
        height: "300px",
        title: "Saisonnalité des " + serieType.toUpperCase(),
        subtitle: "Annonceur ou marque / mois par mois",
        brushes: ["#3C6BBF", "#8D3CC0", "#2B6077", "#14C896", "#B8292F", "#3C7EC0", "#B2912F", "#4D4D4D", "#60BD68", "#DECF3F"],
        outlines: ["#3C6BBF", "#8D3CC0", "#2B6077", "#14C896", "#B8292F", "#3C7EC0", "#B2912F", "#4D4D4D", "#60BD68", "#DECF3F"],
        titleTextColor: "white",
        subtitleTextColor: "white",
        horizontalZoomable: true,
        verticalZoomable: true,
        //overviewPlusDetailPaneVisibility: "visible",
        axes: [
                {
                    type: "categoryX",
                    name: "Month",
                    label: "Month",
                    title: "Mois",
                    labelTextColor: "white",
                }, {
                    type: "numericY",
                    name: "KPIAxe",
                    title: serieType.toUpperCase(),
                    labelTextColor: "white",
                }
        ],

        series: listSerie
    });

    $('#seriesType').on('change', function () {
        getDataConcurKPI();
    });
}

function getDataConcurExpenditure(e) {

    var disExpenditure = $("#chartConcurExpenditure");
    var data = $(".elmtsChartConcur");
    brushes = ["#3C6BBF", "#8D3CC0", "#2B6077", "#14C896", "#B8292F", "#3C7EC0", "#B2912F", "#4D4D4D", "#60BD68", "#DECF3F"];
    intTabBrush = 0;

    var listSerie = [];
    var arrayMonth = [];
    $.each(data, function () {
        var arrayData = [];
        var datas = $(this).children(".monthConcur").attr('name').split(",");
        var label = $(this).children(".labelConcur").attr('name');
        var currentElmnt = $(this);
        arrayMonth = [];
        $.each(datas, function (index, value) {
            var elem = {
                Month: value.substring(4, 6) + "/" + value.substring(2, 4),
                Expenditure: Number(currentElmnt.children(".expenditureConcur").attr('name').split(",")[index]),
            };
            arrayData.push(elem);
            arrayMonth.push({ Month: value.substring(4, 6) + "/" + value.substring(2, 4) });
        });
        listSerie.push(
                {
                    dataSource: arrayData,
                    type: (arrayMonth.length < 2) ? "point" : "spline",
                    markerType: (arrayMonth.length < 2) ? "circle" : "unset",
                    isHighlightingEnabled: true,
                    isTransitionInEnabled: true,
                    name: label,
                    title: label,
                    markerBrush: brushes[intTabBrush],
                    markerOutline: brushes[intTabBrush],
                    xAxis: "Month",
                    yAxis: "ExpenditureAxe",
                    valueMemberPath: "Expenditure",
                    thickness: 3,
                    showTooltip: true,
                    tooltipTemplate: "<div>Mois: <label class='bold'>${item.Month}</label></div><div>" + label + ": <label class='bold'>${item.Expenditure}</label></div>"
                }
            );
        intTabBrush = (intTabBrush < brushes.length - 1) ? intTabBrush + 1 : 0
    });

    disExpenditure.igDataChart({
        dataSource: arrayMonth,
        height: "300px",
        width: "100%",
        title: "Saisonnalité des B€X",
        subtitle: "Annonceur ou marque / mois par mois",
        brushes: ["#3C6BBF", "#8D3CC0", "#2B6077", "#14C896", "#B8292F", "#3C7EC0", "#B2912F", "#4D4D4D", "#60BD68", "#DECF3F"],
        outlines: ["#3C6BBF", "#8D3CC0", "#2B6077", "#14C896", "#B8292F", "#3C7EC0", "#B2912F", "#4D4D4D", "#60BD68", "#DECF3F"],
        titleTextColor: "white",
        subtitleTextColor: "white",
        horizontalZoomable: true,
        verticalZoomable: true,
        //overviewPlusDetailPaneVisibility: "visible",
        axes: [
                {
                    type: "categoryX",
                    name: "Month",
                    label: "Month",
                    title: "Mois",
                    labelTextColor: "white"
                }, {
                    type: "numericY",
                    name: "ExpenditureAxe",
                    title: "Brand Exposure",
                    labelTextColor: "white"
                }
        ],

        series: listSerie
    });
}

function getDataConcurEngagement(e) {

    var disExpenditure = $("#chartConcurEngagement");
    var data = $(".elmtsChartConcur");

    var listSerie = [];
    var arrayData = [];
    var elem = {};
    $.each(data, function () {
        var datas = $(this).children(".monthConcur").attr('name').split(",");
        var label = $(this).children(".labelConcur").attr('name');
        //var concurMemberPathLabel = "Engagement".concat($(this).children(".IdConcur").attr('name'))
        var concurMemberPathLabel = label;
        elem[concurMemberPathLabel] = 0;
        var currentElmnt = $(this);
        $.each(datas, function (index, value) {
            elem[concurMemberPathLabel] = elem[concurMemberPathLabel] + Number(currentElmnt.children(".likeConcur").attr('name').split(",")[index])
                            + Number(currentElmnt.children(".shareConcur").attr('name').split(",")[index])
                            + Number(currentElmnt.children(".commentConcur").attr('name').split(",")[index]);
        });
        listSerie.push(
                {
                    type: "bar",
                    isHighlightingEnabled: true,
                    isTransitionInEnabled: true,
                    name: label,
                    title: label,
                    xAxis: "EngagementAxe",
                    yAxis: "yAxis",
                    valueMemberPath: concurMemberPathLabel,
                    showTooltip: true,
                    radius: 0,
                    legend: { element: "legendChartFb" }
                }
            );

    });
    arrayData.push(elem);

    disExpenditure.igDataChart({
        dataSource: arrayData,
        height: "300px",
        width: "100%",
        title: "Niveau d’ENGAGEMENT",
        subtitle: "Ventilé par annonceur ou marque",
        titleTextColor: "white",
        subtitleTextColor: "white",
        brushes: ["#3C6BBF", "#8D3CC0", "#2B6077", "#14C896", "#B8292F", "#3C7EC0", "#B2912F", "#4D4D4D", "#60BD68", "#DECF3F"],
        outlines: ["#3C6BBF", "#8D3CC0", "#2B6077", "#14C896", "#B8292F", "#3C7EC0", "#B2912F", "#4D4D4D", "#60BD68", "#DECF3F"],
        horizontalZoomable: true,
        verticalZoomable: true,
        //overviewPlusDetailPaneVisibility: "visible",
        axes: [
                {
                    type: "numericX",
                    name: "EngagementAxe",
                    title: "Engagement",
                    labelTextColor: "white",
                }, {
                    name: "yAxis",
                    type: "categoryY",
                    overlap: -.1
                }
        ],

        series: listSerie
    });

    $("#legendChartFb").igChartLegend({ width: "100%" });
}

function getDataConcurDecompositionEngagement(e) {

    var disExpenditure = $("#chartConcurDecompositionEngagement");
    var data = $(".elmtsChartConcur");

    var listSerie = [];
    var elem = {};
    $.each(data, function () {
        var datas = $(this).children(".monthConcur").attr('name').split(",");
        var label = $(this).children(".labelConcur").attr('name');
        elem = { Like: 0, Share: 0, Comment: 0 };
        elem["Label"] = label;
        var currentElmnt = $(this);
        $.each(datas, function (index, value) {
            elem.Like = elem.Like + Number(currentElmnt.children(".likeConcur").attr('name').split(",")[index]),
            elem.Comment = elem.Comment + Number(currentElmnt.children(".commentConcur").attr('name').split(",")[index]),
            elem.Share = elem.Share + Number(currentElmnt.children(".shareConcur").attr('name').split(",")[index])
        });

        listSerie.push(
                function () { // a self executing function to create the series initialization object
                    var seriesObj = {
                        name: label,
                        xAxis: "EngagementAxe",
                        yAxis: "yAxis",
                        dataSource: [elem],
                        type: "stackedBar",
                        radius: 0,
                        outline: "transparent",
                        series: [{
                            name: "like",
                            title: "Like",
                            type: "stackedFragment",
                            valueMemberPath: "Like",
                            showTooltip: true,
                        }, {
                            name: "share",
                            title: "Share",
                            type: "stackedFragment",
                            valueMemberPath: "Share",
                            showTooltip: true,
                        },
                        {
                            name: "comment",
                            title: "Comment",
                            type: "stackedFragment",
                            valueMemberPath: "Comment",
                            showTooltip: true,
                        }
                        ]
                    };
                    return seriesObj;
                }()
            );
    });

    disExpenditure.igDataChart({
        dataSource: [{ Like: 0, Share: 0, Comment: 0, Label: "" }],
        height: "300px",
        width: "100%",
        title: "Cumul des LIKE, SHARE et COMMENT",
        subtitle: "Ventilé par annonceur ou marque",
        titleTextColor: "white",
        subtitleTextColor: "white",
        brushes: ["#FFE100", "#B8DC00", "#00C8FF"],
        horizontalZoomable: true,
        verticalZoomable: true,
        //overviewPlusDetailPaneVisibility: "visible",
        axes: [
                {
                    type: "numericX",
                    name: "EngagementAxe",
                    title: "KPI",
                    labelTextColor: "white"
                }, {
                    name: "yAxis",
                    type: "categoryY",
                    label: "Label",
                    overlap: -.1
                }
        ],

        series: listSerie
    });
}

function getDataPlurimediaStacked(e) {

    var disPDM = $("#chartConcurPlurimediaStacked");
    var data = $(".elmtsChartPDVByMediaConcur");

    var arrayData = [];
    var listSubSeries = [];
    $.each(data, function () {
        listSubSeries = [];
        var media = $(this).children(".labelMediaConcur").attr('name');
        var elem = { "Media": media };
        var currentElmnt = $(this);
        var datas = currentElmnt.children(".idAdvertiserBrandConcur").attr('name').split(",");
        $.each(datas, function (index, value) {
            var label = currentElmnt.children(".labelAdvertiserBrandConcur").attr('name').split(",")[index];
            elem[label] = Number(currentElmnt.children(".expenditureConcur").attr('name').split(",")[index]);

            listSubSeries.push({
                name: label,
                title: label,
                type: "stackedFragment",
                valueMemberPath: label,
                showTooltip: true,
                tooltipTemplate: "<div>Media: <label class='bold'>${item.Media}</label></div><div>" + label + ": <label class='bold'>${item."+label+"} %</label></div>"
            });

        });
        arrayData.push(elem);

    });

    var listSeries = [
        function () { // a self executing function to create the series initialization object
            var seriesObj = {
                name: "PDV",
                xAxis: "MediaAxe",
                yAxis: "PDVAxe",
                type: "stacked100Column",
                series: listSubSeries,
            };
            return seriesObj;
        }()
    ];

    disPDM.igDataChart({
        height: "300px",
        width: "100%",
        title: "PDV annonceurs ou marques ventilées par Media",
        subtitle: "INVESTISSEMENTS et B€X",
        titleTextColor: "white",
        subtitleTextColor: "white",
        horizontalZoomable: true,
        verticalZoomable: true,
        brushes: ["#3C6BBF", "#8D3CC0", "#2B6077", "#14C896", "#B8292F", "#3C7EC0", "#B2912F", "#4D4D4D", "#60BD68", "#DECF3F"],
        outlines: ["#3C6BBF", "#8D3CC0", "#2B6077", "#14C896", "#B8292F", "#3C7EC0", "#B2912F", "#4D4D4D", "#60BD68", "#DECF3F"],
        dataSource: arrayData,
        //overviewPlusDetailPaneVisibility: "visible",
        axes: [
                {
                    type: "categoryX",
                    name: "MediaAxe",
                    label: "Media",
                    title: "Media",
                    labelTextColor: "white",
                    isTransitionInEnabled: true,
                    isHighlightingEnabled: true
                }, {
                    type: "numericY",
                    name: "PDVAxe",
                    title: "PDV (%)",
                    majorStroke: "white",
                    labelTextColor: "white",
                    isTransitionInEnabled: true,
                    isHighlightingEnabled: true
                }
        ],
        series: listSeries

    });
}

function CallReferChart(concurSelected) {
    $.ajax({
        url: '/SocialMedia/GetReferChart',
        contentType: "application/x-www-form-urlencoded",
        type: "POST",
        datatype: "json",
        error: function (xmlHttpRequest, errorText, thrownError) {
            bootbox.alert(thrownError);
        },
        success: function (data) {
            $('#Refer-chart').html('').append(data);
            getDataReferKPI();
            getDataReferExpenditure();
            $(".mediaLoaderRefer").hide();

            /*Follow scroll*/
            $('#KPIButtonFix').affix({
                offset: { top: $('#KPIButtonFix').offset().top }
            });

            if (!concurSelected) {
                $("#exportCharts").show();
            }
        }
    });
}

function CallPDMChart() {
    $.ajax({
        url: '/SocialMedia/GetPDMChart',
        contentType: "application/x-www-form-urlencoded",
        type: "POST",
        datatype: "json",
        error: function (xmlHttpRequest, errorText, thrownError) {
            bootbox.alert(thrownError);
        },
        success: function (data) {
            $('#PDM-chart').html('').append(data);
            getDataPDM();
            $(".mediaLoaderPDM").hide();
        }
    });
}

function CallConcurChart() {
    $.ajax({
        url: '/SocialMedia/GetConcurChart',
        contentType: "application/x-www-form-urlencoded",
        type: "POST",
        datatype: "json",
        error: function (xmlHttpRequest, errorText, thrownError) {
            bootbox.alert(thrownError);
        },
        success: function (data) {
            $('#Concur-chart').html('').append(data);
            getDataConcurKPI();
            getDataConcurExpenditure();
            getDataConcurEngagement();
            getDataConcurDecompositionEngagement();
            $(".mediaLoaderConcur").hide();
        }
    });
}

function CallPlurimediaStackedChart() {
    $.ajax({
        url: '/SocialMedia/GetPlurimediaStackedChart',
        contentType: "application/x-www-form-urlencoded",
        type: "POST",
        datatype: "json",
        error: function (xmlHttpRequest, errorText, thrownError) {
            bootbox.alert(thrownError);
        },
        success: function (data) {
            $('#Stacked-chart').html('').append(data);
            getDataPlurimediaStacked();
            $(".mediaLoaderPlurimediaStacked").hide();
            $("#exportCharts").show();
        }
    });
}

function CallTopPostsAll() {
    var ids = []
    $('#pageAdverBrandTypeCombo > option').each(function () {
        if ($(this).val().split(",").length == 1 && $(this).val().split(",") != "*" && Number($(this).val().split(",")) != 0)
            ids.push(Number($(this).val().split(",")[0]));
    });
    if (ids.length == 0)
        return false;

    var params = {
        ids: ids
    };
    $.ajax({
        url: '/SocialMedia/GetPostbyIdpage',
        contentType: "application/x-www-form-urlencoded",
        type: "POST",
        datatype: "json",
        data: {
            ids: ids
        },
        error: function (xmlHttpRequest, errorText, thrownError) {
            bootbox.alert(thrownError);
        },
        success: function (data) {
            $('#top-post').html('').append(data);
            getData();
            $(".mediaLoaderTopPost").hide();
        }
    });
}

function SetListUnivers() {
    $.ajax({
        url: '/SocialMedia/GetPostbyIdpage',
    });
}

$('#pageAdverBrandTypeCombo, #seriesType').on('change', function () {
    var id = $('#pageAdverBrandTypeCombo').val();
    if (!id)
        return false;
    if (id == "*") {
        CallTopPostsAll();
        return;
    }

    var array = id.split(",");
    var ids = []
    $.each(array, function (index, value) {
        ids.push(Number(value));
    });
    console.log(ids);
    var params = {
        ids: ids
    };
    $.ajax({
        url: '/SocialMedia/GetPostbyIdpage',
        contentType: "application/x-www-form-urlencoded",
        type: "POST",
        datatype: "json",
        data: {
            ids: ids
        },
        error: function (xmlHttpRequest, errorText, thrownError) {
            bootbox.alert(thrownError);
        },
        success: function (data) {
            $('#top-post').html('').append(data);
            getData();
            var postPosition = $('#post-id').offset().top;
            $('html, body').animate({ scrollTop: postPosition }, 'slow');
        }
    });
});

$("#postFacebookModal").attr('src', '');
$("#postFacebookModal").on('shown.bs.modal', function (event) {
    var button = $(event.relatedTarget);// Button that triggered the modal
    var datas = button.data('creative').toString(); // Extract info from data-* attributes

    if (datas === null || datas == "" || datas == 0 || datas == "0") {
        bootbox.alert("Post indisponible.");
    }
    else {
        var params = {
            id: datas
        };
        $.ajax({
            url: '/SocialMedia/GetKPIByPostId',
            ccontentType: "application/x-www-form-urlencoded",
            type: 'POST',
            datatype: 'JSON',
            data: params,
            error: function (xmlHttpRequest, errorText, thrownError) {
                bootbox.alert("error");
            },
            success: function (response) {
                $('#postFacebookModal .modal-content .modal-body').html('').append(response);
                $('#postFacebookModal').modal('show');
                getDataZoom();
                $("#mediaLoader").hide();
            }
        });

    }
});

$("#postFacebookModal").on('hide.bs.modal', function () {
    $("#objectPost").attr('src', '');
    $('#postFacebookModal .modal-content .modal-body').html('');
    $("#mediaLoader").show();
});

function getDataZoom(e) {
    var serieType = $('#seriesTypeZoom').val();
    var brush = "#FFE100";
    switch (serieType) {
        case "share": brush = "#B8DC00"; break;
        case "comment": brush = "#00C8FF"; break;
        default: brush = "#FFE100"; break;
    }
    var dis = $('#chart');
    var data = $("[id='" + serieType + "']");
    var arrayData = [];
    var series = []
    $.each(data, function (indexLike, value) {
        var serie = {
            name: "name",
            type: "line",
            title: "1995",
            xAxis: "Days",
            yAxis: "Value",
            valueMemberPath: indexLike,
            isTransitionInEnabled: true,
            isHighlightingEnabled: true,
            thickness: 5
        };
        var datas = $(value).attr('value').split(",");
        var elem = {
            "DAY": "J0",
            Data: 0
        };
        arrayData.push(elem);
        $.each(datas, function (index, value) {
            index = index + 1;
            var elem = {
                "DAY": "J" + index,
                Data: Number(value)
            };
            arrayData.push(elem);
        });
        series.push(serie);
    });
    dis.igDataChart({
        width: "70%",
        height: "250px",
        //title: "Evolution des partages",
        dataSource: arrayData,
        axes: [
            {
                name: "Days",
                type: "categoryX",
                label: "DAY"
            },
            {
                name: "Value",
                type: "numericY",
                minimumValue: 0,
                title: serieType.toUpperCase(),
            }
        ],

        series: [{
            name: "wahtever",
            type: "line",
            title: "1995",
            xAxis: "Days",
            yAxis: "Value",
            brush: brush,
            valueMemberPath: "Data",
            isTransitionInEnabled: true,
            isHighlightingEnabled: true,
            thickness: 5
        }]
    });

    $('#unityZoom > .form-control').on('change', function () {
        getDataZoom();
    });

}

function LoadSocialMediaUniverses() {

    var params = {
        dimension: 0
    };
    $.ajax({
        url: '/Universe/GetModuleUniverses',
        contentType: "application/x-www-form-urlencoded",
        data: params,
        type: "POST",
        datatype: "json",
        error: function (xmlHttpRequest, errorText, thrownError) {
            bootbox.alert(thrownError);
        },
        success: function (data) {
            if (data != null)
                AppendUniverseComboBox(data);
        }
    });
}

function AppendUniverseComboBox(data) {
    //$(".custom-button.selectexporttype.select-universe-choice").empty();
    $(".custom-button.selectexporttype.select-universe-choice").hide();

    if (data != null) {

        var htmlArr = [];

        //htmlArr.push("  <div class='pull-right custom-button selectexporttype'>");
        htmlArr.push("  <select id='universe-choice' class='selectdatepicker'>");
        //Default
        htmlArr.push(" <option value='0'>");
        htmlArr.push("Sélectionner un univers marché");
        htmlArr.push("</option>");
        $.each(data, function (i, val) {
            htmlArr.push(" <option value='");
            htmlArr.push(val.Id);
            htmlArr.push("'");
            if (val.IsDefault == true) {
                htmlArr.push(" selected ");
            }
            htmlArr.push(">");
            htmlArr.push(val.Description);
            htmlArr.push("</option>");
        });
        htmlArr.push("  </select>&nbsp;");
        htmlArr.push("  <button class='btn btn-save' id='btn-universe-choice' title='Sélectionner un univers marché'><i class='fa fa-check fa-file-excel-size'></i></button>");//<!--btn-universe-choice-->
        //htmlArr.push(" </div>"); //<!--pull-right custom-button selectexporttype-->      
        $(".custom-button.selectexporttype.select-universe-choice").html(htmlArr.join(""));
        $('#universe-choice').selectpicker();
        $(".custom-button.selectexporttype.select-universe-choice").show();
    }
}

$("#btn-save-result").click(function () {
    $("#resultModal").modal("show");
});

$("#resultModal").on('shown.bs.modal', function (event) {
    var params = {
        id: 0
    };
    CallUserResult(params);
});

$("#resultModal").on('hide.bs.modal', function () {
    $("#resultModal").html('<div class="modal-dialog"><div class="modal-content"></div></div>)');
});

function SaveResultEvents() {
    $("#folders").on('change', function (event) {
        var idFolder = $("#folders").val();
        var idResult = $('#results').val();
        var params = {
            id: idFolder
        };
        CallUserResult(params);
    });
}

function CallUserResult(params) {
    $.ajax({
        url: "/Universe/UserResult",
        contentType: "application/x-www-form-urlencoded",
        type: "GET",
        datatype: "json",
        data: params,
        error: function (xmlHttpRequest, errorText, thrownError) {
        },
        success: function (data) {
            $('#resultModal').html(data);
            SaveResultEvents();
            CallSaveResult();
        }
    });
}

function CallSaveResult() {
    $('#btnSaveResult').on('click', function (e) {
        var idFolder = $("#folders").val();
        var idResult = $('#results').val();
        var resultName = $('#resultName').val();
        var params = {
            folderId: idFolder,
            saveAsResultId: idResult,
            saveResult: resultName
        };
        $.ajax({
            url: "/Universe/SaveUserResult",
            contentType: "application/x-www-form-urlencoded",
            type: "POST",
            datatype: "json",
            data: params,
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                if (data != null && data != "") {
                    bootbox.alert(data.Message);
                }
            }
        });
    });
}