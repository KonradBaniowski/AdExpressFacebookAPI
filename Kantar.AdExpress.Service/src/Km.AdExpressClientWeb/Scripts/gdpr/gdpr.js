$(document).ready(function () {
    $('body').ihavecookies({
        title: $('#CookiesTitle').val(),
        message: $('#CookiesMessage').val(),
        moreInfoLabel: $('#MoreInfoLabel').val().toLowerCase(),
        acceptBtnLabel: $('#AcceptBtnLabel').val(),
        advancedBtnLabel: $('#AdvancedBtnLabel').val(),
        cookieTypesTitle: $('#CookieTypesTitle').val(),
        fixedCookieTypeLabel: $('#FixedCookieTypeLabel').val(),
        fixedCookieTypeDesc: $('#FixedCookieTypeDesc').val(),
        siteLanguage: $('#SiteLanguage').val(),
        isStoredInDb: false,
        link: "/CookiePolicy",
        delay: 1000,
        expires: 395,
        uncheckBoxes: true,
        cookieTypes: [
             {
                 type: $('#CookieStat').val(),
                 value: $('#CookieStat').val(),
                 description: $('#CookieStatDesc').val()
             },
             {
                 type: $('#CookieDiagnostic').val(),
                 value: $('#CookieDiagnostic').val(),
                 description: $('#CookieDiagnosticDesc').val()
             }
        ]
    });

    var policyUpdateDate = $('#PolicyUpdateDate').val();
    var cookieValue = getCookie("cookieControlPrefs");

    if (cookieValue.length > 0) {
        var creationDate = JSON.parse(cookieValue).creationDate;

        if (toDate(policyUpdateDate) > toDate(creationDate)) {
            document.cookie = "cookieControl=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
            document.cookie = "cookieControlPrefs=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        }
    }

    function toDate(dateStr) {
        var parts = dateStr.split("-");
        return new Date(parts[2], parts[1] - 1, parts[0]);
    }

    function getCookie(name) {
        var cookie_name = name + "=";
        var decodedCookie = decodeURIComponent(document.cookie);
        var ca = decodedCookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(cookie_name) === 0) {
                return c.substring(cookie_name.length, c.length);
            }
        }
        return false;
    };
});