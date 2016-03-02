function GetHtmlSelectableFromObject(datas, classNameSelection, idSelection) {
    var grp = $('<ul/>');
    grp.attr('id', 'selectable' + idSelection)
    if (classNameSelection != undefined)
        grp.addClass(classNameSelection);

    if (datas.length > 0) {
        for (var i = 0; i < datas.length; i++) {
            var item = $('<li/>');
            item.val(datas[i].IdItem);
            item.attr('data-id', datas[i].IdItem)
            item.text(datas[i].Label);
            item.appendTo(grp);
        }
    }
    return grp;
}

jQuery.fn.extend({
    fillSelectable: function (datas, classNameSelection, idSelection) {
        var html = GetHtmlSelectableFromObject(datas, classNameSelection, idSelection);
        if (html != undefined)
            html.appendTo(this);
        else
            this.html('');
    },
    fillGroupSelectable: function (titleText, datas, classNameTitle, classNameDivSelection, idSelection, classNameSelection, nbElemMax, nbElemMaxText) {
        var divHtmlList = $('<div/>');

        if (classNameDivSelection != undefined)
            divHtmlList.addClass(classNameDivSelection);
        divHtmlList.attr('id', 'containerSelectable' + idSelection);

        var nbElemHtml = $('<span/>');
        nbElemHtml.addClass('badge bg-blue');
        var listElem = undefined;
        var warningHtml = undefined;

        if (datas.length > nbElemMax) {
            listElem = GetHtmlSelectableFromObject(datas.slice(0, nbElemMax), classNameSelection, idSelection);
            nbElemHtml.text(nbElemMax);
            if (nbElemMaxText != undefined) {
                warningHtml = $('<small/>');
                warningHtml.text(nbElemMaxText.replace('{NB_ELEM_MAX}', nbElemMax).replace('{NB_ELEM}', datas.length));
            }
        }
        else {
            nbElemHtml.text(datas.length);
            listElem = GetHtmlSelectableFromObject(datas, classNameSelection, idSelection);
        }

        titleText = titleText.replace('{NB_ELEM}', $("<div>").append(nbElemHtml.clone()).html());

        var divTitleHtml = $('<div/>');
        var headerTitle = $('<h6/>');

        if (classNameTitle != undefined)
            divTitleHtml.addClass(classNameTitle);
        divTitleHtml.append(headerTitle.addClass('famille blue uppercase').append(titleText));

        if (warningHtml != undefined)
            divTitleHtml.append(warningHtml);
        divTitleHtml.appendTo(this);

        if (listElem != undefined) {
            listElem.appendTo(divHtmlList);
        }

        divHtmlList.appendTo(this);
    },
    getSelectableSelectedItems: function () {
        var result = [];
        $(".ui-selected", this).each(function () {
            var val = $(this).val();
            var text = $(this).text();
            result.push({ Value: $(this).val(), Text: $(this).text() })
        });
        return result;

    },
    getSelectableSelectedItems2: function () {
        var result = [];
        var items = this[0].getElementsByTagName("li");

        for (var i = items.length; i--;) {
            if ((' ' + items[i].className + ' ').indexOf(' ui-selected ') > -1) {
                result.push({ Value: items[i].value, Text: items[i].innerHTML })
            }
        }
        return result;
    }
});