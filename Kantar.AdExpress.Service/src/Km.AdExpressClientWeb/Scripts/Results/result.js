var sortOrder = "NONE";
var columnIndex = 1;

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

$(document).on('click', '#weekly', function () {
    $('#weeklyCalendar').datetimepicker({
        format: 'dd'
    });
});

$(document).on('click', '#monthly', function () {
    $('#monthlyCalendar').datetimepicker({
        format: 'DD'
    });
});

$(document).on('change', '#ddlPeriodicity', function (event) {
    event.preventDefault();
    var id = $('#ddlPeriodicity').val();
    if (id == "10") {
        $('#monthly').hide();
        $('#weekly').hide();
    }
    if (id == "20") {
        $('#monthly').hide();
        $('#weekly').show();
    }
    if (id == "30") {
        $('#monthly').show();
        $('#weekly').hide();
    }
});

$(document).on('click', '#btnSaveAlert', function (event) {
    event.preventDefault();
    var type = $('#ddlPeriodicity').val();
    var date;
    if (type == "20") {
        date = $('#weeklyOccurrenceValue').val();
    }
    if (type == "30") {
        date = $('#montlyOccurrenceValue').val();
    }
    var params = {
        title: $('#alertTitle').val(),
        email: $('#email').val(),
        type: type,
        date: date
    };
    $.ajax({
        url: '/Alert/SaveAlert',
        contentType: 'application/json',
        type: 'POST',
        datatype: 'JSON',
        data: JSON.stringify(params),
        error: function (xmlHttpRequest, errorText, thrownError) {
            bootbox.alert("An error occurred while processing your request.");
        },
        success: function (response) {
            $('#alertModal').modal('hide');
            bootbox.alert(response.Message);
        }
    });
});

////TODO: Temporaire pour MediaSchedule (à modifier dans BLL)
//$(document).on('click', '*[id*=grid_table_PERIOD]', function (event) {
//    sortFuncMediaSchedule(2);
//});
//$(document).on('click', '*[id*=grid_table_PDM]', function (event) {
//    sortFuncMediaSchedule(3);
//});

//var sortFuncMediaSchedule = function (index) {
//    columnIndex = index;
//    if (sortOrder == "NONE")
//        sortOrder = "ASC";
//    else if (sortOrder == "ASC")
//        sortOrder = "DESC";
//    else if (sortOrder == "DESC")
//        sortOrder = "ASC";
//}
///***********************************************************/

$(document).on('click', '*[id*=grid_table_g]', function (event) {
    var element = $(this);
    sortFunc(element);
});

var sortFunc = function (field) {
    var index = field[0].id.split("-")[0].split("_g")[1];
    testIndex = parseInt(index);
    if (!isNaN(testIndex)) {
        columnIndex = testIndex;
        if (sortOrder == "NONE")
            sortOrder = "ASC";
        else if (sortOrder == "ASC")
            sortOrder = "DESC";
        else if (sortOrder == "DESC")
            sortOrder = "ASC";
    }
}

$(document).on('click', '#btn-export', function (event) {
    event.preventDefault();
    var selectedValue = $('#export-type').val();
    var paramsExport = "?sortOrder=NONE&columnIndex=1";
    if (sortOrder != null && columnIndex != null)
        paramsExport = "?sortOrder=" + sortOrder + "&columnIndex=" + columnIndex;

    var params = {
        exportType: selectedValue
    };
    var controller = $('#ExportController').val();
    switch (selectedValue) {
        case "1":
            window.open('/' + controller + '/Index' + paramsExport, "_blank");
            break;
        case "2":
            window.open('/' + controller + '/ResultValue', "_blank");
            break;
        case "3":
            window.open('/' + controller + '/ResultBrut', "_blank");
            break;
        case "4":
        case "5":
            $.ajax({
                url: '/ExportResult/CreateExport',
                contentType: 'application/json',
                type: 'POST',
                datatype: 'JSON',
                data: JSON.stringify(params),
                error: function (xmlHttpRequest, errorText, thrownError) {
                    bootbox.alert("An error occurred while processing your request.");
                },
                success: function (response) {
                    $('#exportResultModal .modal-content').html('');
                    $('#exportResultModal .modal-content').append(response);
                    $('#exportResultModal').modal('show');
                    var cookies = document.cookie.split(";");
                    for (var i = 0; i < cookies.length; i++) {
                        var eachCookie = cookies[i].split("=");
                        var cookieName = eachCookie[0];
                        if ( cookieName.indexOf("mail") > -1) {
                            var cookieValue = eachCookie[1];
                            $('#email').val(cookieValue);
                        }
                    }
                }
            });
            break;
        default:
            window.open('@Url.Action("Index", "TestExport")', "_blank");
            break;
    }
});

$(document).on('click', '#btnExport', function (event) {
    event.preventDefault();
    var fileName = $('#fileName').val();
    var email = $('#email').val();
    var remember = $('#rememberEmail').is(':checked');
    if (remember) {
        var date = new Date();
        date.setTime(date.getTime() + (1 * 24 * 60 * 60 * 1000));   // add 1 day
        document.cookie = "mail=" + email + "; expires=" + date.toGMTString();
        console.log(document.cookie);
    }
    var type = $('#ExportType').val();
    var params = {
        fileName: fileName,
        email: email,
        type: type
    };
    $.ajax({
        url: '/ExportResult/Export',
        contentType: 'application/json',
        type: 'POST',
        datatype: 'JSON',
        data: JSON.stringify(params),
        error: function (xmlHttpRequest, errorText, thrownError) {
            bootbox.alert("An error occurred while processing your request.");
        },
        success: function (response) {
            $('#alertExportModal').modal('hide');
            bootbox.alert(response.Message);
        }
    });
});