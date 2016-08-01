﻿
if (typeof jQuery === "undefined") { throw new Error("No jQuery") }

$("#grid").igTreeGrid({
    //dataSource: data,
    primaryKey: "Id",
    width: "95%",
    defaultColumnWidth: 200,
    avgRowHeight: 60,
    autoGenerateColumns: true,
    features: [
            {
                name: "Sorting",
                type: "local",
                applySortedColumnCss: false
            }
]
});


var ds;
var cols;
var colsFixed;
var needFixedColumns = false;

var renderGrid = function (success, error) {

    if (success) {
        $("#grid").igTreeGrid("destroy");
        $("#gridLoader").hide();
        $("#grid").show();
        var height = window.innerHeight - $("#grid").offset().top - 10;

        $("#grid").igTreeGrid({
            dataSource: ds.dataView(),
            columns: cols,
            height: height,
            autoGenerateColumns: true,
            primaryKey: "ID",
            foreignKey: "PID",
            width: "90%",
            features: [
                {
                    name: "MultiColumnHeaders"
                },
                {
                    name: "Paging",
                    mode: "allLevels",
                    pageSize: 100
                },
                {
                    name: "Sorting",
                    type: "local",
                    applySortedColumnCss: false
                }
            ]
        })

    } else {
        alert("Error");
    }
}

function CallInsertionsResult() {
    $("#gridEmpty").hide();
    $("#gridLoader").show();
    var ids = $('#ids').val();//$('#paramsUrl[0]').val();ids
    var period = $('.btn-group.btn-group-margin > .btn.btn-default-sub-period.sub-period-btn').attr("period");
    var type = $('#type').val();
    var parameters = {
        ids:ids,
        period: period
    };

    $.ajax({
        url: '/SocialMedia/GetSocialMediaCreative',
        contentType: 'application/json',
        type: "POST",
        datatype: "json",
        data: JSON.stringify(parameters),
        error: function (xmlHttpRequest, errorText, thrownError) {
            bootbox.alert("Error : GetSocialMediaCreative");
            $("#gridLoader").hide();
            $("#gridEmpty").show();
        },
        success: function (data) {

            if (data != null && data != "") {
                dataTreeGrid = data.datagrid;
                var a = dataTreeGrid.length;
                colsFixed = data.columnsfixed;
                needFixedColumns = data.needfixedcolumns;

                var schema = new $.ig.DataSchema("array", {
                    fields: data.schema
                });

                cols = data.columns

                for (i = 0; i < cols.length; i++) {
                    if (cols[i].key == "IdPostFacebook")
                        cols[i].template = $("#colPostTmpl").html();
                }

                ds = new $.ig.DataSource({
                    type: "json",
                    schema: schema,
                    dataSource: dataTreeGrid,
                    callback: renderGrid
                });

                ds.dataBind();
            }
            else {
                $("#gridLoader").hide();
                $("#btn-export").hide();
                $("#gridEmpty").show();
                $("#grid").hide();
            }
        }
    });
}

CallInsertionsResult();


//** charge les images au fur et a mesure que le teableau s'affiche (image post facebook)
$("#grid").on("igtreegridrowsrendered igtreegridrowexpanding igtreegridrowcollapsing", function (evt, ui) {

    //AutoPlayVisu();

    $(".carousel").each(function (index) {
        $("#visuCarou" + index.toString()).carousel("pause");
        $("#visuCarou" + index.toString()).find('.left').click(function () {
            $("#visuCarou" + index.toString()).carousel("prev");
        });
        $("#visuCarou" + index.toString()).find('.right').click(function () {
            $("#visuCarou" + index.toString()).carousel("next");
        });
    });


    $(".imgPostsFacebook").each(function () {
        var datas = $(this).attr('data-post').toString();
        var link = "/Image/GetPostImage?itemId=" + datas;

        $(this).attr("src", link);
    });

});

$("#postFacebookModal").attr('src', '');
$("#postFacebookModal").on('shown.bs.modal', function (event) {
    $("#mediaLoader").show();
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
                getData();
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

//Resize de la page, la longuer du tableau egalament
$(window).resize(function () {
    $("#grid").igTreeGrid("option", "height", window.innerHeight - $("#grid").offset().top - 10);
});

function getData(e) {
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
                    brush:brush,
                    thickness: 5
                }]
            });
       
    $('#unityZoom > .form-control').on('change', function () {
        getData();
    });
}

$(document).on("click", ".btn-group.btn-group-margin > .btn.btn-default", function (event) {
    $("#btn-export").hide();
    $("#gridEmpty").hide();
    $("#grid").hide();
    $("#gridLoader").show();
    var element = this;
    var period = $(element).attr("period");
    var ids = $('#ids').val();
    var parameters = {
        ids: ids,
        period: period
    };

    $.ajax({
        url: '/SocialMedia/GetSocialMediaCreative',
        contentType: 'application/json',
        type: "POST",
        datatype: "json",
        data: JSON.stringify(parameters),
        error: function (xmlHttpRequest, errorText, thrownError) {
            bootbox.alert("Error : GetSocialMediaCreative");
            $("#gridLoader").hide();
            $("#gridEmpty").show();
        },
        success: function (data) {

            if (data != null && data != "") {
                dataTreeGrid = data.datagrid;
                colsFixed = data.columnsfixed;
                needFixedColumns = data.needfixedcolumns;

                var schema = new $.ig.DataSchema("array", {
                    fields: data.schema
                });

                cols = data.columns

                for (i = 0; i < cols.length; i++) {
                    if (cols[i].key == "IdPostFacebook")
                        cols[i].template = $("#colPostTmpl").html();
                }

                ds = new $.ig.DataSource({
                    type: "json",
                    schema: schema,
                    dataSource: dataTreeGrid,
                    callback: renderGrid
                });

                ds.dataBind();
            }
            else {
                $("#gridLoader").hide();
                $("#gridEmpty").show();
                $("#grid").hide();
            }
        }
    });
    $(".btn-default-sub-period").prop('disabled', false);
    $(".btn-default-sub-period").attr('class', 'btn btn-default sub-period-btn');
    $(this).attr('class', 'btn btn-default-sub-period sub-period-btn');
    $(this).prop('disabled', true);
    $(".sub-period-btn").on('mouseenter', function (e) {
        previousSubPeriodLabel = $(".btn-default-sub-period").attr("periodlabel");
        $("#subPeriodLabel").text($(this).attr("periodlabel"));
    });
    $(".sub-period-btn").on('mouseleave', function (e) {
        $("#subPeriodLabel").text(previousSubPeriodLabel);
    });
});

$(document).on("click", "#btn-export", function (event) {
    var ids = $('#ids').val();
    var period = $('.btn-group.btn-group-margin > .btn.btn-default-sub-period.sub-period-btn').attr("period");
    var params = "?ids=" + ids + "&period=" + period;
    window.open('/SocialMediaExport/CreativeExport'+params, "_blank");
});