if (typeof jQuery === "undefined") { throw new Error("Period Selector requires jQuery") }


$(function () {

    $("#begin-date").datepicker({
        autoSize: true,
        altField: "#alt-begin-date",
        dayNamesMin: ["Di", "Lu", "Ma", "Me", "Je", "Ve", "Sa"],
        maxDate: new Date(),
        minDate: "-3y",
        showOtherMonths: true,
        changeMonth: true,
        changeYear: true
    }, $.datepicker.regional["fr"]);

    $("#end-date").datepicker({
        autoSize: true,
        altField: "#alt-end-date",
        dayNamesMin: ["Di", "Lu", "Ma", "Me", "Je", "Ve", "Sa"],
        maxDate: new Date(),
        minDate: "-3y",
        showOtherMonths: true,
        changeMonth: true,
        changeYear: true
    }, $.datepicker.regional["fr"]);
});