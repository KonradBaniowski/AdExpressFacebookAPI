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

$('#datetimepicker').datetimepicker({
    inline: true,
    locale: 'fr',
    format: 'DD/MM/YYYY',
    useCurrent: false,
    //minDate: minDate,
    //maxDate: maxDate
}).on('dp.change', function (e) {
    $('#end-date-value').val($('#datetimepicker-end').data('date'));
});