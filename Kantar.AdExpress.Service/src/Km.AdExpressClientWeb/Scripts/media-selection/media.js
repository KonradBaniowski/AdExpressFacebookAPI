if (typeof jQuery === "undefined") { throw new Error("Media Client selector requires jQuery") }

$(function () {
    var idList = [];
    if ($('#Multiple').val() == "True") {
        
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

    $('#btnSubmitMediaSelection').on('click', function (e) {
        e.preventDefault();
        var params = {
            selectedMedia:idList,
            nextStep:"PeriodSelection"
        };
        $.ajax({
            url: '/MediaSchedule/SaveMediaSelection',
            contentType: 'application/json',
            type: 'POST',
            datatype: 'JSON',
            data: JSON.stringify(params),
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                if (data != null) {
                    document.location = data.RedirectUrl;
                }
            }
        });
    });

    $('#Media').on('click', function (e) {
        e.preventDefault();
        var fileName = location.href.split("/").slice(-1);
        var action = "";
        var params;
        if (fileName == "MediaSchedule") { //this the market selection page
            action = "SaveMarketSelection";
            params = {
                nextStep: "MediaSelection"
            };
        }
        $.ajax({
            url: '/MediaSchedule/' + action,
            contentType: 'application/json',
            type: 'POST',
            datatype: 'JSON',
            data: JSON.stringify(params),
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                if (data != null) {
                    document.location = data.RedirectUrl;
                }
            }
        });
    });
    $('#Market').on('click', function (e) {
        e.preventDefault();
        var fileName = location.href.split("/").slice(-1);
        var action = "";
        var params;
        if (fileName == "MediaSelection")
        {
            action = "SaveMediaSelection";
            params = {
                selectedMedia: idList,
                nextStep: "Index"
            };
        }
        $.ajax({
            url: '/MediaSchedule/'+action,
            contentType: 'application/json',
            type: 'POST',
            datatype: 'JSON',

            data: JSON.stringify(params),
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                if (data != null) {
                    document.location = data.RedirectUrl;
                }
            }
        });
    });

    $('#Dates').on('click', function (e) {
        e.preventDefault();
        var fileName = location.href.split("/").slice(-1);
        var action = "";
        var params;
        if (fileName == "Index") {
            action = "SaveMarketSelection";
            params = {
                nextStep: "PeriodSelection"
            };

        }
        if (fileName == "MediaSelection") {
            action = "SaveMediaSelection";
            params = {
                selectedMedia: idList,
                nextStep: "PeriodSelection"
            };
        }

        $.ajax({
            url: '/MediaSchedule/' + action,
            contentType: 'application/json',
            type: 'POST',
            datatype: 'JSON',

            data: JSON.stringify(params),
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                if (data != null) {
                    document.location = data.RedirectUrl;
                }
            }
        });
    });

    $('#Results').on('click', function (e) {
        e.preventDefault();
        var fileName = location.href.split("/").slice(-1);
        var action = "";
        var params;
        if (fileName == "Index") {
            action = "SaveMarketSelection",
            nextStep="Results"

        }
        if (fileName == "MediaSelection") {
            action = "SaveMediaSelection";
            params = {
                selectedMedia: idList,
                nextStep: "Results"
            };
        }
        $.ajax({
            url: '/MediaSchedule/' + action,
            contentType: 'application/json',
            type: 'POST',
            datatype: 'JSON',

            data: JSON.stringify(params),
            error: function (xmlHttpRequest, errorText, thrownError) {
            },
            success: function (data) {
                if (data != null) {
                    document.location = data.RedirectUrl;
                }
            }
        });
    });
});