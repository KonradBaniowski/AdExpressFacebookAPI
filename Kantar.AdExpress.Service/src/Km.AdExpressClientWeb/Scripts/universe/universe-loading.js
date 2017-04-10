var dimension = $('#Dimension').val();
var PRESENTABSENTID = 278;
var params = {
    dimension: dimension
};

$(document).on('click', '#myUnivers', function (event) {
    event.preventDefault();
    $('#myUnivers').off('click');
    var dimension = $('#Dimension').val();
    var params = {
        dimension: dimension
    };
    $.ajax({
        url: '/Universe/LoadUserUniversGroups',
        type: 'GET',
        data: params,
        error: function (xmlHttpRequest, errorText, thrownError) {
            bootbox.alert("An error occurred while processing your request.");
        },
        success: function (response) {
            $('.modal-content').html('');
            $('#monunivers .modal-content').append(response);
            $('#monunivers').modal('show');
        }

    });
});

$('#save-universe').on('click', function (event) {
    event.preventDefault();
    //$('#save-universe').off('click');
    var dimension = $('#Dimension').val();
    var params = {
        dimension: dimension
    };
    $.ajax({
        url: '/Universe/SaveUserUnivers',
        type: 'GET',
        data: params,
        success: function (response) {
            $('#saveunivers').html(response);
            $('#saveunivers').modal('show');
        }
    });
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
        }
    });
});

