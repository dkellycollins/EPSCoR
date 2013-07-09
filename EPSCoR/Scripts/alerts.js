//Adds an alert message box to the alerts div.
function addAlert(message, header, alertType) {
    var $alert = $('<div/>')
        .addClass('fade alert alert-' + alertType)
        .append('<strong>' + header + '</strong> ' + message)
        .append($('<button type="button" class="close" data-dismiss="alert">x</button>'));

    $('#alerts').append($alert);

    //Delay the fade in.
    window.setTimeout(function () {
        $alert.addClass('in');
    }, 100);
}