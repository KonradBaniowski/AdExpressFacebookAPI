
    var idNewDirectory = "";
    var idOldDirectory = "";
    var universId = "";

//opening the move session's modal
    $(document).on('click','.btnMoveResult', function (){
        $('#moveResult').modal('show');
        universId = $(this).attr("data-id");
        idOldDirectory = $(this).attr("data-directory-id");
    });

//opening the rename session's modal
    $(document).on('click','.btnRenameResult', function (){
        $('#renameResult').modal('show');
        universId = $(this).attr("data-id");
    });

//saving the session's new name
    $(document).on('click','#btnRenameResult', function (){
        var name = $('#newResultName').val();
        var params = {
            name: name,
            universId: universId
        };
        $.ajax({
            url: '/Universe/RenameSession',
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
                if (response == "Success") {
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
                }
                bootbox.alert(response);
            }
        });    
    });

//saving the session under the new diretory
    $(document).on('click','#btnMoveResult', function (){
        var idNewDirectory = $('.input-group-btn').find('.btn.btn-default.form-control.bg-black-only.idResult').attr("data-result");
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


// Managing sessions' and unverses' ddl
    $(document).on('click','.dropdown-menu.bg-blue.form-control li > a', function (e){
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


    //opening the move univers' modal
    $(document).on('click','.btnMoveUnivers', function ()
    { 
        $('#moveUnivers').modal('show');
        universId = $(this).attr("data-id");
        idOldDirectory = $(this).attr("data-directory-id");
    });

    //opening the save univers' modal
    $(document).on('click','.btnRenameUnivers', function ()
    {
        $('#renameUnivers').modal('show');
        universId = $(this).attr("data-id");
    });

//saving the univers' new name
    $(document).on('click','#btnRenameUnivers', function (){
        var name = $('#newUniversName').val();
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
                $('#renameUnivers').modal('hide');
                if (response == "Success") {
                    //Reload the page
                    $.ajax({
                        url: '/Home/ReloadUnivers',
                        type: 'POST',
                        data: params,
                        error: function (data) {
                            bootbox.alert(data);
                        },
                        success: function (data) {
                            $("#Univers").html(data);
                        }
                    });                
                }
                bootbox.alert(response);

            }
        });
    });

//saving the univers under the new diretory
    $(document).on('click','#btnMoveUnivers', function (){
        var idNewDirectory = $('.input-group-btn').find('.btn.btn-default.form-control.bg-black-only.idUnivers').attr("data-result");
        var params = {
            idOldDirectory: idOldDirectory,
            idNewDirectory: idNewDirectory,
            id: universId
        };
        $.ajax({
            url: '/Universe/MoveUnivers',
            contentType: 'application/json',
            type: 'POST',
            datatype: 'JSON',
            data: JSON.stringify(params),
            error: function (xmlHttpRequest, errorText, thrownError) {
                $('#moveUnivers').modal('hide');
                bootbox.alert("An error occurred while processing your request.");
            },
            success: function (response) {
                $('#moveUnivers').modal('hide');
                //Reload the page
                $.ajax({
                    url: '/Home/ReloadUnivers',
                    type: 'POST',
                    data: params,
                    error: function (data) {
                        bootbox.alert(data);
                    },
                    success: function (data) {
                        $("#Univers").html(data);
                    }
                });
                bootbox.alert("Success");
            }
        });
    });
