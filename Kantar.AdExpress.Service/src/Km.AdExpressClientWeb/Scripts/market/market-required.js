
$(function () {
    //FIL D ARRIANE
    $('#Media').on('click', function (e) {
        e.preventDefault();
        var dis = this;
        var nextUrl = $(this).attr('href').split('/').pop();
        NextStep(nextUrl, dis)
    });

    $('#Dates').on('click', function (e) {
        e.preventDefault();
        var dis = this;
        var nextUrl = $(this).attr('href').split('/').pop();
        NextStep(nextUrl, dis)
    });

    $('#Results').on('click', function (e) {
         e.preventDefault();
         var gotoResult = true;
         strHtml = "";
        var items = $(this).parent().parent().find('.btn.btn-warning.btn-circle.btn-empty');
        $.each(items, function (index, value) {
            var page = $(value).attr('id');
            if (page == "Dates" || page == "Media") {
                strHtml += "<li>" + page + "</li>";
                gotoResult = false;
            }
        });
        if (gotoResult) {
            var dis = this;
            var nextUrl = $(this).attr('href').split('/').pop();
            NextStep(nextUrl, dis)
        }
        else {
            strHtml = "Veuillez compléter le(s) paramètre(s) suivant(s) : <ul>" + strHtml + "</ul>";
            bootbox.alert(strHtml);
        }
    });

    function NextStep(nextUrl, dis)
    {
        var msg = validate();
        if (msg) {
            bootbox.alert(msg);
            return;
        }
        var things = [];
        $('#btnSubmitMarketSelection').off('click');
        var trees = [];
        $.each($('.nav.nav-tabs > li a'), function (index, elem) {
            var itemContainer = $(elem).attr('data-target');
            var accessType = $(itemContainer + ' .panel-group').attr('data-access-type');
            var UniversLvl = [];
            $.each($(itemContainer + ' .panel-group .panel-body'), function (index, elem) {
                var idLevel = $(elem).attr('data-level');
                console.log(this);
                var UniLvl = [];
                $.each($(this).find('ul > li'), function (index, elem) {
                    var itemUniver = $(elem).attr('data-id');
                    var universItems = {
                        Id: itemUniver
                    }
                    UniLvl.push(universItems);
                });
                var UnisLvl = {
                    Id: idLevel,
                    UniversItems: UniLvl
                };
                UniversLvl.push(UnisLvl);
            });
            var stuff = {
                Id: itemContainer,
                AccessType: accessType,
                UniversLevels: UniversLvl
            };
            trees.push(stuff);
        });
        var params = {
            trees: trees,
            nextStep: nextUrl
        };
        
        $.ajax({
            url: '/Selection/SaveMarketSelection',
            type: 'POST',
            data: params,
            error: function (data) {
                bootbox.alert(data.ErrorMessage);
            },
            success: function (data) {
                if (data.ErrorMessage != null && data.ErrorMessage != "") {
                    bootbox.alert(data.ErrorMessage);
                }
                if (data.RedirectUrl != null && data.RedirectUrl != "") {
                    document.location = data.RedirectUrl;
                }
            }
        });
    }
    var moduleId = $('#CurrentModule').val();
    if (moduleId == 194 || moduleId == 195)
    {
     $("#branch1 > div").each(function () {
        var DIS = $(this);
        var universe = parseFloat($(this).attr('data-universe'))
        var univerLabel = $(this).attr('data-label') + "\{NB_ELEM\}";
        if (universe === 1) {
            $.ajax({
        url: '/Universe/GetCategoryItems',
        contentType: 'application/json',
        type: 'POST',
        datatype: 'JSON',
        error: function (xmlHttpRequest, errorText, thrownError) {
                    alert("error");
    },
        success: function (response) {
                    DIS.fillGroupSelectable(univerLabel, response.data, response.total, 'panel-heading', 'panel-body', universe, undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.');

                    $('#selectable' + universe).selectableScroll({
        stop: SelectedItems
    });

    }
    });
    }
    else {
            DIS.fillGroupSelectable(univerLabel, undefined, 0, 'panel-heading', 'panel-body', universe, undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.');
    }

    })
    }
});

function validate() {
    var message = "";
    var nbElemInclus = $("[id^='tree'][data-access-type='1'] li[data-id]").length;
    if (nbElemInclus < 1)
    {
        message = $('#Labels_ErrorMininumInclude').val();
    }
    return message;
}
