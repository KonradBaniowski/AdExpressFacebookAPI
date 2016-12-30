﻿$(document).ready(function () {

    if (typeof jQuery === "undefined") { throw new Error("jQuery") }

    $("#grid").igTreeGrid({
        //dataSource: data,
        primaryKey: "ClassifId",
        width: "100%",
        defaultColumnWidth: 200,
        avgRowHeight: 60,
        autoGenerateColumns: true
    });

    var ds;
    var cols;
    var colsFixed;
    var columnsNotAllowedSorting;
    var needFixedColumns = false;
    var zoomDate = '';
    var previousSubPeriodLabel;
    var hasMSCreatives = false;
    var creatives = [];
    var gridWidth;

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
        this.L1DetailValue = $('#l1Detail').val();
        this.L2DetailValue = $('#l2Detail').val();
        this.L3DetailValue = $('#l3Detail').val();
        this.L4DetailValue = $('#l4Detail').val();
    }

    function UnitFilter() {
        this.Unit = $('#unit').val();
    }

    function PercentageFilter() {
        this.Percentage = $('#percentage').val();
    }

    function ResultTypeFilter() {
        this.ResultType = $('#resultType').val();
    }

    function UserFilter() {
        this.GenericDetailLevelFilter = new GenericDetailLevelFilter();
        this.UnitFilter = new UnitFilter();
        this.PercentageFilter = new PercentageFilter();
        this.ResultTypeFilter = new ResultTypeFilter();
    }

    var userFilter = new UserFilter();

    var renderGrid = function (success, error) {
        if (success) {

            $("#grid").igTreeGrid("destroy");
            $("#gridLoader").addClass("hide");
            $("#grid").removeClass("hide");
            if (needFixedColumns) {
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
                            applySortedColumnCss: false,
                            columnSettings: columnsNotAllowedSorting
                        }
                    ]
                })
            }
            else {
                $("#grid").igTreeGrid({
                    dataSource: ds.dataView(),
                    columns: cols,
                    height: "530px",
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
                        }
                           ,
                        {
                            name: "Sorting",
                            type: "local",
                            applySortedColumnCss: false,
                            columnSettings: columnsNotAllowedSorting
                        }
                    ]
                })
            }

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
            alert(error);
        }

    }

    function UnitFormatter(val) {
        if (val > 0)
            return $.ig.formatter(val, "number");

        return "";
    }

    function PercentFormatter(val) {
        
        if (val > 0)
            return $.ig.formatter(val, "percent");

        return "";
    }

    function PageFormatter(val) {
        if (val > 0)
            return $.ig.formatter(val, "number", "#,##0.###");

        return "";
    }

    function EvolFormatter(val) {
        //if (isNaN(val))
        //    return '';

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

    function GetColumnsFormatter(columns) {

        if (columns != null) {

            columns.forEach(function (elem) {
                if (elem.group != null && elem.group != 'undefined') {
                    for (var i = 0, len = elem.group.length; i < len; i++) {
                        if (elem.group[i].key.indexOf("unit") > -1) {
                            if (elem.group[i].key.indexOf("duration") > -1)
                                elem.group[i].formatter = DurationFormatter;
                            else if (elem.group[i].key.indexOf("pages") > -1)
                                elem.group[i].formatter = PageFormatter;
                            else
                                elem.group[i].formatter = UnitFormatter;
                        }
                        if (elem.group[i].key.indexOf("percent") > -1)
                            elem.group[i].formatter = PercentFormatter;
                        if (elem.group[i].key.indexOf("evol") > -1) {
                            elem.group[i].formatter = EvolFormatter;
                        }
                    }
                } else if (elem.key.indexOf("unit") > -1) {
                    if (elem.key.indexOf("duration") > -1)
                        elem.formatter = DurationFormatter;
                    else if (elem.key.indexOf("pages") > -1)
                        elem.formatter = PageFormatter;
                    else
                        elem.formatter = UnitFormatter;
                } 
                else if (elem.key.indexOf("percent") > -1)
                        elem.formatter = PercentFormatter;
                else if (elem.key.indexOf("evol") > -1) {
                    elem.formatter = EvolFormatter;
                }
            });

            return columns;
        }

        return columns;
      
    }

    $("#gadModal").on('shown.bs.modal', function (event) {
        var link = $(event.relatedTarget);// Button that triggered the modal
        var datas = link.data('gad').toString(); // Extract info from data-* attributes
        datas = datas.replace(/\[|\]/g, '');
        datas = datas.split(",");

        if (datas[0] === null || datas[0] == "" || datas[0] == 0 || datas[0] == "0") {
            alert("Les infos Gad ne sont pas disponibles.");
        }
        else {
            var params = {
                idAddress: datas[2],
                advertiser: datas[1]
            };
            CallGadInfos(params);
        }
    });

    function CallGadInfos(params) {
        $.ajax({
            url: '/Gad/GadInfos',
            contentType: "application/x-www-form-urlencoded",
            type: "GET",
            datatype: "json",
            data: params,
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                $('#gadModal').html(data);
                $("#btn-gad-detail").click(function (event) {
                    var link = $(event.target);// Button that triggered the modal
                    var datas = link.data('gad').toString(); // Extract info from data-* attributes

                    if (datas === null || datas == "" || datas == 0 || datas == "0") {
                        alert("Le lien vers Gad n'est pas disponible.");
                    }
                    else {
                        window.open(datas, "_blank");
                    }
                });
            }
        });
    }

    function CallProgramAnalysisResult() {

        $("#gridMessage").addClass("hide");

        $.ajax({
            url: 'ProgramAnalysisResult',
            contentType: "application/x-www-form-urlencoded",
            type: "POST",
            datatype: "json",
            timeout: 300000, //5 min
            error: function (xmlHttpRequest, errorText, thrownError) {
                var message;
                if (errorText == 'timeout') {
                    message = $('#Labels_Timeout').val() + '<br \>' + $('#Labels_TimeoutBis').val();
                    $("#gridLoader").addClass("hide");
                    $("#gridMessage").removeClass("hide");
                    $("#gridMessage").html(message);
                }
                else {
                    message = $('#Labels_ResultError').val() + '. ' + $('#Labels_WarningBackNavigator').val();
                    bootbox.alert(message);
                    $("#gridLoader").addClass("hide");
                }
            },
            success: function (data) {
                if (data != null && data != "") {
                    if (data.hasMoreThanMaxRowsAllowed) {
                        var message = $('#Labels_MaxAllowedRows').val() + '<br \>' + $('#Labels_MaxAllowedRowsBis').val();
                        $("#gridLoader").addClass("hide");
                        $("#gridMessage").removeClass("hide");
                        $("#gridMessage").html(message);
                    }
                    else {
                        dataTreeGrid = data.datagrid;
                        cols = GetColumnsFormatter(data.columns);
                        colsFixed = data.columnsfixed;
                        columnsNotAllowedSorting = data.columnsNotAllowedSorting;
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
                }
                else {
                    $("#gridLoader").addClass("hide");
                    $("#gridMessage").removeClass("hide");
                    $("#gridEmpty").show();
                }
            }
        });
    }

    function CallSetOptions() {
        $.ajax({
            url: 'SetResultOptions',
            contentType: "application/x-www-form-urlencoded",
            type: "POST",
            datatype: "json",
            data: userFilter,
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                CallProgramAnalysisResult();
            }
        });
    }

    $("#btn-save-result").click(function () {
        $("#resultModal").modal("show");
    });

    CallProgramAnalysisResult();

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

    $("#resultModal").on('shown.bs.modal', function (event) {
        var params = {
            id: 0
        };
        CallUserResult(params);
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

    $('#export-type').removeClass("hide");
    $('#export-type').selectpicker();

    var sortOrder = "NONE";
    var columnIndex = 1;

    $('#save-custom-detail-levels').on('click', function (e) {
        var levels = $('#l1Detail').val() + ',' + $('#l2Detail').val() + ',' + $('#l3Detail').val() + ',' + $('#l4Detail').val();
        var type = 1;

        var params = {
            levels: levels,
            type: type
        };
        $.ajax({
            url: 'SaveCustomDetailLevels',
            contentType: "application/x-www-form-urlencoded",
            type: "POST",
            datatype: "json",
            data: params,
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                if (data != null && data != "") {
                    if (data.Id > 0) {
                        $("#customDetail")
                        .append('<option value="' + data.Id + '">' + data.Label + '</option>')
                        .selectpicker('refresh');
                    }
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
            url: 'RemoveCustomDetailLevels',
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

    $('#validate-options').on('click', function (e) {
        $("#grid").addClass("hide");
        $("#gridLoader").removeClass("hide");
        $("#collapseContainerOne").collapse('hide');

        if ($("#initializeMedia").prop('checked') == true) {
            $("#initializeMediaContainer").hide();
        }

        CallSetOptions();

        if ($("#initializeMedia").prop('checked') == true) {
            $('#initializeMedia').attr('checked', false);
            userFilter.InitializeMedia = false;
        }
    });

    $('#unit').selectpicker();

    $('#unit').on('change', function (e) {
        userFilter.UnitFilter.Unit = $('#unit').val();
    });

    $('#percentage').selectpicker();

    $('#percentage').on('change', function (e) {
        userFilter.PercentageFilter.Percentage = $('#percentage').val();
    });

    $('#resultType').selectpicker();

    $('#resultType').on('change', function (e) {
        userFilter.ResultTypeFilter.ResultType = $('#resultType').val();
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

    Array.prototype.remove = function (value) {
        if (this.indexOf(value) !== -1) {
            this.splice(this.indexOf(value), 1);
            return true;
        } else {
            return false;
        };
    }

    if (!Array.indexOf) {
        Array.prototype.indexOf = function (obj) {
            for (var i = 0; i < this.length; i++) {
                if (this[i] == obj) {
                    return i;
                }
            }
            return -1;
        };
    }

});