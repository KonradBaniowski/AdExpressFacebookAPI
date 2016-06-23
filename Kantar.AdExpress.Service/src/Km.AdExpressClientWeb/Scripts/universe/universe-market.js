
var dimension = $('#Dimension').val();
//Déplacer un élement marché
$('#move-item').on('click', function () {
    var levelSrc = $('.panel-marche .ui-selectee.ui-selected');
    if (levelSrc.length >= 1) {
        var universSrc = $('.ui-selectee.ui-selected').closest('.panel-default').attr('data-universe');
        var tabSelected = $('ul > li[class="active"] > a').attr('data-tab');
        var universDst = $('.panel-body[data-tree=' + tabSelected + '][data-level=' + universSrc + '] > ul');
        var levelDst = $('.panel-body[data-tree=' + tabSelected + '][data-level=' + universSrc + '] > ul > li')
        var nbItemSrc = levelSrc.length;
        var nbItemDst = levelDst.length;
        if ($('#CurrentModule').val() == 17109)
        {
            if (!(universSrc == 6 || universSrc == 8))
            {
                bootbox.alert("You can select only advertisers or brands.");
                return
            }
            if(nbItemSrc>5 ||nbItemDst>5 )
            {
                bootbox.alert("Please select at maximum 5 advertisers or brands. ");
                return
            }
        }
        
        if (nbItemDst + nbItemSrc > 1000) {
            bootbox.alert($('#Labels_ErrorMessageLimitUniverses').val());
            return
        }
        $('#collapse-' + universSrc + '-' + tabSelected).collapse('show');
        $('#heading-' + universSrc + '-' + tabSelected).find('.panel-title').addClass('orange');
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

//RECHERCHE SANS MEDIAS
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
                    DIS.updateGroup(univerLabel, response.data, response.total, 'panel-heading', universe, undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.', $("#containerSelectable" + universe), $("#groupSelectable" + universe + " > .panel-heading"));
                }
                else {

                    DIS.fillGroupSelectable(univerLabel, response.data, response.total, 'panel-heading', 'panel-body', universe, undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.');
                }
                $('#selectable' + universe).selectableScroll({
                    stop: SelectedItems
                });

            }
        });

    });
});

