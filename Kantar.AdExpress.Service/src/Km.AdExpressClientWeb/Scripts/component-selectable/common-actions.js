﻿//CHANGER DE SECTEUR
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

//Touche ENTER 
$('#keyword').off('keyup');

$('#keyword').on('keyup', function () {
    if (event.keyCode == 13) {
        $(".btn-recherche").click();
    }
});

//MAJ des boxes adjacentes
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