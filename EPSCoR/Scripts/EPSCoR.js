$(function () {
    EPSCoR.init();
});

var EPSCoR = EPSCoR || {};

EPSCoR.init = function () {
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
};