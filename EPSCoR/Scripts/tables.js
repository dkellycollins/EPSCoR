function toggleTableView(divId) {
    var $context = $('#' + divId),
        $details = $('#' + divId + 'Details')
        $icon = $('.icon', $context);
    $details.slideToggle();
    $icon.toggleClass('icon-plus').toggleClass('icon-minus');

    if ($context.has('img')) {
        loadTableDetails($details, divId);
    }
}

function loadTableDetails($context, tableName) {
    $context.load('/tables/details/' + tableName, function () {
        $('.table', $context).dataTable({
            "bFilter": false,
            "bSort": false,
            "sPaginationType": "full_numbers",
            "bServerSide": true,
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "tableName", "value": tableName });
            },
            "sAjaxSource": "/Tables/DataTableDetails"
        });
    });
}

function deleteTable(tableName) {
    yesnodialog('Yes', 'No', 'Confirm delete', 'Are you sure you want to delete ' + tableName + '?', function (result) {
        if (result) {
            $('#delete-' + tableName).submit();
            $('#' + tableName).slideUp(500, function () {
                $('#' + tableName).remove();
            });
        }
    });
}

function addTable(tableIndex) {
    //Expecting tableindex to be a json representation of the Model class TableIndex.
    //Add a table index to the tables div.
}

function updateTable(tableIndex) {
    //Update a table div on the page with the info in tableIndex.
}

function toggleButtons(divId, enable) {
    $('#' + divId + ' .btn').toggleAttr('disabled', enable);
}