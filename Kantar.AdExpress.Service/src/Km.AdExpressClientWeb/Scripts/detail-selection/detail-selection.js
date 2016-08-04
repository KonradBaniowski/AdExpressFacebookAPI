
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
            $("#DetailSelectionLoader").hide();
            $('#detail-selection .modal-body .datas').html('');
            $('#detail-selection .modal-body .datas').append(data);
            $('.treeview').each(function () {
                var tree = $(this);
                tree.treeview();
            })
        }
    });
});

$('#myModal').on('hidden.bs.modal', function (e) {
    $('#detail-selection .modal-body .datas').html('');
    $("#DetailSelectionLoader").show();
})