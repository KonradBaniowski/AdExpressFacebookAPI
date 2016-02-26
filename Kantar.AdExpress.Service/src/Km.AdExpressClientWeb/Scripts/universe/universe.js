function BuildJson(title, nbElem) {
    var jsonResult = '[{"Title":"' + title + '","Values":[';
    for (var i = 0 ; i < nbElem ; i++) {
        if (i > 0)
            jsonResult += ',';
        jsonResult += '{"Id":' + i + ', "Text": "Item ' + i + '"}';
    }
    jsonResult += ']}]';

    return jsonResult;
}

$(function () {


    var jsonResult1 = BuildJson('Groupe de sociétés 1 \{NB_ELEM\}', 35);
    //var jsonResult2 = BuildJson('Groupe de sociétés 2 \{NB_ELEM\}', 35);
    //var jsonResult3 = BuildJson('Groupe de sociétés 3 \{NB_ELEM\}', 1001);


    var jsonParse1 = JSON.parse(jsonResult1)[0];

    $("#groupSelectable1").fillGroupSelectable(jsonParse1.Title, jsonParse1.Values, 'panel-heading', 'panel-body', 'containerSelectable1', undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.')

    $("#containerSelectable1").selectable(
    {
        stop: function () {
            ShowSelection($(this));
        }
    });

    //var jsonParse2 = JSON.parse(jsonResult2)[0];

    //$("#groupSelectable2").fillGroupSelectable(jsonParse2.Title, jsonParse2.Values, 'panel-heading', 'panel-body', 'containerSelectable2', undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.')

    //$("#containerSelectable2").selectable(
    //{
    //    stop: function () {
    //        ShowSelection($(this));
    //    }
    //});

    //var jsonParse3 = JSON.parse(jsonResult3)[0];

    //$("#groupSelectable3").fillGroupSelectable(jsonParse3.Title, jsonParse3.Values, 'panel-heading', 'panel-body', 'containerSelectable3', undefined, 1000, '{NB_ELEM_MAX} éléments sur {NB_ELEM}. Affinez votre recherche.')

    //$("#containerSelectable3").selectable(
    //{
    //    stop: function () {
    //        ShowSelection($(this));
    //    }
    //});

});

function ShowSelection(elem) {
    var selectedItems = elem.getSelectableSelectedItems();

    var result = '';
    selectedItems.forEach(function (item) {
        result += ' #' + item.Value + ';' + item.Text + '\n';
    });

    alert('Selection : \n' + result);
}