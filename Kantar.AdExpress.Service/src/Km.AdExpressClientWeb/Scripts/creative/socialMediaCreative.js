
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
        contentType: "application/x-www-form-urlencoded",
        type: "POST",
        datatype: "json",
        data: parameters,
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
            contentType: 'application/json',
            type: 'POST',
            datatype: 'JSON',
            data: JSON.stringify(params),
            error: function (xmlHttpRequest, errorText, thrownError) {
                bootbox.alert("error");
            },
            success: function (response) {
                $('#postFacebookModal .modal-content').html('');
                $('#postFacebookModal .modal-content').append(response);
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
    console.log(e);

    $("[id^='chart-div-comment']").each(function (index) {
        var dis = $(this);
        var data = dis.prev().attr('name').split(",");

        console.log(data);
        var arrayData = [];
        $.each(data, function (index, value) {
            index = index + 1;
            var elem = {
                "COMMENT": "J" + index,
                "NB": value
            };
            arrayData.push(elem);
        });
        dis.igDataChart({
            autoMarginHeight: 15,
            autoMarginWidth: 15,
            //width: "33%",
            //height: "250px",
            title: "Evolution des commentaires",
            dataSource: arrayData,
            axes: [
                {
                    name: "NameAxis",
                    type: "categoryX",
                    label: "COMMENT"
                },
                {
                    name: "PopulationAxis",
                    type: "numericY",
                    minimumValue: 0,
                    title: "COMMENTS in K",
                }
            ],
            series: [
                {
                    name: "2005Population",
                    type: "line",
                    title: "2005",
                    xAxis: "NameAxis",
                    yAxis: "PopulationAxis",
                    valueMemberPath: "NB",
                    isTransitionInEnabled: true,
                    isHighlightingEnabled: true,
                    thickness: 5
                }]
        });
    });

    $("[id^='chart-div-like']").each(function (index) {
        var dis = $(this);
        var data = dis.prev().attr('name').split(",");

        console.log(data);
        var arrayData = [];
        $.each(data, function (index, value) {
            index = index + 1;
            var elem = {
                "LIKE": "J" + index,
                "NB": value
            };
            arrayData.push(elem);
        });
        dis.igDataChart({
            autoMarginHeight: 15,
            autoMarginWidth: 15,
            //width: "33%",
            //height: "250px",
            title: "Evolution des likes",
            dataSource: arrayData,
            axes: [
                {
                    name: "NameAxis",
                    type: "categoryX",
                    label: "LIKE"
                },
                {
                    name: "PopulationAxis",
                    type: "numericY",
                    minimumValue: 0,
                    title: "LIKES in K",
                }
            ],
            series: [
                {
                    name: "2005Population",
                    type: "line",
                    title: "2005",
                    xAxis: "NameAxis",
                    yAxis: "PopulationAxis",
                    valueMemberPath: "NB",
                    isTransitionInEnabled: true,
                    isHighlightingEnabled: true,
                    thickness: 5
                }]
        });
    });
    $("[id^='chart-div-share']").each(function (index) {
        var dis = $(this);
        var data = dis.prev().attr('name').split(",");

        console.log(data);
        var arrayData = [];
        $.each(data, function (index, value) {
            index = index + 1;
            var elem = {
                "SHARE": "J" + index,
                "NB": value
            };
            arrayData.push(elem);
        });
        dis.igDataChart({
            autoMarginHeight: 15,
            autoMarginWidth: 15,
            //width: "33%",
            //height: "250px",
            title: "Evolution des partages",
            dataSource: arrayData,
            axes: [
                {
                    name: "NameAxis",
                    type: "categoryX",
                    label: "SHARE"
                },
                {
                    name: "PopulationAxis",
                    type: "numericY",
                    minimumValue: 0,
                    title: "SHARES in K",
                }
            ],
            series: [
                {
                    name: "SHARE",
                    type: "line",
                    title: "SHARE",
                    xAxis: "NameAxis",
                    yAxis: "PopulationAxis",
                    valueMemberPath: "NB",
                    isTransitionInEnabled: true,
                    isHighlightingEnabled: true,
                    thickness: 5
                }]
        });
    });
}