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
    if(id=="20")
    {
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
        date:date
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
            bootbox.alert(response);
        }
    });
});