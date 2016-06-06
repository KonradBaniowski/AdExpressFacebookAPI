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

$(document).on('click', '#btn-export', function (event) {
    event.preventDefault();
    var selectedValue = $('#export-type').val();
    var params = {
        exportType: selectedValue
    };
    var controller = $('#ExportController').val();
    switch (selectedValue) {
        case "1":
            window.open('/' + controller + '/Index', "_blank");
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