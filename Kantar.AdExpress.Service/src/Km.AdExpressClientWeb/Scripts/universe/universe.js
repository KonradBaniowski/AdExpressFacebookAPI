
$(function () {
    
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
    //CHANGER DE SECTEUR
    $(".dropdown-menu.bg-blue.pull-right li > a").on('click', function (e) {
        e.preventDefault();
        var selText = $(this).text();
        var selValue = $(this).attr("data-id");
        $("#default").show();
        $("#default").removeAttr("id");
        $(this).attr("id", "default");
        $("#default").hide();
        $(this).parents('.input-group-btn').find('.btn.btn-default.select-recherche').html(selText + '<span class="caret"></span>');
        $(this).parents('.input-group-btn').find('.btn.btn-default.select-recherche').attr("data-branch", selValue);
        //CLEAN PANEL ON CHANGE BRANCH
        $("[id^='groupSelectable'] [id^='containerSelectable']").parents('.panel-default').html('');
    });

   

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
            if (nbItemDst + nbItemSrc > 1000)
            {
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
        var things = [];
        var spinner = new Spinner().spin(this);
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
            nextStep: "MediaSelection"
        };

        $.ajax({
            url: '/MediaSchedule/SaveMarketSelection',
            type: 'POST',
            //datatype: 'JSON',
            data: params,
            error: function (data) {
                spinner.stop();
                bootbox.alert(data.ErrorMessage);
            },
            success: function (data) {
                spinner.stop();
                if (data.ErrorMessage != null && data.ErrorMessage !="") {
                    bootbox.alert(data.ErrorMessage);
                }
                if (data.RedirectUrl!=null && data.RedirectUrl !="") {
                    document.location = data.RedirectUrl;
                }
            }
        });
    });

    $('#Media').on('click', function (e) {
        e.preventDefault();
        var msg = validate();
        var isValide = !msg || msg.lentgh === 0;
        if (!isValide) {//mycondition
            bootbox.alert(msg);
        }
        else {
            var action = "SaveMediaSelection";
            var params = {
                selectedMedia: idList,
                nextStep: "Index"
            };

            $.ajax({
                url: '/MediaSchedule/' + action,
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
        }
    });



    $('#Dates').on('click', function (e) {
        e.preventDefault();
        var msg = validate();
        var isValide = !msg || msg.lentgh === 0;
        if (!isValide) {//mycondition
            alert(msg);
        }
        else {
            var action = "SaveMediaSelection";
            var params = {
                selectedMedia: idList,
                nextStep: "PeriodSelection"
            };

            $.ajax({
                url: '/MediaSchedule/' + action,
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
        }
    });
    $('#Results').on('click', function (e) {
        e.preventDefault();
        var msg = validate();
        var isValide = !msg || msg.lentgh === 0;
        if (!isValide) {//mycondition
            alert(msg);
        }
        else {
            action = "SaveMediaSelection";
            params = {
                selectedMedia: idList,
                nextStep: "Results"
            };
            $.ajax({
                url: '/MediaSchedule/' + action,
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
        }
    });

    $('.btn.btn-save-univers').on('click', function (event) {
        event.preventDefault();
        var spinner = new Spinner().spin(this);
        $('.btn.btn-save-univers').off("click");
        $.ajax({
            url: '/MediaSchedule/SaveUserUnivers',
            type: 'GET',
            data: params,
            success: function (response) {
                spinner.stop();
                $('#saveunivers').append(response);
                $('#saveunivers').modal('show');
            }
        });
    });

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
        var spinner = new Spinner().spin(DIS);
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
                    spinner.stop();
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
});
$('#keyword').off('keyup');

$('#keyword').on('keyup', function () {
    if (event.keyCode == 13) {
        $(".btn-recherche").click();
        console.log('Enter was pressed');
    }
});

var Example = (function () {
    "use strict";

    var elem,
        hideHandler,
        that = {};

    that.init = function (options) {
        elem = $(options.selector);
    };

    that.show = function (text) {
        clearTimeout(hideHandler);

        elem.find("span").html(text);
        elem.delay(200).fadeIn().delay(4000).fadeOut();
    };

    return that;
}());

$(function () {
    Example.init({
        "selector": ".bb-alert"
    });
});

//clean l element selectionnée
$(document).on('click', '.tab-content li', function () {
    this.remove();
});

$(document).on('click', '#btnSaveUnivers', function (event) {
    event.preventDefault();
    var dimension = $('#Dimension').val();
    var groupId = $('#ddlGroup').val();
    var universId = $('#ddlUnivers').val();
    var name = $('#universName').val();
    var spinner = new Spinner().spin(this);
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
        dimension:dimension
    };
    $.ajax({
        url: '/MediaSchedule/SaveUserUnivers',
        type: 'POST',
        data: params,
        success: function (response) {
            spinner.stop();
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


$(document).on('change', '#ddlGroup', function (event) {
    event.preventDefault();
    var dimension = $('#Dimension').val();
    var idGroup = $("#ddlGroup").val();
    var params = {
        id: idGroup,
        dimension: dimension
    };
    var local = $(this);
    var spinner = new Spinner().spin(this);
    $.ajax({
        url: '/MediaSchedule/GetUniversByGroup',
        type: 'GET',
        data: params,
        success: function (response) {
            $('#ddlUnivers').empty();
            $.each(response, function (i, item) {
                spinner.stop();
                $("#ddlUnivers").append('<option value="' + item.Value + '">' +
                     item.Text + '</option>');
            });
        }
    });
});

//Clean l'ensemble des elements du tableau
$(document).on('click', 'button.tout-suppr', function () {
    var test = $(this).parent('.pull-right').siblings('.panel-group.panel-group-results');
    var idTree = $(this).parent('.pull-right').siblings('.panel-group.panel-group-results').attr('id');
    console.log(idTree);
    test.find('li').remove();
    $("#" + idTree + " [id^='collapse'].in").collapse('hide');
});

$(document).on('click', '#LoadUnivers', function (event) {
    event.preventDefault();
    var dimension = $('#Dimension').val();
    var spinner = new Spinner().spin(this);
    $('.btn.btn-valider').off('click');
    var universId = $('input[name="universOpt"]:checked').val();
    var params = {
        id: universId,
        dimension:dimension
    };
    $.ajax({
        url: '/MediaSchedule/GetUserUnivers',
        type: 'POST',
        data: params,
        success: function (response) {
            spinner.stop();
            $('#monunivers').modal('hide');
            var trees = response.Trees;
            $.each(trees, function (index, tree) {
                var id = tree.Id + 1;
                var tab = $('.panel-group.panel-group-results[id=tree-' + id + ']');
                $.each($(tree.UniversLevels), function (index, uniLvl) {
                    console.log(uniLvl);
                    var panel = $('.panel-group.panel-group-results[id=tree-' + id + '] .panel-body[data-level=' + uniLvl.Id + '] > ul');
                    panel.html('');
                    $('#collapse-' + uniLvl.Id + '-' + id).collapse('show');
                    SetUniversItems(uniLvl, panel);
                });

            });
        },
        error: function (response) {
            spinner.stop();
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

function ShowSelection(elem) {
    var selectedItems = elem.getSelectableSelectedItems();
    var result = '';
    selectedItems.forEach(function (item) {
        result += ' #' + item.Value + ';' + item.Text + '\n';
    });
    alert('Selection : \n' + result);
};


function validate() {
    var message = "";
    //nbr éléments déplacer dans les univers inclus
    var nbElemInclus = $("[id^='tree'][data-access-type='1'] li[data-id]").length;
    if (nbElemInclus > 1)
    {
        message = $('#Labels_ErrorMessageLimitUniverses').val();
    }
    return message;
}
