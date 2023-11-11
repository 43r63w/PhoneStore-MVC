var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#orderTable').DataTable({
        "ajax": { url: '/admin/order/getall' },
        "columns": [
            { "data": "id", "width": "15%" },
            { "data": "name", "width": "15%" },
           { "data": "orderStatus", "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/order/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>                              
                    </div>`
                },
                "width": "25%"
            }
        ]
    });
}

