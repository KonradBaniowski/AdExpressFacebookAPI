﻿
var idList = null;
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
//VALIDER TODO
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
            selectedMedia: idList,
            mediaSupport: selectedMediaSupportTrees,
            nextStep: "PeriodSelection"
        };
        $.ajax({
            url: '/PresentAbsent/SaveMediaSelection',
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


//FIL D ARRIANE
$('#Market').on('click', function (e) {
    e.preventDefault();
    var dis = this;
    var nextUrl = $(this).attr('href').split('/').pop();
    if (nextUrl === "PresentAbsent") {
        nextUrl = "Index";
    }
    NextStep(nextUrl, dis)
});

$('#Dates').on('click', function (e) {
    e.preventDefault();
    var dis = this;
    var nextUrl = $(this).attr('href').split('/').pop();
    if (nextUrl === "PresentAbsent") {
        nextUrl = "Index";
    }
    NextStep(nextUrl, dis)
});

$('#Results').on('click', function (e) {
    e.preventDefault();
    var dis = this;
    var nextUrl = $(this).attr('href').split('/').pop();
    if (nextUrl === "PresentAbsent") {
        nextUrl = "Index";
    }
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

function NextStep(nextUrl, dis) {
    var msg = validate();
    if (msg) {
        bootbox.alert(msg);
        return;
    }
    $('#btnSubmitMarketSelection').off('click');
    var selectedMediaSupportTrees = getSelectedMediaSupport();
    var params = {
        selectedMedia: idList,
        mediaSupport: selectedMediaSupportTrees,
        nextStep: nextUrl
    };
    var ctrl = $('#Labels_CurrentController').val();
    $.ajax({
        url: '/' + ctrl + '/SaveMediaSelection',
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

function validate() {
    var message = "";
    var nbElemInclus = $("[id^='tree'][data-access-type='1'] li[data-id]").length;
    if (nbElemInclus < 1) {
        message = $('#Labels_ErrorMininumInclude').val();
    }
    if (idList.length == 0)
        message += "\n" + $('#Labels_ErrorMediaSelected').val();
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
            //var isLock = $('#' + idTab).attr('lock');
            //if (isLock == undefined || isLock == false) {
                var universDst = $('.panel-body[data-tree=' + indexTab + '][data-level=' + universSrc + '] > ul');
                var levelAllDst = $('.tab-content li');
                if (levelAllDst.length < 200) {
                    
                    $.each(levelSrc, function (index, value) {
                        var item = $(value).clone();
                        var find = false;
                        $.each(levelAllDst, function (index, value) {

                            if (item.val() == $(value).val()) {
                                find = true;
                                bootbox.alert($('#Labels_ErrorSupportAlreadyDefine').val());
                            }
                        });
                        if (!find) {
                            $('#collapse-' + universSrc + '-' + indexTab).collapse('show');
                            $('#heading-' + universSrc + '-' + indexTab).find('.panel-title').addClass('orange');
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
            //}
            //else {
            //    bootbox.alert('lock');
            //}
        }
        else {
            bootbox.alert($('#Labels_ErrorNoSupport').val());
        }
    }
    else {
        console.log('rien selectione');
    }
});

