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

   


    

});
