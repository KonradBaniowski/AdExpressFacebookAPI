
$(document).ajaxStart(function () {
    $('#loader').modal({
        keyboard: false,
        backdrop: 'static'
    });
    $('#loader').modal('show');
    $('#loader .modal-footer').delay(5000).fadeOut(200, function () {
        $(this).show();
    });
});

$(document).ajaxStop(function () {
    $('#loader .modal-footer').dequeue();
    $('#loader .modal-footer').hide();
    $('#loader').modal('hide');
});

$('#loader .modal-footer button').on('click', function () {
    $(this).hide();
});
