//Adds an alert message box to the alerts div.
function addAlert(message, header, alertType) {
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

    return alert;
}