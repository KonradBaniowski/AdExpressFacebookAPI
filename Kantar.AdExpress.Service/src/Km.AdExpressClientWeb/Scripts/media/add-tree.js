$(function () {
 
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
                aHtml.attr('data-target', '#tab-' + idNext);
                aHtml.attr('data-tab', idNext);
                aHtml.attr('data-toggle', 'tab');
                aHtml.appendTo(liHtml);
                liHtml.appendTo(ulSource);

                var ref = $('#tab-' + nbIncludes + '');
                var tabHtml = ref.clone();

                //LOCK OLD ONE
                ref.attr('lock', 'true');
                ref.attr('class', 'tab-pane');
                tabHtml.attr('id', 'tab-' + idNext);
                var panelHtml = tabHtml.find('.panel-group.panel-group-results[id="tree-' + nbIncludes + '"]');
                panelHtml.attr('id', 'tree-' + idNext);
               
                panelHtml.find('.items-famille').empty();
                $.each(panelHtml.find('a[data-parent]'), function (index, value) {
                    var href = $(value).attr('href');
                    href = href.slice(0, -1);
                    href += idNext;
                    $(value).attr('href', href);
                    var data = $(value).attr('data-parent');
                    data = data.slice(0, -1);
                    data += idNext;
                    $(value).attr('data-parent', '#tree-'+ idNext);
                });
                $.each(panelHtml.find('.panel-collapse'), function (index, value) {
                    var id = $(value).attr('id');
                    id = id.slice(0, -1);
                    id += idNext;
                    $(value).attr('id', id);
                });
                $.each(panelHtml.find('.panel-body'), function (index, value) {
                    var dataTree = $(value).attr('data-tree');
                    $(value).attr('data-tree', idNext);
                });
                panelHtml.find('[id^=collapse-14]').collapse('hide');
                $('.tab-content').append(tabHtml);
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