$(document).ready(function () {
    if (typeof jQuery === "undefined") { throw new Error("jQuery") }

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

    CallSocialMediaResult();

    function UnitFormatter(val) {
        if (val > 0)
            return $.ig.formatter(val, "number");

        return "";
    }

    function PageFormatter(val) {
        if (val > 0)
            return $.ig.formatter(val, "number", "#,##0.###");

        return "";
    }

    function DurationFormatter(val) {

        if (val == 0)
            return "";

        var s = val.toString();
        var nbToFillWithZero = 6 - s.length;
        for (var i = 0; i < nbToFillWithZero; i++)
            s = "0" + s;
        return s.substr(0, 2) + " H " + s.substr(2, 2) + " M " + s.substr(4, 2) + " S";
    }

    function GetColumnsFormatter(columns, unit) {

        if (columns != null) {

            columns.forEach(function (elem) {
                if (elem.key != "ID" && elem.key != "PID" && elem.key != "PageName" && elem.key != "IdPage") {
                    if (unit == "duration")
                        elem.formatter = DurationFormatter;
                    else if (unit == "pages")
                        elem.formatter = PageFormatter;
                    else
                        elem.formatter = UnitFormatter;
                }
            });

            return columns;
        }

        return columns;
    }

    function CallSocialMediaResult() {
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

                    //ComboBox Init
                    $.each(data.combo, function (index, value) {
                        $('#combo > .form-control').append('<option ' + value.Selected + ' value=' + value.Value + '>' + value.Text + '</option>');
                    })

                    dataTreeGrid = data.datagrid;
                    //cols = GetColumnsFormatter(data.columns, data.unit);
                    cols = data.columns;
                    colsFixed = data.columnsfixed;
                    needFixedColumns = data.needfixedcolumns;

                    for (i = 0; i < cols.length; i++) {
                        if (cols[i].key == "PageName")
                            cols[i].template = $("#titleTmpl").html();
                        if (cols[i].key == "IdPageFacebook")
                            cols[i].template = $("#linkToPostTmpl").html();
                        if (cols[i].key == "Url")
                            cols[i].template = $("#linkUrlTmpl").html();
                        if (cols[i].key == "checkBox")
                            cols[i].template = $("#checkBoxTmpl").html();
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

                    CallReferChart();
                    CallPDMChart();
                    CallConcurChart();
                    CallPlurimediaStackedChart();
                }
                else {
                    $("#gridLoader").addClass("hide");
                    $("#grid").addClass("hide");
                    $("#resaccord").addClass("hide");
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
            $("#grid").igTreeGrid({
                dataSource: ds.dataView(),
                columns: cols,
                height: "400px",
                autoGenerateColumns: false,
                primaryKey: "ID",
                foreignKey: "PID",
                width: "1140px",
                autofitLastColumn: false,
                features: [
                    {
                        name: "MultiColumnHeaders"
                    },
                    {
                        name: "Paging",
                        mode: "allLevels",
                        pageSize: 100
                    }
                ]
            })

            gridWidth = $("#grid_table_headers").width();
            gridWidth += $("#grid_table_headers_fixed").width();

            if (gridWidth > 1140)
                gridWidth = 1140;

            $("#grid").igTreeGrid("option", "width", gridWidth + "px");

        } else {
            bootbox.alert(error);
        }
    }

    //** charge les images au fur et a mesure que le teableau s'affiche (image page facebook)
    $("#grid").on("igtreegridrowsrendered igtreegridrowexpanding igtreegridrowcollapsing", function (evt, ui) {

        $(".imgPageFacebook").each(function () {
            var datas = $(this).attr('data-post').toString();

            //var link = "http://192.168.158.145/POSTS/" + datas.substring(0, 1) + "/" + datas.substring(1, 4) + "/new_" + datas + ".jpg"

            var link = "/Image/GetPageImage?itemId=" + datas;
            $(this).attr("src", link);
        });

    });
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
            autoMarginHeight: 15,
            autoMarginWidth: 15,
            width: "70%",
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
                    title: serieType,
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
                thickness: 4,
                showTooltip:true
            }]
        });
    });
}

