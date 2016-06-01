//CHANGER DE SECTEUR
$(".dropdown-menu.bg-blue li > a").on('click', function (e) {
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

$('#keyword').on('keyup', function (event) {
    if (event.keyCode == 13) {
        $(".btn-recherche").click();
    }
});
var idMedias = [];
//MAJ des boxes adjacentes

function SelectedItems(event, ui) {
    var itemIds = [];
    $(this).closest('.panel').find($(".ui-selected")).each(function (index, elem) {
        itemIds.push($(elem).attr('data-id'));

        //$(".ui-selected")
    });
    var DIS = this;
    this.itemIds = itemIds;
    var universeIdCalling = $(this).closest('.panel').attr('data-universe');
    var branchId = $(this).closest('.panel').attr('data-branch');
    var prevs = $(this).closest('.panel').prevAll();
    var nextOne = $(this).closest('.panel').next();
    var universesToUpdate = prevs.add(nextOne);
    var others = $("[id^='groupSelectable'][data-branch='" + branchId + "'][data-universe!='" + universeIdCalling + "']");
    var test = $("[id^='groupSelectable'][data-branch='" + branchId + "']");
    var id = nextOne.attr("data-universe");
    var clean = false;
    $.each(test, function (index, value) {
        if ($(value).attr("data-universe") === id) {
            clean = true;
        }
        else if (clean) {
            $(value).find($('.badge.bg-blue')).html('');
            $(value).find($('.panel-body > ul')).html('');
            $(value).find($('small')).html('');
        }
    });
    var idMedias = [];
    if ($('.tuile-medias-active').length > 0) {
        $.each($('.tuile-medias-active'), function (index, value) {
            idMedias.push($(value).attr('data-attr-id'));
        });
    }
    $.each(universesToUpdate, function (index, elem) {

        var universe = $(elem).attr('data-universe');
        var selectedClassification = DIS.itemIds.join(",")
        var params = {
            levelId: universe,
            selectedClassification: selectedClassification,
            selectedLevelId: universeIdCalling,
            dimension: dimension,
            idMedias: idMedias
        };
        var univerIndex = parseFloat($(elem).attr('data-universe'));
        var univerLabel = $(elem).attr('data-label') + "\{NB_ELEM\}";
        if (selectedClassification != "") {
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
                    $('#selectable' + univerIndex).selectableScroll(
                    {
                        filter: 'li',
                        stop: SelectedItems
                    });

               
                }
            });
        }
        else {
            bootbox.alert("Would you please select an item.");
            return false;
        }
    });
};

function SelectSelectableElement(selectableContainer, elementsToSelect) {
    // add unselecting class to all elements in the styleboard canvas except the ones to select
    $(".ui-selected", selectableContainer).not(elementsToSelect).removeClass("ui-selected").addClass("ui-unselecting");

    // add ui-selecting class to the elements to select
    $(elementsToSelect).not(".ui-selected").addClass("ui-selecting");


    selectableContainer.data("ui-selectableScroll")._mouseStop(null);

};

//clean l element selectionnée
$(document).on('click', '.tab-content li > .pull-right', function () {
    var DIS = this;
    var parent = $(this).parents('.items-famille');
    var header = parent.closest('.panel.panel-results');
    $(DIS).parents('li').remove()

    if (parent.find('li').length == 0) {
        header.find('[id^=collapse-].in').collapse('hide');
        header.find('.panel-title.famille.orange').removeClass('orange')
    }
});

//Clean l'ensemble des elements du tableau
$(document).on('click', 'button.tout-suppr', function () {
    var test = $(this).parent('.pull-right').siblings('.panel-group.panel-group-results');
    var idTree = $(this).parent('.pull-right').siblings('.panel-group.panel-group-results').attr('id');
    console.log(idTree);
    test.find('li').remove();
    test.find('.panel-title.famille.orange').removeClass('orange');
    $("#" + idTree + " [id^='collapse'].in").collapse('hide');
});

$(document).on('click', '.add-all', function () {
    var selectable = $(this).closest('.panel-default').find('ul');
    var id = selectable.attr('id');
    var idselectable = "#" + id;
    var others = $("[id^='selectable'][id!='" + id + "']");
    $.each(others, function (index, value) {
        $(value).find('li').removeClass("ui-selected");
    });
    SelectSelectableElement($(idselectable), $(idselectable + " li"));
});

$(document).on('click', '.rem-all', function () {
    var selectable = $(this).closest('.panel-default').find('ul');
    var idselectable = "#" + selectable.attr('id');
    SelectSelectableElement($(idselectable), $(idselectable + " no:li"));
});