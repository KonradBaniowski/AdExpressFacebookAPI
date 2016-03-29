$(function () {

    $('.btn-green2.btn-circle').on('click', function () {
        var levelSrc = $('.panel-marche .ui-selectee.ui-selected');
        var tabSelected = $('ul > li[class="active"] > a');
        var indexTab = tabSelected.attr('data-tab');
        var idTab = "tab-" + indexTab;
        if (levelSrc.length == 1) {
            var universSrc = $('.ui-selectee.ui-selected').closest('.panel-default').attr('data-universe');
            if (universSrc == 14) {
                var universDst = $('.panel-body[data-tree=' + indexTab + '][data-level=' + universSrc + '] > ul');
                $('#collapse-' + universSrc + '-' + indexTab).collapse('show');
                $.each(levelSrc, function (index, value) {
                    var item = $(value).clone();
                    var find = false;

                    if (!find) {
                        var buttonSupp = $('<button/>');
                        buttonSupp.addClass('pull-right');
                        var icon = $('<i/>');
                        icon.addClass('fa fa-times-circle black text-base');
                        buttonSupp.append(icon);
                        item.append(buttonSupp);
                        universDst.append(item);
                    }
                });
            }
            else {
                bootbox.alert($('#Labels_ErrorNoSupport').val());
            }
        }
        else {
            bootbox.alert($('#Labels_ErrorItemExceeded').val());
        }
    });
});