function getDataReferKPI(e) {

    var dis = $("#chartReferKPI");
    var data = $(".elmtsChart");

    var arrayData = [];
    $.each(data, function () {

        var datas = $(this).children(".monthRef").attr('name').split(",");
        $.each(datas, function (index, value) {
            var elem = {
                Month: Number($(".elmtsChart").children(".monthRef").attr('name').split(",")[index]),
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
            },
            {
                type: "line",
                isHighlightingEnabled: true,
                isTransitionInEnabled: true,
                name: "Posts",
                title: "Posts",
                xAxis: "Month",
                yAxis: "PostsAxe",
                valueMemberPath: "Post",
                brush: "#FF0080",
                showTooltip: true,
                tooltipTemplate: "PostTooltipTemplate",
                thickness: 3
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
                tooltipTemplate: "ShareTooltipTemplate"
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
                tooltipTemplate: "CommentTooltipTemplate"
            }
    ];


    dis.igDataChart({
        autoMarginHeight: 15,
        autoMarginWidth: 15,
        width: "100%",
        height: "300px",
        title: "Titre à definir",
        subtitle: "Sous titre à definir",
        titleTextColor: "white",
        subtitleTextColor: "white",
        horizontalZoomable: true,
        verticalZoomable: true,
        dataSource: arrayData,
        axes: [
                {
                    type: "categoryX",
                    name: "Month",
                    label: "Month",
                    title: "Month",
                    labelTextColor: "white",
                }, {
                    type: "numericY",
                    name: "KPI",
                    title: "KPI",
                    majorStroke: "white",
                    stroke: "rgba(0,0,0,0)",
                    labelTextColor: "white",
                }, {
                    type: "numericY",
                    name: "PostsAxe",
                    labelLocation: "outsideRight",
                    title: "Posts",
                    minimumValue: 0,
                    majorStroke: "rgba(0,0,0,0)",
                    stroke: "rgba(0,0,0,0)",
                    labelTextColor: "white",
                }
        ],

        series: listSerie.slice(0, 2)
    });

    $('#seriesType').on('change', function () {
        var serieType = $(this).val();
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
                Month: Number($(".elmtsChart").children(".monthRef").attr('name').split(",")[index]),
                Expenditure: Number($(".elmtsChart").children(".expenditureRef").attr('name').split(",")[index]),
                Post: Number($(".elmtsChart").children(".postRef").attr('name').split(",")[index])
            };
            arrayData.push(elem);
        });

    });

    disExpenditure.igDataChart({
        autoMarginHeight: 15,
        autoMarginWidth: 15,
        height: "300px",
        width: "100%",
        title: "Titre à definir",
        subtitle: "Sous titre à definir",
        titleTextColor: "white",
        subtitleTextColor: "white",
        horizontalZoomable: true,
        verticalZoomable: true,
        dataSource: arrayData,
        axes: [
                {
                    type: "categoryX",
                    name: "Month",
                    label: "Month",
                    title: "Month",
                    labelTextColor: "white"
                }, {
                    type: "numericY",
                    name: "ExpenditureAxe",
                    title: "Expenditure",
                    majorStroke: "white",
                    stroke: "rgba(0,0,0,0)",
                    labelTextColor: "white"
                }, {
                    type: "numericY",
                    name: "PostsAxe",
                    labelLocation: "outsideRight",
                    title: "Posts",
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
                title: "Expenditure",
                xAxis: "Month",
                yAxis: "ExpenditureAxe",
                valueMemberPath: "Expenditure",
                brush: "#FF8C00",
                outline: "#FF8C00",
                showTooltip: true,
                tooltipTemplate: "ExpenditureTooltipTemplate"
            },
            {
                type: "line",
                isHighlightingEnabled: true,
                isTransitionInEnabled: true,
                name: "Posts",
                title: "Posts",
                xAxis: "Month",
                yAxis: "PostsAxe",
                valueMemberPath: "Post",
                brush: "#FF0080",
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
                Month: Number($(".elmtsChartPDM").children(".monthRef").attr('name').split(",")[index]),
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
                outline: "transparent",
                series: [{
                    name: "ReferentPercent",
                    title: "ReferentPercent",
                    type: "stackedFragment",
                    valueMemberPath: "ReferentPercent"
                },
                    {
                        name: "ConcurrentPercent",
                        title: "ConcurrentPercent",
                        type: "stackedFragment",
                        valueMemberPath: "ConcurrentPercent"
                    }
                ]
            };
            return seriesObj;
        }()
    ];
    listSeries.push(
            {
                type: "line",
                isHighlightingEnabled: true,
                isTransitionInEnabled: true,
                name: "ReferentFBPercent",
                title: "ReferentFBPercent",
                xAxis: "Month",
                yAxis: "PDMAxe",
                valueMemberPath: "ReferentFBPercent",
                thickness: 3
            }
        );

    disPDM.igDataChart({
        autoMarginHeight: 15,
        autoMarginWidth: 15,
        height: "300px",
        width: "100%",
        title: "Titre à definir",
        subtitle: "Sous titre à definir",
        titleTextColor: "white",
        subtitleTextColor: "white",
        horizontalZoomable: true,
        verticalZoomable: true,
        dataSource: arrayData,
        brushes: ["#14C896", "#B8292F", "#2D2B62", "#000000", "#787878", "#E7E7E7", "#9C1E8D", "#3C6BBF", "#51266B", "#2B6077"],
        axes: [
                {
                    type: "categoryX",
                    name: "Month",
                    label: "Month",
                    title: "Month",
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
                Month: Number(value),
                Comment: Number(currentElmnt.children(".commentConcur").attr('name').split(",")[index]),
                Like: Number(currentElmnt.children(".likeConcur").attr('name').split(",")[index]),
                Share: Number(currentElmnt.children(".shareConcur").attr('name').split(",")[index]),
                Post: Number(currentElmnt.children(".postConcur").attr('name').split(",")[index])
            };
            arrayData.push(elem);
            arrayMonth.push({ Month: Number(value) });
        });
        listSerie.push(
                {
                    dataSource: arrayData,
                    type: "line",
                    isHighlightingEnabled: true,
                    isTransitionInEnabled: true,
                    name: label,
                    title: label,
                    xAxis: "Month",
                    yAxis: "KPIAxe",
                    valueMemberPath: serieType.substr(0, 1).toUpperCase() + serieType.substr(1),
                    thickness: 3
                }
            );
    });

    dis.igDataChart({
        autoMarginHeight: 15,
        autoMarginWidth: 15,
        dataSource: arrayMonth,
        width: "100%",
        height: "300px",
        title: "Titre à definir",
        subtitle: "Sous titre à definir",
        titleTextColor: "white",
        subtitleTextColor: "white",
        brushes: ["#E7E7E7", "#9C1E8D", "#3C6BBF", "#51266B", "#2B6077"],
        horizontalZoomable: true,
        verticalZoomable: true,
        axes: [
                {
                    type: "categoryX",
                    name: "Month",
                    label: "Month",
                    title: "Month",
                    labelTextColor: "white",
                }, {
                    type: "numericY",
                    name: "KPIAxe",
                    title: "KPI",
                    majorStroke: "white",
                    stroke: "rgba(0,0,0,0)",
                    labelTextColor: "white",
                }
        ],

        series: listSerie
    });
}

