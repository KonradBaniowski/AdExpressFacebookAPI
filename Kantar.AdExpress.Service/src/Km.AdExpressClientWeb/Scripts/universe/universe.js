$(function () {



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
    });


    $(".btn-recherche").on('click', function (event) {
        var keyword = $('#keyword').val();
        var branchId = $('#branch').attr("data-branch");
        $(".universes").hide();
        $("#branch" + branchId).show();
        $("#branch" + branchId + "> div").html('');
        $("#branch" + branchId + "> div").each(function () {
            var DIS = $(this);
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
                    DIS.fillGroupSelectable(univerLabel, response.data, 'panel-heading', 'panel-body', 'containerSelectable' + univerIndex, undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.');
                    $('#containerSelectable' + univerIndex).selectable(
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
        console.log(itemIds);
        var DIS = this;
        this.itemIds = itemIds;
        var universeIdCalling = $(this).closest('.panel').attr('data-universe');
        var branchId = $(this).closest('.panel').attr('data-branch');
        var universesToUpdate = $("[id^='groupSelectable'][data-branch='" + branchId + "'][data-universe!='" + universeIdCalling + "']");

        $.each(universesToUpdate, function (index, elem) {
            var levelId = $(elem).attr('data-universe');
            var params = {
                levelId: levelId,
                selectedClassification: DIS.itemIds.join(","),
                selectedLevelId: universeIdCalling
            };
            console.log(params);

            var univerIndex = parseFloat($(elem).attr('data-universe'));
            var univerLabel = $(elem).attr('data-label') + "\{NB_ELEM\}";
            $("#groupSelectable" + levelId).html('');
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
                    console.log("#groupSelectable" + levelId);
                    $("#groupSelectable" + levelId).fillGroupSelectable(univerLabel, response.data, 'panel-heading', 'panel-body', 'containerSelectable' + univerIndex, undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.');
                    $('#containerSelectable' + univerIndex).selectable(
                    {
                        stop: SelectedItems
                    });
                }
            });
        });



    };

});

//function SelectedItems(event, ui) {
//    itemIds = [];
//    $(".ui-selected").each(function (index, elem) {
//        itemIds.push($(elem).attr('data-id'));
//    });

//    var universeIdCalling = $(this).closest('.panel').attr('data-universe');
//    var branchId = $(this).closest('.panel').attr('data-branch');
//    var universesToUpdate = $("[id^='groupSelectable'][data-branch='" + branchId + "'][data-universe!='" + universeIdCalling + "']");

//    universesToUpdate.each(function (index, elem, itemsIds) {
//        console.log(elem);
//        console.log(itemsIds);
//        var levelId = $(elem).attr('data-universe');
//        //var params = {
//        //    levelId: levelId,
//        //    selectedClassification: $(itemsIds).join(),
//        //    selectedLevelId: universeIdCalling
//        //};
//    });


//    //$.ajax({
//    //    url: '/Universe/GetClassification',
//    //    contentType: 'application/json',
//    //    type: 'POST',
//    //    datatype: 'JSON',
//    //    data: JSON.stringify(params),
//    //    error: function (xmlHttpRequest, errorText, thrownError) {
//    //        alert("error");
//    //    },
//    //    success: function (response) {
//    //        DIS.fillGroupSelectable(univerLabel, response.data, 'panel-heading', 'panel-body', 'containerSelectable' + univerIndex, undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.');
//    //        $('#containerSelectable' + univerIndex).selectable(
//    //        {
//    //            stop: SelectedItems
//    //        });
//    //    }
//    //});
//};

function ShowSelection(elem) {
    var selectedItems = elem.getSelectableSelectedItems();
    var result = '';
    selectedItems.forEach(function (item) {
        result += ' #' + item.Value + ';' + item.Text + '\n';
    });
    alert('Selection : \n' + result);
};
