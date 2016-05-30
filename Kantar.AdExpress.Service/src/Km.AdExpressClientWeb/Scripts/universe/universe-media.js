var dimension = $('#Dimension').val();
//rechercher un élement en fonction du media(s)
$(".btn-recherche").on('click', function (event) {
    event.preventDefault();
    var keyword = $('#keyword').val();
    if (keyword.length < 2) {
        bootbox.alert($('#Labels_ErrorMessageLimitKeyword').val());
        return;
    }
    var branchId = $('#branch').attr("data-branch");
    $(".universes").hide();
    $("#branch" + branchId).show();

    var branchUpdate = $("[id^='groupSelectable'] [id^='selectable']").length;
    var idMedias = [];
    if ($('.tuile-medias-active').length > 0) {
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
                    $('#selectable' + universe).selectableScroll({
                        filter: 'li',
                        stop: SelectedItems,
                        selected: SelectedItems
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


//clean la selection en cas de changement de médias
$('.tuile-medias').on('click', function (event) {
    var panel = $('.panel-group.panel-group-results[data-access-type]');
    var test = $(this).parent('.pull-right').siblings('.panel-group.panel-group-results');
    var idTree = $('.panel-group.panel-group-results[data-access-type]').attr('id');
    panel.find('li').remove();
    $("#" + idTree + " [id^='collapse'].in").collapse('hide');
});