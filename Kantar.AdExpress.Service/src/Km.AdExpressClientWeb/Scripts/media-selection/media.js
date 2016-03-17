$(function () {
    var dimension = $('#Dimension').val();
    $(".btn-recherche").on('click', function (event) {
        var keyword = $('#keyword').val();
        var branchId = $('#branch').attr("data-branch");
        $(".universes").hide();
        $("#branch" + branchId).show();

        var branchUpdate = $("[id^='groupSelectable'] [id^='selectable']").length;
        var idMedias = [];
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

