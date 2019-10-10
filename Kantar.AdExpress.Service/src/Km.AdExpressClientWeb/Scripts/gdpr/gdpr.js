$(document).ready(function () {

    var policyUpdateDate = $('#PolicyUpdateDate').val();
    var cookiesNames = getCookiesNames();

    cookiesNames.forEach(function (element) {
        var cookieValue = getCookie(element);

        if (cookieValue.length > 0) {
            var creationDate = JSON.parse(cookieValue).creationDate;

            if (toDate(policyUpdateDate) > toDate(creationDate)) {
                document.cookie = "" + element + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;" + "; secure";
            }
        }
    });

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
        forceReInit: $('#ForceCookieReInit').val(),
        link: "/CookiePolicy",
        delay: 1000,
        expires: 395,
        uncheckBoxes: true,
        cookieTypes: [
             {
                 type: 'Statistics',
                 value: 'Statistics',
                 description: $('#CookieStatDesc').val()
             },
             {
                 type: 'Diagnostic',
                 value: 'Diagnostic',
                 description: $('#CookieDiagnosticDesc').val()
             }
        ]
    });

    function getCookiesNames() {
        var names = [];
        document.cookie.split(';').filter(function(c) {
            return c.trim().indexOf('cookieControlPrefs') === 0;
        }).map(function (c) {
            names.push(c.trim().split('=')[0]);
        });
        return names;
    }

    function toDate(dateStr) {
        var parts = dateStr.split("-");
        return new Date(parts[0], parts[1] - 1, parts[2], parts[3], parts[4], parts[5]);
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