function getDataConcurExpenditure(e) {

    var disExpenditure = $("#chartConcurExpenditure");
    var data = $(".elmtsChartConcur");

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
                Month: Number(value),
                Expenditure: Number(currentElmnt.children(".expenditureConcur").attr('name').split(",")[index]),
            };
            arrayData.push(elem);
            arrayMonth.push({ Month: Number(value) });
        });
        listSerie.push(
                {
                    dataSource: arrayData,
                    type: "line",
                    isHighlightingEnabled: true,
                    isTransitionInEnabled: true,
                    name: label,
                    title: label,
                    xAxis: "Month",
                    yAxis: "ExpenditureAxe",
                    valueMemberPath: "Expenditure",
                    thickness: 3
                }
            );

    });

    disExpenditure.igDataChart({
        autoMarginHeight: 15,
        autoMarginWidth: 15,
        dataSource: arrayMonth,
        height: "300px",
        width: "100%",
        title: "Titre à definir",
        subtitle: "Sous titre à definir",
        titleTextColor: "white",
        subtitleTextColor: "white",
        horizontalZoomable: true,
        verticalZoomable: true,
        axes: [
                {
                    type: "categoryX",
                    name: "Month",
                    label: "Month",
                    title: "Month",
                    labelTextColor: "white"
                }, {
                    type: "numericY",
                    name: "ExpenditureAxe",
                    title: "Expenditure",
                    majorStroke: "white",
                    stroke: "rgba(0,0,0,0)",
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
                    outline: "transparent",
                    radius: 0
                }
            );

    });
    arrayData.push(elem);
    disExpenditure.igDataChart({
        autoMarginHeight: 15,
        autoMarginWidth: 15,
        dataSource: arrayData,
        height: "300px",
        width: "100%",
        title: "Titre à definir",
        subtitle: "Sous titre à definir",
        titleTextColor: "white",
        subtitleTextColor: "white",
        brushes: ["#000000", "#E7E7E7", "#9C1E8D", "#3C6BBF", "#51266B", "#2B6077"],
        horizontalZoomable: true,
        verticalZoomable: true,
        axes: [
                {
                    type: "numericX",
                    name: "EngagementAxe",
                    title: "Engagement",
                    labelTextColor: "white"
                }, {
                    name: "yAxis",
                    type: "categoryY",
                    overlap: -.1
                }
        ],

        series: listSerie
    });
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
            elem.Comment = elem.Comment + Number(currentElmnt.children(".shareConcur").attr('name').split(",")[index]),
            elem.Share = elem.Share + Number(currentElmnt.children(".commentConcur").attr('name').split(",")[index])
        });

        listSerie.push(
                function () { // a self executing function to create the series initialization object
                    var seriesObj = {
                        name: label,
                        xAxis: "EngagementAxe",
                        yAxis: "yAxis",
                        dataSource: [elem],
                        type: "stackedBar",
                        outline: "transparent",
                        radius: 0,
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
        title: "Titre à definir",
        subtitle: "Sous titre à definir",
        titleTextColor: "white",
        subtitleTextColor: "white",
        brushes: ["#FFE100", "#B8DC00", "#00C8FF"],
        horizontalZoomable: true,
        verticalZoomable: true,
        axes: [
                {
                    type: "numericX",
                    name: "EngagementAxe",
                    title: "Engagement",
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
                outline: "transparent",
                series: listSubSeries,
            };
            return seriesObj;
        }()
    ];

    disPDM.igDataChart({
        autoMarginHeight: 15,
        autoMarginWidth: 15,
        height: "300px",
        width: "100%",
        title: "Titre à definir",
        subtitle: "Sous titre à definir",
        titleTextColor: "white",
        subtitleTextColor: "white",
        horizontalZoomable: true,
        verticalZoomable: true,
        dataSource: arrayData,
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

function CallReferChart() {
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
        }
    });
}

function SetListUnivers() {
    $.ajax({
        url: '/SocialMedia/GetPostbyIdpage',
    });
}

$('#combo > .form-control, #unity > .form-control').on('change', function () {
    var id = $('#combo > .form-control').val();
    if (!id)
        return false;
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
                $('#postFacebookModal .modal-content').html('').append(response);
                $('#postFacebookModal').modal('show');
                getDataZoom();
            }
        });

    }
});

function getDataZoom(e) {
    var serieType = $('#seriesTypeZoom').val();
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
        autoMarginHeight: 15,
        autoMarginWidth: 15,
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
                title: serieType,
            }
        ],

        series: [{
            name: "wahtever",
            type: "line",
            title: "1995",
            xAxis: "Days",
            yAxis: "Value",
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





