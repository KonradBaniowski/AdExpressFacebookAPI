$(function () {

    //VALIDER TODO

    //FIL D ARRIANE
    $('#Market').on('click', function (e) {
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
        var dis = this;
        var nextUrl = $(this).attr('href').split('/').pop();
        NextStep(nextUrl, dis)
    });

    function NextStep(nextUrl, dis) {
        var msg = validate();
        if (msg) {
            bootbox.alert(msg);
            return;
        }
        var things = [];
        var spinner = new Spinner().spin(dis);
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
            url: '/MediaSchedule/SaveMarketSelection',
            type: 'POST',
            data: params,
            error: function (data) {
                spinner.stop();
                bootbox.alert(data.ErrorMessage);
            },
            success: function (data) {
                spinner.stop();
                if (data.ErrorMessage != null && data.ErrorMessage != "") {
                    bootbox.alert(data.ErrorMessage);
                }
                if (data.RedirectUrl != null && data.RedirectUrl != "") {
                    document.location = data.RedirectUrl;
                }
            }
        });
    }

    function validate() {
        var message = "";
        var nbElemInclus = $("[id^='tree'][data-access-type='1'] li[data-id]").length;
        if (nbElemInclus < 1) {
            message = $('#Labels_ErrorMininumInclude').val();
        }
        return message;
    }

    //Déplacer un élement marché
    $('#move-item').on('click', function () {
        var levelSrc = $('.panel-marche .ui-selectee.ui-selected');
        var tabSelected = $('ul > li[class="active"] > a');
        var indexTab = tabSelected.attr('data-tab');
        var idTab = "tab-" + indexTab;
        if (levelSrc.length >= 1) {
            var universSrc = $('.ui-selectee.ui-selected').closest('.panel-default').attr('data-universe');
            if (universSrc == 14) {
                var isLock = $('#' + idTab).attr('lock');
                if (isLock == undefined || isLock == false) {
                    var universDst = $('.panel-body[data-tree=' + indexTab + '][data-level=' + universSrc + '] > ul');
                    var levelAllDst = $('.tab-content li');
                    if (levelAllDst.length < 200) {
                        $('#collapse-' + universSrc + '-' + indexTab).collapse('show');
                        $.each(levelSrc, function (index, value) {
                            var item = $(value).clone();
                            var find = false;
                            $.each(levelAllDst, function (index, value) {

                                if (item.val() == $(value).val()) {
                                    find = true;
                                    //bootbox.alert('deja mis qq part');
                                }
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
                    else {
                        console.log('DEPASEE LA LIMIT');
                    }
                }
                else {
                    bootbox.alert('lock');
                }
            }
            else {
                bootbox.alert($('#Labels_ErrorNoSupport').val());
            }
        }
        else {
            console.log('rien selectione');
        }
    });
});
