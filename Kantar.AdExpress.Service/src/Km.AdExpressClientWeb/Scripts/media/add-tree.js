 $('#add-tree').on('click', function () {
        //Check si ya des elem a lock
        var nbElemToLock = $('.tab-content .items-famille > li').length;
        if (nbElemToLock >= 1) {
            var nbIncludes = $('.panel-heading-choix.nav-tabs-alt').find('li').length;
            //limit concurrent a 6 
            if (nbIncludes < 7) {
                $('[data-tab=' + nbIncludes + ']').append('<i class="fa fa-lock"></i>');
                $('.panel-heading-choix.nav-tabs-alt').find('li[class="active"]').attr('class', '');
                var nextId = nbIncludes;
                var ulSource = $('.nav.nav-tabs');
                var liHtml = $('<li/>');
                liHtml.attr('class', 'active');
                var aHtml = $('<a/>');
                //SET LABEL
                var concurrent=$('#Labels_Concurrent').val();
                aHtml.text(concurrent);
                aHtml.attr('data-target', '#tab-' + nextId);
                aHtml.attr('id', 'tree-' + nextId);
                aHtml.attr('data-tab', nextId);
                aHtml.attr('data-toggle', 'tab');
                aHtml.appendTo(liHtml);
                liHtml.appendTo(ulSource);

                var ref = $('#tab-0');
                var tabHtml = ref.clone();

                //LOCK OLD ONE
                ref.attr('lock', 'true');
                ref.attr('class', 'tab-pane');
                tabHtml.attr('id', 'tab-' + nextId);
                var panelHtml = tabHtml.find('.panel-group.panel-group-results[id=tree-0]');
                panelHtml.attr('id', 'tree-' + nextId);
               
                panelHtml.find('.items-famille').empty();
                $.each(panelHtml.find('a[data-parent]'), function (index, value) {
                    var href = $(value).attr('href');
                    href = href.slice(0, -1);
                    href += nextId;
                    $(value).attr('href', href);
                    var data = $(value).attr('data-parent');
                    data = data.slice(0, -1);
                    data += nextId;
                    $(value).attr('data-parent', '#tree-'+ nextId);
                });
                $.each(panelHtml.find('.panel-collapse'), function (index, value) {
                    var id = $(value).attr('id');
                    id = id.slice(0, -1);
                    id += nextId;
                    $(value).attr('id', id);
                });
                $.each(panelHtml.find('.panel-body'), function (index, value) {
                    var dataTree = $(value).attr('data-tree');
                    $(value).attr('data-tree', nextId);
                });
                panelHtml.find('[id^=collapse-14]').collapse('hide');
                $('.tab-content').append(tabHtml);
            }
            else {
                $('#add-tree').off();
            }
        }
        else
        {
            var message =  $('#Labels_ErrorMininumInclude').val();
            bootbox.alert(message);
        }
    })