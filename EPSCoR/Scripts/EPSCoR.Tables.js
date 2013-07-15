/* Dependencies:
    jQuery
    JavaScriptTemplates
    DataTables
    jQueryExt.js
    dialogs.js
*/

EPSCoR.Tables = {
    
    toggleTableView: function(divId) {
        var $context = $('#' + divId),
            $details = $('#' + divId + 'Details')
            $icon = $('.icon', $context);
        $details.slideToggle();
        $icon.toggleClass('icon-plus').toggleClass('icon-minus');

        if ($context.has('img').length) {
            EPSCoR.Tables.loadTableDetails($details, divId);
        }
    },

    loadTableDetails: function($context, tableName) {
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
    },

    deleteTable: function(tableName) {
        EPSCoR.Dialogs.yesNoDialog('Yes', 'No', 'Confirm delete', 'Are you sure you want to delete ' + tableName + '?', function (result) {
            if (result) {
                $('#delete-' + tableName).submit();
                $('#' + tableName).slideUp(500, function () {
                    $('#' + tableName).remove();
                });
            }
        });
    },

    addTable: function(tableIndex) {
        //Expecting tableindex to be a json representation of the Model class TableIndex.
        $('#tables').append(tmpl('tableIndexTmpl', tableIndex));
        EPSCoR.Tables.toggleButtons(tableIndex.Name, !tableIndex.Processed);

        window.setTimeout(function () {
            $("#"+ tableIndex.Name).addClass('in');
        }, 100);

    },

    addTables: function(tableIndexes) {
        $.each(tableIndexes, function (index, tableIndex) {
            EPSCoR.Tables.addTable(tableIndex);
        });
    },

    updateTable: function(tableIndex) {
        var $title = $('#' + tableIndex.Name + ' b');
        if (tableIndex.Processed) {
            $title.text(tableIndex.Name);
        } else {
            $title.text(tableIndex.Name + ' - ' + tableIndex.Status);
        }
        EPSCoR.Tables.toggleButtons(tableIndex.Name, !tableIndex.Processed);
    },

    toggleButtons: function(divId, enable) {
        $('#' + divId + ' .btn').toggleAttr('disabled', enable);
    }
};