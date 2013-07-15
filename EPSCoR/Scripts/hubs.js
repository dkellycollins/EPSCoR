$(function () {
    var tableHub = $.connection.tableHub,
        alertHub = $.connection.alertsHub

    tableHub.client.addTable = addTable;
    tableHub.client.updateTable = updateTable;
    //tableHub.client.removeTable = removeTable;

    alertsHub.client.newAlert = addAlert;

    $.connection.hub.start();
});