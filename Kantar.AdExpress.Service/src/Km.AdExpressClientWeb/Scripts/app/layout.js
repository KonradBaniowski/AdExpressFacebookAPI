
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
    //$('#loader .modal-footer').dequeue();
    $('#loader .modal-footer').hide();
    $('#loader').modal('hide');
});

$(document).ready(function () {
    var options = {
        useEasing: true,
        useGrouping: true,
        separator: ',',
        decimal: '.',
        prefix: '',
        suffix: ''
    };

    $.each($('.animationFun'), function (index, value) {
        var number = $(value).attr('data-value');
        var demo = new CountUp("#fun"+index, 0, number, 0, 3, options);
        demo.start();
    });
    //var demo = new CountUp("#animationFun", 0, 2854, 0, 5, options);
   

});

$('#loader .modal-footer button').on('click', function () {
    $(this).hide();
});

