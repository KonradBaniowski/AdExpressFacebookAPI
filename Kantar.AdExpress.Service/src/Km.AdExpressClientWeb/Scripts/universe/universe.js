
$(function () {
    $(".dropdown-menu.bg-blue.pull-right li > a").on('click', function (e) {
        e.preventDefault();
        var selText = $(this).text();
        var selValue = $(this).attr("data-id");
        //$("#" + selValue).show();
        $(this).parents('.input-group-btn').find('.btn.btn-default.select-recherche').html(selText + '<span class="caret"></span>');
    });

    $(".btn-recherche").on('click', function (event) {
        var keyword = $('#keyword').val();
        var branchId = $('#branch').attr("data-branch");
        $(".universes").hide();
        $("#branch" + branchId).show();
        $("#branch" + branchId + "> div").html('');
        $("#branch" + branchId + "> div").each(function () {
            var univerIndex = parseFloat($(this).attr('data-universe'))
            var univerLabel = $(this).attr('data-label') + "\{NB_ELEM\}";
            var params = {
                keyWord: keyword,
                universeId: univerIndex
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
                    $("#groupSelectable" + univerIndex).fillGroupSelectable(univerLabel, response.data, 'panel-heading', 'panel-body', 'containerSelectable1', undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.');
                    $("#groupSelectable" + univerIndex).selectable(
                    {
                    });
                }
            });
        });
    });


});

function ShowSelection(elem) {
    var selectedItems = elem.getSelectableSelectedItems();
    var result = '';
    selectedItems.forEach(function (item) {
        result += ' #' + item.Value + ';' + item.Text + '\n';
    });
}

