function initHubs() {
    var tableHub = $.connection.tables,
        alertHub = $.connection.alerts;

    tableHub.client.newTable = addTable;
    tableHub.client.updateTable = updateTable;

    alertHub.client.newAlert = addAlert;

    $.connection.hub.start();
}