$(document).on('click', '#btnSaveUnivers', function (event) {
    event.preventDefault();
    var dimension = $('#Dimension').val();
    var groupId = $('#ddlGroup').val();
    var universId = $('#ddlUnivers').val();
    var name = $('#universName').val();
    var isDefaultUniverse = $('#isDefaultUniverse').is(":checked");
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
        isDefaultUniverse: isDefaultUniverse,
        media: idMedias
    };
    $.ajax({
        url: '/Universe/SaveUserUnivers',
        type: 'POST',
        data: params,
        success: function (response) {
            $('#saveunivers').modal('hide');
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
    if (universId != undefined && universId > 0) {
        $.ajax({
            url: '/Universe/GetUserUnivers',
            type: 'POST',
            data: params,
            success: function (response) {
                $('.modal-content').html('');
                $('#monunivers').modal('hide');
                if (response.Message.length > 0) {
                    bootbox.alert(response.Message);
                }
                else {
                    clearAllPanels();
                    var trees = response.Trees;
                    var medias = response.UniversMediaIds;
                    var nav = $('.nav.nav-tabs');
                    var id = -1;
                    var oldAccesType = "";
                    $.each(trees, function (index, tree) {
                        if (response.ModuleId == PRESENTABSENTID)
                            id = tree.Id;

                        if (id > 0 && response.ModuleId == PRESENTABSENTID && !$("#tree-" + id).length)//Create the tab
                        {
                            id = tree.Id;
                            var ulSource = $('.nav.nav-tabs');
                            var liHtml = $('<li/>');
                            var aHtml = $('<a/>');
                            //SET LABEL 
                            aHtml.text(tree.Label);
                            aHtml.attr('data-target', '#tab-' + id);
                            aHtml.attr('id', 'tree-' + id);
                            aHtml.attr('data-tab', id);
                            aHtml.attr('data-toggle', 'tab');
                            aHtml.appendTo(liHtml);
                            liHtml.appendTo(ulSource);
                            var nbIncludes = $('.panel-heading-choix.nav-tabs-alt').find('li').length;
                            var ref = $('#tab-0');
                            var tabHtml = ref.clone();
                            tabHtml.attr('id', 'tab-' + id);
                            tabHtml.attr('class', 'tab-pane');
                            var panelHtml = tabHtml.find('.panel-group.panel-group-results[id=tree-0]');
                            panelHtml.attr('id', 'tree-' + id);
                            panelHtml.find('.items-famille').empty();
                            $.each(panelHtml.find('a[data-parent]'), function (index, value) {
                                var href = $(value).attr('href');
                                href = href.slice(0, -1);
                                href += id;
                                $(value).attr('href', href);
                                var data = $(value).attr('data-parent');
                                data = data.slice(0, -1);
                                data += id;
                                $(value).attr('data-parent', '#tree-' + id);
                            });
                            $.each(panelHtml.find('.panel-collapse'), function (index, value) {
                                var panelId = $(value).attr('id');
                                panelId = panelId.slice(0, -1);
                                panelId += id;
                                $(value).attr('id', panelId);
                            });
                            $.each(panelHtml.find('.panel-body'), function (index, value) {
                                var dataTree = $(value).attr('data-tree');
                                $(value).attr('data-tree', id);
                            });
                            $.each(panelHtml.find('.panel-heading.head-results'), function (index, value) {
                                $(value).attr('id', $(value).parent().attr('href').replace('#collapse', 'heading').slice(0, -1) + id);
                            });
                            panelHtml.find('[id^=collapse-14]').collapse('hide');
                            $('.tab-content').append(tabHtml);
                        }

                        if (tree.AccessType.toString() != oldAccesType) {
                            oldAccesType = tree.AccessType;
                            var panelIds = $('.panel-group.panel-group-results[data-access-type="' + tree.AccessType + '"]').map(function () {
                                return parseInt(this.id.split("tree-")[1], 10);
                            }).get();
                            id = Math.min.apply(Math, panelIds);
                        }
                        else if (response.ModuleId != PRESENTABSENTID) {
                            id++;
                        }

                        //var tab = $('.panel-group.panel-group-results[id=tree-' + id + ']');
                        $.each($(tree.UniversLevels), function (index, uniLvl) {
                            var panel = $('.panel-group.panel-group-results[id=tree-' + id + '] .panel-body[data-level=' + uniLvl.Id + '] > ul');
                            panel.html('');
                            $('#collapse-' + uniLvl.Id + '-' + id).collapse("show");
                            $('#heading-' + uniLvl.Id + '-' + id).addClass("bg-highTransparency");
                            $('#heading-' + uniLvl.Id + '-' + id).find(".panel-title").addClass("blue");
                            SetUniversItems(uniLvl, panel);
                        });

                    });
                    $.each($('.tuile-medias-active'), function (index, item) {
                        $(this).toggleClass("tuile-medias tuile-medias-active");
                    });

                    $.each(medias, function (index, item) {
                        $('.tuile-medias[data-attr-id="' + item + '"]').toggleClass("tuile-medias tuile-medias-active");
                    });
                }
            },
            error: function (response) {
                bootbox.alert("Error has been occured!");
            }
        });
    }
    else {
        $('.modal-content').html('');
        bootbox.alert("Please select an univers to load");
    }
});

function SetUniversItems(data, panel) {
    if (data.UniversItems.length > 0) {
        for (var i = 0; i < data.UniversItems.length; i++) {
            var item = $('<li/>');
            item.val(data.UniversItems[i].Id);
            item.attr('data-id', data.UniversItems[i].Id)
            item.text(data.UniversItems[i].Label);
            var buttonSupp = $('<button/>');
            buttonSupp.addClass('pull-right');
            var icon = $('<i/>');
            icon.addClass('fa fa-times-circle black text-base');
            buttonSupp.append(icon);
            item.append(buttonSupp);
            item.appendTo(panel);
        }
    }
}

function clearAllPanels() {
    panelsGrp = $('.panel-group.panel-group-results[id^="tree-"] .panel-body[data-level] > ul.items-famille');
    $.each($(panelsGrp), function () {
        $(this).html('');
    });
    $('.panel-group.panel-group-results[id^="tree-"] > .panel-collapse.collapse.in').collapse('hide');
    $(".panel-heading.head-results.bg-highTransparency").removeClass("bg-highTransparency");
    $(".panel-title.blue").removeClass("blue");
}


$(document).on('click', '.accordion-toggle', function (e) {
    event.preventDefault();
    $('#accordion').find('.accordion-body.collapse.in').collapse("hide");
});