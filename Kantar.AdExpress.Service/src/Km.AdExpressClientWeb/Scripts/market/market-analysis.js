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