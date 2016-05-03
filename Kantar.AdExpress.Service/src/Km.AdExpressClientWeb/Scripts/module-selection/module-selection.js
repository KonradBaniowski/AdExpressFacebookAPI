if (typeof jQuery === "undefined") { throw new Error("Module selector requires jQuery") }

$(function () {

    //$('a.tile-body.text-center.module-done').on('click', currentModule);
    $('a.module-done').on('click', currentModule);

    function currentModule(event) {
        event.preventDefault();
        var url = $(this).attr('href');

        var params = {
            idModule: $(this).attr('id'),
        };

        $.ajax({
            url: '/Home/CurrentModule',
            contentType: 'application/json',
            type: 'POST',
            datatype: 'JSON',
            data: JSON.stringify(params),
            error: function (xmlHttpRequest, errorText, thrownError) {

            },
            success: function () {
                document.location = url;
            }
        });
    }

    $('.navbar-nav > li > .dropdown-menu > li  a').on('click', function (event, returnUrl) {
        event.preventDefault();
        var languageId = $(this).attr("data-language-id");
        var th = $(this).addClass("active");
        var selText = $(this).text();
        var par = $(this).parents().find('#language-choice').html(selText + '<span class="caret"></span>');
       
        var params = {
            returnUrl: $('#current-url').val(),
            siteLanguage: languageId
        };

        $.ajax({
            url: '/Home/ChangeLanguage',
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


    

});
