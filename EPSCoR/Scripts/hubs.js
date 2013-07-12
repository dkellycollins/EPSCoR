$(function () {
    var tableHub = $.connection.tableHub;

    tableHub.client.addTable = addTable;
    tableHub.client.updateTable = updateTable;
    //tableHub.client.removeTable = removeTable;
    tableHub.client.sendAlert = addAlert;

    $.connection.hub.start();
});