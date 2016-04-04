var dimension = $('#Dimension').val();
var params = {
    dimension: dimension
};
$(function () {
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
});

$('#save-universe').on('click', function (event) {
    event.preventDefault();
    $('#save-universe').off('click');
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
        url: '/Universe/SaveUserUnivers',
        type: 'POST',
        data: params,
        success: function (response) {
            $('#saveunivers').modal('hide');
            $.ajax({
                url: '/Universe/LoadUserUniversGroups',
                type: 'POST',
                data: params,
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
            var nav = $('.nav.nav-tabs');
            $.each(trees, function (index, tree) {
                var id = tree.Id;
                if (id > 0)//Create the tab
                {
                    var ulSource = $('.nav.nav-tabs');
                    var liHtml = $('<li/>');
                    //liHtml.attr('class', 'active');
                    var aHtml = $('<a/>');
                    //SET LABEL 
                    console.log(tree);
                    aHtml.text(tree.Label);
                    aHtml.attr('data-target', '#tab-' + id);
                    aHtml.attr('id', 'tree-'+id);
                    aHtml.attr('data-tab', id);
                    aHtml.attr('data-toggle', 'tab');
                    aHtml.appendTo(liHtml);
                    liHtml.appendTo(ulSource);
                    var nbIncludes = $('.panel-heading-choix.nav-tabs-alt').find('li').length;
                    var ref = $('#tab-0');// + nbIncludes + '');
                    var tabHtml = ref.clone();
                    tabHtml.attr('id', 'tab-' + id);
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
                        var id = $(value).attr('id');
                        id = id.slice(0, -1);
                        id += id;
                        $(value).attr('id', id);
                    });
                    $.each(panelHtml.find('.panel-body'), function (index, value) {
                        var dataTree = $(value).attr('data-tree');
                        $(value).attr('data-tree', id);
                    });
                    panelHtml.find('[id^=collapse-14]').collapse('hide');
                    $('.tab-content').append(tabHtml);
                    //var tabContent =$('.tab-content');
                    //var tabPanel = $('<div/>');
                    //tabPanel.attr('id', 'tab-' + id);
                    //tabPanel.attr('role', 'tabpanel');
                    //var content = $('<div/>');
                    //content.attr('class', '.panel-group panel-group-results');
                    //content.attr('data-access-type', tree.AccessType);
                    //content.attr('id', 'tree-' + id);
                    //content.attr('role', 'tablist');
                    //content.attr('aria-multiselectable', true);
                    //content.appendTo(tabPanel);
                    //tabPanel.appendTo(tabContent);

                }
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
