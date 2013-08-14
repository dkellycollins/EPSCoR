var initDataTable = function (tableId, tableName) {
    $('#' + tableId).dataTable({
        "bFilter": false,
        "bSort":false,
        "sPaginationType": "full_numbers",
        "bServerSide": true,
        "fnServerParams": function (aoData) {
            aoData.push({ "name": "tableName", "value" : tableName });
        },
        "sAjaxSource": "/Tables/DataTableDetails"
    });
};