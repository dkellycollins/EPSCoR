function initHubs() {
    var tableHub = $.connection.tableHub;

    tableHub.client.newTable = addTable;
    tableHub.client.updateTable = updateTable;

    tableHub.client.sendAlert = addAlert;

    $.connection.hub.start();
}