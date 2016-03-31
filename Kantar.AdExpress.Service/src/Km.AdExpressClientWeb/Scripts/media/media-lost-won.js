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

$('#move-item').on('click', function () {
    var levelSrc = $('.panel-marche .ui-selectee.ui-selected');
    if (levelSrc.length >= 1) {
        var universSrc = $('.ui-selectee.ui-selected').closest('.panel-default').attr('data-universe');
        var tabSelected = $('ul > li[class="active"] > a').attr('data-tab');
        if (universSrc == 14) {
            var universDst = $('.panel-body[data-tree=' + tabSelected + '][data-level=' + universSrc + '] > ul');
            var levelDst = $('.panel-body[data-tree=' + tabSelected + '][data-level=' + universSrc + '] > ul > li');
            var nbItemSrc = levelSrc.length;
            var nbItemDst = levelDst.length;
            if (nbItemDst + nbItemSrc > 200) {
                bootbox.alert($('#Labels_ErrorMessageLimitUniverses').val());
                return
            }
            $('#collapse-' + universSrc + '-' + tabSelected).collapse('show');
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
        else {
            bootbox.alert($('#Labels_ErrorNoSupport').val());
        }
    }
    else {
        console.log('rien selectione');
    }
});