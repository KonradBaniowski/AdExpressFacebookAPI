function GetHtmlSelectableFromObject(datas, classNameSelection, idSelection) {
    var grp = $('<ul/>');
    grp.attr('id', 'selectable' + idSelection)
    if (classNameSelection != undefined)
        grp.addClass(classNameSelection);

    if (datas.length > 0) {
        for (var i = 0; i < datas.length; i++) {
            var item = $('<li/>');
            item.val(datas[i].Id);
            item.attr('data-id', datas[i].Id)
            item.text(datas[i].Label);
            item.appendTo(grp);
        }
    }
    return grp;
}

jQuery.fn.extend({
    updateGroup: function (titleText, datas, total, classNameTitle, idSelection, classNameSelection, nbElemMax, nbElemMaxText, divHtmlList, divTitleHtml) {
        divHtmlList.html('');
        divTitleHtml.html('');
        var nbElemHtml = $('<span/>');
        nbElemHtml.addClass('badge bg-blue');
        titleText = titleText.replace('{NB_ELEM}', '       {NB_ELEM}');
        var listElem = undefined;
        var warningHtml = undefined;
        if (datas != undefined) {
            if (total > nbElemMax) {
                //listElem = GetHtmlSelectableFromObject(datas.slice(0, nbElemMax), classNameSelection, idSelection);
                listElem = GetHtmlSelectableFromObject(datas, classNameSelection, idSelection);
                nbElemHtml.text(nbElemMax);
                if (nbElemMaxText != undefined) {
                    warningHtml = $('<small/>');
                    warningHtml.text(nbElemMaxText.replace('{NB_ELEM_MAX}', nbElemMax).replace('{NB_ELEM}', total));
                }
            }
            else {
                nbElemHtml.text(total);
                listElem = GetHtmlSelectableFromObject(datas, classNameSelection, idSelection);
            }
        }

        titleText = titleText.replace('{NB_ELEM}', $("<div>").append(nbElemHtml.clone()).html());

        var headerTitle = $('<h6/>');

        //BUTTON 
        var buttonAdd = $('<button/>', { class: 'btn-clean add-all'});
        var iconAdd = $('<i/>', { class: 'fa fa-plus-square blue' });
        buttonAdd.append(iconAdd);
        var buttonRem = $('<button/>', { class: 'btn-clean rem-all' });
        var iconRem = $('<i/>', { class: 'fa fa-minus-square blue' });
        buttonRem.append(iconRem);
        var headerButtonRight = $('<div/>')
        headerButtonRight.addClass('pull-right');
        headerButtonRight.append(buttonRem).append('&nbsp;').append(buttonAdd);


        divTitleHtml.append(headerTitle.addClass('famille blue uppercase').append(titleText)).append(headerButtonRight);

        if (warningHtml != undefined)
            divTitleHtml.append(warningHtml);
        
        if (listElem != undefined) {
            listElem.appendTo(divHtmlList);
        }
    },
    fillSelectable: function (datas, classNameSelection, idSelection, nbElemMax, nbElemMaxText) {
        var html = GetHtmlSelectableFromObject(datas, classNameSelection, idSelection);
        if (html != undefined)
            html.appendTo(this);
        else
            this.html('');

    },
    fillGroupSelectable: function (titleText, datas, total, classNameTitle, classNameDivSelection, idSelection, classNameSelection, nbElemMax, nbElemMaxText) {
        var divHtmlList = $('<div/>');

        if (classNameDivSelection != undefined)
            divHtmlList.addClass(classNameDivSelection);
        divHtmlList.attr('id', 'containerSelectable' + idSelection);

        var divTitleHtml = $('<div/>');
        if (classNameTitle != undefined)
            divTitleHtml.addClass(classNameTitle);
        divTitleHtml.appendTo(this);
        this.updateGroup(titleText, datas, total, classNameTitle, idSelection, classNameSelection, nbElemMax, nbElemMaxText, divHtmlList, divTitleHtml);
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