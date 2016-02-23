if (typeof jQuery === "undefined") { throw new Error("Media Client selector requires jQuery") }
$(function () {
    $('#Media').on('click', function (e) {
        e.preventDefault();
        var fileName = location.href.split("/").slice(-1);
        var action = "";
        var params;
        if (fileName == "MarketSelection") {
            action = "SavearketSelection";
            params = {
                selectedMedia: idList,
                nextStep: "Index"
            };
        }
        //switch (fileName)
        //{
        //    case "Index":
        //        action = "Index";
        //        alert(fileName + action);
        //        break
        //    case media:
        //        action = "SaveMediaSelection";
        //        alert("Hello");
        //        //params = {
        //        //    selectedMedia: idList,
        //        //    nextStep: "Index"
        //        //};
        //        break;
        //    case "PeriodSelection":
        //        action = "PeriodSelection";
        //        alert(fileName + action);
        //        break;
        //    case "Results":
        //        action = "Results";
        //        alert(fileName + action);
        //        break;
        //    }
        //var params = {
        //    selectedMedia: idList,
        //    nextStep: "Index"
        //};
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

    $('#Dates').on('click', function (e) {
        e.preventDefault();
        var fileName = location.href.split("/").slice(-1);
        var action = "";
        var params;
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
            action = "SaveMarketSelection";

        }
        if (fileName == "MediaSelection") {
            action = "SaveMediaSelection";
            params = {
                selectedMedia: idList,
                nextStep: "Results"
            };
        }

        if (fileName == "PeriodSelection") {
            action = "CalendarValidation";
            //params = {                        ||
            //    selectedMedia: idList,        ||  To Do
            //    nextStep: "Results"           ||
            //};                                ||
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

});//end of file