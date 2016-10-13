$(document).ready(function () {
    if (typeof jQuery === "undefined") { throw new Error("jQuery") }

    $("#grid").igTreeGrid({
        primaryKey: "ClassifId",
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
    var sortOrder = "NONE";
    var columnIndex = 1;

    function MediaDetailLevel() {
        this.LevelDetailValue = $('#mediaDetail').val();
    }

    function ProductDetailLevel() {
        this.LevelDetailValue = $('#productDetail').val();
    }

    function ResultTypeFilter() {
        this.ResultType = $('#resultType').val();
    }

    function UserFilter() {
        this.MediaDetailLevel = new MediaDetailLevel();
        this.ProductDetailLevel = new ProductDetailLevel();
        this.Evol = false;
        this.PDM = false;
        this.PDV = false;
        this.ResultTypeFilter = new ResultTypeFilter();
        this.IsSelectRetailerDisplay = false;
    }

    var userFilter = new UserFilter();
    function ProductDetailLevel() {
        this.LevelDetailValue = $('#productDetail').val();
    }

    function ResultTypeFilter() {
        this.ResultType = $('#resultType').val();
    }

    function UserFilter() {
        this.MediaDetailLevel = new MediaDetailLevel();
        this.ProductDetailLevel = new ProductDetailLevel();
        this.Evol = false;
        this.PDM = false;
        this.PDV = false;
        this.ResultTypeFilter = new ResultTypeFilter();
    }

    var userFilter = new UserFilter();
    function CallSetOptions() {
        $.ajax({
            url: '/Analysis/SetResultOptions',
            contentType: "application/x-www-form-urlencoded",
            type: "POST",
            datatype: "json",
            data: userFilter,
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                sortOrder = "NONE";
                columnIndex = 1;
                CallAnalysisResult();
            }
        });
    }

    CallAnalysisResult();
    $('#export-type').removeClass("hide");
    $('#export-type').selectpicker();
    $('#btn-export').unbind('click').on('click', function (e) {
        var selectedValue = $('#export-type').val();
        var params = "?sortOrder=" + sortOrder + "&columnIndex=" + columnIndex;
        switch (selectedValue) {
            case "1":
                window.open('/ProductClassAnalysisExport/Index' + params, "_blank");
                break;
            case "2":
                window.open('/ProductClassAnalysisExport/ResultBrut', "_blank");
                break;
            default:
                window.open('/ProductClassAnalysisExport/Index' + params, "_blank");
                break;
        }
    });

    $(document).on('click', '#btn-create-alert', function (event) {
        $('#btn-create-alert').off();
        var params = {

        };
        $.ajax({
            url: '/Alert/CreateAlert',
            contentType: 'application/json',
            type: 'POST',
            datatype: 'JSON',
            //data: JSON.stringify(params),
            error: function (xmlHttpRequest, errorText, thrownError) {
                bootbox.alert("An error occurred while processing your request.");
            },
            success: function (response) {
                $('#alertModal .modal-content').html('');
                $('#alertModal .modal-content').append(response);
                $('#alertModal').modal('show');
            }
        });
    });

    $('#mediaDetail').selectpicker();

    $('#mediaDetail').on('change', function (e) {
        userFilter.MediaDetailLevel.LevelDetailValue = $('#mediaDetail').val();
    });

    $('#productDetail').selectpicker();

    $('#productDetail').on('change', function (e) {
        userFilter.ProductDetailLevel.LevelDetailValue = $('#productDetail').val();
    });

    $('#resultType').selectpicker();

    $('#resultType').on('change', function (e) {
        userFilter.ResultTypeFilter.ResultType = $('#resultType').val();
    });

    if ($("#set-evol").prop('checked') == true) {
        userFilter.Evol = true;
    }
    else {
        userFilter.Evol = false;
    }

    $("#set-evol").click(function () {
        if ($(this).prop('checked') == true) {
            userFilter.Evol = true;
        }
        else {
            userFilter.Evol = false;
        }
    });

    if ($("#set-pdm").prop('checked') == true) {
        userFilter.PDM = true;
    }
    else {
        userFilter.PDM = false;
    }

    $("#set-pdm").click(function () {
        if ($(this).prop('checked') == true) {
            userFilter.PDM = true;
        }
        else {
            userFilter.PDM = false;
        }
    });

    if ($("#set-pdv").prop('checked') == true) {
        userFilter.PDV = true;
    }
    else {
        userFilter.PDV = false;
    }

    $("#set-pdv").click(function () {
        if ($(this).prop('checked') == true) {
            userFilter.PDV = true;
        }
        else {
            userFilter.PDV = false;
        }
    });

    if ($("#retailer-display").prop('checked') == true) {
        userFilter.IsSelectRetailerDisplay = true;
    }
    else {
        userFilter.IsSelectRetailerDisplay = false;
    }

    $("#retailer-display").click(function () {
        if ($(this).prop('checked') == true) {
            userFilter.IsSelectRetailerDisplay = true;
        }
        else {
            userFilter.IsSelectRetailerDisplay = false;
        }
    });

    $('#validate-options').on('click', function (e) {
        $("#grid").addClass("hide");
        $("#gridLoader").removeClass("hide");
        $("#collapseContainerOne").collapse('hide');
        CallSetOptions();
    });

    //$(document).on('click', '.ui-iggrid-header.ui-widget-header', function (event) {
    $(document).on('click', '*[id*=grid_table_g]', function (event) {
        var element = $(this);
        sortFunc(element);
    });
   
    function CallAnalysisResult() {
        $("#gridEmpty").hide();
        var params = {
            sortOrder: sortOrder,
            columnIndex: columnIndex
        };

        $.ajax({
            url: '/Analysis/AnalysisResult',
            contentType: "application/x-www-form-urlencoded",
            type: "POST",
            datatype: "json",
            data: params,
            error: function (xmlHttpRequest, errorText, thrownError) {
                var message = $('#Labels_ResultError').val() + '. ' + $('#Labels_WarningBackNavigator').val();
                bootbox.alert(message);
                $("#gridLoader").addClass("hide");
            },
            success: function (data) {
                if (data != null && data != "") {
                    dataTreeGrid = data.datagrid;
                    cols = data.columns;
                    colsFixed = data.columnsfixed;
                    needFixedColumns = data.needfixedcolumns;

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
                }
                else {
                    $("#gridLoader").addClass("hide");
                    $("#grid").addClass("hide");
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
            $("#grid").igTreeGrid({
                dataSource: ds.dataView(),
                columns: cols,
                height: "530px",
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
                    },
                    {
                        name: "ColumnFixing",
                        fixingDirection: "left",
                        columnSettings: colsFixed
                    }
                    ]
            })

            gridWidth = $("#grid_table_headers").width();
            gridWidth += $("#grid_table_headers_fixed").width();

            if (gridWidth > 1140)
                gridWidth = 1140;

            $("#grid").igTreeGrid("option", "width", gridWidth + "px");

            $("#grid").igTreeGrid({
                rowCollapsed: function (evt, ui) {
                    $("#grid_table_container").attr("style", "position: relative; height: 530px; width: " + gridWidth + "px;");
                }
            });

            //$('.ui-iggrid-header.ui-widget-header').each(function (index) {
            $('*[id*=grid_table_g]').each(function (index) {
                var element = $(this);
                element.attr("title", "Cliquez pour trier la colonne");
                element.css("cursor", "pointer");
                var child = element.find('.ui-iggrid-headertext');
                child.css("cursor", "pointer");
                if (index == (columnIndex - 1)) {
                    if (sortOrder != "NONE") {
                        var elementWidth = element.width();
                        var childWidth = child.width();

                        $(".customColumnSizeSort").removeClass("customColumnSizeSort");
                        var elem = $(this).find(".ui-iggrid-headertext");
                        elem.each(function () {
                            this.style.setProperty('width', '80%', 'important');
                            this.style.setProperty('margin-left', '-3px');
                        });
                        elem.addClass("customColumnSizeSort");


                        //if (elementWidth == childWidth || (elementWidth - childWidth) <= 20) {
                        //    var str = child.html();
                        //    str = str.replace(/&nbsp;/g, " ");
                        //    if (str.length > 6)
                        //        str = str.substring(0, str.length - 6) + "...";
                        //    else
                        //        str = str.substring(0, str.length - 3) + ".";

                        //    child.html(str);
                        //}
                        element.addClass("ui-iggrid-colheaderasc-ktr");
                        if (sortOrder == "ASC")
                            element.append('<div class="ui-iggrid-indicatorcontainer" style="margin-right:-8px !important;"><span class="ui-iggrid-colindicator ui-iggrid-colindicator-asc ui-icon ui-icon-arrowthick-1-n"></span></div>');
                        else
                            element.append('<div class="ui-iggrid-indicatorcontainer" style="margin-right:-8px !important;"><span class="ui-iggrid-colindicator ui-iggrid-colindicator-desc ui-icon ui-icon-arrowthick-1-s"></span></div>');
                    }
                }

            });
        
        } else {
            bootbox.alert(error);
        }
    }

    var sortFunc = function (field) {
        var index = field[0].id.split("-")[0].split("_g")[1];
        if (sortOrder == "NONE")
            sortOrder = "ASC";
        else if (sortOrder == "ASC")
            sortOrder = "DESC";
        else if (sortOrder == "DESC")
            sortOrder = "ASC";
        columnIndex = parseInt(index);

        $("#grid").addClass("hide");
        $("#gridLoader").removeClass("hide");
        CallAnalysisResult();
    }

    //Export
    $("#btn-save-result").click(function () {
        $("#exportResultModal").modal("show");
    });

    $("#exportResultModal").on('shown.bs.modal', function (event) {
        var params = {
            id: 0
        };
        CallUserResult(params);
    });

    $("#exportResultModal").on('hide.bs.modal', function () {
        $("#exportResultModal").html('<div class="modal-dialog"><div class="modal-content"></div></div>)');
    });

    function CallUserResult(params) {
        $.ajax({
            url: '/Universe/UserResult',
            contentType: "application/x-www-form-urlencoded",
            type: "GET",
            datatype: "json",
            data: params,
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                $('#exportResultModal').html(data);
                SaveResultEvents();
                CallSaveResult();
            }
        });
    }

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
                url: '/Universe/SaveUserResult',
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
    
});