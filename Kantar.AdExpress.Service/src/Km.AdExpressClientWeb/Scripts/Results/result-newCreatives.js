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

    function GetItems(idItems) {
        var items = '';

        if (idItems != null) {
            $.each(idItems, function (index, value) {
                items += value + ",";
            });
            return items.substring(0, items.length - 1);
        }
        return items;
    }

    function GenericDetailLevelFilter() {
        this.DefaultDetailValue = $('#defaultDetail').val();
        this.CustomDetailValue = -1;
        this.L1DetailValue = -1;
        this.L2DetailValue = -1;
        this.L3DetailValue = -1;
    }

    function PeriodDetailFilter() {
        this.PeriodDetailType = $('#periodDetailType').val();
    }

    function FormatFilter() {
        this.Formats = GetItems($('#format').val());
    }

    function UserFilter() {
        this.GenericDetailLevelFilter = new GenericDetailLevelFilter();
        this.PeriodDetailFilter = new PeriodDetailFilter();
        this.FormatFilter = new FormatFilter();
    }

    var userFilter = new UserFilter();


    function CallSetOptions() {
        $.ajax({
            url: '/NewCreatives/SetResultOptions',
            contentType: "application/x-www-form-urlencoded",
            type: "POST",
            datatype: "json",
            data: userFilter,
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                sortOrder = "NONE";
                columnIndex = 1;
                CallNewCreativesResult();
            }
        });
    }

    CallNewCreativesResult();

    $('#export-type').removeClass("hide");
    $('#export-type').selectpicker();
    $('#btn-export').on('click', function (e) {
        var selectedValue = $('#export-type').val();
        var params = "?sortOrder=" + sortOrder + "&columnIndex=" + columnIndex;
        switch (selectedValue) {
            case "1":
                window.open('/NewCreativesExport/Index' + params, "_blank");
                break;
            case "2":
                window.open('/NewCreativesExport/ResultBrut', "_blank");
                break;
            default:
                window.open('/NewCreativesExport/Index' + params, "_blank");
                break;
        }
    });

    $('#periodDetailType').selectpicker();

    $('#periodDetailType').on('change', function (e) {
        userFilter.PeriodDetailFilter.PeriodDetailType = $('#periodDetailType').val();
    });

    $('#save-custom-detail-levels').on('click', function (e) {
        var levels = $('#l1Detail').val() + ',' + $('#l2Detail').val() + ',' + $('#l3Detail').val();
        var type = 1;

        var params = {
            levels: levels,
            type: type
        };
        $.ajax({
            url: '/NewCreatives/SaveCustomDetailLevels',
            contentType: "application/x-www-form-urlencoded",
            type: "POST",
            datatype: "json",
            data: params,
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                if (data != null && data != "") {
                    $("#customDetail")
                    .append('<option value="' + data.Id + '">' + data.Label + '</option>')
                    .selectpicker('refresh');
                    bootbox.alert(data.Message);
                }
            }
        });
    });

    $('#remove-custom-detail-levels').on('click', function (e) {
        var detailLevel = $('#customDetail').val();

        var params = {
            detailLevel: detailLevel
        };
        $.ajax({
            url: '/NewCreatives/RemoveCustomDetailLevels', 
            contentType: "application/x-www-form-urlencoded",
            type: "POST",
            datatype: "json",
            data: params,
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                if (data != null && data != "") {
                    if (!(data.Message.indexOf("Impossible") > -1)) {
                        $('#customDetail').find('[value=' + detailLevel + ']').remove();
                        $('#customDetail').selectpicker('refresh');
                    }
                    bootbox.alert(data.Message);
                }
            }
        });
    });

    $('#format').selectpicker();

    $('#format').on('change', function (e) {
        userFilter.FormatFilter.Formats = GetItems($('#format').val());
    });

    $('#defaultDetail').selectpicker();
    $('#customDetail').selectpicker();
    $('#l1Detail').selectpicker();
    $('#l2Detail').selectpicker();
    $('#l3Detail').selectpicker();
    $('#l4Detail').selectpicker();

    $('#defaultDetail').on('change', function (e) {
        $('#customDetail').selectpicker('val', '-1');
        $('#l1Detail').selectpicker('val', '-1');
        $('#l2Detail').selectpicker('val', '-1');
        $('#l3Detail').selectpicker('val', '-1');
        $('#l4Detail').selectpicker('val', '-1');
        InitGenericDetailLevelFilter();
        userFilter.GenericDetailLevelFilter.DefaultDetailValue = $('#defaultDetail').val();
    });

    $('#customDetail').on('change', function (e) {
        $('#defaultDetail').selectpicker('val', '-1');
        $('#l1Detail').selectpicker('val', '-1');
        $('#l2Detail').selectpicker('val', '-1');
        $('#l3Detail').selectpicker('val', '-1');
        $('#l4Detail').selectpicker('val', '-1');
        InitGenericDetailLevelFilter();
        userFilter.GenericDetailLevelFilter.CustomDetailValue = $('#customDetail').val();
    });

    function ResetLnLevel(lx, ly) {
        if ($(lx).val() == $(ly).val()) {
            $(ly).selectpicker('val', '-1');
            switch (ly) {
                case "#l1Detail":
                    userFilter.GenericDetailLevelFilter.L1DetailValue = -1;
                    break;
                case "#l2Detail":
                    userFilter.GenericDetailLevelFilter.L2DetailValue = -1;
                    break;
                case "#l3Detail":
                    userFilter.GenericDetailLevelFilter.L3DetailValue = -1;
                    break;
                case "#l4Detail":
                    userFilter.GenericDetailLevelFilter.L4DetailValue = -1;
                    break;
            }
        }
    }

    $('#l1Detail').on('change', function (e) {
        $('#customDetail').selectpicker('val', '-1');
        $('#defaultDetail').selectpicker('val', '-1');
        userFilter.GenericDetailLevelFilter.DefaultDetailValue = -1;
        userFilter.GenericDetailLevelFilter.CustomDetailValue = -1;
        userFilter.GenericDetailLevelFilter.L1DetailValue = $('#l1Detail').val();
        ResetLnLevel("#l1Detail", "#l2Detail");
        ResetLnLevel("#l1Detail", "#l3Detail");
        ResetLnLevel("#l1Detail", "#l4Detail");
    });

    $('#l2Detail').on('change', function (e) {
        $('#customDetail').selectpicker('val', '-1');
        $('#defaultDetail').selectpicker('val', '-1');
        userFilter.GenericDetailLevelFilter.DefaultDetailValue = -1;
        userFilter.GenericDetailLevelFilter.CustomDetailValue = -1;
        userFilter.GenericDetailLevelFilter.L2DetailValue = $('#l2Detail').val();
        ResetLnLevel("#l2Detail", "#l1Detail");
        ResetLnLevel("#l2Detail", "#l3Detail");
        ResetLnLevel("#l2Detail", "#l4Detail");
    });

    $('#l3Detail').on('change', function (e) {
        $('#customDetail').selectpicker('val', '-1');
        $('#defaultDetail').selectpicker('val', '-1');
        userFilter.GenericDetailLevelFilter.DefaultDetailValue = -1;
        userFilter.GenericDetailLevelFilter.CustomDetailValue = -1;
        userFilter.GenericDetailLevelFilter.L3DetailValue = $('#l3Detail').val();
        ResetLnLevel("#l3Detail", "#l1Detail");
        ResetLnLevel("#l3Detail", "#l2Detail");
        ResetLnLevel("#l3Detail", "#l4Detail");
    });

    $('#l4Detail').on('change', function (e) {
        $('#customDetail').selectpicker('val', '-1');
        $('#defaultDetail').selectpicker('val', '-1');
        userFilter.GenericDetailLevelFilter.DefaultDetailValue = -1;
        userFilter.GenericDetailLevelFilter.CustomDetailValue = -1;
        userFilter.GenericDetailLevelFilter.L4DetailValue = $('#l4Detail').val();
        ResetLnLevel("#l4Detail", "#l1Detail");
        ResetLnLevel("#l4Detail", "#l2Detail");
        ResetLnLevel("#l4Detail", "#l3Detail");
    });

    function InitGenericDetailLevelFilter() {
        userFilter.GenericDetailLevelFilter.DefaultDetailValue = -1;
        userFilter.GenericDetailLevelFilter.CustomDetailValue = -1;
        userFilter.GenericDetailLevelFilter.L1DetailValue = -1;
        userFilter.GenericDetailLevelFilter.L2DetailValue = -1;
        userFilter.GenericDetailLevelFilter.L3DetailValue = -1;
        userFilter.GenericDetailLevelFilter.L4DetailValue = -1;
    }

    $('#validate-options').on('click', function (e) {
        $("#grid").addClass("hide");
        $("#gridLoader").removeClass("hide");
        $("#collapseContainerOne").collapse('hide');
        CallSetOptions();
    });

    function CallNewCreativesResult() {
        $("#gridEmpty").hide();

        $.ajax({
            url: '/NewCreatives/NewCreativesResult',
            contentType: "application/x-www-form-urlencoded",
            type: "POST",
            datatype: "json",
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
                    },
                    {
                        name: "Sorting",
                        type: "local",
                        applySortedColumnCss: false
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

                        element.addClass("ui-iggrid-colheaderasc-ktr");
                        if (sortOrder == "ASC")
                            element.append('<div class="ui-iggrid-indicatorcontainer"><span class="ui-iggrid-colindicator ui-iggrid-colindicator-asc ui-icon ui-icon-arrowthick-1-n"></span></div>');
                        else
                            element.append('<div class="ui-iggrid-indicatorcontainer"><span class="ui-iggrid-colindicator ui-iggrid-colindicator-desc ui-icon ui-icon-arrowthick-1-s"></span></div>');
                    }
                }

            });
        
        } else {
            bootbox.alert(error);
        }
    }

    $(document).on('click', '*[id*=grid_table_g]', function (event) {
        var element = $(this);
        sortFunc(element);
    });

    var sortFunc = function (field) {
        var index = field[0].id.split("-")[0].split("_g")[1];
        if (sortOrder == "NONE")
            sortOrder = "ASC";
        else if (sortOrder == "ASC")
            sortOrder = "DESC";
        else if (sortOrder == "DESC")
            sortOrder = "ASC";
        columnIndex = parseInt(index);
    }

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