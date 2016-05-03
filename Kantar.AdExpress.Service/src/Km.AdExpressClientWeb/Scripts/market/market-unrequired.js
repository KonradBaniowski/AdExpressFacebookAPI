$(function () {
    //Déplacer un élement marché 
    $('#move-item').on('click', function () {
        var levelSrc = $('.panel-marche .ui-selectee.ui-selected');
        if (levelSrc.length >= 1) {
            var universSrc = $('.ui-selectee.ui-selected').closest('.panel-default').attr('data-universe');
            var tabSelected = $('ul > li[class="active"] > a').attr('data-tab');
            var universDst = $('.panel-body[data-tree=' + tabSelected + '][data-level=' + universSrc + '] > ul');
            var levelDst = $('.panel-body[data-tree=' + tabSelected + '][data-level=' + universSrc + '] > ul > li')
            var nbItemSrc = levelSrc.length;
            var nbItemDst = levelDst.length;
            if (nbItemDst + nbItemSrc > 100) {
                bootbox.alert($('#Labels_ErrorMessageLimitUniverses').val());
                return
            }
            console.log(levelDst);
            $('#collapse-' + universSrc + '-' + tabSelected).collapse('show');
            $('#heading-' + universSrc + '-' + tabSelected).find('.panel-title').addClass('orange');
            $.each(levelSrc, function (index, value) {
                var item = $(value).clone();
                var find = false;
                $.each(levelDst, function (index, value) {
                    if (item.val() == $(value).val())
                        find = true;
                });
                if (!find) {
                    var buttonSupp = $('<button/>');
                    buttonSupp.addClass('pull-right');
                    var icon = $('<i/>');
                    icon.addClass('fa fa-times-circle black text-base');
                    buttonSupp.append(icon);
                    item.append(buttonSupp);
                    universDst.append(item);
                }
            });
        }
    });

    //VALIDER 
    $('#btnSubmitMarketSelection').on('click', function (e) {
        e.preventDefault();
        var dis = this;
        var nextUrl = $(this).attr('href').split('/').pop();
        NextStep(nextUrl, dis)
    });

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
        var items = $(this).parent().parent().find('.btn.btn-warning.btn-circle.btn-empty');
        $.each(items, function (index, value) {
            var page = $(value).attr('id');
            if (page == "Dates" || page=="Media") {
                gotoResult = false;
            }
        });
        if (gotoResult) {
            var dis = this;
            var nextUrl = $(this).attr('href').split('/').pop();
            NextStep(nextUrl, dis)
        }
        else {
            bootbox.alert("Missing data");
        }
    });

    function NextStep(nextUrl, dis) {
        var things = [];
        $('#btnSubmitMarketSelection').off('click');
        var trees = [];
        $.each($('.nav.nav-tabs > li a'), function (index, elem) {
            var itemContainer = $(elem).attr('data-target');
            var accessType = $(itemContainer + ' .panel-group').attr('data-access-type');
            var UniversLvl = [];
            $.each($(itemContainer + ' .panel-group .panel-body'), function (index, elem) {
                var idLevel = $(elem).attr('data-level');
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
        var ctrl = $('#Labels_CurrentController').val();
        $.ajax({
            url: '/' + ctrl + '/SaveMarketSelection',
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
});



