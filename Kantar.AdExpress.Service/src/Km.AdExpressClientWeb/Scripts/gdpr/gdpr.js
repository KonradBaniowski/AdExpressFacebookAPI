$(document).ready(function () {
    $('body').ihavecookies({
        title: $('#Labels_CookiesTitle').val(),
        message: $('#Labels_CookiesMessage').val(),
        moreInfoLabel: $('#Labels_MoreInfoLabel').val().toLowerCase(),
        acceptBtnLabel: $('#Labels_AcceptBtnLabel').val(),
        fixedCookieTypeLabel: $('#Labels_FixedCookieTypeLabel').val(),
        fixedCookieTypeDesc: $('#Labels_FixedCookieTypeDesc').val(),
        link: "/CookiePolicy",
        delay: 1000,
        uncheckBoxes: true,
        displayAdvancedBtn: false,
        cookieTypes: []
    });
});