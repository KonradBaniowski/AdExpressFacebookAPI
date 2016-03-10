
$(function () {

    $.ajax({
        url: '/MediaSchedule/LoadUserUniversGroups',
        type: 'GET',
        error: function (xmlHttpRequest, errorText, thrownError) {
            alert("error");
        },
        success: function (response) {
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
                universeId: universe
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
    $('.btn-green2.btn-circle').on('click', function () {
        var levelSrc = $('.panel-marche .ui-selectee.ui-selected');
        console.log(levelSrc);
        if (levelSrc.length >= 1) {
            var universSrc = $('.ui-selectee.ui-selected').closest('.panel-default').attr('data-universe');
            var tabSelected = $('ul > li[class="active"] > a').attr('data-tab');
            var universDst = $('.panel-body[data-tree=' + tabSelected + '][data-level=' + universSrc + '] > ul');
            var levelDst = $('.panel-body[data-tree=' + tabSelected + '][data-level=' + universSrc + '] > ul > li')
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
        //bootbox.alert("Hello world!");
        var things = [];
        $.each($('.nav.nav-tabs > li a'), function (index, elem) {
            var itemContainer = $(elem).attr('data-target');
            var accessType = $(itemContainer + ' .panel-group').attr('data-access-type');
            var idUnivers = [];
            $.each($(itemContainer + ' .panel-group .panel-body > ul > li'), function (index, elem) {
                var id = $(elem).attr('data-id');
                idUnivers.push(id);
            });
            var item = {
                AccessType: accessType,
                UniversLevels: idUnivers
            };
            things.push(item);
        });
        var params = {
            tree: things,
            nextStep: "PeriodSelection"
        };
        //    $.ajax({
        //        url: '/MediaSchedule/SaveMediaSelection',
        //        contentType: 'application/json',
        //        type: 'POST',
        //        datatype: 'JSON',
        //        data: JSON.stringify(params),
        //        error: function (xmlHttpRequest, errorText, thrownError) {
        //        },
        //        success: function (data) {
        //            if (data != null) {
        //                document.location = data.RedirectUrl;
        //            }
        //        }
        //    });
        //}
    });

    $('#Market').on('click', function (e) {
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
                selectedLevelId: universeIdCalling
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
                    Id : itemUniver
                }
                UniLvl.push(universItems);
            });
            var UnisLvl = {
                Id : idLevel,
                UniversItems: UniLvl
            };
            UniversLvl.push(UnisLvl);
        });
        var stuff = {
            Id: itemContainer,
            UniversLevels: UniversLvl
        };
        trees.push(stuff);
    });



    var params = {
        trees:  trees,
        groupId: groupId,
        universId: universId,
        name: name
    };
    $.ajax({
        url: '/MediaSchedule/SaveUserUnivers',
        type: 'POST',
        data: params,
        success: function (response) {
            spinner.stop();
            $('#saveunivers').modal('hide');
            bootbox.alert(response);
        }
    });
});


    $(document).on('change', '#ddlGroup', function (event) {
        event.preventDefault();
        var idGroup = $("#ddlGroup").val();
        var local = $(this);
        var spinner = new Spinner().spin(this);
        $.ajax({
            url: '/MediaSchedule/GetUniversByGroup',
            type: 'GET',
            data: { id: idGroup },
            success: function (response) {
                $('#ddlUnivers').empty();
                $.each(response, function (i, item) {
                    spinner.stop();
                    $("#ddlUnivers").append('<option value="' + item.Value + '">' +
                         item.Text + '</option>');
                });
                //$('#ddlUnivers').html(response);
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

    function ShowSelection(elem) {
        var selectedItems = elem.getSelectableSelectedItems();
        var result = '';
        selectedItems.forEach(function (item) {
            result += ' #' + item.Value + ';' + item.Text + '\n';
        });
        alert('Selection : \n' + result);
    };
