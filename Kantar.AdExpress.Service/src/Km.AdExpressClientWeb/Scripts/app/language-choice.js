if (typeof jQuery === "undefined") { throw new Error("Language selector requires jQuery") }

$('.navbar-nav > li > .dropdown-menu > li  a').on('click', function (event, returnUrl) {
    event.preventDefault();
    var languageId = $(this).attr("data-language-id");
    var urlAction = $(this).attr("data-url-action");
    //var th = $(this).addClass("active");
   // var selText = $(this).text();
   // var par = $(this).parents().find('#language-choice').html(selText + '<span class="caret"></span>');
    var currentUrl=$('#current-url').val();

    var params = {
        returnUrl: currentUrl,//'/Account/Login',
        siteLanguage: languageId
    };

    $.ajax({
        url: urlAction, //'/Account/ChangeLanguage',
        contentType: 'application/json',
        type: 'POST',
        datatype: 'JSON',
        data: JSON.stringify(params),
        error: function (xmlHttpRequest, errorText, thrownError) {

        },
        success: function (data) {
            if (data != null) {
                document.location = data.RedirectUrl;
            }
        }
    });
});

$('#headerlanguagecontainer > #headerlanguage > a').on('click', function (event, returnUrl) {
    event.preventDefault();
    var languageId = $(this).attr("data-language-id");
    var urlAction = $(this).attr("data-url-action");
    $('#headerlanguagecontainer > #headerlanguage > a').each(function () {
        $(this).removeClass('active');
    });
    $(this).addClass('active');
    // var selText = $(this).text();
    // var par = $(this).parents().find('#language-choice').html(selText + '<span class="caret"></span>');
    var currentUrl = $('#current-url').val();

    var params = {
        returnUrl: currentUrl,//'/Account/Login',
        siteLanguage: languageId
    };

    $.ajax({
        url: urlAction, //'/Account/ChangeLanguage',
        contentType: 'application/json',
        type: 'POST',
        datatype: 'JSON',
        data: JSON.stringify(params),
        error: function (xmlHttpRequest, errorText, thrownError) {          
        },
        success: function (data) {
            if (data != null) {
                document.location = data.RedirectUrl;
            }
        }
    });
});