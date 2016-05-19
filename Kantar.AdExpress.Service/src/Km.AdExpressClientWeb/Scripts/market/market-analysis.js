$(function () {

    $("#branch1 > div").each(function () {
        var DIS = $(this);
        var universe = parseFloat($(this).attr('data-universe'))
        var univerLabel = $(this).attr('data-label') + "\{NB_ELEM\}";
        if (universe === 1) {
            $.ajax({
                url: '/Universe/GetCategoryItems',
                contentType: 'application/json',
                type: 'POST',
                datatype: 'JSON',
                error: function (xmlHttpRequest, errorText, thrownError) {
                    alert("error");
                },
                success: function (response) {
                    DIS.fillGroupSelectable(univerLabel, response.data, response.total, 'panel-heading', 'panel-body', universe, undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.');

                    $('#selectable' + universe).selectableScroll({
                        stop: SelectedItems
                    });

                }
            });
        }
        else {
            DIS.fillGroupSelectable(univerLabel, undefined, 0, 'panel-heading', 'panel-body', universe, undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.');
        }

    });










    //var branchId = $('#branch').attr("data-branch");   
    //$("#branch" + branchId).show();
    //var DIS = $("#branch1 > div");
    //var universe = parseFloat(DIS.attr('data-universe'))
    //var univerLabel = DIS.attr('data-label') + "\{NB_ELEM\}"; 

    //$.ajax({
    //    url: '/Universe/GetCategoryItems',
    //    contentType: 'application/json',
    //    type: 'POST',
    //    datatype: 'JSON',
    //    //data: JSON.stringify(params),
    //    error: function (xmlHttpRequest, errorText, thrownError) {
    //        alert("error");
    //    },
    //    success: function (response) {
    //        DIS.fillCategorySelectable(univerLabel, response.data, response.total, 'panel-heading', 'panel-body', universe, undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.');
    //       $('#selectable' + universe).selectableScroll({
    //            stop: SelectedItems
    //        });

    //    }
    //});
})

jQuery.fn.extend({
    fillCategorySelectable: function (titleText, datas, total, classNameTitle, classNameDivSelection, idSelection, classNameSelection, nbElemMax, nbElemMaxText) {
        var divHtmlList = $('<div/>');
        var groupSelectable = $("#groupSelectable1");
        if (classNameDivSelection != undefined)
            divHtmlList.addClass(classNameDivSelection);
        divHtmlList.attr('id', 'containerSelectable' + idSelection);

        var divTitleHtml = $('<div/>');
        if (classNameTitle != undefined)
            divTitleHtml.addClass(classNameTitle);
        divTitleHtml.appendTo(groupSelectable);
        this.updateGroup(titleText, datas, total, classNameTitle, idSelection, classNameSelection, nbElemMax, nbElemMaxText, divHtmlList, divTitleHtml);
        divHtmlList.appendTo(groupSelectable);
    }
})