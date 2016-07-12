
if (typeof jQuery === "undefined") { throw new Error("No jQuery") }

$("#grid").igTreeGrid({
    //dataSource: data,
    primaryKey: "Id",
    width: "95%",
    defaultColumnWidth: 200,
    avgRowHeight: 60,
    autoGenerateColumns: true
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
            width: "100%",
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

    } else {
        alert("Error");
    }
}

function CallInsertionsResult() {
    $("#gridEmpty").hide();
    $("#gridLoader").show();
    var ids = $('#ids').val();//$('#paramsUrl[0]').val();ids
    var type = $('#type').val();
    var parameters = {
        ids:ids,
        type:type
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
        var link = "http://192.168.158.145/POSTS/" + datas.substring(0, 1) + "/" + datas.substring(1, 4) + "/" + datas + "_Post.png"
        $(this).attr("src", link);
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
        //id = datas;
        //$("#objectPost").attr('src', "http://kmchmd1002:82/Facebook/GetPost?id=" + id);
        //$("#objectPost").show();
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
                getData();               
            }
        });

    }
});

$("#postFacebookModal").on('hide.bs.modal', function () {
    $("#objectPost").attr('src', '');
    $("#objectPost").hide()
    $("#chartsPostFacebook").hide();
    $("#chartsPostFacebook").hide();
    $("#mediaLoader").show();
});



//Resize de la page, la longuer du tableau egalament
$(window).resize(function () {
    $("#grid").igTreeGrid("option", "height", window.innerHeight - $("#grid").offset().top - 10);
});
$(document).on('shown.bs.collapse', '#lvlperso', function (e) {
    $("#grid").igTreeGrid("option", "height", window.innerHeight - $("#grid").offset().top - 10);
});
$(document).on('hidden.bs.collapse', '#lvlperso', function (e) {
    $("#grid").igTreeGrid("option", "height", window.innerHeight - $("#grid").offset().top - 10);
});

function getData(e) {
            var serieType = $('#seriesType').val();
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
                        Data: value
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
       
    $('#unity > .form-control').on('change', function () {
        getData();
    });

}