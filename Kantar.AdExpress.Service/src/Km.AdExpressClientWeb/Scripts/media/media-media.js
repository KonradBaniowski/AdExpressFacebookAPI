//VALIDER TODO
var idMedias = [];
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

function getSelectedMediaSupport() {
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
    return trees;
}
function validate() {
    var message = "";
    idMedias = [];
    if ($('.tuile-medias-active').length > 0) {
        $.each($('.tuile-medias-active'), function (index, value) {
            idMedias.push($(value).attr('data-attr-id'));
        });
    }
    if (idMedias.length == 0)
        message = $('#Labels_ErrorMediaSelected');
    return message;
}

function NextStep(nextUrl, dis) {
    var msg = validate();
    if (msg) {
        bootbox.alert(msg);
        return;
    }
    $('#btnSubmitMarketSelection').off('click');
   
    var selectedMediaSupportTrees = getSelectedMediaSupport();
    var params = {
        selectedMedia: idMedias,
        mediaSupport: selectedMediaSupportTrees,
        nextStep: nextUrl
    };
    $.ajax({
        url: '/MediaSchedule/SaveMediaSelection',
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
});