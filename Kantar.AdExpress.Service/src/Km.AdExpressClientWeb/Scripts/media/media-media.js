var idList = null;

var idMedias = [];
$(document).ready(function () {
    var searchId = '18';
    //var socialId=TBD
    if ($('#Multiple').val() == "True") {

        $('.tuile-medias[data-attr-id]').on('click', selectMultiple)
        idList = [];
        //CHECKBOX
        var idCommon = [];
        $('[name="HiddenIntList"]').each(function (index) {
            idCommon.push($(this).attr("value"));
        });
        $('.tuile-medias[data-attr-id]').each(function (i, e) {
            var elem = $(e);
            var indexElem = elem.attr('data-attr-id');
            var index = $.inArray(indexElem, idCommon);
            if (index == -1)
                elem.attr("data-grp", "A");
            else
                elem.attr("data-grp", "B");
        });

        $(':checkbox').on('change', preselection);

        //FOCUS
        $('[data-grp]').on('mouseenter', highlight);
        $('[data-grp]').on('mouseleave', unhighlight);
    }
    else {
        idList = "";
        $('.tuile-medias[data-attr-id]').on('click', selectUnique)
    }

    function highlight() {
        var grp = $(this).attr('data-grp');
        $('[data-grp="' + grp + '"]').addClass('highlight');
    }

    function unhighlight() {
        var grp = $(this).attr('data-grp');
        $('[data-grp="' + grp + '"]').removeClass('highlight');
    }

    function selectUnique() {
        $(this).toggleClass("tuile-medias tuile-medias-active")
        var id = $(this).attr("data-attr-id");
        if (idList !== null && idList !== undefined) {
            $('.tuile-medias-active[data-attr-id="' + idList + '"]').toggleClass("tuile-medias tuile-medias-active")
        }
        idList = id;
    }

    function selectMultiple(e) {
        $(this).toggleClass("tuile-medias tuile-medias-active")
        var id = $(this).attr("data-attr-id");

        var index = $.inArray(id, idList);
        if (index > -1) {
            idList.splice(index, 1);
        }
        else {
            idList.push(id);
        }
    }

    function preselection() {
        $('.tuile-medias-active').toggleClass("tuile-medias tuile-medias-active")
        var attr = $(this).attr('checked');
        if (typeof attr !== typeof undefined && attr !== false) {
            $.each(idCommon, function (index, value) {
                $('.tuile-medias-active[data-attr-id="' + value + '"]').toggleClass("tuile-medias tuile-medias-active")
            });
            $(this).removeAttr("checked");
        }
        else {
            $.each(idCommon, function (index, value) {
                $('.tuile-medias[data-attr-id="' + value + '"]').toggleClass("tuile-medias tuile-medias-active")
            });
            $(this).attr("checked", "");
        }
    }
});

$('#btnSubmitMediaSelection').on('click', function (e) {
    e.preventDefault();
    var msg = validate();
    var isValide = !msg || msg.lentgh === 0;
    if (!isValide) {//mycondition
        bootbox.alert(msg);
    }
    else {
        var selectedMediaSupportTrees = getSelectedMediaSupport();
        var params = {
            selectedMedia: idMedias,
            mediaSupport: selectedMediaSupportTrees,
            nextStep: "PeriodSelection"
        };
        $.ajax({
            url: '/MediaSchedule/SaveMediaSelection',
            contentType: 'application/json',
            type: 'POST',
            datatype: 'JSON',
            data: JSON.stringify(params),
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                if (data.RedirectUrl != null) {
                    document.location = data.RedirectUrl;
                }
                if (data.ErrorMessage != null) {
                    bootbox.alert(data.ErrorMessage);
                }
            }
        });
    }
});
//VALIDER TODO

//FIL D ARRIANE
$('#Market').on('click', function (e) {
    e.preventDefault();
    var dis = this;
    var nextUrl = $(this).attr('href').split('/').pop();
    if (nextUrl === "MediaSchedule"|| nextUrl==="Analysis") {
        //nextUrl = "Index";
        nextUrl = "Market";
    }
    NextStep(nextUrl, dis)
});

$('#Dates').on('click', function (e) {
    e.preventDefault();
    var dis = this;
    var nextUrl = $(this).attr('href').split('/').pop();
    
    if (nextUrl === "MediaSchedule"|| nextUrl==="Analysis") {
        //nextUrl = "Index";
        nextUrl = "Market";
    }
    NextStep(nextUrl, dis)
});

$('#Results').on('click', function (e) {
    e.preventDefault();
    var gotoResult = true;
    strHtml = "";
    var items = $(this).parent().parent().find('.btn.btn-warning.btn-circle.btn-empty');
    $.each(items, function (index, value) {
        var page = $(value).attr('id');
        if (page == "Dates" || page =="Market")
        {
            gotoResult = false;
            return;
        }
    });
    if(gotoResult)
    {
        var nextUrl = $(this).attr('href').split('/').pop();
        if (nextUrl === "MediaSchedule") {
            strHtml += "<li>" + page + "</li>";
            //nextUrl = "Index";
            nextUrl = "Market";
        }
        var dis = this;
        NextStep(nextUrl, dis)
    }
    else
    {
        strHtml = "Veuillez compléter le(s) paramètre(s) suivant(s) : <ul>" + strHtml + "</ul>";
        bootbox.alert(strHtml);
    }
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
        message = $('#Labels_ErrorMediaSelected').val();
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
        url: '/Selection/SaveMediaSelection',
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
