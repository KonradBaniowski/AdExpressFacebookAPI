
$(document).ajaxStart(function () {
    $('#loader').modal({
        keyboard: false,
        backdrop: 'static'
    });
    console.log("Triggered ajaxStart handler.");
    $('#loader').modal('show');
});

$(document).ajaxStop(function () {
    $('#loader').modal('hide');
});