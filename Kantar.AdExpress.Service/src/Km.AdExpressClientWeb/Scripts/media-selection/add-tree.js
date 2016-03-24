$(function () {
    console.log('load add-tree presents-absents');

    $('#add-tree').on('click', function () {
        //Check si ya des elem a lock
        var nbElemToLock = $('.tab-content .items-famille > li').length;
        if (nbElemToLock >= 1) {
            var nbIncludes = $('.panel-heading-choix.nav-tabs-alt').find('li').length;
            $('[data-tab=' + nbIncludes + ']').append('<i class="fa fa-lock"></i>');
            //limit concurrent a 5 
            if (nbIncludes < 5) {

                $('.panel-heading-choix.nav-tabs-alt').find('li[class="active"]').attr('class', '');
                var idNext = nbIncludes + 1
                var ulSource = $('.nav.nav-tabs');
                var liHtml = $('<li/>');
                liHtml.attr('class', 'active');
                var aHtml = $('<a/>');
                //SET LABEL 
                aHtml.text('Concurrent');
                aHtml.attr('data-target', '#tab' + idNext);
                aHtml.attr('data-tab', idNext);
                aHtml.attr('data-toggle', 'tab');
                aHtml.appendTo(liHtml);
                liHtml.appendTo(ulSource);

                var panelHtml = $('.panel-group.panel-group-results [id="tree-' + nbIncludes + '"]').clone();
                panelHtml.attr('id', 'tree-' + idNext);
                $.each(panelHtml.find('a[data-parent]'), function (index, value) {
                    var href = $(value).attr('href');
                    href = href.slice(0, -1);
                    href += idNext;
                    $(value).attr('href', href);
                });
                $.each(panelHtml.find('.panel-collapse'), function (index, value) {
                    var id = $(value).attr('id');
                    id = id.slice(0, -1);
                    id += idNext;
                    $(value).attr('id', id);
                });
                $.each(panelHtml.find('.panel-collapse'), function (index, value) {
                    var dataTree = $(value).attr('data-tree');
                    $(value).attr('data-tree', idNext);
                });
                $('.tabpanel').append(panelHtml);

            }
            else {
                bootbox.alert('too much');
            }
        }
        else
        {
            bootbox.alert('rien a locker');
        }
    })
});