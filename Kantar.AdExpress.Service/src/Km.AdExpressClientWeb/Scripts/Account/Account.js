$('#btnSubmit').on('click', function (e) {
    var login = $('#login').val();
    var pwd = $('#pwd').val()
    if (login == "" || pwd == "") {
        bootbox.alert($('#ErrorMessage').val());
        return;
    }
    //else {
    //    var model ={
    //        Email:login,
    //        Password:pwd
    //    };
    //    var params = {
    //        model: model,
    //        returnUrl:""
    //    };
    //    $.ajax({
    //        url: '/Account/Login',
    //        type: 'POST',
    //        data: params,
    //        error: function (data) {
    //            bootbox.alert(data.ErrorMessage);
    //        },
    //        success: function (data) {
    //            if (data.RedirectUrl != null && data.RedirectUrl != "") {
    //                document.location = data.RedirectUrl;
    //            }
    //        }
    //    });
    //}
});