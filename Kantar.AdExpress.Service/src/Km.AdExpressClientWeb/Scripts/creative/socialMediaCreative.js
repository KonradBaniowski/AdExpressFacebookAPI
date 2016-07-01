
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
        if (needFixedColumns) {
            $("#grid").igTreeGrid({
                dataSource: ds.dataView(),
                columns: cols,
                height: height,
                autoGenerateColumns: false,
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
                    },
                    {
                        name: "ColumnFixing",
                        fixingDirection: "left",
                        columnSettings: colsFixed
                    }
                ]
            })
        }
        else {
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
        }
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
        var link = "PostsFacebook/" + datas.substring(0, 1) + "/" + datas.substring(1, 4) + "/" + datas + "_Post.png"
        $(this).attr("src", link);
    });

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