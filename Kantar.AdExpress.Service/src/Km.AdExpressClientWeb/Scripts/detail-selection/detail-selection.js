
//append modal with detail selection
$("#load-detail-selection").on('click', function (e) {
    e.preventDefault();
    console.log('load-detail-selection');
    $.ajax({
        url: '/DetailSelection/GetDetailSelection',
        contentType: 'application/json',
        type: 'POST',
        datatype: 'JSON',
        error: function (xmlHttpRequest, errorText, thrownError) {
            bootbox.alert("An error occurred while processing your request.");
        },
        success: function (data) {
            $('#detail-selection .modal-body').html('');
            $('#detail-selection .modal-body').append(data);
            $('.treeview').each(function () {
                var tree = $(this);
                tree.treeview();
            })
        }
    });
});