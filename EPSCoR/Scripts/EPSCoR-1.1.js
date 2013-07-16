/* jslint unparam: true, todo: true */
/* global $ window tmpl*/

/* Plugin Dependencies:
    jQuery
    jQueryUI
    JavaScriptTemplates
    DataTables
    SignalR
*/

var EPSCoR = EPSCoR || {};

$(function () {
    "use strict";

    EPSCoR = {
        /* Defines functions to interact with alerts on the page. 
        */
        Alerts: {
            //Adds an alert to the alerts div.
            addAlert: function (message, header, alertType) {
                var alertInfo = {
                    "type": alertType,
                    "header": header,
                    "message": message
                };

                $('#alerts').append(tmpl('alertTmpl', alertInfo));

                //Delay the fade in.
                window.setTimeout(function () {
                    $("#alerts .alert").addClass('in');
                }, 100);
            }
        },

        /* Sets up and provides functions to interact with dialogs on the page. 
        */
        Dialogs: {
            //Initializes all dialogs on the page.
            init: function () {
                $('div[id^="dialog-"]').dialog({
                    autoOpen: false,
                    resizable: true,
                    modal: true,
                    height: "auto",
                    width: "auto",
                    show: {
                        effect: "clip",
                        duration: 500
                    },
                    hide: {
                        effect: "clip",
                        duration: 500
                    }
                });
            },
            //Opens the dialog with the given id.
            openDialog: function (id, options) {
                var $dialog = $('#' + id);
                if (options.fillScreen) {
                    $dialog.dialog("option", "height", window.innerHeight * 0.8);
                    $dialog.dialog("option", "width", window.innerWidth * 0.8);
                }

                $dialog.dialog('open');

                if (options.url) {
                    $dialog.load(options.url);
                }
            },
            
            //Displays a dialog with yes and no options. Executes on close when either yes or no is clicked.
            yesNoDialog: function (title, text, onClose) {
                var btns = {};
                btns['Yes'] = function () {
                    $(this).dialog("close");
                    onClose(true);
                };
                btns['No'] = function () {
                    $(this).dialog("close");
                    onClose(false);
                };
                $("<div><span>" + text + "</span></div>").dialog({
                    autoOpen: true,
                    title: title,
                    modal: true,
                    buttons: btns
                });
            }
        },
        /* Defines functions for interacting with the tables on the page.
        */
        Tables: {
            //Displays or hides the table. Assumes the table has a div with the id of the given id plus 'Details'. If the details div contains an img tag then will attempt to load the table.
            toggleTableView: function (divId) {
                var $context = $('#' + divId),
                    $details = $('#' + divId + 'Details'),
                    $icon = $('.icon', $context);
                $details.slideToggle();
                $icon.toggleClass('icon-plus').toggleClass('icon-minus');

                if ($context.has('img').length) {
                    EPSCoR.Tables.loadTableDetails($details, divId);
                }
            },
            //Loads the given table into the given context. Uses Datatables to initalize the table.
            loadTableDetails: function ($context, tableName) {
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
            //Removes the table from the page.
            deleteTable: function (tableName) {
                EPSCoR.Dialogs.yesNoDialog('Confirm delete', 'Are you sure you want to delete ' + tableName + '?', function (result) {
                    if (result) {
                        $('#delete-' + tableName).submit();
                        $('#' + tableName).slideUp(500, function () {
                            $('#' + tableName).remove();
                        });
                    }
                });
            },
            //Adds a new table to the page.
            addTable: function (tableIndex) {
                //Expecting tableindex to be a json representation of the Model class TableIndex.
                $('#tables').append(tmpl('tableIndexTmpl', tableIndex));
                EPSCoR.Tables.toggleButtons(tableIndex.Name, !tableIndex.Processed);

                window.setTimeout(function () {
                    $("#" + tableIndex.Name).addClass('in');
                }, 100);

            },
            //Adds multiple tables to the page.
            addTables: function (tableIndexes) {
                $.each(tableIndexes, function (index, tableIndex) {
                    EPSCoR.Tables.addTable(tableIndex);
                });
            },
            //Updates a table with current information.
            updateTable: function (tableIndex) {
                var $title = $('#' + tableIndex.Name + ' b');
                if (tableIndex.Processed) {
                    $title.text(tableIndex.Name);
                } else {
                    $title.text(tableIndex.Name + ' - ' + tableIndex.Status);
                }
                EPSCoR.Tables.toggleButtons(tableIndex.Name, !tableIndex.Processed);
            },
            //Enables or disables buttons on the given div.
            toggleButtons: function (divId, enable) {
                $('#' + divId + ' .btn').toggleAttr('disabled', enable);
            }
        }
    };

    //After everything has been defned initailize the app.
    //Setup hubs.
    var tableHub = $.connection.tableHub,
        alertHub = $.connection.alertsHub;

    tableHub.client.addTable = EPSCoR.Tables.addTable;
    tableHub.client.updateTable = EPSCoR.Tables.updateTable;
    //tableHub.client.removeTable = removeTable;

    alertHub.client.newAlert = EPSCoR.Alerts.addAlert;

    $.connection.hub.start();

    //Setup dialogs.
    EPSCoR.Dialogs.init();

    //Get initial data.
    $.getJSON('Tables/GetAllDetails', function (tables) {
        EPSCoR.Tables.addTables(tables);
    });
});