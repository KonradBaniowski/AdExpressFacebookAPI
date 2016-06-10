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
                CallAnalysisResult();
            }
        });
    }

    CallAnalysisResult();
    $('#export-type').removeClass("hide");
    $('#export-type').selectpicker();
    $('#btn-export').on('click', function (e) {
        var selectedValue = $('#export-type').val();
        switch (selectedValue) {
            case "1":
                window.open('/ProductClassAnalysisExport/Index', "_blank");
                break;
            case "2":
                window.open('/ProductClassAnalysisExport/ResultBrut', "_blank");
                break;
            default:
                window.open('/ProductClassAnalysisExport/Index', "_blank");
                break;
        }
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

    $('#validate-options').on('click', function (e) {
        $("#grid").addClass("hide");
        $("#gridLoader").removeClass("hide");
        $("#collapseContainerOne").collapse('hide');
        CallSetOptions();
    });
    var sortOrder = "NONE";
    var columnIndex = 1;
    $(document).on('click', '.ui-iggrid-header.ui-widget-header', function (event) {
        var element = $(this);
        sortFunc(element);
    });
    function UnitFormatter(val) {
        if (val > 0)
            return $.ig.formatter(val, "number");

        return "";
    }
    

    //Infragistic
    function PercentFormatter(val) {
        if (val > 0)
            return $.ig.formatter(val, "number", "percent");

        return "";
    }
    function PageFormatter(val) {
        if (val > 0)
            return $.ig.formatter(val, "number", "#,##0.###");

        return "";
    }
    function EvolFormatter(val) {

        if (val == "+Infinity")
            return '+<img src="../Content/img/g.gif" />';
        else if (val == "-Infinity")
            return '-<img src="../Content/img/r.gif" />';

        if (val > 0)
            return $.ig.formatter(val, "number", "percent") + '<img src="../Content/img/g.gif" />';

        if (val < 0)
            return $.ig.formatter(val, "number", "percent") + '<img src="../Content/img/r.gif" />';

        if (val == 0)
            return '<img src="../Content/img/o.gif" />';

        return '';
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
            },
            success: function (data) {
                if (data != null && data != "") {
                    dataTreeGrid = data.datagrid;
                    cols = GetColumnsFormatter(data.columns, data.unit);
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
    function GetColumnsFormatter(columns, unit) {

        if (columns != null) {

            columns.forEach(function (elem) {
                if (elem.group != null && elem.group != 'undefined') {
                    for (var i = 0, len = elem.group.length; i < len; i++) {
                        if (elem.group[i].key.indexOf("unit") > -1) {
                            if (unit == "duration")
                                elem.group[i].formatter = DurationFormatter;
                            else if (unit == "pages")
                                elem.group[i].formatter = PageFormatter;
                            else
                                elem.group[i].formatter = UnitFormatter;
                        }
                        if (elem.group[i].key.indexOf("evol") > -1) {
                            elem.group[i].formatter = EvolFormatter;
                        }
                        if (elem.group[i].key.indexOf("pdm") > -1) {
                            elem.group[i].formatter = PercentFormatter;
                        }
                    }
                } else if (elem.key.indexOf("unit") > -1) {
                    if (unit == "duration")
                        elem.formatter = DurationFormatter;
                    else if (unit == "pages")
                        elem.formatter = PageFormatter;
                    else
                        elem.formatter = UnitFormatter;
                } else if (elem.key.indexOf("evol") > -1) {
                    elem.formatter = EvolFormatter;
                }
                else if (elem.key.indexOf("pdm") > -1) {
                    elem.formatter = PercentFormatter;
                }
            });

            return columns;
        }

        return columns;
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
                    //,{
                    //    name: "Sorting",
                    //    type: "remote",
                    //    columnSorted: sortFunc,
                    //    applySortedColumnCss: false
                    //}
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