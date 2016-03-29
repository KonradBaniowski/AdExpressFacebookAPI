﻿
$(function () {
    console.log('open');
    var dimension = $('#Dimension').val();

    var params = {
        dimension: dimension
    };
    $.ajax({
        url: '/MediaSchedule/LoadUserUniversGroups',
        type: 'GET',
        data: params,
        error: function (xmlHttpRequest, errorText, thrownError) {
            alert("error");
        },
        success: function (response) {
            $('#monunivers .modal-content').empty();
            $('#monunivers .modal-content').append(response);
        }

    });
    //RECHERCHE 
    $(".btn-recherche").on('click', function (event) {
        var keyword = $('#keyword').val();
        if (keyword.length < 2) {
            bootbox.alert($('#Labels_ErrorMessageLimitKeyword').val());
            return;
        }
        var branchId = $('#branch').attr("data-branch");
        $(".universes").hide();
        $("#branch" + branchId).show();

        var branchUpdate = $("[id^='groupSelectable'] [id^='selectable']").length;

        $("#branch" + branchId + "> div").each(function () {
            var DIS = $(this);
            var universe = parseFloat($(this).attr('data-universe'))
            var univerLabel = $(this).attr('data-label') + "\{NB_ELEM\}";
            var params = {
                keyWord: keyword,
                universeId: universe,
                dimension: dimension
            };

            $.ajax({
                url: '/Universe/GetUniverses',
                contentType: 'application/json',
                type: 'POST',
                datatype: 'JSON',
                data: JSON.stringify(params),
                error: function (xmlHttpRequest, errorText, thrownError) {
                    alert("error");
                },
                success: function (response) {
                    if (branchUpdate > 0) {
                        console.log(DIS);
                        DIS.updateGroup(univerLabel, response.data, response.total, 'panel-heading', universe, undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.', $("#containerSelectable" + universe), $("#groupSelectable" + universe + " > .panel-heading"));
                    }
                    else {

                        DIS.fillGroupSelectable(univerLabel, response.data, response.total, 'panel-heading', 'panel-body', universe, undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.');
                    }
                    $('#selectable' + universe).selectable(
                        {
                            stop: SelectedItems
                        });

                }
            });

        });
    });

    //Déplacer un élement marché
    $('#move-item').on('click', function () {
        var levelSrc = $('.panel-marche .ui-selectee.ui-selected');
        console.log(levelSrc);
        if (levelSrc.length >= 1) {
            var universSrc = $('.ui-selectee.ui-selected').closest('.panel-default').attr('data-universe');
            var tabSelected = $('ul > li[class="active"] > a').attr('data-tab');
            var universDst = $('.panel-body[data-tree=' + tabSelected + '][data-level=' + universSrc + '] > ul');
            var levelDst = $('.panel-body[data-tree=' + tabSelected + '][data-level=' + universSrc + '] > ul > li')
            var nbItemSrc = levelSrc.length;
            var nbItemDst = levelDst.length;
            if (nbItemDst + nbItemSrc > 1000) {
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
        var dis = this;
        var nextUrl = $(this).attr('href').split('/').pop();
        NextStep(nextUrl, dis)
    });


    function NextStep(nextUrl, dis) {
        //var msg = validate();
        //if (msg) {
        //    bootbox.alert(msg);
        //    return;
        //}
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
            url: '/MediaSchedule/SaveMarketSelection',
            type: 'POST',
            //datatype: 'JSON',
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

    function SelectedItems(event, ui) {
        var itemIds = [];
        $(".ui-selected").each(function (index, elem) {
            itemIds.push($(elem).attr('data-id'));
        });
        var DIS = this;
        this.itemIds = itemIds;
        var universeIdCalling = $(this).closest('.panel').attr('data-universe');
        var branchId = $(this).closest('.panel').attr('data-branch');
        var universesToUpdate = $("[id^='groupSelectable'][data-branch='" + branchId + "'][data-universe!='" + universeIdCalling + "']");
        $.each(universesToUpdate, function (index, elem) {

            var universe = $(elem).attr('data-universe');
            var params = {
                levelId: universe,
                selectedClassification: DIS.itemIds.join(","),
                selectedLevelId: universeIdCalling,
                dimension: dimension
            };
            var univerIndex = parseFloat($(elem).attr('data-universe'));
            var univerLabel = $(elem).attr('data-label') + "\{NB_ELEM\}";

            $.ajax({
                url: '/Universe/GetClassification',
                contentType: 'application/json',
                type: 'POST',
                datatype: 'JSON',
                data: JSON.stringify(params),
                error: function (xmlHttpRequest, errorText, thrownError) {
                    alert("error");
                },
                success: function (response) {
                    $("#containerSelectable" + universe).html('');
                    $("#groupSelectable" + universe).updateGroup(univerLabel, response.data, response.total, 'panel-heading', univerIndex, undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.', $("#containerSelectable" + universe), $("#groupSelectable" + universe + " > .panel-heading"));
                    $('#selectable' + univerIndex).selectable(
                    {
                        stop: SelectedItems
                    });
                }
            });
        });
    };

    $('.btn.btn-save-univers').on('click', function (event) {
        event.preventDefault();
        $('.btn.btn-save-univers').off('click');
        $.ajax({
            url: '/MediaSchedule/SaveUserUnivers',
            type: 'GET',
            data: params,
            success: function (response) {
                $('#saveunivers').append(response);
                $('#saveunivers').modal('show');
            }
        });
    });
});

$('#keyword').off('keyup');

$('#keyword').on('keyup', function () {
    if (event.keyCode == 13) {
        $(".btn-recherche").click();
        console.log('Enter was pressed');
    }
});

//clean l element selectionnée
$(document).on('click', '.tab-content li > .pull-right', function () {
    var DIS = this;
    var parent = $(this).parents('.items-famille');
    var header = $(this).parents('.panel.panel-results');
    $(DIS).parents('li').remove()

    if (parent.find('li').length == 0) {
        header.find('[id^=collapse]').collapse('hide');
    }
});

//Clean l'ensemble des elements du tableau
$(document).on('click', 'button.tout-suppr', function () {
    var test = $(this).parent('.pull-right').siblings('.panel-group.panel-group-results');
    var idTree = $(this).parent('.pull-right').siblings('.panel-group.panel-group-results').attr('id');
    console.log(idTree);
    test.find('li').remove();
    $("#" + idTree + " [id^='collapse'].in").collapse('hide');
});

$(document).on('change', '#ddlGroup', function (event) {
    event.preventDefault();
    var idGroup = $("#ddlGroup").val();
    var dimension = $('#Dimension').val();
    var params = {
        id: idGroup,
        dimension: dimension
    };
    var local = $(this);
    $.ajax({
        url: '/MediaSchedule/GetUniversByGroup',
        type: 'GET',
        data: params,
        success: function (response) {
            $('#ddlUnivers').empty();
            $.each(response, function (i, item) {
                $("#ddlUnivers").append('<option value="' + item.Value + '">' +
                     item.Text + '</option>');
            });
            //$('#ddlUnivers').html(response);
        }
    });
});

$(document).on('click', '#btnSaveUnivers', function (event) {
    event.preventDefault();
    var spinner = new Spinner().spin(this);
    var dimension = $('#Dimension').val();
    var groupId = $('#ddlGroup').val();
    var universId = $('#ddlUnivers').val();
    var name = $('#universName').val();
    var idMedias = [];
    $.each($('.tuile-medias-active'), function (index, value) {
        idMedias.push($(value).attr('data-attr-id'));
    });

    $('#btnSaveUnivers').off('click');
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
        groupId: groupId,
        universId: universId,
        name: name,
        dimension: dimension,
        media: idMedias
    };
    $.ajax({
        url: '/MediaSchedule/SaveUserUnivers',
        type: 'POST',
        data: params,
        success: function (response) {
            $('#saveunivers').modal('hide');
            $.ajax({
                url: '/MediaSchedule/LoadUserUniversGroups',
                type: 'GET',
                error: function (data) {
                    bootbox.alert(data);
                },
                success: function (data) {
                    $('#monunivers .modal-content').empty();
                    $('#monunivers .modal-content').append(data);
                }
            });
            bootbox.alert(response);
        }
    });
});

$(document).on('click', '#LoadUnivers', function (event) {
    event.preventDefault();
    var dimension = $('#Dimension').val();
    $('.btn.btn-valider').off('click');
    var universId = $('input[name="universOpt"]:checked').val();
    var params = {
        id: universId,
        dimension: dimension
    };
    $.ajax({
        url: '/MediaSchedule/GetUserUnivers',
        type: 'POST',
        data: params,
        success: function (response) {
            $('#monunivers').modal('hide');
            var trees = response.Trees;
            var medias = response.UniversMediaIds;
            $.each(trees, function (index, tree) {
                var id = tree.Id+1;
                var tab = $('.panel-group.panel-group-results[id=tree-' + id + ']');
                $.each($(tree.UniversLevels), function (index, uniLvl) {
                    console.log(uniLvl);
                    var panel = $('.panel-group.panel-group-results[id=tree-' + id + '] .panel-body[data-level=' + uniLvl.Id + '] > ul');
                    panel.html('');
                    $('#collapse-' + uniLvl.Id + '-' + id).collapse('show');
                    SetUniversItems(uniLvl, panel);
                });

            });
            $.each(medias, function (index, item) {
                $('.tuile-medias[data-attr-id="' + item + '"]').toggleClass("tuile-medias tuile-medias-active");
            });
        },
        error: function (response) {
            bootbox.alert("Error has been occured!");
        }
    });
});

function SetUniversItems(data, panel) {
    if (data.UniversItems.length > 0) {
        for (var i = 0; i < data.UniversItems.length; i++) {
            var item = $('<li/>');
            item.val(data.UniversItems[i].Id);
            item.attr('data-id', data.UniversItems[i].Id)
            item.text(data.UniversItems[i].Label);
            item.appendTo(panel);
        }
    }
}


