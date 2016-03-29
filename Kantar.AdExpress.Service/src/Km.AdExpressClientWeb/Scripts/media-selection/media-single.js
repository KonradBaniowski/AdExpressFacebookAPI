$(function () {
    var dimension = $('#Dimension').val();
    var params = {
        dimension: dimension
    };
    $.ajax({
        url: '/Universe/LoadUserUniversGroups',
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

    //CHANGER DE branche
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

    //clean la selection en cas de changement de médias
    $('.tuile-medias').on('click', function (event) {
        var panel = $('.panel-group.panel-group-results[data-access-type]');
        var test = $(this).parent('.pull-right').siblings('.panel-group.panel-group-results');
        var idTree = $('.panel-group.panel-group-results[data-access-type]').attr('id');
        panel.find('li').remove();
        $("#" + idTree + " [id^='collapse'].in").collapse('hide');
    });

    //rechercher un élement 
    $(".btn-recherche").on('click', function (event) {
        event.preventDefault();
        var keyword = $('#keyword').val();
        if (keyword.length < 2)
            return;
        var branchId = $('#branch').attr("data-branch");
        $(".universes").hide();
        $("#branch" + branchId).show();

        var branchUpdate = $("[id^='groupSelectable'] [id^='selectable']").length;
        var idMedias = [];
        if ($('.tuile-medias-active').length > 0 ) {
            $.each($('.tuile-medias-active'), function (index, value) {
                idMedias.push($(value).attr('data-attr-id'));
            });
            $("#branch" + branchId + "> div").each(function () {
                var DIS = $(this);
                var universe = parseFloat($(this).attr('data-universe'))
                var univerLabel = $(this).attr('data-label') + "\{NB_ELEM\}";
                var params = {
                    keyWord: keyword,
                    universeId: universe,
                    dimension: dimension,
                    idMedias: idMedias
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
        }
        else {
            bootbox.alert($('#Labels_ErrorMediaSelected').val());
            $('#collapseTwo').collapse('hide');
            $('#collapseOne').collapse('show');
        }
    });

    //Déplacer uniquement un élement marché
    //$('.btn-green2.btn-circle').on('click', function () {
    //    var levelSrc = $('.panel-marche .ui-selectee.ui-selected');
    //    var tabSelected = $('ul > li[class="active"] > a').attr('data-tab');
    //    if (levelSrc.length == 1) {
    //        var universSrc = $('.ui-selectee.ui-selected').closest('.panel-default').attr('data-universe');
    //        if (universSrc == 14) {
    //            var universDst = $('.panel-body[data-tree=' + tabSelected + '][data-level=' + universSrc + '] > ul');
    //            var levelDst = $('.panel-body[data-tree=' + tabSelected + '][data-level=' + universSrc + '] > ul > li')
    //            $('#collapse-' + universSrc + '-' + tabSelected).collapse('show');
    //            $.each(levelSrc, function (index, value) {
    //                var item = $(value).clone();
    //                var find = false;
    //                $.each(levelDst, function (index, value) {

    //                    if (item.val() == $(value).val())
    //                        find = true;
    //                });
    //                if (!find) {
    //                    var buttonSupp = $('<button/>');
    //                    buttonSupp.addClass('pull-right');
    //                    var icon = $('<i/>');
    //                    icon.addClass('fa fa-times-circle black text-base');
    //                    buttonSupp.append(icon);
    //                    item.append(buttonSupp);
    //                    universDst.append(item);
    //                }
    //            });
    //        }
    //        else {
    //            bootbox.alert($('#Labels_ErrorNoSupport').val());
    //        }
    //    }
    //    else {
    //        bootbox.alert($('#Labels_ErrorItemExceeded').val());
    //    }
    //});

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
        var idMedias = [];
        $.each($('.tuile-medias-active'), function (index, value) {
            idMedias.push($(value).attr('data-attr-id'));
        });
        $.each(universesToUpdate, function (index, elem) {

            var universe = $(elem).attr('data-universe');
            var params = {
                levelId: universe,
                selectedClassification: DIS.itemIds.join(","),
                selectedLevelId: universeIdCalling,
                dimension: dimension,
                idMedias: idMedias
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

    $('#save-universe').on('click', function (event) {
        event.preventDefault();
        $('.btn.btn-save-univers').off("click");
        $.ajax({
            url: '/Universe/SaveUserUnivers',
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
    }
});

//clean element
$(document).on('click', '.tab-content li > .pull-right', function () {
    var DIS = this;
    var parent = $(this).parents('.items-famille');
    var header = $(this).closest('.panel.panel-results');
    $(DIS).parents('li').remove()

    if (parent.find('li').length == 0) {
        header.find('[id^=collapse]').collapse('hide');
    }
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
        url: '/Universe/GetUniversByGroup',
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
        url: '/Universe/SaveUserUnivers',
        type: 'POST',
        data: params,
        success: function (response) {
            $('#saveunivers').modal('hide');
            $.ajax({
                url: '/Universe/LoadUserUniversGroups',
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
        url: '/Universe/GetUserUnivers',
        type: 'POST',
        data: params,
        success: function (response) {
            $('#monunivers').modal('hide');
            var trees = response.Trees;
            var medias = response.UniversMediaIds;
            $.each(trees, function (index, tree) {
                var id = tree.Id ;
                var tab = $('.panel-group.panel-group-results[id=tree-' + id + ']');
                $.each($(tree.UniversLevels), function (index, uniLvl) {
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
