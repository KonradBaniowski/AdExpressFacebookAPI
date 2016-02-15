if (typeof jQuery === "undefined") { throw new Error("Media Client selector requires jQuery") }

$(function () {

    if ($('#Multiple').val() == "True") {
        var idList = [];
        $('.tuile-medias[data-attr-id]').on('click', selectMultiple)

        //CHECKBOX
        var idCommon = [];
        $('[name="HiddenIntList"]').each(function (index) {
            idCommon.push($(this).attr("value"));
        });
        $('.tuile-medias[data-attr-id]').each(function (i, e) {
            var elem = $(e);
            var indexElem = elem.attr('data-attr-id');
            var index = $.inArray(indexElem, idCommon);
            if (index == -1)
                elem.attr("data-grp", "A");
            else
                elem.attr("data-grp", "B");
        });

        $(':checkbox').on('change', preselection)

        //FOCUS
        $('[data-grp]').on('mouseenter', highlight);
        $('[data-grp]').on('mouseleave', unhighlight);
    }
    else {
        var idList;
        $('.tuile-medias[data-attr-id]').on('click', selectUnique)
    }

    function highlight() {
        console.log("az");
        var grp = $(this).attr('data-grp');
        $('[data-grp="' + grp + '"]').addClass('highlight');
    }

    function unhighlight() {
        var grp = $(this).attr('data-grp');
        $('[data-grp="' + grp + '"]').removeClass('highlight');
    }

    function selectUnique() {
        $(this).toggleClass("tuile-medias tuile-medias-active")
        var id = $(this).attr("data-attr-id");
        if (idList !== null && idList !== undefined) {
            $('.tuile-medias-active[data-attr-id="' + idList + '"]').toggleClass("tuile-medias tuile-medias-active")
        }
        idList = id;
    }

    function selectMultiple(e) {
        $(this).toggleClass("tuile-medias tuile-medias-active")
        var id = $(this).attr("data-attr-id");
        var index = $.inArray(id, idList);
        if (index == -1) {
            idList.push(id);
        } else {
            idList.splice(index, 1);
        }
        console.log(idList);
    }

    function preselection() {
        $('.tuile-medias-active').toggleClass("tuile-medias tuile-medias-active")
        var attr = $(this).attr('checked');
        if (typeof attr !== typeof undefined && attr !== false) {
            $.each(idCommon, function (index, value) {
                $('.tuile-medias-active[data-attr-id="' + value + '"]').toggleClass("tuile-medias tuile-medias-active")
            });
            $(this).removeAttr("checked");
        }
        else {
            $.each(idCommon, function (index, value) {
                $('.tuile-medias[data-attr-id="' + value + '"]').toggleClass("tuile-medias tuile-medias-active")
            });
            $(this).attr("checked", "");
        }
    }
});