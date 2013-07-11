function toggleTableView(divId) {
    var $context = $('#' + divId),
        $details = $('#' + divId + 'Details')
        $icon = $('.icon', $context);
    $details.slideToggle();
    $icon.toggleClass('icon-plus').toggleClass('icon-minus');

    if ($context.has('img').length) {
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
            "sAjaxSource": "/Tables/DataTableDetails",
            "sDom": '<"top"flp>rt<"bottom"i><"clear">'
        });
        $context.prepend('<hr />');
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
    $('#tables').append(tmpl('tableIndexTmpl', tableIndex));
    toggleButtons(tableIndex.Name, !tableIndex.Processed);

    window.setTimeout(function () {
        $("#"+ tableIndex.Name).addClass('in');
    }, 100);

}

function addTables(tableIndexes) {
    $.each(tableIndexes, function (index, tableIndex) {
        addTable(tableIndex);
    });
}

function updateTable(tableIndex) {
    var $title = $('#' + tableIndex.Name + ' b');
    if (tableIndex.Processed) {
        $title.text(tableIndex.Name);
    } else {
        $title.text(tableIndex.Name + ' - ' + tableIndex.Status);
    }
    toggleButtons(tableIndex.Name, !tableIndex.Processed);
}

function toggleButtons(divId, enable) {
    $('#' + divId + ' .btn').toggleAttr('disabled', enable);
}