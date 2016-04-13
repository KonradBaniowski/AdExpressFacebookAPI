var idNewDirectory = "";
var idOldDirectory = "";
var universId = "";
$('.btnMoveResult').on('click', function () {
    $('#moveResult').modal('show');
    universId = $(this).attr("data-id");
    idOldDirectory = $(this).attr("data-directory-id");
});


$('.btnRenameResult').on('click', function () {
    $('#renameResult').modal('show');
    universId = $(this).attr("data-id");
});

$('#btnRenameUnivers').on('click', function () {
    var name = $('#universName').val();
    var params = {
        name: name,
        universId: universId
    };
    $.ajax({
        url: '/Universe/RenameUnivers',
        contentType: 'application/json',
        type: 'POST',
        datatype: 'JSON',
        data: JSON.stringify(params),
        error: function (xmlHttpRequest, errorText, thrownError) {
                    $('#renameResult').modal('hide');
                    bootbox.alert("An error occurred while processing your request.");
        },
        success: function (response) {
            $('#renameResult').modal('hide');
            //Reload the page
            $.ajax({
                url: '/Home/ReloadSession',
                type: 'POST',
                data: params,
                error: function (data) {
                    bootbox.alert(data);
                },
                success: function (data) {
                    $("#Result").html(data);
                }
            });
                    bootbox.alert("Success");
        }
    });    
});


$('#btnMoveUnivers').on('click', function () {
    var idNewDirectory = $('.input-group-btn').find('.btn.btn-default.form-control.bg-black-only').attr("data-result");
    var params = {
        idOldDirectory: idOldDirectory,
        idNewDirectory: idNewDirectory,
        id: universId
    };
    $.ajax({
        url: '/Universe/MoveSession',
        contentType: 'application/json',
        type: 'POST',
        datatype: 'JSON',
        data: JSON.stringify(params),
        error: function (xmlHttpRequest, errorText, thrownError) {
                    $('#moveResult').modal('hide');
                    bootbox.alert("An error occurred while processing your request.");            
        },
        success: function (response) {
                    $('#moveResult').modal('hide');                   
            //Reload the page
                    $.ajax({
                        url: '/Home/ReloadSession',
                        type: 'POST',
                        data: params,
                        error: function (data) {
                            bootbox.alert(data);
                        },
                        success: function (data) {
                            $("#Result").html(data);
                        }
                    });
                    bootbox.alert("Success");
        }
    });
});

$(".dropdown-menu.bg-blue.form-control li > a").on('click', function (e) {
    e.preventDefault();
    var selText = $(this).text();
    var selValue = $(this).attr("data-id");
    $("#default").show();
    $("#default").removeAttr("id");
    $(this).attr("id", "default");
    $("#default").hide();
    $(this).parents('.input-group-btn').find('.btn.btn-default.form-control.bg-black-only').html(selText + '<span class="caret"></span>');
    $(this).parents('.input-group-btn').find('.btn.btn-default.form-control.bg-black-only').attr("data-result", selValue);

});