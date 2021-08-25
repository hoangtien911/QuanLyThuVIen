// Call the dataTables jQuery plugin
$(document).ready(function() {
    $('#dataTable').DataTable({
        "aoColumnDefs": [
            { 'bSortable': false, 'aTargets': [1] }
        ]
    });
});

function ConfirmSave(ms) {
    var r = confirm(ms);
    if (r == false) {
        window.open("https://localhost:44317/Admin/ql_Sach/ListBooks");
        window.close(this);
    }
}